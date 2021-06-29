using System;

public class VehicleRepair
{
	const float g_fDistThreshold = 4.0f;
	private bool m_bIsRepairing = false;
	// TODO_POST_LAUNCH: Use these flags here + fuel etc? do we even need it?
	private bool m_bPendingRequest = false;

	public VehicleRepair()
	{
		RageEvents.RAGE_OnRender += OnRender;

		NetworkEvents.VehicleRepairRequestResponse += OnVehicleRepairRequestResponse;
		NetworkEvents.VehicleRepairComplete += OnVehicleRepairComplete;
	}

	enum ECanRepairResult
	{
		NotInVehicle,
		NotDriver,
		AlreadyRepairing,
		TooMuchHealth,
		VehicleDead,
		CanRepair
	}

	private ECanRepairResult CanRepair()
	{
		RAGE.Elements.Player localPlayer = RAGE.Elements.Player.LocalPlayer;

		bool bIsDamaged = false;
		RAGE.Elements.Vehicle playerVehicle = null;
		var isDriver = false;
		if (localPlayer.IsInAnyVehicle(false))
		{
			playerVehicle = localPlayer.Vehicle;
			// TODO_RAGE: Fix this check
			//isDriver = g_LastVehicleSeat == EVehicleSeat.Driver;
			isDriver = true;

			if (playerVehicle != null)
			{
				bIsDamaged = playerVehicle.IsDamaged();

				for (int i = 0; i < 5; i++)
				{
					if (playerVehicle.IsTyreBurst(i, false))
					{
						bIsDamaged = true;
					}
				}
			}
		}

		if (playerVehicle == null)
		{
			return ECanRepairResult.NotInVehicle;
		}
		else if (!isDriver)
		{
			return ECanRepairResult.NotDriver;
		}
		else if (m_bIsRepairing)
		{
			return ECanRepairResult.AlreadyRepairing;
		}
		else if (!bIsDamaged)
		{
			return ECanRepairResult.TooMuchHealth;
		}
		else if (playerVehicle.GetHealth() < 0.0f)
		{
			return ECanRepairResult.VehicleDead;
		}

		return ECanRepairResult.CanRepair;
	}

	private void OnRender()
	{
		RAGE.Elements.Marker nearestRepairPoint = GetNearestRepairPoint();
		if (nearestRepairPoint != null)
		{
			ECanRepairResult canRepairResult = CanRepair();

			string strMessage = String.Empty;
			ConsoleKey key = ConsoleKey.NoName;
			if (canRepairResult == ECanRepairResult.NotInVehicle)
			{
				strMessage = "You must be in a vehicle to use the Auto Repair";
				key = ConsoleKey.NoName;
			}
			else if (canRepairResult == ECanRepairResult.NotDriver)
			{
				strMessage = "You must be the driver to use the Auto Repair";
				key = ConsoleKey.NoName;
			}
			else if (canRepairResult == ECanRepairResult.TooMuchHealth)
			{
				strMessage = "The vehicle is not damaged";
				key = ConsoleKey.NoName;
			}
			else if (canRepairResult == ECanRepairResult.AlreadyRepairing)
			{
				strMessage = "Vehicle is Repairing";
				key = ConsoleKey.NoName;
			}
			else if (canRepairResult == ECanRepairResult.VehicleDead)
			{
				strMessage = "Vehicle is irreparable";
				key = ConsoleKey.NoName;
			}
			else if (canRepairResult == ECanRepairResult.CanRepair)
			{
				strMessage = "Repair";
				key = ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact);
			}

			if (strMessage != String.Empty)
			{
				WorldHintManager.DrawExclusiveWorldHint(key, strMessage, null, InteractWithRepairPoint, nearestRepairPoint.Position, nearestRepairPoint.Dimension, false, false, g_fDistThreshold, bAllowInVehicle: true);
			}
		}
	}

	private void InteractWithRepairPoint()
	{
		ECanRepairResult canRepairResult = CanRepair();

		if (canRepairResult == ECanRepairResult.CanRepair && !m_bIsRepairing && !m_bPendingRequest)
		{
			// TODO: Could cache this for optimization, but probably doesn't matter since its a one off event here
			RAGE.Elements.Marker nearestRepairPoint = GetNearestRepairPoint();
			if (nearestRepairPoint != null)
			{
				Int64 repairPointID = DataHelper.GetEntityData<Int64>(nearestRepairPoint, EDataNames.VEHREP_ID);
				NetworkEventSender.SendNetworkEvent_RequestVehicleRepair(repairPointID);
				m_bPendingRequest = true;
			}
		}
	}

	private RAGE.Elements.Marker GetNearestRepairPoint()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.VehicleRepair);
		return poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.Marker>() : null;
	}

	private void OnVehicleRepairRequestResponse(bool bSuccess)
	{
		m_bIsRepairing = bSuccess;
		m_bPendingRequest = false;
	}

	private void OnVehicleRepairComplete()
	{
		m_bIsRepairing = false;
	}
}