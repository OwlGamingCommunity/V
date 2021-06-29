using System;
using System.Collections.Generic;
using System.Linq;

public class BlackjackActivityManager : ActivityManager
{
	private CGUIBlackjackOverlay m_BlackjackOverlay = new CGUIBlackjackOverlay(() => { });

	public BlackjackActivityManager()
	{
		NetworkEvents.Blackjack_PlaceBet_GotDetails += ShowPlaceBet;
	}

	public static void ShowPlaceBet(int totalChips)
	{
		UserInputHelper.RequestUserInput("Place Bet", Helpers.FormatString("Enter your bet (You have {0} chips)", totalChips), "10", UIEventID.Blackjack_PlaceBet_Submit, UIEventID.Blackjack_PlaceBet_Cancel, EPromptPosition.Center, UserInputHelper.EUserInputType.TextBox, 9999, false);
	}

	public void ShowOverlay()
	{
		m_BlackjackOverlay.SetVisible(true, false, false);
	}

	public void HideOverlay()
	{
		m_BlackjackOverlay.ResetOverlay();
		m_BlackjackOverlay.SetVisible(false, false, false);
	}

	public void UpdateOverlayTimeRemaining(string timeRemaining)
	{
		m_BlackjackOverlay.SetTimeRemaining(timeRemaining);
	}

	public void UpdateOverlayWaitingText(string strWaitingText)
	{
		m_BlackjackOverlay.SetWaitingText(strWaitingText);
	}

	public void ShowOutcome(string strDealerOutcome, List<string> lstPlayerOutcomes)
	{
		string strHTML = strDealerOutcome;
		foreach (string strOutcome in lstPlayerOutcomes)
		{
			strHTML += "<br>";
			strHTML += strOutcome;
		}
		m_BlackjackOverlay.ShowOutcome(strHTML);
	}

	public void UpdateOverlayToParticipant(BlackjackActivityInstance inst, int participantIndex, bool bShowPlayerActions)
	{
		BlackjackActivityState state = inst.GetActivityState();

		m_BlackjackOverlay.ResetOverlay();

		if (bShowPlayerActions)
		{
			CursorManager.SetCursorVisible(true, this);
		}
		else
		{
			CursorManager.SetCursorVisible(false, this);
		}

		m_BlackjackOverlay.SetActionsVisible(bShowPlayerActions);

		if (state != null)
		{
			List<CasinoCard> lstCards = null;

			string strParticipantName = String.Empty;
			int currentBet = 0;

			if (participantIndex == -1)
			{
				lstCards = state.DealerState.Cards;

				strParticipantName = "Dealer";
				currentBet = 0;
			}
			else
			{
				if (participantIndex < state.PlayerStates.Count)
				{
					var playerState = state.PlayerStates.ElementAt(participantIndex).Value;
					lstCards = playerState.Cards;

					RAGE.Elements.Player player = inst.GetPlayerFromIndex(participantIndex);

					strParticipantName = player.Name;
					currentBet = playerState.CurrentBet;
				}
			}

			if (lstCards != null && !String.IsNullOrEmpty(strParticipantName))
			{
				foreach (var cards in lstCards)
				{
					m_BlackjackOverlay.AddParticipantCard(cards.Suite, cards.Card, cards.FaceUp);
				}

				BlackjackActivityHelpers.CalculateValueOfCards(lstCards, out List<string> lstDisplays, out List<int> lstValues, out List<int> lstValuesWithinMaxRange, out string strValidCardCombinations, out int highestValueWithinMaxRange);

				m_BlackjackOverlay.SetParticipantDetails(strParticipantName, strValidCardCombinations, currentBet);
			}
		}
	}

	protected override ActivityInstance CreateNewInstance_Internal(Int64 uniqueIdentifier)
	{
		return new BlackjackActivityInstance(uniqueIdentifier);
	}
}