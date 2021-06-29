using System;

public static class DiscordManager
{
	private static string m_strCachedStatus = String.Empty;

	static DiscordManager()
	{
		RageEvents.RAGE_OnPlayerJoin += (RAGE.Elements.Player player) => { RePublishStatus(); };
		RageEvents.RAGE_OnPlayerQuit += (RAGE.Elements.Player player) => { RePublishStatus(); };

		NetworkEvents.SetDiscordStatus += SetDiscordStatus;

		ClientTimerPool.CreateTimer(RePublishStatus, 300000);
	}

	public static void SetDiscordStatus(string strMessage)
	{
		m_strCachedStatus = strMessage;
		RePublishStatus();
	}

	// Sets previous status, but updates player count
	public static void RePublishStatus(object[] parameters = null)
	{
		// TODO_GITHUB: Replace CommunityName with your community name
		RAGE.Discord.Update(m_strCachedStatus, DebugHelper.IsDebug() ? "Development" : "CommunityName");
	}
}