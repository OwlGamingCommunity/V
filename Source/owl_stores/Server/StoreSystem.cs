using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

public class StoreSystem
{
	private DancerSystem m_DancerSystem = new DancerSystem();
	private PlasticSurgeon m_PlasticSurgeon = new PlasticSurgeon();
	private TattooArtist m_TattooArtist = new TattooArtist();

	public StoreSystem()
	{
		NetworkEvents.GetStoreInfo += OnRequestStoreInfo;
		NetworkEvents.OnStoreCheckout += OnStoreCheckout;

		NetworkEvents.EnterOutfitEditor += OnEnterOutfitEditor;
		NetworkEvents.RequestOutfitList += OnRequestOutfitList;

		NetworkEvents.EnterClothingStore += OnEnterClothingStore;
		NetworkEvents.ClothingStore_CalculatePrice += ClothingStore_CalculatePrice;
		NetworkEvents.ClothingStore_OnCheckout += OnClothingStore_OnCheckout;

		NetworkEvents.EnterBarberShop += OnEnterBarberShop;
		NetworkEvents.BarberShop_CalculatePrice += BarberShop_CalculatePrice;
		NetworkEvents.BarberShop_OnCheckout += OnBarberShop_OnCheckout;

		NetworkEvents.ExitGenericCharacterCustomization += OnExitGenericCharacterCustomization;

		NetworkEvents.FurnitureStore_OnCheckout += FurnitureStore_OnCheckout;
		NetworkEvents.Store_InitiateRobbery += InitiateStoreRobbery;
		NetworkEvents.Store_CancelRobbery += CancelStoreRobbery;

		NetworkEvents.OutfitEditor_CreateOrUpdateOutfit += OnCreateOrUpdateOutfit;
		NetworkEvents.OutfitEditor_DeleteOutfit += OnDeleteOutfit;

		CommandManager.RegisterCommand("recentrobberies", "Lists the recent robberies that have occured.", new Action<CPlayer, CVehicle>(RecentRobberies), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		List<CDatabaseStructureStore> lstStores = Database.LegacyFunctions.LoadAllStores().Result;
		NAPI.Task.Run(async () =>
		{
			foreach (var store in lstStores)
			{
				await StorePool.CreateStore(store.storeID, store.vecPos, store.fRotZ, store.storeType, store.Dimension, store.parentPropertyID, store.lastRobbedAt, false).ConfigureAwait(true);
			}
		});
		NAPI.Util.ConsoleOutput("[STORES] Loaded {0} Stores!", lstStores.Count);
	}

	private struct SStoreCheckoutItem
	{
#pragma warning disable 0649
		public int index;
		public int count;
#pragma warning restore 0649
	}

	private void OnCreateOrUpdateOutfit(CPlayer player, string strName, Dictionary<int, Int64> ClothingItemIDs, Dictionary<int, Int64> PropItemIDs, EntityDatabaseID outfitID, bool a_bHideHair)
	{
		if (outfitID == -1)
		{
			// give outfit item
			CItemValueOutfit outfitValue = new CItemValueOutfit(strName, ClothingItemIDs, PropItemIDs, a_bHideHair);
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.OUTFIT, outfitValue);
			player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
		}
		else
		{
			CItemInstanceDef outfitDef = player.Inventory.GetItemFromDBID(outfitID);
			if (outfitDef != null)
			{
				// update the value and save
				CItemValueOutfit outfitValue = outfitDef.GetValueData<CItemValueOutfit>();
				outfitValue.Name = strName;
				outfitValue.Clothes = ClothingItemIDs;
				outfitValue.Props = PropItemIDs;
				outfitValue.HideHair = a_bHideHair;

				// Is it currently active? if so use it so we 'update our clothing'
				if (outfitValue.IsActive)
				{
					player.ActivateOutfit(outfitDef);
				}

				Database.Functions.Items.SaveItemValue(outfitDef);
			}
		}

		player.CheckForOutfitAchievements();
	}

	private void OnDeleteOutfit(CPlayer player, EntityDatabaseID outfitID)
	{
		CItemInstanceDef outfitDef = player.Inventory.GetItemFromDBID(outfitID);
		if (outfitDef != null)
		{
			player.Inventory.RemoveItem(outfitDef);
		}

		player.CheckForOutfitAchievements();
	}

