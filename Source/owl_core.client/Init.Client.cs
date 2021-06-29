using owl_core.client;

class Init_Core : RAGE.Events.Script { Init_Core() { OwlScriptManager.RegisterScript<Core>(); } }

public class Core : OwlScript
{
	public Core()
	{
		PerfManager.Init();
		OptimizationCachePool.Init();
		KeyBinds.Init();
		ClientTimerPool.Init();
		WorldPedManager.Init();
		CursorManager.Init();
		ShardManager.Init();
		WorldPedManager.Init();
		WorldHintManager.Init();
		UserInputHelper.Init();
		PlayerList.Init();
		PlayerHelper.Init();
		WorldHelper.Init();
		HUD.Init();
		NotificationManager.Init();
		VehicleHUD.Init();
		AudioManager.Init();
		GenericMessageBoxHelper.Init();
		GenericPromptHelper.Init();
		AsyncModelLoader.Init();
		AsyncAnimLoader.Init();
		CameraManager.Init();
		ScreenFadeHelper.Init();
		ScaleformManager.Init();
		MultiFrameWorkScheduler.Initialize();
		LargeDataTransferManager.Initialize();
		RageClientStorageManager.Initialize();
		OwlJSON.Init();
		GangTagPool.Init();

		m_WeaponSelector = new WeaponSelector();
		m_RAGEWeaponWorkaround = new RAGEWeaponWorkaround();
		m_GTAWeaponSwitchFix = new GTAWeaponSwitchFix();
		m_PDRenderTargets = new PDRenderTargets();
		m_KeyBindManager = new KeyBindManager();
		m_Nametags = new Nametags();
		m_MapLoader = new MapLoader();
		m_PlayerKeyBindManager = new PlayerKeyBindManager();
		m_WeatherAPI = new WeatherAPI();
		m_Island = new Island(); m_TrainManager = new TrainManager();
	}

	public static WeaponSelector GetWeaponSelector() { return m_WeaponSelector; }
	private static WeaponSelector m_WeaponSelector = null;

	public static PlayerKeyBindManager GetPlayerKeyBindManager() { return m_PlayerKeyBindManager; }
	private static PlayerKeyBindManager m_PlayerKeyBindManager = null;

	public static Nametags GetNametags() { return m_Nametags; }
	private static Nametags m_Nametags = null;

	private static RAGEWeaponWorkaround m_RAGEWeaponWorkaround = null;
	private static GTAWeaponSwitchFix m_GTAWeaponSwitchFix = null;
	private static PDRenderTargets m_PDRenderTargets = null;
	private static KeyBindManager m_KeyBindManager = null;
	private static MapLoader m_MapLoader = null;
	private static WeatherAPI m_WeatherAPI = null;

	private static Island m_Island = null; private static TrainManager m_TrainManager = null;
}