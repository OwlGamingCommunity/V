using System;

public class PDVehicleHUD
{
	private CGUIPDVehicleHUD m_PDVehicleHUDUI = new CGUIPDVehicleHUD(() => { });

	private RAGE.Elements.Vehicle m_CachedVehicleToUseForRequests = null;

	private RAGE.Elements.Marker m_SpeedTrapMarker = null;
	private float m_fAngularOffset = 0.0f;
	private float m_fForwardOffset = 10.0f;

	private const float g_fANPRDist = 15.0f;
	private const float g_fSpeedTrapRadius = 12.0f;
	private const float g_fOffsetMax = 55.0f;
	private const float g_fOffsetMaxForward = 45.0f;
	private const float g_fOffsetStep = 0.1f;

	public PDVehicleHUD()
	{
		RageEvents.RAGE_OnTick_PerFrame += OnTick;
		RageEvents.RAGE_OnTick_LowFrequency += OnTick_LowFrequency;
		RageEvents.RAGE_OnTick_OncePerSecond += OnTick_OncePerSecond;
		NetworkEvents.ExitVehicleReal += OnLeaveVehicle;

		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ChangeCharacterApproved += ClosePDVehicleHUD;

		NetworkEvents.ANPR_GotSpeed += OnGotSpeed;

		RageEvents.AddDataHandler(EDataNames.UNIT_NUMBER, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateUnitNumber(); });
		RageEvents.AddDataHandler(EDataNames.SIREN_STATE, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { UpdateSirensState(); });
	}

	~PDVehicleHUD()
	{

	}

	private void UpdateUnitNumber()
	{
		int unitNumber = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.UNIT_NUMBER);
		m_PDVehicleHUDUI.SetUnit(unitNumber);
	}

	private void UpdateSirensState()
	{
		bool bSilentSiren = DataHelper.GetEntityData<bool>(RAGE.Elements.Player.LocalPlayer.Vehicle, EDataNames.SIREN_STATE);
		m_PDVehicleHUDUI.SetSilentSiren(bSilentSiren);
	}

	private void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		bool bIsPoliceVehicle = DataHelper.GetEntityData<bool>(vehicle, EDataNames.IS_POLICE_VEHICLE);
		bool bCanSeeDevice = (seatId == (int)EVehicleSeat.Driver || seatId == (int)EVehicleSeat.FrontPassenger);

		// Don't let rear seat passengers see the device, only front passenger + driver
		if (bIsPoliceVehicle && bCanSeeDevice)
		{
			if (vehicle.GetClass() != (int)EVehicleClass.VehicleClass_Helicopters)
			{
				m_PDVehicleHUDUI.SetVisible(true, false, false);

				m_PDVehicleHUDUI.SetDefaults();

				UpdateUnitNumber();
			}
		}
	}

	private void OnLeaveVehicle(RAGE.Elements.Vehicle vehicle)
	{
		ClosePDVehicleHUD();
	}

	public void OnRunPlate()
	{
		if (m_CachedVehicleToUseForRequests != null)
		{
			NetworkEventSender.SendNetworkEvent_RequestPlateRun(m_CachedVehicleToUseForRequests);
		}
	}

	public void OnToggleSpeedTrap()
	{
		if (m_SpeedTrapMarker != null)
		{
			DisableSpeedTrap();
		}
		else
		{
			if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
			{
				RAGE.Vector3 vecSpeedTrapPos = CalculateSpeedTrapPos();
				m_SpeedTrapMarker = new RAGE.Elements.Marker(25, vecSpeedTrapPos, g_fSpeedTrapRadius, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.RGBA(255, 194, 15, 200), false, RAGE.Elements.Player.LocalPlayer.Dimension);
			}
		}
	}

	public void OnToggleSilentSiren()
	{
		NetworkEventSender.SendNetworkEvent_ToggleSirenMode();
	}

	private RAGE.Vector3 CalculateSpeedTrapPos()
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;

		RAGE.Vector3 vecRot = vehicle.GetRotation(0);
		RAGE.Vector3 vecSpeedTrapPos = vehicle.Position.CopyVector();
		float radians = (vecRot.Z + 90.0f + m_fAngularOffset) * (3.14f / 180.0f);
		vecSpeedTrapPos.X += (float)Math.Cos(radians) * m_fForwardOffset;
		vecSpeedTrapPos.Y += (float)Math.Sin(radians) * m_fForwardOffset;
		vecSpeedTrapPos.Z = WorldHelper.GetGroundPosition(vecSpeedTrapPos) + 0.1f;
		return vecSpeedTrapPos;
	}

	private void DisableSpeedTrap()
	{
		if (m_SpeedTrapMarker != null)
		{
			m_SpeedTrapMarker.Destroy();
			m_SpeedTrapMarker = null;
		}
	}

	private void ClosePDVehicleHUD()
	{
		DisableSpeedTrap();
		m_PDVehicleHUDUI.SetVisible(false, false, false);
	}

	RAGE.Elements.Vehicle GetPDHudVehicleTarget()
	{
		RAGE.Elements.Vehicle closestVeh = null;
		float fSmallestDist = 999999.0f;
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;

		if (m_SpeedTrapMarker == null) // automatic running ANPR
		{
			RAGE.Vector3 vecRot = vehicle.GetRotation(0);
			RAGE.Vector3 vecAngledEndpointPos = vehicle.Position.CopyVector();
			float radians = (vecRot.Z + 90.0f) * (3.14f / 180.0f);
			vecAngledEndpointPos.X += (float)Math.Cos(radians) * g_fANPRDist;
			vecAngledEndpointPos.Y += (float)Math.Sin(radians) * g_fANPRDist;
			vecAngledEndpointPos.Z = WorldHelper.GetGroundPosition(vecAngledEndpointPos);

			foreach (RAGE.Elements.Vehicle otherVeh in OptimizationCachePool.StreamedInVehicles())
			{
				if (otherVeh != vehicle)
				{
					float fDist = WorldHelper.GetDistance(vehicle.Position, otherVeh.Position);

					if (fDist < g_fANPRDist)
					{
						bool bIsInCone = otherVeh.IsInAngledArea(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, vecAngledEndpointPos.X, vecAngledEndpointPos.Y, vecAngledEndpointPos.Z, 20.0f, false, false, 0);
						if (bIsInCone)
						{
							if (fDist <= fSmallestDist)
							{
								fSmallestDist = fDist;
								closestVeh = otherVeh;
							}
						}
					}
				}
			}
		}
		else // speed trap positional mode
		{
			foreach (RAGE.Elements.Vehicle otherVeh in OptimizationCachePool.StreamedInVehicles())
			{
				if (otherVeh != vehicle)
				{
					float fDist = WorldHelper.GetDistance(m_SpeedTrapMarker.Position, otherVeh.Position);

					if (fDist < g_fSpeedTrapRadius / 2.0f)
					{
						if (fDist <= fSmallestDist)
						{
							fSmallestDist = fDist;
							closestVeh = otherVeh;
						}
					}
				}
			}
		}

		return closestVeh;
	}

	private const float g_fOffsetMinForward = 3.5f;

	private void UpdateSpeedTrap()
	{
		// TODO_CONTROLS: Make this a control?
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			if (KeyBinds.IsConsoleKeyDown(ConsoleKey.NumPad4))
			{
				m_fAngularOffset = Math.Clamp(m_fAngularOffset + g_fOffsetStep * 10, -g_fOffsetMax, g_fOffsetMax);
			}

			if (KeyBinds.IsConsoleKeyDown(ConsoleKey.NumPad6))
			{
				m_fAngularOffset = Math.Clamp(m_fAngularOffset - g_fOffsetStep * 10, -g_fOffsetMax, g_fOffsetMax);
			}

			if (KeyBinds.IsConsoleKeyDown(ConsoleKey.NumPad8))
			{
				m_fForwardOffset = Math.Clamp(m_fForwardOffset + g_fOffsetStep, g_fOffsetMinForward, g_fOffsetMaxForward);
			}

			if (KeyBinds.IsConsoleKeyDown(ConsoleKey.NumPad2))
			{
				m_fForwardOffset = Math.Clamp(m_fForwardOffset - g_fOffsetStep, g_fOffsetMinForward, g_fOffsetMaxForward);
			}

			RAGE.Vector3 vecPos = CalculateSpeedTrapPos();
			m_SpeedTrapMarker.Position = vecPos;
			m_SpeedTrapMarker.Visible = true; // TODO_RAGE_HACK: Fix for one frame marker pos being wrong. We set invisible on creation then update here
		}
	}

	private void OnTick()
	{
		if (m_SpeedTrapMarker != null)
		{
			UpdateSpeedTrap();
		}
	}

	DateTime m_TimeLastSpeedRequest = DateTime.Now;
	const int m_timeBetweenSpeedRequests = 10000; // unless vehicle changes
	RAGE.Elements.Vehicle cachedVeh = null;

	private void OnTick_OncePerSecond()
	{
		if (m_PDVehicleHUDUI.IsVisible())
		{
			m_PDVehicleHUDUI.SetSpeedLimit(HUD.m_LastSpeedLimitMPH);
		}
	}

	private void OnTick_LowFrequency()
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;

		if (m_PDVehicleHUDUI.IsVisible() && vehicle != null)
		{
			RAGE.Elements.Vehicle targetVeh = GetPDHudVehicleTarget();

			// update UI
			if (targetVeh != null)
			{
				m_CachedVehicleToUseForRequests = targetVeh;

				// Can we request the speed?
				double timeSinceLastProc = (DateTime.Now - m_TimeLastSpeedRequest).TotalMilliseconds;
				if ((targetVeh != null && timeSinceLastProc >= m_timeBetweenSpeedRequests) || targetVeh != cachedVeh)
				{
					m_TimeLastSpeedRequest = DateTime.Now;
					NetworkEventSender.SendNetworkEvent_ANPR_GetSpeed(targetVeh);
				}
			}

			cachedVeh = targetVeh;
		}
	}

	private void OnGotSpeed(float fSpeedMps)
	{
		if (cachedVeh != null)
		{
			float fSpeedMetersPerHour = (fSpeedMps * 3600.9f);
			float fSpeedMilesPerHour = (float)Math.Ceiling((fSpeedMetersPerHour / 1609.344f) * 1.25f);

			string strPlateText = cachedVeh.GetNumberPlateText();

			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(cachedVeh.Model);
			if (vehicleDef != null)
			{
				string strVehicleString = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
				m_PDVehicleHUDUI.SetSpeed(fSpeedMilesPerHour);
				m_PDVehicleHUDUI.SetVehicleInfo(strVehicleString, strPlateText);
			}
		}
	}
}

