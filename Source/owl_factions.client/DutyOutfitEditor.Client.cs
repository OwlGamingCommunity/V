using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

public class DutyOutfitEditor : GenericCharacterCustomization
{
	private enum EDutyOutfitEditorUIState
	{
		List,
		Editor_SelectPreset,
		Editor_Clothing,
	}

	private EDutyOutfitEditorUIState m_State = EDutyOutfitEditorUIState.List;
	private List<CItemInstanceDef> m_lstDutyOutfits = null;
	private EntityDatabaseID m_DutyOutfitBeingEdited = -1;
	private string g_strDutyOutfitName = String.Empty;
	private EDutyType m_DutyType = EDutyType.None;
	private EDutyOutfitType m_CurrentOutfitType = EDutyOutfitType.Custom;
	private uint m_PremadeSkinHash = 0;
	private bool m_bHairVisible = true;

	private Dictionary<EDutyWeaponSlot, EItemID> CurrentLoadout = new Dictionary<EDutyWeaponSlot, EItemID>();

	public DutyOutfitEditor() : base(EGUIID.DutyOutfitEditor)
	{
		SetNameAndCallbacks("Duty Outfit Editor", OnPreFinish, OnFinish, OnRequestShow, OnPreExit, OnExit, OnRender);

		UIEvents.DutyOutfitEditor_SetTop += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Tops, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetMask += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Masks, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetTorso += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Torsos, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetBodyArmor += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.BodyArmor, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetUndershirt += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Undershirts, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetBagsAndParachutes += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.BagsAndParachutes, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetAccessory += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Accessories, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetDecals += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Decals, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetLegs += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Legs, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetShoes += (string strValue, string strDisplay) => { ChangeClothing(ECustomClothingComponent.Shoes, strValue, strDisplay); };

		UIEvents.DutyOutfitEditor_SetHat += (string strValue, string strDisplay) => { ChangeProp(ECustomPropSlot.Hats, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetGlasses += (string strValue, string strDisplay) => { ChangeProp(ECustomPropSlot.Glasses, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetEars += (string strValue, string strDisplay) => { ChangeProp(ECustomPropSlot.Ears, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetWatches += (string strValue, string strDisplay) => { ChangeProp(ECustomPropSlot.Watches, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_SetBracelets += (string strValue, string strDisplay) => { ChangeProp(ECustomPropSlot.Bracelets, strValue, strDisplay); };

		UIEvents.DutyOutfitEditor_Loadout_SetPursuitAccessory += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.PursuitAccessory, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetMelee += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.Melee, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetAccessory1 += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.Accessory1, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetAccessory2 += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.Accessory2, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetAccessory3 += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.Accessory3, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetHandgun1 += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.HandgunHipHolster, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetHandgun2 += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.HandgunLegHolster, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetLargeWeapon += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.LargeWeapon, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetProjectile += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.Projectile, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetProjectile2 += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.Projectile2, strValue, strDisplay); };
		UIEvents.DutyOutfitEditor_Loadout_SetLargeCarriedItem += (string strValue, string strDisplay) => { ChangeLoadout(EDutyWeaponSlot.LargeCarriedItem, strValue, strDisplay); };

		UIEvents.DutyOutfitEditor_EditOutfit += OnEditOutfit;
		UIEvents.DutyOutfitEditor_Outfit_OnMouseEnter += OnMouseEnterOutfitItem;

		NetworkEvents.EnterDutyOutfitEditor_Response += OnEnterDutyOutfitEditorResponse;
		NetworkEvents.RequestDutyOutfitList_Response += OnRequestDutyOutfitListResponse;

		UIEvents.DutyOutfitEditor_SelectPreset_Done += SelectPreset_Done;
		UIEvents.DutyOutfitEditor_SelectPreset_Cancel += SelectPreset_Cancel;
		UIEvents.DutyOutfitEditor_SelectPreset_DropdownSelectionChanged += SelectPreset_SelectionChanged;

		UIEvents.DutyOutfitEditor_SetOutfitName += OnSetOutfitName;
		UIEvents.DutyOutfitEditor_DeleteOutfit += OnDeleteOutfit;

		UIEvents.DutyOutfitEditor_SetHairVisible += SetHairVisible;
	}

	private void SetHairVisible(bool bVisible)
	{
		// radio buttons are inverted for bool, so we have to invert it back
		m_bHairVisible = !bVisible;
		UpdateHairStatus();
	}

	private void UpdateHairStatus()
	{
		if (m_bHairVisible)
		{
			SkinHelpers.ApplyTattoosAndHairTattoosForPlayer(RAGE.Elements.Player.LocalPlayer);

			int HAIRSTYLE = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_HAIRSTYLE);
			int HAIRCOLOR = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_HAIRCOLOR);
			int HAIRCOLORHIGHLIGHTS = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.CC_HAIRCOLORHIGHLIGHTS);

			RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.HairStyles, HAIRSTYLE, 0, 0);
			RAGE.Elements.Player.LocalPlayer.SetHairColor(HAIRCOLOR, HAIRCOLORHIGHLIGHTS);
		}
		else
		{
			SkinHelpers.ApplyTattoosAndHairTattoosForPlayer(RAGE.Elements.Player.LocalPlayer, true);
			RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.HairStyles, 0, 0, 0);
		}
	}

	public void Activate(EDutyType a_DutyType)
	{
		m_DutyType = a_DutyType;
		ResetData();
		NetworkEventSender.SendNetworkEvent_EnterDutyOutfitEditor(m_DutyType);
	}

	private void ResetData()
	{
		foreach (EDutyWeaponSlot slot in Enum.GetValues(typeof(EDutyWeaponSlot)))
		{
			CurrentLoadout[slot] = EItemID.None;
		}
	}

	private void OnEnterDutyOutfitEditorResponse(List<CItemInstanceDef> lstOutfits)
	{
		m_lstDutyOutfits = lstOutfits;

		base.ForceShow();
		ForceClothesState(true); // set clothes to visible

		GotoList();
	}

	private void OnRequestDutyOutfitListResponse(List<CItemInstanceDef> lstOutfits)
	{
		m_lstDutyOutfits = lstOutfits;

		m_UI.SetVisible(true, true, true);
		ForceClothesState(true); // set clothes to visible

		GotoList();
	}

	private bool OnRequestShow()
	{
		// false = don't show, wait for event
		return false;
	}

	private void OnRender()
	{

	}

	private void OnExit()
	{

	}

	private void OnSetOutfitName(string strOutfitName)
	{
		g_strDutyOutfitName = strOutfitName;
	}

	private void ChangeLoadout(EDutyWeaponSlot loadoutSlot, string strValue, string strDisplay)
	{
		int indexWithinList = Convert.ToInt32(strValue);
		List<EItemID> lstLoadoutItems = GetLoadoutItemsAvailable(m_DutyType, loadoutSlot, CurrentLoadout[loadoutSlot], out int initialIndex);
		EItemID loadoutItem = lstLoadoutItems[indexWithinList];
		CurrentLoadout[loadoutSlot] = loadoutItem;
	}

	private void ChangeProp(ECustomPropSlot slot, string strValue, string strDisplay)
	{
		int indexWithinList = Convert.ToInt32(strValue);
		int model = 0;
		int texture = 0;

		if (indexWithinList == -1)
		{
			model = -1;
			texture = -1;
		}
		else
		{
			EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
			List<DutyCustomAvailablePropItem> lstProps = GetAvailablePropItems(m_DutyType, gender, slot, CurrentPropDrawables[slot], CurrentPropTextures[slot], out int initialIndex);
			DutyCustomAvailablePropItem clothingItem = lstProps[indexWithinList];

			model = clothingItem.Model;
			texture = clothingItem.Texture;
		}

		if (model != -1 && texture != -1)
		{
			CurrentPropDrawables[slot] = model;
			CurrentPropTextures[slot] = texture;

			if (IsClothingVisible())
			{
				ApplyProp(slot);
			}
		}
		else
		{
			CurrentPropDrawables[slot] = 0;
			CurrentPropTextures[slot] = 0;

			if (IsClothingVisible())
			{
				ClearProp(slot);
			}
		}
	}

	private void ChangeClothing(ECustomClothingComponent slot, string strValue, string strDisplay)
	{
		int indexWithinList = Convert.ToInt32(strValue);
		int model = 0;
		int texture = 0;

		if (indexWithinList == -1)
		{
			EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
			if (gender == EGender.Male)
			{
				if (slot == ECustomClothingComponent.Torsos)
				{
					model = 15;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Legs)
				{
					model = 21;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Tops)
				{
					model = 15;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Undershirts)
				{
					model = 15;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Shoes)
				{
					model = 34;
					texture = 0;
				}
				else
				{
					model = -1;
					texture = -1;
				}
			}
			else
			{
				if (slot == ECustomClothingComponent.Torsos)
				{
					model = 15;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Legs)
				{
					model = 56;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Tops)
				{
					model = 15;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Undershirts)
				{
					model = 15;
					texture = 0;
				}
				else if (slot == ECustomClothingComponent.Shoes)
				{
					model = 35;
					texture = 0;
				}
				else
				{
					model = -1;
					texture = -1;
				}
			}
		}
		else
		{
			EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
			List<DutyCustomAvailableClothesItem> lstClothing = GetAvailableClothingItems(m_DutyType, gender, slot, CurrentDrawables[(int)slot], CurrentTextures[(int)slot], out int initialIndex);


			DutyCustomAvailableClothesItem clothingItem = lstClothing[indexWithinList];

			model = clothingItem.Model;
			texture = clothingItem.Texture;
		}

		if (model != -1 && texture != -1)
		{
			CurrentDrawables[(int)slot] = model;
			CurrentTextures[(int)slot] = texture;

			if (IsClothingVisible())
			{
				ApplyComponent(slot);
			}
		}
		else
		{
			CurrentDrawables[(int)slot] = 0;
			CurrentTextures[(int)slot] = 0;

			if (IsClothingVisible())
			{
				ClearComponent(slot);
			}
		}
	}

	private void OnDeleteOutfit()
	{
		if (m_DutyOutfitBeingEdited != -1)
		{
			NetworkEventSender.SendNetworkEvent_DutyOutfitEditor_DeleteOutfit(m_DutyOutfitBeingEdited);
		}

		ReRequestOutfitList();
	}

	private void ApplyOutfitPreview(int outfitIndex)
	{
		if (outfitIndex < m_lstDutyOutfits.Count)
		{
			CItemInstanceDef outfitItem = m_lstDutyOutfits[outfitIndex];
			ApplyOutfitPreview(outfitItem);
		}
	}

	public void ApplyOutfitPreview(CItemInstanceDef outfitItem, bool bForce = false)
	{
		RAGE.Elements.Player.LocalPlayer.SetAlpha(255, false);
		EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

		if (outfitItem != null)
		{
			CItemValueDutyOutfit itemValue = OwlJSON.DeserializeObject<CItemValueDutyOutfit>(outfitItem.GetValueDataSerialized(), EJsonTrackableIdentifier.ApplyOutfitPreview);
			g_strDutyOutfitName = itemValue.Name;


			if (itemValue.CharType == EDutyOutfitType.Custom)
			{
				m_CurrentOutfitType = EDutyOutfitType.Custom;
				m_PremadeSkinHash = 0;
				m_bHairVisible = !itemValue.HideHair;

				// have to set to custom, incase we were on a premade before
				int hair = RAGE.Elements.Player.LocalPlayer.GetDrawableVariation((int)ECustomClothingComponent.HairStyles);
				int hair_tex = RAGE.Elements.Player.LocalPlayer.GetTextureVariation((int)ECustomClothingComponent.HairStyles);
				int hair_pal = RAGE.Elements.Player.LocalPlayer.GetPaletteVariation((int)ECustomClothingComponent.HairStyles);
				RAGE.Elements.Player.LocalPlayer.SetDefaultComponentVariation();
				RAGE.Elements.Player.LocalPlayer.ClearAllProps();
				RAGE.Elements.Player.LocalPlayer.Model = gender == EGender.Male ? CharacterConstants.CustomMaleSkin : CharacterConstants.CustomFemaleSkin;

				// re-add hair (if visible)
				if (m_bHairVisible)
				{
					RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.HairStyles, hair, hair_tex, hair_pal);
				}

				// CLOTHES
				foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
				{
					int model = itemValue.Drawables[(int)component];
					int texture = itemValue.Textures[(int)component];

					if (model != -1 && texture != -1)
					{
						CurrentDrawables[(int)component] = model;
						CurrentTextures[(int)component] = texture;

						if (IsClothingVisible() || bForce)
						{
							ApplyComponent(component);
						}
					}
					else
					{
						CurrentDrawables[(int)component] = 0;
						CurrentTextures[(int)component] = 0;

						if (IsClothingVisible() || bForce)
						{
							ClearComponent(component);
						}
					}
				}

				// PROPS
				foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
				{
					int model = itemValue.PropDrawables[(int)prop];
					int texture = itemValue.PropTextures[(int)prop];

					if (model != -1 && texture != -1)
					{
						CurrentPropDrawables[prop] = model;
						CurrentPropTextures[prop] = texture;

						if (IsClothingVisible() || bForce)
						{
							ApplyProp(prop);
						}
					}
					else
					{
						CurrentPropDrawables[prop] = 0;
						CurrentPropDrawables[prop] = 0;

						if (IsClothingVisible() || bForce)
						{
							ClearProp(prop);
						}
					}
				}
			}
			else
			{
				m_CurrentOutfitType = EDutyOutfitType.Premade;
				m_PremadeSkinHash = itemValue.PremadeHash;
				m_bHairVisible = true;

				foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
				{
					CurrentDrawables[(int)component] = 0;
					CurrentTextures[(int)component] = 0;
				}

				foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
				{
					CurrentPropDrawables[prop] = 0;
					CurrentPropDrawables[prop] = 0;
				}

				RAGE.Elements.Player.LocalPlayer.SetDefaultComponentVariation();
				RAGE.Elements.Player.LocalPlayer.ClearAllProps();
				RAGE.Elements.Player.LocalPlayer.Model = itemValue.PremadeHash;
			}

			// LOADOUT (required to select existing dropdown item when populating)
			foreach (var kvPair in itemValue.Loadout)
			{
				CurrentLoadout[(EDutyWeaponSlot)kvPair.Key] = (EItemID)kvPair.Value;
			}
		}

		UpdateHairStatus();
	}

