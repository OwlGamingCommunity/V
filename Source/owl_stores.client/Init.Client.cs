class Init_StoreSystem_Core : RAGE.Events.Script { Init_StoreSystem_Core() { OwlScriptManager.RegisterScript<StoreSystem_Core>(); } }

class StoreSystem_Core : OwlScript
{
	public StoreSystem_Core()
	{
		m_StoreSystem = new StoreSystem();
		m_ClothingStore = new ClothingStore();
		m_BarberShop = new BarberShop();
		m_FurnitureStore = new FurnitureStore();
		m_TattooArtist = new TattooArtist();
		m_PlasticSurgeon = new PlasticSurgeon();
		m_DancerSystem = new DancerSystem();
		m_OutfitEditor = new OutfitEditor();
	}

	public static StoreSystem GetStoreSystem() { return m_StoreSystem; }
	private static StoreSystem m_StoreSystem = null;

	public static ClothingStore GetClothingStore() { return m_ClothingStore; }
	private static ClothingStore m_ClothingStore = null;

	public static BarberShop GetBarberShop() { return m_BarberShop; }
	private static BarberShop m_BarberShop = null;

	public static FurnitureStore GetFurnitureStore() { return m_FurnitureStore; }
	private static FurnitureStore m_FurnitureStore = null;

	public static TattooArtist GetTattooArtist() { return m_TattooArtist; }
	private static TattooArtist m_TattooArtist = null;

	public static PlasticSurgeon GetPlasticSurgeon() { return m_PlasticSurgeon; }
	private static PlasticSurgeon m_PlasticSurgeon = null;

	public static DancerSystem GetDancerSystem() { return m_DancerSystem; }
	private static DancerSystem m_DancerSystem = null;

	public static OutfitEditor GetOutfitEditor() { return m_OutfitEditor; }
	private static OutfitEditor m_OutfitEditor = null;
}