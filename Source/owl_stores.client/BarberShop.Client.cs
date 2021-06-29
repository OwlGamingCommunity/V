using System;
using System.Collections.Generic;
using System.Linq;

public class BarberShop : GenericCharacterCustomization
{
	private int baseHair = -2; // -2 since -1 is actually 'bald/none'
	private int hairStyle = -1;
	private int hairColor = -1;
	private int hairColorHighlights = -1;

	private int chestHairStyle = -2;
	private int chestHairColor = -1;
	private int chestHairColorHighlights = -1;
	private float chestHairOpacity = 0.0f;

	private int facialHairStyle = -2;
	private int facialHairColor = -1;
	private int facialHairColorHighlights = -1;
	private float facialHairOpacity = 0.0f;

	private int currentDrawableFullBeard = 0;
	private int currentTextureFullBeard = 0;

	public BarberShop() : base(EGUIID.Barber)
	{
		SetNameAndCallbacks("Barber Shop", null, OnFinish, OnRequestShow, null, OnExit, OnRender);

		NetworkEvents.BarberShop_GotPrice += GotPriceInfo;
		NetworkEvents.EnterBarberShop_Response += OnEnterBarberResponse;

		UIEvents.BarberShop_SetBaseHair += SetBaseHair;

		UIEvents.BarberShop_SetHairStyleDrawable += SetHairStyleDrawable;
		UIEvents.BarberShop_SetHairColor += SetHairColor;
		UIEvents.BarberShop_SetHairColorHighlights += SetHairColorHighlights;

		UIEvents.BarberShop_SetChestHair += SetChestHairStyleDrawable;
		UIEvents.BarberShop_SetChestHairColor += SetChestHairColor;
		UIEvents.BarberShop_SetChestHairColorHighlights += SetChestHairColorHighlights;
		UIEvents.BarberShop_UpdateChestHairOpacity += PreviewUpdateChestHairOpacity;
		UIEvents.BarberShop_SetChestHairOpacity += SetChestHairOpacity;

		UIEvents.BarberShop_SetFacialHair += SetFacialHairStyleDrawable;
		UIEvents.BarberShop_SetFacialHairColor += SetFacialHairColor;
		UIEvents.BarberShop_SetFacialHairColorHighlights += SetFacialHairColorHighlights;
		UIEvents.BarberShop_UpdateFacialHairOpacity += PreviewUpdateFacialHairOpacity;
		UIEvents.BarberShop_SetFacialHairOpacity += SetFacialHairOpacity;

		UIEvents.BarberShop_SetComponentDrawable_FullBeards += OnSetComponentDrawable_FullBeards;
		UIEvents.BarberShop_SetComponentTexture_FullBeards += OnSetComponentTexture_FullBeards;
		UIEvents.BarberShop_OnRootChanged_FullBeards += OnRootChanged_FullBeards;
	}

