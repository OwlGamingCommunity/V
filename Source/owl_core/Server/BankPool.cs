using GTANetworkAPI;
using System.Collections.Generic;
using System.Threading.Tasks;

using EntityDatabaseID = System.Int64;

public static class BankPool
{
	static BankPool()
	{
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
	}

	public static void OnPlayerConnected(CPlayer a_pPlayer)
	{
		// Send all instances
		foreach (var bankInst in m_dictBankInstances)
		{
			bankInst.Value.SendToPlayer(a_pPlayer);
		}
	}

	public static async Task<CBankInstance> CreateBank(EntityDatabaseID bankID, Vector3 vecPos, float fRot, EBankSystemType bankType, uint dimension, bool bInsertIntoDB)
	{
		if (bInsertIntoDB)
		{
			bankID = await Database.LegacyFunctions.CreateBank(vecPos, fRot, bankType, dimension).ConfigureAwait(true);
		}

		CBankInstance newInst = new CBankInstance(bankID, vecPos, fRot, bankType, dimension);
		m_dictBankInstances.Add(bankID, newInst);
		return newInst;
	}

	public static async void DestroyBank(CBankInstance a_Inst, bool bDeleteFromDB)
	{
		if (bDeleteFromDB)
		{
			await Database.LegacyFunctions.DestroyBank(a_Inst.m_DatabaseID).ConfigureAwait(true);
		}

		a_Inst.Destroy();
		m_dictBankInstances.Remove(a_Inst.m_DatabaseID);
	}

	public static Dictionary<EntityDatabaseID, CBankInstance> GetBanks()
	{
		return m_dictBankInstances;
	}

	public static CBankInstance GetBankByID(EntityDatabaseID ID)
	{
		return m_dictBankInstances.ContainsKey(ID) ? m_dictBankInstances[ID] : null;
	}

	private static Dictionary<EntityDatabaseID, CBankInstance> m_dictBankInstances = new Dictionary<EntityDatabaseID, CBankInstance>();
}