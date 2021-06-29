//#define CREATE_BLIP_FOR_GASSTATIONS
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EntityDatabaseID = System.Int64;

public class CFuelPoint
{
	public CFuelPoint(EntityDatabaseID a_DatabaseID, Vector3 a_vecPos, uint a_Dimension)
	{
		DatabaseID = a_DatabaseID;
		Position = a_vecPos;
		Dimension = a_Dimension;

		Create();
	}

	public void Destroy()
	{
		if (m_Marker != null)
		{
			NAPI.Entity.DeleteEntity(m_Marker.Handle);
		}

#if CREATE_BLIP_FOR_GASSTATIONS
		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
		}
#endif

		Database.LegacyFunctions.DestroyFuelPoint(DatabaseID);
	}

	private void Create()
	{
		if (m_Marker != null)
		{
			m_Marker.Delete();
			m_Marker = null;
		}

#if CREATE_BLIP_FOR_GASSTATIONS
		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
			m_Blip = null;
		}
#endif

		m_Marker = NAPI.Marker.CreateMarker(36, Position, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f), 1.0f, new Color(255, 255, 255, 100), true, Dimension);
		EntityDataManager.SetData(m_Marker, EDataNames.FUEL_POINT, true, EDataType.Synced);
		EntityDataManager.SetData(m_Marker, EDataNames.FUEL_ID, DatabaseID, EDataType.Synced);

#if CREATE_BLIP_FOR_GASSTATIONS
		m_Blip = HelperFunctions.Blip.Create(Position, true, 50.0f, Dimension, "Gas Station", 361);
#endif
	}

	public EntityDatabaseID DatabaseID { get; set; }
	public Vector3 Position { get; set; }

	private Marker m_Marker = null;
	public uint Dimension { get; set; }

#if CREATE_BLIP_FOR_GASSTATIONS
	private Blip m_Blip = null;
#endif
}

public class FuelStations
{
	public FuelStations()
	{
		NetworkEvents.RequestFueling += OnRequestFueling;
		NetworkEvents.JerryCanRefuelVehicle += OnJerryCanRefuel;

		LoadAllFuelPoints();
	}

	public void OnRequestFueling(CPlayer player, Int64 fuelPointID)
	{
		// TODO_CSHARP: Fix casting here. Entity data doesn't support EntityDatabaseID?
		CFuelPoint fuelPoint = m_dictInstances[fuelPointID];

		// TODO: Check distance, in vehicle, etc, all same checks that client has
		if (fuelPoint != null)
		{
			bool bSuccess = true;
			bool bIsPetrolCan = false;

			// Are we already fueling?
			if (m_dictFuelingActions.ContainsKey(player))
			{
				bSuccess = false;
			}

			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
			if (pVehicle == null)
			{
				if (player.GetWeaponDataClientside().Contains(WeaponHash.Petrolcan))
				{
					bSuccess = true;
					bIsPetrolCan = true;
				}
				else
				{
					bSuccess = false;
				}
			}

			if (pVehicle != null && !pVehicle.DoesVehicleConsumeFuel())
			{
				player.SendNotification("Fueling", ENotificationIcon.ExclamationSign, "This type of vehicle does not consume gas.", true);
				bSuccess = false;
			}

			NetworkEventSender.SendNetworkEvent_FuelingRequestResponse(player, bSuccess);

			if (bSuccess)
			{
				CFuelingAction fuelingAction = new CFuelingAction(player, pVehicle, fuelPoint, bIsPetrolCan);
				m_dictFuelingActions.Add(player, fuelingAction);
			}
		}
	}

	public async void LoadAllFuelPoints()
	{
		List<CDatabaseStructureFuelPoint> lstPoints = await Database.LegacyFunctions.LoadAllFuelPoints().ConfigureAwait(true);

		NAPI.Task.Run(async () =>
		{
			foreach (var point in lstPoints)
			{
				await CreateFuelPoint(point.dbID, point.vecPos, point.dimension, false).ConfigureAwait(true);
			}
		});

		NAPI.Util.ConsoleOutput("[FUEL POINTS] Loaded {0} Fuel Points!", lstPoints.Count);
	}

