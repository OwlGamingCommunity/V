public static class GenericUserLoginBox
{
	static GenericUserLoginBox()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += GenericUserLoginBox_OnClose;
	}

	public static void RequestUserLogin(string strTitle, string strCaption, UIEventID SubmitEvent, UIEventID CancelEvent, int maxLength = 9999, bool bHideChat = true)
	{
		m_GUI.SetVisible(true, true, bHideChat);

		m_GUI.ShowLoginBox(strTitle, strCaption, SubmitEvent.ToString(), CancelEvent.ToString(), maxLength);
	}

	public static void GenericUserLoginBox_OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericUserLoginBox m_GUI = new CGUIGenericUserLoginBox(OnUILoaded);
}

internal class CGUIGenericUserLoginBox : CEFCore
{
	public CGUIGenericUserLoginBox(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericuserloginbox.html", EGUIID.GenericUserLogin, callbackOnLoad)
	{
		UIEvents.GenericUserLoginBox_OnClose += GenericUserLoginBox.GenericUserLoginBox_OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ShowLoginBox(string strTitle, string strCaption, string strSubmitEventName, string strCancelEventName, int maxLength)
	{
		Execute("ShowLoginBox", strTitle, strCaption, strSubmitEventName, strCancelEventName, maxLength);
	}
}