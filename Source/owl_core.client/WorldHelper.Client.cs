//#define ALLOW_CHRISTMAS

using System;
using System.Collections.Generic;
using System.Linq;

public class CRaycastResult
{
	public int Ray = -1;
	public bool Hit = false;
	public RAGE.Vector3 EndCoords = new RAGE.Vector3();
	public RAGE.Vector3 SurfaceNormal = new RAGE.Vector3();
	public RAGE.Elements.Entity EntityHit = null;
	public int MaterialHash = -1;
	public int ShapeResult = -1;
	public int elementHitHandle = -1;
}

public enum EDoorState
{
	Unlocked,
	Locked,
	LockedWhenNotWorldDimension,
}

public static class WorldHelper
{
	static WorldHelper()
	{

	}

	public static void Init()
	{
		InteriorProps.Init();
		WorldDoors.Init();
		InteriorMusic.Init();
		NetworkEvents.InitialJoinEvent += OnInitialJoinEvent;
		NetworkEvents.UpdateWeatherState += OnUpdateWeatherState;
	}

	public static bool IsChristmas()
	{
#if ALLOW_CHRISTMAS
		if (DebugHelper.IsDebug())
		{
			return false;
		}
		else
		{
			return DateTime.Now.Month == 12;
		}
#else
		return false;
#endif
	}

	public static bool IsHalloween()
	{
		return DateTime.Now.Month == 10 && DateTime.Now.Day >= 15;
	}

	private static string[] g_strWeatherNames = new string[]
	{
		"EXTRASUNNY",
		"CLEAR",
		"CLOUDS",
		"SMOG",
		"FOGGY",
		"OVERCAST",
		"RAIN",
		"THUNDER",
		"CLEARING",
		"NEUTRAL",
		"SNOW",
		"BLIZZARD",
		"SNOWLIGHT",
		"XMAS",
		"HALLOWEEN"
	};

	public static string GetWeatherNameFromID(int weatherID)
	{
		if (weatherID < g_strWeatherNames.Length)
		{
			return g_strWeatherNames[weatherID];
		}

		return g_strWeatherNames[0];
	}

	// TODO_WEATHER: Implement smooth transitioning for weather types
	private static void OnUpdateWeatherState(int weatherValue)
	{
		if (PlayerHelper.GetLocalPlayerDimension() != 0) return;
		// clear weathers
		RAGE.Game.Misc.ClearWeatherTypePersist();
		RAGE.Game.Misc.ClearOverrideWeather();
		// Set new weather.
		RAGE.Game.Misc.SetWeatherTypeNowPersist(GetWeatherNameFromID(weatherValue));
	}

	// TODO: Fade weather (not on initial join, but later weather changes)
	private static void OnInitialJoinEvent(int day, int month, int year, int hour, int min, int sec, int weather, bool bIsDebug)
	{
		//RAGE.Game.Clock.SetClockDate(day, month, year);
		RAGE.Game.Clock.SetClockTime(hour, min, sec);
		RAGE.Game.Clock.PauseClock(true);
		RAGE.Game.Misc.SetWeatherTypeNowPersist(GetWeatherNameFromID(weather));

		DebugHelper.SetDebug(bIsDebug);
	}

	public static UInt32 GetPlayerSpecificDimension()
	{
		// NOTE: Must also update in serverside Player class
		int accountID = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.ACCOUNT_ID);
		return UInt32.MaxValue - (UInt32)accountID;
	}

	public static float GetGroundPosition(RAGE.Vector3 vecPos, bool bIgnoreDistanceCheck = false, float fZAddon = 1.0f, bool bConsiderRaycastedObjectsAsGround = true, float fRaycastHeightAddon = 0.5f, float fRaycastHeightSubtract = 0.0f)
	{
		// we need the place to be streamed in, otherwise we can't get the ground
		float fGroundZ = vecPos.Z;
		bool bGotGroundZ = false;

		float fRayCastZ = 0.0f;
		bool bGotRaycastZ = false;
		if (vecPos != null)
		{
			float fDistance = GetDistance(vecPos, RAGE.Elements.Player.LocalPlayer.Position);
			if (fDistance <= 25.0f && !bIgnoreDistanceCheck)
			{
				bGotGroundZ = RAGE.Game.Misc.GetGroundZFor3dCoord(vecPos.X, vecPos.Y, vecPos.Z + fZAddon, ref fGroundZ, false);
			}
		}

		if (bConsiderRaycastedObjectsAsGround)
		{
			CRaycastResult ray = WorldHelper.RaycastFromTo(new RAGE.Vector3(vecPos.X, vecPos.Y, vecPos.Z + fRaycastHeightAddon), new RAGE.Vector3(vecPos.X, vecPos.Y, fGroundZ - fRaycastHeightSubtract), RAGE.Elements.Player.LocalPlayer.Handle, 1);

			if (ray.Hit)
			{
				fRayCastZ = ray.EndCoords.Z;
				bGotRaycastZ = true;
			}
		}

		if (bGotRaycastZ)
		{
			fGroundZ = fRayCastZ;
		}
		else if (!bGotGroundZ && !bGotRaycastZ)
		{
			fGroundZ = vecPos.Z;
		}

		return fGroundZ;
	}

	public static float GetDistance(RAGE.Vector3 vecPos1, RAGE.Vector3 vecPos2)
	{
		if (vecPos1 == null || vecPos2 == null)
		{
			return 999999.0f;
		}

		return (vecPos1 - vecPos2).Length();
	}

	public static float GetDistance2D(RAGE.Vector3 vecPos1, RAGE.Vector3 vecPos2)
	{
		if (vecPos1 == null || vecPos2 == null)
		{
			return 999999.0f;
		}

		// make fake 2D vectors, so we can just use length... it's much faster than sqrt
		RAGE.Vector3 vecPos12D = vecPos1.CopyVector();
		vecPos12D.Z = 0.0f;

		RAGE.Vector3 vecPos22D = vecPos2.CopyVector();
		vecPos22D.Z = 0.0f;

		return (vecPos12D - vecPos22D).Length();

	}

	public static CRaycastResult RaycastFromTo(RAGE.Vector3 from, RAGE.Vector3 to, int ignoreEntity, int flags)
	{
		int ray = RAGE.Game.Shapetest.StartShapeTestRay(from.X, from.Y, from.Z, to.X, to.Y, to.Z, flags, ignoreEntity, 0);
		CRaycastResult cast = new CRaycastResult();

		int curtemp = 0;
		try
		{
			cast.ShapeResult = RAGE.Game.Shapetest.GetShapeTestResultEx(ray, ref curtemp, cast.EndCoords, cast.SurfaceNormal, ref cast.MaterialHash, ref cast.elementHitHandle);

			// TODO_RAGE_HACK: GetAtHandle is broken
			if (cast.elementHitHandle > 0)
			{
				int entityType = RAGE.Game.Entity.GetEntityType(cast.elementHitHandle);
				if (entityType == 0) // nothing
				{
					// This basically means we hit something, but it wasnt an element. It was probably the world.
					// We still set the hit flag, but the entity hit will be null
					cast.EntityHit = null;
				}
				else if (entityType == 1) // ped OR player, have to check both
				{
					cast.EntityHit = RAGE.Elements.Entities.Players.All.FirstOrDefault(x => x.Handle == cast.elementHitHandle);

					if (cast.EntityHit == null)
					{
						cast.EntityHit = RAGE.Elements.Entities.Peds.All.FirstOrDefault(x => x.Handle == cast.elementHitHandle);
					}
				}
				else if (entityType == 2) // vehicle
				{
					cast.EntityHit = OptimizationCachePool.StreamedInVehicles().FirstOrDefault(x => x.Handle == cast.elementHitHandle);
				}
				else if (entityType == 3) // object
				{
					cast.EntityHit = OptimizationCachePool.StreamedInObjects().FirstOrDefault(x => x.Handle == cast.elementHitHandle);
				}
			}
		}
		catch
		{
			curtemp = 0;
			cast.EntityHit = null;
			cast.Hit = false;
		}

		cast.Hit = curtemp > 0;
		return cast;
	}

	public static bool IsPositionConsideredAbandoned(RAGE.Vector3 vecPos)
	{
		RAGE.Vector3 vecNodePos = new RAGE.Vector3();
		bool bFoundMain = RAGE.Game.Pathfind.GetClosestVehicleNode(vecPos.X, vecPos.Y, vecPos.Z, vecNodePos, 0, 100.0f, 25.0f);

		RAGE.Vector3 vecNodePosIncludeSlow = new RAGE.Vector3();
		bool bFoundMainIncludeSlow = RAGE.Game.Pathfind.GetClosestVehicleNode(vecPos.X, vecPos.Y, vecPos.Z, vecNodePosIncludeSlow, 1, 100.0f, 25.0f);
		// dist check
		if (bFoundMain)
		{
			if ((vecNodePos - vecPos).Length() > 7.5f)
			{
				bFoundMain = false;
			}
		}

		int node = RAGE.Game.Pathfind.GetNthClosestVehicleNodeId(vecPos.X, vecPos.Y, vecPos.Z, 0, 0, 0.0f, 0.0f);
		bool bSlow = RAGE.Game.Pathfind.GetIsSlowRoadFlag(node);

		return !(!bFoundMain || bSlow);
	}
}

