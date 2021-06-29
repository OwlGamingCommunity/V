public class PDRenderTargets
{
	private int m_RenderTargetID_Billboard = -1;
	private const string g_RenderTargetName = "big_disp";
	private const uint g_RenderObjectModel = 3542263935;
	RAGE.Elements.MapObject m_BillboardObjLS = null;

	public PDRenderTargets()
	{
		AsyncModelLoader.RequestSyncInstantLoad(g_RenderObjectModel);
		m_BillboardObjLS = new RAGE.Elements.MapObject(g_RenderObjectModel, new RAGE.Vector3(425.1f, -994.0f, 37.0f), new RAGE.Vector3(0.0f, 0.0f, 270.0f)); // LS

		RageEvents.RAGE_OnRender += OnRender;
	}

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
			m_RenderTargetID_Billboard = RAGE.Game.Ui.GetNamedRendertargetRenderId(g_RenderTargetName);
		}
	}

	// TODO_CSHARP: Make a render target class... lots of dupe code here
	private void OnRender()
	{
		float fDistance = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, m_BillboardObjLS.Position);
		if (fDistance < 100.0f)
		{
			if (m_BillboardObjLS.IsOnScreen())
			{
				if (m_RenderTargetID_Billboard == -1)
				{
					CreateRenderTarget();
				}

				// set render target + rtt layer
				RAGE.Game.Ui.SetTextRenderId(m_RenderTargetID_Billboard);
				RAGE.Game.Graphics.Set2dLayer(4);

				// STARS
				const string strDictStar = "3dtextures";
				if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(strDictStar))
				{
					RAGE.Game.Graphics.RequestStreamedTextureDict(strDictStar, true);
				}

				if (RAGE.Game.Graphics.HasStreamedTextureDictLoaded(strDictStar))
				{
					float width = 0.3f;
					float height = 0.6f;
					RAGE.Game.Graphics.DrawSprite(strDictStar, "mpgroundlogo_cops", 0.5f, 0.45f, width, height, 0.0f, 255, 255, 255, 255, 0);
				}

				// FLAGS
				const string strDictFlag = "mpcarhud";
				if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(strDictFlag))
				{
					RAGE.Game.Graphics.RequestStreamedTextureDict(strDictFlag, true);
				}

				if (RAGE.Game.Graphics.HasStreamedTextureDictLoaded(strDictFlag))
				{
					float width = 0.06f;
					float height = 0.09f;

					RAGE.Game.Graphics.DrawSprite(strDictFlag, "vehicle_card_icons_flag_usa", 0.05f, 0.11f, width, height, 0.0f, 255, 255, 255, 255, 0);
					RAGE.Game.Graphics.DrawSprite(strDictFlag, "vehicle_card_icons_flag_usa", 0.95f, 0.11f, width, height, 0.0f, 255, 255, 255, 255, 0);
				}

				// TODO_LAUNCH: Hook up arrest counter
				TextHelper.Draw2D("LOS SANTOS POLICE DEPARTMENT", 0.5f, 0.05f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false, true);
				TextHelper.Draw2D("~HUD_COLOUR_RADAR_DAMAGE~Making ~HUD_COLOUR_PURE_WHITE~Los Santos ~HUD_COLOUR_FREEMODE_DARK~Safe Again", 0.5f, 0.75f, 0.4f, 255, 0, 0, 255, RAGE.Game.Font.Monospace, RAGE.NUI.UIResText.Alignment.Centered, true, false, true);
				TextHelper.Draw2D("Chief Richard Bankfield", 0.5f, 0.85f, 0.35f, 255, 255, 255, 255, RAGE.Game.Font.HouseScript, RAGE.NUI.UIResText.Alignment.Centered, true, false, true);
				//TextHelper.Draw2D("0 Arrests To Date", 0.5f, 0.8f, 0.3f, 255, 255, 255, 255, RAGE.Game.Font.Monospace, RAGE.NUI.UIResText.Alignment.Centered, true, false, true);

				// Reset render target
				RAGE.Game.Ui.SetTextRenderId(1);
			}
		}
	}
}