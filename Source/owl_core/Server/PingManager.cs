using GTANetworkAPI;
using System;

public class PingManager 
{
	private WeakReference<MainThreadTimer> g_UpdatePingTimer = new WeakReference<MainThreadTimer>(null);

	public PingManager()
	{
		g_UpdatePingTimer = MainThreadTimerPool.CreateGlobalTimer(UpdatePing, 60000);
	}

	private void UpdatePing(object[] parameters)
	{
		foreach (var player in PlayerPool.GetAllPlayers_IncludeOutOfGame())
		{
			player.SetData(player.Client, EDataNames.PING, player.Client.Ping, EDataType.Synced);
		}
	}
}