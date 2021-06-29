using RAGE;
using System;

public class FourthOfJuly
{
	private static WeakReference<AudioInstance> m_musicInst = new WeakReference<AudioInstance>(null);

	private const string CONFETTI_DICTIONARY = "scr_xs_celebration";
	private const string CONFETTI_BURST = "scr_xs_confetti_burst";

	private const string FIREWORK_DICTIONARY = "scr_indep_fireworks";
	private const string FIREWORK_TRAILBURST = "scr_indep_firework_trailburst";
	private const string FIREWORK_FOUNTAIN = "scr_indep_firework_fountain";
	private const string FIREWORK_SHOTBURST = "scr_indep_firework_shotburst";
	private const string FIREWORK_STARBURST = "scr_indep_firework_starburst";

	// Locations
	private Vector3 Firework_1 = new Vector3(-1856.464f, -1289.215f, 5.0f);
	private Vector3 Firework_2 = new Vector3(-1859.999f, -1285.479f, 5.0f);
	private Vector3 Firework_3 = new Vector3(-1868.457f, -1278.499f, 5.0f);
	private Vector3 Firework_4 = new Vector3(-1880.634f, -1268.149f, 5.0f);
	private Vector3 Firework_5 = new Vector3(-1889.869f, -1260.294f, 5.0f);
	private Vector3 Firework_6 = new Vector3(-1877.767f, -1345.594f, 2.0f);
	private Vector3 Firework_7 = new Vector3(-1910.284f, -1311.653f, 2.0f);
	private Vector3 Firework_8 = new Vector3(-1930.89f, -1289.02f, 2.0f);


	private Vector3 Confetti_1 = new Vector3(-1865.681f, -1237.594f, 8.615784f);
	private Vector3 Confetti_2 = new Vector3(-1826.983f, -1270.005f, 8.615784f);

	private RAGE.Elements.MapObject m_ObjUSFlag_1 = null;
	private RAGE.Elements.MapObject m_ObjUSFlag_2 = null;
	private RAGE.Elements.MapObject m_ObjUSFlag_3 = null;
	private RAGE.Elements.MapObject m_ObjUSFlag_4 = null;

	public FourthOfJuly()
	{
		NetworkEvents.StartFourthOfJuly += StartFourth;
		NetworkEvents.StartFourthOfJulyFireworksOnly += StartFourthFireworksOnly;

		// We have to request this in the constructor so it requests when someone joins. Otherwise it will never work... :(
		RAGE.Game.Streaming.RequestNamedPtfxAsset(FIREWORK_DICTIONARY);
		RAGE.Game.Streaming.RequestNamedPtfxAsset(CONFETTI_DICTIONARY);

		uint hash = HashHelper.GetHashUnsigned("prop_flagpole_2b");
		AsyncModelLoader.RequestSyncInstantLoad(hash);
		m_ObjUSFlag_1 = new RAGE.Elements.MapObject(hash, new Vector3Definition(-1850.531f, -1237.562f, 15.41716f), new Vector3Definition(0f, 0f, 0f));
		m_ObjUSFlag_2 = new RAGE.Elements.MapObject(hash, new Vector3Definition(-1860.54f, -1229.326f, 15.41716f), new Vector3Definition(0f, 0f, 0f));
		m_ObjUSFlag_3 = new RAGE.Elements.MapObject(hash, new Vector3Definition(-1875.472f, -1216.703f, 15.41716f), new Vector3Definition(0f, 0f, 0f));
		m_ObjUSFlag_4 = new RAGE.Elements.MapObject(hash, new Vector3Definition(-1835.095f, -1250.553f, 15.41716f), new Vector3Definition(0f, 0f, 0f));

	}

	private void StartFourth()
	{
		StartCountdown();
	}

	private void StartCountdown()
	{
		m_musicInst = AudioManager.PlayAudio(EAudioIDs.FourthOfJulyCountdown, true, false, HandleFireworkStages);
	}

