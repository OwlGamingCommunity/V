using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityDatabaseID = System.Int64;

public class FactionSystem : OwlScript
{
	public DutyPoints DutyPoints { get; } = new DutyPoints();
	public DutySystem DutySystem { get; } = new DutySystem();
	public EMSSystem EMSSystem { get; } = new EMSSystem();
	public FDSystem FDSystem { get; } = new FDSystem();
	public NewsSystem NewsSystem { get; } = new NewsSystem();
	public PDSystem PDSystem { get; } = new PDSystem();
	public RoadblockSystem RoadblockSystem { get; } = new RoadblockSystem();
	public SpikeStripSystem SpikeStripSystem { get; } = new SpikeStripSystem();

	public FactionSystem()
	{
		CommandManager.RegisterCommand("issuebadge", "Issues a badge to faction members", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID, int, int, int, string>(IssueBadgeCommand), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("setfactiontype", "Sets the type of a faction", new Action<CPlayer, CVehicle, EntityDatabaseID, EFactionType>(SetFactionTypeCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.FactionInviteDecision += FactionInviteDecision;

		NetworkEvents.ExitFactionMenu += OnExitFactionMenu;
		NetworkEvents.Faction_RequestFactionInfo += RequestFactionInfo;
		NetworkEvents.Faction_SetMemberRank += SetMemberRank;
		NetworkEvents.Faction_ToggleManager += ToggleManager;
		NetworkEvents.Faction_Kick += Kick;
		NetworkEvents.Faction_InvitePlayer += InvitePlayer;
		NetworkEvents.Faction_EditMessage += EditMessage;
		NetworkEvents.Faction_SaveRanksAndSalaries += SaveRanksAndSalaries;
		NetworkEvents.Faction_DisbandFaction += DisbandFaction;
		NetworkEvents.Faction_RespawnFactionVehicles += RespawnFactionVehicles;
		NetworkEvents.Faction_ViewFactionVehicles += ViewFactionVehicles;
		NetworkEvents.Faction_LeaveFaction += LeaveFaction;
		NetworkEvents.Faction_CreateFaction += CreateFaction;


		Database.Functions.Factions.LoadAllFactions((List<CDatabaseStructureFaction> lstFactions) =>
		{
			foreach (var faction in lstFactions)
			{
				CFaction loadedFaction = FactionPool.CreateFaction(faction.factionID, faction.factionType, faction.strName, faction.bOfficial, faction.strShortName, faction.strMessage, faction.fMoney, faction.lstFactionRanks, faction.CreatorID);
			}
			NAPI.Util.ConsoleOutput("[FACTIONS] Loaded {0} Factions!", lstFactions.Count);
		});
	}

	public void SetFactionTypeCommand(CPlayer player, CVehicle _, EntityDatabaseID factionID, EFactionType factionType)
	{
		CFaction faction = FactionPool.GetFactionFromID(factionID);
		faction.SetType(factionType);
		player.SendNotification("Faction Type Updated", ENotificationIcon.InfoSign, "{0} is now faction type {1}", faction.Name, faction.Type);
		Logging.Log.CreateLog(player, Logging.ELogType.FactionAction, new List<CBaseEntity> { }, Helpers.FormatString("/setfactiontype - Faction ID: {0} - New type: {1}", factionID, faction.Type));
	}

	public async void SetMemberRank(CPlayer player, int FactionIndex, int MemberIndex, int RankIndex)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;
		CFactionTransmit factionInfo = await GetFactionInfoByMembership(factionMembershipManager).ConfigureAwait(true);

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			// Get the player info
			CFactionMemberTransmit remoteFactionMember = factionInfo.Members[MemberIndex];

			if (remoteFactionMember != null)
			{
				// Is the other player a manager?
				if (remoteFactionMember.Name != player.GetCharacterName(ENameType.StaticCharacterName) && remoteFactionMember.IsManager && remoteFactionMember.Rank > factionMembershipManager.Rank)
				{
					SendFactionTransactionComplete(player);
					player.SendNotification(Helpers.FormatString("Faction Management - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "You cannot modify another manager who is higher rank then you.");
				}
				else
				{
					// Update DB
					Database.Functions.Characters.GetCharacterDBIDFromName(remoteFactionMember.Name, async (Int64 remotePlayerDBID) =>
					{
						await Database.LegacyFunctions.FactionSetMemberRank(remotePlayerDBID, factionInst.FactionID, RankIndex).ConfigureAwait(true);

						// Update faction membership instance (if online)
						WeakReference<CPlayer> remotePlayerRef = PlayerPool.GetPlayerFromName(remoteFactionMember.Name);
						CPlayer remotePlayer = remotePlayerRef.Instance();
						if (remotePlayer != null)
						{
							CFactionMembership factionMembershipRemote = remotePlayer.GetFactionMembershipFromFaction(factionInst);

							if (factionMembershipRemote != null)
							{
								factionMembershipRemote.Rank = RankIndex;
							}
						}

						// TODO: Send seperate message to sender and receiver
						factionInst.SendNotificationToAll(Helpers.FormatString("{0} set {1}'s rank to {2}", player.GetCharacterName(ENameType.StaticCharacterName), remoteFactionMember.Name, factionInst.GetFactionRank(RankIndex).Name));

						SendFactionInfoToAllPlayersInFactionAndInMenu(factionInst);
					});
				}
			}
		}
		else
		{
			SendFactionTransactionComplete(player);
			player.SendNotification(Helpers.FormatString("Faction Management - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "You are not a manager.", null);
		}
	}

	private void SendFactionTransactionComplete(CPlayer a_Player)
	{
		NetworkEventSender.SendNetworkEvent_FactionTransactionComplete(a_Player);
	}

	// TODO_FACTIONS: Make it retrieve faction info on tabbing? Could reduce BW a lot
	private void SendFactionInfoToAllPlayersInFactionAndInMenu(CFaction a_Faction)
	{
		foreach (CPlayer pPlayer in PlayerPool.GetAllPlayers())
		{
			if (pPlayer.IsInFaction(a_Faction.FactionID))
			{
				if (pPlayer.IsInFactionMenu)
				{
					RequestFactionInfo(pPlayer);
				}
			}
		}
	}

	public async void ToggleManager(CPlayer player, int FactionIndex, int MemberIndex)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;
		CFactionTransmit factionInfo = await GetFactionInfoByMembership(factionMembershipManager).ConfigureAwait(true);

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			// Get the player info
			CFactionMemberTransmit remoteFactionMember = factionInfo.Members[MemberIndex];

			if (remoteFactionMember != null)
			{
				// Is the other player a manager and same rank or higher?
				if (remoteFactionMember.Name != player.GetCharacterName(ENameType.StaticCharacterName) && remoteFactionMember.IsManager && remoteFactionMember.Rank > factionMembershipManager.Rank)
				{
					player.SendNotification(Helpers.FormatString("Faction Management - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "You cannot modify another manager who is higher rank then you.");
				}
				else
				{
					// Update DB
					Database.Functions.Characters.GetCharacterDBIDFromName(remoteFactionMember.Name, async (Int64 remotePlayerDBID) =>
					{
						bool bNewState = !remoteFactionMember.IsManager;
						await Database.LegacyFunctions.FactionSetFactionManager(remotePlayerDBID, factionInst.FactionID, bNewState).ConfigureAwait(true);

						// Update faction membership instance (if online)
						WeakReference<CPlayer> remotePlayerRef = PlayerPool.GetPlayerFromName(remoteFactionMember.Name);
						CPlayer remotePlayer = remotePlayerRef.Instance();
						if (remotePlayer != null)
						{
							CFactionMembership factionMembershipRemote = remotePlayer.GetFactionMembershipFromFaction(factionInst);

							if (factionMembershipRemote != null)
							{
								factionMembershipRemote.Manager = bNewState;
							}
						}

						// TODO: Send separate message to sender and receiver
						factionInst.SendNotificationToAll(Helpers.FormatString("{0} set {1}'s status to {2}", player.GetCharacterName(ENameType.StaticCharacterName), remoteFactionMember.Name, bNewState ? "Manager" : "Member"));

						SendFactionInfoToAllPlayersInFactionAndInMenu(factionInst);
					});
				}
			}
			else
			{
				SendFactionTransactionComplete(player);
			}
		}
		else
		{
			SendFactionTransactionComplete(player);
		}
	}

	public async void Kick(CPlayer player, int FactionIndex, int MemberIndex)
	{
		// TODO_FACTIONS: This actually fails if player had UI open, and got kicked in the mean time
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;
		CFactionTransmit factionInfo = await GetFactionInfoByMembership(factionMembershipManager).ConfigureAwait(true);

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			// Get the player info
			CFactionMemberTransmit remoteFactionMember = factionInfo.Members[MemberIndex];

			if (remoteFactionMember != null)
			{

				if (player.GetCharacterName(ENameType.StaticCharacterName) == remoteFactionMember.Name)
				{
					player.SendNotification(Helpers.FormatString("Faction Management - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "You cannot kick yourself.");
				}
				else if (remoteFactionMember.IsManager && remoteFactionMember.Rank > factionMembershipManager.Rank) // Is the other player a manager and higher rank?
				{
					player.SendNotification(Helpers.FormatString("Faction Management - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "You cannot modify another manager who is higher rank then you.");
				}
				else
				{
					// Update DB
					Database.Functions.Characters.GetCharacterDBIDFromName(remoteFactionMember.Name, async (Int64 remotePlayerDBID) =>
					{
						// Update faction membership instance (if online)
						WeakReference<CPlayer> remotePlayerRef = PlayerPool.GetPlayerFromName(remoteFactionMember.Name);
						CPlayer remotePlayer = remotePlayerRef.Instance();
						if (remotePlayer != null)
						{
							CFactionMembership factionMembershipRemote = remotePlayer.GetFactionMembershipFromFaction(factionInst);

							if (factionMembershipRemote != null)
							{
								remotePlayer.RemoveFactionMembership(factionMembershipRemote);
							}

							player.SendNotification(Helpers.FormatString("Faction Update - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "You have been kicked from the faction by {0}.", player.GetCharacterName(ENameType.StaticCharacterName));
						}

						await Database.LegacyFunctions.FactionLeaveFaction(remotePlayerDBID, factionInst.FactionID).ConfigureAwait(true);

						factionInst.SendNotificationToAllExcept(player, Helpers.FormatString("{0} kicked {1}.", player.GetCharacterName(ENameType.StaticCharacterName), remoteFactionMember.Name), remotePlayer);

						SendFactionInfoToAllPlayersInFactionAndInMenu(factionInst);
					});
				}
			}
			else
			{
				SendFactionTransactionComplete(player);
			}
		}
		else
		{
			SendFactionTransactionComplete(player);
		}
	}

	// NOTE: This doesn't require a pending transaction because it doesn't modify the faction directly. Accepting the invite does
	// TODO_FACTIONS: In future we should maybe show pending invites and let them cancel invites? If so, pending flag must be set opposite to above
	public void InvitePlayer(CPlayer player, int FactionIndex, string strPlayerName)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;

		if (player.GetCharacterName(ENameType.StaticCharacterName).ToLower() == strPlayerName.ToLower())
		{
			player.SendNotification(Helpers.FormatString("Faction Invite - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "You cannot invite yourself to a faction.");
			return;
		}

		Database.Functions.Characters.GetCharacterDBIDFromName(strPlayerName, async (Int64 remotePlayerDBID) =>
		{
			if (remotePlayerDBID >= 0)
			{
				// Pending invite check
				bool bHasPendingInviteForFaction = await Database.LegacyFunctions.DoesCharacterAlreadyHavePendingInviteForFaction(remotePlayerDBID, factionInst.m_DatabaseID).ConfigureAwait(true);
				if (bHasPendingInviteForFaction)
				{
					// already has been invited
					player.SendNotification(Helpers.FormatString("Faction Invite - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "This character already has a pending invite to this faction.");
				}
				else
				{
					// invite player
					await Database.LegacyFunctions.InvitePlayerToFaction(remotePlayerDBID, player.ActiveCharacterDatabaseID, factionInst.m_DatabaseID).ConfigureAwait(true);
					// NOTE: if player is online, send now
					WeakReference<CPlayer> remotePlayerRef = PlayerPool.GetPlayerFromName(strPlayerName);
					CPlayer remotePlayer = remotePlayerRef.Instance();

					if (remotePlayer != null)
					{
						NetworkEventSender.SendNetworkEvent_ReceivedFactionInvite(remotePlayer, factionInst.Name, player.GetCharacterName(ENameType.StaticCharacterName), factionInst.m_DatabaseID);
					}

					factionInst.SendNotificationToAll(Helpers.FormatString("{0} invited {1} to the faction.", player.GetCharacterName(ENameType.StaticCharacterName), strPlayerName));
				}
			}
			else
			{
				player.SendNotification(Helpers.FormatString("Faction Invite - {0}", factionInst.ShortName), ENotificationIcon.ExclamationSign, "Failed to invite {0} - Character Not Found", strPlayerName);
			}
		});
	}

	public void EditMessage(CPlayer player, int FactionIndex, string a_strMessage)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			// TODO: Move mysql into setter? (for this and above)
			Database.LegacyFunctions.FactionSetFactionMessage(factionInst.FactionID, a_strMessage);

			// Update faction instance
			factionInst.Message = a_strMessage;

			factionInst.SendNotificationToAll(Helpers.FormatString("{0} updated the faction message", player.GetCharacterName(ENameType.StaticCharacterName)));

			SendFactionInfoToAllPlayersInFactionAndInMenu(factionInst);
		}
		else
		{
			SendFactionTransactionComplete(player);
		}
	}

	public async void FactionInviteDecision(CPlayer player, bool bAccepted, EntityDatabaseID factionID)
	{
		CFaction Faction = FactionPool.GetFactionFromID(factionID);

		if (Faction != null)
		{
			// SECURITY: Check that we actually have a pending invite to this faction
			bool bHasPendingInviteForFaction = await Database.LegacyFunctions.DoesCharacterAlreadyHavePendingInviteForFaction(player.ActiveCharacterDatabaseID, factionID).ConfigureAwait(true);
			if (bHasPendingInviteForFaction)
			{
				if (bAccepted)
				{
					// SECURITY: Check we aren't already in this faction
					if (player.GetFactionMembershipFromFaction(Faction) != null)
					{
						player.SendNotification("Faction Invite", ENotificationIcon.ExclamationSign, "You cannot accept the invite to the faction {0} because you are already a member.", Faction.ShortName);
					}
					else
					{
						// How many factions is the person in?
						int numCurrentFactionMemberships = player.GetFactionMemberships().Count;

						if (numCurrentFactionMemberships >= FactionConstants.MaxFactionMembershipCount)
						{
							player.SendNotification("Faction Invite", ENotificationIcon.ExclamationSign, "You cannot accept the invite to the faction {0} because you are already in the maximum number of factions ({1}).", Faction.ShortName, FactionConstants.MaxFactionMembershipCount);
						}
						else
						{
							// add to faction
							player.AddFactionMembership(new CFactionMembership(Faction, false, 0), true);

							// achievement
							player.AwardAchievement(EAchievementID.JoinFaction);

							player.SendNotification("Faction Invite", ENotificationIcon.InfoSign, "You have joined the '{0}' faction.", Faction.ShortName);

							SendFactionInfoToAllPlayersInFactionAndInMenu(Faction);
						}
					}
				}
				else
				{
					SendFactionTransactionComplete(player);
				}

				// remove the pending invite
				await Database.LegacyFunctions.DeletePendingFactionInvite(player.ActiveCharacterDatabaseID, factionID).ConfigureAwait(true);
			}
		}
		else
		{
			SendFactionTransactionComplete(player);
		}
	}

	public void SaveRanksAndSalaries(CPlayer player, int FactionIndex, string a_strJSONBlob)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;

		bool bHasNameTooLong = false;

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			try
			{
				SFactionRanksUpdateStruct[] factionRanks = JsonConvert.DeserializeObject<SFactionRanksUpdateStruct[]>(a_strJSONBlob);

				// check length

				foreach (SFactionRanksUpdateStruct rank in factionRanks)
				{
					if (rank.Name.Length > 32)
					{
						bHasNameTooLong = true;
						break;
					}
				}

				if (bHasNameTooLong)
				{
					player.SendNotification("Faction Management", ENotificationIcon.ExclamationSign, "Faction rank names must be <= 32 characters in length.");
				}
				else
				{
					factionInst.UpdateFactionRanksAndSalaries(factionRanks);

					Database.LegacyFunctions.FactionSaveRanksAndSalaries(factionInst.FactionID, factionRanks);
				}
			}
			catch
			{
				// TODO_GITHUB: You should replace the below with your own website
				player.SendNotification("Faction Management", ENotificationIcon.ExclamationSign, "Unexpected error: Send a screenshot to bugs.website.com.");
				player.SendNotification("Faction Management", ENotificationIcon.ExclamationSign, a_strJSONBlob);
			}

		}

		if (!bHasNameTooLong)
		{
			SendFactionInfoToAllPlayersInFactionAndInMenu(factionInst);
		}
	}

	// TODO_POST_LAUNCH: Sending index could cause issues, send faction_id instead
	public void DisbandFaction(CPlayer player, int FactionIndex)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			if (factionInst.CanDisband())
			{
				// TODO: UI Should have a confirmation?
				factionInst.SendNotificationToAll(Helpers.FormatString("{0} disbanded the faction.", player.GetCharacterName(ENameType.StaticCharacterName)));
				DestroyFactionCompletely(factionInst);
			}
			else
			{
				player.SendNotification("Faction Management", ENotificationIcon.ExclamationSign, "This type of faction cannot be disbanded.");
			}
		}
	}

	public void RespawnFactionVehicles(CPlayer player, int FactionIndex)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			// TODO: UI Should have a confirmation?
			factionInst.SendNotificationToAll(Helpers.FormatString("{0} respawned the faction vehicles.", player.GetCharacterName(ENameType.StaticCharacterName)));
			factionInst.RespawnVehicles();
		}
	}

	public void ViewFactionVehicles(CPlayer player, int FactionIndex)
	{
		CFactionMembership factionMembershipManager = player.GetFactionMemberships()[FactionIndex];
		CFaction factionInst = factionMembershipManager.Faction;

		// Is the requesting player a manager?
		if (factionMembershipManager.Manager)
		{
			List<CVehicle> lstFactionVehicles = VehiclePool.GetVehiclesFromFaction(factionInst);

			List<CFactionVehicleInfoTransmit> lstTransmit = new List<CFactionVehicleInfoTransmit>();
			foreach (CVehicle factionVeh in lstFactionVehicles)
			{
				lstTransmit.Add(new CFactionVehicleInfoTransmit(factionVeh.m_DatabaseID, factionVeh.GetModelHash(), factionVeh.GetPlateText(true), factionVeh.GTAInstance.Position.X, factionVeh.GTAInstance.Position.Y, factionVeh.GTAInstance.Position.Z));
			}

			NetworkEventSender.SendNetworkEvent_Faction_ViewFactionVehicles_Response(player, lstTransmit);
		}
	}

	private void DestroyFactionCompletely(CFaction a_Faction)
	{
		if (a_Faction.Type == EFactionType.UserCreated)
		{
			FactionPool.DestroyFaction(a_Faction);
			SendFactionInfoToAllPlayersInFactionAndInMenu(a_Faction);
		}
	}

	public async void LeaveFaction(CPlayer player, int FactionIndex)
	{
		CFactionMembership factionMembership = player.GetFactionMemberships()[FactionIndex];
		int numMembers = await factionMembership.Faction.GetTotalNumMembers().ConfigureAwait(true);
		// We check for amount of members before leaving. If we need to destroy the faction members will be removed automatically. Should be in this order.
		if (numMembers == 1)
		{
			if (factionMembership.Faction.CanDisband())
			{
				DestroyFactionCompletely(factionMembership.Faction);
			}
		}

		await Database.LegacyFunctions.FactionLeaveFaction(player.ActiveCharacterDatabaseID, factionMembership.Faction.FactionID).ConfigureAwait(true);

		// Update faction membership instance
		player.RemoveFactionMembership(factionMembership);

		factionMembership.Faction.SendNotificationToAllExcept(player, Helpers.FormatString("{0} left the faction.", player.GetCharacterName(ENameType.StaticCharacterName)), player);
		player.SendNotification(Helpers.FormatString("Faction Management - {0}", factionMembership.Faction.ShortName), ENotificationIcon.ExclamationSign, "You have left the faction.", null);

		SendFactionInfoToAllPlayersInFactionAndInMenu(factionMembership.Faction);
	}

	public async void CreateFaction(CPlayer player, string strFullName, string strShortName, string strFactionType)
	{
		ECreateFactionResult createFactionError = ECreateFactionResult.Success;

		if (strFullName.Length < 5)
		{
			createFactionError = ECreateFactionResult.FullNameTooShort;
		}
		else if (strShortName.Length < 2)
		{
			createFactionError = ECreateFactionResult.ShortNameTooShort;
		}
		else if (strFullName.Length > 50)
		{
			createFactionError = ECreateFactionResult.FullNameTooLong;
		}
		else if (strShortName.Length > 4)
		{
			createFactionError = ECreateFactionResult.ShortNameTooLong;
		}
		else
		{
			// Check names are unique
			Database.LegacyFunctions.EFactionNameUniqueResult FactionNameUniqueRes = await Database.LegacyFunctions.IsFactionNameUnique(strFullName, strShortName).ConfigureAwait(true);
			if (FactionNameUniqueRes == Database.LegacyFunctions.EFactionNameUniqueResult.IsUnique)
			{
				createFactionError = ECreateFactionResult.Success;
				player.AwardAchievement(EAchievementID.CreateFaction);

				player.SendNotification("Faction Created", ENotificationIcon.InfoSign, "You have created your faction '{0}'", strFullName);

				EFactionType factionType = strFactionType == "illegal"
					? EFactionType.UserCreatedCriminal
					: EFactionType.UserCreated;

				CDatabaseStructureFaction faction = await Database.LegacyFunctions.CreateFaction(player.ActiveCharacterDatabaseID, strFullName, strShortName, factionType, false).ConfigureAwait(true);

				CFaction loadedFaction = FactionPool.CreateFaction(faction.factionID, faction.factionType, faction.strName, faction.bOfficial, faction.strShortName, faction.strMessage, faction.fMoney, faction.lstFactionRanks, faction.CreatorID);
				player.AddFactionMembership(new CFactionMembership(loadedFaction, true, 0), true);

				SendFactionInfoToAllPlayersInFactionAndInMenu(loadedFaction);
			}
			else if (FactionNameUniqueRes == Database.LegacyFunctions.EFactionNameUniqueResult.FullNameTaken)
			{
				createFactionError = ECreateFactionResult.FullNameTaken;
			}
			else if (FactionNameUniqueRes == Database.LegacyFunctions.EFactionNameUniqueResult.ShortNameTaken)
			{
				createFactionError = ECreateFactionResult.ShortNameTaken;
			}
		}

		NetworkEventSender.SendNetworkEvent_FactionCreateResult(player, createFactionError);
	}


	private async Task<CFactionTransmit> GetFactionInfoByMembership(CFactionMembership factionMembership)
	{
		CFactionTransmit factionToTransmit = new CFactionTransmit(factionMembership.Faction.FactionID, factionMembership.Faction.ShortName, factionMembership.Faction.Name, factionMembership.Faction.Money, factionMembership.Faction.Message, factionMembership.Manager);

		if (factionMembership.Faction.IsOfficial)
		{
			factionToTransmit.AddTag("Official");
		}

		if (factionMembership.Faction.Type == EFactionType.LawEnforcement)
		{
			factionToTransmit.AddTag("Law Enforcement");
		}

		if (factionMembership.Faction.Type == EFactionType.Medical)
		{
			factionToTransmit.AddTag("Medical Services");
		}

		if (factionMembership.Faction.Type == EFactionType.Criminal)
		{
			factionToTransmit.AddTag("Crime Faction");
		}

		if (factionMembership.Faction.Type == EFactionType.NewsFaction)
		{
			factionToTransmit.AddTag("News Agency");
		}

		foreach (CFactionRank factionRank in factionMembership.Faction.FactionRanks)
		{
			factionToTransmit.AddRank(new CFactionRankTransmit(factionRank.Name, factionRank.Salary));
		}

		// Get members
		Task<SGetFactionMembers> RetrieveMembersTask = Database.LegacyFunctions.GetFactionMembers(factionMembership.Faction.FactionID);

		SGetFactionMembers result = await RetrieveMembersTask.ConfigureAwait(true);

		foreach (SGetFactionMemberInst memberInst in result.lstMembers)
		{
			CFactionMemberTransmit newMember = new CFactionMemberTransmit(memberInst.Name, memberInst.Rank, memberInst.IsManager, memberInst.LastSeen);
			factionToTransmit.AddMember(newMember);
		}

		return factionToTransmit;
	}

	public void OnExitFactionMenu(CPlayer player)
	{
		player.IsInFactionMenu = false;
	}

	public async void RequestFactionInfo(CPlayer player)
	{
		player.IsInFactionMenu = true;

		// TODO: Cache factions until ui hide/signoff?
		List<CFactionTransmit> lstFactionsToTransmit = new List<CFactionTransmit>();

		if (player != null)
		{
			foreach (CFactionMembership factionMembership in player.GetFactionMemberships())
			{
				CFactionTransmit factionToTransmit = await GetFactionInfoByMembership(factionMembership).ConfigureAwait(true);
				lstFactionsToTransmit.Add(factionToTransmit);
			}

			NetworkEventSender.SendNetworkEvent_RequestFactionInfo_Response(player, lstFactionsToTransmit);
		}
	}

	public void IssueBadgeCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EntityDatabaseID factionID, int r, int g, int b, string badgeValue)
	{
		CFaction targetFaction = FactionPool.GetFactionFromID(factionID);
		if (targetFaction == null)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, Helpers.FormatString("The faction doesn't exist"));
			return;
		}

		if (!targetFaction.IsOfficial)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, Helpers.FormatString("The faction is not an official faction"));
			return;
		}

		if (badgeValue.Length > 50)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, Helpers.FormatString("Badge value too long. Maximum 50 characters allowed."));
			return;
		}

		if (SenderPlayer.IsFactionManager(factionID) || SenderPlayer.IsAdmin(EAdminLevel.Admin, true))
		{
			System.Drawing.Color color = System.Drawing.Color.FromArgb(r, g, b);

			// Check if it's a official govt faction with colors defined
			if (g_dictBlockedColors.ContainsKey(targetFaction.ShortName))
			{
				// overwrite color because it's an official govt faction.
				color = g_dictBlockedColors[targetFaction.ShortName];
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 255, 255, Helpers.FormatString("Because you are in '{0}' we have set your badge color to their standard.", Helpers.ColorString(0, 255, 0, "{0}", targetFaction.Name)));
			}
			else if (g_dictBlockedColors.ContainsValue(color))
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 255, 255, Helpers.FormatString("This is a reserved or a blocked color. Please change the color"));
				return;
			}
			else
			{
				//We have to go all through all of the colors and verify they aren't close to the blocked colors.
				foreach (var c in g_dictBlockedColors.Values)
				{
					if (Helpers.AreColorsClose(c, color, 100))
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 255, 255, Helpers.FormatString("Your color is too close to a reserved color. Please use a different color."));
						return;
					}
				}
			}

			CItemValueBadge badgeItemVal = new CItemValueBadge(targetFaction.ShortName, badgeValue, color, false, -1);
			CItemInstanceDef itemDef = CItemInstanceDef.FromTypedObjectNoDBID(EItemID.BADGE, badgeItemVal, 1);
			TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, null);

			SenderPlayer.SendNotification("Badge", ENotificationIcon.ExclamationSign, "Successfully given a badge to {0}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			TargetPlayer.SendNotification("Badge", ENotificationIcon.ExclamationSign, "Received a badge from {0}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
			Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.FactionAction, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/issuebadge - Faction ID: {0} - Badge value: {1} ", factionID, badgeValue));
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You have to be manager of the faction to be able to issue a badge");
		}
	}

	private readonly Dictionary<string, System.Drawing.Color> g_dictBlockedColors = new Dictionary<string, System.Drawing.Color>()
	{
		{ "LSPD", System.Drawing.Color.FromArgb(0, 100, 255) },
		{ "EMS", System.Drawing.Color.FromArgb(175, 50, 50) },
		{ "CAA", System.Drawing.Color.FromArgb(9, 31, 98) },
		{ "GOVT", System.Drawing.Color.FromArgb(0, 80, 0) },
		{ "LSCTS", System.Drawing.Color.FromArgb(255, 136, 0) },
		{ "PW", System.Drawing.Color.FromArgb(255, 136, 0) },
		{ "SEMC", System.Drawing.Color.FromArgb(6, 129, 121) },
		{ "UATColor", System.Drawing.Color.FromArgb(14, 194, 255) },
		{ "GATColor", System.Drawing.Color.FromArgb(255, 194, 14) },
		{ "NormalColor", System.Drawing.Color.FromArgb(255, 255, 255) },
	};
}

