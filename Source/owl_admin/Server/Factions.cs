using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

namespace PlayerAdminCommands
{
	public class Factions
	{
		public Factions()
		{
			// COMMANDS
			CommandManager.RegisterCommand("afactionlist", "Gets a list of factions with ID's", new Action<CPlayer, CVehicle>(GetFactionList), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "factions", "factionlist", "listfactions", "showfactions" });
			CommandManager.RegisterCommand("asetfaction", "Sets a target player into a faction with the specified rank", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID, int, bool>(SetFaction), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setfaction" });
			CommandManager.RegisterCommand("makefaction", "Creates a faction", new Action<CPlayer, CVehicle, CPlayer, string, EFactionType, bool, string>(MakeFaction), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("delfaction", "Deletes a faction", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteFaction), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setfactionmoney", "Set the factions money", new Action<CPlayer, CVehicle, EntityDatabaseID, int>(SetFactionMoney), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("addfactionmoney", "Adds faction money", new Action<CPlayer, CVehicle, EntityDatabaseID, int>(AddFactionMoney), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("takefactionmoney", "Takes faction money", new Action<CPlayer, CVehicle, EntityDatabaseID, int>(TakeFactionMoney), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setfactionname", "Sets the name and short name of an existing faction", new Action<CPlayer, CVehicle, EntityDatabaseID, string, string>(ChangeFactionName), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);

			NetworkEvents.AdminDeleteFaction += OnConfirmFactionDelete;

			NetworkEvents.Faction_AdminViewFactions_DeleteFaction += OnAdminViewFactions_DeleteFaction;
			NetworkEvents.Faction_AdminViewFactions_JoinFaction += OnAdminViewFactions_JoinFaction;
			NetworkEvents.Faction_AdminRequestViewFactions += (CPlayer SourcePlayer) =>
			{
				GetFactionList(SourcePlayer, null);
			};
		}

		private void OnAdminViewFactions_DeleteFaction(CPlayer SourcePlayer, EntityDatabaseID factionId)
		{
			if (SourcePlayer.IsAdmin(EAdminLevel.SeniorAdmin))
			{
				// check faction still exists, might have been destroyed by another admin while the UI was up
				CFaction targetFaction = FactionPool.GetFactionFromID(factionId);
				if (targetFaction != null)
				{
					string name = targetFaction.Name;
					string shortName = targetFaction.ShortName;

					Log.CreateLog(SourcePlayer, ELogType.AdminCommand, new List<CBaseEntity>() { targetFaction }, $"Admin View Faction List: Delete {name} ({shortName}) - ID: {factionId}");
					FactionPool.DestroyFaction(targetFaction);

					SourcePlayer.SendNotification("Delete Faction", ENotificationIcon.ExclamationSign, "You have deleted faction {0}({1}) (#{2}).", name, shortName, factionId);

				}
				else
				{
					SourcePlayer.SendNotification("Delete Faction", ENotificationIcon.ExclamationSign, "Faction not found.");
				}
			}
			else
			{
				SourcePlayer.SendNotification("Delete Faction", ENotificationIcon.ExclamationSign, "You must be a Senior Admin or higher to perform this action.");
			}

			// send updated list
			GetFactionList(SourcePlayer, null);
		}

		private void OnAdminViewFactions_JoinFaction(CPlayer SourcePlayer, EntityDatabaseID factionId)
		{
			// check faction still exists, might have been destroyed by another admin while the UI was up
			CFaction targetFaction = FactionPool.GetFactionFromID(factionId);
			if (targetFaction != null)
			{
				string name = targetFaction.Name;
				string shortName = targetFaction.ShortName;

				Log.CreateLog(SourcePlayer, ELogType.AdminCommand, new List<CBaseEntity>() { targetFaction }, $"Admin View Faction List: Join {name} ({shortName}) - ID: {factionId}");

				if (SourcePlayer.IsInFaction(factionId))
				{
					SourcePlayer.SendNotification("Join Faction", ENotificationIcon.ExclamationSign, "You are already in this faction.");
				}
				else
				{
					SourcePlayer.AddFactionMembership(new CFactionMembership(targetFaction, true, 0), true);
					SourcePlayer.SendNotification("Join Faction", ENotificationIcon.ExclamationSign, "You have joined faction {0}({1}) (#{2}).", name, shortName, factionId);
				}
			}
			else
			{
				SourcePlayer.SendNotification("Join Faction", ENotificationIcon.ExclamationSign, "Faction not found.");
			}

			// send updated list
			GetFactionList(SourcePlayer, null);
		}