	private void OnMouseEnterOutfitItem(string strElementName, int outfitIndex)
	{
		if (outfitIndex < m_lstDutyOutfits.Count)
		{
			ApplyOutfitPreview(outfitIndex);
		}
	}

	private void OnEditOutfit(string strElementName, int outfitIndex)
	{
		if (outfitIndex < m_lstDutyOutfits.Count)
		{
			OnMouseEnterOutfitItem(strElementName, outfitIndex);

			CItemInstanceDef outfitItem = m_lstDutyOutfits[outfitIndex];
			GotoEditor_SelectPreset(outfitItem.DatabaseID);
		}
	}

	private void SelectPreset_Done(string strName, string strValue)
	{
		EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

		int index = Convert.ToInt32(strValue);
		if (index >= 0) // custom
		{
			EDutyCustomSkinPresets presetID = (EDutyCustomSkinPresets)Convert.ToInt32(strValue);
			DutyCustomSkinPreset preset = DutyCustomSkins.CustomSkinPresets[m_DutyType][presetID];

			// init editor with preset values
			g_strDutyOutfitName = preset.strDisplayName;
			m_bHairVisible = true;

			// have to set to custom, incase we were on a premade before
			int hair = RAGE.Elements.Player.LocalPlayer.GetDrawableVariation((int)ECustomClothingComponent.HairStyles);
			int hair_tex = RAGE.Elements.Player.LocalPlayer.GetTextureVariation((int)ECustomClothingComponent.HairStyles);
			int hair_pal = RAGE.Elements.Player.LocalPlayer.GetPaletteVariation((int)ECustomClothingComponent.HairStyles);
			RAGE.Elements.Player.LocalPlayer.SetDefaultComponentVariation();
			RAGE.Elements.Player.LocalPlayer.ClearAllProps();
			RAGE.Elements.Player.LocalPlayer.Model = gender == EGender.Male ? CharacterConstants.CustomMaleSkin : CharacterConstants.CustomFemaleSkin;

			// re-add hair (if visible)
			if (m_bHairVisible)
			{
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.HairStyles, hair, hair_tex, hair_pal);
			}
			UpdateHairStatus();

			foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
			{
				CurrentDrawables[(int)component] = 0;
				CurrentTextures[(int)component] = 0;
			}

			foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
			{
				CurrentPropDrawables[prop] = 0;
				CurrentPropTextures[prop] = 0;
			}

			// copy in our presets
			foreach (var kvPair in preset.Slots)
			{
				if (kvPair.IsProp)
				{
					CurrentPropDrawables[kvPair.Prop] = kvPair.Drawable;
					CurrentPropTextures[kvPair.Prop] = kvPair.Texture;
				}
				else
				{
					CurrentDrawables[(int)kvPair.Component] = kvPair.Drawable;
					CurrentTextures[(int)kvPair.Component] = kvPair.Texture;
				}
			}

			// now apply the combined version
			foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
			{
				int drawable = CurrentDrawables[(int)component];
				int texture = CurrentTextures[(int)component];

				if (drawable != -1 && texture != -1)
				{
					ApplyComponent(component);
				}
				else
				{
					ClearComponent(component);
				}
			}

			foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
			{
				int drawable = CurrentPropDrawables[prop];
				int texture = CurrentPropTextures[prop];

				if (drawable != -1 && texture != -1)
				{
					ApplyProp(prop);
				}
				else
				{
					ClearProp(prop);
				}
			}
		}
		else // premade
		{
			int realIndex = Math.Abs(index) - 1; // subtract 1 because we actually start at -1 since 0 is a legit custom skin

			// get premade
			m_PremadeSkinHash = DutyCustomSkins.PremadeSkins[m_DutyType][gender].ElementAt(realIndex).Key;

			// init editor with preset values
			g_strDutyOutfitName = strName;
			m_bHairVisible = true;

			RAGE.Elements.Player.LocalPlayer.SetDefaultComponentVariation();
			RAGE.Elements.Player.LocalPlayer.ClearAllProps();
			RAGE.Elements.Player.LocalPlayer.Model = m_PremadeSkinHash;
		}

