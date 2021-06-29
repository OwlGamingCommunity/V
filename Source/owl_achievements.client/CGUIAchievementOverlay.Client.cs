using System.Collections.Generic;

class CPendingAchievement
{
	public CPendingAchievement(int a_AchievementID, string a_strTitle, string a_strCaption, int a_Points, EAchievementRarity a_Rarity, int a_Percent)
	{
		AchievementID = a_AchievementID;
		Title = a_strTitle;
		Caption = a_strCaption;
		Points = a_Points;
		Rarity = a_Rarity;
		Percent = a_Percent;
	}

	public int AchievementID { get; }
	public string Title { get; }
	public string Caption { get; }
	public int Points { get; }
	public EAchievementRarity Rarity { get; }
	public int Percent { get; }
}

internal class CGUIAchievementOverlay : CEFCore
{
	public CGUIAchievementOverlay(OnGUILoadedDelegate callbackOnLoad) : base("owl_achievements.client/achievement_overlay.html", EGUIID.AchievementOverlay, callbackOnLoad)
	{
		NetworkEvents.AwardAchievement += OnAwardAchievement;
		UIEvents.AchievementOverlay_OnFadedOut += OnFadedOut;
	}

	public override void OnLoad()
	{

	}

	// Used by UI to show
	private void ShowAchievement(int achievementID, string strTitle, string strCaption, int points, EAchievementRarity rarity, int percent)
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FREEMODE_SOUNDSET", true);
		Execute("ShowAchievement", achievementID, strTitle, strCaption, points, (int)rarity, percent);
	}

	private void OnFadedOut()
	{
		m_bShowingAchievement = false;
		PopNextAchievement();
	}

	private void PopNextAchievement()
	{
		if (!m_bShowingAchievement)
		{
			SetVisible(m_PendingAchievements.Count > 0, false, false);
			if (m_PendingAchievements.Count > 0)
			{
				CPendingAchievement achievement = m_PendingAchievements[0];
				ShowAchievement(achievement.AchievementID, achievement.Title, achievement.Caption, achievement.Points, achievement.Rarity, achievement.Percent);
				m_PendingAchievements.Remove(achievement);
				m_bShowingAchievement = true;
			}
		}
	}

	private void OnAwardAchievement(int achievementID, string strTitle, string strCaption, int points, EAchievementRarity rarity, int percent)
	{
		ScreenshotHelper.TakeScreenshot(Helpers.FormatString("achievement_{0}", achievementID));

		m_PendingAchievements.Add(new CPendingAchievement(achievementID, strTitle, strCaption, points, rarity, percent));
		PopNextAchievement();
	}

	private List<CPendingAchievement> m_PendingAchievements = new List<CPendingAchievement>();
	private bool m_bShowingAchievement = false;
}