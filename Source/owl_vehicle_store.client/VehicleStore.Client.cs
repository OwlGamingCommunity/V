using RAGE.Elements;
using System;
using System.Collections.Generic;

public class VehicleStore
{
	private CGUIVehicleStore m_VehicleStoreUI = new CGUIVehicleStore(OnUILoaded);

	private List<WeakReference<CWorldPed>> m_lstWorldPeds_Vehicle = new List<WeakReference<CWorldPed>>();
	private List<WeakReference<CWorldPed>> m_lstWorldPeds_Boats = new List<WeakReference<CWorldPed>>();
	private List<Blip> m_lstWorldBlips_Boats = new List<Blip>();

	private EVehicleStoreRotationDirection m_RotationDirection = EVehicleStoreRotationDirection.None;
	private EVehicleStoreZoomDirection m_ZoomDirection = EVehicleStoreZoomDirection.None;

	private EVehicleStoreType m_storeType = EVehicleStoreType.Vehicles;

	const float g_fMaxZoom_Boat = 25.0f;
	const float g_fMinZoom_Boat = 5.0f;
	const float g_fMaxZoom_Vehicle = 7.5f;
	const float g_fMinZoom_Vehicle = 0.5f;
	float m_fCurrentZoom = 5.0f;

	private bool m_bDoorsOpen = false;

	private readonly Vector3Definition g_vecCarPos = new Vector3Definition(228.8492f, -992.0068f, -100.009995f);
	private readonly Vector3Definition g_vecBoatPos = new Vector3Definition(-877.6823f, -1363.707f, -0.5f);
	private float m_fCameraRot = 45.0f;

	// CAR DETAILS
	private RAGE.Elements.Vehicle m_Vehicle = null;
	private EScriptLocation m_Location = EScriptLocation.Paleto;
	private int m_CurrentVehicleIndex = -1;
	private RAGE.RGBA m_PrimaryColor = new RAGE.RGBA(0, 0, 0);
	private RAGE.RGBA m_SecondaryColor = new RAGE.RGBA(0, 0, 0);

	// DOWNPAYMENTS
	private float m_fDownpaymentAmount = 0.0f;
	private int m_iNumberMonthlyPayments = 60;

	private List<Purchaser> m_lstPurchasers = new List<Purchaser>();
	private List<string> m_lstMethods = new List<string>();

	private RAGE.Vector3 m_vecExitPos = null;


	public VehicleStore()
	{
		// TODO_CSHARP: Maybe we want to return an object that can be modified directly? Also weird that we have to reactive etc
		CameraManager.RegisterCamera(ECameraID.VEHICLE_STORE, g_vecCarPos.AsRageVector(), GetEntityPositionToUse(), new RAGE.Vector3(-90.0f, -90.0f, 45.0f), 60.0f);
		UpdateCamera();

		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.ChangeCharacterApproved += OnChangeCharacter;
		NetworkEvents.VehicleStore_RequestInfoResponse += OnRequestInfoResponse;
		NetworkEvents.VehicleStore_OnCheckoutResult += OnCheckoutResult;

		// Paleto (autos)
		var weakRefPed = WorldPedManager.CreatePed(EWorldPedType.VehicleStore, 0xE7714013, new RAGE.Vector3(-57.16781f, -1098.738f, 26.42244f), 10.288f, 8);
		weakRefPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Purchase Vehicle", null, () => { OnInteractWithWorldPed(EScriptLocation.Paleto, EVehicleStoreType.Vehicles); }, false, false, 3.0f);
		m_lstWorldPeds_Vehicle.Add(weakRefPed);

		// LS (autos)
		weakRefPed = WorldPedManager.CreatePed(EWorldPedType.VehicleStore, 0xE7714013, new RAGE.Vector3(-57.16781f, -1098.738f, 26.42244f), 10.288f, 0);
		weakRefPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Purchase Vehicle", null, () => { OnInteractWithWorldPed(EScriptLocation.LS, EVehicleStoreType.Vehicles); }, false, false, 3.0f);
		m_lstWorldPeds_Vehicle.Add(weakRefPed);

		// BOATS
		// LS
		weakRefPed = WorldPedManager.CreatePed(EWorldPedType.VehicleStore, 0xC85F0A88, new RAGE.Vector3(-846.0367f, -1333.93f, 1.605169f), 292.8937f, 0);
		weakRefPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Purchase Boat", null, () => { OnInteractWithWorldPed(EScriptLocation.LS, EVehicleStoreType.Boats); }, false, false, 3.0f);
		m_lstWorldPeds_Boats.Add(weakRefPed);

		// boat blip (no interior, so needs a blip)
		m_lstWorldBlips_Boats.Add(new Blip(410, new RAGE.Vector3(-846.0367f, -1333.93f, 1.605169f), "Boat Store", 1, 0, 255, 0, true));


	}

