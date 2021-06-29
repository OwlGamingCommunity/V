//#define USE_OLD_DB_SYSTEM

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Database
{
	public class MySQLInstance : IDisposable
	{
		private MySqlConnection m_Connection_Game = null;
		private MySqlConnection m_Connection_Auth = null;

		// TODO: We can optimize this more... make it lockless
		private static Mutex g_MutexGame = new Mutex();
		private static Mutex g_MutexAuth = new Mutex();

		private ThreadChecker m_ThreadChecker = new ThreadChecker();

		public MySQLInstance()
		{
			Initialize();
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool a_CleanupNativeAndManaged)
		{
			m_Connection_Game.Dispose();
			m_Connection_Auth.Dispose();
		}

		private DateTime m_LastQueryTime_Auth = DateTime.Now;
		private DateTime m_LastQueryTime_Game = DateTime.Now;
		public async void KeepAlive()
		{
			m_ThreadChecker.IsOnMainThread_LogIfNot();

			double timeSinceLastQueryAuth = (DateTime.Now - m_LastQueryTime_Auth).TotalMilliseconds;
			if (timeSinceLastQueryAuth > 300000)
			{
				await QueryAuth("SELECT id FROM accounts LIMIT 1;").ConfigureAwait(true);
			}

			double timeSinceLastQueryGame = (DateTime.Now - m_LastQueryTime_Game).TotalMilliseconds;
			if (timeSinceLastQueryGame > 300000)
			{
				await QueryGame("SELECT id FROM fuel_points LIMIT 1;").ConfigureAwait(true);
			}
		}

		public bool Initialize(bool bIsStartup = true)
		{
			// TODO_GITHUB: You need to set the below environment variables / variables to your GAME database connection info
			string[] gameSettings = new string[] {
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_IP") ?? "127.0.0.1",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_NAME") ?? "gtav",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_USERNAME") ?? "root",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_PASSWORD") ?? "example",
				SettingHelpers.GetDevEnvironmentSetting("GAME_DATABASE_PORT") ?? "3306"
				};

			// TODO_GITHUB: You need to set the below environment variables / variables to your AUTH database connection info
			string[] authSettings = new string[] {
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_IP") ?? "127.0.0.1",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_NAME") ?? "core",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_USERNAME") ?? "root",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_PASSWORD") ?? "example",
				SettingHelpers.GetDevEnvironmentSetting("AUTH_DATABASE_PORT") ?? "3307"
				};
			try
			{
				Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

				m_Connection_Game = new MySqlConnection(Helpers.FormatString(
					"Server={0}; database={1}; UID={2}; password={3}; port={4}; ConnectionTimeout=10", gameSettings));
				m_Connection_Game.Open();

				m_Connection_Auth = new MySqlConnection(Helpers.FormatString(
					"Server={0}; database={1}; UID={2}; password={3}; port={4}; ConnectionTimeout=10", authSettings));
				m_Connection_Auth.Open();

				PrintLogger.LogMessage(ELogSeverity.HIGH, "MySQL Initialized");

				return true;
			}
			catch (MySqlException ex)
			{
				switch (ex.Number)
				{
					// These are partially useless since MySQL tends to not throw these properly half the time
					case 0:
						PrintLogger.LogMessage(ELogSeverity.ERROR,
							"MySQL Connection Failed. Cannot Connect to Server.");
						break;
					case 1:
						PrintLogger.LogMessage(ELogSeverity.ERROR,
							"MySQL Connection Failed. Invalid username/password.");
						break;
					case 1042:
						PrintLogger.LogMessage(ELogSeverity.ERROR, "MySQL Connection Failed. Connection Timed Out.");
						break;
				}

				if (bIsStartup)
				{
					PrintLogger.LogMessage(ELogSeverity.ERROR, "\tPress any key to exit");
					Console.Read();
					Environment.Exit(1);
				}

				return false;
			}
			catch (InvalidOperationException)
			{
				PrintLogger.LogMessage(ELogSeverity.ERROR, "MySQL Connection Failed. Potentially Malformed Connection String.");
				PrintLogger.LogMessage(ELogSeverity.ERROR, "\tPress any key to exit");
				Console.Read();
				Environment.Exit(1);

				return false;
			}
		}

		private string EscapeAllAndFormatQuery(string strQuery, params object[] formatParams)
		{
			m_ThreadChecker.IsOnMainThread_LogIfNot();
			// Escape everything
			for (int i = 0; i < formatParams.Length; ++i)
			{
				if (formatParams[i].GetType() == typeof(string))
				{
					formatParams[i] = MySqlHelper.EscapeString((string)formatParams[i]);
				}
				else if (formatParams[i].GetType().IsEnum)
				{
					formatParams[i] = (int)formatParams[i];
				}
			}

			return Helpers.FormatString(strQuery, formatParams);
		}

		public async Task<CMySQLResult> QueryGame(string strQuery, params object[] formatParams)
		{
			m_ThreadChecker.IsOnMainThread_LogIfNot();
			m_LastQueryTime_Game = DateTime.Now;

			strQuery = EscapeAllAndFormatQuery(strQuery, formatParams);
			PrintLogger.LogMessage(ELogSeverity.DATABASE_DEBUG, "MYSQL QUERY: {0}", strQuery);

			CMySQLResult result = null;

			try
			{
				g_MutexGame.WaitOne();
				MySqlCommand cmd = m_Connection_Game.CreateCommand();
				cmd.CommandText = strQuery;

				// TODO_POST_LAUNCH: Better way of doing this than string compare?
				if (strQuery.StartsWith("DELETE") || strQuery.StartsWith("UPDATE"))
				{
					int numRowsModified = await cmd.ExecuteNonQueryAsync().ConfigureAwait(true);
					result = new CMySQLResult(numRowsModified);
				}
				else
				{
					System.Data.Common.DbDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(true);
					result = new CMySQLResult(reader, (ulong)cmd.LastInsertedId);
				}
			}
			catch (System.InvalidOperationException)
			{
				// TODO_MYSQL: Is it safe to assume that the entire operation failed? e.g. we don't insert twice

				// Try to reconnect and reissue
				PrintLogger.LogMessage(ELogSeverity.HIGH, "MySQL is attempting to reconnect");
				Initialize(false);
				return await QueryGame(strQuery, formatParams).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				// TODO: Store this somewhere important, it's a pretty critical error
				PrintLogger.LogMessage(ELogSeverity.ERROR, "MySQL Query Error [GAME]: {0}", e.InnerException == null ? e.Message : e.InnerException.ToString());

				// If we have a debugger, re-throw
				if (System.Diagnostics.Debugger.IsAttached)
				{
					throw;
				}
			}
			finally
			{
				g_MutexGame.ReleaseMutex();
			}

			return result;
		}

		public async Task<CMySQLResult> QueryAuth(string strQuery, params object[] formatParams)
		{
			m_ThreadChecker.IsOnMainThread_LogIfNot();
			m_LastQueryTime_Auth = DateTime.Now;
			strQuery = EscapeAllAndFormatQuery(strQuery, formatParams);

			CMySQLResult result = null;
			List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

			PrintLogger.LogMessage(ELogSeverity.DATABASE_DEBUG, "MYSQL QUERY: {0}", strQuery);

			try
			{
				g_MutexAuth.WaitOne();
				MySqlCommand cmd = m_Connection_Auth.CreateCommand();
				cmd.CommandText = strQuery;

				// TODO_POST_LAUNCH: Better way of doing this than string compare?
				if (strQuery.StartsWith("DELETE") || strQuery.StartsWith("UPDATE"))
				{
					int numRowsModified = await cmd.ExecuteNonQueryAsync().ConfigureAwait(true);
					result = new CMySQLResult(numRowsModified);
				}
				else
				{
					System.Data.Common.DbDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(true);
					result = new CMySQLResult(reader, (ulong)cmd.LastInsertedId);
				}

			}
			catch (System.InvalidOperationException)
			{
				// TODO_MYSQL: Is it safe to assume that the entire operation failed? e.g. we don't insert twice

				// Try to reconnect and reissue
				PrintLogger.LogMessage(ELogSeverity.HIGH, "MySQL is attempting to reconnect");
				Initialize(false);
				return await QueryAuth(strQuery, formatParams).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				// TODO: Store this somewhere important, it's a pretty critical error
				PrintLogger.LogMessage(ELogSeverity.DATABASE_DEBUG, "MySQL Query Error [AUTH]: {0}", e.InnerException == null ? e.Message : e.InnerException.ToString());

#if DEBUG
				if (!System.Diagnostics.Debugger.IsAttached)
				{
					Console.Read();
				}
#endif

				// If we have a debugger, re-throw
				if (System.Diagnostics.Debugger.IsAttached)
				{
					throw;
				}
			}
			finally
			{
				g_MutexAuth.ReleaseMutex();
			}

			return result;
		}
	}

	public class CMySQLResult
	{
		public CMySQLResult(int rowsAffected)
		{
			m_RowsAffected = rowsAffected;
		}

		public CMySQLResult(System.Data.Common.DbDataReader dbReader, ulong InsertID)
		{
			while (dbReader.Read())
			{
				CMySQLRow thisRow = new CMySQLRow();
				for (int i = 0; i < dbReader.FieldCount; i++)
				{
					string value = "";
					if (!dbReader.IsDBNull(i))
					{
						value = dbReader.GetString(i);
					}
					string fieldName = dbReader.GetName(i);

					thisRow[fieldName] = value;
				}

				m_Rows.Add(thisRow);
			}

			dbReader.Close();

			m_InsertID = InsertID;
			m_RowsAffected = 0;
		}

		public List<CMySQLRow> GetRows()
		{
			return m_Rows;
		}

		public CMySQLRow GetRow(int a_Index)
		{
			return m_Rows[a_Index];
		}

		public int NumRows()
		{
			return m_Rows.Count;
		}

		public ulong GetInsertID()
		{
			return m_InsertID;
		}

		public int GetNumRowsAffected()
		{
			return m_RowsAffected;
		}

		private List<CMySQLRow> m_Rows = new List<CMySQLRow>();
		private readonly ulong m_InsertID = 0;
		private readonly int m_RowsAffected = 0;
	}
}
