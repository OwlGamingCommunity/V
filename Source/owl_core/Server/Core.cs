//#define DEBUG_USE_REAL_TIME
using GTANetworkAPI;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

public static class VectorExtensions
{
	public static bool IsNull(this GTANetworkAPI.Vector3 vecToCheck)
	{
		return vecToCheck == null || (vecToCheck.X == 0.0f && vecToCheck.Y == 0.0f && vecToCheck.Z == 0.0f);
	}
}

public static class VersionHelpers
{
	public static bool IsTestServer()
	{
#if DEBUG || RAGE_11_TEST_SERVER
		return true;
#else
		return false;
#endif
	}
}

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
public abstract class OwlScript
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
{
	private static OwlScript m_Singleton = null;

	public OwlScript()
	{
		m_Singleton = this;
	}

	public static T Get<T>()
	{
		return (T)Convert.ChangeType(m_Singleton, m_Singleton.GetType());
	}
}

public class Core : IDisposable
{
	private WeakReference<MainThreadTimer> g_SetWorldClockTimer = new WeakReference<MainThreadTimer>(null);
	private WeakReference<MainThreadTimer> g_SetWeatherTimer = new WeakReference<MainThreadTimer>(null);

	private BlipManager m_BlipManager = new BlipManager();
	private Christmas m_Christmas = new Christmas();
	private DiscordBotIntegration m_DiscordBotIntegration = new DiscordBotIntegration();
	private CustomEventManagerScript m_CustomEventManagerScript = new CustomEventManagerScript();
	private Halloween m_Halloween = new Halloween();
	private Logging.LogCleanup m_LogCleanup = new Logging.LogCleanup();
	private MainThreadTimerPool m_MainThreadTimerPool = new MainThreadTimerPool();
	private MapLoader m_MapLoader = new MapLoader();
	private PersistentNotifications m_PersistentNotifications = new PersistentNotifications();
	private PingManager m_PingManager = new PingManager();
	private RadialMenuActions m_RadialMenuActions = new RadialMenuActions();
	private ResourceManagement m_ResourceManagement = new ResourceManagement();
	private TrainManager m_TrainManager = new TrainManager();
	private WorldBlipSystem m_WorldBlipSystem = new WorldBlipSystem();

	private static bool g_bHasOverrideTime = false;
	private static bool g_bHasOverrideWeather = false;

	private string g_exceptionFilePath = String.Empty;

	private SemaphoreSlim g_SemaphoreExceptionFile = new SemaphoreSlim(1);

