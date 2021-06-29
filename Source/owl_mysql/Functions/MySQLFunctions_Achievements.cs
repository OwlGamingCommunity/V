using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Achievements
		{
			public static void GivePlayerAchievement(int a_AccountID, EAchievementID a_AchievementID, Int64 a_UnlockTime, Action<EntityDatabaseID> CompletionCallback)
			{
				ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Achievements, new Dictionary<string, object>
				{
					{"account", a_AccountID },
					{"achievement_id", a_AchievementID },
					{"unlocked", a_UnlockTime }
				}, (CMySQLResult mysqlResult) =>
				{
					CompletionCallback((EntityDatabaseID)mysqlResult.GetInsertID());
				});
			}

			public static void GetAccountsWithAchievement(EAchievementID achievementID, Action<List<EntityDatabaseID>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Achievements, new List<string> { "account" }, WhereClause.Create("achievement_id={0}", (int)achievementID), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					List<EntityDatabaseID> lstAccountIDsWithAchievement = new List<EntityDatabaseID>();

					foreach (CMySQLRow row in mysqlResult.GetRows())
					{
						EntityDatabaseID account_id = (EntityDatabaseID)Convert.ChangeType(row["account"], typeof(EntityDatabaseID));
						lstAccountIDsWithAchievement.Add(account_id);
					}

					CompletionCallback?.Invoke(lstAccountIDsWithAchievement);
				});
			}

			public static void GetPercentOfActiveUsersWithAchievement(bool bAddOneUserForAchievementGrant, EntityDatabaseID accountBeingGranted, EAchievementID a_AchievementID, Action<int, EAchievementRarity> CompletionCallback)
			{
				// work out rarity
				Database.Functions.Util.GetActiveUsers((List<EntityDatabaseID> lstActiveUserAccountIDs) =>
				{
					Database.Functions.Achievements.GetAccountsWithAchievement(a_AchievementID, (List<EntityDatabaseID> lstAccountIDsWithAchievement) =>
					{
						// work out which accounts with the achievement, were active
						int numActiveUsersWithAchievement = 0;
						foreach (EntityDatabaseID accountWithAchievement in lstAccountIDsWithAchievement)
						{
							// is it active?
							if (lstActiveUserAccountIDs.Contains(accountWithAchievement))
							{
								++numActiveUsersWithAchievement;
							}
						}

						// if granting, and we don't have it, add 1 user
						if (bAddOneUserForAchievementGrant && !lstAccountIDsWithAchievement.Contains(accountBeingGranted))
						{
							++numActiveUsersWithAchievement;
						}

						int percentOfUsers = (int)(((float)(numActiveUsersWithAchievement) / (float)(lstActiveUserAccountIDs.Count)) * 100.0f);
						if (percentOfUsers > 100)
						{
							percentOfUsers = 100;
						}

						EAchievementRarity rarity = EAchievementRarity.Common;
						if (percentOfUsers <= AchievementConstants.RarityPercent_VeryRare)
						{
							rarity = EAchievementRarity.VeryRare;
						}
						else if (percentOfUsers <= AchievementConstants.RarityPercent_Rare)
						{
							rarity = EAchievementRarity.Rare;
						}

						CompletionCallback?.Invoke(percentOfUsers, rarity);
					});
				});
			}
		}
	}
}