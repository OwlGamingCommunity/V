using System;
using EntityDatabaseID = System.Int64;

public class InformationMarkerSystem
{
	private EntityDatabaseID m_infoMarkerIDBeingOperatedUpon = -1;
	private string m_infoMarkerText = String.Empty;

	public InformationMarkerSystem()
	{
		NetworkEvents.CreateInfoMarker_Request += OnRequestCreateInfoMarker;
		NetworkEvents.ReadInfoMarker_Response += OnReadInfoMarker_Response;

		UIEvents.CreateInfoMarker_Submit += OnCreateInfoMarker_Submit;
		UIEvents.InfoMarkerOwned_Exit += OnInfoMarkerOwned_Exit;
		UIEvents.InfoMarkerOwned_Read += OnInfoMarkerOwned_Read;
		UIEvents.InfoMarkerOwned_Delete += OnInfoMarkerOwned_Delete;

		RageEvents.RAGE_OnRender += OnRender;
	}

	private void OnRequestCreateInfoMarker()
	{
		UserInputHelper.RequestUserInput("Create Info Marker", Helpers.FormatString("Enter your information marker text (Limit: {0} characters)", InformationMarkerConstants.MAX_INFOMARKER_TEXT_LENGTH), "Write Here",
			UIEventID.CreateInfoMarker_Submit, UIEventID.CreateInfoMarker_Submit, EPromptPosition.Center, UserInputHelper.EUserInputType.TextArea, InformationMarkerConstants.MAX_INFOMARKER_TEXT_LENGTH);
	}

	private void ShowReadInfoMarker(string strText)
	{
		GenericMessageBoxHelper.ShowMessageBox("Information Marker", strText, "Close", "");
	}

	private void ResetCachedData()
	{
		m_infoMarkerIDBeingOperatedUpon = -1;
		m_infoMarkerText = String.Empty;
	}

	private void OnReadInfoMarker_Response(EntityDatabaseID infoMarkerID, bool bIsCreator, string strText)
	{
		if (!bIsCreator)
		{
			// not needed
			ResetCachedData();
			ShowReadInfoMarker(strText);
		}
		else
		{
			m_infoMarkerIDBeingOperatedUpon = infoMarkerID;
			m_infoMarkerText = strText;

			GenericPrompt3Helper.ShowPrompt("Information Marker", "This information marker was created by you. What would you like to do?", "Exit", "Read", "Delete", UIEventID.InfoMarkerOwned_Exit, UIEventID.InfoMarkerOwned_Read, UIEventID.InfoMarkerOwned_Delete);
		}
	}

	private void OnInfoMarkerOwned_Read()
	{
		ShowReadInfoMarker(m_infoMarkerText);
	}

	private void OnInfoMarkerOwned_Delete()
	{
		NetworkEventSender.SendNetworkEvent_DeleteInfoMarker(m_infoMarkerIDBeingOperatedUpon);
		ResetCachedData();
	}

	private void OnInfoMarkerOwned_Exit()
	{
		ResetCachedData();
	}

	private void OnCreateInfoMarker_Submit(string strText)
	{
		RAGE.Vector3 vecGroundPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
		vecGroundPos.Z = WorldHelper.GetGroundPosition(vecGroundPos);

		NetworkEventSender.SendNetworkEvent_CreateInfoMarker_Response(strText, vecGroundPos.X, vecGroundPos.Y, vecGroundPos.Z);
	}

	private RAGE.Elements.Marker GetNearestInfoMarker()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.InfoMarker);
		return poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.Marker>() : null;
	}

	private void OnRender()
	{
		RAGE.Elements.Marker nearestInfoMarker = GetNearestInfoMarker();
		if (nearestInfoMarker != null)
		{
			WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Read Information Marker", null, OnReadInfoMarker, nearestInfoMarker.Position, nearestInfoMarker.Dimension, false, false);
		}
	}

	private void OnReadInfoMarker()
	{
		RAGE.Elements.Marker nearestInfoMarker = GetNearestInfoMarker();
		if (nearestInfoMarker != null)
		{
			Int64 nearestInfoMarkerID = DataHelper.GetEntityData<Int64>(nearestInfoMarker, EDataNames.INFO_MARKER_ID);
			NetworkEventSender.SendNetworkEvent_ReadInfoMarker(nearestInfoMarkerID);
		}
	}
}