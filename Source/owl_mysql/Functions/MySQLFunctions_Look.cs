using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Look
		{
			public static void Create(EntityDatabaseID characterID, int height, int weight,
				string physicalAppearance, string scars, string tattoos, string makeup, Action CompletionCallback)
			{
				ThreadedMySQL.Query_INSERT(
					EDatabase.Game,
					EThreadContinuationFlag.ContinueOnMainThread,
					TableNames.Character_Looks,
					new Dictionary<string, object>
					{
						{"character_id", characterID},
						{"height", height},
						{"weight", weight},
						{"physical_appearance", physicalAppearance},
						{"scars", scars},
						{"tattoos", tattoos},
						{"makeup", makeup},
					}, result => { CompletionCallback(); }
				);
			}

			public static void Update(EntityDatabaseID characterID, int height, int weight,
				string physicalAppearance, string scars, string tattoos, string makeup, Action CompletionCallback)
			{
				ThreadedMySQL.Query_UPDATE(
					EDatabase.Game,
					EThreadContinuationFlag.ContinueOnMainThread,
					TableNames.Character_Looks,
					new Dictionary<string, object>
					{
						{"height", height},
						{"weight", weight},
						{"physical_appearance", physicalAppearance},
						{"scars", scars},
						{"tattoos", tattoos},
						{"makeup", makeup},
					},
					WhereClause.Create("character_id={0}", characterID),
					result =>
					{
						CompletionCallback();
					}
				);
			}

			public static void Find(EntityDatabaseID characterID, Action<CCharacterLook> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(
					EDatabase.Game,
					EThreadContinuationFlag.ContinueOnMainThread,
					TableNames.Character_Looks,
					new List<string> { "character_id", "height", "weight", "physical_appearance", "scars", "tattoos", "makeup", "UNIX_TIMESTAMP(created_at) AS created_at", "UNIX_TIMESTAMP(updated_at) AS updated_at" },
					WhereClause.Create("character_id={0}", characterID),
					lookResult =>
					{
						if (lookResult.NumRows() == 0)
						{
							CompletionCallback(null);
							return;
						}

						CMySQLRow row = lookResult.GetRow(0);
						CompletionCallback(new CCharacterLook(row));
					});
			}
		}
	}
}