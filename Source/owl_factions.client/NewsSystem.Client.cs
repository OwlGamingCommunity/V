using RAGE;
using System.Collections.Generic;

public class NewsSystem
{
	const float g_fDistThreshold = 3.0f;
	private RAGE.Elements.Player m_CamOperator;

	private RAGE.Elements.MapObject m_NewsCamObject;

	private bool m_bIsCamBeingOperated = false;
	private bool m_bIsOperatingCam = false;
	private bool m_bJoinedTvBroadcast = false;
	private Vector3 m_NewsCamRotVec = new Vector3();
	private Vector3 m_NewsCamPosVec = new Vector3();

	private float m_MaxRotLeft = 0f;
	private float m_MaxRotRight = 0f;

	private string m_StrNewsMsg;

	public NewsSystem()
	{
		NetworkEvents.JoinTvBroadcast += OnJoinTvBroadcast;
		NetworkEvents.LeaveTvBroadcast += OnLeaveTvBroadcast;
		NetworkEvents.RemoveAsCamOperator += RemovePlayerAsCamOperator;
		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_HighFrequency += OnUpdate;

		NetworkEvents.SendBreakingNewsMessage += BreakingText;

		RageEvents.RAGE_OnEntityStreamIn += OnEntityStreamIn;
		RageEvents.RAGE_OnEntityStreamOut += OnEntityStreamOut;
		ClientTimerPool.CreateTimer(UpdateAttachments, 200);

		CameraManager.RegisterCamera(ECameraID.NEWS, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
	}

	private void OnRender()
	{
		RAGE.Elements.MapObject nearestNewsCamera = GetNearestNewsCamera();
		if (nearestNewsCamera != null && !m_bIsCamBeingOperated)
		{
			WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Operate News Camera", null, OnOperateNewsCamera, nearestNewsCamera.Position, nearestNewsCamera.Dimension, false, false, g_fDistThreshold);
		}
		else if (nearestNewsCamera != null && m_bIsCamBeingOperated && m_CamOperator == RAGE.Elements.Player.LocalPlayer)
		{
			WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Stop operating News Camera", null, RemovePlayerAsCamOperator, nearestNewsCamera.Position, nearestNewsCamera.Dimension, false, false, g_fDistThreshold);
		}
		else if (nearestNewsCamera != null && m_bIsCamBeingOperated && m_CamOperator != RAGE.Elements.Player.LocalPlayer)
		{
			WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.DummyNone), "Camera is being operated", null, null, nearestNewsCamera.Position, nearestNewsCamera.Dimension, false, false, g_fDistThreshold);
		}

