internal class CGUIEditInterior : CEFCore
{
	public CGUIEditInterior(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/editinterior.html", EGUIID.EditInterior, callbackOnLoad)
	{
		UIEvents.EditInterior_OnChangeClass += (string strClassName) => { ItemSystem.GetFurnitureSystem()?.OnChangeClass(strClassName); };
		UIEvents.EditInterior_OnChangeFurnitureItem += (int index) => { ItemSystem.GetFurnitureSystem()?.OnChangeFurnitureItem(index); };
		UIEvents.EditInterior_OnChangeRemovedFurnitureItem += (int index) => { ItemSystem.GetFurnitureSystem()?.OnChangeRemovedFurnitureItem(index); };
		UIEvents.EditInterior_OnChangeCurrentFurnitureItem += (int index) => { ItemSystem.GetFurnitureSystem()?.OnChangeCurrentFurnitureItem(index); };

		UIEvents.EditInterior_PlaceFurniture += () => { ItemSystem.GetFurnitureSystem()?.OnPlaceFurniture(); };
		UIEvents.EditInterior_PickupFurniture += () => { ItemSystem.GetFurnitureSystem()?.OnPickupFurniture(); };
		UIEvents.EditInterior_RestoreFurniture += () => { ItemSystem.GetFurnitureSystem()?.OnRestoreFurniture(); };

		UIEvents.EditInterior_Hide += () => { ItemSystem.GetFurnitureSystem()?.OnExit(); };
	}

	public override void OnLoad()
	{

	}

	public void SetFurnitureCapacity(int capacity)
	{
		Execute("SetFurnitureCapacity", capacity);
	}

	public void SetOutfitChange(bool bAllowed)
	{
		Execute("SetOutfitChange", bAllowed);
	}

	public void SetButtonsEnabled(bool bEnabled)
	{
		Execute("SetButtonsEnabled", bEnabled);
	}

	public void AddFurnitureItem(int index, CItemInstanceDef a_Item)
	{
		Execute("AddFurnitureItem", index, a_Item.GetName());
	}

	public void CommitFurnitureItems()
	{
		Execute("CommitFurnitureItems");
	}

	public void AddCurrentFurnitureItem(int index, string strDisplayName)
	{
		Execute("AddCurrentFurnitureItem", index, strDisplayName);
	}

	public void CommitCurrentFurnitureItems()
	{
		Execute("CommitCurrentFurnitureItems");
	}

	public void AddRemovedFurnitureItem(int index, string strDisplayName)
	{
		Execute("AddRemovedFurnitureItem", index, strDisplayName);
	}

	public void CommitRemovedFurnitureItems()
	{
		Execute("CommitRemovedFurnitureItems");
	}
}