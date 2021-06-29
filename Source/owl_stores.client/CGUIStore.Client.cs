internal class CGUIStore : CEFCore
{
	public CGUIStore(OnGUILoadedDelegate callbackOnLoad) : base("owl_stores.client/store.html", EGUIID.Store, callbackOnLoad)
	{
		UIEvents.HideStore += () => { StoreSystem_Core.GetStoreSystem()?.HideStoreUI(); };
		UIEvents.StoreCheckout += (string strJsonData, int numItems) => { StoreSystem_Core.GetStoreSystem()?.OnStoreCheckout(strJsonData, numItems); };
	}

	public override void OnLoad()
	{

	}

	public void SetButtonsEnabled(bool bEnabled)
	{
		Execute("SetButtonsEnabled", bEnabled);
	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void SetTaxRate(float fTaxRate)
	{
		Execute("SetTaxRate", fTaxRate);
	}

	public void AddItem(EItemID itemID, string strItemDescriptionDisplay)
	{
		// TODO_GENERICS: Needs support for generics, othewrise all frisk items are 'furniture', also the concept of registering doesn't work anymore
		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemID];

		// TODO_GENERICS: Must support generics for furniture prices
		Execute("AddStoreItem", itemDef.GetNameIgnoreGenericItems(), itemDef.GetCostIgnoreGenericItems(), (int)itemID, strItemDescriptionDisplay);
	}

	public void ShowErrorMessage(string strMessage)
	{
		Execute("ShowErrorMessage", strMessage);
	}
}