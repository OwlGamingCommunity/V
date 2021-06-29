using GTANetworkAPI;
using System;
using System.Collections.Generic;

using EntityDatabaseID = System.Int64;

public class CVehicleRepairPoint
{
	public CVehicleRepairPoint(EntityDatabaseID a_DatabaseID, Vector3 a_vecPos, uint a_Dimension)
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

		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
		}

		Database.LegacyFunctions.DestroyVehicleRepairPoint(DatabaseID);
	}

	private void Create()
	{
		if (m_Marker != null)
		{
			m_Marker.Delete();
			m_Marker = null;
		}

		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
			m_Blip = null;
		}

		m_Marker = API.Shared.CreateMarker(27, Position, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 0.0f, 0.0f), 5.0f, new Color(15, 164, 101, 200), true, Dimension);
		EntityDataManager.SetData(m_Marker, EDataNames.VEHREP_POINT, true, EDataType.Synced);
		EntityDataManager.SetData(m_Marker, EDataNames.VEHREP_ID, DatabaseID, EDataType.Synced);

		m_Blip = HelperFunctions.Blip.Create(Position, true, 50.0f, Dimension, "Auto Repairs", 72);
	}

	public EntityDatabaseID DatabaseID { get; set; }
	public Vector3 Position { get; set; }

	private Marker m_Marker = null;
	public uint Dimension { get; set; }
	private Blip m_Blip = null;
}

public class CVehicleRepairPoints
{
	public CVehicleRepairPoints()
	{
		NetworkEvents.RequestVehicleRepair += OnRequestVehicleRepair;

		List<CDatabaseStructureVehicleRepairPoint> lstPoints = Database.LegacyFunctions.LoadAllVehicleRepairPoints().Result;

		NAPI.Task.Run(() =>
		{
			foreach (var point in lstPoints)
			{
				CreateVehicleRepairPoint(point.dbID, point.vecPos, point.dimension);
			}
		});

		NAPI.Util.ConsoleOutput("[VEH REPAIR POINTS] Loaded {0} Vehicle Repair Points!", lstPoints.Count);
	}

	public void OnRequestVehicleRepair(CPlayer player, Int64 repairPointID)
	{
		// TODO_CSHARP: Fix cast
		CVehicleRepairPoint repairPoint = m_dictInstances[repairPointID];

		if (repairPoint != null)
		{
			bool bSuccess = true;

			// Are we already using a point?
			if (m_dictRepairPointActions.ContainsKey(player))
			{
				bSuccess = false;
			}

			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
			if (pVehicle == null)
			{
				bSuccess = false;
			}

			// Can we afford it (estimated, still checked on completion) NOTE: Must change below also
			// NOTE: IF we hit this code, the damage might be visual only and health could be full, so we min the cost to 50 bucks
			float fCost = Math.Min(50.0f, (1000 - pVehicle.GTAInstance.Health) * 0.4f);
			if (!player.CanPlayerAffordCost(fCost))
			{
				player.SendNotification("Auto Repair", ENotificationIcon.ExclamationSign, "You cannot afford a repair (${0:0.00})", fCost);
				bSuccess = false;
			}

			NetworkEventSender.SendNetworkEvent_VehicleRepairRequestResponse(player, bSuccess);

			if (bSuccess)
			{
				CVehicleRepairAction repairAction = new CVehicleRepairAction(player, pVehicle, repairPoint);
				m_dictRepairPointActions.Add(player, repairAction);
			}
		}
	}

	public void CreateVehicleRepairPoint(EntityDatabaseID a_DatabaseID, Vector3 a_vecPos, uint a_Dimension)
	{
		CVehicleRepairPoint newInst = new CVehicleRepairPoint(a_DatabaseID, a_vecPos, a_Dimension);
		m_dictInstances.Add(a_DatabaseID, newInst);
	}

	public void DestroyVehicleRepairPoint(CVehicleRepairPoint a_Inst)
	{
		a_Inst.Destroy();
		m_dictInstances.Remove(a_Inst.DatabaseID);
	}

	public static void OnVehicleRepairActionComplete(CPlayer a_RequestingPlayer)
	{
		NetworkEventSender.SendNetworkEvent_VehicleRepairComplete(a_RequestingPlayer);
		m_dictRepairPointActions.Remove(a_RequestingPlayer);
	}

	private Dictionary<EntityDatabaseID, CVehicleRepairPoint> m_dictInstances = new Dictionary<EntityDatabaseID, CVehicleRepairPoint>();
	private static Dictionary<CPlayer, CVehicleRepairAction> m_dictRepairPointActions = new Dictionary<CPlayer, CVehicleRepairAction>();

