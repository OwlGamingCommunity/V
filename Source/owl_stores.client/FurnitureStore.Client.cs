using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;

public class FurnitureStore
{
	private CGUIFurnitureStore m_FurnitureStoreGUI = new CGUIFurnitureStore(() => { });

	private List<Purchaser> m_lstPurchasers = new List<Purchaser>();
	private List<string> m_lstMethods = new List<string>();

	private uint m_currentSelectedFurnitureIndex = 999999;

	private RAGE.Vector3 m_vecExitPos = null;

	private RAGE.Vector3 m_vecCamLookAt = new RAGE.Vector3();
	private readonly Vector3Definition g_vecCarPos = new Vector3Definition(228.8492f, -992.0068f, -100.009995f);

	private float g_fCurrentSalesTax = 0.0f;
	private MapObject m_FurnitureObject = null;

	// CAMERA ROTATION & ZOOMING
	private float m_fObjectRot = 45.0f;
	private EFurnitureStoreRotationDirection m_RotationDirection = EFurnitureStoreRotationDirection.None;
	private EFurnitureStoreZoomDirection m_ZoomDirection = EFurnitureStoreZoomDirection.None;
	const float g_fMaxZoom = 7.5f;
	const float g_fMinZoom = 0.5f;
	const float g_fDefaultZoom = g_fMaxZoom;
	float m_fCurrentZoom = g_fDefaultZoom;

	public FurnitureStore()
	{
		NetworkEvents.GotoFurnitureStore += OnGotoFurnitureStore;
		NetworkEvents.FurnitureStore_OnCheckoutResult += OnCheckoutResult;

		CameraManager.RegisterCamera(ECameraID.FURNITURE_STORE, g_vecCarPos.AsRageVector(), m_vecCamLookAt, new RAGE.Vector3(-90.0f, -90.0f, 45.0f), 60.0f);
		UpdateCamera();

		RageEvents.RAGE_OnTick_PerFrame += OnTick;
	}

	private void ResetData()
	{
		if (m_FurnitureObject != null)
		{
			m_FurnitureObject.Destroy();
			m_FurnitureObject = null;
		}

		m_fObjectRot = 45.0f;
		m_RotationDirection = EFurnitureStoreRotationDirection.None;
		m_ZoomDirection = EFurnitureStoreZoomDirection.None;
		m_fCurrentZoom = g_fDefaultZoom;
	}

	private void OnCheckoutResult(EStoreCheckoutResult result)
	{
		m_FurnitureStoreGUI.SetButtonsEnabled(true);

		if (result == EStoreCheckoutResult.CannotAfford)
		{
			m_FurnitureStoreGUI.ShowErrorMessage("You cannot afford that furniture.");
		}
		else if (result == EStoreCheckoutResult.FailedPartial)
		{
			m_FurnitureStoreGUI.ShowErrorMessage("The furniture could not be purchased. Please check the notification.");
		}
		else if (result == EStoreCheckoutResult.Success)
		{
			OnExit();
		}
	}

	private void OnTick()
	{
		if (IsRotating() && m_FurnitureObject != null)
		{
			const float fDeltaRot = 4.0f;

			if (m_RotationDirection == EFurnitureStoreRotationDirection.Left)
			{
				m_fObjectRot -= fDeltaRot;
			}
			else if (m_RotationDirection == EFurnitureStoreRotationDirection.Right)
			{
				m_fObjectRot += fDeltaRot;
			}

			if (m_fObjectRot >= 360.0f)
			{
				m_fObjectRot = 0.0f;
			}
			else if (m_fObjectRot <= 0.0f)
			{
				m_fObjectRot = 360.0f;
			}

			m_FurnitureObject.SetRotation(0.0f, 0.0f, m_fObjectRot, 0, true);
		}

		if (IsZooming() && m_FurnitureObject != null)
		{
			const float fDeltaZoom = 0.1f;
			m_fCurrentZoom += (m_ZoomDirection == EFurnitureStoreZoomDirection.In) ? -fDeltaZoom : fDeltaZoom;
			m_fCurrentZoom = Math.Clamp(m_fCurrentZoom, g_fMinZoom, g_fMaxZoom);
		}

		if (IsRotating() || IsZooming())
		{
			UpdateCamera();
		}

		// TODO: Better way of tracking this
		if (m_FurnitureStoreGUI.IsVisible())
		{
			RAGE.Game.Ui.DisplayRadar(false);
		}
	}

	private bool IsRotating()
	{
		return m_RotationDirection != EFurnitureStoreRotationDirection.None;
	}

	private bool IsZooming()
	{
		return m_ZoomDirection != EFurnitureStoreZoomDirection.None;
	}

	public void OnStartRotation(EFurnitureStoreRotationDirection direction)
	{
		m_RotationDirection = direction;
	}

	public void OnStopRotation()
	{
		m_RotationDirection = EFurnitureStoreRotationDirection.None;
	}

	public void OnStartZoom(EFurnitureStoreZoomDirection direction)
	{
		m_ZoomDirection = direction;
	}

	public void OnStopZoom()
	{
		m_ZoomDirection = EFurnitureStoreZoomDirection.None;
	}

