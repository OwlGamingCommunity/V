using Newtonsoft.Json.Linq;
using RAGE.Elements;
using System;
using System.Collections.Generic;

public enum EInventoryUIActionsMode
{
	Full = 0,
	MoveOnly = 1,
	UseOnly = 2,
	DutyOutfit = 3,
}


public class PlayerInventory
{
	private CGUIPlayerInventory m_PlayerInventoryUI = new CGUIPlayerInventory(() => { });

	public const int g_InventorySlots = 20;
	public const float g_fMaxInventoryWeight = 20.0f;

	private static List<CItemInstanceDef> m_lstPlayerInventory = new List<CItemInstanceDef>();
	private static List<CItemInstanceDef> m_lstVehicleInventory = new List<CItemInstanceDef>();
	private static List<CItemInstanceDef> m_lstFurnitureInventory = new List<CItemInstanceDef>();
	private static List<CItemInstanceDef> m_lstPropertyInventory = new List<CItemInstanceDef>();

	private bool IsSocketAContainerWhichOnlyAllowsRestrictedActions(EItemSocket socket)
	{
		return InventoryHelpers.IsSocketAVehicleSocket(socket) || socket == EItemSocket.PlacedFurnitureStorage || socket == EItemSocket.Property_Mailbox;
	}

	private void ResetVehicleAndFurnitureWindowsAndSocketVisibilities()
	{
		List<InventoryWindow> lstWindowsToDestroy = new List<InventoryWindow>();
		foreach (var window in g_lstWindows)
		{
			if (IsSocketAContainerWhichOnlyAllowsRestrictedActions(window.GetItemSocket()) || IsSocketAContainerWhichOnlyAllowsRestrictedActions(window.GetItemSocketAtCreation()))
			{
				window.RemoveFromUI(m_PlayerInventoryUI);
				lstWindowsToDestroy.Add(window);
			}
		}

		foreach (var destroyWindow in lstWindowsToDestroy)
		{
			g_lstWindows.Remove(destroyWindow);
		}

		ItemSystem.GetVehicleInventory().CloseVehicleInventory();
		m_lstVehicleInventory.Clear();

		ItemSystem.GetFurnitureInventory().CloseFurnitureInventory();
		m_lstFurnitureInventory.Clear();

		ItemSystem.GetPropertyInventory().ClosePropertyInventory();
		m_lstPropertyInventory.Clear();
	}

	private string GetDisplayNameForSocket(EItemSocket socket)
	{
		if (socket == EItemSocket.Wallet)
		{
			float fMoney = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.MONEY);
			return Helpers.FormatString("Wallet (${0:0.00})", fMoney);
		}
		else if (socket == EItemSocket.PlacedFurnitureStorage)
		{
			var furnitureDef = ItemSystem.GetFurnitureInventory().GetCurrentFurnitureItemDefinition();
			if (furnitureDef != null)
			{
				return furnitureDef.Name;
			}
		}
		else if (socket == EItemSocket.Property_Mailbox)
		{
			return "Mailbox";
		}

		return System.Text.RegularExpressions.Regex.Replace(socket.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim().Replace("_", " ");
	}

	public void SetCurrentVehicleInventory(List<CItemInstanceDef> lstVehicleInventory, EItemSocket vehicleSocket)
	{
		m_lstVehicleInventory = lstVehicleInventory;
		RefreshInventory();
	}

	public void SetCurrentFurnitureInventory(List<CItemInstanceDef> lstFurnitureInventory)
	{
		m_lstFurnitureInventory = lstFurnitureInventory;
		RefreshInventory();
	}

	public void SetCurrentPropertyInventory(List<CItemInstanceDef> lstPropertyInventory)
	{
		m_lstPropertyInventory = lstPropertyInventory;
		RefreshInventory();
	}

	CItemInstanceDef m_itemBeingOperatedUpon = null;
	CItemInstanceDef m_DutyOutfitBeingShared = null;

	private Dictionary<RAGE.Elements.Entity, Dictionary<EItemSocket, RAGE.Elements.MapObject>> m_dictWorldPlayerItems = new Dictionary<RAGE.Elements.Entity, Dictionary<EItemSocket, RAGE.Elements.MapObject>>();

	public PlayerInventory()
	{
		NetworkEvents.LocalPlayerInventoryFull += OnLocalPlayerInventory;
		NetworkEvents.ChangeCharacterApproved += OnChangeCharacter;

		RageEvents.RAGE_OnPlayerQuit += OnPlayerDisconnected;

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleInventory, ToggleInventory);

		RageEvents.RAGE_OnTick_LowFrequency += OnTick_Infrequent;

		NetworkEvents.CharacterSelectionApproved += OnSpawn;

		NetworkEvents.HideInventory += HideInventory;

		UIEvents.OnDestroyItem_Cancel += OnDestroyItem_Cancel;
		UIEvents.OnDestroyItem_Confirm += OnDestroyItem_Confirm;

		UIEvents.ShowItemInfo += OnShowItemInfo;
		UIEvents.ExpandContainer += OnExpandContainer;
		UIEvents.GotoSplitItem += OnGotoSplitItem;
		UIEvents.Inventory_CloseWindow += OnCloseWindow;
		UIEvents.Inventory_GoBack += OnGoBack;

		UIEvents.ResetSplitItem += OnResetSplitItem;
		UIEvents.SplitItem += OnSplitItem;
		UIEvents.CopyToClipboardItemValue += OnCopyToClipboardItemValue;

		NetworkEvents.DutyOutfitShareInformClient += OnInformDutyOutfitBeingShared;
		UIEvents.IncomingDutyOutfitShare_Accept += () => { IncomingDutyOutfitShareOutcome(true); };
		UIEvents.IncomingDutyOutfitShare_Decline += () => { IncomingDutyOutfitShareOutcome(false); };

