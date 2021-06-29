class Init_VehicleStore : RAGE.Events.Script { Init_VehicleStore() { OwlScriptManager.RegisterScript<VehicleStore_Core>(); } }

class VehicleStore_Core : OwlScript
{
	public VehicleStore_Core()
	{
		m_VehicleStore = new VehicleStore();
		m_VehicleRentalStore = new VehicleRentalStore();
		m_VehicleRentalShopPeds = new VehicleRentalShopPeds();
	}

	public static VehicleStore GetVehicleStore() { return m_VehicleStore; }
	private static VehicleStore m_VehicleStore = null;

	public static VehicleRentalStore GetVehicleRentalStore() { return m_VehicleRentalStore; }
	private static VehicleRentalStore m_VehicleRentalStore = null;

	public static VehicleRentalShopPeds GetVehicleRentalShopPeds() { return m_VehicleRentalShopPeds; }
	private static VehicleRentalShopPeds m_VehicleRentalShopPeds = null;
}

// TODO_CSHARP: Get rid of static classes in all resources