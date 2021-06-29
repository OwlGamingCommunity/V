internal class CGUIVehicleModShop : CEFCore
{
	public CGUIVehicleModShop(OnGUILoadedDelegate callbackOnLoad) : base("owl_vehicles.client/vehiclemodshop.html", EGUIID.VehicleModShop, callbackOnLoad)
	{
		UIEvents.VehicleModShop_StartRotation += (EVehicleStoreRotationDirection direction) => { VehicleSystem.GetVehicleModShop().OnStartRotation(direction); };
		UIEvents.VehicleModShop_StopRotation += () => { VehicleSystem.GetVehicleModShop().OnStopRotation(); };
		UIEvents.VehicleModShop_ResetCamera += () => { VehicleSystem.GetVehicleModShop().OnResetCamera(); };

		UIEvents.VehicleModShop_StartZoom += (EVehicleStoreZoomDirection direction) => { VehicleSystem.GetVehicleModShop().OnStartZoom(direction); };
		UIEvents.VehicleModShop_StopZoom += () => { VehicleSystem.GetVehicleModShop().OnStopZoom(); };

		UIEvents.VehicleModShop_UpdateModIndex += (int category, int index) => { VehicleSystem.GetVehicleModShop().OnUpdateModIndex(category, index); };
		UIEvents.VehicleModShop_ChangeModCategory += (int category) => { VehicleSystem.GetVehicleModShop().OnChangeModCategory(category); };

		UIEvents.VehicleModShop_OnCheckout += () => { VehicleSystem.GetVehicleModShop().OnCheckout(); };
		UIEvents.VehicleModShop_OnExit_Discard += () => { VehicleSystem.GetVehicleModShop().OnExit_Discard(); };

		UIEvents.VehicleModShop_UpdatePrice += () => { VehicleSystem.GetVehicleModShop().OnUpdatePrice(); };

		UIEvents.VehicleModShop_SetPlateText += (string strPlateText) => { VehicleSystem.GetVehicleModShop().OnSetPlateText(strPlateText); };
		UIEvents.VehicleModShop_ResetPlate += () => { VehicleSystem.GetVehicleModShop().OnResetPlateText(); };

		UIEvents.VehicleModShop_OnChangeNeonsColor += (uint r, uint g, uint b) => { VehicleSystem.GetVehicleModShop().OnChangeNeonsColor(r, g, b); };
		UIEvents.VehicleModShop_UpdateNeonState += (bool bEnabled) => { VehicleSystem.GetVehicleModShop().OnUpdateNeonState(bEnabled); };
	}

	public override void OnLoad()
	{

	}

	public void ResetModOverview()
	{
		Execute("ResetModOverview");
	}

	public void ResetModOverviewToBlank()
	{
		Execute("ResetModOverviewToBlank");
	}

	public void AddModToOverview(string strName, string strDisplayPrice)
	{
		Execute("AddModToOverview", strName, strDisplayPrice);
	}

	public void ShowErrorMessage(string strMessage)
	{
		Execute("ShowErrorMessage", strMessage);
	}

	public void SetPrice(float fPrice, int CostGC)
	{
		Execute("SetPrice", fPrice, CostGC);
	}

	public void SetModCost(float fPrice, int CostGC)
	{
		Execute("SetModCost", fPrice, CostGC);
	}

	public void GotoCustomizeNeons(bool bEnabled, int r, int g, int b)
	{
		Execute("GotoCustomizeNeons", bEnabled, r, g, b);
	}

	public void SetNumModsThisCategory(int currentIndex, int numMods)
	{
		Execute("SetNumModsThisCategory", currentIndex, numMods);
	}

	public void FinalizeGotoSelectMod(int modCategory)
	{
		Execute("FinalizeGotoSelectMod", modCategory);
	}

	public void GotoCustomizePlateText(string strCurrentPlateText)
	{
		Execute("GotoCustomizePlateText", strCurrentPlateText);
	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void AddModCategory(int modCategoryIndex, string strDisplayName)
	{
		Execute("AddModCategory", modCategoryIndex, strDisplayName);
	}
}