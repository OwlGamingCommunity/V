using System;

public static class Halloween
{
	static RAGE.Elements.MapObject m_MainGravestone = null;

	static RAGE.Elements.MapObject m_Coffin1 = null;
	static RAGE.Elements.MapObject m_Body1 = null;
	static RAGE.Elements.MapObject m_Coffin2 = null;
	static RAGE.Elements.MapObject m_Body2 = null;
	static RAGE.Elements.MapObject m_Coffin3 = null;
	static RAGE.Elements.MapObject m_Body3 = null;
	static RAGE.Elements.MapObject m_Pumpkin1 = null;
	static RAGE.Elements.MapObject m_Pumpkin2 = null;


	static WeakReference<CWorldPed> m_HalloweenPed = new WeakReference<CWorldPed>(null);
	static WeakReference<CWorldPed> m_HalloweenPed2 = new WeakReference<CWorldPed>(null);
	static WeakReference<CWorldPed> m_HalloweenPed3 = new WeakReference<CWorldPed>(null);
	static bool m_bWasStreamedIn = false;
	static RAGE.Elements.Blip m_Blip = null;
	private static WeakReference<AudioInstance> m_musicInst = new WeakReference<AudioInstance>(null);

	public static void Init()
	{
		if (WorldHelper.IsHalloween())
		{
			uint hash = HashHelper.GetHashUnsigned("test_prop_gravetomb_02a");
			AsyncModelLoader.RequestSyncInstantLoad(hash);
			m_MainGravestone = new RAGE.Elements.MapObject(hash, new RAGE.Vector3(160.4981f, -982.6807f, 29.09193f), new RAGE.Vector3(0.0f, 0.0f, 335.0f));

			RageEvents.RAGE_OnRender += Update;

			RageEvents.RAGE_OnTick_OncePerSecond += () =>
			{
				m_MainGravestone.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Coffin1.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Body1.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Coffin2.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Body2.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Coffin3.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Body3.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Pumpkin1.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_Pumpkin2.Dimension = RAGE.Elements.Player.LocalPlayer.Dimension;
				m_HalloweenPed.Instance().UpdateDimension(RAGE.Elements.Player.LocalPlayer.Dimension);
				m_HalloweenPed2.Instance().UpdateDimension(RAGE.Elements.Player.LocalPlayer.Dimension);
				m_HalloweenPed3.Instance().UpdateDimension(RAGE.Elements.Player.LocalPlayer.Dimension);
			};

			m_HalloweenPed2 = WorldPedManager.CreatePed(EWorldPedType.Halloween, 0xAC4B4506, new RAGE.Vector3(156.7232f, -987.6946f, 30.09193f), 161.0186f, 0);
			m_HalloweenPed = WorldPedManager.CreatePed(EWorldPedType.Halloween, 0x705E61F2, new RAGE.Vector3(158.3252f, -988.4136f, 30.09193f), 161.0186f, 0);
			m_HalloweenPed.Instance().AddWorldInteraction(EScriptControlID.Interact, "Trick or Treat", null, TrickOrTreat, false, false);
			m_HalloweenPed3 = WorldPedManager.CreatePed(EWorldPedType.Halloween, 0xAC4B4506, new RAGE.Vector3(159.8452f, -988.7723f, 30.09193f), 161.0186f, 0);

			// bodies and blood
			uint hash_coffin = HashHelper.GetHashUnsigned("prop_coffin_02b");
			AsyncModelLoader.RequestSyncInstantLoad(hash_coffin);
			uint hash_body = HashHelper.GetHashUnsigned("prop_water_corpse_02");
			AsyncModelLoader.RequestSyncInstantLoad(hash_body);

			m_Coffin1 = new RAGE.Elements.MapObject(hash_coffin, new RAGE.Vector3(158.34666f, -985.69086f, 29.35f), new RAGE.Vector3(180.0f, 180.0f, 160.0f));
			m_Body1 = new RAGE.Elements.MapObject(hash_body, new RAGE.Vector3(157.34666f, -985.69086f, 29.15f), new RAGE.Vector3(270.0f, 90.0f, 70.0f));

			m_Coffin2 = new RAGE.Elements.MapObject(hash_coffin, new RAGE.Vector3(159.99222f, -986.4375f, 29.35f), new RAGE.Vector3(180.0f, 180.0f, 160.0f));
			m_Body2 = new RAGE.Elements.MapObject(hash_body, new RAGE.Vector3(158.99222f, -986.4375f, 29.15f), new RAGE.Vector3(270.0f, 90.0f, 70.0f));

			m_Coffin3 = new RAGE.Elements.MapObject(hash_coffin, new RAGE.Vector3(161.39459f, -986.89374f, 29.35f), new RAGE.Vector3(180.0f, 180.0f, 160.0f));
			m_Body3 = new RAGE.Elements.MapObject(hash_body, new RAGE.Vector3(160.39459f, -986.89374f, 29.15f), new RAGE.Vector3(270.0f, 90.0f, 70.0f));

			// pumpkins
			hash = HashHelper.GetHashUnsigned("prop_veg_crop_03_pump");
			AsyncModelLoader.RequestSyncInstantLoad(hash);
			m_Pumpkin1 = new RAGE.Elements.MapObject(hash, new RAGE.Vector3(161.14786f, -989.39233f, 29.1f), new RAGE.Vector3(0.0f, 0.0f, 160.0f));
			m_Pumpkin2 = new RAGE.Elements.MapObject(hash, new RAGE.Vector3(155.3613f, -987.1042f, 29.1f), new RAGE.Vector3(0.0f, 0.0f, 160.0f));

			m_Blip = new RAGE.Elements.Blip(630, m_HalloweenPed.Instance().Position, "Graveyard");

			RAGE.Game.Misc.SetOverrideWeather("HALLOWEEN");
		}
	}


