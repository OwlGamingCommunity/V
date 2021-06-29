public static class UserInputHelper
{
	public enum EUserInputType
	{
		TextBox,
		TextArea
	}

	static UserInputHelper()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += UserInput_OnClose;
	}

	public static void RequestUserInput(string strTitle, string strCaption, string strPlaceholder, UIEventID SubmitEvent, UIEventID CancelEvent, EPromptPosition position = EPromptPosition.Center, EUserInputType inputType = EUserInputType.TextBox, int maxLength = 9999, bool bHideChat = true)
	{
		m_GUI.SetVisible(true, true, bHideChat);

		m_GUI.ShowInputBox(strTitle, strCaption, strPlaceholder, SubmitEvent.ToString(), CancelEvent.ToString(), position, inputType == EUserInputType.TextArea, maxLength);
	}

	public static void UserInput_OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIUserInput m_GUI = new CGUIUserInput(OnUILoaded);
}

internal class CGUIUserInput : CEFCore
{
	public CGUIUserInput(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/userinput.html", EGUIID.UserInputHelpers, callbackOnLoad)
	{
		UIEvents.UserInput_OnClose += UserInputHelper.UserInput_OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ShowInputBox(string strTitle, string strCaption, string strPlaceholder, string strSubmitEventName, string strCancelEventName, EPromptPosition position, bool bIsTextArea, int maxLength)
	{
		Execute("ShowInputBox", strTitle, strCaption, strPlaceholder, strSubmitEventName, strCancelEventName, (int)position, bIsTextArea, maxLength);
	}
}