		// TODO_POST_LAUNCH: UI
		private async void GetFactionList(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			List<CFactionListTransmit> lstFactions = new List<CFactionListTransmit>();

			var factionList = FactionPool.GetAllFactions();
			foreach (var kvPair in factionList)
			{
				CFaction faction = kvPair.Value;
				int numMembers = await faction.GetTotalNumMembers().ConfigureAwait(true);

				if (faction.CreatorID != -1)
				{
					Database.Functions.Characters.GetCharacterNameFromDBID(faction.CreatorID, (string strReturnedName) =>
					{
						AddFactionToList(faction, strReturnedName, numMembers);
					});
				}
				else
				{
					AddFactionToList(faction, "", numMembers);
				}
			}

			void AddFactionToList(CFaction faction, string strCreatorName, int numMembers)
			{
				lstFactions.Add(new CFactionListTransmit(faction.FactionID, faction.ShortName, faction.Name, faction.Money, numMembers, faction.CreatorID, strCreatorName));

				// are we done?
				if (lstFactions.Count == factionList.Count)
				{
					lstFactions = lstFactions.OrderBy(iterFaction => iterFaction.FactionID).ToList();
					NetworkEventSender.SendNetworkEvent_Faction_AdminViewFactions(SourcePlayer, lstFactions);
				}
			}
		}