	~VehicleStore()
	{

	}

	private void OnCheckoutResult(EPurchaseVehicleResult result)
	{
		if (result == EPurchaseVehicleResult.CannotAffordVehicle)
		{
			m_VehicleStoreUI.ShowErrorMessage("You cannot afford that vehicle:<br>Your bank balance is too low.");
		}
		else if (result == EPurchaseVehicleResult.GenericFailureCheckNotification)
		{
			m_VehicleStoreUI.ShowErrorMessage("The vehicle could not be purchased. Please check the notification.");
		}
		else if (result == EPurchaseVehicleResult.Success)
		{
			OnExit();
		}
		else if (result == EPurchaseVehicleResult.CannotAffordDownpaymentForCredit)
		{
			m_VehicleStoreUI.ShowErrorMessage("You cannot afford that vehicle:<br>Your bank balance is too low for the down payment.");
		}
		else if (result == EPurchaseVehicleResult.MonthlyIncomeTooLowForCredit)
		{
			m_VehicleStoreUI.ShowErrorMessage("You cannot afford that vehicle:<br>Your monthly income is too low.");
		}
		else if (result == EPurchaseVehicleResult.Faction_CannotAffordVehicle)
		{
			m_VehicleStoreUI.ShowErrorMessage("Your faction cannot afford that vehicle:<br>Your bank balance is too low.");
		}
		else if (result == EPurchaseVehicleResult.Faction_CannotAffordDownpaymentForCredit)
		{
			m_VehicleStoreUI.ShowErrorMessage("Your faction cannot afford that vehicle:<br>Your bank balance is too low for the down payment.");
		}
		else if (result == EPurchaseVehicleResult.Faction_MonthlyIncomeTooLowForCredit)
		{
			m_VehicleStoreUI.ShowErrorMessage("Your faction cannot afford that vehicle:<br>Your monthly income is too low.");
		}
		else if (result == EPurchaseVehicleResult.VehicleTokensInvalidForFactions)
		{
			m_VehicleStoreUI.ShowErrorMessage("Vehicle Tokens can not be used for Faction Purchases.");
		}
		else if (result == EPurchaseVehicleResult.InvalidClassForVehicleToken)
		{
			m_VehicleStoreUI.ShowErrorMessage("Vehicle Tokens can only be used for 'Token Vehicle' class vehicles.");
		}
	}

	private void OnChangeCharacter()
	{
		OnExit();
	}

	public void ToggleDoors()
	{
		if (m_Vehicle != null)
		{
			m_bDoorsOpen = !m_bDoorsOpen;

			for (int doorIndex = 0; doorIndex < 6; ++doorIndex)
			{
				if (m_bDoorsOpen)
				{
					m_Vehicle.SetDoorShut(doorIndex, false);
				}
				else
				{
					m_Vehicle.SetDoorOpen(doorIndex, false, false);
				}
			}
		}
	}

