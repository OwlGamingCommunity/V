using System;
public static class Christmas
{
	static RAGE.Elements.MapObject m_Tree = null;
	static WeakReference<CWorldPed> m_Santa = new WeakReference<CWorldPed>(null);
	static bool m_bWasStreamedIn = false;
	static RAGE.Elements.Blip m_Blip = null;
	private static WeakReference<AudioInstance> m_musicInst = new WeakReference<AudioInstance>(null);

	public static void Init()
	{
		if (WorldHelper.IsChristmas())
		{
			uint hash = HashHelper.GetHashUnsigned("prop_xmas_ext");
			AsyncModelLoader.RequestSyncInstantLoad(hash);
			m_Tree = new RAGE.Elements.MapObject(hash, new RAGE.Vector3(160.4981f, -982.6807f, 29.09193f), new RAGE.Vector3(0.0f, 0.0f, 270.0f));

			RageEvents.RAGE_OnRender += Update;

			RageEvents.RAGE_OnTick_OncePerSecond += () =>
			{
				m_Tree.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
			};

			WorldPedManager.CreatePed(EWorldPedType.Christmas, 0xD86B5A95, new RAGE.Vector3(156.7232f, -987.6946f, 30.09193f), 161.0186f, 0);
			m_Santa = WorldPedManager.CreatePed(EWorldPedType.Christmas, 0x705E61F2, new RAGE.Vector3(158.3252f, -988.4136f, 30.09193f), 161.0186f, 0);
			m_Santa.Instance().AddWorldInteraction(EScriptControlID.Interact, "Talk to Santa", null, PlayWithSanta, false, false);
			WorldPedManager.CreatePed(EWorldPedType.Christmas, 0xD86B5A95, new RAGE.Vector3(159.8452f, -988.7723f, 30.09193f), 161.0186f, 0);

			m_Blip = new RAGE.Elements.Blip(685, m_Santa.Instance().Position, "Santa's Grotto");
		}
	}

	private static void PlayWithSanta()
	{
		NetworkEventSender.SendNetworkEvent_TalkToSanta();
	}

	private static void Update()
	{
		// check for stream in
		bool bNowStreamedIn = m_Santa.Instance().IsStreamedIn();
		if (m_bWasStreamedIn != bNowStreamedIn)
		{
			// re apply clothing
			if (bNowStreamedIn)
			{
				var pedInstance = m_Santa.Instance().PedInstance;

				if (pedInstance != null)
				{
					pedInstance.SetComponentVariation(1, 8, 0, 0); // hat

					pedInstance.SetComponentVariation(4, 19, 0, 0); // pants
					pedInstance.SetComponentVariation(6, 17, 0, 0); // shoes
					pedInstance.SetComponentVariation(3, 81, 0, 0); // torso
					pedInstance.SetComponentVariation(8, 15, 0, 0); // undershirt
					pedInstance.SetComponentVariation(11, 194, 0, 0); // top

					m_bWasStreamedIn = bNowStreamedIn;

					m_musicInst = AudioManager.PlayAudio(EAudioIDs.Christmas, true, true);
				}
			}
			else
			{
				m_bWasStreamedIn = bNowStreamedIn;

				AudioManager.FadeOutAudio(m_musicInst);
			}
		}
	}
}

