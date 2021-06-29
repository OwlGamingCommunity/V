using GTANetworkAPI;
using System.Collections.Generic;

public class WorldBlipSystem
{
	public WorldBlipSystem()
	{
		List<CDatabaseStructureWorldBlip> lstWorldBlips = Database.LegacyFunctions.LoadAllWorldBlips().Result;
		NAPI.Task.Run(async () =>
		{
			foreach (var worldBlip in lstWorldBlips)
			{
				await WorldBlipPool.CreateWorldBlip(worldBlip.ID, worldBlip.Name, worldBlip.Sprite, worldBlip.Color, worldBlip.vecPos, false).ConfigureAwait(true);
			}
		});
		NAPI.Util.ConsoleOutput("[WORLD BLIPS] Loaded {0} World Blips!", lstWorldBlips.Count);
	}
}



