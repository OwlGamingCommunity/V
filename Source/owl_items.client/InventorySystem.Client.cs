public class InventorySystem
{
	public InventorySystem()
	{
		CInventoryItemDefinition[] jsonData = OwlJSON.DeserializeObject<CInventoryItemDefinition[]>(CItemData.ItemData, EJsonTrackableIdentifier.ItemData);

		foreach (CInventoryItemDefinition itemDef in jsonData)
		{
			ItemDefinitions.g_ItemDefinitions.Add(itemDef.ItemId, itemDef);
		}
	}
}