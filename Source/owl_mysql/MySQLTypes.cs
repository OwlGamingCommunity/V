using GTANetworkAPI;
using System;
using System.Collections.Generic;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

public class CMySQLRow
{
	public CMySQLRow()
	{

	}

	public T GetValue<T>(string strKey)
	{
		return (T)Convert.ChangeType(m_Fields[strKey], typeof(T));
	}

	public Dictionary<string, string> GetFields()
	{
		return m_Fields;
	}

	public string this[string strKey]
	{
		get => m_Fields[strKey];
		set => m_Fields[strKey] = value;
	}

	private readonly Dictionary<string, string> m_Fields = new Dictionary<string, string>();
}

public class CDatabaseStructureWorldBlip
{
	public CDatabaseStructureWorldBlip(CMySQLRow row)
	{
		ID = row.GetValue<EntityDatabaseID>("id");
		Name = row["name"];
		Sprite = row.GetValue<int>("sprite");
		Color = row.GetValue<int>("color");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
	}

	public EntityDatabaseID ID { get; }
	public string Name { get; }
	public int Sprite { get; }
	public int Color { get; }
	public Vector3 vecPos { get; }
}

public class CDatabaseStructureStore
{
	public CDatabaseStructureStore(CMySQLRow row)
	{
		storeID = row.GetValue<UInt32>("id");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		fRotZ = float.Parse(row["rz"]);
		storeType = (EStoreType)row.GetValue<int>("type");
		Dimension = Dimension.Parse(row["dimension"]);
		parentPropertyID = row.GetValue<EntityDatabaseID>("parent_property");
		lastRobbedAt = row.GetValue<Int64>("last_robbed_at");
	}

	public UInt32 storeID { get; }
	public Vector3 vecPos { get; }
	public float fRotZ { get; }
	public EStoreType storeType { get; }
	public Dimension Dimension { get; }
	public Int64 lastRobbedAt { get; }
	public EntityDatabaseID parentPropertyID { get; }
}

public class CDatabaseStructureDancer
{
	public CDatabaseStructureDancer(CMySQLRow row)
	{
		dancerID = row.GetValue<EntityDatabaseID>("id");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		fRotZ = float.Parse(row["rz"]);
		Dimension = Dimension.Parse(row["dimension"]);
		dancerSkin = row.GetValue<uint>("skin");
		parentPropertyID = row.GetValue<EntityDatabaseID>("parent_property");
		tipMoney = float.Parse(row["tip_money"]);
		bAllowTip = row.GetValue<bool>("allow_tip");
		animDict = row.GetValue<string>("anim_dict");
		animName = row.GetValue<string>("anim_name");
	}

	public EntityDatabaseID dancerID { get; }
	public Vector3 vecPos { get; }
	public float fRotZ { get; }
	public uint dancerSkin { get; }
	public Dimension Dimension { get; }
	public EntityDatabaseID parentPropertyID { get; }
	public float tipMoney { get; }
	public bool bAllowTip { get; }
	public string animDict { get; }
	public string animName { get; }
}

public class CDatabaseStructureDutyPoint
{
	public CDatabaseStructureDutyPoint(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		dutyType = (EDutyType)row.GetValue<int>("type");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		dimension = row.GetValue<Dimension>("dimension");
	}

	public EntityDatabaseID dbID { get; }
	public EDutyType dutyType { get; }
	public Vector3 vecPos { get; }
	public Dimension dimension { get; }
}

public class CDatabaseStructureFuelPoint
{
	public CDatabaseStructureFuelPoint(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		dimension = row.GetValue<Dimension>("dimension");
	}

	public EntityDatabaseID dbID { get; }
	public Vector3 vecPos { get; }
	public Dimension dimension { get; }
}

public class CDatabaseStructureCarWashPoint
{
	public CDatabaseStructureCarWashPoint(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		dimension = row.GetValue<Dimension>("dimension");
	}

	public EntityDatabaseID dbID { get; }
	public Vector3 vecPos { get; }
	public Dimension dimension { get; }
}

