using System;
using System.Collections.Generic;

public class PDHelicopterHUD
{
	private CGUIPDHelicopterHUD m_PDHelicopterHUDUI = new CGUIPDHelicopterHUD(() => { });
	private bool m_bRenderHelicopterHUD = false;

	private const string g_SpriteDictName = "helicopterhud";

	private bool bShow_UnoccupiedVehicles = false;
	private bool bShow_OccupiedVehicles = false;
	private bool bShow_MovingVehiclesOnly = false;
	private bool bShow_People = false;

	public PDHelicopterHUD()
	{
		RageEvents.RAGE_OnTick_OncePerSecond += RefreshRenderPool;
		RageEvents.RAGE_OnRender += OnRender;
		NetworkEvents.ExitVehicleReal += OnLeaveVehicle;

		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ChangeCharacterApproved += CloseHUD;

		if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(g_SpriteDictName))
		{
			RAGE.Game.Graphics.RequestStreamedTextureDict(g_SpriteDictName, true);
		}
	}

	~PDHelicopterHUD()
	{

	}

	// UI Functions
	public void OnToggleVehiclesUnoccupied(bool bEnabled)
	{
		bShow_UnoccupiedVehicles = bEnabled;
		RefreshRenderPool(); // refresh immediately
	}

	public void OnToggleVehiclesOccupied(bool bEnabled)
	{
		bShow_OccupiedVehicles = bEnabled;
		RefreshRenderPool(); // refresh immediately
	}

	public void OnToggleMovingVehiclesOnly(bool bEnabled)
	{
		bShow_MovingVehiclesOnly = bEnabled;
		RefreshRenderPool(); // refresh immediately
	}


	public void OnTogglePeople(bool bEnabled)
	{
		bShow_People = bEnabled;
		RefreshRenderPool(); // refresh immediately
	}

	public void OnToggleNVG(bool bEnabled)
	{
		RAGE.Game.Graphics.SetNightvision(bEnabled);
	}

	public void OnToggleThermal(bool bEnabled)
	{
		RAGE.Game.Graphics.SetSeethrough(bEnabled);
	}
	// END UI Functions

	private void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		bool bIsPoliceVehicle = DataHelper.GetEntityData<bool>(vehicle, EDataNames.IS_POLICE_VEHICLE);
		bool bCanSeeDevice = (seatId == (int)EVehicleSeat.Driver || seatId == (int)EVehicleSeat.FrontPassenger);

		// Don't let rear seat passengers see the device, only front passenger + driver
		if (bIsPoliceVehicle && bCanSeeDevice)
		{
			if (vehicle.GetClass() == (int)EVehicleClass.VehicleClass_Helicopters)
			{
				// init
				bShow_UnoccupiedVehicles = true;
				bShow_OccupiedVehicles = true;
				bShow_People = true;
				bShow_MovingVehiclesOnly = false;
				m_PDHelicopterHUDUI.SetDefaults(bShow_UnoccupiedVehicles, bShow_OccupiedVehicles, bShow_MovingVehiclesOnly, bShow_People, false, false);

				m_bRenderHelicopterHUD = true;
				m_PDHelicopterHUDUI.SetVisible(true, false, false);
			}
		}
	}

	private void OnLeaveVehicle(RAGE.Elements.Vehicle vehicle)
	{
		CloseHUD();
	}

	private void CloseHUD()
	{
		m_bRenderHelicopterHUD = false;
		m_PDHelicopterHUDUI.SetVisible(false, false, false);
		RAGE.Game.Graphics.SetNightvision(false);
		RAGE.Game.Graphics.SetSeethrough(false);
	}

	private List<RAGE.Elements.Entity> m_lstEntitiesToRender = new List<RAGE.Elements.Entity>();
	private void RefreshRenderPool()
	{
		if (m_bRenderHelicopterHUD && RAGE.Game.Graphics.HasStreamedTextureDictLoaded(g_SpriteDictName))
		{
			m_lstEntitiesToRender.Clear();

			foreach (var vehicle in OptimizationCachePool.StreamedInVehicles())
			{
				if (vehicle.IsOnScreen() && vehicle.GetClass() != (int)EVehicleClass.VehicleClass_Helicopters && vehicle.GetClass() != (int)EVehicleClass.VehicleClass_Planes && vehicle.Dimension == RAGE.Elements.Player.LocalPlayer.Dimension)
				{
					bool bVehicleHasOccupants = !vehicle.IsSeatFree(-1, 0);

					if ((bVehicleHasOccupants && bShow_OccupiedVehicles) || (!bVehicleHasOccupants && bShow_UnoccupiedVehicles))
					{
						if ((bShow_MovingVehiclesOnly && vehicle.GetSpeed() > 0.0f) || !bShow_MovingVehiclesOnly)
						{
							m_lstEntitiesToRender.Add(vehicle);
						}
					}
				}
			}

			if (bShow_People)
			{
				foreach (var player in RAGE.Elements.Entities.Players.Streamed)
				{
					if (player != RAGE.Elements.Player.LocalPlayer && player.IsOnScreen() && player.Dimension == RAGE.Elements.Player.LocalPlayer.Dimension)
					{
						m_lstEntitiesToRender.Add(player);
					}
				}
			}
		}
	}

	private void OnRender()
	{
		foreach (var entity in m_lstEntitiesToRender)
		{
			DrawHeliOverview(entity);
		}
	}

	private void DrawHeliOverview(RAGE.Elements.Entity entity)
	{
		const float fMaxDist = 5000.0f;

		float fDistPlayerPos = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, entity.Position);
		float fDistCameraPos = WorldHelper.GetDistance(CameraManager.GetCameraPosition(ECameraID.GAME), entity.Position);

		// Use whichever vector is closer for our render viewport
		float fDist = Math.Max(fDistPlayerPos, fDistCameraPos);

		if (fDist <= fMaxDist)
		{
			Vector2 vecScreenPos = GraphicsHelper.GetScreenPositionFromWorldPosition(entity.Position);

			if (vecScreenPos.SetSuccessfully)
			{
				CRaycastResult raycast = WorldHelper.RaycastFromTo(RAGE.Elements.Player.LocalPlayer.Position, entity.Position, RAGE.Elements.Player.LocalPlayer.Handle, 1);

				if (!raycast.Hit || (RAGE.Elements.Player.LocalPlayer.Vehicle != null && raycast.EntityHit == RAGE.Elements.Player.LocalPlayer.Vehicle))
				{
					Vector2 vecResolution = GraphicsHelper.GetScreenResolution();
					float scaleX = (128 / vecResolution.X);
					float scaleY = (128 / vecResolution.Y);

					RAGE.Game.Graphics.DrawSprite(g_SpriteDictName, "hud_outline", vecScreenPos.X, vecScreenPos.Y, scaleX, scaleY, 0, 0, 255, 0, 200, 0);

					List<string> lstDataToRender = new List<string>();
					if (entity.Type == RAGE.Elements.Type.Vehicle)
					{
						var vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(entity.Model);
						if (vehicleDef != null)
						{
							RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;

							bool bVehicleHasOccupants = !vehicle.IsSeatFree(-1, 0);

							lstDataToRender.Add(Helpers.FormatString("Vehicle ({0} {1}) - {2}", vehicleDef.Manufacturer, vehicleDef.Name, bVehicleHasOccupants ? "Occupied" : "Not Occupied"));

							float fSpeedMps = vehicle.GetSpeed();
							float fSpeedMetersPerHour = (fSpeedMps * 3600.9f);
							float fSpeedMilesPerHour = (float)Math.Ceiling((fSpeedMetersPerHour / 1609.344f) * 1.25f);
							lstDataToRender.Add(Helpers.FormatString("Speed {0} mph", fSpeedMilesPerHour));

							// render vehicle color
							int r = 0;
							int g = 0;
							int b = 0;
							vehicle.GetCustomPrimaryColour(ref r, ref g, ref b);
							int r2 = 0;
							int g2 = 0;
							int b2 = 0;
							vehicle.GetCustomSecondaryColour(ref r2, ref g2, ref b2);

							RAGE.Game.Graphics.DrawRect(vecScreenPos.X - 0.04f, vecScreenPos.Y, 0.02f, 0.02f, r, g, b, 255, 0);
							RAGE.Game.Graphics.DrawRect(vecScreenPos.X - 0.04f, vecScreenPos.Y + 0.02f, 0.02f, 0.02f, r2, g2, b2, 255, 0);
						}
					}
					else if (entity.Type == RAGE.Elements.Type.Player)
					{
						lstDataToRender.Add("Person");
						lstDataToRender.Add("Male");
					}

					const float incrementPerString = 0.02f;
					int counter = 0;
					foreach (string strToRender in lstDataToRender)
					{
						float fOffset = incrementPerString * counter;
						TextHelper.Draw2D(strToRender, vecScreenPos.X, vecScreenPos.Y + (2 * incrementPerString) + fOffset, 0.4f, new RAGE.RGBA(0, 255, 0), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, false);

						++counter;
					}
				}
			}
		}
	}
}