	private void OnRequestInfoResponse(List<Purchaser> lstPurchasers, List<string> lstMethods)
	{
		m_lstPurchasers = lstPurchasers;
		m_lstMethods = lstMethods;

		bool bAddedSpacer = false;

		foreach (Purchaser purchaser in lstPurchasers)
		{
			if (purchaser.Type == EPurchaserType.Faction && !bAddedSpacer)
			{
				bAddedSpacer = true;
				m_VehicleStoreUI.AddDivider();
			}

			m_VehicleStoreUI.AddPurchaser(purchaser.DisplayName);
		}

		foreach (string strPaymentMethod in lstMethods)
		{
			m_VehicleStoreUI.AddMethod(strPaymentMethod);
		}

		m_VehicleStoreUI.CommitPurchasesAndMethods();

		if (m_storeType == EVehicleStoreType.Vehicles)
		{
			m_VehicleStoreUI.SetStoreAsVehicleStore();
		}
		else if (m_storeType == EVehicleStoreType.Boats)
		{
			m_VehicleStoreUI.SetStoreAsBoatStore();
			// Fake a class change to boats, no dropdown on this UI because theres only one category
			OnChangeClass("Boats");
		}
	}

	public void OnChangeVehicle(int vehicleIndex)
	{
		CreateVehicle(vehicleIndex);
	}

	public void OnChangePrimaryColor(uint r, uint g, uint b)
	{
		m_PrimaryColor.Red = r;
		m_PrimaryColor.Green = g;
		m_PrimaryColor.Blue = b;
		m_Vehicle.SetCustomPrimaryColour((int)m_PrimaryColor.Red, (int)m_PrimaryColor.Green, (int)m_PrimaryColor.Blue);
	}

	public void OnChangeSecondaryColor(uint r, uint g, uint b)
	{
		m_SecondaryColor.Red = r;
		m_SecondaryColor.Green = g;
		m_SecondaryColor.Blue = b;
		m_Vehicle.SetCustomSecondaryColour((int)m_SecondaryColor.Red, (int)m_SecondaryColor.Green, (int)m_SecondaryColor.Blue);
	}

	public void OnExit()
	{
		if (m_VehicleStoreUI.IsVisible())
		{
			RAGE.Elements.Player.LocalPlayer.Position = m_vecExitPos;
			RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
			RAGE.Elements.Player.LocalPlayer.SetAlpha(255, false);

			NetworkEventSender.SendNetworkEvent_RequestPlayerNonSpecificDimension();

			CameraManager.DeactivateCamera(ECameraID.VEHICLE_STORE);

			// TODO: Camera fades
			m_VehicleStoreUI.SetVisible(false, false, true);
			HUD.SetVisible(true, false, false);

			// TODO_RAGE: Add a reset
			m_VehicleStoreUI.Reload();

			ResetData();

			RAGE.Game.Ui.DisplayRadar(true);
		}
	}

	public void OnCheckout(int purchaserIndex, EPaymentMethod method)
	{
		if (m_Vehicle != null && m_CurrentVehicleIndex != -1)
		{
			if (purchaserIndex < 0 || method == EPaymentMethod.None)
			{
				m_VehicleStoreUI.ShowErrorMessage("You must pick a valid purchaser & payment method.");
			}
			else
			{
				Purchaser currentPurchaser = m_lstPurchasers[purchaserIndex];
				NetworkEventSender.SendNetworkEvent_PurchaseVehicle_OnCheckout(m_CurrentVehicleIndex, m_PrimaryColor.Red, m_PrimaryColor.Green, m_PrimaryColor.Blue, m_SecondaryColor.Red, m_SecondaryColor.Green, m_SecondaryColor.Blue, currentPurchaser.Type,
					currentPurchaser.ID, method, m_fDownpaymentAmount, m_iNumberMonthlyPayments, m_Location, m_storeType);
			}
		}
	}

	public void SetDownpayment(float fDownpayment)
	{
		m_fDownpaymentAmount = fDownpayment;
		UpdateVehicleInfo(true);
	}

