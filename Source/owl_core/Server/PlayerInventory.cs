using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

public enum EItemGiveError
{
	NoSlots,
	TooHeavy,
	HasItemLimit,
	NoSuitableBinding,
}

public class GiveMultipleItemsResult
{
	public GiveMultipleItemsResult(List<EItemGiveError> a_Errors, string a_UserFriendlyMessage)
	{
		Errors = a_Errors;
		UserFriendlyMessage = a_UserFriendlyMessage;
	}

	public List<EItemGiveError> Errors { get; }
	public string UserFriendlyMessage { get; }
}

public class CPlayerInventory
{
	public CPlayerInventory(CPlayer a_OwningPlayer)
	{
		m_OwningPlayer.SetTarget(a_OwningPlayer);

		Reset();
	}
	// TODO_INVENTORY: On new inventory, only let player scroll to a weapon if its on their body or in an acceptable container (e.g. holster, not backpack)

	public void TransmitFullInventory(EShowInventoryAction showInventoryAction = EShowInventoryAction.DoNothing)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		SynchronizeAllWeaponsAndAmmoWithInventory();

		NetworkEventSender.SendNetworkEvent_LocalPlayerInventoryFull(pPlayer, m_arrInventorySlots, showInventoryAction);

		// Update all sockets
		foreach (EItemSocket socket in Enum.GetValues(typeof(EItemSocket)))
		{
			if (InventoryHelpers.IsSocketAPlayerSocket(socket))
			{
				CItemInstanceDef itemInst = GetItemFromSocket(socket);
				OnSocketUpdate(itemInst, socket);
			}
		}

		// Do we need to send a vehicle inventory?
		if (pPlayer.CurrentVehicleInventoryType != EVehicleInventoryType.NONE)
		{
			CVehicle pInventoryVehicle = pPlayer.CurrentVehicleInventory.Instance();
			if (pInventoryVehicle != null)
			{
				NetworkEventSender.SendNetworkEvent_VehicleInventoryDetails(pPlayer, pPlayer.CurrentVehicleInventoryType, pInventoryVehicle.Inventory.GetAllItems());
			}
		}

		// Do we need to send a furniture inventory?
		if (pPlayer.CurrentFurnitureInventory != -1)
		{
			Database.Functions.Items.GetInventoryForFurnitureItemRecursive(pPlayer.CurrentFurnitureInventory, (List<CItemInstanceDef> lstFurnitureInventory) =>
			{
				NetworkEventSender.SendNetworkEvent_FurnitureInventoryDetails(pPlayer, lstFurnitureInventory);
			});
		}

