using GTANetworkAPI;
using System;
using System.Collections.Generic;

using EntityDatabaseID = System.Int64;

public class CCarWashPoint
{
	public CCarWashPoint(EntityDatabaseID a_DatabaseID, Vector3 a_vecPos, uint a_Dimension)
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

		Database.LegacyFunctions.DestroyCarWashPoint(DatabaseID);
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

		m_Marker = NAPI.Marker.CreateMarker(36, Position, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f), 1.0f, new Color(255, 255, 255, 100), false, Dimension);
		EntityDataManager.SetData(m_Marker, EDataNames.CARWASH_POINT, true, EDataType.Synced);
		EntityDataManager.SetData(m_Marker, EDataNames.CARWASH_ID, DatabaseID, EDataType.Synced);

		m_Blip = HelperFunctions.Blip.Create(Position, true, 50.0f, Dimension, "Car Wash", 100);
	}

	public EntityDatabaseID DatabaseID { get; set; }
	public Vector3 Position { get; set; }

	private Marker m_Marker = null;
	public uint Dimension { get; set; }
	private Blip m_Blip = null;
}

public class CarWashStations
{
	public CarWashStations()
	{
		NetworkEvents.RequestCarWashing += OnRequestCarWashing;

		LoadAllCarWashPoints();
	}

	public void OnRequestCarWashing(CPlayer player, Int64 carWashID)
	{
		// TODO_CSHARP: Fix cast
		CCarWashPoint carwashPoint = m_dictInstances[carWashID];

		// TODO: Check distance, in vehicle, etc, all same checks that client has
		if (carwashPoint != null)
		{
			bool bSuccess = true;

			// Are we already using a car wash?
			if (m_dictCarWashingActions.ContainsKey(player))
			{
				bSuccess = false;
			}

			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
			if (pVehicle == null)
			{
				bSuccess = false;
			}

			if (pVehicle != null && !pVehicle.DoesVehicleGetDirty())
			{
				player.SendNotification("Car Wash", ENotificationIcon.ExclamationSign, "This type of vehicle can not be washed.", null);
				bSuccess = false;
			}

			NetworkEventSender.SendNetworkEvent_CarWashingRequestResponse(player, bSuccess);

			if (bSuccess)
			{
				CCarWashingAction carwashAction = new CCarWashingAction(player, pVehicle, carwashPoint);
				m_dictCarWashingActions.Add(player, carwashAction);

				pVehicle.IsBeingWashed = true;
			}
		}
	}

	public async void LoadAllCarWashPoints()
	{
		List<CDatabaseStructureCarWashPoint> lstPoints = await Database.LegacyFunctions.LoadAllCarWashPoints().ConfigureAwait(true);

		NAPI.Task.Run(() =>
		{
			foreach (var point in lstPoints)
			{
				CreateCarWashPoint(point.dbID, point.vecPos, point.dimension);
			}
		});

		NAPI.Util.ConsoleOutput("[CARWASH POINTS] Loaded {0} CarWash Points!", lstPoints.Count);
	}

	public void CreateCarWashPoint(EntityDatabaseID a_DatabaseID, Vector3 a_vecPos, uint a_Dimension)
	{
		CCarWashPoint newInst = new CCarWashPoint(a_DatabaseID, a_vecPos, a_Dimension);
		m_dictInstances.Add(a_DatabaseID, newInst);
	}

	public void DestroyCarWashPoint(CCarWashPoint a_Inst)
	{
		a_Inst.Destroy();
		m_dictInstances.Remove(a_Inst.DatabaseID);
	}

	public static void OnCarWashingActionComplete(CPlayer a_RequestingPlayer, CVehicle a_Vehicle)
	{
		a_Vehicle.IsBeingWashed = false;

		NetworkEventSender.SendNetworkEvent_CarWashingComplete(a_RequestingPlayer);
		m_dictCarWashingActions.Remove(a_RequestingPlayer);
	}

	private Dictionary<EntityDatabaseID, CCarWashPoint> m_dictInstances = new Dictionary<EntityDatabaseID, CCarWashPoint>();
	private static Dictionary<CPlayer, CCarWashingAction> m_dictCarWashingActions = new Dictionary<CPlayer, CCarWashingAction>();

	public static float gs_fDistThreshold = 5.0f; // NOTE: If you change this, change carwash_system.ts also
	public static float gs_fCarWashCost = 20.0f; // TODO: increase by vehicle size
}