	public static void OnCustomEvent(Player sender, params object[] arguments) { EventManager.OnCustomEvent(sender, arguments); }
	public static void RAGE_OnUpdate() { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnUpdate, true); }
	public static void RAGE_OnChatMessage(Player client, System.String message) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnChatMessage, true, client, message); }
	public static void RAGE_OnEntityCreated(Entity entity) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityCreated, true, entity); }
	public static void RAGE_OnEntityDeleted(Entity entity) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityDeleted, true, entity); }
	public static void RAGE_OnEntityModelChange(Entity entity, System.UInt32 oldModel) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnEntityModelChange, true, entity, oldModel); }
	public static void RAGE_OnPlayerConnected(Player client) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerConnected, true, client); }
	public static void RAGE_OnPlayerDisconnected(Player client, DisconnectionType type, string reason) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerDisconnected, true, client, type, reason); }
	public static void RAGE_OnPlayerSpawn(Player client) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerSpawn, true, client); }
	public static void RAGE_OnPlayerDeath(Player client, Player killer, System.UInt32 reason) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerDeath, true, client, killer, reason); }
	public static void RAGE_OnPlayerDamage(Player client, float healthLoss, float armorLoss) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerDamage, true, client, healthLoss, armorLoss); }
	public static void RAGE_OnPlayerPickup(Player client, Pickup pickup) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerPickup, true, client, pickup); }
	public static void RAGE_OnPlayerWeaponSwitch(Player client, WeaponHash oldWeaponHash, GTANetworkAPI.WeaponHash newWeaponHash) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerWeaponSwitch, true, client, oldWeaponHash, newWeaponHash); }
	public static void RAGE_OnPlayerDetonateStickies(Player client) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerDetonateStickies, true, client); }
	public static void RAGE_OnPlayerEnterCheckpoint(Checkpoint checkpoint, Player client) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerEnterCheckpoint, true, checkpoint, client); }
	public static void RAGE_OnPlayerExitCheckpoint(Checkpoint checkpoint, Player client) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerExitCheckpoint, true, checkpoint, client); }
	public static void RAGE_OnPlayerEnterColshape(ColShape colShape, Player client) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerEnterColshape, true, colShape, client); }
	public static void RAGE_OnPlayerExitColshape(ColShape colShape, Player client) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerExitColshape, true, colShape, client); }
	public static void RAGE_OnPlayerEnterVehicleAttempt(Player client, Vehicle vehicle, sbyte seatId) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerEnterVehicleAttempt, true, client, vehicle, seatId); }
	public static void RAGE_OnPlayerExitVehicleAttempt(Player client, Vehicle vehicle) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerExitVehicleAttempt, true, client, vehicle); }
	public static void RAGE_OnPlayerEnterVehicle(Player client, Vehicle vehicle, sbyte seatId) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerEnterVehicle, true, client, vehicle, seatId); }
	public static void RAGE_OnPlayerExitVehicle(Player client, Vehicle vehicle) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnPlayerExitVehicle, true, client, vehicle); }
	public static void RAGE_OnVehicleDamage(Vehicle vehicle, float bodyHealthLoss, float engineHealthLoss) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleDamage, true, vehicle, bodyHealthLoss, engineHealthLoss); }
	public static void RAGE_OnVehicleDeath(Vehicle vehicle) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleDeath, true, vehicle); }
	public static void RAGE_OnVehicleHornToggle(Vehicle vehicle, bool bToggle) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleHornToggle, true, vehicle, bToggle); }
	public static void RAGE_OnVehicleSirenToggle(Vehicle vehicle) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleSirenToggle, true, vehicle); }
	public static void RAGE_OnVehicleDoorBreak(Vehicle vehicle, int index) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleDoorBreak, true, vehicle, index); }
	public static void RAGE_OnVehicleWindowSmash(Vehicle vehicle, int index) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleWindowSmash, true, vehicle, index); }
	public static void RAGE_OnVehicleTyreBurst(Vehicle vehicle, int index) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleTyreBurst, true, vehicle, index); }
	public static void RAGE_OnVehicleTrailerChange(Vehicle vehicle, Vehicle trailer) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnVehicleTrailerChange, true, vehicle, trailer); }
	public static void RAGE_OnFirstChanceException(Exception exception) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnFirstChanceException, true, exception); }
	public static void RAGE_OnUnhandledException(Exception exception) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnUnhandledException, true, exception); }

	public Core()
	{
		// Create directory
		if (!Directory.Exists("exceptions"))
		{
			Directory.CreateDirectory("exceptions");
		}

		// Create a file
#if DEBUG
		g_exceptionFilePath = Path.Combine("exceptions", "exceptions_debug.log");
#else
		g_exceptionFilePath = Path.Combine("exceptions", Helpers.FormatString("exceptions_{0}.log", System.Diagnostics.Process.GetCurrentProcess().StartTime.ToString("s").Replace(":", "-")));
#endif
		if (!File.Exists(g_exceptionFilePath))
		{
			File.CreateText(g_exceptionFilePath);
		}

		// Limited to 2 seconds to execute because rage is some hacky .NET Framework
		AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
		{
			Console.WriteLine("Ungraceful shutdown detected. Only owl_boot can gracefully shutdown the server.");
		};

		AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
		{
			HandleServerException(eventArgs.Exception);
		};

		AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
		{
			HandleServerException((Exception)eventArgs.ExceptionObject);
		};

		NetworkEvents.ClientsideException += OnClientsideException;

		Initialize();
	}

	public void LoadAllInformationMarkers()
	{
		InformationMarkerPool.Initialize();

		Database.Functions.Items.LoadAllInfoMarkers((List<CDatabaseStructureInformationMarker> lstInformationMarkers) =>
		{
			foreach (var infoMarker in lstInformationMarkers)
			{
				InformationMarkerPool.CreateInformationMarker(infoMarker.ID, infoMarker.OwnerCharacterID, infoMarker.Position, infoMarker.Dim, infoMarker.strText, false);
			}

			NAPI.Util.ConsoleOutput("[INFOMARKERS] Loaded {0} Info Markers!", lstInformationMarkers.Count);
		});
	}

	private async void HandleServerException(Exception ex)
	{
		// try catch to avoid stack overflow
		try
		{
			bool bWriteToSentry = true;
			bool bWriteToDisk = true;

			if (ex.Message.ToLower().Contains("expando"))
			{
				bWriteToSentry = false;
				bWriteToDisk = false;
			}

			if (ex.Source == "Discord.Net.Core")
			{
				bWriteToSentry = false;
				bWriteToDisk = true;
			}

			// http 404s etc always throw :(
			if (ex.Source.StartsWith("System.Net.Http"))
			{
				bWriteToSentry = false;
				bWriteToDisk = true;
			}

			// http 404s etc always throw :(
			if (ex.Message.Contains("A connection attempt failed") || ex.Message.Contains("No such host is known") || ex.Message.Contains("The remote certificate is invalid according to the validation procedure"))
			{
				bWriteToSentry = false;
				bWriteToDisk = true;
			}

#if !SCRIPT_FEATURE_SENTRY
			bWriteToSentry = false;
#endif

			if (!bWriteToSentry && !bWriteToDisk) // Nothing to do
			{
				return;
			}

			// shared data
			List<string> strExceptionDetails = new List<string>();
			strExceptionDetails.Add("============== EXCEPTION ==============");
			strExceptionDetails.Add(Helpers.FormatString("TIME: {0}", DateTime.Now.ToString("s")));
			strExceptionDetails.Add(Helpers.FormatString("MOST RECENT COMMAND: {0}", CommandManager.g_strLastCommand));

			Database.ThreadedMySQL.Debug_GetRecentQueries(out Queue<String> recentQueries, out Dictionary<int, string> activeQueries);
			int numQueries = recentQueries.Count;
			int index = 0;
			foreach (string strRecentQuery in recentQueries)
			{
				strExceptionDetails.Add(Helpers.FormatString("MOST RECENT QUERY #{0}: {1}", numQueries - index, strRecentQuery));
				++index;
			}

			strExceptionDetails.Add(Helpers.FormatString("CURRENT THREAD ID: {0}", Thread.CurrentThread.ManagedThreadId));
			foreach (var kvPair in activeQueries)
			{
				strExceptionDetails.Add(Helpers.FormatString("ACTIVE QUERY Thread {0}: {1}", kvPair.Key, kvPair.Value));
			}

			// was it a sql error?
			foreach (var kvPair in activeQueries)
			{
				if (kvPair.Key == Thread.CurrentThread.ManagedThreadId)
				{
					strExceptionDetails.Add(Helpers.FormatString("ERROR IS WITH THE FOLLOWING QUERY: {0}", kvPair.Value));
				}
			}

			strExceptionDetails.Add(Helpers.FormatString("INNEREXCEPTION: {0}", ex.InnerException != null ? ex.InnerException.Message : "None"));
			strExceptionDetails.Add(Helpers.FormatString("MESSAGE: {0}", ex.Message));
			strExceptionDetails.Add(Helpers.FormatString("SOURCE: {0}", ex.Source));
			strExceptionDetails.Add(Helpers.FormatString("EXSTACK: {0}", ex.StackTrace));
			strExceptionDetails.Add(Helpers.FormatString("ENVSTACK: {0}", Environment.StackTrace));
			strExceptionDetails.Add(Helpers.FormatString("=========================================="));

			// sentry
			if (bWriteToSentry)
			{
				Sentry.SentrySdk.AddBreadcrumb("Attempting to record exception", "owl_core", level: Sentry.Protocol.BreadcrumbLevel.Info);
				Sentry.SentryEvent logEvent = new Sentry.SentryEvent(ex);
				logEvent.SetTag("rage11", "true");
				logEvent.Message = String.Join("\n", strExceptionDetails);
				Sentry.SentrySdk.CaptureEvent(logEvent);

				//Sentry.SentrySdk.CaptureException(ex);
			}


			if (bWriteToDisk)
			{
				await g_SemaphoreExceptionFile.WaitAsync().ConfigureAwait(false);
				await File.AppendAllLinesAsync(g_exceptionFilePath, strExceptionDetails).ConfigureAwait(false);
				g_SemaphoreExceptionFile.Release();
			}

			Console.ForegroundColor = ConsoleColor.Red;
			foreach (string strDetail in strExceptionDetails)
			{
				NAPI.Util.ConsoleOutput(strDetail);
			}
			Console.ForegroundColor = ConsoleColor.Gray;
		}
		catch (Exception overflow)
		{
			try
			{
				Sentry.SentrySdk.CaptureException(overflow);
			}
			catch
			{

			}
		}
	}

	private Dictionary<CPlayer, List<ClientsideExceptionCooldown>> m_dictClientsideExceptionCooldowns = new Dictionary<CPlayer, List<ClientsideExceptionCooldown>>();
	class ClientsideExceptionCooldown
	{
		public ClientsideExceptionCooldown(string strMessage)
		{
			timeStarted = DateTime.Now;
			Message = strMessage;
		}

		public string Message { get; }

		public bool Expired()
		{
			return (MainThreadTimerPool.GetMillisecondsSinceDateTime(timeStarted)) >= ExceptionConstants.CooldownPeriod;
		}

		DateTime timeStarted = DateTime.Now;
	}

	private bool IsClientsideExceptionOnCooldown(CPlayer player, string strMessage)
	{
		// Remove any expired
		foreach (var kvPair in m_dictClientsideExceptionCooldowns)
		{
			List<ClientsideExceptionCooldown> lstToRemove = new List<ClientsideExceptionCooldown>();
			foreach (var cooldownInst in kvPair.Value)
			{
				if (cooldownInst.Expired())
				{
					lstToRemove.Add(cooldownInst);
				}
			}

			foreach (var cooldownToRemove in lstToRemove)
			{
				kvPair.Value.Remove(cooldownToRemove);
			}
		}

		bool bCooldown = false;
		if (m_dictClientsideExceptionCooldowns.ContainsKey(player))
		{
			var dictForPlayer = m_dictClientsideExceptionCooldowns[player];
			foreach (var cooldownInst in dictForPlayer)
			{
				if (cooldownInst.Message == strMessage)
				{
					bCooldown = true;
				}
			}
		}

		return bCooldown;
	}

	private void AddClientsideExceptionCooldown(CPlayer player, string strMessage)
	{
		if (!m_dictClientsideExceptionCooldowns.ContainsKey(player))
		{
			m_dictClientsideExceptionCooldowns.Add(player, new List<ClientsideExceptionCooldown>());
		}

		m_dictClientsideExceptionCooldowns[player].Add(new ClientsideExceptionCooldown(strMessage));
	}

	/// <summary>
	/// Parses a stack trace for fingerprinting based on the Class name, method name and typed params.
	/// 
	/// For example:
	/// at CharacterSelection.OnCharacterSelectionApproved() in C:\RAGEMP\client_resources\127.0.0.1_5000\cs_packages\owl_account_system.client\CharacterSelection.Client.cs:line 51
	/// 
	/// Is parsed to MethodName as CharacterSelection.OnCharacterSelectionApproved()
	///
	/// Using the StackTrace object was attempted but it does not include line number information
	/// to make it easier to determine what packages are ours.
	/// </summary>
	/// <param name="ex"></param>
	private void OnClientsideException(CPlayer player, ClientsideException exceptionObject)
	{
		try
		{
			string[] strCallstackSplit = exceptionObject.CallStack.Split(" at ");

#if DEBUG
			Console.WriteLine("============== CLIENTSIDE EXCEPTION ==============");
			Console.WriteLine("GOT CLIENTSIDE EXCEPTION FROM: {0}", player.Username);
			Console.WriteLine(exceptionObject.Message);

			foreach (string strLine in strCallstackSplit)
			{
				Console.WriteLine("\t{0}", strLine);
			}
			Console.WriteLine("==========================================");
#endif

			if (!IsClientsideExceptionOnCooldown(player, exceptionObject.Message))
			{
				AddClientsideExceptionCooldown(player, exceptionObject.Message);

				// Push a new scope to prevent any future client side exceptions from having invalid breadcrumbs
				Sentry.SentrySdk.WithScope(scope =>
				{
					var usr = new Sentry.Protocol.User();
					usr.Username = player.Username;
					usr.IpAddress = player.Client.Address;

					foreach (string strLine in strCallstackSplit)
					{
						Sentry.SentrySdk.AddBreadcrumb(strLine);
					}

					Sentry.SentryEvent logEvent = new Sentry.SentryEvent();

					// Fingerprint by the exception message as well so different exceptions from the same method aren't grouped
					Match e = Regex.Match(exceptionObject.CallStack, ".*:line .*", RegexOptions.Multiline);
					string methodName = "";
					if (e.Success)
					{
						Match y = Regex.Match(e.Value, "at (.*?) in ");
						methodName = y.Groups[1].Value;
					}
					string[] messageLines = exceptionObject.Message.Split(
						new[] { Environment.NewLine },
						StringSplitOptions.None
					);

					logEvent.Message = Helpers.FormatString("{0}\r\n{1}", exceptionObject.Message, exceptionObject.CallStack);
					logEvent.User = usr;
					logEvent.ServerName = "RAGE Windows Client";
					Sentry.BaseScopeExtensions.SetTag(scope, "clientside", "true");
					Sentry.BaseScopeExtensions.SetTag(scope, "rage11", "true");
					if (player.IsSpawned)
					{
						Sentry.BaseScopeExtensions.SetExtra(scope, "character_name", player.GetCharacterName(ENameType.StaticCharacterName));
						Sentry.BaseScopeExtensions.SetExtra(scope, "character_id", player.ActiveCharacterDatabaseID);
					}
					Sentry.BaseScopeExtensions.SetExtra(scope, "account_id", player.AccountID.ToString());
					Sentry.BaseScopeExtensions.SetFingerprint(scope, new[] { "{{ default }}", methodName, messageLines[0] });

					Sentry.SentrySdk.CaptureEvent(logEvent);
				});
			}
		}
		catch
		{

		}
	}

	public static void RecordSlowdownToSentry(string strSentryTitle, double timeTaken, string strStackTrace)
	{
		Sentry.SentrySdk.WithScope(scope =>
		{
			var usr = new Sentry.Protocol.User();
			usr.Username = "Server";

			string[] strCallstackSplit = strStackTrace.Split(" at ");
			foreach (string strLine in strCallstackSplit)
			{
				Sentry.SentrySdk.AddBreadcrumb(strLine);
			}

			Sentry.SentryEvent logEvent = new Sentry.SentryEvent();

			logEvent.Message = Helpers.FormatString("{0}\r\n{1}ms\r\n{2}", strSentryTitle, Math.Round(timeTaken, 2), strStackTrace);
			logEvent.User = usr;
			logEvent.ServerName = "Server";
			Sentry.BaseScopeExtensions.SetTag(scope, "serverperf", "true");
			Sentry.BaseScopeExtensions.SetTag(scope, "rage11", "true");
			Sentry.BaseScopeExtensions.SetFingerprint(scope, new[] { "{{ default }}", strSentryTitle });

			Sentry.SentrySdk.CaptureEvent(logEvent);
		});
	}

	/// Used for setting global server stuff
	public void Initialize()
	{
		LoadAllInformationMarkers();

		// By default, outputs the command doesn't exist. Instead just don't show anything if it doesn't exist.
		// RAGE BUG, Still prints out a black dot if you do null or "".
		// NAPI.Server.SetCommandErrorMessage(null);

		// Init Sentry for use across any projects

		// TODO_GITHUB: You should set the environment variables below to your versioning data
#if DEBUG
		string version = Environment.GetEnvironmentVariable("GIT_LATEST_TAG") ?? "DEVBUILD";
		string hash = Environment.GetEnvironmentVariable("GIT_COMMIT_SHA") ?? "";
#else
		string version = Environment.GetEnvironmentVariable("GIT_LATEST_TAG") ?? "EXTENDED_BETA";
		string hash = Environment.GetEnvironmentVariable("GIT_COMMIT_SHA") ?? "0bba702d";
#endif
		if (version[0] == 'v')
		{
			// Strip the version "v" "v1.0"
			version = version.Substring(1);
		}

		// Refers to SENTRY_DSN environment variable
		Sentry.SentrySdk.Init(o =>
		{
			o.AttachStacktrace = true;
			o.SendDefaultPii = true;
			o.Release = "v@" + version;
			o.Environment = "Beta";
		});

		// TODO_GITHUB: You should set the environment variable below
		Console.WriteLine("[SENTRY] {0}", string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SENTRY_DSN")) ? "Disabled." : "Enabled.");

		NetworkEvents.RequestPlayerSpecificDimension += (CPlayer a_Player) => { a_Player.GotoPlayerSpecificDimension(); };
		NetworkEvents.RequestPlayerNonSpecificDimension += (CPlayer a_Player) => { a_Player.GotoNonPlayerSpecificDimension(); };
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
		NetworkEvents.StoreAmmo += StoreAmmo;
		NetworkEvents.StoreWeapons += StoreWeapons;
		g_SetWorldClockTimer = MainThreadTimerPool.CreateGlobalTimer(SetWorldClockTimer, 60000);

		g_SetWeatherTimer = MainThreadTimerPool.CreateGlobalTimer(CalculateWeather, 2700000);

		SetWorldClockTimer(null);

		CalculateWeather(null);
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		if (a_CleanupNativeAndManaged)
		{

			// Attempt to push any remaining events
			Sentry.SentrySdk.Close();
		}
	}

	public void OnPlayerConnected(CPlayer a_pPlayer)
	{
		DateTime ServerClock = GetServerClock();
		NetworkEventSender.SendNetworkEvent_InitialJoinEvent(a_pPlayer, ServerClock.Day, ServerClock.Month, ServerClock.Year, ServerClock.Hour, ServerClock.Minute, ServerClock.Second, (int)HelperFunctions.World.GetCurrentWeather(),
#if DEBUG
			true
#else
			false
#endif
			);
	}

	public static void SetOverrideTime(bool bOverride, int hour)
	{
		g_bHasOverrideTime = bOverride;

		if (bOverride)
		{
			NAPI.World.SetTime(hour, DateTime.Now.Minute, DateTime.Now.Second);
		}
		else
		{
			SetWorldClockTimer();
		}
	}

	public static void SetOverrideWeather(bool bOverride, int weatherID)
	{
		g_bHasOverrideWeather = bOverride;

		if (bOverride)
		{
			WeatherOverride = (GTANetworkAPI.Weather)weatherID;
			NetworkEventSender.SendNetworkEvent_UpdateWeatherState_ForAll_IncludeEveryone(weatherID);
		}
		else
		{
			CalculateWeather();
		}
	}

	public static DateTime GetServerClock()
	{
#if DEBUG && !DEBUG_USE_REAL_TIME
		return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 0, 0);
#else
		return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("UTC"));
