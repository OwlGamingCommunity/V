using GTANetworkAPI;
using System;

public class RadialMenuActions 
{
	public RadialMenuActions()
	{
		NetworkEvents.CuffPlayer += CuffPlayer;
		NetworkEvents.FriskPlayer += FriskPlayer;
	}

	public void CuffPlayer(CPlayer requestingPlayer, Player targetPlayerClient)
	{
		if (targetPlayerClient == null)
		{
			return;
		}

		WeakReference<CPlayer> targetPlayerRef = PlayerPool.GetPlayerFromClient(targetPlayerClient);
		CPlayer targetPlayer = targetPlayerRef.Instance();

		if (targetPlayer != null)
		{
			if (!targetPlayer.IsCuffed())
			{
				targetPlayer.Cuff(requestingPlayer);
			}
			else
			{
				targetPlayer.Uncuff(requestingPlayer);
			}
		}
	}

	public void FriskPlayer(CPlayer requestingPlayer, Player targetPlayerClient)
	{
		if (targetPlayerClient == null)
		{
			return;
		}

		WeakReference<CPlayer> targetPlayerRef = PlayerPool.GetPlayerFromClient(targetPlayerClient);
		CPlayer targetPlayer = targetPlayerRef.Instance();

		if (targetPlayer != null)
		{
			targetPlayer.Frisk(requestingPlayer);
		}
	}
}