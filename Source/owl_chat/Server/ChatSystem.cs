using GTANetworkAPI;
using Logging;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class ChatSystem
{
	List<CPlayer> m_PrivateMessagesDisabled = new List<CPlayer>();
	List<CPlayer> m_AdvertisementsDisabled = new List<CPlayer>();

	private class BigEars
	{
		public CPlayer admin;
		public CPlayer player;
	}
	List<BigEars> m_BigEars = new List<BigEars>();

	public const float LANGUAGE_XP = 0.1f;

	private LanguageSystem m_LanguageSystem = new LanguageSystem();
	private PhoneSystem m_PhoneSystem = new PhoneSystem();

	public ChatSystem()
	{
		NAPI.Server.SetGlobalServerChat(false);

		// COMMANDS
		CommandManager.RegisterCommand("pos", "Shows the players position", new Action<CPlayer, CVehicle>(PosCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("vehpos", "Shows the players vehicle position", new Action<CPlayer, CVehicle>(VehPosCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle);
		CommandManager.RegisterCommand("id", "Retrieves a players ID", new Action<CPlayer, CVehicle, string>(IDCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("me", "Performs a me action", new Action<CPlayer, CVehicle, string>(OnMeMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("do", "Performs a do action", new Action<CPlayer, CVehicle, string>(OnDoMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("save", "Saves your current character", new Action<CPlayer, CVehicle>(SaveCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("ks", "Commits suicide", new Action<CPlayer, CVehicle>(KillCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("k", "Forcefully disconnects from the server", new Action<CPlayer, CVehicle>(KickSelf), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("b", "Sends a local OOC message", new Action<CPlayer, CVehicle, string>(OnLocalOOCMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("g", "Sends a government chat message", new Action<CPlayer, CVehicle, string>(GovtChat), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("m", "Sends a megaphone message", new Action<CPlayer, CVehicle, string>(MegaphoneChat), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("radios", "Displays the characters radios", new Action<CPlayer, CVehicle>(Radios), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("r", "Sends a radio message to your first radio", new Action<CPlayer, CVehicle, string>(RadioChat), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("r2", "Sends a radio message to your second radio", new Action<CPlayer, CVehicle, string>(Radio2Chat), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("r3", "Sends a radio message to your third radio", new Action<CPlayer, CVehicle, string>(Radio3Chat), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("rlow", "Sends a low volume radio message to your first radio", new Action<CPlayer, CVehicle, string>(RadioChatLow), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("r2low", "Sends a low volume radio message to your second radio", new Action<CPlayer, CVehicle, string>(Radio2ChatLow), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("r3low", "Sends a low volume radio message to your third radio", new Action<CPlayer, CVehicle, string>(Radio3ChatLow), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f", "Sends a faction message to faction 1", new Action<CPlayer, CVehicle, string>(FactionChat), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f2", "Sends a faction message to faction 2", new Action<CPlayer, CVehicle, string>(FactionChat2), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f3", "Sends a faction message to faction 3", new Action<CPlayer, CVehicle, string>(FactionChat3), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f4", "Sends a faction message to faction 4", new Action<CPlayer, CVehicle, string>(FactionChat4), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f5", "Sends a faction message to faction 5", new Action<CPlayer, CVehicle, string>(FactionChat5), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f6", "Sends a faction message to faction 6", new Action<CPlayer, CVehicle, string>(FactionChat6), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f7", "Sends a faction message to faction 7", new Action<CPlayer, CVehicle, string>(FactionChat7), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f8", "Sends a faction message to faction 8", new Action<CPlayer, CVehicle, string>(FactionChat8), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f9", "Sends a faction message to faction 9", new Action<CPlayer, CVehicle, string>(FactionChat9), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("f10", "Sends a faction message to faction 10", new Action<CPlayer, CVehicle, string>(FactionChat10), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("c", "Sends a close-by message", new Action<CPlayer, CVehicle, string>(OnClosebyMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("w", "Whisper a message", new Action<CPlayer, CVehicle, CPlayer, string>(OnWhisperCommand), CommandParsingFlags.TargetPlayerAndGreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("s", "Shouts a message", new Action<CPlayer, CVehicle, string>(OnShoutMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("sl", "Loudly shouts a message", new Action<CPlayer, CVehicle, string>(OnShoutLoudMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("a", "Admin chat", new Action<CPlayer, CVehicle, string>(OnAdminChatMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("uat", "Upper Administration Team chat", new Action<CPlayer, CVehicle, string>(OnUatAdminChatMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("ann", "Admin Announcement", new Action<CPlayer, CVehicle, string>(OnAdminAnnouncement), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("stats", "Shows your character statistics", new Action<CPlayer, CVehicle>(StatsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("astats", "Shows a character's statistics", new Action<CPlayer, CVehicle, CPlayer>(AStatsCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("pm", "Sends a private message to a target player", new Action<CPlayer, CVehicle, CPlayer, string>(PMCommand), CommandParsingFlags.TargetPlayerAndGreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("togpm", "Allows disabling the ability for others to direct message you.", new Action<CPlayer, CVehicle>(TogPMCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("togads", "Allows disabling seeing in-game advertisements.", new Action<CPlayer, CVehicle>(TogAdsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("district", "Sends an IC message to the district for an event", new Action<CPlayer, CVehicle, string>(OnDistrictMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("ad", "Creates an advertisement to show to other players.", new Action<CPlayer, CVehicle, int, bool, string>(AdCommand), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("factionad", "Creates an advertisement to show to other players.", new Action<CPlayer, CVehicle, int, int, bool, string>(FactionAdCommand), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.Default, aliases: new[] { "fad" });
		CommandManager.RegisterCommand("adgps", "Sets your GPS to an adverts location.", new Action<CPlayer, CVehicle, int>(AdGPSCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("adcleargps", "Clears your Advert GPS to.", new Action<CPlayer, CVehicle>(AdClearGPSCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("gps", "Sets your GPS to a ZIP code.", new Action<CPlayer, CVehicle, int>(GPSCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("cleargps", "Clears your GPS.", new Action<CPlayer, CVehicle>(ClearGPSCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("ame", "Performs an ame action", new Action<CPlayer, CVehicle, string>(OnAmeMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("ado", "Performs an ado action", new Action<CPlayer, CVehicle, string>(OnAdoMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("status", "Adds a status", new Action<CPlayer, CVehicle, string>(OnStatusMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("delstatus", "Removes a status", new Action<CPlayer, CVehicle>(OnClearStatusMessage), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("air", "Air radio", new Action<CPlayer, CVehicle, string>(OnAirMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("u", "Quick reply PM", new Action<CPlayer, CVehicle, string>(OnQuickReplyMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("bigears", "Listen to a player's private messages.", new Action<CPlayer, CVehicle, CPlayer>(BigEarsCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("togooc", "Allow or disallow players to use global OOC chat.", new Action<CPlayer, CVehicle>(TogOocCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("toggleooc", "Toggle out of character chat on or off.", new Action<CPlayer, CVehicle>(ToggleOocCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("o", "Send a message in global OOC.", new Action<CPlayer, CVehicle, string>(GlobalOocCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default, aliases: new[] { "ooc", "globalooc" });
		CommandManager.RegisterCommand("clearchat", "Clears your chat.", new Action<CPlayer, CVehicle>(ClearChatCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		// Chance commands
		CommandManager.RegisterCommand("luck", "Chooses a number between 1 and the maximum passed in", new Action<CPlayer, CVehicle, uint>(LuckCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("chance", "Determines a success/failure case based on the percentage chance of success provided", new Action<CPlayer, CVehicle, uint>(ChanceCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("flipcoin", "Flips a coin with a heads or tails outcome", new Action<CPlayer, CVehicle>(FlipCoinCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("coin", "Show off your skills with a coin trick", new Action<CPlayer, CVehicle>(CoinCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		CommandManager.RegisterCommand("weatherinfo", "Outputs the current weather conditions", new Action<CPlayer, CVehicle>(WeatherInfoCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		RageEvents.RAGE_OnChatMessage += OnChatMessage;

		// TODO: Move
		NetworkEvents.SaveControls += (CPlayer player, List<GameControlObject> lstGameControls) =>
		{
			// saving all controls everytime is kinda inefficient, but we don't expect this to occur THAT often (we also remove it if its a default key)
			Database.LegacyFunctions.SaveControls(player.AccountID, lstGameControls).ConfigureAwait(true);
		};
		NetworkEvents.SetAllControlsToDefault += (CPlayer player) =>
		{
			Database.LegacyFunctions.ResetControls(player.AccountID).ConfigureAwait(true);
		};

		NetworkEvents.CreateKeybind += CreateKeybind;

		NetworkEvents.DeleteKeybind += (CPlayer player, int index) =>
		{
			if (index < player.Keybinds.Count)
			{
				var keybindData = player.Keybinds[index];
				if (keybindData != null)
				{
					Database.LegacyFunctions.DeleteKeybind(player.AccountID, keybindData.ID).ConfigureAwait(true);
					player.Keybinds.RemoveAt(index);
				}
			}
		};
		NetworkEvents.TriggerKeybind += (CPlayer player, int index) =>
		{
			if (index < player.Keybinds.Count)
			{
				var keybindData = player.Keybinds[index];
				if (keybindData != null && keybindData.Action.Length > 0)
				{
					bool bIsCommand = keybindData.Action[0] == '/';
					if (bIsCommand)
					{
						CommandManager.OnPlayerRawCommand(player, keybindData.Action.Substring(1));
					}
					else
					{
						OnChatMessage(player.Client, keybindData.Action);
					}
				}
			}
		};
		CommandManager.RegisterCommand("keybinds", "Toggles the Controls & Keybinds UI", new Action<CPlayer, CVehicle>(ShowKeybindsUI), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("controls", "Toggles the Controls & Keybinds UI", new Action<CPlayer, CVehicle>(ShowKeybindsUI), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("bind", "Toggles the Controls & Keybinds UI", new Action<CPlayer, CVehicle>(ShowKeybindsUI), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("bindkey", "Toggles the Controls & Keybinds UI", new Action<CPlayer, CVehicle>(ShowKeybindsUI), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		// EVENTS
		NetworkEvents.GetPos += GetPos;

		NetworkEvents.SaveChatSettings += OnSaveChatSettings;
		NetworkEvents.ResetChatSettings += OnResetChatSettings;
	}

	private async void CreateKeybind(CPlayer player, ConsoleKey key, EPlayerKeyBindType bindType, string strAction)
	{
		EntityDatabaseID characterID = bindType == EPlayerKeyBindType.Character ? player.ActiveCharacterDatabaseID : -1;
		EntityDatabaseID keybindID = await Database.LegacyFunctions.CreateKeybind(player.AccountID, characterID, bindType, strAction, key).ConfigureAwait(true);
		player.Keybinds.Add(new PlayerKeybindObject(keybindID, key, bindType, strAction));
	}

	private void ShowKeybindsUI(CPlayer a_Player, CVehicle a_Vehicle)
	{
		NetworkEventSender.SendNetworkEvent_ShowKeyBindManager(a_Player);
	}

	private async void OnSaveChatSettings(CPlayer a_Player, ChatSettings settings)
	{
		if (settings.Tabs.Count == ChatConstants.MaxTabs)
		{
			await Database.LegacyFunctions.SaveChatSettings(a_Player.AccountID, settings).ConfigureAwait(true);
			a_Player.SendNotification("Chat", ENotificationIcon.InfoSign, "Chat settings have been saved.");
		}
	}

	private async void OnResetChatSettings(CPlayer a_Player)
	{
		await Database.LegacyFunctions.ResetChatSettings(a_Player.AccountID).ConfigureAwait(true);
		a_Player.SendNotification("Chat", ENotificationIcon.InfoSign, "Chat settings have been reset.");
	}

	private void LuckCommand(CPlayer a_Player, CVehicle a_Vehicle, uint UpperLimit)
	{
		if (UpperLimit >= 1)
		{
			int randNumber = new Random().Next(1, Convert.ToInt32(UpperLimit));
			SendChanceCommandMessage(a_Player, "{0} rolled a dice for {1} (0-{2})", a_Player.GetCharacterName(ENameType.StaticCharacterName), randNumber, UpperLimit);
		}
		else
		{
			a_Player.PushChatMessage(EChatChannel.Syntax, "Luck: Invalid upper limit: {0} (Limit must be >= 1)", UpperLimit);
		}
	}

	private void ChanceCommand(CPlayer a_Player, CVehicle a_Vehicle, uint PercentChanceSuccess)
	{
		if (PercentChanceSuccess > 100)
		{
			a_Player.PushChatMessage(EChatChannel.Syntax, "Chance: Invalid percentage chance of success: {0} (Percentages must be 0-100%)", PercentChanceSuccess);
		}

		int randNumber = new Random().Next(1, 100);
		bool bSuccess = randNumber <= PercentChanceSuccess;
		SendChanceCommandMessage(a_Player, "{0} used the chance command with {1}% chance of success. The outcome was: {2}", a_Player.GetCharacterName(ENameType.StaticCharacterName), PercentChanceSuccess, bSuccess ? "Success" : "Failure");
	}

	private void FlipCoinCommand(CPlayer a_Player, CVehicle a_Vehicle)
	{
		int randNumber = new Random().Next(1, 100);
		bool bIsHeads = randNumber <= 50;

		// Full coinflip sequence
		MainThreadTimerPool.CreateEntityTimer(CoinFlipCallback, 9200, this, 1, new object[] { this });
		a_Player.StartCoinFlip(true, false);


		void CoinFlipCallback(object[] a_Parameters)
		{
			int gAnimFlags = (int)(AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl | AnimationFlags.StopOnLastFrame);
			a_Player.AddAnimationToQueue(gAnimFlags, "anim@mp_player_intuppercoin_roll_and_toss", "exit", false, false, true, 3000, false);

			SendChanceCommandMessage(a_Player, "{0} flipped a coin. The outcome was {1}", a_Player.GetCharacterName(ENameType.StaticCharacterName), bIsHeads ? "Heads" : "Tails");
			a_Player.StartCoinFlip(false, false);
		}
	}

	private void CoinCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		// THIS IS TO ONLY SPIN THE COIN IN THE FINGERS.
		if (!SenderPlayer.StartedCoinFlip)
		{
			SenderPlayer.StartCoinFlip(true, true);
		}
		else
		{
			SenderPlayer.StartCoinFlip(false, false);
		}
	}

	private void SendChanceCommandMessage(CPlayer SenderPlayer, string strMessageFormat, params object[] objs)
	{
		string strMessage = Helpers.FormatString(strMessageFormat, objs);

		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

		foreach (var player in lstPlayers)
		{
			player.PushChatMessageWithColor(EChatChannel.Nearby, 196, 255, 255, strMessage);
		}
	}

	public void OnChatMessage(Player sender, string message)
	{
		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = message;

		WeakReference<CPlayer> SenderPlayerRef = PlayerPool.GetPlayerFromClient(sender);
		CPlayer SenderPlayer = SenderPlayerRef.Instance();

		if (SenderPlayer != null)
		{
			if (message.Contains("&#"))
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
				return;
			}

			List<CPlayer> lstOccupants = new List<CPlayer>();
			bool bSendToOccupantsOnly = false;

			if (SenderPlayer.IsInVehicleReal)
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(SenderPlayer.Client.Vehicle);

				if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
				{
					bSendToOccupantsOnly = true;
					lstOccupants = VehiclePool.GetVehicleOccupants(pVehicle);
				}
			}

			List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);
			Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerSay, null, message);

			foreach (var player in bSendToOccupantsOnly ? lstOccupants : lstPlayers)
			{
				if (player.IsInVehicleReal && !bSendToOccupantsOnly)
				{
					CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
					if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
					{
						continue;
					}
				}
				const int maxColorToDecrement = 128;
				int r = 255;
				int g = 255;
				int b = 255;

				float fDist = (SenderPlayer.Client.Position - player.Client.Position).Length();
				float fDistRatio = (fDist / ChatConstants.g_fDistance_Nearby);

				int colorDecrement = (int)(maxColorToDecrement * fDistRatio);
				r -= colorDecrement;
				g -= colorDecrement;
				b -= colorDecrement;

				(message, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, player);

				if (bTargetNeedsXpAward)
				{
					LanguageSystem.AwardXP(player, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
				}

				player.PushChatMessageWithPlayerNameAndPostfixAndColor(EChatChannel.Nearby, r, g, b, SenderPlayer.GetActiveLanguage(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), bSendToOccupantsOnly ? "((In Car)) says" : "says", "{0}", message);

				log.addAffected(player);
			}
			log.execute();

			if (bSenderNeedsXpAward)
			{
				LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
			}

			//Clear message
			m_CachedStrMessage = string.Empty;
		}
	}

	public void GetPos(CPlayer SenderPlayer)
	{
		CVehicle veh = null;
		if (SenderPlayer.IsInVehicleReal)
		{
			veh = VehiclePool.GetVehicleFromGTAInstance(SenderPlayer.Client.Vehicle);
		}

		PosCommand(SenderPlayer, veh);
	}

	public void PosCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.PushChatMessage(EChatChannel.AdminActions, "POS: {0}, {1}, {2} ROT: {3} DIMENSION: {4}", SenderPlayer.Client.Position.X, SenderPlayer.Client.Position.Y, SenderPlayer.Client.Position.Z, SenderPlayer.Client.Rotation.Z, SenderPlayer.Client.Dimension);

#if DEBUG
		PrintLogger.LogMessage(ELogSeverity.HIGH, "POS: {0}, {1}, {2} ROT: {3}, DIMENSION: {4}", SenderPlayer.Client.Position.X, SenderPlayer.Client.Position.Y, SenderPlayer.Client.Position.Z, SenderPlayer.Client.Rotation.Z, SenderPlayer.Client.Dimension);
#endif

		//System.IO.File.AppendAllText("plr_coords.txt", Helpers.FormatString("POS: {0}, {1}, {2} ROT: {3}\n", SenderPlayer.Client.Position.X, SenderPlayer.Client.Position.Y, SenderPlayer.Client.Position.Z, SenderPlayer.Client.Rotation.Z));
	}

	public void VehPosCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderPlayer.IsInVehicleReal)
		{
			SenderPlayer.PushChatMessage(EChatChannel.AdminActions, "VEH POS: {0}, {1}, {2} ROT: {3} DIMENSION: {4}", SenderPlayer.Client.Vehicle.Position.X, SenderPlayer.Client.Vehicle.Position.Y, SenderPlayer.Client.Vehicle.Position.Z, SenderPlayer.Client.Vehicle.Rotation.Z, SenderPlayer.Client.Vehicle.Dimension);

#if DEBUG
			PrintLogger.LogMessage(ELogSeverity.HIGH, "VEH POS: {0}, {1}, {2} ROT: {3}, DIMENSION: {4}", SenderPlayer.Client.Vehicle.Position.X, SenderPlayer.Client.Vehicle.Position.Y, SenderPlayer.Client.Vehicle.Position.Z, SenderPlayer.Client.Vehicle.Rotation.Z, SenderPlayer.Client.Vehicle.Dimension);
#endif

			//System.IO.File.AppendAllText("veh_coords.txt", Helpers.FormatString("POS: {0}, {1}, {2} ROT: {3}\n", SenderPlayer.Client.Vehicle.Position.X, SenderPlayer.Client.Vehicle.Position.Y, SenderPlayer.Client.Vehicle.Position.Z, SenderPlayer.Client.Vehicle.Rotation.Z));
		}
	}

	public void PMCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, string strMessage)
	{
		SendPrivateMessage(SenderPlayer, TargetPlayer, strMessage);
	}

	public void OnQuickReplyMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		CPlayer quickReplyTarget = SenderPlayer.GetData<CPlayer>(SenderPlayer.Client, EDataNames.QUICK_REPLY_PLAYER);

		if (quickReplyTarget == null)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 200, 200, 200, "No one is PM'ing you.");
			return;
		}

		SendPrivateMessage(SenderPlayer, quickReplyTarget, strMessage);
	}

	public void BigEarsCommand(CPlayer player, CVehicle vehicle, CPlayer other)
	{
		BigEars bigears = new BigEars()
		{
			admin = player,
			player = other
		};

		if (m_BigEars.Find(be => be == bigears) != null)
		{
			m_BigEars.Remove(bigears);
			player.SendNotification("Big Ears", ENotificationIcon.InfoSign, "Stopped listening to {0}.", other.GetCharacterName(ENameType.StaticCharacterName));
			new Logging.Log(player, Logging.ELogType.AdminCommand, new List<CBaseEntity> { other }, "/bigears stopped listening").execute();
			return;
		}

		m_BigEars.Add(bigears);
		player.SendNotification("Big Ears", ENotificationIcon.InfoSign, "Started listening to {0}.", other.GetCharacterName(ENameType.StaticCharacterName));
		new Logging.Log(player, Logging.ELogType.AdminCommand, new List<CBaseEntity> { other }, "/bigears started listening").execute();
	}

	private bool m_GlobalOocEnabled = false;

	public void TogOocCommand(CPlayer player, CVehicle vehicle)
	{
		m_GlobalOocEnabled = !m_GlobalOocEnabled;
		string action = m_GlobalOocEnabled ? "enabled" : "disabled";
		player.SendNotification("Global OOC", ENotificationIcon.Star, "Global OOC chat was {0}.", action);
		HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("{0} {1} {2} global OOC chat.", player.AdminTitle, player.Username, action), true, r: 255, g: 0, b: 0);
	}

	public void ToggleOocCommand(CPlayer player, CVehicle vehicle)
	{
		player.GlobalOocEnabled = !player.GlobalOocEnabled;
		string action = player.GlobalOocEnabled ? "enabled" : "disabled";
		player.SendNotification("Global OOC", ENotificationIcon.Star, "Global OOC chat was {0} for you.", action);
	}

	public void GlobalOocCommand(CPlayer player, CVehicle vehicle, string message)
	{
		if (message.Contains("&#"))
		{
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		string newMessage = EmojiManager.TryAndParseEmoji(message);

		if (!m_GlobalOocEnabled && !player.IsAdmin())
		{
			player.SendNotification("Global OOC", ENotificationIcon.ExclamationSign, "Global OOC is not enabled right now.");
			return;
		}

		Log log = new Log(player, ELogType.GlobalOOC, null, message);
		foreach (var other in PlayerPool.GetAllPlayers())
		{
			if (m_GlobalOocEnabled && !other.GlobalOocEnabled)
			{
				continue;
			}

			log.addAffected(other);
			other.PushChatMessageWithColor(EChatChannel.Global, 196, 255, 255, "(( {0} {1}: {2} ))", player.AdminTitle, player.GetCharacterName(ENameType.StaticCharacterName), newMessage);
		}
		log.execute();
	}

	private void ClearChatCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		NetworkEventSender.SendNetworkEvent_ClearChatbox(SenderPlayer);
	}

	private void SendPrivateMessage(CPlayer SenderPlayer, CPlayer TargetPlayer, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		string newMessage = EmojiManager.TryAndParseEmoji(strMessage);

		if (TargetPlayer == null || !TargetPlayer.IsLoggedIn)
		{
			// TODO_CHAT: Should be a notification
			SenderPlayer.PushChatMessage(EChatChannel.Syntax, "Player not found.");
			return;
		}

		if (m_PrivateMessagesDisabled.Find(p => p == TargetPlayer) != null && !SenderPlayer.IsAdmin() && !TargetPlayer.AdminDuty)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "Player has private messages disabled.");
			return;
		}

		if (SenderPlayer == TargetPlayer)
		{
			return;
		}

		if (!TargetPlayer.IsSpawned)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Player not found");
			return;
		}

		// TODO_CHAT: Better support for multi color, message below should be multi color
		SenderPlayer.PushChatMessageWithColor(EChatChannel.Global, 244, 232, 66, "PM Sent To [{1}] {0}: {2}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetPlayer.PlayerID, newMessage);
		TargetPlayer.PushChatMessageWithColor(EChatChannel.Global, 244, 232, 66, "PM From [{1}] {0}: {2}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.PlayerID, newMessage);

		List<CBaseEntity> affected = new List<CBaseEntity> { TargetPlayer };

		foreach (var bigear in m_BigEars)
		{
			if (bigear.player == SenderPlayer || bigear.player == TargetPlayer)
			{
				affected.Add(bigear.admin);
				bigear.admin.PushChatMessageWithColor(EChatChannel.Global, 234, 222, 56, "[BE] PM From [{0}] {1} to [{2}] {3}: {4}", SenderPlayer.PlayerID, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), TargetPlayer.PlayerID, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), newMessage);
			}
		}

		TargetPlayer.SetData(TargetPlayer.Client, EDataNames.QUICK_REPLY_PLAYER, SenderPlayer, EDataType.Unsynced);

		new Logging.Log(SenderPlayer, Logging.ELogType.PlayerPM, affected, strMessage).execute();
	}

	public void TogPMCommand(CPlayer player, CVehicle vehicle)
	{
		if (!player.IsAdmin() && !player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.TogPM))
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "Purchase the donator perk to toggle private messages!");
			return;
		}

		if (m_PrivateMessagesDisabled.Find(p => p == player) != null)
		{
			m_PrivateMessagesDisabled.Remove(player);
			player.PushChatMessageWithColor(EChatChannel.Notifications, 100, 255, 100, "Enabled private messages.");
			return;
		}

		m_PrivateMessagesDisabled.Add(player);
		player.PushChatMessageWithColor(EChatChannel.Notifications, 100, 255, 100, "Disabled private messages.");
	}

	public void TogAdsCommand(CPlayer player, CVehicle vehicle)
	{
		if (!player.IsAdmin() && !player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.TogAds))
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "Purchase the donator perk to toggle advertisements!");
			return;
		}

		if (m_AdvertisementsDisabled.Find(p => p == player) != null)
		{
			m_AdvertisementsDisabled.Remove(player);
			player.PushChatMessageWithColor(EChatChannel.Notifications, 100, 255, 100, "Enabled advertisements.");
			return;
		}

		m_AdvertisementsDisabled.Add(player);
		player.PushChatMessageWithColor(EChatChannel.Notifications, 100, 255, 100, "Disabled advertisements.");
	}

	// TODO_CHAT: reuse ids?
	int g_AdvertID = 0;
	Dictionary<int, Vector3> g_lstVecAdvertPositions = new Dictionary<int, Vector3>();

	public void AdGPSCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, int advertID)
	{
		if (g_lstVecAdvertPositions.ContainsKey(advertID))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 100, 255, 100, "Added GPS to your navigation. /adcleargps to reset.");
			NetworkEventSender.SendNetworkEvent_GPS_Set(SenderPlayer, g_lstVecAdvertPositions[advertID]);
		}
		else
		{
			SenderPlayer.SendNotification("Advertisement", ENotificationIcon.ExclamationSign, "This advertisement does not exist or does not have a location associated with it.");
		}
	}

	public void AdClearGPSCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		NetworkEventSender.SendNetworkEvent_GPS_Clear(SenderPlayer);
	}

	public void GPSCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, int zip)
	{
		CPropertyInstance property = PropertyPool.GetPropertyInstanceFromID(zip);
		if (property == null || property.Model.EntranceDimension != 0)
		{
			SenderPlayer.SendNotification("GPS", ENotificationIcon.ExclamationSign, "This ZIP code could not be found.");
			return;
		}

		SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 100, 255, 100, "Added GPS to your navigation. /cleargps to reset.");
		NetworkEventSender.SendNetworkEvent_GPS_Set(SenderPlayer, property.Model.EntrancePosition);
	}

	public void ClearGPSCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		NetworkEventSender.SendNetworkEvent_GPS_Clear(SenderPlayer);
	}

	public void AdCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, int phoneNumber, bool bShareLocation, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		if (SenderPlayer.LastAdvert > Helpers.GetUnixTimestamp(true) - (60 * 5))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "You can only place an advertisement once every 5 minutes.");
			return;
		}
		SenderPlayer.LastAdvert = Helpers.GetUnixTimestamp(true);

		if (!SenderPlayer.SubtractBankBalanceIfCanAfford(100.0f, PlayerMoneyModificationReason.Advert))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "You do not have $100 to pay for this advertisement.");
			return;
		}

		string strPhoneNumber = phoneNumber == -1 ? "Unknown" : phoneNumber.ToString();

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var pPlayer in players)
		{
			if (m_AdvertisementsDisabled.Find(p => p == pPlayer) != null && pPlayer != SenderPlayer)
			{
				continue;
			}
			if (pPlayer.IsAdmin(EAdminLevel.TrialAdmin, false))
			{
				if (bShareLocation)
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1} (( {2} )) (( use /adgps {3} to navigate to advert location ))", strMessage, strPhoneNumber, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), g_AdvertID);
				}
				else
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1} (( {2} ))", strMessage, strPhoneNumber, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
				}
			}
			else
			{
				if (bShareLocation)
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1}  (( use /adgps {2} to navigate to advert location ))", strMessage, strPhoneNumber, g_AdvertID);
				}
				else
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1}", strMessage, strPhoneNumber);
				}
			}
		}

		if (bShareLocation)
		{
			// strip @ because thats tagging in discord
			DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.Ads, "[AD] {0} - #{1} (( use /adgps {2} ingame to navigate to advert location ))", strMessage.Replace("@", " at "), strPhoneNumber, g_AdvertID);

			Vector3 vecPos = SenderPlayer.Client.Position;

			// Are we in an interior? Use the entrance marker as the position instead
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(SenderPlayer.Client.Dimension);

			if (propertyInst != null)
			{
				vecPos = propertyInst.Model.EntrancePosition;
			}

			g_lstVecAdvertPositions.Add(g_AdvertID, vecPos);
			++g_AdvertID;
		}
		else
		{
			DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.Ads, "[AD] {0} - #{1}", strMessage, strPhoneNumber);
		}
	}

	public void FactionAdCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, int factionSlot, int phoneNumber, bool bShareLocation, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}


		List<CFactionMembership> factionMemberships = SenderPlayer.GetFactionMemberships();
		CFaction faction = factionSlot < factionMemberships.Count ? factionMemberships[factionSlot].Faction : null;
		if (faction == null)
		{
			SenderPlayer.SendNotification("Faction Advertisement", ENotificationIcon.ExclamationSign, "You do not have a faction in that slot.");
			return;
		}

		if (!faction.SubtractMoney(100f))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "{0} does not have $100 to pay for this advertisement.", faction.Name);
			return;
		}

		new Log(SenderPlayer, ELogType.FactionAction, new List<CBaseEntity> { faction }, Helpers.FormatString("ADVERT PLACED - {0}", strMessage)).execute();

		string strPhoneNumber = phoneNumber == -1 ? "Unknown" : phoneNumber.ToString();

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var pPlayer in players)
		{
			if (m_AdvertisementsDisabled.Find(p => p == pPlayer) != null && pPlayer != SenderPlayer)
			{
				continue;
			}
			if (pPlayer.IsAdmin(EAdminLevel.TrialAdmin, false))
			{
				if (bShareLocation)
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1} (( {2} - {3} )) (( use /adgps {4} to navigate to advert location ))", strMessage, strPhoneNumber, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), faction.ShortName, g_AdvertID);
				}
				else
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1} (( {2} - {3} ))", strMessage, strPhoneNumber, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), faction.ShortName);
				}
			}
			else
			{
				if (bShareLocation)
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1}  (( use /adgps {2} to navigate to advert location ))", strMessage, strPhoneNumber, g_AdvertID);
				}
				else
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1}", strMessage, strPhoneNumber);
				}
			}
		}

		if (bShareLocation)
		{
			DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.Ads, "[AD] {0} - #{1} (( use /adgps {2} ingame to navigate to advert location ))", strMessage, strPhoneNumber, g_AdvertID);

			Vector3 vecPos = SenderPlayer.Client.Position;

			// Are we in an interior? Use the entrance marker as the position instead
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(SenderPlayer.Client.Dimension);

			if (propertyInst != null)
			{
				vecPos = propertyInst.Model.EntrancePosition;
			}

			g_lstVecAdvertPositions.Add(g_AdvertID, vecPos);
			++g_AdvertID;
		}
		else
		{
			DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.Ads, "[AD] {0} - #{1}", strMessage, strPhoneNumber);
		}
	}

	public void IDCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, string a_strPartialName)
	{
		List<CPlayer> lstPlayersFound = new List<CPlayer>();
		Dictionary<int, CPlayer> dictPlayerIDs = new Dictionary<int, CPlayer>();

		// TODO: Optimize this all, its probably one of the most used commands
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var pPlayer in players)
		{
			dictPlayerIDs[pPlayer.PlayerID] = pPlayer;
			if (pPlayer.DoesAnyCharacterNameMatchPartial(a_strPartialName))
			{
				lstPlayersFound.Add(pPlayer);
			}
		}

		if (lstPlayersFound.Count == 1)
		{
			CPlayer playerFound = lstPlayersFound[0];
			// TODO_CHAT: should this just output to one channel?
			// TODO_CHAT: Multi color below
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "ID {0}: {1}", playerFound.PlayerID, playerFound.GetCharacterName(ENameType.StaticCharacterName, SenderPlayer, true));
		}
		else if (lstPlayersFound.Count > 1)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "{0} Players Were Found:", lstPlayersFound.Count);
			foreach (CPlayer playerFound in lstPlayersFound)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "     ID {0}: {1}", playerFound.PlayerID, playerFound.GetCharacterName(ENameType.StaticCharacterName, SenderPlayer, true));
			}
		}
		else
		{
			// Try for ID instead
			bool bFound = false;
			if (int.TryParse(a_strPartialName, out int outID))
			{
				if (dictPlayerIDs.ContainsKey(outID))
				{
					bFound = true;
					CPlayer playerFromID = dictPlayerIDs[outID];
					SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "ID {0}: {1}", playerFromID.PlayerID, playerFromID.GetCharacterName(ENameType.StaticCharacterName, SenderPlayer, true));
				}
			}

			if (!bFound)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "No results found for '{0}'.", a_strPartialName);
			}
		}
	}

	public void OnMeMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		HelperFunctions.Chat.SendMeMessage(SenderPlayer, strMessage);
	}

	public void OnDoMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		HelperFunctions.Chat.SendDoMessage(SenderPlayer, strMessage);
	}

	// TODO: Better location
	public void SaveCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.Save();
	}

	public void KillCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.Client.Kill();
		new Logging.Log(SenderPlayer, Logging.ELogType.Death, null, Helpers.FormatString("/ks {0} commited suicide", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
	}

	public void KickSelf(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.KickFromServer(CPlayer.EKickReason.PLAYER_REQUESTED);
		new Logging.Log(SenderPlayer, Logging.ELogType.ConnectionEvents, null, Helpers.FormatString("/k {0} kicked himself", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
	}

	public void OnLocalOOCMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		string newMessage = EmojiManager.TryAndParseEmoji(strMessage);
		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

		Color OOCColor = HelperFunctions.Chat.GetAdminTagColor(SenderPlayer);
		Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerOOC, null, strMessage);
		foreach (var player in lstPlayers)
		{
			player.PushChatMessageWithColorAndPlayerName(EChatChannel.Nearby, OOCColor.Red, OOCColor.Green, OOCColor.Blue, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), "(( {0} ))", newMessage);
			log.addAffected(player);
		}
		log.execute();
	}

	public void GovtChat(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		if (SenderPlayer.IsInGovernmentFaction() || SenderPlayer.IsInFactionOfType(EFactionType.Towing))
		{
			CFaction factionToUse = null;
			List<CFactionMembership> lstPlayerFactionMemberships = SenderPlayer.GetFactionMemberships();
			foreach (var factionMembership in lstPlayerFactionMemberships)
			{
				if (factionMembership.Faction.Type == EFactionType.LawEnforcement || factionMembership.Faction.Type == EFactionType.Medical || factionMembership.Faction.Type == EFactionType.Government || factionMembership.Faction.Type == EFactionType.Towing)
				{
					factionToUse = factionMembership.Faction;
					break;
				}
			}

			if (factionToUse != null)
			{
				List<CPlayer> playersAlreadySent = new List<CPlayer>();

				// Send to all govt players
				List<CFaction> lstGovtFactions = FactionPool.GetGovernmentFactions();
				lstGovtFactions.AddRange(FactionPool.GetTowingFactions());
				foreach (var govtFaction in lstGovtFactions)
				{
					List<CPlayer> lstFactionMembers = govtFaction.GetMembers();

					foreach (var factionMember in lstFactionMembers)
					{
						if (!playersAlreadySent.Contains(factionMember))
						{
							(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, factionMember);
							playersAlreadySent.Add(factionMember);
							factionMember.PushChatMessageWithColor(EChatChannel.Factions, 126, 91, 242, "[GOVT] [{0}] [{3}] {1}: {2}", factionToUse.ShortName, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage, SenderPlayer.GetActiveLanguage().ToString());

							if (bTargetNeedsXpAward)
							{
								LanguageSystem.AwardXP(factionMember, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
							}
						}
					}
				}

				if (bSenderNeedsXpAward)
				{
					LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
				}
			}
		}
		else
		{
			SenderPlayer.SendNotification("Government Chat", ENotificationIcon.ExclamationSign, "You are not in a government faction");
		}
	}

	public void MegaphoneChat(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		bool bHasMegaphone = false;


		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.MEGAPHONE, 0);
		if (SenderPlayer.Inventory.HasItem(ItemInstanceDef, false, out CItemInstanceDef outItem))
		{
			bHasMegaphone = true;
		}
		else if (SenderVehicle != null && SenderVehicle.IsPoliceCar())
		{
			bHasMegaphone = true;
		}

		if (bHasMegaphone)
		{
			List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Megaphone);

			foreach (var player in lstPlayers)
			{
				(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, player);
				player.PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel.Nearby, 225, 214, 0, SenderPlayer.GetActiveLanguage(), "[MEGAPHONE]", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), "{0}!", strMessage);

				if (bTargetNeedsXpAward)
				{
					LanguageSystem.AwardXP(player, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
				}
			}

			if (bSenderNeedsXpAward)
			{
				LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
			}
		}
		else
		{
			SenderPlayer.SendNotification("Megaphone", ENotificationIcon.ExclamationSign, "You do not have a megaphone.");
		}

		m_CachedStrMessage = string.Empty;
	}

	public void Radios(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		int index = 0;
		bool foundRadios = false;

		SenderPlayer.PushChatMessageWithColor(EChatChannel.Nearby, 225, 194, 14, "~ RADIOS ~", String.Empty);

		foreach (var item in SenderPlayer.Inventory.GetAllItems())
		{
			if (item != null)
			{
				if (item.ItemID == EItemID.RADIO)
				{
					++index;
					foundRadios = true;

					CItemValueBasic itemVal = (CItemValueBasic)item.Value;

					if (itemVal.value < 0)
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Nearby, 225, 194, 14, "   Radio {0}: Turned off", index);
					}
					else
					{
						SenderPlayer.PushChatMessageWithColor(EChatChannel.Nearby, 225, 194, 14, "   Radio {0}: {1}", index, Convert.ToInt32(itemVal.value));
					}
				}
			}
		}

		if (!foundRadios)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Nearby, 225, 194, 14, "   You do not have any radios.", String.Empty);
		}
	}

	public void RadioChat(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		RadioChatInternal(SenderPlayer, 0, strMessage);
	}

	public void Radio2Chat(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		RadioChatInternal(SenderPlayer, 1, strMessage);
	}

	public void Radio3Chat(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		RadioChatInternal(SenderPlayer, 2, strMessage);
	}

	public void RadioChatLow(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		RadioChatInternal(SenderPlayer, 0, strMessage, true);
	}

	public void Radio2ChatLow(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		RadioChatInternal(SenderPlayer, 1, strMessage, true);
	}

	public void Radio3ChatLow(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		RadioChatInternal(SenderPlayer, 2, strMessage, true);
	}

	public void RadioChatInternal(CPlayer SenderPlayer, int index, string strMessage, bool bIsLow = false)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		CItemInstanceDef radioToUse = null;

		int currentFoundIndex = 0;
		foreach (var item in SenderPlayer.Inventory.GetAllItems())
		{
			if (item != null)
			{
				if (item.ItemID == EItemID.RADIO)
				{
					// Is it the one we're looking for?
					if (currentFoundIndex == index)
					{
						radioToUse = item;
						break;
					}
					else
					{
						++currentFoundIndex;
					}
				}
			}
		}

		if (radioToUse != null)
		{
			CItemValueBasic itemVal = (CItemValueBasic)radioToUse.Value;
			if (itemVal.value < 0)
			{
				SenderPlayer.SendNotification("Radio", ENotificationIcon.ExclamationSign, "This radio is turned off.");
			}
			else
			{
				int currentChannel = Convert.ToInt32(itemVal.value);
				if (strMessage == null || strMessage.Length == 0)
				{
					SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 225, 194, 14, "This radio is tuned to channel {0}. Type a message to communicate over it.", currentChannel);
				}
				else
				{
					List<CPlayer> lstOccupants = new List<CPlayer>();
					bool bSendToOccupantsOnly = false;

					if (SenderPlayer.IsInVehicleReal)
					{
						CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(SenderPlayer.Client.Vehicle);

						if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
						{
							bSendToOccupantsOnly = true;
							lstOccupants = VehiclePool.GetVehicleOccupants(pVehicle);
						}
					}
					// Get eligible players for nearby hearing, and send to them if they DONT have a radio tuned to that channel
					float fHearingRadiusForNearbyPlayers = bIsLow ? ChatConstants.g_fDistance_Closeby : ChatConstants.g_fDistance_Radio;
					List<CPlayer> lstPlayersEligibleForNearbyHearing = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, fHearingRadiusForNearbyPlayers);
					Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerRadio, null, strMessage);

					CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.RADIO, currentChannel);
					ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
					foreach (var player in players)
					{
						(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, player);
						if (player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem))
						{
							if (player == SenderPlayer)
							{
								player.PushChatMessageWithColor(EChatChannel.Nearby, 162, 207, 252, "[RADIO {0}: #{1}{4}] [{5}] {2}: {3}", index + 1, currentChannel, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage, bIsLow ? " ((Spoken Quietly))" : "", SenderPlayer.GetActiveLanguage().ToString());
							}
							else
							{
								player.PushChatMessageWithColor(EChatChannel.Nearby, 162, 207, 252, "[RADIO #{0}{3}] [{4}] {1}: {2}", currentChannel, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage, bIsLow ? " ((Spoken Quietly))" : "", SenderPlayer.GetActiveLanguage().ToString());
							}

							if (!log.characters.Contains(player.ActiveCharacterDatabaseID))
							{
								log.addAffected(player);
							}
						}
						else if (lstPlayersEligibleForNearbyHearing.Contains(player) && !bSendToOccupantsOnly) // SEND TO NEARBY PLAYERS ALSO
						{
							if (player.IsInVehicleReal)
							{
								CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
								if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
								{
									continue;
								}
							}

							// Nearby only gets the display name, not the real name
							player.PushChatMessageWithColor(EChatChannel.Nearby, 162, 207, 252, "[RADIO{2}] [{3}] {0}: {1}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage, bIsLow ? " ((Spoken Quietly))" : "", SenderPlayer.GetActiveLanguage().ToString());
							if (!log.characters.Contains(player.ActiveCharacterDatabaseID))
							{
								log.addAffected(player);
							}
						}
						else if (lstOccupants.Contains(player) && bSendToOccupantsOnly)
						{
							player.PushChatMessageWithColor(EChatChannel.Nearby, 162, 207, 252, "[RADIO{2}] [{3}] {0} ((In Car)): {1}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage, bIsLow ? " ((Spoken Quietly))" : "", SenderPlayer.GetActiveLanguage().ToString());
							if (!log.characters.Contains(player.ActiveCharacterDatabaseID))
							{
								log.addAffected(player);
							}
						}

						if (bTargetNeedsXpAward)
						{
							LanguageSystem.AwardXP(player, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
						}
					}

					if (bSenderNeedsXpAward)
					{
						LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
					}

					log.execute();
				}

			}
		}
		else
		{
			SenderPlayer.SendNotification("Radio", ENotificationIcon.ExclamationSign, "You do not have a radio in that slot.");
		}
	}

	public void FactionChat(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		FactionChatInternal(SenderPlayer, 0, strMessage);
	}

	public void FactionChat2(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 1, strMessage); }
	public void FactionChat3(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 2, strMessage); }
	public void FactionChat4(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 3, strMessage); }
	public void FactionChat5(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 4, strMessage); }
	public void FactionChat6(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 5, strMessage); }
	public void FactionChat7(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 6, strMessage); }
	public void FactionChat8(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 7, strMessage); }
	public void FactionChat9(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 8, strMessage); }
	public void FactionChat10(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage) { FactionChatInternal(SenderPlayer, 9, strMessage); }

	public void FactionChatInternal(CPlayer SenderPlayer, int factionIndex, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		string newMessage = EmojiManager.TryAndParseEmoji(strMessage);

		List<CFactionMembership> factionMemberships = SenderPlayer.GetFactionMemberships();

		if (factionIndex < factionMemberships.Count)
		{
			CFaction faction = factionMemberships[factionIndex].Faction;

			if (faction != null)
			{
				List<CPlayer> lstFactionMembers = faction.GetMembers();
				Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerFactionChat, null, strMessage);
				foreach (var factionMember in lstFactionMembers)
				{
					factionMember.PushChatMessageWithColor(EChatChannel.Factions, 3, 157, 157, "[{0}] {1}: {2}", faction.ShortName, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), newMessage);
					log.addAffected(factionMember);
				}
				log.execute();
			}
		}
		else
		{
			string strErrorMessage = factionIndex == 0 ? "You are not in a faction" : "You have no faction in that slot";
			SenderPlayer.SendNotification("Faction Chat", ENotificationIcon.ExclamationSign, strErrorMessage);
		}
	}

	public void OnClosebyMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		List<CPlayer> lstOccupants = new List<CPlayer>();
		bool bSendToOccupantsOnly = false;

		if (SenderPlayer.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(SenderPlayer.Client.Vehicle);

			if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
			{
				bSendToOccupantsOnly = true;
				lstOccupants = VehiclePool.GetVehicleOccupants(pVehicle);
			}
		}

		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Closeby);
		Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerWhisper, null, Helpers.FormatString(strMessage));

		foreach (var player in bSendToOccupantsOnly ? lstOccupants : lstPlayers)
		{
			if (player.IsInVehicleReal && !bSendToOccupantsOnly)
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
				if (pVehicle != null && !pVehicle.VehicleWindowState() && pVehicle.HasWindows())
				{
					continue;
				}
			}

			(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, player);
			player.PushChatMessage(EChatChannel.Nearby, "[{0}] {1} {3} speaks quietly: {2}", SenderPlayer.GetActiveLanguage().ToString(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage, bSendToOccupantsOnly ? "((In Car))" : "");
			log.addAffected(player);

			if (bTargetNeedsXpAward)
			{
				LanguageSystem.AwardXP(player, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
			}
		}
		log.execute();

		if (bSenderNeedsXpAward)
		{
			LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
		}

		m_CachedStrMessage = string.Empty;
	}

	public void OnWhisperCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		float fDist = (SenderPlayer.Client.Position - TargetPlayer.Client.Position).Length();

		if (SenderPlayer == TargetPlayer)
		{
			return;
		}

		if (fDist < 3.5f)
		{
			HelperFunctions.Chat.SendAmeMessage(SenderPlayer, Helpers.FormatString("whispers into the ear of {0}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)));

			(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, SenderPlayer);
			SenderPlayer.PushChatMessage(EChatChannel.Nearby, "[{0}] {1} Whispers: {2}", SenderPlayer.GetActiveLanguage().ToString(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage);

			(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, TargetPlayer);
			TargetPlayer.PushChatMessage(EChatChannel.Nearby, "[{0}] {1} Whispers: {2}", SenderPlayer.GetActiveLanguage().ToString(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage);

			new Logging.Log(SenderPlayer, Logging.ELogType.PlayerWhisper, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString(strMessage)).execute();

			if (bSenderNeedsXpAward)
			{
				LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
			}

			if (bTargetNeedsXpAward)
			{
				LanguageSystem.AwardXP(TargetPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
			}
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, Helpers.FormatString("{0} is too far away.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)));
		}

		m_CachedStrMessage = string.Empty;
	}

	public void OnShoutMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Shout);
		Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerShout, null, Helpers.FormatString(strMessage));
		foreach (var player in lstPlayers)
		{
			(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, player);
			player.PushChatMessageWithPlayerName(EChatChannel.Nearby, SenderPlayer.GetActiveLanguage(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName) + " shouts", strMessage + "!");
			log.addAffected(player);

			if (bTargetNeedsXpAward)
			{
				LanguageSystem.AwardXP(player, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
			}
		}
		log.execute();

		if (bSenderNeedsXpAward)
		{
			LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
		}

		m_CachedStrMessage = string.Empty;
	}

	public void OnShoutLoudMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Shout);
		Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerShout, null, strMessage);

		foreach (var player in lstPlayers)
		{
			(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, player);
			player.PushChatMessageWithPlayerName(EChatChannel.Nearby, SenderPlayer.GetActiveLanguage(), SenderPlayer.GetCharacterName(ENameType.StaticCharacterName) + " shouts loudly", strMessage + "!");
			log.addAffected(player);

			if (bTargetNeedsXpAward)
			{
				LanguageSystem.AwardXP(player, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
			}
		}
		log.execute();

		if (bTargetNeedsXpAward)
		{
			LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
		}

		m_CachedStrMessage = string.Empty;
	}

	public void OnDistrictMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_District);

		foreach (var player in lstPlayers)
		{
			player.PushChatMessage(EChatChannel.Nearby, "District IC: {0} (({1}))", strMessage, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
		}
	}

	public void OnAdminChatMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.AdminChat, null, Helpers.FormatString(strMessage));

		string newMessage = EmojiManager.TryAndParseEmoji(strMessage);

		foreach (var player in players)
		{
			if (player.IsAdmin())
			{
				player.PushChatMessageWithColorAndPlayerName(EChatChannel.AdminChat, 95, 244, 66, Helpers.FormatString("[ADMIN] {0} ({1})", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username), "{0}", newMessage);
				log.addAffected(player);
			}
		}
		log.execute();

		DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AdminChat, "[ADMIN IN GAME] {0} ({1}): {2}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, strMessage);
	}

	public void OnUatAdminChatMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (!SenderPlayer.IsAdmin(EAdminLevel.LeadAdmin))
		{
			return;
		}

		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.LeadAdminChat, null, Helpers.FormatString(strMessage));

		string newMessage = EmojiManager.TryAndParseEmoji(strMessage);

		foreach (var player in players)
		{
			if (player.IsAdmin(EAdminLevel.LeadAdmin))
			{
				player.PushChatMessageWithColorAndPlayerName(EChatChannel.AdminChat, 117, 214, 255, Helpers.FormatString("[UAT] {0} ({1})", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username), "{0}", newMessage);
				log.addAffected(player);
			}
		}
		log.execute();
	}

	public void OnAdminAnnouncement(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		string newMessage = EmojiManager.TryAndParseEmoji(strMessage);
		HelperFunctions.Chat.SendAdminAnnouncement(SenderPlayer, newMessage);
	}

	public void StatsCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		ShowPlayerStats(SenderPlayer, SenderPlayer);
	}

	public void AStatsCommand(CPlayer player, CVehicle vehicle, CPlayer other)
	{
		ShowPlayerStats(player, other);
	}

	private async void ShowPlayerStats(CPlayer player, CPlayer other)
	{
		CStatsResult characterStats = await Database.LegacyFunctions.GetCharacterStats(other.ActiveCharacterDatabaseID).ConfigureAwait(true);
		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "~~~~~~~~~~~{0}'s Stats~~~~~~~~~~~", other.GetCharacterName(ENameType.StaticCharacterName));

		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Hours Played (Account): {0}", Convert.ToUInt32(other.MinutesPlayed_Account / 60));
		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Hours Played (Character): {0}", Convert.ToUInt32(other.MinutesPlayed_Character / 60));
		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Job: {0}", other.GetJobName());
		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Money: ${0:0.00}", characterStats.money);
		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Bank Money: ${0:0.00}", characterStats.bank_money);

		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Vehicles:");

		if (characterStats.m_lstVehicles.Count == 0)
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 255, 255, "None");
		}

		foreach (SCharacterVehicleResult vehicle in characterStats.m_lstVehicles)
		{
			string strDisplayName = "Unknown";
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicle.model);

			if (vehicleDef != null)
			{
				strDisplayName = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);
			}

			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 255, 255, " - [{0}] {1}", vehicle.id, strDisplayName);
		}

		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Properties:");

		if (characterStats.m_lstProperties.Count == 0)
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 255, 255, "None");
		}

		foreach (SCharacterPropertyResult property in characterStats.m_lstProperties)
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 255, 255, " - [{0}] {1}", property.id, property.name);
		}

		var phones = PhoneSystem.GetPlayerPhones(other);
		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Phones in inventory:");
		if (phones != null)
		{
			int index = 0;
			foreach (var itemInstance in phones)
			{
				index++;
				var phone = (CItemValueCellphone)itemInstance.Value;
				player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 255, 255, $"#{index} - {phone.number}");
			}
		}

		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Type /jobstats to see job XP statistics.");
	}

	private void OnAmeMessage(CPlayer SourcePlayer, CVehicle SourceVehicle, string message)
	{
		if (message.Contains("&#"))
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		if (!string.IsNullOrEmpty(message))
		{
			HelperFunctions.Chat.SendAmeMessage(SourcePlayer, message);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 231, 217, 176, Helpers.FormatString("*{1} {0}*", message, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName)));
		}
	}

	private void OnAdoMessage(CPlayer SourcePlayer, CVehicle SourceVehicle, string message)
	{
		if (message.Contains("&#"))
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		if (!string.IsNullOrEmpty(message))
		{
			HelperFunctions.Chat.SendAdoMessage(SourcePlayer, message);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 231, 217, 176, Helpers.FormatString("*{0} (({1}))*", message, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName)));
		}
	}

	private void OnStatusMessage(CPlayer SourcePlayer, CVehicle SourceVehicle, string message)
	{
		if (message.Contains("&#"))
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		if (!string.IsNullOrEmpty(message))
		{
			HelperFunctions.Chat.SendStatusMessage(SourcePlayer, message);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 231, 217, 176, Helpers.FormatString("*{0}*", message));
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 136, 87, 201, Helpers.FormatString("Status was set! Use /delstatus to remove it."));
		}
	}

	private void OnClearStatusMessage(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		string statusMessage = SourcePlayer.GetData<string>(SourcePlayer.Client, EDataNames.STATUS_MESSAGE);
		if (!string.IsNullOrEmpty(statusMessage))
		{
			HelperFunctions.Chat.ClearStatusMessage(SourcePlayer);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 136, 87, 201, Helpers.FormatString("Status was removed! Use /status to add another status."));
		}
	}

	private void OnAirMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Contains("&#"))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
			return;
		}

		bool bSenderNeedsXpAward = false;
		bool bTargetNeedsXpAward = false;
		string m_CachedStrMessage = strMessage;

		CFaction airportFaction = FactionPool.GetFactionFromShortName("CAA");
		if ((SenderVehicle == null || !SenderVehicle.IsAircraft()) && (airportFaction == null || !SenderPlayer.IsInFaction(airportFaction.FactionID)))
		{
			return;
		}

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerSay, null, Helpers.FormatString(strMessage));
		foreach (CPlayer player in players)
		{
			if (player.IsInAircraft() || player.IsInFaction(airportFaction.FactionID))
			{
				(strMessage, bSenderNeedsXpAward, bTargetNeedsXpAward) = LanguageSystem.ApplyLanguageLogic(m_CachedStrMessage, SenderPlayer, player);
				player.PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel.Factions, 0, 132, 255, SenderPlayer.GetActiveLanguage(), "[AIR]", Helpers.FormatString("{0}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName)), "{0}", strMessage);
				log.addAffected(player);

				if (bTargetNeedsXpAward)
				{
					LanguageSystem.AwardXP(player, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
				}
			}
		}
		log.execute();

		if (bSenderNeedsXpAward)
		{
			LanguageSystem.AwardXP(SenderPlayer, SenderPlayer.GetActiveLanguage(), LANGUAGE_XP);
		}

		m_CachedStrMessage = string.Empty;
	}

	// TODO_WEATHERINFO_COMMAND: pseudo data? real data? what should we do here? For now just tell 
	private void WeatherInfoCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 0, 255, 0, Helpers.FormatString("Current weather: {0}", HelperFunctions.World.GetCurrentWeather()));
	}
}