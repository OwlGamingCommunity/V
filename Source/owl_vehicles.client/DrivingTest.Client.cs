using System;

public class DrivingTest
{
	private WeakReference<CWorldPed> m_refWorldPed_Bike_Paleto = new WeakReference<CWorldPed>(null);
	private WeakReference<CWorldPed> m_refWorldPed_Car_Paleto = new WeakReference<CWorldPed>(null);
	private WeakReference<CWorldPed> m_refWorldPed_Truck_Paleto = new WeakReference<CWorldPed>(null);
	private WeakReference<CWorldPed> m_refWorldPed_Bike_LS = new WeakReference<CWorldPed>(null);
	private WeakReference<CWorldPed> m_refWorldPed_Car_LS = new WeakReference<CWorldPed>(null);
	private WeakReference<CWorldPed> m_refWorldPed_Truck_LS = new WeakReference<CWorldPed>(null);
	private bool m_bIsInDrivingTest = false;
	private EDrivingTestState m_DrivingTestState = EDrivingTestState.Idle;
	private EDrivingTestType m_DrivingTestType = EDrivingTestType.None;
	private bool m_bPendingOperation = false;

	private RAGE.Elements.Blip m_BlipDrivingTest = null;
	private RAGE.Elements.Marker m_MarkerDrivingTest = null;
	private RAGE.Vector3 m_vecColShapePosition = null;

	private const int g_SubtextDisplayTime = 15000;

	enum EDrivingTestState
	{
		Idle,
		GetVehicle,
		GotoCheckpoint,
		ReturnToVehicle,
		ReturnVehicle,
		WaitingServerResponse,
	}
	// TODO_POST_LAUNCH: Have the server determine the dimension for DMV, Faction Creation and vehicle store. Changing DB ID breaks this.
	public DrivingTest()
	{
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;
		NetworkEvents.EnterVehicleReal += OnEnterVehicleReal;
		NetworkEvents.StartDrivingTest_Rejected += StartDrivingTest_Rejected;

		NetworkEvents.StartDrivingTest += StartDrivingTest;
		NetworkEvents.StopDrivingTest += StopDrivingTest;

		RAGE.Game.Entity.CreateModelHide(266.1027f, -348.6416f, 43.73014f, 10.0f, 242636620, true);
		RAGE.Game.Entity.CreateModelHide(285.7195f, -356.0675f, 44.14019f, 10.0f, 406416082, true);

		NetworkEvents.DrivingTest_GotoReturnVehicle += ReturnVehicle;
		NetworkEvents.DrivingTest_GotoNextCheckpoint += GotoNextCheckpoint;
		NetworkEvents.DrivingTest_GotoVehicleReturned += GotoVehicleReturned;

		m_refWorldPed_Bike_Paleto = WorldPedManager.CreatePed(EWorldPedType.DrivingTest, 1498487404, new RAGE.Vector3(254.2168f, 222.6659f, 106.2869f), 156.3986f, 1);
		m_refWorldPed_Bike_Paleto.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Start Motorbike Driving Test", null, () => { OnInteract_Bike(EScriptLocation.Paleto); }, false, false, 4.0f, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true);

		m_refWorldPed_Car_Paleto = WorldPedManager.CreatePed(EWorldPedType.DrivingTest, 1498487404, new RAGE.Vector3(248.5044f, 224.4868f, 106.2871f), 165.0929f, 1);
		m_refWorldPed_Car_Paleto.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Start Car Driving Test", null, () => { OnInteract_Car(EScriptLocation.Paleto); }, false, false, 4.0f, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true);

		m_refWorldPed_Truck_Paleto = WorldPedManager.CreatePed(EWorldPedType.DrivingTest, 1498487404, new RAGE.Vector3(243.5189f, 226.3305f, 106.2876f), 163.9986f, 1);
		m_refWorldPed_Truck_Paleto.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Start Heavy Vehicle Driving Test", null, () => { OnInteract_Truck(EScriptLocation.Paleto); }, false, false, 4.0f, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true);

		m_refWorldPed_Bike_LS = WorldPedManager.CreatePed(EWorldPedType.DrivingTest, 1498487404, new RAGE.Vector3(254.2168f, 222.6659f, 106.2869f), 156.3986f, 1223);
		m_refWorldPed_Bike_LS.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Start Motorbike Driving Test", null, () => { OnInteract_Bike(EScriptLocation.LS); }, false, false, 4.0f, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true);

		m_refWorldPed_Car_LS = WorldPedManager.CreatePed(EWorldPedType.DrivingTest, 1498487404, new RAGE.Vector3(248.5044f, 224.4868f, 106.2871f), 165.0929f, 1223);
		m_refWorldPed_Car_LS.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Start Car Driving Test", null, () => { OnInteract_Car(EScriptLocation.LS); }, false, false, 4.0f, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true);

		m_refWorldPed_Truck_LS = WorldPedManager.CreatePed(EWorldPedType.DrivingTest, 1498487404, new RAGE.Vector3(243.5189f, 226.3305f, 106.2876f), 163.9986f, 1223);
		m_refWorldPed_Truck_LS.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Start Heavy Vehicle Driving Test", null, () => { OnInteract_Truck(EScriptLocation.LS); }, false, false, 4.0f, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true);
	}

