class Init_AdminSystem : RAGE.Events.Script { Init_AdminSystem() { OwlScriptManager.RegisterScript<AdminSystem>(); } }

class AdminSystem : OwlScript
{
	public AdminSystem()
	{
		m_AdminHud = new AdminHud();
		m_AdminCheck = new AdminCheck();
		m_HelpCenter = new HelpCenter();
		m_ViewApplication = new ViewApplication();
		m_AdminRecon = new AdminRecon();
		m_AdminEntityDeleteConfirmation = new AdminEntityDeleteConfirmation();
		m_AdminCheckVeh = new AdminCheckVeh();
		m_AdminCheckInt = new AdminCheckInt();
		m_FourthOfJuly = new FourthOfJuly();
		m_AdminReports = new AdminReports();
		m_ClientSideDebug = new ClientSideDebug();
	}

	public static AdminHud GetAdminHud() { return m_AdminHud; }
	private static AdminHud m_AdminHud = null;

	public static AdminRecon GetAdminRecon() { return m_AdminRecon; }
	private static AdminRecon m_AdminRecon = null;

	public static AdminCheck GetAdminCheck() { return m_AdminCheck; }
	private static AdminCheck m_AdminCheck = null;

	public static AdminReports GetAdminReports() { return m_AdminReports; }
	private static AdminReports m_AdminReports = null;

	public static HelpCenter GetHelpCenter() { return m_HelpCenter; }
	private static HelpCenter m_HelpCenter = null;

	public static ViewApplication GetViewApplication() { return m_ViewApplication; }
	private static ViewApplication m_ViewApplication = null;

	public static AdminEntityDeleteConfirmation GetAdminEntityDeleteConfirmation() { return m_AdminEntityDeleteConfirmation; }
	private static AdminEntityDeleteConfirmation m_AdminEntityDeleteConfirmation = null;

	public static AdminCheckVeh GetAdminCheckVeh() { return m_AdminCheckVeh; }
	private static AdminCheckVeh m_AdminCheckVeh = null;

	public static AdminCheckInt GetAdminCheckInt() { return m_AdminCheckInt; }
	private static AdminCheckInt m_AdminCheckInt = null;

	public static FourthOfJuly GetFourthOfJuly() { return m_FourthOfJuly; }
	private static FourthOfJuly m_FourthOfJuly = null;

	public static ClientSideDebug GetClientSideDebug() { return m_ClientSideDebug; }
	private static ClientSideDebug m_ClientSideDebug = null;
}