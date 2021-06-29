public static class ChatHelper
{
	static ChatHelper()
	{

	}

	public static void PushMessage(EChatChannel channel, string strFormat, params object[] parameters)
	{
		RAGE.Chat.Output(Helpers.FormatString("{0}{1}", (int)channel, Helpers.FormatString(strFormat, parameters)));
	}

	public static void DebugMessage(string strFormat, params object[] parameters)
	{
		if (DebugHelper.IsDebug())
		{
			RAGE.Ui.Console.Verbosity = RAGE.Ui.ConsoleVerbosity.Info;

			string strMessage = Helpers.FormatString("DEBUG: {0}", Helpers.FormatString(strFormat, parameters));
			RAGE.Ui.Console.Log(RAGE.Ui.ConsoleVerbosity.Info, strMessage, false, false);
			RAGE.Chat.Output(Helpers.FormatString("0{0}", strMessage));
		}
	}

	public static void ErrorMessage(string strFormat, params object[] parameters)
	{
		string strMessage = Helpers.FormatString("ERROR: {0}", Helpers.FormatString(strFormat, parameters));
		RAGE.Chat.Output(Helpers.FormatString("0{0}", strMessage));
		NotificationManager.ShowNotification("Error", strMessage, ENotificationIcon.ExclamationSign);
	}
}