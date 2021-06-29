using System;
using System.Collections.Generic;

class SpeedCameraDefinition
{
	public SpeedCameraDefinition(int a_ID, RAGE.Vector3 a_RootObjectPosition, float a_RootObjectRotation, RAGE.Vector3 a_TriggerRadiusPosition, string a_strName)
	{
		RootObjectPosition = a_RootObjectPosition;
		RootObjectRotation = a_RootObjectRotation;
		TriggerRadiusPosition = a_TriggerRadiusPosition;
		Name = a_strName;
		ID = a_ID;

		uint baseObjectModel = HashHelper.GetHashUnsigned("prop_sub_trans_06b");
		AsyncModelLoader.RequestAsyncLoad(baseObjectModel, (uint modelLoaded) =>
		{
			m_BaseObject = new RAGE.Elements.MapObject(modelLoaded, RootObjectPosition, new RAGE.Vector3(0.0f, 0.0f, RootObjectRotation));
		});

		renderTargetObjectModel = HashHelper.GetHashUnsigned("prop_tv_03_overlay");
		AsyncModelLoader.RequestAsyncLoad(renderTargetObjectModel, (uint modelLoaded) =>
		{
			// calc pos infront of base object
			float fDist = 0.2f;
			float fRot = RootObjectRotation + 90.0f;
			var vecPos = new RAGE.Vector3(RootObjectPosition.X, RootObjectPosition.Y, RootObjectPosition.Z);
			var radians = fRot * (3.14 / 180.0);
			vecPos.X += (float)Math.Cos(radians) * fDist;
			vecPos.Y += (float)Math.Sin(radians) * fDist;
			vecPos.Z += 2.6f;

			m_RenderTarget = new RAGE.Elements.MapObject(modelLoaded, vecPos, new RAGE.Vector3(0.0f, 0.0f, RootObjectRotation + 180.0f));
		});
	}

	public RAGE.Vector3 RootObjectPosition = null;
	public float RootObjectRotation = 0.0f;
	public RAGE.Vector3 TriggerRadiusPosition = null;
	public string Name;
	public int ID = -1;

	private RAGE.Elements.MapObject m_BaseObject = null;
	private RAGE.Elements.MapObject m_RenderTarget = null;

	// RT
	private const string g_RenderTargetName = "tvscreen";
	private int m_RenderTargetID = -1;
	private uint renderTargetObjectModel = 0;

	private void CreateRenderTarget()
	{
		if (!RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetName))
		{
			RAGE.Game.Ui.RegisterNamedRendertarget(g_RenderTargetName, false);
		}

		// Link it to all models
		if (!RAGE.Game.Ui.IsNamedRendertargetLinked(renderTargetObjectModel))
		{
			RAGE.Game.Ui.LinkNamedRendertarget(renderTargetObjectModel);
		}

