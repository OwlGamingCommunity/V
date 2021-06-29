using System;
using System.Collections.Generic;

public class WeaponSelector
{
	public WeaponSelector()
	{
		UIEvents.FadeOutWeaponSelector += OnFadeOutWeaponSelector;

		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_Melee, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.Melee); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_Handguns, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.Handgun); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_SMG, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.SMG); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_Rifle, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.Rifle); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_MachineGun, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.MachineGun); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_Shotgun, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.Shotgun); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_Sniper, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.Sniper); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_RangedProjectile, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.RangedProjectile); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_Throwable, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.Throwable); });
		ScriptControls.SubscribeToControl(EScriptControlID.ShowWeaponSelector_Special, (EControlActionType actionType) => { ShowWeaponSelector(EWeaponCategory.Special); });
	}

	private bool m_bVisible = false;
	private EWeaponCategory m_CurrentSlot = EWeaponCategory.Melee;
	private WeaponHash m_SwitchingToWeapon = 0;

	private void OnFadeOutWeaponSelector()
	{
		m_bVisible = false;
		m_CurrentSlot = EWeaponCategory.Melee;
		m_SwitchingToWeapon = 0;
	}

	private Dictionary<EWeaponCategory, int> m_CurrentWeaponIndices = new Dictionary<EWeaponCategory, int>()
	{
		{ EWeaponCategory.Melee, 0},
		{ EWeaponCategory.Handgun, 0},
		{ EWeaponCategory.SMG, 0},
		{ EWeaponCategory.Rifle, 0},
		{ EWeaponCategory.MachineGun, 0},
		{ EWeaponCategory.Shotgun, 0},
		{ EWeaponCategory.Sniper, 0},
		{ EWeaponCategory.RangedProjectile, 0},
		{ EWeaponCategory.Throwable, 0},
		{ EWeaponCategory.Special, 0},
	};

	private void ShowWeaponSelector(EWeaponCategory slot)
	{
		bool bCuffed = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.CUFFED);
		if (KeyBinds.IsChatInputVisible() || bCuffed)
		{
			return;
		}

		// We're we already visible but with a different slot? Reset flag so we don't increment weapon slot
		if (m_CurrentSlot != slot)
		{
			m_bVisible = false;
		}

		m_CurrentSlot = slot;
		HUD.GetHudBrowser().Execute("ResetWeaponSelector");

		List<WeaponHash> weaponsInCurrentSlot = new List<WeaponHash>();

		// If melee slot, add fists
		if (slot == EWeaponCategory.Melee)
		{
			weaponsInCurrentSlot.Add(WeaponHash.Unarmed);
		}

		foreach (WeaponHash weaponHash in Enum.GetValues(typeof(WeaponHash)))
		{
			if (weaponHash != 0 && weaponHash != WeaponHash.Unarmed)
			{
				bool hasGotWeapon = RAGE.Elements.Player.LocalPlayer.HasGotWeapon((uint)weaponHash, false);

				if (hasGotWeapon)
				{
					// TODO_CSHARP: Safety around dictionaries
					var weaponCategory = WeaponHelpers.WeaponCategories[weaponHash];

					if (weaponCategory == slot)
					{
						weaponsInCurrentSlot.Add(weaponHash);
					}
				}
			}
		}

		// Increment current (if already visible, first click just shows the UI)
		var currentIndex = m_CurrentWeaponIndices[slot];
		if (m_bVisible)
		{
			currentIndex++;
			if (currentIndex >= weaponsInCurrentSlot.Count)
			{
				currentIndex = 0;
			}
			m_CurrentWeaponIndices[slot] = currentIndex;
		}

		// Add from list
		bool bFoundNewWeapon = false;
		WeaponHash newWeaponHash = 0;
		int i = 0;
		foreach (WeaponHash weaponHash in weaponsInCurrentSlot)
		{
			int ammo = (slot == EWeaponCategory.Melee) ? -1 : RAGE.Elements.Player.LocalPlayer.GetAmmoInWeapon((uint)weaponHash);

			int weaponIndex = WeaponHelpers.GetWeaponItemIDFromHash((uint)weaponHash);

			HUD.GetHudBrowser().Execute("AddWeaponSelectorItem", weaponIndex, ammo);

			if (i == currentIndex)
			{
				bFoundNewWeapon = true;
				newWeaponHash = weaponHash;
			}

			++i;
		}

		HUD.GetHudBrowser().Execute("CommitWeaponSelectorItems", currentIndex);
		RAGE.Game.Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FREEMODE_SOUNDSET", true);

		if (bFoundNewWeapon)
		{
			RAGE.Elements.Player.LocalPlayer.SetCurrentWeapon((uint)newWeaponHash, true);
			m_SwitchingToWeapon = newWeaponHash;
		}

		m_bVisible = true;
	}
}