public class CDatabaseStructureScooterRentalShop
{
	public CDatabaseStructureScooterRentalShop(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		vecPedPos = new Vector3(float.Parse(row["ped_x"]), float.Parse(row["ped_y"]), float.Parse(row["ped_z"]));
		vecSpawnPos = new Vector3(float.Parse(row["spawn_x"]), float.Parse(row["spawn_y"]), float.Parse(row["spawn_z"]));
		pedHeading = float.Parse(row["ped_heading"]);
		spawnHeading = float.Parse(row["spawn_heading"]);
		pedDimension = row.GetValue<Dimension>("ped_dimension");
		spawnDimension = row.GetValue<Dimension>("spawn_dimension");
	}

	public EntityDatabaseID dbID { get; }
	public Vector3 vecPedPos { get; }
	public float pedHeading { get; }
	public Vector3 vecSpawnPos { get; }
	public float spawnHeading { get; }
	public Dimension pedDimension { get; }
	public Dimension spawnDimension { get; }
}

public class CDatabaseStructureVehicleRepairPoint
{
	public CDatabaseStructureVehicleRepairPoint(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		dimension = row.GetValue<Dimension>("dimension");
	}

	public EntityDatabaseID dbID { get; }
	public Vector3 vecPos { get; }
	public Dimension dimension { get; }
}

public class CDatabaseStructureVehicle
{
	public CDatabaseStructureVehicle(CMySQLRow row)
	{
		vehicleID = row.GetValue<EntityDatabaseID>("id");
		vehicleType = (EVehicleType)row.GetValue<int>("type");
		ownerID = row.GetValue<EntityDatabaseID>("owner");
		model = row.GetValue<uint>("model");

		vecDefaultSpawnPos = new Vector3(float.Parse(row["spawn_x"]), float.Parse(row["spawn_y"]), float.Parse(row["spawn_z"]));
		vecDefaultSpawnRot = new Vector3(float.Parse(row["spawn_rx"]), float.Parse(row["spawn_ry"]), float.Parse(row["spawn_rz"]));

		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		vecRot = new Vector3(float.Parse(row["rx"]), float.Parse(row["ry"]), float.Parse(row["rz"]));
		plateType = (EPlateType)row.GetValue<int>("plate_type");
		strPlateText = row["plate_text"];
		fFuel = row.GetValue<float>("fuel");

		colPrimaryR = row.GetValue<int>("color1_r");
		colPrimaryG = row.GetValue<int>("color1_g");
		colPrimaryB = row.GetValue<int>("color1_b");
		colSecondaryR = row.GetValue<int>("color2_r");
		colSecondaryG = row.GetValue<int>("color2_g");
		colSecondaryB = row.GetValue<int>("color2_b");
		PearlescentColor = row.GetValue<int>("pearlescent_color");


		colWheel = row.GetValue<int>("color_wheel");
		livery = row.GetValue<int>("livery");

		fDirt = row.GetValue<float>("dirt");
		fOdometer = row.GetValue<float>("odometer");
		health = row.GetValue<float>("health");
		bLocked = row.GetValue<bool>("locked");
		bEngineOn = row.GetValue<bool>("engine");

		iPaymentsRemaining = row.GetValue<int>("payments_remaining");
		iPaymentsMade = row.GetValue<int>("payments_made");
		iPaymentsMissed = row.GetValue<int>("payments_missed");
		fCreditAmount = row.GetValue<float>("credit_amount");
		expiryTime = row.GetValue<Int64>("expiry_time");

		dimension = (Dimension)Convert.ChangeType(row["dimension"], typeof(Dimension));
		bTowed = row.GetValue<bool>("towed");

		bNeons = row.GetValue<bool>("neons");
		neonR = row.GetValue<int>("neon_r");
		neonG = row.GetValue<int>("neon_g");
		neonB = row.GetValue<int>("neon_b");

		lastUsed = row.GetValue<Int64>("last_used");
		radio = row.GetValue<int>("radio");

		show_plate = row.GetValue<bool>("show_plate");
		bStolen = row.GetValue<bool>("stolen");
		transmissionType = (EVehicleTransmissionType)row.GetValue<int>("transmission");
		token_purchase = row.GetValue<bool>("is_token_purchase");
	}

