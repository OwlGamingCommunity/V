using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

#if SERVER
using Vector3 = GTANetworkAPI.Vector3;
using System.Net;
using System.Threading.Tasks;
#else
using Vector3 = RAGE.Vector3;
#endif

public enum EAchievementID : int
{
	None = -1,
	WelcomeToPaletoBay = 0,
	Play_5Hours = 1,
	Play_10Hours = 2,
	Play_50Hours = 3,
	Play_100Hours = 4,
	Play_500Hours = 5,
	Play_1000Hours = 6,
	Earn_10kMoney = 7,
	Earn_50kMoney = 8,
	Earn_100kMoney = 9,
	Earn_500kMoney = 10,
	Earn_1mMoney = 11,
	BuyCar = 12,
	TruckerJob = 13,
	DeliveryJob = 14,
	MailmanJob = 15,
	TaxiDriverJob = 16,
	TrashManJob = 17,
	BusDriverJob = 18,
	LEOFaction = 19,
	EMSFaction = 20,
	NewsFaction = 21,
	JoinFaction = 22,
	CreateFaction = 23,
	BeFined = 24,
	BeJailed = 25,
	ArrestSomeone = 26,
	ShootSomeoneAsCop = 27,
	BeShotByCop = 28,
	CustomCharacter = 29,
	BuyProperty = 30,
	CompleteTruckerJob = 31,
	CompleteDeliveryJob = 32,
	CompleteMailmanJob = 33,
	CompleteFareTaxiDriverJob = 34,
	CompleteTrashManJob = 35,
	CompleteBusDriverJob = 36,
	CrimeFaction = 37,
	BailOut = 38,
	IssueFine = 39,
	PremadeCharacter = 40,
	Uganda = 41,
	EatATaco = 42,
	WelcomeToLosSantos = 43,
	TagRemoverJob = 44,
	CreateCustomRadioStation = 45,
	Christmas = 46,
	Vubstersmurf = 47,
	StartFishing = 48,
	FishingMaxLevel = 49,
	BuyBoat = 50,
	LearnLanguage = 51,
	MakeOutfit = 52,
	Make10Outfits = 53,
	Halloween = 54,
	Halloween_RIP = 55,
	RunningCode = 56,
	GoingInQuiet = 57,
	RiskyMove = 58,
	NeverGoFullSemiAuto = 59,
	WinBlackjack = 60,
	LoseBlackjack = 61,
	GetBlackjack = 62,
	GoBust = 63,
	GetFiveCardsAndWinBlackjack = 64,
	SpeedCamera = 65,
	CayoPericoIsland = 66,
	Max
};

public static class FireConstants
{
	public const int MaxFires = 25;
}

public static class ChatConstants
{
	public const float g_fDistance_Nearby = 30.0f;
	public const float g_fDistance_Radio = 15.0f;
	public const float g_fDistance_Closeby = 2.0f;
	public const float g_fDistance_Shout = 50.0f;
	public const float g_fDistance_ShoutLoudly = 70.0f;
	public const float g_fDistance_Megaphone = 80.0f;
	public const float g_fDistance_District = 340.0f;

	public const int MaxTabs = 4;
}

public static class Constants
{
	public const uint DefaultWorldDimension = 0;
	public const uint PoliceLockerRoomDimensionPaleto = 3;
	public const uint PoliceLockerRoomDimensionLS = 0;
	public const uint FDEMSLockerRoomDimensionPaleto = 9;
	public const uint FDEMSLockerRoomDimensionLS = 0;
	public const uint TowedCarDimension = 65535;
	public const float CostToUnimpoundCar = 50.0f;

	public const int TimeBetweenPaydays = 3600000;
	public const float NearbyPlayerActionDistandThreshold = 5.0f;

	public const float BorderOfLStoPaleto = 1700.0f;
}

public class CCharacterLanguageTransmit
{
	public CCharacterLanguageTransmit(ECharacterLanguage a_CharacterLanguage, bool a_Active, float a_fProgress)
	{
		CharacterLanguage = a_CharacterLanguage;
		Active = a_Active;
		Progress = a_fProgress;
	}

	[JsonProperty]
	public ECharacterLanguage CharacterLanguage { get; set; }
	[JsonProperty]
	public bool Active { get; set; }
	[JsonProperty]
	public float Progress { get; set; }

}

public enum EMailboxAccessType
{
	NoAccess,
	AdminAccess,
	ReadOnly,
	ReadWrite
}


public class CFactionVehicleInfoTransmit
{
	[JsonProperty]
	public EntityDatabaseID ID { get; set; }

	[JsonProperty]
	public uint Hash { get; set; }

	[JsonProperty]
	public string Plate { get; set; }

	[JsonProperty]
	public float fX { get; set; }

	[JsonProperty]
	public float fY { get; set; }

	[JsonProperty]
	public float fZ { get; set; }

	public CFactionVehicleInfoTransmit(EntityDatabaseID a_id, uint a_vehicleHash, string a_vehiclePlate, float a_fX, float a_fY, float a_fZ)
	{
		ID = a_id;
		Hash = a_vehicleHash;
		Plate = a_vehiclePlate;
		fX = a_fX;
		fY = a_fY;
		fZ = a_fZ;
	}
}

public class CFactionTransmit
{
	public CFactionTransmit(EntityDatabaseID id, string a_ShortName, string a_Name, float a_fMoney, string a_MOTD, bool a_bManager)
	{
		ShortName = a_ShortName;
		Name = a_Name;
		Money = a_fMoney;
		MOTD = a_MOTD;
		IsManager = a_bManager;
		FactionID = id;
	}

	public void AddTag(string strName)
	{
		Tags.Add(strName);
	}

	public void AddRank(CFactionRankTransmit a_Rank)
	{
		Ranks.Add(a_Rank);
	}

	public void AddMember(CFactionMemberTransmit a_Member)
	{
		Members.Add(a_Member);
	}

	[JsonProperty]
	public List<string> Tags { get; set; } = new List<string>();

	[JsonProperty]
	public List<CFactionRankTransmit> Ranks { get; set; } = new List<CFactionRankTransmit>();

	[JsonProperty]
	public List<CFactionMemberTransmit> Members { get; set; } = new List<CFactionMemberTransmit>();

	[JsonProperty]
	public string ShortName { get; set; }
	[JsonProperty]
	public string Name { get; set; }
	[JsonProperty]
	public float Money { get; set; }
	[JsonProperty]
	public string MOTD { get; set; }
	[JsonProperty]
	public bool IsManager { get; set; }
	[JsonProperty]
	public EntityDatabaseID FactionID { get; set; }
}

public class CFactionMemberTransmit
{
	public CFactionMemberTransmit(string a_strName, int a_Rank, bool a_bManager, string a_StrSeen)
	{
		Name = a_strName;
		Rank = a_Rank;
		IsManager = a_bManager;
		LastSeen = a_StrSeen;
	}

	[JsonProperty]
	public string Name { get; set; }
	[JsonProperty]
	public string LastSeen { get; set; }
	[JsonProperty]
	public int Rank { get; set; }
	[JsonProperty]
	public bool IsManager { get; set; }
}

public class CFactionRankTransmit
{
	public CFactionRankTransmit(string a_strName, float a_fSalary)
	{
		Name = a_strName;
		Salary = a_fSalary;
	}

	[JsonProperty]
	public string Name { get; set; }
	[JsonProperty]
	public float Salary { get; set; }
}

public class CFactionListTransmit
{
	public CFactionListTransmit(EntityDatabaseID id, string a_ShortName, string a_Name, float a_fMoney, int a_NumMembers, Int64 a_CreatorID, string a_strCreatorName)
	{
		FactionID = id;
		ShortName = a_ShortName;
		Name = a_Name;
		Money = a_fMoney;
		NumMembers = a_NumMembers;
		CreatorID = a_CreatorID;
		strCreatorName = a_strCreatorName;
	}

	[JsonProperty]
	public EntityDatabaseID FactionID { get; set; }
	[JsonProperty]
	public string ShortName { get; set; }
	[JsonProperty]
	public string Name { get; set; }
	[JsonProperty]
	public float Money { get; set; }

	[JsonProperty]
	public int NumMembers { get; set; }
	[JsonProperty]
	public Int64 CreatorID { get; set; }
	[JsonProperty]
	public string strCreatorName { get; set; }
}

public enum ECreateFactionResult
{
	Success = -1,
	FullNameTooShort,
	ShortNameTooShort,
	FullNameTooLong,
	ShortNameTooLong,
	FullNameTaken,
	ShortNameTaken
}

public enum EFireType
{
	Circular,
	Linear,
	SemiCircle
}

public enum EFireMissionID
{
	ForestFire,
	//ForestFire2,
	ChurchFire,
	MotelFire,
	MAX
}

public enum EAnimCategory
{
	General,
	Dance,
	Misc,
	Sit,
	Lay,
	Lean,
	Stand,
	Smoke,
	Drink,
	Gestures,
	Workout,
	Surrender,
	Custom
}

public class CStatsResult
{
	public bool found { get; set; } = false;
	public EntityDatabaseID id { get; set; }
	public int gender { get; set; }
	public int age { get; set; }
	public string name { get; set; }
	public float money { get; set; }
	public float bank_money { get; set; }
	public int job { get; set; }
	public List<SCharacterVehicleResult> m_lstVehicles { get; set; } = new List<SCharacterVehicleResult>();
	public List<SCharacterPropertyResult> m_lstProperties { get; set; } = new List<SCharacterPropertyResult>();
}

public class SCharacterVehicleResult
{
	public EntityDatabaseID id { get; set; }
	public uint model { get; set; }
	public string plate { get; set; }
}

public class SCharacterPropertyResult
{
	public EntityDatabaseID id { get; set; }
	public string name { get; set; }
}

public class CMdtVehicle
{
	public bool found { get; set; } = false;
	public EntityDatabaseID id { get; set; }
	public int owner { get; set; }
	public string owner_name { get; set; }
	public uint model { get; set; }
	public int plate_type { get; set; }
	public string plate_text { get; set; }
	public int color1_r { get; set; }
	public int color1_g { get; set; }
	public int color1_b { get; set; }
	public int color2_r { get; set; }
	public int color2_g { get; set; }
	public int color2_b { get; set; }
	public int livery { get; set; }
}

public class CMdtProperty
{
	public bool found { get; set; } = false;
	public EntityDatabaseID id { get; set; }
	public string name { get; set; }
	public int? owner { get; set; }
	public string owner_name { get; set; }
	public int? renter { get; set; }
	public string renter_name { get; set; }
	public float entrance_x { get; set; }
	public float entrance_y { get; set; }
	public float entrance_z { get; set; }
	public float entrance_dimension { get; set; }
	public float buy_price { get; set; }
	public float rent_price { get; set; }
}

public class ChatSettings
{
	public List<ChatTab> Tabs { get; set; } = new List<ChatTab>();
	public int numMessagesToShow { get; set; } = 10;
	public bool chatboxBackground { get; set; } = true;
	public float chatboxBackgroundAlpha { get; set; } = 0.8f;
}

public class ChatTab
{
	public ChatTab()
	{

	}

	public ChatTab(string strName, bool bEnabled, Dictionary<EChatChannel, bool> bDictFilters)
	{
		if (bDictFilters.Count != Enum.GetValues(typeof(EChatChannel)).Length)
		{
			// TODO_CSHARP: Error, apply defaults?
			return;
		}

		Name = strName;
		Enabled = bEnabled;
		Channels = bDictFilters;
	}

	public string Name { get; set; }
	public bool Enabled { get; set; }
	public Dictionary<EChatChannel, bool> Channels { get; set; } // TODO: Storing this with strings as key seems like a bad idea for forwards comparability? renaming a variable would break peoples chat settings. Might be a way to map.
}

public class PendingApplication
{
	public int AccountID { get; set; }
	public string AccountName { get; set; }
	//public DateTime ApplicationTime; // TODO_APPS: Add and orderby on select
}

public class PendingApplicationDetails
{
	public UInt32 NumApps { get; set; }

	[JsonIgnore]
	public UInt32[] QuestionIndices { get; set; } = new UInt32[4];

	public string[] Questions { get; set; } = new string[4];
	public string[] Answers { get; set; } = new string[4];
}

public enum EPurchaseVehicleResult
{
	CannotAffordVehicle,
	GenericFailureCheckNotification,
	Success,
	CannotAffordDownpaymentForCredit,
	MonthlyIncomeTooLowForCredit,
	Faction_CannotAffordVehicle,
	Faction_CannotAffordDownpaymentForCredit,
	Faction_MonthlyIncomeTooLowForCredit,
	VehicleTokensInvalidForFactions,
	InvalidClassForVehicleToken,
}

public enum ERentVehicleResult
{
	CannotAffordVehicle,
	GenericFailureCheckNotification,
	Success
}

public enum EDrivingTestState
{
	Idle,
	GetVehicle,
	GotoCheckpoint,
	ReturnToVehicle,
	ReturnVehicle,
}
public enum EHeadlightState
{
	Off,
	On,
	On_FullBeam
}

public enum EChatChannel : int
{
	Nearby = 0,
	Factions = 1,
	Global = 2,
	Notifications = 3,
	Syntax = 4,
	AdminChat = 5,
	AdminActions = 6,
	AdminReports = 7
}

// Make sure you add its map into the map below also, otherwise it won't work
public enum ENotificationIcon
{
	ExclamationSign,
	InfoSign,
	Flash,
	Remove,
	USD,
	Phone,
	Headphones,
	ThumbsUp,
	PiggyBank,
	Font,
	HeartEmpty,
	Star,
	VolumeUp,
}

public enum EBankingResponseCode
{
	Success,
	Failed_CannotAfford,
	Failed_CharacterBelongsToSameAccount,
	Failed_TargetDoesntExist,
}

public enum ECallFailedReason
{
	NumberNotFoundOrPlayerOffline,
	PhoneEngaged,
	NoAnswer,
	OtherPersonCancelled,
}

public class GetCharactersCharacter
{
	public EntityDatabaseID id { get; set; } = 0;
	public string name { get; set; } = "";
	public Vector3 pos { get; set; }
	public float rz { get; set; }
	public int LastSeenHours { get; set; } = -1;
	public bool cked { get; set; }
}

public class AchievementTransmissionObject
{
	public int id { get; set; }
	public string title { get; set; }
	public string description { get; set; }
	public Int64 timestamp { get; set; }
	public int points { get; set; }
	public EAchievementRarity rarity { get; set; }
	public int percent { get; set; }

	public AchievementTransmissionObject(int a_id, string a_title, string a_description, Int64 a_timestamp, int a_points, EAchievementRarity a_rarity, int a_percent)
	{
		id = a_id;
		title = a_title;
		description = a_description;
		timestamp = a_timestamp;
		points = a_points;
		rarity = a_rarity;
		percent = a_percent;
	}
};

public enum EApplicationState
{
	NoApplicationSubmitted = 0,
	QuizCompleted = 1, // Quiz completed, not yet done written portion
	ApplicationPendingReview = 2,
	ApplicationApproved = 3,
	ApplicationRejected = 4
}


public enum EVehicleInventoryType
{
	NONE = -1,
	TRUNK,
	SEATS_AND_FLOOR,
	CENTER_CONSOLE_AND_GLOVEBOX
}

public enum EPropertyState
{
	AvailableToBuy = 0,
	AvailableToRent,
	Owned,
	Rented,
	AvailableToBuy_AlwaysEnterable,
	AvailableToRent_AlwaysEnterable,
	Owned_AlwaysEnterable,
	Rented_AlwaysEnterable,
};

public enum EPropertyType
{
	Owned = 0,
	Rented,
};

public enum EPropertyEntranceType
{
	Normal,
	World
}

public enum EPurchaserType
{
	Character,
	Faction
}

public enum EPaymentMethod
{
	None = -1,
	BankBalance,
	Credit,
	Token
}


public enum EBinocularsType
{
	None,
	Regular,
	ThermalOnly,
	Advanced,
}

public enum ESmokingItemType
{
	None,
	Cigarette,
	CigarBasic,
	CigarHighClass,
	Joint,
}

//only add at the bottom otherwise you will fuck a whole buch of things
public enum ECharacterLanguage
{
	None, // For resetting
	English,
	Russian,
	German,
	French,
	Dutch,
	Italian,
	Gaelic,
	Japanese,
	Chinese,
	Arabic,
	Norwegian,
	Swedish,
	Danish,
	Welsh,
	Hungarian,
	Bosnian,
	Somalian,
	Finnish,
	Georgian,
	Greek,
	Polish,
	Portugese,
	Turkish,
	Estonian,
	Korean,
	Viatnemese,
	Romanian,
	Albanian,
	Lithuanian,
	Serbian,
	Croatian,
	Slovak,
	Hebrew,
	Persian,
	Afrikaans,
	Latvian,
	Bulgarian,
	Armenian,
	Zulu,
	Punjabi,
	Spanish,
}

// TODO: We need support for other flags, e.g. faction owned AND rental, perhaps make it a mask?
// NOTE: If you extend this, please handle appropriately in MYSQL GetVehicleByType
public enum EVehicleType
{
	None = -1,
	PlayerOwned = 0,
	FactionOwned,
	Civilian,
	TruckerJob,
	DeliveryDriverJob,
	BusDriverJob,
	MailmanJob,
	RentalCar,
	Temporary,
	DrivingTest_Bike,
	DrivingTest_Car,
	DrivingTest_Truck,
	TrashmanJob,
	TaxiJob,
	FactionOwnedRental,
	TagRemoverJob
}

public enum EVehicleTransmissionType
{
	Automatic = 0,
	Manual = 1
}

public enum EVehicleClass
{
	VehicleClass_Compacts = 0,
	VehicleClass_Sedans = 1,
	VehicleClass_SUVs = 2,
	VehicleClass_Coupes = 3,
	VehicleClass_Muscle = 4,
	VehicleClass_SportsClassics = 5,
	VehicleClass_Sports = 6,
	VehicleClass_Super = 7,
	VehicleClass_Motorcycles = 8,
	VehicleClass_OffRoad = 9,
	VehicleClass_Industrial = 10,
	VehicleClass_Utility = 11,
	VehicleClass_Vans = 12,
	VehicleClass_Cycles = 13,
	VehicleClass_Boats = 14,
	VehicleClass_Helicopters = 15,
	VehicleClass_Planes = 16,
	VehicleClass_Service = 17,
	VehicleClass_Emergency = 18,
	VehicleClass_Military = 19,
	VehicleClass_Commercial = 20,
	VehicleClass_Trains = 21,
	VehicleClass_MAX = 22
};


public static class VehicleInventoryConstants
{
	// vehicle inventory sizes by class
	public static readonly int[] g_iVehicleInventorySizes = new int[]
		{
		5, // VehicleClass_Compacts
		10, // VehicleClass_Sedans
		15, // VehicleClass_SUVs
		10, // VehicleClass_Coupes
		10, // VehicleClass_Muscle
		5, // VehicleClass_SportsClassics
		5, // VehicleClass_Sports
		3, // VehicleClass_Super
		2, // VehicleClass_Motorcycles
		3, // VehicleClass_OffRoad
		20, // VehicleClass_Industrial 
		20, // VehicleClass_Utility 
		20, // VehicleClass_Vans 
		1, // VehicleClass_Cycles 
		15, // VehicleClass_Boats 
		20, // VehicleClass_Helicopters 
		20, // VehicleClass_Planes 
		20, // VehicleClass_Service 
		20, // VehicleClass_Emergency 
		20, // VehicleClass_Military 
		20, // VehicleClass_Commercial 
		0, // VehicleClass_Trains 
		};
}

public static class DrivingTestConstants
{
	public const float g_ColshapeRadius_Small = 3.0f;
	public const float g_ColshapeRadius_Medium = 4.0f;
	public const float g_ColshapeRadius_Large = 6.0f;
}


[Flags]
public enum AnimationFlags
{
	Loop = 1 << 0,
	StopOnLastFrame = 1 << 1,
	OnlyAnimateUpperBody = 1 << 4,
	AllowPlayerControl = 1 << 5,
	Cancellable = 1 << 7
}

public enum EStoreType
{
	Guns_Legal_Handguns,
	Guns_Legal_Shotguns,
	Guns_Legal_Rifles,
	Guns_Illegal_Handguns,
	Guns_Illegal_Shotguns,
	Guns_Illegal_Rifles,
	Guns_Illegal_Heavy,
	Guns_Illegal_Snipers,
	Guns_Illegal_SMG,
	General,
	Police,
	Hunting,
	Armor,
	Ammo,
	Clothing,
	Barber,
	Alcohol,
	Drugs,
	Furniture,
	Fishing,
	Fishmonger,
	TattooArtist,
	PlasticSurgeon,
	Tobbaco,
	Guns_Legal_SMG,
}

public enum EGender
{
	Male,
	Female
}
public enum ECharacterType
{
	Premade,
	Custom
};

public enum EDrivingTestType
{
	None,
	Bike,
	Car,
	Truck
}

public enum EScriptLocation
{
	LS,
	Paleto
}

public enum EVehicleStoreType
{
	Vehicles,
	Boats
}

public enum EDutyType
{
	None,
	Law_Enforcement,
	EMS,
	Fire,
	News,
	Towing
}

public class CPlayerReport
{
	public CPlayerReport(string a_strReportID, string a_strReporter, string a_strReporting, string a_strContent, string a_strReportType, string a_strHandlingAdmin)
	{
		reportID = a_strReportID;
		reporter = a_strReporter;
		reporting = a_strReporting;
		content = a_strContent;
		reportType = a_strReportType;
		handlingAdmin = a_strHandlingAdmin;
	}

	public string reportID { get; set; }
	public string reporter { get; set; }
	public string reporting { get; set; }
	public string content { get; set; }
	public string reportType { get; set; }
	public string handlingAdmin { get; set; }
}

public class CQuizWrittenQuestion
{
	public CQuizWrittenQuestion(uint a_index, string a_question, bool a_IsActive)
	{
		Question = a_question;
		IsActive = a_IsActive;
		Index = a_index;
	}

	public string Question { get; set; }

	[JsonIgnore]
	public bool IsActive { get; set; }

	[JsonIgnore]
	public uint Index { get; set; }
}

public class CPhoneMessage
{
	public CPhoneMessage(string a_strFrom, string a_strTo, string a_strContent, string a_strDate, bool a_viewed)
	{
		from = a_strFrom;
		to = a_strTo;
		content = a_strContent;
		date = a_strDate;
		viewed = a_viewed;
	}

	public string from { get; set; }
	public string to { get; set; }
	public string content { get; set; }
	public string date { get; set; }
	public bool viewed { get; set; }
}

public class CPhoneMessageContact
{
	public CPhoneMessageContact(string a_strNumber, string a_strEntryName, int a_viewed)
	{
		number = a_strNumber;
		entryName = a_strEntryName;
		viewed = a_viewed;
	}

	public string number { get; set; }
	public string entryName { get; set; }
	public int viewed { get; set; }
}

public class CQuizQuestion
{
	public CQuizQuestion(string a_question, string a_answer1, string a_answer2, string a_answer3, string a_answer4, int a_correct_answer_index)
	{
		Question = a_question;
		Answer1 = a_answer1;
		Answer2 = a_answer2;
		Answer3 = a_answer3;
		Answer4 = a_answer4;
		CorrectAnswerIndex = a_correct_answer_index;
	}

	public string Question { get; set; }
	public string Answer1 { get; set; }
	public string Answer2 { get; set; }
	public string Answer3 { get; set; }
	public string Answer4 { get; set; }

	[JsonIgnore]
	public int CorrectAnswerIndex { get; set; }
}

public enum ECreateCharacterResponse
{
	Success,
	Failed_NameInvalid,
	Failed_NameTaken,
	Failed_SameLanguage,
	Failed_NoLanguage
}

public enum EShieldType
{
	None,
	Riot,
	SWAT
}

// TODO_CSHARP: We need to stop using default(T) for enums... its zero which is bad
public enum EObjectTypes
{
	NONE = 0,
	OBJECT_TYPE_WORLD_ITEM = 1,
	SPIKE_STRIP,
	SPIKE_STRIP_NO_PICKUP,
	NEWS_CAMERA,
	ROADBLOCK
}

public enum EJobID
{
	None = -1,
	TruckerJob,
	DeliveryDriverJob,
	BusDriverJob,
	MailmanJob,
	TrashmanJob,
	TaxiDriverJob,
	TagRemoverJob
}

