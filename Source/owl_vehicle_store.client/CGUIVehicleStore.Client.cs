internal class CGUIVehicleStore : CEFCore
{
	public CGUIVehicleStore(OnGUILoadedDelegate callbackOnLoad) : base("owl_vehicle_store.client/vehiclestore.html", EGUIID.VehicleStore, callbackOnLoad)
	{
		UIEvents.VehicleStore_StartRotation += (EVehicleStoreRotationDirection direction) => { VehicleStore_Core.GetVehicleStore().OnStartRotation(direction); };
		UIEvents.VehicleStore_StopRotation += () => { VehicleStore_Core.GetVehicleStore().OnStopRotation(); };
		UIEvents.VehicleStore_ResetCamera += () => { VehicleStore_Core.GetVehicleStore().OnResetCamera(); };

		UIEvents.VehicleStore_StartZoom += (EVehicleStoreZoomDirection direction) => { VehicleStore_Core.GetVehicleStore().OnStartZoom(direction); };
		UIEvents.VehicleStore_StopZoom += () => { VehicleStore_Core.GetVehicleStore().OnStopZoom(); };

		UIEvents.VehicleStore_OnChangeClass += (string strClassName) => { VehicleStore_Core.GetVehicleStore().OnChangeClass(strClassName); };
		UIEvents.VehicleStore_OnChangeVehicle += (int vehicleIndex) => { VehicleStore_Core.GetVehicleStore().OnChangeVehicle(vehicleIndex); };

		UIEvents.VehicleStore_OnChangePrimaryColor += (uint r, uint g, uint b) => { VehicleStore_Core.GetVehicleStore().OnChangePrimaryColor(r, g, b); };
		UIEvents.VehicleStore_OnChangeSecondaryColor += (uint r, uint g, uint b) => { VehicleStore_Core.GetVehicleStore().OnChangeSecondaryColor(r, g, b); };

		UIEvents.VehicleStore_OnCheckout += (int purchaserIndex, EPaymentMethod method) => { VehicleStore_Core.GetVehicleStore().OnCheckout(purchaserIndex, method); };

		UIEvents.VehicleStore_SetMonthlyDownpayment += (float fDownpayment) => { VehicleStore_Core.GetVehicleStore().SetDownpayment(fDownpayment); };
		UIEvents.VehicleStore_SetNumMonthlyPayments += (int iNumPayments) => { VehicleStore_Core.GetVehicleStore().SetNumMonthlyPayments(iNumPayments); };

		UIEvents.VehicleStore_ToggleDoors += () => { VehicleStore_Core.GetVehicleStore().ToggleDoors(); };

		UIEvents.VehicleStore_Hide += () => { VehicleStore_Core.GetVehicleStore().OnExit(); };
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

	public void SetDownpayment(float fDownpayment, float fMin, float fMax)
	{
		Execute("SetDownpayment", fDownpayment, fMin, fMax);
	}

	public void SetPriceInfo(float fPrice, float fInterest, float fCreditAmount, float fMonthlyPaymentTotalAmount, float fMonthlyPayment)
	{
		Execute("SetPriceInfo", fPrice, fInterest, fCreditAmount, fMonthlyPaymentTotalAmount, fMonthlyPayment);
	}

	public void AddPurchaser(string strPurchaserName)
	{
		Execute("AddPurchaser", strPurchaserName);
	}

	public void AddMethod(string strMethodName)
	{
		Execute("AddMethod", strMethodName);
	}

	public void CommitPurchasesAndMethods()
	{
		Execute("CommitPurchasesAndMethods");
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