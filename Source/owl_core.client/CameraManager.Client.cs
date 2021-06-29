using System;
using System.Collections.Generic;

public enum ECameraID
{
	NONE,
	GAME,
	LOGIN_SCREEN,
	CHARACTER_SELECTION_PREVIEW,
	CHARACTER_CREATION_HEAD,
	CHARACTER_CREATION_BODY_NEAR,
	CHARACTER_CREATION_BODY_FAR,
	TUTORIAL,
	VEHICLE_STORE,
	DUTY_SCENE,
	UGANDA,
	CLOTHING_STORE_HEAD,
	CLOTHING_STORE_BODY,
	BARBER_SHOP_HEAD,
	VEHICLE_MOD_SHOP,
	TAGCREATOR,
	FURNITURE_STORE,
	CHARACTER_CUSTOMIZATION_HEAD,
	CHARACTER_CUSTOMIZATION_BODY_NEAR,
	CHARACTER_CUSTOMIZATION_BODY_FAR,
	NEWS,
	BINOCULARS,
	ACTIVITY
}

public static class CameraManager
{
	private static ECameraID m_ActiveCamera = ECameraID.NONE;
	private static int m_CameraPtr = -1;
	private static bool m_bIsSmoothing = false;
	private const float DEFAULT_FOV = 68.0f;

