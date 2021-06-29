using System;
using System.Collections.Generic;
using Database.Models;
using EntityDatabaseID = System.Int64;

namespace Database.Functions
{
	public static class Properties
	{
		public static void Create(Property property, Action<Property> CompletionCallback)
		{
			ThreadedMySQL.Query_INSERT(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Properties,
				property.ToDictionary(),
				result =>
				{
					property.SetId((EntityDatabaseID) result.GetInsertID());
					CompletionCallback(property);
				});
		}

		public static void Update(Property property, Action CompletionCallback)
		{
			ThreadedMySQL.Query_UPDATE(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Properties,
				property.ToDictionary(),
				WhereClause.Create("id={0}", property.Id),
				result => CompletionCallback()
			);
		}

		public static void Delete(Property property, Action CompletionCallback)
		{
			ThreadedMySQL.Query_DELETE(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Properties,
				WhereClause.Create("id={0}", property.Id),
				result => CompletionCallback()
			);
		}

		public static void Find(EntityDatabaseID propertyId, Action<Property> CompletionCallback)
		{
			ThreadedMySQL.Query_SELECT(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Properties,
				new List<string> {"*"},
				WhereClause.Create("id={0}", propertyId),
				result => CompletionCallback(result.NumRows() == 0 ? null : Property.FromDB(result.GetRow(0)))
			);
		}

		public static void Get(Action<List<Property>> CompletionCallback)
		{
			ThreadedMySQL.Query_SELECT(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Properties,
				new List<string> {"*"},
				null,
				result =>
				{
					// TODO: Move these nested threaded queries into some sort of thing that waits for all to resolve.
					
					Items.GetAllPropertyFurnitureItemsAndRemovals((furnitureItems, furnitureRemovals) =>
					{
						Items.GetAllMailboxItems(mailboxes =>
						{
							List<Property> properties = new List<Property>();
							foreach (CMySQLRow row in result.GetRows())
							{
								Property property = Property.FromDB(row);

								if (furnitureItems.ContainsKey(property.Id))
								{
									property.SetFurnitureItems(furnitureItems[property.Id]);
									furnitureItems.Remove(property.Id);
								}

								if (furnitureRemovals.ContainsKey(property.Id))
								{
									property.SetFurnitureRemovals(furnitureRemovals[property.Id]);
									furnitureRemovals.Remove(property.Id);
								}

								// hook up inventory
								List<CItemInstanceDef> inventoryItems = new List<CItemInstanceDef>();
								if (mailboxes.ContainsKey(property.Id))
								{
									inventoryItems = mailboxes[property.Id];
								}

								// now hook up our container items
								List<CItemInstanceDef> lstContainerItems = new List<CItemInstanceDef>();
								foreach (CItemInstanceDef inventoryItem in inventoryItems)
								{
									lstContainerItems.AddRange(Items.ResolveContainerItemsFromItemListRecursively(inventoryItem.DatabaseID, mailboxes));
								}
								inventoryItems.AddRange(lstContainerItems);

								property.SetInventory(inventoryItems);

								properties.Add(property);
							}

							// Items.RemoveGhostFurnitureItems(furnitureItems);
							// Items.RemoveGhostFurnitureRemovals(furnitureRemovals);

							CompletionCallback(properties);
						});
					});
				});
		}
	}
}