	public void CopyInventory(List<CItemInstanceDef> a_Inventory)
	{
		inventory = a_Inventory;
	}

	public Dictionary<EModSlot, int> VehicleMods { get; set; } = null;
	public Dictionary<int, bool> VehicleExtras { get; set; } = null;

	public EntityDatabaseID vehicleID { get; }
	public EVehicleType vehicleType { get; }
	public EntityDatabaseID ownerID { get; }
	public uint model { get; }

	public Vector3 vecDefaultSpawnPos { get; }
	public Vector3 vecDefaultSpawnRot { get; }

	public Vector3 vecPos { get; }
	public Vector3 vecRot { get; }
	public EPlateType plateType { get; }
	public string strPlateText { get; }
	public float fFuel { get; }

	public int colPrimaryR { get; }
	public int colPrimaryG { get; }
	public int colPrimaryB { get; }
	public int colSecondaryR { get; }
	public int colSecondaryG { get; }
	public int colSecondaryB { get; }
	public int PearlescentColor { get; }

	public int colWheel { get; }
	public int livery { get; }

	public float fDirt { get; }
	public float fOdometer { get; }
	public float health { get; }
	public bool bLocked { get; }
	public bool bEngineOn { get; }

	public int iPaymentsRemaining { get; }
	public int iPaymentsMade { get; }
	public int iPaymentsMissed { get; }
	public float fCreditAmount { get; }
	public Int64 expiryTime { get; }
	public List<CItemInstanceDef> inventory { get; private set; }
	public Dimension dimension { get; }
	public bool bTowed { get; }
	public bool bNeons { get; }
	public int neonR { get; }
	public int neonG { get; }
	public int neonB { get; }
	public Int64 lastUsed { get; }
	public int radio { get; }
	public bool show_plate { get; }
	public bool token_purchase { get; }

	public bool bStolen { get; }
	public EVehicleTransmissionType transmissionType { get; }

}

public class CDatabaseStructureBank
{
	public CDatabaseStructureBank(CMySQLRow row)
	{
		BankID = row.GetValue<EntityDatabaseID>("id");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		fRot = row.GetValue<float>("rz");
		bankType = (EBankSystemType)row.GetValue<int>("type");
		dimension = row.GetValue<Dimension>("dimension");
	}

	public EntityDatabaseID BankID { get; }
	public Vector3 vecPos { get; }
	public float fRot { get; }
	public EBankSystemType bankType { get; }
	public Dimension dimension { get; }
}

public class CDatabaseStructureElevator
{
	public CDatabaseStructureElevator(CMySQLRow row)
	{
		elevatorID = row.GetValue<EntityDatabaseID>("id");
		entrancePosition = new Vector3(float.Parse(row["entrance_x"]), float.Parse(row["entrance_y"]), float.Parse(row["entrance_z"]));
		exitPosition = new Vector3(float.Parse(row["exit_x"]), float.Parse(row["exit_y"]), float.Parse(row["exit_z"]));
		exitDimension = row.GetValue<uint>("exit_dimension");
		startDimension = row.GetValue<uint>("entrance_dimension");
		isCarElevator = row.GetValue<bool>("car");
		startRotation = row.GetValue<float>("entrance_rot");
		endRotation = row.GetValue<float>("exit_rot");
		elevatorName = row.GetValue<string>("name");
	}

	public EntityDatabaseID elevatorID { get; }
	public Vector3 entrancePosition { get; }
	public Vector3 exitPosition { get; }
	public uint exitDimension { get; }
	public uint startDimension { get; }
	public bool isCarElevator { get; }
	public float startRotation { get; }
	public float endRotation { get; }
	public string elevatorName { get; }
}

public class CDatabaseStructureInformationMarker
{
	public CDatabaseStructureInformationMarker(CMySQLRow row)
	{
		ID = row.GetValue<EntityDatabaseID>("id");
		OwnerCharacterID = row.GetValue<EntityDatabaseID>("owner_char");
		Position = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		Dim = row.GetValue<uint>("dimension");
		strText = row.GetValue<string>("text");
	}

