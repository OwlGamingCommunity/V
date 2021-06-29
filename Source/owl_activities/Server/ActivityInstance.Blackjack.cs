using System;
using System.Collections.Generic;
using System.Linq;

public class BlackjackActivityInstance : ActivityInstance
{
	private bool m_bFixForHouse = false;
	private int m_iFixForHouse_Target = -1;
	private Random m_RNG = new Random(Environment.TickCount);

	public BlackjackActivityInstance(Int64 uniqueIdentifier) : base(ActivityConstants.BlackjackMaxParticipants, uniqueIdentifier, EActivityType.Blackjack)
	{
		m_State = new BlackjackActivityState();
	}

	public BlackjackActivityState GetActivityState()
	{
		return (BlackjackActivityState)m_State;
	}

	public override bool TryParticipate_EventSpecificChecks(CPlayer a_Player, int participantIndex, out string strFailureReason)
	{
		strFailureReason = string.Empty;

		// player needs at least 1 chip to participate
		UInt32 totalChips = a_Player.Inventory.GetTotalStackSizeOfAllInstancesOfItemID(EItemID.CASINO_CHIP_BUCKET);

		if (totalChips == 0)
		{
			strFailureReason = "You must have casino chips to play at this table. Talk to the dealer to buy chips.";
		}

		return totalChips > 0;
	}

	public override void OnParticipantJoined(CPlayer a_Player)
	{
		a_Player.AddAnimationToQueue((int)(AnimationFlags.Loop), "anim_casino_b@amb@casino@games@shared@player@", "idle_cardgames", false, true, true, 0, false);

		// add player state
		GetActivityState().PlayerStates.Add(a_Player.PlayerID, new BlackjackPlayerState());
	}

	public override void OnParticipantLeft(CPlayer a_Player)
	{
		var ActivityState = GetActivityState();
		if (ActivityState != null && ActivityState.PlayerStates.ContainsKey(a_Player.PlayerID))
		{
			ActivityState.PlayerStates.Remove(a_Player.PlayerID);
		}

		m_lstParticipants.Remove(a_Player);
		a_Player.StopCurrentAnimation(true, true);
	}

	private void SetState(EBlackJackActivityState newState)
	{
		GetActivityState().State = newState;
		ResetStateTimer();
		TransmitState();
	}

	private BlackjackPlayerState GetPlayerState(CPlayer player)
	{
		return GetActivityState().PlayerStates[player.PlayerID];
	}

	public void OnPlayerSetBet(CPlayer a_Player, int amount)
	{
		// take currency from the player (we will give it back in the case where they win)
		// if they don't have enough, we tell the client and keep them on the bet screen
		const int maxBet = 10000;
		if (amount > 0 && amount <= maxBet)
		{
			bool bRemovedCurrency = a_Player.Inventory.DecrementStackSizeOverMultipleInstances(EItemID.CASINO_CHIP_BUCKET, Convert.ToUInt32(amount));
			if (bRemovedCurrency)
			{
				GetPlayerState(a_Player).CurrentBet = amount;
				SendActivityMessage("Dealer: Okay, we've got ${0} from {1}", amount, a_Player.GetCharacterName(ENameType.StaticCharacterName));

				TransmitState();
			}
			else
			{
				UInt32 totalChips = a_Player.Inventory.GetTotalStackSizeOfAllInstancesOfItemID(EItemID.CASINO_CHIP_BUCKET);
				a_Player.SendNotification("BlackJack Place Bet", ENotificationIcon.ExclamationSign, "You cannot bet {0} chips since you only have {1}", amount, totalChips);

				BlackjackActivityManager.OnPlayerPlaceBet_GetDetails_AndGotoPlaceBet(a_Player);
			}
		}
		else if (amount > maxBet)
		{
			a_Player.SendNotification("BlackJack Place Bet", ENotificationIcon.ExclamationSign, "You cannot bet more than the limit for this table ({0} chips)", maxBet);
			BlackjackActivityManager.OnPlayerPlaceBet_GetDetails_AndGotoPlaceBet(a_Player);
		}
		else
		{
			BlackjackActivityManager.OnPlayerPlaceBet_GetDetails_AndGotoPlaceBet(a_Player);
		}
	}

