using System;
using System.Collections.Generic;

public class Chatbox
{
	private ChatSettings m_ChatSettings = new ChatSettings();

	public Chatbox()
	{
		m_ChatBox = new CGUIChatbox(OnUILoaded);
		ResetToDefaults();

		m_ChatBox.SetVisible(true, false, false);
		RAGE.Chat.Show(false);
		NetworkEvents.CharacterSelectionApproved += OnChangeCharacterApproved;
		NetworkEvents.ChangeCharacterApproved += OnCharacterChanged;

		NetworkEvents.LoadDefaultChatSettings += ResetToDefaults;
		NetworkEvents.ApplyRemoteChatSettings += ApplyRemoteChatSettings;

		// TODO: Better location
		NetworkEvents.GPS_Set += OnGPS_Set;
		NetworkEvents.GPS_Clear += OnGPS_Clear;
		RageEvents.RAGE_OnTick_OncePerSecond += GPS_CheckCloseEnough;

		NetworkEvents.OnScriptControlChanged += OnScriptControlChanged;
		NetworkEvents.ScriptUpdate += SetScriptUpdateFlag;

		UIEvents.OnDisconnectedFromServer += OnRAGEDisconnectedFromServer;

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleChatVisibility, (EControlActionType actionType) =>
		{
			if (m_ChatBox.IsVisible())
			{
				m_ChatBox.SetVisible(false, false, false);
			}
			else
			{
				m_ChatBox.SetVisible(true, false, false);
			}
		});
	}

	// TODO: Better location
	RAGE.Elements.Blip g_GPSBlip = null;
	RAGE.Vector3 g_GPSLocation = null;
	private void OnGPS_Set(RAGE.Vector3 vecPos)
	{
		NotificationManager.ShowNotification("GPS", "Location was added to your map and your GPS was set to its location.", ENotificationIcon.InfoSign);

		CleanupGPSBlip();

		g_GPSLocation = vecPos;
		g_GPSBlip = new RAGE.Elements.Blip(162, vecPos, "GPS Location", 1, 47);
		g_GPSBlip.SetAsShortRange(false);
		g_GPSBlip.SetRoute(true);
	}

	private void OnGPS_Clear()
	{
		NotificationManager.ShowNotification("GPS", "GPS location was reset.", ENotificationIcon.InfoSign);
		CleanupGPSBlip();
	}

	private void GPS_CheckCloseEnough()
	{
		if (g_GPSBlip != null)
		{
			// NOTE: Blip position doesnt work here, so we have to use a cached version
			float fDist = WorldHelper.GetDistance2D(RAGE.Elements.Player.LocalPlayer.Position, g_GPSLocation);

			if (fDist < 5.0f)
			{
				NotificationManager.ShowNotification("GPS", "You have arrived at the location.", ENotificationIcon.InfoSign);
				CleanupGPSBlip();
			}
		}
	}

	private void CleanupGPSBlip()
	{
		if (g_GPSBlip != null)
		{
			g_GPSBlip.Destroy();
			g_GPSBlip = null;
		}

		g_GPSLocation = null;
	}

	private bool m_bScriptUpdatePending = false;
	private void SetScriptUpdateFlag()
	{
		m_bScriptUpdatePending = true;
	}

	private void OnRAGEDisconnectedFromServer()
	{
		if (m_bScriptUpdatePending)
		{
			GenericMessageBoxHelper.ShowMessageBox("Script Update", "A script update has been deployed! Please reconnect to the server...\n\nIf you do not automatically reconnect, you may need to restart RAGE.", "OK", "");
		}
		else
		{
			GenericMessageBoxHelper.ShowMessageBox("Disconnected", "The connection to the server has been lost... Attempting to Reconnect\n\nIf you do not automatically reconnect, you may need to restart RAGE.", "OK", "");
		}
	}

	private void OnScriptControlChanged(EScriptControlID controlID, ConsoleKey oldKey, ConsoleKey newKey)
	{
		if (controlID == EScriptControlID.ShowChatInput || controlID == EScriptControlID.ShowChatInput_LocalOOC || controlID == EScriptControlID.ShowChatInput_PrimaryRadio)
		{
			SetChatboxControls();
		}
	}

	private void SetChatboxControls()
	{
		m_ChatBox.InitializeControls((int)ScriptControls.g_dictScriptControlDescriptors[EScriptControlID.ShowChatInput].CurrentKey, (int)ScriptControls.g_dictScriptControlDescriptors[EScriptControlID.ShowChatInput_LocalOOC].CurrentKey, (int)ScriptControls.g_dictScriptControlDescriptors[EScriptControlID.ShowChatInput_PrimaryRadio].CurrentKey);
	}

	private void ApplyRemoteChatSettings(ChatSettings chatSettings)
	{
		m_ChatSettings = chatSettings;
		ApplySettings();
	}

	public void SaveSettings()
	{
		NetworkEventSender.SendNetworkEvent_SaveChatSettings(m_ChatSettings);
	}

	public void ResetSettingsOnServer()
	{
		ResetToDefaults();
		NetworkEventSender.SendNetworkEvent_ResetChatSettings();
	}

	public void UpdateSettingsForTab(int tabIndex, string strJsonData)
	{
		if (tabIndex < m_ChatSettings.Tabs.Count)
		{
			ChatTab tab = OwlJSON.DeserializeObject<ChatTab>(strJsonData, EJsonTrackableIdentifier.ChatSettingsForTab);
			m_ChatSettings.Tabs[tabIndex] = tab;
		}
	}

	public void UpdateSettingsGlobal(int max_messages_displayed, bool showChatboxBackground, float chatboxBackgroundAlpha)
	{
		m_ChatSettings.numMessagesToShow = max_messages_displayed;
		m_ChatSettings.chatboxBackground = showChatboxBackground;
		m_ChatSettings.chatboxBackgroundAlpha = chatboxBackgroundAlpha;
	}

	private void ResetToDefaults()
	{
		m_ChatSettings.Tabs.Clear();

		m_ChatSettings.numMessagesToShow = 10;
		m_ChatSettings.chatboxBackground = true;
		m_ChatSettings.chatboxBackgroundAlpha = 0.8f;

		m_ChatSettings.Tabs.Add(new ChatTab("Chat", true, new Dictionary<EChatChannel, bool>
		{
			{ EChatChannel.Nearby, true },
			{ EChatChannel.Factions, true },
			{ EChatChannel.Global, true },
			{ EChatChannel.Notifications, true },
			{ EChatChannel.Syntax, true },
			{ EChatChannel.AdminChat, true },
			{ EChatChannel.AdminActions, true },
			{ EChatChannel.AdminReports, true }
		}));

		m_ChatSettings.Tabs.Add(new ChatTab("Notifications", true, new Dictionary<EChatChannel, bool>
		{
			{ EChatChannel.Nearby, false },
			{ EChatChannel.Factions, false },
			{ EChatChannel.Global, false },
			{ EChatChannel.Notifications, true },
			{ EChatChannel.Syntax, true },
			{ EChatChannel.AdminChat, true },
			{ EChatChannel.AdminActions, true },
			{ EChatChannel.AdminReports, true }
		}));

		m_ChatSettings.Tabs.Add(new ChatTab("Tab #3", false, new Dictionary<EChatChannel, bool>
		{
			{ EChatChannel.Nearby, true },
			{ EChatChannel.Factions, true },
			{ EChatChannel.Global, true },
			{ EChatChannel.Notifications, true },
			{ EChatChannel.Syntax, true },
			{ EChatChannel.AdminChat, true },
			{ EChatChannel.AdminActions, true },
			{ EChatChannel.AdminReports, true }
		}));

		m_ChatSettings.Tabs.Add(new ChatTab("Tab #4", false, new Dictionary<EChatChannel, bool>
		{
			{ EChatChannel.Nearby, true },
			{ EChatChannel.Factions, true },
			{ EChatChannel.Global, true },
			{ EChatChannel.Notifications, true },
			{ EChatChannel.Syntax, true },
			{ EChatChannel.AdminChat, true },
			{ EChatChannel.AdminActions, true },
			{ EChatChannel.AdminReports, true }
		}));

		ApplySettings();
	}

	private void ApplySettings()
	{
		// Apply settings
		for (int i = 0; i < m_ChatSettings.Tabs.Count; ++i)
		{
			ChatTab tab = m_ChatSettings.Tabs[i];
			m_ChatBox.AddSettings(i, tab.Name, tab.Enabled, tab.Channels);
		}

		m_ChatBox.Initialize(m_ChatSettings.numMessagesToShow, m_ChatSettings.chatboxBackground, m_ChatSettings.chatboxBackgroundAlpha);
	}

	private void OnUILoaded()
	{
		ApplySettings();
		SetChatboxControls();
	}

	private void OnChangeCharacterApproved()
	{
		m_ChatBox.Clear();
		RAGE.Chat.Show(true);
	}

	private void OnCharacterChanged()
	{
		RAGE.Chat.Show(false);
	}

	private CGUIChatbox m_ChatBox = null;
}