	public EntityDatabaseID ID { get; }
	public EntityDatabaseID OwnerCharacterID { get; }
	public Vector3 Position { get; }
	public uint Dim { get; }
	public string strText { get; }
}


public class CDatabaseStructureMetalDetector
{
	public CDatabaseStructureMetalDetector(CMySQLRow row)
	{
		metalDetectorID = row.GetValue<EntityDatabaseID>("id");
		detectorPosition = new Vector3(float.Parse(row["position_x"]), float.Parse(row["position_y"]), float.Parse(row["position_z"]));
		detectorRotation = row.GetValue<float>("rotation");
		detectorDimension = row.GetValue<uint>("dimension");
	}

	public EntityDatabaseID metalDetectorID { get; }
	public Vector3 detectorPosition { get; }
	public float detectorRotation { get; }
	public uint detectorDimension { get; }
}

public class CDatabaseStructureWorldItem
{
	public CDatabaseStructureWorldItem(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		itemID = (EItemID)row.GetValue<EntityDatabaseID>("item_id");
		itemValue = row["item_value"];
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		fRotX = row.GetValue<float>("rx");
		fRotY = row.GetValue<float>("ry");
		fRotZ = row.GetValue<float>("rz");
		droppedByID = row.GetValue<EntityDatabaseID>("dropped_by");
		dimension = row.GetValue<Dimension>("dimension");
		StackSize = row.GetValue<UInt32>("stack_size");
	}

	public EntityDatabaseID dbID { get; }
	public EItemID itemID { get; }
	public string itemValue { get; }
	public Vector3 vecPos { get; }
	public float fRotX { get; }
	public float fRotY { get; }
	public float fRotZ { get; }
	public EntityDatabaseID droppedByID { get; }
	public Dimension dimension { get; }
	public UInt32 StackSize { get; }
}

public class CDatabaseStructureFurnitureItem
{
	public CDatabaseStructureFurnitureItem(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		furnitureID = Convert.ToUInt32(row.GetValue<EntityDatabaseID>("item_id"));
		itemValue = row["item_value"];
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		fRot = row.GetValue<float>("rz");
		droppedByID = row.GetValue<EntityDatabaseID>("dropped_by");
	}

	public EntityDatabaseID dbID { get; }
	public uint furnitureID { get; }
	public Vector3 vecPos { get; }
	public float fRot { get; }
	public EntityDatabaseID droppedByID { get; }
	public string itemValue { get; }
}

public class CDatabaseStructureGangTag
{
	public CDatabaseStructureGangTag(CMySQLRow row)
	{
		dbID = row.GetValue<EntityDatabaseID>("id");
		ownerCharacterID = row.GetValue<EntityDatabaseID>("owner_char");
		vecPos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"]));
		fRotZ = row.GetValue<float>("rz");
		dimension = row.GetValue<Dimension>("dim");
		tagData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GangTagLayer>>(row["tagdata"]);
		progress = row.GetValue<float>("progress");
	}

	public EntityDatabaseID dbID { get; }
	public EntityDatabaseID ownerCharacterID { get; }
	public Vector3 vecPos { get; }
	public float fRotZ { get; }
	public Dimension dimension { get; }
	public List<GangTagLayer> tagData { get; }
	public float progress { get; }
}

public class CCharacterLook
{
	public CCharacterLook(CMySQLRow row)
	{
		CharacterID = row.GetValue<EntityDatabaseID>("character_id");
		Height = row.GetValue<int>("height");
		Weight = row.GetValue<int>("weight");
		PhysicalAppearance = row.GetValue<string>("physical_appearance");
		Scars = row.GetValue<string>("scars");
		Tattoos = row.GetValue<string>("tattoos");
		Makeup = row.GetValue<string>("makeup");
		CreatedAt = row.GetValue<int>("created_at");
		UpdatedAt = row.GetValue<int>("updated_at");
	}

	public EntityDatabaseID CharacterID { get; }
	public int Height { get; }
	public int Weight { get; }
	public string PhysicalAppearance { get; }
	public string Scars { get; }
	public string Tattoos { get; }
	public string Makeup { get; }
	public int CreatedAt { get; }
	public int UpdatedAt { get; }
}

