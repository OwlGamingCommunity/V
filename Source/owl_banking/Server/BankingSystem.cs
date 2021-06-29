using GTANetworkAPI;
using System;
using System.Collections.Generic;

using EntityDatabaseID = System.Int64;

public class BankingSystem
{
	private const float g_fPayCommandDistanceLimit = 10.0f;
	private const float g_fMaxAmountViaPay = 999999999.0f;

	public BankingSystem()
	{
		NetworkEvents.Banking_GetAccountInfo += OnGetAccountInfo;
		NetworkEvents.Banking_OnWithdraw += OnWithdraw;
		NetworkEvents.Banking_OnDeposit += OnDeposit;
		NetworkEvents.Banking_OnWireTransfer += OnWireTransfer;
		NetworkEvents.Banking_OnPayDownDebt += OnPayDownDebt;
		NetworkEvents.Banking_ShowMobileBankingUI += OnMobileBanking;

		CommandManager.RegisterCommand("pay", "Pays money to another player using the cash you have on your person. Bank transfers can be done via ATM's / Bank Tellers.", new Action<CPlayer, CVehicle, CPlayer, float>(PayCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("charity", "Donates money to charity from your on-hand cash ((Removes money from your character, using on-character cash))", new Action<CPlayer, CVehicle, float>(CharityCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("bankcharity", "Donates money to charity from your bank balance ((Removes money from your character, using cash in your bank))", new Action<CPlayer, CVehicle, float>(BankCharityCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		LoadAllBanks();
	}

	private void OnMobileBanking(CPlayer SourcePlayer)
	{
		NetworkEventSender.SendNetworkEvent_ShowMobileBankUI(SourcePlayer);

	}

	private void CharityCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, float fAmount)
	{
		// Negative would be injecting money / removing it from the target player...
		if (fAmount <= 0.0f)
		{
			SourcePlayer.SendNotification("Charity", ENotificationIcon.ExclamationSign, "The amount you wish to donate must be greater than zero.");
		}
		else
		{
			// Do we have enough?
			if (SourcePlayer.Money < fAmount)
			{
				SourcePlayer.SendNotification("Charity", ENotificationIcon.ExclamationSign, "You do not have enough money to donate ${0:0.00} to charity from your on-hand cash.", fAmount);
			}
			else
			{
				SourcePlayer.RemoveMoney(fAmount, PlayerMoneyModificationReason.Charity);

				SourcePlayer.SendNotification("Charity", ENotificationIcon.PiggyBank, "You have donated ${0:0.00} to charity from your on-hand cash.", fAmount);
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("'{0} donated ${1:0.00} to charity using their on-hand cash.", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);

				new Logging.Log(SourcePlayer, Logging.ELogType.CashTransfer, null, Helpers.FormatString("/charity - Character: {0} - Amount: {1}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount)).execute();
			}
		}
	}

	private void BankCharityCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, float fAmount)
	{
		// Negative would be injecting money / removing it from the target player...
		if (fAmount <= 0.0f)
		{
			SourcePlayer.SendNotification("Charity", ENotificationIcon.ExclamationSign, "The amount you wish to donate must be greater than zero.");
		}
		else
		{
			// Do we have enough?
			if (SourcePlayer.BankMoney < fAmount)
			{
				SourcePlayer.SendNotification("Charity", ENotificationIcon.ExclamationSign, "You do not have enough money to donate ${0:0.00} to charity from your bank.", fAmount);
			}
			else
			{
				SourcePlayer.RemoveBankMoney(fAmount, PlayerMoneyModificationReason.BankCharity);

				SourcePlayer.SendNotification("Charity", ENotificationIcon.PiggyBank, "You have donated ${0:0.00} to charity.", fAmount);
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("'{0} donated ${1:0.00} to charity using their on-hand cash.", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount), r: 255, g: 0, b: 0);

				new Logging.Log(SourcePlayer, Logging.ELogType.CashTransfer, null, Helpers.FormatString("/bankcharity - Character: {0} - Amount: {1}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount)).execute();
			}
		}
	}

	private void PayCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, float fAmount)
	{
		// Is it the same player?
		if (SourcePlayer == TargetPlayer)
		{
			SourcePlayer.SendNotification("Payments", ENotificationIcon.ExclamationSign, "You cannot transfer money to yourself.");
		}
		else
		{
			// Negative would be injecting money / removing it from the target player...
			if (fAmount <= 0.0f)
			{
				SourcePlayer.SendNotification("Payments", ENotificationIcon.ExclamationSign, "The amount you wish to transfer must be greater than zero.");
			}
			else
			{
				// Do we have enough?
				if (SourcePlayer.Money < fAmount)
				{
					SourcePlayer.SendNotification("Payments", ENotificationIcon.ExclamationSign, "You do not have enough money to transfer ${0:0.00} to '{1}'.", fAmount, TargetPlayer.GetCharacterName(ENameType.CharacterDisplayName));
				}
				else
				{
					// Is it too much?
					if (fAmount > g_fMaxAmountViaPay)
					{
						SourcePlayer.SendNotification("Payments", ENotificationIcon.ExclamationSign, "You cannot transfer more than ${0:0.00} with /pay. Use a wire transfer at an ATM / Bank Teller to transfer higher amounts.", g_fMaxAmountViaPay);
					}
					else
					{
						float fDist = (SourcePlayer.Client.Position - TargetPlayer.Client.Position).Length();
						if (fDist <= g_fPayCommandDistanceLimit)
						{
							SourcePlayer.RemoveMoney(fAmount, PlayerMoneyModificationReason.PayCommand_Sender);
							TargetPlayer.AddMoney(fAmount, PlayerMoneyModificationReason.PayCommand_Receiver);

							// TODO_LAUNCH: Log this for admins! especially large transfers
							SourcePlayer.SendNotification("Payments", ENotificationIcon.PiggyBank, "You have paid ${0:0.00} to '{1}'.", fAmount, TargetPlayer.GetCharacterName(ENameType.CharacterDisplayName));
							TargetPlayer.SendNotification("Payments", ENotificationIcon.PiggyBank, "You have received ${0:0.00} from '{1}'.", fAmount, SourcePlayer.GetCharacterName(ENameType.CharacterDisplayName));
							TargetPlayer.PushChatMessage(EChatChannel.Notifications, "You have received ${0:0.00} from '{1}'.", fAmount, SourcePlayer.GetCharacterName(ENameType.CharacterDisplayName));
							SourcePlayer.PushChatMessage(EChatChannel.Notifications, "You have paid ${0:0.00} to '{1}'.", fAmount, TargetPlayer.GetCharacterName(ENameType.CharacterDisplayName));

							new Logging.Log(SourcePlayer, Logging.ELogType.CashTransfer, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/pay - From: {0} - To: {1} - Amount: {2}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fAmount)).execute();
						}
						else
						{
							SourcePlayer.SendNotification("Payments", ENotificationIcon.ExclamationSign, "'{0}' is too far away.", fAmount);
						}
					}
				}
			}
		}

	}

	public async void LoadAllBanks()
	{
		List<CDatabaseStructureBank> lstBanks = await Database.LegacyFunctions.LoadAllBanks().ConfigureAwait(true);
		foreach (var bank in lstBanks)
		{
			NAPI.Task.Run(async () =>
			{
				await BankPool.CreateBank(bank.BankID, bank.vecPos, bank.fRot, bank.bankType, bank.dimension, false).ConfigureAwait(true);
			});
		}

		NAPI.Util.ConsoleOutput("[BANKING] Loaded {0} Banks!", lstBanks.Count);
	}

	private void SendResponse(CPlayer player, EBankingResponseCode result)
	{
		NetworkEventSender.SendNetworkEvent_Banking_OnServerResponse(player, result);
	}

	public void OnWithdraw(CPlayer player, float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID)
	{
		if (PurchaserType == EPurchaserType.Character)
		{
			if (player.SubtractBankBalanceIfCanAfford(fAmount, PlayerMoneyModificationReason.Bank_Withdraw))
			{
				player.AddMoney(fAmount, PlayerMoneyModificationReason.Bank_Withdraw);
				player.SendNotification("Bank", ENotificationIcon.PiggyBank, "You withdrew ${0:0.00} from your bank account.", fAmount);
				SendResponse(player, EBankingResponseCode.Success);
				new Logging.Log(player, Logging.ELogType.CashTransfer, null, Helpers.FormatString("BANK WITHDRAW - new balance: ${0} - Amount: ${1}", player.BankMoney, fAmount)).execute();
			}
			else
			{
				SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
			}
		}
		else if (PurchaserType == EPurchaserType.Faction)
		{
			CFaction factionInst = FactionPool.GetFactionFromID(PurchaserID);

			if (factionInst != null)
			{
				CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);

				if (factionMembership != null)
				{
					if (factionMembership.Manager)
					{
						if (factionInst.SubtractMoney(fAmount))
						{
							player.AddMoney(fAmount, PlayerMoneyModificationReason.Bank_WithdrawFromFaction);

							factionInst.SendNotificationToAllManagers(Helpers.FormatString("{0} withdrew ${1:0.00} from the faction bank account.", player.GetCharacterName(ENameType.StaticCharacterName), fAmount));
							SendResponse(player, EBankingResponseCode.Success);

							new Logging.Log(player, Logging.ELogType.CashTransfer, null, Helpers.FormatString("BANK WITHDRAW - new balance: ${0} - Amount: ${1} - Faction ID: {2}", factionInst.Money, fAmount, factionInst.m_DatabaseID)).execute();
						}
						else
						{
							SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
						}
					}
				}
			}
		}
	}

	public void OnDeposit(CPlayer player, float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID)
	{
		if (PurchaserType == EPurchaserType.Character)
		{
			if (player.SubtractMoney(fAmount, PlayerMoneyModificationReason.Bank_Deposit))
			{
				player.AddBankMoney(fAmount, PlayerMoneyModificationReason.Bank_Deposit);
				player.SendNotification("Bank", ENotificationIcon.PiggyBank, "You deposited ${0:0.00} into your bank account.", fAmount);
				SendResponse(player, EBankingResponseCode.Success);

				new Logging.Log(player, Logging.ELogType.CashTransfer, null, Helpers.FormatString("BANK DEPOSIT - new balance: ${0} - Amount: ${1}", player.BankMoney, fAmount)).execute();
			}
			else
			{
				SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
			}
		}
		else if (PurchaserType == EPurchaserType.Faction)
		{
			CFaction factionInst = FactionPool.GetFactionFromID(PurchaserID);

			if (factionInst != null)
			{
				CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);

				if (factionMembership != null)
				{
					if (factionMembership.Manager)
					{
						if (player.SubtractMoney(fAmount, PlayerMoneyModificationReason.Bank_DepositToFaction))
						{
							factionInst.Money += fAmount;
							factionInst.SendNotificationToAllManagers(Helpers.FormatString("{0} deposited ${1:0.00} into the faction bank account.", player.GetCharacterName(ENameType.StaticCharacterName), fAmount));
							SendResponse(player, EBankingResponseCode.Success);

							new Logging.Log(player, Logging.ELogType.CashTransfer, null, Helpers.FormatString("BANK DEPOSIT - new balance: ${0} - Amount: ${1} - Faction ID: {2}", factionInst.Money, fAmount, factionInst.m_DatabaseID)).execute();
						}
						else
						{
							SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
						}
					}
				}
			}
		}
	}

	public void OnGetAccountInfo(CPlayer player, EPurchaserType PurchaserType, Int64 PurchaserID)
	{
		if (PurchaserType == EPurchaserType.Character)
		{
			List<CreditDetails> lstCreditDetails = player.GetActiveCreditDetails();
			NetworkEventSender.SendNetworkEvent_Banking_GotAccountInfo(player, player.BankMoney, lstCreditDetails);
		}
		else if (PurchaserType == EPurchaserType.Faction)
		{
			CFaction factionInst = FactionPool.GetFactionFromID(PurchaserID);

			if (factionInst != null)
			{
				CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);

				if (factionMembership != null)
				{
					if (factionMembership.Manager)
					{
						List<CreditDetails> lstCreditDetails = factionInst.GetActiveCreditDetails();
						NetworkEventSender.SendNetworkEvent_Banking_GotAccountInfo(player, factionInst.Money, lstCreditDetails);
					}
				}
			}
		}
	}

	// NOTE: CreditSourceID is only set if source is faction, otherwise just use source player
	public void OnPayDownDebt(CPlayer player, EPurchaserType CreditSource, Int64 CreditSourceID, ECreditType CreditType, EntityDatabaseID a_ID, float fAmount)
	{
		if (fAmount <= 0.0f)
		{
			return;
		}

		CFaction factionInst = null;
		if (CreditSource == EPurchaserType.Faction)
		{
			factionInst = FactionPool.GetFactionFromID(CreditSourceID);
			if (factionInst == null)
			{
				return;
			}
		}

		if (CreditType == ECreditType.Vehicle)
		{
			CVehicle vehicle = factionInst == null
				? VehiclePool.GetPlayerOwnedVehicleByID(player, a_ID)
				: VehiclePool.GetFactionOwnedVehicleByID(factionInst, a_ID);

			PayDownVehicle(vehicle, player, factionInst, fAmount);
		}

		if (CreditType == ECreditType.Property)
		{
			CPropertyInstance property = factionInst == null
				? PropertyPool.GetPlayerOwnedPropertyByID(player, a_ID)
				: PropertyPool.GetFactionOwnedPropertyByID(factionInst, a_ID);

			PayDownProperty(property, player, factionInst, fAmount);
		}
	}

	private void PayDownProperty(CPropertyInstance property, CPlayer player, CFaction faction, float fAmount)
	{
		if (property == null)
		{
			return;
		}

		float fDebtRemaining = property.GetRemainingCredit(true);
		if (fAmount > fDebtRemaining)
		{
			fAmount = fDebtRemaining;
		}

		// Subtract from the faction instance if we have one, otherwise take from the player.
		bool subtractedBalance = faction?.SubtractMoney(fAmount) ?? player.SubtractBankBalanceIfCanAfford(fAmount, PlayerMoneyModificationReason.PropertyPayDown);
		if (!subtractedBalance)
		{
			player.SendNotification("Bank", ENotificationIcon.PiggyBank, "You do not have sufficient bank balance to pay this.");
			return;
		}

		property.DecreaseCreditAmount(fAmount);

		List<CreditDetails> lstCreditDetails = faction == null
			? player.GetActiveCreditDetails()
			: faction.GetActiveCreditDetails();
		NetworkEventSender.SendNetworkEvent_Banking_RefreshCreditInfo(player, lstCreditDetails);

		SendPayDownSuccessMessage(
			player,
			faction,
			fAmount,
			property.GetRemainingCredit(true),
			"property " + property.Model.Name
		);
	}

	private void PayDownVehicle(CVehicle vehicle, CPlayer player, CFaction faction, float fAmount)
	{
		if (vehicle == null)
		{
			return;
		}
		if (vehicle.IsLegacyCreditVehicle())
		{
			player.SendNotification("Bank", ENotificationIcon.PiggyBank, "This vehicle was purchased under the legacy credit system and cannot be paid off early.");
			return;
		}

		float fDebtRemaining = vehicle.GetRemainingCredit(true);
		if (fAmount > fDebtRemaining)
		{
			fAmount = fDebtRemaining;
		}

		// Subtract from the faction instance if we have one, otherwise take from the player.
		bool subtractedBalance = faction?.SubtractMoney(fAmount) ?? player.SubtractBankBalanceIfCanAfford(fAmount, PlayerMoneyModificationReason.VehiclePayDown);
		if (!subtractedBalance)
		{
			player.SendNotification("Bank", ENotificationIcon.PiggyBank, "You do not have sufficient bank balance to pay this.");
			return;
		}

		vehicle.DecreaseCreditAmount(fAmount);

		List<CreditDetails> lstCreditDetails = faction == null
			? player.GetActiveCreditDetails()
			: faction.GetActiveCreditDetails();
		NetworkEventSender.SendNetworkEvent_Banking_RefreshCreditInfo(player, lstCreditDetails);

		SendPayDownSuccessMessage(
			player,
			faction,
			fAmount,
			vehicle.GetRemainingCredit(true),
			vehicle.GetFullDisplayName()
		);
	}

	private void SendPayDownSuccessMessage(CPlayer player, CFaction faction, float fAmount, float fDebtRemaining, string entityName)
	{
		string message = Helpers.FormatString(
			"{0} paid off ${1:0.00} of the {2}.",
			faction == null ? "You" : player.GetCharacterName(ENameType.StaticCharacterName),
			fAmount,
			entityName
		);

		if (fDebtRemaining > 0)
		{
			message += Helpers.FormatString(" The remaining debt is  ${0:0.00}.", fDebtRemaining);
		}
		else
		{
			message += " Congratulations, the whole debt has been paid off.";
		}

		if (faction == null)
		{
			player.SendNotification("Bank", ENotificationIcon.PiggyBank, message);
			return;
		}

		faction.SendNotificationToAllManagers(message);
	}

	public async void OnWireTransfer(CPlayer player, string strTargetName, float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID)
	{
		// Does target player exist?
		SVerifyCharacterExists Result = await Database.LegacyFunctions.VerifyCharacterExists(strTargetName).ConfigureAwait(true);
		CFaction toFactionInst = FactionPool.GetFactionFromName(strTargetName);
		CFaction fromFactionInst = FactionPool.GetFactionFromID(PurchaserID);

		// Check for short name if the long name returned null
		if (toFactionInst == null)
		{
			toFactionInst = FactionPool.GetFactionFromShortName(strTargetName);
		}

		// Faction to faction
		if (toFactionInst != null)
		{
			if (fromFactionInst != null)
			{
				CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(fromFactionInst);

				if (factionMembership != null)
				{
					if (factionMembership.Manager)
					{
						if (fromFactionInst.SubtractMoney(fAmount))
						{
							fromFactionInst.SendNotificationToAllManagers(Helpers.FormatString("{0} wire transferred ${1:0.00} to {2} from the faction bank account.", player.GetCharacterName(ENameType.StaticCharacterName), fAmount, toFactionInst.Name));
							SendResponse(player, EBankingResponseCode.Success);

							toFactionInst.Money += fAmount;
							fromFactionInst.SendNotificationToAllManagers(Helpers.FormatString("{0} wire transferred ${1:0.00} to the faction bank account.", fromFactionInst.Name, fAmount));

							new Logging.Log(player, Logging.ELogType.CashTransfer, null, Helpers.FormatString("BANK TRANSFER - From faction ({4}) balance: ${0} - Amount: ${1} - To faction ({3}) balance: ${2}", fromFactionInst.Money, fAmount, toFactionInst.Money, toFactionInst.m_DatabaseID, fromFactionInst.m_DatabaseID)).execute();
						}
						else
						{
							SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
						}
					}
				}
			}
			else // Player to Faction
			{
				if (player.SubtractBankBalanceIfCanAfford(fAmount, PlayerMoneyModificationReason.Bank_WireTransfer_ToFaction))
				{
					player.SendNotification("Bank", ENotificationIcon.PiggyBank, "You wire transferred ${0:0.00} to {1}.", fAmount, toFactionInst.Name);
					SendResponse(player, EBankingResponseCode.Success);

					toFactionInst.Money += fAmount;
					toFactionInst.SendNotificationToAllManagers(Helpers.FormatString("{0} wire transferred ${1:0.00} to the faction bank account.", player.GetCharacterName(ENameType.StaticCharacterName), fAmount));
				}
				else
				{
					SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
				}
			}
		}
		else
		{
			// Does the character exist?
			if (Result.CharacterExists)
			{
				// Does this character belong to our account?
				if (Result.AccountID != player.AccountID)
				{
					if (PurchaserType == EPurchaserType.Character)
					{
						if (player.SubtractBankBalanceIfCanAfford(fAmount, PlayerMoneyModificationReason.Bank_WireTransfer_ToPlayer))
						{
							player.SendNotification("Bank", ENotificationIcon.PiggyBank, "You wire transferred ${0:0.00} to {1}.", fAmount, Result.CharacterNameClean);
							SendResponse(player, EBankingResponseCode.Success);

							// Update bank for remote player

							// Is the player online now? if so update + send notification
							WeakReference<CPlayer> remotePlayerRef = PlayerPool.GetPlayerFromName(Result.CharacterNameClean);
							CPlayer remotePlayer = remotePlayerRef.Instance();

							if (remotePlayer != null)
							{
								remotePlayer.AddBankMoney(fAmount, PlayerMoneyModificationReason.Bank_WireTransfer_FromPlayer);

								// TODO: Support offline/queued notifications for remote players
								remotePlayer.SendNotification("Bank", ENotificationIcon.PiggyBank, "You received ${0:0.00} from {1} via wire transfer.", fAmount, player.GetCharacterName(ENameType.StaticCharacterName));

								new Logging.Log(player, Logging.ELogType.CashTransfer, new List<CBaseEntity> { remotePlayer }, Helpers.FormatString("BANK TRANSFER - From balance: ${0} - Amount: ${1} - To balance: ${2}", player.BankMoney, fAmount, remotePlayer.BankMoney)).execute();
							}
							else // Give it to the offline player, we only have to do the sql query here since BankMoney does it above
							{
								await Database.LegacyFunctions.AddOfflinePlayerBankMoney(Result.CharacterID, fAmount).ConfigureAwait(true);

								new Logging.Log(player, Logging.ELogType.CashTransfer, null, Helpers.FormatString("BANK TRANSFER OFFLINE - From balance: ${0} - Amount: ${1} - To player: {2}", player.BankMoney, fAmount, Result.CharacterNameClean)).execute();
							}
						}
						else
						{
							SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
						}
					}
					else if (PurchaserType == EPurchaserType.Faction)
					{
						CFaction factionInst = FactionPool.GetFactionFromID(PurchaserID);

						if (factionInst != null)
						{
							CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);

							if (factionMembership != null)
							{
								if (factionMembership.Manager)
								{
									if (factionInst.SubtractMoney(fAmount))
									{
										factionInst.SendNotificationToAllManagers(Helpers.FormatString("{0} wire transferred ${1:0.00} to {2} from the faction bank account.", player.GetCharacterName(ENameType.StaticCharacterName), fAmount, Result.CharacterNameClean));
										SendResponse(player, EBankingResponseCode.Success);

										// Update bank for remote player

										// Is the player online now? if so update + send notification
										WeakReference<CPlayer> remotePlayerRef = PlayerPool.GetPlayerFromName(Result.CharacterNameClean);
										CPlayer remotePlayer = remotePlayerRef.Instance();

										if (remotePlayer != null)
										{
											remotePlayer.AddBankMoney(fAmount, PlayerMoneyModificationReason.Bank_WireTransfer_FromFaction);

											// TODO: Support offline/queued notifications for remote players
											player.SendNotification("Bank", ENotificationIcon.PiggyBank, "You received ${0:0.00} from {1} (({2})) via wire transfer.", fAmount, factionInst.Name, player.GetCharacterName(ENameType.StaticCharacterName));

											new Logging.Log(player, Logging.ELogType.CashTransfer, new List<CBaseEntity> { remotePlayer }, Helpers.FormatString("BANK TRANSFER - From faction balance: ${0} - Amount: ${1} - To balance: ${2}", factionInst.Money, fAmount, remotePlayer.BankMoney)).execute();

										}
										else // Give it to the offline player, we only have to do the sql query here since BankMoney does it above
										{
											await Database.LegacyFunctions.AddOfflinePlayerBankMoney(Result.CharacterID, fAmount).ConfigureAwait(true);
											new Logging.Log(player, Logging.ELogType.CashTransfer, null, Helpers.FormatString("BANK TRANSFER OFFLINE - From faction balance: ${0} - Amount: ${1} - To player: {2}", factionInst.Money, fAmount, Result.CharacterNameClean)).execute();
										}
									}
									else
									{
										SendResponse(player, EBankingResponseCode.Failed_CannotAfford);
									}
								}
							}
						}
					}
				}
				else
				{
					SendResponse(player, EBankingResponseCode.Failed_CharacterBelongsToSameAccount);
				}
			}
			else
			{
				SendResponse(player, EBankingResponseCode.Failed_TargetDoesntExist);
			}
		}
	}
}