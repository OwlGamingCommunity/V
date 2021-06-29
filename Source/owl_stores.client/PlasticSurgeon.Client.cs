using System;
using System.Collections.Generic;
using System.Linq;

public class PlasticSurgeon : GenericCharacterCustomization
{
	private CCharacterCreationData m_CharData = null;

	public PlasticSurgeon() : base(EGUIID.PlasticSurgeon)
	{
		SetNameAndCallbacks("Plastic Surgeon", null, OnFinish, OnRequestShow, null, OnExit, OnRender);

		NetworkEvents.PlasticSurgeon_GotPrice += GotPriceInfo;
		NetworkEvents.EnterPlasticSurgeon_Response += OnEnterPlasticSurgeonResponse;
		NetworkEvents.EnterPlasticSurgeon_OfferCharacterUpgrade += OnEnterPlasticSurgeonOfferCharacterUpgrade;

		UIEvents.PlasticSurgeonOfferCharacterUpgrade_Confirm += OnOfferCharacterUpgrade_Confirm;
		UIEvents.PlasticSurgeonOfferCharacterUpgrade_Decline += OnOfferCharacterUpgrade_Decline;

		// UI EVENTS
		UIEvents.PlasticSurgeon_SetBlemishes += OnSetBlemishes;
		UIEvents.PlasticSurgeon_SetEyeBrows += OnSetEyeBrows;
		UIEvents.PlasticSurgeon_SetAgeing += OnSetAgeing;
		UIEvents.PlasticSurgeon_SetMakeup += OnSetMakeup;
		UIEvents.PlasticSurgeon_SetMakeupColor += OnSetMakeupColor;
		UIEvents.PlasticSurgeon_SetMakeupColorHighlights += OnSetMakeupColorHighlights;
		UIEvents.PlasticSurgeon_SetBlush += OnSetBlush;
		UIEvents.PlasticSurgeon_SetComplexion += OnSetComplexion;
		UIEvents.PlasticSurgeon_SetSunDamage += OnSetSunDamage;
		UIEvents.PlasticSurgeon_SetLipstick += OnSetLipstick;
		UIEvents.PlasticSurgeon_SetMolesFreckles += OnSetMolesFreckles;
		UIEvents.PlasticSurgeon_SetLipstickColor += OnSetLipstickColor;
		UIEvents.PlasticSurgeon_SetLipstickColorHighlights += OnSetLipstickColorHighlights;
		UIEvents.PlasticSurgeon_SetBlushColor += OnSetBlushColor;
		UIEvents.PlasticSurgeon_SetBlushColorHighlights += OnSetBlushColorHighlights;
		UIEvents.PlasticSurgeon_SetEyeBrowsColor += OnSetEyeBrowsColor;
		UIEvents.PlasticSurgeon_SetEyeBrowsColorHighlights += OnSetEyeBrowsColorHighlights;
		UIEvents.PlasticSurgeon_SetBlushOpacity += OnSetBlushOpacity;
		UIEvents.PlasticSurgeon_SetBlushOpacity_Finalize += OnSetBlushOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetComplexionOpacity += OnSetComplexionOpacity;
		UIEvents.PlasticSurgeon_SetComplexionOpacity_Finalize += OnSetComplexionOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetSunDamageOpacity += OnSetSunDamageOpacity;
		UIEvents.PlasticSurgeon_SetSunDamageOpacity_Finalize += OnSetSunDamageOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetLipstickOpacity += OnSetLipstickOpacity;
		UIEvents.PlasticSurgeon_SetLipstickOpacity_Finalize += OnSetLipstickOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetMolesFrecklesOpacity += OnSetMolesFrecklesOpacity;
		UIEvents.PlasticSurgeon_SetMolesFrecklesOpacity_Finalize += OnSetMolesFrecklesOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetBodyBlemishes += OnSetBodyBlemishes;
		UIEvents.PlasticSurgeon_SetBodyBlemishesOpacity += OnSetBodyBlemishesOpacity;
		UIEvents.PlasticSurgeon_SetBodyBlemishesOpacity_Finalize += OnSetBodyBlemishesOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetBlemishesOpacity += OnSetBlemishesOpacity;
		UIEvents.PlasticSurgeon_SetBlemishesOpacity_Finalize += OnSetBlemishesOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetEyeBrowsOpacity += OnSetEyeBrowsOpacity;
		UIEvents.PlasticSurgeon_SetEyeBrowsOpacity_Finalize += OnSetEyeBrowsOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetAgeingOpacity += OnSetAgeingOpacity;
		UIEvents.PlasticSurgeon_SetAgeingOpacity_Finalize += OnSetAgeingOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetMakeupOpacity += OnSetMakeupOpacity;
		UIEvents.PlasticSurgeon_SetMakeupOpacity_Finalize += OnSetMakeupOpacity_Finalize;
		UIEvents.PlasticSurgeon_SetEyeColor += OnSetEyeColor;
		UIEvents.PlasticSurgeon_SetNoseSizeHorizontal += OnSetNoseSizeHorizontal;
		UIEvents.PlasticSurgeon_SetNoseSizeHorizontal_Finalize += OnSetNoseSizeHorizontal_Finalize;
		UIEvents.PlasticSurgeon_SetNoseSizeVertical += OnSetNoseSizeVertical;
		UIEvents.PlasticSurgeon_SetNoseSizeVertical_Finalize += OnSetNoseSizeVertical_Finalize;
		UIEvents.PlasticSurgeon_SetNoseSizeOutwards += OnSetNoseSizeOutwards;
		UIEvents.PlasticSurgeon_SetNoseSizeOutwards_Finalize += OnSetNoseSizeOutwards_Finalize;
		UIEvents.PlasticSurgeon_SetNoseSizeOutwardsUpper += OnSetNoseSizeOutwardsUpper;
		UIEvents.PlasticSurgeon_SetNoseSizeOutwardsUpper_Finalize += OnSetNoseSizeOutwardsUpper_Finalize;
		UIEvents.PlasticSurgeon_SetNoseSizeOutwardsLower += OnSetNoseSizeOutwardsLower;
		UIEvents.PlasticSurgeon_SetNoseSizeOutwardsLower_Finalize += OnSetNoseSizeOutwardsLower_Finalize;
		UIEvents.PlasticSurgeon_SetNoseAngle += OnSetNoseAngle;
		UIEvents.PlasticSurgeon_SetNoseAngle_Finalize += OnSetNoseAngle_Finalize;
		UIEvents.PlasticSurgeon_SetEyebrowHeight += OnSetEyebrowHeight;
		UIEvents.PlasticSurgeon_SetEyebrowHeight_Finalize += OnSetEyebrowHeight_Finalize;
		UIEvents.PlasticSurgeon_SetEyebrowDepth += OnSetEyebrowDepth;
		UIEvents.PlasticSurgeon_SetEyebrowDepth_Finalize += OnSetEyebrowDepth_Finalize;
		UIEvents.PlasticSurgeon_SetCheekboneHeight += OnSetCheekboneHeight;
		UIEvents.PlasticSurgeon_SetCheekboneHeight_Finalize += OnSetCheekboneHeight_Finalize;
		UIEvents.PlasticSurgeon_SetCheekWidth += OnSetCheekWidth;
		UIEvents.PlasticSurgeon_SetCheekWidth_Finalize += OnSetCheekWidth_Finalize;
		UIEvents.PlasticSurgeon_SetCheekWidthLower += OnSetCheekWidthLower;
		UIEvents.PlasticSurgeon_SetCheekWidthLower_Finalize += OnSetCheekWidthLower_Finalize;
		UIEvents.PlasticSurgeon_SetEyeSize += OnSetEyeSize;
		UIEvents.PlasticSurgeon_SetEyeSize_Finalize += OnSetEyeSize_Finalize;
		UIEvents.PlasticSurgeon_SetLipSize += OnSetLipSize;
		UIEvents.PlasticSurgeon_SetLipSize_Finalize += OnSetLipSize_Finalize;
		UIEvents.PlasticSurgeon_SetMouthSize += OnSetMouthSize;
		UIEvents.PlasticSurgeon_SetMouthSize_Finalize += OnSetMouthSize_Finalize;
		UIEvents.PlasticSurgeon_SetMouthSizeLower += OnSetMouthSizeLower;
		UIEvents.PlasticSurgeon_SetMouthSizeLower_Finalize += OnSetMouthSizeLower_Finalize;
		UIEvents.PlasticSurgeon_SetChinSize += OnSetChinSize;
		UIEvents.PlasticSurgeon_SetChinSize_Finalize += OnSetChinSize_Finalize;
		UIEvents.PlasticSurgeon_SetChinSizeUnderneath += OnSetChinSizeUnderneath;
		UIEvents.PlasticSurgeon_SetChinSizeUnderneath_Finalize += OnSetChinSizeUnderneath_Finalize;
		UIEvents.PlasticSurgeon_SetChinWidth += OnSetChinWidth;
		UIEvents.PlasticSurgeon_SetChinWidth_Finalize += OnSetChinWidth_Finalize;
		UIEvents.PlasticSurgeon_SetChinEffect += OnSetChinEffect;
		UIEvents.PlasticSurgeon_SetChinEffect_Finalize += OnSetChinEffect_Finalize;
		UIEvents.PlasticSurgeon_SetNeckWidth += OnSetNeckWidth;
		UIEvents.PlasticSurgeon_SetNeckWidth_Finalize += OnSetNeckWidth_Finalize;
		UIEvents.PlasticSurgeon_SetNeckWidthLower += OnSetNeckWidthLower;
		UIEvents.PlasticSurgeon_SetNeckWidthLower_Finalize += OnSetNeckWidthLower_Finalize;
		UIEvents.PlasticSurgeon_SetFaceShape += OnSetFaceShape;
		UIEvents.PlasticSurgeon_SetSkinPercentage_Shape += OnSetSkinPercentage_Shape;
		UIEvents.PlasticSurgeon_SetSkinPercentage_Shape_Finalize += OnSetSkinPercentage_Shape_Finalize;
		UIEvents.PlasticSurgeon_SetSkinPercentage_Color += OnSetSkinPercentage_Color;
		UIEvents.PlasticSurgeon_SetSkinPercentage_Color_Finalize += OnSetSkinPercentage_Color_Finalize;
	}

