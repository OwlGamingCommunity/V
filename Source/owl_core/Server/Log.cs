using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logging
{
	public static class Logger
	{
		public static SemaphoreSlim g_Semaphore = new SemaphoreSlim(1);
		private static List<string> g_lstLogLinesCached = new List<string>();
		public static StreamWriter logFile = File.CreateText(Path.Combine("logs/", DateTime.Now.ToString("s").Replace(":", "-") + ".log")); // S for sortable datetime format
																																			// Logfile will be in the Output/Release/logs/file.log

		public static async Task CheckFlush()
		{
			if (g_lstLogLinesCached.Count > 10)
			{
				await g_Semaphore.WaitAsync().ConfigureAwait(true);

				foreach (string strLogLine in g_lstLogLinesCached)
				{
					await logFile.WriteLineAsync(strLogLine).ConfigureAwait(true);
				}

				// Write now since there are more than 10 log lines waiting
				await logFile.FlushAsync().ConfigureAwait(true);

				// Reset cache
				g_lstLogLinesCached.Clear();

				// Release lock
				g_Semaphore.Release();
			}
		}

		public static async Task Add(string strJson)
		{
			await g_Semaphore.WaitAsync().ConfigureAwait(true);
			g_lstLogLinesCached.Add(strJson);
			g_Semaphore.Release();
		}

		public static async Task TimedFlush()
		{
			await g_Semaphore.WaitAsync().ConfigureAwait(true);
			await logFile.FlushAsync().ConfigureAwait(true);
			g_Semaphore.Release();
		}

		public static async Task Shutdown()
		{
			await g_Semaphore.WaitAsync().ConfigureAwait(true);
			logFile.Close();
			g_Semaphore.Release();
		}
	}

	public class Log
	{
		/// <summary>
		/// You should use this constructor if you plan to add affected elements in a loop, then you can call: `myLog.addAffected(element);` after
		/// determining if they should be included in the log. 
		/// 
		/// REMEMBER: You must set the context. `myLog.content = 'something';`
		/// </summary>
		/// <param name="source"></param>
		/// <param name="action"></param>
		public Log(CPlayer source, ELogType action)
		{
			origin = generateOrigin(source);
			this.action = action;
		}

		/// <summary>
		/// In most cases this constructor is used. Run `myLog.execute()` to commit it to disk. 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="action"></param>
		/// <param name="affected"></param>
		/// <param name="context"></param>
		public Log(CPlayer source, ELogType action, List<CBaseEntity> affected, string context)
		{
			origin = generateOrigin(source);
			this.action = action;
			content = context;

			if (!generateAffected(affected))
			{
				throw new ArgumentException("Invalid Affected Elments.");
			}
		}

		/// <summary>
		/// This should be used as a last resort. It will not be able to associate accounts with character sources
		/// if you handle it this way. ONLY USE FOR "SYSTEM" origins.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="originType"></param>
		/// <param name="action"></param>
		/// <param name="affected"></param>
		/// <param name="context"></param>
		/// <exception cref="ArgumentException"></exception>
		public Log(long source, EOriginType originType, ELogType action, List<CBaseEntity> affected, string context)
		{
			origin = generateOrigin(source, originType);
			this.action = action;
			content = context;

			if (!generateAffected(affected))
			{
				throw new ArgumentException("Invalid Affected Elments.");
			}
		}

		/// <summary>
		/// A static function that immediately executes the log you create to ES.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="action"></param>
		/// <param name="affected"></param>
		/// <param name="context"></param>
		public static void CreateLog(CPlayer source, ELogType action, List<CBaseEntity> affected, string context)
		{
			new Log(source, action, affected, context).execute();
		}

		/// <summary>
		/// DO NOT USE UNLESS REALLY NEEDED. Will not be able to add effected accounts for this source type.
		/// Send in a CPLAYER instead if you can. A static function that immediately executes the log you create to ES.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="action"></param>
		/// <param name="affected"></param>
		/// <param name="context"></param>
		public static void CreateLog(long source, EOriginType originType, ELogType action, List<CBaseEntity> affected, string context)
		{
			new Log(source, originType, action, affected, context).execute();
		}

		public bool addAffected(CBaseEntity entity)
		{
			if (entity is CPlayer temp)
			{
				if (temp.IsLoggedIn)
				{
					characters.Add(temp.ActiveCharacterDatabaseID);
				}
				else
				{
					accounts.Add(temp.m_DatabaseID);
				}
			}
			else if (entity is CVehicle)
			{
				vehicles.Add(entity.m_DatabaseID);
			}
			else if (entity is CPropertyInstance)
			{
				properties.Add(entity.m_DatabaseID);
			}
			else if (entity is CFaction)
			{
				factions.Add(entity.m_DatabaseID);
			}
			else if (entity is CWorldItem)
			{
				objects.Add(entity.m_DatabaseID);
			}
			else
			{
				// Unknown type
#if DEBUG
				PrintLogger.LogMessage(ELogSeverity.ERROR, "Unknown Log Type sent as an affected element - {0}", entity.ToString());
#endif
				return false;
			}
			// Inventory items?
			// Phones?
			return true;
		}

		public void addOfflineAffectedAccount(long AccountDatabaseID)
		{
			accounts.Add(AccountDatabaseID);
		}

		private long generateOrigin(long origin, EOriginType type)
		{
			origin_type = type;
			if (type == EOriginType.Account)
			{
				origin_account = origin;
				return origin;
			}

			if (type == EOriginType.Character)
			{
				return origin;
			}

			throw new ArgumentException("Invalid event origin defined");
		}

		private long generateOrigin(CPlayer origin)
		{
			// Also record the account in affected for searching
			origin_account = origin.AccountID;

			if (origin.IsLoggedIn && origin.IsSpawned)
			{
				origin_type = EOriginType.Character;
				location.x = origin.Client.Position.X;
				location.y = origin.Client.Position.Y;
				location.z = origin.Client.Position.Z;
				location.dimension = origin.Client.Dimension;

				return origin.ActiveCharacterDatabaseID;
			}

			origin_type = EOriginType.Account;
			return origin.AccountID;
		}

		private bool generateAffected(List<CBaseEntity> elements)
		{
			if (elements != null)
			{
				foreach (CBaseEntity entity in elements)
				{
					bool stat = addAffected(entity);
					if (!stat)
					{
						return false;
					}
				}
			}
			return true;
		}

		public async void execute()
		{
			try
			{
				if (origin == 0)
				{
					throw new ArgumentException("Unknown Source.");
				}
				if (content.Length == 0)
				{
					throw new ArgumentException("You must pass context.");
				}

				date = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds();
				string json = JsonConvert.SerializeObject(this);

				await Logger.Add(json).ConfigureAwait(true);

				await Logger.CheckFlush().ConfigureAwait(true);
			}
			catch (Exception ex)
			{
				SentryEvent logEvent = new SentryEvent(ex);
				SentrySdk.CaptureEvent(logEvent);
			}
		}

		public bool ShouldSerializelocation()
		{
			// Don't serialize if the location is nothing.
			return location.x != 0 && location.y != 0 && location.z != 0;
		}

		public ulong date { get; set; } // Milliseconds since epoch
		public ELogType action { get; set; }
		public long origin { get; set; }
		public string content { get; set; }

		// Field not used warning disabled
		// Have to opt in private members to be used as properties
#pragma warning disable 0414
		[JsonProperty]
		private EOriginType origin_type;
#pragma warning restore 0414
		[JsonProperty]
		private long origin_account;
		[JsonProperty]
		public List<long> characters { get; private set; } = new List<long>();
		[JsonProperty]
		public List<long> accounts { get; private set; } = new List<long>();
		[JsonProperty]
		public List<long> objects { get; private set; } = new List<long>();
		[JsonProperty]
		public List<long> vehicles { get; private set; } = new List<long>();
		[JsonProperty]
		public List<long> properties { get; private set; } = new List<long>();
		// TODO_CHAOS: Add this
		//[JsonProperty]
		//private List<Int64> phones = new List<Int64>();
		[JsonProperty]
		public List<long> factions { get; private set; } = new List<long>();
		[JsonProperty]
		private Location location;
	}

	public class LogCleanup
	{
		private async void timerEvent(object[] a_Parameters)
		{
			await Logger.TimedFlush().ConfigureAwait(true);
		}

		~LogCleanup()
		{
			Logger.Shutdown().ConfigureAwait(true);
		}

		public LogCleanup()
		{
			m_Timer = MainThreadTimerPool.CreateGlobalTimer(timerEvent, 5000);

			int daysAgo = 7;
			int maxToDelete = 10;
			// Create the directory if it doesn't exist
			Directory.CreateDirectory("logs/");
			string[] files = Directory.GetFiles("logs/", "*.log", SearchOption.TopDirectoryOnly);
			if (files.Length > 0)
			{
				string[] filesToDelete = files.Where(c =>
				{
					TimeSpan ts = DateTime.Now - File.GetLastAccessTime(c);
					return (ts.Days > daysAgo);
				}).ToArray();
				for (int i = 0; i < Math.Min(filesToDelete.Length, maxToDelete); i++)
				{
					File.Delete(filesToDelete[i]);
				}
			}
		}

		private WeakReference<MainThreadTimer> m_Timer = new WeakReference<MainThreadTimer>(null);
	}

	public struct Location : IEquatable<Location>
	{
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }
		public uint dimension { get; set; }

		public bool Equals(Location rhs)
		{
			return Equals(rhs);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Location))
			{
				return false;
			}

			Location rhsStruct = (Location)obj;
			return rhsStruct.x == x && rhsStruct.y == y && rhsStruct.z == z && rhsStruct.dimension == dimension;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = 0;
				result = (result * 301) ^ Convert.ToInt32(x);
				result = (result * 301) ^ Convert.ToInt32(y);
				result = (result * 301) ^ Convert.ToInt32(z);
				result = (result * 301) ^ Convert.ToInt32(dimension);

				return result;
			}
		}

		public static bool operator ==(Location lhs, Location rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Location lhs, Location rhs)
		{
			return !lhs.Equals(rhs);
		}
	}
	public enum EOriginType
	{
		Account,
		Character
	}

	public enum ELogType
	{
		AdminChat,
		LeadAdminChat,
		AdminCommand,
		PlayerSay,
		PlayerOOC,
		PlayerShout,
		PlayerPM,
		PlayerMe,
		PlayerDo,
		PlayerWhisper,
		PlayerFactionChat,
		VehicleRelated,
		PropertyRelated,
		PhoneChat,
		CashTransfer,
		SMS,
		UCPRelated,
		Death,
		FactionAction,
		StatTransfer,
		PlayerAme,
		PlayerAdo,
		StaffChat,
		ItemMovement,
		ConnectionEvents,
		Email,
		WeaponLicense,
		GlobalOOC,
		ObjectSpawner,
		PlayerRadio,
		PlayerMoneyChange,
		InfoMarker,
		BlackjackRoundOutcome
	}
}
