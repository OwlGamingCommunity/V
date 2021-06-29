using RAGE;
using System;
using System.Collections.Generic;

public class FurnitureSystem
{
	private CGUIEditInterior m_EditInteriorUI = new CGUIEditInterior(() => { });

	private List<CItemInstanceDef> m_lstFurnitureItemsAvailable = null;
	private int m_currentSelectedPlaceFurnitureIndex = -1;
	private int m_currentSelectedCurrentFurnitureIndex = -1;
	private int m_currentSelectedDefaultFurnitureRemovalIndex = -1;

	private string m_strCurrentClass = String.Empty;

	private bool m_bMovingX = false;
	private bool m_bMovingY = false;
	private bool m_bMovingZ = true;
	private bool m_bRotationX = false;
	private bool m_bRotationY = false;
	private bool m_bRotationZ = true;
	private bool m_bRotating = false;

	private DateTime AlertCreatedTime = DateTime.Now;
	private string m_strAlert = null;

	public FurnitureSystem()
	{
		CFurnitureDefinition[] jsonData = OwlJSON.DeserializeObject<CFurnitureDefinition[]>(CFurnitureData.FurnitureData, EJsonTrackableIdentifier.FurnitureData);

		foreach (CFurnitureDefinition furnitureDef in jsonData)
		{
			FurnitureDefinitions.g_FurnitureDefinitions.Add(furnitureDef.ID, furnitureDef);
		}

		NetworkEvents.GotoPropertyEditMode += OnGotoPropertyEditMode;

		NetworkEvents.UpdateFurnitureCache += OnUpdateFurnitureCache;
		NetworkEvents.LocalPlayerDimensionChanged += OnLocalPlayerDimensionChanged;

		NetworkEvents.ChangeCharacterApproved += OnExit;

		RageEvents.RAGE_OnRender += OnRender;

		KeyBinds.Bind(ConsoleKey.B, () =>
		{
			if (IsInEditMode() && !m_bRotating)
			{
				if (!m_bMovingX || !m_bMovingY || m_bMovingZ)
				{
					m_bMovingX = true;
					m_bMovingY = true;
					m_bMovingZ = true;
				}
				HandleXYZAlert();
			}
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);

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
				HandleXYZAlert();
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
				HandleXYZAlert();
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
				HandleXYZAlert();
			}
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);

		KeyBinds.Bind(ConsoleKey.R, () =>
		{
			if (IsInEditMode())
			{
				m_bRotating = !m_bRotating;

				ShowAlert(m_bRotating ? "Switched to 'Rotation' mode" : "Switched to 'Position' mode");
			}
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
	}

	private void HandleXYZAlert()
	{
		if (!m_bRotating && m_bMovingX && m_bMovingY && m_bMovingZ)
		{
			ShowAlert("Switched to 'Snap to XYZ' mode");
		}
		else
		{
			if (m_bRotating)
			{
				if (m_bRotationX)
				{
					ShowAlert("Rotating on the X axis");
				}
				else if (m_bRotationY)
				{
					ShowAlert("Rotating on the Y axis");
				}
				else if (m_bRotationZ)
				{
					ShowAlert("Rotating on the Z axis");
				}
			}
			else
			{
				if (m_bMovingX)
				{
					ShowAlert("Positioning on the X axis");
				}
				else if (m_bMovingY)
				{
					ShowAlert("Positioning on the Y axis");
				}
				else if (m_bMovingZ)
				{
					ShowAlert("Positioning on the Z axis");
				}
			}
		}
	}

	private bool IsInEditMode()
	{
		return m_EditInteriorUI.IsVisible();
	}

	public bool IsObjectBeingMoved()
	{
		return m_ItemBeingMoved != null;
	}

	private void ShowAlert(string strMessage)
	{
		// reset existing timer otherwise we'll only show for the remainder
		AlertCreatedTime = DateTime.Now;
		m_strAlert = strMessage;
	}

	private static Dictionary<long, List<CPropertyFurnitureInstance>> m_dictFurnitures = new Dictionary<long, List<CPropertyFurnitureInstance>>();
	private static Dictionary<long, List<CPropertyDefaultFurnitureRemovalInstance>> m_dictRemovals = new Dictionary<long, List<CPropertyDefaultFurnitureRemovalInstance>>();

	public static CPropertyFurnitureInstance GetNearestStreamedFurnitureItemWithAction()
	{
		CPropertyFurnitureInstance nearestFurnitureInstance = null;
		uint propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;

		if (m_dictFurnitures.ContainsKey(propertyID))
		{
			// TODO_FURNITURE: Move to optimization cache pool, might lag in large interiors
			float fSmallestDist = 999999.0f;

			foreach (CPropertyFurnitureInstance furn in m_dictFurnitures[propertyID])
			{
				uint furnitureID = furn.FurnitureID;

				if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
				{
					CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[furnitureID];
					if (furnitureDef.StorageCapacity > 0 || furnitureDef.AllowOutfitChange)
					{
						float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, furn.vecPos);
						if (fDist <= fSmallestDist)
						{
							fSmallestDist = fDist;
							nearestFurnitureInstance = furn;
						}
					}
				}
			}
		}

		return nearestFurnitureInstance;
	}

