using System.Collections.Generic;

public static class AchievementsList
{
	static AchievementsList()
	{

	}


	public static void Init()
	{
		// EVENTS
		NetworkEvents.ReceivedAchievements += OnReceivedAchievements;
	}

	private static void OnReceivedAchievements(int numAchievements, int maxAchievements, int totalScore, int maxScore, List<AchievementTransmissionObject> lstAchievements)
	{
		g_AchievementsListUI.InitializeAchievementInfo(totalScore, maxScore, numAchievements, maxAchievements);

		foreach (AchievementTransmissionObject achievementDef in lstAchievements)
		{
			g_AchievementsListUI.AddAchievement(achievementDef.id, achievementDef.title, achievementDef.description, achievementDef.timestamp, achievementDef.points, achievementDef.rarity, achievementDef.percent);
		}
	}

	public static void Show()
	{
		g_AchievementsListUI.Show();
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIAchievementsList g_AchievementsListUI = new CGUIAchievementsList(OnUILoaded);
}