	// UI CALLBACKS
	private void OnSetBlemishes(int value) { m_CharData.SetBlemishes(value); }
	private void OnSetEyeBrows(int value) { m_CharData.SetEyeBrows(value); }
	private void OnSetAgeing(int value) { m_CharData.SetAgeing(value); }
	private void OnSetMakeup(int value) { m_CharData.SetMakeup(value); }
	private void OnSetMakeupColor(int value) { m_CharData.SetMakeupColor(value); }
	private void OnSetMakeupColorHighlights(int value) { m_CharData.SetMakeupColorHighlights(value); }
	private void OnSetBlush(int value) { m_CharData.SetBlush(value); }
	private void OnSetComplexion(int value) { m_CharData.SetComplexion(value); }
	private void OnSetSunDamage(int value) { m_CharData.SetSunDamage(value); }
	private void OnSetLipstick(int value) { m_CharData.SetLipstick(value); }
	private void OnSetMolesFreckles(int value) { m_CharData.SetMolesFreckles(value); }
	private void OnSetLipstickColor(int value) { m_CharData.SetLipstickColor(value); }
	private void OnSetLipstickColorHighlights(int value) { m_CharData.SetLipstickColorHighlights(value); }
	private void OnSetBlushColor(int value) { m_CharData.SetBlushColor(value); }
	private void OnSetBlushColorHighlights(int value) { m_CharData.SetBlushColorHighlights(value); }
	private void OnSetEyeBrowsColor(int value) { m_CharData.SetEyeBrowsColor(value); }
	private void OnSetEyeBrowsColorHighlights(int value) { m_CharData.SetEyeBrowsColorHighlights(value); }
	private void OnSetBlushOpacity(float value) { m_CharData.SetBlushOpacity(value); }
	private void OnSetComplexionOpacity(float value) { m_CharData.SetComplexionOpacity(value); }
	private void OnSetSunDamageOpacity(float value) { m_CharData.SetSunDamageOpacity(value); }
	private void OnSetLipstickOpacity(float value) { m_CharData.SetLipstickOpacity(value); }
	private void OnSetMolesFrecklesOpacity(float value) { m_CharData.SetMolesFrecklesOpacity(value); }
	private void OnSetBodyBlemishes(int value) { m_CharData.SetBodyBlemishes(value); }
	private void OnSetBodyBlemishesOpacity(float value) { m_CharData.SetBodyBlemishesOpacity(value); }
	private void OnSetBlemishesOpacity(float value) { m_CharData.SetBlemishesOpacity(value); }
	private void OnSetEyeBrowsOpacity(float value) { m_CharData.SetEyeBrowsOpacity(value); }
	private void OnSetAgeingOpacity(float value) { m_CharData.SetAgeingOpacity(value); }
	private void OnSetMakeupOpacity(float value) { m_CharData.SetMakeupOpacity(value); }
	private void OnSetEyeColor(int value) { m_CharData.SetEyeColor(value); }
	private void OnSetNoseSizeHorizontal(float value) { m_CharData.SetNoseSizeHorizontal(value); }
	private void OnSetNoseSizeVertical(float value) { m_CharData.SetNoseSizeVertical(value); }
	private void OnSetNoseSizeOutwards(float value) { m_CharData.SetNoseSizeOutwards(value); }
	private void OnSetNoseSizeOutwardsUpper(float value) { m_CharData.SetNoseSizeOutwardsUpper(value); }
	private void OnSetNoseSizeOutwardsLower(float value) { m_CharData.SetNoseSizeOutwardsLower(value); }
	private void OnSetNoseAngle(float value) { m_CharData.SetNoseAngle(value); }
	private void OnSetEyebrowHeight(float value) { m_CharData.SetEyebrowHeight(value); }
	private void OnSetEyebrowDepth(float value) { m_CharData.SetEyebrowDepth(value); }
	private void OnSetCheekboneHeight(float value) { m_CharData.SetCheekboneHeight(value); }
	private void OnSetCheekWidth(float value) { m_CharData.SetCheekWidth(value); }
	private void OnSetCheekWidthLower(float value) { m_CharData.SetCheekWidthLower(value); }
	private void OnSetEyeSize(float value) { m_CharData.SetEyeSize(value); }
	private void OnSetLipSize(float value) { m_CharData.SetLipSize(value); }
	private void OnSetMouthSize(float value) { m_CharData.SetMouthSize(value); }
	private void OnSetMouthSizeLower(float value) { m_CharData.SetMouthSizeLower(value); }
	private void OnSetChinSize(float value) { m_CharData.SetChinSize(value); }
	private void OnSetChinSizeUnderneath(float value) { m_CharData.SetChinSizeUnderneath(value); }
	private void OnSetChinWidth(float value) { m_CharData.SetChinWidth(value); }
	private void OnSetChinEffect(float value) { m_CharData.SetChinEffect(value); }
	private void OnSetNeckWidth(float value) { m_CharData.SetNeckWidth(value); }
	private void OnSetNeckWidthLower(float value) { m_CharData.SetNeckWidthLower(value); }
	private void OnSetFaceShape(int index, int gender, int value) { m_CharData.SetFaceShape(index, gender, value); }
	private void OnSetSkinPercentage_Shape(float value) { m_CharData.SetSkinPercentage_Shape(value, false); }
	private void OnSetSkinPercentage_Color(float value) { m_CharData.SetSkinPercentage_Color(value, false); }

