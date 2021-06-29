using RAGE;
using System;

public class RoadblockSystem
{
	private CGUIRoadblockEditor m_RoadblockEditorUI = new CGUIRoadblockEditor(() => { });

	private bool m_bMovingX = false;
	private bool m_bMovingY = false;
	private bool m_bMovingZ = true;
	private bool m_bRotationX = false;
	private bool m_bRotationY = false;
	private bool m_bRotationZ = true;
	private bool m_bRotating = false;

	public RoadblockSystem()
	{
		NetworkEvents.GotoRoadblockEditor += OnShow;

		UIEvents.RoadblockEditor_PlaceRoadblock += OnPlaceRoadblock;

		RageEvents.RAGE_OnRender += OnRender;

		KeyBinds.Bind(ConsoleKey.X, () =>
		{
			if (IsInEditMode())
			{
				if (m_bRotating)
				{
					m_bRotationX = true;
					m_bRotationY = false;
					m_bRotationZ = false;
				}
				else
				{
					m_bMovingX = true;
					m_bMovingY = false;
					m_bMovingZ = false;
				}
			}
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);

		KeyBinds.Bind(ConsoleKey.Y, () =>
		{
			if (IsInEditMode())
			{
				if (m_bRotating)
				{
					m_bRotationX = false;
					m_bRotationY = true;
					m_bRotationZ = false;
				}
				else
				{
					m_bMovingX = false;
					m_bMovingY = true;
					m_bMovingZ = false;
				}
			}
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);

		KeyBinds.Bind(ConsoleKey.Z, () =>
		{
			if (IsInEditMode())
			{
				if (m_bRotating)
				{
					m_bRotationX = false;
					m_bRotationY = false;
					m_bRotationZ = true;
				}
				else
				{
					m_bMovingX = false;
					m_bMovingY = false;
					m_bMovingZ = true;
				}
			}
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);

		KeyBinds.Bind(ConsoleKey.R, () =>
		{
			if (IsInEditMode())
			{
				m_bRotating = !m_bRotating;
			}
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
	}

	private void OnShow()
	{
		int index = 0;
		foreach (RoadblockDefinition def in Roadblocks.Definitions)
		{
			m_RoadblockEditorUI.AddRoadblockType(index, def.DisplayName);
			++index;
		}
		m_RoadblockEditorUI.CommitRoadblockTypes();

		RAGE.Game.Ui.DisplayRadar(false);
		HUD.SetVisible(false, false, false);
		m_RoadblockEditorUI.SetVisible(true, false, true);
	}

	public void OnExit()
	{
		RAGE.Game.Ui.DisplayRadar(true);
		HUD.SetVisible(true, false, false);
		m_RoadblockEditorUI.SetVisible(false, false, true);
	}

	private bool IsInEditMode()
	{
		return m_RoadblockEditorUI.IsVisible();
	}

	private void OnRender()
	{
		UpdateMouse();

		if (IsInEditMode())
		{
			// R Key :(
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack1);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack2);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackAlternate);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackHeavy);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackLight);

			const float fFontScale = 0.4f;
			// controls
			TextHelper.Draw2D("Controls:", 0.01f, 0.75f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("Right click: Select existing roadblock", 0.03f, 0.8f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("Right click + Delete key: Remove existing roadblock", 0.03f, 0.85f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("X, Y, Z: Switch axis in positioning/rotation mode", 0.03f, 0.9f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("R: Switch between Rotation & Position mode", 0.03f, 0.95f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);

			// current status
			if (m_bRotating)
			{
				if (m_bRotationX)
				{
					TextHelper.Draw2D("Edit Mode: Rotating on X Axis", 0.01f, 0.7f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
				}
				else if (m_bRotationY)
				{
					TextHelper.Draw2D("Edit Mode: Rotating on Y Axis", 0.01f, 0.7f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
				}
				else if (m_bRotationZ)
				{
					TextHelper.Draw2D("Edit Mode: Rotating on Z Axis", 0.01f, 0.7f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
				}
			}
			else
			{
				if (m_bMovingX)
				{
					TextHelper.Draw2D("Edit Mode: Moving on X Axis", 0.01f, 0.7f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
				}
				else if (m_bMovingY)
				{
					TextHelper.Draw2D("Edit Mode: Moving on Y Axis", 0.01f, 0.7f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
				}
				else if (m_bMovingZ)
				{
					TextHelper.Draw2D("Edit Mode: Moving on Z Axis", 0.01f, 0.7f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
				}
			}
		}
	}

	private void OnPlaceRoadblock(int index)
	{
		if (index >= 0)
		{
			Vector3 vecPos = RAGE.Elements.Player.LocalPlayer.Position;
			float fDist = 2.0f;
			float radians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f) * (3.14f / 180.0f);
			vecPos.X += (float)Math.Cos(radians) * fDist;
			vecPos.Y += (float)Math.Sin(radians) * fDist;
			vecPos.Z = WorldHelper.GetGroundPosition(vecPos);

			NetworkEventSender.SendNetworkEvent_Roadblock_PlaceNew(index, vecPos.X, vecPos.Y, vecPos.Z);
		}
	}

	private RAGE.Elements.MapObject m_ItemBeingMoved = null;

	private RAGE.Vector3 vecClickedLast = null;
	private RAGE.Vector3 vecStartDrag = null;
	private Vector2 vecLastCursorPos = null;
	private void UpdateMouse()
	{
		if (!IsInEditMode())
		{
			return;
		}

		float fBoxWidth = 0.4f;
		if (m_ItemBeingMoved != null)
		{
			RAGE.Game.Graphics.DrawBox(m_ItemBeingMoved.Position.X, m_ItemBeingMoved.Position.Y, m_ItemBeingMoved.Position.Z, m_ItemBeingMoved.Position.X + fBoxWidth, m_ItemBeingMoved.Position.Y + fBoxWidth, m_ItemBeingMoved.Position.Z + fBoxWidth, 0, 255, 0, 250);
		}

		if (vecClickedLast != null)
		{
			RAGE.Game.Graphics.DrawBox(vecClickedLast.X, vecClickedLast.Y, vecClickedLast.Z, vecClickedLast.X + fBoxWidth, vecClickedLast.Y + fBoxWidth, vecClickedLast.Z + fBoxWidth, 255, 0, 0, 250);
		}

		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		if (bIsSpawned)
		{
			if (CursorManager.IsCursorVisible())
			{
				if (m_ItemBeingMoved != null)
				{
					if (RAGE.Game.Pad.IsDisabledControlJustReleased(0, 214))
					{
						EObjectTypes objType = DataHelper.GetEntityData<EObjectTypes>(m_ItemBeingMoved, EDataNames.OBJECT_TYPE);
						if (objType == EObjectTypes.ROADBLOCK)
						{
							int entryID = DataHelper.GetEntityData<int>(m_ItemBeingMoved, EDataNames.ITEM_ID);
							NetworkEventSender.SendNetworkEvent_Roadblock_RemoveExisting(entryID);
							m_ItemBeingMoved = null;
						}
					}
					else if (RAGE.Game.Pad.IsDisabledControlJustReleased(0, 25))
					{
						if (m_ItemBeingMoved != null)
						{
							int entryID = DataHelper.GetEntityData<int>(m_ItemBeingMoved, EDataNames.ITEM_ID);
							NetworkEventSender.SendNetworkEvent_Roadblock_UpdateExisting(entryID, m_ItemBeingMoved.Position.X, m_ItemBeingMoved.Position.Y, m_ItemBeingMoved.Position.Z, m_ItemBeingMoved.GetRotation(0).X, m_ItemBeingMoved.GetRotation(0).Y, m_ItemBeingMoved.GetRotation(0).Z);
							m_ItemBeingMoved = null;
						}
					}
					else
					{
						RAGE.Vector3 vecClickedWorldPos = GraphicsHelper.GetWorldPositionFromScreenPosition(CursorManager.GetCursorPosition());

						if (vecClickedWorldPos != null)
						{
							RAGE.Vector3 vecStorePos = null;
							if (m_bRotating)
							{
								const float fOffset = 1.0f;

								if (m_bRotationX)
								{
									Vector3 vecRot = m_ItemBeingMoved.GetRotation(0);
									m_ItemBeingMoved.SetRotation(vecRot.X + fOffset, vecRot.Y, vecRot.Z, 0, false);
								}
								else if (m_bRotationY)
								{
									Vector3 vecRot = m_ItemBeingMoved.GetRotation(0);
									m_ItemBeingMoved.SetRotation(vecRot.X, vecRot.Y + fOffset, vecRot.Z, 0, false);
								}
								else if (m_bRotationZ)
								{
									Vector3 vecRot = m_ItemBeingMoved.GetRotation(0);
									m_ItemBeingMoved.SetRotation(vecRot.X, vecRot.Y, vecRot.Z + fOffset, 0, false);
								}
							}
							else
							{
								if (m_bMovingX)
								{
									float fDist = vecClickedWorldPos.X - vecStartDrag.X;
									m_ItemBeingMoved.Position = new Vector3(m_ItemBeingMoved.Position.X + fDist, m_ItemBeingMoved.Position.Y, m_ItemBeingMoved.Position.Z);
									vecStorePos = vecClickedWorldPos;
								}
								else if (m_bMovingY)
								{
									float fDist = vecClickedWorldPos.Y - vecStartDrag.Y;
									m_ItemBeingMoved.Position = new Vector3(m_ItemBeingMoved.Position.X, m_ItemBeingMoved.Position.Y + fDist, m_ItemBeingMoved.Position.Z);
									vecStorePos = vecClickedWorldPos;
								}
								else if (m_bMovingZ)
								{
									float fDist2D = vecLastCursorPos.Y - CursorManager.GetCursorPosition().Y;

									const float fOffsetPer2DUnit = 0.005f;
									float f3DScaledDist = fOffsetPer2DUnit * fDist2D;

									// limit to ground
									Vector3 vecNewPos = new Vector3(m_ItemBeingMoved.Position.X, m_ItemBeingMoved.Position.Y, m_ItemBeingMoved.Position.Z + f3DScaledDist);
									float fGround = WorldHelper.GetGroundPosition(vecNewPos, false, 0.0f);
									if (vecNewPos.Z < fGround)
									{
										vecNewPos.Z = fGround;
									}

									m_ItemBeingMoved.Position = vecNewPos;
									vecStorePos = m_ItemBeingMoved.Position; ;
									vecLastCursorPos = CursorManager.GetCursorPosition();
								}

								vecStartDrag = vecStorePos;
							}
						}
					}
				}
				else if (RAGE.Game.Pad.IsDisabledControlJustPressed(0, 25))
				{
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

								EObjectTypes objType = DataHelper.GetEntityData<EObjectTypes>(objHit, EDataNames.OBJECT_TYPE);
								if (objType == EObjectTypes.ROADBLOCK)
								{
									m_ItemBeingMoved = objHit;
									vecStartDrag = vecClickedWorldPos;
									vecLastCursorPos = CursorManager.GetCursorPosition();
								}
							}
						}
					}
				}
			}
		}
	}
}