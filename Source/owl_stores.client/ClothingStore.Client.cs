using System;
using System.Collections.Generic;
using System.Linq;

public class ClothingStore : GenericCharacterCustomization
{
	private uint m_PremadeCharSkin = 0;

	public ClothingStore() : base(EGUIID.ClothingStore)
	{
		SetNameAndCallbacks("Clothing Store", null, OnFinish, OnRequestShow, null, OnExit, OnRender);

		NetworkEvents.ClothingStore_GotPrice += GotPriceInfo;
		NetworkEvents.EnterClothingStore_Response += OnEnterClothingStoreResponse;

		UIEvents.ClothingStore_SetPropDrawable_Earrings += (int value) => { SetPropDrawable(ECustomPropSlot.Ears, value); };
		UIEvents.ClothingStore_SetPropTexture_Earrings += (int value) => { SetPropTexture(ECustomPropSlot.Ears, value); };
		UIEvents.ClothingStore_SetPropDrawable_Glasses += (int value) => { SetPropDrawable(ECustomPropSlot.Glasses, value); };
		UIEvents.ClothingStore_SetPropTexture_Glasses += (int value) => { SetPropTexture(ECustomPropSlot.Glasses, value); };
		UIEvents.ClothingStore_SetPropDrawable_Hats += (int value) => { SetPropDrawable(ECustomPropSlot.Hats, value); };
		UIEvents.ClothingStore_SetPropTexture_Hats += (int value) => { SetPropTexture(ECustomPropSlot.Hats, value); };
		UIEvents.ClothingStore_SetPropDrawable_Watches += (int value) => { SetPropDrawable(ECustomPropSlot.Watches, value); };
		UIEvents.ClothingStore_SetPropTexture_Watches += (int value) => { SetPropTexture(ECustomPropSlot.Watches, value); };
		UIEvents.ClothingStore_SetPropDrawable_Bracelets += (int value) => { SetPropDrawable(ECustomPropSlot.Bracelets, value); };
		UIEvents.ClothingStore_SetPropTexture_Bracelets += (int value) => { SetPropTexture(ECustomPropSlot.Bracelets, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Accessories += (int value) => { SetComponentDrawable(ECustomClothingComponent.Accessories, value); };
		UIEvents.ClothingStore_SetComponentTexture_Accessories += (int value) => { SetComponentTexture(ECustomClothingComponent.Accessories, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Masks += (int value) => { SetComponentDrawable(ECustomClothingComponent.Masks, value); };
		UIEvents.ClothingStore_SetComponentTexture_Masks += (int value) => { SetComponentTexture(ECustomClothingComponent.Masks, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Decals += (int value) => { SetComponentDrawable(ECustomClothingComponent.Decals, value); };
		UIEvents.ClothingStore_SetComponentTexture_Decals += (int value) => { SetComponentTexture(ECustomClothingComponent.Decals, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Legs += (int value) => { SetComponentDrawable(ECustomClothingComponent.Legs, value); };
		UIEvents.ClothingStore_SetComponentTexture_Legs += (int value) => { SetComponentTexture(ECustomClothingComponent.Legs, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Shoes += (int value) => { SetComponentDrawable(ECustomClothingComponent.Shoes, value); };
		UIEvents.ClothingStore_SetComponentTexture_Shoes += (int value) => { SetComponentTexture(ECustomClothingComponent.Shoes, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Tops += (int value) => { SetComponentDrawable(ECustomClothingComponent.Tops, value); };
		UIEvents.ClothingStore_SetComponentTexture_Tops += (int value) => { SetComponentTexture(ECustomClothingComponent.Tops, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Torso += (int value) => { SetComponentDrawable(ECustomClothingComponent.Torsos, value); };
		UIEvents.ClothingStore_SetComponentTexture_Torso += (int value) => { SetComponentTexture(ECustomClothingComponent.Torsos, value); };
		UIEvents.ClothingStore_SetComponentDrawable_Undershirts += (int value) => { SetComponentDrawable(ECustomClothingComponent.Undershirts, value); };
		UIEvents.ClothingStore_SetComponentTexture_Undershirts += (int value) => { SetComponentTexture(ECustomClothingComponent.Undershirts, value); };
		UIEvents.ClothingStore_OnRootChanged_Accessories += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Accessories, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Masks += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Masks, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Decals += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Decals, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Legs += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Legs, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Shoes += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Shoes, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Tops += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Tops, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Torso += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Torsos, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Undershirts += (string strElementToReset) => { UpdateMaxTextureForComponent(ECustomClothingComponent.Undershirts, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Hats += (string strElementToReset) => { UpdateMaxTextureForProp(ECustomPropSlot.Hats, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Bracelets += (string strElementToReset) => { UpdateMaxTextureForProp(ECustomPropSlot.Bracelets, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Watches += (string strElementToReset) => { UpdateMaxTextureForProp(ECustomPropSlot.Watches, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Earrings += (string strElementToReset) => { UpdateMaxTextureForProp(ECustomPropSlot.Ears, strElementToReset); };
		UIEvents.ClothingStore_OnRootChanged_Glasses += (string strElementToReset) => { UpdateMaxTextureForProp(ECustomPropSlot.Glasses, strElementToReset); };
		UIEvents.ClothingStore_SetSkinID += OnPremadeSkinChanged;
	}

	private void OnPremadeSkinChanged(int value)
	{
		EGender Gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

		uint[] arrayToUse = Gender == EGender.Male ? CharacterConstants.g_PremadeMaleSkins : CharacterConstants.g_PremadeFemaleSkins;
		m_PremadeCharSkin = arrayToUse[value];
		RAGE.Elements.Player.LocalPlayer.Model = m_PremadeCharSkin;

		// do we need pricing?
		RequestPricing();
	}

	private void SetComponentDrawable(ECustomClothingComponent component, int index)
	{
		int value = -1;
		EGender Gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

		if (component == ECustomClothingComponent.Masks)
		{
			value = SkinConstants.GetMasks(Gender)[index];
		}
		else if (component == ECustomClothingComponent.HairStyles)
		{
			// NOTE: Unsupported at this time
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

		if (IsClothingVisible() || component == ECustomClothingComponent.HairStyles) // 2 is hair styles, we let that one go
		{
			ApplyComponent(component);
		}

		RequestPricing();
	}

	private void SetComponentTexture(ECustomClothingComponent component, int value)
	{
		CurrentTextures[(int)component] = value;

		if (IsClothingVisible() || component == ECustomClothingComponent.HairStyles) // 2 is hair styles, we let that one go
		{
			ApplyComponent(component);
		}

		RequestPricing();
	}

	private void SetPropDrawable(ECustomPropSlot slot, int index)
	{
		EGender Gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

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

		if (IsClothingVisible())
		{
			ApplyProp(slot);
		}

		RequestPricing();
	}

	private void SetPropTexture(ECustomPropSlot slot, int value)
	{
		CurrentPropTextures[slot] = value;

		if (IsClothingVisible())
		{
			ApplyProp(slot);
		}

		RequestPricing();
	}

	private void UpdateMaxTextureForComponent(ECustomClothingComponent component, string strElementToReset)
	{
		int currentDrawable = CurrentDrawables[(int)component];
		int numTextures = RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)component, currentDrawable) - 1; // -1 because 0 is a texture

		m_UI.SetMaxForElement(strElementToReset, numTextures);
	}

	private void UpdateMaxTextureForProp(ECustomPropSlot prop, string strElementToReset)
	{
		int currentDrawable = CurrentPropDrawables[prop];
		int numTextures = RAGE.Elements.Player.LocalPlayer.GetNumberOfPropTextureVariations((int)prop, currentDrawable) - 1; // -1 because 0 is a texture

		m_UI.SetMaxForElement(strElementToReset, numTextures);
	}


	private void GotPriceInfo(float fPrice, bool bHasToken)
	{
		string strPriceString = Helpers.FormatString("Cost: {0} {1}", bHasToken ? "Free" : Helpers.FormatString("${0:0.00}", fPrice), bHasToken ? "(Legacy Character Clothing Store Token)" : "");
		m_UI.SetPriceString(strPriceString);
	}

	private bool OnRequestShow()
	{
		// Got to trigger a server event to handle skin and dimension
		NetworkEventSender.SendNetworkEvent_EnterClothingStore();

		// false = don't show, wait for event
		return false;
	}

	// TODO_CLOTHING_STORE: Save an event and get this from data?
	private void OnEnterClothingStoreResponse()
	{
		// TODO_CSHARP: Move these all after the other execute calls, and cache execute calls
		base.ForceShow();
		ForceClothesState(true); // set clothes to visible, since its a clothing store

		// set initial values
		// TODO: Impl
		m_PremadeCharSkin = RAGE.Elements.Player.LocalPlayer.Model;

		for (int i = 0; i < CurrentDrawables.Length; ++i)
		{
			CurrentDrawables[i] = RAGE.Elements.Player.LocalPlayer.GetDrawableVariation(i);
			CurrentTextures[i] = RAGE.Elements.Player.LocalPlayer.GetTextureVariation(i);
		}

		foreach (ECustomPropSlot propSlot in Enum.GetValues(typeof(ECustomPropSlot)))
		{
			CurrentPropDrawables[propSlot] = RAGE.Elements.Player.LocalPlayer.GetPropIndex((int)propSlot);
			CurrentPropTextures[propSlot] = RAGE.Elements.Player.LocalPlayer.GetPropTextureIndex((int)propSlot);

			if (CurrentPropDrawables[propSlot] == -1)
			{
				CurrentPropDrawables[propSlot] = 0;
			}

			if (CurrentPropTextures[propSlot] == -1)
			{
				CurrentPropTextures[propSlot] = 0;
			}
		}

		EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
		bool bIsCustom = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_CUSTOM);

		if (bIsCustom)
		{
			//////////////////////////////////////////////////
			// CLOTHING TAB
			//////////////////////////////////////////////////

			// NOTE: For all props, we add on 1, because 0 is 'disable'
			// NOTE: Because of the above, this means we also set max to 0 initially because they have 'no prop' as default
			// PROP: HATS
			List<int> lstHats = SkinConstants.GetHatsMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Hats", "Drawable", 0, lstHats.Count, CurrentPropDrawables[ECustomPropSlot.Hats], UIEventID.ClothingStore_SetPropDrawable_Hats, "Texture", 0, 0, CurrentPropTextures[ECustomPropSlot.Hats], UIEventID.ClothingStore_SetPropTexture_Hats, UIEventID.ClothingStore_OnRootChanged_Hats);
			m_UI.AddSeperator();

			// PROP: GLASSES
			List<int> lstGlasses = SkinConstants.GetGlassesMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Glasses", "Drawable", 0, lstGlasses.Count, CurrentPropDrawables[ECustomPropSlot.Glasses], UIEventID.ClothingStore_SetPropDrawable_Glasses, "Texture", 0, 0, CurrentPropTextures[ECustomPropSlot.Glasses], UIEventID.ClothingStore_SetPropTexture_Glasses, UIEventID.ClothingStore_OnRootChanged_Glasses);
			m_UI.AddSeperator();

			// MASKS
			List<int> lstMasks = SkinConstants.GetMasks(gender);
			int MasksMax = lstMasks.Count - 1;
			int maxTextureForMasks = RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Masks, 0) - 1;

			int currentMask = Array.IndexOf(lstMasks.ToArray(), CurrentDrawables[(int)ECustomClothingComponent.Masks]);

			// Dont set beards...
			if (MaskHelpers.MasksFunctioningAsBeards.Contains(currentMask))
			{
				currentMask = 0;
			}

			// if its not in the mask list, just goto zero (some masks exist for players, but arent in the list, e.g. halloween mask)
			if (!lstMasks.Contains(currentMask))
			{
				currentMask = 0;
			}

			m_UI.AddTabContent_ClothingSelector("Masks", "Drawable", 0, MasksMax, currentMask, UIEventID.ClothingStore_SetComponentDrawable_Masks,
				"Texture", 0, maxTextureForMasks, CurrentTextures[(int)ECustomClothingComponent.Masks], UIEventID.ClothingStore_SetComponentTexture_Masks, UIEventID.ClothingStore_OnRootChanged_Masks);
			m_UI.AddSeperator();

			// PROP: EARRINGS
			List<int> lstEarrings = SkinConstants.GetEaringsMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Earrings", "Drawable", 0, lstEarrings.Count, CurrentPropDrawables[ECustomPropSlot.Ears], UIEventID.ClothingStore_SetPropDrawable_Earrings, "Texture", 0, 0, CurrentPropTextures[ECustomPropSlot.Ears], UIEventID.ClothingStore_SetPropTexture_Earrings, UIEventID.ClothingStore_OnRootChanged_Earrings);
			m_UI.AddSeperator();

			// TORSO
			List<int> lstTorsos = SkinConstants.GetTorsoMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Torsos", "Drawable", 0, lstTorsos.Count - 1, CurrentDrawables[(int)ECustomClothingComponent.Torsos], UIEventID.ClothingStore_SetComponentDrawable_Torso,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Torsos, 0) - 1, CurrentTextures[(int)ECustomClothingComponent.Torsos], UIEventID.ClothingStore_SetComponentTexture_Torso, UIEventID.ClothingStore_OnRootChanged_Torso);
			m_UI.AddSeperator();

			// TOPS
			List<int> lstTops = SkinConstants.GetTopsMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Tops", "Drawable", 0, lstTops.Count - 1, CurrentDrawables[(int)ECustomClothingComponent.Tops], UIEventID.ClothingStore_SetComponentDrawable_Tops,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Tops, 0) - 1, CurrentTextures[(int)ECustomClothingComponent.Tops], UIEventID.ClothingStore_SetComponentTexture_Tops, UIEventID.ClothingStore_OnRootChanged_Tops);
			m_UI.AddSeperator();

			// UNDERSHIRTS
			List<int> lstUndershirts = SkinConstants.GetUndershirtMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Undershirts", "Drawable", 0, lstUndershirts.Count - 1, CurrentDrawables[(int)ECustomClothingComponent.Undershirts], UIEventID.ClothingStore_SetComponentDrawable_Undershirts,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Undershirts, 0) - 1, CurrentTextures[(int)ECustomClothingComponent.Undershirts], UIEventID.ClothingStore_SetComponentTexture_Undershirts, UIEventID.ClothingStore_OnRootChanged_Undershirts);
			m_UI.AddSeperator();

			// ACCESSORIES
			List<int> lstAccessories = SkinConstants.GetAccessoriesMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Accessories", "Drawable", 0, lstAccessories.Count - 1, CurrentDrawables[(int)ECustomClothingComponent.Accessories], UIEventID.ClothingStore_SetComponentDrawable_Accessories,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Accessories, 0) - 1, CurrentTextures[(int)ECustomClothingComponent.Accessories], UIEventID.ClothingStore_SetComponentTexture_Accessories, UIEventID.ClothingStore_OnRootChanged_Accessories);
			m_UI.AddSeperator();

			// NOTE: For all props, we add on 1, because 0 is 'disable'
			// PROP: WATCHES
			List<int> lstWatches = SkinConstants.GetWatchesMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Watches", "Drawable", 0, lstWatches.Count, CurrentPropDrawables[ECustomPropSlot.Watches], UIEventID.ClothingStore_SetPropDrawable_Watches, "Texture", 0, 0, CurrentPropDrawables[ECustomPropSlot.Watches], UIEventID.ClothingStore_SetPropTexture_Watches, UIEventID.ClothingStore_OnRootChanged_Watches);
			m_UI.AddSeperator();

			// PROP: BRACELETS
			List<int> lstBracelets = SkinConstants.GetBraceletsMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Bracelets", "Drawable", 0, lstBracelets.Count, CurrentPropDrawables[ECustomPropSlot.Bracelets], UIEventID.ClothingStore_SetPropDrawable_Bracelets, "Texture", 0, 0, CurrentPropDrawables[ECustomPropSlot.Bracelets], UIEventID.ClothingStore_SetPropTexture_Bracelets, UIEventID.ClothingStore_OnRootChanged_Bracelets);
			m_UI.AddSeperator();

			// DECALS
			List<int> lstDecals = SkinConstants.GetDecalsMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Decals", "Drawable", 0, lstDecals.Count - 1, CurrentDrawables[(int)ECustomClothingComponent.Decals], UIEventID.ClothingStore_SetComponentDrawable_Decals,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Decals, 0) - 1, CurrentTextures[(int)ECustomClothingComponent.Decals], UIEventID.ClothingStore_SetComponentTexture_Decals, UIEventID.ClothingStore_OnRootChanged_Decals);
			m_UI.AddSeperator();

			// LEGS
			List<int> lstLegs = SkinConstants.GetLegsMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Legs", "Drawable", 0, lstLegs.Count - 1, CurrentDrawables[(int)ECustomClothingComponent.Legs], UIEventID.ClothingStore_SetComponentDrawable_Legs,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Legs, 0) - 1, CurrentTextures[(int)ECustomClothingComponent.Legs], UIEventID.ClothingStore_SetComponentTexture_Legs, UIEventID.ClothingStore_OnRootChanged_Legs);
			m_UI.AddSeperator();

			// SHOES
			List<int> lstShoes = SkinConstants.GetShoesMaxForGender(gender);
			m_UI.AddTabContent_ClothingSelector("Shoes", "Drawable", 0, lstShoes.Count - 1, CurrentDrawables[(int)ECustomClothingComponent.Shoes], UIEventID.ClothingStore_SetComponentDrawable_Shoes,
				"Texture", 0, RAGE.Elements.Player.LocalPlayer.GetNumberOfTextureVariations((int)ECustomClothingComponent.Shoes, 0) - 1, CurrentTextures[(int)ECustomClothingComponent.Shoes], UIEventID.ClothingStore_SetComponentTexture_Shoes, UIEventID.ClothingStore_OnRootChanged_Shoes);
			m_UI.AddSeperator();
		}
		else
		{
			uint[] arrayToUse = SkinConstants.GetPremadeSkinsForGender(gender);
			int skinIndex = Array.IndexOf(arrayToUse, m_PremadeCharSkin);

			int skinIDMax = arrayToUse.Length - 1;
			m_UI.AddTabContent_NumberSelector("Skin", Helpers.FormatString("{0} to {1}", 0, (uint)skinIDMax), 0, (uint)skinIDMax, (uint)skinIndex, UIEventID.ClothingStore_SetSkinID);
		}

		// get initial pricing info, mainly so we can see if we have a token or not
		RequestPricing();
	}

	private void RequestPricing()
	{
		NetworkEventSender.SendNetworkEvent_ClothingStore_CalculatePrice(CurrentDrawables, CurrentTextures, CurrentPropDrawables, CurrentPropTextures, m_PremadeCharSkin);
	}

	private void OnRender()
	{

	}

	private void OnExit()
	{

	}

	private void OnFinish()
	{
		NetworkEventSender.SendNetworkEvent_ClothingStore_OnCheckout(StoreSystem.CurrentStoreID, CurrentDrawables, CurrentTextures, CurrentPropDrawables, CurrentPropTextures, m_PremadeCharSkin);
	}
}

