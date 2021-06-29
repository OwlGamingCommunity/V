using System;

public class PDMDC
{
	private CGUIPDMDC_Person m_MDCPersonUI = null;
	private CGUIPDMDC_Vehicle m_MDCVehicleUI = null;
	private CGUIPDMDC_Property m_MDCPropertyUI = null;

	public PDMDC()
	{
		NetworkEvents.ChangeCharacterApproved += CleanupUIs;
		NetworkEvents.MdcVehicleResult += OnVehicleFound;
		NetworkEvents.MdcPropertyResult += OnPropertyFound;
		NetworkEvents.MdcPersonResult += OnPersonFound;

		UIEvents.HideMDCUIs += CleanupUIs;
		UIEvents.MdcGotoProperty += GotoProperty;
		UIEvents.MdcGotoVehicle += GotoVehicle;
		UIEvents.MdcGotoPerson += GotoPerson;
	}

	~PDMDC()
	{

	}

	private void GotoProperty(Int64 propertyID)
	{
		NetworkEventSender.SendNetworkEvent_MdcGotoProperty(propertyID);
		CleanupUIs();
	}

	private void GotoVehicle(Int64 vehicleID)
	{
		NetworkEventSender.SendNetworkEvent_MdcGotoVehicle(vehicleID);
		CleanupUIs();
	}

	private void GotoPerson(string strName)
	{
		NetworkEventSender.SendNetworkEvent_MdcGotoPerson(strName);
		CleanupUIs();
	}

	private void CleanupUIs()
	{
		if (m_MDCVehicleUI != null)
		{
			m_MDCVehicleUI.SetVisible(false, false, false);
		}

		m_MDCVehicleUI = null;

		if (m_MDCPersonUI != null)
		{
			m_MDCPersonUI.SetVisible(false, false, false);
		}

		m_MDCPersonUI = null;

		if (m_MDCPropertyUI != null)
		{
			m_MDCPropertyUI.SetVisible(false, false, false);
		}

		m_MDCPropertyUI = null;
	}

	private void OnVehicleFound(CMdtVehicle vehicleInfo)
	{
		CleanupUIs();

		m_MDCVehicleUI = new CGUIPDMDC_Vehicle(() => { });
		m_MDCVehicleUI.SetVisible(true, true, false);
		m_MDCVehicleUI.ShowTerminal(vehicleInfo);
	}

	private void OnPropertyFound(CMdtProperty propertyInfo)
	{
		CleanupUIs();

		m_MDCPropertyUI = new CGUIPDMDC_Property(() => { });
		m_MDCPropertyUI.SetVisible(true, true, false);
		m_MDCPropertyUI.ShowTerminal(propertyInfo);
	}

	private void OnPersonFound(CStatsResult personInfo)
	{
		CleanupUIs();

		m_MDCPersonUI = new CGUIPDMDC_Person(() => { });
		m_MDCPersonUI.SetVisible(true, true, false);
		m_MDCPersonUI.ShowTerminal(personInfo);
	}
}

