// TODO: Would be cool if the UI showed for the customer too

class TaxiDriverJobInstance : BaseJob
{
	enum ETaxiJob_State
	{
		GetVehicle,
		Idle,
		GotoPickup,
		DriveToLocation
	}

	public TaxiDriverJobInstance() : base(EJobID.TaxiDriverJob, "Taxi Driver", "Proceed to a Taxi", EVehicleType.TaxiJob, EWorldPedType.TaxiDriverJob, 411102470, new RAGE.Vector3(-70.33584f, 6569.752f, 31.51085f), 207.47f, 0,
		new RAGE.Vector3(894.9326f, -179.1359f, 74.70026f), 235.5179f, 0, 225)
	{
		NetworkEvents.TaxiAccepted += OnTaxiAccepted;
		NetworkEvents.TaxiCleanup += CleanupAll;
		NetworkEvents.ChangeCharacterApproved += OnChangeCharacter;
		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ExitVehicleReal += OnExitVehicle;

		UIEvents.ChangeFarePerMile += UI_OnChangeFarePerMile;
		UIEvents.ToggleAvailableForHire += UI_OnToggleAvailableForHire;
		UIEvents.ResetFare += UI_OnResetFare;

		UIEvents.OnChangeFarePerMile_Submit += OnChangeFarePerMile_Submit;
		UIEvents.OnChangeFarePerMile_Cancel += OnChangeFarePerMile_Cancel;

		RageEvents.RAGE_OnEntityStreamIn += OnEntityStreamIn;
		RageEvents.RAGE_OnRender += OnRender;

		RageEvents.AddDataHandler(EDataNames.TAXI_AFH, UpdateTaxiUIAndState);
		RageEvents.AddDataHandler(EDataNames.TAXI_CPM, UpdateTaxiUIAndState);
		RageEvents.AddDataHandler(EDataNames.TAXI_DIST, UpdateTaxiUIAndState);
	}

	private void OnRender()
	{
		if (m_JobState == ETaxiJob_State.GotoPickup)
		{
			RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
			float fDist = WorldHelper.GetDistance(vecPlayerPos, m_vecCurrentCheckpointPos);

			if (!m_bPendingOperation && fDist <= g_fRadius)
			{
				m_JobState = ETaxiJob_State.DriveToLocation;

				NetworkEventSender.SendNetworkEvent_TaxiDriverJob_AtPickup();
				CleanupAll();
			}
		}
	}

