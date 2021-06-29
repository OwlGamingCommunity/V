using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Notifications
		{
			public static void Create(EntityDatabaseID accountID, string title, string clickEvent, string body, Action<EntityDatabaseID> CompletionCallback = null)
			{
				ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Notifications, new Dictionary<string, object>
				{
					{ "account_id", accountID },
					{ "title", title },
					{ "click_event", clickEvent },
					{ "body", body },
				}, (CMySQLResult result) => { CompletionCallback?.Invoke((EntityDatabaseID)result.GetInsertID()); });
			}

			public static void Delete(EntityDatabaseID DatabaseID)
			{
				ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Notifications, 
					WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}

			public static void Get(EntityDatabaseID accountID, Action<List<CDatabaseStructurePersistentNotification>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Notifications, new List<string> { "id", "account_id", "title", "click_event", "body", "UNIX_TIMESTAMP(created_at) as created_at" }, WhereClause.Create("account_id={0}", accountID), (CMySQLResult mysqlResult) =>
				{
					List<CDatabaseStructurePersistentNotification> lstNotifications = mysqlResult.GetRows().Select(row => new CDatabaseStructurePersistentNotification(row)).ToList();

					CompletionCallback?.Invoke(lstNotifications);
				});
			}
		}
	}
}
