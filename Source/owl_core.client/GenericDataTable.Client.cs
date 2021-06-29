using System.Collections.Generic;


public static class GenericDataTable
{
	static GenericDataTable()
	{

	}

	public static void Init()
	{
		NetworkEvents.ChangeCharacterApproved += OnClose;
	}

	public static void ShowGenericDataTable(List<string> lstTableHeaders, List<List<string>> dictTableRows, string strTitle, string strExitButtonText, UIEventID ExitEventID)
	{
		m_GUI.ClearDatabox();

		int headerIndex = 0;
		foreach (string strHeader in lstTableHeaders)
		{
			m_GUI.AddDatatableHeaders(headerIndex, strHeader);
			++headerIndex;
		}

		int rowIndex = 0;
		foreach (List<string> lstRowItems in dictTableRows)
		{
			int columnIndex = 0;
			foreach (string strColumnEntry in lstRowItems)
			{
				m_GUI.AddDatatableEntry(rowIndex, columnIndex, strColumnEntry);
				++columnIndex;
			}
			++rowIndex;
		}

		m_GUI.CommitDataTableItems();

		m_GUI.ShowDataTable(strTitle, strExitButtonText, ExitEventID);

		m_GUI.SetVisible(true, true, true);
	}

	public static void OnClose()
	{
		m_GUI.SetVisible(false, false, true);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIGenericDataTable m_GUI = new CGUIGenericDataTable(OnUILoaded);
}

internal class CGUIGenericDataTable : CEFCore
{
	public CGUIGenericDataTable(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/genericdatatable.html", EGUIID.GenericDataTable, callbackOnLoad)
	{
		UIEvents.GenericDataTable_OnClose += GenericDataTable.OnClose;
	}

	public override void OnLoad()
	{

	}

	public void ClearDatabox()
	{
		Execute("ClearDatabox");
	}

	public void AddDatatableHeaders(int columnID, string strName)
	{
		Execute("AddDatatableHeaders", columnID, strName);
	}

	public void AddDatatableEntry(int rowID, int columnID, string strText)
	{
		Execute("AddDatatableEntry", rowID, columnID, strText);
	}

	public void CommitDataTableItems()
	{
		Execute("CommitDataTableItems");
	}

	public void ShowDataTable(string strTitle, string strExitButtonText, UIEventID ExitEventID)
	{
		Execute("ShowDataTable", strTitle, strExitButtonText, ExitEventID.ToString());
	}
}