using GTANetworkAPI;
using System.Collections.Generic;
using System.Threading.Tasks;

using EntityDatabaseID = System.Int64;

public static class DancerPool
{
	static DancerPool()
	{
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
	}

	public static void OnPlayerConnected(CPlayer a_pPlayer)
	{
		// Send all instances
		foreach (var dancerInst in m_dictDancerInstances)
		{
			dancerInst.Value.SendToPlayer(a_pPlayer);
		}
	}

	public static async Task<CDancerInstance> CreateDancer(EntityDatabaseID dancerID, Vector3 vecPos, float fRot, uint dancerSkin, uint dimension, float fTipMoney, string animDict, string animName, bool bAllowTip, EntityDatabaseID parentPropertyID, bool bInsertIntoDB)
	{
		if (bInsertIntoDB)
		{
			dancerID = await Database.LegacyFunctions.CreateDancer(vecPos, fRot, dancerSkin, bAllowTip, dimension, parentPropertyID, animDict, animName).ConfigureAwait(true);

		}

		CDancerInstance newInst = new CDancerInstance(dancerID, vecPos, fRot, dancerSkin, dimension, fTipMoney, animDict, animName, bAllowTip, parentPropertyID);
		m_dictDancerInstances.Add(dancerID, newInst);
		return newInst;
	}

	public static async void DestroyDancer(CDancerInstance a_Inst, bool bDeleteFromDB)
	{
		if (bDeleteFromDB)
		{
			await Database.LegacyFunctions.DestroyDancer(a_Inst.m_DatabaseID).ConfigureAwait(true);
		}

		a_Inst.Destroy();
		m_dictDancerInstances.Remove(a_Inst.m_DatabaseID);
	}

	public static Dictionary<EntityDatabaseID, CDancerInstance> GetDancers()
	{
		return m_dictDancerInstances;
	}

	public static CDancerInstance GetInstanceFromID(EntityDatabaseID a_ID)
	{
		if (m_dictDancerInstances.ContainsKey(a_ID))
		{
			return m_dictDancerInstances[a_ID];
		}

		return null;
	}

	private static Dictionary<EntityDatabaseID, CDancerInstance> m_dictDancerInstances = new Dictionary<EntityDatabaseID, CDancerInstance>();
}