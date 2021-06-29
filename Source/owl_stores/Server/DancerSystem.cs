using GTANetworkAPI;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class DancerSystem
{
	private const float DANCER_TIP = 10.0f;

	public DancerSystem()
	{
		NetworkEvents.OnInteractWithDancer += OnInteractWithDancer;
		NetworkEvents.OnOwnerCollectDancerTips += OnCollectDancerTips;

		List<CDatabaseStructureDancer> lstDancer = Database.LegacyFunctions.LoadAllDancers().Result;
		NAPI.Task.Run(async () =>
		{
			foreach (var dancer in lstDancer)
			{
				await DancerPool.CreateDancer(dancer.dancerID, dancer.vecPos, dancer.fRotZ, dancer.dancerSkin, dancer.Dimension, dancer.tipMoney, dancer.animDict, dancer.animName, dancer.bAllowTip, dancer.parentPropertyID, false).ConfigureAwait(true);
			}
		});
		NAPI.Util.ConsoleOutput("[DANCERS] Loaded {0} dancers!", lstDancer.Count);
	}

	private void OnInteractWithDancer(CPlayer player, EntityDatabaseID dancerId)
	{
		CDancerInstance dancerInst = DancerPool.GetInstanceFromID(dancerId);

		// Sanity check just to be sure
		if (dancerInst == null)
		{
			return;
		}

		if (!player.CanPlayerAffordCost(DANCER_TIP))
		{
			player.SendNotification("Dancer", ENotificationIcon.HeartEmpty, "You don't have 10$ cash to tip the dancer");
			return;
		}

		if (dancerInst.m_parentPropertyID != -1 && dancerInst.m_parentPropertyID != 0)
		{
			DancerReceiveTip(dancerInst);
			player.SubtractMoney(DANCER_TIP, PlayerMoneyModificationReason.DancerTip);
			player.SendNotification("Dancer", ENotificationIcon.HeartEmpty, "You tipped 10$!");
		}
		else
		{
			player.SendNotification("Dancer", ENotificationIcon.HeartEmpty, "This dancer can't be tipped");
		}
	}

	private async void DancerReceiveTip(CDancerInstance dancerInst)
	{
		dancerInst.m_tipMoney += DANCER_TIP;
		await Database.LegacyFunctions.TipDancer(dancerInst.m_DatabaseID, dancerInst.m_tipMoney).ConfigureAwait(true);
	}

	private async void OnCollectDancerTips(CPlayer player, long dancerID)
	{
		CDancerInstance dancerInst = DancerPool.GetInstanceFromID(dancerID);

		if (dancerInst != null)
		{
			if (!dancerInst.m_bAllowTip || dancerInst.m_parentPropertyID == -1 || dancerInst.m_parentPropertyID == 0)
			{
				player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "Can't collect the tips from this dancer. If this is not supposed to happen, contact and administrator using F2");
				return;
			}

			if (dancerInst.m_tipMoney != 0.0f)
			{
				player.AddMoney(dancerInst.m_tipMoney, PlayerMoneyModificationReason.DancerTip_Receive);
				player.PushChatMessageWithColor(EChatChannel.Notifications, 0, 255, 0, "You received {0} from the dancer their tips.", Helpers.ColorString(255, 255, 255, "${0}", dancerInst.m_tipMoney));
				dancerInst.m_tipMoney = 0.0f;
				await Database.LegacyFunctions.ResetDancerTipMoney(dancerInst.m_DatabaseID).ConfigureAwait(true);
			}
			else
			{
				player.SendNotification("Dancer", ENotificationIcon.HeartEmpty, "The dancer currently doesn't have any tips.");
			}
		}
	}
}
