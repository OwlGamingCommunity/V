using Database.Models;
using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

namespace Database
{
    namespace Functions
    {
        public static class Items
        {
            public static void GiveEntityItemBulk(List<CItemInstanceDef> lstItemInstances, Action<Dictionary<CItemInstanceDef, EntityDatabaseID>> CompletionCallback = null)
            {
                // rows
                List<List<object>> lstRows = new List<List<object>>();
                foreach (CItemInstanceDef itemInstanceDef in lstItemInstances)
                {
                    List<object> newRow = new List<object>();
                    newRow.Add((int)itemInstanceDef.ItemID);
                    newRow.Add(itemInstanceDef.GetValueDataSerialized());
                    newRow.Add((int)itemInstanceDef.ParentType);
                    newRow.Add(itemInstanceDef.ParentDatabaseID);
                    newRow.Add(itemInstanceDef.CurrentSocket);
                    newRow.Add(itemInstanceDef.StackSize);
                    lstRows.Add(newRow);
                }

                ThreadedMySQL.BulkQuery_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new List<string>
                {
                    "item_id",
                    "item_value",
                    "parent_type",
                    "parent",
                    "current_socket",
                    "stack_size"
                }, lstRows, (CMySQLResult result) =>
                {
                    // work out our insert ID's
                    Dictionary<CItemInstanceDef, EntityDatabaseID> dictInsertIDs = new Dictionary<CItemInstanceDef, EntityDatabaseID>();
                    int numRows = lstItemInstances.Count;
                    EntityDatabaseID lastInsertID = (EntityDatabaseID)result.GetInsertID();
                    for (int i = 0; i < numRows; ++i)
                    {
                        EntityDatabaseID thisInsertID = lastInsertID + i; // insert ID's give you back the first ID in bulk queries, so we can extrapolate to get the rest since its a synchronous insert
                        dictInsertIDs.Add(lstItemInstances[i], thisInsertID);
                    }

                    CompletionCallback(dictInsertIDs);
                });
            }

            public static void GiveEntityItem(CItemInstanceDef a_ItemInstance, Action<EntityDatabaseID> CompletionCallback = null)
            {
                ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
                {
                    {"item_id", (int)a_ItemInstance.ItemID },
                    {"item_value", a_ItemInstance.GetValueDataSerialized() },
                    {"parent_type", (int)a_ItemInstance.ParentType },
                    {"parent", a_ItemInstance.ParentDatabaseID },
                    {"current_socket", a_ItemInstance.CurrentSocket },
                    {"stack_size", a_ItemInstance.StackSize }
                }, (CMySQLResult result) => { CompletionCallback?.Invoke((EntityDatabaseID)result.GetInsertID()); });
            }

