using System;
using System.Collections.Generic;
using System.Linq;

public class VehicleSystemGeneral
{
	private WeakReference<CWorldPed> m_refWorldPed = new WeakReference<CWorldPed>(null);
	private int m_currentVehGear = 0;
	private bool m_isManual = false;

	private void ApplyVehicleExtras(RAGE.Elements.Vehicle vehicle, string strJSONData)
	{
		if (strJSONData != null)
		{
			try
			{
				List<int> lstExtras = OwlJSON.DeserializeObject<List<int>>(strJSONData, EJsonTrackableIdentifier.VehicleExtras);
				for (int i = 0; i < VehicleConstants.NumExtras; ++i)
				{
					if (lstExtras.Contains(i))
					{
						vehicle.SetExtra(i, true);
					}
					else
					{
						vehicle.SetExtra(i, false);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionHelper.SendException(e);
			}
		}
	}

	public VehicleSystemGeneral()
	{
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleEngine, OnToggleEngine);
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleLock, OnToggleLock);
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleHeadlights, OnToggleHeadlights);
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleWindows, OnToggleWindows);
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleHandbrake, OnToggleHandbrake);

		RageEvents.AddDataHandler(EDataNames.VEH_EXTRAS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { ApplyVehicleExtras((RAGE.Elements.Vehicle)entity, (string)newValue); });
		RageEvents.RAGE_OnEntityStreamIn += (RAGE.Elements.Entity entity) =>
		{
			if (entity.Type == RAGE.Elements.Type.Vehicle)
			{
				RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;

				// apply extras
				string strJSONData = DataHelper.GetEntityData<string>(vehicle, EDataNames.VEH_EXTRAS);
				ApplyVehicleExtras(vehicle, strJSONData);

				float fTrueZ = DataHelper.GetEntityData<float>(vehicle, EDataNames.VEH_Z_FIX);
				float fDiff = Math.Abs(fTrueZ - vehicle.Position.Z);

				if (fDiff > 0.5f)
				{
					vehicle.Position.Z = fTrueZ;
				}
			}
		};

		// TODO_CSHARP: Flag to allow/disallow triggeing when keybinds disabled, chat visible etc. Would remove all these if statements.
		ScriptControls.SubscribeToControl(EScriptControlID.EnterVehicleDriver, (EControlActionType actionType) => { if (!KeyBinds.IsChatInputVisible() && KeyBinds.CanProcessKeybinds()) { RequestEnterVehicle(false, true); } });
		ScriptControls.SubscribeToControl(EScriptControlID.EnterVehiclePassenger, (EControlActionType actionType) => { if (!KeyBinds.IsChatInputVisible() && KeyBinds.CanProcessKeybinds()) { RequestEnterVehicle(false, false); } });

		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
		NetworkEvents.EnterVehicleReal += OnEnterVehicleReal;
		NetworkEvents.ExitVehicleReal += OnExitVehicle;

		// Seatbelt
		NetworkEvents.ToggleSeatbelt += OnToggleSeatbelt;
		NetworkEvents.SyncVehicleHandbrakeSound += OnPlayHandbrakeSound;

		// TODO_CSHARP: Type checking of entity
		// TODO_CSHARP: Wrap this? Use EDataName, strings are going away.
		RageEvents.AddDataHandler(EDataNames.VEH_DOOR_0, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateVehicleDoorState((RAGE.Elements.Vehicle)entity, 0, Convert.ToBoolean(newValue)); });
		RageEvents.AddDataHandler(EDataNames.VEH_DOOR_1, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateVehicleDoorState((RAGE.Elements.Vehicle)entity, 1, Convert.ToBoolean(newValue)); });
		RageEvents.AddDataHandler(EDataNames.VEH_DOOR_2, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateVehicleDoorState((RAGE.Elements.Vehicle)entity, 2, Convert.ToBoolean(newValue)); });
		RageEvents.AddDataHandler(EDataNames.VEH_DOOR_3, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateVehicleDoorState((RAGE.Elements.Vehicle)entity, 3, Convert.ToBoolean(newValue)); });
		RageEvents.AddDataHandler(EDataNames.VEH_DOOR_4, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateVehicleDoorState((RAGE.Elements.Vehicle)entity, 4, Convert.ToBoolean(newValue)); });
		RageEvents.AddDataHandler(EDataNames.VEH_DOOR_5, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateVehicleDoorState((RAGE.Elements.Vehicle)entity, 5, Convert.ToBoolean(newValue)); });

		RageEvents.AddDataHandler(EDataNames.VEHICLE_WINDOWS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateVehicleWindowState((RAGE.Elements.Vehicle)entity, Convert.ToBoolean(newValue)); });

		RageEvents.AddDataHandler(EDataNames.HEADLIGHTS, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateLightsState_WithValue((RAGE.Elements.Vehicle)entity, (EHeadlightState)newValue); });
		RageEvents.AddDataHandler(EDataNames.SIREN_STATE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateSirensState_WithValue((RAGE.Elements.Vehicle)entity, (bool)newValue); });

		RageEvents.RAGE_OnTick_PerFrame += () =>
		{
			RAGE.Elements.Vehicle currentVehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
			if (currentVehicle != null)
			{
				bool bEngineOn = DataHelper.GetEntityData<bool>(currentVehicle, EDataNames.ENGINE);

				if (bEngineOn != currentVehicle.GetIsEngineRunning())
				{
					// Undriveable if engine is off (this stops auto-start engine code in GTA from executing)
					RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleEngineOn, currentVehicle.Handle, bEngineOn, true, false);
					RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleUndriveable, currentVehicle.Handle, !bEngineOn);
				}
			}
		};

		RageEvents.RAGE_OnTick_OncePerSecond += () =>
		{
			foreach (var vehicle in OptimizationCachePool.StreamedInVehicles())
			{
				UpdateVehicleDrivableAndLocks(vehicle);
				UpdateLightsState(vehicle);
			}
		};

		RageEvents.RAGE_OnTick_PerFrame += () =>
		{
			foreach (var vehicle in OptimizationCachePool.StreamedInVehicles())
			{
				UpdateManualVehicleStates(vehicle);
				bool bHandbrakeOn = DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEHICLE_HANDBRAKE);
				RAGE.Game.Vehicle.SetVehicleHandbrake(vehicle.Handle, bHandbrakeOn);
			}
		};
	}

	private void UpdateVehicleDrivableAndLocks(RAGE.Elements.Vehicle vehicle)
	{
		// Stops autostart
		bool bEngineOn = DataHelper.GetEntityData<bool>(vehicle, EDataNames.ENGINE);
		RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleUndriveable, vehicle.Handle, !bEngineOn);

		if (bEngineOn != vehicle.GetIsEngineRunning())
		{
			vehicle.SetEngineOn(bEngineOn, true, false);
		}

		// lock states
		int doorLockStatus = vehicle.GetDoorLockStatus();
		if (doorLockStatus == 2)
		{
			if (vehicle == RAGE.Elements.Player.LocalPlayer.Vehicle)
			{
				vehicle.SetDoorsLocked(4);
			}
		}
		else if (doorLockStatus == 4)
		{
			if (vehicle != RAGE.Elements.Player.LocalPlayer.Vehicle)
			{
				vehicle.SetDoorsLocked(2);
			}
		}
	}

	private void OnExitVehicle(RAGE.Elements.Vehicle vehicle)
	{
		if (vehicle == null || RAGE.Elements.Player.LocalPlayer.Vehicle == null)
		{
			return;
		}

		if (m_bIsDriver)
		{
			if (WorldHelper.IsPositionConsideredAbandoned(vehicle.Position))
			{
				NotificationManager.ShowNotification("Vehicle Notice", "Parking this vehicle here would block a roadway and your vehicle will potentially be towed.", ENotificationIcon.InfoSign);
			}

			// TODO_CSHARP: Check vehicle handle is still valid, might have to cache it
			// Prevent default GTAV behaviour changing engine state when leaving the vehicle
			RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleEngineOn, RAGE.Elements.Player.LocalPlayer.Vehicle.Handle, RAGE.Elements.Player.LocalPlayer.Vehicle.GetIsEngineRunning(), true, false);

			// Reset undriveable state since being undriveable also stops entry
			RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleUndriveable, RAGE.Elements.Player.LocalPlayer.Vehicle.Handle, false);

			m_bIsDriver = false;
			m_isManual = false;
			vehicle.Rpm = vehicle.GetIsEngineRunning() ? 0.1f : 0.0f;
			NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
			NetworkEventSender.SendNetworkEvent_SyncManualVehBrakes(false);
		}
	}

	private bool DoesVehicleSupportHanging(RAGE.Elements.Vehicle vehicle)
	{
		uint hash = vehicle.Model;

		// SWAT trucks, FBI SUV, SHERIFF SUV
		if (hash == 3089277354
			|| hash == 2601952180
			|| hash == 1922257928
			|| hash == 2647026068
			|| hash == 1938952078) // fire truck
		{
			return true;
		}

		return false;
	}

	private bool IsBigVehicle(RAGE.Elements.Vehicle vehicle)
	{
		bool isBigVehicle = false;
		uint hash = vehicle.Model;
		int vehClass = vehicle.GetClass();

		if (hash == 3089277354
			|| hash == 2601952180)
		{
			isBigVehicle = true;
		}
		else if (vehClass == 10 || vehClass == 11 || vehClass == 12 || vehClass == 14 || vehClass == 15 || vehClass == 16 || vehClass == 17 || vehClass == 19 || vehClass == 20 || vehClass == 21)
		{
			isBigVehicle = true;
		}
		else if (IsBus(vehicle))
		{
			isBigVehicle = true;
		}

		return isBigVehicle;
	}

	private bool IsBus(RAGE.Elements.Vehicle vehicle)
	{
		return (vehicle.Model == 3581397346 || vehicle.Model == 1941029835 || vehicle.Model == 345756458 || vehicle.Model == 1283517198 || vehicle.Model == 2287941233 || vehicle.Model == 3196165219);
	}

	private void RequestEnterVehicle(bool bIsFromWorldHint = false, bool bIsDriver = true)
	{
		if (m_NearestVehicle != null)
		{
			if (IsVehicleWithoutGTALocks(m_NearestVehicle) && m_NearestVehicle.GetDoorLockStatus() == 2)
			{
				NotificationManager.ShowNotification("Vehicle Locked", "That vehicle is locked", ENotificationIcon.ExclamationSign);
				return;
			}

			if (bIsDriver)
			{
				m_NearestSeat = EVehicleSeat.Driver;
			}

			m_bIsEnteringVehicle = true;
			m_vehicleBeingEntered = m_NearestVehicle;
			bool bRunning = RAGE.Elements.Player.LocalPlayer.IsRunning();

			// seat is -1 because GTA still starts at -1 for driver, rage doesnt :)
			RAGE.Elements.Player.LocalPlayer.TaskEnterVehicle(m_NearestVehicle.Handle, 10000, (int)m_NearestSeat - 1, bRunning ? 2.0f : 1.0f, 1, 0);
			if (m_tmrEnterVehicleTimeout != null)
			{
				ClientTimerPool.MarkTimerForDeletion(ref m_tmrEnterVehicleTimeout);
				m_tmrEnterVehicleTimeout = null;
			}

			m_tmrEnterVehicleTimeout = ClientTimerPool.CreateTimer(OnVehicleEnterTimeout, 5000, 1);
		}
		else if (RAGE.Elements.Player.LocalPlayer.Vehicle != null) // Are we currently trying to exit a locked vehicle?
		{
			int lockStatus = RAGE.Elements.Player.LocalPlayer.Vehicle.GetDoorLockStatus();
			if (lockStatus == 2 || lockStatus == 4)
			{
				NotificationManager.ShowNotification("Vehicle", "You cannot exit the vehicle due to it being locked.", ENotificationIcon.ExclamationSign);
			}
		}
	}

	private RAGE.Elements.Vehicle GetNearestVehicleEligibleForEntry(float fDistThreshold)
	{
		RAGE.Elements.Vehicle nearestVehicle = null;
		float fSmallestDistance = 999999.0f;

		foreach (RAGE.Elements.Vehicle vehicle in OptimizationCachePool.StreamedInVehicles())
		{
			// Same dimension?
			// TODO_RAGE: This is bugged when teleporting, but we dont need to check dim because we are only using streamed in vehicles (which are guaranteed to be in our dim)
			//if (RAGE.Elements.Player.LocalPlayer.Dimension == vehicle.Dimension)
			{
				RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;

				// is the vehicle nearby?
				float fDistance = WorldHelper.GetDistance(vecPlayerPos, vehicle.Position);
				if (fDistance <= fDistThreshold)
				{
					if (fDistance <= fSmallestDistance && vehicle.GetSpeed() < 5) // Nearby and not moving much
					{
						fSmallestDistance = fDistance;
						nearestVehicle = vehicle;
					}
				}
			}
		}
		return nearestVehicle;
	}

	private void OnVehicleEnterTimeout(object[] parameters)
	{
		if (m_bIsEnteringVehicle)
		{
			ResetEnteringVehicleFlag();
			RAGE.Elements.Player.LocalPlayer.ClearTasks();

			m_tmrEnterVehicleTimeout = null;
		}
	}

	private float GetEnterThreshold(RAGE.Elements.Vehicle vehicle)
	{
		return IsBigVehicle(vehicle) ? g_fEnterThreshold_Big : g_fEnterThreshold;
	}

	private void UpdateEnterVehicleSystem()
	{
		RAGE.Game.Pad.DisableControlAction(0, 23, true);

		m_NearestVehicle = null;
		m_NearestSeat = 0;

		if (m_bIsEnteringVehicle)
		{
			bool bShouldCancel = false;

			// Did we cancel via moving?
			if (RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveLeft)
				|| RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveRight)
				|| RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveUp)
				|| RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveDown))
			{
				bShouldCancel = true;
			}

			// Does the vehicle still exist?
			// TODO_RAGE_HACK: GetAtHandle is broken
			if (m_vehicleBeingEntered == null || OptimizationCachePool.StreamedInVehicles().FirstOrDefault(x => x.Handle == m_vehicleBeingEntered.Handle) == null)
			{
				bShouldCancel = true;
			}
			else // Are we out of range? Perhaps the vehicle was moving
			{
				float fDist = WorldHelper.GetDistance(m_vehicleBeingEntered.Position, RAGE.Elements.Player.LocalPlayer.Position);
				if (fDist > GetEnterThreshold(m_vehicleBeingEntered) * 2.0f) // We double this here, some larger vehicles, the player goes out of range as he navigates the chassis
				{
					bShouldCancel = true;
				}
			}

			if (bShouldCancel)
			{
				RAGE.Elements.Player.LocalPlayer.ClearTasks();
				m_bIsEnteringVehicle = false;
			}
		}

		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null || m_bIsEnteringVehicle)
		{
			return;
		}

		RAGE.Elements.Vehicle vehicle = GetNearestVehicleEligibleForEntry(g_fEnterThreshold_Big);
		if (vehicle != null)
		{
			bool bHeliWithExtraSeats = vehicle.Model == 2634305738 || vehicle.Model == 353883353 || vehicle.Model == 837858166;

			RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
			const int maxPassengerDoors = 8;
			RAGE.Vector3[] vecDoorPositions = new RAGE.Vector3[maxPassengerDoors];
			float[] arrDoorDistances = new float[maxPassengerDoors];

			bool bHasDoors = vehicle.GetBoneIndexByName("seat_dside_f") != -1;
			bool bUseVehiclePos = !bHasDoors && vehicle.GetBoneIndexByName("bodyshell") != -1;

			if (!bUseVehiclePos)
			{
				string strDriverEntryName = bHasDoors ? "seat_dside_f" : "bodyshell";
				string strPassengerRightName = bHasDoors ? "seat_pside_f" : "bodyshell";
				// TODO_CSHARP: Helper function to get dist between two vectors
				// DRIVER
				vecDoorPositions[0] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName(strDriverEntryName));
				arrDoorDistances[0] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[0]);

				// PASSENGER RIGHT
				vecDoorPositions[1] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName(strPassengerRightName));
				arrDoorDistances[1] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[1]);

				// DRIVER REAR
				vecDoorPositions[2] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("seat_dside_r"));
				arrDoorDistances[2] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[2]);

				// PASSENGER REAR
				vecDoorPositions[3] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("seat_pside_r"));
				arrDoorDistances[3] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[3]);
			}
			else
			{
				// set everything to the vehicle root pos
				vecDoorPositions[0] = vehicle.GetCoords(false);
				arrDoorDistances[0] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[0]);

				vecDoorPositions[1] = vehicle.GetCoords(false);
				arrDoorDistances[1] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[1]);

				vecDoorPositions[2] = vehicle.GetCoords(false);
				arrDoorDistances[2] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[2]);

				vecDoorPositions[3] = vehicle.GetCoords(false);
				arrDoorDistances[3] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[3]);
			}

			if (DoesVehicleSupportHanging(vehicle) || (bHeliWithExtraSeats))
			{
				if (bHeliWithExtraSeats)
				{
					// HANGING LEFT
					vecDoorPositions[4] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_lr"));
					vecDoorPositions[4].Z += 1.0f;
					arrDoorDistances[4] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[4]);

					// HANGING RIGHT
					vecDoorPositions[5] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_rr"));
					vecDoorPositions[5].Z += 1.0f;
					arrDoorDistances[5] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[5]);
				}
				else
				{
					// HANGING LEFT
					vecDoorPositions[4] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_lr"));
					vecDoorPositions[4].Z += 1.0f;
					arrDoorDistances[4] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[4]);

					// HANGING RIGHT
					vecDoorPositions[5] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_rr"));
					vecDoorPositions[5].Z += 1.0f;
					arrDoorDistances[5] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[5]);

					// HANGING LEFT
					vecDoorPositions[6] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_lr"));
					vecDoorPositions[6].Z += 1.0f;
					arrDoorDistances[6] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[6]);

					// HANGING RIGHT
					vecDoorPositions[7] = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_rr"));
					vecDoorPositions[7].Z += 1.0f;
					arrDoorDistances[7] = WorldHelper.GetDistance(vecPlayerPos, vecDoorPositions[7]);
				}
			}

			/*
			int index = 0;
			foreach (var vecDoorPos in vecDoorPositions)
			{
				if (vecDoorPos != null)
				{
					TextHelper.Draw3D(Helpers.FormatString("Door {0}", index), vecDoorPos, 0.3f, new RAGE.RGBA(255, 0, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, false, false);
					++index;
				}
			}
			*/

			float SmallestDistance = 999999.0f;
			for (int i = 0; i < maxPassengerDoors; ++i) // We skip driver, since there is only one, we dont need to determine a 'nearest seat'
			{
				if (arrDoorDistances[i] > 0)
				{
					if (arrDoorDistances[i] < SmallestDistance)
					{
						// Check the vehicle supports hanging, skip hanging slots if not
						if (i < 4 || DoesVehicleSupportHanging(vehicle) || (bHeliWithExtraSeats && i < 6))
						{
							// We need smallest distance for driver AND passengers to determine the nearest vehicle
							// but we DONT save nearest seat when its driver, because enter as driver doesnt need a seat
							if (i != 0)
							{
								// Is the seat free?
								bool bSeatEmpty = vehicle.IsSeatFree(i - 1, 0);
								if (bSeatEmpty)
								{
									SmallestDistance = arrDoorDistances[i];
									m_NearestSeat = (EVehicleSeat)i;
								}
							}

							// always store nearest veh
							m_NearestVehicle = vehicle;
						}
					}
				}
			}

			// DEBUG DRAW
			/*
			for (int i= 0; i < maxPassengerDoors; ++i)
			{
				float fDist = arrDoorDistances[i];
				Vector3 vecPos = vecDoorPositions[i];

				if (vecPos != null)
				{
					bool bIsNearest = fDist == SmallestDistance;

					TextHelper.Draw3D(Helpers.FormatString("Door {0}", i), vecPos, 0.6f, bIsNearest ? new RGBA(0, 255, 0) : new RGBA(255, 0, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, false, false);
				}
			}
			*/

			// TODO: check for big vehicle
			var distThreshold = GetEnterThreshold(vehicle);
			if (SmallestDistance < distThreshold)
			{
				if (m_NearestSeat == EVehicleSeat.HangingLeft || m_NearestSeat == EVehicleSeat.HangingLeft2 || m_NearestSeat == EVehicleSeat.HangingRight || m_NearestSeat == EVehicleSeat.HangingRight2)
				{
					WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.G, bHeliWithExtraSeats ? "Enter Rear Seating" : "Hang On Side", null, null, vecDoorPositions[(int)m_NearestSeat], RAGE.Elements.Player.LocalPlayer.Dimension, false, false);
				}

				// TODO_LAUNCH: Make this cancellable... holding wasd? or move control
				// TODO_LAUNCH: Should we have world hints? probably not unless non trivial, like hang
			}
		}
	}

	private void ResetEnteringVehicleFlag()
	{
		m_bIsEnteringVehicle = false;
	}

	private void OnRender()
	{
		UpdateEnterVehicleSystem();

		// fix for constnatly trying to turn engine on
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			if (RAGE.Elements.Player.LocalPlayer.Vehicle.GetIsEngineRunning())
			{
				RAGE.Elements.Player.LocalPlayer.SetConfigFlag(429, false);
			}
			else
			{
				RAGE.Elements.Player.LocalPlayer.SetConfigFlag(429, true);
			}
		}
	}

	private bool IsVehicleWithoutGTALocks(RAGE.Elements.Vehicle vehicle)
	{
		return (vehicle.GetClass() == (int)EVehicleClass.VehicleClass_Cycles || vehicle.GetClass() == (int)EVehicleClass.VehicleClass_Motorcycles || vehicle.GetClass() == (int)EVehicleClass.VehicleClass_Boats);
	}

	private void OnEnterVehicleReal(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		UpdateVehicleDrivableAndLocks(vehicle);

		RAGE.Elements.Player.LocalPlayer.SetHelmet(false); // disable helmet

		// cleanup timer
		if (m_tmrEnterVehicleTimeout != null)
		{
			ClientTimerPool.MarkTimerForDeletion(ref m_tmrEnterVehicleTimeout);
			m_tmrEnterVehicleTimeout = null;
		}

		ResetEnteringVehicleFlag();
		m_bIsDriver = (seatId == (int)EVehicleSeat.Driver);

		if (m_bIsDriver)
		{
			// Undriveable if engine is off (this stops auto-start engine code in GTA from executing)
			RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleEngineOn, vehicle.Handle, vehicle.GetIsEngineRunning(), true, false);
			RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleUndriveable, vehicle.Handle, !vehicle.GetIsEngineRunning());
		}

		EVehicleTransmissionType transmission = DataHelper.GetEntityData<EVehicleTransmissionType>(vehicle, EDataNames.VEHICLE_TRANSMISSION);
		if (transmission.Equals(EVehicleTransmissionType.Manual))
		{
			m_isManual = true;
			m_currentVehGear = DataHelper.GetEntityData<int>(vehicle, EDataNames.MANUAL_VEHICLE_GEAR);
		}
		else
		{
			// If the transmission type changed, we need to set the max speed again instead of capping it off.
			m_isManual = false;
			float maxSpeed = RAGE.Game.Vehicle.GetVehicleModelMaxSpeed(vehicle.Model);
			vehicle.SetMaxSpeed(maxSpeed);
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;
			UpdateLightsState(vehicle);
			UpdateSirensState(vehicle);
			UpdateVehicleDrivableAndLocks(vehicle);
			UpdateManualVehicleStates(vehicle);

			if (vehicle.GetClass() == (int)EVehicleClass.VehicleClass_Commercial
				|| vehicle.GetClass() == (int)EVehicleClass.VehicleClass_Service
				|| vehicle.GetClass() == (int)EVehicleClass.VehicleClass_Industrial)
			{
				RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleAudio, vehicle.Handle, "LANDSTALKER");
			}

			if (vehicle.Model == 0x885F3671
					|| vehicle.Model == 0x73920F8E
					|| vehicle.Model == 0xB822A1AA)
			{
				RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetVehicleAudio, vehicle.Handle, "FIB2");
			}


			ApplyVehicleDoorStates(vehicle);

			// RAGE_HACK: Fix for large vehicles having broken audio
			int vehClass = vehicle.GetClass();
			if (vehClass == (int)EVehicleClass.VehicleClass_Commercial
				|| vehClass == (int)EVehicleClass.VehicleClass_Service
				|| vehClass == (int)EVehicleClass.VehicleClass_Industrial)
			{
				// but not for taxi
				if (vehicle.Model != 3338918751 && vehicle.Model != 1884962369)
				{
					vehicle.SetAudio("STOCKADE");
				}
			}

		}
	}

	private void UpdateLightsState(RAGE.Elements.Vehicle vehicle)
	{
		EHeadlightState lightState = DataHelper.GetEntityData<EHeadlightState>(vehicle, EDataNames.HEADLIGHTS);
		UpdateLightsState_WithValue(vehicle, lightState);
	}

	private void UpdateManualVehicleStates(RAGE.Elements.Vehicle vehicle)
	{
		bool brakeLights = DataHelper.GetEntityData<bool>(vehicle, EDataNames.MANUAL_VEHICLE_BRAKELIGHTS);
		EVehicleTransmissionType bIsManual = DataHelper.GetEntityData<EVehicleTransmissionType>(vehicle, EDataNames.VEHICLE_TRANSMISSION);
		float currentRpm = DataHelper.GetEntityData<float>(vehicle, EDataNames.MANUAL_VEHICLE_RPM);

		if (bIsManual.Equals(EVehicleTransmissionType.Manual))
		{
			vehicle.SetBrakeLights(brakeLights);
			vehicle.Rpm = currentRpm;
		}
	}

	private void UpdateLightsState_WithValue(RAGE.Elements.Vehicle vehicle, EHeadlightState lightState)
	{
		if (lightState == EHeadlightState.Off)
		{
			vehicle.SetLights(1);
			vehicle.SetFullbeam(false);
		}
		else if (lightState == EHeadlightState.On)
		{
			vehicle.SetLights(2);
			vehicle.SetFullbeam(false);
		}
		else if (lightState == EHeadlightState.On_FullBeam)
		{
			vehicle.SetLights(2);
			vehicle.SetFullbeam(true);
		}
	}

	private void UpdateSirensState(RAGE.Elements.Vehicle vehicle)
	{
		bool bSilentSiren = DataHelper.GetEntityData<bool>(vehicle, EDataNames.SIREN_STATE);
		UpdateSirensState_WithValue(vehicle, bSilentSiren);
	}

	private void UpdateSirensState_WithValue(RAGE.Elements.Vehicle vehicle, bool bSilentSiren)
	{
		vehicle.SetSirenSound(bSilentSiren);
	}

	private void UpdateVehicleDoorState(RAGE.Elements.Vehicle vehicle, int doorIndex, bool bDoorOpen)
	{
		if (bDoorOpen)
		{
			vehicle.SetDoorOpen(doorIndex, false, false);
		}
		else
		{
			vehicle.SetDoorShut(doorIndex, false);
		}
	}

	private void UpdateVehicleWindowState(RAGE.Elements.Vehicle vehicle, bool bWindowOpen)
	{
		if (bWindowOpen)
		{
			vehicle.RollDownWindows();
		}
		else
		{
			vehicle.RollUpWindow(0);
			vehicle.RollUpWindow(1);
			vehicle.RollUpWindow(2);
			vehicle.RollUpWindow(3);
		}
	}

	private void ApplyVehicleDoorStates(RAGE.Elements.Vehicle vehicle)
	{
		UpdateVehicleDoorState(vehicle, 0, DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEH_DOOR_0));
		UpdateVehicleDoorState(vehicle, 1, DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEH_DOOR_1));
		UpdateVehicleDoorState(vehicle, 2, DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEH_DOOR_2));
		UpdateVehicleDoorState(vehicle, 3, DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEH_DOOR_3));
		UpdateVehicleDoorState(vehicle, 4, DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEH_DOOR_4));
		UpdateVehicleDoorState(vehicle, 5, DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEH_DOOR_5));
	}

	private void OnToggleWindows(EControlActionType actionType)
	{
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null && KeyBinds.CanProcessKeybinds())
		{
			NetworkEventSender.SendNetworkEvent_ToggleWindows();
		}
	}

	private void OnToggleHandbrake(EControlActionType actionType)
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		if (vehicle != null && KeyBinds.CanProcessKeybinds() && vehicle.GetPedInSeat(-1, 0).Equals(RAGE.Elements.Player.LocalPlayer.Handle))
		{
			NetworkEventSender.SendNetworkEvent_SyncVehicleHandbrake();
		}
	}

	private void OnPlayHandbrakeSound()
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		bool bHandbrakeOn = DataHelper.GetEntityData<bool>(vehicle, EDataNames.VEHICLE_HANDBRAKE);
		AudioManager.PlayAudio(bHandbrakeOn ? EAudioIDs.Handbrake_Up : EAudioIDs.Handbrake_Down, false, false);
	}

	private void OnToggleEngine(EControlActionType actionType)
	{
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null && KeyBinds.CanProcessKeybinds())
		{
			NetworkEventSender.SendNetworkEvent_ToggleEngine();
		}
	}

	private void OnToggleLock(EControlActionType actionType)
	{
		// NOTE: No in vehicle check as you remote unlock
		if (KeyBinds.CanProcessKeybinds())
		{
			NetworkEventSender.SendNetworkEvent_ToggleVehicleLocked();
		}
	}

	private void OnToggleHeadlights(EControlActionType actionType)
	{
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null && KeyBinds.CanProcessKeybinds())
		{
			NetworkEventSender.SendNetworkEvent_ToggleHeadlights();
		}
	}

	private void OnToggleSeatbelt()
	{
		RAGE.Elements.Player player = RAGE.Elements.Player.LocalPlayer;
		bool seatbeltOn = DataHelper.GetEntityData<bool>(player, EDataNames.SEATBELT);

		if (!seatbeltOn)
		{
			AudioManager.PlayAudio(EAudioIDs.Seatbelt_Buckle, false, false);
			player.SetConfigFlag(32, true); // ped flag - 32 - Cannot fly out of vehicle (seat belt)
		}
		else
		{
			AudioManager.PlayAudio(EAudioIDs.Seatbelt_Unbuckle, false, false);
			player.SetResetFlag(32, true); // R* being weird. We can't disable with the SetConfigFlag so we Reset it.
		}
	}

	private void OnTick()
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;

		if (!KeyBinds.IsChatInputVisible())
		{
			if (vehicle != null && vehicle.GetPedInSeat(-1, 0).Equals(RAGE.Elements.Player.LocalPlayer.Handle) && m_isManual)
			{
				UpdateManualTransmission();

				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.FrontendLeft);
				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.FrontendRight);
				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleMoveUpOnly);
				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleRadioWheel);

				if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleMoveUpOnly)) // Left Shift
				{
					ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleAccelerate);

					if (vehicle.Rpm > 0.1f && !RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.VehicleAccelerate) && vehicle.GetIsEngineRunning())
					{
						vehicle.Rpm -= 0.05f;
						NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
					}

					if (RAGE.Game.Pad.IsDisabledControlJustReleased(0, (int)RAGE.Game.Control.FrontendRight) && m_isManual) // Arrow right
					{
						SetVehicleGear(true);
					}

					if (RAGE.Game.Pad.IsDisabledControlJustReleased(0, (int)RAGE.Game.Control.FrontendLeft) && m_isManual) // Arrow left
					{
						SetVehicleGear(false);
					}

					if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleAccelerate) && vehicle.GetIsEngineRunning())
					{
						if (vehicle.Rpm != 1.0f)
						{
							vehicle.Rpm += 0.1f;
						}
						else if (vehicle.Rpm > 1.0f)
						{
							vehicle.Rpm = 1.0f;
						}
						NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);

					}
				}

				if (vehicle.GetIsEngineRunning())
				{
					if (m_currentVehGear == 0)
					{
						if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleAccelerate))
						{
							if (vehicle.Rpm != 1.0f)
							{
								vehicle.Rpm += 0.1f;
							}
							else if (vehicle.Rpm > 1.0f)
							{
								vehicle.Rpm = 1.0f;
							}
							NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
						}
						else
						{
							if (vehicle.Rpm < 0.1f)
							{
								vehicle.Rpm = 0.1f;
							}
							else
							{
								vehicle.Rpm -= 0.05f;
							}
							NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);

						}
					}
					else if (m_currentVehGear > 0)
					{
						if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleAccelerate))
						{
							if (vehicle.Rpm < 1.0f)
							{
								vehicle.Rpm += 0.01f;
							}
							else
							{
								vehicle.Rpm = 1.0f;
							}
							NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
						}
					}
					else if (m_currentVehGear == -1)
					{
						if (RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.VehicleBrake))
						{
							if (vehicle.Rpm < 1.0f)
							{
								vehicle.Rpm += 0.1f;
							}
							else
							{
								vehicle.Rpm = 1.0f;
							}
							NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
						}
					}

					if (!RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleAccelerate) && !RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleBrake) && !RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleMoveUpOnly))
					{
						if (vehicle.Rpm > 0.1f)
						{
							vehicle.Rpm -= 0.05f;
						}
						else
						{
							vehicle.Rpm = 0.1f;
						}
						NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
					}
				}
				else
				{
					vehicle.Rpm = 0.0f;
					NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
				}
			}
		}
	}

	private void UpdateManualTransmission()
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		if (vehicle != null && m_isManual)
		{
			float vehSpeed = vehicle.GetSpeed();
			float FirstMax = 12.0f;
			float SecondMax = 25.0f;
			float ThirdMax = 33.4f;
			float FourthMax = 42.0f;

			// Ensures realistic gear
			if (vehSpeed < 0.89f && m_currentVehGear > -1)
			{
				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleBrake);
			}
			else if (m_currentVehGear == -1 && vehSpeed < 0.89f)
			{
				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleAccelerate);
			}

			// Brake lights
			if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleBrake) || (m_currentVehGear == -1 && RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.VehicleAccelerate)))
			{
				vehicle.SetBrakeLights(true);
				NetworkEventSender.SendNetworkEvent_SyncManualVehBrakes(true);
			}
			else
			{
				vehicle.SetBrakeLights(false);
				NetworkEventSender.SendNetworkEvent_SyncManualVehBrakes(false);
			}

			// Gears switching cases
			switch (m_currentVehGear)
			{

				// Neutral
				case 0:
					{
						ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleAccelerate);
						ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleBrake);
					}
					break;

				// Gear 1
				case 1:
					{
						if (vehSpeed > FirstMax)
						{
							vehicle.SetMaxSpeed(vehSpeed);
						}
						else
						{
							vehicle.SetMaxSpeed(FirstMax);
						}
					}
					break;
				// Gear 2
				case 2:
					{
						if (vehSpeed > SecondMax)
						{
							vehicle.SetMaxSpeed(vehSpeed);
						}
						else
						{
							vehicle.SetMaxSpeed(SecondMax);
						}
					}
					break;
				// Gear 3
				case 3:
					{
						if (vehSpeed > ThirdMax)
						{
							vehicle.SetMaxSpeed(vehSpeed);
						}
						else
						{
							vehicle.SetMaxSpeed(ThirdMax);
						}
					}
					break;
				// Gear 4
				case 4:
					{
						if (vehSpeed > FourthMax)
						{
							vehicle.SetMaxSpeed(vehSpeed);
						}
						else
						{
							vehicle.SetMaxSpeed(FourthMax);
						}
					}
					break;
				// Gear 5
				case 5:
					{
						float maxSpeed = RAGE.Game.Vehicle.GetVehicleModelMaxSpeed(vehicle.Model);
						vehicle.SetMaxSpeed(maxSpeed);
					}
					break;
			}

			// Gear safety checking
			if (m_currentVehGear == 2 && vehSpeed <= FirstMax - 2.1)
			{
				if (RAGE.Game.Pad.IsControlPressed(27, (int)RAGE.Game.Control.VehicleAccelerate))
				{
					vehicle.SetMaxSpeed(FirstMax);
				}
			}
			else if (m_currentVehGear == 3 && vehSpeed <= (SecondMax / 2))
			{
				if (RAGE.Game.Pad.IsControlPressed(27, (int)RAGE.Game.Control.VehicleAccelerate))
				{
					NetworkEventSender.SendNetworkEvent_ToggleEngineStall();
					// Shifted too fast
				}
			}
			else if (m_currentVehGear == 4 && vehSpeed <= (ThirdMax / 2))
			{
				if (RAGE.Game.Pad.IsControlPressed(27, (int)RAGE.Game.Control.VehicleAccelerate))
				{
					NetworkEventSender.SendNetworkEvent_ToggleEngineStall();
					// Shifted too fast
				}
			}
			else if (m_currentVehGear == 5 && vehSpeed <= (FourthMax / 2))
			{
				if (RAGE.Game.Pad.IsControlPressed(27, (int)RAGE.Game.Control.VehicleAccelerate))
				{
					NetworkEventSender.SendNetworkEvent_ToggleEngineStall();
					// Shifted too fast
				}
			}

			if (m_currentVehGear != 1 && vehicle.IsStopped())
			{
				if (RAGE.Game.Pad.IsControlPressed(27, (int)RAGE.Game.Control.VehicleAccelerate))
				{
					NetworkEventSender.SendNetworkEvent_ToggleEngineStall();
				}
			}

		}
	}

	private void SetVehicleGear(bool bShiftUp)
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;

		if (vehicle != null)
		{
			if (bShiftUp)
			{
				if (m_currentVehGear < 5)
				{
					m_currentVehGear += 1;
					vehicle.Rpm = 0.2f;
					NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
				}
			}
			else
			{
				if (m_currentVehGear > 0)
				{
					m_currentVehGear -= 1;
					vehicle.Rpm = 0.2f;
					NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
				}
				else if (m_currentVehGear == 0 && vehicle.GetSpeed() == 0)
				{
					m_currentVehGear = -1;
					vehicle.Rpm = 0.2f;
					NetworkEventSender.SendNetworkEvent_SyncManualVehRpm(vehicle.Rpm);
				}
			}

			NetworkEventSender.SendNetworkEvent_SetVehicleGear(m_currentVehGear);
		}
	}

	private bool m_bIsDriver = false;

	// enter vehicle flags
	private bool m_bIsEnteringVehicle = false;
	private RAGE.Elements.Vehicle m_NearestVehicle = null;
	private RAGE.Elements.Vehicle m_vehicleBeingEntered = null;
	private EVehicleSeat m_NearestSeat = EVehicleSeat.Driver;

	private const float g_fEnterThreshold = 6.0f;
	private const float g_fEnterThreshold_Big = 9.0f;

	private WeakReference<ClientTimer> m_tmrEnterVehicleTimeout = null;
}