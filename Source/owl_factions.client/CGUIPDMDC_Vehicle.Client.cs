internal class CGUIPDMDC_Vehicle : CEFCore
{
	public CGUIPDMDC_Vehicle(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/mdc_vehicle.html", EGUIID.MDCVehicle, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void ShowTerminal(CMdtVehicle vehicleInfo)
	{
		string strDisplayName = "Unknown";
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicleInfo.model);

		if (vehicleDef != null)
		{
			strDisplayName = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
		}

		Execute("ShowTerminal", vehicleInfo.id, vehicleInfo.owner, vehicleInfo.owner_name, strDisplayName, vehicleInfo.plate_type,
			vehicleInfo.plate_text, vehicleInfo.color1_r, vehicleInfo.color1_g, vehicleInfo.color1_b, vehicleInfo.color2_r, vehicleInfo.color2_g, vehicleInfo.color2_b, vehicleInfo.livery);
	}
}