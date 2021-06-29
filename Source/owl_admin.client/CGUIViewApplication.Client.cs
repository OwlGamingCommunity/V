internal class CGUIViewApplication : CEFCore
{
	public CGUIViewApplication(OnGUILoadedDelegate callbackOnLoad) : base("owl_admin.client/view_application.html", EGUIID.ViewApplication, callbackOnLoad)
	{
		UIEvents.GetPendingApplications += () => { AdminSystem.GetViewApplication().GetPendingApplications(); };
		UIEvents.ExitPendingAppsScreen += () => { AdminSystem.GetViewApplication().HidePendingApplications(); };
		UIEvents.RequestApplicationDetails += (int accountID) => { AdminSystem.GetViewApplication().RequestApplicationDetails(accountID); };

		UIEvents.ApproveApplication += (int accountID) => { AdminSystem.GetViewApplication().ApproveApplication(accountID); };
		UIEvents.DenyApplication += (int accountID) => { AdminSystem.GetViewApplication().DenyApplication(accountID); };
	}

	public override void OnLoad()
	{

	}

	public void Reset()
	{
		Execute("ResetVals");
	}

	public void AddApplication(int accountID, string strAccountName)
	{
		Execute("AddApplication", accountID, strAccountName);
	}

	public void CommitApplications()
	{
		Execute("CommitApplications");
	}

	public void ReceivedApplicationInformation(uint numApps, string q1, string q2, string q3, string q4, string a1, string a2, string a3, string a4)
	{
		Execute("ReceivedApplicationInformation", numApps, q1, q2, q3, q4, a1, a2, a3, a4);
	}
}