public static class InteriorMusic
{
	public static void Init()
	{
		foreach (string sound in g_strRockstartInteriorEmitters)
		{
			RAGE.Game.Audio.SetStaticEmitterEnabled(sound, false);
		}
	}

	private static readonly List<string> g_strRockstartInteriorEmitters = new List<string>()
	{
		"COUNTRYSIDE_ALTRUIST_CULT_01",
		"DLC_IE_Office_Garage_Mod_Shop_Radio_01",
		"DLC_IE_Office_Garage_Radio_01",
		"DLC_IE_Steal_Photo_Shoot_Pier_Radio_Emitter",
		"DLC_IE_Steal_Photo_Shoot_Sonora_Desert_Radio_Emitter",
		"DLC_IE_Steal_Photo_Shoot_Wind_Farm_Radio_Emitter",
		"DLC_IE_Steal_Pool_Party_Lake_Vine_Radio_Emitter",
		"DLC_IE_Steal_Pool_Party_Milton_Rd__Radio_Emitter",
		"DLC_IE_Warehouse_Radio_01",
		"LOS_SANTOS_AMMUNATION_GUN_RANGE",
		"LOS_SANTOS_VANILLA_UNICORN_01_STAGE",
		"LOS_SANTOS_VANILLA_UNICORN_02_MAIN_ROOM",
		"LOS_SANTOS_VANILLA_UNICORN_03_BACK_ROOM",
		"MP_ARM_WRESTLING_RADIO_01",
		"MP_ARM_WRESTLING_RADIO_02",
		"MP_ARM_WRESTLING_RADIO_03",
		"MP_ARM_WRESTLING_RADIO_04",
		"MP_ARM_WRESTLING_RADIO_05",
		"MP_ARM_WRESTLING_RADIO_06",
		"MP_ARM_WRESTLING_RADIO_07",
		"MP_ARM_WRESTLING_RADIO_08",
		"MP_ARM_WRESTLING_RADIO_09",
		"MP_ARM_WRESTLING_RADIO_10",
		"SE_AMMUNATION_CYPRESS_FLATS_GUN_RANGE",
		"SE_ba_dlc_club_exterior",
		"SE_ba_dlc_int_01_Bogs",
		"SE_ba_dlc_int_01_Entry_Hall",
		"SE_ba_dlc_int_01_Entry_Stairs",
		"SE_ba_dlc_int_01_garage",
		"SE_ba_dlc_int_01_main_area",
		"SE_ba_dlc_int_01_main_area_2",
		"SE_ba_dlc_int_01_office",
		"SE_ba_dlc_int_01_rear_L_corridor",
		"se_ba_int_02_ba_workshop_radio",
		"se_ba_int_03_ba_hktrk_radio",
		"SE_bkr_biker_dlc_int_01_BAR",
		"SE_bkr_biker_dlc_int_01_GRG",
		"SE_bkr_biker_dlc_int_01_REC",
		"SE_bkr_biker_dlc_int_02_GRG",
		"SE_bkr_biker_dlc_int_02_REC",
		"SE_COUNTRY_SAWMILL_MAIN_BUILDING",
		"SE_DLC_APT_Custom_Bedroom",
		"SE_DLC_APT_Custom_Heist_Room",
		"SE_DLC_APT_Custom_Living_Room",
		"SE_DLC_APT_Stilts_A_Bedroom",
		"SE_DLC_APT_Stilts_A_Heist_Room",
		"SE_DLC_APT_Stilts_A_Living_Room",
		"SE_DLC_APT_Stilts_B_Bedroom",
		"SE_DLC_APT_Stilts_B_Heist_Room",
		"SE_DLC_APT_Stilts_B_Living_Room",
		"SE_DLC_APT_Yacht_Bar",
		"SE_DLC_APT_Yacht_Bedroom",
		"SE_DLC_APT_Yacht_Bedroom_02",
		"SE_DLC_APT_Yacht_Bedroom_03",
		"SE_DLC_APT_Yacht_Exterior_01",
		"SE_DLC_APT_Yacht_Exterior_02",
		"SE_DLC_APT_Yacht_Exterior_03",
		"SE_DLC_APT_Yacht_Exterior_04",
		"SE_DLC_Biker_Cash_Warehouse_Radio",
		"SE_DLC_Biker_Crack_Warehouse_Radio",
		"SE_DLC_Biker_FakeID_Warehouse_Radio",
		"SE_DLC_Biker_Meth_Warehouse_Radio",
		"SE_DLC_Biker_Tequilala_Exterior_Emitter",
		"SE_DLC_Biker_Weed_Warehouse_Radio",
		"SE_DLC_BTL_Yacht_Exterior_01",
		"SE_DLC_GR_MOC_Radio_01",
		"SE_DLC_SM_Hangar_Radio_Living_Quarters_01",
		"SE_DLC_SM_Hangar_Radio_Living_Quarters_02",
		"SE_DLC_SM_Hangar_Radio_Mechanic",
		"SE_DLC_SM_Hangar_Radio_Office_01",
		"SE_DLC_SM_Hangar_Radio_Office_02",
		"SE_DLC_SM_Hangar_Radio_Office_03",
		"SE_DLC_SM_Hangar_Radio_Office_04",
		"SE_DMOD_Trailer_Radio",
		"SE_ex_int_office_01a_Radio_01",
		"SE_ex_int_office_01b_Radio_01",
		"SE_ex_int_office_01c_Radio_01",
		"SE_ex_int_office_02a_Radio_01",
		"SE_ex_int_office_02b_Radio_01",
		"SE_ex_int_office_02c_Radio_01",
		"SE_ex_int_office_03a_Radio_01",
		"SE_ex_int_office_03b_Radio_01",
		"SE_ex_int_office_03c_Radio_01",
		"SE_EXEC_WH_L_RADIO",
		"SE_EXEC_WH_M_RADIO",
		"SE_EXEC_WH_S_RADIO",
		"SE_FAMILY_2_BOAT_RADIO",
		"SE_FRANKLIN_AUNT_HOUSE_RADIO_01",
		"SE_FRANKLIN_HILLS_HOUSE_RADIO_01",
		"SE_LOS_SANTOS_EPSILONISM_BUILDING_01",
		"SE_LR_Car_Park_Radio_01",
		"SE_LS_DOCKS_RADIO_01",
		"SE_LS_DOCKS_RADIO_02",
		"SE_LS_DOCKS_RADIO_03",
		"SE_LS_DOCKS_RADIO_04",
		"SE_LS_DOCKS_RADIO_05",
		"SE_LS_DOCKS_RADIO_06",
		"SE_LS_DOCKS_RADIO_07",
		"SE_LS_DOCKS_RADIO_08",
		"SE_MICHAELS_HOUSE_RADIO",
		"SE_MP_AP_RAD_v_apart_midspaz_lounge",
		"SE_MP_AP_RAD_v_studio_lo_living",
		"SE_MP_APT_1_1",
		"SE_MP_APT_1_2",
		"SE_MP_APT_1_3",
		"SE_MP_APT_10_1",
		"SE_MP_APT_10_2",
		"SE_MP_APT_10_3",
		"SE_MP_APT_11_1",
		"SE_MP_APT_11_2",
		"SE_MP_APT_11_3",
		"SE_MP_APT_12_1",
		"SE_MP_APT_12_2",
		"SE_MP_APT_12_3",
		"SE_MP_APT_13_1",
		"SE_MP_APT_13_2",
		"SE_MP_APT_13_3",
		"SE_MP_APT_14_1",
		"SE_MP_APT_14_2",
		"SE_MP_APT_14_3",
		"SE_MP_APT_15_1",
		"SE_MP_APT_15_2",
		"SE_MP_APT_15_3",
		"SE_MP_APT_16_1",
		"SE_MP_APT_16_2",
		"SE_MP_APT_16_3",
		"SE_MP_APT_17_1",
		"SE_MP_APT_17_2",
		"SE_MP_APT_17_3",
		"SE_MP_APT_2_1",
		"SE_MP_APT_2_2",
		"SE_MP_APT_2_3",
		"SE_MP_APT_3_1",
		"SE_MP_APT_3_2",
		"SE_MP_APT_3_3",
		"SE_MP_APT_4_1",
		"SE_MP_APT_4_2",
		"SE_MP_APT_4_3",
		"SE_MP_APT_5_1",
		"SE_MP_APT_5_2",
		"SE_MP_APT_5_3",
		"SE_MP_APT_6_1",
		"SE_MP_APT_6_2",
		"SE_MP_APT_6_3",
		"SE_MP_APT_7_1",
		"SE_MP_APT_7_2",
		"SE_MP_APT_7_3",
		"SE_MP_APT_8_1",
		"SE_MP_APT_8_2",
		"SE_MP_APT_8_3",
		"SE_MP_APT_9_1",
		"SE_MP_APT_9_2",
		"SE_MP_APT_9_3",
		"SE_MP_APT_NEW_1_1",
		"SE_MP_APT_NEW_1_2",
		"SE_MP_APT_NEW_1_3",
		"SE_MP_APT_NEW_2_1",
		"SE_MP_APT_NEW_2_2",
		"SE_MP_APT_NEW_2_3",
		"SE_MP_APT_NEW_3_1",
		"SE_MP_APT_NEW_3_2",
		"SE_MP_APT_NEW_3_3",
		"SE_MP_APT_NEW_4_1",
		"SE_MP_APT_NEW_4_2",
		"SE_MP_APT_NEW_4_3",
		"SE_MP_APT_NEW_5_1",
		"SE_MP_APT_NEW_5_2",
		"SE_MP_APT_NEW_5_3",
		"SE_MP_GARAGE_L_RADIO",
		"SE_MP_GARAGE_M_RADIO",
		"SE_MP_GARAGE_S_RADIO",
		"SE_RESTAURANTS_SUNSET_13",
		"SE_Script_Placed_Prop_Emitter_Boombox",
		"SE_TREVOR_TRAILER_RADIO_01",
		"se_xm_int_01_avngr_radio",
		"se_xm_int_02_bedroom_radio",
		"se_xm_int_02_lounge_radio",
		"se_xm_x17dlc_int_sub_stream",
		"TREVOR_APARTMENT_RADIO",
		"TREVOR1_TRAILER_PARK_MAIN_STAGE_RADIO",
		"TREVOR1_TRAILER_PARK_MAIN_TRAILER_RADIO_01",
		"TREVOR1_TRAILER_PARK_MAIN_TRAILER_RADIO_02",
		"TREVOR1_TRAILER_PARK_MAIN_TRAILER_RADIO_03",
		"SE_ba_dlc_int_01_Bogs",
		"SE_ba_dlc_int_01_Entry_Hall",
		"SE_ba_dlc_int_01_Entry_Stairs",
		"SE_ba_dlc_int_01_main_area_2",
		"SE_ba_dlc_int_01_garage",
		"SE_ba_dlc_int_01_main_area",
		"SE_ba_dlc_int_01_office",
		"SE_ba_dlc_int_01_rear_L_corridor",
		"se_ba_int_02_ba_workshop_radio",
		"se_ba_int_03_ba_hktrk_radio",
		"se_ba_int_02_ba_workshop_radio",
		"se_ba_int_03_ba_hktrk_radio",
		"se_ba_int_02_ba_workshop_radio",
		"SE_DLC_AW_xs_arena_VIP_Radio",
		"SE_DLC_AW_xs_x18_int_mod_garage_radio",
		"SE_DLC_AW_xs_x18_int_mod2_garage_radio",
		"se_vw_dlc_casino_apart_Apart_Spa_Room_Water",
		"se_vw_dlc_casino_exterior_main_entrance",
		"se_vw_dlc_casino_exterior_terrace_01",
		"se_vw_dlc_casino_exterior_terrace_02",
		"se_vw_dlc_casino_exterior_terrace_03",
		"se_vw_dlc_casino_exterior_terrace_bar",
		"se_vw_dlc_casino_apart_Apart_Party_Music_01",
		"se_vw_dlc_casino_apart_Apart_Party_Music_02",
		"se_vw_dlc_casino_apart_Apart_Party_Music_03",
		"se_vw_dlc_casino_apart_Apart_Arcade_Room_radio",
		"se_vw_dlc_casino_apart_Apart_Lounge_Room_radio",
		"DLC_BTL_Nightclub_Queue_SCL",
		"SE_DLC_AW_Arena_Crowd_Background_Main",
		"DLC_H3_FM_Prep_Explosives_Radio",
		"DLC_H3_FM_Cashier_Radio_04",
		"DLC_H3_FM_Cashier_Radio_03",
		"DLC_H3_FM_Cashier_Radio_01",
		"DLC_H3_FM_Cashier_Radio_02",
		"DLC_BTL_Nightclub_SCL",
		"DLC_H3_Arcade_Main_Area_Music_Emitter",
		"DLC_H3_Arcade_Planning_Room_Radio_Emitter",
		"TV_FLOYDS_APARTMENT",
	};
}

