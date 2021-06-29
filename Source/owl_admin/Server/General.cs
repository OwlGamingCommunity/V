using GTANetworkAPI;
using Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

namespace PlayerAdminCommands
{
	public class General
	{
		public General()
		{
			// DEBUG CMD
			CommandManager.RegisterCommand("ilikespam", "Toggles debug spam", new Action<CPlayer, CVehicle>(ToggleDebugSpam), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

			// COMMANDS
			CommandManager.RegisterCommand("admins", "Shows a list of online admins", new Action<CPlayer, CVehicle>(AdminList), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "staff" });
			CommandManager.RegisterCommand("aduty", "Toggle your admin duty status", new Action<CPlayer, CVehicle>(AdminDuty), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("makeadmin", "Modifies a players admin level", new Action<CPlayer, CVehicle, CPlayer, EAdminLevel>(MakeAdmin), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("makescripter", "Modifies a players scripter level", new Action<CPlayer, CVehicle, CPlayer, EScripterLevel>(MakeScripter), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("omakeadmin", "Modifies an offline players admin level", new Action<CPlayer, CVehicle, string, EAdminLevel>(OfflineMakeAdmin), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("goto", "Teleport to a player", new Action<CPlayer, CVehicle, CPlayer>(GotoPlayer), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty, aliases: new string[] { "tp" });
			CommandManager.RegisterCommand("auncuff", "Admin Uncuff a player", new Action<CPlayer, CVehicle, CPlayer>(AdminUncuffPlayer), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("ogethere", "Teleport an offline player to you", new Action<CPlayer, CVehicle, string>(OfflineGetPlayer), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("gethere", "Teleport a player to you", new Action<CPlayer, CVehicle, CPlayer>(GetPlayer), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("x", "Adds/subtracts from your x coord", new Action<CPlayer, CVehicle, float>(XCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty, aliases: new string[] { "setx" });
			CommandManager.RegisterCommand("y", "Adds/subtracts from your y coord", new Action<CPlayer, CVehicle, float>(YCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty, aliases: new string[] { "sety" });
			CommandManager.RegisterCommand("z", "Adds/subtracts from your z coord", new Action<CPlayer, CVehicle, float>(ZCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty, aliases: new string[] { "setz" });
			CommandManager.RegisterCommand("xyz", "Teleport yourself to an XYZ coordinate", new Action<CPlayer, CVehicle, float, float, float>(TeleportToXYZ), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty, aliases: new string[] { "setxyz" });
			CommandManager.RegisterCommand("dim", "Change your dimension", new Action<CPlayer, CVehicle, uint>(SetDimension), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty, aliases: new string[] { "dimension", "setdim" });
			CommandManager.RegisterCommand("disappear", "Makes you appear/disappear", new Action<CPlayer, CVehicle>(Disappear), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("sendto", "Teleport a player to another", new Action<CPlayer, CVehicle, CPlayer, CPlayer>(SendPlayerTo), CommandParsingFlags.TwoTargetPlayers, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("setw", "Changes the weather", new Action<CPlayer, CVehicle, int>(SetWeather), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("resetw", "Resets the weather to realtime LA", new Action<CPlayer, CVehicle>(ResetWeatherCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("adminlounge", "Goes to the admin lounge", new Action<CPlayer, CVehicle>(GotoAdminLounge), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("gotoplace", "Goes to a saved location", new Action<CPlayer, CVehicle, string>(GotoPlace), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("places", "Show a list of saved locations", new Action<CPlayer, CVehicle>(Places), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("addplace", "Saves the current location", new Action<CPlayer, CVehicle, string>(AddPlace), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("delplace", "Deletes a saved location", new Action<CPlayer, CVehicle, string>(DeletePlace), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("sethp", "Sets target players health", new Action<CPlayer, CVehicle, CPlayer, int>(SetHealth), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setarmor", "Sets target players armor", new Action<CPlayer, CVehicle, CPlayer, int>(SetArmor), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("settime", "Set the world hour", new Action<CPlayer, CVehicle, int>(SetTime), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("resettime", "Reset the world time to the server time", new Action<CPlayer, CVehicle>(ResetTime), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setskin", "Sets the target player's skin", new Action<CPlayer, CVehicle, CPlayer, string>(SetPlayerSkin), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setage", "Sets the player's age", new Action<CPlayer, CVehicle, CPlayer, int>(SetPlayerAge), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("check", "Runs a check on an online player", new Action<CPlayer, CVehicle, CPlayer>(Check), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("recon", "Recons an online player", new Action<CPlayer, CVehicle, CPlayer>(Recon), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("stoprecon", "Stops reconning a player", new Action<CPlayer, CVehicle>(StopRecon), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "srecon", "reconoff" });
			CommandManager.RegisterCommand("setname", "Changes the name for a player", new Action<CPlayer, CVehicle, CPlayer, string>(SetName), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("afreeze", "Freeze a player", new Action<CPlayer, CVehicle, CPlayer>(AdminFreeze), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("aunfreeze", "Un-freeze a player", new Action<CPlayer, CVehicle, CPlayer>(AdminUnfreeze), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("saveall", "Saves all the things", new Action<CPlayer, CVehicle>(SaveAll), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("findalts", "Finds character names and the account name", new Action<CPlayer, CVehicle, int, string>(FindAlts), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("kick", "Kicks the player specified", new Action<CPlayer, CVehicle, CPlayer, string>(KickPlayer), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("trainstations", "Lists all train stations", new Action<CPlayer, CVehicle>(ListTrainStations), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("gotostation", "Teleports to a train station", new Action<CPlayer, CVehicle, int>(GotoTrainStation), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);

			CommandManager.RegisterCommand("addmoney", "Adds money to a player", new Action<CPlayer, CVehicle, CPlayer, float>(AddMoney), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "givemoney" });
			CommandManager.RegisterCommand("takemoney", "Removes money from a player", new Action<CPlayer, CVehicle, CPlayer, float>(TakeMoney), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setmoney", "Sets a players money", new Action<CPlayer, CVehicle, CPlayer, float>(SetMoney), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("addbankmoney", "Adds money to a player", new Action<CPlayer, CVehicle, CPlayer, float>(AddBankMoney), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("takebankmoney", "Removes money from a player", new Action<CPlayer, CVehicle, CPlayer, float>(TakeBankMoney), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setbankmoney", "Sets a players money", new Action<CPlayer, CVehicle, CPlayer, float>(SetBankMoney), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

			// punishments
			// CommandManager.RegisterCommand("punish", "Punishes an online player", new Action<CPlayer, CVehicle, CPlayer, int, bool, string>(PunishPlayer), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.MustBeAdminOnDuty);
			// CommandManager.RegisterCommand("opunish", "Punishes an offline player", new Action<CPlayer, CVehicle, string, int, bool, string>(PunishPlayerOffline), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("ban", "Bans an online player", new Action<CPlayer, CVehicle, CPlayer, int, string>(BanPlayer), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("oban", "Bans an offline player", new Action<CPlayer, CVehicle, string, int, string>(BanPlayerOffline), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("unban", "Unbans a player", new Action<CPlayer, CVehicle, string, string, string>(UnbanPlayer), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);

			// TODO_POST_LAUNCH: We probably want a way to let people have multiple guns?
			CommandManager.RegisterCommand("makegun", "Gives a gun to the target player", new Action<CPlayer, CVehicle, CPlayer, EItemID, bool>(MakeGun), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("makeammo", "Gives ammo to the target player", new Action<CPlayer, CVehicle, CPlayer, EItemID, uint>(MakeAmmo), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("makedrugs", "Gives drugs to the target player", new Action<CPlayer, CVehicle, CPlayer, EItemID, uint>(MakeDrugs), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("addpet", "Gives an unnamed pet to the target player", new Action<CPlayer, CVehicle, CPlayer, EPetType>(AddPet), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("addgc", "Adds GC to an account", new Action<CPlayer, CVehicle, CPlayer, int>(AddGC), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "givegc" });
			CommandManager.RegisterCommand("takegc", "Removes GC from a account", new Action<CPlayer, CVehicle, CPlayer, int>(TakeGC), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setgc", "Sets a accounts GC", new Action<CPlayer, CVehicle, CPlayer, int>(SetGC), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("addmd", "Create a new metal detector", new Action<CPlayer, CVehicle>(CreateMetalDetector), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("delmd", "Deletes an metal detector", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteMetalDetector), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("revive", "Revives the target player", new Action<CPlayer, CVehicle, CPlayer>(Revive), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("forcerevive", "Force revives the target player, even if they are not dead", new Action<CPlayer, CVehicle, CPlayer>(ForceRevive), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("givelicense", "Gives a license to the target player", new Action<CPlayer, CVehicle, CPlayer, EDrivingTestType>(GiveLicense), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("givegunlicense", "Gives a license to the target player", new Action<CPlayer, CVehicle, CPlayer, int>(GiveGunLicense), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);

			CommandManager.RegisterCommand("forceapp", "Sends a player back to the application stage.", new Action<CPlayer, CVehicle, CPlayer>(ForceAppPlayer), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty, aliases: new string[] { "fa" });

			CommandManager.RegisterCommand("checkveh", "Runs a check on a vehicle", new Action<CPlayer, CVehicle, EntityDatabaseID>(CheckVeh), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("checkint", "Runs a check on an interior", new Action<CPlayer, CVehicle, EntityDatabaseID>(CheckInt), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("showram", "Shows RAM usage", new Action<CPlayer, CVehicle>(ShowRAM), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeUaOrScripter);
			CommandManager.RegisterCommand("showsql", "Shows SQL stats", new Action<CPlayer, CVehicle>(ShowSQL), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeUaOrScripter);
			CommandManager.RegisterCommand("getnativeinteriorid", "Outputs the current native interior ID you are in", new Action<CPlayer, CVehicle>(NativeInteriorIdCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeScripter);
			// FOURTH OF JULY
			CommandManager.RegisterCommand("4th", "Start 4th of july", new Action<CPlayer, CVehicle>(Start4Th), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("blockcommand", "Blocks a command to prevent server crashes", new Action<CPlayer, CVehicle, string>(BlockCmdCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("unblockcommand", "Unblocks a command", new Action<CPlayer, CVehicle, string>(UnblockCmdCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminOnDuty);

			NetworkEvents.EndFourthOfJulyEvent += EndFourthOfJulyEvent;

			NetworkEvents.SaveAdminNotes += OnSaveAdminNotes;
			NetworkEvents.SaveAdminInteriorNote += OnSaveAdminInteriorNote;
			NetworkEvents.SaveAdminVehicleNote += OnSaveAdminVehicleNote;
			NetworkEvents.ReloadCheckVehData += OnReloadCheckVehData;
			NetworkEvents.ReloadCheckIntData += OnReloadCheckIntData;
			NetworkEvents.UpdateStolenState += OnUpdateStolenState;

			CommandManager.RegisterCommand("stopdiscordbot", "Stops discord bot", new Action<CPlayer, CVehicle>(StopDiscordBot), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("startdiscordbot", "Start discord bot", new Action<CPlayer, CVehicle>(StartDiscordBot), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("discordbotstatus", "Shows discord bot status", new Action<CPlayer, CVehicle>(DiscordBotStatus), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("bandwidth", "Shows bandwidth savings", new Action<CPlayer, CVehicle>(BandwidthSavings), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("showinv", "Shows a player inventory", new Action<CPlayer, CVehicle, CPlayer>(ShowInventoryCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		}

		private void BandwidthSavings(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			if (SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				ServerPerfManager.GetTotalBytesSentData(out UInt64 sent, out UInt64 sentCompressed, out double sizeKB, out double sizeCompressedKB, out double sizeMB, out double sizeCompressedMB, out string strDisplayString);
				SenderPlayer.PushChatMessage(EChatChannel.AdminActions, strDisplayString);

				ServerPerfManager.GetTotalBytesReceivedData(out UInt64 recv, out UInt64 recvCompressed, out double recvsizeKB, out double recvsizeCompressedKB, out double recvsizeMB, out double recvsizeCompressedMB, out string strRecvDisplayString);
				SenderPlayer.PushChatMessage(EChatChannel.AdminActions, strRecvDisplayString);
			}
		}

		private void StartDiscordBot(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			if (SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				if (!DiscordBotIntegration.IsBotRunning())
				{
					DiscordBotIntegration.StartBotProcess();
				}
				else
				{
					SenderPlayer.SendNotification("Admin", ENotificationIcon.InfoSign, "Discord bot is already running");
				}
			}
		}

		private void StopDiscordBot(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			if (SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				if (DiscordBotIntegration.IsBotRunning())
				{
					DiscordBotIntegration.KillBotProcess();
				}
				else
				{
					SenderPlayer.SendNotification("Admin", ENotificationIcon.InfoSign, "Discord bot is not currently running");
				}
			}
		}

		private void DiscordBotStatus(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			if (SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				SenderPlayer.SendNotification("Admin", ENotificationIcon.InfoSign, "Discord bot is {0}", DiscordBotIntegration.IsBotRunning() ? "running" : "stopped");
			}
		}

		private void PunishPlayer(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int numPoints, bool repeatOffence, string Reason)
		{
			TargetPlayer.AddPunishmentPoints(SenderPlayer, numPoints, repeatOffence, Reason);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) gave you {3} punishment points for '{4}'.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, numPoints, Reason);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You gave {1} punishment points to '{0}' ({3}) for '{2}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), numPoints, Reason, TargetPlayer.Username);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) gave {4} punishment points to '{3}' ({6}) for {5}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), numPoints, Reason, TargetPlayer.Username), r: 255, g: 0, b: 0);

			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/punish {0} - issued {1} points", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), numPoints)).execute();
		}

		private async void PunishPlayerOffline(CPlayer SenderPlayer, CVehicle SenderVehicle, string TargetUsername, int numPoints, bool repeatOffence, string Reason)
		{
			BasicAccountInfo accountInfo = await Database.LegacyFunctions.GetBasicAccountInfoFromExactUsername(TargetUsername).ConfigureAwait(true);

			if (accountInfo.Result == BasicAccountInfo.EGetAccountInfoResult.NotFound)
			{
				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "Account '{0}' was not found.", TargetUsername);
			}
			else
			{
				await Database.LegacyFunctions.AddPunishmentPoints(accountInfo.AccountID, numPoints, SenderPlayer.m_DatabaseID, Reason).ConfigureAwait(true);

				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You gave {1} punishment points to '{0}' for '{2}'.", accountInfo.Username, numPoints, Reason);
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) gave {4} punishment points to '{3}' for {5}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, accountInfo.Username, numPoints, Reason), r: 255, g: 0, b: 0);
				new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/opunish {0} - issued {1} points", TargetUsername, numPoints)).execute();
			}
		}

		private void BanPlayer(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int numHoursOrZeroForPermanent, string Reason)
		{
			TargetPlayer.Ban(numHoursOrZeroForPermanent, Reason, SenderPlayer);

			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You banned '{0}' for '{1}' {2}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Reason, numHoursOrZeroForPermanent > 0 ? Helpers.FormatString("for {0} hour(s)", numHoursOrZeroForPermanent) : "Permanently");
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) banned '{3}' for '{4}' {5}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Reason, numHoursOrZeroForPermanent > 0 ? Helpers.FormatString("for {0} hour(s)", numHoursOrZeroForPermanent) : "Permanently"), r: 255, g: 0, b: 0);

			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/ban {0} - hours {1} - Reason: {2}", TargetPlayer.Username, numHoursOrZeroForPermanent, Reason)).execute();
		}

		private async void BanPlayerOffline(CPlayer SenderPlayer, CVehicle SenderVehicle, string TargetUsername, int numHoursOrZeroForPermanent, string Reason)
		{
			BasicAccountInfo accountInfo = await Database.LegacyFunctions.GetBasicAccountInfoFromExactUsername(TargetUsername).ConfigureAwait(true);

			if (accountInfo.Result == BasicAccountInfo.EGetAccountInfoResult.NotFound)
			{
				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "Account '{0}' was not found.", TargetUsername);
			}
			else
			{
				// TODO_POST_LAUNCH: Get last logged in serial + ip and ban using that
				if (numHoursOrZeroForPermanent > 0)
				{
					await Database.LegacyFunctions.AddDurationBan(String.Empty, String.Empty, accountInfo.AccountID, SenderPlayer.AccountID, Reason, numHoursOrZeroForPermanent).ConfigureAwait(true);
				}
				else
				{
					await Database.LegacyFunctions.AddPermanentBan(String.Empty, String.Empty, accountInfo.AccountID, SenderPlayer.AccountID, Reason).ConfigureAwait(true);
				}

				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You banned '{0}' for '{1}' {2}.", accountInfo.Username, Reason, numHoursOrZeroForPermanent > 0 ? Helpers.FormatString("for {0} hour(s)", numHoursOrZeroForPermanent) : "Permanently");
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) banned '{3}' for '{4}' {5}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, accountInfo.Username, Reason, numHoursOrZeroForPermanent > 0 ? Helpers.FormatString("for {0} hour(s)", numHoursOrZeroForPermanent) : "Permanently"), r: 255, g: 0, b: 0);
				new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/oban {0} - hours {1} - Reason: {2}", TargetUsername, numHoursOrZeroForPermanent, Reason)).execute();
			}
		}

		private async void UnbanPlayer(CPlayer SenderPlayer, CVehicle SenderVehicle, string UsernameOrMinusOneToIgnore, String SerialOrMinusOneToIgnore, string IPAddressOrMinusOneToIgnore)
		{
			int bansRemoved = await Database.LegacyFunctions.RemoveBans(UsernameOrMinusOneToIgnore, SerialOrMinusOneToIgnore, IPAddressOrMinusOneToIgnore, SenderPlayer.m_DatabaseID).ConfigureAwait(true);

			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You removed {0} bans. (Request - Username: {1}, Serial: {2}, IPAddress: {3}).", bansRemoved, UsernameOrMinusOneToIgnore, SerialOrMinusOneToIgnore, IPAddressOrMinusOneToIgnore);

			if (bansRemoved > 0)
			{
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) removed {3} bans. (Request - Username: {4}, Serial: {5}, IPAddress: {6}).", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, bansRemoved, UsernameOrMinusOneToIgnore, SerialOrMinusOneToIgnore, IPAddressOrMinusOneToIgnore), r: 255, g: 0, b: 0);
			}

			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/unban {0} - Serial {1} - IP: {2}", UsernameOrMinusOneToIgnore, SerialOrMinusOneToIgnore, IPAddressOrMinusOneToIgnore)).execute();
		}

		private void AddMoney(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float fAmount)
		{
			if ((!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin) && fAmount > 5000) || (!SenderPlayer.IsAdmin(EAdminLevel.Admin) && fAmount <= 5000))
			{
				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You may only add an amount of 5000 with your current rank.");
				return;
			}

			TargetPlayer.AddMoney(fAmount, PlayerMoneyModificationReason.AddMoneyAdminCMD);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) gave you ${3:0.00} on-hand.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, fAmount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You gave ${1:0.00} on-hand to '{0}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) gave ${4:0.00} on-hand to '{3}'.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/givemoney {0} ${1} - New balance: ${2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount, TargetPlayer.Money)).execute();
		}

		private void TakeMoney(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float fAmount)
		{
			if ((!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin) && fAmount > 5000) || (!SenderPlayer.IsAdmin(EAdminLevel.Admin) && fAmount <= 5000))
			{
				return;
			}

			TargetPlayer.RemoveMoney(fAmount, PlayerMoneyModificationReason.TakeMoneyAdminCMD);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) removed ${3:0.00} from your on-hand money.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, fAmount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You removed ${1:0.00} from '{0}s' on-hand money.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) removed ${4:0.00} from '{3}s' on-hand money.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/takemoney {0} ${1} - New balance: ${2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount, TargetPlayer.Money)).execute();
		}

		private void SetMoney(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float fAmount)
		{
			if (!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				return;
			}

			TargetPlayer.SetMoney(fAmount, PlayerMoneyModificationReason.SetMoneyAdminCMD);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) set your bank balance to ${3:0.00}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, fAmount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You set '{0}s' bank balance to ${1:0.00}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) set the bank balance of '{4}' to ${3:0.00}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/setmoney - {0} ${1} - New balance: ${2} ", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount, TargetPlayer.Money)).execute();
		}

		private void AddBankMoney(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float fAmount)
		{
			if ((!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin) && fAmount > 5000) || (!SenderPlayer.IsAdmin(EAdminLevel.Admin) && fAmount <= 5000))
			{
				return;
			}

			TargetPlayer.AddBankMoney(fAmount, PlayerMoneyModificationReason.AddBankMoneyAdminCMD);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) gave you ${3:0.00} via bank balance.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, fAmount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You gave ${1:0.00} bank balance to '{0}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) gave ${4:0.00} bank balance to '{3}'.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/addbankmoney {0} ${1} - New balance: ${2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount, TargetPlayer.BankMoney)).execute();
		}

		private void TakeBankMoney(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float fAmount)
		{
			if ((!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin) && fAmount > 5000) || (!SenderPlayer.IsAdmin(EAdminLevel.Admin) && fAmount <= 5000))
			{
				return;
			}

			TargetPlayer.RemoveBankMoney(fAmount, PlayerMoneyModificationReason.TakeBankMoneyAdminCMD);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) removed ${3:0.00} from your bank balance.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, fAmount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You removed ${1:0.00} from '{0}s' bank balance.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) removed ${4:0.00} from '{3}s' bank balance.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/takebankmoney {0} ${1} - New balance: ${2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount, TargetPlayer.BankMoney)).execute();
		}

		private void SetBankMoney(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float fAmount)
		{
			if (!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				return;
			}

			TargetPlayer.SetBankMoney(fAmount, PlayerMoneyModificationReason.SetBankMoneyAdminCMD);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) set your bank balance to ${3:0.00}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, fAmount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You set '{0}s' bank balance to ${1:0.00}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) set the bank balance of '{3}' to ${4:0.00}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/setbankmoney {0} ${1} - New balance: ${2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount, TargetPlayer.BankMoney)).execute();
		}

		private void OnSaveAdminNotes(CPlayer SenderPlayer, string strNotes, int accountID)
		{
			Database.LegacyFunctions.UpdateAdminNotes(accountID, strNotes);
		}

		private void OnSaveAdminVehicleNote(CPlayer SenderPlayer, string strNote, long vehicleID)
		{
			Database.LegacyFunctions.CreateVehicleNote(vehicleID, SenderPlayer.AccountID, strNote);
		}

		private void OnSaveAdminInteriorNote(CPlayer SenderPlayer, string strNote, long interiorID)
		{
			Database.LegacyFunctions.CreatePropertyNote(interiorID, SenderPlayer.AccountID, strNote);
		}

		private async void GetCheckVehicleData(long vehicleID, Action<AdminCheckVehicleDetails> onCompletion)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				AdminCheckVehInfo adminCheckVehInfo = await Database.LegacyFunctions.GetAdminCheckVehInfo(vehicleID).ConfigureAwait(true);
				if (adminCheckVehInfo != null)
				{
					CDatabaseStructureVehicle vehicleDetails = adminCheckVehInfo.VehicleDetails;
					string vehicleFactionType = "No";
					if (vehicleDetails.vehicleType == EVehicleType.FactionOwned || vehicleDetails.vehicleType == EVehicleType.FactionOwnedRental)
					{
						vehicleFactionType = "Yes";
					}

					if (vehicleDetails.vehicleType == EVehicleType.PlayerOwned || vehicleDetails.vehicleType == EVehicleType.RentalCar)
					{
						Database.Functions.Characters.GetCharacterNameFromDBID(vehicleDetails.ownerID, (string charName) =>
						{
							GetVehicleActionsById(charName);
						});
					}
					else
					{
						GetVehicleActionsById("Unknown");
					}

					void GetVehicleActionsById(string charName)
					{
						ESFunctions.GetVehicleActionsById(vehicleID, (outVehicleActions) =>
						{
							adminCheckVehInfo.VehicleActions = outVehicleActions;

							CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicleDetails.model);

							string paymentMethod = "Paid Completely";
							if (vehicleDetails.iPaymentsRemaining > 0)
							{
								paymentMethod = "Car on credit";
							}
							else if (vehicleDetails.vehicleType == EVehicleType.RentalCar)
							{
								paymentMethod = "Rental";
							}

							AdminCheckVehicleDetails vehicleCheckDetails = new AdminCheckVehicleDetails(adminCheckVehInfo.OwnerName, charName, vehicleDef.Price, vehicleDetails.model, paymentMethod, vehicleDetails.iPaymentsMade, vehicleDetails.iPaymentsMissed, vehicleDetails.iPaymentsRemaining, vehicleDetails.fCreditAmount, vehicleFactionType, vehicleDetails.bStolen, adminCheckVehInfo.VehicleActions, adminCheckVehInfo.AdminVehicleNotes);
							onCompletion.Invoke(vehicleCheckDetails);
						});
					}
				}
				else
				{
					onCompletion.Invoke(null);
				}
			}
			else
			{
				onCompletion.Invoke(null);
			}
		}

		private async void GetCheckInteriorData(long interiorID, Action<AdminCheckInteriorDetails> onCompletion)
		{
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(interiorID);
			if (propertyInst == null)
			{
				onCompletion.Invoke(null);
				return;
			}

			AdminCheckIntInfo adminCheckIntInfo = await Database.LegacyFunctions.GetAdminCheckIntInfo(interiorID).ConfigureAwait(true);
			if (adminCheckIntInfo == null)
			{
				onCompletion.Invoke(null);
				return;
			}

			Database.Models.Property propertyDetails = adminCheckIntInfo.PropertyDetails;
			if (propertyDetails.OwnerType == EPropertyOwnerType.Player && propertyDetails.OwnerId != 0)
			{
				Database.Functions.Characters.GetCharacterNameFromDBID(propertyDetails.OwnerId, charName =>
				{
					OnDone(charName);
				});
			}
			else
			{
				OnDone("Unknown");
			}

			void OnDone(string charName)
			{
				string propertyState = propertyDetails.State switch
				{
					EPropertyState.Owned => "Owned",
					EPropertyState.Rented => "Rented",
					EPropertyState.AvailableToRent => "Up for rental",
					EPropertyState.AvailableToBuy => "Up for buying",
					_ => "Unknown"
				};

				ESFunctions.GetPropertyActionsById(interiorID, (outPropertyActions) =>
				{
					adminCheckIntInfo.PropertyActions = outPropertyActions;

					AdminCheckInteriorDetails propertyCheckDetails = new AdminCheckInteriorDetails(adminCheckIntInfo.OwnerName, charName, propertyDetails.BuyPrice.ToString(), propertyDetails.RentPrice.ToString(), propertyDetails.Locked, propertyDetails.Name, propertyDetails.InteriorId, propertyDetails.PaymentsRemaining > 0 ? "Credit" : "Cash", propertyDetails.PaymentsMade, propertyDetails.PaymentsMissed, propertyDetails.PaymentsRemaining, propertyDetails.CreditAmount, propertyState, propertyDetails.LastUsed.ToString(), adminCheckIntInfo.PropertyActions, adminCheckIntInfo.AdminPropertyNotes);
					onCompletion.Invoke(propertyCheckDetails);
				});
			}
		}

		private void OnReloadCheckIntData(CPlayer SenderPlayer, long propertyID)
		{
			GetCheckInteriorData(propertyID, (propertyCheckDetails) =>
			{
				if (propertyCheckDetails != null)
				{
					NetworkEventSender.SendNetworkEvent_AdminReloadCheckIntDetails(SenderPlayer, propertyCheckDetails);
				}
			});
		}

		private void OnReloadCheckVehData(CPlayer SenderPlayer, long vehicleID)
		{
			GetCheckVehicleData(vehicleID, (vehicleCheckDetails) =>
			{
				if (vehicleCheckDetails != null)
				{
					NetworkEventSender.SendNetworkEvent_AdminReloadCheckVehDetails(SenderPlayer, vehicleCheckDetails);
				}
			});

		}

		private async void OnUpdateStolenState(CPlayer SenderPlayer, long vehicleID, bool stolen)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				await Database.LegacyFunctions.UpdateVehicleStolenState(Vehicle.m_DatabaseID, stolen).ConfigureAwait(true);
				Log.CreateLog(SenderPlayer.AccountID, EOriginType.Account, ELogType.VehicleRelated, new List<CBaseEntity>() { Vehicle }, $"stolen: {stolen}");
			}
		}

		private async void Check(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer)
		{
			int donatorCurrency = await TargetPlayer.GetDonatorCurrency().ConfigureAwait(true);
			int numPunishmentPointsActive = await TargetPlayer.GetActivePunishmentPoints().ConfigureAwait(true);
			int numPunishmentPointsLifetime = await TargetPlayer.GetAllPunishmentPoints().ConfigureAwait(true);
			// TODO_LAUNCH: Implement warns

			List<CFactionTransmit> lstFactions = new List<CFactionTransmit>();
			foreach (CFactionMembership factionMembership in TargetPlayer.GetFactionMemberships())
			{
				// We dont truly populate CFactionTransmit, for example members etc, just the basics for display purposes
				CFactionTransmit factionToTransmit = new CFactionTransmit(factionMembership.Faction.FactionID, factionMembership.Faction.ShortName, factionMembership.Faction.Name, factionMembership.Faction.Money, factionMembership.Faction.Message, factionMembership.Manager);
				lstFactions.Add(factionToTransmit);
			}

			// Get DB details
			AdminCheckInfo adminCheckInfo = await Database.LegacyFunctions.GetAdminCheckInfo(TargetPlayer.AccountID).ConfigureAwait(true);

			AdminCheckDetails playerDetails = new AdminCheckDetails(TargetPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetPlayer.Client.Address.ToString(), donatorCurrency, lstFactions, TargetPlayer.MinutesPlayed_Account / 60, TargetPlayer.MinutesPlayed_Character / 60, numPunishmentPointsActive, numPunishmentPointsLifetime, adminCheckInfo.AdminNotes, adminCheckInfo.AdminHistory);
			NetworkEventSender.SendNetworkEvent_AdminCheck(SenderPlayer, TargetPlayer.Client, playerDetails);
		}

		private void CheckVeh(CPlayer SenderPlayer, CVehicle SenderVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				if (Vehicle.VehicleType == EVehicleType.Temporary)
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "The vehicle you've entered is a temporary vehicle.");
					return;
				}

				GetCheckVehicleData(vehicleID, (vehicleCheckDetails) =>
				{
					if (vehicleCheckDetails != null)
					{
						NetworkEventSender.SendNetworkEvent_AdminCheckVeh(SenderPlayer, Vehicle.m_DatabaseID, vehicleCheckDetails);
					}
				});
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
			}
		}

		private void CheckInt(CPlayer SenderPlayer, CVehicle SenderVehicle, EntityDatabaseID interiorID)
		{
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(interiorID);
			if (propertyInst != null)
			{
				GetCheckInteriorData(interiorID, (propertyCheckDetails) =>
				{

					if (propertyCheckDetails != null)
					{
						NetworkEventSender.SendNetworkEvent_AdminCheckInt(SenderPlayer, propertyInst.Model.Id, propertyCheckDetails);
					}
				});
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
			}
		}

		private void ShowRAM(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
			System.Diagnostics.Process botProc = DiscordBotIntegration.GetBotProcess();
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "Current Server RAM usage is: WorkingSet: {0:0.00}MB PrivateMem: {1:0.00}MB", (double)proc.WorkingSet64 / 1024 / 1024, (double)proc.PrivateMemorySize64 / 1024 / 1024);

			if (botProc != null)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "Current Discord Bot RAM usage is: WorkingSet: {0:0.00}MB PrivateMem: {1:0.00}MB", (double)botProc.WorkingSet64 / 1024 / 1024, (double)botProc.PrivateMemorySize64 / 1024 / 1024);
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "Current Discord Bot RAM usage is unavailable - bot is not running.");
			}
		}

		private void ShowSQL(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "MySQL Stats:");
			List<string> lstStats = Database.ThreadedMySQL.GetDebugStats();
			foreach (string strStat in lstStats)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, strStat);
			}
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "=================");
		}

		private void NativeInteriorIdCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			NetworkEventSender.SendNetworkEvent_AdminNativeInteriorID(SenderPlayer);
		}

		private void SetName(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, string strNewName)
		{
			Database.Functions.Characters.IsNameUnique(strNewName, async (bool bIsUnique) =>
			{
				if (bIsUnique)
				{
					await Database.LegacyFunctions.SetCharacterName(TargetPlayer.ActiveCharacterDatabaseID, strNewName).ConfigureAwait(true);
					TargetPlayer.SetCharacterNameOnSpawn(strNewName);

					TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) changed your character name to '{3}'.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, strNewName);
					SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You changed '{0}'s name to '{1}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), strNewName);
					new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/setname - Set old name {0} to {1} ", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), strNewName)).execute();
				}
				else
				{
					SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "Character name '{0}' is already taken.", strNewName);
				}
			});
		}


		private void AdminFreeze(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer)
		{
			TargetPlayer.Freeze(true);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You have frozen '{0}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "You were frozen by {0} {1} ({2}).", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/afreeze - Froze {0} ", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
		}

		private void AdminUnfreeze(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer)
		{
			TargetPlayer.Freeze(false);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You have un-frozen '{0}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "You were un-frozen by {0} {1} ({2}).", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/aunfreeze - Unfroze {0} ", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
		}

		private void Recon(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer)
		{
			if (SenderPlayer != TargetPlayer)
			{
				if (SenderPlayer.IsReconning)
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Use /stoprecon followed by /recon to recon another player");
					return;
				}
				if (TargetPlayer.IsReconning)
				{
					SenderPlayer.SendNotification("Recon", ENotificationIcon.ExclamationSign, "You cannot recon '{0}' because they are reconning someone else.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
				}
				if (SenderPlayer.Client.Dimension != 0)
				{
					//TODO Ability to use from an interior. Not able to get it stable.
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Please go outside of the interior to use /recon.");
					return;
				}
				else
				{
					if (SenderPlayer.Client.Dimension != TargetPlayer.Client.Dimension)
					{
						Dimension propertyID = TargetPlayer.Client.Dimension;
						CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
						SenderPlayer.OnEnterProperty(Property);
					}
					Vector3 cachedPos = SenderPlayer.Client.Position;

					SenderPlayer.Client.Transparency = 0;
					SenderPlayer.StartRecon(TargetPlayer);
					SenderPlayer.OnTeleport();
					SenderPlayer.SendNotification("Recon", ENotificationIcon.Font, "You are now reconning '{0}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
					HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} Started reconning {1}({2})", SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetPlayer.Username), r: 255, g: 0, b: 0);
					NetworkEventSender.SendNetworkEvent_StartRecon(SenderPlayer, TargetPlayer.Client);

					new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/recon - Started recon on {0}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
				}
			}
			else
			{
				SenderPlayer.SendNotification("Recon", ENotificationIcon.ExclamationSign, "You cannot recon yourself.");
			}
		}

		private void StopRecon(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			if (SenderPlayer.IsReconning)
			{
				SenderPlayer.SendNotification("Recon", ENotificationIcon.Font, "You are no longer reconning.");
				NetworkEventSender.SendNetworkEvent_StopRecon(SenderPlayer);

				SenderPlayer.StopRecon();
				SenderPlayer.OnTeleport();
				SenderPlayer.Client.Transparency = 255;
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} Stopped reconning", SenderPlayer.Username), r: 255, g: 0, b: 0);
				new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/stoprecon - Stopped reconning")).execute();
			}
			else
			{
				SenderPlayer.SendNotification("Recon", ENotificationIcon.ExclamationSign, "You are not reconning anyone.");
			}
		}

		private void MakeGun(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EItemID WeaponID, bool bIsLegal)
		{
			if (!WeaponHelpers.IsItemAWeapon(WeaponID))
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' is not a weapon.", WeaponID.ToString());
			}
			else
			{
				CItemInstanceDef itemDef = CItemInstanceDef.FromDefaultValueWithStackSize(WeaponID, -1, 1);
				((CItemValueBasic)itemDef.Value).is_legal = bIsLegal;

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;

				if (TargetPlayer.Inventory.CanGiveItem(itemDef, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received weapon '{1}'", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), WeaponID.ToString());
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you weapon '{2}'", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), WeaponID.ToString());
							new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/makegun - Gave {0} a weapon with ID {1} - Legal: {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), WeaponID, bIsLegal)).execute();
						}
						else
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive weapon '{1}': Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), WeaponID.ToString());
						}
					});
				}
				else
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive weapon '{1}': {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), WeaponID.ToString(), strHumanReadableError);
				}
			}
		}

		private void MakeDrugs(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EItemID DrugID, uint StackSize)
		{
			if (DrugID != EItemID.METH
				&& DrugID != EItemID.COCAINE
				&& DrugID != EItemID.HEROIN
				&& DrugID != EItemID.XANAX
				&& DrugID != EItemID.WEED)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' is not a drug.", DrugID.ToString());
			}
			else
			{
				CItemInstanceDef itemDef = CItemInstanceDef.FromDefaultValueWithStackSize(DrugID, -1, StackSize);

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;

				if (TargetPlayer.Inventory.CanGiveItem(itemDef, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received drug '{1}' with stack size {2}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), DrugID.ToString(), StackSize);
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you drug '{2}' with stack size {3}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), DrugID.ToString(), StackSize);
							new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/makedrugs - Gave {0} drugs with ID {1} - Stack size: {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), DrugID, StackSize)).execute();
						}
						else
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive drug '{1}': Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), DrugID.ToString());
						}
					});
				}
				else
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive drug '{1}': {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), DrugID.ToString(), strHumanReadableError);
				}
			}
		}

		private void AddPet(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EPetType petType)
		{
			if (SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				CItemValuePet petValue = new CItemValuePet(petType, String.Empty, false);
				CItemInstanceDef itemDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.PET, petValue);

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;

				if (TargetPlayer.Inventory.CanGiveItem(itemDef, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received pet '{1}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), petType.ToString());
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you pet '{2}' .", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), petType.ToString());
							new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/addpet - Gave {0} pet with type {1}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), petType)).execute();
						}
						else
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive pet '{1}': Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), petType.ToString());
						}
					});
				}
				else
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive pet '{1}': {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), petType.ToString(), strHumanReadableError);
				}
			}
		}

		private void MakeAmmo(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EItemID AmmoItemID, uint NumAmmo)
		{
			if (AmmoItemID < EItemID.AMMO_HANDGUN || AmmoItemID > EItemID.AMMO_FLARE)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' is not ammunition.", AmmoItemID.ToString());
			}
			else
			{
				CItemInstanceDef itemDef = CItemInstanceDef.FromDefaultValueWithStackSize(AmmoItemID, -1, NumAmmo);

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;

				if (TargetPlayer.Inventory.CanGiveItem(itemDef, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received ammo '{1}'", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), AmmoItemID.ToString());
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you weapon '{2}'", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), AmmoItemID.ToString());
							new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/makeammo - Gave {0} ammo with ID {1} - Amount: {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), AmmoItemID, NumAmmo)).execute();
						}
						else
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive ammo '{1}': Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), AmmoItemID.ToString());
						}
					});
				}
				else
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive ammo '{1}': {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), AmmoItemID.ToString(), strHumanReadableError);
				}
			}
		}

		private void ToggleDebugSpam(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			NetworkEventSender.SendNetworkEvent_ToggleDebugSpam(SenderPlayer);
		}

		private void AdminList(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			SenderPlayer.PushChatMessage(EChatChannel.Notifications, "~~~~~~~~~~~~~~ ADMINS ~~~~~~~~~~~~~~~~");
			List<CPlayer> adminsList = new List<CPlayer>();
			foreach (CPlayer player in PlayerPool.GetAllPlayers())
			{
				if (player.IsAdmin(EAdminLevel.TrialAdmin, true))
				{
					adminsList.Add(player);
				}
			}

			adminsList.Sort((a, b) => a.AdminLevel.CompareTo(b.AdminLevel));
			adminsList.Reverse();

			foreach (CPlayer player in adminsList)
			{
				SenderPlayer.PushChatMessage(EChatChannel.Notifications, "\t{0} {1} ({2})", player.AdminTitle, player.Username, player.GetCharacterName(ENameType.StaticCharacterName));
			}
			SenderPlayer.PushChatMessage(EChatChannel.Notifications, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
		}

		private void AdminDuty(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			bool bNewState = !SenderPlayer.AdminDuty;

			// If we are going on duty, set the state immediately so they get the message. Otherwise, do it later so they still get the message. Hacky.
			if (bNewState)
			{
				SenderPlayer.AdminDuty = bNewState;
			}

			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} has gone {2} duty.", SenderPlayer.AdminTitle, SenderPlayer.Username, bNewState ? "on" : "off"), true, EAdminLevel.TrialAdmin, r: 255, g: 0, b: 0);

			if (!bNewState)
			{
				SenderPlayer.AdminDuty = bNewState;
			}
		}

		private void MakeAdmin(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, EAdminLevel adminLevel)
		{
			if ((SourcePlayer.IsAdmin(EAdminLevel.LeadAdmin) && TargetPlayer.AdminLevel < SourcePlayer.AdminLevel
															&& adminLevel < SourcePlayer.AdminLevel)
															|| SourcePlayer.IsAdmin(EAdminLevel.HeadAdmin))
			{
				TargetPlayer.AdminLevel = adminLevel;
				Database.LegacyFunctions.SetAdminLevel(TargetPlayer.AccountID, TargetPlayer.AdminLevel);
				SourcePlayer.SendNotification("Admin", ENotificationIcon.Font, Helpers.FormatString("You have set {0} to level {1} ({2}).", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), adminLevel, adminLevel.ToString()));

				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/makeadmin - Promoted to level {0} ", adminLevel)).execute();
			}
		}

		private void MakeScripter(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, EScripterLevel scripterLevel)
		{
			if (SourcePlayer.IsAdmin(EAdminLevel.HeadAdmin) && (SourcePlayer.Username.ToLower() == "danielsdev" || SourcePlayer.Username.ToLower() == "chaos"))
			{
				if (!Enum.IsDefined(typeof(EScripterLevel), scripterLevel))
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Incorrect scripter level provided.");
					return;
				}

				TargetPlayer.ScripterLevel = scripterLevel;
				Database.Functions.Accounts.UpdateScripterLevel(TargetPlayer.AccountID, scripterLevel);
				SourcePlayer.SendNotification("Scripter", ENotificationIcon.Font, Helpers.FormatString("You have set {0} to level {1} ({2}).", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), scripterLevel, scripterLevel.ToString()));
				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/makescripter - Promoted to level {0} ", scripterLevel)).execute();
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "No I don't think so.");
			}
		}

		private async void OfflineMakeAdmin(CPlayer SourcePlayer, CVehicle SourceVehicle, string TargetPlayerUsername,
			EAdminLevel adminLevel)
		{
			if (SourcePlayer.IsAdmin(EAdminLevel.LeadAdmin) && adminLevel < SourcePlayer.AdminLevel
				|| SourcePlayer.IsAdmin(EAdminLevel.HeadAdmin))
			{
				if (!Enum.IsDefined(typeof(EAdminLevel), adminLevel))
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Incorrect admin level provided.");
					return;
				}

				BasicAccountInfo accountInfo = await Database.LegacyFunctions
					.GetBasicAccountInfoFromExactUsername(TargetPlayerUsername).ConfigureAwait(true);

				if (accountInfo.Result == BasicAccountInfo.EGetAccountInfoResult.NotFound)
				{
					SourcePlayer.SendNotification("Admin", ENotificationIcon.Font, "Account '{0}' was not found.",
						TargetPlayerUsername);
				}
				else if (accountInfo.Result == BasicAccountInfo.EGetAccountInfoResult.MultipleFound)
				{
					SourcePlayer.SendNotification("Admin", ENotificationIcon.Font,
						"Account '{0}' matched multiple accounts. Please specify a more specific username.",
						TargetPlayerUsername);
				}
				else
				{
					if (accountInfo.AdminLevel < SourcePlayer.AdminLevel)
					{
						await Database.LegacyFunctions.SetAdminLevel(accountInfo.AccountID, adminLevel).ConfigureAwait(true);
						SourcePlayer.SendNotification("Admin", ENotificationIcon.Font,
							Helpers.FormatString("You have set {0} to level {1} ({2}).",
								accountInfo.Username, adminLevel, adminLevel.ToString()));

						var log = new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand,
							null,
							Helpers.FormatString("/makeadmin - Promoted to level {0} ", adminLevel));
						log.addOfflineAffectedAccount(accountInfo.AccountID);
						log.execute();
					}
					else
					{
						SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "The account you are trying to change admin levels of is a higher or equal rank to you.");
					}
				}
			}
		}

		private void SaveAll(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			// If you update this, update HTTPServer.cs also
			if (SourcePlayer.IsAdmin(EAdminLevel.HeadAdmin))
			{
				PlayerPool.SaveAll();
				VehiclePool.SaveAll();

				SourcePlayer.SendNotification("Admin", ENotificationIcon.Font, "You have saved everything that is dynamic.");
			}
		}

		private void GotoPlayer(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
		{
			SourcePlayer.TeleportToPlayer(TargetPlayer);
			SourcePlayer.SendNotification("Admin", ENotificationIcon.Font, Helpers.FormatString("You have teleported to {0}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)));
			SourcePlayer.OnTeleport();

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, Helpers.FormatString("{0} {1} has teleported to you.", SourcePlayer.AdminTitle, SourcePlayer.Username));

			new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/goto - {0} teleported to {1}", SourcePlayer.Username, TargetPlayer.Username)).execute();
		}

		private void AdminUncuffPlayer(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
		{
			if (TargetPlayer.IsCuffed())
			{
				TargetPlayer.ForceUncuff();
				SourcePlayer.SendNotification("Admin", ENotificationIcon.Font, Helpers.FormatString("You have uncuffed {0}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)));

				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/auncuff")).execute();
			}
			else
			{
				SourcePlayer.SendNotification("Admin", ENotificationIcon.ExclamationSign, "Player is not cuffed.");
			}
		}

		private void GetPlayer(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
		{
			TargetPlayer.TeleportToPlayer(SourcePlayer);

			// TODO_CHAT: notification
			SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have teleported {0} to you.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			TargetPlayer.PushChatMessage(EChatChannel.AdminActions, "{0} {1} has teleported you to them.", SourcePlayer.AdminTitle, SourcePlayer.Username);

			TargetPlayer.OnTeleport();

			new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/gethere")).execute();
		}

		private async void OfflineGetPlayer(CPlayer SourcePlayer, CVehicle SourceVehicle, string FullCharacterName)
		{
			if (PlayerPool.GetPlayerFromPartialName(FullCharacterName).Instance() == null)
			{
				await Database.LegacyFunctions.OfflineSetPlayerPosition(FullCharacterName, SourcePlayer.Client.Position, SourcePlayer.Client.Dimension).ConfigureAwait(true);
				SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have teleported {0} to you.", FullCharacterName);
			}
			else
			{
				SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "{0} is online! Use /gethere instead.", FullCharacterName);
			}
		}

		private void UpdatePosition(CPlayer SourcePlayer, CVehicle SourceVehicle, Vector3 position)
		{
			if (SourceVehicle != null)
			{
				List<CPlayer> lstOccupants = VehiclePool.GetVehicleOccupants(SourceVehicle);
				SourceVehicle.TeleportAndWarpOccupants(lstOccupants, position, SourcePlayer.Client.Dimension, SourceVehicle.GTAInstance.Rotation);
				return;
			}

			SourcePlayer.Client.Position = position;
		}

		private void XCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, float x)
		{
			UpdatePosition(SourcePlayer, SourceVehicle, new Vector3(SourcePlayer.Client.Position.X + x, SourcePlayer.Client.Position.Y, SourcePlayer.Client.Position.Z));
		}

		private void YCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, float y)
		{
			UpdatePosition(SourcePlayer, SourceVehicle, new Vector3(SourcePlayer.Client.Position.X, SourcePlayer.Client.Position.Y + y, SourcePlayer.Client.Position.Z));
		}

		private void ZCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, float z)
		{
			UpdatePosition(SourcePlayer, SourceVehicle, new Vector3(SourcePlayer.Client.Position.X, SourcePlayer.Client.Position.Y, SourcePlayer.Client.Position.Z + z));
		}

		private void TeleportToXYZ(CPlayer SourcePlayer, CVehicle SourceVehicle, float x, float y, float z)
		{
			SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have teleported to {0}, {1}, {2}", x, y, z);
			UpdatePosition(SourcePlayer, SourceVehicle, new Vector3(x, y, z));

			SourcePlayer.OnTeleport();

			new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/xyz - {0}, {1}, {2}", x, y, z)).execute();
		}

		private void SetDimension(CPlayer SourcePlayer, CVehicle SourceVehicle, uint dimension)
		{
			SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have moved to dimension {0}", dimension);
			SourcePlayer.SetSafeDimension(dimension);

			SourcePlayer.OnTeleport();

			new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/dim - Set his dimension to {0}", dimension)).execute();
		}

		private void SendPlayerTo(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer CplayerToSend, CPlayer CplayerToRecieve)
		{
			if (CplayerToSend != null && CplayerToRecieve != null)
			{
				CplayerToSend.OnTeleport();
				CplayerToSend.TeleportToPlayer(CplayerToRecieve);
				SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have teleported {0} to {1}.", CplayerToSend.GetCharacterName(ENameType.StaticCharacterName), CplayerToRecieve.GetCharacterName(ENameType.StaticCharacterName));
				CplayerToSend.PushChatMessage(EChatChannel.AdminActions, "{0} {1} has teleported you to {2}.", SourcePlayer.AdminTitle, SourcePlayer.Username, CplayerToRecieve.GetCharacterName(ENameType.StaticCharacterName));
				CplayerToRecieve.PushChatMessage(EChatChannel.AdminActions, "{0} {1} has teleported {2} to you.", SourcePlayer.AdminTitle, SourcePlayer.Username, CplayerToSend.GetCharacterName(ENameType.StaticCharacterName));

				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { CplayerToSend, CplayerToRecieve }, Helpers.FormatString("/sendto - teleported {0} to {1}", CplayerToSend.Username, CplayerToRecieve.Username)).execute();
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Player not found.");
			}
		}

		private void SetWeather(CPlayer SenderPlayer, CVehicle SenderVehicle, int WeatherID)
		{
			if (HelperFunctions.World.SetWeather(WeatherID))
			{
				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "Weather set to {0} by {1} {2}", WeatherID, SenderPlayer.AdminTitle, SenderPlayer.Username);
				// Affected would be the entire server, omitting it here on purpose since it's given.
				new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/setw - Set weather to {0}", WeatherID)).execute();
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid weather ID.");
			}
		}

		private void ResetWeatherCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			Core.SetOverrideWeather(false, -1);
			SenderPlayer.SendNotification("Weather", ENotificationIcon.Star, "Weather reset successfully.");
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/resetw - Reset weather to standard random weather routine")).execute();
		}

		private void GotoAdminLounge(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			SenderPlayer.SetPositionSafe(new Vector3(2154.735f, 2921.066f, -81.07545f));
			SenderPlayer.Client.Rotation = new Vector3(0.0f, 0.0f, 265.9187f);
			SenderPlayer.SetSafeDimension(1337);
			SenderPlayer.OnTeleport();

			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Welcome to the Admin Lounge!");
		}

		private async void GotoPlace(CPlayer SenderPlayer, CVehicle SenderVehicle, string location)
		{
			STeleportPlace place = await Database.LegacyFunctions.GetTeleportPlaceFromName(location).ConfigureAwait(true);
			if (!place.found)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "No such teleport exists with name '{0}'.", location);
				return;
			}

			if (SenderPlayer.IsInVehicleReal && SenderPlayer.Client.Vehicle != null)
			{
				SenderPlayer.SetPositionSafe(new Vector3(place.x, place.y, place.z + 1.0f));
				SenderPlayer.SetSafeDimension(place.dimension);
			}
			else
			{
				SenderPlayer.SetPositionSafe(new Vector3(place.x, place.y, place.z + 1.0f));
				SenderPlayer.SetSafeDimension(place.dimension);
			}


			SenderPlayer.OnTeleport();

			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Teleported to location '{0}'.", location);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/gotoplace - Teleported to {0} ", location)).execute();
		}

		private void ListTrainStations(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Train Stations:");
			foreach (CTrainTripWire tripWire in TrainConstants.TripWires)
			{
				if (tripWire.TripWireType == ETrainTripWireType.StationStop)
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "- {0} = {1}", tripWire.Sector, tripWire.Name);
				}
			}
		}

		private void GotoTrainStation(CPlayer SenderPlayer, CVehicle SenderVehicle, int stationID)
		{
			foreach (CTrainTripWire tripWire in TrainConstants.TripWires)
			{
				if (tripWire.TripWireType == ETrainTripWireType.StationStop && tripWire.Sector == stationID)
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Teleported to station #{0} ({1})", tripWire.Sector, tripWire.Name);

					SenderPlayer.SetSafeDimension(0);
					UpdatePosition(SenderPlayer, SenderVehicle, tripWire.ExitPosition);

					SenderPlayer.OnTeleport();
					return;
				}
			}

			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Station #{0} was not found. Use /trainstations to see a list of all stations", stationID);
		}

		private async void Places(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			CTeleportPlaces teleports = await Database.LegacyFunctions.GetTeleportPlaces().ConfigureAwait(true);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Teleport Locations:");
			foreach (STeleportPlace place in teleports.places)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "- {0}", place.name);
			}
		}

		private async void AddPlace(CPlayer SenderPlayer, CVehicle SenderVehicle, string LocationName)
		{
			Regex r = new Regex("^[a-zA-Z]*$");
			if (!r.IsMatch(LocationName))
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "Teleport location names can only have alphabetical characters.");
				return;
			}

			STeleportPlace PlaceTask = await Database.LegacyFunctions.GetTeleportPlaceFromName(LocationName).ConfigureAwait(true);
			if (PlaceTask.found)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "A teleport location already exists with name '{0}'!", LocationName);
				return;
			}

			Database.LegacyFunctions.CreateTeleportLocation(LocationName, SenderPlayer.Client.Position.X, SenderPlayer.Client.Position.Y, SenderPlayer.Client.Position.Z, SenderPlayer.Client.Dimension, SenderPlayer.AccountID);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Created teleport location '{0}'.", LocationName);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/addplace - Created location {0} ", LocationName)).execute();
		}

		private void DeletePlace(CPlayer SenderPlayer, CVehicle SenderVehicle, string LocationName)
		{
			Database.LegacyFunctions.DeleteTeleportLocation(LocationName);

			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Deleted teleport location '{0}'.", LocationName);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/delplace - Deleted location {0} ", LocationName)).execute();
		}

		private void SetHealth(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int health)
		{
			if (health < 0 || health > 100)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid health setting.");
				return;
			}

			if (TargetPlayer.Client.Dead)
			{
				Revive(SenderPlayer, SenderVehicle, TargetPlayer);
			}

			TargetPlayer.Client.Health = health;
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Set health of {0} to {1}%.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), health);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/sethp - Set health to {0}", health)).execute();
		}

		private void SetArmor(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int armor)
		{
			if (armor < 0 || armor > 100)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid armor setting.");
				return;
			}

			if (TargetPlayer.Client.Dead)
			{
				Revive(SenderPlayer, SenderVehicle, TargetPlayer);
			}

			TargetPlayer.Client.Armor = armor;
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Set armor of {0} to {1}%.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), armor);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/setarmor - Set armor to {0}", armor)).execute();
		}

		private void SetTime(CPlayer SenderPlayer, CVehicle SourceVehicle, int hour)
		{
			if (hour < 0 || hour > 23)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid hour setting (0-23).");
				return;
			}

			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, $"Set world time to {hour}:00.");
			Core.SetOverrideTime(true, hour);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/settime - Set hours to {0} ", hour)).execute();
		}

		private void ResetTime(CPlayer SenderPlayer, CVehicle SourceVehicle)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Set world time back to server time.");
			Core.SetOverrideTime(false, -1);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/resettime - Set server time to default")).execute();
		}

		private void SetPlayerSkin(CPlayer SenderPlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, string SkinHash)
		{
			uint hash = Convert.ToUInt32(SkinHash, 16);
			if (Enum.IsDefined(typeof(PedHash), hash))
			{
				TargetPlayer.SetCharacterSkin((PedHash)hash);
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, $"Set {TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)}'s skin to {((PedHash)hash).ToString()}.");
				TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, $"Your skin has been set to {((PedHash)hash).ToString()} by {SenderPlayer.Username}");
				new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/setskin - Set skin to hash {0} ", hash)).execute();
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, $"Invalid Skin Hash.");
			}
		}

		private void SetPlayerAge(CPlayer SenderPlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, int age)
		{

			if (age < CharacterConstants.MinAge)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Age can't be lower than {0}!", CharacterConstants.MinAge);
			}
			else
			{
				TargetPlayer.SetPlayerAge(age);
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, $"Set {TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)}'s age to {age}.");
				TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, $"Your age has been set to {age} by {SenderPlayer.Username}");
			}
		}

		private async void CreateMetalDetector(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			Vector3 detectorPosition = SourcePlayer.Client.Position.Copy();
			Vector3 detectorRotation = SourcePlayer.Client.Rotation.Copy();
			uint detectorDimension = SourcePlayer.Client.Dimension;

			detectorPosition.Z -= 1.0f;

			await MetalDetectorPool.CreateMetalDetector(0, detectorPosition, detectorRotation.Z, detectorDimension, true).ConfigureAwait(true);

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "Detector was created!");
		}

		private void DeleteMetalDetector(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID metalDetectorID)
		{
			try
			{
				CMetalDetectorInstance metalDetector = MetalDetectorPool.GetMetalDetectorInstanceFromID(metalDetectorID);
				if (metalDetector != null)
				{
					MetalDetectorPool.DestroyMetalDetector(metalDetector);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted the metal detector with id {0}.", metalDetectorID);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "No metal detector exists with id {0}", metalDetectorID);
				}
			}
			catch
			{

			}
		}

		private async void FindAlts(CPlayer SenderPlayer, CVehicle SenderVehicle, int type, string strEntry)
		{
			void Syntax()
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "SYNTAX: [Type - 0: Character name | 1: Account name] [Name]");
			}

			Dictionary<string, string> characters = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(strEntry))
			{
				switch (type)
				{
					case 0:
						characters = await Database.LegacyFunctions.GetCharactersByName(strEntry).ConfigureAwait(true);
						break;
					case 1:
						characters = await Database.LegacyFunctions.GetCharactersByAccount(strEntry).ConfigureAwait(true);
						break;
					default:
						break;
				}
			}
			else
			{
				Syntax();
				return;
			}

			if (characters.Count > 0)
			{
				foreach (var character in characters)
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "{0} - {1}", character.Key, character.Value);
				}
				new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/findalts - Searched characters for entry {0} ", strEntry)).execute();
			}
			else
			{
				Syntax();
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "No results were found for entry {0}", strEntry);
				return;
			}
		}

		private void KickPlayer(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, string reason)
		{
			if (string.IsNullOrEmpty(reason))
			{
				reason = "No reason specified.";
			}

			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} ({1}) was kicked by {2} for the following reason: {3}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetPlayer.Username, SourcePlayer.Username, reason), r: 255, g: 0, b: 0);
			new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/kick - {0} ({1}) was removed for the following reason: {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetPlayer.Username, reason)).execute();
			NAPI.Player.KickPlayer(TargetPlayer.Client, reason);
		}

		private void Disappear(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			if (!SourcePlayer.IsDisappeared)
			{
				SourcePlayer.Client.Transparency = 0;
				SourcePlayer.IsDisappeared = true;
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "You have now disappeared, type again /disappear to become visible.");
			}
			else
			{
				SourcePlayer.Client.Transparency = 255;
				SourcePlayer.IsDisappeared = false;
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "You have now appeared, type again /disappear to become invisible.");
			}
		}

		private void Revive(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer targetPlayer)
		{
			if (targetPlayer.Client.Dead)
			{
				ReviveInner(SourcePlayer, targetPlayer);
				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { targetPlayer }, Helpers.FormatString("/revive - {0} ({1}) was revived", targetPlayer.GetCharacterName(ENameType.StaticCharacterName), targetPlayer.Username)).execute();
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, Helpers.FormatString("{0} is not dead!", targetPlayer.GetCharacterName(ENameType.StaticCharacterName)));
			}
		}

		private void ReviveInner(CPlayer SourcePlayer, CPlayer targetPlayer)
		{
			NAPI.Player.SpawnPlayer(targetPlayer.Client, targetPlayer.Client.Position, targetPlayer.Client.Rotation.Z);
			NAPI.Player.SetPlayerHealth(targetPlayer.Client, 100);
			targetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, Helpers.FormatString("You were revived by {0} {1} (({2})) ", SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), SourcePlayer.Username));
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, Helpers.FormatString("{0} was revived.", targetPlayer.GetCharacterName(ENameType.StaticCharacterName)));
		}

		private void ForceRevive(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer targetPlayer)
		{
			ReviveInner(SourcePlayer, targetPlayer);
			new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { targetPlayer }, Helpers.FormatString("/fprcerevive - {0} ({1}) was force revived", targetPlayer.GetCharacterName(ENameType.StaticCharacterName), targetPlayer.Username)).execute();
		}

		private void GiveGunLicense(CPlayer player, CVehicle vehicle, CPlayer other, int tier)
		{
			if (!player.IsAdmin(EAdminLevel.LeadAdmin))
			{
				return;
			}

			if (tier > 2 || tier < 1)
			{
				player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "License tier must be either tier 1 or tier 2.");
				return;
			}

			CItemInstanceDef itemDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER1, other.ActiveCharacterDatabaseID);
			if (tier == 2)
			{
				itemDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER2, other.ActiveCharacterDatabaseID);
			}

			other.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, null);

			player.SendNotification("Weapons License", ENotificationIcon.ThumbsUp, "You have given {0} a firearms license (Tier {1}),", other.GetCharacterName(ENameType.StaticCharacterName), tier);
			if (other == player) return;
			other.SendNotification("Weapons License", ENotificationIcon.ThumbsUp, "You have received a firearms license (Tier {1}) from {0}", player.GetCharacterName(ENameType.StaticCharacterName), tier);
		}

		private void ForceAppPlayer(CPlayer player, CVehicle vehicle, CPlayer other)
		{
			player.PushChatMessage(EChatChannel.AdminActions, "You have sent {0} back to the application stage.", other.Username);
			Database.Functions.Accounts.SetApplicationState(other.AccountID, EApplicationState.NoApplicationSubmitted);
			NAPI.Player.KickPlayer(other.Client, "You have been forced back to the application stage. Please fill out an application to the server to play again.");
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} sent {2} back to the application stage.", player.AdminTitle, player.Username, other.Username), true,
				EAdminLevel.TrialAdmin, 255, 0, 0, EChatChannel.AdminActions);
		}

		private void GiveLicense(CPlayer player, CVehicle vehicle, CPlayer TargetPlayer, EDrivingTestType type)
		{
			EItemID itemID = EItemID.None;
			switch (type)
			{
				case EDrivingTestType.Bike:
					itemID = EItemID.DRIVERS_PERMIT_BIKE;
					break;
				case EDrivingTestType.Car:
					itemID = EItemID.DRIVERS_PERMIT_CAR;
					break;
				case EDrivingTestType.Truck:
					itemID = EItemID.DRIVERS_PERMIT_LARGE;
					break;
			}

			if (itemID == EItemID.None)
			{
				player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 200, 200, "Invalid driving license type. Valid Types:");
				player.PushChatMessage(EChatChannel.Notifications, "1 - Bike");
				player.PushChatMessage(EChatChannel.Notifications, "2 - Car");
				player.PushChatMessage(EChatChannel.Notifications, "3 - Truck");
				return;
			}

			CItemInstanceDef item = CItemInstanceDef.FromBasicValueNoDBID(itemID, TargetPlayer.ActiveCharacterDatabaseID);
			TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
			{
				if (!bItemGranted)
				{
					player.PushChatMessage(EChatChannel.Notifications, "{0} does not have enough inventory space.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
					return;
				}

				Log.CreateLog(player, ELogType.AdminCommand, new List<CBaseEntity>() { TargetPlayer }, Helpers.FormatString("/givelicense Gave item {0}", item.GetName()));

				TargetPlayer.SendNotification("Items", ENotificationIcon.InfoSign, "You have been given a {0} by {1}.", item.GetName(), player.GetCharacterName(ENameType.StaticCharacterName));
				player.SendNotification("Items", ENotificationIcon.InfoSign, "You have given {0} a {1}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), item.GetName());
			});
		}

		private void AddGC(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int amount)
		{
			if (!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				return;
			}

			TargetPlayer.AddDonatorCurrency(amount);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) gave you {3} GC (donator currency).", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, amount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You gave {1} GC to '{0}'.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) gave {4} GC to '{3}'.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/addgc {0} {1} - New balance: {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount, TargetPlayer.GetDonatorCurrency())).execute();
		}

		private void TakeGC(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int amount)
		{
			if (!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				return;
			}

			TargetPlayer.SubtractDonatorCurrency(amount);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) removed {3} from your GC balance (donator currency).", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, amount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You removed {1} from '{0}s' GC.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) removed {4} from '{3}s' GC.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/takemoney {0} {1} - New balance: ${2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount, TargetPlayer.GetDonatorCurrency())).execute();
		}

		private void SetGC(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int amount)
		{
			if (!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				return;
			}

			TargetPlayer.SetDonatorCurrency(amount);

			TargetPlayer.SendNotification("Admin", ENotificationIcon.Font, "{0} {1} ({2}) set your GC balance (donator currency) to {3}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, amount);
			SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "You set '{0}s' GC balance to {1}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} ({2}) set the GC balance of '{4}' to {3}.", SenderPlayer.AdminTitle, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount), r: 255, g: 0, b: 0);
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/setmoney - {0} ${1} - New balance: ${2} ", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount, TargetPlayer.GetDonatorCurrency())).execute();
		}

		// START 4th of july
		private void Start4Th(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			if (SenderPlayer.AdminLevel < EAdminLevel.LeadAdmin)
			{
				return;
			}

			if (!HelperFunctions.World.IsFourthOfJuly())
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "It's not 4th of July");
				return;
			}

			if (HelperFunctions.World.IsFourthOfJulyEventInProgress())
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Event already in progress. Wait for it to finish if you want to restart it.");
				return;
			}

			NetworkEventSender.SendNetworkEvent_StartFourthOfJuly_ForAll_SpawnedOnly();
			HelperFunctions.World.SetFourthOfJulyEvent(true);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} has initiated the 4th of July event.", SenderPlayer.Username), r: 255, g: 0, b: 0);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Succsessfully started the 4TH of July event!");
		}

		private void EndFourthOfJulyEvent(CPlayer player)
		{
			HelperFunctions.World.SetFourthOfJulyEvent(false);
		}
		// END 4th of july

		private void BlockCmdCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, string strCommandToBlock)
		{
			if (SenderPlayer.AdminLevel < EAdminLevel.LeadAdmin)
			{
				return;
			}

			string strAnswer = CommandManager.BlockCommand(strCommandToBlock);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 255, 255, strAnswer);
		}

		private void UnblockCmdCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, string strCommandToUnblock)
		{
			if (SenderPlayer.AdminLevel < EAdminLevel.LeadAdmin)
			{
				return;
			}

			string strAnswer = CommandManager.UnBlockCommand(strCommandToUnblock);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 255, 255, strAnswer);
		}

		public void ShowInventoryCommand(CPlayer player, CVehicle _, CPlayer target)
		{
			target.Frisk(player, true);
		}
	}
}
