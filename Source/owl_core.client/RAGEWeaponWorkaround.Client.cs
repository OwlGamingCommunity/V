using System;
using System.Collections.Generic;

// TODO_0_4: This is a work around for serverside ammo not working
public class RAGEWeaponWorkaround
{

	private List<EWeaponCategory> exemptedWeaponGroups = new List<EWeaponCategory>()
	{
		EWeaponCategory.Melee,
		EWeaponCategory.Throwable,
		EWeaponCategory.Sniper,
		EWeaponCategory.RangedProjectile,
		EWeaponCategory.Shotgun,
		EWeaponCategory.Special,
		EWeaponCategory.None
	};

	private List<WeaponHash> allowedBurstWeapons = new List<WeaponHash>() { WeaponHash.Appistol };
	private List<EWeaponCategory> allowedBurstWeaponGroups = new List<EWeaponCategory>() { EWeaponCategory.SMG, EWeaponCategory.MachineGun, EWeaponCategory.Rifle };
	private List<WeaponHash> blockSingleFireWeapons = new List<WeaponHash>() {
		WeaponHash.Stungun,
		WeaponHash.Doubleaction,
		WeaponHash.Musket,
		WeaponHash.Flaregun,
		WeaponHash.Pumpshotgun,
		WeaponHash.Dbshotgun,
		WeaponHash.Marksmanpistol,
		WeaponHash.Pumpshotgun_mk2,
		WeaponHash.Sniperrifle,
		WeaponHash.Revolver,
		WeaponHash.Sawnoffshotgun,
		WeaponHash.Heavysniper,
		WeaponHash.Revolver_mk2,
		WeaponHash.Bullpupshotgun,
		WeaponHash.Heavysniper_mk2
	};

	private bool IgnoreCurrentWeapon = false;
	private Dictionary<int, EWeaponFireTypes> WeaponConfigs = new Dictionary<int, EWeaponFireTypes>();
	private EWeaponFireTypes CurrentFiringMode = EWeaponFireTypes.Auto;
	private int CurrentBurstShots = 0;
	private int CurrentWeaponHash = 0;
	private bool m_bIsSemiAutoWeapon = true;

	public RAGEWeaponWorkaround()
	{
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleFireMode, ToggleWeaponMode);

		RageEvents.RAGE_OnPlayerWeaponShot += OnWeaponShot;
		RageEvents.RAGE_OnTick_LowFrequency += UpdateAmmo;
		ClientTimerPool.CreateTimer(UpdateWeapons, 1000);

		NetworkEvents.InformPlayerOfFireModes += (bool bSemiAuto) => { m_bIsSemiAutoWeapon = bSemiAuto; };

		foreach (EWeapons weaponID in Enum.GetValues(typeof(EWeapons)))
		{
			m_LastAmmoData[weaponID] = 0;
		}

		m_TaserAmmoToSubtract = 0;