public enum EDataNames
{
	IS_LOGGED_IN,
	IS_SPAWNED,
	ACCOUNT_ID,
	MONEY,
	ADMIN_LEVEL,
	FUEL,
	DIRT,
	TURNSIGNAL_LEFT,
	TURNSIGNAL_RIGHT,
	BANK_MONEY,
	HEADLIGHTS,
	OBJECT_TYPE,
	ITEM_ID,
	VEHICLE_TYPE,
	JOB_ID,
	ATM,
	PLAYER_ID,
	PROP_ID,
	PROP_ENTER,
	PROP_EXIT,
	PROP_STATE,
	PROP_NAME,
	PROP_BUY_PRICE,
	PROP_OWNER_TEXT,
	PROP_ENT_TYPE,
	FUEL_POINT,
	FUEL_ID,
	CARWASH_POINT,
	CARWASH_ID,
	HAS_ANIM,
	ANIM_CANCELLABLE,
	ANIM_DATA,
	CUFFED,
	DUTY,
	SIREN_STATE,
	STORE,
	TAXI_CPM,
	TAXI_DIST,
	TAXI_AFH,
	VEHREP_POINT,
	VEHREP_ID,
	PING,
	IS_CUSTOM,
	CC_AGEING,
	CC_AGEINGOPACITY,
	CC_MAKEUP,
	CC_MAKEUPOPACITY,
	CC_BLUSH,
	CC_BLUSHOPACITY,
	CC_BLUSHCOLOR,
	CC_BLUSHCOLORHIGHLIGHT,
	CC_COMPLEXION,
	CC_COMPLEXIONOPACITY,
	CC_SUNDAMAGE,
	CC_SUNDAMAGEOPACITY,
	CC_LIPSTICK,
	CC_LIPSTICKOPACITY,
	CC_LIPSTICKCOLOR,
	CC_LIPSTICKCOLORHIGHLIGHTS,
	CC_MOLESANDFRECKLES,
	CC_MOLESANDFRECKLESOPACITY,
	CC_NOSESIZEHORIZONTAL,
	CC_NOSESIZEVERTICAL,
	CC_NOSESIZEOUTWARDS,
	CC_NOSESIZEOUTWARDSUPPER,
	CC_NOSESIZEOUTWARDSLOWER,
	CC_NOSEANGLE,
	CC_EYEBROWHEIGHT,
	CC_EYEBROWDEPTH,
	CC_CHEEKBONEHEIGHT,
	CC_CHEEKWIDTH,
	CC_CHEEKWIDTHLOWER,
	CC_EYESIZE,
	CC_LIPSIZE,
	CC_MOUTHSIZE,
	CC_MOUTHSIZELOWER,
	CC_CHINSIZE,
	CC_CHINSIZELOWER,
	CC_CHINWIDTH,
	CC_CHINEFFECT,
	CC_NECKWIDTH,
	CC_NECKWIDTHLOWER,
	CC_FACEBLEND1MOTHER,
	CC_FACEBLEND1FATHER,
	CC_FACEBLENDFATHERPERCENT,
	CC_SKINBLENDFATHERPERCENT,
	CC_BASEHAIR,
	CC_HAIRSTYLE,
	CC_HAIRCOLOR,
	CC_HAIRCOLORHIGHLIGHTS,
	CC_EYECOLOR,
	CC_FACIALHAIRSTYLE,
	CC_FACIALHAIRCOLOR,
	CC_FACIALHAIRCOLORHIGHLIGHT,
	CC_FACIALHAIROPACITY,
	CC_BLEMISHES,
	CC_BLEMISHESOPACITY,
	CC_EYEBROWS,
	CC_EYEBROWSOPACITY,
	CC_EYEBROWSCOLOR,
	CC_EYEBROWSCOLORHIGHLIGHT,
	CC_MAKEUPCOLOR,
	CC_MAKEUPCOLORHIGHLIGHT,
	CC_BODYBLEMISHES,
	CC_BODYBLEMISHESOPACITY,
	CC_CHESTHAIR,
	CC_CHESTHAIROPACITY,
	CC_CHESTHAIRCOLOR,
	CC_CHESTHAIRHIGHLIGHT,
	CC_TATTOOS, // NOTE: This is used as a max iter in CustomSkinDataHandler.Client.cs, if you change it change there too
	ODOMETER,
	HAS_CELL,
	IS_POLICE_VEHICLE,
	SEARCHLIGHT,
	SEARCHLIGHT_ROT,
	SHIELD,
	ITEM_SOCKET_0,
	ITEM_SOCKET_1,
	ITEM_SOCKET_2,
	ITEM_SOCKET_3,
	ITEM_SOCKET_4,
	ITEM_SOCKET_5,
	ITEM_SOCKET_6,
	ITEM_SOCKET_7,
	ITEM_SOCKET_8,
	ITEM_SOCKET_9,
	ITEM_SOCKET_10,
	ITEM_SOCKET_11,
	ITEM_SOCKET_12,
	ITEM_SOCKET_13,
	ITEM_SOCKET_14,
	ITEM_SOCKET_15,
	ITEM_SOCKET_16,
	ITEM_SOCKET_17,
	ITEM_SOCKET_18,
	ITEM_SOCKET_19, // UNUSED BUT REQUIRED FOR OFFSET CALC
	ITEM_SOCKET_20, // UNUSED BUT REQUIRED FOR OFFSET CALC
	ITEM_SOCKET_21, // UNUSED BUT REQUIRED FOR OFFSET CALC
	ITEM_SOCKET_22, // UNUSED BUT REQUIRED FOR OFFSET CALC
	ITEM_SOCKET_23, // NOTE: If you update this you must update the loops (search for it). Also note there is no 19, 20, 21, 22 because these are wallet, keyring, furniture storage and outfits.
	GENDER,
	VEH_DOOR_0,
	VEH_DOOR_1,
	VEH_DOOR_2,
	VEH_DOOR_3,
	VEH_DOOR_4,
	VEH_DOOR_5,
	IMPAIRMENT,
	/// <summary>
	/// Screen effect DrugsDrivingIn
	/// </summary>
	DRUG_FX_1,
	/// <summary>
	/// Screen effect DrugsMichaelAliensFight
	/// </summary>
	DRUG_FX_2,
	/// <summary>
	/// Screen effect DrugsTrevorClownsFight
	/// </summary>
	DRUG_FX_3,
	/// <summary>
	/// Screen effect DeadlineNeon
	/// </summary>
	DRUG_FX_4,
	/// <summary>
	/// Screen effect BeastLaunch
	/// </summary>
	DRUG_FX_5,
	PROP_MODELS,
	PROP_TEXTS,
	/// <summary>
	/// Used for /oldcar
	/// </summary>
	PREVIOUS_VEH,
	RECON,
	ADMIN_DUTY,
	LEO_BADGE,
	ENGINE,
	SCRIPTED_ID,
	CHARACTER_ID,
	VEH_RADIO,
	ELEVATOR_ID,
	ELEVATOR_ENTRANCE,
	ELEVATOR_EXIT,
	ELEVATOR_VEHICLE,
	ELEVATOR_START_X,
	ELEVATOR_START_Y,
	ELEVATOR_START_Z,
	ELEVATOR_ROT,
	ELEVATOR_DIM,
	ELEVATOR_NAME,
	ELEVATOR_LOCKED,
	BOOMBOX,
	BOOMBOX_RID,
	MOVE_CLIPSET,
	STRAFE_CLIPSET,
	WEAP_MODS,
	DETECTOR_ID,
	IS_DETECTOR,
	DETECTOR_LASTUSED,
	AME_MESSAGE,
	ADO_MESSAGE,
	STATUS_MESSAGE,
	MESSAGE_DRAWN,
	CLOTHING,
	DISAPPEAR,
	PET_NAME,
	PET_TYPE,
	NAMETAGS,
	FISHING,
	BACKUP,
	UNIT_NUMBER,
	DP_X,
	DP_Y,
	DP_RZ,
	MINUTES_PLAYED,
	QUICK_REPLY_PLAYER,
	SEATBELT,
	DUTY_POINT,
	DUTY_TYPE,
	MARIJUANA_GROWTH_STAGE,
	MARIJUANA_WATERED,
	MARIJUANA_FERTILIZED,
	MARIJUANA_TRIMMED,
	NEWS_MIC,
	NEWS_BOOM_MIC,
	NEWS_CAM_HAND,
	JOINED_TV,
	NEWS_CAM,
	BINOCULARS,
	BINOCULARS_TYPE,
	SMOKING,
	SMOKING_TYPE,
	CHARACTER_NAME, //THIS IS FOR THE PLAYER LIST
	INTERIOR_MANAGER,
	USERNAME, //THIS IS FOR THE PLAYER LIST
	NOTE_LOCKED,
	NOTE_MESSAGE,
	NOTE_OWNER_NAME,
	VEHICLE_WINDOWS,
	BADGE_ENABLED,
	BADGE_NAME,
	BADGE_FACTION_NAME,
	BADGE_COLOR_R,
	BADGE_COLOR_G,
	BADGE_COLOR_B,
	LOCALPLAYER_NAMETAG_TOGGLED,
	COINFLIP,
	VEHICLE_TRANSMISSION,
	MANUAL_VEHICLE_GEAR,
	MANUAL_VEHICLE_RPM,
	MANUAL_VEHICLE_BRAKELIGHTS,
	VEHICLE_HANDBRAKE,
	LOCKSMITH_PENDING_PICKUP,
	ITEM_CUSTOM_VALUE,
	ITEM_LOCKED,
	CAN_MOVE_OBJECTS,
	SCRIPTER_LEVEL,
	GENERIC_ROTATION,
	VEH_Z_FIX,
	BLEEDING,
	VEH_EXTRAS,
	IS_INFO_MARKER,
	INFO_MARKER_ID,
	TOWTRUCK_BEACON,
	PROP_OWNER_ID,
	PROP_RENTER_ID,
	PROP_OWNER_TYPE,
	PROP_RENTER_TYPE,
	PROP_XP,
	PROP_LAST_MOWED_AT,
	VEH_INVISIBLE,
}

public enum EPropertyOwnerType
{
	Player = 0,
	Faction
};

public enum EDecalTypes
{
	splatters_blood = 1010,
	splatters_blood_dir = 1015,
	splatters_blood_mist = 1017,
	splatters_mud = 1020,
	splatters_paint = 1030,
	splatters_water = 1040,
	splatters_water_hydrant = 1050,
	splatters_blood2 = 1110,
	weapImpact_metal = 4010,
	weapImpact_concrete = 4020,
	weapImpact_mattress = 4030,
	weapImpact_mud = 4032,
	weapImpact_wood = 4050,
	weapImpact_sand = 4053,
	weapImpact_cardboard = 4040,
	weapImpact_melee_glass = 4100,
	weapImpact_glass_blood = 4102,
	weapImpact_glass_blood2 = 4104,
	weapImpact_shotgun_paper = 4200,
	weapImpact_shotgun_mattress,
	weapImpact_shotgun_metal,
	weapImpact_shotgun_wood,
	weapImpact_shotgun_dirt,
	weapImpact_shotgun_tvscreen,
	weapImpact_shotgun_tvscreen2,
	weapImpact_shotgun_tvscreen3,
	weapImpact_melee_concrete = 4310,
	weapImpact_melee_wood = 4312,
	weapImpact_melee_metal = 4314,
	burn1 = 4421,
	burn2,
	burn3,
	burn4,
	burn5,
	bang_concrete_bang = 5000,
	bang_concrete_bang2,
	bang_bullet_bang,
	bang_bullet_bang2 = 5004,
	bang_glass = 5031,
	bang_glass2,
	solidPool_water = 9000,
	solidPool_blood,
	solidPool_oil,
	solidPool_petrol,
	solidPool_mud,
	porousPool_water,
	porousPool_blood,
	porousPool_oil,
	porousPool_petrol,
	porousPool_mud,
	porousPool_water_ped_drip,
	liquidTrail_water = 9050
}

public enum EAdminLevel
{
	None = 0,
	TrialAdmin = 1,
	Admin = 2,
	SeniorAdmin = 3,
	LeadAdmin = 4,
	HeadAdmin = 5
}

public enum EScripterLevel
{
	None = 0,
	Tester = 1,
	TrialScripter = 2,
	Scripter = 3
}

public enum EAchievementRarity
{
	Common,
	Rare,
	VeryRare
}

public enum EClipsetID
{
	None = 0,
	Crouch,
	CrouchStrafe,
	DrunkLow,
	DrunkMed,
	DrunkHigh,
	Injured,
	Injured2
}

public class BulkClothing
{
	public const int numComponents = 12;

	public BulkClothing()
	{

	}

	public BulkClothing(string strData)
	{
		int[] combinedData = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(strData);
		Array.Copy(combinedData, 0, arrDrawables, 0, numComponents);
		Array.Copy(combinedData, numComponents, arrTextures, 0, numComponents);
	}

	public void Set(int componentID, int drawable, int texture)
	{
		arrDrawables[componentID] = drawable;
		arrTextures[componentID] = texture;
	}

	public int GetComponent(int componentID)
	{
		if (componentID < arrDrawables.Length)
		{
			return arrDrawables[componentID];
		}

		return 0;
	}

	public int GetTexture(int componentID)
	{
		if (componentID < arrTextures.Length)
		{
			return arrTextures[componentID];
		}

		return 0;
	}

	public string Serialize()
	{
		int[] combinedData = new int[numComponents * 2];
		Array.Copy(arrDrawables, 0, combinedData, 0, numComponents);
		Array.Copy(arrTextures, 0, combinedData, numComponents, numComponents);
		return Newtonsoft.Json.JsonConvert.SerializeObject(combinedData);
	}

#if !SERVER
	public void Apply(RAGE.Elements.Player player)
	{
		bool bIsCustom = DataHelper.GetEntityData<bool>(player, EDataNames.IS_CUSTOM);

		for (int i = 0; i < numComponents; ++i)
		{
			if (i != 2 || !bIsCustom) // ignore hair if custom
			{
				player.SetComponentVariation(i, arrDrawables[i], arrTextures[i], 0);
			}
		}
	}
#endif

	int[] arrDrawables = new int[numComponents];
	int[] arrTextures = new int[numComponents];
}

public static class Helpers
{
	public static Int64 GetUnixTimestamp(bool toUTC = false)
	{
		DateTime now = DateTime.Now;

		if (toUTC)
		{
			now = now.ToUniversalTime();
		}

		return (Int64)now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public static string FormatString(string strFormat, params object[] strParams)
	{
		return String.Format(new System.Globalization.CultureInfo("en-US"), strFormat, strParams);
	}

	public static string ConvertSecondsToTimeString(int seconds)
	{
		if (seconds < 60)
		{
			return Helpers.FormatString("{0} seconds", seconds);
		}
		else if (seconds == 60)
		{
			return "1 minute";
		}
		else if (seconds > 60)
		{
			int minutes = seconds / 60;
			seconds = seconds % 60;
			return Helpers.FormatString("{0} {1} and {2} {3}", minutes, minutes > 1 ? "minutes" : "minute", seconds, seconds > 1 ? "seconds" : "second");
		}

		return String.Empty;
	}

	public static string ColorString(int r, int g, int b, string strFormat, params object[] strParams)
	{
		string strColored = "<font style = 'color: rgb(" + r.ToString() + "," + g.ToString() + "," + b.ToString() + "')>" + strFormat + "</font>";
		return String.Format(new System.Globalization.CultureInfo("en-US"), strColored, strParams);
	}

	public static bool AreColorsClose(System.Drawing.Color a, System.Drawing.Color z, int threshold = 50)
	{
		int r = (int)a.R - z.R,
			g = (int)a.G - z.G,
			b = (int)a.B - z.B;
		return (r * r + g * g + b * b) <= threshold * threshold;
	}

	public static string GetAdminLevelName(EAdminLevel adminLevel)
	{
		switch (adminLevel)
		{
			default:
			case EAdminLevel.None:
				return "Player";
			case EAdminLevel.TrialAdmin:
				return "Trial Admin";
			case EAdminLevel.Admin:
				return "Admin";
			case EAdminLevel.SeniorAdmin:
				return "Senior Admin";
			case EAdminLevel.LeadAdmin:
				return "Lead Admin";
			case EAdminLevel.HeadAdmin:
				return "Head Admin";
		}
	}

	public static string GetScripterLevelName(EScripterLevel scripterLevel)
	{
		switch (scripterLevel)
		{
			default:
			case EScripterLevel.None:
				return "Player";
			case EScripterLevel.Tester:
				return "Tester";
			case EScripterLevel.TrialScripter:
				return "Trial Scripter";
			case EScripterLevel.Scripter:
				return "Scripter";
		}
	}
}

/// NOTE: THESE IDS MUST MATCH help_center.html
public enum EAdminReportType
{
	General,
	PlayerGrievance,
	Properties,
	Vehicles,
	Other
}

// TODO_CSHARP: Why are there unknowns?
// TODO_LAUNCH: Add more weapons?
// TODO_LAUNCH: A bunch of these arent actually mapped to items...
public enum EWeapons
{
	WEAPON_UNARMED = -1,
	WEAPON_KNIFE,
	WEAPON_NIGHTSTICK,
	WEAPON_HAMMER,
	WEAPON_BAT,
	WEAPON_GOLFCLUB,
	WEAPON_CROWBAR,
	WEAPON_PISTOL,
	WEAPON_COMBATPISTOL,
	WEAPON_APPISTOL,
	WEAPON_PISTOL50,
	WEAPON_MICROSMG,
	WEAPON_SMG,
	WEAPON_ASSAULTSMG,
	WEAPON_ASSAULTRIFLE,
	WEAPON_CARBINERIFLE,
	WEAPON_ADVANCEDRIFLE,
	WEAPON_MG,
	WEAPON_COMBATMG,
	WEAPON_PUMPSHOTGUN,
	WEAPON_SAWNOFFSHOTGUN,
	WEAPON_ASSAULTSHOTGUN,
	WEAPON_BULLPUPSHOTGUN,
	WEAPON_STUNGUN,
	WEAPON_SNIPERRIFLE,
	WEAPON_HEAVYSNIPER,
	WEAPON_GRENADELAUNCHER,
	WEAPON_GRENADELAUNCHER_SMOKE,
	WEAPON_RPG,
	WEAPON_MINIGUN,
	WEAPON_GRENADE,
	WEAPON_STICKYBOMB,
	WEAPON_SMOKEGRENADE,
	WEAPON_BZGAS,
	WEAPON_MOLOTOV,
	WEAPON_FIREEXTINGUISHER,
	WEAPON_PETROLCAN,
	WEAPON_SNSPISTOL,
	WEAPON_SPECIALCARBINE,
	WEAPON_HEAVYPISTOL,
	WEAPON_BULLPUPRIFLE,
	WEAPON_HOMINGLAUNCHER,
	WEAPON_PROXMINE,
	WEAPON_SNOWBALL,
	WEAPON_VINTAGEPISTOL,
	WEAPON_DAGGER,
	WEAPON_FIREWORK,
	WEAPON_MUSKET,
	WEAPON_MARKSMANRIFLE,
	WEAPON_HEAVYSHOTGUN,
	WEAPON_GUSENBERG,
	WEAPON_HATCHET,
	WEAPON_RAILGUN,
	WEAPON_COMBATPDW,
	WEAPON_KNUCKLE,
	WEAPON_MARKSMANPISTOL,
	WEAPON_BOTTLE,
	WEAPON_FLAREGUN,
	WEAPON_FLARE,
	WEAPON_REVOLVER,
	WEAPON_SWITCHBLADE,
	WEAPON_MACHETE,
	WEAPON_FLASHLIGHT,
	WEAPON_MACHINEPISTOL,
	WEAPON_DBSHOTGUN,
	WEAPON_COMPACTRIFLE,
	WEAPON_BATTLEAXE,
	WEAPON_BALL,
	WEAPON_PARACHUTE,
	WEAPON_WRENCH,
	WEAPON_COMPACTLAUNCHER,
	WEAPON_MINISMG,
	WEAPON_AUTOSHOTGUN,
	WEAPON_UNKNOWN_MAYBE_BATTLEAXE,
	WEAPON_UNKNOWN_MAYBE_COMPACTLAUNCHER,
	WEAPON_UNKNOWN_MAYBE_MINISMG,
	WEAPON_PIPEBOMB,
	WEAPON_SWEEPER,
	WEAPON_UNKNOWN_MAYBE_WRENCH,
	WEAPON_CERAMICPISTOL,
	WEAPON_NAVYREVOLVER,
	WEAPON_HAZARDCAN,
	WEAPON_POOLCUE,
	WEAPON_STONE_HATCHET,
	WEAPON_Pistol_mk2,
	WEAPON_Snspistol_mk2,
	WEAPON_RAYPISTOL,
	WEAPON_Smg_mk2,
	WEAPON_RAYCARBINE,
	WEAPON_Pumpshotgun_mk2,
	WEAPON_Assaultrifle_mk2,
	WEAPON_Specialcarbine_mk2,
	WEAPON_Bullpuprifle_mk2,
	WEAPON_Combatmg_mk2,
	WEAPON_Heavysniper_mk2,
	WEAPON_Marksmanrifle_mk2,
	WEAPON_RAYMINIGUN,
	WEAPON_CARBINE_MK2,
	WEAPON_Revolver_mk2,
	DOUBLE_ACTION_REVOLVER,
	WEAPON_GADGETPISTOL,
	WEAPON_COMBATSHOTGUN,
	WEAPON_MILITARYRIFLE,
	WEAPON_MAX
};

public enum EWeaponCategory
{
	Melee,
	Handgun,
	SMG,
	Rifle,
	MachineGun,
	Shotgun,
	Sniper,
	RangedProjectile,
	Throwable,
	Special,
	None
}

public enum EPurchaseAndPaymentMethodsRequestType
{
	Vehicle,
	Property,
	Bank,
	RentalCar
}

public class Purchaser
{
	public string DisplayName { get; set; }
	public Int64 ID { get; set; }
	public EPurchaserType Type { get; set; }
}

public static class WeaponHelpers
{
	public static bool IsItemAWeaponAttachment(EItemID itemID)
	{
		return itemID == EItemID.HANDGUN_SUPPRESSOR
			|| itemID == EItemID.SMG_SUPPRESSOR
			|| itemID == EItemID.SHOTGUN_SUPPRESSOR
			|| itemID == EItemID.RIFLE_SUPPRESSOR
			|| itemID == EItemID.HANDGUN_FLASHLIGHT
			|| itemID == EItemID.SMG_FLASHLIGHT
			|| itemID == EItemID.SHOTGUN_FLASHLIGHT
			|| itemID == EItemID.HANDGUN_EXTENDED_MAG
			|| itemID == EItemID.SMG_EXTENDED_MAG
			|| itemID == EItemID.SHOTGUN_EXTENDED_MAG
			|| itemID == EItemID.RIFLE_EXTENDED_MAG
			|| itemID == EItemID.SMG_DRUM_MAGAZINE
			|| itemID == EItemID.SHOTGUN_DRUM_MAGAZINE
			|| itemID == EItemID.RIFLE_DRUM_MAGAZINE
			|| itemID == EItemID.HANDGUN_HOLOGRAPHIC_SIGHT
			|| itemID == EItemID.SMG_HOLOGRAPHIC_SIGHT
			|| itemID == EItemID.SHOTGUN_HOLOGRAPHIC_SIGHT
			|| itemID == EItemID.RIFLE_HOLOGRAPHIC_SIGHT
			|| itemID == EItemID.HANDGUN_COMPENSATOR
			|| itemID == EItemID.HANDGUN_SMALL_SCOPE
			|| itemID == EItemID.SMG_SMALL_SCOPE
			|| itemID == EItemID.SHOTGUN_SMALL_SCOPE
			|| itemID == EItemID.RIFLE_SMALL_SCOPE
			|| itemID == EItemID.RIFLE_LARGE_SCOPE
			|| itemID == EItemID.HANDGUN_MOUNTED_SCOPE
			|| itemID == EItemID.SMG_FOREGRIP
			|| itemID == EItemID.SHOTGUN_FOREGRIP
			|| itemID == EItemID.RIFLE_FOREGRIP
			|| itemID == EItemID.SMG_BASIC_SCOPE
			|| itemID == EItemID.RIFLE_BASIC_SCOPE
			|| itemID == EItemID.SMG_MEDIUM_SCOPE
			|| itemID == EItemID.SHOTGUN_MEDIUM_SCOPE
			|| itemID == EItemID.RIFLE_MEDIUM_SCOPE
			|| itemID == EItemID.RIFLE_ADVANCED_SCOPE
			|| itemID == EItemID.RIFLE_ZOOM_SCOPE
			|| itemID == EItemID.RIFLE_NIGHTVISION_SCOPE
			|| itemID == EItemID.RIFLE_THERMAL_SCOPE
			|| itemID == EItemID.SMG_HEAVY_BARREL
			|| itemID == EItemID.RIFLE_HEAVY_BARREL
			|| itemID == EItemID.SMG_FLAT_MUZZLE
			|| itemID == EItemID.SMG_TACTICAL_MUZZLE
			|| itemID == EItemID.SMG_FAT_END_MUZZLE
			|| itemID == EItemID.SMG_PRECISION_MUZZLE
			|| itemID == EItemID.SMG_HEAVY_DUTY_MUZZLE
			|| itemID == EItemID.SMG_SLANTED_MUZZLE
			|| itemID == EItemID.SMG_SPLIT_END_MUZZLE
			|| itemID == EItemID.SHOTGUN_SQUARED_MUZZLE
			|| itemID == EItemID.RIFLE_FLAG_MUZZLE
			|| itemID == EItemID.RIFLE_TACTICAL_MUZZLE
			|| itemID == EItemID.RIFLE_FAT_END_MUZZLE
			|| itemID == EItemID.RIFLE_PRECISION_MUZZLE
			|| itemID == EItemID.RIFLE_HEAVY_DUTY_MUZZLE
			|| itemID == EItemID.RIFLE_SLANTED_MUZZLE
			|| itemID == EItemID.RIFLE_SPLIT_END_MUZZLE
			|| itemID == EItemID.RIFLE_SQUARED_MUZZLE
			|| itemID == EItemID.RIFLE_BELL_END_MUZZLE
			|| itemID == EItemID.HANDGUN_TRACER_ROUNDS
			|| itemID == EItemID.HANDGUN_INCENDIARY_ROUNDS
			|| itemID == EItemID.HANDGUN_HOLLOW_ROUNDS
			|| itemID == EItemID.HANDGUN_FMJ_ROUNDS
			|| itemID == EItemID.SMG_TRACER_ROUNDS
			|| itemID == EItemID.SMG_INCENDIARY_ROUNDS
			|| itemID == EItemID.SMG_HOLLOW_ROUNDS
			|| itemID == EItemID.SMG_FMJ_ROUNDS
			|| itemID == EItemID.RIFLE_TRACER_ROUNDS
			|| itemID == EItemID.RIFLE_INCENDIARY_ROUNDS
			|| itemID == EItemID.RIFLE_AP_ROUNDS
			|| itemID == EItemID.RIFLE_FMJ_ROUNDS
			|| itemID == EItemID.RIFLE_EXPLOSIVE_ROUNDS
			|| itemID == EItemID.SHOTGUN_DRAGONSBREATH_SHELLS
			|| itemID == EItemID.SHOTGUN_STEEL_BUCKSHOT_SHELLS
			|| itemID == EItemID.SHOTGUN_FLECHETTE_SHELLS
			|| itemID == EItemID.SHOTGUN_EXPLOSIVE_SHELLS;
	}

