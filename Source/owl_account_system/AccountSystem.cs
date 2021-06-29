using Database.Functions;
using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EntityDatabaseID = System.Int64;

public class AccountSystem
{
	private static int g_PlayerPeak = -1;

	private static int g_CurrentHour = 0;
	private static int g_CurrentHourPeak = 0;

	private AssetTransferSystem m_AssetTransferSystem = new AssetTransferSystem();

	public AccountSystem()
	{
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
		NetworkEvents.OnPlayerDisconnected += OnPlayerDisconnected;

		// player stats trackers
		g_PlayerPeak = Database.LegacyFunctions.GetGlobalPeakPlayerCount().Result;

		// Load tattoo data
		try
		{
			PrintLogger.LogMessage(ELogSeverity.HIGH, "AccountSystem: Deserializing Tattoos");

			CTattooDefinition[] jsonData = JsonConvert.DeserializeObject<CTattooDefinition[]>(
				System.IO.File.ReadAllText(System.IO.Path.Combine("dotnet", "resources", "owl_gamedata",
					"TattooData.json")));

			foreach (CTattooDefinition tattooDef in jsonData)
			{
				TattooDefinitions.g_TattooDefinitions.Add(tattooDef.ID, tattooDef);
			}

			PrintLogger.LogMessage(ELogSeverity.HIGH, "Deserialized {0} tattoos.",
				TattooDefinitions.g_TattooDefinitions.Count);
		}
		catch (Exception ex)
		{
			PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading tattoo data: {0}", ex.ToString());
		}

		// Load hair tattoo data
		try
		{
			PrintLogger.LogMessage(ELogSeverity.HIGH, "AccountSystem: Deserializing Hair Tattoos");

			CHairTattooDefinition[] jsonData = JsonConvert.DeserializeObject<CHairTattooDefinition[]>(
				System.IO.File.ReadAllText(System.IO.Path.Combine("dotnet", "resources", "owl_gamedata",
					"HairTattooData.json")));

			foreach (CHairTattooDefinition hairTattooDef in jsonData)
			{
				TattooDefinitions.g_HairTattooDefinitions.Add(hairTattooDef.ID, hairTattooDef);
			}

			PrintLogger.LogMessage(ELogSeverity.HIGH, "Deserialized {0} hair tattoos.",
				TattooDefinitions.g_HairTattooDefinitions.Count);
		}
		catch (Exception ex)
		{
			PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading hair tattoo data: {0}", ex.ToString());
		}

		NetworkEvents.CharacterSelectedLocal += CharacterSelected_Automatic;
		NetworkEvents.CharacterSelected += (CPlayer player, long CharacterID) =>
		{
			CharacterSelected(player, CharacterID, null);
		};

		NetworkEvents.SpawnSelected += SpawnSelected;
		NetworkEvents.PreviewCharacter += PreviewCharacter;
		NetworkEvents.LoginPlayer += LoginPlayer;
		NetworkEvents.FinishTutorialState += FinishTutorialState;
		NetworkEvents.RegisterPlayer += RegisterPlayer;
		NetworkEvents.RequestLogout += LogoutPlayer;
		NetworkEvents.CreateCharacterPremade += CreateCharacterPremade;
		NetworkEvents.CreateCharacterCustom += CreateCharacterCustom;
		NetworkEvents.QuizComplete += QuizComplete;
		NetworkEvents.RequestWrittenQuestions += RequestWrittenQuestions;
		NetworkEvents.RequestQuizQuestions += RequestQuizQuestions;
		NetworkEvents.SubmitWrittenPortion += SubmitWrittenPortion;
		NetworkEvents.RequestChangeCharacter += RequestChangeCharacter;
		NetworkEvents.RequestTutorialState += OnRequestTutorialState;
		NetworkEvents.ToggleNametags += OnToggleNametags;
		NetworkEvents.ToggleLocalPlayerNametag += OnToggleLocalNametag;

		NetworkEvents.ForceReSelectCharacter += CharacterSelected_Automatic;

		NetworkEvents.SetAutoSpawnCharacter += OnSetAutoSpawnCharacter;
		NetworkEvents.UpdateCharacterLook += UpdateCharacterLook;

		RageEvents.RAGE_OnUpdate += OnUpdate;

		g_CurrentHour = Core.GetServerClock().Hour;

		CommandManager.RegisterCommand("look", "Look at another player to read about their physical appearance.",
			new Action<CPlayer, CVehicle, CPlayer>(LookCommand), CommandParsingFlags.TargetPlayer,
			CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("setlook", "Create or update a look page for your character.",
			new Action<CPlayer, CVehicle>(CreateLookCommand), CommandParsingFlags.Default,
			CommandRequirementsFlags.Default);
	}

	public void CreateLookCommand(CPlayer player, CVehicle _)
	{
		Look.Find(player.ActiveCharacterDatabaseID, look =>
		{
			NetworkEventSender.SendNetworkEvent_ShowUpdateCharacterLookUI(
				player,
				player.ActiveCharacterDatabaseID,
				player.GetCharacterName(ENameType.StaticCharacterName),
				look?.Height ?? 0,
				look?.Weight ?? 0,
				look?.PhysicalAppearance ?? "",
				look?.Scars ?? "",
				look?.Tattoos ?? "",
				look?.Makeup ?? "",
				look?.CreatedAt ?? 0,
				look?.UpdatedAt ?? 0
			);
		});
	}

	public void LookCommand(CPlayer player, CVehicle _, CPlayer target)
	{
		Look.Find(target.ActiveCharacterDatabaseID, look =>
		{
			if (look == null)
			{
				player.SendNotification("Character Look", ENotificationIcon.ExclamationSign,
					"No character look could be found for {0}.",
					target.GetCharacterName(ENameType.StaticCharacterName));
				return;
			}

			NetworkEventSender.SendNetworkEvent_ShowCharacterLook(
				player,
				look.CharacterID,
				target.GetCharacterName(ENameType.StaticCharacterName),
				Convert.ToInt32(target.Age),
				look.Height,
				look.Weight,
				look.PhysicalAppearance,
				look.Scars,
				look.Tattoos,
				look.Makeup,
				look.CreatedAt,
				look.UpdatedAt
			);
		});
	}

	public void UpdateCharacterLook(CPlayer player, int height, int weight, string physicalAppearance, string scars, string tattoos, string makeup)
	{
		Look.Find(player.ActiveCharacterDatabaseID, look =>
		{
			if (look == null)
			{
				Look.Create(player.ActiveCharacterDatabaseID, height, weight, physicalAppearance, scars, tattoos, makeup,
					() =>
					{
						player.SendNotification("Character Look", ENotificationIcon.InfoSign, "Your character look was updated.");
						LookCommand(player, null, player);
					});
				return;
			}

			Look.Update(player.ActiveCharacterDatabaseID, height, weight, physicalAppearance, scars, tattoos, makeup, () =>
			{
				player.SendNotification("Character Look", ENotificationIcon.InfoSign, "Your character look was updated.");
				LookCommand(player, null, player);
			});
		});
	}

	private async void OnUpdate()
	{
		// Check for overall player peak
		int numPlayers = NAPI.Pools.GetAllPlayers().Count;
		if (numPlayers > g_PlayerPeak)
		{
			DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.PeakAlerts, "New Player Peak! Previous: {0} New: {1}", g_PlayerPeak, numPlayers);

			g_PlayerPeak = numPlayers;
			await Database.LegacyFunctions.UpdateGlobalPeakPlayerCount(numPlayers).ConfigureAwait(true);
		}

		// Check for hourly peak
		DateTime serverClock = Core.GetServerClock();
		int currentHour = Core.GetServerClock().Hour;
		if (currentHour != g_CurrentHour) // hour changed, reset our peak, and tell everyone what it was
		{
			DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.PlayerCounts, "Player Peak for {0}/{1}/{2} Hour {3} (Server Time) was: {4}", serverClock.Month, serverClock.Day, serverClock.Year, g_CurrentHour, g_CurrentHourPeak);
			g_CurrentHourPeak = 0;
			g_CurrentHour = currentHour;
		}

