using Database.Models;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Accounts
		{
			public static void CreateCustomAnim(CustomAnim customAnimModel, Action<CustomAnim> callback)
			{
				ThreadedMySQL.Query_INSERT(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Custom_Anims,
				customAnimModel.ToDictionary(),
				result =>
				{
					customAnimModel.SetId((EntityDatabaseID)result.GetInsertID());
					callback(customAnimModel);
				});
			}

			public static void UpdateCustomAnim(CustomAnim customAnimModel, Action<CustomAnim> callback)
			{
				ThreadedMySQL.Query_UPDATE(
					EDatabase.Game,
					EThreadContinuationFlag.ContinueOnMainThread,
					TableNames.Custom_Anims,
					customAnimModel.ToDictionary(),
					WhereClause.Create("id={0} LIMIT 1", customAnimModel.Id),
					result => callback(customAnimModel)
				);
			}

			public static void DeleteCustomAnim(CustomAnim customAnimModel, Action callback)
			{
				ThreadedMySQL.Query_DELETE(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Custom_Anims,
				WhereClause.Create("id={0}", customAnimModel.Id),
				result =>
				{
					callback();
				});
			}

			public static void LoadCustomAnims(EntityDatabaseID AccountID, Action<List<CustomAnim>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(
				EDatabase.Game,
				EThreadContinuationFlag.ContinueOnMainThread,
				TableNames.Custom_Anims,
				new List<string> { "*" },
				WhereClause.Create("account_id={0}", AccountID),
				result =>
				{
					List<CustomAnim> lstAnims = new List<CustomAnim>();
					foreach (CMySQLRow row in result.GetRows())
					{
						CustomAnim anim = CustomAnim.FromDB(row);
						lstAnims.Add(anim);
					}

					CompletionCallback(lstAnims);
				});
			}

			public static void SetApplicationState(EntityDatabaseID AccountID, EApplicationState a_AppState)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts, new Dictionary<string, object>
				{
					{ "app_state", (uint)a_AppState }

				}, WhereClause.Create("account_id={0} LIMIT 1", AccountID));
			}

			public static void SetApplicationQuestionsAndAnswers(EntityDatabaseID AccountID, UInt32 question1, UInt32 question2, UInt32 question3, UInt32 question4, string answer1, string answer2, string answer3, string answer4)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts, new Dictionary<string, object>
				{
					{ "app_question1", question1 },
					{ "app_question2", question2 },
					{ "app_question3", question3 },
					{ "app_question4", question4 },
					{ "app_answer1", answer1 },
					{ "app_answer2", answer2 },
					{ "app_answer3", answer3 },
					{ "app_answer4", answer4 },

				}, WhereClause.Create("account_id={0} LIMIT 1", AccountID));
			}

			public static void SetNumberOfApplications(EntityDatabaseID AccountID, UInt32 a_NumApps)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts, new Dictionary<string, object>
				{
					{ "num_apps", (uint)a_NumApps }

				}, WhereClause.Create("account_id={0} LIMIT 1", AccountID));
			}

			public static void GetPendingApplicationDetails(int AccountID, Action<PendingApplicationDetails> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts,
					new List<string>
					{
					"num_apps",
					"IFNULL(app_question1, '0') AS app_question1",
					"IFNULL(app_question2, '0') AS app_question2",
					"IFNULL(app_question3, '0') AS app_question3",
					"IFNULL(app_question4, '0') AS app_question4",
					"IFNULL(app_answer1, '') AS app_answer1",
					"IFNULL(app_answer2, '') AS app_answer2",
					"IFNULL(app_answer3, '') AS app_answer3",
					"IFNULL(app_answer4, '') AS app_answer4"
					},
				WhereClause.Create("app_state=2 AND account_id={0};", AccountID), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					// TODO: This is pretty heavy?
					PendingApplicationDetails result = null;

					if (mysqlResult.NumRows() > 0)
					{
						CMySQLRow row = mysqlResult.GetRow(0);
						UInt32 numapps = row.GetValue<UInt32>("num_apps");
						UInt32[] questionIndicies = new UInt32[] { row.GetValue<UInt32>("app_question1"), row.GetValue<UInt32>("app_question2"), row.GetValue<UInt32>("app_question3"), row.GetValue<UInt32>("app_question4") };
						string[] answers = new string[] { row["app_answer1"], row["app_answer2"], row["app_answer3"], row["app_answer4"] };

						result = new PendingApplicationDetails
						{
							NumApps = numapps,
							QuestionIndices = questionIndicies,
							Answers = answers
						};
					}

					CompletionCallback(result);
				});
			}

			public static void GetPendingApplications(Action<List<PendingApplication>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts, new List<string> { "account_id" }, WhereClause.Create("app_state=2"), (CMySQLResult mysqlResult) =>
				{
					int numExpected = mysqlResult.NumRows();
					int numProcessed = 0;
					List<PendingApplication> result = new List<PendingApplication>();

					foreach (CMySQLRow row in mysqlResult.GetRows())
					{
						int accountID = row.GetValue<int>("account_id");

						ThreadedMySQL.Query_SELECT(EDatabase.Auth, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts, new List<string> { "username" }, WhereClause.Create("id={0} LIMIT 1", accountID), (CMySQLResult extraSqlResult) =>
						{
							if (extraSqlResult.NumRows() > 0)
							{
								PendingApplication pendingApp = new PendingApplication
								{
									AccountID = accountID,
									AccountName = extraSqlResult.GetRow(0)["username"]
								};

								result.Add(pendingApp);
							}
							else
							{
								// TODO: Fatal error? User doesnt exist?
							}

							++numProcessed;

							// are we done?
							if (numProcessed == numExpected)
							{
								CompletionCallback?.Invoke(result);
							}
						});
					}
				});
			}

			public static void UpdateScripterLevel(EntityDatabaseID a_AccountID, EScripterLevel scripterLevel)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Auth, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Accounts, new Dictionary<string, object>
				{
					{ "scripter", (int)scripterLevel }

				}, WhereClause.Create("id={0}", a_AccountID));
			}

			public static void SetTutorialCompleted(int a_AccountID)
			{
				ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Tutorial_State, WhereClause.Create("account_id={0}", a_AccountID), (CMySQLResult mysqlResult) =>
				{
					ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Tutorial_State, new Dictionary<string, object>
					{
						{"account_id", a_AccountID },
						{"version", TutorialConstants.TutorialVersion }
					}, null);
				});
			}

			public static void UpdateMinutesPlayed(EntityDatabaseID AccountID, uint MinutesToAdd)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts, new Dictionary<string, object>
				{
					{ "minutes_played", SqlFieldOperation.Create("minutes_played+{0}", MinutesToAdd) }

				}, WhereClause.Create("account_id={0}", AccountID));
			}

			public static void RemoveExpiredSessions(Action CompletionCallback)
			{
				const int SessionExpiryTimeDays = 14;
				const int expiryTime = (SessionExpiryTimeDays * (24 * 60)) * 60;

				ThreadedMySQL.Query_DELETE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Saved_Sessions, WhereClause.Create("created_timestamp+{0} <= UNIX_TIMESTAMP()", expiryTime), (CMySQLResult result) =>
				{
					CompletionCallback?.Invoke();
				});
			}

			public static void GetAccountControls(EntityDatabaseID a_AccountID, Action<List<GameControlObject>> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Controls, new List<string> { "*" }, WhereClause.Create("account={0}", a_AccountID), (CMySQLResult mysqlResult) =>
				{
					List<GameControlObject> lstControls = new List<GameControlObject>();

					if (mysqlResult.NumRows() == 1)
					{
						string strJsonControls = mysqlResult.GetRow(0).GetValue<string>("controls");
						lstControls = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GameControlObject>>(strJsonControls);
					}

					CompletionCallback?.Invoke(lstControls);
				});
			}

			public static void UpdateSessionExpiryTime(EntityDatabaseID SessionID, Action CompletionCallback)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Saved_Sessions, new Dictionary<string, object> {
				{ "created_timestamp", SqlFieldOperation.Create("UNIX_TIMESTAMP()") }
				}, WhereClause.Create("id={0}", SessionID), (CMySQLResult sqlResult) =>
				{
					CompletionCallback?.Invoke();
				});
			}

			public static void CreateGameAccount(int accountID, string strString, Action CompletionCallback)
			{
				ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Game_Accounts, new Dictionary<string, object> {
				{ "account_id", accountID },
				{ "app_state", EApplicationState.ApplicationApproved },
				{ "num_apps", 0 },
				{ "serial", strString }
			}, (CMySQLResult sqlResult) =>
			{
				CompletionCallback?.Invoke();
			});
			}

			public static void LoginAccount(string strUsername, string strPassword, string strIPAddress, bool bAutoLogin, string strSerial, Action<CLoginResult> CompletionCallback)
			{
				CLoginResult returnValue = new CLoginResult
				{
					m_Result = Auth.ELoginResult.Failed,
					m_UserID = 0,
					m_AdminLevel = EAdminLevel.None,
				};

				void OnComplete()
				{
					NAPI.Task.Run(() =>
					{
						CompletionCallback?.Invoke(returnValue);
					});
				}

				ThreadedMySQL.Query_SELECT(EDatabase.Auth, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Accounts, new List<string> { "id", "username", "admin", "scripter", "password", "activated", "IFNULL(discord, '0') AS discord" }, WhereClause.Create("LOWER(username)=LOWER('{0}') LIMIT 1", strUsername), (CMySQLResult rows) =>
				{
					if (rows.NumRows() == 1)
					{
						string strDBPassword = rows.GetRow(0)["password"];

						Auth.ELoginResult VerifyResult = Auth.VerifyPassword(strPassword, strDBPassword);

						if (VerifyResult == Auth.ELoginResult.Success)
						{
							// Is the account activated?
							bool bActivated = bool.Parse(rows.GetRow(0)["activated"]);
							if (bActivated)
							{
								int accountID = Convert.ToInt32(rows.GetRow(0)["id"]);

								ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Game_Accounts, new List<string> { "app_state", "num_apps", "minutes_played", "admin_report_count", "local_nametag_toggled", "auto_spawn_character", "admin_jail_minutes_remaining", "admin_jail_reason" }, WhereClause.Create("account_id={0} LIMIT 1", accountID), (CMySQLResult gameDataRows) =>
								{
									if (gameDataRows.NumRows() == 0)
									{
										// Create one
										CreateGameAccount(accountID, strSerial, () =>
										{
											// Requery for defaults
											ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Game_Accounts, new List<string> { "app_state", "num_apps", "minutes_played", "admin_report_count", "local_nametag_toggled", "auto_spawn_character", "admin_jail_minutes_remaining", "admin_jail_reason" }, WhereClause.Create("account_id={0} LIMIT 1", accountID), (CMySQLResult gameDataRows) =>
											{
												ProcessGameDataRows(gameDataRows);
											});
										});
									}
									else
									{
										ProcessGameDataRows(gameDataRows);
									}

									void ProcessGameDataRows(CMySQLResult gameDataRows)
									{
										// NOTE: You must update AttemptAutoLogin also if you modify this function
										if (gameDataRows.NumRows() == 1)
										{
											// Load donation inventory
											Database.Functions.Donations.LoadDonationInventory(accountID, (List<DonationInventoryItem> lstDonationInventory) =>
											{
												// Load controls
												GetAccountControls(accountID, (List<GameControlObject> lstGameControls) =>
												{
													returnValue = new CLoginResult
													{
														m_Result = Auth.ELoginResult.Success,
														m_UserID = accountID,
														m_Username = rows.GetRow(0)["username"],
														m_AdminLevel = (EAdminLevel)Convert.ToInt32(rows.GetRow(0)["admin"]),
														m_ScripterLevel = (EScripterLevel)Convert.ToInt32(rows.GetRow(0)["scripter"]),
														appState = (EApplicationState)Convert.ToInt32(gameDataRows.GetRow(0)["app_state"]),
														numApps = Convert.ToUInt32(gameDataRows.GetRow(0)["num_apps"]),
														serial = strSerial,
														ip = strIPAddress,
														minutesPlayed = Convert.ToUInt32(gameDataRows.GetRow(0)["minutes_played"]),
														adminReportCount = Convert.ToInt32(gameDataRows.GetRow(0)["admin_report_count"]),
														localPlayerNametagToggled = bool.Parse(gameDataRows.GetRow(0)["local_nametag_toggled"]),
														discordID = Convert.ToUInt64(rows.GetRow(0)["discord"]),
														autoSpawnCharacter = Convert.ToInt64(gameDataRows.GetRow(0)["auto_spawn_character"]),
														adminJailMinutesRemaining = Convert.ToInt32(gameDataRows.GetRow(0)["admin_jail_minutes_remaining"]),
														adminJailReason = gameDataRows.GetRow(0)["admin_jail_reason"],

													};
													returnValue.m_lstDonationInventory.AddRange(lstDonationInventory);
													returnValue.m_lstControls.AddRange(lstGameControls);

													// Create session (we don't wait for / care about the outcome)
													ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Saved_Sessions, new Dictionary<string, object> {
													{ "ip_address", strIPAddress.ToLower() },
													{ "account_id", accountID },
													{ "created_timestamp", "UNIX_TIMESTAMP()" },
													{ "serial", strSerial.ToUpper() }
												}, null);

													OnComplete();
												});
											});
										}
										else
										{
											// TODO_LAUNCH: Log this, this is a fatal error
											returnValue = new CLoginResult
											{
												m_Result = Auth.ELoginResult.AccountDoesNotExist,
												m_UserID = 0,
												m_AdminLevel = EAdminLevel.None,
												m_ScripterLevel = EScripterLevel.None
											};
											OnComplete();
										}
									}
								});
							}
							else
							{
								returnValue = new CLoginResult
								{
									m_Result = Auth.ELoginResult.NotActivated,
									m_UserID = 0,
									m_AdminLevel = EAdminLevel.None,
									m_ScripterLevel = EScripterLevel.None
								};
								OnComplete();
							}
						}
						else
						{
							returnValue.m_Result = VerifyResult;
							OnComplete();
						}
					}
					else
					{
						OnComplete();
					}
				});
			}

			public static void AttemptAutoLogin(string strIPAddress, string strSerial, Action<CLoginResult> CompletionCallback)
			{
				CLoginResult returnValue = new CLoginResult
				{
					m_Result = Auth.ELoginResult.Failed,
					m_UserID = 0,
					m_AdminLevel = EAdminLevel.None,
				};

				void OnComplete()
				{
					NAPI.Task.Run(() =>
					{
						CompletionCallback?.Invoke(returnValue);
					});
				}

				RemoveExpiredSessions(() =>
				{
					ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Saved_Sessions, new List<string> { "id", "account_id" }, WhereClause.Create("LOWER(ip_address)=LOWER('{0}') && UPPER(serial)=UPPER('{1}')", strIPAddress, strSerial), (CMySQLResult sqlResult) =>
					{
						if (sqlResult.NumRows() >= 1)
						{
							int accountID = Convert.ToInt32(sqlResult.GetRow(0)["account_id"]);

							ThreadedMySQL.Query_SELECT(EDatabase.Auth, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Accounts, new List<string> { "username", "admin", "scripter", "IFNULL(discord, '0') AS discord" }, WhereClause.Create("id={0} LIMIT 1", accountID), (CMySQLResult extraSqlResult) =>
							{
								// TODO_POST_LAUNCH: Normal login does an insert of defaults into game_accounts if not present, do we need that here? probably not since they must log in once manually

								ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Game_Accounts, new List<string> { "app_state", "num_apps", "minutes_played", "admin_report_count", "local_nametag_toggled", "auto_spawn_character", "admin_jail_minutes_remaining", "admin_jail_reason" }, WhereClause.Create("account_id={0} LIMIT 1", accountID), (CMySQLResult gameDataRows) =>
								{
									// NOTE: You must update LoginAccount also if you change this function
									if (extraSqlResult.NumRows() >= 1 && gameDataRows.NumRows() >= 1)
									{
										// TODO_POST_LAUNCH: Merge the shared logic between here and LoginPlayer so we dont have to keep duplicating code...
										// Load donation inventory
										Database.Functions.Donations.LoadDonationInventory(accountID, (List<DonationInventoryItem> lstDonationInventory) =>
										{
											// Load controls
											GetAccountControls(accountID, (List<GameControlObject> lstGameControls) =>
											{
												returnValue = new CLoginResult
												{
													m_Result = Auth.ELoginResult.Success,
													m_UserID = accountID,
													m_Username = extraSqlResult.GetRow(0)["username"],
													m_AdminLevel = (EAdminLevel)Convert.ToInt32(extraSqlResult.GetRow(0)["admin"]),
													m_ScripterLevel = (EScripterLevel)Convert.ToInt32(extraSqlResult.GetRow(0)["scripter"]),
													appState = (EApplicationState)Convert.ToInt32(gameDataRows.GetRow(0)["app_state"]),
													numApps = Convert.ToUInt32(gameDataRows.GetRow(0)["num_apps"]),
													ip = strIPAddress,
													serial = strSerial,
													minutesPlayed = Convert.ToUInt32(gameDataRows.GetRow(0)["minutes_played"]),
													localPlayerNametagToggled = bool.Parse(gameDataRows.GetRow(0)["local_nametag_toggled"]),
													adminReportCount = Convert.ToInt32(gameDataRows.GetRow(0)["admin_report_count"]),
													discordID = Convert.ToUInt64(extraSqlResult.GetRow(0)["discord"]),
													autoSpawnCharacter = Convert.ToInt64(gameDataRows.GetRow(0)["auto_spawn_character"]),
													adminJailMinutesRemaining = Convert.ToInt32(gameDataRows.GetRow(0)["admin_jail_minutes_remaining"]),
													adminJailReason = gameDataRows.GetRow(0)["admin_jail_reason"],
												};
												returnValue.m_lstDonationInventory.AddRange(lstDonationInventory);
												returnValue.m_lstControls.AddRange(lstGameControls);

												EntityDatabaseID sessionID = (EntityDatabaseID)Convert.ChangeType(sqlResult.GetRow(0)["id"], typeof(EntityDatabaseID));

												// we don't care about the result or waiting on this one
												UpdateSessionExpiryTime(sessionID, null);

												OnComplete();
											});
										});
									}
									else
									{
										OnComplete();
									}
								});
							});
						}
						else
						{
							OnComplete();
						}
					});
				});
			}
			
			public static void GetAccountIdFromName(string accountName, Action<EntityDatabaseID> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Auth, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Accounts, new List<string> { "id" }, WhereClause.Create("username='{0}' LIMIT 1;", accountName), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					EntityDatabaseID databaseID = -1;
					if (mysqlResult.NumRows() >= 1)
					{
						databaseID = (EntityDatabaseID)Convert.ChangeType(mysqlResult.GetRow(0)["id"], typeof(EntityDatabaseID));
					}

					CompletionCallback(databaseID);
				});
			}
			
			public static void GetJailInformationFromAccountName(string strName, Action<EntityDatabaseID, int, string> CompletionCallback)
			{
				GetAccountIdFromName(strName, accountID =>
				{
					ThreadedMySQL.Query_SELECT(
						EDatabase.Game,
						EThreadContinuationFlag.ContinueOnMainThread,
						TableNames.Game_Accounts,
						new List<string> { "account_id", "admin_jail_minutes_remaining", "admin_jail_reason" },
						WhereClause.Create("account_id={0} LIMIT 1;", accountID),
						(CMySQLResult mysqlResult) => // NOTE: on main thread at this point
						{
							EntityDatabaseID databaseID = -1;
							int minutes = -1;
							string reason = "";
							if (mysqlResult.NumRows() >= 1)
							{
								databaseID = (EntityDatabaseID)Convert.ChangeType(mysqlResult.GetRow(0)["account_id"], typeof(EntityDatabaseID));
								minutes = mysqlResult.GetRow(0).GetValue<int>("admin_jail_minutes_remaining");
								reason = mysqlResult.GetRow(0).GetValue<string>("admin_jail_reason");
							}

							CompletionCallback(databaseID, minutes, reason);
						}
					);
				});
			}

			public static void SetAdminJailInformation(EntityDatabaseID accountID, int timeRemaining, string reason, Action CompletionCallback)
			{
				ThreadedMySQL.Query_UPDATE(
					EDatabase.Game,
					EThreadContinuationFlag.ContinueOnMainThread,
					TableNames.Game_Accounts,
					new Dictionary<string, object>
					{
						{"admin_jail_minutes_remaining", timeRemaining},
						{"admin_jail_reason", reason}
					},
					WhereClause.Create("account_id={0}", accountID),
					result => CompletionCallback()
				);
			}
		}
	}
}