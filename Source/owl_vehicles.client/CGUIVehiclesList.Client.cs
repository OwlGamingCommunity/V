using System.Collections.Generic;

internal class CGUIVehiclesList : CEFCore
{
	public CGUIVehiclesList(OnGUILoadedDelegate callbackOnLoad) : base("owl_vehicles.client/vehiclelist.html", EGUIID.VehiclesList, callbackOnLoad)
	{
		UIEvents.CloseVehiclesListUI += () => { VehicleSystem.GetVehiclesListUI().OnCloseVehiclesListUI(); };
	}

	public override void OnLoad()
	{

	}

	public void Initialize(Dictionary<int, CVehicleDefinition> vehiclesList)
	{
		Execute("loadVehicleData", OwlJSON.SerializeObject(vehiclesList.Values, EJsonTrackableIdentifier.LoadVehicleData));
	}

	public void LoadCategories(List<string> categories)
	{
		foreach (string category in categories)
		{
			Execute("loadCategory", category);
		}
	}
}