public static class InteriorProps
{
	private static Dictionary<int, Dictionary<string, bool>> g_dictInteriorProps = new Dictionary<int, Dictionary<string, bool>>
	{
		// Casino Nightclub
		{ 281089, new Dictionary<string, bool>()
		{
			{"dj_01_lights_04", true},
			{"dj_02_lights_04", true},
			{"dj_03_lights_04", true},
			{"dj_04_lights_04", true},
			{"int01_ba_bar_content", true},
			{"int01_ba_booze_03", true},
			{"int01_ba_dj_keinemusik", true},
			{"int01_ba_dry_ice", true},
			{"int01_ba_equipment_upgrade", true},
			{"int01_ba_lightgrid_01", true},
			{"int01_ba_lights_screen", true},
			{"int01_ba_screen", true},
			{"int01_ba_security_upgrade", true},
			{"int01_ba_style02_podium", true}
		}},
		// NIGHTCLUB
		{ 271617, new Dictionary<string, bool>()
		{
			{ "Int01_ba_security_upgrade", true },
			{ "Int01_ba_equipment_setup", true },
			{ "Int01_ba_Style01", false },
			{ "Int01_ba_Style02", false },
			{ "Int01_ba_Style03", true },
			{ "Int01_ba_style01_podium", false },
			{ "Int01_ba_style02_podium", false },
			{ "Int01_ba_style03_podium", true },
			{ "Int01_ba_lights_screen", true },
			{ "Int01_ba_Screen", true },
			{ "Int01_ba_bar_content", true },
			{ "Int01_ba_booze_01", true },
			{ "Int01_ba_booze_02", false },
			{ "Int01_ba_booze_03", false },
			{ "Int01_ba_dj01", false },
			{ "Int01_ba_dj02", false },
			{ "Int01_ba_dj03", false },
			{ "Int01_ba_dj04", true },
			{ "DJ_01_Lights_01", false },
			{ "DJ_01_Lights_02", false },
			{ "DJ_01_Lights_03", false },
			{ "DJ_01_Lights_04", false },
			{ "DJ_02_Lights_01", false },
			{ "DJ_02_Lights_02", false },
			{ "DJ_02_Lights_03", false },
			{ "DJ_02_Lights_04", false },
			{ "DJ_03_Lights_01", false },
			{ "DJ_03_Lights_02", false },
			{ "DJ_03_Lights_03", false },
			{ "DJ_03_Lights_04", false },
			{ "DJ_04_Lights_01", true },
			{ "DJ_04_Lights_02", true },
			{ "DJ_04_Lights_03", true },
			{ "DJ_04_Lights_04", true },
			{ "light_rigs_off", false },
			{ "Int01_ba_lightgrid_01", true },
			{ "Int01_ba_Clutter", false },
			{ "Int01_ba_equipment_upgrade", true },
			{ "Int01_ba_clubname_01", false },
			{ "Int01_ba_clubname_02", false },
			{ "Int01_ba_clubname_03", false },
			{ "Int01_ba_clubname_04", false },
			{ "Int01_ba_clubname_05", false },
			{ "Int01_ba_clubname_06", false },
			{ "Int01_ba_clubname_07", false },
			{ "Int01_ba_clubname_08", true },
			{ "Int01_ba_clubname_09", false },
			{ "Int01_ba_dry_ice", true },
			{ "Int01_ba_deliverytruck", false },
			{ "Int01_ba_trophy04", true },
			{ "Int01_ba_trophy05", false },
			{ "Int01_ba_trophy07", false },
			{ "Int01_ba_trophy09", false },
			{ "Int01_ba_trophy08", false },
			{ "Int01_ba_trophy11", false },
			{ "Int01_ba_trophy10", false },
			{ "Int01_ba_trophy03", false },
			{ "Int01_ba_trophy01", false },
			{ "Int01_ba_trophy02", false },
			{ "Int01_ba_trad_lights", true },
			{ "Int01_ba_Worklamps", false },
		}},

		// NIGHTCLUB WAREHOUSE
		{ 271873, new Dictionary<string, bool>()
		{
			{ "Int02_ba_floor01", true },
			{ "Int02_ba_floor02", false },
			{ "Int02_ba_floor03", false },
			{ "Int02_ba_floor04", false },
			{ "Int02_ba_floor05", false },
			{ "Int02_ba_sec_upgrade_grg", true },
			{ "Int02_ba_sec_upgrade_strg", true },
			{ "Int02_ba_sec_upgrade_desk", true },
			{ "Int02_ba_storage_blocker", true },
			{ "Int02_ba_garage_blocker", true },
			{ "Int02_ba_FanBlocker01", true },
			{ "Int02_ba_equipment_upgrade", true },
			{ "Int02_ba_coke01", false },
			{ "Int02_ba_coke02", false },
			{ "Int02_ba_meth01", false },
			{ "Int02_ba_meth02", false },
			{ "Int02_ba_meth03", false },
			{ "Int02_ba_meth04", false },
			{ "Int02_ba_Weed01", false },
			{ "Int02_ba_Weed02", false },
			{ "Int02_ba_Weed03", false },
			{ "Int02_ba_Weed04", false },
			{ "Int02_ba_Weed05", false },
			{ "Int02_ba_Weed06", false },
			{ "Int02_ba_Weed07", false },
			{ "Int02_ba_Weed08", false },
			{ "Int02_ba_Weed09", false },
			{ "Int02_ba_Weed10", false },
			{ "Int02_ba_Weed11", false },
			{ "Int02_ba_Weed12", false },
			{ "Int02_ba_Weed13", false },
			{ "Int02_ba_Weed14", false },
			{ "Int02_ba_Weed15", false },
			{ "Int02_ba_Weed16", false },
			{ "Int02_ba_Forged01", true },
			{ "Int02_ba_Forged02", true },
			{ "Int02_ba_Forged03", true },
			{ "Int02_ba_Forged04", true },
			{ "Int02_ba_Forged05", true },
			{ "Int02_ba_Forged06", true },
			{ "Int02_ba_Forged07", true },
			{ "Int02_ba_Forged08", true },
			{ "Int02_ba_Forged09", true },
			{ "Int02_ba_Forged10", true },
			{ "Int02_ba_Forged11", true },
			{ "Int02_ba_Forged12", true },
			{ "Int02_ba_Cash01", true },
			{ "Int02_ba_Cash02", true },
			{ "Int02_ba_Cash03", true },
			{ "Int02_ba_Cash04", true },
			{ "Int02_ba_Cash05", true },
			{ "Int02_ba_Cash06", true },
			{ "Int02_ba_Cash07", true },
			{ "Int02_ba_Cash08", true },
			{ "Int02_ba_truckmod", true },
			{ "Int02_ba_coke_EQP", true },
			{ "Int02_ba_Cash_EQP", true },
			{ "Int02_ba_Forged_EQP", true },
			{ "Int02_ba_meth_EQP", true },
			{ "Int02_ba_Weed_EQP", true },
			{ "Int02_ba_DeskPC", true },
			{ "Int02_ba_sec_desks_L1", true },
			{ "Int02_ba_sec_desks_L2345", true },
			{ "Int02_ba_sec_upgrade_desk02", true },
			{ "Int02_ba_clutterstuff", false },
		}},

		// Car dealership close window and backdoor - Dimension 0 props
		{ 7170, new Dictionary<string, bool>()
		{
			{ "csr_beforeMission", true },
			{ "csr_afterMissionA", false },
			{ "csr_afterMissionB", false },
			{ "csr_inMission", false },
			{ "shutter_open", false },
			{ "shutter_closed", true },
		}},
		
		// Weed farm props
		{ 247297, new Dictionary<string, bool>()
		{
			{ "weed_upgrade_equip", true },
			{ "weed_security_upgrade", true },
			{ "weed_set_up", true },
			{ "weed_chairs", true },
			{ "weed_production", true },
			{ "weed_drying", true },
			{ "weed_hosea", true },
			{ "weed_hoseb", true },
			{ "weed_hosec", true },
			{ "weed_hosed", true },
			{ "weed_hosee", true },
			{ "weed_hosef", true },
			{ "weed_hoseg", true },
			{ "weed_hoseh", true },
			{ "weed_hosei", true },
			{ "weed_growtha_stage3", true },
			{ "weed_growthb_stage3", true },
			{ "weed_growthc_stage3", true },
			{ "weed_growthd_stage3", true },
			{ "weed_growthe_stage3", true },
			{ "weed_growthf_stage3", true },
			{ "weed_growthg_stage3", true },
			{ "weed_growthh_stage3", true },
			{ "weed_growthi_stage3", true },
			{ "light_growtha_stage23_upgrade", true },
			{ "light_growthb_stage23_upgrade", true },
			{ "light_growthc_stage23_upgrade", true },
			{ "light_growthd_stage23_upgrade", true },
			{ "light_growthe_stage23_upgrade", true },
			{ "light_growthf_stage23_upgrade", true },
			{ "light_growthg_stage23_upgrade", true },
			{ "light_growthh_stage23_upgrade", true },
			{ "light_growthi_stage23_upgrade", true },
		}},
		
		// Counterfeit Cash interior
		{ 247809, new Dictionary<string, bool>()
		{
			{ "counterfeit_upgrade_equip", true },
			{ "money_cutter", true },
			{ "special_chairs", true },
			{ "weed_chairs", true },
			{ "weed_production", true },
			{ "dryera_on", true },
			{ "dryerb_on", true },
			{ "dryerc_on", true },
			{ "dryerd_on", true },
			{ "counterfeit_cashpile10d", true },
			{ "counterfeit_cashpile20d", true },
			{ "counterfeit_cashpile100d", true },
		}},

		// Document Forgery Office Interior
		{ 246785, new Dictionary<string, bool>()
		{
			{ "set_up", true },
			{ "production", true },
			{ "clutter", true },
			{ "equipment_upgrade", true },
			{ "security_high", true },
			{ "interior_upgrade", true },
			{ "chair07", true },
			{ "chair06", true },
			{ "chair05", true },
			{ "chair04", true },
			{ "chair03", true },
			{ "chair02", true },
			{ "chair01", true },
		}},

		
		// Meth Lab Interior
		{ 247041, new Dictionary<string, bool>()
		{
			{ "meth_lab_upgrade", true },
			{ "meth_lab_production", true },
			{ "meth_lab_setup", true },
			{ "meth_lab_security_high", true },

		}},
		// MC Clubhouse 1
		{ 246273, new Dictionary<string, bool>()
		{
			{ "Walls_01", true },
			{ "Furnishings_01", true },
			{ "Decorative_01", true },
			{ "Mural_01", true },
			{ "Mod_Booth", true },
			{ "Gun_Locker", true },
			{ "Cash_stash1", true },
			{ "Weed_stash1", true },
			{ "id_stash1", true },
			{ "meth_stash1", true },
			{ "coke_stash1", true },
			{ "counterfeit_stash1", true },
		}},

		// MC Clubhouse 2
		{ 246529, new Dictionary<string, bool>()
		{
			{ "Walls_01", true },
			{ "Furnishings_01", true },
			{ "Decorative_01", true },
			{ "Mural_01", true },
			{ "Mod_Booth", true },
			{ "Gun_Locker", true },
			{ "lower_walls_default", true },
			{ "Weed_stash1", true },
			{ "id_stash1", true },
			{ "meth_stash1", true },
			{ "coke_stash1", true },
			{ "counterfeit_stash1", true },
		}},

		// Floyd Apartment.
		{ 171777, new Dictionary<string, bool>()
		{
			{ "swap_clean_apt", true },
			{ "layer_mess_A", false },
			{ "layer_mess_B", false },
			{ "layer_mess_C", false },
			{ "layer_sextoys_a", false },
			{ "layer_wade_sh*t", false },
			{ "swap_wade_sofa_A", false },
			{ "layer_debra_pic", true },
			{ "layer_torture", false },
			{ "swap_sofa_A", true },
			{ "swap_sofa_B", false },
			{ "layer_whiskey", false },
			{ "swap_mrJam_A", false },
			{ "swap_mrJam_B", false },
			{ "swap_mrJam_C", false },
		}},

		// Vehicle Warehouse
		{ 252673, new Dictionary<string, bool>()
		{
			{ "garage_decor_01", true },
			{ "garage_decor_02", true },
			{ "garage_decor_03", true },
			{ "garage_decor_04", true },
			{ "lighting_option01", true },
			{ "lighting_option02", true },
			{ "lighting_option03", true },
			{ "lighting_option04", true },
			{ "lighting_option05", true },
			{ "lighting_option06", true },
			{ "lighting_option07", true },
			{ "lighting_option08", true },
			{ "lighting_option09", true },
			{ "numbering_style01_n3", true },
			{ "numbering_style02_n3", true },
			{ "numbering_style03_n3", true },
			{ "numbering_style04_n3", true },
			{ "numbering_style05_n3", true },
			{ "numbering_style06_n3", true },
			{ "numbering_style07_n3", true },
			{ "numbering_style08_n3", true },
			{ "numbering_style09_n3", true },
			{ "floor_vinyl_01", true },
			{ "floor_vinyl_02", true },
			{ "floor_vinyl_03", true },
			{ "floor_vinyl_04", true },
			{ "floor_vinyl_05", true },
			{ "floor_vinyl_06", true },
			{ "floor_vinyl_07", true },
			{ "floor_vinyl_08", true },
			{ "floor_vinyl_09", true },
			{ "floor_vinyl_10", true },
			{ "floor_vinyl_11", true },
			{ "floor_vinyl_12", true },
			{ "floor_vinyl_13", true },
			{ "floor_vinyl_14", true },
			{ "floor_vinyl_15", true },
			{ "floor_vinyl_16", true },
			{ "floor_vinyl_17", true },
			{ "floor_vinyl_18", true },
			{ "floor_vinyl_19", true },
			{ "urban_style_set", true },
			{ "car_floor_hatch", true },
			{ "door_blocker", true },
		}},

		//Smuggler's run Hangar
		{ 260353, new Dictionary<string, bool>()
		{
			{ "set_tint_shell", true },
			{ "set_crane_tint", false },
			{ "set_bedroom_tint", false },
			{ "set_lighting_tint_props", true },
			{ "set_modarea", false },
			{ "set_office_basic", false },
			{ "set_office_traditional", false },
			{ "set_office_modern", true },
			{ "set_bedroom_traditional", false },
			{ "set_bedroom_modern", true },
			{ "set_bedroom_clutter", true },
			{ "set_bedroom_blinds_closed", false },
			{ "set_bedroom_blinds_open", true },
			{ "set_floor_1", true },
			{ "set_floor_2", true },
			{ "set_floor_decal_1", true },
			{ "set_floor_decal_2", false },
			{ "set_floor_decal_3", false },
			{ "set_floor_decal_4", false },
			{ "set_floor_decal_5", false },
			{ "set_floor_decal_6", false },
			{ "set_floor_decal_7", false },
			{ "set_floor_decal_8", false },
			{ "set_floor_decal_9", false },
			{ "set_lighting_hangar_a", false },
			{ "set_lighting_hangar_b", false },
			{ "set_lighting_hangar_c", false },
			{ "set_lighting_wall_neutral", true },
			{ "set_lighting_wall_tint01", false },
			{ "set_lighting_wall_tint02", false },
			{ "set_lighting_wall_tint03", false },
			{ "set_lighting_wall_tint04", false },
			{ "set_lighting_wall_tint05", false },
			{ "set_lighting_wall_tint06", false },
			{ "set_lighting_wall_tint07", false },
			{ "set_lighting_wall_tint08", false },
			{ "set_lighting_wall_tint09", false },
		}},

		// Yacht interior
		{ 279041, new Dictionary<string, bool>()
		{
			{ "sum_mp_apa_yacht", true },
			{ "sum_mp_apa_yacht_win", true },
		}}
	};

