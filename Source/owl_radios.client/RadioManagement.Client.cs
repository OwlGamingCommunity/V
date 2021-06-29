using System;
using System.Collections.Generic;
using System.Linq;

public class RadioManagement
{
	private CGUIRadioPlayer m_RadioPlayer = new CGUIRadioPlayer(() => { });
	private CGUIRadioManagement m_RadioManagementUI = new CGUIRadioManagement(() => { });
	private List<RadioInstance> m_lstRadios = new List<RadioInstance>();

	private RadioInstance m_RadioOff = new RadioInstance(-1, -1, "Off", String.Empty, 0);

	private List<RadioInstance> m_lstDummyRadios = new List<RadioInstance>()
	{
		new RadioInstance(-2, -1, "Los Santos Rock Radio", "RADIO_01_CLASS_ROCK", 0),
		new RadioInstance(-3, -1, "Non-Stop-Pop FM", "RADIO_02_POP", 0),
		new RadioInstance(-4, -1, "Radio Los Santos", "RADIO_03_HIPHOP_NEW", 0),
		new RadioInstance(-5, -1, "Channel X", "RADIO_04_PUNK", 0),
		new RadioInstance(-6, -1, "West Coast Talk Radio", "RADIO_05_TALK_01", 0),
		new RadioInstance(-7, -1, "Rebel Radio", "RADIO_06_COUNTRY", 0),
		new RadioInstance(-8, -1, "Soulwax FM", "RADIO_07_DANCE_01", 0),
		new RadioInstance(-9, -1, "East Los FM", "RADIO_08_MEXICAN", 0),
		new RadioInstance(-10, -1, "West Coast Classics", "RADIO_09_HIPHOP_OLD", 0),
		new RadioInstance(-11, -1, "Blue Ark", "RADIO_12_REGGAE", 0),
		new RadioInstance(-12, -1, "Worldwide FM", "RADIO_13_JAZZ", 0),
		new RadioInstance(-13, -1, "FlyLo FM", "RADIO_14_DANCE_02", 0),
		new RadioInstance(-14, -1, "The Lowdown 91.1", "RADIO_15_MOTOWN", 0),
		new RadioInstance(-15, -1, "The Lab", "RADIO_20_THELAB", 0),
		new RadioInstance(-16, -1, "Radio Mirror Park", "RADIO_16_SILVERLAKE", 0),
		new RadioInstance(-17, -1, "Space 103.2", "RADIO_17_FUNK", 0),
		new RadioInstance(-18, -1, "Vinewood Boulevard Radio", "RADIO_18_90S_ROCK", 0),
		new RadioInstance(-19, -1, "Blonded Los Santos 97.8 FM", "RADIO_21_DLC_XM17", 0),
		new RadioInstance(-20, -1, "Blaine County Radio", "RADIO_11_TALK_02", 0),
		new RadioInstance(-21, -1, "Los Santos Underground Radio", "RADIO_22_DLC_BATTLE_MIX1_RADIO", 0),
		new RadioInstance(-22, -1, "Self Radio", "RADIO_19_USER", 0),
		new RadioInstance(-23, -1, "Classic Rock", "HIDDEN_RADIO_01_CLASS_ROCK", 0),
		new RadioInstance(-24, -1, "Bright", "HIDDEN_RADIO_AMBIENT_TV_BRIGHT", 0),
		new RadioInstance(-25, -1, "TV Radio", "HIDDEN_RADIO_AMBIENT_TV", 0),
		new RadioInstance(-26, -1, "Advert Radio", "HIDDEN_RADIO_ADVERTS", 0),
		new RadioInstance(-27, -1, "Pop", "HIDDEN_RADIO_02_POP", 0),
		new RadioInstance(-28, -1, "Hiphop", "HIDDEN_RADIO_03_HIPHOP_NEW", 0),
		new RadioInstance(-29, -1, "Punk", "HIDDEN_RADIO_04_PUNK", 0),
		new RadioInstance(-30, -1, "Country", "HIDDEN_RADIO_06_COUNTRY", 0),
		new RadioInstance(-31, -1, "Dance", "HIDDEN_RADIO_07_DANCE_01", 0),
		new RadioInstance(-32, -1, "Hiphop Old", "HIDDEN_RADIO_09_HIPHOP_OLD", 0),
		new RadioInstance(-33, -1, "Reggae", "HIDDEN_RADIO_12_REGGAE", 0),
		new RadioInstance(-34, -1, "Motown", "HIDDEN_RADIO_15_MOTOWN", 0),
		new RadioInstance(-35, -1, "Silverlake", "HIDDEN_RADIO_16_SILVERLAKE", 0),
		new RadioInstance(-36, -1, "Club", "HIDDEN_RADIO_STRIP_CLUB", 0),
		new RadioInstance(-37, -1, "Club 2", "RADIO_22_DLC_BATTLE_MIX1_CLUB", 0),
		new RadioInstance(-38, -1, "Club 3", "DLC_BATTLE_MIX1_CLUB_PRIV", 0),
		new RadioInstance(-39, -1, "Classic Rock 2", "HIDDEN_RADIO_BIKER_CLASSIC_ROCK", 0),
		new RadioInstance(-40, -1, "Club 4", "DLC_BATTLE_MIX2_CLUB_PRIV", 0),
		new RadioInstance(-41, -1, "Club 5", "RADIO_23_DLC_BATTLE_MIX2_CLUB", 0),
		new RadioInstance(-42, -1, "Modern Rock", "HIDDEN_RADIO_BIKER_MODERN_ROCK", 0),
		new RadioInstance(-43, -1, "Club 6", "RADIO_25_DLC_BATTLE_MIX4_CLUB", 0),
		new RadioInstance(-44, -1, "Club 7", "RADIO_26_DLC_BATTLE_CLUB_WARMUP", 0),
		new RadioInstance(-45, -1, "Club 8", "DLC_BATTLE_MIX4_CLUB_PRIV", 0),
		new RadioInstance(-46, -1, "Biker Punk", "HIDDEN_RADIO_BIKER_PUNK", 0),
		new RadioInstance(-47, -1, "Club 9", "RADIO_24_DLC_BATTLE_MIX3_CLUB", 0),
		new RadioInstance(-48, -1, "Club 10", "DLC_BATTLE_MIX3_CLUB_PRIV", 0),
		new RadioInstance(-49, -1, "Biker Hiphop", "HIDDEN_RADIO_BIKER_HIP_HOP", 0),
		new RadioInstance(-50, -1, "ML Radio", "RADIO_35_DLC_HEI4_MLR", 0),
		new RadioInstance(-51, -1, "Kult", "RADIO_34_DLC_HEI4_KULT", 0),
		new RadioInstance(-52, -1, "XM19", "RADIO_23_DLC_XM19_RADIO", 0),
		new RadioInstance(-53, -1, "PR Radio", "RADIO_27_DLC_PRHEI4", 0),
		new RadioInstance(-54, -1, "Casino FM", "HIDDEN_RADIO_CASINO", 0),
		new RadioInstance(-55, -1, "Pop", "HIDDEN_RADIO_ARCADE_POP", 0),
		new RadioInstance(-56, -1, "WWFM", "HIDDEN_RADIO_ARCADE_WWFM", 0),
		new RadioInstance(-57, -1, "Mirror Park FM", "HIDDEN_RADIO_ARCADE_MIRROR_PARK", 0),
		new RadioInstance(-58, -1, "EDM", "HIDDEN_RADIO_ARCADE_EDM", 0),
		new RadioInstance(-59, -1, "Dance", "HIDDEN_RADIO_ARCADE_DANCE", 0),
		new RadioInstance(-60, -1, "Penthouse", "HIDDEN_RADIO_CASINO_PENTHOUSE_P", 0),
		new RadioInstance(-61, -1, "Funk", "HIDDEN_RADIO_17_FUNK", 0)
	};