		RageEvents.RAGE_OnTick_PerFrame += () =>
		{
			// TAZER FIX
			// Is this a taser? GTA5 tazers have unlimited ammo...
			CurrentWeaponHash = 0;
			RAGE.Elements.Player.LocalPlayer.GetCurrentWeapon(ref CurrentWeaponHash, false);

			if (CurrentWeaponHash == HashHelper.GetHashSigned(EWeapons.WEAPON_STUNGUN.ToString()))
			{
				Dictionary<EWeapons, int> weaponData = WeaponHelpers.GetAllWeaponsAmmo();
				if (weaponData.ContainsKey(EWeapons.WEAPON_STUNGUN))
				{
					int ammo = weaponData[EWeapons.WEAPON_STUNGUN];
					if (ammo <= 0)
					{
						RAGE.Game.Player.DisablePlayerFiring(true);
					}
				}
			}

			int comparedWeaponHash = 0;
			RAGE.Elements.Player.LocalPlayer.GetCurrentWeapon(ref comparedWeaponHash, false);

			IgnoreCurrentWeapon = IsWeaponIgnored(CurrentWeaponHash);

			if (CurrentWeaponHash != comparedWeaponHash)
			{
				RAGE.Elements.Player.LocalPlayer.GetCurrentWeapon(ref CurrentWeaponHash, false);

				CurrentFiringMode = WeaponConfigs.ContainsKey(CurrentWeaponHash) ? (EWeaponFireTypes)WeaponConfigs[CurrentWeaponHash] : EWeaponFireTypes.Auto;
				CurrentBurstShots = 0;
			}

			if (IgnoreCurrentWeapon)
			{
				CurrentFiringMode = EWeaponFireTypes.Auto;
				return;
			}

			if (m_bIsSemiAutoWeapon)
			{
				if (CanWeaponUseSingleFire(CurrentWeaponHash))
				{
					CurrentFiringMode = EWeaponFireTypes.SingleFire;
				}
				else
				{
					CurrentFiringMode = EWeaponFireTypes.Auto;
				}
			}

			if (CurrentFiringMode != EWeaponFireTypes.Auto)
			{
				if (CurrentFiringMode == EWeaponFireTypes.Burst)
				{
					// handle burst fire
					if (RAGE.Elements.Player.LocalPlayer.IsShooting())
					{
						CurrentBurstShots++;
					}
					if (CurrentBurstShots > 0 && CurrentBurstShots < 3)
					{
						RAGE.Game.Pad.SetControlNormal(0, 24, 1.0f);
					}

					if (CurrentBurstShots == 3)
					{
						RAGE.Game.Player.DisablePlayerFiring(false);
						if (RAGE.Game.Pad.IsDisabledControlJustReleased(0, 24))
						{
							CurrentBurstShots = 0;
						}
					}

					if (RAGE.Elements.Player.LocalPlayer.IsReloading())
					{
						CurrentBurstShots = 0;
					}
				}
				else if (CurrentFiringMode == EWeaponFireTypes.SingleFire)
				{
					// handle single fire
					if (RAGE.Game.Pad.IsDisabledControlPressed(0, 24))
					{
						RAGE.Game.Player.DisablePlayerFiring(false);
					}
				}
			}
		};

	}

	private void OnWeaponShot(RAGE.Vector3 targetPos, RAGE.Elements.Player target, RAGE.Events.CancelEventArgs cancel)
	{
		// Is this a taser?
		int currentWeaponHash = 0;
		RAGE.Elements.Player.LocalPlayer.GetCurrentWeapon(ref currentWeaponHash, false);

		if (currentWeaponHash == HashHelper.GetHashSigned(EWeapons.WEAPON_STUNGUN.ToString()))
		{
			++m_TaserAmmoToSubtract;
		}
	}

	private void UpdateWeapons(object[] parameters)
	{
		// Have our weapons changed? Sync to server
		List<WeaponHash> lstWeapons = WeaponHelpers.GetAllWeapons();
		if (lstWeapons != m_lastWeaponData)
		{
			NetworkEventSender.SendNetworkEvent_StoreWeapons(lstWeapons);
		}
	}

	private void UpdateAmmo()
	{
		// Calculate a diff from last send
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		if (!bIsSpawned)
		{
			return;
		}

		Dictionary<EWeapons, int> newWeaponData = WeaponHelpers.GetAllWeaponsAmmo();
		Dictionary<EWeapons, int> weaponsDiff = new Dictionary<EWeapons, int>();

		foreach (var kvPair in newWeaponData)
		{
			EWeapons weaponID = kvPair.Key;
			int newAmmo = kvPair.Value;

			// Is this a taser?
			if (kvPair.Key == EWeapons.WEAPON_STUNGUN)
			{
				if (m_TaserAmmoToSubtract > 0)
				{
					newAmmo -= m_TaserAmmoToSubtract;
					m_TaserAmmoToSubtract = 0;
				}
			}

			int lastAmmo = m_LastAmmoData.ContainsKey(weaponID) ? m_LastAmmoData[weaponID] : 0;

			if (lastAmmo != newAmmo)
			{
				if (newAmmo < 0)
				{
					newAmmo = 0;
				}

				m_LastAmmoData[weaponID] = newAmmo;
				weaponsDiff.Add(weaponID, newAmmo);
			}
		}

		// store this revision
		m_LastAmmoData = newWeaponData;

		if (weaponsDiff.Count > 0)
		{
			// TODO_0_4: If we do keep this, we should optimize it, it sends all weapons using the ammo type despite the ammo being shared (e.g. 7 AR's even if you only have 1, because ammo is by type of gun, not gun)
			NetworkEventSender.SendNetworkEvent_StoreAmmo(weaponsDiff);
		}
	}

	private void ToggleWeaponMode(EControlActionType actionType)
	{
		if (IgnoreCurrentWeapon)
		{
			return;
		}

		// are we semi auto?
		if (m_bIsSemiAutoWeapon)
		{
			NotificationManager.ShowNotification("Weapon Firemode", "This weapon is semi-automatic and does not have fire modes", ENotificationIcon.InfoSign);
			return;
		}

		EWeaponFireTypes newFiringMode = CurrentFiringMode + 1;
		if (newFiringMode > EWeaponFireTypes.SingleFire)
		{
			newFiringMode = EWeaponFireTypes.Auto;
		}

		if (newFiringMode == EWeaponFireTypes.Burst)
		{
			// switched to burst, check if weapon supports burst fire. if not, skip to safe
			if (!CanWeaponUseBurstFire(CurrentWeaponHash))
			{
				newFiringMode = CanWeaponUseSingleFire(CurrentWeaponHash) ? EWeaponFireTypes.SingleFire : EWeaponFireTypes.Auto;
			}
		}
		else if (newFiringMode == EWeaponFireTypes.SingleFire)
		{
			// switched to single, check if weapon supports single fire. if not, skip to safe
			if (!CanWeaponUseSingleFire(CurrentWeaponHash))
			{
				newFiringMode = EWeaponFireTypes.Auto;
			}
		}

		if (CurrentFiringMode != newFiringMode)
		{
			CurrentFiringMode = newFiringMode;
			CurrentBurstShots = 0;

			// TODO_JER: add output that weapon mode changed besides a sound?
			RAGE.Game.Audio.PlaySoundFrontend(-1, "Faster_Click", "RESPAWN_ONLINE_SOUNDSET", true);
			WeaponConfigs[CurrentWeaponHash] = CurrentFiringMode;
			NotificationManager.ShowNotification("Weapon Firemode", Helpers.FormatString("Weapon switched to {0}", newFiringMode), ENotificationIcon.InfoSign);
		}
	}

	private bool IsWeaponIgnored(int weaponHash)
	{
		EWeaponCategory weaponCategory = WeaponHelpers.GetWeaponCategory((uint)weaponHash);
		return exemptedWeaponGroups.Contains(weaponCategory);
	}

	private bool CanWeaponUseBurstFire(int weaponHash)
	{
		EWeaponCategory weaponCategory = WeaponHelpers.GetWeaponCategory((uint)weaponHash);
		return allowedBurstWeaponGroups.Contains(weaponCategory) || allowedBurstWeapons.Contains((WeaponHash)weaponHash);
	}

	private bool CanWeaponUseSingleFire(int weaponHash)
	{
		return !blockSingleFireWeapons.Contains((WeaponHash)weaponHash);
	}

	// FIX: GTA actually does not deduct tazer ammo
	private int m_TaserAmmoToSubtract = 0;
	private Dictionary<EWeapons, int> m_LastAmmoData = new Dictionary<EWeapons, int>();
	private List<WeaponHash> m_lastWeaponData = new List<WeaponHash>();
}