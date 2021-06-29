internal class CGUIFactionManagement : CEFCore
{
	public CGUIFactionManagement(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/faction.html", EGUIID.FactionManagement, callbackOnLoad)
	{
		UIEvents.SetMemberRank += (int factionIndex, int memberIndex, int rankIndex) => { FactionSystem.GetFactionManagement()?.OnSetMemberRank(factionIndex, memberIndex, rankIndex); };
		UIEvents.ToggleFactionManager += (int factionIndex, int memberIndex) => { FactionSystem.GetFactionManagement()?.OnToggleFactionManager(factionIndex, memberIndex); };
		UIEvents.KickFactionMember += (int factionIndex, int memberIndex) => { FactionSystem.GetFactionManagement()?.OnKickFactionMember(factionIndex, memberIndex); };
		UIEvents.InviteFactionPlayer += (int factionIndex, string strPlayerName) => { FactionSystem.GetFactionManagement()?.OnInviteFactionPlayer(factionIndex, strPlayerName); };
		UIEvents.EditFactionMessage += (int factionIndex, string strMessage) => { FactionSystem.GetFactionManagement()?.OnEditFactionMessage(factionIndex, strMessage); };
		UIEvents.SaveRanksAndSalaries += (int factionIndex, string strJsonData) => { FactionSystem.GetFactionManagement()?.OnSaveRanksAndSalaries(factionIndex, strJsonData); };
		UIEvents.DisbandFaction += (int factionIndex) => { FactionSystem.GetFactionManagement()?.OnDisbandFaction(factionIndex); };
		UIEvents.ViewFactionVehicles += (int factionIndex) => { FactionSystem.GetFactionManagement()?.OnViewFactionVehicles(factionIndex); };
		UIEvents.RespawnFactionVehicles += (int factionIndex) => { FactionSystem.GetFactionManagement()?.OnRespawnFactionVehicles(factionIndex); };
		UIEvents.LeaveFaction += (int factionIndex) => { FactionSystem.GetFactionManagement()?.OnLeaveFaction(factionIndex); };
		UIEvents.HideCreateFaction += () => { FactionSystem.GetFactionManagement()?.HideFactionUI(); };
		UIEvents.CreateFaction += (string strFullName, string strShortName, string strFactionType) => { FactionSystem.GetFactionManagement()?.OnCreateFaction(strFullName, strShortName, strFactionType); };
	}

	public override void OnLoad()
	{

	}

	public void GotoCreateFaction()
	{
		Execute("GotoCreateFaction");
	}

	public void NoFactions()
	{
		Execute("NoFactions");
	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void ShowFactionCreationError(ECreateFactionResult result)
	{
		Execute("ShowFactionCreationError", (int)result);
	}

	public void AddFaction(string strShortName, string strFullName, float fMoney, string strMOTD, bool bIsManager)
	{
		Execute("AddFaction", strShortName, strFullName, fMoney, strMOTD, bIsManager);
	}

	public void AddFactionTag(int factionIndex, string strTag)
	{
		Execute("AddFactionTag", factionIndex, strTag);
	}

	public void AddFactionRank(int factionIndex, string strRankName, float fRankSalary)
	{
		Execute("AddFactionRank", factionIndex, strRankName, fRankSalary);
	}

	public void AddFactionMember(int factionIndex, string strName, int rankIndex, bool bIsOnline, bool bIsManager, string sLastSeen)
	{
		Execute("AddFactionMember", factionIndex, strName, rankIndex, bIsOnline, bIsManager, sLastSeen);
	}

	public void SwitchFaction(int factionIndex)
	{
		Execute("SwitchFaction", factionIndex);
	}
}