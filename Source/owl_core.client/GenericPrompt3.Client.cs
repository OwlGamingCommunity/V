public static class GenericPrompt3Helper
{
	static GenericPrompt3Helper()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += OnClose;
	}

	public static void ShowPrompt(string strTitle, string strCaption, string strLeftText, string strCenterText, string strRightText, UIEventID LeftEventName, UIEventID CenterEventName, UIEventID RightEventName, EPromptPosition position = EPromptPosition.Center)
	{
		m_GUI.SetVisible(true, true, true);

		m_GUI.ShowGenericPrompt(strTitle, strCaption, strLeftText, strCenterText, strRightText, LeftEventName, CenterEventName, RightEventName, position);
	}

	public static void OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericPrompt3 m_GUI = new CGUIGenericPrompt3(OnUILoaded);
}

internal class CGUIGenericPrompt3 : CEFCore
{
	public CGUIGenericPrompt3(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericprompt3.html", EGUIID.GenericPrompt3, callbackOnLoad)
	{
		UIEvents.GenericPrompt3_OnClose += GenericPrompt3Helper.OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ShowGenericPrompt(string strTitle, string strCaption, string strLeftText, string strCenterText, string strRightText, UIEventID LeftEventName, UIEventID CenterEventName, UIEventID RightEventName, EPromptPosition position)
	{
		Execute("ShowGenericPrompt", strTitle, strCaption, strLeftText, strCenterText, strRightText, LeftEventName.ToString(), CenterEventName.ToString(), RightEventName.ToString(), (int)position);
	}
}