	private void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		// All passengers can see the taxi UI
		EVehicleType vehicleType = DataHelper.GetEntityData<EVehicleType>(vehicle, EDataNames.VEHICLE_TYPE);
		if (vehicleType == EVehicleType.TaxiJob)
		{
			ShowTaxiUI(seatId == (int)EVehicleSeat.Driver);
		}
	}

	private void OnExitVehicle(RAGE.Elements.Vehicle vehicle)
	{
		// All passengers can see the taxi UI
		if (vehicle != null)
		{
			EVehicleType vehicleType = DataHelper.GetEntityData<EVehicleType>(vehicle, EDataNames.VEHICLE_TYPE);
			if (vehicleType == EVehicleType.TaxiJob)
			{
				HideTaxiUI();
			}
		}
		else // probably safe to always hide?
		{
			HideTaxiUI();
		}
	}

	private void OnEntityStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			bool bTaxiAFH = DataHelper.GetEntityData<bool>(entity, EDataNames.TAXI_AFH);

			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;
			vehicle.SetTaxiLights(bTaxiAFH);
		}
	}

	private void UpdateTaxiUIAndState(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;

			bool bTaxiAFH = DataHelper.GetEntityData<bool>(entity, EDataNames.TAXI_AFH);
			vehicle.SetTaxiLights(bTaxiAFH);

			if (vehicle == RAGE.Elements.Player.LocalPlayer.Vehicle)
			{
				float fCostPerMile = DataHelper.GetEntityData<float>(vehicle, EDataNames.TAXI_CPM);
				float fCurrentDist = DataHelper.GetEntityData<float>(vehicle, EDataNames.TAXI_DIST);
				float fCurrentCost = fCostPerMile * fCurrentDist;

				m_TaxiUI.SetCurrentFare(fCurrentCost, fCurrentDist);
				m_TaxiUI.SetFarePerMile(fCostPerMile);
			}
		}
	}

	private void OnTaxiAccepted(RAGE.Vector3 vecPickupPos)
	{
		CleanupAll();

		m_Blip = new RAGE.Elements.Blip(480, vecPickupPos, "Taxi Pickup");
		m_Blip.SetRoute(true);
		m_Blip.SetRouteColour(9);

		m_JobState = ETaxiJob_State.GotoPickup;

		// Marker
		vecPickupPos.Z = WorldHelper.GetGroundPosition(vecPickupPos);
		m_vecCurrentCheckpointPos = vecPickupPos;
		m_Marker = new RAGE.Elements.Marker(1, m_vecCurrentCheckpointPos, g_fRadius, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.RGBA(255, 194, 15, 200)); ;
	}

	private void OnChangeFarePerMile_Submit(float fAmount)
	{
		NetworkEventSender.SendNetworkEvent_ChangeFarePerMile(fAmount);
	}

	private void OnChangeFarePerMile_Cancel()
	{

	}

	private void ShowTaxiUI(bool bIsDriver = true)
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		m_TaxiUI.SetVisible(true, false, false);

		float fCostPerMile = DataHelper.GetEntityData<float>(vehicle, EDataNames.TAXI_CPM);
		float fDistanceTravelled = DataHelper.GetEntityData<float>(vehicle, EDataNames.TAXI_DIST);
		float fCurrentCost = fCostPerMile * fDistanceTravelled;
		bool bAvailableForHire = DataHelper.GetEntityData<bool>(vehicle, EDataNames.TAXI_AFH);

		m_TaxiUI.SetFarePerMile(fCostPerMile);
		m_TaxiUI.SetCurrentFare(fCurrentCost, fDistanceTravelled);
		m_TaxiUI.SetIsDriver(bIsDriver);
		m_TaxiUI.SetAvailableForHire(bAvailableForHire);
	}

	private void UI_OnChangeFarePerMile()
	{
		var vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		if (vehicle != null)
		{
			if (m_JobState == ETaxiJob_State.GotoPickup || m_JobState == ETaxiJob_State.Idle)
			{
				float fCostPerMile = DataHelper.GetEntityData<float>(vehicle, EDataNames.TAXI_CPM);

				UserInputHelper.RequestUserInput("Taxi Fare", "Enter the full amount you wish to charge per mile", fCostPerMile.ToString(), UIEventID.OnChangeFarePerMile_Submit, UIEventID.OnChangeFarePerMile_Cancel);
			}
			else
			{
				NotificationManager.ShowNotification("Taxi", "You cannot change the fare during a pickup.", ENotificationIcon.ExclamationSign);
			}
		}
	}

	private void UI_OnToggleAvailableForHire()
	{
		var vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		if (vehicle != null)
		{
			bool bAvailableForHire = DataHelper.GetEntityData<bool>(vehicle, EDataNames.TAXI_AFH);
			m_TaxiUI.SetAvailableForHire(!bAvailableForHire);
			NetworkEventSender.SendNetworkEvent_ToggleAvailableForHire();
		}
	}

	private void UI_OnResetFare()
	{
		if (RAGE.Elements.Player.LocalPlayer.IsInAnyVehicle(false))
		{
			NetworkEventSender.SendNetworkEvent_ResetFare();
		}
	}

	private void HideTaxiUI()
	{
		m_TaxiUI.SetVisible(false, false, false);
	}

	private void OnChangeCharacter()
	{
		HideTaxiUI();
	}

	public override void CleanupAll()
	{
		if (m_Blip != null)
		{
			m_Blip.Destroy();
			m_Blip = null;
		}

		if (m_Marker != null)
		{
			m_Marker.Destroy();
			m_Marker = null;
		}

		m_vecCurrentCheckpointPos = null;
		m_JobState = ETaxiJob_State.Idle;
	}

	public override void Reset()
	{
		m_JobState = ETaxiJob_State.GetVehicle;
	}

	public override void OnEnteredJobVehicle()
	{
		ShowTaxiUI();
		m_JobState = ETaxiJob_State.Idle;
	}

	public override void OnExitedJobVehicle()
	{
		HideTaxiUI();
		m_JobState = ETaxiJob_State.GetVehicle;
	}

	private static void OnUILoaded()
	{

	}

	private const float WorldPedRotation = 7.23f;
	private const uint WorldPedDimension = 0;
	private const float g_fRadius = 5.0f;
	private ETaxiJob_State m_JobState = ETaxiJob_State.GetVehicle;
	private RAGE.Vector3 m_vecCurrentCheckpointPos = null;
	private RAGE.Elements.Blip m_Blip = null;
	private RAGE.Elements.Marker m_Marker = null;
	private CGUITaxiUI m_TaxiUI = new CGUITaxiUI(OnUILoaded);
}