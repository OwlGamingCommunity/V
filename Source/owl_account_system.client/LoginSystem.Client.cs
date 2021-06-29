using System;

public static class LoginSystem
{
	static LoginSystem()
	{

	}

	private static RAGE.Vector3 vecCamRootPos = new RAGE.Vector3();
	private static RAGE.Vector3 vecLookAt = new RAGE.Vector3();

	public static void Init()
	{
		// EVENTS
		NetworkEvents.GotoLogin += GotoLogin;
		NetworkEvents.LoginResult += OnLoginResult;
		RageEvents.RAGE_OnRender += OnRender;

		UIEvents.DoLogin += DoLogin;
		UIEvents.GotoRegisterPressed += OnRegisterPressed;

		CameraManager.RegisterCamera(ECameraID.LOGIN_SCREEN, vecCamRootPos, vecLookAt);
		GotoLoginCamera();
		UpdatePlayerToCameraPos();

		RAGE.Chat.Show(false);

		// TODO_CSHARP: Move this to hud
		if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(g_LoadingTextureDictionary))
		{
			RAGE.Game.Graphics.RequestStreamedTextureDict(g_LoadingTextureDictionary, true);
		}
	}

	private static void InitializeCameraData()
	{
		vecCamRootPos = new RAGE.Vector3(-250.84f, 575.03f, 220.43f);
		if (WorldHelper.IsChristmas())
		{
			vecCamRootPos = new RAGE.Vector3(152.2309f, -1005.356f, 49.3525f);
		}
		else if (WorldHelper.IsHalloween())
		{
			vecCamRootPos = new RAGE.Vector3(152.2309f, -1005.356f, 45.3525f);
		}

		vecLookAt = (WorldHelper.IsChristmas() || WorldHelper.IsHalloween()) ? new RAGE.Vector3(158.6392f, -987.6302f, 40.09193f) : new RAGE.Vector3(-76.33f, -823.8f, 362.05f);
		CameraManager.UpdateCamera(ECameraID.LOGIN_SCREEN, vecCamRootPos, vecLookAt, new RAGE.Vector3(0.0f, 0.0f, 0.0f));
	}

	private static void UpdatePlayerToCameraPos()
	{
		RAGE.Vector3 vecPlayerPos = CameraManager.GetCameraPosition(ECameraID.LOGIN_SCREEN).CopyVector();
		vecPlayerPos.Z -= 15.0f;
		RAGE.Elements.Player.LocalPlayer.Position = vecPlayerPos;
	}

	static float fDistOffset = 0.0f;
	const float g_fDistIncrementPerSecond = 20.0f;
	const float g_MinDist = -100.0f;
	const float g_MaxDist = 3000.0f;
	static DateTime g_CameraLastFrameTick = DateTime.Now;
	static bool bFlipCamera = false;

	private static void OnRender()
	{
		if (CameraManager.GetActiveCamera() == ECameraID.LOGIN_SCREEN)
		{
			if (!WorldHelper.IsChristmas() && !WorldHelper.IsHalloween())
			{
				// update camera
				double dMSSinceLastFrame = (DateTime.Now - g_CameraLastFrameTick).TotalMilliseconds;
				double dDistIncrementThisFrame = (dMSSinceLastFrame / 1000.0) * g_fDistIncrementPerSecond;
				g_CameraLastFrameTick = DateTime.Now;
				float fRot = 270.0f;

				RAGE.Vector3 vecCamPos = vecCamRootPos.CopyVector();
				var radians = fRot * (3.14 / 180.0);
				vecCamPos.X += (float)Math.Cos(radians) * fDistOffset;
				vecCamPos.Y += (float)Math.Sin(radians) * fDistOffset;

				if (bFlipCamera)
				{
					fDistOffset -= (float)dDistIncrementThisFrame;
				}
				else
				{
					fDistOffset += (float)dDistIncrementThisFrame;
				}


				if (fDistOffset < g_MinDist || fDistOffset >= g_MaxDist)
				{
					bFlipCamera = !bFlipCamera;
				}

				CameraManager.UpdateCamera(ECameraID.LOGIN_SCREEN, vecCamPos, vecLookAt, new RAGE.Vector3(0.0f, 0.0f, 0.0f), false);
				UpdatePlayerToCameraPos();
			}
		}

		// TODO: Hacky, why do we need to do this on the first gui?
		if (g_LoginScreenUI.IsVisible())
		{
			g_LoginScreenUI.SetInputEnabled(true);
		}
	}

	private static void DoLogin(string strUsername, string strPassword, bool bAutoLogin)
	{
		NetworkEventSender.SendNetworkEvent_LoginPlayer(strUsername, strPassword, bAutoLogin);
	}

	private static void OnLoginResult(bool bSuccessful, int userID, string titleID, string Username, string strErrorMessage)
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "1st_Person_Transition", "PLAYER_SWITCH_CUSTOM_SOUNDSET", true);

		if (!bSuccessful)
		{
			g_LoginScreenUI.ShowLoginBox(true, true, strErrorMessage);
		}
		else
		{
			g_LoginScreenUI.DoBackendLogin(userID, titleID, Username);
			g_LoginScreenUI.GotoAccessingAccount();
		}
	}

	public static void HideLogin()
	{
		g_LoginScreenUI.SetVisible(false, false, false);
	}

	public static void GotoLoginCamera()
	{
		InitializeCameraData();
		UpdatePlayerToCameraPos();
		CameraManager.ActivateCamera(ECameraID.LOGIN_SCREEN);
		g_CameraLastFrameTick = DateTime.Now;

		RAGE.Game.Ui.DisplayRadar(false);
	}

	public static void GotoLogin(bool bShowUI)
	{
		GotoLoginCamera();

		CharacterSelection.HideCharacterUI();

		// hide hud
		RAGE.Game.Ui.DisplayHud(false);

		if (bShowUI)
		{
			g_LoginScreenUI.SetVisible(true, true, true);
			g_LoginScreenUI.ShowLoginBox(false, true, "");
			g_LoginScreenUI.Reset();
			// TODO: Ref count this now that we can share things
			g_LoginScreenUI.SetCursorAndInputEnabled(true);
		}
		else
		{
			g_LoginScreenUI.SetVisible(false, false, false);
			g_LoginScreenUI.SetCursorAndInputEnabled(false);
		}

		RAGE.Chat.Activate(false);

		DiscordManager.SetDiscordStatus("Logging In");
	}

	private static void OnRegisterPressed()
	{
		HideLogin();
		RegisterSystem.ShowRegister();
	}

	private static void OnUILoaded()
	{

	}

	private static CGUILoginScreen g_LoginScreenUI = new CGUILoginScreen(OnUILoaded);

	// TODO_CSHARP: Move this to hud
	private static string g_LoadingTextureDictionary = "shared";
}