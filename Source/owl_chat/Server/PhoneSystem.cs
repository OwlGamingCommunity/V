using System;
using System.Collections.Generic;
using System.Linq;

// TODO_CELLPHONE: handle death, change char, quit, etc.
public class PhoneSystem
{

	private readonly List<CScriptedCall> ScriptedCalls = new List<CScriptedCall>() {
		new CScriptedCall("911", new List<long>() { 1, 2 }, "911 Operator", "911 emergency, Which emergency service do you require?", "Can you tell me your name please?", "Please state your emergency."),
		new CScriptedCall("311", new List<long>() { 1 }, "LSPD Operator", "LSPD Hotline. Please state your location.", "Can you please describe the reason for your call?"),
		new CScriptedCall("411", new List<long>() { 2 }, "LSFD Operator", "LSFD Hotline. Please state your location.", "Can you please tell us the reason for your call?"),
		new CScriptedCall("511", new List<long>() { 3 }, "Gov Employee", "Government of Los Santos. How can we help you?"),
		new CScriptedCall("9021", new List<long>() { 134 }, "LSCTS Dispatcher", "Hello, this is Los Santos County Traffic Services hotline. How can we help you today?", "Where do you want our unit to be dispatched?"),
		new CScriptedCall("2600", new List<long>() { 67 }, "Receptionist", "You've reached Saint Ernest Medical Center. Can I get your name please?", "How can we help you?"),
		new CScriptedCall("699", new List<long>() { 15 }, "Receptionist", "You've reached Sabre Ltd. Please state your name.", "How can we help you?"),
		new CScriptedCall("711", new List<long>() { 12 }, "Operator", "Superior Court of San Andreas, please state your name.", "How may we assist you?"),
		new CScriptedCall("811", new List<long>() { 12 }, "Operator", "Federal Task Force, please state your name.", "How may we assist you?"),
	};
	public PhoneSystem()
	{
		CommandManager.RegisterCommand("p", "Talks on a phone", new Action<CPlayer, CVehicle, string>(PhoneCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);

		NetworkEvents.ClosePhone += ClosePhone;
		NetworkEvents.CallNumber += CallNumber;
		NetworkEvents.EndCall += EndCall;
		NetworkEvents.CallTaxi += OnCallTaxi;
		NetworkEvents.CancelTaxi += OnRequestCancelTaxi;
		NetworkEvents.AnswerCall += AnswerCall;
		NetworkEvents.CancelCall += CancelCall;
		NetworkEvents.SavePhoneContact += SavePhoneContact;
		NetworkEvents.RemovePhoneContact += RemovePhoneContact;
		NetworkEvents.GetPhoneContacts += GetPhoneContacts;
		NetworkEvents.CreatePhoneMessage += CreatePhoneMessage;
		NetworkEvents.GetPhoneMessagesContacts += GetPhoneMessagesContacts;
		NetworkEvents.GetPhoneMessagesFromNumber += GetPhoneMessagesFromNumber;
		NetworkEvents.SendSMSNotification += SendSMSNotification;
		NetworkEvents.UpdateMessageViewed += UpdateMessageViewed;
		NetworkEvents.GetTotalUnviewedMessages += GetTotalUnviewedMessages;
		NetworkEvents.GetPhoneContactByNumber += GetPhoneContactByNumber;
		NetworkEvents.GetPhoneState += OnGetPhoneState;

		RageEvents.RAGE_OnUpdate += OnUpdate;
	}

	public void OnUpdate()
	{
		// Lets expire any calls that have done so
		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			if (player.IsInCall(out CPlayer.ECallState callState))
			{
				if (player.HasOutgoingCallExpired())
				{
					CPlayer TargetPlayer = player.GetPlayerIsInCallWith();
					if (TargetPlayer != null)
					{
						NetworkEventSender.SendNetworkEvent_RemoteCallEnded(TargetPlayer);
					}

					EndCall(player);
					NetworkEventSender.SendNetworkEvent_CallFailed(player, ECallFailedReason.NoAnswer);
				}
			}
		}
	}

