using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;
using Newtonsoft.Json.Linq;
using Color = System.Drawing.Color;

#if SERVER
using GTANetworkAPI;
#else
using RAGE;
#endif

// Useful for reserializing when adding new variables (or rolling the version)
/*
public class CInventoryItemDefinitionv2
{
	public CInventoryItemDefinitionv2()
	{

	}

	public EItemID ItemId { get; set; }

#pragma warning disable CA1044
	public string Name { private get; set; }
	public string Desc { private get; set; }
	public float Cost { private get; set; }
#pragma warning restore CA1044

	// NOTE: You should avoid use of this function where possible. It doesn't support generic items and is only useful when you KNOW a generic item wont be present (e.g. weapon hud)
	public string GetNameIgnoreGenericItems()
	{
		return Name;
	}

	// NOTE: You should avoid use of this function where possible. It doesn't support generic items and is only useful when you KNOW a generic item wont be present (e.g. weapon hud)
	public string GetDescIgnoreGenericItems()
	{
		return Desc;
	}

	// NOTE: You should avoid use of this function where possible. It doesn't support generic items and is only useful when you KNOW a generic item wont be present (e.g. weapon hud)
	public float GetCostIgnoreGenericItems()
	{
		return Cost;
	}

	public bool IsFurniture()
	{
		return ItemId == EItemID.FURNITURE;
	}


	public float Weight { get; set; }
	public float DefaultVal { get; set; }
	public ushort Limit { get; set; }
	public bool CanTake { get; set; }
	public bool CanDrop { get; set; }

	// for world player items
	[JsonIgnore]
	public bool IsLoad = true;

	public bool ShouldSerializeSocketMounts()
	{
		if (IsLoad)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public ItemMount[] SocketMounts { get; set; }

	public Dictionary<EItemSocket, ItemMount> WorldSocketMounts { get; set; }

#if SERVER
	public Type SerializationType { get; set; }
#endif

	public string ValueKey { get; set; }
	public string ValueString { get; set; }

	public string Model { get; set; }
	public bool CanSplit { get; set; }
	public bool IsContainer { get; set; }
	public int MaxStack { get; set; }
	public EItemSocket[] Sockets { get; set; }
	public EItemID[] AcceptedItems { get; set; }
	public int ContainerCapacity { get; set; }
	public float WeightAddon { get; set; }
	public UInt32 DefaultStackSize { get; set; }
	public bool ShowOnPlayer { get; set; }

	public bool IsSplittable()
	{
		return MaxStack > 1;
	}

	public bool ContainerCanAcceptItem(EItemID a_ItemID)
	{
		return IsContainer && Array.IndexOf(AcceptedItems, a_ItemID) != -1;
	}
};
*/

public class CInventoryItemDefinition
{
	public CInventoryItemDefinition()
	{

	}

	public EItemID ItemId { get; set; }

#pragma warning disable CA1044
	public string Name { private get; set; }
	public string Desc { private get; set; }
	public float Cost { private get; set; }
#pragma warning restore CA1044

	// NOTE: You should avoid use of this function where possible. It doesn't support generic items and is only useful when you KNOW a generic item wont be present (e.g. weapon hud)
	public string GetNameIgnoreGenericItems()
	{
		return Name;
	}

	// NOTE: You should avoid use of this function where possible. It doesn't support generic items and is only useful when you KNOW a generic item wont be present (e.g. weapon hud)
	public string GetDescIgnoreGenericItems()
	{
		return Desc;
	}

	// NOTE: You should avoid use of this function where possible. It doesn't support generic items and is only useful when you KNOW a generic item wont be present (e.g. weapon hud)
	public float GetCostIgnoreGenericItems()
	{
		return Cost;
	}

	public bool IsFurniture()
	{
		return ItemId == EItemID.FURNITURE;
	}


	public float Weight { get; set; }
	public float DefaultVal { get; set; }
	public ushort Limit { get; set; }
	public bool CanTake { get; set; }
	public bool CanDrop { get; set; }

	// for world player items
	public Dictionary<EItemSocket, ItemMount> WorldSocketMounts { get; set; }

#if SERVER
	public Type SerializationType { get; set; }
#endif

	public string ValueKey { get; set; }
	public string ValueString { get; set; }

	public string Model { get; set; }
	public bool CanSplit { get; set; }
	public bool IsContainer { get; set; }
	public UInt32 MaxStack { get; set; }
	public EItemSocket[] Sockets { get; set; }
	public EItemID[] AcceptedItems { get; set; }
	public int ContainerCapacity { get; set; }
	public float WeightAddon { get; set; }
	public UInt32 DefaultStackSize { get; set; }
	public bool ShowOnPlayer { get; set; }
	public EItemCategory Category { get; set; }

	public bool IsSplittable()
	{
		return CanSplit;
	}

	public bool ContainerCanAcceptItem(EItemID a_ItemID)
	{
		return IsContainer && Array.IndexOf(AcceptedItems, a_ItemID) != -1;
	}
};

