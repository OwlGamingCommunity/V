using System;

public class BlackjackActivityManager : ActivityManager
{
	public BlackjackActivityManager() : base(EActivityType.Blackjack)
	{
		NetworkEvents.Blackjack_PlaceBet += OnPlayerPlaceBet;
		NetworkEvents.BlackJack_Action_HitMe += OnPlayerAction_HitMe;
		NetworkEvents.BlackJack_Action_Stick += OnPlayerAction_Stick;

		NetworkEvents.Blackjack_PlaceBet_GetDetails += OnPlayerPlaceBet_GetDetails_AndGotoPlaceBet;
	}

	public static void OnPlayerPlaceBet_GetDetails_AndGotoPlaceBet(CPlayer SenderPlayer)
	{
		UInt32 totalChips = SenderPlayer.Inventory.GetTotalStackSizeOfAllInstancesOfItemID(EItemID.CASINO_CHIP_BUCKET);
		NetworkEventSender.SendNetworkEvent_Blackjack_PlaceBet_GotDetails(SenderPlayer, Convert.ToInt32(totalChips));
	}

	private void OnPlayerAction_HitMe(CPlayer a_Player, Int64 uniqueActivityIdentifier)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			BlackjackActivityInstance activityInstance = (BlackjackActivityInstance)m_dictActivityInstances[uniqueActivityIdentifier];
			activityInstance.OnPlayerHitMe(a_Player);
		}
	}

	private void OnPlayerAction_Stick(CPlayer a_Player, Int64 uniqueActivityIdentifier)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			BlackjackActivityInstance activityInstance = (BlackjackActivityInstance)m_dictActivityInstances[uniqueActivityIdentifier];
			activityInstance.OnPlayerStick(a_Player);
		}
	}

	private void OnPlayerPlaceBet(CPlayer a_Player, Int64 uniqueActivityIdentifier, int amount)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			BlackjackActivityInstance activityInstance = (BlackjackActivityInstance)m_dictActivityInstances[uniqueActivityIdentifier];
			activityInstance.OnPlayerSetBet(a_Player, amount);
		}
	}

	protected override ActivityInstance CreateNewInstance_Internal(Int64 uniqueIdentifier)
	{
		return new BlackjackActivityInstance(uniqueIdentifier);
	}
}