		// Has our hourly peak been broken?
		if (numPlayers > g_CurrentHourPeak)
		{
			g_CurrentHourPeak = numPlayers;
		}
	}

	public void OnPlayerLoadedLowPrio(CPlayer player)
	{
		// TODO: Nothing to do here, maybe dont let them proceed to game?
	}

	public void OnToggleNametags(CPlayer player, bool isHidden)
	{
		player.SetData(player.Client, EDataNames.NAMETAGS, isHidden, EDataType.Synced);
	}

	private async void OnToggleLocalNametag(CPlayer player)
	{
		await player.ToggleLocalNametag().ConfigureAwait(true);
	}

	public void OnRequestTutorialState(CPlayer player, ETutorialVersions currentTutorialVersion)
	{
		StartTutorialForPlayer(player, currentTutorialVersion);
	}

	private void GotoLogin(CPlayer player, bool bShowGUI)
	{
		player.Freeze(true);
		GotoLoginDimension(player);
		NetworkEventSender.SendNetworkEvent_GotoLogin(player, bShowGUI);
	}

	class WhitelistItem
	{
#pragma warning disable 0649
		public string Serial;
		public string Name;
		public string AddedBy;
#pragma warning restore 0649
	}

	public void OnPlayerConnected(CPlayer player)
	{
		player.SetCharacterSkin(PedHash.Pug);
		player.SetVisible(false);

		player.SetCharacterNameOnConnect();

		// Do we have a whitelist?
		// TODO_LAUNCH: Comment this out, its kinda expensive IO (plus we won't have a whitelist :))
		if (File.Exists("whitelist.json"))
		{
			List<WhitelistItem> allowedSerials = JsonConvert.DeserializeObject<List<WhitelistItem>>(File.ReadAllText("whitelist.json"));


			bool bSerialFound = false;

			foreach (WhitelistItem serialData in allowedSerials)
			{
				if (serialData.Serial.ToLower() == player.GetSmallSerial().ToLower())
				{
					bSerialFound = true;
				}
			}

			if (!bSerialFound)
			{
				player.KickFromServer(CPlayer.EKickReason.SERIAL_NOT_AUTHORIZED);
				return;
			}
		}

		// Does the person have CEF enabled? If not kick
		if (!player.Client.IsCeFenabled)
		{
			player.KickFromServer(CPlayer.EKickReason.NO_CEF);
			return;
		}

		if (!player.IsLoggedIn)
		{
			GotoLogin(player, false);

			// TODO: Clean up dupe sessions when creating a new one
			Database.Functions.Accounts.AttemptAutoLogin(player.Client.Address, player.GetBigSerial(), async (CLoginResult LoginResult) =>
			{
				await OnLoginResult(player, LoginResult).ConfigureAwait(true);

				// If this failed, we need to give them the initial login screen
				if (LoginResult.m_Result == Auth.ELoginResult.Failed || LoginResult.m_Result == Auth.ELoginResult.AccountDoesNotExist || LoginResult.m_Result == Auth.ELoginResult.NotActivated)
				{
					GotoLogin(player, true);
				}
			});
		}
	}

	public void OnPlayerDisconnected(CPlayer player, DisconnectionType type, string reason)
	{
		// Cleanup any tutorial questions
		// TODO: Maybe store this on the player object in the future?
		if (m_PendingQuizQuestions.ContainsKey(player.PlayerID))
		{
			m_PendingQuizQuestions.Remove(player.PlayerID);
		}
	}

	private void GotoLoginDimension(CPlayer player)
	{
		NAPI.Entity.SetEntityTransparency(player.Client, 255);
		player.GotoPlayerSpecificDimension();
	}

	private async Task OnLoginResult(CPlayer player, CLoginResult LoginResult)
	{
		// Is the account banned?
		AccountBanDetails banDetails = await Database.LegacyFunctions.CheckForAccountAndDeviceBan(LoginResult.m_UserID, player.GetBigSerial(), player.Client.Address).ConfigureAwait(true);

		if (banDetails.IsBanned)
		{
			player.KickFromServer(CPlayer.EKickReason.ADMIN_BANNED, Helpers.FormatString("You are banned {0} for '{1}'.", banDetails.Until.Length == 0 ? "permanently" : Helpers.FormatString("until {0}", banDetails.Until), banDetails.GetDisplayReason()));
		}
		else
		{
			bool bSuccessful = LoginResult.m_Result == Auth.ELoginResult.Success;
			string strErrorMessage = "";

			if (!bSuccessful)
			{
				if (LoginResult.m_Result == Auth.ELoginResult.AccountDoesNotExist || LoginResult.m_Result == Auth.ELoginResult.Failed)
				{
					strErrorMessage = "Verify your credentials";
				}
				else if (LoginResult.m_Result == Auth.ELoginResult.NotActivated)
				{
					strErrorMessage = "Your email has not been validated<br>Please check your email for an activation link.";
				}
			}

			NetworkEventSender.SendNetworkEvent_LoginResult(player, bSuccessful, LoginResult.m_UserID, PlayfabWebAPI.g_TitleID, LoginResult.m_Username, strErrorMessage);

			// If successful, get characters
			if (bSuccessful)
			{
				// Update last login
				Database.LegacyFunctions.UpdateLastLogin(LoginResult.m_UserID);

				// Load chat settings (if present)
				ChatSettings chatSettings = await Database.LegacyFunctions.GetChatSettings(LoginResult.m_UserID).ConfigureAwait(true);
				if (chatSettings != null)
				{
					NetworkEventSender.SendNetworkEvent_ApplyRemoteChatSettings(player, chatSettings);
				}
				else
				{
					// apply defaults, but don't save
					NetworkEventSender.SendNetworkEvent_LoadDefaultChatSettings(player);
				}

				// Send controls
				// We send this always, if length zero, client restores defaults
				NetworkEventSender.SendNetworkEvent_ApplyCustomControls(player, LoginResult.m_lstControls);

				// Load Achievements
				Dictionary<EAchievementID, CAchievementInstance> dictAchievements = await Database.LegacyFunctions.GetPlayerAchievements(LoginResult.m_UserID).ConfigureAwait(true);
				player.CopyAchievements(dictAchievements);

				player.SetCharacterNameOnLogin();

				// Kick any duplicates
				WeakReference<CPlayer> dupePlayerRef = PlayerPool.GetPlayerFromAccountID_IncludeOutOfGame(LoginResult.m_UserID);
				CPlayer dupePlayer = dupePlayerRef.Instance();

				// We can't have more than one dupe
				if (dupePlayer != null && dupePlayer != player)
				{
					dupePlayer.KickFromServer(CPlayer.EKickReason.LOGGED_IN_ELSEWHERE);
				}

				player.SetLoggedIn(LoginResult.m_UserID, true, LoginResult.m_AdminLevel, LoginResult.m_ScripterLevel, LoginResult.m_Username, LoginResult.appState, LoginResult.numApps, LoginResult.discordID, LoginResult.autoSpawnCharacter, LoginResult.adminReportCount, LoginResult.localPlayerNametagToggled);
				player.SetAdminJailData(LoginResult.adminJailMinutesRemaining, LoginResult.adminJailReason);
				// TODO_POST_LAUNCH: On logout, can we just delete and re-create CPlayer?

				// custom anims
				player.LoadCustomAnimations();

				// Copy donation inventory
				player.DonationInventory.CopyInventory(LoginResult.m_lstDonationInventory);

				player.MinutesPlayed_Account = LoginResult.minutesPlayed;

				Logging.Log log = new Logging.Log(player, Logging.ELogType.ConnectionEvents)
				{
					content = Helpers.FormatString("Player logged in - IP: {0} Serial: {1}.", LoginResult.ip, LoginResult.serial)
				};
				log.execute();

				// Have we finished the tutorial?
				TutorialCheckResult tutorialResult = await Database.LegacyFunctions.HasAccountFinishedTutorial(LoginResult.m_UserID).ConfigureAwait(true);
				if (tutorialResult.Result != ETutorialCheckResult.NotComplete)
				{
					if (tutorialResult.Result == ETutorialCheckResult.CompletedLatestVersion)
					{
						GotoLoginDimension(player);
						player.HandleApplicationStateAndTransmitCharacters(true);
					}
					else
					{
						NetworkEventSender.SendNetworkEvent_OfferNewTutorial(player, tutorialResult.Version);
					}
				}
				else
				{
					// Switch back to StartTutorialForPlayer if you want to enforce tutorial on sign up.
					NetworkEventSender.SendNetworkEvent_OfferNewTutorial(player, tutorialResult.Version);
					// StartTutorialForPlayer(player, ETutorialVersions.None);
				}
			}
			else
			{
				player.SetLoggedIn();
			}
		}
	}

	private void StartTutorialForPlayer(CPlayer player, ETutorialVersions currentTutorialVersion)
	{
		player.GotoNonPlayerSpecificDimension();
		NAPI.Entity.SetEntityTransparency(player.Client, 0);
		NetworkEventSender.SendNetworkEvent_GotoTutorialState(player, currentTutorialVersion);
	}

	public void LoginPlayer(CPlayer player, string strUsername, string strPassword, bool bAutoLogin)
	{
		Database.Functions.Accounts.LoginAccount(strUsername, strPassword, player.Client.Address, bAutoLogin, player.GetBigSerial(), async (CLoginResult LoginResult) =>
		{
			await OnLoginResult(player, LoginResult).ConfigureAwait(true);
		});
	}

	public void LogoutPlayer(CPlayer a_Player)
	{
		// destroy saved session/autologin
		a_Player.Logout();

		GotoLogin(a_Player, true);
	}

	public async void RegisterPlayer(CPlayer a_Player, string strUsername, string strPassword, string strPasswordVerify, string strEmail)
	{
		RegisterAPIResponse result = new RegisterAPIResponse();
		bool serialInUse = await Database.LegacyFunctions.SerialInUse(a_Player.GetBigSerial()).ConfigureAwait(true);

		if (strUsername.Length < 3)
		{
			result.success = false;
			result.error = "Username Too Short";
		}
		else if (strPassword != strPasswordVerify)
		{
			result.success = false;
			result.error = "Passwords Don't Match";
		}
		else if (serialInUse)
		{
			result.success = false;
			result.error = "Serial in use";
		}
		else
		{
#if !DEBUG
		try
		{
			System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
			Dictionary<string, string> values = new Dictionary<string, string>
			{
			{ "username", strUsername },
			{ "email", strEmail },
			{ "password", strPassword },
			{ "ip", a_Player.Client.Address }
			};

			var content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(values), System.Text.Encoding.UTF8, "application/json");

				var response = await client.PostAsync(new Uri("http://ucp:8000/api/register/"), content).ConfigureAwait(true);
				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

			// extra logging for daniels only
			if (strUsername.ToLower().Contains("danielsregtest"))
			{
				a_Player.SendNotification("Register Debug", ENotificationIcon.InfoSign, "Hello Daniels, your registration has been logged!");
				Sentry.SentrySdk.WithScope(scope =>
				{
					var usr = new Sentry.Protocol.User();
					usr.Username = "Server";

					Sentry.SentryEvent logEvent = new Sentry.SentryEvent();

					logEvent.Message = Helpers.FormatString("REGISTER ATTEMPT\r\nREQ:{0}\r\nRESP:{1}", Newtonsoft.Json.JsonConvert.SerializeObject(values), responseString);
					logEvent.User = usr;
					logEvent.ServerName = "Server";
					Sentry.BaseScopeExtensions.SetTag(scope, "debugregister", "true");

					Sentry.SentrySdk.CaptureEvent(logEvent);
				});
			}


				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					RegisterAPIResponse responseDeserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterAPIResponse>(responseString);
					OnRegisterResult(a_Player, responseDeserialized);
					return;
			}
			else
			{
				result.success = false;
				result.error = "The server is experiencing errors (Code: 1)";
			}
		}
		catch
		{
			// result might have become garbled in due to the error/exception, so make a new one
			result = new RegisterAPIResponse();
			result.success = false;
			result.error = "The server is experiencing errors (Code: 2)";
			OnRegisterResult(a_Player, result);
		}
	}

		OnRegisterResult(a_Player, result);
