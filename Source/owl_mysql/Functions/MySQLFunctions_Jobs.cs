using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Jobs
		{
			public static void SaveTruckerJobProgress(EntityDatabaseID DatabaseID, int a_XP)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "trucker_job_xp", a_XP }

				}, WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}

			public static void SaveDeliveryDriverJobProgress(EntityDatabaseID DatabaseID, int a_XP)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "deliverydriver_job_xp", a_XP }

				}, WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}

			public static void SaveBusDriverJobProgress(EntityDatabaseID DatabaseID, int a_XP)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "busdriver_job_xp", a_XP }

				}, WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}

			public static void SaveMailmanJobProgress(EntityDatabaseID DatabaseID, int a_XP)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "mailman_job_xp", a_XP }

				}, WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}

			public static void SaveTrashmanJobProgress(EntityDatabaseID DatabaseID, int a_XP)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "trashman_job_xp", a_XP }
				}, WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}
		}
	}
}