	public void OnPlayerHitMe(CPlayer a_Player)
	{
		SendActivityMessage("{0}: Hit me!", a_Player.GetCharacterName(ENameType.StaticCharacterName));

		TryIssueCardToParticipant(a_Player);

		// If we hit the max card limit, increment state / go forward, otherwise go backwards so they can hit again
		var playerState = GetPlayerState(a_Player);
		if (playerState != null && playerState.Cards.Count >= ActivityConstants.BlackjackMaxCards)
		{
			IncrementState();
		}
		else
		{
			DecrementState(); // go back to issue state for this player index
		}
	}

	private void IncrementState()
	{
		// sanity check
		if (((int)GetActivityState().State + 1) > (int)EBlackJackActivityState.DetermineRoundOutcome_WaitingToGotoInactiveAndNextRound)
		{
			return;
		}

		SetState((EBlackJackActivityState)((int)GetActivityState().State + 1));
	}

	private void DecrementState()
	{
		// sanity check
		if (((int)GetActivityState().State - 1) < 0)
		{
			return;
		}

		SetState((EBlackJackActivityState)(GetActivityState().State - 1));
	}

	public void OnPlayerStick(CPlayer a_Player)
	{
		SendActivityMessage("{0}: Stick!", a_Player.GetCharacterName(ENameType.StaticCharacterName));
		IncrementState();
	}

	private CasinoCard AddRandomizedCard(BlackjackPlayerState state, bool bFaceUp, bool bIsDealer, bool bIsFirstCard)
	{
		ECard card;

		if (bIsDealer && m_bFixForHouse) // fix the cards
		{
			if (!bIsFirstCard)
			{
				// work out what card we need to give to hit our target
				BlackjackActivityHelpers.CalculateValueOfCards(state.Cards, out List<string> lstDisplays, out List<int> lstValues, out List<int> lstValuesWithinMaxRange, out string strValidCardCombinations, out int highestValueWithinMaxRange);

				int numRequired = m_iFixForHouse_Target - highestValueWithinMaxRange;

				card = (ECard)Math.Max((int)ECard.Ace, Math.Min(numRequired - 1, (int)ECard.King));
			}
			else // give them a reasonably high card for the first card
			{
				card = (ECard)m_RNG.Next((int)ECard.Nine, (int)ECard.King);
			}
		}
		else // // not dealer, or not fixed, or fixed but is first card
		{
			card = (ECard)m_RNG.Next((int)ECard.Ace, (int)ECard.King);
		}

		ECardSuite suite = (ECardSuite)m_RNG.Next((int)ECardSuite.Heart, (int)ECardSuite.Diamond);

		CasinoCard newCard = new CasinoCard(card, suite, bFaceUp);
		state.Cards.Add(newCard);

		TransmitState();
		return newCard;
	}

