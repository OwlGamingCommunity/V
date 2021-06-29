using GTANetworkAPI;
using System;
using System.Collections.Generic;

using EntityDatabaseID = System.Int64;

public static class InformationMarkerPool
{
	public static void Initialize()
	{
		// player commands
		CommandManager.RegisterCommand("createinfomarker", "Create an information marker", new Action<CPlayer, CVehicle>(CreateInfoMarkerCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		// admin commands
		CommandManager.RegisterCommand("anearbyinfomarkers", "Returns a list of all nearby information markers", new Action<CPlayer, CVehicle>(Admin_NearbyInformationMarkersCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("adelnearbyinfomarkers", "Deletes the nearby information markers", new Action<CPlayer, CVehicle>(Admin_DeleteNearbyInformationMarkersCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("adelinfomarker", "Delete a specific information marker", new Action<CPlayer, CVehicle, EntityDatabaseID>(Admin_DeleteInformationMarkerCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.CreateInfoMarker_Response += OnCreateInfoMarker_Response;
		NetworkEvents.ReadInfoMarker += OnReadInfoMarker;
		NetworkEvents.DeleteInfoMarker += OnDeleteInfoMarker;
	}

	private static void Admin_NearbyInformationMarkersCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Information Markers:");

		foreach (var infoMarker in m_dictInformationMarkers)
		{
			CInformationMarkerInstance infoMarkerInst = infoMarker.Value;

			if (SenderPlayer.Client.Position.DistanceTo2D(infoMarkerInst.Position) <= InformationMarkerConstants.NEARBY_INFOMARKERS_LIMIT && SenderPlayer.Client.Dimension == infoMarkerInst.MarkerDimension)
			{
				Database.Functions.Characters.GetCharacterNameFromDBID(infoMarkerInst.OwnerCharacterID, (string strCharacterName) =>
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, Helpers.FormatString("InfoMarker ID: {2} - Note: {0} - Owner: {1}",
					Helpers.ColorString(255, 255, 255, "{0}", infoMarkerInst.strText), Helpers.ColorString(255, 255, 255, "{0}", strCharacterName), Helpers.ColorString(255, 255, 255, "{0}", infoMarkerInst.m_DatabaseID)));
				});
			}
		}
	}

	private static void Admin_DeleteNearbyInformationMarkersCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		List<EntityDatabaseID> lstInfoMarkersToRemove = new List<EntityDatabaseID>();
		foreach (var infoMarker in m_dictInformationMarkers)
		{
			CInformationMarkerInstance infoMarkerInst = infoMarker.Value;

			if (SenderPlayer.Client.Position.DistanceTo2D(infoMarkerInst.Position) <= InformationMarkerConstants.NEARBY_INFOMARKERS_LIMIT && SenderPlayer.Client.Dimension == infoMarkerInst.MarkerDimension)
			{
				lstInfoMarkersToRemove.Add(infoMarker.Key);
				infoMarkerInst.Destroy(true);
			}
		}

		foreach (var infoMarkerID in lstInfoMarkersToRemove)
		{
			m_dictInformationMarkers.Remove(infoMarkerID);
		}

		SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Deleted {0} nearby info markers.", lstInfoMarkersToRemove.Count);
		Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.AdminCommand, null, "/delnearbynotes - bulk deleted info markers");
	}

	private static void Admin_DeleteInformationMarkerCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, EntityDatabaseID infoMarkerID)
	{
		if (m_dictInformationMarkers.ContainsKey(infoMarkerID))
		{
			CInformationMarkerInstance infoMarkerInst = m_dictInformationMarkers[infoMarkerID];

			if (infoMarkerInst != null)
			{
				infoMarkerInst.Destroy(true);
			}

			m_dictInformationMarkers.Remove(infoMarkerID);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Deleted info marker with ID: {0}.", infoMarkerID);
			Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/adelinfomarker - deleted infomarker ID: {0}", infoMarkerID));
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Info Marker ID: {0} not found.", infoMarkerID);
		}
	}

	private static void CreateInfoMarkerCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		NetworkEventSender.SendNetworkEvent_CreateInfoMarker_Request(SenderPlayer);
	}

	private static void OnCreateInfoMarker_Response(CPlayer SenderPlayer, string strText, float fX, float fY, float fZ)
	{
		if (strText.Length > InformationMarkerConstants.MAX_INFOMARKER_TEXT_LENGTH)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "Info Markers can be {0} characters maximum only.", InformationMarkerConstants.MAX_INFOMARKER_TEXT_LENGTH);
			return;
		}

