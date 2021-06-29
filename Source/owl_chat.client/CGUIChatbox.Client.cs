using System;
using System.Collections.Generic;

internal class CGUIChatbox : CEFCore
{
	private const int g_MessageHistoryLength = 100;
	private int m_MessageHistoryScrollIndex = 0;
	private bool m_bChatVisible = false;

	private List<string> m_lstMessageHistory = new List<string>();

	public CGUIChatbox(OnGUILoadedDelegate callbackOnLoad) : base("owl_chat.client/chatbox.html", EGUIID.Chatbox, callbackOnLoad)
	{
		UIEvents.OnChatBoxCommand += OnChatBoxCommand;
		UIEvents.OnChatBoxMessage += OnChatBoxMessage;

		UIEvents.ResetChatSettings += OnResetChatSettings;
		UIEvents.HideChatSettings += OnHideChatSettings;
		UIEvents.ShowChatSettings += OnShowChatSettings;
		UIEvents.SaveChatSettingsForTab += OnSaveChatSettingsForTab;
		UIEvents.SaveChatSettingsGlobal += OnSaveChatSettingsGlobal;
		UIEvents.GotRadioMessage += OnGotRadioMessage;

		UIEvents.OnChatInputVisibleChanged += OnChatInputVisibleChanged;

		NetworkEvents.InputEnabledChanged += SetGlobalInputEnabled;
		NetworkEvents.ClearChatbox += OnClearChatbox;

		ScriptControls.SubscribeToControl(EScriptControlID.HideChatInput, (EControlActionType actionType) => { Execute("SetChatInputVisible", false, "", ""); });
		ScriptControls.SubscribeToControl(EScriptControlID.SubmitChatMessage, SubmitChatMessage);

		ScriptControls.SubscribeToControl(EScriptControlID.ChatScrollUp, ScrollUp);
		ScriptControls.SubscribeToControl(EScriptControlID.ChatScrollDown, ScrollDown);
		ScriptControls.SubscribeToControl(EScriptControlID.ChatScrollToStart, ScrollToStart);
		ScriptControls.SubscribeToControl(EScriptControlID.ChatScrollToEnd, ScrollToEnd);
		ScriptControls.SubscribeToControl(EScriptControlID.ChatScrollHistoryUp, ScrollUpHistory);
		ScriptControls.SubscribeToControl(EScriptControlID.ChatScrollHistoryDown, ScrollDownHistory);

		NetworkEvents.RageClientStorageLoaded += () =>
		{
			if (RageClientStorageManager.Container.ChatHistory != null)
			{
				foreach (string strEnty in RageClientStorageManager.Container.ChatHistory)
				{
					m_lstMessageHistory.Add(strEnty);
				}
			}
		};
	}

	public void InitializeControls(int normalChat, int localOOCChat, int primaryRadioChat)
	{
		Execute("InitializeControls", normalChat, localOOCChat, primaryRadioChat);
	}

	public void SetGlobalInputEnabled(bool bEnabled)
	{
		Execute("SetGlobalInputEnabled", bEnabled);
	}

	private void SubmitChatMessage(EControlActionType actionType)
	{
		Execute("SubmitChatMessage");
		OnChatInputVisibleChanged(false);
	}

	private void ScrollUp(EControlActionType actionType)
	{
		Execute("ScrollUp");
	}

	private void ScrollDown(EControlActionType actionType)
	{
		Execute("ScrollDown");
	}

	private void ScrollToStart(EControlActionType actionType)
	{
		Execute("ScrollToStart");
	}

	private void ScrollToEnd(EControlActionType actionType)
	{
		Execute("ScrollToEnd");
	}

	private void OnChatInputVisibleChanged(bool bVisible)
	{
		m_MessageHistoryScrollIndex = -1;
		if (bVisible)
		{
			OnChatShowInput();
		}
		else
		{
			OnChatHideInput();
		}
	}

	public override void OnLoad()
	{
		RAGE.Chat.Show(false);
		RAGE.Chat.SafeMode = false; // dont need RAGE safe mode since we have our own
		MarkAsChat();
		SetVisible(true, false, false);
	}

	private void OnShowChatSettings()
	{
		SetCursorAndInputEnabled(true);
	}

	private void OnResetChatSettings()
	{
		ChatSystem.GetChatbox()?.ResetSettingsOnServer();
	}

	private void OnHideChatSettings(bool bSendToServer)
	{
		SetCursorAndInputEnabled(false);

		if (bSendToServer)
		{
			ChatSystem.GetChatbox()?.SaveSettings();
		}
	}

