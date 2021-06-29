internal class CGUIPDTicket : CEFCore
{
	public CGUIPDTicket(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/ticket.html", EGUIID.PDTicket, callbackOnLoad)
	{
		UIEvents.AcceptPDTicket += () => { FactionSystem.GetPDSystem()?.OnAcceptPDTicket(); };
		UIEvents.DeclinePDTicket += () => { FactionSystem.GetPDSystem()?.OnDeclinePDTicket(); };
	}

	public override void OnLoad()
	{

	}

	public void ShowTicket(string strOfficerName, float fAmount, string strReason)
	{
		Execute("ShowTicket", strOfficerName, fAmount, strReason);
	}
}