
using System;
using System.Collections.Generic;

using EntityDatabaseID = System.Int64;

public static class CharacterSelection
{
	private static WeakReference<AudioInstance> m_musicInst = new WeakReference<AudioInstance>(null);

	static CharacterSelection()
	{
		// EVENTS
		NetworkEvents.RetrievedCharacters += OnRetrievedCharacters;
		NetworkEvents.PreviewCharacterGotData += OnPreviewCharacterGotData;
		NetworkEvents.CharacterSelectionApproved += OnCharacterSelectionApproved;
		NetworkEvents.ChangeCharacterApproved += OnChangeCharacterApproved;
		NetworkEvents.ShowSpawnSelector += OnShowSpawnSelector;
		NetworkEvents.LoadTransferAssetsCharacterData += OnLoadAssetTransfer;
		NetworkEvents.AssetTransferCompleted += OnAssetTransferCompleted;

		// TODO_CSHARP: Move this to hud
		RageEvents.RAGE_OnRender += OnRender;

		UIEvents.OnSpawnSelector_LS += () => { OnSpawnSelectorResult(EScriptLocation.LS); };
		UIEvents.OnSpawnSelector_Paleto += () => { OnSpawnSelectorResult(EScriptLocation.Paleto); };
	}

	public static void Init()
	{

	}

	private static void OnShowSpawnSelector()
	{
		GenericPromptHelper.ShowPrompt("Legacy Character", "This character is a legacy character which was created before the expansion to Los Santos. Where would you like to spawn?", "Spawn in Los Santos", "Keep my existing Paleto Bay spawn", UIEventID.OnSpawnSelector_LS, UIEventID.OnSpawnSelector_Paleto);
	}

	private static GetCharactersCharacter GetCharacterFromID(EntityDatabaseID ID)
	{
		foreach (var charIter in m_lstCharacters)
		{
			if (charIter.id == ID)
			{
				return charIter;
			}
		}

		return null;
	}

	public static List<GetCharactersCharacter> GetCharacters()
	{
		return m_lstCharacters;
	}

	private static void OnSpawnSelectorResult(EScriptLocation location)
	{
		if (g_LastCharacterID >= 0)
		{
			GetCharactersCharacter character = GetCharacterFromID(g_LastCharacterID);

			if (character != null)
			{
				NetworkEventSender.SendNetworkEvent_SpawnSelected(location, character.id);
			}
		}
	}

	private static void OnRender()
	{
		// TODO_CSHARP: Move this to hud
		if (g_PendingRetrieveCharacter)
		{
			HUD.SetLoadingMessage("Loading Character");
		}

		if (bIsLoadingWorld)
		{
			HUD.SetLoadingMessage("Loading World");

			if (RAGE.Elements.Player.LocalPlayer.HasCollisionLoadedAround())
			{
				bIsLoadingWorld = false;
			}
		}
	}

	private static void OnChangeCharacterApproved()
	{
		ShowCharacterUI();
	}

	private static void OnCharacterSelectionApproved()
	{
		GenericProgressBar.CloseAnyProgressBar();

		RAGE.Chat.Activate(true);

		CleanupCharacterUI(true);
		RAGE.Game.Ui.DisplayHud(true);
		RAGE.Chat.Show(true); // TODO_CSHARP: These should be in a helper class + refcounted

		// TODO_CSHARP: Once hud is converted, we must remove this bridge
		HUD.SetVisible(true, false, false);
		RAGE.Game.Ui.DisplayRadar(true);

		// resolution warning
		const float fMinResX = 1920.0f;
		const float fMinResY = 1080.0f;
		Vector2 vecResolution = GraphicsHelper.GetScreenResolution();
		if (vecResolution.X < fMinResX || vecResolution.Y < fMinResY)
		{
			NotificationManager.ShowNotification("Resolution Warning", Helpers.FormatString("Your current resolution ({0}x{1}) is lower than the recommended {2}x{3} resolution.<br><br>Consider increasing your resolution for the best possible experience.", vecResolution.X, vecResolution.Y, fMinResX, fMinResY), ENotificationIcon.InfoSign);
		}
	}

