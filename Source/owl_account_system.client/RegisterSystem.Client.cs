public static class RegisterSystem
{
	static RegisterSystem()
	{

	}

	public static void Init()
	{
		// EVENTS
		NetworkEvents.RegisterResult += OnRegisterResult;

		UIEvents.DoRegister += DoRegister;
		UIEvents.GoBackToRegister += CancelRegister;

		UIEvents.HideRegisterSuccessMessageBox += () =>
		{
			LoginSystem.GotoLogin(true);
		};
	}

	public static void ShowRegister()
	{
		g_RegisterScreenUI.SetVisible(true, true, false);

		// TODO_CSHARP: Better way of doing this, they're mostly tied together... and it would suck if someone forgot one of them
		g_RegisterScreenUI.SetCursorAndInputEnabled(true);
		g_RegisterScreenUI.Reset();

		DiscordManager.SetDiscordStatus("Registering Account");
	}

	private static void DoRegister(string strUsername, string strPassword, string strPasswordVerify, string strEmail)
	{
		// TODO_CSHARP: Camera doesnt change when going from camera to tutorial
		NetworkEventSender.SendNetworkEvent_RegisterPlayer(strUsername, strPassword, strPasswordVerify, strEmail);
	}

	private static void OnRegisterResult(bool bSuccess, string strError, string[] strErrorDetails)
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "1st_Person_Transition", "PLAYER_SWITCH_CUSTOM_SOUNDSET", true);

		if (!bSuccess)
		{
			g_RegisterScreenUI.HandleRegisterError(strError, strErrorDetails);
		}
		else
		{
			HideRegister();
			GenericMessageBoxHelper.ShowMessageBox("Account Registered", "Your account has been registered.\n\nYou must activate your account before logging in.\nPlease check your email for an activation email.", "Continue", "HideRegisterSuccessMessageBox");
		}
	}

	private static void HideRegister()
	{
		g_RegisterScreenUI.SetVisible(false, false, false);
	}

	private static void CancelRegister()
	{
		HideRegister();
		LoginSystem.GotoLogin(true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIRegisterScreen g_RegisterScreenUI = new CGUIRegisterScreen(OnUILoaded);
}

