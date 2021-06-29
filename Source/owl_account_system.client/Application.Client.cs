using System.Collections.Generic;

public static class Application
{
	static Application()
	{

	}

	public static void Init()
	{
		// EVENTS
		NetworkEvents.ApplicationState += OnApplicationState;
		NetworkEvents.GotQuizQuestions += OnGotQuizQuestions;
		NetworkEvents.GotQuizWrittenQuestions += OnGotQuizWrittenQuestions;
		NetworkEvents.QuizResults += OnQuizResults;

		UIEvents.Logout += OnLogout;
	}

	private static void OnLogout()
	{
		HideApplication();
	}

	private static void OnQuizResults(bool bPassed, int numCorrect, int numIncorrect)
	{
		g_ApplicationUI.OnQuizCompleteResponse(bPassed, numCorrect, numIncorrect);
	}

	private static void OnGotQuizQuestions(List<CQuizQuestion> lstQuizQuestions)
	{
		int num_questions = lstQuizQuestions.Count;

		g_ApplicationUI.Reset();

		for (var i = 0; i < num_questions; ++i)
		{
			CQuizQuestion question = lstQuizQuestions[i];
			g_ApplicationUI.AddQuestion(question.Question, question.Answer1, question.Answer2, question.Answer3, question.Answer4);
		}

		g_ApplicationUI.CommitQuestions();

		g_ApplicationUI.SetVisible(true, true, false);
	}

	private static void OnGotQuizWrittenQuestions(List<CQuizWrittenQuestion> lstWrittenQuestions)
	{
		int num_questions = lstWrittenQuestions.Count;

		for (var i = 0; i < num_questions; ++i)
		{
			CQuizWrittenQuestion question = lstWrittenQuestions[i];
			g_ApplicationUI.AddWrittenQuestion(question.Question);
		}

		g_ApplicationUI.CommitWrittenQuestions();
		g_ApplicationUI.SetVisible(true, true, false);
	}

	private static void OnApplicationState(EApplicationState applicationState)
	{
		LoginSystem.HideLogin();

		g_ApplicationUI.SetVisible(true, true, false);

		// TODO_GITHUB: Replace CommunityName with your community name
		DiscordManager.SetDiscordStatus("Applying to Join CommunityName");

		if (applicationState == EApplicationState.NoApplicationSubmitted)
		{
			g_ApplicationUI.SetLogoutVisible(false);
			GotoApplicationStart();
		}
		else if (applicationState == EApplicationState.QuizCompleted)
		{
			// Goto written test
			g_ApplicationUI.SetInputEnabled(true);
			g_ApplicationUI.SetLogoutVisible(false);
			g_ApplicationUI.ContinueToNextStep();
		}
		else if (applicationState == EApplicationState.ApplicationPendingReview)
		{
			g_ApplicationUI.GotoApplicationPendingState();
		}
		else if (applicationState == EApplicationState.ApplicationRejected)
		{
			// TODO_LAUNCH: add reason
			g_ApplicationUI.SetLogoutVisible(false);
			g_ApplicationUI.GotoApplicationRejectedState();

			ClientTimerPool.CreateTimer(GotoApplicationStart, 5000, 1);
		}
		else if (applicationState == EApplicationState.ApplicationApproved)
		{
			g_ApplicationUI.SetLogoutVisible(false);
			HideApplication();
		}
	}

	private static void GotoApplicationStart(object[] a_Parameters = null)
	{
		NetworkEventSender.SendNetworkEvent_RequestQuizQuestions();
	}

	private static void HideApplication()
	{
		g_ApplicationUI.SetVisible(false, false, false);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIApplication g_ApplicationUI = new CGUIApplication(OnUILoaded);
}