	private static void OnRetrievedCharacters(List<GetCharactersCharacter> lstCharacters, Int64 currentAutoSpawnCharacter)
	{
		ResetLastCharacterID();
		g_CharactersListUI.SetAutoSpawnVisible(false);
		g_CurrentAutoSpawnCharacter = currentAutoSpawnCharacter;

		LoginSystem.HideLogin();

		ShowCharacterUI();

		m_lstCharacters = lstCharacters;

		g_CharactersListUI.ClearCharacters();

		foreach (var character in m_lstCharacters)
		{
			g_CharactersListUI.AddCharacter(character.id, character.name, character.LastSeenHours, character.pos, character.cked);
		}

		g_CharactersListUI.CommitCharacters();

		bool bAltTrack = new Random().Next(2) == 1 ? true : false;

		EAudioIDs audioID = (bAltTrack ? EAudioIDs.Country : EAudioIDs.MenuMusic);
		if (WorldHelper.IsChristmas())
		{
			audioID = EAudioIDs.Christmas;
		}
		else if (WorldHelper.IsHalloween())
		{
			audioID = EAudioIDs.Halloween;
		}

#if !DEBUG
		m_musicInst = AudioManager.PlayAudio(audioID, true, true);
#endif
	}

	public static void StopMusic()
	{
#if !DEBUG
		AudioManager.FadeOutAudio(m_musicInst);
#endif
	}

	private static void CleanupCharacterUI(bool bSmooth)
	{
		g_LastCharacterID = -1;
		m_lstCharacters.Clear();
		g_CharactersListUI.SetVisible(false, false, false);


		// Is the gameplay cam far away? don't smooth
		RAGE.Vector3 vecActiveCamPos = CameraManager.GetActiveCameraPosition();
		RAGE.Vector3 vecGamePlayCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);

		float fDist = WorldHelper.GetDistance2D(vecActiveCamPos, vecGamePlayCamPos);

		if (fDist > 5.0f)
		{
			bSmooth = false;
		}

		CameraManager.DeactiveAnyCamera(bSmooth, 3000);

