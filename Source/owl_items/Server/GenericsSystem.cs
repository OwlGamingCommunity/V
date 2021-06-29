using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Database.Functions;

public class GenericsSystem 
{
	public const uint NEARBY_GENERICS_DISTANCE = 15;

	public GenericsSystem()
	{
		CommandManager.RegisterCommand("nearbyitems", "Shows all nearby world items", new Action<CPlayer, CVehicle>(NearbyItemsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("delnearbyitems", "Deletes all nearby world items", new Action<CPlayer, CVehicle>(DelNearbyItemsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.Generics_SpawnGenerics += OnGiveGenericItems;
		NetworkEvents.AdminToggleItemLock += OnAdminToggleItemLock;
		NetworkEvents.Generics_UpdateGenericPosition += OnUpdateGenericPosition;
	}

	public async void OnUpdateGenericPosition(CPlayer SourcePlayer, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, GTANetworkAPI.Object GenericObject)
	{
		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstanceHandle(GenericObject.Handle);
		if (pWorldItem != null)
		{
			if (pWorldItem.ItemInstance.ItemID == EItemID.GENERIC_ITEM)
			{
				CItemValueGenericItem itemValue = (CItemValueGenericItem)pWorldItem.ItemInstance.Value;

				if (!itemValue.AdminLocked)
				{
					await Database.LegacyFunctions.UpdateGenericPosition(pWorldItem.ItemInstance.DatabaseID, posX, posY, posZ, rotX, rotY, rotZ).ConfigureAwait(true);
					pWorldItem.GTAInstance.Position = new Vector3(posX, posY, posZ);
					pWorldItem.GTAInstance.Rotation = new Vector3(rotX, rotY, rotZ);
					pWorldItem.SetData(pWorldItem.GTAInstance, EDataNames.GENERIC_ROTATION, new Vector3(rotX, rotY, rotZ), EDataType.Synced);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Successfully updated the position!");
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Cannot move the item, please unlock the item first before moving it.");
				}
			}
		}
	}

	public void NearbyItemsCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby items:");
		Dictionary<CWorldItem, float> NearbyItemsDict = new Dictionary<CWorldItem, float>();
		foreach (CWorldItem worldItem in WorldItemPool.GetAllWorldItems())
		{
			if (SourcePlayer.Client.Dimension == worldItem.GTAInstance.Dimension)
			{
				if (SourcePlayer.Client.Position.DistanceTo2D(worldItem.GTAInstance.Position) <= NEARBY_GENERICS_DISTANCE)
				{
					float fDistanceToObj = SourcePlayer.Client.Position.DistanceTo2D(worldItem.GTAInstance.Position);

					NearbyItemsDict.Add(worldItem, fDistanceToObj);
				}
			}
		}

		foreach (var NearbyItem in NearbyItemsDict.OrderBy(i => i.Value))
		{
			CItemInstanceDef item = NearbyItem.Key.ItemInstance;
			if (item == null)
			{
				continue;
			}
			Characters.GetCharacterNameFromDBID(NearbyItem.Key.DroppedByID, (characterName) =>
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "[{1}] {0} | Placed By: {2} | Distance: {3}", item.GetName(), item.DatabaseID, characterName, NearbyItem.Value);
			});
		}
	}

	public void DelNearbyItemsCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		List<CWorldItem> NearbyItems = new List<CWorldItem>();
		foreach (CWorldItem worldItem in WorldItemPool.GetAllWorldItems())
		{
			if (SourcePlayer.Client.Dimension == worldItem.GTAInstance.Dimension)
			{
				if (SourcePlayer.Client.Position.DistanceTo2D(worldItem.GTAInstance.Position) <= NEARBY_GENERICS_DISTANCE)
				{
					NearbyItems.Add(worldItem);
				}
			}
		}

		int totalDestroyed = 0;
		foreach (var NearbyItem in NearbyItems)
		{
			WorldItemPool.DestroyWorldItem(NearbyItem);
			totalDestroyed++;
		}

		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Deleted a total of {0} world items nearby.", totalDestroyed);
	}

	public void OnGiveGenericItems(CPlayer SourcePlayer, string genericName, string model, int amount, float price)
	{
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[EItemID.GENERIC_ITEM];
		float totalPrice = amount * price;

		if (SourcePlayer.CanPlayerAffordCost(totalPrice))
		{
			// Give number of items
			int amountSpawned = 0;
			for (int i = 0; i < amount; ++i)
			{
				CItemValueGenericItem genericItem = new CItemValueGenericItem(genericName, model, false, -1);

				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromJSONStringNoDBID(itemDef.ItemId, JsonConvert.SerializeObject(genericItem));

				if (SourcePlayer.Inventory.CanGiveItem(ItemInstanceDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage, true))
				{
					SourcePlayer.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
					amountSpawned++;
				}
				else
				{
					SourcePlayer.SendNotification("Generic Creator", ENotificationIcon.ExclamationSign, "You do not have enough space to spawn all the generics.");
					break;
				}
			}

			SourcePlayer.SendNotification("Generic Creator", ENotificationIcon.InfoSign, "You have spawned {0} generics with name {1}.", amountSpawned, genericName);

			// Lets only charge for the amount spawned instead of the entered amount to make sure none overpays.
			SourcePlayer.SubtractMoney(amountSpawned * price, PlayerMoneyModificationReason.Generics);

			Logging.Log.CreateLog(SourcePlayer, Logging.ELogType.ObjectSpawner, null, Helpers.FormatString("Admin {0} spawned {1} of generics with name {2} and model {3} for a price of {4} per product", SourcePlayer.Username, amountSpawned, genericName, model, price));
		}
		else
		{
			SourcePlayer.SendNotification("Generic Creator", ENotificationIcon.ExclamationSign, "You do not have enough money to spawn all the generics.");
		}
	}

	private void OnAdminToggleItemLock(CPlayer SenderPlayer, GTANetworkAPI.Object GenericObject)
	{
		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstanceHandle(GenericObject.Handle);

		if (pWorldItem != null)
		{
			if (pWorldItem.ItemInstance.ItemID == EItemID.GENERIC_ITEM)
			{
				CItemValueGenericItem itemValue = (CItemValueGenericItem)pWorldItem.ItemInstance.Value;

				itemValue.AdminLocked = !itemValue.AdminLocked;

				Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);

				pWorldItem.SetData(GenericObject, EDataNames.ITEM_LOCKED, itemValue.AdminLocked, EDataType.Synced);

				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, Helpers.FormatString("Successfully {0} the world item.", Helpers.ColorString(255, 255, 255, "{0}", itemValue.AdminLocked ? "locked" : "unlocked")));
				Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("GENERICS - {0} a world item ({1}, XYZ: {2})", itemValue.AdminLocked ? "locked" : "unlocked", itemValue.name, pWorldItem.GTAInstance.Position.ToString()));
			}
		}
	}
}