	private void CleanupAll()
	{
		if (m_BlipDrivingTest != null)
		{
			m_BlipDrivingTest.Destroy();
		}

		if (m_MarkerDrivingTest != null)
		{
			m_MarkerDrivingTest.Destroy();
		}

		m_BlipDrivingTest = null;
		m_MarkerDrivingTest = null;
		m_vecColShapePosition = null;
	}

	private void OnEnterVehicleReal(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		if (m_DrivingTestState == EDrivingTestState.GetVehicle)
		{
			if (seatId == (int)EVehicleSeat.Driver) // driver
			{
				EVehicleType vehicleType = DataHelper.GetEntityData<EVehicleType>(vehicle, EDataNames.VEHICLE_TYPE);

				if ((m_DrivingTestType == EDrivingTestType.Bike && vehicleType == EVehicleType.DrivingTest_Bike) // dmv bike
					|| (m_DrivingTestType == EDrivingTestType.Car && vehicleType == EVehicleType.DrivingTest_Car) // dmv car
					|| (m_DrivingTestType == EDrivingTestType.Truck && vehicleType == EVehicleType.DrivingTest_Truck) // dmv truck
				)
				{
					GotoCheckpointState();
				}
			}
		}
	}

	private float GetRadiusToUse()
	{
		if (m_DrivingTestType == EDrivingTestType.Bike)
		{
			return DrivingTestConstants.g_ColshapeRadius_Small;
		}
		else if (m_DrivingTestType == EDrivingTestType.Car)
		{
			return DrivingTestConstants.g_ColshapeRadius_Medium;
		}
		else if (m_DrivingTestType == EDrivingTestType.Truck)
		{
			return DrivingTestConstants.g_ColshapeRadius_Large;
		}

		return DrivingTestConstants.g_ColshapeRadius_Small;
	}

	private void GotoCheckpointState()
	{
		if (!m_bPendingOperation)
		{
			NetworkEventSender.SendNetworkEvent_DrivingTest_GotoCheckpointState();
			SetPendingServerResponse();
		}
	}

	private void GetNextCheckpoint()
	{
		if (!m_bPendingOperation)
		{
			bool bVisualDamage = RAGE.Elements.Player.LocalPlayer.Vehicle != null ? RAGE.Elements.Player.LocalPlayer.Vehicle.IsDamaged() : false;
			SetPendingServerResponse();
			NetworkEventSender.SendNetworkEvent_DrivingTest_GetNextCheckpoint(bVisualDamage);
		}
	}

	private void SetPendingServerResponse()
	{
		m_DrivingTestState = EDrivingTestState.WaitingServerResponse;
		m_bPendingOperation = true;
	}

	private void InteractWithTest(EDrivingTestType a_TestType, EScriptLocation location)
	{
		if (a_TestType != EDrivingTestType.None && m_DrivingTestState == EDrivingTestState.Idle && !m_bPendingOperation)
		{
			m_DrivingTestType = a_TestType;
			SetPendingServerResponse();
			NetworkEventSender.SendNetworkEvent_AttemptStartDrivingTest(a_TestType, location);
		}
		else if (a_TestType != EDrivingTestType.None && m_DrivingTestType != EDrivingTestType.None && !m_bPendingOperation)
		{
			m_DrivingTestType = EDrivingTestType.None;
			SetPendingServerResponse();
			NetworkEventSender.SendNetworkEvent_AttemptEndDrivingTest(m_DrivingTestType);
		}
	}

	private void OnInteract_Bike(EScriptLocation location)
	{
		InteractWithTest(EDrivingTestType.Bike, location);
	}

	private void OnInteract_Car(EScriptLocation location)
	{
		InteractWithTest(EDrivingTestType.Car, location);
	}

	private void OnInteract_Truck(EScriptLocation location)
	{
		InteractWithTest(EDrivingTestType.Truck, location);
	}

