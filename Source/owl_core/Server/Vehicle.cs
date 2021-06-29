using GTANetworkAPI;
using Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dimension = System.UInt32;

// TODO_POST_LAUNCH: Support for being DMV vehicle AND owned by DMV faction
// TODO_POST_LAUNCH: DB should use index not hash? MAYBE? indexes could change, making this a bad idea - Alert Chaos when you do this as it will fuck up the UCP

public class CVehicle : CBaseEntity, IDisposable
{
	// TODO_POST_LAUNCH: These could go out of sync with the actual GTA vehicle, people shouldn't use them. Use the vehicle version instead.
	// TODO_POST_LAUNCH: We need to update these, especially if the player can change the color etc later

	// in sync:
	private bool m_bLocked;
	private bool m_bEngineOn;
	public bool EngineOn
	{
		get => m_bEngineOn;
		set
		{
			if (m_Vehicle != null)
			{
				m_Vehicle.EngineStatus = value;
				SetData(m_Vehicle, EDataNames.ENGINE, value, EDataType.Synced);
			}

			m_bEngineOn = value;
		}
	}

	// callbacks
	private Action<CVehicle, NetHandle> m_OnCreateCallback = null;

	// not in sync:
	private Color m_colPrimary;
	private Color m_colSecondary;
	private VehicleHash m_VehicleHash;
	private Vector3 m_vecPos;
	private Vector3 m_vecRot;
	private string m_strPlateText;
	private EPlateType m_PlateType;
	private int m_WheelColor;
	private int m_Livery;
	private int m_PearlescentColor;
	private float m_Health;
	private EVehicleTransmissionType m_transmissionType;
	public bool m_bTokenPurchase { get; private set; }
	public bool m_bShowPlate { get; set; } = true;
	public bool IsInModShop { get; set; } = false;

	public DateTime LastUsed
	{
		get
		{
			DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return date.AddSeconds(m_LastUsed).ToLocalTime();
		}
	}

	DateTime m_TimeLastTeleport = DateTime.UnixEpoch;
	public void TeleportVehicleOnly(Vector3 vecPos)
	{
		GiveTeleportImmunity();
		GTAInstance.Position = vecPos;
	}

	public void GiveTeleportImmunity()
	{
		m_TimeLastTeleport = DateTime.Now;
	}

	public void TeleportAndWarpOccupants(List<CPlayer> lstOccupants, Vector3 vecPos, uint a_Dimension, Vector3 vecRot)
	{
		if (vecPos == null)
		{
			// TODO_RAGE1.1: We need to log null values for Vector3's
			return;
		}

		GTAInstance.Dimension = a_Dimension;
		GTAInstance.Position = vecPos;
		GTAInstance.Rotation = new Vector3(vecRot.X, vecRot.Y, vecRot.Z);

		Dictionary<int, CPlayer> occupantSeats = new Dictionary<int, CPlayer>();
		foreach (CPlayer occupant in lstOccupants)
		{
			if (occupant != null)
			{
				occupant.SetSafeDimension(a_Dimension);
				occupant.SetPositionSafe(vecPos);
				occupantSeats.Add(occupant.Client.VehicleSeat, occupant);
			}
		}

		// We have to keep retrying this, it sucks, but blame RAGE 1.1
		MainThreadTimerPool.CreateEntityTimer((object[] parameters) =>
		{
			Dictionary<int, CPlayer> occupants = (Dictionary<int, CPlayer>)parameters[0];
			foreach (var occupant in occupants)
			{
				if (occupant.Value.Client.Vehicle == null)
				{
					occupant.Value.Client.SetIntoVehicle(GTAInstance.Handle, occupant.Key);
				}
			}
		}, 50, this, 20, new[] { occupantSeats });
	}

	// With teleport immunity, to avoid deducting large quantities of gas, or adding dirt, odometer etc, we'll still update their pos so it looks like they never moved basically but wont deduct fuel etc...
	public bool HasTeleportImmunity()
	{
		return (DateTime.Now - m_TimeLastTeleport).TotalMinutes <= 1;
	}

	private bool m_bPendingRespawn = false;

	public void ToggleLocked()
	{
		bool newState = !m_bLocked;

		m_bLocked = newState;
		m_Vehicle.Locked = newState;
	}

	public void OnUpdate()
	{
		// sanity check for debugging
		if (GTAInstance == null || GTAInstance.Handle.IsNull)
		{
			if (m_bDebugWasCreated)
			{
				ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
				foreach (var pPlayer in players)
				{
					pPlayer.PushChatMessage(EChatChannel.Global, "DEBUG ERROR: VEHICLE ERROR OCCURED. TELL DANIELS");
				}

				//throw new Exception(Helpers.FormatString("DEBUG ERROR: VEHICLE ERROR OCCURED. TELL DANIELS, VEHICLE ID WAS {0}", m_DatabaseID));
			}
		}
	}

	bool m_bDebugWasCreated = false;

	private void Create(bool bIsRespawn = false)
	{
		NAPI.Task.Run(() =>
		{
			if (m_Vehicle != null)
			{
				m_Vehicle.Delete();
				m_Vehicle = null;
			}

			if (bIsRespawn)
			{
				m_vecPos = m_vecDefaultSpawnPos;
				m_vecRot = m_vecDefaultSpawnRot;
			}

			bool bHasValidColor = m_colPrimary.Red >= 0 && m_colPrimary.Green >= 0 && m_colPrimary.Blue >= 0 &&
				m_colSecondary.Red >= 0 && m_colSecondary.Green >= 0 && m_colSecondary.Blue >= 0;

			if (bHasValidColor)
			{
				m_Vehicle = NAPI.Vehicle.CreateVehicle(m_VehicleHash, m_vecPos, m_vecRot.Z, m_colPrimary, m_colSecondary, m_strPlateText, 255, m_bLocked, EngineOn);
			}
			else
			{
				// Default color
				m_Vehicle = NAPI.Vehicle.CreateVehicle(m_VehicleHash, m_vecPos, m_vecRot.Z, -1, -1, m_strPlateText, 255, m_bLocked, EngineOn);
			}

			m_bDebugWasCreated = true;

			if (HasSiren())
			{
				// TODO: Do we want to save/load this from DB? Probably don't care
				SetData(m_Vehicle, EDataNames.SIREN_STATE, false, EDataType.Synced);
			}

			m_Vehicle.NumberPlateStyle = (int)m_PlateType;
			m_Vehicle.NumberPlate = !IsPlateToggled() ? "" : m_strPlateText;
			m_Vehicle.WheelColor = m_WheelColor;
			m_Vehicle.Livery = m_Livery;
			m_Vehicle.PearlescentColor = m_PearlescentColor;
			m_Vehicle.Health = m_Health;

			m_Vehicle.Dimension = m_DefaultSpawnDimension;

			if (bHasValidColor)
			{
				m_Vehicle.CustomPrimaryColor = new Color(m_colPrimary.Red, m_colPrimary.Green, m_colPrimary.Blue);
				m_Vehicle.CustomSecondaryColor = new Color(m_colSecondary.Red, m_colSecondary.Green, m_colSecondary.Blue);
			}
			else
			{
				// TODO_POST_LAUNCH: error? this seems to cause a client crash if the above doesn't occur
			}

			ApplyDefaultEntityDatas();

			SetData(m_Vehicle, EDataNames.TURNSIGNAL_LEFT, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.TURNSIGNAL_RIGHT, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEH_DOOR_0, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEH_DOOR_1, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEH_DOOR_2, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEH_DOOR_3, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEH_DOOR_4, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEH_DOOR_5, false, EDataType.Synced);
			UpdateVehicleZFix();

			SetData(m_Vehicle, EDataNames.SCRIPTED_ID, m_DatabaseID, EDataType.Synced);

			// TAXI
			if (m_VehicleType == EVehicleType.TaxiJob)
			{
				TaxiSetCostPerMile(1.50f);
				SetData(m_Vehicle, EDataNames.TAXI_DIST, 0.0f, EDataType.Synced);
				SetAvailableForHire(false);
			}

			SetData(m_Vehicle, EDataNames.VEHICLE_TYPE, (int)m_VehicleType, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEHICLE_TRANSMISSION, m_transmissionType, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.VEHICLE_HANDBRAKE, HasHandbrake(), EDataType.Synced);

			// TODO: Apply this from DB
			ApplyHardcodedVehicleCustomizations();

			if (IsPoliceCar())
			{
				SetData(m_Vehicle, EDataNames.SEARCHLIGHT, false, EDataType.Synced);
				SetData(m_Vehicle, EDataNames.SEARCHLIGHT_ROT, 0.0f, EDataType.Synced);
			}

			Fuel = m_fFuel;
			Dirt = m_fDirt;
			Odometer = m_fOdometer;

			CurrentRadioID = m_CurrentRadioID;

			// TODO: Sync from DB
			Headlights = EHeadlightState.Off;

			// Hacky, but re-applies it now the vehicle exists
			EngineOn = EngineOn;

			// Check if we need to tow it in the near future
			CheckForVehicleInsideVehicleStoreNoParkZoneAndCreateTowTimer();

			// apply mods
			ApplyMods();

			// apply extras
			ApplyExtras();

			// neons
			m_Vehicle.Neons = NeonsEnabled;
			m_Vehicle.NeonColor = NeonsColor;

			m_OnCreateCallback?.Invoke(this, GTAInstance.Handle);
		});
	}