	public static CPropertyFurnitureInstance GetFurnitureItemFromID(uint FurnitureDBID)
	{
		uint propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;

		if (m_dictFurnitures.ContainsKey(propertyID))
		{
			foreach (CPropertyFurnitureInstance furn in m_dictFurnitures[propertyID])
			{
				if (furn.DBID == FurnitureDBID)
				{
					return furn;
				}
			}
		}

		return null;
	}

	public static CPropertyFurnitureInstance GetNearestStreamedFurnitureItemWithActivity(out EActivityType activityType)
	{
		activityType = EActivityType.None;
		CPropertyFurnitureInstance nearestFurnitureInstance = null;
		uint propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;

		if (m_dictFurnitures.ContainsKey(propertyID))
		{
			// TODO_FURNITURE: Move to optimization cache pool, might lag in large interiors
			float fSmallestDist = 999999.0f;

			foreach (CPropertyFurnitureInstance furn in m_dictFurnitures[propertyID])
			{
				uint furnitureID = furn.FurnitureID;

				if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
				{
					CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[furnitureID];
					if (furnitureDef.Category == EFurnitureCategory.Activities)
					{
						float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, furn.vecPos);
						if (fDist <= fSmallestDist)
						{
							fSmallestDist = fDist;
							nearestFurnitureInstance = furn;
							activityType = furnitureDef.Activity;
						}
					}
				}
			}
		}

