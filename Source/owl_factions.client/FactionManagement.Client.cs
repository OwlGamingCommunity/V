using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class FactionManagement
{
	private CGUIFactionManagement m_FactionManagementUI = new CGUIFactionManagement(() => { });

	private List<WeakReference<CWorldPed>> m_lstFactionCreationPeds = new List<WeakReference<CWorldPed>>();
	private bool m_bFactionUIBlocked = false;

	private EntityDatabaseID m_AdminEntityIDBeingOperatedOn = -1;

	public FactionManagement()
	{
		// Uncomment to bring back faction creation peds.
		// m_lstFactionCreationPeds.Add(WorldPedManager.CreatePed(EWorldPedType.FactionCreation, 2362341647, new RAGE.Vector3(253.6098f, 207.724f, 106.2879f), 339.7787f, 1));
		// m_lstFactionCreationPeds.Add(WorldPedManager.CreatePed(EWorldPedType.FactionCreation, 2362341647, new RAGE.Vector3(253.6098f, 207.724f, 106.2879f), 339.7787f, 1223));
		// foreach (var pedWeakRef in m_lstFactionCreationPeds)
		// {
		//     pedWeakRef.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Create Faction", null, InteractWithCreateFactionWorldPed, false, false, 4.0f, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true);
		// }

		RageEvents.RAGE_OnRender += OnRender;

		NetworkEvents.ChangeCharacterApproved += OnCharacterChanged;
		NetworkEvents.FactionTransactionComplete += OnFactionTransactionComplete;
		NetworkEvents.FactionCreateResult += OnFactionCreateResult;
		NetworkEvents.RequestFactionInfo_Response += OnRequestFactionInfo_Response;
		NetworkEvents.Faction_ViewFactionVehicles_Response += OnViewFactionVehicles_Response;
		NetworkEvents.Faction_AdminViewFactions += OnAdminViewFactions;

		UIEvents.OnFactionLeave_Cancel += OnFactionLeave_Cancel;
		UIEvents.OnFactionLeave_Confirm += OnFactionLeave_Confirm;
		UIEvents.OnFactionDisband_Cancel += OnFactionDisband_Cancel;
		UIEvents.OnFactionDisband_Confirm += OnFactionDisband_Confirm;

		UIEvents.FactionVehicleList_Hide += OnFactionVehicleList_Hide;

		// admin faction list
		UIEvents.AdminViewFactions_JoinFaction += OnAdminViewFactions_JoinFaction;
		UIEvents.AdminViewFactions_DeleteFaction += OnAdminViewFactions_DeleteFaction;
		UIEvents.AdminViewFaction_Join_Yes += OnAdminViewFaction_Join_Yes;
		UIEvents.AdminViewFaction_Join_No += OnAdminViewFaction_Join_No;
		UIEvents.AdminViewFaction_Delete_Yes += OnAdminViewFaction_Delete_Yes;
		UIEvents.AdminViewFaction_Delete_No += OnAdminViewFaction_Delete_No;

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleFactionUI, ToggleFactionUI);
	}

	~FactionManagement()
	{

	}

	private void OnAdminViewFaction_Join_Yes()
	{
		// tell server to delete it (server will re-send faction list)
		NetworkEventSender.SendNetworkEvent_Faction_AdminViewFactions_JoinFaction(m_AdminEntityIDBeingOperatedOn);

		m_AdminEntityIDBeingOperatedOn = -1;
	}

	private void OnAdminViewFaction_Join_No()
	{
		// re-request list, nothing to do
		NetworkEventSender.SendNetworkEvent_Faction_AdminRequestViewFactions();
		m_AdminEntityIDBeingOperatedOn = -1;
	}

	private void OnAdminViewFaction_Delete_Yes()
	{
		// tell server to delete it (server will re-send faction list)
		NetworkEventSender.SendNetworkEvent_Faction_AdminViewFactions_DeleteFaction(m_AdminEntityIDBeingOperatedOn);

		m_AdminEntityIDBeingOperatedOn = -1;
	}

	private void OnAdminViewFaction_Delete_No()
	{
		// re-request list, nothing to do
		NetworkEventSender.SendNetworkEvent_Faction_AdminRequestViewFactions();
		m_AdminEntityIDBeingOperatedOn = -1;
	}

	private void OnAdminViewFactions_JoinFaction(EntityDatabaseID FactionID)
	{
		GenericDataTable.OnClose();
		m_AdminEntityIDBeingOperatedOn = FactionID;
		GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("Are you SURE you want to join faction ID #{0}?", FactionID), "YES, JOIN IT", "No!", UIEventID.AdminViewFaction_Join_Yes, UIEventID.AdminViewFaction_Join_No);
	}

	private void OnAdminViewFactions_DeleteFaction(EntityDatabaseID FactionID)
	{
		GenericDataTable.OnClose();
		m_AdminEntityIDBeingOperatedOn = FactionID;
		GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("Are you SURE you want to delete faction ID #{0}?", FactionID), "YES, DELETE IT", "No! Keep it", UIEventID.AdminViewFaction_Delete_Yes, UIEventID.AdminViewFaction_Delete_No);
	}

	private void OnAdminViewFactions(List<CFactionListTransmit> lstFactions)
	{
		List<string> lstHeaders = new List<string> { "ID", "Short Name", "Name", "# Members", "Money", "Creator", "Join", "Delete" };

		List<List<string>> lstData = new List<List<string>>();
		foreach (CFactionListTransmit faction in lstFactions)
		{
			string strCreatorName = faction.CreatorID == -1 ? "Unknown / No Longer Exists" : faction.strCreatorName;
			string strCreatorNameString = faction.CreatorID == -1 ? strCreatorName : Helpers.FormatString("{0} (ID: {1})", strCreatorName, faction.CreatorID);

			string strHTMLJoin = "<a href='javascript:TriggerEvent(\"AdminViewFactions_JoinFaction\", " + faction.FactionID + ");'>Join Faction</a>";
			string strHTMLDelete = "<a href='javascript:TriggerEvent(\"AdminViewFactions_DeleteFaction\", " + faction.FactionID + ");'>Delete Faction</a>";

			lstData.Add(new List<string> { faction.FactionID.ToString(), faction.ShortName, faction.Name, faction.NumMembers.ToString(), faction.Money.ToString(), strCreatorNameString, strHTMLJoin, strHTMLDelete });
		}
		GenericDataTable.ShowGenericDataTable(lstHeaders, lstData, Helpers.FormatString("Faction List ({0} factions)", lstFactions.Count), "Exit", UIEventID.FactionVehicleList_Hide);
	}

	private void OnFactionVehicleList_Hide()
	{
		ShowFactionUI();
	}

	private void InteractWithCreateFactionWorldPed()
	{
		ShowFactionUI();
		m_FactionManagementUI.GotoCreateFaction();
	}

	private void OnFactionTransactionComplete()
	{
		SetTransactionInProgress(false);
	}

	private void ToggleFactionUI(EControlActionType actionType)
	{
		if (m_FactionManagementUI.IsVisible())
		{
			NetworkEventSender.SendNetworkEvent_ExitFactionMenu();
			SetTransactionInProgress(false);
			HideFactionUI();
		}
		else if (KeyBinds.CanProcessKeybinds()) // We can hide always, but can only show when eligible
		{
			SetTransactionInProgress(true);
			NetworkEventSender.SendNetworkEvent_Faction_RequestFactionInfo();
		}
	}

	private void SetTransactionInProgress(bool bInProgress, bool bUpdateState = true)
	{
		if (bUpdateState)
		{
			if (bInProgress)
			{
				HideFactionUI();
			}
			else
			{
				ShowFactionUI();
			}
		}

		m_bFactionUIBlocked = bInProgress;
	}

	private void OnRender()
	{
		if (m_bFactionUIBlocked)
		{
			HUD.SetLoadingMessage("Updating Factions");
		}
	}

	private void OnCharacterChanged()
	{
		HideFactionUI();
		SetTransactionInProgress(false, false);
	}

	private void ShowFactionUI()
	{
		m_FactionManagementUI.SetVisible(true, true, false);
	}

	public void HideFactionUI()
	{
		m_FactionManagementUI.SetVisible(false, false, false);
	}

	public void OnSetMemberRank(int factionIndex, int memberIndex, int rankIndex)
	{
		SetTransactionInProgress(true);
		NetworkEventSender.SendNetworkEvent_Faction_SetMemberRank(factionIndex, memberIndex, rankIndex);
	}

	public void OnToggleFactionManager(int factionIndex, int memberIndex)
	{
		SetTransactionInProgress(true);
		NetworkEventSender.SendNetworkEvent_Faction_ToggleManager(factionIndex, memberIndex);
	}

	public void OnKickFactionMember(int factionIndex, int memberIndex)
	{
		SetTransactionInProgress(true);
		NetworkEventSender.SendNetworkEvent_Faction_Kick(factionIndex, memberIndex);
	}

	public void OnInviteFactionPlayer(int factionIndex, string strPlayerName)
	{
		// NOTE: Don't have to set faction transaction here because this doesnt modify the faction directly
		NetworkEventSender.SendNetworkEvent_Faction_InvitePlayer(factionIndex, strPlayerName);
	}

	public void OnEditFactionMessage(int factionIndex, string strMessage)
	{
		SetTransactionInProgress(true);
		NetworkEventSender.SendNetworkEvent_Faction_EditMessage(factionIndex, strMessage);
	}

	public void OnSaveRanksAndSalaries(int factionIndex, string strJsonData)
	{
		SetTransactionInProgress(true);
		NetworkEventSender.SendNetworkEvent_Faction_SaveRanksAndSalaries(factionIndex, strJsonData);
	}

	private int m_PendingFactionAction_Index = -1;
	private void OnFactionDisband_Confirm()
	{
		SetTransactionInProgress(true);
		NetworkEventSender.SendNetworkEvent_Faction_DisbandFaction(m_PendingFactionAction_Index);
	}

	private void OnFactionDisband_Cancel()
	{
		m_PendingFactionAction_Index = -1;
		ShowFactionUI();
	}

	private void OnFactionLeave_Confirm()
	{
		SetTransactionInProgress(true);
		NetworkEventSender.SendNetworkEvent_Faction_LeaveFaction(m_PendingFactionAction_Index);
	}

	private void OnFactionLeave_Cancel()
	{
		m_PendingFactionAction_Index = -1;
		ShowFactionUI();
	}

	public void OnDisbandFaction(int factionIndex)
	{
		m_PendingFactionAction_Index = factionIndex;
		HideFactionUI();
		GenericPromptHelper.ShowPrompt("Confirmation", "Are you sure you want to disband the faction?", "Yes, Disband it", "No, Keep it", UIEventID.OnFactionDisband_Confirm, UIEventID.OnFactionDisband_Cancel);
	}

	public void OnRespawnFactionVehicles(int factionIndex)
	{
		NetworkEventSender.SendNetworkEvent_Faction_RespawnFactionVehicles(factionIndex);
	}

	public void OnViewFactionVehicles(int factionIndex)
	{
		NetworkEventSender.SendNetworkEvent_Faction_ViewFactionVehicles(factionIndex);
	}

	public void OnLeaveFaction(int factionIndex)
	{
		m_PendingFactionAction_Index = factionIndex;
		HideFactionUI();
		GenericPromptHelper.ShowPrompt("Confirmation", "Are you sure you want to leave the faction?", "Yes, Leave it", "No, Stay in it", UIEventID.OnFactionLeave_Confirm, UIEventID.OnFactionLeave_Cancel);
	}

	public void OnCreateFaction(string strFullName, string strShortName, string strFactionType)
	{
		SetTransactionInProgress(true);
		HideFactionUI();
		NetworkEventSender.SendNetworkEvent_Faction_CreateFaction(strFullName, strShortName, strFactionType);
	}

	private void OnFactionCreateResult(ECreateFactionResult result)
	{
		SetTransactionInProgress(false);

		if (result == ECreateFactionResult.Success)
		{
			HideFactionUI();
		}
		else
		{
			ShowFactionUI();
			m_FactionManagementUI.GotoCreateFaction();
			m_FactionManagementUI.ShowFactionCreationError(result);
		}
	}

	private void OnViewFactionVehicles_Response(List<CFactionVehicleInfoTransmit> lstFactionVehicles)
	{
		HideFactionUI();

		List<string> lstHeaders = new List<string> { "ID", "Name", "Plate", "Location" };
		List<List<string>> lstData = new List<List<string>>();

		foreach (CFactionVehicleInfoTransmit vehicle in lstFactionVehicles)
		{
			EntityDatabaseID vehicleID = vehicle.ID;

			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicle.Hash);
			string strName = vehicleDef != null ? Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name) : "Unknown";

			string strPlate = vehicle.Plate;

			// work out location
			int streetNameID = -1;
			int crossroadNameID = -1;
			string strStreetName = String.Empty;
			string strCrossroadName = String.Empty;
			RAGE.Game.Pathfind.GetStreetNameAtCoord(vehicle.fX, vehicle.fY, vehicle.fZ, ref streetNameID, ref crossroadNameID);

			if (streetNameID > 0)
			{
				strStreetName = RAGE.Game.Ui.GetStreetNameFromHashKey((uint)streetNameID);
			}

			if (crossroadNameID > 0)
			{
				strCrossroadName = RAGE.Game.Ui.GetStreetNameFromHashKey((uint)crossroadNameID);
			}

			string strZoneName = RAGE.Game.Zone.GetNameOfZone(vehicle.fX, vehicle.fY, vehicle.fZ);
			string strRealZoneName = ZoneNameHelper.ZoneNames.ContainsKey(strZoneName) ? ZoneNameHelper.ZoneNames[strZoneName] : "San Andreas"; // TODO_CSHARP: Helper func instead
			string strLocationString = "Unknown";

			if (strCrossroadName == String.Empty)
			{
				if (strStreetName.Length > 0 && strRealZoneName.Length > 0)
				{
					strLocationString = Helpers.FormatString("{0}, {1}", strStreetName, strRealZoneName);
				}
				else if (strStreetName.Length > 0)
				{
					strLocationString = strStreetName;
				}
				else if (strRealZoneName.Length > 0)
				{
					strLocationString = strRealZoneName;
				}
			}
			else
			{
				if (strStreetName.Length > 0 && strCrossroadName.Length > 0 && strRealZoneName.Length > 0)
				{
					strLocationString = Helpers.FormatString("{0} & {1}, {2}", strStreetName, strCrossroadName, strRealZoneName);
				}
				else if (strStreetName.Length > 0)
				{
					strLocationString = strStreetName;
				}
				else if (strCrossroadName.Length > 0)
				{
					strLocationString = strCrossroadName;
				}
				else if (strRealZoneName.Length > 0)
				{
					strLocationString = strRealZoneName;
				}
			}

			lstData.Add(new List<string> { vehicleID.ToString(), strName, strPlate, strLocationString });
		}

		GenericDataTable.ShowGenericDataTable(lstHeaders, lstData, Helpers.FormatString("Faction Vehicle List ({0} vehicles)", lstFactionVehicles.Count), "Exit", UIEventID.FactionVehicleList_Hide);
	}

	private void OnRequestFactionInfo_Response(List<CFactionTransmit> lstFactions)
	{
		SetTransactionInProgress(false);
		m_FactionManagementUI.Reset();
		ShowFactionUI();

		if (lstFactions.Count == 0)
		{
			m_FactionManagementUI.NoFactions();
			return;
		}

		int factionIndex = 0;
		foreach (CFactionTransmit faction in lstFactions)
		{
			// base
			m_FactionManagementUI.AddFaction(faction.ShortName, faction.Name, faction.Money, faction.MOTD, faction.IsManager);

			// Faction ID (for admin reports)
			m_FactionManagementUI.AddFactionTag(factionIndex, Helpers.FormatString("ID: {0}", faction.FactionID));

			// tags
			foreach (string strFactionTag in faction.Tags)
			{
				m_FactionManagementUI.AddFactionTag(factionIndex, strFactionTag);
			}

			// ranks
			foreach (CFactionRankTransmit rank in faction.Ranks)
			{
				m_FactionManagementUI.AddFactionRank(factionIndex, rank.Name, rank.Salary);
			}

			// members
			foreach (CFactionMemberTransmit member in faction.Members)
			{
				bool bIsOnline = false;
				foreach (RAGE.Elements.Player player in RAGE.Elements.Entities.Players.All)
				{
					if (player.Name == member.Name)
					{
						bIsOnline = true;
					}
				}

				m_FactionManagementUI.AddFactionMember(factionIndex, member.Name, member.Rank, bIsOnline, member.IsManager, member.LastSeen);

			}

			++factionIndex;
		}

		m_FactionManagementUI.SwitchFaction(0);
	}
}