		StopMusic();
	}

	public static void ResetLastCharacterID()
	{
		g_LastCharacterID = -1;
	}

	public static void OnSetAutoSpawn()
	{
		if (g_LastCharacterID != -1)
		{
			GetCharactersCharacter character = GetCharacterFromID(g_LastCharacterID);

			if (character != null)
			{
				// Is it our current auto spawn one?
				if (g_CurrentAutoSpawnCharacter == character.id)
				{
					g_CurrentAutoSpawnCharacter = -1;
				}
				else
				{
					g_CurrentAutoSpawnCharacter = character.id;
				}

				NetworkEventSender.SendNetworkEvent_SetAutoSpawnCharacter(g_CurrentAutoSpawnCharacter);
				UpdateAutoSpawnState(character.id);
			}
		}
	}

	public static void OpenTransferAssets()
	{
		if (g_LastCharacterID == -1)
		{
			return;
		}

		GetCharactersCharacter character = GetCharacterFromID(g_LastCharacterID);
		if (character == null)
		{
			return;
		}

		NetworkEventSender.SendNetworkEvent_FetchTransferAssetsData(g_LastCharacterID);
	}

	private static void UpdateAutoSpawnState(Int64 selectedCharacterID)
	{
		if (g_CurrentAutoSpawnCharacter != selectedCharacterID)
		{
			g_CharactersListUI.SetAutoSpawnText("Enable Auto Spawn");
		}
		else
		{
			g_CharactersListUI.SetAutoSpawnText("Disable Auto Spawn");
		}
	}

	public static void PreviewCharacter(EntityDatabaseID ID)
	{
		CameraManager.DeactivateCamera(ECameraID.LOGIN_SCREEN);

		// don't let them spawn further or spam different char xyz's until the world is loaded
		if (bIsLoadingWorld)
		{
			return;
		}

		bIsLoadingWorld = true;

		GetCharactersCharacter character = GetCharacterFromID(ID);
		if (character != null)
		{
			if (!g_PendingRetrieveCharacter)
			{
				bool bSuccess = false;

				if (ID == g_LastCharacterID)
				{
					if (!character.cked)
					{
						bSuccess = true;

						// Dont cleanup everything here, since it cleans the camera up etc
						g_CharactersListUI.SetVisible(false, false, false);

						NetworkEventSender.SendNetworkEvent_CharacterSelected(character.id);
					}
					else
					{
						RAGE.Game.Audio.PlaySoundFrontend(-1, "CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET", true);
					}
				}
				else
				{
					bSuccess = true;
					g_LastCharacterID = ID;
					g_PendingRetrieveCharacter = true;

					RAGE.Game.Graphics.StartScreenEffect("MinigameTransitionOut", 250, false);

					// TODO_CSHARP: Put this on a 150ms delay timer, we need clientside C# timer pool for that
					GetCharacterDetails(character.id);
				}

				if (bSuccess)
				{
					CursorManager.SetCursorVisible(false, g_CharactersListUI);
					RAGE.Game.Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FREEMODE_SOUNDSET", true);
				}

				g_CharactersListUI.SetAutoSpawnVisible(true);
				UpdateAutoSpawnState(character.id);
			}
		}
	}

	private static void GetCharacterDetails(long characterID)
	{
		NetworkEventSender.SendNetworkEvent_PreviewCharacter(characterID);
	}

	private static void OnPreviewCharacterGotData()
	{
		// NOTE: Fixes excessive rain puddles on spawn that stick around
		if (WorldHelper.IsHalloween())
		{
			RAGE.Game.Misc.ClearOverrideWeather();
			RAGE.Game.Misc.SetWeatherTypeNow("EXTRASUNNY");
		}

		CursorManager.SetCursorVisible(true, g_CharactersListUI);
		g_PendingRetrieveCharacter = false;
		float fDist = 2.5f;
		float fRot = RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f;
		var vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
		var vecCamPos = new RAGE.Vector3(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z);
		var radians = fRot * (3.14 / 180.0);
		vecCamPos.X += (float)Math.Cos(radians) * fDist;
		vecCamPos.Y += (float)Math.Sin(radians) * fDist;
		vecCamPos.Z += 0.5f;

		CameraManager.RegisterCamera(ECameraID.CHARACTER_SELECTION_PREVIEW, vecCamPos, vecPlayerPos);
		CameraManager.ActivateCamera(ECameraID.CHARACTER_SELECTION_PREVIEW);
	}

	private static void OnLoadAssetTransfer(long characterId, float money, float bankmoney, List<SVehicle> vehicles,
		List<SProperty> properties)
	{
		g_CharactersListUI.SetVisible(false, false, false);
		g_AssetTransferUI.Show(characterId, money, bankmoney, vehicles, properties);
	}

	private static void OnAssetTransferCompleted()
	{
		g_AssetTransferUI.CloseAssetTransfer();
	}

	public static void ShowCharacterUI()
	{
		g_LastCharacterID = -1;
		LoginSystem.GotoLoginCamera();
		g_CharactersListUI.SetVisible(true, true, true);
		DiscordManager.SetDiscordStatus("Choosing a Character");
	}

	public static void HideCharacterUI()
	{
		g_CharactersListUI.SetVisible(false, false, false);
	}

	private static void OnUILoaded()
	{

	}

	private static CGUIAssetTransfer g_AssetTransferUI = new CGUIAssetTransfer(OnUILoaded);
	private static GUICharacterList g_CharactersListUI = new GUICharacterList(OnUILoaded);
	private static List<GetCharactersCharacter> m_lstCharacters = new List<GetCharactersCharacter>();
	private static bool g_PendingRetrieveCharacter = false;
	private static EntityDatabaseID g_LastCharacterID = -1;
	private static Int64 g_CurrentAutoSpawnCharacter = -1;
	private static bool bIsLoadingWorld = false;
}