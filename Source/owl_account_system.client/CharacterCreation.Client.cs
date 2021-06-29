using System;
using System.Collections.Generic;
using System.Linq;

public static class CharacterCreation
{
	private static CGUICharacterCreation g_CharacterCreationUI = new CGUICharacterCreation(OnUILoaded);
	private static bool m_bPendingCharCreateResponse = false;
	private static bool m_IsRotating = false;
	private static int m_RotationDirection = 0;
	private static float m_fCharacterRot = 0.0f;
	const float g_fDefaultRot = 0.0f;
	private static RAGE.Vector3 g_vecPlayerPosition = new RAGE.Vector3(402.8675f, -996.4f, -99.00024f);

	// CHAR DATA
	private static CCharacterCreationData m_CharData = new CCharacterCreationData(ECharacterCreationDataType.CharacterCreation);

	public static string g_strDefaultName = "Firstname Lastname";
	private static string g_strName = g_strDefaultName;

	static CharacterCreation()
	{

	}

	public static void Init()
	{
		RageEvents.RAGE_OnRender += OnRender;
		UIEvents.ExitCharCreate += OnExitCharCreate;
		UIEvents.CharCreate_DismissError += OnDismissError;
		NetworkEvents.CreateCharacterResponse += OnCreateCharacterResponse;

		UIEvents.CharCreate_OnChangeTab_Custom += OnChangeTab_Custom;
		UIEvents.CharCreate_OnChangeTab_Premade += OnChangeTab_Premade;

		RAGE.Vector3 vecCamPosHead = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y - 0.45f, g_vecPlayerPosition.Z + 0.725f);
		RAGE.Vector3 vecCamLookAtHead = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y, g_vecPlayerPosition.Z + 0.725f);
		CameraManager.RegisterCamera(ECameraID.CHARACTER_CREATION_HEAD, vecCamPosHead, vecCamLookAtHead);

		RAGE.Vector3 vecCamPosBody_Near = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y - 1.9f, g_vecPlayerPosition.Z + 0.3f);
		RAGE.Vector3 vecCamLookAtBody_Near = g_vecPlayerPosition;
		CameraManager.RegisterCamera(ECameraID.CHARACTER_CREATION_BODY_NEAR, vecCamPosBody_Near, vecCamLookAtBody_Near);

		RAGE.Vector3 vecCamPosBody_Far = new RAGE.Vector3(g_vecPlayerPosition.X, g_vecPlayerPosition.Y - 2.9f, g_vecPlayerPosition.Z + 0.3f);
		RAGE.Vector3 vecCamLookAtBody_Far = g_vecPlayerPosition;
		CameraManager.RegisterCamera(ECameraID.CHARACTER_CREATION_BODY_FAR, vecCamPosBody_Far, vecCamLookAtBody_Far);
	}

	public static void SetCharacterName(string strName)
	{
		g_strName = strName;
	}

	private static void OnUILoaded()
	{

	}

	public static bool IsInCharacterCreation()
	{
		return g_CharacterCreationUI.IsVisible();
	}

	private static void OnChangeTab_Custom(uint TabID)
	{
		if (TabID == TabID_Custom_Appearance)
		{
			m_CharData.ForceClothesState(false);
		}
		else if (TabID == TabID_Custom_HairAndMakeup)
		{
			m_CharData.ForceClothesState(false);
		}
		else if (TabID == TabID_Custom_Tattoos)
		{
			m_CharData.ForceClothesState(false);
		}
		else if (TabID == TabID_Custom_Clothing)
		{
			m_CharData.ForceClothesState(true);
		}
		else if (TabID == TabID_Custom_Info)
		{
			m_CharData.ForceClothesState(true);
		}
	}

	private static void OnChangeTab_Premade(uint TabID)
	{
		if (TabID == TabID_Premade_Appearance)
		{
			m_CharData.ForceClothesState(true);
		}
		else if (TabID == TabID_Premade_Info)
		{
			m_CharData.ForceClothesState(true);
		}
	}

	private static void OnRender()
	{
		if (m_IsRotating)
		{
			const float fDeltaRot = 4.0f;
			var vecRot = RAGE.Elements.Player.LocalPlayer.GetRotation(0);

			m_fCharacterRot += (m_RotationDirection == 1) ? -fDeltaRot : fDeltaRot;
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, m_fCharacterRot, 0, true);
		}

		// TODO: Better way of tracking this
		if (g_CharacterCreationUI.IsVisible())
		{
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

	public static void StartRotation(int direction)
	{
		m_IsRotating = true;
		m_RotationDirection = direction;
	}

	public static void StopRotation(int direction)
	{
		m_IsRotating = false;
		m_RotationDirection = 0;
	}

	public static void ResetRotation()
	{
		RAGE.Elements.Player.LocalPlayer.SetRotation(-180.000000f, -180.000000f, g_fDefaultRot, 0, false);
		m_IsRotating = false;
		m_RotationDirection = 0;
	}

	// UI TABS
	const uint TabID_Custom_Appearance = 0;
	const uint TabID_Custom_HairAndMakeup = 1;
	const uint TabID_Custom_Tattoos = 2;
	const uint TabID_Custom_Clothing = 3;
	const uint TabID_Custom_Info = 4;

	const uint TabID_Premade_Appearance = 0;
	const uint TabID_Premade_Info = 1;

	private static void SetupDefaultTabs()
	{
		g_CharacterCreationUI.Reset();

		// CUSTOM CHAR
		if (m_CharData.CharacterType == ECharacterType.Custom)
		{
			g_CharacterCreationUI.AddTab("Body", "fa-user-circle", UIEventID.CharCreate_OnChangeTab_Custom);
			g_CharacterCreationUI.AddTab("Visuals", "fa-cut", UIEventID.CharCreate_OnChangeTab_Custom);
			g_CharacterCreationUI.AddTab("Tattoos", "fa-palette", UIEventID.CharCreate_OnChangeTab_Custom);
			g_CharacterCreationUI.AddTab("Clothing", "fa-tshirt", UIEventID.CharCreate_OnChangeTab_Custom);
			g_CharacterCreationUI.AddTab("Info", "fa-info-circle", UIEventID.CharCreate_OnChangeTab_Custom);

			Dictionary<EOverlayTypes, uint> maxHeadOverlays = new Dictionary<EOverlayTypes, uint>();
			foreach (EOverlayTypes overlayType in Enum.GetValues(typeof(EOverlayTypes)))
			{
				uint num = (uint)RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetNumHeadOverlayValues, (int)overlayType) + 1;
				maxHeadOverlays[overlayType] = num; // + 1 above is because 0 is actually -1 (none)
			}

			//////////////////////////////////
			// APPEARANCE TAB
			//////////////////////////////////
			// heritage selectors
			// register heritages
			foreach (int value in CharacterConstants.g_CustomFaces_Male)
			{
				g_CharacterCreationUI.AddHeritage(EGender.Male, value);
			}

			foreach (int value in CharacterConstants.g_CustomFaces_Female)
			{
				g_CharacterCreationUI.AddHeritage(EGender.Female, value);
			}

			//////////////////////////////////////////////////
			// BODY APPEARANCE TAB
			//////////////////////////////////////////////////
			g_CharacterCreationUI.AddTabContent_HeritageSelector(TabID_Custom_Appearance, "Parent 1", 0, EGender.Male, "What did your 1st parent look like?", UIEventID.CharCreate_SetFaceShape);
			g_CharacterCreationUI.AddTabContent_HeritageSelector(TabID_Custom_Appearance, "Parent 2", 1, EGender.Female, "What did your 2nd parent look like?", UIEventID.CharCreate_SetFaceShape);

			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Face Shape %", "Do you look more like your 1st or 2nd parent? (Shape)", "Parent 1", "Parent 2", 0, 100, (uint)CCharacterCreationData.GetSkinPercentageForGender(), UIEventID.CharCreate_SetSkinPercentage_Shape);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Skin Color %", "Do you look more like your 1st or 2nd parent? (Color)", "Parent 1", "Parent 2", 0, 100, (uint)CCharacterCreationData.GetSkinPercentageForGender(), UIEventID.CharCreate_SetSkinPercentage_Color);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Eyebrow Depth", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetEyebrowDepth);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Eyebrow Height", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetEyebrowHeight);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Nose Angle", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNoseAngle);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Nose Width", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNoseSizeHorizontal);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Nose Height", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNoseSizeVertical);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Nose Size (Outwards)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNoseSizeOutwards);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Nose Size (Outwards - Lower)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNoseSizeOutwardsLower);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Nose Size (Outwards - Upper)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNoseSizeOutwardsUpper);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Cheekbone Height", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetCheekboneHeight);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Cheek Width (Upper)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetCheekWidth);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Cheek Width (Lower)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetCheekWidthLower);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Mouth Size (Upper)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetMouthSize);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Mouth Size (Lower)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetMouthSizeLower);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Lip Size", "", "Thinner", "Thicker", 0, 100, 50, UIEventID.CharCreate_SetLipSize);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Chin Effect", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetChinEffect);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Chin Width", "", "Healthier", "Skinnier", 0, 100, 50, UIEventID.CharCreate_SetChinWidth);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Chin Size", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetChinSize);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Chin Size (Underneath)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetChinSizeUnderneath);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Neck Width (Upper)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNeckWidth);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Neck Width (Lower)", "", "Smaller", "Larger", 0, 100, 50, UIEventID.CharCreate_SetNeckWidthLower);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_Appearance, "Sun Damage", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.SunDamage]), 0, (uint)maxHeadOverlays[EOverlayTypes.SunDamage], UIEventID.CharCreate_SetSunDamage);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Sun Damage (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetSunDamageOpacity);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_Appearance, "Skin Ageing Effect", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Ageing]), 0, (uint)maxHeadOverlays[EOverlayTypes.Ageing], UIEventID.CharCreate_SetAgeing);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Skin Ageing Effect (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetAgeingOpacity);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_Appearance, "Complexion", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Complexion]), 0, (uint)maxHeadOverlays[EOverlayTypes.Complexion], UIEventID.CharCreate_SetComplexion);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Complexion (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetComplexionOpacity);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_Appearance, "Blemishes", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Blemishes]), 0, (uint)maxHeadOverlays[EOverlayTypes.Blemishes], UIEventID.CharCreate_SetBlemishes);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Blemishes (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetBlemishesOpacity);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_Appearance, "Moles & Freckles", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.MolesFreckles]), 0, (uint)maxHeadOverlays[EOverlayTypes.MolesFreckles], UIEventID.CharCreate_SetMolesFreckles);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Moles & Freckles (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetMolesFrecklesOpacity);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_Appearance);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_Appearance, "Body Blemishes", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.BodyBlemishes]), 0, (uint)maxHeadOverlays[EOverlayTypes.BodyBlemishes], UIEventID.CharCreate_SetBodyBlemishes);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_Appearance, "Body Blemishes (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetBodyBlemishesOpacity);

			//////////////////////////////////////////////////
			// CUSTOMIZATION TAB (makeup etc)
			//////////////////////////////////////////////////

			// BASE HAIR
			int MaxBaseHairStyles = TattooDefinitions.g_HairTattooDefinitions.Count;
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Base Hair", Helpers.FormatString("{0} to {1}", 0, (uint)MaxBaseHairStyles), 0, (uint)MaxBaseHairStyles, UIEventID.CharCreate_SetBaseHair);

			// HAIR
			uint MaxHairStyles = m_CharData.GetGender() == EGender.Male ? CharacterConstants.MaxHairStyles_Male : CharacterConstants.MaxHairStyles_Female;
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Hair", Helpers.FormatString("{0} to {1}", 0, MaxHairStyles), 0, MaxHairStyles, UIEventID.CharCreate_SetHairStyleDrawable);
			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Hair.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Hair Color", "What is the primary color of your hair?", UIEventID.CharCreate_SetHairColor);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Hair.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Hair Color (Highlights)", "What is the highlight color of your hair?", UIEventID.CharCreate_SetHairColorHighlights);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_HairAndMakeup);

			// CHEST HAIR
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Chest Hair", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.ChestHair]), 0, (uint)maxHeadOverlays[EOverlayTypes.ChestHair], UIEventID.CharCreate_SetChestHair);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_HairAndMakeup, "Chest Hair (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetChestHairOpacity);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_ChestHair.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Chest Hair Color (Main)", "What is the primary color of your chest hair?", UIEventID.CharCreate_SetChestHairColor);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_ChestHair.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Chest Hair Color (Highlights)", "What is the highlight color of your chest hair?", UIEventID.CharCreate_SetChestHairColorHighlights);

			// BLUSH
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Blush", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Blush]), 0, (uint)maxHeadOverlays[EOverlayTypes.Blush], UIEventID.CharCreate_SetBlush);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_HairAndMakeup, "Blush (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetBlushOpacity);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Blush.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Blush Color (Main)", "What is the primary color of your blush?", UIEventID.CharCreate_SetBlushColor);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Blush.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Blush Color (Highlights)", "What is the highlight color of your blush?", UIEventID.CharCreate_SetBlushColorHighlights);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_HairAndMakeup);

			// EYE BROWS
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Eyebrows", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Eyebrows]), 0, (uint)maxHeadOverlays[EOverlayTypes.Eyebrows], UIEventID.CharCreate_SetEyeBrows);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_HairAndMakeup, "Eyebrows (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetEyeBrowsOpacity);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Eyebrows.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Eyebrows Color (Main)", "What is the primary color of your eyebrows?", UIEventID.CharCreate_SetEyeBrowsColor);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Eyebrows.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Eyebrows Color (Highlights)", "What is the highlight color of your eyebrows?", UIEventID.CharCreate_SetEyeBrowsColorHighlights);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_HairAndMakeup);

			// EYE SIZE & COLOR
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_HairAndMakeup, "Eye Size", "", "Thicker", "Thinner", 0, 100, 50, UIEventID.CharCreate_SetEyeSize);

			uint eyeMax = 32;
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Eye Color", Helpers.FormatString("{0} to {1}", 0, eyeMax), 0, eyeMax, UIEventID.CharCreate_SetEyeColor);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_HairAndMakeup);

			// FACIAL HAIR
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Facial Hair", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.FacialHair]), 0, (uint)maxHeadOverlays[EOverlayTypes.FacialHair], UIEventID.CharCreate_SetFacialHair);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_HairAndMakeup, "Facial Hair (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetFacialHairOpacity);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_FacialHair.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Facial Hair Color (Main)", "What is the primary color of your facial hair?", UIEventID.CharCreate_SetFacialHairColor);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_FacialHair.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Facial Hair Color (Highlights)", "What is the highlight color of your facial hair?", UIEventID.CharCreate_SetFacialHairColorHighlights);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_HairAndMakeup);

			// FULL BEARDS
			//int BeardsMax = MaskHelpers.MasksFunctioningAsBeards.Length - 1;
			//int maxTextureForBeards = SkinHelpers.GetTexturesForBeard(0);

			//g_CharacterCreationUI.AddTabContent_ClothingSelector(1, "Full Beards", "Style", 0, BeardsMax, UIEventID.CharCreate_SetComponentDrawable_FullBeards,
			//	"Color", 0, maxTextureForBeards, UIEventID.CharCreate_SetComponentTexture_FullBeards, UIEventID.CharCreate_OnRootChanged_FullBeards);

			// LIPSTICK
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Lipstick", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Lipstick]), 0, (uint)maxHeadOverlays[EOverlayTypes.Lipstick], UIEventID.CharCreate_SetLipstick);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_HairAndMakeup, "Lipstick (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetLipstickOpacity);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Lipstick.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Lipstick Color (Main)", "What is the primary color of your lipstick?", UIEventID.CharCreate_SetLipstickColor);

			foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Lipstick.Select((value, i) => (value, (uint)i)))
			{
				g_CharacterCreationUI.AddPendingColor(index, strHexCode);
			}
			g_CharacterCreationUI.AddTabContent_ColorPicker(1, "Lipstick Color (Highlights)", "What is the highlight color of your lipstick?", UIEventID.CharCreate_SetLipstickColorHighlights);

			g_CharacterCreationUI.AddSeperator(TabID_Custom_HairAndMakeup);

			// MAKEUP
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Makeup", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Makeup]), 0, (uint)maxHeadOverlays[EOverlayTypes.Makeup], UIEventID.CharCreate_SetMakeup);
			g_CharacterCreationUI.AddTabContent_Slider(TabID_Custom_HairAndMakeup, "Makeup (Opacity)", "", "Invisible", "Visible", 0, 100, 100, UIEventID.CharCreate_SetMakeupOpacity);

			uint maxMakeupStyles = 54;
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Makeup Styling (Primary)", Helpers.FormatString("{0} to {1}", 0, maxMakeupStyles), 0, maxMakeupStyles, UIEventID.CharCreate_SetMakeupColor);
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_HairAndMakeup, "Makeup Styling (Secondary)", Helpers.FormatString("{0} to {1}", 0, maxMakeupStyles), 0, maxMakeupStyles, UIEventID.CharCreate_SetMakeupColorHighlights);


			//////////////////////////////////////////////////
			// CLOTHING TAB
			//////////////////////////////////////////////////

			// NOTE: For all props, we add on 1, because 0 is 'disable'
			// NOTE: Because of the above, this means we also set max to 0 initially because they have 'no prop' as default
			// PROP: HATS
			List<int> lstHats = SkinConstants.GetHatsMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Hats", "Drawable", 0, lstHats.Count, UIEventID.CharCreate_SetPropDrawable_Hats, "Texture", 0, 0, UIEventID.CharCreate_SetPropTexture_Hats, UIEventID.CharCreate_OnRootChanged_Hats);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// PROP: GLASSES
			List<int> lstGlasses = SkinConstants.GetGlassesMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Glasses", "Drawable", 0, lstGlasses.Count, UIEventID.CharCreate_SetPropDrawable_Glasses, "Texture", 0, 0, UIEventID.CharCreate_SetPropTexture_Glasses, UIEventID.CharCreate_OnRootChanged_Glasses);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// PROP: EARRINGS
			List<int> lstEarrings = SkinConstants.GetEaringsMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Earrings", "Drawable", 0, lstEarrings.Count, UIEventID.CharCreate_SetPropDrawable_Earrings, "Texture", 0, 0, UIEventID.CharCreate_SetPropTexture_Earrings, UIEventID.CharCreate_OnRootChanged_Earrings);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// TORSO
			List<int> lstTorsos = SkinConstants.GetTorsoMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Torsos", "Drawable", 0, lstTorsos.Count, UIEventID.CharCreate_SetComponentDrawable_Torso,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Torsos, 0) - 1, UIEventID.CharCreate_SetComponentTexture_Torso, UIEventID.CharCreate_OnRootChanged_Torso);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// TOPS
			List<int> lstTops = SkinConstants.GetTopsMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Tops", "Drawable", 0, lstTops.Count, UIEventID.CharCreate_SetComponentDrawable_Tops,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Tops, 0) - 1, UIEventID.CharCreate_SetComponentTexture_Tops, UIEventID.CharCreate_OnRootChanged_Tops);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// UNDERSHIRTS
			List<int> lstUndershirts = SkinConstants.GetUndershirtMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Undershirts", "Drawable", 0, lstUndershirts.Count, UIEventID.CharCreate_SetComponentDrawable_Undershirts,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Undershirts, 0) - 1, UIEventID.CharCreate_SetComponentTexture_Undershirts, UIEventID.CharCreate_OnRootChanged_Undershirts);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// ACCESSORIES
			List<int> lstAccessories = SkinConstants.GetAccessoriesMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Accessories", "Drawable", 0, lstAccessories.Count, UIEventID.CharCreate_SetComponentDrawable_Accessories,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Accessories, 0) - 1, UIEventID.CharCreate_SetComponentTexture_Accessories, UIEventID.CharCreate_OnRootChanged_Accessories);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// NOTE: For all props, we add on 1, because 0 is 'disable'
			// PROP: WATCHES
			List<int> lstWatches = SkinConstants.GetWatchesMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Watches", "Drawable", 0, lstWatches.Count, UIEventID.CharCreate_SetPropDrawable_Watches, "Texture", 0, 0, UIEventID.CharCreate_SetPropTexture_Watches, UIEventID.CharCreate_OnRootChanged_Watches);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// PROP: BRACELETS
			List<int> lstBracelets = SkinConstants.GetBraceletsMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Bracelets", "Drawable", 0, lstBracelets.Count, UIEventID.CharCreate_SetPropDrawable_Bracelets, "Texture", 0, 0, UIEventID.CharCreate_SetPropTexture_Bracelets, UIEventID.CharCreate_OnRootChanged_Bracelets);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// DECALS
			List<int> lstDecals = SkinConstants.GetDecalsMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Decals", "Drawable", 0, lstDecals.Count, UIEventID.CharCreate_SetComponentDrawable_Decals,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Decals, 0) - 1, UIEventID.CharCreate_SetComponentTexture_Decals, UIEventID.CharCreate_OnRootChanged_Decals);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// LEGS
			List<int> lstLegs = SkinConstants.GetLegsMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Legs", "Drawable", 0, lstLegs.Count, UIEventID.CharCreate_SetComponentDrawable_Legs,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Legs, 0) - 1, UIEventID.CharCreate_SetComponentTexture_Legs, UIEventID.CharCreate_OnRootChanged_Legs);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			// SHOES
			List<int> lstShoes = SkinConstants.GetShoesMaxForGender(m_CharData.GetGender());
			g_CharacterCreationUI.AddTabContent_ClothingSelector(TabID_Custom_Clothing, "Shoes", "Drawable", 0, lstShoes.Count, UIEventID.CharCreate_SetComponentDrawable_Shoes,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Shoes, 0) - 1, UIEventID.CharCreate_SetComponentTexture_Shoes, UIEventID.CharCreate_OnRootChanged_Shoes);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Clothing);

			//////////////////////////////////////////////////
			// TATTOO TAB
			//////////////////////////////////////////////////
			g_CharacterCreationUI.AddTabContent_GenericButton(TabID_Custom_Tattoos, "Add New Tattoo", UIEventID.CharCreate_Tattoo_AddNew);

			//////////////////////////////////
			// INFO TAB
			//////////////////////////////////
			g_CharacterCreationUI.AddTabContent_Textbox(TabID_Custom_Info, "Name", "What should we call you?", 5, true, UIEventID.CharCreate_SetCharacterName, g_strName);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Info);

			g_CharacterCreationUI.AddTabContent_TwoRadioOptions(TabID_Custom_Info, "Spawn", "Where would you like to spawn?", "Los Santos", "Paleto Bay", UIEventID.CharCreate_SetSpawn);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Info);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Custom_Info, "Age", "How old are you?", CharacterConstants.MinAge, CharacterConstants.MaxAge, UIEventID.CharCreate_SetAge);
			g_CharacterCreationUI.AddSeperator(TabID_Custom_Info);

			g_CharacterCreationUI.AddLanguageListItems();
			g_CharacterCreationUI.AddTabContent_LanguageDropdown(TabID_Custom_Info, "Native Language", "What is your native language?", UIEventID.CharCreate_SetLanguage, false);
			g_CharacterCreationUI.AddTabContent_LanguageDropdown(TabID_Custom_Info, "Secondary Language", "What is your secondary language? (Optional)", UIEventID.CharCreate_SetSecondLanguage, true);

			UpdateTattooCount();
		}
		else if (m_CharData.CharacterType == ECharacterType.Premade)
		{
			g_CharacterCreationUI.AddTab("Skin", "fa-user-circle", UIEventID.CharCreate_OnChangeTab_Premade);
			g_CharacterCreationUI.AddTab("Info", "fa-info-circle", UIEventID.CharCreate_OnChangeTab_Premade);

			//////////////////////////////////
			// SKIN TAB
			//////////////////////////////////
			int skinIDMax = SkinConstants.GetPremadeSkinsForGender(m_CharData.GetGender()).Length - 1;
			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Premade_Appearance, "Skin", Helpers.FormatString("{0} to {1}", 0, (uint)skinIDMax), 0, (uint)skinIDMax, UIEventID.CharCreate_SetSkinID);

			//////////////////////////////////
			// INFO TAB
			//////////////////////////////////
			g_CharacterCreationUI.AddTabContent_Textbox(TabID_Premade_Info, "Name", "What should we call you?", 5, true, UIEventID.CharCreate_SetCharacterName, "Firstname Lastname");
			g_CharacterCreationUI.AddSeperator(TabID_Premade_Info);

			g_CharacterCreationUI.AddTabContent_TwoRadioOptions(TabID_Premade_Info, "Spawn", "Where would you like to spawn?", "Los Santos", "Paleto Bay", UIEventID.CharCreate_SetSpawn);
			g_CharacterCreationUI.AddSeperator(TabID_Premade_Info);

			g_CharacterCreationUI.AddTabContent_NumberSelector(TabID_Premade_Info, "Age", "How old are you?", CharacterConstants.MinAge, CharacterConstants.MaxAge, UIEventID.CharCreate_SetAge);
			g_CharacterCreationUI.AddSeperator(TabID_Premade_Info);

			g_CharacterCreationUI.AddLanguageListItems();
			g_CharacterCreationUI.AddTabContent_LanguageDropdown(TabID_Premade_Info, "Native Language", "What is your native language?", UIEventID.CharCreate_SetLanguage, false);
			g_CharacterCreationUI.AddTabContent_LanguageDropdown(TabID_Premade_Info, "Secondary Language", "What is your secondary language? (Optional)", UIEventID.CharCreate_SetSecondLanguage, true);
		}
	}

	private static void ResetToDefaultCharacter()
	{
		if (m_CharData.IsCustom())
		{
			m_CharData.SetCustomSkin();
		}
		else
		{
			m_CharData.SetSkinIndex(0);
		}

		m_CharData.Reset();
		UpdateTattooCount();

		ApplyCharCreatePosAndAnim();
	}

	private static void CleanupCameras()
	{
		CameraManager.DeactivateCamera(ECameraID.CHARACTER_CREATION_HEAD);
		CameraManager.DeactivateCamera(ECameraID.CHARACTER_CREATION_BODY_NEAR);
		CameraManager.DeactivateCamera(ECameraID.CHARACTER_CREATION_BODY_FAR);
	}

	private static void OnExitCharCreate()
	{
		ClearErrorMessages();

		CleanupCameras();
		g_CharacterCreationUI.SetVisible(false, false, false);
		CharacterSelection.ShowCharacterUI();

		// TODO_RAGE: Add a reset, instead of reload?
		g_CharacterCreationUI.Reload();
		m_CharData.CharacterType = ECharacterType.Custom;
		CCharacterCreationData.Gender = EGender.Male;
		m_CharData.Reset();
	}

	private static void OnCreateCharacterResponse(ECreateCharacterResponse response)
	{
		m_bPendingCharCreateResponse = false;
		if (response == ECreateCharacterResponse.Success)
		{
			OnExitCharCreate();
		}
		else
		{
			g_CharacterCreationUI.ShowCreationUI();

			if (response == ECreateCharacterResponse.Failed_NameInvalid)
			{
				ShowErrorMessage("Your name is formatted incorrectly (Must be Firstname Lastname)");
			}
			else if (response == ECreateCharacterResponse.Failed_NameTaken)
			{
				ShowErrorMessage("Your name is already taken");
			}
			else if (response == ECreateCharacterResponse.Failed_NoLanguage)
			{
				ShowErrorMessage("You have to select at least one language");
			}
			else if (response == ECreateCharacterResponse.Failed_SameLanguage)
			{
				ShowErrorMessage("You have selected two of the same language, please select one other language or leave blank");
			}
		}
	}

	public static void Show(string strCharacterNamePreset)
	{
		g_strName = strCharacterNamePreset;

		m_bPendingCharCreateResponse = false;

		DiscordManager.SetDiscordStatus("Creating a Character");

		g_CharacterCreationUI.Reset();
		SetupDefaultTabs();
		g_CharacterCreationUI.Show();



		g_CharacterCreationUI.SetMaxPedSkinIndexForThisGender(EGender.Male, CharacterConstants.g_PremadeMaleSkins.Length - 1);
		g_CharacterCreationUI.SetMaxPedSkinIndexForThisGender(EGender.Female, CharacterConstants.g_PremadeFemaleSkins.Length - 1);

		g_CharacterCreationUI.SetMaxCustomSkinFaceIndexForThisGender(EGender.Male, CharacterConstants.g_CustomFaces_Male.Length - 1);
		g_CharacterCreationUI.SetMaxCustomSkinFaceIndexForThisGender(EGender.Female, CharacterConstants.g_CustomFaces_Female.Length - 1);

		RAGE.Game.Ui.DisplayHud(false);
		RAGE.Elements.Player.LocalPlayer.SetRotation(-180.000000f, -180.000000f, g_fDefaultRot, 0, false);
		m_fCharacterRot = RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z;

		GotoBodyCam_Near();

		ResetToDefaultCharacter();
	}

	private static void ApplyCharCreatePosAndAnim()
	{
		// TODO_CSHARP: Helper to manage loaded dicts etc (plus perhaps async?)
		RAGE.Elements.Player.LocalPlayer.Position = g_vecPlayerPosition;
	}

	public static void OfferCreateCharacter()
	{
		GenericPromptHelper.ShowPrompt("Are you sure?", "Once created, you will be able to modify your character (including type) at an in-game currency cost, but will not be able to change your gender.", "Yes, Create it!", "No, Go back!", UIEventID.CharCreate_OfferCreateCharacter_Create, UIEventID.CharCreate_OfferCreateCharacter_Cancel);
		g_CharacterCreationUI.HideCreationUI();
	}

	public static void OfferCreateCharacter_Cancel()
	{
		g_CharacterCreationUI.ShowCreationUI();
	}

	public static void OfferCreateCharacter_Create()
	{
		if (!m_bPendingCharCreateResponse)
		{
			// TODO: Disable button on UI also
			m_bPendingCharCreateResponse = true;
			m_CharData.TransmitCreateEvent(g_strName);
		}
	}

	public static void GotoBodyCam_Near()
	{
		CameraManager.ActivateCamera(ECameraID.CHARACTER_CREATION_BODY_NEAR);
	}

	public static void GotoBodyCam_Far()
	{
		CameraManager.ActivateCamera(ECameraID.CHARACTER_CREATION_BODY_FAR);
	}

	public static void GotoHeadCam()
	{
		CameraManager.ActivateCamera(ECameraID.CHARACTER_CREATION_HEAD);
	}

	public static void ToggleClothes()
	{
		m_CharData.ToggleClothes();
	}

	public static void ChangeGender(EGender a_Gender)
	{
		CCharacterCreationData.Gender = a_Gender;
		ResetToDefaultCharacter();
	}

	public static void ChangeSpawn(EScriptLocation location)
	{
		m_CharData.SetSpawn(location);
	}

	public static void SetAge(int age)
	{
		m_CharData.SetAge(age);
	}

	public static void SetPrimaryLanguage(string language)
	{
		m_CharData.SetPrimaryLanguage((ECharacterLanguage)Enum.Parse(typeof(ECharacterLanguage), language));
	}

	public static void SetSecondaryLanguage(string language)
	{
		m_CharData.SetSecondaryLanguage((ECharacterLanguage)Enum.Parse(typeof(ECharacterLanguage), language));
	}

	public static void ChangeType(ECharacterType a_Type)
	{
		m_CharData.CharacterType = a_Type;
		ResetToDefaultCharacter();
		SetupDefaultTabs();
	}

	public static void SetSkinID(int a_SkinIndex)
	{
		m_CharData.SetSkinIndex(a_SkinIndex);
	}


	public static void UpdateMaxTextureForComponent(ECustomClothingComponent component, string strElementToReset)
	{
		int currentDrawable = m_CharData.CurrentDrawables[(int)component];
		int numTextures = RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)component, currentDrawable) - 1; // -1 because 0 is a texture

		if (component == ECustomClothingComponent.Masks)
		{
			numTextures = SkinHelpers.GetTexturesForBeard(currentDrawable);
		}

		g_CharacterCreationUI.SetMaxForElement(strElementToReset, numTextures);
	}
	public static void UpdateMaxTextureForProp(ECustomPropSlot prop, string strElementToReset)
	{
		int currentDrawable = m_CharData.CurrentPropDrawables[prop];
		int numTextures = RAGE.Elements.Player.LocalPlayer.GetNumberOfPropTextureVariations((int)prop, currentDrawable) - 1; // -1 because 0 is a texture

		g_CharacterCreationUI.SetMaxForElement(strElementToReset, numTextures);
	}

	public static void SetComponentDrawable(ECustomClothingComponent component, int value)
	{
		if (component == ECustomClothingComponent.Masks)
		{
			value = MaskHelpers.MasksFunctioningAsBeards[value];
		}

		m_CharData.SetComponentDrawable(component, value);

		if ((int)component > 2)
		{
			GotoBodyCam_Near();
		}
		else
		{
			GotoHeadCam();
		}
	}

	public static void SetComponentPalette(ECustomClothingComponent component, int value)
	{
		m_CharData.SetComponentPalette(component, value);

		if ((int)component > 2)
		{
			GotoBodyCam_Near();
		}
		else
		{
			GotoHeadCam();
		}
	}

	public static void SetComponentTexture(ECustomClothingComponent component, int value)
	{
		m_CharData.SetComponentTexture(component, value);

		if ((int)component > 2)
		{
			GotoBodyCam_Near();
		}
		else
		{
			GotoHeadCam();
		}
	}

	public static void SetPropDrawable(ECustomPropSlot slot, int value)
	{
		m_CharData.SetPropDrawable(slot, value);

		if (slot == ECustomPropSlot.Ears || slot == ECustomPropSlot.Glasses)
		{
			GotoHeadCam();
		}
		else
		{
			GotoBodyCam_Near();
		}
	}

	public static void SetPropTexture(ECustomPropSlot slot, int value)
	{
		m_CharData.SetPropTexture(slot, value);

		if (slot == ECustomPropSlot.Ears || slot == ECustomPropSlot.Glasses)
		{
			GotoHeadCam();
		}
		else
		{
			GotoBodyCam_Near();
		}
	}

	// Custom char actions
	public static void SetBlemishes(int value) { m_CharData.SetBlemishes(value); }
	public static void SetEyeBrows(int value) { m_CharData.SetEyeBrows(value); }
	public static void SetFacialHair(int value) { m_CharData.SetFacialHair(value); }
	public static void SetAgeing(int value) { m_CharData.SetAgeing(value); }
	public static void SetMakeup(int value) { m_CharData.SetMakeup(value); }
	public static void SetMakeupColor(int value) { m_CharData.SetMakeupColor(value); }
	public static void SetMakeupColorHighlights(int value) { m_CharData.SetMakeupColorHighlights(value); }
	public static void SetBlush(int value) { m_CharData.SetBlush(value); }
	public static void SetComplexion(int value) { m_CharData.SetComplexion(value); }
	public static void SetSunDamage(int value) { m_CharData.SetSunDamage(value); }
	public static void SetLipstick(int value) { m_CharData.SetLipstick(value); }
	public static void SetMolesFreckles(int value) { m_CharData.SetMolesFreckles(value); }
	public static void SetLipstickColor(int value) { m_CharData.SetLipstickColor(value); }
	public static void SetLipstickColorHighlights(int value) { m_CharData.SetLipstickColorHighlights(value); }
	public static void SetBlushColor(int value) { m_CharData.SetBlushColor(value); }
	public static void SetBlushColorHighlights(int value) { m_CharData.SetBlushColorHighlights(value); }
	public static void SetEyeBrowsColor(int value) { m_CharData.SetEyeBrowsColor(value); }
	public static void SetEyeBrowsColorHighlights(int value) { m_CharData.SetEyeBrowsColorHighlights(value); }
	public static void SetFacialHairColor(int value) { m_CharData.SetFacialHairColor(value); }
	public static void SetFacialHairColorHighlights(int value) { m_CharData.SetFacialHairColorHighlights(value); }
	public static void SetHairColor(int value) { m_CharData.SetHairColor(value); }
	public static void SetHairColorHighlights(int value) { m_CharData.SetHairColorHighlights(value); }
	public static void SetBlushOpacity(float value) { m_CharData.SetBlushOpacity(value); }
	public static void SetFacialHairOpacity(float value) { m_CharData.SetFacialHairOpacity(value); }
	public static void SetComplexionOpacity(float value) { m_CharData.SetComplexionOpacity(value); }
	public static void SetSunDamageOpacity(float value) { m_CharData.SetSunDamageOpacity(value); }
	public static void SetLipstickOpacity(float value) { m_CharData.SetLipstickOpacity(value); }
	public static void SetMolesFrecklesOpacity(float value) { m_CharData.SetMolesFrecklesOpacity(value); }
	public static void SetChestHair(int value) { m_CharData.SetChestHair(value); }
	public static void SetChestHairColor(int value) { m_CharData.SetChestHairColor(value); }
	public static void SetChestHairColorHighlights(int value) { m_CharData.SetChestHairColorHighlights(value); }
	public static void SetChestHairOpacity(float value) { m_CharData.SetChestHairOpacity(value); }

	// TATTOOS

	private static void ShowErrorMessage(string strMessage)
	{
		g_CharacterCreationUI.HideCreationUI();
		GenericMessageBoxHelper.ShowMessageBox("Character Creation", strMessage, "OK", UIEventID.CharCreate_DismissError.ToString());
	}

	private static void ClearErrorMessages()
	{
		g_CharacterCreationUI.ShowCreationUI();
		GenericMessageBoxHelper.CloseAnyMessageBox();
	}

	public static void Tattoo_AddNew()
	{
		ClearErrorMessages();

		if (m_CharData.GetNumTattoos() >= CharacterConstants.MaxTattoos)
		{
			ShowErrorMessage("You have reached the maximum number of tattoos.");
		}
		else
		{
			m_CharData.ClearPreviewTattoo();
			g_CharacterCreationUI.ShowTattooCreator();
		}
	}

	public static void Tattoo_Cancel()
	{
		m_CharData.ClearPreviewTattoo();
	}

	private static void OnDismissError()
	{
		g_CharacterCreationUI.ShowCreationUI();
	}

	private static void UpdateTattooCount()
	{
		g_CharacterCreationUI.SetTabName(TabID_Custom_Tattoos, Helpers.FormatString("Tattoos ({0}/{1})", m_CharData.GetNumTattoos(), CharacterConstants.MaxTattoos));
	}

	public static void Tattoo_Create()
	{
		CTattooDefinition previewTattooDef = m_CharData.GetPreviewTattoo();
		if (previewTattooDef != null)
		{
			m_CharData.AddTattoo(previewTattooDef);
			g_CharacterCreationUI.AddTabContent_GenericListItem(TabID_Custom_Tattoos, previewTattooDef.LocalizedName, previewTattooDef.Zone.ToString().Replace("ZONE_", ""), "Click to Remove", UIEventID.CharCreate_Tattoo_RemoveTattoo, previewTattooDef.ID);

			UpdateTattooCount();
		}
	}

	public static void Tattoo_RemoveTattoo(string strElementName, int tattooID)
	{
		m_CharData.RemoveTattoo(tattooID);
		g_CharacterCreationUI.DeleteTabContent_GenericListItem(strElementName);

		UpdateTattooCount();
	}

	public static void Tattoo_ChangeZone(TattooZone zone)
	{
		// reset on change category
		m_CharData.ClearPreviewTattoo();

		foreach (var kvPair in TattooDefinitions.g_TattooDefinitions)
		{
			CTattooDefinition tattoo = kvPair.Value;

			if (tattoo.Zone == zone)
			{
				// only add it if we dont already have the tattoo on our character - tattoos are unique
				if (!m_CharData.HasTattoo(tattoo))
				{
					if (tattoo.SupportsGender(m_CharData.GetGender()))
					{
						g_CharacterCreationUI.AddTattooListItem(tattoo.ID, tattoo.LocalizedName);
					}
				}
			}
		}

		g_CharacterCreationUI.CommitTattooList();
	}

	public static void Tattoo_ChangeTattoo(int tattooID)
	{
		CTattooDefinition tattooDef = null;
		foreach (var kvPair in TattooDefinitions.g_TattooDefinitions)
		{
			CTattooDefinition tattoo = kvPair.Value;

			if (tattoo.ID == tattooID)
			{
				tattooDef = tattoo;
				break;
			}
		}

		m_CharData.SetPreviewTattoo(tattooDef);
	}

	public static void SetBodyBlemishes(int value) { m_CharData.SetBodyBlemishes(value); }
	public static void SetBodyBlemishesOpacity(float value) { m_CharData.SetBodyBlemishesOpacity(value); }
	//public static void SetExtraBodyBlemishes(int value) { m_CharData.SetExtraBodyBlemishes(value); }
	//public static void SetExtraBodyBlemishesOpacity(float value) { m_CharData.SetExtraBodyBlemishesOpacity(value); }
	public static void SetBlemishesOpacity(float value) { m_CharData.SetBlemishesOpacity(value); }
	public static void SetEyeBrowsOpacity(float value) { m_CharData.SetEyeBrowsOpacity(value); }
	public static void SetAgeingOpacity(float value) { m_CharData.SetAgeingOpacity(value); }
	public static void SetMakeupOpacity(float value) { m_CharData.SetMakeupOpacity(value); }
	public static void SetEyeColor(int value) { m_CharData.SetEyeColor(value); }
	public static void SetBaseHair(int value) { m_CharData.SetBaseHair(value); }
	public static void SetHairStyleDrawable(int value) { m_CharData.SetHairStyleDrawable(value); }
	public static void SetNoseSizeHorizontal(float value) { m_CharData.SetNoseSizeHorizontal(value); }
	public static void SetNoseSizeVertical(float value) { m_CharData.SetNoseSizeVertical(value); }
	public static void SetNoseSizeOutwards(float value) { m_CharData.SetNoseSizeOutwards(value); }
	public static void SetNoseSizeOutwardsUpper(float value) { m_CharData.SetNoseSizeOutwardsUpper(value); }
	public static void SetNoseSizeOutwardsLower(float value) { m_CharData.SetNoseSizeOutwardsLower(value); }
	public static void SetNoseAngle(float value) { m_CharData.SetNoseAngle(value); }
	public static void SetEyebrowHeight(float value) { m_CharData.SetEyebrowHeight(value); }
	public static void SetEyebrowDepth(float value) { m_CharData.SetEyebrowDepth(value); }
	public static void SetCheekboneHeight(float value) { m_CharData.SetCheekboneHeight(value); }
	public static void SetCheekWidth(float value) { m_CharData.SetCheekWidth(value); }
	public static void SetCheekWidthLower(float value) { m_CharData.SetCheekWidthLower(value); }
	public static void SetEyeSize(float value) { m_CharData.SetEyeSize(value); }
	public static void SetLipSize(float value) { m_CharData.SetLipSize(100.0f - value); }
	public static void SetMouthSize(float value) { m_CharData.SetMouthSize(value); }
	public static void SetMouthSizeLower(float value) { m_CharData.SetMouthSizeLower(value); }
	public static void SetChinSize(float value) { m_CharData.SetChinSize(value); }
	public static void SetChinSizeUnderneath(float value) { m_CharData.SetChinSizeUnderneath(value); }
	public static void SetChinWidth(float value) { m_CharData.SetChinWidth(value); } // NOTE: Inverted so right is bigger
	public static void SetChinEffect(float value) { m_CharData.SetChinEffect(value); }
	public static void SetNeckWidth(float value) { m_CharData.SetNeckWidth(value); }
	public static void SetNeckWidthLower(float value) { m_CharData.SetNeckWidthLower(value); }
	public static void SetFaceShape(int index, int genderID, int value) { m_CharData.SetFaceShape(index, genderID, value); }
	public static void SetSkinPercentage_Shape(float value) { m_CharData.SetSkinPercentage_Shape(value); }
	public static void SetSkinPercentage_Color(float value) { m_CharData.SetSkinPercentage_Color(value); }
}

// TODO_LAUNCH: Support chest hair, body blemishes?

