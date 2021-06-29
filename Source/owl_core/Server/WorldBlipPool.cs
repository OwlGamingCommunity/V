using GTANetworkAPI;
using System.Collections.Generic;
using System.Threading.Tasks;

using EntityDatabaseID = System.Int64;

public static class WorldBlipPool
{
	static WorldBlipPool()
	{

	}

	public static async Task<CWorldBlipInstance> CreateWorldBlip(EntityDatabaseID ID, string strName, int Sprite, int Color, Vector3 vecPos, bool bInsertIntoDB)
	{
		if (bInsertIntoDB)
		{
			ID = await Database.LegacyFunctions.CreateWorldBlip(strName, Sprite, Color, vecPos).ConfigureAwait(true);
		}

		CWorldBlipInstance newInst = new CWorldBlipInstance(ID, strName, Sprite, Color, vecPos);
		m_dictWorldBlips.Add(ID, newInst);
		return newInst;
	}

	public static async void DestroyWorldBlip(CWorldBlipInstance a_Inst, bool bDeleteFromDB)
	{
		if (bDeleteFromDB)
		{
			await Database.LegacyFunctions.DestroyWorldBlip(a_Inst.m_DatabaseID).ConfigureAwait(true);
		}

		a_Inst.Destroy();
		m_dictWorldBlips.Remove(a_Inst.m_DatabaseID);
	}

	public static Dictionary<EntityDatabaseID, CWorldBlipInstance> GetWorldBlips()
	{
		return m_dictWorldBlips;
	}

	public static CWorldBlipInstance GetInstanceFromID(EntityDatabaseID a_ID)
	{
		if (m_dictWorldBlips.ContainsKey(a_ID))
		{
			return m_dictWorldBlips[a_ID];
		}

		return null;
	}

	private static Dictionary<EntityDatabaseID, CWorldBlipInstance> m_dictWorldBlips = new Dictionary<EntityDatabaseID, CWorldBlipInstance>();
}