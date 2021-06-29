public class AdminCheckInt
{
	private CGUIAdminCheckInt m_AdminCheckIntUI = new CGUIAdminCheckInt(OnUILoaded);

	private AdminCheckInteriorDetails m_CachedDetails = null;
	private long m_cachedInterior = -1;

	public AdminCheckInt()
	{
		NetworkEvents.AdminCheckInt += OnAdminCheckInt;
		NetworkEvents.AdminReloadCheckIntDetails += OnAdminReloadCheckIntDetails;
	}

	private static void OnUILoaded() { }

	public void OnCloseCheckInt()
	{
		m_AdminCheckIntUI.SetVisible(false, false, false);
	}

	public void OnSaveInteriorNote(string strNote)
	{
		if (m_cachedInterior != -1 && !string.IsNullOrEmpty(strNote))
		{
			NetworkEventSender.SendNetworkEvent_SaveAdminInteriorNote(strNote, m_cachedInterior);
		}
	}

	public void OnAdminReloadCheckIntDetails(AdminCheckInteriorDetails interiorDetails)
	{
		m_CachedDetails = interiorDetails;
		ApplyCheckIntDetails();
	}

	public void OnReloadCheckIntData()
	{
		NetworkEventSender.SendNetworkEvent_ReloadCheckIntData(m_cachedInterior);
	}

	private void ApplyCheckIntDetails()
	{
		m_AdminCheckIntUI.Execute("SetAllData", OwlJSON.SerializeObject(m_CachedDetails, EJsonTrackableIdentifier.CheckIntDetails));
	}

	private void OnAdminCheckInt(long interior, AdminCheckInteriorDetails interiorDetails)
	{
		m_AdminCheckIntUI.SetVisible(true, true, false);

		m_cachedInterior = interior;
		m_CachedDetails = interiorDetails;

		ApplyCheckIntDetails();
	}
}