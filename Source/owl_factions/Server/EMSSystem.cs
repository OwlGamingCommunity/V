using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class EMSSystem
{
	public EMSSystem()
	{
		NAPI.Server.SetAutoRespawnAfterDeath(false);

		RageEvents.RAGE_OnUpdate += UpdateBleeding;
		RageEvents.RAGE_OnPlayerDeath += API_onPlayerDeath;

		// COMMANDS
		CommandManager.RegisterCommand("heal", "Heals a player", new Action<CPlayer, CVehicle, CPlayer>(HealCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("respawn", "Respawns yourself", new Action<CPlayer, CVehicle>(RespawnCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		CommandManager.RegisterCommand("forcerespawn", "Force respawns the target player, even if they are not dead, at the hospital and bills them", new Action<CPlayer, CVehicle, CPlayer>(ForceRespawn), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private void UpdateBleeding()
	{
		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			if (!player.Client.Dead)
			{
				// less than 50 health and not bleeding? make them bleed
				if (player.Client.Health <= 50)
				{
					// do we need to update moveset?
					if (player.Client.Health > 30)
					{
						if (player.MoveClipset != EClipsetID.Injured)
						{
							ResetBleeding(player, true);
							HelperFunctions.Chat.SendAmeMessage(player, "begins to limp.");
							player.MoveClipset = EClipsetID.Injured;
						}
					}
					else if (player.Client.Health > 15)
					{
						if (player.MoveClipset != EClipsetID.Injured2)
						{
							ResetBleeding(player, true);
							HelperFunctions.Chat.SendAmeMessage(player, "begins to limp and struggle to walk.");
							player.MoveClipset = EClipsetID.Injured2;
						}
					}
					else if (!player.HasBleedingAnimation) // force anim on them
					{
						// no anims if in vehicle
						if (!player.IsInVehicleReal)
						{
							ResetBleeding(player, true);
							player.HasBleedingAnimation = true;
							player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame), "amb@world_human_bum_slumped@male@laying_on_left_side@base", "base", false, true, true, 0, false);
							HelperFunctions.Chat.SendAmeMessage(player, "falls to the ground.");
						}
					}

					// need to update flag?
					if (!player.IsBleeding)
					{
						player.IsBleeding = true;
					}
				}
				else if (player.Client.Health > 50 && player.IsBleeding)
				{
					ResetBleeding(player, false);
				}

			}
		}
	}

	private void ResetBleeding(CPlayer SourcePlayer, bool bAnimOnly)
	{
		if (!bAnimOnly)
		{
			SourcePlayer.IsBleeding = false;
			SourcePlayer.ClearMoveClipset();
		}

		SourcePlayer.StopCurrentAnimation(true, true);
		SourcePlayer.HasBleedingAnimation = false;
	}

	public void API_onPlayerDeath(Player sender, Player entityKiller, uint weapon)
	{
		WeakReference<CPlayer> PlayerRef = PlayerPool.GetPlayerFromClient(sender);
		CPlayer player = PlayerRef.Instance();

		if (player != null)
		{
			// incase they had an injured anim
			ResetBleeding(player, false);

			if (weapon == (uint)WeaponHash.Snowball)
			{
				if (HelperFunctions.World.IsChristmas())
				{
					RespawnPlayer(player, weapon);
				}
				else if (HelperFunctions.World.IsHalloween())
				{
					player.SendNotification("Halloween", ENotificationIcon.InfoSign, "You fade out to darkness and see a ghost...");

					MainThreadTimerPool.CreateEntityTimer((object[] parameters) =>
					{
						RespawnPlayer(player, weapon);
						player.AwardAchievement(EAchievementID.Halloween_RIP);
					}, 10000, player, 1);
				}
			}
			else
			{
				player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "You died! Type /respawn in order to go to the hospital or wait until a paramedic treats you.");
				player.HandleDeathAchievements(entityKiller, weapon);
			}

			NetworkEventSender.SendNetworkEvent_StartDeathEffect(player);
		}
	}

	private void RespawnPlayer(CPlayer SourcePlayer, uint weapon = 0)
	{
		ResetBleeding(SourcePlayer, false);

		bool bIsJailed = SourcePlayer.IsJailed();
		bool bIsPaletoBay = SourcePlayer.Client.Position.Y >= Constants.BorderOfLStoPaleto;
		uint targetDimension = bIsJailed ? SourcePlayer.Client.Dimension : 0;

		Vector3 vecRespawnPos = bIsJailed ? SourcePlayer.Client.Position : (bIsPaletoBay ? new Vector3(-246.3201, 6330.104, 32.52617) : new Vector3(298.7604, -584.6354, 43.26083));
		float fRespawnRot = bIsJailed ? SourcePlayer.Client.Rotation.Z : (bIsPaletoBay ? -138.9874f : 65.9893f);
		NAPI.Player.SpawnPlayer(SourcePlayer.Client, vecRespawnPos, fRespawnRot);

		NAPI.Player.SetPlayerHealth(SourcePlayer.Client, 100);

		// Don't deduct costs for snowballs :)
		bool bIsChristmas = HelperFunctions.World.IsChristmas();
		if (bIsChristmas && weapon == (uint)WeaponHash.Snowball)
		{
			SourcePlayer.SendNotification("Healthcare", ENotificationIcon.HeartEmpty, "Your healthcare bill was free. Merry Christmas!");
			// Just respawn them where they were
			vecRespawnPos = SourcePlayer.Client.Position;
			fRespawnRot = SourcePlayer.Client.Rotation.Z;
		}
		else if (HelperFunctions.World.IsHalloween() && weapon == (uint)WeaponHash.Snowball)
		{
			SourcePlayer.SendNotification("Halloween", ENotificationIcon.HeartEmpty, "A spiritual being resurrected you!");
			// Just respawn them back at the halloween area
			vecRespawnPos = new Vector3(158.3252f, -989.4136f, 30.09193f);
			fRespawnRot = 340.0f;
		}
		else
		{
			float fCostToRespawn = 50.0f; // TODO_LAUNCH: Dynamic based on damage
			if (!SourcePlayer.SubtractBankBalanceIfCanAfford(fCostToRespawn, PlayerMoneyModificationReason.EMSRespawn))
			{
				SourcePlayer.SubtractMoneyAllowNegative(fCostToRespawn, PlayerMoneyModificationReason.EMSRespawn);
			}

			CFaction Faction = FactionPool.GetFDEMSFaction();
			Faction.Money += fCostToRespawn;

			SourcePlayer.SendNotification("Healthcare", ENotificationIcon.HeartEmpty, "Your healthcare bill was ${0:0.00}", fCostToRespawn);
		}



		SourcePlayer.SetSpawnFix(vecRespawnPos, new Vector3(0.0, 0.0, fRespawnRot), targetDimension, 100, SourcePlayer.Client.Armor);

		SourcePlayer.SetPositionSafe(vecRespawnPos);
		SourcePlayer.Client.Rotation = new Vector3(0.0, 0.0, fRespawnRot);
		// TODO_POST_LAUNCH: Unload IPL if inside interior?

		SourcePlayer.SetSafeDimension(targetDimension);

		NetworkEventSender.SendNetworkEvent_RespawnChar(SourcePlayer);
	}

	private void ForceRespawn(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
	{
		RespawnPlayer(TargetPlayer);
	}

	private void RespawnCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		if (SourcePlayer != null)
		{
			if (SourcePlayer.Client.Dead)
			{
				RespawnPlayer(SourcePlayer);
			}
			else
			{
				SourcePlayer.SendNotification("Respawn", ENotificationIcon.InfoSign, "You must be dead to use this command.");
			}
		}
	}

	public void HealCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
	{
		if (SourcePlayer == TargetPlayer)
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You can't heal yourself");
			return;
		}

		// Are we in the EMS faction AND on duty?
		if (SourcePlayer.IsInFactionOfType(EFactionType.Medical))
		{
			if (SourcePlayer.IsOnDutyOfType(EDutyType.EMS) || SourcePlayer.IsOnDutyOfType(EDutyType.Fire))
			{
				TargetPlayer.Client.Health = 100;
				// revive fix for RAGE 1.1
				NAPI.Player.SpawnPlayer(TargetPlayer.Client, TargetPlayer.Client.Position, TargetPlayer.Client.Rotation.Z);
				TargetPlayer.Save();

				SourcePlayer.SendNotification("Healthcare", ENotificationIcon.HeartEmpty, "You have healed {0}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
				TargetPlayer.SendNotification("Healthcare", ENotificationIcon.HeartEmpty, "You were healed by {0}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName));

				new Logging.Log(SourcePlayer, Logging.ELogType.FactionAction, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/heal - {0}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
			}
			else
			{
				SourcePlayer.SendNotification("Healthcare", ENotificationIcon.ExclamationSign, "You must be on duty to heal others.", null);
			}
		}
		else
		{
			SourcePlayer.SendNotification("Healthcare", ENotificationIcon.ExclamationSign, "You must in a Medical faction to use this command.", null);
		}
	}
}