		// Get the handle
		if (RAGE.Game.Ui.IsNamedRendertargetRegistered(g_RenderTargetName))
		{
			m_RenderTargetID = RAGE.Game.Ui.GetNamedRendertargetRenderId(g_RenderTargetName);
		}
	}


	public void OnRender()
	{
		if (m_BaseObject == null || m_RenderTarget == null || renderTargetObjectModel == 0)
		{
			return;
		}


		float fDistance = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, m_RenderTarget.Position);
		if (fDistance < 1000.0f)
		{
			if (m_RenderTarget.IsOnScreen())
			{
				if (m_RenderTargetID == -1)
				{
					CreateRenderTarget();
				}

				// set render target + rtt layer
				RAGE.Game.Ui.SetTextRenderId(m_RenderTargetID);
				RAGE.Game.Graphics.Set2dLayer(4);

				string strText = "---";

				if (m_fRenderSpeed > 0.0f)
				{
					strText = ((int)m_fRenderSpeed).ToString();
				}

				float xPos = strText.Length >= 3 ? 0.14f : 0.2f;
				TextHelper.Draw2D(strText, xPos, -0.2f, 5.0f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, false, true);

				// Reset render target
				RAGE.Game.Ui.SetTextRenderId(1);
			}
		}
	}

	private float m_fRenderSpeed = 0.0f;
	private WeakReference<ClientTimer> m_ResetRenderSpeedTimer = new WeakReference<ClientTimer>(null);
	public void SetRenderSpeed(float fSpeed)
	{
		m_fRenderSpeed = fSpeed;

		if (m_ResetRenderSpeedTimer.Instance() != null)
		{
			ClientTimerPool.MarkTimerForDeletion(ref m_ResetRenderSpeedTimer);
		}

		m_ResetRenderSpeedTimer = ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			m_fRenderSpeed = 0.0f;
		}, 30000, 1);
	}

	private bool m_bHasCooldown = false;

	public void CheckTrigger()
	{
		if (m_bHasCooldown)
		{
			return;
		}

		const float fTriggerRadius = 10.0f;

		if (RAGE.Elements.Player.LocalPlayer.Vehicle != null)
		{
			float fDistance = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, TriggerRadiusPosition);
			if (fDistance <= fTriggerRadius)
			{
				float fSpeedMps = RAGE.Elements.Player.LocalPlayer.Vehicle.GetSpeed();
				float fSpeedMetersPerHour = (fSpeedMps * 3600.9f);
				float fSpeedMilesPerHour = (float)Math.Ceiling((fSpeedMetersPerHour / 1609.344f) * 1.25f);

				bool bTriggered = fSpeedMilesPerHour > HUD.m_LastSpeedLimitMPH;
				if (bTriggered)
				{
					RAGE.Game.Audio.PlaySoundFrontend(-1, "Camera_Shoot", "Phone_Soundset_Franklin", true);
					ScreenFadeHelper.BeginFade(100, 50,
						null,
						null,
						null,
						null,
						255,
						255,
						255);
				}

				NetworkEventSender.SendNetworkEvent_SpeedCameraTrigger(fSpeedMilesPerHour, HUD.m_LastSpeedLimitMPH, Name, ID);
				m_bHasCooldown = true;
				ClientTimerPool.CreateTimer((object[] parameters) =>
				{
					m_bHasCooldown = false;
				}, 10000, 1);
			}
		}
	}
}

public class PDSpeedCameraSystem
{
	private List<SpeedCameraDefinition> lstCameras = new List<SpeedCameraDefinition>()
	{
		new SpeedCameraDefinition(0, new RAGE.Vector3(315.06235f, 140.65398f, 102.62092f), 75.0f, new RAGE.Vector3(290.36264f, 155.78363f, 103.20769f), "Vinewood Blvd Eastbound"),
		new SpeedCameraDefinition(1, new RAGE.Vector3(140.61044f, 231.87213f, 106.2f), -112.3566f, new RAGE.Vector3(159.76848f, 218.20299f, 105.89375f), "Vinewood Blvd Westbound")
	};

	public PDSpeedCameraSystem()
	{
		RageEvents.RAGE_OnRender += OnRender;

		RageEvents.RAGE_OnTick_LowFrequency += OnCheckTriggers;

		NetworkEvents.SpeedCameraTrigger_Response += OnRemoteSpeedCameraTrigger;
	}

	private void OnRemoteSpeedCameraTrigger(float fSpeed, int cameraID)
	{
		// find camera and update its rendered speed
		foreach (SpeedCameraDefinition camera in lstCameras)
		{
			if (camera.ID == cameraID)
			{
				camera.SetRenderSpeed(fSpeed);
			}
		}
	}

	private void OnRender()
	{
		foreach (SpeedCameraDefinition camera in lstCameras)
		{
			camera.OnRender();
		}
	}

	private void OnCheckTriggers()
	{
		foreach (SpeedCameraDefinition camera in lstCameras)
		{
			camera.CheckTrigger();
		}
	}
}