	public static void Init()
	{
		foreach (var kvPair in g_dictInteriorProps)
		{
			Dictionary<string, bool> dictProps = kvPair.Value;

			foreach (var kvPairData in dictProps)
			{
				if (kvPairData.Value)
				{
					RAGE.Game.Interior.EnableInteriorProp(kvPair.Key, kvPairData.Key);
				}
				else
				{
					RAGE.Game.Interior.DisableInteriorProp(kvPair.Key, kvPairData.Key);
				}
			}

			// refresh interior now
			RAGE.Game.Interior.RefreshInterior(kvPair.Key);
		}
	}
}

public static class WorldDoors
{
	private static bool m_bProcessingWorldDoors = false;
	private static Queue<CWorldDoor> m_queuePendingDoors = new Queue<CWorldDoor>();


	static WorldDoors()
	{

	}

	public static void Init()
	{
		RageEvents.RAGE_OnTick_PerFrame += OnTick_PerFrame;
		QueueMultiFrameDoorFix();

		NetworkEvents.LocalPlayerStreamInNewArea += (RAGE.Vector3 vecOldArea, RAGE.Vector3 vecNewArea) =>
		{
			// remove casino tables
			UInt32 hash = HashHelper.GetHashUnsigned("ch_prop_casino_blackjack_01a");
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(2523.2446f, -230.32756f, -61.108505f, 150.0f, hash, true);
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(2524.9539f, -231.9869f, -61.108505f, 150.0f, hash, true);
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(2528.7478f, -224.75551f, -61.10851f, 150.0f, hash, true);
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(2530.4624f, -226.49669f, -61.10851f, 150.0f, hash, true);

			hash = HashHelper.GetHashUnsigned("ch_prop_casino_roulette_01a");
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(2529.0981f, -230.58774f, -61.10851f, 150.0f, hash, true);
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(2524.5269f, -226.32425f, -61.10851f, 150.0f, hash, true);
			
			// Remove gates for Mirror Park Parking Lot
			hash = 227019171;
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(1212.7501f, -431.95163f, 67.30978f, 150.0f, hash, true);
			hash = 1526539404;
			RAGE.Game.Entity.CreateModelHideExcludingScriptObjects(1212.7501f, -431.95163f, 67.30978f, 150.0f, hash, true);

			QueueMultiFrameDoorFix();
		};

		// Update doors immediately on dimension change
		NetworkEvents.LocalPlayerDimensionChanged += (uint oldDimension, uint newDimension) =>
		{
			QueueMultiFrameDoorFix();
		};

	}

