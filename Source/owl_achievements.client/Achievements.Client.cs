using System;
using System.Collections.Generic;

public static class Achievements
{
	private const int g_CooldownMS = 300000; // only send to server once per 5 min, we do this because we dont know clientside if we have the achievement or not and don't want to spam the server after we do
	private static Dictionary<EAchievementID, DateTime> m_dictCooldowns = new Dictionary<EAchievementID, DateTime>();

	static Achievements()
	{

	}

	public static void Init()
	{
		m_AchievementOverlay = new CGUIAchievementOverlay(OnUILoaded);

		// clientside achievement detection
		RageEvents.RAGE_OnTick_OncePerSecond += CheckAchievements;
	}

	private static void CheckCooldowns()
	{
		List<EAchievementID> lstCooldownsToRemove = new List<EAchievementID>();
		foreach (var cooldown in m_dictCooldowns)
		{
			double timeSinceCooldown = (DateTime.Now - cooldown.Value).TotalMilliseconds;
			if (timeSinceCooldown >= g_CooldownMS)
			{
				lstCooldownsToRemove.Add(cooldown.Key);
			}
		}

		foreach (EAchievementID idToRemove in lstCooldownsToRemove)
		{
			m_dictCooldowns.Remove(idToRemove);
		}
	}

	private static bool HasCooldown(EAchievementID id)
	{
		return (m_dictCooldowns.ContainsKey(id));
	}

	private static void AddCooldown(EAchievementID id)
	{
		m_dictCooldowns[id] = DateTime.Now;
	}

	private static void CheckAchievements()
	{
		List<EAchievementID> lstAchievementsToUnlock = new List<EAchievementID>();

		CheckCooldowns();

		// sirens at high speed
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			if (RAGE.Elements.Player.LocalPlayer.Vehicle.IsSirenOn())
			{
				float fSpeedMilesPerHour = PlayerHelper.GetLocalPlayerVehicleMilesPerHour();

				if (fSpeedMilesPerHour >= 120.0f) // speed
				{
					lstAchievementsToUnlock.Add(EAchievementID.RunningCode);
				}
			}
		}

		// silent sirens
		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			if (RAGE.Elements.Player.LocalPlayer.Vehicle.IsSirenOn())
			{
				if (!RAGE.Elements.Player.LocalPlayer.Vehicle.IsSirenSoundOn())
				{
					lstAchievementsToUnlock.Add(EAchievementID.GoingInQuiet);
				}
			}
		}

		// check all and transmit
		foreach (EAchievementID achievement in lstAchievementsToUnlock)
		{
			if (!HasCooldown(achievement))
			{
				AddCooldown(achievement);
				NetworkEventSender.SendNetworkEvent_UnlockAchievement(achievement);
			}
		}
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIAchievementOverlay m_AchievementOverlay = null;
}