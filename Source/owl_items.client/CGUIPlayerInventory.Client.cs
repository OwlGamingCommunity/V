using System;

public class CGUIPlayerInventory : CEFCore
{
	public CGUIPlayerInventory(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/player_inventory.html", EGUIID.PlayerInventory, callbackOnLoad)
	{
		UIEvents.RequestMoveItem += (bool bIsSocket, Int64 SocketOrItemDBID) => { ItemSystem.GetPlayerInventory()?.OnRequestMoveItem(bIsSocket, SocketOrItemDBID); };
		UIEvents.RequestMergeItem += (Int64 ItemDBID) => { ItemSystem.GetPlayerInventory()?.OnRequestMergeItem(ItemDBID); };

		UIEvents.OnMoveItem += () => { ItemSystem.GetPlayerInventory()?.OnMoveItem(); };
		UIEvents.OnMergeItem += () => { ItemSystem.GetPlayerInventory()?.OnMergeItem(); };
		UIEvents.OnUseItem += () => { ItemSystem.GetPlayerInventory()?.OnUseItem(); };
		UIEvents.OnShowItem += () => { ItemSystem.GetPlayerInventory()?.OnShowItem(); };
		UIEvents.OnDropItem += () => { ItemSystem.GetPlayerInventory()?.OnDropItem(); };
		UIEvents.OnDestroyItem += () => { ItemSystem.GetPlayerInventory()?.OnDestroyItem(); };
		UIEvents.OnDutyOutfitShare += () => { ItemSystem.GetPlayerInventory()?.OnDutyOutfitShare(); };
	}

	public override void OnLoad()
	{

	}

	public void CreateWindow(string strWindowName, bool isSecondPage, int windowID)
	{
		Execute("CreateWindow", strWindowName, isSecondPage, windowID);
	}

	public void ResetWindow(int WindowID)
	{
		Execute("ResetWindow", WindowID);
	}

	public void CommitBuiltUpWindowsContents(int WindowID)
	{
		Execute("CommitBuiltUpWindowsContents", WindowID);
	}

	public void AddItemToWindow(int WindowID, int itemIndex, int itemID, string strImagePath, bool bIsContainer, int numContainedItems, int nmaxContainerItems, bool bIsStackable, uint numStacks, bool bIsWeapon, bool bIsWeaponAttachment)
	{
		Execute("AddItemToWindow", WindowID, itemIndex, itemID, strImagePath, bIsContainer, numContainedItems, nmaxContainerItems, bIsStackable, numStacks, bIsWeapon, bIsWeaponAttachment);
	}

	public void CommitTabItems(uint TabID, string strName)
	{
		Execute("CommitTabItems", TabID, strName);
	}

	public void AddTabItem(int itemIndex, int itemID, string strImagePath, string strPreBadgeText, string strPostBadgeText)
	{
		Execute("AddTabItem", itemIndex, itemID, strImagePath, strPreBadgeText, strPostBadgeText);
	}

	public void RemoveWindow(int WindowID)
	{
		Execute("RemoveWindow", WindowID);
	}

	public void ShowItemInfo(string item_name, string item_desc, float item_weight, string item_value_string, EInventoryUIActionsMode actionsMode, float rootX, float rootY, string UseItemTextOverride)
	{
		Execute("ShowItemInfo", item_name, item_desc, item_weight, item_value_string, (int)actionsMode, rootX, rootY, UseItemTextOverride);
	}

	/*
	public void AddBreadcrumb(string displayName, int window_index)
	{
		Execute("AddBreadcrumb", displayName, window_index);
	}

	public void CommitBreadcrumbs(int window_index)
	{
		Execute("CommitBreadcrumbs", window_index);
	}
	*/

	public void OverrideWindowContents(int window_index, string overrideText)
	{
		Execute("OverrideWindowContents", window_index, overrideText);
	}

	public void UpdateBreadcrumb(int window_index, string displayName, string strGoBackName)
	{
		Execute("UpdateBreadcrumb", window_index, displayName, strGoBackName);
	}

	public void SetNoBreadcrumb(int window_index, string displayName)
	{
		Execute("SetNoBreadcrumb", window_index, displayName);
	}

	public void HideItemInfo()
	{
		Execute("HideItemInfo");
	}

	public void ShowSplitItem(string name, UInt32 max)
	{
		Execute("ShowSplitItem", name, max);
	}

	public void SetItemSocketIcon(EItemSocket socket_id, string image_path, bool is_socket_container, bool is_container, int num_items, int max_items, bool is_stackable, uint num_stacks)
	{
		bool bIsSmallType = InventoryHelpers.IsSocketAVehicleSocket(socket_id) || socket_id == EItemSocket.Clothing || socket_id == EItemSocket.Furniture || socket_id == EItemSocket.Keyring || socket_id == EItemSocket.Outfit;
		Execute("SetItemSocketIcon", (int)socket_id, image_path, is_socket_container, is_container, num_items, max_items, is_stackable, num_stacks, bIsSmallType);
	}

	public void ShowMoveItem(string strDisplayName)
	{
		Execute("ShowMoveItem", strDisplayName);
	}

	public void AddMoveTarget(string strDisplayName, bool bCanMove, bool bIsSocket, Int64 SocketOrItemDBID)
	{
		Execute("AddMoveTarget", strDisplayName, bCanMove, bIsSocket, SocketOrItemDBID);
	}

	public void ShowMergeItem(string strDisplayName)
	{
		Execute("ShowMergeItem", strDisplayName);
	}

	public void AddMergeTarget(string strDisplayName, bool bCanMove, Int64 ItemDBID)
	{
		Execute("AddMergeTarget", strDisplayName, bCanMove, ItemDBID);
	}
}