	public void EndCall(CPlayer a_Player)
	{
		if (a_Player != null)
		{
			// Were we actually in a call?
			if (a_Player.IsInCall(out CPlayer.ECallState callState))
			{
				CPlayer TargetPlayer = a_Player.GetPlayerIsInCallWith();

				if (TargetPlayer != null)
				{
					if (TargetPlayer.IsInCall(out CPlayer.ECallState targetPlayerCallState))
					{
						NetworkEventSender.SendNetworkEvent_RemoteCallEnded(TargetPlayer);

						if (targetPlayerCallState == CPlayer.ECallState.Connected)
						{
							if (a_Player != TargetPlayer)
							{
								TargetPlayer.PushChatMessage(EChatChannel.Nearby, "Phone: ** The line goes dead **");
								HelperFunctions.Chat.SendAmeMessage(TargetPlayer, "hangs up on their cellphone.");
							}

							a_Player.IsCallingHotline = false;
							a_Player.CallingHotline = null;
							new Logging.Log(a_Player, Logging.ELogType.PhoneChat, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("Hang up call with number {0} ", TargetPlayer.GetPhoneInUse().number)).execute();
						}
					}

					TargetPlayer.ResetInCall();
				}

				a_Player.ResetInCall();

				HelperFunctions.Chat.SendAmeMessage(a_Player, "hangs up on their cellphone.");
			}
		}
	}

