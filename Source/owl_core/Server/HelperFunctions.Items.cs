using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelperFunctions
{
	public static class Items
	{
		public static async Task DeleteAllItems(CItemInstanceDef a_ItemInstance)
		{
			// Loop through player inventories IG and clean them of the item
			ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
			foreach (var player in players)
			{
				if (player.IsSpawned)
				{
					player.Inventory.RemoveItems(a_ItemInstance, true);
				}
			}

			// TODO: Optimize using Linq
			// Loop through world items and delete the items that match (we do this as two loops to avoid modifying during iteration)
			List<CWorldItem> lstWorldItemsToDestroy = new List<CWorldItem>();
			ICollection<CWorldItem> WorldItems = WorldItemPool.GetAllWorldItems();
			foreach (var WorldItem in WorldItems)
			{
				if (WorldItem.ItemInstance.ItemID == a_ItemInstance.ItemID && WorldItem.ItemInstance.Equals(a_ItemInstance))
				{
					lstWorldItemsToDestroy.Add(WorldItem);
				}
			}

			// Now destroy
			foreach (var WorldItemToDestroy in lstWorldItemsToDestroy)
			{
				Database.Functions.Items.DestroyWorldItem(WorldItemToDestroy.m_DatabaseID);
				WorldItemPool.DestroyWorldItem(WorldItemToDestroy);
			}

			// Clear all offline player items
			await Database.LegacyFunctions.DestroyItems(a_ItemInstance).ConfigureAwait(true);
		}

		public static EDataNames GetDataNameFromDrugEffect(EDrugEffect a_DrugEffect)
		{
			switch (a_DrugEffect)
			{
				case EDrugEffect.Weed:
					{
						return EDataNames.DRUG_FX_1;
					}

				case EDrugEffect.Meth:
					{
						return EDataNames.DRUG_FX_1;
					}

				case EDrugEffect.Cocaine:
					{
						return EDataNames.DRUG_FX_1;
					}

				case EDrugEffect.Heroin:
					{
						return EDataNames.DRUG_FX_1;
					}

				case EDrugEffect.Xanax:
					{
						return EDataNames.DRUG_FX_1;
					}
			}

			// TODO: default?
			return EDataNames.DRUG_FX_1;
		}
	}
}