	private bool HasParticipant(int index, bool bIncludeNotParticipatingInThisRound, out bool HasBlackjack, out bool IsBust, out CPlayer player)
	{
		player = null;
		HasBlackjack = false;
		IsBust = true;

		bool bHasParticipant = m_lstParticipants.Count >= index + 1;
		if (!bIncludeNotParticipatingInThisRound)
		{
			// otherwise its already out of range
			if (bHasParticipant)
			{
				CPlayer participant = m_lstParticipants[index];
				var playerState = GetPlayerState(participant);
				player = participant;

				// check for blackjack and bust
				BlackjackActivityHelpers.CalculateValueOfCards(playerState.Cards, out List<string> lstDisplays, out List<int> lstValues, out List<int> lstValuesWithinMaxRange, out string strValidCardCombinations, out int highestValueWithinMaxRange);
				foreach (int cardValue in lstValues)
				{
					// if we have a valid card set, we arent bust
					if (cardValue <= BlackjackActivityHelpers.BlackjackValue)
					{
						IsBust = false;
					}

					if (cardValue == BlackjackActivityHelpers.BlackjackValue)
					{
						HasBlackjack = true;
					}
				}

				if (playerState.IsParticipatingInCurrentRound())
				{
					bHasParticipant = true;
				}
				else
				{
					bHasParticipant = false;
				}

				// the first time we see blackjack/bust this round, announce it
				if (!playerState.FirstTimeFlag_HasBlackjack && HasBlackjack)
				{
					playerState.FirstTimeFlag_HasBlackjack = true;
					SendActivityMessage("{0} has blackjack!", participant.GetCharacterName(ENameType.StaticCharacterName));

					participant.AwardAchievement(EAchievementID.GetBlackjack);
				}
				else if (!playerState.FirstTimeFlag_IsBust && IsBust)
				{
					playerState.FirstTimeFlag_IsBust = true;
					SendActivityMessage("{0} is bust!", participant.GetCharacterName(ENameType.StaticCharacterName));

					participant.AwardAchievement(EAchievementID.GoBust);
				}
			}
		}

		return bHasParticipant;
	}

	private void TryIssueCardToParticipant(CPlayer participant)
	{
		if (GetPlayerState(participant).IsParticipatingInCurrentRound()) // are they participating?
		{
			CasinoCard card = AddRandomizedCard(GetPlayerState(participant), true, false, false);
			//SendActivityMessage("{0} got a {2} of {1}s", participant.GetCharacterName(ENameType.StaticCharacterName), card.Suite, card.Card);
		}
	}

	private void TryIssueCardToParticipant(int index)
	{
		if (m_lstParticipants.Count >= index + 1)
		{
			CPlayer participant = m_lstParticipants[index];
			TryIssueCardToParticipant(participant);
		}
	}

