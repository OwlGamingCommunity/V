using System;

public class CustomMapLoader
{
	private CGUICustomMapLoader m_CustomMapUI = new CGUICustomMapLoader(OnUILoaded);
	private long m_PropertyID = 0;
	private float m_MarkerX = 0.0f;
	private float m_MarkerY = 0.0f;
	private float m_MarkerZ = 0.0f;

	// Transfer
	private const int MaxBytesPerSecond = 10240; // 10kb
	private int m_DataWriteOffset = 0;
	private byte[] m_MapDataBytes;
	private WeakReference<ClientTimer> m_TransferTimer = new WeakReference<ClientTimer>(null);

	private string m_TempMapType = string.Empty;

	public CustomMapLoader()
	{
		NetworkEvents.CustomInterior_OpenCustomIntUI += OnLoadCustomMapUI;
		NetworkEvents.CustomInterior_CustomMapTransfer_Reset += OnCustomMapTransferReset;
		UIEvents.CustomInterior_Confirmation_Yes += OnYes;
		UIEvents.CustomInterior_Confirmation_No += OnNo;

		UIEvents.MapLoader_CancelUpload += OnCancelUpload;
	}

	~CustomMapLoader()
	{

	}

	private bool HasPendingTransfer()
	{
		return m_MapDataBytes != null;
	}

	public void OnLoadCustomMapUI(long propertyID, string propertyName)
	{
		if (!HasPendingTransfer())
		{
			ItemSystem.GetPlayerInventory()?.HideInventory();

			m_CustomMapUI.SetVisible(true, true, false);
			m_CustomMapUI.Execute("SetUIData", propertyID, propertyName);

			m_PropertyID = propertyID;
		}
	}

	public void OnCloseCustomMapUI()
	{
		m_CustomMapUI.SetVisible(false, false, false);
	}

	private void OnTransferDataTick(object[] parameters)
	{
		int bytesRemaining = m_MapDataBytes.Length - m_DataWriteOffset;
		int bytesToSend = Math.Min(bytesRemaining, MaxBytesPerSecond);

		byte[] buffer = new byte[bytesToSend];
		Array.Copy(m_MapDataBytes, m_DataWriteOffset, buffer, 0, bytesToSend);
		NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_SendBytes(buffer);

		m_DataWriteOffset += bytesToSend;
		GUIUpdateUploadProgress();

		if (m_DataWriteOffset == m_MapDataBytes.Length)
		{
			OnTransferComplete();
		}
	}

	private void OnTransferComplete()
	{
		Reset();
	}

	public void ProcessCustomInterior(string mapData, string mapType, float markerX, float markerY, float markerZ)
	{
		Hide();

		byte[] mapDataBytes = Convert.FromBase64String(mapData);

		if (mapDataBytes.Length > MapTransferConstants.MaxMapSizeBytes)
		{
			GenericMessageBoxHelper.ShowMessageBox("Map Upload", Helpers.FormatString("That map is too big, the maximum map size is {0} KB", (int)(MapTransferConstants.MaxMapSizeBytes / 1024)), "OK", "");
		}
		else
		{
			m_DataWriteOffset = 0;
			m_MapDataBytes = Convert.FromBase64String(mapData);
			m_TempMapType = mapType;
			m_MarkerX = markerX;
			m_MarkerY = markerY;
			m_MarkerZ = markerZ;

			GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("Are you SURE you want to update the interior of {0} to a custom interior? It'll reduce GC's from your account.", m_PropertyID), "Yes, continue", "No! Quit the action", UIEventID.CustomInterior_Confirmation_Yes, UIEventID.CustomInterior_Confirmation_No);
		}
	}

	private void GUIUpdateUploadProgress()
	{
		int bytesRemaining = m_MapDataBytes.Length - m_DataWriteOffset;
		int secondsRemaining = (int)Math.Ceiling((float)bytesRemaining / (float)MaxBytesPerSecond);
		int percent = (int)(Math.Ceiling(((float)m_DataWriteOffset / (float)m_MapDataBytes.Length * 100.0f)));

		string strEstimatedTimeRemaining = Helpers.FormatString("{0} remaining (estimated)", Helpers.ConvertSecondsToTimeString(secondsRemaining));
		GenericProgressBar.UpdateCaption(Helpers.FormatString("Please wait while we upload your map... <br>{0}", strEstimatedTimeRemaining));

		GenericProgressBar.UpdateProgress(percent);
	}

	private void OnCancelUpload()
	{
		NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_Cancel();
		GenericMessageBoxHelper.ShowMessageBox("Map Upload", "You have canceled your map upload.", "Okay", "");
		Reset();
	}

	private void Hide()
	{
		ItemSystem.GetPlayerInventory()?.HideInventory();
		m_CustomMapUI.SetVisible(false, false, false);
	}

	private void OnCustomMapTransferReset()
	{
		Reset();
	}

	private void OnYes()
	{
		Hide();

		int crc32 = CRC32.ComputeHash(m_MapDataBytes);
		NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_Start(m_TempMapType, m_PropertyID, m_MarkerX, m_MarkerY, m_MarkerZ, m_MapDataBytes.Length, crc32);

		GenericProgressBar.ShowGenericProgressBar("Map Uploading", "Please wait while we upload your map...", 0, true, "Cancel Upload", UIEventID.MapLoader_CancelUpload);
		GUIUpdateUploadProgress();
		m_TransferTimer = ClientTimerPool.CreateTimer(OnTransferDataTick, 1000, 0);
	}

	private void OnNo()
	{
		Hide();
		Reset();
	}

	private void Reset()
	{
		GenericProgressBar.CloseAnyProgressBar();
		ClientTimerPool.MarkTimerForDeletion(ref m_TransferTimer);

		m_DataWriteOffset = 0;
		m_MapDataBytes = null;
		m_TempMapType = string.Empty;
		m_PropertyID = 0;
		m_MarkerX = 0.0f;
		m_MarkerY = 0.0f;
		m_MarkerZ = 0.0f;

		m_CustomMapUI.Execute("Reset");
	}

	private static void OnUILoaded() { }
}