internal class CGUIFurnitureStore : CEFCore
{
	public CGUIFurnitureStore(OnGUILoadedDelegate callbackOnLoad) : base("owl_stores.client/furniturestore.html", EGUIID.FurnitureStore, callbackOnLoad)
	{
		UIEvents.FurnitureStore_StartRotation += (EFurnitureStoreRotationDirection direction) => { StoreSystem_Core.GetFurnitureStore()?.OnStartRotation(direction); };
		UIEvents.FurnitureStore_StopRotation += () => { StoreSystem_Core.GetFurnitureStore()?.OnStopRotation(); };
		UIEvents.FurnitureStore_ResetCamera += () => { StoreSystem_Core.GetFurnitureStore()?.OnResetCamera(); };

		UIEvents.FurnitureStore_StartZoom += (EFurnitureStoreZoomDirection direction) => { StoreSystem_Core.GetFurnitureStore()?.OnStartZoom(direction); };
		UIEvents.FurnitureStore_StopZoom += () => { StoreSystem_Core.GetFurnitureStore()?.OnStopZoom(); };

		UIEvents.FurnitureStore_OnChangeClass += (string strClassName) => { StoreSystem_Core.GetFurnitureStore()?.OnChangeClass(strClassName); };
		UIEvents.FurnitureStore_OnChangeFurnitureItem += (int index) => { StoreSystem_Core.GetFurnitureStore()?.OnChangeFurnitureItem((uint)index); };
		UIEvents.FurnitureStore_OnCheckout += () => { StoreSystem_Core.GetFurnitureStore()?.OnCheckout(); };

		UIEvents.FurnitureStore_Hide += () => { StoreSystem_Core.GetFurnitureStore()?.OnExit(); };
	}

	public override void OnLoad()
	{

	}

	public void AddFurnitureItem(uint index, string strDisplayName)
	{
		Execute("AddFurnitureItem", index, strDisplayName);
	}

	public void CommitFurnitureItems()
	{
		Execute("CommitFurnitureItems");
	}

	public void SetPriceInfo(float fPrice, float fTax, float fPriceWithTax, int storageCapacity, bool bAllowsOutfitChange)
	{
		Execute("SetPriceInfo", fPrice, fTax, fPriceWithTax, storageCapacity, bAllowsOutfitChange);
	}

	public void ShowErrorMessage(string strMessage)
	{
		Execute("ShowErrorMessage", strMessage);
	}

	public void SetButtonsEnabled(bool bEnabled)
	{
		Execute("SetButtonsEnabled", bEnabled);
	}
}