	private RadioInstance m_RadioBeingEdited = null;

	private const float m_fMaxVolume = 0.3f;
	private float m_fVolume = m_fMaxVolume;
	private bool m_IsDriver = false;
	private ERadioType m_CurrentRadioType = ERadioType.None;
	private float m_fLastDistBasedVolume = m_fMaxVolume;

	private RAGE.Elements.MapObject g_BoomboxBeingOperatedUpon = null;

	public RadioManagement()
	{
		NetworkEvents.GotoRadioStationManagement += ShowRadioManagement;

		NetworkEvents.GotBasicRadioInfo += OnGotBasicRadioInfo;

		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.SyncAllRadios += OnSyncAllRadios;
		NetworkEvents.SyncSingleRadio += OnSyncSingleRadio;

		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ExitVehicleStart += OnExitVehicle;

		NetworkEvents.ChangeCharacterApproved += () => { StopRadio(true); };

		NetworkEvents.LocalPlayerInventoryFull += OnInventoryChange;

		NetworkEvents.RequestBeginChangeBoomboxRadio_Response += OnChangeBoomboxRadioResponse;

		UIEvents.BoomBox_OnChangeRadio_Confirm += BoomBox_OnChangeRadio;

		RAGE.Game.Audio.SetRadioToStationName("OFF");
		RAGE.Game.Audio.SetMobilePhoneRadioState(false);
		RAGE.Game.Audio.SetMobileRadioEnabledDuringGameplay(false);

		RageEvents.AddDataHandler(EDataNames.VEH_RADIO, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { OnRadioUpdate((RAGE.Elements.Vehicle)entity, (int)newValue); });
		RageEvents.AddDataHandler(EDataNames.BOOMBOX_RID, (RAGE.Elements.Entity entity, object newValue, object oldValue) => { OnBoomboxUpdate((RAGE.Elements.MapObject)entity, (int)newValue); });
	}