public enum EItemID
{
	None = -1,
	WEAPON_KNIFE = 0,
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
	DRIVERS_PERMIT_BIKE,
	DRIVERS_PERMIT_CAR,
	DRIVERS_PERMIT_LARGE,
	WATCH,
	VEHICLE_KEY,
	TACO,
	FIREARMS_LICENSE_TIER1,
	FIREARMS_LICENSE_TIER2,
	PROPERTY_KEY,
	ARMOR_LIGHT,
	ARMOR_MEDIUM,
	ARMOR_HEAVY,
	CLOTHES,
	HANDCUFFS,
	HANDCUFFS_KEY,
	BACKPACK,
	HOLSTER,
	HOLSTER_LEG,
	SMALL_BAG,
	WEED,
	CELLPHONE,
	SPIKESTRIP,
	AMMO_HANDGUN,
	AMMO_RIFLE,
	AMMO_SHOTGUN,
	AMMO_TASER_PRODS,
	AMMO_GRENADESHELL,
	AMMO_ROCKET,
	AMMO_FLARE,
	RIOT_SHIELD,
	SWAT_SHIELD,
	// NOTE: Clothes must remain in this order, and together, or you'll break a whole bunch of things
	CLOTHES_CUSTOM_FACE,
	CLOTHES_CUSTOM_MASK,
	CLOTHES_CUSTOM_HAIR,
	CLOTHES_CUSTOM_TORSO,
	CLOTHES_CUSTOM_LEGS,
	CLOTHES_CUSTOM_BACK,
	CLOTHES_CUSTOM_FEET,
	CLOTHES_CUSTOM_ACCESSORY,
	CLOTHES_CUSTOM_UNDERSHIRT,
	CLOTHES_CUSTOM_BODYARMOR,
	CLOTHES_CUSTOM_DECALS,
	CLOTHES_CUSTOM_TOPS,
	CLOTHES_CUSTOM_HELMET,
	CLOTHES_CUSTOM_GLASSES,
	CLOTHES_CUSTOM_EARRINGS,
	// END CLOTHES
	DUTY_OUTFIT,
	METH,
	COCAINE,
	HEROIN,
	XANAX,
	BEER,
	VODKA,
	WHISKY,
	MEGAPHONE,
	RADIO,
	DUTY_BELT,
	DUTY_VEST,
	LEO_BADGE,
	FIREARMS_LICENSING_DEVICE,
	FIREARMS_LICENSING_DEVICE_REMOVAL,
	SPRAY_CAN,
	MP3_PLAYER,
	BOOMBOX,
	WEAPON_CERAMICPISTOL,
	WEAPON_NAVYREVOLVER,
	WEAPON_HAZARDCAN,
	WEAPON_POOLCUE,
	WEAPON_STONE_HATCHET,
	WEAPON_PISTOL_MK2,
	WEAPON_SNSPISTOL_MK2,
	WEAPON_RAYPISTOL,
	WEAPON_SMG_MK2,
	WEAPON_RAYCARBINE,
	WEAPON_PUMPSHOTGUN_MK2,
	WEAPON_ASSAULTRIFLE_MK2,
	WEAPON_SPECIALCARBINE_MK2,
	WEAPON_BULLPUPRIFLE_MK2,
	WEAPON_COMBATMG_MK2,
	WEAPON_HEAVYSNIPER_MK2,
	WEAPON_MARKSMANRIFLE_MK2,
	WEAPON_RAYMINIGUN,
	WEAPON_CARBINERIFLE_MK2,
	HANDGUN_SUPPRESSOR,
	SMG_SUPPRESSOR,
	SHOTGUN_SUPPRESSOR,
	RIFLE_SUPPRESSOR,
	HANDGUN_FLASHLIGHT,
	SMG_FLASHLIGHT,
	SHOTGUN_FLASHLIGHT,
	RIFLE_FLASHLIGHT,
	HANDGUN_EXTENDED_MAG,
	SMG_EXTENDED_MAG,
	SHOTGUN_EXTENDED_MAG,
	RIFLE_EXTENDED_MAG,
	SMG_DRUM_MAGAZINE,
	SHOTGUN_DRUM_MAGAZINE,
	RIFLE_DRUM_MAGAZINE,
	HANDGUN_HOLOGRAPHIC_SIGHT,
	SMG_HOLOGRAPHIC_SIGHT,
	SHOTGUN_HOLOGRAPHIC_SIGHT,
	RIFLE_HOLOGRAPHIC_SIGHT,
	WEAPON_HEAVYREVOLVER_MK2,
	HANDGUN_COMPENSATOR,
	DOUBLE_ACTION_REVOLVER,
	HANDGUN_SMALL_SCOPE,
	SMG_SMALL_SCOPE,
	SHOTGUN_SMALL_SCOPE,
	RIFLE_SMALL_SCOPE,
	RIFLE_LARGE_SCOPE,
	HANDGUN_MOUNTED_SCOPE,
	SMG_FOREGRIP,
	SHOTGUN_FOREGRIP,
	RIFLE_FOREGRIP,
	SMG_BASIC_SCOPE,
	RIFLE_BASIC_SCOPE,
	SMG_MEDIUM_SCOPE,
	SHOTGUN_MEDIUM_SCOPE,
	RIFLE_MEDIUM_SCOPE,
	RIFLE_ADVANCED_SCOPE,
	RIFLE_ZOOM_SCOPE,
	RIFLE_NIGHTVISION_SCOPE,
	RIFLE_THERMAL_SCOPE,
	SMG_HEAVY_BARREL,
	RIFLE_HEAVY_BARREL,
	SMG_FLAT_MUZZLE,
	SMG_TACTICAL_MUZZLE,
	SMG_FAT_END_MUZZLE,
	SMG_PRECISION_MUZZLE,
	SMG_HEAVY_DUTY_MUZZLE,
	SMG_SLANTED_MUZZLE,
	SMG_SPLIT_END_MUZZLE,
	SHOTGUN_SQUARED_MUZZLE,
	RIFLE_FLAG_MUZZLE,
	RIFLE_TACTICAL_MUZZLE,
	RIFLE_FAT_END_MUZZLE,
	RIFLE_PRECISION_MUZZLE,
	RIFLE_HEAVY_DUTY_MUZZLE,
	RIFLE_SLANTED_MUZZLE,
	RIFLE_SPLIT_END_MUZZLE,
	RIFLE_SQUARED_MUZZLE,
	RIFLE_BELL_END_MUZZLE,
	HANDGUN_TRACER_ROUNDS,
	HANDGUN_INCENDIARY_ROUNDS,
	HANDGUN_HOLLOW_ROUNDS,
	HANDGUN_FMJ_ROUNDS,
	SMG_TRACER_ROUNDS,
	SMG_INCENDIARY_ROUNDS,
	SMG_HOLLOW_ROUNDS,
	SMG_FMJ_ROUNDS,
	RIFLE_TRACER_ROUNDS,
	RIFLE_INCENDIARY_ROUNDS,
	RIFLE_AP_ROUNDS,
	RIFLE_FMJ_ROUNDS,
	RIFLE_EXPLOSIVE_ROUNDS,
	SHOTGUN_DRAGONSBREATH_SHELLS,
	SHOTGUN_STEEL_BUCKSHOT_SHELLS,
	SHOTGUN_FLECHETTE_SHELLS,
	SHOTGUN_EXPLOSIVE_SHELLS,
	FURNITURE,
	PET,
	FISH,
	FISH_COOLER_BOX,
	FISHING_ROD_AMATEUR,
	FISHING_ROD_INTERMEDIATE,
	FISHING_ROD_ADVANCED,
	FISHING_LINE,
	CLOTHES_CUSTOM_WATCHES,
	CLOTHES_CUSTOM_BRACELETS,
	PREMADE_CHAR_MASK,
	MARIJUANA_PLANT,
	MARIJUANA_SEED,
	WATERING_CAN,
	PLANTING_POT,
	FERTILIZER,
	SHEERS,
	MARIJUANA_DRYING,
	BOOM_MIC,
	VIDEO_CAMERA,
	MICROPHONE,
	NEWS_CAMERA,
	OPTICAL_BINOCULARS,
	NV_BINOCULARS,
	THERMAL_BINOCULARS,
	CIGARETTE_PACK_JERED,
	CIGARETTE_JERED,
	LIGHTER,
	CIGARETTE_PACK_BLUE,
	CIGARETTE_BLUE,
	CIGAR_BASIC,
	CIGAR_CASE_BASIC,
	CIGAR_PREMIUM,
	CIGAR_CASE_PREMIUM,
	ROLLING_PAPERS,
	JOINT,
	NOTE,
	BADGE,
	CASINO_CHIP_BUCKET,
	OUTFIT,
	GENERIC_ITEM,
	WEAPON_GADGETPISTOL,
	WEAPON_MILITARYRIFLE,
	WEAPON_COMBATSHOTGUN,
	MAX
};

// NOTE: If you modify this, and want items to show up in the new socket as clientside objects, add it to EDataNames etc too
// Also make sure you update helper functions like IsSocketAPlayerSocket, IsSocketAVehicleSocket
public enum EItemSocket
{
	None = -1,
	Heart = 0,
	Back = 1,
	RearPockets = 2,
	FrontPockets = 3,
	LeftWaist = 4,
	RightWaist = 5,
	BackPants = 6,
	FrontPants = 7,
	LeftAnkle = 8,
	RightAnkle = 9,
	Chest = 10,
	Head = 11,
	Clothing = 12,
	Vehicle_Trunk = 13,
	Vehicle_Seats = 14,
	Vehicle_Console_And_Glovebox = 15,
	Furniture = 16, // NOTE: This is the player socket for furniture, e.g. when they buy furniture
	LeftHand = 17,
	RightHand = 18,
	Wallet = 19,
	Keyring = 20,
	PlacedFurnitureStorage = 21, // NOTE: This is container storage, e.g. furniture dropped in a property, being accessed by the player
	Outfit = 22,
	ChestSling = 23,
	Property_Mailbox = 24,
	MAX
}

public enum EItemCategory
{
	Misc = 0,
	Weapon = 1,
	Ammo = 2,
	Clothes = 3,
	Furniture = 4,
	Keys = 5,
	Cards = 6,
	Electronics = 7,
	FoodAndDrink = 8,
	Drugs = 9,
	Armor = 10,
	Equipment = 11,
	Containers = 12,
	WeaponAttachment = 13,
}

public class ItemMount
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public float RX { get; set; }
	public float RY { get; set; }
	public float RZ { get; set; }
	public int Bone { get; set; }
}

public enum EItemParentTypes
{
	World,
	Player,
	Vehicle,
	Container,
	FurnitureInsideProperty, // This is furniture placed in a property, where parent id is the property ID. This is used to load furnitures for a property. It is NOT used for furniture inventory.
	FurnitureContainer, // This is items that are stored inside a furniture item
	DefaultFurnitureRemoval,
	PropertyMailbox
};

public static class ItemDefinitions
{
	public static Dictionary<EItemID, CInventoryItemDefinition> g_ItemDefinitions = new Dictionary<EItemID, CInventoryItemDefinition>((int)EItemID.MAX);
};

public static class ItemHelpers
{
	public static bool IsItemIDClothing(EItemID itemid)
	{
		return (itemid >= EItemID.CLOTHES_CUSTOM_FACE && itemid <= EItemID.CLOTHES_CUSTOM_TOPS);
	}

	public static bool IsItemIDAProp(EItemID itemID)
	{
		return (itemID >= EItemID.CLOTHES_CUSTOM_HELMET && itemID <= EItemID.CLOTHES_CUSTOM_EARRINGS) || itemID == EItemID.CLOTHES_CUSTOM_WATCHES || itemID == EItemID.CLOTHES_CUSTOM_BRACELETS || itemID == EItemID.CLOTHES_CUSTOM_GLASSES;
	}