	public async Task<bool> CanBeInactive()
	{
		if (m_VehicleType != EVehicleType.PlayerOwned)
		{
			return false;
		}

		bool bProtected = await Database.LegacyFunctions.IsEntityInactivityProtected(m_DatabaseID, EDonationInactivityPurchasables.VehiclePurchasable).ConfigureAwait(true);
		return !bProtected;
	}

	public async Task<bool> IsInactive()
	{
		if (!await CanBeInactive().ConfigureAwait(true))
		{
			return false;
		}

		bool bIsOwnerCharacterInactive = await Database.LegacyFunctions.IsCharacterInactive(m_ownerID).ConfigureAwait(true);

		// Has it not been used in a while?
		DateTime InactiveTime = DateTime.Now.Subtract(TimeSpan.FromDays(InactivityScannerContains.numDaysToConsiderInactiveForUse));
		bool bIsLastUsedConsideredInactive = LastUsed <= InactiveTime;
		return bIsOwnerCharacterInactive || bIsLastUsedConsideredInactive;
	}

	public async void MarkAsUsed()
	{
		m_LastUsed = await Database.LegacyFunctions.UpdateVehicleLastUsedTime(m_DatabaseID).ConfigureAwait(true);
	}

	public CVehicle(long a_VehicleID, EVehicleType a_VehicleType, long a_OwnerID, uint a_Model, Vector3 a_vecDefaultSpawnPos, Vector3 a_vecDefaultSpawnRot, Vector3 a_Pos, Vector3 a_vecRot, EPlateType a_PlateType, string a_strPlateText,
		float a_fFuel, int a_colPrimaryR, int a_colPrimaryG, int a_colPrimaryB, int a_colSecondaryR, int a_colSecondaryG, int a_colSecondaryB, int a_ColWheels, int a_Livery, float a_fDirt, float a_fHealth, bool a_bLocked, bool a_bEngineOn,
		int a_NumPaymentsRemaining, int a_NumPaymentsMade, int a_NumPaymentsMissed, float a_fCreditAmount, Int64 a_ExpiryTime, float a_fOdometer, bool a_bTowed, Dimension a_Dimension, Dictionary<EModSlot, int> a_Mods, Dictionary<int, bool> a_Extras,
		bool bNeonsEnabled, int neon_r, int neon_g, int neon_b, Int64 last_used, int radio, bool show_plate, EVehicleTransmissionType transmission, bool token_purchase, int a_pearlescentColor, Action<CVehicle, NetHandle> OnCreateCallback = null)
	{
		m_OnCreateCallback = OnCreateCallback;

		// TODO: This isn't owned by the resource and wont be destroyed on stop
		// TODO_RAGE: Model is just int, fix in code + db, remove convert etc
		m_bLocked = a_bLocked && (a_VehicleType == EVehicleType.PlayerOwned || IsFactionCar());
		m_colPrimary = new Color(a_colPrimaryR, a_colPrimaryG, a_colPrimaryB);
		m_colSecondary = new Color(a_colSecondaryR, a_colSecondaryG, a_colSecondaryB);
		m_VehicleHash = (VehicleHash)a_Model;
		m_vecPos = a_Pos;
		m_vecRot = a_vecRot;
		m_strPlateText = a_strPlateText;
		EngineOn = a_bEngineOn;
		m_ExpiryTime = a_ExpiryTime;
		m_bTokenPurchase = token_purchase;

		m_bTowed = a_bTowed;
		m_DefaultSpawnDimension = a_Dimension;

		m_PlateType = a_PlateType;
		m_WheelColor = a_ColWheels;
		m_Livery = a_Livery;
		m_PearlescentColor = a_pearlescentColor;
		m_Health = a_fHealth;

		// Not needed for creation
		OwnerID = a_OwnerID;

		m_vecDefaultSpawnPos = a_vecDefaultSpawnPos;
		m_vecDefaultSpawnRot = a_vecDefaultSpawnRot;

		m_iPaymentsRemaining = a_NumPaymentsRemaining;
		m_iPaymentsMade = a_NumPaymentsMade;
		m_iPaymentsMissed = a_NumPaymentsMissed;
		m_fCreditAmount = a_fCreditAmount;

		m_VehicleType = a_VehicleType;

		m_LastUsed = last_used;

		m_DatabaseID = a_VehicleID;

		m_fFuel = a_fFuel;
		m_fDirt = a_fDirt;
		m_fOdometer = a_fOdometer;

		m_CurrentRadioID = radio;
		m_bShowPlate = show_plate;

		m_transmissionType = transmission;

		// Do not auto respawn
		//m_IdleRespawnTimerHandle = MainThreadTimerPool.CreateEntityTimer(IdleRespawnTimer, 300000, this);

		m_Inventory = new CVehicleInventory(this);

		// Setup expiry timer if appropriate
		if (IsRentalCar())
		{
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();

			// Has the expiry timer already passed?
			if (unixTimestamp >= m_ExpiryTime)
			{
				ExpireIfRentalVehicle();
			}
			else
			{
				Int64 OffsetTimestamp = (m_ExpiryTime - unixTimestamp);
				MainThreadTimerPool.CreateEntityTimer(ExpireVehicleTimer, OffsetTimestamp * 1000, this, 1);
			}
		}

		foreach (var kvPair in a_Mods)
		{
			SetMod(kvPair.Key, kvPair.Value, false, false);
		}

		for (int extraID = 0; extraID < VehicleConstants.NumExtras; ++extraID)
		{
			// if it is in the list, set its state
			if (a_Extras.ContainsKey(extraID))
			{
				SetExtra(extraID, a_Extras[extraID], false, false);
			}
			else // otherwise it's inactive by default
			{
				SetExtra(extraID, false, false, false);
			}
		}

		NeonsEnabled = bNeonsEnabled;
		NeonsColor = new Color(neon_r, neon_g, neon_b);

		Create();
	}

	//private readonly WeakReference<MainThreadTimer> m_IdleRespawnTimerHandle = new WeakReference<MainThreadTimer>(null);

	public bool IsRentalCar()
	{
		return m_VehicleType == EVehicleType.RentalCar || m_VehicleType == EVehicleType.FactionOwnedRental;
	}

	public bool IsFactionCar()
	{
		return m_VehicleType == EVehicleType.FactionOwned || m_VehicleType == EVehicleType.FactionOwnedRental;
	}

	public bool HasModel(VehicleHash a_Hash)
	{
		return (VehicleHash)m_Vehicle.Model == a_Hash;
	}

	public bool HasSeatbelts()
	{
		return !GetClass().Equals(EVehicleClass.VehicleClass_Cycles)
			   && !GetClass().Equals(EVehicleClass.VehicleClass_Trains)
			   && !GetClass().Equals(EVehicleClass.VehicleClass_Motorcycles)
			   && !GetClass().Equals(EVehicleClass.VehicleClass_Boats);
	}

	public bool HasHandbrake()
	{
		return !GetClass().Equals(EVehicleClass.VehicleClass_Cycles);
	}

