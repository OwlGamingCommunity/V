using System.Collections.Generic;

public class VehiclesListUI
{
	public VehiclesListUI()
	{
		NetworkEvents.ShowVehiclesList += ShowVehiclesList;
	}

	private void ShowVehiclesList()
	{
		List<string> categories = new List<string>
		{
			"all"
		};

		foreach (CVehicleDefinition vehicleDefinition in VehicleDefinitions.g_VehicleDefinitions.Values)
		{
			if (!categories.Contains(vehicleDefinition.Class))
			{
				categories.Add(vehicleDefinition.Class);
			}
		}

		m_vehiclesListUI.Initialize(VehicleDefinitions.g_VehicleDefinitions);
		m_vehiclesListUI.LoadCategories(categories);

		m_vehiclesListUI.SetVisible(true, true, false);
	}

	public void OnCloseVehiclesListUI()
	{
		m_vehiclesListUI.SetVisible(false, false, false);
	}

	private CGUIVehiclesList m_vehiclesListUI = new CGUIVehiclesList(() => { });
}