		if (m_bJoinedTvBroadcast)
		{
			BreakingText(m_StrNewsMsg);
		}
	}

	private void OnEntityStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			UpdateAttachment((RAGE.Elements.Player)entity);
		}
	}

	private void OnEntityStreamOut(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			UpdateAttachment((RAGE.Elements.Player)entity);
		}
	}

	// TODO_HELPER: Create a helper function for this
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> g_DictBoomMikeAttachments = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> g_DictHandCamAttachments = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> g_DictHandMikeAttachments = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();

	private void UpdateAttachment(RAGE.Elements.Player player)
	{
		bool boomMicState = DataHelper.GetEntityData<bool>(player, EDataNames.NEWS_BOOM_MIC);
		bool handCamState = DataHelper.GetEntityData<bool>(player, EDataNames.NEWS_CAM_HAND);
		bool handMicState = DataHelper.GetEntityData<bool>(player, EDataNames.NEWS_MIC);

		if (boomMicState)
		{
			if (!g_DictBoomMikeAttachments.ContainsKey(player))
			{
				uint hash = HashHelper.GetHashUnsigned("prop_v_bmike_01");
				AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
				{
					g_DictBoomMikeAttachments.Add(player, new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension));
				});
			}

			if (g_DictBoomMikeAttachments.ContainsKey(player))
			{
				RAGE.Game.Entity.AttachEntityToEntity(g_DictBoomMikeAttachments[player].Handle, player.Handle, player.GetBoneIndex(28422), -0.08f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true, false, false, false, 0, true);
			}
		}
		else if (handCamState)
		{
			if (!g_DictHandCamAttachments.ContainsKey(player))
			{
				uint hash = HashHelper.GetHashUnsigned("prop_v_cam_01");
				AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
				{
					g_DictHandCamAttachments.Add(player, new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension));
				});
			}

			if (g_DictHandCamAttachments.ContainsKey(player))
			{
				RAGE.Game.Entity.AttachEntityToEntity(g_DictHandCamAttachments[player].Handle, player.Handle, player.GetBoneIndex(28422), 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true, true, false, true, 0, true);
			}
		}
		else if (handMicState)
		{
			if (!g_DictHandMikeAttachments.ContainsKey(player))
			{
				uint hash = HashHelper.GetHashUnsigned("p_ing_microphonel_01");
				AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
				{
					g_DictHandMikeAttachments.Add(player, new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension));
				});
			}

			if (g_DictHandMikeAttachments.ContainsKey(player))
			{
				RAGE.Game.Entity.AttachEntityToEntity(g_DictHandMikeAttachments[player].Handle, player.Handle, player.GetBoneIndex(60309), 0.055f, 0.05f, 0.0f, 240.0f, 0.0f, 0.0f, true, true, false, true, 0, true);
			}
		}
		else
		{
			if (g_DictBoomMikeAttachments.ContainsKey(player))
			{
				g_DictBoomMikeAttachments[player].Destroy();
				g_DictBoomMikeAttachments.Remove(player);
			}
			else if (g_DictHandCamAttachments.ContainsKey(player))
			{
				g_DictHandCamAttachments[player].Destroy();
				g_DictHandCamAttachments.Remove(player);
			}
			else if (g_DictHandMikeAttachments.ContainsKey(player))
			{
				g_DictHandMikeAttachments[player].Destroy();
				g_DictHandMikeAttachments.Remove(player);
			}
		}
	}

	private void UpdateAttachments(object[] parameters)
	{
		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			UpdateAttachment(player);
		}
	}

	private void OnUpdate()
	{
		UpdateRightClick();

		if (m_bIsOperatingCam && m_CamOperator == RAGE.Elements.Player.LocalPlayer)
		{
			if (RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveLeftOnly))
			{
				m_NewsCamRotVec.Z += 0.5f;
				CameraManager.UpdateCamera(ECameraID.NEWS, m_NewsCamPosVec, m_NewsCamPosVec, m_NewsCamRotVec);
			}

			if (RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveRightOnly))
			{
				m_NewsCamRotVec.Z -= 0.5f;
				CameraManager.UpdateCamera(ECameraID.NEWS, m_NewsCamPosVec, m_NewsCamPosVec, m_NewsCamRotVec);
			}
			// Set the rotation of the person controlling the camera so we can use that as our rotation for the watchers
			RAGE.Elements.Player.LocalPlayer.SetRotation(m_NewsCamRotVec.X, m_NewsCamRotVec.Y, m_NewsCamRotVec.Z, 0, true);

			// In case I ever figure out how to sync X and Y properly :(
			/*
			if (RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveUpOnly))
			{
				m_NewsCamRotVec.X += 0.5f;
				CameraManager.UpdateCamera(ECameraID.NEWS, m_NewsCamPosVec, m_NewsCamPosVec, m_NewsCamRotVec);
			}

			if (RAGE.Game.Pad.IsControlPressed(0, (int)RAGE.Game.Control.MoveDownOnly))
			{
				m_NewsCamRotVec.X -= 0.5f;
				CameraManager.UpdateCamera(ECameraID.NEWS, m_NewsCamPosVec, m_NewsCamPosVec, m_NewsCamRotVec);
			}*/



			// Limit camera movement
			if (m_NewsCamRotVec.Z >= m_MaxRotLeft)
			{
				m_NewsCamRotVec.Z -= 1.0f;
			}

			if (m_NewsCamRotVec.Z <= m_MaxRotRight)
			{
				m_NewsCamRotVec.Z += 1.0f;
			}
			// X and Y rotation limits for when it gets implemented.
			/*
			if (m_NewsCamRotVec.X >= 90.0f)
			{
				m_NewsCamRotVec.X -= 1.0f;
			}

			if (m_NewsCamRotVec.X <= -90.0f)
			{
				m_NewsCamRotVec.X += 1.0f;
			}*/
		}

		if (m_bJoinedTvBroadcast && m_CamOperator != null && m_NewsCamObject != null)
		{
			CameraManager.UpdateCamera(ECameraID.NEWS, m_NewsCamPosVec, m_NewsCamPosVec, m_CamOperator.GetRotation(0));
		}
	}

	private void OnOperateNewsCamera()
	{
		RAGE.Elements.MapObject nearestNewsCamera = GetNearestNewsCamera();
		EDutyType currentDutyType = DataHelper.GetLocalPlayerEntityData<EDutyType>(EDataNames.DUTY);
		if (nearestNewsCamera != null && currentDutyType == EDutyType.News)
		{
			m_bIsCamBeingOperated = true;
			NetworkEventSender.SendNetworkEvent_NewsCameraOperator(nearestNewsCamera);

			m_NewsCamRotVec = nearestNewsCamera.GetRotation(0);
			m_NewsCamRotVec.Z -= 180f;

			m_NewsCamPosVec = MathHelpers.OffsetPosition(nearestNewsCamera.Position, m_NewsCamRotVec.Z, 1.0f);
			m_NewsCamPosVec.Z += 0.6f;

			//Variables for limiting cam movement
			m_MaxRotLeft = m_NewsCamRotVec.Z - 90.0f;
			m_MaxRotRight = m_NewsCamRotVec.Z + 90.0f;

			CameraManager.UpdateCamera(ECameraID.NEWS, m_NewsCamPosVec, m_NewsCamPosVec, m_NewsCamRotVec);
			SetPlayerAsCamOperator();
		}
	}

	private void SetPlayerAsCamOperator()
	{
		HUD.SetVisible(false, false, false);
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		// Refer to the scaleform manager to how this shit works.
		ScaleformManager.RequestScaleform("security_camera", 255, 255, 255, 255, FullScreen: true, isCustom: true, customScaleformFunctionName: "SET_CAM_LOGO");

		CameraManager.ActivateCamera(ECameraID.NEWS);

		m_bIsOperatingCam = true;
		m_CamOperator = RAGE.Elements.Player.LocalPlayer;

		NetworkEventSender.SendNetworkEvent_OnOperateNewsCamera();
		ScaleformManager.PushAndPopScaleforms();
	}

	private void RemovePlayerAsCamOperator()
	{
		if (m_bIsCamBeingOperated && m_CamOperator == RAGE.Elements.Player.LocalPlayer)
		{
			// remove all scaleforms, unfreeze, deactive the camera.
			HUD.SetVisible(true, false, false);
			ScaleformManager.DeactiveAnyScaleform();
			RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
			CameraManager.DeactiveAnyCamera();

			m_CamOperator = null;
			m_bIsOperatingCam = false;
			m_bIsCamBeingOperated = false;
		}
	}

	private void OnJoinTvBroadcast(RAGE.Elements.Player NewsCameraOperator, RAGE.Elements.MapObject NewsCameraObject)
	{
		bool bJoinedTV = DataHelper.GetEntityData<bool>(RAGE.Elements.Player.LocalPlayer, EDataNames.JOINED_TV);
		if (bJoinedTV)
		{
			HUD.SetVisible(false, false, false);
			m_bJoinedTvBroadcast = true;
			ScaleformManager.RequestScaleform("breaking_news", 255, 255, 255, 255, 0.5f, 0.63f, 1.0f, 1.0f);
			m_CamOperator = NewsCameraOperator;
			m_NewsCamObject = NewsCameraObject;

			m_NewsCamRotVec = NewsCameraObject.GetRotation(0);
			m_NewsCamRotVec.Z -= 180f;

			m_NewsCamPosVec = MathHelpers.OffsetPosition(NewsCameraObject.Position, m_NewsCamObject.GetRotation(0).Z, -1.0f);
			m_NewsCamPosVec.Z += 0.6f;

			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

			CameraManager.UpdateCamera(ECameraID.NEWS, m_NewsCamPosVec, m_NewsCamPosVec, m_NewsCamRotVec);
			CameraManager.ActivateCamera(ECameraID.NEWS);
			ScaleformManager.PushAndPopScaleforms();
		}
	}

	private void OnLeaveTvBroadcast()
	{
		bool bJoinedTV = DataHelper.GetEntityData<bool>(RAGE.Elements.Player.LocalPlayer, EDataNames.JOINED_TV);
		if (!bJoinedTV)
		{
			// remove all scaleforms, unfreeze, deactive the camera, enable HUD.
			m_bJoinedTvBroadcast = false;
			ScaleformManager.DeactiveAnyScaleform();

			RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
			CameraManager.DeactiveAnyCamera();
			HUD.SetVisible(true, false, false);
		}
	}

	public void BreakingText(string newsMsg)
	{
		// We limit the string here to 25 characters from the server so it will always look good.
		if (!string.IsNullOrEmpty(newsMsg))
		{
			m_StrNewsMsg = newsMsg.ToUpper();
			TextHelper.Draw2D(newsMsg.ToUpper(), 0.2f, 0.85f, 1.2f, new RGBA(255, 255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true, false);
		}
		else
		{
			// If for some reason the event doesn't pass a string we get our nice sample text.
			string StrMsg = "SAMPLE TEXT";
			m_StrNewsMsg = StrMsg;
			TextHelper.Draw2D(StrMsg, 0.2f, 0.85f, 1.2f, new RGBA(255, 255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true, false);
		}
	}

	private RAGE.Elements.MapObject GetNearestNewsCamera()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.NewsCamera);
		RAGE.Elements.MapObject newsCamera = poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.MapObject>() : null;

		return newsCamera;
	}

	// Took this from the boombox script we should make this into a raycast helper.
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

								// Is it a News Camera?
								bool isNewsCamera = DataHelper.GetEntityData<bool>(objHit, EDataNames.NEWS_CAM);
								if (isNewsCamera)
								{
									// Are they near enough?
									float fDist = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, raycast.EntityHit.Position);

									if (fDist <= 3.0)
									{
										NetworkEventSender.SendNetworkEvent_PickupNewsCamera(objHit);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}