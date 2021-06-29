using RAGE.Elements;
using System.Linq;

public static class PlayerList
{
	static PlayerList()
	{

	}

	public static void Init()
	{
		ScriptControls.SubscribeToControl(EScriptControlID.ShowPlayerList, TogglePlayerList);
	}

	private static void TogglePlayerList(EControlActionType actionType)
	{
		if (actionType == EControlActionType.Pressed)
		{
			if (!m_PlayerListGUI.IsVisible())
			{
				ShowPlayerList();
			}
		}
		else if (actionType == EControlActionType.Released)
		{
			if (m_PlayerListGUI.IsVisible())
			{
				HidePlayerList();
			}
		}
	}


	// TODO_LAUNCH: This should update in semi-real time
	private static void ShowPlayerList()
	{
		bool bSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);

		if (bSpawned)
		{
			m_PlayerListGUI.SetVisible(true, false, false);

			m_PlayerListGUI.Reset();
			AddPlayer(Player.LocalPlayer);
			foreach (var player in Entities.Players.All.Where(player => player != Player.LocalPlayer))
			{
				AddPlayer(player);
			}

			bool bAdminDuty = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ADMIN_DUTY);
			m_PlayerListGUI.CommitPlayers(bAdminDuty ? true : false);
		}
	}

	private static void AddPlayer(Player player)
	{
		int latency = DataHelper.GetEntityData<int>(player, EDataNames.PING);
		int playerID = DataHelper.GetEntityData<int>(player, EDataNames.PLAYER_ID);
		string strPlayerName = DataHelper.GetEntityData<string>(player, EDataNames.CHARACTER_NAME);
		string strUsername = DataHelper.GetEntityData<string>(player, EDataNames.USERNAME);
		int hoursPlayed = DataHelper.GetEntityData<int>(player, EDataNames.MINUTES_PLAYED) / 60;
		EAdminLevel adminLevel = DataHelper.GetEntityData<EAdminLevel>(player, EDataNames.ADMIN_LEVEL);
		bool adminDuty = DataHelper.GetEntityData<bool>(player, EDataNames.ADMIN_DUTY);

		string strTitle = Helpers.GetAdminLevelName(adminLevel);
		bool bIsLoggedIn = DataHelper.GetEntityData<bool>(player, EDataNames.IS_LOGGED_IN);
		bool bIsSpawned = DataHelper.GetEntityData<bool>(player, EDataNames.IS_SPAWNED);

		if (!bIsLoggedIn)
		{
			strTitle = "Logging In";
		}
		else if (!bIsSpawned)
		{
			strTitle = "Selecting Character";
		}

		m_PlayerListGUI.AddPlayer(playerID, strPlayerName, strTitle, latency, hoursPlayed, adminDuty, player == RAGE.Elements.Player.LocalPlayer, strUsername);
	}

	private static void HidePlayerList()
	{
		m_PlayerListGUI.SetVisible(false, false, false);
		m_PlayerListGUI.Reset();
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIPlayerList m_PlayerListGUI = new CGUIPlayerList(OnUILoaded);
}

internal class CGUIPlayerList : CEFCore
{
	public CGUIPlayerList(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/playerlist.html", EGUIID.PlayerList, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void AddPlayer(int playerID, string strPlayerName, string strTitle, int latency, int hoursPlayed, bool adminDuty, bool bIsLocal, string strUsername)
	{
		Execute("AddPlayer", playerID, strPlayerName, strTitle, latency, hoursPlayed, adminDuty, bIsLocal, strUsername);
	}

	public void CommitPlayers(bool bAdminDuty = false)
	{
		Execute("CommitPlayers", bAdminDuty);
	}
}