	~RadioManagement()
	{

	}

	private void OnChangeBoomboxRadioResponse(bool bSuccess)
	{
		if (bSuccess)
		{
			List<KeyValuePair<string, string>> dictItems = new List<KeyValuePair<string, string>>();
			foreach (RadioInstance radio in m_lstRadios)
			{
				dictItems.Add(new KeyValuePair<string, string>(radio.Name, radio.ID.ToString()));
			}

			int radioID = DataHelper.GetEntityData<int>(g_BoomboxBeingOperatedUpon, EDataNames.BOOMBOX_RID);
			string strCurrentStationName = "Off";
			RadioInstance currentRadio = GetRadioFromID(radioID);
			if (currentRadio != null)
			{
				strCurrentStationName = currentRadio.Name;
			}

			GenericListbox.ShowGenericListbox(dictItems, "Boombox: Select Radio Station", Helpers.FormatString("Current Station: {0}", strCurrentStationName), "Change Station", "Cancel", "BoomBox_OnChangeRadio_Confirm", "", "");
		}
		else
		{
			g_BoomboxBeingOperatedUpon = null;
		}
	}

	private void OnRadioUpdate(RAGE.Elements.Vehicle vehicle, int radioID)
	{
		if (vehicle == RAGE.Elements.Player.LocalPlayer.Vehicle)
		{
			LoadRadioForVehicle(vehicle, true);
		}
	}

	private void OnBoomboxUpdate(RAGE.Elements.MapObject obj, int radioID)
	{
		// Not the one we're currently playing? Then we don't care
		if (obj == m_CurrentWorldRadioObj)
		{
			RadioInstance currentRadio = GetRadioFromID(radioID);
			LoadRadio(currentRadio, ERadioType.Boombox, null);
		}
	}

	private void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		StopRadio(true);