	private void OnSetBlushOpacity_Finalize(float value)
	{
		OnSetBlushOpacity(value);
		RequestPricing();
	}

	private void OnSetComplexionOpacity_Finalize(float value)
	{
		OnSetComplexionOpacity(value);
		RequestPricing();
	}

	private void OnSetSunDamageOpacity_Finalize(float value)
	{
		OnSetSunDamageOpacity(value);
		RequestPricing();
	}

	private void OnSetLipstickOpacity_Finalize(float value)
	{
		OnSetLipstickOpacity(value);
		RequestPricing();
	}

	private void OnSetMolesFrecklesOpacity_Finalize(float value)
	{
		OnSetMolesFrecklesOpacity(value);
		RequestPricing();
	}

	private void OnSetBodyBlemishesOpacity_Finalize(float value)
	{
		OnSetBodyBlemishesOpacity(value);
		RequestPricing();
	}

	private void OnSetBlemishesOpacity_Finalize(float value)
	{
		OnSetBlemishesOpacity(value);
		RequestPricing();
	}

	private void OnSetEyeBrowsOpacity_Finalize(float value)
	{
		OnSetEyeBrowsOpacity(value);
		RequestPricing();
	}

	private void OnSetAgeingOpacity_Finalize(float value)
	{
		OnSetAgeingOpacity(value);
		RequestPricing();
	}