	public static EItemID GetItemIDFromPropSlot(ECustomPropSlot slot)
	{
		if (slot == ECustomPropSlot.Hats)
		{
			return EItemID.CLOTHES_CUSTOM_HELMET;
		}
		else if (slot == ECustomPropSlot.Glasses)
		{
			return EItemID.CLOTHES_CUSTOM_GLASSES;
		}
		else if (slot == ECustomPropSlot.Ears)
		{
			return EItemID.CLOTHES_CUSTOM_EARRINGS;
		}
		else if (slot == ECustomPropSlot.Watches)
		{
			return EItemID.CLOTHES_CUSTOM_WATCHES;
		}
		else if (slot == ECustomPropSlot.Bracelets)
		{
			return EItemID.CLOTHES_CUSTOM_BRACELETS;
		}

		return EItemID.None;
	}
};

public static class FurnitureDefinitions
{
	public static Dictionary<uint, CFurnitureDefinition> g_FurnitureDefinitions = new Dictionary<uint, CFurnitureDefinition>((int)EItemID.MAX);
};

public enum EDrugEffect
{
	Weed,
	Meth,
	Cocaine,
	Heroin,
	Xanax
}

////////////////////////////////////////////////////////////
// NOTE: IF YOU ADD A TYPE HERE, YOU MUST HANDLE IT IN FROMDEFAULTVALUE
////////////////////////////////////////////////////////////
public class CItemValueBasic
{
	public CItemValueBasic(float a_fValue, bool a_bDuty, bool a_bSemiAuto)
	{
		value = a_fValue;
		duty = a_bDuty;
		semi_auto = a_bSemiAuto;
	}

	// set defaults since legacy items dont have certain flags set
	public CItemValueBasic()
	{
		value = 0.0f;
		duty = false;
		semi_auto = true;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueBasic casted = obj as CItemValueBasic;
		if (casted == null)
		{
			return false;
		}

		return value == casted.value;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 412) ^ Convert.ToInt32(value);
			return result;
		}
	}

	public float value { get; set; }
	public bool duty { get; set; }
	public bool is_legal { get; set; } = true; // only really refers to weapons, but we set it regardless
	public bool semi_auto { get; set; } = true; // only really refers to weapons, but we set it regardless (defaults to true)
}

public class CItemValueBasicBoolean
{
	public CItemValueBasicBoolean(bool a_bValue)
	{
		value = a_bValue;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueBasicBoolean casted = obj as CItemValueBasicBoolean;
		if (casted == null)
		{
			return false;
		}

		return value == casted.value;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 452) ^ Convert.ToInt32(value);
			return result;
		}
	}

	public bool value { get; set; }
}

public class CItemValueFurniture
{
	public CItemValueFurniture(float a_FurnitureID, int a_ActivityCurrency)
	{
		FurnitureID = a_FurnitureID;
		ActivityCurrency = a_ActivityCurrency;
	}

	public CItemValueFurniture(float a_FurnitureID)
	{
		FurnitureID = a_FurnitureID;
		ActivityCurrency = 0;
	}

	public CItemValueFurniture()
	{
		FurnitureID = 0;
		ActivityCurrency = 0;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueFurniture casted = obj as CItemValueFurniture;
		if (casted == null)
		{
			return false;
		}

		return FurnitureID == casted.FurnitureID && ActivityCurrency == casted.ActivityCurrency;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 412) ^ Convert.ToInt32(FurnitureID) ^ ActivityCurrency;
			return result;
		}
	}

	[JsonProperty("value")] // value is actually the furniture ID... this is legacy from when furniture used CItemValueBasic
	public float FurnitureID { get; set; }

	public int ActivityCurrency { get; set; }
}

public enum EGrowthState
{
	Seed,
	Sapling,
	Growing,
	FullyGrown,
}

public class CItemValueMarijuanaPlant
{
	public EGrowthState growthState { get; set; }
	public Int64 lastWatered { get; set; }
	public Int64 startedDrying { get; set; }
	public uint wateredCount { get; set; }
	public bool fertilized { get; set; }
	public bool trimmed { get; set; }

	public CItemValueMarijuanaPlant(EGrowthState a_GrowthState, Int64 a_Watered, uint a_WateredCount, bool a_Fertilized, bool a_Trimmed, Int64 a_StartedDrying)
	{
		growthState = a_GrowthState;
		lastWatered = a_Watered;
		startedDrying = a_StartedDrying;
		wateredCount = a_WateredCount;
		fertilized = a_Fertilized;
		trimmed = a_Trimmed;
	}

	public void water(Int64 unixTimestamp)
	{
		lastWatered = unixTimestamp;
		wateredCount++;
		switch (wateredCount)
		{
			case 1:
				growthState = growthState > EGrowthState.Seed ? growthState : EGrowthState.Seed;
				break;
			case 2:
			case 3:
				growthState = growthState > EGrowthState.Sapling ? growthState : EGrowthState.Sapling;
				break;
			case 4:
			case 5:
				growthState = growthState > EGrowthState.Growing ? growthState : EGrowthState.Growing;
				break;
			case 6:
				growthState = growthState > EGrowthState.FullyGrown ? growthState : EGrowthState.FullyGrown;
				break;
		}
	}
}

public class CItemValueCellphone
{
	public CItemValueCellphone(Int64 a_Number)
	{
		number = a_Number;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueCellphone casted = obj as CItemValueCellphone;
		if (casted == null)
		{
			return false;
		}

		return number == casted.number;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 397);
			return result;
		}
	}

	public Int64 number { get; set; }
}

public class CItemValueBoombox
{
	public CItemValueBoombox(int a_radioID, EntityDatabaseID a_placedBy)
	{
		radioID = a_radioID;
		placedBy = a_placedBy;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueBoombox casted = obj as CItemValueBoombox;
		if (casted == null)
		{
			return false;
		}

		return radioID == casted.radioID && placedBy == casted.placedBy;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 397) ^ radioID & (int)placedBy;
			return result;
		}
	}

	public int radioID { get; set; }
	public EntityDatabaseID placedBy { get; set; }
}

public class CItemValueBadge
{
	public CItemValueBadge(string a_factionShortName, string a_badgeName, Color a_badgeColor, bool a_bEnabled, EntityDatabaseID a_placedBy)
	{
		factionShortName = a_factionShortName;
		badgeName = a_badgeName;
		Color = a_badgeColor;
		Enabled = a_bEnabled;
		placedBy = a_placedBy;
	}

	public string factionShortName { get; set; }
	public string badgeName { get; set; }
	public Color Color { get; set; }
	public bool Enabled { get; set; }
	public EntityDatabaseID placedBy { get; set; }
}

public class CItemValueNote
{
	public CItemValueNote(string a_noteMessage, EntityDatabaseID a_placedBy, bool a_bLocked, string a_characterName)
	{
		NoteMessage = a_noteMessage;
		placedBy = a_placedBy;
		AdminLocked = a_bLocked;
		CharacterDroppedName = a_characterName;
	}

	public string NoteMessage { get; set; }
	public EntityDatabaseID placedBy { get; set; }
	public bool AdminLocked { get; set; }
	public string CharacterDroppedName { get; set; }
}

public class CItemValueClothingDuty
{
	public CItemValueClothingDuty()
	{

	}

	public CItemValueClothingDuty(PedHash a_SkinHash, int[] a_Models, int[] a_Textures, int[] a_PropsModels, int[] a_PropsTextures, bool a_IsActive, EDutyType a_DutyType)
	{
		SkinHash = a_SkinHash;

		DutyType = a_DutyType;
		IsActive = a_IsActive;
	}

	[JsonIgnore]
	public const int numItems = 12;
	[JsonIgnore]
	public const int numProps = 3;

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueClothingPremade casted = obj as CItemValueClothingPremade;
		if (casted == null)
		{
			return false;
		}

		return SkinHash == casted.SkinHash
			&& IsActive == casted.IsActive;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 412) ^ (int)SkinHash;
			result = (result * 412) ^ (int)DutyType;
			result = (result * 412) ^ (IsActive ? 1 : 0);
			return result;
		}
	}

	public PedHash SkinHash { get; set; }
	public EDutyType DutyType { get; set; }
	public bool IsActive { get; set; }
}

public class CItemValuePet
{
	public CItemValuePet()
	{

	}

	public CItemValuePet(EPetType a_PetType, string a_strName, bool a_bIsActive)
	{
		PetType = a_PetType;
		strName = a_strName;
		IsActive = a_bIsActive;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValuePet casted = obj as CItemValuePet;
		if (casted == null)
		{
			return false;
		}

		return PetType == casted.PetType
			&& strName == casted.strName
			&& IsActive == casted.IsActive;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 462) ^ (int)PetType;
			result = (result * 462) ^ (IsActive ? 1 : 0);
			return result;
		}
	}

	public EPetType PetType { get; set; }
	public string strName { get; set; }
	public bool IsActive { get; set; }
}

