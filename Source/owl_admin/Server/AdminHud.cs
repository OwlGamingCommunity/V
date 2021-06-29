using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class AdminHud
{
	private WeakReference<MainThreadTimer> g_FPSTimer = new WeakReference<MainThreadTimer>(null);
	private WeakReference<MainThreadTimer> g_PlayerCountTimer = new WeakReference<MainThreadTimer>(null);
	private int g_NumFramesThisSecond = 0;
	private int g_LastFPS = 0;
	private int g_TargetFPS = 30;
	private DateTime g_StartTime = DateTime.Now;

	public AdminHud()
	{
		// COMMANDS
		CommandManager.RegisterCommand("uptime", "Show Server Uptime", new Action<CPlayer, CVehicle>(Uptime), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeUaOrScripter);
		CommandManager.RegisterCommand("perfhud", "Toggle Performance Hud", new Action<CPlayer, CVehicle>(AdminPerfHud), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeUaOrScripter);

		NetworkEvents.OnPlayerDisconnected += OnPlayerDisconnected;

		g_FPSTimer = MainThreadTimerPool.CreateGlobalTimer(UpdateFPS, 1000);
		g_PlayerCountTimer = MainThreadTimerPool.CreateGlobalTimer(WriteServerStats, 60000);

		RageEvents.RAGE_OnUpdate += UpdateStats;
	}

	public void WriteServerStats(object[] a_Parameters = null)
	{
		TimeSpan uptime = DateTime.Now - g_StartTime;

		String[] strData = new string[]
		{
			PlayerPool.GetAllPlayers_IncludeOutOfGame().Count.ToString(),
			g_LastFPS.ToString(),
			uptime.ToString(@"d\d\:h\h\:m\m\:s\s")
		};
		System.IO.File.WriteAllLines("serverstats", strData);
	}

	private void UpdateFPS(object[] a_Parameters = null)
	{
		// transmit to subscribed players
		foreach (var playerRef in m_lstPlayersSubscribedToDevelopmentData)
		{
			CPlayer player = playerRef.Instance();
			if (player != null && (player.IsAdmin(EAdminLevel.HeadAdmin) || player.IsScripter()))
			{
				// TODO_POST_LAUNCH: Unsubscribe if no longer admin?
				NetworkEventSender.SendNetworkEvent_PerfData(player, g_NumFramesThisSecond);
			}
		}

		// TODO_GITHUB: Replace CommunityName with your community name
		Console.Title = Helpers.FormatString("CommunityName Server - FPS: {0} - PLAYERS: {1}", g_NumFramesThisSecond, PlayerPool.GetAllPlayers_IncludeOutOfGame().Count);
		g_LastFPS = g_NumFramesThisSecond;

		if (g_NumFramesThisSecond < g_TargetFPS)
		{
			PrintLogger.LogMessage(ELogSeverity.PERFORMANCE, "WARNING: Missed Target FPS (Expected: {0}fps Actual: {1}fps)", g_TargetFPS, g_NumFramesThisSecond);
		}

		g_NumFramesThisSecond = 0;

		// On debug, put every admin in the perf subscribers group
#if DEBUG
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player.IsAdmin())
			{
				bool bAlreadyInList = false;
				foreach (var playerRef in m_lstPlayersSubscribedToDevelopmentData)
				{
					if (playerRef.Instance() == player)
					{
						bAlreadyInList = true;
						break;
					}
				}

				if (!bAlreadyInList)
				{
					m_lstPlayersSubscribedToDevelopmentData.Add(new WeakReference<CPlayer>(player));
					player.SendNotification("Admin", ENotificationIcon.Font, "Development HUD has been enabled.");
					NetworkEventSender.SendNetworkEvent_AdminPerfHudState(player, true);
				}
			}
		}
#endif
	}

	private DateTime g_LastFrameTime = DateTime.Now;

	private void UpdateStats()
	{
		g_NumFramesThisSecond++;

		// last frame time
		Int64 TimeTakenLastFrame = MainThreadTimerPool.GetMillisecondsSinceDateTime(g_LastFrameTime);
		if (TimeTakenLastFrame > 0)
		{
			g_LastFrameTime = DateTime.Now;

			if (TimeTakenLastFrame > (1000 / g_TargetFPS))
			{
#if DEBUG
				PrintLogger.LogMessage(ELogSeverity.PERFORMANCE, "WARNING: Framerate slowdown (Last Frametime: {0}ms)", TimeTakenLastFrame);
#endif
			}
		}
	}


	public void OnPlayerDisconnected(CPlayer player, DisconnectionType type, string reason)
	{
		foreach (var playerRef in m_lstPlayersSubscribedToDevelopmentData)
		{
			if (playerRef.Instance() == player)
			{
				m_lstPlayersSubscribedToDevelopmentData.Remove(playerRef);
				break;
			}
		}
	}

	public void Uptime(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderPlayer.IsAdmin(EAdminLevel.HeadAdmin) || SenderPlayer.IsScripter())
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 194, 14, "Server started at {0}", g_StartTime.ToString("r"));

			TimeSpan uptime = DateTime.Now - g_StartTime;
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 194, 14, "Uptime: {0}", uptime.ToString(@"d\d\:h\h\:m\m\:s\s"));
		}
	}

	public void AdminPerfHud(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderPlayer.IsAdmin(EAdminLevel.HeadAdmin) || SenderPlayer.IsScripter())
		{
			// Are we already in the subscribers list?
			bool bAlreadyInList = false;
			WeakReference<CPlayer> reftoRemove = null;
			foreach (var playerRef in m_lstPlayersSubscribedToDevelopmentData)
			{
				if (playerRef.Instance() == SenderPlayer)
				{
					reftoRemove = playerRef;
					bAlreadyInList = true;
					break;
				}
			}

			if (bAlreadyInList)
			{
				m_lstPlayersSubscribedToDevelopmentData.Remove(reftoRemove);

				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "Development HUD has been disabled.");
				NetworkEventSender.SendNetworkEvent_AdminPerfHudState(SenderPlayer, false);
			}
			else
			{
				m_lstPlayersSubscribedToDevelopmentData.Add(new WeakReference<CPlayer>(SenderPlayer));
				SenderPlayer.SendNotification("Admin", ENotificationIcon.Font, "Development HUD has been enabled.");
				NetworkEventSender.SendNetworkEvent_AdminPerfHudState(SenderPlayer, true);
			}
		}
	}

	List<WeakReference<CPlayer>> m_lstPlayersSubscribedToDevelopmentData = new List<WeakReference<CPlayer>>();
}