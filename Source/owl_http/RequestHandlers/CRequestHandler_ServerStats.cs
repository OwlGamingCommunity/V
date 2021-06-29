using GTANetworkAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

/// <summary>
/// Gets a JSON array of server information
/// </summary>
internal class CRequestHandler_ServerStats : CRequestHandler
{
	public CRequestHandler_ServerStats() : base("ServerStats")
	{

	}

	private struct SServerStats
	{
		public SServerStats(int players, int port, string name, int maxplayers)
		{
			Players = players;
			Port = port;
			Name = name;
			MaxPlayers = maxplayers;
		}

		public int Players;
		public int Port;
		public string Name;
		public int MaxPlayers;
	}

	public override string Get(HttpListenerRequest request)
	{
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers_IncludeOutOfGame();
		SServerStats stats = new SServerStats(players.Count,
			NAPI.Server.GetServerPort(),
			NAPI.Server.GetServerName(),
			NAPI.Server.GetMaxPlayers());

		return JsonConvert.SerializeObject(stats);
	}
}