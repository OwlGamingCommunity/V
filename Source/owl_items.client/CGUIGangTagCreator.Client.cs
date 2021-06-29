internal class CGUIGangTagCreator : CEFCore
{
	public CGUIGangTagCreator(OnGUILoadedDelegate callbackOnLoad) : base("owl_items.client/gangtagcreator.html", EGUIID.GangTagCreator, callbackOnLoad)
	{
		UIEvents.GangTagCreator_AddLayer_Sprite += () => { ItemSystem.GetGangTagCreator()?.OnAddSpriteLayer(); };
		UIEvents.GangTagCreator_AddLayer_Rectangle += () => { ItemSystem.GetGangTagCreator()?.OnAddRectangleLayer(); };
		UIEvents.GangTagCreator_AddLayer_Text += () => { ItemSystem.GetGangTagCreator()?.OnAddTextLayer(); };
		UIEvents.GangTagCreator_DeleteLayer += (int layerID) => { ItemSystem.GetGangTagCreator()?.OnDeleteLayer(layerID); };
		UIEvents.GangTagCreator_EditLayer += (int layerID) => { ItemSystem.GetGangTagCreator()?.OnEditLayer(layerID); };
		UIEvents.GangTagCreator_MoveLayerUp += (int layerID) => { ItemSystem.GetGangTagCreator()?.OnMoveLayerUp(layerID); };
		UIEvents.GangTagCreator_MoveLayerDown += (int layerID) => { ItemSystem.GetGangTagCreator()?.OnMoveLayerDown(layerID); };

		UIEvents.GangTagCreator_Exit_Save += () => { ItemSystem.GetGangTagCreator()?.OnExit_Save(); };
		UIEvents.GangTagCreator_Exit_Discard += () => { ItemSystem.GetGangTagCreator()?.OnExit_Discard(); };
		UIEvents.GangTagCreator_Exit_KeepWIP += () => { ItemSystem.GetGangTagCreator()?.OnExit_KeepWIP(); };

		UIEvents.GangTagCreator_EditLayer_SetFontID += (int fontID) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetFontID(fontID); };
		UIEvents.GangTagCreator_EditLayer_SetOutline += (bool bOutline) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetOutline(bOutline); };
		UIEvents.GangTagCreator_EditLayer_SetShadow += (bool bShadow) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetShadow(bShadow); };
		UIEvents.GangTagCreator_EditLayer_SetText += (string strText) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetText(strText); };
		UIEvents.GangTagCreator_EditLayer_SetScale += (float fScale) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetScale(fScale); };
		UIEvents.GangTagCreator_EditLayer_SetColor += (int r, int g, int b) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetColor(r, g, b); };
		UIEvents.GangTagCreator_EditLayer_SetAlpha += (int alpha) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetAlpha(alpha); };
		UIEvents.GangTagCreator_EditLayer_SetXCoordinate += (float fX) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetXCoordinate(fX); };
		UIEvents.GangTagCreator_EditLayer_SetYCoordinate += (float fY) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetYCoordinate(fY); };
		UIEvents.GangTagCreator_EditLayer_SetWidth += (float fWidth) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetWidth(fWidth); };
		UIEvents.GangTagCreator_EditLayer_SetHeight += (float fHeight) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetHeight(fHeight); };
		UIEvents.GangTagCreator_EditLayer_SetSpriteRotation += (float fRotation) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_SetSpriteRotation(fRotation); };
		UIEvents.GangTagCreator_EditLayer_ChangeSprite += (string strHumanName) => { ItemSystem.GetGangTagCreator()?.OnEditLayer_ChangeSprite(strHumanName); };
	}

	public override void OnLoad()
	{

	}

	public void ClearLayers()
	{
		Execute("ClearLayers");
	}

	public void AddLayer(int layerID, ELayerType layerType)
	{
		Execute("AddLayer", layerID, layerType, Helpers.FormatString("{0} Layer", layerType.ToString()));
	}

	public void GotoEditLayer(GangTagLayer layer)
	{
		if (layer.T == ELayerType.Text)
		{
			Execute("GotoEditTextLayer", layer.Txt, layer.R, layer.G, layer.B, layer.A, layer.X, layer.Y, layer.S, layer.Font, layer.OL, layer.SH);
		}
		else if (layer.T == ELayerType.Sprite)
		{
			Execute("GotoEditSpriteLayer", layer.R, layer.G, layer.B, layer.A, layer.X, layer.Y, layer.W, layer.H, layer.ROT);
		}
		else if (layer.T == ELayerType.Rectangle)
		{
			Execute("GotoEditRectangleLayer", layer.R, layer.G, layer.B, layer.A, layer.X, layer.Y, layer.W, layer.H);
		}
	}

	public void RemoveLayer(int layerID)
	{
		Execute("RemoveLayer", layerID);
	}

	public void SetUsageInfo(int numLayers, int maxLayers)
	{
		Execute("SetUsageInfo", numLayers, maxLayers);
	}

	public void RegisterSprite(string strHumanName)
	{
		Execute("RegisterSprite", strHumanName);
	}

	public void CommitSprites()
	{
		Execute("CommitSprites");
	}
}