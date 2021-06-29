using System;
internal class CGUIAchievementsList : CEFCore
{
	public CGUIAchievementsList(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/achievementslist.html", EGUIID.AchievementsList, callbackOnLoad)
	{
		UIEvents.ExitAchievements += OnExitAchievements;
	}

	public override void OnLoad()
	{

	}

	private void OnExitAchievements()
	{
		SetVisible(false, false, false);
		CharacterSelection.ShowCharacterUI();
	}

	public void InitializeAchievementInfo(int totalScore, int maxScore, int numAchievements, int maxAchievements)
	{
		Execute("Initialize", totalScore, maxScore, numAchievements, maxAchievements);
	}

	public void AddAchievement(int id, string title, string description, Int64 timestamp, int points, EAchievementRarity a_rarity, int a_percent)
	{
		Execute("AddAchievement", id, title, description, timestamp, points, (int)a_rarity, a_percent);
	}

	public void Show()
	{
		SetVisible(true, true, false);
		Execute("Reset");

		NetworkEventSender.SendNetworkEvent_RequestAchievements();

		DiscordManager.SetDiscordStatus("Viewing Achievements");
	}
}