	private static void QueueMultiFrameDoorFix()
	{
		if (!m_bProcessingWorldDoors)
		{
			m_bProcessingWorldDoors = true;
			m_queuePendingDoors.Clear();

			// queue doors
			foreach (CWorldDoor definedDoor in g_lstDoors)
			{
				m_queuePendingDoors.Enqueue(definedDoor);
			}
		}
	}

	private static void OnTick_PerFrame()
	{
		if (m_bProcessingWorldDoors)
		{
			const int perFrameProcessLimit = 5;

			int numToProcess = perFrameProcessLimit;
			int numPending = m_queuePendingDoors.Count;
			if (numPending < numToProcess)
			{
				numToProcess = numPending;
			}

			for (int i = 0; i < numToProcess; ++i)
			{
				try
				{
					CWorldDoor door = m_queuePendingDoors.Dequeue();
					if (door != null)
					{
						door.Update();
					}
				}
				catch
				{
					// nothing is left
					m_bProcessingWorldDoors = false;
				}
			}

			if (numToProcess == 0)
			{
				// nothing is left
				m_bProcessingWorldDoors = false;
			}

		}
	}

	// TODO_POST_LAUNCH: Support for dimensions so doors can be locked sometimes and open other times?
	static private List<CWorldDoor> g_lstDoorsToUnlock = new List<CWorldDoor>()
	{

	};

