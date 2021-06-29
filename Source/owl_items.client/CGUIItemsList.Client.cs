using System.Collections.Generic;

internal class CGUIItemsList : CEFCore
{
	public CGUIItemsList(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/itemlist.html", EGUIID.ItemsList, callbackOnLoad)
	{
		UIEvents.CloseItemsListUI += () => { ItemSystem.GetItemsListUI().OnCloseItemsListUI(); };
	}

	public override void OnLoad()
	{

	}

	public void Initialize(List<CItemDetails> itemsList)
	{
		Execute("loadItemData", OwlJSON.SerializeObject(itemsList, EJsonTrackableIdentifier.ItemsListInit));
	}
}