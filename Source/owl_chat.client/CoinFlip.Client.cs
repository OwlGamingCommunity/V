using System.Collections.Generic;

public class CoinFlip
{
	public CoinFlip()
	{
		RageEvents.RAGE_OnEntityStreamIn += OnEntityStreamIn;
		ClientTimerPool.CreateTimer(UpdateCoinForAll, 200);
	}

	private void OnEntityStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			UpdateCoin((RAGE.Elements.Player)entity);
		}
	}

	// TODO_HELPER: Create a helper function for this
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> g_DictCoinAttachments = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();

	private void UpdateCoin(RAGE.Elements.Player player)
	{
		// TODO_CELLPHONE: Fix this attachment code
		bool bCoinFlip = DataHelper.GetEntityData<bool>(player, EDataNames.COINFLIP);

		if (bCoinFlip)
		{
			if (!g_DictCoinAttachments.ContainsKey(player))
			{
				uint hash = HashHelper.GetHashUnsigned("vw_prop_vw_coin_01a");
				AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
				{
					g_DictCoinAttachments.Add(player, new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension));
				});
			}

			if (g_DictCoinAttachments.ContainsKey(player))
			{
				RAGE.Game.Entity.AttachEntityToEntity(g_DictCoinAttachments[player].Handle, player.Handle, player.GetBoneIndex(28422), 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true, false, false, false, 0, true);
			}
		}
		else
		{
			if (g_DictCoinAttachments.ContainsKey(player))
			{
				g_DictCoinAttachments[player].Destroy();
				g_DictCoinAttachments.Remove(player);
			}
		}
	}

	private void UpdateCoinForAll(object[] parameters)
	{
		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			UpdateCoin(player);
		}
	}
}
