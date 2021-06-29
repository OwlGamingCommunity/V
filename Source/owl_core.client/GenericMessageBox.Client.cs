public static class GenericMessageBoxHelper
{
	static GenericMessageBoxHelper()
	{

	}

	public static void Init()
	{
		NetworkEvents.ShowGenericMessageBox += OnNetworkShowGenericMessageBox;
		NetworkEvents.ChangeCharacterApproved += OnClose;

		UIEvents.ShowGenericMessageBox += ShowMessageBox;
	}

	private static void OnNetworkShowGenericMessageBox(string strMessage, string strCaption)
	{
		ShowMessageBox(strMessage, strCaption, "OK", "");
	}

	public static void ShowMessageBox(string strTitle, string strCaption, string strButtonText, string strHideEventName)
	{
		m_GUI.SetVisible(true, true, true);

		m_GUI.ShowGenericMessageBox(strTitle, strCaption, strButtonText, strHideEventName);
	}

	public static void OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	public static void CloseAnyMessageBox()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericMessageBox m_GUI = new CGUIGenericMessageBox(OnUILoaded);
}

internal class CGUIGenericMessageBox : CEFCore
{
	public CGUIGenericMessageBox(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericmessagebox.html", EGUIID.GenericMessageBox, callbackOnLoad, true) // Immune to nuking because we use this to display disconnection messages
	{
		UIEvents.GenericMessageBox_OnClose += GenericMessageBoxHelper.OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ShowGenericMessageBox(string strTitle, string strCaption, string strButtonText, string strHideEventName)
	{
		Execute("ShowGenericMessageBox", strTitle, strCaption, strButtonText, strHideEventName);
	}
}