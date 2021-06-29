public class AdminCheckVeh
{
	private CGUIAdminCheckVeh m_AdminCheckVehUI = new CGUIAdminCheckVeh(OnUILoaded);

	private AdminCheckVehicleDetails m_CachedDetails = null;
	private long m_cachedVehicle = -1;

	public AdminCheckVeh()
	{
		NetworkEvents.AdminCheckVeh += OnAdminCheckVeh;
		NetworkEvents.AdminReloadCheckVehDetails += OnAdminReloadCheckVehDetails;
	}

	private static void OnUILoaded() { }

	public void OnSaveVehicleNote(string strNote)
	{
		if (m_cachedVehicle != -1 && !string.IsNullOrEmpty(strNote))
		{
			NetworkEventSender.SendNetworkEvent_SaveAdminVehicleNote(strNote, m_cachedVehicle);
		}
	}

	public void OnAdminReloadCheckVehDetails(AdminCheckVehicleDetails vehicleDetails)
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicleDetails.ModelHash);
		string vehicleModel = "Unknown";
		if (vehicleDef != null)
		{
			vehicleModel = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
		}

		vehicleDetails.Model = vehicleModel;

		m_CachedDetails = vehicleDetails;
		ApplyCheckVehDetails();
	}

	public void OnUpdateStolenState(bool stolen)
	{
		NetworkEventSender.SendNetworkEvent_UpdateStolenState(m_cachedVehicle, stolen);
	}

	public void OnCloseCheckVeh()
	{
		m_AdminCheckVehUI.SetVisible(false, false, false);
	}

	public void OnReloadCheckVehData()
	{
		NetworkEventSender.SendNetworkEvent_ReloadCheckVehData(m_cachedVehicle);
	}

	private void ApplyCheckVehDetails()
	{
		m_AdminCheckVehUI.Execute("SetAllData", OwlJSON.SerializeObject(m_CachedDetails, EJsonTrackableIdentifier.ApplyCheckVehDetails2));
	}

	private void OnAdminCheckVeh(long vehicleID, AdminCheckVehicleDetails vehicleDetails)
	{
		m_AdminCheckVehUI.SetVisible(true, true, false);

		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicleDetails.ModelHash);
		string vehicleModel = "Unknown";
		if (vehicleDef != null)
		{
			vehicleModel = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
		}
		vehicleDetails.Model = vehicleModel;

		m_cachedVehicle = vehicleID;
		m_CachedDetails = vehicleDetails;

		ApplyCheckVehDetails();
	}
}