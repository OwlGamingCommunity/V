using System;

public class ItemMoverUI
{
	public ItemMoverUI()
	{
		RageEvents.RAGE_OnTick_HighFrequency += OnTick;
	}

	private void OnTick()
	{
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		bool bCanMoveObjects = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.CAN_MOVE_OBJECTS) || DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ADMIN_DUTY);
		if (bIsSpawned)
		{
			if (CursorManager.IsCursorVisible())
			{
				if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.Sprint) && RAGE.Game.Pad.IsDisabledControlJustPressed(0, (int)RAGE.Game.Control.Attack))
				{
					// Did we click a player?
					RAGE.Vector3 vecClickedWorldPos = GraphicsHelper.GetWorldPositionFromScreenPosition(CursorManager.GetCursorPosition());
					if (vecClickedWorldPos != null)
					{
						RAGE.Vector3 vecGameplayCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);
						CRaycastResult raycast = WorldHelper.RaycastFromTo(vecGameplayCamPos, vecClickedWorldPos, RAGE.Elements.Player.LocalPlayer.Handle, -1);

						if (raycast.Hit && raycast.EntityHit != null)
						{
							if (raycast.EntityHit.Type == RAGE.Elements.Type.Object)
							{
								RAGE.Elements.MapObject objHit = (RAGE.Elements.MapObject)raycast.EntityHit;
								EItemID itemID = DataHelper.GetEntityData<EItemID>(objHit, EDataNames.ITEM_ID);

								string genericData = DataHelper.GetEntityData<string>(objHit, EDataNames.ITEM_CUSTOM_VALUE);
								CItemValueGenericItem genericItem = OwlJSON.DeserializeObject<CItemValueGenericItem>(genericData, EJsonTrackableIdentifier.ItemMoverUIClick);
								if (itemID == EItemID.GENERIC_ITEM && genericItem != null)
								{
									// Are they near enough?
									float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, raycast.EntityHit.Position);
									Int64 charID = DataHelper.GetLocalPlayerEntityData<Int64>(EDataNames.CHARACTER_ID);

									if (fDist <= 3.0 && (bCanMoveObjects || genericItem.PlacedBy.Equals(charID)))
									{
										ShowItemMover(objHit);
									}
								}
							}
						}
					}
				}
			}
		}
	}
	private void ShowItemMover(RAGE.Elements.MapObject clickedItem)
	{
		m_itemMoverUI.Initialize(clickedItem);
		m_itemMoverUI.SetVisible(true, false, false);
	}

	private CGUIItemMover m_itemMoverUI = new CGUIItemMover(() => { });
}