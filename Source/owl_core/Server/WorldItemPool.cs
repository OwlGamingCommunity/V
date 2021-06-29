using GTANetworkAPI;
using System.Collections.Generic;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

public static class WorldItemPool
{
	public static void CreateWorldItem(CItemInstanceDef a_ItemInstance, Vector3 a_vecPos, float a_fRotZ, EntityDatabaseID a_DroppedByID, Dimension a_Dimension, float a_fRotX = -90.0f, float a_fRotY = 0.0f)
	{
		NAPI.Task.Run(() =>
		{
			CWorldItem worldItem = new CWorldItem(a_ItemInstance, a_vecPos, a_fRotZ, a_DroppedByID, a_Dimension, a_fRotX, a_fRotY);
			m_LookupTableID[a_ItemInstance.DatabaseID] = worldItem;
			m_LookupTableInstance[worldItem.GTAInstance.Handle] = worldItem;
		});
	}

	public static void DestroyWorldItem(CWorldItem a_WorldItemInst)
	{
		if (a_WorldItemInst != null)
		{
			m_LookupTableID.Remove(a_WorldItemInst.m_DatabaseID);

			if (a_WorldItemInst.GTAInstance != null)
			{
				m_LookupTableInstance.Remove(a_WorldItemInst.GTAInstance.Handle);
			}

			a_WorldItemInst.Destroy();
			a_WorldItemInst.Cleanup();
		}
	}

	public static ICollection<CWorldItem> GetAllWorldItems()
	{
		return m_LookupTableID.Values;
	}

	public static CWorldItem GetWorldItemFromID(EntityDatabaseID a_DBID)
	{
		if (m_LookupTableID.ContainsKey(a_DBID))
		{
			return (CWorldItem)m_LookupTableID[a_DBID];
		}

		return null;
	}

	public static CWorldItem GetWorldItemFromGTAInstance(GTANetworkAPI.Object GTAInstance)
	{
		if (m_LookupTableInstance.ContainsKey(GTAInstance.Handle))
		{
			return (CWorldItem)m_LookupTableInstance[GTAInstance.Handle];
		}

		return null;
	}

	public static CWorldItem GetWorldItemFromGTAInstanceHandle(NetHandle GTAInstanceHandle)
	{
		if (m_LookupTableInstance.ContainsKey(GTAInstanceHandle))
		{
			return (CWorldItem)m_LookupTableInstance[GTAInstanceHandle];
		}

		return null;
	}

	private static Dictionary<EntityDatabaseID, CWorldItem> m_LookupTableID = new Dictionary<EntityDatabaseID, CWorldItem>();
	private static Dictionary<NetHandle, CWorldItem> m_LookupTableInstance = new Dictionary<NetHandle, CWorldItem>();
}