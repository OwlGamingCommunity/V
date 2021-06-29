using System;
using System.Collections.Generic;

public class AdminHud
{
	private bool m_bAdminPerfHudEnabled = false;
	private int m_LastServerFPS = 0;
	private int m_numTotalAdmins = 0;
	private Dictionary<EAdminLevel, int> m_dictAdminsOnlineByRank = null;

	public AdminHud()
	{
		m_dictAdminsOnlineByRank = new Dictionary<EAdminLevel, int>();

		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_OncePerSecond += UpdateHudNumbers;

		NetworkEvents.AdminPerfHudState += OnAdminPerfHudStateChange;
		NetworkEvents.PerfData += OnServerPerfData;
	}

	private void OnAdminPerfHudStateChange(bool bEnabled)
	{
		m_bAdminPerfHudEnabled = bEnabled;
	}

	private void OnServerPerfData(int fps)
	{
		m_LastServerFPS = fps;
	}

	private void UpdateHudNumbers()
	{
		m_numTotalAdmins = 0;

		m_dictAdminsOnlineByRank.Clear();
		foreach (EAdminLevel iterAdminLevel in Enum.GetValues(typeof(EAdminLevel)))
		{
			if (iterAdminLevel != EAdminLevel.None)
			{
				m_dictAdminsOnlineByRank.Add(iterAdminLevel, 0);
			}
		}

		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			EAdminLevel playerAdminLevel = DataHelper.GetEntityData<EAdminLevel>(player, EDataNames.ADMIN_LEVEL);
			if (playerAdminLevel != EAdminLevel.None)
			{
				m_dictAdminsOnlineByRank[playerAdminLevel]++;
				++m_numTotalAdmins;
			}
		}
	}

	private void OnRender()
	{
		if (!HUD.IsVisible())
		{
			return;
		}

		EAdminLevel adminLevel = DataHelper.GetLocalPlayerEntityData<EAdminLevel>(EDataNames.ADMIN_LEVEL);
		EScripterLevel scripterLevel = DataHelper.GetLocalPlayerEntityData<EScripterLevel>(EDataNames.SCRIPTER_LEVEL);

		// normal admin hud
		if (adminLevel != EAdminLevel.None || scripterLevel != EScripterLevel.None)
		{
			string strAdminRank = Helpers.GetAdminLevelName(adminLevel);

			bool bOnadminDuty = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ADMIN_DUTY);

			// TODO_GITHUB: Replace CommunityName with your community name
			string strStringToDisplay = Helpers.FormatString("CommunityName - {0} - {7} - {1} Admins Online - {2} Trial - {3} Game - {4} Senior - {5} Lead - {6} Head",
				strAdminRank,
				m_numTotalAdmins,
				m_dictAdminsOnlineByRank[EAdminLevel.TrialAdmin],
				m_dictAdminsOnlineByRank[EAdminLevel.Admin],
				m_dictAdminsOnlineByRank[EAdminLevel.SeniorAdmin],
				m_dictAdminsOnlineByRank[EAdminLevel.LeadAdmin],
				m_dictAdminsOnlineByRank[EAdminLevel.HeadAdmin],
				bOnadminDuty ? "ON DUTY" : "OFF DUTY");

			RAGE.Game.Graphics.DrawRect(0.5f, 0.0f, 0.38f, 0.06f, 0, 0, 0, 200, 0);
			TextHelper.Draw2D(strStringToDisplay, 0.5f, 0.0f, 0.4f, new RAGE.RGBA(255, 255, 255, 255), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);

			if (m_bAdminPerfHudEnabled)
			{
				RAGE.Game.Graphics.DrawRect(0.0f, 0.0f, 0.134f, 0.0485f, 0, 0, 0, 200, 0);
				TextHelper.Draw2D(Helpers.FormatString("Server FPS: {0}", m_LastServerFPS), 0.03f, 0.0f, 0.4f, new RAGE.RGBA(255, 255, 255, 255), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);
			}
		}
	}
}