		m_IsDriver = seatId == (int)EVehicleSeat.Driver;
		vehicle.SetRadioEnabled(true);
		RAGE.Game.Audio.SetRadioToStationIndex(0);
		LoadRadioForVehicle(vehicle, false);
	}

	private void OnExitVehicle(RAGE.Elements.Vehicle vehicle, int seatID)
	{
		StopRadio(false);

		if (g_MP3PlayerRadioID != -1)
		{
			// reload mp3
			if (PlayerInventory.DoesLocalPlayerHaveItemOfType(EItemID.MP3_PLAYER))
			{
				RadioInstance currentRadio = GetRadioFromID(g_MP3PlayerRadioID);
				LoadRadio(currentRadio, ERadioType.MP3, null);
			}
			else
			{
				g_MP3PlayerRadioID = -1;
			}
		}
	}

	private RAGE.Elements.MapObject m_CurrentWorldRadioObj = null;
	private void OnCheckForWorldRadios()
	{
		// Only if no MP3, MP3 should override world audio since its in-ear
		if (g_MP3PlayerRadioID != -1)
		{
			if (PlayerInventory.DoesLocalPlayerHaveItemOfType(EItemID.MP3_PLAYER))
			{
				m_CurrentWorldRadioObj = null;
				return;
			}
		}

		// if in vehicle, use vehicle radio
		if (m_CurrentRadioType == ERadioType.Vehicle && RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			m_CurrentWorldRadioObj = null;
			return;
		}

		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.BoomBox);
		RAGE.Elements.MapObject boomboxObj = null;

		if (poolEntry != null)
		{
			boomboxObj = poolEntry.GetEntity<RAGE.Elements.MapObject>();

			if (boomboxObj != null)
			{
				// Has it changed? Otherwise we don't have to do anything
				if (m_CurrentWorldRadioObj != boomboxObj)
				{
					int radioID = DataHelper.GetEntityData<int>(boomboxObj, EDataNames.BOOMBOX_RID);

					RadioInstance currentRadio = GetRadioFromID(radioID);
					LoadRadio(currentRadio, ERadioType.Boombox, null);
				}
				else // update volume / distance
				{
					float fRatio = (OptimizationCachePool.g_fDefaultDistThreshold_Audio - poolEntry.GetDistance()) / OptimizationCachePool.g_fDefaultDistThreshold_Audio;
					float fDistBasedVolume = (fRatio * m_fMaxVolume);

					if (fDistBasedVolume != m_fLastDistBasedVolume)
					{
						m_fLastDistBasedVolume = fDistBasedVolume;
						SetBrowserVolume(fDistBasedVolume);
					}
				}
			}
		}

		// Support for transitioning away from a world radio to no world radio
		if (boomboxObj == null)
		{
			// Did we have a valid boombox?
			if (m_CurrentWorldRadioObj != null)
			{
				StopRadio(true);
				m_CurrentWorldRadioObj = null;
				m_fLastDistBasedVolume = m_fMaxVolume;
			}
		}

		m_CurrentWorldRadioObj = boomboxObj;
	}

	private void UpdateRightClick()
	{
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		if (bIsSpawned)
		{
			if (CursorManager.IsCursorVisible())
			{
				if (RAGE.Game.Pad.IsDisabledControlJustPressed(0, 25))
				{
					// Did we click a player?
					RAGE.Vector3 vecClickedWorldPos = GraphicsHelper.GetWorldPositionFromScreenPosition(CursorManager.GetCursorPosition());

					if (vecClickedWorldPos != null)
					{
						RAGE.Vector3 vecGameplayCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);

						CRaycastResult raycast = WorldHelper.RaycastFromTo(vecGameplayCamPos, vecClickedWorldPos, RAGE.Elements.Player.LocalPlayer.Handle, -1);

						if (raycast.Hit && raycast.EntityHit != null)
						{
							if (raycast.EntityHit.Type == RAGE.Elements.Type.Object)
							{
								RAGE.Elements.MapObject objHit = (RAGE.Elements.MapObject)raycast.EntityHit;

								// Is it a boombox?
								bool isBoombox = DataHelper.GetEntityData<bool>(objHit, EDataNames.BOOMBOX);
								if (isBoombox)
								{
									// Are they near enough?
									float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, raycast.EntityHit.Position);

									if (fDist <= 3.0)
									{
										g_BoomboxBeingOperatedUpon = objHit;
										NetworkEventSender.SendNetworkEvent_RequestBeginChangeBoomboxRadio(objHit);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private void BoomBox_OnChangeRadio(string strName, string strValue)
	{
		NotificationManager.ShowNotification("Boombox", Helpers.FormatString("You changed the Boombox to '{0}'", strName), ENotificationIcon.VolumeUp);

		NetworkEventSender.SendNetworkEvent_ChangeBoomboxRadio(g_BoomboxBeingOperatedUpon, strName, Convert.ToInt32(strValue));

		g_BoomboxBeingOperatedUpon = null;
	}

	private void OnTick()
	{
		OnCheckForWorldRadios();
		UpdateRightClick();

		if (KeyBinds.CanProcessKeybinds())
		{
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleNextRadio);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehiclePrevRadio);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleCinematicUpOnly);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleCinematicDownOnly);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.RadioWheelLeftRight);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.RadioWheelUpDown);

			if (RAGE.Game.Pad.IsDisabledControlJustReleased(2, (int)RAGE.Game.Control.VehicleCinematicUpOnly))
			{
				Vehicle_NextRadioStation();
			}

			if (RAGE.Game.Pad.IsDisabledControlJustReleased(2, (int)RAGE.Game.Control.VehicleCinematicDownOnly))
			{
				Vehicle_PrevRadioStation();
			}
		}
	}

	private void LoadRadioForVehicle(RAGE.Elements.Vehicle vehicle, bool bShowNotifications)
	{
		int vehicleRadioID = DataHelper.GetEntityData<int>(vehicle, EDataNames.VEH_RADIO);

		RadioInstance currentRadio = GetRadioFromID(vehicleRadioID);

		int currentRadioIndex = m_lstRadios.IndexOf(currentRadio);
		if (currentRadio == null)
		{
			currentRadioIndex = 0;
		}

		RadioInstance newRadio = m_lstRadios[currentRadioIndex];
		string strMessage = LoadRadio(newRadio, ERadioType.Vehicle, vehicle);

		if (bShowNotifications && !String.IsNullOrEmpty(strMessage))
		{
			HUD.ShowRadio(strMessage);
		}
	}

	enum ERadioType
	{
		None = -1,
		Vehicle,
		MP3,
		Boombox
	}

	private int g_MP3PlayerRadioID = -1;

	private void OnInventoryChange(List<CItemInstanceDef> lstInventory, EShowInventoryAction showInventoryAction)
	{
		bool bFoundMP3 = false;
		foreach (CItemInstanceDef itemDef in lstInventory)
		{
			if (itemDef.ItemID == EItemID.MP3_PLAYER)
			{
				bFoundMP3 = true;
			}
		}

		if (g_MP3PlayerRadioID != -1 && !bFoundMP3)
		{
			g_MP3PlayerRadioID = 0;

			StopRadio(true);
		}
	}

	private void Vehicle_NextRadioStation(int overrideAddon = 0)
	{
		int currentRadioID = -1;

		ERadioType radioType = ERadioType.None;

		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null && m_IsDriver)
		{
			currentRadioID = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer.Vehicle, EDataNames.VEH_RADIO);
			radioType = ERadioType.Vehicle;
		}
		else if (RAGE.Elements.Player.LocalPlayer.Vehicle == null && PlayerInventory.DoesLocalPlayerHaveItemOfType(EItemID.MP3_PLAYER))
		{
			currentRadioID = g_MP3PlayerRadioID;
			radioType = ERadioType.MP3;
		}

		if (radioType != ERadioType.None)
		{
			RadioInstance currentRadio = GetRadioFromID(currentRadioID);
			int currentRadioIndex = m_lstRadios.IndexOf(currentRadio);

			int newRadioIndex = currentRadioIndex + 1 + overrideAddon;

			// Are we going beyond the real end? use dummy 'none'
			if (newRadioIndex >= m_lstRadios.Count)
			{
				newRadioIndex = 0;
			}

			RadioInstance newRadio = m_lstRadios[newRadioIndex];

			// If it didn't resolve, skip
			if (!newRadio.ResolvedSuccessfully())
			{
				Vehicle_NextRadioStation(overrideAddon + 1);
			}
			else
			{
				// Apply
				if (radioType == ERadioType.Vehicle)
				{
					// Don't load here, load on change data event so we know server processed it
					NetworkEventSender.SendNetworkEvent_ChangeVehicleRadio(newRadio.ID);
				}
				else if (radioType == ERadioType.MP3)
				{
					g_MP3PlayerRadioID = newRadio.ID;

					string strMessage = LoadRadio(newRadio, ERadioType.MP3, null);

					if (!String.IsNullOrEmpty(strMessage))
					{
						HUD.ShowRadio(strMessage);
					}
				}
			}
		}
	}

	private void Vehicle_PrevRadioStation(int overrideAddon = 0)
	{
		int currentRadioID = -1;

		ERadioType radioType = ERadioType.None;

		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null && m_IsDriver)
		{
			currentRadioID = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer.Vehicle, EDataNames.VEH_RADIO);
			radioType = ERadioType.Vehicle;
		}
		else if (RAGE.Elements.Player.LocalPlayer.Vehicle == null && PlayerInventory.DoesLocalPlayerHaveItemOfType(EItemID.MP3_PLAYER))
		{
			currentRadioID = g_MP3PlayerRadioID;
			radioType = ERadioType.MP3;
		}

		if (radioType != ERadioType.None)
		{
			RadioInstance currentRadio = GetRadioFromID(currentRadioID);
			int currentRadioIndex = m_lstRadios.IndexOf(currentRadio);

			int newRadioIndex = currentRadioIndex - 1 - overrideAddon;

			// Are we going beyond the real end? use dummy 'none'
			if (newRadioIndex < 0)
			{
				newRadioIndex = m_lstRadios.Count - 1;
			}

			RadioInstance newRadio = m_lstRadios[newRadioIndex];

			// If it didn't resolve, skip
			if (!newRadio.ResolvedSuccessfully())
			{
				Vehicle_PrevRadioStation(overrideAddon + 1);
			}
			else
			{
				// Apply
				if (radioType == ERadioType.Vehicle)
				{
					// Don't load here, load on change data event so we know server processed it
					NetworkEventSender.SendNetworkEvent_ChangeVehicleRadio(newRadio.ID);
				}
				else if (radioType == ERadioType.MP3)
				{
					g_MP3PlayerRadioID = newRadio.ID;

					string strMessage = LoadRadio(newRadio, ERadioType.MP3, null);

					if (!String.IsNullOrEmpty(strMessage))
					{
						HUD.ShowRadio(strMessage);
					}
				}
			}
		}
	}

	private void StopRadio(bool bStopMP3Player)
	{
		if (m_RadioPlayer != null)
		{
			m_RadioPlayer.StopRadioStation();
		}

		if (bStopMP3Player)
		{
			RAGE.Game.Audio.SetRadioToStationName("OFF");
			RAGE.Game.Audio.SetMobilePhoneRadioState(false);
			RAGE.Game.Audio.SetMobileRadioEnabledDuringGameplay(false);
		}
	}

	private void SetBrowserVolume(float fVol)
	{
		if (m_RadioPlayer != null)
		{
			m_RadioPlayer.SetVolume(fVol);
		}
	}

	private string LoadRadio(RadioInstance radio, ERadioType radioType, RAGE.Elements.Vehicle vehicle)
	{
		string strMessage = String.Empty;

		if (radioType == ERadioType.Boombox)
		{
			StopRadio(true);
		}
		else
		{
			m_CurrentWorldRadioObj = null;
			StopRadio(false);
		}

		m_CurrentRadioType = radioType;

		int index = m_lstRadios.IndexOf(radio);
		int numRadios = m_lstRadios.Count - 1;
		string strInternalRadioStationChangeToMake = null;

		if (radio != null)
		{
			if (radio.ID < 0)
			{
				if (radio.ID == -1)
				{
					// do nothing
					strMessage = "Radio Off";
					strInternalRadioStationChangeToMake = "OFF";
				}
				else
				{
					// load a GTA built in radio
					strMessage = Helpers.FormatString("You are now listening to '{0}' ({1}/{2})", radio.Name, index, numRadios);
					strInternalRadioStationChangeToMake = radio.Endpoint;
				}
			}
			else
			{
				strInternalRadioStationChangeToMake = "OFF";

				if (!String.IsNullOrEmpty(radio.EndpointResolved))
				{
					m_RadioPlayer.SetVisible(true, false, false);
					m_RadioPlayer.PlayRadioStation(radio.EndpointResolved);
					m_fVolume = m_fMaxVolume;
					SetBrowserVolume(m_fVolume);

					strMessage = Helpers.FormatString("You are now listening to '{0}' ({1}/{2})", radio.Name, index, numRadios);
				}
			}
		}

		if (strInternalRadioStationChangeToMake != null)
		{
			if (radioType == ERadioType.Vehicle)
			{
				if (strInternalRadioStationChangeToMake == "OFF")
				{
					vehicle?.SetRadioEnabled(false);
				}
				else
				{
					vehicle?.SetRadioEnabled(true);
				}

				vehicle?.SetVehRadioStation(strInternalRadioStationChangeToMake);
			}
			else if (radioType == ERadioType.MP3)
			{
				if (strInternalRadioStationChangeToMake == "OFF")
				{
					RAGE.Game.Audio.SetRadioToStationName("OFF");
					RAGE.Game.Audio.SetMobilePhoneRadioState(false);
					RAGE.Game.Audio.SetMobileRadioEnabledDuringGameplay(false);
				}
				else
				{
					RAGE.Game.Audio.SetRadioToStationName(strInternalRadioStationChangeToMake);
					RAGE.Game.Audio.SetMobilePhoneRadioState(true);
					RAGE.Game.Audio.SetMobileRadioEnabledDuringGameplay(true);
				}
			}
			else if (radioType == ERadioType.Boombox)
			{
				if (strInternalRadioStationChangeToMake == "OFF")
				{
					RAGE.Game.Audio.SetRadioToStationName("OFF");
					RAGE.Game.Audio.SetMobilePhoneRadioState(false);
					RAGE.Game.Audio.SetMobileRadioEnabledDuringGameplay(false);
				}
				else
				{
					RAGE.Game.Audio.SetRadioToStationName(strInternalRadioStationChangeToMake);
					RAGE.Game.Audio.SetMobilePhoneRadioState(true);
					RAGE.Game.Audio.SetMobileRadioEnabledDuringGameplay(true);
				}
			}
		}

		return strMessage;
	}

	private void OnSyncAllRadios(List<RadioInstance> lstRadios)
	{
		// Clear
		m_lstRadios.Clear();

		// Add Off (must be first)
		m_lstRadios.Add(m_RadioOff);

		// Add remote radios
		m_lstRadios.AddRange(lstRadios);

		// Add Dummies
		m_lstRadios.AddRange(m_lstDummyRadios);

		UpdateUIRadiosList(false);
	}

	private void UpdateUIRadiosList(bool bDoGotoRadioList)
	{
		if (m_RadioManagementUI.IsVisible())
		{
			m_RadioManagementUI.Reset_Radios();

			foreach (RadioInstance radio in GetRadiosFromLocalPlayer())
			{
				m_RadioManagementUI.AddRadio(radio.ID, radio.Name, radio.ExpirationTime.ToString());
			}

			m_RadioManagementUI.CommitRadios();

			// We only do this on the initialization, otherwise a remote player editing a radio would change our UI visible state back to radios list if we were editing
			if (bDoGotoRadioList)
			{
				m_RadioManagementUI.GotoRadioList();
			}
		}
	}

	private void OnSyncSingleRadio(RadioInstance radioInst)
	{
		// remove existing
		m_lstRadios.RemoveAll(radio => radio.ID == radioInst.ID);

		// add
		m_lstRadios.Add(radioInst);

		// Update UI if showing
		UpdateUIRadiosList(false);
	}

	private RadioInstance GetRadioFromID(long radioID)
	{
		foreach (RadioInstance radio in m_lstRadios)
		{
			if (radio.ID == radioID)
			{
				return radio;
			}
		}

		return null;
	}

	private List<RadioInstance> GetRadiosFromLocalPlayer()
	{
		int accountID = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.ACCOUNT_ID);
		return m_lstRadios.Where(radio => radio.Account == accountID).ToList();
	}

	private void ShowRadioManagement()
	{
		NetworkEventSender.SendNetworkEvent_GetBasicRadioInfo();
	}

	private void OnGotBasicRadioInfo(int donatorCurrency)
	{
		m_RadioManagementUI.SetVisible(true, true, false);
		m_RadioManagementUI.Reset_GC();

		m_RadioManagementUI.SetGCBalance(donatorCurrency);

		UpdateUIRadiosList(true);
	}

	public void HideRadioManagement()
	{
		m_RadioManagementUI.SetVisible(false, false, false);
	}

	public void OnPurchaseRadio()
	{
		NetworkEventSender.SendNetworkEvent_PurchaseRadioRequest();
	}

	public void OnEditRadio(long a_RadioID)
	{
		RadioInstance targetRadio = GetRadioFromID(a_RadioID);
		if (targetRadio != null)
		{
			m_RadioBeingEdited = targetRadio;
			m_RadioManagementUI.GotoEditRadio(targetRadio.Name, targetRadio.Endpoint);
		}
	}

	public void OnExtendRadio7Days(int a_RadioID)
	{
		NetworkEventSender.SendNetworkEvent_ExtendRadio7Days(a_RadioID);
	}

	public void OnExtendRadio30Days(int a_RadioID)
	{
		NetworkEventSender.SendNetworkEvent_ExtendRadio30Days(a_RadioID);
	}

	public void OnSaveRadio(string strName, string strEndpoint)
	{
		NetworkEventSender.SendNetworkEvent_SaveRadio(m_RadioBeingEdited.ID, strName, strEndpoint);
		m_RadioBeingEdited = null;
	}

	public void OnCancelEditRadio()
	{
		m_RadioBeingEdited = null;
	}
}