		CreateInformationMarker(-1, SenderPlayer.ActiveCharacterDatabaseID, new Vector3(fX, fY, fZ), SenderPlayer.Client.Dimension, strText, true);

		Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.InfoMarker, null, Helpers.FormatString("Create Information Marker - '{0}' ", strText));
	}

	public static void CreateInformationMarker(EntityDatabaseID a_ID, EntityDatabaseID a_OwnerCharacterID, Vector3 position, uint dimension, string strText, bool a_bMakeDatabaseEntry, Action<CInformationMarkerInstance> Callback = null)
	{
		if (a_bMakeDatabaseEntry)
		{
			Database.Functions.Items.CreateInfoMarker(a_OwnerCharacterID, strText, position, dimension, (EntityDatabaseID dbid) =>
			{
				a_ID = dbid;
				CreateInstanceInternal();
			});
		}
		else
		{
			CreateInstanceInternal();
		}

		void CreateInstanceInternal()
		{
			CInformationMarkerInstance newInst = new CInformationMarkerInstance(a_ID, a_OwnerCharacterID, position, dimension, strText);

			m_dictInformationMarkers.Add(a_ID, newInst);
			Callback?.Invoke(newInst);
		}
	}

	public static void DestroyInformationMarker(CInformationMarkerInstance a_Inst)
	{
		try
		{
			NAPI.Task.Run(() =>
			{
				a_Inst.Destroy(true);
				a_Inst.Cleanup();
				m_dictInformationMarkers.Remove(a_Inst.m_DatabaseID);
			});
		}
		catch
		{

		}
	}

	public static CInformationMarkerInstance GetInformationMarkerInstanceFromID(EntityDatabaseID a_ID)
	{
		if (a_ID == 0)
		{
			return null;
		}

		CInformationMarkerInstance inst = null;
		if (m_dictInformationMarkers.ContainsKey(a_ID))
		{
			inst = m_dictInformationMarkers[a_ID];
		}

		return inst;
	}

	private static void OnReadInfoMarker(CPlayer a_Player, EntityDatabaseID a_ID)
	{
		CInformationMarkerInstance infoMarkerInst = GetInformationMarkerInstanceFromID(a_ID);
		if (infoMarkerInst != null)
		{
			NetworkEventSender.SendNetworkEvent_ReadInfoMarker_Response(a_Player, infoMarkerInst.m_DatabaseID, infoMarkerInst.OwnerCharacterID == a_Player.ActiveCharacterDatabaseID, infoMarkerInst.strText);
		}
	}

	private static void OnDeleteInfoMarker(CPlayer a_Player, EntityDatabaseID a_ID)
	{
		CInformationMarkerInstance infoMarkerInst = GetInformationMarkerInstanceFromID(a_ID);
		if (infoMarkerInst != null)
		{
			infoMarkerInst.Destroy(true);

			m_dictInformationMarkers.Remove(a_ID);
			a_Player.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Your information marker has been deleted.");

			Logging.Log.CreateLog(a_Player, Logging.ELogType.InfoMarker, null, Helpers.FormatString("Delete Owned Information Marker - '{0}' ", a_ID));
		}
	}

	private static Dictionary<EntityDatabaseID, CInformationMarkerInstance> m_dictInformationMarkers = new Dictionary<EntityDatabaseID, CInformationMarkerInstance>();
}