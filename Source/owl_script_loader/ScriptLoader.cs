using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class OwlScriptManager : GTANetworkAPI.Script
{
	private static HashSet<Type> g_Scripts = new HashSet<Type>()
	{
		typeof(Core),
		typeof(Database.ThreadedMySQL),
		typeof(VehicleSystem),
		typeof(AccountSystem),
		typeof(AchievementSystem),
		typeof(ActivitySystem),
		typeof(ChatSystem),
		typeof(BankingSystem),
		typeof(AdminSystem),
		typeof(HTTPInstance),
		typeof(FactionSystem),
		typeof(ItemSystem),
		typeof(JobSystem),
		typeof(AnimationSystem),
		typeof(AntiCheatSystem),
		typeof(DonationSystem),
		typeof(PropertySystem),
		typeof(RadioSystem),
		typeof(StoreSystem),
		typeof(VehicleStore)
	};

	public OwlScriptManager()
	{
		DateTime dtStart = DateTime.Now;

		// LOAD ALL DLLS FIRST
		// NOTE: Cooker doesn't support unarchiving to subdirectories, so we dont have to do this recursively
		foreach (string strDirFullPath in Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "dotnet", "resources")))
		{
			foreach (string strDLLFullPath in Directory.GetFiles(strDirFullPath, "*.dll"))
			{
				Assembly domainAssembly = Assembly.LoadFrom(strDLLFullPath);

				if (domainAssembly == null)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					string strError = String.Format("OWL_LOADER FATAL ERROR: Failed to load DLL {0} at {1}", Path.GetFileName(strDLLFullPath), strDirFullPath);
					Console.WriteLine(strError);
					throw new Exception(strError);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Green;
					string strError = String.Format("OWL_LOADER: Loaded DLL {0} at {1}", Path.GetFileName(strDLLFullPath), strDirFullPath);
					Console.WriteLine(strError);
				}
			}
		}

		foreach (Type scriptType in g_Scripts)
		{
			DateTime dtStartScript = DateTime.Now;

			Activator.CreateInstance(scriptType);

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("OWL_LOADER: Loaded {0} ({1}ms)", scriptType.Name, (DateTime.Now - dtStartScript).TotalMilliseconds);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("OWL LOADER: Loaded {0} scripts in {1}ms", g_Scripts.Count, (DateTime.Now - dtStart).TotalMilliseconds);
		Console.ForegroundColor = ConsoleColor.Gray;
	}

	// NOTE: Disabled on purpose, use the Owl Script loader
	//[ServerEvent(Event.ResourceStart)] public void RAGE_OnResourceStart() { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnScriptStart, true); }
	//[ServerEvent(Event.ResourceStop)] public void RAGE_OnResourceStop() { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnScriptStop, true); }
	//[ServerEvent(Event.ResourceStartEx)] public void RAGE_OnResourceStartEx(string resourceName) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnResourceStart, false, resourceName); }
	//[ServerEvent(Event.ResourceStopEx)] public void RAGE_OnResourceStopEx(string resourceName) { EventManager.InvokeRageDelegate(RageEvents.RAGE_OnResourceStop, false, resourceName); }

	[GTANetworkAPI.RemoteEvent("CE")] public void OnCustomEvent(GTANetworkAPI.Player sender, params object[] arguments) { Core.OnCustomEvent(sender, arguments); }

	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.Update)] void RAGE_OnUpdate() { Core.RAGE_OnUpdate(); Database.ThreadedMySQL.Tick(); }

	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.ChatMessage)] void RAGE_OnChatMessage(GTANetworkAPI.Player client, System.String message) { Core.RAGE_OnChatMessage(client, message); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.EntityCreated)] void RAGE_OnEntityCreated(GTANetworkAPI.Entity entity) { Core.RAGE_OnEntityCreated(entity); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.EntityDeleted)] void RAGE_OnEntityDeleted(GTANetworkAPI.Entity entity) { Core.RAGE_OnEntityDeleted(entity); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.EntityModelChange)] void RAGE_OnEntityModelChange(GTANetworkAPI.Entity entity, System.UInt32 oldModel) { Core.RAGE_OnEntityModelChange(entity, oldModel); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerConnected)] void RAGE_OnPlayerConnected(GTANetworkAPI.Player client) { Core.RAGE_OnPlayerConnected(client); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerDisconnected)] void RAGE_OnPlayerDisconnected(GTANetworkAPI.Player client, GTANetworkAPI.DisconnectionType type, string reason) { Core.RAGE_OnPlayerDisconnected(client, type, reason); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerSpawn)] void RAGE_OnPlayerSpawn(GTANetworkAPI.Player client) { Core.RAGE_OnPlayerSpawn(client); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerDeath)] void RAGE_OnPlayerDeath(GTANetworkAPI.Player client, GTANetworkAPI.Player killer, System.UInt32 reason) { Core.RAGE_OnPlayerDeath(client, killer, reason); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerDamage)] void RAGE_OnPlayerDamage(GTANetworkAPI.Player client, float healthLoss, float armorLoss) { Core.RAGE_OnPlayerDamage(client, healthLoss, armorLoss); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerPickup)] void RAGE_OnPlayerPickup(GTANetworkAPI.Player client, GTANetworkAPI.Pickup pickup) { Core.RAGE_OnPlayerPickup(client, pickup); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerWeaponSwitch)] void RAGE_OnPlayerWeaponSwitch(GTANetworkAPI.Player client, GTANetworkAPI.WeaponHash oldWeaponHash, GTANetworkAPI.WeaponHash newWeaponHash) { Core.RAGE_OnPlayerWeaponSwitch(client, oldWeaponHash, newWeaponHash); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerDetonateStickies)] void RAGE_OnPlayerDetonateStickies(GTANetworkAPI.Player client) { Core.RAGE_OnPlayerDetonateStickies(client); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerEnterCheckpoint)] void RAGE_OnPlayerEnterCheckpoint(GTANetworkAPI.Checkpoint checkpoint, GTANetworkAPI.Player client) { Core.RAGE_OnPlayerEnterCheckpoint(checkpoint, client); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerExitCheckpoint)] void RAGE_OnPlayerExitCheckpoint(GTANetworkAPI.Checkpoint checkpoint, GTANetworkAPI.Player client) { Core.RAGE_OnPlayerExitCheckpoint(checkpoint, client); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerEnterColshape)] void RAGE_OnPlayerEnterColshape(GTANetworkAPI.ColShape colShape, GTANetworkAPI.Player client) { Core.RAGE_OnPlayerEnterColshape(colShape, client); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerExitColshape)] void RAGE_OnPlayerExitColshape(GTANetworkAPI.ColShape colShape, GTANetworkAPI.Player client) { Core.RAGE_OnPlayerExitColshape(colShape, client); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerEnterVehicleAttempt)] void RAGE_OnPlayerEnterVehicleAttempt(GTANetworkAPI.Player client, GTANetworkAPI.Vehicle vehicle, sbyte seatId) { Core.RAGE_OnPlayerEnterVehicleAttempt(client, vehicle, seatId); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerExitVehicleAttempt)] void RAGE_OnPlayerExitVehicleAttempt(GTANetworkAPI.Player client, GTANetworkAPI.Vehicle vehicle) { Core.RAGE_OnPlayerExitVehicleAttempt(client, vehicle); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerEnterVehicle)] void RAGE_OnPlayerEnterVehicle(GTANetworkAPI.Player client, GTANetworkAPI.Vehicle vehicle, sbyte seatId) { Core.RAGE_OnPlayerEnterVehicle(client, vehicle, seatId); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.PlayerExitVehicle)] void RAGE_OnPlayerExitVehicle(GTANetworkAPI.Player client, GTANetworkAPI.Vehicle vehicle) { Core.RAGE_OnPlayerExitVehicle(client, vehicle); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleDamage)] void RAGE_OnVehicleDamage(GTANetworkAPI.Vehicle vehicle, float bodyHealthLoss, float engineHealthLoss) { Core.RAGE_OnVehicleDamage(vehicle, bodyHealthLoss, engineHealthLoss); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleDeath)] void RAGE_OnVehicleDeath(GTANetworkAPI.Vehicle vehicle) { Core.RAGE_OnVehicleDeath(vehicle); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleHornToggle)] void RAGE_OnVehicleHornToggle(GTANetworkAPI.Vehicle vehicle, bool bToggle) { Core.RAGE_OnVehicleHornToggle(vehicle, bToggle); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleSirenToggle)] void RAGE_OnVehicleSirenToggle(GTANetworkAPI.Vehicle vehicle) { Core.RAGE_OnVehicleSirenToggle(vehicle); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleDoorBreak)] void RAGE_OnVehicleDoorBreak(GTANetworkAPI.Vehicle vehicle, int index) { Core.RAGE_OnVehicleDoorBreak(vehicle, index); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleWindowSmash)] void RAGE_OnVehicleWindowSmash(GTANetworkAPI.Vehicle vehicle, int index) { Core.RAGE_OnVehicleWindowSmash(vehicle, index); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleTyreBurst)] void RAGE_OnVehicleTyreBurst(GTANetworkAPI.Vehicle vehicle, int index) { Core.RAGE_OnVehicleTyreBurst(vehicle, index); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.VehicleTrailerChange)] void RAGE_OnVehicleTrailerChange(GTANetworkAPI.Vehicle vehicle, GTANetworkAPI.Vehicle trailer) { Core.RAGE_OnVehicleTrailerChange(vehicle, trailer); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.FirstChanceException)] void RAGE_OnFirstChanceException(Exception exception) { Core.RAGE_OnFirstChanceException(exception); }
	[GTANetworkAPI.ServerEvent(GTANetworkAPI.Event.UnhandledException)] void RAGE_OnUnhandledException(Exception exception) { Core.RAGE_OnUnhandledException(exception); }
}