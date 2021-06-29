using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityDatabaseID = System.Int64;

public class VehicleStore
{
	private RentalCarStore m_RentalStore = new RentalCarStore();

	public VehicleStore()
	{
		NetworkEvents.GetPurchaserAndPaymentMethods += OnRequestInfo;
		NetworkEvents.PurchaseVehicle_OnCheckout += OnCheckout;
	}

	public async void OnCheckout(CPlayer player, int vehicleIndex, uint primary_r, uint primary_g, uint primary_b, uint secondary_r, uint secondary_g, uint secondary_b, EPurchaserType PurchaserType, long PurchaserID, EPaymentMethod PaymentMethod,
		float fDownpayment, int numMonthsForPaymentPlan, EScriptLocation location, EVehicleStoreType storeType)
	{
		CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(vehicleIndex);
		if (!vehicleDef.IsPurchasable)
		{
			player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "That vehicle is not purchasable.");
			NetworkEventSender.SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(player, ERentVehicleResult.GenericFailureCheckNotification);
			return;
		}

		// Can we afford it?

		Color primaryCol = new Color(Convert.ToInt32(primary_r), Convert.ToInt32(primary_g), Convert.ToInt32(primary_b));
		Color secondaryCol = new Color(Convert.ToInt32(secondary_r), Convert.ToInt32(secondary_g), Convert.ToInt32(secondary_b));

		EPlateType plateType = EPlateType.Blue_White;
		const float fFuel = 100.0f;
		const string strPlateText = "";
		int colWheel = 0; // TODO: Allow customization on buy?
		int liveryID = 0; // TODO: Allow customization on buy? It really only applies to business vehicles
		const float fDirt = 0.0f;
		const float fHealth = 1000.0f;
		const bool bLocked = true;
		const bool bEngineOn = false;
		float fOdometer = 0.0f;

		VehicleStoreSpawnPosition spawnPos = GetSpawnPosition(location, storeType);

