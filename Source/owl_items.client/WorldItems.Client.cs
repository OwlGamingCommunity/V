public class WorldItems
{
	const float g_fDistThreshold = 3.0f;

	public WorldItems()
	{
		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
	}

	public RAGE.Elements.MapObject GetNearestWorldItem()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.WorldItem);
		return poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.MapObject>() : null;
	}

	private void OnRender()
	{
		RAGE.Elements.MapObject nearestWorldItem = GetNearestWorldItem();
		if (nearestWorldItem != null)
		{
			EItemID itemID = DataHelper.GetEntityData<EItemID>(nearestWorldItem, EDataNames.ITEM_ID);
			bool bIsLocked = DataHelper.GetEntityData<bool>(nearestWorldItem, EDataNames.ITEM_LOCKED);

			// TODO_GENERICS: How can we get item instance def here to get the generic name?
			string strItemName = ItemDefinitions.g_ItemDefinitions[itemID].GetNameIgnoreGenericItems();

			if (itemID == EItemID.GENERIC_ITEM)
			{
				string genericData = DataHelper.GetEntityData<string>(nearestWorldItem, EDataNames.ITEM_CUSTOM_VALUE);
				CItemValueGenericItem genericItem = OwlJSON.DeserializeObject<CItemValueGenericItem>(genericData, EJsonTrackableIdentifier.RenderGenericWorldItemName);
				if (!string.IsNullOrEmpty(genericItem.name))
				{
					strItemName = genericItem.name;
				}
			}

			if (!bIsLocked)
			{
				WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), Helpers.FormatString("Pickup \"{0}\"", strItemName), null, PickupItem, nearestWorldItem.Position, nearestWorldItem.Dimension, false, false, g_fDistThreshold);
			}
		}
	}

	private void PickupItem()
	{
		RAGE.Elements.MapObject nearestWorldItem = GetNearestWorldItem();

		if (nearestWorldItem != null)
		{
			NetworkEventSender.SendNetworkEvent_OnPickupItem(nearestWorldItem);
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Object)
		{
			RAGE.Elements.MapObject mapObject = (RAGE.Elements.MapObject)entity;
			EItemID itemID = DataHelper.GetEntityData<EItemID>(mapObject, EDataNames.ITEM_ID);
			if (itemID == EItemID.GENERIC_ITEM)
			{
				RAGE.Vector3 rotation = DataHelper.GetEntityData<RAGE.Vector3>(mapObject, EDataNames.GENERIC_ROTATION);

				if (rotation != null)
				{
					mapObject.SetRotation(rotation.X, rotation.Y, rotation.Z, 0, false);
				}
			}
		}
	}
}