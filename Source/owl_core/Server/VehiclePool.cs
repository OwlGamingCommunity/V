using ExtensionMethods;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

public static class VehiclePool
{
	public static void Init()
	{
		m_LookupTableNetHandle = new Dictionary<NetHandle, CVehicle>();
		m_LookupTableID = new Dictionary<EntityDatabaseID, CVehicle>();
	}

	public static void InitTicks()
	{
		// EVENTS
		RageEvents.RAGE_OnUpdate += Tick;
	}

	public static void Tick()
	{
		foreach (var kvPair in m_LookupTableID)
		{
			CVehicle vehicle = kvPair.Value;
			vehicle.OnUpdate();
		}
	}

	// TODO: Dimension support
	public static async Task<CVehicle> CreateVehicle(EntityDatabaseID a_VehicleID, EVehicleType a_VehicleType, EntityDatabaseID a_OwnerID, VehicleHash a_Model, Vector3 a_vecDefaultSpawnPos, Vector3 a_vecDefaultSpawnRot, Vector3 a_Pos,
		Vector3 a_vecRot, EPlateType a_PlateType, string a_strPlateText, float a_fFuel, int a_colPrimaryR, int a_colPrimaryG, int a_colPrimaryB, int a_colSecondaryR, int a_colSecondaryG, int a_colSecondaryB, int a_ColWheels, int a_Livery,
		float a_fDirt, float a_fHealth, bool a_bLocked, bool a_bEngineOn, int a_NumPaymentsRemaining, int a_NumPaymentsMade, int a_NumPaymentsMissed, float a_fCreditAmount, bool a_bMakeDatabaseEntry, bool a_bGeneratePlate, Int64 a_ExpiryTime,
		float a_fOdometer, bool a_bTowed, Dimension a_Dimension, Dictionary<EModSlot, int> a_Mods, Dictionary<int, bool> a_Extras, bool bNeonsEnabled, int neon_r, int neon_g, int neon_b, Int64 last_used, int radio, bool show_plate, bool token_purchase, EVehicleTransmissionType transmission, int pearlescent_color, uint addonModel = 0, Action<CVehicle> CompletionCallback = null)
	{
		CVehicleDefinition vehDef = VehicleDefinitions.GetVehicleDefinitionFromHash(a_Model == 0 && addonModel != 0 ? addonModel : (uint)a_Model);
		if (vehDef != null)
		{
			// if extras are null or a given key is missing, use the defaults
			if (a_Extras.Count == 0)
			{
				a_Extras = vehDef.DefaultExtras;
			}
			else
			{
				foreach (var kvPair in vehDef.DefaultExtras)
				{
					// Copy over any we haven't got overridden in the in param
					if (!a_Extras.ContainsKey(kvPair.Key))
					{
						a_Extras.Add(kvPair.Key, kvPair.Value);
					}
				}
			}

			if (vehDef.Equals(EVehicleClass.VehicleClass_Boats) ||
			vehDef.Equals(EVehicleClass.VehicleClass_Cycles) ||
			vehDef.Equals(EVehicleClass.VehicleClass_Helicopters) ||
			vehDef.Equals(EVehicleClass.VehicleClass_Trains) ||
			vehDef.Equals(EVehicleClass.VehicleClass_Planes) ||
			vehDef.Equals(EVehicleClass.VehicleClass_Military))
			{
				transmission = EVehicleTransmissionType.Automatic;
			}

			if (a_bMakeDatabaseEntry)
			{
				// TODO: No support for creating a vehicle with mods + neons
				a_VehicleID = await Database.LegacyFunctions.CreateVehicle(a_VehicleType, a_OwnerID, a_Model == 0 && addonModel != 0 ? addonModel : (uint)a_Model, a_vecDefaultSpawnPos, a_vecDefaultSpawnRot, a_Pos, a_vecRot, a_PlateType, a_strPlateText, a_fFuel, a_colPrimaryR, a_colPrimaryG, a_colPrimaryB, a_colSecondaryR, a_colSecondaryG, a_colSecondaryB, a_ColWheels, a_Livery, a_fDirt, a_fHealth, a_bLocked, a_bEngineOn, a_NumPaymentsRemaining, a_NumPaymentsMade, a_fCreditAmount, a_ExpiryTime, a_fOdometer, a_Dimension, last_used, transmission, pearlescent_color, token_purchase).ConfigureAwait(true);
			}

			if (a_bGeneratePlate)
			{
				a_strPlateText = HelperFunctions.Vehicle.GenerateLicensePlate(a_VehicleID);
				await Database.LegacyFunctions.SetVehiclePlateText(a_VehicleID, a_strPlateText).ConfigureAwait(true);
			}

			CVehicle newVehicle = new CVehicle(a_VehicleID, a_VehicleType, a_OwnerID, a_Model == 0 && addonModel != 0 ? addonModel : (uint)a_Model, a_vecDefaultSpawnPos, a_vecDefaultSpawnRot, a_Pos, a_vecRot, a_PlateType, a_strPlateText, a_fFuel, a_colPrimaryR, a_colPrimaryG,
				a_colPrimaryB, a_colSecondaryR, a_colSecondaryG, a_colSecondaryB, a_ColWheels, a_Livery, a_fDirt, a_fHealth, a_bLocked, a_bEngineOn, a_NumPaymentsRemaining, a_NumPaymentsMade, a_NumPaymentsMissed, a_fCreditAmount,
				a_ExpiryTime, a_fOdometer, a_bTowed, a_Dimension, a_Mods, a_Extras, bNeonsEnabled, neon_r, neon_g, neon_b, last_used, radio, show_plate, transmission, token_purchase, pearlescent_color, (CVehicle vehicle, NetHandle handle) =>
				{
					m_LookupTableNetHandle[handle] = vehicle;
					
					CompletionCallback?.Invoke(vehicle);
				});

			m_LookupTableID[a_VehicleID] = newVehicle;
			return newVehicle;
		}

		return null;
	}

