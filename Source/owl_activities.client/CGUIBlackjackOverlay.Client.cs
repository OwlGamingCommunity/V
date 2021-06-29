internal class CGUIBlackjackOverlay : CEFCore
{
	public CGUIBlackjackOverlay(OnGUILoadedDelegate callbackOnLoad) : base("owl_activities.client/blackjack_overlay.html", EGUIID.TaxiUI, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void ResetOverlay()
	{
		Execute("ResetOverlay");
	}

	public void AddParticipantCard(ECardSuite cardSuite, ECard card, bool bFaceUp)
	{
		Execute("AddParticipantCard", cardSuite.ToString().ToLower(), card.ToString().ToLower(), bFaceUp);
	}

	public void SetParticipantDetails(string strName, string cardTotal, int bet)
	{
		Execute("SetParticipantDetails", strName, cardTotal, bet);
	}

	public void SetTimeRemaining(string strTimeString)
	{
		Execute("SetTimeRemaining", strTimeString);
	}

	public void SetWaitingText(string strWaitingText)
	{
		Execute("SetWaitingText", strWaitingText);
	}

	public void SetActionsVisible(bool bVisible)
	{
		Execute("SetActionsVisible", bVisible);
	}

	public void ShowOutcome(string html)
	{
		Execute("ShowOutcome", html);
	}
}