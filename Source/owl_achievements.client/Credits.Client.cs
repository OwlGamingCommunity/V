public static class Credits
{
	private static RAGE.Vector3 m_vecPosCreditsComputer = new RAGE.Vector3(447.3951f, -973.9786f, 31.44458f);
	static Credits()
	{

	}

	public static void Init()
	{
		// EVENTS
		RageEvents.RAGE_OnRender += OnRender;

		CreateCreditsRenderTarget();
	}

	private static void CreateCreditsRenderTarget()
	{
		// Register render target
		if (!RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetName))
		{
			RAGE.Game.Ui.RegisterNamedRendertarget(g_RenderTargetName, false);
		}

		// Link it to all models
		if (!RAGE.Game.Ui.IsNamedRendertargetLinked(HashHelper.GetHashUnsigned(g_RenderObjectModel)))
		{
			RAGE.Game.Ui.LinkNamedRendertarget(HashHelper.GetHashUnsigned(g_RenderObjectModel));
		}

		// Get the handle
		if (RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetName))
		{
			m_RenderTargetID = RAGE.Game.Ui.GetNamedRendertargetRenderId(g_RenderTargetName);
		}
	}

	private static void OnRender()
	{
		float fDistance = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, m_vecPosCreditsComputer);
		if (fDistance < 15.0f)
		{
			// set render target + rtt layer
			RAGE.Game.Ui.SetTextRenderId(m_RenderTargetID);
			RAGE.Game.Graphics.Set2dLayer(4);

			TextHelper.Draw2D("OwlGaming V Script", 0.5f, 0.05f, 0.9f, 255, 255, 255, 255, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false, true);
			TextHelper.Draw2D("Script By:\nDaniels\nChaos\nCourtez\nJer\nYannick\n\nDesign & Data:\nThatGuy & Wright", 0.5f, 0.25f, 0.3f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false, true);

			// Reset render target
			RAGE.Game.Ui.SetTextRenderId(1);
		}
	}

	private const string g_RenderTargetName = "tvscreen";
	private const string g_RenderObjectModel = "prop_monitor_w_large";
	private static int m_RenderTargetID = -1;
}