public class CItemValueOutfit
{
	public CItemValueOutfit()
	{
		Name = String.Empty;
		Clothes = new Dictionary<int, EntityDatabaseID>();
		Props = new Dictionary<int, EntityDatabaseID>();
		HideHair = false;
	}

	public CItemValueOutfit(string a_Name, Dictionary<int, EntityDatabaseID> a_ClothingItemIDs, Dictionary<int, EntityDatabaseID> a_PropItemIDs, bool a_HideHair)
	{
		Name = a_Name;
		Clothes = a_ClothingItemIDs;
		Props = a_PropItemIDs;
		HideHair = a_HideHair;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueOutfit casted = obj as CItemValueOutfit;
		if (casted == null)
		{
			return false;
		}

		return true;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			return result;
		}
	}

	// int instead of enum to save DB space
	public string Name { get; set; }
	public Dictionary<int, EntityDatabaseID> Clothes { get; set; }
	public Dictionary<int, EntityDatabaseID> Props { get; set; }
	public bool IsActive { get; set; }
	public bool HideHair { get; set; }
}

public class CItemValueDutyOutfit
{
	public CItemValueDutyOutfit()
	{
		DutyType = EDutyType.None;
		Name = String.Empty;
		Drawables = new Dictionary<int, int>();
		Textures = new Dictionary<int, int>();
		PropDrawables = new Dictionary<int, int>();
		PropTextures = new Dictionary<int, int>();
		Loadout = new Dictionary<int, int>();
		CharType = EDutyOutfitType.Custom;
		PremadeHash = 0;
		HideHair = false;
	}

	public CItemValueDutyOutfit(string a_Name, EDutyType a_DutyType, Dictionary<ECustomClothingComponent, int> DrawablesClothing, Dictionary<ECustomClothingComponent, int> TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables,
		Dictionary<ECustomPropSlot, int> CurrentPropTextures, Dictionary<EDutyWeaponSlot, EItemID> a_Loadout, EDutyOutfitType a_OutfitType, uint a_PremadeHash, bool a_bHideHair)
	{
		CharType = a_OutfitType;
		PremadeHash = a_PremadeHash;

		DutyType = a_DutyType;
		Name = a_Name;

		// drawables
		Drawables = new Dictionary<int, int>();
		foreach (var kvPair in DrawablesClothing) { Drawables[(int)kvPair.Key] = (int)kvPair.Value; }

		// Textures
		Textures = new Dictionary<int, int>();
		foreach (var kvPair in TexturesClothing) { Textures[(int)kvPair.Key] = (int)kvPair.Value; }

		// PropDrawables
		PropDrawables = new Dictionary<int, int>();
		foreach (var kvPair in CurrentPropDrawables) { PropDrawables[(int)kvPair.Key] = (int)kvPair.Value; }

		// PropTextures
		PropTextures = new Dictionary<int, int>();
		foreach (var kvPair in CurrentPropTextures) { PropTextures[(int)kvPair.Key] = (int)kvPair.Value; }

		// loadout
		Loadout = new Dictionary<int, int>();
		foreach (var kvPair in a_Loadout) { Loadout[(int)kvPair.Key] = (int)kvPair.Value; }

		HideHair = a_bHideHair;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueOutfit casted = obj as CItemValueOutfit;
		if (casted == null)
		{
			return false;
		}

		return true;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			return result;
		}
	}

	// int instead of enum to save DB space
	public string Name { get; set; }
	public Dictionary<int, int> Drawables { get; set; }
	public Dictionary<int, int> Textures { get; set; }
	public Dictionary<int, int> PropDrawables { get; set; }
	public Dictionary<int, int> PropTextures { get; set; }

	public Dictionary<int, int> Loadout { get; set; }
	public bool IsActive { get; set; }
	public EDutyType DutyType { get; set; }
	public EDutyOutfitType CharType { get; set; }
	public uint PremadeHash { get; set; }
	public bool HideHair { get; set; }
}

public class CItemValueClothingPremade
{
	public CItemValueClothingPremade()
	{

	}

	public CItemValueClothingPremade(PedHash a_SkinHash, bool a_IsActive)
	{
		SkinHash = a_SkinHash;
		IsActive = a_IsActive;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueClothingPremade casted = obj as CItemValueClothingPremade;
		if (casted == null)
		{
			return false;
		}

		return SkinHash == casted.SkinHash
			&& IsActive == casted.IsActive;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 412) ^ (int)SkinHash;
			result = (result * 412) ^ (IsActive ? 1 : 0);
			return result;
		}
	}

	public PedHash SkinHash { get; set; }
	public bool IsActive { get; set; }
}

public class CItemValueClothingCustom
{
	public CItemValueClothingCustom(int a_Model, int a_Texture, bool a_IsActive)
	{
		Model = a_Model;
		Texture = a_Texture;
		IsActive = a_IsActive;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueClothingCustom casted = obj as CItemValueClothingCustom;
		if (casted == null)
		{
			return false;
		}

		return IsActive == casted.IsActive
			&& Model == casted.Model
			&& Texture == casted.Texture;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 413) ^ (IsActive ? 1 : 0);
			result = (result * 413) ^ Model;
			result = (result * 413) ^ Texture;
			return result;
		}
	}

	public bool IsActive { get; set; }
	public int Model { get; set; }
	public int Texture { get; set; }
}

public class CItemValueGenericItem
{
	public CItemValueGenericItem(string a_strName, string a_strModel, bool a_bLocked, EntityDatabaseID a_PlacedBy)
	{
		name = a_strName;
		model = a_strModel;
		AdminLocked = a_bLocked;
		PlacedBy = a_PlacedBy;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemValueGenericItem casted = obj as CItemValueGenericItem;
		if (casted == null)
		{
			return false;
		}

		return name.Equals(casted.name)
			&& model.Equals(casted.model);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			return result;
		}
	}

	public string name { get; set; }
	public string model { get; set; }
	public bool AdminLocked { get; set; }
	public EntityDatabaseID PlacedBy { get; set; }
}

