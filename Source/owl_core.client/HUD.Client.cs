using System;
using System.Collections.Generic;

public static class HUD
{
	static HUD()
	{

	}

	public static void Init()
	{
		RageEvents.RAGE_OnTick_PerFrame += OnTick;
		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_LowFrequency += UpdateSpeedLimit;
		NetworkEvents.CharacterSelectionApproved += OnSpawned;
		NetworkEvents.ChangeCharacterApproved += OnCharacterChanged;

		NetworkEvents.SetLoadingWorld += OnSetLoadingWorld;

		UIEvents.ChangeCharacterRequested += OnHUDChangeCharacterRequested;
		UIEvents.ReportBug += OnReportBug;
		UIEvents.GotoFullScreenBrowser += OnGotoFullScreenBrowser;
		UIEvents.HideHudMenu += OnHideHudMenu;

		UIEvents.GotoDiscordLinking += OnGotoDiscordLinking;
		NetworkEvents.GotoDiscordLinking_Response += OnGotoDiscordLinking_Response;
		UIEvents.DiscordGetURLHack += OnDiscordPageURIResult;
		UIEvents.OnUnlinkDiscord_Confirm += OnUnlinkDiscord_Confirm;
		UIEvents.OnUnlinkDiscord_Cancel += OnUnlinkDiscord_Cancel;
		UIEvents.GotoEditInterior += OnEditInterior;
		UIEvents.OpenRadioManager += OnOpenRadioManager;
		UIEvents.GotoLanguages += OnGotoLanguages;
		UIEvents.ToggleLocalNametag += OnToggleLocalNametag;
		UIEvents.CloseF10Menu += CloseF10Menu;
		UIEvents.PersistentNotifications_Dismissed += OnPersistentNotificationDismissed;

		NetworkEvents.LocalPlayerDimensionChanged += OnDimensionChanged;
		NetworkEvents.SetProgressBar += SetProgressBar;
		NetworkEvents.HideProgressBar += HideProgressBar;

		// TODO_RAGE: Workaround for OnBrowserDomReady not working
		RageEvents.RAGE_OnTick_MediumFrequency += CheckDiscordBrowser;

		ClientTimerPool.CreateTimer(UpdateCachedHUD, 100);
		ClientTimerPool.CreateTimer(UpdateWeaponClipStatus, 100);
		ClientTimerPool.CreateTimer(UpdateCachedWeaponHUD, 100);

		ScriptControls.SubscribeToControl(EScriptControlID.ShowFullScreenBrowser, OpenFullscreenBrowser);
		ScriptControls.SubscribeToControl(EScriptControlID.CloseFullScreenBrowser, CloseFullscreenBrowser);

		ScriptControls.SubscribeToControl(EScriptControlID.ChangeMinimapMode, ChangeMinimapMode);

		ScriptControls.SubscribeToControl(EScriptControlID.GetPosition, (EControlActionType actionType) => { NetworkEventSender.SendNetworkEvent_GetPos(); });
		ScriptControls.SubscribeToControl(EScriptControlID.TakeScreenshot, OnTakeScreenshot);

		NetworkEvents.PersistentNotifications_Created += OnPersistentNotificationCreated;
		NetworkEvents.PersistentNotifications_LoadAll += OnPersistentNotificationsLoaded;

		KeyBinds.Bind(ConsoleKey.F10, ToggleF10Menu, EKeyBindType.Pressed, EKeyBindFlag.Default);

		// Load dicts
		if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(g_LoadingTextureDictionary))
		{
			RAGE.Game.Graphics.RequestStreamedTextureDict(g_LoadingTextureDictionary, true);
		}