	public static bool IsItemAWeapon(EItemID itemID)
	{
		return (itemID > EItemID.None && itemID <= EItemID.WEAPON_AUTOSHOTGUN)
			|| itemID == EItemID.WEAPON_CERAMICPISTOL
			|| itemID == EItemID.WEAPON_NAVYREVOLVER
			|| itemID == EItemID.WEAPON_HAZARDCAN
			|| itemID == EItemID.WEAPON_POOLCUE
			|| itemID == EItemID.WEAPON_STONE_HATCHET
			|| itemID == EItemID.WEAPON_PISTOL_MK2
			|| itemID == EItemID.WEAPON_SNSPISTOL_MK2
			|| itemID == EItemID.WEAPON_RAYPISTOL
			|| itemID == EItemID.WEAPON_SMG_MK2
			|| itemID == EItemID.WEAPON_RAYCARBINE
			|| itemID == EItemID.WEAPON_PUMPSHOTGUN_MK2
			|| itemID == EItemID.WEAPON_ASSAULTRIFLE_MK2
			|| itemID == EItemID.WEAPON_SPECIALCARBINE_MK2
			|| itemID == EItemID.WEAPON_BULLPUPRIFLE_MK2
			|| itemID == EItemID.WEAPON_COMBATMG_MK2
			|| itemID == EItemID.WEAPON_HEAVYSNIPER_MK2
			|| itemID == EItemID.WEAPON_MARKSMANRIFLE_MK2
			|| itemID == EItemID.WEAPON_RAYMINIGUN
			|| itemID == EItemID.WEAPON_CARBINERIFLE_MK2
			|| itemID == EItemID.WEAPON_HEAVYREVOLVER_MK2
			|| itemID == EItemID.DOUBLE_ACTION_REVOLVER
			|| itemID == EItemID.WEAPON_GADGETPISTOL
			|| itemID == EItemID.WEAPON_COMBATSHOTGUN
			|| itemID == EItemID.WEAPON_MILITARYRIFLE;
	}

	public static bool IsItemAmmo(EItemID itemID)
	{
		EItemID[] ammos = {
			EItemID.AMMO_HANDGUN,
			EItemID.AMMO_FLARE,
			EItemID.AMMO_RIFLE,
			EItemID.AMMO_ROCKET,
			EItemID.AMMO_SHOTGUN,
			EItemID.AMMO_TASER_PRODS,
			EItemID.AMMO_GRENADESHELL,
		};

		return ammos.Contains(itemID);
	}

	public static bool IsItemAFirearm(EItemID itemID)
	{
		return itemID == EItemID.WEAPON_PISTOL
			|| itemID == EItemID.WEAPON_COMBATPISTOL
			|| itemID == EItemID.WEAPON_APPISTOL
			|| itemID == EItemID.WEAPON_PISTOL50
			|| itemID == EItemID.WEAPON_MICROSMG
			|| itemID == EItemID.WEAPON_SMG
			|| itemID == EItemID.WEAPON_ASSAULTSMG
			|| itemID == EItemID.WEAPON_ASSAULTRIFLE
			|| itemID == EItemID.WEAPON_CARBINERIFLE
			|| itemID == EItemID.WEAPON_ADVANCEDRIFLE
			|| itemID == EItemID.WEAPON_MG
			|| itemID == EItemID.WEAPON_COMBATMG
			|| itemID == EItemID.WEAPON_PUMPSHOTGUN
			|| itemID == EItemID.WEAPON_SAWNOFFSHOTGUN
			|| itemID == EItemID.WEAPON_ASSAULTSHOTGUN
			|| itemID == EItemID.WEAPON_BULLPUPSHOTGUN
			|| itemID == EItemID.WEAPON_STUNGUN
			|| itemID == EItemID.WEAPON_SNIPERRIFLE
			|| itemID == EItemID.WEAPON_HEAVYSNIPER
			|| itemID == EItemID.WEAPON_GRENADELAUNCHER
			|| itemID == EItemID.WEAPON_GRENADELAUNCHER_SMOKE
			|| itemID == EItemID.WEAPON_RPG
			|| itemID == EItemID.WEAPON_MINIGUN
			|| itemID == EItemID.WEAPON_SNSPISTOL
			|| itemID == EItemID.WEAPON_SPECIALCARBINE
			|| itemID == EItemID.WEAPON_HEAVYPISTOL
			|| itemID == EItemID.WEAPON_BULLPUPRIFLE
			|| itemID == EItemID.WEAPON_HOMINGLAUNCHER
			|| itemID == EItemID.WEAPON_VINTAGEPISTOL
			|| itemID == EItemID.WEAPON_MUSKET
			|| itemID == EItemID.WEAPON_MARKSMANRIFLE
			|| itemID == EItemID.WEAPON_HEAVYSHOTGUN
			|| itemID == EItemID.WEAPON_GUSENBERG
			|| itemID == EItemID.WEAPON_HATCHET
			|| itemID == EItemID.WEAPON_RAILGUN
			|| itemID == EItemID.WEAPON_COMBATPDW
			|| itemID == EItemID.WEAPON_MARKSMANPISTOL
			|| itemID == EItemID.WEAPON_REVOLVER
			|| itemID == EItemID.WEAPON_MACHINEPISTOL
			|| itemID == EItemID.WEAPON_DBSHOTGUN
			|| itemID == EItemID.WEAPON_COMPACTRIFLE
			|| itemID == EItemID.WEAPON_COMPACTLAUNCHER
			|| itemID == EItemID.WEAPON_MINISMG
			|| itemID == EItemID.WEAPON_AUTOSHOTGUN
			|| itemID == EItemID.WEAPON_CERAMICPISTOL
			|| itemID == EItemID.WEAPON_NAVYREVOLVER
			|| itemID == EItemID.WEAPON_PISTOL_MK2
			|| itemID == EItemID.WEAPON_SNSPISTOL_MK2
			|| itemID == EItemID.WEAPON_RAYPISTOL
			|| itemID == EItemID.WEAPON_SMG_MK2
			|| itemID == EItemID.WEAPON_RAYCARBINE
			|| itemID == EItemID.WEAPON_PUMPSHOTGUN_MK2
			|| itemID == EItemID.WEAPON_ASSAULTRIFLE_MK2
			|| itemID == EItemID.WEAPON_SPECIALCARBINE_MK2
			|| itemID == EItemID.WEAPON_BULLPUPRIFLE_MK2
			|| itemID == EItemID.WEAPON_COMBATMG_MK2
			|| itemID == EItemID.WEAPON_HEAVYSNIPER_MK2
			|| itemID == EItemID.WEAPON_MARKSMANRIFLE_MK2
			|| itemID == EItemID.WEAPON_RAYMINIGUN
			|| itemID == EItemID.WEAPON_CARBINERIFLE_MK2
			|| itemID == EItemID.WEAPON_HEAVYREVOLVER_MK2
			|| itemID == EItemID.DOUBLE_ACTION_REVOLVER
			|| itemID == EItemID.WEAPON_GADGETPISTOL
			|| itemID == EItemID.WEAPON_COMBATSHOTGUN
			|| itemID == EItemID.WEAPON_MILITARYRIFLE;
	}

#if !SERVER
	public static Dictionary<WeaponHash, EWeaponCategory> WeaponCategories = new Dictionary<WeaponHash, EWeaponCategory>()
	{
		{ WeaponHash.Unarmed, EWeaponCategory.Melee },
		{ WeaponHash.Knife, EWeaponCategory.Melee },
		{ WeaponHash.Nightstick, EWeaponCategory.Melee },
		{ WeaponHash.Hammer, EWeaponCategory.Melee },
		{ WeaponHash.Bat, EWeaponCategory.Melee },
		{ WeaponHash.Golfclub, EWeaponCategory.Melee },
		{ WeaponHash.Crowbar, EWeaponCategory.Melee },
		{ WeaponHash.Pistol, EWeaponCategory.Handgun },
		{ WeaponHash.Combatpistol, EWeaponCategory.Handgun },
		{ WeaponHash.Appistol, EWeaponCategory.Handgun },
		{ WeaponHash.Pistol50, EWeaponCategory.Handgun },
		{ WeaponHash.Microsmg, EWeaponCategory.Handgun },
		{ WeaponHash.Smg, EWeaponCategory.SMG },
		{ WeaponHash.Assaultsmg, EWeaponCategory.SMG },
		{ WeaponHash.Assaultrifle, EWeaponCategory.Rifle },
		{ WeaponHash.Carbinerifle, EWeaponCategory.Rifle },
		{ WeaponHash.Advancedrifle, EWeaponCategory.Rifle },
		{ WeaponHash.Mg, EWeaponCategory.MachineGun },
		{ WeaponHash.Combatmg, EWeaponCategory.MachineGun },
		{ WeaponHash.Pumpshotgun, EWeaponCategory.Shotgun },
		{ WeaponHash.Sawnoffshotgun, EWeaponCategory.Shotgun },
		{ WeaponHash.Assaultshotgun, EWeaponCategory.Shotgun },
		{ WeaponHash.Bullpupshotgun, EWeaponCategory.Shotgun },
		{ WeaponHash.Stungun, EWeaponCategory.Handgun },
		{ WeaponHash.Sniperrifle, EWeaponCategory.Sniper },
		{ WeaponHash.Heavysniper, EWeaponCategory.Sniper },
		{ WeaponHash.Grenadelauncher, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Grenadelauncher_smoke, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Rpg, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Minigun, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Grenade, EWeaponCategory.Throwable },
		{ WeaponHash.Stickybomb, EWeaponCategory.Throwable },
		{ WeaponHash.Smokegrenade, EWeaponCategory.Throwable },
		{ WeaponHash.Bzgas, EWeaponCategory.Throwable },
		{ WeaponHash.Molotov, EWeaponCategory.Throwable },
		{ WeaponHash.Fireextinguisher, EWeaponCategory.Special },
		{ WeaponHash.Petrolcan, EWeaponCategory.Special },
		{ WeaponHash.Snspistol, EWeaponCategory.Handgun },
		{ WeaponHash.Specialcarbine, EWeaponCategory.Rifle },
		{ WeaponHash.Heavypistol, EWeaponCategory.Handgun },
		{ WeaponHash.Bullpuprifle, EWeaponCategory.Rifle },
		{ WeaponHash.Hominglauncher, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Proximine, EWeaponCategory.Throwable },
		{ WeaponHash.Snowball, EWeaponCategory.Throwable },
		{ WeaponHash.Vintagepistol, EWeaponCategory.Handgun },
		{ WeaponHash.Dagger, EWeaponCategory.Melee },
		{ WeaponHash.Firework, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Musket, EWeaponCategory.Shotgun },
		{ WeaponHash.Marksmanrifle, EWeaponCategory.Rifle },
		{ WeaponHash.Heavyshotgun, EWeaponCategory.Shotgun },
		{ WeaponHash.Gusenberg, EWeaponCategory.SMG },
		{ WeaponHash.Hatchet, EWeaponCategory.Melee },
		{ WeaponHash.Railgun, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Combatpdw, EWeaponCategory.SMG },
		{ WeaponHash.Knuckle, EWeaponCategory.Melee },
		{ WeaponHash.Marksmanpistol, EWeaponCategory.Handgun },
		{ WeaponHash.Bottle, EWeaponCategory.Melee },
		{ WeaponHash.Flaregun, EWeaponCategory.Handgun },
		{ WeaponHash.Flare, EWeaponCategory.Handgun },
		{ WeaponHash.Revolver, EWeaponCategory.Handgun },
		{ WeaponHash.Switchblade, EWeaponCategory.Melee },
		{ WeaponHash.Machete, EWeaponCategory.Melee },
		{ WeaponHash.Flashlight, EWeaponCategory.Melee },
		{ WeaponHash.Machinepistol, EWeaponCategory.SMG },
		{ WeaponHash.Dbshotgun, EWeaponCategory.Shotgun },
		{ WeaponHash.Compactrifle, EWeaponCategory.Rifle },
		{ WeaponHash.Battleaxe, EWeaponCategory.Melee },
		{ WeaponHash.Ball, EWeaponCategory.Melee },
		{ WeaponHash.Parachute, EWeaponCategory.Special },
		{ WeaponHash.Wrench, EWeaponCategory.Melee },
		{ WeaponHash.Compactlauncher, EWeaponCategory.RangedProjectile },
		{ WeaponHash.Minismg, EWeaponCategory.SMG },
		{ WeaponHash.Sweepershotgun, EWeaponCategory.Shotgun},
		//{ WeaponHash.WEAPON_UNKNOWN_MAYBE_COMPACTLAUNCHER, EWeaponCategory.RangedProjectile },
		//{ WeaponHash.WEAPON_UNKNOWN_MAYBE_MINISMG, EWeaponCategory.SMG },
		{ WeaponHash.Pipebomb, EWeaponCategory.Throwable },
		//{ WeaponHash.WEAPON_UNKNOWN_MAYBE_WRENCH, EWeaponCategory.Melee },
		{ WeaponHash.CeramicPistol, EWeaponCategory.Handgun },
		{ WeaponHash.NavyRevolver, EWeaponCategory.Handgun },
		{ WeaponHash.HazardCan, EWeaponCategory.Special },
		{ WeaponHash.Poolcue, EWeaponCategory.Melee },
		{ WeaponHash.Stone_hatchet, EWeaponCategory.Melee },
		{ WeaponHash.Pistol_mk2, EWeaponCategory.Handgun },
		{ WeaponHash.Snspistol_mk2, EWeaponCategory.Handgun },
		{ WeaponHash.Raypistol, EWeaponCategory.Handgun },
		{ WeaponHash.Smg_mk2, EWeaponCategory.SMG },
		{ WeaponHash.Raycarbine, EWeaponCategory.Rifle },
		{ WeaponHash.Pumpshotgun_mk2, EWeaponCategory.Shotgun },
		{ WeaponHash.Assaultrifle_mk2, EWeaponCategory.Rifle },
		{ WeaponHash.Specialcarbine_mk2, EWeaponCategory.Rifle },
		{ WeaponHash.Bullpuprifle_mk2, EWeaponCategory.Rifle },
		{ WeaponHash.Combatmg_mk2, EWeaponCategory.MachineGun },
		{ WeaponHash.Heavysniper_mk2, EWeaponCategory.Sniper },
		{ WeaponHash.Marksmanrifle_mk2, EWeaponCategory.Sniper },
		{ WeaponHash.Rayminigun, EWeaponCategory.Special },
		{ WeaponHash.Carbinerifle_mk2, EWeaponCategory.Rifle },
		{ WeaponHash.Revolver_mk2, EWeaponCategory.Handgun },
		{ WeaponHash.Doubleaction, EWeaponCategory.Handgun },
		{ WeaponHash.GadgetPistol, EWeaponCategory.Handgun },
		{ WeaponHash.CombatShotgun, EWeaponCategory.Shotgun },
		{ WeaponHash.MilitaryRifle, EWeaponCategory.Rifle }
	};
#endif
#if !SERVER
	// NOTE: Unused, use item ID instead so images are correct etc
	/*
	public static int GetWeaponIndexFromHash(uint targetWeaponHash)
	{
		int index = -1;
		for (EWeapons weaponID = EWeapons.WEAPON_UNARMED; weaponID < EWeapons.WEAPON_MAX; ++weaponID)
		{
			if (HashHelper.GetHashUnsigned(weaponID.ToString()) == targetWeaponHash)
			{
				return index;
			}

			++index;
		}

		return -2;
	}
	*/

	public static EWeaponCategory GetWeaponCategory(uint targetWeaponHash)
	{
		foreach (var kvPair in WeaponCategories)
		{
			if (kvPair.Key == (WeaponHash)targetWeaponHash)
			{
				return kvPair.Value;
			}
		}

		return EWeaponCategory.None;
	}

	public static int GetWeaponItemIDFromHash(uint targetWeaponHash)
	{
		if (targetWeaponHash == (uint)WeaponHash.Unarmed)
		{
			return -1;
		}

		foreach (var kvPair in ItemWeaponDefinitions.g_DictItemIDToWeaponHash)
		{
			if (kvPair.Value == (WeaponHash)targetWeaponHash)
			{
				return (int)kvPair.Key;
			}
		}

		return -2;
	}

	public static List<WeaponHash> GetAllWeapons()
	{
		List<WeaponHash> lstWeapons = new List<WeaponHash>();
		for (EWeapons weaponID = 0; weaponID < EWeapons.WEAPON_MAX; ++weaponID)
		{
			uint weaponHash = HashHelper.GetHashUnsigned(weaponID.ToString());
			if (weaponHash != 0)
			{
				if (RAGE.Elements.Player.LocalPlayer.HasGotWeapon(weaponHash, false))
				{
					lstWeapons.Add((WeaponHash)weaponHash);
				}
			}
		}

		return lstWeapons;
	}

	public static Dictionary<EWeapons, int> GetAllWeaponsAmmo()
	{
		Dictionary<EWeapons, int> dictAmmos = new Dictionary<EWeapons, int>();
		for (EWeapons weaponID = 0; weaponID < EWeapons.WEAPON_MAX; ++weaponID)
		{
			uint weaponHash = HashHelper.GetHashUnsigned(weaponID.ToString());
			if (weaponHash != 0)
			{
				dictAmmos[weaponID] = RAGE.Elements.Player.LocalPlayer.GetAmmoInWeapon(weaponHash);
			}
		}

		return dictAmmos;
	}
#endif
}

public class CommandHelpInfo
{
	public CommandHelpInfo(string strCmd, string strDescription, string strRequirements, string strSyntax)
	{
		Cmd = strCmd;
		Description = strDescription;
		Requirements = strRequirements;
		Syntax = strSyntax;
	}

	public string Cmd { get; set; }
	public string Description { get; set; }
	public string Requirements { get; set; }
	public string Syntax { get; set; }
}

public enum EDonationInactivityEntityType
{
	Property,
	Vehicle
}

public static class DonationConstants
{
	public const int GCCostAssetTransfer = 750;
	public const int GCCostPer7DaysOfInactivityProtection = 30;
	public const int InactivityProtectionMaxDays = 182;
	public const int InactivityProtectionIncrement = 7;
}

public enum ETutorialVersions
{
	None = -1,
	FirstRelease_Paleto = 0,
	MoveToLS = 1,
	FishingAndBoats = 2,
	Tattoos = 3,
	PlasticSurgeon = 4,
}

public static class TutorialConstants
{
	public const ETutorialVersions TutorialVersion = ETutorialVersions.PlasticSurgeon;

	public static Dictionary<ETutorialVersions, string> TutorialDescriptions = new Dictionary<ETutorialVersions, string>()
	{
		{ ETutorialVersions.None, "Entire Tutorial" },
		{ ETutorialVersions.FirstRelease_Paleto, "Entire Tutorial" },
		{ ETutorialVersions.MoveToLS, "Los Santos specifics" },
		{ ETutorialVersions.FishingAndBoats, "Fishing & Boats" },
		{ ETutorialVersions.Tattoos, "Tattoos" },
		{ ETutorialVersions.PlasticSurgeon, "Plastic Surgeon" },
	};

}

public class TutorialCheckResult
{
	public TutorialCheckResult(ETutorialCheckResult a_Result, ETutorialVersions a_Version)
	{
		Result = a_Result;
		Version = a_Version;
	}

	public ETutorialCheckResult Result { get; }
	public ETutorialVersions Version { get; }
}


public enum ETutorialCheckResult
{
	NotComplete,
	CompletedLatestVersion,
	CompletedOldVersion
}

// NOTE: If you modify this, don't ever remove anything unless you update the donation store to match, I recommend just adding _DISABLED
public enum EDonationEffect
{
	VehicleToken,
	PropertyToken,
	SmallDollarPaycheckIncrease,
	LargeDollarPaycheckIncrease,
	FreeGas,
	InstantDriversLicenseBike,
	InstantDriversLicenseCar,
	InstantDriversLicenseTruck,
	InstantFirearmsLicenseSmall_DISABLED,
	InstantFirearmsLicenseLarge_DISABLED,
	FreeCalls,
	DiscountCard20Percent,
	NoPropertyTax,
	NoPropertySalesTax_DISABLED,
	NoVehicleTax,
	NoVehicleSalesTax_DISABLED,
	NoStoreSalesTax,
	InactivityProtection_OneWeek_Dummy,
	Pet_Boar,
	Pet_Cat,
	Pet_Chickenhawk,
	Pet_Chop,
	Pet_Cormorant,
	Pet_Coyote,
	Pet_Crow,
	Pet_Hen,
	Pet_Husky,
	Pet_Pig,
	Pet_Pigeon,
	Pet_Poodle,
	Pet_Pug,
	Pet_Rabbit,
	Pet_Rat,
	Pet_Retriever,
	Pet_Rottweiler,
	Pet_Seagull,
	Pet_Shepherd,
	Pet_Westy,
	Pet_Panther,
	Free_Visit_Clothing_Store,
	Free_Visit_Barber,
	Free_Visit_Tattoo,
	Free_Visit_Plastic_Surgeon,
	TogPM,
	TogAds
}
public class DonationEntityListEntry
{
	public DonationEntityListEntry(EntityDatabaseID a_ID, string a_strDisplayName)
	{
		ID = a_ID;
		DisplayName = a_strDisplayName;
	}

	public EntityDatabaseID ID { get; set; }
	public string DisplayName { get; set; }
}

public class DonationPurchasable
{
	public DonationPurchasable(UInt32 a_ID, string a_Title, string a_Description, int a_Cost, EDonationType a_Type, bool a_Unique, int a_Duration, bool a_bActive, EDonationEffect a_DonationEffect)
	{
		ID = a_ID;
		Title = a_Title;
		Description = a_Description;
		Cost = a_Cost;
		Unique = a_Unique;
		Duration = a_Duration;
		m_Type = a_Type;
		Active = a_bActive;
		DonationEffect = a_DonationEffect;
	}

	public UInt32 ID { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public int Cost { get; set; }
	public EDonationType m_Type { get; set; }
	public EDonationEffect DonationEffect { get; set; }
	public bool Unique { get; set; }
	public int Duration { get; set; }
	public bool Active { get; set; }
}

public enum EDonationType
{
	Account,
	Character,
	Vehicle,
	Property
}

public class DonationInventoryItem
{
	public DonationInventoryItem(UInt32 a_ID, Int64 a_Character, Int64 a_TimeActivated, Int64 a_TimeExpire, UInt32 a_DonationID, Int64 a_VehicleID, Int64 a_PropertyID)
	{
		ID = a_ID;
		Character = a_Character;
		TimeActivated = a_TimeActivated;
		TimeExpire = a_TimeExpire;
		DonationID = a_DonationID;
		VehicleID = a_VehicleID;
		PropertyID = a_PropertyID;
	}

	public UInt32 ID { get; set; }
	public Int64 Character { get; set; }
	public Int64 TimeActivated { get; set; }
	public Int64 TimeExpire { get; set; }
	public UInt32 DonationID { get; set; }
	public Int64 VehicleID { get; set; }
	public Int64 PropertyID { get; set; }

	public bool IsActive()
	{
		return TimeActivated != 0;
	}
}

public class DonationInventoryItemTransmit
{
	public DonationInventoryItemTransmit(DonationInventoryItem a_InventoryItemSource, string strAppliedTo)
	{
		InventoryItemSource = a_InventoryItemSource;
		AppliedTo = strAppliedTo;
	}

	public DonationInventoryItem InventoryItemSource { get; set; }
	public string AppliedTo { get; set; }
}

public enum EStoreCheckoutResult
{
	FailedPartial = 0,
	CannotAfford = 1,
	Success = 2,
	NoHandgunLicense = 3,
	NoLonggunLicense = 4,
	NotInCriminalFaction = 5,
	NeedAnyFirearmLicenseForAmmo = 6,
	Failed_MaskForCustomChar,
}

public class CAnimationCommand
{
	public CAnimationCommand(string strCmd, string strDescription, EAnimCategory animCategory, bool hasArgument)
	{
		commandName = strCmd;
		description = strDescription;
		category = animCategory.ToString();
		HasArgument = hasArgument;
	}

