using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class CRoadblock : CBaseEntity
{
	public GTANetworkAPI.Object GTAInstance => m_Object;

	private GTANetworkAPI.Object m_Object = null;

	public string DisplayName { get; }

	public CRoadblock(RoadblockDefinition roadblockDef, Vector3 vecPos, float fRotZ, uint Dimension, int entryID)
	{
		DisplayName = roadblockDef.DisplayName;
		m_Object = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(roadblockDef.HashKey), vecPos, new Vector3(0.0f, 0.0f, fRotZ), 255, Dimension);
		SetData(m_Object, EDataNames.OBJECT_TYPE, EObjectTypes.ROADBLOCK, EDataType.Synced);
		SetData(m_Object, EDataNames.ITEM_ID, entryID, EDataType.Synced);
	}

	~CRoadblock()
	{
		Destroy();
	}

	public void Destroy()
	{
		if (m_Object != null)
		{
			m_Object.Delete();
			m_Object = null;
		}
	}
}

public class RoadblockSystem
{
	private int m_NextID = 0;
	private Dictionary<int, CRoadblock> m_dictRoadblocks = new Dictionary<int, CRoadblock>();

	public RoadblockSystem()
	{
		CommandManager.RegisterCommand("rb", "Uses roadblock system", new Action<CPlayer, CVehicle>(RoadblockCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "roadblock", "roadblocks" });

		CommandManager.RegisterCommand("nearbyrb", "Shows nearby roadblocks", new Action<CPlayer, CVehicle>(Admin_NearbyRB), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("deleterb", "Deletes a roadblock by ID", new Action<CPlayer, CVehicle, int>(Admin_DeleteRB), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("deletenearbyrb", "Deletes nearby roadblocks within the specified radius", new Action<CPlayer, CVehicle, float>(Admin_DeleteNearbyRB), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("deleteallrb", "Deletes all roadblocks", new Action<CPlayer, CVehicle>(Admin_DeleteAllRB), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.Roadblock_PlaceNew += PlaceRoadblock;
		NetworkEvents.Roadblock_UpdateExisting += UpdateRoadblock;
		NetworkEvents.Roadblock_RemoveExisting += RemoveRoadblock;
	}

	private void Admin_NearbyRB(CPlayer player, CVehicle vehicle)
	{
		player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Roadblocks:");

		foreach (var kvPair in m_dictRoadblocks)
		{
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1}", kvPair.Key, kvPair.Value.DisplayName);
		}
	}

	private void Admin_DeleteRB(CPlayer player, CVehicle vehicle, int id)
	{
		if (m_dictRoadblocks.ContainsKey(id))
		{
			m_dictRoadblocks[id].Destroy();
			m_dictRoadblocks.Remove(id);
		}
	}

	private void Admin_DeleteNearbyRB(CPlayer player, CVehicle vehicle, float radius)
	{
		List<int> lstToRemove = new List<int>();
		foreach (var kvPair in m_dictRoadblocks)
		{
			float fDist = (kvPair.Value.GTAInstance.Position - player.Client.Position).Length();
			if (fDist <= radius)
			{
				player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - Removed #{0} - {1}", kvPair.Key, kvPair.Value.DisplayName);

				lstToRemove.Add(kvPair.Key);
			}
		}

		foreach (int roadblockToRemove in lstToRemove)
		{
			m_dictRoadblocks[roadblockToRemove].Destroy();
			m_dictRoadblocks.Remove(roadblockToRemove);
		}

		if (lstToRemove.Count == 0)
		{
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "No roadblocks were within that radius.");
		}
	}

	private void Admin_DeleteAllRB(CPlayer player, CVehicle vehicle)
	{
		foreach (var kvPair in m_dictRoadblocks)
		{
			kvPair.Value.Destroy();
		}

		m_dictRoadblocks.Clear();
	}

	private void RoadblockCommand(CPlayer player, CVehicle vehicle)
	{
		if (player.IsInFactionOfType(EFactionType.LawEnforcement)
			|| player.IsInFactionOfType(EFactionType.Medical) || player.IsAdmin(EAdminLevel.TrialAdmin, true))
		{
			if (vehicle == null)
			{
				NetworkEventSender.SendNetworkEvent_GotoRoadblockEditor(player);
			}
			else
			{
				player.SendNotification("Roadblock System", ENotificationIcon.ExclamationSign, "You must be on foot to perform this action.");
			}
		}
		else
		{
			player.SendNotification("Roadblock System", ENotificationIcon.ExclamationSign, "You must be in a Law Enforcement or FD EMS faction to perform this action.", null);
		}
	}

	private void PlaceRoadblock(CPlayer player, int descriptorIndex, float x, float y, float z)
	{
		if (descriptorIndex < Roadblocks.Definitions.Count)
		{
			RoadblockDefinition roadblockDef = Roadblocks.Definitions[descriptorIndex];
			player.SendNotification("Roadblock System", ENotificationIcon.ExclamationSign, "'{0}' was placed.", roadblockDef.DisplayName);

			CRoadblock newRoadblock = new CRoadblock(roadblockDef, new Vector3(x, y, z), player.Client.Rotation.Z, player.Client.Dimension, m_NextID);
			m_dictRoadblocks[m_NextID] = newRoadblock;
			++m_NextID;
		}
	}

	private void RemoveRoadblock(CPlayer player, int entryID)
	{
		if (m_dictRoadblocks.ContainsKey(entryID))
		{
			m_dictRoadblocks[entryID].Destroy();
			m_dictRoadblocks.Remove(entryID);
		}
	}

	private void UpdateRoadblock(CPlayer player, int entryID, float x, float y, float z, float rx, float ry, float rz)
	{
		if (m_dictRoadblocks.ContainsKey(entryID))
		{
			CRoadblock roadblock = m_dictRoadblocks[entryID];
			roadblock.GTAInstance.Position = new Vector3(x, y, z);
			roadblock.GTAInstance.Rotation = new Vector3(rx, ry, rz);
		}
	}
}