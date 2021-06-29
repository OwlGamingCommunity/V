class CGUIPDVehicleHUD : CEFCore
{
	public CGUIPDVehicleHUD(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/pdvehiclehud.html", EGUIID.PDVehicleHud, callbackOnLoad)
	{
		UIEvents.ToggleSpeedTrap += () => { FactionSystem.GetPDVehicleHUD()?.OnToggleSpeedTrap(); };
		UIEvents.ToggleSilentSiren += () => { FactionSystem.GetPDVehicleHUD()?.OnToggleSilentSiren(); };
		UIEvents.RunPlate += () => { FactionSystem.GetPDVehicleHUD()?.OnRunPlate(); };
	}

	public override void OnLoad()
	{

	}

	public void SetSpeed(double dSpeed)
	{
		Execute("SetSpeed", dSpeed);
	}

	public void SetSpeedLimit(int speedLimit)
	{
		Execute("SetSpeedLimit", speedLimit);
	}

	public void SetUnit(int unit)
	{
		Execute("SetUnit", unit);
	}

	public void SetSilentSiren(bool bEnabled)
	{
		Execute("SetSilentSiren", bEnabled);
	}

	public void SetVehicleInfo(string strVehicleString, string strPlateText)
	{
		Execute("SetVehicleInfo", strVehicleString, strPlateText);
	}

	public void SetDefaults()
	{
		SetSpeed(0.0f);
		SetVehicleInfo("Out Of Range", "N/A");
	}
}