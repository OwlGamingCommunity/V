internal class CGUIFactionInvite : CEFCore
{
	public CGUIFactionInvite(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/receivefactioninvite.html", EGUIID.FactionInvite, callbackOnLoad)
	{
		UIEvents.FactionInvite_Accept += () => { FactionSystem.GetFactionInvites()?.SendFactionInviteDecision(true); };
		UIEvents.FactionInvite_Decline += () => { FactionSystem.GetFactionInvites()?.SendFactionInviteDecision(false); };
	}

	public override void OnLoad()
	{

	}

	public void ShowInvite(string strFactionName, string strFromName)
	{
		Execute("ShowInvite", strFactionName, strFromName);
	}
}