	private void FurnitureStore_OnCheckout(CPlayer player, EntityDatabaseID storeID, uint furnitureIndex)
	{
		EStoreCheckoutResult result = EStoreCheckoutResult.FailedPartial;

		if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureIndex))
		{
			CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[furnitureIndex];
			if (furnitureDef != null)
			{
				// calculate cost
				float fPriceForItem = furnitureDef.Price;
				float fTotalCost = 0.0f;
				float fStateSalesTax = Taxation.GetSalesTax();

				// Do we have a donator perk for no sales tax?
				if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.NoStoreSalesTax))
				{
					fStateSalesTax = 0.0f;
				}

				// Do we have a donator perk active for 20% discount?
				if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.DiscountCard20Percent))
				{
					float fDiscount = 0.20f * fPriceForItem;
					fPriceForItem -= fDiscount;
				}

				fTotalCost += fPriceForItem + (fStateSalesTax * fPriceForItem);

				// Do we have enough space?
				CItemInstanceDef newFurnItemDef = CItemInstanceDef.FromDefaultValue(EItemID.FURNITURE, furnitureDef.ID);
				bool bCanGiveItems = player.Inventory.CanGiveItem(newFurnItemDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage, true);
				if (bCanGiveItems)
				{
					// TODO: Credit cards or debit cards
					// Can we afford it?
					if (player.SubtractMoney(fTotalCost, PlayerMoneyModificationReason.FurnitureStoreCheckout))
					{
						StoreSystem.HandleStoreTransactionOwnerShare(storeID, fTotalCost);

						// give generic furniture item

						player.Inventory.AddItemToNextFreeSuitableSlot(newFurnItemDef, EShowInventoryAction.DoNothing, EItemID.None, null);

						result = EStoreCheckoutResult.Success;

						player.SendNotification("Furniture Store", ENotificationIcon.InfoSign, "Your piece of furniture was purchased and can be found in your inventory.");
					}
					else
					{
						result = EStoreCheckoutResult.CannotAfford;
					}
				}
				else
				{
					result = EStoreCheckoutResult.FailedPartial; // Not enough space in this case
				}
			}
		}

		NetworkEventSender.SendNetworkEvent_FurnitureStore_OnCheckoutResult(player, result);
	}

	public void OnStoreCheckout(CPlayer player, EntityDatabaseID storeID, string strCartContents)
	{
		EStoreCheckoutResult result = EStoreCheckoutResult.FailedPartial;

		SStoreCheckoutItem[] cartContents = JsonConvert.DeserializeObject<SStoreCheckoutItem[]>(strCartContents);

		// Note: These dont have the correct value or anything, just the correct item ID
		List<CItemInstanceDef> itemIDs = new List<CItemInstanceDef>();
		float fTotalCost = 0.0f;
		float fStateSalesTax = Taxation.GetSalesTax();

		CStoreInstance storeInst = StorePool.GetInstanceFromID(storeID);
		if (storeInst != null)
		{
			bool bIsLegalItem = true;
			bool bIsCivilianFirearm = true;

			// Can we access this store type?
			bool bCanAccessStore = true;
			EStoreType storetype = storeInst.GetStoreType();
			if (storetype == EStoreType.Guns_Legal_Handguns || storetype == EStoreType.Guns_Legal_Shotguns)
			{
				bIsCivilianFirearm = true;
				if (!player.HasHandgunFirearmLicense())
				{
					result = EStoreCheckoutResult.NoHandgunLicense;
					bCanAccessStore = false;
				}
			}
			else if (storetype == EStoreType.Armor)
			{
				if (!player.HasHandgunFirearmLicense())
				{
					result = EStoreCheckoutResult.NoHandgunLicense;
					bCanAccessStore = false;
				}
			}
			else if (storetype == EStoreType.Guns_Legal_Rifles || storetype == EStoreType.Guns_Legal_SMG)
			{
				bIsCivilianFirearm = true;
				if (!player.HasLargeFirearmLicense())
				{
					result = EStoreCheckoutResult.NoLonggunLicense;
					bCanAccessStore = false;
				}
			}
			else if (storetype == EStoreType.Guns_Illegal_Handguns
				|| storetype == EStoreType.Guns_Illegal_Shotguns
				|| storetype == EStoreType.Guns_Illegal_Rifles
				|| storetype == EStoreType.Guns_Illegal_Heavy
				|| storetype == EStoreType.Guns_Illegal_Snipers
				|| storetype == EStoreType.Guns_Illegal_SMG)
			{
				bIsCivilianFirearm = false;
				bIsLegalItem = false;
				if (!player.IsInFactionOfType(EFactionType.Criminal))
				{
					result = EStoreCheckoutResult.NotInCriminalFaction;
					bCanAccessStore = false;
				}
			}
			else if (storetype == EStoreType.Drugs)
			{
				bIsLegalItem = false;
				if (!player.IsInFactionOfType(EFactionType.Criminal))
				{
					result = EStoreCheckoutResult.NotInCriminalFaction;
					bCanAccessStore = false;
				}
			}
			else if (storetype == EStoreType.Ammo)
			{
				if (!player.HasHandgunFirearmLicense() && !player.HasLargeFirearmLicense())
				{
					result = EStoreCheckoutResult.NeedAnyFirearmLicenseForAmmo;
					bCanAccessStore = false;
				}
			}

			if (bCanAccessStore)
			{
				bool bPurchaseBlocked = false;

				foreach (SStoreCheckoutItem cartItem in cartContents)
				{
					CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[storeInst.GetItemFromIndex(cartItem.index)];

					// Check for a premade mask on a custom char
					if (player.CharacterType == ECharacterType.Custom)
					{
						if (itemDef.ItemId == EItemID.PREMADE_CHAR_MASK)
						{
							bPurchaseBlocked = true;
							result = EStoreCheckoutResult.Failed_MaskForCustomChar;
							break;
						}
					}

					// We need to add one instance for each count
					for (int i = 0; i < cartItem.count; ++i)
					{
						// TODO_POST_LAUNCH: Should stores support stacked items? probably not. Let players buy in bulk and then merge?
						itemIDs.Add(CItemInstanceDef.FromDefaultValueWithStackSize(itemDef.ItemId, 0.0f, 1));
					}

					// TODO_GENERICS: Must support generics for prices
					float fTotalCostForItems = cartItem.count * itemDef.GetCostIgnoreGenericItems();

					// Do we have a donator perk for no sales tax?
					if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.NoStoreSalesTax))
					{
						fStateSalesTax = 0.0f;
					}

					// Do we have a donator perk active for 20% discount?
					if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.DiscountCard20Percent))
					{
						float fDiscount = 0.20f * fTotalCostForItems;
						fTotalCostForItems -= fDiscount;
					}

					fTotalCost += fTotalCostForItems + (fStateSalesTax * fTotalCostForItems);
				}

				if (!bPurchaseBlocked)
				{
					// Do we have enough space?
					bool bCanGiveItems = player.Inventory.CanGiveItems(itemIDs, out List<KeyValuePair<CItemInstanceDef, GiveMultipleItemsResult>> lstResult);
					if (bCanGiveItems)
					{
						// TODO: Credit cards or debit cards
						// Can we afford it?
						if (player.SubtractMoney(fTotalCost, PlayerMoneyModificationReason.StoreCheckout))
						{
							HandleStoreTransactionOwnerShare(storeID, fTotalCost);

							// Give items
							foreach (SStoreCheckoutItem cartItem in cartContents)
							{
								CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[storeInst.GetItemFromIndex(cartItem.index)];
								CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromDefaultValueWithStackSize(itemDef.ItemId, itemDef.DefaultVal, itemDef.DefaultStackSize);
								if (ItemInstanceDef.Value is CItemValueBasic)
								{
									((CItemValueBasic)ItemInstanceDef.Value).is_legal = bIsLegalItem;
								}

								if (WeaponHelpers.IsItemAFirearm(itemDef.ItemId) || WeaponHelpers.IsItemAmmo(itemDef.ItemId) || ItemInstanceDef.ItemID == EItemID.CELLPHONE)
								{
									StoreGiveItemSpecial(player, cartItem, itemDef, bIsLegalItem, bIsCivilianFirearm);
									continue;
								}

								uint countAndStack = (uint)cartItem.count * itemDef.DefaultStackSize;
								ItemInstanceDef.StackSize = countAndStack;
								player.Inventory.AddItemOrAddToExistingStack(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, _ => { });
							}

							result = EStoreCheckoutResult.Success;
						}
						else
						{
							result = EStoreCheckoutResult.CannotAfford;
						}
					}
					else
					{
						// We void the entire transaction here, rather than doing partials, because it's confusing for the user to work out what was and wasn't purchased in the end.
						foreach (var kvPair in lstResult)
						{
							CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[kvPair.Key.ItemID];
							player.SendNotification("Store Purchase Failed", ENotificationIcon.ExclamationSign, "You cannot receive item '{0}':<br>{1}", kvPair.Key.GetName(), kvPair.Value.UserFriendlyMessage);
						}

						result = EStoreCheckoutResult.FailedPartial;
					}
				}
			}

			NetworkEventSender.SendNetworkEvent_OnStoreCheckout_Response(player, result);
		}
	}

	private static void StoreGiveItemSpecial(CPlayer player, SStoreCheckoutItem cartItem,
		CInventoryItemDefinition itemDef, bool bIsLegalItem, bool bIsCivilianFirearm)
	{
		for (int i = 0; i < cartItem.count; ++i)
		{
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromDefaultValueWithStackSize(itemDef.ItemId, itemDef.DefaultVal, itemDef.DefaultStackSize);
			if (ItemInstanceDef.Value is CItemValueBasic)
			{
				((CItemValueBasic)ItemInstanceDef.Value).is_legal = bIsLegalItem;
			}

			player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
			{

				if (WeaponHelpers.IsItemAFirearm(itemDef.ItemId))
				{
					// serialize value (have to resave to update db also, we need the initial insert for the dbid...)
					((CItemValueBasic)ItemInstanceDef.Value).value = ItemInstanceDef.DatabaseID;

					// set fire mode depending on store type
					((CItemValueBasic)ItemInstanceDef.Value).semi_auto = bIsCivilianFirearm;

					if (bIsCivilianFirearm)
					{
						player.AwardAchievement(EAchievementID.NeverGoFullSemiAuto);
					}

					Database.Functions.Items.SaveItemValue(ItemInstanceDef);
				}

				// cellphone number generation
				if (ItemInstanceDef.ItemID == EItemID.CELLPHONE)
				{
					Int64 number = ItemInstanceDef.DatabaseID + Helpers.GetUnixTimestamp();
					((CItemValueCellphone)ItemInstanceDef.Value).number = Convert.ToInt64(number);
					Database.Functions.Items.SaveItemValue(ItemInstanceDef);
				}
			});
		}
	}

	public static async void HandleStoreTransactionOwnerShare(EntityDatabaseID storeID, float fTotalCost)
	{
		CStoreInstance storeInst = StorePool.GetInstanceFromID(storeID);
		if (storeInst != null)
		{
			// Does this store have an associated property/business? If so lets give 80% to the owner
			// TODO_POST_LAUNCH: Taxation
			if (storeInst.ParentPropertyID != -1)
			{
				CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(storeInst.ParentPropertyID);

				if (propertyInst != null)
				{
					if (propertyInst.Model.State == EPropertyState.Owned || propertyInst.Model.State == EPropertyState.Owned_AlwaysEnterable)
					{
						float fMoneyToAdd = fTotalCost * 0.80f;
						// TODO_LAUNCH: Cache the money up and give it in paycheck, same with civ jobs
						if (propertyInst.Model.OwnerType == EPropertyOwnerType.Player)
						{
							WeakReference<CPlayer> playerRef = PlayerPool.GetPlayerFromCharacterID(propertyInst.Model.OwnerId);
							CPlayer ownerPlayer = playerRef.Instance();

							// Is the player online? Give them the cash now
							if (ownerPlayer != null)
							{
								ownerPlayer.AddBankMoney(fMoneyToAdd, PlayerMoneyModificationReason.StoreOwner_ReceiveMoney);
								ownerPlayer.SendNotification("Store Business", ENotificationIcon.PiggyBank, "You received ${0:0.00} from your store '{1}' due to a purchase.", fMoneyToAdd, propertyInst.Model.Name);
							}
							else // player is offline
							{
								await Database.LegacyFunctions.AddOfflinePlayerBankMoney(propertyInst.Model.OwnerId, fMoneyToAdd).ConfigureAwait(true);
							}
						}
						else if (propertyInst.Model.OwnerType == EPropertyOwnerType.Faction)
						{
							CFaction factionInst = FactionPool.GetFactionFromID(propertyInst.Model.OwnerId);

							if (factionInst != null)
							{
								NAPI.Util.ConsoleOutput("Gave faction {0} from sale", fMoneyToAdd);
								factionInst.Money += fMoneyToAdd;
							}
						}
					}
				}
			}
		}
	}

	public async void OnRequestStoreInfo(CPlayer player, EntityDatabaseID storeID)
	{
		CStoreInstance storeInst = StorePool.GetInstanceFromID(storeID);
		if (storeInst != null)
		{
			// police isn't really a store anymore, it's just a ped who gives licenses. This could be removed from store system really and made into a standard world ped
			if (storeInst.GetStoreType() == EStoreType.Police)
			{
				PendingWeaponLicenseStates pendingLicenseStates = await Database.LegacyFunctions.GetPendingFirearmsLicenseStates(player.ActiveCharacterDatabaseID).ConfigureAwait(true);

				if (pendingLicenseStates.Tier1 == EPendingFirearmLicenseState.None && pendingLicenseStates.Tier2 == EPendingFirearmLicenseState.None)
				{
					// check if we already have all permits
					if (player.HasHandgunFirearmLicense() && player.HasLargeFirearmLicense())
					{
						player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You already have all available license types.");
					}
					else if (player.HasHandgunFirearmLicense())
					{
						// TODO_GITHUB: You should replace the below with your own website
						player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You already have a Tier 1 Firearms License.<br><br>If you wish to apply for Tier 2 Firearms license, visit https://leo.website.com/.");
					}
					else if (player.HasLargeFirearmLicense())
					{
						// TODO_GITHUB: You should replace the below with your own website
						player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You already have a Tier 2 Firearms License.<br><br>If you wish to apply for Tier 1 Firearms license, visit https://leo.website.com/.");
					}
					else
					{
						// TODO_GITHUB: You should replace the below with your own website
						player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "If you wish to apply for a Tier 1 and/or Tier 2 Firearms license, visit https://leo.website.com/.");
					}
				}
				else
				{
					if (pendingLicenseStates.Tier1 == EPendingFirearmLicenseState.Issued_PendingPickup)
					{
						CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER1, player.ActiveCharacterDatabaseID);
						List<EItemGiveError> lstErrors;
						string strErrorMsg;
						if (player.Inventory.CanGiveItem(ItemInstanceDef, out lstErrors, out strErrorMsg))
						{
							Database.Functions.Characters.SetTier1PendingLicenseState(player.ActiveCharacterDatabaseID, EPendingFirearmLicenseState.None);
							player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
							player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You have received your Tier 1 Firearms License.");
						}
						else
						{
							player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You cannot receive your Tier 1 weapons license:<br><br>{0}", strErrorMsg);
						}

					}

					if (pendingLicenseStates.Tier2 == EPendingFirearmLicenseState.Issued_PendingPickup)
					{
						CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER2, player.ActiveCharacterDatabaseID);
						List<EItemGiveError> lstErrors;
						string strErrorMsg;
						if (player.Inventory.CanGiveItem(ItemInstanceDef, out lstErrors, out strErrorMsg))
						{
							Database.Functions.Characters.SetTier2PendingLicenseState(player.ActiveCharacterDatabaseID, EPendingFirearmLicenseState.None);
							player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
							player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You have received your Tier 2 Firearms License.");
						}
						else
						{
							player.SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You cannot receive your Tier 2 weapons license:<br><br>{0}", strErrorMsg);
						}
					}
				}
			}
			else
			{
				if (storeInst.GetStoreType() == EStoreType.Furniture)
				{
					NetworkEventSender.SendNetworkEvent_GotoFurnitureStore(player, Taxation.GetSalesTax());
				}
				else if (storeInst.GetStoreType() == EStoreType.Fishmonger)
				{
					CItemInstanceDef ItemInstanceDef_Fish = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISH, 0);
					if (player.Inventory.HasItem(ItemInstanceDef_Fish, false, out CItemInstanceDef unusedInstance))
					{
						// If they have fish, they must have a cooler box
						CItemInstanceDef ItemInstanceDef_FishingCooler = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISH_COOLER_BOX, 0);
						if (player.Inventory.HasItem(ItemInstanceDef_FishingCooler, false, out CItemInstanceDef coolerItemInstance))
						{
							List<CItemInstanceDef> lstFishItems = player.Inventory.GetItemsInsideContainer(coolerItemInstance.DatabaseID);
							Dictionary<float, int> dictFishSoldAtEachPrice = new Dictionary<float, int>();
							float fCashToGive = 0.0f;

							foreach (CItemInstanceDef fishItem in lstFishItems)
							{
								CItemValueBasic itemValue = (CItemValueBasic)fishItem.Value;
								float fMarketValue = itemValue.value;
								if (dictFishSoldAtEachPrice.ContainsKey(fMarketValue))
								{
									dictFishSoldAtEachPrice[fMarketValue]++;
								}
								else
								{
									dictFishSoldAtEachPrice[fMarketValue] = 1;
								}

								fCashToGive += fMarketValue;
							}

							player.PushChatMessage(EChatChannel.Notifications, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
							player.PushChatMessage(EChatChannel.Notifications, "You sold {0} (${1:0.00}) fish to the fishmonger: ", lstFishItems.Count, fCashToGive);
							foreach (var kvPair in dictFishSoldAtEachPrice)
							{
								player.PushChatMessage(EChatChannel.Notifications, "\t{0} fish at ${1:0.00}: ${2:0.00} total.", kvPair.Value, kvPair.Key, kvPair.Value * kvPair.Key);
							}
							player.PushChatMessage(EChatChannel.Notifications, "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

							// Remove all fish
							player.Inventory.RemoveItems(ItemInstanceDef_Fish, false);

							// give cash
							player.AddBankMoney(fCashToGive, PlayerMoneyModificationReason.SellFish);
						}
					}
					else
					{
						player.SendNotification("Fishing", ENotificationIcon.ExclamationSign, "You do not have any fish to sell.");
					}
				}
				else
				{
					float fStateSalesTaxRate = Taxation.GetSalesTax();

					// Do we have a donator perk for no sales tax?
					if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.NoStoreSalesTax))
					{
						fStateSalesTaxRate = 0.0f;
					}

					NetworkEventSender.SendNetworkEvent_GotStoreInfo(player, storeInst.GetItems(), fStateSalesTaxRate);
				}
			}

		}
	}

	public void OnEnterOutfitEditor(CPlayer player)
	{
		player.GotoPlayerSpecificDimension();
		player.CacheHealthAndArmor();

		// Force the normal skin, we don't let them modify job or duty skins here.
		player.ApplySkinFromInventory(true, true);

		List<CItemInstanceDef> lstOutfits = player.Inventory.GetAllOutfits();
		List<CItemInstanceDef> lstClothing = player.Inventory.GetAllClothing();

		NetworkEventSender.SendNetworkEvent_EnterOutfitEditor_Response(player, lstOutfits, lstClothing);
	}

	private void OnRequestOutfitList(CPlayer player)
	{
		// Force the normal skin, we don't let them modify job or duty skins here.
		player.ApplySkinFromInventory(true, true);

		List<CItemInstanceDef> lstOutfits = player.Inventory.GetAllOutfits();
		List<CItemInstanceDef> lstClothing = player.Inventory.GetAllClothing();

		NetworkEventSender.SendNetworkEvent_RequestOutfitList_Response(player, lstOutfits, lstClothing);
	}

	public void OnExitOutfitEditor(CPlayer player)
	{
		player.GotoNonPlayerSpecificDimension();
		player.RestoreHealthAndArmor();

		// Restore whatever skin they were using (could be duty/job also)
		player.ApplySkinFromInventory();
	}

	public void OnEnterClothingStore(CPlayer player)
	{
		player.GotoPlayerSpecificDimension();
		player.CacheHealthAndArmor();

		// Force the normal skin, we don't let them modify job or duty skins here.
		player.ApplySkinFromInventory(true, true);

		NetworkEventSender.SendNetworkEvent_EnterClothingStore_Response(player);
	}

	public void OnEnterBarberShop(CPlayer player)
	{
		if (player.CharacterType == ECharacterType.Custom)
		{
			player.GotoPlayerSpecificDimension();
			player.CacheHealthAndArmor();

			// Force the normal skin, we don't let them modify job or duty skins here.
			player.ApplySkinFromInventory(true, true);

			NetworkEventSender.SendNetworkEvent_EnterBarberShop_Response(player);
		}
		else
		{
			// Offer character type change
			NetworkEventSender.SendNetworkEvent_EnterPlasticSurgeon_OfferCharacterUpgrade(player);
		}
	}

	public void OnExitGenericCharacterCustomization(CPlayer player)
	{
		player.GotoNonPlayerSpecificDimension();
		player.RestoreHealthAndArmor();

		// Restore whatever skin they were using (could be duty/job also)
		player.ApplySkinFromInventory();
	}

	private bool CalculateChangesCost(CPlayer player, int[] DrawablesClothing, int[] TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, uint skin, out float costOfChanges, out bool bHasToken)
	{
		bHasToken = player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Clothing_Store);

		BulkClothing currentClothing = player.GetActiveClothing();

		// determine how many items changed
		int numChanges = 0;
		for (var i = 0; i < DrawablesClothing.Length; ++i)
		{
			bool bDrawableChanged = DrawablesClothing[i] != -1 && currentClothing.GetComponent(i) != DrawablesClothing[i];
			bool bTextureChanged = TexturesClothing[i] != -1 && currentClothing.GetTexture(i) != TexturesClothing[i];
			if (bDrawableChanged || bTextureChanged)
			{
				++numChanges;
			}
		}

		Dictionary<int, int> dictCurrentPropDrawables = player.GetCurrentPropDrawables();
		Dictionary<int, int> dictCurrentPropTextures = player.GetCurrentPropTextures();

		foreach (var kvPair in CurrentPropDrawables)
		{
			int adjustedDrawable = kvPair.Value; // adjusted for none
			int adjustedTexture = CurrentPropTextures[kvPair.Key]; // adjusted for none

			bool bDrawableChanged = adjustedDrawable != -1 && dictCurrentPropDrawables[(int)kvPair.Key] != adjustedDrawable;
			bool bTextureChanged = adjustedTexture != -1 && dictCurrentPropTextures[(int)kvPair.Key] != adjustedTexture;
			if (bDrawableChanged || bTextureChanged)
			{
				++numChanges;
			}
		}

		if (skin != player.Client.Model)
		{
			++numChanges;
		}

		const int pricePerChange = 10;
		costOfChanges = numChanges * pricePerChange;

		// TODO: If we ever get sales tax on clothing items, we need to check the relevant donator perk here

		// Do we have a donator perk active for 20% discount?
		if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.DiscountCard20Percent))
		{
			float fDiscount = 0.20f * costOfChanges;
			costOfChanges -= fDiscount;
		}

		return numChanges > 0;
	}

	private bool CalculateChangesCost_Barber(CPlayer player, int BaseHair, int HairStyle, int HairColor, int HairColorHighlight, int ChestHairStyle, int ChestHairColor, int ChestHairColorHighlight, float ChestHairOpacity,
		int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlight, float FacialHairOpacity, int FullBeardStyle, int FullBeardColor,
		out float fCostOfChanges, out bool bHasToken)
	{
		// BASE HAIR
		int currentBaseHair = player.GetData<int>(player.Client, EDataNames.CC_BASEHAIR);
		bool bGotBaseHairCut = BaseHair != -2 && currentBaseHair != BaseHair;

		// HAIR
		int currentHairStyle = player.GetData<int>(player.Client, EDataNames.CC_HAIRSTYLE);
		bool bGotHairCut = HairStyle != -1 && currentHairStyle != HairStyle;

		int currentHairColor = player.GetData<int>(player.Client, EDataNames.CC_HAIRCOLOR);
		int currentHairColorHighlights = player.GetData<int>(player.Client, EDataNames.CC_HAIRCOLORHIGHLIGHTS);
		bool bGotColor = (HairColor != -1 && currentHairColor != HairColor) || (HairColorHighlight != -1 && currentHairColorHighlights != HairColorHighlight);

		// CHEST HAIR
		bool bGotChestHairCut = ChestHairStyle != -1 && player.GetData<int>(player.Client, EDataNames.CC_CHESTHAIR) != ChestHairStyle;
		int currentChestHairColor = player.GetData<int>(player.Client, EDataNames.CC_CHESTHAIRCOLOR);
		int currentChestHairColorHighlights = player.GetData<int>(player.Client, EDataNames.CC_CHESTHAIRCOLOR);
		float fCurrentChestHairOpacity = player.GetData<float>(player.Client, EDataNames.CC_CHESTHAIROPACITY);

		// TODO_BARBER: This is hacky, tied to UI
		bool bChestHairOpacityIsWithinDeadzone = (fCurrentChestHairOpacity <= 0.05f && ChestHairOpacity == 0.05f) || (fCurrentChestHairOpacity >= 0.95f && ChestHairOpacity == 0.95f);
		bool bGotChestColorOrOpacityChange = (ChestHairColor != -1 && currentChestHairColor != ChestHairColor) || (ChestHairColorHighlight != -1 && currentChestHairColorHighlights != ChestHairColorHighlight) || (!bChestHairOpacityIsWithinDeadzone && fCurrentChestHairOpacity != ChestHairOpacity);


		// FACIAL HAIR
		bool bGotFacialHairCut = FacialHairStyle != -1 && player.GetData<int>(player.Client, EDataNames.CC_FACIALHAIRSTYLE) != FacialHairStyle;
		int currentFacialHairColor = player.GetData<int>(player.Client, EDataNames.CC_FACIALHAIRCOLOR);
		int currentFacialHairColorHighlights = player.GetData<int>(player.Client, EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT);
		float fCurrentFacialHairOpacity = player.GetData<float>(player.Client, EDataNames.CC_FACIALHAIROPACITY);

		// TODO_BARBER: This is hacky, tied to UI
		bool bFacialHairOpacityIsWithinDeadzone = (fCurrentFacialHairOpacity <= 0.05f && FacialHairOpacity == 0.05f) || (fCurrentFacialHairOpacity >= 0.95f && FacialHairOpacity == 0.95f);
		bool bGotFacialColorOrOpacityChange = (FacialHairColor != -1 && currentFacialHairColor != FacialHairColor) || (FacialHairColorHighlight != -1 && currentFacialHairColorHighlights != FacialHairColorHighlight) || (!bFacialHairOpacityIsWithinDeadzone && fCurrentFacialHairOpacity != FacialHairOpacity);

		// full beards
		bool bGotFullBeardChange = FullBeardStyle != 0 && player.m_CustomSkinData.FullBeardStyle != FullBeardStyle;
		bool bGotFullBeardColor = (FullBeardColor != 0 && player.m_CustomSkinData.FullBeardColor != FullBeardColor);

		fCostOfChanges = 0.0f;

		if (bGotBaseHairCut || bGotHairCut)
		{
			fCostOfChanges += 20.0f;
		}

		if (bGotColor)
		{
			fCostOfChanges += 25.0f;
		}

		if (bGotChestHairCut)
		{
			fCostOfChanges += 10.0f;
		}

		if (bGotChestColorOrOpacityChange)
		{
			fCostOfChanges += 15.0f;
		}

		if (bGotFacialHairCut)
		{
			fCostOfChanges += 10.0f;
		}

		if (bGotFacialColorOrOpacityChange)
		{
			fCostOfChanges += 15.0f;
		}

		if (bGotFullBeardChange)
		{
			fCostOfChanges += 10.0f;
		}

		if (bGotFullBeardColor)
		{
			fCostOfChanges += 15.0f;
		}

		// TODO: If we ever get sales tax on clothing items, we need to check the relevant donator perk here

		// Do we have a donator perk active for 20% discount?
		if (player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.DiscountCard20Percent))
		{
			float fDiscount = 0.20f * fCostOfChanges;
			fCostOfChanges -= fDiscount;
		}

		bHasToken = player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Barber);

		return bGotBaseHairCut || bGotHairCut || bGotColor || bGotChestHairCut || bGotChestColorOrOpacityChange || bGotFacialHairCut || bGotFacialColorOrOpacityChange || bGotFullBeardChange || bGotFullBeardColor;
	}

	public void BarberShop_CalculatePrice(CPlayer player, int BaseHair, int HairStyle, int HairColor, int HairColorHighlight, int ChestHairStyle, int ChestHairColor, int ChestHairColorHighlight, float ChestHairOpacity,
		int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlight, float FacialHairOpacity, int FullBeardStyle, int FullBeardColor)
	{
		CalculateChangesCost_Barber(player, BaseHair, HairStyle, HairColor, HairColorHighlight, ChestHairStyle, ChestHairColor, ChestHairColorHighlight, ChestHairOpacity, FacialHairStyle, FacialHairColor, FacialHairColorHighlight, FacialHairOpacity, FullBeardStyle, FullBeardColor, out float fCostOfChanges, out bool bHasToken);
		NetworkEventSender.SendNetworkEvent_BarberShop_GotPrice(player, fCostOfChanges, bHasToken);
	}

	public async void OnBarberShop_OnCheckout(CPlayer player, EntityDatabaseID storeID, int BaseHair, int HairStyle, int HairColor, int HairColorHighlight, int ChestHairStyle, int ChestHairColor, int ChestHairColorHighlight, float ChestHairOpacity,
		int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlight, float FacialHairOpacity, int FullBeardStyle, int FullBeardColor)
	{
		bool bMadeAChange = CalculateChangesCost_Barber(player, BaseHair, HairStyle, HairColor, HairColorHighlight, ChestHairStyle, ChestHairColor, ChestHairColorHighlight, ChestHairOpacity, FacialHairStyle, FacialHairColor, FacialHairColorHighlight, FacialHairOpacity, FullBeardStyle, FullBeardColor, out float fCostOfChanges, out bool bHasToken);
		bool bWasPurchased = false;

		if (bMadeAChange)
		{
			// Do we have a token?
			if (bHasToken)
			{
				// consume token
				player.DonationInventory.RemoveTokenOfTypeForActiveCharacter(EDonationEffect.Free_Visit_Barber);
				bWasPurchased = true;
			}
			else if (player.SubtractMoney(fCostOfChanges, PlayerMoneyModificationReason.BarberCheckout))
			{
				bWasPurchased = true;
			}
			else
			{
				bWasPurchased = false;
				player.SendNotification("Barber Shop", ENotificationIcon.ExclamationSign, "You do not have enough money to purchase this hair cut.");
			}

			if (bWasPurchased)
			{
				StoreSystem.HandleStoreTransactionOwnerShare(storeID, fCostOfChanges);

				// BASE HAIR
				if (BaseHair != -2)
				{
					// TODO: Probably want a player helper function for the below functions that does DB + data
					player.SetData(player.Client, EDataNames.CC_BASEHAIR, BaseHair, EDataType.Synced);
					player.m_CustomSkinData.BaseHair = BaseHair;
					await Database.LegacyFunctions.SetCustomCharacterBaseHair(player.ActiveCharacterDatabaseID, BaseHair).ConfigureAwait(true);
				}

				// HAIR
				if (HairStyle != -1)
				{
					// TODO: Probably want a player helper function for the below functions that does DB + data
					player.SetData(player.Client, EDataNames.CC_HAIRSTYLE, HairStyle, EDataType.Synced);
					player.m_CustomSkinData.HairStyle = HairStyle;
					await Database.LegacyFunctions.SetCustomCharacterHairStyle(player.ActiveCharacterDatabaseID, HairStyle).ConfigureAwait(true);
				}

				if (HairColor != -1)
				{
					player.SetData(player.Client, EDataNames.CC_HAIRCOLOR, HairColor, EDataType.Synced);
					player.m_CustomSkinData.HairColor = HairColor;
					await Database.LegacyFunctions.SetCustomCharacterHairColor(player.ActiveCharacterDatabaseID, HairColor).ConfigureAwait(true);
				}

				if (HairColorHighlight != -1)
				{
					player.SetData(player.Client, EDataNames.CC_HAIRCOLORHIGHLIGHTS, HairColorHighlight, EDataType.Synced);
					player.m_CustomSkinData.HairColorHighlights = HairColorHighlight;
					await Database.LegacyFunctions.SetCustomCharacterHairColorHighlight(player.ActiveCharacterDatabaseID, HairColorHighlight).ConfigureAwait(true);
				}

				// CHEST HAIR
				if (ChestHairStyle != -1)
				{
					player.SetData(player.Client, EDataNames.CC_CHESTHAIR, HairStyle, EDataType.Synced);
					player.m_CustomSkinData.ChestHair = ChestHairStyle;
					await Database.LegacyFunctions.SetCustomCharacterChestHairStyle(player.ActiveCharacterDatabaseID, ChestHairStyle).ConfigureAwait(true);
				}

				if (ChestHairColor != -1)
				{
					player.SetData(player.Client, EDataNames.CC_CHESTHAIRCOLOR, ChestHairColor, EDataType.Synced);
					player.m_CustomSkinData.ChestHairColor = ChestHairColor;
					await Database.LegacyFunctions.SetCustomCharacterChestHairColor(player.ActiveCharacterDatabaseID, ChestHairColor).ConfigureAwait(true);
				}

				if (ChestHairColorHighlight != -1)
				{
					player.SetData(player.Client, EDataNames.CC_CHESTHAIRHIGHLIGHT, ChestHairColorHighlight, EDataType.Synced);
					player.m_CustomSkinData.ChestHairColorHighlight = ChestHairColorHighlight;
					await Database.LegacyFunctions.SetCustomCharacterChestHairColorHighlight(player.ActiveCharacterDatabaseID, ChestHairColorHighlight).ConfigureAwait(true);
				}

				if (ChestHairOpacity != -1)
				{
					player.SetData(player.Client, EDataNames.CC_CHESTHAIROPACITY, ChestHairOpacity, EDataType.Synced);
					player.m_CustomSkinData.ChestHairOpacity = ChestHairOpacity;
					await Database.LegacyFunctions.SetCustomCharacterChestHairOpacity(player.ActiveCharacterDatabaseID, ChestHairOpacity).ConfigureAwait(true);
				}

				// FACIAL HAIR
				if (FacialHairStyle != -1)
				{
					player.SetData(player.Client, EDataNames.CC_FACIALHAIRSTYLE, HairStyle, EDataType.Synced);
					player.m_CustomSkinData.FacialHairColor = FacialHairStyle;
					await Database.LegacyFunctions.SetCustomCharacterFacialHairStyle(player.ActiveCharacterDatabaseID, FacialHairStyle).ConfigureAwait(true);
				}

				if (FacialHairColor != -1)
				{
					player.SetData(player.Client, EDataNames.CC_FACIALHAIRCOLOR, FacialHairColor, EDataType.Synced);
					player.m_CustomSkinData.FacialHairColor = FacialHairColor;
					await Database.LegacyFunctions.SetCustomCharacterFacialHairColor(player.ActiveCharacterDatabaseID, FacialHairColor).ConfigureAwait(true);
				}

				if (FacialHairColorHighlight != -1)
				{
					player.SetData(player.Client, EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT, FacialHairColorHighlight, EDataType.Synced);
					player.m_CustomSkinData.FacialHairColorHighlight = FacialHairColorHighlight;
					await Database.LegacyFunctions.SetCustomCharacterFacialHairColorHighlight(player.ActiveCharacterDatabaseID, FacialHairColorHighlight).ConfigureAwait(true);
				}

				if (FacialHairOpacity != -1)
				{
					player.SetData(player.Client, EDataNames.CC_FACIALHAIROPACITY, FacialHairOpacity, EDataType.Synced);
					player.m_CustomSkinData.FacialHairOpacity = FacialHairOpacity;
					await Database.LegacyFunctions.SetCustomCharacterFacialHairOpacity(player.ActiveCharacterDatabaseID, FacialHairOpacity).ConfigureAwait(true);
				}

				// full beard
				player.m_CustomSkinData.FullBeardStyle = FullBeardStyle;
				player.m_CustomSkinData.FullBeardColor = FullBeardColor;
				await Database.LegacyFunctions.SetCharacterFullBeard(player.ActiveCharacterDatabaseID, FullBeardStyle, FullBeardColor).ConfigureAwait(true);

				player.SendNotification("Barber Shop", ENotificationIcon.InfoSign, "Your hair was styled for {0}", bHasToken ? "free (Legacy Character Barber Token)" : Helpers.FormatString("${0:0.00}", fCostOfChanges));
			}
		}
		else
		{
			player.SendNotification("Barber Shop", ENotificationIcon.ExclamationSign, "You did not re-style any of your hair.");
		}

		player.ApplySkinFromInventory();
	}

	public void ClothingStore_CalculatePrice(CPlayer player, int[] DrawablesClothing, int[] TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, uint skinHash)
	{
		CalculateChangesCost(player, DrawablesClothing, TexturesClothing, CurrentPropDrawables, CurrentPropTextures, skinHash, out float costOfChanges, out bool bHasToken);
		NetworkEventSender.SendNetworkEvent_ClothingStore_GotPrice(player, costOfChanges, bHasToken);
	}

	public void OnClothingStore_OnCheckout(CPlayer player, EntityDatabaseID storeID, int[] DrawablesClothing, int[] TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, uint skinHash)
	{
		bool bMadeAChange = CalculateChangesCost(player, DrawablesClothing, TexturesClothing, CurrentPropDrawables, CurrentPropTextures, skinHash, out float fCostOfChanges, out bool bHasToken);

		bool bWasPurchased = false;

		if (bMadeAChange)
		{
			// Do we have a token?
			if (bHasToken)
			{
				// consume token
				player.DonationInventory.RemoveTokenOfTypeForActiveCharacter(EDonationEffect.Free_Visit_Clothing_Store);
				bWasPurchased = true;
			}
			else if (player.SubtractMoney(fCostOfChanges, PlayerMoneyModificationReason.ClothingStoreCheckout))
			{
				bWasPurchased = true;
			}
			else
			{
				bWasPurchased = false;
				player.SendNotification("Barber Shop", ENotificationIcon.ExclamationSign, "You do not have enough money to purchase this outfit.");
			}

			if (bWasPurchased)
			{
				StoreSystem.HandleStoreTransactionOwnerShare(storeID, fCostOfChanges);

				// PREMADES
				if (player.CharacterType == ECharacterType.Premade)
				{
					CItemInstanceDef itemToSave = player.GetActivePremadeClothing();

					// give item, dont need to do this inside the for loop because it takes the array version
					if (skinHash != player.Client.Model) // everything is changing, give a new item
					{
						// Set old item as inactive + save to DB
						if (player.DeactivatePremadeClothing())
						{
							CItemValueClothingPremade clothingValue = new CItemValueClothingPremade((PedHash)skinHash, true);
							CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.CLOTHES, clothingValue);
							player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
						}
						else
						{
							// TODO_CLOTHING: Show error on UI if they dont have space
						}

					}

					// TODO_CLOTHING: Make clothes items take up no slots limit/capacity and have a container/socket for clothing
					// TODO_CLOTHING: Trigger item drop event for each item inside containers, so clothes get removed if in backpack for example
					// Save the item value
					if (itemToSave != null)
					{
						Database.Functions.Items.SaveItemValueAndSocket(itemToSave);
					}
				}
				else
				{
					BulkClothing currentClothing = player.GetActiveClothing();

					for (var i = 0; i < DrawablesClothing.Length; ++i)
					{
						bool bDrawableChanged = DrawablesClothing[i] != -1 && currentClothing.GetComponent(i) != DrawablesClothing[i];
						bool bTextureChanged = TexturesClothing[i] != -1 && currentClothing.GetTexture(i) != TexturesClothing[i];
						if (bDrawableChanged || bTextureChanged)
						{
							// deactivate current item
							EItemID itemType = EItemID.CLOTHES_CUSTOM_FACE + i;

							// dont give hair, for obvious reasons
							if (itemType != EItemID.CLOTHES_CUSTOM_HAIR)
							{
								if (player.DeactivateCustomClothing(itemType))
								{
									// Add new item
									CItemValueClothingCustom clothingValue = new CItemValueClothingCustom(DrawablesClothing[i], TexturesClothing[i], true);
									CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(itemType, clothingValue);
									player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
								}
								else
								{
									// TODO_CLOTHING: Show error on UI if they dont have space
								}
							}
						}
					}

					// Check prop also
					Dictionary<int, int> dictCurrentPropDrawables = player.GetCurrentPropDrawables();
					Dictionary<int, int> dictCurrentPropTextures = player.GetCurrentPropTextures();
					foreach (var kvPair in CurrentPropDrawables)
					{
						int adjustedDrawable = kvPair.Value; // adjusted for none
						int adjustedTexture = CurrentPropTextures[kvPair.Key]; // adjusted for none

						bool bDrawableChanged = adjustedDrawable != -1 && dictCurrentPropDrawables[(int)kvPair.Key] != adjustedDrawable;
						bool bTextureChanged = adjustedTexture != -1 && dictCurrentPropTextures[(int)kvPair.Key] != adjustedTexture;

						if (bDrawableChanged || bTextureChanged)
						{
							// deactivate current item
							EItemID itemType = ItemHelpers.GetItemIDFromPropSlot(kvPair.Key);
							if (player.DeactivateCustomClothing(itemType))
							{
								// Add new item
								CItemValueClothingCustom clothingValue = new CItemValueClothingCustom(adjustedDrawable, adjustedTexture, true);
								CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(itemType, clothingValue);
								player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
							}
							else
							{
								// TODO_CLOTHING: Show error on UI if they dont have space
							}
						}
					}
				}
			}

			player.ApplySkinFromInventory();
		}
	}

	private Dictionary<DateTime, RecentRobbery> m_RecentRobberies = new Dictionary<DateTime, RecentRobbery>();

	private void InitiateStoreRobbery(CPlayer player, Int64 storeID)
	{
		WeakReference<CPlayer> playerRef = new WeakReference<CPlayer>(player);
		CStoreInstance store = StorePool.GetInstanceFromID(storeID);
		if (!store.CanBeRobbed())
		{
			player.SendNotification("Store Robbery", ENotificationIcon.ExclamationSign, "This store has been robbed too recently to rob again.");
			NetworkEventSender.SendNetworkEvent_Store_OnRobberyFinished(player);
			return;
		}

		if (!CanRobStore(store))
		{
			player.SendNotification("Store Robbery", ENotificationIcon.ExclamationSign, "This store type cannot be robbed.");
			NetworkEventSender.SendNetworkEvent_Store_OnRobberyFinished(player);
			return;
		}

		player.SendNotification("Store Robbery", ENotificationIcon.Star, "You have initiated a store robbery.");
		RecentRobbery robbery = new RecentRobbery(storeID, player);

		MainThreadTimerPool.CreateEntityTimer(RobberyCallPolice, robbery.CallPoliceTimer(), player, 1, new object[] { playerRef, store });
		MainThreadTimerPool.CreateEntityTimer(RobberyPedEmoteGettingMoney, 15000, player, 1, new object[] { playerRef, robbery });
		MainThreadTimerPool.CreateEntityTimer(RobberyGivePlayerMoney, 30000, player, 1, new object[] { playerRef, robbery });
		m_RecentRobberies.Add(DateTime.Now, robbery);
		HelperFunctions.Chat.SendMessageToAdmins("A player is committing a robbery, /recentrobberies to view.", true, r: 255, g: 0, b: 0);
		HelperFunctions.Chat.SendPedSpeak(player, "[English] Store Clerk: Okay, okay... I'll get the money!");
		HelperFunctions.Chat.SendPedEmote(player, "The store clerk grabs a bag from beneath the counter, popping the register open to fill the bag with money.");
	}

	private bool CanRobStore(CStoreInstance store)
	{
		return store.m_storeType == EStoreType.General
		|| store.m_storeType == EStoreType.Tobbaco
		|| store.m_storeType == EStoreType.Alcohol
		|| store.m_storeType == EStoreType.Barber
		|| store.m_storeType == EStoreType.Clothing;
	}

	private void CancelStoreRobbery(CPlayer player, Int64 storeID)
	{
		RecentRobbery robbery = m_RecentRobberies.Values.LastOrDefault(r => r.StoreID == storeID && r.PlayerName == player.GetCharacterName(ENameType.StaticCharacterName));
		if (robbery == null)
		{
			return;
		}

		robbery.Cancelled = true;

		player.SendNotification("Store Robbery", ENotificationIcon.ExclamationSign, "You are too far from the store, the robbery has been cancelled. Police will be notified.");
	}

	private void RobberyPedEmoteGettingMoney(object[] parameters)
	{
		WeakReference<CPlayer> playerRef = (WeakReference<CPlayer>)parameters[0];
		CPlayer player = playerRef.Instance();
		if (player == null)
		{
			return;
		}

		RecentRobbery robbery = (RecentRobbery)parameters[1];
		if (robbery.Cancelled)
		{
			return;
		}

		HelperFunctions.Chat.SendPedEmote(player, "The store clerk fumbles with the cash, continuing to pull it from the register, stuffing it into the bag.");
		HelperFunctions.Chat.SendPedSpeak(player, "[English] Store Clerk: I'm going, just don't hurt me!");
	}

	private void RecentRobberies(CPlayer player, CVehicle vehicle)
	{
		DateTime oneDayAgo = DateTime.Now.Subtract(TimeSpan.FromDays(1));
		m_RecentRobberies = m_RecentRobberies.Where(recent => recent.Key > oneDayAgo).ToDictionary(i => i.Key, i => i.Value);

		player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 155, 155, "Recent Robberies:");
		foreach (var robbery in m_RecentRobberies)
		{
			string message = Helpers.FormatString(
				"- {0} robbed store {1} at {2}",
				robbery.Value.PlayerName,
				robbery.Value.StoreID,
				robbery.Key.ToShortTimeString()
			);

			if (robbery.Value.Cancelled)
			{
				message += " (cancelled)";
			}

			if (robbery.Value.MoneyGiven > 0)
			{
				message += Helpers.FormatString(" for ${0:0.00}", robbery.Value.MoneyGiven);
			}

			player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 255, 255, message);
		}

		if (!m_RecentRobberies.Any())
		{
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 255, 255, "No recent robberies.");
		}
	}

	private void RobberyCallPolice(object[] parameters)
	{
		WeakReference<CPlayer> playerRef = (WeakReference<CPlayer>)parameters[0];
		CPlayer player = playerRef.Instance();
		if (player == null)
		{
			return;
		}

		string gender = player.Gender == EGender.Male ? "dude" : "chick";

		CStoreInstance store = (CStoreInstance)parameters[1];
		Vector3 storeRobberyPosition = store.m_vecPos;

		if (store.m_dimension != 0)
		{
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(store.m_dimension);

			if (propertyInst != null)
			{
				storeRobberyPosition = propertyInst.Model.EntrancePosition;
			}
		}

		FactionSystem.Get<FactionSystem>().PDSystem.Add911Call(null, null, storeRobberyPosition, Helpers.FormatString("Some {0} just robbed my store!", gender), bIsRobbery: true);
	}

	private void RobberyGivePlayerMoney(object[] parameters)
	{
		WeakReference<CPlayer> playerRef = (WeakReference<CPlayer>)parameters[0];
		CPlayer player = playerRef.Instance();
		if (player == null)
		{
			return;
		}

		RecentRobbery robbery = (RecentRobbery)parameters[1];
		if (robbery.Cancelled)
		{
			return;
		}

		player.AddMoney(robbery.GetMoney(), PlayerMoneyModificationReason.ScriptedStoreRobbery);
		HelperFunctions.Chat.SendPedEmote(player, "The store clerk pushes the bag over the counter, cowering behind the counter.");
		player.SendNotification("Store Robbery", ENotificationIcon.PiggyBank, "You have received ${0:0.00} from robbing a store.", robbery.GetMoney());
		NetworkEventSender.SendNetworkEvent_Store_OnRobberyFinished(player);

		CStoreInstance store = StorePool.GetInstanceFromID(robbery.StoreID);
		store.TouchStoreLastRobbedAt();
	}
}
