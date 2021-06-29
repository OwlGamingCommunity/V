using System;
using System.Collections.Generic;

// TODO_LAUNCH: Have a more generic way of attaching items, it'll be used widely
public class Cellphone
{
	public Cellphone()
	{
		NetworkEvents.UseCellphone += OnUseCellphone;
		NetworkEvents.CallFailed += OnCallFailed;
		NetworkEvents.CallReceived += OnCallReceived;
		NetworkEvents.CallState += OnCallState;
		NetworkEvents.RemoteCallEnded += OnRemoteCallEnded;
		NetworkEvents.ChangeCharacterApproved += OnCharacterChanged;
		NetworkEvents.GotPhoneContacts += OnGotPhoneContacts;
		NetworkEvents.GotPhoneMessagesContacts += OnGotPhoneMessagesContacts;
		NetworkEvents.GotPhoneMessagesFromNumber += OnGotPhoneMessagesFromNumber;
		NetworkEvents.GotTotalUnviewedMessages += OnGotTotalUnviewedMessages;
		NetworkEvents.GotPhoneContactByNumber += OnGotPhoneContactByNumber;
		NetworkEvents.ClosePhoneByToggle += OnClosePhone;
		RageEvents.RAGE_OnEntityStreamIn += OnEntityStreamIn;

		ClientTimerPool.CreateTimer(UpdateCellPhone, 200);
		ClientTimerPool.CreateTimer(UpdateTimeHud, 100);

		ScriptControls.SubscribeToControl(EScriptControlID.TogglePhone, OnTogglePhone);
	}

	// TODO_LAUNCH: We probably have to handle stream out? Remove the phone...
	private void OnEntityStreamIn(RAGE.Elements.Entity entity)
	{
		bool isReconing = DataHelper.GetEntityData<bool>(entity, EDataNames.RECON);
		bool isInvisible = DataHelper.GetEntityData<bool>(entity, EDataNames.DISAPPEAR);
		if (entity.Type == RAGE.Elements.Type.Player && !isReconing && !isInvisible)
		{
			UpdateCellphoneAttachment((RAGE.Elements.Player)entity);
		}
	}

	private void OnGotPhoneContacts(List<KeyValuePair<string, string>> contactslist)
	{
		m_CellphoneUI.Execute("loadPhoneContacts", OwlJSON.SerializeObject(contactslist, EJsonTrackableIdentifier.OnGotPhoneContacts));
	}

	private void OnGotPhoneMessagesContacts(List<CPhoneMessageContact> messagesContactList)
	{
		m_CellphoneUI.Execute("loadPhoneMessagesContacts", OwlJSON.SerializeObject(messagesContactList, EJsonTrackableIdentifier.OnGotPhoneMessageContacts));
	}

	private void OnGotPhoneMessagesFromNumber(List<CPhoneMessage> messagesList)
	{
		m_CellphoneUI.Execute("loadPhoneMessagesFromNumber", OwlJSON.SerializeObject(messagesList, EJsonTrackableIdentifier.OnGotPhoneMessagesFromNumber));
	}

	private void OnGotTotalUnviewedMessages(int unreadMessages)
	{
		m_CellphoneUI.Execute("displayUnreadMessages", unreadMessages);
	}

	private void OnGotPhoneContactByNumber(string contactName)
	{
		m_CellphoneUI.Execute("loadContactNameByNumber", contactName);
	}

	// TODO_HELPER: Create a helper function for this
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> g_DictCellphoneAttachments = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();

	private void UpdateCellphoneAttachment(RAGE.Elements.Player player)
	{
		// TODO_CELLPHONE: Fix this attachment code
		int cellState = DataHelper.GetEntityData<int>(player, EDataNames.HAS_CELL);

		if (cellState != 0)
		{
			// Do we need to make our phone?
			if (!g_DictCellphoneAttachments.ContainsKey(player))
			{
				uint hash = HashHelper.GetHashUnsigned("p_amb_phone_01");
				AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
				{
					g_DictCellphoneAttachments.Add(player, new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension));
				});
			}

			if (g_DictCellphoneAttachments.ContainsKey(player))
			{
				// Update phone based on relevant anim state
				if (cellState == 1)
				{
					RAGE.Game.Entity.AttachEntityToEntity(g_DictCellphoneAttachments[player].Handle, player.Handle, player.GetBoneIndex(6286), 0.11f, 0.04f, 0.0f, -17, 121, 182, true, false, false, false, 0, true);
				}
				else
				{
					RAGE.Game.Entity.AttachEntityToEntity(g_DictCellphoneAttachments[player].Handle, player.Handle, player.GetBoneIndex(6286), 0.11f, 0.04f, 0.0f, -87, 191, 111, true, false, false, false, 0, true);
				}
			}
		}
		else
		{
			// Do we have a cellphone to destroy?
			if (g_DictCellphoneAttachments.ContainsKey(player))
			{
				g_DictCellphoneAttachments[player].Destroy();
				g_DictCellphoneAttachments.Remove(player);
			}
		}
	}

	private void UpdateCellPhone(object[] parameters)
	{
		foreach (var player in RAGE.Elements.Entities.Players.Streamed)
		{
			UpdateCellphoneAttachment(player);
		}
	}

	private void UpdateTimeHud(object[] parameters)
	{
		if (m_CellphoneUI.IsVisible())
		{
			int hours = RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetClockHours);
			int mins = RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetClockMinutes);

			string strDateString = DateTime.Now.ToString("dddd, dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat);

			m_CellphoneUI.SetTime(hours, mins, strDateString);
		}
	}

	private void OnCharacterChanged()
	{
		m_CellphoneUI.OnClosePhone();
	}

	private void OnUseCellphone(bool bHasExistingTaxiRequest, bool isCalled, Int64 number)
	{
		m_CellphoneUI.Initialize(bHasExistingTaxiRequest, isCalled, number);
		m_CellphoneUI.SetVisible(true, true, false);

		// TODO_CELLPHONE: Calling state OR dont let them close it whilst in call

		RAGE.Game.Audio.PlaySoundFrontend(-1, "Pull_Out", "Phone_SoundSet_Franklin", true);
	}

	private void OnCallFailed(ECallFailedReason reason)
	{
		m_CellphoneUI.OnCallFailed(reason);
	}

	private void OnCallReceived(bool bHasExistingTaxiRequest, Int64 number)
	{
		OnUseCellphone(bHasExistingTaxiRequest, true, number);
		OnCallState(number, false);
		// TODO_CELLPHONE: Ringtones
	}

	private void OnCallState(Int64 number, bool bIsConnected)
	{
		m_CellphoneUI.OnCallReceived(number, bIsConnected);
	}

	private void OnRemoteCallEnded()
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "Hang_Up", "Phone_SoundSet_Michael", true);
		m_CellphoneUI.OnRemoteCallEnded();
	}

	private void OnTogglePhone(EControlActionType actionType)
	{
		NetworkEventSender.SendNetworkEvent_GetPhoneState(m_CellphoneUI.IsVisible());
	}

	private void OnClosePhone()
	{
		if (m_CellphoneUI.IsVisible())
		{
			m_CellphoneUI.SetVisible(false, false, false);
			NetworkEventSender.SendNetworkEvent_ClosePhone();
			RAGE.Game.Audio.PlaySoundFrontend(-1, "Put_Away", "Phone_SoundSet_Michael", true);
		}
	}

	private CGUICellphone m_CellphoneUI = new CGUICellphone(() => { });
}