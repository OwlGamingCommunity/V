using System;

internal class CGUICellphone : CEFCore
{
	public CGUICellphone(OnGUILoadedDelegate callbackOnLoad) : base("owl_chat.client/cellphone.html", EGUIID.Cellphone, callbackOnLoad)
	{
		UIEvents.CallNumber += OnCallNumber;
		UIEvents.ClosePhone += OnClosePhone;
		UIEvents.OnPressPhoneButton += OnPressPhoneButton;
		UIEvents.EndCall += OnEndCall;
		UIEvents.CallTaxi += OnCallTaxi;
		UIEvents.CancelTaxi += OnCancelTaxi;
		UIEvents.AnswerCall += OnAnswerCall;
		UIEvents.CancelCall += OnCancelCall;
		UIEvents.SavePhoneContact += OnSavePhoneContact;
		UIEvents.RemovePhoneContact += OnRemovePhoneContact;
		UIEvents.GetPhoneContacts += OnGetPhoneContacts;
		UIEvents.CreatePhoneMessage += OnCreatePhoneMessage;
		UIEvents.UpdateMessageViewed += OnUpdateMessageViewed;
		UIEvents.GetPhoneMessagesContacts += OnGetPhoneMessagesContacts;
		UIEvents.GetPhoneMessagesFromNumber += OnGetPhoneMessagesFromNumber;
		UIEvents.GetTotalUnviewedMessages += OnGetTotalUnviewedMessages;
		UIEvents.GetPhoneContactByNumber += OnGetPhoneContactByNumber;
		UIEvents.OpenMobileBankingUI += OnOpenMobileBankingUI;

	}

	public override void OnLoad()
	{

	}

	public void Initialize(bool bHasExistingTaxiRequest, bool isCalled, Int64 number)
	{
		Execute("Initialize", bHasExistingTaxiRequest, isCalled, number);
	}

	private void OnCallNumber(string number)
	{
		NetworkEventSender.SendNetworkEvent_CallNumber(number);
		// TODO_CELLPHONE: Ringtones

		EnableChatboxTyping();
	}

	private void EnableChatboxTyping()
	{
		if (IsVisible())
		{
			SetVisible(true, false, false);
			KeyBinds.SetKeybindsDisabled(false);
			CursorManager.SetCursorVisible(false, this);
			SetInputEnabled(false);
		}
	}

	private void DisableChatboxTyping()
	{
		if (IsVisible())
		{
			SetVisible(true, true, false);
			KeyBinds.SetKeybindsDisabled(true);
			CursorManager.SetCursorVisible(true, this);
			SetInputEnabled(true);
		}
	}

	public void SetTime(int hours, int mins, string date)
	{
		ExecuteDelayed_OverwriteDupes("SetTime", hours, mins, date);
	}

	public void OnCallFailed(ECallFailedReason reason)
	{
		string reasonText = "";
		switch (reason)
		{
			case ECallFailedReason.NumberNotFoundOrPlayerOffline:
				reasonText = "The entered phone number was not found/in use.";
				break;
			case ECallFailedReason.PhoneEngaged:
				reasonText = "You cannot call yourself.";
				break;
			case ECallFailedReason.NoAnswer:
				reasonText = "The person you tried to call did not answer the call.";
				break;
			case ECallFailedReason.OtherPersonCancelled:
				reasonText = "The person you tried to call cancelled the call.";
				break;
			default:
				reasonText = "Something gone wrong while calling the entered phone number.";
				break;
		}

		if (reason == ECallFailedReason.PhoneEngaged)
		{
			Execute("createPhoneAlert", "Call Failed", reasonText);
			Execute("cancelPhoneCall");
		}
		else
		{
			Execute("UpdateCallStatus", reasonText);
		}

		DisableChatboxTyping();
	}

	public void OnCallReceived(Int64 number, bool bIsConnected)
	{
		Execute("OnCallReceived", number, bIsConnected);
	}

	public void OnRemoteCallEnded()
	{
		Execute("cancelPhoneCall");
		DisableChatboxTyping();
	}

	public void OnClosePhone()
	{
		if (IsVisible())
		{
			SetVisible(false, false, false);
			NetworkEventSender.SendNetworkEvent_ClosePhone();

			RAGE.Game.Audio.PlaySoundFrontend(-1, "Put_Away", "Phone_SoundSet_Michael", true);
		}
	}

	private void OnPressPhoneButton()
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "Phone_Generic_Key_03", "HUD_MINIGAME_SOUNDSET", true);
	}

	private void OnEndCall()
	{
		NetworkEventSender.SendNetworkEvent_EndCall();
		RAGE.Game.Audio.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", true);
		DisableChatboxTyping();
	}

	private void OnCallTaxi()
	{
		NetworkEventSender.SendNetworkEvent_CallTaxi();
	}

	private void OnCancelTaxi()
	{
		NetworkEventSender.SendNetworkEvent_CancelTaxi();
	}

	private void OnAnswerCall()
	{
		NetworkEventSender.SendNetworkEvent_AnswerCall();
		EnableChatboxTyping();
	}

	private void OnCancelCall()
	{
		NetworkEventSender.SendNetworkEvent_CancelCall();
		DisableChatboxTyping();
	}

	private void OnSavePhoneContact(string entryNumber, string entryName)
	{
		NetworkEventSender.SendNetworkEvent_SavePhoneContact(entryNumber, entryName);
	}

	private void OnRemovePhoneContact(string entryNumber, string entryName)
	{
		NetworkEventSender.SendNetworkEvent_RemovePhoneContact(entryNumber, entryName);
	}

	private void OnGetPhoneContacts()
	{
		NetworkEventSender.SendNetworkEvent_GetPhoneContacts();
		DisableChatboxTyping();
	}

	private void OnOpenMobileBankingUI()
	{
		NetworkEventSender.SendNetworkEvent_Banking_ShowMobileBankingUI();
	}


	private void OnCreatePhoneMessage(string toNumber, string content)
	{
		NetworkEventSender.SendNetworkEvent_CreatePhoneMessage(toNumber, content);
		NetworkEventSender.SendNetworkEvent_SendSMSNotification(toNumber);
	}

	private void OnUpdateMessageViewed(string toNumber)
	{
		NetworkEventSender.SendNetworkEvent_UpdateMessageViewed(toNumber);
	}

	private void OnGetPhoneMessagesContacts()
	{
		NetworkEventSender.SendNetworkEvent_GetPhoneMessagesContacts();
	}

	private void OnGetPhoneMessagesFromNumber(string toNumber)
	{
		NetworkEventSender.SendNetworkEvent_GetPhoneMessagesFromNumber(toNumber);
		DisableChatboxTyping();
	}

	private void OnGetTotalUnviewedMessages()
	{
		NetworkEventSender.SendNetworkEvent_GetTotalUnviewedMessages();
	}

	private void OnGetPhoneContactByNumber(string callingNumber)
	{
		NetworkEventSender.SendNetworkEvent_GetPhoneContactByNumber(callingNumber);
	}
}