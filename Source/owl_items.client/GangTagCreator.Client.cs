using RAGE.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

public class GangTagCreator
{
	private const int g_MaxLayers = 10;
	private List<GangTagLayer> m_GangTagLayers = new List<GangTagLayer>();
	private bool m_bIsDirty = false;
	private WeakReference<ClientTimer> m_timerSaveWIP = new WeakReference<ClientTimer>(null);

	private CGUIGangTagCreator m_GangTagCreatorUI = null;

	private int g_LayerBeingEdited = -1;
	private bool m_bIsInEditor = false;

	private bool m_bIsInShare = false;

	private int m_RenderTargetID = -1;
	private const string g_RenderTargetName = "clubhouse_wall";
	private const uint g_RenderObjectModel = 2539789527;
	RAGE.Elements.MapObject m_TagObject = null;

	private const int g_TimeBetweenWIPSaves = 30000;
	private const int g_TimeToShowSavedMessage = 10000;
	private bool m_bRenderSavedMessage = false;

	private RAGE.Vector3 g_vecWallPos = new RAGE.Vector3(-272.3f, 6166.949f, 32.0f);
	private RAGE.Vector3 g_vecPlayerRestorePos = null;

	// NOTE: Only used during the 'which tag do you want' phase
	private List<GangTagLayer> m_lstPendingChoice_SavedTag;
	private List<GangTagLayer> m_lstPendingChoice_WIPTag;

	public GangTagCreator()
	{
		m_GangTagCreatorUI = new CGUIGangTagCreator(() => { });

		RAGE.Vector3 vecCameraPos = g_vecWallPos.CopyVector();
		float fRot = 45.03018f;
		float fDist = 1.4f;
		float radians = (fRot + 90.0f) * (3.14f / 180.0f);
		vecCameraPos.X += (float)Math.Cos(radians) * fDist;
		vecCameraPos.Y += (float)Math.Sin(radians) * fDist;

		CameraManager.RegisterCamera(ECameraID.TAGCREATOR, vecCameraPos, g_vecWallPos);

		// Render target
		RAGE.Vector3 vecRenderTargetPos = g_vecWallPos.CopyVector();
		fDist = 0.01f;
		radians = (fRot + 90.0f) * (3.14f / 180.0f);
		vecRenderTargetPos.X += (float)Math.Cos(radians) * fDist;
		vecRenderTargetPos.Y += (float)Math.Sin(radians) * fDist;

		AsyncModelLoader.RequestSyncInstantLoad(g_RenderObjectModel);
		m_TagObject = new RAGE.Elements.MapObject(g_RenderObjectModel, vecRenderTargetPos, new RAGE.Vector3(0.0f, 0.0f, 225.0f));

		NetworkEvents.GangTags_GotoCreator += GotoCreator;

		NetworkEvents.GangTags_RequestShareTag += OnRequestShareTag;

		RageEvents.RAGE_OnRender += OnRender;

		UIEvents.GangTag_EditSavedTag += OnConfirm_EditSavedTag;
		UIEvents.GangTag_EditWIPTag += OnConfirm_EditWIPTag;

		UIEvents.GangTag_AcceptShare += OnAcceptShare;
		UIEvents.GangTag_DeclineShare += OnDeclineShare;
	}

	private void OnAcceptShare()
	{
		HideCreatorUI();
		NotificationManager.ShowNotification("Gang Tags", "You have accepted the gang tag share request", ENotificationIcon.InfoSign);
		NetworkEventSender.SendNetworkEvent_GangTags_AcceptShare(m_GangTagLayers);
	}

	private void OnDeclineShare()
	{
		HideCreatorUI();
		NotificationManager.ShowNotification("Gang Tags", "You have declined the gang tag share request", ENotificationIcon.InfoSign);
	}