	private void OnSetMakeupOpacity_Finalize(float value)
	{
		OnSetMakeupOpacity(value);
		RequestPricing();
	}

	private void OnSetNoseSizeHorizontal_Finalize(float value)
	{
		OnSetNoseSizeHorizontal(value);
		RequestPricing();
	}


	private void OnSetNoseSizeVertical_Finalize(float value)
	{
		OnSetNoseSizeVertical(value);
		RequestPricing();
	}

	private void OnSetNoseSizeOutwards_Finalize(float value)
	{
		OnSetNoseSizeOutwards(value);
		RequestPricing();
	}

	private void OnSetNoseSizeOutwardsUpper_Finalize(float value)
	{
		OnSetNoseSizeOutwardsUpper(value);
		RequestPricing();
	}


	private void OnSetNoseSizeOutwardsLower_Finalize(float value)
	{
		OnSetNoseSizeOutwardsLower(value);
		RequestPricing();
	}

	private void OnSetNoseAngle_Finalize(float value)
	{
		OnSetNoseAngle(value);
		RequestPricing();
	}

	private void OnSetEyebrowHeight_Finalize(float value)
	{
		OnSetEyebrowHeight(value);
		RequestPricing();
	}

	private void OnSetEyebrowDepth_Finalize(float value)
	{
		OnSetEyebrowDepth(value);
		RequestPricing();
	}


	private void OnSetCheekboneHeight_Finalize(float value)
	{
		OnSetCheekboneHeight(value);
		RequestPricing();
	}

	private void OnSetCheekWidth_Finalize(float value)
	{
		OnSetCheekWidth(value);
		RequestPricing();
	}

	private void OnSetCheekWidthLower_Finalize(float value)
	{
		OnSetCheekWidthLower(value);
		RequestPricing();
	}


	private void OnSetEyeSize_Finalize(float value)
	{
		OnSetEyeSize(value);
		RequestPricing();
	}

	private void OnSetLipSize_Finalize(float value)
	{
		OnSetLipSize(value);
		RequestPricing();
	}

	private void OnSetMouthSize_Finalize(float value)
	{
		OnSetMouthSize(value);
		RequestPricing();
	}

	private void OnSetMouthSizeLower_Finalize(float value)
	{
		OnSetMouthSizeLower(value);
		RequestPricing();
	}

	private void OnSetChinSize_Finalize(float value)
	{
		OnSetChinSize(value);
		RequestPricing();
	}

	private void OnSetChinSizeUnderneath_Finalize(float value)
	{
		OnSetChinSizeUnderneath(value);
		RequestPricing();
	}

	private void OnSetChinWidth_Finalize(float value)
	{
		OnSetChinWidth(value);
		RequestPricing();
	}

	private void OnSetChinEffect_Finalize(float value)
	{
		OnSetChinEffect(value);
		RequestPricing();
	}

	private void OnSetNeckWidth_Finalize(float value)
	{
		OnSetNeckWidth(value);
		RequestPricing();
	}

	private void OnSetNeckWidthLower_Finalize(float value)
	{
		OnSetNeckWidthLower(value);
		RequestPricing();
	}

	private void OnSetSkinPercentage_Shape_Finalize(float value)
	{
		OnSetSkinPercentage_Shape(value);
		RequestPricing();
	}

	private void OnSetSkinPercentage_Color_Finalize(float value)
	{
		OnSetSkinPercentage_Color(value);
		RequestPricing();
	}

	// END UI CALLBACKS

	private void OnOfferCharacterUpgrade_Confirm()
	{
		ScreenFadeHelper.BeginFade(500, 3500, null, () =>
		{
			NetworkEventSender.SendNetworkEvent_DoCharacterTypeUpgrade();
		}, null, null);
	}

	private void OnOfferCharacterUpgrade_Decline()
	{
		// Nothing to do
	}

	private void GotPriceInfo(float fPrice, bool bHasToken)
	{
		string strPriceString = Helpers.FormatString("Cost: {0} {1}", bHasToken ? "Free" : Helpers.FormatString("${0:0.00} (Fixed Price)", fPrice), bHasToken ? "(Legacy Character Plastic Surgeon Token)" : "");
		m_UI.SetPriceString(strPriceString);
	}

	private bool OnRequestShow()
	{
		// Got to trigger a server event to handle skin and dimension
		NetworkEventSender.SendNetworkEvent_EnterPlasticSurgeon();

		// false = don't show, wait for event
		return false;
	}

