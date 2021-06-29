using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class PDSystem
{
	public PDSystem()
	{
		// COMMANDS
		CommandManager.RegisterCommand("togsirens", "Toggles strobes without sound", new Action<CPlayer, CVehicle>(ToggleSirenCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default | CommandRequirementsFlags.MustBeInVehicle);
		// CommandManager.RegisterCommand("bailout", "Bails out of jail", new Action<CPlayer, CVehicle>(BailOutCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("arrest", "Arrests a person", new Action<CPlayer, CVehicle, CPlayer, int, int, int, float, string>(ArrestCommand), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.Default, overrideSyntax: "[ID or (Partial) Name] [Days] [Hours] [Minutes] [Fine amount (0 for none)] [Reason]");
		CommandManager.RegisterCommand("setfree", "Releases someone from jail manually. (Teleports the player to the officer that performs the command)", new Action<CPlayer, CVehicle, CPlayer>(SetFreeCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("ticket", "Tickets a person", new Action<CPlayer, CVehicle, CPlayer, float, string>(TicketCommand), CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("mdtveh", "Runs an MDT check on a vehicle", new Action<CPlayer, CVehicle, string>(OpenVehicleMDT), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default | CommandRequirementsFlags.MustBeInVehicle);
		CommandManager.RegisterCommand("mdtproperty", "Runs an MDT check on a property", new Action<CPlayer, CVehicle, string>(OpenPropertyMDT), CommandParsingFlags.Default, CommandRequirementsFlags.Default | CommandRequirementsFlags.MustBeInVehicle, aliases: new string[] { "mdtprop" });
		CommandManager.RegisterCommand("mdtperson", "Runs an MDT check on a person", new Action<CPlayer, CVehicle, string>(OpenPersonMDT), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default | CommandRequirementsFlags.MustBeInVehicle);
		CommandManager.RegisterCommand("acc911", "Responds to a 911 call", new Action<CPlayer, CVehicle, int>(Accept911), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("cancel911", "Cancels your response to a 911 call", new Action<CPlayer, CVehicle, int>(Cancel911), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("gooc", "Sends an OOC message to government factions", new Action<CPlayer, CVehicle, string>(SendGovtFactionsOOCMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("backup", "Toggles backup beacon", new Action<CPlayer, CVehicle>(ToggleBackupBeaconCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("towtruck", "Toggles backup beacon", new Action<CPlayer, CVehicle>(ToggleTowtruckBeaconCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("setunitnumber", "Changes your unit number", new Action<CPlayer, CVehicle, int>(ChangeUnitNumberCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("911", "Makes a 911 call", new Action<CPlayer, CVehicle, string>(NineOneOneCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.RequestPlateRun += RequestPlateRun;
		NetworkEvents.ToggleSirenMode += TogSirensEvent;
		NetworkEvents.TicketResponse += OnTicketResponse;

		NetworkEvents.MdcGotoPerson += RequestGotoPerson;
		NetworkEvents.MdcGotoProperty += RequestGotoProperty;
		NetworkEvents.MdcGotoVehicle += RequestGotoVehicle;

		NetworkEvents.ANPR_GetSpeed += RequestVehicleSpeed;

		NetworkEvents.BlipSiren_Request += BlipSirenRequest;

		NetworkEvents.FinalizeLicenseDevice += OnFinalizeLicenseDevice;

		NetworkEvents.MoveToRappelPosition += OnMoveToRappelPosition;

		NetworkEvents.SpeedCameraTrigger += OnSpeedCameraTrigger;

		CommandManager.RegisterCommand("speech", "Plays a custom speech event (synchronized)", new Action<CPlayer, CVehicle, int, int>(PlayCustomSpeech), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
	}

	private void OnMoveToRappelPosition(CPlayer player, int seat)
	{
		if (player.IsInVehicleReal)
		{
			player.Client.SetIntoVehicle(player.Client.Vehicle.Handle, seat);
		}
	}

	private void OnSpeedCameraTrigger(CPlayer player, float fSpeed, int speedLimit, string strName, int cameraID)
	{
		CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
		if (vehicle != null)
		{
			if (player.Client.VehicleSeat == 0)
			{
				bool bTriggered = fSpeed > speedLimit;
				NetworkEventSender.SendNetworkEvent_SpeedCameraTrigger_Response_ForAll_IncludeEveryone(fSpeed, cameraID);

				// alert?
				if (bTriggered)
				{
					SendPDFactionMessage(Helpers.FormatString("[SPEED CAMERA] {0} (Plate: {1}) triggered Camera '{2}'. Speed was {3} in a {4} zone.", vehicle.GetFullDisplayName(), vehicle.GetPlateText(false), strName, (int)Math.Ceiling(fSpeed), speedLimit), false);

					player.AwardAchievement(EAchievementID.SpeedCamera);
				}
			}
		}
	}

	private void BlipSirenRequest(CPlayer player)
	{
		CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
		if (vehicle != null)
		{
			if (vehicle.HasSiren())
			{
				NetworkEventSender.SendNetworkEvent_BlipSiren_Response_ForAll_IncludeEveryone(vehicle.GTAInstance);
			}
		}
	}

	private async void OnFinalizeLicenseDevice(CPlayer player, string strTargetName, EWeaponLicenseType weaponLicenseType, bool a_bIsRemoval)
	{
		if (player.IsFactionManager(FactionPool.GetPoliceFaction().FactionID))
		{
			// Does target player exist?
			SVerifyCharacterExists Result = await Database.LegacyFunctions.VerifyCharacterExists(strTargetName).ConfigureAwait(true);
			// Does the character exist?
			if (Result.CharacterExists)
			{
				// Does this character belong to our account?
				if (Result.AccountID != player.AccountID)
				{
					string strLicenseTypeName = weaponLicenseType == EWeaponLicenseType.Tier1 ? "Tier 1" : "Tier 2";

					if (!a_bIsRemoval)
					{
						player.SendNotification("Firearms Licensing Device", ENotificationIcon.Star, Helpers.FormatString("'{0}' was granted a {1} license. They can pick it up from the SO Front Desk.", strTargetName, strLicenseTypeName));

						new Logging.Log(player, Logging.ELogType.WeaponLicense, null, Helpers.FormatString("PD Manager {0} approved a {1} license for {2}.", player.GetCharacterName(ENameType.StaticCharacterName), strLicenseTypeName, strTargetName)).execute();

						EPendingFirearmLicenseState tier1LicenseStateChange = EPendingFirearmLicenseState.None;
						EPendingFirearmLicenseState tier2LicenseStateChange = EPendingFirearmLicenseState.None;
						if (weaponLicenseType == EWeaponLicenseType.Tier1)
						{
							Database.Functions.Characters.SetTier1PendingLicenseState(Result.CharacterID, EPendingFirearmLicenseState.Issued_PendingPickup);
							tier1LicenseStateChange = EPendingFirearmLicenseState.Issued_PendingPickup;
						}
						else if (weaponLicenseType == EWeaponLicenseType.Tier2)
						{
							Database.Functions.Characters.SetTier2PendingLicenseState(Result.CharacterID, EPendingFirearmLicenseState.Issued_PendingPickup);
							tier2LicenseStateChange = EPendingFirearmLicenseState.Issued_PendingPickup;
						}

						// if the player is online, handle the state immediately
						WeakReference<CPlayer> targetPlayerRef = PlayerPool.GetPlayerFromName(strTargetName);
						CPlayer targetPlayer = targetPlayerRef.Instance();

						if (targetPlayer != null)
						{
							targetPlayer.HandlePendingWeaponLicenseStates(tier1LicenseStateChange, tier2LicenseStateChange);
						}
					}
					else
					{
						EPendingFirearmLicenseState tier1LicenseStateChange = EPendingFirearmLicenseState.None;
						EPendingFirearmLicenseState tier2LicenseStateChange = EPendingFirearmLicenseState.None;
						if (weaponLicenseType == EWeaponLicenseType.Tier1)
						{
							tier1LicenseStateChange = EPendingFirearmLicenseState.Revoked;
							Database.Functions.Characters.SetTier1PendingLicenseState(Result.CharacterID, EPendingFirearmLicenseState.Revoked);
						}
						else if (weaponLicenseType == EWeaponLicenseType.Tier2)
						{
							tier2LicenseStateChange = EPendingFirearmLicenseState.Revoked;
							Database.Functions.Characters.SetTier2PendingLicenseState(Result.CharacterID, EPendingFirearmLicenseState.Revoked);
						}

						// if the player is online, handle the state immediately
						WeakReference<CPlayer> targetPlayerRef = PlayerPool.GetPlayerFromName(strTargetName);
						CPlayer targetPlayer = targetPlayerRef.Instance();

						if (targetPlayer != null)
						{
							targetPlayer.HandlePendingWeaponLicenseStates(tier1LicenseStateChange, tier2LicenseStateChange);
						}

						// update in DB, this is fine without checking if they actually have the item, on spawn we check for the item before notifying them they lost it
						player.SendNotification("Firearms Licensing Device", ENotificationIcon.Star, Helpers.FormatString("'{0}' had their {1} license revoked. If they have one, they will be informed immediately if online, or in person upon next spawn.", strTargetName, strLicenseTypeName));
						new Logging.Log(player, Logging.ELogType.WeaponLicense, null, Helpers.FormatString("PD Manager {0} removed a {1} license from {2}.", player.GetCharacterName(ENameType.StaticCharacterName), strLicenseTypeName, strTargetName)).execute();
					}
				}
				else
				{
					player.SendNotification("Firearms Licensing Device", ENotificationIcon.ExclamationSign, "You cannot grant a license to a character which belongs to your own account.");
				}
			}
			else
			{
				player.SendNotification("Firearms Licensing Device", ENotificationIcon.ExclamationSign, Helpers.FormatString("No character was found with the name '{0}'.", strTargetName));
			}
		}
		else
		{
			player.SendNotification("Firearms Licensing Device", ENotificationIcon.ExclamationSign, "You must be a manager in the LE faction to use the weapon licensing device.");
		}
	}

	private void PlayCustomSpeech(CPlayer SenderPlayer, CVehicle vehicle, int speechID, int speechType)
	{
		ESpeechID speechIDReal = (ESpeechID)speechID;
		ESpeechType speechTypeReal = (ESpeechType)speechType;

		if (SenderPlayer.IsInFactionOfType(EFactionType.LawEnforcement))
		{
			// TODO_POST_LAUNCH: Check player actually has a megaphone
			// TODO_POST_LAUNCH: Add a cooldown? or only transmit to nearby players
			float fDist = 0.0f;
			if (speechTypeReal == ESpeechType.SPEECH_PARAMS_NORMAL)
			{
				fDist = ChatConstants.g_fDistance_Nearby;
			}
			else if (speechTypeReal == ESpeechType.SPEECH_PARAMS_SHOUTED)
			{
				fDist = ChatConstants.g_fDistance_ShoutLoudly;
			}
			else if (speechTypeReal == ESpeechType.SPEECH_PARAMS_MEGAPHONE)
			{
				fDist = ChatConstants.g_fDistance_Megaphone;
			}

			List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, fDist);

			foreach (CPlayer player in lstPlayers)
			{
				NetworkEventSender.SendNetworkEvent_PlayCustomSpeech(player, SenderPlayer.Client, speechIDReal, speechTypeReal);
			}
		}
		else
		{
			SenderPlayer.SendNotification("Custom Speech", ENotificationIcon.ExclamationSign, "You must be in a Law Enforcement faction to perform this action.", null);
		}
	}

	private class CTicketInstance
	{
		public CTicketInstance(CPlayer a_TicketingOfficer, CPlayer a_TicketedPlayer, float a_fAmount, string a_strReason)
		{
			m_TicketingOfficer.SetTarget(a_TicketingOfficer);
			m_TicketedPlayer.SetTarget(a_TicketedPlayer);
			m_fAmount = a_fAmount;
			m_strReason = a_strReason;
		}

		public WeakReference<CPlayer> m_TicketingOfficer = new WeakReference<CPlayer>(null);
		public WeakReference<CPlayer> m_TicketedPlayer = new WeakReference<CPlayer>(null);
		public float m_fAmount;
		public string m_strReason;
	}

	private List<CTicketInstance> m_lstTicketedPlayerInstances = new List<CTicketInstance>();

	public void TogSirensEvent(CPlayer player)
	{
		CommandManager.TriggerCommandManual(player, "togsirens");
	}

	public void ToggleBackupBeaconCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderPlayer.IsInFactionOfType(EFactionType.LawEnforcement) || SenderPlayer.IsInFactionOfType(EFactionType.Medical))
		{
			if (!SenderPlayer.IsBackupBeaconActive())
			{
				SenderPlayer.SetBackupBeacon();

				SendGovtFactionsMessage(Helpers.FormatString("[EMERGENCY] Unit {0} ({1}) has activated their backup beacon!", SenderPlayer.GetUnitNumber(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName)), false);
				SenderPlayer.SendNotification("Backup Beacon", ENotificationIcon.InfoSign, "Your backup beacon is now activated.", null);
			}
			else
			{
				SenderPlayer.ClearBackupBeacon();

				SendGovtFactionsMessage(Helpers.FormatString("[EMERGENCY] Unit {0} ({1}) has deactivated their backup beacon!", SenderPlayer.GetUnitNumber(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName)), false);
				SenderPlayer.SendNotification("Backup Beacon", ENotificationIcon.InfoSign, "Your backup beacon is now deactivated.", null);
			}
		}
		else
		{
			SenderPlayer.SendNotification("Backup Beacon", ENotificationIcon.ExclamationSign, "You must be in a Law Enforcement or FD EMS faction to perform this action.", null);
		}
	}

	public void ToggleTowtruckBeaconCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (!SenderPlayer.IsTowtruckBeaconActive())
		{
			SenderPlayer.SetTowtruckBeacon();

			SendTowingFactionsMessage("[TOWING] Someone has requested a towtruck at their location.", false);
			SenderPlayer.SendNotification("Towing Request", ENotificationIcon.InfoSign, "Your location has been sent to the available tow trucks.", null);
		}
		else
		{
			SenderPlayer.ClearTowtruckBeacon();

			SenderPlayer.SendNotification("Towing Request", ENotificationIcon.InfoSign, "Your towing request has been cancelled.", null);
		}
	}

	public void ChangeUnitNumberCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, int unitNumber)
	{
		if (SenderPlayer.IsOnDuty())
		{
			if (unitNumber >= 0 || unitNumber <= 99)
			{
				SenderPlayer.SetUnitNumber(unitNumber);
				SenderPlayer.SendNotification("Unit Number", ENotificationIcon.InfoSign, "Your unit number was changed to {0}.", unitNumber);
			}
			else
			{
				SenderPlayer.SendNotification("Unit Number", ENotificationIcon.InfoSign, "Your unit number must be between 0 and 99.");
			}
		}
	}

	public void ToggleSirenCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (!SenderPlayer.IsInFactionOfType(EFactionType.LawEnforcement) &&
			!SenderPlayer.IsInFactionOfType(EFactionType.Medical))
		{
			return;
		}

		if (SenderPlayer.Client.VehicleSeat != (int)EVehicleSeat.Driver)
		{
			return;
		}

		if (!SenderVehicle.HasSiren())
		{
			return;
		}

		bool bCurrentState = SenderVehicle.GetData<bool>(SenderVehicle.GTAInstance, EDataNames.SIREN_STATE);
		SenderVehicle.SetData(SenderVehicle.GTAInstance, EDataNames.SIREN_STATE, !bCurrentState, EDataType.Synced);
		SenderPlayer.SendNotification("Siren Management", ENotificationIcon.InfoSign, "You have set your sirens to {0}", !bCurrentState ? "silent" : "loud");
	}

	public void BailOutCommand(CPlayer player, CVehicle SenderVehicle)
	{
		if (player != null)
		{
			if (player.IsJailed())
			{
				float fBailoutAmount = player.GetBailoutCost();
				if (fBailoutAmount > 0.0f)
				{
					if (player.SubtractMoney(fBailoutAmount, PlayerMoneyModificationReason.BailOut) || player.SubtractBankBalanceIfCanAfford(fBailoutAmount, PlayerMoneyModificationReason.BailOut))
					{
						// TODO_CHAT: Notifications
						player.PushChatMessageWithColor(EChatChannel.Notifications, 0, 255, 0, "You have bailed out of jail for ${0:0.00}", fBailoutAmount);
						player.Unjail();

						player.AwardAchievement(EAchievementID.BailOut);

						CFaction Faction = FactionPool.GetPoliceFaction();
						Faction.Money += fBailoutAmount;
					}
				}
				else
				{
					player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "The arresting officer did not give you a bail out for this crime.");
				}
			}
			else
			{
				player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "You are not currently in jail.");
			}
		}
	}

	public void ArrestCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, int Days, int Hours, int Minutes, float fineAmount, string reason)
	{
		if (TargetPlayer.BankMoney < fineAmount)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "Player does not have that much money to fine.");
			return;
		}

		// hardcoded pos for mission row
		Vector3 vecArrestPos = new Vector3(459.723022f, -989.058533f, 24.914858f);

		// check for hardcoded custom interior overrides
		if (SenderPlayer.Client.Dimension != 0)
		{
			CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(SenderPlayer.Client.Dimension);
			if (propInst != null)
			{
				// NOTE: All custom ints need one arresting station
				// check for custom PD interiors
				if (propInst.Model.InteriorId == PDConstants.VSPDInteriorID)
				{
					vecArrestPos = new Vector3(1489.3265, 3201.0188, 47.57256);
				}
			}
		}

		// Are we near the arrest point?
		if (SenderPlayer.IsWithinDistanceOf(vecArrestPos, 5.0f, 0, false))
		{
			// Are we in PD + on duty?
			if (!SenderPlayer.IsInFactionOfType(EFactionType.LawEnforcement))
			{
				// TODO_CHAT: Notifications
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You must be in a Law Enforcement faction.");
			}
			// else if (SenderPlayer == TargetPlayer)
			// {
			// 	SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You cannot arrest yourself.");
			// }
			else
			{
				if (!SenderPlayer.IsOnDutyOfType(EDutyType.Law_Enforcement))
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You must be on duty.");
				}
				else
				{
					if (fineAmount < 0)
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You must provide a valid number for the fine amount.");
					}
					else if (Days < 0)
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: Days must be >= 0");
					}
					else if (Hours < 0)
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: Hours must be >= 0");
					}
					else if (Minutes < 0)
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: Minutes must be >= 0");
					}
					else
					{
						// Are we within range?
						if (SenderPlayer.IsWithinDistanceOf(TargetPlayer, 5.0f))
						{
							// Give back handcuffs to the officer
							if (TargetPlayer.IsCuffed())
							{
								TargetPlayer.Uncuff(SenderPlayer);
							}

							TargetPlayer.RemoveBankMoney(fineAmount, PlayerMoneyModificationReason.ArrestFine);
							TargetPlayer.Jail(Days, Hours, Minutes, reason, GetNextPrisonCell());

							TargetPlayer.AwardAchievement(EAchievementID.BeJailed);
							SenderPlayer.AwardAchievement(EAchievementID.ArrestSomeone);

							// TODO_CHAT: Notification
							SenderPlayer.PushChatMessage(EChatChannel.Notifications, "You have arrested {0} for {1} Days, {2} Hours and {3} Minutes for '{4}'. {5}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Days, Hours, Minutes, reason, fineAmount > 0.0 ? Helpers.FormatString("Their fine is ${0:0.00}.", fineAmount) : "");
							TargetPlayer.PushChatMessage(EChatChannel.Notifications, "You have been arrested by {0} for {1} Days, {2} Hours and {3} Minutes for '{4}'. {5}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), Days, Hours, Minutes, reason, fineAmount > 0.0 ? Helpers.FormatString("Your fine is ${0:0.00}.", fineAmount) : "");
							new Logging.Log(SenderPlayer, Logging.ELogType.FactionAction, null, Helpers.FormatString("/arrest - arrested {0}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
						}
						else
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: {0} is too far away.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
						}
					}
				}
			}
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You are too far away from the jail.");
		}
	}

	private void SetFreeCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer)
	{
		// Are we in PD + on duty?
		if (!SenderPlayer.IsInFactionOfType(EFactionType.LawEnforcement))
		{
			// TODO_CHAT: Notifications
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You must be in a Law Enforcement faction.");
		}
		// else if (SenderPlayer == TargetPlayer)
		// {
		// 	SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You cannot release yourself.");
		// }
		else
		{
			if (!SenderPlayer.IsOnDutyOfType(EDutyType.Law_Enforcement))
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: You must be on duty.");
			}
			else
			{
				// Are we within range?
				if (SenderPlayer.IsWithinDistanceOf(TargetPlayer, 5.0f))
				{
					SenderPlayer.ReleaseFromJail(TargetPlayer);
					// TODO_CHAT: Notification
					SenderPlayer.PushChatMessage(EChatChannel.Notifications, "You have released {0} from jail.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
					TargetPlayer.PushChatMessage(EChatChannel.Notifications, "You have been released from jail by {0}.", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
					new Logging.Log(SenderPlayer, Logging.ELogType.FactionAction, null, Helpers.FormatString("/setfree - released {0} from jail manually", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
				}
				else
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "ARREST: {0} is too far away.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
				}
			}
		}
	}

	private EPrisonCell m_NextPrisonCell = EPrisonCell.One;

	private EPrisonCell GetNextPrisonCell()
	{
		EPrisonCell PrisonCell = m_NextPrisonCell;

		if (m_NextPrisonCell == EPrisonCell.One)
		{
			m_NextPrisonCell = EPrisonCell.Two;
		}
		else if (m_NextPrisonCell == EPrisonCell.Two)
		{
			m_NextPrisonCell = EPrisonCell.Three;
		}
		else if (m_NextPrisonCell == EPrisonCell.Three)
		{
			m_NextPrisonCell = EPrisonCell.One;
		}

		return PrisonCell;
	}

	private CTicketInstance GetTicketInstanceFromTicketedPlayer(CPlayer TicketedPlayer)
	{
		foreach (var ticketInstance in m_lstTicketedPlayerInstances)
		{
			if (ticketInstance.m_TicketedPlayer.Instance() == TicketedPlayer)
			{
				return ticketInstance;
			}
		}

		return null;
	}

	public void TicketCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float amount, string reason)
	{
		// Are we in PD + on duty?
		if (!SenderPlayer.IsInFactionOfType(EFactionType.LawEnforcement))
		{
			// Do nothing
			return;
		}
		else
		{
			if (!SenderPlayer.IsOnDutyOfType(EDutyType.Law_Enforcement))
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TICKET: You must be on duty.");
			}
			else
			{
				if (reason.Length == 0)
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TICKET: You must provide a reason.");
				}
				else
				{
					// Are we within range?
					if (SenderPlayer.IsWithinDistanceOf(TargetPlayer, 5.0f))
					{
						var existingTicket = GetTicketInstanceFromTicketedPlayer(TargetPlayer);

						if (existingTicket != null)
						{
							SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TICKET: {0} already has another pending ticket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
						}
						else
						{
							// Make a note of the ticket instance
							m_lstTicketedPlayerInstances.Add(new CTicketInstance(SenderPlayer, TargetPlayer, amount, reason));

							SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "You have requested to ticket {0} ${1:0.00} for '{2}'. Please wait for their response.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), amount, reason);
							NetworkEventSender.SendNetworkEvent_RequestTicket(TargetPlayer, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), amount, reason);
						}
					}
					else
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TICKET: {0} is too far away.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
					}
				}
			}
		}
	}

	// Event for clientside request for MDT vehicle (used by PD HUD)
	public void RequestPlateRun(CPlayer player, Vehicle vehicle)
	{
		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);
		if (pVehicle != null)
		{
			OpenVehicleMDT(player, null, pVehicle.GTAInstance.NumberPlate);
		}
	}

	public void RequestGotoProperty(CPlayer player, EntityDatabaseID propertyID)
	{
		OpenPropertyMDT(player, null, propertyID.ToString());
	}

	public void RequestGotoVehicle(CPlayer player, EntityDatabaseID vehicleID)
	{
		CVehicle vehicle = VehiclePool.GetVehicleFromID(vehicleID);
		if (vehicle != null)
		{
			OpenVehicleMDT(player, null, vehicle.GTAInstance.NumberPlate);
		}
	}

	public void RequestVehicleSpeed(CPlayer player, Vehicle vehicle)
	{
		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);
		if (pVehicle != null)
		{
			Vector3 vecVel = NAPI.Entity.GetEntityVelocity(pVehicle.GTAInstance.Handle);
			Vector3 vecSquared = new Vector3(vecVel.X * vecVel.X, vecVel.Y * vecVel.Y, vecVel.Z * vecVel.Z);
			float fSpeed = (float)Math.Sqrt((double)vecSquared.Length());
			NetworkEventSender.SendNetworkEvent_ANPR_GotSpeed(player, fSpeed);
		}
	}

	public void RequestGotoPerson(CPlayer player, string strName)
	{
		OpenPersonMDT(player, null, strName);
	}

	public async void OpenVehicleMDT(CPlayer SendingPlayer, CVehicle SenderVehicle, string plate)
	{
		CFaction courtFaction = FactionPool.GetFactionFromShortName("SCSA");
		if ((!SendingPlayer.IsInFactionOfType(EFactionType.LawEnforcement) || !SendingPlayer.IsOnDutyOfType(EDutyType.Law_Enforcement)) && !SendingPlayer.IsInFaction(courtFaction.FactionID))
		{
			SendingPlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "You must be on duty as Law Enforcement to perform this action.");
			return;
		}

		CVehicle pVehicle = VehiclePool.GetVehicleFromPlate(plate);

		if (pVehicle != null)
		{
			CMdtVehicle vehicle = await Database.LegacyFunctions.GetVehicleByPlate(pVehicle.VehicleType, plate).ConfigureAwait(true);
			if (!vehicle.found)
			{
				SendingPlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "Plate '{0}' could not be found in the system.", plate);
				return;
			}

			NetworkEventSender.SendNetworkEvent_MdcVehicleResult(SendingPlayer, vehicle);
		}
		else
		{
			SendingPlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "Plate '{0}' could not be found in the system.", plate);
			return;
		}
	}

	public async void OpenPropertyMDT(CPlayer SendingPlayer, CVehicle SenderVehicle, string zip)
	{
		CFaction courtFaction = FactionPool.GetFactionFromShortName("SCSA");
		if ((!SendingPlayer.IsInFactionOfType(EFactionType.LawEnforcement) || !SendingPlayer.IsOnDutyOfType(EDutyType.Law_Enforcement)) && !SendingPlayer.IsInFaction(courtFaction.FactionID))
		{
			SendingPlayer.SendNotification("Property", ENotificationIcon.ExclamationSign, "You must be on duty as Law Enforcement to perform this action.");
			return;
		}

		CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(Convert.ToInt64(zip));

		if (propInst != null)
		{
			CMdtProperty property = await Database.LegacyFunctions.GetPropertyByZip(propInst.Model.State, propInst.Model.OwnerType, zip).ConfigureAwait(true);
			if (!property.found)
			{
				SendingPlayer.SendNotification("Property", ENotificationIcon.ExclamationSign, "ZIP '{0}' could not be found in the system.", zip);
				return;
			}

			NetworkEventSender.SendNetworkEvent_MdcPropertyResult(SendingPlayer, property);
		}
		else
		{
			SendingPlayer.SendNotification("Property", ENotificationIcon.ExclamationSign, "ZIP '{0}' could not be found in the system.", zip);
			return;
		}
	}

	public void OpenPersonMDT(CPlayer SendingPlayer, CVehicle SenderVehicle, string name)
	{
		CFaction courtFaction = FactionPool.GetFactionFromShortName("SCSA");
		if ((!SendingPlayer.IsInFactionOfType(EFactionType.LawEnforcement) || !SendingPlayer.IsOnDutyOfType(EDutyType.Law_Enforcement)) && !SendingPlayer.IsInFaction(courtFaction.FactionID))
		{
			SendingPlayer.SendNotification("Person", ENotificationIcon.ExclamationSign, "You must be on duty as Law Enforcement to perform this action.");
			return;
		}

		// TODO_OPTIMIZATION: Might be faster to check online players first and THEN hit db?
		Database.Functions.Characters.GetCharacterDBIDFromName(name, async (EntityDatabaseID CharacterID) =>
		{
			if (CharacterID == -1)
			{
				WeakReference<CPlayer> TargetPlayerRef = PlayerPool.GetPlayerFromPartialNameOrID(name);
				CPlayer TargetPlayer = TargetPlayerRef.Instance();
				if (TargetPlayer != null)
				{
					CharacterID = TargetPlayer.ActiveCharacterDatabaseID;
				}
			}

			CStatsResult person = await Database.LegacyFunctions.GetCharacterStats(CharacterID).ConfigureAwait(true);
			if (!person.found)
			{
				SendingPlayer.SendNotification("Person", ENotificationIcon.ExclamationSign, "Name '{0}' could not be found in the system.", name);
				return;
			}

			NetworkEventSender.SendNetworkEvent_MdcPersonResult(SendingPlayer, person);
		});
	}

	private void OnTicketResponse(CPlayer pTicketedPlayer, bool bAccepted)
	{
		var ticketInstance = GetTicketInstanceFromTicketedPlayer(pTicketedPlayer);
		if (ticketInstance != null)
		{
			m_lstTicketedPlayerInstances.Remove(ticketInstance);

			CPlayer pTicketingOfficer = ticketInstance.m_TicketingOfficer.Instance();
			if (pTicketingOfficer == null)
			{
				pTicketedPlayer.PushChatMessage(EChatChannel.Notifications, "Your ticket was not paid because the officer is no longer online.");
				return;
			}

			// Can we afford if we accepted?
			if (bAccepted)
			{
				bool bCouldAfford = false;
				if (pTicketedPlayer.SubtractBankBalanceIfCanAfford(ticketInstance.m_fAmount, PlayerMoneyModificationReason.PDTicket))
				{
					bCouldAfford = true;
				}
				else
				{
					if (pTicketedPlayer.SubtractMoney(ticketInstance.m_fAmount, PlayerMoneyModificationReason.PDTicket))
					{
						bCouldAfford = true;
					}
				}

				if (bCouldAfford)
				{
					pTicketingOfficer.PushChatMessage(EChatChannel.Notifications, "{0} has paid your ticket of {1} for '{2}'.", pTicketedPlayer.GetCharacterName(ENameType.StaticCharacterName), ticketInstance.m_fAmount, ticketInstance.m_strReason);
					pTicketedPlayer.PushChatMessage(EChatChannel.Notifications, "You have paid your ticket of ${0} from {1}.", ticketInstance.m_fAmount, pTicketingOfficer.GetCharacterName(ENameType.StaticCharacterName));

					pTicketedPlayer.AwardAchievement(EAchievementID.BeFined);
					pTicketingOfficer.AwardAchievement(EAchievementID.IssueFine);

					CFaction Faction = FactionPool.GetPoliceFaction();
					Faction.Money += ticketInstance.m_fAmount;
				}
				else
				{
					pTicketingOfficer.PushChatMessage(EChatChannel.Notifications, "{0} could not afford your ticket of {1} for '{2}'.", pTicketedPlayer.GetCharacterName(ENameType.StaticCharacterName), ticketInstance.m_fAmount, ticketInstance.m_strReason);
					pTicketedPlayer.PushChatMessage(EChatChannel.Notifications, "You could not afford your ticket of {0} from {1} for '{2}'.", ticketInstance.m_fAmount, pTicketingOfficer.GetCharacterName(ENameType.StaticCharacterName), ticketInstance.m_strReason);
				}
			}
			else
			{
				pTicketingOfficer.PushChatMessage(EChatChannel.Notifications, "{0} has declined to pay your ticket of {1} for '{2}'.", pTicketedPlayer.GetCharacterName(ENameType.StaticCharacterName), ticketInstance.m_fAmount, ticketInstance.m_strReason);
				pTicketedPlayer.PushChatMessage(EChatChannel.Notifications, "You have declined to pay your ticket of {0} from {1} for '{2}'.", ticketInstance.m_fAmount, pTicketingOfficer.GetCharacterName(ENameType.StaticCharacterName), ticketInstance.m_strReason);
			}
		}
	}

	// acc911 cancel911
	public void Add911Call(CPlayer a_CallingPlayer, CItemValueCellphone phone, Vector3 a_vecPosition, string a_strEmergency, string a_strService = "Police", string a_strCaller = "Unknown", bool bIsRobbery = false)
	{
		int index = m_lst911CallInstances.Count;
		C911CallInstance newInst = new C911CallInstance(a_CallingPlayer, a_vecPosition, a_strEmergency);
		m_lst911CallInstances.Add(newInst);

		// Inform all govt players if it's not a robbery, otherwise inform only PD
		if (!bIsRobbery)
		{
			SendGovtFactionsMessage(Helpers.FormatString(" 911 DISPATCH FROM {0}: Service requested: {1}, Caller name: {2}, Emergency: {3}", phone != null ? phone.number.ToString() : "Unknown", a_strService, a_strCaller, a_strEmergency), true);
			SendGovtFactionsMessage(Helpers.FormatString("Use /acc911 {0} to respond", index), true);
		}
		else
		{
			SendPDFactionMessage(Helpers.FormatString(" 911 DISPATCH FROM {0}: Service requested: {1}, Caller name: {2}, Emergency: {3}", phone != null ? phone.number.ToString() : "Unknown", a_strService, a_strCaller, a_strEmergency), true);
			SendPDFactionMessage(Helpers.FormatString("Use /acc911 {0} to respond", index), true);
		}
	}

	public void NineOneOneCommand(CPlayer player, CVehicle vehicle, string a_strMessage)
	{
		int index = m_lst911CallInstances.Count;
		C911CallInstance newInst = new C911CallInstance(player, player.Client.Position, a_strMessage);
		m_lst911CallInstances.Add(newInst);
		string number = "Unknown";

		player.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "911 Dispatched.");
		// Inform all govt players
		SendGovtFactionsMessage(Helpers.FormatString(" 911 DISPATCH FROM {0}: {1}", number, a_strMessage), true);
		SendGovtFactionsMessage(Helpers.FormatString("Use /acc911 {0} to respond", index), true);
	}

	public void Remove911CallsForPlayer(CPlayer a_CallingPlayer)
	{
		List<C911CallInstance> lstToRemove = new List<C911CallInstance>();
		foreach (C911CallInstance callInstance in m_lst911CallInstances)
		{
			if (callInstance.m_CallingPlayer.Instance() == a_CallingPlayer)
			{
				lstToRemove.Add(callInstance);
			}
		}

		foreach (C911CallInstance callInstanceToDestroy in lstToRemove)
		{
			m_lst911CallInstances.Remove(callInstanceToDestroy);
		}
	}

	private void SendGovtFactionsMessage(string message, bool bIncludeOffDuty)
	{
		List<CFaction> lstGovtFactions = FactionPool.GetEmergencyFactions();
		List<CPlayer> lstFactionMembers = new List<CPlayer>();

		foreach (CFaction govtFaction in lstGovtFactions)
		{
			foreach (CPlayer factionMember in govtFaction.GetMembers())
			{
				if (!lstFactionMembers.Contains(factionMember))
				{
					lstFactionMembers.Add(factionMember);
				}
			}
		}

		foreach (CPlayer factionMember in lstFactionMembers)
		{
			if (bIncludeOffDuty || factionMember.IsOnDuty())
			{
				factionMember.PushChatMessageWithColor(EChatChannel.Factions, 255, 140, 105, message);
			}
		}
	}

	private void SendTowingFactionsMessage(string message, bool bIncludeOffDuty)
	{
		List<CFaction> lstTowingFactions = FactionPool.GetTowingFactions();
		List<CPlayer> lstFactionMembers = new List<CPlayer>();

		foreach (CFaction towingFaction in lstTowingFactions)
		{
			foreach (CPlayer factionMember in towingFaction.GetMembers())
			{
				if (!lstFactionMembers.Contains(factionMember))
				{
					lstFactionMembers.Add(factionMember);
				}
			}
		}

		foreach (CPlayer factionMember in lstFactionMembers)
		{
			if (bIncludeOffDuty || factionMember.IsOnDuty())
			{
				factionMember.PushChatMessageWithColor(EChatChannel.Factions, 255, 140, 105, message);
			}
		}
	}


	private void SendPDFactionMessage(string message, bool bIncludeOffDuty)
	{
		CFaction policeFaction = FactionPool.GetPoliceFaction();
		List<CPlayer> lstFactionMembers = new List<CPlayer>();

		foreach (CPlayer factionMember in policeFaction.GetMembers())
		{
			if (!lstFactionMembers.Contains(factionMember))
			{
				lstFactionMembers.Add(factionMember);
			}
		}

		foreach (CPlayer factionMember in lstFactionMembers)
		{
			if (bIncludeOffDuty || factionMember.IsOnDuty())
			{
				factionMember.PushChatMessageWithColor(EChatChannel.Factions, 255, 140, 105, message);
			}
		}
	}

	public void Accept911(CPlayer PolicePlayer, CVehicle SenderVehicle, int index)
	{
		if (PolicePlayer != null)
		{
			if (PolicePlayer.IsInGovernmentFaction())
			{
				if (index < m_lst911CallInstances.Count)
				{
					C911CallInstance inst = m_lst911CallInstances[index];
					if (inst != null)
					{
						if (inst.m_RespondingPlayers.Contains(PolicePlayer))
						{
							PolicePlayer.SendNotification("911 Dispatch", ENotificationIcon.ExclamationSign, "You are already responding to this call.");
						}
						else
						{
							inst.m_RespondingPlayers.Add(PolicePlayer);
							SendGovtFactionsMessage(Helpers.FormatString("{0} is responding to call {1}", PolicePlayer.GetCharacterName(ENameType.StaticCharacterName), index), true);
							NetworkEventSender.SendNetworkEvent_Create911LocationBlip(PolicePlayer, inst.m_vecPosition);

							if (inst.m_RespondingPlayers.Count == 1) // first officer? tell the reporter
							{
								if (inst.m_CallingPlayer.Instance() != null)
								{
									inst.m_CallingPlayer.Instance().PushChatMessageWithColor(EChatChannel.Notifications, 255, 194, 15, "911 Dispatch: An officer has been dispatched to your call.");
								}
							}
						}
					}
				}
				else
				{
					PolicePlayer.SendNotification("911 Dispatch", ENotificationIcon.ExclamationSign, "Invalid call index.");
				}
			}
			else
			{
				PolicePlayer.SendNotification("911 Dispatch", ENotificationIcon.ExclamationSign, "You must be in a government faction to perform this action.");
			}
		}
	}

	public void Cancel911(CPlayer PolicePlayer, CVehicle SenderVehicle, int index)
	{
		if (PolicePlayer != null)
		{
			if (PolicePlayer.IsInGovernmentFaction())
			{
				if (index < m_lst911CallInstances.Count)
				{
					C911CallInstance inst = m_lst911CallInstances[index];
					if (inst != null)
					{
						if (inst.m_RespondingPlayers.Contains(PolicePlayer))
						{
							NetworkEventSender.SendNetworkEvent_Destroy911LocationBlip(PolicePlayer);

							foreach (CPlayer respondingPlayer in inst.m_RespondingPlayers)
							{
								respondingPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 194, 15, Helpers.FormatString("911 Dispatch: {0} was dismissed from the 911 call.", PolicePlayer.GetCharacterName(ENameType.StaticCharacterName)));
							}

							inst.m_RespondingPlayers.Remove(PolicePlayer);
						}
						else
						{
							PolicePlayer.SendNotification("911 Dispatch", ENotificationIcon.ExclamationSign, "You are already responding to this call.");
						}
					}
				}
				else
				{
					PolicePlayer.SendNotification("911 Dispatch", ENotificationIcon.ExclamationSign, "Invalid call index.");
				}
			}
			else
			{
				PolicePlayer.SendNotification("911 Dispatch", ENotificationIcon.ExclamationSign, "You must be in a government faction to perform this action.");
			}
		}
	}

	private void SendGovtFactionsOOCMessage(CPlayer SourcePlayer, CVehicle SourceVehicle, string message)
	{
		if (SourcePlayer.IsInGovernmentFaction())
		{
			List<CFaction> lstGovtFactions = FactionPool.GetGovernmentFactions();
			List<CPlayer> lstFactionMembers = new List<CPlayer>();

			foreach (CFaction govtFaction in lstGovtFactions)
			{
				foreach (CPlayer factionMember in govtFaction.GetMembers())
				{
					if (!lstFactionMembers.Contains(factionMember))
					{
						lstFactionMembers.Add(factionMember);
					}
				}
			}

			foreach (CPlayer factionMember in lstFactionMembers)
			{
				factionMember.PushChatMessageWithColor(EChatChannel.Factions, 216, 191, 216, Helpers.FormatString("[GOVERNMENT OOC] {0}: {1}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), message));
			}
		}
	}

	private class C911CallInstance
	{
		public C911CallInstance(CPlayer a_CallingPlayer, Vector3 a_vecPosition, string a_strMessage)
		{
			m_CallingPlayer.SetTarget(a_CallingPlayer);
			m_vecPosition = a_vecPosition;
			m_strMessage = a_strMessage;
		}

		public WeakReference<CPlayer> m_CallingPlayer = new WeakReference<CPlayer>(null);
		public Vector3 m_vecPosition;
		public string m_strMessage;
		public List<CPlayer> m_RespondingPlayers = new List<CPlayer>();
	}

	private List<C911CallInstance> m_lst911CallInstances = new List<C911CallInstance>();
}