	public void OnResetCamera()
	{
		m_RotationDirection = EFurnitureStoreRotationDirection.None;
		m_ZoomDirection = EFurnitureStoreZoomDirection.None;
		m_fCurrentZoom = g_fDefaultZoom;
		m_fObjectRot = 45.0f;
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		// Calculate cam pos
		var radians = 45.0f * (3.14 / 180.0);
		RAGE.Vector3 vecCamPosNew = g_vecCarPos.AsRageVector();
		vecCamPosNew.X += (float)Math.Cos(radians) * m_fCurrentZoom;
		vecCamPosNew.Y += (float)Math.Sin(radians) * m_fCurrentZoom;
		vecCamPosNew.Z += 2.5f;

		CameraManager.UpdateCamera(ECameraID.FURNITURE_STORE, vecCamPosNew, m_vecCamLookAt, new RAGE.Vector3(-90.0f, -90.0f, 45.0f));
	}

	private void OnGotoFurnitureStore(float fCurrentSalesTax)
	{
		g_fCurrentSalesTax = fCurrentSalesTax;

		m_vecCamLookAt = g_vecCarPos.AsRageVector();
		ResetData();

		m_vecExitPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();

		NetworkEventSender.SendNetworkEvent_RequestPlayerSpecificDimension();

		HUD.SetVisible(false, false, false);

		OnResetCamera();
		UpdateCamera();

		RAGE.Elements.Player.LocalPlayer.Position = g_vecCarPos.AsRageVector().Add(new RAGE.Vector3(2.0f, 0.0f, 5.0f));
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

		CameraManager.ActivateCamera(ECameraID.FURNITURE_STORE);

		m_FurnitureStoreGUI.SetVisible(true, true, true);

		m_lstPurchasers.Clear();
		m_lstMethods.Clear();
	}

	public void OnChangeClass(string strClass)
	{
		m_currentSelectedFurnitureIndex = 999999;

		bool bIncludeAllFurnitureItems = strClass.ToLower() == "allstorage";
		bool bIncludeAllOutfitChangeItems = strClass.ToLower() == "alloutfitchange";

		foreach (var kvPair in FurnitureDefinitions.g_FurnitureDefinitions)
		{
			CFurnitureDefinition furnitureDef = kvPair.Value;

			if (furnitureDef.Purchasable)
			{
				if (bIncludeAllFurnitureItems)
				{
					if (furnitureDef.StorageCapacity > 0)
					{
						m_FurnitureStoreGUI.AddFurnitureItem(kvPair.Key, furnitureDef.Name);
					}
				}
				else if (bIncludeAllOutfitChangeItems)
				{
					if (furnitureDef.AllowOutfitChange)
					{
						m_FurnitureStoreGUI.AddFurnitureItem(kvPair.Key, furnitureDef.Name);
					}
				}
				else
				{
					if (furnitureDef.Category.ToString().ToLower() == strClass.ToLower())
					{
						m_FurnitureStoreGUI.AddFurnitureItem(kvPair.Key, furnitureDef.Name);
					}
				}
			}
		}

		m_FurnitureStoreGUI.CommitFurnitureItems();
	}

	public void OnCheckout()
	{
		if (m_currentSelectedFurnitureIndex != 999999)
		{
			NetworkEventSender.SendNetworkEvent_FurnitureStore_OnCheckout(StoreSystem.CurrentStoreID, m_currentSelectedFurnitureIndex);
		}
		else
		{
			m_FurnitureStoreGUI.SetButtonsEnabled(true);
		}
	}

	public void OnChangeFurnitureItem(uint index)
	{
		if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(index))
		{
			CFurnitureDefinition furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[index];

			if (furnitureDef != null)
			{
				if (m_FurnitureObject != null)
				{
					m_FurnitureObject.Destroy();
				}

				uint hash = HashHelper.GetHashUnsigned(furnitureDef.Model);
				AsyncModelLoader.RequestSyncInstantLoad(hash);
				m_FurnitureObject = new MapObject(hash, g_vecCarPos, new Vector3(0.0f, 0.0f, m_fObjectRot), 255, RAGE.Elements.Player.LocalPlayer.Dimension);

				m_FurnitureStoreGUI.SetPriceInfo(furnitureDef.Price, (g_fCurrentSalesTax * furnitureDef.Price), furnitureDef.Price + (g_fCurrentSalesTax * furnitureDef.Price), furnitureDef.StorageCapacity, furnitureDef.AllowOutfitChange);

				m_currentSelectedFurnitureIndex = index;
			}
		}
	}

	public void OnExit()
	{
		if (m_FurnitureStoreGUI.IsVisible())
		{
			RAGE.Elements.Player.LocalPlayer.Position = m_vecExitPos;
			RAGE.Elements.Player.LocalPlayer.FreezePosition(false);

			NetworkEventSender.SendNetworkEvent_RequestPlayerNonSpecificDimension();

			CameraManager.DeactivateCamera(ECameraID.FURNITURE_STORE);

			// TODO: Camera fades
			m_FurnitureStoreGUI.SetVisible(false, false, true);
			HUD.SetVisible(true, false, false);

			// TODO_RAGE: Add a reset
			m_FurnitureStoreGUI.Reload();

			ResetData();

			RAGE.Game.Ui.DisplayRadar(true);
		}
	}
}