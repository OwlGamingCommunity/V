public static class GenericProgressBar
{
	static GenericProgressBar()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += OnClose;
	}

	public static void ShowGenericProgressBar(string strTitle, string strCaption, int bar_percentage, bool canUserClose, string close_button_text, UIEventID close_event)
	{
		m_GUI.SetVisible(true, true, true);

		m_GUI.ShowGenericProgressBar(strTitle, strCaption, bar_percentage, canUserClose, close_button_text, close_event);
	}

	public static void UpdateCaption(string strCaption)
	{
		m_GUI.UpdateCaption(strCaption);
	}

	public static void UpdateProgress(int bar_percentage)
	{
		m_GUI.UpdateProgress(bar_percentage);
	}

	public static void OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	public static void CloseAnyProgressBar(bool bAffectChat = true)
	{
		m_GUI.SetVisible(false, false, bAffectChat);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericProgressBar m_GUI = new CGUIGenericProgressBar(OnUILoaded);
}

internal class CGUIGenericProgressBar : CEFCore
{
	public CGUIGenericProgressBar(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericprogressbar.html", EGUIID.GenericMessageBox, callbackOnLoad, false)
	{
		UIEvents.GenericProgressBar_OnClose += GenericProgressBar.OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ShowGenericProgressBar(string strTitle, string strCaption, int bar_percentage, bool canUserClose, string close_button_text, UIEventID close_event)
	{
		Execute("ShowGenericProgressBar", strTitle, strCaption, bar_percentage, canUserClose, close_button_text, close_event.ToString());
	}

	public void UpdateCaption(string strCaption)
	{
		Execute("UpdateCaption", strCaption);
	}

	public void UpdateProgress(int bar_percentage)
	{
		Execute("UpdateProgress", bar_percentage);
	}
}