public class CItemInstanceDef
{
	// START CONSTRUCTORS
	public CItemInstanceDef()
	{

	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		CItemInstanceDef casted = obj as CItemInstanceDef;
		if (casted == null)
		{
			return false;
		}

		bool bDBIDMatches = DatabaseID.Equals(casted.DatabaseID) || DatabaseID == 999999 || casted.DatabaseID == 999999;

		return Value.Equals(casted.Value)
		&& bDBIDMatches
		&& ItemID.Equals(casted.ItemID);
		// We probably don't have ot compare the rest here, DBID itself should be unique enough. ItemID probably isn't even required.
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 300) ^ Convert.ToInt32(DatabaseID);
			result = (result * 300) ^ Convert.ToInt32(ItemID);
			result = (result * 300) ^ Convert.ToInt32(Value.GetHashCode());
			return result;
		}
	}

	public string GetName()
	{
		if (ItemDefinitions.g_ItemDefinitions.ContainsKey(ItemID))
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[ItemID];
			if (itemDef.IsFurniture())
			{
				string strValueKey = "value";
				string strValue = String.Empty;

				JObject obj = JObject.FromObject(Value);

				if (obj != null)
				{
					JToken jTok = obj.GetValue(strValueKey, StringComparison.OrdinalIgnoreCase);
					if (jTok != null)
					{
						strValue = jTok.ToString();
					}
				}

				if (!String.IsNullOrEmpty(strValue))
				{
					uint furnitureID;
					if (UInt32.TryParse(strValue, out furnitureID))
					{
						if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
						{
							return FurnitureDefinitions.g_FurnitureDefinitions[furnitureID].Name;
						}
						else
						{
							return Helpers.FormatString("SCRIPT ERROR FURN1: {0}", ItemID);
						}
					}
					else
					{
						return Helpers.FormatString("SCRIPT ERROR FURN2: {0}", ItemID);
					}
				}
				else
				{
					return Helpers.FormatString("SCRIPT ERROR FURN3: {0}", ItemID);
				}
			}
			else if (ItemID == EItemID.PET) // pets are generic
			{
				// TODO: Better way
				CItemValuePet itemValue = Newtonsoft.Json.JsonConvert.DeserializeObject<CItemValuePet>(GetValueDataSerialized());
				return Helpers.FormatString("Pet Cage - {0}", itemValue.PetType.ToString());
			}
			else if (ItemID == EItemID.GENERIC_ITEM)
			{
				CItemValueGenericItem itemValue = JsonConvert.DeserializeObject<CItemValueGenericItem>(GetValueDataSerialized());
				return itemValue.name;
			}
			else if (ItemID == EItemID.CLOTHES_CUSTOM_MASK) // item name overrides
			{
				CItemValueClothingCustom itemValue = Newtonsoft.Json.JsonConvert.DeserializeObject<CItemValueClothingCustom>(GetValueDataSerialized());
				if (itemValue.Model == MaskHelpers.HalloweenMask)
				{
					return "Halloween Mask";
				}
				else if (itemValue.Model == MaskHelpers.ChristmasMask)
				{
					return "Santa Mask";
				}
			}

			return itemDef.GetNameIgnoreGenericItems();
		}

		return Helpers.FormatString("SCRIPT ERROR: {0}", ItemID);
	}

	public string GetDescription()
	{
		if (ItemDefinitions.g_ItemDefinitions.ContainsKey(ItemID))
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[ItemID];
			if (itemDef.IsFurniture())
			{
				string strValueKey = "value";
				string strValue = String.Empty;

				JObject obj = JObject.FromObject(Value);

				if (obj != null)
				{
					JToken jTok = obj.GetValue(strValueKey, StringComparison.OrdinalIgnoreCase);
					if (jTok != null)
					{
						strValue = jTok.ToString();
					}
				}

				if (!String.IsNullOrEmpty(strValue))
				{
					uint furnitureID;
					if (UInt32.TryParse(strValue, out furnitureID))
					{
						if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
						{
							CFurnitureDefinition furnDef = FurnitureDefinitions.g_FurnitureDefinitions[furnitureID];
							string strDesc = furnDef.Description;
							int storageCapacity = furnDef.StorageCapacity;

							return Helpers.FormatString("{0}<br><br>Storage Capacity: {1}", strDesc, storageCapacity);
						}
						else
						{
							return Helpers.FormatString("SCRIPT ERROR FURN1: {0}", ItemID);
						}
					}
					else
					{
						return Helpers.FormatString("SCRIPT ERROR FURN2: {0}", ItemID);
					}
				}
				else
				{
					return Helpers.FormatString("SCRIPT ERROR FURN3: {0}", ItemID);
				}
			}
			else if (ItemID == EItemID.PET) // pets are generic
			{
				CItemValuePet itemValue = Newtonsoft.Json.JsonConvert.DeserializeObject<CItemValuePet>(GetValueDataSerialized());
				return Helpers.FormatString("A pet cage containing a {0}", itemValue.PetType.ToString());
			}
			else if (ItemID == EItemID.CLOTHES_CUSTOM_MASK) // item name overrides
			{
				CItemValueClothingCustom itemValue = Newtonsoft.Json.JsonConvert.DeserializeObject<CItemValueClothingCustom>(GetValueDataSerialized());
				if (itemValue.Model == MaskHelpers.HalloweenMask)
				{
					return "A spooky mask!";
				}
				else if (itemValue.Model == MaskHelpers.ChristmasMask)
				{
					return "A Santa mask";
				}
			}

			return itemDef.GetDescIgnoreGenericItems();
		}

		return Helpers.FormatString("SCRIPT ERROR: {0}", ItemID);
	}

	public float GetCost()
	{
		if (ItemDefinitions.g_ItemDefinitions.ContainsKey(ItemID))
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[ItemID];
			if (itemDef.IsFurniture())
			{
				string strValueKey = "value";
				string strValue = String.Empty;

				JObject obj = JObject.FromObject(Value);

				if (obj != null)
				{
					JToken jTok = obj.GetValue(strValueKey, StringComparison.OrdinalIgnoreCase);
					if (jTok != null)
					{
						strValue = jTok.ToString();
					}
				}

				if (!String.IsNullOrEmpty(strValue))
				{
					uint furnitureID;
					if (UInt32.TryParse(strValue, out furnitureID))
					{
						if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
						{
							return FurnitureDefinitions.g_FurnitureDefinitions[furnitureID].Price;
						}
						else
						{
							return 999999.0f;
						}
					}
					else
					{
						return 999998.0f;
					}
				}
				else
				{
					return 999997.0f;
				}
			}

			return itemDef.GetCostIgnoreGenericItems();
		}

		return 999996.0f;
	}

	public bool IsFurniture()
	{
		if (ItemDefinitions.g_ItemDefinitions.ContainsKey(ItemID))
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[ItemID];
			return itemDef.IsFurniture();
		}

		return false;
	}

	public uint GetFurnitureID()
	{
		if (ItemDefinitions.g_ItemDefinitions.ContainsKey(ItemID))
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[ItemID];
			if (itemDef.IsFurniture())
			{
				string strValueKey = "value";
				string strValue = String.Empty;

				JObject obj = JObject.FromObject(Value);

				if (obj != null)
				{
					JToken jTok = obj.GetValue(strValueKey, StringComparison.OrdinalIgnoreCase);
					if (jTok != null)
					{
						strValue = jTok.ToString();
					}
				}

				if (!String.IsNullOrEmpty(strValue))
				{
					uint furnitureID;
					if (UInt32.TryParse(strValue, out furnitureID))
					{
						if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
						{
							return FurnitureDefinitions.g_FurnitureDefinitions[furnitureID].ID;
						}
					}
				}
			}
		}

		return 0;
	}

#if SERVER
	private void Validate()
	{
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[ItemID];
		if (Value.GetType() != itemDef.SerializationType || !Value.GetType().IsClass)
		{
			if (Value.GetType() != itemDef.SerializationType)
			{
				PrintLogger.LogMessage(ELogSeverity.ERROR, "FATAL ERROR: CItemInstanceDef::Validate failed. Expected Type: {0} Got Type: {1}", Value.GetType().ToString(), itemDef.SerializationType.ToString());
			}

			if (!Value.GetType().IsClass)
			{
				PrintLogger.LogMessage(ELogSeverity.ERROR, "FATAL ERROR: CItemInstanceDef::Validate failed. Expected a class.");
			}
		}
	}
#endif