	public string commandName { get; set; }
	public string description { get; set; }
	public string category { get; set; }
	public bool HasArgument { get; set; }
}

public static class VehicleConstants
{
	public static int NumExtras = 16;
}

// NOTE: Make sure you update LatestCharacterVersion in CharacterConstants
public enum ECharacterVersions
{
	None = -1,
	Beta_And_1_0_HoldingPattern = 0,
	Beta_And_1_0_GiveTattooToken = 1, // Old char creation, no tattoos etc, played on beta or <= 1.0 (April 2020)
	Beta_And_1_0_GiveBarberToken = 2, // Old char creation, no tattoos etc, played on beta or <= 1.0 (April 2020)
	Beta_And_1_0_GiveClothingStoreToken = 3, // Old char creation, no tattoos etc, played on beta or <= 1.0 (April 2020)
	Beta_And_1_0_GivePlasticSurgeon = 4, // Old char creation, no tattoos etc, played on beta or <= 1.0 (April 2020)
	VERSION_1_1_NEW_CHAR_CREATE = 5, // New char creation with tattoos etc, having character version before this, will give tokens on spawn to get free clothing change, haircut, tattoos and plastic surgeon (before incrementing version to current)
	ADDED_HAIR_TATTOOS = 6, // Added hair tattoos, characters created before this version get a free visit to the barber!
}

public static class CharacterConstants
{
	public static string MaskedDisplayName = "Masked Person";

	public static uint[] g_PremadeMaleSkins = new uint[] { 225514697, 2602752943, 2608926626, 3614493108, 4227433577, 939183526, 2802535058, 2426248831, 797459875, 1464257942, 3181518428, 3183167778, 2230970679, 1975732938, 1825562762, 2634057640, 3865252245, 678319271, 1182012905, 365775923, 1952555184, 2620240008, 3666413874, 3422293493, 712602007, 3499148112, 988062523, 2981205682, 2216405299, 4274948997, 1704428387, 3457361118, 2050158196, 3986688045, 1459905209, 3189787803, 3776618420, 2040438510, 1706635382, 3756278757, 1302784073, 1401530684, 666718676, 4055673113, 4248931856, 3408943538, 3990661997, 3170921201, 3367442045, 1906124788, 4011150407, 1625728984, 768005095, 648372919, 2577072326, 645279998, 1681385341, 2237544099, 3845001836, 1165307954, 3740245870, 3870061732, 3585757951, 1024089777, 1283141381, 2260598310, 941695432, 915948376, 3697041061, 2089096292, 2721800023, 1728056212, 3447159466, 1461287021, 1382414087, 4194109068, 2459507570, 2867128955, 188012277, 3614493108, 1841036427, 3881194279, 2781317046, 2560490906, 2884567044, 2539657518, 1767447799, 3027157846, 4024807398, 2363277399, 71501447, 327394568, 2362341647, 2739391114, 2925257274, 2831296918, 3687553076, 3404326357, 3253960934, 2756669323, 216536661, 2240226444, 788622594, 1198698306, 1012965715, 2745392175, 1191403201, 1482427218, 103106535, 466359675, 2727244247, 2058033618, 3898166818, 3969814300, 1863555924, 1531218220, 3812756443, 1153203121, 3254803008, 60192701, 3100414644, 4036845097, 4203395201, 1167549130, 1158606749, 1162230285, 949295643, 3046438339, 1918178165, 4222842058, 1129928304, 3077190415, 2372398717, 3284966005, 2023152276, 3779566603, 518814684, 2566514544, 2339419141, 3235579087, 4095687067, 1798879480, 1635617250, 793443893, 1299047806, 4027271643, 3263172030, 2240582840, 569740212, 4132362192, 776079908, 2858686092, 1179785778, 3230888450, 4140949582, 2766184958, 2302502917, 2288257085, 1397974313, 1545995274, 978452933, 1776856003, 2349847778, 1224690857, 3529955798, 819699067, 3937184496, 4037813798, 4042020578, 3479321132, 3658575486, 3272005365, 1380197501, 3300333010, 1984382277, 3361671816, 848542878, 2340239206, 610290475, 3237179831, 1914945105, 1897303236, 3455927962, 1074385436, 2625926338, 755956971, 1943971979, 1943971979, 1161072059, 983887149, 2913175640, 866411749, 874722259, 2243544680, 728636342, 1189322339, 3691903615, 325317957, 1169888870, 3293887675, 3333724719, 3005388626, 2579169528, 4030826507, 3459037009, 2899099062, 3872144604, 3367706194, 479578891, 3778572496, 1293671805, 1191548746, 1822283721, 1346941736, 921110016, 2180468199, 3306347811, 3465937675, 1624626906, 416176080, 2886641112, 2585681490, 1787764635, 3299219389, 2423691919, 1596374223, 2648833641, 3513928062, 2651349821, 233415434, 4058522530, 4255728232, 3310258058, 1657546978, 68070371, 4033578141, 1752208920, 4096714883, 588969535, 599294057, 3852538118, 189425762, 1077785853, 2217202584, 3523131524, 2021631368, 600300561, 3886638041, 2114544056, 3394697810, 1423699487, 1982350912, 1068876755, 1720428295, 2681481517, 933205398, 3640249671, 2597531625, 3382649284, 3014915558, 2705543429, 2912874939, 3387290987, 261586155, 3118269184, 275618457, 2119136831, 4285659174, 436345731, 4257633223, 3835149295, 2606068340, 4282288299, 579932932, 766375082, 4188468543, 2756120947, 131961260, 377976310, 2010389054, 2860711835, 1097048408, 3896218551, 3681718840, 2217749257, 2488675799, 1641152947, 2841034142, 3519864886, 2775713665, 115168927, 330231874, 2908022696, 2850754114, 3609190705, 815693290, 1099825042, 1809430156, 3782053633, 4049719826, 691061163, 1358380044, 1822107721, 2064532783, 2097407511, 587703123, 349505262, 1312913862, 3721046572, 706935758, 2842417644, 767028979, 2445950508, 891945583, 611648169, 2414729609, 2093736314, 3512565361, 355916122, 452351020, 696250687, 2659242702, 321657486, 3724572669, 3684436375, 1330042375, 1032073858, 850468060, 803106487, 2124742566, 1768677545, 1466037421, 1226102803, 3716251309, 3185399110, 653210662, 832784782, 2521633500, 2992445106, 810804565, 3977045190, 1021093698, 1694362237, 2007797722, 3630066984, 1264920838, 3374523516, 1746653202, 3972697109, 1209091352, 1329576454, 2733138262, 2849617566, 2206530719, 2534589327, 4116817094, 3227390873, 623927022, 2218630415, 1001210244, 1328415626, 539004493, 3613420592, 1626646295, 2995538501, 2521108919, 2422005962, 663522487, 846439045, 62440720, 1794381917, 1846684678, 3654768780, 3250873975, 2952446692, 32417469, 193817059, 1750583735, 718836251, 3877027275, 2674735073, 1082572151, 2896414922, 2346291386, 238213328, 3287349092, 3271294718, 2318861297, 3454621138, 2442448387, 3482496489, 2563194959, 2255803900, 3265820418, 2035992488, 469792763, 4246489531, 228715206, 3465614249, 2457805603, 605602864, 919005580, 3072929548, 3938633710, 2494442380, 1416254276, 1347814329, 3365863812, 1224306523, 516505552, 390939205, 2359345766, 1404403376, 3773208948, 4144940484, 1498487404, 3389018345, 1520708641, 999748158, 3247667175, 1264851357, 1561705728, 534725268, 835315305, 2907468364, 1426951581, 1142162924, 3189832196, 2869588309 };
	public static uint[] g_PremadeFemaleSkins = new uint[] { 1074457665, 1567728751, 2014052797, 1250841910, 3349113128, 1146800212, 2129936603, 1830688247, 1546450936, 549978415, 920595805, 664399832, 826475330, 2928082356, 3083210802, 4121954205, 70821038, 1371553700, 1755064960, 1777626099, 373000027, 1165780219, 331645324, 793439294, 2111372120, 813893651, 343259175, 2185745201, 2549481101, 2780469782, 429425116, 42647445, 348382215, 51789996, 153984193, 3675473203, 3767780806, 3579522037, 587253782, 3343476521, 1064866854, 3680420864, 2923947184, 2842568196, 1055701597, 1767892582, 744758650, 1519319503, 2276611093, 1381498905, 1846523796, 1544875514, 824925120, 2231547570, 1426880966, 2633130371, 1446741360, 2435054400, 435429221, 3669401835, 933092024, 4209271110, 3290105390, 1633872967, 2181772221, 225287241, 257763003, 1530648845, 4242313482, 411185872, 2741999622, 1005070462, 2936266209, 946007720, 503621995, 3726105915, 3312325004, 357551935, 4293277303, 3885222120, 226559113, 3402126148, 3728026165, 2306246977, 2515474659, 117698822, 650367097, 2193587873, 3973074921, 1870669624, 3045926185, 261428209, 3272931111, 808778210, 1145088004, 1477887514, 1464721716, 161007533, 3166991819, 1167167044, 1270514905, 3422397391, 1334976110, 1325314544, 1528799427, 3750433537, 2346790124, 2934601397, 2168724337, 1123963760, 1665391897, 101298480, 4198014287, 3064628686, 431423238, 2526768638, 4040474158, 1095737979, 1573528872, 894928436, 602513566, 3538133636, 2718472679, 1535236204, 695248020, 361513884, 808859815, 3188223741, 2688103263, 1004114196, 532905404, 1699403886, 2638072698, 1674107025, 1309468115, 4206136267, 3050275044, 951767867, 640504453, 3134700416, 1388848350, 1204772502, 1090617681, 4250220510, 379310561, 3725461865, 3439295882, 2962707003, 1951946145, 1039800368, 4079145784, 2775443222, 1640504453 };

	public const uint CustomMaleSkin = 1885233650;
	public const uint CustomFemaleSkin = 2627665880;

	// NOTE: removed 46 seemed buggy
	public static int[] g_CustomFaces_Male = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 42, 43, 44 };
	public static int[] g_CustomFaces_Female = new int[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 45 };

	public static float StartingMoney = 1000.0f;
	public static float StartingBankMoney = 5000.0f;

	public static uint MinAge = 16;
	public static uint MaxAge = 95;

	public static uint MaxTattoos = 25;

	public static Vector3 SpawnPosition_Paleto = new Vector3(-130.559f, 6325.08f, 31.5339f);
	public static float SpawnRotation_Paleto = 133.91f;
	public static Vector3 SpawnPosition_LS = new Vector3(1129.9227f, -642.2933f, 56.743473f);
	public static float SpawnRotation_LS = -158.41095f;

	public static ECharacterVersions LatestCharacterVersion = ECharacterVersions.ADDED_HAIR_TATTOOS;

	public static uint MaxHairStyles_Male = 121;
	public static uint MaxHairStyles_Female = 158;
}

public enum EAudioIDs
{
	MenuMusic,
	Uganda,
	Mexico,
	Country,
	Christmas,
	Seatbelt_Buckle,
	Seatbelt_Unbuckle,
	FourthOfJulyCountdown,
	Handbrake_Up,
	Handbrake_Down,
	Halloween
}

public class ClientsideException
{
	public ClientsideException(string strMessage, string strCallStack)
	{
		Message = strMessage;
		CallStack = strCallStack;
	}

	public string Message { get; set; }
	public string CallStack { get; set; }
}

public static class ExceptionConstants
{
	public const int CooldownPeriod = 10000;
}

public static class ItemConstants
{
	public const float g_fDistVehicleTrunkThresholdSmall = 2.0f;
	public const float g_fDistVehicleTrunkThresholdLarge = 6.0f;
	public const float g_fDistFurnitureStorageThreshold = 2.0f;
}

public class TransmitAnimation
{
	public TransmitAnimation(string strAnimDict, string strName, int AnimFlags)
	{
		Dict = strAnimDict;
		Name = strName;
		Flags = AnimFlags;
	}

	public string AsJSON()
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(this);
	}

	public static TransmitAnimation FromJSON(string strJSON)
	{
		return Newtonsoft.Json.JsonConvert.DeserializeObject<TransmitAnimation>(strJSON);
	}

	public string Dict { get; set; }
	public string Name { get; set; }
	public int Flags { get; set; }
}



public enum ECustomClothingComponent
{
	Masks = 1,
	HairStyles = 2,
	Torsos = 3,
	Legs = 4,
	BagsAndParachutes = 5,
	Shoes = 6,
	Accessories = 7,
	Undershirts = 8,
	BodyArmor = 9,
	Decals = 10,
	Tops = 11
}

public enum ECharacterSource
{
	CreatedOnOwl = 0
}

public enum ECustomPropSlot
{
	Hats = 0,
	Glasses = 1,
	Ears = 2,
	Watches = 6,
	Bracelets = 7
}

public enum EDutyWeaponSlot
{
	PursuitAccessory,
	Melee,
	Accessory1,
	Accessory2,
	Accessory3,
	HandgunHipHolster,
	HandgunLegHolster,
	LargeWeapon,
	Projectile,
	Projectile2,
	LargeCarriedItem
}

public enum EDutyOutfitType
{
	Custom,
	Premade
}

// NOTE: Only add at end, don't change indices of earlier elements
public enum EScriptControlID
{
	EnterVehicleDriver,
	EnterVehiclePassenger,
	ToggleCursor,
	HideCursor,
	ToggleStatistics,
	ShowFullScreenBrowser,
	CloseFullScreenBrowser,
	GetPosition,
	ShowPlayerList,
	ShowWeaponSelector_Melee,
	ShowWeaponSelector_Handguns,
	ShowWeaponSelector_SMG,
	ShowWeaponSelector_Rifle,
	ShowWeaponSelector_MachineGun,
	ShowWeaponSelector_Shotgun,
	ShowWeaponSelector_Sniper,
	ShowWeaponSelector_RangedProjectile,
	ShowWeaponSelector_Throwable,
	ShowWeaponSelector_Special,
	CancelAnimation,
	ShowChatInput,
	ShowChatInput_LocalOOC,
	HideChatInput,
	SubmitChatMessage,
	ChatScrollUp,
	ChatScrollDown,
	ChatScrollHistoryUp,
	ChatScrollHistoryDown,
	ToggleChatVisibility,
	ToggleEngine,
	ToggleLock,
	ToggleHeadlights,
	LeftTurnSignal,
	RightTurnSignal,
	ToggleHelpCenter,
	ToggleFactionUI,
	DropWaterFromFireHeli,
	TogglePoliceSearchlight,
	ToggleSirensMode,
	ToggleInventory,
	ToggleDonations,
	Interact,
	ShowChatInput_PrimaryRadio,
	ToggleCruiseControl,
	ToggleCrouch,
	BlipSiren,
	TakeScreenshot,
	TogglePhone,
	ChangeMinimapMode,
	ChatScrollToStart,
	ChatScrollToEnd,
	ToggleBinocularView,
	ToggleBinocularFx,
	ToggleWindows,
	ToggleHandbrake,
	ToggleFireMode,
	ToggleTrainDoors,
	TrainAccelerate,
	TrainDecelerate,
	DummyNone, // NOTE: This must be the last one in the list, for UI index reasons.
}

public enum EWeaponLicenseType
{
	Tier1,
	Tier2
}

public class GameControlObject
{
	public GameControlObject(EScriptControlID a_ControlID, ConsoleKey a_Key)
	{
		C = a_ControlID;
		K = a_Key;
	}

	public EScriptControlID C { get; set; }
	public ConsoleKey K { get; set; }
}

public enum EPlayerKeyBindType
{
	Character,
	Account
}

public class PlayerKeybindObject
{
	public PlayerKeybindObject(EntityDatabaseID a_ID, ConsoleKey a_Key, EPlayerKeyBindType a_KeybindType, string a_Action)
	{
		ID = a_ID;
		KeybindType = a_KeybindType;
		Action = a_Action;
		Key = a_Key;
	}

	public EntityDatabaseID ID { get; set; }
	public EPlayerKeyBindType KeybindType { get; set; }
	public string Action { get; set; }
	public ConsoleKey Key { get; set; }
}

public enum ESpeechID
{
	ANGRY_WITH_PLAYER_TREVOR = 0,
	ARREST_PLAYER = 1,
	BLOCKED_GENERIC = 2,
	BLOCKED_IN_PURSUIT = 3,
	BUDDY_DOWN = 4,
	BUMP = 5,
	CHASE_VEHICLE_MEGAPHONE = 6,
	CHAT_RESP = 7,
	CHAT_STATE = 8,
	CLEAR_AREA_MEGAPHONE = 9,
	CLEAR_AREA_PANIC_MEGAPHONE = 10,
	COMBAT_TAUNT = 11,
	COP_PEEK = 12,
	COVER_ME = 13,
	COVER_YOU = 14,
	CRASH_GENERIC = 15,
	CRIMINAL_APPREHEND = 16,
	CRIMINAL_GET_IN_CAR = 17,
	CRIMINAL_WARNING = 18,
	DODGE = 19,
	DRAW_GUN = 20,
	DUCK = 21,
	EXPLOSION_IS_IMMINENT = 22,
	FALL_BACK = 23,
	FIGHT = 24,
	FOOT_CHASE = 25,
	FOOT_CHASE_HEADING_EAST = 26,
	FOOT_CHASE_HEADING_NORTH = 27,
	FOOT_CHASE_HEADING_SOUTH = 28,
	FOOT_CHASE_HEADING_WEST = 29,
	FOOT_CHASE_LOSING = 30,
	FOOT_CHASE_RESPONSE = 31,
	GENERIC_BYE = 32,
	GENERIC_CURSE_HIGH = 33,
	GENERIC_CURSE_MED = 34,
	GENERIC_FRIGHTENED_HIGH = 35,
	GENERIC_FRIGHTENED_MED = 36,
	GENERIC_HI = 37,
	GENERIC_INSULT_HIGH = 38,
	GENERIC_INSULT_MED = 39,
	GENERIC_SHOCKED_HIGH = 40,
	GENERIC_SHOCKED_MED = 41,
	GENERIC_THANKS = 42,
	GENERIC_WAR_CRY = 43,
	GENERIC_WHATEVER = 44,
	GET_HIM = 45,
	GUN_COOL = 46,
	JACK_VEHICLE_BACK = 47,
	JACKED_GENERIC = 48,
	KIFFLOM_GREET = 49,
	MOVE_IN = 50,
	MOVE_IN_PERSONAL = 51,
	NEED_SOME_HELP = 52,
	NO_LOITERING_MEGAPHONE = 53,
	PINNED_DOWN = 54,
	PROVOKE_TRESPASS = 55,
	RELOADING = 56,
	REQUEST_BACKUP = 57,
	REQUEST_NOOSE = 58,
	RESCUE_INJURED_COP = 59,
	SETTLE_DOWN = 60,
	SHOOTOUT_OPEN_FIRE = 61,
	SHOOTOUT_READY = 62,
	SHOOTOUT_READY_RESP = 63,
	STOP_ON_FOOT_MEGAPHONE = 64,
	STOP_VEHICLE_BOAT_MEGAPHONE = 65,
	STOP_VEHICLE_CAR_MEGAPHONE = 66,
	STOP_VEHICLE_CAR_WARNING_MEGAPHONE = 67,
	STOP_VEHICLE_GENERIC_MEGAPHONE = 68,
	STOP_VEHICLE_GENERIC_WARNING_MEGAPHONE = 69,
	SURROUNDED = 70,
	SUSPECT_KILLED = 71,
	SUSPECT_LOST = 72,
	SUSPECT_SPOTTED = 73,
	TAKE_COVER = 74,
	TRAPPED = 75,
	WAIT = 76
}

public enum ESpeechType
{
	SPEECH_PARAMS_NORMAL = 0,
	SPEECH_PARAMS_SHOUTED = 1,
	SPEECH_PARAMS_MEGAPHONE = 2
}

public class AdminCheckDetails
{
	public AdminCheckDetails(string a_Username, string a_CharacterName, string a_IpAddress, int a_GameCoins, List<CFactionTransmit> a_Factions, uint a_HoursPlayed_Account, uint a_HoursPlayed_Character, int a_NumPunishmentPointsActive, int a_NumPunishmentPointsLifetime, string a_AdminNotes, List<string> a_AdminHistory)
	{
		Username = a_Username;
		CharacterName = a_CharacterName;
		IpAddress = a_IpAddress;
		GameCoins = a_GameCoins;
		Factions = a_Factions;
		HoursPlayed_Account = a_HoursPlayed_Account;
		HoursPlayed_Character = a_HoursPlayed_Character;
		NumPunishmentPointsActive = a_NumPunishmentPointsActive;
		NumPunishmentPointsLifetime = a_NumPunishmentPointsLifetime;
		AdminNotes = a_AdminNotes;
		AdminHistory = a_AdminHistory;
	}

	public string Username { get; set; }
	public string CharacterName { get; set; }
	public string IpAddress { get; set; }
	public int GameCoins { get; set; }
	public List<CFactionTransmit> Factions { get; set; }
	public uint HoursPlayed_Account { get; set; }
	public uint HoursPlayed_Character { get; set; }
	public int NumPunishmentPointsActive { get; set; }
	public int NumPunishmentPointsLifetime { get; set; }
	public string AdminNotes { get; set; }
	public List<string> AdminHistory { get; set; }
}

public class CVehicleAction
{
	public CVehicleAction(string a_strDate, EntityDatabaseID a_VehicleID, string a_Action, string a_Actor)
	{
		date = a_strDate;
		vehicleID = a_VehicleID;
		action = a_Action;
		actor = a_Actor;
	}
	public string date { get; set; }
	public EntityDatabaseID vehicleID { get; set; }
	public string action { get; set; }
	public string actor { get; set; }
}

public class CAdminVehicleNote
{
	public CAdminVehicleNote(EntityDatabaseID a_VehicleID, string a_Creator, string a_Note, string a_strDate)
	{
		vehicleID = a_VehicleID;
		creator = a_Creator;
		note = a_Note;
		date = a_strDate;
	}

	public EntityDatabaseID vehicleID { get; set; }
	public string creator { get; set; }
	public string note { get; set; }
	public string date { get; set; }
}

public class CAdminPropertyNote
{
	public CAdminPropertyNote(EntityDatabaseID a_PropertyID, string a_Creator, string a_Note, string a_strDate)
	{
		propertyID = a_PropertyID;
		creator = a_Creator;
		note = a_Note;
		date = a_strDate;
	}

	public EntityDatabaseID propertyID { get; set; }
	public string creator { get; set; }
	public string note { get; set; }
	public string date { get; set; }
}

public class CPropertyAction
{
	public CPropertyAction(string a_strDate, EntityDatabaseID a_PropertyID, string a_Action, string a_Actor)
	{
		date = a_strDate;
		vehicleID = a_PropertyID;
		action = a_Action;
		actor = a_Actor;
	}
	public string date { get; set; }
	public EntityDatabaseID vehicleID { get; set; }
	public string action { get; set; }
	public string actor { get; set; }
}

public class AdminCheckVehicleDetails
{
	public AdminCheckVehicleDetails(string a_Username, string a_CharacterName, float a_StorePrice, uint a_ModelHash, string a_PaymentMethod, int a_PaymentMade, int a_PaymentMissed, int a_PaymentRemaining, float a_CreditAmount, string a_Faction, bool a_Stolen, List<CVehicleAction> a_Actions, List<CAdminVehicleNote> a_Notes)
	{
		Username = a_Username;
		CharacterName = a_CharacterName;
		StorePrice = a_StorePrice;
		ModelHash = a_ModelHash;
		Model = string.Empty;
		PaymentMethod = a_PaymentMethod;
		PaymentsMade = a_PaymentMade;
		PaymentsMissed = a_PaymentMissed;
		PaymentsRemaining = a_PaymentRemaining;
		CreditAmount = a_CreditAmount;
		Faction = a_Faction;
		Stolen = a_Stolen;
		Actions = a_Actions;
		Notes = a_Notes;
	}

	public string Username { get; set; }
	public string CharacterName { get; set; }
	public float StorePrice { get; set; }
	public uint ModelHash { get; set; }
	public string Model { get; set; }
	public string PaymentMethod { get; set; }
	public int PaymentsMade { get; set; }
	public int PaymentsMissed { get; set; }
	public int PaymentsRemaining { get; set; }
	public float CreditAmount { get; set; }
	public string Faction { get; set; }
	public bool Stolen { get; set; }
	public List<CVehicleAction> Actions { get; set; }
	public List<CAdminVehicleNote> Notes { get; set; }
}

public class AdminCheckInteriorDetails
{
	public AdminCheckInteriorDetails(string a_Username, string a_CharacterName, string buy_price, string rent_price, bool locked, string propertyName, int interiorId, string a_PaymentMethod, int a_PaymentMade, int a_PaymentMissed, int a_PaymentRemaining, float a_CreditAmount, string a_State, string last_used, List<CPropertyAction> a_Actions, List<CAdminPropertyNote> a_Notes)
	{
		Username = a_Username;
		CharacterName = a_CharacterName;
		BoughtFor = buy_price;
		RentFor = rent_price;
		IsLocked = locked;
		PropertyName = propertyName;
		InteriorID = interiorId;
		PaymentMethod = a_PaymentMethod;
		PaymentsMade = a_PaymentMade;
		PaymentsMissed = a_PaymentMissed;
		PaymentsRemaining = a_PaymentRemaining;
		CreditAmount = a_CreditAmount;
		State = a_State;
		lastUsed = last_used;
		Actions = a_Actions;
		Notes = a_Notes;
	}

	public string Username { get; set; }
	public string CharacterName { get; set; }
	public string BoughtFor { get; set; }
	public string RentFor { get; set; }
	public bool IsLocked { get; set; }
	public string PropertyName { get; set; }
	public int InteriorID { get; set; }
	public string PaymentMethod { get; set; }
	public int PaymentsMade { get; set; }
	public int PaymentsMissed { get; set; }
	public int PaymentsRemaining { get; set; }
	public float CreditAmount { get; set; }
	public string State { get; set; }
	public string lastUsed { get; set; }
	public List<CPropertyAction> Actions { get; set; }
	public List<CAdminPropertyNote> Notes { get; set; }
}

