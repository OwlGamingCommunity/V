using System;

// This fixes weapons pulling out when changing model / recreating ped
public class GTAWeaponSwitchFix
{
	private uint m_CachedWeapon = 0;
	private WeakReference<ClientTimer> m_FixTimer = new WeakReference<ClientTimer>(null);

	public GTAWeaponSwitchFix()
	{
		RageEvents.RAGE_OnTick_PerFrame += () =>
		{
			int weaponHash = 0;
			if (!RAGE.Elements.Player.LocalPlayer.GetCurrentWeapon(ref weaponHash, false))
			{
				m_CachedWeapon = unchecked((uint)weaponHash);
			}
		};

		NetworkEvents.CharacterSelectionApproved += () =>
		{
			uint unarmedHash = HashHelper.GetHashUnsigned("WEAPON_UNARMED");
			SetFixTimer(unarmedHash);
		};

		NetworkEvents.LocalPlayerModelChanged += (uint oldModel, uint newModel) =>
		{
			SetFixTimer(m_CachedWeapon);
		};
	}

	private void SetFixTimer(uint weaponHashToRestore)
	{
		if (m_FixTimer.Instance() != null)
		{
			ClientTimerPool.MarkTimerForDeletion(ref m_FixTimer);
		}

		m_FixTimer = ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			// restore our weapon
			RAGE.Elements.Player.LocalPlayer.SetCurrentWeapon(weaponHashToRestore, true);
		}, 50, 40);
	}
}