	private void OnTick()
	{
		// Update world hints
		// TODO_POST_LAUNCH Sync inventory to client?
		bool bHasBikeLicense = false;
		bool bHasCarLicense = false;
		bool bHasTruckLicense = false;

		// BIKE
		if (bHasBikeLicense)
		{
			m_refWorldPed_Bike_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.DummyNone, "You already have a Motorbike License");
			m_refWorldPed_Bike_LS.Instance()?.UpdateWorldHint(EScriptControlID.DummyNone, "You already have a Motorbike License");
		}
		else if (!m_bIsInDrivingTest)
		{
			m_refWorldPed_Bike_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Start Driving Test (Motorbike) - $50");
			m_refWorldPed_Bike_LS.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Start Driving Test (Motorbike) - $50");
		}
		else if (m_bIsInDrivingTest)
		{
			m_refWorldPed_Bike_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Cancel Driving Test");
			m_refWorldPed_Bike_LS.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Cancel Driving Test");
		}

		// CAR
		if (bHasCarLicense)
		{
			m_refWorldPed_Car_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.DummyNone, "You already have a Car License");
			m_refWorldPed_Car_LS.Instance()?.UpdateWorldHint(EScriptControlID.DummyNone, "You already have a Car License");
		}
		else if (!m_bIsInDrivingTest)
		{
			m_refWorldPed_Car_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Start Driving Test (Car) - $75");
			m_refWorldPed_Car_LS.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Start Driving Test (Car) - $75");
		}
		else if (m_bIsInDrivingTest)
		{
			m_refWorldPed_Car_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Cancel Driving Test");
			m_refWorldPed_Car_LS.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Cancel Driving Test");
		}

		// TRUCK
		if (bHasTruckLicense)
		{
			m_refWorldPed_Truck_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.DummyNone, "You already have a Heavy Vehicle License");
			m_refWorldPed_Truck_LS.Instance()?.UpdateWorldHint(EScriptControlID.DummyNone, "You already have a Heavy Vehicle License");
		}
		else if (!m_bIsInDrivingTest)
		{
			m_refWorldPed_Truck_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Start Driving Test (Heavy Vehicle) - $100");
			m_refWorldPed_Truck_LS.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Start Driving Test (Heavy Vehicle) - $100");
		}
		else if (m_bIsInDrivingTest)
		{
			m_refWorldPed_Truck_Paleto.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Cancel Driving Test");
			m_refWorldPed_Truck_LS.Instance()?.UpdateWorldHint(EScriptControlID.Interact, "Cancel Driving Test");
		}

		// Update gameplay
		if (m_DrivingTestState == EDrivingTestState.GotoCheckpoint)
		{
			if (m_vecColShapePosition != null && RAGE.Elements.Player.LocalPlayer.Vehicle != null)
			{
				float fDistance = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Vehicle.Position, m_vecColShapePosition);

				if (!m_bPendingOperation && fDistance <= GetRadiusToUse())
				{
					GetNextCheckpoint();
				}
			}
		}
		else if (m_DrivingTestState == EDrivingTestState.ReturnVehicle)
		{
			if (m_vecColShapePosition != null && RAGE.Elements.Player.LocalPlayer.Vehicle != null)
			{
				float fDistance = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Vehicle.Position, m_vecColShapePosition);

				if (!m_bPendingOperation && fDistance <= GetRadiusToUse())
				{
					bool bVisualDamage = RAGE.Elements.Player.LocalPlayer.Vehicle != null ? RAGE.Elements.Player.LocalPlayer.Vehicle.IsDamaged() : false;
					SetPendingServerResponse();
					NetworkEventSender.SendNetworkEvent_DrivingTest_ReturnVehicle(bVisualDamage);
				}
			}
		}
	}

	private void StartDrivingTest_Rejected()
	{
		StopDrivingTest();
	}

	private void StartDrivingTest(EDrivingTestType testType, bool a_bIsResume)
	{
		m_DrivingTestType = testType;
		m_bPendingOperation = false;
		m_bIsInDrivingTest = true;

		CleanupAll();
		m_DrivingTestState = EDrivingTestState.GetVehicle;

		if (!a_bIsResume)
		{
			string strTestName = String.Empty;

			if (m_DrivingTestType == EDrivingTestType.Bike)
			{
				strTestName = "Bike";
			}
			else if (m_DrivingTestType == EDrivingTestType.Car)
			{
				strTestName = "Car";
			}
			else if (m_DrivingTestType == EDrivingTestType.Truck)
			{
				strTestName = "Heavy";
			}

			ShardManager.ShowShard(Helpers.FormatString("{0} Driving Test Started!", strTestName), "Complete the course without breaking any traffic laws or damaging the vehicle.", "");
		}

		// TODO_POST_LAUNCH: Put a blip on the vehicle
		HUD.ShowSubtitle("Proceed to the DMV vehicles.", g_SubtextDisplayTime);
	}

	private void StopDrivingTest()
	{
		m_bIsInDrivingTest = false;
		m_bPendingOperation = false;

		CleanupAll();
		m_DrivingTestState = EDrivingTestState.Idle;
	}

	private void ReturnVehicle(bool bSuccess, float x, float y, float z)
	{
		m_bPendingOperation = false;

		m_DrivingTestState = EDrivingTestState.ReturnVehicle;

		if (bSuccess)
		{
			CleanupAll();

			// Create a blip
			RAGE.Vector3 vecTarget = new RAGE.Vector3(x, y, z);
			vecTarget.Z = WorldHelper.GetGroundPosition(vecTarget);
			m_vecColShapePosition = vecTarget;
			CreateDrivingTestBlip(m_vecColShapePosition, "Driving Test Checkpoint", 315);

			// Marker
			float fRadiusToUse = GetRadiusToUse();
			m_MarkerDrivingTest = new RAGE.Elements.Marker(1, m_vecColShapePosition, fRadiusToUse, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(fRadiusToUse, fRadiusToUse, 1.0f), new RAGE.RGBA(255, 194, 15, 200));

			HUD.ShowSubtitle("Return the vehicle to the DMV.", g_SubtextDisplayTime);
		}
	}

	private void CreateDrivingTestBlip(RAGE.Vector3 vecPos, string strText, uint sprite)
	{
		m_BlipDrivingTest = new RAGE.Elements.Blip(sprite, vecPos, strText, shortRange: true);
		m_BlipDrivingTest.SetRoute(true);
		m_BlipDrivingTest.SetRouteColour(9);
	}

	private void GotoNextCheckpoint(bool bSuccess, RAGE.Vector3 vecTarget)
	{
		m_bPendingOperation = false;

		m_DrivingTestState = EDrivingTestState.GotoCheckpoint;

		if (bSuccess)
		{
			CleanupAll();

			// Create a blip
			vecTarget.Z = WorldHelper.GetGroundPosition(vecTarget);
			m_vecColShapePosition = vecTarget;
			CreateDrivingTestBlip(m_vecColShapePosition, "Driving Test Checkpoint", 315);

			// Marker
			float fRadiusToUse = GetRadiusToUse();
			m_MarkerDrivingTest = new RAGE.Elements.Marker(1, m_vecColShapePosition, fRadiusToUse, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(fRadiusToUse, fRadiusToUse, 1.0f), new RAGE.RGBA(255, 194, 15, 200));

			HUD.ShowSubtitle("Drive to the checkpoint, obeying the law.", g_SubtextDisplayTime);
		}
	}

	private void GotoVehicleReturned(bool bSuccess, bool bPassed, bool bFailedSpeeding, bool bFailedDamage)
	{
		m_bPendingOperation = false;

		if (bSuccess)
		{
			CleanupAll();

			string strTestName = String.Empty;

			if (m_DrivingTestType == EDrivingTestType.Bike)
			{
				strTestName = "Bike";
			}
			else if (m_DrivingTestType == EDrivingTestType.Car)
			{
				strTestName = "Car";
			}
			else if (m_DrivingTestType == EDrivingTestType.Truck)
			{
				strTestName = "Heavy";
			}

			if (bPassed)
			{
				ShardManager.ShowShard(Helpers.FormatString("{0} Driving Test Complete!", strTestName), Helpers.FormatString("You have passed the {0} Driving Test.", strTestName), "Congratulations!");
			}
			else
			{
				string strMessage = String.Empty;
				if (bFailedSpeeding)
				{
					strMessage = "Dangerous Driving: Speeding";
				}
				else if (bFailedDamage)
				{
					strMessage = "Dangerous Driving: Vehicular Damage";
				}
				else
				{
					strMessage = "Dangerous Driving: Speeding & Vehicular Damage";
				}

				ShardManager.ShowShard(Helpers.FormatString("{0} Driving Test Failed!", strTestName), "You have failed due to:", strMessage);
			}

			StopDrivingTest();
		}
		else
		{
			m_DrivingTestState = EDrivingTestState.ReturnVehicle;
		}
	}
}