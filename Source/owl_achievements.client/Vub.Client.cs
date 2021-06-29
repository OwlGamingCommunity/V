using RAGE.Ui;
using System;

public class Vub
{
	static WeakReference<CWorldPed> m_Vub = new WeakReference<CWorldPed>(null);
	private RAGE.Elements.MapObject m_ObjLeft = null;
	private RAGE.Elements.MapObject m_ObjMid = null;
	private RAGE.Elements.MapObject m_ObjRight = null;
	private RAGE.Elements.MapObject m_ObjSoccerLeft = null;
	private RAGE.Elements.MapObject m_ObjSoccerRight = null;

	private static RAGE.Ui.HtmlWindow m_browser = null;

	public Vub()
	{
		uint hash = HashHelper.GetHashUnsigned("apa_prop_flag_england");
		AsyncModelLoader.RequestSyncInstantLoad(hash);
		m_ObjLeft = new RAGE.Elements.MapObject(hash, new Vector3Definition(801.8226f, -248.4672f, 67.50936f), new Vector3Definition(0f, 0f, 0f));
		m_ObjMid = new RAGE.Elements.MapObject(hash, new Vector3Definition(802.976f, -246.5232f, 67.50936f), new Vector3Definition(0f, 0f, 0f));
		m_ObjRight = new RAGE.Elements.MapObject(hash, new Vector3Definition(800.6534f, -250.7496f, 67.50873f), new Vector3Definition(0f, 0f, 0f));

		hash = HashHelper.GetHashUnsigned("stt_prop_stunt_soccer_ball");
		AsyncModelLoader.RequestSyncInstantLoad(hash);
		m_ObjSoccerLeft = new RAGE.Elements.MapObject(hash, new Vector3Definition(805.0866f, -242.1096f, 66.51021f), new Vector3Definition(0f, 0f, 0f));
		m_ObjSoccerRight = new RAGE.Elements.MapObject(hash, new Vector3Definition(798.5191f, -254.8538f, 66.50954f), new Vector3Definition(0f, 0f, 0f));

		m_Vub = WorldPedManager.CreatePed(EWorldPedType.Vub, 0x573201B8, new RAGE.Vector3(800.1985f, -247.7617f, 65.31431f), 65.42212f, 0);
		m_Vub.Instance().AddWorldInteraction(EScriptControlID.Interact, "Teach Vub English", null, Teach, false, false, 2.0f, null, true);
	}

	private void Teach()
	{
		m_browser = new HtmlWindow("https://www.youtube.com/embed/pb2agMaIJ40?autoplay=1");
		ClientTimerPool.CreateTimer(OnTaught, 70000, a_NumIterations: 1);
	}

	private void OnTaught(object[] parameters)
	{
		if (m_browser != null)
		{
			m_browser.Destroy();
			m_browser = null;
		}

		NetworkEventSender.SendNetworkEvent_TaughtVub();
	}
}

