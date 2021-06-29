using System.Collections.Generic;

public class PDShield
{
	Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> m_dictShields = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();
	List<RAGE.Elements.Player> m_lstPendingLoad = new List<RAGE.Elements.Player>();


	public PDShield()
	{
		RageEvents.RAGE_OnTick_PerFrame += OnTick;
	}

	private void OnTick()
	{
		UpdateShields();
	}

	// TODO_LAUNCH: What about when a player quits? Leaking objects.
	// TODO_POST_LAUNCH: changing skin whilst having a shield probably breaks anim + shield object gets loose
	// TODO_POST_LAUNCH: Block AR's etc
	// TODO_POST_LAUNCH: On arm state for walknig around
	// TODO_POST_LAUNCH: reload takes down shield temporarily
	private void UpdateShields()
	{
		List<RAGE.Elements.Player> lstPlayerShieldsToRemove = new List<RAGE.Elements.Player>();
		foreach (var player in RAGE.Elements.Entities.Players.Streamed)
		{
			EShieldType shieldType = DataHelper.GetEntityData<EShieldType>(player, EDataNames.SHIELD);

			if (shieldType != EShieldType.None)
			{
				// Do we need to create it?
				if (!m_dictShields.ContainsKey(player))
				{
					if (!m_lstPendingLoad.Contains(player))
					{
						m_lstPendingLoad.Add(player);
						uint hash = HashHelper.GetHashUnsigned(shieldType == EShieldType.Riot ? "prop_riot_shield" : "prop_ballistic_shield");
						AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
						{
							m_dictShields[player] = new RAGE.Elements.MapObject(hash, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f));
							RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetPedCanArmIk, player.Handle, 0);
							m_lstPendingLoad.Remove(player);

							RAGE.Game.Entity.AttachEntityToEntity(m_dictShields[player].Handle, player.Handle, player.GetBoneIndex(61163), 0.2f, 0.12f, -0.26f, 121.0f, 26.0f, 224.0f, true, false, false, false, 0, true);
						});
					}
				}
				else
				{
					// re attach, update dim etc
					m_dictShields[player].Dimension = player.Dimension;

					RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetPedCanArmIk, player.Handle, 0);

					RAGE.Game.Entity.AttachEntityToEntity(m_dictShields[player].Handle, player.Handle, player.GetBoneIndex(61163), 0.2f, 0.12f, -0.26f, 121.0f, 26.0f, 224.0f, true, false, false, false, 0, true);
				}
			}
			else
			{
				// Do we need to destroy it?
				if (m_dictShields.ContainsKey(player))
				{
					RAGE.Game.Invoker.Invoke(RAGE.Game.Natives.SetPedCanArmIk, player.Handle, 1);

					RAGE.Elements.MapObject shieldObject = m_dictShields[player];
					if (shieldObject != null)
					{
						shieldObject.Destroy();
					}

					m_dictShields[player] = null;
					lstPlayerShieldsToRemove.Add(player);
				}

				if (m_lstPendingLoad.Contains(player))
				{
					m_lstPendingLoad.Remove(player);
				}


			}
		}

		// process pending removals
		foreach (var playerToRemove in lstPlayerShieldsToRemove)
		{
			m_dictShields.Remove(playerToRemove);
		}
	}
}