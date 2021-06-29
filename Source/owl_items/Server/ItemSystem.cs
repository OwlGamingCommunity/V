using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class ItemSystem
{
	/*
	public enum EAssetType
	{
		ServerScript,
		ClientScript,
		ClientAsset,
		ServerDependency,
		Meta,
		ServerAsset,
		DotNetConfigFile,
		RageOverrideDLL,
		Audio,
		MapFile_Interior,
		MapFile_Persistent,
	}

	private struct AssetFiles
	{
		public string Name;
		public EAssetType Type;
	}

	private struct AssetExportedFunctions
	{
		public string Class;
		public string Function;
	}

	private class AssetDescriptor
	{
		public List<AssetFiles> Files = new List<AssetFiles>();
		public List<AssetExportedFunctions> ExportedFunctions = new List<AssetExportedFunctions>();

		public string FastSerialize()
		{
			if (Files.Count == 0 && ExportedFunctions.Count == 0)
			{
				return "{\"Files\": [],\"ExportedFunctions\": []}";
			}

			return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
		}
	}
	*/

	public BinocularSystem BinocularSystem { get; } = new BinocularSystem();
	public FurnitureSystem FurnitureSystem { get; } = new FurnitureSystem();
	public GangTagSystem GangTagSystem { get; } = new GangTagSystem();
	public GenericsSystem GenericsSystem { get; } = new GenericsSystem();
	public LocksmithSystem LocksmithSystem { get; } = new LocksmithSystem();
	public MarijuanaSystem MarijuanaSystem { get; } = new MarijuanaSystem();
	public MetalDetectorSystem MetalDetectorSystem { get; } = new MetalDetectorSystem();
	private NoteSystem NoteSystem { get; } = new NoteSystem();
	public ItemSystem()
	{
		CommandManager.RegisterCommand("makegeneric", "Opens the generic creation UI", new Action<CPlayer, CVehicle>(MakeGenericCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("items", "Opens the items UI", new Action<CPlayer, CVehicle>(ItemsListCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("givetempmovement", "Gives temporary item movement", new Action<CPlayer, CVehicle, CPlayer>(ItemMovementCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.RequestPlayerInventory += OnRequestPlayerInventory;

		NetworkEvents.SetItemInSocket += SetItemInSocket;
		NetworkEvents.SplitItem += SplitItem;
		NetworkEvents.MergeItem += MergeItem;
		NetworkEvents.SetItemInContainer += SetItemInContainer;
		NetworkEvents.OnUseItem += OnUseItem;
		NetworkEvents.OnShowItem += OnShowItem;
		NetworkEvents.OnDropItem += (CPlayer a_Player, Int64 item_dbid, float x, float y, float z) => { OnDropItem(a_Player, item_dbid, x, y, z, false); };
		NetworkEvents.OnDestroyItem += OnDestroyItem;
		NetworkEvents.OnPickupItem += OnPickupItem;

		NetworkEvents.CloseVehicleInventory += OnCloseVehicleInventory;
		NetworkEvents.RequestVehicleInventory += OnRequestVehicleInventory;
		NetworkEvents.RequestFurnitureInventory += OnRequestFurnitureInventory;
		NetworkEvents.RequestMailbox += OnRequestMailbox;
		NetworkEvents.CloseFurnitureInventory += OnCloseFurnitureInventory;
		NetworkEvents.ClosePropertyInventory += OnClosePropertyInventory;

		NetworkEvents.OnFriskTakeItem += OnFriskTakeItem;
		NetworkEvents.OnEndFrisking += EndFrisking;

		NetworkEvents.RetuneRadio += RetuneRadio;

		NetworkEvents.SavePetName += OnSavePetName;

		NetworkEvents.ShareDutyOutfit += OnShareDutyOutfit;
		NetworkEvents.ShareDutyOutfit_Outcome += OnShareDutyOutfit_Outcome;

		RageEvents.RAGE_OnPlayerWeaponSwitch += API_onPlayerWeaponSwitch;

		try
		{
			// Populate Item Definitions
			PrintLogger.LogMessage(ELogSeverity.MEDIUM, "Deserializing Items");

			CInventoryItemDefinition[] jsonData = JsonConvert.DeserializeObject<CInventoryItemDefinition[]>(System.IO.File.ReadAllText(System.IO.Path.Combine("dotnet", "resources", "owl_gamedata", "ItemData.json")));

			foreach (CInventoryItemDefinition itemDef in jsonData)
			{
				ItemDefinitions.g_ItemDefinitions.Add(itemDef.ItemId, itemDef);
			}

			if (ItemDefinitions.g_ItemDefinitions.Count != (int)EItemID.MAX)
			{
				PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading item data: Expected {0} items, got {1} from JSON data.", (int)EItemID.MAX, ItemDefinitions.g_ItemDefinitions.Count);
			}

			PrintLogger.LogMessage(ELogSeverity.MEDIUM, "Deserialized {0}/{1} items.", ItemDefinitions.g_ItemDefinitions.Count, jsonData.Length);

			// Useful for reserializing when adding new variables (or rolling the version)
			/*
			CInventoryItemDefinitionv2[] jsonDatav2 = JsonConvert.DeserializeObject<CInventoryItemDefinitionv2[]>(System.IO.File.ReadAllText(System.IO.Path.Combine("dotnet", "resources", "owl_gamedata", "ItemData.json")));
			foreach (CInventoryItemDefinitionv2 itemDef in jsonDatav2)
			{
				itemDef.IsLoad = false;

				itemDef.WorldSocketMounts = new Dictionary<EItemSocket, ItemMount>();
				if (itemDef.SocketMounts != null)
				{
					int socket = 0;
					foreach (var mount in itemDef.SocketMounts)
					{
						if (mount.Bone != 0)
						{
							// should copy
							itemDef.WorldSocketMounts.Add((EItemSocket)socket, mount);
						}

						++socket;
					}
				}
			}

			string strData = JsonConvert.SerializeObject(jsonDatav2);
			File.WriteAllText("itemdatadump.json", strData);
			*/

			/*
			List<string> lstToAdd = new List<string>();

			string strDescriptorPath = @"F:\Owl\V\dev\Descriptors\items.client.json";
			AssetDescriptor lstDescriptor = Newtonsoft.Json.JsonConvert.DeserializeObject<AssetDescriptor>(File.ReadAllText(strDescriptorPath));
			foreach (AssetFiles file in lstDescriptor.Files)
			{
				if (file.Name.EndsWith(".png"))
				{
					try
					{
						int itemID = int.Parse(Path.GetFileNameWithoutExtension(file.Name));
						//string strSource = Helpers.FormatString(@"F:\Owl\V\dev\Source\owl_items.client\{0}.png", Path.GetFileNameWithoutExtension(file.Name));
						string strTarget = Helpers.FormatString(@"F:\Owl\V\dev\Source\owl_items.client\Assets\item_images\{0}_state_2.png", Path.GetFileNameWithoutExtension(file.Name));
						//string strTargetFilename = Helpers.FormatString(@"{0}_state_1.png", Path.GetFileNameWithoutExtension(file.Name));

						bool bFound = false;

						// check for state 1
						foreach (AssetFiles fileInner in lstDescriptor.Files)
						{
							if (fileInner.Name.Contains(Helpers.FormatString("{0}_state_2.png", itemID)))
							{
								bFound = true;
								break;
							}
						}

						if (!bFound)
						{
							if (File.Exists(strTarget))
							{
								lstToAdd.Add(Helpers.FormatString("Assets/item_images/{0}_state_2.png", itemID));
							}
						}
					}
					catch
					{

					}
				}
			}

			foreach (string strAdd in lstToAdd)
			{
				var file = new AssetFiles();
				file.Name = strAdd;
				file.Type = EAssetType.ClientAsset;
				lstDescriptor.Files.Add(file);
			}
			

			File.WriteAllText(strDescriptorPath, Newtonsoft.Json.JsonConvert.SerializeObject(lstDescriptor));
			*/

			/*
			foreach (CInventoryItemDefinition itemDef in jsonData)
			{
				if (itemDef.IsContainer)
				{
					string strSource = Helpers.FormatString(@"F:\Owl\V\dev\Source\owl_items.client\Assets\item_images\{0}.png", (int)itemDef.ItemId);
					string strTarget = Helpers.FormatString(@"F:\Owl\V\dev\Source\owl_items.client\Assets\item_images\{0}_state_2.png", (int)itemDef.ItemId);
					if (!File.Exists(strTarget))
					{
						File.Copy(strSource, strTarget);
					}
				}
			}
			*/

			/*
			// DEBUG CODE TO AUTO-STORE ACCEPTED ATTACHMENTS
			foreach(CInventoryItemDefinition itemDef in jsonData)
			{
				if (WeaponHelpers.IsItemAWeapon(itemDef.ItemId))
				{
					// add all attachments
					//

					foreach (var kvPair in WeaponAttachmentDefinitions.g_WeaponAttachmentIDs)
					{
						WeaponHash weaponHash = ItemWeaponDefinitions.g_DictItemIDToWeaponHash[itemDef.ItemId];
						if (kvPair.Value.ContainsKey((uint)weaponHash))
						{
							bool bAdd = true;
							foreach (var itemIdAccepted in itemDef.AcceptedItems)
							{
								if (itemIdAccepted == itemDef.ItemId)
								{
									bAdd = false;
									break;
								}
							}

							if (bAdd)
							{
								var arr = itemDef.AcceptedItems;
								Array.Resize(ref arr, arr.Length + 1);
								arr[arr.Length - 1] = kvPair.Key;

								itemDef.AcceptedItems = arr;
							}
						}
					}
				}
			}

			string strData = JsonConvert.SerializeObject(jsonData);
			File.WriteAllText("F:\\Owl\\V\\dev\\Source\\owl_gamedata\\itemdata.json", strData);
			*/
		}
		catch (Exception ex)
		{
			PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading item data: {0}", ex.ToString());
		}

		List<CDatabaseStructureWorldItem> lstWorldItems = Database.LegacyFunctions.LoadAllWorldItems().Result;
		NAPI.Task.Run(() =>
		{
			foreach (CDatabaseStructureWorldItem worldItem in lstWorldItems)
			{
				CItemInstanceDef ItemInstance = CItemInstanceDef.FromJSONString(worldItem.dbID, worldItem.itemID, worldItem.itemValue, EItemSocket.None, -1, EItemParentTypes.World, worldItem.StackSize);
				WorldItemPool.CreateWorldItem(ItemInstance, worldItem.vecPos, worldItem.fRotZ, worldItem.droppedByID, worldItem.dimension, worldItem.fRotX, worldItem.fRotY);
			}

		});

		List<CDatabaseStructureMetalDetector> lstMetalDetectors = Database.LegacyFunctions.LoadAllMetalDetectors().Result;
		NAPI.Task.Run(() =>
		{
			foreach (var metalDetector in lstMetalDetectors)
			{
				MetalDetectorPool.CreateMetalDetector(metalDetector.metalDetectorID, metalDetector.detectorPosition, metalDetector.detectorRotation, metalDetector.detectorDimension, false);
			}
		});

		NAPI.Util.ConsoleOutput("[WORLD ITEMS] Loaded {0} World Items!", lstWorldItems.Count);
		NAPI.Util.ConsoleOutput("[WORLD ITEMS] Loaded {0} Metal Detectors!", lstMetalDetectors.Count);
	}

	private const float g_fStaticInteractionDist = 5.0f;

	private void ItemMovementCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
	{
		bool bCanMoveObjects = TargetPlayer.GetData<bool>(TargetPlayer.Client, EDataNames.CAN_MOVE_OBJECTS);
		TargetPlayer.SetData(TargetPlayer.Client, EDataNames.CAN_MOVE_OBJECTS, !bCanMoveObjects, EDataType.Synced);

		SourcePlayer.SendNotification("Item Movement", ENotificationIcon.ExclamationSign, Helpers.FormatString("Permissions {0} for {1}.", bCanMoveObjects ? "taken" : "given", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)));
		TargetPlayer.SendNotification("Item Movement", ENotificationIcon.InfoSign, Helpers.FormatString("Your permissions were {0} by {1} ({2}).", bCanMoveObjects ? "taken" : "given", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), SourcePlayer.Username));

		Logging.Log.CreateLog(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("Set the moving permissions of {0} to {1}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), !bCanMoveObjects));
	}

	private void ItemsListCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		NetworkEventSender.SendNetworkEvent_ShowItemsList(SourcePlayer);
	}

	private void MakeGenericCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		NetworkEventSender.SendNetworkEvent_Generics_ShowGenericUI(SourcePlayer);
	}

	// TODO_MAILBOX: Error with windows when moving? also occurs with vehicles
	public async void SetItemInSocket(CPlayer player, EntityDatabaseID ItemDBID, EItemSocket socketID, Vehicle gtaVehicle, EntityDatabaseID currentFurnitureDBID)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			List<CItemInstanceDef> itemsModified = null;

			bool bIsGoingToVehicle = (socketID == EItemSocket.Vehicle_Console_And_Glovebox || socketID == EItemSocket.Vehicle_Seats || socketID == EItemSocket.Vehicle_Trunk);
			bool bIsGoingToFurniture = (socketID == EItemSocket.PlacedFurnitureStorage);
			bool bIsGoingToProperty = (socketID == EItemSocket.Property_Mailbox);

			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(gtaVehicle);
			CPropertyInstance pProperty = player.CurrentPropertyInventory.Instance();

			// find the source item
			if (pVehicle != null)
			{
				CItemInstanceDef sourceItem = pVehicle.Inventory.GetItemFromDBID(ItemDBID);

				if (sourceItem != null)
				{
					itemsModified = pVehicle.Inventory.LocalRemoveItem_NoDBOperation_Recursive(sourceItem);
				}
			}

			// property
			if (pProperty != null)
			{
				CItemInstanceDef sourceItem = pProperty.Inventory.GetItemFromDBID(ItemDBID);

				if (sourceItem != null)
				{
					// do we have permissions to remove items from this property?
					if (player.CurrentPropertyInventoryAccessLevel == EMailboxAccessType.AdminAccess || player.CurrentPropertyInventoryAccessLevel == EMailboxAccessType.ReadWrite)
					{
						itemsModified = pProperty.Inventory.LocalRemoveItem_NoDBOperation_Recursive(sourceItem);
					}
					else
					{
						player.SendNotification("Mailbox", ENotificationIcon.ExclamationSign, "You can only add items to this mailbox.", null);
						return;
					}
				}
			}

			// Perhaps we're removing it from furniture?
			if ((itemsModified == null || itemsModified.Count == 0) && currentFurnitureDBID != -1)
			{
				// NOTE: This function ONLY removes it locally, it does NOT touch the database. Similr as the noDB op function above

				// find our root item
				CItemInstanceDef itemBeingMoved = await Database.LegacyFunctions.GetInventoryItemFromFurnitureItem(currentFurnitureDBID, ItemDBID).ConfigureAwait(true);

				if (itemBeingMoved != null)
				{
					itemsModified = new List<CItemInstanceDef>();

					// add the root item
					itemsModified.Add(itemBeingMoved);

					// now find and add any children
					List<CItemInstanceDef> lstChildItems = await Database.LegacyFunctions.GetItemsInsideContainerRecursive(itemBeingMoved.DatabaseID).ConfigureAwait(true);
					itemsModified.AddRange(lstChildItems);
				}
			}

			// if it was null, let's check the player inventory instead
			if (itemsModified == null || itemsModified.Count == 0)
			{
				CItemInstanceDef sourceItem = player.Inventory.GetItemFromDBID(ItemDBID);

				if (sourceItem != null)
				{
					itemsModified = player.Inventory.LocalRemoveItem_NoDBOperation_Recursive(sourceItem);
				}
			}

			if (itemsModified == null || itemsModified.Count == 0)
			{
				// TODO_INVENTORY: error
				return;
			}

			// TODO_INVENTORY: detect vehicle
			if (bIsGoingToVehicle)
			{
				if (pVehicle != null)
				{
					foreach (CItemInstanceDef itemModified in itemsModified)
					{
						// Is it the root object?
						if (itemModified.DatabaseID == ItemDBID)
						{
							pVehicle.Inventory.SetItemSocket(itemModified, ItemDBID, socketID);
						}
						else
						{
							pVehicle.Inventory.SetItemInContainer(itemModified, itemModified.DatabaseID, itemModified.ParentDatabaseID, false);
						}
					}
				}


				// This sends vehicle inventory update also
				player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
			}
			else if (bIsGoingToFurniture)
			{
				foreach (CItemInstanceDef itemModified in itemsModified)
				{
					// Is it the root object?
					if (itemModified.DatabaseID == ItemDBID)
					{
						EntityDatabaseID dbID = itemModified.DatabaseID;
						EItemParentTypes parentType = EItemParentTypes.FurnitureContainer;
						EntityDatabaseID parentID = currentFurnitureDBID;
						Database.Functions.Items.SetItemBinding(parentID, itemModified.ParentDatabaseID, itemModified.ParentType, dbID, EItemSocket.PlacedFurnitureStorage, parentType);
					}
					else
					{
						EntityDatabaseID dbID = itemModified.DatabaseID;
						EItemParentTypes parentType = EItemParentTypes.Container;
						EntityDatabaseID parentID = itemModified.ParentDatabaseID;
						Database.Functions.Items.SetItemBinding(parentID, itemModified.ParentDatabaseID, itemModified.ParentType, dbID, EItemSocket.None, parentType);
					}
				}


				// This sends vehicle & furniture inventory update also
				player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
			}
			else if (bIsGoingToProperty)
			{
				if (pProperty != null)
				{
					foreach (CItemInstanceDef itemModified in itemsModified)
					{
						// Is it the root object?
						if (itemModified.DatabaseID == ItemDBID)
						{
							pProperty.Inventory.SetItemSocket(itemModified, ItemDBID, socketID);
						}
						else
						{
							pProperty.Inventory.SetItemInContainer(itemModified, itemModified.DatabaseID, itemModified.ParentDatabaseID, false);
						}
					}
				}

				// This sends full inventory update also
				player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
			}
			else
			{
				foreach (CItemInstanceDef itemModified in itemsModified)
				{
					// Is it the root object?
					if (itemModified.DatabaseID == ItemDBID)
					{
						player.Inventory.SetItemSocket(itemModified, ItemDBID, socketID);
					}
					else
					{
						player.Inventory.SetItemInContainer(itemModified, itemModified.DatabaseID, itemModified.ParentDatabaseID, false);
					}
				}
			}
		}
	}

	public void SplitItem(CPlayer player, EntityDatabaseID baseItemDBID, uint baseItemPostSplitAmount, uint newItemPostSplitAmount)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			// if it was null, let's check the player inventory instead
			CItemInstanceDef playerInventoryItem = player.Inventory.GetItemFromDBID(baseItemDBID);

			if (playerInventoryItem.StackSize < newItemPostSplitAmount)
			{
				player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You don't have enough of this item to split it.");
				return;
			}

			if (playerInventoryItem != null)
			{
				player.Inventory.SplitItem(baseItemDBID, baseItemPostSplitAmount, newItemPostSplitAmount);
				new Logging.Log(player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("Split item with ID: {0}", playerInventoryItem.ItemID)).execute();
			}
		}
	}

	public void MergeItem(CPlayer player, EntityDatabaseID sourceItemDBID, EntityDatabaseID targetItemDBID)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			CItemInstanceDef playerInventoryItem = player.Inventory.GetItemFromDBID(sourceItemDBID);
			if (playerInventoryItem != null)
			{
				bool bMerged = player.Inventory.MergeItem(sourceItemDBID, targetItemDBID);
				if (!bMerged)
				{
					player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "Those items cannot be merged.", null);
				}

				new Logging.Log(player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("Merged item with ID: {0}", playerInventoryItem.ItemID)).execute();
			}
		}
	}

	public async void SetItemInContainer(CPlayer player, EntityDatabaseID ItemDBID, EntityDatabaseID ContainerID, bool bGoingToSocketContainer, Vehicle gtaVehicle, EntityDatabaseID currentFurnitureDBID)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			List<CItemInstanceDef> itemsModified = null;
			bool bIsGoingToVehicle = (ContainerID == (EntityDatabaseID)EItemSocket.Vehicle_Console_And_Glovebox || ContainerID == (EntityDatabaseID)EItemSocket.Vehicle_Seats || ContainerID == (EntityDatabaseID)EItemSocket.Vehicle_Trunk);
			bool bIsGoingToFurniture = (ContainerID == (EntityDatabaseID)EItemSocket.PlacedFurnitureStorage);
			bool bIsGoingToProperty = (ContainerID == (EntityDatabaseID)EItemSocket.Property_Mailbox);

			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(gtaVehicle);
			CPropertyInstance pProperty = player.CurrentPropertyInventory.Instance();

			// find the source item
			if (pVehicle != null)
			{
				CItemInstanceDef sourceItem = pVehicle.Inventory.GetItemFromDBID(ItemDBID);

				itemsModified = pVehicle.Inventory.LocalRemoveItem_NoDBOperation_Recursive(sourceItem);
			}

			// property
			if (pProperty != null)
			{
				// do we have permissions to remove items from this property?

				CItemInstanceDef sourceItem = pProperty.Inventory.GetItemFromDBID(ItemDBID);

				if (sourceItem != null)
				{
					if (player.CurrentPropertyInventoryAccessLevel == EMailboxAccessType.AdminAccess || player.CurrentPropertyInventoryAccessLevel == EMailboxAccessType.ReadWrite)
					{
						itemsModified = pProperty.Inventory.LocalRemoveItem_NoDBOperation_Recursive(sourceItem);
					}
					else
					{
						player.SendNotification("Mailbox", ENotificationIcon.ExclamationSign, "You can only add items to this mailbox.", null);
						return;
					}
				}
			}

			// Perhaps we're removing it from furniture?
			if ((itemsModified == null || itemsModified.Count == 0) && currentFurnitureDBID != -1)
			{
				// NOTE: This function ONLY removes it locally, it does NOT touch the database. Similar as the noDB op function above

				// find our root item
				CItemInstanceDef itemBeingMoved = await Database.LegacyFunctions.GetInventoryItemFromFurnitureItem(currentFurnitureDBID, ItemDBID).ConfigureAwait(true);

				if (itemBeingMoved != null)
				{
					itemsModified = new List<CItemInstanceDef>();

					// add the root item
					itemsModified.Add(itemBeingMoved);

					// now find and add any children
					List<CItemInstanceDef> lstChildItems = await Database.LegacyFunctions.GetItemsInsideContainerRecursive(itemBeingMoved.DatabaseID).ConfigureAwait(true);
					itemsModified.AddRange(lstChildItems);
				}
			}

			// if it was null, let's check the player inventory instead
			if (itemsModified == null || itemsModified.Count == 0)
			{
				CItemInstanceDef sourceItem = player.Inventory.GetItemFromDBID(ItemDBID);
				itemsModified = player.Inventory.LocalRemoveItem_NoDBOperation_Recursive(sourceItem);
			}

			if (itemsModified == null || itemsModified.Count == 0)
			{
				// TODO_INVENTORY: error
				return;
			}

			if (bIsGoingToVehicle)
			{
				foreach (CItemInstanceDef itemModified in itemsModified)
				{
					// Is it the root object?
					if (itemModified.DatabaseID == ItemDBID)
					{
						pVehicle.Inventory.SetItemInContainer(itemModified, ItemDBID, ContainerID, bGoingToSocketContainer);
					}
					else
					{
						pVehicle.Inventory.SetItemInContainer(itemModified, itemModified.DatabaseID, itemModified.ParentDatabaseID, false);
					}
				}


				// This sends vehicle inventory update also
				player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
			}
			else if (bIsGoingToFurniture)
			{
				foreach (CItemInstanceDef itemModified in itemsModified)
				{
					// Is it the root object?
					if (itemModified.DatabaseID == ItemDBID)
					{
						EntityDatabaseID dbID = itemModified.DatabaseID;
						EItemParentTypes parentType = EItemParentTypes.FurnitureContainer;
						EntityDatabaseID parentID = currentFurnitureDBID;
						Database.Functions.Items.SetItemBinding(parentID, itemModified.ParentDatabaseID, itemModified.ParentType, dbID, EItemSocket.PlacedFurnitureStorage, parentType);
					}
					else
					{
						EntityDatabaseID dbID = itemModified.DatabaseID;
						EItemParentTypes parentType = EItemParentTypes.Container;
						EntityDatabaseID parentID = itemModified.ParentDatabaseID;
						Database.Functions.Items.SetItemBinding(parentID, itemModified.ParentDatabaseID, itemModified.ParentType, dbID, EItemSocket.None, parentType);
					}
				}


				// This sends vehicle & furniture inventory update also
				player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
			}
			else if (bIsGoingToProperty)
			{
				foreach (CItemInstanceDef itemModified in itemsModified)
				{
					// Is it the root object?
					if (itemModified.DatabaseID == ItemDBID)
					{
						pProperty.Inventory.SetItemInContainer(itemModified, ItemDBID, ContainerID, bGoingToSocketContainer);
					}
					else
					{
						pProperty.Inventory.SetItemInContainer(itemModified, itemModified.DatabaseID, itemModified.ParentDatabaseID, false);
					}
				}


				// This sends full update also
				player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
			}
			else
			{
				foreach (CItemInstanceDef itemModified in itemsModified)
				{
					// Is it the root object?
					if (itemModified.DatabaseID == ItemDBID)
					{
						player.Inventory.SetItemInContainer(itemModified, ItemDBID, ContainerID, bGoingToSocketContainer);
					}
					else
					{
						player.Inventory.SetItemInContainer(itemModified, itemModified.DatabaseID, itemModified.ParentDatabaseID, false);
					}
				}
			}

			PrintLogger.LogMessage(ELogSeverity.DEBUG, "SetItemInContainer with {0} {1} {2} {3} GoingToVehicle: {4}", ItemDBID, ContainerID, bGoingToSocketContainer, bIsGoingToVehicle);
		}
	}

	public void OnDetachItemFromSocket(CPlayer player, EntityDatabaseID ItemDBID, EItemSocket PreviousItemSocket, EItemID ItemID)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			PrintLogger.LogMessage(ELogSeverity.DEBUG, "OnDetachItemFromSocket with {0} {1} {2}", ItemDBID, PreviousItemSocket, ItemID);
			player.Inventory.DetachItemFromSocket(ItemDBID, PreviousItemSocket, ItemID);
		}
	}

	public void OnAttachItemToSocket(CPlayer player, EntityDatabaseID ItemDBID, EItemSocket NewItemSocket, EItemID ItemID)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			PrintLogger.LogMessage(ELogSeverity.DEBUG, "OnAttachItemToSocket with {0} {1} {2}", ItemDBID, NewItemSocket, ItemID);
			player.Inventory.AttachItemToSocket(ItemDBID, NewItemSocket, ItemID);
		}
	}

	public void OnFriskTakeItem(CPlayer player, Int64 dbid)
	{
		CPlayer playerBeingFrisked = player.GetPlayerBeingFrisked();
		bool bFriskingAsAdmin = player.GetFriskingAsAdmin();

		// TODO: What if player changes char? I guess distance check covers 99.999% of cases
		if (playerBeingFrisked == null)
		{
			return;
		}

		if (!bFriskingAsAdmin && !player.IsWithinDistanceOf(playerBeingFrisked, g_fStaticInteractionDist))
		{
			return;
		}

		// This verifies that the target player owns it also
		CItemInstanceDef item = playerBeingFrisked.Inventory.GetItemFromDBID(dbid);
		if (item == null)
		{
			return;
		}

		EItemID itemID = item.ItemID;
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemID];

		// Can the item be taken via frisking?
		if (!itemDef.CanTake)
		{
			player.SendNotification("Frisk", ENotificationIcon.ExclamationSign, "This item cannot be taken via frisking ({0}).", item.GetName());
			return;
		}

		if (!player.Inventory.CanGiveItem(item, out List<EItemGiveError> lstErrors, out string strHumanReadableError))
		{
			player.SendNotification("Frisk", ENotificationIcon.ExclamationSign, "You do not have space to take '{0}' due to: {1}", item.GetName(), strHumanReadableError);
			return;
		}

		if (!bFriskingAsAdmin)
		{
			HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("takes a {0} from {1}.", item.GetName(), playerBeingFrisked.GetCharacterName(ENameType.StaticCharacterName)));
		}

		bool bItemRemoved = playerBeingFrisked.Inventory.RemoveItem(item, EShowInventoryAction.Show);
		// TODO_INVENTORY: If its a container, we need to ensure its contents stay inside, so we either update contents OR instead of remove + give item, just change its owner etc
		CItemInstanceDef itemInstDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(itemID, item.Value);
		player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, null);

		// re-send updated inventory to allow player to continue frisking
		player.Frisk(playerBeingFrisked, bFriskingAsAdmin);

		new Logging.Log(player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("Took item with ID {0} while frisking {1}", itemDef.ItemId, playerBeingFrisked.GetCharacterName(ENameType.StaticCharacterName))).execute();
	}

	public void EndFrisking(CPlayer player)
	{
		player.SetFrisking(null);
	}

	public void OnRequestPlayerInventory(CPlayer player)
	{
		player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
	}

	// TODO_CSHARP: Does object over the wire work?
	public void OnPickupItem(CPlayer player, GTANetworkAPI.Object pickupItem)
	{
		if (player != null)
		{
			if (pickupItem != null)
			{
				if (player.IsCuffed())
				{
					player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
				}
				else
				{
					// Check the world item actually exists
					CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstanceHandle(pickupItem.Handle);
					if (pWorldItem != null)
					{
						// Check we are really nearby, don't trust client
						if (player.IsWithinDistanceOf(pWorldItem, g_fStaticInteractionDist))
						{
							bool bCanPickUp = true;

							if (pWorldItem.ItemInstance.ItemID == EItemID.NOTE)
							{
								CItemValueNote itemValue = (CItemValueNote)pWorldItem.ItemInstance.Value;

								if (player.IsAdmin(EAdminLevel.TrialAdmin, true) && itemValue.AdminLocked)
								{
									bCanPickUp = false;
									player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "This item is locked. Please unlock the item by right clicking it to pick it up.");
									return;
								}

								if (itemValue.AdminLocked)
								{
									bCanPickUp = false;
									player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "This item has been locked by an administrator. Use F2 to request for an administrator to unlock.");
									return;
								}

								itemValue.CharacterDroppedName = player.GetCharacterName(ENameType.StaticCharacterName);
								Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);
							}

							if (pWorldItem.ItemInstance.ItemID == EItemID.GENERIC_ITEM)
							{
								CItemValueGenericItem itemValue = (CItemValueGenericItem)pWorldItem.ItemInstance.Value;

								if (player.IsAdmin(EAdminLevel.TrialAdmin, true) && itemValue.AdminLocked)
								{
									player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "This item is locked. Please unlock the item by right clicking it to pick it up.");
									return;
								}

								if (itemValue.AdminLocked)
								{
									player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "This item has been locked by an administrator. Use F2 to request for an administrator to unlock.");
									return;
								}
							}

							if (pWorldItem.ItemInstance.ItemID == EItemID.BOOMBOX)
							{
								CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(player.Client.Dimension);
								EntityDatabaseID droppedBy = ((CItemValueBoombox)pWorldItem.ItemInstance.Value).placedBy;
								if (Property != null)
								{
									if (player.IsAdmin(EAdminLevel.TrialAdmin, true) || Property.OwnedBy(player) || Property.RentedBy(player) || (Property.Model.OwnerType == EPropertyOwnerType.Faction && player.IsFactionManager(Property.Model.OwnerId)) || droppedBy == player.ActiveCharacterDatabaseID)
									{
										bCanPickUp = true;
									}
									else if (droppedBy != -1 && droppedBy != player.ActiveCharacterDatabaseID)
									{
										bCanPickUp = false;
									}
								}
								else
								{
									if (player.IsAdmin(EAdminLevel.TrialAdmin, true) || droppedBy == player.ActiveCharacterDatabaseID)
									{
										bCanPickUp = true;
									}
									else
									{
										bCanPickUp = false;
									}
								}

							}

							if (bCanPickUp)
							{
								// reset picked up flag if boombox
								if (pWorldItem.ItemInstance.ItemID == EItemID.BOOMBOX)
								{
									((CItemValueBoombox)pWorldItem.ItemInstance.Value).placedBy = -1;
								}

								// Reset pickup and admin locked status
								if (pWorldItem.ItemInstance.ItemID == EItemID.GENERIC_ITEM)
								{
									((CItemValueGenericItem)pWorldItem.ItemInstance.Value).PlacedBy = -1;
									((CItemValueGenericItem)pWorldItem.ItemInstance.Value).AdminLocked = false;
								}

								// Give item to player

								// TODO_INVENTORY: If its a container, we need to ensure its contents stay inside, so we either update contents OR instead of remove + give item, just change its owner etc
								CItemInstanceDef itemInstDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParentWithStackSize(pWorldItem.ItemInstance.ItemID, pWorldItem.ItemInstance.Value, pWorldItem.ItemInstance.StackSize);
								player.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
								{
									// Was it successfully given?
									if (bItemGranted)
									{
										// Remove item from world
										//pWorldItem.Destroy();
										HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("picks up a {0}.", pWorldItem.ItemInstance.GetName()));

										EntityDatabaseID DBID = pWorldItem.m_DatabaseID;

										// Destroy world item
										WorldItemPool.DestroyWorldItem(pWorldItem);
										Database.Functions.Items.DestroyWorldItem(DBID);

										// Update child items of the world item to belong to the item we just awarded to the player
										Database.Functions.Items.UpdateChildItems(itemInstDef.DatabaseID, DBID, EItemParentTypes.Container, EItemSocket.None, EItemParentTypes.Container, async () =>
										{
											// Load child items from DB (This is recursive and will even load containers inside containers)
											List<CItemInstanceDef> childItems = await Database.LegacyFunctions.LoadInventoryRecursiveAsync(EItemParentTypes.Container, itemInstDef.DatabaseID).ConfigureAwait(true);
											foreach (CItemInstanceDef childItem in childItems)
											{
												player.Inventory.AddItemFromDB(childItem);
											}

											// TODO_OPTIMIZATION: Optimize this, we could in theory send the inventory multiple times due to the removes above which may or may not happen. But this transmit must happen in all cases
											// We must send this to unblock the client
											player.Inventory.TransmitFullInventory(EShowInventoryAction.DoNothing);

											new Logging.Log(player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("Picked up item with ID: {0} - Stack size: {1}", pWorldItem.ItemInstance.ItemID, pWorldItem.ItemInstance.StackSize)).execute();
										});
									}
									else
									{

									}
								});
							}
							else
							{
								player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot pick this item up because another person dropped it.", null);
							}
						}
					}
				}
			}
		}
	}

	public void API_onPlayerWeaponSwitch(Player player, WeaponHash oldWeapon, WeaponHash newWeapon)
	{
		WeakReference<CPlayer> pPlayerRef = PlayerPool.GetPlayerFromClient(player);
		CPlayer pPlayer = pPlayerRef.Instance();
		if (pPlayer != null)
		{
			pPlayer.OnWeaponSwitch(oldWeapon, newWeapon);
		}
	}

	public async void OnUseItem(CPlayer player, EntityDatabaseID dbid)
	{
		// TODO_INVENTORY: This should probably take a dbid and not an index? maybe? Needs work for sure
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action whilst restrained.", null);
		}
		else
		{
			CItemInstanceDef inventoryItem = player.Inventory.GetItemFromDBID(dbid);
			EShowInventoryAction showInventoryAction = EShowInventoryAction.Show;

			if (inventoryItem != null)
			{
				if (inventoryItem.ItemID == EItemID.WATCH)
				{
					DateTime ServerClock = Core.GetServerClock();
					DateTime ServerDate = DateTime.Today;

					player.SendNotification("Watch", ENotificationIcon.InfoSign, "The current time is {0}:{1}.", ServerClock.Hour, ServerClock.Minute.ToString().PadLeft(2, '0'));
					player.SendNotification("Watch", ENotificationIcon.InfoSign, "The current date is {0}/{1}/{2}.", ServerDate.Month, ServerDate.Day, ServerDate.Year);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.RADIO)
				{
					CItemValueBasic itemVal = (CItemValueBasic)inventoryItem.Value;
					NetworkEventSender.SendNetworkEvent_RetuneRadio(player, inventoryItem.DatabaseID, Convert.ToInt32(itemVal.value));

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.BOOMBOX)
				{
					((CItemValueBoombox)inventoryItem.Value).placedBy = player.ActiveCharacterDatabaseID;

					Vector3 vecDropPos = player.Client.Position;
					float fRotZ = player.Client.Rotation.Z + 90.0f;

					const float fDist = 0.9f;
					float fRadians = fRotZ * (3.14f / 180.0f);
					vecDropPos.X += (float)Math.Cos(fRadians) * fDist;
					vecDropPos.Y += (float)Math.Sin(fRadians) * fDist;
					vecDropPos.Z -= 1.0f;

					OnDropItem(player, inventoryItem.DatabaseID, vecDropPos.X, vecDropPos.Y, vecDropPos.Z, true);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.FIREARMS_LICENSING_DEVICE)
				{
					if (player.IsFactionManager(FactionPool.GetPoliceFaction().FactionID))
					{
						NetworkEventSender.SendNetworkEvent_UseFirearmsLicensingDevice(player, false);

						showInventoryAction = EShowInventoryAction.HideIfVisible;
					}
					else
					{
						player.SendNotification("Firearms Licensing Device", ENotificationIcon.ExclamationSign, "You must be a manager in the LE faction to use the {0}", inventoryItem.GetName());
					}

				}
				else if (inventoryItem.ItemID == EItemID.SPRAY_CAN)
				{
					NetworkEventSender.SendNetworkEvent_UseSprayCan(player);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.FIREARMS_LICENSING_DEVICE_REMOVAL)
				{
					if (player.IsFactionManager(FactionPool.GetPoliceFaction().FactionID))
					{
						NetworkEventSender.SendNetworkEvent_UseFirearmsLicensingDevice(player, true);

						showInventoryAction = EShowInventoryAction.HideIfVisible;
					}
					else
					{
						player.SendNotification("Firearms Licensing Removal Device", ENotificationIcon.ExclamationSign, "You must be a manager in the LE faction to use the {0}", inventoryItem.GetName());
					}
				}
				else if (inventoryItem.ItemID == EItemID.TACO)
				{
					const int HealthToGive = 20;

					if (player.Client.Health <= 100 - HealthToGive)
					{
						// Consume item
						player.Inventory.RemoveItem(inventoryItem);

						player.Client.Health += HealthToGive;
						player.Save();

						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("eats a {0}.", inventoryItem.GetName()));
						player.SendNotification("Health", ENotificationIcon.InfoSign, "You gained {0} health from eating.", HealthToGive);

						bool bAwarded = player.AwardAchievement(EAchievementID.EatATaco);
						if (bAwarded)
						{
							NetworkEventSender.SendNetworkEvent_PlayCustomAudio(player, EAudioIDs.Mexico, false);
						}
					}
					else
					{
						player.SendNotification("Health", ENotificationIcon.ExclamationSign, "You are not hungry enough to eat a {0}", inventoryItem.GetName());
					}
				}
				else if (inventoryItem.ItemID == EItemID.ARMOR_LIGHT || inventoryItem.ItemID == EItemID.ARMOR_MEDIUM || inventoryItem.ItemID == EItemID.ARMOR_HEAVY)
				{
					int ArmorToSet = 0;

					if (inventoryItem.ItemID == EItemID.ARMOR_LIGHT)
					{
						ArmorToSet = 50;
					}
					else if (inventoryItem.ItemID == EItemID.ARMOR_MEDIUM)
					{
						ArmorToSet = 75;
					}
					else if (inventoryItem.ItemID == EItemID.ARMOR_HEAVY)
					{
						ArmorToSet = 100;
					}

					if (player.Client.Armor < ArmorToSet)
					{
						// Consume item
						player.Inventory.RemoveItem(inventoryItem);

						player.Client.Armor = ArmorToSet;
						player.Save();

						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("puts on a {0}.", inventoryItem.GetName()));
						player.SendNotification("Armor", ENotificationIcon.InfoSign, "You now have {0} armor from using a {1}.", ArmorToSet, inventoryItem.GetName());
					}
					else
					{
						player.SendNotification("Health", ENotificationIcon.ExclamationSign, "You must have below {0} armor to use a {1}.", ArmorToSet, inventoryItem.GetName());
					}
				}
				else if (inventoryItem.ItemID == EItemID.LEO_BADGE)
				{
					// TODO_POST_LAUNCH: If the player has this off, reconnects, the star will appear due to re-going on duty. This would expose UC'es
					bool bNewLeoBadgeState = !player.GetData<bool>(player.Client, EDataNames.LEO_BADGE);
					player.SetLEOBadgeState(bNewLeoBadgeState);
					player.SendNotification("Item", ENotificationIcon.InfoSign, "You {0} a {1}.", bNewLeoBadgeState ? "put on" : "took off", inventoryItem.GetName());
				}
				else if (inventoryItem.ItemID == EItemID.CLOTHES)
				{
					CItemValueClothingPremade itemValueData = inventoryItem.GetValueData<CItemValueClothingPremade>();

					if (itemValueData.IsActive)
					{
						// active item, just make them naked
						if (player.DeactivatePremadeClothing())
						{
							player.ApplySkinFromInventory();
							HelperFunctions.Chat.SendAmeMessage(player, "removes their clothes.");
							player.SendNotification("Clothing", ENotificationIcon.InfoSign, "Your clothing has been removed", null);
						}
						else
						{
							player.SendNotification("Clothing", ENotificationIcon.ExclamationSign, "Your cannot remove these clothes due to lack of storage.", null);
						}

					}
					else
					{
						// deactivate old clothes & activate new
						if (player.DeactivatePremadeClothing())
						{
							player.ActivatePremadeClothing(inventoryItem);

							player.ApplySkinFromInventory();
							HelperFunctions.Chat.SendAmeMessage(player, "changes their clothes.");
							player.SendNotification("Clothing", ENotificationIcon.InfoSign, "Your clothing has been changed", null);
						}
						else
						{
							player.SendNotification("Clothing", ENotificationIcon.ExclamationSign, "You cannot remove these clothes due to lack of storage.", null);
						}
					}
				}
				else if ((inventoryItem.ItemID >= EItemID.CLOTHES_CUSTOM_FACE && inventoryItem.ItemID <= EItemID.CLOTHES_CUSTOM_TOPS)) // custom clothing
				{
					// TODO_POST_LAUNCH: Later we could have a 'naked id' for each slot?

					// Cannot remove torso
					// TODO_LAUNCH: Make clothing store changes replace existing torso? We really need a way of determining suitable torso for clothing
					if (inventoryItem.ItemID == EItemID.CLOTHES_CUSTOM_TORSO)
					{
						// TODO_LAUNCH: What do we really want to do here?
						//player.SendNotification("Clothing", ENotificationIcon.ExclamationSign, "You cannot remove this item of clothing.", null);
						//return;
					}

					CItemValueClothingCustom itemValue = (CItemValueClothingCustom)inventoryItem.Value;

					// Was the player wearing this?
					if (itemValue.IsActive)
					{
						// active item, just make them naked
						if (player.DeactivateCustomClothing(inventoryItem.ItemID))
						{
							player.ApplySkinFromInventory();

							// TODO: Better me + notification
							HelperFunctions.Chat.SendAmeMessage(player, "removes an item of clothing.");
							player.SendNotification("Clothing", ENotificationIcon.InfoSign, "Your item of clothing has been removed", null);
						}
						else
						{
							player.SendNotification("Clothing", ENotificationIcon.ExclamationSign, "You cannot remove these clothes due to lack of storage.", null);
						}
					}
					else
					{
						// deactivate old clothes & activate new
						if (player.DeactivateCustomClothing(inventoryItem.ItemID))
						{
							player.ActivateCustomClothing(inventoryItem);

							// TODO: Better me + notification
							HelperFunctions.Chat.SendAmeMessage(player, "changes an item of clothing.");
							player.SendNotification("Clothing", ENotificationIcon.InfoSign, "Your item of clothing has been changed", null);
						}
						else
						{
							player.SendNotification("Clothing", ENotificationIcon.ExclamationSign, "You cannot remove these clothes due to lack of storage.", null);
						}
					}

				}
				else if (ItemHelpers.IsItemIDAProp(inventoryItem.ItemID)) // props
				{
					CItemValueClothingCustom itemValue = (CItemValueClothingCustom)inventoryItem.Value;

					// Was the player wearing this?
					if (itemValue.IsActive)
					{
						// active item, just make them naked
						if (player.DeactivateCustomClothing(inventoryItem.ItemID))
						{
							player.ApplySkinFromInventory();

							// TODO: Better me + notification
							HelperFunctions.Chat.SendAmeMessage(player, "removes an item of clothing.");
							player.SendNotification("Clothing", ENotificationIcon.InfoSign, "Your item of clothing has been removed", null);
						}
						else
						{
							player.SendNotification("Clothing", ENotificationIcon.ExclamationSign, "You cannot remove these clothes due to lack of storage.", null);
						}
					}
					else
					{
						// deactivate old clothes & activate new
						if (player.DeactivateCustomClothing(inventoryItem.ItemID))
						{
							player.ActivateCustomClothing(inventoryItem);

							// TODO: Better me + notification
							HelperFunctions.Chat.SendAmeMessage(player, "changes an item of clothing.");
							player.SendNotification("Clothing", ENotificationIcon.InfoSign, "Your item of clothing has been changed", null);
						}
						else
						{
							player.SendNotification("Clothing", ENotificationIcon.ExclamationSign, "You cannot remove these clothes due to lack of storage.", null);
						}
					}
				}
				else if (inventoryItem.ItemID == EItemID.CELLPHONE)
				{
					// TODO_CELLPHONE: We definitely have to pass more here in the future, contacts etc
					bool bHasExistingTaxiRequest = TaxiDriverJob.GetTaxiStateForPlayer(player);
					CItemValueCellphone cellphone = (CItemValueCellphone)inventoryItem.Value;
					NetworkEventSender.SendNetworkEvent_UseCellphone(player, bHasExistingTaxiRequest, false, -1);
					player.SetPhoneInUse(cellphone);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.SPIKESTRIP)
				{
					FactionSystem.Get<FactionSystem>().SpikeStripSystem.DeploySpikeStrip(player);
				}
				else if (inventoryItem.ItemID == EItemID.RIOT_SHIELD)
				{
					int shieldData = player.GetData<int>(player.Client, EDataNames.SHIELD);
					if (shieldData == 0)
					{
						player.SetData(player.Client, EDataNames.SHIELD, EShieldType.Riot, EDataType.Synced);
						player.AddAnimationToQueue(49, "weapons@pistol_1h@gang", "aim_med_loop", false, true, true, 0, false);
					}
					else
					{
						player.SetData(player.Client, EDataNames.SHIELD, EShieldType.None, EDataType.Synced);
						player.StopCurrentAnimation(true, true);
					}
				}
				else if (inventoryItem.ItemID == EItemID.SWAT_SHIELD)
				{
					int shieldData = player.GetData<int>(player.Client, EDataNames.SHIELD);
					if (shieldData == 0)
					{
						player.SetData(player.Client, EDataNames.SHIELD, EShieldType.SWAT, EDataType.Synced);
						player.AddAnimationToQueue(49, "weapons@pistol_1h@gang", "aim_med_loop", false, true, true, 0, false);
					}
					else
					{
						player.SetData(player.Client, EDataNames.SHIELD, EShieldType.None, EDataType.Synced);
						player.StopCurrentAnimation(true, true);
					}
				}
				else if (inventoryItem.ItemID == EItemID.WEED)
				{
					CItemInstanceDef item = CItemInstanceDef.FromItemID(EItemID.ROLLING_PAPERS);
					if (!player.Inventory.HasItem(item, false, out CItemInstanceDef outItem))
					{
						player.SendNotification("Joint", ENotificationIcon.ExclamationSign, "You need rolling papers to roll a joint.");
						return;
					}

					CItemInstanceDef itemDef = CItemInstanceDef.FromDefaultValueWithStackSize(EItemID.JOINT, 1, 1);

					player.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (!bItemGranted)
						{
							player.SendNotification("Error", ENotificationIcon.ExclamationSign, "Couldn't give item. No suitable slot");
						}
						else
						{
							player.Inventory.DecrementStackSizeOverMultipleInstances(item.ItemID, 1);
							player.Inventory.DecrementStackSizeOverMultipleInstances(inventoryItem.ItemID, 1);
							HelperFunctions.Chat.SendAmeMessage(player, "moves their fingers up and down on the rolling paper as they licks the sticky edge.");
						}
					});
				}
				else if (inventoryItem.ItemID == EItemID.METH)
				{
					player.SetDrugEffectEnabled(EDrugEffect.Meth, 18000);

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "takes Meth.");
					player.SendNotification("Item Consumed", ENotificationIcon.InfoSign, "You have taken Meth.", null);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.COCAINE)
				{
					player.SetDrugEffectEnabled(EDrugEffect.Cocaine, 18000);

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "takes Cocaine.");
					player.SendNotification("Item Consumed", ENotificationIcon.InfoSign, "You have taken Cocaine.", null);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.HEROIN)
				{
					player.SetDrugEffectEnabled(EDrugEffect.Heroin, 18000);

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "takes Heroin.");
					player.SendNotification("Item Consumed", ENotificationIcon.InfoSign, "You have taken Heroin.", null);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.XANAX)
				{
					player.SetDrugEffectEnabled(EDrugEffect.Xanax, 18000);

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "takes Xanax.");
					player.SendNotification("Item Consumed", ENotificationIcon.InfoSign, "You have taken Xanax.", null);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.BEER)
				{
					player.IncreaseImpairmentLevel(0.1f);

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "drinks a beer.");
					player.SendNotification("Item Consumed", ENotificationIcon.InfoSign, "You have drank a beer.", null);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.VODKA)
				{
					// TODO: Make it so one bottle allows for X number of uses, entire item system could use a consumption limit system
					player.IncreaseImpairmentLevel(0.4f);

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "drinks vodka.");
					player.SendNotification("Item Consumed", ENotificationIcon.InfoSign, "You have drank vodka.", null);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.WHISKY)
				{
					player.IncreaseImpairmentLevel(0.7f);

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "drinks whisky.");
					player.SendNotification("Item Consumed", ENotificationIcon.InfoSign, "You have drank whisky.", null);

					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.PET)
				{
					showInventoryAction = EShowInventoryAction.HideIfVisible;

					// Does the pet have a name?
					if (inventoryItem.Value != null)
					{
						CItemValuePet itemValue = (CItemValuePet)inventoryItem.Value;

						if (itemValue.strName != null && itemValue.strName.Length > 0)
						{
							bool bCurrentPet = player.IsCurrentPet(itemValue);

							if (bCurrentPet)
							{
								player.SetCurrentPet(null);

								player.SendNotification("Pet Uncaged", ENotificationIcon.InfoSign, "You have put {0} (pet {1}) in their cage.", itemValue.strName, itemValue.PetType.ToString());
								HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("puts {0} (pet {1}) back in their cage.", itemValue.strName, itemValue.PetType.ToString()));
							}
							else
							{
								player.SetCurrentPet(itemValue);

								player.SendNotification("Pet Uncaged", ENotificationIcon.InfoSign, "You have let {0} (pet {1}) out of their cage.", itemValue.strName, itemValue.PetType.ToString());
								HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("lets {0} (pet {1}) out of their cage.", itemValue.strName, itemValue.PetType.ToString()));
							}

							// Set active flag on value in DB
							CItemValuePet itemVal = (CItemValuePet)inventoryItem.Value;
							itemVal.IsActive = !bCurrentPet;
							inventoryItem.Value = itemVal;
							Database.Functions.Items.SaveItemValueAndStackSize(inventoryItem);

							showInventoryAction = EShowInventoryAction.HideIfVisible;
						}
						else
						{
							NetworkEventSender.SendNetworkEvent_SetPetName(player, itemValue.PetType, inventoryItem.DatabaseID);
						}
					}
				}
				else if (inventoryItem.ItemID == EItemID.FURNITURE)
				{
					player.SendNotification("Furniture", ENotificationIcon.ExclamationSign, "To use furniture, enter an interior belonging to you/your faction, and hit 'Edit Interior'.", null);
				}
				else if (inventoryItem.ItemID == EItemID.FISHING_ROD_AMATEUR)
				{
					player.TryToggleFishing(inventoryItem.ItemID);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.FISHING_ROD_INTERMEDIATE)
				{
					player.TryToggleFishing(inventoryItem.ItemID);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.FISHING_ROD_ADVANCED)
				{
					player.TryToggleFishing(inventoryItem.ItemID);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.FISHING_LINE)
				{
					player.SendNotification("Fishing Line", ENotificationIcon.ExclamationSign, "Fishing line is used with a fishing rod. Use a fishing rod to use this item.", null);
				}
				else if (inventoryItem.ItemID == EItemID.FISH_COOLER_BOX)
				{
					player.SendNotification("Fishing Cooler Box", ENotificationIcon.ExclamationSign, "Fishing coolers are used to store fish caught with a fishing rod. Visit a fishmonger to sell your fish.", null);
				}
				else if (inventoryItem.ItemID == EItemID.FISH)
				{
					const int HealthToGive = 20;

					if (player.Client.Health <= 100 - HealthToGive)
					{
						// Consume item
						player.Inventory.RemoveItem(inventoryItem);

						player.Client.Health += HealthToGive;
						player.Save();

						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("eats a {0}.", inventoryItem.GetName()));
						player.SendNotification("Health", ENotificationIcon.InfoSign, "You gained {0} health from eating.", HealthToGive);
					}
					else
					{
						player.SendNotification("Health", ENotificationIcon.ExclamationSign, "You are not hungry enough to eat a {0}", inventoryItem.GetName());
					}
				}
				else if (inventoryItem.ItemID == EItemID.PREMADE_CHAR_MASK)
				{
					if (player.CharacterType == ECharacterType.Premade)
					{
						CItemValueBasicBoolean itemValueData = inventoryItem.GetValueData<CItemValueBasicBoolean>();

						if (itemValueData.value)
						{
							player.SetPremadeMasked(false, true);
							HelperFunctions.Chat.SendAmeMessage(player, "removes their mask.");
						}
						else
						{
							HelperFunctions.Chat.SendAmeMessage(player, "puts on a mask.");
							player.SetPremadeMasked(true, true);
						}

						// Set active flag on value in DB
						itemValueData.value = !itemValueData.value;
						Database.Functions.Items.SaveItemValueAndStackSize(inventoryItem);
					}
					else
					{
						player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You must be a premade character to use this item.", null);
					}
				}
				else if (inventoryItem.ItemID == EItemID.MARIJUANA_SEED)
				{
					CItemInstanceDef item = CItemInstanceDef.FromItemID(EItemID.PLANTING_POT);
					if (!player.Inventory.HasItem(item, false, out CItemInstanceDef outItem))
					{
						player.SendNotification("Planting", ENotificationIcon.ExclamationSign, "You need a pot to plant this seed in.");
						return;
					}

					CItemValueMarijuanaPlant plant = new CItemValueMarijuanaPlant(EGrowthState.Seed, 0, 0, false, false, 0);
					CItemInstanceDef planted = CItemInstanceDef.FromTypedObjectNoDBID(EItemID.MARIJUANA_PLANT, plant, 1);
					if (!player.Inventory.CanGiveItem(planted, out _, out var giveItemError))
					{
						player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "Cannot plant seed in pot: " + giveItemError);
						return;
					}

					player.Inventory.DecrementStackSizeOverMultipleInstances(EItemID.PLANTING_POT, 1);
					player.Inventory.DecrementStackSizeOverMultipleInstances(EItemID.MARIJUANA_SEED, 1);

					player.Inventory.AddItemToNextFreeSuitableSlot(planted, EShowInventoryAction.DoNothing, EItemID.None, null);
				}
				else if (inventoryItem.ItemID == EItemID.MARIJUANA_PLANT)
				{
					CItemValueMarijuanaPlant plant = inventoryItem.GetValueData<CItemValueMarijuanaPlant>();
					if (plant.growthState < EGrowthState.FullyGrown)
					{
						player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "This plant is still growing! Place it down in the world to keep growing.");
						return;
					}

					CItemInstanceDef item = CItemInstanceDef.FromItemID(EItemID.SHEERS);
					if (!player.Inventory.HasItem(item, false, out CItemInstanceDef outItem))
					{
						player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "You need shears to clip this plant.");
						return;
					}

					player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "Place the plant on the ground and use your shears to cut it.");
					return;
				}
				else if (inventoryItem.ItemID == EItemID.WATERING_CAN)
				{
					MarijuanaSystem.WaterNearbyPlant(player);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.SHEERS)
				{
					MarijuanaSystem.SheerNearbyPlant(player);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.FERTILIZER)
				{
					player.Inventory.DecrementStackSizeOverMultipleInstances(EItemID.FERTILIZER, 1);
					MarijuanaSystem.FertilizeNearbyPlant(player);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.MARIJUANA_DRYING)
				{
					MarijuanaSystem.SplitMarijuanaDrying(player, inventoryItem);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.BOOM_MIC)
				{
					NewsSystem.ToggleBoomMic(player);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.VIDEO_CAMERA)
				{
					NewsSystem.ToggleHandCamera(player);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.MICROPHONE)
				{
					NewsSystem.ToggleNewsMicrophone(player);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.NEWS_CAMERA)
				{
					NewsSystem.PlaceNewsCamera(player);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.OPTICAL_BINOCULARS)
				{
					BinocularSystem.ToggleBinoculars(player, EBinocularsType.Regular);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.NV_BINOCULARS)
				{
					BinocularSystem.ToggleBinoculars(player, EBinocularsType.Advanced);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.THERMAL_BINOCULARS)
				{
					BinocularSystem.ToggleBinoculars(player, EBinocularsType.ThermalOnly);
					showInventoryAction = EShowInventoryAction.HideIfVisible;
				}
				else if (inventoryItem.ItemID == EItemID.CIGARETTE_PACK_JERED)
				{
					CItemInstanceDef item = CItemInstanceDef.FromBasicValueNoDBID(EItemID.CIGARETTE_JERED, 1);

					player.Inventory.DecrementStackSizeOverMultipleInstances(inventoryItem.ItemID, 1);
					player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);

					HelperFunctions.Chat.SendAmeMessage(player, "takes a cigarette out of the pack.");
				}
				else if (inventoryItem.ItemID == EItemID.CIGARETTE_PACK_BLUE)
				{
					CItemInstanceDef item = CItemInstanceDef.FromBasicValueNoDBID(EItemID.CIGARETTE_BLUE, 1);

					player.Inventory.DecrementStackSizeOverMultipleInstances(inventoryItem.ItemID, 1);
					player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);

					HelperFunctions.Chat.SendAmeMessage(player, "takes a cigarette out of the pack.");
				}
				else if (inventoryItem.ItemID == EItemID.CIGAR_CASE_BASIC)
				{
					CItemInstanceDef item = CItemInstanceDef.FromBasicValueNoDBID(EItemID.CIGAR_BASIC, 1);

					player.Inventory.DecrementStackSizeOverMultipleInstances(inventoryItem.ItemID, 1);
					player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);

					HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("takes a cigar out of the box."));
				}
				else if (inventoryItem.ItemID == EItemID.CIGAR_CASE_PREMIUM)
				{
					CItemInstanceDef item = CItemInstanceDef.FromDefaultValue(EItemID.CIGAR_PREMIUM, 1);

					player.Inventory.DecrementStackSizeOverMultipleInstances(inventoryItem.ItemID, 1);
					player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);

					HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("takes a cigar out of the box."));
				}
				else if (inventoryItem.ItemID == EItemID.CIGARETTE_JERED || inventoryItem.ItemID == EItemID.CIGARETTE_BLUE || inventoryItem.ItemID == EItemID.JOINT || inventoryItem.ItemID == EItemID.CIGAR_BASIC || inventoryItem.ItemID == EItemID.CIGAR_PREMIUM)
				{
					CItemInstanceDef item = CItemInstanceDef.FromItemID(EItemID.LIGHTER);
					if (!player.Inventory.HasItem(item, false, out CItemInstanceDef outItem))
					{
						player.SendNotification("Smoking", ENotificationIcon.ExclamationSign, Helpers.FormatString("You need a lighter to light up a {0}", inventoryItem.GetName()));
						return;
					}

					player.Inventory.RemoveItem(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("lights up a {0}", inventoryItem.GetName()));

					ESmokingItemType m_SmokingItemType = ESmokingItemType.None;
					string strSmokingType = string.Empty;

					if (inventoryItem.ItemID == EItemID.CIGARETTE_BLUE || inventoryItem.ItemID == EItemID.CIGARETTE_JERED)
					{
						m_SmokingItemType = ESmokingItemType.Cigarette;
						strSmokingType = "Cigarette";
					}
					else if (inventoryItem.ItemID == EItemID.CIGAR_BASIC)
					{
						m_SmokingItemType = ESmokingItemType.CigarBasic;
						strSmokingType = "Cigar";
					}
					else if (inventoryItem.ItemID == EItemID.CIGAR_PREMIUM)
					{
						m_SmokingItemType = ESmokingItemType.CigarHighClass;
						strSmokingType = "Cigar";
					}
					else if (inventoryItem.ItemID == EItemID.JOINT)
					{
						m_SmokingItemType = ESmokingItemType.Joint;
						strSmokingType = "Joint";
						player.SetDrugEffectEnabled(EDrugEffect.Weed, 18000);
					}

					showInventoryAction = EShowInventoryAction.HideIfVisible;
					player.PushChatMessageWithColor(EChatChannel.Notifications, 231, 217, 176, Helpers.FormatString("Use /throwaway to throw your {0} on the ground and /smoke [1-3] to change animations.", strSmokingType));
					player.SetSmokingOfType(true, m_SmokingItemType);
					player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking@male@male_b@idle_a", "idle_a", true, true, false, 1000 * 1000, false);
				}
				else if (inventoryItem.ItemID == EItemID.ROLLING_PAPERS)
				{
					CItemInstanceDef item = CItemInstanceDef.FromItemID(EItemID.WEED);
					if (!player.Inventory.HasItem(item, false, out CItemInstanceDef outItem))
					{
						player.SendNotification("Joint", ENotificationIcon.ExclamationSign, "You can't roll a joint without weed.");
						return;
					}
					else
					{
						player.PushChatMessageWithColor(EChatChannel.Global, 255, 255, 255, "You have to put the {0} inside the rolling papers. Not the other way around.", Helpers.ColorString(0, 102, 0, "weed"));
						return;
					}
				}
				else if (inventoryItem.ItemID == EItemID.LIGHTER)
				{
					player.SendNotification("Smoking", ENotificationIcon.ExclamationSign, "Use the lighter to light up a cigarette or a cigar");
					return;
				}
				else if (inventoryItem.ItemID == EItemID.NOTE)
				{
					CItemValueNote noteValue = (CItemValueNote)inventoryItem.Value;
					showInventoryAction = EShowInventoryAction.DoNothing;
					player.PushChatMessageWithColor(EChatChannel.Nearby, 255, 255, 255, Helpers.FormatString("This note reads: {0}", Helpers.ColorString(231, 217, 176, "{0}", noteValue.NoteMessage)));
				}
				else if (inventoryItem.ItemID == EItemID.PROPERTY_KEY)
				{
					CItemValueBasic itemVal = (CItemValueBasic)inventoryItem.Value;
					CPropertyInstance property = PropertyPool.GetPropertyInstanceFromID(Convert.ToInt64(itemVal.value));
					string strReasonCantAdd = "";
					bool CanAddCustomInt = false;

					if (property != null)
					{
						if (property.Model.OwnerType == EPropertyOwnerType.Faction)
						{
							if (player.IsFactionManager(property.Model.OwnerId))
							{
								CanAddCustomInt = true;
							}
							else
							{
								strReasonCantAdd = "You are not a manager in the faction which this property belongs to";
							}
						}
						else if (property.Model.OwnerType == EPropertyOwnerType.Player)
						{
							if (property.Model.OwnerId == player.ActiveCharacterDatabaseID)
							{
								CanAddCustomInt = true;
							}
							else
							{
								strReasonCantAdd = "You are not the owner of this property.";
							}
						}

						// do we have enough GC?
						if (CanAddCustomInt)
						{
							int GCCost = await MapLoader.DetermineGCCostForMapUpload(property.Model.Id).ConfigureAwait(true);

							int donatorCurrency = await player.GetDonatorCurrency().ConfigureAwait(true);

							if (donatorCurrency < GCCost)
							{
								CanAddCustomInt = false;
								strReasonCantAdd = Helpers.FormatString("A map upload to this property would cost {0} GC, you have {1} GC.", GCCost, donatorCurrency);
							}
						}

						if (CanAddCustomInt)
						{
							NetworkEventSender.SendNetworkEvent_CustomInterior_OpenCustomIntUI(player, property.Model.Id, property.Model.Name);
							showInventoryAction = EShowInventoryAction.HideIfVisible;
						}
						else
						{
							player.SendNotification("Add Custom Interior", ENotificationIcon.ExclamationSign, strReasonCantAdd);
						}
					}
					return;
				}
				else if (inventoryItem.ItemID == EItemID.BADGE)
				{
					CItemValueBadge badgeValue = (CItemValueBadge)inventoryItem.Value;
					showInventoryAction = EShowInventoryAction.HideIfVisible;
					player.SetBadge(player.BadgeEnabled ? false : true, badgeValue.Color, badgeValue.factionShortName, badgeValue.badgeName);
				}
				else if (inventoryItem.ItemID == EItemID.OUTFIT)
				{
					showInventoryAction = EShowInventoryAction.HideIfVisible;
					player.ActivateOutfit(inventoryItem);
					HelperFunctions.Chat.SendAmeMessage(player, "changes their clothes");
				}
				else if (inventoryItem.ItemID == EItemID.DUTY_OUTFIT)
				{
					showInventoryAction = EShowInventoryAction.HideIfVisible;

					CItemValueDutyOutfit dutyOutfitValue = (CItemValueDutyOutfit)inventoryItem.Value;
					if (player.DutyType == EDutyType.None)
					{
						player.SendNotification("Change Duty Outfit", ENotificationIcon.ExclamationSign, "You are not on duty");
					}
					else if (player.DutyType != dutyOutfitValue.DutyType)
					{
						player.SendNotification("Change Duty Outfit", ENotificationIcon.ExclamationSign, Helpers.FormatString("You are on duty type of {0}, this outfit is for type {1}.", player.DutyType.ToString().Replace("_", " "), dutyOutfitValue.DutyType.ToString().Replace("_", " ")));
					}
					else
					{
						player.ActivateDutyOutfit(player.DutyType, inventoryItem);
						HelperFunctions.Chat.SendAmeMessage(player, "changes their clothes");
					}
				}
				else if (inventoryItem.ItemID == EItemID.WEAPON_PETROLCAN)
				{
					showInventoryAction = EShowInventoryAction.HideIfVisible;
					NetworkEventSender.SendNetworkEvent_InitiateJerryCanRefuel(player, inventoryItem.DatabaseID);
				}

				new Logging.Log(player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("Used item with ID: {0} - Stack size: {1}", inventoryItem.ItemID, inventoryItem.StackSize)).execute();
			}

			// TODO_OPTIMIZATION: Optimize this, we could in theory send the inventory multiple times due to the removes above which may or may not happen. But this transmit must happen in all cases
			// We must send this to unblock the client
			player.Inventory.TransmitFullInventory(showInventoryAction);
		}
	}

	public void RetuneRadio(CPlayer player, EntityDatabaseID radioDBID, int radioChannel)
	{
		// Verify ownership to avoid malicious clients
		CItemInstanceDef inventoryItem = player.Inventory.GetItemFromDBID(radioDBID);

		if (inventoryItem != null && inventoryItem.ItemID == EItemID.RADIO)
		{
			CItemValueBasic itemVal = (CItemValueBasic)inventoryItem.Value;
			itemVal.value = radioChannel;
			inventoryItem.Value = itemVal;
			Database.Functions.Items.SaveItemValueAndStackSize(inventoryItem);
			player.SendNotification("Radio", ENotificationIcon.InfoSign, "Your radio was {0}.", radioChannel == -1 ? " turned off" : $" tuned to channel {radioChannel}");

			// TODO_POST_LAUNCH: Make a inventoryItem.Save function that saves everything?

			player.Inventory.TransmitFullInventory(EShowInventoryAction.DoNothing);
		}
	}

	public void OnShowItem(CPlayer player, EntityDatabaseID dbid)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			CItemInstanceDef inventoryItem = player.Inventory.GetItemFromDBID(dbid);

			if (inventoryItem != null)
			{
				if (inventoryItem.ItemID == EItemID.WATCH)
				{
					DateTime ServerTime = Core.GetServerClock();
					DateTime ServerDate = DateTime.Today;

					HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("shows everyone a watch with time {0}:{1} and date {2}/{3}/{4}", ServerTime.Hour, ServerTime.Minute.ToString().PadRight(2, '0'), ServerDate.Month, ServerDate.Day, ServerDate.Year));
				}
				else
				{
					HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("shows everyone a {0}.", inventoryItem.GetName()));
				}
			}
		}
	}

	// TOOD_INVENTORY: dont let drop, use, show when in vehicle
	// TODO_INVENTORY: Check all player.Inventory calls for which ones may need vehicle support

	public void OnDestroyItem(CPlayer player, EntityDatabaseID dbid)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			CItemInstanceDef inventoryItem = player.Inventory.GetItemFromDBID(dbid);

			if (inventoryItem != null)
			{
				// if we destroy a badge and we are still wearing it remove the entity data
				if (inventoryItem.ItemID == EItemID.BADGE && player.BadgeEnabled)
				{
					player.SetBadge(false, System.Drawing.Color.FromArgb(0, 0, 0));
				}

				bool bItemRemoved = player.Inventory.RemoveItem(inventoryItem, EShowInventoryAction.Show, true, true);

				// SECURITY: If item wasn't removed, don't do anything
				if (!bItemRemoved)
				{
					return;
				}

				CheckForAndHandleDropOrDestroyFishingItem(player, inventoryItem.ItemID);
				CheckForAndHandleDropOrDestroyPremadeMasks(player, inventoryItem);
				HandleItemDroppedOrDestroyedGenerics(player, inventoryItem, "destroys");
				CheckForAndHandleDropOrDestroyClothingItemInsideOutfit(player, inventoryItem);

				new Logging.Log(player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("Destroyed item with ID: {0} - Stack size: {1}", inventoryItem.ItemID, inventoryItem.StackSize)).execute();
			}
		}

		// TODO_OPTIMIZATION: Optimize this, we could in theory send the inventory multiple times due to the removes above which may or may not happen. But this transmit must happen in all cases
		// We must send this to unblock the client
		player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
	}

	private void HandleItemDroppedOrDestroyedGenerics(CPlayer player, CItemInstanceDef inventoryItem, string mePrefix)
	{
		// Any per-item reactions to dropping the item should go here, before the new data is entered into the DB
		bool bDoGenericMe = true;

		EItemID itemID = inventoryItem.ItemID;
		if (itemID == EItemID.CLOTHES)
		{
			CItemValueClothingPremade itemValue = (CItemValueClothingPremade)inventoryItem.Value;

			// Was the player wearing this?
			if (itemValue.IsActive)
			{
				itemValue.IsActive = false; // no need to update DB as we're just about to destroy it and re-write it to the DB as a world item

				// NOTE: This function will apply naked skin if no valid clothing item is found
				player.ApplySkinFromInventory();

				HelperFunctions.Chat.SendAmeMessage(player, "removes their clothes.");
				bDoGenericMe = false;
			}
		}
		else if ((itemID >= EItemID.CLOTHES_CUSTOM_FACE && itemID <= EItemID.CLOTHES_CUSTOM_TOPS) || ItemHelpers.IsItemIDAProp(itemID))
		{
			CItemValueClothingCustom itemValue = (CItemValueClothingCustom)inventoryItem.Value;

			// Was the player wearing this?
			if (itemValue.IsActive)
			{
				itemValue.IsActive = false; // no need to update DB as we're just about to destroy it and re-write it to the DB as a world item

				// NOTE: This function will apply naked skin if no valid clothing item is found
				player.ApplySkinFromInventory();

				HelperFunctions.Chat.SendAmeMessage(player, "removes an item of clothing.");
				bDoGenericMe = false;
			}
		}
		else if (itemID == EItemID.PET)
		{
			// destroy pet
			player.SetCurrentPet(null);
		}

		// Remove weapons
		player.Inventory.SynchronizeAllWeaponsAndAmmoWithInventory();

		if (bDoGenericMe)
		{
			HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("{1} a {0}.", inventoryItem.GetName(), mePrefix));
		}
		// end reactions
	}

	private void CheckForAndHandleDropOrDestroyFishingItem(CPlayer player, EItemID itemID)
	{
		// if we drop anything fishing related, stop fishing
		if (itemID == EItemID.FISHING_LINE || itemID == EItemID.FISHING_ROD_AMATEUR || itemID == EItemID.FISHING_ROD_INTERMEDIATE || itemID == EItemID.FISHING_ROD_ADVANCED || itemID == EItemID.FISH_COOLER_BOX)
		{
			if (player.IsFishing())
			{
				player.SendNotification("Fishing", ENotificationIcon.ExclamationSign, "You have stopped fishing due to disposing of a required item.", null);
				player.StopFishing();
			}
		}
	}

	private void CheckForAndHandleDropOrDestroyClothingItemInsideOutfit(CPlayer player, CItemInstanceDef itemDef)
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
			List<CItemInstanceDef> lstOutfits = player.Inventory.GetAllOutfits();
			foreach (CItemInstanceDef outfit in lstOutfits)
			{
				CItemValueOutfit outfitData = outfit.GetValueData<CItemValueOutfit>();

				// check props
				foreach (var prop in outfitData.Props)
				{
					if (prop.Value == itemDef.DatabaseID)
					{
						outfitData.Props[prop.Key] = -1;
						break;
					}
				}

				// check clothes
				foreach (var clothesItem in outfitData.Clothes)
				{
					if (clothesItem.Value == itemDef.DatabaseID)
					{
						outfitData.Clothes[clothesItem.Key] = -1;
						break;
					}
				}
			}
		}
	}

	private void CheckForAndHandleDropOrDestroyPremadeMasks(CPlayer player, CItemInstanceDef inventoryItem)
	{
		// Custom char masks are handled by the clothing system, so no need to worry about it
		if (inventoryItem.ItemID == EItemID.PREMADE_CHAR_MASK)
		{
			CItemValueBasicBoolean itemValueData = inventoryItem.GetValueData<CItemValueBasicBoolean>();
			if (itemValueData.value)
			{
				player.SetPremadeMasked(false, true);
				HelperFunctions.Chat.SendAmeMessage(player, "removes their mask.");
			}
		}
	}

	public void OnDropItem(CPlayer player, EntityDatabaseID dbid, float x, float y, float z, bool bIsUseItemDrop)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else if (player.IsPreviewingProperty)
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot drop an item in a property which you are previewing.", null);
		}
		else
		{
			CItemInstanceDef inventoryItem = player.Inventory.GetItemFromDBID(dbid);

			if (inventoryItem != null)
			{
				EItemID itemID = inventoryItem.ItemID;
				bool bCanDrop = ItemDefinitions.g_ItemDefinitions[itemID].CanDrop;
				bool bIsAdminOnDuty = player.IsAdmin(EAdminLevel.Admin) && player.AdminDuty;
				bool bIsDutyItem = false;
				bool bIsLegalWeapon = false;
				if (inventoryItem.Value is CItemValueBasic)
				{
					CItemValueBasic itemValueBasic = inventoryItem.Value as CItemValueBasic;
					bIsDutyItem = itemValueBasic.duty;

					// Is it a weapon?
					if (inventoryItem.IsFirearm())
					{
						bIsLegalWeapon = itemValueBasic.is_legal;
					}
				}

				if (bIsDutyItem)
				{
					player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot drop duty items.", null);
				}
				else if (!bCanDrop)
				{
					player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot drop this item as it is undroppable.", null);
				}
				else if (bIsLegalWeapon && !bIsAdminOnDuty)
				{
					player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot drop legal firearms.", null);
				}
				else
				{
					bool bItemRemoved = player.Inventory.RemoveItem(inventoryItem, EShowInventoryAction.Show, true, false);

					// SECURITY: If item wasn't removed, don't drop a world item, could be an exploit
					if (!bItemRemoved)
					{
						return;
					}

					float fRotZ = player.Client.Rotation.Z;

					Int64 characterID = player.ActiveCharacterDatabaseID;
					uint dimension = player.Client.Dimension;

					CheckForAndHandleDropOrDestroyFishingItem(player, itemID);
					CheckForAndHandleDropOrDestroyPremadeMasks(player, inventoryItem);
					HandleItemDroppedOrDestroyedGenerics(player, inventoryItem, "drops");
					CheckForAndHandleDropOrDestroyClothingItemInsideOutfit(player, inventoryItem);

					if (!bIsUseItemDrop)
					{
						if (itemID == EItemID.BOOMBOX)
						{
							// Set to -1, saying no player dropped it / it's able to be picked up
							((CItemValueBoombox)inventoryItem.Value).placedBy = -1;
						}
					}

					CItemInstanceDef ItemInstance = CItemInstanceDef.FromObjectNoDBIDWithStackSize(itemID, inventoryItem.Value, EItemSocket.None, 0, EItemParentTypes.World, inventoryItem.StackSize);
					Database.Functions.Items.CreateWorldItem(ItemInstance, new Vector3(x, y, z), fRotZ, characterID, dimension, (EntityDatabaseID insertID) =>
					{
						ItemInstance.DatabaseID = insertID;

						// TODO: Move this to a helper function? ConvertPlayerItemToWorld and vice-versa.

						// Create item
						WorldItemPool.CreateWorldItem(ItemInstance, new Vector3(x, y, z), fRotZ, characterID, dimension);

						// Update child items in db (don't have to do anything in memory since they arent loaded)
						// This works for containers inside containers because we only touch the root container's DBID
						Database.Functions.Items.UpdateChildItems(ItemInstance.DatabaseID, inventoryItem.DatabaseID, EItemParentTypes.Container, EItemSocket.None, EItemParentTypes.Container, () =>
						{
							new Logging.Log(player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("Dropped item with ID: {0} - Stack size: {1} - Position: {2}, {3}, {4} - Dimension {5}", itemID, inventoryItem.StackSize, x, y, z, dimension)).execute();
						});
					});


				}
			}
		}

		// TODO_OPTIMIZATION: Optimize this, we could in theory send the inventory multiple times due to the removes above which may or may not happen. But this transmit must happen in all cases
		// We must send this to unblock the client
		player.Inventory.TransmitFullInventory(EShowInventoryAction.Show);
	}


	public void OnRequestMailbox(CPlayer player, EntityDatabaseID PropertyID)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Mailbox", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(PropertyID);

			if (propertyInst != null && propertyInst.Model.EntranceType != EPropertyEntranceType.World)
			{
				if (player.IsWithinDistanceOf(propertyInst.Model.EntrancePosition, 5.0f, propertyInst.Model.EntranceDimension) && !player.Client.Dead)
				{
					EMailboxAccessType accessLevel = EMailboxAccessType.NoAccess;
					string strAccessMessage = String.Empty;

					// do we have a key to the property?
					bool bHasFactionForPropertyAndManager = propertyInst.IsPropertyForAnyPlayerFaction(player, true);
					if (bHasFactionForPropertyAndManager || propertyInst.HasKeys(player))
					{
						accessLevel = EMailboxAccessType.ReadWrite;

						if (bHasFactionForPropertyAndManager)
						{
							strAccessMessage = "You are accessing this mailbox with permissions to add or remove items due to being a faction manager.";
						}
						else
						{
							strAccessMessage = "You are accessing this mailbox with permissions to add or remove items due to having a key.";
						}
					}
					else if (player.IsAdmin(EAdminLevel.TrialAdmin, true)) // admin on duty
					{
						accessLevel = EMailboxAccessType.AdminAccess;
						strAccessMessage = "You are accessing this mailbox with permissions to add or remove items due to being an on-duty admin.";
					}
					else
					{
						accessLevel = EMailboxAccessType.ReadOnly;
						strAccessMessage = "You are accessing this mailbox with permissions to add items only due to not being the property owner.";
					}

					if (accessLevel != EMailboxAccessType.NoAccess)
					{
						player.SendNotification("Mailbox", ENotificationIcon.ExclamationSign, strAccessMessage, null);

						NetworkEventSender.SendNetworkEvent_PropertyMailboxDetails(player, propertyInst.Model.Id, accessLevel, propertyInst.Inventory.GetAllItems());

						player.SetCurrentPropertyInventory(propertyInst, accessLevel);
					}
				}
			}
		}
	}

	public void OnRequestVehicleInventory(CPlayer player, Vehicle gtaVehicle, EVehicleInventoryType inventoryType, bool bIsInvertedTrunk)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			// TODO: dist check from vehicle
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(gtaVehicle);
			if (pVehicle != null)
			{
				float fDist = (pVehicle.GTAInstance.Position - player.Client.Position).Length();
				if (fDist <= ItemConstants.g_fDistVehicleTrunkThresholdLarge) // Don't worry about checking small one for small vehicles, we can safely trust the client with 4 extra units of distance...
				{
					// Does the vehicle have a trunk?
					if (pVehicle.VehicleType == EVehicleType.PlayerOwned || pVehicle.VehicleType == EVehicleType.FactionOwned)
					{
						// Can we access it?
						bool bHasFactionForVehicle = pVehicle.IsVehicleForAnyPlayerFaction(player);
						bool bIsUnlockedAndNotFactionVehicle = !pVehicle.GTAInstance.Locked && pVehicle.VehicleType == EVehicleType.PlayerOwned;

						CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, pVehicle.m_DatabaseID);
						bool bHasKey = player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
						bool bIsAdminOnDuty = player.IsAdmin(EAdminLevel.TrialAdmin, true);

						if (pVehicle.GTAInstance.Locked)
						{
							player.SendNotification("Vehicle Inventory", ENotificationIcon.ExclamationSign, "The vehicle is locked.");
						}
						else if (bHasFactionForVehicle || bHasKey || bIsUnlockedAndNotFactionVehicle || bIsAdminOnDuty)
						{
							NetworkEventSender.SendNetworkEvent_VehicleInventoryDetails(player, inventoryType, pVehicle.Inventory.GetAllItems());

							player.SetCurrentVehicleInventory(pVehicle, inventoryType);

							if (inventoryType == EVehicleInventoryType.TRUNK)
							{
								pVehicle.SetDoorState(bIsInvertedTrunk ? EDataNames.VEH_DOOR_4 : EDataNames.VEH_DOOR_5, true);
								HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("opens the trunk."));
							}
						}
						else
						{
							player.SendNotification("Vehicle Inventory", ENotificationIcon.ExclamationSign, "You do not have access to this vehicle.");
						}
					}
					else
					{
						player.SendNotification("Vehicle Inventory", ENotificationIcon.ExclamationSign, "This vehicle does not have an inventory.");
					}
				}
				else
				{
					player.SendNotification("Vehicle Inventory", ENotificationIcon.ExclamationSign, "You are too far from the vehicle.");
				}
			}
		}
	}

	public void OnCloseVehicleInventory(CPlayer player, Vehicle gtaVehicle, EVehicleInventoryType inventoryType, bool bIsInvertedTrunk)
	{
		if (player.CurrentVehicleInventoryType != EVehicleInventoryType.NONE)
		{
			player.SetCurrentVehicleInventory(null, EVehicleInventoryType.NONE);

			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(gtaVehicle);
			if (pVehicle != null)
			{
				if (inventoryType == EVehicleInventoryType.TRUNK)
				{
					pVehicle.SetDoorState(bIsInvertedTrunk ? EDataNames.VEH_DOOR_4 : EDataNames.VEH_DOOR_5, false);
					HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("closed the trunk."));
				}
			}
		}
	}

	public void OnCloseFurnitureInventory(CPlayer player)
	{
		player.ResetCurrentFurnitureInventory();
	}

	public void OnClosePropertyInventory(CPlayer player)
	{
		player.ResetCurrentPropertyInventory();
	}

	public void OnRequestFurnitureInventory(CPlayer player, EntityDatabaseID furnitureDBID)
	{
		if (player.IsCuffed())
		{
			player.SendNotification("Inventory", ENotificationIcon.ExclamationSign, "You cannot perform this action while restrained.", null);
		}
		else
		{
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(player.Client.Dimension);
			if (propertyInst != null)
			{
				CPropertyFurnitureInstance furnInst = propertyInst.GetFurnitureItemFromDBID(furnitureDBID);

				if (furnInst != null)
				{
					float fDist = (furnInst.vecPos - player.Client.Position).Length();
					if (fDist <= ItemConstants.g_fDistFurnitureStorageThreshold)
					{
						// Does the vehicle have a trunk?
						if (propertyInst.Model.OwnerType == EPropertyOwnerType.Player || propertyInst.Model.OwnerType == EPropertyOwnerType.Faction)
						{
							// Can we access it?
							bool bHasFactionForProperty = propertyInst.IsPropertyForAnyPlayerFaction(player);
							CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, propertyInst.Model.Id);
							bool bHasKey = player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
							bool bIsAdminOnDuty = player.IsAdmin(EAdminLevel.TrialAdmin, true);

							if (bHasFactionForProperty || bHasKey || bIsAdminOnDuty)
							{
								// NOTE: We hotload this from DB, we don't preload all inventories for all furniture items because that could be a LOT of items
								Database.Functions.Items.GetInventoryForFurnitureItemRecursive(furnitureDBID, (List<CItemInstanceDef> lstFurnitureInventory) =>
								{
									NetworkEventSender.SendNetworkEvent_FurnitureInventoryDetails(player, lstFurnitureInventory);

									player.SetCurrentFurnitureInventory(furnitureDBID);
								});
							}
							else
							{
								player.SendNotification("Furniture Inventory", ENotificationIcon.ExclamationSign, "You do not have access to this piece of furniture.");
							}
						}
						else
						{
							player.SendNotification("Furniture Inventory", ENotificationIcon.ExclamationSign, "This piece of furniture does not have an inventory.");
						}
					}
					else
					{
						player.SendNotification("Furniture Inventory", ENotificationIcon.ExclamationSign, "You are too far from the vehicle.");
					}
				}
			}
		}
	}

	private void OnSavePetName(CPlayer player, Int64 petID, string strName)
	{
		// Verify ownership to avoid malicious clients
		CItemInstanceDef inventoryItem = player.Inventory.GetItemFromDBID(petID);

		if (inventoryItem != null && inventoryItem.ItemID == EItemID.PET)
		{
			CItemValuePet itemVal = (CItemValuePet)inventoryItem.Value;
			itemVal.strName = strName;
			inventoryItem.Value = itemVal;
			Database.Functions.Items.SaveItemValueAndStackSize(inventoryItem);
			player.SendNotification("Pet", ENotificationIcon.InfoSign, "Your pet {0} was named '{1}'.", itemVal.PetType.ToString(), strName);

			// TODO_POST_LAUNCH: Make a inventoryItem.Save function that saves everything?

			player.Inventory.TransmitFullInventory(EShowInventoryAction.DoNothing);

			// Now use the item, they were trying to use it in the first place, we forced them to rename
			OnUseItem(player, petID);
		}
	}

	private void OnShareDutyOutfit_Outcome(CPlayer player, bool bAccepted)
	{
		if (bAccepted)
		{
			CItemValueDutyOutfit outfitDetails = player.GetPendingDutyOutfitShare();
			if (outfitDetails != null)
			{
				// give outfit item
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.DUTY_OUTFIT, outfitDetails);
				player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);

				player.SendNotification("Duty Outfit", ENotificationIcon.InfoSign, "Your new outfit is available in the editor & in your inventory under the name '{0}'", outfitDetails.Name);

				player.ResetPendingDutyOutfitShare();
			}
		}
		else
		{
			player.ResetPendingDutyOutfitShare();
		}
	}

	private void OnShareDutyOutfit(CPlayer sourcePlayer, EntityDatabaseID outfitID, Player rageTargetPlayer)
	{
		WeakReference<CPlayer> targetPlayerRef = PlayerPool.GetPlayerFromClient(rageTargetPlayer);
		CPlayer targetPlayer = targetPlayerRef.Instance();

		if (targetPlayer != null)
		{
			// get the outfit
			CItemInstanceDef dutyOutfit = null;
			List<CItemInstanceDef> lstDutyOutfits = sourcePlayer.Inventory.GetAllDutyOutfits();
			foreach (CItemInstanceDef iterOutfit in lstDutyOutfits)
			{
				if (iterOutfit.DatabaseID == outfitID)
				{
					dutyOutfit = iterOutfit;
					break;
				}
			}

			if (dutyOutfit != null)
			{
				CItemValueDutyOutfit itemValue = (CItemValueDutyOutfit)dutyOutfit.Value;
				EDutyType outfitType = itemValue.DutyType;

				if (targetPlayer.IsEligbleToUseDutyOfType(outfitType))
				{
					targetPlayer.SetPendingDutyOutfitShare(itemValue);
					NetworkEventSender.SendNetworkEvent_DutyOutfitShareInformClient(targetPlayer, sourcePlayer.GetCharacterName(ENameType.StaticCharacterName), itemValue.Name);
				}
				else if (targetPlayer.CharacterType == ECharacterType.Premade && itemValue.CharType == EDutyOutfitType.Custom)
				{
					sourcePlayer.SendNotification("Share Duty Outfit", ENotificationIcon.InfoSign, "Your duty outfit could not be shared - {0} is a premade character and '{1} requires a custom character.", targetPlayer.GetCharacterName(ENameType.StaticCharacterName), outfitType);
				}
				else if (targetPlayer.Gender != sourcePlayer.Gender)
				{
					sourcePlayer.SendNotification("Share Duty Outfit", ENotificationIcon.InfoSign, "Your duty outfit could not be shared - the outfit is for a {0} character and {1} is a {2}", sourcePlayer.Gender, targetPlayer.GetCharacterName(ENameType.StaticCharacterName), targetPlayer.Gender);
				}
				else
				{
					sourcePlayer.SendNotification("Share Duty Outfit", ENotificationIcon.InfoSign, "Your duty outfit could not be shared - {0} is not eligible to use outfits of type '{1}'.", targetPlayer.GetCharacterName(ENameType.StaticCharacterName), outfitType);
				}
			}
		}
		else
		{
			sourcePlayer.SendNotification("Share Duty Outfit", ENotificationIcon.InfoSign, "Your duty outfit could not be shared - the player was not found.");

			// This even will reset the clientside UI, since we didnt actually create a report for them
			NetworkEventSender.SendNetworkEvent_AdminReportEnded(sourcePlayer);
		}
	}
}