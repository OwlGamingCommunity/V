using GTANetworkAPI;
using System;
using System.Collections.Generic;
// TODO_WORKFLOW: Check all dispose functions
public class AntiCheatSystem : IDisposable
{
	private class AnticheatAlertCooldown
	{
		public AnticheatAlertCooldown(CPlayer player, ECheatType cheatType)
		{
			m_PlayerRef = new WeakReference<CPlayer>(player);
			m_cheatType = cheatType;
			m_StartTime = DateTime.Now;
		}

		public bool HasExpired()
		{
			const int DurationMS = 10000;
			Int64 timeSinceStart = MainThreadTimerPool.GetMillisecondsSinceDateTime(m_StartTime);
			return timeSinceStart >= DurationMS;
		}

		public WeakReference<CPlayer> m_PlayerRef { get; }
		public ECheatType m_cheatType { get; }
		private DateTime m_StartTime { get; }
	}

	private List<AnticheatAlertCooldown> g_lstCooldowns = new List<AnticheatAlertCooldown>();

	private Dictionary<ECheatType, EAnticheatAction> g_dictActions = new Dictionary<ECheatType, EAnticheatAction>()
	{
		{ ECheatType.Weapon,  EAnticheatAction.InformAdmins },
		{ ECheatType.SpeedHack,  EAnticheatAction.InformAdmins },
		{ ECheatType.VehicleSpawnHack,  EAnticheatAction.InformAdmins },
		{ ECheatType.EntityDataModified,  EAnticheatAction.InformAdmins }
	};

	public AntiCheatSystem()
	{
		//m_VerifyEntityDataTimer = MainThreadTimerPool.CreateGlobalTimer(VerifyPlayerEntityData, 60000);

		RageEvents.RAGE_OnPlayerWeaponSwitch += OnWeaponSwitch;

		RageEvents.RAGE_OnPlayerEnterVehicle += OnEnterVehicle;

		RageEvents.RAGE_OnUpdate += OnUpdate;
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	private void OnEnterVehicle(Player client, Vehicle vehicle, sbyte seatId)
	{
		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

		// If the vehicle entered exists on the client... and not on the server... it's a hacked vehicle (script does use clientside vehicles, but only for show, the player never enters them)
		if (pVehicle == null && vehicle != null)
		{
			WeakReference<CPlayer> SourcePlayerRef = PlayerPool.GetPlayerFromClient(client);
			OnCheatDetected(SourcePlayerRef.Instance(), ECheatType.VehicleSpawnHack, Helpers.FormatString("Spawned a {0}", vehicle.DisplayName));
		}
	}

	private void OnUpdate()
	{
		// Only check ingame cheaters for things like teleports etc
		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			if (player != null && player.Client != null)
			{
				// speed hack
				bool bIsInVehicle = player.IsInVehicleReal;

				if (!bIsInVehicle || (bIsInVehicle && player.Client.Vehicle != null))
				{
					Vector3 vecVel = NAPI.Entity.GetEntityVelocity(bIsInVehicle ? player.Client.Vehicle.Handle : player.Client.Handle);
					Vector3 vecSquared = new Vector3(vecVel.X * vecVel.X, vecVel.Y * vecVel.Y, vecVel.Z * vecVel.Z);
					float fSpeed = (float)Math.Sqrt(vecSquared.Length());

					float fSpeedLimit = 55.0f;
					if (bIsInVehicle)
					{
						if (player.Client.Vehicle.Class == (int)EVehicleClass.VehicleClass_Boats)
						{
							fSpeedLimit = 200.0f;
						}
						else if (player.Client.Vehicle.Class == (int)EVehicleClass.VehicleClass_Planes || player.Client.Vehicle.Class == (int)EVehicleClass.VehicleClass_Helicopters)
						{
							fSpeedLimit = 500.0f;
						}
						else
						{
							fSpeedLimit = 100.0f;
						}
					}

					if (fSpeed > fSpeedLimit)
					{
						OnCheatDetected(player, ECheatType.SpeedHack, bIsInVehicle ? Helpers.FormatString("Vehicle: {0} speed ({1})", fSpeed, player.Client.Vehicle.DisplayName) : Helpers.FormatString("OnFoot: {0} speed", fSpeed));
					}
				}
			}
		}
	}

	private string GenerateRandomString(int length)
	{
		var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var stringChars = new char[length];
		var random = new Random();
		for (int i = 0; i < stringChars.Length; i++)
		{
			stringChars[i] = chars[random.Next(chars.Length)];
		}

		return new String(stringChars);
	}