	public void OnJerryCanRefuel(CPlayer player, Int64 inventoryDBID, Int64 vehicleID)
	{
		CItemInstanceDef item = player.Inventory.GetItemFromDBID(inventoryDBID);
		if (item == null)
		{
			player.SendNotification("Jerry Can", ENotificationIcon.ExclamationSign, "You do not have a jerry can.");
			return;
		}

		CVehicle vehicle = VehiclePool.GetVehicleFromID(vehicleID);
		if (vehicle == null)
		{
			player.SendNotification("Jerry Can", ENotificationIcon.ExclamationSign, "You are not near a vehicle.");
			return;
		}

		float fGallonsInCan = ((CItemValueBasic)item.Value).value;
		float fGallonsUsed = fGallonsInCan;
		float fFuelTankSize = FuelSystem.g_fFuelTankSizes[vehicle.GTAInstance.Class];
		float fCurrentFuelGallons = (vehicle.Fuel / 100.0f) * fFuelTankSize;
		float fFuelNeededForFull = fFuelTankSize - fCurrentFuelGallons;

		if (fFuelNeededForFull < fGallonsUsed)
		{
			fGallonsUsed = fFuelNeededForFull;
		}

		vehicle.Fuel += (fGallonsUsed / fFuelTankSize) * 100.0f; // as a percentage
		vehicle.Save();

		((CItemValueBasic)item.Value).value -= fGallonsUsed;
		player.Inventory.RemoveItem(item);
		player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);

		player.SendNotification("Jerry Can", ENotificationIcon.Star, "You have added {0} gallons of fuel to the {1}.", fGallonsUsed, vehicle.GetFullDisplayName());
	}

	public static async Task<CFuelPoint> CreateFuelPoint(EntityDatabaseID a_DatabaseID, Vector3 a_vecPos, uint a_Dimension, bool bInsertIntoDB)
	{
		if (bInsertIntoDB)
		{
			a_DatabaseID = await Database.LegacyFunctions.CreateFuelPoint(a_vecPos, a_Dimension).ConfigureAwait(true);
		}

		CFuelPoint newInst = new CFuelPoint(a_DatabaseID, a_vecPos, a_Dimension);
		m_dictInstances.Add(a_DatabaseID, newInst);
		return newInst;
	}

	public static async void DestroyFuelPoint(CFuelPoint a_Inst, bool bDeleteFromDB)
	{
		if (bDeleteFromDB)
		{
			await Database.LegacyFunctions.DestroyFuelPoint(a_Inst.DatabaseID).ConfigureAwait(true);
		}

		a_Inst.Destroy();
		m_dictInstances.Remove(a_Inst.DatabaseID);
	}

	public static void OnFuelingActionComplete(CPlayer a_RequestingPlayer)
	{
		NetworkEventSender.SendNetworkEvent_FuelingComplete(a_RequestingPlayer);
		m_dictFuelingActions.Remove(a_RequestingPlayer);
	}

	public static Dictionary<EntityDatabaseID, CFuelPoint> GetFuelPoints()
	{
		return m_dictInstances;
	}

	public static CFuelPoint GetFuelPointByID(EntityDatabaseID ID)
	{
		return m_dictInstances.ContainsKey(ID) ? m_dictInstances[ID] : null;
	}

	private static Dictionary<EntityDatabaseID, CFuelPoint> m_dictInstances = new Dictionary<EntityDatabaseID, CFuelPoint>();
	private static Dictionary<CPlayer, CFuelingAction> m_dictFuelingActions = new Dictionary<CPlayer, CFuelingAction>();

	public static float gs_fDistThreshold = 5.0f; // NOTE: If you change this, change fuel_stations.ts also
	public static float gs_fFuelCostPerGallon = 3.0f; // TODO: Allow player to specify amount to fill up, make like real life (e.g. hold button)
}

internal class CFuelingAction : IDisposable
{
	private const float MAX_FUEL_CAN_SIZE = 5.0f;