#if SERVER
	public static CItemInstanceDef FromJSONString(EntityDatabaseID a_DBID, EItemID a_ItemID, string a_strJson, EItemSocket a_ItemSocket, EntityDatabaseID a_ParentDatabaseID, EItemParentTypes a_ParentType, UInt32 a_StackSize)
	{
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[a_ItemID];
		Type typeToUse = itemDef.SerializationType;

		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			MissingMemberHandling = MissingMemberHandling.Ignore,
			CheckAdditionalContent = false
		};

		CItemInstanceDef itemInst = null;

		try
		{
			itemInst = new CItemInstanceDef
			{
				DatabaseID = a_DBID,
				ItemID = a_ItemID,
				Value = JsonConvert.DeserializeObject(a_strJson, typeToUse, settings),
				CurrentSocket = a_ItemSocket,
				ParentDatabaseID = a_ParentDatabaseID,
				ParentType = a_ParentType,
				StackSize = a_StackSize,
			};
		}
		catch
		{
			PrintLogger.LogMessage(ELogSeverity.ERROR, "FATAL ERROR: CItemInstanceDef::FromJSONString failed. Input JSON was not mappable to defined type {0}.", typeToUse.ToString());
		}
		itemInst.Validate();
		return itemInst;
	}

	public static CItemInstanceDef FromJSONStringNoDBID(EItemID a_ItemID, string a_strJson)
	{
		// TODO_INVENTORY: what should this do for item parent type?
		return FromJSONString(999999, a_ItemID, a_strJson, EItemSocket.None, 999999, EItemParentTypes.Player, 0);
	}

	public static CItemInstanceDef FromItemID(EItemID a_ItemID)
	{
		CItemInstanceDef itemInst = new CItemInstanceDef
		{
			DatabaseID = 999999,
			ItemID = a_ItemID,
			Value = new CItemValueBasic(0, false, true)
		};

		itemInst.Validate();
		return itemInst;
	}

	public static CItemInstanceDef FromObjectNoDBID(EItemID a_ItemID, object a_Value, EItemSocket a_ItemSocket, EntityDatabaseID a_ParentDatabaseID, EItemParentTypes a_ParentType)
	{
		return FromObject(999999, a_ItemID, a_Value, a_ItemSocket, a_ParentDatabaseID, a_ParentType, 1);
	}

	public static CItemInstanceDef FromObjectNoDBIDWithStackSize(EItemID a_ItemID, object a_Value, EItemSocket a_ItemSocket, EntityDatabaseID a_ParentDatabaseID, EItemParentTypes a_ParentType, uint a_StackSize)
	{
		return FromObject(999999, a_ItemID, a_Value, a_ItemSocket, a_ParentDatabaseID, a_ParentType, a_StackSize);
	}

	public static CItemInstanceDef FromObjectNoDBIDNoSocketPlayerParent(EItemID a_ItemID, object a_Value)
	{
		return FromObject(999999, a_ItemID, a_Value, EItemSocket.None, 999999, EItemParentTypes.Player, 1);
	}

	public static CItemInstanceDef FromObjectNoDBIDNoSocketPlayerParentWithStackSize(EItemID a_ItemID, object a_Value, uint a_StackSize)
	{
		return FromObject(999999, a_ItemID, a_Value, EItemSocket.None, 999999, EItemParentTypes.Player, a_StackSize);
	}

	public static CItemInstanceDef FromTypedObjectNoDBID<T>(EItemID a_ItemID, T a_Value, uint a_StackSize)
	{
		return FromTypedObject<T>(999999, a_ItemID, a_Value, a_StackSize);
	}

	public static CItemInstanceDef FromBasicValueNoDBID(EItemID a_ItemID, float a_Value, UInt32 a_StackSize = 1)
	{
		return FromBasicValue(999999, a_ItemID, a_Value, a_StackSize);
	}

	public static CItemInstanceDef FromDefaultValue(EItemID a_ItemID, float a_Value)
	{
		return FromDefaultValueWithStackSize(a_ItemID, a_Value, ItemDefinitions.g_ItemDefinitions[a_ItemID].DefaultStackSize);
	}

	public static CItemInstanceDef FromDefaultValueWithStackSize(EItemID a_ItemID, float a_Value, uint a_StackSize)
	{
		object valueObj = null;
		if (a_ItemID == EItemID.CELLPHONE)
		{
			valueObj = new CItemValueCellphone(-1); // caller must generate phone number from insert ID
		}
		else if (a_ItemID == EItemID.CLOTHES)
		{
			valueObj = new CItemValueClothingPremade(PedHash.Dolphin, false);
		}
		else if ((a_ItemID >= EItemID.CLOTHES_CUSTOM_FACE && a_ItemID <= EItemID.CLOTHES_CUSTOM_TOPS) || ItemHelpers.IsItemIDAProp(a_ItemID)) // custom clothing or props
		{
			valueObj = new CItemValueClothingCustom(0, 0, false);
		}
		else if (a_ItemID == EItemID.BOOMBOX)
		{
			valueObj = new CItemValueBoombox(-1, -1);
		}
		else if (a_ItemID == EItemID.PREMADE_CHAR_MASK)
		{
			valueObj = new CItemValueBasicBoolean(false);
		}
		else if (a_ItemID == EItemID.GENERIC_ITEM)
		{
			valueObj = new CItemValueGenericItem("Generic Item", "hei_prop_drug_statue_box_big", false, -1);
		}
		else if (a_ItemID == EItemID.OUTFIT)
		{
			valueObj = new CItemValueOutfit();
		}
		else if (a_ItemID == EItemID.DUTY_OUTFIT)
		{
			valueObj = new CItemValueDutyOutfit();
		}
		else if (a_ItemID == EItemID.FURNITURE)
		{
			valueObj = new CItemValueFurniture(a_Value);
		}
		else
		{
			valueObj = new CItemValueBasic(a_Value, false, true);
		}

		CItemInstanceDef itemInst = new CItemInstanceDef
		{
			DatabaseID = 999999,
			ItemID = a_ItemID,
			Value = valueObj,
			StackSize = a_StackSize,
		};

		itemInst.Validate();
		return itemInst;
	}

	public static CItemInstanceDef FromObject(EntityDatabaseID a_DBID, EItemID a_ItemID, object a_Value, EItemSocket a_ItemSocket, EntityDatabaseID a_ParentDatabaseID, EItemParentTypes a_ParentType, uint a_StackSize)
	{
		CItemInstanceDef itemInst = new CItemInstanceDef
		{
			DatabaseID = a_DBID,
			ItemID = a_ItemID,
			Value = a_Value,
			CurrentSocket = a_ItemSocket,
			ParentDatabaseID = a_ParentDatabaseID,
			ParentType = a_ParentType,
			StackSize = a_StackSize
		};

		itemInst.Validate();
		return itemInst;
	}

	public static CItemInstanceDef FromTypedObject<T>(EntityDatabaseID a_DBID, EItemID a_ItemID, T a_Value, uint a_StackSize)
	{
		CItemInstanceDef itemInst = new CItemInstanceDef
		{
			DatabaseID = a_DBID,
			ItemID = a_ItemID,
			Value = a_Value,
			StackSize = a_StackSize
		};

		itemInst.Validate();
		return itemInst;
	}

	public static CItemInstanceDef FromBasicValue(EntityDatabaseID a_DBID, EItemID a_ItemID, float a_Value, UInt32 a_StackSize = 1)
	{
		CItemInstanceDef itemInst = new CItemInstanceDef
		{
			DatabaseID = a_DBID,
			ItemID = a_ItemID,
			Value = new CItemValueBasic(a_Value, false, true),
			StackSize = a_StackSize,
		};

		itemInst.Validate();
		return itemInst;
	}
	// FINISH CONSTRUCTORS
#endif
	public T GetValueData<T>()
	{
		return (T)Convert.ChangeType(Value, typeof(T));
	}

	public string GetValueDataSerialized()
	{
		return JsonConvert.SerializeObject(Value);
	}

	public void SetBinding(EItemSocket a_CurrentSocket, EntityDatabaseID a_ParentDBID, EItemParentTypes a_ParentType)
	{
		CurrentSocket = a_CurrentSocket;
		ParentDatabaseID = a_ParentDBID;
		ParentType = a_ParentType;
	}

	public bool IsWeapon()
	{
		return WeaponHelpers.IsItemAWeapon(ItemID);
	}

	public bool IsWeaponAttachment()
	{
		return WeaponHelpers.IsItemAWeaponAttachment(ItemID);
	}

	public bool IsFirearm()
	{
		return WeaponHelpers.IsItemAFirearm(ItemID);
	}

	public EntityDatabaseID DatabaseID { get; set; }
	public EItemID ItemID { get; set; }
	public object Value { get; set; }
	// TODO: Provide constructors?
	public EItemSocket CurrentSocket { get; set; } = EItemSocket.None;
	public EItemParentTypes ParentType { get; set; } = EItemParentTypes.Player;
	public EntityDatabaseID ParentDatabaseID { get; set; } = -1;
	public UInt32 StackSize { get; set; } = 1;
}

