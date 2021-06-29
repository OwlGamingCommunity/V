using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public static class CodeAnalysis
{
	private static Mutex g_CodeAnalysisMutex = new Mutex();
	private static Dictionary<string, int> g_dictTodos = new Dictionary<string, int>();

	private static int NumTodos = 0;
	private static int NumLines = 0;
	private static int NumFiles = 0;
	private static int NumJS = 0;
	private static int NumExported = 0;
	private static int NumLegacyAPICalls = 0;
	private static int NumLegacySQLCalls = 0;
	private static Dictionary<string, int> g_dictLegacySQLCounts = new Dictionary<string, int>();

	public static void AddTodo(string msg)
	{
		g_CodeAnalysisMutex.WaitOne();
		if (CodeAnalysis.g_dictTodos.ContainsKey(msg))
		{
			g_dictTodos[msg]++;
		}
		else
		{
			g_dictTodos[msg] = 1;
		}

		++NumTodos;

		g_CodeAnalysisMutex.ReleaseMutex();
	}

	public static void AddLines(int a_num) { g_CodeAnalysisMutex.WaitOne(); NumLines += a_num; g_CodeAnalysisMutex.ReleaseMutex(); }
	public static void AddFile() { g_CodeAnalysisMutex.WaitOne(); ++NumFiles; g_CodeAnalysisMutex.ReleaseMutex(); }
	public static void AddJS() { g_CodeAnalysisMutex.WaitOne(); ++NumJS; g_CodeAnalysisMutex.ReleaseMutex(); }
	public static void AddExported() { g_CodeAnalysisMutex.WaitOne(); ++NumExported; g_CodeAnalysisMutex.ReleaseMutex(); }
	public static void AddLegacy() { g_CodeAnalysisMutex.WaitOne(); ++NumLegacyAPICalls; g_CodeAnalysisMutex.ReleaseMutex(); }
	public static void AddLegacySQL(string strFunctionName)
	{
		g_CodeAnalysisMutex.WaitOne();

		if (CodeAnalysis.g_dictLegacySQLCounts.ContainsKey(strFunctionName))
		{
			g_dictLegacySQLCounts[strFunctionName]++;
		}
		else
		{
			g_dictLegacySQLCounts[strFunctionName] = 1;
		}

		++NumLegacySQLCalls;

		g_CodeAnalysisMutex.ReleaseMutex();
	}

	public static List<string> g_strHTMLFilesAllowedNoCore = new List<string>
	{
		"callingscreen.html",
		"chatscreen.html",
		"contactsscreen.html",
		"dialpadscreen.html",
		"homescreen.html",
		"lockscreen.html",
		"messagescreen.html",
		"pickupscreen.html",
		"taxiscreen.html"
	};

	public static Dictionary<string, List<string>> g_strBannedCodeAnalysisWords = new Dictionary<string, List<string>>
	{
		{ "SendChatMessage", new List<string>() {"Player.cs", "AccountSystem.cs" }},
		{ "SendChatMessageToPlayer", new List<string>() {"Player.cs" }},
		{ "SetSkin(", new List<string>() {"Player.cs" }},
		{ "Event.PlayerConnected", new List<string>() {"ScriptLoader.cs" }},
		{ "Event.PlayerDisconnected", new List<string>() {"ScriptLoader.cs" }},
		{ "System.Timers", new List<string>() { "Boot.cs" }},
		{ "Timers.Timer", new List<string>() { "Boot.cs" }},
		{ "ContinueWith", new List<string>() { "TaskExtensions.cs" }},
		{ "RAGE.Events.Add(", new List<string>() { "EventManager.Client.cs" }},
		{ "RAGE.Events.AddDataHandler", new List<string>() { "EventManager.Client.cs" }},
		{ "RAGE.Events.CallLocal", new List<string>() { "EventManager.Client.cs", "VehicleSystem.Client.cs", "PerfManager.Client.cs", "CharacterSelection.Client.cs", "AnimationsUI.Client.cs", "RageClientStorage.Client.cs" }},
		{ "RAGE.Events.CallRemote", new List<string>() { "EventManager.Client.cs", "CharacterSelection.Client.cs" }}, // TODO_CSHARP: Remove everything except from EventManager when fully C#
		{ "RAGE.Events.EnableKeyRemoteEvent", new List<string>() { "EventManager.Client.cs" }},
		{ "RAGE.Events.EnableKeyDataChangeEvent", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerEnterCheckpoint", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnEventTriggeredByKey", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnEntityDataChangeByKey", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.Tick", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerStopTalking", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerStartTalking", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerLeaveVehicle", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerEnterVehicle", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerStartEnterVehicle", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnBrowserLoadingFailed", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnBrowserDomReady", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnBrowserCreated", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnGuiReady", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerWeaponShot", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnClickWithRaycast", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnClick", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerResurrect", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerDeath", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerExitCheckpoint", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerEnterColshape", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerExitColshape", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerChat", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerCommand", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnEntityCreated", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnEntityDestroyed", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnScriptWindowDestroyed", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnEntityStreamIn", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnEntityStreamOut", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerJoin", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerQuit", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnPlayerSpawn", new List<string>() { "EventManager.Client.cs" }},
		{ "Rage.Events.OnScriptWindowCreated", new List<string>() { "EventManager.Client.cs" }},

		// Use NetworkEvents.OnPlayerConnected instead, which is AFTER CPlayer is created, so you can actually do things, send events, etc
		{ "RageEvents.RAGE_OnPlayerConnected", new List<string>() { "EventManager.cs", "PlayerPool.cs", "Core.cs" }},

		// Use owl script loader instead
		{ "[ServerEvent(Event.Resource", new List<string>() { }},

		// Use owl script events instead
		{ "[ServerEvent(", new List<string>() { "EventManager.cs" }},

		{ "[Command(", new List<string>() { }},
		{ "RAGE.Ui.Cursor.Visible", new List<string>() { "Cursor.Client.cs" }},
		{ ".GetSharedData(", new List<string>() { "DataHelper.Client.cs" }},
		//{ ".GetData<", new List<string>() { "" }},
		// TODO_CSHARP: Reenable
		{ "Discord.Update<", new List<string>() { "DiscordManager.Client.cs" }},
		{ "window.onload", new List<string>() { "core.js" }},
		{ "RAGE.Game.Misc.GetHashKey", new List<string>() { "HashHelper.Client.cs" }},
		{ "EventManager.", new List<string>() { "EventManager.cs", "EventManager.Client.cs", "EventManager.Definitions.cs", "EventManager.Definitions.Client.cs", "ClientTimer.Client.cs", "EventComposer.cs", "CEFCore.Client.cs", "Core.cs" }},
		{ ".TriggerEvent(", new List<string>() { "EventManager.cs", "EventManager.Client.cs" }},
		{ ".TriggerClientEvent(", new List<string>() { "EventManager.cs", "EventManager.Client.cs" }},
		{ "[RemoteEvent", new List<string>() { "EventManager.cs", "EventManager.Client.cs" }},
		{ ".Kick(", new List<string>() { "Player.cs" }},
		{ "Client.Serial", new List<string>() { "Player.cs" }},
		{ "RAGE_TimerTickInternal_DO_NOT_USE", new List<string>() { "ClientTimer.Client.cs", "EventManager.Client.cs" }},
		{ "NAPI.Blip.CreateBlip", new List<string>() { "HelperFunctions.Blip.cs" }},
		{ "NAPI.Exported", new List<string>() { }},
		{ "RAGE.Input.", new List<string>() { "KeyBinds.Client.cs", "ScreenshotHelper.Client.cs" }},
		{ "Environment.TickCount", new List<string>() { "ActivityInstance.Blackjack.cs" }},

		{ "GTAInstance.Position =", new List<string>() { "Vehicle.cs", "RoadblockSystem.cs", "GenericsSystem.cs" }},
		{ "string.format", new List<string>() { "ScriptLoader.cs", "SerializationDefinitions.cs", "EventComposer.cs", "Boot.cs", "Management.cs", "Unarchiver.cs", "MapLoader.cs", "Discord.cs" }},
		{ "Vehicle.Class", new List<string>() { "Vehicle.cs", "AntiCheatSystem.cs" }},

		// NOTE: You can use this function, just add it here BUT BE WARNED: If this is being used ingame (e.g. clothing store) and not pre-spawn (e.g. create char)
		//																		you MUST have the server side call cache health (when player enters the system) and restore health (when done) Otherwise GTA maxes out their HP etc.
		{ "RAGE.Elements.Player.LocalPlayer.Model =", new List<string>() { "CharacterCreationData.Client.cs", "ClothingStore.Client.cs", "DutySystem.Client.cs", "DutyOutfitEditor.Client.cs" }},

		// Use CVehicle.EngineOn instead to ensure data gets synced correctly
		{ ".EngineStatus", new List<string>() { "Vehicle.cs" }},

		// Use vehicle definitions instead, RAGE doesnt include mod vehicles or some DLC vehicles
		{ "RAGE.Game.Vehicle.GetDisplayName", new List<string>() { } },
		{ "GetDisplayNameFromVehicleModel", new List<string>() { } },

		// Use Core.GetServerTime() instead, this time is wrong
		{ "NAPI.World.GetTime", new List<string>() { }},

		// Use Player.GetActiveClothing instead
		{ "Client.GetClothes", new List<string>() { }},

		// Use Player.GetCurrentPropDrawables / GetCurrentPropTextures instead
		{ "Client.GetAccessory", new List<string>() { }},

		// Use Player.GetCharacterName instead
		{ "Client.Name", new List<string> () { "Player.cs" } },
		{ "Client.Nametag", new List<string> () { "Player.cs" } },

		// Use OptimizationCachePool.StreamedInObjects()
		{ "RAGE.Elements.Entities.Objects.All", new List<string> () { "EventManager.Client.cs", "OptimizationCachePool.Client.cs" } },

		// Use DiscordManager
		{ "RAGE.Discord", new List<string> () { "DiscordManager.Client.cs" } },

		// Use owl script loader
		{ ": Script", new List<string>() { "ScriptLoader.cs" }},
	};

	public static List<string> g_lstUIJavascriptFiles = new List<string>
		{
			"bootstrap-notify.min.js",
			"core.js",
			"jquery-ui.min.js",
			"jquery.min.js",
			"jscolor.min.js",
			"player_radialmenu_src.js",
			"playfabclientapi.js",
			"vue.min.js"
		};

	public static void ShowResults()
	{
		if (!CookerSettings.IsFastIterationMode())
		{
			g_CodeAnalysisMutex.WaitOne();

			Console.WriteLine("\n\t========== Code Analysis ==========");

			Console.WriteLine("\n\t\tStatic Checks");
			Console.WriteLine("\t\t\tNum Code Files: {0}", NumFiles);
			Console.WriteLine("\t\t\tNum Code Lines: {0}", NumLines);
			Console.WriteLine("\t\t\tNum JS Files: {0}", NumJS);
			Console.WriteLine("\t\t\tNum Exports: {0}", NumExported);
			Console.WriteLine("\t\t\tNum Legacy API calls: {0}", NumLegacyAPICalls);
			Console.WriteLine("\t\t\tNum Legacy SQL calls: {0}", NumLegacySQLCalls);

			Console.WriteLine("\n\t\tTop 10 Legacy SQL calls:");
			foreach (var keyValuePair in g_dictLegacySQLCounts.OrderByDescending(keyValuePair => keyValuePair.Value).Take(10))
			{
				int count = keyValuePair.Value;
				Console.WriteLine("\t\t\t{0}: {1} ({2}%)", keyValuePair.Key, keyValuePair.Value, Math.Ceiling(((float)count / (float)NumLegacySQLCalls) * 100.0));
			}

			// TODOS
			Console.WriteLine("\n\t\tProject TODO's ({0}):", NumTodos);
			foreach (var keyValuePair in g_dictTodos.OrderByDescending(keyValuePair => keyValuePair.Value))
			{
				int numTodos = keyValuePair.Value;
				Console.WriteLine("\t\t\t{0}: {1} ({2}%)", keyValuePair.Key.ToUpper(), keyValuePair.Value, Math.Ceiling(((float)numTodos / (float)NumTodos) * 100.0));
			}

			g_CodeAnalysisMutex.ReleaseMutex();
		}
	}
}