		// Do we need to send a property inventory?
		if (pPlayer.CurrentPropertyInventory.Instance() != null)
		{
			CPropertyInstance pInventoryProperty = pPlayer.CurrentPropertyInventory.Instance();
			if (pInventoryProperty != null)
			{
				NetworkEventSender.SendNetworkEvent_PropertyMailboxDetails(pPlayer, pInventoryProperty.Model.Id, pPlayer.CurrentPropertyInventoryAccessLevel, pInventoryProperty.Inventory.GetAllItems());
			}
		}
	}

	public string GetAsJSON()
	{
		return JsonConvert.SerializeObject(m_arrInventorySlots);
	}

	// TODO_INVENTORY: inv_bugs.txt

	public void SyncInventoryAmmoWithWeaponAmmoAndSave()
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		if (pPlayer.IsSpawned)
		{
			List<CItemInstanceDef> itemsToDelete = new List<CItemInstanceDef>();
			foreach (CItemInstanceDef item in m_arrInventorySlots.ToArray())
			{
				if (item != null)
				{
					if (WeaponHelpers.IsItemAWeapon(item.ItemID))
					{
						EItemID ammoUsed = ItemWeaponDefinitions.g_DictAmmoUsedByWeapon[item.ItemID];

						if (ammoUsed != EItemID.None)
						{
							UInt32 uiAmmo = GetTotalStackSizeOfAllInstancesOfItemID(ammoUsed);

							WeaponHash weaponHash = ItemWeaponDefinitions.g_DictItemIDToWeaponHash[item.ItemID];

							// Find the ammo used by this weapon

							int ammo = -1;
							foreach (var kvPair in pPlayer.GetAmmoData())
							{
								WeaponHash playerWeaponHash = (WeaponHash)NAPI.Util.GetHashKey(kvPair.Key.ToString());
								if (playerWeaponHash == weaponHash)
								{
									pPlayer.Client.SetWeaponAmmo(playerWeaponHash, kvPair.Value);
									ammo = kvPair.Value;
									break;
								}
							}

							if (ammo != -1)
							{
								UInt32 currentAmmo = (UInt32)ammo;

								if (currentAmmo < uiAmmo)
								{
									UInt32 ammoToRemove = uiAmmo - currentAmmo;

									// TODO_0_4: GetWeaponAmmo doesn't work
									foreach (CItemInstanceDef iterItem in m_arrInventorySlots.ToArray())
									{
										if (ammoToRemove == 0)
										{
											break; // we can break, we are done
										}

										if (iterItem.ItemID == ammoUsed)
										{
											if (iterItem.StackSize > ammoToRemove)
											{
												iterItem.StackSize -= ammoToRemove;
												ammoToRemove = 0;
											}
											else
											{
												// We still need to remove more from somewhere else...
												// remove item
												ammoToRemove -= iterItem.StackSize;

												// NOTE: Avoids modifying the collection
												itemsToDelete.Add(iterItem);
											}
										}
									}
								}
							}
						}
					}
				}
			}

			foreach (var item in itemsToDelete)
			{
				if (item != null)
				{
					RemoveItem(item);
				}
			}

			// Iterate and update every ammo item
			foreach (var item in m_arrInventorySlots.ToArray())
			{
				if (item != null)
				{
					// Is this an ammo item?
					if (item.ItemID != EItemID.None && ItemWeaponDefinitions.g_DictAmmoUsedByWeapon.ContainsValue(item.ItemID))
					{
						// TODO: This is writing to the db ~every second,
						// we should figure out how to only save if something changes.
						Database.Functions.Items.SaveItemValueAndStackSize(item);
					}
				}
			}
		}
	}

	private void RemoveAnyNonPlayerItems()
	{
		List<CItemInstanceDef> itemsToRemove = new List<CItemInstanceDef>();
		foreach (CItemInstanceDef item in m_arrInventorySlots.ToArray())
		{
			if (item.CurrentSocket != EItemSocket.None)
			{
				if (!InventoryHelpers.IsSocketAPlayerSocket(item.CurrentSocket))
				{
					itemsToRemove.Add(item);
				}
			}
		}

		foreach (CItemInstanceDef item in itemsToRemove)
		{
			m_arrInventorySlots.Remove(item);
		}
	}

	public void CopyInventory(List<CItemInstanceDef> arrInventorySlots)
	{
		if (arrInventorySlots != null)
		{
			m_arrInventorySlots.Clear();
			m_arrInventorySlots.AddRange(arrInventorySlots);

			// SAFETY: Remove any items that are not in a player socket (if socket is not none)
			RemoveAnyNonPlayerItems();

			TransmitFullInventory();

			SynchronizeAllWeaponsAndAmmoWithInventory(true);
		}
	}

	public void Reset()
	{
		m_arrInventorySlots.Clear();
	}

	public float GetWeightUsed()
	{
		float fWeight = 0.0f;
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item != null)
			{
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];

				UInt32 StackSize = itemDef.IsSplittable() ? item.StackSize : 1;
				fWeight += itemDef.Weight * StackSize;
			}
		}

		return fWeight;
	}

	// TODO_POST_LAUNCH: this doesn't take into consideration going over the limits as a result of the add (e.g. buying two backpacks at in one cart)
	public bool CanGiveItems(List<CItemInstanceDef> a_Items, out List<KeyValuePair<CItemInstanceDef, GiveMultipleItemsResult>> lstResult)
	{
		lstResult = new List<KeyValuePair<CItemInstanceDef, GiveMultipleItemsResult>>();

		bool result = true;
		foreach (CItemInstanceDef itemInstanceDef in a_Items)
		{
			result &= CanGiveItem(itemInstanceDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage);
			lstResult.Add(new KeyValuePair<CItemInstanceDef, GiveMultipleItemsResult>(itemInstanceDef, new GiveMultipleItemsResult(lstErrors, strUserFriendlyMessage)));
		}

		return result;
	}

	public float GetMaxWeight()
	{
		// TODO_POST_LAUNCH: Use m_DefaultHumanMaxInventoryWeight
		float fWeightCapacity = m_DefaultHumanMaxInventoryWeight;

		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item != null)
			{
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];
				fWeightCapacity += itemDef.WeightAddon;
			}
		}

		return fWeightCapacity;
	}

	public UInt32 GetTotalStackSizeOfAllInstancesOfItemID(EItemID a_ItemID)
	{
		UInt32 totalStackSize = 0;
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.ItemID == a_ItemID)
			{
				totalStackSize += item.StackSize;
			}
		}

		return totalStackSize;
	}

	public bool CanGiveItem(CItemInstanceDef a_ItemInstanceDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage, bool bSkipWeightCheck = false)
	{
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[a_ItemInstanceDef.ItemID];
		float fWeightRequired = itemDef.Weight * a_ItemInstanceDef.StackSize;

		bool bHasSlots = GetNumFreeInventorySlots() >= 1;
		float fCurrentWeight = GetWeightUsed();

		bool bHasItemLimit = GetNumItemsWithID(a_ItemInstanceDef.ItemID, false) >= itemDef.Limit;
		bool bHasWeightCapacity = (bSkipWeightCheck || fWeightRequired == 0.0f) ? true : (fCurrentWeight + fWeightRequired) <= GetMaxWeight();
		bool bSuitableBindingFound = DetermineSuitableBindingForItemGranting(a_ItemInstanceDef, out EItemParentTypes itemParentType, out EntityDatabaseID parentDBID, out EItemSocket recommendedSocket);

		lstErrors = new List<EItemGiveError>();
		strUserFriendlyMessage = String.Empty;

		if (bHasItemLimit)
		{
			strUserFriendlyMessage += $"{(strUserFriendlyMessage.Length > 0 ? "<br>" : "")}\tYou already have the maximum amount of this item";
			lstErrors.Add(EItemGiveError.HasItemLimit);
		}

		if (!bHasSlots)
		{
			strUserFriendlyMessage += $"{(strUserFriendlyMessage.Length > 0 ? "<br>" : "")}\tYou do not have space for this item";
			lstErrors.Add(EItemGiveError.NoSlots);
		}

		if (!bHasWeightCapacity)
		{
			strUserFriendlyMessage += $"{(strUserFriendlyMessage.Length > 0 ? "<br>" : "")}\tYou cannot carry the extra weight from this item";
			lstErrors.Add(EItemGiveError.TooHeavy);
		}

		if (!bSuitableBindingFound)
		{
			strUserFriendlyMessage += $"{(strUserFriendlyMessage.Length > 0 ? "<br>" : "")}\tYou do not have a container or socket to hold this item";
			lstErrors.Add(EItemGiveError.NoSuitableBinding);
		}

		return (lstErrors.Count == 0);
	}

	private int GetFreeSlotCount(CItemInstanceDef item)
	{
		int numFreeSlots = 0;
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];

		if (itemDef.IsContainer)
		{
			List<CItemInstanceDef> itemsInsideContainer = GetItemsInsideContainer(item.DatabaseID);
			int numCurrentItems = itemsInsideContainer.Count;
			numFreeSlots += itemDef.ContainerCapacity - numCurrentItems;
		}

		return numFreeSlots;
	}

	private int GetSocketContainerCapacity(EItemSocket itemSocket, bool bSubtractCurrent)
	{
		if (InventoryHelpers.IsSocketAVehicleSocket(itemSocket))
		{
			GTANetworkAPI.Vehicle currentVehicle = m_OwningPlayer.Instance().Client.Vehicle;
			if (currentVehicle != null)
			{
				CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(currentVehicle);
				if (vehicle != null)
				{
					EVehicleClass vehicleClass = vehicle.GetClass();
					return VehicleInventoryConstants.g_iVehicleInventorySizes[(int)vehicleClass];
				}
			}

			return 0;
		}

		int containerSize = InventoryHelpers.IsUnlimitedContainerSocket(itemSocket) ? 999999 : 5;

		if (bSubtractCurrent)
		{
			List<CItemInstanceDef> lstItemsInsideSocket = GetItemsInsideSocket(itemSocket);
			containerSize -= lstItemsInsideSocket.Count;
		}

		return containerSize;
	}

	private int GetNumFreeInventorySlots()
	{
		int numFreeSlots = 0;

		foreach (EItemSocket socket in Enum.GetValues(typeof(EItemSocket)))
		{
			// TODO_INVENTORY: Some sockets function as containers... how much do they hold?
			if (socket == EItemSocket.FrontPockets || socket == EItemSocket.RearPockets || socket == EItemSocket.Clothing || socket == EItemSocket.Outfit)
			{
				// for now, just spoof the number... this does effectively make these containers unlimited in size though... must fix for launch
				numFreeSlots += GetSocketContainerCapacity(socket, true);
			}
			else if (IsSocketFree(socket)) // Is something already in this socket?
			{
				++numFreeSlots;
			}
		}

		foreach (CItemInstanceDef item in m_arrInventorySlots.ToArray())
		{
			numFreeSlots += GetFreeSlotCount(item);
		}

		return numFreeSlots;
	}

	public uint GetNumItemsWithID(EItemID a_ItemID, bool bCountStacks)
	{
		uint count = 0;
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.ItemID == a_ItemID)
			{
				if (bCountStacks)
				{
					count += item.StackSize;
				}
				else
				{
					++count;
				}
			}
		}

		return count;
	}

	public CItemInstanceDef GetFirstItemOfID(EItemID a_ItemID)
	{
		return m_arrInventorySlots.ToArray().FirstOrDefault(item => item.ItemID == a_ItemID);
	}

	public bool DecrementStackSizeOverMultipleInstances(EItemID itemID, uint toRemove)
	{
		// early out if we just dont have enough to remove
		UInt32 TotalAvailable = GetTotalStackSizeOfAllInstancesOfItemID(itemID);
		if (TotalAvailable < toRemove)
		{
			return false;
		}

		uint remainingToRemove = toRemove;
		List<CItemInstanceDef> itemsToDelete = new List<CItemInstanceDef>();
		foreach (CItemInstanceDef iterItem in m_arrInventorySlots.ToArray())
		{
			if (remainingToRemove <= 0)
			{
				break;
			}

			if (iterItem.ItemID == itemID)
			{
				// this one has more than we need, just subtract
				if (iterItem.StackSize > remainingToRemove)
				{
					iterItem.StackSize -= remainingToRemove;
					remainingToRemove = 0;

					// have to update the stack size
					Database.Functions.Items.SaveItemValueAndStackSize(iterItem);
				}
				else // either has less, or the exact number, so we remove it. If it was exact number, remainingToRemove will be zero.
				{
					// We still need to remove more from somewhere else... but this one is consumed so remove it
					remainingToRemove -= iterItem.StackSize;

					// NOTE: Avoids modifying the collection
					itemsToDelete.Add(iterItem);

					// no need to update stack size since its going to be removed
				}
			}
		}

		// now do delayed removal
		foreach (var item in itemsToDelete)
		{
			if (item != null)
			{
				RemoveItem(item);
			}
		}

		return true;
	}

	public bool IncrementStackSizeOfFirstInstance(EItemID itemID, uint toAdd)
	{
		CItemInstanceDef item = GetAllItems().FirstOrDefault(i => i.ItemID == itemID);
		if (item == null)
		{
			return false;
		}

		item.StackSize += toAdd;
		Database.Functions.Items.SaveItemValueAndStackSize(item);
		return true;
	}

	public bool IsSocketFree(EItemSocket a_Socket)
	{
		if (a_Socket == EItemSocket.FrontPockets || a_Socket == EItemSocket.RearPockets || a_Socket == EItemSocket.Clothing || a_Socket == EItemSocket.Furniture || a_Socket == EItemSocket.Keyring || a_Socket == EItemSocket.Wallet || a_Socket == EItemSocket.Outfit)
		{
			return true;
		}

		bool bSocketFree = true;
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.CurrentSocket == a_Socket)
			{
				bSocketFree = false;
				break;
			}
		}

		return bSocketFree;
	}

	public bool DetermineSuitableBindingForItemGranting(CItemInstanceDef a_GrantedItem, out EItemParentTypes outItemParentType, out EntityDatabaseID outParentDBID, out EItemSocket outItemSocket)
	{
		CInventoryItemDefinition newItemDef = ItemDefinitions.g_ItemDefinitions[a_GrantedItem.ItemID];

		outItemParentType = EItemParentTypes.Player;
		outParentDBID = -1;
		outItemSocket = EItemSocket.None;

		bool bSuitableBindingFound = false;

		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return false;
		}

		for (EItemSocket socket = 0; socket < EItemSocket.MAX; ++socket)
		{
			if (InventoryHelpers.IsSocketAPlayerSocket(socket))
			{
				// Is something already in this socket?
				if (IsSocketFree(socket))
				{
					// Does this socket accept this item?
					if (Array.IndexOf(newItemDef.Sockets, socket) != -1)
					{
						outItemParentType = EItemParentTypes.Player;
						outParentDBID = pPlayer.ActiveCharacterDatabaseID;
						outItemSocket = socket;
						bSuitableBindingFound = true;
						PrintLogger.LogMessage(ELogSeverity.DEBUG, "DetermineSuitableBindingForItemGranting: Found Socket: {0}", socket.ToString());
						break;
					}
				}
			}
		}

		// If no socket was found, check if we have a suitable container?
		if (!bSuitableBindingFound)
		{
			foreach (CItemInstanceDef item in m_arrInventorySlots.ToArray())
			{
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];

				if (itemDef.IsContainer && pPlayer.Inventory.GetFreeSlotCount(item) > 0 && itemDef.ContainerCanAcceptItem(a_GrantedItem.ItemID))
				{
					outItemParentType = EItemParentTypes.Container;
					outParentDBID = item.DatabaseID;
					outItemSocket = EItemSocket.None;
					bSuitableBindingFound = true;
					PrintLogger.LogMessage(ELogSeverity.DEBUG, "DetermineSuitableBindingForItemGranting: Found Container: {0}", item.GetName());
					break;
				}
			}
		}

		return bSuitableBindingFound;
	}

	private void OnSocketUpdate(CItemInstanceDef a_ItemInst, EItemSocket a_Socket)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// TODO_INVENTORY: Handle removal only (e.g. nothing new was added to socket)
		int itemID = a_ItemInst == null ? -1 : (int)a_ItemInst.ItemID;

		EDataNames targetDataName = (EDataNames.ITEM_SOCKET_0 + ((int)a_Socket));
		if (targetDataName >= EDataNames.ITEM_SOCKET_0 && targetDataName <= EDataNames.ITEM_SOCKET_23)
		{
			pPlayer.SetData(pPlayer.Client, targetDataName, itemID, EDataType.Synced);
		}
	}

	public void AddItemFromDB(CItemInstanceDef a_ItemInstanceDef)
	{
		m_arrInventorySlots.Add(a_ItemInstanceDef);
		SynchronizeAllWeaponsAndAmmoWithInventory(true);
	}

	public void BulkAddIndependentItemsToNextFreeSuitableSlotsSynchronously(Dictionary<CItemInstanceDef, EItemID> dictItemsAndForceIntoContainerTypes, EShowInventoryAction showInventoryAction, Action<Dictionary<CItemInstanceDef, bool>> CompletionCallback)
	{
		Dictionary<CItemInstanceDef, bool> dictResults = new Dictionary<CItemInstanceDef, bool>();

		// if empty, just trigger the callback
		if (dictItemsAndForceIntoContainerTypes.Count == 0)
		{
			CompletionCallback?.Invoke(dictResults);
			return;
		}

		foreach (var kvPair in dictItemsAndForceIntoContainerTypes)
		{
			CItemInstanceDef itemInstanceDef = kvPair.Key;
			EItemID itemID = kvPair.Value;

			AddItemToNextFreeSuitableSlot(itemInstanceDef, showInventoryAction, itemID, (bool bSuccess) =>
			{
				dictResults[itemInstanceDef] = bSuccess;
			}, false);
		}

		// TODO_MYSQL: Allow partial failure to be considered a full failure?

		// the above wasn't async, so we are done
		Database.Functions.Items.GiveEntityItemBulk(dictResults.Keys.ToList(), (Dictionary<CItemInstanceDef, EntityDatabaseID> dictDBIDs) =>
		{
			// update our DBID's
			foreach (var kvPair in dictDBIDs)
			{
				kvPair.Key.DatabaseID = kvPair.Value;
			}

			TransmitFullInventory(showInventoryAction);

			SynchronizeAllWeaponsAndAmmoWithInventory(true);

			CompletionCallback?.Invoke(dictResults);
		});
	}

	public void AddItemOrAddToExistingStack(CItemInstanceDef a_ItemInstanceDef, EShowInventoryAction showInventoryAction, EItemID a_ForceIntoContainerOfType, Action<bool> CompletionCallback, bool bDoQuery = true)
	{
		CPlayer PlayerInstance = m_OwningPlayer.Instance();
		if (PlayerInstance == null)
		{
			CompletionCallback?.Invoke(false);
			return;
		}
		
		CInventoryItemDefinition newItemDef = ItemDefinitions.g_ItemDefinitions[a_ItemInstanceDef.ItemID];
		int ItemLimit = newItemDef.Limit;
		uint MaxStack = newItemDef.MaxStack;
		uint NumItems = GetNumItemsWithID(a_ItemInstanceDef.ItemID, false);

		if (ItemLimit != -1 && NumItems > ItemLimit)
		{
			CompletionCallback?.Invoke(false);
			return;
		}

		List<CItemInstanceDef> existingItems = GetAllItems()
			.Where(item => item.ItemID == a_ItemInstanceDef.ItemID)
			.Where(item => item.StackSize < MaxStack)
			.ToList();

		uint uAmountAdded = 0;
		foreach (CItemInstanceDef existingItem in existingItems)
		{
			if (existingItem.StackSize == MaxStack)
			{
				continue;
			}

			uint uAmountToAdd = a_ItemInstanceDef.StackSize - uAmountAdded;
			uint uExistingItemStackRemainder = MaxStack - existingItem.StackSize;

			// If the whole remaining amount to add fits in the current existing item, add it and break out.
			if (uExistingItemStackRemainder >= uAmountToAdd)
			{
				uAmountAdded = a_ItemInstanceDef.StackSize;
				existingItem.StackSize += uAmountToAdd;
				if (bDoQuery)
				{
					Database.Functions.Items.SaveItemValueAndStackSize(existingItem);
				}
				break;
			}

			// Stack limit on existing item is less than the amount we need to add, so just push this item to max stack.
			uAmountAdded += uExistingItemStackRemainder;
			existingItem.StackSize += uExistingItemStackRemainder;
			if (bDoQuery)
			{
				Database.Functions.Items.SaveItemValueAndStackSize(existingItem);
			}
		}

		if (uAmountAdded == a_ItemInstanceDef.StackSize)
		{
			CompletionCallback?.Invoke(true);
			return;
		}

		// There is still more to add that did not fit into existing items, so create a new item.
		a_ItemInstanceDef.StackSize -= uAmountAdded;

		if (a_ItemInstanceDef.StackSize < MaxStack)
		{
			AddItemToNextFreeSuitableSlot(a_ItemInstanceDef, showInventoryAction, a_ForceIntoContainerOfType, CompletionCallback, bDoQuery);
			CompletionCallback?.Invoke(true);
			return;
		}

		uint NumberToCreateAtMaxStack = (uint)Math.Floor((double)a_ItemInstanceDef.StackSize / MaxStack);
		uint Remainder = a_ItemInstanceDef.StackSize % MaxStack;

		for (int i = 0; i < NumberToCreateAtMaxStack; i++)
		{
			CItemInstanceDef child = CItemInstanceDef.FromObjectNoDBIDWithStackSize(
				a_ItemInstanceDef.ItemID,
				a_ItemInstanceDef.Value,
				a_ItemInstanceDef.CurrentSocket,
				a_ItemInstanceDef.ParentDatabaseID,
				a_ItemInstanceDef.ParentType,
				MaxStack
			);
			AddItemToNextFreeSuitableSlot(child, showInventoryAction, a_ForceIntoContainerOfType, CompletionCallback, bDoQuery);
		}

		if (Remainder > 0)
		{
			CItemInstanceDef child = CItemInstanceDef.FromObjectNoDBIDWithStackSize(
				a_ItemInstanceDef.ItemID,
				a_ItemInstanceDef.Value,
				a_ItemInstanceDef.CurrentSocket,
				a_ItemInstanceDef.ParentDatabaseID,
				a_ItemInstanceDef.ParentType,
				Remainder
			);
			AddItemToNextFreeSuitableSlot(child, showInventoryAction, a_ForceIntoContainerOfType, CompletionCallback, bDoQuery);
		}
		
		CompletionCallback?.Invoke(true);
	}
	
	// TODO: Make this fill a back queue if no space, optionally, and fill from that DB table when space becomes available
	// TODO: Add a function to force socket?
	public void AddItemToNextFreeSuitableSlot(CItemInstanceDef a_ItemInstanceDef, EShowInventoryAction showInventoryAction, EItemID a_ForceIntoContainerOfType, Action<bool> CompletionCallback, bool bDoQuery = true)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			CompletionCallback?.Invoke(false);
			return;
		}

		// Have we exceeded the item stackability limit?
		// TODO_INVENTORY: We should be able to flag stackability limit as being by value also (or not)
		CInventoryItemDefinition newItemDef = ItemDefinitions.g_ItemDefinitions[a_ItemInstanceDef.ItemID];
		int ItemLimit = newItemDef.Limit;
		uint NumItems = GetNumItemsWithID(a_ItemInstanceDef.ItemID, false);

		if (ItemLimit == -1 || NumItems < ItemLimit)
		{
			bool bSuitableBindingFound = false;
			EItemParentTypes itemParentType = EItemParentTypes.Player;
			long parentDBID = -1; ;
			EItemSocket recommendedSocket = EItemSocket.None; ;

			if (a_ForceIntoContainerOfType != EItemID.None)
			{
				foreach (CItemInstanceDef item in m_arrInventorySlots.ToArray())
				{
					CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];

					if (itemDef.IsContainer && itemDef.ItemId == a_ForceIntoContainerOfType && pPlayer.Inventory.GetFreeSlotCount(item) > 0 && itemDef.ContainerCanAcceptItem(a_ItemInstanceDef.ItemID))
					{
						itemParentType = EItemParentTypes.Container;
						parentDBID = item.DatabaseID;
						recommendedSocket = EItemSocket.None;
						bSuitableBindingFound = true;
						break;
					}
				}

			}
			else
			{
				bSuitableBindingFound = DetermineSuitableBindingForItemGranting(a_ItemInstanceDef, out itemParentType, out parentDBID, out recommendedSocket);
			}

			if (bSuitableBindingFound)
			{
				// is it a weapon? grant temporary immunity against weapon spawn hacks due to sync times between server and client
				if (WeaponHelpers.IsItemAWeapon(a_ItemInstanceDef.ItemID))
				{
					pPlayer.GrantTemporaryImmunityAgainstWeaponGrantHacks();
				}

				// Update item def
				a_ItemInstanceDef.SetBinding(recommendedSocket, parentDBID, itemParentType);
				m_arrInventorySlots.Add(a_ItemInstanceDef);
				SynchronizeAllWeaponsAndAmmoWithInventory(true);

				// Trigger OnSocketUpdate
				if (itemParentType == EItemParentTypes.Player && recommendedSocket != EItemSocket.None)
				{
					OnSocketUpdate(a_ItemInstanceDef, recommendedSocket);
				}

				// Store in DB
				if (bDoQuery)
				{
					Database.Functions.Items.GiveEntityItem(a_ItemInstanceDef, (EntityDatabaseID dbID) =>
					{
						a_ItemInstanceDef.DatabaseID = dbID;

						TransmitFullInventory(showInventoryAction);

						SynchronizeAllWeaponsAndAmmoWithInventory(true);

						CompletionCallback?.Invoke(true);
					});
				}
				else
				{
					CompletionCallback?.Invoke(true);
				}
			}
			else
			{
				PrintLogger.LogMessage(ELogSeverity.HIGH, "Cannot award player {0} item {1} with value {2} as the player has no suitable binding for that item.", pPlayer.ActiveCharacterDatabaseID, a_ItemInstanceDef.ItemID, a_ItemInstanceDef.GetValueDataSerialized());
				pPlayer.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You do not have space to pick up that item ({0}).", a_ItemInstanceDef.GetName());

				CompletionCallback?.Invoke(false);
				return;
			}
		}
		else
		{
			// TODO: Create a logger
			PrintLogger.LogMessage(ELogSeverity.HIGH, "Cannot award player {0} item {1} with value {2} as the player has the max stackables.", pPlayer.ActiveCharacterDatabaseID, a_ItemInstanceDef.ItemID, a_ItemInstanceDef.GetValueDataSerialized());
			pPlayer.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You already have the maximum amount of that item ({0}).", a_ItemInstanceDef.GetName());

			CompletionCallback?.Invoke(false);
			return;
		}
	}

	private UInt32 GetAmmoFromItem(EItemID a_ItemID, UInt32 itemValue)
	{
		if (WeaponHelpers.IsItemAWeapon(a_ItemID))
		{
			EItemID ammoUsed = ItemWeaponDefinitions.g_DictAmmoUsedByWeapon[a_ItemID];

			UInt32 uiAmmo = 1;
			if (ammoUsed == EItemID.None)
			{
				if (a_ItemID == EItemID.WEAPON_FIREEXTINGUISHER || a_ItemID == EItemID.WEAPON_PETROLCAN)
				{
					uiAmmo = 5000;
				}
				else
				{
					uiAmmo = itemValue;
				}
			}
			else
			{
				uiAmmo = GetTotalStackSizeOfAllInstancesOfItemID(ammoUsed);
			}

			return uiAmmo;
		}

		return 0;
	}

	// TODO_RAGE: This is a workaround because Client.Weapons is always null.
	List<WeaponHash> lstWeapons = new List<WeaponHash>();

	// TODO_LAUNCH: Call this more frequently to ensure ammo integrity?
	public void SynchronizeAllWeaponsAndAmmoWithInventory(bool bAwardMissingWeapons = false)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// get attachments
		Dictionary<WeaponHash, List<EItemID>> dictAttachments = pPlayer.WeaponAttachments;

		List<WeaponHash> lstWeaponsInInventory = new List<WeaponHash>();
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (WeaponHelpers.IsItemAWeapon(item.ItemID))
			{
				CItemValueBasic itemValue = item.GetValueData<CItemValueBasic>();
				UInt32 uiAmmo = GetAmmoFromItem(item.ItemID, (uint)itemValue.value);

				// Do we have the weapon associated with this item? if not, grant it
				WeaponHash weaponHash = ItemWeaponDefinitions.g_DictItemIDToWeaponHash[item.ItemID];
				if (bAwardMissingWeapons && !lstWeapons.Contains(weaponHash))
				{
					lstWeapons.Add(weaponHash);
					pPlayer.Client.GiveWeapon(weaponHash, 0);
				}
				lstWeaponsInInventory.Add(weaponHash);

				pPlayer.Client.SetWeaponAmmo(ItemWeaponDefinitions.g_DictItemIDToWeaponHash[item.ItemID], (int)uiAmmo);

				// ATTACHMENTS
				List<CItemInstanceDef> lstWeaponAttachments = GetItemsInsideContainer(item.DatabaseID);
				List<EItemID> lstWeaponAttachmentsFoundThisIter = new List<EItemID>();
				foreach (CItemInstanceDef weaponAttachment in lstWeaponAttachments)
				{
					lstWeaponAttachmentsFoundThisIter.Add(weaponAttachment.ItemID);

					WeaponAttachmentDefinition attachmentDef = WeaponAttachmentDefinitions.GetWeaponAttachmentDefinitionByID(weaponAttachment.ItemID);
					if (attachmentDef != null)
					{
						bool bAddAttachment = true;

						if (dictAttachments.ContainsKey(weaponHash))
						{
							List<EItemID> lstCurrentAttachments = dictAttachments[weaponHash];
							if (lstCurrentAttachments.Contains(weaponAttachment.ItemID))
							{
								bAddAttachment = false;
							}
						}
						else if (bAddAttachment)
						{
							dictAttachments.Add(weaponHash, new List<EItemID>());
						}

						if (bAddAttachment)
						{
							dictAttachments[weaponHash].Add(weaponAttachment.ItemID);
						}
					}
				}

				// REMOVE any attachments we had but no longer have...
				if (dictAttachments.ContainsKey(weaponHash))
				{
					List<EItemID> lstCurrentAttachments = dictAttachments[weaponHash];
					List<EItemID> lstWeaponAttachmentsToRemove = new List<EItemID>();

					foreach (EItemID currentAttachment in lstCurrentAttachments)
					{
						if (!lstWeaponAttachmentsFoundThisIter.Contains(currentAttachment))
						{
							lstWeaponAttachmentsToRemove.Add(currentAttachment);
						}
					}

					// now remove
					foreach (EItemID itemToRemove in lstWeaponAttachmentsToRemove)
					{
						dictAttachments[weaponHash].Remove(itemToRemove);
					}
				}
			}
		}

		// save attachments
		pPlayer.WeaponAttachments = dictAttachments;

		List<WeaponHash> weaponsToRemove = new List<WeaponHash>();
		// Do we have a weapon clientside which we don't have an item for? No item to remove, but do a GTA remove
		// NOTE: Changed to just be safe and remove anything we dont have in our inventory
		//foreach (WeaponHash weaponHash in pPlayer.GetWeaponDataClientside())
		foreach (WeaponHash weaponHash in Enum.GetValues(typeof(WeaponHash)))
		{
			// Does our inventory actually have this weapon?
			if (!lstWeaponsInInventory.Contains(weaponHash))
			{
				weaponsToRemove.Add(weaponHash);

				dictAttachments.Remove(weaponHash);
			}
		}

		foreach (WeaponHash weaponHash in weaponsToRemove)
		{
			// NOTE: Remove, seems to 'give' the weapon and triggers AC
			//pPlayer.Client.SetWeaponAmmo(weaponHash, 0);
			pPlayer.Client.RemoveWeapon(weaponHash);
			lstWeapons.Remove(weaponHash);
			pPlayer.RemoveWeaponDataClientside(weaponHash);
		}
	}

	// NOTE: Only use this if you REALLY know what you're doing, this is probably most useful for clothing
	public void AddClothingItemToSocketForcefully(CItemInstanceDef a_ItemInstanceDef, EItemSocket a_Socket, EntityDatabaseID CharacterID)
	{
		if (a_ItemInstanceDef.ItemID == EItemID.CLOTHES || (a_ItemInstanceDef.ItemID >= EItemID.CLOTHES_CUSTOM_FACE && a_ItemInstanceDef.ItemID <= EItemID.CLOTHES_CUSTOM_TOPS) || ItemHelpers.IsItemIDAProp(a_ItemInstanceDef.ItemID))
		{
			CPlayer pPlayer = m_OwningPlayer.Instance();
			if (pPlayer == null)
			{
				return;
			}

			// Update item def
			a_ItemInstanceDef.SetBinding(a_Socket, CharacterID, EItemParentTypes.Player);
			m_arrInventorySlots.Add(a_ItemInstanceDef);
			SynchronizeAllWeaponsAndAmmoWithInventory(true);

			// Trigger OnSocketUpdate
			OnSocketUpdate(a_ItemInstanceDef, a_Socket);

			// Store in DB
			Database.Functions.Items.GiveEntityItem(a_ItemInstanceDef, (EntityDatabaseID dbID) =>
			{
				a_ItemInstanceDef.DatabaseID = dbID;
			});
		}
	}

	public void RemoveAllDutyItems()
	{
		// NOTE: This only removes CItemValueBasic items, which is good because we don't touch clothing for example
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		List<CItemInstanceDef> itemsToRemove = new List<CItemInstanceDef>();

		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item != null && item.Value.GetType() == typeof(CItemValueBasic))
			{
				CItemValueBasic itemValue = (CItemValueBasic)item.Value;
				if (itemValue.duty)
				{
					itemsToRemove.Add(item);
				}
			}
		}

		foreach (var item in itemsToRemove)
		{
			EntityDatabaseID characterID = pPlayer.ActiveCharacterDatabaseID;
			EntityDatabaseID dbID = item.DatabaseID;
			m_arrInventorySlots.Remove(item);

			// TODO_INVENTORY: If items are removed, it will potentially change indexing clientside making the UI send incorrect DBID's for actions...
			// queue queries on background thread
			Database.Functions.Items.RemoveEntityItem(item, null);
		}

		SynchronizeAllWeaponsAndAmmoWithInventory();
	}

	public void RemoveAllItemsOfType(EItemID itemID)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		List<CItemInstanceDef> itemsToRemove = new List<CItemInstanceDef>();

		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item != null && item.ItemID == itemID)
			{
				itemsToRemove.Add(item);
			}
		}

		foreach (var item in itemsToRemove)
		{
			m_arrInventorySlots.Remove(item);

			// TODO_INVENTORY: If items are removed, it will potentially change indexing clientside making the UI send incorrect DBID's for actions...
			// queue queries on background thread
			Database.Functions.Items.RemoveEntityItem(item, null);
		}

		SynchronizeAllWeaponsAndAmmoWithInventory();
	}


	private void RecursivelyRemoveChildItems(CItemInstanceDef a_ParentItem, bool bRemoveFromDB)
	{
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[a_ParentItem.ItemID];
		if (itemDef.IsContainer)
		{
			List<CItemInstanceDef> childItemsToRemove = new List<CItemInstanceDef>();
			foreach (var item in m_arrInventorySlots.ToArray())
			{
				if (item.ParentDatabaseID == a_ParentItem.DatabaseID && item.ParentType == EItemParentTypes.Container)
				{
					childItemsToRemove.Add(item);
				}
			}

			foreach (var childItemToRemove in childItemsToRemove)
			{
				// Remove children of this children
				RecursivelyRemoveChildItems(childItemToRemove, bRemoveFromDB);

				// Remove this child from inventory
				m_arrInventorySlots.Remove(childItemToRemove);

				if (bRemoveFromDB)
				{
					// queue queries on background thread
					Database.Functions.Items.RemoveEntityItem(childItemToRemove, null);
				}
			}
		}
	}

	public bool RemoveItem(CItemInstanceDef a_Item, EShowInventoryAction showInventoryAction = EShowInventoryAction.DoNothing, bool bRemoveChildItems = true, bool bRemoveChildItemsFromDB = true)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return false;
		}

		bool bReturnValue = false;

		EntityDatabaseID characterID = pPlayer.ActiveCharacterDatabaseID;
		EntityDatabaseID dbID = a_Item.DatabaseID;

		if (m_arrInventorySlots.Remove(a_Item))
		{
			TransmitFullInventory(showInventoryAction);
			// queue queries on background thread
			Database.Functions.Items.RemoveEntityItem(a_Item, null);
			bReturnValue = true;

			// Remove child items
			if (bRemoveChildItems)
			{
				RecursivelyRemoveChildItems(a_Item, bRemoveChildItemsFromDB);
			}
		}

		return bReturnValue;
	}

	public bool RemoveItemFromBasicDefinition(CItemInstanceDef a_ItemInstanceDef, bool a_bCheckValue)
	{
		bool returnValue = false;
		if (HasItem(a_ItemInstanceDef, a_bCheckValue, out CItemInstanceDef outItem))
		{
			bool bItemRemoved = RemoveItem(outItem);
			returnValue = bItemRemoved;
		}

		return returnValue;
	}

	public void RemoveItems(CItemInstanceDef a_ItemInstance, bool a_bCheckValue)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		List<CItemInstanceDef> itemsToRemove = new List<CItemInstanceDef>();
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item != null && item.ItemID == a_ItemInstance.ItemID)
			{
				if (!a_bCheckValue || item.Equals(a_ItemInstance))
				{
					EntityDatabaseID characterID = pPlayer.ActiveCharacterDatabaseID;
					EntityDatabaseID dbID = item.DatabaseID;

					itemsToRemove.Add(item);
				}
			}
		}

		foreach (CItemInstanceDef item in itemsToRemove)
		{
			m_arrInventorySlots.Remove(item);

			// TODO_INVENTORY: If items are removed, it will potentially change indexing clientside making the UI send incorrect DBID's for actions... full transmit below probably fixes this?
			// queue queries on background thread
			Database.Functions.Items.RemoveEntityItem(item, null);
		}

		TransmitFullInventory();
	}

	public bool HasItem(CItemInstanceDef a_ItemInstanceDef, bool a_bCheckValue, out CItemInstanceDef a_OutItem)
	{
		foreach (CItemInstanceDef item in m_arrInventorySlots.ToArray())
		{
			if (item != null && item.ItemID == a_ItemInstanceDef.ItemID)
			{
				if (!a_bCheckValue || item.Equals(a_ItemInstanceDef))
				{
					a_OutItem = item;
					return true;
				}
			}
		}

		a_OutItem = null;
		return false;
	}

	public bool HasItemInSlot(int slot)
	{
		return GetItemFromSlot(slot) != null;
	}

	public void SoftRemoveItem(CItemInstanceDef itemDef)
	{
		m_arrInventorySlots.Remove(itemDef);
	}

	public CItemInstanceDef GetItemFromSlot(int slot)
	{
		return m_arrInventorySlots[slot];
	}

	public CItemInstanceDef GetItemFromDBID(EntityDatabaseID a_DBID)
	{
		CItemInstanceDef itemInst = null;
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.DatabaseID == a_DBID)
			{
				itemInst = item;
				break;
			}
		}

		return itemInst;
	}

	public CItemInstanceDef GetItemFromSocket(EItemSocket a_Socket)
	{
		CItemInstanceDef itemInst = null;
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.ParentType == EItemParentTypes.Player && item.CurrentSocket == a_Socket)
			{
				itemInst = item;
				break;
			}
		}

		return itemInst;
	}

	public void SplitItem(EntityDatabaseID baseItemDBID, uint baseItemPostSplitAmount, uint newItemPostSplitAmount)
	{
		// TODO_INVENTORY: What if the player/vehicle doesn't have space for the new split item?
		CItemInstanceDef itemDef = GetItemFromDBID(baseItemDBID);

		if (itemDef != null)
		{
			EntityDatabaseID parentID = itemDef.ParentDatabaseID;
			EItemParentTypes parentType = itemDef.ParentType;
			EItemSocket targetSocket = itemDef.CurrentSocket;

			// update existing count
			itemDef.StackSize = baseItemPostSplitAmount;
			Database.Functions.Items.SaveItemValueAndStackSize(itemDef);

			// give a new item with the new split count
			// Reuse the existing item, but with a new split count, DBID gets filled in by AddItemToSocket
			CItemInstanceDef splitItemDef = CItemInstanceDef.FromObject(0, itemDef.ItemID, itemDef.Value, itemDef.CurrentSocket, 0, EItemParentTypes.Player, newItemPostSplitAmount);

			// TODO_INVENTORY: We could probably do the below logic in one fell swoop

			AddItemToNextFreeSuitableSlot(splitItemDef, EShowInventoryAction.Show, EItemID.None, (bool bItemGranted) =>
			{
				// Set it into the same parent container, if it was in a container. If it was in a socket, just let it be assigned to wherever AddItemToNextFreeSuitableSlot decided is best
				if (parentType == EItemParentTypes.Container)
				{
					Database.Functions.Items.SetItemBinding(parentID, splitItemDef.ParentDatabaseID, splitItemDef.ParentType, splitItemDef.DatabaseID, targetSocket, parentType);
					splitItemDef.ParentDatabaseID = parentID;
					splitItemDef.ParentType = parentType;
				}

				TransmitFullInventory(EShowInventoryAction.Show); // TODO_INVENTORY: The add call already transmitted the inventory once... this is wasteful
			});

		}
	}

	public bool MergeItem(EntityDatabaseID sourceItemDBID, EntityDatabaseID targetItemDBID)
	{
		CItemInstanceDef sourceItemDef = GetItemFromDBID(sourceItemDBID);
		CItemInstanceDef targetItemDef = GetItemFromDBID(targetItemDBID);

		if (targetItemDef != null && sourceItemDBID != targetItemDBID && sourceItemDef.ItemID == targetItemDef.ItemID) // Check not the same item, but same type of item
		{
			//Fake the count and check the values.
			uint newStackSize = targetItemDef.StackSize + sourceItemDef.StackSize;
			var itemDef = ItemDefinitions.g_ItemDefinitions[targetItemDef.ItemID];
			// If the new stack size is bigger than the max stack size we return false.
			if (newStackSize > itemDef.MaxStack)
			{
				return false;
			}
			// update existing count
			targetItemDef.StackSize += sourceItemDef.StackSize;
			Database.Functions.Items.SaveItemValueAndStackSize(targetItemDef);

			// Remove the source
			RemoveItem(sourceItemDef, EShowInventoryAction.Show); // Function does TransmitFullInventory
			return true;
		}

		return false;
	}

	public void SetItemInContainer(CItemInstanceDef sourceItem, EntityDatabaseID a_ItemDBID, EntityDatabaseID a_ContainerID, bool bGoingToSocketContainer)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;

		}
		// TODO_INVENTORY: We have to put removed items in another suitable socket
		if (sourceItem != null)
		{
			EntityDatabaseID characterID = pPlayer.ActiveCharacterDatabaseID;
			EntityDatabaseID dbID = sourceItem.DatabaseID;
			EItemParentTypes parentType = EItemParentTypes.Container;
			EntityDatabaseID parentID = a_ContainerID;
			EItemSocket targetSocket = EItemSocket.None;


			if (bGoingToSocketContainer)
			{
				targetSocket = (EItemSocket)a_ContainerID;
				if (targetSocket == EItemSocket.RearPockets || targetSocket == EItemSocket.FrontPockets)
				{
					parentType = EItemParentTypes.Player;
					parentID = characterID;
				}
				Database.Functions.Items.SetItemBinding(parentID, sourceItem.ParentDatabaseID, sourceItem.ParentType, dbID, targetSocket, parentType);
			}
			else
			{
				Database.Functions.Items.SetItemBinding(parentID, sourceItem.ParentDatabaseID, sourceItem.ParentType, dbID, EItemSocket.None, parentType);
			}

			sourceItem.CurrentSocket = targetSocket;
			sourceItem.ParentDatabaseID = parentID;
			sourceItem.ParentType = parentType;

			// TODO: Make this use AddItemToNextFreeSuitableSlot incase we add new functionality and miss it out here?
			m_arrInventorySlots.Add(sourceItem);
			SynchronizeAllWeaponsAndAmmoWithInventory(true);

			TransmitFullInventory();
		}
		else
		{
			// TODO: Log error
		}
	}

	public List<CItemInstanceDef> LocalRemoveItem_NoDBOperation_Recursive(CItemInstanceDef itemToRemove)
	{
		List<CItemInstanceDef> lstItemsRemoved = new List<CItemInstanceDef>();

		// NOTE: This function ONLY removes it locally, it does NOT touch the database. Use SetItemSocket/SetItemInContainer to achieve that

		if (itemToRemove != null)
		{
			// remove root item
			m_arrInventorySlots.Remove(itemToRemove);
			lstItemsRemoved.Add(itemToRemove);

			// Look for child containers and recurse, looking for more children
			foreach (var slot in m_arrInventorySlots.ToArray())
			{
				if (slot.ParentType == EItemParentTypes.Container && slot.ParentDatabaseID == itemToRemove.DatabaseID)
				{
					lstItemsRemoved.AddRange(LocalRemoveItem_NoDBOperation_Recursive(slot));
				}
			}
		}

		return lstItemsRemoved;
	}

	public void SetItemSocket(CItemInstanceDef sourceItem, EntityDatabaseID a_ItemDBID, EItemSocket a_SocketID)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;

		}
		// TODO_INVENTORY: We have to put removed items in another suitable socket
		if (sourceItem != null)
		{
			EntityDatabaseID characterID = pPlayer.ActiveCharacterDatabaseID;
			EntityDatabaseID dbID = sourceItem.DatabaseID;
			EItemParentTypes parentType = EItemParentTypes.Player;
			EntityDatabaseID parentID = characterID;

			Database.Functions.Items.SetItemBinding(parentID, sourceItem.ParentDatabaseID, sourceItem.ParentType, dbID, a_SocketID, parentType);

			sourceItem.CurrentSocket = a_SocketID;
			sourceItem.ParentDatabaseID = characterID;
			sourceItem.ParentType = parentType;

			// TODO: Make this use AddItemToNextFreeSuitableSlot incase we add new functionality and miss it out here?
			m_arrInventorySlots.Add(sourceItem);
			SynchronizeAllWeaponsAndAmmoWithInventory(true);

			TransmitFullInventory();
		}
		else
		{
			// TODO: Log error
		}
	}

	public void DetachItemFromSocket(EntityDatabaseID a_ItemDBID, EItemSocket a_PreviousItemSocket, EItemID a_ItemID)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;

		}
		// TODO_INVENTORY: We have to put removed items in another suitable socket
		CItemInstanceDef inventoryItem = GetItemFromDBID(a_ItemDBID);

		if (inventoryItem != null && inventoryItem.ItemID == a_ItemID && inventoryItem.CurrentSocket == a_PreviousItemSocket)
		{
			EntityDatabaseID characterID = pPlayer.ActiveCharacterDatabaseID;
			EntityDatabaseID dbID = inventoryItem.DatabaseID;

			Database.Functions.Items.SetItemBinding(pPlayer.ActiveCharacterDatabaseID, inventoryItem.ParentDatabaseID, inventoryItem.ParentType, dbID, inventoryItem.CurrentSocket, EItemParentTypes.Player);

			inventoryItem.CurrentSocket = EItemSocket.None;
			inventoryItem.ParentDatabaseID = pPlayer.ActiveCharacterDatabaseID;
			inventoryItem.ParentType = EItemParentTypes.Player;

			m_arrInventorySlots.Add(inventoryItem);
			SynchronizeAllWeaponsAndAmmoWithInventory(true);
		}
		else
		{
			// TODO: Log error
		}
	}

	public void AttachItemToSocket(EntityDatabaseID a_ItemDBID, EItemSocket a_NewItemSocket, EItemID a_ItemID)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		CItemInstanceDef inventoryItem = GetItemFromDBID(a_ItemDBID);

		if (inventoryItem != null)
		{
			EntityDatabaseID characterID = pPlayer.ActiveCharacterDatabaseID;
			EntityDatabaseID dbID = inventoryItem.DatabaseID;

			Database.Functions.Items.SetItemBinding(pPlayer.ActiveCharacterDatabaseID, inventoryItem.ParentDatabaseID, inventoryItem.ParentType, dbID, a_NewItemSocket, EItemParentTypes.Player);

			inventoryItem.CurrentSocket = a_NewItemSocket;
			inventoryItem.ParentDatabaseID = pPlayer.ActiveCharacterDatabaseID;
			inventoryItem.ParentType = EItemParentTypes.Player;
		}
		else
		{
			// TODO: Log error (maybe security/hack attempt? item probably doesnt belong to them...)
		}
	}

	public List<CItemInstanceDef> GetItemsInsideContainer(EntityDatabaseID parentID)
	{
		List<CItemInstanceDef> lstContainerItems = new List<CItemInstanceDef>();
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.ParentType == EItemParentTypes.Container && item.ParentDatabaseID == parentID)
			{
				lstContainerItems.Add(item);
			}
		}

		return lstContainerItems;
	}

	public List<CItemInstanceDef> GetItemsInsideSocket(EItemSocket socket)
	{
		List<CItemInstanceDef> lstContainerItems = new List<CItemInstanceDef>();
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.ParentType == EItemParentTypes.Container && item.CurrentSocket == socket)
			{
				lstContainerItems.Add(item);
			}
		}

		return lstContainerItems;
	}

	public List<CItemInstanceDef> GetAllItems()
	{
		return m_arrInventorySlots;
	}

	public List<CItemInstanceDef> GetAllClothing()
	{
		List<CItemInstanceDef> lstClothing = new List<CItemInstanceDef>();

		foreach (CItemInstanceDef itemDef in m_arrInventorySlots)
		{
			if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_FACE
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_MASK
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_HAIR
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_TORSO
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_LEGS
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_BACK
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_FEET
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_ACCESSORY
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_UNDERSHIRT
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_BODYARMOR
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_DECALS
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_TOPS
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_HELMET
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_GLASSES
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_EARRINGS
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_WATCHES
			|| itemDef.ItemID == EItemID.CLOTHES_CUSTOM_BRACELETS)
			{
				lstClothing.Add(itemDef);
			}
		}

		return lstClothing;
	}

	public List<CItemInstanceDef> GetAllOutfits()
	{
		List<CItemInstanceDef> lstOutfits = new List<CItemInstanceDef>();

		foreach (CItemInstanceDef itemDef in m_arrInventorySlots)
		{
			if (itemDef.ItemID == EItemID.OUTFIT)
			{
				lstOutfits.Add(itemDef);
			}
		}

		return lstOutfits;
	}

	public List<CItemInstanceDef> GetAllDutyOutfits()
	{
		List<CItemInstanceDef> lstOutfits = new List<CItemInstanceDef>();

		foreach (CItemInstanceDef itemDef in m_arrInventorySlots)
		{
			if (itemDef.ItemID == EItemID.DUTY_OUTFIT)
			{
				lstOutfits.Add(itemDef);
			}
		}

		return lstOutfits;
	}

	public List<CItemInstanceDef> GetDutyOutfitsOfType(EDutyType a_Type)
	{
		List<CItemInstanceDef> lstOutfits = new List<CItemInstanceDef>();

		foreach (CItemInstanceDef itemDef in m_arrInventorySlots)
		{
			if (itemDef.ItemID == EItemID.DUTY_OUTFIT)
			{
				CItemValueDutyOutfit itemVal = (CItemValueDutyOutfit)itemDef.Value;
				if (itemVal.DutyType == a_Type)
				{
					lstOutfits.Add(itemDef);
				}
			}
		}

		return lstOutfits;
	}

	public CItemInstanceDef GetActiveDutyOutfitOfType(EDutyType a_Type)
	{
		foreach (CItemInstanceDef itemDef in m_arrInventorySlots)
		{
			if (itemDef.ItemID == EItemID.DUTY_OUTFIT)
			{
				CItemValueDutyOutfit itemVal = (CItemValueDutyOutfit)itemDef.Value;
				if (itemVal.DutyType == a_Type)
				{
					if (itemVal.IsActive)
					{
						return itemDef;
					}
				}
			}
		}

		return null;
	}

	// TODO_INVENTORY: Enforce weight restriction on pickup etc...
	private const float m_DefaultHumanMaxInventoryWeight = 20.0f;
	private List<CItemInstanceDef> m_arrInventorySlots = new List<CItemInstanceDef>();
	private WeakReference<CPlayer> m_OwningPlayer = new WeakReference<CPlayer>(null);
}

