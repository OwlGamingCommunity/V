using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityDatabaseID = System.Int64;

public static class StorePool
{
	static StorePool()
	{
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
	}

	public static void OnPlayerConnected(CPlayer a_pPlayer)
	{
		// Send all instances
		foreach (var storeInst in m_dictStoreInstances)
		{
			storeInst.Value.SendToPlayer(a_pPlayer);
		}
	}

	public static async Task<CStoreInstance> CreateStore(EntityDatabaseID storeID, Vector3 vecPos, float fRot, EStoreType storeType, uint dimension, EntityDatabaseID parentPropertyID, Int64 lastRobbedAt, bool bInsertIntoDB)
	{
		if (bInsertIntoDB)
		{
			storeID = await Database.LegacyFunctions.CreateStore(vecPos, fRot, storeType, dimension, parentPropertyID).ConfigureAwait(true);
		}

		CStoreInstance newInst = new CStoreInstance(storeID, vecPos, fRot, storeType, dimension, parentPropertyID, lastRobbedAt);
		m_dictStoreInstances.Add(storeID, newInst);
		return newInst;
	}

	public static async void DestroyStore(CStoreInstance a_Inst, bool bDeleteFromDB)
	{
		if (bDeleteFromDB)
		{
			await Database.LegacyFunctions.DestroyStore(a_Inst.m_DatabaseID).ConfigureAwait(true);
		}

		a_Inst.Destroy();
		m_dictStoreInstances.Remove(a_Inst.m_DatabaseID);
	}

	public static Dictionary<EntityDatabaseID, CStoreInstance> GetStores()
	{
		return m_dictStoreInstances;
	}

	public static CStoreInstance GetInstanceFromID(EntityDatabaseID a_ID)
	{
		if (m_dictStoreInstances.ContainsKey(a_ID))
		{
			return m_dictStoreInstances[a_ID];
		}

		return null;
	}

	private static Dictionary<EntityDatabaseID, CStoreInstance> m_dictStoreInstances = new Dictionary<EntityDatabaseID, CStoreInstance>();
}