            public static void CreateWorldItem(CItemInstanceDef a_ItemInstance, Vector3 a_vecPos, float a_fRotZ, EntityDatabaseID a_CharacterID, Dimension a_Dimension, Action<EntityDatabaseID> CompletionCallback = null)
            {
                ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
                {
                    {"item_id", (int)a_ItemInstance.ItemID },
                    {"item_value", a_ItemInstance.GetValueDataSerialized() },
                    {"x", a_vecPos.X },
                    {"y", a_vecPos.Y },
                    {"z", a_vecPos.Z },
                    {"rz", a_fRotZ },
                    {"dropped_by", a_CharacterID },
                    {"dimension", a_Dimension },
                    {"parent_type", (int)EItemParentTypes.World },
                    {"parent", 0 },
                    {"stack_size", a_ItemInstance.StackSize },
                }, (CMySQLResult result) => { CompletionCallback?.Invoke((EntityDatabaseID)result.GetInsertID()); });
            }

            public static void DestroyWorldItem(EntityDatabaseID a_DatabaseID, Action CompletionCallback = null)
            {
                ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, WhereClause.Create("id={0} and parent_type={1} LIMIT 1;", a_DatabaseID, (int)EItemParentTypes.World), (CMySQLResult mysqlResult) =>
                {
                    CompletionCallback?.Invoke();
                });
            }

            public static void RemoveEntityItem(CItemInstanceDef a_ItemInstance, Action CompletionCallback)
            {
                ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, WhereClause.Create("id={0} AND parent={1} and parent_type={2} LIMIT 1;", a_ItemInstance.DatabaseID, a_ItemInstance.ParentDatabaseID, a_ItemInstance.ParentType), (CMySQLResult mysqlResult) =>
                {
                    CompletionCallback?.Invoke();
                });
            }

            public static void DeleteFurnitureItemFromProperty(EntityDatabaseID furnitureDBID, EntityDatabaseID a_PropertyID, Action CompletionCallback = null)
            {
                ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, WhereClause.Create("id={0} AND parent_type={1} AND parent={2} LIMIT 1;", furnitureDBID, (int)EItemParentTypes.FurnitureInsideProperty, a_PropertyID), (CMySQLResult mysqlResult) =>
                {
                    CompletionCallback?.Invoke();
                });
            }

            public static void DeleteDefaultFurnitureRemovalFromProperty(EntityDatabaseID removalDBID, EntityDatabaseID a_PropertyID, Action CompletionCallback = null)
            {
                ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, WhereClause.Create("id={0} AND parent_type={1} AND parent={2} LIMIT 1;", removalDBID, (int)EItemParentTypes.DefaultFurnitureRemoval, a_PropertyID), (CMySQLResult mysqlResult) =>
                {
                    CompletionCallback?.Invoke();
                });
            }

            public static void CommitFurnitureChange(EntityDatabaseID furnitureDBID, EntityDatabaseID a_PropertyID, Vector3 a_vecPos, Vector3 a_vecRot)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
            {
                { "x", a_vecPos.X },
                { "y", a_vecPos.Y },
                { "z", a_vecPos.Z },
                { "rx", a_vecRot.X },
                { "ry", a_vecRot.Y },
                { "rz", a_vecRot.Z },

            }, WhereClause.Create("id={0} AND parent_type={1} AND parent={2} LIMIT 1", furnitureDBID, (int)EItemParentTypes.FurnitureInsideProperty, a_PropertyID));
            }

            public static void SetGangTagProgress(EntityDatabaseID a_databaseID, float fProgress)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Gangtags, new Dictionary<string, object>
                {
                    { "progress", fProgress }

                }, WhereClause.Create("id={0} LIMIT 1", a_databaseID));
            }

            public static List<CItemInstanceDef> ResolveContainerItemsFromItemListRecursively(EntityDatabaseID parentDatabaseID, Dictionary<EntityDatabaseID, List<CItemInstanceDef>> dictAllContainerItems)
            {
                List<CItemInstanceDef> lstContainerItems = new List<CItemInstanceDef>();

                // Does this item have items store inside?
                if (dictAllContainerItems.ContainsKey(parentDatabaseID))
                {
                    // add the contents
                    lstContainerItems.AddRange(dictAllContainerItems[parentDatabaseID]);

                    // call recursive now so if our child items have container items inside, those are also loaded
                    foreach (CItemInstanceDef containerItem in dictAllContainerItems[parentDatabaseID])
                    {
                        // SECURITY FIX: we cannot be in ourselves, if we are, this is a stack overflow and would crash the server
                        if (containerItem.DatabaseID != parentDatabaseID)
                        {
                            if (containerItem.ParentType == EItemParentTypes.Container)
                            {
                                lstContainerItems.AddRange(ResolveContainerItemsFromItemListRecursively(containerItem.DatabaseID, dictAllContainerItems));
                            }
                        }
                    }
                }

                return lstContainerItems;
            }

            public static void SaveItemValueAndSocket(CItemInstanceDef a_ItemInstance)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
                {
                    { "item_value", a_ItemInstance.GetValueDataSerialized() },
                    { "current_socket", a_ItemInstance.CurrentSocket }

                }, WhereClause.Create("id={0} LIMIT 1", a_ItemInstance.DatabaseID));
            }

            public static void GetInventoryForFurnitureItemRecursive(EntityDatabaseID a_FurnitureID, Action<List<CItemInstanceDef>> CompletionCallback)
            {
                Dictionary<EntityDatabaseID, List<CItemInstanceDef>> dictAllContainerItems = new Dictionary<EntityDatabaseID, List<CItemInstanceDef>>();
                ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new List<string> { "id", "item_id", "item_value", "current_socket", "parent", "parent_type", "stack_size" },
                    WhereClause.Create("parent_type={0} OR (parent_type={1} AND parent={2})", (int)EItemParentTypes.Container, (int)EItemParentTypes.FurnitureContainer, a_FurnitureID), (CMySQLResult inventoryResult) => // NOTE: on main thread at this point
                    {
                        // store container items and root item
                        List<CItemInstanceDef> lstRet = new List<CItemInstanceDef>();
                        CItemInstanceDef furnitureContainerRootItem = null;

                        foreach (CMySQLRow itemRow in inventoryResult.GetRows())
                        {
                            EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
                            EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
                            string itemValue = itemRow["item_value"];
                            EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
                            EItemParentTypes parentType = (EItemParentTypes)Convert.ToInt32(itemRow["parent_type"]);
                            EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
                            UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

                            if (parentType == EItemParentTypes.Container)
                            {
                                if (!dictAllContainerItems.ContainsKey(parentDatabaseID))
                                {
                                    dictAllContainerItems[parentDatabaseID] = new List<CItemInstanceDef>();
                                }

                                dictAllContainerItems[parentDatabaseID].Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize));
                            }
                            else if (parentType == EItemParentTypes.FurnitureContainer)
                            {
                                furnitureContainerRootItem = CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize);

                                // hook up inventory
                                if (furnitureContainerRootItem != null)
                                {
                                    lstRet.Add(furnitureContainerRootItem);
                                    lstRet.AddRange(ResolveContainerItemsFromItemListRecursively(furnitureContainerRootItem.DatabaseID, dictAllContainerItems));
                                }
                            }
                        }

                        CompletionCallback(lstRet);
                    });
            }

            public static void UpdateChildItems(EntityDatabaseID a_NewParent, EntityDatabaseID a_CurrentParent, EItemParentTypes a_CurrentParentType, EItemSocket a_ItemSocket, EItemParentTypes a_NewParentType, Action CompletionCallback)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
            {
                { "current_socket", a_ItemSocket },
                { "parent_type", a_NewParentType },
                { "parent", a_NewParent },

            }, WhereClause.Create("parent={0} and parent_type={1}", a_CurrentParent, (int)a_CurrentParentType), (CMySQLResult result) =>
                {
                    CompletionCallback?.Invoke();
                });
            }

            public static void SetItemBinding(EntityDatabaseID a_NewParent, EntityDatabaseID a_CurrentParent, EItemParentTypes a_CurrentParentType, EntityDatabaseID a_DatabaseID, EItemSocket a_ItemSocket, EItemParentTypes a_NewParentType)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
            {
                { "current_socket", a_ItemSocket },
                { "parent_type", a_NewParentType },
                { "parent", a_NewParent }

            }, WhereClause.Create("id={0} AND parent={1} AND parent_type={2} LIMIT 1", a_DatabaseID, a_CurrentParent, (int)a_CurrentParentType));
            }

            public static void SaveItemValue(CItemInstanceDef a_ItemInstance)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
            {
                { "item_value", a_ItemInstance.GetValueDataSerialized() }

            }, WhereClause.Create("id={0} LIMIT 1;", a_ItemInstance.DatabaseID));
            }

            public static void SavePropertyFurnitureValue(CPropertyFurnitureInstance propFurnitureInstance)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
                {
                    { "item_value", JsonConvert.SerializeObject(propFurnitureInstance.Value) }

                }, WhereClause.Create("id={0} LIMIT 1;", propFurnitureInstance.DBID));
            }

            public static void SaveItemValueAndStackSize(CItemInstanceDef a_ItemInstance)
            {
                ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new Dictionary<string, object>
                {
                    { "item_value", a_ItemInstance.GetValueDataSerialized() },
                    { "stack_size", a_ItemInstance.StackSize }

                }, WhereClause.Create("id={0} LIMIT 1;", a_ItemInstance.DatabaseID));
            }

            public static void LoadAllInfoMarkers(Action<List<CDatabaseStructureInformationMarker>> CompletionCallback)
            {
                ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.InfoMarkers, new List<string> { "*" }, null, (CMySQLResult result) => // NOTE: on main thread at this point
                {
                    List<CDatabaseStructureInformationMarker> lstInfoMarkers = new List<CDatabaseStructureInformationMarker>();

                    // now load all vehicles
                    foreach (CMySQLRow row in result.GetRows())
                    {
                        CDatabaseStructureInformationMarker infoMarker = new CDatabaseStructureInformationMarker(row);
                        lstInfoMarkers.Add(infoMarker);
                    }

                    CompletionCallback?.Invoke(lstInfoMarkers);
                });
            }

            public static void CreateInfoMarker(EntityDatabaseID a_OwnerCharacterID, string a_strText, Vector3 a_vecPos, Dimension a_Dimension, Action<EntityDatabaseID> CompletionCallback = null)
            {
                ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.InfoMarkers, new Dictionary<string, object>
                {
                    {"owner_char", a_OwnerCharacterID },
                    {"x", a_vecPos.X },
                    {"y", a_vecPos.Y },
                    {"z", a_vecPos.Z },
                    {"dimension", a_Dimension },
                    {"text", a_strText }
                }, (CMySQLResult result) => { CompletionCallback?.Invoke((EntityDatabaseID)result.GetInsertID()); });
            }

            public static void DestroyInfoMarker(EntityDatabaseID a_DatabaseID, Action CompletionCallback = null)
            {
                ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.InfoMarkers, WhereClause.Create("id={0} LIMIT 1;", a_DatabaseID), (CMySQLResult mysqlResult) =>
                {
                    CompletionCallback?.Invoke();
                });
            }

            public static void GetAllPropertyFurnitureItemsAndRemovals(Action<Dictionary<EntityDatabaseID, List<CDatabaseStructureFurnitureItem>>, Dictionary<EntityDatabaseID, List<FurnitureRemoval>>> CompletionCallback)
            {
                ThreadedMySQL.Query_SELECT(
                    EDatabase.Game,
                    EThreadContinuationFlag.ContinueOnQueryThread,
                    TableNames.Inventories,
                    new List<string> { "*" },
                    WhereClause.Create("parent_type={0} OR parent_type={1}", EItemParentTypes.FurnitureInsideProperty, EItemParentTypes.DefaultFurnitureRemoval),
                    result =>
                    {
                        Dictionary<EntityDatabaseID, List<CDatabaseStructureFurnitureItem>> lstAllFurniture = new Dictionary<EntityDatabaseID, List<CDatabaseStructureFurnitureItem>>();
                        Dictionary<EntityDatabaseID, List<FurnitureRemoval>> lstAllRemovals = new Dictionary<EntityDatabaseID, List<FurnitureRemoval>>();

                        foreach (CMySQLRow row in result.GetRows())
                        {
                            EItemParentTypes parentType = (EItemParentTypes)Convert.ToInt32(row["parent_type"]);
                            EntityDatabaseID parentPropertyID = row.GetValue<EntityDatabaseID>("parent");

                            if (parentType == EItemParentTypes.FurnitureInsideProperty)
                            {
                                CDatabaseStructureFurnitureItem entry = new CDatabaseStructureFurnitureItem(row);
                                if (!lstAllFurniture.ContainsKey(parentPropertyID))
                                {
                                    lstAllFurniture.Add(parentPropertyID, new List<CDatabaseStructureFurnitureItem>());
                                }

                                lstAllFurniture[parentPropertyID].Add(entry);
                            }

                            if (parentType == EItemParentTypes.DefaultFurnitureRemoval)
                            {
                                FurnitureRemoval entry = FurnitureRemoval.FromDB(row);
                                if (!lstAllRemovals.ContainsKey(parentPropertyID))
                                {
                                    lstAllRemovals.Add(parentPropertyID, new List<FurnitureRemoval>());
                                }

                                lstAllRemovals[parentPropertyID].Add(entry);
                            }
                        }

                        CompletionCallback(lstAllFurniture, lstAllRemovals);
                    }
                );
            }

            public static void GetAllMailboxItems(Action<Dictionary<EntityDatabaseID, List<CItemInstanceDef>>> CompletionCallback)
            {
                ThreadedMySQL.Query_SELECT(
                    EDatabase.Game,
                    EThreadContinuationFlag.ContinueOnQueryThread,
                    TableNames.Inventories,
                    new List<string> { "*" },
                    WhereClause.Create("parent_type={0} OR parent_type={1}", EItemParentTypes.Container, EItemParentTypes.PropertyMailbox),
                    result =>
                    {
                        Dictionary<EntityDatabaseID, List<CItemInstanceDef>> mailboxes = new Dictionary<EntityDatabaseID, List<CItemInstanceDef>>();

                        foreach (CMySQLRow itemRow in result.GetRows())
                        {
                            EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
                            EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
                            string itemValue = itemRow["item_value"];
                            EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
                            EItemParentTypes parentType = (EItemParentTypes)Convert.ToInt32(itemRow["parent_type"]);
                            EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
                            UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

                            if (!mailboxes.ContainsKey(parentDatabaseID))
                            {
                                mailboxes[parentDatabaseID] = new List<CItemInstanceDef>();
                            }

                            mailboxes[parentDatabaseID].Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize));
                        }

                        CompletionCallback(mailboxes);
                    }
                );
            }

            public static void RemoveGhostFurnitureItems(Dictionary<EntityDatabaseID, List<CDatabaseStructureFurnitureItem>> furnitureItems)
            {
                if (furnitureItems.Count == 0)
                {
                    return;
                }

                NAPI.Util.ConsoleOutput("Removing furniture linked to {0} properties that no longer exist.", furnitureItems.Count);

                ThreadedMySQL.Query_DELETE(
                    EDatabase.Game,
                    EThreadContinuationFlag.ContinueOnQueryThread,
                    TableNames.Inventories,
                    WhereClause.Create("parent_type={0} AND parent IN ({1})", EItemParentTypes.FurnitureInsideProperty, furnitureItems.Keys.ToArray())
                );
            }

            public static void RemoveGhostFurnitureRemovals(Dictionary<EntityDatabaseID, List<FurnitureRemoval>> furnitureRemovals)
            {
                if (furnitureRemovals.Count == 0)
                {
                    return;
                }

                NAPI.Util.ConsoleOutput("Removing removals linked to {0} properties that no longer exist.", furnitureRemovals.Count);

                ThreadedMySQL.Query_DELETE(
                    EDatabase.Game,
                    EThreadContinuationFlag.ContinueOnQueryThread,
                    TableNames.Inventories,
                    WhereClause.Create("parent_type={0} AND parent IN ({1})", EItemParentTypes.DefaultFurnitureRemoval, furnitureRemovals.Keys.ToArray())
                );
            }
        }
    }
}
