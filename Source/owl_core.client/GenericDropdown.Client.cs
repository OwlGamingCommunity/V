using System.Collections.Generic;

public static class GenericDropdown
{
	static GenericDropdown()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += OnClose;
	}

	public static void ShowGenericDropdown(string strDefaultText, Dictionary<string, string> dictDropdownItems, string strTitle, string strCaption, string strConfirmText, string strCancelText, UIEventID ConfirmEvent, UIEventID CancelEvent, UIEventID DropdownChangeEvent, EPromptPosition position = EPromptPosition.Center)
	{
		m_GUI.ClearDropdown(strDefaultText);

		foreach (var kvPair in dictDropdownItems)
		{
			m_GUI.AddDropdownItem(kvPair.Key, kvPair.Value);
		}

		m_GUI.ShowGenericDropdown(strTitle, strCaption, strConfirmText, strCancelText, ConfirmEvent, CancelEvent, DropdownChangeEvent, position);

		m_GUI.SetVisible(true, true, true);
	}

	public static void OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericDropdown m_GUI = new CGUIGenericDropdown(OnUILoaded);
}

internal class CGUIGenericDropdown : CEFCore
{
	public CGUIGenericDropdown(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericdropdown.html", EGUIID.GenericDropdown, callbackOnLoad)
	{
		UIEvents.GenericDropdown_OnClose += GenericDropdown.OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ClearDropdown(string strDefaultText)
	{
		Execute("ClearDropdown", strDefaultText);
	}

	public void AddDropdownItem(string strName, string strValue)
	{
		Execute("AddDropdownItem", strName, strValue);
	}

	public void ShowGenericDropdown(string strTitle, string strCaption, string strConfirmText, string strCancelText, UIEventID ConfirmEvent, UIEventID CancelEvent, UIEventID DropdownChangeEvent, EPromptPosition position)
	{
		Execute("ShowGenericDropdown", strTitle, strCaption, strConfirmText, strCancelText, ConfirmEvent.ToString(), CancelEvent.ToString(), DropdownChangeEvent.ToString(), (int)position);
	}
}