using System;
using System.Collections.Generic;
using System.Net;

using Vector3 = GTANetworkAPI.Vector3;

public static class InactivityScannerContains
{
	public const int numDaysToConsiderInactiveForOwnerLogin = 30;
	public const int numDaysToConsiderInactiveForUse = 14;
}

public static class AchievementConstants
{
	public const int RarityPercent_VeryRare = 10;
	public const int RarityPercent_Rare = 25;
	public const int RarityPercent_Common = 100;
}

public static class FactionConstants
{
	public const uint MaxFactionMembershipCount = 10;
}

public static class SettingHelpers
{
	public static string GetDevEnvironmentSetting(string strKey)
	{
		if (System.IO.File.Exists("dev_settings.json"))
		{
			Dictionary<string, string> dictSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText("dev_settings.json"));

			if (dictSettings.TryGetValue(strKey, out string strValue))
			{
				return strValue;
			}
			else
			{
				return "null";
			}
		}
		else
		{
			return Environment.GetEnvironmentVariable(strKey);
		}
	}
}

public enum EFactionType
{
	LawEnforcement = 0,
	Medical = 1,
	Government = 2,
	Criminal = 3,
	NewsFaction = 4,
	UserCreated = 5,
	ScriptedVehicleStore = 6,
	ScriptedFurnitureStore = 7,
	ScriptedPlasticSurgeon = 8,
	UserCreatedCriminal = 9,
	Towing = 10,
}


public enum ENewsBroadcastType
{
	Television = 1,
	Interview = 2
}

public enum EPlateType
{
	Blue_White = 0,
	Yellow_Black = 1,
	Yellow_Blue = 2,
	Blue_White_2 = 3,
	Blue_White_3_EXEMPT = 4,
	Yankton = 5
};

public enum EBankSystemType
{
	ObjectATM = 0,
	Teller,
	WorldATM
}

public enum EPrisonCell
{
	One,
	Two,
	Three
}

public class CBlip
{
	public CBlip(int a_sprite, Vector3 a_position, string a_name, bool a_bShortRange)
	{
		sprite = a_sprite;
		position = a_position;
		name = a_name;
		ShortRange = a_bShortRange;
	}

	public int sprite { get; set; }
	public Vector3 position { get; set; }
	public string name { get; set; }
	public bool ShortRange { get; set; }
}

public class CScriptedCall
{
	public CScriptedCall(string number, List<long> a_lstFactions, string operatorName, string question1, string question2 = "", string question3 = "", int currentQuestion = 1, List<string> responses = null)
	{
		Number = number;
		Factions = a_lstFactions;
		Operator = operatorName;
		Question1 = question1;
		Question2 = question2;
		Question3 = question3;
		CurrentQuestion = currentQuestion;
		Responses = responses ?? new List<string>();
	}

	public string Number { get; set; }
	public List<long> Factions { get; set; }
	public string Operator { get; set; }
	public string Question1 { get; set; }
	public string Question2 { get; set; }
	public string Question3 { get; set; }
	public int CurrentQuestion { get; set; }
	public List<string> Responses { get; set; }

}

public class CBankTransaction
{
	public CBankTransaction(string a_strFrom, string a_strTo, float a_fAmount, string a_strDate)
	{
		from = a_strFrom;
		to = a_strTo;
		amount = a_fAmount;
		date = a_strDate;
	}
	public string from { get; set; }
	public string to { get; set; }
	public float amount { get; set; }
	public string date { get; set; }
}

public enum EPendingFirearmLicenseState
{
	None,
	Issued_PendingPickup,
	Revoked,
}

public enum EDonationInactivityPurchasables
{
	VehiclePurchasable = 2,
	PropertyPurchasable = 3
}

public class DutyItemGrant
{
	public int Value { get; }
	public uint Stack { get; }
	public EItemID ContainerToBindTo { get; }
	public bool IsCritical { get; }

	public DutyItemGrant(int a_Value, uint a_Stack, EItemID a_ContainerToBindTo, bool a_IsCritical)
	{
		Value = a_Value;
		Stack = a_Stack;
		ContainerToBindTo = a_ContainerToBindTo;
		IsCritical = a_IsCritical;
	}
}

public class DutyItemRelatedGrant
{
	public EItemID ItemID { get; }
	public int Value { get; }
	public uint Stack { get; }
	public EItemID ContainerToBindTo { get; }
	public bool IsCritical { get; }