	private static void TrickOrTreat()
	{
		NetworkEventSender.SendNetworkEvent_HalloweenInteraction();
	}

	private static void Update()
	{
		// check for stream in
		bool bNowStreamedIn = m_HalloweenPed.Instance().IsStreamedIn();
		if (m_bWasStreamedIn != bNowStreamedIn)
		{
			// re apply clothing
			if (bNowStreamedIn)
			{
				var pedInstance = m_HalloweenPed.Instance().PedInstance;

				if (pedInstance != null)
				{
					pedInstance.SetComponentVariation((int)ECustomClothingComponent.Masks, 92, 0, 0);
					pedInstance.SetComponentVariation((int)ECustomClothingComponent.Torsos, 169, 0, 0);
					pedInstance.SetComponentVariation((int)ECustomClothingComponent.Tops, 277, 0, 0);
					pedInstance.SetComponentVariation((int)ECustomClothingComponent.Legs, 108, 0, 0);
					pedInstance.SetComponentVariation((int)ECustomClothingComponent.Undershirts, 15, 0, 0);
					pedInstance.SetComponentVariation((int)ECustomClothingComponent.Shoes, 13, 0, 0);

					m_bWasStreamedIn = bNowStreamedIn;

					m_musicInst = AudioManager.PlayAudio(EAudioIDs.Halloween, true, true);
				}
			}
			else
			{
				m_bWasStreamedIn = bNowStreamedIn;

				AudioManager.FadeOutAudio(m_musicInst);
			}
		}

		// world hints
		if (bNowStreamedIn)
		{
			bool bHasAnim = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.HAS_ANIM);

			if (!bHasAnim)
			{
				WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.E, "Rest", null, () =>
				{
					RestInCoffin(m_Coffin1, false);
				}, m_Coffin1.Position, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 2.0f, null, true);

				WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.E, "Rest In Pieces", null, () =>
				{
					RestInCoffin(m_Coffin2, true);
				}, m_Coffin2.Position, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 2.0f, null, true);

				WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.E, "Rest", null, () =>
				{
					RestInCoffin(m_Coffin3, false);
				}, m_Coffin3.Position, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 2.0f, null, true);
			}
		}
	}

	private static void RestInCoffin(RAGE.Elements.MapObject coffinObject, bool bInPieces)
	{
		RAGE.Elements.Player.LocalPlayer.Position = coffinObject.Position.Add(new RAGE.Vector3(0.0f, 0.0f, 0.5f));
		RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, coffinObject.GetRotation(0).Z - 180.0f, 0, true);

		NetworkEventSender.SendNetworkEvent_HalloweenCoffin(bInPieces);

		if (bInPieces)
		{
			ClientTimerPool.CreateTimer((object[] parameters) =>
			{
				RAGE.Elements.Player.LocalPlayer.ExplodeHead((uint)WeaponHash.Snowball);
			}, 3000, 1);
		}
	}
}