public static class EncryptionHelper
{
	public static string EncryptStringToString(string strInput, byte[] key)
	{
		return Convert.ToBase64String(EncryptBytesToBytes(System.Text.Encoding.ASCII.GetBytes(strInput), key));
	}

	public static string EncryptBytesToString(byte[] data, byte[] key)
	{
		return Convert.ToBase64String(EncryptBytesToBytes(data, key));
	}

	public static byte[] EncryptStringToBytes(string strInput, byte[] key)
	{
		return EncryptBytesToBytes(System.Text.Encoding.ASCII.GetBytes(strInput), key);
	}

	public static byte[] EncryptBytesToBytes(byte[] data, byte[] key)
	{
		byte[] encryptedBytes = new byte[data.Length + CRC32.CRCSize];
		for (int i = 0; i < data.Length; i++)
		{
			for (int k = 0; k < key.Length; ++k)
			{
				encryptedBytes[i + CRC32.CRCSize] = (byte)(data[i] ^ key[k]);
			}
		}

		// put crc32 of the raw data at start for checks later
		byte[] crc32 = CRC32.ComputeHashAsBytes(data);
		Array.Copy(crc32, encryptedBytes, CRC32.CRCSize);

		return encryptedBytes;
	}

	public static bool DecryptString(string strInput, byte[] key, out string strOutput)
	{
		bool bResult = DecryptBytesToBytes(Convert.FromBase64String(strInput), key, out byte[] bytesOutput);
		strOutput = System.Text.Encoding.ASCII.GetString(bytesOutput);
		return bResult;
	}

	public static bool DecryptBytesToString(byte[] data, byte[] key, out string strOutput)
	{
		bool bResult = DecryptBytesToBytes(data, key, out byte[] bytesOutput);
		strOutput = System.Text.Encoding.ASCII.GetString(bytesOutput);
		return bResult;
	}

	public static bool DecryptStringToBytes(string strInput, byte[] key, out byte[] bytesOutput)
	{
		bool bResult = DecryptBytesToBytes(Convert.FromBase64String(strInput), key, out bytesOutput);
		return bResult;
	}

	public static bool DecryptBytesToBytes(byte[] data, byte[] key, out byte[] bytesOutput)
	{
		// grab the raw crc32
		byte[] crc32 = new byte[CRC32.CRCSize];
		Array.Copy(data, crc32, CRC32.CRCSize);

		bytesOutput = new byte[data.Length - CRC32.CRCSize];

		for (int i = CRC32.CRCSize; i < data.Length; i++)
		{
			for (int k = 0; k < key.Length; ++k)
			{
				bytesOutput[i - CRC32.CRCSize] = (byte)(data[i] ^ key[k]);
			}
		}

		return crc32.SequenceEqual(CRC32.ComputeHashAsBytes(bytesOutput));
	}
}

public enum EModSlot
{
	Neons = -3,
	CustomizePlateStyle = -2,
	CustomizePlateText = -1,
	Spoilers = 0,
	FrontBumper = 1,
	RearBumper = 2,
	SideSkirt = 3,
	Exhaust = 4,
	Frame = 5,
	Grille = 6,
	Hood = 7,
	Fender = 8,
	RightFender = 9,
	Roof = 10,
	Engine = 11,
	Brakes = 12,
	Transmission = 13,
	Horns = 14,
	Suspension = 15,
	Armor = 16,
	Turbo = 18,
	Xenon = 22,
	FrontWheels = 23,
	BackWheels = 24,
	Plateholders = 25,
	TrimDesign = 27,
	Ornaments = 28,
	DialDesign = 30,
	SteeringWheel = 33,
	ShiftLever = 34,
	Plaques = 35,
	Hydraulics = 38,
	Livery = 48,
	Plate = 62,
	Colour1 = 66,
	Colour2 = 67,
	WindowTint = 69,
	DashboardColor = 74,
	TrimColor = 75,
}

public enum EVehicleModShopCheckoutResult
{
	Success = 0,
	CannotAfford = 1,
	CannotAffordGC = 2,
	PlateNotUnique = 3,
	PlateNotValid = 4,
}

public static class VehicleModHelpers
{
	public static Vector3 VecModShopCarPosition = new Vector3(197.8153f, -1002.293f, -99.35749f);
	public static float fModShopCarRotation = 201.2445f;

	private static Dictionary<EModSlot, float> m_dictModSlotPrices = new Dictionary<EModSlot, float>
	{
		{ EModSlot.Neons, 5000.0f },
		{ EModSlot.CustomizePlateStyle, 200.0f }, // GC
		{ EModSlot.CustomizePlateText, 200.0f }, // GC
		{ EModSlot.Spoilers, 2500.0f },
		{ EModSlot.FrontBumper, 1250.0f },
		{ EModSlot.RearBumper, 1250.0f },
		{ EModSlot.SideSkirt, 900.0f },
		{ EModSlot.Exhaust, 1500.0f },
		{ EModSlot.Frame, 1250.0f },
		{ EModSlot.Grille, 2500.0f },
		{ EModSlot.Hood, 2000.0f },
		{ EModSlot.Fender, 900.0f },
		{ EModSlot.RightFender, 900.0f },
		{ EModSlot.Roof, 1250.0f },
		{ EModSlot.Engine, 1500.0f }, // Scales
		{ EModSlot.Brakes, 1500.0f }, // Scales
		{ EModSlot.Transmission, 1500.0f }, // Scales
		{ EModSlot.Horns, 500.0f },
		{ EModSlot.Suspension, 1500.0f }, // Scales
		{ EModSlot.Armor, 0.0f }, // not ingame
		{ EModSlot.Turbo, 0.0f }, // not ingame
		{ EModSlot.Xenon, 500.0f },
		{ EModSlot.FrontWheels, 1000.0f },
		{ EModSlot.BackWheels, 1000.0f },
		{ EModSlot.Plateholders, 0.0f },  // not ingame
		{ EModSlot.TrimDesign, 0.0f },  // not ingame
		{ EModSlot.Ornaments, 0.0f },  // not ingame
		{ EModSlot.DialDesign, 0.0f },  // not ingame
		{ EModSlot.SteeringWheel, 0.0f },  // not ingame
		{ EModSlot.ShiftLever, 0.0f },  // not ingame
		{ EModSlot.Plaques, 0.0f },  // not ingame
		{ EModSlot.Hydraulics, 0.0f },  // not ingame
		{ EModSlot.Livery, 500.0f },
		{ EModSlot.Plate, 0.0f },  // not ingame
		{ EModSlot.Colour1, 0.0f },  // not ingame (yet)
		{ EModSlot.Colour2, 0.0f },  // not ingame (yet)
		{ EModSlot.WindowTint, 300.0f }, // Scales
		{ EModSlot.DashboardColor, 0.0f },  // not ingame
		{ EModSlot.TrimColor, 0.0f },  // not ingame
	};

	public static void CalculateModChangeCost(EModSlot slot, int modID, out float outCost, out int outCostGC)
	{
		outCost = 0;
		outCostGC = 0;

		if (slot == EModSlot.CustomizePlateStyle || slot == EModSlot.CustomizePlateText) // GC
		{
			outCostGC = (int)m_dictModSlotPrices[slot];
		}
		else if (slot == EModSlot.Engine || slot == EModSlot.Brakes || slot == EModSlot.Transmission || slot == EModSlot.Suspension || slot == EModSlot.WindowTint) // scale by index (each one is better than the last, e.g. engine upgrades)
		{
			outCost = (modID * m_dictModSlotPrices[slot]);
		}
		else
		{
			outCost = m_dictModSlotPrices[slot];
		}
	}
}

// Gang tags
public class GangTagLayer
{
	public GangTagLayer()
	{

	}

	public GangTagLayer(ELayerType a_LayerType, int a_LayerID, int a_Red, int a_Green, int a_Blue, int a_Alpha, float a_fPosX, float a_fPosY, float a_fScale, string a_strText, int a_Font, bool a_bOutline, bool a_bShadow, int a_SpriteID, float a_fWidth, float a_fHeight, float a_fRotation)
	{
		ID = a_LayerID;
		T = a_LayerType;

		R = a_Red;
		G = a_Green;
		B = a_Blue;
		A = a_Alpha;
		X = a_fPosX;
		Y = a_fPosY;
		S = a_fScale;

		Txt = a_strText;
		Font = a_Font;
		OL = a_bOutline;
		SH = a_bShadow;

		SID = a_SpriteID;
		W = a_fWidth;
		H = a_fHeight;
		ROT = a_fRotation;
	}

	// RECTANGLES AND SPRITES
	public float W { get; set; }
	public float H { get; set; }
	// END RECTANGLES

	// SPRITES
	public int SID { get; set; }
	public float ROT { get; set; }
	// END SPRITES

	// TEXT
	public string Txt { get; set; }
	public int Font { get; set; }
	public bool OL { get; set; }
	public bool SH { get; set; }
	// END TEXT

#if !SERVER
	public void Render(int AlphaOverride = -1)
	{
		int renderAlpha = AlphaOverride == -1 ? A : AlphaOverride;

		if (T == ELayerType.Rectangle)
		{
			RAGE.Game.Graphics.DrawRect(X, Y, W, H, R, G, B, renderAlpha, 1);
		}
		else if (T == ELayerType.Sprite)
		{
			CGangTagSpriteDefinition spriteDef = GangTagSpriteDefinitions.GetGangTagSpriteDefinitionFromID(SID);
			if (spriteDef != null)
			{
				if (RAGE.Game.Graphics.HasStreamedTextureDictLoaded(spriteDef.Dictionary))
				{
					RAGE.Game.Graphics.DrawSprite(spriteDef.Dictionary, spriteDef.SpriteName, X, Y, W, H, ROT, R, G, B, renderAlpha, 0);
				}
				else
				{
					RAGE.Game.Graphics.RequestStreamedTextureDict(spriteDef.Dictionary, true);
				}
			}
		}
		else if (T == ELayerType.Text)
		{
			TextHelper.Draw2D(Txt, X, Y, S, R, G, B, renderAlpha, (RAGE.Game.Font)Font, RAGE.NUI.UIResText.Alignment.Left, OL, SH, true);
		}
	}
#endif

	public ELayerType T { get; set; }
	public int ID { get; set; }

	public int R { get; set; }
	public int G { get; set; }
	public int B { get; set; }
	public int A { get; set; }
	public float X { get; set; }
	public float Y { get; set; }
	public float S { get; set; }
}

public enum ELayerType
{
	Text,
	Sprite,
	Rectangle
}

public class CGangTagSpriteDefinition
{
	public CGangTagSpriteDefinition(int a_ID, string strHumanName, string strDictionary, string strSpriteName, bool bEnabled)
	{
		ID = a_ID;
		HumanName = strHumanName;
		Dictionary = strDictionary;
		SpriteName = strSpriteName;
		Enabled = bEnabled;
	}

	public int ID { get; set; }
	public string HumanName { get; set; }
	public string Dictionary { get; set; }
	public string SpriteName { get; set; }
	public bool Enabled { get; set; } // we keep disabled ones at same index so existing tags dont get randomly swapped, but if disabled, people cannot pick it anymore
}

public static class GangTagSpriteDefinitions
{
	public static CGangTagSpriteDefinition GetGangTagSpriteDefinitionFromID(int id)
	{
		if (id > -1 && id < g_GangTagSpriteDefinitions.Count)
		{
			return g_GangTagSpriteDefinitions[id];
		}

		return null;
	}

	public static CGangTagSpriteDefinition GetGangTagSpriteDefinitionFromHumanName(string strHumanName)
	{
		foreach (var kvPair in g_GangTagSpriteDefinitions)
		{
			if (kvPair.Value.HumanName == strHumanName)
			{
				return kvPair.Value;
			}
		}

		return null;
	}

	public static Dictionary<int, CGangTagSpriteDefinition> g_GangTagSpriteDefinitions = new Dictionary<int, CGangTagSpriteDefinition>()
	{

	};
}

public class CPersistentNotification
{
	public CPersistentNotification(EntityDatabaseID id, string title, string clickEvent, string body, Int64 createdAt)
	{
		ID = id;
		Title = title;
		ClickEvent = clickEvent;
		Body = body;
		CreatedAt = createdAt;
	}

	public EntityDatabaseID ID { get; }
	public string Title { get; }
	public string ClickEvent { get; }
	public string Body { get; }
	public Int64 CreatedAt { get; }
}

public class RadioInstance
{
	public RadioInstance(int a_ID, EntityDatabaseID a_Account, string strName, string strEndpoint, Int64 expirationTime)
	{
		ID = a_ID;
		Account = a_Account;
		Name = strName;
		Endpoint = strEndpoint;
		ExpirationTime = expirationTime;
	}

#if SERVER
	public async Task Resolve()
	{
		try
		{
			Uri uriResult;
			bool result = Uri.TryCreate(Endpoint, UriKind.Absolute, out uriResult)
				&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

			if (!result || !Endpoint.EndsWith(".pls"))
			{
				Resolved = false;
				EndpointResolved = String.Empty;
			}
			else
			{
				Resolved = false;
				WebClient resolver = new WebClient();
				string strBody = await resolver.DownloadStringTaskAsync(Endpoint).ConfigureAwait(true);

				string strFind = "File1=";
				string strEnd1 = "\r";
				string strEnd2 = "\n";
				string strEnd3 = "\r\n";
				int startIndex = strBody.IndexOf(strFind) + strFind.Length;
				if (startIndex != -1)
				{
					int endIndex = strBody.IndexOf(strEnd1, startIndex);
					if (endIndex == -1)
					{
						endIndex = strBody.IndexOf(strEnd2, startIndex);
						if (endIndex == -1)
						{
							endIndex = strBody.IndexOf(strEnd3, startIndex);
						}
					}

					if (endIndex != -1)
					{
						string strResolved = strBody.Substring(startIndex, endIndex - startIndex);
						EndpointResolved = strResolved;
						Resolved = true;
					}
				}
			}
		}
		catch
		{
			Resolved = false;
			EndpointResolved = String.Empty;
		}
	}
#endif

	public int ID { get; set; }
	public EntityDatabaseID Account { get; set; }
	public string Name { get; set; }
	public string Endpoint { get; set; }
	public string EndpointResolved { get; set; }
	public Int64 ExpirationTime { get; set; }
	private bool Resolved { get; set; }

	public bool IsExpired()
	{
		if (ExpirationTime == 0 || Account < 0)
		{
			return false;
		}

		Int64 unixTimestamp = Helpers.GetUnixTimestamp();
		return unixTimestamp >= ExpirationTime;
	}

	public bool ResolvedSuccessfully()
	{
		return ID < 0 || !String.IsNullOrEmpty(EndpointResolved);
	}
}

public enum EWeaponAttachmentType
{
	None = -1,
	MagazineOrRound,
	Scopes,
	Livery,
	Livery_Slide,
	Flashlight,
	Muzzle,
	BasicColor,
	Grips,
	Barrels
}

public enum EWeaponAttachmentWeaponType
{
	Melee,
	Handgun,
	SMG,
	Shotgun,
	Rifle
}

public class WeaponAttachmentDefinition
{
	public WeaponAttachmentDefinition(EItemID a_ID, EWeaponAttachmentType a_AttachmentType, EWeaponAttachmentWeaponType a_WeaponType)
	{
		ID = a_ID;
		AttachmentType = a_AttachmentType;
		WeaponType = a_WeaponType;
	}

#if !SERVER
	public bool DoesAttachmentBelongOnWeapon(WeaponHash weaponHash, EItemID attachmentID)
	{
		return GetAttachmentHashForWeaponAndAttachment(weaponHash, attachmentID) > 0;
	}

	private uint GetAttachmentHashForWeaponAndAttachment(WeaponHash weaponHash, EItemID attachmentID)
	{
		uint val = 0;
		if (WeaponAttachmentDefinitions.g_WeaponAttachmentIDs.ContainsKey(attachmentID))
		{
			if (WeaponAttachmentDefinitions.g_WeaponAttachmentIDs[attachmentID].ContainsKey(weaponHash))
			{
				val = WeaponAttachmentDefinitions.g_WeaponAttachmentIDs[attachmentID][weaponHash];
			}
		}

		return val;
	}


	public void ApplyToWeapon(RAGE.Elements.Player player, WeaponHash weaponHash, EItemID attachmentID)
	{
		uint attachmentHash = GetAttachmentHashForWeaponAndAttachment(weaponHash, attachmentID);
		if (attachmentHash > 0)
		{
			if (!player.HasGotWeaponComponent((uint)weaponHash, attachmentHash))
			{
				player.GiveWeaponComponentTo((uint)weaponHash, attachmentHash);
			}
		}
	}

	public void RemoveFromWeapon(RAGE.Elements.Player player, WeaponHash weaponHash, EItemID attachmentID)
	{
		uint attachmentHash = GetAttachmentHashForWeaponAndAttachment(weaponHash, attachmentID);
		if (attachmentHash > 0)
		{
			if (player.HasGotWeaponComponent((uint)weaponHash, attachmentHash))
			{
				player.RemoveWeaponComponentFrom((uint)weaponHash, attachmentHash);
			}
		}
	}
#endif

	public EItemID ID { get; }
	public EWeaponAttachmentType AttachmentType { get; }
	public EWeaponAttachmentWeaponType WeaponType { get; }
}

public static class WeaponAttachmentDefinitions
{
	public static WeaponAttachmentDefinition GetWeaponAttachmentDefinitionByID(EItemID id)
	{
		WeaponAttachmentDefinition outDef = null;
		g_WeaponAttachmentDefinitions.TryGetValue(id, out outDef);
		return outDef;
	}

