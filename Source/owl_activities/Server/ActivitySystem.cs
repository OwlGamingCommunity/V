using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class ActivitySystem
{
	private readonly Dictionary<EActivityType, ActivityManager> m_dictActivityManagers = new Dictionary<EActivityType, ActivityManager>()
	{
		{ EActivityType.Blackjack, new BlackjackActivityManager() }
	};

	public ActivitySystem()
	{
		NetworkEvents.RequestStartActivity += OnRequestStartActivity;
		NetworkEvents.RequestStopActivity += OnRequestStopActivity;

		NetworkEvents.ActivityRequestInteract += ActivityRequestInteract;

		NetworkEvents.CharacterChangeRequested += KillPlayerActivity;
		NetworkEvents.OnPlayerDisconnected += (CPlayer player, DisconnectionType type, string reason) =>
		{
			KillPlayerActivity(player);
		};

		NetworkEvents.RequestSubscribeActivity += OnRequestSubscribeActivity;
		NetworkEvents.RequestUnsubscribeActivity += OnRequestUnsubscribeActivity;

		NetworkEvents.OnPropertyFurnitureInstanceCreated += OnFurnitureCreated;
		NetworkEvents.OnPropertyFurnitureInstanceDestroyed += OnFurnitureDestroyed;

		CommandManager.RegisterCommand("danielslovesblackjack", "", new Action<CPlayer, CVehicle>(DanielsBlackjack), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);

		RageEvents.RAGE_OnUpdate += OnUpdate;

		NetworkEvents.ChipManagement_Buy += OnBuyChips;
		NetworkEvents.ChipManagement_Sell += OnSellChips;

		NetworkEvents.CasinoManagement_Add += Management_OnAddCurrency;
		NetworkEvents.CasinoManagement_Take += Management_OnTakeCurrency;

		NetworkEvents.ChipManagement_Buy_GetDetails += OnBuyChips_GetDetails;
		NetworkEvents.ChipManagement_Sell_GetDetails += OnSellChips_GetDetails;
		NetworkEvents.CasinoManagement_GetDetails += OnCasinoManagement_GetDetails;
	}

	private void KillPlayerActivity(CPlayer player)
	{
		foreach (EActivityType activityType in Enum.GetValues(typeof(EActivityType)))
		{
			if (m_dictActivityManagers.ContainsKey(activityType))
			{
				ActivityManager activityMgr = m_dictActivityManagers[activityType];
				activityMgr.KillPlayerActivity(player);
			}
		}
	}

	private void OnBuyChips_GetDetails(CPlayer SenderPlayer)
	{
		UInt32 totalChips = SenderPlayer.Inventory.GetTotalStackSizeOfAllInstancesOfItemID(EItemID.CASINO_CHIP_BUCKET);
		NetworkEventSender.SendNetworkEvent_ChipManagement_Buy_GotDetails(SenderPlayer, Convert.ToInt32(totalChips));
	}

	private void OnSellChips_GetDetails(CPlayer SenderPlayer)
	{
		UInt32 totalChips = SenderPlayer.Inventory.GetTotalStackSizeOfAllInstancesOfItemID(EItemID.CASINO_CHIP_BUCKET);
		NetworkEventSender.SendNetworkEvent_ChipManagement_Sell_GotDetails(SenderPlayer, Convert.ToInt32(totalChips));
	}

	public static CPropertyFurnitureInstance GetFurnitureItemAssociatedWithActivityFromCurrentProperty(CPlayer a_Player, Int64 uniqueActivityIdentifier)
	{
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(a_Player.Client.Dimension);

		if (propertyInst != null)
		{
			return propertyInst.GetFurnitureItemFromDBID(uniqueActivityIdentifier);
		}

		return null;
	}

	private void OnCasinoManagement_GetDetails(CPlayer SenderPlayer, Int64 uniqueActivityIdentifier)
	{
		bool bIsManager = IsPlayerManagerOfCurrentProperty(SenderPlayer);

		if (bIsManager)
		{
			CPropertyFurnitureInstance furnitureItem = GetFurnitureItemAssociatedWithActivityFromCurrentProperty(SenderPlayer, uniqueActivityIdentifier);
			CItemValueFurniture furnitureValue = (CItemValueFurniture)furnitureItem.Value;

			NetworkEventSender.SendNetworkEvent_CasinoManagement_GotDetails(SenderPlayer, furnitureValue.ActivityCurrency);
		}
	}

	private void OnBuyChips(CPlayer SenderPlayer, int amount)
	{
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[EItemID.CASINO_CHIP_BUCKET];
		float pricePerChip = itemDef.GetCostIgnoreGenericItems();

		float price = amount * pricePerChip;

		bool bSuccess = false;
		if (SenderPlayer.SubtractMoney(price, PlayerMoneyModificationReason.Activity_BuyCasinoChips))
		{
			bSuccess = true;
		}
		else if (SenderPlayer.SubtractBankBalanceIfCanAfford(price, PlayerMoneyModificationReason.Activity_BuyCasinoChips))
		{
			bSuccess = true;
		}
		else
		{
			bSuccess = false;
		}

		if (bSuccess)
		{
			CItemInstanceDef casinoChipsDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.CASINO_CHIP_BUCKET, 0.0f, Convert.ToUInt32(amount));
			bool bCanGiveItems = SenderPlayer.Inventory.CanGiveItem(casinoChipsDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage, true);
			if (bCanGiveItems)
			{
				SenderPlayer.Inventory.AddItemToNextFreeSuitableSlot(casinoChipsDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						SenderPlayer.SendNotification("Buy Casino Chips", ENotificationIcon.InfoSign, "You have purchased {0} casino chips for ${1:0.00}", amount, price);
					}
				});
			}
			else
			{
				SenderPlayer.SendNotification("Buy Casino Chips", ENotificationIcon.ExclamationSign, "You do not have space to receive any casino chips");
			}
		}
		else
		{
			SenderPlayer.SendNotification("Buy Casino Chips", ENotificationIcon.ExclamationSign, "You cannot afford {0} chips (Price: ${1:0.00})", amount, price);
		}
	}

	private void OnSellChips(CPlayer SenderPlayer, int amount)
	{
		UInt32 totalChips = SenderPlayer.Inventory.GetTotalStackSizeOfAllInstancesOfItemID(EItemID.CASINO_CHIP_BUCKET);
		if (totalChips >= amount)
		{
			bool bRemoved = SenderPlayer.Inventory.DecrementStackSizeOverMultipleInstances(EItemID.CASINO_CHIP_BUCKET, Convert.ToUInt32(amount));

			if (bRemoved)
			{
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[EItemID.CASINO_CHIP_BUCKET];
				float pricePerChip = itemDef.GetCostIgnoreGenericItems();
				float price = amount * pricePerChip;

				SenderPlayer.AddMoney(price, PlayerMoneyModificationReason.Activity_SellCasinoChips);

				SenderPlayer.SendNotification("Sell Casino Chips", ENotificationIcon.InfoSign, "You have sold {0} casino chips for ${1:0.00}", amount, price);
			}
		}
		else
		{
			SenderPlayer.SendNotification("Sell Casino Chips", ENotificationIcon.ExclamationSign, "You cannot sell {0} chips since you only have {1}", amount, totalChips);
		}
	}

	private void Management_OnAddCurrency(CPlayer SenderPlayer, Int64 uniqueActivityIdentifier, int amount)
	{
		CPropertyFurnitureInstance furnInstance = GetFurnitureItemAssociatedWithActivityFromCurrentProperty(SenderPlayer, uniqueActivityIdentifier);

		if (furnInstance != null && amount > 0)
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[EItemID.CASINO_CHIP_BUCKET];
			float pricePerChip = itemDef.GetCostIgnoreGenericItems();

			float price = amount * pricePerChip;

			bool bSuccess = false;
			if (SenderPlayer.SubtractMoney(price, PlayerMoneyModificationReason.ActivityManagement_AddCurrency))
			{
				bSuccess = true;
			}
			else if (SenderPlayer.SubtractBankBalanceIfCanAfford(price, PlayerMoneyModificationReason.ActivityManagement_AddCurrency))
			{
				bSuccess = true;
			}
			else
			{
				bSuccess = false;
			}

			if (bSuccess)
			{
				// add the stock to the table
				CItemValueFurniture furnValue = (CItemValueFurniture)furnInstance.Value;
				furnValue.ActivityCurrency += amount;
				Database.Functions.Items.SavePropertyFurnitureValue(furnInstance);
				SenderPlayer.SendNotification("Management", ENotificationIcon.InfoSign, "You have added ${0:0.00} to the table.", price);
			}
			else
			{
				SenderPlayer.SendNotification("Management", ENotificationIcon.ExclamationSign, "You cannot afford ${0:0.00}", price);
			}
		}
	}

	private void Management_OnTakeCurrency(CPlayer SenderPlayer, Int64 uniqueActivityIdentifier, int amount)
	{
		CPropertyFurnitureInstance furnInstance = GetFurnitureItemAssociatedWithActivityFromCurrentProperty(SenderPlayer, uniqueActivityIdentifier);

		if (furnInstance != null && amount > 0)
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[EItemID.CASINO_CHIP_BUCKET];
			float pricePerChip = itemDef.GetCostIgnoreGenericItems();

			float price = amount * pricePerChip;

			CItemValueFurniture furnValue = (CItemValueFurniture)furnInstance.Value;
			if (furnValue.ActivityCurrency >= price)
			{
				furnValue.ActivityCurrency -= amount;
				Database.Functions.Items.SavePropertyFurnitureValue(furnInstance);

				SenderPlayer.AddMoney(price, PlayerMoneyModificationReason.ActivityManagement_TakeCurrency);
				SenderPlayer.SendNotification("Management", ENotificationIcon.InfoSign, "You have withdrawn ${0:0.00} from the table to your person.", price);
			}
			else
			{
				SenderPlayer.SendNotification("Management", ENotificationIcon.ExclamationSign, "The table does not have ${0:0.00} - it only has ${1:0.00}", price, furnValue.ActivityCurrency);
			}
		}
	}

	private void DanielsBlackjack(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderPlayer.IsDaniels())
		{
			CItemInstanceDef newFurnItemDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FURNITURE, 1045);
			bool bCanGiveItems = SenderPlayer.Inventory.CanGiveItem(newFurnItemDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage, true);
			if (bCanGiveItems)
			{
				SenderPlayer.Inventory.AddItemToNextFreeSuitableSlot(newFurnItemDef, EShowInventoryAction.DoNothing, EItemID.None, null);
			}
		}
	}

	private void OnUpdate()
	{
		foreach (ActivityManager mgr in m_dictActivityManagers.Values)
		{
			mgr.OnUpdate();
		}
	}

	private void OnFurnitureCreated(CPropertyFurnitureInstance furnInst)
	{
		CFurnitureDefinition furnDef = FurnitureDefinitions.g_FurnitureDefinitions[furnInst.FurnitureID];
		if (furnDef != null)
		{
			if (furnDef.Activity != EActivityType.None)
			{
				ActivityManager activityMgr = m_dictActivityManagers[furnDef.Activity];
				activityMgr.CreateNewInstance(furnInst.DBID);
			}
		}
	}

	private void OnFurnitureDestroyed(CPropertyFurnitureInstance furnInst)
	{
		CFurnitureDefinition furnDef = FurnitureDefinitions.g_FurnitureDefinitions[furnInst.FurnitureID];
		if (furnDef != null)
		{
			if (furnDef.Activity != EActivityType.None)
			{
				ActivityManager activityMgr = m_dictActivityManagers[furnDef.Activity];
				activityMgr.RemoveActivityInstance(furnInst.DBID);
			}
		}
	}

	private void OnRequestStartActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType)
	{
		ActivityManager activityMgr = m_dictActivityManagers[activityType];
		activityMgr.StartActivity(a_Player, uniqueActivityIdentifier);
	}

	private void OnRequestStopActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType)
	{
		ActivityManager activityMgr = m_dictActivityManagers[activityType];
		activityMgr.StopActivity(a_Player, uniqueActivityIdentifier);
	}

	private void OnRequestSubscribeActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType)
	{
		ActivityManager activityMgr = m_dictActivityManagers[activityType];
		activityMgr.SubscribeActivity(a_Player, uniqueActivityIdentifier);
	}

	private void OnRequestUnsubscribeActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType)
	{
		ActivityManager activityMgr = m_dictActivityManagers[activityType];
		activityMgr.UnsubscribeActivity(a_Player, uniqueActivityIdentifier);
	}

	private bool IsPlayerManagerOfCurrentProperty(CPlayer a_Player)
	{
		bool bIsManager = false;

		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(a_Player.Client.Dimension);

		if (propertyInst != null)
		{
			// do we have a key to the property?
			bool bHasFactionForProperty = propertyInst.IsPropertyForAnyPlayerFaction(a_Player, false);
			if (bHasFactionForProperty || propertyInst.HasKeys(a_Player))
			{
				bIsManager = true;
			}
		}

		return bIsManager;
	}

	private void ActivityRequestInteract(CPlayer a_Player)
	{
		bool bIsManager = IsPlayerManagerOfCurrentProperty(a_Player);
		NetworkEventSender.SendNetworkEvent_ActivityRequestInteract_Response(a_Player, bIsManager);
	}
}