public class CDatabaseStructureFaction
{
	public CDatabaseStructureFaction(CMySQLRow row)
	{
		factionID = row.GetValue<EntityDatabaseID>("id");
		factionType = (EFactionType)row.GetValue<int>("type");
		strName = row["name"];
		bOfficial = row.GetValue<bool>("official");
		strShortName = row["short_name"];
		strMessage = row["message"];
		fMoney = row.GetValue<float>("money");
		lstFactionRanks = new List<CFactionRank>();
		CreatorID = row.GetValue<EntityDatabaseID>("creator");
	}

	public EntityDatabaseID factionID { get; }
	public EFactionType factionType { get; }
	public string strName { get; }
	public bool bOfficial { get; }
	public string strShortName { get; }
	public string strMessage { get; }
	public float fMoney { get; }
	public List<CFactionRank> lstFactionRanks { get; }
	public EntityDatabaseID CreatorID { get; }
}

public class CDatabaseStructureFactionMembership
{
	public CDatabaseStructureFactionMembership(CMySQLRow row)
	{
		factionID = row.GetValue<EntityDatabaseID>("faction_id");
		Manager = row.GetValue<bool>("manager");
		Rank = row.GetValue<int>("rank_index");
	}

	public EntityDatabaseID factionID { get; }
	public bool Manager { get; }
	public int Rank { get; }

}

public class CDatabaseStructureCharacterLanguage
{
	public CDatabaseStructureCharacterLanguage(CMySQLRow row)
	{
		languageID = (ECharacterLanguage)row.GetValue<int>("language_id");
		Progress = row.GetValue<float>("progress");
		Active = row.GetValue<bool>("active");
	}

	public ECharacterLanguage languageID { get; }
	public float Progress { get; }
	public bool Active { get; }
}

public class CDatabaseStructurePersistentNotification
{
	public CDatabaseStructurePersistentNotification(CMySQLRow row)
	{
		ID = row.GetValue<EntityDatabaseID>("id");
		AccountID = row.GetValue<EntityDatabaseID>("account_id");
		Title = row.GetValue<string>("title");
		ClickEvent = row.GetValue<string>("click_event");
		Body = row.GetValue<string>("body");
		CreatedAt = row.GetValue<Int64>("created_at");
	}

	public EntityDatabaseID ID { get; }
	public EntityDatabaseID AccountID { get; }
	public string Title { get; }
	public string ClickEvent { get; }
	public string Body { get; }
	public Int64 CreatedAt { get; }
}

public class BasicAccountInfo
{
	public BasicAccountInfo(EGetAccountInfoResult result)
	{
		Result = result;
	}

	public BasicAccountInfo(int a_AccountID, string strUsername, UInt64 a_Discord, EAdminLevel a_AdminLevel)
	{
		AccountID = a_AccountID;
		Username = strUsername;
		Discord = a_Discord;
		AdminLevel = a_AdminLevel;
		Result = EGetAccountInfoResult.OK;
	}

	public int AccountID { get; }
	public string Username { get; }
	public EGetAccountInfoResult Result { get; }
	public EAdminLevel AdminLevel { get; }
	public UInt64 Discord { get; }

	public enum EGetAccountInfoResult
	{
		OK,
		NotFound,
		MultipleFound
	}
}

public class AccountBanDetails
{
	public AccountBanDetails()
	{
		IsBanned = false;
	}

	public AccountBanDetails(string a_Until, string a_Reason)
	{
		Until = a_Until;
		Reason = a_Reason;
		IsBanned = true;
	}

	public string GetDisplayReason()
	{
		if (Reason.StartsWith("[AC]"))
		{
			return "AntiCheat";
		}

		return Reason;
	}

	public string Until { get; }
	private string Reason { get; }
	public bool IsBanned { get; }
}

public class AdminCheckInfo
{
	public string AdminNotes { get; set; }
	public List<string> AdminHistory { get; set; } = new List<string>();
}

