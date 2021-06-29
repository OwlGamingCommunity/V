using System.Collections.Generic;

public class GovernmentBlips
{
	// TODO_LAUNCH: Cleanup on character change, leave faction, etc
	// TODO_LAUNCH: a /backup
	Dictionary<RAGE.Elements.Player, GovernmentBlip> m_dictGovtBlips = new Dictionary<RAGE.Elements.Player, GovernmentBlip>();

	RAGE.Elements.Blip g_911Blip = null;
	RAGE.Vector3 g_911Location = null;
	private class GovernmentBlip
	{
		public EDutyType DutyType { get; }
		public RAGE.Elements.Blip BlipInstance { get; }

		public GovernmentBlip(RAGE.Elements.Player a_Player, EDutyType a_DutyType)
		{
			DutyType = a_DutyType;

			// TODO_LAUNCH: What about dimensions? Does it copy player dim? Probably not.
			BlipInstance = new RAGE.Elements.Blip(1, a_Player.Position, a_Player.Name);
		}
	}

	public GovernmentBlips()
	{
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;
		RageEvents.RAGE_OnTick_OncePerSecond += Blip911_CheckCloseEnough;
		RageEvents.RAGE_OnPlayerQuit += DestroyBlipForPlayer;

		NetworkEvents.PlayerWentOffDuty += OnPlayerWentOffDuty;
		NetworkEvents.Create911LocationBlip += OnCreate911LocationBlip;
		NetworkEvents.Destroy911LocationBlip += OnDestroy911LocationBlip;

		RageEvents.AddDataHandler(EDataNames.BACKUP, BackupChanged);
		RageEvents.AddDataHandler(EDataNames.TOWTRUCK_BEACON, TowtruckChanged);
	}

	private void BackupChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		bool bNewValue = (bool)newValue;