	private uint ConvertGTAFloatToUIntRawPercentage(float fInput)
	{
		return (uint)(fInput * 100.0f);
	}

	private uint ConvertGTAFloatNegativeOneToOneToUIntPercentage(float fInput)
	{
		// support -1 > 0 < 1 range
		return (uint)((fInput + 1.0f) * 50.0);
	}

	// TODO_PLASTIC_SURGEON: Save an event and get this from data?
	private void OnEnterPlasticSurgeonResponse()
	{
		// must recreate m_chardata so we init from char data
		m_CharData = new CCharacterCreationData(ECharacterCreationDataType.PlasticSurgeon);
		m_CharData.CharacterType = ECharacterType.Custom;

		// TODO_CSHARP: Move these all after the other execute calls, and cache execute calls
		base.ForceShow();

		// set initial values
		// TODO

		// Tab content
		Dictionary<EOverlayTypes, uint> maxHeadOverlays = new Dictionary<EOverlayTypes, uint>();
		foreach (EOverlayTypes overlayType in Enum.GetValues(typeof(EOverlayTypes)))
		{
			uint num = (uint)RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetNumHeadOverlayValues, (int)overlayType) + 1;
			maxHeadOverlays[overlayType] = num; // + 1 above is because 0 is actually -1 (none)
		}