	public void PhoneCommand(CPlayer SourcePlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		if (SourcePlayer.IsInCall(out CPlayer.ECallState callState))
		{
			CPlayer TargetPlayer = SourcePlayer.GetPlayerIsInCallWith();

			if (TargetPlayer != null)
			{
				if (SourcePlayer.IsCallingHotline)
				{
					if (SourcePlayer.CallingHotline != null)
					{
						CScriptedCall hotline = SourcePlayer.CallingHotline;

						(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SourcePlayer, SourcePlayer);
						SourcePlayer.PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel.Nearby, 200, 255, 200, SourcePlayer.GetActiveLanguage(), "[Phone]", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage);
						bool isFinalQuestion = hotline.CurrentQuestion == 3;

						if (hotline.CurrentQuestion == 2 && string.IsNullOrEmpty(hotline.Question3))
						{
							isFinalQuestion = true;
						}
						else if (hotline.CurrentQuestion == 1 && string.IsNullOrEmpty(hotline.Question2))
						{
							isFinalQuestion = true;
						}

						if (isFinalQuestion) // Finish call
						{
							hotline.Responses.Insert(hotline.CurrentQuestion - 1, strMessage);
							CItemValueCellphone phone = SourcePlayer.GetPhoneInUse();
							switch (hotline.Number)
							{
								case "911":
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thank you for your call. A unit will be there shortly. 911");
									FactionSystem.Get<FactionSystem>().PDSystem.Add911Call(SourcePlayer, SourcePlayer.GetPhoneInUse(), SourcePlayer.Client.Position, hotline.Responses[2], hotline.Responses[0], hotline.Responses[1]);
									break;
								case "411":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("411 DISPATCH FROM {0}: Reason for call: {1}, Location: {2}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[1], hotline.Responses[0]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thanks for your call, we'll get in touch soon.");
									break;
								case "311":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("311 DISPATCH FROM {0}: Reason for call: {1}, Location: {2}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[1], hotline.Responses[0]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thanks for your call, we'll get in touch soon.");
									break;
								case "511":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("[RADIO] This is dispatch, we've got a call from {0}: Reason for call: {1}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[0]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thanks for your call.");
									break;
								case "9021":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("[RADIO] This is dispatch, we've got an incident report from {0}: Situation: {1}, Location: {2}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[0], hotline.Responses[1]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thanks for your call, we've dispatched a unit to your location.");
									break;
								case "2600":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("[RADIO] This is dispatch, we've got a call from {0}: Name: {1}, Request: {2}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[0], hotline.Responses[1]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thank you for calling us, we'll get back to you as soon as possible.");
									break;
								case "699":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("SMS from Sabre Ltd: Someone with number {0} and name {1} asked for help: {2}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[0], hotline.Responses[1]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thanks for your call, we'll get in touch soon.");
									break;
								case "711":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("[RADIO] This is dispatch, we've got a call from {1} with number {0} via #711: Request: {2}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[0], hotline.Responses[1]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thanks for calling us, we'll get back to you as soon as possible.");
									break;
								case "811":
									SendHotlineMessageToFaction(hotline.Factions, Helpers.FormatString("[RADIO] This is dispatch, we've got a call from {1} with number {0} via #811: Request: {2}", phone != null ? phone.number.ToString() : "Unknown", hotline.Responses[0], hotline.Responses[1]));
									SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, "Thanks for calling us, we'll get back to you as soon as possible.");
									break;
							}
							EndCall(SourcePlayer);
							hotline.CurrentQuestion = 1;
						}

						if (!isFinalQuestion && hotline.CurrentQuestion == 2 && !string.IsNullOrEmpty(hotline.Question3))
						{
							hotline.Responses.Insert(1, strMessage);
							SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, hotline.Question3);
							hotline.CurrentQuestion = 3;
						}

						if (!isFinalQuestion && hotline.CurrentQuestion == 1 && !string.IsNullOrEmpty(hotline.Question2))
						{
							hotline.Responses.Insert(0, strMessage);
							SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "{0} [Phone]: {1}", hotline.Operator, hotline.Question2);
							hotline.CurrentQuestion = 2;
						}
					}
					else
					{
						EndCall(SourcePlayer);
					}
				}
				else
				{
					if (callState == CPlayer.ECallState.Connected)
					{
						// TODO_LANGUAGES: Make this look better for non looping chat commands.
						// TODO_CELLPHONE: add number
						(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SourcePlayer, SourcePlayer);
						SourcePlayer.PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel.Nearby, 200, 255, 200, SourcePlayer.GetActiveLanguage(), "[Phone]", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage);

						(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SourcePlayer, TargetPlayer);
						TargetPlayer.PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel.Nearby, 200, 255, 200, SourcePlayer.GetActiveLanguage(), "[Phone]", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage);


						List<CPlayer> lstOccupants = new List<CPlayer>();
						bool bSendToOccupantsOnly = false;

						if (SourcePlayer.IsInVehicleReal)
						{
							CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(SourcePlayer.Client.Vehicle);

							if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
							{
								bSendToOccupantsOnly = true;
								lstOccupants = VehiclePool.GetVehicleOccupants(pVehicle);
							}
						}

						List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SourcePlayer, ChatConstants.g_fDistance_Nearby);

						Logging.Log log = new Logging.Log(SourcePlayer, Logging.ELogType.PhoneChat, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("{0} [Phone]: {1}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage));

						foreach (var player in bSendToOccupantsOnly ? lstOccupants : lstPlayers)
						{
							if (player != SourcePlayer)
							{
								if (player.IsInVehicleReal && !bSendToOccupantsOnly)
								{
									CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
									if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
									{
										continue;
									}
								}

								(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SourcePlayer, player);
								player.PushChatMessageWithPlayerNameAndPostfixAndColor(EChatChannel.Nearby, 255, 255, 255, SourcePlayer.GetActiveLanguage(), SourcePlayer.GetCharacterName(ENameType.StaticCharacterName) + " [Phone]", bSendToOccupantsOnly ? "((In Car)) says" : "says", "{0}", strMessage);
								log.addAffected(player);

								if (bTargetNeedsXpAward)
								{
									LanguageSystem.AwardXP(player, SourcePlayer.GetActiveLanguage(), ChatSystem.LANGUAGE_XP);
								}
							}
						}
						log.execute();

						if (bSenderNeedsXpAward)
						{
							LanguageSystem.AwardXP(SourcePlayer, SourcePlayer.GetActiveLanguage(), ChatSystem.LANGUAGE_XP);
						}
					}
					else
					{
						// TODO_CHAT: Notification
						SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 255, 0, 0, "You are not yet connected.");
					}
				}
			}
		}
		else
		{
			// TODO_CHAT: Notification
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Nearby, 255, 0, 0, "You are not in a phone call.");
		}

		m_CachedStrMessage = string.Empty;
	}

	public void OnGetPhoneState(CPlayer a_Player, bool isVisible)
	{
		CItemValueCellphone lastUsedPhone = a_Player.GetLastUsedPhone();
		if (lastUsedPhone == null)
		{
			CItemInstanceDef item = a_Player.Inventory.GetFirstItemOfID(EItemID.CELLPHONE);
			if (item != null)
			{
				lastUsedPhone = (CItemValueCellphone)item.Value;
			}
		}

		if (lastUsedPhone == null)
		{
			a_Player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "You don't have a phone.");
			return;
		}

		if (isVisible)
		{
			NetworkEventSender.SendNetworkEvent_ClosePhoneByToggle(a_Player);
			a_Player.SetPhoneNoLongerInUse();
		}
		else
		{
			bool bHasExistingTaxiRequest = TaxiDriverJob.GetTaxiStateForPlayer(a_Player);
			NetworkEventSender.SendNetworkEvent_UseCellphone(a_Player, bHasExistingTaxiRequest, false, -1);
			a_Player.SetPhoneInUse(lastUsedPhone);
		}
	}

	public void OnCallTaxi(CPlayer a_Player)
	{
		TaxiDriverJob.OnRequestTaxi(a_Player);
	}

	public void OnRequestCancelTaxi(CPlayer a_Player)
	{
		TaxiDriverJob.OnRequestCancelTaxi(a_Player);
	}

	public void ClosePhone(CPlayer SourcePlayer)
	{
		SourcePlayer.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@code_human_wander_mobile@male@exit", "exit", false, true, false, 2500, false);
		SourcePlayer.SetPhoneNoLongerInUse();

		// TODO_CELLPHONE: End call for local and remote
	}

	public void CallNumber(CPlayer a_Player, string a_strNumber)
	{
		if (a_Player != null)
		{
			if (a_strNumber.Length == 0)
			{
				a_Player.SendNotification("Phone", ENotificationIcon.Phone, "Number cannot be empty", null);
			}
			else
			{
				HelperFunctions.Chat.SendAmeMessage(a_Player, "dials a number on their cellphone.");

				bool bIsHotline = false;
				bool bFoundNumber = false;
				CPlayer TargetPlayer = null;
				CItemValueCellphone targetCellphone = null;
				CScriptedCall calledHotline = null;

				foreach (CScriptedCall hotline in ScriptedCalls)
				{
					if (a_strNumber == hotline.Number)
					{
						bIsHotline = true;
						bFoundNumber = true;
						calledHotline = hotline;
					}
				}

				if (!bIsHotline)
				{
					if (Int64.TryParse(a_strNumber, out Int64 number) && !a_Player.IsInCall(out CPlayer.ECallState remoteCallState))
					{
						targetCellphone = new CItemValueCellphone(number);
						CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.CELLPHONE, targetCellphone);

						// Find an item with that number
						foreach (CPlayer player in PlayerPool.GetAllPlayers())
						{
							if (player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem))
							{
								TargetPlayer = player;
								bFoundNumber = true;
								break;
							}
						}
					}
				}

				if (bFoundNumber)
				{
					if (TargetPlayer == a_Player)
					{
						NetworkEventSender.SendNetworkEvent_CallFailed(a_Player, ECallFailedReason.PhoneEngaged);
						a_Player.ResetInCall();
					}
					else if (TargetPlayer != null && TargetPlayer.IsInCall(out CPlayer.ECallState callState))
					{
						NetworkEventSender.SendNetworkEvent_CallFailed(a_Player, ECallFailedReason.PhoneEngaged);
						a_Player.ResetInCall();
					}
					else
					{
						a_Player.SetCallState(CPlayer.ECallState.Connecting);
						a_Player.SetInCall(TargetPlayer);

						if (!bIsHotline)
						{
							// TODO_CELLPHONE: phone ring noise

							HelperFunctions.Chat.SendAdoMessage(TargetPlayer, "A cellphone rings");

							TargetPlayer.PushChatMessage(EChatChannel.Notifications, "You are receiving a phone call from {0:###-###-####}.", a_Player.GetLastUsedPhone().number);

							// Notify Target player
							bool bHasExistingTaxiRequest = TaxiDriverJob.GetTaxiStateForPlayer(TargetPlayer);
							NetworkEventSender.SendNetworkEvent_CallReceived(TargetPlayer, bHasExistingTaxiRequest, a_Player.GetPhoneInUse().number);
							TargetPlayer.SetPhoneInUse(targetCellphone);
							TargetPlayer.SetCallState(CPlayer.ECallState.Incoming);
							TargetPlayer.SetInCall(a_Player);
						}
						else
						{
							if (calledHotline != null)
							{
								if (!string.IsNullOrEmpty(calledHotline.Question1))
								{
									a_Player.PushChatMessageWithColor(EChatChannel.Nearby, 200, 255, 200, "Phone ((From: {0})) {1}: {2}", calledHotline.Number, calledHotline.Operator, calledHotline.Question1);
									a_Player.CallingHotline = calledHotline;
									a_Player.IsCallingHotline = true;
								}

								// Inform the calling player
								NetworkEventSender.SendNetworkEvent_CallState(a_Player, Int64.Parse(calledHotline.Number), true);
								a_Player.SetCallState(CPlayer.ECallState.Connected);
								a_Player.SetInCall(a_Player);
							}
						}
					}
				}
				else
				{
					NetworkEventSender.SendNetworkEvent_CallFailed(a_Player, ECallFailedReason.NumberNotFoundOrPlayerOffline);
					a_Player.ResetInCall();
				}
			}

			new Logging.Log(a_Player, Logging.ELogType.PhoneChat, null, Helpers.FormatString("Started call to number {0} ", a_strNumber)).execute();
		}
	}

	public void AnswerCall(CPlayer playerBeingCalled)
	{
		if (playerBeingCalled != null)
		{
			CPlayer callingPlayer = playerBeingCalled.GetPlayerIsInCallWith();

			if (callingPlayer != null)
			{
				NetworkEventSender.SendNetworkEvent_CallState(playerBeingCalled, callingPlayer.GetPhoneInUse().number, true);

				playerBeingCalled.SetCallState(CPlayer.ECallState.Connected);
				playerBeingCalled.SetInCall(callingPlayer);

				// Inform the calling player
				NetworkEventSender.SendNetworkEvent_CallState(callingPlayer, callingPlayer.GetPhoneInUse().number, true);
				callingPlayer.SetCallState(CPlayer.ECallState.Connected);
				callingPlayer.SetInCall(playerBeingCalled);
			}
		}
	}

	public void CancelCall(CPlayer playerBeingCalled)
	{
		if (playerBeingCalled != null)
		{
			CPlayer callingPlayer = playerBeingCalled.GetPlayerIsInCallWith();

			if (callingPlayer != null)
			{
				NetworkEventSender.SendNetworkEvent_CallFailed(callingPlayer, ECallFailedReason.OtherPersonCancelled);
				callingPlayer.ResetInCall();

				playerBeingCalled.ResetInCall();
			}
		}
	}

	public void SavePhoneContact(CPlayer phoneUser, string entryNumber, string entryName)
	{
		if (entryName != null && entryNumber != null)
		{
			Database.Functions.Phones.SavePhoneContact(phoneUser.GetPhoneInUse().number.ToString(), entryNumber, entryName);
		}
	}

	public void RemovePhoneContact(CPlayer phoneUser, string entryNumber, string entryName)
	{
		if (phoneUser != null && entryName != null && entryNumber != null)
		{
			Database.Functions.Phones.RemovePhoneContact(phoneUser.GetPhoneInUse().number.ToString(), entryNumber, entryName);
		}
	}

	public void GetPhoneContacts(CPlayer phoneUser)
	{
		if (phoneUser != null)
		{
			Database.Functions.Phones.GetPhoneContacts(phoneUser.GetPhoneInUse().number.ToString(), (List<KeyValuePair<string, string>> contactList) =>
			{
				NetworkEventSender.SendNetworkEvent_GotPhoneContacts(phoneUser, contactList);
			});
		}
	}

	public void CreatePhoneMessage(CPlayer fromPhone, string toNumber, string content)
	{
		if (fromPhone != null && toNumber != null && content != null)
		{
			Database.Functions.Phones.CreatePhoneMessage(fromPhone.GetPhoneInUse().number.ToString(), toNumber, content);

			new Logging.Log(fromPhone, Logging.ELogType.SMS, null, Helpers.FormatString("SMS To: {0} - Content: {1}", toNumber, content)).execute();
		}
	}

	public void GetPhoneMessagesContacts(CPlayer phoneUser)
	{
		if (phoneUser != null)
		{
			Database.Functions.Phones.GetPhoneMessagesContacts(phoneUser.GetPhoneInUse().number.ToString(), (List<CPhoneMessageContact> messagesContactList) =>
			{
				NetworkEventSender.SendNetworkEvent_GotPhoneMessagesContacts(phoneUser, messagesContactList);
			});
		}
	}

	public void GetTotalUnviewedMessages(CPlayer phoneUser)
	{
		if (phoneUser.GetPhoneInUse() != null)
		{
			Database.Functions.Phones.GetTotalUnviewedMessages(phoneUser.GetPhoneInUse().number.ToString(), (int unreadMessages) =>
			{
				NetworkEventSender.SendNetworkEvent_GotTotalUnviewedMessages(phoneUser, unreadMessages);
			});
		}
	}

	public void GetPhoneMessagesFromNumber(CPlayer phoneUser, string toNumber)
	{
		if (phoneUser != null && toNumber != null)
		{
			Database.Functions.Phones.GetPhoneMessagesFromNumber(phoneUser.GetPhoneInUse().number.ToString(), toNumber, (List<CPhoneMessage> messagesList) =>
			{
				NetworkEventSender.SendNetworkEvent_GotPhoneMessagesFromNumber(phoneUser, messagesList);
			});
		}
	}

	public void SendSMSNotification(CPlayer a_Player, string a_strNumber)
	{
		if (a_Player != null)
		{
			if (a_strNumber.Length == 0)
			{
				a_Player.SendNotification("Phone", ENotificationIcon.Phone, "Number cannot be empty", null);
			}
			else
			{
				bool bFoundNumber = false;
				CPlayer TargetPlayer = null;
				CItemValueCellphone targetCellphone = null;

				if (Int64.TryParse(a_strNumber, out Int64 number))
				{
					targetCellphone = new CItemValueCellphone(number);
					CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.CELLPHONE, targetCellphone);

					// Find an item with that number
					foreach (CPlayer player in PlayerPool.GetAllPlayers())
					{
						if (player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem))
						{
							TargetPlayer = player;
							bFoundNumber = true;
							break;
						}
					}
				}

				if (bFoundNumber && TargetPlayer != a_Player)
				{
					HelperFunctions.Chat.SendAdoMessage(TargetPlayer, "A cellphone chimes.");
					TargetPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 150, 200, 255, "Your cellphone has received a new text message.");
				}
			}
		}
	}

	public void UpdateMessageViewed(CPlayer phoneUser, string toNumber)
	{
		if (phoneUser != null && toNumber != null)
		{
			Database.Functions.Phones.UpdateMessageViewed(phoneUser.GetPhoneInUse().number.ToString(), toNumber);
		}
	}

	public void GetPhoneContactByNumber(CPlayer phoneUser, string callingNumber)
	{
		if (phoneUser.GetPhoneInUse() != null)
		{
			Database.Functions.Phones.GetPhoneContactByNumber(phoneUser.GetPhoneInUse().number.ToString(), callingNumber, (string contactName) =>
			{
				NetworkEventSender.SendNetworkEvent_GotPhoneContactByNumber(phoneUser, contactName);
			});
		}
	}

	private void SendHotlineMessageToFaction(List<long> factions, string message)
	{
		foreach (long faction in factions)
		{
			CFaction foundFaction = FactionPool.GetFactionFromID(faction);
			if (foundFaction != null)
			{
				List<CPlayer> lstFactionMembers = new List<CPlayer>();

				foreach (CPlayer factionMember in foundFaction.GetMembers())
				{
					if (!lstFactionMembers.Contains(factionMember))
					{
						lstFactionMembers.Add(factionMember);
					}
				}

				foreach (CPlayer factionMember in lstFactionMembers)
				{
					factionMember.PushChatMessageWithColor(EChatChannel.Factions, 255, 140, 105, message);
				}
			}
		}
	}

	public static List<CItemInstanceDef> GetPlayerPhones(CPlayer player)
	{
		return player.Inventory.GetAllItems().Where(i => i.ItemID == EItemID.CELLPHONE).ToList();
	}
}
