using GTANetworkAPI;
using System;
using System.Collections.Generic;

namespace Database
{
	namespace Functions
	{
		public static class Phones
		{
			public static void SavePhoneContact(string CPNumber, string entryNumber, string entryName, Action<ulong> CompletionCallback = null)
			{
				ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Phone_Contacts, new Dictionary<string, object>
				{
					{"phone", CPNumber },
					{"entryName", entryName },
					{"entryNumber", entryNumber }
				}, (CMySQLResult result) => { CompletionCallback?.Invoke(result.GetInsertID()); });
			}

			public static void RemovePhoneContact(string CPNumber, string entryNumber, string entryName)
			{
				ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Phone_Contacts, WhereClause.Create("phone = '{0}' AND entryNumber = '{1}' AND entryName = '{2}'", CPNumber, entryNumber, entryName), (CMySQLResult mysqlResult) =>
				{

				});
			}

			public static void GetPhoneContacts(string CPNumber, Action<List<KeyValuePair<string, string>>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Phone_Contacts, new List<string> { "*" },
					WhereClause.Create("phone = '{0}'", CPNumber), (CMySQLResult result) => // NOTE: on main thread at this point
					{
					List<KeyValuePair<string, string>> contactsList = new List<KeyValuePair<string, string>>();

					foreach (var contact in result.GetRows())
					{
						contactsList.Add(new KeyValuePair<string, string>(contact["entryName"], contact["entryNumber"]));
					}

					CompletionCallback?.Invoke(contactsList);
				});
			}

			public static void CreatePhoneMessage(string from, string to, string content, Action<ulong> CompletionCallback = null)
			{
				ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Phone_SMS, new Dictionary<string, object>
				{
					{"from", from },
					{"to", to },
					{"content", content }
				}, (CMySQLResult result) => { CompletionCallback?.Invoke(result.GetInsertID()); });
			}

			public static void UpdateMessageViewed(string fromNumber, string toNumber)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Phone_SMS, new Dictionary<string, object>
				{
					{ "viewed", true },

				}, WhereClause.Create("`from`='{0}' AND `to`='{1}'", toNumber, fromNumber));
			}

			public static void GetTotalUnviewedMessages(string CPNumber, Action<int> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Phone_SMS, new List<string> { "viewed" },
				WhereClause.Create("`viewed` = false AND `to` = '{0}'", CPNumber), (CMySQLResult result) => // NOTE: on main thread at this point
				{
					CompletionCallback?.Invoke(result.NumRows());
				});
			}

			public static void GetPhoneContactByNumber(string CPNumber, string callingNumber, Action<string> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Phone_Contacts, new List<string> { "entryName" }, WhereClause.Create("`phone` = '{0}' AND `entryNumber` = '{1}' LIMIT 1", CPNumber, callingNumber), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					string contactName = string.Empty;
					if (mysqlResult.NumRows() >= 1)
					{
						contactName = mysqlResult.GetRow(0)["entryName"];
					}

					CompletionCallback(contactName);
				});
			}

			private static bool GetKeyValuePairFromKey(List<KeyValuePair<string, string>> lstKvPairs, string strKey, out KeyValuePair<string, string> outKvPair)
			{
				foreach (var kvPair in lstKvPairs)
				{
					if (kvPair.Key == strKey)
					{
						outKvPair = kvPair;
						return true;

					}
				}

				outKvPair = new KeyValuePair<string, string>();
				return false;
			}

			public static void GetPhoneMessagesContacts(string CPNumber, Action<List<CPhoneMessageContact>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Phone_SMS, new List<string> { "DISTINCT `from`", "`to`" }, WhereClause.Create("`from` = '{0}' OR `to` = '{0}' ORDER BY `date` DESC", CPNumber), (CMySQLResult messagesContactsResult) =>
				{
					ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Phone_Contacts, new List<string> { "*" }, WhereClause.Create("phone = '{0}'", CPNumber), (CMySQLResult contactsResult) =>
					{
						List<KeyValuePair<string, string>> messagesContactsList = new List<KeyValuePair<string, string>>();

						foreach (var messageRow in messagesContactsResult.GetRows())
						{
							KeyValuePair<string, string> outKvPair;

							foreach (var contactRow in contactsResult.GetRows())
							{
								if (contactRow["entryNumber"] == messageRow["from"] && !GetKeyValuePairFromKey(messagesContactsList, messageRow["from"], out outKvPair))
								{
									messagesContactsList.Add(new KeyValuePair<string, string>(messageRow["from"], contactRow["entryName"]));
								}

								if (contactRow["entryNumber"] == messageRow["to"] && !GetKeyValuePairFromKey(messagesContactsList, messageRow["to"], out outKvPair))
								{
									messagesContactsList.Add(new KeyValuePair<string, string>(messageRow["to"], contactRow["entryName"]));
								}
							}

							if (!GetKeyValuePairFromKey(messagesContactsList, messageRow["to"], out outKvPair))
							{
								messagesContactsList.Add(new KeyValuePair<string, string>(messageRow["to"], messageRow["to"]));
							}

							if (!GetKeyValuePairFromKey(messagesContactsList, messageRow["from"], out outKvPair))
							{
								messagesContactsList.Add(new KeyValuePair<string, string>(messageRow["from"], messageRow["from"]));
							}

							if (GetKeyValuePairFromKey(messagesContactsList, CPNumber, out outKvPair))
							{
								messagesContactsList.Remove(outKvPair);
							}
						}

						// now do the next query
						List<CPhoneMessageContact> returningList = new List<CPhoneMessageContact>();

						int numExpected = 0;
						int numCompleted = 0;
						foreach (var entry in messagesContactsList)
						{
							if (entry.Key != CPNumber)
							{
								++numExpected;

								ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Phone_SMS, new List<string> { "viewed" }, WhereClause.Create("`viewed` = false AND (`from`='{1}' AND `to`='{0}')", CPNumber, entry.Key), (CMySQLResult unreadMessagesResult) =>
								{
									returningList.Add(new CPhoneMessageContact(entry.Key, entry.Value, unreadMessagesResult.GetRows().Count));
									++numCompleted;

									if (numCompleted == numExpected)
									{
										NAPI.Task.Run(() => // switch back to main thread, we are on query thread for speed
										{
											CompletionCallback?.Invoke(returningList);
										});
									}
								});
							}
						}
					});
				});
			}

			public static void GetPhoneMessagesFromNumber(string CPNumber, string toNumber, Action<List<CPhoneMessage>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Phone_SMS, new List<string> { "*" },
					WhereClause.Create("(`from`='{0}' AND `to`='{1}') OR (`from`='{1}' AND `to`='{0}') ORDER BY `date`", CPNumber, toNumber), (CMySQLResult result) => // NOTE: on main thread at this point
					{
						List<CPhoneMessage> messagesList = new List<CPhoneMessage>();

						foreach (var row in result.GetRows())
						{
							messagesList.Add(new CPhoneMessage(row["from"], row["to"], row["content"], row["date"], Convert.ToBoolean(row["viewed"])));
						}

						CompletionCallback?.Invoke(messagesList);
					});
			}
		}
	}
}
