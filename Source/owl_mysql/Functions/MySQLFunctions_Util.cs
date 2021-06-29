using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Util
		{
			public static void GetAccountAndCharacterKeybinds(EntityDatabaseID AccountID, EntityDatabaseID CharacterID, Action<List<PlayerKeybindObject>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Keybinds, new List<string> { "*" },
				WhereClause.Create("account={0} AND ((character_id = -1 AND bind_type={3}) OR (character_id = {1} AND bind_type={2}))", AccountID, CharacterID, EPlayerKeyBindType.Character, EPlayerKeyBindType.Account), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					List<PlayerKeybindObject> lstBinds = new List<PlayerKeybindObject>();

					foreach (CMySQLRow row in mysqlResult.GetRows())
					{
						EntityDatabaseID id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID));
						EPlayerKeyBindType bindType = (EPlayerKeyBindType)Convert.ToInt32(row["bind_type"]);
						string strAction = row["bind_action"];
						ConsoleKey key = (ConsoleKey)Convert.ToInt32(row["bind_key"]);

						PlayerKeybindObject newKeybind = new PlayerKeybindObject(id, key, bindType, strAction);
						lstBinds.Add(newKeybind);
					}

					CompletionCallback?.Invoke(lstBinds);
				});
			}

			public static void GetActiveUsers(Action<List<EntityDatabaseID>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Auth, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Accounts, new List<string> { "id" }, WhereClause.Create("DATEDIFF(NOW(), ucp_lastlogin) <= 30"), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					List<EntityDatabaseID> lstActiveUserAccountIDs = new List<EntityDatabaseID>();

					foreach (CMySQLRow row in mysqlResult.GetRows())
					{
						EntityDatabaseID id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID));
						lstActiveUserAccountIDs.Add(id);
					}

					CompletionCallback?.Invoke(lstActiveUserAccountIDs);
				});
			}

			public static void SavePerformanceCapture(int a_AccountID, string strData, Action<EntityDatabaseID> CompletionCallback)
			{
				ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.PerfCaptures, new Dictionary<string, object>
				{
					{"account_id", a_AccountID },
					{"data", strData }
				}, (CMySQLResult result) =>
				{
					CompletionCallback((EntityDatabaseID)result.GetInsertID());
				});
			}

			public static void GetPerformanceCaptures(Action<List<CDatabaseStructurePerformanceCapture>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.PerfCaptures, new List<string> { "id, account_id, data" }, null, (CMySQLResult mysqlResult) =>
				{
					List<CDatabaseStructurePerformanceCapture> lstCaptures = new List<CDatabaseStructurePerformanceCapture>();

					foreach (CMySQLRow row in mysqlResult.GetRows())
					{
						CDatabaseStructurePerformanceCapture capture = new CDatabaseStructurePerformanceCapture(row);
						lstCaptures.Add(capture);
					}

					CompletionCallback?.Invoke(lstCaptures);
				});
			}

			public static void DeletePerformanceCapture(EntityDatabaseID dbid, Action CompletionCallback = null)
			{
				ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.PerfCaptures, WhereClause.Create("id={0} LIMIT 1", dbid), (CMySQLResult mysqlResult) =>
				{
					CompletionCallback?.Invoke();
				});
			}

			public static void GetPerformanceCapture(EntityDatabaseID dbid, Action<CDatabaseStructurePerformanceCapture> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.PerfCaptures, new List<string> { "id, account_id, data" }, WhereClause.Create("id={0} LIMIT 1", dbid), (CMySQLResult mysqlResult) =>
				{
					CDatabaseStructurePerformanceCapture capture = null;

					if (mysqlResult.NumRows() > 0)
					{
						capture = new CDatabaseStructurePerformanceCapture(mysqlResult.GetRow(0));
					}

					CompletionCallback?.Invoke(capture);
				});
			}
		}
	}
}
