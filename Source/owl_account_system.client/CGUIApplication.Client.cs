using System.Collections.Generic;

internal class CGUIApplication : CEFCore
{
	public CGUIApplication(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/application.html", EGUIID.Application, callbackOnLoad)
	{
		UIEvents.QuizComplete += OnQuizComplete;
		UIEvents.RequestWrittenQuestions += OnRequestWrittenQuestions;
		UIEvents.RestartQuiz += OnRestartQuiz;
		UIEvents.SubmitWrittenPortion += OnSubmitWrittenPortion;
	}

	public override void OnLoad()
	{

	}

	public void SetLogoutVisible(bool bVisible)
	{
		Execute("SetLogoutVisible", bVisible);
	}

	private void OnQuizComplete(List<int> lstResponseIndexes)
	{
		NetworkEventSender.SendNetworkEvent_QuizComplete(lstResponseIndexes);
	}

	private void OnRequestWrittenQuestions()
	{
		NetworkEventSender.SendNetworkEvent_RequestWrittenQuestions();
	}

	private void OnRestartQuiz()
	{
		NetworkEventSender.SendNetworkEvent_RequestQuizQuestions();
	}

	private void OnSubmitWrittenPortion(string strQ1Answer, string strQ2Answer, string strQ3Answer, string strQ4Answer)
	{
		NetworkEventSender.SendNetworkEvent_SubmitWrittenPortion(strQ1Answer, strQ2Answer, strQ3Answer, strQ4Answer);
	}

	public void ContinueToNextStep()
	{
		Execute("ContinueToNextStep");
	}

	public void GotoApplicationPendingState()
	{
		Execute("GotoApplicationPendingState");

		SetLogoutVisible(true);
	}

	public void GotoApplicationRejectedState()
	{
		Execute("GotoApplicationRejectedState");
	}

	public void AddQuestion(string strQuestion, string strAnswer1, string strAnswer2, string strAnswer3, string strAnswer4)
	{
		Execute("AddQuestion", strQuestion, strAnswer1, strAnswer2, strAnswer3, strAnswer4);
	}

	public void CommitQuestions()
	{
		Execute("CommitQuestions");
	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void AddWrittenQuestion(string strQuestion)
	{
		Execute("AddWrittenQuestion", strQuestion);
	}

	public void CommitWrittenQuestions()
	{
		Execute("CommitWrittenQuestions");
	}

	public void OnQuizCompleteResponse(bool bPassed, int numCorrect, int numIncorrect)
	{
		Execute("OnQuizComplete_Response", bPassed, numCorrect, numIncorrect);
	}
}