internal class CCarWashingAction : IDisposable
{
	public CCarWashingAction(CPlayer a_RequestingPlayer, CVehicle a_Vehicle, CCarWashPoint a_CarWashPoint)
	{
		// NOTE: We bind this timer to the vehicle, so even if the player quits, the IsBeingWashed flag gets set back to false and people don't get a dirt-free vehicle for the duration of the server
		m_Timer = MainThreadTimerPool.CreateEntityTimer(OnCarWashingActionComplete, 12000, a_Vehicle, 1);

		m_RequestingPlayer.SetTarget(a_RequestingPlayer);
		m_RequestingPlayerVehicle = a_Vehicle;
		m_CarWashPoint = a_CarWashPoint;

		m_fDirtBeforeWash = a_Vehicle.Dirt;
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
	// TODO: Sales tax on car wash?
	private void OnCarWashingActionComplete(object[] a_Parameters = null)
	{
		bool bSucceeded = false;

		CPlayer requestingPlayer = m_RequestingPlayer.Instance();
		if (requestingPlayer == null)
		{
			return;
		}

		if (m_RequestingPlayerVehicle != null && m_RequestingPlayerVehicle != null && m_CarWashPoint != null)
		{
			// Is the player still in the same vehicle?
			CVehicle pVehicle = requestingPlayer.Client.Vehicle != null ? VehiclePool.GetVehicleFromGTAInstance(requestingPlayer.Client.Vehicle) : null;

			if (pVehicle != m_RequestingPlayerVehicle)
			{
				requestingPlayer.SendNotification("Car Wash", ENotificationIcon.ExclamationSign, "You are no longer in the same vehicle.", null);
			}
			else
			{
				if (requestingPlayer.IsWithinDistanceOf(m_CarWashPoint.Position, CarWashStations.gs_fDistThreshold, m_CarWashPoint.Dimension))
				{
					bool bBilledToFaction = false;
					// Is it a faction vehicle for the requesting player
					if (pVehicle.IsVehicleForAnyPlayerFaction(requestingPlayer))
					{
						CFaction pOwningFaction = pVehicle.GetFactionOwner();

						if (pOwningFaction != null)
						{
							bBilledToFaction = true;

							if (pOwningFaction.SubtractMoney(CarWashStations.gs_fCarWashCost))
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
						if (requestingPlayer.SubtractMoney(CarWashStations.gs_fCarWashCost, PlayerMoneyModificationReason.CarWash))
						{
							bSucceeded = true;
						}
					}


					if (bSucceeded)
					{
						if (pVehicle.IsJobVehicle())
						{
							requestingPlayer.SendNotification("Car Wash", ENotificationIcon.InfoSign, "Your vehicle has been washed. (Paid by Employer)");
						}
						else
						{
							requestingPlayer.SendNotification("Car Wash", ENotificationIcon.InfoSign, "Your vehicle has been washed for ${0:0.00}{1}", CarWashStations.gs_fCarWashCost, bBilledToFaction ? " (Paid by Faction)" : "");
						}
					}
					else
					{
						if (bBilledToFaction)
						{
							requestingPlayer.SendNotification("Car Wash", ENotificationIcon.ExclamationSign, "Your faction cannot afford a car wash (Cost: ${0:0.00}).", CarWashStations.gs_fCarWashCost);
						}
						else
						{
							requestingPlayer.SendNotification("Car Wash", ENotificationIcon.ExclamationSign, "You cannot afford a car wash (Cost: ${0:0.00}).", CarWashStations.gs_fCarWashCost);
						}
					}
				}
				else
				{
					requestingPlayer.SendNotification("Car Wash", ENotificationIcon.ExclamationSign, "You are no longer in the car wash circle.", null);
				}
			}
		}

		if (bSucceeded)
		{
			m_RequestingPlayerVehicle.Dirt = 0.0f;
			m_RequestingPlayerVehicle.Save();
		}
		else
		{
			// Rollback dirt to reset clientside effect
			m_RequestingPlayerVehicle.Dirt = m_fDirtBeforeWash;
		}

		CarWashStations.OnCarWashingActionComplete(requestingPlayer, m_RequestingPlayerVehicle);
	}

	private WeakReference<CPlayer> m_RequestingPlayer = new WeakReference<CPlayer>(null);
	private CVehicle m_RequestingPlayerVehicle = null;
	private CCarWashPoint m_CarWashPoint = null;
	private WeakReference<MainThreadTimer> m_Timer = new WeakReference<MainThreadTimer>(null);
	private readonly float m_fDirtBeforeWash = 0.0f;
};