#endif
	}

	private static void SetWorldClockTimer(object[] a_Parameters = null)
	{
		if (g_bHasOverrideTime)
		{
			return;
		}

		DateTime ServerClock = GetServerClock();

#if DEBUG && !DEBUG_USE_REAL_TIME
		NAPI.World.SetTime(12, 00, 00);
#else
		NAPI.World.SetTime(ServerClock.Hour, ServerClock.Minute, ServerClock.Second);
#endif
	}

	private static void CalculateWeather(object[] a_Parameters = null)
	{
		int weather = g_bHasOverrideWeather ? (int)WeatherOverride : (int)WeatherService.GetCalculatedWeather();
		NetworkEventSender.SendNetworkEvent_UpdateWeatherState_ForAll_IncludeEveryone(weather);
	}

	public static GTANetworkAPI.Weather WeatherOverride { get; set; } = Weather.CLEAR;
	public static bool IsWeatherOverriden => g_bHasOverrideWeather;

	public void StoreAmmo(CPlayer a_Player, Dictionary<EWeapons, int> ammoDiff)
	{
		a_Player.CopyAmmoData(ammoDiff);

		a_Player.Inventory.SyncInventoryAmmoWithWeaponAmmoAndSave(); // must come first to update based on ammoDiff
		a_Player.Inventory.SynchronizeAllWeaponsAndAmmoWithInventory();
	}

	public void StoreWeapons(CPlayer a_Player, List<WeaponHash> lstWeaponsFull)
	{
		// NOTE: This function IS client authorative. It is used simply for removing weapons the player has clientside/ in GTA, but not in the inventory.
		//		 The inventory is STILL the source of truth for which weapons the player has, and you should NEVER use this function to do anything that grants items/weapons etc.
		a_Player.CopyWeaponDataClientside(lstWeaponsFull);

		a_Player.Inventory.SyncInventoryAmmoWithWeaponAmmoAndSave(); // must come first to update based on ammoDiff
		a_Player.Inventory.SynchronizeAllWeaponsAndAmmoWithInventory();
	}
}
