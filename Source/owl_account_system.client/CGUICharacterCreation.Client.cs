using System;
using System.Collections.Generic;
using System.Linq;

internal class CGUICharacterCreation : CEFCore
{
	public CGUICharacterCreation(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/createv2.html", EGUIID.CharacterCreation, callbackOnLoad)
	{
		UIEvents.CharCreate_GotoHeadCam += CharacterCreation.GotoHeadCam;
		UIEvents.CharCreate_GotoBodyCam_Near += CharacterCreation.GotoBodyCam_Near;
		UIEvents.CharCreate_GotoBodyCam_Far += CharacterCreation.GotoBodyCam_Far;

		UIEvents.CharCreate_ToggleClothes += CharacterCreation.ToggleClothes;

		UIEvents.CharCreate_SetGender += CharacterCreation.ChangeGender;
		UIEvents.CharCreate_SetSpawn += CharacterCreation.ChangeSpawn;
		UIEvents.CharCreate_SetType += CharacterCreation.ChangeType;
		UIEvents.CharCreate_SetSkinID += CharacterCreation.SetSkinID;

		UIEvents.CharCreate_SetPropDrawable_Earrings += (int value) => { CharacterCreation.SetPropDrawable(ECustomPropSlot.Ears, value); };
		UIEvents.CharCreate_SetPropTexture_Earrings += (int value) => { CharacterCreation.SetPropTexture(ECustomPropSlot.Ears, value); };

		UIEvents.CharCreate_SetPropDrawable_Glasses += (int value) => { CharacterCreation.SetPropDrawable(ECustomPropSlot.Glasses, value); };
		UIEvents.CharCreate_SetPropTexture_Glasses += (int value) => { CharacterCreation.SetPropTexture(ECustomPropSlot.Glasses, value); };

		UIEvents.CharCreate_SetPropDrawable_Hats += (int value) => { CharacterCreation.SetPropDrawable(ECustomPropSlot.Hats, value); };
		UIEvents.CharCreate_SetPropTexture_Hats += (int value) => { CharacterCreation.SetPropTexture(ECustomPropSlot.Hats, value); };

		UIEvents.CharCreate_SetPropDrawable_Watches += (int value) => { CharacterCreation.SetPropDrawable(ECustomPropSlot.Watches, value); };
		UIEvents.CharCreate_SetPropTexture_Watches += (int value) => { CharacterCreation.SetPropTexture(ECustomPropSlot.Watches, value); };

		UIEvents.CharCreate_SetPropDrawable_Bracelets += (int value) => { CharacterCreation.SetPropDrawable(ECustomPropSlot.Bracelets, value); };
		UIEvents.CharCreate_SetPropTexture_Bracelets += (int value) => { CharacterCreation.SetPropTexture(ECustomPropSlot.Bracelets, value); };

		UIEvents.CharCreate_SetComponentDrawable_Accessories += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Accessories, value); };
		UIEvents.CharCreate_SetComponentPalette_Accessories += (int value) => { CharacterCreation.SetComponentPalette(ECustomClothingComponent.Accessories, value); };
		UIEvents.CharCreate_SetComponentTexture_Accessories += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Accessories, value); };

		UIEvents.CharCreate_SetComponentDrawable_Decals += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Decals, value); };
		UIEvents.CharCreate_SetComponentPalette_Decals += (int value) => { CharacterCreation.SetComponentPalette(ECustomClothingComponent.Decals, value); };
		UIEvents.CharCreate_SetComponentTexture_Decals += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Decals, value); };

		UIEvents.CharCreate_SetComponentDrawable_FullBeards += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Masks, value); };
		UIEvents.CharCreate_SetComponentTexture_FullBeards += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Masks, value); };

		UIEvents.CharCreate_SetComponentDrawable_Legs += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Legs, value); };
		UIEvents.CharCreate_SetComponentPalette_Legs += (int value) => { CharacterCreation.SetComponentPalette(ECustomClothingComponent.Legs, value); };
		UIEvents.CharCreate_SetComponentTexture_Legs += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Legs, value); };

		UIEvents.CharCreate_SetComponentDrawable_Shoes += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Shoes, value); };
		UIEvents.CharCreate_SetComponentPalette_Shoes += (int value) => { CharacterCreation.SetComponentPalette(ECustomClothingComponent.Shoes, value); };
		UIEvents.CharCreate_SetComponentTexture_Shoes += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Shoes, value); };

		UIEvents.CharCreate_SetComponentDrawable_Tops += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Tops, value); };
		UIEvents.CharCreate_SetComponentPalette_Tops += (int value) => { CharacterCreation.SetComponentPalette(ECustomClothingComponent.Tops, value); };
		UIEvents.CharCreate_SetComponentTexture_Tops += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Tops, value); };

		UIEvents.CharCreate_SetComponentDrawable_Torso += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Torsos, value); };
		UIEvents.CharCreate_SetComponentPalette_Torso += (int value) => { CharacterCreation.SetComponentPalette(ECustomClothingComponent.Torsos, value); };
		UIEvents.CharCreate_SetComponentTexture_Torso += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Torsos, value); };

		UIEvents.CharCreate_SetComponentDrawable_Undershirts += (int value) => { CharacterCreation.SetComponentDrawable(ECustomClothingComponent.Undershirts, value); };
		UIEvents.CharCreate_SetComponentPalette_Undershirts += (int value) => { CharacterCreation.SetComponentPalette(ECustomClothingComponent.Undershirts, value); };
		UIEvents.CharCreate_SetComponentTexture_Undershirts += (int value) => { CharacterCreation.SetComponentTexture(ECustomClothingComponent.Undershirts, value); };

		UIEvents.CharCreate_OnRootChanged_Accessories += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Accessories, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Decals += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Decals, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_FullBeards += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Masks, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Legs += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Legs, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Shoes += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Shoes, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Tops += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Tops, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Torso += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Torsos, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Undershirts += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForComponent(ECustomClothingComponent.Undershirts, strElementToReset); };

		UIEvents.CharCreate_OnRootChanged_Hats += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForProp(ECustomPropSlot.Hats, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Bracelets += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForProp(ECustomPropSlot.Bracelets, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Watches += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForProp(ECustomPropSlot.Watches, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Earrings += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForProp(ECustomPropSlot.Ears, strElementToReset); };
		UIEvents.CharCreate_OnRootChanged_Glasses += (string strElementToReset) => { CharacterCreation.UpdateMaxTextureForProp(ECustomPropSlot.Glasses, strElementToReset); };

		UIEvents.CharCreate_SetLanguage += CharacterCreation.SetPrimaryLanguage;
		UIEvents.CharCreate_SetSecondLanguage += CharacterCreation.SetSecondaryLanguage;
		UIEvents.CharCreate_SetAge += CharacterCreation.SetAge;
		UIEvents.CharCreate_StartRotation += CharacterCreation.StartRotation;
		UIEvents.CharCreate_StopRotation += CharacterCreation.StopRotation;
		UIEvents.CharCreate_ResetRotation += CharacterCreation.ResetRotation;
		UIEvents.CharCreate_SetCharacterName += CharacterCreation.SetCharacterName;
		UIEvents.CharCreate_CreateCharacter += CharacterCreation.OfferCreateCharacter;
		UIEvents.CharCreate_OfferCreateCharacter_Create += CharacterCreation.OfferCreateCharacter_Create;
		UIEvents.CharCreate_OfferCreateCharacter_Cancel += CharacterCreation.OfferCreateCharacter_Cancel;
		UIEvents.CharCreate_SetBlemishes += CharacterCreation.SetBlemishes;
		UIEvents.CharCreate_SetEyeBrows += CharacterCreation.SetEyeBrows;
		UIEvents.CharCreate_SetFacialHair += CharacterCreation.SetFacialHair;
		UIEvents.CharCreate_SetAgeing += CharacterCreation.SetAgeing;
		UIEvents.CharCreate_SetMakeup += CharacterCreation.SetMakeup;
		UIEvents.CharCreate_SetMakeupColor += CharacterCreation.SetMakeupColor;
		UIEvents.CharCreate_SetMakeupColorHighlights += CharacterCreation.SetMakeupColorHighlights;
		UIEvents.CharCreate_SetBlush += CharacterCreation.SetBlush;
		UIEvents.CharCreate_SetComplexion += CharacterCreation.SetComplexion;
		UIEvents.CharCreate_SetSunDamage += CharacterCreation.SetSunDamage;
		UIEvents.CharCreate_SetLipstick += CharacterCreation.SetLipstick;
		UIEvents.CharCreate_SetMolesFreckles += CharacterCreation.SetMolesFreckles;
		UIEvents.CharCreate_SetLipstickColor += CharacterCreation.SetLipstickColor;
		UIEvents.CharCreate_SetLipstickColorHighlights += CharacterCreation.SetLipstickColorHighlights;
		UIEvents.CharCreate_SetBlushColor += CharacterCreation.SetBlushColor;
		UIEvents.CharCreate_SetBlushColorHighlights += CharacterCreation.SetBlushColorHighlights;
		UIEvents.CharCreate_SetEyeBrowsColor += CharacterCreation.SetEyeBrowsColor;
		UIEvents.CharCreate_SetEyeBrowsColorHighlights += CharacterCreation.SetEyeBrowsColorHighlights;
		UIEvents.CharCreate_SetFacialHairColor += CharacterCreation.SetFacialHairColor;
		UIEvents.CharCreate_SetFacialHairColorHighlights += CharacterCreation.SetFacialHairColorHighlights;
		UIEvents.CharCreate_SetHairColor += CharacterCreation.SetHairColor;
		UIEvents.CharCreate_SetHairColorHighlights += CharacterCreation.SetHairColorHighlights;
		UIEvents.CharCreate_SetBlushOpacity += CharacterCreation.SetBlushOpacity;
		UIEvents.CharCreate_SetFacialHairOpacity += CharacterCreation.SetFacialHairOpacity;
		UIEvents.CharCreate_SetComplexionOpacity += CharacterCreation.SetComplexionOpacity;
		UIEvents.CharCreate_SetSunDamageOpacity += CharacterCreation.SetSunDamageOpacity;
		UIEvents.CharCreate_SetLipstickOpacity += CharacterCreation.SetLipstickOpacity;
		UIEvents.CharCreate_SetMolesFrecklesOpacity += CharacterCreation.SetMolesFrecklesOpacity;
		UIEvents.CharCreate_SetChestHair += CharacterCreation.SetChestHair;
		UIEvents.CharCreate_SetChestHairColor += CharacterCreation.SetChestHairColor;
		UIEvents.CharCreate_SetChestHairColorHighlights += CharacterCreation.SetChestHairColorHighlights;
		UIEvents.CharCreate_SetChestHairOpacity += CharacterCreation.SetChestHairOpacity;

		UIEvents.CharCreate_Tattoo_Cancel += CharacterCreation.Tattoo_Cancel;
		UIEvents.CharCreate_Tattoo_AddNew += CharacterCreation.Tattoo_AddNew;
		UIEvents.CharCreate_Tattoo_Create += CharacterCreation.Tattoo_Create;
		UIEvents.CharCreate_Tattoo_RemoveTattoo += CharacterCreation.Tattoo_RemoveTattoo;
		UIEvents.CharCreate_Tattoo_ChangeZone += CharacterCreation.Tattoo_ChangeZone;
		UIEvents.CharCreate_Tattoo_ChangeTattoo += CharacterCreation.Tattoo_ChangeTattoo;
		UIEvents.CharCreate_SetBodyBlemishes += CharacterCreation.SetBodyBlemishes;
		UIEvents.CharCreate_SetBodyBlemishesOpacity += CharacterCreation.SetBodyBlemishesOpacity;
		//UIEvents.CharCreate_SetExtraBodyBlemishes += CharacterCreation.SetExtraBodyBlemishes;
		//UIEvents.CharCreate_SetExtraBodyBlemishesOpacity += CharacterCreation.SetExtraBodyBlemishesOpacity;
		UIEvents.CharCreate_SetBlemishesOpacity += CharacterCreation.SetBlemishesOpacity;
		UIEvents.CharCreate_SetEyeBrowsOpacity += CharacterCreation.SetEyeBrowsOpacity;
		UIEvents.CharCreate_SetAgeingOpacity += CharacterCreation.SetAgeingOpacity;
		UIEvents.CharCreate_SetMakeupOpacity += CharacterCreation.SetMakeupOpacity;
		UIEvents.CharCreate_SetEyeColor += CharacterCreation.SetEyeColor;
		UIEvents.CharCreate_SetBaseHair += CharacterCreation.SetBaseHair;
		UIEvents.CharCreate_SetHairStyleDrawable += CharacterCreation.SetHairStyleDrawable;
		UIEvents.CharCreate_SetNoseSizeHorizontal += CharacterCreation.SetNoseSizeHorizontal;
		UIEvents.CharCreate_SetNoseSizeVertical += CharacterCreation.SetNoseSizeVertical;
		UIEvents.CharCreate_SetNoseSizeOutwards += CharacterCreation.SetNoseSizeOutwards;
		UIEvents.CharCreate_SetNoseSizeOutwardsUpper += CharacterCreation.SetNoseSizeOutwardsUpper;
		UIEvents.CharCreate_SetNoseSizeOutwardsLower += CharacterCreation.SetNoseSizeOutwardsLower;
		UIEvents.CharCreate_SetNoseAngle += CharacterCreation.SetNoseAngle;
		UIEvents.CharCreate_SetEyebrowHeight += CharacterCreation.SetEyebrowHeight;
		UIEvents.CharCreate_SetEyebrowDepth += CharacterCreation.SetEyebrowDepth;
		UIEvents.CharCreate_SetCheekboneHeight += CharacterCreation.SetCheekboneHeight;
		UIEvents.CharCreate_SetCheekWidth += CharacterCreation.SetCheekWidth;
		UIEvents.CharCreate_SetCheekWidthLower += CharacterCreation.SetCheekWidthLower;
		UIEvents.CharCreate_SetEyeSize += CharacterCreation.SetEyeSize;
		UIEvents.CharCreate_SetLipSize += CharacterCreation.SetLipSize;
		UIEvents.CharCreate_SetMouthSize += CharacterCreation.SetMouthSize;
		UIEvents.CharCreate_SetMouthSizeLower += CharacterCreation.SetMouthSizeLower;
		UIEvents.CharCreate_SetChinSize += CharacterCreation.SetChinSize;
		UIEvents.CharCreate_SetChinSizeUnderneath += CharacterCreation.SetChinSizeUnderneath;
		UIEvents.CharCreate_SetChinWidth += CharacterCreation.SetChinWidth;
		UIEvents.CharCreate_SetChinEffect += CharacterCreation.SetChinEffect;
		UIEvents.CharCreate_SetNeckWidth += CharacterCreation.SetNeckWidth;
		UIEvents.CharCreate_SetNeckWidthLower += CharacterCreation.SetNeckWidthLower;
		UIEvents.CharCreate_SetFaceShape += CharacterCreation.SetFaceShape;
		UIEvents.CharCreate_SetSkinPercentage_Shape += CharacterCreation.SetSkinPercentage_Shape;
		UIEvents.CharCreate_SetSkinPercentage_Color += CharacterCreation.SetSkinPercentage_Color;
	}

	public override void OnLoad()
	{

	}

	public void Show()
	{
		SetVisible(true, true, false);
	}

	public void SetMaxPedSkinIndexForThisGender(EGender a_Gender, int maxSkins)
	{
		Execute("SetMaxPedSkinIndexForThisGender", Convert.ToInt32(a_Gender), maxSkins);
	}

	public void SetMaxCustomSkinFaceIndexForThisGender(EGender a_Gender, int maxSkins)
	{
		Execute("SetMaxCustomSkinFaceIndexForThisGender", Convert.ToInt32(a_Gender), maxSkins);
	}


	public void SetHairStyleDrawable_Max(int max) { Execute("SetHairStyleDrawable_Max", max); }
	public void SetHairColor_Max(int max) { Execute("SetHairColor_Max", max); }
	public void SetHairColorHighlights_Max(int max) { Execute("SetHairColorHighlights_Max", max); }
	public void SetFacialHairColor_Max(int max) { Execute("SetFacialHairColor_Max", max); }
	public void SetFacialHairColorHighlights_Max(int max) { Execute("SetFacialHairColorHighlights_Max", max); }
	public void SetFacialHair_Max(int max) { Execute("SetFacialHair_Max", max); }
	public void SetEyeColor_Max(int max) { Execute("SetEyeColor_Max", max); }
	public void SetAgeing_Max(int max) { Execute("SetAgeing_Max", max); }
	public void SetEyeBrowsColor_Max(int max) { Execute("SetEyeBrowsColor_Max", max); }
	public void SetEyeBrowsColorHighlights_Max(int max) { Execute("SetEyeBrowsColorHighlights_Max", max); }
	public void SetEyeBrows_Max(int max) { Execute("SetEyeBrows_Max", max); }
	public void SetBlemishes_Max(int max) { Execute("SetBlemishes_Max", max); }
	public void SetMakeup_Max(int max) { Execute("SetMakeup_Max", max); }
	public void SetBlush_Max(int max) { Execute("SetBlush_Max", max); }
	public void SetBlushColor_Max(int max) { Execute("SetBlushColor_Max", max); }
	public void SetBlushColorHighlights_Max(int max) { Execute("SetBlushColorHighlights_Max", max); }
	public void SetComplexion_Max(int max) { Execute("SetComplexion_Max", max); }
	public void SetSunDamage_Max(int max) { Execute("SetSunDamage_Max", max); }
	public void SetLipstick_Max(int max) { Execute("SetLipstick_Max", max); }
	public void SetLipstickColor_Max(int max) { Execute("SetLipstickColor_Max", max); }
	public void SetLipstickColorHighlights_Max(int max) { Execute("SetLipstickColorHighlights_Max", max); }
	public void SetMolesFreckles_Max(int max) { Execute("SetMolesFreckles_Max", max); }
	public void SetClothingFaceMax(int max) { Execute("SetClothingFaceMax", max); }
	public void SetClothingTorsoMax(int max) { Execute("SetClothingTorsoMax", max); }
	public void SetClothingLegsMax(int max) { Execute("SetClothingLegsMax", max); }
	public void SetClothingFeetMax(int max) { Execute("SetClothingFeetMax", max); }
	public void SetClothingTorsoAdditionalMax(int max) { Execute("SetClothingTorsoAdditionalMax", max); }
	public void SetClothingAccessoriesMax(int max) { Execute("SetClothingAccessoriesMax", max); }
	public void SetClothingEyesMax(int max) { Execute("SetClothingEyesMax", max); }
	public void SetClothingTexturesMax(int max) { Execute("SetClothingTexturesMax", max); }
	public void SetPropHelmet_Max(int max) { Execute("SetPropHelmet_Max", max); }
	public void SetPropGlasses_Max(int max) { Execute("SetPropGlasses_Max", max); }
	public void SetPropEaring_Max(int max) { Execute("SetPropEaring_Max", max); }

	// BEGIN V2
	public void AddHeritage(EGender gender, int value)
	{
		Execute("AddHeritage", gender, value);
	}

	public void AddTabContent_HeritageSelector(uint tabID, string strTitle, int callbackID, EGender gender, string strDescription, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_HeritageSelector", tabID, strTitle, callbackID, gender, strDescription, onChangeEvent.ToString());
	}

	public void AddTabContent_Slider(uint tabID, string strTitle, string strDescription, string strLeftText, string strRightText, uint minVal, uint maxVal, uint defaultVal, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_Slider", tabID, strTitle, strDescription, strLeftText, strRightText, minVal, maxVal, defaultVal, onChangeEvent.ToString());
	}

	public void AddTabContent_NumberSelector(uint tabID, string strTitle, string strDescription, uint minVal, uint maxVal, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_NumberSelector", tabID, strTitle, strDescription, minVal, maxVal, onChangeEvent.ToString());
	}

	public void AddTabContent_GenericButton(uint tabID, string strCaption, UIEventID onPressEvent)
	{
		Execute("AddTabContent_GenericButton", tabID, strCaption, onPressEvent.ToString());
	}

	public void AddTabContent_ClothingSelector(uint tabID, string strTitle, string box1_name, int box1_min, int box1_max, UIEventID box1_event, string box2_name, int box2_min, int box2_max, UIEventID box2_event, UIEventID eventOnRootChanged)
	{
		if (box1_min < 0) { box1_min = 0; }
		if (box1_max < 0) { box1_max = 15; }
		if (box2_min < 0) { box2_min = 0; }
		if (box2_max < 0) { box2_max = 15; }
		Execute("AddTabContent_ClothingSelector", tabID, strTitle, box1_name, box1_min, box1_max, box1_event.ToString(), box2_name, box2_min, box2_max, box2_event.ToString(), eventOnRootChanged.ToString()); ;
	}


	public void AddPendingColor(uint colorID, string strHexCode)
	{
		Execute("AddPendingColor", colorID, strHexCode);
	}

	public void AddTabContent_ColorPicker(uint tabID, string strTitle, string strDescription, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_ColorPicker", tabID, strTitle, strDescription, onChangeEvent.ToString());
	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void AddTab(string strName, string strIcon, UIEventID onChangeEvent)
	{
		Execute("AddTab", strName, strIcon, onChangeEvent.ToString());
	}

	public void AddSeperator(uint tabID)
	{
		Execute("AddSeperator", tabID);
	}

	public void AddTabContent_Textbox(uint tabID, string strTitle, string strDescription, uint maxLength, bool bAddValidator, UIEventID onChangeEvent, string strDefault)
	{
		Execute("AddTabContent_Textbox", tabID, strTitle, strDescription, maxLength, bAddValidator, onChangeEvent.ToString(), strDefault);
	}

	public void AddTabContent_TwoRadioOptions(uint tabID, string strTitle, string strDescription, string strRadioButton1, string strRadioButton2, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_TwoRadioOptions", tabID, strTitle, strDescription, strRadioButton1, strRadioButton2, onChangeEvent.ToString());
	}

	public void AddPendingDropdownItem(string strValue, string strName)
	{
		Execute("AddPendingDropdownItem", strValue, strName);
	}

	public void AddTabContent_Dropdown(uint tabID, string strTitle, string strDescription, UIEventID onChangeEvent)
	{
		Execute("AddTabContent_Dropdown", tabID, strTitle, strDescription, onChangeEvent.ToString());
	}

	public void AddTabContent_LanguageDropdown(uint tabID, string strTitle, string strDescription, UIEventID onChangeEvent, bool bIsSecondary)
	{
		Execute("AddTabContent_Language", tabID, strTitle, strDescription, onChangeEvent.ToString(), bIsSecondary);
	}

	public void AddTattooListItem(int ID, string strDisplayName)
	{
		Execute("AddTattooListItem", ID, strDisplayName);
	}

	private IEnumerable<ECharacterLanguage> GetAllLanguages()
	{
		return Enum.GetValues(typeof(ECharacterLanguage)).Cast<ECharacterLanguage>().OrderBy(c => c.ToString());
	}

	public void AddLanguageListItems()
	{
		foreach (ECharacterLanguage language in GetAllLanguages())
		{
			if (language != ECharacterLanguage.None)
			{
				Execute("LoadLanguageInCharUI", language.ToString());
			}
		}
	}

	public void CommitTattooList()
	{
		Execute("CommitTattooList");
	}

	public void ShowTattooCreator()
	{
		Execute("ShowTattooCreator");
	}

	public void AddTabContent_GenericListItem(uint tabID, string strTitle, string strSubtitle, string strTinyText, UIEventID onClickEvent, int callbackID)
	{
		Execute("AddTabContent_GenericListItem", tabID, strTitle, strSubtitle, strTinyText, onClickEvent.ToString(), callbackID);
	}

	public void DeleteTabContent_GenericListItem(string strElementName)
	{
		Execute("DeleteTabContent_GenericListItem", strElementName);
	}

	public void SetTabName(uint tabID, string strName)
	{
		Execute("SetTabName", tabID, strName);
	}

	public void ShowCreationUI()
	{
		RAGE.Chat.Show(false);
		Execute("ShowCreationUI");
	}

	public void HideCreationUI()
	{
		Execute("HideCreationUI");
	}

	public void SetMaxForElement(string strElementName, int newMax)
	{
		Execute("SetMaxForElement", strElementName, newMax);
	}
}