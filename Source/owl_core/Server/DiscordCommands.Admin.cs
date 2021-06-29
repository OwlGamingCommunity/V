using Database;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DiscordCommands_Admin
{
	private DateTime g_StartTime = DateTime.Now;

	public DiscordCommands_Admin()
	{
		DiscordCommandManager.RegisterCommand("players", "Shows player count", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(PlayerCount), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.TrialAdmin);
		DiscordCommandManager.RegisterCommand("playerlist", "Shows player list", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(PlayerList), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.TrialAdmin);
		DiscordCommandManager.RegisterCommand("uptime", "Shows uptime", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(Uptime), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.TrialAdmin);
		DiscordCommandManager.RegisterCommand("apps", "Shows pending applications", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(PendingApps), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.TrialAdmin);
		DiscordCommandManager.RegisterCommand("viewapp", "Shows pending application details", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs, int>(ViewApp), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.TrialAdmin);
		DiscordCommandManager.RegisterCommand("approveapp", "Approves an application", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs, int>(ApproveApp), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.TrialAdmin);
		DiscordCommandManager.RegisterCommand("denyapp", "Denies an application", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs, int>(DenyApp), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.TrialAdmin);
		DiscordCommandManager.RegisterCommand("showram", "Shows current RAM usage", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(ShowRAM), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.LeadAdmin);
		DiscordCommandManager.RegisterCommand("showsql", "Shows current SQL stats", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(ShowSQL), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.LeadAdmin);
		DiscordCommandManager.RegisterCommand("restartbot", "Restarts the Discord bot", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(RestartBot), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.LeadAdmin);
		DiscordCommandManager.RegisterCommand("bandwidth", "Shows bandwidth savings", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(Bandwidth), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.RequiresLinkedAccount, EAdminLevel.LeadAdmin);
	}

	private void Uptime(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		TimeSpan uptime = DateTime.Now - g_StartTime;
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("Server started at {0}", g_StartTime.ToString("r")));
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("Uptime: {0}", uptime.ToString(@"d\d\:h\h\:m\m\:s\s")));
	}

	private void PendingApps(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		Database.Functions.Accounts.GetPendingApplications((List<PendingApplication> lstPendingApps) =>
		{
			string strResponse = "Pending Applications: ";

			if (lstPendingApps.Count > 0)
			{
				foreach (PendingApplication app in lstPendingApps)
				{
					strResponse += Helpers.FormatString("\n\t#{0} - {1}", app.AccountID, app.AccountName);
				}
			}
			else
			{
				strResponse += Helpers.FormatString("\n\tNone");
			}

			DiscordBotIntegration.PushMessage(discordUser, channelToUse, strResponse);
		});
	}

	private void ViewApp(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse, int AppID)
	{
		Database.Functions.Accounts.GetPendingApplicationDetails(AppID, (PendingApplicationDetails appDetails) =>
		{
			if (appDetails != null)
			{
				DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("Pending Application #{0}: \n\tApplications Made:{1}", AppID, appDetails.NumApps));

				int iterIndex = 0;
				foreach (int questionIndex in appDetails.QuestionIndices)
				{
					if (questionIndex < QuizWrittenQuestionDefinitions.g_QuizQuestionDefinitions.Count)
					{
						appDetails.Questions[iterIndex] = QuizWrittenQuestionDefinitions.g_QuizQuestionDefinitions[questionIndex].Question;
					}
					else
					{
						appDetails.Questions[iterIndex] = "SCRIPT NOTICE: This question was most likely removed since the player submitted their app. As an admin, you can ignore it";
					}

					DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("\n\tQuestion {0}: {1}", iterIndex + 1, appDetails.Questions[iterIndex]));
					DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("\n\tAnswer {0}: {1}", iterIndex + 1, appDetails.Answers[iterIndex]));

					++iterIndex;
				}
			}
			else
			{
				DiscordBotIntegration.PushMessage(discordUser, channelToUse, "No such application exists or the application is already approved/denied");
			}
		});
	}

	private void ApproveApp(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse, int AppID)
	{
		Database.Functions.Accounts.GetPendingApplicationDetails(AppID, (PendingApplicationDetails appDetails) =>
		{
			if (appDetails != null)
			{
				ApplicationsSharedLogic.SetApplicationState(AppID, EApplicationState.ApplicationApproved);
				DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("Application '{0}' was approved by '{1} ({2})'!", AppID, discordUser.Username, accountInfo.Username));
			}
			else
			{
				DiscordBotIntegration.PushMessage(discordUser, channelToUse, "No such application exists or the application is already approved/denied");
			}
		});
	}

	private void DenyApp(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse, int AppID)
	{
		Database.Functions.Accounts.GetPendingApplicationDetails(AppID, (PendingApplicationDetails appDetails) =>
		{
			if (appDetails != null)
			{
				ApplicationsSharedLogic.SetApplicationState(AppID, EApplicationState.ApplicationRejected);
				DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("Application '{0}' was denied by '{1} ({2})'!", AppID, discordUser.Username, accountInfo.Username));
			}
			else
			{
				DiscordBotIntegration.PushMessage(discordUser, channelToUse, "No such application exists or the application is already approved/denied");
			}
		});
	}

	private void ShowRAM(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
		System.Diagnostics.Process botProc = DiscordBotIntegration.GetBotProcess();
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("Current Server RAM usage is: WorkingSet: {0:0.00}MB PrivateMem: {1:0.00}MB", (double)proc.WorkingSet64 / 1024 / 1024, (double)proc.PrivateMemorySize64 / 1024 / 1024));

		if (botProc != null)
		{
			DiscordBotIntegration.PushMessage(discordUser, channelToUse, Helpers.FormatString("Current Discord Bot RAM usage is: WorkingSet: {0:0.00}MB PrivateMem: {1:0.00}MB", (double)botProc.WorkingSet64 / 1024 / 1024, (double)botProc.PrivateMemorySize64 / 1024 / 1024));
		}
		else
		{
			DiscordBotIntegration.PushMessage(discordUser, channelToUse, "Current Discord Bot RAM usage is unavailable - bot is not running.");
		}
	}

	private async void ShowSQL(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, "MySQL Stats:");
		List<string> lstStats = ThreadedMySQL.GetDebugStats();
		foreach (string strStat in lstStats)
		{
			await Task.Delay(1000).ConfigureAwait(true);
			DiscordBotIntegration.PushMessage(discordUser, channelToUse, strStat);
		}
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, "=================");
	}

	private async void RestartBot(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, "Restarting!");
		await Task.Delay(1000).ConfigureAwait(true);

		DiscordBotIntegration.KillBotProcess();
		await Task.Delay(1000).ConfigureAwait(true);

		DiscordBotIntegration.StartBotProcess();
	}

	private void Bandwidth(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		ServerPerfManager.GetTotalBytesSentData(out UInt64 sent, out UInt64 sentCompressed, out double sizeKB, out double sizeCompressedKB, out double sizeMB, out double sizeCompressedMB, out string strDisplayString);
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, strDisplayString);

		ServerPerfManager.GetTotalBytesReceivedData(out UInt64 recv, out UInt64 recvCompressed, out double recvsizeKB, out double recvsizeCompressedKB, out double recvsizeMB, out double recvsizeCompressedMB, out string strRecvDisplayString);
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, strRecvDisplayString);
	}

	private void PlayerCount(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		string strResponseDM = Helpers.FormatString("OwlV has {0} players!", NAPI.Pools.GetAllPlayers().Count);
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, strResponseDM);
	}

	private void PlayerList(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		int count = NAPI.Pools.GetAllPlayers().Count;
		string strResponseDM = Helpers.FormatString("OwlV has {0} players:", count);

		if (count > 0)
		{
			foreach (var player in PlayerPool.GetAllPlayers_IncludeOutOfGame())
			{
				int latency = player.GetData<int>(player.Client, EDataNames.PING);
				int playerID = player.PlayerID;
				string strState = "Playing";

				bool bIsLoggedIn = player.IsLoggedIn;
				bool bIsSpawned = player.IsSpawned;

				if (!bIsLoggedIn)
				{
					strState = "Logging In";
				}
				else if (bIsLoggedIn && !bIsSpawned)
				{
					strState = "Selecting Character";
				}

				strResponseDM += Helpers.FormatString("\n\t#{0} - {1} - {2} - {3}ms", playerID, player.GetCharacterName(ENameType.StaticCharacterName).Length == 0 ? "No Character Yet" : player.GetCharacterName(ENameType.StaticCharacterName), strState, latency);
			}
		}
		else
		{
			strResponseDM += "\n\t No Players";
		}

		DiscordBotIntegration.PushMessage(discordUser, channelToUse, strResponseDM);
	}
}