	public DutyItemRelatedGrant(EItemID a_ItemID, int a_Value, uint a_Stack, EItemID a_ContainerToBindTo, bool a_IsCritical)
	{
		ItemID = a_ItemID;
		Value = a_Value;
		Stack = a_Stack;
		ContainerToBindTo = a_ContainerToBindTo;
		IsCritical = a_IsCritical;
	}
}

public static class DutyCustomSkins_Server
{
	public static Dictionary<EItemID, DutyItemGrant> DutyServersideGrants = new Dictionary<EItemID, DutyItemGrant>
	{
		{ EItemID.SPIKESTRIP, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_CARBINERIFLE_MK2, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_SPECIALCARBINE, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_SMG, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_PUMPSHOTGUN_MK2, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_HEAVYSNIPER_MK2, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_MARKSMANRIFLE, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_SNIPERRIFLE, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_GRENADELAUNCHER_SMOKE, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_SMOKEGRENADE, new DutyItemGrant(50, 1, EItemID.None, true) },
		{ EItemID.RIOT_SHIELD, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.SWAT_SHIELD, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_FLAREGUN, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_FLARE, new DutyItemGrant(50, 1, EItemID.None, true) },
		{ EItemID.WEAPON_BZGAS, new DutyItemGrant(50, 1, EItemID.None, true) },

		{ EItemID.WEAPON_PISTOL_MK2, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_STUNGUN, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.HANDCUFFS, new DutyItemGrant(-1, 1, EItemID.DUTY_BELT, true) },
		{ EItemID.WEAPON_FLASHLIGHT, new DutyItemGrant(1, 1, EItemID.DUTY_BELT, true) },
		{ EItemID.WEAPON_NIGHTSTICK, new DutyItemGrant(1, 1, EItemID.DUTY_BELT, true) },
		{ EItemID.MEGAPHONE, new DutyItemGrant(1, 1, EItemID.DUTY_BELT, true) },

		{ EItemID.MICROPHONE, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.BOOM_MIC, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.VIDEO_CAMERA, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.NEWS_CAMERA, new DutyItemGrant(1, 1, EItemID.None, true) },
		{ EItemID.WEAPON_FIREEXTINGUISHER, new DutyItemGrant(10000, 1, EItemID.None, true) },
	};

