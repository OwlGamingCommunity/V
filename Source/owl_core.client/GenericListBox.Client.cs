using System.Collections.Generic;

public static class GenericListbox
{
	static GenericListbox()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += OnClose;
	}

	public static void ShowGenericListbox(List<KeyValuePair<string, string>> dictListboxItems, string strTitle, string strCaption, string strConfirmText, string strCancelText, string strConfirmEventName, string strCancelEventName, string strListboxChangeEventName)
	{
		m_GUI.ClearListbox();

		foreach (var kvPair in dictListboxItems)
		{
			m_GUI.AddListboxItem(kvPair.Key, kvPair.Value);
		}

		m_GUI.CommitListboxItems();

		m_GUI.ShowGenericListbox(strTitle, strCaption, strConfirmText, strCancelText, strConfirmEventName, strCancelEventName, strListboxChangeEventName);

		m_GUI.SetVisible(true, true, true);
	}

	public static void OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericListbox m_GUI = new CGUIGenericListbox(OnUILoaded);
}

internal class CGUIGenericListbox : CEFCore
{
	public CGUIGenericListbox(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericlistbox.html", EGUIID.GenericListBox, callbackOnLoad)
	{
		UIEvents.GenericListBox_OnClose += GenericListbox.OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ClearListbox()
	{
		Execute("ClearListbox");
	}

	public void AddListboxItem(string strName, string strValue)
	{
		Execute("AddListboxItem", strName, strValue);
	}

	public void CommitListboxItems()
	{
		Execute("CommitListboxItems");
	}

	public void ShowGenericListbox(string strTitle, string strCaption, string strConfirmText, string strCancelText, string strConfirmEventName, string strCancelEventName, string strListboxChangeEventName)
	{
		Execute("ShowGenericListbox", strTitle, strCaption, strConfirmText, strCancelText, strConfirmEventName, strCancelEventName, strListboxChangeEventName);
	}
}