public static class ItemWeaponDefinitions
{
	public static Dictionary<EItemID, WeaponHash> g_DictItemIDToWeaponHash = new Dictionary<EItemID, WeaponHash>()
	{
		{ EItemID.WEAPON_KNIFE, WeaponHash.Knife },
		{ EItemID.WEAPON_NIGHTSTICK, WeaponHash.Nightstick },
		{ EItemID.WEAPON_HAMMER, WeaponHash.Hammer },
		{ EItemID.WEAPON_BAT, WeaponHash.Bat },
		{ EItemID.WEAPON_GOLFCLUB, WeaponHash.Golfclub },
		{ EItemID.WEAPON_CROWBAR, WeaponHash.Crowbar },
		{ EItemID.WEAPON_PISTOL, WeaponHash.Pistol },
		{ EItemID.WEAPON_COMBATPISTOL, WeaponHash.Combatpistol },
		{ EItemID.WEAPON_APPISTOL, WeaponHash.Appistol },
		{ EItemID.WEAPON_PISTOL50, WeaponHash.Pistol50 },
		{ EItemID.WEAPON_MICROSMG, WeaponHash.Microsmg },
		{ EItemID.WEAPON_SMG, WeaponHash.Smg },
		{ EItemID.WEAPON_ASSAULTSMG, WeaponHash.Assaultsmg },
		{ EItemID.WEAPON_ASSAULTRIFLE, WeaponHash.Assaultrifle },
		{ EItemID.WEAPON_CARBINERIFLE, WeaponHash.Carbinerifle },
		{ EItemID.WEAPON_ADVANCEDRIFLE, WeaponHash.Advancedrifle },
		{ EItemID.WEAPON_MG, WeaponHash.Mg },
		{ EItemID.WEAPON_COMBATMG, WeaponHash.Combatmg },
		{ EItemID.WEAPON_PUMPSHOTGUN, WeaponHash.Pumpshotgun },
		{ EItemID.WEAPON_SAWNOFFSHOTGUN, WeaponHash.Sawnoffshotgun },
		{ EItemID.WEAPON_ASSAULTSHOTGUN, WeaponHash.Assaultshotgun },
		{ EItemID.WEAPON_BULLPUPSHOTGUN, WeaponHash.Bullpupshotgun },
		{ EItemID.WEAPON_STUNGUN, WeaponHash.Stungun },
		{ EItemID.WEAPON_SNIPERRIFLE, WeaponHash.Sniperrifle },
		{ EItemID.WEAPON_HEAVYSNIPER, WeaponHash.Heavysniper },
		{ EItemID.WEAPON_GRENADELAUNCHER, WeaponHash.Grenadelauncher },
		{ EItemID.WEAPON_GRENADELAUNCHER_SMOKE, WeaponHash.Grenadelauncher_smoke },
		{ EItemID.WEAPON_RPG, WeaponHash.Rpg },
		{ EItemID.WEAPON_MINIGUN, WeaponHash.Minigun },
		{ EItemID.WEAPON_GRENADE, WeaponHash.Grenade },
		{ EItemID.WEAPON_STICKYBOMB, WeaponHash.Stickybomb },
		{ EItemID.WEAPON_SMOKEGRENADE, WeaponHash.Smokegrenade },
		{ EItemID.WEAPON_BZGAS, WeaponHash.Bzgas },
		{ EItemID.WEAPON_MOLOTOV, WeaponHash.Molotov },
		{ EItemID.WEAPON_FIREEXTINGUISHER, WeaponHash.Fireextinguisher },
		{ EItemID.WEAPON_PETROLCAN, WeaponHash.Petrolcan },
		{ EItemID.WEAPON_SNSPISTOL, WeaponHash.Snspistol },
		{ EItemID.WEAPON_SPECIALCARBINE, WeaponHash.Specialcarbine },
		{ EItemID.WEAPON_HEAVYPISTOL, WeaponHash.Heavypistol },
		{ EItemID.WEAPON_BULLPUPRIFLE, WeaponHash.Bullpuprifle },
		{ EItemID.WEAPON_HOMINGLAUNCHER, WeaponHash.Hominglauncher },
		{ EItemID.WEAPON_PROXMINE, WeaponHash.Proximine },
		{ EItemID.WEAPON_SNOWBALL, WeaponHash.Snowball },
		{ EItemID.WEAPON_VINTAGEPISTOL, WeaponHash.Vintagepistol },
		{ EItemID.WEAPON_DAGGER, WeaponHash.Dagger },
		{ EItemID.WEAPON_FIREWORK, WeaponHash.Firework },
		{ EItemID.WEAPON_MUSKET, WeaponHash.Musket },
		{ EItemID.WEAPON_MARKSMANRIFLE, WeaponHash.Marksmanrifle },
		{ EItemID.WEAPON_HEAVYSHOTGUN, WeaponHash.Heavyshotgun },
		{ EItemID.WEAPON_GUSENBERG, WeaponHash.Gusenberg },
		{ EItemID.WEAPON_HATCHET, WeaponHash.Hatchet },
		{ EItemID.WEAPON_RAILGUN, WeaponHash.Railgun },
		{ EItemID.WEAPON_COMBATPDW, WeaponHash.Combatpdw },
		{ EItemID.WEAPON_KNUCKLE, WeaponHash.Knuckle },
		{ EItemID.WEAPON_MARKSMANPISTOL, WeaponHash.Marksmanpistol },
		{ EItemID.WEAPON_BOTTLE, WeaponHash.Bottle },
		{ EItemID.WEAPON_FLAREGUN, WeaponHash.Flaregun },
		{ EItemID.WEAPON_FLARE, WeaponHash.Flare },
		{ EItemID.WEAPON_REVOLVER, WeaponHash.Revolver },
		{ EItemID.WEAPON_SWITCHBLADE, WeaponHash.Switchblade },
		{ EItemID.WEAPON_MACHETE, WeaponHash.Machete },
		{ EItemID.WEAPON_FLASHLIGHT, WeaponHash.Flashlight },
		{ EItemID.WEAPON_MACHINEPISTOL, WeaponHash.Machinepistol },
		{ EItemID.WEAPON_DBSHOTGUN, WeaponHash.Dbshotgun },
		{ EItemID.WEAPON_COMPACTRIFLE, WeaponHash.Compactrifle },
		{ EItemID.WEAPON_BATTLEAXE, WeaponHash.Battleaxe },
		{ EItemID.WEAPON_BALL, WeaponHash.Ball },
		{ EItemID.WEAPON_PARACHUTE, WeaponHash.Parachute },
		{ EItemID.WEAPON_WRENCH, WeaponHash.Wrench },
		{ EItemID.WEAPON_COMPACTLAUNCHER, WeaponHash.Compactlauncher },
		{ EItemID.WEAPON_MINISMG, WeaponHash.Minismg },
		{ EItemID.WEAPON_AUTOSHOTGUN, WeaponHash.Assaultshotgun },
		{ EItemID.WEAPON_CERAMICPISTOL, WeaponHash.CeramicPistol },
		{ EItemID.WEAPON_NAVYREVOLVER, WeaponHash.NavyRevolver },
		{ EItemID.WEAPON_HAZARDCAN, WeaponHash.HazardCan },
		{ EItemID.WEAPON_POOLCUE, WeaponHash.Poolcue },
		{ EItemID.WEAPON_STONE_HATCHET, WeaponHash.Stone_hatchet },
		{ EItemID.WEAPON_PISTOL_MK2, WeaponHash.Pistol_mk2 },
		{ EItemID.WEAPON_SNSPISTOL_MK2, WeaponHash.Snspistol_mk2 },
		{ EItemID.WEAPON_RAYPISTOL, WeaponHash.Raypistol },
		{ EItemID.WEAPON_SMG_MK2, WeaponHash.Smg_mk2 },
		{ EItemID.WEAPON_RAYCARBINE, WeaponHash.Raycarbine },
		{ EItemID.WEAPON_PUMPSHOTGUN_MK2, WeaponHash.Pumpshotgun_mk2 },
		{ EItemID.WEAPON_ASSAULTRIFLE_MK2, WeaponHash.Assaultrifle_mk2 },
		{ EItemID.WEAPON_SPECIALCARBINE_MK2, WeaponHash.Specialcarbine_mk2 },
		{ EItemID.WEAPON_BULLPUPRIFLE_MK2, WeaponHash.Bullpuprifle_mk2 },
		{ EItemID.WEAPON_COMBATMG_MK2, WeaponHash.Combatmg_mk2 },
		{ EItemID.WEAPON_HEAVYSNIPER_MK2, WeaponHash.Heavysniper_mk2 },
		{ EItemID.WEAPON_MARKSMANRIFLE_MK2, WeaponHash.Marksmanrifle_mk2 },
		{ EItemID.WEAPON_RAYMINIGUN, WeaponHash.Rayminigun },
		{ EItemID.WEAPON_CARBINERIFLE_MK2, WeaponHash.Carbinerifle_mk2},
		{ EItemID.WEAPON_HEAVYREVOLVER_MK2, WeaponHash.Revolver_mk2 },
		{ EItemID.DOUBLE_ACTION_REVOLVER, WeaponHash.Doubleaction },
		{ EItemID.WEAPON_GADGETPISTOL, (WeaponHash)NewWeaponHash_GadgetPistol }, // TODO_RAGE: These arent defined in the server API yet
		{ EItemID.WEAPON_COMBATSHOTGUN, (WeaponHash)NewWeaponHash_CombatShotgun },
		{ EItemID.WEAPON_MILITARYRIFLE, (WeaponHash)NewWeaponHash_MilitaryRifle },
	};

	const uint NewWeaponHash_GadgetPistol = 0x57A4368C;
	const uint NewWeaponHash_CombatShotgun = 0x5A96BA4;
	const uint NewWeaponHash_MilitaryRifle = 0x9D1F17E6;

