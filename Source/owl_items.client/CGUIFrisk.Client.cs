internal class CGUIFrisk : CEFCore
{
	public CGUIFrisk(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/frisk.html", EGUIID.Frisk, callbackOnLoad)
	{
		UIEvents.OnFriskTakeItem += (int index) => { ItemSystem.GetFrisk()?.OnFriskTakeItem(index); };
		UIEvents.OnHideFrisk += () => { ItemSystem.GetFrisk()?.OnHideFrisk(); };
	}

	public override void OnLoad()
	{

	}

	public void Initialize(int maxSlots, float fMaxWeight)
	{
		Execute("Initialize", maxSlots, fMaxWeight);
	}

	public void RegisterItem(int index, CInventoryItemDefinition itemDef)
	{
		// TODO_GENERICS: Needs support for generics, othewrise all frisk items are 'furniture', same goes for desc etc, also the concept of registering doesn't work anymore
		Execute("RegisterItem", index, itemDef.GetNameIgnoreGenericItems(), itemDef.ValueString, itemDef.GetDescIgnoreGenericItems(), itemDef.Weight);
	}

	public void SetCharacterName(string strCharacterName)
	{
		Execute("SetCharacterName", strCharacterName);
	}

	public void AddItem(int index, EItemID itemID, string strValue)
	{
		Execute("AddItem", index, itemID, strValue);
	}
}