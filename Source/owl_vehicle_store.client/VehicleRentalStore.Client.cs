using System;
using System.Collections.Generic;

public class VehicleRentalStore
{
	private CGUIVehicleRentalStore m_VehicleRentalStoreUI = new CGUIVehicleRentalStore(OnUILoaded);

	private List<WeakReference<CWorldPed>> m_lstWorldPeds_Vehicle = new List<WeakReference<CWorldPed>>();
	private List<WeakReference<CWorldPed>> m_lstWorldPeds_Boats = new List<WeakReference<CWorldPed>>();

	private EVehicleStoreRotationDirection m_RotationDirection = EVehicleStoreRotationDirection.None;
	private EVehicleStoreZoomDirection m_ZoomDirection = EVehicleStoreZoomDirection.None;

	private EVehicleStoreType m_storeType = EVehicleStoreType.Vehicles;

	const float g_fMaxZoom_Boat = 25.0f;
	const float g_fMinZoom_Boat = 5.0f;
	const float g_fMaxZoom_Vehicle = 7.5f;
	const float g_fMinZoom_Vehicle = 0.5f;
	float m_fCurrentZoom = 5.0f;

	private bool m_bDoorsOpen = false;

	private uint m_RentalLength = 1;

	private readonly Vector3Definition g_vecCarPos = new Vector3Definition(228.8492f, -992.0068f, -100.009995f);
	private readonly Vector3Definition g_vecBoatPos = new Vector3Definition(-877.6823f, -1363.707f, -0.5f);

	private float m_fCameraRot = 45.0f;

	// CAR DETAILS
	private RAGE.Elements.Vehicle m_Vehicle = null;
	private EScriptLocation m_Location = EScriptLocation.Paleto;
	private int m_CurrentVehicleIndex = -1;
	private RAGE.RGBA m_PrimaryColor = new RAGE.RGBA(0, 0, 0);
	private RAGE.RGBA m_SecondaryColor = new RAGE.RGBA(0, 0, 0);

	private List<Purchaser> m_lstPurchasers = new List<Purchaser>();
	private List<string> m_lstMethods = new List<string>();

	private RAGE.Vector3 m_vecExitPos = null;


	public VehicleRentalStore()
	{
		// TODO_CSHARP: Maybe we want to return an object that can be modified directly? Also weird that we have to reactive etc
		CameraManager.RegisterCamera(ECameraID.VEHICLE_STORE, g_vecCarPos.AsRageVector(), GetEntityPositionToUse(), new RAGE.Vector3(-90.0f, -90.0f, 45.0f), 60.0f);
		UpdateCamera();

		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.ChangeCharacterApproved += OnChangeCharacter;
		NetworkEvents.VehicleRentalStore_RequestInfoResponse += OnRequestInfoResponse;
		NetworkEvents.VehicleRentalStore_OnCheckoutResult += OnCheckoutResult;

		// Paleto (autos)
		var weakRefPed = WorldPedManager.CreatePed(EWorldPedType.VehicleRentalStore, 0xE7714013, new RAGE.Vector3(-56.70538f, -1089.907f, 26.42234f), 196.7531f, 8);
		weakRefPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Rent Vehicle", null, () => { OnInteractWithWorldPed(EScriptLocation.Paleto, EVehicleStoreType.Vehicles); }, false, false, 3.0f);
		m_lstWorldPeds_Vehicle.Add(weakRefPed);

		// LS (autos)
		weakRefPed = WorldPedManager.CreatePed(EWorldPedType.VehicleRentalStore, 0xE7714013, new RAGE.Vector3(-56.70538f, -1089.907f, 26.42234f), 196.7531f, 0);
		weakRefPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Rent Vehicle", null, () => { OnInteractWithWorldPed(EScriptLocation.LS, EVehicleStoreType.Vehicles); }, false, false, 3.0f);
		m_lstWorldPeds_Vehicle.Add(weakRefPed);

		// BOATS
		// LS
		weakRefPed = WorldPedManager.CreatePed(EWorldPedType.VehicleStore, 0xC85F0A88, new RAGE.Vector3(-840.6887f, -1349.56f, 1.605169f), 295.2087f, 0);
		weakRefPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Rent Boat", null, () => { OnInteractWithWorldPed(EScriptLocation.LS, EVehicleStoreType.Boats); }, false, false, 3.0f);
		m_lstWorldPeds_Boats.Add(weakRefPed);

	}

	~VehicleRentalStore()
	{

	}

	private void ResetData()
	{
		m_bDoorsOpen = false;
		m_RentalLength = 1;

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

	private void OnCheckoutResult(ERentVehicleResult result)
	{
		if (result == ERentVehicleResult.CannotAffordVehicle)
		{
			m_VehicleRentalStoreUI.ShowErrorMessage("You cannot afford to rent that vehicle:<br>Your bank balance is too low.");
		}
		else if (result == ERentVehicleResult.GenericFailureCheckNotification)
		{
			m_VehicleRentalStoreUI.ShowErrorMessage("The vehicle could not be purchased. Please check the notification.");
		}
		else if (result == ERentVehicleResult.Success)
		{
			OnExit();
		}
	}

	private void OnChangeCharacter()
	{
		OnExit();
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
				m_VehicleRentalStoreUI.AddDivider();
			}

			m_VehicleRentalStoreUI.AddPurchaser(purchaser.DisplayName);
		}

