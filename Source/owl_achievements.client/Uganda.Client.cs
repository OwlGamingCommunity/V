using System;

public class Uganda
{
	public Uganda()
	{
		RageEvents.RAGE_OnRender += OnRender;

		// Tombstomb
		uint hash = HashHelper.GetHashUnsigned("prop_gravestones_01a");
		AsyncModelLoader.RequestSyncInstantLoad(hash);
		ugandaObject = new RAGE.Elements.MapObject(hash, new Vector3Definition(822.0036f, 6668.602f, 1.5248496f), new Vector3Definition(0f, 0f, 0f));

		// Camera
		CameraManager.RegisterCamera(ECameraID.UGANDA, ugandaCameraPos, ugandaPos);
	}

	private void OnRespectsPaid(object[] a_Parameters)
	{
		m_bIsInUganda = false;

		NetworkEventSender.SendNetworkEvent_UgandaStop();

		CameraManager.DeactivateCamera(ECameraID.UGANDA);
	}

	private void GotoCloseupCam(object[] a_Parameters)
	{
		CameraManager.UpdateCamera(ECameraID.UGANDA, ugandaCameraPosCloseUp, ugandaPos, new Vector3Definition(0f, 0f, 0f));
	}

	private void OnRender()
	{
		WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.F, "Pay Respects", null, PayRespects, ugandaPos, Constants.DefaultWorldDimension, false, false, PLAYER_DISTANCE_UGANDA);
	}

	private void PayRespects()
	{
		// Check we aren't already doing Uganada-ness
		if (!m_bIsInUganda)
		{
			m_bIsInUganda = true;

			AudioManager.PlayAudio(EAudioIDs.Uganda, true, true);

			RAGE.Elements.Player.LocalPlayer.Position = ugandaStandPos;
			NetworkEventSender.SendNetworkEvent_UgandaStart();

			CameraManager.ActivateCamera(ECameraID.UGANDA);

			ClientTimerPool.CreateTimer(OnRespectsPaid, 14000, a_NumIterations: 1);
			ClientTimerPool.CreateTimer(GotoCloseupCam, 7000, a_NumIterations: 1);
		}
	}

	private const float PLAYER_DISTANCE_UGANDA = 1.0f;

	private readonly Vector3Definition ugandaPos = new Vector3Definition(819.7635f, 6668.495f, 2.374744f);
	private readonly Vector3Definition ugandaStandPos = new Vector3Definition(819.7444f, 6667.109f, 2.343063f);
	private readonly Vector3Definition ugandaCameraPos = new Vector3Definition(821.6208f, 6666.007f, 3.705027f);
	private readonly Vector3Definition ugandaCameraPosCloseUp = new Vector3Definition(819.0945f, 6667.507f, 2.338255f);
	private RAGE.Elements.MapObject ugandaObject = null;
	private bool m_bIsInUganda = false;
}