	public CFuelingAction(CPlayer a_RequestingPlayer, CVehicle a_Vehicle, CFuelPoint a_FuelPoint, bool bIsPetrolCan = false)
	{
		m_Timer = MainThreadTimerPool.CreateEntityTimer(OnFuelingActionComplete, 15000, a_RequestingPlayer, 1);

		m_RequestingPlayer.SetTarget(a_RequestingPlayer);
		m_RequestingPlayerVehicle = a_Vehicle;
		m_FuelPoint = a_FuelPoint;
		m_bIsPetrolCan = bIsPetrolCan;
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		if (a_CleanupNativeAndManaged)
		{

		}
	}

	// TODO: Sales tax on gas?
	private void OnFuelingActionComplete(object[] a_Parameters = null)
	{
		CPlayer pPlayer = m_RequestingPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		if (m_FuelPoint != null && m_bIsPetrolCan)
		{
			CItemInstanceDef item = pPlayer.Inventory.GetFirstItemOfID(EItemID.WEAPON_PETROLCAN);

			float currentFuelLevel = ((CItemValueBasic)item.Value).value;
			if (currentFuelLevel >= MAX_FUEL_CAN_SIZE)
			{
				pPlayer.SendNotification("Refuel Failed", ENotificationIcon.ExclamationSign, "This fuel can is already full.");
				FuelStations.OnFuelingActionComplete(pPlayer);
				return;
			}

			if (currentFuelLevel < 0.0f)
			{
				currentFuelLevel = 0.0f;
			}

			float fAmountToAdd = MAX_FUEL_CAN_SIZE - currentFuelLevel;
			float fCostForFull = fAmountToAdd * FuelStations.gs_fFuelCostPerGallon;

			if (!pPlayer.SubtractMoney(fCostForFull, PlayerMoneyModificationReason.FuelStation))
			{
				pPlayer.SendNotification("Refuel Failed", ENotificationIcon.ExclamationSign, "You cannot afford the ${0:0.00} to fill this jerry can.", fCostForFull);
				FuelStations.OnFuelingActionComplete(pPlayer);
				return;
			}

			((CItemValueBasic)item.Value).value = MAX_FUEL_CAN_SIZE;
			pPlayer.Inventory.RemoveItem(item);
			pPlayer.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);
			pPlayer.SendNotification("Jerry Can Refuel", ENotificationIcon.Star, "You paid ${0:0.00} to add {1:0.0} gallons of fuel to your jerry can.", fCostForFull, fAmountToAdd);
		}

		if (m_RequestingPlayerVehicle != null && m_RequestingPlayerVehicle != null && m_FuelPoint != null)
		{
			// Is the player still in the same vehicle?
			CVehicle pVehicle = pPlayer.Client.Vehicle != null ? VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle) : null;