	public static Dictionary<EDutyType, List<DutyItemRelatedGrant>> DutyTypeSpecificGrants = new Dictionary<EDutyType, List<DutyItemRelatedGrant>>
	{
		{ EDutyType.Law_Enforcement, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.DUTY_VEST, 1, 1, EItemID.None, true),
				new DutyItemRelatedGrant(EItemID.DUTY_BELT, 1, 1, EItemID.None, true),
				new DutyItemRelatedGrant(EItemID.HOLSTER, 1, 1, EItemID.None, true),
				new DutyItemRelatedGrant(EItemID.HOLSTER_LEG, 1, 1, EItemID.None, true),
				new DutyItemRelatedGrant(EItemID.RADIO, 68911, 1, EItemID.None, true),
				new DutyItemRelatedGrant(EItemID.FIREARMS_LICENSING_DEVICE, 1, 1, EItemID.None, true),
			}
		},
		{ EDutyType.EMS, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.DUTY_BELT, 1, 1, EItemID.None, true),
				new DutyItemRelatedGrant(EItemID.RADIO, 68999, 1, EItemID.None, true),
			}
		},
		{ EDutyType.Fire, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.DUTY_BELT, 1, 1, EItemID.None, true),
				new DutyItemRelatedGrant(EItemID.RADIO, 68999, 1, EItemID.None, true),
			}
		}
	};

	public static Dictionary<EItemID, List<DutyItemRelatedGrant>> DutyServersideRelatedGrants = new Dictionary<EItemID, List<DutyItemRelatedGrant>>
	{
		{ EItemID.WEAPON_CARBINERIFLE_MK2, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.RIFLE_EXTENDED_MAG, 1, 1, EItemID.WEAPON_CARBINERIFLE_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_FOREGRIP, 1, 1, EItemID.WEAPON_CARBINERIFLE_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_HEAVY_BARREL, 1, 1, EItemID.WEAPON_CARBINERIFLE_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_FLASHLIGHT, 1, 1, EItemID.WEAPON_CARBINERIFLE_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_SUPPRESSOR, 1, 1, EItemID.WEAPON_CARBINERIFLE_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_SMALL_SCOPE, 1, 1, EItemID.WEAPON_CARBINERIFLE_MK2, true),
				new DutyItemRelatedGrant(EItemID.AMMO_RIFLE, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_SPECIALCARBINE, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.RIFLE_EXTENDED_MAG, 1, 1, EItemID.WEAPON_SPECIALCARBINE, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_FOREGRIP, 1, 1, EItemID.WEAPON_SPECIALCARBINE, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_FLASHLIGHT, 1, 1, EItemID.WEAPON_SPECIALCARBINE, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_BASIC_SCOPE, 1, 1, EItemID.WEAPON_SPECIALCARBINE, true),
				new DutyItemRelatedGrant(EItemID.AMMO_RIFLE, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_SMG, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.SMG_EXTENDED_MAG, 1, 1, EItemID.WEAPON_SMG, true),
				new DutyItemRelatedGrant(EItemID.SMG_FLASHLIGHT, 1, 1, EItemID.WEAPON_SMG, true),
				new DutyItemRelatedGrant(EItemID.SMG_BASIC_SCOPE, 1, 1, EItemID.WEAPON_SMG, true),
				new DutyItemRelatedGrant(EItemID.AMMO_RIFLE, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_PUMPSHOTGUN_MK2, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.SHOTGUN_STEEL_BUCKSHOT_SHELLS, 1, 1, EItemID.WEAPON_PUMPSHOTGUN_MK2, true),
				new DutyItemRelatedGrant(EItemID.SHOTGUN_FLASHLIGHT, 1, 1, EItemID.WEAPON_PUMPSHOTGUN_MK2, true),
				new DutyItemRelatedGrant(EItemID.AMMO_SHOTGUN, 1, 50, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_HEAVYSNIPER_MK2, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.RIFLE_AP_ROUNDS, 1, 1, EItemID.WEAPON_HEAVYSNIPER_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_ADVANCED_SCOPE, 1, 1, EItemID.WEAPON_HEAVYSNIPER_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_SQUARED_MUZZLE, 1, 1, EItemID.WEAPON_HEAVYSNIPER_MK2, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_HEAVY_BARREL, 1, 1, EItemID.WEAPON_HEAVYSNIPER_MK2, true),
				new DutyItemRelatedGrant(EItemID.AMMO_RIFLE, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_MARKSMANRIFLE, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.RIFLE_EXTENDED_MAG, 1, 1, EItemID.WEAPON_MARKSMANRIFLE, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_BASIC_SCOPE, 1, 1, EItemID.WEAPON_MARKSMANRIFLE, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_SUPPRESSOR, 1, 1, EItemID.WEAPON_MARKSMANRIFLE, true),
				new DutyItemRelatedGrant(EItemID.RIFLE_FOREGRIP, 1, 1, EItemID.WEAPON_MARKSMANRIFLE, true),
				new DutyItemRelatedGrant(EItemID.AMMO_RIFLE, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_SNIPERRIFLE, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.RIFLE_ADVANCED_SCOPE, 1, 1, EItemID.WEAPON_SNIPERRIFLE, true),
				new DutyItemRelatedGrant(EItemID.AMMO_RIFLE, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_GRENADELAUNCHER_SMOKE, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.AMMO_GRENADESHELL, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_FLAREGUN, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.AMMO_FLARE, 1, 50, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_PISTOL_MK2, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.HANDGUN_FLASHLIGHT, 1, 1, EItemID.WEAPON_PISTOL_MK2, true),
				new DutyItemRelatedGrant(EItemID.AMMO_HANDGUN, 1, 200, EItemID.None, true),
			}
		},

		{ EItemID.WEAPON_STUNGUN, new List<DutyItemRelatedGrant>
			{
				new DutyItemRelatedGrant(EItemID.AMMO_TASER_PRODS, 1, 200, EItemID.None, true),
			}
		}
	};
}

public enum EAdminHistoryType
{
	PUNISH_POINTS = 0,
	BAN,
	UNBAN,
	JAIL,
	UNJAIL
}


public enum ECheatType
{
	Weapon,
	SpeedHack,
	VehicleSpawnHack,
	EntityDataModified
}

public enum EAnticheatAction
{
	InformAdmins,
	Kick,
	Ban
}

public static class CustomMapConstants
{
	public static int newInteriorGC = 500;
	public static int updateInside24h = 0;
	public static int updateInsideWeek = 250;
}