		// TODO_CSHARP: Better location + not a timer + not all types of markers
		ClientTimerPool.CreateTimer((object[] parameters) => { foreach (var marker in RAGE.Elements.Entities.Markers.All) { marker.Rotating = true; } }, 5000);
	}

	private static void OnPersistentNotificationCreated(CPersistentNotification notification)
	{
		m_guiHUD.PushPersistentNotification(notification);
	}

	private static void OnPersistentNotificationDismissed(Int64 notificationID)
	{
		NetworkEventSender.SendNetworkEvent_PersistentNotifications_Deleted(notificationID);
	}

	private static void SetProgressBar(string text, string percent)
	{
		m_guiHUD.SetProgressBar(text, percent);
	}

	private static void HideProgressBar()
	{
		m_guiHUD.HideProgressBar();
	}

	private static void OnPersistentNotificationsLoaded(List<CPersistentNotification> lstNotifications)
	{
		m_guiHUD.SetPersistentNotifications(lstNotifications);
	}

	private static void OnSetLoadingWorld(bool bLoadingWorld)
	{
		m_bLoadingWorld = bLoadingWorld;
	}

	private static void OnDimensionChanged(uint oldDim, uint newDim)
	{
		if (newDim > 0 && newDim != WorldHelper.GetPlayerSpecificDimension())
		{
			m_guiHUD.SetEditInteriorVisible(true);
		}
		else
		{
			m_guiHUD.SetEditInteriorVisible(false);
		}
	}

	private static void OnOpenRadioManager()
	{
		NetworkEventSender.SendNetworkEvent_GetBasicRadioInfo();
	}

	private static void OnGotoLanguages()
	{
		NetworkEventSender.SendNetworkEvent_OpenLanguagesUI();
	}

	private static void OnToggleLocalNametag()
	{
		NetworkEventSender.SendNetworkEvent_ToggleLocalPlayerNametag();
	}

	public static bool b_F10MenuOpen = false;

	private static void ToggleF10Menu()
	{
		b_F10MenuOpen = !b_F10MenuOpen;
		m_guiHUD.ToggleF10Menu(b_F10MenuOpen);

		CursorManager.SetCursorVisible(b_F10MenuOpen, m_guiHUD);
	}

	private static void CloseF10Menu()
	{
		if (b_F10MenuOpen)
		{
			b_F10MenuOpen = false;
			m_guiHUD.ToggleF10Menu(b_F10MenuOpen);

			CursorManager.SetCursorVisible(b_F10MenuOpen, m_guiHUD);
		}
	}


	private static void UpdateSpeedLimit()
	{
		if (RAGE.Elements.Player.LocalPlayer.Vehicle == null)
		{
			if (m_LastSpeedLimit != ESpeedLimit.None)
			{
				VehicleHUD.ClearHUD();
				m_LastSpeedLimit = ESpeedLimit.None;
				m_LastSpeedLimitMPH = 0;
			}

			return;
		}

		EVehicleClass vehicleClass = (EVehicleClass)RAGE.Elements.Player.LocalPlayer.Vehicle.GetClass();
		if (vehicleClass == EVehicleClass.VehicleClass_Cycles
			|| vehicleClass == EVehicleClass.VehicleClass_Boats
			|| vehicleClass == EVehicleClass.VehicleClass_Helicopters
			|| vehicleClass == EVehicleClass.VehicleClass_Planes
			|| vehicleClass == EVehicleClass.VehicleClass_Trains)
		{
			if (m_LastSpeedLimit != ESpeedLimit.None)
			{
				VehicleHUD.ClearHUD();
				m_LastSpeedLimit = ESpeedLimit.None;
				m_LastSpeedLimitMPH = 0;
			}

			return;
		}

		RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();

		RAGE.Vector3 vecNodePos = new RAGE.Vector3();
		bool bFoundMain = RAGE.Game.Pathfind.GetClosestVehicleNode(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z, vecNodePos, 0, 100.0f, 25.0f);

		RAGE.Vector3 vecNodePosIncludeSlow = new RAGE.Vector3();
		bool bFoundMainIncludeSlow = RAGE.Game.Pathfind.GetClosestVehicleNode(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z, vecNodePosIncludeSlow, 1, 100.0f, 25.0f);

		ESpeedLimit speedLimit = ESpeedLimit.None;

		if (bFoundMain && bFoundMainIncludeSlow)
		{
			// If these are equal, then we are on a fast road
			// If these are NOT equal, then we found a slow node, and we should determine which one is nearer
			if (vecNodePos == vecNodePosIncludeSlow)
			{
				speedLimit = ESpeedLimit.High;
			}
			else
			{
				float fDistMainNode = WorldHelper.GetDistance(vecNodePos, vecPlayerPos);
				float fDistMainNodeIncludeSlow = WorldHelper.GetDistance(vecNodePos, vecPlayerPos);

				if (fDistMainNode < fDistMainNodeIncludeSlow)
				{
					speedLimit = ESpeedLimit.High;
				}
				else
				{
					speedLimit = ESpeedLimit.Low;
				}
			}
		}
		else
		{
			speedLimit = ESpeedLimit.Low;
		}

		// Check for highways
		bool bIsOnHighway = false;
		int streetNameID = -1;
		int crossroadNameID = -1;
		RAGE.Game.Pathfind.GetStreetNameAtCoord(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z, ref streetNameID, ref crossroadNameID);

		if (streetNameID != -1)
		{
			string strStreetName = RAGE.Game.Ui.GetStreetNameFromHashKey((uint)streetNameID);
			if (strStreetName.ToLower().Contains("hwy") || strStreetName.ToLower().Contains("highway"))
			{
				bIsOnHighway = true;
				speedLimit = ESpeedLimit.High;
			}
		}

		m_LastSpeedLimit = speedLimit;

		bool bIsInPaleto = (RAGE.Game.Zone.GetNameOfZone(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z) == "PALETO");
		if (m_LastSpeedLimit == ESpeedLimit.High)
		{
			if (bIsOnHighway)
			{
				m_LastSpeedLimitMPH = 80;
			}
			else
			{
				m_LastSpeedLimitMPH = bIsInPaleto ? 50 : 80;
			}
		}
		else
		{
			m_LastSpeedLimitMPH = bIsInPaleto ? 30 : 50;
		}
	}

	public static void ShowSubtitle(string strMessage, int time)
	{
		m_guiHUD.ShowSubtitle(strMessage, time);
	}

	public static void ShowRadio(string strMessage)
	{
		m_guiHUD.ShowRadio(strMessage);
	}

	private static void OnHideHudMenu()
	{
		CursorManager.SetCursorVisible(false, m_guiHUD);
	}

	private static void OnGotoDiscordLinking()
	{
		CursorManager.SetCursorVisible(false, m_guiHUD);
		NetworkEventSender.SendNetworkEvent_GotoDiscordLinking();
	}

	private static void OnGotoDiscordLinking_Response(bool bHasLink, string strDiscordName, string strURL)
	{
		if (bHasLink)
		{
			GenericPromptHelper.ShowPrompt("Discord Account Un-Link", Helpers.FormatString("Your account is already linked to Discord account '{0}'. Would you like to un-link these accounts?", strDiscordName), "Yes, Un-link them", "No, keep my accounts linked", UIEventID.OnUnlinkDiscord_Confirm, UIEventID.OnUnlinkDiscord_Cancel);
		}
		else
		{
			ShowFullScreenBrowser(strURL);
			m_bIsDiscordLinking = true;
		}
	}

	private static string m_strLastDiscordPage = String.Empty;
	private static void OnDiscordPageURIResult(string strURL)
	{
		if (m_bIsDiscordLinking && m_strLastDiscordPage != strURL)
		{
			m_strLastDiscordPage = strURL;

			// TODO_GITHUB: You should replace the below with your own website
			if (strURL.ToLower().StartsWith("https://www.website.com"))
			{
				NetworkEventSender.SendNetworkEvent_DiscordLinkFinalize(strURL);
				CloseFullscreenBrowser(EControlActionType.Pressed);
				m_strLastDiscordPage = String.Empty;
				m_bIsDiscordLinking = false;
			}
		}
	}

	private static void OnUnlinkDiscord_Confirm()
	{
		NetworkEventSender.SendNetworkEvent_DiscordDeLink();
		NotificationManager.ShowNotification("Discord", "Your Discord account has been unlinked from your Owl account.", ENotificationIcon.InfoSign);
	}

	private static void OnUnlinkDiscord_Cancel()
	{
		NotificationManager.ShowNotification("Discord", "You canceled unlinking your accounts, your Discord account remains linked.", ENotificationIcon.InfoSign);
	}

	private static void OnEditInterior()
	{
		NetworkEventSender.SendNetworkEvent_RequestEditInterior();
	}

	private static void CheckDiscordBrowser()
	{
		if (m_bIsDiscordLinking && m_cefFullScreenBrowser != null)
		{
			string strCMD = Helpers.FormatString("mp.trigger('{0}', '{1}', {2});", "UI", "DiscordGetURLHack", "window.location.href");
			m_cefFullScreenBrowser.ExecuteJs(strCMD);
		}
	}

	private static void OnCharacterChanged()
	{
		m_guiHUD.Reload();
		CloseFullscreenBrowser(EControlActionType.Pressed);
		SetVisible(false, false, false);
	}

	private static void OnHUDChangeCharacterRequested()
	{
		NetworkEventSender.SendNetworkEvent_RequestChangeCharacter();
	}

	private static void OnReportBug()
	{
		ShowFullScreenBrowser(g_strBugtrackerURL);
	}

	private static void OnGotoFullScreenBrowser(string strURL)
	{
		ShowFullScreenBrowser(strURL);
	}

	public static void ShowFullScreenBrowser(string strURL)
	{
		m_bHasPendingFullScreenBrowser = true;

		if (m_cefFullScreenBrowser != null)
		{
			m_cefFullScreenBrowser.Destroy();
		}

		KeyBinds.SetKeybindsDisabled(false);
		CursorManager.SetCursorVisible(false, m_guiHUD);

		m_cefFullScreenBrowser = new RAGE.Ui.HtmlWindow(strURL);
		m_cefFullScreenBrowser.Active = false;
	}

	private static void ShowPendingFullScreenBrowser()
	{
		m_bHasPendingFullScreenBrowser = false;
		m_cefFullScreenBrowser.Active = true;
		KeyBinds.SetKeybindsDisabled(true);
		CursorManager.SetCursorVisible(true, m_guiHUD);
	}

	enum EMinimapMode
	{
		Normal,
		Large_LocalArea,
		Large_FullMap
	}
	private static EMinimapMode m_CurrentMinimapMode = EMinimapMode.Normal;

	private static void ChangeMinimapMode(EControlActionType actionType)
	{
		if (m_CurrentMinimapMode == EMinimapMode.Normal)
		{
			m_CurrentMinimapMode = EMinimapMode.Large_LocalArea;
			RAGE.Game.Ui.SetRadarBigmapEnabled(true, false);
		}
		else if (m_CurrentMinimapMode == EMinimapMode.Large_LocalArea)
		{
			m_CurrentMinimapMode = EMinimapMode.Large_FullMap;
			RAGE.Game.Ui.SetRadarBigmapEnabled(true, true);
		}
		else if (m_CurrentMinimapMode == EMinimapMode.Large_FullMap)
		{
			m_CurrentMinimapMode = EMinimapMode.Normal;
			RAGE.Game.Ui.SetRadarBigmapEnabled(false, false);
		}
	}

	private static void CloseFullscreenBrowser(EControlActionType actionType)
	{
		if (m_cefFullScreenBrowser != null)
		{
			m_cefFullScreenBrowser.Destroy();
		}

		KeyBinds.SetKeybindsDisabled(false);
		CursorManager.SetCursorVisible(false, m_guiHUD);

		m_bHasPendingFullScreenBrowser = false;
	}

	private static void OpenFullscreenBrowser(EControlActionType actionType)
	{
		if (m_bHasPendingFullScreenBrowser)
		{
			m_bHasPendingFullScreenBrowser = false;
			ShowPendingFullScreenBrowser();
		}
	}

	public static bool IsVisible()
	{
		return m_guiHUD.IsVisible();
	}

	public static void SetVisible(bool bVisible, bool bEnableInput, bool bAffectChat)
	{
		m_guiHUD.SetVisible(bVisible, bEnableInput, bAffectChat);
	}

	private static void OnSpawned()
	{
		SetVisible(true, false, false);
	}

	// PER FRAME FUNCTIONS
	private static void OnTick()
	{
		UpdateCuffState();
		ProcessBlockedControlsAndHUD();
	}

	private static void OnRender()
	{
		if (m_bLoadingWorld)
		{
			SetLoadingMessage("Loading World");
		}

		DrawVersion();
		RenderBrowserMessage();
		RenderLoadingMessage();
	}

	private static void UpdateCachedHUD(object[] parameters)
	{
		if (IsVisible())
		{
			// DATE & TIME
			int hours = RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetClockHours);
			int mins = RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetClockMinutes);

			string strDateString = DateTime.Now.ToString("dddd, dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat);

			// DIRECTION
			float fRot = RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z;
			string strDirection = String.Empty;
			if (fRot <= 22.5 && fRot > -22.5)
			{
				strDirection = "N";
			}
			else if (fRot <= -22.5 && fRot > -67.5)
			{
				strDirection = "NE";
			}
			else if (fRot <= -67.5 && fRot > -112.5)
			{
				strDirection = "E";
			}
			else if (fRot <= -112.5 && fRot > -157.5)
			{
				strDirection = "SE";
			}
			else if (fRot <= -157.5 || fRot >= 157.5)
			{
				strDirection = "S";
			}
			else if (fRot >= 112.5 && fRot < 157.5)
			{
				strDirection = "SW";
			}
			else if (fRot >= 67.5 && fRot < 112.5)
			{
				strDirection = "W";
			}
			else if (fRot >= 22.5 && fRot < 67.5)
			{
				strDirection = "NW";
			}
			//m_strLocationString

			RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;

			int streetNameID = -1;
			int crossroadNameID = -1;
			string strStreetName = String.Empty;
			string strCrossroadName = String.Empty;
			RAGE.Game.Pathfind.GetStreetNameAtCoord(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z, ref streetNameID, ref crossroadNameID);

			if (streetNameID > 0)
			{
				strStreetName = RAGE.Game.Ui.GetStreetNameFromHashKey((uint)streetNameID);
			}

			if (crossroadNameID > 0)
			{
				strCrossroadName = RAGE.Game.Ui.GetStreetNameFromHashKey((uint)crossroadNameID);
			}

			string strZoneName = RAGE.Game.Zone.GetNameOfZone(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z);
			string strRealZoneName = ZoneNameHelper.ZoneNames.ContainsKey(strZoneName) ? ZoneNameHelper.ZoneNames[strZoneName] : "San Andreas"; // TODO_CSHARP: Helper func instead
			string strLocationString;

			if (strCrossroadName == String.Empty)
			{
				strLocationString = Helpers.FormatString("{0} on {1}, {2}", strDirection, strStreetName, strRealZoneName);
			}
			else
			{
				strLocationString = Helpers.FormatString("{0} on {1} & {2}, {3}", strDirection, strStreetName, strCrossroadName, strRealZoneName);
			}

			m_guiHUD.SetLocation(strLocationString);
		}
	}

	private static void ProcessBlockedControlsAndHUD()
	{
		RAGE.Game.Invoker.Invoke(RAGE.Game.Natives._0x0AFC4AF510774B47); // _BLOCK_WEAPON_WHEEL_THIS_FRAME
		RAGE.Game.Ui.ShowWeaponWheel(false);

		// Disable UI elements
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.WantedStars);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.WeaponIcon);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.Cash);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.CashChange);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.Saving);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.WeaponWheel);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.WeaponWheelStats);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.VehicleName);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.AreaName);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.Unused); //HUD_VEHICLE_CLASS
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.StreetName);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.HelpText);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.FloatingHelpText1);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.FloatingHelpText2);
		HUDHelper.HideHudComponentThisFrame(RAGE.Game.HudComponent.GamingStreamUnusde);

		if (CursorManager.IsCursorVisible())
		{
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.FrontendPause);
		}

		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.CharacterWheel);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MultiplayerInfo);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Phone);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SpecialAbility);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SpecialAbilitySecondary);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SpecialAbilityPC);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.EnterCheatCode);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.InteractionMenu);

		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelUpDown);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelLeftRight);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelNext);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelPrev);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectNextWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectPrevWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.DropWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.DropAmmo);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponUnarmed);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponMelee);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponHandgun);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponShotgun);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponSmg);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponAutoRifle);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponSniper);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponHeavy);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponSpecial);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.PrevWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.NextWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectCharacterMichael);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectCharacterFranklin);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectCharacterTrevor);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectCharacterMultiplayer);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SaveReplayClip);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.FrontendLeaderboard);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Duck);

		// TODO_CSHARP: Why doesn't this system work anymore? Task just does nothing.
		//ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Enter);

		// Only disable exit key if vehicle is locked
		var localPlayer = RAGE.Elements.Player.LocalPlayer;
		if (localPlayer.Vehicle != null)
		{
			if (localPlayer.Vehicle.GetDoorLockStatus() == 0)
			{
				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleExit);
			}

			// HACK: allow headlight for tow truck, its used for the rear tow release control (bizarre, thanks rockstar)
			if (localPlayer.Vehicle.Model != 2971866336)
			{
				ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleHeadlight);
			}
		}
	}

	private static void RenderBrowserMessage()
	{
		if (m_bHasPendingFullScreenBrowser)
		{
			TextHelper.Draw2D("You are about to enter a fullscreen browser experience.", 0.5f, 0.85f, 0.5f, 255, 194, 15, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
			TextHelper.Draw2D(Helpers.FormatString("Press {0} to proceed. Press {1} to cancel.", ScriptControls.GetKeyBoundToControl(EScriptControlID.ShowFullScreenBrowser).ToString(), ScriptControls.GetKeyBoundToControl(EScriptControlID.CloseFullScreenBrowser).ToString()), 0.50f, 0.89f, 0.3f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
			TextHelper.Draw2D(Helpers.FormatString("Press {0} at any time to exit during browsing", ScriptControls.GetKeyBoundToControl(EScriptControlID.CloseFullScreenBrowser).ToString()), 0.50f, 0.92f, 0.3f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
		}
	}

	private static string m_strLoadingMessage = String.Empty;

	public static void SetLoadingMessage(string strMessage)
	{
		if (strMessage != String.Empty)
		{
			m_strLoadingMessage = strMessage;
		}
	}

	private static void RenderLoadingMessage()
	{
		if (m_strLoadingMessage != String.Empty)
		{
			float x = 0.93f;
			float y = 0.97f;

			// sprite
			if (RAGE.Game.Graphics.HasStreamedTextureDictLoaded(g_LoadingTextureDictionary))
			{
				RAGE.Game.Graphics.DrawSprite(g_LoadingTextureDictionary, "info_icon_32", x - 0.068f, y, 0.015f, 0.03f, 0, 255, 255, 255, 255, 0);
			}

			RAGE.Game.Graphics.DrawRect(x, y, 0.12f, 0.03f, 0, 0, 0, 200, 0);
			TextHelper.Draw2D(m_strLoadingMessage, x, y - 0.017f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);

			m_strLoadingMessage = String.Empty;
		}
	}

	private static void DrawVersion()
	{
#if DEBUG
		if (IsVisible())
		{
			TextHelper.Draw2D("DEBUG BUILD", 0.5f, 0.97f, 0.27f, 255, 255, 255, 200, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, false, false);
		}
#endif
	}

	private static void UpdateCuffState()
	{
		bool bCuffed = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.CUFFED);
		if (bCuffed) // switch to unarmed
		{
			uint unarmedHash = HashHelper.GetHashUnsigned("WEAPON_UNARMED");
			RAGE.Elements.Player.LocalPlayer.SetCurrentWeapon(unarmedHash, true);
		}
	}
	// END PER FRAME FUNCTIONS

	private static void OnUILoaded()
	{

	}

	public static CGUIHUD GetHudBrowser()
	{
		return m_guiHUD;
	}

	private static void UpdateWeaponClipStatus(object[] parameters)
	{
		RAGE.Elements.Player localPlayer = RAGE.Elements.Player.LocalPlayer;

		int weaponHash = 0;
		if (!localPlayer.GetCurrentWeapon(ref weaponHash, false))
		{
			m_LastWeapon = (uint)WeaponHash.Unarmed;
			m_RemainingAmmoClip = -1;
			m_RemainingTotalAmmo = -1;
			return;
		}

		m_LastWeapon = unchecked((uint)weaponHash);

		int clipSize = RAGE.Game.Weapon.GetWeaponClipSize(m_LastWeapon);

		// Fix for melee weapons
		if (clipSize == 0)
		{
			m_RemainingAmmoClip = -1;
			m_RemainingTotalAmmo = -1;
		}
		else
		{
			int ammoRemainingInWeaponClip = RAGE.Elements.Player.LocalPlayer.GetAmmoInWeapon(m_LastWeapon);

			// Fix for tazer
			if (clipSize > 2000)
			{
				m_RemainingAmmoClip = ammoRemainingInWeaponClip;
				m_RemainingTotalAmmo = -1;
			}
			else
			{
				int maxClipSize = RAGE.Elements.Player.LocalPlayer.GetMaxAmmoInClip(m_LastWeapon, true);
				RAGE.Elements.Player.LocalPlayer.GetAmmoInClip(m_LastWeapon, ref m_RemainingAmmoClip);
				m_RemainingTotalAmmo = ammoRemainingInWeaponClip - maxClipSize + (maxClipSize - m_RemainingAmmoClip);
			}
		}
	}

	private static void UpdateCachedWeaponHUD(object[] parameters)
	{
		// only when changed, due to fade in/out
		if (m_LastWeapon_Cached != m_LastWeapon)
		{
			m_LastWeapon_Cached = m_LastWeapon;

			int clip = m_RemainingAmmoClip;
			int total = m_RemainingTotalAmmo;

			int currentWeaponID = WeaponHelpers.GetWeaponItemIDFromHash(m_LastWeapon);

			// weapon selector supports -2, we don't (fixes #0002436)
			if (currentWeaponID < -1)
			{
				return;
			}

			// TODO_OPTIMIZATION: We could optimize this by not executing hud stuff every frame, just when it changes? (all hud stuff, ammo should be set all the time tho, for sanity. Otherwise we miss updates)

			// Tazer fix
			if (m_LastWeapon == 911657153)
			{
				total = -2;

				if (clip < 0)
				{
					clip = 0;
				}
			}

			m_guiHUD.Execute("SetWeaponInfo", currentWeaponID, clip, total);
		}
	}

	private static void OnTakeScreenshot(EControlActionType actionType)
	{
		if (m_guiHUD.IsVisible())
		{
			m_guiHUD.SetVisible(false, false, false);
			RAGE.Game.Ui.DisplayRadar(false);
			NetworkEventSender.SendNetworkEvent_ToggleNametags(true);
		}
		else
		{
			m_guiHUD.SetVisible(true, false, false);
			RAGE.Game.Ui.DisplayRadar(true);
			NetworkEventSender.SendNetworkEvent_ToggleNametags(false);
		}
	}

	private static uint m_LastWeapon_Cached = 0;
	private static uint m_LastWeapon = 0;
	private static int m_RemainingAmmoClip = -1;
	private static int m_RemainingTotalAmmo = -1;

	private static CGUIHUD m_guiHUD = new CGUIHUD(OnUILoaded);
	private static string g_LoadingTextureDictionary = "shared";

	// TODO_GITHUB: You should replace the below with your own website
	private static string g_strBugtrackerURL = "https://bugs.website.com/bug_report_page.php";
	private static bool m_bHasPendingFullScreenBrowser = false;
	private static RAGE.Ui.HtmlWindow m_cefFullScreenBrowser = null;

	enum ESpeedLimit
	{
		None = -1,
		Low,
		High
	}

	private static ESpeedLimit m_LastSpeedLimit = ESpeedLimit.None;
	public static int m_LastSpeedLimitMPH = 0;
	private static bool m_bLoadingWorld = false;
	private static bool m_bIsDiscordLinking = false;
}

