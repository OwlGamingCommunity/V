using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class CStoreInstance : CBaseEntity
{
	public CStoreInstance(EntityDatabaseID storeID, Vector3 vecPos, float fRot, EStoreType storeType, uint dimension, EntityDatabaseID parentPropertyID, Int64 lastRobbedAt)
	{
		m_DatabaseID = storeID;
		m_vecPos = vecPos;
		m_fRot = fRot;
		m_storeType = storeType;
		m_dimension = dimension;
		m_lastRobbedAt = lastRobbedAt;
		ParentPropertyID = parentPropertyID;

		int blipSprite = -1;
		string blipName = "";

		if (storeType == EStoreType.General)
		{
			blipSprite = 52;
			blipName = "General Store";

			// TODO: This could be player set in future and come from DB
			AddItem(EItemID.PREMADE_CHAR_MASK);
			AddItem(EItemID.TACO);
			AddItem(EItemID.SPRAY_CAN);
			AddItem(EItemID.CIGARETTE_PACK_JERED);
			AddItem(EItemID.LIGHTER);
			AddItem(EItemID.WATCH);
			AddItem(EItemID.CELLPHONE);
			AddItem(EItemID.RADIO);
			AddItem(EItemID.SMALL_BAG);
			AddItem(EItemID.BACKPACK);
			AddItem(EItemID.BEER);
			AddItem(EItemID.VODKA);
			AddItem(EItemID.WHISKY);
			AddItem(EItemID.MP3_PLAYER);
			AddItem(EItemID.BOOMBOX);
			AddItem(EItemID.PLANTING_POT);
			AddItem(EItemID.FERTILIZER);
			AddItem(EItemID.WATERING_CAN);
			AddItem(EItemID.SHEERS);
			AddItem(EItemID.WEAPON_PETROLCAN);
		}
		else if (storeType == EStoreType.Guns_Legal_Handguns || storeType == EStoreType.Guns_Legal_Shotguns || storeType == EStoreType.Guns_Legal_Rifles || storeType == EStoreType.Guns_Legal_SMG || storeType == EStoreType.Ammo)
		{
			blipSprite = 110;
			blipName = "Firearms Store";

			if (storeType == EStoreType.Guns_Legal_Handguns)
			{
				AddItem(EItemID.WEAPON_STUNGUN);
				AddItem(EItemID.WEAPON_PISTOL);
				AddItem(EItemID.WEAPON_PISTOL_MK2);
				AddItem(EItemID.WEAPON_COMBATPISTOL);
				AddItem(EItemID.WEAPON_GADGETPISTOL);
				AddItem(EItemID.WEAPON_CERAMICPISTOL);
				AddItem(EItemID.WEAPON_VINTAGEPISTOL);
				AddItem(EItemID.WEAPON_SNSPISTOL);
				AddItem(EItemID.WEAPON_SNSPISTOL_MK2);
				AddItem(EItemID.WEAPON_PISTOL50);
				AddItem(EItemID.WEAPON_MARKSMANPISTOL);
				AddItem(EItemID.WEAPON_REVOLVER);
				AddItem(EItemID.WEAPON_HEAVYREVOLVER_MK2);
				AddItem(EItemID.DOUBLE_ACTION_REVOLVER);
				AddItem(EItemID.WEAPON_NAVYREVOLVER);
				AddItem(EItemID.HOLSTER);
				AddItem(EItemID.HOLSTER_LEG);

				// attachments
				// AddItem(EItemID.HANDGUN_SUPPRESSOR);
				AddItem(EItemID.HANDGUN_FLASHLIGHT);
				AddItem(EItemID.HANDGUN_EXTENDED_MAG);
				AddItem(EItemID.HANDGUN_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.HANDGUN_COMPENSATOR);
				AddItem(EItemID.HANDGUN_SMALL_SCOPE);
				AddItem(EItemID.HANDGUN_MOUNTED_SCOPE);
				AddItem(EItemID.HANDGUN_TRACER_ROUNDS);
				//AddItem(EItemID.HANDGUN_INCENDIARY_ROUNDS);
				AddItem(EItemID.HANDGUN_HOLLOW_ROUNDS);
				AddItem(EItemID.HANDGUN_FMJ_ROUNDS);
			}
			else if (storeType == EStoreType.Guns_Legal_Shotguns)
			{
				AddItem(EItemID.WEAPON_PUMPSHOTGUN);
				AddItem(EItemID.WEAPON_PUMPSHOTGUN_MK2);
				AddItem(EItemID.WEAPON_MUSKET);
				AddItem(EItemID.WEAPON_COMBATSHOTGUN);

				// attachments
				// AddItem(EItemID.SHOTGUN_SUPPRESSOR);
				AddItem(EItemID.SHOTGUN_FLASHLIGHT);
				AddItem(EItemID.SHOTGUN_EXTENDED_MAG);
				AddItem(EItemID.SHOTGUN_DRUM_MAGAZINE);
				AddItem(EItemID.SHOTGUN_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.SHOTGUN_SMALL_SCOPE);
				AddItem(EItemID.SHOTGUN_FOREGRIP);
				AddItem(EItemID.SHOTGUN_MEDIUM_SCOPE);
				AddItem(EItemID.SHOTGUN_SQUARED_MUZZLE);
				//AddItem(EItemID.SHOTGUN_DRAGONSBREATH_SHELLS);
				AddItem(EItemID.SHOTGUN_STEEL_BUCKSHOT_SHELLS);
				AddItem(EItemID.SHOTGUN_FLECHETTE_SHELLS);
				//AddItem(EItemID.SHOTGUN_EXPLOSIVE_SHELLS);
			}
			else if (storeType == EStoreType.Guns_Legal_Rifles)
			{
				AddItem(EItemID.WEAPON_SNIPERRIFLE);
				AddItem(EItemID.WEAPON_ASSAULTRIFLE);
				AddItem(EItemID.WEAPON_ASSAULTRIFLE_MK2);
				AddItem(EItemID.WEAPON_CARBINERIFLE);
				AddItem(EItemID.WEAPON_CARBINERIFLE_MK2);
				AddItem(EItemID.WEAPON_BULLPUPRIFLE);
				AddItem(EItemID.WEAPON_BULLPUPRIFLE_MK2);
				AddItem(EItemID.WEAPON_MILITARYRIFLE);
				AddItem(EItemID.WEAPON_SPECIALCARBINE);
				AddItem(EItemID.WEAPON_SPECIALCARBINE_MK2);
				AddItem(EItemID.WEAPON_ADVANCEDRIFLE);
				AddItem(EItemID.WEAPON_SPECIALCARBINE_MK2);
				AddItem(EItemID.WEAPON_COMPACTRIFLE);

				// attachments
				AddItem(EItemID.RIFLE_SUPPRESSOR);
				AddItem(EItemID.RIFLE_FLASHLIGHT);
				AddItem(EItemID.RIFLE_EXTENDED_MAG);
				AddItem(EItemID.RIFLE_DRUM_MAGAZINE);
				AddItem(EItemID.RIFLE_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.RIFLE_FLAG_MUZZLE);
				AddItem(EItemID.RIFLE_TACTICAL_MUZZLE);
				AddItem(EItemID.RIFLE_FAT_END_MUZZLE);
				AddItem(EItemID.RIFLE_PRECISION_MUZZLE);
				AddItem(EItemID.RIFLE_HEAVY_DUTY_MUZZLE);
				AddItem(EItemID.RIFLE_SLANTED_MUZZLE);
				AddItem(EItemID.RIFLE_SPLIT_END_MUZZLE);
				AddItem(EItemID.RIFLE_SQUARED_MUZZLE);
				AddItem(EItemID.RIFLE_BELL_END_MUZZLE);
				AddItem(EItemID.RIFLE_SMALL_SCOPE);
				AddItem(EItemID.RIFLE_LARGE_SCOPE);
				AddItem(EItemID.RIFLE_FOREGRIP);
				AddItem(EItemID.RIFLE_BASIC_SCOPE);
				AddItem(EItemID.RIFLE_MEDIUM_SCOPE);
				AddItem(EItemID.RIFLE_ADVANCED_SCOPE);
				AddItem(EItemID.RIFLE_ZOOM_SCOPE);
				AddItem(EItemID.RIFLE_NIGHTVISION_SCOPE);
				AddItem(EItemID.RIFLE_THERMAL_SCOPE);
				AddItem(EItemID.RIFLE_HEAVY_BARREL);
				AddItem(EItemID.RIFLE_TRACER_ROUNDS);
				//AddItem(EItemID.RIFLE_INCENDIARY_ROUNDS);
				AddItem(EItemID.RIFLE_AP_ROUNDS);
				AddItem(EItemID.RIFLE_FMJ_ROUNDS);
				//AddItem(EItemID.RIFLE_EXPLOSIVE_ROUNDS);
			}
			else if (storeType == EStoreType.Ammo)
			{
				AddItem(EItemID.AMMO_HANDGUN);
				AddItem(EItemID.AMMO_RIFLE);
				AddItem(EItemID.AMMO_SHOTGUN);
			}
			else if (storeType == EStoreType.Guns_Legal_SMG)
			{
				AddItem(EItemID.WEAPON_MICROSMG);
				AddItem(EItemID.WEAPON_SMG);
				AddItem(EItemID.WEAPON_SMG_MK2);
				AddItem(EItemID.WEAPON_ASSAULTSMG);
				AddItem(EItemID.WEAPON_COMBATPDW);
				AddItem(EItemID.WEAPON_MACHINEPISTOL);
				AddItem(EItemID.WEAPON_MINISMG);
				AddItem(EItemID.WEAPON_GUSENBERG);

				// attachments
				AddItem(EItemID.SMG_SUPPRESSOR);
				AddItem(EItemID.SMG_FLASHLIGHT);
				AddItem(EItemID.SMG_EXTENDED_MAG);
				AddItem(EItemID.SMG_DRUM_MAGAZINE);
				AddItem(EItemID.SMG_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.SMG_SMALL_SCOPE);
				AddItem(EItemID.SMG_FOREGRIP);
				AddItem(EItemID.SMG_BASIC_SCOPE);
				AddItem(EItemID.SMG_MEDIUM_SCOPE);
				AddItem(EItemID.SMG_HEAVY_BARREL);
				AddItem(EItemID.SMG_FLAT_MUZZLE);
				AddItem(EItemID.SMG_TACTICAL_MUZZLE);
				AddItem(EItemID.SMG_FAT_END_MUZZLE);
				AddItem(EItemID.SMG_PRECISION_MUZZLE);
				AddItem(EItemID.SMG_HEAVY_DUTY_MUZZLE);
				AddItem(EItemID.SMG_SLANTED_MUZZLE);
				AddItem(EItemID.SMG_SPLIT_END_MUZZLE);
				AddItem(EItemID.SMG_TRACER_ROUNDS);
				//AddItem(EItemID.SMG_INCENDIARY_ROUNDS);
				AddItem(EItemID.SMG_HOLLOW_ROUNDS);
				AddItem(EItemID.SMG_FMJ_ROUNDS);
			}
		}
		else if (storeType == EStoreType.Guns_Illegal_Handguns || storeType == EStoreType.Guns_Illegal_Shotguns || storeType == EStoreType.Guns_Illegal_Rifles || storeType == EStoreType.Guns_Illegal_Heavy || storeType == EStoreType.Guns_Illegal_Snipers || storeType == EStoreType.Guns_Illegal_SMG)
		{
			blipSprite = -1;
			blipName = "Unknown";

			if (storeType == EStoreType.Guns_Illegal_Handguns)
			{
				AddItem(EItemID.WEAPON_APPISTOL);
				AddItem(EItemID.WEAPON_HEAVYPISTOL);
				
				// attachments
				AddItem(EItemID.HANDGUN_SUPPRESSOR);
				AddItem(EItemID.HANDGUN_FLASHLIGHT);
				AddItem(EItemID.HANDGUN_EXTENDED_MAG);
				AddItem(EItemID.HANDGUN_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.HANDGUN_COMPENSATOR);
				AddItem(EItemID.HANDGUN_SMALL_SCOPE);
				AddItem(EItemID.HANDGUN_MOUNTED_SCOPE);
				AddItem(EItemID.HANDGUN_TRACER_ROUNDS);
				//AddItem(EItemID.HANDGUN_INCENDIARY_ROUNDS);
				AddItem(EItemID.HANDGUN_HOLLOW_ROUNDS);
				AddItem(EItemID.HANDGUN_FMJ_ROUNDS);
			}
			else if (storeType == EStoreType.Guns_Illegal_Shotguns)
			{
				AddItem(EItemID.WEAPON_SAWNOFFSHOTGUN);
				AddItem(EItemID.WEAPON_ASSAULTSHOTGUN);
				AddItem(EItemID.WEAPON_BULLPUPSHOTGUN);
				AddItem(EItemID.WEAPON_HEAVYSHOTGUN);
				AddItem(EItemID.WEAPON_DBSHOTGUN);
				
				// attachments
				// AddItem(EItemID.SHOTGUN_SUPPRESSOR);
				AddItem(EItemID.SHOTGUN_FLASHLIGHT);
				AddItem(EItemID.SHOTGUN_EXTENDED_MAG);
				AddItem(EItemID.SHOTGUN_DRUM_MAGAZINE);
				AddItem(EItemID.SHOTGUN_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.SHOTGUN_SMALL_SCOPE);
				AddItem(EItemID.SHOTGUN_FOREGRIP);
				AddItem(EItemID.SHOTGUN_MEDIUM_SCOPE);
				AddItem(EItemID.SHOTGUN_SQUARED_MUZZLE);
				//AddItem(EItemID.SHOTGUN_DRAGONSBREATH_SHELLS);
				AddItem(EItemID.SHOTGUN_STEEL_BUCKSHOT_SHELLS);
				AddItem(EItemID.SHOTGUN_FLECHETTE_SHELLS);
				//AddItem(EItemID.SHOTGUN_EXPLOSIVE_SHELLS);
			}
			else if (storeType == EStoreType.Guns_Illegal_Rifles)
			{
				AddItem(EItemID.WEAPON_ADVANCEDRIFLE);
				AddItem(EItemID.WEAPON_SPECIALCARBINE);
				AddItem(EItemID.WEAPON_SPECIALCARBINE_MK2);
				AddItem(EItemID.WEAPON_BULLPUPRIFLE);
				AddItem(EItemID.WEAPON_BULLPUPRIFLE_MK2);
				AddItem(EItemID.WEAPON_COMPACTRIFLE);
				
				// attachments
				AddItem(EItemID.RIFLE_SUPPRESSOR);
				AddItem(EItemID.RIFLE_FLASHLIGHT);
				AddItem(EItemID.RIFLE_EXTENDED_MAG);
				AddItem(EItemID.RIFLE_DRUM_MAGAZINE);
				AddItem(EItemID.RIFLE_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.RIFLE_FLAG_MUZZLE);
				AddItem(EItemID.RIFLE_TACTICAL_MUZZLE);
				AddItem(EItemID.RIFLE_FAT_END_MUZZLE);
				AddItem(EItemID.RIFLE_PRECISION_MUZZLE);
				AddItem(EItemID.RIFLE_HEAVY_DUTY_MUZZLE);
				AddItem(EItemID.RIFLE_SLANTED_MUZZLE);
				AddItem(EItemID.RIFLE_SPLIT_END_MUZZLE);
				AddItem(EItemID.RIFLE_SQUARED_MUZZLE);
				AddItem(EItemID.RIFLE_BELL_END_MUZZLE);
				AddItem(EItemID.RIFLE_SMALL_SCOPE);
				AddItem(EItemID.RIFLE_LARGE_SCOPE);
				AddItem(EItemID.RIFLE_FOREGRIP);
				AddItem(EItemID.RIFLE_BASIC_SCOPE);
				AddItem(EItemID.RIFLE_MEDIUM_SCOPE);
				AddItem(EItemID.RIFLE_ADVANCED_SCOPE);
				AddItem(EItemID.RIFLE_ZOOM_SCOPE);
				AddItem(EItemID.RIFLE_NIGHTVISION_SCOPE);
				AddItem(EItemID.RIFLE_THERMAL_SCOPE);
				AddItem(EItemID.RIFLE_HEAVY_BARREL);
				AddItem(EItemID.RIFLE_TRACER_ROUNDS);
				//AddItem(EItemID.RIFLE_INCENDIARY_ROUNDS);
				AddItem(EItemID.RIFLE_AP_ROUNDS);
				AddItem(EItemID.RIFLE_FMJ_ROUNDS);
				//AddItem(EItemID.RIFLE_EXPLOSIVE_ROUNDS);
			}
			else if (storeType == EStoreType.Guns_Illegal_Heavy)
			{
				AddItem(EItemID.WEAPON_MG);
				AddItem(EItemID.WEAPON_COMBATMG);
				AddItem(EItemID.WEAPON_COMBATMG_MK2);
				AddItem(EItemID.WEAPON_GUSENBERG);
			}
			else if (storeType == EStoreType.Guns_Illegal_Snipers)
			{
				AddItem(EItemID.WEAPON_HEAVYSNIPER);
				AddItem(EItemID.WEAPON_HEAVYSNIPER_MK2);
				AddItem(EItemID.WEAPON_MARKSMANRIFLE);
				AddItem(EItemID.WEAPON_MARKSMANRIFLE_MK2);
			}
			else if (storeType == EStoreType.Guns_Illegal_SMG)
			{
				AddItem(EItemID.WEAPON_MICROSMG);
				AddItem(EItemID.WEAPON_SMG);
				AddItem(EItemID.WEAPON_SMG_MK2);
				AddItem(EItemID.WEAPON_ASSAULTSMG);
				AddItem(EItemID.WEAPON_COMBATPDW);
				AddItem(EItemID.WEAPON_MACHINEPISTOL);

				// attachments
				AddItem(EItemID.SMG_SUPPRESSOR);
				AddItem(EItemID.SMG_FLASHLIGHT);
				AddItem(EItemID.SMG_EXTENDED_MAG);
				AddItem(EItemID.SMG_DRUM_MAGAZINE);
				AddItem(EItemID.SMG_HOLOGRAPHIC_SIGHT);
				AddItem(EItemID.SMG_SMALL_SCOPE);
				AddItem(EItemID.SMG_FOREGRIP);
				AddItem(EItemID.SMG_BASIC_SCOPE);
				AddItem(EItemID.SMG_MEDIUM_SCOPE);
				AddItem(EItemID.SMG_HEAVY_BARREL);
				AddItem(EItemID.SMG_FLAT_MUZZLE);
				AddItem(EItemID.SMG_TACTICAL_MUZZLE);
				AddItem(EItemID.SMG_FAT_END_MUZZLE);
				AddItem(EItemID.SMG_PRECISION_MUZZLE);
				AddItem(EItemID.SMG_HEAVY_DUTY_MUZZLE);
				AddItem(EItemID.SMG_SLANTED_MUZZLE);
				AddItem(EItemID.SMG_SPLIT_END_MUZZLE);
				AddItem(EItemID.SMG_TRACER_ROUNDS);
				//AddItem(EItemID.SMG_INCENDIARY_ROUNDS);
				AddItem(EItemID.SMG_HOLLOW_ROUNDS);
				AddItem(EItemID.SMG_FMJ_ROUNDS);
			}
		}
		else if (storeType == EStoreType.Police)
		{
			blipSprite = 498;
			blipName = "Firearms Licensing";

			AddItem(EItemID.FIREARMS_LICENSE_TIER1);
			AddItem(EItemID.FIREARMS_LICENSE_TIER2);
		}
		else if (storeType == EStoreType.Hunting)
		{
			blipSprite = 154;
			blipName = "Survival Store";

			AddItem(EItemID.WEAPON_HAMMER);
			AddItem(EItemID.WEAPON_BAT);
			AddItem(EItemID.WEAPON_GOLFCLUB);
			AddItem(EItemID.WEAPON_CROWBAR);
			AddItem(EItemID.WEAPON_HATCHET);
			AddItem(EItemID.WEAPON_SWITCHBLADE);
			AddItem(EItemID.WEAPON_BOTTLE);
			AddItem(EItemID.WEAPON_MACHETE);
			AddItem(EItemID.WEAPON_FLASHLIGHT);
			AddItem(EItemID.WEAPON_DAGGER);
			AddItem(EItemID.WEAPON_KNIFE);
			AddItem(EItemID.WEAPON_POOLCUE);
			AddItem(EItemID.WEAPON_STONE_HATCHET);
			AddItem(EItemID.OPTICAL_BINOCULARS);
			AddItem(EItemID.NV_BINOCULARS);
			AddItem(EItemID.THERMAL_BINOCULARS);
		}
		else if (storeType == EStoreType.Armor)
		{
			blipSprite = 175;
			blipName = "Armor Merchant";

			AddItem(EItemID.ARMOR_LIGHT);
			AddItem(EItemID.ARMOR_MEDIUM);
			AddItem(EItemID.ARMOR_HEAVY);
		}
		else if (storeType == EStoreType.Alcohol)
		{
			blipSprite = 93;
			blipName = "Alcohol Sales";

			AddItem(EItemID.BEER);
			AddItem(EItemID.VODKA);
			AddItem(EItemID.WHISKY);
		}
		else if (storeType == EStoreType.Drugs)
		{
			blipSprite = -1;
			blipName = "";

			AddItem(EItemID.METH);
			AddItem(EItemID.COCAINE);
			AddItem(EItemID.HEROIN);
			AddItem(EItemID.XANAX);
		}
		else if (storeType == EStoreType.Clothing)
		{
			blipSprite = 73;
			blipName = "Clothing Store";
		}
		else if (storeType == EStoreType.Barber)
		{
			blipSprite = 71;
			blipName = "Barber Shop";
		}
		else if (storeType == EStoreType.Furniture)
		{
			blipSprite = 587;
			blipName = "Furniture Store";
		}
		else if (storeType == EStoreType.Fishing)
		{
			blipSprite = 88;
			blipName = "Fishing Store";

			AddItem(EItemID.FISHING_ROD_AMATEUR);
			AddItem(EItemID.FISHING_ROD_INTERMEDIATE);
			AddItem(EItemID.FISHING_ROD_ADVANCED);
			AddItem(EItemID.FISHING_LINE);
			AddItem(EItemID.FISH_COOLER_BOX);
		}
		else if (storeType == EStoreType.Fishmonger)
		{
			blipSprite = 88;
			blipName = "Fishmonger";
		}
		else if (storeType == EStoreType.TattooArtist)
		{
			blipSprite = 79;
			blipName = "Tattoo Artist";
		}
		else if (storeType == EStoreType.PlasticSurgeon)
		{
			blipSprite = 267;
			blipName = "Plastic Surgeon";
		}
		else if (storeType == EStoreType.Tobbaco)
		{
			blipSprite = -1;
			blipName = "";

			AddItem(EItemID.CIGARETTE_PACK_JERED);
			AddItem(EItemID.CIGARETTE_PACK_BLUE);
			AddItem(EItemID.CIGAR_BASIC);
			AddItem(EItemID.CIGAR_CASE_BASIC);
			AddItem(EItemID.CIGAR_PREMIUM);
			AddItem(EItemID.CIGAR_CASE_PREMIUM);
			AddItem(EItemID.ROLLING_PAPERS);
			AddItem(EItemID.LIGHTER);
		}


		SendToAllPlayers();

		if (blipSprite != -1)
		{
			m_Blip = HelperFunctions.Blip.Create(vecPos, true, 50.0f, dimension, blipName, blipSprite);
		}
	}

	public void AddItem(EItemID a_ItemID)
	{
		m_lstItems.Add(a_ItemID);
	}

	public void SendToAllPlayers()
	{
		NetworkEventSender.SendNetworkEvent_CreateStorePed_ForAll_IncludeEveryone(m_vecPos, m_fRot, m_dimension, m_DatabaseID, m_storeType);
	}

	public void SendToPlayer(CPlayer a_Player)
	{
		NetworkEventSender.SendNetworkEvent_CreateStorePed(a_Player, m_vecPos, m_fRot, m_dimension, m_DatabaseID, m_storeType);
	}

	public List<EItemID> GetItems()
	{
		return m_lstItems;
	}

	~CStoreInstance()
	{
		Destroy();
	}

	public void Destroy()
	{
		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
			m_Blip = null;
		}

		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			NetworkEventSender.SendNetworkEvent_DestroyStorePed(player, m_vecPos, m_fRot, m_dimension);
		}
	}

	public EItemID GetItemFromIndex(int a_Index)
	{
		return m_lstItems[a_Index];
	}

	public EStoreType GetStoreType()
	{
		return m_storeType;
	}

	public void TouchStoreLastRobbedAt()
	{
		Int64 timestamp = Helpers.GetUnixTimestamp();
		Database.LegacyFunctions.SetStoreLastRobbedAt(m_DatabaseID, timestamp);
		m_lastRobbedAt = timestamp;
	}

	public bool CanBeRobbed()
	{
		Int64 now = Helpers.GetUnixTimestamp();
		return m_lastRobbedAt < now - ROBBERY_COOLDOWN;
	}

	private const int ROBBERY_COOLDOWN = 21600; // 6 hours
	public Vector3 m_vecPos { get; set; }
	private readonly float m_fRot;
	public EStoreType m_storeType { get; set; }
	public uint m_dimension { get; set; }
	private List<EItemID> m_lstItems = new List<EItemID>();
	private Int64 m_lastRobbedAt;
	private Blip m_Blip;
	public EntityDatabaseID ParentPropertyID { get; private set; }
}

