// NOTE: Not all UI's may respect this, you might need to add it in
public enum EPromptPosition
{
	Center,
	Center_Bottom,
	Center_Left,
	Bottom_Right
}

public static class GenericPromptHelper
{
	static GenericPromptHelper()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += OnClose;
	}

	public static void ShowPrompt(string strTitle, string strCaption, string strYesText, string strNoText, UIEventID YesEvent, UIEventID NoEvent, EPromptPosition position = EPromptPosition.Center, bool bHideChat = true)
	{
		m_GUI.SetVisible(true, true, bHideChat);

		m_GUI.ShowGenericPrompt(strTitle, strCaption, strYesText, strNoText, YesEvent.ToString(), NoEvent.ToString(), position);
	}

	public static void OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericPrompt m_GUI = new CGUIGenericPrompt(OnUILoaded);
}

internal class CGUIGenericPrompt : CEFCore
{
	public CGUIGenericPrompt(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericprompt.html", EGUIID.GenericPrompt, callbackOnLoad)
	{
		UIEvents.GenericPrompt_OnClose += GenericPromptHelper.OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ShowGenericPrompt(string strTitle, string strCaption, string strYesText, string strNoText, string strYesEventName, string strNoEventName, EPromptPosition position)
	{
		Execute("ShowGenericPrompt", strTitle, strCaption, strYesText, strNoText, strYesEventName, strNoEventName, (int)position);
	}
}