	public static Dictionary<EItemID, WeaponAttachmentDefinition> g_WeaponAttachmentDefinitions = new Dictionary<EItemID, WeaponAttachmentDefinition>()
	{
		{ EItemID.HANDGUN_SUPPRESSOR, new WeaponAttachmentDefinition(EItemID.HANDGUN_SUPPRESSOR, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.SMG_SUPPRESSOR, new WeaponAttachmentDefinition(EItemID.SMG_SUPPRESSOR, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_SUPPRESSOR, new WeaponAttachmentDefinition(EItemID.SHOTGUN_SUPPRESSOR, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_SUPPRESSOR, new WeaponAttachmentDefinition(EItemID.RIFLE_SUPPRESSOR, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.HANDGUN_FLASHLIGHT, new WeaponAttachmentDefinition(EItemID.HANDGUN_FLASHLIGHT, EWeaponAttachmentType.Flashlight, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.SMG_FLASHLIGHT, new WeaponAttachmentDefinition(EItemID.SMG_FLASHLIGHT, EWeaponAttachmentType.Flashlight, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_FLASHLIGHT, new WeaponAttachmentDefinition(EItemID.SHOTGUN_FLASHLIGHT, EWeaponAttachmentType.Flashlight, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_FLASHLIGHT, new WeaponAttachmentDefinition(EItemID.RIFLE_FLASHLIGHT, EWeaponAttachmentType.Flashlight, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.HANDGUN_EXTENDED_MAG, new WeaponAttachmentDefinition(EItemID.HANDGUN_EXTENDED_MAG, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.SMG_EXTENDED_MAG, new WeaponAttachmentDefinition(EItemID.SMG_EXTENDED_MAG, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_EXTENDED_MAG, new WeaponAttachmentDefinition(EItemID.SHOTGUN_EXTENDED_MAG, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_EXTENDED_MAG, new WeaponAttachmentDefinition(EItemID.RIFLE_EXTENDED_MAG, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.SMG_DRUM_MAGAZINE, new WeaponAttachmentDefinition(EItemID.SMG_DRUM_MAGAZINE, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_DRUM_MAGAZINE, new WeaponAttachmentDefinition(EItemID.SHOTGUN_DRUM_MAGAZINE, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_DRUM_MAGAZINE, new WeaponAttachmentDefinition(EItemID.RIFLE_DRUM_MAGAZINE, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.HANDGUN_HOLOGRAPHIC_SIGHT, new WeaponAttachmentDefinition(EItemID.HANDGUN_HOLOGRAPHIC_SIGHT, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.SMG_HOLOGRAPHIC_SIGHT, new WeaponAttachmentDefinition(EItemID.SMG_HOLOGRAPHIC_SIGHT, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_HOLOGRAPHIC_SIGHT, new WeaponAttachmentDefinition(EItemID.SHOTGUN_HOLOGRAPHIC_SIGHT, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_HOLOGRAPHIC_SIGHT, new WeaponAttachmentDefinition(EItemID.RIFLE_HOLOGRAPHIC_SIGHT, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.HANDGUN_COMPENSATOR, new WeaponAttachmentDefinition(EItemID.HANDGUN_COMPENSATOR, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Handgun) },

		{ EItemID.HANDGUN_SMALL_SCOPE, new WeaponAttachmentDefinition(EItemID.HANDGUN_SMALL_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.SMG_SMALL_SCOPE, new WeaponAttachmentDefinition(EItemID.SMG_SMALL_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_SMALL_SCOPE, new WeaponAttachmentDefinition(EItemID.SHOTGUN_SMALL_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_SMALL_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_SMALL_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.RIFLE_LARGE_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_LARGE_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.HANDGUN_MOUNTED_SCOPE, new WeaponAttachmentDefinition(EItemID.HANDGUN_MOUNTED_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Handgun) },

		{ EItemID.SMG_FOREGRIP, new WeaponAttachmentDefinition(EItemID.SMG_FOREGRIP, EWeaponAttachmentType.Grips, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_FOREGRIP, new WeaponAttachmentDefinition(EItemID.SHOTGUN_FOREGRIP, EWeaponAttachmentType.Grips, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_FOREGRIP, new WeaponAttachmentDefinition(EItemID.RIFLE_FOREGRIP, EWeaponAttachmentType.Grips, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.SMG_BASIC_SCOPE, new WeaponAttachmentDefinition(EItemID.SMG_BASIC_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.RIFLE_BASIC_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_BASIC_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.SMG_MEDIUM_SCOPE, new WeaponAttachmentDefinition(EItemID.SMG_MEDIUM_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_MEDIUM_SCOPE, new WeaponAttachmentDefinition(EItemID.SHOTGUN_MEDIUM_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_MEDIUM_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_MEDIUM_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.RIFLE_ADVANCED_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_ADVANCED_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.RIFLE_ZOOM_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_ZOOM_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.RIFLE_NIGHTVISION_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_NIGHTVISION_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.RIFLE_THERMAL_SCOPE, new WeaponAttachmentDefinition(EItemID.RIFLE_THERMAL_SCOPE, EWeaponAttachmentType.Scopes, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.SMG_HEAVY_BARREL, new WeaponAttachmentDefinition(EItemID.SMG_HEAVY_BARREL, EWeaponAttachmentType.Barrels, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.RIFLE_HEAVY_BARREL, new WeaponAttachmentDefinition(EItemID.RIFLE_HEAVY_BARREL, EWeaponAttachmentType.Barrels, EWeaponAttachmentWeaponType.Rifle) },

		// muzzles
		{ EItemID.SMG_FLAT_MUZZLE, new WeaponAttachmentDefinition(EItemID.SMG_FLAT_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_TACTICAL_MUZZLE, new WeaponAttachmentDefinition(EItemID.SMG_TACTICAL_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_FAT_END_MUZZLE, new WeaponAttachmentDefinition(EItemID.SMG_FAT_END_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_PRECISION_MUZZLE, new WeaponAttachmentDefinition(EItemID.SMG_PRECISION_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_HEAVY_DUTY_MUZZLE, new WeaponAttachmentDefinition(EItemID.SMG_HEAVY_DUTY_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_SLANTED_MUZZLE, new WeaponAttachmentDefinition(EItemID.SMG_SLANTED_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_SPLIT_END_MUZZLE, new WeaponAttachmentDefinition(EItemID.SMG_SPLIT_END_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SHOTGUN_SQUARED_MUZZLE, new WeaponAttachmentDefinition(EItemID.SHOTGUN_SQUARED_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.RIFLE_FLAG_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_FLAG_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_TACTICAL_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_TACTICAL_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_FAT_END_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_FAT_END_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_PRECISION_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_PRECISION_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_HEAVY_DUTY_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_HEAVY_DUTY_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_SLANTED_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_SLANTED_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_SPLIT_END_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_SPLIT_END_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_SQUARED_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_SQUARED_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_BELL_END_MUZZLE, new WeaponAttachmentDefinition(EItemID.RIFLE_BELL_END_MUZZLE, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Rifle) },

		// ammo
		{ EItemID.HANDGUN_TRACER_ROUNDS, new WeaponAttachmentDefinition(EItemID.HANDGUN_TRACER_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.HANDGUN_INCENDIARY_ROUNDS, new WeaponAttachmentDefinition(EItemID.HANDGUN_INCENDIARY_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.HANDGUN_HOLLOW_ROUNDS, new WeaponAttachmentDefinition(EItemID.HANDGUN_HOLLOW_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Handgun) },
		{ EItemID.HANDGUN_FMJ_ROUNDS, new WeaponAttachmentDefinition(EItemID.HANDGUN_FMJ_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Handgun) },

		{ EItemID.SMG_TRACER_ROUNDS, new WeaponAttachmentDefinition(EItemID.SMG_TRACER_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_INCENDIARY_ROUNDS, new WeaponAttachmentDefinition(EItemID.SMG_INCENDIARY_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_HOLLOW_ROUNDS, new WeaponAttachmentDefinition(EItemID.SMG_HOLLOW_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.SMG) },
		{ EItemID.SMG_FMJ_ROUNDS, new WeaponAttachmentDefinition(EItemID.SMG_FMJ_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.SMG) },

		{ EItemID.RIFLE_TRACER_ROUNDS, new WeaponAttachmentDefinition(EItemID.RIFLE_TRACER_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_INCENDIARY_ROUNDS, new WeaponAttachmentDefinition(EItemID.RIFLE_INCENDIARY_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_AP_ROUNDS, new WeaponAttachmentDefinition(EItemID.RIFLE_AP_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Rifle) },
		{ EItemID.RIFLE_FMJ_ROUNDS, new WeaponAttachmentDefinition(EItemID.RIFLE_FMJ_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.RIFLE_EXPLOSIVE_ROUNDS, new WeaponAttachmentDefinition(EItemID.RIFLE_EXPLOSIVE_ROUNDS, EWeaponAttachmentType.MagazineOrRound, EWeaponAttachmentWeaponType.Rifle) },

		{ EItemID.SHOTGUN_DRAGONSBREATH_SHELLS, new WeaponAttachmentDefinition(EItemID.SHOTGUN_DRAGONSBREATH_SHELLS, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.SHOTGUN_STEEL_BUCKSHOT_SHELLS, new WeaponAttachmentDefinition(EItemID.SHOTGUN_STEEL_BUCKSHOT_SHELLS, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.SHOTGUN_FLECHETTE_SHELLS, new WeaponAttachmentDefinition(EItemID.SHOTGUN_FLECHETTE_SHELLS, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Shotgun) },
		{ EItemID.SHOTGUN_EXPLOSIVE_SHELLS, new WeaponAttachmentDefinition(EItemID.SHOTGUN_EXPLOSIVE_SHELLS, EWeaponAttachmentType.Muzzle, EWeaponAttachmentWeaponType.Shotgun) },

	};

#if !SERVER
	public static Dictionary<EItemID, Dictionary<WeaponHash, uint>> g_WeaponAttachmentIDs = new Dictionary<EItemID, Dictionary<WeaponHash, uint>>
	{
		{ EItemID.HANDGUN_SUPPRESSOR, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pistol_mk2, 0x65EA7EBB },
											{ WeaponHash.Pistol, 0x65EA7EBB },
											{ WeaponHash.Snspistol_mk2, 0x65EA7EBB },
											{ WeaponHash.Combatpistol, 0xC304849A },
											{ WeaponHash.Appistol, 0xC304849A },
											{ WeaponHash.Pistol50, 0xA73D4664 },
											{ WeaponHash.Heavypistol, 0xC304849A },
											{ WeaponHash.Vintagepistol, 0xC304849A },
											{ WeaponHash.CeramicPistol, 0x9307D6FA },
										}
		},

		{ EItemID.SMG_SUPPRESSOR, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Microsmg, 0xA73D4664 },
												{ WeaponHash.Smg, 0xC304849A },
												{ WeaponHash.Assaultsmg, 0xA73D4664 },
												{ WeaponHash.Smg_mk2, 0xC304849A },
												{ WeaponHash.Machinepistol, 0xC304849A },
											}
		},

		{ EItemID.SHOTGUN_SUPPRESSOR, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Pumpshotgun, 0xE608B35E },
												{ WeaponHash.Assaultshotgun, 0x837445AA },
												{ WeaponHash.Pumpshotgun_mk2, 0xAC42DF71 },
												{ WeaponHash.Heavyshotgun, 0xA73D4664 },
												{ WeaponHash.Bullpupshotgun, 0xA73D4664 },
												{ WeaponHash.CombatShotgun, 0x837445AA },
											}
		},

		{ EItemID.RIFLE_SUPPRESSOR, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Assaultrifle, 0xA73D4664 },
												{ WeaponHash.Carbinerifle, 0x837445AA },
												{ WeaponHash.Advancedrifle, 0x837445AA },
												{ WeaponHash.Specialcarbine, 0xA73D4664 },
												{ WeaponHash.Bullpuprifle, 0x837445AA },
												{ WeaponHash.Bullpuprifle_mk2, 0x837445AA },
												{ WeaponHash.Specialcarbine_mk2, 0xA73D4664 },
												{ WeaponHash.Assaultrifle_mk2, 0xA73D4664 },
												{ WeaponHash.Carbinerifle_mk2, 0x837445AA },
												{ WeaponHash.Sniperrifle, 0xA73D4664 },
												{ WeaponHash.Marksmanrifle_mk2, 0x837445AA },
												{ WeaponHash.Heavysniper_mk2, 0xAC42DF71 },
												{ WeaponHash.Marksmanrifle, 0x837445AA },
												{ WeaponHash.MilitaryRifle, 0x837445AA },
											}
		},

		// FLASHLIGHT
		{ EItemID.HANDGUN_FLASHLIGHT, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pistol_mk2, 0x43FD595B },
											{ WeaponHash.Pistol, 0x359B7AAE },
											{ WeaponHash.Snspistol_mk2, 0x4A4965F3 },
											{ WeaponHash.Combatpistol, 0x359B7AAE },
											{ WeaponHash.Appistol, 0x359B7AAE },
											{ WeaponHash.Pistol50, 0x359B7AAE },
											{ WeaponHash.Heavypistol, 0x359B7AAE },
											{ WeaponHash.Revolver, 0x359B7AAE },
											{ WeaponHash.Revolver_mk2, 0x359B7AAE },
										}
		},

		{ EItemID.SMG_FLASHLIGHT, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Microsmg, 0x359B7AAE },
												{ WeaponHash.Smg, 0x7BC4CDDC },
												{ WeaponHash.Assaultsmg, 0x7BC4CDDC },
												{ WeaponHash.Smg_mk2, 0x7BC4CDDC },
												{ WeaponHash.Combatpdw, 0x7BC4CDDC },
											}
		},

		{ EItemID.SHOTGUN_FLASHLIGHT, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Pumpshotgun, 0x7BC4CDDC },
												{ WeaponHash.Assaultshotgun, 0x7BC4CDDC },
												{ WeaponHash.Pumpshotgun_mk2, 0x7BC4CDDC },
												{ WeaponHash.Heavyshotgun, 0x7BC4CDDC },
												{ WeaponHash.Bullpupshotgun, 0x7BC4CDDC },
												{ WeaponHash.CombatShotgun, 0x7BC4CDDC },
											}
		},

		{ EItemID.RIFLE_FLASHLIGHT, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Assaultrifle, 0x7BC4CDDC },
												{ WeaponHash.Carbinerifle, 0x7BC4CDDC },
												{ WeaponHash.Advancedrifle, 0x7BC4CDDC },
												{ WeaponHash.Specialcarbine, 0x7BC4CDDC },
												{ WeaponHash.Bullpuprifle, 0x7BC4CDDC },
												{ WeaponHash.Bullpuprifle_mk2, 0x7BC4CDDC },
												{ WeaponHash.Specialcarbine_mk2, 0x7BC4CDDC },
												{ WeaponHash.Assaultrifle_mk2, 0x7BC4CDDC },
												{ WeaponHash.Carbinerifle_mk2, 0x7BC4CDDC },
												{ WeaponHash.Grenadelauncher, 0x7BC4CDDC },
												{ WeaponHash.Marksmanrifle_mk2, 0x7BC4CDDC },
												{ WeaponHash.Marksmanrifle, 0x7BC4CDDC },
												{ WeaponHash.MilitaryRifle, 0x7BC4CDDC }
											}
		},

		// EXTENDED MAG
		{ EItemID.HANDGUN_EXTENDED_MAG, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pistol_mk2, 0x5ED6C128 },
											{ WeaponHash.Pistol, 0xED265A1C },
											{ WeaponHash.Snspistol, 0x7B0033B3 },
											{ WeaponHash.Snspistol_mk2, 0xCE8C0772 },
											{ WeaponHash.Combatpistol, 0xD67B4F2D },
											{ WeaponHash.Appistol, 0x249A17D5 },
											{ WeaponHash.Pistol50, 0xD9D3AC92 },
											{ WeaponHash.Heavypistol, 0x64F9C62B },
											{ WeaponHash.Vintagepistol, 0x33BA12E8 },
											{ WeaponHash.CeramicPistol, 0x81786CA9 },
										}
		},

		{ EItemID.SMG_EXTENDED_MAG, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Microsmg, 0x10E6BA2B },
												{ WeaponHash.Smg, 0x350966FB },
												{ WeaponHash.Assaultsmg, 0xBB46E417 },
												{ WeaponHash.Minismg, 0x937ED0B7 },
												{ WeaponHash.Smg_mk2, 0xB9835B2E },
												{ WeaponHash.Machinepistol, 0xB92C6979 },
												{ WeaponHash.Combatpdw, 0x334A5203 },
											}
		},

		{ EItemID.SHOTGUN_EXTENDED_MAG, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Assaultshotgun, 0x86BD7F72 },
												{ WeaponHash.Heavyshotgun, 0x971CF6FD },
											}
		},

		{ EItemID.RIFLE_EXTENDED_MAG, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Assaultrifle, 0xB1214F9B },
												{ WeaponHash.Carbinerifle, 0x91109691 },
												{ WeaponHash.Advancedrifle, 0x8EC1C979 },
												{ WeaponHash.Specialcarbine, 0x7C8BD10E },
												{ WeaponHash.Bullpuprifle, 0xB3688B0F },
												{ WeaponHash.Bullpuprifle_mk2, 0xEFB00628 },
												{ WeaponHash.Specialcarbine_mk2, 0xDE1FA12C },
												{ WeaponHash.Assaultrifle_mk2, 0xD12ACA6F },
												{ WeaponHash.Carbinerifle_mk2, 0x5DD5DBD5 },
												{ WeaponHash.Mg, 0x82158B47 },
												{ WeaponHash.Combatmg, 0xD6C59CD6 },
												{ WeaponHash.Compactrifle, 0x59FF9BF8 },
												{ WeaponHash.Combatmg_mk2, 0x17DF42E9 },
												{ WeaponHash.Gusenberg, 0x17DF42E9 },
												{ WeaponHash.Marksmanrifle_mk2, 0x17DF42E9 },
												{ WeaponHash.Heavysniper_mk2, 0x17DF42E9 },
												{ WeaponHash.Marksmanrifle, 0x17DF42E9 },
												{ WeaponHash.MilitaryRifle, 0x684ACE42 },
											}
		},

		// DRUM EXTENDED MAG
		{ EItemID.SMG_DRUM_MAGAZINE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg, 0x79C77076 },
											{ WeaponHash.Machinepistol, 0xA9E9CAF4 },
											{ WeaponHash.Combatpdw, 0x6EB8C8DB }
										}
		},

		{ EItemID.SHOTGUN_DRUM_MAGAZINE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Heavyshotgun, 0x88C7DA53 }
										}
		},

		{ EItemID.RIFLE_DRUM_MAGAZINE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Assaultrifle, 0xDBF0A53D },
											{ WeaponHash.Specialcarbine, 0x6B59AEAA },
											{ WeaponHash.Compactrifle, 0xC607740E }
										}
		},

		// HOLOGRAPHIC SIGHT
		{ EItemID.HANDGUN_HOLOGRAPHIC_SIGHT, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Revolver_mk2, 0x420FD713 },
										}
		},


		{ EItemID.SMG_HOLOGRAPHIC_SIGHT, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0x9FDB5652 }
										}
		},

		{ EItemID.SHOTGUN_HOLOGRAPHIC_SIGHT, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0x420FD713 }
										}
		},

		{ EItemID.RIFLE_HOLOGRAPHIC_SIGHT, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x420FD713 },
											{ WeaponHash.Specialcarbine_mk2, 0x420FD713 },
											{ WeaponHash.Assaultrifle_mk2, 0x420FD713 },
											{ WeaponHash.Carbinerifle_mk2, 0x420FD713 },
											{ WeaponHash.Combatmg_mk2, 0x420FD713 },
											{ WeaponHash.Marksmanrifle_mk2, 0x420FD713 },
										}
		},

		// COMPENSATOR
		{ EItemID.HANDGUN_COMPENSATOR, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Revolver_mk2, 0x27077CCB },
											{ WeaponHash.Pistol_mk2, 0x21E34793 },
											{ WeaponHash.Snspistol_mk2, 0xAA8283BF },
										}
		},

		// SMALL SCOPE
		{ EItemID.HANDGUN_SMALL_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Revolver_mk2, 0x49B2945 },
										}
		},


		{ EItemID.SMG_SMALL_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xE502AB6B }
										}
		},

		{ EItemID.SHOTGUN_SMALL_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0x49B2945 }
										}
		},

		{ EItemID.RIFLE_SMALL_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0xC7ADD105 },
											{ WeaponHash.Specialcarbine_mk2, 0x49B2945 },
											{ WeaponHash.Assaultrifle_mk2, 0x49B2945 },
											{ WeaponHash.Carbinerifle_mk2, 0x49B2945 },
											{ WeaponHash.MilitaryRifle, 0xAA2C45B4 },
										}
		},

		// LARGE SCOPE
		{ EItemID.RIFLE_LARGE_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Specialcarbine_mk2, 0xC66B6542 },
											{ WeaponHash.Assaultrifle_mk2, 0xC66B6542 },
											{ WeaponHash.Carbinerifle_mk2, 0xC66B6542 },
											{ WeaponHash.Combatmg_mk2, 0xC66B6542 },
											{ WeaponHash.Marksmanrifle_mk2, 0xC66B6542 },
										}
		},

		// MOUNTED SCOPE
		{ EItemID.HANDGUN_MOUNTED_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Snspistol_mk2, 0x47DE9258 },
											{ WeaponHash.Pistol_mk2, 0x8ED4BB70 },
										}
		},

		// FOREGRIP
		{ EItemID.SMG_FOREGRIP, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Combatpdw, 0xC164F53 }
										}
		},

		{ EItemID.SHOTGUN_FOREGRIP, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Assaultshotgun, 0xC164F53 },
											{ WeaponHash.Bullpupshotgun, 0xC164F53 },
											{ WeaponHash.Heavyshotgun, 0xC164F53 },
										}
		},

		{ EItemID.RIFLE_FOREGRIP, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Assaultrifle, 0xC164F53 },
											{ WeaponHash.Carbinerifle, 0xC164F53 },
											{ WeaponHash.Specialcarbine, 0xC164F53 },
											{ WeaponHash.Bullpuprifle, 0xC164F53 },
											{ WeaponHash.Bullpuprifle_mk2, 0x9D65907A },
											{ WeaponHash.Specialcarbine_mk2, 0x9D65907A },
											{ WeaponHash.Assaultrifle_mk2, 0x9D65907A },
											{ WeaponHash.Carbinerifle_mk2, 0x9D65907A },
											{ WeaponHash.Combatmg, 0xC164F53 },
											{ WeaponHash.Combatmg_mk2, 0x9D65907A },
											{ WeaponHash.Marksmanrifle_mk2, 0x9D65907A },
											{ WeaponHash.Marksmanrifle, 0xC164F53 },
											{ WeaponHash.Grenadelauncher, 0xC164F53 },
										}
		},

		// BASIC SCOPE
		{ EItemID.SMG_BASIC_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Microsmg, 0x9D2FBF29 },
											{ WeaponHash.Smg, 0x3CC6BA57 },
											{ WeaponHash.Assaultsmg, 0x9D2FBF29 },
											{ WeaponHash.Combatpdw, 0xAA2C45B4 }
										}
		},

		{ EItemID.RIFLE_BASIC_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Assaultrifle, 0x9D2FBF29 },
											{ WeaponHash.Carbinerifle, 0xA0D89C42 },
											{ WeaponHash.Advancedrifle, 0xAA2C45B4 },
											{ WeaponHash.Specialcarbine, 0xA0D89C42 },
											{ WeaponHash.Bullpuprifle, 0xAA2C45B4 },
											{ WeaponHash.Mg, 0x3C00AFED },
											{ WeaponHash.Combatmg, 0xA0D89C42 },
											{ WeaponHash.Sniperrifle, 0xD2443DDC },
											{ WeaponHash.Heavysniper, 0xD2443DDC },
											{ WeaponHash.Marksmanrifle, 0x1C221B1A },
											{ WeaponHash.Grenadelauncher, 0xAA2C45B4 }
										}
		},

		// MEDIUM SCOPE
		{ EItemID.SMG_MEDIUM_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2,  0x3DECC7DA}
										}
		},

		{ EItemID.SHOTGUN_MEDIUM_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0x3F3C8181 }

										}
		},

		{ EItemID.RIFLE_MEDIUM_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x3F3C8181 },
											{ WeaponHash.Combatmg_mk2, 0x3F3C8181 }
										}
		},

		// ADVANCED SCOPE
		{ EItemID.RIFLE_ADVANCED_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Sniperrifle, 0xBC54DA77 },
											{ WeaponHash.Heavysniper, 0xBC54DA77 },
											{ WeaponHash.Heavysniper_mk2, 0xBC54DA77 },
										}
		},

		// ZOOM SCOPE
		{ EItemID.RIFLE_ZOOM_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Marksmanrifle_mk2, 0x5B1C713C },
											{ WeaponHash.Heavysniper_mk2, 0x82C10383 },
										}
		},

		// NIGHTVISION SCOPE
		{ EItemID.RIFLE_NIGHTVISION_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Heavysniper_mk2, 0xB68010B0 },
										}
		},

		// THERMAL SCOPE
		{ EItemID.RIFLE_THERMAL_SCOPE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Heavysniper_mk2, 0x2E43DA41 },
										}
		},

		// HEAVY BARREL
		{ EItemID.SMG_HEAVY_BARREL, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xA564D78B },
										}
		},

		{ EItemID.RIFLE_HEAVY_BARREL, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x3BF26DC7 },
											{ WeaponHash.Specialcarbine_mk2, 0xF97F783B },
											{ WeaponHash.Assaultrifle_mk2, 0x5646C26A },
											{ WeaponHash.Carbinerifle_mk2, 0x8B3C480B },
											{ WeaponHash.Combatmg_mk2, 0xB5E2575B },
											{ WeaponHash.Marksmanrifle_mk2, 0x68373DDC },
											{ WeaponHash.Heavysniper_mk2, 0x108AB09E }
										}
		},

		// SMG_FLAG_MUZZLE
		{ EItemID.SMG_FLAT_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xB99402D4 },
										}
		},

		// SMG_TACTICAL_MUZZLE
		{ EItemID.SMG_TACTICAL_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xC867A07B },
										}
		},

		// SMG_FAT_END_MUZZLE
		{ EItemID.SMG_FAT_END_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xDE11CBCF },
										}
		},

		// SMG_PRECISION_MUZZLE
		{ EItemID.SMG_PRECISION_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xEC9068CC },
										}
		},

		// SMG_HEAVY_DUTY_MUZZLE
		{ EItemID.SMG_HEAVY_DUTY_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0x2E7957A },
										}
		},

		// SMG_SLANTED_MUZZLE
		{ EItemID.SMG_SLANTED_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0x347EF8AC },
										}
		},

		// SMG_SPLIT_END_MUZZLE
		{ EItemID.SMG_SPLIT_END_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0x4DB62ABE },
										}
		},

		// SHOTGUN_SQUARED_MUZZLE
		{ EItemID.SHOTGUN_SQUARED_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0x5F7DCE4D },
										}
		},

		// RIFLE_FLAG_MUZZLE
		{ EItemID.RIFLE_FLAG_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0xB99402D4 },
											{ WeaponHash.Specialcarbine_mk2, 0xB99402D4 },
											{ WeaponHash.Assaultrifle_mk2, 0xB99402D4 },
											{ WeaponHash.Carbinerifle_mk2, 0xB99402D4 },
											{ WeaponHash.Combatmg_mk2, 0xB99402D4 },
											{ WeaponHash.Marksmanrifle_mk2, 0xB99402D4 },
										}
		},

		// RIFLE_TACTICAL_MUZZLE
		{ EItemID.RIFLE_TACTICAL_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0xC867A07B },
											{ WeaponHash.Specialcarbine_mk2, 0xC867A07B },
											{ WeaponHash.Assaultrifle_mk2, 0xC867A07B },
											{ WeaponHash.Carbinerifle_mk2, 0xC867A07B },
											{ WeaponHash.Combatmg_mk2, 0xC867A07B },
											{ WeaponHash.Marksmanrifle_mk2, 0xC867A07B },
										}
		},

		// RIFLE_FAT_END_MUZZLE
		{ EItemID.RIFLE_FAT_END_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0xDE11CBCF },
											{ WeaponHash.Specialcarbine_mk2, 0xDE11CBCF },
											{ WeaponHash.Assaultrifle_mk2, 0xDE11CBCF },
											{ WeaponHash.Carbinerifle_mk2, 0xDE11CBCF },
											{ WeaponHash.Combatmg_mk2, 0xDE11CBCF },
											{ WeaponHash.Marksmanrifle_mk2, 0xDE11CBCF },
										}
		},

		// RIFLE_PRECISION_MUZZLE
		{ EItemID.RIFLE_PRECISION_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0xEC9068CC },
											{ WeaponHash.Specialcarbine_mk2, 0xEC9068CC },
											{ WeaponHash.Assaultrifle_mk2, 0xEC9068CC },
											{ WeaponHash.Carbinerifle_mk2, 0xEC9068CC },
											{ WeaponHash.Combatmg_mk2, 0xEC9068CC },
											{ WeaponHash.Marksmanrifle_mk2, 0xEC9068CC },
										}
		},

		// RIFLE_HEAVY_DUTY_MUZZLE
		{ EItemID.RIFLE_HEAVY_DUTY_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x2E7957A },
											{ WeaponHash.Specialcarbine_mk2, 0x2E7957A },
											{ WeaponHash.Assaultrifle_mk2, 0x2E7957A },
											{ WeaponHash.Carbinerifle_mk2, 0x2E7957A },
											{ WeaponHash.Combatmg_mk2, 0x2E7957A },
											{ WeaponHash.Marksmanrifle_mk2, 0x2E7957A },
										}
		},

		// RIFLE_SLANTED_MUZZLE
		{ EItemID.RIFLE_SLANTED_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x347EF8AC },
											{ WeaponHash.Specialcarbine_mk2, 0x347EF8AC },
											{ WeaponHash.Assaultrifle_mk2, 0x347EF8AC },
											{ WeaponHash.Carbinerifle_mk2, 0x347EF8AC },
											{ WeaponHash.Combatmg_mk2, 0x347EF8AC },
											{ WeaponHash.Marksmanrifle_mk2, 0x347EF8AC },
										}
		},

		// RIFLE_SPLIT_END_MUZZLE
		{ EItemID.RIFLE_SPLIT_END_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x4DB62ABE },
											{ WeaponHash.Specialcarbine_mk2, 0x4DB62ABE },
											{ WeaponHash.Assaultrifle_mk2, 0x4DB62ABE },
											{ WeaponHash.Carbinerifle_mk2, 0x4DB62ABE },
											{ WeaponHash.Combatmg_mk2, 0x4DB62ABE },
											{ WeaponHash.Marksmanrifle_mk2, 0x4DB62ABE },
										}
		},

		// RIFLE_SQUARED_MUZZLE
		{ EItemID.RIFLE_SQUARED_MUZZLE, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Heavysniper_mk2, 0x5F7DCE4D },
										}
		},

		// RIFLE_BELL_END_MUZZLE
		{ EItemID.RIFLE_BELL_END_MUZZLE, new Dictionary<WeaponHash, uint>()
											{
												{ WeaponHash.Heavysniper_mk2, 0x6927E1A1 },
											}
		},


		// HANDGUN_TRACER_ROUNDS,
		{ EItemID.HANDGUN_TRACER_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Revolver_mk2, 0xC6D8E476 },
											{ WeaponHash.Snspistol_mk2, 0x902DA26E },
											{ WeaponHash.Pistol_mk2, 0x25CAAEAF },
										}
		},

		// HANDGUN_INCENDIARY_ROUNDS,
		{ EItemID.HANDGUN_INCENDIARY_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Revolver_mk2, 0xEFBF25 },
											{ WeaponHash.Snspistol_mk2, 0xE6AD5F79 },
											{ WeaponHash.Pistol_mk2, 0x2BBD7A3A },
										}
		},

		// HANDGUN_HOLLOW_ROUNDS,
		{ EItemID.HANDGUN_HOLLOW_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Revolver_mk2, 0x10F42E8F },
											{ WeaponHash.Snspistol_mk2, 0x8D107402 },
											{ WeaponHash.Pistol_mk2, 0x85FEA109 },
										}
		},

		// HANDGUN_FMJ_ROUNDS,
		{ EItemID.HANDGUN_FMJ_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Revolver_mk2, 0xDC8BA3F },
											{ WeaponHash.Snspistol_mk2, 0xC111EB26 },
											{ WeaponHash.Pistol_mk2, 0x4F37DF2A },
										}
		},

		// SMG_TRACER_ROUNDS,
		{ EItemID.SMG_TRACER_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0x7FEA36EC }
										}
		},

		// SMG_INCENDIARY_ROUNDS,
		{ EItemID.SMG_INCENDIARY_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xD99222E5 }
										}
		},

		// SMG_HOLLOW_ROUNDS,
		{ EItemID.SMG_HOLLOW_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0x3A1BD6FA },
										}
		},

		// SMG_FMJ_ROUNDS,
		{ EItemID.SMG_FMJ_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Smg_mk2, 0xB5A715F },
										}
		},

		// RIFLE_TRACER_ROUNDS,
		{ EItemID.RIFLE_TRACER_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x822060A9 },
											{ WeaponHash.Specialcarbine_mk2, 0x8765C68A },
											{ WeaponHash.Assaultrifle_mk2, 0xEF2C78C1 },
											{ WeaponHash.Carbinerifle_mk2, 0x1757F566 },
											{ WeaponHash.Combatmg_mk2, 0xF6649745 },
											{ WeaponHash.Marksmanrifle_mk2, 0xD77A22D2 }
										}
		},

		// RIFLE_INCENDIARY_ROUNDS,
		{ EItemID.RIFLE_INCENDIARY_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0xA99CF95A },
											{ WeaponHash.Specialcarbine_mk2, 0xDE011286 },
											{ WeaponHash.Assaultrifle_mk2, 0xFB70D853 },
											{ WeaponHash.Carbinerifle_mk2, 0x3D25C2A7 },
											{ WeaponHash.Combatmg_mk2, 0xC326BDBA },
											{ WeaponHash.Marksmanrifle_mk2, 0x6DD7A86E },
											{ WeaponHash.Heavysniper_mk2, 0xEC0F617 },
										}
		},

		// RIFLE_HOLLOW_ROUNDS,
		{ EItemID.RIFLE_AP_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0xFAA7F5ED },
											{ WeaponHash.Specialcarbine_mk2, 0x51351635 },
											{ WeaponHash.Assaultrifle_mk2, 0xA7DD1E58 },
											{ WeaponHash.Carbinerifle_mk2, 0x255D5D57 },
											{ WeaponHash.Combatmg_mk2, 0x29882423 },
											{ WeaponHash.Marksmanrifle_mk2, 0xF46FD079 },
											{ WeaponHash.Heavysniper_mk2, 0xF835D6D4 },
										}
		},

		// RIFLE_FMJ_ROUNDS,
		{ EItemID.RIFLE_FMJ_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Bullpuprifle_mk2, 0x43621710 },
											{ WeaponHash.Specialcarbine_mk2, 0x503DEA90 },
											{ WeaponHash.Assaultrifle_mk2, 0x63E0A098 },
											{ WeaponHash.Carbinerifle_mk2, 0x44032F11 },
											{ WeaponHash.Combatmg_mk2, 0x57EF1CC8 },
											{ WeaponHash.Marksmanrifle_mk2, 0xE14A9ED3 },
											{ WeaponHash.Heavysniper_mk2, 0x3BE948F6 },
										}
		},

		// RIFLE_EXPLOSIVE_ROUNDS,
		{ EItemID.RIFLE_EXPLOSIVE_ROUNDS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Heavysniper_mk2, 0x89EBDAA7 }
										}
		},

		// SHOTGUN_DRAGONSBREATH_SHELLS,
		{ EItemID.SHOTGUN_DRAGONSBREATH_SHELLS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0x9F8A1BF5 }
										}
		},

		// SHOTGUN_BUCKSHOT_SHELLS,
		{ EItemID.SHOTGUN_STEEL_BUCKSHOT_SHELLS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0x4E65B425 }
										}
		},

		// SHOTGUN_FLECHETTE_SHELLS,
		{ EItemID.SHOTGUN_FLECHETTE_SHELLS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0xE9582927 }
										}
		},

		// SHOTGUN_EXPLOSIVE_SHELLS,
		{ EItemID.SHOTGUN_EXPLOSIVE_SHELLS, new Dictionary<WeaponHash, uint>()
										{
											{ WeaponHash.Pumpshotgun_mk2, 0x3BE4465D }
										}
		},
	};

