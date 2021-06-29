using System;

public static class VehicleHUD
{
	static VehicleHUD()
	{
		NetworkEvents.EnterVehicleReal += OnEnterVehicleReal;
		NetworkEvents.ExitVehicleReal += OnPlayerLeaveVehicle;

		NetworkEvents.ChangeCharacterApproved += OnClose;

		RageEvents.RAGE_OnRender += OnTick;
	}

	public static void Init()
	{

	}

	private static void OnTick()
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;

		if (vehicle != null)
		{
			float fOdometer = (float)Math.Ceiling(DataHelper.GetEntityData<float>(vehicle, EDataNames.ODOMETER));

			// TODO_SPEED: Helper func
			float fSpeedMps = vehicle.GetSpeed();
			float fSpeedMetersPerHour = (fSpeedMps * 3600.9f);
			float fSpeedMilesPerHour = (float)Math.Ceiling((fSpeedMetersPerHour / 1609.344f) * 1.25f);

			float fAccel = vehicle.GetSpeedVector(true).Y;
			var strGear = "";
			int gear = 0;
			bool bIsManual = DataHelper.GetEntityData<EVehicleTransmissionType>(vehicle, EDataNames.VEHICLE_TRANSMISSION).Equals(EVehicleTransmissionType.Manual);

			if (bIsManual)
			{
				gear = DataHelper.GetEntityData<int>(vehicle, EDataNames.MANUAL_VEHICLE_GEAR);
				if (gear > 0)
				{
					strGear = Helpers.FormatString("D{0}", gear);
				}
				else if (gear < 0)
				{
					strGear = "R";
				}
				else if (!vehicle.GetIsEngineRunning() || gear == 0)
				{
					strGear = "N";
				}
			}
			else
			{
				if (!vehicle.GetIsEngineRunning())
				{
					strGear = "P";
				}
				else if (fAccel > 1 && fSpeedMilesPerHour != 0)
				{
					gear = (int)(fSpeedMilesPerHour / 18.0f) + 1;
					strGear = Helpers.FormatString("D{0}", gear);
				}
				else if (fAccel < -1 && fSpeedMilesPerHour != 0)
				{
					strGear = "R";
				}
				else
				{
					strGear = "D1";
				}
			}

			float fFuelLevel = (float)Math.Ceiling(DataHelper.GetEntityData<float>(vehicle, EDataNames.FUEL));

			HUD.GetHudBrowser().Execute("UpdateVehicleHUD", fOdometer, fSpeedMilesPerHour, strGear, fFuelLevel, HUD.m_LastSpeedLimitMPH);
		}
	}

	public static void ClearHUD()
	{
		HUD.GetHudBrowser().Execute("UpdateVehicleHUD", 0.0f, 0.0f, "", 0.0f, HUD.m_LastSpeedLimitMPH);
	}

	private static void OnEnterVehicleReal(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		HUD.GetHudBrowser().Execute("SetVehicleHUDVisible", true);
	}

	private static void OnPlayerLeaveVehicle(RAGE.Elements.Vehicle vehicle)
	{
		ClearHUD();
		HUD.GetHudBrowser().Execute("SetVehicleHUDVisible", false);
	}

	private static void OnClose()
	{
		ClearHUD();
		HUD.GetHudBrowser().Execute("SetVehicleHUDVisible", false);
	}
}