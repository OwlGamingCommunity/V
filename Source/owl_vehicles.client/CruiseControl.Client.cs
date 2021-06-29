using RAGE.Elements;
using System;

public class CruiseControl
{
	//private Vehicle playerVehicle;
	private float cruiseSpeed;
	private float maxVehSpeed;
	private bool isCruising = false;
	private bool isDriver = false;

	private readonly int MINIMUM_CC_SPEED = 5;
	private readonly int INPUT_CONTROL_GROUP = 0; // PC only.
	private readonly int INPUT_VEH_ACCELERATE = 71;
	private readonly int INPUT_VEH_BRAKE = 72;
	private readonly int INPUT_VEH_HANDBRAKE = 76;

	public CruiseControl()
	{
		RageEvents.RAGE_OnTick_PerFrame += Tick;
		NetworkEvents.EnterVehicleReal += OnPlayerEnterVehicle;
		NetworkEvents.ExitVehicleReal += OnPlayerLeaveVehicle;

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleCruiseControl, OnToggleCruiseControl);
	}

	private void OnToggleCruiseControl(EControlActionType actionType)
	{
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null && isDriver)
		{
			// Make sure we have atleast a bit of speed, so it can be maintained.
			if (RAGE.Elements.Player.LocalPlayer.Vehicle.GetSpeed() > MINIMUM_CC_SPEED && !isCruising)
			{
				EnableCruiseControl();
			}
			else if (isCruising)
			{
				DisableCruiseControl(RAGE.Elements.Player.LocalPlayer.Vehicle);
			}
		}
	}

	private void Tick()
	{
		ChecksForCruiseControl();
	}

	private bool DoesVehicleHaveCruiseControl(Vehicle vehicle)
	{
		EVehicleClass vehicleClass = (EVehicleClass)vehicle.GetClass();

		return (vehicleClass != EVehicleClass.VehicleClass_Motorcycles
			&& vehicleClass != EVehicleClass.VehicleClass_Cycles
			&& vehicleClass != EVehicleClass.VehicleClass_Boats
			&& vehicleClass != EVehicleClass.VehicleClass_Helicopters
			&& vehicleClass != EVehicleClass.VehicleClass_Planes
			&& vehicleClass != EVehicleClass.VehicleClass_Trains);
	}

	private void OnPlayerEnterVehicle(Vehicle vehicle, int seatId)
	{
		if (seatId == (int)EVehicleSeat.Driver && DoesVehicleHaveCruiseControl(vehicle))
		{
			maxVehSpeed = RAGE.Game.Vehicle.GetVehicleModelMaxSpeed(vehicle.Model);
		}

		isDriver = (seatId == (int)EVehicleSeat.Driver);
	}

	private void OnPlayerLeaveVehicle(Vehicle vehicle)
	{
		DisableCruiseControl(vehicle);
		cruiseSpeed = 0.0f;
		maxVehSpeed = 0.0f;
		isCruising = false;
		isDriver = false;
	}

	/*
        * Checks while having cruise control enabled, this is mainly to manage all the user's inputs that take place while driving.
    */
	private void ChecksForCruiseControl()
	{
		if (isCruising && RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			// Make sure that we manage the forward speed while he's driving the car in cruise control.
			HandleCruiseSpeed();

			float fFuelLevel = (float)Math.Ceiling(DataHelper.GetEntityData<float>(RAGE.Elements.Player.LocalPlayer.Vehicle, EDataNames.FUEL));

			// See if he had any crashes so far or pressed the brakes in order to disable cruise control.
			if (CrashChecks() || KeyControls() || fFuelLevel <= 0.0f)
			{
				DisableCruiseControl(RAGE.Elements.Player.LocalPlayer.Vehicle);
			}

			// See if he pressed the accelleration button 'W', disable if so
			if (RAGE.Game.Pad.IsControlJustPressed(INPUT_CONTROL_GROUP, INPUT_VEH_ACCELERATE))
			{
				DisableCruiseControl(RAGE.Elements.Player.LocalPlayer.Vehicle);
			}
		}
		else
		{
			isCruising = false;
		}
	}

	/*
         * Enables the Cruise Control. We set the current speed as cruise speed and we set his max speed to it, so he can't go over it.
        */
	private void EnableCruiseControl()
	{
		var playerVehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		if (DoesVehicleHaveCruiseControl(playerVehicle))
		{
			if (!playerVehicle.IsInAir())
			{
				float fFuelLevel = (float)Math.Ceiling(DataHelper.GetEntityData<float>(playerVehicle, EDataNames.FUEL));

				if (fFuelLevel > 0.0f)
				{
					cruiseSpeed = playerVehicle.GetSpeed();
					playerVehicle.SetMaxSpeed(cruiseSpeed);
					DrawCruiseControlMsg("Your cruise control is now enabled!<br><br>You can press the control again, brake or accelerate to disable cruise control.");
					isCruising = true;
				}
				else
				{
					DrawCruiseControlMsg("You cannot enable cruise control due to lack of fuel.");
				}
			}
			else
			{
				DrawCruiseControlMsg("You cannot enable cruise control at this time.");
			}
		}
		else
		{
			DrawCruiseControlMsg("This vehicle does not support cruise control.");
		}
	}

	/*
	 * Disables the Cruise Control. We reset the vehicle's max speed to it's original speed.
	*/
	private void DisableCruiseControl(RAGE.Elements.Vehicle vehicle)
	{
		if (isCruising)
		{
			if (maxVehSpeed > 0)
			{
				vehicle.SetMaxSpeed(maxVehSpeed);
			}

			DrawCruiseControlMsg("Your cruise control is now disabled!");
			isCruising = false;
		}
	}

	/*
	 * Manages the speed of the user, to make sure he keeps going forward.
	*/
	private void HandleCruiseSpeed()
	{
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			RAGE.Elements.Player.LocalPlayer.Vehicle.SetForwardSpeed(cruiseSpeed);
		}
	}

	/*
	 * Checks if the vehicle was braking, done by either the spacebar or the 'S' key.
	 * @return bool It'll return either a true when the car had a crash, gone flying or landed in the water. It'll return false if not.
	*/
	private bool CrashChecks()
	{
		var playerVehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		return playerVehicle.HasCollidedWithAnything() || playerVehicle.IsInAir() || playerVehicle.IsInWater() ? true : false;
	}

	/*
	 * Checks if the vehicle was braking, done by either the spacebar or the 'S' key.
	 * @return bool It'll return either a true when either GTA controls were used or a false if not.
	*/
	private bool KeyControls()
	{
		return RAGE.Game.Pad.IsControlJustPressed(INPUT_CONTROL_GROUP, INPUT_VEH_BRAKE) || RAGE.Game.Pad.IsControlJustPressed(INPUT_CONTROL_GROUP, INPUT_VEH_HANDBRAKE);
	}

	/*
	 * Draws a notification above the minimap for the user to know if he has CC enabled or not.
	*/
	private void DrawCruiseControlMsg(string description)
	{
		NotificationManager.ShowNotification("Cruise Control", description, ENotificationIcon.InfoSign);
	}
}