public class AdminCheckVehInfo
{
	public CDatabaseStructureVehicle VehicleDetails { get; set; }
	public List<CVehicleAction> VehicleActions { get; set; } = new List<CVehicleAction>();
	public List<CAdminVehicleNote> AdminVehicleNotes { get; set; } = new List<CAdminVehicleNote>();
	public string OwnerName { get; set; }
}

public class AdminCheckIntInfo
{
	public Database.Models.Property PropertyDetails { get; set; }
	public List<CPropertyAction> PropertyActions { get; set; } = new List<CPropertyAction>();
	public List<CAdminPropertyNote> AdminPropertyNotes { get; set; } = new List<CAdminPropertyNote>();
	public string OwnerName { get; set; }
}

public class PendingWeaponLicenseStates
{
	public EPendingFirearmLicenseState Tier1 { get; set; }
	public EPendingFirearmLicenseState Tier2 { get; set; }
}

public class SVerifyCharacterExists
{
	public bool CharacterExists { get; set; } = false;
	public int AccountID { get; set; }
	public EntityDatabaseID CharacterID { get; set; }
	public string CharacterNameClean { get; set; }
}

public class OwlMapDatabase
{
	public EntityDatabaseID MapID { get; set; }
	public EntityDatabaseID PropertyID { get; set; }
	public float MarkerX { get; set; }
	public float MarkerY { get; set; }
	public float MarkerZ { get; set; }
	public string UploadedAt { get; set; }
	public string UpdatedAt { get; set; }
	public List<OwlMapObjectDatabase> MapObjects { get; set; } = new List<OwlMapObjectDatabase>();
}

public class OwlMapObjectDatabase
{
	public string model { get; set; }
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public float rx { get; set; }
	public float ry { get; set; }
	public float rz { get; set; }
}

public class PendingFactionInvite
{
	public EntityDatabaseID SourceCharacter { get; set; }
	public EntityDatabaseID FactionID { get; set; }
}

public class SGetFactionMemberInst
{
	public SGetFactionMemberInst(string a_strName, int a_Rank, bool a_bManager, string a_LastSeen)
	{
		Name = a_strName;
		Rank = a_Rank;
		IsManager = a_bManager;
		LastSeen = a_LastSeen;
	}

	public string Name { get; set; }
	public string LastSeen { get; set; }
	public int Rank { get; set; }
	public bool IsManager { get; set; }
}

public class SGetFactionMembers
{
	public List<SGetFactionMemberInst> lstMembers { get; } = new List<SGetFactionMemberInst>();
}

public class SGetCharacterSkinData
{
	public uint SkinHash { get; set; }
	public int[] Models { get; set; }
	public int[] Textures { get; set; }
	public int[] PropsModels { get; set; }
	public int[] PropsTextures { get; set; }
}

public class CGetCharactersResult
{
	public List<GetCharactersCharacter> m_lstCharacters { get; } = new List<GetCharactersCharacter>();
}

public class SGetCharacter
{
	public EntityDatabaseID id { get; set; }
	public Vector3 pos { get; set; }
	public float rz { get; set; }
	public int health { get; set; }
	public int armor { get; set; }
	public float money { get; set; }
	public float bank_money { get; set; }
	public float pending_job_money { get; set; }
	public int payday_progress { get; set; }
	public string CharacterName { get; set; }
	public List<CItemInstanceDef> Inventory { get; } = new List<CItemInstanceDef>();
	public List<CDatabaseStructureCharacterLanguage> CharacterLanguages { get; } = new List<CDatabaseStructureCharacterLanguage>();
	public List<CDatabaseStructureFactionMembership> FactionMemberships { get; } = new List<CDatabaseStructureFactionMembership>();
	public List<PlayerKeybindObject> Keybinds { get; } = new List<PlayerKeybindObject>();
	public EJobID Job { get; set; }
	public int TruckerJobXP { get; set; }
	public int DeliveryDriverJobXP { get; set; }
	public int BusDriverJobXP { get; set; }
	public int MailmanJobXP { get; set; }
	public int TrashmanJobXP { get; set; }
	public int FishingXP { get; set; }
	public Dimension Dimension { get; set; }
	public EGender Gender { get; set; }
	public bool Cuffed { get; set; }
	public EntityDatabaseID Cuffer { get; set; }
	public Int64 UnjailTime { get; set; }
	public EPrisonCell CellNumber { get; set; }
	public float BailAmount { get; set; }
	public string JailReason { get; set; }
	public EDutyType Duty { get; set; }
	public bool CKed { get; set; }
	public ECharacterType CharacterType { get; set; }
	public float Impairment { get; set; }
	public bool DrugFX1 { get; set; }
	public bool DrugFX2 { get; set; }
	public bool DrugFX3 { get; set; }
	public bool DrugFX4 { get; set; }
	public bool DrugFX5 { get; set; }
	public Int64 DrugFX1_Duration { get; set; }
	public Int64 DrugFX2_Duration { get; set; }
	public Int64 DrugFX3_Duration { get; set; }
	public Int64 DrugFX4_Duration { get; set; }
	public Int64 DrugFX5_Duration { get; set; }
	public bool FirstUse { get; set; }
	public bool ShowSpawnSelector { get; set; }

