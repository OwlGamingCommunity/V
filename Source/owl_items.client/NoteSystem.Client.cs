using RAGE;

public class NoteSystem
{
	private bool m_bAdminDuty = false;

	public NoteSystem()
	{
		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_HighFrequency += OnTick;

	}

	private void OnRender()
	{
		if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.CharacterWheel))
		{
			NoteTextDrawing();
		}
	}

	private void OnTick()
	{
		if (m_bAdminDuty)
		{
			UpdateRightClick();
		}
	}

	private void NoteTextDrawing()
	{
		foreach (var note in OptimizationCachePool.StreamedInObjects())
		{
			EItemID itemID = DataHelper.GetEntityData<EItemID>(note, EDataNames.ITEM_ID);
			if (itemID != EItemID.NOTE)
			{
				continue;
			}

			if (WorldHelper.GetDistance2D(RAGE.Elements.Player.LocalPlayer.Position, note.Position) >= 20f)
			{
				continue;
			}

			Vector3 position = note.Position.Add(new Vector3(0, 0, 1f));
			Vector2 vecScreenPos = GraphicsHelper.GetScreenPositionFromWorldPosition(position);

			bool bAdminDuty = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ADMIN_DUTY);
			m_bAdminDuty = bAdminDuty;

			Vector2 vecScreenPosAdminText = new Vector2(vecScreenPos.X, vecScreenPos.Y - 0.025f);

			string noteString = DataHelper.GetEntityData<string>(note, EDataNames.NOTE_MESSAGE);
			string strOwner = DataHelper.GetEntityData<string>(note, EDataNames.NOTE_OWNER_NAME);
			bool bAdminLocked = DataHelper.GetEntityData<bool>(note, EDataNames.NOTE_LOCKED);

			string noteAdminText = Helpers.FormatString("Owner: {0}. Locked: {1}", strOwner, bAdminLocked);

			if (bAdminDuty)
			{
				TextHelper.Draw2D(noteAdminText, vecScreenPosAdminText.X, vecScreenPosAdminText.Y, 0.5f, 0, 255, 0, 255, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			}

			TextHelper.Draw2D(noteString, vecScreenPos.X, vecScreenPos.Y, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);
		}
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

								bool isNote = false;
								EItemID itemID = DataHelper.GetEntityData<EItemID>(objHit, EDataNames.ITEM_ID);
								if (itemID != EItemID.NOTE)
								{
									isNote = false;
								}
								else
								{
									isNote = true;
								}

								if (isNote)
								{
									// Are they near enough?
									float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, raycast.EntityHit.Position);

									if (fDist <= 3.0)
									{
										NetworkEventSender.SendNetworkEvent_AdminToggleNoteLock(objHit);
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