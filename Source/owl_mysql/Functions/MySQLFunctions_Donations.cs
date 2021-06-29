using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Donations
		{
			public static void RemoveExpiredDonationItems(Action CompletionCallback)
			{
				Int64 unixTimestamp = Helpers.GetUnixTimestamp();
				ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Donation_Inventory, WhereClause.Create("time_expire >= 0 && time_expire < {0}", unixTimestamp), (CMySQLResult mysqlResult) =>
				{
					CompletionCallback?.Invoke();
				});
			}

			public static void LoadDonationInventory(EntityDatabaseID a_AccountID, Action<List<DonationInventoryItem>> CompletionCallback)
			{
				RemoveExpiredDonationItems(() =>
				{
					ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Donation_Inventory, new List<string> { "*" }, WhereClause.Create("account={0}", a_AccountID), (CMySQLResult mysqlResult) =>
					{
						List<DonationInventoryItem> lstDonationInventory = new List<DonationInventoryItem>();

						foreach (CMySQLRow row in mysqlResult.GetRows())
						{
							UInt32 ID = row.GetValue<UInt32>("id");
							Int64 Character = row.GetValue<Int64>("character_id");
							Int64 TimeActivated = row.GetValue<Int64>("time_activated");
							Int64 TimeExpire = row.GetValue<Int64>("time_expire");
							UInt32 DonationID = row.GetValue<UInt32>("donation_id");
							Int64 VehicleID = row.GetValue<Int64>("vehicle_id");
							Int64 PropertyID = row.GetValue<Int64>("property_id");

							lstDonationInventory.Add(new DonationInventoryItem(ID, Character, TimeActivated, TimeExpire, DonationID, VehicleID, PropertyID));
						}

						CompletionCallback?.Invoke(lstDonationInventory);
					});
				});
			}

			public static void GetDonationInventoryOffline(long characterId, Action<List<DonationInventoryItem>> CompletionCallback)
			{
				Database.Functions.Characters.GetAccountIdFromCharacterId(characterId, accountId =>
				{
					LoadDonationInventory(accountId, donatorInventory =>
					{
						CompletionCallback?.Invoke(donatorInventory);
					});
				});
			}
		}
	}
}
