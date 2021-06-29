using System;
public enum NetworkEventID
{
	ActivityRequestInteract,
	ActivityRequestInteract_Response,
	Activity_RoundOutcome,
	AdminCheck,
	AdminCheckInt,
	AdminCheckVeh,
	AdminClearGangTags,
	AdminConfirmEntityDelete,
	AdminDeleteElevator,
	AdminDeleteFaction,
	AdminDeleteProperty,
	AdminDeleteVehicle,
	AdminNativeInteriorID,
	AdminPerfHudState,
	AdminReloadCheckIntDetails,
	AdminReloadCheckVehDetails,
	AdminReportEnded,
	AdminToggleItemLock,
	AdminToggleNoteLock,
	AdminTowGetVehicles,
	AdminTowGotVehicles,
	Admin_GotPendingApps,
	ANPR_GetSpeed,
	ANPR_GotSpeed,
	AnswerCall,
	ApplicationState,
	ApplyCustomControls,
	ApplyCustomSkinData,
	ApplyPlayerKeybinds,
	ApplyRemoteChatSettings,
	ApproveApplication,
	AssetTransferCompleted,
	AttemptEndDrivingTest,
	AttemptQuitJob,
	AttemptStartDrivingTest,
	AttemptStartJob,
	AwardAchievement,
	Banking_GetAccountInfo,
	Banking_GotAccountInfo,
	Banking_OnDeposit,
	Banking_OnPayDownDebt,
	Banking_OnServerResponse,
	Banking_OnWireTransfer,
	Banking_OnWithdraw,
	Banking_PayDownDebt,
	Banking_RefreshCreditInfo,
	Banking_RequestInfoResponse,
	Banking_ShowMobileBankingUI,
	BarberShop_CalculatePrice,
	BarberShop_GotPrice,
	BarberShop_OnCheckout,
	BasicDonatorInfo,
	BlackJack_Action_HitMe,
	BlackJack_Action_Stick,
	Blackjack_PlaceBet,
	Blackjack_PlaceBet_GetDetails,
	Blackjack_PlaceBet_GotDetails,
	BlipSiren_Request,
	BlipSiren_Response,
	CallFailed,
	CallNumber,
	CallReceived,
	CallState,
	CallTaxi,
	CancelAdminReport,
	CancelCall,
	CancelGoingOnDuty,
	CancelTaggingInProgress,
	CancelTaxi,
	CarWashingComplete,
	CarWashingRequestResponse,
	CasinoManagement_Add,
	CasinoManagement_GetDetails,
	CasinoManagement_GotDetails,
	CasinoManagement_Take,
	ChangeBoomboxRadio,
	ChangeCharacterApproved,
	ChangeFarePerMile,
	ChangeVehicleRadio,
	CharacterChangeRequested,
	CharacterSelected,
	CharacterSelectedLocal,
	CharacterSelectionApproved,
	CharacterSpawned,
	CheckpointBasedJob_GotoCheckpointState,
	CheckpointBasedJob_GotoCheckpointState_Response,
	CheckpointBasedJob_VerifyCheckpoint,
	CheckpointBasedJob_VerifyCheckpoint_Response,
	ChipManagement_Buy,
	ChipManagement_Buy_GetDetails,
	ChipManagement_Buy_GotDetails,
	ChipManagement_Sell,
	ChipManagement_Sell_GetDetails,
	ChipManagement_Sell_GotDetails,
	ClearChatbox,
	ClearNearbyTags,
	ClientFriskPlayer,
	ClientsideException,
	CloseFurnitureInventory,
	ClosePhone,
	ClosePhoneByToggle,
	ClosePropertyInventory,
	CloseVehicleInventory,
	ClothingStore_CalculatePrice,
	ClothingStore_GotPrice,
	ClothingStore_OnCheckout,
	ConsumeDonationPerk,
	Create911LocationBlip,
	CreateBankPed,
	CreateCharacterCustom,
	CreateCharacterPremade,
	CreateCharacterResponse,
	CreateDancerPed,
	CreateGangTag,
	CreateInfoMarker_Request,
	CreateInfoMarker_Response,
	CreateKeybind,
	CreatePhoneMessage,
	CreateStorePed,
	CreateSyncedTrain,
	CuffPlayer,
	CustomAnim_Create,
	CustomAnim_Delete,
	CustomAnim_RequestClientLegacyLoad,
	CustomInterior_CustomMapTransfer_Cancel,
	CustomInterior_CustomMapTransfer_Reset,
	CustomInterior_CustomMapTransfer_SendBytes,
	CustomInterior_CustomMapTransfer_Start,
	CustomInterior_OpenCustomIntUI,
	DeleteInfoMarker,
	DeleteKeybind,
	DenyApplication,
	Destroy911LocationBlip,
	DestroyBankPed,
	DestroyDancerPed,
	DestroyGangTag,
	DestroyStorePed,
	DestroySyncedTrain,
	DiscordDeLink,
	DiscordLinkFinalize,
	DoCharacterTypeUpgrade,
	Donation_RequestInactivityEntities,
	Donation_RespondInactivityEntities,
	DrivingLicense_VerifyVehicleReturn,
	DrivingTest_GetNextCheckpoint,
	DrivingTest_GotoCheckpointState,
	DrivingTest_GotoNextCheckpoint,
	DrivingTest_GotoReturnVehicle,
	DrivingTest_GotoVehicleReturned,
	DrivingTest_ReturnVehicle,
	DutyOutfitEditor_CreateOrUpdateOutfit,
	DutyOutfitEditor_DeleteOutfit,
	DutyOutfitShareInformClient,
	DutySystem_GotUpdatedOutfitList,
	DutySystem_RequestUpdatedOutfitList,
	EditInterior_CommitChange,
	EditInterior_PickupFurniture,
	EditInterior_PlaceFurniture,
	EditInterior_RemoveDefaultFurniture,
	EditInterior_RestoreFurniture,
	EndCall,
	EndFourthOfJulyEvent,
	EnterBarberShop,
	EnterBarberShop_Response,
	EnterClothingStore,
	EnterClothingStore_Response,
	EnterDutyOutfitEditor,
	EnterDutyOutfitEditor_Response,
	EnterElevatorApproved,
	EnterInteriorApproved,
	EnterOutfitEditor,
	EnterOutfitEditor_Response,
	EnterPlasticSurgeon,
	EnterPlasticSurgeon_OfferCharacterUpgrade,
	EnterPlasticSurgeon_Response,
	EnterTattooArtist,
	EnterTattooArtist_Response,
	EnterVehicleReal,
	ExitElevatorApproved,
	ExitFactionMenu,
	ExitGenericCharacterCustomization,
	ExitInteriorApproved,
	ExitVehicleReal,
	ExitVehicleStart,
	ExtendRadio30Days,
	ExtendRadio7Days,
	FactionCreateResult,
	FactionInviteDecision,
	FactionTransactionComplete,
	Faction_AdminRequestViewFactions,
	Faction_AdminViewFactions,
	Faction_AdminViewFactions_DeleteFaction,
	Faction_AdminViewFactions_JoinFaction,
	Faction_CreateFaction,
	Faction_DisbandFaction,
	Faction_EditMessage,
	Faction_InvitePlayer,
	Faction_Kick,
	Faction_LeaveFaction,
	Faction_RequestFactionInfo,
	Faction_RespawnFactionVehicles,
	Faction_SaveRanksAndSalaries,
	Faction_SetMemberRank,
	Faction_ToggleManager,
	Faction_ViewFactionVehicles,
	Faction_ViewFactionVehicles_Response,
	FetchTransferAssetsData,
	FinalizeGoOnDuty,
	FinalizeLicenseDevice,
	FinishTutorialState,
	FireFullCleanup,
	FireHeliDropWater,
	FireHeliDropWaterRequest,
	FireMissionComplete,
	FirePartialCleanup,
	FishingLevelUp,
	Fishing_OnBite,
	Fishing_OnBiteOutcome,
	ForceReSelectCharacter,
	Freeze,
	FriskPlayer,
	FuelingComplete,
	FuelingRequestResponse,
	FurnitureInventoryDetails,
	FurnitureStore_OnCheckout,
	FurnitureStore_OnCheckoutResult,
	GangTags_AcceptShare,
	GangTags_GotoCreator,
	GangTags_GotoTagMode,
	GangTags_RequestShareTag,
	GangTags_SaveActive,
	GangTags_SaveWIP,
	GangTags_ShareTag,
	Generics_ShowGenericUI,
	Generics_SpawnGenerics,
	Generics_UpdateGenericPosition,
	GetBasicDonatorInfo,
	GetBasicRadioInfo,
	GetPhoneContactByNumber,
	GetPhoneContacts,
	GetPhoneMessagesContacts,
	GetPhoneMessagesFromNumber,
	GetPhoneState,
	GetPos,
	GetPurchaserAndPaymentMethods,
	GetStoreInfo,
	GetTotalUnviewedMessages,
	GiveAIOwnership,
	GotBasicDonatorInfo,
	GotBasicRadioInfo,
	GotDonationPurchasables,
	GotoDiscordLinking,
	GotoDiscordLinking_Response,
	GotoFurnitureStore,
	GotoLogin,
	GotoPropertyEditMode,
	GotoRadioStationManagement,
	GotoRoadblockEditor,
	GotoTutorialState,
	GotoVehicleModShop,
	GotoVehicleModShop_Approved,
	GotPhoneContactByNumber,
	GotPhoneContacts,
	GotPhoneMessagesContacts,
	GotPhoneMessagesFromNumber,
	GotQuizQuestions,
	GotQuizWrittenQuestions,
	GotStoreInfo,
	GotTotalUnviewedMessages,
	GPS_Clear,
	GPS_Set,
	HalloweenCoffin,
	HalloweenInteraction,
	HelpRequestCommands,
	HelpRequestCommandsResponse,
	InformClientTaggingComplete,
	InformPlayerOfFireModes,
	InitialJoinEvent,
	InitiateJerryCanRefuel,
	JerryCanRefuelVehicle,
	JoinTvBroadcast,
	LargeDataTransfer_AckFinalTransfer,
	LargeDataTransfer_ClientToServer_AckBlock,
	LargeDataTransfer_ClientToServer_AckFinalTransfer,
	LargeDataTransfer_ClientToServer_Begin,
	LargeDataTransfer_ClientToServer_RequestResend,
	LargeDataTransfer_ClientToServer_ServerAck,
	LargeDataTransfer_SendBytes,
	LargeDataTransfer_ServerToClient_AckBlock,
	LargeDataTransfer_ServerToClient_AckFinalTransfer,
	LargeDataTransfer_ServerToClient_Begin,
	LargeDataTransfer_ServerToClient_ClientAck,
	LargeDataTransfer_ServerToClient_RequestResend,
	LeaveTvBroadcast,
	ListNearbyTags,
	LoadCustomMap,
	LoadDefaultChatSettings,
	LoadTransferAssetsCharacterData,
	LocalPlayerInventoryFull,
	LocksmithOnPickupKeys,
	LocksmithRequestDuplication,
	LoginPlayer,
	LoginResult,
	Marijuana_CloseMenu,
	Marijuana_FertilizeNearbyPlant,
	Marijuana_OnFertilize,
	Marijuana_OnGetSeeds,
	Marijuana_OnSellDrugs,
	Marijuana_OnSheer,
	Marijuana_OnWater,
	Marijuana_SheerNearbyPlant,
	Marijuana_WaterNearbyPlant,
	MdcGotoPerson,
	MdcGotoProperty,
	MdcGotoVehicle,
	MdcPersonResult,
	MdcPropertyResult,
	MdcVehicleResult,
	MergeItem,
	MoveToRappelPosition,
	NewsCameraOperator,
	Notification,
	OfferNewTutorial,
	OnDestroyItem,
	OnDropItem,
	OnEndFrisking,
	OnFriskTakeItem,
	OnInteractWithDancer,
	OnOperateNewsCamera,
	OnOwnerCollectDancerTips,
	OnPickupItem,
	OnPickupStrips,
	OnPlayerConnected,
	OnPlayerDisconnected,
	OnPropertyFurnitureInstanceCreated,
	OnPropertyFurnitureInstanceDestroyed,
	OnShowItem,
	OnStoreCheckout,
	OnStoreCheckout_Response,
	OnUseItem,
	OpenLanguagesUI,
	OutfitEditor_CreateOrUpdateOutfit,
	OutfitEditor_DeleteOutfit,
	PendingApplicationDetails,
	PerfData,
	PersistentNotifications_Created,
	PersistentNotifications_Deleted,
	PersistentNotifications_LoadAll,
	PickupDropoffBasedJob_GotoDropoffState,
	PickupDropoffBasedJob_GotoDropoffState_Response,
	PickupDropoffBasedJob_GotoPickupState,
	PickupDropoffBasedJob_GotoPickupState_Response,
	PickupDropoffBasedJob_VerifyDropoff,
	PickupDropoffBasedJob_VerifyDropoff_Response,
	PickupDropoffBasedJob_VerifyPickup,
	PickupDropoffBasedJob_VerifyPickup_Response,
	PickupNewsCamera,
	PickupStrips,
	PlasticSurgeon_CalculatePrice,
	PlasticSurgeon_Checkout,
	PlasticSurgeon_GotPrice,
	PlayCustomAudio,
	PlayCustomSpeech,
	PlayerLoadedHighPrio,
	PlayerLoadedLowPrio,
	PlayerRawCommand,
	PlayerWentOffDuty,
	PlayerWentOnDuty,
	PlayMetalDetectorAlarm,
	PreviewCharacter,
	PreviewCharacterGotData,
	PropertyMailboxDetails,
	Property_CreatePlayerBlip,
	Property_DestroyAllPlayerBlips,
	Property_DestroyPlayerBlip,
	Property_MowedLawn,
	PurchaseDonationPerk,
	PurchaseInactivityProtection,
	PurchaseMethodsResponse,
	PurchaseProperty_OnCheckout,
	PurchaseProperty_OnPreview,
	PurchaseProperty_RequestInfoResponse,
	PurchaseRadioRequest,
	PurchaseVehicle_OnCheckout,
	QuizComplete,
	QuizResults,
	RadialSetDoorState,
	RadialSetLockState,
	ReadInfoMarker,
	ReadInfoMarker_Response,
	ReceivedAchievements,
	ReceivedFactionInvite,
	RegisterPlayer,
	RegisterResult,
	ReloadCheckIntData,
	ReloadCheckVehData,
	RemoteCallEnded,
	RemoveAsCamOperator,
	RemovePhoneContact,
	RentalShop_CreatePed,
	RentalShop_DestroyPed,
	RentalShop_RentScooter,
	RentVehicle_OnCheckout,
	ReplicateActivityState,
	Reports_ReloadReportData,
	Reports_SendReportData,
	Reports_UpdateReportData,
	RequestAchievements,
	RequestApplicationDetails,
	RequestBeginChangeBoomboxRadio,
	RequestBeginChangeBoomboxRadio_Response,
	RequestCarWashing,
	RequestChangeCharacter,
	RequestCrouch,
	RequestDimensionChange,
	RequestDutyOutfitList,
	RequestDutyOutfitList_Response,
	RequestEditInterior,
	RequestEditTagMode,
	RequestEnterElevator,
	RequestEnterInterior,
	RequestExitElevator,
	RequestExitInterior,
	RequestExitInteriorForced,
	RequestFactionInfo_Response,
	RequestFueling,
	RequestFurnitureInventory,
	RequestGotoTagMode,
	RequestLogout,
	RequestMailbox,
	RequestMap,
	RequestOutfitList,
	RequestOutfitList_Response,
	RequestPendingApplications,
	RequestPlateRun,
	RequestPlayerInventory,
	RequestPlayerNonSpecificDimension,
	RequestPlayerSpecificDimension,
	RequestQuizQuestions,
	RequestStartActivity,
	RequestStartTagging,
	RequestStopActivity,
	RequestStopAnimation,
	RequestStopFishing,
	RequestSubscribeActivity,
	RequestTagCleaning,
	RequestTicket,
	RequestTransferAssets,
	RequestTutorialState,
	RequestUnimpoundVehicle,
	RequestUnsubscribeActivity,
	RequestVehicleInventory,
	RequestVehicleRepair,
	RequestWrittenQuestions,
	ResetActivityState,
	ResetChatSettings,
	ResetFare,
	RespawnChar,
	RetrievedCharacters,
	RetuneRadio,
	Roadblock_PlaceNew,
	Roadblock_RemoveExisting,
	Roadblock_UpdateExisting,
	SafeTeleport,
	SaveAdminInteriorNote,
	SaveAdminNotes,
	SaveAdminVehicleNote,
	SaveChatSettings,
	SaveControls,
	SavePetName,
	SavePhoneContact,
	SaveRadio,
	ScriptUpdate,
	SendBreakingNewsMessage,
	SendSMSNotification,
	SetAllControlsToDefault,
	SetAutoSpawnCharacter,
	SetDiscordStatus,
	SetItemInContainer,
	SetItemInSocket,
	SetKeybindsDisabled,
	SetLoadingWorld,
	SetPetName,
	SetPlayerVisible,
	SetSpotlightRotation,
	SetVehicleGear,
	ShareDutyOutfit,
	ShareDutyOutfit_Outcome,
	ShowAnimationList,
	ShowCharacterLook,
	ShowGenericMessageBox,
	ShowHelpCenter,
	ShowItemsList,
	ShowKeyBindManager,
	ShowLanguages,
	ShowMobileBankUI,
	ShowPayDayOverview,
	ShowSpawnSelector,
	ShowUpdateCharacterLookUI,
	ShowVehiclesList,
	SpawnSelected,
	SpeedCameraTrigger,
	SpeedCameraTrigger_Response,
	SplitItem,
	StartActivityApproved,
	StartDeathEffect,
	StartDrivingTest,
	StartDrivingTest_Rejected,
	StartFireMission,
	StartFishing,
	StartFishing_Approved,
	StartFourthOfJuly,
	StartFourthOfJulyFireworksOnly,
	StartJob,
	StartPerformanceCapture,
	StartRecon,
	StartTagCleaningResponse,
	StartTaggingResponse,
	StopDrivingTest,
	StopJob,
	StopRecon,
	StoreAmmo,
	StoreWeapons,
	Store_CancelRobbery,
	Store_InitiateRobbery,
	Store_OnRobberyFinished,
	SubmitAdminReport,
	SubmitWrittenPortion,
	SyncAllRadios,
	SyncManualVehBrakes,
	SyncManualVehRpm,
	SyncSingleRadio,
	SyncVehicleHandbrake,
	SyncVehicleHandbrakeSound,
	TagCleaningComplete,
	TakeAIOwnership,
	TalkToSanta,
	TattooArtist_CalculatePrice,
	TattooArtist_Checkout,
	TattooArtist_GotPrice,
	TaughtVub,
	TaxiAccepted,
	TaxiCleanup,
	TaxiDriverJob_AtPickup,
	TicketResponse,
	ToggleAvailableForHire,
	ToggleClientSideDebugOption,
	ToggleDebugSpam,
	ToggleEngine,
	ToggleEngineStall,
	ToggleHeadlights,
	ToggleLeftTurnSignal,
	ToggleLocalPlayerNametag,
	ToggleNametags,
	ToggleRightTurnSignal,
	ToggleSeatbelt,
	ToggleSirenMode,
	ToggleSpotlight,
	ToggleVehicleLocked,
	ToggleWindows,
	TowedVehicleList_Request,
	TowedVehicleList_Response,
	TrainDoorStateChanged,
	TrainEnter,
	TrainEnter_Approved,
	TrainExit,
	TrainExit_Approved,
	TrainSync,
	TrainSync_Ack,
	TrainSync_GiveOwnership,
	TrainSync_TakeOwnership,
	TriggerKeybind,
	UgandaStart,
	UgandaStop,
	UnloadCustomMap,
	UnlockAchievement,
	UpdateActiveLanguage,
	UpdateCharacterLook,
	UpdateFireMission,
	UpdateFurnitureCache,
	UpdateGangTagLayers,
	UpdateGangTagProgress,
	UpdateMessageViewed,
	UpdateStolenState,
	UpdateTagCleaning,
	UpdateTagging,
	UpdateWeatherState,
	UseCellphone,
	UseDutyPoint,
	UseDutyPointResult,
	UseFirearmsLicensingDevice,
	UseSprayCan,
	VehicleCrusher_CrushVehicle,
	VehicleCrusher_RequestCrushInformation,
	VehicleCrusher_ShowCrushInterface,
	VehicleInventoryDetails,
	VehicleModShop_GetModPrice,
	VehicleModShop_GetPrice,
	VehicleModShop_GotModPrice,
	VehicleModShop_GotPrice,
	VehicleModShop_OnCheckout,
	VehicleModShop_OnCheckout_Response,
	VehicleModShop_OnExit_Discard,
	VehicleRentalStore_OnCheckoutResult,
	VehicleRentalStore_RequestInfoResponse,
	VehicleRepairComplete,
	VehicleRepairRequestResponse,
	VehicleStore_OnCheckoutResult,
	VehicleStore_RequestInfoResponse,
	WeatherInfo,
}