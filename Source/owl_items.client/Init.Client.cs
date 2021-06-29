class Init_ItemSystem : RAGE.Events.Script { Init_ItemSystem() { OwlScriptManager.RegisterScript<ItemSystem>(); } }

public class ItemSystem : OwlScript
{
	public ItemSystem()
	{
		CGangTagSpriteDefinition[] jsonData = OwlJSON.DeserializeObject<CGangTagSpriteDefinition[]>(CGangtagData.GangtagData, EJsonTrackableIdentifier.GangTagData);

		foreach (CGangTagSpriteDefinition spriteDef in jsonData)
		{
			GangTagSpriteDefinitions.g_GangTagSpriteDefinitions.Add(spriteDef.ID, spriteDef);
		}

		m_InventorySystem = new InventorySystem();
		m_FurnitureSystem = new FurnitureSystem();
		m_DrugEffects = new DrugEffects();
		m_PlayerInventory = new PlayerInventory();
		m_RetuneRadio = new RetuneRadio();
		m_Frisk = new Frisk();
		m_WorldItems = new WorldItems();
		m_VehicleInventory = new VehicleInventory();
		m_FurnitureInventory = new FurnitureInventory();
		m_PropertyInventory = new PropertyInventory();
		m_GangTagCreator = new GangTagCreator();
		m_GangTags = new GangTags();
		m_WeaponAttachmentsSystem = new WeaponAttachmentsSystem();
		m_MetalDetectors = new MetalDetector();
		m_PetSystem = new PetSystem();
		m_MarijuanaSystem = new MarijuanaSystem();
		m_BinocularSystem = new BinocularSystem();
		m_SmokingSystem = new SmokingSystem();
		m_NoteSystem = new NoteSystem();
		m_LockSmith = new LockSmith();
		m_GenericSystem = new GenericSystem();
		m_ItemsListUI = new ItemsListUI();
		m_ItemMoverUI = new ItemMoverUI();
		m_InformationMarkerSystem = new InformationMarkerSystem();
	}

	public static InventorySystem GetInventorySystem() { return m_InventorySystem; }
	private static InventorySystem m_InventorySystem = null;

	public static MarijuanaSystem GetMarijuanaSystem() { return m_MarijuanaSystem; }
	private static MarijuanaSystem m_MarijuanaSystem = null;

	public static BinocularSystem GetBinocularSystem() { return m_BinocularSystem; }
	private static BinocularSystem m_BinocularSystem = null;

	public static SmokingSystem GetSmokingSystem() { return m_SmokingSystem; }
	private static SmokingSystem m_SmokingSystem = null;

	public static FurnitureSystem GetFurnitureSystem() { return m_FurnitureSystem; }
	private static FurnitureSystem m_FurnitureSystem = null;

	public static WeaponAttachmentsSystem GetWeaponAttachmentsSystem() { return m_WeaponAttachmentsSystem; }
	private static WeaponAttachmentsSystem m_WeaponAttachmentsSystem = null;

	public static DrugEffects GetDrugEffects() { return m_DrugEffects; }
	private static DrugEffects m_DrugEffects = null;

	public static PlayerInventory GetPlayerInventory() { return m_PlayerInventory; }
	private static PlayerInventory m_PlayerInventory = null;

	public static VehicleInventory GetVehicleInventory() { return m_VehicleInventory; }
	private static VehicleInventory m_VehicleInventory = null;

	public static FurnitureInventory GetFurnitureInventory() { return m_FurnitureInventory; }
	private static FurnitureInventory m_FurnitureInventory = null;

	public static PropertyInventory GetPropertyInventory() { return m_PropertyInventory; }
	private static PropertyInventory m_PropertyInventory = null;

	public static Frisk GetFrisk() { return m_Frisk; }
	private static Frisk m_Frisk = null;

	public static GangTagCreator GetGangTagCreator() { return m_GangTagCreator; }
	private static GangTagCreator m_GangTagCreator = null;

	public static GangTags GetGangTags() { return m_GangTags; }
	private static GangTags m_GangTags = null;

	public static MetalDetector GetMetalDetectors() { return m_MetalDetectors; }
	private static MetalDetector m_MetalDetectors = null;


	public static NoteSystem GetNoteSystem() { return m_NoteSystem; }
	private static NoteSystem m_NoteSystem = null;

	private static RetuneRadio m_RetuneRadio = null;
	private static PetSystem m_PetSystem = null;
	private static WorldItems m_WorldItems = null;
	public static WorldItems GetWorldItems() { return m_WorldItems; }

	public static LockSmith GetLockSmith() { return m_LockSmith; }
	private static LockSmith m_LockSmith = null;

	public static GenericSystem GetGenericSystem() { return m_GenericSystem; }
	private static GenericSystem m_GenericSystem = null;

	public static ItemsListUI GetItemsListUI() { return m_ItemsListUI; }
	private static ItemsListUI m_ItemsListUI = null;

	public static ItemMoverUI GetItemMoverUI() { return m_ItemMoverUI; }
	private static ItemMoverUI m_ItemMoverUI = null;

	public static InformationMarkerSystem GetInformationMarkerSystem() { return m_InformationMarkerSystem; }
	private static InformationMarkerSystem m_InformationMarkerSystem = null;
}