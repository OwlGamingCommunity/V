using System;
using System.Collections.Generic;
using System.Linq;

public delegate void VoidDelegate();
public delegate bool BoolDelegate();

public class GenericCharacterCustomization
{
	public CGUIGenericCharacterCustomization m_UI = null;
	private bool m_bInit = false;

	public string m_strStoreName = String.Empty;
	private BoolDelegate m_OnShowCallback = null;
	private BoolDelegate m_OnPreExitCallback = null;
	private VoidDelegate m_OnExitCallback = null;
	private BoolDelegate m_OnPreFinishCallback = null;
	private VoidDelegate m_OnFinishCallback = null;
	private VoidDelegate m_OnRenderCallback = null;

	private bool m_bClothesVisible = false;
	private RAGE.Vector3 m_vecRestorePos = null;

	private bool m_IsRotating = false;
	private int m_RotationDirection = 0;
	private float m_fCharacterRot = 0.0f;
	const float g_fDefaultRot = 0.0f;

	private RAGE.Vector3 g_vecPlayerPosition = new RAGE.Vector3(402.8675f, -996.4f, -99.00024f);

	public GenericCharacterCustomization(EGUIID guiID)
	{
		m_UI = new CGUIGenericCharacterCustomization(() => { }, guiID);

		NetworkEvents.ChangeCharacterApproved += Hide;

		RageEvents.RAGE_OnRender += OnRender;

		UIEvents.GenericCharacterCustomization_DismissError += OnDismissError;

		// Cameras
		RAGE.Vector3 vecCamPosHead = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y - 0.45f, g_vecPlayerPosition.Z + 0.725f);
		RAGE.Vector3 vecCamLookAtHead = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y, g_vecPlayerPosition.Z + 0.725f);
		CameraManager.RegisterCamera(ECameraID.CHARACTER_CUSTOMIZATION_HEAD, vecCamPosHead, vecCamLookAtHead);

		RAGE.Vector3 vecCamPosBody_Near = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y - 1.9f, g_vecPlayerPosition.Z + 0.3f);
		RAGE.Vector3 vecCamLookAtBody_Near = g_vecPlayerPosition;
		CameraManager.RegisterCamera(ECameraID.CHARACTER_CUSTOMIZATION_BODY_NEAR, vecCamPosBody_Near, vecCamLookAtBody_Near);

		RAGE.Vector3 vecCamPosBody_Far = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y - 2.9f, g_vecPlayerPosition.Z + 0.3f);
		RAGE.Vector3 vecCamLookAtBody_Far = g_vecPlayerPosition;
		CameraManager.RegisterCamera(ECameraID.CHARACTER_CUSTOMIZATION_BODY_FAR, vecCamPosBody_Far, vecCamLookAtBody_Far);

		UIEvents.GenericCharacterCustomization_Finish += OnFinish;
		UIEvents.GenericCharacterCustomization_Exit += OnExit;
		UIEvents.GenericCharacterCustomization_ToggleClothes += OnToggleClothes;

		UIEvents.GenericCharacterCustomization_GotoNearCamEvent += OnGotoNearCam;
		UIEvents.GenericCharacterCustomization_GotoFarCamEvent += OnGotoFarCam;
		UIEvents.GenericCharacterCustomization_GotoHeadCamEvent += OnGotoHeadCam;
		UIEvents.GenericCharacterCustomization_StartRotationEvent += OnStartRotation;
		UIEvents.GenericCharacterCustomization_StopRotationEvent += OnStopRotation;
		UIEvents.GenericCharacterCustomization_ResetRotationEvent += OnResetRotation;
	}

	private void CleanupCameras()
	{
		CameraManager.DeactivateCamera(ECameraID.CHARACTER_CUSTOMIZATION_HEAD);
		CameraManager.DeactivateCamera(ECameraID.CHARACTER_CUSTOMIZATION_BODY_NEAR);
		CameraManager.DeactivateCamera(ECameraID.CHARACTER_CUSTOMIZATION_BODY_FAR);
	}

	public bool IsClothingVisible()
	{
		return m_bClothesVisible;
	}

	private void OnRender()
	{
		// TODO: Better way of tracking this
		if (m_UI.IsVisible())
		{
			if (m_OnRenderCallback != null)
			{
				m_OnRenderCallback();
			}

			if (m_IsRotating)
			{
				const float fDeltaRot = 4.0f;
				m_fCharacterRot += (m_RotationDirection == 1) ? -fDeltaRot : fDeltaRot;
				RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, m_fCharacterRot, 0, true);
			}

			RAGE.Game.Ui.DisplayRadar(false);

			string strDict = "mp_sleep";
			string strAnim = "bind_pose_180";
			if (!RAGE.Game.Streaming.HasAnimDictLoaded(strDict))
			{
				RAGE.Game.Streaming.RequestAnimDict(strDict);
			}
			else
			{
				if (!RAGE.Elements.Player.LocalPlayer.IsPlayingAnim(strDict, strAnim, 3))
				{
					RAGE.Game.Ai.TaskPlayAnim(RAGE.Elements.Player.LocalPlayer.Handle, strDict, strAnim, 8.0f, 1.0f, 10000, 1, 1.0f, false, false, false);
				}
			}
		}
	}

	public void SetNameAndCallbacks(string strStoreName, BoolDelegate OnPreFinishCallback, VoidDelegate OnFinishCallback, BoolDelegate OnShowCallback, BoolDelegate OnPreExitCallback, VoidDelegate OnExitCallback, VoidDelegate OnRenderCallback)
	{
		m_strStoreName = strStoreName;
		m_OnPreFinishCallback = OnPreFinishCallback;
		m_OnFinishCallback = OnFinishCallback;
		m_OnPreExitCallback = OnPreExitCallback;
		m_OnExitCallback = OnExitCallback;
		m_OnShowCallback = OnShowCallback;
		m_OnRenderCallback = OnRenderCallback;
	}

	private void InternalInitialize()
	{
		m_UI.Initialize(m_strStoreName, UIEventID.GenericCharacterCustomization_Finish, UIEventID.GenericCharacterCustomization_Exit, UIEventID.GenericCharacterCustomization_ToggleClothes,
			UIEventID.GenericCharacterCustomization_GotoNearCamEvent, UIEventID.GenericCharacterCustomization_GotoFarCamEvent, UIEventID.GenericCharacterCustomization_GotoHeadCamEvent,
			UIEventID.GenericCharacterCustomization_StartRotationEvent, UIEventID.GenericCharacterCustomization_StopRotationEvent, UIEventID.GenericCharacterCustomization_ResetRotationEvent);

		m_bInit = true;
	}

	public void Show()
	{
		// only show if callback null or callback returns true (show)
		if (m_OnShowCallback == null || m_OnShowCallback())
		{
			ForceShow();
		}
	}

	public void ForceShow()
	{
		if (!m_UI.IsVisible())
		{
			// teleport player
			m_vecRestorePos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

			RAGE.Elements.Player.LocalPlayer.Position = g_vecPlayerPosition;
			m_fCharacterRot = RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z;

			RAGE.Elements.Player.LocalPlayer.SetRotation(-180.000000f, -180.000000f, g_fDefaultRot, 0, false);

			HUD.SetVisible(false, false, false);
			m_UI.SetVisible(true, true, true);

			if (!m_bInit)
			{
				InternalInitialize();
			}
			else
			{
				m_UI.Reset();
			}

			OnGotoNearCam();

			CacheClothing();
			ForceClothesState(false);
		}
	}

	public void TemporaryHide()
	{
		m_UI.SetVisible(false, false, false);
	}

	private void RestoreTemporaryHide()
	{
		m_UI.SetVisible(true, true, false);
	}

	public void SoftHide()
	{
		m_UI.SetVisible(false, false, true);
		CleanupCameras();
	}

	public void Hide()
	{
		if (m_UI.IsVisible())
		{
			SoftHide();

			// non soft things
			HUD.SetVisible(true, false, false);
			RAGE.Elements.Player.LocalPlayer.FreezePosition(false);

			if (m_vecRestorePos != null)
			{
				RAGE.Elements.Player.LocalPlayer.Position = m_vecRestorePos;
			}

			RAGE.Game.Ui.DisplayRadar(true);

			// Got to trigger a server event to reset skin and dimension
			NetworkEventSender.SendNetworkEvent_ExitGenericCharacterCustomization();
		}
	}


	public CGUIGenericCharacterCustomization GetBaseGUI()
	{
		return m_UI;
	}

	private void OnExit()
	{
		if (m_UI.IsVisible())
		{
			if (m_OnExitCallback != null)
			{
				if (m_OnPreExitCallback != null)
				{
					if (m_OnPreExitCallback())
					{
						m_OnExitCallback();
						Hide();
					}
				}
				else
				{
					m_OnExitCallback();
					Hide();
				}
			}
			else
			{
				Hide();
			}
		}
	}

	private void OnFinish()
	{
		if (m_UI.IsVisible())
		{
			if (m_OnFinishCallback != null)
			{
				if (m_OnPreFinishCallback != null)
				{
					if (m_OnPreFinishCallback())
					{
						m_OnFinishCallback();
						Hide();
					}
				}
				else
				{
					m_OnFinishCallback();
					Hide();
				}
			}
			else
			{
				Hide();
			}
		}
	}

	private void OnToggleClothes()
	{
		if (m_UI.IsVisible())
		{
			ToggleClothes();
		}
	}

	private void OnGotoNearCam()
	{
		if (m_UI.IsVisible())
		{
			CameraManager.ActivateCamera(ECameraID.CHARACTER_CUSTOMIZATION_BODY_NEAR);
		}
	}

	private void OnGotoFarCam()
	{
		if (m_UI.IsVisible())
		{
			CameraManager.ActivateCamera(ECameraID.CHARACTER_CUSTOMIZATION_BODY_FAR);
		}
	}

	private void OnGotoHeadCam()
	{
		if (m_UI.IsVisible())
		{
			CameraManager.ActivateCamera(ECameraID.CHARACTER_CUSTOMIZATION_HEAD);
		}
	}

	private void OnStartRotation(int direction)
	{
		if (m_UI.IsVisible())
		{
			m_IsRotating = true;
			m_RotationDirection = direction;
		}
	}

	private void OnStopRotation(int direction)
	{
		if (m_UI.IsVisible())
		{
			m_IsRotating = false;
			m_RotationDirection = 0;
		}
	}

	private void OnResetRotation()
	{
		if (m_UI.IsVisible())
		{
			RAGE.Elements.Player.LocalPlayer.SetRotation(-180.000000f, -180.000000f, g_fDefaultRot, 0, false);
			m_IsRotating = false;
			m_RotationDirection = 0;
		}
	}

	public void ShowErrorMessage(string strMessage)
	{
		TemporaryHide();
		GenericMessageBoxHelper.ShowMessageBox(m_strStoreName, strMessage, "OK", UIEventID.GenericCharacterCustomization_DismissError.ToString());
	}

	public void ClearErrorMessages()
	{
		RestoreTemporaryHide();
		GenericMessageBoxHelper.CloseAnyMessageBox();
	}


	private void OnDismissError()
	{
		RestoreTemporaryHide();
	}

	private void CacheClothing()
	{
		for (int i = 0; i < SkinConstants.NumModels; ++i)
		{
			CurrentDrawables[i] = RAGE.Elements.Player.LocalPlayer.GetDrawableVariation(i);
			CurrentTextures[i] = RAGE.Elements.Player.LocalPlayer.GetTextureVariation(i);
		}

		foreach (ECustomPropSlot slot in Enum.GetValues(typeof(ECustomPropSlot)))
		{
			CurrentPropDrawables[slot] = RAGE.Elements.Player.LocalPlayer.GetPropIndex((int)slot);
			CurrentPropTextures[slot] = RAGE.Elements.Player.LocalPlayer.GetPropTextureIndex((int)slot);
		}

		// cache beard if we have one so we can show it when they set masks to none
		if (MaskHelpers.MasksFunctioningAsBeards.Contains(CurrentDrawables[(int)ECustomClothingComponent.Masks]))
		{
			m_FullBeardDrawable = CurrentDrawables[(int)ECustomClothingComponent.Masks];
			m_FullBeardTexture = CurrentTextures[(int)ECustomClothingComponent.Masks];
		}
	}


	public int[] CurrentDrawables { get; } = new int[SkinConstants.NumModels];
	public int[] CurrentTextures { get; } = new int[SkinConstants.NumModels];

	private int m_FullBeardDrawable = -1;
	private int m_FullBeardTexture = -1;

	// This must be a dictionary, because the enum values are spread out
	public Dictionary<ECustomPropSlot, int> CurrentPropDrawables { get; } = new Dictionary<ECustomPropSlot, int>()
	{
		{ ECustomPropSlot.Hats, 0 },
		{ ECustomPropSlot.Glasses, 0 },
		{ ECustomPropSlot.Ears, 0 },
		{ ECustomPropSlot.Watches, 0 },
		{ ECustomPropSlot.Bracelets, 0 }
	};

	public Dictionary<ECustomPropSlot, int> CurrentPropTextures { get; } = new Dictionary<ECustomPropSlot, int>()
	{
		{ ECustomPropSlot.Hats, 0 },
		{ ECustomPropSlot.Glasses, 0 },
		{ ECustomPropSlot.Ears, 0 },
		{ ECustomPropSlot.Watches, 0 },
		{ ECustomPropSlot.Bracelets, 0 }
	};

	public void ApplyComponent(ECustomClothingComponent component)
	{
		// handle beards
		if (component == ECustomClothingComponent.Masks)
		{
			if (CurrentDrawables[(int)component] == 0)
			{
				// reapply beard?
				if (m_FullBeardDrawable != -1 && m_FullBeardTexture != -1)
				{
					RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.Masks, m_FullBeardDrawable, m_FullBeardTexture, 0);
					return;
				}
			}
		}

		RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)component, CurrentDrawables[(int)component], CurrentTextures[(int)component], 0);

		// re-apply hair
		SkinHelpers.ApplyTattoosAndHairTattoosForPlayer(RAGE.Elements.Player.LocalPlayer);
		int HAIRSTYLE = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_HAIRSTYLE);
		int HAIRCOLOR = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_HAIRCOLOR);
		int HAIRCOLORHIGHLIGHTS = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_HAIRCOLORHIGHLIGHTS);

		RAGE.Elements.Player.LocalPlayer.SetComponentVariation(2, HAIRSTYLE, 0, 0);
		RAGE.Elements.Player.LocalPlayer.SetHairColor(HAIRCOLOR, HAIRCOLORHIGHLIGHTS);
	}

	public void ApplyProp(ECustomPropSlot slot)
	{
		if (CurrentPropDrawables[slot] <= 0)
		{
			RAGE.Elements.Player.LocalPlayer.ClearProp((int)slot);
		}
		else
		{
			RAGE.Elements.Player.LocalPlayer.SetPropIndex((int)slot, CurrentPropDrawables[slot], CurrentPropTextures[slot], true);
		}
	}

	public void ClearComponent(ECustomClothingComponent component)
	{
		RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)component, 0, 0, 0);
	}

	public void ClearProp(ECustomPropSlot slot)
	{
		RAGE.Elements.Player.LocalPlayer.ClearProp((int)slot);
	}

	public void ApplyClothesToggleState()
	{
		if (m_bClothesVisible)
		{
			foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
			{
				ApplyComponent(component);
			}

			foreach (ECustomPropSlot slot in Enum.GetValues(typeof(ECustomPropSlot)))
			{
				ApplyProp(slot);
			}
		}
		else
		{
			foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
			{
				if (component != ECustomClothingComponent.HairStyles)
				{
					// Don't remove the mask if its actually a beard
					if (component == ECustomClothingComponent.Masks && MaskHelpers.MasksFunctioningAsBeards.Contains(CurrentDrawables[(int)ECustomClothingComponent.Masks]))
					{
						ApplyComponent(component);
					}
					else
					{
						ClearComponent(component);
					}
				}
			}

			foreach (ECustomPropSlot slot in Enum.GetValues(typeof(ECustomPropSlot)))
			{
				ClearProp(slot);
			}

			EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
			if (gender == EGender.Male)
			{
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, 15, 0, 0); // shirtless
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(4, 21, 0, 0); // boxers
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(11, 15, 0, 0); // no tshirt
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, 15, 0, 0); // no undershirt
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, 34, 0, 0); // no shoes
			}
			else
			{
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(3, 15, 0, 0); // topless
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(4, 56, 0, 0); // underwear
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(11, 15, 0, 0); // bra
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(8, 15, 0, 0); // no undershirt
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation(6, 35, 0, 0); // no shoes
			}
		}
	}

	public void ToggleClothes()
	{
		m_bClothesVisible = !m_bClothesVisible;
		ApplyClothesToggleState();
	}

	public void ForceClothesState(bool bVisible)
	{
		m_bClothesVisible = bVisible;
		ApplyClothesToggleState();
	}
}