		return nearestFurnitureInstance;
	}

	private void OnUpdateFurnitureCache(long propID, List<CPropertyFurnitureInstance> lstFurniture, List<CPropertyDefaultFurnitureRemovalInstance> lstRemovals)
	{
		// remove everything now and we'll re-stream it in
		UnloadAllStreamedOutFurnitureAndRemovals();

		m_dictFurnitures[propID] = lstFurniture;
		m_dictRemovals[propID] = lstRemovals;

		// Is the edit UI visible? repopulate the current list
		if (m_EditInteriorUI.IsVisible())
		{
			EditInterior_PopulateCurrentFurnitureList(propID);
			EditInterior_PopulateDefaultFurnitureRemovalList(propID);
			OnChangeClass(m_strCurrentClass);
		}

		// Current interior? update now
		if (propID == RAGE.Elements.Player.LocalPlayer.Dimension)
		{
			LoadObjectsAndRemovalsInProperty(propID);
		}
	}

	private void EditInterior_PopulateCurrentFurnitureList(long propertyID)
	{
		if (m_dictFurnitures.ContainsKey(propertyID))
		{
			int index = 0;
			foreach (CPropertyFurnitureInstance furn in m_dictFurnitures[propertyID])
			{
				uint furnitureID = furn.FurnitureID;

				if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
				{
					CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[furnitureID];
					m_EditInteriorUI.AddCurrentFurnitureItem(index, Helpers.FormatString("[#{0}] {1} ({2})", index + 1, furnitureDef.Name, furnitureDef.Category));
				}

				++index;
			}
		}

		m_EditInteriorUI.CommitCurrentFurnitureItems();
	}

	private void EditInterior_PopulateDefaultFurnitureRemovalList(long propertyID)
	{
		if (m_dictRemovals.ContainsKey(propertyID))
		{
			int index = 0;
			foreach (CPropertyDefaultFurnitureRemovalInstance removal in m_dictRemovals[propertyID])
			{
				m_EditInteriorUI.AddRemovedFurnitureItem(index, Helpers.FormatString("[#{0}] {1} (near {2}, {3})", index + 1, removal.Model, removal.vecPos.X, removal.vecPos.Y));

				++index;
			}
		}

		m_EditInteriorUI.CommitRemovedFurnitureItems();
	}

	private void UnloadAllStreamedOutFurnitureAndRemovals()
	{
		foreach (var propData in m_dictFurnitures)
		{
			foreach (var furniture in propData.Value)
			{
				furniture.Destroy();
			}
		}

		foreach (var removalData in m_dictRemovals)
		{
			foreach (var removal in removalData.Value)
			{
				removal.Restore();
			}
		}
	}

	private void LoadObjectsAndRemovalsInProperty(long propID)
	{
		if (m_dictFurnitures.ContainsKey(propID))
		{
			foreach (var furniture in m_dictFurnitures[propID])
			{
				furniture.Create();
			}
		}

		if (m_dictRemovals.ContainsKey(propID))
		{
			foreach (var removal in m_dictRemovals[propID])
			{
				removal.Apply();
			}
		}
	}

	private void OnLocalPlayerDimensionChanged(uint oldDimension, uint newDimension)
	{
		// exit edit UI if they were in it and got dim changed
		OnExit();

		// unload old objects
		UnloadAllStreamedOutFurnitureAndRemovals();

		// load new objects
		LoadObjectsAndRemovalsInProperty(newDimension);
	}

	public void OnChangeClass(string strClass)
	{
		m_strCurrentClass = strClass;
		m_currentSelectedPlaceFurnitureIndex = -1;

		bool bIncludeAllFurnitureItems = strClass.ToLower() == "allstorage";
		bool bIncludeAllOutfitChangeItems = strClass.ToLower() == "alloutfitchange";

		int index = 0;
		foreach (CItemInstanceDef item in m_lstFurnitureItemsAvailable)
		{
			if (item.IsFurniture())
			{
				uint furnitureID = item.GetFurnitureID();

				if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
				{
					CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[furnitureID];

					if (bIncludeAllFurnitureItems)
					{
						if (furnitureDef.StorageCapacity > 0)
						{
							m_EditInteriorUI.AddFurnitureItem(index, item);
						}
					}
					else if (bIncludeAllOutfitChangeItems)
					{
						if (furnitureDef.AllowOutfitChange)
						{
							m_EditInteriorUI.AddFurnitureItem(index, item);
						}
					}
					else
					{
						if (furnitureDef.Category.ToString().ToLower() == strClass.ToLower())
						{
							m_EditInteriorUI.AddFurnitureItem(index, item);
						}
					}
				}
			}

			// indexes always increase, so we can just access the array directly by index, UI doesn't care.
			++index;
		}
		m_EditInteriorUI.CommitFurnitureItems();
		m_EditInteriorUI.SetFurnitureCapacity(0);
		m_EditInteriorUI.SetOutfitChange(false);
	}

	public void OnChangeFurnitureItem(int index)
	{
		m_currentSelectedPlaceFurnitureIndex = index;


		var item = m_lstFurnitureItemsAvailable[m_currentSelectedPlaceFurnitureIndex];
		uint furnitureID = item.GetFurnitureID();

		if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnitureID))
		{
			CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[furnitureID];
			m_EditInteriorUI.SetFurnitureCapacity(furnitureDef.StorageCapacity);
			m_EditInteriorUI.SetOutfitChange(furnitureDef.AllowOutfitChange);
		}
	}

	public void OnChangeRemovedFurnitureItem(int index)
	{
		m_currentSelectedDefaultFurnitureRemovalIndex = index;

		// teleport
		long propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;
		if (m_dictRemovals.ContainsKey(propertyID))
		{
			List<CPropertyDefaultFurnitureRemovalInstance> lstCurrentRemovals = m_dictRemovals[propertyID];
			CPropertyDefaultFurnitureRemovalInstance removalInstance = lstCurrentRemovals[index];

			RAGE.Vector3 vecPos = removalInstance.vecPos.CopyVector();
			vecPos.Z += 1.0f;
			//RAGE.Elements.Player.LocalPlayer.Position = vecPos;
		}
	}

	public void OnChangeCurrentFurnitureItem(int index)
	{
		m_currentSelectedCurrentFurnitureIndex = index;

		// teleport
		long propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;
		if (m_dictFurnitures.ContainsKey(propertyID))
		{
			List<CPropertyFurnitureInstance> lstCurrentFurniture = m_dictFurnitures[propertyID];
			CPropertyFurnitureInstance furnitureInstance = lstCurrentFurniture[index];

			RAGE.Vector3 vecPos = furnitureInstance.vecPos.CopyVector();
			vecPos.Z += 1.0f;
			//RAGE.Elements.Player.LocalPlayer.Position = vecPos;
		}
	}

	public void OnPlaceFurniture()
	{
		if (m_currentSelectedPlaceFurnitureIndex == -1)
		{
			m_EditInteriorUI.SetButtonsEnabled(true);
			ShowAlert("Please select a furniture item to place");
		}
		else
		{
			float fDist = 1.0f;
			Vector3 vecPosInFront = RAGE.Elements.Player.LocalPlayer.Position;
			float radians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f) * (3.14f / 180.0f);
			vecPosInFront.X += (float)Math.Cos(radians) * fDist;
			vecPosInFront.Y += (float)Math.Sin(radians) * fDist;
			vecPosInFront.Z = WorldHelper.GetGroundPosition(vecPosInFront);
			NetworkEventSender.SendNetworkEvent_EditInterior_PlaceFurniture(vecPosInFront.X, vecPosInFront.Y, vecPosInFront.Z, m_lstFurnitureItemsAvailable[m_currentSelectedPlaceFurnitureIndex].DatabaseID);
		}
	}

	public void OnPickupFurniture()
	{
		if (m_currentSelectedCurrentFurnitureIndex == -1)
		{
			m_EditInteriorUI.SetButtonsEnabled(true);
			ShowAlert("Please select a furniture item to pickup");
		}
		else
		{
			long propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;
			if (m_dictFurnitures.ContainsKey(propertyID))
			{
				List<CPropertyFurnitureInstance> lstCurrentFurniture = m_dictFurnitures[propertyID];
				CPropertyFurnitureInstance furnitureInstance = lstCurrentFurniture[m_currentSelectedCurrentFurnitureIndex];

				NetworkEventSender.SendNetworkEvent_EditInterior_PickupFurniture(furnitureInstance.DBID);
			}
		}
	}

	public void OnRestoreFurniture()
	{
		if (m_currentSelectedDefaultFurnitureRemovalIndex == -1)
		{
			m_EditInteriorUI.SetButtonsEnabled(true);
			ShowAlert("Please select a default furniture item to restore");
		}
		else
		{
			long propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;
			if (m_dictRemovals.ContainsKey(propertyID))
			{
				List<CPropertyDefaultFurnitureRemovalInstance> lstRemovedDefaultFurniture = m_dictRemovals[propertyID];
				CPropertyDefaultFurnitureRemovalInstance removalInstance = lstRemovedDefaultFurniture[m_currentSelectedDefaultFurnitureRemovalIndex];

				NetworkEventSender.SendNetworkEvent_EditInterior_RestoreFurniture(removalInstance.DBID);
			}
		}
	}

	public void OnExit()
	{
		if (m_EditInteriorUI.IsVisible())
		{
			RAGE.Game.Ui.DisplayRadar(true);
			HUD.SetVisible(true, false, false);
			m_EditInteriorUI.SetVisible(false, false, true);

			// TODO_FURNITURE: Add reset instead
			m_EditInteriorUI.Reload();

			// Reset local vars
			m_lstFurnitureItemsAvailable = null;
			m_currentSelectedPlaceFurnitureIndex = -1;
			m_currentSelectedCurrentFurnitureIndex = -1;
			m_strCurrentClass = String.Empty;
		}
	}

	private void OnGotoPropertyEditMode(List<CItemInstanceDef> lstFurnitureItemsAvailable)
	{
		RAGE.Game.Ui.DisplayRadar(false);
		HUD.SetVisible(false, false, false);

		m_bMovingX = true;
		m_bMovingY = true;
		m_bMovingZ = true;

		m_lstFurnitureItemsAvailable = lstFurnitureItemsAvailable;

		EditInterior_PopulateCurrentFurnitureList(RAGE.Elements.Player.LocalPlayer.Dimension);
		EditInterior_PopulateDefaultFurnitureRemovalList(RAGE.Elements.Player.LocalPlayer.Dimension);

		m_EditInteriorUI.SetVisible(true, false, true);

		if (m_strCurrentClass != String.Empty)
		{
			OnChangeClass(m_strCurrentClass);
		}
	}

	private CPropertyFurnitureInstance m_furnInstBeingMoved = null;
	private RAGE.Elements.MapObject m_ItemBeingMoved = null;

	private RAGE.Vector3 vecClickedLast = null;
	private RAGE.Vector3 vecStartDrag = null;
	private Vector2 vecLastCursorPos = null;
	private void UpdateRightClick()
	{
		if (!m_EditInteriorUI.IsVisible())
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
					if (RAGE.Game.Pad.IsDisabledControlJustReleased(0, 25))
					{
						NetworkEventSender.SendNetworkEvent_EditInterior_CommitChange(m_ItemBeingMoved.Position.X, m_ItemBeingMoved.Position.Y, m_ItemBeingMoved.Position.Z, m_ItemBeingMoved.GetRotation(0).X, m_ItemBeingMoved.GetRotation(0).Y, m_ItemBeingMoved.GetRotation(0).Z, m_furnInstBeingMoved.DBID);

						// Wait for the server update, allows editing with others in tandem
						m_furnInstBeingMoved = null;
						m_ItemBeingMoved = null;
					}
					else
					{
						RAGE.Vector3 vecClickedWorldPos = GraphicsHelper.GetWorldPositionFromScreenPosition(CursorManager.GetCursorPosition());

						if (vecClickedWorldPos != null)
						{
							if (!m_bRotating && m_bMovingX && m_bMovingY && m_bMovingZ)
							{
								m_ItemBeingMoved.Position = vecClickedWorldPos;
							}
							else
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

								// find furn
								long propertyID = RAGE.Elements.Player.LocalPlayer.Dimension;
								if (m_dictFurnitures.ContainsKey(propertyID))
								{
									List<CPropertyFurnitureInstance> lstCurrentFurniture = m_dictFurnitures[propertyID];
									foreach (CPropertyFurnitureInstance furnInst in lstCurrentFurniture)
									{
										if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnInst.FurnitureID))
										{
											CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[furnInst.FurnitureID];

											if (HashHelper.GetHashUnsigned(furnitureDef.Model) == objHit.Model)
											{
												if (furnInst.vecPos == objHit.Position)
												{
													if (m_ItemBeingMoved != objHit)
													{
														m_furnInstBeingMoved = furnInst;
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
						else
						{
							// try again for world, we have to recast :( sad
							raycast = WorldHelper.RaycastFromTo(vecGameplayCamPos, vecClickedWorldPos, RAGE.Elements.Player.LocalPlayer.Handle, 16);
							if (raycast.Hit && raycast.EntityHit == null)
							{
								if (RAGE.Game.Entity.GetEntityType(raycast.elementHitHandle) == 3)
								{
									uint model = RAGE.Game.Entity.GetEntityModel(raycast.elementHitHandle);
									NetworkEventSender.SendNetworkEvent_EditInterior_RemoveDefaultFurniture(raycast.EndCoords.X, raycast.EndCoords.Y, raycast.EndCoords.Z, model);
								}
							}
						}
					}
				}
			}
		}
	}

	private void OnRender()
	{
		UpdateRightClick();

		if (IsInEditMode())
		{
			// R Key :(
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack1);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack2);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackAlternate);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackHeavy);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackLight);

			if (m_strAlert != null)
			{
				TextHelper.Draw2D(m_strAlert, 0.4f, 0.86f, 0.5f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);

				// has it timed out?
				double timeSinceAlertCreated = (DateTime.Now - AlertCreatedTime).TotalMilliseconds;
				if (timeSinceAlertCreated > 5000)
				{
					m_strAlert = null;
				}
			}

			const float fFontScale = 0.4f;
			// controls
			TextHelper.Draw2D("Controls:", 0.01f, 0.75f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("Right click -> Select object/remove R* object, drag to move or rotate object", 0.03f, 0.8f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("X, Y, Z -> Switch axis in positioning/rotation mode", 0.03f, 0.85f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("R -> Switch between Rotation & Position mode", 0.03f, 0.9f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("B -> Positioning - Switch to Snap to XYZ mode", 0.03f, 0.95f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);

			// current status
			if (!m_bRotating && m_bMovingX && m_bMovingY && m_bMovingZ)
			{
				TextHelper.Draw2D("Edit Mode: Snap to XYZ", 0.01f, 0.7f, fFontScale, new RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			}
			else
			{
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
	}
}