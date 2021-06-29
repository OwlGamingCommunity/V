using System;
using System.Collections.Generic;
using System.Linq;

public enum ECharacterCreationDataType
{
	None,
	CharacterCreation,
	PlasticSurgeon
}

// NOTE: If you modify this class, check plastic surgeon AND char creation still work correctly
public class CCharacterCreationData
{
	public int[] CurrentDrawables { get; } = new int[SkinConstants.NumModels];
	public int[] CurrentTextures { get; } = new int[SkinConstants.NumModels];
	public int[] CurrentPalettes { get; } = new int[SkinConstants.NumModels];

	public List<CTattooDefinition> g_CharacterTattoos = new List<CTattooDefinition>();

	private CTattooDefinition g_PreviewTattoo = null;

	private ECharacterCreationDataType g_Type = ECharacterCreationDataType.None;


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

	public CCharacterCreationData(ECharacterCreationDataType type)
	{
		RageEvents.RAGE_OnTick_HighFrequency += OnTick60fps;
		g_Type = type;

		// Must initialize values by char data
		if (type == ECharacterCreationDataType.PlasticSurgeon)
		{
			g_Ageing = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_AGEING);
			g_AgeingOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_AGEINGOPACITY);
			g_Makeup = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MAKEUP);
			g_MakeupOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MAKEUPOPACITY);
			g_MakeupColor = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MAKEUPCOLOR);
			g_MakeupColorHighlights = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MAKEUPCOLORHIGHLIGHT);
			g_Blush = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLUSH);
			g_BlushOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_BLUSHOPACITY);
			g_BlushColor = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLUSHCOLOR);
			g_BlushColorHighlight = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLUSHCOLORHIGHLIGHT);
			g_Complexion = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_COMPLEXION);
			g_ComplexionOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_COMPLEXIONOPACITY);
			g_SunDamage = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_SUNDAMAGE);
			g_SunDamageOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_SUNDAMAGEOPACITY);
			g_Lipstick = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_LIPSTICK);
			g_LipstickOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_LIPSTICKOPACITY);
			g_LipstickColor = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_LIPSTICKCOLOR);
			g_LipstickColorHighlights = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS);
			g_MolesAndFreckles = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MOLESANDFRECKLES);
			g_MolesAndFrecklesOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MOLESANDFRECKLESOPACITY);
			g_NoseAngle = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEHORIZONTAL);
			g_NoseSizeVertical = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEVERTICAL);
			g_NoseSizeOutwards = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEOUTWARDS);
			g_NoseSizeOutwardsUpper = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEOUTWARDSUPPER);
			g_NoseSizeOutwardsLower = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEOUTWARDSLOWER);
			g_NoseAngle = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSEANGLE);
			g_EyebrowHeight = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYEBROWHEIGHT);
			g_EyebrowDepth = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYEBROWDEPTH);
			g_CheekboneHeight = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHEEKBONEHEIGHT);
			g_CheekWidth = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHEEKWIDTH);
			g_CheekWidthLower = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHEEKWIDTHLOWER);
			g_EyeSize = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYESIZE);
			g_LipSize = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_LIPSIZE);
			g_MouthSize = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MOUTHSIZE);
			g_MouthSizeLower = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MOUTHSIZELOWER);
			g_ChinSize = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINSIZE);
			g_ChinSizeLower = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINSIZELOWER);
			g_ChinWidth = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINWIDTH);
			g_ChinEffect = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINEFFECT);
			g_NeckWidth = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NECKWIDTH);
			g_NeckWidthLower = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NECKWIDTHLOWER);

			SetFaceShape((int)EGender.Female, (int)EGender.Female, DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_FACEBLEND1MOTHER));
			SetFaceShape((int)EGender.Male, (int)EGender.Male, DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_FACEBLEND1FATHER));
			SetSkinPercentage_Shape(DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_FACEBLENDFATHERPERCENT) * 100.0f, false);
			SetSkinPercentage_Color(DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_SKINBLENDFATHERPERCENT) * 100.0f, false);

			g_EyeColor = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYECOLOR);
			g_Blemishes = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLEMISHES);
			g_BlemishesOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_BLEMISHESOPACITY);
			g_Eyebrows = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYEBROWS);
			g_EyebrowsOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYEBROWSOPACITY);
			g_EyebrowsColor = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYEBROWSCOLOR);
			g_EyebrowsColorHighlight = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYEBROWSCOLORHIGHLIGHT);
			g_BodyBlemishes = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BODYBLEMISHES);
			g_BodyBlemishesOpacity = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_BODYBLEMISHESOPACITY);

			ApplyCustomCharData();
		}
	}

	public CTattooDefinition GetPreviewTattoo()
	{
		return g_PreviewTattoo;
	}

	public void SetPreviewTattoo(CTattooDefinition previewTattoo)
	{
		g_PreviewTattoo = previewTattoo;
		ApplyCustomCharData();
	}

	public int GetNumTattoos()
	{
		return g_CharacterTattoos.Count;
	}

	public void ClearPreviewTattoo()
	{
		g_PreviewTattoo = null;
		ApplyCustomCharData();
	}

	public bool HasTattoo(CTattooDefinition tattoo)
	{
		return g_CharacterTattoos.Contains(tattoo);
	}

	public void AddTattoo(CTattooDefinition tattooDef)
	{
		ClearPreviewTattoo();
		if (tattooDef != null)
		{
			g_CharacterTattoos.Add(tattooDef);
			ApplyCustomCharData();
		}
	}

	public void RemoveTattoo(int tattooID)
	{
		ClearPreviewTattoo();
		foreach (CTattooDefinition tattooDef in g_CharacterTattoos)
		{
			if (tattooDef.ID == tattooID)
			{
				g_CharacterTattoos.Remove(tattooDef);
				break;
			}
		}

		ApplyCustomCharData();
	}

	public ECharacterType CharacterType { get; set; } = ECharacterType.Custom;
	public static EGender Gender { get; set; } = EGender.Male;

	public ECharacterLanguage PrimaryLanguage { get; set; } = ECharacterLanguage.None;
	public ECharacterLanguage SecondaryLanguage { get; set; } = ECharacterLanguage.None;

	public bool IsCustom() { return CharacterType == ECharacterType.Custom; }

	public uint SkinHash { get; set; } = 0;

	private void ApplyComponent(ECustomClothingComponent component)
	{
		RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)component, CurrentDrawables[(int)component], CurrentTextures[(int)component], CurrentPalettes[(int)component]);
	}

	private void ApplyProp(ECustomPropSlot slot)
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

	// NOTE: Only used for toggle clothes
	private void ClearComponent(ECustomClothingComponent component)
	{
		RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)component, 0, 0, 0);
	}

	private void ClearProp(ECustomPropSlot slot)
	{
		RAGE.Elements.Player.LocalPlayer.ClearProp((int)slot);
	}

	private bool m_bClothesVisible = true;
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

			if (Gender == EGender.Male)
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

	public void SetComponentDrawable(ECustomClothingComponent component, int index)
	{
		if (CharacterType == ECharacterType.Custom)
		{
			int value = -1;
			if (component == ECustomClothingComponent.Masks)
			{
				value = SkinConstants.GetMasks(Gender)[index];
			}
			else if (component == ECustomClothingComponent.HairStyles)
			{
				value = index;
			}
			else if (component == ECustomClothingComponent.Torsos)
			{
				value = SkinConstants.GetTorsoMaxForGender(Gender)[index];
			}
			else if (component == ECustomClothingComponent.Legs)
			{
				value = SkinConstants.GetLegsMaxForGender(Gender)[index];
			}
			else if (component == ECustomClothingComponent.BagsAndParachutes)
			{
				// NOTE: Unsupported at this time
			}
			else if (component == ECustomClothingComponent.Shoes)
			{
				value = SkinConstants.GetShoesMaxForGender(Gender)[index];
			}
			else if (component == ECustomClothingComponent.Accessories)
			{
				value = SkinConstants.GetAccessoriesMaxForGender(Gender)[index];
			}
			else if (component == ECustomClothingComponent.Undershirts)
			{
				value = SkinConstants.GetUndershirtMaxForGender(Gender)[index];
			}
			else if (component == ECustomClothingComponent.BodyArmor)
			{
				// NOTE: Unsupported at this time
			}
			else if (component == ECustomClothingComponent.Decals)
			{
				value = SkinConstants.GetDecalsMaxForGender(Gender)[index];
			}
			else if (component == ECustomClothingComponent.Tops)
			{
				value = SkinConstants.GetTopsMaxForGender(Gender)[index];
			}

			CurrentDrawables[(int)component] = value;

			if (m_bClothesVisible || component == ECustomClothingComponent.HairStyles || component == ECustomClothingComponent.Masks) // 2 is hair styles and masks are beards, we let that one go
			{
				ApplyComponent(component);
			}
		}
	}

	public void SetComponentPalette(ECustomClothingComponent component, int value)
	{
		CurrentPalettes[(int)component] = value;

		if (m_bClothesVisible || component == ECustomClothingComponent.HairStyles || component == ECustomClothingComponent.Masks) // 2 is hair styles and masks are beards, we let that one go
		{
			ApplyComponent(component);
		}
	}

	public void SetComponentTexture(ECustomClothingComponent component, int value)
	{
		CurrentTextures[(int)component] = value;

		if (m_bClothesVisible || component == ECustomClothingComponent.HairStyles || component == ECustomClothingComponent.Masks) // 2 is hair styles and masks are beards, we let that one go
		{
			ApplyComponent(component);
		}
	}

	public void SetPropDrawable(ECustomPropSlot slot, int index)
	{
		int value = -1;
		if (slot == ECustomPropSlot.Hats)
		{
			value = SkinConstants.GetHatsMaxForGender(Gender)[index];
		}
		else if (slot == ECustomPropSlot.Glasses)
		{
			value = SkinConstants.GetGlassesMaxForGender(Gender)[index];
		}
		else if (slot == ECustomPropSlot.Ears)
		{
			value = SkinConstants.GetEaringsMaxForGender(Gender)[index];
		}
		else if (slot == ECustomPropSlot.Watches)
		{
			value = SkinConstants.GetWatchesMaxForGender(Gender)[index];
		}
		else if (slot == ECustomPropSlot.Bracelets)
		{
			value = SkinConstants.GetBraceletsMaxForGender(Gender)[index];
		}

		CurrentPropDrawables[slot] = value;

		if (m_bClothesVisible)
		{
			ApplyProp(slot);
		}
	}

	public void SetPropTexture(ECustomPropSlot slot, int value)
	{
		CurrentPropTextures[slot] = value;

		if (m_bClothesVisible)
		{
			ApplyProp(slot);
		}
	}

	public void SetCustomSkin()
	{
		if (Gender == EGender.Male)
		{
			SkinHash = CharacterConstants.CustomMaleSkin;
		}
		else
		{
			SkinHash = CharacterConstants.CustomFemaleSkin;
		}

		Reset();
	}

	public void SetSkinIndex(int index)
	{
		if (Gender == EGender.Male)
		{
			SkinHash = CharacterConstants.g_PremadeMaleSkins[index];
		}
		else
		{
			SkinHash = CharacterConstants.g_PremadeFemaleSkins[index];
		}

		Reset();
	}

	private void OnTick60fps()
	{
		if (m_bIsFadingCharOut)
		{
			m_CurrentAlpha -= 50;
			RAGE.Elements.Player.LocalPlayer.SetAlpha(m_CurrentAlpha, false);
			// TODO: Generic entity fader?
			if (m_CurrentAlpha <= 0)
			{
				RAGE.Elements.Player.LocalPlayer.SetAlpha(0, false);
				ResetInternal();
				m_bIsFadingCharOut = false;

			}
		}

		if (m_bIsFadingCharIn)
		{
			m_CurrentAlpha += 50;
			RAGE.Elements.Player.LocalPlayer.SetAlpha(m_CurrentAlpha, false);

			if (m_CurrentAlpha >= 255)
			{
				RAGE.Elements.Player.LocalPlayer.SetAlpha(255, false);
				m_bIsFadingCharIn = false;
			}
		}
	}

	private bool m_bIsFadingCharIn = false;
	private bool m_bIsFadingCharOut = false;
	private int m_CurrentAlpha = 255;

	public void Reset()
	{
		if (g_Type != ECharacterCreationDataType.PlasticSurgeon)
		{
			m_bIsFadingCharIn = false;
			m_bIsFadingCharOut = true;
			m_CurrentAlpha = RAGE.Elements.Player.LocalPlayer.GetAlpha();
			ApplyClothesToggleState();
		}
	}

	private void ResetInternal()
	{
		if (g_Type != ECharacterCreationDataType.PlasticSurgeon)
		{
			ResetCustomCharacterData();

			AsyncModelLoader.RequestAsyncLoad(SkinHash, (uint modelLoaded) =>
			{
				RAGE.Elements.Player.LocalPlayer.Model = modelLoaded;
				RAGE.Elements.Player.LocalPlayer.SetDefaultComponentVariation();

				//for (int i = 0; i < CurrentDrawables.Length; i++)
				foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
				{
					if (component == ECustomClothingComponent.HairStyles)
					{
						// TODO_POST_LAUNCH: Much better way of having defaults. Make it use array of defaults by default (no pun intended)
						int defaultVal = 0;
						if (Gender == EGender.Female)
						{
							defaultVal = 10;
						}

						SetComponentDrawable(component, defaultVal);
						SetComponentTexture(component, 0);
						SetComponentPalette(component, 0);
					}
					else
					{
						if (CharacterType == ECharacterType.Premade)
						{
							SetComponentDrawable(component, -1);
							SetComponentTexture(component, 0);
							SetComponentPalette(component, 0);
						}
						else
						{
							SetComponentDrawable(component, 0);
							SetComponentTexture(component, 0);
							SetComponentPalette(component, 0);
						}
					}
				}

				foreach (ECustomPropSlot slot in Enum.GetValues(typeof(ECustomPropSlot)))
				{
					if (CharacterType == ECharacterType.Premade)
					{
						CurrentPropDrawables[slot] = -1;
						CurrentPropTextures[slot] = 0;
					}
					else
					{
						CurrentPropDrawables[slot] = 0;
						CurrentPropTextures[slot] = 0;
					}
				}

				// Reapply custom gender
				if (CharacterType == ECharacterType.Custom)
				{
					for (int i = 0; i < 2; ++i)
					{
						SetFaceShape(i, 0, 0);
					}

					SetSkinPercentage_Shape(GetSkinPercentageForGender());
					SetSkinPercentage_Color(GetSkinPercentageForGender());
				}

				m_bIsFadingCharIn = true;
				m_bIsFadingCharOut = false;
				m_CurrentAlpha = RAGE.Elements.Player.LocalPlayer.GetAlpha();

				ApplyClothesToggleState();
			});
		}
	}

	public static float GetSkinPercentageForGender()
	{
		return Gender == EGender.Male ? 75.0f : 25.0f;
	}

	public void SetAge(int age)
	{
		g_Age = age;
	}

	public void SetPrimaryLanguage(ECharacterLanguage language)
	{
		PrimaryLanguage = language;
	}

	public void SetSecondaryLanguage(ECharacterLanguage language)
	{
		SecondaryLanguage = language;
	}

	public void SetSpawn(EScriptLocation location)
	{
		m_Spawn = location;
	}

	public float GetFaceBlendAsPercentile()
	{
		return g_FaceBlendFatherPercent * 100.0f;
	}

	public float GetColorBlendAsPercentile()
	{
		return g_SkinBlendFatherPercent * 100.0f;
	}

	// CUSTOM CHAR DATA
	EScriptLocation m_Spawn;
	int g_Age;
	int g_FaceBlend1Mother;
	int g_FaceBlend1Father;
	float g_FaceBlendFatherPercent;
	float g_SkinBlendFatherPercent;
	int g_HairColor;
	int g_HairColorHighlights;
	int g_EyeColor;
	int g_FacialHairStyle;
	int g_FacialHairColor;
	int g_FacialHairColorHighlight;
	float g_FacialHairOpacity;
	int g_Blemishes;
	float g_BlemishesOpacity;
	int g_Eyebrows;
	float g_EyebrowsOpacity;
	int g_EyebrowsColor;
	int g_EyebrowsColorHighlight;
	int g_Ageing;
	float g_AgeingOpacity;
	int g_Makeup;
	float g_MakeupOpacity;
	int g_MakeupColor;
	int g_MakeupColorHighlights;
	int g_Blush;
	float g_BlushOpacity;
	int g_BlushColor;
	int g_BlushColorHighlight;
	int g_Complexion;
	float g_ComplexionOpacity;
	int g_SunDamage;
	float g_SunDamageOpacity;
	int g_Lipstick;
	float g_LipstickOpacity;
	int g_LipstickColor;
	int g_LipstickColorHighlights;
	int g_MolesAndFreckles;
	float g_MolesAndFrecklesOpacity;
	int g_ChestHair;
	int g_ChestHairColor;
	int g_ChestHairColorHighlights;
	float g_ChestHairOpacity;
	int g_BodyBlemishes;
	float g_BodyBlemishesOpacity;
	float g_NoseSizeHorizontal;
	float g_NoseSizeVertical;
	float g_NoseSizeOutwards;
	float g_NoseSizeOutwardsUpper;
	float g_NoseSizeOutwardsLower;
	float g_NoseAngle;
	float g_EyebrowHeight;
	float g_EyebrowDepth;
	float g_CheekboneHeight;
	float g_CheekWidth;
	float g_CheekWidthLower;
	float g_EyeSize;
	float g_LipSize;
	float g_MouthSize;
	float g_MouthSizeLower;
	float g_ChinSize;
	float g_ChinSizeLower;
	float g_ChinWidth;
	float g_ChinEffect;
	float g_NeckWidth;
	float g_NeckWidthLower;
	int g_BaseHair;

	private void ResetCustomCharacterData()
	{
		if (g_Type != ECharacterCreationDataType.PlasticSurgeon)
		{
			g_CharacterTattoos.Clear();
			m_Spawn = EScriptLocation.LS;
			g_Age = 18;
			g_FaceBlend1Mother = CharacterConstants.g_CustomFaces_Female[0];
			g_FaceBlend1Father = CharacterConstants.g_CustomFaces_Male[0];
			g_FaceBlendFatherPercent = 0.0f;
			g_SkinBlendFatherPercent = 0.0f;
			g_HairColor = 0;
			g_HairColorHighlights = 0;
			g_BaseHair = -1;
			g_EyeColor = 0;
			g_ChestHair = -1;
			g_ChestHairColor = 0;
			g_ChestHairColorHighlights = 0;
			g_ChestHairOpacity = 1.0f;
			g_BodyBlemishes = -1;
			g_BodyBlemishesOpacity = 1.0f;
			g_FacialHairStyle = -1;
			g_FacialHairColor = 1;
			g_FacialHairColorHighlight = 1;
			g_FacialHairOpacity = 1.0f;
			g_Blemishes = -1;
			g_BlemishesOpacity = 1.0f;
			g_Eyebrows = -1;
			g_EyebrowsOpacity = 1.0f;
			g_EyebrowsColor = 1;
			g_EyebrowsColorHighlight = 1;
			g_Ageing = -1;
			g_AgeingOpacity = 1.0f;
			g_Makeup = -1;
			g_MakeupOpacity = 1.0f;
			g_MakeupColor = 0;
			g_MakeupColorHighlights = 0;
			g_Blush = -1;
			g_BlushOpacity = 1.0f;
			g_BlushColor = 0;
			g_BlushColorHighlight = 0;
			g_Complexion = -1;
			g_ComplexionOpacity = 1.0f;
			g_SunDamage = -1;
			g_SunDamageOpacity = 1.0f;
			g_Lipstick = -1;
			g_LipstickOpacity = 1.0f;
			g_LipstickColor = 0;
			g_LipstickColorHighlights = 0;
			g_MolesAndFreckles = -1;
			g_MolesAndFrecklesOpacity = 1.0f;
			g_NoseSizeHorizontal = 0.0f;
			g_NoseSizeVertical = 0.0f;
			g_NoseSizeOutwards = 0.0f;
			g_NoseSizeOutwardsUpper = 0.0f;
			g_NoseSizeOutwardsLower = 0.0f;
			g_NoseAngle = 0.0f;
			g_EyebrowHeight = 0.0f;
			g_EyebrowDepth = 0.0f;
			g_CheekboneHeight = 0.0f;
			g_CheekWidth = 0.0f;
			g_CheekWidthLower = 0.0f;
			g_EyeSize = 0.0f;
			g_LipSize = 0.0f;
			g_MouthSize = 0.0f;
			g_MouthSizeLower = 0.0f;
			g_ChinSize = 0.0f;
			g_ChinSizeLower = 0.0f;
			g_ChinWidth = 0.0f;
			g_ChinEffect = 0.0f;
			g_NeckWidth = 0.0f;
			g_NeckWidthLower = 0.0f;

			ApplyCustomCharData();
		}
	}

	public void ApplyCustomCharData()
	{
		RAGE.Elements.Player.LocalPlayer.SetHeadBlendData(g_FaceBlend1Mother, g_FaceBlend1Father, 0, g_FaceBlend1Mother, g_FaceBlend1Father, 0, g_FaceBlendFatherPercent, g_SkinBlendFatherPercent, 0, true);

		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(0, g_Blemishes, g_BlemishesOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(2, g_Eyebrows, g_EyebrowsOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(0, 1, 0, 0); // TODO_LAUNCH: Support color for this (Blemishes)
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(2, 1, g_EyebrowsColor, g_EyebrowsColorHighlight);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(3, g_Ageing, g_AgeingOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(3, 1, 0, 0); // TODO_LAUNCH: Support color for this
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(4, g_Makeup, g_MakeupOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(4, 1, g_MakeupColor, g_MakeupColorHighlights);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(5, g_Blush, g_BlushOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(5, 1, g_BlushColor, g_BlushColorHighlight);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(6, g_Complexion, g_ComplexionOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(7, g_SunDamage, g_SunDamageOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(8, g_Lipstick, g_LipstickOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(6, 1, 0, 0); // TODO_LAUNCH: Support color for this?
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(7, 1, 0, 0); // TODO_LAUNCH: Support color for this?
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(8, 2, g_LipstickColor, g_LipstickColorHighlights);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(9, g_MolesAndFreckles, g_MolesAndFrecklesOpacity);

		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay((int)EOverlayTypes.BodyBlemishes, g_BodyBlemishes, g_BodyBlemishesOpacity);
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor((int)EOverlayTypes.BodyBlemishes, 1, 0, 0);

		//RAGE.Elements.Player.LocalPlayer.SetHeadOverlay((int)EOverlayTypes.AddBodyBlemishes, g_ExtraBodyBlemishes, g_ExtraBodyBlemishesOpacity);
		//RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor((int)EOverlayTypes.AddBodyBlemishes, 1, 0, 0);

		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(9, 1, 0, 0); // TODO_LAUNCH: Support color for this?
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(0, g_NoseSizeHorizontal);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(1, g_NoseSizeVertical);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(2, g_NoseSizeOutwards);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(3, g_NoseSizeOutwardsUpper);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(4, g_NoseSizeOutwardsLower);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(5, g_NoseAngle);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(6, g_EyebrowHeight);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(7, g_EyebrowDepth);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(8, g_CheekboneHeight);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(9, g_CheekWidth);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(10, g_CheekWidthLower);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(11, g_EyeSize);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(12, g_LipSize);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(13, g_MouthSize);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(14, g_MouthSizeLower);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(15, g_ChinSize);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(16, g_ChinSizeLower);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(17, g_ChinWidth);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(18, g_ChinEffect);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(19, g_NeckWidth);
		RAGE.Elements.Player.LocalPlayer.SetFaceFeature(20, g_NeckWidthLower);
		RAGE.Elements.Player.LocalPlayer.SetEyeColor(g_EyeColor);


		if (g_Type != ECharacterCreationDataType.PlasticSurgeon)
		{
			// Hair
			RAGE.Elements.Player.LocalPlayer.SetHairColor(g_HairColor, g_HairColorHighlights);
			RAGE.Elements.Player.LocalPlayer.SetHeadOverlay(1, g_FacialHairStyle, g_FacialHairOpacity);
			RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor(1, 1, g_FacialHairColor, g_FacialHairColorHighlight);

			RAGE.Elements.Player.LocalPlayer.SetHeadOverlay((int)EOverlayTypes.ChestHair, g_ChestHair, g_ChestHairOpacity);
			RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor((int)EOverlayTypes.ChestHair, 1, g_ChestHairColor, g_ChestHairColorHighlights);

			// Clear base hair + Tattoos
			RAGE.Elements.Player.LocalPlayer.ClearFacialDecorations();

			// Base Hair
			if (g_BaseHair != -1)
			{
				CHairTattooDefinition hairTattooDef = TattooDefinitions.GetHairTattooDefinitionFromID(g_BaseHair);
				if (hairTattooDef != null)
				{
					RAGE.Elements.Player.LocalPlayer.SetFacialDecoration((uint)hairTattooDef.HairTattooCollection, (uint)hairTattooDef.HairTattooOverlay);
				}
			}

			// Tattoos
			foreach (CTattooDefinition tattooDef in g_CharacterTattoos)
			{
				RAGE.Elements.Player.LocalPlayer.SetFacialDecoration(tattooDef.GetHash_Collection(), tattooDef.GetHash_Tattoo(Gender));
			}

			// Also add the preview tattoo if available (the one the user is currently adding)
			if (g_PreviewTattoo != null)
			{
				RAGE.Elements.Player.LocalPlayer.SetFacialDecoration(g_PreviewTattoo.GetHash_Collection(), g_PreviewTattoo.GetHash_Tattoo(Gender));
			}
		}
	}

	public void SetBlemishes(int value) { g_Blemishes = value - 1; ApplyCustomCharData(); }
	public void SetEyeBrows(int value) { g_Eyebrows = value - 1; ApplyCustomCharData(); }
	public void SetFacialHair(int value) { g_FacialHairStyle = value - 1; ApplyCustomCharData(); }
	public void SetAgeing(int value) { g_Ageing = value - 1; ApplyCustomCharData(); }
	public void SetMakeup(int value) { g_Makeup = value - 1; ApplyCustomCharData(); }
	public void SetBlush(int value) { g_Blush = value - 1; ApplyCustomCharData(); }
	public void SetComplexion(int value) { g_Complexion = value - 1; ApplyCustomCharData(); }
	public void SetSunDamage(int value) { g_SunDamage = value - 1; ApplyCustomCharData(); }
	public void SetLipstick(int value) { g_Lipstick = value - 1; ApplyCustomCharData(); }
	public void SetMolesFreckles(int value) { g_MolesAndFreckles = value - 1; ApplyCustomCharData(); }
	public void SetLipstickColor(int value) { g_LipstickColor = value; ApplyCustomCharData(); }
	public void SetLipstickColorHighlights(int value) { g_LipstickColorHighlights = value; ApplyCustomCharData(); }

	public void SetMakeupColor(int value) { g_MakeupColor = value; ApplyCustomCharData(); }
	public void SetMakeupColorHighlights(int value) { g_MakeupColorHighlights = value; ApplyCustomCharData(); }

	public void SetBlushColor(int value) { g_BlushColor = value; ApplyCustomCharData(); }
	public void SetBlushColorHighlights(int value) { g_BlushColorHighlight = value; ApplyCustomCharData(); }
	public void SetEyeBrowsColor(int value) { g_EyebrowsColor = value; ApplyCustomCharData(); }
	public void SetEyeBrowsColorHighlights(int value) { g_EyebrowsColorHighlight = value; ApplyCustomCharData(); }
	public void SetFacialHairColor(int value) { g_FacialHairColor = value; ApplyCustomCharData(); }
	public void SetFacialHairColorHighlights(int value) { g_FacialHairColorHighlight = value; ApplyCustomCharData(); }
	public void SetHairColor(int value) { g_HairColor = value; ApplyCustomCharData(); }
	public void SetHairColorHighlights(int value) { g_HairColorHighlights = value; ApplyCustomCharData(); }
	public void SetBlushOpacity(float value) { g_BlushOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetFacialHairOpacity(float value) { g_FacialHairOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetComplexionOpacity(float value) { g_ComplexionOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetSunDamageOpacity(float value) { g_SunDamageOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetLipstickOpacity(float value) { g_LipstickOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetMolesFrecklesOpacity(float value) { g_MolesAndFrecklesOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetChestHair(int value) { g_ChestHair = value - 1; ApplyCustomCharData(); }
	public void SetChestHairColor(int value) { g_ChestHairColor = value; ApplyCustomCharData(); }
	public void SetChestHairColorHighlights(int value) { g_ChestHairColorHighlights = value; ApplyCustomCharData(); }
	public void SetChestHairOpacity(float value) { g_ChestHairOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetBodyBlemishes(int value) { g_BodyBlemishes = value - 1; ApplyCustomCharData(); }
	public void SetBodyBlemishesOpacity(float value) { g_BodyBlemishesOpacity = (value / 100.0f); ApplyCustomCharData(); }
	//public void SetExtraBodyBlemishes(int value) { g_ExtraBodyBlemishes = value - 1; ApplyCustomCharData(); }
	//public void SetExtraBodyBlemishesOpacity(float value) { g_ExtraBodyBlemishesOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetBlemishesOpacity(float value) { g_BlemishesOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetEyeBrowsOpacity(float value) { g_EyebrowsOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetAgeingOpacity(float value) { g_AgeingOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetMakeupOpacity(float value) { g_MakeupOpacity = (value / 100.0f); ApplyCustomCharData(); }
	public void SetEyeColor(int value) { g_EyeColor = value; ApplyCustomCharData(); }

	public void SetBaseHair(int value)
	{
		g_BaseHair = value - 1;
		ApplyCustomCharData();
	}

	public void SetHairStyleDrawable(int value)
	{
		// If greater than or equal to 23, we add one, so we skip the night vision (24 for female)
		// TODO_LATER: We might need a more flexible system for this, incase R* adds more things we want to remove
		if (Gender == EGender.Male)
		{
			if (value >= 23)
			{
				value++;
			}
		}
		else
		{
			if (value >= 24)
			{
				value++;
			}
		}

		SetComponentDrawable(ECustomClothingComponent.HairStyles, value);
	}

	public EGender GetGender()
	{
		return Gender;
	}

	private float ClampPercentageToMinus1and1Range(float fInput)
	{
		return ((fInput / 100.0f) * 2.0f) - 1;
	}

	public void SetNoseSizeHorizontal(float value) { g_NoseSizeHorizontal = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetNoseSizeVertical(float value) { g_NoseSizeVertical = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetNoseSizeOutwards(float value) { g_NoseSizeOutwards = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetNoseSizeOutwardsUpper(float value) { g_NoseSizeOutwardsUpper = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetNoseSizeOutwardsLower(float value) { g_NoseSizeOutwardsLower = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetNoseAngle(float value) { g_NoseAngle = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetEyebrowHeight(float value) { g_EyebrowHeight = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetEyebrowDepth(float value) { g_EyebrowDepth = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetCheekboneHeight(float value) { g_CheekboneHeight = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetCheekWidth(float value) { g_CheekWidth = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetCheekWidthLower(float value) { g_CheekWidthLower = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetEyeSize(float value) { g_EyeSize = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetLipSize(float value) { g_LipSize = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetMouthSize(float value) { g_MouthSize = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetMouthSizeLower(float value) { g_MouthSizeLower = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetChinSize(float value) { g_ChinSize = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetChinSizeUnderneath(float value) { g_ChinSizeLower = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetChinWidth(float value) { g_ChinWidth = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetChinEffect(float value) { g_ChinEffect = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetNeckWidth(float value) { g_NeckWidth = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }
	public void SetNeckWidthLower(float value) { g_NeckWidthLower = ClampPercentageToMinus1and1Range(value); ApplyCustomCharData(); }

	public void SetFaceShape(int index, int genderID, int value)
	{
		if (index == 1)
		{
			g_FaceBlend1Mother = value;
		}
		else if (index == 0)
		{
			g_FaceBlend1Father = value;
		}

		ApplyCustomCharData();
	}

	public void SetSkinPercentage_Shape(float value, bool bDoUIInversion = true)
	{
		// We subtract from 1.0 here because 0.0 is actually mother, 1.0 is father, but the UI we do parent 1 (father) on the left
		float valueAsFloatPercent = value / 100.0f;

		if (bDoUIInversion)
		{
			g_FaceBlendFatherPercent = 1.0f - Math.Clamp(valueAsFloatPercent, 0.0f, 1.0f);
		}
		else
		{
			g_FaceBlendFatherPercent = Math.Clamp(valueAsFloatPercent, 0.0f, 1.0f);
		}

		ApplyCustomCharData();
	}

	public void SetSkinPercentage_Color(float value, bool bDoUIInversion = true)
	{
		// We subtract from 1.0 here because 0.0 is actually mother, 1.0 is father, but the UI we do parent 1 (father) on the left
		float valueAsFloatPercent = value / 100.0f;
		if (bDoUIInversion)
		{
			g_SkinBlendFatherPercent = 1.0f - Math.Clamp(valueAsFloatPercent, 0.0f, 1.0f);
		}
		else
		{
			g_SkinBlendFatherPercent = Math.Clamp(valueAsFloatPercent, 0.0f, 1.0f);
		}

		ApplyCustomCharData();
	}

	public void TransmitPlasticSurgeonEvent(Int64 storeID)
	{
		NetworkEventSender.SendNetworkEvent_PlasticSurgeon_Checkout(
				storeID,
				g_Ageing,
				g_AgeingOpacity,
				g_Makeup,
				g_MakeupOpacity,
				g_MakeupColor,
				g_MakeupColorHighlights,
				g_Blush,
				g_BlushOpacity,
				g_BlushColor,
				g_BlushColorHighlight,
				g_Complexion,
				g_ComplexionOpacity,
				g_SunDamage,
				g_SunDamageOpacity,
				g_Lipstick,
				g_LipstickOpacity,
				g_LipstickColor,
				g_LipstickColorHighlights,
				g_MolesAndFreckles,
				g_MolesAndFrecklesOpacity,
				g_NoseSizeHorizontal,
				g_NoseSizeVertical,
				g_NoseSizeOutwards,
				g_NoseSizeOutwardsUpper,
				g_NoseSizeOutwardsLower,
				g_NoseAngle,
				g_EyebrowHeight,
				g_EyebrowDepth,
				g_CheekboneHeight,
				g_CheekWidth,
				g_CheekWidthLower,
				g_EyeSize,
				g_LipSize,
				g_MouthSize,
				g_MouthSizeLower,
				g_ChinSize,
				g_ChinSizeLower,
				g_ChinWidth,
				g_ChinEffect,
				g_NeckWidth,
				g_NeckWidthLower,
				g_FaceBlend1Mother,
				g_FaceBlend1Father,
				g_FaceBlendFatherPercent,
				g_SkinBlendFatherPercent,
				g_EyeColor,
				g_Blemishes,
				g_BlemishesOpacity,
				g_Eyebrows,
				g_EyebrowsOpacity,
				g_EyebrowsColor,
				g_EyebrowsColorHighlight,
				g_BodyBlemishes,
				g_BodyBlemishesOpacity);
	}

	public void TransmitCreateEvent(string strName)
	{
		if (IsCustom())
		{
			List<int> lstCondensedTattooList = new List<int>();
			foreach (CTattooDefinition tattooDef in g_CharacterTattoos)
			{
				lstCondensedTattooList.Add(tattooDef.ID);
			}

			NetworkEventSender.SendNetworkEvent_CreateCharacterCustom(m_Spawn, Gender, strName, SkinHash, g_Age, CurrentDrawables, CurrentTextures, CurrentPropDrawables,
				CurrentPropTextures,
				g_Ageing,
				g_AgeingOpacity,
				g_Makeup,
				g_MakeupOpacity,
				g_MakeupColor,
				g_MakeupColorHighlights,
				g_Blush,
				g_BlushOpacity,
				g_BlushColor,
				g_BlushColorHighlight,
				g_Complexion,
				g_ComplexionOpacity,
				g_SunDamage,
				g_SunDamageOpacity,
				g_Lipstick,
				g_LipstickOpacity,
				g_LipstickColor,
				g_LipstickColorHighlights,
				g_MolesAndFreckles,
				g_MolesAndFrecklesOpacity,
				g_NoseSizeHorizontal,
				g_NoseSizeVertical,
				g_NoseSizeOutwards,
				g_NoseSizeOutwardsUpper,
				g_NoseSizeOutwardsLower,
				g_NoseAngle,
				g_EyebrowHeight,
				g_EyebrowDepth,
				g_CheekboneHeight,
				g_CheekWidth,
				g_CheekWidthLower,
				g_EyeSize,
				g_LipSize,
				g_MouthSize,
				g_MouthSizeLower,
				g_ChinSize,
				g_ChinSizeLower,
				g_ChinWidth,
				g_ChinEffect,
				g_NeckWidth,
				g_NeckWidthLower,
				g_FaceBlend1Mother,
				g_FaceBlend1Father,
				g_FaceBlendFatherPercent,
				g_SkinBlendFatherPercent,
				g_BaseHair,
				g_HairColor,
				g_HairColorHighlights,
				g_EyeColor,
				g_FacialHairStyle,
				g_FacialHairColor,
				g_FacialHairColorHighlight,
				g_FacialHairOpacity,
				g_Blemishes,
				g_BlemishesOpacity,
				g_Eyebrows,
				g_EyebrowsOpacity,
				g_EyebrowsColor,
				g_EyebrowsColorHighlight,
				lstCondensedTattooList,
				g_BodyBlemishes,
				g_BodyBlemishesOpacity,
				g_ChestHair,
				g_ChestHairColor,
				g_ChestHairColorHighlights,
				g_ChestHairOpacity,
				PrimaryLanguage,
				SecondaryLanguage);
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_CreateCharacterPremade(m_Spawn, Gender, strName, SkinHash, g_Age, CurrentDrawables, CurrentTextures, CurrentPropDrawables, CurrentPropTextures, PrimaryLanguage, SecondaryLanguage);
		}
	}
}