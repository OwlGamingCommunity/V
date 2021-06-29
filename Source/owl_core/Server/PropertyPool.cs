using Database.Models;
using GTANetworkAPI;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

public static class PropertyPool
{
	public static CPropertyInstance Add(Property property)
	{
		CPropertyInstance cProperty = new CPropertyInstance(property);
		m_dictPropertyInstances.Add(property.Id, cProperty);

		return cProperty;
	}

	/// <summary>
	/// Removes the property from the database, destroys it and removes it from the pool
	/// </summary>
	/// <param name="a_PropInst">The instance to remove</param>
	public static void DestroyProperty(CPropertyInstance a_PropInst)
	{
		try
		{
			NAPI.Task.Run(() =>
			{
				a_PropInst.Destroy(true);
			});

			// cleanup custom map if present
			MapLoader.FullyRemoveCustomMap((int)a_PropInst.Model.Id);
			a_PropInst.Cleanup();
			NetworkEventSender.SendNetworkEvent_Property_DestroyPlayerBlip_ForAll_IncludeEveryone((int)a_PropInst.Model.Id);
			m_dictPropertyInstances.Remove(a_PropInst.Model.Id);
		}
		catch
		{

		}
	}

	/// <summary>
	/// Deletes the property entities, removes it from the pool and queries the database to remake it
	/// </summary>
	/// <param name="a_PropInst">The instance to reload</param>
	public static void ReloadProperty(CPropertyInstance a_PropInst)
	{
		EntityDatabaseID propID = a_PropInst.Model.Id;

		NAPI.Task.Run(() =>
		{
			a_PropInst.Destroy(false);
		});
		m_dictPropertyInstances.Remove(propID);
		Database.Functions.Properties.Find(propID, property =>
		{
			Add(property);
		});
	}

	public static void UpdatePlayerPropertyBlips(CPlayer player)
	{
		if (player != null)
		{
			NetworkEventSender.SendNetworkEvent_Property_DestroyAllPlayerBlips(player);

			List<CPropertyInstance> lstProperties = GetPropertyInstancesFromOwner(EPropertyOwnerType.Player, player.ActiveCharacterDatabaseID);
			foreach (CPropertyInstance propInst in lstProperties)
			{
				propInst.UpdateOwnerBlipForPlayer(player);
			}
		}
	}

	public static CPropertyInstance GetPropertyInstanceFromID(EntityDatabaseID a_PropertyID)
	{
		// TODO_POST_LAUNCH: Don't allow interiors at zero because dimension zero is world.
		if (a_PropertyID == 0)
		{
			return null;
		}

		CPropertyInstance propertyInst = null;
		if (m_dictPropertyInstances.Keys.Contains(a_PropertyID))
		{
			propertyInst = m_dictPropertyInstances[a_PropertyID];
		}

		return propertyInst;
	}

	/// <summary>
	/// Returns a list of all properties owned or rented by the character ID
	/// </summary>
	/// <param name="a_OwnerID"></param>
	/// <returns></returns>
	public static List<CPropertyInstance> GetPropertyInstancesFromOwner(EPropertyOwnerType a_OwnerType, EntityDatabaseID a_OwnerID)
	{
		List<CPropertyInstance> lstPropertyInstances = new List<CPropertyInstance>();

		foreach (CPropertyInstance propertyInst in m_dictPropertyInstances.Values)
		{
			if ((propertyInst.Model.State == EPropertyState.Owned || propertyInst.Model.State == EPropertyState.Owned_AlwaysEnterable) && propertyInst.Model.OwnerType == a_OwnerType && a_OwnerID == propertyInst.Model.OwnerId)
			{
				lstPropertyInstances.Add(propertyInst);
			}
		}

		return lstPropertyInstances;
	}

	public static List<CPropertyInstance> GetPropertyInstancesFromRenter(EPropertyOwnerType a_RenterType, EntityDatabaseID a_OwnerID)
	{
		List<CPropertyInstance> lstPropertyInstances = new List<CPropertyInstance>();

		foreach (CPropertyInstance propertyInst in m_dictPropertyInstances.Values)
		{
			if (propertyInst.Model.State == EPropertyState.Rented && propertyInst.Model.RenterType == a_RenterType && a_OwnerID == propertyInst.Model.RenterId)
			{
				lstPropertyInstances.Add(propertyInst);
			}
		}

		return lstPropertyInstances;
	}

	public static List<CPropertyInstance> GetPropertyInstancesOwnedByPlayer(CPlayer a_Player)
	{
		return GetPropertyInstancesFromOwner(EPropertyOwnerType.Player, a_Player.ActiveCharacterDatabaseID);
	}

	public static List<CPropertyInstance> GetPropertyInstancesRentedByPlayer(CPlayer a_Player)
	{
		return GetPropertyInstancesFromOwner(EPropertyOwnerType.Player, a_Player.ActiveCharacterDatabaseID);
	}

	public static List<CPropertyInstance> GetPropertyInstancesOwnedByFaction(CFaction a_Faction)
	{
		return GetPropertyInstancesFromOwner(EPropertyOwnerType.Faction, a_Faction.FactionID);
	}

	public static CPropertyInstance GetPlayerOwnedPropertyByID(CPlayer a_Player, EntityDatabaseID a_ID)
	{
		return GetPropertyInstancesOwnedByPlayer(a_Player).FirstOrDefault(a_Property => a_Property.Model.Id == a_ID);
	}

	public static CPropertyInstance GetFactionOwnedPropertyByID(CFaction a_Faction, EntityDatabaseID a_ID)
	{
		return GetPropertyInstancesOwnedByFaction(a_Faction).FirstOrDefault(a_Property => a_Property.Model.Id == a_ID);
	}

	public static List<CPropertyInstance> GetPropertyInstancesRentedByFaction(CFaction a_Faction)
	{
		return GetPropertyInstancesFromOwner(EPropertyOwnerType.Faction, a_Faction.FactionID);
	}

	public static List<CPropertyInstance> GetAllPropertyInstances()
	{
		return m_dictPropertyInstances.Values.ToList();
	}

	public static List<CPropertyInstance> GetCharacterProperties(EntityDatabaseID characterId)
	{
		return (from property in m_dictPropertyInstances where property.Value.OwnedBy(EPropertyOwnerType.Player, characterId) select property.Value).ToList();
	}

	private static Dictionary<EntityDatabaseID, CPropertyInstance> m_dictPropertyInstances = new Dictionary<EntityDatabaseID, CPropertyInstance>();
}