		private async void MakeFaction(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer FactionLeader, string ShortName, EFactionType FactionType, bool IsOfficial, string FullName)
		{
			if (!SourcePlayer.IsAdmin(EAdminLevel.LeadAdmin))
			{
				return;
			}

			if (FullName.Length < 5)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Full Name is too short.");
			}
			else if (ShortName.Length < 2)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Short Name is too short.");
			}
			else if (FullName.Length > 64)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Full Name is too long.");
			}
			else if (ShortName.Length > 4)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Short Name is too long.");
			}
			else
			{
				// Check names are unique
				Database.LegacyFunctions.EFactionNameUniqueResult FactionNameUniqueRes = await Database.LegacyFunctions.IsFactionNameUnique(FullName, ShortName).ConfigureAwait(true);
				if (FactionNameUniqueRes == Database.LegacyFunctions.EFactionNameUniqueResult.IsUnique)
				{
					if (FactionLeader != null)
					{
						CDatabaseStructureFaction faction = await Database.LegacyFunctions.CreateFaction(FactionLeader.ActiveCharacterDatabaseID, FullName, ShortName, FactionType, IsOfficial).ConfigureAwait(true);

						if (faction != null)
						{
							CFaction loadedFaction = FactionPool.CreateFaction(faction.factionID, faction.factionType, faction.strName, faction.bOfficial, faction.strShortName, faction.strMessage, faction.fMoney, faction.lstFactionRanks, faction.CreatorID);
							FactionLeader.AddFactionMembership(new CFactionMembership(loadedFaction, true, 0), true);

							FactionLeader.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "{0} {1} ({2}) created a faction '{3}' and set you to the leader.", SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), SourcePlayer.Username, FullName);
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Faction '{0}' was created with id {1} and owner '{2}'", FullName, loadedFaction.FactionID, FactionLeader.GetCharacterName(ENameType.StaticCharacterName));
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Failed to create faction. Check player is still online.");
						}
					}
					else
					{
						SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction leader is offline.");
					}
				}
				else if (FactionNameUniqueRes == Database.LegacyFunctions.EFactionNameUniqueResult.FullNameTaken)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Full Name is already in use.");
				}
				else if (FactionNameUniqueRes == Database.LegacyFunctions.EFactionNameUniqueResult.ShortNameTaken)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Short Name is already in use.");
				}
			}
		}

		private void DeleteFaction(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID factionId)
		{
			if (!SourcePlayer.IsAdmin(EAdminLevel.SeniorAdmin))
			{
				return;
			}

			CFaction targetFaction = FactionPool.GetFactionFromID(factionId);
			if (targetFaction != null)
			{
				NetworkEventSender.SendNetworkEvent_AdminConfirmEntityDelete(SourcePlayer, factionId, EEntityType.Faction);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction not found.");
			}
		}

		private void OnConfirmFactionDelete(CPlayer SourcePlayer, EntityDatabaseID factionId)
		{
			if (SourcePlayer.IsAdmin(EAdminLevel.SeniorAdmin))
			{
				// check faction still exists, might have been destroyed by another admin while the UI was up
				CFaction targetFaction = FactionPool.GetFactionFromID(factionId);
				if (targetFaction != null)
				{
					string name = targetFaction.Name;
					string shortName = targetFaction.ShortName;

					Log.CreateLog(SourcePlayer, ELogType.AdminCommand, new List<CBaseEntity>() { targetFaction }, $"/delfaction {name} ({shortName}) - ID: {factionId}");
					FactionPool.DestroyFaction(targetFaction);

					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted faction {0}({1}) (#{2}).", name, shortName, factionId);

				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction not found.");
				}
			}
		}

		private void SetFaction(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, EntityDatabaseID FactionID, int FactionRank, bool IsManager)
		{
			CFaction TargetFaction = FactionPool.GetFactionFromID(FactionID);
			if (TargetFaction != null)
			{
				// TODO_POST_LAUNCH: If we increase # of ranks, gotta increase this too
				if (FactionRank >= 0 && FactionRank < 20)
				{
					// Is the player already in this faction?
					if (TargetPlayer.IsInFaction(FactionID))
					{
						SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Target Player '{0}' is already in the {1} faction (ID {2}).", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetFaction.Name, FactionID);
					}
					else
					{
						if (FactionRank > TargetFaction.FactionRanks.Count)
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "The faction rank that was entered does not exist for this faction.");
						}
						else
						{
							TargetPlayer.AddFactionMembership(new CFactionMembership(TargetFaction, IsManager, FactionRank), true);

							SourcePlayer.SendNotification("Admin Action", ENotificationIcon.InfoSign, "You were added to the faction {0} as a {1} with rank {2} by {3} {4}", TargetFaction.Name, IsManager ? "Manager" : "Member", TargetFaction.GetFactionRank(FactionRank).Name, SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName));
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have added {0} to the faction {1} as a {2} with rank {3}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetFaction.Name, IsManager ? "Manager" : "Member", TargetFaction.GetFactionRank(FactionRank).Name);
						}
					}
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Rank must be between 0 and 19.");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction not found.");
			}
		}

		private void SetFactionMoney(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID factionID, int NewMoney)
		{
			CFaction TargetFaction = FactionPool.GetFactionFromID(factionID);
			if (TargetFaction != null)
			{
				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetFaction }, Helpers.FormatString("Set faction money from ${0} to ${1}", TargetFaction.Money, NewMoney)).execute();
				TargetFaction.Money = NewMoney;
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Faction '{0}' money set to ${1}", TargetFaction.Name, NewMoney);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction not found.");
			}
		}

		private void AddFactionMoney(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID factionID, int amount)
		{
			CFaction TargetFaction = FactionPool.GetFactionFromID(factionID);
			if (TargetFaction != null)
			{
				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetFaction }, Helpers.FormatString("Added ${0} faction money", amount)).execute();
				TargetFaction.Money += amount;
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Faction '{0}' had ${1} added to their faction bank.", TargetFaction.Name, amount);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction not found.");
			}
		}

		private void TakeFactionMoney(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID factionID, int amount)
		{
			CFaction TargetFaction = FactionPool.GetFactionFromID(factionID);
			if (TargetFaction != null)
			{
				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetFaction }, Helpers.FormatString("Took ${0} from their faction money", amount)).execute();
				TargetFaction.Money -= amount;
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Faction '{0}' had ${1} taken from their faction bank.", TargetFaction.Name, amount);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction not found.");
			}
		}

		private void ChangeFactionName(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID factionID, string factionShortName, string factionName)
		{
			if (factionName.Length < 5)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Full Name is too short.");
			}
			else if (factionShortName.Length < 2)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Short Name is too short.");
			}
			else if (factionName.Length > 64)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Full Name is too long.");
			}
			else if (factionShortName.Length > 4)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction Short Name is too long.");
			}

			CFaction TargetFaction = FactionPool.GetFactionFromID(factionID);
			if (TargetFaction != null)
			{
				new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetFaction }, Helpers.FormatString("Faction name was set from {0} to {1} and short name from {2} to {3}", TargetFaction.Name, factionName, TargetFaction.ShortName, factionShortName)).execute();
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, Helpers.FormatString("Faction name was set from {0} to {1} and short name from {2} to {3}", TargetFaction.Name, factionName, TargetFaction.ShortName, factionShortName));

				TargetFaction.Name = factionName;
				TargetFaction.ShortName = factionShortName;
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Faction not found.");
			}
		}
	}
}
