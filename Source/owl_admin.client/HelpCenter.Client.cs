using System;
using System.Collections.Generic;

public class HelpCenter
{
	private CGUIHelpCenter m_HelpCenterUI = new CGUIHelpCenter(OnUILoaded);

	private bool m_bHasPendingAdminReport = false;

	public HelpCenter()
	{
		NetworkEvents.ChangeCharacterApproved += Hide;
		NetworkEvents.ShowHelpCenter += Show;
		NetworkEvents.AdminReportEnded += OnAdminReportEnded;

		NetworkEvents.HelpRequestCommandsResponse += OnHelpRequestCommandsResponse;

		NetworkEvents.AdminNativeInteriorID += OutputNativeInterior;


		ScriptControls.SubscribeToControl(EScriptControlID.ToggleHelpCenter, Toggle);
	}

	private static void OnUILoaded()
	{

	}

	public void Show()
	{
		m_HelpCenterUI.Reset();
		m_HelpCenterUI.Init(m_bHasPendingAdminReport);

		m_HelpCenterUI.SetVisible(true, true, false);
	}

	public void Hide()
	{
		m_HelpCenterUI.SetVisible(false, false, false);
	}

	private void Toggle(EControlActionType actionType)
	{
		if (m_HelpCenterUI.IsVisible())
		{
			Hide();
		}
		else if (KeyBinds.CanProcessKeybinds()) // We can hide always, but can only show when eligible
		{
			Show();
		}
	}

	public void SubmitAdminReport(EAdminReportType reportType, string strDetails, int playerID)
	{
		RAGE.Elements.Player targetPlayer = null;

		if (playerID == -1)
		{
			playerID = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.PLAYER_ID);
		}

		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			int thisID = DataHelper.GetEntityData<int>(player, EDataNames.PLAYER_ID);

			if (thisID == playerID)
			{
				targetPlayer = player;
				break;
			}
		}

		if (targetPlayer == null)
		{
			m_bHasPendingAdminReport = false;
			Show();
		}
		else
		{
			m_bHasPendingAdminReport = true;
			NetworkEventSender.SendNetworkEvent_SubmitAdminReport(reportType, strDetails, targetPlayer);
		}
	}

	private void OnAdminReportEnded()
	{
		m_bHasPendingAdminReport = false;

		// Are we visible? if so re-init
		if (m_HelpCenterUI.IsVisible())
		{
			m_HelpCenterUI.Init(m_bHasPendingAdminReport);
		}
	}

	private void OnHelpRequestCommandsResponse(List<CommandHelpInfo> lstCommands)
	{
		foreach (CommandHelpInfo cmdInfo in lstCommands)
		{
			m_HelpCenterUI.AddCommandInfo(cmdInfo);
		}

		m_HelpCenterUI.CommitCommands();
	}

	public void CancelAdminReport()
	{
		m_bHasPendingAdminReport = false;
		NetworkEventSender.SendNetworkEvent_CancelAdminReport();
	}

	public void HelpRequestCommands()
	{
		NetworkEventSender.SendNetworkEvent_HelpRequestCommands();
	}

	public void FindPlayerForReport(string strPartialName)
	{
		strPartialName = strPartialName.ToLower();

		int playerID = -1;
		string strPlayerName = String.Empty;
		uint numMatches = 0;

		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			int thisID = DataHelper.GetEntityData<int>(player, EDataNames.PLAYER_ID);
			string strThisName = player.Name;

			if (strThisName.ToLower().Contains(strPartialName))
			{
				playerID = thisID;
				strPlayerName = strThisName;
				++numMatches;
			}
		}

		if (numMatches > 1)
		{
			playerID = -2;
		}

		m_HelpCenterUI.FindPlayerForReportResult(playerID, strPlayerName);
	}

	private void OutputNativeInterior()
	{
		RAGE.Vector3 localPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
		NotificationManager.ShowNotification("ID", RAGE.Game.Interior.GetInteriorAtCoords(localPlayerPos.X, localPlayerPos.Y, localPlayerPos.Z).ToString(), ENotificationIcon.ExclamationSign);
	}
}