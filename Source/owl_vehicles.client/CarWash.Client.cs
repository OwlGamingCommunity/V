using System;

public class CarWash
{
	const float g_fDistThreshold = 4.0f;
	private bool m_bIsWashing = false;
	// TODO_POST_LAUNCH: Use these flags here + fuel etc? do we even need it?
	private bool m_bPendingRequest = false;

	public CarWash()
	{
		RageEvents.RAGE_OnRender += OnRender;

		NetworkEvents.CarWashingRequestResponse += OnCarWashingRequestResponse;
		NetworkEvents.CarWashingComplete += OnCarWashingComplete;
	}

	enum ECanWashCarResult
	{
		NotInVehicle,
		NotDriver,
		AlreadyWashing,
		NotEnoughDirt,
		VehicleDead,
		CanWash
	}

	private ECanWashCarResult CanWashCar()
	{
		RAGE.Elements.Player localPlayer = RAGE.Elements.Player.LocalPlayer;

		float fDirt = 0.0f;
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
				fDirt = DataHelper.GetEntityData<float>(playerVehicle, EDataNames.DIRT);
			}
		}

		if (playerVehicle == null)
		{
			return ECanWashCarResult.NotInVehicle;
		}
		else if (!isDriver)
		{
			return ECanWashCarResult.NotDriver;
		}
		else if (m_bIsWashing)
		{
			return ECanWashCarResult.AlreadyWashing;
		}
		else if (fDirt < 0.25f)
		{
			return ECanWashCarResult.NotEnoughDirt;
		}
		else if (playerVehicle.GetHealth() <= 0.0f)
		{
			return ECanWashCarResult.VehicleDead;
		}

		return ECanWashCarResult.CanWash;
	}

	private void OnRender()
	{
		RAGE.Elements.Marker nearestCarWash = GetNearestCarWash();
		if (nearestCarWash != null)
		{
			ECanWashCarResult canWashCarResult = CanWashCar();

			string strMessage = String.Empty;
			ConsoleKey key = ConsoleKey.NoName;
			if (canWashCarResult == ECanWashCarResult.NotInVehicle)
			{
				strMessage = "You must be in a vehicle to use the Car Wash";
				key = ConsoleKey.NoName;
			}
			else if (canWashCarResult == ECanWashCarResult.NotDriver)
			{
				strMessage = "You must be the driver to use the Car Wash";
				key = ConsoleKey.NoName;
			}
			else if (canWashCarResult == ECanWashCarResult.NotEnoughDirt)
			{
				strMessage = "The vehicle is not dirty";
				key = ConsoleKey.NoName;
			}
			else if (canWashCarResult == ECanWashCarResult.AlreadyWashing)
			{
				strMessage = "Vehicle is being cleaned";
				key = ConsoleKey.NoName;
			}
			else if (canWashCarResult == ECanWashCarResult.VehicleDead)
			{
				strMessage = "Vehicle is too damaged";
				key = ConsoleKey.NoName;
			}
			else if (canWashCarResult == ECanWashCarResult.CanWash)
			{
				strMessage = "Clean Vehicle";
				key = ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact);
			}

			if (strMessage != String.Empty)
			{
				WorldHintManager.DrawExclusiveWorldHint(key, strMessage, null, InteractWithCarWash, nearestCarWash.Position, nearestCarWash.Dimension, false, false, g_fDistThreshold, bAllowInVehicle: true);
			}
		}
	}

	private void InteractWithCarWash()
	{
		ECanWashCarResult canWashCarResult = CanWashCar();

		if (canWashCarResult == ECanWashCarResult.CanWash && !m_bIsWashing && !m_bPendingRequest)
		{
			// TODO: Could cache this for optimization, but probably doesn't matter since its a one off event here
			RAGE.Elements.Marker nearestCarWash = GetNearestCarWash();
			if (nearestCarWash != null)
			{
				Int64 nearestCarWashID = DataHelper.GetEntityData<Int64>(nearestCarWash, EDataNames.CARWASH_ID);
				NetworkEventSender.SendNetworkEvent_RequestCarWashing(nearestCarWashID);
				m_bPendingRequest = true;
			}
		}
	}

	private RAGE.Elements.Marker GetNearestCarWash()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.CarWash);
		return poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.Marker>() : null;
	}

	private void OnCarWashingRequestResponse(bool bSuccess)
	{
		m_bIsWashing = bSuccess;
		m_bPendingRequest = false;

		if (bSuccess)
		{
			RAGE.Elements.Vehicle playerVehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
			float fDist = 2.0f;

			if (playerVehicle != null)
			{
				// under
				RAGE.Vector3 vecPosOrig = playerVehicle.Position;
				vecPosOrig.Z = WorldHelper.GetGroundPosition(vecPosOrig);
				RAGE.Game.Fire.AddExplosion(vecPosOrig.X, vecPosOrig.Y, vecPosOrig.Z, 13, 1.0f, true, false, 0.0f, true);

				// under front
				RAGE.Vector3 vecPosCalc = vecPosOrig;
				float fRadians = (playerVehicle.GetRotation(0).Z + 90.0f) * (3.14f / 180.0f);
				vecPosCalc.X += (float)Math.Cos(fRadians) * fDist;
				vecPosCalc.Y += (float)Math.Sin(fRadians) * fDist;
				vecPosCalc.Z = WorldHelper.GetGroundPosition(vecPosCalc);
				RAGE.Game.Fire.AddExplosion(vecPosCalc.X, vecPosCalc.Y, vecPosCalc.Z, 13, 1.0f, true, false, 0.0f, true);

				// under back
				RAGE.Vector3 vecPosCalc2 = vecPosOrig;
				float fRadians2 = (playerVehicle.GetRotation(0).Z - 90.0f) * (3.14f / 180.0f);
				vecPosCalc2.X += (float)Math.Cos(fRadians2) * fDist;
				vecPosCalc2.Y += (float)Math.Sin(fRadians2) * fDist;
				vecPosCalc2.Z = WorldHelper.GetGroundPosition(vecPosCalc2);
				RAGE.Game.Fire.AddExplosion(vecPosCalc2.X, vecPosCalc2.Y, vecPosCalc2.Z, 13, 1.0f, true, false, 0.0f, true);


				ClientTimerPool.CreateTimer(CarWashTimerTask, 100, 100, new object[] { playerVehicle });
			}
		}
	}

	private void CarWashTimerTask(object[] parameters)
	{
		if (parameters[0] != null)
		{
			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)parameters[0];
			vehicle.SetDirtLevel(vehicle.GetDirtLevel() - 0.135f);
		}
	}

	private void OnCarWashingComplete()
	{
		m_bIsWashing = false;
	}
}