	public static void Init()
	{
		DeactiveAnyCamera();

		RageEvents.RAGE_OnRender += OnRender;

		// disable idle cam
		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			RAGE.Game.Invoker.Invoke(RAGE.Game.Natives._0x9E4CFFF989258472);
			RAGE.Game.Invoker.Invoke(RAGE.Game.Natives._0xF4F2C0D4EE209E20);
		}, 25000);
	}

	public static void RegisterCamera(ECameraID cameraID, RAGE.Vector3 a_Pos, RAGE.Vector3 a_LookAt, RAGE.Vector3 a_Rot, float a_FOV)
	{
		// Does the camera already exist? Just update it instead
		if (m_dictCameras.ContainsKey(cameraID))
		{
			UpdateCamera(cameraID, a_Pos, a_LookAt, a_Rot, false, 1000, a_FOV);
		}
		else
		{
			CameraInstance newCamera = new CameraInstance(a_Pos.CopyVector(), a_LookAt.CopyVector(), a_Rot.CopyVector(), a_FOV);
			m_dictCameras[cameraID] = newCamera;
		}
	}

	public static void RegisterCamera(ECameraID cameraID, RAGE.Vector3 a_Pos, RAGE.Vector3 a_LookAt)
	{
		RegisterCamera(cameraID, a_Pos, a_LookAt, new RAGE.Vector3(0.0f, 0.0f, 0.0f), 60.0f);
	}

	public static RAGE.Vector3 GetCameraPosition(ECameraID cameraID)
	{
		if (m_dictCameras.ContainsKey(cameraID))
		{
			CameraInstance camDetails = m_dictCameras[cameraID];
			return camDetails.Position;
		}
		else if (cameraID == ECameraID.GAME)
		{
			return RAGE.Game.Cam.GetGameplayCamCoords();
		}
		else
		{
			throw new Exception(Helpers.FormatString("Camera {0} does not exist", cameraID.ToString()));
		}
	}

	public static RAGE.Vector3 GetCameraRotation(ECameraID cameraID)
	{
		if (m_dictCameras.ContainsKey(cameraID))
		{
			CameraInstance camDetails = m_dictCameras[cameraID];
			return camDetails.Rotation;
		}
		else if (cameraID == ECameraID.GAME)
		{
			return RAGE.Game.Cam.GetGameplayCamRot(0);
		}
		else
		{
			throw new Exception(Helpers.FormatString("Camera {0} does not exist", cameraID.ToString()));
		}
	}

	public static float GetCameraFOV(ECameraID cameraID)
	{
		if (m_dictCameras.ContainsKey(cameraID))
		{
			CameraInstance camDetails = m_dictCameras[cameraID];
			return camDetails.FOV;
		}
		else if (cameraID == ECameraID.GAME)
		{
			return RAGE.Game.Cam.GetGameplayCamFov();
		}
		else
		{
			throw new Exception(Helpers.FormatString("Camera {0} does not exist", cameraID.ToString()));
		}
	}

	public static void UpdateCamera(ECameraID cameraID, RAGE.Vector3 a_Pos, RAGE.Vector3 a_LookAt, RAGE.Vector3 a_vecRot, bool bSmooth = false, int smoothTime = 1000, float a_FOV = DEFAULT_FOV)
	{
		if (m_dictCameras.ContainsKey(cameraID))
		{
			CameraInstance camDetails = m_dictCameras[cameraID];
			camDetails.Position = a_Pos;
			camDetails.LookAt = a_LookAt;
			camDetails.Rotation = a_vecRot;
			camDetails.FOV = a_FOV;

			if (m_ActiveCamera == cameraID && m_CameraPtr != -1)
			{
				if (a_FOV != DEFAULT_FOV)
				{
					RAGE.Game.Cam.SetCamFov(m_CameraPtr, camDetails.FOV);
				}

				RAGE.Game.Cam.SetCamCoord(m_CameraPtr, camDetails.Position.X, camDetails.Position.Y, camDetails.Position.Z);
				RAGE.Game.Cam.SetCamRot(m_CameraPtr, camDetails.Rotation.X, camDetails.Rotation.Y, camDetails.Rotation.Z, 0);
				RAGE.Game.Cam.PointCamAtCoord(m_CameraPtr, camDetails.LookAt.X, camDetails.LookAt.Y, camDetails.LookAt.Z);
				//RAGE.Game.Cam.SetCamActive(m_CameraPtr, true);
				//RAGE.Game.Cam.RenderScriptCams(true, bSmooth, bSmooth ? smoothTime : 0, true, false, 0);
			}
		}
		else
		{
			throw new Exception(Helpers.FormatString("Camera {0} does not exist", cameraID.ToString()));
		}
	}

	public static void ActivateCamera(ECameraID cameraID)
	{
		// if already active, return
		if (m_ActiveCamera == cameraID)
		{
			return;
		}

		DeactiveAnyCamera();

		if (m_dictCameras.ContainsKey(cameraID))
		{
			if (m_ActiveCamera != cameraID)
			{
				CameraInstance cameraInst = m_dictCameras[cameraID];
				m_CameraPtr = RAGE.Game.Cam.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", cameraInst.Position.X, cameraInst.Position.Y, cameraInst.Position.Z, cameraInst.Rotation.X, cameraInst.Rotation.Y, cameraInst.Rotation.Z, cameraInst.FOV, true, 1);
				RAGE.Game.Cam.PointCamAtCoord(m_CameraPtr, cameraInst.LookAt.X, cameraInst.LookAt.Y, cameraInst.LookAt.Z);
				RAGE.Game.Cam.SetCamActive(m_CameraPtr, true);
				RAGE.Game.Cam.RenderScriptCams(true, false, 0, true, false, 0);
				m_ActiveCamera = cameraID;
			}
		}
		else
		{
			throw new Exception(Helpers.FormatString("Camera {0} does not exist", cameraID.ToString()));
		}
	}

	public static void DeactivateCamera(ECameraID cameraID, bool bSmooth = false, int smoothTime = 3000)
	{
		if (m_ActiveCamera == cameraID)
		{
			RAGE.Game.Cam.SetCamActive(m_CameraPtr, false);
			RAGE.Game.Cam.RenderScriptCams(false, bSmooth, bSmooth ? smoothTime : 0, true, false, 0);
			m_ActiveCamera = ECameraID.NONE;
			m_CameraPtr = -1;

			if (bSmooth)
			{
				m_bIsSmoothing = true;
				ClientTimerPool.CreateTimer((object[] parameters) =>
				{
					CursorManager.SetCursorVisible(false, g_DummyMouseObject);
					m_bIsSmoothing = false;

				}, smoothTime, 1);
			}
		}
	}

	// TODO: Better way of disabling mouse cam control?
	private static object g_DummyMouseObject = new object();
	public static void DeactiveAnyCamera(bool bSmooth = false, int smoothTime = 3000)
	{
		RAGE.Game.Cam.SetCamActive(m_CameraPtr, false);
		RAGE.Game.Cam.RenderScriptCams(false, bSmooth, bSmooth ? smoothTime : 0, true, false, 0);
		RAGE.Game.Cam.DestroyAllCams(false);
		m_ActiveCamera = ECameraID.NONE;
		m_CameraPtr = -1;

		if (bSmooth)
		{
			m_bIsSmoothing = true;
			ClientTimerPool.CreateTimer((object[] parameters) =>
			{
				CursorManager.SetCursorVisible(false, g_DummyMouseObject);
				m_bIsSmoothing = false;

			}, smoothTime, 1);
		}
	}

	public static RAGE.Vector3 GetActiveCameraPosition()
	{
		return GetCameraPosition(GetActiveCamera());
	}

	public static ECameraID GetActiveCamera()
	{
		return (m_ActiveCamera == ECameraID.NONE ? ECameraID.GAME : m_ActiveCamera);
	}


	private static void OnRender()
	{
		if (m_bIsSmoothing)
		{
			CursorManager.SetCursorVisible(true, g_DummyMouseObject);
		}
	}


	public static void ShakeCamera(ECameraID cameraID, string type, float amplitude)
	{
		if (m_ActiveCamera == cameraID)
		{
			RAGE.Game.Cam.ShakeCam(m_CameraPtr, type, amplitude);
		}
	}

	private static Dictionary<ECameraID, CameraInstance> m_dictCameras = new Dictionary<ECameraID, CameraInstance>();
}

public class CameraInstance
{
	public CameraInstance(RAGE.Vector3 a_Pos, RAGE.Vector3 a_LookAt, RAGE.Vector3 a_Rot, float a_FOV)
	{
		Position = a_Pos;
		Rotation = a_Rot;
		LookAt = a_LookAt;
		FOV = a_FOV;
	}

	public RAGE.Vector3 Position { get; set; }
	public RAGE.Vector3 LookAt { get; set; }
	public RAGE.Vector3 Rotation { get; set; }
	public float FOV { get; set; }
}

