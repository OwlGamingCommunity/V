using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

public static class FactionPool
{
	public static CFaction CreateFaction(EntityDatabaseID a_FactionID, EFactionType a_Type, string a_strName, bool a_bOfficial, string strShortName, string strMessage, float fMoney, List<CFactionRank> a_lstFactionRanks, EntityDatabaseID a_CreatorID)
	{
		CFaction newFaction = new CFaction(a_FactionID, a_Type, a_strName, a_bOfficial, strShortName, strMessage, fMoney, a_lstFactionRanks, a_CreatorID);
		m_LookupTable[a_FactionID] = newFaction;
		return newFaction;
	}

	public static void DestroyFaction(CFaction a_Instance)
	{
		a_Instance.Cleanup();
		a_Instance.OnDestroy();
		m_LookupTable.Remove(a_Instance.FactionID);
	}

	public static CFaction GetFactionFromID(EntityDatabaseID a_FactionID)
	{
		CFaction retVal = null;
		m_LookupTable.TryGetValue(a_FactionID, out retVal);
		return retVal;
	}

	public static CFaction GetFactionFromName(string a_strName)
	{
		CFaction factionToReturn = null;

		foreach (CFaction faction in m_LookupTable.Values)
		{
			if (faction.Name.Equals(a_strName))
			{
				factionToReturn = faction;
			}
		}

		return factionToReturn;
	}

	public static CFaction GetFactionFromShortName(string a_strShortName)
	{
		CFaction factionToReturn = null;

		foreach (CFaction faction in m_LookupTable.Values)
		{
			if (faction.ShortName.Equals(a_strShortName))
			{
				factionToReturn = faction;
			}
		}

		return factionToReturn;
	}

	public static Dictionary<EntityDatabaseID, CFaction> GetAllFactions()
	{
		return m_LookupTable;
	}

	public static CFaction GetPoliceFaction()
	{
		CFaction factionToReturn = null;
		long lowestID = -1;

		foreach (CFaction faction in m_LookupTable.Values)
		{
			if (faction.Type == EFactionType.LawEnforcement)
			{
				if (lowestID == -1 || faction.FactionID < lowestID)
				{
					lowestID = faction.FactionID;
					factionToReturn = faction;
				}
			}
		}

		return factionToReturn;
	}

	public static List<CFaction> GetTowingFactions()
	{
		return m_LookupTable.Values.Cast<CFaction>().Where(faction => faction.Type == EFactionType.Towing).ToList();
	}

	public static CFaction GetFDEMSFaction()
	{
		CFaction factionToReturn = null;
		long lowestID = -1;

		foreach (CFaction faction in m_LookupTable.Values)
		{
			if (faction.Type == EFactionType.Medical)
			{
				if (lowestID == -1 || faction.FactionID < lowestID)
				{
					lowestID = faction.FactionID;
					factionToReturn = faction;
				}
			}
		}

		return factionToReturn;
	}

	public static List<CFaction> GetGovernmentFactions()
	{
		List<CFaction> returnValue = new List<CFaction>();
		foreach (CFaction faction in m_LookupTable.Values)
		{
			if (faction.Type == EFactionType.LawEnforcement || faction.Type == EFactionType.Medical || faction.Type == EFactionType.Government)
			{
				returnValue.Add(faction);
			}
		}

		return returnValue;
	}

	public static List<CFaction> GetEmergencyFactions()
	{
		List<CFaction> returnValue = new List<CFaction>();
		foreach (CFaction faction in m_LookupTable.Values)
		{
			if (faction.Type == EFactionType.LawEnforcement || faction.Type == EFactionType.Medical)
			{
				returnValue.Add(faction);
			}
		}

		return returnValue;
	}

	private static Dictionary<EntityDatabaseID, CFaction> m_LookupTable = new Dictionary<EntityDatabaseID, CFaction>();
}