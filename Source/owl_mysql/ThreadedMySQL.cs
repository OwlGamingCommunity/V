#if DEBUG
//#define USE_SINGLE_THREADED_MYSQL
#endif

using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Database
{
	public enum TableNames
	{
		Accounts,
		Achievements,
		Banks,
		Carwash_Points,
		Characters,
		Characters_Custom_Data,
		Characters_Tattoos,
		Character_Kills,
		Character_Languages,
		Chat_Settings,
		Corpses,
		Custom_Interior_Maps,
		Custom_Interior_Objects,
		Dancers,
		Disallowed_Character_Names,
		Donation_Inventory,
		Donation_Store,
		Duty_Points,
		Elevators,
		Emails,
		Email_Accounts,
		Email_Contacts,
		Email_Domains,
		Email_Recipients,
		Factions,
		Faction_Invites,
		Faction_Memberships,
		Faction_Ranks,
		Fuel_Points,
		Game_Accounts,
		Game_Controls,
		Gangtags,
		Globals,
		Inventories,
		Keybinds,
		Metal_Detectors,
		Phone_Contacts,
		Phone_SMS,
		Player_Admin_History,
		Properties,
		Property_Notes,
		Radios,
		Saved_Sessions,
		Scooter_Rental_Shops,
		Stores,
		Teleport_Places,
		Terminal_Crimes,
		Terminal_Crime_Officers_Involved,
		Termanal_Logs,
		Terminal_Notes,
		Terminal_Person_Details,
		Terminal_Users,
		Tutorial_State,
		VehicleRepair_Points,
		Vehicles,
		Vehicle_Mods,
		Vehicle_Notes,
		World_Blips,
		Vehicle_Extras,
		Notifications,
		PerfCaptures,
		InfoMarkers,
		Character_Looks,
		Custom_Anims
	}

	public enum EThreadContinuationFlag
	{
		ContinueOnMainThread,
		ContinueOnQueryThread
	}

	public enum EDatabase
	{
		Auth,
		Game
	}

	public class SqlFieldOperation
	{
		public string FieldOperation { get; set; } = String.Empty;

		public static SqlFieldOperation Create(string strFieldOperationFormat, params object[] FormatParams)
		{
			SqlFieldOperation newOp = new SqlFieldOperation();

			// Escape everything
			for (int i = 0; i < FormatParams.Length; ++i)
			{
				if (FormatParams[i].GetType() == typeof(string))
				{
					FormatParams[i] = MySqlHelper.EscapeString((string)FormatParams[i]);
				}
				else if (FormatParams[i].GetType().IsEnum)
				{
					FormatParams[i] = (int)FormatParams[i];
				}
			}

			newOp.FieldOperation = Helpers.FormatString(strFieldOperationFormat, FormatParams);
			return newOp;
		}
	}

	public class WhereClause
	{
		public string Value { get; set; } = String.Empty;

		public static WhereClause Create(string strFormat, params object[] WhereParams)
		{
			WhereClause newClause = new WhereClause();

			// Escape everything
			for (int i = 0; i < WhereParams.Length; ++i)
			{
				WhereParams[i] = FormatParam(WhereParams[i]);
			}

			newClause.Value = Helpers.FormatString(strFormat, WhereParams);
			return newClause;
		}

		private static object FormatParam(object value)
		{
			if (value is string)
			{
				return MySqlHelper.EscapeString((string)value);
			}
			if (value.GetType().IsEnum)
			{
				return (int)value;
			}
			if (value is long[])
			{
				string output = "";
				long[] valArray = (long[])value;
				for (int i = 0; i < valArray.Length; ++i)
				{
					if (i > 0)
					{
						output += ", ";
					}

					output += FormatParam(valArray[i]);
				}

				return output;
			}

			return value;
		}
	}

	// TODO THREADED SQL: Move all our CDatabaseStructure classes to ref structs once we have completely moved over to the threaded sql
	public class ThreadedMySQLConnectionPool : IDisposable
	{
		private List<ThreadedMySQLConnection> m_lstConnectionPool = new List<ThreadedMySQLConnection>();
		private Mutex g_FreeConnectionsMutex = new Mutex();
		private Queue<ThreadedMySQLConnection> m_queueFreeConnections = new Queue<ThreadedMySQLConnection>();
		private Queue<QueuedMySQLQuery> m_lstPendingQueries = new Queue<QueuedMySQLQuery>();

		private Mutex g_PendingQueriesMutex = new Mutex();
		public bool HasPendingQueries()
		{
			g_PendingQueriesMutex.WaitOne();
			g_FreeConnectionsMutex.WaitOne();

			// we must have no pending queries and free connections should be the entire pool
			bool bHasPending = m_lstPendingQueries.Count > 0 || m_queueFreeConnections.Count != m_lstConnectionPool.Count;

			g_PendingQueriesMutex.ReleaseMutex();
			g_FreeConnectionsMutex.ReleaseMutex();
			return bHasPending;
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool a_CleanupNativeAndManaged)
		{
			g_FreeConnectionsMutex.Dispose();
			g_PendingQueriesMutex.Dispose();
		}

		public List<string> GetDebugStats()
		{
			g_PendingQueriesMutex.WaitOne();
			g_FreeConnectionsMutex.WaitOne();

			List<string> lstDebugStats = new List<string>();
			lstDebugStats.Add(Helpers.FormatString("Database {0} has {1} connections ({2} are idle/free) and {3} queries pending", m_lstConnectionPool[0].GetDatabaseName(), m_lstConnectionPool.Count, m_queueFreeConnections.Count, m_lstPendingQueries.Count));

			foreach (ThreadedMySQLConnection conn in m_lstConnectionPool)
			{
				lstDebugStats.Add(Helpers.FormatString("[{0}] {1} Connected: {2} State: {3}!", conn.GetID(), conn.GetDatabaseName(), conn.IsConnected(), conn.GetState()));
			}

			g_PendingQueriesMutex.ReleaseMutex();
			g_FreeConnectionsMutex.ReleaseMutex();
			return lstDebugStats;
		}

		private class ThreadedMySQLConnection : IDisposable
		{
			private int m_CurrentQueryAttempts = 0;
			private int m_ID = -1;
			private MySqlConnection m_connection = null;

#if !USE_SINGLE_THREADED_MYSQL
			private Thread m_Thread = null;
			private AutoResetEvent m_Signal = new AutoResetEvent(false); // not signalled by default
#endif

			private QueuedMySQLQuery m_CurrentQuery = null;
			private Action<CMySQLResult, bool> m_CompletionCallback = null;
			private string m_strDatabaseName = String.Empty;

			public ThreadedMySQLConnection(int ID, string strIP, string strDatabaseName, string strUsername, string strPassword, string strPort)
			{
				m_ID = ID;
				m_strDatabaseName = strDatabaseName;
				m_connection = new MySqlConnection(Helpers.FormatString(
					"Server={0}; database={1}; UID={2}; password={3}; port={4}; ConnectionTimeout=10; Pooling=False;", strIP, strDatabaseName, strUsername, strPassword, strPort));
				Connect();

				// spawn thread
#if !USE_SINGLE_THREADED_MYSQL
				m_Thread = new Thread(ThreadedTick);
				m_Thread.Name = Helpers.FormatString("[{1}] Threaded SQL Thread {0}", m_ID, strDatabaseName);
				m_Thread.Start();
#endif

				m_connection.StateChange += OnConnectionStateChange;
			}

			private void OnConnectionStateChange(object sender, System.Data.StateChangeEventArgs e)
			{
				if (e.CurrentState == System.Data.ConnectionState.Closed || e.CurrentState == System.Data.ConnectionState.Broken)
				{
					Reconnect();
				}
			}

			public string GetDatabaseName()
			{
				return m_strDatabaseName;
			}

			public int GetID()
			{
				return m_ID;
			}

			public bool IsConnected()
			{
				return m_connection != null && m_connection.State != System.Data.ConnectionState.Closed && m_connection.State != System.Data.ConnectionState.Broken;
			}

			public System.Data.ConnectionState GetState()
			{
				return m_connection.State;
			}

			~ThreadedMySQLConnection()
			{
				if (IsConnected())
				{
					m_connection.Close();
				}
			}

			private async void Connect()
			{
				try
				{
					NAPI.Util.ConsoleOutput("[{1}] MySQL Connecting on thread {0}", m_ID, m_strDatabaseName);
					m_connection.Open();
					NAPI.Util.ConsoleOutput("[{1}] MySQL Connected successfully on thread {0}", m_ID, m_strDatabaseName);
				}
				catch (Exception ex)
				{
					if (ex.GetType() == typeof(System.InvalidOperationException))
					{
						// this exception means we WERE actually still connected...
						NAPI.Util.ConsoleOutput("[{1}] MySQL Connected successfully on thread {0}", m_ID, m_strDatabaseName);
					}
					else
					{
						// sleep and retry in 2 sec
						NAPI.Util.ConsoleOutput("[{1}] Connection {0} failed... retrying in 2 sec", m_ID, m_strDatabaseName);
						await Task.Delay(2000).ConfigureAwait(true);
						Connect();
					}
				}
			}

			public bool NotifyWork(QueuedMySQLQuery query, Action<CMySQLResult, bool> CompletionCallback)
			{
				if (IsConnected())
				{
					m_CurrentQueryAttempts = 0;
					m_CurrentQuery = query;
					m_CompletionCallback = CompletionCallback;

#if !USE_SINGLE_THREADED_MYSQL
					m_Signal.Set();
#endif
					return true;
				}

				return false;
			}
			// TODO_THREADED_SQL: Don't let shutdown occur until all queries are done?
			public void ThreadedTick()
			{
#if !USE_SINGLE_THREADED_MYSQL
				NAPI.Util.ConsoleOutput("[{1}] Thread {0} started...", m_ID, m_strDatabaseName);

				// register it
				ThreadedMySQL.Debug_SetActiveQuery(Thread.CurrentThread.ManagedThreadId, "NONE");

				while (Thread.CurrentThread.IsAlive)
#endif
				{
					ProcessCurrentQuery();
				}
			}

			public async void ProcessCurrentQuery()
			{
#if !USE_SINGLE_THREADED_MYSQL
				m_Signal.WaitOne();
#else
				if (m_CurrentQuery == null)
				{
					return;
				}
#endif
				if (m_CurrentQuery == null)
				{
					return;
				}

				string strQuery = m_CurrentQuery.GetQueryString();
				ThreadedMySQL.Debug_AddRecentQuery(strQuery);

				// set active query
				ThreadedMySQL.Debug_SetActiveQuery(Thread.CurrentThread.ManagedThreadId, strQuery);

				MySqlCommand cmd = m_connection.CreateCommand();
				cmd.CommandText = strQuery;

				CMySQLResult result = null;

				const int NumAttempts = 5;

				// TODO_POST_LAUNCH: Better way of doing this than string compare?
				if (strQuery.StartsWith("DELETE") || strQuery.StartsWith("UPDATE"))
				{
					try
					{
						int numRowsModified = await cmd.ExecuteNonQueryAsync().ConfigureAwait(true);
						result = new CMySQLResult(numRowsModified);
					}
					catch (System.InvalidOperationException)
					{
						// TODO_MYSQL: Is it safe to assume that the entire operation failed? e.g. we don't insert twice
						// Try to reconnect and reissue
						PrintLogger.LogMessage(ELogSeverity.HIGH, "MySQL {0} [{1}] is attempting to reconnect", m_ID, m_strDatabaseName);
						Reconnect(); // query will be retried here because it isnt nulled out

						return;
					}
					catch (Exception e)
					{
						// TODO: Store this somewhere important, it's a pretty critical error
						PrintLogger.LogMessage(ELogSeverity.ERROR, "MySQL {2} Query Error [{1}]: {0}", e.InnerException == null ? e.Message : e.InnerException.ToString(), m_strDatabaseName, m_ID);

						// If we have a debugger, re-throw
						if (System.Diagnostics.Debugger.IsAttached)
						{
							throw;
						}

						return;
					}
				}
				else
				{
					try
					{
						++m_CurrentQueryAttempts;
						System.Data.Common.DbDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(true);
						result = new CMySQLResult(reader, (ulong)cmd.LastInsertedId);
					}
					catch (System.InvalidOperationException)
					{
						// TODO_MYSQL: Is it safe to assume that the entire operation failed? e.g. we don't insert twice
						// Try to reconnect and reissue
						if (m_CurrentQueryAttempts >= NumAttempts)
						{
							// give up, throw exception and put it back in the ready pool
							OnQueryComplete(null, false);
						}
						else
						{
							PrintLogger.LogMessage(ELogSeverity.HIGH, "MySQL {0} [{1}] is attempting to reconnect", m_ID, m_strDatabaseName);
							Reconnect(); // query will be retried here because it isnt nulled out
							m_Signal.Set();
						}

						return;
					}
					catch (Exception e)
					{
						// TODO: Store this somewhere important, it's a pretty critical error
						if (m_CurrentQueryAttempts >= NumAttempts)
						{
							// give up, throw exception and put it back in the ready pool
							OnQueryComplete(null, false);

							PrintLogger.LogMessage(ELogSeverity.ERROR, "MySQL {2} Query Error [{1}]: {0}", e.InnerException == null ? e.Message : e.InnerException.ToString(), m_strDatabaseName, m_ID);

							// If we have a debugger, re-throw
							if (System.Diagnostics.Debugger.IsAttached)
							{
								throw;
							}
						}
						else
						{
							// query will be retried here because it isnt nulled out 
							m_Signal.Set();
						}


						return;
					}
				}

				// query done
				OnQueryComplete(result, true);
			}

			private void Reconnect()
			{
				Connect();
			}

			private void OnQueryComplete(CMySQLResult result, bool bSucceeded)
			{
				// clear active query
				ThreadedMySQL.Debug_SetActiveQuery(Thread.CurrentThread.ManagedThreadId, "NONE");

				if (m_CompletionCallback != null)
				{
					m_CompletionCallback(result, bSucceeded);
				}
				m_CurrentQuery = null;
			}

			public void Dispose()
			{
				Dispose(true);

				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool a_CleanupNativeAndManaged)
			{
				m_connection.Dispose();
			}
		}

		private class QueuedMySQLQuery
		{
			private string m_strQuery = String.Empty;
			private Action<CMySQLResult> m_Callback = null;
			private EThreadContinuationFlag m_ThreadContinuation;

			public QueuedMySQLQuery(string strQuery, Action<CMySQLResult> Callback, EThreadContinuationFlag ThreadContinuation)
			{
				m_strQuery = strQuery;
				m_Callback = Callback;
				m_ThreadContinuation = ThreadContinuation;
			}

			public string GetQueryString()
			{
				return m_strQuery;
			}

			public void TriggerCallback(CMySQLResult result)
			{
				if (m_Callback != null)
				{
					// if null, give back an empty result so we dont hang a UI or anything like that
					if (result == null)
					{
						result = new CMySQLResult(0);
					}

					if (m_ThreadContinuation == EThreadContinuationFlag.ContinueOnQueryThread)
					{
#if USE_SINGLE_THREADED_MYSQL
						// In single threaded mode, we have to force back to the main thread here otherwise continuation queries will stack overflow
						NAPI.Task.Run(() =>
						{
#endif
						m_Callback(result);
#if USE_SINGLE_THREADED_MYSQL
						});
#endif
					}
					else if (m_ThreadContinuation == EThreadContinuationFlag.ContinueOnMainThread)
					{
						NAPI.Task.Run(() =>
						{
							m_Callback(result);
						});
					}
				}
			}
		}

		public ThreadedMySQLConnectionPool(int numConnections, string strIP, string strDatabaseName, string strUsername, string strPassword, string strPort)
		{
			Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			// TODO: Keep-alive logic
			for (int i = 0; i < numConnections; ++i)
			{
				ThreadedMySQLConnection newConnection = new ThreadedMySQLConnection(i, strIP, strDatabaseName, strUsername, strPassword, strPort);
				m_lstConnectionPool.Add(newConnection);

				// mark as free
				g_FreeConnectionsMutex.WaitOne();
				m_queueFreeConnections.Enqueue(newConnection);
				g_FreeConnectionsMutex.ReleaseMutex();
			}

			NAPI.Util.ConsoleOutput("[{1}] Initialized MySQL Connection Pool with {0} threads/connections", numConnections, strDatabaseName);
		}

		public void Tick()
		{
#if USE_SINGLE_THREADED_MYSQL
			foreach (ThreadedMySQLConnection conn in m_lstConnectionPool)
			{
				conn.ThreadedTick();
			}
#endif

			// Do we have pending work? dispatch as much as we can
			g_PendingQueriesMutex.WaitOne();
			g_FreeConnectionsMutex.WaitOne();

			while (m_lstPendingQueries.Count > 0) // while we have queries, try to dispatch
												  //if (m_lstPendingQueries.Count > 0)
			{
				// Do we have a thread available to process the query?
				if (m_queueFreeConnections.Count > 0)
				{
					QueuedMySQLQuery query = m_lstPendingQueries.Dequeue();

					ThreadedMySQLConnection handler = m_queueFreeConnections.Dequeue();
					bool bHandlerAcceptedWork = handler.NotifyWork(query, (CMySQLResult result, bool bSucceeded) => // finished callback (NOTE: NOT ON MAIN THREAD!)
					{
						// only trigger callback if we succeeded
						if (bSucceeded)
						{
							query.TriggerCallback(result);
						}

						// add the connection back into the free pool
						// always do this, even when we fail!
						g_FreeConnectionsMutex.WaitOne();
						m_queueFreeConnections.Enqueue(handler);
						g_FreeConnectionsMutex.ReleaseMutex();
					});

					// if the work wasn't accepted, requeue it and add the handler back to pool (at the end)
					if (!bHandlerAcceptedWork)
					{
						m_queueFreeConnections.Enqueue(handler);
						m_lstPendingQueries.Enqueue(query);
					}
				}
				else
				{
					// we've used all of our connections
					break;
				}
			}
			g_FreeConnectionsMutex.ReleaseMutex();
			g_PendingQueriesMutex.ReleaseMutex();
		}

		public void QueueDistributedQuery(string strQuery, Action<CMySQLResult> Callback, EThreadContinuationFlag ThreadContinuation)
		{
			// queue the work
			g_PendingQueriesMutex.WaitOne();
			QueuedMySQLQuery query = new QueuedMySQLQuery(strQuery, Callback, ThreadContinuation);
			m_lstPendingQueries.Enqueue(query);
			g_PendingQueriesMutex.ReleaseMutex();
		}
	}

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
	public class ThreadedMySQL
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
	{
		private static Mutex g_ConnectionPoolsMutex = new Mutex();
		private static Mutex g_DebugRecentQueriesMutex = new Mutex();
		private static Queue<string> g_DebugRecentQueries = new Queue<string>();
		private static Dictionary<int, string> g_DebugActiveQueries = new Dictionary<int, string>();

		// Game DB
		private static ThreadedMySQLConnectionPool m_ConnectionPool_Game = new ThreadedMySQLConnectionPool(
#if USE_SINGLE_THREADED_MYSQL
				1,
#elif DEBUG
	Math.Min(4, Environment.ProcessorCount - 1),
#else
				Environment.ProcessorCount - 1, // spawn hyperthreaded cores - 1 so we dont swamp / run on main thread (well only on the virtual core)
#endif
				// TODO_GITHUB: You need to set the below environment variables / variables to your GAME database connection info
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_IP") ?? "127.0.0.1",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_NAME") ?? "gtav",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_USERNAME") ?? "root",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_PASSWORD") ?? "example",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_PORT") ?? "3306"
				);

		// Auth DB
		private static ThreadedMySQLConnectionPool m_ConnectionPool_Auth = new ThreadedMySQLConnectionPool(
#if USE_SINGLE_THREADED_MYSQL
				1,
#elif DEBUG
	Math.Min(4, Environment.ProcessorCount - 1),
#else
				Environment.ProcessorCount - 1, // spawn hyperthreaded cores - 1 so we dont swamp / run on main thread (well only on the virtual core)
#endif

				// TODO_GITHUB: You need to set the below environment variables / variables to your AUTH database connection info
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_IP") ?? "127.0.0.1",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_NAME") ?? "core",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_USERNAME") ?? "root",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_PASSWORD") ?? "example",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_PORT") ?? "3307"
				);

		public ThreadedMySQL()
		{

		}

		public static void Tick()
		{
			m_ConnectionPool_Game.Tick();
			m_ConnectionPool_Auth.Tick();
			Database.LegacyFunctions.Tick();
		}

		public static bool HasPendingQueries()
		{
			g_ConnectionPoolsMutex.WaitOne();
			bool bHasPending = m_ConnectionPool_Game.HasPendingQueries() || m_ConnectionPool_Auth.HasPendingQueries();
			g_ConnectionPoolsMutex.ReleaseMutex();
			return bHasPending;
		}

		public static void Debug_GetRecentQueries(out Queue<String> recentQueries, out Dictionary<int, string> activeQueries)
		{
			g_DebugRecentQueriesMutex.WaitOne();
			recentQueries = g_DebugRecentQueries;
			activeQueries = g_DebugActiveQueries;
			g_DebugRecentQueriesMutex.ReleaseMutex();
		}

		public static void Debug_AddRecentQuery(string strQuery)
		{
			// pop front
			g_DebugRecentQueriesMutex.WaitOne();
			if (g_DebugRecentQueries.Count >= 10)
			{
				g_DebugRecentQueries.Dequeue();
			}

			g_DebugRecentQueries.Enqueue(strQuery);
			g_DebugRecentQueriesMutex.ReleaseMutex();
		}

		public static void Debug_SetActiveQuery(int threadID, string strQuery)
		{
			g_DebugRecentQueriesMutex.WaitOne();
			g_DebugActiveQueries[threadID] = strQuery;
			g_DebugRecentQueriesMutex.ReleaseMutex();
		}

		public static void Query_SELECT(EDatabase Database, EThreadContinuationFlag ThreadContinuation, TableNames tableName, List<string> lstFields, WhereClause a_WhereClause = null, Action<CMySQLResult> CompletionCallback = null)
		{
			string strFields = String.Join(", ", lstFields);
			string strQuery = Helpers.FormatString("SELECT {0} FROM `{1}` {2};", strFields, tableName.ToString().ToLower(), (a_WhereClause != null && a_WhereClause.Value.Length > 0) ? Helpers.FormatString("WHERE {0}", a_WhereClause.Value) : "");

			Query_Internal(Database, strQuery, CompletionCallback, ThreadContinuation);
		}

		public static List<string> GetDebugStats()
		{
			List<string> lstStats = m_ConnectionPool_Game.GetDebugStats();
			lstStats.AddRange(m_ConnectionPool_Auth.GetDebugStats());
			return lstStats;
		}

		public static void Query_DELETE(EDatabase Database, EThreadContinuationFlag ThreadContinuation, TableNames tableName, WhereClause a_WhereClause = null, Action<CMySQLResult> CompletionCallback = null)
		{
			string strQuery = Helpers.FormatString("DELETE FROM `{0}` {1};", tableName.ToString().ToLower(), (a_WhereClause != null && a_WhereClause.Value.Length > 0) ? Helpers.FormatString("WHERE {0}", a_WhereClause.Value) : "");

			Query_Internal(Database, strQuery, CompletionCallback, ThreadContinuation);
		}

		public static void Query_INSERT(EDatabase Database, EThreadContinuationFlag ThreadContinuation, TableNames tableName, Dictionary<string, object> dictKVPairsToUpdate, Action<CMySQLResult> CompletionCallback = null)
		{
			string strFields = null;
			string strValues = null;
			foreach (var kvPair in dictKVPairsToUpdate)
			{
				object objValue = kvPair.Value;

				if (strFields != null)
				{
					strFields += ", ";
				}

				if (strValues != null)
				{
					strValues += ", ";
				}

				// Escape everything
				if (kvPair.Value.GetType() == typeof(string))
				{
					objValue = MySqlHelper.EscapeString((string)objValue);
				}
				else if (kvPair.Value.GetType().IsEnum)
				{
					objValue = (int)objValue;
				}

				string strFormattedValue = objValue.GetType() == typeof(string) ? (objValue.ToString().EndsWith("()") ? objValue.ToString() : Helpers.FormatString("'{0}'", objValue)) : objValue.ToString();

				strFields += Helpers.FormatString("`{0}`", kvPair.Key);
				strValues += Helpers.FormatString("{0}", strFormattedValue);
			}

			string strQuery = Helpers.FormatString("INSERT INTO `{0}` ({1}) VALUES ({2});", tableName.ToString().ToLower(), strFields, strValues);

			Query_Internal(Database, strQuery, CompletionCallback, ThreadContinuation);
		}

		public static void BulkQuery_INSERT(EDatabase Database, EThreadContinuationFlag ThreadContinuation, TableNames tableName, List<string> lstFields, List<List<object>> lstDataRows, Action<CMySQLResult> CompletionCallback = null)
		{
			// TODO_MYSQL: Assert or error if lstFields count doesnt match per row count of lstDataRows
			string strFields = String.Join(", ", lstFields);
			List<string> lstValues = new List<string>();

			foreach (var insertRow in lstDataRows)
			{
				string strValuesThisRow = null;

				// each value for this row
				foreach (object objValue in insertRow)
				{
					object objValueScrubbed = objValue;

					if (strValuesThisRow != null)
					{
						strValuesThisRow += ", ";
					}

					// Escape everything
					if (objValueScrubbed.GetType() == typeof(string))
					{
						objValueScrubbed = MySqlHelper.EscapeString((string)objValueScrubbed);
					}
					else if (objValueScrubbed.GetType().IsEnum)
					{
						objValueScrubbed = (int)objValueScrubbed;
					}

					string strFormattedValue = objValueScrubbed.GetType() == typeof(string) ? (objValueScrubbed.ToString().EndsWith("()") ? objValueScrubbed.ToString() : Helpers.FormatString("'{0}'", objValueScrubbed)) : objValueScrubbed.ToString();

					strValuesThisRow += Helpers.FormatString("{0}", strFormattedValue);
				}

				lstValues.Add(strValuesThisRow);
			}

			string strQuery = Helpers.FormatString("INSERT INTO `{0}` ({1}) VALUES ", tableName.ToString().ToLower(), strFields);

			int index = 0;
			foreach (string strValues in lstValues)
			{
				if (index > 0)
				{
					strQuery += ",";
				}

				strQuery += Helpers.FormatString("({0})", strValues);
				++index;
			}
			strQuery += ";";

			Query_Internal(Database, strQuery, CompletionCallback, ThreadContinuation);
		}

		private static bool IsReservedValueKeyword(string str)
		{
			return str.EndsWith("()") || str.ToUpper() == "CURRENT_TIMESTAMP";
		}




		public static void Query_UPDATE(EDatabase Database, EThreadContinuationFlag ThreadContinuation, TableNames tableName, Dictionary<string, object> dictKVPairsToUpdate, WhereClause a_WhereClause = null, Action<CMySQLResult> CompletionCallback = null)
		{
			string strFieldsAndValues = null;
			foreach (var kvPair in dictKVPairsToUpdate)
			{
				object objValue = kvPair.Value;

				if (strFieldsAndValues != null)
				{
					strFieldsAndValues += ", ";
				}

				// Escape everything
				if (kvPair.Value.GetType() == typeof(string))
				{
					objValue = MySqlHelper.EscapeString((string)objValue);
				}
				else if (kvPair.Value.GetType().IsEnum)
				{
					objValue = (int)objValue;
				}

				string strFormattedValue = null;

				if (objValue.GetType() == typeof(SqlFieldOperation))
				{
					strFormattedValue = ((SqlFieldOperation)objValue).FieldOperation;
				}
				else if (IsReservedValueKeyword(objValue.ToString()))
				{
					strFormattedValue = objValue.ToString();
				}
				else if (objValue.GetType() == typeof(string))
				{
					strFormattedValue = Helpers.FormatString("'{0}'", objValue);
				}
				else
				{
					strFormattedValue = objValue.ToString();
				}

				strFieldsAndValues += Helpers.FormatString("`{0}`={1}", kvPair.Key, strFormattedValue);
			}

			if (strFieldsAndValues == null)
			{
				// TODO: Log to sentry
				return;
			}

			string strQuery = Helpers.FormatString("UPDATE `{0}` SET {1} {2};", tableName.ToString().ToLower(), strFieldsAndValues, (a_WhereClause != null && a_WhereClause.Value.Length > 0) ? Helpers.FormatString("WHERE {0}", a_WhereClause.Value) : "");

			Query_Internal(Database, strQuery, CompletionCallback, ThreadContinuation);
		}

		private static void Query_Internal(EDatabase Database, string strQuery, Action<CMySQLResult> CompletionCallback, EThreadContinuationFlag ThreadContinuation)
		{
			PrintLogger.LogMessage(ELogSeverity.DATABASE_DEBUG, "MYSQL QUERY: {0}", strQuery);

			g_ConnectionPoolsMutex.WaitOne();
			if (Database == EDatabase.Game)
			{
				m_ConnectionPool_Game.QueueDistributedQuery(strQuery, CompletionCallback, ThreadContinuation);
			}
			else
			{
				m_ConnectionPool_Auth.QueueDistributedQuery(strQuery, CompletionCallback, ThreadContinuation);
			}
			g_ConnectionPoolsMutex.ReleaseMutex();
		}
	}
}