		// get the players current values
		// NOTE: we add one onto some that -1 is disabled, since slider is 0->max+1
		int AGEING = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_AGEING) + 1;
		float AGEINGOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_AGEINGOPACITY);
		int MAKEUP = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MAKEUP) + 1;
		float MAKEUPOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MAKEUPOPACITY);
		int MAKEUPCOLOR = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MAKEUPCOLOR);
		int MAKEUPCOLORHIGHLIGHT = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MAKEUPCOLORHIGHLIGHT);
		int BLUSH = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLUSH) + 1;
		float BLUSHOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_BLUSHOPACITY);
		int BLUSHCOLOR = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLUSHCOLOR);
		int BLUSHCOLORHIGHLIGHT = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLUSHCOLORHIGHLIGHT);
		int COMPLEXION = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_COMPLEXION) + 1;
		float COMPLEXIONOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_COMPLEXIONOPACITY);
		int SUNDAMAGE = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_SUNDAMAGE) + 1;
		float SUNDAMAGEOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_SUNDAMAGEOPACITY);
		int LIPSTICK = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_LIPSTICK) + 1;
		float LIPSTICKOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_LIPSTICKOPACITY);
		int LIPSTICKCOLOR = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_LIPSTICKCOLOR);
		int LIPSTICKCOLORHIGHLIGHTS = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS);
		int MOLESANDFRECKLES = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_MOLESANDFRECKLES) + 1;
		float MOLESANDFRECKLESOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MOLESANDFRECKLESOPACITY);
		float NOSESIZEHORIZONTAL = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEHORIZONTAL);
		float NOSESIZEVERTICAL = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEVERTICAL);
		float NOSESIZEOUTWARDS = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEOUTWARDS);
		float NOSESIZEOUTWARDSUPPER = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEOUTWARDSUPPER);
		float NOSESIZEOUTWARDSLOWER = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSESIZEOUTWARDSLOWER);
		float NOSEANGLE = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NOSEANGLE);
		float EYEBROWHEIGHT = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYEBROWHEIGHT);
		float EYEBROWDEPTH = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYEBROWDEPTH);
		float CHEEKBONEHEIGHT = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHEEKBONEHEIGHT);
		float CHEEKWIDTH = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHEEKWIDTH);
		float CHEEKWIDTHLOWER = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHEEKWIDTHLOWER);
		float EYESIZE = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYESIZE);
		float LIPSIZE = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_LIPSIZE);
		float MOUTHSIZE = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MOUTHSIZE);
		float MOUTHSIZELOWER = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_MOUTHSIZELOWER);
		float CHINSIZE = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINSIZE);
		float CHINSIZELOWER = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINSIZELOWER);
		float CHINWIDTH = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINWIDTH);
		float CHINEFFECT = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_CHINEFFECT);
		float NECKWIDTH = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NECKWIDTH);
		float NECKWIDTHLOWER = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_NECKWIDTHLOWER);
		int FACEBLEND1MOTHER = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_FACEBLEND1MOTHER);
		int FACEBLEND1FATHER = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_FACEBLEND1FATHER);
		float FACEBLENDFATHERPERCENT = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_FACEBLENDFATHERPERCENT);
		float SKINBLENDFATHERPERCENT = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_SKINBLENDFATHERPERCENT);
		int EYECOLOR = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYECOLOR);
		int BLEMISHES = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BLEMISHES) + 1;
		float BLEMISHESOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_BLEMISHESOPACITY);
		int EYEBROWS = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYEBROWS) + 1;
		float EYEBROWSOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_EYEBROWSOPACITY);
		int EYEBROWSCOLOR = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYEBROWSCOLOR);
		int EYEBROWSCOLORHIGHLIGHT = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_EYEBROWSCOLORHIGHLIGHT);
		int BODYBLEMISHES = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_BODYBLEMISHES) + 1;
		float BODYBLEMISHESOPACITY = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.CC_BODYBLEMISHESOPACITY);

		//////////////////////////////////
		// APPEARANCE TAB
		//////////////////////////////////
		// heritage selectors
		// register heritages
		foreach (int value in CharacterConstants.g_CustomFaces_Male)
		{
			m_UI.AddHeritage(EGender.Male, value);
		}

		foreach (int value in CharacterConstants.g_CustomFaces_Female)
		{
			m_UI.AddHeritage(EGender.Female, value);
		}

		//////////////////////////////////////////////////
		// BODY APPEARANCE TAB
		//////////////////////////////////////////////////
		// TODO_PLASTIC_SURGEON: Must set default highlight

		// is our parent male or female?
		bool bMotherIsFemale = CharacterConstants.g_CustomFaces_Female.Contains(FACEBLEND1MOTHER);
		bool bFatherIsMale = CharacterConstants.g_CustomFaces_Male.Contains(FACEBLEND1FATHER);

		m_UI.AddTabContent_HeritageSelector("Parent 1", 0, bFatherIsMale ? EGender.Male : EGender.Female, "What did your 1st parent look like?", UIEventID.PlasticSurgeon_SetFaceShape);
		m_UI.AddTabContent_HeritageSelector("Parent 2", 1, bMotherIsFemale ? EGender.Female : EGender.Male, "What did your 2nd parent look like?", UIEventID.PlasticSurgeon_SetFaceShape);

		m_UI.AddTabContent_Slider("Face Shape %", "Do you look more like your 1st or 2nd parent? (Shape)", "Parent 1", "Parent 2", 0, 100, ConvertGTAFloatToUIntRawPercentage(FACEBLENDFATHERPERCENT),
			UIEventID.PlasticSurgeon_SetSkinPercentage_Shape, UIEventID.PlasticSurgeon_SetSkinPercentage_Shape);
		m_UI.AddTabContent_Slider("Skin Color %", "Do you look more like your 1st or 2nd parent? (Color)", "Parent 1", "Parent 2", 0, 100, ConvertGTAFloatToUIntRawPercentage(SKINBLENDFATHERPERCENT),
			UIEventID.PlasticSurgeon_SetSkinPercentage_Color, UIEventID.PlasticSurgeon_SetSkinPercentage_Color);

		m_UI.AddSeperator();

		m_UI.AddTabContent_Slider("Eyebrow Depth", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(EYEBROWDEPTH), UIEventID.PlasticSurgeon_SetEyebrowDepth, UIEventID.PlasticSurgeon_SetEyebrowDepth_Finalize);
		m_UI.AddTabContent_Slider("Eyebrow Height", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(EYEBROWHEIGHT), UIEventID.PlasticSurgeon_SetEyebrowHeight, UIEventID.PlasticSurgeon_SetEyebrowHeight_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_Slider("Nose Angle", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NOSEANGLE), UIEventID.PlasticSurgeon_SetNoseAngle, UIEventID.PlasticSurgeon_SetNoseAngle_Finalize);
		m_UI.AddTabContent_Slider("Nose Width", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NOSESIZEHORIZONTAL), UIEventID.PlasticSurgeon_SetNoseSizeHorizontal, UIEventID.PlasticSurgeon_SetNoseSizeHorizontal_Finalize);
		m_UI.AddTabContent_Slider("Nose Height", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NOSESIZEVERTICAL), UIEventID.PlasticSurgeon_SetNoseSizeVertical, UIEventID.PlasticSurgeon_SetNoseSizeVertical_Finalize);
		m_UI.AddTabContent_Slider("Nose Size (Outwards)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NOSESIZEOUTWARDS), UIEventID.PlasticSurgeon_SetNoseSizeOutwards, UIEventID.PlasticSurgeon_SetNoseSizeOutwards_Finalize);
		m_UI.AddTabContent_Slider("Nose Size (Outwards - Lower)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NOSESIZEOUTWARDSLOWER), UIEventID.PlasticSurgeon_SetNoseSizeOutwardsLower, UIEventID.PlasticSurgeon_SetNoseSizeOutwardsLower_Finalize);
		m_UI.AddTabContent_Slider("Nose Size (Outwards - Upper)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NOSESIZEOUTWARDSUPPER), UIEventID.PlasticSurgeon_SetNoseSizeOutwardsUpper, UIEventID.PlasticSurgeon_SetNoseSizeOutwardsUpper_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_Slider("Cheekbone Height", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(CHEEKBONEHEIGHT), UIEventID.PlasticSurgeon_SetCheekboneHeight, UIEventID.PlasticSurgeon_SetCheekboneHeight_Finalize);
		m_UI.AddTabContent_Slider("Cheek Width (Upper)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(CHEEKWIDTH), UIEventID.PlasticSurgeon_SetCheekWidth, UIEventID.PlasticSurgeon_SetCheekWidth_Finalize);
		m_UI.AddTabContent_Slider("Cheek Width (Lower)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(CHEEKWIDTHLOWER), UIEventID.PlasticSurgeon_SetCheekWidthLower, UIEventID.PlasticSurgeon_SetCheekWidthLower_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_Slider("Mouth Size (Upper)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(MOUTHSIZE), UIEventID.PlasticSurgeon_SetMouthSize, UIEventID.PlasticSurgeon_SetMouthSize_Finalize);
		m_UI.AddTabContent_Slider("Mouth Size (Lower)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(MOUTHSIZELOWER), UIEventID.PlasticSurgeon_SetMouthSizeLower, UIEventID.PlasticSurgeon_SetMouthSizeLower_Finalize);
		m_UI.AddTabContent_Slider("Lip Size", "", "Thinner", "Thicker", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(LIPSIZE), UIEventID.PlasticSurgeon_SetLipSize, UIEventID.PlasticSurgeon_SetLipSize_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_Slider("Chin Effect", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(CHINEFFECT), UIEventID.PlasticSurgeon_SetChinEffect, UIEventID.PlasticSurgeon_SetChinEffect_Finalize);
		m_UI.AddTabContent_Slider("Chin Width", "", "Healthier", "Skinnier", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(CHINWIDTH), UIEventID.PlasticSurgeon_SetChinWidth, UIEventID.PlasticSurgeon_SetChinWidth_Finalize);
		m_UI.AddTabContent_Slider("Chin Size", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(CHINSIZE), UIEventID.PlasticSurgeon_SetChinSize, UIEventID.PlasticSurgeon_SetChinSize_Finalize);
		m_UI.AddTabContent_Slider("Chin Size (Underneath)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(CHINSIZELOWER), UIEventID.PlasticSurgeon_SetChinSizeUnderneath, UIEventID.PlasticSurgeon_SetChinSizeUnderneath_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_Slider("Neck Width (Upper)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NECKWIDTH), UIEventID.PlasticSurgeon_SetNeckWidth, UIEventID.PlasticSurgeon_SetNeckWidth_Finalize);
		m_UI.AddTabContent_Slider("Neck Width (Lower)", "", "Smaller", "Larger", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(NECKWIDTHLOWER), UIEventID.PlasticSurgeon_SetNeckWidthLower, UIEventID.PlasticSurgeon_SetNeckWidthLower_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_NumberSelector("Sun Damage", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.SunDamage]), 0, (uint)maxHeadOverlays[EOverlayTypes.SunDamage], (uint)SUNDAMAGE, UIEventID.PlasticSurgeon_SetSunDamage);
		m_UI.AddTabContent_Slider("Sun Damage (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(SUNDAMAGEOPACITY), UIEventID.PlasticSurgeon_SetSunDamageOpacity, UIEventID.PlasticSurgeon_SetSunDamageOpacity_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_NumberSelector("Skin Ageing Effect", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Ageing]), 0, (uint)maxHeadOverlays[EOverlayTypes.Ageing], (uint)AGEING, UIEventID.PlasticSurgeon_SetAgeing);
		m_UI.AddTabContent_Slider("Skin Ageing Effect (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(AGEINGOPACITY), UIEventID.PlasticSurgeon_SetAgeingOpacity, UIEventID.PlasticSurgeon_SetAgeingOpacity_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_NumberSelector("Complexion", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Complexion]), 0, (uint)maxHeadOverlays[EOverlayTypes.Complexion], (uint)COMPLEXION, UIEventID.PlasticSurgeon_SetComplexion);
		m_UI.AddTabContent_Slider("Complexion (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(COMPLEXIONOPACITY), UIEventID.PlasticSurgeon_SetComplexionOpacity, UIEventID.PlasticSurgeon_SetComplexionOpacity_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_NumberSelector("Blemishes", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Blemishes]), 0, (uint)maxHeadOverlays[EOverlayTypes.Blemishes], (uint)BLEMISHES, UIEventID.PlasticSurgeon_SetBlemishes);
		m_UI.AddTabContent_Slider("Blemishes (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(BLEMISHESOPACITY), UIEventID.PlasticSurgeon_SetBlemishesOpacity, UIEventID.PlasticSurgeon_SetBlemishesOpacity_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_NumberSelector("Moles & Freckles", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.MolesFreckles]), 0, (uint)maxHeadOverlays[EOverlayTypes.MolesFreckles], (uint)MOLESANDFRECKLES, UIEventID.PlasticSurgeon_SetMolesFreckles);
		m_UI.AddTabContent_Slider("Moles & Freckles (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(MOLESANDFRECKLESOPACITY), UIEventID.PlasticSurgeon_SetMolesFrecklesOpacity, UIEventID.PlasticSurgeon_SetMolesFrecklesOpacity_Finalize);

		m_UI.AddSeperator();

		m_UI.AddTabContent_NumberSelector("Body Blemishes", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.BodyBlemishes]), 0, (uint)maxHeadOverlays[EOverlayTypes.BodyBlemishes], (uint)BODYBLEMISHES, UIEventID.PlasticSurgeon_SetBodyBlemishes);
		m_UI.AddTabContent_Slider("Body Blemishes (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(BODYBLEMISHESOPACITY), UIEventID.PlasticSurgeon_SetBodyBlemishesOpacity, UIEventID.PlasticSurgeon_SetBodyBlemishesOpacity_Finalize);

		//////////////////////////////////////////////////
		// makeup etc
		//////////////////////////////////////////////////

		// BLUSH
		m_UI.AddTabContent_NumberSelector("Blush", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Blush]), 0, (uint)maxHeadOverlays[EOverlayTypes.Blush], (uint)BLUSH, UIEventID.PlasticSurgeon_SetBlush);
		m_UI.AddTabContent_Slider("Blush (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(BLUSHOPACITY), UIEventID.PlasticSurgeon_SetBlushOpacity, UIEventID.PlasticSurgeon_SetBlushOpacity_Finalize);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Blush.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Blush Color (Main)", "What is the primary color of your blush?", (uint)BLUSHCOLOR, UIEventID.PlasticSurgeon_SetBlushColor);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Blush.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Blush Color (Highlights)", "What is the highlight color of your blush?", (uint)BLUSHCOLORHIGHLIGHT, UIEventID.PlasticSurgeon_SetBlushColorHighlights);

		m_UI.AddSeperator();

		// EYE BROWS
		m_UI.AddTabContent_NumberSelector("Eyebrows", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Eyebrows]), 0, (uint)maxHeadOverlays[EOverlayTypes.Eyebrows], (uint)EYEBROWS, UIEventID.PlasticSurgeon_SetEyeBrows);
		m_UI.AddTabContent_Slider("Eyebrows (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(EYEBROWSOPACITY), UIEventID.PlasticSurgeon_SetEyeBrowsOpacity, UIEventID.PlasticSurgeon_SetEyeBrowsOpacity_Finalize);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Eyebrows.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Eyebrows Color (Main)", "What is the primary color of your eyebrows?", (uint)EYEBROWSCOLOR, UIEventID.PlasticSurgeon_SetEyeBrowsColor);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Eyebrows.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Eyebrows Color (Highlights)", "What is the highlight color of your eyebrows?", (uint)EYEBROWSCOLORHIGHLIGHT, UIEventID.PlasticSurgeon_SetEyeBrowsColorHighlights);

		m_UI.AddSeperator();

		// EYE SIZE & COLOR
		m_UI.AddTabContent_Slider("Eye Size", "", "Thicker", "Thinner", 0, 100, ConvertGTAFloatNegativeOneToOneToUIntPercentage(EYESIZE), UIEventID.PlasticSurgeon_SetEyeSize, UIEventID.PlasticSurgeon_SetEyeSize_Finalize);

		uint eyeMax = 32;
		m_UI.AddTabContent_NumberSelector("Eye Color", Helpers.FormatString("{0} to {1}", 0, eyeMax), 0, eyeMax, (uint)EYECOLOR, UIEventID.PlasticSurgeon_SetEyeColor);

		m_UI.AddSeperator();

		// LIPSTICK
		m_UI.AddTabContent_NumberSelector("Lipstick", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Lipstick]), 0, (uint)maxHeadOverlays[EOverlayTypes.Lipstick], (uint)LIPSTICK, UIEventID.PlasticSurgeon_SetLipstick);
		m_UI.AddTabContent_Slider("Lipstick (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(LIPSTICKOPACITY), UIEventID.PlasticSurgeon_SetLipstickOpacity, UIEventID.PlasticSurgeon_SetLipstickOpacity_Finalize);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Lipstick.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Lipstick Color (Main)", "What is the primary color of your lipstick?", (uint)LIPSTICKCOLOR, UIEventID.PlasticSurgeon_SetLipstickColor);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Lipstick.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Lipstick Color (Highlights)", "What is the highlight color of your lipstick?", (uint)LIPSTICKCOLORHIGHLIGHTS, UIEventID.PlasticSurgeon_SetLipstickColorHighlights);

		m_UI.AddSeperator();

		// MAKEUP
		m_UI.AddTabContent_NumberSelector("Makeup", Helpers.FormatString("{0} to {1}", 0, (uint)maxHeadOverlays[EOverlayTypes.Makeup]), 0, (uint)maxHeadOverlays[EOverlayTypes.Makeup], (uint)MAKEUP, UIEventID.PlasticSurgeon_SetMakeup);
		m_UI.AddTabContent_Slider("Makeup (Opacity)", "", "Invisible", "Visible", 0, 100, ConvertGTAFloatToUIntRawPercentage(MAKEUPOPACITY), UIEventID.PlasticSurgeon_SetMakeupOpacity, UIEventID.PlasticSurgeon_SetMakeupOpacity_Finalize);

		uint maxMakeupStyles = 54;
		m_UI.AddTabContent_NumberSelector("Makeup Styling (Primary)", Helpers.FormatString("{0} to {1}", 0, maxMakeupStyles), 0, maxMakeupStyles, (uint)MAKEUPCOLOR, UIEventID.PlasticSurgeon_SetMakeupColor);
		m_UI.AddTabContent_NumberSelector("Makeup Styling (Secondary)", Helpers.FormatString("{0} to {1}", 0, maxMakeupStyles), 0, maxMakeupStyles, (uint)MAKEUPCOLORHIGHLIGHT, UIEventID.PlasticSurgeon_SetMakeupColorHighlights);


		// get initial pricing info, mainly so we can see if we have a token or not
		RequestPricing();
	}

	private void OnEnterPlasticSurgeonOfferCharacterUpgrade()
	{
		GenericPromptHelper.ShowPrompt("Change Character Type", "Your character is a premade skin character - you must be a Custom (GTA Online) character to use this feature.<br><br>Would you like to change your character to a custom one?<br><br>You will need to visit a Plastic Surgeon to customize your appearance, and a clothing store. You will be given a free token for each if you do not have one.",
			"Yes, change my character!", "No, keep my current character", UIEventID.PlasticSurgeonOfferCharacterUpgrade_Confirm, UIEventID.PlasticSurgeonOfferCharacterUpgrade_Decline);
	}

	private void RequestPricing()
	{
		NetworkEventSender.SendNetworkEvent_PlasticSurgeon_CalculatePrice();
	}

	private void OnRender()
	{

	}

	private void OnExit()
	{

	}

	private void OnFinish()
	{
		m_CharData.TransmitPlasticSurgeonEvent(StoreSystem.CurrentStoreID);
	}
}