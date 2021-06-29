internal class CGUIRadioManagement : CEFCore
{
	public CGUIRadioManagement(OnGUILoadedDelegate callbackOnLoad) : base("owl_radios.client/radios.html", EGUIID.RadioManagement, callbackOnLoad)
	{
		UIEvents.CloseRadioManagement += () => { RadioSystem.GetRadioManagement()?.HideRadioManagement(); };
		UIEvents.PurchaseRadio += () => { RadioSystem.GetRadioManagement()?.OnPurchaseRadio(); };

		UIEvents.EditRadio += (long a_RadioID) => { RadioSystem.GetRadioManagement()?.OnEditRadio(a_RadioID); };
		UIEvents.ExtendRadio7Days += (int a_RadioID) => { RadioSystem.GetRadioManagement()?.OnExtendRadio7Days(a_RadioID); };
		UIEvents.ExtendRadio30Days += (int a_RadioID) => { RadioSystem.GetRadioManagement()?.OnExtendRadio30Days(a_RadioID); };

		UIEvents.SaveRadio += (string strName, string strEndpoint) => { RadioSystem.GetRadioManagement()?.OnSaveRadio(strName, strEndpoint); };
		UIEvents.CancelEditRadio += () => { RadioSystem.GetRadioManagement()?.OnCancelEditRadio(); };
	}

	public override void OnLoad()
	{

	}

	public void AddRadio(long ID, string strName, string strExpiration)
	{
		Execute("AddRadio", ID, strName, strExpiration);
	}

	public void Reset_GC()
	{
		Execute("Reset_GC");
	}

	public void Reset_Radios()
	{
		Execute("Reset_Radios");
	}

	public void CommitRadios()
	{
		Execute("CommitRadios");
	}

	public void SetGCBalance(int gcBalance)
	{
		Execute("SetGCBalance", gcBalance);
	}

	public void GotoEditRadio(string strName, string strEndpoint)
	{
		Execute("GotoEditRadio", strName, strEndpoint);
	}

	public void GotoRadioList()
	{
		Execute("GotoRadioList");
	}
}