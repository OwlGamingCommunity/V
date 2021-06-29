using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

public class OutfitEditor : GenericCharacterCustomization
{
	private enum EOutfitEditorUIState
	{
		List,
		Editor
	}

	private EOutfitEditorUIState m_State = EOutfitEditorUIState.List;
	private List<CItemInstanceDef> m_lstOutfits = null;
	private List<CItemInstanceDef> m_lstClothing = null;

	// outfit data
	private Dictionary<ECustomClothingComponent, List<CItemInstanceDef>> dictClothing = new Dictionary<ECustomClothingComponent, List<CItemInstanceDef>>();
	private Dictionary<ECustomPropSlot, List<CItemInstanceDef>> dictProps = new Dictionary<ECustomPropSlot, List<CItemInstanceDef>>();

	private EntityDatabaseID m_OutfitBeingEdited = -1;

	private string g_strOutfitName = String.Empty;
	private bool m_bHairVisible = true;

	private Dictionary<ECustomClothingComponent, int> ClothingIndexes { get; } = new Dictionary<ECustomClothingComponent, int>();
	private Dictionary<ECustomPropSlot, int> PropIndexes { get; } = new Dictionary<ECustomPropSlot, int>();

	public OutfitEditor() : base(EGUIID.OutfitEditor)
	{
		SetNameAndCallbacks("Outfit Editor", OnPreFinish, OnFinish, OnRequestShow, OnPreExit, OnExit, OnRender);

		UIEvents.OutfitEditor_EditOutfit += OnEditOutfit;
		UIEvents.OutfitEditor_DeleteOutfit += OnDeleteOutfit;

		UIEvents.OutfitEditor_Outfit_OnMouseEnter += OnMouseEnterOutfitItem;

		UIEvents.OutfitEditor_SetOutfitName += OnSetOutfitName;
		UIEvents.OutfitEditor_SetHatIndex += (int value) => { ChangeProp(ECustomPropSlot.Hats, value - 1); };
		UIEvents.OutfitEditor_SetMaskIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Masks, value - 1); };
		UIEvents.OutfitEditor_SetGlassesIndex += (int value) => { ChangeProp(ECustomPropSlot.Glasses, value - 1); };
		UIEvents.OutfitEditor_SetEarsIndex += (int value) => { ChangeProp(ECustomPropSlot.Ears, value - 1); };
		UIEvents.OutfitEditor_SetTorsoIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Torsos, value - 1); };
		UIEvents.OutfitEditor_SetTopsIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Tops, value - 1); };
		UIEvents.OutfitEditor_SetUndershirtsIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Undershirts, value - 1); };
		UIEvents.OutfitEditor_SetBagsAndParachutesIndex += (int value) => { ChangeClothing(ECustomClothingComponent.BagsAndParachutes, value - 1); };
		UIEvents.OutfitEditor_SetAccessoriesIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Accessories, value - 1); };
		UIEvents.OutfitEditor_SetWatchesIndex += (int value) => { ChangeProp(ECustomPropSlot.Watches, value - 1); };
		UIEvents.OutfitEditor_SetBraceletsIndex += (int value) => { ChangeProp(ECustomPropSlot.Bracelets, value - 1); };
		UIEvents.OutfitEditor_SetDecalsIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Decals, value - 1); };
		UIEvents.OutfitEditor_SetLegsIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Legs, value - 1); };
		UIEvents.OutfitEditor_SetShoesIndex += (int value) => { ChangeClothing(ECustomClothingComponent.Shoes, value - 1); };

		NetworkEvents.EnterOutfitEditor_Response += OnEnterOutfitEditorResponse;
		NetworkEvents.RequestOutfitList_Response += OnRequestOutfitListResponse;

		UIEvents.OutfitEditor_SetHairVisible += SetHairVisible;
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

	private void OnEnterOutfitEditorResponse(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing)
	{
		m_lstOutfits = lstOutfits;
		m_lstClothing = lstClothing;

		base.ForceShow();
		ForceClothesState(true); // set clothes to visible

		GotoList();
	}

	private void OnRequestOutfitListResponse(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing)
	{
		m_lstOutfits = lstOutfits;
		m_lstClothing = lstClothing;

		m_UI.SetVisible(true, true, true);
		ForceClothesState(true); // set clothes to visible

		GotoList();
	}

	private bool OnRequestShow()
	{
		// Got to trigger a server event to handle skin, dimension and get relevant items
		NetworkEventSender.SendNetworkEvent_EnterOutfitEditor();

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
		g_strOutfitName = strOutfitName;
	}

	private void ChangeProp(ECustomPropSlot slot, int value)
	{
		int model = -1;
		int texture = -1;

		if (value < 0) // none
		{
			model = -1;
			texture = -1;
		}
		else if (value <= dictProps[slot].Count)
		{
			CItemInstanceDef item = dictProps[slot][value];
			CItemValueClothingCustom itemValue = OwlJSON.DeserializeObject<CItemValueClothingCustom>(item.GetValueDataSerialized(), EJsonTrackableIdentifier.OutfitEditorChangeProp);
			model = itemValue.Model;
			texture = itemValue.Texture;
		}

		if (model != -1 && texture != -1)
		{
			PropIndexes[slot] = value;
			CurrentPropDrawables[slot] = model;
			CurrentPropTextures[slot] = texture;

			if (IsClothingVisible())
			{
				ApplyProp(slot);
			}
		}
		else
		{
			PropIndexes[slot] = value;
			CurrentPropDrawables[slot] = 0;
			CurrentPropTextures[slot] = 0;

			if (IsClothingVisible())
			{
				ClearProp(slot);
			}
		}
	}

	private void ChangeClothing(ECustomClothingComponent slot, int value)
	{
		int model = -1;
		int texture = -1;

		if (value < 0) // none
		{
			// give them a naked state for this component
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
		else if (value <= dictClothing[slot].Count)
		{
			CItemInstanceDef item = dictClothing[slot][value];
			// TODO: This is a double deserialize because getting the value clientside is weird, other places in here need fixing up too. Search for dupes.
			CItemValueClothingCustom itemValue = OwlJSON.DeserializeObject<CItemValueClothingCustom>(item.GetValueDataSerialized(), EJsonTrackableIdentifier.OutfitEditorChangeClothing);
			model = itemValue.Model;
			texture = itemValue.Texture;
		}

		if (model != -1 && texture != -1)
		{
			ClothingIndexes[slot] = value;
			CurrentDrawables[(int)slot] = model;
			CurrentTextures[(int)slot] = texture;

			if (IsClothingVisible())
			{
				ApplyComponent(slot);
			}
		}
		else
		{
			ClothingIndexes[slot] = value;
			CurrentDrawables[(int)slot] = 0;
			CurrentTextures[(int)slot] = 0;

			if (IsClothingVisible())
			{
				ClearComponent(slot);
			}
		}

		UpdateHairStatus();
	}

	private void OnDeleteOutfit()
	{
		if (m_OutfitBeingEdited != -1)
		{
			NetworkEventSender.SendNetworkEvent_OutfitEditor_DeleteOutfit(m_OutfitBeingEdited);
		}

		ReRequestOutfitList();
	}

	private void ApplyOutfitPreview(int outfitIndex)
	{
		CItemInstanceDef outfitItem = m_lstOutfits[outfitIndex];
		CItemValueOutfit itemValue = OwlJSON.DeserializeObject<CItemValueOutfit>(outfitItem.GetValueDataSerialized(), EJsonTrackableIdentifier.OutfitEditorApplyPreview3);
		g_strOutfitName = itemValue.Name;
		m_bHairVisible = !itemValue.HideHair;

		ClothingIndexes.Clear();
		foreach (var kvPair in itemValue.Clothes)
		{
			ECustomClothingComponent component = (ECustomClothingComponent)kvPair.Key;

			bool bFound = false;
			if (kvPair.Value != -1)
			{
				int index = 0;
				foreach (CItemInstanceDef clothingItem in dictClothing[component])
				{
					if (clothingItem.DatabaseID == kvPair.Value)
					{
						CItemValueClothingCustom clothingItemValue = OwlJSON.DeserializeObject<CItemValueClothingCustom>(clothingItem.GetValueDataSerialized(), EJsonTrackableIdentifier.OutfitEditorApplyPreview2);
						bFound = true;
						ClothingIndexes.Add(component, index);
						CurrentDrawables[(int)component] = clothingItemValue.Model;
						CurrentTextures[(int)component] = clothingItemValue.Texture;
					}

					++index;
				}
			}

			if (!bFound) // was -1, or item was missing
			{
				ClothingIndexes.Add(component, -1);
				CurrentDrawables[(int)component] = 0;
				CurrentTextures[(int)component] = 0;

				// do we need to handle naked state?
				// now give them a naked state for their gender
				EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
				if (gender == EGender.Male)
				{
					if (component == ECustomClothingComponent.Torsos)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Torsos] = 15;
						CurrentTextures[(int)ECustomClothingComponent.Torsos] = 0;
					}
					else if (component == ECustomClothingComponent.Legs)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Legs] = 21;
						CurrentTextures[(int)ECustomClothingComponent.Legs] = 0;
					}
					else if (component == ECustomClothingComponent.Tops)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Tops] = 15;
						CurrentTextures[(int)ECustomClothingComponent.Tops] = 0;
					}
					else if (component == ECustomClothingComponent.Undershirts)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Undershirts] = 15;
						CurrentTextures[(int)ECustomClothingComponent.Undershirts] = 0;
					}
					else if (component == ECustomClothingComponent.Shoes)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Shoes] = 34;
						CurrentTextures[(int)ECustomClothingComponent.Shoes] = 0;
					}
				}
				else
				{
					if (component == ECustomClothingComponent.Torsos)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Torsos] = 15;
						CurrentTextures[(int)ECustomClothingComponent.Torsos] = 0;
					}
					else if (component == ECustomClothingComponent.Legs)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Legs] = 56;
						CurrentTextures[(int)ECustomClothingComponent.Legs] = 0;
					}
					else if (component == ECustomClothingComponent.Tops)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Tops] = 15;
						CurrentTextures[(int)ECustomClothingComponent.Tops] = 0;
					}
					else if (component == ECustomClothingComponent.Undershirts)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Undershirts] = 15;
						CurrentTextures[(int)ECustomClothingComponent.Undershirts] = 0;
					}
					else if (component == ECustomClothingComponent.Shoes)
					{
						CurrentDrawables[(int)ECustomClothingComponent.Shoes] = 35;
						CurrentTextures[(int)ECustomClothingComponent.Shoes] = 0;
					}
				}

				ApplyComponent(ECustomClothingComponent.Torsos);
				ApplyComponent(ECustomClothingComponent.Legs);
				ApplyComponent(ECustomClothingComponent.Tops);
				ApplyComponent(ECustomClothingComponent.Undershirts);
				ApplyComponent(ECustomClothingComponent.Shoes);
			}
			else
			{
				ApplyComponent(component);
			}

		}

		PropIndexes.Clear();
		foreach (var kvPair in itemValue.Props)
		{
			ECustomPropSlot slot = (ECustomPropSlot)kvPair.Key;

			bool bFound = false;
			if (kvPair.Value != -1)
			{
				int index = 0;
				foreach (CItemInstanceDef propItem in dictProps[slot])
				{
					if (propItem.DatabaseID == kvPair.Value)
					{
						CItemValueClothingCustom clothingItemValue = OwlJSON.DeserializeObject<CItemValueClothingCustom>(propItem.GetValueDataSerialized(), EJsonTrackableIdentifier.OutfitEditorApplyPreview4);
						bFound = true;
						PropIndexes.Add(slot, index);
						CurrentPropDrawables[slot] = clothingItemValue.Model;
						CurrentPropTextures[slot] = clothingItemValue.Texture;
					}

					++index;
				}
			}

			if (!bFound) // was -1, or item was missing
			{
				PropIndexes.Add(slot, -1);
				CurrentPropDrawables[slot] = 0;
				CurrentPropTextures[slot] = 0;

				// do we need to handle naked state?

			}
			else
			{
				ApplyProp(slot);
			}

			ApplyProp(slot);
		}

		UpdateHairStatus();
	}

	private void OnMouseEnterOutfitItem(string strElementName, int outfitIndex)
	{
		ResetClothingAndPropsDicts();

		if (outfitIndex < m_lstOutfits.Count)
		{
			ApplyOutfitPreview(outfitIndex);
		}
	}

	private void OnEditOutfit(string strElementName, int outfitIndex)
	{
		if (outfitIndex < m_lstOutfits.Count)
		{
			OnMouseEnterOutfitItem(strElementName, outfitIndex);

			CItemInstanceDef outfitItem = m_lstOutfits[outfitIndex];
			GotoEditor(false, outfitItem.DatabaseID);
		}
	}

	private void ResetClothingAndPropsDicts()
	{
		dictClothing.Clear();
		foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
		{
			dictClothing.Add(component, new List<CItemInstanceDef>());
		}

		dictProps.Clear();
		foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
		{
			dictProps.Add(prop, new List<CItemInstanceDef>());
		}

		foreach (CItemInstanceDef itemDef in m_lstClothing)
		{
			if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_FACE)
			{
				// nothing goes in face it seems, only beards
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_MASK)
			{
				dictClothing[ECustomClothingComponent.Masks].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_HAIR)
			{
				dictClothing[ECustomClothingComponent.HairStyles].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_TORSO)
			{
				dictClothing[ECustomClothingComponent.Torsos].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_LEGS)
			{
				dictClothing[ECustomClothingComponent.Legs].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_BACK)
			{
				dictClothing[ECustomClothingComponent.BagsAndParachutes].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_FEET)
			{
				dictClothing[ECustomClothingComponent.Shoes].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_ACCESSORY)
			{
				dictClothing[ECustomClothingComponent.Accessories].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_UNDERSHIRT)
			{
				dictClothing[ECustomClothingComponent.Undershirts].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_BODYARMOR)
			{
				dictClothing[ECustomClothingComponent.BodyArmor].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_DECALS)
			{
				dictClothing[ECustomClothingComponent.Decals].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_TOPS)
			{
				dictClothing[ECustomClothingComponent.Tops].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_HELMET)
			{
				dictProps[ECustomPropSlot.Hats].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_GLASSES)
			{
				dictProps[ECustomPropSlot.Glasses].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_EARRINGS)
			{
				dictProps[ECustomPropSlot.Ears].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_WATCHES)
			{
				dictProps[ECustomPropSlot.Watches].Add(itemDef);
			}
			else if (itemDef.ItemID == EItemID.CLOTHES_CUSTOM_BRACELETS)
			{
				dictProps[ECustomPropSlot.Bracelets].Add(itemDef);
			}
		}
	}

	private void GotoEditor(bool bResetOutfitData, EntityDatabaseID outfitBeingEdited)
	{
		m_OutfitBeingEdited = outfitBeingEdited;

		if (bResetOutfitData)
		{
			ResetClothingAndPropsDicts();

			g_strOutfitName = "Outfit";
			m_bHairVisible = true;

			ClothingIndexes.Clear();
			foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
			{
				ClothingIndexes.Add(component, -1);
				CurrentDrawables[(int)component] = 0;
				CurrentTextures[(int)component] = 0;
			}

			PropIndexes.Clear();
			foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
			{
				PropIndexes.Add(prop, -1);
				CurrentPropDrawables[prop] = 0;
				CurrentPropTextures[prop] = 0;
			}

			// reset?
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

			// now give them a naked state for their gender
			EGender gender = DataHelper.GetLocalPlayerEntityData<EGender>(EDataNames.GENDER);
			if (gender == EGender.Male)
			{
				CurrentDrawables[(int)ECustomClothingComponent.Torsos] = 15;
				CurrentTextures[(int)ECustomClothingComponent.Torsos] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Legs] = 21;
				CurrentTextures[(int)ECustomClothingComponent.Legs] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Tops] = 15;
				CurrentTextures[(int)ECustomClothingComponent.Tops] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Undershirts] = 15;
				CurrentTextures[(int)ECustomClothingComponent.Undershirts] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Shoes] = 34;
				CurrentTextures[(int)ECustomClothingComponent.Shoes] = 0;
			}
			else
			{
				CurrentDrawables[(int)ECustomClothingComponent.Torsos] = 15;
				CurrentTextures[(int)ECustomClothingComponent.Torsos] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Legs] = 56;
				CurrentTextures[(int)ECustomClothingComponent.Legs] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Tops] = 15;
				CurrentTextures[(int)ECustomClothingComponent.Tops] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Undershirts] = 15;
				CurrentTextures[(int)ECustomClothingComponent.Undershirts] = 0;

				CurrentDrawables[(int)ECustomClothingComponent.Shoes] = 35;
				CurrentTextures[(int)ECustomClothingComponent.Shoes] = 0;
			}

			ApplyComponent(ECustomClothingComponent.Torsos);
			ApplyComponent(ECustomClothingComponent.Legs);
			ApplyComponent(ECustomClothingComponent.Tops);
			ApplyComponent(ECustomClothingComponent.Undershirts);
			ApplyComponent(ECustomClothingComponent.Shoes);
		}

		m_State = EOutfitEditorUIState.Editor;
		m_UI.Reset();
		m_UI.SetConfirmButtonText("Save Outfit");
		m_UI.SetExitButtonText("Exit Without Saving");

		// name
		m_UI.AddTabContent_Textbox("Outfit Name", "Give your outfit a name so you can identify it later.", 16, false, UIEventID.OutfitEditor_SetOutfitName, g_strOutfitName);

		// hide hair
		m_UI.AddTabContent_TwoRadioOptions("Hair", "This option lets you hide your hair if it interferes with a certain clothing item.", "Visible", "Hidden", UIEventID.OutfitEditor_SetHairVisible, m_bHairVisible ? 0 : 1);

		// TODO_OUTFITS: Masks - support masks functioning as beards
		// 0 for all is off
		if (dictProps[ECustomPropSlot.Hats].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Hat", Helpers.FormatString("{0} to {1}", 0, (uint)dictProps[ECustomPropSlot.Hats].Count), 0, (uint)dictProps[ECustomPropSlot.Hats].Count, (uint)PropIndexes[ECustomPropSlot.Hats] + 1, UIEventID.OutfitEditor_SetHatIndex);
		}

		if (dictClothing[ECustomClothingComponent.Masks].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Masks", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Masks].Count), 0, (uint)dictClothing[ECustomClothingComponent.Masks].Count, (uint)ClothingIndexes[ECustomClothingComponent.Masks] + 1, UIEventID.OutfitEditor_SetMaskIndex);
		}

		if (dictProps[ECustomPropSlot.Glasses].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Glasses", Helpers.FormatString("{0} to {1}", 0, (uint)dictProps[ECustomPropSlot.Glasses].Count), 0, (uint)dictProps[ECustomPropSlot.Glasses].Count, (uint)PropIndexes[ECustomPropSlot.Glasses] + 1, UIEventID.OutfitEditor_SetGlassesIndex);
		}

		if (dictProps[ECustomPropSlot.Ears].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Earrings", Helpers.FormatString("{0} to {1}", 0, (uint)dictProps[ECustomPropSlot.Ears].Count), 0, (uint)dictProps[ECustomPropSlot.Ears].Count, (uint)PropIndexes[ECustomPropSlot.Ears] + 1, UIEventID.OutfitEditor_SetEarsIndex);
		}

		if (dictClothing[ECustomClothingComponent.Torsos].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Torsos", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Torsos].Count), 0, (uint)dictClothing[ECustomClothingComponent.Torsos].Count, (uint)ClothingIndexes[ECustomClothingComponent.Torsos] + 1, UIEventID.OutfitEditor_SetTorsoIndex);
		}

		if (dictClothing[ECustomClothingComponent.Tops].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Tops", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Tops].Count), 0, (uint)dictClothing[ECustomClothingComponent.Tops].Count, (uint)ClothingIndexes[ECustomClothingComponent.Tops] + 1, UIEventID.OutfitEditor_SetTopsIndex);
		}

		if (dictClothing[ECustomClothingComponent.Undershirts].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Undershirts", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Undershirts].Count), 0, (uint)dictClothing[ECustomClothingComponent.Undershirts].Count, (uint)ClothingIndexes[ECustomClothingComponent.Undershirts] + 1, UIEventID.OutfitEditor_SetUndershirtsIndex);
		}

		if (dictClothing[ECustomClothingComponent.BagsAndParachutes].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Bags & Parachutes", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.BagsAndParachutes].Count), 0, (uint)dictClothing[ECustomClothingComponent.BagsAndParachutes].Count, (uint)ClothingIndexes[ECustomClothingComponent.BagsAndParachutes] + 1, UIEventID.OutfitEditor_SetBagsAndParachutesIndex);
		}

		if (dictClothing[ECustomClothingComponent.Accessories].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Accessories", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Accessories].Count), 0, (uint)dictClothing[ECustomClothingComponent.Accessories].Count, (uint)ClothingIndexes[ECustomClothingComponent.Accessories] + 1, UIEventID.OutfitEditor_SetAccessoriesIndex);
		}

		if (dictProps[ECustomPropSlot.Watches].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Watches", Helpers.FormatString("{0} to {1}", 0, (uint)dictProps[ECustomPropSlot.Watches].Count), 0, (uint)dictProps[ECustomPropSlot.Watches].Count, (uint)PropIndexes[ECustomPropSlot.Watches] + 1, UIEventID.OutfitEditor_SetWatchesIndex);
		}

		if (dictProps[ECustomPropSlot.Bracelets].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Bracelets", Helpers.FormatString("{0} to {1}", 0, (uint)dictProps[ECustomPropSlot.Bracelets].Count), 0, (uint)dictProps[ECustomPropSlot.Bracelets].Count, (uint)PropIndexes[ECustomPropSlot.Bracelets] + 1, UIEventID.OutfitEditor_SetBraceletsIndex);
		}

		if (dictClothing[ECustomClothingComponent.Decals].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Decals", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Decals].Count), 0, (uint)dictClothing[ECustomClothingComponent.Decals].Count, (uint)ClothingIndexes[ECustomClothingComponent.Decals] + 1, UIEventID.OutfitEditor_SetDecalsIndex);
		}

		if (dictClothing[ECustomClothingComponent.Legs].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Legs", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Legs].Count), 0, (uint)dictClothing[ECustomClothingComponent.Legs].Count, (uint)ClothingIndexes[ECustomClothingComponent.Legs] + 1, UIEventID.OutfitEditor_SetLegsIndex);
		}

		if (dictClothing[ECustomClothingComponent.Shoes].Count > 0)
		{
			m_UI.AddTabContent_NumberSelector("Shoes", Helpers.FormatString("{0} to {1}", 0, (uint)dictClothing[ECustomClothingComponent.Shoes].Count), 0, (uint)dictClothing[ECustomClothingComponent.Shoes].Count, (uint)ClothingIndexes[ECustomClothingComponent.Shoes] + 1, UIEventID.OutfitEditor_SetShoesIndex);
		}

		// delete button if its not a new outfit
		if (m_OutfitBeingEdited != -1)
		{
			m_UI.AddTabContent_GenericButton("Delete Outfit", UIEventID.OutfitEditor_DeleteOutfit);
		}

		UpdateHairStatus();
	}

	private void GotoList()
	{
		m_State = EOutfitEditorUIState.List;
		m_UI.Reset();
		m_UI.SetConfirmButtonText("Create New Outfit");
		m_UI.SetExitButtonText("Exit");

		int index = 0;
		foreach (CItemInstanceDef outfitItem in m_lstOutfits)
		{
			CItemValueOutfit itemValue = OwlJSON.DeserializeObject<CItemValueOutfit>(outfitItem.GetValueDataSerialized(), EJsonTrackableIdentifier.OutfitEditorGotoList);

			int itemsInOutfit = 0;
			foreach (var kvPair in itemValue.Clothes)
			{
				if (kvPair.Value != -1)
				{
					++itemsInOutfit;
				}
			}

			foreach (var kvPair in itemValue.Props)
			{
				if (kvPair.Value != -1)
				{
					++itemsInOutfit;
				}
			}

			m_UI.AddTabContent_GenericListItem(itemValue.Name, Helpers.FormatString("Contains {0} {1}", itemsInOutfit, itemsInOutfit == 1 ? "item" : "items"), "Click to Edit", UIEventID.OutfitEditor_EditOutfit, UIEventID.OutfitEditor_Outfit_OnMouseEnter, UIEventID.OutfitEditor_Outfit_OnMouseExit, index);
			++index;
		}
	}

	private bool OnPreFinish()
	{
		if (m_State == EOutfitEditorUIState.List) // finish button is actually create new outfit in this state
		{
			GotoEditor(true, -1);
		}
		else if (m_State == EOutfitEditorUIState.Editor) // finish button is actually save outfit
		{
			if (g_strOutfitName.Length > 0)
			{
				// get the item id's for our selections
				Dictionary<int, EntityDatabaseID> ClothingItemIDs = new Dictionary<int, EntityDatabaseID>();
				Dictionary<int, EntityDatabaseID> PropItemIDs = new Dictionary<int, EntityDatabaseID>();

				foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
				{
					int value = ClothingIndexes.ContainsKey(component) ? ClothingIndexes[component] : -1;
					if (value != -1)
					{
						CItemInstanceDef item = dictClothing[component][value];
						ClothingItemIDs[(int)component] = item.DatabaseID;
					}
					else
					{
						ClothingItemIDs[(int)component] = -1;
					}
				}

				foreach (ECustomPropSlot slot in Enum.GetValues(typeof(ECustomPropSlot)))
				{
					int value = PropIndexes.ContainsKey(slot) ? PropIndexes[slot] : -1;
					if (value != -1)
					{
						CItemInstanceDef item = dictProps[slot][value];
						PropItemIDs[(int)slot] = item.DatabaseID;
					}
					else
					{
						PropItemIDs[(int)slot] = -1;
					}
				}

				NetworkEventSender.SendNetworkEvent_OutfitEditor_CreateOrUpdateOutfit(g_strOutfitName, ClothingItemIDs, PropItemIDs, m_OutfitBeingEdited, !m_bHairVisible);

				ReRequestOutfitList();
			}
			else
			{
				NotificationManager.ShowNotification("Edit Outfit", "Outfit must have a name.", ENotificationIcon.ExclamationSign);
			}

		}

		return false;
	}

	private bool OnPreExit()
	{
		if (m_State == EOutfitEditorUIState.List) // exit button is actually exit
		{
			return true;
		}
		else if (m_State == EOutfitEditorUIState.Editor) // exit button is actually exit outfit editor without saving
		{
			ReRequestOutfitList();
			return false;
		}

		return false;
	}

	private void OnFinish()
	{

	}

	private void ReRequestOutfitList()
	{
		NetworkEventSender.SendNetworkEvent_RequestOutfitList();
		m_UI.SetVisible(false, false, true);
	}
}
// TODO_OUTFITS: Why do we have earrings 1 etc when its actually zero? is it because we give them a blank item for none?