		// is local player on duty?
		bool bIsOnDuty = false;
		bool bIsLoggedIn = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_LOGGED_IN);
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);

		if (bIsLoggedIn && bIsSpawned)
		{
			EDutyType dutyType = DataHelper.GetLocalPlayerEntityData<EDutyType>(EDataNames.DUTY);
			if (dutyType == EDutyType.Law_Enforcement || dutyType == EDutyType.EMS || dutyType == EDutyType.Fire)
			{
				bIsOnDuty = true;
			}
		}

		if (!bIsOnDuty)
		{
			return;
		}

		if (bNewValue)
		{
			RAGE.Game.Audio.PlaySoundFrontend(-1, "Out_Of_Bounds_Timer", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", true);
		}
	}

	private void TowtruckChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		bool bNewValue = (bool)newValue;

		// is local player on duty?
		bool bIsOnDuty = false;
		bool bIsLoggedIn = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_LOGGED_IN);
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);

		if (bIsLoggedIn && bIsSpawned)
		{
			EDutyType dutyType = DataHelper.GetLocalPlayerEntityData<EDutyType>(EDataNames.DUTY);
			if (dutyType == EDutyType.Towing)
			{
				bIsOnDuty = true;
			}
		}

		if (!bIsOnDuty)
		{
			return;
		}

		if (bNewValue)
		{
			RAGE.Game.Audio.PlaySoundFrontend(-1, "Out_Of_Bounds_Timer", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", true);
		}
		else
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			DestroyBlipForPlayer(player);
		}
	}

	private void OnPlayerWentOffDuty(RAGE.Elements.Player playerGoingOffDuty)
	{
		if (playerGoingOffDuty == RAGE.Elements.Player.LocalPlayer)
		{
			foreach (var player in RAGE.Elements.Entities.Players.All)
			{
				DestroyBlipForPlayer(player);
			}

			// Remove all blips
			m_dictGovtBlips.Clear();
		}
		else
		{
			DestroyBlipForPlayer(playerGoingOffDuty);
		}
	}

	private void DestroyBlipForPlayer(RAGE.Elements.Player player)
	{
		if (player != null)
		{
			if (m_dictGovtBlips.ContainsKey(player))
			{
				RAGE.Elements.Blip blip = m_dictGovtBlips[player].BlipInstance;
				m_dictGovtBlips.Remove(player);
				blip.Destroy();
			}
		}
		Destroy911Blip();
	}

	private void Destroy911Blip()
	{
		if (g_911Blip != null)
		{
			g_911Blip.Destroy();
			g_911Blip = null;
		}

		g_911Location = null;
	}

	private void OnTick()
	{
		// is local player on duty?
		bool bIsOnDuty = false;
		bool bIsLoggedIn = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_LOGGED_IN);
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);

		if (bIsLoggedIn && bIsSpawned)
		{
			EDutyType dutyType = DataHelper.GetLocalPlayerEntityData<EDutyType>(EDataNames.DUTY);
			if (dutyType == EDutyType.Law_Enforcement || dutyType == EDutyType.EMS || dutyType == EDutyType.Fire || dutyType == EDutyType.Towing)
			{
				bIsOnDuty = true;
			}
		}

		if (!bIsOnDuty)
		{
			// If we're not on duty, but have blips, destroy them
			if (m_dictGovtBlips.Count > 0)
			{
				foreach (RAGE.Elements.Player player in RAGE.Elements.Entities.Players.All)
				{
					if (player != RAGE.Elements.Player.LocalPlayer)
					{
						DestroyBlipForPlayer(player);
					}
				}
			}
			return;
		}

		// Do we need to create a blip?
		foreach (RAGE.Elements.Player player in RAGE.Elements.Entities.Players.All)
		{
			if (player != RAGE.Elements.Player.LocalPlayer)
			{
				bool bRemotePlayerIsLoggedIn = DataHelper.GetEntityData<bool>(player, EDataNames.IS_LOGGED_IN);
				bool bRemotePlayerIsSpawned = DataHelper.GetEntityData<bool>(player, EDataNames.IS_SPAWNED);

				if (bRemotePlayerIsLoggedIn && bRemotePlayerIsSpawned)
				{
					EDutyType remotePlayerDutyType = DataHelper.GetEntityData<EDutyType>(player, EDataNames.DUTY);

					if (remotePlayerDutyType == EDutyType.Law_Enforcement || remotePlayerDutyType == EDutyType.EMS || remotePlayerDutyType == EDutyType.Fire)
					{
						if (!m_dictGovtBlips.ContainsKey(player))
						{
							CreateBlipForPlayer(player, remotePlayerDutyType);
						}
					}
					
					bool bRemotePlayerRequestingTowtruck = DataHelper.GetEntityData<bool>(player, EDataNames.TOWTRUCK_BEACON);
					if (!m_dictGovtBlips.ContainsKey(player) && bRemotePlayerRequestingTowtruck)
					{
						CreateBlipForPlayer(player, EDutyType.Towing);
					}
				}
				
			}
		}

		// TODO: Function inside GovernmentBlip?
		foreach (var kvPair in m_dictGovtBlips)
		{
			RAGE.Elements.Player ownerPlayer = kvPair.Key;
			GovernmentBlip blip = kvPair.Value;

			int blipColor = 0;
			if (blip.DutyType == EDutyType.Law_Enforcement)
			{
				blipColor = 29;
			}
			else if (blip.DutyType == EDutyType.EMS)
			{
				blipColor = 17;
			}
			else if (blip.DutyType == EDutyType.Fire)
			{
				blipColor = 1;
			}
			else if (blip.DutyType == EDutyType.Towing)
			{
				blipColor = 28;
			}

			if (blip.DutyType != EDutyType.Towing)
			{
				int unitNumber = DataHelper.GetEntityData<int>(ownerPlayer, EDataNames.UNIT_NUMBER);
				blip.BlipInstance.ShowNumberOn(unitNumber);
			}

			// use real position if streamed in, otherwise use the slower cached position
			float fDist = WorldHelper.GetDistance2D(RAGE.Elements.Player.LocalPlayer.Position, ownerPlayer.Position);
			if (fDist <= 300.0f)
			{
				blip.BlipInstance.Position = ownerPlayer.Position.CopyVector();
				blip.BlipInstance.SetRotation((int)ownerPlayer.GetRotation(0).Z);
			}
			else
			{
				float fX = DataHelper.GetEntityData<float>(ownerPlayer, EDataNames.DP_X);
				float fY = DataHelper.GetEntityData<float>(ownerPlayer, EDataNames.DP_Y);
				float fRotZ = DataHelper.GetEntityData<float>(ownerPlayer, EDataNames.DP_RZ);

				// TODO: We could compress rot probably, since its 0->360
				// We always set fZ to our local players Z, we don't care if they're above or lower
				blip.BlipInstance.Position = new RAGE.Vector3(fX, fY, RAGE.Elements.Player.LocalPlayer.Position.Z);
				blip.BlipInstance.SetRotation((int)fRotZ);
			}


			blip.BlipInstance.SetColour(blipColor);
			blip.BlipInstance.Dimension = ownerPlayer.Dimension;
			blip.BlipInstance.ShowHeadingIndicatorOn(true);
			blip.BlipInstance.SetScale(2.0f);

			// Is the backup or towtruck beacon active?
			if (DataHelper.GetEntityData<bool>(ownerPlayer, EDataNames.BACKUP) || DataHelper.GetEntityData<bool>(ownerPlayer, EDataNames.TOWTRUCK_BEACON))
			{
				if (!blip.BlipInstance.IsFlashing())
				{
					blip.BlipInstance.SetFlashes(true);
					blip.BlipInstance.SetRoute(true);
					blip.BlipInstance.SetRouteColour(blip.BlipInstance.GetColour());
				}
			}
			else
			{
				if (blip.BlipInstance.IsFlashing())
				{
					blip.BlipInstance.SetFlashes(false);
					blip.BlipInstance.SetRoute(false);
				}
			}
		}
	}

	private void Blip911_CheckCloseEnough()
	{
		if (g_911Blip != null)
		{
			// NOTE: Blip position doesnt work here, so we have to use a cached version
			float fDist = WorldHelper.GetDistance2D(RAGE.Elements.Player.LocalPlayer.Position, g_911Location);

			if (fDist < 5.0f)
			{
				Destroy911Blip();
			}
		}
	}
	private void CreateBlipForPlayer(RAGE.Elements.Player player, EDutyType dutyType)
	{
		DestroyBlipForPlayer(player);

		m_dictGovtBlips.Add(player, new GovernmentBlip(player, dutyType));
	}

	private void OnCreate911LocationBlip(RAGE.Vector3 position)
	{
		Destroy911Blip();

		g_911Location = position;
		g_911Blip = new RAGE.Elements.Blip(162, position, "911 call");
		g_911Blip.SetFlashes(true);
		g_911Blip.SetRoute(true);
	}

	private void OnDestroy911LocationBlip()
	{
		Destroy911Blip();
	}
}