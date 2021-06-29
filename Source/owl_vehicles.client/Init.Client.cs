using System;

class Init_VehicleSystem : RAGE.Events.Script { Init_VehicleSystem() { OwlScriptManager.RegisterScript<VehicleSystem>(); } }

class VehicleSystem : OwlScript
{
	public VehicleSystem()
	{
		try
		{
			CVehicleDefinition[] jsonData =
				OwlJSON.DeserializeObject<CVehicleDefinition[]>(CVehicleData.VehicleData, EJsonTrackableIdentifier.VehicleDefs);

			foreach (CVehicleDefinition vehicleDef in jsonData)
			{
				VehicleDefinitions.g_VehicleDefinitions.Add(vehicleDef.Index, vehicleDef);
			}
		}
		catch (Exception e)
		{
			ExceptionHelper.SendException(e);
		}

		m_VehicleSystemGeneral = new VehicleSystemGeneral();
		m_DirtSystem = new DirtSystem();
		m_TurnSignals = new TurnSignals();
		m_DrivingTest = new DrivingTest();
		m_FuelStations = new FuelStations();
		m_VehicleRepair = new VehicleRepair();
		m_CarWash = new CarWash();
		m_TowingSystem = new TowingSystem();
		m_CruiseControl = new CruiseControl();
		m_ModShop = new VehicleModShop();
		m_VehicleCrusher = new VehicleCrusher();
		m_VehiclesList = new VehiclesListUI();
	}

	public static VehicleSystemGeneral GetVehicleSystemGeneral() { return m_VehicleSystemGeneral; }
	private static VehicleSystemGeneral m_VehicleSystemGeneral = null;

	public static CruiseControl GetCruiseControl() { return m_CruiseControl; }
	private static CruiseControl m_CruiseControl = null;

	public static DirtSystem GetDirtSystem() { return m_DirtSystem; }
	private static DirtSystem m_DirtSystem = null;

	public static TurnSignals GetTurnSignals() { return m_TurnSignals; }
	private static TurnSignals m_TurnSignals = null;

	public static DrivingTest GetDrivingTest() { return m_DrivingTest; }
	private static DrivingTest m_DrivingTest = null;

	public static FuelStations GetFuelStations() { return m_FuelStations; }
	private static FuelStations m_FuelStations = null;

	public static VehicleRepair GetVehicleRepair() { return m_VehicleRepair; }
	private static VehicleRepair m_VehicleRepair = null;

	public static CarWash GetCarWash() { return m_CarWash; }
	private static CarWash m_CarWash = null;

	public static TowingSystem GetTowingSystem() { return m_TowingSystem; }
	private static TowingSystem m_TowingSystem = null;

	public static VehicleModShop GetVehicleModShop() { return m_ModShop; }
	private static VehicleModShop m_ModShop = null;

	public static VehicleCrusher GetVehicleCrusher() { return m_VehicleCrusher; }
	private static VehicleCrusher m_VehicleCrusher = null;

	public static VehiclesListUI GetVehiclesListUI() { return m_VehiclesList; }
	private static VehiclesListUI m_VehiclesList = null;
}