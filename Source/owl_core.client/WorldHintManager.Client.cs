using System;

public delegate void WorldHintDrawDelegate();
public delegate void WorldHintInteractDelegate();

public static class WorldHintManager
{
	static WorldHintManager()
	{

	}

	public static void Init()
	{
		RageEvents.RAGE_OnTick_PerFrame += OnTick;
	}

	private static CWorldHint g_PendingWorldHint = new CWorldHint();

	private class CWorldHint
	{
		public CWorldHint()
		{
			Reset();
		}

		public void Set(float fDist, float fDistRatio, bool bHadHit, ConsoleKey a_Key, RAGE.Vector3 vecPos, string strMessage, WorldHintDrawDelegate a_DrawCallback, WorldHintInteractDelegate a_InteractCallback, bool bCallerWillDraw)
		{
			Distance = fDist;
			DistanceRatio = fDistRatio;
			HadHit = bHadHit;
			Key = a_Key;
			Position = vecPos;
			Message = strMessage;
			DrawCallback = a_DrawCallback;
			InteractCallback = a_InteractCallback;
			CallerWillDraw = bCallerWillDraw;
		}

		public void Reset()
		{
			Distance = 999999.0f;
			DistanceRatio = 0.0f;
			HadHit = false;
			Position = new RAGE.Vector3(0.0f, 0.0f, 0.0f);
			Message = String.Empty;
			DrawCallback = null;
			InteractCallback = null;
			Key = ConsoleKey.NoName;
		}

		public float Distance { get; set; }
		public float DistanceRatio { get; set; }
		public bool HadHit { get; set; }
		public ConsoleKey Key { get; set; }
		public RAGE.Vector3 Position { get; set; }
		public string Message { get; set; }
		public WorldHintDrawDelegate DrawCallback { get; set; }
		public WorldHintInteractDelegate InteractCallback { get; set; }
		public bool CallerWillDraw { get; set; }
	}

	public static void DrawExclusiveWorldHint(ConsoleKey keyCode, string strMessage, WorldHintDrawDelegate callbackOnDraw, WorldHintInteractDelegate callbackOnInteract, RAGE.Vector3 vecPos,
		uint dimension, bool bHideOnRaycast, bool bFadeOnRayCast, float fMaxDistance = 1.5f, RAGE.Vector3 vecRot = null, bool bStrongFont = false, bool bDoInfrontCheck = false, float fAngleForInfrontCheck = 1.5f, bool bAllowInVehicle = false, bool bCallerWillDraw = false)
	{
		if (!RAGE.Game.Cam.IsGameplayCamRendering())
		{
			return;
		}

		RAGE.Elements.Player localPlayer = RAGE.Elements.Player.LocalPlayer;

		if (localPlayer.Dimension != dimension)
		{
			return;
		}

		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		if (!bIsSpawned)
		{
			return;
		}

		if (!bAllowInVehicle && localPlayer.IsInAnyVehicle(true))
		{
			return;
		}

		if (!RAGE.Game.Cam.IsGameplayCamRendering())
		{
			return;
		}

		RAGE.Vector3 vecGameplayCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);
		float fDist = WorldHelper.GetDistance(vecPos, localPlayer.Position);

		bool bIsInCone = false;
		if (bDoInfrontCheck)
		{
			// TODO_CSHARP: Helper func
			RAGE.Vector3 vecConePos = new RAGE.Vector3(vecPos.X, vecPos.Y, vecPos.Z);
			float radians = (vecRot.Z) * (3.14f / 180.0f);
			vecConePos.X += (float)Math.Cos(radians) * (fMaxDistance);
			vecConePos.Y += (float)Math.Sin(radians) * (fMaxDistance);
			vecPos.Z = WorldHelper.GetGroundPosition(vecPos);
			vecConePos.Z = WorldHelper.GetGroundPosition(vecConePos);

			bIsInCone = localPlayer.IsInAngledArea(vecPos.X, vecPos.Y, vecPos.Z, vecConePos.X, vecConePos.Y, vecConePos.Z, fAngleForInfrontCheck, false, false, 0);
		}