#else
			result = await Database.LegacyFunctions.RegisterAccount(strUsername, strPassword, strEmail, a_Player.GetBigSerial()).ConfigureAwait(true);
			OnRegisterResult(a_Player, result);

			return;
		}
#endif
	}

	public void QuizComplete(CPlayer player, List<int> lstResponseIndexes)
	{
		if (m_PendingQuizQuestions.ContainsKey(player.PlayerID))
		{
			CQuizQuestion[] questionsForPlayer = m_PendingQuizQuestions[player.PlayerID];

			int numCorrect = 0;
			int numIncorrect = 0;

			// Does the count match?
			if (questionsForPlayer.Length == lstResponseIndexes.Count)
			{
				int index = 0;
				foreach (CQuizQuestion question in questionsForPlayer)
				{
					if (question.CorrectAnswerIndex == lstResponseIndexes[index])
					{
						numCorrect++;
					}
					else
					{
						numIncorrect++;
					}

					index++;
				}

				// Clean our stored questions
				m_PendingQuizQuestions.Remove(player.PlayerID);

				bool bPassed = (numCorrect >= g_NumQuestionsToPass);

				// if they passed, move them onto the next state (record that they finished this part of the quiz)
				if (bPassed)
				{
					player.SetApplicationState(EApplicationState.QuizCompleted);
					// TODO_APPLICATIONS: Re-enable this if we want to skip approvals.
					// await player.SetApplicationState(EApplicationState.ApplicationApproved).ConfigureAwait(true);

					// Discord
					DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AdminCommands, "Account '{0}' just passed the quiz and had their account application auto-approved!", player.AccountID);
				}

				NetworkEventSender.SendNetworkEvent_QuizResults(player, bPassed, numCorrect, numIncorrect);
			}
			else
			{
				throw new Exception(Helpers.FormatString("FATAL ERROR - QuizComplete: questionsForPlayer.Length == lstResponseIndexes.Count IS FALSE. {0} == {1}", questionsForPlayer.Length, lstResponseIndexes.Count));
			}

		}
	}

	private const int g_NumQuestions = 5;
	private const int g_NumQuestionsToPass = 4;
	private Dictionary<int, CQuizQuestion[]> m_PendingQuizQuestions = new Dictionary<int, CQuizQuestion[]>();

	public void RequestQuizQuestions(CPlayer player)
	{
		// Lets create a list of all possible questions
		CQuizQuestion[] PossibleQuestions = QuizQuestionDefinitions.g_QuizQuestionDefinitions.ToArray();

		// now randomize the order and take the first N questions (where N = NumQuestions)
		Random rng = new Random();
		PossibleQuestions = PossibleQuestions.OrderBy(x => rng.Next()).Take(g_NumQuestions).ToArray();

		m_PendingQuizQuestions[player.PlayerID] = PossibleQuestions;

		// TODO_CSHARP: Got to to list because arrays get deserialized as individual parameters so 5 instead of 1... fix this
		NetworkEventSender.SendNetworkEvent_GotQuizQuestions(player, PossibleQuestions.ToList());
	}

	private const int g_NumWrittenQuestions = 4; // NOTE: Must update the UI to match this if you change it and SubmitWrittenPortion
	private Dictionary<int, CQuizWrittenQuestion[]> m_PendingQuizWrittenQuestions = new Dictionary<int, CQuizWrittenQuestion[]>();

	public void RequestWrittenQuestions(CPlayer player)
	{
		// Lets create a list of all possible questions
		CQuizWrittenQuestion[] PossibleWrittenQuestions = QuizWrittenQuestionDefinitions.g_QuizQuestionDefinitions.Where(question => question.IsActive).ToArray();

		// now randomize the order and take the first N questions (where N = NumQuestions)
		Random rng = new Random();
		PossibleWrittenQuestions = PossibleWrittenQuestions.OrderBy(x => rng.Next()).Take(g_NumWrittenQuestions).ToArray();

		m_PendingQuizWrittenQuestions[player.PlayerID] = PossibleWrittenQuestions;

		// TODO_CSHARP: Got to to list because arrays get deserialized as individual parameters so 5 instead of 1... fix this
		NetworkEventSender.SendNetworkEvent_GotQuizWrittenQuestions(player, PossibleWrittenQuestions.ToList());
	}

	public void SubmitWrittenPortion(CPlayer player, string strQ1Answer, string strQ2Answer, string strQ3Answer, string strQ4Answer)
	{
		if (m_PendingQuizWrittenQuestions.ContainsKey(player.PlayerID))
		{
			CQuizWrittenQuestion[] questionsForPlayer = m_PendingQuizWrittenQuestions[player.PlayerID];

			// Clean our stored questions
			m_PendingQuizWrittenQuestions.Remove(player.PlayerID);

			// Update their progression
			player.SetApplicationState(EApplicationState.ApplicationPendingReview);

			// Store the questions and answers
			Database.Functions.Accounts.SetApplicationQuestionsAndAnswers(player.AccountID, questionsForPlayer[0].Index, questionsForPlayer[1].Index, questionsForPlayer[2].Index, questionsForPlayer[3].Index, strQ1Answer, strQ2Answer, strQ3Answer, strQ4Answer);

			// Increment the number of applications
			player.NumberOfApplicationsSubmitted++;
			Database.Functions.Accounts.SetNumberOfApplications(player.AccountID, player.NumberOfApplicationsSubmitted);

			// Inform admins
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[APPLICATION] New Application received from '{0}' (/apps to review!)", player.Username), true, EAdminLevel.TrialAdmin, 200, 200, 0);

			// Discord
			DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.AdminCommands, "[APPLICATION] New Application received from '{0}' (!apps to review!)", player.Username);
		}
	}

	private void OnRegisterResult(CPlayer player, RegisterAPIResponse RegisterResponse)
	{
		NAPI.Task.Run(() =>
		{
			bool bSuccessful = RegisterResponse.success;

			if (bSuccessful)
			{
				Database.Functions.Accounts.CreateGameAccount(RegisterResponse.account, player.GetBigSerial(), () =>
				{
					NetworkEventSender.SendNetworkEvent_RegisterResult(player, bSuccessful, RegisterResponse.error, RegisterResponse.errors);

					// Removed autologin as the user first has to activate/validate their email
				});
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_RegisterResult(player, bSuccessful, RegisterResponse.error, RegisterResponse.errors);
			}
		});
	}

	public void RequestChangeCharacter(CPlayer player)
	{
		player.OnCharacterChangeRequested();

		NetworkEventSender.SendNetworkEvent_ChangeCharacterApproved(player);
		player.HandleApplicationStateAndTransmitCharacters(false);

		player.GotoPlayerSpecificDimension();
	}

	public void FinishTutorialState(CPlayer player)
	{
		// Has to be logged in but not ingame for tutorial
		if (player.IsLoggedIn)
		{
			// Save
			GotoLoginDimension(player);
			Database.Functions.Accounts.SetTutorialCompleted(player.AccountID);
			player.HandleApplicationStateAndTransmitCharacters(false);
		}
	}

	private async void OnSetAutoSpawnCharacter(CPlayer player, Int64 CharacterID)
	{
		player.AutoSpawnCharacter = CharacterID;
		await Database.LegacyFunctions.SetAutoSpawnCharacter(player.AccountID, CharacterID).ConfigureAwait(true);
	}

	public async void SpawnSelected(CPlayer player, EScriptLocation location, long CharacterID)
	{
		// Clear spawn flag
		await Database.LegacyFunctions.ConsumeShowSpawnSelector(CharacterID).ConfigureAwait(true);

		if (location == EScriptLocation.Paleto)
		{
			// Do nothing, they're already in paleto because it's a legacy character
			CharacterSelected(player, CharacterID, null);
		}
		else
		{
			// Set spawn position
			await Database.LegacyFunctions.SetPlayerSpawn(CharacterID, CharacterConstants.SpawnPosition_LS, CharacterConstants.SpawnRotation_LS, 0).ConfigureAwait(true);

			// Give them an extra token
			// vehicle token
			DonationPurchasable propertyToken = null;
			foreach (var purchasable in Donations.g_lstPurchasables)
			{
				if (purchasable.DonationEffect == EDonationEffect.PropertyToken)
				{
					propertyToken = purchasable;
				}
			}

			if (propertyToken != null)
			{
				// NOTE: We set the active char dbid here, its needed to purchase successfully... hacky
				player.ActiveCharacterDatabaseID = CharacterID;
				await player.DonationInventory.OnPurchaseScripted(propertyToken, true).ConfigureAwait(true);
			}

			// Spawn
			CharacterSelected(player, CharacterID, null);
		}
	}

	// NOTE: If you change this, ensure the 'change character type' flow (plastic surgeon) still works
	public void CharacterSelected_Automatic(CPlayer player, long CharacterID)
	{
		Database.Functions.Characters.Get(player.AccountID, CharacterID, true, (SGetCharacter GetCharacterResult) =>
		{
			if (!GetCharacterResult.CKed)
			{
				// some stuff to do first that is normally done by preview char :(
				ApplySharedCharacterLogic(player, CharacterID, GetCharacterResult);
				NetworkEventSender.SendNetworkEvent_PreviewCharacterGotData(player);
				CharacterSelected(player, CharacterID, GetCharacterResult);
			}
			else
			{
				//send to char select otherwise spawns in as zombie :(
				player.ResetAutoSpawnFlag();
				player.SendToCharSelect();
			}
		});
	}

	private void ApplySharedCharacterLogic(CPlayer player, long CharacterID, SGetCharacter GetCharacterResult)
	{
		// TODO: use ground coord + freeze + invincible
		player.GotoPlayerSpecificDimension();
		// TODO_POST_LAUNCH: What do we wanna do about objects etc? Previewing an interior in a player specific dimension won't match the REAL interior. E.g. gates wont be visible in FD
		// Solution to the above would be to make those things maps, like an interior map

		// Fake a property enter if inside a property. This causes the client to load the IPL, the map, etc.
		player.HandleRestoreInterior(true, true, GetCharacterResult.Dimension);

		// Inventory for clothing, plus we need gender and type
		player.Inventory.CopyInventory(GetCharacterResult.Inventory);
		player.Gender = GetCharacterResult.Gender;
		player.CharacterType = GetCharacterResult.CharacterType;

		NAPI.Entity.SetEntityPosition(player.Client, GetCharacterResult.pos);
		NAPI.Entity.SetEntityRotation(player.Client, new Vector3(0.0f, 0.0f, GetCharacterResult.rz));
		player.SetSpawnFix(GetCharacterResult.pos, new Vector3(0.0f, 0.0f, GetCharacterResult.rz), player.Client.Dimension, GetCharacterResult.health, GetCharacterResult.armor);

		// Spawn player again and set player health. Without this autospawn kills player OnSpawn.
		NAPI.Player.SpawnPlayer(player.Client, GetCharacterResult.pos, GetCharacterResult.rz);


		// Apply skin data
		if (GetCharacterResult.CKed)
		{
			CustomCharacterSkinData CustomSkinData = new CustomCharacterSkinData();
			player.SetCharacterSkin(PedHash.Zombie01);
			player.AddAnimationToQueue((int)AnimationFlags.Loop, "special_ped@zombie@base", "base", false, true, true, 0, false);
		}
		else
		{
			CustomCharacterSkinData CustomSkinData = new CustomCharacterSkinData();
			List<int> lstTattoos = new List<int>();
			if (player.CharacterType == ECharacterType.Custom)
			{
				CustomSkinData = Database.LegacyFunctions.GetCharacterCustomData(CharacterID).Result;
				lstTattoos = Database.LegacyFunctions.GetCharacterTattooData(CharacterID).Result;
			}

			// If the player was on duty, show their duty skin, but don't actually put them on duty since that does a bunch of other things, this is purely visual
			if (GetCharacterResult.Duty != EDutyType.None)
			{
				player.SetSkinDataFromDB(CustomSkinData, lstTattoos);
				player.ApplyDutySkin(GetCharacterResult.Duty);
			}
			else
			{
				player.SetSkinDataFromDB(CustomSkinData, lstTattoos);
			}

			player.StopCurrentAnimation(true, true);
		}

		// TODO_CSHARP: Could optimize by not sending local player?
		NetworkEventSender.SendNetworkEvent_ApplyCustomSkinData(player, player.Client);

		NAPI.Player.SetPlayerHealth(player.Client, 100);
	}

	private async void CharacterSelected_Apply(CPlayer player, long CharacterID, SGetCharacter RetrieveCharacterResult)
	{
		if (!RetrieveCharacterResult.CKed)
		{
			if (RetrieveCharacterResult.ShowSpawnSelector)
			{
				NetworkEventSender.SendNetworkEvent_ShowSpawnSelector(player);
			}
			else
			{
				player.SetDiscordStatus(Helpers.FormatString("Playing As {0}", RetrieveCharacterResult.CharacterName));

				player.DonationInventory.AttemptToFixLosSantosPropertyTokens(CharacterID);

				player.OnCharacterPreSpawned(CharacterID);

				player.Client.Health = RetrieveCharacterResult.health;
				player.Client.Armor = RetrieveCharacterResult.armor;

				player.SetCharacterNameOnSpawn(RetrieveCharacterResult.CharacterName);

				player.OnCharacterChange_SetInitialMoney(RetrieveCharacterResult.money, RetrieveCharacterResult.bank_money, false);
				player.PendingJobMoney = RetrieveCharacterResult.pending_job_money;
				player.PaydayProgress = RetrieveCharacterResult.payday_progress;
				player.Inventory.CopyInventory(RetrieveCharacterResult.Inventory);
				player.Gender = RetrieveCharacterResult.Gender;
				player.CharacterType = RetrieveCharacterResult.CharacterType;
				player.Age = RetrieveCharacterResult.Age;
				player.RestoreCuffStatusFromDB(RetrieveCharacterResult.Cuffed, RetrieveCharacterResult.Cuffer);

				player.SetPremadeMasked(RetrieveCharacterResult.PremadeMasked, false);

				player.GangTag = RetrieveCharacterResult.gangTags;
				player.GangTagWIP = RetrieveCharacterResult.gangTagsWIP;

				player.MinutesPlayed_Character = RetrieveCharacterResult.minutesPlayed;

				bool bAlreadyHasActiveLanguage = false;
				foreach (var characterLanguageDB in RetrieveCharacterResult.CharacterLanguages)
				{
					bool bIsActive = characterLanguageDB.Active;

					// FIX: We had a bug where new chars would have two active languages, so if we find 2, take the first as active only
					if (!bAlreadyHasActiveLanguage && bIsActive)
					{
						bAlreadyHasActiveLanguage = true;
					}
					else if (bAlreadyHasActiveLanguage && bIsActive)
					{
						bIsActive = false;
					}

					CCharacterLanguage characterLanguage = new CCharacterLanguage(characterLanguageDB.languageID, bIsActive, characterLanguageDB.Progress);
					player.AddLanguageForPlayer(characterLanguage, false);
				}

				foreach (var factionMembershipDB in RetrieveCharacterResult.FactionMemberships)
				{
					CFaction factionInst = FactionPool.GetFactionFromID(factionMembershipDB.factionID);
					if (factionInst != null)
					{
						CFactionMembership factionMembership = new CFactionMembership(factionInst, factionMembershipDB.Manager, factionMembershipDB.Rank);
						player.AddFactionMembership(factionMembership, false);
					}
				}

				// Apply duty (MUST come before loading inventory or it wont be applied)
				// NOTE: If player is no longer in the faction, this function will do nothing other than reset duty in db to none
				CItemInstanceDef activeDutyOutfit = player.Inventory.GetActiveDutyOutfitOfType(RetrieveCharacterResult.Duty);
				player.GoOnDuty(RetrieveCharacterResult.Duty, activeDutyOutfit, true);

				// Apply skin data
				CustomCharacterSkinData CustomSkinData = new CustomCharacterSkinData();
				List<int> lstTattoos = new List<int>();
				if (player.CharacterType == ECharacterType.Custom)
				{
					CustomSkinData = Database.LegacyFunctions.GetCharacterCustomData(CharacterID).Result;
					lstTattoos = Database.LegacyFunctions.GetCharacterTattooData(CharacterID).Result;
				}

				player.SetSkinDataFromDB(CustomSkinData, lstTattoos);

				NAPI.Entity.SetEntityPosition(player.Client, RetrieveCharacterResult.pos);
				NAPI.Entity.SetEntityRotation(player.Client, new Vector3(0.0f, 0.0f, RetrieveCharacterResult.rz));
				player.SetSpawnFix(RetrieveCharacterResult.pos, new Vector3(0.0f, 0.0f, RetrieveCharacterResult.rz), RetrieveCharacterResult.Dimension, RetrieveCharacterResult.health, RetrieveCharacterResult.armor);

				player.Freeze(false);

				// Keybinds
				player.Keybinds = RetrieveCharacterResult.Keybinds;
				NetworkEventSender.SendNetworkEvent_ApplyPlayerKeybinds(player, RetrieveCharacterResult.Keybinds);

				NetworkEventSender.SendNetworkEvent_CharacterSelectionApproved(player);

				await Database.LegacyFunctions.UpdateCharacterLastSeen(CharacterID).ConfigureAwait(true);

				// NOTE: Must be after we set spawn pos
				player.RestoreJailStatusFromDB(RetrieveCharacterResult.Dimension, RetrieveCharacterResult.UnjailTime, RetrieveCharacterResult.CellNumber, RetrieveCharacterResult.BailAmount, RetrieveCharacterResult.JailReason);

				// NOTE: Do this here so the skin gets overridden
				player.Job = RetrieveCharacterResult.Job;
				player.TruckerJobXP = RetrieveCharacterResult.TruckerJobXP;
				player.DeliveryDriverJobXP = RetrieveCharacterResult.DeliveryDriverJobXP;
				player.BusDriverJobXP = RetrieveCharacterResult.BusDriverJobXP;
				player.MailmanJobXP = RetrieveCharacterResult.MailmanJobXP;
				player.TrashmanJobXP = RetrieveCharacterResult.TrashmanJobXP;
				player.FishingXP = RetrieveCharacterResult.FishingXP;

				uint jailDimension = player.ApplyAdminJailOnCharacterSelect();

				player.OnCharacterSpawned(CharacterID, jailDimension == 0 ? RetrieveCharacterResult.Dimension : jailDimension);
				player.IsSpawned = true;



				player.PushChatMessage(EChatChannel.Notifications, "You are now playing as '{0}'{1}.", player.GetCharacterName(ENameType.StaticCharacterName), player.IsMasked() ? " (Masked)" : "");
				new Logging.Log(player, Logging.ELogType.ConnectionEvents, null, Helpers.FormatString("{0} spawned as character {1}", player.Username, player.GetCharacterName(ENameType.StaticCharacterName))).execute();

				// APPS NOTIFICATION
				if (player.IsAdmin())
				{
					Database.Functions.Accounts.GetPendingApplications((List<PendingApplication> lstPendingApps) =>
					{
						player.PushChatMessage(EChatChannel.Notifications, "[ADMIN - Server Applications] There are {0} pending application(s)!", lstPendingApps.Count);
					});
				}

				NetworkEventSender.SendNetworkEvent_ApplyCustomSkinData_ForAll_IncludeEveryone(player.Client);

				// load pending faction invites
				List<PendingFactionInvite> lstInvites = await Database.LegacyFunctions.GetCharacterPendingFactionInvites(CharacterID).ConfigureAwait(true);
				foreach (var factionInvite in lstInvites)
				{
					CFaction factionInst = FactionPool.GetFactionFromID(factionInvite.FactionID);

					if (factionInst != null)
					{
						Database.Functions.Characters.GetCharacterNameFromDBID(factionInvite.SourceCharacter, (string strInvitingCharacterName) =>
						{
							NetworkEventSender.SendNetworkEvent_ReceivedFactionInvite(player, factionInst.Name, strInvitingCharacterName, factionInst.m_DatabaseID);
						});
					}
				}

				// NOTE: Do not set dimension here, set it on a timer in OnCharacterSpawned (RAGE bug)

				// Fake a property enter if inside a property. This causes the client to load the IPL, the map, furniture, etc.
				player.HandleRestoreInterior(true, false);

				// Apply drug and impairment effects
				player.ReapplyDrugAndImpairmentFromDB(RetrieveCharacterResult);

				/////////////////////////////////////
				// START FIRST USE CODE
				/////////////////////////////////////
				if (RetrieveCharacterResult.FirstUse)
				{
					bool bIsPaleto = player.Client.Position.Y >= Constants.BorderOfLStoPaleto;

					await Database.LegacyFunctions.ConsumeFirstUse(CharacterID).ConfigureAwait(true);
					// Give first time things (we do this here rather than create so if they create and don't play, they still get the latest goodies + we can show notification)

					// Give vehicle + property token

					// vehicle token
					DonationPurchasable vehicleToken = null;
					DonationPurchasable propertyToken = null;
					foreach (var purchasable in Donations.g_lstPurchasables)
					{
						if (purchasable.DonationEffect == EDonationEffect.VehicleToken)
						{
							vehicleToken = purchasable;
						}
						else if (purchasable.DonationEffect == EDonationEffect.PropertyToken)
						{
							propertyToken = purchasable;
						}
					}

					if (vehicleToken != null)
					{
						await player.DonationInventory.OnPurchaseScripted(vehicleToken, true).ConfigureAwait(true);
					}

					if (propertyToken != null)
					{
						await player.DonationInventory.OnPurchaseScripted(propertyToken, true).ConfigureAwait(true);
					}

					// money
					player.OnCharacterChange_SetInitialMoney(CharacterConstants.StartingMoney, CharacterConstants.StartingBankMoney, true);

					player.SendNotification(bIsPaleto ? "Welcome To Paleto Bay" : "Welcome To Los Santos", ENotificationIcon.InfoSign, "Your new character has been given:<br><br>1 Vehicle Token (Good for any 'Token Vehicle' vehicle)<br>1 Property Token (Good for properties up to $50,000 value)<br>${0:0.00} on hand<br>${1:0.00} bank balance", CharacterConstants.StartingMoney, CharacterConstants.StartingBankMoney);
				}
				/////////////////////////////////////
				// END FIRST USE CODE
				/////////////////////////////////////
				///

				player.HandlePendingWeaponLicenseStates(RetrieveCharacterResult.pendingFirearmsLicenseStateTier1, RetrieveCharacterResult.pendingFirearmsLicenseStateTier2);
				HandleReturnedTokens(player);
				player.Notifications.LoadAll();

				player.RestorePet();


				// HANDLE OLD CHARACTER VERSIONS / UPGRADE THEM
				if (RetrieveCharacterResult.CurrentVersion < CharacterConstants.LatestCharacterVersion)
				{
					ECharacterVersions versionToSet = CharacterConstants.LatestCharacterVersion;

					// handle each case here
					// Do nothing, but don't upgrade them to latest so we can give them stuff!
					/*
					if (RetrieveCharacterResult.CurrentVersion <= ECharacterVersions.Beta_And_1_0_HoldingPattern) // Old character create, so they get free stuff!
					{
						versionToSet = ECharacterVersions.Beta_And_1_0_HoldingPattern;
					}
					*/

					if (RetrieveCharacterResult.CurrentVersion <= ECharacterVersions.Beta_And_1_0_GiveTattooToken) // Old character create, so they get free stuff!
					{
						// give tokens
						DonationPurchasable tattooArtistToken = null;
						foreach (var purchasable in Donations.g_lstPurchasables)
						{
							if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Tattoo)
							{
								tattooArtistToken = purchasable;
								break;
							}
						}
						if (tattooArtistToken != null) { await player.DonationInventory.OnPurchaseScripted(tattooArtistToken, true).ConfigureAwait(true); }

						player.SendNotification("Legacy Character", ENotificationIcon.InfoSign, "Your character was created prior to Owl V 1.1. You have been granted:<br><br>1 free visit to the Tattoo Artist");
						versionToSet = ECharacterVersions.Beta_And_1_0_GiveBarberToken;

						// NOTE: We dont set latest version here, but an interim one so we can track that they were legacy, but were given the above tokens. We do this because we'll be granting more tokens soon to legacy chars (e.g. plastic surgeon), which are not yet usable in this update
					}

					// UPDATE WHICH ADDS BARBER
					if (RetrieveCharacterResult.CurrentVersion <= ECharacterVersions.Beta_And_1_0_GiveBarberToken) // Old character create, so they get free stuff!
					{
						// Hold them here until we have tokens for them
						DonationPurchasable barbersToken = null;

						foreach (var purchasable in Donations.g_lstPurchasables)
						{
							if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Barber)
							{
								barbersToken = purchasable;
								break;
							}
						}
						if (barbersToken != null) { await player.DonationInventory.OnPurchaseScripted(barbersToken, true).ConfigureAwait(true); }
						player.SendNotification("Legacy Character", ENotificationIcon.InfoSign, "Your character was created prior to Owl V 1.1. You have been granted:<br><br>1 free visit to the Barbers");
						versionToSet = ECharacterVersions.Beta_And_1_0_GiveClothingStoreToken;
					}

					// UPDATE WHICH ADDS NEW CLOTHING
					if (RetrieveCharacterResult.CurrentVersion <= ECharacterVersions.Beta_And_1_0_GiveClothingStoreToken) // Old character create, so they get free stuff!
					{
						DonationPurchasable clothingStoreToken = null;

						foreach (var purchasable in Donations.g_lstPurchasables)
						{
							if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Clothing_Store)
							{
								clothingStoreToken = purchasable;
								break;
							}
						}

						if (clothingStoreToken != null) { await player.DonationInventory.OnPurchaseScripted(clothingStoreToken, true).ConfigureAwait(true); }
						player.SendNotification("Legacy Character", ENotificationIcon.InfoSign, "Your character was created prior to Owl V 1.1. You have been granted:<br><br>1 free visit to the Clothing Store");
						versionToSet = ECharacterVersions.Beta_And_1_0_GivePlasticSurgeon;
					}

					// UPDATE WHICH ADDS PLASTIC SURGEON
					if (RetrieveCharacterResult.CurrentVersion <= ECharacterVersions.Beta_And_1_0_GivePlasticSurgeon) // Old character create, so they get free stuff!
					{
						DonationPurchasable plasticSurgeonToken = null;

						foreach (var purchasable in Donations.g_lstPurchasables)
						{
							if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Plastic_Surgeon)
							{
								plasticSurgeonToken = purchasable;
								break;
							}
						}

						if (plasticSurgeonToken != null) { await player.DonationInventory.OnPurchaseScripted(plasticSurgeonToken, true).ConfigureAwait(true); }
						player.SendNotification("Legacy Character", ENotificationIcon.InfoSign, "Your character was created prior to Owl V 1.1. You have been granted:<br><br>1 free visit to the Plastic Surgeon");
						versionToSet = ECharacterVersions.VERSION_1_1_NEW_CHAR_CREATE;
					}

					// UPDATE WHICH ADDS HAIR TATTOOS (Free visit to barber)
					if (RetrieveCharacterResult.CurrentVersion < ECharacterVersions.ADDED_HAIR_TATTOOS) // Old character create, so they get free stuff!
					{
						// Hold them here until we have tokens for them
						DonationPurchasable barbersToken = null;

						foreach (var purchasable in Donations.g_lstPurchasables)
						{
							if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Barber)
							{
								barbersToken = purchasable;
								break;
							}
						}
						if (barbersToken != null) { await player.DonationInventory.OnPurchaseScripted(barbersToken, true).ConfigureAwait(true); }
						player.SendNotification("Legacy Character", ENotificationIcon.InfoSign, "Your character was created prior to Base Hair / Hair Tattoos being added. You have been granted:<br><br>1 free visit to the Barbers");
						versionToSet = ECharacterVersions.ADDED_HAIR_TATTOOS;
					}

					// handle other versions here
					{

					}

					// Set to latest version
					await Database.LegacyFunctions.SetCharacterCurrentVersionToLatest(CharacterID, versionToSet).ConfigureAwait(true);
				}
			}
		}
		else
		{
			// Do not let them spawn
		}
	}

	public void CharacterSelected(CPlayer player, long CharacterID, SGetCharacter RetrieveCharacterResult)
	{
		if (player.IsLoggedIn)
		{
			// TODO_LAUNCH: Verify character belongs to account
			if (RetrieveCharacterResult == null)
			{
				Database.Functions.Characters.Get(player.AccountID, CharacterID, true, (SGetCharacter RetrieveCharacterResult) =>
				{
					CharacterSelected_Apply(player, CharacterID, RetrieveCharacterResult);
				});
			}
			else
			{
				CharacterSelected_Apply(player, CharacterID, RetrieveCharacterResult);
			}
		}
	}

	public async void HandleReturnedTokens(CPlayer player)
	{
		bool bHasVehicleToken = player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.VehicleToken);
		int vehiclesCount = await Database.LegacyFunctions.GetCharacterVehiclesCount(player.ActiveCharacterDatabaseID).ConfigureAwait(true);
		bool bOwnsVehicles = vehiclesCount > 0;
		bool bGiveVehicleToken = !bHasVehicleToken && !bOwnsVehicles;

		bool bHasPropertyToken = player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.VehicleToken);
		int propertiesCount = await Database.LegacyFunctions.GetCharacterPropertiesCount(player.ActiveCharacterDatabaseID).ConfigureAwait(true);
		bool bOwnsProperties = propertiesCount > 0;
		bool bGivePropertyToken = !bHasPropertyToken && !bOwnsProperties;

		if (!bGiveVehicleToken && !bGivePropertyToken)
		{
			return;
		}

		DonationPurchasable vehicleToken = null;
		DonationPurchasable propertyToken = null;
		foreach (var purchasable in Donations.g_lstPurchasables)
		{
			if (purchasable.DonationEffect == EDonationEffect.VehicleToken)
			{
				vehicleToken = purchasable;
			}
			else if (purchasable.DonationEffect == EDonationEffect.PropertyToken)
			{
				propertyToken = purchasable;
			}
		}

		if (vehicleToken != null && bGiveVehicleToken)
		{
			await player.DonationInventory.OnPurchaseScripted(vehicleToken, true).ConfigureAwait(true);
		}

		if (propertyToken != null && bGivePropertyToken)
		{
			await player.DonationInventory.OnPurchaseScripted(propertyToken, true).ConfigureAwait(true);
		}
	}


	public void PreviewCharacter(CPlayer player, long CharacterID)
	{
		if (player.IsLoggedIn)
		{
			// TODO_LAUNCH: Verify character belongs to account
			Database.Functions.Characters.Get(player.AccountID, CharacterID, false, (SGetCharacter GetCharacterResult) =>
			{
				ApplySharedCharacterLogic(player, CharacterID, GetCharacterResult);

				// TODO: Shared event, not its own					
				NetworkEventSender.SendNetworkEvent_PreviewCharacterGotData(player);
			});
		}
	}

	public void CreateCharacterPremade(CPlayer player, EScriptLocation spawn, EGender gender, string strName, uint SkinHash, int Age, int[] Drawables, int[] Textures, Dictionary<ECustomPropSlot, int> PropsDrawables, Dictionary<ECustomPropSlot, int> PropsTextures, ECharacterLanguage primaryLanguage, ECharacterLanguage secondaryLanguage = ECharacterLanguage.None)
	{
		// Is the name valid?
		bool bNameValid = IsValidCharacterName(strName);

		if (bNameValid)
		{
			// Is the name taken?
			Database.Functions.Characters.IsNameUnique(strName, async (bool bIsNameUnique) =>
			{
				if (bIsNameUnique)
				{
					// Check languages if selected none
					bool bNotZeroLanguages = DoWeNotHaveZeroLanguages(primaryLanguage);
					if (bNotZeroLanguages)
					{
						//  Check languages if we have duplicates
						bool bUniqueLanguages = DoWeHaveTwoUniqueLanguages(primaryLanguage, secondaryLanguage);
						if (bUniqueLanguages)
						{
							Vector3 vecSpawnPos = spawn == EScriptLocation.Paleto ? CharacterConstants.SpawnPosition_Paleto : CharacterConstants.SpawnPosition_LS;
							float fSpawnRot = spawn == EScriptLocation.Paleto ? CharacterConstants.SpawnRotation_Paleto : CharacterConstants.SpawnRotation_LS;

							ECharacterSource source = ECharacterSource.CreatedOnOwl;
							EntityDatabaseID characterID = await Database.LegacyFunctions.CreateCharacterPremade(vecSpawnPos, fSpawnRot, gender, strName, SkinHash, Age, player.AccountID, source).ConfigureAwait(true);

							// Give the player their clothing
							CItemValueClothingPremade clothingValue = new CItemValueClothingPremade((PedHash)SkinHash, true);
							CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.CLOTHES, clothingValue);
							player.Inventory.AddClothingItemToSocketForcefully(ItemInstanceDef, EItemSocket.Clothing, characterID);

							void OnFinish()
							{
								NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Success);
								player.HandleApplicationStateAndTransmitCharacters(false);

								player.AwardAchievement(EAchievementID.PremadeCharacter);
							}

							// Always grant first language if we get here.
							Database.Functions.Characters.AddLanguage(characterID, primaryLanguage, 100f, true, (ulong insertID) =>
							{
								// Only grant second language if it's not none
								if (secondaryLanguage != ECharacterLanguage.None)
								{
									Database.Functions.Characters.AddLanguage(characterID, secondaryLanguage, 100f, false, (ulong insertID) =>
									{
										OnFinish();
									});
								}
								else
								{
									OnFinish();
								}
							});
						}
						else
						{
							NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_SameLanguage);
						}
					}
					else
					{
						NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_NoLanguage);
					}
				}
				else
				{
					NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_NameTaken);
				}
			});
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_NameInvalid);
		}
	}

	public void CreateCharacterCustom(CPlayer player, EScriptLocation spawn, EGender gender, string strName, uint SkinHash, int Age, int[] Drawables, int[] Textures, Dictionary<ECustomPropSlot, int> DrawablesProps, Dictionary<ECustomPropSlot, int> TexturesProps, int Ageing,
											float AgeingOpacity, int Makeup, float MakeupOpacity, int MakeupColor, int MakeupColorHighlight, int Blush, float BlushOpacity, int BlushColor, int BlushColorHighlight,
											int Complexion, float ComplexionOpacity, int SunDamage, float SunDamageOpacity, int Lipstick, float LipstickOpacity, int LipstickColor,
											int LipstickColorHighlights, int MolesAndFreckles, float MolesAndFrecklesOpacity, float NoseSizeHorizontal, float NoseSizeVertical, float NoseSizeOutwards, float NoseSizeOutwardsUpper,
											float NoseSizeOutwardsLower, float NoseAngle, float EyebrowHeight, float EyebrowDepth, float CheekboneHeight, float CheekWidth, float CheekWidthLower, float EyeSize, float LipSize,
											float MouthSize, float MouthSizeLower, float ChinSize, float ChinSizeLower, float ChinWidth, float ChinEffect, float NeckWidth, float NeckWidthLower, int FaceBlend1Mother,
											int FaceBlend1Father, float FaceBlendFatherPercent, float SkinBlendFatherPercent, int BaseHair, int HairColor, int HairColorHighlights, int EyeColor, int FacialHairStyle,
											int FacialHairColor, int FacialHairColorHighlight, float FacialHairOpacity, int Blemishes, float BlemishesOpacity, int Eyebrows, float EyebrowsOpacity, int EyebrowsColor, int EyebrowsColorHighlight, List<int> lstTattooIDs,
											int BodyBlemishes, float BodyBlemishesOpacity, int ChestHair, int ChestHairColor, int ChestHairColorHighlights, float ChestHairOpacity, ECharacterLanguage primaryLanguage, ECharacterLanguage secondaryLanguage = ECharacterLanguage.None)
	{
		if (player != null && player.IsLoggedIn)
		{
			// Is the name valid?
			bool bNameValid = IsValidCharacterName(strName);

			if (bNameValid)
			{
				// Is the name taken?
				Database.Functions.Characters.IsNameUnique(strName, async (bool bIsNameUnique) =>
				{
					if (bIsNameUnique)
					{
						// Check languages if selected none
						bool bNotZeroLanguages = DoWeNotHaveZeroLanguages(primaryLanguage);
						if (bNotZeroLanguages)
						{
							//  Check languages if we have duplicates
							bool bUniqueLanguages = DoWeHaveTwoUniqueLanguages(primaryLanguage, secondaryLanguage);
							if (bUniqueLanguages)
							{
								Vector3 vecSpawnPos = spawn == EScriptLocation.Paleto ? CharacterConstants.SpawnPosition_Paleto : CharacterConstants.SpawnPosition_LS;
								float fSpawnRot = spawn == EScriptLocation.Paleto ? CharacterConstants.SpawnRotation_Paleto : CharacterConstants.SpawnRotation_LS;

								// get our mask beard info, we dont give this below as a clothing item so the player doesnt get a 'mask' item from char create
								int BeardStyle = Drawables[(int)ECustomClothingComponent.Masks];
								int BeardTexture = Textures[(int)ECustomClothingComponent.Masks];

								// NOTE: Hair style is a drawable, which we want to save, but we don't want to give it as an item because that would be weird
								int HairStyle = Drawables[(int)ECustomClothingComponent.HairStyles];
								ECharacterSource source = ECharacterSource.CreatedOnOwl;

								EntityDatabaseID characterID = await Database.LegacyFunctions.CreateCharacterCustom(vecSpawnPos, fSpawnRot, gender, strName, SkinHash, Age, player.AccountID,
									Ageing,
									AgeingOpacity,
									Makeup,
									MakeupOpacity,
									MakeupColor,
									MakeupColorHighlight,
									Blush,
									BlushOpacity,
									BlushColor,
									BlushColorHighlight,
									Complexion,
									ComplexionOpacity,
									SunDamage,
									SunDamageOpacity,
									Lipstick,
									LipstickOpacity,
									LipstickColor,
									LipstickColorHighlights,
									MolesAndFreckles,
									MolesAndFrecklesOpacity,
									NoseSizeHorizontal,
									NoseSizeVertical,
									NoseSizeOutwards,
									NoseSizeOutwardsUpper,
									NoseSizeOutwardsLower,
									NoseAngle,
									EyebrowHeight,
									EyebrowDepth,
									CheekboneHeight,
									CheekWidth,
									CheekWidthLower,
									EyeSize,
									LipSize,
									MouthSize,
									MouthSizeLower,
									ChinSize,
									ChinSizeLower,
									ChinWidth,
									ChinEffect,
									NeckWidth,
									NeckWidthLower,
									FaceBlend1Mother,
									FaceBlend1Father,
									FaceBlendFatherPercent,
									SkinBlendFatherPercent,
									BaseHair,
									HairStyle,
									HairColor,
									HairColorHighlights,
									EyeColor,
									FacialHairStyle,
									FacialHairColor,
									FacialHairColorHighlight,
									FacialHairOpacity,
									Blemishes,
									BlemishesOpacity,
									Eyebrows,
									EyebrowsOpacity,
									EyebrowsColor,
									EyebrowsColorHighlight,
									lstTattooIDs,
									BodyBlemishes,
									BodyBlemishesOpacity,
									ChestHair,
									ChestHairColor,
									ChestHairColorHighlights,
									ChestHairOpacity,
									BeardStyle,
									BeardTexture,
									source).ConfigureAwait(true);

								// Give the player their clothing
								for (int i = 0; i < Drawables.Length; ++i)
								{
									// Only give them certain items if they actually picked one, blank isn't an item for some types (e.g. hats)...
									bool bShouldGrant = false;

									// NOTE: We currently only let the player modify these indexes, but we still need the entire array for GTA5.
									if (i == 3 || i == 4 || i == 6 || i == 11 || i == 7 || i == 8 || i == 10)
									{
										bShouldGrant = true;
									}

									// grant torso item only if the player actually set something
									if (i == 3 && Drawables[i] == 0)
									{
										bShouldGrant = false;
									}

									if (bShouldGrant)
									{
										CItemValueClothingCustom clothingValue = new CItemValueClothingCustom(Drawables[i], Textures[i], true);
										CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.CLOTHES_CUSTOM_FACE + i, clothingValue);
										player.Inventory.AddClothingItemToSocketForcefully(ItemInstanceDef, EItemSocket.Clothing, characterID);
									}
								}

								// Give the player their props
								foreach (var (kvPair, i) in DrawablesProps.Select((value, i) => (value, i)))
								{
									ECustomPropSlot slot = kvPair.Key;
									int prop = kvPair.Value;

									// Only give them a prop item if they actually picked one, blank isn't an item...
									if (prop != -1)
									{
										CItemValueClothingCustom clothingValue = new CItemValueClothingCustom(prop, TexturesProps[slot], true);
										CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(ItemHelpers.GetItemIDFromPropSlot(slot), clothingValue);
										player.Inventory.AddClothingItemToSocketForcefully(ItemInstanceDef, EItemSocket.Clothing, characterID);
									}
								}

								void OnFinish()
								{
									NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Success);
									player.HandleApplicationStateAndTransmitCharacters(false);

									player.AwardAchievement(EAchievementID.CustomCharacter);
								}

								// Always grant first language if we get here.
								Database.Functions.Characters.AddLanguage(characterID, primaryLanguage, 100f, true, (ulong insertID) =>
								{
									// Only grant second language if it's not none
									if (secondaryLanguage != ECharacterLanguage.None)
									{
										Database.Functions.Characters.AddLanguage(characterID, secondaryLanguage, 100f, false, (ulong insertID) =>
										{
											OnFinish();
										});
									}
									else
									{
										OnFinish();
									}
								});
							}
							else
							{
								NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_SameLanguage);
							}
						}
						else
						{
							NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_NoLanguage);
						}
					}
					else
					{
						NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_NameTaken);
					}
				});
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_CreateCharacterResponse(player, ECreateCharacterResponse.Failed_NameInvalid);
			}
		}
	}

	// TODO: Move to owl core
	public static bool IsValidCharacterName(string strName)
	{
		return strName.Length <= 32 && System.Text.RegularExpressions.Regex.IsMatch(strName, @"^[a-zA-Z '-]+[\s]+((['-][a-zA-Z ])?[a-zA-Z]*)*$");
	}

	public static bool DoWeHaveTwoUniqueLanguages(ECharacterLanguage primaryLanguage, ECharacterLanguage secondaryLanguage)
	{
		return !primaryLanguage.Equals(secondaryLanguage);
	}

	public static bool DoWeNotHaveZeroLanguages(ECharacterLanguage primaryLanguage)
	{
		return !primaryLanguage.Equals(ECharacterLanguage.None);
	}
}
