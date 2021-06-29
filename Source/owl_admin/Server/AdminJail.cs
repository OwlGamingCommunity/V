using System;

public class AdminJail
{
	public AdminJail()
	{
		CommandManager.RegisterCommand("jail", "Puts a player into admin jail",
			new Action<CPlayer, CVehicle, CPlayer, int, string>(JailPlayer),
			CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("unjail", "Removes a player from admin jail",
			new Action<CPlayer, CVehicle, CPlayer>(UnjailPlayer), CommandParsingFlags.TargetPlayer,
			CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("ojail", "Puts a player into admin jail",
			new Action<CPlayer, CVehicle, string, int, string>(OfflineJailPlayer),
			CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("ounjail", "Removes a player from admin jail",
			new Action<CPlayer, CVehicle, string>(OfflineUnjailPlayer), CommandParsingFlags.Default,
			CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("timeleft", "How much time is remaining in admin jail.",
			new Action<CPlayer, CVehicle>(TimeLeft), CommandParsingFlags.Default,
			CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("jailed", "Shows admins what players are currently jailed.",
			new Action<CPlayer, CVehicle>(Jailed), CommandParsingFlags.Default,
			CommandRequirementsFlags.MustBeAdminIgnoreDuty);
	}


	private static void JailPlayer(CPlayer player, CVehicle _, CPlayer target, int minutes, string reason)
	{
		if (minutes > 60)
		{
			player.SendNotification("Admin Jail Failed", ENotificationIcon.ExclamationSign,
				"Jail time must be 60 minutes or less.");
			return;
		}

		target.SetPlayerInAdminJail(minutes, reason);

		player.SendNotification("Admin Jailed", ENotificationIcon.Star, "You have placed {0} in admin jail.",
			target.GetCharacterName(ENameType.StaticCharacterName));

		HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} has jailed {1} for {2} minutes: {3}",
			player.Username, target.GetCharacterName(ENameType.StaticCharacterName), minutes, reason));

		Database.LegacyFunctions.AddPlayerAdminHistoryEntry(target.AccountID, "Admin Jail (" + minutes + " minutes): " + reason, player.AccountID,
			minutes, EAdminHistoryType.JAIL);

		target.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0,
			"You have been placed in admin jail for {0} minutes by {1} {2}",
			minutes, player.AdminTitle, player.Username);
		target.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0,
			"Reason: {0}", reason);
	}

	private static void UnjailPlayer(CPlayer player, CVehicle _, CPlayer target)
	{
		if (!target.IsInAdminJail())
		{
			player.SendNotification("Admin Jail Failed", ENotificationIcon.ExclamationSign,
				"That player is not in admin jail.");
			return;
		}

		target.RemoveFromAdminJail();

		player.SendNotification("Admin Unjail", ENotificationIcon.Star, "You have removed {0} from admin jail.",
			target.GetCharacterName(ENameType.StaticCharacterName));

		HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} has unjailed {1}.",
			player.Username, target.GetCharacterName(ENameType.StaticCharacterName)));

		Database.LegacyFunctions.AddPlayerAdminHistoryEntry(target.AccountID, "UNJAIL", player.AccountID,
			0, EAdminHistoryType.UNJAIL);

		target.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0,
			"You have been removed from admin jail by {0} {1}", player.AdminTitle, player.Username);
	}

	private static void OfflineJailPlayer(CPlayer player, CVehicle _, string targetAccountName, int minutes,
		string reason)
	{
		if (minutes > 60)
		{
			player.SendNotification("Admin Jail Failed", ENotificationIcon.ExclamationSign,
				"Jail time must be 60 minutes or less.");
			return;
		}

		Database.Functions.Accounts.GetJailInformationFromAccountName(targetAccountName, (accountID, _, __) =>
		{
			if (accountID == -1)
			{
				player.SendNotification("Admin Jail Failed", ENotificationIcon.ExclamationSign,
					"Could not find such account.");
				return;
			}

			Database.LegacyFunctions.AddPlayerAdminHistoryEntry(accountID, "Admin Jail (" + minutes + " minutes): " + reason, player.AccountID,
				minutes, EAdminHistoryType.JAIL);

			Database.Functions.Accounts.SetAdminJailInformation(accountID, minutes, reason, () =>
			{
				player.SendNotification("Admin Jailed", ENotificationIcon.Star, "You have placed {0} in admin jail.",
					targetAccountName);
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} has jailed {1} for {2} minutes: {3}",
					player.Username, targetAccountName, minutes, reason));
			});
		});
	}

	private static void OfflineUnjailPlayer(CPlayer player, CVehicle _, string targetAccountName)
	{
		Database.Functions.Accounts.GetJailInformationFromAccountName(targetAccountName,
			(accountID, minutes, reason) =>
			{
				if (accountID == -1)
				{
					player.SendNotification("Admin Jail Failed", ENotificationIcon.ExclamationSign,
						"Could not find such character.");
					return;
				}

				if (minutes == -1)
				{
					player.SendNotification("Admin Jail Failed", ENotificationIcon.ExclamationSign,
						"That player is not in admin jail.");
				}

				Database.LegacyFunctions.AddPlayerAdminHistoryEntry(accountID, "UNJAIL", player.AccountID,
					0, EAdminHistoryType.UNJAIL);

				Database.Functions.Accounts.SetAdminJailInformation(accountID, -1, "",
					() =>
					{
						player.SendNotification("Admin Jail Removed", ENotificationIcon.Star,
							"You have removed {0} from admin jail!", targetAccountName);
					});
			});
	}

	private static void TimeLeft(CPlayer player, CVehicle _)
	{
		if (!player.IsInAdminJail())
		{
			player.SendNotification("Admin Jail", ENotificationIcon.Star, "You are not in admin jail.");
			return;
		}

		player.SendNotification("Admin Jail", ENotificationIcon.Star,
			"You have {0} minutes remaining in admin jail. You are jailed for {1}", player.AdminJailMinutesLeft,
			player.AdminJailReason);
	}

	private static void Jailed(CPlayer player, CVehicle _)
	{
		player.PushChatMessage(EChatChannel.AdminActions, "Players in admin jail:");
		foreach (var other in PlayerPool.GetAllPlayers())
		{
			if (other.IsInAdminJail())
			{
				player.PushChatMessage(EChatChannel.AdminActions, "{0} ({1}m): {2}",
					other.GetCharacterName(ENameType.StaticCharacterName), other.AdminJailMinutesLeft,
					other.AdminJailReason);
			}
		}
	}
}