	public static float gs_fDistThreshold = 5.0f; // NOTE: If you change this, change vehiclerepair_system.ts also
}

internal class CVehicleRepairAction : IDisposable
{
	public CVehicleRepairAction(CPlayer a_RequestingPlayer, CVehicle a_Vehicle, CVehicleRepairPoint a_repairPoint)
	{
		m_Timer = MainThreadTimerPool.CreateEntityTimer(OnRepairActionComplete, 12000, a_RequestingPlayer, 1);

		m_RequestingPlayer.SetTarget(a_RequestingPlayer);
		m_RequestingPlayerVehicle = a_Vehicle;
		m_VehicleRepairPoint = a_repairPoint;
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

	// TODO: Sales tax?
	private void OnRepairActionComplete(object[] a_Parameters = null)
	{
		CPlayer pPlayer = m_RequestingPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		bool bSucceeded = false;

		if (m_RequestingPlayerVehicle != null && m_RequestingPlayerVehicle != null && m_VehicleRepairPoint != null)
		{
			// Is the player still in the same vehicle?
			CVehicle pVehicle = pPlayer.Client.Vehicle != null ? VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle) : null;

			if (pVehicle != m_RequestingPlayerVehicle)
			{
				pPlayer.SendNotification("Auto Repair", ENotificationIcon.ExclamationSign, "You are no longer in the same vehicle.", null);
			}
			else
			{
				if (pPlayer.IsWithinDistanceOf(m_VehicleRepairPoint.Position, CVehicleRepairPoints.gs_fDistThreshold, m_VehicleRepairPoint.Dimension))
				{
					// Calculate cost NOTE: Must change above also
					// NOTE: IF we hit this code, the damage might be visual only and health could be full, so we min the cost to 50 bucks
					float fCost = Math.Min(50.0f, (1000 - pVehicle.GTAInstance.Health) * 0.4f); // 20 cents per unit of damage, meaning maximum cost is $400

					bool bBilledToFaction = false;
					if (pVehicle.IsVehicleForAnyPlayerFaction(pPlayer))
					{
						CFaction pOwningFaction = pVehicle.GetFactionOwner();

						if (pOwningFaction != null)
						{
							bBilledToFaction = true;

							if (pOwningFaction.SubtractMoney(fCost))
							{
								bSucceeded = true;
							}
						}
					}
					else if (pVehicle.IsJobVehicle())
					{
						bSucceeded = true;
					}
					else
					{
						if (pPlayer.SubtractMoney(fCost, PlayerMoneyModificationReason.VehicleRepair))
						{
							bSucceeded = true;
						}
					}

					if (bSucceeded)
					{
						if (pVehicle.IsJobVehicle())
						{
							pPlayer.SendNotification("Auto Repair", ENotificationIcon.InfoSign, "Your vehicle has been repaired. (Paid by Employer)");
						}
						else
						{
							pPlayer.SendNotification("Auto Repair", ENotificationIcon.InfoSign, "Your vehicle has been repaired for ${0:0.00}{1}", fCost, bBilledToFaction ? " (Paid by Faction)" : "");
						}
					}
					else
					{
						if (bBilledToFaction)
						{
							pPlayer.SendNotification("Auto Repair", ENotificationIcon.ExclamationSign, "Your faction cannot afford a repair (Cost: ${0:0.00}).", fCost);
						}
						else
						{
							pPlayer.SendNotification("Auto Repair", ENotificationIcon.ExclamationSign, "You cannot afford a repair (Cost: ${0:0.00}).", fCost);
						}
					}
				}
				else
				{
					pPlayer.SendNotification("Auto Repair", ENotificationIcon.ExclamationSign, "You are no longer in the repair circle.", null);
				}
			}
		}

		if (bSucceeded)
		{
			m_RequestingPlayerVehicle.GTAInstance.Health = 1000.0f;
			m_RequestingPlayerVehicle.GTAInstance.Repair();
			m_RequestingPlayerVehicle.Save();
		}

		CVehicleRepairPoints.OnVehicleRepairActionComplete(pPlayer);
	}

	private WeakReference<CPlayer> m_RequestingPlayer = new WeakReference<CPlayer>(null);
	private CVehicle m_RequestingPlayerVehicle = null;
	private CVehicleRepairPoint m_VehicleRepairPoint = null;
	private WeakReference<MainThreadTimer> m_Timer = new WeakReference<MainThreadTimer>(null);
};