#endif
}

public enum EFurnitureCategory
{
	Bathroom,
	Seating,
	Tables,
	Storage,
	Bedroom,
	Lights,
	Electronics,
	Miscellaneous,
	Kitchen,
	Decoration,
	Activities
}

public class CFurnitureDefinition
{
	public CFurnitureDefinition()
	{

	}

	public uint ID { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public EFurnitureCategory Category { get; set; }
	public float Price { get; set; }
	public bool Purchasable { get; set; }
	public string Model { get; set; }
	public int StorageCapacity { get; set; }
	public EActivityType Activity { get; set; } = EActivityType.None;
	public bool AllowOutfitChange { get; set; } // NOTE: Only works if storage capacity is zero
};

public enum EPetType
{
	None = -1,
	Boar,
	Cat,
	Chickenhawk,
	Chimp,
	Chop,
	Cormorant,
	Cow,
	Coyote,
	Crow,
	Deer,
	Dolphin,
	Fish,
	HammerheadShark,
	Hen,
	Humpback,
	Husky,
	Killerwhale,
	MountainLion,
	Sasquatch,
	Pig,
	Pigeon,
	Poodle,
	Pug,
	Rabbit,
	Rat,
	Retriever,
	RhesusMonkey,
	Rottweiler,
	Seagull,
	Shepherd,
	Stingray,
	Tigershark,
	Westy,
	Panther
}

public static class InventoryHelpers
{
	public static bool IsSocketAPlayerSocket(EItemSocket itemSocket)
	{
		return (itemSocket > EItemSocket.None && itemSocket <= EItemSocket.Clothing)
			|| itemSocket == EItemSocket.Furniture
			|| itemSocket == EItemSocket.LeftHand
			|| itemSocket == EItemSocket.RightHand
			|| itemSocket == EItemSocket.Wallet
			|| itemSocket == EItemSocket.Keyring
			|| itemSocket == EItemSocket.Outfit
			|| itemSocket == EItemSocket.ChestSling;
	}

	public static bool CanSocketBeIncludedInMerge(EItemSocket itemSocket)
	{
		return IsSocketAPlayerSocket(itemSocket);
	}

	public static bool IsSocketAVehicleSocket(EItemSocket itemSocket)
	{
		return (itemSocket >= EItemSocket.Vehicle_Trunk && itemSocket <= EItemSocket.Vehicle_Console_And_Glovebox);
	}

	public static bool IsUnlimitedContainerSocket(EItemSocket itemSocket)
	{
		return (itemSocket == EItemSocket.Clothing || itemSocket == EItemSocket.Wallet || itemSocket == EItemSocket.Keyring || itemSocket == EItemSocket.Furniture || itemSocket == EItemSocket.Outfit || itemSocket == EItemSocket.Property_Mailbox);
	}
}

public enum ECreditType
{
	Vehicle,
	Property
}

public class CreditDetails
{
	public CreditDetails(string a_strDisplayName, int a_numPaymentsMade, int a_numPaymentsRemaining, float a_fAmount, float a_fInterest, ECreditType a_CreditType, EntityDatabaseID a_ID)
	{
		strDisplayName = a_strDisplayName;
		numPaymentsMade = a_numPaymentsMade;
		numPaymentsRemaining = a_numPaymentsRemaining;
		fAmount = a_fAmount;
		CreditType = a_CreditType;
		ID = a_ID;
		fInterest = a_fInterest;
	}


	public string strDisplayName { get; set; }
	public int numPaymentsMade { get; set; }
	public int numPaymentsRemaining { get; set; }
	public float fAmount { get; set; }
	public float fInterest { get; set; }
	public ECreditType CreditType { get; set; }
	public EntityDatabaseID ID { get; set; }
}

public enum EVehicleSeat
{
	Driver = 0,
	FrontPassenger,
	RearLeftPassenger,
	RearRightPassenger,
	HangingLeft,
	HangingRight,
	HangingLeft2,
	HangingRight2
}

public enum EVehicleDoor
{
	Driver = 0,
	FrontPassenger,
	RearLeftPassenger,
	RearRightPassenger,
	Hood,
	Trunk
}

public class CPaycheckEntry
{
	public CPaycheckEntry(float a_fGrossIncome, string a_strSalaryName, float a_fStateIncomeTax, float a_fFederalIncomeTax, float a_fNetIncome)
	{
		GrossIncome = a_fGrossIncome;
		SalaryName = a_strSalaryName;
		StateIncomeTax = a_fStateIncomeTax;
		FederalIncomeTax = a_fFederalIncomeTax;
		NetIncome = a_fNetIncome;
	}

	public void AppendValues(ref float fTotalStateIncomeTax, ref float fTotalFederalIncomeTax, ref float fTotalGrossIncome, ref float fTotalNetIncome)
	{
		fTotalStateIncomeTax += StateIncomeTax;
		fTotalFederalIncomeTax += FederalIncomeTax;
		fTotalGrossIncome += GrossIncome;
		fTotalNetIncome += NetIncome;
	}

	public float GrossIncome { get; set; }
	public string SalaryName { get; set; }
	public float StateIncomeTax { get; set; }
	public float FederalIncomeTax { get; set; }
	public float NetIncome { get; set; }
}

public class PayDayVehicleOrPropertyDetails
{
	public PayDayVehicleOrPropertyDetails(bool a_bIsVehicle, string a_strDisplayName, float a_monthlyPayment, int a_paymentsRemaining, int a_paymentsMade, int a_paymentsMissed, bool a_bMissedPayment, bool a_bReposessed, float a_fMonthlyTax)
	{
		IsVehicle = a_bIsVehicle;
		DisplayName = a_strDisplayName;
		MonthlyPayment = a_monthlyPayment;
		PaymentsRemaining = a_paymentsRemaining;
		PaymentsMade = a_paymentsMade;
		PaymentsMissed = a_paymentsMissed;
		MissedPayment = a_bMissedPayment;
		Reposessed = a_bReposessed;
		MonthlyTax = a_fMonthlyTax;
	}

	public bool IsVehicle { get; set; }
	public string DisplayName { get; set; }
	public float MonthlyPayment { get; set; }
	public int PaymentsRemaining { get; set; }
	public int PaymentsMade { get; set; }
	public int PaymentsMissed { get; set; }
	public bool MissedPayment { get; set; }
	public bool Reposessed { get; set; }
	public float MonthlyTax { get; set; }
}


public class PayDayDetails
{
	public PayDayDetails(List<CPaycheckEntry> lstPaycheckEntries, float a_fTotalDonatorPerks)
	{
		m_lstPaycheckEntries = lstPaycheckEntries;
		TotalDonatorPerks = a_fTotalDonatorPerks;
		m_VehicleAndPropertiesData = new List<PayDayVehicleOrPropertyDetails>();
	}

	public void AddVehicleOrProperty(PayDayVehicleOrPropertyDetails details)
	{
		m_VehicleAndPropertiesData.Add(details);
	}

	public float TotalDonatorPerks { get; set; }

	public List<CPaycheckEntry> m_lstPaycheckEntries { get; set; }

	public List<PayDayVehicleOrPropertyDetails> m_VehicleAndPropertiesData { get; set; }

	public float TotalVehicleTaxSaved { get; set; }
	public float TotalPropertyTaxSaved { get; set; }
}

public enum EEntityType
{
	None,
	Vehicle,
	Property,
	Elevator,
	Faction
}

public static class MapTransferConstants
{
	public static int MaxMapSizeBytes = 307200; // 300kb

	public static MessagePack.MessagePackCompression CompressionAlgorithm = MessagePack.MessagePackCompression.Lz4Block;
}

public static class CRC32
{
	private static readonly uint[] ChecksumTable = new uint[0x100];
	private const uint Polynomial = 0xEDB88320;

	public const int CRCSize = sizeof(int);

	static CRC32()
	{
		for (uint index = 0; index < 0x100; ++index)
		{
			uint item = index;
			for (int bit = 0; bit < 8; ++bit)
				item = ((item & 1) != 0) ? (Polynomial ^ (item >> 1)) : (item >> 1);
			ChecksumTable[index] = item;
		}
	}

	public static byte[] ComputeHashAsBytes(byte[] bytes)
	{
		uint result = 0xFFFFFFFF;

		foreach (byte current in bytes)
		{
			result = ChecksumTable[(result & 0xFF) ^ current] ^ (result >> 8);
		}

		byte[] hash = BitConverter.GetBytes(~result);
		Array.Reverse(hash);
		return hash;
	}

	public static int ComputeHash(byte[] bytes)
	{
		byte[] crcbytes = ComputeHashAsBytes(bytes);
		return BitConverter.ToInt32(crcbytes, 0);
	}
}

public enum EActivityType
{
	None = -1,
	Blackjack
}

public enum ECardSuite
{
	Heart,
	Club,
	Spade,
	Diamond
}

public enum ECard
{
	Ace,
	Two,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
	Nine,
	Ten,
	Jack,
	Queen,
	King
}

public class CasinoCard
{
	public CasinoCard(ECard a_Card, ECardSuite a_Suite, bool a_bFaceUp)
	{
		Card = a_Card;
		Suite = a_Suite;
		FaceUp = a_bFaceUp;
	}

	public ECard Card { get; set; }
	public ECardSuite Suite { get; set; }
	public bool FaceUp { get; set; }
}

public static class ActivityConstants
{
	public const int BlackjackMaxParticipants = 4;
	public const int BlackjackMaxCards = 5;

	public static Dictionary<int, string> DictChipObjectHashesByValue = new Dictionary<int, string>()
	{
		{ 10, "vw_prop_chip_10dollar_x1" },
		{ 50, "vw_prop_chip_50dollar_x1" },
		{ 100, "vw_prop_chip_100dollar_x1" },
		{ 500, "vw_prop_chip_500dollar_x1" },
		{ 1000, "vw_prop_chip_1kdollar_x1" },
		{ 5000, "vw_prop_chip_5kdollar_x1" },
		{ 10000, "vw_prop_chip_10kdollar_x1" },
	};

	public static Dictionary<ECardSuite, Dictionary<ECard, string>> DictCardObjectHashes = new Dictionary<ECardSuite, Dictionary<ECard, string>>()
	{
		// SPADES
		{ ECardSuite.Spade, new Dictionary<ECard, string>()
			{
				{ ECard.Ace, "vw_prop_cas_card_spd_ace" },
				{ ECard.Two, "vw_prop_cas_card_spd_02" },
				{ ECard.Three, "vw_prop_cas_card_spd_03" },
				{ ECard.Four, "vw_prop_cas_card_spd_04" },
				{ ECard.Five, "vw_prop_cas_card_spd_05" },
				{ ECard.Six, "vw_prop_cas_card_spd_06" },
				{ ECard.Seven, "vw_prop_cas_card_spd_07" },
				{ ECard.Eight, "vw_prop_cas_card_spd_08" },
				{ ECard.Nine, "vw_prop_cas_card_spd_09" },
				{ ECard.Ten, "vw_prop_cas_card_spd_10" },
				{ ECard.Jack, "vw_prop_cas_card_spd_jack" },
				{ ECard.Queen, "vw_prop_cas_card_spd_queen" },
				{ ECard.King, "vw_prop_cas_card_spd_king" }
			}
		},

		// DIAMONDS
		{ ECardSuite.Diamond, new Dictionary<ECard, string>()
			{
				{ ECard.Ace, "vw_prop_cas_card_dia_ace" },
				{ ECard.Two, "vw_prop_cas_card_dia_02" },
				{ ECard.Three, "vw_prop_cas_card_dia_03" },
				{ ECard.Four, "vw_prop_cas_card_dia_04" },
				{ ECard.Five, "vw_prop_cas_card_dia_05" },
				{ ECard.Six, "vw_prop_cas_card_dia_06" },
				{ ECard.Seven, "vw_prop_cas_card_dia_07" },
				{ ECard.Eight, "vw_prop_cas_card_dia_08" },
				{ ECard.Nine, "vw_prop_cas_card_dia_09" },
				{ ECard.Ten, "vw_prop_cas_card_dia_10" },
				{ ECard.Jack, "vw_prop_cas_card_dia_jack" },
				{ ECard.Queen, "vw_prop_cas_card_dia_queen" },
				{ ECard.King, "vw_prop_cas_card_dia_king" }
			}
		},

		// HEARTS
		{ ECardSuite.Heart, new Dictionary<ECard, string>()
			{
				{ ECard.Ace, "vw_prop_cas_card_hrt_ace" },
				{ ECard.Two, "vw_prop_cas_card_hrt_02" },
				{ ECard.Three, "vw_prop_cas_card_hrt_03" },
				{ ECard.Four, "vw_prop_cas_card_hrt_04" },
				{ ECard.Five, "vw_prop_cas_card_hrt_05" },
				{ ECard.Six, "vw_prop_cas_card_hrt_06" },
				{ ECard.Seven, "vw_prop_cas_card_hrt_07" },
				{ ECard.Eight, "vw_prop_cas_card_hrt_08" },
				{ ECard.Nine, "vw_prop_cas_card_hrt_09" },
				{ ECard.Ten, "vw_prop_cas_card_hrt_10" },
				{ ECard.Jack, "vw_prop_cas_card_hrt_jack" },
				{ ECard.Queen, "vw_prop_cas_card_hrt_queen" },
				{ ECard.King, "vw_prop_cas_card_hrt_king" }
			}
		},

		// CLUBS
		{ ECardSuite.Club, new Dictionary<ECard, string>()
			{
				{ ECard.Ace, "vw_prop_cas_card_club_ace" },
				{ ECard.Two, "vw_prop_cas_card_club_02" },
				{ ECard.Three, "vw_prop_cas_card_club_03" },
				{ ECard.Four, "vw_prop_cas_card_club_04" },
				{ ECard.Five, "vw_prop_cas_card_club_05" },
				{ ECard.Six, "vw_prop_cas_card_club_06" },
				{ ECard.Seven, "vw_prop_cas_card_club_07" },
				{ ECard.Eight, "vw_prop_cas_card_club_08" },
				{ ECard.Nine, "vw_prop_cas_card_club_09" },
				{ ECard.Ten, "vw_prop_cas_card_club_10" },
				{ ECard.Jack, "vw_prop_cas_card_club_jack" },
				{ ECard.Queen, "vw_prop_cas_card_club_queen" },
				{ ECard.King, "vw_prop_cas_card_club_king" }
			}
		}
	};
}

public static class BlackjackTimeouts
{
	public const Int64 timeForBets = 30000;
	public const Int64 timeBetweenCardDeals = 4000;
	public const Int64 timeBetweenActionStates = 30000;
	public const Int64 timeToDetermineRoundOutcome = 5000;
	public const Int64 timeToSpendInRoundOutcome = 15000;
	public const Int64 timeBetweenStandOn17Deals = 5000;
	public const Int64 timeToSpendRetrievingCards = 2000;
}

// STATES
public abstract class ActivityState
{

}

// TODO_ACTIVITY_LOW_PRIO: Could be in base class to reduce for cards? or an interim class like CardBasedActivityState
public class BlackjackPlayerState
{
	public int CurrentBet { get; set; } = 0;
	public int Chips { get; set; } = 0;
	public List<CasinoCard> Cards { get; set; } = new List<CasinoCard>();

	public bool FirstTimeFlag_IsBust { get; set; } = false;
	public bool FirstTimeFlag_HasBlackjack { get; set; } = false;

	public bool IsParticipatingInCurrentRound()
	{
		return CurrentBet > 0;
	}

	public void ResetPerRoundVariables()
	{
		Cards.Clear();
		CurrentBet = 0;
		FirstTimeFlag_IsBust = false;
		FirstTimeFlag_HasBlackjack = false;
	}
}

public enum EBlackJackActivityState
{
	Inactive,
	PlaceBets_Init,
	PlaceBets_Wait,
	DealCard_Player1_1_Camera,
	DealCard_Player1_1_WaitForIssue,
	DealCard_Player2_1_Camera,
	DealCard_Player2_1_WaitForIssue,
	DealCard_Player3_1_Camera,
	DealCard_Player3_1_WaitForIssue,
	DealCard_Player4_1_Camera,
	DealCard_Player4_1_WaitForIssue,
	DealCard_Dealer_1_Camera,
	DealCard_Dealer_1_WaitForIssue,
	DealCard_Player1_2_Camera,
	DealCard_Player1_2_WaitForIssue,
	DealCard_Player2_2_Camera,
	DealCard_Player2_2_WaitForIssue,
	DealCard_Player3_2_Camera,
	DealCard_Player3_2_WaitForIssue,
	DealCard_Player4_2_Camera,
	DealCard_Player4_2_WaitForIssue,
	DealCard_Dealer_2_Camera,
	DealCard_Dealer_2_WaitForIssue,
	Player1_MakeChoice,
	Player1_Wait,
	Player2_MakeChoice,
	Player2_Wait,
	Player3_MakeChoice,
	Player3_Wait,
	Player4_MakeChoice,
	Player4_Wait,
	Dealer_TurnSecondCard,
	Dealer_TurnSecondCard_Wait,
	Dealer_StandOn17,
	Dealer_StandOn17_Wait,
	DetermineRoundOutcome,
	DetermineRoundOutcome_Wait,
	DetermineRoundOutcome_RetrieveCards_Player1,
	DetermineRoundOutcome_RetrieveCards_Player2,
	DetermineRoundOutcome_RetrieveCards_Player3,
	DetermineRoundOutcome_RetrieveCards_Player4,
	DetermineRoundOutcome_RetrieveCards_Dealer,
	DetermineRoundOutcome_WaitingToGotoInactiveAndNextRound
}

public enum EBlackJackAnimations
{
	CheckOwnCard_Dealer,
	CheckAndTurnOwnCard_Dealer,
	DealCard_Player1,
	DealCard_Player2,
	DealCard_Player3,
	DealCard_Player4,
	DealCard_Dealer,
	DealerFocusPlayer1_Idle,
	//DealerFocusPlayer1_Idle_Impatient,
	DealerFocusPlayer2_Idle,
	//DealerFocusPlayer2_Idle_Impatient,
	DealerFocusPlayer3_Idle,
	//DealerFocusPlayer3_Idle_Impatient,
	DealerFocusPlayer4_Idle,
	//DealerFocusPlayer4_Idle_Impatient,
	DealerIdle,
	HitCardPlayer1,
	HitCardPlayer2,
	HitCardPlayer3,
	HitCardPlayer4,
	//HitSecondCardPlayer1,
	//HitSecondCardPlayer2,
	//HitSecondCardPlayer3,
	//HitSecondCardPlayer4,
	PlaceBet,
	RetrieveCardsPlayer1,
	RetrieveCardsPlayer2,
	RetrieveCardsPlayer3,
	RetrieveCardsPlayer4,
	RetrieveDealerCards,
	TurnCard
}


public class BlackjackActivityState : ActivityState
{
	public EBlackJackActivityState State { get; set; } = EBlackJackActivityState.Inactive;
	public BlackjackPlayerState DealerState { get; set; } = new BlackjackPlayerState();
	public Dictionary<int, BlackjackPlayerState> PlayerStates { get; set; } = new Dictionary<int, BlackjackPlayerState>();
}

public static class BlackjackActivityHelpers
{
	public const int BlackjackValue = 21;
	public const int DealerStandValue = 17;

	public static Dictionary<ECard, int> dictCardValues = new Dictionary<ECard, int>()
	{
		{ ECard.Ace, 1 },
		{ ECard.Two, 2 },
		{ ECard.Three, 3 },
		{ ECard.Four, 4 },
		{ ECard.Five, 5 },
		{ ECard.Six, 6 },
		{ ECard.Seven, 7 },
		{ ECard.Eight, 8 },
		{ ECard.Nine, 9 },
		{ ECard.Ten, 10 },
		{ ECard.Jack, 10 },
		{ ECard.Queen, 10 },
		{ ECard.King, 10 }
	};

	public const string AnimationDictionary = "anim_casino_b@amb@casino@games@blackjack@dealer_female";
	public static Dictionary<EBlackJackAnimations, string> Animations = new Dictionary<EBlackJackAnimations, string>()
	{
		{ EBlackJackAnimations.CheckOwnCard_Dealer, "check_card" },
		{ EBlackJackAnimations.CheckAndTurnOwnCard_Dealer, "check_and_turn_card" },
		{ EBlackJackAnimations.DealCard_Player1, "deal_card_player_04" }, // NOTE: our player slots are inverted!
		{ EBlackJackAnimations.DealCard_Player2, "deal_card_player_03" },
		{ EBlackJackAnimations.DealCard_Player3, "deal_card_player_03" },
		{ EBlackJackAnimations.DealCard_Player4, "deal_card_player_01" },
		{ EBlackJackAnimations.DealCard_Dealer, "deal_card_self" },
		{ EBlackJackAnimations.DealerFocusPlayer1_Idle, "dealer_focus_player_04_idle" },
		{ EBlackJackAnimations.DealerFocusPlayer2_Idle, "dealer_focus_player_03_idle" },
		{ EBlackJackAnimations.DealerFocusPlayer3_Idle, "dealer_focus_player_02_idle" },
		{ EBlackJackAnimations.DealerFocusPlayer4_Idle, "dealer_focus_player_01_idle" },
		{ EBlackJackAnimations.DealerIdle, "dealer_idle" },
		{ EBlackJackAnimations.HitCardPlayer1, "hit_card_player_04" },
		{ EBlackJackAnimations.HitCardPlayer2, "hit_card_player_03" },
		{ EBlackJackAnimations.HitCardPlayer3, "hit_card_player_02" },
		{ EBlackJackAnimations.HitCardPlayer4, "hit_card_player_01" },
		{ EBlackJackAnimations.PlaceBet, "place_bet_request" },
		{ EBlackJackAnimations.RetrieveCardsPlayer1, "retrieve_cards_player_04" },
		{ EBlackJackAnimations.RetrieveCardsPlayer2, "retrieve_cards_player_03" },
		{ EBlackJackAnimations.RetrieveCardsPlayer3, "retrieve_cards_player_02" },
		{ EBlackJackAnimations.RetrieveCardsPlayer4, "retrieve_cards_player_01" },
		{ EBlackJackAnimations.RetrieveDealerCards, "retrieve_own_cards_and_remove" },
		{ EBlackJackAnimations.TurnCard, "turn_card" },
	};