		// Character purchaser
		if (PurchaserType == EPurchaserType.Character)
		{
			if (PaymentMethod == EPaymentMethod.Token)
			{
				if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.VehicleToken))
				{
					// Only allow token vehicles
					if (!vehicleDef.CanBuyWithToken)
					{
						NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.InvalidClassForVehicleToken);
					}
					else
					{
						// Does the player have space for key?
						if (player.Inventory.CanGiveItem(CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, 0), out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
						{
							const int iPaymentsRemaining = 0;
							int iPaymentsMade = 0;
							int iPaymentsMissed = 0;
							float fCreditAmount = 0.0f;

							const EntityDatabaseID EmptyDBID = 0;
							const uint expiryTime = 0;

							Int64 unixTimestamp = Helpers.GetUnixTimestamp();
							CVehicle pVehicle = await VehiclePool.CreateVehicle(EmptyDBID, EVehicleType.PlayerOwned, player.ActiveCharacterDatabaseID, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? 0 : (VehicleHash)vehicleDef.Hash, spawnPos.Position, spawnPos.Rotation, spawnPos.Position, spawnPos.Rotation, plateType,
								strPlateText, fFuel, primaryCol.Red, primaryCol.Green, primaryCol.Blue, secondaryCol.Red, secondaryCol.Green, secondaryCol.Blue, colWheel, liveryID, fDirt, fHealth, bLocked, bEngineOn, iPaymentsRemaining, iPaymentsMade, iPaymentsMissed,
								fCreditAmount, true, true, expiryTime, fOdometer, false, 0, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, true, EVehicleTransmissionType.Automatic, 0, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? NAPI.Util.GetHashKey(vehicleDef.AddOnName) : 0).ConfigureAwait(true);
							player.DonationInventory.RemoveTokenOfTypeForActiveCharacter(EDonationEffect.VehicleToken);

							// which achievement type?
							if (vehicleDef.Class == EVehicleClass.VehicleClass_Boats.ToString().Replace("VehicleClass_", ""))
							{
								player.AwardAchievement(EAchievementID.BuyBoat);
							}
							else
							{
								player.AwardAchievement(EAchievementID.BuyCar);
							}

							CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, pVehicle.m_DatabaseID);
							player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
							{
								if (bItemGranted)
								{
									player.SendNotification("Vehicle Store", ENotificationIcon.ThumbsUp, "Congratulations on purchasing your {0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
									NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Success);
								}
							});
						}
						else
						{
							player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "You cannot receive the keys to this vehicle:<br>{0}", strUserFriendlyMessage);
							NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
						}
					}
				}
				else
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "You do not have a vehicle token.");
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
			}
			else if (PaymentMethod == EPaymentMethod.BankBalance)
			{
				if (player.CanPlayerAffordBankCost(vehicleDef.Price))
				{
					// Does the player have space for key?
					if (player.Inventory.CanGiveItem(CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, 0), out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
					{
						const int iPaymentsRemaining = 0;
						int iPaymentsMade = 0;
						int iPaymentsMissed = 0;
						float fCreditAmount = 0.0f;

						const EntityDatabaseID EmptyDBID = 0;
						const uint expiryTime = 0;

						Int64 unixTimestamp = Helpers.GetUnixTimestamp();
						CVehicle pVehicle = await VehiclePool.CreateVehicle(EmptyDBID, EVehicleType.PlayerOwned, player.ActiveCharacterDatabaseID, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? 0 : (VehicleHash)vehicleDef.Hash, spawnPos.Position, spawnPos.Rotation, spawnPos.Position, spawnPos.Rotation, plateType,
							strPlateText, fFuel, primaryCol.Red, primaryCol.Green, primaryCol.Blue, secondaryCol.Red, secondaryCol.Green, secondaryCol.Blue, colWheel, liveryID, fDirt, fHealth, bLocked, bEngineOn, iPaymentsRemaining, iPaymentsMade, iPaymentsMissed,
							fCreditAmount, true, true, expiryTime, fOdometer, false, 0, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, EVehicleTransmissionType.Automatic, 0, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? NAPI.Util.GetHashKey(vehicleDef.AddOnName) : 0).ConfigureAwait(true);

						// Take money from bank, otherwise take from player and allow negative (this fixes race conditions)
						if (!player.SubtractBankBalanceIfCanAfford(vehicleDef.Price, PlayerMoneyModificationReason.VehicleStoreCheckout))
						{
							player.SubtractMoneyAllowNegative(vehicleDef.Price, PlayerMoneyModificationReason.VehicleStoreCheckout);
						}

						// which achievement type?
						if (vehicleDef.Class == EVehicleClass.VehicleClass_Boats.ToString().Replace("VehicleClass_", ""))
						{
							player.AwardAchievement(EAchievementID.BuyBoat);
						}
						else
						{
							player.AwardAchievement(EAchievementID.BuyCar);
						}

						CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, pVehicle.m_DatabaseID);
						player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
						{
							if (bItemGranted)
							{
								player.SendNotification("Vehicle Store", ENotificationIcon.ThumbsUp, "Congratulations on purchasing your {0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
								NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Success);
							}
							else
							{
								player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "You cannot receive the keys to this vehicle. Unknown error.");
								NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
							}
						});
					}
					else
					{
						player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "You cannot receive the keys to this vehicle:<br>{0}", strUserFriendlyMessage);
						NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
					}
				}
				else
				{
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.CannotAffordVehicle);
				}
			}
			else if (PaymentMethod == EPaymentMethod.Credit)
			{
				// check params
				if (numMonthsForPaymentPlan < 2)
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Your payment plan must be at least 2 months.", null);
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
				else if (numMonthsForPaymentPlan > 200)
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Your payment plan must be 200 months or less.", null);
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
				else if (fDownpayment < (vehicleDef.Price * 0.05f))
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Your down payment must be at least 5% of the value.", null);
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
				else
				{
					if (player.CanPlayerAffordBankCost(fDownpayment))
					{
						float fAmountMinusDownpayment = (vehicleDef.Price - fDownpayment);
						float fCreditAmount = fAmountMinusDownpayment;
						float fInterest = (fAmountMinusDownpayment * Taxation.GetPaymentPlanInterestPercent());
						float fMonthlyPaymentAmount = (fCreditAmount / numMonthsForPaymentPlan) + (fInterest / numMonthsForPaymentPlan);

						// Can we afford the monthly payments?
						if (player.CanPlayerAffordMonthlyExpense(fMonthlyPaymentAmount))
						{
							// Does the player have space for key?
							if (player.Inventory.CanGiveItem(CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, 0), out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
							{
								int iPaymentsRemaining = numMonthsForPaymentPlan;
								const int iPaymentsMade = 0;
								int iPaymentsMissed = 0;

								const EntityDatabaseID EmptyDBID = 0;
								const uint expiryTime = 0;

								Int64 unixTimestamp = Helpers.GetUnixTimestamp();
								Task<CVehicle> CreateVehicleTask = VehiclePool.CreateVehicle(EmptyDBID, EVehicleType.PlayerOwned, player.ActiveCharacterDatabaseID, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? 0 : (VehicleHash)vehicleDef.Hash, spawnPos.Position, spawnPos.Rotation, spawnPos.Position, spawnPos.Rotation, plateType,
									strPlateText, fFuel, primaryCol.Red, primaryCol.Green, primaryCol.Blue, secondaryCol.Red, secondaryCol.Green, secondaryCol.Blue, colWheel, liveryID, fDirt, fHealth, bLocked, bEngineOn, iPaymentsRemaining, iPaymentsMade,
									iPaymentsMissed, fCreditAmount, true, true, expiryTime, fOdometer, false, 0, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, EVehicleTransmissionType.Automatic, 0, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? NAPI.Util.GetHashKey(vehicleDef.AddOnName) : 0);

								CVehicle pVehicle = await CreateVehicleTask.ConfigureAwait(true);
								// Take downpayment from bank, otherwise take from player and allow negative (this fixes race conditions)
								if (!player.SubtractBankBalanceIfCanAfford(fDownpayment, PlayerMoneyModificationReason.VehicleStoreCheckout))
								{
									player.SubtractMoneyAllowNegative(fDownpayment, PlayerMoneyModificationReason.VehicleStoreCheckout);
								}

								// which achievement type?
								if (vehicleDef.Class == EVehicleClass.VehicleClass_Boats.ToString().Replace("VehicleClass_", ""))
								{
									player.AwardAchievement(EAchievementID.BuyBoat);
								}
								else
								{
									player.AwardAchievement(EAchievementID.BuyCar);
								}

								CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, pVehicle.m_DatabaseID);
								player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
								{
									if (bItemGranted)
									{
										player.SendNotification("Vehicle Store", ENotificationIcon.ThumbsUp, "Congratulations on purchasing your {0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
										NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Success);
									}
									else
									{
										player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "You cannot receive the keys to this vehicle. Unknown error.");
										NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
									}
								});
							}
							else
							{
								player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "You cannot receive the keys to this vehicle:<br>{0}", strUserFriendlyMessage);
								NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
							}
						}
						else
						{
							NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.MonthlyIncomeTooLowForCredit);
						}
					}
					else
					{
						NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.CannotAffordDownpaymentForCredit);
					}
				}
			}
		}
		// Faction purchaser
		else if (PurchaserType == EPurchaserType.Faction)
		{
			if (PaymentMethod == EPaymentMethod.Token)
			{
				player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Vehicle Tokens can not be used for Faction Purchases.");
				NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.VehicleTokensInvalidForFactions);
			}
			else if (PaymentMethod == EPaymentMethod.BankBalance)
			{
				// TODO_POST_LAUNCH: Should factions pay tax?
				CFaction factionInst = FactionPool.GetFactionFromID(PurchaserID);

				if (factionInst != null)
				{
					CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);

					if (factionMembership != null)
					{
						if (factionMembership.Manager)
						{
							if (factionInst.Money >= vehicleDef.Price)
							{
								const int iPaymentsRemaining = 0;
								int iPaymentsMade = 0;
								int iPaymentsMissed = 0;
								float fCreditAmount = 0.0f;

								const EntityDatabaseID EmptyDBID = 0;
								const uint expiryTime = 0;

								Int64 unixTimestamp = Helpers.GetUnixTimestamp();
								CVehicle pVehicle = await VehiclePool.CreateVehicle(EmptyDBID, EVehicleType.FactionOwned, factionInst.FactionID, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? 0 : (VehicleHash)vehicleDef.Hash, spawnPos.Position, spawnPos.Rotation, spawnPos.Position, spawnPos.Rotation, plateType,
									strPlateText, fFuel, primaryCol.Red, primaryCol.Green, primaryCol.Blue, secondaryCol.Red, secondaryCol.Green, secondaryCol.Blue, colWheel, liveryID, fDirt, fHealth, bLocked, bEngineOn, iPaymentsRemaining, iPaymentsMade,
									iPaymentsMissed, fCreditAmount, true, true, expiryTime, fOdometer, false, 0, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, EVehicleTransmissionType.Automatic, 0, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? NAPI.Util.GetHashKey(vehicleDef.AddOnName) : 0).ConfigureAwait(true);
								// Take money from bank, allow negative, since we already did the sql op above
								factionInst.SubtractMoneyAllowNegatve(vehicleDef.Price);

								string strMessage = Helpers.FormatString("{0} bought a {1} {2} for this faction for ${3:0.00}", player.GetCharacterName(ENameType.StaticCharacterName), vehicleDef.Manufacturer, vehicleDef.Name, vehicleDef.Price);
								factionInst.SendNotificationToAllManagers(strMessage);

								NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Success);
							}
							else
							{
								NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Faction_CannotAffordVehicle);
							}
						}
						else
						{
							player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "You are not a manager of this faction.");
							NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
						}
					}
					else
					{
						player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Generic Faction Error");
						NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
					}
				}
				else
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Generic Faction Error");
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
			}
			else if (PaymentMethod == EPaymentMethod.Credit)
			{
				// check params
				if (numMonthsForPaymentPlan < 2)
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Your payment plan must be at least 2 months.", null);
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
				else if (numMonthsForPaymentPlan > 200)
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Your payment plan must be 200 months or less.", null);
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
				else if (fDownpayment < (vehicleDef.Price * 0.05f))
				{
					player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Your down payment must be at least 5% of the value.", null);
					NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
				}
				else
				{
					CFaction factionInst = FactionPool.GetFactionFromID(PurchaserID);

					if (factionInst != null)
					{
						CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);

						if (factionMembership != null)
						{
							if (factionMembership.Manager)
							{
								if (factionInst.Money >= fDownpayment)
								{
									float fAmountMinusDownpayment = (vehicleDef.Price - fDownpayment);
									float fCreditAmount = fAmountMinusDownpayment;
									float fInterest = (fAmountMinusDownpayment * Taxation.GetPaymentPlanInterestPercent());
									float fMonthlyPaymentAmount = (fCreditAmount / numMonthsForPaymentPlan) + (fInterest / numMonthsForPaymentPlan);

									// Can we afford the monthly payments?
									if (factionInst.CanAffordMonthlyExpense(fMonthlyPaymentAmount))
									{
										int iPaymentsRemaining = numMonthsForPaymentPlan;
										const int iPaymentsMade = 0;
										int iPaymentsMissed = 0;

										const EntityDatabaseID EmptyDBID = 0;
										const uint expiryTime = 0;

										Int64 unixTimestamp = Helpers.GetUnixTimestamp();
										CVehicle pVehicle = await VehiclePool.CreateVehicle(EmptyDBID, EVehicleType.FactionOwned, factionInst.FactionID, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? 0 : (VehicleHash)vehicleDef.Hash, spawnPos.Position, spawnPos.Rotation, spawnPos.Position, spawnPos.Rotation, plateType,
											strPlateText, fFuel, primaryCol.Red, primaryCol.Green, primaryCol.Blue, secondaryCol.Red, secondaryCol.Green, secondaryCol.Blue, colWheel, liveryID, fDirt, fHealth, bLocked, bEngineOn, iPaymentsRemaining, iPaymentsMade,
											iPaymentsMissed, fCreditAmount, true, true, expiryTime, fOdometer, false, 0, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, EVehicleTransmissionType.Automatic, 0, vehicleDef.Hash == 0 && !string.IsNullOrEmpty(vehicleDef.AddOnName) ? NAPI.Util.GetHashKey(vehicleDef.AddOnName) : 0).ConfigureAwait(true);

										// Take money from bank, allow negative, since we already did the sql op above
										factionInst.SubtractMoneyAllowNegatve(fDownpayment);

										string strMessage = Helpers.FormatString("{0} bought a {1} {2} for this faction on credit with a downpayment of ${3:0.00} and monthly payment of ${4:0.00}", player.GetCharacterName(ENameType.StaticCharacterName), vehicleDef.Manufacturer, vehicleDef.Name, fDownpayment, fMonthlyPaymentAmount);
										factionInst.SendNotificationToAllManagers(strMessage);

										NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Success);
									}
									else
									{
										NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Faction_MonthlyIncomeTooLowForCredit);
									}
								}
								else
								{
									NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.Faction_CannotAffordDownpaymentForCredit);
								}
							}
						}
						else
						{
							player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Generic Faction Error");
							NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
						}
					}
					else
					{
						player.SendNotification("Vehicle Store", ENotificationIcon.ExclamationSign, "Generic Faction Error");
						NetworkEventSender.SendNetworkEvent_VehicleStore_OnCheckoutResult(player, EPurchaseVehicleResult.GenericFailureCheckNotification);
					}
				}
			}
		}
	}

	// TODO_POST_LAUNCH: More generic location
	public void OnRequestInfo(CPlayer player, EPurchaseAndPaymentMethodsRequestType requestType)
	{
		List<Purchaser> lstPurchasers = new List<Purchaser>
			{
				new Purchaser
				{
					DisplayName = player.GetCharacterName(ENameType.StaticCharacterName),
					ID = -1,
					Type = EPurchaserType.Character
				}
			};

		foreach (CFactionMembership factionMembership in player.GetFactionMemberships())
		{
			if (factionMembership.Manager)
			{
				lstPurchasers.Add(new Purchaser
				{
					DisplayName = factionMembership.Faction.ShortName,
					ID = factionMembership.Faction.FactionID,
					Type = EPurchaserType.Faction
				});
			}
		}

		List<string> lstMethods = new List<string>();

		if (requestType == EPurchaseAndPaymentMethodsRequestType.Vehicle)
		{
			lstMethods.Add(System.Text.RegularExpressions.Regex.Replace(EPaymentMethod.BankBalance.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());
			lstMethods.Add(System.Text.RegularExpressions.Regex.Replace(EPaymentMethod.Credit.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());

			// Do we have any tokens?
			if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.VehicleToken))
			{
				lstMethods.Add("Vehicle Token");
			}
		}
		else if (requestType == EPurchaseAndPaymentMethodsRequestType.Property)
		{
			lstMethods.Add(System.Text.RegularExpressions.Regex.Replace(EPaymentMethod.BankBalance.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());
			lstMethods.Add(System.Text.RegularExpressions.Regex.Replace(EPaymentMethod.Credit.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());

			// Do we have any tokens?
			if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.PropertyToken))
			{
				lstMethods.Add("Property Token");
			}
		}
		else if (requestType == EPurchaseAndPaymentMethodsRequestType.RentalCar)
		{
			lstMethods.Add(System.Text.RegularExpressions.Regex.Replace(EPaymentMethod.BankBalance.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());
		}
		else if (requestType == EPurchaseAndPaymentMethodsRequestType.Bank)
		{
			lstMethods.Add(System.Text.RegularExpressions.Regex.Replace(EPaymentMethod.BankBalance.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());
		}


		switch (requestType)
		{
			case EPurchaseAndPaymentMethodsRequestType.Bank:
				{
					NetworkEventSender.SendNetworkEvent_Banking_RequestInfoResponse(player, lstPurchasers, lstMethods);
					break;
				}

			case EPurchaseAndPaymentMethodsRequestType.Property:
				{
					NetworkEventSender.SendNetworkEvent_PurchaseProperty_RequestInfoResponse(player, lstPurchasers, lstMethods);
					break;
				}

			case EPurchaseAndPaymentMethodsRequestType.RentalCar:
				{
					NetworkEventSender.SendNetworkEvent_VehicleRentalStore_RequestInfoResponse(player, lstPurchasers, lstMethods);
					break;
				}

			case EPurchaseAndPaymentMethodsRequestType.Vehicle:
				{
					NetworkEventSender.SendNetworkEvent_VehicleStore_RequestInfoResponse(player, lstPurchasers, lstMethods);
					break;
				}

			default:
				{
					throw new Exception(Helpers.FormatString("GetPurchaserAndPaymentMethods: Unhandled request type ({0})", requestType));
				}
		}
	}

	private VehicleStoreSpawnPosition GetSpawnPosition(EScriptLocation location, EVehicleStoreType storeType)
	{
		const float fDistThresholdForNearby = 2.0f;
		var spawnArrayToUse = storeType == EVehicleStoreType.Boats ? g_SpawnPositions_Boat : (location == EScriptLocation.Paleto ? g_SpawnPositions_Paleto : g_SpawnPositions_LS);

		// hoping for the best case and that we can exit fast... after one iter of vehicles... otherwise inverting the loops would actually be faster
		foreach (VehicleStoreSpawnPosition iterSpawnPosition in spawnArrayToUse)
		{
			bool bThisPosWasInvalidated = false;

			foreach (var vehicle in NAPI.Pools.GetAllVehicles())
			{
				float fDist = (vehicle.Position - iterSpawnPosition.Position).Length();

				if (fDist <= fDistThresholdForNearby)
				{
					bThisPosWasInvalidated = true;
					break;
				}
			}

			if (!bThisPosWasInvalidated)
			{
				// We are good
				return iterSpawnPosition;
			}
		}

		// We didn't find an empty space :( Let's just spawn ontop, its risky but... tally-ho!
		return spawnArrayToUse[0];
	}

	private class VehicleStoreSpawnPosition
	{
		public VehicleStoreSpawnPosition(Vector3 vecPos, Vector3 vecRot)
		{
			Position = vecPos;
			Rotation = vecRot;
		}

		public Vector3 Position { get; }
		public Vector3 Rotation { get; }
	}

	private readonly VehicleStoreSpawnPosition[] g_SpawnPositions_Paleto = new VehicleStoreSpawnPosition[]
	{
		new VehicleStoreSpawnPosition(new Vector3(-208.1837f, 6220.204f, 31.49133f), new Vector3(0.0f, 0.0f, -134.6f)),
		new VehicleStoreSpawnPosition(new Vector3(-205.7078f, 6222.686f, 31.00783f), new Vector3(0.0f, 0.0f, 224.4302f)),
		new VehicleStoreSpawnPosition(new Vector3(-203.2211f, 6225.033f, 31.00697f), new Vector3(0.0f, 0.0f, 223.4211f)),
		new VehicleStoreSpawnPosition(new Vector3(-201.0052f, 6227.419f, 31.01201f), new Vector3(0.0f, 0.0f, 225.1946f)),
		new VehicleStoreSpawnPosition(new Vector3(-198.768f, 6229.751f, 31.01847f), new Vector3(0.0f, 0.0f, 225.1233f)),
		new VehicleStoreSpawnPosition(new Vector3(-195.9782f, 6231.663f, 31.01569f), new Vector3(0.0f, 0.0f, 225.0401f)),
		new VehicleStoreSpawnPosition(new Vector3(-193.5028f, 6232.747f, 31.0115f), new Vector3(0.0f, 0.0f, 225.1827f)),
		new VehicleStoreSpawnPosition(new Vector3(-188.9651f, 6226.094f, 31.00724f), new Vector3(0.0f, 0.0f, 139.2003f)),
		new VehicleStoreSpawnPosition(new Vector3(-186.4736f, 6223.247f, 31.00723f), new Vector3(0.0f, 0.0f, 136.9547f)),
		new VehicleStoreSpawnPosition(new Vector3(-183.837f, 6220.379f, 31.00849f), new Vector3(0.0f, 0.0f, 134.0986f)),
		new VehicleStoreSpawnPosition(new Vector3(-190.5749f, 6214.584f, 31.0088f), new Vector3(0.0f, 0.0f, 45.94998f)),
		new VehicleStoreSpawnPosition(new Vector3(-201.1123f, 6205.457f, 31.00359f), new Vector3(0.0f, 0.0f, 46.12689f)),
		new VehicleStoreSpawnPosition(new Vector3(-203.8647f, 6203.355f, 31.00334f), new Vector3(0.0f, 0.0f, 42.39114f)),
		new VehicleStoreSpawnPosition(new Vector3(-205.3552f, 6199.54f, 31.00417f), new Vector3(0.0f, 0.0f, 43.1073f))
	};

	private readonly VehicleStoreSpawnPosition[] g_SpawnPositions_LS = new VehicleStoreSpawnPosition[]
	{
		new VehicleStoreSpawnPosition(new Vector3(-61.87835f, -1117.447f, 26.43296f), new Vector3(0.0f, 0.0f, 2.437349)),
		new VehicleStoreSpawnPosition(new Vector3(-58.92167f, -1117.236f, 26.43373f), new Vector3(0.0f, 0.0f, 3.291843)),
		new VehicleStoreSpawnPosition(new Vector3(-56.42322f, -1117.21f, 26.43402f), new Vector3(0.0f, 0.0f, 2.299594)),
		new VehicleStoreSpawnPosition(new Vector3(-53.50222f, -1117.241f, 26.43385f), new Vector3(0.0f, 0.0f, 0.8828678)),
		new VehicleStoreSpawnPosition(new Vector3(-50.74012f, -1117.316f, 26.43352f), new Vector3(0.0f, 0.0f, 358.2441)),
		new VehicleStoreSpawnPosition(new Vector3(-47.8179f, -1117.085f, 26.43342f), new Vector3(0.0f, 0.0f, 359.997)),
		new VehicleStoreSpawnPosition(new Vector3(-44.99456f, -1116.84f, 26.43335f), new Vector3(0.0f, 0.0f, 0.5146134))
	};

	private readonly VehicleStoreSpawnPosition[] g_SpawnPositions_Boat = new VehicleStoreSpawnPosition[]
	{
		new VehicleStoreSpawnPosition(new Vector3(-856.6497f, -1327.855f, 0.0f), new Vector3(0.0f, 0.0f, 110.0f)),
		new VehicleStoreSpawnPosition(new Vector3(-853.7342, -1336.08, 0.0), new Vector3(0.0f, 0.0f, 110.0f)),
		new VehicleStoreSpawnPosition(new Vector3(-851.9915, -1345.226, 0.0), new Vector3(0.0f, 0.0f, 110.0f)),
		new VehicleStoreSpawnPosition(new Vector3(-849.7295, -1353.724, 0.0), new Vector3(0.0f, 0.0f, 110.0f)),
		new VehicleStoreSpawnPosition(new Vector3(-845.5875, -1362.183, 0.0), new Vector3(0.0f, 0.0f, 110.0f)),
		new VehicleStoreSpawnPosition(new Vector3(-840.653, -1371.037, 0.0), new Vector3(0.0f, 0.0f, 110.0f)),
	};
}