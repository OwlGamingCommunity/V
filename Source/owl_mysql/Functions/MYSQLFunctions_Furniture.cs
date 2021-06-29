using System;
using System.Collections.Generic;
using Database.Models;
using EntityDatabaseID = System.Int64;

namespace Database.Functions
{
    public static class Furniture
    {
        public static void CreateRemoval(FurnitureRemoval furnitureRemoval, Action<FurnitureRemoval> callback)
        {
            ThreadedMySQL.Query_INSERT(
                EDatabase.Game, 
                EThreadContinuationFlag.ContinueOnMainThread, 
                TableNames.Inventories,
                furnitureRemoval.ToDictionary(),
                result =>
                {
                    furnitureRemoval.SetId((EntityDatabaseID) result.GetInsertID());
                    callback(furnitureRemoval);
                });
        }
        
        public static void UpdateRemoval(FurnitureRemoval furnitureRemoval, Action<FurnitureRemoval> callback)
        {
            ThreadedMySQL.Query_UPDATE(
                EDatabase.Game,
                EThreadContinuationFlag.ContinueOnMainThread,
                TableNames.Inventories,
                furnitureRemoval.ToDictionary(),
                WhereClause.Create("id={0} LIMIT 1", furnitureRemoval.Id),
                result => callback(furnitureRemoval)
            );
        }
    }
}