	public static void CalculateValueOfCards(List<CasinoCard> lstCards, out List<string> lstDisplays, out List<int> lstValues, out List<int> lstValuesWithinMaxRange, out string strValidCardCombinations, out int highestValueWithinMaxRange)
	{
		// TODO_ACTIVITY_LOW_PRIO: if aces, dont show combinations > blackjack, unless its the only one (impossible?)

		lstDisplays = new List<string>();
		lstValues = new List<int>();
		lstValuesWithinMaxRange = new List<int>();
		highestValueWithinMaxRange = -1;

		// get number of aces
		int numAces = 0;

		foreach (CasinoCard card in lstCards)
		{
			if (card.Card == ECard.Ace)
			{
				++numAces;
			}
		}

		// calculate all combinations
		int numCombinations = numAces + 1;
		for (int i = 0; i < numCombinations; ++i)
		{
			int expectedAcesAs11 = i;

			int numAcesProcessed = 0;
			int value = 0;
			string strDisplay = String.Empty;

			int cardIndex = 0;
			foreach (CasinoCard card in lstCards)
			{
				if (cardIndex > 0)
				{
					strDisplay += " + ";
				}

				if (card.FaceUp)
				{
					if (card.Card == ECard.Ace)
					{
						if (numAcesProcessed < expectedAcesAs11)
						{
							strDisplay += Helpers.FormatString("{0} (11)", card.Card.ToString());
							value += 11;

							++numAcesProcessed;
						}
						else
						{
							strDisplay += Helpers.FormatString("{0} (1)", card.Card.ToString());
							value += 1;
						}
					}
					else
					{
						strDisplay += Helpers.FormatString("{0}", card.Card.ToString());
						value += dictCardValues[card.Card];
					}
				}
				else
				{
					strDisplay += "Face Down Card";
					// dont add value here so they dont know
				}


				++cardIndex;
			}

			lstDisplays.Add(strDisplay);
			lstValues.Add(value);

			if (value > highestValueWithinMaxRange)
			{
				if (value <= BlackjackActivityHelpers.BlackjackValue)
				{
					highestValueWithinMaxRange = value;
				}
			}

			if (value <= BlackjackActivityHelpers.BlackjackValue)
			{
				lstValuesWithinMaxRange.Add(value);
			}
		}

		// generate value string
		strValidCardCombinations = lstValuesWithinMaxRange.Count > 1 ? "Hand Values: " : "Hand Value: ";
		for (int i = 0; i < lstValuesWithinMaxRange.Count; ++i)
		{
			if (i > 0)
			{
				strValidCardCombinations += ", ";
			}

			strValidCardCombinations += lstValuesWithinMaxRange[i].ToString();
		}

		if (lstValuesWithinMaxRange.Count == 0)
		{
			strValidCardCombinations = "Bust";
		}
	}
}

public class RoadblockDefinition
{
	public RoadblockDefinition(string a_strDisplayName, string a_strHashKey)
	{
		DisplayName = a_strDisplayName;
		HashKey = a_strHashKey;
	}

	public string DisplayName { get; }
	public string HashKey { get; }
}


public static class Roadblocks
{
	public static List<RoadblockDefinition> Definitions = new List<RoadblockDefinition>()
	{
		new RoadblockDefinition("Police Barrier", "prop_barrier_work05"),
		new RoadblockDefinition("Double Red Light", "prop_air_lights_04a"),
		new RoadblockDefinition("Small Red Light", "prop_air_lights_02b"),
		new RoadblockDefinition("Big Blue Light", "prop_air_lights_05a"),
		new RoadblockDefinition("Tall Blue Light", "prop_air_lights_02a"),
		new RoadblockDefinition("Blue Cone Light", "prop_air_conelight"),
		new RoadblockDefinition("Air Lights", "prop_air_lights_03a"),
		new RoadblockDefinition("Big Red Light", "prop_air_lights_02b"),
		new RoadblockDefinition("Red Warning Light", "prop_warninglight_01"),
		new RoadblockDefinition("Crash Barrier", "prop_barriercrash_04"),
		new RoadblockDefinition("Small Construction Barrier", "prop_barrier_work01a"),
		new RoadblockDefinition("Road Work Ahead", "prop_barrier_work04a"),
		new RoadblockDefinition("Simple Barrier", "prop_barrier_work06a"),
		new RoadblockDefinition("Simple Barrier w/ Light", "prop_barrier_work02a"),
		new RoadblockDefinition("Simple Barrier", "prop_barrier_work01c"),
		new RoadblockDefinition("Water Barrier", "prop_barrier_wat_03b"),
		new RoadblockDefinition("Water Barrier 2", "prop_barrier_wat_03a"),
		new RoadblockDefinition("Large Crash Barrier", "prop_barriercrash_03"),
		new RoadblockDefinition("Road Work Ahead", "prop_barrier_work06b"),
		new RoadblockDefinition("Water Barrier 3", "prop_barrier_wat_04b"),
		new RoadblockDefinition("Uneven Pavement", "prop_barrier_work01d"),
		new RoadblockDefinition("Concrete Hazard Barrier", "prop_mc_conc_barrier_01"),
		new RoadblockDefinition("Small Barrier", "prop_barriercrash_02"),
		new RoadblockDefinition("Work Barrier", "prop_barrier_work01b"),
		new RoadblockDefinition("Water Barrier 4", "prop_barrier_wat_04a"),
		new RoadblockDefinition("Security Barrier ", "prop_sec_barrier_ld_02a"),
		new RoadblockDefinition("Security Barrier", "prop_sec_barrier_ld_01a"),
		new RoadblockDefinition("Concrete Barrier", "prop_mp_barrier_01"),
		new RoadblockDefinition("Concrete Barrier Big", "prop_mp_barrier_01b"),
		new RoadblockDefinition("Barrier", "prop_ld_barrier_01"),
		new RoadblockDefinition("Barrier", "prop_mp_barrier_02b"),
		new RoadblockDefinition("Barrier with arrow", "prop_mp_arrow_barrier_01"),
		new RoadblockDefinition("Square of barriers", "prop_air_barrier"),
		new RoadblockDefinition("Concrete Warning Barrier", "prop_mp_conc_barrier_01"),
		new RoadblockDefinition("Worklight 1", "prop_worklight_03a"),
		new RoadblockDefinition("Worklight 2", "prop_worklight_01a"),
		new RoadblockDefinition("Worklight 3", "prop_worklight_04b"),
		new RoadblockDefinition("Worklight 4", "prop_worklight_04d"),
		new RoadblockDefinition("Worklight 5", "prop_worklight_04c_l1"),
		new RoadblockDefinition("Worklight 6", "prop_worklight_03b"),
		new RoadblockDefinition("Worklight 7", "prop_worklight_02a"),
		new RoadblockDefinition("Road Cone 1", "prop_mp_cone_01"),
		new RoadblockDefinition("Road Cone 2", "prop_mp_cone_02"),
		new RoadblockDefinition("Road Cone 3", "prop_mp_cone_03"),
		new RoadblockDefinition("Road Cone 4", "prop_mp_cone_04"),
		new RoadblockDefinition("Road Cone 5", "prop_roadcone01b"),
		new RoadblockDefinition("Road Cone 6", "prop_roadcone01c"),
		new RoadblockDefinition("Road Cone 7", "prop_roadcone02b")
	};
}

public enum PlayerMoneyModificationReason
{
	PayCheck,
	VehicleMonthlyPayment,
	VehicleMonthlyTax,
	PropertyMonthlyPayment,
	PropertyMonthlyTax,
	PhoneCall,
	EMSRespawn,
	BailOut,
	PDTicket,
	Generics,
	Locksmith,
	SellDrugs,
	CharacterChange,
	ScriptedStoreRobbery,
	TattooArtistCheckout,
	RentalCarCheckout,
	VehicleStoreCheckout,
	Advert,
	CarWash,
	DrivingTest,
	FuelStation,
	CrushVehicle,
	VehicleModShop,
	VehicleRepair,
	DancerTip,
	DancerTip_Receive,
	PlasticSurgeonCheckout,
	FurnitureStoreCheckout,
	StoreCheckout,
	StoreOwner_ReceiveMoney,
	SellFish,
	BarberCheckout,
	ClothingStoreCheckout,
	Charity,
	BankCharity,
	PayCommand_Sender,
	PayCommand_Receiver,
	Bank_Withdraw,
	Bank_Deposit,
	Bank_WithdrawFromFaction,
	Bank_DepositToFaction,
	AddMoneyAdminCMD,
	TakeMoneyAdminCMD,
	SetMoneyAdminCMD,
	AddBankMoneyAdminCMD,
	TakeBankMoneyAdminCMD,
	SetBankMoneyAdminCMD,
	UnimpoundCar,
	PropertyPayDown,
	VehiclePayDown,
	Bank_WireTransfer_ToPlayer,
	Bank_WireTransfer_ToFaction,
	Bank_WireTransfer_FromPlayer,
	Bank_WireTransfer_FromFaction,
	SellProperty_ToGovernment,
	BuyProperty_Outright,
	BuyProperty_Downpayment,
	Activity_BuyCasinoChips,
	Activity_SellCasinoChips,
	ActivityManagement_AddCurrency,
	ActivityManagement_TakeCurrency,
	InactivityScanner_VehicleRefund,
	InactivityScanner_PropertyRefund,
	ArrestFine
}

public enum EShowInventoryAction
{
	DoNothing,
	Show,
	HideIfVisible
}

public enum ELargeDataTransferType
{
	Test,
	MapData_Persistent,
	MapData_OnDemand,
	PerfCapture
}

public static class PerformanceCaptureConstants
{
	public static byte[] EncryptionKey = new byte[] { 0x12, 0x34, 0x51, 0x08 };
}


public enum ELargeDataTransferState
{
	PendingAck,
	InProgress_PendingAck,
	InProgress_Ready,
	DoneWaitingAck,
	DoneAndAcked
}

public static class LargeTransferConstants
{
	public const int MaxBytesPer100ms = 6144; // 6kb (so 60kb/sec, 5 sec for a 300kb map - the max size for a map)
}

public static class PDConstants
{
	public const int VSPDInteriorID = 290;
}

public static class MaskHelpers
{
	public const int ChristmasMask = 8;
	public const int HalloweenMask = 71;

	public static int[] MasksFunctioningAsBeards = { 0, 121, 127 }; // 125, 122 and 126 are buggy :( gives player a mask when combined with certain clothing items (e.g. top 242, undershirt 130)

	public static int[] MasksWhichCoverHair = { 1, 2, 3, 5, 7, 8, 9, 10, 13, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 41, 42, 33, 34, 35, 37, 39, 40, 41, 42, 43, 44, 45, 47, 48, 49, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65,
		66, 67, 69, 69, 70, 71, 72, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 102, 103, 104, 105, 106, 109, 110, 112, 113, 114, 115, 116, 117, 118, 119, 123, 124, 129,
		130, 131, 132, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 162, 163, 170, 171, 172, 173, 174, 176, 177, 178, 180, 181, 182,  184 };
}

public enum TattooZone
{
	ZONE_TORSO = 0,
	ZONE_HEAD = 1,
	ZONE_LEFT_ARM = 2,
	ZONE_RIGHT_ARM = 3,
	ZONE_LEFT_LEG = 4,
	ZONE_RIGHT_LEG = 5,
	ZONE_UNKNOWN = 6,
	ZONE_NONE = 7,
};

public enum ETrainType
{
	Metro,
	Freight
}

public enum ETrainTripWireType
{
	ApproachingStation,
	ApplyBrakes,
	StationStop
}

public class CTrainTripWire
{
	public CTrainTripWire(int a_SectorID, ETrainTripWireType type, Vector3 vecPos, string strName, bool bDirection, Vector3 vecExitPos = null)
	{
		Sector = a_SectorID;
		TripWireType = type;
		Position = vecPos;
		Name = strName;
		Direction = bDirection ? "North Line" : "South Line";
		ExitPosition = vecExitPos;
	}

	public int Sector { get; set; }
	public ETrainTripWireType TripWireType { get; set; }
	public Vector3 Position { get; set; }
	public string Name { get; set; }
	public string Direction { get; set; }
	public Vector3 ExitPosition { get; set; }
}

public static class TrainConstants
{
	public const float MaxSpeed = 40.0f;
	public const float SpeedIncrements = 5.0f;
	public const float TripWireThreshold = 2.0f;

	public const int PureSyncUpdatesPerSecond = 10;

	public static List<CTrainTripWire> TripWires = new List<CTrainTripWire>
	{
		// Station 1 NORTH
		new CTrainTripWire(0, ETrainTripWireType.ApproachingStation, new Vector3(68.5143f, -1199.5667f, 35.34318f), "Pillbox North & Puerto Del Sol", true),
		new CTrainTripWire(0, ETrainTripWireType.ApplyBrakes, new Vector3(317.02005f, -1198.4636f, 38.07329f), "Pillbox North & Puerto Del Sol", true),
		new CTrainTripWire(0, ETrainTripWireType.StationStop, new Vector3(273.07898f, -1198.4647f, 38.075672f), "Pillbox North & Puerto Del Sol", true, new Vector3(275.15103f, -1202.5717f, 38.897297f)),

		// Station 2 NORTH
		new CTrainTripWire(1, ETrainTripWireType.ApproachingStation, new Vector3(-506.0325f, -1221.218f, 27.692663f), "Airport & Strawberry", true),
		new CTrainTripWire(1, ETrainTripWireType.ApplyBrakes, new Vector3(-526.1083f, -1241.0474f, 26.252419f), "Airport & Strawberry", true),
		new CTrainTripWire(1, ETrainTripWireType.StationStop, new Vector3(-547.4404f, -1290.7961f, 25.906128f), "Airport & Strawberry", true, new Vector3(-543.367f, -1285.8303f, 26.901588f)),

		// Station 3 NORTH
		new CTrainTripWire(2, ETrainTripWireType.ApproachingStation, new Vector3(-546.12573f, -1289.222f, 26.9016f), "LSI Airport Parking", true),
		new CTrainTripWire(2, ETrainTripWireType.ApplyBrakes, new Vector3(-546.12573f, -1289.222f, 26.9016f), "LSI Airport Parking", true),
		new CTrainTripWire(2, ETrainTripWireType.StationStop, new Vector3(-546.12573f, -1289.222f, 26.9016f), "LSI Airport Parking", true, new Vector3(-886.10876f, -2318.8704f, -11.732738f)),

		// Station 4 SOUTH
		new CTrainTripWire(3, ETrainTripWireType.ApproachingStation, new Vector3(-793.8265f, -2194.264f, -7.158527f), "LSI Airport Parking", false),
		new CTrainTripWire(3, ETrainTripWireType.ApplyBrakes, new Vector3(-873.3293f, -2273.137f, -11.95641f), "LSI Airport Parking", false),
		new CTrainTripWire(3, ETrainTripWireType.StationStop, new Vector3(-891.6512f, -2321.971f, -11.95729f), "LSI Airport Parking", false, new Vector3(-885.4503f, -2316.2188f, -11.732789f)),

		// Station 5 SOUTH
		new CTrainTripWire(4, ETrainTripWireType.ApproachingStation, new Vector3(-990.6615f, -2633.009f, -7.635078f), "LSI Airport Terminal 4", false),
		new CTrainTripWire(4, ETrainTripWireType.ApplyBrakes, new Vector3(-1058.815f, -2676.251f, -7.636047f), "LSI Airport Terminal 4", false),
		new CTrainTripWire(4, ETrainTripWireType.StationStop, new Vector3(-1094.851f, -2718.784f, -7.635012f), "LSI Airport Terminal 4", false, new Vector3(-1085.3671f, -2713.9292f, -7.410072f)),

		// Station 6 SOUTH
		new CTrainTripWire(5, ETrainTripWireType.ApproachingStation, new Vector3(-1137.216f, -2826.418f, -7.634951f), "LSI Airport Terminal 4", false),
		new CTrainTripWire(5, ETrainTripWireType.ApplyBrakes, new Vector3(-1110.205f, -2758.427f, -7.634838f), "LSI Airport Terminal 4", false),
		new CTrainTripWire(5, ETrainTripWireType.StationStop, new Vector3(-1078.1027f, -2720.788f, -8.297113f), "LSI Airport Terminal 4", false, new Vector3(-1077.6323f, -2712.4775f, -7.410072f)),

		// Station 7 SOUTH
		new CTrainTripWire(6, ETrainTripWireType.ApproachingStation, new Vector3(-876.81f, -2478.901f, -11.91769f), "LSI Airport Parking", false),
		new CTrainTripWire(6, ETrainTripWireType.ApplyBrakes, new Vector3(-894.114f, -2368.268f, -11.95768f), "LSI Airport Parking", false),
		new CTrainTripWire(6, ETrainTripWireType.StationStop, new Vector3(-878.8309f, -2326.467f, -11.95472f), "LSI Airport Parking", false, new Vector3(-887.7345f, -2339.4683f, -11.732736f)),

		// Station 8 SOUTH
		new CTrainTripWire(7, ETrainTripWireType.ApproachingStation, new Vector3(-619.1251f, -1479.31f, 17.44181f), "Airport & Strawberry", false),
		new CTrainTripWire(7, ETrainTripWireType.ApplyBrakes, new Vector3(-562.7336f, -1334.6511f, 24.705248f), "Airport & Strawberry", false),
		new CTrainTripWire(7, ETrainTripWireType.StationStop, new Vector3(-536.5131f, -1282.887f, 26.59014f), "Airport & Strawberry", false, new Vector3(-542.382f, -1288.1743f, 26.901588f)),

		// Station 9 SOUTH
		new CTrainTripWire(8, ETrainTripWireType.ApproachingStation, new Vector3(-13.37188f, -1217.998f, 38.24228f), "Pillbox North & Puerto Del Sol", false),
		new CTrainTripWire(8, ETrainTripWireType.ApplyBrakes, new Vector3(211.8274f, -1209.184f, 38.7354f), "Pillbox North & Puerto Del Sol", false),
		new CTrainTripWire(8, ETrainTripWireType.StationStop, new Vector3(271.6958f, -1209.158f, 38.76118f), "Pillbox North & Puerto Del Sol", false, new Vector3(273.74048f, -1205.5459f, 38.899765f)),

		// Station 10 SOUTH
		new CTrainTripWire(9, ETrainTripWireType.ApproachingStation, new Vector3(-210.127f, -451.9084f, 9.859692f), "Burton", false),
		new CTrainTripWire(9, ETrainTripWireType.ApplyBrakes, new Vector3(-288.1129f, -379.4066f, 9.838596f), "Burton", false),
		new CTrainTripWire(9, ETrainTripWireType.StationStop, new Vector3(-287.915f, -339.1162f, 9.838324f), "Burton", false, new Vector3(-291.69073f, -328.42697f, 10.06315f)),

		// Station 11 SOUTH
		new CTrainTripWire(10, ETrainTripWireType.ApproachingStation, new Vector3(-656.3364f, -105.4386f, 19.72595f), "Portola Drive", false),
		new CTrainTripWire(10, ETrainTripWireType.ApplyBrakes, new Vector3(-777.9604f, -108.7708f, 19.72595f), "Portola Drive", false),
		new CTrainTripWire(10, ETrainTripWireType.StationStop, new Vector3(-812.6067f, -127.45895f, 19.05591f), "Portola Drive", false, new Vector3(-812.8491f, -133.80371f, 19.9503f)),
		
		// Del Perro
		new CTrainTripWire(11, ETrainTripWireType.ApproachingStation, new Vector3(-1340.634f, -337.149f, 14.82091f), "Del Perro", false),
		new CTrainTripWire(11, ETrainTripWireType.ApplyBrakes, new Vector3(-1381.343f, -423.6795f, 14.81992f), "Del Perro", false),
		new CTrainTripWire(11, ETrainTripWireType.StationStop, new Vector3(-1361.56f, -461.0657f, 14.82045f), "Del Perro", false, new Vector3(-1358.3801f, -458.7637f, 15.045317f)),

		// Little Seoul
		new CTrainTripWire(12, ETrainTripWireType.ApproachingStation, new Vector3(-665.0348f, -695.3074f, 11.58494f), "Little Seoul", false),
		new CTrainTripWire(12, ETrainTripWireType.ApplyBrakes, new Vector3(-544.8835f, -679.6088f, 11.58564f), "Little Seoul", false),
		new CTrainTripWire(12, ETrainTripWireType.StationStop, new Vector3(-498.3536f, -679.8986f, 11.5851f), "Little Seoul", false, new Vector3(-491.62836f, -674.43945f, 11.809021f)),

		// Pillbox South
		new CTrainTripWire(13, ETrainTripWireType.ApproachingStation, new Vector3(-174.7711f, -862.6757f, 21.32765f), "Pillbox South", false),
		new CTrainTripWire(13, ETrainTripWireType.ApplyBrakes, new Vector3(-201.9329f, -990.3533f, 29.57914f), "Pillbox South", false),
		new CTrainTripWire(13, ETrainTripWireType.StationStop, new Vector3(-218.4626f, -1035.709f, 30.01372f), "Pillbox South", false, new Vector3(-214.19261f, -1037.7529f, 30.139889f)),

		// Davis
		new CTrainTripWire(14, ETrainTripWireType.ApproachingStation, new Vector3(-103.2644f, -1544.763f, 34.39619f), "Davis", false),
		new CTrainTripWire(14, ETrainTripWireType.ApplyBrakes, new Vector3(77.98711f, -1697.417f, 29.78064f), "Davis", false),
		new CTrainTripWire(14, ETrainTripWireType.StationStop, new Vector3(118.6346f, -1732.97f, 29.7415f), "Davis", false, new Vector3(119.41951f, -1730.2153f, 30.111513f)),

		// Davis again
		new CTrainTripWire(15, ETrainTripWireType.ApproachingStation, new Vector3(264.8557f, -1848.788f, 19.51463f), "Davis", true),
		new CTrainTripWire(15, ETrainTripWireType.ApplyBrakes, new Vector3(158.7672f, -1759.128f, 29.80586f), "Davis", true),
		new CTrainTripWire(15, ETrainTripWireType.StationStop, new Vector3(111.8464f, -1718.353f, 29.81675f), "Davis", true, new Vector3(109.435005f, -1719.877f, 30.113262f)),

		// pillbox again
		new CTrainTripWire(16, ETrainTripWireType.ApproachingStation, new Vector3(-251.8382f, -1167.566f, 29.84963f), "Pillbox", true),
		new CTrainTripWire(16, ETrainTripWireType.ApplyBrakes, new Vector3(-233.0271f, -1088.468f, 29.84811f), "Pillbox", true),
		new CTrainTripWire(16, ETrainTripWireType.StationStop, new Vector3(-208.2553f, -1030.699f, 30.01384f), "Pillbox", true, new Vector3(-210.50356f, -1029.423f, 30.139036f)),

		// seoul again
		new CTrainTripWire(17, ETrainTripWireType.ApproachingStation, new Vector3(-370.4215f, -669.957f, 11.58475f), "Little Seoul", true),
		new CTrainTripWire(17, ETrainTripWireType.ApplyBrakes, new Vector3(-447.4201f, -666.6638f, 11.58508f), "Little Seoul", true),
		new CTrainTripWire(17, ETrainTripWireType.StationStop, new Vector3(-502.3734f, -666.3566f, 11.58487f), "Little Seoul", true, new Vector3(-502.58762f, -670.0882f, 11.809643f)),

		// del perro again
		new CTrainTripWire(18, ETrainTripWireType.ApproachingStation, new Vector3(-1246.168f, -551.2654f, 13.06057f), "Del Perro", true),
		new CTrainTripWire(18, ETrainTripWireType.ApplyBrakes, new Vector3(-1318.69f, -508.7895f, 14.82095f), "Del Perro", true),
		new CTrainTripWire(18, ETrainTripWireType.StationStop, new Vector3(-1340.599f, -470.367f, 14.82079f), "Del Perro", true, new Vector3(-1340.5896f, -475.37238f, 15.002015f)),

		// portola again
		new CTrainTripWire(19, ETrainTripWireType.ApproachingStation, new Vector3(-957.8483f, -219.7883f, 19.72589f), "Portola Drive", true),
		new CTrainTripWire(19, ETrainTripWireType.ApplyBrakes, new Vector3(-863.3777f, -172.1579f, 19.72585f), "Portola Drive", true),
		new CTrainTripWire(19, ETrainTripWireType.StationStop, new Vector3(-823.8882f, -150.3994f, 19.72563f), "Portola Drive", true, new Vector3(-817.1548f, -141.6193f, 19.95034f)),

		// burton again
		new CTrainTripWire(20, ETrainTripWireType.ApproachingStation, new Vector3(-403.1715f, -196.0468f, 9.837678f), "Burton", true),
		new CTrainTripWire(20, ETrainTripWireType.ApplyBrakes, new Vector3(-301.353f, -280.2737f, 9.838572f), "Burton", true),
		new CTrainTripWire(20, ETrainTripWireType.StationStop, new Vector3(-301.4424f, -319.6911f, 9.838763f), "Burton", true, new Vector3(-296.7642f, -324.63538f, 10.063151f)),
	};
}

public static class InformationMarkerConstants
{
	public const int MAX_INFOMARKER_TEXT_LENGTH = 500;
	public const float NEARBY_INFOMARKERS_LIMIT = 30.0f;
}

public class SVehicle
{
	public SVehicle(long id, string name)
	{
		Id = id;
		Name = name;
	}

	public long Id { get; }
	public string Name { get; }
}

public class SProperty
{
	public SProperty(long id, string name)
	{
		Id = id;
		Name = name;
	}

	public long Id { get; }
	public string Name { get; }
}

public enum EClientsideDebugOption
{
	DrawStreamedEntitiesCount = 0,
	DrawPlayerBox = 1,
}