	private void HandleFireworkStages(WeakReference<AudioInstance> musicInstance)
	{
		DoFireworkLaunching();
	}

	private void DoConfetti()
	{
		LaunchFirework(Confetti_1, CONFETTI_DICTIONARY, CONFETTI_BURST, 8.0f);
		LaunchFirework(Confetti_2, CONFETTI_DICTIONARY, CONFETTI_BURST, 8.0f);
	}

	private void StartFourthFireworksOnly()
	{
		DoFireworkLaunching();
	}

	private void DoFireworkLaunching()
	{
		LaunchFirework(Firework_1, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST);
		LaunchFirework(Firework_2, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST);
		LaunchFirework(Firework_3, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST);
		LaunchFirework(Firework_4, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST);
		LaunchFirework(Firework_5, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST);

		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			LaunchFirework(Firework_6, FIREWORK_DICTIONARY, FIREWORK_FOUNTAIN, 6.0f);
			LaunchFirework(Firework_7, FIREWORK_DICTIONARY, FIREWORK_FOUNTAIN, 6.0f);
			LaunchFirework(Firework_8, FIREWORK_DICTIONARY, FIREWORK_FOUNTAIN, 6.0f);
		}, 2750, 10);

		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			LaunchFirework(Firework_1, FIREWORK_DICTIONARY, FIREWORK_STARBURST);
			LaunchFirework(Firework_2, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST);
			LaunchFirework(Firework_3, FIREWORK_DICTIONARY, FIREWORK_STARBURST);
			LaunchFirework(Firework_4, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST);
			LaunchFirework(Firework_5, FIREWORK_DICTIONARY, FIREWORK_STARBURST);
		}, 5000, 4);

		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			LaunchFirework(Firework_1, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST, 4.0f);
			LaunchFirework(Firework_2, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST, 4.0f);
			LaunchFirework(Firework_3, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST, 4.0f);
			LaunchFirework(Firework_4, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST, 4.0f);
			LaunchFirework(Firework_5, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST, 4.0f);
		}, 10000, 6);

		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			LaunchFirework(Firework_1, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST, 4.0f);
			LaunchFirework(Firework_2, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST, 4.0f);
			LaunchFirework(Firework_3, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST, 4.0f);
			LaunchFirework(Firework_4, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST, 4.0f);
			LaunchFirework(Firework_5, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST, 4.0f);
		}, 7500, 7);

		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			DoConfetti();
		}, 1500, 45);

		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			LaunchFirework(Firework_6, FIREWORK_DICTIONARY, FIREWORK_SHOTBURST, 10.0f);
			LaunchFirework(Firework_7, FIREWORK_DICTIONARY, FIREWORK_FOUNTAIN, 10.0f);
			LaunchFirework(Firework_8, FIREWORK_DICTIONARY, FIREWORK_TRAILBURST, 10.0f);
			NetworkEventSender.SendNetworkEvent_EndFourthOfJulyEvent();
		}, 67500, 1);
	}

	private void LaunchFirework(Vector3 vecPosFirework, string DictToUse, string fireworkType, float fScale = 2.0f)
	{
		if (!DictToUse.Equals(CONFETTI_DICTIONARY))
		{
			RAGE.Game.Graphics.UseParticleFxAssetNextCall(FIREWORK_DICTIONARY);
			RAGE.Game.Graphics.StartParticleFxNonLoopedAtCoord(fireworkType, vecPosFirework.X, vecPosFirework.Y, vecPosFirework.Z, 0.0f, 0.0f, 0.0f, fScale, false, false, false);
		}
		else
		{
			RAGE.Game.Graphics.UseParticleFxAssetNextCall(CONFETTI_DICTIONARY);
			RAGE.Game.Graphics.StartParticleFxNonLoopedAtCoord(fireworkType, vecPosFirework.X, vecPosFirework.Y, vecPosFirework.Z, 0.0f, 0.0f, 0.0f, fScale, false, false, false);
		}
	}
}
