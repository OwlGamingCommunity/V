using System.Collections.Generic;

public class ItemsListUI
{
	public ItemsListUI()
	{
		NetworkEvents.ShowItemsList += ShowItemsList;
	}

	private void ShowItemsList()
	{
		List<CItemDetails> items = new List<CItemDetails>();

		foreach (CInventoryItemDefinition invItem in ItemDefinitions.g_ItemDefinitions.Values)
		{
			items.Add(new CItemDetails(invItem.ItemId, invItem.GetNameIgnoreGenericItems(), invItem.GetDescIgnoreGenericItems(), invItem.Weight));
		}
		m_itemsListUI.Initialize(items);

		m_itemsListUI.SetVisible(true, true, false);
	}

	public void OnCloseItemsListUI()
	{
		m_itemsListUI.SetVisible(false, false, false);
	}

	private CGUIItemsList m_itemsListUI = new CGUIItemsList(() => { });
}