		// Items on body
		// TODO: Separate location?
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_0, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.Heart, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_1, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.Back, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_2, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.RearPockets, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_3, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.FrontPockets, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_4, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.LeftWaist, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_5, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.RightWaist, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_6, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.BackPants, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_7, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.FrontPants, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_8, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.LeftAnkle, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_9, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.RightAnkle, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_10, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.Chest, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_11, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.Head, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_17, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.LeftHand, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_18, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.RightHand, value); });
		RageEvents.AddDataHandler(EDataNames.ITEM_SOCKET_23, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateItemSocketInternal(entity, EItemSocket.ChestSling, value); });

		// duty share (we dont care about dropdown changed so we dont subscribe
		UIEvents.ShareDutyOutfit_SelectPlayer_Done += OnDutyOutfitEditor_SelectPreset_Done;
		UIEvents.ShareDutyOutfit_SelectPlayer_Cancel += OnDutyOutfitEditor_SelectPreset_Cancel;
	}

	private void OnInformDutyOutfitBeingShared(string strPlayerName, string strOutfitName)
	{
		GenericPromptHelper.ShowPrompt("Duty Outfit Share", Helpers.FormatString("{0} would like to share their custom duty outfit ('{1}') with you.", strPlayerName, strOutfitName), "Accept", "Decline",
			UIEventID.IncomingDutyOutfitShare_Accept, UIEventID.IncomingDutyOutfitShare_Decline, EPromptPosition.Center, false);
	}

	private void IncomingDutyOutfitShareOutcome(bool bAccepted)
	{
		NetworkEventSender.SendNetworkEvent_ShareDutyOutfit_Outcome(bAccepted);
	}

	CItemInstanceDef DetermineItemToUse(long itemDBID, EItemSocket itemSocket)
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		if (itemSocket == EItemSocket.None)
		{
			CItemInstanceDef itemInstanceDef = GetItemFromDBID(itemDBID);
			if (itemInstanceDef != null)
			{
				return itemInstanceDef;
			}
		}
		else
		{
			// need to determine item from socket
			foreach (CItemInstanceDef itemInstanceDef in lstCombinedInventory)
			{
				if (itemInstanceDef.CurrentSocket == itemSocket)
				{
					return itemInstanceDef;
				}
			}
		}

		return null;
	}

	private static bool IsSocketAFurnitureContainer(EItemSocket itemSocket)
	{
		return (itemSocket == EItemSocket.PlacedFurnitureStorage);
	}

	private static bool IsSocketAPropertyContainer(EItemSocket itemSocket)
	{
		return (itemSocket == EItemSocket.Property_Mailbox);
	}

	private static bool IsSocketAVehicleContainer(EItemSocket itemSocket)
	{
		return (itemSocket == EItemSocket.Vehicle_Trunk
			|| itemSocket == EItemSocket.Vehicle_Seats
			|| itemSocket == EItemSocket.Vehicle_Console_And_Glovebox);
	}

	private static bool IsSocketAContainer(EItemSocket itemSocket)
	{
		return (itemSocket == EItemSocket.RearPockets
			|| itemSocket == EItemSocket.FrontPockets
			|| itemSocket == EItemSocket.Clothing
			|| itemSocket == EItemSocket.Outfit
			|| itemSocket == EItemSocket.Vehicle_Trunk
			|| itemSocket == EItemSocket.Vehicle_Seats
			|| itemSocket == EItemSocket.Vehicle_Console_And_Glovebox
			|| itemSocket == EItemSocket.Furniture
			|| itemSocket == EItemSocket.Wallet
			|| itemSocket == EItemSocket.Keyring
			|| itemSocket == EItemSocket.PlacedFurnitureStorage
			|| itemSocket == EItemSocket.Property_Mailbox);
	}

	private static bool DoesSocketHaveIcon(EItemSocket itemSocket)
	{
		return (itemSocket != EItemSocket.Clothing
			&& itemSocket != EItemSocket.Vehicle_Trunk
			&& itemSocket != EItemSocket.Vehicle_Seats
			&& itemSocket != EItemSocket.Vehicle_Console_And_Glovebox
			&& itemSocket != EItemSocket.Furniture
			&& itemSocket != EItemSocket.Keyring
			&& itemSocket != EItemSocket.PlacedFurnitureStorage
			&& itemSocket != EItemSocket.Outfit
			&& itemSocket != EItemSocket.Property_Mailbox);
	}

	public static bool DoesLocalPlayerHaveItemOfType(EItemID itemID)
	{
		foreach (CItemInstanceDef itemDef in m_lstPlayerInventory)
		{
			if (itemDef.ItemID == itemID)
			{
				return true;
			}
		}

		return false;
	}

	public static List<CItemInstanceDef> GetContainerItems(bool bIsSocket, bool bIsSocketAContainer, EItemSocket itemSocket, CItemInstanceDef containerItemDef)
	{
		List<CItemInstanceDef> itemsInsideContainer = new List<CItemInstanceDef>();
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		// Socket
		if (bIsSocket && bIsSocketAContainer)
		{
			foreach (CItemInstanceDef item in lstCombinedInventory)
			{
				if (item.CurrentSocket == itemSocket)
				{
					itemsInsideContainer.Add(item);
				}
			}
		}
		else
		{
			if (containerItemDef != null)
			{
				foreach (CItemInstanceDef item in lstCombinedInventory)
				{
					if (item.ParentType == EItemParentTypes.Container && item.ParentDatabaseID == containerItemDef.DatabaseID)
					{
						itemsInsideContainer.Add(item);
					}
				}
			}
		}

		return itemsInsideContainer;
	}

	private void OnResetSplitItem()
	{
		g_itemBeingSplit = null;
	}

	private void OnSplitItem(uint newStackSize)
	{
		if (g_itemBeingSplit != null)
		{
			NetworkEventSender.SendNetworkEvent_SplitItem(g_itemBeingSplit.DatabaseID, g_itemBeingSplit.StackSize - newStackSize, newStackSize);
		}
	}

	private void OnGotoSplitItem()
	{
		// -1 and None, means it was the drop down in item info
		if (m_itemBeingOperatedUpon != null)
		{
			CItemInstanceDef itemBeingSplit = m_itemBeingOperatedUpon;
			m_PlayerInventoryUI.HideItemInfo();

			var itemDef = ItemDefinitions.g_ItemDefinitions[itemBeingSplit.ItemID];

			if (itemDef.IsSplittable())
			{
				g_itemBeingSplit = itemBeingSplit;
				m_PlayerInventoryUI.HideItemInfo();
				m_PlayerInventoryUI.ShowSplitItem(itemBeingSplit.GetName(), itemDef.MaxStack);
			}
			else
			{
				NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' cannot be split.", itemBeingSplit.GetName()), ENotificationIcon.InfoSign);
			}
		}
	}

	private static List<CItemInstanceDef> GetCombinedInventory()
	{
		List<CItemInstanceDef> lstCombinedInventory = new List<CItemInstanceDef>();
		lstCombinedInventory.AddRange(m_lstPlayerInventory);
		lstCombinedInventory.AddRange(m_lstVehicleInventory);
		lstCombinedInventory.AddRange(m_lstFurnitureInventory);
		lstCombinedInventory.AddRange(m_lstPropertyInventory);
		return lstCombinedInventory;
	}

	public void OnExpandContainer(int itemIndex, EItemSocket itemSocket, bool bClosewindowIfAlreadyExists = true)
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();
		m_PlayerInventoryUI.HideItemInfo();

		long dbid = -1;

		CItemInstanceDef containerItemDef = null;
		bool bIsSocket = itemSocket != EItemSocket.None;
		bool bIsSocketAContainer = IsSocketAContainer(itemSocket);
		if (bIsSocket && !bIsSocketAContainer) // cant expand a non-container socket unless theres an item inside, so lets find the root item
		{
			int index = 0;
			foreach (CItemInstanceDef item in GetCombinedInventory())
			{
				if (item.CurrentSocket == itemSocket)
				{
					// set item index to be the item index, since its -1 in this case
					dbid = item.DatabaseID;
					itemSocket = EItemSocket.None;

					containerItemDef = item;
					break;
				}

				++index;
			}
		}
		else if (!bIsSocket)
		{
			dbid = GetItemDBIDFromIndex(itemIndex);
			containerItemDef = DetermineItemToUse(dbid, itemSocket);
		}

		List<CItemInstanceDef> itemsInsideContainer = GetContainerItems(bIsSocket, bIsSocketAContainer, itemSocket, containerItemDef);

		if (containerItemDef != null)
		{
			var itemDef = ItemDefinitions.g_ItemDefinitions[containerItemDef.ItemID];

			// Check if the parent is a container item AND we have a window for it, if so we'll navigate that window instead of making a new one
			int window_index = -1;
			if (itemDef.IsContainer)
			{
				if (containerItemDef.ParentType == EItemParentTypes.Container)
				{
					// TODO_SIMPLE_INV: Make html use dbid also, so we can just get rid of indexes
					InventoryWindow window = GetWindowFromItemDBID(containerItemDef.ParentDatabaseID);

					if (window != null)
					{
						window_index = window.GetWindowID();
						window.UpdateToSubItem(m_PlayerInventoryUI, lstCombinedInventory, containerItemDef.DatabaseID, itemSocket, itemsInsideContainer);
					}
				}

				// check for a socket window
				if (containerItemDef.CurrentSocket != EItemSocket.None)
				{
					InventoryWindow window = GetWindowFromSocket(containerItemDef.CurrentSocket);

					if (window != null)
					{
						window_index = window.GetWindowID();
						window.UpdateToSubItem(m_PlayerInventoryUI, lstCombinedInventory, containerItemDef.DatabaseID, itemSocket, itemsInsideContainer);
					}
				}

				if (window_index == -1)
				{
					InventoryWindow existingWindowFromParentTree = GetWindowFromParentsTree(containerItemDef.DatabaseID);
					if (existingWindowFromParentTree == null)
					{
						window_index = CreateWindow(containerItemDef.GetName(), containerItemDef.DatabaseID, itemSocket, itemsInsideContainer, bClosewindowIfAlreadyExists);
					}
					else
					{
						window_index = g_lstWindows.IndexOf(existingWindowFromParentTree);
						existingWindowFromParentTree.UpdateToSubItem(m_PlayerInventoryUI, lstCombinedInventory, containerItemDef.DatabaseID, itemSocket, itemsInsideContainer);
					}
				}

				// create breadcrumbs
				if (window_index != -1)
				{
					GenerateBreadcrumbForWindow(window_index, containerItemDef, EItemSocket.None);
				}
			}
		}
		else if (bIsSocket && bIsSocketAContainer) // It's a socket container! We don't have to worry about items in sockets here, those are handled above
		{
			int window_index = -1;

			if (window_index == -1)
			{
				InventoryWindow existingWindowFromParentTree = null;
				if (existingWindowFromParentTree == null)
				{
					window_index = CreateWindow(GetDisplayNameForSocket(itemSocket), -1, itemSocket, itemsInsideContainer, bClosewindowIfAlreadyExists);
				}
				else
				{
					window_index = g_lstWindows.IndexOf(existingWindowFromParentTree);
					existingWindowFromParentTree.UpdateToSubItem(m_PlayerInventoryUI, lstCombinedInventory, -1, itemSocket, itemsInsideContainer);
				}
			}

			// create breadcrumbs
			if (window_index != -1)
			{
				GenerateBreadcrumbForWindow(window_index, containerItemDef, itemSocket);
			}
		}
	}

	private InventoryWindow GetWindowFromParentsTree(long DesiredItemDBID)
	{
		foreach (var window in g_lstWindows)
		{
			// Does this windows item have the requested item in its tree?
			long WindowItemDBID = window.GetItemDBID();
			if (WindowItemDBID != -1)
			{
				// skip the base item, this lets us close if we are opening the same item
				var baseItem = GetItemFromDBID(WindowItemDBID);

				if (baseItem != null)
				{
					CItemInstanceDef parentItemDefIter = GetItemFromDBID(baseItem.ParentDatabaseID);
					while (parentItemDefIter != null)
					{
						if (parentItemDefIter.DatabaseID == DesiredItemDBID)
						{
							return window;
						}

						parentItemDefIter = GetItemFromDBID(parentItemDefIter.ParentDatabaseID);
					}
				}
			}
		}

		return null;
	}

	private void GenerateBreadcrumbForWindow(int window_index, CItemInstanceDef containerItemDef, EItemSocket SocketContainer)
	{
		CItemInstanceDef parentItemDefIter = containerItemDef;
		List<string> lstBreadcrumbs = new List<string>();

		while (parentItemDefIter != null)
		{
			//lstBreadcrumbs.Add(parentItemDefIter.GetName());
			lstBreadcrumbs.Insert(0, parentItemDefIter.GetName());

			if (parentItemDefIter.CurrentSocket == EItemSocket.None)
			{
				parentItemDefIter = GetItemFromDBID(parentItemDefIter.ParentDatabaseID);
			}
			else
			{
				break;
			}
		}

		if (containerItemDef != null)
		{
			if (containerItemDef.CurrentSocket != EItemSocket.None)
			{
				if (IsSocketAContainer(containerItemDef.CurrentSocket))
				{
					string strSocketDisplayName = GetDisplayNameForSocket(containerItemDef.CurrentSocket);
					//lstBreadcrumbs.Add(strSocketDisplayName);
					lstBreadcrumbs.Insert(0, strSocketDisplayName);
				}
			}
		}

		if (lstBreadcrumbs.Count > 1)
		{
			m_PlayerInventoryUI.UpdateBreadcrumb(window_index, lstBreadcrumbs[lstBreadcrumbs.Count - 1], "Go Back");
		}
		else if (parentItemDefIter != null)
		{
			m_PlayerInventoryUI.SetNoBreadcrumb(window_index, parentItemDefIter.GetName());
		}
		else if (containerItemDef == null && SocketContainer == EItemSocket.None)
		{
			var window = g_lstWindows[window_index];
			if (window != null)
			{
				if (window.GetItemSocketAtCreation() != EItemSocket.None)
				{
					string strSocketDisplayName = GetDisplayNameForSocket(window.GetItemSocketAtCreation());
					m_PlayerInventoryUI.SetNoBreadcrumb(window_index, strSocketDisplayName);
				}
			}
		}
	}

	public static CItemInstanceDef GetItemFromDBID(long dbid)
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();
		foreach (var item in lstCombinedInventory)
		{
			if (item.DatabaseID == dbid)
			{
				return item;
			}
		}

		return null;
	}

	public static CItemInstanceDef GetItemFromSocket(EItemSocket itemSocket)
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();
		foreach (var item in lstCombinedInventory)
		{
			if (item.CurrentSocket == itemSocket)
			{
				return item;
			}
		}

		return null;
	}

	InventoryWindow GetWindowFromItemDBID(long item_index)
	{
		foreach (InventoryWindow window in g_lstWindows)
		{
			if (window.GetItemDBID() == item_index)
			{
				return window;
			}
		}

		return null;
	}

	InventoryWindow GetWindowFromSocket(EItemSocket socket)
	{
		foreach (InventoryWindow window in g_lstWindows)
		{
			if (window.GetItemSocket() == socket)
			{
				return window;
			}
		}

		return null;
	}

	private List<InventoryWindow> g_lstWindows = new List<InventoryWindow>();
	class InventoryWindow
	{
		private InventoryWindow m_parentWindow = null;
		private long m_itemDBID = -1;
		private EItemSocket m_itemSocket = EItemSocket.None;
		private EItemSocket m_baseCreationItemSocket = EItemSocket.None;
		private int m_WindowID;
		private List<CItemInstanceDef> m_items;
		private string m_strDisplayName;
		private int m_Column = 0;

		public InventoryWindow(string strDisplayName, int a_Column, InventoryWindow a_parentWindow, long a_itemDBID, EItemSocket a_itemSocket, int a_windowID, List<CItemInstanceDef> items)
		{
			m_strDisplayName = strDisplayName;
			m_parentWindow = a_parentWindow;
			m_itemDBID = a_itemDBID;
			m_itemSocket = a_itemSocket;
			m_baseCreationItemSocket = a_itemSocket;
			m_WindowID = a_windowID;
			m_items = items;
			m_Column = a_Column;
		}

		public int GetWindowID()
		{
			return m_WindowID;
		}

		public long GetItemDBID()
		{
			return m_itemDBID;
		}

		public int GetColumn()
		{
			return m_Column;
		}

		public EItemSocket GetItemSocket()
		{
			return m_itemSocket;
		}

		public EItemSocket GetItemSocketAtCreation()
		{
			return m_baseCreationItemSocket;
		}

		public void CreateInUI(CGUIPlayerInventory playerInvUI, List<CItemInstanceDef> lstInventoryForIndexes)
		{
			playerInvUI.CreateWindow(m_strDisplayName, m_Column == 1, m_WindowID);
			ProcessItems(playerInvUI, lstInventoryForIndexes);
		}

		private void ProcessItems(CGUIPlayerInventory playerInvUI, List<CItemInstanceDef> lstInventoryForIndexes)
		{
			foreach (var item in m_items)
			{
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];
				int index = lstInventoryForIndexes.IndexOf(item);

				bool bIsContainer = itemDef.IsContainer;
				int numContainedItems = 0;
				foreach (var containedItem in lstInventoryForIndexes)
				{
					if (containedItem.ParentDatabaseID == item.DatabaseID)
					{
						++numContainedItems;
					}
				}

				bool bIsStackable = itemDef.MaxStack > 1;
				uint numStacks = item.StackSize;

				bool bIsWeapon = item.IsWeapon();
				bool bIsWeaponAttachment = item.IsWeaponAttachment();

				playerInvUI.AddItemToWindow(m_WindowID, index, (int)item.ItemID, GetImagePathForItem(item), bIsContainer, numContainedItems, itemDef.ContainerCapacity, bIsStackable, numStacks, bIsWeapon, bIsWeaponAttachment);
			}

			playerInvUI.CommitBuiltUpWindowsContents(m_WindowID);
		}

		public void RemoveFromUI(CGUIPlayerInventory playerInvUI)
		{
			playerInvUI.RemoveWindow(m_WindowID);
		}

		public bool Matches(long itemDBID, EItemSocket itemSocket)
		{
			if (itemDBID != -1 && itemDBID == m_itemDBID)
			{
				return true;
			}
			else if (itemSocket != EItemSocket.None && itemSocket == m_itemSocket)
			{
				return true;
			}

			return false;
		}

		public void Update(CGUIPlayerInventory playerInvUI, List<CItemInstanceDef> lstInventoryForIndexes)
		{
			// Does the item still exist?
			// index and socket didn't update here, it's just a refresh of the container items
			CItemInstanceDef item = null;
			if (m_itemSocket == EItemSocket.None)
			{
				item = GetItemFromDBID(m_itemDBID);
			}

			m_items = GetContainerItems(m_itemSocket != EItemSocket.None, m_itemSocket != EItemSocket.None, m_itemSocket, item);
			playerInvUI.ResetWindow(m_WindowID);
			ProcessItems(playerInvUI, lstInventoryForIndexes);
		}

		// this is used when navigating to a container inside the current window
		public void UpdateToSubItem(CGUIPlayerInventory playerInvUI, List<CItemInstanceDef> lstInventoryForIndexes, long a_itemDBID, EItemSocket a_itemSocket, List<CItemInstanceDef> items)
		{
			m_itemDBID = a_itemDBID;
			m_itemSocket = a_itemSocket;
			m_items = items;

			playerInvUI.ResetWindow(m_WindowID);
			ProcessItems(playerInvUI, lstInventoryForIndexes);
		}
	}

	private int GetNumWindowsInFirstColumn()
	{
		int num = 0;
		foreach (InventoryWindow wnd in g_lstWindows)
		{
			if (wnd.GetColumn() == 0)
			{
				++num;
			}
		}

		return num;
	}

	private int CreateWindow(string strDisplayName, long itemDBID, EItemSocket itemSocket, List<CItemInstanceDef> items, bool bClosewindowIfAlreadyExists = true)
	{
		const int maxWindowsPerColumn = 3;
		const int maxWindows = maxWindowsPerColumn * 1;

		// TODO_INVENTORY: What happens after we max out both window columns?
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		// check for existing window
		bool bHasExistingWindow = false;
		InventoryWindow matchingWnd = null;
		foreach (InventoryWindow wnd in g_lstWindows)
		{
			if (wnd.Matches(itemDBID, itemSocket))
			{
				matchingWnd = wnd;
				bHasExistingWindow = true;
				break;
			}
		}

		if (!bHasExistingWindow)
		{
			if (g_lstWindows.Count >= maxWindows)
			{
				NotificationManager.ShowNotification("Inventory", "You cannot open any more windows", ENotificationIcon.ExclamationSign);
				return -1;
			}

			int windowID = g_lstWindows.Count;
			int column = GetNumWindowsInFirstColumn() >= maxWindowsPerColumn ? 1 : 0;
			InventoryWindow newWindow = new InventoryWindow(strDisplayName, column, null, itemDBID, itemSocket, windowID, items);
			g_lstWindows.Add(newWindow);
			newWindow.CreateInUI(m_PlayerInventoryUI, lstCombinedInventory);
			return windowID;
		}


		if (bHasExistingWindow && bClosewindowIfAlreadyExists)
		{
			// Hide window
			if (matchingWnd != null)
			{
				RemoveWindow(matchingWnd);
			}
		}
		else if (bHasExistingWindow && !bClosewindowIfAlreadyExists)
		{
			return matchingWnd.GetWindowID();
		}

		return -1;
	}

	private void OnCloseWindow(int windowID)
	{
		foreach (InventoryWindow wnd in g_lstWindows)
		{
			if (wnd.GetWindowID() == windowID)
			{
				RemoveWindow(wnd);
				break;
			}
		}
	}

	private void OnGoBack(int windowID)
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		int window_index = 0;
		foreach (InventoryWindow wnd in g_lstWindows)
		{
			if (wnd.GetWindowID() == windowID)
			{
				long itemDBID = wnd.GetItemDBID();
				if (itemDBID != -1)
				{
					var baseItem = GetItemFromDBID(itemDBID);

					if (baseItem != null)
					{
						var parentItem = GetItemFromDBID(baseItem.ParentDatabaseID);
						if (parentItem != null)
						{
							EItemSocket itemSocket = parentItem.CurrentSocket;

							CItemInstanceDef containerItemDef = null;
							bool bIsSocket = itemSocket != EItemSocket.None;
							bool bIsSocketAContainer = IsSocketAContainer(itemSocket);
							if (bIsSocket && !bIsSocketAContainer) // cant expand a non-container socket unless theres an item inside, so lets find the root item
							{
								foreach (CItemInstanceDef item in lstCombinedInventory)
								{
									if (item.CurrentSocket == itemSocket)
									{
										// set item index to be the item index, since its -1 in this case
										itemDBID = item.DatabaseID;
										itemSocket = EItemSocket.None;
										containerItemDef = item;
										break;
									}
								}
							}
							else
							{
								containerItemDef = DetermineItemToUse(itemDBID, itemSocket);
							}

							List<CItemInstanceDef> itemsInsideContainer = GetContainerItems(bIsSocket, bIsSocketAContainer, itemSocket, containerItemDef);
							wnd.UpdateToSubItem(m_PlayerInventoryUI, lstCombinedInventory, lstCombinedInventory.IndexOf(parentItem), parentItem.CurrentSocket, itemsInsideContainer);
						}
						else // going back to a socket container perhaps
						{
							if (baseItem.CurrentSocket != EItemSocket.None)
							{
								bool bIsSocketAContainer = IsSocketAContainer(baseItem.CurrentSocket);

								List<CItemInstanceDef> itemsInsideContainer = GetContainerItems(true, true, baseItem.CurrentSocket, null);
								wnd.UpdateToSubItem(m_PlayerInventoryUI, lstCombinedInventory, -1, baseItem.CurrentSocket, itemsInsideContainer);
							}
						}

						// create breadcrumbs
						if (window_index != -1)
						{
							GenerateBreadcrumbForWindow(window_index, parentItem, EItemSocket.None);
						}
					}
				}
				break;
			}

			++window_index;
		}
	}

	// Shouldnt be used for comparison, used only for ui driven events like show item info
	long GetItemDBIDFromIndex(int itemIndex)
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		long dbid = -1;
		if (itemIndex >= 0 && itemIndex < lstCombinedInventory.Count)
		{
			return lstCombinedInventory[itemIndex].DatabaseID;
		}

		return dbid;
	}

	private void OnShowItemInfo(int itemIndex, EItemSocket itemSocket, float rootX, float rootY)
	{
		if (IsSocketAContainer(itemSocket))
		{
			m_PlayerInventoryUI.HideItemInfo();
			return;
		}

		CItemInstanceDef itemInstanceDef = DetermineItemToUse(GetItemDBIDFromIndex(itemIndex), itemSocket);

		if (itemInstanceDef != null)
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemInstanceDef.ItemID];

			string strValueKey = "value";
			string strValueDisplayName = itemDef.ValueString;
			string strValue = String.Empty;

			if (itemDef.ValueKey != null && itemDef.ValueKey.Length > 0)
			{
				strValueKey = itemDef.ValueKey;
			}

			JObject obj = JObject.FromObject(itemInstanceDef.Value);

			if (obj != null)
			{
				JToken jTok = obj.GetValue(strValueKey, StringComparison.OrdinalIgnoreCase);
				if (jTok != null)
				{
					strValue = jTok.ToString();
				}
			}

			if (strValue.Length == 0)
			{
				strValue = "N/A";
			}

			if (strValueDisplayName.Length == 0)
			{
				strValueDisplayName = "Value";
			}

			// Radio fix
			if (itemInstanceDef.ItemID == EItemID.RADIO && strValue == "-1")
			{
				strValue = "Turned Off";
			}
			else
			{
				if (strValue.ToLower() == "true")
				{
					strValue = "Yes";
				}
				else if (strValue.ToLower() == "false")
				{
					strValue = "No";
				}

				strValue = Helpers.FormatString("{0}: {1}", strValueDisplayName, strValue);
			}

			m_itemBeingOperatedUpon = itemInstanceDef;

			EInventoryUIActionsMode actionsMode = itemInstanceDef.CurrentSocket == EItemSocket.None ? EInventoryUIActionsMode.Full : (!IsSocketAContainerWhichOnlyAllowsRestrictedActions(itemInstanceDef.CurrentSocket) ? EInventoryUIActionsMode.Full : EInventoryUIActionsMode.MoveOnly);

			// If we are inside a vehicle/furn container, dont allow actions other than move
			var window = GetWindowFromItemDBID(itemInstanceDef.ParentDatabaseID);
			if (window != null)
			{
				if (IsSocketAContainerWhichOnlyAllowsRestrictedActions(window.GetItemSocket()) || IsSocketAContainerWhichOnlyAllowsRestrictedActions(window.GetItemSocketAtCreation()))
				{
					actionsMode = EInventoryUIActionsMode.MoveOnly;
				}
			}

			string strName = itemInstanceDef.GetName();
			string strDescription = itemInstanceDef.GetDescription();
			string UseItemTextOverride = String.Empty;

			if (WeaponHelpers.IsItemAWeaponAttachment(itemInstanceDef.ItemID))
			{
				strDescription = Helpers.FormatString("{0}<br><br><b>Fits Weapons: </b>", itemInstanceDef.GetDescription());
				int index = 0;

				if (WeaponAttachmentDefinitions.g_WeaponAttachmentIDs.ContainsKey(itemInstanceDef.ItemID))
				{
					foreach (var kvPair in WeaponAttachmentDefinitions.g_WeaponAttachmentIDs[itemInstanceDef.ItemID])
					{
						// find the item that corresponds to the hash
						foreach (var kvPairItemToWeapon in ItemWeaponDefinitions.g_DictItemIDToWeaponHash)
						{
							if (kvPairItemToWeapon.Value == kvPair.Key)
							{
								if (index > 0)
								{
									strDescription += ", ";
								}

								CInventoryItemDefinition itemDefOfWeapon = ItemDefinitions.g_ItemDefinitions[kvPairItemToWeapon.Key];

								string strColor = RAGE.Elements.Player.LocalPlayer.HasGotWeapon((uint)kvPair.Key, false) ? "green" : "red";
								strDescription += Helpers.FormatString("<font color='{0}'>{1}</font>", strColor, itemDefOfWeapon.GetNameIgnoreGenericItems());

								++index;
								break;
							}
						}
					}

				}
			}

			// handling for outfits
			if (itemDef.ItemId == EItemID.OUTFIT)
			{
				CItemValueOutfit itemValue = OwlJSON.DeserializeObject<CItemValueOutfit>(itemInstanceDef.GetValueDataSerialized(), EJsonTrackableIdentifier.ShowItemOutfit);
				int itemsInOutfit = 0;
				foreach (var kvPair in itemValue.Clothes)
				{
					if (kvPair.Value != -1)
					{
						++itemsInOutfit;
					}
				}

				foreach (var kvPair in itemValue.Props)
				{
					if (kvPair.Value != -1)
					{
						++itemsInOutfit;
					}
				}

				strName = itemValue.Name;
				strDescription = "Use the outfit to equip the associated clothing items.<br><br>Visit a clothing store, or use a wardrobe to modify this outfit.";
				strValue = Helpers.FormatString("{0} {1}", itemsInOutfit, itemsInOutfit == 1 ? "item" : "items");

				actionsMode = EInventoryUIActionsMode.UseOnly;
				UseItemTextOverride = "Change Outfit";
			}
			else if (itemDef.ItemId == EItemID.DUTY_OUTFIT)
			{
				CItemValueDutyOutfit itemValue = OwlJSON.DeserializeObject<CItemValueDutyOutfit>(itemInstanceDef.GetValueDataSerialized(), EJsonTrackableIdentifier.ShowItemDutyOutfit);
				int itemsInOutfit = 0;

				foreach (var kvPair in itemValue.Drawables)
				{
					if (kvPair.Value != -1)
					{
						++itemsInOutfit;
					}
				}

				foreach (var kvPair in itemValue.PropDrawables)
				{
					if (kvPair.Value != -1)
					{
						++itemsInOutfit;
					}
				}

				foreach (var kvPair in itemValue.Loadout)
				{
					if (kvPair.Value != -1)
					{
						++itemsInOutfit;
					}
				}

				strName = itemValue.Name;
				strDescription = "Use the outfit to equip the duty outfit.<br><br>Visit a duty point to modify this outfit.";
				strValue = Helpers.FormatString("{0} {1}", itemsInOutfit, itemsInOutfit == 1 ? "item" : "items");

				actionsMode = EInventoryUIActionsMode.DutyOutfit;
				UseItemTextOverride = "Change Outfit";
			}

			m_PlayerInventoryUI.ShowItemInfo(strName, strDescription, itemDef.Weight, strValue, actionsMode, rootX, rootY, UseItemTextOverride);
		}
		else
		{
			m_PlayerInventoryUI.HideItemInfo();
		}
	}

	public void ShowInventory()
	{
		m_PlayerInventoryUI.SetVisible(true, true, false);

		// Populate Inventory
		RefreshInventory();
	}

	enum ETabId
	{
		OnPerson = 0,
		Clothing = 1,
		Outfits = 2,
		Keys = 3,
		Furniture = 4
	}

	private void RefreshClothing()
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		int index = 0;
		foreach (CItemInstanceDef item in lstCombinedInventory)
		{
			if (item.CurrentSocket == EItemSocket.Clothing)
			{
				bool bIsBeingWorn = false;

				// get value
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];
				string strValueKey = "value";
				string strValueDisplayName = itemDef.ValueString;

				if (itemDef.ValueKey != null && itemDef.ValueKey.Length > 0)
				{
					strValueKey = itemDef.ValueKey;
				}

				JObject obj = JObject.FromObject(item.Value);

				if (obj != null)
				{
					JToken jTok = obj.GetValue(strValueKey, StringComparison.OrdinalIgnoreCase);
					if (jTok != null)
					{
						bIsBeingWorn = jTok.ToObject<bool>();
					}
				}

				m_PlayerInventoryUI.AddTabItem(index, (int)item.ItemID, GetImagePathForItem(item), "", bIsBeingWorn ? "Active" : "");
			}

			++index;
		}

		m_PlayerInventoryUI.CommitTabItems((uint)ETabId.Clothing, "clothing");
	}

	private void RefreshOutfits()
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		int index = 0;
		foreach (CItemInstanceDef item in lstCombinedInventory)
		{
			int itemsInOutfit = 0;
			string strName = "";

			if (item.CurrentSocket == EItemSocket.Outfit)
			{
				if (item.ItemID == EItemID.OUTFIT)
				{
					CItemValueOutfit itemValue = OwlJSON.DeserializeObject<CItemValueOutfit>(item.GetValueDataSerialized(), EJsonTrackableIdentifier.InventoryRefreshOutfits);
					strName = itemValue.Name;

					foreach (var kvPair in itemValue.Clothes)
					{
						if (kvPair.Value != -1)
						{
							++itemsInOutfit;
						}
					}

					foreach (var kvPair in itemValue.Props)
					{
						if (kvPair.Value != -1)
						{
							++itemsInOutfit;
						}
					}
				}
				else if (item.ItemID == EItemID.DUTY_OUTFIT)
				{
					CItemValueDutyOutfit itemValue = OwlJSON.DeserializeObject<CItemValueDutyOutfit>(item.GetValueDataSerialized(), EJsonTrackableIdentifier.InventoryRefreshDutyOutfits);
					strName = itemValue.Name;

					foreach (var kvPair in itemValue.Drawables)
					{
						if (kvPair.Value != -1)
						{
							++itemsInOutfit;
						}
					}

					foreach (var kvPair in itemValue.PropDrawables)
					{
						if (kvPair.Value != -1)
						{
							++itemsInOutfit;
						}
					}

					foreach (var kvPair in itemValue.Loadout)
					{
						if (kvPair.Value != -1)
						{
							++itemsInOutfit;
						}
					}
				}

				m_PlayerInventoryUI.AddTabItem(index, (int)item.ItemID, GetImagePathForItem(item), strName, Helpers.FormatString("{0} {1}", itemsInOutfit, itemsInOutfit == 1 ? "item" : "items"));
			}

			++index;
		}

		m_PlayerInventoryUI.CommitTabItems((uint)ETabId.Outfits, "outfits");
	}

	private void RefreshFurniture()
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		int index = 0;
		foreach (CItemInstanceDef item in lstCombinedInventory)
		{
			if (item.CurrentSocket == EItemSocket.Furniture)
			{
				string strItemName = item.GetName();
				m_PlayerInventoryUI.AddTabItem(index, (int)item.ItemID, GetImagePathForItem(item), String.Empty, strItemName.Substring(0, Math.Min(strItemName.Length, 8)));
			}

			++index;
		}

		m_PlayerInventoryUI.CommitTabItems((uint)ETabId.Furniture, "furniture");
	}

	private void RefreshKeys()
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		int index = 0;
		foreach (CItemInstanceDef item in lstCombinedInventory)
		{
			if (item.CurrentSocket == EItemSocket.Keyring)
			{
				string strPrebadgeText = "Key";
				string strPostbadgeText = "";


				// get value
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];
				string strValueKey = "value";
				string strValueDisplayName = itemDef.ValueString;

				if (itemDef.ValueKey != null && itemDef.ValueKey.Length > 0)
				{
					strValueKey = itemDef.ValueKey;
				}

				JObject obj = JObject.FromObject(item.Value);

				if (obj != null)
				{
					JToken jTok = obj.GetValue(strValueKey, StringComparison.OrdinalIgnoreCase);
					if (jTok != null)
					{
						strPostbadgeText = jTok.ToObject<int>().ToString();
					}
				}

				if (item.ItemID == EItemID.VEHICLE_KEY)
				{
					strPrebadgeText = "Vehicle";

				}
				else if (item.ItemID == EItemID.PROPERTY_KEY)
				{
					strPrebadgeText = "Property";
				}

				m_PlayerInventoryUI.AddTabItem(index, (int)item.ItemID, GetImagePathForItem(item), strPrebadgeText, strPostbadgeText);
			}

			++index;
		}

		m_PlayerInventoryUI.CommitTabItems((uint)ETabId.Keys, "keys");
	}

	private void RefreshSocketIcons()
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		// Initialize socket icons first
		foreach (EItemSocket socket in Enum.GetValues(typeof(EItemSocket)))
		{
			if (socket != EItemSocket.MAX && socket != EItemSocket.None)
			{
				if (IsSocketAContainer(socket))
				{
					if (DoesSocketHaveIcon(socket))
					{
						string strImagePath = Helpers.FormatString("package://owl_items.client/socketcontainer_{0}.png", (int)socket);
						int numItems = 0;
						foreach (CItemInstanceDef itemIter in lstCombinedInventory)
						{
							if (itemIter.CurrentSocket == socket)
							{
								++numItems;
							}
						}

						m_PlayerInventoryUI.SetItemSocketIcon(socket, strImagePath, true, false, numItems, GetSocketContainerCapacity(socket), false, 1);
					}
				}
				else
				{
					// set empty
					m_PlayerInventoryUI.SetItemSocketIcon(socket, "package://owl_items.client/empty_slot.png", false, false, 0, 0, false, 0);
				}
			}
		}

		int index = 0;
		foreach (CItemInstanceDef item in lstCombinedInventory)
		{
			// TODO_BASIC_INV: Modify this and add items
			if (item.CurrentSocket != EItemSocket.None)
			{
				if (!IsSocketAContainer(item.CurrentSocket))
				{
					CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];

					int numItems = 0;
					foreach (CItemInstanceDef itemIter in lstCombinedInventory)
					{
						if (itemIter.ParentDatabaseID == item.DatabaseID)
						{
							++numItems;
						}
					}

					m_PlayerInventoryUI.SetItemSocketIcon(item.CurrentSocket, GetImagePathForItem(item), false, itemDef.IsContainer, numItems, itemDef.ContainerCapacity, itemDef.MaxStack > 1, item.StackSize);
				}
			}

			++index;
		}
	}

	public static string GetImagePathForItem(CItemInstanceDef item)
	{
		var itemDef = ItemDefinitions.g_ItemDefinitions[item.ItemID];
		string strStateAppendage = "";
		// Is it stacked?
		if (item.StackSize > 1)
		{
			strStateAppendage = "_state_1";
		}
		else if (itemDef.IsContainer && GetContainerItems(false, false, EItemSocket.None, item).Count > 0)
		{
			strStateAppendage = "_state_2";
		}

		string strImagePath = Helpers.FormatString("package://owl_items.client/{0}{1}.png", (int)item.ItemID, strStateAppendage);
		return strImagePath;
	}

	private void OnSpawn()
	{
		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			UpdateItemSocketInternal(player, EItemSocket.Heart, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_0));
			UpdateItemSocketInternal(player, EItemSocket.Back, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_1));
			UpdateItemSocketInternal(player, EItemSocket.RearPockets, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_2));
			UpdateItemSocketInternal(player, EItemSocket.FrontPockets, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_3));
			UpdateItemSocketInternal(player, EItemSocket.LeftWaist, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_4));
			UpdateItemSocketInternal(player, EItemSocket.RightWaist, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_5));
			UpdateItemSocketInternal(player, EItemSocket.BackPants, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_6));
			UpdateItemSocketInternal(player, EItemSocket.FrontPants, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_7));
			UpdateItemSocketInternal(player, EItemSocket.LeftAnkle, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_8));
			UpdateItemSocketInternal(player, EItemSocket.RightAnkle, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_9));
			UpdateItemSocketInternal(player, EItemSocket.Chest, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_10));
			UpdateItemSocketInternal(player, EItemSocket.Head, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_11));
			UpdateItemSocketInternal(player, EItemSocket.LeftHand, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_17));
			UpdateItemSocketInternal(player, EItemSocket.RightHand, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_18));
			UpdateItemSocketInternal(player, EItemSocket.ChestSling, DataHelper.GetEntityData<int>(player, EDataNames.ITEM_SOCKET_23));
		}
	}

	public CGUIPlayerInventory GetPlayerInventoryUI()
	{
		return m_PlayerInventoryUI;
	}

	public bool IsVisible()
	{
		return m_PlayerInventoryUI.IsVisible();
	}

	private void OnPlayerDisconnected(RAGE.Elements.Player player)
	{
		if (m_dictWorldPlayerItems.ContainsKey(player))
		{
			Dictionary<EItemSocket, RAGE.Elements.MapObject> playerData = m_dictWorldPlayerItems[player];

			foreach (var kvPair in playerData)
			{
				RAGE.Elements.MapObject oldItemObject = kvPair.Value;
				if (oldItemObject != null)
				{
					// remove old item
					oldItemObject.Destroy();
				}
			}

			m_dictWorldPlayerItems.Remove(player);
		}
	}

	// TODO_INVENTORY: Hide item when its in hand if weapon
	// TODO_INVENTORY: Remove on player quit
	// TODO_INVENTORY: What about current dimension + dimension changes?
	private void UpdateItemSocketInternal(RAGE.Elements.Entity entity, EItemSocket socket, object value)
	{
		if (value == null)
		{
			return;
		}

		EItemID itemID = (EItemID)Convert.ToInt32(value);
		bool bShouldCreateObject = itemID != EItemID.None;

		if (itemID != EItemID.None)
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemID];

			if (itemDef.WorldSocketMounts.ContainsKey(socket))
			{
				ItemMount mountData = itemDef.WorldSocketMounts[socket];
				int bone = mountData.Bone;

				// If bone is zero, mounting is disabled
				if (bone == 0 || !itemDef.ShowOnPlayer)
				{
					bShouldCreateObject = false;
				}
			}
			else
			{
				bShouldCreateObject = false;
			}
		}

		if (m_dictWorldPlayerItems.ContainsKey(entity))
		{
			Dictionary<EItemSocket, RAGE.Elements.MapObject> playerData = m_dictWorldPlayerItems[entity];
			if (playerData.ContainsKey(socket))
			{
				RAGE.Elements.MapObject oldItemObject = playerData[socket];

				if (oldItemObject != null)
				{
					// remove old item
					oldItemObject.Destroy();
					playerData.Remove(socket);
				}
			}
		}
		else
		{
			// create player object
			m_dictWorldPlayerItems.Add(entity, new Dictionary<EItemSocket, RAGE.Elements.MapObject>());
		}

		if (bShouldCreateObject)
		{
			// create world object
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemID];

			ItemMount mountData = itemDef.WorldSocketMounts[socket];

			Dictionary<EItemSocket, RAGE.Elements.MapObject> playerData = m_dictWorldPlayerItems[entity];

			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			RAGE.Vector3 vecMountPos = player.GetBoneCoords(player.GetBoneIndex(mountData.Bone), mountData.X, mountData.Y, mountData.Z).CopyVector();

			AsyncModelLoader.RequestAsyncLoad(HashHelper.GetHashUnsigned(itemDef.Model), (uint modelLoaded) =>
			{
				if (playerData.ContainsKey(socket))
				{
					RAGE.Elements.MapObject oldItemObject = playerData[socket];

					if (oldItemObject != null)
					{
						// remove old item
						oldItemObject.Destroy();
						playerData.Remove(socket);
					}
				}

				playerData[socket] = new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension);
				var bone = mountData.Bone;
				var vecOffsetPos = new RAGE.Vector3(mountData.X, mountData.Y, mountData.Z);
				var vecOffsetRot = new RAGE.Vector3(mountData.RX, mountData.RY, mountData.RZ);
				RAGE.Game.Entity.AttachEntityToEntity(playerData[socket].Handle, player.Handle, player.GetBoneIndex(bone), vecOffsetPos.X, vecOffsetPos.Y, vecOffsetPos.Z, vecOffsetRot.X, vecOffsetRot.Y, vecOffsetRot.Z, true, true, true, false, 0, false);
			});
		}
	}

	private void OnTick_Infrequent()
	{
		UpdatePlayerAttachments();
	}

	private void UpdatePlayerAttachments()
	{
		// we do this on render so when skin changes etc, it reattaches automatically
		foreach (RAGE.Elements.Player player in RAGE.Elements.Entities.Players.All)
		{
			if (m_dictWorldPlayerItems.ContainsKey(player))
			{
				Dictionary<EItemSocket, RAGE.Elements.MapObject> data = m_dictWorldPlayerItems[player];
				// TODO_LAUNCH: Why ignore socket 0?
				for (EItemSocket i = EItemSocket.Heart; i < EItemSocket.MAX; ++i)
				{
					if (InventoryHelpers.IsSocketAPlayerSocket(i))
					{
						if (data.ContainsKey(i))
						{
							EItemID item_id = DataHelper.GetEntityData<EItemID>(player, (EDataNames)(((int)(EDataNames.ITEM_SOCKET_0)) + i));
							if (item_id > EItemID.None)
							{
								// Is the player reconning?
								bool bReconning = DataHelper.GetEntityData<bool>(player, EDataNames.RECON);
								if (bReconning) // destroy existing and don't allow creation if reconning
								{
									if (data[i] != null)
									{
										data[i].Destroy();
										data[i] = null;
									}
									continue;
								}

								if (ItemDefinitions.g_ItemDefinitions[item_id].ShowOnPlayer)
								{
									// Is this weapon active?
									if (ItemWeaponDefinitions.g_DictItemIDToWeaponHash.ContainsKey(item_id))
									{
										uint weaponHashForItem = (uint)ItemWeaponDefinitions.g_DictItemIDToWeaponHash[item_id];
										uint currentWeaponHash = RAGE.Elements.Player.LocalPlayer.GetSelectedWeapon();

										if (currentWeaponHash == weaponHashForItem)
										{
											// needs delete
											if (data[i] != null)
											{
												data[i].Destroy();
												data[i] = null;
											}

											continue;
										}
									}

									// TODO_LAUNCH: Skin changes
									// TODO_LAUNCH: Dimensions
									if (data[i] == null)
									{
										var obj_name = ItemDefinitions.g_ItemDefinitions[item_id].Model;
										uint hash = HashHelper.GetHashUnsigned(obj_name);
										AsyncModelLoader.RequestSyncInstantLoad(hash);

										data[i] = new RAGE.Elements.MapObject(hash, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension);
									}

									if (data[i] != null)
									{
										if (ItemDefinitions.g_ItemDefinitions[item_id].WorldSocketMounts.ContainsKey(i))
										{
											var mountData = ItemDefinitions.g_ItemDefinitions[item_id].WorldSocketMounts[i];

											var bone = mountData.Bone;

											var vecOffsetPos = new RAGE.Vector3(mountData.X, mountData.Y, mountData.Z);
											var vecOffsetRot = new RAGE.Vector3(mountData.RX, mountData.RY, mountData.RZ);
											RAGE.Game.Entity.AttachEntityToEntity(data[i].Handle, player.Handle, player.GetBoneIndex(bone), vecOffsetPos.X, vecOffsetPos.Y, vecOffsetPos.Z, vecOffsetRot.X, vecOffsetRot.Y, vecOffsetRot.Z, true, false, false, false, 0, true);
										}

										// Dimension update
										data[i].Dimension = player.Dimension;
									}
								}

							}
						}
					}
				}
			}
		}
	}

	private void ToggleInventory(EControlActionType actionType)
	{
		if (actionType == EControlActionType.Released)
		{
			ResetVehicleAndFurnitureWindowsAndSocketVisibilities();

		if (m_PlayerInventoryUI.IsVisible())
		{
			HideInventory();
		}
		else if (KeyBinds.CanProcessKeybinds()) // We can hide always, but can only show when eligible
		{
			NetworkEventSender.SendNetworkEvent_RequestPlayerInventory();
			}
		}
	}

	// TODO_SIMPLE_INV: a blocking mechanism?
	private void OnChangeCharacter()
	{
		foreach (InventoryWindow wnd in g_lstWindows)
		{
			wnd.RemoveFromUI(m_PlayerInventoryUI);
		}
		g_lstWindows.Clear();

		HideInventory();
	}

	private void OnLocalPlayerInventory(List<CItemInstanceDef> lstInvItems, EShowInventoryAction showInventoryAction)
	{
		m_lstPlayerInventory = lstInvItems;

		// Do nothing will refresh if already visible due to the below, but wont show/hide
		if (showInventoryAction == EShowInventoryAction.Show || m_PlayerInventoryUI.IsVisible())
		{
			ShowInventory();
			RefreshInventory();
		}

		if (showInventoryAction == EShowInventoryAction.HideIfVisible && m_PlayerInventoryUI.IsVisible())
		{
			HideInventory();
		}
	}

	private void RefreshInventory()
	{
		RefreshSocketIcons();

		foreach (var window in g_lstWindows)
		{
			window.Update(m_PlayerInventoryUI, GetCombinedInventory());
		}

		RefreshClothing();
		RefreshOutfits();
		RefreshFurniture();
		RefreshKeys();
	}

	public void HideInventory()
	{
		m_PlayerInventoryUI.HideItemInfo();

		m_PlayerInventoryUI.SetVisible(false, false, false);

		// Were we in a vehicle inventory?
		ItemSystem.GetVehicleInventory().CloseVehicleInventory();
		ItemSystem.GetFurnitureInventory().CloseFurnitureInventory();

		ResetVehicleAndFurnitureWindowsAndSocketVisibilities();
	}

	public void OnRequestMergeItem(Int64 ItemDBID)
	{
		if (m_itemBeingOperatedUpon != null)
		{
			CItemInstanceDef targetItemInstanceDef = GetItemFromDBID(ItemDBID);
			CInventoryItemDefinition itemDefSource = ItemDefinitions.g_ItemDefinitions[m_itemBeingOperatedUpon.ItemID];

			if (itemDefSource.MaxStack <= 1)
			{
				NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' is not able to be merged or stacked.", m_itemBeingOperatedUpon.GetName()), ENotificationIcon.ExclamationSign);
			}
			else if (targetItemInstanceDef != null)
			{
				CInventoryItemDefinition itemDefTarget = ItemDefinitions.g_ItemDefinitions[targetItemInstanceDef.ItemID];

				if (targetItemInstanceDef != m_itemBeingOperatedUpon)
				{
					// If not a container, we should try to merge, must be same and stackable
					if (itemDefSource.ItemId == itemDefTarget.ItemId)
					{
						string strCurrentContainer = "Unknown";

						if (targetItemInstanceDef.CurrentSocket == EItemSocket.None)
						{
							CItemInstanceDef parentItemInstanceDef = GetItemFromDBID(targetItemInstanceDef.ParentDatabaseID);
							if (parentItemInstanceDef != null)
							{
								strCurrentContainer = parentItemInstanceDef.GetName();
							}
						}
						else
						{
							strCurrentContainer = GetDisplayNameForSocket(targetItemInstanceDef.CurrentSocket);
						}

						if (targetItemInstanceDef.StackSize < itemDefTarget.MaxStack)
						{
							NetworkEventSender.SendNetworkEvent_MergeItem(m_itemBeingOperatedUpon.DatabaseID, ItemDBID);
						}
						else
						{
							NotificationManager.ShowNotification("Inventory", Helpers.FormatString("{0} in {1} is already at the max stack size.", targetItemInstanceDef.GetName(), strCurrentContainer), ENotificationIcon.InfoSign);
						}
					}
					else
					{
						NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' is not able to be merged with '{1}'.", m_itemBeingOperatedUpon.GetName(), targetItemInstanceDef.GetName()), ENotificationIcon.InfoSign);
					}
				}
			}

			m_itemBeingOperatedUpon = null;
		}
	}

	public void OnRequestMoveItem(bool bIsSocket, Int64 SocketOrItemDBID)
	{
		if (m_itemBeingOperatedUpon != null)
		{
			CItemInstanceDef targetItemInstanceDef = null;
			EItemSocket itemSocket = EItemSocket.None;
			CInventoryItemDefinition itemDefSource = ItemDefinitions.g_ItemDefinitions[m_itemBeingOperatedUpon.ItemID];

			// is it an item?
			if (!bIsSocket)
			{
				targetItemInstanceDef = GetItemFromDBID(SocketOrItemDBID);
			}
			else
			{
				itemSocket = (EItemSocket)SocketOrItemDBID;
				if (!IsSocketAContainer(itemSocket)) // only if not a container socket, since those should store the item 
				{
					// Do we have an item in the socket? USe that instead and pretend we just dropped on to that item instead
					targetItemInstanceDef = GetItemFromSocket(itemSocket);
				}
			}

			// We have an item!
			if (targetItemInstanceDef != null)
			{
				CInventoryItemDefinition itemDefTarget = ItemDefinitions.g_ItemDefinitions[targetItemInstanceDef.ItemID];

				WeaponAttachmentDefinition attachmentDef = null;
				EWeaponAttachmentType attachmentType = EWeaponAttachmentType.None;
				if (targetItemInstanceDef.IsWeapon())
				{
					attachmentDef = WeaponAttachmentDefinitions.GetWeaponAttachmentDefinitionByID(m_itemBeingOperatedUpon.ItemID);
					if (attachmentDef != null)
					{
						attachmentType = attachmentDef.AttachmentType;
					}
				}

				// can this item actually go into the other item?
				if (itemDefTarget.IsContainer)
				{
					if (!DoesContainerHaveSpaceForItem(targetItemInstanceDef, EItemSocket.None))
					{
						NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' does not have space in it for '{1}'.", targetItemInstanceDef.GetName(), m_itemBeingOperatedUpon.GetName()), ENotificationIcon.InfoSign);
					}
					else if (targetItemInstanceDef.IsWeapon() && !DoesWeaponAcceptAttachment(targetItemInstanceDef, m_itemBeingOperatedUpon))
					{
						NotificationManager.ShowNotification("Inventory", Helpers.FormatString("b '{0}' cannot be attached to '{1}'.", m_itemBeingOperatedUpon.GetName(), targetItemInstanceDef.GetName()), ENotificationIcon.InfoSign);
					}
					else if (targetItemInstanceDef.IsWeapon() && !DoesWeaponHaveSpaceForAttachment(targetItemInstanceDef, attachmentType))
					{
						NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' already has an attachment of type '{1}'.", targetItemInstanceDef.GetName(), attachmentType), ENotificationIcon.InfoSign);
					}
					else if (itemDefTarget.ContainerCanAcceptItem(itemDefSource.ItemId))
					{
						NetworkEventSender.SendNetworkEvent_SetItemInContainer(m_itemBeingOperatedUpon.DatabaseID, targetItemInstanceDef.DatabaseID, false, ItemSystem.GetVehicleInventory().GetCurrentVehicle(), ItemSystem.GetFurnitureInventory().GetCurrentFurnitureItemDBID());
					}
					else
					{
						if (targetItemInstanceDef.IsWeapon())
						{
							NotificationManager.ShowNotification("Inventory", Helpers.FormatString("a '{0}' cannot be attached to '{1}'.", m_itemBeingOperatedUpon.GetName(), targetItemInstanceDef.GetName()), ENotificationIcon.InfoSign);
						}
						else
						{
							NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' cannot be put inside '{1}'.", m_itemBeingOperatedUpon.GetName(), targetItemInstanceDef.GetName()), ENotificationIcon.InfoSign);
						}
					}
				}
			}
			else if (itemSocket != EItemSocket.None)
			{
				// does the source item fit in the socket?
				bool bSocketAcceptsSourceItem = false;
				foreach (EItemSocket iterSocket in itemDefSource.Sockets)
				{
					if (iterSocket == itemSocket)
					{
						bSocketAcceptsSourceItem = true;
					}
				}

				string strSocketDisplayName = GetDisplayNameForSocket(itemSocket);
				if (!DoesContainerHaveSpaceForItem(null, itemSocket))
				{
					NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' does not have space in it for '{1}'.", strSocketDisplayName, m_itemBeingOperatedUpon.GetName()), ENotificationIcon.InfoSign);
				}
				else if (bSocketAcceptsSourceItem)
				{
					NetworkEventSender.SendNetworkEvent_SetItemInSocket(m_itemBeingOperatedUpon.DatabaseID, itemSocket, ItemSystem.GetVehicleInventory().GetCurrentVehicle(), ItemSystem.GetFurnitureInventory().GetCurrentFurnitureItemDBID());
				}
				else
				{
					NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' is not able to be put into the '{1}' socket.", m_itemBeingOperatedUpon.GetName(), strSocketDisplayName), ENotificationIcon.InfoSign);
				}
			}
		}

		m_itemBeingOperatedUpon = null;
	}

	public void OnUseItem()
	{
		if (m_itemBeingOperatedUpon != null)
		{
			NetworkEventSender.SendNetworkEvent_OnUseItem(m_itemBeingOperatedUpon.DatabaseID);
		}
	}

	public void OnMergeItem()
	{
		if (m_itemBeingOperatedUpon != null)
		{
			// can the root item be merged?
			CInventoryItemDefinition itemDefSource = ItemDefinitions.g_ItemDefinitions[m_itemBeingOperatedUpon.ItemID];

			if (itemDefSource.MaxStack <= 1)
			{
				NotificationManager.ShowNotification("Inventory", Helpers.FormatString("'{0}' is not able to be merged or stacked.", m_itemBeingOperatedUpon.GetName()), ENotificationIcon.ExclamationSign);
			}
			else
			{
				int numAdded = 0;

				// find all potential merge targets
				List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

				foreach (var targetItem in lstCombinedInventory)
				{
					if (targetItem != m_itemBeingOperatedUpon)
					{
						CInventoryItemDefinition itemDefTarget = ItemDefinitions.g_ItemDefinitions[targetItem.ItemID];

						// If not a container, we should try to merge, must be same and stackable
						if (itemDefSource.ItemId == itemDefTarget.ItemId)
						{
							string strCurrentContainer = "";

							if (targetItem.CurrentSocket == EItemSocket.None)
							{
								CItemInstanceDef parentItemInstanceDef = GetItemFromDBID(targetItem.ParentDatabaseID);
								if (parentItemInstanceDef != null)
								{
									strCurrentContainer = parentItemInstanceDef.GetName();
								}
							}
							else
							{
								if (InventoryHelpers.CanSocketBeIncludedInMerge(targetItem.CurrentSocket))
								{
									strCurrentContainer = GetDisplayNameForSocket(targetItem.CurrentSocket);
								}
							}


							if (strCurrentContainer.Length > 0)
							{
								if (targetItem.StackSize < itemDefTarget.MaxStack)
								{
									m_PlayerInventoryUI.AddMergeTarget(Helpers.FormatString("{0} in {1} (Current Stack: {2})", targetItem.GetName(), strCurrentContainer, targetItem.StackSize), true, targetItem.DatabaseID);
									++numAdded;
								}
								else
								{
									m_PlayerInventoryUI.AddMergeTarget(Helpers.FormatString("{0} in {1}: Already at max stack size ({2})", targetItem.GetName(), strCurrentContainer, itemDefTarget.MaxStack), false, targetItem.DatabaseID);
									++numAdded;
								}
							}
						}
					}
				}

				if (numAdded > 0)
				{
					m_PlayerInventoryUI.ShowMergeItem(m_itemBeingOperatedUpon.GetName());
				}
				else
				{
					NotificationManager.ShowNotification("Inventory", "You have nothing to combine this item with.", ENotificationIcon.InfoSign);
				}
			}
		}
	}

	public void OnMoveItem()
	{
		if (m_itemBeingOperatedUpon != null)
		{
			// find all potential targets
			CInventoryItemDefinition itemDefSource = ItemDefinitions.g_ItemDefinitions[m_itemBeingOperatedUpon.ItemID];

			List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

			// TODO: SOCKETS

			// How many items does it have?
			int numAdded = 0;
			foreach (var targetItem in lstCombinedInventory)
			{
				if (targetItem != m_itemBeingOperatedUpon)
				{
					CInventoryItemDefinition targetItemDef = ItemDefinitions.g_ItemDefinitions[targetItem.ItemID];
					if (targetItemDef != null)
					{
						if (targetItemDef.IsContainer)
						{
							if (targetItemDef.ContainerCanAcceptItem(itemDefSource.ItemId))
							{
								// Do we have space?
								if (!DoesContainerHaveSpaceForItem(targetItem, EItemSocket.None))
								{
									m_PlayerInventoryUI.AddMoveTarget(Helpers.FormatString("{0} (No Space in Container)", targetItem.GetName()), false, false, targetItem.DatabaseID);
									++numAdded;
								}
								else if (targetItem.IsWeapon() && DoesWeaponAcceptAttachment(targetItem, m_itemBeingOperatedUpon))
								{
									m_PlayerInventoryUI.AddMoveTarget(Helpers.FormatString("Attach To Weapon: {0}", targetItem.GetName()), true, false, targetItem.DatabaseID);
									++numAdded;
								}
								else
								{
									m_PlayerInventoryUI.AddMoveTarget(Helpers.FormatString("{0}", targetItem.GetName()), true, false, targetItem.DatabaseID);
									++numAdded;
								}
							}
						}
					}
				}
			}

			// sockets
			foreach (EItemSocket iterSocket in Enum.GetValues(typeof(EItemSocket)))
			{
				if (iterSocket != m_itemBeingOperatedUpon.CurrentSocket)
				{
					// does the source item fit in the socket?
					bool bSocketAcceptsSourceItem = false;
					foreach (EItemSocket innerLoopSockets in itemDefSource.Sockets)
					{
						if (innerLoopSockets == iterSocket)
						{
							if (IsSocketAVehicleContainer(innerLoopSockets))
							{
								bSocketAcceptsSourceItem = (ItemSystem.GetVehicleInventory().GetCurrentVehicle() != null);
							}
							else if (IsSocketAFurnitureContainer(innerLoopSockets))
							{
								bSocketAcceptsSourceItem = (ItemSystem.GetFurnitureInventory().GetCurrentFurnitureItemDefinition() != null);
							}
							else if (IsSocketAPropertyContainer(innerLoopSockets))
							{
								bSocketAcceptsSourceItem = (ItemSystem.GetPropertyInventory().GetCurrentPropertyID() != -1);
							}
							else
							{
								bSocketAcceptsSourceItem = true;
							}
						}
					}

					string strSocketDisplayName = GetDisplayNameForSocket(iterSocket);
					if (bSocketAcceptsSourceItem && !DoesContainerHaveSpaceForItem(null, iterSocket))
					{
						m_PlayerInventoryUI.AddMoveTarget(Helpers.FormatString("{0} (No Space in Socket Container)", strSocketDisplayName), false, true, (int)iterSocket);
						++numAdded;
					}
					else if (bSocketAcceptsSourceItem)
					{
						string strSocketPrefix = "Socket";
						if (IsSocketAVehicleContainer(iterSocket))
						{
							strSocketPrefix = "Vehicle";
						}
						else if (IsSocketAFurnitureContainer(iterSocket))
						{
							strSocketPrefix = "Furniture";
						}
						else if (IsSocketAPropertyContainer(iterSocket))
						{
							strSocketPrefix = "Property";
						}

						// Only show socket if its empty or a container, otherwise, its either full, or it has a container item (e.g. holster) inside that will appear via the above logic
						if (!IsSocketAContainer(iterSocket))
						{
							var targetItemInstanceDef = GetItemFromSocket(iterSocket);
							if (targetItemInstanceDef == null)
							{
								m_PlayerInventoryUI.AddMoveTarget(Helpers.FormatString("{0}: {1}", strSocketPrefix, strSocketDisplayName), true, true, (int)iterSocket);
								++numAdded;
							}
							else
							{
								m_PlayerInventoryUI.AddMoveTarget(Helpers.FormatString("{0}: {1} (Socket occupied)", strSocketPrefix, strSocketDisplayName), false, true, (int)iterSocket);
								++numAdded;
							}
						}
						else
						{
							bool bCanAccessSeats = true;
							if (iterSocket == EItemSocket.Vehicle_Seats)
							{
								if (ItemSystem.GetVehicleInventory().GetCurrentVehicle() != null)
								{
									Vehicle vehicle = ItemSystem.GetVehicleInventory().GetCurrentVehicle();
									if (vehicle.GetNumberOfDoors() <= 4) // 2 door vehicles do not have space to be put on the ground
									{
										bCanAccessSeats = false;
									}
								}
							}

							if (bCanAccessSeats)
							{
								m_PlayerInventoryUI.AddMoveTarget(Helpers.FormatString("{0}: {1}", strSocketPrefix, strSocketDisplayName), true, true, (int)iterSocket);
								++numAdded;
							}

						}

					}
				}
			}

			if (numAdded > 0)
			{
				m_PlayerInventoryUI.ShowMoveItem(m_itemBeingOperatedUpon.GetName());
			}
			else
			{
				NotificationManager.ShowNotification("Inventory", "You have no where to move this item to.", ENotificationIcon.InfoSign);
			}
		}
	}

	public void OnShowItem()
	{
		if (m_itemBeingOperatedUpon != null)
		{
			NetworkEventSender.SendNetworkEvent_OnShowItem(m_itemBeingOperatedUpon.DatabaseID);
		}
	}

	private void RemoveWindow(InventoryWindow wnd)
	{
		wnd.RemoveFromUI(m_PlayerInventoryUI);
		g_lstWindows.Remove(wnd);
	}

	private void DestroyWindowsBelongingToContainer(CItemInstanceDef containerItemDef)
	{
		long dbid = containerItemDef.DatabaseID;
		if (dbid != -1)
		{
			List<InventoryWindow> lstWindowsToDestroy = new List<InventoryWindow>();
			foreach (InventoryWindow wnd in g_lstWindows)
			{
				if (wnd.Matches(dbid, EItemSocket.None))
				{
					lstWindowsToDestroy.Add(wnd);
				}
			}

			foreach (InventoryWindow wndToDestroy in lstWindowsToDestroy)
			{
				RemoveWindow(wndToDestroy);
			}
		}
	}

	public void OnDropItem()
	{
		if (m_itemBeingOperatedUpon != null)
		{
			// Close windows for this slot
			m_PlayerInventoryUI.HideItemInfo();
			DestroyWindowsBelongingToContainer(m_itemBeingOperatedUpon);

			// Calculate the drop position (has to be clientside for the ground z)
			RAGE.Vector3 vecDropPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
			float fRotZ = RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f;

			const float fDist = 0.9f;
			float fRadians = fRotZ * (3.14f / 180.0f);
			vecDropPos.X += (float)Math.Cos(fRadians) * fDist;
			vecDropPos.Y += (float)Math.Sin(fRadians) * fDist;
			vecDropPos.Z = WorldHelper.GetGroundPosition(vecDropPos) + 0.05f;

			NetworkEventSender.SendNetworkEvent_OnDropItem(m_itemBeingOperatedUpon.DatabaseID, vecDropPos.X, vecDropPos.Y, vecDropPos.Z);
		}
	}

	public void OnCopyToClipboardItemValue()
	{
		NotificationManager.ShowNotification("Copied!", "Copied item value to clipboard.", ENotificationIcon.InfoSign);
	}

	public void OnSplitItem(Int64 current_item_dbid, uint current_item_new_numstacks, uint new_item_num_stacks)
	{
		NetworkEventSender.SendNetworkEvent_SplitItem(current_item_dbid, current_item_new_numstacks, new_item_num_stacks);
	}

	CItemInstanceDef g_itemBeingSplit = null;
	private bool DoesWeaponAcceptAttachment(CItemInstanceDef instTargetWeapon, CItemInstanceDef instSourceAttachment)
	{
		if (!ItemWeaponDefinitions.g_DictItemIDToWeaponHash.ContainsKey(instTargetWeapon.ItemID))
		{
			return false;
		}

		WeaponAttachmentDefinition attachmentDef = WeaponAttachmentDefinitions.GetWeaponAttachmentDefinitionByID(instSourceAttachment.ItemID);
		if (attachmentDef != null)
		{
			WeaponHash weaponHash = ItemWeaponDefinitions.g_DictItemIDToWeaponHash[instTargetWeapon.ItemID];
			return attachmentDef.DoesAttachmentBelongOnWeapon(weaponHash, instSourceAttachment.ItemID);
		}

		return false;
	}

	private bool DoesWeaponHaveSpaceForAttachment(CItemInstanceDef instTarget, EWeaponAttachmentType attachmentType)
	{
		// We check we don't have another contained item of the same unique attachment type (e.g. muzzle)
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();

		CInventoryItemDefinition itemDefTarget = ItemDefinitions.g_ItemDefinitions[instTarget.ItemID];
		// How many items does it have?
		foreach (var item in lstCombinedInventory)
		{
			if (item.ParentDatabaseID == instTarget.DatabaseID)
			{
				// Is it the same type?
				WeaponAttachmentDefinition attachmentDef = WeaponAttachmentDefinitions.GetWeaponAttachmentDefinitionByID(item.ItemID);
				if (attachmentDef != null)
				{
					if (attachmentDef.AttachmentType == attachmentType)
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	private bool DoesContainerHaveSpaceForItem(CItemInstanceDef instTarget, EItemSocket itemSocket)
	{
		List<CItemInstanceDef> lstCombinedInventory = GetCombinedInventory();
		// TODO_SIMPLE_INV: This function doesnt consider items inside child containers, do we want to restrict that? possibly exploitable if not?
		int itemsInsideTarget = 0;
		int ContainerCapacity = 0;

		if (itemSocket == EItemSocket.None)
		{
			CInventoryItemDefinition itemDefTarget = ItemDefinitions.g_ItemDefinitions[instTarget.ItemID];
			ContainerCapacity = itemDefTarget.ContainerCapacity;

			// How many items does it have?
			foreach (var item in lstCombinedInventory)
			{
				if (item.ParentDatabaseID == instTarget.DatabaseID)
				{
					++itemsInsideTarget;
				}
			}
		}
		else
		{
			// TODO_SIMPLE_INV: Respect weights for everything + have support for dynamic socket capacities
			ContainerCapacity = GetSocketContainerCapacity(itemSocket);

			if (ContainerCapacity == -1)
			{
				return true;
			}
			else
			{
				foreach (var item in lstCombinedInventory)
				{
					if (item.CurrentSocket == itemSocket)
					{
						++itemsInsideTarget;
					}
				}
			}
		}


		return itemsInsideTarget < ContainerCapacity;
	}

	private int GetSocketContainerCapacity(EItemSocket itemSocket)
	{
		if (InventoryHelpers.IsSocketAVehicleSocket(itemSocket))
		{
			Vehicle currentVehicle = ItemSystem.GetVehicleInventory().GetCurrentVehicle();
			if (currentVehicle != null)
			{
				var vehicleClass = currentVehicle.GetClass();
				return VehicleInventoryConstants.g_iVehicleInventorySizes[vehicleClass];
			}

			return 0;
		}

		if (itemSocket == EItemSocket.PlacedFurnitureStorage)
		{
			var furnitureDef = ItemSystem.GetFurnitureInventory().GetCurrentFurnitureItemDefinition();
			if (furnitureDef != null)
			{
				return furnitureDef.StorageCapacity;
			}

			return 0;
		}

		return InventoryHelpers.IsUnlimitedContainerSocket(itemSocket) ? -1 : 5;
	}

	public void OnDutyOutfitShare()
	{
		if (m_itemBeingOperatedUpon != null)
		{
			HideInventory();

			Dictionary<string, string> dictDropdownItems = new Dictionary<string, string>();

			foreach (RAGE.Elements.Player player in RAGE.Elements.Entities.Players.All)
			{
				//if (player != RAGE.Elements.Player.LocalPlayer)
				{
					bool bIsLoggedIn = DataHelper.GetEntityData<bool>(player, EDataNames.IS_LOGGED_IN);
					bool bIsSpawned = DataHelper.GetEntityData<bool>(player, EDataNames.IS_SPAWNED);

					if (bIsLoggedIn && bIsSpawned)
					{
						dictDropdownItems.Add(player.Name, player.Name);
					}
				}
			}

			if (dictDropdownItems.Count == 0)
			{
				NotificationManager.ShowNotification("Share Duty Outfit", "No other players are online", ENotificationIcon.ExclamationSign);
			}
			else
			{
				m_DutyOutfitBeingShared = m_itemBeingOperatedUpon;

				GenericDropdown.ShowGenericDropdown("Select Outfit", dictDropdownItems, "Share Duty Outfit", "Please select a player you wish to share with", "Share", "Cancel",
				UIEventID.ShareDutyOutfit_SelectPlayer_Done, UIEventID.ShareDutyOutfit_SelectPlayer_Cancel, UIEventID.ShareDutyOutfit_SelectPlayer_DropdownSelectionChanged, EPromptPosition.Center);
			}
		}
	}

	private void OnDutyOutfitEditor_SelectPreset_Done(string strDisplayName, string strValue)
	{
		if (m_DutyOutfitBeingShared != null)
		{
			RAGE.Elements.Player targetPlayer = null;
			foreach (RAGE.Elements.Player player in RAGE.Elements.Entities.Players.All)
			{
				if (player.Name.ToLower() == strValue.ToLower())
				{
					targetPlayer = player;
					break;
				}
			}

			if (targetPlayer != null)
			{
				NetworkEventSender.SendNetworkEvent_ShareDutyOutfit(m_DutyOutfitBeingShared.DatabaseID, targetPlayer);
				m_DutyOutfitBeingShared = null;
			}
			else
			{
				NotificationManager.ShowNotification("Share Duty Outfit", "Player was not found.", ENotificationIcon.ExclamationSign);
			}
		}
	}

	private void OnDutyOutfitEditor_SelectPreset_Cancel()
	{
		ShowInventory();
	}

	public void OnDestroyItem()
	{
		if (m_itemBeingOperatedUpon != null)
		{
			// Close windows for this slot
			m_PlayerInventoryUI.HideItemInfo();
			DestroyWindowsBelongingToContainer(m_itemBeingOperatedUpon);

			string strItemName = m_itemBeingOperatedUpon.GetName();

			HideInventory();
			GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("Are you sure you want to destroy '{0}'? Any items contained inside will also be destroyed.", strItemName), "Yes, Destroy it", "No, Keep it", UIEventID.OnDestroyItem_Confirm, UIEventID.OnDestroyItem_Cancel);
		}
	}

	private void OnDestroyItem_Confirm()
	{
		NetworkEventSender.SendNetworkEvent_OnDestroyItem(m_itemBeingOperatedUpon.DatabaseID);
		// TODO_BASIC_INV: reset m_itemBeingOperatedUpon
	}

	private void OnDestroyItem_Cancel()
	{
		// TODO_BASIC_INV: reset m_itemBeingOperatedUpon
		ShowInventory();
	}
}