	public static void DestroyVehicle(CVehicle a_VehicleInst)
	{
		if (a_VehicleInst != null)
		{
			if (a_VehicleInst.GTAInstance != null)
			{
				m_LookupTableNetHandle.Remove(a_VehicleInst.GTAInstance.Handle);
			}

			NAPI.Task.Run(() =>
			{
				a_VehicleInst.Destroy(true);

				a_VehicleInst.Cleanup();
				a_VehicleInst.Dispose();
			});

			a_VehicleInst.Cleanup();

			m_LookupTableID.Remove(a_VehicleInst.m_DatabaseID);
		}
	}

	public static async Task<bool> SetVehicleModel(CVehicle a_VehicleInst, uint hash)
	{
		a_VehicleInst.ChangeVehicleModel(hash);

		return await ReloadVehicle(a_VehicleInst).ConfigureAwait(true);
	}

	public static async Task<bool> ReloadVehicle(CVehicle a_VehicleInst)
	{
		EntityDatabaseID vehID = a_VehicleInst.m_DatabaseID;

		if (vehID >= 0)
		{
			if (a_VehicleInst.GTAInstance != null)
			{
				m_LookupTableNetHandle.Remove(a_VehicleInst.GTAInstance.Handle);
			}

			m_LookupTableID.Remove(a_VehicleInst.m_DatabaseID);

			NAPI.Task.Run(() =>
			{
				a_VehicleInst.Destroy(false);
			});

			CDatabaseStructureVehicle vehicle = await Database.LegacyFunctions.LoadVehicle(vehID).ConfigureAwait(true);

			CVehicleDefinition vehDef = VehicleDefinitions.GetVehicleDefinitionFromAddon(vehicle.model);

			CVehicle pVehicle = await CreateVehicle(vehicle.vehicleID, vehicle.vehicleType, vehicle.ownerID, vehDef == null ? (VehicleHash)vehicle.model : 0, vehicle.vecDefaultSpawnPos, vehicle.vecDefaultSpawnRot, vehicle.vecPos, vehicle.vecRot, vehicle.plateType,
						vehicle.strPlateText, vehicle.fFuel, vehicle.colPrimaryR, vehicle.colPrimaryG, vehicle.colPrimaryB, vehicle.colSecondaryR, vehicle.colSecondaryG, vehicle.colSecondaryB, vehicle.colWheel, vehicle.livery, vehicle.fDirt, vehicle.health,
						vehicle.bLocked, vehicle.bEngineOn, vehicle.iPaymentsRemaining, vehicle.iPaymentsMade, vehicle.iPaymentsMissed, vehicle.fCreditAmount, false, false, vehicle.expiryTime, vehicle.fOdometer, vehicle.bTowed, vehicle.dimension, vehicle.VehicleMods, vehicle.VehicleExtras,
						vehicle.bNeons, vehicle.neonR, vehicle.neonG, vehicle.neonB, vehicle.lastUsed, vehicle.radio, vehicle.show_plate, vehicle.token_purchase, vehicle.transmissionType, vehicle.PearlescentColor, vehDef == null ? 0 : vehicle.model).ConfigureAwait(true);

			pVehicle.Inventory.CopyInventory(vehicle.inventory);

			return true;
		}

		return false;
	}

	public static CVehicle GetVehicleFromID(Int64 a_VehicleID)
	{
		CVehicle retVal = null;
		m_LookupTableID.TryGetValue(a_VehicleID, out retVal);
		return retVal;
	}

