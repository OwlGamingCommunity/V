using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class LocksmithSystem 
{
	private const string LOCKSMITH_NAME = "Jeremy Rutherford";
	private const int JOB_TIME_MS = 600000;
	private const float LOCKSMITH_PRICE = 24.99f;

	public LocksmithSystem()
	{
		NetworkEvents.LocksmithRequestDuplication += OnPlayerRequestKeyDuplication;
		NetworkEvents.LocksmithOnPickupKeys += OnPlayerPickupKeys;
		NetworkEvents.OnPlayerDisconnected += AttemptDictCleanUp;

		//4 minute timer and send message if ready
		MainThreadTimerPool.CreateEntityTimer(CheckForKeyReady, 240000, this);
	}

	private void CheckForKeyReady(object[] parameters)
	{
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();

		foreach (var character in g_dictLocksmithPendingPickup)
		{
			foreach (var pendingKey in character.Value)
			{
				//compare times and see if indeed 10 minutes have passed
				if ((unixTimestamp - pendingKey.RequestedTime) <= JOB_TIME_MS)
				{
					// if this returns null that means the player is not online anymore.
					if (character.Key != null && !character.Key.IsLocksmithMessageSent)
					{
						HelperFunctions.Chat.SendAdoMessage(character.Key, "A cellphone chimes");

						bool bPlural = character.Value.Count > 1;
						character.Key.PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel.Nearby, 200, 255, 200, ECharacterLanguage.English, "[SMS]", LOCKSMITH_NAME, Helpers.FormatString("Hey {0}, your {1} ready! Please come pick {2} up as soon as possible! Thanks, \n Rutherford Locksmiths", character.Key.GetCharacterName(ENameType.StaticCharacterName), Helpers.FormatString("{0}", bPlural ? "keys are" : "key is"), Helpers.FormatString("{0}", bPlural ? "them" : "it")));

						character.Key.SetLocksmithPendingPickup();
					}
				}
			}
		}
	}

	private void OnPlayerRequestKeyDuplication(CPlayer a_requestingPlayer, string strKeyType, EntityDatabaseID keyID)
	{
		EItemID keyType = (EItemID)Enum.Parse(typeof(EItemID), strKeyType);

		if (keyType != EItemID.VEHICLE_KEY && keyType != EItemID.PROPERTY_KEY)
		{
			a_requestingPlayer.SendNotification("Locksmith", ENotificationIcon.ExclamationSign, "Something went wrong. Please bug report on the bug tracker.");
			return;
		}

		// We need to do different look ups based on key type.
		if (keyType == EItemID.VEHICLE_KEY)
		{
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, keyID);
			if (!a_requestingPlayer.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem))
			{
				a_requestingPlayer.SendNotification("Locksmith", ENotificationIcon.ExclamationSign, "You don't have this key so we can't make a copy");
				return;
			}
		}
		else if (keyType == EItemID.PROPERTY_KEY)
		{
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, keyID);
			if (!a_requestingPlayer.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem))
			{
				a_requestingPlayer.SendNotification("Locksmith", ENotificationIcon.ExclamationSign, "You don't have this key so we can't make a copy");
				return;
			}
		}

		if (!g_dictLocksmithPendingPickup.ContainsKey(a_requestingPlayer))
		{
			List<LockSmithKeyInfo> lstTemp = new List<LockSmithKeyInfo>()
			{
				{ new LockSmithKeyInfo(keyID, keyType, Helpers.GetUnixTimestamp()) }
			};

			g_dictLocksmithPendingPickup.Add(a_requestingPlayer, lstTemp);
		}
		else
		{
			g_dictLocksmithPendingPickup[a_requestingPlayer].Add(new LockSmithKeyInfo(keyID, keyType, Helpers.GetUnixTimestamp()));
		}

		HelperFunctions.Chat.SendAmeMessage(a_requestingPlayer, "slips out some notes from their wallet as he hands them to Jeremy");
		a_requestingPlayer.SubtractMoney(LOCKSMITH_PRICE, PlayerMoneyModificationReason.Locksmith);

		a_requestingPlayer.SendNotification("Locksmith", ENotificationIcon.ExclamationSign, "You paid ${0} to Rutherford Locksmiths", LOCKSMITH_PRICE);


		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(a_requestingPlayer, 10.0f);

		foreach (CPlayer player in lstPlayers)
		{
			player.PushChatMessageWithPlayerName(EChatChannel.Nearby, ECharacterLanguage.English, LOCKSMITH_NAME, Helpers.FormatString("Alright {0}, I will send you a text once you can come pick 'em up.", a_requestingPlayer.GetCharacterName(ENameType.StaticCharacterName)));
		}

	}

	private void OnPlayerPickupKeys(CPlayer a_requestingPlayer)
	{
		foreach (var pendingKey in g_dictLocksmithPendingPickup[a_requestingPlayer])
		{
			CItemInstanceDef itemDef = CItemInstanceDef.FromDefaultValueWithStackSize(pendingKey.KeyType, pendingKey.KeyID, 1);
			a_requestingPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
			{
				string strKeyType = (pendingKey.KeyType == EItemID.VEHICLE_KEY) ? "vehicle" : "property";

				a_requestingPlayer.PushChatMessageWithColor(EChatChannel.Nearby, 255, 255, 255, Helpers.FormatString("You {0} a {1} key with ID: {2}", bItemGranted ? Helpers.ColorString(0, 255, 0, "received") : Helpers.ColorString(255, 0, 0, "couldn't receive"), strKeyType, pendingKey.KeyID));
			});
		}

		HelperFunctions.Chat.SendAmeMessage(a_requestingPlayer, Helpers.FormatString("extends their arm as they grab the bag from {0}", LOCKSMITH_NAME));

		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(a_requestingPlayer, 10.0f);

		foreach (CPlayer player in lstPlayers)
		{
			player.PushChatMessageWithPlayerName(EChatChannel.Nearby, ECharacterLanguage.English, LOCKSMITH_NAME, Helpers.FormatString("Thanks for choosing Rutherford Locksmiths, hope to see you again soon!"));
		}

		// reset
		g_dictLocksmithPendingPickup.Remove(a_requestingPlayer);
		a_requestingPlayer.SetLocksmithPendingPickup(false);
	}

	private void AttemptDictCleanUp(CPlayer player, DisconnectionType type, string reason)
	{
		if (player != null)
		{
			if (g_dictLocksmithPendingPickup.ContainsKey(player))
			{
				g_dictLocksmithPendingPickup.Remove(player);
			}
		}
	}

	private Dictionary<CPlayer, List<LockSmithKeyInfo>> g_dictLocksmithPendingPickup = new Dictionary<CPlayer, List<LockSmithKeyInfo>>();
}

public class LockSmithKeyInfo
{
	public LockSmithKeyInfo(EntityDatabaseID a_keyID, EItemID a_keyType, Int64 a_requestedTime)
	{
		KeyID = a_keyID;
		KeyType = a_keyType;
		RequestedTime = a_requestedTime;
	}

	public EntityDatabaseID KeyID { get; set; }
	public EItemID KeyType { get; set; }
	public Int64 RequestedTime { get; set; }
}