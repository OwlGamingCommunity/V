internal class CGUIPDMDC_Person : CEFCore
{
	public CGUIPDMDC_Person(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/mdc_person.html", EGUIID.MDCPerson, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void ShowTerminal(CStatsResult personInfo)
	{
		// Person info
		Execute("ShowTerminal", personInfo.id, personInfo.name, personInfo.age, personInfo.gender);

		// Vehicles
		foreach (SCharacterVehicleResult vehicleInfo in personInfo.m_lstVehicles)
		{
			string strDisplayName = "Unknown";
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicleInfo.model);

			if (vehicleDef != null)
			{
				strDisplayName = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
			}

			Execute("AddVehicle", vehicleInfo.id, strDisplayName, vehicleInfo.plate);
		}

		// Properties
		foreach (SCharacterPropertyResult propertyInfo in personInfo.m_lstProperties)
		{
			Execute("AddProperty", propertyInfo.id, propertyInfo.name);
		}
	}
}