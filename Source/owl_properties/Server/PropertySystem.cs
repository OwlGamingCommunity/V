using GTANetworkAPI;
using Logging;
using System;
using System.Collections.Generic;
using Dimension = System.UInt32;

public class PropertySystem
{
	private const int TWENTYFOURHOURS_MS = 86400000;
	private PropertyXPService _propertyXPService;

	private Commands m_PropertyCommands = new Commands();

	public PropertySystem()
	{
		NetworkEvents.RequestEnterInterior += OnRequestEnterInterior;
		NetworkEvents.RequestExitInterior += OnRequestExitInterior;
		NetworkEvents.RequestExitInteriorForced += OnRequestExitInteriorForced;
		NetworkEvents.PurchaseProperty_OnPreview += OnPreview;
		NetworkEvents.PurchaseProperty_OnCheckout += OnCheckout;

		NetworkEvents.RequestEnterElevator += OnRequestEnterElevator;
		NetworkEvents.RequestExitElevator += OnRequestExitElevator;

		LoadAllProperties();
		LoadAllElevators();
		_propertyXPService = new PropertyXPService();
		MainThreadTimerPool.CreateGlobalTimer(RunXPCheck, TWENTYFOURHOURS_MS);
	}

	private void RunXPCheck(params object[] p)
	{
		NAPI.Util.ConsoleOutput("[PROPERTY XP] Started Timer");
		_propertyXPService.RunXPCheck().ConfigureAwait(false);
	}

	public void LoadAllProperties()
	{
		DateTime StartTime = DateTime.Now;
		Database.Functions.Properties.Get(properties =>
		{
			foreach (var property in properties)
			{
				CPropertyInstance cProperty = PropertyPool.Add(property);
				cProperty.Inventory.CopyInventory(property.Inventory);
			}

			NAPI.Util.ConsoleOutput("[PROPERTIES] Loaded {0} Properties in {1} seconds!", properties.Count, (Int64)(DateTime.Now - StartTime).TotalSeconds);
			NAPI.Task.Run(() =>
			{
				RunXPCheck(null);
			});
		});
	}

	public async void LoadAllElevators()
	{
		List<CDatabaseStructureElevator> lstElevators = await Database.LegacyFunctions.LoadAllElevators().ConfigureAwait(true);
		NAPI.Task.Run(() =>
		{
			foreach (var elevator in lstElevators)
			{
				ElevatorPool.CreateElevator(elevator.elevatorID, elevator.entrancePosition, elevator.exitPosition, elevator.exitDimension, elevator.startDimension, elevator.isCarElevator, elevator.startRotation, elevator.endRotation, elevator.elevatorName, false);
			}
		});

		NAPI.Util.ConsoleOutput("[PROPERTIES] Loaded {0} Elevators!", lstElevators.Count);
	}