	public override void Update()
	{
		BlackjackActivityState activityState = GetActivityState();
		switch (activityState.State)
		{
			case EBlackJackActivityState.Inactive:
				{
					if (m_lstParticipants.Count > 0)
					{
						SetState(EBlackJackActivityState.PlaceBets_Init);
					}

					break;
				}

			case EBlackJackActivityState.PlaceBets_Init:
				{
					// reset every players bet, cards etc
					foreach (var playerState in activityState.PlayerStates.Values)
					{
						playerState.ResetPerRoundVariables();
					}

					// Reset dealer
					activityState.DealerState.Cards.Clear();

					SendActivityMessage("Dealer: Place your bets please!");
					SetState(EBlackJackActivityState.PlaceBets_Wait);

					// TODO_ACTIVITY_LOW_PRIO: Every state should check if no participants, go back to inactive

					break;
				}

			case EBlackJackActivityState.PlaceBets_Wait:
				{
					Int64 timeInState = GetMillisecondsInState();

					// TODO_ACTIVITY_LOW_PRIO: Dont let player bet 0

					// Are all bets in? lets us fast track to the next stage
					bool bAllBetsIn = true;
					foreach (var playerState in activityState.PlayerStates.Values)
					{
						bAllBetsIn &= playerState.CurrentBet > 0;
					}

					if (timeInState > BlackjackTimeouts.timeForBets || bAllBetsIn)
					{
						// Did we get any bet? if not, go back to idle
						bool bHadValidBet = false;
						foreach (var playerState in activityState.PlayerStates.Values)
						{
							if (playerState.CurrentBet > 0)
							{
								bHadValidBet = true;
								break;
							}
						}

						if (bHadValidBet)
						{
							SendActivityMessage("Dealer: All bets are in!");
							SetState(EBlackJackActivityState.DealCard_Player1_1_Camera);

							// should we fix it for the house?
							int random = m_RNG.Next(0, 5);
							if (random != 4) // 80% chance of being fixed for the house
							{
								m_bFixForHouse = true;
								m_iFixForHouse_Target = m_RNG.Next(20, 22); // 21 max for blackjack
							}
							else
							{
								m_bFixForHouse = false;
								m_iFixForHouse_Target = -1;
							}
						}
						else
						{
							SendActivityMessage("Dealer: No bets for this round!");
							SetState(EBlackJackActivityState.Inactive);
						}
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_1_Camera:
				{
					TryIssueCardToParticipant(0);
					SetState(EBlackJackActivityState.DealCard_Player1_1_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_1_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(0, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Player2_1_Camera);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_1_Camera:
				{
					TryIssueCardToParticipant(1);
					SetState(EBlackJackActivityState.DealCard_Player2_1_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_1_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(1, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Player3_1_Camera);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_1_Camera:
				{
					TryIssueCardToParticipant(2);
					SetState(EBlackJackActivityState.DealCard_Player3_1_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_1_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(2, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Player4_1_Camera);
					}

					break;
				}

			case EBlackJackActivityState.DealCard_Player4_1_Camera:
				{
					TryIssueCardToParticipant(3);
					SetState(EBlackJackActivityState.DealCard_Player4_1_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_1_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(3, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Dealer_1_Camera);
					}

					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_1_Camera:
				{
					CasinoCard card = AddRandomizedCard(GetActivityState().DealerState, true, true, true);
					//SendActivityMessage("Dealer got a {1} of {0}s", card.Suite, card.Card);
					SetState(EBlackJackActivityState.DealCard_Dealer_1_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_1_WaitForIssue:
				{
					if (GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Player1_2_Camera);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_2_Camera:
				{
					TryIssueCardToParticipant(0);

					SetState(EBlackJackActivityState.DealCard_Player1_2_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_2_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(0, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Player2_2_Camera);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_2_Camera:
				{
					TryIssueCardToParticipant(1);

					SetState(EBlackJackActivityState.DealCard_Player2_2_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_2_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(1, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Player3_2_Camera);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_2_Camera:
				{
					TryIssueCardToParticipant(2);

					SetState(EBlackJackActivityState.DealCard_Player3_2_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_2_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(2, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Player4_2_Camera);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_2_Camera:
				{
					TryIssueCardToParticipant(3);

					SetState(EBlackJackActivityState.DealCard_Player4_2_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_2_WaitForIssue:
				{
					// If no participant, just go straight in!
					if (!HasParticipant(3, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						SetState(EBlackJackActivityState.DealCard_Dealer_2_Camera);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_2_Camera:
				{
					CasinoCard card = AddRandomizedCard(GetActivityState().DealerState, false, true, false);
					//SendActivityMessage("Dealer got their second card");
					SetState(EBlackJackActivityState.DealCard_Dealer_2_WaitForIssue);
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_2_WaitForIssue:
				{
					if (GetMillisecondsInState() > BlackjackTimeouts.timeBetweenCardDeals)
					{
						// If blackjack for next player, skip the next state
						if (HasParticipant(0, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) && (bHasBlackjack || bIsBust))
						{
							SetState(EBlackJackActivityState.Player1_Wait);
						}
						else
						{
							SetState(EBlackJackActivityState.Player1_MakeChoice);
						}
					}
					break;
				}

			case EBlackJackActivityState.Player1_MakeChoice:
				{
					SetState(EBlackJackActivityState.Player1_Wait);
					break;
				}

			case EBlackJackActivityState.Player1_Wait:
				{
					if (!HasParticipant(0, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || bHasBlackjack || bIsBust || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenActionStates)
					{
						// If blackjack for next player, skip the next state
						if (HasParticipant(1, false, out bool bHasBlackjackNext, out bool bIsBustNext, out CPlayer playerNext) && (bHasBlackjackNext || bIsBustNext))
						{
							SetState(EBlackJackActivityState.Player2_Wait);
						}
						else
						{
							SetState(EBlackJackActivityState.Player2_MakeChoice);
						}
					}
					break;
				}

			case EBlackJackActivityState.Player2_MakeChoice:
				{
					SetState(EBlackJackActivityState.Player2_Wait);
					break;
				}

			case EBlackJackActivityState.Player2_Wait:
				{
					if (!HasParticipant(1, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || bHasBlackjack || bIsBust || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenActionStates)
					{
						// If blackjack for next player, skip the next state
						if (HasParticipant(2, false, out bool bHasBlackjackNext, out bool bIsBustNext, out CPlayer playerNext) && (bHasBlackjackNext || bIsBustNext))
						{
							SetState(EBlackJackActivityState.Player3_Wait);
						}
						else
						{
							SetState(EBlackJackActivityState.Player3_MakeChoice);
						}
					}
					break;
				}

			case EBlackJackActivityState.Player3_MakeChoice:
				{
					SetState(EBlackJackActivityState.Player3_Wait);
					break;
				}

			case EBlackJackActivityState.Player3_Wait:
				{
					if (!HasParticipant(2, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || bHasBlackjack || bIsBust || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenActionStates)
					{
						// If blackjack for next player, skip the next state
						if (HasParticipant(3, false, out bool bHasBlackjackNext, out bool bIsBustNext, out CPlayer playerNext) && (bHasBlackjackNext || bIsBustNext))
						{


							SetState(EBlackJackActivityState.Player4_Wait);
						}
						else
						{
							SetState(EBlackJackActivityState.Player4_MakeChoice);
						}
					}
					break;
				}

			case EBlackJackActivityState.Player4_MakeChoice:
				{
					SetState(EBlackJackActivityState.Player4_Wait);
					break;
				}

			case EBlackJackActivityState.Player4_Wait:
				{
					if (!HasParticipant(3, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || bHasBlackjack || bIsBust || GetMillisecondsInState() > BlackjackTimeouts.timeBetweenActionStates)
					{
						SetState(EBlackJackActivityState.Dealer_TurnSecondCard);
					}
					break;
				}

			case EBlackJackActivityState.Dealer_TurnSecondCard:
				{
					// flip dealer cards
					foreach (CasinoCard card in activityState.DealerState.Cards)
					{
						if (!card.FaceUp)
						{
							card.FaceUp = true;
							//SendActivityMessage("Dealer flipped their second card to reveal a {0} of {1}s", card.Card, card.Suite);
							break;
						}
					}

					SetState(EBlackJackActivityState.Dealer_TurnSecondCard_Wait);
					break;
				}

			case EBlackJackActivityState.Dealer_TurnSecondCard_Wait:
				{
					if (GetMillisecondsInState() > BlackjackTimeouts.timeToDetermineRoundOutcome)
					{
						// TODO_ACTIVITY_LOW_PRIO: Use increment state instead of setstate so enum defines state
						SetState(EBlackJackActivityState.Dealer_StandOn17);
					}
					break;
				}

			case EBlackJackActivityState.Dealer_StandOn17:
				{
					BlackjackActivityHelpers.CalculateValueOfCards(activityState.DealerState.Cards, out List<string> lstDisplays_Dealer, out List<int> lstValues_Dealer, out List<int> lstValuesWithinMaxRange, out string strValidCardCombinations, out int highestValueWithinMaxRange_Dealer);
					int biggestDealerValue = highestValueWithinMaxRange_Dealer > 0 ? highestValueWithinMaxRange_Dealer : lstValues_Dealer[lstValues_Dealer.Count - 1]; // biggest number is always the last, because it has most ages

					if (biggestDealerValue >= BlackjackActivityHelpers.DealerStandValue)
					{
						// We're done, go straight to determine round outcome
						SetState(EBlackJackActivityState.DetermineRoundOutcome);
					}
					else
					{
						// we must issue a card and stand on 17 again
						CasinoCard card = AddRandomizedCard(GetActivityState().DealerState, true, true, false);
						SendActivityMessage("Dealer must stand on {2} so had to draw another card. A {1} of {0}s", card.Suite, card.Card, BlackjackActivityHelpers.DealerStandValue);

						IncrementState();
					}

					break;
				}

			case EBlackJackActivityState.Dealer_StandOn17_Wait:
				{
					if (GetMillisecondsInState() > BlackjackTimeouts.timeBetweenStandOn17Deals)
					{
						// Go back to stand on 17 state now, if we're >= 17, we'll just skip forward, otherwise, we'll issue another card
						DecrementState();
					}
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome:
				{
					// TODO_ACTIVITY_LOW_PRIO: UI can overflow with many aces
					// lets work out how won and who didnt
					BlackjackActivityHelpers.CalculateValueOfCards(activityState.DealerState.Cards, out List<string> lstDisplays_Dealer, out List<int> lstValues_Dealer, out List<int> lstValuesWithinMaxRange_Dealer, out string strValidCardCombinations_Dealer, out int highestValueWithinMaxRange_Dealer);

					// TODO_ACTIVITY_LOW_PRIO: a ui for this instead
					// TODO_ACTIVITY_LOW_PRIO: reduce chatbox spam everywhere

					string strDealerOutcome = String.Empty;

					int biggestDealerValue = highestValueWithinMaxRange_Dealer > 0 ? highestValueWithinMaxRange_Dealer : lstValues_Dealer[lstValues_Dealer.Count - 1]; // biggest number is always the last, because it has most ages
					bool bDealerIsBust = biggestDealerValue > BlackjackActivityHelpers.BlackjackValue;
					if (bDealerIsBust)
					{
						strDealerOutcome = Helpers.FormatString("Dealer: Bust ({0})", biggestDealerValue);
					}
					else
					{
						strDealerOutcome = Helpers.FormatString("Dealer: {0}", biggestDealerValue);
					}

					int ModificationToDealer = 0;
					int totalBets = 0;

					List<string> lstPlayerOutcomes = new List<string>();
					foreach (CPlayer participant in m_lstParticipants)
					{
						BlackjackPlayerState playerState = GetPlayerState(participant);
						BlackjackActivityHelpers.CalculateValueOfCards(playerState.Cards, out List<string> lstDisplays_Player, out List<int> lstValues_Player, out List<int> lstValuesWithinMaxRange_Player, out string strValidCardCombinations_Player, out int highestValueWithinMaxRange_Player);

						int biggestPlayerValue = highestValueWithinMaxRange_Player > 0 ? highestValueWithinMaxRange_Player : lstValues_Player[lstValues_Player.Count - 1]; // biggest number is always the last, because it has most aces
						bool bPlayerIsBust = biggestPlayerValue > BlackjackActivityHelpers.BlackjackValue;
						bool bWon = (bDealerIsBust || biggestPlayerValue > biggestDealerValue) && !bPlayerIsBust;
						bool bDraw = (!bWon && biggestPlayerValue == biggestDealerValue) && !bPlayerIsBust;

						totalBets += playerState.CurrentBet;

						if (bPlayerIsBust)
						{
							LogOutcome(participant, biggestPlayerValue, activityState, playerState, EOutcomeLog.Lost_WentBust, 0);
							lstPlayerOutcomes.Add(Helpers.FormatString("{0}: Bust with {1} and a bet of ${2}", participant.GetCharacterName(ENameType.StaticCharacterName), biggestPlayerValue, playerState.CurrentBet));
						}
						else
						{
							if (bWon)
							{
								int payOut = (int)((float)playerState.CurrentBet * 1.5f) + playerState.CurrentBet;
								LogOutcome(participant, biggestPlayerValue, activityState, playerState, EOutcomeLog.Won, payOut);
								lstPlayerOutcomes.Add(Helpers.FormatString("{0}: won ${1} with {2} and a bet of ${3}", participant.GetCharacterName(ENameType.StaticCharacterName), payOut, biggestPlayerValue, playerState.CurrentBet));
							}
							else if (bDraw)
							{
								LogOutcome(participant, biggestPlayerValue, activityState, playerState, EOutcomeLog.Draw, 0);
								lstPlayerOutcomes.Add(Helpers.FormatString("{0}: drew with {2} and had their ${1} bet returned.", participant.GetCharacterName(ENameType.StaticCharacterName), playerState.CurrentBet, biggestPlayerValue));
							}
							else
							{
								LogOutcome(participant, biggestPlayerValue, activityState, playerState, EOutcomeLog.Lost_WithLowerCards, 0);
								lstPlayerOutcomes.Add(Helpers.FormatString("{0}: lost ${1} with {2}", participant.GetCharacterName(ENameType.StaticCharacterName), playerState.CurrentBet, biggestPlayerValue));
							}

							// give player 50% of bet, and give them their original bet back (but integer it, so we don't give like 3 cents...)
							if (bWon)
							{
								int payOutNetGain = (int)(playerState.CurrentBet * 0.50f);
								int payOutWithBetIncluded = payOutNetGain + playerState.CurrentBet;

								// safety
								if (payOutNetGain >= 0 && payOutWithBetIncluded >= 0)
								{
									// subtract from dealer
									ModificationToDealer -= payOutNetGain;

									// if they already have a casino chip bucket, just increase the stack, otherwise give a new one
									bool bAddedCurrency = participant.Inventory.IncrementStackSizeOfFirstInstance(EItemID.CASINO_CHIP_BUCKET, (UInt32)payOutWithBetIncluded);
									if (!bAddedCurrency)
									{
										CItemInstanceDef casinoChipsDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.CASINO_CHIP_BUCKET, 0.0f, (UInt32)payOutWithBetIncluded);
										if (participant.Inventory.CanGiveItem(casinoChipsDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage, true))
										{
											participant.Inventory.AddItemToNextFreeSuitableSlot(casinoChipsDef, EShowInventoryAction.DoNothing, EItemID.None, null);
										}
									}

									participant.AwardAchievement(EAchievementID.WinBlackjack);

									// did we have 5 cards?
									if (playerState.Cards.Count >= ActivityConstants.BlackjackMaxCards)
									{
										participant.AwardAchievement(EAchievementID.GetFiveCardsAndWinBlackjack);
									}
								}
							}
							else if (bDraw)
							{
								// give the player their bet back

								// if they already have a casino chip bucket, just increase the stack, otherwise give a new one
								bool bAddedCurrency = participant.Inventory.IncrementStackSizeOfFirstInstance(EItemID.CASINO_CHIP_BUCKET, (UInt32)playerState.CurrentBet);
								if (!bAddedCurrency)
								{
									CItemInstanceDef casinoChipsDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.CASINO_CHIP_BUCKET, 0.0f, (UInt32)playerState.CurrentBet);
									if (participant.Inventory.CanGiveItem(casinoChipsDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage, true))
									{
										participant.Inventory.AddItemToNextFreeSuitableSlot(casinoChipsDef, EShowInventoryAction.DoNothing, EItemID.None, null);
									}
								}

								// NOTE: No modification to dealer here because they didn't lose or gain anything (and we don't pre-subtract currency like we do for players)
							}
							else
							{
								// give bet to dealer
								ModificationToDealer += playerState.CurrentBet;

								participant.AwardAchievement(EAchievementID.LoseBlackjack);
							}
						}
					}

					// save the dealer modification to the furniture item
					if (m_lstParticipants.Count > 0)
					{
						CPropertyFurnitureInstance furnInstance = ActivitySystem.GetFurnitureItemAssociatedWithActivityFromCurrentProperty(m_lstParticipants[0], m_uniqueIdentifier);

						if (furnInstance != null)
						{
							CItemValueFurniture furnValue = (CItemValueFurniture)furnInstance.Value;

							// if the house cannot cover ALL player bets, don't give them any winnings
							if (furnValue.ActivityCurrency >= totalBets)
							{
								// if negative, bankrupt / dont go negative
								if (ModificationToDealer < 0 && furnValue.ActivityCurrency < Math.Abs(ModificationToDealer))
								{
									ModificationToDealer = -furnValue.ActivityCurrency;
								}

								furnValue.ActivityCurrency += ModificationToDealer;
								Database.Functions.Items.SavePropertyFurnitureValue(furnInstance);
							}
						}
					}

					// send outcome to participants, this is for GUI so we don't send it to subscribers, only participants
					foreach (CPlayer participant in m_lstParticipants)
					{
						NetworkEventSender.SendNetworkEvent_Activity_RoundOutcome(participant, m_uniqueIdentifier, EActivityType.Blackjack, strDealerOutcome, lstPlayerOutcomes);
					}

					//SendActivityMessage("Dealer: We are starting a new round in {0} seconds!", (BlackjackTimeouts.timeToSpendInRoundOutcome / 1000));
					SetState(EBlackJackActivityState.DetermineRoundOutcome_Wait);
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_Wait:
				{
					if (GetMillisecondsInState() > BlackjackTimeouts.timeToSpendRetrievingCards)
					{
						SetState(EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player1);
					}

					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player1:
				{
					if (!HasParticipant(0, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeToSpendRetrievingCards)
					{
						SetState(EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player2);
					}
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player2:
				{
					if (!HasParticipant(1, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeToSpendRetrievingCards)
					{
						SetState(EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player3);
					}
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player3:
				{
					if (!HasParticipant(2, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeToSpendRetrievingCards)
					{
						SetState(EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player4);
					}
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player4:
				{
					if (!HasParticipant(3, false, out bool bHasBlackjack, out bool bIsBust, out CPlayer player) || GetMillisecondsInState() > BlackjackTimeouts.timeToSpendRetrievingCards)
					{
						SetState(EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Dealer);
					}
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Dealer:
				{
					if (GetMillisecondsInState() > BlackjackTimeouts.timeToSpendRetrievingCards)
					{
						SetState(EBlackJackActivityState.DetermineRoundOutcome_WaitingToGotoInactiveAndNextRound);
					}
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_WaitingToGotoInactiveAndNextRound:
				{
					if (GetMillisecondsInState() > BlackjackTimeouts.timeToSpendInRoundOutcome)
					{
						SetState(EBlackJackActivityState.Inactive);
					}
					break;
				}
		}
	}


	enum EOutcomeLog
	{
		Won,
		Lost_WithLowerCards,
		Lost_WentBust,
		Draw
	}

	private void LogOutcome(CPlayer participant, int biggestPlayerValue, BlackjackActivityState activityState, BlackjackPlayerState playerState, EOutcomeLog outcome, int payout)
	{
		new Logging.Log(participant, Logging.ELogType.BlackjackRoundOutcome, null, Helpers.FormatString("Player {0} Blackjack Outcome: {1} (BiggestPlayerValue: {2}, Bet: {3}, Cards: {4}, DealerCards: {5}, PayOut: {6})",
			participant.GetCharacterName(ENameType.StaticCharacterName), outcome.ToString(), biggestPlayerValue,
			playerState.CurrentBet, String.Join(", ", playerState.Cards.Select(x => x.Card.ToString()).ToArray()),
			String.Join(", ", activityState.DealerState.Cards.Select(x => x.Card.ToString()).ToArray()),
			payout)).execute();
	}
}