	public void SetNumMonthlyPayments(int numMonthlyPayments)
	{
		m_iNumberMonthlyPayments = numMonthlyPayments;
		UpdateVehicleInfo(true);
	}

	private void CreateVehicle(int vehicleIndex)
	{
		m_CurrentVehicleIndex = vehicleIndex;

		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(vehicleIndex);
		if (vehicleDef == null)
		{
			// TODO: Error
			return;
		}

		RAGE.Vector3 vecGroundedPos = GetEntityPositionToUse();
		vecGroundedPos.Z = WorldHelper.GetGroundPosition(vecGroundedPos);

		uint modelHash = vehicleDef.Hash;

		if (vehicleDef.Hash == 0)
		{
			if (!string.IsNullOrEmpty(vehicleDef.AddOnName))
			{
				modelHash = HashHelper.GetHashUnsigned(vehicleDef.AddOnName);
			}
		}

		AsyncModelLoader.RequestSyncInstantLoad(modelHash);
		if (m_Vehicle == null)
		{
			m_Vehicle = new RAGE.Elements.Vehicle(modelHash, vecGroundedPos, 0.0f, "SA AUTOS", 255, true, 0, 0, RAGE.Elements.Player.LocalPlayer.Dimension);

		}
		else // Just change the model
		{
			m_Vehicle.Model = modelHash;
		}

		// fix for bike + motorbikes being underground
		int vehClass = m_Vehicle.GetClass();
		if (vehClass == (int)EVehicleClass.VehicleClass_Motorcycles || vehClass == (int)EVehicleClass.VehicleClass_Cycles)
		{
			vecGroundedPos.Z = GetEntityPositionToUse().Z + 0.5f;
		}

		m_Vehicle.Position = vecGroundedPos;
		m_Vehicle.SetRotation(0.0f, 0.0f, 0.0f, 1, true);
		m_Vehicle.SetAlpha(255, true);
		m_Vehicle.SetOnGroundProperly(0);
		m_Vehicle.FreezePosition(true);

		// re-apply color
		OnChangePrimaryColor(m_PrimaryColor.Red, m_PrimaryColor.Green, m_PrimaryColor.Blue);
		OnChangeSecondaryColor(m_SecondaryColor.Red, m_SecondaryColor.Green, m_SecondaryColor.Blue);

		UpdateVehicleInfo(false);
	}

