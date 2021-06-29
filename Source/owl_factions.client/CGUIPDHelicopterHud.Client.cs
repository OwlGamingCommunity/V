class CGUIPDHelicopterHUD : CEFCore
{
	public CGUIPDHelicopterHUD(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/pdhelicopterhud.html", EGUIID.PDHelicopterHud, callbackOnLoad)
	{
		UIEvents.PDHelicopterHUD_ToggleVehiclesUnoccupied += (bool bEnabled) => { FactionSystem.GetPDHelicopterHUD()?.OnToggleVehiclesUnoccupied(bEnabled); };
		UIEvents.PDHelicopterHUD_ToggleVehiclesOccupied += (bool bEnabled) => { FactionSystem.GetPDHelicopterHUD()?.OnToggleVehiclesOccupied(bEnabled); };
		UIEvents.PDHelicopterHUD_ToggleMovingVehiclesOnly += (bool bEnabled) => { FactionSystem.GetPDHelicopterHUD()?.OnToggleMovingVehiclesOnly(bEnabled); };
		UIEvents.PDHelicopterHUD_TogglePeople += (bool bEnabled) => { FactionSystem.GetPDHelicopterHUD()?.OnTogglePeople(bEnabled); };
		UIEvents.PDHelicopterHUD_ToggleNVG += (bool bEnabled) => { FactionSystem.GetPDHelicopterHUD()?.OnToggleNVG(bEnabled); };
		UIEvents.PDHelicopterHUD_ToggleThermal += (bool bEnabled) => { FactionSystem.GetPDHelicopterHUD()?.OnToggleThermal(bEnabled); };
	}

	public override void OnLoad()
	{

	}

	public void SetDefaults(bool vehicles_unoccupied, bool vehicles_occupied, bool moving_vehicles_only, bool people, bool nvg, bool thermal)
	{
		Execute("SetDefaults", vehicles_unoccupied, vehicles_occupied, moving_vehicles_only, people, nvg, thermal);
	}
}