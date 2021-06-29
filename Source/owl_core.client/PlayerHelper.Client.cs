using System;
using RAGE;
public static class PlayerHelper
{
	private static uint m_CachedSkin = 0;
	private static uint m_CachedDimension = 0;
	private static RAGE.Vector3 vecLastCheck = new RAGE.Vector3();
	private static bool m_bIsFrozen = false;

	static PlayerHelper()
	{

	}

	public static void Init()
	{
		NetworkEvents.Freeze += OnFreeze;
		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ExitVehicleReal += OnExitVehicle;

		NetworkEvents.CharacterSelectionApproved += OnSpawn;

		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.SafeTeleport += OnSafeTeleport;

		m_CachedSkin = RAGE.Elements.Player.LocalPlayer.Model;
		m_CachedDimension = RAGE.Elements.Player.LocalPlayer.Dimension;
		vecLastCheck = RAGE.Elements.Player.LocalPlayer.Position;

		NetworkEvents.SetPlayerVisible += (RAGE.Elements.Player player, bool bVisible) =>
		{
			try
			{
				if (player != null && player.Handle != RAGE.Elements.Entity.InvalidId)
				{
					player.SetAlpha(bVisible ? 255 : 0, false);
				}
			}
			catch
			{

			}
		};

		RageEvents.RAGE_OnTick_OncePerSecond += () =>
		{
			float fDistTravelled = (RAGE.Elements.Player.LocalPlayer.Position - vecLastCheck).Length();

			if (fDistTravelled > 200.0f)
			{
				NetworkEvents.SendLocalEvent_LocalPlayerStreamInNewArea(vecLastCheck, RAGE.Elements.Player.LocalPlayer.Position);
				vecLastCheck = RAGE.Elements.Player.LocalPlayer.Position;
			}
		};
	}

	private static void OnTick()
	{
		// Stop health regen
		RAGE.Game.Player.SetPlayerHealthRechargeMultiplier(0.0f);

		// Detect skin change
		if (RAGE.Elements.Player.LocalPlayer.Model != m_CachedSkin)
		{
			NetworkEvents.SendLocalEvent_LocalPlayerModelChanged(m_CachedSkin, RAGE.Elements.Player.LocalPlayer.Model);
			m_CachedSkin = RAGE.Elements.Player.LocalPlayer.Model;
		}

		// Detect dimension change
		if (RAGE.Elements.Player.LocalPlayer.Dimension != m_CachedDimension)
		{
			NetworkEvents.LocalPlayerDimensionChanged(m_CachedDimension, RAGE.Elements.Player.LocalPlayer.Dimension);
			m_CachedDimension = RAGE.Elements.Player.LocalPlayer.Dimension;
		}

		// We could while() and RAGE.Game.System.Wait, but this is properly async
		if (m_bPendingSafeTeleport)
		{
			HUD.SetLoadingMessage("Loading World");

			//if (RAGE.Game.Entity.HasCollisionLoadedAroundEntity(RAGE.Elements.Player.LocalPlayer.Handle) || m_bPendingSafeTeleportExpired)
			{
				// Calculate ground pos
				m_vecPendingSafeTeleport.Z = WorldHelper.GetGroundPosition(m_vecPendingSafeTeleport) + 1.1f;
				RAGE.Elements.Player.LocalPlayer.Position = m_vecPendingSafeTeleport;

				// Unfreeze player
				RAGE.Elements.Player.LocalPlayer.FreezePosition(false);

				m_bPendingSafeTeleport = false;
				m_vecPendingSafeTeleport = null;
			}
		}
	}

	private static bool m_bPendingSafeTeleport = false;
	private static Vector3 m_vecPendingSafeTeleport = null;
	public static void OnSafeTeleport(float x, float y, float z)
	{
		RAGE.Vector3 vecPos = new RAGE.Vector3(x, y, z);

		// Freeze so we don't fall through the ground
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

		// Set pos nearby so we can get ground (col must be loaded in)
		RAGE.Elements.Player.LocalPlayer.Position = vecPos;

		m_bPendingSafeTeleport = true;
		m_vecPendingSafeTeleport = vecPos;
	}

	private static void OnSpawn()
	{
		InitPlayerStats();
	}

	private static void InitPlayerStats()
	{
		RAGE.Game.Stats.StatSetInt(HashHelper.GetHashUnsigned("SP0_STAMINA"), 100, false);
		RAGE.Game.Stats.StatSetInt(HashHelper.GetHashUnsigned("SP0_STRENGTH"), 100, false);
		RAGE.Game.Stats.StatSetInt(HashHelper.GetHashUnsigned("SP0_LUNG_CAPACITY"), 100, false);
		RAGE.Game.Stats.StatSetInt(HashHelper.GetHashUnsigned("SP0_WHEELIE_ABILITY"), 100, false);
		RAGE.Game.Stats.StatSetInt(HashHelper.GetHashUnsigned("SP0_FLYING_ABILITY"), 100, false);
		RAGE.Game.Stats.StatSetInt(HashHelper.GetHashUnsigned("SP0_SHOOTING_ABILITY"), 100, false);
		RAGE.Game.Stats.StatSetInt(HashHelper.GetHashUnsigned("SP0_STEALTH_ABILITY"), 100, false);
	}

	private static void OnFreeze(bool bFreeze)
	{
		m_bIsFrozen = bFreeze;

		RAGE.Elements.Player.LocalPlayer.FreezePosition(bFreeze);

		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			RAGE.Elements.Player.LocalPlayer.Vehicle.FreezePosition(bFreeze);
		}
	}

	private static void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatID)
	{
		if (m_bIsFrozen)
		{
			vehicle.FreezePosition(true);
		}
	}

	private static void OnExitVehicle(RAGE.Elements.Vehicle vehicle)
	{
		if (m_bIsFrozen)
		{
			vehicle.FreezePosition(false);
		}
	}

	public static RAGE.Elements.Player GetLocalPlayer()
	{
		return RAGE.Elements.Player.LocalPlayer;
	}
	
	public static float GetLocalPlayerVehicleMilesPerHour()
	{
		float fSpeedMps = GetLocalPlayer().Vehicle.GetSpeed();
		float fSpeedMetersPerHour = (fSpeedMps * 3600.9f);
		float fSpeedMilesPerHour = (float)Math.Ceiling((fSpeedMetersPerHour / 1609.344f) * 1.25f);
		return fSpeedMilesPerHour;
	}

	public static RAGE.Vector3 GetLocalPlayerPosition()
	{
		return GetLocalPlayer().Position;
	}

	public static uint GetLocalPlayerDimension()
	{
		return GetLocalPlayer().Dimension;
	}

	public static RAGE.Elements.Vehicle GetLocalPlayerVehicle()
	{
		return GetLocalPlayer().Vehicle;
	}
}