		GotoEditor_EditOutfit();
	}

	private void GotoEditor_EditOutfit()
	{
		m_UI.SetVisible(true, true, true);
		m_State = EDutyOutfitEditorUIState.Editor_Clothing;
		PopulateUI();
	}

	private void SelectPreset_Cancel()
	{
		GoBackToOutfitList();
	}

	private void SelectPreset_SelectionChanged(string strName, string strValue)
	{
		EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

		int index = Convert.ToInt32(strValue);
		if (index >= 0) // custom
		{
			m_CurrentOutfitType = EDutyOutfitType.Custom;
			m_PremadeSkinHash = 0;
			m_bHairVisible = true;

			// have to set to custom, incase we were on a premade before
			int hair = RAGE.Elements.Player.LocalPlayer.GetDrawableVariation((int)ECustomClothingComponent.HairStyles);
			int hair_tex = RAGE.Elements.Player.LocalPlayer.GetTextureVariation((int)ECustomClothingComponent.HairStyles);
			int hair_pal = RAGE.Elements.Player.LocalPlayer.GetPaletteVariation((int)ECustomClothingComponent.HairStyles);
			RAGE.Elements.Player.LocalPlayer.SetDefaultComponentVariation();
			RAGE.Elements.Player.LocalPlayer.ClearAllProps();
			RAGE.Elements.Player.LocalPlayer.Model = gender == EGender.Male ? CharacterConstants.CustomMaleSkin : CharacterConstants.CustomFemaleSkin;

			// re-add hair (if visible)
			if (m_bHairVisible)
			{
				RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)ECustomClothingComponent.HairStyles, hair, hair_tex, hair_pal);
			}
			UpdateHairStatus();

			EDutyCustomSkinPresets presetID = (EDutyCustomSkinPresets)Convert.ToInt32(strValue);
			DutyCustomSkinPreset preset = DutyCustomSkins.CustomSkinPresets[m_DutyType][presetID];

			// apply it
			foreach (DutyCustomSkinPresetSlot slot in preset.Slots)
			{
				// TODO_DUTY: custom beard support
				/*
				if (slot.Component == ECustomClothingComponent.Masks)
				{
					bHasMask = true;
				}
				*/

				slot.ApplyToLocalPlayer();
			}
		}
		else // premade
		{
			m_CurrentOutfitType = EDutyOutfitType.Premade;
			m_bHairVisible = true;

			int realIndex = Math.Abs(index) - 1; // subtract 1 because we actually start at -1 since 0 is a legit custom skin

			// get premade
			m_PremadeSkinHash = DutyCustomSkins.PremadeSkins[m_DutyType][gender].ElementAt(realIndex).Key;

			RAGE.Elements.Player.LocalPlayer.SetDefaultComponentVariation();
			RAGE.Elements.Player.LocalPlayer.ClearAllProps();
			RAGE.Elements.Player.LocalPlayer.Model = m_PremadeSkinHash;
		}
	}

	private List<DutyCustomAvailableClothesItem> GetAvailableClothingItems(EDutyType dutyType, EGender gender, ECustomClothingComponent component, int currentModel, int currentTexture, out int initialIndex)
	{
		initialIndex = -1;

		if (DutyCustomSkins.AvailableClothes.ContainsKey(dutyType))
		{
			if (DutyCustomSkins.AvailableClothes[dutyType].ContainsKey(gender))
			{
				if (DutyCustomSkins.AvailableClothes[dutyType][gender].ContainsKey(component))
				{
					List<DutyCustomAvailableClothesItem> lstClothing = DutyCustomSkins.AvailableClothes[dutyType][gender][component];

					// Is the current value in the array?
					int index = 0;
					foreach (var clothes in lstClothing)
					{
						if (clothes.Model == currentModel && clothes.Texture == currentTexture)
						{
							initialIndex = index;
							break;
						}

						++index;
					}
					return lstClothing;
				}
			}
		}

		return new List<DutyCustomAvailableClothesItem>();
	}

	private List<DutyCustomAvailablePropItem> GetAvailablePropItems(EDutyType dutyType, EGender gender, ECustomPropSlot prop, int currentModel, int currentTexture, out int initialIndex)
	{
		initialIndex = -1;

		if (DutyCustomSkins.AvailableProps.ContainsKey(dutyType))
		{
			if (DutyCustomSkins.AvailableProps[dutyType].ContainsKey(gender))
			{
				if (DutyCustomSkins.AvailableProps[dutyType][gender].ContainsKey(prop))
				{
					List<DutyCustomAvailablePropItem> lstProps = DutyCustomSkins.AvailableProps[dutyType][gender][prop];

					// Is the current value in the array?
					int index = 0;
					foreach (var propInst in lstProps)
					{
						if (propInst.Model == currentModel && propInst.Texture == currentTexture)
						{
							initialIndex = index;
							break;
						}

						++index;
					}
					return lstProps;
				}
			}
		}

		return new List<DutyCustomAvailablePropItem>();
	}

	private void AddClothingDropdown(ECustomClothingComponent component, string strDisplayName, UIEventID onChangeEvent)
	{
		EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
		List<DutyCustomAvailableClothesItem> lstItems = GetAvailableClothingItems(m_DutyType, gender, component, CurrentDrawables[(int)component], CurrentTextures[(int)component], out int initialIndex);

		m_UI.AddPendingDropdownItem("-1", "None");

		int index = 0;
		foreach (var item in lstItems) { m_UI.AddPendingDropdownItem(index.ToString(), item.DisplayName); ++index; }

		string strInitialDisplayName = initialIndex == -1 ? "None" : lstItems[initialIndex].DisplayName;
		m_UI.AddTabContent_Dropdown(strDisplayName, Helpers.FormatString("{0} options", lstItems.Count), strInitialDisplayName, onChangeEvent);
	}

	private void AddPropDropdown(ECustomPropSlot prop, string strDisplayName, UIEventID onChangeEvent)
	{
		EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
		List<DutyCustomAvailablePropItem> lstItems = GetAvailablePropItems(m_DutyType, gender, prop, CurrentPropDrawables[prop], CurrentPropTextures[prop], out int initialIndex);

		m_UI.AddPendingDropdownItem("-1", "None");

		int index = 0;
		foreach (var item in lstItems) { m_UI.AddPendingDropdownItem(index.ToString(), item.DisplayName); ++index; }

		string strInitialDisplayName = initialIndex == -1 ? "None" : lstItems[initialIndex].DisplayName;
		m_UI.AddTabContent_Dropdown(strDisplayName, Helpers.FormatString("{0} options", lstItems.Count), strInitialDisplayName, onChangeEvent);
	}

	private void AddLoadoutDropdown(EDutyWeaponSlot slot, string strDisplayName, UIEventID onChangeEvent)
	{
		var lstItems = GetLoadoutItemsAvailable(m_DutyType, slot, CurrentLoadout[slot], out int initialIndex);

		if (lstItems.Count > 0)
		{
			int index = 0;
			foreach (EItemID item in lstItems)
			{
				string strName = String.Empty;
				if (item == EItemID.None)
				{
					strName = "None";
				}
				else
				{
					CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item];
					strName = itemDef.GetNameIgnoreGenericItems();
				}

				m_UI.AddPendingDropdownItem(index.ToString(), strName); ++index;
			}

			string strInitialDisplayName;

			if (initialIndex == -1 || lstItems[initialIndex] == EItemID.None)
			{
				strInitialDisplayName = "None";
			}
			else
			{
				EItemID item = lstItems[initialIndex];
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[item];
				strInitialDisplayName = itemDef.GetNameIgnoreGenericItems();
			}

			m_UI.AddTabContent_Dropdown(strDisplayName, Helpers.FormatString("{0} options", lstItems.Count), strInitialDisplayName, onChangeEvent);
		}
	}

	private void PopulateUI()
	{
		m_UI.Reset();
		m_UI.SetConfirmButtonText("Save Outfit");
		m_UI.SetExitButtonText("Exit Without Saving");

		// name
		m_UI.AddTabContent_Textbox("Outfit Name", "Give your duty outfit a name so you can identify it later.", 24, false, UIEventID.DutyOutfitEditor_SetOutfitName, g_strDutyOutfitName);

		if (m_CurrentOutfitType == EDutyOutfitType.Custom)
		{
			// hide hair
			m_UI.AddTabContent_TwoRadioOptions("Hair", "This option lets you hide your hair if it interferes with a certain clothing item.", "Visible", "Hidden", UIEventID.DutyOutfitEditor_SetHairVisible, m_bHairVisible ? 0 : 1);

			AddPropDropdown(ECustomPropSlot.Hats, "Hats", UIEventID.DutyOutfitEditor_SetHat);
			AddClothingDropdown(ECustomClothingComponent.Masks, "Masks", UIEventID.DutyOutfitEditor_SetMask);
			AddPropDropdown(ECustomPropSlot.Glasses, "Glasses", UIEventID.DutyOutfitEditor_SetGlasses);
			AddPropDropdown(ECustomPropSlot.Ears, "Ears", UIEventID.DutyOutfitEditor_SetEars);
			AddClothingDropdown(ECustomClothingComponent.Tops, "Tops", UIEventID.DutyOutfitEditor_SetTop);
			AddClothingDropdown(ECustomClothingComponent.Torsos, "Torsos", UIEventID.DutyOutfitEditor_SetTorso);
			AddClothingDropdown(ECustomClothingComponent.BodyArmor, "Chest", UIEventID.DutyOutfitEditor_SetBodyArmor);
			AddClothingDropdown(ECustomClothingComponent.Undershirts, "Undershirts", UIEventID.DutyOutfitEditor_SetUndershirt);
			AddPropDropdown(ECustomPropSlot.Watches, "Watches", UIEventID.DutyOutfitEditor_SetWatches);
			AddPropDropdown(ECustomPropSlot.Bracelets, "Bracelets", UIEventID.DutyOutfitEditor_SetBracelets);
			AddClothingDropdown(ECustomClothingComponent.BagsAndParachutes, "Bags And Parachutes", UIEventID.DutyOutfitEditor_SetBagsAndParachutes);
			AddClothingDropdown(ECustomClothingComponent.Accessories, "Accessories", UIEventID.DutyOutfitEditor_SetAccessory);
			AddClothingDropdown(ECustomClothingComponent.Decals, "Decals", UIEventID.DutyOutfitEditor_SetDecals);
			AddClothingDropdown(ECustomClothingComponent.Legs, "Legs", UIEventID.DutyOutfitEditor_SetLegs);
			AddClothingDropdown(ECustomClothingComponent.Shoes, "Shoes", UIEventID.DutyOutfitEditor_SetShoes);
		}

		// loadouts
		m_UI.AddSeperator();
		AddLoadoutDropdown(EDutyWeaponSlot.PursuitAccessory, "Pursuit Accessory", UIEventID.DutyOutfitEditor_Loadout_SetPursuitAccessory);
		AddLoadoutDropdown(EDutyWeaponSlot.Melee, "Melee", UIEventID.DutyOutfitEditor_Loadout_SetMelee);
		AddLoadoutDropdown(EDutyWeaponSlot.Accessory1, "Accessory 1", UIEventID.DutyOutfitEditor_Loadout_SetAccessory1);
		AddLoadoutDropdown(EDutyWeaponSlot.Accessory2, "Accessory 2", UIEventID.DutyOutfitEditor_Loadout_SetAccessory2);
		AddLoadoutDropdown(EDutyWeaponSlot.Accessory3, "Accessory 3", UIEventID.DutyOutfitEditor_Loadout_SetAccessory3);
		AddLoadoutDropdown(EDutyWeaponSlot.HandgunHipHolster, "Handgun (Waist Holster)", UIEventID.DutyOutfitEditor_Loadout_SetHandgun1);
		AddLoadoutDropdown(EDutyWeaponSlot.HandgunLegHolster, "Handgun (Leg Holster)", UIEventID.DutyOutfitEditor_Loadout_SetHandgun2);
		AddLoadoutDropdown(EDutyWeaponSlot.LargeWeapon, "Large Weapon", UIEventID.DutyOutfitEditor_Loadout_SetLargeWeapon);
		AddLoadoutDropdown(EDutyWeaponSlot.Projectile, "Projectile", UIEventID.DutyOutfitEditor_Loadout_SetProjectile);
		AddLoadoutDropdown(EDutyWeaponSlot.Projectile2, "Projectile 2", UIEventID.DutyOutfitEditor_Loadout_SetProjectile2);
		AddLoadoutDropdown(EDutyWeaponSlot.LargeCarriedItem, "Large Carried Item", UIEventID.DutyOutfitEditor_Loadout_SetLargeCarriedItem);
	}

	private List<EItemID> GetLoadoutItemsAvailable(EDutyType dutyType, EDutyWeaponSlot dutyItemSlot, EItemID currentItem, out int initialIndex)
	{
		initialIndex = -1;
		// Do we have an item for it?
		if (DutyCustomSkins.DutyLoadouts.ContainsKey(dutyType))
		{
			if (DutyCustomSkins.DutyLoadouts[dutyType].ContainsKey(dutyItemSlot))
			{
				List<EItemID> lstItems = new List<EItemID>();
				List<EItemID> lstItemsDefined = DutyCustomSkins.DutyLoadouts[dutyType][dutyItemSlot];
				if (lstItemsDefined.Count > 0)
				{
					lstItems.Add(EItemID.None);
					lstItems.AddRange(lstItemsDefined);

					int index = 0;
					foreach (EItemID definedItem in lstItems)
					{
						if (definedItem == currentItem)
						{
							initialIndex = index;
							break;
						}
						++index;
					}
				}

				return lstItems;
			}
		}

		return new List<EItemID>();
	}

	private void GotoEditor_SelectPreset(EntityDatabaseID outfitBeingEdited)
	{
		m_State = EDutyOutfitEditorUIState.Editor_SelectPreset;
		m_DutyOutfitBeingEdited = outfitBeingEdited;

		// If making a new one, offer presets
		if (m_DutyOutfitBeingEdited == -1)
		{
			m_UI.SetVisible(false, true, false);

			Dictionary<string, string> dictDropdownItems = new Dictionary<string, string>();
			EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);

			// add customs (if we are custom)
			bool bIsCustom = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_CUSTOM);
			if (bIsCustom)
			{
				foreach (var kvPair in DutyCustomSkins.CustomSkinPresets[m_DutyType])
				{
					DutyCustomSkinPreset preset = kvPair.Value;
					if (preset.Gender == gender)
					{
						dictDropdownItems.Add(preset.strDisplayName, ((int)preset.ID).ToString());
					}
				}
			}

			// add premades
			int premadeIndex = 1;
			foreach (var kvPair in DutyCustomSkins.PremadeSkins[m_DutyType][gender])
			{
				string strDisplayName = Helpers.FormatString("GTA Premade - {0}", kvPair.Value);
				dictDropdownItems.Add(strDisplayName, (-premadeIndex).ToString());

				++premadeIndex;
			}


			GenericDropdown.ShowGenericDropdown("Select Preset", dictDropdownItems, "Select Preset", "Please select a preset as the starting point for your outfit. You will be able to customize it afterwards.", "Customize", "Cancel",
				UIEventID.DutyOutfitEditor_SelectPreset_Done, UIEventID.DutyOutfitEditor_SelectPreset_Cancel, UIEventID.DutyOutfitEditor_SelectPreset_DropdownSelectionChanged, EPromptPosition.Center_Left);
		}
		else
		{
			GotoEditor_EditOutfit();

			// delete button if its not a new outfit
			if (m_DutyOutfitBeingEdited != -1)
			{
				m_UI.AddTabContent_GenericButton("Delete Outfit", UIEventID.DutyOutfitEditor_DeleteOutfit);
			}
		}
	}

	private void GotoList()
	{
		m_State = EDutyOutfitEditorUIState.List;
		m_UI.Reset();
		m_UI.SetConfirmButtonText("Create New Duty Outfit");
		m_UI.SetExitButtonText("Exit");

		int index = 0;
		foreach (CItemInstanceDef outfitItem in m_lstDutyOutfits)
		{
			CItemValueDutyOutfit itemValue = OwlJSON.DeserializeObject<CItemValueDutyOutfit>(outfitItem.GetValueDataSerialized(), EJsonTrackableIdentifier.DutyOutfitEditorGotoList);

			m_UI.AddTabContent_GenericListItem(itemValue.Name, "", "Click to Edit", UIEventID.DutyOutfitEditor_EditOutfit, UIEventID.DutyOutfitEditor_Outfit_OnMouseEnter, UIEventID.DutyOutfitEditor_Outfit_OnMouseExit, index);
			++index;
		}
	}

	private bool OnPreFinish()
	{
		if (m_State == EDutyOutfitEditorUIState.List) // finish button is actually create new outfit in this state
		{
			GotoEditor_SelectPreset(-1);
		}
		else if (m_State == EDutyOutfitEditorUIState.Editor_Clothing) // finish button is actually save
		{
			if (g_strDutyOutfitName.Length > 0)
			{
				Dictionary<ECustomClothingComponent, int> dictDrawables = new Dictionary<ECustomClothingComponent, int>();
				Dictionary<ECustomClothingComponent, int> dictTextures = new Dictionary<ECustomClothingComponent, int>();

				foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
				{
					dictDrawables.Add(component, CurrentDrawables[(int)component]);
					dictTextures.Add(component, CurrentTextures[(int)component]);
				}

				NetworkEventSender.SendNetworkEvent_DutyOutfitEditor_CreateOrUpdateOutfit(g_strDutyOutfitName, m_DutyType, dictDrawables, dictTextures, CurrentPropDrawables, CurrentPropTextures, CurrentLoadout, m_DutyOutfitBeingEdited, m_CurrentOutfitType, m_PremadeSkinHash, !m_bHairVisible);

				ReRequestOutfitList();
			}
			else
			{
				NotificationManager.ShowNotification("Edit Outfit", "Outfit must have a name.", ENotificationIcon.ExclamationSign);
			}

		}

		return false;
	}

	private void GoBackToOutfitList()
	{
		ReRequestOutfitList();
	}

	private bool OnPreExit()
	{
		if (m_State == EDutyOutfitEditorUIState.List) // exit button is actually exit (and go back to duty system)
		{
			SoftHide();
			FactionSystem.GetDutySystem()?.OnExitEditor();
			return false;
		}
		else if (m_State == EDutyOutfitEditorUIState.Editor_Clothing) // exit button is actually exit outfit editor without saving
		{
			GoBackToOutfitList();
			return false;
		}

		return false;
	}

	private void OnFinish()
	{

	}

	private void ReRequestOutfitList()
	{
		NetworkEventSender.SendNetworkEvent_RequestDutyOutfitList(m_DutyType);
		m_UI.SetVisible(false, false, true);
	}
}
// TODO_OUTFITS: Why do we have earrings 1 etc when its actually zero? is it because we give them a blank item for none?