		m_VehicleRentalStoreUI.CommitPurchasers();

		if (m_storeType == EVehicleStoreType.Vehicles)
		{
			m_VehicleRentalStoreUI.SetStoreAsVehicleStore();
		}
		else if (m_storeType == EVehicleStoreType.Boats)
		{
			m_VehicleRentalStoreUI.SetStoreAsBoatStore();
			// Fake a class change to boats, no dropdown on this UI because theres only one category
			OnChangeClass("Boats");
		}

	}

	public void OnChangeVehicle(int vehicleIndex)
	{
		CreateVehicle(vehicleIndex);
		m_Vehicle.SetAlpha(255, true);
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
		if (m_VehicleRentalStoreUI.IsVisible())
		{
			RAGE.Elements.Player.LocalPlayer.Position = m_vecExitPos;
			RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
			RAGE.Elements.Player.LocalPlayer.SetAlpha(255, false);

			NetworkEventSender.SendNetworkEvent_RequestPlayerNonSpecificDimension();

			CameraManager.DeactivateCamera(ECameraID.VEHICLE_STORE);

			// TODO: Camera fades
			m_VehicleRentalStoreUI.SetVisible(false, false, true);
			HUD.SetVisible(true, false, false);

			// TODO_RAGE: Add a reset
			m_VehicleRentalStoreUI.Reload();

			if (m_Vehicle != null)
			{
				m_Vehicle.Destroy();
				m_Vehicle = null;
			}

			RAGE.Game.Ui.DisplayRadar(true);
		}
	}

	public void OnCheckout(int purchaserIndex)
	{
		if (m_Vehicle != null && m_CurrentVehicleIndex != -1)
		{
			if (purchaserIndex < 0)
			{
				m_VehicleRentalStoreUI.ShowErrorMessage("You must pick a valid purchaser & payment method.");
			}
			else
			{
				Purchaser currentPurchaser = m_lstPurchasers[purchaserIndex];
				NetworkEventSender.SendNetworkEvent_RentVehicle_OnCheckout(m_CurrentVehicleIndex, m_PrimaryColor.Red, m_PrimaryColor.Green, m_PrimaryColor.Blue, m_SecondaryColor.Red, m_SecondaryColor.Green, m_SecondaryColor.Blue, currentPurchaser.Type,
					currentPurchaser.ID, m_RentalLength, m_Location, m_storeType);
			}
		}
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

	public void OnChangeRentalLength(uint lengthInDays)
	{
		m_RentalLength = lengthInDays;
		UpdateVehicleInfo();
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

		UpdateVehicleInfo();
	}

	public void UpdateVehicleInfo()
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
				m_VehicleRentalStoreUI.SetVehicleInfo(fMaxBraking, fMaxTraction, fMaxSpeed, fMaxAcceleration, iNumPassengers, fMonthlyTax);
			}
			else if (m_storeType == EVehicleStoreType.Boats)
			{
				m_VehicleRentalStoreUI.SetVehicleInfo(fMaxBraking, -1.0f, -1.0f, fMaxAcceleration, iNumPassengers, fMonthlyTax);
			}


			// COST INFO
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(m_Vehicle.Model);
			if (vehicleDef != null)
			{
				float fTotalPrice = vehicleDef.RentalPricePerDay * m_RentalLength;
				m_VehicleRentalStoreUI.SetPriceInfo(vehicleDef.RentalPricePerDay, fTotalPrice);
			}
		}
	}

	public void OnChangeClass(string strClassName)
	{
		foreach (var kvPair in VehicleDefinitions.g_VehicleDefinitions)
		{
			CVehicleDefinition vehicleDef = kvPair.Value;
			if (vehicleDef.Class.ToLower() == strClassName.ToLower())
			{
				if (vehicleDef.IsRentable && vehicleDef.RentalPricePerDay > 0.0f)
				{
					string brandString = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
					m_VehicleRentalStoreUI.AddVehicle(kvPair.Key, brandString);
				}
			}
		}

		m_VehicleRentalStoreUI.CommitVehicles();
	}

	// TODO_CSHARP: Don't save character pos if in the warehouse
	private void OnInteractWithWorldPed(EScriptLocation location, EVehicleStoreType storeType)
	{
		ShowVehicleRentalStoreUI(location, storeType);
	}

	private void ShowVehicleRentalStoreUI(EScriptLocation location, EVehicleStoreType storeType)
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

		m_VehicleRentalStoreUI.SetVisible(true, true, true);

		m_PrimaryColor = new RAGE.RGBA(0, 0, 0);
		m_SecondaryColor = new RAGE.RGBA(0, 0, 0);
		m_CurrentVehicleIndex = -1;
		m_Location = location;

		m_lstPurchasers.Clear();
		m_lstMethods.Clear();

		m_VehicleRentalStoreUI.ResetData();

		NetworkEventSender.SendNetworkEvent_GetPurchaserAndPaymentMethods(EPurchaseAndPaymentMethodsRequestType.RentalCar);
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
		vecCamPosNew.Z += 2.5f;

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
		if (m_VehicleRentalStoreUI.IsVisible())
		{
			RAGE.Game.Ui.DisplayRadar(false);
		}
	}
}