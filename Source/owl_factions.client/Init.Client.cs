class Init_FactionSystem : RAGE.Events.Script { Init_FactionSystem() { OwlScriptManager.RegisterScript<FactionSystem>(); } }

public class FactionSystem : OwlScript
{
	public FactionSystem()
	{
		m_DutySystem = new DutySystem();
		m_FactionManagement = new FactionManagement();
		m_FactionInvites = new FactionInvites();
		m_EMSSystem = new EMSSystem();
		m_PDSearchlight = new PDSearchlight();
		m_SpikeStrips = new SpikeStrips();
		m_RoadblockSystem = new RoadblockSystem();
		m_PDShield = new PDShield();
		m_PDVehicleHUD = new PDVehicleHUD();
		m_PDHelicopterHUD = new PDHelicopterHUD();
		m_PDSystem = new PDSystem();
		m_FDSystem = new FDSystem();
		m_GovernmentBlips = new GovernmentBlips();
		m_PDMDC = new PDMDC();
		m_PDSpeedCameraSystem = new PDSpeedCameraSystem();
		m_PDGates = new PDGates();
		m_NewsSystem = new NewsSystem();
	}

	public static DutySystem GetDutySystem() { return m_DutySystem; }
	private static DutySystem m_DutySystem = null;

	public static FactionManagement GetFactionManagement() { return m_FactionManagement; }
	private static FactionManagement m_FactionManagement = null;

	public static FactionInvites GetFactionInvites() { return m_FactionInvites; }
	private static FactionInvites m_FactionInvites = null;

	public static EMSSystem GetEMSSystem() { return m_EMSSystem; }
	private static EMSSystem m_EMSSystem = null;

	public static PDSearchlight GetPDSearchlight() { return m_PDSearchlight; }
	private static PDSearchlight m_PDSearchlight = null;

	public static SpikeStrips GetSpikeStrips() { return m_SpikeStrips; }
	private static SpikeStrips m_SpikeStrips = null;

	public static RoadblockSystem GetRoadblockSystem() { return m_RoadblockSystem; }
	private static RoadblockSystem m_RoadblockSystem = null;

	public static PDShield GetPDShield() { return m_PDShield; }
	private static PDShield m_PDShield = null;


	public static PDHelicopterHUD GetPDHelicopterHUD() { return m_PDHelicopterHUD; }
	private static PDHelicopterHUD m_PDHelicopterHUD = null;

	public static PDVehicleHUD GetPDVehicleHUD() { return m_PDVehicleHUD; }
	private static PDVehicleHUD m_PDVehicleHUD = null;

	public static FDSystem GetFDSystem() { return m_FDSystem; }
	private static FDSystem m_FDSystem = null;

	public static PDSystem GetPDSystem() { return m_PDSystem; }
	private static PDSystem m_PDSystem = null;

	public static GovernmentBlips GetGovernmentBlips() { return m_GovernmentBlips; }
	private static GovernmentBlips m_GovernmentBlips = null;

	public static PDMDC GetPDMDC() { return m_PDMDC; }
	private static PDMDC m_PDMDC = null;

	public static PDSpeedCameraSystem GetPDSpeedCameraSystem() { return m_PDSpeedCameraSystem; }
	private static PDSpeedCameraSystem m_PDSpeedCameraSystem = null;

	public static PDGates GetPDGates() { return m_PDGates; }
	private static PDGates m_PDGates = null;

	public static NewsSystem GetNewsSystem() { return m_NewsSystem; }
	private static NewsSystem m_NewsSystem = null;
}