		if (((bDoInfrontCheck && bIsInCone) || (!bDoInfrontCheck)) && fDist <= fMaxDistance && fDist <= g_PendingWorldHint.Distance)
		{

			CRaycastResult raycast = WorldHelper.RaycastFromTo(vecGameplayCamPos, vecPos, -1, -1);
			bool bHit = raycast.Hit;

			if (!bHideOnRaycast && !bFadeOnRayCast)
			{
				bHit = false;
			}

			if (!bHit || !bHideOnRaycast)
			{
				Vector2 vecScreenPos = GraphicsHelper.GetScreenPositionFromWorldPosition(vecPos);

				if (vecScreenPos.SetSuccessfully) // Used for on-screen check
				{
					if (bStrongFont)
					{
						fDist /= 2.0f;
					}

					float fDistRatio = 1.0f - (fDist / fMaxDistance);
					g_PendingWorldHint.Set(fDist, fDistRatio, bHit, keyCode, vecPos, strMessage, callbackOnDraw, callbackOnInteract, bCallerWillDraw);
				}
			}
		}
	}

	// TODO_CSHARP: Why does this goto zero zero sometimes?
	private static Vector2 m_vecLastDraw = null;

	private static void OnTick()
	{
		// process + render last frame message
		if (KeyBinds.CanProcessKeybinds())
		{
			if (KeyBinds.WasKeyJustReleased(g_PendingWorldHint.Key))
			{
				if (g_PendingWorldHint.InteractCallback != null)
				{
					g_PendingWorldHint.InteractCallback();
				}

				g_PendingWorldHint.Key = ConsoleKey.NoName;
			}
		}

		if (g_PendingWorldHint.Distance != 999999.0 && g_PendingWorldHint.Position != null)
		{
			int iRectAlpha = (int)(g_PendingWorldHint.DistanceRatio * (g_PendingWorldHint.HadHit ? 100 : 180));
			int iMainTextAlpha = (int)(g_PendingWorldHint.DistanceRatio * (g_PendingWorldHint.HadHit ? 100 : 255));
			Vector2 vecScreenPos = GraphicsHelper.GetScreenPositionFromWorldPosition(g_PendingWorldHint.Position);

			if (!vecScreenPos.SetSuccessfully)
			{
				vecScreenPos = m_vecLastDraw;
			}

			if (vecScreenPos != null && vecScreenPos.X <= 0 && vecScreenPos.Y <= 0)
			{
				if (m_vecLastDraw != null)
				{
					vecScreenPos = m_vecLastDraw;
				}
				else
				{
					return;
				}
				//RAGE.Chat.Output(Helpers.FormatString("0 x: {0} y: {1}", vecScreenPos.X, vecScreenPos.Y));
			}

			m_vecLastDraw = vecScreenPos;


			if (vecScreenPos != null)
			{
				RAGE.Game.Ui.BeginTextCommandWidth("STRING");
				RAGE.Game.Ui.AddTextComponentSubstringPlayerName("A");
				float fTextWidthSingleChar = RAGE.Game.Ui.EndTextCommandGetWidth((int)RAGE.Game.Font.ChaletComprimeCologne);

				if (g_PendingWorldHint.Key != ConsoleKey.NoName)
				{
					if (!g_PendingWorldHint.CallerWillDraw)
					{
						float fRectLeft = vecScreenPos.X - 0.03f - ((fTextWidthSingleChar * g_PendingWorldHint.Key.ToString().Length) / 10.0f);
						float fRectWidth = Math.Max(0.015f, 0.015f * (g_PendingWorldHint.Key.ToString().Length / 2.0f));
						float fTextLeft = vecScreenPos.X - 0.0309f - ((fTextWidthSingleChar * g_PendingWorldHint.Key.ToString().Length) / 10.0f);
						RAGE.Game.Graphics.DrawRect(fRectLeft, vecScreenPos.Y + 0.017f, fRectWidth, 0.03f, 0, 0, 0, iRectAlpha, 0);
						TextHelper.Draw2D(g_PendingWorldHint.Key.ToString(), fTextLeft, vecScreenPos.Y + 0.0035f, 0.5f, 255, 194, 15, iMainTextAlpha, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);
					}

					// If rendering a world hint, disable E key
					ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleHorn);
				}

				if (!g_PendingWorldHint.CallerWillDraw)
				{
					float fTextLeft = (vecScreenPos.X - 0.03f) + 0.010f + ((fTextWidthSingleChar * g_PendingWorldHint.Message.Length) / 10.0f);
					TextHelper.Draw2D(g_PendingWorldHint.Message, fTextLeft, vecScreenPos.Y, 0.5f, 209, 209, 209, iMainTextAlpha, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);
				}

				if (g_PendingWorldHint.DrawCallback != null)
				{
					g_PendingWorldHint.DrawCallback();
				}
			}
		}

		g_PendingWorldHint.Reset();
	}
}