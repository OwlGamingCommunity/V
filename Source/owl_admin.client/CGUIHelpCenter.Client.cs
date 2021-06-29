internal class CGUIHelpCenter : CEFCore
{
	public CGUIHelpCenter(OnGUILoadedDelegate callbackOnLoad) : base("owl_admin.client/help_center.html", EGUIID.HelpCenter, callbackOnLoad)
	{
		UIEvents.SubmitAdminReport += (EAdminReportType reportType, string strDetails, int playerID) => { AdminSystem.GetHelpCenter().SubmitAdminReport(reportType, strDetails, playerID); };

		UIEvents.HideHelpCenter += () => { AdminSystem.GetHelpCenter().Hide(); };
		UIEvents.CancelAdminReport += () => { AdminSystem.GetHelpCenter().CancelAdminReport(); };
		UIEvents.FindPlayerForReport += (string strPartialName) => { AdminSystem.GetHelpCenter().FindPlayerForReport(strPartialName); };

		UIEvents.HelpRequestCommands += () => { AdminSystem.GetHelpCenter().HelpRequestCommands(); };
	}

	public override void OnLoad()
	{

	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void Init(bool bHasAdminReport)
	{
		Execute("Init", bHasAdminReport);
	}

	public void FindPlayerForReportResult(int playerID, string strPlayerName)
	{
		Execute("FindPlayerForReportResult", playerID, strPlayerName);
	}

	public void AddCommandInfo(CommandHelpInfo cmdInfo)
	{
		Execute("AddCommandInfo", cmdInfo.Cmd, cmdInfo.Description, cmdInfo.Requirements, cmdInfo.Syntax);
	}

	public void CommitCommands()
	{
		Execute("CommitCommands");
	}
}