	public void OnPreview(CPlayer player, Int64 PropertyID)
	{
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(PropertyID);

		if (propertyInst != null)
		{
			// Check distance, don't alert use if they're out of range
			if (player.IsWithinDistanceOf(propertyInst.Model.EntrancePosition, 3.0f, propertyInst.Model.EntranceDimension))
			{
				if (player.IsPreviewingProperty)
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You are already previewing another property.", null);
				}
				else if (propertyInst.Model.EntranceType == EPropertyEntranceType.World)
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "This interior is a world interior. You can walk inside to preview it.", null);
				}
				else
				{

					if (propertyInst.Model.State == EPropertyState.AvailableToBuy || propertyInst.Model.State == EPropertyState.AvailableToRent || propertyInst.Model.State == EPropertyState.AvailableToBuy_AlwaysEnterable || propertyInst.Model.State == EPropertyState.AvailableToRent_AlwaysEnterable)
					{
						player.OnPreviewProperty(propertyInst);
					}
				}
			}
		}
	}

	private bool validateCreditParameters(CPropertyInstance property, float fDownpayment, int numMonthsForPaymentPlan, out string error)
	{
		if (numMonthsForPaymentPlan < 2)
		{
			error = "Your payment plan must be at least 2 months.";
			return false;
		}
		if (numMonthsForPaymentPlan > 200)
		{
			error = "Your payment plan must be 200 months or less.";
			return false;
		}
		if (fDownpayment < (property.Model.BuyPrice * 0.05f))
		{
			error = "Your down payment must be at least 5% of the value.";
			return false;
		}

		error = "";
		return true;
	}

	public void OnCheckout(CPlayer player, Int64 PropertyID, EPurchaserType purchaserType, long purchaserID,
		EPaymentMethod paymentMethod, float fDownpayment, int numMonthsForPaymentPlan)
	{
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(PropertyID);
		if (propertyInst == null || !player.IsWithinDistanceOf(propertyInst.Model.EntrancePosition, 3.0f, propertyInst.Model.EntranceDimension))
		{
			return;
		}

		if (paymentMethod == EPaymentMethod.Credit && !validateCreditParameters(propertyInst, fDownpayment, numMonthsForPaymentPlan, out string error))
		{
			player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, error, null);
			return;
		}

		if (purchaserType == EPurchaserType.Character)
		{
			if (paymentMethod == EPaymentMethod.Token)
			{
				if (!player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.PropertyToken))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You cannot afford this property.", null);
					return;
				}

				// Only allow cheap houses
				if (propertyInst.Model.BuyPrice >= 50000.0f)
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign,
						"Property Tokens can only be used for properties which cost 50k or less.");
					return;
				}

				// Does the player have space for key?
				CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, PropertyID);
				if (!player.Inventory.CanGiveItem(itemInstDef, out _, out string strUserFriendlyMessage))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign,
						"You cannot receive the keys to this property:<br>{0}", strUserFriendlyMessage);
					return;
				}

				player.DonationInventory.RemoveTokenOfTypeForActiveCharacter(EDonationEffect.PropertyToken);
				player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing,
					EItemID.None, bItemGranted =>
					{
						if (bItemGranted)
						{
							player.SendNotification("Realtor", ENotificationIcon.ThumbsUp,
								"Congratulations on purchasing {0}.", propertyInst.Model.Name);
							propertyInst.OnPurchasedPlayer(player, 0, 0.0f, true);
							Log.CreateLog(player.ActiveCharacterDatabaseID, EOriginType.Character,
								ELogType.PropertyRelated, new List<CBaseEntity>() { propertyInst },
								"PROPERTY BOUGHT WITH TOKEN.");
						}
					});
			}
			if (paymentMethod == EPaymentMethod.BankBalance)
			{
				float fTotalPrice = propertyInst.Model.BuyPrice;
				if (!player.CanPlayerAffordBankCost(fTotalPrice))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You cannot afford this property.", null);
					return;
				}

				// Does the player have space for key?
				CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, PropertyID);
				if (!player.Inventory.CanGiveItem(itemInstDef, out _, out string strUserFriendlyMessage))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign,
						"You cannot receive the keys to this property:<br>{0}", strUserFriendlyMessage);
					return;
				}

				// Take money from bank, otherwise take from player and allow negative (this fixes race conditions)
				if (!player.SubtractBankBalanceIfCanAfford(fTotalPrice, PlayerMoneyModificationReason.BuyProperty_Outright))
				{
					player.SubtractMoneyAllowNegative(fTotalPrice, PlayerMoneyModificationReason.BuyProperty_Outright);
				}

				player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing,
					EItemID.None, bItemGranted =>
					{
						if (!bItemGranted)
						{
							player.SendNotification("Property Purchase Failed", ENotificationIcon.ExclamationSign, "Could not receive key to this property.");
							player.AddBankMoney(fTotalPrice, PlayerMoneyModificationReason.BuyProperty_Outright);
							return;
						}

						player.SendNotification("Realtor", ENotificationIcon.ThumbsUp,
							"Congratulations on purchasing {0}.", propertyInst.Model.Name);
						Log.CreateLog(player.ActiveCharacterDatabaseID, EOriginType.Character,
							ELogType.PropertyRelated, new List<CBaseEntity>() { propertyInst },
							"PROPERTY BOUGHT WITH BANK BALANCE.");
						propertyInst.OnPurchasedPlayer(player, 0, 0.0f);
					});
			}
			else if (paymentMethod == EPaymentMethod.Credit)
			{
				float fAmountMinusDownpayment = (propertyInst.Model.BuyPrice - fDownpayment);
				float fCreditAmount = fAmountMinusDownpayment;
				float fInterest = (fAmountMinusDownpayment * Taxation.GetPaymentPlanInterestPercent());
				float fMonthlyPaymentAmount = (fCreditAmount / numMonthsForPaymentPlan) + (fInterest / numMonthsForPaymentPlan);

				if (!player.CanPlayerAffordBankCost(fDownpayment))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You cannot afford the down payment for this property.", null);
					return;
				}

				if (!player.CanPlayerAffordMonthlyExpense(fMonthlyPaymentAmount))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You cannot afford the monthly payment for this property.", null);
					return;
				}

				// Does the player have space for key?
				CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, PropertyID);
				if (!player.Inventory.CanGiveItem(itemInstDef, out _, out string strUserFriendlyMessage))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You cannot receive the keys to this property:<br>{0}", strUserFriendlyMessage);
					return;
				}

				// Take downpayment from bank, otherwise take from player and allow negative (this fixes race conditions)
				if (!player.SubtractBankBalanceIfCanAfford(fDownpayment, PlayerMoneyModificationReason.BuyProperty_Downpayment))
				{
					player.SubtractMoneyAllowNegative(fDownpayment, PlayerMoneyModificationReason.BuyProperty_Downpayment);
				}

				player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing,
					EItemID.None, bItemGranted =>
					{
						if (bItemGranted)
						{
							player.SendNotification("Realtor", ENotificationIcon.ThumbsUp, "Congratulations on purchasing {0}.", propertyInst.Model.Name);
							Log.CreateLog(player.ActiveCharacterDatabaseID, EOriginType.Character, ELogType.PropertyRelated, new List<CBaseEntity>() { propertyInst }, "PROPERTY BOUGHT ON CREDIT.");
							propertyInst.OnPurchasedPlayer(player, numMonthsForPaymentPlan, fCreditAmount);
						}
					});
			}
		}
		else if (purchaserType == EPurchaserType.Faction)
		{
			if (paymentMethod == EPaymentMethod.Token)
			{
				player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "Property Tokens can not be used for Faction Purchases.");
				return;
			}

			CFaction factionInst = FactionPool.GetFactionFromID(purchaserID);
			if (factionInst == null)
			{
				player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "Faction was not found.", null);
				return;
			}

			CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(factionInst);
			if (factionMembership == null || !factionMembership.Manager)
			{
				player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You are not a manager of this faction.", null);
				return;
			}

			if (paymentMethod == EPaymentMethod.BankBalance)
			{
				// TODO_POST_LAUNCH: Should factions pay tax?
				float fTotalPrice = propertyInst.Model.BuyPrice;

				if (!(factionInst.Money >= fTotalPrice))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "Your faction cannot afford this property.", null);
					return;
				}

				// Take money from bank, allow negative
				factionInst.SubtractMoneyAllowNegatve(fTotalPrice);

				string strMessage = Helpers.FormatString("{0} bought a property for this faction for ${1:0.00}", player.GetCharacterName(ENameType.StaticCharacterName), fTotalPrice);
				factionInst.SendNotificationToAllManagers(strMessage);
				Log.CreateLog(player.ActiveCharacterDatabaseID, EOriginType.Character, ELogType.PropertyRelated, new List<CBaseEntity>() { propertyInst }, $"PROPERTY BOUGHT BY FACTION ({factionInst.FactionID}) WITH BANK.");
				propertyInst.OnPurchasedFaction(factionInst, 0, 0.0f);
			}
			else if (paymentMethod == EPaymentMethod.Credit)
			{
				float fAmountMinusDownpayment = (propertyInst.Model.BuyPrice - fDownpayment);
				float fCreditAmount = fAmountMinusDownpayment;
				float fInterest = (fAmountMinusDownpayment * Taxation.GetPaymentPlanInterestPercent());
				float fMonthlyPaymentAmount = (fCreditAmount / numMonthsForPaymentPlan) + (fInterest / numMonthsForPaymentPlan);

				if (!factionInst.CanAffordMonthlyExpense(fMonthlyPaymentAmount))
				{
					player.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "Your faction cannot afford this property.", null);
					return;
				}

				// Take money from bank, allow negative
				factionInst.SubtractMoneyAllowNegatve(fDownpayment);

				string strMessage = Helpers.FormatString(
					"{0} bought a property for this faction on credit with a downpayment of ${1:0.00} and monthly payment of ${2:0.00}",
					player.GetCharacterName(ENameType.StaticCharacterName), fDownpayment, fMonthlyPaymentAmount);
				factionInst.SendNotificationToAllManagers(strMessage);
				Log.CreateLog(player.ActiveCharacterDatabaseID, EOriginType.Character,
					ELogType.PropertyRelated, new List<CBaseEntity>() { propertyInst },
					$"PROPERTY BOUGHT BY FACTION ({factionInst.FactionID}) WITH CREDIT.");
				propertyInst.OnPurchasedFaction(factionInst, numMonthsForPaymentPlan, fCreditAmount);
			}
		}
	}

	public void OnRequestEnterInterior(CPlayer player, Int64 PropertyID)
	{
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(PropertyID);

		if (propertyInst != null && propertyInst.Model.EntranceType != EPropertyEntranceType.World)
		{
			if (player.IsWithinDistanceOf(propertyInst.Model.EntrancePosition, 5.0f, propertyInst.Model.EntranceDimension) && !player.Client.Dead)
			{
				if (!propertyInst.Model.Locked)
				{
					//HelperFunctions.Chat.SendMeMessage(player, "pulls on the handle to open the door and enters the property.");

					player.OnEnterProperty(propertyInst);
				}
				else
				{
					HelperFunctions.Chat.SendAmeMessage(player, "pulls on the handle to open the door to find that it is locked.");
				}
			}
		}
	}

	public void OnRequestExitInterior(CPlayer player)
	{
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(player.Client.Dimension);

		if (propertyInst != null && propertyInst.Model.EntranceType != EPropertyEntranceType.World)
		{
			if (player.IsWithinDistanceOf(propertyInst.Model.ExitPosition, 5.0f, (Dimension)propertyInst.Model.Id) && !player.Client.Dead)
			{
				// TODO: should we check they are actually previewing THIS specific property?
				if (!propertyInst.Model.Locked || player.IsPreviewingProperty)
				{
					//HelperFunctions.Chat.SendMeMessage(player, "pulls on the handle to open the door and exits the property.");

					player.OnExitProperty(propertyInst);
				}
				else
				{
					HelperFunctions.Chat.SendAmeMessage(player, "pulls on the handle to open the door to find that it is locked.");
				}
			}
		}
	}

	public void OnRequestExitInteriorForced(CPlayer player)
	{
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(player.Client.Dimension);

		if (propertyInst != null && propertyInst.Model.EntranceType != EPropertyEntranceType.World)
		{
			player.OnExitProperty(propertyInst);
		}
	}

	public void OnRequestEnterElevator(CPlayer player, Int64 ElevatorID)
	{
		CElevatorInstance elevatorInst = ElevatorPool.GetElevatorInstanceFromID(ElevatorID);

		if (elevatorInst != null)
		{
			if (player.IsWithinDistanceOf(elevatorInst.EntrancePos, 5.0f, elevatorInst.StartDim))
			{
				player.OnEnterElevator(elevatorInst);
			}
		}

	}

	public void OnRequestExitElevator(CPlayer player, Int64 ElevatorID)
	{
		CElevatorInstance elevatorInst = ElevatorPool.GetElevatorInstanceFromID(ElevatorID);

		if (elevatorInst != null)
		{
			if (player.IsWithinDistanceOf(elevatorInst.ExitPos, 5.0f, elevatorInst.ExitDim))
			{
				player.OnExitElevator(elevatorInst);
			}
		}

	}
}