using System;
using System.Collections.Generic;

namespace ApplicationAdminCommands
{
	public class Applications
	{
		public Applications()
		{
			// COMMANDS
			CommandManager.RegisterCommand("apps", "Show pending applications", new Action<CPlayer, CVehicle>(PendingApps), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

			NetworkEvents.RequestPendingApplications += RequestPendingApplications;
			NetworkEvents.RequestApplicationDetails += RequestApplicationDetails;
			NetworkEvents.ApproveApplication += ApproveApplication;
			NetworkEvents.DenyApplication += DenyApplication;
		}

		public void PendingApps(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			Database.Functions.Accounts.GetPendingApplications((List<PendingApplication> lstPendingApps) =>
			{
				NetworkEventSender.SendNetworkEvent_Admin_GotPendingApps(SenderPlayer, lstPendingApps);
			});
		}

		// event sent from UI for refreshing the interface
		public void RequestPendingApplications(CPlayer SenderPlayer)
		{
			if (SenderPlayer.IsAdmin())
			{
				PendingApps(SenderPlayer, null);
			}
		}

		public void RequestApplicationDetails(CPlayer SenderPlayer, int AccountID)
		{
			if (SenderPlayer.IsAdmin())
			{
				Database.Functions.Accounts.GetPendingApplicationDetails(AccountID, (PendingApplicationDetails pendingAppDetails) =>
				{
					if (pendingAppDetails != null)
					{
						// We have to convert the question indices into actual question strings, MYSQL cant do this as it can't see the types and we don't want additional deps
						int iterIndex = 0;
						foreach (int questionIndex in pendingAppDetails.QuestionIndices)
						{
							if (questionIndex < QuizWrittenQuestionDefinitions.g_QuizQuestionDefinitions.Count)
							{
								pendingAppDetails.Questions[iterIndex] = QuizWrittenQuestionDefinitions.g_QuizQuestionDefinitions[questionIndex].Question;
							}
							else
							{
								pendingAppDetails.Questions[iterIndex] = "SCRIPT NOTICE: This question was most likely removed since the player submitted their app. As an admin, you can ignore it";
							}
							++iterIndex;
						}

						NetworkEventSender.SendNetworkEvent_PendingApplicationDetails(SenderPlayer, pendingAppDetails);
					}
				});
			}
		}

		public void ApproveApplication(CPlayer SenderPlayer, int AccountID)
		{
			if (SenderPlayer.IsAdmin())
			{
				Database.Functions.Accounts.GetPendingApplicationDetails(AccountID, (PendingApplicationDetails appDetails) =>
				{
					if (appDetails != null)
					{
						ApplicationsSharedLogic.SetApplicationState(AccountID, EApplicationState.ApplicationApproved);
						PendingApps(SenderPlayer, null);

						// Discord
						DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AdminCommands, "Application '{0}' was approved by '{1} ({2})'!", AccountID, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username);
					}
				});
			}
		}

		public void DenyApplication(CPlayer SenderPlayer, int AccountID)
		{
			if (SenderPlayer.IsAdmin())
			{
				Database.Functions.Accounts.GetPendingApplicationDetails(AccountID, (PendingApplicationDetails appDetails) =>
				{
					if (appDetails != null)
					{
						ApplicationsSharedLogic.SetApplicationState(AccountID, EApplicationState.ApplicationRejected);
						PendingApps(SenderPlayer, null);

						// Discord
						DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AdminCommands, "Application '{0}' was denied by '{1} ({2})'!", AccountID, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username);
					}
				});
			}
		}
	}
}


