using System.Collections.Generic;
using System.Linq;

using EntityDatabaseID = System.Int64;

public static class RadioPool
{
	private static List<RadioInstance> m_lstRadios = new List<RadioInstance>();

	static RadioPool()
	{

	}

	public static void Init(List<RadioInstance> lstRadios)
	{
		m_lstRadios = lstRadios;

		// Send all instances on join
		NetworkEvents.OnPlayerConnected += (CPlayer a_Player) => { NetworkEventSender.SendNetworkEvent_SyncAllRadios(a_Player, m_lstRadios); };
	}

	public static void Add(RadioInstance newRadio)
	{
		m_lstRadios.Add(newRadio);
		NetworkEventSender.SendNetworkEvent_SyncAllRadios_ForAll_IncludeEveryone(m_lstRadios);
	}

	public static async void DestroyRadio(RadioInstance radioInst, bool bRemoveFromDB)
	{
		await Database.LegacyFunctions.RemoveRadio(radioInst.ID).ConfigureAwait(true);
		m_lstRadios.Remove(radioInst);

		NetworkEventSender.SendNetworkEvent_SyncAllRadios_ForAll_IncludeEveryone(m_lstRadios);
	}

	public static void RemoveRange(List<RadioInstance> lstRadiosToRemove)
	{
		foreach (RadioInstance inst in lstRadiosToRemove)
		{
			m_lstRadios.Remove(inst);
		}

		NetworkEventSender.SendNetworkEvent_SyncAllRadios_ForAll_IncludeEveryone(m_lstRadios);
	}

	public static RadioInstance GetRadioFromID(EntityDatabaseID radioID)
	{
		return m_lstRadios.Where(radio => radio.ID == radioID).Single();
	}

	public static List<RadioInstance> GetRadiosFromPlayer(CPlayer a_Player)
	{
		return m_lstRadios.Where(radio => radio.Account == a_Player.AccountID).ToList();
	}

	public static RadioInstance GetRadioFromIDForPlayer(CPlayer a_Player, int a_ID)
	{
		RadioInstance radioInst = null;
		List<RadioInstance> lstPlayerRadios = GetRadiosFromPlayer(a_Player);

		// Get player radios and then look for one with this ID, this verifies ownership too
		foreach (RadioInstance radio in lstPlayerRadios)
		{
			if (radio.ID == a_ID)
			{
				radioInst = radio;
				break;
			}
		}

		return radioInst;
	}

	public static List<RadioInstance> GetRadios()
	{
		return m_lstRadios;
	}
}