	public static Dictionary<EItemID, EItemID> g_DictAmmoUsedByWeapon = new Dictionary<EItemID, EItemID>()
	{
		{ EItemID.WEAPON_KNIFE, EItemID.None },
		{ EItemID.WEAPON_NIGHTSTICK, EItemID.None },
		{ EItemID.WEAPON_HAMMER, EItemID.None },
		{ EItemID.WEAPON_BAT, EItemID.None },
		{ EItemID.WEAPON_GOLFCLUB, EItemID.None },
		{ EItemID.WEAPON_CROWBAR, EItemID.None },
		{ EItemID.WEAPON_PISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_COMBATPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_APPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_PISTOL50, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_MICROSMG, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_SMG, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_ASSAULTSMG, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_ASSAULTRIFLE,EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_CARBINERIFLE, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_ADVANCEDRIFLE, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_MG, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_COMBATMG, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_PUMPSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_SAWNOFFSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_ASSAULTSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_BULLPUPSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_STUNGUN, EItemID.AMMO_TASER_PRODS },
		{ EItemID.WEAPON_SNIPERRIFLE, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_HEAVYSNIPER, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_GRENADELAUNCHER, EItemID.AMMO_GRENADESHELL },
		{ EItemID.WEAPON_GRENADELAUNCHER_SMOKE, EItemID.AMMO_GRENADESHELL },
		{ EItemID.WEAPON_RPG, EItemID.AMMO_ROCKET },
		{ EItemID.WEAPON_MINIGUN, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_GRENADE, EItemID.None },
		{ EItemID.WEAPON_STICKYBOMB, EItemID.None },
		{ EItemID.WEAPON_SMOKEGRENADE, EItemID.None },
		{ EItemID.WEAPON_BZGAS, EItemID.None },
		{ EItemID.WEAPON_MOLOTOV, EItemID.None },
		{ EItemID.WEAPON_FIREEXTINGUISHER, EItemID.None },
		{ EItemID.WEAPON_PETROLCAN, EItemID.None },
		{ EItemID.WEAPON_SNSPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_SPECIALCARBINE, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_HEAVYPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_BULLPUPRIFLE,EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_HOMINGLAUNCHER, EItemID.AMMO_ROCKET },
		{ EItemID.WEAPON_PROXMINE, EItemID.None },
		{ EItemID.WEAPON_SNOWBALL, EItemID.WEAPON_SNOWBALL }, // Uses its own stack size
		{ EItemID.WEAPON_VINTAGEPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_DAGGER, EItemID.None },
		{ EItemID.WEAPON_FIREWORK, EItemID.None },
		{ EItemID.WEAPON_MUSKET, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_MARKSMANRIFLE, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_HEAVYSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_GUSENBERG, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_HATCHET, EItemID.None },
		{ EItemID.WEAPON_RAILGUN, EItemID.None },
		{ EItemID.WEAPON_COMBATPDW, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_KNUCKLE, EItemID.None },
		{ EItemID.WEAPON_MARKSMANPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_BOTTLE, EItemID.None },
		{ EItemID.WEAPON_FLAREGUN, EItemID.AMMO_FLARE },
		{ EItemID.WEAPON_FLARE, EItemID.None },
		{ EItemID.WEAPON_REVOLVER, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_SWITCHBLADE, EItemID.None },
		{ EItemID.WEAPON_MACHETE, EItemID.None },
		{ EItemID.WEAPON_FLASHLIGHT, EItemID.None },
		{ EItemID.WEAPON_MACHINEPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_DBSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_COMPACTRIFLE, EItemID.AMMO_RIFLE},
		{ EItemID.WEAPON_BATTLEAXE, EItemID.None },
		{ EItemID.WEAPON_BALL, EItemID.None },
		{ EItemID.WEAPON_PARACHUTE, EItemID.None },
		{ EItemID.WEAPON_WRENCH, EItemID.None },
		{ EItemID.WEAPON_COMPACTLAUNCHER, EItemID.AMMO_GRENADESHELL },
		{ EItemID.WEAPON_MINISMG, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_AUTOSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_CERAMICPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_NAVYREVOLVER, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_HAZARDCAN, EItemID.None },
		{ EItemID.WEAPON_POOLCUE, EItemID.None },
		{ EItemID.WEAPON_STONE_HATCHET, EItemID.None },
		{ EItemID.WEAPON_PISTOL_MK2, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_SNSPISTOL_MK2, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_RAYPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_SMG_MK2, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_RAYCARBINE, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_PUMPSHOTGUN_MK2, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_ASSAULTRIFLE_MK2, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_SPECIALCARBINE_MK2, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_BULLPUPRIFLE_MK2, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_COMBATMG_MK2, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_HEAVYSNIPER_MK2, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_MARKSMANRIFLE_MK2, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_RAYMINIGUN, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_CARBINERIFLE_MK2, EItemID.AMMO_RIFLE },
		{ EItemID.WEAPON_HEAVYREVOLVER_MK2, EItemID.AMMO_HANDGUN },
		{ EItemID.DOUBLE_ACTION_REVOLVER, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_GADGETPISTOL, EItemID.AMMO_HANDGUN },
		{ EItemID.WEAPON_COMBATSHOTGUN, EItemID.AMMO_SHOTGUN },
		{ EItemID.WEAPON_MILITARYRIFLE, EItemID.AMMO_RIFLE },
	};
}

public class CPropertyDefaultFurnitureRemovalInstance
{
	public CPropertyDefaultFurnitureRemovalInstance(EntityDatabaseID a_DBID, EntityDatabaseID a_ParentProperty, uint a_Model, Vector3 a_vecPos, EntityDatabaseID a_PlacedBy)
	{
		DBID = a_DBID;
		Model = a_Model;
		vecPos = a_vecPos;
		PlacedBy = a_PlacedBy;
		ParentProperty = a_ParentProperty;
	}

	public EntityDatabaseID DBID { get; set; }
	public EntityDatabaseID ParentProperty { get; set; }
	public uint Model { get; set; }
	public Vector3 vecPos { get; set; }
	public Vector3 vecRot { get; set; }
	public EntityDatabaseID PlacedBy { get; set; }

#if !SERVER
	public void Apply()
	{
		RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(vecPos.X, vecPos.Y, vecPos.Z, 150.0f, Model, true);
	}

	public void Restore()
	{
		RAGE.Game.Entity.RemoveModelHide((int)vecPos.X, (int)vecPos.Y, (int)vecPos.Z, (int)150.0f, (int)Model, 1);
	}
#endif
}

public class CPropertyFurnitureInstance
{
	public CPropertyFurnitureInstance(EntityDatabaseID a_DBID, EntityDatabaseID a_ParentProperty, uint a_FurnitureID, Vector3 a_vecPos, Vector3 a_vecRot, EntityDatabaseID a_PlacedBy, string strItemValueJSON)
	{
		DBID = a_DBID;
		FurnitureID = a_FurnitureID;
		vecPos = a_vecPos;
		vecRot = a_vecRot;
		PlacedBy = a_PlacedBy;
		ParentProperty = a_ParentProperty;

#if SERVER
		// just so we can deserialize the item value...
		CItemInstanceDef dummyItemInstance = CItemInstanceDef.FromJSONStringNoDBID(EItemID.FURNITURE, strItemValueJSON);
		Value = (CItemValueFurniture)dummyItemInstance.Value;
#endif
	}

	public EntityDatabaseID DBID { get; set; }
	public EntityDatabaseID ParentProperty { get; set; }
	public uint FurnitureID { get; set; }
	public Vector3 vecPos { get; set; }
	public Vector3 vecRot { get; set; }
	public EntityDatabaseID PlacedBy { get; set; }

#if SERVER
	public CItemValueFurniture Value { get; set; }
#endif

#if !SERVER
	public void Create()
	{
		if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(FurnitureID))
		{
			CFurnitureDefinition furnDef = FurnitureDefinitions.g_FurnitureDefinitions[FurnitureID];

			if (furnDef != null)
			{
				uint model = HashHelper.GetHashUnsigned(furnDef.Model);
				AsyncModelLoader.RequestSyncInstantLoad(model);
				m_Object = new RAGE.Elements.MapObject(model, vecPos, vecRot, 255, (uint)ParentProperty);

				NetworkEvents.SendLocalEvent_CreateFurnitureItemFromCache(furnDef, m_Object, DBID);
			}
		}
	}

	public void Destroy()
	{
		if (m_Object != null)
		{
			CFurnitureDefinition furnDef = null;

			if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(FurnitureID))
			{
				furnDef = FurnitureDefinitions.g_FurnitureDefinitions[FurnitureID];
			}
			NetworkEvents.SendLocalEvent_DestroyFurnitureItemFromCache(furnDef, m_Object, DBID);

			m_Object.Destroy();
			m_Object = null;
		}
	}

	public RAGE.Elements.MapObject m_Object = null;
#endif
}

