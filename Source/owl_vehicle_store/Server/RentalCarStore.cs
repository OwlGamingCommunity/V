using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

public class CScooterRentalShop : CBaseEntity
{
	public CScooterRentalShop(EntityDatabaseID dbid, Vector3 vecPedPos, float pedHeading, Dimension pedDimension, Vector3 vecSpawnPos, float spawnHeading, Dimension spawnDimension)
	{
		m_DatabaseID = dbid;

		m_vecPedPos = vecPedPos;
		m_fPedRot = pedHeading;
		m_pedDimension = pedDimension;

		m_vecSpawnPos = vecSpawnPos;
		m_fSpawnRot = spawnHeading;
		m_spawnDimension = spawnDimension;

		SendToAllPlayers();
	}

	public void SendToAllPlayers()
	{
		NetworkEventSender.SendNetworkEvent_RentalShop_CreatePed_ForAll_IncludeEveryone(m_DatabaseID, m_vecPedPos, m_fPedRot, m_pedDimension);
	}

	public void SendToPlayer(CPlayer a_Player)
	{
		NetworkEventSender.SendNetworkEvent_RentalShop_CreatePed(a_Player, m_DatabaseID, m_vecPedPos, m_fPedRot, m_pedDimension);
	}

	~CScooterRentalShop()
	{
		Destroy();
	}

	public void Destroy()
	{
		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			NetworkEventSender.SendNetworkEvent_RentalShop_DestroyPed(player, m_vecPedPos, m_fPedRot, m_pedDimension);
		}
	}

	public float SpawnRot()
	{
		return m_fSpawnRot;
	}

	public Vector3 m_vecPedPos { get; set; }
	public Vector3 m_vecSpawnPos { get; set; }
	private readonly float m_fPedRot;
	private readonly float m_fSpawnRot;
	public uint m_pedDimension { get; set; }
	public uint m_spawnDimension { get; set; }
}

public class RentalCarStore
{
	public RentalCarStore()
	{
		NetworkEvents.RentVehicle_OnCheckout += OnCheckout;
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
		NetworkEvents.RentalShop_RentScooter += OnRentScooter;

		LoadAllScooterRentalShops();
	}

	public static void OnPlayerConnected(CPlayer a_pPlayer)
	{
		// Send all instances
		foreach (var storeInst in m_dictStoreInstances)
		{
			storeInst.Value.SendToPlayer(a_pPlayer);
		}
	}

	public async void LoadAllScooterRentalShops()
	{
		List<CDatabaseStructureScooterRentalShop> lstPoints = await Database.LegacyFunctions.LoadAllScooterRentalShops().ConfigureAwait(true);

		NAPI.Task.Run(() =>
		{
			foreach (var point in lstPoints)
			{
				CreateScooterRentalShop(point.dbID, point.vecPedPos, point.pedHeading, point.pedDimension, point.vecSpawnPos, point.spawnHeading, point.spawnDimension);
			}
		});

		NAPI.Util.ConsoleOutput("[SCOOTER RENTAL SHOPS] Loaded {0} rental shops!", lstPoints.Count);
	}

	public void CreateScooterRentalShop(EntityDatabaseID dbid, Vector3 vecPedPos, float pedHeading, Dimension pedDimension, Vector3 vecSpawnPos, float spawnHeading, Dimension spawnDimension)
	{
		CScooterRentalShop newInst = new CScooterRentalShop(dbid, vecPedPos, pedHeading, pedDimension, vecSpawnPos, spawnHeading, spawnDimension);
		m_dictStoreInstances.Add(dbid, newInst);
	}

	public void OnRentScooter(CPlayer player, Int64 storeID)
	{
		CScooterRentalShop shop = m_dictStoreInstances.Values.FirstOrDefault(store => store.m_DatabaseID == storeID);
		if (shop == null)
		{
			return;
		}

		Vector3 vecSpawnPos = shop.m_vecSpawnPos;
		Vector3 vecSpawnRot = new Vector3(0, 0, shop.SpawnRot());
		RentVehicle(player, 150, 255, 255, 255, 255, 255, 255, EPurchaserType.Character,
			player.ActiveCharacterDatabaseID, 1, vecSpawnPos, vecSpawnRot);
	}

