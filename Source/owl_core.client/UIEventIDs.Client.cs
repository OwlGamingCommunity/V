using System;
public enum UIEventID
{
	AcceptPDTicket,
	AchievementOverlay_OnFadedOut,
	ActivityInteraction_Dropdown_Cancel,
	ActivityInteraction_Dropdown_Done,
	ActivityInteraction_Dropdown_DropdownSelectionChanged,
	AdminEntityDeleteConfirmation_No,
	AdminEntityDeleteConfirmation_Yes,
	AdminViewFactions_DeleteFaction,
	AdminViewFactions_JoinFaction,
	AdminViewFaction_Delete_No,
	AdminViewFaction_Delete_Yes,
	AdminViewFaction_Join_No,
	AdminViewFaction_Join_Yes,
	AnswerCall,
	ApproveApplication,
	AudioPlayFinished,
	Banking_OnDeposit,
	Banking_OnHide,
	Banking_OnSwitchCredit,
	Banking_OnWireTransfer,
	Banking_OnWithdraw,
	Banking_PayDownDebt,
	BarberShop_Checkout,
	BarberShop_Exit,
	BarberShop_GetPriceDetails,
	BarberShop_OnRootChanged_FullBeards,
	BarberShop_SetBaseHair,
	BarberShop_SetChestHair,
	BarberShop_SetChestHairColor,
	BarberShop_SetChestHairColorHighlights,
	BarberShop_SetChestHairOpacity,
	BarberShop_SetComponentDrawable_FullBeards,
	BarberShop_SetComponentTexture_FullBeards,
	BarberShop_SetFacialHair,
	BarberShop_SetFacialHairColor,
	BarberShop_SetFacialHairColorHighlights,
	BarberShop_SetFacialHairOpacity,
	BarberShop_SetHairColor,
	BarberShop_SetHairColorHighlights,
	BarberShop_SetHairStyleDrawable,
	BarberShop_UpdateChestHairOpacity,
	BarberShop_UpdateFacialHairOpacity,
	Blackjack_Action_HitMe,
	Blackjack_Action_Stick,
	Blackjack_PlaceBet_Cancel,
	Blackjack_PlaceBet_Submit,
	BoomBox_OnChangeRadio_Confirm,
	CallNumber,
	CallTaxi,
	CancelAdminReport,
	CancelCall,
	CancelEditRadio,
	CancelGoingOnDuty,
	CancelTaxi,
	CasinoManagement_AddCurrency,
	CasinoManagement_Add_Cancel,
	CasinoManagement_Add_Submit,
	CasinoManagement_Exit,
	CasinoManagement_TakeCurrency,
	CasinoManagement_Take_Cancel,
	CasinoManagement_Take_Submit,
	ChangeCharacterRequested,
	ChangeFarePerMile,
	CharacterLook_Close,
	CharCreate_CreateCharacter,
	CharCreate_DismissError,
	CharCreate_GotoBodyCam_Far,
	CharCreate_GotoBodyCam_Near,
	CharCreate_GotoHeadCam,
	CharCreate_OfferCreateCharacter_Cancel,
	CharCreate_OfferCreateCharacter_Create,
	CharCreate_OnChangeTab_Custom,
	CharCreate_OnChangeTab_Premade,
	CharCreate_OnRootChanged_Accessories,
	CharCreate_OnRootChanged_Bracelets,
	CharCreate_OnRootChanged_Decals,
	CharCreate_OnRootChanged_Earrings,
	CharCreate_OnRootChanged_FullBeards,
	CharCreate_OnRootChanged_Glasses,
	CharCreate_OnRootChanged_Hats,
	CharCreate_OnRootChanged_Legs,
	CharCreate_OnRootChanged_Shoes,
	CharCreate_OnRootChanged_Tops,
	CharCreate_OnRootChanged_Torso,
	CharCreate_OnRootChanged_Undershirts,
	CharCreate_OnRootChanged_Watches,
	CharCreate_ResetRotation,
	CharCreate_SetAge,
	CharCreate_SetAgeing,
	CharCreate_SetAgeingOpacity,
	CharCreate_SetBaseHair,
	CharCreate_SetBlemishes,
	CharCreate_SetBlemishesOpacity,
	CharCreate_SetBlush,
	CharCreate_SetBlushColor,
	CharCreate_SetBlushColorHighlights,
	CharCreate_SetBlushOpacity,
	CharCreate_SetBodyBlemishes,
	CharCreate_SetBodyBlemishesOpacity,
	CharCreate_SetCharacterName,
	CharCreate_SetCheekboneHeight,
	CharCreate_SetCheekWidth,
	CharCreate_SetCheekWidthLower,
	CharCreate_SetChestHair,
	CharCreate_SetChestHairColor,
	CharCreate_SetChestHairColorHighlights,
	CharCreate_SetChestHairOpacity,
	CharCreate_SetChinEffect,
	CharCreate_SetChinSize,
	CharCreate_SetChinSizeUnderneath,
	CharCreate_SetChinWidth,
	CharCreate_SetComplexion,
	CharCreate_SetComplexionOpacity,
	CharCreate_SetComponentDrawable_Accessories,
	CharCreate_SetComponentDrawable_Decals,
	CharCreate_SetComponentDrawable_FullBeards,
	CharCreate_SetComponentDrawable_Legs,
	CharCreate_SetComponentDrawable_Shoes,
	CharCreate_SetComponentDrawable_Tops,
	CharCreate_SetComponentDrawable_Torso,
	CharCreate_SetComponentDrawable_Undershirts,
	CharCreate_SetComponentPalette_Accessories,
	CharCreate_SetComponentPalette_Decals,
	CharCreate_SetComponentPalette_Legs,
	CharCreate_SetComponentPalette_Shoes,
	CharCreate_SetComponentPalette_Tops,
	CharCreate_SetComponentPalette_Torso,
	CharCreate_SetComponentPalette_Undershirts,
	CharCreate_SetComponentTexture_Accessories,
	CharCreate_SetComponentTexture_Decals,
	CharCreate_SetComponentTexture_FullBeards,
	CharCreate_SetComponentTexture_Legs,
	CharCreate_SetComponentTexture_Shoes,
	CharCreate_SetComponentTexture_Tops,
	CharCreate_SetComponentTexture_Torso,
	CharCreate_SetComponentTexture_Undershirts,
	CharCreate_SetExtraBodyBlemishes,
	CharCreate_SetExtraBodyBlemishesOpacity,
	CharCreate_SetEyebrowDepth,
	CharCreate_SetEyebrowHeight,
	CharCreate_SetEyeBrows,
	CharCreate_SetEyeBrowsColor,
	CharCreate_SetEyeBrowsColorHighlights,
	CharCreate_SetEyeBrowsOpacity,
	CharCreate_SetEyeColor,
	CharCreate_SetEyeSize,
	CharCreate_SetFaceShape,
	CharCreate_SetFacialHair,
	CharCreate_SetFacialHairColor,
	CharCreate_SetFacialHairColorHighlights,
	CharCreate_SetFacialHairOpacity,
	CharCreate_SetGender,
	CharCreate_SetHairColor,
	CharCreate_SetHairColorHighlights,
	CharCreate_SetHairStyleDrawable,
	CharCreate_SetLanguage,
	CharCreate_SetLipSize,
	CharCreate_SetLipstick,
	CharCreate_SetLipstickColor,
	CharCreate_SetLipstickColorHighlights,
	CharCreate_SetLipstickOpacity,
	CharCreate_SetMakeup,
	CharCreate_SetMakeupColor,
	CharCreate_SetMakeupColorHighlights,
	CharCreate_SetMakeupOpacity,
	CharCreate_SetMolesFreckles,
	CharCreate_SetMolesFrecklesOpacity,
	CharCreate_SetMouthSize,
	CharCreate_SetMouthSizeLower,
	CharCreate_SetNeckWidth,
	CharCreate_SetNeckWidthLower,
	CharCreate_SetNoseAngle,
	CharCreate_SetNoseSizeHorizontal,
	CharCreate_SetNoseSizeOutwards,
	CharCreate_SetNoseSizeOutwardsLower,
	CharCreate_SetNoseSizeOutwardsUpper,
	CharCreate_SetNoseSizeVertical,
	CharCreate_SetPropDrawable_Bracelets,
	CharCreate_SetPropDrawable_Earrings,
	CharCreate_SetPropDrawable_Glasses,
	CharCreate_SetPropDrawable_Hats,
	CharCreate_SetPropDrawable_Watches,
	CharCreate_SetPropTexture_Bracelets,
	CharCreate_SetPropTexture_Earrings,
	CharCreate_SetPropTexture_Glasses,
	CharCreate_SetPropTexture_Hats,
	CharCreate_SetPropTexture_Watches,
	CharCreate_SetSecondLanguage,
	CharCreate_SetSkinID,
	CharCreate_SetSkinPercentage_Color,
	CharCreate_SetSkinPercentage_Shape,
	CharCreate_SetSpawn,
	CharCreate_SetSunDamage,
	CharCreate_SetSunDamageOpacity,
	CharCreate_SetType,
	CharCreate_StartRotation,
	CharCreate_StopRotation,
	CharCreate_Tattoo_AddNew,
	CharCreate_Tattoo_Cancel,
	CharCreate_Tattoo_ChangeTattoo,
	CharCreate_Tattoo_ChangeZone,
	CharCreate_Tattoo_Create,
	CharCreate_Tattoo_RemoveTattoo,
	CharCreate_ToggleClothes,
	ChipManagement_Buy_Cancel,
	ChipManagement_Buy_Submit,
	ChipManagement_Sell_Cancel,
	ChipManagement_Sell_Submit,
	CloseAnimationUI,
	CloseAssetTransfer,
	CloseCheck,
	CloseCheckInt,
	CloseCheckVeh,
	CloseDonations,
	CloseF10Menu,
	CloseItemsListUI,
	ClosePhone,
	CloseRadioManagement,
	CloseVehiclesListUI,
	ClothingStoreSelector_Exit,
	ClothingStoreSelector_GotoClothingStore,
	ClothingStoreSelector_GotoOutfitEditor,
	ClothingStore_Checkout,
	ClothingStore_Exit,
	ClothingStore_GetPriceDetails,
	ClothingStore_OnRootChanged_Accessories,
	ClothingStore_OnRootChanged_Bracelets,
	ClothingStore_OnRootChanged_Decals,
	ClothingStore_OnRootChanged_Earrings,
	ClothingStore_OnRootChanged_Glasses,
	ClothingStore_OnRootChanged_Hats,
	ClothingStore_OnRootChanged_Legs,
	ClothingStore_OnRootChanged_Masks,
	ClothingStore_OnRootChanged_Shoes,
	ClothingStore_OnRootChanged_Tops,
	ClothingStore_OnRootChanged_Torso,
	ClothingStore_OnRootChanged_Undershirts,
	ClothingStore_OnRootChanged_Watches,
	ClothingStore_SetClothing,
	ClothingStore_SetComponentDrawable_Accessories,
	ClothingStore_SetComponentDrawable_Decals,
	ClothingStore_SetComponentDrawable_Legs,
	ClothingStore_SetComponentDrawable_Masks,
	ClothingStore_SetComponentDrawable_Shoes,
	ClothingStore_SetComponentDrawable_Tops,
	ClothingStore_SetComponentDrawable_Torso,
	ClothingStore_SetComponentDrawable_Undershirts,
	ClothingStore_SetComponentTexture_Accessories,
	ClothingStore_SetComponentTexture_Decals,
	ClothingStore_SetComponentTexture_Legs,
	ClothingStore_SetComponentTexture_Masks,
	ClothingStore_SetComponentTexture_Shoes,
	ClothingStore_SetComponentTexture_Tops,
	ClothingStore_SetComponentTexture_Torso,
	ClothingStore_SetComponentTexture_Undershirts,
	ClothingStore_SetProp,
	ClothingStore_SetPropDrawable_Bracelets,
	ClothingStore_SetPropDrawable_Earrings,
	ClothingStore_SetPropDrawable_Glasses,
	ClothingStore_SetPropDrawable_Hats,
	ClothingStore_SetPropDrawable_Watches,
	ClothingStore_SetPropTexture_Bracelets,
	ClothingStore_SetPropTexture_Earrings,
	ClothingStore_SetPropTexture_Glasses,
	ClothingStore_SetPropTexture_Hats,
	ClothingStore_SetPropTexture_Watches,
	ClothingStore_SetSkinID,
	ConsumeDonationPerk,
	CopyToClipboardItemValue,
	CreateCustomAnimation,
	CreateFaction,
	CreateInfoMarker_Cancel,
	CreateInfoMarker_Submit,
	CreateKeybind,
	CreatePhoneMessage,
	CustomAnimationDeletion_No,
	CustomAnimationDeletion_Yes,
	CustomInterior_CloseWindow,
	CustomInterior_Confirmation_No,
	CustomInterior_Confirmation_Yes,
	CustomInterior_ProcessCustomInterior,
	DeclinePDTicket,
	DeleteCustomAnimCmd,
	DeleteKeybind,
	DenyApplication,
	DisbandFaction,
	DiscordGetURLHack,
	DoLogin,
	Donation_ChangeInactivityEntity,
	Donation_ChangeInactivityLength,
	Donation_ChangeInactivityType,
	DoRegister,
	Dummy,
	DutyOutfitEditor_DeleteOutfit,
	DutyOutfitEditor_EditOutfit,
	DutyOutfitEditor_Loadout_SetAccessory1,
	DutyOutfitEditor_Loadout_SetAccessory2,
	DutyOutfitEditor_Loadout_SetAccessory3,
	DutyOutfitEditor_Loadout_SetHandgun1,
	DutyOutfitEditor_Loadout_SetHandgun2,
	DutyOutfitEditor_Loadout_SetLargeCarriedItem,
	DutyOutfitEditor_Loadout_SetLargeWeapon,
	DutyOutfitEditor_Loadout_SetMelee,
	DutyOutfitEditor_Loadout_SetProjectile,
	DutyOutfitEditor_Loadout_SetProjectile2,
	DutyOutfitEditor_Loadout_SetPursuitAccessory,
	DutyOutfitEditor_Outfit_OnMouseEnter,
	DutyOutfitEditor_Outfit_OnMouseExit,
	DutyOutfitEditor_SelectPreset_Cancel,
	DutyOutfitEditor_SelectPreset_Done,
	DutyOutfitEditor_SelectPreset_DropdownSelectionChanged,
	DutyOutfitEditor_SetAccessory,
	DutyOutfitEditor_SetBagsAndParachutes,
	DutyOutfitEditor_SetBodyArmor,
	DutyOutfitEditor_SetBracelets,
	DutyOutfitEditor_SetDecals,
	DutyOutfitEditor_SetEars,
	DutyOutfitEditor_SetGlasses,
	DutyOutfitEditor_SetHairVisible,
	DutyOutfitEditor_SetHat,
	DutyOutfitEditor_SetLegs,
	DutyOutfitEditor_SetMask,
	DutyOutfitEditor_SetOutfitName,
	DutyOutfitEditor_SetShoes,
	DutyOutfitEditor_SetTop,
	DutyOutfitEditor_SetTorso,
	DutyOutfitEditor_SetUndershirt,
	DutyOutfitEditor_SetWatches,
	Duty_GotoOutfits,
	Duty_SelectOutfit_DropdownSelectionChanged,
	Duty_SelectOutfit_DropdownSelectionHoverChanged,
	EditFactionMessage,
	EditInterior_Hide,
	EditInterior_OnChangeClass,
	EditInterior_OnChangeCurrentFurnitureItem,
	EditInterior_OnChangeFurnitureItem,
	EditInterior_OnChangeRemovedFurnitureItem,
	EditInterior_PickupFurniture,
	EditInterior_PlaceFurniture,
	EditInterior_RestoreFurniture,
	EditRadio,
	EndCall,
	ExitAchievements,
	ExitCharCreate,
	ExitPendingAppsScreen,
	ExpandContainer,
	ExtendRadio30Days,
	ExtendRadio7Days,
	FactionInvite_Accept,
	FactionInvite_Decline,
	FactionVehicleList_Hide,
	FadeOutWeaponSelector,
	FinalizeLicenseDevice,
	FindPlayerForReport,
	FindPlayerForReportResult,
	FurnitureStore_Hide,
	FurnitureStore_OnChangeClass,
	FurnitureStore_OnChangeFurnitureItem,
	FurnitureStore_OnCheckout,
	FurnitureStore_ResetCamera,
	FurnitureStore_StartRotation,
	FurnitureStore_StartZoom,
	FurnitureStore_StopRotation,
	FurnitureStore_StopZoom,
	GangTagCreator_AddLayer_Rectangle,
	GangTagCreator_AddLayer_Sprite,
	GangTagCreator_AddLayer_Text,
	GangTagCreator_DeleteLayer,
	GangTagCreator_EditLayer,
	GangTagCreator_EditLayer_ChangeSprite,
	GangTagCreator_EditLayer_SetAlpha,
	GangTagCreator_EditLayer_SetColor,
	GangTagCreator_EditLayer_SetFontID,
	GangTagCreator_EditLayer_SetHeight,
	GangTagCreator_EditLayer_SetOutline,
	GangTagCreator_EditLayer_SetScale,
	GangTagCreator_EditLayer_SetShadow,
	GangTagCreator_EditLayer_SetSpriteRotation,
	GangTagCreator_EditLayer_SetText,
	GangTagCreator_EditLayer_SetWidth,
	GangTagCreator_EditLayer_SetXCoordinate,
	GangTagCreator_EditLayer_SetYCoordinate,
	GangTagCreator_Exit_Discard,
	GangTagCreator_Exit_KeepWIP,
	GangTagCreator_Exit_Save,
	GangTagCreator_MoveLayerDown,
	GangTagCreator_MoveLayerUp,
	GangTag_AcceptShare,
	GangTag_DeclineShare,
	GangTag_EditSavedTag,
	GangTag_EditWIPTag,
	GenericCharacterCustomization_DismissError,
	GenericCharacterCustomization_Exit,
	GenericCharacterCustomization_Finish,
	GenericCharacterCustomization_GotoFarCamEvent,
	GenericCharacterCustomization_GotoHeadCamEvent,
	GenericCharacterCustomization_GotoNearCamEvent,
	GenericCharacterCustomization_ResetRotationEvent,
	GenericCharacterCustomization_StartRotationEvent,
	GenericCharacterCustomization_StopRotationEvent,
	GenericCharacterCustomization_ToggleClothes,
	GenericDataTable_OnClose,
	GenericDropdown_OnClose,
	GenericListBox_OnClose,
	GenericMessageBox_OnClose,
	GenericProgressBar_OnClose,
	GenericPrompt3_OnClose,
	GenericPrompt_OnClose,
	GenericUserLoginBox_OnClose,
	Generic_CloseGenericsUI,
	Generic_CloseItemMoverUI,
	Generic_SpawnGenerics,
	Generic_UpdateGenericPosition,
	Generic_UpdateGenericPreviewPosition,
	GetPendingApplications,
	GetPhoneContactByNumber,
	GetPhoneContacts,
	GetPhoneMessagesContacts,
	GetPhoneMessagesFromNumber,
	GetTotalUnviewedMessages,
	GoBackToRegister,
	GoOnDuty,
	GotoCreateCharacter,
	GotoDiscordLinking,
	GotoDonations,
	GotoEditInterior,
	GotoFullScreenBrowser,
	GotoLanguages,
	GotoRegisterPressed,
	GotoSplitItem,
	GotoViewAchievements,
	GotRadioMessage,
	HelpRequestCommands,
	HideChatSettings,
	HideControlManager,
	HideCreateFaction,
	HideHelpCenter,
	HideHudMenu,
	HideLicenseDevice,
	HideMDCUIs,
	HidePurchasePropertyUI,
	HideRegisterSuccessMessageBox,
	HideStore,
	IncomingDutyOutfitShare_Accept,
	IncomingDutyOutfitShare_Decline,
	InfoMarkerOwned_Delete,
	InfoMarkerOwned_Exit,
	InfoMarkerOwned_Read,
	Inventory_CloseWindow,
	Inventory_GoBack,
	InviteFactionPlayer,
	KickFactionMember,
	LanguageMenu_Close,
	LanguageMenu_SelectLanguage,
	LeaveFaction,
	Logout,
	MapLoader_CancelUpload,
	Marijuana_Exit,
	Marijuana_GetSeeds,
	Marijuana_SellDrugs,
	MdcGotoPerson,
	MdcGotoProperty,
	MdcGotoVehicle,
	OnChangeFarePerMile_Cancel,
	OnChangeFarePerMile_Submit,
	OnChatBoxCommand,
	OnChatBoxMessage,
	OnChatInputVisibleChanged,
	OnDestroyItem,
	OnDestroyItem_Cancel,
	OnDestroyItem_Confirm,
	OnDisconnectedFromServer,
	OnDropItem,
	OnDutyOutfitShare,
	OnFactionDisband_Cancel,
	OnFactionDisband_Confirm,
	OnFactionLeave_Cancel,
	OnFactionLeave_Confirm,
	OnFriskTakeItem,
	OnHideFrisk,
	OnHideImpoundedVehicle,
	OnHTMLLoaded,
	OnLocksmithRequestChoose_Property,
	OnLocksmithRequestChoose_Vehicle,
	OnLocksmithRequest_Cancel,
	OnLocksmithRequest_Submit,
	OnMergeItem,
	OnMoveItem,
	OnNamePet_Cancel,
	OnNamePet_Submit,
	OnPressPhoneButton,
	OnRetuneRadio_Cancel,
	OnRetuneRadio_Submit,
	OnShareGangTag_Cancel,
	OnShareGangTag_Submit,
	OnShowItem,
	OnSpawnSelector_LS,
	OnSpawnSelector_Paleto,
	OnSwitchAccount,
	OnUnlinkDiscord_Cancel,
	OnUnlinkDiscord_Confirm,
	OnUseItem,
	OnWatchNewTutorial_Cancel,
	OnWatchNewTutorial_Confirm_All,
	OnWatchNewTutorial_Confirm_New,
	OnWireTransferKeyboard_Cancel,
	OnWireTransferKeyboard_Submit,
	OpenMobileBankingUI,
	OpenRadioManager,
	OpenTransferAssets,
	OutfitEditor_DeleteOutfit,
	OutfitEditor_EditOutfit,
	OutfitEditor_Outfit_OnMouseEnter,
	OutfitEditor_Outfit_OnMouseExit,
	OutfitEditor_SetAccessoriesIndex,
	OutfitEditor_SetBagsAndParachutesIndex,
	OutfitEditor_SetBraceletsIndex,
	OutfitEditor_SetDecalsIndex,
	OutfitEditor_SetEarsIndex,
	OutfitEditor_SetGlassesIndex,
	OutfitEditor_SetHairVisible,
	OutfitEditor_SetHatIndex,
	OutfitEditor_SetLegsIndex,
	OutfitEditor_SetMaskIndex,
	OutfitEditor_SetOutfitName,
	OutfitEditor_SetShoesIndex,
	OutfitEditor_SetTopsIndex,
	OutfitEditor_SetTorsoIndex,
	OutfitEditor_SetUndershirtsIndex,
	OutfitEditor_SetWatchesIndex,
	PaydayOverview_OnClose,
	PDHelicopterHUD_ToggleMovingVehiclesOnly,
	PDHelicopterHUD_ToggleNVG,
	PDHelicopterHUD_TogglePeople,
	PDHelicopterHUD_ToggleThermal,
	PDHelicopterHUD_ToggleVehiclesOccupied,
	PDHelicopterHUD_ToggleVehiclesUnoccupied,
	PersistentNotifications_Dismissed,
	PlasticSurgeonOfferCharacterUpgrade_Confirm,
	PlasticSurgeonOfferCharacterUpgrade_Decline,
	PlasticSurgeon_SetAgeing,
	PlasticSurgeon_SetAgeingOpacity,
	PlasticSurgeon_SetAgeingOpacity_Finalize,
	PlasticSurgeon_SetBlemishes,
	PlasticSurgeon_SetBlemishesOpacity,
	PlasticSurgeon_SetBlemishesOpacity_Finalize,
	PlasticSurgeon_SetBlush,
	PlasticSurgeon_SetBlushColor,
	PlasticSurgeon_SetBlushColorHighlights,
	PlasticSurgeon_SetBlushOpacity,
	PlasticSurgeon_SetBlushOpacity_Finalize,
	PlasticSurgeon_SetBodyBlemishes,
	PlasticSurgeon_SetBodyBlemishesOpacity,
	PlasticSurgeon_SetBodyBlemishesOpacity_Finalize,
	PlasticSurgeon_SetCheekboneHeight,
	PlasticSurgeon_SetCheekboneHeight_Finalize,
	PlasticSurgeon_SetCheekWidth,
	PlasticSurgeon_SetCheekWidthLower,
	PlasticSurgeon_SetCheekWidthLower_Finalize,
	PlasticSurgeon_SetCheekWidth_Finalize,
	PlasticSurgeon_SetChestHair,
	PlasticSurgeon_SetChestHairColor,
	PlasticSurgeon_SetChestHairColorHighlights,
	PlasticSurgeon_SetChestHairOpacity,
	PlasticSurgeon_SetChestHairOpacity_Finalize,
	PlasticSurgeon_SetChinEffect,
	PlasticSurgeon_SetChinEffect_Finalize,
	PlasticSurgeon_SetChinSize,
	PlasticSurgeon_SetChinSizeUnderneath,
	PlasticSurgeon_SetChinSizeUnderneath_Finalize,
	PlasticSurgeon_SetChinSize_Finalize,
	PlasticSurgeon_SetChinWidth,
	PlasticSurgeon_SetChinWidth_Finalize,
	PlasticSurgeon_SetComplexion,
	PlasticSurgeon_SetComplexionOpacity,
	PlasticSurgeon_SetComplexionOpacity_Finalize,
	PlasticSurgeon_SetEyebrowDepth,
	PlasticSurgeon_SetEyebrowDepth_Finalize,
	PlasticSurgeon_SetEyebrowHeight,
	PlasticSurgeon_SetEyebrowHeight_Finalize,
	PlasticSurgeon_SetEyeBrows,
	PlasticSurgeon_SetEyeBrowsColor,
	PlasticSurgeon_SetEyeBrowsColorHighlights,
	PlasticSurgeon_SetEyeBrowsOpacity,
	PlasticSurgeon_SetEyeBrowsOpacity_Finalize,
	PlasticSurgeon_SetEyeColor,
	PlasticSurgeon_SetEyeSize,
	PlasticSurgeon_SetEyeSize_Finalize,
	PlasticSurgeon_SetFaceShape,
	PlasticSurgeon_SetLipSize,
	PlasticSurgeon_SetLipSize_Finalize,
	PlasticSurgeon_SetLipstick,
	PlasticSurgeon_SetLipstickColor,
	PlasticSurgeon_SetLipstickColorHighlights,
	PlasticSurgeon_SetLipstickOpacity,
	PlasticSurgeon_SetLipstickOpacity_Finalize,
	PlasticSurgeon_SetMakeup,
	PlasticSurgeon_SetMakeupColor,
	PlasticSurgeon_SetMakeupColorHighlights,
	PlasticSurgeon_SetMakeupOpacity,
	PlasticSurgeon_SetMakeupOpacity_Finalize,
	PlasticSurgeon_SetMolesFreckles,
	PlasticSurgeon_SetMolesFrecklesOpacity,
	PlasticSurgeon_SetMolesFrecklesOpacity_Finalize,
	PlasticSurgeon_SetMouthSize,
	PlasticSurgeon_SetMouthSizeLower,
	PlasticSurgeon_SetMouthSizeLower_Finalize,
	PlasticSurgeon_SetMouthSize_Finalize,
	PlasticSurgeon_SetNeckWidth,
	PlasticSurgeon_SetNeckWidthLower,
	PlasticSurgeon_SetNeckWidthLower_Finalize,
	PlasticSurgeon_SetNeckWidth_Finalize,
	PlasticSurgeon_SetNoseAngle,
	PlasticSurgeon_SetNoseAngle_Finalize,
	PlasticSurgeon_SetNoseSizeHorizontal,
	PlasticSurgeon_SetNoseSizeHorizontal_Finalize,
	PlasticSurgeon_SetNoseSizeOutwards,
	PlasticSurgeon_SetNoseSizeOutwardsLower,
	PlasticSurgeon_SetNoseSizeOutwardsLower_Finalize,
	PlasticSurgeon_SetNoseSizeOutwardsUpper,
	PlasticSurgeon_SetNoseSizeOutwardsUpper_Finalize,
	PlasticSurgeon_SetNoseSizeOutwards_Finalize,
	PlasticSurgeon_SetNoseSizeVertical,
	PlasticSurgeon_SetNoseSizeVertical_Finalize,
	PlasticSurgeon_SetSkinPercentage_Color,
	PlasticSurgeon_SetSkinPercentage_Color_Finalize,
	PlasticSurgeon_SetSkinPercentage_Shape,
	PlasticSurgeon_SetSkinPercentage_Shape_Finalize,
	PlasticSurgeon_SetSunDamage,
	PlasticSurgeon_SetSunDamageOpacity,
	PlasticSurgeon_SetSunDamageOpacity_Finalize,
	PlayAnimationFromUI,
	PlayerRadial_OnEnterItem,
	PlayerRadial_OnExitItem,
	PreviewCharacter,
	PurchaseDonationPerk,
	PurchaseGC,
	PurchaseInactivityProtection,
	PurchaseProperty_OnCheckout,
	PurchaseProperty_OnPreview,
	PurchaseProperty_SetMonthlyDownpayment,
	PurchaseProperty_SetNumMonthlyPayments,
	PurchaseRadio,
	QuizComplete,
	ReloadCheckIntData,
	ReloadCheckVehData,
	RemovePhoneContact,
	ReportBug,
	RequestApplicationDetails,
	RequestMergeItem,
	RequestMoveItem,
	RequestWrittenQuestions,
	ResetChatSettings,
	ResetFare,
	ResetSplitItem,
	RespawnFactionVehicles,
	RestartQuiz,
	RoadblockEditor_Hide,
	RoadblockEditor_PlaceRoadblock,
	RunPlate,
	SaveAdminNotes,
	SaveChatSettingsForTab,
	SaveChatSettingsGlobal,
	SaveControl,
	SaveInteriorNote,
	SavePhoneContact,
	SaveRadio,
	SaveRanksAndSalaries,
	SaveVehicleNote,
	ScooterRental_Close,
	ScooterRental_Rent,
	SendSMSNotification,
	SetAllControlsToDefault,
	SetAutoSpawn,
	SetMemberRank,
	Shard_OnFadedOut,
	ShareDutyOutfit_SelectPlayer_Cancel,
	ShareDutyOutfit_SelectPlayer_Done,
	ShareDutyOutfit_SelectPlayer_DropdownSelectionChanged,
	ShowChatSettings,
	ShowGenericMessageBox,
	ShowItemInfo,
	SplitItem,
	SprayCan_EditTag,
	SprayCan_Exit,
	SprayCan_ShareTag,
	SprayCan_TagWall,
	StoreCheckout,
	SubmitAdminReport,
	SubmitAssetTransfer,
	SubmitWrittenPortion,
	TattooArtist_AddNew,
	TattooArtist_Cancel,
	TattooArtist_ChangeTattoo,
	TattooArtist_ChangeZone,
	TattooArtist_Create,
	TattooArtist_OnMouseEnter,
	TattooArtist_OnMouseExit,
	TattooArtist_RemoveTattoo,
	ToggleAvailableForHire,
	ToggleFactionManager,
	ToggleLocalNametag,
	ToggleSilentSiren,
	ToggleSpeedTrap,
	UnimpoundVehicle,
	UpdateCharacterLook_Close,
	UpdateCharacterLook_Save,
	UpdateMessageViewed,
	UpdateStolenState,
	UserInput_OnClose,
	VehicleCrusher_Crush,
	VehicleCrusher_Exit,
	VehicleModShop_ChangeModCategory,
	VehicleModShop_OnChangeNeonsColor,
	VehicleModShop_OnCheckout,
	VehicleModShop_OnExit_Discard,
	VehicleModShop_ResetCamera,
	VehicleModShop_ResetPlate,
	VehicleModShop_SetPlateText,
	VehicleModShop_StartRotation,
	VehicleModShop_StartZoom,
	VehicleModShop_StopRotation,
	VehicleModShop_StopZoom,
	VehicleModShop_UpdateModIndex,
	VehicleModShop_UpdateNeonState,
	VehicleModShop_UpdatePrice,
	VehicleRentalStore_Hide,
	VehicleRentalStore_OnChangeClass,
	VehicleRentalStore_OnChangePrimaryColor,
	VehicleRentalStore_OnChangeRentalLength,
	VehicleRentalStore_OnChangeSecondaryColor,
	VehicleRentalStore_OnChangeVehicle,
	VehicleRentalStore_OnCheckout,
	VehicleRentalStore_ResetCamera,
	VehicleRentalStore_StartRotation,
	VehicleRentalStore_StartZoom,
	VehicleRentalStore_StopRotation,
	VehicleRentalStore_StopZoom,
	VehicleRentalStore_ToggleDoors,
	VehicleStore_Hide,
	VehicleStore_OnChangeClass,
	VehicleStore_OnChangePrimaryColor,
	VehicleStore_OnChangeSecondaryColor,
	VehicleStore_OnChangeVehicle,
	VehicleStore_OnCheckout,
	VehicleStore_ResetCamera,
	VehicleStore_SetMonthlyDownpayment,
	VehicleStore_SetNumMonthlyPayments,
	VehicleStore_StartRotation,
	VehicleStore_StartZoom,
	VehicleStore_StopRotation,
	VehicleStore_StopZoom,
	VehicleStore_ToggleDoors,
	ViewFactionVehicles,
}