			if (pVehicle != m_RequestingPlayerVehicle)
			{
				pPlayer.SendNotification("Fueling", ENotificationIcon.ExclamationSign, "Fueling: You are no longer in the same vehicle.", null);
			}
			else
			{
				if (pPlayer.IsWithinDistanceOf(m_FuelPoint.Position, FuelStations.gs_fDistThreshold, m_FuelPoint.Dimension))
				{
					// How much can the player afford?
					float fFuelTankSize = FuelSystem.g_fFuelTankSizes[m_RequestingPlayerVehicle.GTAInstance.Class];
					float fCurrentFuelGallons = (pVehicle.Fuel / 100.0f) * fFuelTankSize;
					float fFuelNeededForFull = fFuelTankSize - fCurrentFuelGallons;
					float fCostForFull = fFuelNeededForFull * FuelStations.gs_fFuelCostPerGallon;

					float fGallonsBought = 0.0f;
					float fMoneySpent = 0.0f;

					// BEGIN DONATION PERK
					if (pPlayer.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.FreeGas))
					{
						fCostForFull = 0.0f;
					}
					// END DONATION PERK

					if (pVehicle.IsJobVehicle())
					{
						fCostForFull = 0.0f;
					}

					bool bCanSubtractMoneyFull = false;
					float fGallonsAffordable = 0.0f;
					float fCost = 0.0f;
					bool bBilledToFaction = false;

					// Is it a faction vehicle for the requesting player
					if (pVehicle.IsVehicleForAnyPlayerFaction(pPlayer))
					{
						CFaction pOwningFaction = pVehicle.GetFactionOwner();

						if (pOwningFaction != null)
						{
							bBilledToFaction = true;

							if (pOwningFaction.SubtractMoney(fCostForFull))
							{
								bCanSubtractMoneyFull = true;
							}
							else
							{
								// How much can we afford?
								fGallonsAffordable = pOwningFaction.Money / FuelStations.gs_fFuelCostPerGallon;
								fCost = fGallonsAffordable * FuelStations.gs_fFuelCostPerGallon;

								if (!pOwningFaction.SubtractMoney(fCost))
								{
									// Couldn't afford the minimum? then we bought nothing!
									fGallonsAffordable = 0.0f;
									fCost = 0.0f;
								}
							}
						}
					}
					else
					{
						if (pPlayer.SubtractMoney(fCostForFull, PlayerMoneyModificationReason.FuelStation))
						{
							bCanSubtractMoneyFull = true;
						}
						else
						{
							// How much can we afford?
							fGallonsAffordable = pPlayer.Money / FuelStations.gs_fFuelCostPerGallon;
							fCost = fGallonsAffordable * FuelStations.gs_fFuelCostPerGallon;

							if (!pPlayer.SubtractMoney(fCost, PlayerMoneyModificationReason.FuelStation))
							{
								// Couldn't afford the minimum? then we bought nothing!
								fGallonsAffordable = 0.0f;
								fCost = 0.0f;
							}
						}
					}

					if (bCanSubtractMoneyFull)
					{
						fGallonsBought = fFuelNeededForFull;
						fMoneySpent = fCostForFull;
					}
					else
					{
						fGallonsBought = fGallonsAffordable;
						fMoneySpent = fCost;
					}

					if (fGallonsBought > 0.0f)
					{
						// refuel vehicle
						m_RequestingPlayerVehicle.Fuel += (fGallonsBought / fFuelTankSize) * 100.0f; // as a percentage
						m_RequestingPlayerVehicle.Save();

						// BEGIN DONATION PERK
						if (pPlayer.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.FreeGas))
						{
							pPlayer.SendNotification("Fueling", ENotificationIcon.InfoSign, "You got {0:0.00} gallons of fuel for free (Donator Perk)", fGallonsBought);
						}
						// END DONATION PERK
						else if (pVehicle.IsJobVehicle())
						{
							pPlayer.SendNotification("Fueling", ENotificationIcon.InfoSign, "You got {0:0.00} gallons of fuel (Paid by Employer)", fGallonsBought);
						}
						else
						{
							pPlayer.SendNotification("Fueling", ENotificationIcon.InfoSign, "You bought {0:0.00} gallons of fuel for ${1:0.00}{2}", fGallonsBought, fMoneySpent, bBilledToFaction ? " (Paid by Faction)" : "");
						}
					}
					else
					{
						// TODO: Check money before starting timer...
						if (bBilledToFaction)
						{
							pPlayer.SendNotification("Fueling", ENotificationIcon.ExclamationSign, "Your faction cannot afford any fuel.", null);
						}
						else
						{
							pPlayer.SendNotification("Fueling", ENotificationIcon.ExclamationSign, "You cannot afford any fuel.", null);
						}
					}
				}
				else
				{
					pPlayer.SendNotification("Fueling", ENotificationIcon.ExclamationSign, "You are no longer in the fueling circle.", null);
				}
			}
		}

		FuelStations.OnFuelingActionComplete(pPlayer);
	}

	private WeakReference<CPlayer> m_RequestingPlayer = new WeakReference<CPlayer>(null);
	private CVehicle m_RequestingPlayerVehicle = null;
	private CFuelPoint m_FuelPoint = null;
	private bool m_bIsPetrolCan = false;
	private WeakReference<MainThreadTimer> m_Timer = new WeakReference<MainThreadTimer>(null);
};