	public UInt32 minutesPlayed { get; set; } = 0;
	public UInt32 Age { get; set; } = 0;
	public EPendingFirearmLicenseState pendingFirearmsLicenseStateTier1 { get; set; }
	public EPendingFirearmLicenseState pendingFirearmsLicenseStateTier2 { get; set; }
	public List<GangTagLayer> gangTags { get; set; } = new List<GangTagLayer>();
	public List<GangTagLayer> gangTagsWIP { get; set; } = new List<GangTagLayer>();
	public ECharacterVersions CreationVersion { get; set; } = ECharacterVersions.None;
	public ECharacterVersions CurrentVersion { get; set; } = ECharacterVersions.None;
	public bool PremadeMasked { get; set; } = false;
}

public class RegisterAPIResponse
{
	public bool success { get; set; } = false;
	public int account { get; set; } = -1;
	public string error { get; set; } = String.Empty;
	public string[] errors { get; set; } = Array.Empty<string>();
}

public class CLoginResult
{
	public Auth.ELoginResult m_Result { get; set; }
	public int m_UserID { get; set; } = 0;
	public string m_Username { get; set; }
	public EAdminLevel m_AdminLevel { get; set; } = 0;
	public EScripterLevel m_ScripterLevel { get; set; } = 0;
	public EApplicationState appState { get; set; } = EApplicationState.NoApplicationSubmitted;
	public UInt32 numApps { get; set; } = 0;
	public string serial { get; set; } = "";
	public string ip { get; set; } = "";
	public List<DonationInventoryItem> m_lstDonationInventory { get; } = new List<DonationInventoryItem>();
	public List<GameControlObject> m_lstControls { get; } = new List<GameControlObject>();
	public UInt32 minutesPlayed { get; set; } = 0;
	public int adminReportCount { get; set; } = 0;
	public bool localPlayerNametagToggled { get; set; }
	public UInt64 discordID { get; set; } = 0;
	public Int64 autoSpawnCharacter { get; set; } = -1;
	public Int32 adminJailMinutesRemaining { get; set; } = -1;
	public string adminJailReason { get; set; } = "";

}

public class CAutoLoginResult
{
	public bool m_bSuccessful { get; set; } = false;
	public EntityDatabaseID m_ui64UserID { get; set; } = 0;
}

public class STeleportPlace
{
	public bool found { get; set; } = false;
	public EntityDatabaseID id { get; set; }
	public string name { get; set; }
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public uint dimension { get; set; }
	public int admin_creator_id { get; set; }
}

public class CTeleportPlaces
{
	public List<STeleportPlace> places { get; } = new List<STeleportPlace>();
}

public class CDatabaseStructurePerformanceCapture
{
	public CDatabaseStructurePerformanceCapture(CMySQLRow row)
	{
		DBID = row.GetValue<EntityDatabaseID>("id");
		AccountID = row.GetValue<EntityDatabaseID>("account_id");
		strCombinedData = row.GetValue<string>("data");
	}

	public EntityDatabaseID DBID { get; }
	public EntityDatabaseID AccountID { get; }
	public string strCombinedData { get; }
}