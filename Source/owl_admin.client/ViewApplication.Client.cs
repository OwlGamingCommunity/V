using System.Collections.Generic;

public class ViewApplication
{
	private CGUIViewApplication m_ViewApplicationUI = new CGUIViewApplication(OnUILoaded);

	public ViewApplication()
	{
		NetworkEvents.ChangeCharacterApproved += HidePendingApplications;
		NetworkEvents.Admin_GotPendingApps += OnGotPendingApps;
		NetworkEvents.PendingApplicationDetails += OnGotPendingApplicationDetails;
	}

	private static void OnUILoaded()
	{

	}

	public void GetPendingApplications()
	{
		NetworkEventSender.SendNetworkEvent_RequestPendingApplications();
	}

	public void HidePendingApplications()
	{
		m_ViewApplicationUI.SetVisible(false, false, false);
	}

	private void OnGotPendingApps(List<PendingApplication> lstPendingApps)
	{
		// TODO_CSHARP: Theres probably a race condition here for slower PCS that don't load the UI fast enough, we should really be caching this initial data and adding it in onLoaded if not loaded, here otherwise...
		m_ViewApplicationUI.SetVisible(true, true, false);

		m_ViewApplicationUI.Reset();

		foreach (PendingApplication pendingApp in lstPendingApps)
		{
			m_ViewApplicationUI.AddApplication(pendingApp.AccountID, pendingApp.AccountName);
		}

		m_ViewApplicationUI.CommitApplications();
	}

	public void RequestApplicationDetails(int accountID)
	{
		NetworkEventSender.SendNetworkEvent_RequestApplicationDetails(accountID);
	}

	public void OnGotPendingApplicationDetails(PendingApplicationDetails pendingAppDetails)
	{
		m_ViewApplicationUI.ReceivedApplicationInformation(pendingAppDetails.NumApps, pendingAppDetails.Questions[0], pendingAppDetails.Questions[1], pendingAppDetails.Questions[2], pendingAppDetails.Questions[3], pendingAppDetails.Answers[0], pendingAppDetails.Answers[1], pendingAppDetails.Answers[2], pendingAppDetails.Answers[3]);
	}
	public void ApproveApplication(int accountID)
	{
		NetworkEventSender.SendNetworkEvent_ApproveApplication(accountID);
	}
	public void DenyApplication(int accountID)
	{
		NetworkEventSender.SendNetworkEvent_DenyApplication(accountID);
	}
}