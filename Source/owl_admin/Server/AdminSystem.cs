public class AdminSystem
{
	public AdminSystem()
	{

	}

	private AdminHud m_AdminHud = new AdminHud();
	private AdminJail m_AdminJail = new AdminJail();
	private AdminReports m_AdminReports = new AdminReports();
	private ApplicationAdminCommands.Applications m_Applications = new ApplicationAdminCommands.Applications();
	private Banks m_AdminCommandsBanks = new Banks();
	private Dancers m_AdminCommandsDancers = new Dancers();
	private PlayerAdminCommands.Factions m_AdminCommandsFactions = new PlayerAdminCommands.Factions();
	private FuelPoints m_AdminCommandsFuelPoints = new FuelPoints();
	private PlayerAdminCommands.General m_AdminCommandsGeneral = new PlayerAdminCommands.General();
	private InactivityScanner.InactivityScanner m_InactivityScanner = new InactivityScanner.InactivityScanner();
	private PlayerAdminCommands.Properties m_AdminCommandsProperties = new PlayerAdminCommands.Properties();
	private Radios m_AdminCommandsRadios = new Radios();
	private Stores m_AdminCommandsStores = new Stores();
	private PlayerAdminCommands.Vehicles m_AdminCommandsVehicles = new PlayerAdminCommands.Vehicles();
	private WorldBlips m_AdminCommandsWorldBlips = new WorldBlips();
}