	// TODO_VEHICLES: Better way to do this? Maybe add a flag in vehicle data?
	public bool HasWindows()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		return !GetClass().Equals(EVehicleClass.VehicleClass_Cycles)
			&& !GetClass().Equals(EVehicleClass.VehicleClass_Trains)
			&& !GetClass().Equals(EVehicleClass.VehicleClass_Motorcycles)
			&& !GetClass().Equals(EVehicleClass.VehicleClass_Boats)
			&& vehicleDef.Index != 498 // Hijak Ruston
			&& vehicleDef.Index != 299 // BF Raptor
			&& vehicleDef.Index != 657 // Ocelot Locust
			&& vehicleDef.Index != 674 // Maxwell Vagrant
			&& vehicleDef.Index != 427 // Vapid Trophy Truck
			&& vehicleDef.Index != 673 // Nagasaki Outlaw
			&& vehicleDef.Index != 210 // Canis Kalahari
			&& vehicleDef.Index != 266 // Vapid Peyote
			&& vehicleDef.Index != 572 // Schyster Peyote2
			&& vehicleDef.Index != 526 // Overflod Scramjet
			&& vehicleDef.Index != 651 // Airport Tug
			&& vehicleDef.Index != 80 // Caddy 
			&& vehicleDef.Index != 81 // Another caddy
			&& vehicleDef.Index != 162 // Forklift
			&& vehicleDef.Index != 245 // Lawnmower
			&& vehicleDef.Index != 296 // Dune buggy
			&& vehicleDef.Index != 135 // another dune buggy
			&& vehicleDef.Index != 35 // BF Bifta
			&& vehicleDef.Index != 53 // Canis Bodhi
			&& vehicleDef.Index != 472 // Blazer aqua
			&& vehicleDef.Index != 41 // Quadbike
			&& vehicleDef.Index != 42 // ^
			&& vehicleDef.Index != 43 // ^
			&& vehicleDef.Index != 44 // ^
			&& vehicleDef.Index != 747; // Kart
	}

	public bool IsPlateToggled()
	{
		return m_bShowPlate;
	}

	public string GetPlateText(bool bIgnoreToggledPlate)
	{
		if (IsPlateToggled() || bIgnoreToggledPlate)
		{
			return m_strPlateText;
		}
		else
		{
			return "Unknown Plate";
		}
	}

	public Vector3 GetOffsetPosInFront(float fDist = 1.5f)
	{
		Vector3 vecPosInFront = m_Vehicle.Position;
		float radians = (m_Vehicle.Rotation.Z + 90.0f) * (3.14f / 180.0f);
		vecPosInFront.X += (float)Math.Cos(radians) * fDist;
		vecPosInFront.Y += (float)Math.Sin(radians) * fDist;
		return vecPosInFront;
	}

	private void ApplyHardcodedVehicleCustomizations()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		if (vehicleDef.Hash == (uint)VehicleHash.Police)
		{
			m_Vehicle.SetExtra(1, true);
			m_Vehicle.SetExtra(2, false);
			m_Vehicle.SetExtra(3, false);
		}
		else if (vehicleDef.Hash == (uint)VehicleHash.Taxi)
		{
			for (int i = 1; i <= 9; ++i)
			{
				m_Vehicle.SetExtra(i, false);
			}
		}
	}

	public void TaxiSetCostPerMile(float fCharge)
	{
		SetData(m_Vehicle, EDataNames.TAXI_CPM, fCharge, EDataType.Synced);
	}

	public void TaxiSetCurrentDistance(float fDistance)
	{
		float fCurrentDist = GetData<float>(m_Vehicle, EDataNames.TAXI_DIST);
		if (fCurrentDist != fDistance)
		{
			SetData(m_Vehicle, EDataNames.TAXI_DIST, fDistance, EDataType.Synced);
		}
	}

	public EVehicleTransmissionType GetVehicleTransmissionType()
	{
		return m_transmissionType;
	}

	public void Respawn(bool bForced)
	{
		if (m_bTowed)
		{
			return;
		}

		bool bDoRespawn = true;
		if (!bForced)
		{
			if (m_Vehicle.Occupants.Count == 0)
			{
				// Do we have occupants?
				if (!HasValidOccupant())
				{
					// Is the last leaving player still nearby?
					CPlayer player = m_LastPlayerToLeaveRef.Instance();
					if (player != null)
					{
						if (player.IsWithinDistanceOf(this, 75.0f))
						{
							bDoRespawn = false;
						}
					}
				}
				else
				{
					bDoRespawn = false;
				}
			}
			else
			{
				bDoRespawn = false;
			}
		}

		if (bDoRespawn)
		{
			// Temporary car? Destroy it
			if (m_VehicleType == EVehicleType.Temporary)
			{
				VehiclePool.DestroyVehicle(this);
			}

			// Are we actually far away?
			if (m_Vehicle.Position != m_vecDefaultSpawnPos || m_Vehicle.Rotation.Z != m_vecDefaultSpawnRot.Z)
			{
				ApplyDefaultValues();
				ApplyMods();
				// apply extras
				ApplyExtras();
				m_Vehicle.Position = m_vecDefaultSpawnPos;
				m_Vehicle.Rotation = m_vecDefaultSpawnRot;
				m_Vehicle.Dimension = m_DefaultSpawnDimension;
				UpdateVehicleZFix();
			}
		}

		Save();
	}

	private void UpdateVehicleZFix()
	{
		// RAGE_HACK: Fix for vehicles spawning on roofs when parking underneath
		SetData(m_Vehicle, EDataNames.VEH_Z_FIX, m_Vehicle.Position.Z, EDataType.Synced);
	}

	public void OnPlayerExit(CPlayer a_LeavingPlayer)
	{
		// Did the last player leave?
		// TODO_POST_LAUNCH: What happens when vehicles are destroyed?
		// TODO: last leaving player system needs expansion to other respawn cases (e.g. destruction) + remove from all other vehicles when taking over a new one / leaving a new one
		if (!HasValidOccupant(a_LeavingPlayer))
		{
			m_LastPlayerToLeaveRef.SetTarget(a_LeavingPlayer);
		}

		UpdateVehicleZFix();

		// TODO: Perhaps time delta limit this?
		Save();

		SetData(a_LeavingPlayer.Client, EDataNames.PREVIOUS_VEH, m_DatabaseID, EDataType.Unsynced);

		Log.CreateLog(a_LeavingPlayer, ELogType.VehicleRelated, new List<CBaseEntity>() { this },
			$"Exited a {GetFullDisplayName()}.");

		if (CheckForVehicleInsideVehicleStoreNoParkZoneAndCreateTowTimer())
		{
			a_LeavingPlayer.SendNotification("Vehicle Notice", ENotificationIcon.InfoSign, "Parking this vehicle here is within a prohibited parking zone. Your vehicle will be towed in 30 minutes.");
		}
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		MainThreadTimerPool.DestroyTimersFromParent(this);
	}

	private void ExpireIfRentalVehicle()
	{
		// Can we expire this vehicle type?
		if (IsRentalCar())
		{
			WeakReference<CPlayer> playerRef = PlayerPool.GetPlayerFromCharacterID(m_ownerID);
			CPlayer renterPlayer = playerRef.Instance();

			if (renterPlayer != null)
			{
				renterPlayer.SendNotification("Vehicle Rental", ENotificationIcon.Remove, "Your rental car ({0}) has expired and has been returned to the store.", GetFullDisplayName());
			}

			VehiclePool.DestroyVehicle(this); // NOTE: This also removes it from the DB and deletes all key items
		}
	}

	private void ExpireVehicleTimer(object[] a_Parameters)
	{
		ExpireIfRentalVehicle();
	}

	// TODO_POST_LAUNCH: The player can be the last player to leave of multiple vehicles, when they enter a vehicle, we should remove them from any other vehicle
	private void IdleRespawnTimer(object[] a_Parameters)
	{
		// Can we respawn this vehicle type?
		if (m_VehicleType == EVehicleType.FactionOwned
			|| m_VehicleType == EVehicleType.PlayerOwned
			|| m_VehicleType == EVehicleType.Civilian
			|| m_VehicleType == EVehicleType.TruckerJob
			|| m_VehicleType == EVehicleType.DeliveryDriverJob
			|| m_VehicleType == EVehicleType.BusDriverJob
			|| m_VehicleType == EVehicleType.MailmanJob
			|| m_VehicleType == EVehicleType.RentalCar
			|| m_VehicleType == EVehicleType.Temporary
			|| m_VehicleType == EVehicleType.DrivingTest_Bike
			|| m_VehicleType == EVehicleType.DrivingTest_Car
			|| m_VehicleType == EVehicleType.DrivingTest_Truck
			|| m_VehicleType == EVehicleType.TrashmanJob
			|| m_VehicleType == EVehicleType.TaxiJob
			|| m_VehicleType == EVehicleType.FactionOwnedRental)
		{
			Respawn(false);
		}
	}

	public void SetSpotlightRotation(float a_fRot)
	{
		if (IsPoliceCar())
		{
			SetData(m_Vehicle, EDataNames.SEARCHLIGHT_ROT, a_fRot, EDataType.Synced);
		}
	}

	public void ToggleSpotlight()
	{
		bool currentState = GetData<bool>(m_Vehicle, EDataNames.SEARCHLIGHT);
		SetData(m_Vehicle, EDataNames.SEARCHLIGHT, !currentState, EDataType.Synced);
	}

	private WeakReference<CPlayer> m_LastPlayerToLeaveRef = new WeakReference<CPlayer>(null);

	public void OnPlayerEnter(CPlayer a_EnteringPlayer, int a_Seat)
	{
		MarkAsUsed();
		Log.CreateLog(a_EnteringPlayer, ELogType.VehicleRelated, new List<CBaseEntity>() { this },
			$"Entered a {GetFullDisplayName()}.");

		if (IsRentalCar())
		{
			// TODO: helper function
			System.DateTime ExpiryDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(m_ExpiryTime).ToLocalTime();
			a_EnteringPlayer.SendNotification("Vehicle Info", ENotificationIcon.InfoSign, "This vehicle is a rental car and will be returned to the store on {0} at {1}", ExpiryDateTime.ToLongDateString(), ExpiryDateTime.ToLongTimeString());
		}
	}

	public void TaxiAddDistance(float fDistance)
	{
		float fCurrentDist = GetData<float>(m_Vehicle, EDataNames.TAXI_DIST);
		TaxiSetCurrentDistance(fCurrentDist + fDistance);
	}

	public void SetAvailableForHire(bool bAvailable)
	{
		SetData(m_Vehicle, EDataNames.TAXI_AFH, bAvailable, EDataType.Synced);
	}

	public async void Unimpound(EScriptLocation a_Location)
	{
		VehicleImpoundSpawnPosition targetPos = GetUnimpoundPosition(a_Location);

		m_bTowed = false;
		m_Vehicle.Dimension = 0;
		m_Vehicle.Position = targetPos.Position;
		m_Vehicle.Rotation = targetPos.Rotation;
		UpdateVehicleZFix();

		await Database.LegacyFunctions.SetVehicleTowed(m_DatabaseID, false).ConfigureAwait(true);
		Save();
	}

	private VehicleImpoundSpawnPosition GetUnimpoundPosition(EScriptLocation a_Location)
	{
		const float fDistThresholdForNearby = 2.0f;

		var spawnPositions = a_Location == EScriptLocation.Paleto ? g_SpawnPositions_Paleto : g_SpawnPositions_LS;

		// hoping for the best case and that we can exit fast... after one iter of vehicles... otherwise inverting the loops would actually be faster
		foreach (VehicleImpoundSpawnPosition iterSpawnPosition in spawnPositions)
		{
			bool bThisPosWasInvalidated = false;

			foreach (var vehicle in NAPI.Pools.GetAllVehicles())
			{
				float fDist = (vehicle.Position - iterSpawnPosition.Position).Length();

				if (fDist <= fDistThresholdForNearby)
				{
					bThisPosWasInvalidated = true;
					break;
				}
			}

			if (!bThisPosWasInvalidated)
			{
				// We are good
				return iterSpawnPosition;
			}
		}

		// We didn't find an empty space :( Let's just spawn ontop, its risky but... tally-ho!
		return spawnPositions[0];
	}

	private class VehicleImpoundSpawnPosition
	{
		public VehicleImpoundSpawnPosition(Vector3 vecPos, Vector3 vecRot)
		{
			Position = vecPos;
			Rotation = vecRot;
		}

		public Vector3 Position { get; }
		public Vector3 Rotation { get; }
	}

	private static readonly VehicleImpoundSpawnPosition[] g_SpawnPositions_Paleto = new VehicleImpoundSpawnPosition[]
	{
		new VehicleImpoundSpawnPosition(new Vector3(-0.6143328f, 6342.093f, 31.22907f), new Vector3(0.0f, 0.0f, 207.242f)),
		new VehicleImpoundSpawnPosition(new Vector3(-3.587698f, 6339.727f, 31.22871f), new Vector3(0.0f, 0.0f, 209.6133f)),
		new VehicleImpoundSpawnPosition(new Vector3(-7.073897f, 6336.754f, 31.22846f), new Vector3(0.0f, 0.0f, 210.2131f)),
		new VehicleImpoundSpawnPosition(new Vector3(-9.689834f, 6334.037f, 31.22862f), new Vector3(0.0f, 0.0f, 207.1682f)),
		new VehicleImpoundSpawnPosition(new Vector3(8.717859f, 6350.453f, 31.22679f), new Vector3(0.0f, 0.0f, 207.8749f)),
		new VehicleImpoundSpawnPosition(new Vector3(11.32271f, 6352.329f, 31.22902f), new Vector3(0.0f, 0.0f, 220.4376f))
	};

	private static readonly VehicleImpoundSpawnPosition[] g_SpawnPositions_LS = new VehicleImpoundSpawnPosition[]
	{
		new VehicleImpoundSpawnPosition(new Vector3(241.7362f, -1751.323f, 29.13937f), new Vector3(0.0f, 0.0f, 134.8978f)),
		new VehicleImpoundSpawnPosition(new Vector3(243.6237f, -1753.066f, 29.13937f), new Vector3(0.0f, 0.0f, 131.1853f)),
		new VehicleImpoundSpawnPosition(new Vector3(245.5375f, -1754.788f, 29.13937f), new Vector3(0.0f, 0.0f, 134.9078f)),
		new VehicleImpoundSpawnPosition(new Vector3(247.6295f, -1756.938f, 29.13937f), new Vector3(0.0f, 0.0f, 134.8942f)),
		new VehicleImpoundSpawnPosition(new Vector3(250.2437f, -1759.232f, 29.13937f), new Vector3(0.0f, 0.0f, 141.1369f))
	};


	// TODO: Make this a helper function?
	private bool IsPointInPolygon4(Vector3[] polygon, Vector3 testPoint)
	{
		bool result = false;
		int j = polygon.Length - 1;
		for (int i = 0; i < polygon.Length; i++)
		{
			if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
			{
				if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
				{
					result = !result;
				}
			}
			j = i;
		}
		return result;
	}

	public bool IsBoat()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		if (vehicleDef != null)
		{
			return vehicleDef.Class == EVehicleClass.VehicleClass_Boats.ToString().Replace("VehicleClass_", "");
		}

		return false;
	}

	public async Task<bool> ScriptTow()
	{
		// TODO_BOATS: What if someone parks in the marina where bought boats spawn?
		// Dont tow boats
		if (IsBoat())
		{
			return false;
		}

		// don't tow if dimension > 0 (its an interior, let them do whatever they want)
		if (m_DefaultSpawnDimension > 0)
		{
			return false;
		}

		if (m_Vehicle.Occupants.Count == 0)
		{
			// Do we have occupants?
			if (!HasValidOccupant())
			{
				// Is the last leaving player still nearby?
				bool bDoTow = true;
				CPlayer player = m_LastPlayerToLeaveRef.Instance();
				if (player != null)
				{
					if (player.IsWithinDistanceOf(this, 250.0f))
					{
						bDoTow = false;
					}
				}

				if (bDoTow)
				{
					// dont tow certain vehicle type but respawn instead
					if (VehicleType == EVehicleType.FactionOwned
					|| VehicleType == EVehicleType.FactionOwnedRental
					|| VehicleType == EVehicleType.Civilian
					|| VehicleType == EVehicleType.TruckerJob
					|| VehicleType == EVehicleType.DeliveryDriverJob
					|| VehicleType == EVehicleType.BusDriverJob
					|| VehicleType == EVehicleType.MailmanJob
					|| VehicleType == EVehicleType.DrivingTest_Bike
					|| VehicleType == EVehicleType.DrivingTest_Car
					|| VehicleType == EVehicleType.DrivingTest_Truck
					|| VehicleType == EVehicleType.TrashmanJob
					|| VehicleType == EVehicleType.TaxiJob)
					{
						Respawn(true);
					}
					else if (VehicleType == EVehicleType.Temporary) // we delete these ones
					{
						VehiclePool.DestroyVehicle(this);
					}
					else // PlayerOwned, RentalCar, 
					{
						// inform the owner if online
						if (!m_bTowed)
						{
							var ownerPlayer = PlayerPool.GetPlayerFromCharacterID(m_ownerID);
							if (ownerPlayer != null && ownerPlayer.Instance() != null)
							{
								ownerPlayer.Instance().SendNotification("Vehicle Notice", ENotificationIcon.ExclamationSign, "Your {0} was towed for improper parking. Visit the impound to retrieve your vehicle (Tow icon).", GetFullDisplayName());
							}
						}

						// remove towing timer if we have one
						if (m_VehicleStoreZoneTimer.Instance() != null)
						{
							MainThreadTimerPool.MarkTimerForDeletion(m_VehicleStoreZoneTimer);
						}

						// move to towed car dimension
						m_bTowed = true;
						m_Vehicle.Dimension = Constants.TowedCarDimension;
						m_Vehicle.Position = new Vector3(-1.5f, 19.25f, 71.11388f);

						await Database.LegacyFunctions.SetVehicleTowed(m_DatabaseID, true).ConfigureAwait(true);
						Save();
					}
				}

				return bDoTow;
			}
		}

		Save();

		return false;
	}

	public bool IsTowed()
	{
		return m_bTowed;
	}

	private WeakReference<MainThreadTimer> m_VehicleStoreZoneTimer = new WeakReference<MainThreadTimer>(null);
	public bool IsWithinVehicleStoreNoParkZone()
	{
		Vector3[] vecPolygonPaleto = new Vector3[4]
		{
			new Vector3(-195.1457f, 6241.162f, 31.49206f),
			new Vector3(-175.7812f, 6224.935f, 31.48926f),
			new Vector3(-228.7754f, 6172.316f, 31.44963f),
			new Vector3(-242.9487f, 6189.019f, 31.48921f),
		};

		Vector3[] vecPolygonLS = new Vector3[4]
		{
			new Vector3(-65.07064, -1119.403, 26.51048),
			new Vector3(-64.03498, -1100.771, 26.2026),
			new Vector3(-36.6616, -1111.383, 26.43813),
			new Vector3(-39.42719, -1120.924, 26.74602),
		};

		return IsPointInPolygon4(vecPolygonPaleto, m_Vehicle.Position) || IsPointInPolygon4(vecPolygonLS, m_Vehicle.Position);
	}


	public bool CheckForVehicleInsideVehicleStoreNoParkZoneAndCreateTowTimer()
	{
		bool bRet = false;
		if (IsWithinVehicleStoreNoParkZone())
		{
			if (m_VehicleStoreZoneTimer.Instance() == null)
			{
				m_VehicleStoreZoneTimer = MainThreadTimerPool.CreateEntityTimer(async (object[] parameters) =>
				{
					// Check we are still in the zone when the timer procs
					if (IsWithinVehicleStoreNoParkZone())
					{
						bool bWasTowed = await ScriptTow().ConfigureAwait(true);

						// If we didnt tow (e.g. player was still nearby), but they WERE still within the zone as per the check above, re-create the timer so we can try again
						m_VehicleStoreZoneTimer.SetTarget(null);
						CheckForVehicleInsideVehicleStoreNoParkZoneAndCreateTowTimer();
					}
				}, 1800000, this, 1);
			}

			bRet = true;
		}
		else
		{
			if (m_VehicleStoreZoneTimer.Instance() != null) // we did park inside before, and had a timer, but we since parked outside
			{
				MainThreadTimerPool.MarkTimerForDeletion(m_VehicleStoreZoneTimer);
			}
		}

		return bRet;
	}

	public void ApplyDefaultEntityDatas()
	{
		// Only add things here that arent loaded from DB.
		if (IsPoliceCar())
		{
			SetData(m_Vehicle, EDataNames.IS_POLICE_VEHICLE, true, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.SEARCHLIGHT, false, EDataType.Synced);
			SetData(m_Vehicle, EDataNames.SEARCHLIGHT_ROT, 0.0f, EDataType.Synced);
		}
	}

	public void ApplyDefaultValues()
	{
		EngineOn = false;
		m_Vehicle.Locked = m_VehicleType == EVehicleType.PlayerOwned || IsFactionCar();
		m_Vehicle.SpecialLight = false;
		m_Vehicle.Repair();

		Fuel = 100.0f;

		SetData(m_Vehicle, EDataNames.TURNSIGNAL_LEFT, false, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.TURNSIGNAL_RIGHT, false, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.HEADLIGHTS, EHeadlightState.Off, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.VEH_DOOR_0, false, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.VEH_DOOR_1, false, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.VEH_DOOR_2, false, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.VEH_DOOR_3, false, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.VEH_DOOR_4, false, EDataType.Synced);
		SetData(m_Vehicle, EDataNames.VEH_DOOR_5, false, EDataType.Synced);

		ApplyDefaultEntityDatas();
	}

	public EVehicleClass GetClass()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		if (vehicleDef != null)
		{
			if (Enum.TryParse(typeof(EVehicleClass), Helpers.FormatString("VehicleClass_{0}", vehicleDef.Class), out object vehicleClass))
			{
				return (EVehicleClass)vehicleClass;
			}

		}

		return EVehicleClass.VehicleClass_Compacts;
	}

	public bool HasSiren()
	{
		return GetClass() == EVehicleClass.VehicleClass_Emergency;
	}

	public bool IsAircraft()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		return GetClass().Equals(EVehicleClass.VehicleClass_Planes) || GetClass().Equals(EVehicleClass.VehicleClass_Helicopters) || vehicleDef.Hash == (uint)VehicleHash.Polmav;
	}

	public bool IsPoliceCar()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		if (vehicleDef == null)
		{
			return false;
		}

		return (
			vehicleDef.Hash == (uint)VehicleHash.Police
			|| vehicleDef.Hash == (uint)VehicleHash.Police2
			|| vehicleDef.Hash == (uint)VehicleHash.Police3
			|| vehicleDef.Hash == (uint)VehicleHash.Police4
			|| vehicleDef.Hash == (uint)VehicleHash.Policeb
			|| vehicleDef.Hash == (uint)VehicleHash.Policeold1
			|| vehicleDef.Hash == (uint)VehicleHash.Policeold2
			|| vehicleDef.Hash == (uint)VehicleHash.Policet
			|| vehicleDef.Hash == (uint)VehicleHash.Riot
			|| vehicleDef.Hash == (uint)VehicleHash.Sheriff
			|| vehicleDef.Hash == (uint)VehicleHash.Sheriff2
			|| vehicleDef.Hash == (uint)VehicleHash.Fbi
			|| vehicleDef.Hash == (uint)VehicleHash.Fbi2
			|| IsPoliceCustomPickup()
			|| (vehicleDef.Hash == (uint)VehicleHash.Polmav && GTAInstance.Livery == 0) // Livery 0 is the PD, otherwise its FD
																						// begin custom vehicles
			|| vehicleDef.Hash == 948349681 // lspdsuv
			|| vehicleDef.Hash == 583746231 // lspdsuv2
			|| vehicleDef.Hash == 757287879 // pcruiser
			|| vehicleDef.Hash == 310841658 // pcruiser2
			|| vehicleDef.Hash == 2883648200 // pinter
			|| vehicleDef.Hash == 1229349319 // spsuv
			|| vehicleDef.AddOnName == "pscout" // police explorer
			|| vehicleDef.AddOnName == "umkscout" // uc explorer
			|| vehicleDef.Hash == 1721453765 // uc hsiu (dominator)
			|| vehicleDef.Hash == 418642148 // K9 Explorer
			|| vehicleDef.Hash == 143492960 // SD Explorer 
			|| vehicleDef.Hash == 3477067732 // SAHP Explorer 
			|| vehicleDef.Hash == 3693426662 // Rockford PD explorer 
			|| vehicleDef.AddOnName == "trualamo"
			|| vehicleDef.AddOnName == "trualamo2"
			|| vehicleDef.AddOnName == "lspdcara"
			|| vehicleDef.AddOnName == "lspdcara2"
			|| vehicleDef.AddOnName == "polthrust"
			|| vehicleDef.AddOnName == "poltaxi2"
			|| vehicleDef.AddOnName == "mclarenpd"
			);
	}

	private bool PDVehicleIsExemptFromHandlingMods()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		if (vehicleDef == null)
		{
			return false;
		}

		return (vehicleDef.Hash == 1887487254 // police explorer
				|| vehicleDef.AddOnName == "lspdcara"
				|| vehicleDef.AddOnName == "lspdcara2"
			);
	}

	public bool IsUndercoverCarWithWindowTint()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		if (vehicleDef != null)
		{
			if (vehicleDef.Hash == (uint)2945871676
				|| vehicleDef.Hash == (uint)2883648200
				|| vehicleDef.Hash == (uint)1887487254
				|| vehicleDef.Hash == (uint)3415773196
				|| vehicleDef.Hash == 1721453765
				|| vehicleDef.AddOnName == "trualamo"
				|| vehicleDef.AddOnName == "trualamo2"
				|| vehicleDef.AddOnName == "lspdcara"
				|| vehicleDef.AddOnName == "lspdcara2"
				|| vehicleDef.AddOnName == "polthrust")
			{
				CFaction faction = GetFactionOwner();
				if (faction != null)
				{
					if (faction.Type == EFactionType.LawEnforcement)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool IsPoliceCustomPickup()
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
		if (vehicleDef != null)
		{
			if (vehicleDef.Hash == (uint)2945871676) // check dlc pickup
			{
				CFaction faction = GetFactionOwner();
				if (faction != null)
				{
					if (faction.Type == EFactionType.LawEnforcement)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public string GetFullDisplayName()
	{
		if (GTAInstance != null)
		{
			CVehicleDefinition addonVehicle = VehicleDefinitions.GetVehicleDefinitionFromAddon(m_Vehicle.Model);
			if (addonVehicle != null)
			{
				return Helpers.FormatString("{0} {1}", addonVehicle.Manufacturer, addonVehicle.Name);
			}
			else
			{
				CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
				if (vehicleDef != null)
				{
					return Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
				}
			}
		}

		return String.Empty;
	}

	public Color GetRGBPrimaryColor()
	{
		Color primaryColor = this.m_colPrimary;
		return primaryColor;
	}

	public string GetFullDisplayNameWithColor()
	{
		if (GTAInstance != null)
		{
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(GTAInstance.Model);
			if (vehicleDef != null)
			{
				return Helpers.FormatString("{0} {1} {2}", vehicleDef.Manufacturer, vehicleDef.Name, GetColorsDisplayString());
			}
		}

		return String.Empty;
	}

	public string GetColorsDisplayString()
	{
		Tuple<string, string> strColorNames = GetColorNames();
		const string strUnknown = "Unknown";
		string strReturnValue = "";

		if (strColorNames.Item1 != strUnknown && strColorNames.Item2 != strUnknown)
		{
			strReturnValue = Helpers.FormatString("{0} & {1}", strColorNames.Item1, strColorNames.Item2);
		}
		else if (strColorNames.Item1 != strUnknown)
		{
			strReturnValue = strColorNames.Item1;
		}
		else if (strColorNames.Item2 != strUnknown)
		{
			strReturnValue = strColorNames.Item2;
		}

		return strReturnValue;
	}

	public Tuple<string, string> GetColorNames()
	{
		string strPrimaryName = "Unknown";
		string strSecondaryName = "Unknown";

		try
		{
			System.Drawing.Color primaryColor = System.Drawing.Color.FromArgb(m_Vehicle.CustomPrimaryColor.Red, m_Vehicle.CustomPrimaryColor.Green, m_Vehicle.CustomPrimaryColor.Blue);
			System.Drawing.Color secondaryColor = System.Drawing.Color.FromArgb(m_Vehicle.CustomSecondaryColor.Red, m_Vehicle.CustomSecondaryColor.Green, m_Vehicle.CustomSecondaryColor.Blue);

			if (primaryColor.IsNamedColor && secondaryColor.IsNamedColor)
			{
				strPrimaryName = primaryColor.Name;
				strSecondaryName = secondaryColor.Name;
			}
			else if (primaryColor.IsNamedColor)
			{
				strPrimaryName = primaryColor.Name;
			}
			else if (secondaryColor.IsNamedColor)
			{
				strSecondaryName = secondaryColor.Name;
			}
		}
		catch
		{

		}

		return Tuple.Create(strPrimaryName, strSecondaryName);
	}

	public Vehicle GTAInstance => m_Vehicle;

	// TODO_POST_LAUNCH: Load from DB?
	public EHeadlightState Headlights
	{
		get => GetData<EHeadlightState>(m_Vehicle, EDataNames.HEADLIGHTS);
		set => SetData(m_Vehicle, EDataNames.HEADLIGHTS, value, EDataType.Synced);
	}

	public bool DoesVehicleConsumeFuel()
	{
		EVehicleClass VehicleClass = (EVehicleClass)m_Vehicle.Class;
		return (VehicleClass != EVehicleClass.VehicleClass_Cycles
			&& VehicleClass != EVehicleClass.VehicleClass_Helicopters
			&& VehicleClass != EVehicleClass.VehicleClass_Planes
			&& VehicleClass != EVehicleClass.VehicleClass_Trains);
	}

	public bool DoesVehicleGetDirty()
	{
		EVehicleClass VehicleClass = (EVehicleClass)m_Vehicle.Class;
		return (VehicleClass != EVehicleClass.VehicleClass_Helicopters
			&& VehicleClass != EVehicleClass.VehicleClass_Planes
			&& VehicleClass != EVehicleClass.VehicleClass_Trains);
	}

	/*
	public Task AutoPark(CPlayer a_Player)
	{
		if (m_VehicleType == EVehicleType.PlayerOwned || m_VehicleType == EVehicleType.RentalCar)
		{
			if (HasKeys(a_Player) || OwnedBy(a_Player))
			{
				Database.Functions.Vehicles.Park(m_DatabaseID, m_Vehicle.Position, m_Vehicle.Rotation);
				m_vecDefaultSpawnPos = m_Vehicle.Position;
				m_vecDefaultSpawnRot = m_Vehicle.Rotation;

				Save();
			}
		}
	}
	*/

	public void ManualPark(CPlayer a_Player, bool bSendNotification = true)
	{
		if (a_Player == null) // cant do permission checks without a player
		{
			return;
		}

		if (m_VehicleType == EVehicleType.PlayerOwned
			|| m_VehicleType == EVehicleType.FactionOwned
			|| m_VehicleType == EVehicleType.RentalCar
			|| m_VehicleType == EVehicleType.FactionOwnedRental)
		{
			bool bHasFactionManagerForVehicle = IsVehicleForAnyPlayerFaction(a_Player, true);

			if (bHasFactionManagerForVehicle || a_Player.IsAdmin() || OwnedOrRentedBy(a_Player))
			{
				Database.Functions.Vehicles.Park(m_DatabaseID, m_Vehicle.Position, m_Vehicle.Rotation, m_Vehicle.Dimension);
				m_vecDefaultSpawnPos = m_Vehicle.Position;
				m_vecDefaultSpawnRot = m_Vehicle.Rotation;
				m_DefaultSpawnDimension = m_Vehicle.Dimension;
				if (bSendNotification)
				{
					a_Player.SendNotification("Vehicle", ENotificationIcon.InfoSign, "Your vehicle has been parked.", null);
				}

				Save();
			}
			else if (bSendNotification)
			{
				a_Player.SendNotification("Vehicle", ENotificationIcon.InfoSign, "Your do not have the permissions required to park this vehicle.", null);
			}
		}
	}

	public void AdminPark(CPlayer a_Player)
	{
		if (m_VehicleType != EVehicleType.Temporary)
		{
			if (a_Player.IsAdmin())
			{
				Database.Functions.Vehicles.Park(m_DatabaseID, m_Vehicle.Position, m_Vehicle.Rotation, m_Vehicle.Dimension);
				m_vecDefaultSpawnPos = m_Vehicle.Position;
				m_vecDefaultSpawnRot = m_Vehicle.Rotation;
				m_DefaultSpawnDimension = m_Vehicle.Dimension;
				a_Player.SendNotification("Vehicle", ENotificationIcon.InfoSign, "This vehicle has been parked.", null);

				Save();
			}
		}
		else
		{
			a_Player.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "This vehicle is a temporary vehicle and cannot be parked.", null);
		}
	}

	public void Save()
	{
		if (!m_bPendingRespawn) // FIX: Stops us saving it on respawnall, meaning it wouldn't actually repsawn :)
		{
			if (m_VehicleType != EVehicleType.Temporary)
			{
				// Can't save if in mod shop
				if (!IsInModShop)
				{
					Database.Functions.Vehicles.Save(m_DatabaseID, m_Vehicle.Position, m_Vehicle.Rotation, m_fFuel, Dirt, m_Vehicle.Health, m_Vehicle.Locked, EngineOn, m_fOdometer, m_Vehicle.Dimension);
				}
			}
		}
	}

	public float Fuel
	{
		get => m_fFuel;
		set
		{
			SetData(m_Vehicle, EDataNames.FUEL, value, EDataType.Synced);
			m_fFuel = value;
		}
	}

	public float Dirt
	{
		get => m_fDirt;
		set
		{
			SetData(m_Vehicle, EDataNames.DIRT, value, EDataType.Synced);
			m_fDirt = value;
		}
	}

	public float Odometer
	{
		get => m_fOdometer;
		set
		{
			SetData(m_Vehicle, EDataNames.ODOMETER, value, EDataType.Synced);
			m_fOdometer = value;
		}
	}

	public bool IsJobVehicle()
	{
		return (m_VehicleType == EVehicleType.TruckerJob
			|| m_VehicleType == EVehicleType.DeliveryDriverJob
			|| m_VehicleType == EVehicleType.BusDriverJob
			|| m_VehicleType == EVehicleType.MailmanJob
			|| m_VehicleType == EVehicleType.TrashmanJob
			|| m_VehicleType == EVehicleType.TaxiJob);
	}

	public bool IsDMVVehicle()
	{
		return (m_VehicleType == EVehicleType.DrivingTest_Bike
			|| m_VehicleType == EVehicleType.DrivingTest_Car
			|| m_VehicleType == EVehicleType.DrivingTest_Truck);
	}

	public bool IsVehicleForJob(EJobID a_JobID)
	{
		if (m_VehicleType == EVehicleType.TruckerJob && a_JobID == EJobID.TruckerJob)
		{
			return true;
		}
		else if (m_VehicleType == EVehicleType.DeliveryDriverJob && a_JobID == EJobID.DeliveryDriverJob)
		{
			return true;
		}
		else if (m_VehicleType == EVehicleType.BusDriverJob && a_JobID == EJobID.BusDriverJob)
		{
			return true;
		}
		else if (m_VehicleType == EVehicleType.MailmanJob && a_JobID == EJobID.MailmanJob)
		{
			return true;
		}
		else if (m_VehicleType == EVehicleType.TrashmanJob && a_JobID == EJobID.TrashmanJob)
		{
			return true;
		}
		else if (m_VehicleType == EVehicleType.TaxiJob && a_JobID == EJobID.TaxiDriverJob)
		{
			return true;
		}

		return false;
	}


	public bool IsVehicleForDrivingTest(CPlayer a_Player)
	{
		EDrivingTestType testTypeToCheck = EDrivingTestType.None;

		if (VehicleType == EVehicleType.DrivingTest_Bike)
		{
			testTypeToCheck = EDrivingTestType.Bike;
		}
		else if (VehicleType == EVehicleType.DrivingTest_Car)
		{
			testTypeToCheck = EDrivingTestType.Car;
		}
		else if (VehicleType == EVehicleType.DrivingTest_Truck)
		{
			testTypeToCheck = EDrivingTestType.Truck;
		}

		if (testTypeToCheck != EDrivingTestType.None)
		{
			return a_Player.CurrentDrivingTestType == testTypeToCheck;
		}

		return false;
	}

	public bool IsVehicleForAnyPlayerFaction(CPlayer a_Player, bool a_bManagerOnly = false, bool a_bOfficialFactionsOnly = false)
	{
		if (!IsFactionCar())
		{
			return false;
		}

		foreach (CFactionMembership factionMembership in a_Player.GetFactionMemberships())
		{
			if (OwnerID == factionMembership.Faction.FactionID)
			{
				if (!a_bManagerOnly || (a_bManagerOnly && factionMembership.Manager))
				{
					if (!a_bOfficialFactionsOnly || (a_bOfficialFactionsOnly && factionMembership.Faction.Type != EFactionType.UserCreated))
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool IsVehiclePDorFDVehicle()
	{
		if (!IsFactionCar())
		{
			return false;
		}

		CFaction fdems = FactionPool.GetFDEMSFaction();
		CFaction pd = FactionPool.GetPoliceFaction();

		if (OwnerID == fdems.FactionID || OwnerID == pd.FactionID)
		{
			return true;
		}

		return false;
	}


	public void ToggleLeftTurnSignal()
	{
		SetData(m_Vehicle, EDataNames.TURNSIGNAL_LEFT, !GetData<bool>(m_Vehicle, EDataNames.TURNSIGNAL_LEFT), EDataType.Synced);
	}

	public void ToggleRightTurnSignal()
	{
		SetData(m_Vehicle, EDataNames.TURNSIGNAL_RIGHT, !GetData<bool>(m_Vehicle, EDataNames.TURNSIGNAL_RIGHT), EDataType.Synced);
	}

	public bool VehicleWindowState()
	{
		return GetData<bool>(m_Vehicle, EDataNames.VEHICLE_WINDOWS);
	}

	public void SetWindowState(bool bOpen)
	{
		SetData(m_Vehicle, EDataNames.VEHICLE_WINDOWS, bOpen, EDataType.Synced);
	}

	public void ToggleWindowState(CPlayer a_RequestingPlayer)
	{
		bool bNewState = !GetData<bool>(m_Vehicle, EDataNames.VEHICLE_WINDOWS);
		SetWindowState(bNewState);
		HelperFunctions.Chat.SendAmeMessage(a_RequestingPlayer, Helpers.FormatString("{0}", bNewState ? "rolls down their windows" : "rolls their windows up"));
	}

	public void SetDoorState(EDataNames dataName, bool bOpen)
	{
		SetData(m_Vehicle, dataName, bOpen, EDataType.Synced);
	}

	public void ToggleDoorState(CPlayer a_RequestingPlayer, int doorID)
	{
		if (doorID < 0 || doorID > 5)
		{
			return;
		}

		string strDoorName = "Driver's side front door";
		EDataNames dataNameToUse = EDataNames.VEH_DOOR_0;
		if (doorID == 1)
		{
			strDoorName = "Passenger's side front door";
			dataNameToUse = EDataNames.VEH_DOOR_1;
		}
		else if (doorID == 2)
		{
			strDoorName = "Driver's side rear door";
			dataNameToUse = EDataNames.VEH_DOOR_2;
		}
		else if (doorID == 3)
		{
			strDoorName = "Passenger's side rear door";
			dataNameToUse = EDataNames.VEH_DOOR_3;
		}
		else if (doorID == 4)
		{
			strDoorName = "hood";
			dataNameToUse = EDataNames.VEH_DOOR_4;
		}
		else if (doorID == 5)
		{
			strDoorName = "trunk";
			dataNameToUse = EDataNames.VEH_DOOR_5;
		}

		bool bNewState = !GetData<bool>(m_Vehicle, dataNameToUse);
		SetDoorState(dataNameToUse, bNewState);

		HelperFunctions.Chat.SendAmeMessage(a_RequestingPlayer, Helpers.FormatString("{0} the {1}.", bNewState ? "opens" : "closes", strDoorName));
	}

	public uint GetModelHash()
	{
		return m_Vehicle.Model;
	}

	public void Repossess()
	{
		if (m_VehicleType == EVehicleType.PlayerOwned || m_VehicleType == EVehicleType.RentalCar)
		{
			Log.CreateLog(OwnerID, EOriginType.Character, ELogType.VehicleRelated, new List<CBaseEntity>() { this },
				$"INACTIVITY SCANNER FORCESOLD {GetFullDisplayName()}.");
		}

		VehiclePool.DestroyVehicle(this); // This function also removes it from the DB

		// TODO: Later, we should actually put the vehicle back on the market for 'used vehicles' but don't let original owner re-buy it on his account
	}

	public CFaction GetFactionOwner()
	{
		if (VehicleType == EVehicleType.FactionOwned || VehicleType == EVehicleType.FactionOwnedRental) // Faction vehicles belonging to official factions, must be entered by a faction member only
		{
			CFaction factionInst = FactionPool.GetFactionFromID(OwnerID);
			if (factionInst != null)
			{
				return factionInst;
			}
		}

		return null;
	}

	public void SetOwner(EVehicleType newVehicleType, long newOwner)
	{
		VehicleType = newVehicleType;
		OwnerID = newOwner;

		Database.LegacyFunctions.SetVehicleOwner(m_DatabaseID, VehicleType, OwnerID);

	}

	public void GetOwnerText(Action<string> AsyncCallback)
	{
		if (VehicleType == EVehicleType.FactionOwned || VehicleType == EVehicleType.FactionOwnedRental)
		{
			CFaction factionInst = GetFactionOwner();
			if (factionInst != null)
			{
				AsyncCallback(factionInst.Name);
			}
			else
			{
				AsyncCallback("Unowned");
			}
		}
		else if (VehicleType == EVehicleType.PlayerOwned || VehicleType == EVehicleType.RentalCar)
		{
			Database.Functions.Characters.Get(-1, OwnerID, false, (SGetCharacter owner) =>
			{
				if (owner != null)
				{
					AsyncCallback(owner.CharacterName);
				}
				else
				{
					AsyncCallback("Unowned");
				}
			});
		}
		else
		{
			AsyncCallback("Unowned");
		}
	}

	public bool OwnedBy(CPlayer player)
	{
		return m_VehicleType == EVehicleType.PlayerOwned && OwnerID == player.ActiveCharacterDatabaseID;
	}

	public bool OwnedOrRentedBy(CPlayer player)
	{
		return (m_VehicleType == EVehicleType.PlayerOwned || m_VehicleType == EVehicleType.RentalCar) && OwnerID == player.ActiveCharacterDatabaseID;
	}

	public bool OwnedBy(CFaction faction)
	{
		return IsFactionCar() && OwnerID == faction.FactionID;
	}

	public bool HasKeys(CPlayer player)
	{
		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, m_DatabaseID);
		return player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
	}

	public bool OwnedOrRentedByCharacterID(long characterID)
	{
		return (m_VehicleType == EVehicleType.PlayerOwned || m_VehicleType == EVehicleType.RentalCar) && OwnerID == characterID;
	}

	public bool OwnedByFactionID(long factionID)
	{
		return IsFactionCar() && OwnerID == factionID;
	}

	// NOTE: Do not use this. Use vehicle pool.
	public async void Destroy(bool DeleteFromDatabase = false)
	{
		m_bDebugWasCreated = false;

		if (GTAInstance != null && GTAInstance.Handle != null)
		{
			NAPI.Task.Run(() =>
			{
				NAPI.Entity.DeleteEntity(GTAInstance.Handle);
			});
		}

		if (DeleteFromDatabase && m_VehicleType != EVehicleType.Temporary)
		{
			await Database.LegacyFunctions.DestroyVehicle(m_DatabaseID).ConfigureAwait(true);
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, m_DatabaseID);
			await HelperFunctions.Items.DeleteAllItems(ItemInstanceDef).ConfigureAwait(true);
		}
	}

	public bool HasValidOccupant(CPlayer a_IgnorePlayer = null)
	{
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (CPlayer player in players)
		{
			if (player.IsInVehicleReal && player.Client.Vehicle == m_Vehicle && player != a_IgnorePlayer)
			{
				return true;
			}
		}

		return false;
	}

	private Vehicle m_Vehicle;
	private EVehicleType m_VehicleType;

	private Int64 m_LastUsed;

	public bool IsBeingWashed { get; set; } = false;

	public void ApplyMods()
	{

		// police vehicles have hardcoded mods
		if (IsPoliceCar())
		{
			// set tint always
			if (PDVehicleIsExemptFromHandlingMods())
			{
				m_Vehicle.SetMod((int)EModSlot.Engine, 2);
				m_Vehicle.SetMod((int)EModSlot.Brakes, 1);
				m_Vehicle.SetMod((int)EModSlot.Transmission, 1);
				m_Vehicle.SetMod((int)EModSlot.Suspension, 2);
				m_Vehicle.SetMod((int)EModSlot.Turbo, 0);
				m_Vehicle.SetMod((int)EModSlot.Horns, 1);
			}
			else
			{
				// NOTE: Marked explorer becomes very undriveable with engine 2
				//m_Vehicle.SetMod((int)EModSlot.Engine, 1);
				m_Vehicle.SetMod((int)EModSlot.Brakes, 3);
				m_Vehicle.SetMod((int)EModSlot.Transmission, 3);
				m_Vehicle.SetMod((int)EModSlot.Suspension, 4);
				//m_Vehicle.SetMod((int)EModSlot.Turbo, 1);
				m_Vehicle.SetMod((int)EModSlot.Horns, 1);
			}


			// suv needs tint 2
			if (m_Vehicle.Model == 1887487254)
			{
				m_Vehicle.WindowTint = 2;
			}
			else if (IsUndercoverCarWithWindowTint())
			{
				m_Vehicle.WindowTint = 1;
			}
			else
			{
				m_Vehicle.WindowTint = 3;
			}



			// add extras
			for (int i = 0; i < 32; ++i)
			{
				// UC HSIU cant have 1 and 2, its broken
				if (m_Vehicle.Model == 1721453765)
				{
					if (i == 1 || i == 2)
					{
						continue;
					}
				}

				m_Vehicle.SetExtra(i, true);
			}

			if (IsPoliceCustomPickup())
			{
				m_Vehicle.SetMod((int)EModSlot.FrontBumper, 5);
				m_Vehicle.SetMod((int)EModSlot.RearBumper, 1);
				m_Vehicle.SetMod((int)EModSlot.Hood, 2);
				m_Vehicle.WindowTint = 2;
			}

		}
		else
		{
			// Remove all
			foreach (EModSlot modSlot in Enum.GetValues(typeof(EModSlot)))
			{
				//m_Vehicle.RemoveMod((int)modSlot);
				m_Vehicle.SetMod((int)modSlot, 255);
			}

			// Reapply
			foreach (var kvPair in m_ActiveVehicleMods)
			{
				if (kvPair.Key == EModSlot.WindowTint)
				{
					m_Vehicle.WindowTint = kvPair.Value;
				}
				else
				{
					m_Vehicle.SetMod((int)kvPair.Key, kvPair.Value);

				}
			}

			// plate style + text (we reapply this incase player changed it locally in the ui)
			m_Vehicle.NumberPlateStyle = m_Vehicle.NumberPlateStyle;
			m_Vehicle.NumberPlate = m_Vehicle.NumberPlate;

			// neons
			m_Vehicle.Neons = NeonsEnabled;
			m_Vehicle.NeonColor = new Color(NeonsColor.Red, NeonsColor.Green, NeonsColor.Blue);
		}
	}

	public async void SetMod(EModSlot slot, int modIndex, bool bStoreInDB, bool bApplyInstantly)
	{
		m_ActiveVehicleMods[slot] = modIndex;

		if (bStoreInDB)
		{
			await Database.LegacyFunctions.SetVehicleMod(m_DatabaseID, slot, modIndex).ConfigureAwait(true);
		}

		if (bApplyInstantly)
		{
			ApplyExtras();
		}
	}

	public async void SetExtra(int extraID, bool bEnabled, bool bStoreInDB, bool bApplyInstantly)
	{
		if (m_VehicleExtras.ContainsKey(extraID))
		{
			if (m_VehicleExtras[extraID] == bEnabled)
			{
				// no change, so nothing to save
				return;
			}
		}

		m_VehicleExtras[extraID] = bEnabled;

		if (m_VehicleType != EVehicleType.Temporary)
		{
			if (bStoreInDB)
			{
				await Database.LegacyFunctions.SetVehicleExtra(m_DatabaseID, extraID, bEnabled).ConfigureAwait(true);
			}
		}

		if (bApplyInstantly)
		{
			ApplyExtras();
		}
	}

	private void ApplyExtras()
	{
		// for efficiently, in the entity data only set things which are active, not being present assumes inactive
		List<int> lstExtrasJSON = new List<int>();

		foreach (var kvPair in m_VehicleExtras)
		{
			m_Vehicle.SetExtra(kvPair.Key, kvPair.Value);

			if (kvPair.Value)
			{
				lstExtrasJSON.Add(kvPair.Key);
			}
		}

		// TODO_RAGE: we have to set this as entity data and apply it clientside, serverside doesn't sync correctly
		// set entity data
		SetData(m_Vehicle, EDataNames.VEH_EXTRAS, Newtonsoft.Json.JsonConvert.SerializeObject(lstExtrasJSON), EDataType.Synced);
	}

	public async void ChangeVehicleModel(uint newHash)
	{
		m_VehicleHash = (VehicleHash)newHash;
		await Database.LegacyFunctions.SetVehicleModel(m_DatabaseID, newHash).ConfigureAwait(true);
	}

	public async void SetPlateStyle(EPlateType plateType, bool bSaveToDB)
	{
		m_Vehicle.NumberPlateStyle = (int)plateType;

		if (bSaveToDB)
		{
			await Database.LegacyFunctions.SetVehiclePlateStyle(m_DatabaseID, plateType).ConfigureAwait(true);
		}
	}

	public async void SetPlateText(string strPlateText, bool bSaveToDB)
	{
		m_Vehicle.NumberPlate = strPlateText;
		m_strPlateText = strPlateText;

		if (bSaveToDB)
		{
			await Database.LegacyFunctions.SetVehiclePlateText(m_DatabaseID, strPlateText).ConfigureAwait(true);
		}
	}

	public async void TogglePlate(bool bTogglePlate, string strPlateText)
	{
		m_Vehicle.NumberPlate = bTogglePlate ? strPlateText : "_";
		m_bShowPlate = bTogglePlate;
		await Database.LegacyFunctions.ToggleVehiclePlate(m_DatabaseID, bTogglePlate).ConfigureAwait(true);
	}

	public async void SetVehColor(int r1, int g1, int b1, int r2, int g2, int b2)
	{
		m_colPrimary = new Color(r1, g1, b1);
		m_colSecondary = new Color(r1, g1, b1);
		m_Vehicle.CustomPrimaryColor = m_colPrimary;
		m_Vehicle.CustomSecondaryColor = m_colSecondary;
		await Database.LegacyFunctions.SetVehicleColor(m_DatabaseID, r1, g1, b1, r2, g2, b2).ConfigureAwait(true);

	}

	public void SetVehPearlColor(int color)
	{
		m_PearlescentColor = color;
		m_Vehicle.PearlescentColor = m_PearlescentColor;
		Database.Functions.Vehicles.SetSpecialColor(m_DatabaseID, color);
	}

	public async void SetVehTransmission(EVehicleTransmissionType transmission)
	{
		m_transmissionType = transmission;
		await Database.LegacyFunctions.SetVehicleTransmissionType(m_DatabaseID, transmission).ConfigureAwait(true);

	}


	public async void SetNeonsState(bool bEnabled, int r, int g, int b, bool bSaveToDB)
	{
		m_Vehicle.Neons = bEnabled;
		m_Vehicle.NeonColor = new Color(r, g, b);

		if (bSaveToDB)
		{
			await Database.LegacyFunctions.SetVehicleNeonsState(m_DatabaseID, bEnabled, r, g, b).ConfigureAwait(true);
		}
	}

	private int m_CurrentRadioID = -1;
	public int CurrentRadioID
	{
		get => m_CurrentRadioID;
		set
		{
			Internal_SetRadio(value);
		}
	}

	private async void Internal_SetRadio(int radio)
	{
		m_CurrentRadioID = radio;
		await Database.LegacyFunctions.SetVehicleRadio(m_DatabaseID, m_CurrentRadioID).ConfigureAwait(true);
		SetData(m_Vehicle, EDataNames.VEH_RADIO, m_CurrentRadioID, EDataType.Synced);
	}

	public EVehicleType VehicleType
	{
		get => m_VehicleType;
		private set
		{
			m_VehicleType = value;
			SetData(m_Vehicle, EDataNames.VEHICLE_TYPE, (int)VehicleType, EDataType.Synced);
		}
	}

	public long OwnerID
	{
		get => m_ownerID;
		private set => m_ownerID = value;
	}

	private Vector3 m_vecDefaultSpawnPos { get; set; }
	private Vector3 m_vecDefaultSpawnRot { get; set; }

	// NOTE: This does NOT decrease as they make payments, its the fixed amount when they bought the vehicle
	// NOTE: Does not include interest!
	public float CreditUsedWhenBought
	{
		get => m_fCreditAmount; // NOTE: Does not include interest!
	}

	public int GetPaymentPlanNumberOfMonths()
	{
		return m_iPaymentsRemaining + m_iPaymentsMade;
	}

	public bool IsLegacyCreditVehicle()
	{
		return (m_iPaymentsRemaining >= 0 && m_fCreditAmount == 0.0f);
	}

	public float GetMonthlyPaymentAmount()
	{
		if (m_iPaymentsRemaining == 0)
		{
			return 0.0f;
		}

		// support for legacy (pre-custom credit plan) vehicles
		if (IsLegacyCreditVehicle())
		{
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash((uint)m_VehicleHash);
			if (vehicleDef != null)
			{
				float fMonthlyPayment = vehicleDef.Price / 100;
				return fMonthlyPayment;
			}
		}

		float fInterest = (m_fCreditAmount * Taxation.GetPaymentPlanInterestPercent());
		float fMonthlyPaymentAmount = (m_fCreditAmount / GetPaymentPlanNumberOfMonths()) + (fInterest / GetPaymentPlanNumberOfMonths());
		return fMonthlyPaymentAmount;
	}

	public async void DecreaseCreditAmount(float fAmount)
	{
		if (!IsLegacyCreditVehicle())
		{
			if (GetRemainingCredit() > 0.0f && fAmount <= GetRemainingCredit())
			{
				// If they are paying off the whole debt, wipe the interest & payments remaining.
				if (GetRemainingCredit(true) - fAmount < 0.01f)
				{
					m_fCreditAmount = 0.0f;
					PaymentsRemaining = 0;
				}
				else
				{
					m_fCreditAmount -= fAmount;
				}

				await Database.LegacyFunctions.SetVehicleCredit(m_DatabaseID, m_fCreditAmount).ConfigureAwait(true);
			}
		}
	}

	public float GetRemainingCredit(bool bIgnoreInterest = false)
	{
		if (m_iPaymentsRemaining == 0)
		{
			return 0.0f;
		}

		// support for legacy (pre-custom credit plan) vehicles
		if (IsLegacyCreditVehicle())
		{
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash((uint)m_VehicleHash);
			if (vehicleDef != null)
			{
				float fMonthlyPayment = vehicleDef.Price / 100;
				float fAmountPaidLegacy = fMonthlyPayment * PaymentsMade;
				float fAmountRemaningLegacy = vehicleDef.Price - fAmountPaidLegacy;
				return fAmountRemaningLegacy;
			}
		}

		float fInterest = bIgnoreInterest ? 0.0f : (m_fCreditAmount * Taxation.GetPaymentPlanInterestPercent());
		float fAmountPaid = m_iPaymentsMade * GetMonthlyPaymentAmount();
		float fAmountRemaining = (m_fCreditAmount + fInterest) - fAmountPaid;

		return fAmountRemaining;
	}

	public float GetRemainingCreditInterest()
	{
		if (m_iPaymentsRemaining == 0 || IsLegacyCreditVehicle())
		{
			return 0.0f;
		}

		float fInterest = (m_fCreditAmount * Taxation.GetPaymentPlanInterestPercent());
		return fInterest;
	}

	public int PaymentsRemaining
	{
		get => m_iPaymentsRemaining;
		set
		{
			Database.LegacyFunctions.SetVehiclePaymentsRemaining(m_DatabaseID, value);
			m_iPaymentsRemaining = value;
		}
	}

	public int PaymentsMade
	{
		get => m_iPaymentsMade;
		set
		{
			Database.LegacyFunctions.SetVehiclePaymentsMade(m_DatabaseID, value);
			m_iPaymentsMade = value;
		}
	}

	public int PaymentsMissed
	{
		get => m_iPaymentsMissed;
		set
		{
			Database.LegacyFunctions.SetVehiclePaymentsMissed(m_DatabaseID, value);
			m_iPaymentsMissed = value;
		}
	}

	public Int64 ExpiryTime
	{
		get => m_ExpiryTime;
		set
		{
			Database.LegacyFunctions.SetVehicleExpiryTime(m_DatabaseID, value);
			m_ExpiryTime = value;
		}
	}

	public float DealershipPrice
	{
		get => VehicleDefinitions.GetVehicleDefinitionFromHash((uint)m_VehicleHash).Price;
	}

	private int m_iPaymentsRemaining;
	private int m_iPaymentsMade;
	private int m_iPaymentsMissed;
	private float m_fCreditAmount;
	private long m_ownerID;

	private float m_fFuel;
	private float m_fDirt;
	private float m_fOdometer;

	private Int64 m_ExpiryTime = 0;

	private bool m_bTowed = false;
	private Dimension m_DefaultSpawnDimension = 0;

	public CVehicleInventory Inventory => m_Inventory;
	private CVehicleInventory m_Inventory;

	public Vector3 CachedPositionBeforeModShop { get; set; } = new Vector3();
	public Vector3 CachedRotationBeforeModShop { get; set; } = new Vector3();

	public bool NeonsEnabled { get; set; } = false;
	public Color NeonsColor { get; set; } = new Color();

	private Dictionary<EModSlot, int> m_ActiveVehicleMods = new Dictionary<EModSlot, int>();
	private Dictionary<int, bool> m_VehicleExtras = new Dictionary<int, bool>();

}

