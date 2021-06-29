public class GenericSystem
{
	private CGUIGenericCreation m_GenericCreationUI = new CGUIGenericCreation(OnUILoaded);
	public GenericSystem()
	{
		NetworkEvents.Generics_ShowGenericUI += OnShowGenericUI;
		RageEvents.RAGE_OnTick_HighFrequency += OnTick;

	}

	private void OnTick()
	{
		if (DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ADMIN_DUTY))
		{
			UpdateRightClick();
		}
	}

	private static void OnUILoaded() { }

	private void OnShowGenericUI()
	{
		if (m_GenericCreationUI.IsVisible())
		{
			m_GenericCreationUI.SetVisible(false, false, false);
		}
		else
		{
			m_GenericCreationUI.SetVisible(true, true, false);
		}
	}

	public void OnCloseGenericsUI()
	{
		m_GenericCreationUI.Reset();
		m_GenericCreationUI.SetVisible(false, false, false);
	}

	private void UpdateRightClick()
	{
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		if (bIsSpawned)
		{
			if (CursorManager.IsCursorVisible())
			{
				if (RAGE.Game.Pad.IsDisabledControlJustPressed(0, 25))
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
								if (itemID == EItemID.GENERIC_ITEM)
								{
									// Are they near enough?
									float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, raycast.EntityHit.Position);

									if (fDist <= 3.0)
									{
										NetworkEventSender.SendNetworkEvent_AdminToggleItemLock(objHit);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}