	public void UpdateVehicleInfo(bool bPlayerActionDriven)
	{
		if (m_Vehicle != null)
		{
			float fMaxBraking = m_Vehicle.GetMaxBraking();

			float fMaxAcceleration = RAGE.Game.Invoker.Invoke<float>(RAGE.Game.Natives.GetVehicleModelAcceleration, (Int32)m_Vehicle.Model);
			int iNumPassengers = RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetVehicleMaxNumberOfPassengers, (Int32)m_Vehicle.Model);
			float fMonthlyTax = Taxation.GetVehicleMonthlyTaxRate(m_Vehicle.GetClass());

			if (m_storeType == EVehicleStoreType.Vehicles)
			{
				float fMaxSpeed = (RAGE.Game.Invoker.Invoke<float>(RAGE.Game.Natives.GetVehicleModelMaxSpeed, (Int32)m_Vehicle.Model) * 3600.0f) / 1609.344f;
				float fMaxTraction = m_Vehicle.GetMaxTraction();
				m_VehicleStoreUI.SetVehicleInfo(fMaxBraking, fMaxTraction, fMaxSpeed, fMaxAcceleration, iNumPassengers, fMonthlyTax);
			}
			else if (m_storeType == EVehicleStoreType.Boats)
			{
				m_VehicleStoreUI.SetVehicleInfo(fMaxBraking, -1.0f, -1.0f, fMaxAcceleration, iNumPassengers, fMonthlyTax);
			}


			// COST INFO
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(m_Vehicle.Model);
			if (vehicleDef != null)
			{
				float fRemainingPrice = vehicleDef.Price - m_fDownpaymentAmount;
				float fInterest = fRemainingPrice * Taxation.GetPaymentPlanInterestPercent();
				float fMonthlyPayment = (fRemainingPrice + fInterest) / m_iNumberMonthlyPayments;

				if (!bPlayerActionDriven) // we dont update text box if user changed it... otherwise their typing caret gets screwed
				{
					float fDownpaymentDefault = m_fDownpaymentAmount * vehicleDef.Price;
					float fMin = fDownpaymentDefault;
					float fMax = vehicleDef.Price;
					m_VehicleStoreUI.SetDownpayment(m_fDownpaymentAmount, fMin, fMax);
				}

				m_VehicleStoreUI.SetPriceInfo(vehicleDef.Price, fInterest, fRemainingPrice, fRemainingPrice + fInterest, fMonthlyPayment);
			}
		}
	}

	public void OnChangeClass(string strClassName)
	{
		foreach (var kvPair in VehicleDefinitions.g_VehicleDefinitions)
		{
			CVehicleDefinition vehicleDef = kvPair.Value;
			if (vehicleDef.Class.ToLower() == strClassName.ToLower() || (strClassName.ToLower() == "tokenvehicles" && vehicleDef.CanBuyWithToken))
			{
				if (vehicleDef.IsPurchasable)
				{
					string brandString = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
					m_VehicleStoreUI.AddVehicle(kvPair.Key, brandString);
				}
			}
		}

		m_VehicleStoreUI.CommitVehicles();
	}

	// TODO_CSHARP: Don't save character pos if in the warehouse
	private void OnInteractWithWorldPed(EScriptLocation location, EVehicleStoreType storeType)
	{
		ShowVehicleStoreUI(location, storeType);
	}

	private void ResetData()
	{
		m_bDoorsOpen = false;

		m_fCurrentZoom = GetDefaultZoomLevelToUse();
		m_fCameraRot = 45.0f;

		if (m_Vehicle != null)
		{
			m_Vehicle.Destroy();
			m_Vehicle = null;
		}

		m_CurrentVehicleIndex = -1;
		m_PrimaryColor = new RAGE.RGBA(0, 0, 0);
		m_SecondaryColor = new RAGE.RGBA(0, 0, 0);

		m_lstPurchasers.Clear();
		m_lstMethods.Clear();
	}

	private float GetDefaultZoomLevelToUse()
	{
		return (m_storeType == EVehicleStoreType.Vehicles) ? g_fMaxZoom_Vehicle : g_fMaxZoom_Boat;
	}

	private float GetMinZoom()
	{
		return (m_storeType == EVehicleStoreType.Vehicles) ? g_fMinZoom_Vehicle : g_fMinZoom_Boat;
	}

	private float GetMaxZoom()
	{
		return (m_storeType == EVehicleStoreType.Vehicles) ? g_fMaxZoom_Vehicle : g_fMaxZoom_Boat;
	}

	private RAGE.Vector3 GetEntityPositionToUse()
	{
		return (m_storeType == EVehicleStoreType.Vehicles) ? g_vecCarPos.AsRageVector() : g_vecBoatPos.AsRageVector();
	}

	private void ShowVehicleStoreUI(EScriptLocation location, EVehicleStoreType storeType)
	{
		m_storeType = storeType;
		ResetData();

		m_vecExitPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();

		NetworkEventSender.SendNetworkEvent_RequestPlayerSpecificDimension();

		HUD.SetVisible(false, false, false);

		OnResetCamera();
		UpdateCamera();

		RAGE.Vector3 vecCarPos = GetEntityPositionToUse();
		RAGE.Elements.Player.LocalPlayer.Position = vecCarPos.Add(new RAGE.Vector3(2.0f, 0.0f, 5.0f));
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		RAGE.Elements.Player.LocalPlayer.SetAlpha(0, false);

		CameraManager.ActivateCamera(ECameraID.VEHICLE_STORE);

		m_VehicleStoreUI.SetVisible(true, true, true);

		m_PrimaryColor = new RAGE.RGBA(0, 0, 0);
		m_SecondaryColor = new RAGE.RGBA(0, 0, 0);
		m_CurrentVehicleIndex = -1;
		m_Location = location;


		m_lstPurchasers.Clear();
		m_lstMethods.Clear();

		m_VehicleStoreUI.ResetData();

		NetworkEventSender.SendNetworkEvent_GetPurchaserAndPaymentMethods(EPurchaseAndPaymentMethodsRequestType.Vehicle);
	}

	private bool IsRotating()
	{
		return m_RotationDirection != EVehicleStoreRotationDirection.None;
	}

	private bool IsZooming()
	{
		return m_ZoomDirection != EVehicleStoreZoomDirection.None;
	}

	private static void OnUILoaded()
	{

	}

	public void OnStartRotation(EVehicleStoreRotationDirection direction)
	{
		m_RotationDirection = direction;
	}

	public void OnStopRotation()
	{
		m_RotationDirection = EVehicleStoreRotationDirection.None;
	}

	public void OnResetCamera()
	{
		m_RotationDirection = EVehicleStoreRotationDirection.None;
		m_ZoomDirection = EVehicleStoreZoomDirection.None;
		m_fCurrentZoom = GetDefaultZoomLevelToUse();
		m_fCameraRot = 45.0f;
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		// Calculate cam pos
		var radians = m_fCameraRot * (3.14 / 180.0);
		RAGE.Vector3 vecCamPosNew = GetEntityPositionToUse();
		vecCamPosNew.X += (float)Math.Cos(radians) * m_fCurrentZoom;
		vecCamPosNew.Y += (float)Math.Sin(radians) * m_fCurrentZoom;

		if (m_storeType == EVehicleStoreType.Vehicles)
		{
			vecCamPosNew.Z += 2.5f;
		}
		else if (m_storeType == EVehicleStoreType.Boats)
		{
			vecCamPosNew.Z += 10.0f;
		}

		CameraManager.UpdateCamera(ECameraID.VEHICLE_STORE, vecCamPosNew, GetEntityPositionToUse(), new RAGE.Vector3(-90.0f, -90.0f, 45.0f));
	}

	public void OnStartZoom(EVehicleStoreZoomDirection direction)
	{
		m_ZoomDirection = direction;
	}

	public void OnStopZoom()
	{
		m_ZoomDirection = EVehicleStoreZoomDirection.None;
	}

	private void OnTick()
	{
		if (IsRotating() && m_Vehicle != null)
		{
			const float fDeltaRot = 4.0f;

			if (m_RotationDirection == EVehicleStoreRotationDirection.Left)
			{
				m_fCameraRot -= fDeltaRot;
			}
			else if (m_RotationDirection == EVehicleStoreRotationDirection.Right)
			{
				m_fCameraRot += fDeltaRot;
			}

			if (m_fCameraRot >= 360.0f)
			{
				m_fCameraRot = 0.0f;
			}
			else if (m_fCameraRot <= 0.0f)
			{
				m_fCameraRot = 360.0f;
			}
		}

		if (IsZooming() && m_Vehicle != null)
		{
			const float fDeltaZoom = 0.1f;
			m_fCurrentZoom += (m_ZoomDirection == EVehicleStoreZoomDirection.In) ? -fDeltaZoom : fDeltaZoom;
			m_fCurrentZoom = Math.Clamp(m_fCurrentZoom, GetMinZoom(), GetMaxZoom());
		}

		if (IsRotating() || IsZooming())
		{
			UpdateCamera();
		}

		// TODO: Better way of tracking this
		if (m_VehicleStoreUI.IsVisible())
		{
			RAGE.Game.Ui.DisplayRadar(false);
		}
	}
}