	private void UpdateCooldowns()
	{
		List<AnticheatAlertCooldown> lstItemsToRemove = new List<AnticheatAlertCooldown>();
		foreach (AnticheatAlertCooldown cooldown in g_lstCooldowns)
		{
			if (cooldown.HasExpired())
			{
				lstItemsToRemove.Add(cooldown);
			}
		}

		foreach (var itemToRemove in lstItemsToRemove)
		{
			g_lstCooldowns.Remove(itemToRemove);
		}
	}

	private void OnCheatDetected(CPlayer player, ECheatType cheatType, string strContext)
	{
		if (player == null)
		{
			return;
		}

		UpdateCooldowns();

		// Do we have a cooldown?
		foreach (AnticheatAlertCooldown cooldown in g_lstCooldowns)
		{
			if (cooldown.m_PlayerRef.Instance() == player && cooldown.m_cheatType == cheatType)
			{
				return;
			}
		}

		// Create a cooldown so we dont spam (incase its msg only)
		g_lstCooldowns.Add(new AnticheatAlertCooldown(player, cheatType));


		string strCharacterName = player.GetCharacterName(ENameType.StaticCharacterName);
		if (String.IsNullOrEmpty(strCharacterName))
		{
			strCharacterName = "Not Spawned";
		}

		string strMessage = Helpers.FormatString("CHEAT DETECTED [{0} - {1}]: {2}", player.Username, strCharacterName, cheatType);
		string strContextMessage = Helpers.FormatString("Context: {0}", strContext);
		string strActionTaken = String.Empty;

		EAnticheatAction action = g_dictActions[cheatType];
		if (action == EAnticheatAction.InformAdmins)
		{
			// Nothing to do here, we already did it above
			strActionTaken = "No action was taken, please recon / investigate user";
		}
		else if (action == EAnticheatAction.Kick)
		{
			player.KickFromServer(CPlayer.EKickReason.ANTICHEAT, "AntiCheat Kick");
			strActionTaken = "Player was kicked";
		}
		else if (action == EAnticheatAction.Ban)
		{
			player.AnticheatBan(cheatType);
			strActionTaken = "Player was permanently banned";
		}

		HelperFunctions.Chat.SendMessageToAdmins(strMessage, false, EAdminLevel.TrialAdmin, 255, 0, 0);
		HelperFunctions.Chat.SendMessageToAdmins(strContextMessage, false, EAdminLevel.TrialAdmin, 255, 0, 0);
		HelperFunctions.Chat.SendMessageToAdmins(strActionTaken, false, EAdminLevel.TrialAdmin, 255, 0, 0);

		DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AnticheatAlerts, strMessage);
		DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AnticheatAlerts, strContextMessage);
		DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AnticheatAlerts, strActionTaken);
	}

	private void OnWeaponSwitch(Player client, WeaponHash oldWeaponHash, GTANetworkAPI.WeaponHash newWeaponHash)
	{
		// Do we actually have the weapon we just switched to?
		EItemID weaponItem = EItemID.None;
		foreach (var kvPair in ItemWeaponDefinitions.g_DictItemIDToWeaponHash)
		{
			if (kvPair.Value == newWeaponHash)
			{
				weaponItem = kvPair.Key;
			}
		}

		if (weaponItem != EItemID.None)
		{
			WeakReference<CPlayer> SourcePlayerRef = PlayerPool.GetPlayerFromClient(client);
			CPlayer SourcePlayer = SourcePlayerRef.Instance();

			if (SourcePlayer != null && SourcePlayer.IsSpawned && !SourcePlayer.HasTemporaryImmunityAgainstWeaponGrantHacks())
			{
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromDefaultValue(weaponItem, 0.0f);
				if (!SourcePlayer.Inventory.HasItem(ItemInstanceDef, false, out CItemInstanceDef outItem))
				{
					OnCheatDetected(SourcePlayer, ECheatType.Weapon, Helpers.FormatString("Hash: {0} ItemID: {1}", newWeaponHash.ToString(), weaponItem.ToString()));
				}
			}
		}
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		if (a_CleanupNativeAndManaged)
		{

		}
	}

	private void VerifyPlayerEntityData(object[] a_Parameters = null)
	{
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers_IncludeOutOfGame();
		foreach (var pPlayer in players)
		{
			if (!pPlayer.ValidateEntityDataIntact(pPlayer.Client, out string strFirstDataModified))
			{
				OnCheatDetected(pPlayer, ECheatType.EntityDataModified, strFirstDataModified);
			}
		}
	}



	public void RegisterEntityData(string a_Key, object a_Value)
	{
		m_dictEntityHashes[a_Key] = HelperFunctions.Hashing.sha256(a_Value.ToString());
	}

	//private WeakReference<MainThreadTimer> m_VerifyEntityDataTimer = new WeakReference<MainThreadTimer>(null);
	private Dictionary<string, string> m_dictEntityHashes = new Dictionary<string, string>();
}