	private void OnRequestShareTag(string strSourceName, List<GangTagLayer> lstLayers)
	{
		GotoCreatorWithData(lstLayers, true);

		GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("'{0}' would like to share the gang tag shown with you.", strSourceName), "Accept the share and use their tag!", "Decline the share and keep my own tag", UIEventID.GangTag_AcceptShare, UIEventID.GangTag_DeclineShare, EPromptPosition.Center_Bottom);
	}

	private void OnConfirm_EditSavedTag()
	{
		GotoCreatorWithData(m_lstPendingChoice_SavedTag, false);
		m_lstPendingChoice_SavedTag = null;
	}

	private void OnConfirm_EditWIPTag()
	{
		GotoCreatorWithData(m_lstPendingChoice_WIPTag, false);
		m_lstPendingChoice_WIPTag = null;
	}

	private void OnSaveWIP(object[] parameters)
	{
		if (m_bIsDirty)
		{
			m_bIsDirty = false;

			NetworkEventSender.SendNetworkEvent_GangTags_SaveWIP(m_GangTagLayers);
			m_bRenderSavedMessage = true;
			ClientTimerPool.CreateTimer((object[] innerParameters) => { m_bRenderSavedMessage = false; }, g_TimeToShowSavedMessage, 1);
		}
	}

	private void GotoCreator(List<GangTagLayer> lstLayers, List<GangTagLayer> lstLayersWIP)
	{
		if (lstLayersWIP.Count > 0 && lstLayers.Count > 0)
		{
			m_lstPendingChoice_SavedTag = lstLayers;
			m_lstPendingChoice_WIPTag = lstLayersWIP;
			GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("You have an active tag as well as a WIP tag. Which would you like to edit?"), "Edit active tag & overwrite WIP tag", "Edit WIP tag & overwrite tag when saved", UIEventID.GangTag_EditSavedTag, UIEventID.GangTag_EditWIPTag);
		}
		else if (lstLayersWIP.Count > 0)
		{
			GotoCreatorWithData(lstLayersWIP, false);
		}
		else
		{
			GotoCreatorWithData(lstLayers, false);
		}
	}

	private void GotoCreatorWithData(List<GangTagLayer> lstLayers, bool bIsShareTagViewer)
	{
		m_GangTagLayers = lstLayers;

		g_LayerBeingEdited = -1;

		// TODO_TAGS: set dimension to player dim on server, when we have a way of accessing this that isnt a keybind
		NetworkEventSender.SendNetworkEvent_RequestPlayerSpecificDimension();

		if (g_vecPlayerRestorePos == null)
		{
			g_vecPlayerRestorePos = RAGE.Elements.Player.LocalPlayer.Position;
		}
		RAGE.Vector3 vecPlayerPos = g_vecWallPos.CopyVector();
		float fRot = 45.03018f;
		float fDist = 3.0f;
		float radians = (fRot + 90.0f) * (3.14f / 180.0f);
		vecPlayerPos.X += (float)Math.Cos(radians) * fDist;
		vecPlayerPos.Y += (float)Math.Sin(radians) * fDist;
		RAGE.Elements.Player.LocalPlayer.Position = vecPlayerPos;


		m_bIsDirty = false;

		HUD.SetVisible(false, true, true);
		RAGE.Chat.Show(false);

		if (!bIsShareTagViewer)
		{
			ShowCreatorUI();
			RepopulateLayersUI();

			// save WIP timer
			m_timerSaveWIP = ClientTimerPool.CreateTimer(OnSaveWIP, g_TimeBetweenWIPSaves, -1);
		}
		else
		{
			m_bIsInShare = true;
		}

		CameraManager.ActivateCamera(ECameraID.TAGCREATOR);
	}

	private void ShowCreatorUI()
	{
		UpdateUsageInfo();

		// register sprites
		foreach (var kvPair in GangTagSpriteDefinitions.g_GangTagSpriteDefinitions)
		{
			m_GangTagCreatorUI.RegisterSprite(kvPair.Value.HumanName);
		}
		m_GangTagCreatorUI.CommitSprites();

		m_bIsInEditor = true;

		m_GangTagCreatorUI.SetVisible(true, true, false);
	}

	private void HideCreatorUI()
	{
		m_bIsInEditor = false;
		m_bIsInShare = false;
		m_GangTagCreatorUI.SetVisible(false, false, false);
		HUD.SetVisible(true, false, true);
		RAGE.Chat.Show(true);

		m_GangTagCreatorUI.Reload();

		// TODO_TAGS: set dimension to player dim on server, when we have a way of accessing this that isnt a keybind
		NetworkEventSender.SendNetworkEvent_RequestPlayerNonSpecificDimension();

		if (g_vecPlayerRestorePos != null)
		{
			RAGE.Elements.Player.LocalPlayer.Position = g_vecPlayerRestorePos;
			g_vecPlayerRestorePos = null;
		}

		CameraManager.DeactivateCamera(ECameraID.TAGCREATOR);

		ClientTimerPool.MarkTimerForDeletion(ref m_timerSaveWIP);
	}

	// TODO_TAGS: Cleanup render target
	private void CreateRenderTarget()
	{
		if (!RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetName))
		{
			RAGE.Game.Ui.RegisterNamedRendertarget(g_RenderTargetName, false);
		}

		// Link it to all models
		if (!RAGE.Game.Ui.IsNamedRendertargetLinked(g_RenderObjectModel))
		{
			RAGE.Game.Ui.LinkNamedRendertarget(g_RenderObjectModel);
		}

		// Get the handle
		if (RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetName))
		{
			m_RenderTargetID = RAGE.Game.Ui.GetNamedRendertargetRenderId(g_RenderTargetName);
		}
	}

	private void OnRender()
	{
		if (!m_bIsInEditor && !m_bIsInShare)
		{
			return;
		}

		if (m_RenderTargetID == -1)
		{
			CreateRenderTarget();
		}

		m_TagObject.Dimension = Player.LocalPlayer.Dimension;

		// set render target + rtt layer
		RAGE.Game.Ui.SetTextRenderId(m_RenderTargetID);
		RAGE.Game.Graphics.Set2dLayer(4);

		// background
		RAGE.Game.Graphics.DrawRect(0.0f, 0.0f, 2.0f, 2.0f, 0, 0, 255, 150, 1);

		foreach (var layer in m_GangTagLayers)
		{
			layer.Render();
		}

		// Reset render target
		RAGE.Game.Ui.SetTextRenderId(1);

		if (m_bRenderSavedMessage)
		{
			HUD.SetLoadingMessage("WIP Gang Tag Saved!");
		}

		// TODO: Why does input get disabled?
		m_GangTagCreatorUI.SetInputEnabled(true);

		if (!m_bIsInShare)
		{
			TextHelper.Draw2D("Changes made are automatically saved into your WIP tag to avoid data loss.", 0.5f, 0.8f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			TextHelper.Draw2D("This does not overwrite your active tag until you choose to Save above.", 0.5f, 0.83f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
		}
	}

	// UI CALLBACKS
	public void OnAddSpriteLayer()
	{
		MakeDirty();

		int layerID = m_GangTagLayers.Count;
		string strLayerName = Helpers.FormatString("Layer {0} (Sprite)", layerID + 1);
		m_GangTagLayers.Add(new GangTagLayer(ELayerType.Sprite, layerID, 255, 194, 15, 255, 0.5f, 0.5f, 0.5f, String.Empty, 0, false, false, 0, 0.5f, 0.5f, 0.0f));
		RepopulateLayersUI();

		OnEditLayer(layerID);

		UpdateUsageInfo();
	}

	public void OnAddRectangleLayer()
	{
		MakeDirty();

		int layerID = m_GangTagLayers.Count;
		string strLayerName = Helpers.FormatString("Layer {0} (Rectangle)", layerID + 1);
		m_GangTagLayers.Add(new GangTagLayer(ELayerType.Rectangle, layerID, 255, 194, 15, 255, 0.5f, 0.5f, 0.5f, String.Empty, -1, false, false, -1, 0.5f, 0.5f, 0.5f));

		RepopulateLayersUI();

		OnEditLayer(layerID);

		UpdateUsageInfo();
	}

	public void OnAddTextLayer()
	{
		MakeDirty();

		int layerID = m_GangTagLayers.Count;
		string strLayerName = Helpers.FormatString("Layer {0} (Text)", layerID + 1);
		var newLayer = new GangTagLayer(ELayerType.Text, layerID, 0, 0, 0, 255, 0.5f, 0.5f, 1.0f, "Text", (int)RAGE.Game.Font.ChaletLondon, false, false, -1, 0.0f, 0.0f, 0.0f);
		m_GangTagLayers.Add(newLayer);
		RepopulateLayersUI();

		OnEditLayer(layerID);

		UpdateUsageInfo();
	}

	private GangTagLayer GetLayerFromID(int layerID)
	{
		return m_GangTagLayers.Find(x => x.ID == layerID);
	}

	private GangTagLayer GetLayerBeingEdited()
	{
		return GetLayerFromID(g_LayerBeingEdited);
	}

	private void UpdateUsageInfo()
	{
		m_GangTagCreatorUI.SetUsageInfo(m_GangTagLayers.Count, g_MaxLayers);
	}

	public void OnDeleteLayer(int layerID)
	{
		MakeDirty();

		m_GangTagLayers.RemoveAll(x => x.ID == layerID);
		m_GangTagCreatorUI.RemoveLayer(layerID);

		// remove gap ID's
		int lastID = -1;
		foreach (var layer in m_GangTagLayers)
		{
			if (layer.ID - lastID > 1)
			{
				// shift
				layer.ID--;
			}

			lastID = layer.ID;
		}

		RepopulateLayersUI();

		UpdateUsageInfo();
	}

	public void OnEditLayer(int layerID)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerFromID(layerID);
		if (layer != null)
		{
			g_LayerBeingEdited = layerID;

			m_GangTagCreatorUI.GotoEditLayer(layer);
		}
	}

	public void OnMoveLayerUp(int layerID)
	{
		MakeDirty();

		int targetLayerID = layerID + 1;

		GangTagLayer layer = GetLayerFromID(layerID);
		GangTagLayer layerSwap = GetLayerFromID(targetLayerID);

		if (layer != null && layerSwap != null)
		{
			// swap
			m_GangTagLayers[layerID] = layerSwap;
			m_GangTagLayers[targetLayerID] = layer;

			// update internal ID
			layerSwap.ID = layerID;
			layer.ID = targetLayerID;

			RepopulateLayersUI();
		}
	}

	private void RepopulateLayersUI()
	{
		// repopulate ui
		m_GangTagCreatorUI.ClearLayers();
		foreach (var iterLayer in Enumerable.Reverse(m_GangTagLayers))
		{
			m_GangTagCreatorUI.AddLayer(iterLayer.ID, iterLayer.T);
		}
	}

	public void OnMoveLayerDown(int layerID)
	{
		MakeDirty();

		int targetLayerID = layerID - 1;

		GangTagLayer layer = GetLayerFromID(layerID);
		GangTagLayer layerSwap = GetLayerFromID(targetLayerID);

		if (layer != null && layerSwap != null)
		{
			// swap
			m_GangTagLayers[layerID] = layerSwap;
			m_GangTagLayers[targetLayerID] = layer;

			// update internal ID
			layerSwap.ID = layerID;
			layer.ID = targetLayerID;

			RepopulateLayersUI();
		}
	}

	/////////////////////////////////
	/// TEXT LAYERS
	/////////////////////////////////
	public void OnEditLayer_SetOutline(bool bOutline)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null && layer.T == ELayerType.Text)
		{
			layer.OL = bOutline;
		}
	}

	public void OnEditLayer_SetShadow(bool bShadow)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null && layer.T == ELayerType.Text)
		{
			layer.SH = bShadow;
		}
	}

	public void OnEditLayer_SetText(string a_strText)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null && layer.T == ELayerType.Text)
		{
			layer.Txt = a_strText;
		}
	}

	public void MakeDirty()
	{
		m_bIsDirty = true;
	}

	public void OnEditLayer_SetFontID(int fontID)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer.T == ELayerType.Text)
		{
			layer.Font = fontID;
		}
	}

	public void OnEditLayer_SetScale(float fScale)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			layer.S = fScale;
		}
	}

	public void OnEditLayer_SetColor(int r, int g, int b)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			layer.R = r;
			layer.G = g;
			layer.B = b;
		}
	}

	public void OnEditLayer_SetAlpha(int alpha)
	{
		MakeDirty();

		if (alpha >= 0 && alpha <= 255)
		{
			GangTagLayer layer = GetLayerBeingEdited();
			if (layer != null)
			{
				layer.A = alpha;
			}
		}
	}

	public void OnEditLayer_SetXCoordinate(float fX)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			layer.X = fX;
		}
	}

	public void OnEditLayer_SetYCoordinate(float fY)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			layer.Y = fY;
		}
	}

	public void OnEditLayer_SetWidth(float fWidth)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			if (layer.T == ELayerType.Sprite)
			{
				layer.W = fWidth;
			}
			else if (layer.T == ELayerType.Rectangle)
			{
				layer.W = fWidth;
			}
		}
	}

	public void OnEditLayer_SetHeight(float fHeight)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			if (layer.T == ELayerType.Sprite)
			{
				layer.H = fHeight;
			}
			else if (layer.T == ELayerType.Rectangle)
			{
				layer.H = fHeight;
			}
		}
	}

	public void OnEditLayer_SetSpriteRotation(float fRotation)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			if (layer.T == ELayerType.Sprite)
			{
				layer.ROT = fRotation;
			}
		}
	}


	public void OnEditLayer_ChangeSprite(string strHumanName)
	{
		MakeDirty();

		GangTagLayer layer = GetLayerBeingEdited();
		if (layer != null)
		{
			if (layer.T == ELayerType.Sprite)
			{
				CGangTagSpriteDefinition spriteDef = GangTagSpriteDefinitions.GetGangTagSpriteDefinitionFromHumanName(strHumanName);
				if (spriteDef != null)
				{
					layer.SID = spriteDef.ID;
				}
			}
		}
	}
	/////////////////////////////////
	/// END TEXT LAYERS
	/////////////////////////////////

	public void OnExit_Save()
	{
		NetworkEventSender.SendNetworkEvent_GangTags_SaveActive(m_GangTagLayers);
		HideCreatorUI();
	}

	public void OnExit_Discard()
	{
		NetworkEventSender.SendNetworkEvent_GangTags_SaveWIP(new List<GangTagLayer>());
		HideCreatorUI();
	}

	public void OnExit_KeepWIP()
	{
		NetworkEventSender.SendNetworkEvent_GangTags_SaveWIP(m_GangTagLayers);
		HideCreatorUI();
	}
	// END UI CALLBACKS
}