	private void OnSaveChatSettingsForTab(int tabIndex, string strJsonData)
	{
		ChatSystem.GetChatbox()?.UpdateSettingsForTab(tabIndex, strJsonData);
	}

	private void OnSaveChatSettingsGlobal(int max_messages_displayed, bool showChatboxBackground, float chatboxBackgroundAlpha)
	{
		ChatSystem.GetChatbox()?.UpdateSettingsGlobal(max_messages_displayed, showChatboxBackground, chatboxBackgroundAlpha);
	}

	private void OnGotRadioMessage()
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "Start_Squelch", "CB_RADIO_SFX", true);
		ClientTimerPool.CreateTimer(DoSecondRadioEffect, 200, 1);
	}

	private void DoSecondRadioEffect(object[] parameters)
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "End_Squelch", "CB_RADIO_SFX", true);
	}

	public void Initialize(int numMessagesToShow, bool showChatBackground, float chatboxBackgroundAlpha)
	{
		Execute("Initialize", numMessagesToShow, showChatBackground, chatboxBackgroundAlpha);
	}

	public void Clear()
	{
		Execute("ClearChat");
	}

	public void AddSettings(int index, string strName, bool bEnabled, Dictionary<EChatChannel, bool> dictChannels)
	{
		try
		{
			string strFiltersJSON = OwlJSON.SerializeObject(dictChannels, EJsonTrackableIdentifier.ChatboxAddSettings);
			Execute("AddSettings", index, strName, bEnabled, strFiltersJSON);
		}
		catch (Exception e)
		{
			ExceptionHelper.SendException(e);
		}

	}

	private void OnClearChatbox()
	{
		Execute("ClearChat");
	}

	private void OnChatShowInput()
	{
		m_bChatVisible = true;

		CursorManager.SetCursorVisible(true, this);

		KeyBinds.OnChatShowInput();
	}

	private void OnChatHideInput()
	{
		m_bChatVisible = false;

		CursorManager.SetCursorVisible(false, this);

		KeyBinds.OnChatHideInput();
	}

	private void PushHistory(string strHistory)
	{
		m_lstMessageHistory.Add(strHistory);

		if (m_lstMessageHistory.Count > g_MessageHistoryLength)
		{
			m_lstMessageHistory.RemoveRange(0, 1);
		}

		// store to disk
		if (RageClientStorageManager.Container.ChatHistory == null)
		{
			RageClientStorageManager.Container.ChatHistory = new List<string>();
		}

		RageClientStorageManager.Container.ChatHistory.Add(strHistory);

		if (RageClientStorageManager.Container.ChatHistory.Count > g_MessageHistoryLength)
		{
			RageClientStorageManager.Container.ChatHistory.RemoveRange(0, 1);
		}

		RageClientStorageManager.Flush();
	}

	private void ScrollUpHistory(EControlActionType actionType)
	{
		if (m_bChatVisible)
		{
			m_MessageHistoryScrollIndex++;

			if (m_MessageHistoryScrollIndex >= m_lstMessageHistory.Count)
			{
				m_MessageHistoryScrollIndex = m_lstMessageHistory.Count - 1;
			}

			if (m_MessageHistoryScrollIndex < m_lstMessageHistory.Count && m_lstMessageHistory.Count > 0)
			{
				string strText = m_lstMessageHistory[m_lstMessageHistory.Count - m_MessageHistoryScrollIndex - 1];
				Execute("SetChatInputBoxContent", strText);
			}
		}
	}

	private void ScrollDownHistory(EControlActionType actionType)
	{
		if (m_bChatVisible)
		{
			m_MessageHistoryScrollIndex--;

			if (m_MessageHistoryScrollIndex < 0)
			{
				Execute("SetChatInputBoxContent", "");
				m_MessageHistoryScrollIndex = 0;
				return;
			}

			if (m_MessageHistoryScrollIndex >= 0)
			{
				string strText = m_lstMessageHistory[m_lstMessageHistory.Count - m_MessageHistoryScrollIndex - 1];
				Execute("SetChatInputBoxContent", strText);
			}
		}
	}

	private void OnChatBoxCommand(string strCommand)
	{
		PushHistory(Helpers.FormatString("/{0}", strCommand));
		NetworkEventSender.SendNetworkEvent_PlayerRawCommand(strCommand);
	}

	private void OnChatBoxMessage(string strMessage)
	{
		PushHistory(strMessage);
	}
}