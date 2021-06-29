using System.Collections.Generic;

public class AdminReports
{
	private CGUIAdminReports m_AdminReportsUI = new CGUIAdminReports(OnUILoaded);

	public AdminReports()
	{
		NetworkEvents.Reports_SendReportData += ToggleReportData;
		NetworkEvents.Reports_UpdateReportData += OnAdminReloadReports;

		RageEvents.RAGE_OnTick_OncePerSecond += ReloadReportData;
	}

	private static void OnUILoaded() { }

	public void OnAdminReloadReports(List<CPlayerReport> reports)
	{
		m_AdminReportsUI.Execute("SetAllReports", OwlJSON.SerializeObject(reports, EJsonTrackableIdentifier.AdminReloadReports));
	}

	private void ReloadReportData()
	{
		if (m_AdminReportsUI.IsVisible())
		{
			NetworkEventSender.SendNetworkEvent_Reports_ReloadReportData();
		}
	}

	private void ToggleReportData(List<CPlayerReport> reports)
	{
		if (!m_AdminReportsUI.IsVisible())
		{
			m_AdminReportsUI.SetVisible(true, false, false);
			m_AdminReportsUI.Execute("SetAllReports", OwlJSON.SerializeObject(reports, EJsonTrackableIdentifier.ToggleReportData));
		}
		else
		{
			m_AdminReportsUI.SetVisible(false, false, false);
		}
	}
}