	public void OnCheckout(CPlayer player, int vehicleIndex, uint primary_r, uint primary_g, uint primary_b, uint secondary_r, uint secondary_g, uint secondary_b, EPurchaserType PurchaserType,
		long PurchaserID, uint rentalLengthDays, EScriptLocation location, EVehicleStoreType storeType)
	{
		Vector3 vecSpawnPos = storeType == EVehicleStoreType.Boats
			? g_vecDefaultSpawnPos_Boats
			: (location == EScriptLocation.Paleto ? g_vecDefaultSpawnPos_Paleto : g_vecDefaultSpawnPos_LS);
		Vector3 vecSpawnRot = storeType == EVehicleStoreType.Boats
			? g_vecDefaultSpawnRot_Boats
			: (location == EScriptLocation.Paleto ? g_vecDefaultSpawnRot_Paleto : g_vecDefaultSpawnRot_LS);

		RentVehicle(player, vehicleIndex, primary_r, primary_g, primary_b, secondary_r, secondary_g, secondary_b,
			PurchaserType, PurchaserID, rentalLengthDays, vecSpawnPos, vecSpawnRot);
	}

	private async void RentVehicle(CPlayer player, int vehicleIndex, uint primary_r, uint primary_g, uint primary_b,
		uint secondary_r, uint secondary_g, uint secondary_b, EPurchaserType PurchaserType, long PurchaserID,
		uint rentalLengthDays, Vector3 vecSpawnPos, Vector3 vecSpawnRot)
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(vehicleIndex);
		if (!vehicleDef.IsRentable)
		{
			player.SendNotification("Vehicle Rentals", ENotificationIcon.ExclamationSign, "That vehicle is not rentable.");
			NetworkEventSender.SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(player,
				ERentVehicleResult.GenericFailureCheckNotification);
			return;
		}

		Color primaryCol = new Color(Convert.ToInt32(primary_r), Convert.ToInt32(primary_g), Convert.ToInt32(primary_b));
		Color secondaryCol = new Color(Convert.ToInt32(secondary_r), Convert.ToInt32(secondary_g),
			Convert.ToInt32(secondary_b));

		float fTotalPrice = vehicleDef.RentalPricePerDay * rentalLengthDays;

		EPlateType plateType = EPlateType.Blue_White;
		const float fFuel = 100.0f;
		const string strPlateText = "";
		int colWheel = 0;
		int liveryID = 0;
		const float fDirt = 0.0f;
		const float fHealth = 1000.0f;
		const bool bLocked = true;
		const bool bEngineOn = false;
		const int iPaymentsRemaining = 0;
		int iPaymentsMade = 0;
		int iPaymentsMissed = 0;
		float fCreditAmount = 0.0f;
		float fOdometer = 0.0f;

		const EntityDatabaseID EmptyDBID = 0;

		Int64 expiryTime = Helpers.GetUnixTimestamp() + (86400 * rentalLengthDays);