	private void OnSetComponentDrawable_FullBeards(int value)
	{
		currentDrawableFullBeard = MaskHelpers.MasksFunctioningAsBeards[value];
		RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.Masks, currentDrawableFullBeard, currentTextureFullBeard, 0);
	}

	private void OnSetComponentTexture_FullBeards(int value)
	{
		currentTextureFullBeard = value;
		RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.Masks, currentDrawableFullBeard, currentTextureFullBeard, 0);
	}

	private void OnRootChanged_FullBeards(string strElementToReset)
	{
		int numTextures = SkinHelpers.GetTexturesForBeard(currentDrawableFullBeard);
		m_UI.SetMaxForElement(strElementToReset, numTextures);
	}

	private void GotPriceInfo(float fPrice, bool bHasToken)
	{
		string strPriceString = Helpers.FormatString("Cost: {0} {1}", bHasToken ? "Free" : Helpers.FormatString("${0:0.00}", fPrice), bHasToken ? "(Legacy Character Barber Token)" : "");
		m_UI.SetPriceString(strPriceString);
	}

	public void SetBaseHair(int value)
	{
		// subtract one since 0 = off
		value -= 1;
		baseHair = value;

		SkinHelpers.ApplyTattoosAndHairTattoosForPlayer(RAGE.Elements.Player.LocalPlayer, false, true, baseHair);

		RequestPricing();
	}

	public void SetHairStyleDrawable(int value)
	{
		// If greater than or equal to 23, we add one, so we skip the night vision (24 for female)
		// TODO_LATER: We might need a more flexible system for this, incase R* adds more things we want to remove
		EGender Gender = DataHelper.GetEntityData<EGender>(RAGE.Elements.Player.LocalPlayer, EDataNames.GENDER);
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

		hairStyle = value;
		RAGE.Elements.Player.LocalPlayer.SetComponentVariation(2, value, 0, 0);
		RequestPricing();
	}

	private void SetHairColor(int value)
	{
		RAGE.Elements.Player.LocalPlayer.SetHairColor(value, hairColorHighlights);
		hairColor = value;
		RequestPricing();
	}

	private void SetHairColorHighlights(int value)
	{
		RAGE.Elements.Player.LocalPlayer.SetHairColor(hairColor, value);
		hairColorHighlights = value;
		RequestPricing();
	}

	// chest hair
	public void SetChestHairStyleDrawable(int value)
	{
		// minus 1 because 0 is off
		value -= 1;

		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay((int)EOverlayTypes.ChestHair, value, chestHairOpacity);
		chestHairStyle = value;
		RequestPricing();
	}

	private void SetChestHairColor(int value)
	{
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor((int)EOverlayTypes.ChestHair, 1, value, chestHairColorHighlights);
		chestHairColor = value;
		RequestPricing();
	}

	private void SetChestHairColorHighlights(int value)
	{
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor((int)EOverlayTypes.ChestHair, 1, chestHairColor, value);
		chestHairColorHighlights = value;
		RequestPricing();
	}

	// Update is per-frame on the drag, so we can show the changed effect locally, but dont transmit/save since its spammy if the person drags 0 -> 100 for example
	private void PreviewUpdateChestHairOpacity(float fOpacity)
	{
		chestHairOpacity = (fOpacity / 100.0f);

		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay((int)EOverlayTypes.ChestHair, chestHairStyle, chestHairOpacity);
	}

	private void SetChestHairOpacity(float fOpacity)
	{
		PreviewUpdateChestHairOpacity(fOpacity);
		RequestPricing();
	}

	// Facial hair
	public void SetFacialHairStyleDrawable(int value)
	{
		// minus 1 because 0 is off
		value -= 1;

		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay((int)EOverlayTypes.FacialHair, value, facialHairOpacity);
		facialHairStyle = value;
		RequestPricing();
	}

	private void SetFacialHairColor(int value)
	{
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor((int)EOverlayTypes.FacialHair, 1, value, facialHairColorHighlights);
		facialHairColor = value;
		RequestPricing();
	}

	private void SetFacialHairColorHighlights(int value)
	{
		RAGE.Elements.Player.LocalPlayer.SetHeadOverlayColor((int)EOverlayTypes.FacialHair, 1, facialHairColor, value);
		facialHairColorHighlights = value;
		RequestPricing();
	}

	// Update is per-frame on the drag, so we can show the changed effect locally, but dont transmit/save since its spammy if the person drags 0 -> 100 for example
	private void PreviewUpdateFacialHairOpacity(float fOpacity)
	{
		facialHairOpacity = (fOpacity / 100.0f);

		RAGE.Elements.Player.LocalPlayer.SetHeadOverlay((int)EOverlayTypes.FacialHair, facialHairStyle, facialHairOpacity);
	}

	private void SetFacialHairOpacity(float fOpacity)
	{
		PreviewUpdateFacialHairOpacity(fOpacity);
		RequestPricing();
	}

	private bool OnRequestShow()
	{
		// Got to trigger a server event to handle skin and dimension
		NetworkEventSender.SendNetworkEvent_EnterBarberShop();

		// false = don't show, wait for event
		return false;
	}

	// TODO_BARBER: Save an event and get this from data?
	private void OnEnterBarberResponse()
	{
		// TODO_CSHARP: Move these all after the other execute calls, and cache execute calls
		base.ForceShow();

		baseHair = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_BASEHAIR);

		// set initial values
		hairStyle = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_HAIRSTYLE);
		hairColor = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_HAIRCOLOR);
		hairColorHighlights = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_HAIRCOLORHIGHLIGHTS);

		EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

		// BASE HAIR
		int MaxBaseHairStyles = TattooDefinitions.g_HairTattooDefinitions.Count;
		m_UI.AddTabContent_NumberSelector("Base Hair", Helpers.FormatString("{0} to {1}", 0, (uint)MaxBaseHairStyles), 0, (uint)MaxBaseHairStyles, (uint)baseHair + 1, UIEventID.BarberShop_SetBaseHair);

		// HAIR
		uint MaxHairStyles = gender == EGender.Male ? CharacterConstants.MaxHairStyles_Male : CharacterConstants.MaxHairStyles_Female;
		m_UI.AddTabContent_NumberSelector("Hair", Helpers.FormatString("{0} to {1}", 0, (uint)MaxHairStyles), 0, (uint)MaxHairStyles, (uint)hairStyle, UIEventID.BarberShop_SetHairStyleDrawable);
		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Hair.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Hair Color", "What is the primary color of your hair?", (uint)hairColor, UIEventID.BarberShop_SetHairColor);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_Hair.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Hair Color (Highlights)", "What is the highlight color of your hair?", (uint)hairColorHighlights, UIEventID.BarberShop_SetHairColorHighlights);

		m_UI.AddSeperator();

		// Get maxes
		// NOTE: We have to get all for some reason, ive no idea why
		Dictionary<EOverlayTypes, uint> maxHeadOverlays = new Dictionary<EOverlayTypes, uint>();
		foreach (EOverlayTypes overlayType in Enum.GetValues(typeof(EOverlayTypes)))
		{
			uint num = (uint)RAGE.Game.Invoker.Invoke<int>(RAGE.Game.Natives.GetNumHeadOverlayValues, (int)overlayType) + 1;
			maxHeadOverlays[overlayType] = num; // + 1 above is because 0 is actually -1 (none)
		}

		// FACIAL HAIR
		facialHairStyle = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_FACIALHAIRSTYLE);
		facialHairColor = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_FACIALHAIRCOLOR);
		facialHairColorHighlights = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT);
		facialHairOpacity = DataHelper.GetEntityData<float>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_FACIALHAIROPACITY);

		uint numFacialHairs = maxHeadOverlays[EOverlayTypes.FacialHair];

		m_UI.AddTabContent_NumberSelector("Facial Hair", Helpers.FormatString("{0} to {1}", 0, numFacialHairs), 0, numFacialHairs, (uint)facialHairStyle, UIEventID.BarberShop_SetFacialHair);
		m_UI.AddTabContent_Slider("Facial Hair (Opacity)", "", "Invisible", "Visible", 0, 100, (uint)(facialHairOpacity * 100.0f), UIEventID.BarberShop_UpdateFacialHairOpacity, UIEventID.BarberShop_SetFacialHairOpacity);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_FacialHair.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Facial Hair Color (Main)", "What is the primary color of your facial hair?", (uint)facialHairColor, UIEventID.BarberShop_SetFacialHairColor);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_FacialHair.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Facial Hair Color (Highlights)", "What is the highlight color of your facial hair?", (uint)facialHairColorHighlights, UIEventID.BarberShop_SetFacialHairColorHighlights);

		m_UI.AddSeperator();

		// FULL BEARDS
		//int BeardsMax = MaskHelpers.MasksFunctioningAsBeards.Length - 1;
		int currentMask = CurrentDrawables[(int)ECustomClothingComponent.Masks];
		//int maxTextureForBeards = SkinHelpers.GetTexturesForBeard(currentMask);

		// is the current mask a beard?
		if (!MaskHelpers.MasksFunctioningAsBeards.Contains(currentMask))
		{
			currentMask = 0;
			//maxTextureForBeards = SkinHelpers.GetTexturesForBeard(currentMask);
		}
		else
		{
			currentMask = Array.IndexOf(MaskHelpers.MasksFunctioningAsBeards, currentMask);
		}

		//currentDrawableFullBeard = MaskHelpers.MasksFunctioningAsBeards[currentMask];
		//currentTextureFullBeard = CurrentTextures[(int)ECustomClothingComponent.Masks];


		//m_UI.AddTabContent_ClothingSelector("Full Beards", "Style", 0, BeardsMax, currentMask , UIEventID.BarberShop_SetComponentDrawable_FullBeards,
		//	"Color", 0, maxTextureForBeards, CurrentTextures[(int)ECustomClothingComponent.Masks], UIEventID.BarberShop_SetComponentTexture_FullBeards, UIEventID.BarberShop_OnRootChanged_FullBeards);
		//m_UI.AddSeperator();

		// CHEST HAIR
		chestHairStyle = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_CHESTHAIR);
		chestHairColor = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_CHESTHAIRCOLOR);
		chestHairColorHighlights = DataHelper.GetEntityData<int>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_CHESTHAIRHIGHLIGHT);
		chestHairOpacity = DataHelper.GetEntityData<float>(RAGE.Elements.Player.LocalPlayer, EDataNames.CC_CHESTHAIROPACITY);

		uint numChestHairs = maxHeadOverlays[EOverlayTypes.ChestHair];

		m_UI.AddTabContent_NumberSelector("Chest Hair", Helpers.FormatString("{0} to {1}", 0, numChestHairs), 0, numChestHairs, (uint)chestHairStyle, UIEventID.BarberShop_SetChestHair);
		m_UI.AddTabContent_Slider("Chest Hair (Opacity)", "", "Invisible", "Visible", 0, 100, (uint)(chestHairOpacity * 100.0f), UIEventID.BarberShop_UpdateChestHairOpacity, UIEventID.BarberShop_SetChestHairOpacity);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_ChestHair.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Chest Hair Color (Main)", "What is the primary color of your chest hair?", (uint)chestHairColor, UIEventID.BarberShop_SetChestHairColor);

		foreach (var (strHexCode, index) in CharacterHexCodes.HexCodes_ChestHair.Select((value, i) => (value, (uint)i)))
		{
			m_UI.AddPendingColor(index, strHexCode);
		}
		m_UI.AddTabContent_ColorPicker("Chest Hair Color (Highlights)", "What is the highlight color of your chest hair?", (uint)chestHairColorHighlights, UIEventID.BarberShop_SetChestHairColorHighlights);

		// get initial pricing info, mainly so we can see if we have a token or not
		RequestPricing();
	}

	private void RequestPricing()
	{
		NetworkEventSender.SendNetworkEvent_BarberShop_CalculatePrice(baseHair, hairStyle, hairColor, hairColorHighlights, chestHairStyle, chestHairColor, chestHairColorHighlights, chestHairOpacity, facialHairStyle, facialHairColor, facialHairColorHighlights, facialHairOpacity, currentDrawableFullBeard, currentTextureFullBeard);
	}

	private void OnRender()
	{

	}

	private void OnExit()
	{

	}

	private void OnFinish()
	{
		NetworkEventSender.SendNetworkEvent_BarberShop_OnCheckout(StoreSystem.CurrentStoreID, baseHair, hairStyle, hairColor, hairColorHighlights, chestHairStyle, chestHairColor, chestHairColorHighlights, chestHairOpacity, facialHairStyle, facialHairColor, facialHairColorHighlights, facialHairOpacity, currentDrawableFullBeard, currentTextureFullBeard);
	}
}