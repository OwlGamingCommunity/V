using GTANetworkAPI;
using System.Collections.Generic;

public class AchievementSystem
{
	public AchievementSystem()
	{
		NetworkEvents.RequestAchievements += OnRequestAchievements;
		NetworkEvents.UgandaStart += Uganda_Start;
		NetworkEvents.UgandaStop += Uganda_End;
		NetworkEvents.TaughtVub += OnTaughtVub;

		NetworkEvents.UnlockAchievement += OnUnlockClientsideAchievement;
	}

	private void OnUnlockClientsideAchievement(CPlayer a_Player, EAchievementID achievementID)
	{
		a_Player.AwardAchievement(achievementID);
	}

	public void OnRequestAchievements(CPlayer a_Player)
	{
		int numAchievements = 0;
		int maxAchievements = AchievementDefinitions.g_AchievementDefinitions.Count;
		int totalScore = 0;
		int maxScore = 0;

		// Calculate max score
		foreach (CAchievementDefinition achievementDef in AchievementDefinitions.g_AchievementDefinitions.Values)
		{
			maxScore += achievementDef.Points;
		}

		int expectedAchievements = a_Player.GetAchievments().Count;

		List<AchievementTransmissionObject> achievementTransmissionObjects = new List<AchievementTransmissionObject>();
		foreach (CAchievementInstance achievementInst in a_Player.GetAchievments().Values)
		{
			CAchievementDefinition achievementDef = AchievementDefinitions.g_AchievementDefinitions[achievementInst.AchievementID];
			Database.Functions.Achievements.GetPercentOfActiveUsersWithAchievement(false, -1, achievementDef.AchievementID, (int percentOfUsers, EAchievementRarity rarity) =>
			{
				AchievementTransmissionObject achievementObject = new AchievementTransmissionObject((int)achievementDef.AchievementID, achievementDef.Title, achievementDef.Caption, achievementInst.UnlockTimestamp, achievementDef.Points, rarity, percentOfUsers);
				achievementTransmissionObjects.Add(achievementObject);

				++numAchievements;
				totalScore += achievementDef.Points;

				// are we done?
				if (achievementTransmissionObjects.Count >= expectedAchievements)
				{
					NetworkEventSender.SendNetworkEvent_ReceivedAchievements(a_Player, numAchievements, maxAchievements, totalScore, maxScore, achievementTransmissionObjects);
				}
			});
		}
	}

	public void Uganda_Start(CPlayer player)
	{
		player.Client.Rotation = new Vector3(0.0f, 0.0f, 0.1151604);
		player.AddAnimationToQueue((int)AnimationFlags.Loop, "amb@medic@standing@kneel@base", "base", false, true, true, 0, false);
	}

	public void Uganda_End(CPlayer player)
	{
		player.AwardAchievement(EAchievementID.Uganda);
		player.StopCurrentAnimation(true, true);
	}

	private void OnTaughtVub(CPlayer player)
	{
		player.AwardAchievement(EAchievementID.Vubstersmurf);
	}
};