		// Character purchaser
		if (PurchaserType == EPurchaserType.Character)
		{
			if (player.CanPlayerAffordBankCost(fTotalPrice))
			{
				// Does the player have space for key?
				if (player.Inventory.CanGiveItem(CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, 0),
					out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
				{
					Int64 unixTimestamp = Helpers.GetUnixTimestamp();
					Task<CVehicle> CreateVehicleTask = VehiclePool.CreateVehicle(EmptyDBID, EVehicleType.RentalCar,
						player.ActiveCharacterDatabaseID, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? 0 : (VehicleHash)vehicleDef.Hash, vecSpawnPos, vecSpawnRot,
						vecSpawnPos, vecSpawnRot, plateType,
						strPlateText, fFuel, primaryCol.Red, primaryCol.Green, primaryCol.Blue, secondaryCol.Red,
						secondaryCol.Green, secondaryCol.Blue, colWheel, liveryID, fDirt, fHealth, bLocked, bEngineOn,
						iPaymentsRemaining, iPaymentsMade,
						iPaymentsMissed, fCreditAmount, true, true, expiryTime, fOdometer, false, 0,
						new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, EVehicleTransmissionType.Automatic, 0,
						vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? NAPI.Util.GetHashKey(vehicleDef.AddOnName) : 0);
					CVehicle pVehicle = await CreateVehicleTask.ConfigureAwait(true);

					// Take money from bank, otherwise take from player and allow negative (this fixes race conditions)
					if (!player.SubtractBankBalanceIfCanAfford(fTotalPrice, PlayerMoneyModificationReason.RentalCarCheckout))
					{
						player.SubtractMoneyAllowNegative(fTotalPrice, PlayerMoneyModificationReason.RentalCarCheckout);
					}

					CItemInstanceDef itemInstDef =
						CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, pVehicle.m_DatabaseID);
					player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							player.SendNotification("Vehicle Rental", ENotificationIcon.ThumbsUp,
								"Congratulations on renting a {0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
							NetworkEventSender.SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(player,
								ERentVehicleResult.Success);
						}
					});

				}
				else
				{
					player.SendNotification("Vehicle Rental", ENotificationIcon.ExclamationSign,
						"You cannot receive the keys to this vehicle:<br>{0}", strUserFriendlyMessage);
					NetworkEventSender.SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(player,
						ERentVehicleResult.GenericFailureCheckNotification);
				}
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(player,
					ERentVehicleResult.CannotAffordVehicle);
			}
		}
		else if (PurchaserType == EPurchaserType.Faction)
		{
			// TODO_POST_LAUNCH: Should factions pay tax?

			CFaction factionInst = FactionPool.GetFactionFromID(PurchaserID);

			if (factionInst != null)
			{
				CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);

				if (factionMembership != null)
				{
					if (factionMembership.Manager)
					{
						if (factionInst.Money >= fTotalPrice)
						{
							Int64 unixTimestamp = Helpers.GetUnixTimestamp();
							Task<CVehicle> CreateVehicleTask = VehiclePool.CreateVehicle(EmptyDBID,
								EVehicleType.FactionOwnedRental, factionInst.FactionID, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? 0 : (VehicleHash)vehicleDef.Hash,
								vecSpawnPos, vecSpawnRot, vecSpawnPos, vecSpawnRot, plateType,
								strPlateText, fFuel, primaryCol.Red, primaryCol.Green, primaryCol.Blue, secondaryCol.Red,
								secondaryCol.Green, secondaryCol.Blue, colWheel, liveryID, fDirt, fHealth, bLocked,
								bEngineOn, iPaymentsRemaining, iPaymentsMade,
								iPaymentsMissed, fCreditAmount, true, true, expiryTime, fOdometer, false, 0,
								new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, EVehicleTransmissionType.Automatic, 0,
								vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? NAPI.Util.GetHashKey(vehicleDef.AddOnName) : 0);
							CVehicle pVehicle = await CreateVehicleTask.ConfigureAwait(true);

							// Take money from bank, allow negative, since we already did the sql op above
							factionInst.SubtractMoneyAllowNegatve(fTotalPrice);

							string strMessage = Helpers.FormatString(
								"{0} rented a {1} {2} for this faction for ${3:0.00} ({4} days).",
								player.GetCharacterName(ENameType.StaticCharacterName), vehicleDef.Manufacturer,
								vehicleDef.Name, fTotalPrice, rentalLengthDays);
							factionInst.SendNotificationToAllManagers(strMessage);

							NetworkEventSender.SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(player,
								ERentVehicleResult.Success);
						}
						else
						{
							NetworkEventSender.SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(player,
								ERentVehicleResult.CannotAffordVehicle);
						}
					}
				}
			}
		}
	}

	// TODO: Rental cars don't respect the new spawning model that checks for a suitable slot
	private readonly Vector3 g_vecDefaultSpawnPos_Paleto = new Vector3(-208.1837f, 6220.204f, 31.49133f);
	private readonly Vector3 g_vecDefaultSpawnRot_Paleto = new Vector3(0.0f, 0.0f, -134.6f);
	private readonly Vector3 g_vecDefaultSpawnPos_LS = new Vector3(-61.87835f, -1117.447f, 26.43296f);
	private readonly Vector3 g_vecDefaultSpawnRot_LS = new Vector3(0.0f, 0.0f, 2.437349);
	private readonly Vector3 g_vecDefaultSpawnPos_Boats = new Vector3(-820.6497, -1417.855, 0.0);
	private readonly Vector3 g_vecDefaultSpawnRot_Boats = new Vector3(0.0f, 0.0f, 110.0f);
	private static Dictionary<EntityDatabaseID, CScooterRentalShop> m_dictStoreInstances = new Dictionary<EntityDatabaseID, CScooterRentalShop>();

}