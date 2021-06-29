using System;

public class FuelStations
{
	private const int MAX_REFUEL_JERRY_CAN_DISTANCE = 3;
	const float g_fDistThreshold = 5.0f;
	private bool m_bIsFueling = false;
	private bool m_bFuelingPendingRequest = false;

	public FuelStations()
	{
		RageEvents.RAGE_OnRender += OnRender;

		NetworkEvents.FuelingRequestResponse += OnFuelingRequestResponse;
		NetworkEvents.FuelingComplete += OnFuelingComplete;
		NetworkEvents.InitiateJerryCanRefuel += OnJerryCanRefuel;
	}

	enum ECanFuelResult
	{
		NotInVehicle,
		NotDriver,
		AlreadyFueling,
		TooMuchFuel,
		VehicleDead,
		CanFuel,
		CanFuelJerryCan
	}

	private ECanFuelResult CanFuel()
	{
		RAGE.Elements.Player localPlayer = RAGE.Elements.Player.LocalPlayer;

		float fFuelLevel = 0.0f;
		RAGE.Elements.Vehicle playerVehicle = null;
		var isDriver = false;
		if (localPlayer.IsInAnyVehicle(false))
		{
			playerVehicle = localPlayer.Vehicle;
			// TODO_RAGE: Fix this check
			//isDriver = g_FuelLastVehicleSeat == EVehicleSeat.Driver;
			isDriver = true;

			if (playerVehicle != null)
			{
				fFuelLevel = DataHelper.GetEntityData<float>(playerVehicle, EDataNames.FUEL);
			}
		}

		if (playerVehicle == null && !m_bIsFueling)
		{
			int hashInt = (int)WeaponHash.Petrolcan;
			if (localPlayer.GetCurrentWeapon(ref hashInt, false))
			{
				return ECanFuelResult.CanFuelJerryCan;
			}
			
			return ECanFuelResult.NotInVehicle;
		}
		else if (m_bIsFueling)
		{
			return ECanFuelResult.AlreadyFueling;
		}
		else if (!isDriver)
		{
			return ECanFuelResult.NotDriver;
		}
		else if (playerVehicle.GetHealth() <= 0.0f)
		{
			return ECanFuelResult.VehicleDead;
		}
		else if (fFuelLevel > 99.0f)
		{
			return ECanFuelResult.TooMuchFuel;
		}

		return ECanFuelResult.CanFuel;
	}

	private void OnRender()
	{
		RAGE.Elements.Marker nearestFuelPoint = GetNearestFuelingPoint();
		if (nearestFuelPoint != null)
		{
			ECanFuelResult canFuelResult = CanFuel();

			string strMessage = String.Empty;
			ConsoleKey key = ConsoleKey.NoName;
			switch (canFuelResult)
			{
				case ECanFuelResult.NotInVehicle:
					strMessage = "You must be in a vehicle to refuel";
					key = ConsoleKey.NoName;
					break;
				case ECanFuelResult.NotDriver:
					strMessage = "You must be the driver to refuel";
					key = ConsoleKey.NoName;
					break;
				case ECanFuelResult.TooMuchFuel:
					strMessage = "The vehicle is already full";
					key = ConsoleKey.NoName;
					break;
				case ECanFuelResult.AlreadyFueling:
					strMessage = "Refueling";
					key = ConsoleKey.NoName;
					break;
				case ECanFuelResult.VehicleDead:
					strMessage = "Vehicle is too damaged";
					key = ConsoleKey.NoName;
					break;
				case ECanFuelResult.CanFuel:
					strMessage = "Refuel";
					key = ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact);
					break;
				case ECanFuelResult.CanFuelJerryCan:
					strMessage = "Refuel Jerry Can";
					key = ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact);
					break;
			}

			if (strMessage != String.Empty)
			{
				WorldHintManager.DrawExclusiveWorldHint(key, strMessage, null, InteractWithFuelPoint, nearestFuelPoint.Position, nearestFuelPoint.Dimension, false, false, g_fDistThreshold, bAllowInVehicle: true);
			}
		}
	}

	private void InteractWithFuelPoint()
	{
		ECanFuelResult canFuelResult = CanFuel();
		bool bCanFuel = canFuelResult == ECanFuelResult.CanFuel || canFuelResult == ECanFuelResult.CanFuelJerryCan;

		if (bCanFuel && !m_bIsFueling && !m_bFuelingPendingRequest)
		{
			// TODO: Could cache this for optimization, but probably doesn't matter since its a one off event here
			RAGE.Elements.Marker nearestFuelPoint = GetNearestFuelingPoint();
			if (nearestFuelPoint != null)
			{
				Int64 fuelPointID = DataHelper.GetEntityData<Int64>(nearestFuelPoint, EDataNames.FUEL_ID);
				NetworkEventSender.SendNetworkEvent_RequestFueling(fuelPointID);
				m_bFuelingPendingRequest = true;
			}
		}
	}

	private RAGE.Elements.Marker GetNearestFuelingPoint()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.FuelPoint);
		return poolEntry?.GetEntity<RAGE.Elements.Marker>();
	}

	private void OnFuelingRequestResponse(bool bSuccess)
	{
		m_bIsFueling = bSuccess;
		m_bFuelingPendingRequest = false;
	}

	private void OnFuelingComplete()
	{
		m_bIsFueling = false;
	}

	private void OnJerryCanRefuel(Int64 inventoryDBID)
	{
		RAGE.Elements.Vehicle vehicle = OptimizationCachePool.GetNearestVehicle();
		if (vehicle == null || PlayerHelper.GetLocalPlayerPosition().DistanceTo(vehicle.Position) > MAX_REFUEL_JERRY_CAN_DISTANCE)
		{
			NotificationManager.ShowNotification("Jerry Can Refuel", "You are not close enough to a vehicle.", ENotificationIcon.ExclamationSign);
			return;
		}

		Int64 id = DataHelper.GetEntityData<Int64>(vehicle, EDataNames.SCRIPTED_ID);
		NetworkEventSender.SendNetworkEvent_JerryCanRefuelVehicle(inventoryDBID, id);
	}
}