	static private List<CWorldDoor> g_lstDoors = new List<CWorldDoor>()
	{
        ////////////////////////////////////
        // UNLOCKED
        ////////////////////////////////////
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(1129.51f, -982.7756f, 46.56573f) ) }, // lspd armory
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 749848321, new RAGE.Vector3(453.0793f, -983.1895f, 30.83926f) ) }, // lspd front door
        { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 320433149, new RAGE.Vector3(434.7479f, -983.2151f, 30.83926f) ) }, // lspd front door
		
		{ new CWorldDoor(EDoorState.Unlocked, 543652229, new RAGE.Vector3(322.2017f, 179.4139f, 103.5866f) ) }, // tattoo store near paleto blvd


		{ new CWorldDoor(EDoorState.Unlocked, 3082015943, new RAGE.Vector3(1141.038f, -980.3225f, 46.55986f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 3079744621, new RAGE.Vector3(434.7479f, -980.6184f, 30.83926f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 2974090917, new RAGE.Vector3(446.5728f, -980.0106f, 30.8393f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 185711165, new RAGE.Vector3(450.1041f, -984.0915f, 30.8393f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 185711165, new RAGE.Vector3(450.1041f, -981.4915f, 30.8393f) ) },

		{ new CWorldDoor(EDoorState.Unlocked, 1557126584, new RAGE.Vector3(450.1041f, -985.7384f, 30.8393f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 2271212864, new RAGE.Vector3(452.6248f, -987.3626f, 30.8393f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 749848321, new RAGE.Vector3(461.2865f, -985.3206f, 30.83926f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 3954737168, new RAGE.Vector3(464.3613f, -984.678f, 43.83443f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 185711165, new RAGE.Vector3(443.4078f, -989.4454f, 30.8393f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 185711165, new RAGE.Vector3(446.0079f, -989.4454f, 30.8393f) ) },

		{ new CWorldDoor(EDoorState.Unlocked, 270330101, new RAGE.Vector3(723.116f, -1088.831f, 23.23201f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 3472067116, new RAGE.Vector3(1174.656f, 2644.159f, 40.50673f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 3472067116, new RAGE.Vector3(1182.307f, 2644.166f, 40.50784f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 3472067116, new RAGE.Vector3(114.3135f, 6623.233f, 32.67305f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 3472067116, new RAGE.Vector3(108.8502f, 6617.877f, 32.67305f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 3668283177, new RAGE.Vector3(99.08321f, -1293.701f, 29.41868f) ) }, // inside vanilla unicorn
		{ new CWorldDoor(EDoorState.Unlocked, 3799246327, new RAGE.Vector3(113.9822f, -1297.43f, 29.41868f) ) }, // inside vanilla unicorn
		{ new CWorldDoor(EDoorState.Unlocked, 2413141389, new RAGE.Vector3(116.0046f, -1294.692f, 29.41947f) ) }, // inside vanilla unicorn
		{ new CWorldDoor(EDoorState.Unlocked, 631614199, new RAGE.Vector3(464.5701f, -992.6641f, 25.06443f) ) }, // police cell room
		{ new CWorldDoor(EDoorState.Unlocked, 2793810241, new RAGE.Vector3(-442.66f, 6015.222f, 31.86633f) ) }, // sheriff door paleto left
		{ new CWorldDoor(EDoorState.Unlocked, 2793810241, new RAGE.Vector3(-444.4985f, 6017.06f, 31.86633f) ) }, // sheriff door paleto right
		{ new CWorldDoor(EDoorState.Unlocked, 97297972, new RAGE.Vector3(-326.1122f, 6075.27f, 31.6047f) ) }, // ammunation door paleto left
		{ new CWorldDoor(EDoorState.Unlocked, 4286093708, new RAGE.Vector3(-324.2731f, 6077.109f, 31.6047f) ) }, // ammunation door paleto right
		{ new CWorldDoor(EDoorState.Unlocked, 3941780146, new RAGE.Vector3(-111.48f, 6463.94f, 31.98499f) ) }, //bank left
		{ new CWorldDoor(EDoorState.Unlocked, 2628496933, new RAGE.Vector3(-109.65f, 6462.11f, 31.98499f) ) }, // bank right
		{ new CWorldDoor(EDoorState.Unlocked, 868499217, new RAGE.Vector3(-0.05637026f, 6517.461f, 32.02779f) ) }, //clothing left
		{ new CWorldDoor(EDoorState.Unlocked, 3146141106, new RAGE.Vector3(-1.725257f, 6515.914f, 32.02779f) ) }, // clothing right

		// barbers
		{ new CWorldDoor(EDoorState.Unlocked, 2450522579, new RAGE.Vector3(280.7851f, 6232.782f, 31.84548f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 2450522579, new RAGE.Vector3(-29.86917f, -148.1571f, 57.22648f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 2450522579, new RAGE.Vector3(1932.952f, 3725.154f, 32.9944f) ) },

		{ new CWorldDoor(EDoorState.Unlocked, 1939954886, new RAGE.Vector3(-1086.089f, -2944.411f, 15.73573f) ) }, // test map apt door

		// from DrJose (Gov interior)
		{ new CWorldDoor(EDoorState.Unlocked, 1008115117, new RAGE.Vector3(14.76241f, -2.712021f, 5.149809f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 669314560, new RAGE.Vector3(14.76241f, 2.196315f, 5.149809f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 1530370263, new RAGE.Vector3(12.24583f, -8.196539f, 5.149809f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 2121636270, new RAGE.Vector3(-14.19237f, 5.765537f, 5.149809f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 803934167, new RAGE.Vector3(-12.89237f, 5.765536f, 1.143025f) ) },

        // Vehicle store
        { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1417577297, new RAGE.Vector3(-60.54582f, -1094.749f, 26.88872f) ) },
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2059227086, new RAGE.Vector3(-59.89302f, -1092.952f, 26.88362f) ) },

        ////////////////////////////////////
        // LOCKED
        ////////////////////////////////////



		{ new CWorldDoor(EDoorState.Locked, 3744620119, new RAGE.Vector3(-356.0905f, -134.7714f, 40.01295f) ) }, // mod shop
		{ new CWorldDoor(EDoorState.Locked, 3744620119, new RAGE.Vector3(-1145.898f, -1991.144f, 14.18357f) ) }, // mod shop

		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 110411286, new RAGE.Vector3(232.6054f, 214.1584f, 106.4049f) ) },
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 110411286, new RAGE.Vector3(231.5123f, 216.5177f, 106.4049f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 110411286, new RAGE.Vector3(260.6432f, 203.2052f, 106.4049f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 110411286, new RAGE.Vector3(258.2022f, 204.1005f, 106.4049f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1956494919, new RAGE.Vector3(237.7704f, 227.87f, 106.426f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1956494919, new RAGE.Vector3(236.5488f, 228.3147f, 110.4328f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 110411286, new RAGE.Vector3(259.9831f, 215.2468f, 106.4049f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 110411286, new RAGE.Vector3(259.0879f, 212.8062f, 106.4049f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1956494919, new RAGE.Vector3(256.6172f, 206.1522f, 110.4328f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 964838196, new RAGE.Vector3(260.8579f, 210.4453f, 110.4328f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 964838196, new RAGE.Vector3(262.5366f, 215.0576f, 110.4328f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 3146141106, new RAGE.Vector3(82.38156f, -1390.476f, 29.52609f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 868499217, new RAGE.Vector3(82.38156f, -1390.752f, 29.52609f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1335311341, new RAGE.Vector3(1187.202f, 2644.95f, 38.55176f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1544229216, new RAGE.Vector3(1182.646f, 2641.182f, 39.31031f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1335311341, new RAGE.Vector3(105.1518f, 6614.655f, 32.58521f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1544229216, new RAGE.Vector3(105.7772f, 6620.532f, 33.34266f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4286093708, new RAGE.Vector3(842.7685f, -1024.539f, 28.34478f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 97297972, new RAGE.Vector3(845.3694f, -1024.539f, 28.34478f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4286093708, new RAGE.Vector3(-662.6415f, -944.3256f, 21.97915f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 97297972, new RAGE.Vector3(-665.2424f, -944.3256f, 21.97915f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4286093708, new RAGE.Vector3(810.5769f, -2148.27f, 29.76892f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 97297972, new RAGE.Vector3(813.1779f, -2148.27f, 29.76892f) ) },
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4286093708, new RAGE.Vector3(18.572f, -1115.495f, 29.94694f) ) },
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 97297972, new RAGE.Vector3(16.12787f, -1114.606f, 29.94694f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 452874391, new RAGE.Vector3(6.81789f, -1098.209f, 29.94685f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4286093708, new RAGE.Vector3(243.8379f, -46.52324f, 70.09098f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 97297972, new RAGE.Vector3(244.7275f, -44.07911f, 70.09098f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2372686273, new RAGE.Vector3(-715.6154f, -157.2561f, 37.67493f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2372686273, new RAGE.Vector3(-716.6755f, -155.42f, 37.67493f) ) },
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2372686273, new RAGE.Vector3(-1456.201f, -233.3682f, 50.05648f) ) },
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2372686273, new RAGE.Vector3(-1454.782f, -231.7927f, 50.05649f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2372686273, new RAGE.Vector3(-156.439f, -304.4294f, 39.99308f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2372686273, new RAGE.Vector3(-157.1293f, -306.4341f, 39.99308f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1780022985, new RAGE.Vector3(-1201.435f, -776.8566f, 17.99184f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1780022985, new RAGE.Vector3(127.8201f, -211.8274f, 55.22751f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1780022985, new RAGE.Vector3(617.2458f, 2751.022f, 42.75777f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1780022985, new RAGE.Vector3(-3167.75f, 1055.536f, 21.53288f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 3082015943, new RAGE.Vector3(-2973.535f, 390.1414f, 15.18735f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(-2965.648f, 386.7928f, 15.18735f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(-2961.749f, 390.2573f, 15.19322f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 3082015943, new RAGE.Vector3(-1490.411f, -383.8453f, 40.30745f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(-1482.679f, -380.153f, 40.30745f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(-1482.693f, -374.9365f, 40.31332f) ) },
		{ new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 3082015943, new RAGE.Vector3(-1226.894f, -903.1218f, 12.47039f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(-1224.755f, -911.4182f, 12.47039f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(-1219.633f, -912.406f, 12.47626f) ) },
		
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1173348778, new RAGE.Vector3(1132.645f, -978.6059f, 46.55986f) ) },
		
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 145369505, new RAGE.Vector3(-822.4442f, -188.3924f, 37.81895f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2631455204, new RAGE.Vector3(-823.2001f, -187.0831f, 37.81895f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 1417577297, new RAGE.Vector3(-37.33113f, -1108.873f, 26.7198f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2059227086, new RAGE.Vector3(-39.13366f, -1108.218f, 26.7198f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2243315674, new RAGE.Vector3(-33.80989f, -1107.579f, 26.57225f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2243315674, new RAGE.Vector3(-31.72353f, -1101.847f, 26.57225f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 520341586, new RAGE.Vector3(-14.86892f, -1441.182f, 31.19323f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 703855057, new RAGE.Vector3(-25.2784f, -1431.061f, 30.83955f) ) },

		// 3 LSPD back doords
		{ new CWorldDoor(EDoorState.Unlocked, 2271212864, new RAGE.Vector3(469.9679f, -1014.452f, 26.53623f) ) },
		{ new CWorldDoor(EDoorState.Unlocked, 2271212864, new RAGE.Vector3(467.3716f, -1014.452f, 26.53623f) ) },
		{ new CWorldDoor(EDoorState.Locked, 3261965677, new RAGE.Vector3(463.4782f, -1003.538f, 25.00599f) ) },

		// 3 LSPD Jail cells
		{ new CWorldDoor(EDoorState.Locked, 631614199, new RAGE.Vector3(461.8065f, -994.4086f, 25.06443f) ) },
		{ new CWorldDoor(EDoorState.Locked, 631614199, new RAGE.Vector3(461.8065f, -997.6583f, 25.06443f) ) },
		{ new CWorldDoor(EDoorState.Locked, 631614199, new RAGE.Vector3(461.8065f, -1001.302f, 25.06443f) ) },
		// 2 Vanilla Unicorn doors
		{ new CWorldDoor(EDoorState.Locked, 3178925983, new RAGE.Vector3(127.9552f, -1298.503f, 29.41962f) ) },
		{ new CWorldDoor(EDoorState.Locked, 668467214, new RAGE.Vector3(96.09197f, -1284.854f, 29.43878f) ) },

		// Custom Map - Interior ID: 275
		{ new CWorldDoor(EDoorState.Unlocked, HashHelper.GetHashUnsigned("apa_p_mp_door_apart_door_black"), new RAGE.Vector3(351.0005f, -582.4426f, 75.2467f)) },
		
		// Custom Map - Interior ID: 274
		// Door set 1
		{ new CWorldDoor(EDoorState.Unlocked, HashHelper.GetHashUnsigned("apa_v_ilev_ss_door7"), new RAGE.Vector3(291.7641f, -579.9365f, 85.5808f)) },
		{ new CWorldDoor(EDoorState.Unlocked, HashHelper.GetHashUnsigned("apa_v_ilev_ss_door7"), new RAGE.Vector3(293.1281f, -582.5572f, 85.5808f)) },
		// Door set 2
		{ new CWorldDoor(EDoorState.Unlocked, HashHelper.GetHashUnsigned("apa_v_ilev_ss_door7"), new RAGE.Vector3(297.6233f, -592.4606f, 85.58082f)) },
		{ new CWorldDoor(EDoorState.Unlocked, HashHelper.GetHashUnsigned("apa_v_ilev_ss_door7"), new RAGE.Vector3(296.3168f, -589.8269f, 85.58079f)) },
		// Door inside
		{ new CWorldDoor(EDoorState.Unlocked, HashHelper.GetHashUnsigned("apa_p_mp_door_apart_door_black"), new RAGE.Vector3(289.5221f, -587.0021f, 85.58273f)) },
		
		// Custom Map = Interior ID: 277
		{ new CWorldDoor(EDoorState.Unlocked, 2607919673, new RAGE.Vector3(-1147.6213f, 814.06586f, 173.3686f)) },


		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4163671155, new RAGE.Vector3(443.0298f, -991.941f, 30.8393f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4163671155, new RAGE.Vector3(443.0298f, -994.5412f, 30.8393f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2691149580, new RAGE.Vector3(488.8923f, -1011.67f, 27.14583f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 741314661, new RAGE.Vector3(1844.998f, 2597.482f, 44.63626f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 741314661, new RAGE.Vector3(1818.543f, 2597.482f, 44.60749f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 741314661, new RAGE.Vector3(1806.939f, 2616.975f, 44.60093f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 4286093708, new RAGE.Vector3(-325.5749f, 6075.603f, 31.256f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 97297972, new RAGE.Vector3(-324.4629f, 6076.834f, 31.41498f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 3426294393, new RAGE.Vector3(-53.96112f, -1755.717f, 29.57094f) ) },
		/* auto set, untested */ { new CWorldDoor(EDoorState.LockedWhenNotWorldDimension, 2065277225, new RAGE.Vector3(-51.96669f, -1757.387f, 29.57094f) ) },
	};
}

class CWorldDoor
{
	public CWorldDoor(EDoorState a_State, uint a_Hash, RAGE.Vector3 a_VecPos)
	{
		Hash = a_Hash;
		Position = a_VecPos;
		State = a_State;
	}

	public bool IsLocked()
	{
		int lockedState = -1;
		float fHeading = 0.0f;
		RAGE.Game.Object.GetStateOfClosestDoorOfType(Hash, Position.X, Position.Y, Position.Z, ref lockedState, ref fHeading);

		return (lockedState == 1);
	}

	public void Lock()
	{
		if (!IsLocked())
		{
			RAGE.Game.Object.SetStateOfClosestDoorOfType(Hash, Position.X, Position.Y, Position.Z, true, 0.0f, false);
		}

	}

	public void Unlock()
	{
		if (IsLocked())
		{
			RAGE.Game.Object.SetStateOfClosestDoorOfType(Hash, Position.X, Position.Y, Position.Z, false, 0.0f, false);
		}
	}

	public void Update()
	{
		// Are we nearby enough to care?
		float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, Position);
		if (fDist <= 250.0f)
		{
			if (State == EDoorState.Locked)
			{
				Lock();
			}
			else if (State == EDoorState.Unlocked)
			{
				Unlock();
			}
			else if (State == EDoorState.LockedWhenNotWorldDimension)
			{
				if (RAGE.Elements.Player.LocalPlayer.Dimension == 0)
				{
					Unlock();
				}
				else
				{
					Lock();
				}
			}
		}
	}

	public uint Hash { get; }
	public RAGE.Vector3 Position { get; }
	public EDoorState State { get; }
}