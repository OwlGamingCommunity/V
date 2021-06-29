internal class CGUIVehicleRentalStore : CEFCore
{
	public CGUIVehicleRentalStore(OnGUILoadedDelegate callbackOnLoad) : base("owl_vehicle_store.client/vehiclerentalstore.html", EGUIID.VehicleRentalStore, callbackOnLoad)
	{
		UIEvents.VehicleRentalStore_StartRotation += (EVehicleStoreRotationDirection direction) => { VehicleStore_Core.GetVehicleRentalStore()?.OnStartRotation(direction); };
		UIEvents.VehicleRentalStore_StopRotation += () => { VehicleStore_Core.GetVehicleRentalStore()?.OnStopRotation(); };
		UIEvents.VehicleRentalStore_ResetCamera += () => { VehicleStore_Core.GetVehicleRentalStore()?.OnResetCamera(); };

		UIEvents.VehicleRentalStore_StartZoom += (EVehicleStoreZoomDirection direction) => { VehicleStore_Core.GetVehicleRentalStore()?.OnStartZoom(direction); };
		UIEvents.VehicleRentalStore_StopZoom += () => { VehicleStore_Core.GetVehicleRentalStore()?.OnStopZoom(); };

		UIEvents.VehicleRentalStore_OnChangeClass += (string strClassName) => { VehicleStore_Core.GetVehicleRentalStore()?.OnChangeClass(strClassName); };
		UIEvents.VehicleRentalStore_OnChangeVehicle += (int vehicleIndex) => { VehicleStore_Core.GetVehicleRentalStore()?.OnChangeVehicle(vehicleIndex); };

		UIEvents.VehicleRentalStore_OnChangePrimaryColor += (uint r, uint g, uint b) => { VehicleStore_Core.GetVehicleRentalStore()?.OnChangePrimaryColor(r, g, b); };
		UIEvents.VehicleRentalStore_OnChangeSecondaryColor += (uint r, uint g, uint b) => { VehicleStore_Core.GetVehicleRentalStore()?.OnChangeSecondaryColor(r, g, b); };

		UIEvents.VehicleRentalStore_OnCheckout += (int purchaserIndex) => { VehicleStore_Core.GetVehicleRentalStore()?.OnCheckout(purchaserIndex); };

		UIEvents.VehicleRentalStore_OnChangeRentalLength += (uint lengthInDays) => { VehicleStore_Core.GetVehicleRentalStore()?.OnChangeRentalLength(lengthInDays); };

		UIEvents.VehicleRentalStore_ToggleDoors += () => { VehicleStore_Core.GetVehicleRentalStore()?.ToggleDoors(); };

		UIEvents.VehicleRentalStore_Hide += () => { VehicleStore_Core.GetVehicleRentalStore()?.OnExit(); };
	}

	public override void OnLoad()
	{

	}

	public void ShowErrorMessage(string strMessage)
	{
		Execute("ShowErrorMessage", strMessage);
	}

	public void ResetData()
	{
		Execute("ResetData");
	}

	public void AddVehicle(int id, string strDisplayName)
	{
		Execute("AddVehicle", id, strDisplayName);
	}

	public void CommitVehicles()
	{
		Execute("CommitVehicles");
	}

	public void SetVehicleInfo(float fMaxBraking, float fMaxTraction, float fMaxSpeed, float fMaxAcceleration, int iNumPassengers, float fMonthlyTax)
	{
		Execute("SetVehicleInfo", fMaxBraking, fMaxTraction, fMaxSpeed, fMaxAcceleration, iNumPassengers, fMonthlyTax);
	}

	public void SetPriceInfo(float fPricePerDay, float fTotalPrice)
	{
		Execute("SetPriceInfo", fPricePerDay, fTotalPrice);
	}

	public void AddPurchaser(string strPurchaserName)
	{
		Execute("AddPurchaser", strPurchaserName);
	}

	public void CommitPurchasers()
	{
		Execute("CommitPurchasers");
	}

	public void AddDivider()
	{
		Execute("AddDivider");
	}

	public void SetStoreAsVehicleStore()
	{
		Execute("SetStoreAsVehicleStore");
	}

	public void SetStoreAsBoatStore()
	{
		Execute("SetStoreAsBoatStore");
	}

}