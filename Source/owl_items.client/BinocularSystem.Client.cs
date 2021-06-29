using RAGE;
using System;
using System.Collections.Generic;

public class BinocularSystem
{
	const float FOV_MAX = 60f;
	const float FOV_MIN = 7.0f;
	const float OPTICAL_FOV_MIN = 20f;
	const float ZOOM_SPEED = 0.50f;
	const float SPEED_LR = 8.0f;
	const float SPEED_UD = 8.0f;

	private float m_FOV = (FOV_MAX + FOV_MIN) * 0.5f;
	private int m_VisionState = 0; // 0 is binoculars not in use, 1 is optical, 2 is nightvision, 3 thermalvision
	private bool m_bBinocularsToggled = false;
	private bool m_bEnteredBinocularsView = false;
	private EBinocularsType m_eBinocularsType = EBinocularsType.None;
	private Vector3 m_OffsetVecPos;
	private Vector3 m_VecRot = new Vector3(0f, 0f, 0f);

	private Vector3 m_VecRotFromMouse = new Vector3(0f, 0f, 0f);

	public BinocularSystem()
	{
		// EVENTS
		RageEvents.RAGE_OnEntityStreamIn += OnEntityStreamIn;
		RageEvents.RAGE_OnEntityStreamOut += OnEntityStreamOut;
		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_PerFrame += OnUpdate;

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleBinocularView, ToggleBinocularsView);
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleBinocularFx, ToggleBinocularFx);

		//Register camera to be activated by player.
		CameraManager.RegisterCamera(ECameraID.BINOCULARS, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));


		ClientTimerPool.CreateTimer(UpdateAttachments, 200);
	}

	private void OnRender()
	{
		if (HUD.IsVisible())
		{
			if (m_bBinocularsToggled && !m_bEnteredBinocularsView)
			{
				TextHelper.Draw2D("Press E to enter Binocular View", 0.5f, 0.90f, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			}
			else if (m_bBinocularsToggled && m_bEnteredBinocularsView)
			{
				TextHelper.Draw2D("Press E to exit Binocular View", 0.5f, 0.90f, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			}

			if (m_bEnteredBinocularsView && m_eBinocularsType == EBinocularsType.Advanced && m_VisionState == 1)
			{
				TextHelper.Draw2D("Press X to toggle on Night Vision", 0.5f, 0.95f, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			}
			else if (m_bEnteredBinocularsView && m_eBinocularsType == EBinocularsType.Advanced && m_VisionState == 2)
			{
				TextHelper.Draw2D("Press X to toggle off Night Vision", 0.5f, 0.95f, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			}
		}
	}

	private void OnUpdate()
	{
		if (m_bEnteredBinocularsView)
		{
			if (KeyBinds.IsChatInputVisible())
			{
				return;
			}

			//Disable mouse rotation so we can use it for the camera rotation
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Sprint);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Jump);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MoveLeftOnly);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MoveRightOnly);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MoveUpOnly);
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MoveDownOnly);

			HandleZoomFunctions();
		}
	}

	private void HandleZoomFunctions()
	{
		if (KeyBinds.IsChatInputVisible())
		{
			return;
		}

		if (m_bEnteredBinocularsView)
		{
			if (m_eBinocularsType != EBinocularsType.Regular)
			{
				if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.Jump))
				{
					m_FOV = Math.Max(m_FOV - ZOOM_SPEED, FOV_MIN);
				}
			}
			else
			{
				//Different max zoom for a regular binoculars
				if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.Jump))
				{
					m_FOV = Math.Max(m_FOV - ZOOM_SPEED, OPTICAL_FOV_MIN);
				}
			}

			if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.Sprint))
			{
				m_FOV = Math.Min(m_FOV + ZOOM_SPEED, FOV_MAX);
			}




			float CurrentFOV = CameraManager.GetCameraFOV(ECameraID.BINOCULARS);

			if (Math.Abs(m_FOV - CurrentFOV) < 0.1)
			{
				m_FOV = CurrentFOV;
			}

			if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.MoveLeftOnly))
			{
				m_VecRot.Z += 0.35f;
				CameraManager.UpdateCamera(ECameraID.BINOCULARS, m_OffsetVecPos, m_VecRot, m_VecRot);
			}

			if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.MoveRightOnly))
			{
				m_VecRot.Z -= 0.35f;
				CameraManager.UpdateCamera(ECameraID.BINOCULARS, m_OffsetVecPos, m_VecRot, m_VecRot);
			}


			if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.MoveUpOnly))
			{
				m_VecRot.X += 0.35f;
				CameraManager.UpdateCamera(ECameraID.BINOCULARS, m_OffsetVecPos, m_VecRot, m_VecRot);
			}

			if (RAGE.Game.Pad.IsDisabledControlPressed(0, (int)RAGE.Game.Control.MoveDownOnly))
			{
				m_VecRot.X -= 0.35f;
				CameraManager.UpdateCamera(ECameraID.BINOCULARS, m_OffsetVecPos, m_VecRot, m_VecRot);
			}


			// X and Y limit

			if (m_VecRot.X >= 90.0f)
			{
				m_VecRot.X -= 1.0f;
			}

			if (m_VecRot.X <= -90.0f)
			{
				m_VecRot.X += 1.0f;
			}


			// Set the rotation of the person controlling the binoculars
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, m_VecRot.Z, 0, true);
			m_OffsetVecPos = MathHelpers.OffsetPosition(RAGE.Elements.Player.LocalPlayer.Position, m_VecRot.Z, 1.0f);
			CameraManager.UpdateCamera(ECameraID.BINOCULARS, m_OffsetVecPos, m_OffsetVecPos, m_VecRot, false, 0, CurrentFOV + (m_FOV - CurrentFOV) * 0.05f);
		}
	}

	// Leaving this here for when someone gets mouse input to work.
	/*private void CheckInputRotation(float zoomValue)
    {
        float rightAxisX = RAGE.Game.Pad.GetDisabledControlNormal(0, (int)RAGE.Game.Control.CursorX);
        float rightAxisY = RAGE.Game.Pad.GetDisabledControlNormal(0, (int)RAGE.Game.Control.CursorY);

        Vector3 camRot = CameraManager.GetCameraRotation(ECameraID.BINOCULARS);

        if (rightAxisX != 0.0f || rightAxisY != 0.0f)
        {
            m_VecRotFromMouse.Z = camRot.Z + rightAxisX * -1.0f * (SPEED_UD) * (zoomValue + 0.1f);
            m_VecRotFromMouse.X = Math.Max(Math.Min(20.0f, camRot.X + rightAxisY * -1.0f * (SPEED_LR) * (zoomValue + 0.1f)), -89.5f);
        }
    }*/

	private void OnEntityStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			UpdateBinocularsAttachment((RAGE.Elements.Player)entity);
		}
	}

	private void OnEntityStreamOut(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			UpdateBinocularsAttachment((RAGE.Elements.Player)entity);
		}
	}

	//TODO_HELPER Create a helper function for this
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> g_DictBinocularsAttachments = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();

	private void UpdateBinocularsAttachment(RAGE.Elements.Player player)
	{
		bool BinocularToggled = DataHelper.GetEntityData<bool>(player, EDataNames.BINOCULARS);

		if (player == RAGE.Elements.Player.LocalPlayer)
		{
			m_bBinocularsToggled = BinocularToggled;
		}

		if (BinocularToggled)
		{
			if (!g_DictBinocularsAttachments.ContainsKey(player))
			{
				uint hash = HashHelper.GetHashUnsigned("prop_binoc_01");
				AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
				{
					g_DictBinocularsAttachments.Add(player, new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension));
				});
			}

			if (g_DictBinocularsAttachments.ContainsKey(player))
			{
				RAGE.Game.Entity.AttachEntityToEntity(g_DictBinocularsAttachments[player].Handle, player.Handle, player.GetBoneIndex(28422), 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true, true, false, true, 0, true);
			}
		}
		else
		{
			if (g_DictBinocularsAttachments.ContainsKey(player))
			{
				g_DictBinocularsAttachments[player].Destroy();
				g_DictBinocularsAttachments.Remove(player);
			}
		}
	}

	private void UpdateAttachments(object[] parameters)
	{
		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			UpdateBinocularsAttachment(player);
		}
	}

	private void ToggleBinocularFx(EControlActionType actionType)
	{
		if (!m_bEnteredBinocularsView)
		{
			return;
		}

		if (m_eBinocularsType == EBinocularsType.Advanced)
		{
			if (m_VisionState == 1)
			{
				m_VisionState = 2;
				RAGE.Game.Graphics.SetNightvision(true);
			}
			else if (m_VisionState == 2)
			{
				m_VisionState = 1;
				RAGE.Game.Graphics.SetNightvision(false);
			}
		}
	}

	private void ToggleBinocularsView(EControlActionType actionType)
	{
		EBinocularsType binocularsType = DataHelper.GetEntityData<EBinocularsType>(RAGE.Elements.Player.LocalPlayer, EDataNames.BINOCULARS_TYPE);
		if (!m_bEnteredBinocularsView && m_bBinocularsToggled && binocularsType != EBinocularsType.None)
		{
			if (binocularsType == EBinocularsType.Regular)
			{
				m_eBinocularsType = EBinocularsType.Regular;
				ScaleformManager.RequestScaleform("BINOCULARS", 255, 255, 255, 255, FullScreen: true);
				m_VisionState = 1;
			}
			else if (binocularsType == EBinocularsType.Advanced)
			{
				m_eBinocularsType = EBinocularsType.Advanced;
				ScaleformManager.RequestScaleform("HELI_CAM", 255, 255, 255, 255, FullScreen: true, isCustom: true, customScaleformFunctionName: "SET_CAM_LOGO");
				m_VisionState = 1;
			}
			else if (binocularsType == EBinocularsType.ThermalOnly)
			{
				m_eBinocularsType = EBinocularsType.ThermalOnly;
				ScaleformManager.RequestScaleform("HELI_CAM", 255, 255, 255, 255, FullScreen: true, isCustom: true, customScaleformFunctionName: "SET_CAM_LOGO");
				m_VisionState = 3;
				RAGE.Game.Graphics.SetSeethrough(true);
			}


			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

			Vector3 PlayerVecPos = RAGE.Elements.Player.LocalPlayer.Position;
			float Playerheading = RAGE.Elements.Player.LocalPlayer.GetHeading();
			m_VecRot.Z = Playerheading;

			m_OffsetVecPos = MathHelpers.OffsetPosition(PlayerVecPos, Playerheading, 1.0f);
			RAGE.Game.Cam.AttachCamToEntity((int)ECameraID.BINOCULARS, RAGE.Elements.Player.LocalPlayer.Handle, 0.0f, 0.0f, 1.0f, true);


			CameraManager.UpdateCamera(ECameraID.BINOCULARS, m_OffsetVecPos, RAGE.Elements.Player.LocalPlayer.Position, m_VecRot, false, 0, m_FOV);

			CameraManager.ActivateCamera(ECameraID.BINOCULARS);
			ScaleformManager.PushAndPopScaleforms();
			m_bEnteredBinocularsView = true;
		}
		else
		{
			RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
			m_bEnteredBinocularsView = false;
			m_VisionState = 0;
			RAGE.Game.Graphics.SetNightvision(false);
			RAGE.Game.Graphics.SetSeethrough(false);
			ScaleformManager.DeactiveAnyScaleform();
			CameraManager.DeactiveAnyCamera();
		}
	}
}