	public static CVehicle GetVehicleFromGTAInstance(Vehicle GTAInstance)
	{
		if (GTAInstance == null)
		{
			return null;
		}

		CVehicle retVal = null;
		m_LookupTableNetHandle.TryGetValue(GTAInstance.Handle, out retVal);
		return retVal;
	}

	public static CVehicle GetVehicleFromGTAInstanceHandle(NetHandle GTAInstanceHandle)
	{
		if (GTAInstanceHandle == null)
		{
			return null;
		}

		CVehicle retVal = null;
		m_LookupTableNetHandle.TryGetValue(GTAInstanceHandle, out retVal);
		return retVal;
	}

	public static void SaveAll()
	{
		// init a one time worker
		MultiFrameWorkLoad workLoad = new MultiFrameWorkLoad(EWorkLoadProcessingType.FrameMillisecondsBudget, 10.0,
		(Queue<object> workQueue) => // init - this function is recalled when this is a looped multiframe queue, so you'll want to clear out any temp vars
		{
			// add everything
			workQueue.AddRange(m_LookupTableID.Values);
		}, (object objectToProcess) => // tick
		{
			// process object
			CVehicle vehicle = (CVehicle)objectToProcess;
			vehicle.Save();
		}, () => // completion
		{
			return false; // don't re-queue
		});

		MultiFrameWorkScheduler.QueueWork(workLoad);
	}

	public static List<CVehicle> GetVehiclesFromPlayerOwner(Int64 a_OwnerID)
	{
		List<CVehicle> lstVehicles = new List<CVehicle>();

		foreach (var kvPair in m_LookupTableID)
		{
			CVehicle vehicle = kvPair.Value;
			if (vehicle.OwnedOrRentedByCharacterID(a_OwnerID))
			{
				lstVehicles.Add(vehicle);
			}
		}

		return lstVehicles;
	}

	public static CVehicle GetVehicleFromPlate(string a_Plate)
	{
		foreach (var kvPair in m_LookupTableID)
		{
			CVehicle vehicle = kvPair.Value;
			if (vehicle.GTAInstance.NumberPlate.ToLower() == a_Plate.ToLower())
			{
				return vehicle;
			}
		}

		return null;
	}

	public static List<CVehicle> GetVehiclesFromPlayer(CPlayer a_Player)
	{
		return GetVehiclesFromPlayerOwner(a_Player.ActiveCharacterDatabaseID);
	}

	public static CVehicle GetPlayerOwnedVehicleByID(CPlayer a_Player, EntityDatabaseID a_ID)
	{
		return GetVehiclesFromPlayer(a_Player).FirstOrDefault(a_Vehicle => a_Vehicle.m_DatabaseID == a_ID);
	}

	public static List<CVehicle> GetVehiclesFromFactionOwner(Int64 a_OwnerID)
	{
		List<CVehicle> lstVehicles = new List<CVehicle>();

		foreach (var kvPair in m_LookupTableID)
		{
			CVehicle vehicle = kvPair.Value;
			if (vehicle.OwnedByFactionID(a_OwnerID))
			{
				lstVehicles.Add(vehicle);
			}
		}

		return lstVehicles;
	}

	public static List<CVehicle> GetVehiclesFromFaction(CFaction a_Faction)
	{
		return GetVehiclesFromFactionOwner(a_Faction.FactionID);
	}

	public static CVehicle GetFactionOwnedVehicleByID(CFaction a_Faction, EntityDatabaseID a_ID)
	{
		return GetVehiclesFromFaction(a_Faction).FirstOrDefault(a_Vehicle => a_Vehicle.m_DatabaseID == a_ID);
	}

	public static void FixAllVehicles()
	{
		foreach (var kvPair in m_LookupTableID)
		{
			CVehicle vehicle = kvPair.Value;
			vehicle.GTAInstance.Health = 1000.0f;
			vehicle.GTAInstance.Repair();
			vehicle.Save();
		}
	}

	public static List<CPlayer> GetVehicleOccupants(CVehicle vehicle)
	{
		List<CPlayer> lstPlayerOccupants = new List<CPlayer>();
		foreach (var player in PlayerPool.GetAllPlayers())
		{
			if (player.GetPlayerVehicleIsIn() == vehicle)
			{
				lstPlayerOccupants.Add(player);
			}
		}
		return lstPlayerOccupants;
	}

	public static List<CVehicle> GetCharacterVehicles(Int64 characterId)
	{
		return (from vehicle in m_LookupTableID where vehicle.Value.OwnedOrRentedByCharacterID(characterId) select vehicle.Value).ToList();
	}

	public static Dictionary<EntityDatabaseID, CVehicle> GetAllVehicles()
	{
		return m_LookupTableID;
	}

	private static Dictionary<NetHandle, CVehicle> m_LookupTableNetHandle = null;
	private static Dictionary<EntityDatabaseID, CVehicle> m_LookupTableID = null;
}