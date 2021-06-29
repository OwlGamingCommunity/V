using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Factions
		{
			public static void SetFactionType(EntityDatabaseID factionID, EFactionType newType)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread,
					TableNames.Factions, new Dictionary<string, object>
					{
						{"type", newType},
					}, WhereClause.Create("id={0} LIMIT 1", factionID));
			}

			private static void LoadSingleFaction(EntityDatabaseID a_FactionID, Action<CDatabaseStructureFaction> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Factions, new List<string> { "*" },
				WhereClause.Create("id={0}", a_FactionID), (CMySQLResult result) => // NOTE: on main thread at this point
				{
					if (result.NumRows() > 0)
					{
						LoadFactionFromDBRow(result.GetRow(0), (CDatabaseStructureFaction faction) =>
						 {
							 CompletionCallback(faction);
						 });

						// TODO_STABILITY: Error if faction already loaded, or remove old?
					}
					else
					{
						CompletionCallback(null);
					}
				});
			}

			private static void LoadFactionFromDBRow(CMySQLRow row, Action<CDatabaseStructureFaction> CompletionCallback)
			{
				CDatabaseStructureFaction faction = new CDatabaseStructureFaction(row);

				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Faction_Ranks, new List<string> { "*" },
				WhereClause.Create("faction_id={0}", faction.factionID), (CMySQLResult result) => // NOTE: on main thread at this point
				{
					foreach (CMySQLRow rankRow in result.GetRows())
					{
						EntityDatabaseID databaseID = rankRow.GetValue<EntityDatabaseID>("id");
						string strRankName = rankRow["name"];
						float fSalary = rankRow.GetValue<float>("salary");

						faction.lstFactionRanks.Add(new CFactionRank(databaseID, strRankName, fSalary));
					}

					CompletionCallback(faction);
				});
			}

			public static void LoadAllFactions(Action<List<CDatabaseStructureFaction>> CompletionCallback)
			{
				List<CDatabaseStructureFaction> lstFactions = new List<CDatabaseStructureFaction>();
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Factions, new List<string> { "*" }, null, (CMySQLResult result) => // NOTE: on main thread at this point
				{
					foreach (CMySQLRow row in result.GetRows())
					{
						LoadFactionFromDBRow(row, (CDatabaseStructureFaction faction) =>
						{
							lstFactions.Add(faction);
							// are we done? call callback
							if (lstFactions.Count == result.NumRows())
							{
								CompletionCallback(lstFactions);
							}
						});
					}
				});
			}
		}
	}
}