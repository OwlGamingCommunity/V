using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using RAGE;
using RAGE.Elements;
using PlayerType = RAGE.Elements.Player;
using ObjectType = RAGE.Elements.MapObject;
using VehicleType = RAGE.Elements.Vehicle;
public static class NetworkEvents
{
	public delegate void ActivityRequestInteract_ResponseDelegate(bool bManager);
	public static ActivityRequestInteract_ResponseDelegate ActivityRequestInteract_Response;
	public delegate void Activity_RoundOutcomeDelegate(Int64 uniqueActivityIdentifier, EActivityType activityType, string strDealerOutcome, List<string> lstPlayerOutcomes);
	public static Activity_RoundOutcomeDelegate Activity_RoundOutcome;
	public delegate void AdminCheckDelegate(PlayerType player, AdminCheckDetails playerDetails);
	public static AdminCheckDelegate AdminCheck;
	public delegate void AdminCheckIntDelegate(long interior, AdminCheckInteriorDetails interiorDetails);
	public static AdminCheckIntDelegate AdminCheckInt;
	public delegate void AdminCheckVehDelegate(long vehicleID, AdminCheckVehicleDetails vehicleDetails);
	public static AdminCheckVehDelegate AdminCheckVeh;
	public delegate void AdminConfirmEntityDeleteDelegate(Int64 entityID, EEntityType entityType);
	public static AdminConfirmEntityDeleteDelegate AdminConfirmEntityDelete;
	public delegate void AdminNativeInteriorIDDelegate();
	public static AdminNativeInteriorIDDelegate AdminNativeInteriorID;
	public delegate void AdminPerfHudStateDelegate(bool bEnabled);
	public static AdminPerfHudStateDelegate AdminPerfHudState;
	public delegate void AdminReloadCheckIntDetailsDelegate(AdminCheckInteriorDetails interiorDetails);
	public static AdminReloadCheckIntDetailsDelegate AdminReloadCheckIntDetails;
	public delegate void AdminReloadCheckVehDetailsDelegate(AdminCheckVehicleDetails vehicleDetails);
	public static AdminReloadCheckVehDetailsDelegate AdminReloadCheckVehDetails;
	public delegate void AdminReportEndedDelegate();
	public static AdminReportEndedDelegate AdminReportEnded;
	public delegate void AdminTowGetVehiclesDelegate();
	public static AdminTowGetVehiclesDelegate AdminTowGetVehicles;
	public delegate void Admin_GotPendingAppsDelegate(List<PendingApplication> lstPendingApps);
	public static Admin_GotPendingAppsDelegate Admin_GotPendingApps;
	public delegate void ANPR_GotSpeedDelegate(float fSpeed);
	public static ANPR_GotSpeedDelegate ANPR_GotSpeed;
	public delegate void ApplicationStateDelegate(EApplicationState applicationState);
	public static ApplicationStateDelegate ApplicationState;
	public delegate void ApplyCustomControlsDelegate(List<GameControlObject> lstControls);
	public static ApplyCustomControlsDelegate ApplyCustomControls;
	public delegate void ApplyCustomSkinDataDelegate(PlayerType player);
	public static ApplyCustomSkinDataDelegate ApplyCustomSkinData;
	public static void SendLocalEvent_ApplyCustomSkinData(PlayerType player) { NetworkEvents.ApplyCustomSkinData?.Invoke(player); }
	public delegate void ApplyPlayerKeybindsDelegate(List<PlayerKeybindObject> lstBinds);
	public static ApplyPlayerKeybindsDelegate ApplyPlayerKeybinds;
	public delegate void ApplyRemoteChatSettingsDelegate(ChatSettings chatSettings);
	public static ApplyRemoteChatSettingsDelegate ApplyRemoteChatSettings;
	public delegate void AssetTransferCompletedDelegate();
	public static AssetTransferCompletedDelegate AssetTransferCompleted;
	public delegate void AwardAchievementDelegate(int achievementID, string strTitle, string strCaption, int points, EAchievementRarity rarity, int percent);
	public static AwardAchievementDelegate AwardAchievement;
	public delegate void Banking_GotAccountInfoDelegate(float fMoney, List<CreditDetails> lstCreditDetails);
	public static Banking_GotAccountInfoDelegate Banking_GotAccountInfo;
	public delegate void Banking_OnServerResponseDelegate(EBankingResponseCode result);
	public static Banking_OnServerResponseDelegate Banking_OnServerResponse;
	public delegate void Banking_RefreshCreditInfoDelegate(List<CreditDetails> lstCreditDetails);
	public static Banking_RefreshCreditInfoDelegate Banking_RefreshCreditInfo;
	public delegate void Banking_RequestInfoResponseDelegate(List<Purchaser> lstPurchasers, List<string> lstMethods);
	public static Banking_RequestInfoResponseDelegate Banking_RequestInfoResponse;
	public delegate void BarberShop_GotPriceDelegate(float fPrice, bool hasToken);
	public static BarberShop_GotPriceDelegate BarberShop_GotPrice;
	public delegate void BasicDonatorInfoDelegate(List<DonationPurchasable> lstPurchasables);
	public static BasicDonatorInfoDelegate BasicDonatorInfo;
	public delegate void Blackjack_PlaceBet_GotDetailsDelegate(int totalChips);
	public static Blackjack_PlaceBet_GotDetailsDelegate Blackjack_PlaceBet_GotDetails;
	public delegate void BlipSiren_ResponseDelegate(Vehicle vehicle);
	public static BlipSiren_ResponseDelegate BlipSiren_Response;
	public delegate void CallFailedDelegate(ECallFailedReason reason);
	public static CallFailedDelegate CallFailed;
	public delegate void CallReceivedDelegate(bool bHasExistingTaxiRequest, Int64 number);
	public static CallReceivedDelegate CallReceived;
	public delegate void CallStateDelegate(Int64 number, bool bIsConnected);
	public static CallStateDelegate CallState;
	public delegate void CarWashingCompleteDelegate();
	public static CarWashingCompleteDelegate CarWashingComplete;
	public delegate void CarWashingRequestResponseDelegate(bool bSuccess);
	public static CarWashingRequestResponseDelegate CarWashingRequestResponse;
	public delegate void CasinoManagement_GotDetailsDelegate(int chips);
	public static CasinoManagement_GotDetailsDelegate CasinoManagement_GotDetails;
	public delegate void ChangeCharacterApprovedDelegate();
	public static ChangeCharacterApprovedDelegate ChangeCharacterApproved;
	public delegate void CharacterSelectionApprovedDelegate();
	public static CharacterSelectionApprovedDelegate CharacterSelectionApproved;
	public delegate void CheckpointBasedJob_GotoCheckpointState_ResponseDelegate(EJobID a_JobID, Vector3 a_vecCheckpointPos);
	public static CheckpointBasedJob_GotoCheckpointState_ResponseDelegate CheckpointBasedJob_GotoCheckpointState_Response;
	public delegate void CheckpointBasedJob_VerifyCheckpoint_ResponseDelegate(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel);
	public static CheckpointBasedJob_VerifyCheckpoint_ResponseDelegate CheckpointBasedJob_VerifyCheckpoint_Response;
	public delegate void ChipManagement_Buy_GotDetailsDelegate(int chips);
	public static ChipManagement_Buy_GotDetailsDelegate ChipManagement_Buy_GotDetails;
	public delegate void ChipManagement_Sell_GotDetailsDelegate(int chips);
	public static ChipManagement_Sell_GotDetailsDelegate ChipManagement_Sell_GotDetails;
	public delegate void ClearChatboxDelegate();
	public static ClearChatboxDelegate ClearChatbox;
	public delegate void ClearNearbyTagsDelegate();
	public static ClearNearbyTagsDelegate ClearNearbyTags;
	public delegate void ClientFriskPlayerDelegate(PlayerType playerBeingFrisked, List<CItemInstanceDef> inventory);
	public static ClientFriskPlayerDelegate ClientFriskPlayer;
	public delegate void ClosePhoneByToggleDelegate();
	public static ClosePhoneByToggleDelegate ClosePhoneByToggle;
	public delegate void ClothingStore_GotPriceDelegate(float fPrice, bool hasToken);
	public static ClothingStore_GotPriceDelegate ClothingStore_GotPrice;
	public delegate void Create911LocationBlipDelegate(Vector3 position);
	public static Create911LocationBlipDelegate Create911LocationBlip;
	public delegate void CreateBankPedDelegate(Vector3 vecPos, float fRotZ, uint dimension);
	public static CreateBankPedDelegate CreateBankPed;
	public delegate void CreateCharacterResponseDelegate(ECreateCharacterResponse response);
	public static CreateCharacterResponseDelegate CreateCharacterResponse;
	public delegate void CreateDancerPedDelegate(Vector3 vecPos, float fRot, uint dimension, Int64 dancerID, uint dancerSkin, bool bAllowTip, TransmitAnimation AnimationTransmit);
	public static CreateDancerPedDelegate CreateDancerPed;
	public delegate void CreateFurnitureItemFromCacheDelegate(CFurnitureDefinition furnitureDef, ObjectType objectInstance, long DBID);
	public static CreateFurnitureItemFromCacheDelegate CreateFurnitureItemFromCache;
	public static void SendLocalEvent_CreateFurnitureItemFromCache(CFurnitureDefinition furnitureDef, ObjectType objectInstance, long DBID) { NetworkEvents.CreateFurnitureItemFromCache?.Invoke(furnitureDef, objectInstance, DBID); }
	public delegate void CreateGangTagDelegate(Int64 a_ID, Int64 a_OwnerCharacterID, Vector3 vecPos, float fRotZ, uint a_Dimension, List<GangTagLayer> lstLayers, float fProgress);
	public static CreateGangTagDelegate CreateGangTag;
	public delegate void CreateInfoMarker_RequestDelegate();
	public static CreateInfoMarker_RequestDelegate CreateInfoMarker_Request;
	public delegate void CreateStorePedDelegate(Vector3 vecPos, float fRot, uint dimension, Int64 storeID, EStoreType storeType);
	public static CreateStorePedDelegate CreateStorePed;
	public delegate void CreateSyncedTrainDelegate(int ID, ETrainType TrainType, Vector3 Position, int CurrentSector, int LastTripWireID, float Speed, bool bDoorsOpen);
	public static CreateSyncedTrainDelegate CreateSyncedTrain;
	public delegate void CursorStateChangeDelegate(bool bNewState);
	public static CursorStateChangeDelegate CursorStateChange;
	public static void SendLocalEvent_CursorStateChange(bool bNewState) { NetworkEvents.CursorStateChange?.Invoke(bNewState); }
	public delegate void CustomAnim_RequestClientLegacyLoadDelegate();
	public static CustomAnim_RequestClientLegacyLoadDelegate CustomAnim_RequestClientLegacyLoad;
	public delegate void CustomInterior_CustomMapTransfer_ResetDelegate();
	public static CustomInterior_CustomMapTransfer_ResetDelegate CustomInterior_CustomMapTransfer_Reset;
	public delegate void CustomInterior_OpenCustomIntUIDelegate(long propertyID, string propertyName);
	public static CustomInterior_OpenCustomIntUIDelegate CustomInterior_OpenCustomIntUI;
	public delegate void Destroy911LocationBlipDelegate();
	public static Destroy911LocationBlipDelegate Destroy911LocationBlip;
	public delegate void DestroyBankPedDelegate(Vector3 vecPos, float fRotZ, uint dimension);
	public static DestroyBankPedDelegate DestroyBankPed;
	public delegate void DestroyDancerPedDelegate(Vector3 vecPos, float fRot, uint dimension);
	public static DestroyDancerPedDelegate DestroyDancerPed;
	public delegate void DestroyFurnitureItemFromCacheDelegate(CFurnitureDefinition furnitureDef, ObjectType objectInstance, long DBID);
	public static DestroyFurnitureItemFromCacheDelegate DestroyFurnitureItemFromCache;
	public static void SendLocalEvent_DestroyFurnitureItemFromCache(CFurnitureDefinition furnitureDef, ObjectType objectInstance, long DBID) { NetworkEvents.DestroyFurnitureItemFromCache?.Invoke(furnitureDef, objectInstance, DBID); }
	public delegate void DestroyGangTagDelegate(Int64 a_ID);
	public static DestroyGangTagDelegate DestroyGangTag;
	public delegate void DestroyStorePedDelegate(Vector3 vecPos, float fRot, uint dimension);
	public static DestroyStorePedDelegate DestroyStorePed;
	public delegate void DestroySyncedTrainDelegate(int ID);
	public static DestroySyncedTrainDelegate DestroySyncedTrain;
	public delegate void Donation_RespondInactivityEntitiesDelegate(List<DonationEntityListEntry> lstEntities);
	public static Donation_RespondInactivityEntitiesDelegate Donation_RespondInactivityEntities;
	public delegate void DrivingLicense_VerifyVehicleReturnDelegate();
	public static DrivingLicense_VerifyVehicleReturnDelegate DrivingLicense_VerifyVehicleReturn;
	public delegate void DrivingTest_GotoNextCheckpointDelegate(bool bSuccess, Vector3 vecTarget);
	public static DrivingTest_GotoNextCheckpointDelegate DrivingTest_GotoNextCheckpoint;
	public delegate void DrivingTest_GotoReturnVehicleDelegate(bool bSuccess, float x, float y, float z);
	public static DrivingTest_GotoReturnVehicleDelegate DrivingTest_GotoReturnVehicle;
	public delegate void DrivingTest_GotoVehicleReturnedDelegate(bool bSuccess, bool bPassed, bool bFailedSpeeding, bool bFailedDamage);
	public static DrivingTest_GotoVehicleReturnedDelegate DrivingTest_GotoVehicleReturned;
	public delegate void DutyOutfitShareInformClientDelegate(string strPlayerName, string strOutfitName);
	public static DutyOutfitShareInformClientDelegate DutyOutfitShareInformClient;
	public delegate void DutySystem_GotUpdatedOutfitListDelegate(List<CItemInstanceDef> lstOutfits);
	public static DutySystem_GotUpdatedOutfitListDelegate DutySystem_GotUpdatedOutfitList;
	public delegate void EnterBarberShop_ResponseDelegate();
	public static EnterBarberShop_ResponseDelegate EnterBarberShop_Response;
	public delegate void EnterClothingStore_ResponseDelegate();
	public static EnterClothingStore_ResponseDelegate EnterClothingStore_Response;
	public delegate void EnterDutyOutfitEditor_ResponseDelegate(List<CItemInstanceDef> lstOutfits);
	public static EnterDutyOutfitEditor_ResponseDelegate EnterDutyOutfitEditor_Response;
	public delegate void EnterElevatorApprovedDelegate(float x, float y, float z, int mapID, bool bIsCharacterSelect);
	public static EnterElevatorApprovedDelegate EnterElevatorApproved;
	public delegate void EnterInteriorApprovedDelegate(float x, float y, float z, int mapID, bool bIsCharacterSelect);
	public static EnterInteriorApprovedDelegate EnterInteriorApproved;
	public delegate void EnterOutfitEditor_ResponseDelegate(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing);
	public static EnterOutfitEditor_ResponseDelegate EnterOutfitEditor_Response;
	public delegate void EnterPlasticSurgeon_OfferCharacterUpgradeDelegate();
	public static EnterPlasticSurgeon_OfferCharacterUpgradeDelegate EnterPlasticSurgeon_OfferCharacterUpgrade;
	public delegate void EnterPlasticSurgeon_ResponseDelegate();
	public static EnterPlasticSurgeon_ResponseDelegate EnterPlasticSurgeon_Response;
	public delegate void EnterTattooArtist_ResponseDelegate(List<int> lstCurrentTattooIDs);
	public static EnterTattooArtist_ResponseDelegate EnterTattooArtist_Response;
	public delegate void EnterVehicleRealDelegate(Vehicle vehicle, int seatId);
	public static EnterVehicleRealDelegate EnterVehicleReal;
	public delegate void ExitElevatorApprovedDelegate(float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap);
	public static ExitElevatorApprovedDelegate ExitElevatorApproved;
	public delegate void ExitInteriorApprovedDelegate(float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap);
	public static ExitInteriorApprovedDelegate ExitInteriorApproved;
	public delegate void ExitVehicleRealDelegate(VehicleType vehicleBeingExited);
	public static ExitVehicleRealDelegate ExitVehicleReal;
	public delegate void ExitVehicleStartDelegate(VehicleType vehicleBeingExited, int seatID);
	public static ExitVehicleStartDelegate ExitVehicleStart;
	public delegate void FactionCreateResultDelegate(ECreateFactionResult creationError);
	public static FactionCreateResultDelegate FactionCreateResult;
	public delegate void FactionTransactionCompleteDelegate();
	public static FactionTransactionCompleteDelegate FactionTransactionComplete;
	public delegate void Faction_AdminViewFactionsDelegate(List<CFactionListTransmit> lstFactions);
	public static Faction_AdminViewFactionsDelegate Faction_AdminViewFactions;
	public delegate void Faction_ViewFactionVehicles_ResponseDelegate(List<CFactionVehicleInfoTransmit> lstFactionVehicles);
	public static Faction_ViewFactionVehicles_ResponseDelegate Faction_ViewFactionVehicles_Response;
	public delegate void FireFullCleanupDelegate();
	public static FireFullCleanupDelegate FireFullCleanup;
	public delegate void FireHeliDropWaterDelegate(Vector3 vecPos, bool bIsSyncer);
	public static FireHeliDropWaterDelegate FireHeliDropWater;
	public delegate void FirePartialCleanupDelegate(List<int> cleanedUpSlots);
	public static FirePartialCleanupDelegate FirePartialCleanup;
	public delegate void FishingLevelUpDelegate(int newLevel, int xpRequiredForNextLevel);
	public static FishingLevelUpDelegate FishingLevelUp;
	public delegate void Fishing_OnBiteDelegate(int level);
	public static Fishing_OnBiteDelegate Fishing_OnBite;
	public delegate void FreezeDelegate(bool bFreeze);
	public static FreezeDelegate Freeze;
	public delegate void FuelingCompleteDelegate();
	public static FuelingCompleteDelegate FuelingComplete;
	public delegate void FuelingRequestResponseDelegate(bool bSuccess);
	public static FuelingRequestResponseDelegate FuelingRequestResponse;
	public delegate void FurnitureInventoryDetailsDelegate(List<CItemInstanceDef> inventory);
	public static FurnitureInventoryDetailsDelegate FurnitureInventoryDetails;
	public delegate void FurnitureStore_OnCheckoutResultDelegate(EStoreCheckoutResult result);
	public static FurnitureStore_OnCheckoutResultDelegate FurnitureStore_OnCheckoutResult;
	public delegate void GangTags_GotoCreatorDelegate(List<GangTagLayer> lstLayers, List<GangTagLayer> lstLayersWIP);
	public static GangTags_GotoCreatorDelegate GangTags_GotoCreator;
	public delegate void GangTags_GotoTagModeDelegate(List<GangTagLayer> lstLayers);
	public static GangTags_GotoTagModeDelegate GangTags_GotoTagMode;
	public delegate void GangTags_RequestShareTagDelegate(string strSourceName, List<GangTagLayer> lstLayers);
	public static GangTags_RequestShareTagDelegate GangTags_RequestShareTag;
	public delegate void Generics_ShowGenericUIDelegate();
	public static Generics_ShowGenericUIDelegate Generics_ShowGenericUI;
	public delegate void GiveAIOwnershipDelegate(int number);
	public static GiveAIOwnershipDelegate GiveAIOwnership;
	public delegate void GotBasicDonatorInfoDelegate(int donatorCurrency, List<DonationInventoryItemTransmit> lstDonationInventory);
	public static GotBasicDonatorInfoDelegate GotBasicDonatorInfo;
	public delegate void GotBasicRadioInfoDelegate(int donatorCurrency);
	public static GotBasicRadioInfoDelegate GotBasicRadioInfo;
	public delegate void GotDonationPurchasablesDelegate(List<DonationPurchasable> lstPurchasables);
	public static GotDonationPurchasablesDelegate GotDonationPurchasables;
	public delegate void GotoDiscordLinking_ResponseDelegate(bool bHasLink, string strDiscordName, string strEndpoint);
	public static GotoDiscordLinking_ResponseDelegate GotoDiscordLinking_Response;
	public delegate void GotoFurnitureStoreDelegate(float fStateSalesTax);
	public static GotoFurnitureStoreDelegate GotoFurnitureStore;
	public delegate void GotoLoginDelegate(bool bShowGUI);
	public static GotoLoginDelegate GotoLogin;
	public delegate void GotoPropertyEditModeDelegate(List<CItemInstanceDef> lstFurnitureItemsAvailable);
	public static GotoPropertyEditModeDelegate GotoPropertyEditMode;
	public delegate void GotoRadioStationManagementDelegate();
	public static GotoRadioStationManagementDelegate GotoRadioStationManagement;
	public delegate void GotoRoadblockEditorDelegate();
	public static GotoRoadblockEditorDelegate GotoRoadblockEditor;
	public delegate void GotoTutorialStateDelegate(ETutorialVersions currentTutorialVersion);
	public static GotoTutorialStateDelegate GotoTutorialState;
	public delegate void GotoVehicleModShop_ApprovedDelegate(VehicleType vehicle);
	public static GotoVehicleModShop_ApprovedDelegate GotoVehicleModShop_Approved;
	public delegate void GotPhoneContactByNumberDelegate(string contactName);
	public static GotPhoneContactByNumberDelegate GotPhoneContactByNumber;
	public delegate void GotPhoneContactsDelegate(List<KeyValuePair<string, string>> contactslist);
	public static GotPhoneContactsDelegate GotPhoneContacts;
	public delegate void GotPhoneMessagesContactsDelegate(List<CPhoneMessageContact> contactslist);
	public static GotPhoneMessagesContactsDelegate GotPhoneMessagesContacts;
	public delegate void GotPhoneMessagesFromNumberDelegate(List<CPhoneMessage> messagesList);
	public static GotPhoneMessagesFromNumberDelegate GotPhoneMessagesFromNumber;
	public delegate void GotQuizQuestionsDelegate(List<CQuizQuestion> lstQuizQuestions);
	public static GotQuizQuestionsDelegate GotQuizQuestions;
	public delegate void GotQuizWrittenQuestionsDelegate(List<CQuizWrittenQuestion> lstWrittenQuestions);
	public static GotQuizWrittenQuestionsDelegate GotQuizWrittenQuestions;
	public delegate void GotStoreInfoDelegate(List<EItemID> items, float fSalesTaxRate);
	public static GotStoreInfoDelegate GotStoreInfo;
	public delegate void GotTotalUnviewedMessagesDelegate(int unreadMessages);
	public static GotTotalUnviewedMessagesDelegate GotTotalUnviewedMessages;
	public delegate void GPS_ClearDelegate();
	public static GPS_ClearDelegate GPS_Clear;
	public delegate void GPS_SetDelegate(Vector3 vecPos);
	public static GPS_SetDelegate GPS_Set;
	public delegate void HelpRequestCommandsResponseDelegate(List<CommandHelpInfo> lstCommands);
	public static HelpRequestCommandsResponseDelegate HelpRequestCommandsResponse;
	public delegate void HideInventoryDelegate();
	public static HideInventoryDelegate HideInventory;
	public static void SendLocalEvent_HideInventory() { NetworkEvents.HideInventory?.Invoke(); }
	public delegate void HideProgressBarDelegate();
	public static HideProgressBarDelegate HideProgressBar;
	public static void SendLocalEvent_HideProgressBar() { NetworkEvents.HideProgressBar?.Invoke(); }
	public delegate void InformClientTaggingCompleteDelegate();
	public static InformClientTaggingCompleteDelegate InformClientTaggingComplete;
	public delegate void InformPlayerOfFireModesDelegate(bool isSemiAuto);
	public static InformPlayerOfFireModesDelegate InformPlayerOfFireModes;
	public delegate void InitialJoinEventDelegate(int day, int month, int year, int hour, int min, int sec, int weather, bool bIsDebug);
	public static InitialJoinEventDelegate InitialJoinEvent;
	public delegate void InitiateJerryCanRefuelDelegate(Int64 itemDBID);
	public static InitiateJerryCanRefuelDelegate InitiateJerryCanRefuel;
	public delegate void InputEnabledChangedDelegate(bool enabled);
	public static InputEnabledChangedDelegate InputEnabledChanged;
	public static void SendLocalEvent_InputEnabledChanged(bool enabled) { NetworkEvents.InputEnabledChanged?.Invoke(enabled); }
	public delegate void JoinTvBroadcastDelegate(PlayerType NewsCameraOperator, ObjectType NewsCameraObject);
	public static JoinTvBroadcastDelegate JoinTvBroadcast;
	public delegate void LargeDataTransfer_ClientToServer_AckBlockDelegate(ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_ClientToServer_AckBlockDelegate LargeDataTransfer_ClientToServer_AckBlock;
	public delegate void LargeDataTransfer_ClientToServer_AckFinalTransferDelegate(ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_ClientToServer_AckFinalTransferDelegate LargeDataTransfer_ClientToServer_AckFinalTransfer;
	public delegate void LargeDataTransfer_ClientToServer_RequestResendDelegate(ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_ClientToServer_RequestResendDelegate LargeDataTransfer_ClientToServer_RequestResend;
	public delegate void LargeDataTransfer_ClientToServer_ServerAckDelegate(ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_ClientToServer_ServerAckDelegate LargeDataTransfer_ClientToServer_ServerAck;
	public delegate void LargeDataTransfer_SendBytesDelegate(ELargeDataTransferType a_TransferType, int a_Identifier, byte[] dataBytes);
	public static LargeDataTransfer_SendBytesDelegate LargeDataTransfer_SendBytes;
	public delegate void LargeDataTransfer_ServerToClient_BeginDelegate(ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, bool bAllowClientsideCaching, byte[] key);
	public static LargeDataTransfer_ServerToClient_BeginDelegate LargeDataTransfer_ServerToClient_Begin;
	public delegate void LeaveTvBroadcastDelegate();
	public static LeaveTvBroadcastDelegate LeaveTvBroadcast;
	public delegate void ListNearbyTagsDelegate();
	public static ListNearbyTagsDelegate ListNearbyTags;
	public delegate void LoadCustomMapDelegate(int mapID);
	public static LoadCustomMapDelegate LoadCustomMap;
	public delegate void LoadDefaultChatSettingsDelegate();
	public static LoadDefaultChatSettingsDelegate LoadDefaultChatSettings;
	public delegate void LoadRageClientStorageDelegate(string storageData);
	public static LoadRageClientStorageDelegate LoadRageClientStorage;
	public static void SendLocalEvent_LoadRageClientStorage(string storageData) { NetworkEvents.LoadRageClientStorage?.Invoke(storageData); }
	public delegate void LoadTransferAssetsCharacterDataDelegate(long characterId, float money, float bankmoney, List<SVehicle> vehicles, List<SProperty> properties);
	public static LoadTransferAssetsCharacterDataDelegate LoadTransferAssetsCharacterData;
	public delegate void LocalPlayerDimensionChangedDelegate(uint oldDimension, uint newDimension);
	public static LocalPlayerDimensionChangedDelegate LocalPlayerDimensionChanged;
	public static void SendLocalEvent_LocalPlayerDimensionChanged(uint oldDimension, uint newDimension) { NetworkEvents.LocalPlayerDimensionChanged?.Invoke(oldDimension, newDimension); }
	public delegate void LocalPlayerInventoryFullDelegate(List<CItemInstanceDef> inventory, EShowInventoryAction showInventoryAction);
	public static LocalPlayerInventoryFullDelegate LocalPlayerInventoryFull;
	public delegate void LocalPlayerModelChangedDelegate(uint oldModel, uint newModel);
	public static LocalPlayerModelChangedDelegate LocalPlayerModelChanged;
	public static void SendLocalEvent_LocalPlayerModelChanged(uint oldModel, uint newModel) { NetworkEvents.LocalPlayerModelChanged?.Invoke(oldModel, newModel); }
	public delegate void LocalPlayerStreamInNewAreaDelegate(RAGE.Vector3 vecOldArea, RAGE.Vector3 vecNewArea);
	public static LocalPlayerStreamInNewAreaDelegate LocalPlayerStreamInNewArea;
	public static void SendLocalEvent_LocalPlayerStreamInNewArea(RAGE.Vector3 vecOldArea, RAGE.Vector3 vecNewArea) { NetworkEvents.LocalPlayerStreamInNewArea?.Invoke(vecOldArea, vecNewArea); }
	public delegate void LoginResultDelegate(bool bSuccessful, int userID, string titleID, string Username, string strErrorMessage);
	public static LoginResultDelegate LoginResult;
	public delegate void Marijuana_CloseMenuDelegate();
	public static Marijuana_CloseMenuDelegate Marijuana_CloseMenu;
	public delegate void Marijuana_FertilizeNearbyPlantDelegate();
	public static Marijuana_FertilizeNearbyPlantDelegate Marijuana_FertilizeNearbyPlant;
	public delegate void Marijuana_SheerNearbyPlantDelegate();
	public static Marijuana_SheerNearbyPlantDelegate Marijuana_SheerNearbyPlant;
	public delegate void Marijuana_WaterNearbyPlantDelegate();
	public static Marijuana_WaterNearbyPlantDelegate Marijuana_WaterNearbyPlant;
	public delegate void MdcPersonResultDelegate(CStatsResult personInfo);
	public static MdcPersonResultDelegate MdcPersonResult;
	public delegate void MdcPropertyResultDelegate(CMdtProperty propertyInfo);
	public static MdcPropertyResultDelegate MdcPropertyResult;
	public delegate void MdcVehicleResultDelegate(CMdtVehicle vehicleInfo);
	public static MdcVehicleResultDelegate MdcVehicleResult;
	public delegate void NotificationDelegate(string strTitle, string strMessage, ENotificationIcon icon);
	public static NotificationDelegate Notification;
	public delegate void OfferNewTutorialDelegate(ETutorialVersions currentTutorialVersion);
	public static OfferNewTutorialDelegate OfferNewTutorial;
	public delegate void OnScriptControlChangedDelegate(EScriptControlID controlID, ConsoleKey oldKey, ConsoleKey newKey);
	public static OnScriptControlChangedDelegate OnScriptControlChanged;
	public static void SendLocalEvent_OnScriptControlChanged(EScriptControlID controlID, ConsoleKey oldKey, ConsoleKey newKey) { NetworkEvents.OnScriptControlChanged?.Invoke(controlID, oldKey, newKey); }
	public delegate void OnStoreCheckout_ResponseDelegate(EStoreCheckoutResult result);
	public static OnStoreCheckout_ResponseDelegate OnStoreCheckout_Response;
	public delegate void PendingApplicationDetailsDelegate(PendingApplicationDetails pendingAppDetails);
	public static PendingApplicationDetailsDelegate PendingApplicationDetails;
	public delegate void PerfDataDelegate(int fps);
	public static PerfDataDelegate PerfData;
	public delegate void PersistentNotifications_CreatedDelegate(CPersistentNotification notification);
	public static PersistentNotifications_CreatedDelegate PersistentNotifications_Created;
	public delegate void PersistentNotifications_LoadAllDelegate(List<CPersistentNotification> lstNotifications);
	public static PersistentNotifications_LoadAllDelegate PersistentNotifications_LoadAll;
	public delegate void PickupDropoffBasedJob_GotoDropoffState_ResponseDelegate(EJobID a_JobID, Vector3 a_vecCheckpointPos);
	public static PickupDropoffBasedJob_GotoDropoffState_ResponseDelegate PickupDropoffBasedJob_GotoDropoffState_Response;
	public delegate void PickupDropoffBasedJob_GotoPickupState_ResponseDelegate(EJobID a_JobID, Vector3 a_vecCheckpointPos);
	public static PickupDropoffBasedJob_GotoPickupState_ResponseDelegate PickupDropoffBasedJob_GotoPickupState_Response;
	public delegate void PickupDropoffBasedJob_VerifyDropoff_ResponseDelegate(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel);
	public static PickupDropoffBasedJob_VerifyDropoff_ResponseDelegate PickupDropoffBasedJob_VerifyDropoff_Response;
	public delegate void PickupDropoffBasedJob_VerifyPickup_ResponseDelegate(EJobID a_JobID, bool bIsValid);
	public static PickupDropoffBasedJob_VerifyPickup_ResponseDelegate PickupDropoffBasedJob_VerifyPickup_Response;
	public delegate void PlasticSurgeon_GotPriceDelegate(float fPrice, bool hasToken);
	public static PlasticSurgeon_GotPriceDelegate PlasticSurgeon_GotPrice;
	public delegate void PlayCustomAudioDelegate(EAudioIDs audioID, bool bStopAllOtherAudio);
	public static PlayCustomAudioDelegate PlayCustomAudio;
	public delegate void PlayCustomSpeechDelegate(PlayerType player, ESpeechID speechID, ESpeechType speechType);
	public static PlayCustomSpeechDelegate PlayCustomSpeech;
	public delegate void PlayerWentOffDutyDelegate(PlayerType player);
	public static PlayerWentOffDutyDelegate PlayerWentOffDuty;
	public delegate void PlayerWentOnDutyDelegate(PlayerType playerGoingOnDuty, EDutyType dutyType);
	public static PlayerWentOnDutyDelegate PlayerWentOnDuty;
	public delegate void PlayMetalDetectorAlarmDelegate(Vector3 colshapePosition);
	public static PlayMetalDetectorAlarmDelegate PlayMetalDetectorAlarm;
	public delegate void PreviewCharacterGotDataDelegate();
	public static PreviewCharacterGotDataDelegate PreviewCharacterGotData;
	public delegate void PropertyMailboxDetailsDelegate(Int64 propertyID, EMailboxAccessType accessLevel, List<CItemInstanceDef> inventory);
	public static PropertyMailboxDetailsDelegate PropertyMailboxDetails;
	public delegate void Property_CreatePlayerBlipDelegate(Int32 PropertyID, string strName, Vector3 vecPos);
	public static Property_CreatePlayerBlipDelegate Property_CreatePlayerBlip;
	public delegate void Property_DestroyAllPlayerBlipsDelegate();
	public static Property_DestroyAllPlayerBlipsDelegate Property_DestroyAllPlayerBlips;
	public delegate void Property_DestroyPlayerBlipDelegate(Int32 PropertyID);
	public static Property_DestroyPlayerBlipDelegate Property_DestroyPlayerBlip;
	public delegate void PurchaseMethodsResponseDelegate(List<Purchaser> lstPurchasers, List<string> lstMethods);
	public static PurchaseMethodsResponseDelegate PurchaseMethodsResponse;
	public delegate void PurchaseProperty_RequestInfoResponseDelegate(List<Purchaser> lstPurchasers, List<string> lstMethods);
	public static PurchaseProperty_RequestInfoResponseDelegate PurchaseProperty_RequestInfoResponse;
	public delegate void QuizResultsDelegate(bool bPassed, int numCorrect, int numIncorrect);
	public static QuizResultsDelegate QuizResults;
	public delegate void RageClientStorageLoadedDelegate();
	public static RageClientStorageLoadedDelegate RageClientStorageLoaded;
	public static void SendLocalEvent_RageClientStorageLoaded() { NetworkEvents.RageClientStorageLoaded?.Invoke(); }
	public delegate void ReadInfoMarker_ResponseDelegate(Int64 infoMarkerID, bool bIsCreator, string strText);
	public static ReadInfoMarker_ResponseDelegate ReadInfoMarker_Response;
	public delegate void ReceivedAchievementsDelegate(int numAchievements, int maxAchievements, int totalScore, int maxScore, List<AchievementTransmissionObject> lstAchievements);
	public static ReceivedAchievementsDelegate ReceivedAchievements;
	public delegate void ReceivedFactionInviteDelegate(string strFactionName, string strFromPlayerName, Int64 factionID);
	public static ReceivedFactionInviteDelegate ReceivedFactionInvite;
	public delegate void RegisterResultDelegate(bool bSuccess, string error, string[] errors);
	public static RegisterResultDelegate RegisterResult;
	public delegate void RemoteCallEndedDelegate();
	public static RemoteCallEndedDelegate RemoteCallEnded;
	public delegate void RemoveAsCamOperatorDelegate();
	public static RemoveAsCamOperatorDelegate RemoveAsCamOperator;
	public delegate void RentalShop_CreatePedDelegate(Int64 storeID, Vector3 vecPedPos, float fPedRot, uint pedDimension);
	public static RentalShop_CreatePedDelegate RentalShop_CreatePed;
	public delegate void RentalShop_DestroyPedDelegate(Vector3 vecPos, float fRot, uint dimension);
	public static RentalShop_DestroyPedDelegate RentalShop_DestroyPed;
	public delegate void ReplicateActivityStateDelegate(Int64 uniqueActivityIdentifier, EActivityType activityType, string strState);
	public static ReplicateActivityStateDelegate ReplicateActivityState;
	public delegate void Reports_SendReportDataDelegate(List<CPlayerReport> reports);
	public static Reports_SendReportDataDelegate Reports_SendReportData;
	public delegate void Reports_UpdateReportDataDelegate(List<CPlayerReport> reports);
	public static Reports_UpdateReportDataDelegate Reports_UpdateReportData;
	public delegate void RequestBeginChangeBoomboxRadio_ResponseDelegate(bool bSuccess);
	public static RequestBeginChangeBoomboxRadio_ResponseDelegate RequestBeginChangeBoomboxRadio_Response;
	public delegate void RequestDutyOutfitList_ResponseDelegate(List<CItemInstanceDef> lstOutfits);
	public static RequestDutyOutfitList_ResponseDelegate RequestDutyOutfitList_Response;
	public delegate void RequestFactionInfo_ResponseDelegate(List<CFactionTransmit> lstFactions);
	public static RequestFactionInfo_ResponseDelegate RequestFactionInfo_Response;
	public delegate void RequestOutfitList_ResponseDelegate(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing);
	public static RequestOutfitList_ResponseDelegate RequestOutfitList_Response;
	public delegate void RequestTicketDelegate(string strOfficerName, float fAmount, string strReason);
	public static RequestTicketDelegate RequestTicket;
	public delegate void ResetActivityStateDelegate();
	public static ResetActivityStateDelegate ResetActivityState;
	public delegate void RespawnCharDelegate();
	public static RespawnCharDelegate RespawnChar;
	public delegate void RetrievedCharactersDelegate(List<GetCharactersCharacter> lstCharacters, Int64 currentAutoSpawnCharacter);
	public static RetrievedCharactersDelegate RetrievedCharacters;
	public delegate void RetuneRadioDelegate(Int64 radioID, int channel);
	public static RetuneRadioDelegate RetuneRadio;
	public delegate void SafeTeleportDelegate(float x, float y, float z);
	public static SafeTeleportDelegate SafeTeleport;
	public delegate void ScriptUpdateDelegate();
	public static ScriptUpdateDelegate ScriptUpdate;
	public delegate void SendBreakingNewsMessageDelegate(string BreakingMessage);
	public static SendBreakingNewsMessageDelegate SendBreakingNewsMessage;
	public delegate void SetDiscordStatusDelegate(string strStatus);
	public static SetDiscordStatusDelegate SetDiscordStatus;
	public delegate void SetKeybindsDisabledDelegate(bool a_bKeybindsDisabled);
	public static SetKeybindsDisabledDelegate SetKeybindsDisabled;
	public delegate void SetLoadingWorldDelegate(bool bLoadingWorld);
	public static SetLoadingWorldDelegate SetLoadingWorld;
	public delegate void SetPetNameDelegate(EPetType a_Type, Int64 petID);
	public static SetPetNameDelegate SetPetName;
	public delegate void SetPlayerVisibleDelegate(PlayerType targetPlayer, bool bVisible);
	public static SetPlayerVisibleDelegate SetPlayerVisible;
	public delegate void SetProgressBarDelegate(string text, string percent);
	public static SetProgressBarDelegate SetProgressBar;
	public static void SendLocalEvent_SetProgressBar(string text, string percent) { NetworkEvents.SetProgressBar?.Invoke(text, percent); }
	public delegate void ShowAnimationListDelegate(List<CAnimationCommand> animCmdList);
	public static ShowAnimationListDelegate ShowAnimationList;
	public delegate void ShowCharacterLookDelegate(Int64 characterID, string characterName, Int32 age, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt);
	public static ShowCharacterLookDelegate ShowCharacterLook;
	public delegate void ShowGenericMessageBoxDelegate(string strTitle, string strCaption);
	public static ShowGenericMessageBoxDelegate ShowGenericMessageBox;
	public delegate void ShowHelpCenterDelegate();
	public static ShowHelpCenterDelegate ShowHelpCenter;
	public delegate void ShowItemsListDelegate();
	public static ShowItemsListDelegate ShowItemsList;
	public delegate void ShowKeyBindManagerDelegate();
	public static ShowKeyBindManagerDelegate ShowKeyBindManager;
	public delegate void ShowLanguagesDelegate(List<CCharacterLanguageTransmit> lstCharacterLanguages);
	public static ShowLanguagesDelegate ShowLanguages;
	public delegate void ShowMobileBankUIDelegate();
	public static ShowMobileBankUIDelegate ShowMobileBankUI;
	public delegate void ShowPayDayOverviewDelegate(PayDayDetails paydayDetails);
	public static ShowPayDayOverviewDelegate ShowPayDayOverview;
	public delegate void ShowSpawnSelectorDelegate();
	public static ShowSpawnSelectorDelegate ShowSpawnSelector;
	public delegate void ShowUpdateCharacterLookUIDelegate(Int64 characterID, string characterName, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt);
	public static ShowUpdateCharacterLookUIDelegate ShowUpdateCharacterLookUI;
	public delegate void ShowVehiclesListDelegate();
	public static ShowVehiclesListDelegate ShowVehiclesList;
	public delegate void SpeedCameraTrigger_ResponseDelegate(float speed, int cameraID);
	public static SpeedCameraTrigger_ResponseDelegate SpeedCameraTrigger_Response;
	public delegate void StartActivityApprovedDelegate(int participantIndex, Int64 uniqueIdentifier, EActivityType activityType);
	public static StartActivityApprovedDelegate StartActivityApproved;
	public delegate void StartDeathEffectDelegate();
	public static StartDeathEffectDelegate StartDeathEffect;
	public delegate void StartDrivingTestDelegate(EDrivingTestType testType, bool a_bIsResume);
	public static StartDrivingTestDelegate StartDrivingTest;
	public delegate void StartDrivingTest_RejectedDelegate();
	public static StartDrivingTest_RejectedDelegate StartDrivingTest_Rejected;
	public delegate void StartFireMissionDelegate(EFireMissionID MissionID, EFireType FireType, Vector3 vecPos, bool bIsParticipatingInMission, string strTitle);
	public static StartFireMissionDelegate StartFireMission;
	public delegate void StartFishingDelegate(int currentLevel, int xpRequiredForNextLevel);
	public static StartFishingDelegate StartFishing;
	public delegate void StartFourthOfJulyDelegate();
	public static StartFourthOfJulyDelegate StartFourthOfJuly;
	public delegate void StartFourthOfJulyFireworksOnlyDelegate();
	public static StartFourthOfJulyFireworksOnlyDelegate StartFourthOfJulyFireworksOnly;
	public delegate void StartJobDelegate(EJobID a_JobID, bool a_bIsResume);
	public static StartJobDelegate StartJob;
	public delegate void StartPerformanceCaptureDelegate(int duration);
	public static StartPerformanceCaptureDelegate StartPerformanceCapture;
	public delegate void StartReconDelegate(PlayerType reconTarget);
	public static StartReconDelegate StartRecon;
	public delegate void StartTagCleaningResponseDelegate(bool bApproved);
	public static StartTagCleaningResponseDelegate StartTagCleaningResponse;
	public delegate void StartTaggingResponseDelegate(bool bApproved, Int64 tagID);
	public static StartTaggingResponseDelegate StartTaggingResponse;
	public delegate void StopDrivingTestDelegate();
	public static StopDrivingTestDelegate StopDrivingTest;
	public delegate void StopJobDelegate(EJobID a_JobID);
	public static StopJobDelegate StopJob;
	public delegate void StopReconDelegate();
	public static StopReconDelegate StopRecon;
	public delegate void Store_OnRobberyFinishedDelegate();
	public static Store_OnRobberyFinishedDelegate Store_OnRobberyFinished;
	public delegate void SyncAllRadiosDelegate(List<RadioInstance> lstRadios);
	public static SyncAllRadiosDelegate SyncAllRadios;
	public delegate void SyncSingleRadioDelegate(RadioInstance radioInst);
	public static SyncSingleRadioDelegate SyncSingleRadio;
	public delegate void SyncVehicleHandbrakeSoundDelegate();
	public static SyncVehicleHandbrakeSoundDelegate SyncVehicleHandbrakeSound;
	public delegate void TagCleaningCompleteDelegate();
	public static TagCleaningCompleteDelegate TagCleaningComplete;
	public delegate void TakeAIOwnershipDelegate(int number);
	public static TakeAIOwnershipDelegate TakeAIOwnership;
	public delegate void TattooArtist_GotPriceDelegate(float fPrice, bool hasToken, uint numAdded, uint numRemoved);
	public static TattooArtist_GotPriceDelegate TattooArtist_GotPrice;
	public delegate void TaxiAcceptedDelegate(Vector3 vecPickupPos);
	public static TaxiAcceptedDelegate TaxiAccepted;
	public delegate void TaxiCleanupDelegate();
	public static TaxiCleanupDelegate TaxiCleanup;
	public delegate void ToggleClientSideDebugOptionDelegate(EClientsideDebugOption clientsideDebugOption);
	public static ToggleClientSideDebugOptionDelegate ToggleClientSideDebugOption;
	public delegate void ToggleDebugSpamDelegate();
	public static ToggleDebugSpamDelegate ToggleDebugSpam;
	public delegate void ToggleSeatbeltDelegate();
	public static ToggleSeatbeltDelegate ToggleSeatbelt;
	public delegate void TowedVehicleList_ResponseDelegate(List<Int64> lstVehicles);
	public static TowedVehicleList_ResponseDelegate TowedVehicleList_Response;
	public delegate void TrainDoorStateChangedDelegate(int ID, bool bDoorsOpen);
	public static TrainDoorStateChangedDelegate TrainDoorStateChanged;
	public delegate void TrainEnter_ApprovedDelegate(PlayerType player, int ID, EVehicleSeat Seat);
	public static TrainEnter_ApprovedDelegate TrainEnter_Approved;
	public delegate void TrainExit_ApprovedDelegate(PlayerType player, int ID);
	public static TrainExit_ApprovedDelegate TrainExit_Approved;
	public delegate void TrainSyncDelegate(int ID, float x, float y, float z, float speed, int tripwireID, int currentSector);
	public static TrainSyncDelegate TrainSync;
	public delegate void TrainSync_AckDelegate(int ID);
	public static TrainSync_AckDelegate TrainSync_Ack;
	public delegate void TrainSync_GiveOwnershipDelegate(int id);
	public static TrainSync_GiveOwnershipDelegate TrainSync_GiveOwnership;
	public delegate void TrainSync_TakeOwnershipDelegate(int id);
	public static TrainSync_TakeOwnershipDelegate TrainSync_TakeOwnership;
	public delegate void UnloadCustomMapDelegate(int mapID);
	public static UnloadCustomMapDelegate UnloadCustomMap;
	public delegate void UpdateFireMissionDelegate(List<int> lstSlotsToReIgnite);
	public static UpdateFireMissionDelegate UpdateFireMission;
	public delegate void UpdateFurnitureCacheDelegate(long propertyID, List<CPropertyFurnitureInstance> lstFurniture, List<CPropertyDefaultFurnitureRemovalInstance> lstRemovals);
	public static UpdateFurnitureCacheDelegate UpdateFurnitureCache;
	public delegate void UpdateGangTagLayersDelegate(Int64 a_ID, List<GangTagLayer> lstLayers);
	public static UpdateGangTagLayersDelegate UpdateGangTagLayers;
	public delegate void UpdateGangTagProgressDelegate(Int64 a_ID, float fProgress);
	public static UpdateGangTagProgressDelegate UpdateGangTagProgress;
	public delegate void UpdateWeatherStateDelegate(int weatherValue);
	public static UpdateWeatherStateDelegate UpdateWeatherState;
	public delegate void UseCellphoneDelegate(bool bHasExistingTaxiRequest, bool isCalled, Int64 number);
	public static UseCellphoneDelegate UseCellphone;
	public delegate void UseDutyPointResultDelegate(bool bSuccess, EDutyType a_Type, List<CItemInstanceDef> lstOutfits);
	public static UseDutyPointResultDelegate UseDutyPointResult;
	public delegate void UseFirearmsLicensingDeviceDelegate(bool isRemoval);
	public static UseFirearmsLicensingDeviceDelegate UseFirearmsLicensingDevice;
	public delegate void UseSprayCanDelegate();
	public static UseSprayCanDelegate UseSprayCan;
	public delegate void VehicleCrusher_ShowCrushInterfaceDelegate(float amount, bool token, string name);
	public static VehicleCrusher_ShowCrushInterfaceDelegate VehicleCrusher_ShowCrushInterface;
	public delegate void VehicleInventoryDetailsDelegate(EVehicleInventoryType inventoryType, List<CItemInstanceDef> inventory);
	public static VehicleInventoryDetailsDelegate VehicleInventoryDetails;
	public delegate void VehicleModShop_GotModPriceDelegate(float fPrice, int GCPrice);
	public static VehicleModShop_GotModPriceDelegate VehicleModShop_GotModPrice;
	public delegate void VehicleModShop_GotPriceDelegate(float fPrice, int GCPrice, Dictionary<EModSlot, string> dictOverviewPrices);
	public static VehicleModShop_GotPriceDelegate VehicleModShop_GotPrice;
	public delegate void VehicleModShop_OnCheckout_ResponseDelegate(EVehicleModShopCheckoutResult result);
	public static VehicleModShop_OnCheckout_ResponseDelegate VehicleModShop_OnCheckout_Response;
	public delegate void VehicleRentalStore_OnCheckoutResultDelegate(ERentVehicleResult result);
	public static VehicleRentalStore_OnCheckoutResultDelegate VehicleRentalStore_OnCheckoutResult;
	public delegate void VehicleRentalStore_RequestInfoResponseDelegate(List<Purchaser> lstPurchasers, List<string> lstMethods);
	public static VehicleRentalStore_RequestInfoResponseDelegate VehicleRentalStore_RequestInfoResponse;
	public delegate void VehicleRepairCompleteDelegate();
	public static VehicleRepairCompleteDelegate VehicleRepairComplete;
	public delegate void VehicleRepairRequestResponseDelegate(bool bSuccess);
	public static VehicleRepairRequestResponseDelegate VehicleRepairRequestResponse;
	public delegate void VehicleStore_OnCheckoutResultDelegate(EPurchaseVehicleResult result);
	public static VehicleStore_OnCheckoutResultDelegate VehicleStore_OnCheckoutResult;
	public delegate void VehicleStore_RequestInfoResponseDelegate(List<Purchaser> lstPurchasers, List<string> lstMethods);
	public static VehicleStore_RequestInfoResponseDelegate VehicleStore_RequestInfoResponse;
	public delegate void WeatherInfoDelegate(string strWeatherMain, string strWeatherDescription, float weatherTemp, float weatherTempFeelsLike, Int32 weatherHumidity, float weatherWindSpeed);
	public static WeatherInfoDelegate WeatherInfo;
}
public static class NetworkEventSender
{
	public static void SendNetworkEvent_ActivityRequestInteract() { EventManager.TriggerRemoteEvent(NetworkEventID.ActivityRequestInteract); }
	public static void SendNetworkEvent_AdminClearGangTags(Int64 tagID) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminClearGangTags, tagID); }
	public static void SendNetworkEvent_AdminDeleteElevator(Int64 entityID) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminDeleteElevator, entityID); }
	public static void SendNetworkEvent_AdminDeleteFaction(Int64 entityID) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminDeleteFaction, entityID); }
	public static void SendNetworkEvent_AdminDeleteProperty(Int64 entityID) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminDeleteProperty, entityID); }
	public static void SendNetworkEvent_AdminDeleteVehicle(Int64 entityID) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminDeleteVehicle, entityID); }
	public static void SendNetworkEvent_AdminToggleItemLock(ObjectType worldItemObject) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminToggleItemLock, worldItemObject); }
	public static void SendNetworkEvent_AdminToggleNoteLock(ObjectType worldItemObject) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminToggleNoteLock, worldItemObject); }
	public static void SendNetworkEvent_AdminTowGotVehicles(List<Int64> lstVehicles) { EventManager.TriggerRemoteEvent(NetworkEventID.AdminTowGotVehicles, lstVehicles); }
	public static void SendNetworkEvent_ANPR_GetSpeed(Vehicle vehicle) { EventManager.TriggerRemoteEvent(NetworkEventID.ANPR_GetSpeed, vehicle); }
	public static void SendNetworkEvent_AnswerCall() { EventManager.TriggerRemoteEvent(NetworkEventID.AnswerCall); }
	public static void SendNetworkEvent_ApproveApplication(int accountID) { EventManager.TriggerRemoteEvent(NetworkEventID.ApproveApplication, accountID); }
	public static void SendNetworkEvent_AttemptEndDrivingTest(EDrivingTestType a_DrivingTestType) { EventManager.TriggerRemoteEvent(NetworkEventID.AttemptEndDrivingTest, a_DrivingTestType); }
	public static void SendNetworkEvent_AttemptQuitJob() { EventManager.TriggerRemoteEvent(NetworkEventID.AttemptQuitJob); }
	public static void SendNetworkEvent_AttemptStartDrivingTest(EDrivingTestType a_DrivingTestType, EScriptLocation a_Location) { EventManager.TriggerRemoteEvent(NetworkEventID.AttemptStartDrivingTest, a_DrivingTestType, a_Location); }
	public static void SendNetworkEvent_AttemptStartJob(EJobID a_JobID, EScriptLocation a_Location) { EventManager.TriggerRemoteEvent(NetworkEventID.AttemptStartJob, a_JobID, a_Location); }
	public static void SendNetworkEvent_Banking_GetAccountInfo(EPurchaserType PurchaserType, Int64 PurchaserID) { EventManager.TriggerRemoteEvent(NetworkEventID.Banking_GetAccountInfo, PurchaserType, PurchaserID); }
	public static void SendNetworkEvent_Banking_OnDeposit(float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID) { EventManager.TriggerRemoteEvent(NetworkEventID.Banking_OnDeposit, fAmount, PurchaserType, PurchaserID); }
	public static void SendNetworkEvent_Banking_OnPayDownDebt(EPurchaserType CreditSource, Int64 CreditSourceID, ECreditType CreditType, Int64 ID, float fAmount) { EventManager.TriggerRemoteEvent(NetworkEventID.Banking_OnPayDownDebt, CreditSource, CreditSourceID, CreditType, ID, fAmount); }
	public static void SendNetworkEvent_Banking_OnWireTransfer(string strTargetName, float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID) { EventManager.TriggerRemoteEvent(NetworkEventID.Banking_OnWireTransfer, strTargetName, fAmount, PurchaserType, PurchaserID); }
	public static void SendNetworkEvent_Banking_OnWithdraw(float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID) { EventManager.TriggerRemoteEvent(NetworkEventID.Banking_OnWithdraw, fAmount, PurchaserType, PurchaserID); }
	public static void SendNetworkEvent_Banking_PayDownDebt(float fAmount) { EventManager.TriggerRemoteEvent(NetworkEventID.Banking_PayDownDebt, fAmount); }
	public static void SendNetworkEvent_Banking_ShowMobileBankingUI() { EventManager.TriggerRemoteEvent(NetworkEventID.Banking_ShowMobileBankingUI); }
	public static void SendNetworkEvent_BarberShop_CalculatePrice(int BaseHair, int HairStyle, int HairColor, int HairColorHighlights, int ChestHairStyle, int ChestHairColor, int ChestHairColorHighlights, float ChestHairOpacity, int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlights, float FacialHairOpacity, int FullBeardStyle, int FullBeardColor) { EventManager.TriggerRemoteEvent(NetworkEventID.BarberShop_CalculatePrice, BaseHair, HairStyle, HairColor, HairColorHighlights, ChestHairStyle, ChestHairColor, ChestHairColorHighlights, ChestHairOpacity, FacialHairStyle, FacialHairColor, FacialHairColorHighlights, FacialHairOpacity, FullBeardStyle, FullBeardColor); }
	public static void SendNetworkEvent_BarberShop_OnCheckout(Int64 storeID, int BaseHair, int HairStyle, int HairColor, int HairColorHighlights, int ChestHairStyle, int ChestHairColor, int ChestHairColorHighlights, float ChestHairOpacity, int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlights, float FacialHairOpacity, int FullBeardStyle, int FullBeardColor) { EventManager.TriggerRemoteEvent(NetworkEventID.BarberShop_OnCheckout, storeID, BaseHair, HairStyle, HairColor, HairColorHighlights, ChestHairStyle, ChestHairColor, ChestHairColorHighlights, ChestHairOpacity, FacialHairStyle, FacialHairColor, FacialHairColorHighlights, FacialHairOpacity, FullBeardStyle, FullBeardColor); }
	public static void SendNetworkEvent_BlackJack_Action_HitMe(Int64 uniqueIdentifier) { EventManager.TriggerRemoteEvent(NetworkEventID.BlackJack_Action_HitMe, uniqueIdentifier); }
	public static void SendNetworkEvent_BlackJack_Action_Stick(Int64 uniqueIdentifier) { EventManager.TriggerRemoteEvent(NetworkEventID.BlackJack_Action_Stick, uniqueIdentifier); }
	public static void SendNetworkEvent_Blackjack_PlaceBet(Int64 uniqueIdentifier, int amount) { EventManager.TriggerRemoteEvent(NetworkEventID.Blackjack_PlaceBet, uniqueIdentifier, amount); }
	public static void SendNetworkEvent_Blackjack_PlaceBet_GetDetails() { EventManager.TriggerRemoteEvent(NetworkEventID.Blackjack_PlaceBet_GetDetails); }
	public static void SendNetworkEvent_BlipSiren_Request() { EventManager.TriggerRemoteEvent(NetworkEventID.BlipSiren_Request); }
	public static void SendNetworkEvent_CallNumber(string a_strNumber) { EventManager.TriggerRemoteEvent(NetworkEventID.CallNumber, a_strNumber); }
	public static void SendNetworkEvent_CallTaxi() { EventManager.TriggerRemoteEvent(NetworkEventID.CallTaxi); }
	public static void SendNetworkEvent_CancelAdminReport() { EventManager.TriggerRemoteEvent(NetworkEventID.CancelAdminReport); }
	public static void SendNetworkEvent_CancelCall() { EventManager.TriggerRemoteEvent(NetworkEventID.CancelCall); }
	public static void SendNetworkEvent_CancelGoingOnDuty() { EventManager.TriggerRemoteEvent(NetworkEventID.CancelGoingOnDuty); }
	public static void SendNetworkEvent_CancelTaggingInProgress() { EventManager.TriggerRemoteEvent(NetworkEventID.CancelTaggingInProgress); }
	public static void SendNetworkEvent_CancelTaxi() { EventManager.TriggerRemoteEvent(NetworkEventID.CancelTaxi); }
	public static void SendNetworkEvent_CasinoManagement_Add(Int64 uniqueActivityIdentifier, int amount) { EventManager.TriggerRemoteEvent(NetworkEventID.CasinoManagement_Add, uniqueActivityIdentifier, amount); }
	public static void SendNetworkEvent_CasinoManagement_GetDetails(Int64 uniqueActivityIdentifier) { EventManager.TriggerRemoteEvent(NetworkEventID.CasinoManagement_GetDetails, uniqueActivityIdentifier); }
	public static void SendNetworkEvent_CasinoManagement_Take(Int64 uniqueActivityIdentifier, int amount) { EventManager.TriggerRemoteEvent(NetworkEventID.CasinoManagement_Take, uniqueActivityIdentifier, amount); }
	public static void SendNetworkEvent_ChangeBoomboxRadio(ObjectType worldItemObject, string strStationName, int a_RadioID) { EventManager.TriggerRemoteEvent(NetworkEventID.ChangeBoomboxRadio, worldItemObject, strStationName, a_RadioID); }
	public static void SendNetworkEvent_ChangeFarePerMile(float fCharge) { EventManager.TriggerRemoteEvent(NetworkEventID.ChangeFarePerMile, fCharge); }
	public static void SendNetworkEvent_ChangeVehicleRadio(int a_RadioID) { EventManager.TriggerRemoteEvent(NetworkEventID.ChangeVehicleRadio, a_RadioID); }
	public static void SendNetworkEvent_CharacterChangeRequested() { EventManager.TriggerRemoteEvent(NetworkEventID.CharacterChangeRequested); }
	public static void SendNetworkEvent_CharacterSelected(long characterID) { EventManager.TriggerRemoteEvent(NetworkEventID.CharacterSelected, characterID); }
	public static void SendNetworkEvent_CharacterSpawned() { EventManager.TriggerRemoteEvent(NetworkEventID.CharacterSpawned); }
	public static void SendNetworkEvent_CheckpointBasedJob_GotoCheckpointState() { EventManager.TriggerRemoteEvent(NetworkEventID.CheckpointBasedJob_GotoCheckpointState); }
	public static void SendNetworkEvent_CheckpointBasedJob_VerifyCheckpoint() { EventManager.TriggerRemoteEvent(NetworkEventID.CheckpointBasedJob_VerifyCheckpoint); }
	public static void SendNetworkEvent_ChipManagement_Buy(int amount) { EventManager.TriggerRemoteEvent(NetworkEventID.ChipManagement_Buy, amount); }
	public static void SendNetworkEvent_ChipManagement_Buy_GetDetails() { EventManager.TriggerRemoteEvent(NetworkEventID.ChipManagement_Buy_GetDetails); }
	public static void SendNetworkEvent_ChipManagement_Sell(int amount) { EventManager.TriggerRemoteEvent(NetworkEventID.ChipManagement_Sell, amount); }
	public static void SendNetworkEvent_ChipManagement_Sell_GetDetails() { EventManager.TriggerRemoteEvent(NetworkEventID.ChipManagement_Sell_GetDetails); }
	public static void SendNetworkEvent_ClientsideException(ClientsideException exceptionObject) { EventManager.TriggerRemoteEvent(NetworkEventID.ClientsideException, exceptionObject); }
	public static void SendNetworkEvent_CloseFurnitureInventory() { EventManager.TriggerRemoteEvent(NetworkEventID.CloseFurnitureInventory); }
	public static void SendNetworkEvent_ClosePhone() { EventManager.TriggerRemoteEvent(NetworkEventID.ClosePhone); }
	public static void SendNetworkEvent_ClosePropertyInventory() { EventManager.TriggerRemoteEvent(NetworkEventID.ClosePropertyInventory); }
	public static void SendNetworkEvent_CloseVehicleInventory(VehicleType currentVehicle, EVehicleInventoryType currentVehicleInventoryType, bool bIsInvertedTrunk) { EventManager.TriggerRemoteEvent(NetworkEventID.CloseVehicleInventory, currentVehicle, currentVehicleInventoryType, bIsInvertedTrunk); }
	public static void SendNetworkEvent_ClothingStore_CalculatePrice(int[] DrawablesClothing, int[] TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, uint skinID) { EventManager.TriggerRemoteEvent(NetworkEventID.ClothingStore_CalculatePrice, DrawablesClothing, TexturesClothing, CurrentPropDrawables, CurrentPropTextures, skinID); }
	public static void SendNetworkEvent_ClothingStore_OnCheckout(Int64 storeID, int[] DrawablesClothing, int[] TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, uint skinID) { EventManager.TriggerRemoteEvent(NetworkEventID.ClothingStore_OnCheckout, storeID, DrawablesClothing, TexturesClothing, CurrentPropDrawables, CurrentPropTextures, skinID); }
	public static void SendNetworkEvent_ConsumeDonationPerk(UInt32 id) { EventManager.TriggerRemoteEvent(NetworkEventID.ConsumeDonationPerk, id); }
	public static void SendNetworkEvent_CreateCharacterCustom(EScriptLocation spawn, EGender gender, string strName, uint SkinHash, int Age, int[] drawables, int[] Textures, Dictionary<ECustomPropSlot, int> PropsDrawables, Dictionary<ECustomPropSlot, int> PropsTextures, int Ageing, float AgeingOpacity, int Makeup, float MakeupOpacity, int MakeupColor, int MakeupColorHighlight, int Blush, float BlushOpacity, int BlushColor, int BlushColorHighlight, int Complexion, float ComplexionOpacity, int SunDamage, float SunDamageOpacity, int Lipstick, float LipstickOpacity, int LipstickColor, int LipstickColorHighlights, int MolesAndFreckles, float MolesAndFrecklesOpacity, float NoseSizeHorizontal, float NoseSizeVertical, float NoseSizeOutwards, float NoseSizeOutwardsUpper, float NoseSizeOutwardsLower, float NoseAngle, float EyebrowHeight, float EyebrowDepth, float CheekboneHeight, float CheekWidth, float CheekWidthLower, float EyeSize, float LipSize, float MouthSize, float MouthSizeLower, float ChinSize, float ChinSizeLower, float ChinWidth, float ChinEffect, float NeckWidth, float NeckWidthLower, int FaceBlend1Mother, int FaceBlend1Father, float FaceBlendFatherPercent, float SkinBlendFatherPercent, int BaseHair, int HairColor, int HairColorHighlights, int EyeColor, int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlight, float FacialHairOpacity, int Blemishes, float BlemishesOpacity, int Eyebrows, float EyebrowsOpacity, int EyebrowsColor, int EyebrowsColorHighlight, List<int> lstTattooIDs, int BodyBlemishes, float BodyBlemishesOpacity, int ChestHair, int ChestHairColor, int ChestHairColorHighlight, float ChestHairOpacity, ECharacterLanguage PrimaryLanguage, ECharacterLanguage SecondaryLanguage) { EventManager.TriggerRemoteEvent(NetworkEventID.CreateCharacterCustom, spawn, gender, strName, SkinHash, Age, drawables, Textures, PropsDrawables, PropsTextures, Ageing, AgeingOpacity, Makeup, MakeupOpacity, MakeupColor, MakeupColorHighlight, Blush, BlushOpacity, BlushColor, BlushColorHighlight, Complexion, ComplexionOpacity, SunDamage, SunDamageOpacity, Lipstick, LipstickOpacity, LipstickColor, LipstickColorHighlights, MolesAndFreckles, MolesAndFrecklesOpacity, NoseSizeHorizontal, NoseSizeVertical, NoseSizeOutwards, NoseSizeOutwardsUpper, NoseSizeOutwardsLower, NoseAngle, EyebrowHeight, EyebrowDepth, CheekboneHeight, CheekWidth, CheekWidthLower, EyeSize, LipSize, MouthSize, MouthSizeLower, ChinSize, ChinSizeLower, ChinWidth, ChinEffect, NeckWidth, NeckWidthLower, FaceBlend1Mother, FaceBlend1Father, FaceBlendFatherPercent, SkinBlendFatherPercent, BaseHair, HairColor, HairColorHighlights, EyeColor, FacialHairStyle, FacialHairColor, FacialHairColorHighlight, FacialHairOpacity, Blemishes, BlemishesOpacity, Eyebrows, EyebrowsOpacity, EyebrowsColor, EyebrowsColorHighlight, lstTattooIDs, BodyBlemishes, BodyBlemishesOpacity, ChestHair, ChestHairColor, ChestHairColorHighlight, ChestHairOpacity, PrimaryLanguage, SecondaryLanguage); }
	public static void SendNetworkEvent_CreateCharacterPremade(EScriptLocation spawn, EGender gender, string strName, uint SkinHash, int Age, int[] drawables, int[] Textures, Dictionary<ECustomPropSlot, int> PropsDrawables, Dictionary<ECustomPropSlot, int> PropsTextures, ECharacterLanguage PrimaryLanguage, ECharacterLanguage SecondaryLanguage) { EventManager.TriggerRemoteEvent(NetworkEventID.CreateCharacterPremade, spawn, gender, strName, SkinHash, Age, drawables, Textures, PropsDrawables, PropsTextures, PrimaryLanguage, SecondaryLanguage); }
	public static void SendNetworkEvent_CreateInfoMarker_Response(string strText, float x, float y, float z) { EventManager.TriggerRemoteEvent(NetworkEventID.CreateInfoMarker_Response, strText, x, y, z); }
	public static void SendNetworkEvent_CreateKeybind(ConsoleKey key, EPlayerKeyBindType bindType, string strAction) { EventManager.TriggerRemoteEvent(NetworkEventID.CreateKeybind, key, bindType, strAction); }
	public static void SendNetworkEvent_CreatePhoneMessage(string to, string content) { EventManager.TriggerRemoteEvent(NetworkEventID.CreatePhoneMessage, to, content); }
	public static void SendNetworkEvent_CuffPlayer(PlayerType targetPlayer) { EventManager.TriggerRemoteEvent(NetworkEventID.CuffPlayer, targetPlayer); }
	public static void SendNetworkEvent_CustomAnim_Create(string commandName, string animDictionary, string animName, bool loop, bool stopOnLastFrame, bool onlyAnimateUpperBody, bool allowPlayerMovement, int durationSeconds, bool isSilent) { EventManager.TriggerRemoteEvent(NetworkEventID.CustomAnim_Create, commandName, animDictionary, animName, loop, stopOnLastFrame, onlyAnimateUpperBody, allowPlayerMovement, durationSeconds, isSilent); }
	public static void SendNetworkEvent_CustomAnim_Delete(string commandName) { EventManager.TriggerRemoteEvent(NetworkEventID.CustomAnim_Delete, commandName); }
	public static void SendNetworkEvent_CustomInterior_CustomMapTransfer_Cancel() { EventManager.TriggerRemoteEvent(NetworkEventID.CustomInterior_CustomMapTransfer_Cancel); }
	public static void SendNetworkEvent_CustomInterior_CustomMapTransfer_SendBytes(byte[] dataBytes) { EventManager.TriggerRemoteEvent(NetworkEventID.CustomInterior_CustomMapTransfer_SendBytes, dataBytes); }
	public static void SendNetworkEvent_CustomInterior_CustomMapTransfer_Start(string mapType, long propertyID, float markerX, float markerY, float markerZ, int expectedBytesLen, int crc) { EventManager.TriggerRemoteEvent(NetworkEventID.CustomInterior_CustomMapTransfer_Start, mapType, propertyID, markerX, markerY, markerZ, expectedBytesLen, crc); }
	public static void SendNetworkEvent_DeleteInfoMarker(Int64 a_InfoMarkerID) { EventManager.TriggerRemoteEvent(NetworkEventID.DeleteInfoMarker, a_InfoMarkerID); }
	public static void SendNetworkEvent_DeleteKeybind(int index) { EventManager.TriggerRemoteEvent(NetworkEventID.DeleteKeybind, index); }
	public static void SendNetworkEvent_DenyApplication(int accountID) { EventManager.TriggerRemoteEvent(NetworkEventID.DenyApplication, accountID); }
	public static void SendNetworkEvent_DiscordDeLink() { EventManager.TriggerRemoteEvent(NetworkEventID.DiscordDeLink); }
	public static void SendNetworkEvent_DiscordLinkFinalize(string strURL) { EventManager.TriggerRemoteEvent(NetworkEventID.DiscordLinkFinalize, strURL); }
	public static void SendNetworkEvent_DoCharacterTypeUpgrade() { EventManager.TriggerRemoteEvent(NetworkEventID.DoCharacterTypeUpgrade); }
	public static void SendNetworkEvent_Donation_RequestInactivityEntities(EDonationInactivityEntityType entityType) { EventManager.TriggerRemoteEvent(NetworkEventID.Donation_RequestInactivityEntities, entityType); }
	public static void SendNetworkEvent_DrivingTest_GetNextCheckpoint(bool bVisualDamage) { EventManager.TriggerRemoteEvent(NetworkEventID.DrivingTest_GetNextCheckpoint, bVisualDamage); }
	public static void SendNetworkEvent_DrivingTest_GotoCheckpointState() { EventManager.TriggerRemoteEvent(NetworkEventID.DrivingTest_GotoCheckpointState); }
	public static void SendNetworkEvent_DrivingTest_GotoReturnVehicle(bool bSuccess, float x, float y, float z) { EventManager.TriggerRemoteEvent(NetworkEventID.DrivingTest_GotoReturnVehicle, bSuccess, x, y, z); }
	public static void SendNetworkEvent_DrivingTest_ReturnVehicle(bool bVisualDamage) { EventManager.TriggerRemoteEvent(NetworkEventID.DrivingTest_ReturnVehicle, bVisualDamage); }
	public static void SendNetworkEvent_DutyOutfitEditor_CreateOrUpdateOutfit(string Name, EDutyType a_DutyType, Dictionary<ECustomClothingComponent, int> DrawablesClothing, Dictionary<ECustomClothingComponent, int> TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, Dictionary<EDutyWeaponSlot, EItemID> Loadout, Int64 outfitID, EDutyOutfitType charType, uint premadeHash, bool bHideHair) { EventManager.TriggerRemoteEvent(NetworkEventID.DutyOutfitEditor_CreateOrUpdateOutfit, Name, a_DutyType, DrawablesClothing, TexturesClothing, CurrentPropDrawables, CurrentPropTextures, Loadout, outfitID, charType, premadeHash, bHideHair); }
	public static void SendNetworkEvent_DutyOutfitEditor_DeleteOutfit(Int64 outfitID) { EventManager.TriggerRemoteEvent(NetworkEventID.DutyOutfitEditor_DeleteOutfit, outfitID); }
	public static void SendNetworkEvent_DutySystem_RequestUpdatedOutfitList(EDutyType a_DutyType) { EventManager.TriggerRemoteEvent(NetworkEventID.DutySystem_RequestUpdatedOutfitList, a_DutyType); }
	public static void SendNetworkEvent_EditInterior_CommitChange(float x, float y, float z, float rx, float ry, float rz, long dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.EditInterior_CommitChange, x, y, z, rx, ry, rz, dbid); }
	public static void SendNetworkEvent_EditInterior_PickupFurniture(long dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.EditInterior_PickupFurniture, dbid); }
	public static void SendNetworkEvent_EditInterior_PlaceFurniture(float x, float y, float z, long dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.EditInterior_PlaceFurniture, x, y, z, dbid); }
	public static void SendNetworkEvent_EditInterior_RemoveDefaultFurniture(float x, float y, float z, uint model) { EventManager.TriggerRemoteEvent(NetworkEventID.EditInterior_RemoveDefaultFurniture, x, y, z, model); }
	public static void SendNetworkEvent_EditInterior_RestoreFurniture(long dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.EditInterior_RestoreFurniture, dbid); }
	public static void SendNetworkEvent_EndCall() { EventManager.TriggerRemoteEvent(NetworkEventID.EndCall); }
	public static void SendNetworkEvent_EndFourthOfJulyEvent() { EventManager.TriggerRemoteEvent(NetworkEventID.EndFourthOfJulyEvent); }
	public static void SendNetworkEvent_EnterBarberShop() { EventManager.TriggerRemoteEvent(NetworkEventID.EnterBarberShop); }
	public static void SendNetworkEvent_EnterClothingStore() { EventManager.TriggerRemoteEvent(NetworkEventID.EnterClothingStore); }
	public static void SendNetworkEvent_EnterDutyOutfitEditor(EDutyType a_DutyType) { EventManager.TriggerRemoteEvent(NetworkEventID.EnterDutyOutfitEditor, a_DutyType); }
	public static void SendNetworkEvent_EnterOutfitEditor() { EventManager.TriggerRemoteEvent(NetworkEventID.EnterOutfitEditor); }
	public static void SendNetworkEvent_EnterPlasticSurgeon() { EventManager.TriggerRemoteEvent(NetworkEventID.EnterPlasticSurgeon); }
	public static void SendNetworkEvent_EnterTattooArtist() { EventManager.TriggerRemoteEvent(NetworkEventID.EnterTattooArtist); }
	public static void SendNetworkEvent_ExitFactionMenu() { EventManager.TriggerRemoteEvent(NetworkEventID.ExitFactionMenu); }
	public static void SendNetworkEvent_ExitGenericCharacterCustomization() { EventManager.TriggerRemoteEvent(NetworkEventID.ExitGenericCharacterCustomization); }
	public static void SendNetworkEvent_ExtendRadio30Days(int a_RadioID) { EventManager.TriggerRemoteEvent(NetworkEventID.ExtendRadio30Days, a_RadioID); }
	public static void SendNetworkEvent_ExtendRadio7Days(int a_RadioID) { EventManager.TriggerRemoteEvent(NetworkEventID.ExtendRadio7Days, a_RadioID); }
	public static void SendNetworkEvent_FactionInviteDecision(bool bAccepted, Int64 FactionID) { EventManager.TriggerRemoteEvent(NetworkEventID.FactionInviteDecision, bAccepted, FactionID); }
	public static void SendNetworkEvent_Faction_AdminRequestViewFactions() { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_AdminRequestViewFactions); }
	public static void SendNetworkEvent_Faction_AdminViewFactions_DeleteFaction(Int64 FactionID) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_AdminViewFactions_DeleteFaction, FactionID); }
	public static void SendNetworkEvent_Faction_AdminViewFactions_JoinFaction(Int64 FactionID) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_AdminViewFactions_JoinFaction, FactionID); }
	public static void SendNetworkEvent_Faction_CreateFaction(string strFullName, string strShortName, string strFactionType) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_CreateFaction, strFullName, strShortName, strFactionType); }
	public static void SendNetworkEvent_Faction_DisbandFaction(int factionIndex) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_DisbandFaction, factionIndex); }
	public static void SendNetworkEvent_Faction_EditMessage(int factionIndex, string strMessage) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_EditMessage, factionIndex, strMessage); }
	public static void SendNetworkEvent_Faction_InvitePlayer(int factionIndex, string strPlayerName) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_InvitePlayer, factionIndex, strPlayerName); }
	public static void SendNetworkEvent_Faction_Kick(int factionIndex, int member_id) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_Kick, factionIndex, member_id); }
	public static void SendNetworkEvent_Faction_LeaveFaction(int factionIndex) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_LeaveFaction, factionIndex); }
	public static void SendNetworkEvent_Faction_RequestFactionInfo() { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_RequestFactionInfo); }
	public static void SendNetworkEvent_Faction_RespawnFactionVehicles(int factionIndex) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_RespawnFactionVehicles, factionIndex); }
	public static void SendNetworkEvent_Faction_SaveRanksAndSalaries(int factionIndex, string jsonData) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_SaveRanksAndSalaries, factionIndex, jsonData); }
	public static void SendNetworkEvent_Faction_SetMemberRank(int faction_id, int member_id, int rank_id) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_SetMemberRank, faction_id, member_id, rank_id); }
	public static void SendNetworkEvent_Faction_ToggleManager(int factionIndex, int memberIndex) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_ToggleManager, factionIndex, memberIndex); }
	public static void SendNetworkEvent_Faction_ViewFactionVehicles(int factionIndex) { EventManager.TriggerRemoteEvent(NetworkEventID.Faction_ViewFactionVehicles, factionIndex); }
	public static void SendNetworkEvent_FetchTransferAssetsData(long characterId) { EventManager.TriggerRemoteEvent(NetworkEventID.FetchTransferAssetsData, characterId); }
	public static void SendNetworkEvent_FinalizeGoOnDuty(EDutyType dutyType, Int64 outfitID) { EventManager.TriggerRemoteEvent(NetworkEventID.FinalizeGoOnDuty, dutyType, outfitID); }
	public static void SendNetworkEvent_FinalizeLicenseDevice(string strTargetName, EWeaponLicenseType weaponLicenseType, bool isRemoval) { EventManager.TriggerRemoteEvent(NetworkEventID.FinalizeLicenseDevice, strTargetName, weaponLicenseType, isRemoval); }
	public static void SendNetworkEvent_FinishTutorialState() { EventManager.TriggerRemoteEvent(NetworkEventID.FinishTutorialState); }
	public static void SendNetworkEvent_FireHeliDropWaterRequest() { EventManager.TriggerRemoteEvent(NetworkEventID.FireHeliDropWaterRequest); }
	public static void SendNetworkEvent_FireMissionComplete() { EventManager.TriggerRemoteEvent(NetworkEventID.FireMissionComplete); }
	public static void SendNetworkEvent_FirePartialCleanup(List<int> cleanedUpSlots) { EventManager.TriggerRemoteEvent(NetworkEventID.FirePartialCleanup, cleanedUpSlots); }
	public static void SendNetworkEvent_Fishing_OnBiteOutcome(int correct, int total) { EventManager.TriggerRemoteEvent(NetworkEventID.Fishing_OnBiteOutcome, correct, total); }
	public static void SendNetworkEvent_FriskPlayer(PlayerType targetPlayer) { EventManager.TriggerRemoteEvent(NetworkEventID.FriskPlayer, targetPlayer); }
	public static void SendNetworkEvent_FurnitureStore_OnCheckout(Int64 storeID, uint FurnitureIndex) { EventManager.TriggerRemoteEvent(NetworkEventID.FurnitureStore_OnCheckout, storeID, FurnitureIndex); }
	public static void SendNetworkEvent_GangTags_AcceptShare(List<GangTagLayer> lstLayers) { EventManager.TriggerRemoteEvent(NetworkEventID.GangTags_AcceptShare, lstLayers); }
	public static void SendNetworkEvent_GangTags_SaveActive(List<GangTagLayer> lstLayers) { EventManager.TriggerRemoteEvent(NetworkEventID.GangTags_SaveActive, lstLayers); }
	public static void SendNetworkEvent_GangTags_SaveWIP(List<GangTagLayer> lstLayers) { EventManager.TriggerRemoteEvent(NetworkEventID.GangTags_SaveWIP, lstLayers); }
	public static void SendNetworkEvent_GangTags_ShareTag(string strTargetName) { EventManager.TriggerRemoteEvent(NetworkEventID.GangTags_ShareTag, strTargetName); }
	public static void SendNetworkEvent_Generics_SpawnGenerics(string genericName, string model, int amount, float price) { EventManager.TriggerRemoteEvent(NetworkEventID.Generics_SpawnGenerics, genericName, model, amount, price); }
	public static void SendNetworkEvent_Generics_UpdateGenericPosition(float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, ObjectType item) { EventManager.TriggerRemoteEvent(NetworkEventID.Generics_UpdateGenericPosition, positionX, positionY, positionZ, rotationX, rotationY, rotationZ, item); }
	public static void SendNetworkEvent_GetBasicDonatorInfo() { EventManager.TriggerRemoteEvent(NetworkEventID.GetBasicDonatorInfo); }
	public static void SendNetworkEvent_GetBasicRadioInfo() { EventManager.TriggerRemoteEvent(NetworkEventID.GetBasicRadioInfo); }
	public static void SendNetworkEvent_GetPhoneContactByNumber(string callingNumber) { EventManager.TriggerRemoteEvent(NetworkEventID.GetPhoneContactByNumber, callingNumber); }
	public static void SendNetworkEvent_GetPhoneContacts() { EventManager.TriggerRemoteEvent(NetworkEventID.GetPhoneContacts); }
	public static void SendNetworkEvent_GetPhoneMessagesContacts() { EventManager.TriggerRemoteEvent(NetworkEventID.GetPhoneMessagesContacts); }
	public static void SendNetworkEvent_GetPhoneMessagesFromNumber(string callingNumber) { EventManager.TriggerRemoteEvent(NetworkEventID.GetPhoneMessagesFromNumber, callingNumber); }
	public static void SendNetworkEvent_GetPhoneState(bool isVisible) { EventManager.TriggerRemoteEvent(NetworkEventID.GetPhoneState, isVisible); }
	public static void SendNetworkEvent_GetPos() { EventManager.TriggerRemoteEvent(NetworkEventID.GetPos); }
	public static void SendNetworkEvent_GetPurchaserAndPaymentMethods(EPurchaseAndPaymentMethodsRequestType requestType) { EventManager.TriggerRemoteEvent(NetworkEventID.GetPurchaserAndPaymentMethods, requestType); }
	public static void SendNetworkEvent_GetStoreInfo(Int64 storeID) { EventManager.TriggerRemoteEvent(NetworkEventID.GetStoreInfo, storeID); }
	public static void SendNetworkEvent_GetTotalUnviewedMessages() { EventManager.TriggerRemoteEvent(NetworkEventID.GetTotalUnviewedMessages); }
	public static void SendNetworkEvent_GotoDiscordLinking() { EventManager.TriggerRemoteEvent(NetworkEventID.GotoDiscordLinking); }
	public static void SendNetworkEvent_GotoVehicleModShop() { EventManager.TriggerRemoteEvent(NetworkEventID.GotoVehicleModShop); }
	public static void SendNetworkEvent_HalloweenCoffin(bool bInPieces) { EventManager.TriggerRemoteEvent(NetworkEventID.HalloweenCoffin, bInPieces); }
	public static void SendNetworkEvent_HalloweenInteraction() { EventManager.TriggerRemoteEvent(NetworkEventID.HalloweenInteraction); }
	public static void SendNetworkEvent_HelpRequestCommands() { EventManager.TriggerRemoteEvent(NetworkEventID.HelpRequestCommands); }
	public static void SendNetworkEvent_JerryCanRefuelVehicle(Int64 itemDBID, Int64 vehicleDBID) { EventManager.TriggerRemoteEvent(NetworkEventID.JerryCanRefuelVehicle, itemDBID, vehicleDBID); }
	public static void SendNetworkEvent_LargeDataTransfer_AckFinalTransfer(ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEvent(NetworkEventID.LargeDataTransfer_AckFinalTransfer, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_Begin(ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, byte[] key) { EventManager.TriggerRemoteEvent(NetworkEventID.LargeDataTransfer_ClientToServer_Begin, a_TransferType, a_Identifier, totalBytes, crc32, key); }
	public static void SendNetworkEvent_LargeDataTransfer_SendBytes(ELargeDataTransferType a_TransferType, int a_Identifier, byte[] dataBytes) { EventManager.TriggerRemoteEvent(NetworkEventID.LargeDataTransfer_SendBytes, a_TransferType, a_Identifier, dataBytes); }
	public static void SendNetworkEvent_LargeDataTransfer_ServerToClient_AckBlock(ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEvent(NetworkEventID.LargeDataTransfer_ServerToClient_AckBlock, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LargeDataTransfer_ServerToClient_AckFinalTransfer(ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEvent(NetworkEventID.LargeDataTransfer_ServerToClient_AckFinalTransfer, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LargeDataTransfer_ServerToClient_ClientAck(ELargeDataTransferType a_TransferType, int a_Identifier, bool bNeedsTransfer) { EventManager.TriggerRemoteEvent(NetworkEventID.LargeDataTransfer_ServerToClient_ClientAck, a_TransferType, a_Identifier, bNeedsTransfer); }
	public static void SendNetworkEvent_LargeDataTransfer_ServerToClient_RequestResend(ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEvent(NetworkEventID.LargeDataTransfer_ServerToClient_RequestResend, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LocksmithOnPickupKeys() { EventManager.TriggerRemoteEvent(NetworkEventID.LocksmithOnPickupKeys); }
	public static void SendNetworkEvent_LocksmithRequestDuplication(string strKeyType, Int64 keyID) { EventManager.TriggerRemoteEvent(NetworkEventID.LocksmithRequestDuplication, strKeyType, keyID); }
	public static void SendNetworkEvent_LoginPlayer(string strUsername, string strPassword, bool bAutoLogin) { EventManager.TriggerRemoteEvent(NetworkEventID.LoginPlayer, strUsername, strPassword, bAutoLogin); }
	public static void SendNetworkEvent_Marijuana_OnFertilize(ObjectType worldObject) { EventManager.TriggerRemoteEvent(NetworkEventID.Marijuana_OnFertilize, worldObject); }
	public static void SendNetworkEvent_Marijuana_OnGetSeeds() { EventManager.TriggerRemoteEvent(NetworkEventID.Marijuana_OnGetSeeds); }
	public static void SendNetworkEvent_Marijuana_OnSellDrugs(uint count) { EventManager.TriggerRemoteEvent(NetworkEventID.Marijuana_OnSellDrugs, count); }
	public static void SendNetworkEvent_Marijuana_OnSheer(ObjectType worldObject) { EventManager.TriggerRemoteEvent(NetworkEventID.Marijuana_OnSheer, worldObject); }
	public static void SendNetworkEvent_Marijuana_OnWater(ObjectType worldObject) { EventManager.TriggerRemoteEvent(NetworkEventID.Marijuana_OnWater, worldObject); }
	public static void SendNetworkEvent_MdcGotoPerson(string strName) { EventManager.TriggerRemoteEvent(NetworkEventID.MdcGotoPerson, strName); }
	public static void SendNetworkEvent_MdcGotoProperty(Int64 propertyID) { EventManager.TriggerRemoteEvent(NetworkEventID.MdcGotoProperty, propertyID); }
	public static void SendNetworkEvent_MdcGotoVehicle(Int64 vehicleID) { EventManager.TriggerRemoteEvent(NetworkEventID.MdcGotoVehicle, vehicleID); }
	public static void SendNetworkEvent_MergeItem(Int64 source_item_dbid, Int64 target_item_dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.MergeItem, source_item_dbid, target_item_dbid); }
	public static void SendNetworkEvent_MoveToRappelPosition(int seat) { EventManager.TriggerRemoteEvent(NetworkEventID.MoveToRappelPosition, seat); }
	public static void SendNetworkEvent_NewsCameraOperator(ObjectType NewsCameraObject) { EventManager.TriggerRemoteEvent(NetworkEventID.NewsCameraOperator, NewsCameraObject); }
	public static void SendNetworkEvent_OnDestroyItem(Int64 item_dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.OnDestroyItem, item_dbid); }
	public static void SendNetworkEvent_OnDropItem(Int64 item_dbid, float x, float y, float z) { EventManager.TriggerRemoteEvent(NetworkEventID.OnDropItem, item_dbid, x, y, z); }
	public static void SendNetworkEvent_OnEndFrisking() { EventManager.TriggerRemoteEvent(NetworkEventID.OnEndFrisking); }
	public static void SendNetworkEvent_OnFriskTakeItem(Int64 itemID) { EventManager.TriggerRemoteEvent(NetworkEventID.OnFriskTakeItem, itemID); }
	public static void SendNetworkEvent_OnInteractWithDancer(Int64 dancerID) { EventManager.TriggerRemoteEvent(NetworkEventID.OnInteractWithDancer, dancerID); }
	public static void SendNetworkEvent_OnOperateNewsCamera() { EventManager.TriggerRemoteEvent(NetworkEventID.OnOperateNewsCamera); }
	public static void SendNetworkEvent_OnOwnerCollectDancerTips(Int64 dancerID) { EventManager.TriggerRemoteEvent(NetworkEventID.OnOwnerCollectDancerTips, dancerID); }
	public static void SendNetworkEvent_OnPickupItem(ObjectType worldItemObject) { EventManager.TriggerRemoteEvent(NetworkEventID.OnPickupItem, worldItemObject); }
	public static void SendNetworkEvent_OnPickupStrips(ObjectType spikeStripObject) { EventManager.TriggerRemoteEvent(NetworkEventID.OnPickupStrips, spikeStripObject); }
	public static void SendNetworkEvent_OnShowItem(Int64 item_dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.OnShowItem, item_dbid); }
	public static void SendNetworkEvent_OnStoreCheckout(Int64 storeID, string strCartContents) { EventManager.TriggerRemoteEvent(NetworkEventID.OnStoreCheckout, storeID, strCartContents); }
	public static void SendNetworkEvent_OnUseItem(Int64 item_dbid) { EventManager.TriggerRemoteEvent(NetworkEventID.OnUseItem, item_dbid); }
	public static void SendNetworkEvent_OpenLanguagesUI() { EventManager.TriggerRemoteEvent(NetworkEventID.OpenLanguagesUI); }
	public static void SendNetworkEvent_OutfitEditor_CreateOrUpdateOutfit(string Name, Dictionary<int, Int64> ClothingItemIDs, Dictionary<int, Int64> PropItemIDs, Int64 outfitID, bool bHideHair) { EventManager.TriggerRemoteEvent(NetworkEventID.OutfitEditor_CreateOrUpdateOutfit, Name, ClothingItemIDs, PropItemIDs, outfitID, bHideHair); }
	public static void SendNetworkEvent_OutfitEditor_DeleteOutfit(Int64 outfitID) { EventManager.TriggerRemoteEvent(NetworkEventID.OutfitEditor_DeleteOutfit, outfitID); }
	public static void SendNetworkEvent_PersistentNotifications_Deleted(Int64 notificationID) { EventManager.TriggerRemoteEvent(NetworkEventID.PersistentNotifications_Deleted, notificationID); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoDropoffState() { EventManager.TriggerRemoteEvent(NetworkEventID.PickupDropoffBasedJob_GotoDropoffState); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoPickupState() { EventManager.TriggerRemoteEvent(NetworkEventID.PickupDropoffBasedJob_GotoPickupState); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyDropoff() { EventManager.TriggerRemoteEvent(NetworkEventID.PickupDropoffBasedJob_VerifyDropoff); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyPickup() { EventManager.TriggerRemoteEvent(NetworkEventID.PickupDropoffBasedJob_VerifyPickup); }
	public static void SendNetworkEvent_PickupNewsCamera(ObjectType newsCameraObject) { EventManager.TriggerRemoteEvent(NetworkEventID.PickupNewsCamera, newsCameraObject); }
	public static void SendNetworkEvent_PickupStrips(ObjectType spikeStripObject) { EventManager.TriggerRemoteEvent(NetworkEventID.PickupStrips, spikeStripObject); }
	public static void SendNetworkEvent_PlasticSurgeon_CalculatePrice() { EventManager.TriggerRemoteEvent(NetworkEventID.PlasticSurgeon_CalculatePrice); }
	public static void SendNetworkEvent_PlasticSurgeon_Checkout(Int64 storeID, int Ageing, float AgeingOpacity, int Makeup, float MakeupOpacity, int MakeupColor, int MakeupColorHighlight, int Blush, float BlushOpacity, int BlushColor, int BlushColorHighlight, int Complexion, float ComplexionOpacity, int SunDamage, float SunDamageOpacity, int Lipstick, float LipstickOpacity, int LipstickColor, int LipstickColorHighlights, int MolesAndFreckles, float MolesAndFrecklesOpacity, float NoseSizeHorizontal, float NoseSizeVertical, float NoseSizeOutwards, float NoseSizeOutwardsUpper, float NoseSizeOutwardsLower, float NoseAngle, float EyebrowHeight, float EyebrowDepth, float CheekboneHeight, float CheekWidth, float CheekWidthLower, float EyeSize, float LipSize, float MouthSize, float MouthSizeLower, float ChinSize, float ChinSizeLower, float ChinWidth, float ChinEffect, float NeckWidth, float NeckWidthLower, int FaceBlend1Mother, int FaceBlend1Father, float FaceBlendFatherPercent, float SkinBlendFatherPercent, int EyeColor, int Blemishes, float BlemishesOpacity, int Eyebrows, float EyebrowsOpacity, int EyebrowsColor, int EyebrowsColorHighlight, int BodyBlemishes, float BodyBlemishesOpacity) { EventManager.TriggerRemoteEvent(NetworkEventID.PlasticSurgeon_Checkout, storeID, Ageing, AgeingOpacity, Makeup, MakeupOpacity, MakeupColor, MakeupColorHighlight, Blush, BlushOpacity, BlushColor, BlushColorHighlight, Complexion, ComplexionOpacity, SunDamage, SunDamageOpacity, Lipstick, LipstickOpacity, LipstickColor, LipstickColorHighlights, MolesAndFreckles, MolesAndFrecklesOpacity, NoseSizeHorizontal, NoseSizeVertical, NoseSizeOutwards, NoseSizeOutwardsUpper, NoseSizeOutwardsLower, NoseAngle, EyebrowHeight, EyebrowDepth, CheekboneHeight, CheekWidth, CheekWidthLower, EyeSize, LipSize, MouthSize, MouthSizeLower, ChinSize, ChinSizeLower, ChinWidth, ChinEffect, NeckWidth, NeckWidthLower, FaceBlend1Mother, FaceBlend1Father, FaceBlendFatherPercent, SkinBlendFatherPercent, EyeColor, Blemishes, BlemishesOpacity, Eyebrows, EyebrowsOpacity, EyebrowsColor, EyebrowsColorHighlight, BodyBlemishes, BodyBlemishesOpacity); }
	public static void SendNetworkEvent_PlayerLoadedHighPrio() { EventManager.TriggerRemoteEvent(NetworkEventID.PlayerLoadedHighPrio); }
	public static void SendNetworkEvent_PlayerLoadedLowPrio() { EventManager.TriggerRemoteEvent(NetworkEventID.PlayerLoadedLowPrio); }
	public static void SendNetworkEvent_PlayerRawCommand(string msg) { EventManager.TriggerRemoteEvent(NetworkEventID.PlayerRawCommand, msg); }
	public static void SendNetworkEvent_PreviewCharacter(long characterID) { EventManager.TriggerRemoteEvent(NetworkEventID.PreviewCharacter, characterID); }
	public static void SendNetworkEvent_Property_MowedLawn(Int64 propertyId) { EventManager.TriggerRemoteEvent(NetworkEventID.Property_MowedLawn, propertyId); }
	public static void SendNetworkEvent_PurchaseDonationPerk(UInt32 id) { EventManager.TriggerRemoteEvent(NetworkEventID.PurchaseDonationPerk, id); }
	public static void SendNetworkEvent_PurchaseInactivityProtection(Int64 TargetEntityID, EDonationInactivityEntityType TargetEntityType, int InactivityLength) { EventManager.TriggerRemoteEvent(NetworkEventID.PurchaseInactivityProtection, TargetEntityID, TargetEntityType, InactivityLength); }
	public static void SendNetworkEvent_PurchaseProperty_OnCheckout(Int64 PropertyID, EPurchaserType purchaserType, long purchaserID, EPaymentMethod method, float fDownpayment, int numMonthsForPaymentPlan) { EventManager.TriggerRemoteEvent(NetworkEventID.PurchaseProperty_OnCheckout, PropertyID, purchaserType, purchaserID, method, fDownpayment, numMonthsForPaymentPlan); }
	public static void SendNetworkEvent_PurchaseProperty_OnPreview(Int64 PropertyID) { EventManager.TriggerRemoteEvent(NetworkEventID.PurchaseProperty_OnPreview, PropertyID); }
	public static void SendNetworkEvent_PurchaseRadioRequest() { EventManager.TriggerRemoteEvent(NetworkEventID.PurchaseRadioRequest); }
	public static void SendNetworkEvent_PurchaseVehicle_OnCheckout(int VehicleIndex, uint primary_r, uint primary_g, uint primary_b, uint secondary_r, uint secondary_g, uint secondary_b, EPurchaserType purchaserType, long purchaserID, EPaymentMethod method, float fDownpayment, int numMonthsForPaymentPlan, EScriptLocation location, EVehicleStoreType storeType) { EventManager.TriggerRemoteEvent(NetworkEventID.PurchaseVehicle_OnCheckout, VehicleIndex, primary_r, primary_g, primary_b, secondary_r, secondary_g, secondary_b, purchaserType, purchaserID, method, fDownpayment, numMonthsForPaymentPlan, location, storeType); }
	public static void SendNetworkEvent_QuizComplete(List<int> lstResponseIndexes) { EventManager.TriggerRemoteEvent(NetworkEventID.QuizComplete, lstResponseIndexes); }
	public static void SendNetworkEvent_RadialSetDoorState(int vehicleID, int doorID) { EventManager.TriggerRemoteEvent(NetworkEventID.RadialSetDoorState, vehicleID, doorID); }
	public static void SendNetworkEvent_RadialSetLockState(int vehicleID) { EventManager.TriggerRemoteEvent(NetworkEventID.RadialSetLockState, vehicleID); }
	public static void SendNetworkEvent_ReadInfoMarker(Int64 a_InfoMarkerID) { EventManager.TriggerRemoteEvent(NetworkEventID.ReadInfoMarker, a_InfoMarkerID); }
	public static void SendNetworkEvent_RegisterPlayer(string strUsername, string strPassword, string strPasswordVerify, string strEmail) { EventManager.TriggerRemoteEvent(NetworkEventID.RegisterPlayer, strUsername, strPassword, strPasswordVerify, strEmail); }
	public static void SendNetworkEvent_ReloadCheckIntData(long propertyID) { EventManager.TriggerRemoteEvent(NetworkEventID.ReloadCheckIntData, propertyID); }
	public static void SendNetworkEvent_ReloadCheckVehData(long vehicleID) { EventManager.TriggerRemoteEvent(NetworkEventID.ReloadCheckVehData, vehicleID); }
	public static void SendNetworkEvent_RemovePhoneContact(string entryNumber, string entryName) { EventManager.TriggerRemoteEvent(NetworkEventID.RemovePhoneContact, entryNumber, entryName); }
	public static void SendNetworkEvent_RentalShop_RentScooter(Int64 storeID) { EventManager.TriggerRemoteEvent(NetworkEventID.RentalShop_RentScooter, storeID); }
	public static void SendNetworkEvent_RentVehicle_OnCheckout(int VehicleIndex, uint primary_r, uint primary_g, uint primary_b, uint secondary_r, uint secondary_g, uint secondary_b, EPurchaserType purchaserType, long purchaserID, uint rentalLengthDays, EScriptLocation location, EVehicleStoreType storeType) { EventManager.TriggerRemoteEvent(NetworkEventID.RentVehicle_OnCheckout, VehicleIndex, primary_r, primary_g, primary_b, secondary_r, secondary_g, secondary_b, purchaserType, purchaserID, rentalLengthDays, location, storeType); }
	public static void SendNetworkEvent_Reports_ReloadReportData() { EventManager.TriggerRemoteEvent(NetworkEventID.Reports_ReloadReportData); }
	public static void SendNetworkEvent_RequestAchievements() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestAchievements); }
	public static void SendNetworkEvent_RequestApplicationDetails(int accountID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestApplicationDetails, accountID); }
	public static void SendNetworkEvent_RequestBeginChangeBoomboxRadio(ObjectType worldItemObject) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestBeginChangeBoomboxRadio, worldItemObject); }
	public static void SendNetworkEvent_RequestCarWashing(Int64 a_carWashID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestCarWashing, a_carWashID); }
	public static void SendNetworkEvent_RequestChangeCharacter() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestChangeCharacter); }
	public static void SendNetworkEvent_RequestCrouch() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestCrouch); }
	public static void SendNetworkEvent_RequestDimensionChange(uint dimension) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestDimensionChange, dimension); }
	public static void SendNetworkEvent_RequestDutyOutfitList(EDutyType a_DutyType) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestDutyOutfitList, a_DutyType); }
	public static void SendNetworkEvent_RequestEditInterior() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestEditInterior); }
	public static void SendNetworkEvent_RequestEditTagMode() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestEditTagMode); }
	public static void SendNetworkEvent_RequestEnterElevator(Int64 ElevatorID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestEnterElevator, ElevatorID); }
	public static void SendNetworkEvent_RequestEnterInterior(Int64 PropertyID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestEnterInterior, PropertyID); }
	public static void SendNetworkEvent_RequestExitElevator(Int64 ElevatorID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestExitElevator, ElevatorID); }
	public static void SendNetworkEvent_RequestExitInterior() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestExitInterior); }
	public static void SendNetworkEvent_RequestExitInteriorForced() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestExitInteriorForced); }
	public static void SendNetworkEvent_RequestFueling(Int64 a_fuelPointID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestFueling, a_fuelPointID); }
	public static void SendNetworkEvent_RequestFurnitureInventory(Int64 furnitureDBID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestFurnitureInventory, furnitureDBID); }
	public static void SendNetworkEvent_RequestGotoTagMode() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestGotoTagMode); }
	public static void SendNetworkEvent_RequestLogout() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestLogout); }
	public static void SendNetworkEvent_RequestMailbox(Int64 PropertyID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestMailbox, PropertyID); }
	public static void SendNetworkEvent_RequestMap(int mapID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestMap, mapID); }
	public static void SendNetworkEvent_RequestOutfitList() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestOutfitList); }
	public static void SendNetworkEvent_RequestPendingApplications() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestPendingApplications); }
	public static void SendNetworkEvent_RequestPlateRun(Vehicle vehicle) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestPlateRun, vehicle); }
	public static void SendNetworkEvent_RequestPlayerInventory() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestPlayerInventory); }
	public static void SendNetworkEvent_RequestPlayerNonSpecificDimension() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestPlayerNonSpecificDimension); }
	public static void SendNetworkEvent_RequestPlayerSpecificDimension() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestPlayerSpecificDimension); }
	public static void SendNetworkEvent_RequestQuizQuestions() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestQuizQuestions); }
	public static void SendNetworkEvent_RequestStartActivity(Int64 uniqueActivityIdentifier, EActivityType activityType) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestStartActivity, uniqueActivityIdentifier, activityType); }
	public static void SendNetworkEvent_RequestStartTagging(float x, float y, float z, float fRotZ) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestStartTagging, x, y, z, fRotZ); }
	public static void SendNetworkEvent_RequestStopActivity(Int64 furnitureDBID, EActivityType activityType) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestStopActivity, furnitureDBID, activityType); }
	public static void SendNetworkEvent_RequestStopAnimation() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestStopAnimation); }
	public static void SendNetworkEvent_RequestStopFishing() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestStopFishing); }
	public static void SendNetworkEvent_RequestSubscribeActivity(Int64 uniqueActivityIdentifier, EActivityType activityType) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestSubscribeActivity, uniqueActivityIdentifier, activityType); }
	public static void SendNetworkEvent_RequestTagCleaning(Int64 tagID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestTagCleaning, tagID); }
	public static void SendNetworkEvent_RequestTransferAssets(long fromCharacterId, long toCharacterId, float money, float bankmoney, List<long> vehicles, List<long> properties) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestTransferAssets, fromCharacterId, toCharacterId, money, bankmoney, vehicles, properties); }
	public static void SendNetworkEvent_RequestTutorialState(ETutorialVersions currentTutorialVersion) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestTutorialState, currentTutorialVersion); }
	public static void SendNetworkEvent_RequestUnimpoundVehicle(VehicleType a_Vehicle, EScriptLocation a_Location) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestUnimpoundVehicle, a_Vehicle, a_Location); }
	public static void SendNetworkEvent_RequestUnsubscribeActivity(Int64 uniqueActivityIdentifier, EActivityType activityType) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestUnsubscribeActivity, uniqueActivityIdentifier, activityType); }
	public static void SendNetworkEvent_RequestVehicleInventory(VehicleType gtaVehicle, EVehicleInventoryType vehicleInventoryType, bool bIsInvertedTrunk) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestVehicleInventory, gtaVehicle, vehicleInventoryType, bIsInvertedTrunk); }
	public static void SendNetworkEvent_RequestVehicleRepair(Int64 a_repairPointID) { EventManager.TriggerRemoteEvent(NetworkEventID.RequestVehicleRepair, a_repairPointID); }
	public static void SendNetworkEvent_RequestWrittenQuestions() { EventManager.TriggerRemoteEvent(NetworkEventID.RequestWrittenQuestions); }
	public static void SendNetworkEvent_ResetChatSettings() { EventManager.TriggerRemoteEvent(NetworkEventID.ResetChatSettings); }
	public static void SendNetworkEvent_ResetFare() { EventManager.TriggerRemoteEvent(NetworkEventID.ResetFare); }
	public static void SendNetworkEvent_RetuneRadio(Int64 radioID, int channel) { EventManager.TriggerRemoteEvent(NetworkEventID.RetuneRadio, radioID, channel); }
	public static void SendNetworkEvent_Roadblock_PlaceNew(int descriptorIndex, float x, float y, float z) { EventManager.TriggerRemoteEvent(NetworkEventID.Roadblock_PlaceNew, descriptorIndex, x, y, z); }
	public static void SendNetworkEvent_Roadblock_RemoveExisting(int entryID) { EventManager.TriggerRemoteEvent(NetworkEventID.Roadblock_RemoveExisting, entryID); }
	public static void SendNetworkEvent_Roadblock_UpdateExisting(int entryID, float x, float y, float z, float rx, float ry, float rz) { EventManager.TriggerRemoteEvent(NetworkEventID.Roadblock_UpdateExisting, entryID, x, y, z, rx, ry, rz); }
	public static void SendNetworkEvent_SaveAdminInteriorNote(string strNote, long interiorID) { EventManager.TriggerRemoteEvent(NetworkEventID.SaveAdminInteriorNote, strNote, interiorID); }
	public static void SendNetworkEvent_SaveAdminNotes(string strNotes, int accountID) { EventManager.TriggerRemoteEvent(NetworkEventID.SaveAdminNotes, strNotes, accountID); }
	public static void SendNetworkEvent_SaveAdminVehicleNote(string strNote, long vehicleID) { EventManager.TriggerRemoteEvent(NetworkEventID.SaveAdminVehicleNote, strNote, vehicleID); }
	public static void SendNetworkEvent_SaveChatSettings(ChatSettings chatSettings) { EventManager.TriggerRemoteEvent(NetworkEventID.SaveChatSettings, chatSettings); }
	public static void SendNetworkEvent_SaveControls(List<GameControlObject> lstGameControls) { EventManager.TriggerRemoteEvent(NetworkEventID.SaveControls, lstGameControls); }
	public static void SendNetworkEvent_SavePetName(Int64 petID, string strName) { EventManager.TriggerRemoteEvent(NetworkEventID.SavePetName, petID, strName); }
	public static void SendNetworkEvent_SavePhoneContact(string entryName, string entryNumber) { EventManager.TriggerRemoteEvent(NetworkEventID.SavePhoneContact, entryName, entryNumber); }
	public static void SendNetworkEvent_SaveRadio(int radioID, string strName, string strEndpoint) { EventManager.TriggerRemoteEvent(NetworkEventID.SaveRadio, radioID, strName, strEndpoint); }
	public static void SendNetworkEvent_SendSMSNotification(string a_strNumber) { EventManager.TriggerRemoteEvent(NetworkEventID.SendSMSNotification, a_strNumber); }
	public static void SendNetworkEvent_SetAllControlsToDefault() { EventManager.TriggerRemoteEvent(NetworkEventID.SetAllControlsToDefault); }
	public static void SendNetworkEvent_SetAutoSpawnCharacter(Int64 characterID) { EventManager.TriggerRemoteEvent(NetworkEventID.SetAutoSpawnCharacter, characterID); }
	public static void SendNetworkEvent_SetItemInContainer(Int64 item_dbid, Int64 container_dbid, bool is_going_to_socket_container, VehicleType currentVehicle, Int64 currentFurnitureDBID) { EventManager.TriggerRemoteEvent(NetworkEventID.SetItemInContainer, item_dbid, container_dbid, is_going_to_socket_container, currentVehicle, currentFurnitureDBID); }
	public static void SendNetworkEvent_SetItemInSocket(Int64 item_dbid, EItemSocket socket_id, VehicleType currentVehicle, Int64 currentFurnitureDBID) { EventManager.TriggerRemoteEvent(NetworkEventID.SetItemInSocket, item_dbid, socket_id, currentVehicle, currentFurnitureDBID); }
	public static void SendNetworkEvent_SetSpotlightRotation(float fRotation) { EventManager.TriggerRemoteEvent(NetworkEventID.SetSpotlightRotation, fRotation); }
	public static void SendNetworkEvent_SetVehicleGear(int gear) { EventManager.TriggerRemoteEvent(NetworkEventID.SetVehicleGear, gear); }
	public static void SendNetworkEvent_ShareDutyOutfit(Int64 outfitDBID, PlayerType rageTargetPlayer) { EventManager.TriggerRemoteEvent(NetworkEventID.ShareDutyOutfit, outfitDBID, rageTargetPlayer); }
	public static void SendNetworkEvent_ShareDutyOutfit_Outcome(bool bAccepted) { EventManager.TriggerRemoteEvent(NetworkEventID.ShareDutyOutfit_Outcome, bAccepted); }
	public static void SendNetworkEvent_SpawnSelected(EScriptLocation location, long characterID) { EventManager.TriggerRemoteEvent(NetworkEventID.SpawnSelected, location, characterID); }
	public static void SendNetworkEvent_SpeedCameraTrigger(float speed, int speedLimit, string name, int cameraID) { EventManager.TriggerRemoteEvent(NetworkEventID.SpeedCameraTrigger, speed, speedLimit, name, cameraID); }
	public static void SendNetworkEvent_SplitItem(Int64 current_item_dbid, uint current_item_new_numstacks, uint new_item_num_stacks) { EventManager.TriggerRemoteEvent(NetworkEventID.SplitItem, current_item_dbid, current_item_new_numstacks, new_item_num_stacks); }
	public static void SendNetworkEvent_StartFishing_Approved() { EventManager.TriggerRemoteEvent(NetworkEventID.StartFishing_Approved); }
	public static void SendNetworkEvent_StoreAmmo(Dictionary<EWeapons, int> weaponsDiff) { EventManager.TriggerRemoteEvent(NetworkEventID.StoreAmmo, weaponsDiff); }
	public static void SendNetworkEvent_StoreWeapons(List<WeaponHash> lstWeapons) { EventManager.TriggerRemoteEvent(NetworkEventID.StoreWeapons, lstWeapons); }
	public static void SendNetworkEvent_Store_CancelRobbery(Int64 storeID) { EventManager.TriggerRemoteEvent(NetworkEventID.Store_CancelRobbery, storeID); }
	public static void SendNetworkEvent_Store_InitiateRobbery(Int64 storeID) { EventManager.TriggerRemoteEvent(NetworkEventID.Store_InitiateRobbery, storeID); }
	public static void SendNetworkEvent_SubmitAdminReport(EAdminReportType reportType, string strDetails, PlayerType rageTargetPlayer) { EventManager.TriggerRemoteEvent(NetworkEventID.SubmitAdminReport, reportType, strDetails, rageTargetPlayer); }
	public static void SendNetworkEvent_SubmitWrittenPortion(string strQ1Answer, string strQ2Answer, string strQ3Answer, string strQ4Answer) { EventManager.TriggerRemoteEvent(NetworkEventID.SubmitWrittenPortion, strQ1Answer, strQ2Answer, strQ3Answer, strQ4Answer); }
	public static void SendNetworkEvent_SyncManualVehBrakes(bool bBrakeLights) { EventManager.TriggerRemoteEvent(NetworkEventID.SyncManualVehBrakes, bBrakeLights); }
	public static void SendNetworkEvent_SyncManualVehRpm(float fVehicleRPM) { EventManager.TriggerRemoteEvent(NetworkEventID.SyncManualVehRpm, fVehicleRPM); }
	public static void SendNetworkEvent_SyncVehicleHandbrake() { EventManager.TriggerRemoteEvent(NetworkEventID.SyncVehicleHandbrake); }
	public static void SendNetworkEvent_TalkToSanta() { EventManager.TriggerRemoteEvent(NetworkEventID.TalkToSanta); }
	public static void SendNetworkEvent_TattooArtist_CalculatePrice(List<int> lstTattoos) { EventManager.TriggerRemoteEvent(NetworkEventID.TattooArtist_CalculatePrice, lstTattoos); }
	public static void SendNetworkEvent_TattooArtist_Checkout(Int64 storeID, List<int> lstTattoos) { EventManager.TriggerRemoteEvent(NetworkEventID.TattooArtist_Checkout, storeID, lstTattoos); }
	public static void SendNetworkEvent_TaughtVub() { EventManager.TriggerRemoteEvent(NetworkEventID.TaughtVub); }
	public static void SendNetworkEvent_TaxiDriverJob_AtPickup() { EventManager.TriggerRemoteEvent(NetworkEventID.TaxiDriverJob_AtPickup); }
	public static void SendNetworkEvent_TicketResponse(bool bAccepted) { EventManager.TriggerRemoteEvent(NetworkEventID.TicketResponse, bAccepted); }
	public static void SendNetworkEvent_ToggleAvailableForHire() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleAvailableForHire); }
	public static void SendNetworkEvent_ToggleEngine() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleEngine); }
	public static void SendNetworkEvent_ToggleEngineStall() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleEngineStall); }
	public static void SendNetworkEvent_ToggleHeadlights() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleHeadlights); }
	public static void SendNetworkEvent_ToggleLeftTurnSignal() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleLeftTurnSignal); }
	public static void SendNetworkEvent_ToggleLocalPlayerNametag() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleLocalPlayerNametag); }
	public static void SendNetworkEvent_ToggleNametags(bool isHidden) { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleNametags, isHidden); }
	public static void SendNetworkEvent_ToggleRightTurnSignal() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleRightTurnSignal); }
	public static void SendNetworkEvent_ToggleSirenMode() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleSirenMode); }
	public static void SendNetworkEvent_ToggleSpotlight() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleSpotlight); }
	public static void SendNetworkEvent_ToggleVehicleLocked() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleVehicleLocked); }
	public static void SendNetworkEvent_ToggleWindows() { EventManager.TriggerRemoteEvent(NetworkEventID.ToggleWindows); }
	public static void SendNetworkEvent_TowedVehicleList_Request() { EventManager.TriggerRemoteEvent(NetworkEventID.TowedVehicleList_Request); }
	public static void SendNetworkEvent_TrainDoorStateChanged(int ID, bool bDoorsOpen) { EventManager.TriggerRemoteEvent(NetworkEventID.TrainDoorStateChanged, ID, bDoorsOpen); }
	public static void SendNetworkEvent_TrainEnter(int ID, bool bAsDriver) { EventManager.TriggerRemoteEvent(NetworkEventID.TrainEnter, ID, bAsDriver); }
	public static void SendNetworkEvent_TrainExit(int ID) { EventManager.TriggerRemoteEvent(NetworkEventID.TrainExit, ID); }
	public static void SendNetworkEvent_TrainSync(int ID, float x, float y, float z, float speed, int tripwireID, int currentSector) { EventManager.TriggerRemoteEvent(NetworkEventID.TrainSync, ID, x, y, z, speed, tripwireID, currentSector); }
	public static void SendNetworkEvent_TriggerKeybind(int index) { EventManager.TriggerRemoteEvent(NetworkEventID.TriggerKeybind, index); }
	public static void SendNetworkEvent_UgandaStart() { EventManager.TriggerRemoteEvent(NetworkEventID.UgandaStart); }
	public static void SendNetworkEvent_UgandaStop() { EventManager.TriggerRemoteEvent(NetworkEventID.UgandaStop); }
	public static void SendNetworkEvent_UnlockAchievement(EAchievementID achievementID) { EventManager.TriggerRemoteEvent(NetworkEventID.UnlockAchievement, achievementID); }
	public static void SendNetworkEvent_UpdateActiveLanguage(ECharacterLanguage ActiveCharacterLanguage) { EventManager.TriggerRemoteEvent(NetworkEventID.UpdateActiveLanguage, ActiveCharacterLanguage); }
	public static void SendNetworkEvent_UpdateCharacterLook(Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup) { EventManager.TriggerRemoteEvent(NetworkEventID.UpdateCharacterLook, height, weight, physicalAppearance, scars, tattoos, makeup); }
	public static void SendNetworkEvent_UpdateMessageViewed(string toNumber) { EventManager.TriggerRemoteEvent(NetworkEventID.UpdateMessageViewed, toNumber); }
	public static void SendNetworkEvent_UpdateStolenState(long vehicleID, bool stolen) { EventManager.TriggerRemoteEvent(NetworkEventID.UpdateStolenState, vehicleID, stolen); }
	public static void SendNetworkEvent_UpdateTagCleaning(Int64 tagID) { EventManager.TriggerRemoteEvent(NetworkEventID.UpdateTagCleaning, tagID); }
	public static void SendNetworkEvent_UpdateTagging(Int64 tagID) { EventManager.TriggerRemoteEvent(NetworkEventID.UpdateTagging, tagID); }
	public static void SendNetworkEvent_UseDutyPoint(EDutyType a_DutyType) { EventManager.TriggerRemoteEvent(NetworkEventID.UseDutyPoint, a_DutyType); }
	public static void SendNetworkEvent_UseFirearmsLicensingDevice(bool isRemoval) { EventManager.TriggerRemoteEvent(NetworkEventID.UseFirearmsLicensingDevice, isRemoval); }
	public static void SendNetworkEvent_VehicleCrusher_CrushVehicle() { EventManager.TriggerRemoteEvent(NetworkEventID.VehicleCrusher_CrushVehicle); }
	public static void SendNetworkEvent_VehicleCrusher_RequestCrushInformation() { EventManager.TriggerRemoteEvent(NetworkEventID.VehicleCrusher_RequestCrushInformation); }
	public static void SendNetworkEvent_VehicleModShop_GetModPrice(EModSlot modSlot, int modIndex, string strPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled) { EventManager.TriggerRemoteEvent(NetworkEventID.VehicleModShop_GetModPrice, modSlot, modIndex, strPlateText, neon_r, neon_g, neon_b, neons_enabled); }
	public static void SendNetworkEvent_VehicleModShop_GetPrice(Dictionary<EModSlot, int> dictPurchasesMods, string strPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled) { EventManager.TriggerRemoteEvent(NetworkEventID.VehicleModShop_GetPrice, dictPurchasesMods, strPlateText, neon_r, neon_g, neon_b, neons_enabled); }
	public static void SendNetworkEvent_VehicleModShop_OnCheckout(Dictionary<EModSlot, int> dictPurchasesMods, string strPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled) { EventManager.TriggerRemoteEvent(NetworkEventID.VehicleModShop_OnCheckout, dictPurchasesMods, strPlateText, neon_r, neon_g, neon_b, neons_enabled); }
	public static void SendNetworkEvent_VehicleModShop_OnExit_Discard() { EventManager.TriggerRemoteEvent(NetworkEventID.VehicleModShop_OnExit_Discard); }
}
public static class UIEvents
{
	public delegate void AcceptPDTicketDelegate();
	public static AcceptPDTicketDelegate AcceptPDTicket;
	public delegate void AchievementOverlay_OnFadedOutDelegate();
	public static AchievementOverlay_OnFadedOutDelegate AchievementOverlay_OnFadedOut;
	public delegate void ActivityInteraction_Dropdown_CancelDelegate();
	public static ActivityInteraction_Dropdown_CancelDelegate ActivityInteraction_Dropdown_Cancel;
	public delegate void ActivityInteraction_Dropdown_DoneDelegate(string displayName, string value);
	public static ActivityInteraction_Dropdown_DoneDelegate ActivityInteraction_Dropdown_Done;
	public delegate void ActivityInteraction_Dropdown_DropdownSelectionChangedDelegate(string displayName, string value);
	public static ActivityInteraction_Dropdown_DropdownSelectionChangedDelegate ActivityInteraction_Dropdown_DropdownSelectionChanged;
	public delegate void AdminEntityDeleteConfirmation_NoDelegate();
	public static AdminEntityDeleteConfirmation_NoDelegate AdminEntityDeleteConfirmation_No;
	public delegate void AdminEntityDeleteConfirmation_YesDelegate();
	public static AdminEntityDeleteConfirmation_YesDelegate AdminEntityDeleteConfirmation_Yes;
	public delegate void AdminViewFactions_DeleteFactionDelegate(Int64 FactionID);
	public static AdminViewFactions_DeleteFactionDelegate AdminViewFactions_DeleteFaction;
	public delegate void AdminViewFactions_JoinFactionDelegate(Int64 FactionID);
	public static AdminViewFactions_JoinFactionDelegate AdminViewFactions_JoinFaction;
	public delegate void AdminViewFaction_Delete_NoDelegate();
	public static AdminViewFaction_Delete_NoDelegate AdminViewFaction_Delete_No;
	public delegate void AdminViewFaction_Delete_YesDelegate();
	public static AdminViewFaction_Delete_YesDelegate AdminViewFaction_Delete_Yes;
	public delegate void AdminViewFaction_Join_NoDelegate();
	public static AdminViewFaction_Join_NoDelegate AdminViewFaction_Join_No;
	public delegate void AdminViewFaction_Join_YesDelegate();
	public static AdminViewFaction_Join_YesDelegate AdminViewFaction_Join_Yes;
	public delegate void AnswerCallDelegate();
	public static AnswerCallDelegate AnswerCall;
	public delegate void ApproveApplicationDelegate(int accountID);
	public static ApproveApplicationDelegate ApproveApplication;
	public delegate void AudioPlayFinishedDelegate(uint poolIndex);
	public static AudioPlayFinishedDelegate AudioPlayFinished;
	public delegate void Banking_OnDepositDelegate(float fAmount);
	public static Banking_OnDepositDelegate Banking_OnDeposit;
	public delegate void Banking_OnHideDelegate();
	public static Banking_OnHideDelegate Banking_OnHide;
	public delegate void Banking_OnSwitchCreditDelegate(int creditIndex);
	public static Banking_OnSwitchCreditDelegate Banking_OnSwitchCredit;
	public delegate void Banking_OnWireTransferDelegate(float fAmount);
	public static Banking_OnWireTransferDelegate Banking_OnWireTransfer;
	public delegate void Banking_OnWithdrawDelegate(float fAmount);
	public static Banking_OnWithdrawDelegate Banking_OnWithdraw;
	public delegate void Banking_PayDownDebtDelegate(float fAmount);
	public static Banking_PayDownDebtDelegate Banking_PayDownDebt;
	public delegate void BarberShop_CheckoutDelegate();
	public static BarberShop_CheckoutDelegate BarberShop_Checkout;
	public delegate void BarberShop_ExitDelegate();
	public static BarberShop_ExitDelegate BarberShop_Exit;
	public delegate void BarberShop_GetPriceDetailsDelegate();
	public static BarberShop_GetPriceDetailsDelegate BarberShop_GetPriceDetails;
	public delegate void BarberShop_OnRootChanged_FullBeardsDelegate(string strElementToReset);
	public static BarberShop_OnRootChanged_FullBeardsDelegate BarberShop_OnRootChanged_FullBeards;
	public delegate void BarberShop_SetBaseHairDelegate(int value);
	public static BarberShop_SetBaseHairDelegate BarberShop_SetBaseHair;
	public delegate void BarberShop_SetChestHairDelegate(int value);
	public static BarberShop_SetChestHairDelegate BarberShop_SetChestHair;
	public delegate void BarberShop_SetChestHairColorDelegate(int value);
	public static BarberShop_SetChestHairColorDelegate BarberShop_SetChestHairColor;
	public delegate void BarberShop_SetChestHairColorHighlightsDelegate(int value);
	public static BarberShop_SetChestHairColorHighlightsDelegate BarberShop_SetChestHairColorHighlights;
	public delegate void BarberShop_SetChestHairOpacityDelegate(float value);
	public static BarberShop_SetChestHairOpacityDelegate BarberShop_SetChestHairOpacity;
	public delegate void BarberShop_SetComponentDrawable_FullBeardsDelegate(int value);
	public static BarberShop_SetComponentDrawable_FullBeardsDelegate BarberShop_SetComponentDrawable_FullBeards;
	public delegate void BarberShop_SetComponentTexture_FullBeardsDelegate(int value);
	public static BarberShop_SetComponentTexture_FullBeardsDelegate BarberShop_SetComponentTexture_FullBeards;
	public delegate void BarberShop_SetFacialHairDelegate(int value);
	public static BarberShop_SetFacialHairDelegate BarberShop_SetFacialHair;
	public delegate void BarberShop_SetFacialHairColorDelegate(int value);
	public static BarberShop_SetFacialHairColorDelegate BarberShop_SetFacialHairColor;
	public delegate void BarberShop_SetFacialHairColorHighlightsDelegate(int value);
	public static BarberShop_SetFacialHairColorHighlightsDelegate BarberShop_SetFacialHairColorHighlights;
	public delegate void BarberShop_SetFacialHairOpacityDelegate(float value);
	public static BarberShop_SetFacialHairOpacityDelegate BarberShop_SetFacialHairOpacity;
	public delegate void BarberShop_SetHairColorDelegate(int value);
	public static BarberShop_SetHairColorDelegate BarberShop_SetHairColor;
	public delegate void BarberShop_SetHairColorHighlightsDelegate(int value);
	public static BarberShop_SetHairColorHighlightsDelegate BarberShop_SetHairColorHighlights;
	public delegate void BarberShop_SetHairStyleDrawableDelegate(int value);
	public static BarberShop_SetHairStyleDrawableDelegate BarberShop_SetHairStyleDrawable;
	public delegate void BarberShop_UpdateChestHairOpacityDelegate(float value);
	public static BarberShop_UpdateChestHairOpacityDelegate BarberShop_UpdateChestHairOpacity;
	public delegate void BarberShop_UpdateFacialHairOpacityDelegate(float value);
	public static BarberShop_UpdateFacialHairOpacityDelegate BarberShop_UpdateFacialHairOpacity;
	public delegate void Blackjack_Action_HitMeDelegate();
	public static Blackjack_Action_HitMeDelegate Blackjack_Action_HitMe;
	public delegate void Blackjack_Action_StickDelegate();
	public static Blackjack_Action_StickDelegate Blackjack_Action_Stick;
	public delegate void Blackjack_PlaceBet_CancelDelegate();
	public static Blackjack_PlaceBet_CancelDelegate Blackjack_PlaceBet_Cancel;
	public delegate void Blackjack_PlaceBet_SubmitDelegate(string strInput);
	public static Blackjack_PlaceBet_SubmitDelegate Blackjack_PlaceBet_Submit;
	public delegate void BoomBox_OnChangeRadio_ConfirmDelegate(string strName, string strValue);
	public static BoomBox_OnChangeRadio_ConfirmDelegate BoomBox_OnChangeRadio_Confirm;
	public delegate void CallNumberDelegate(string number);
	public static CallNumberDelegate CallNumber;
	public delegate void CallTaxiDelegate();
	public static CallTaxiDelegate CallTaxi;
	public delegate void CancelAdminReportDelegate();
	public static CancelAdminReportDelegate CancelAdminReport;
	public delegate void CancelCallDelegate();
	public static CancelCallDelegate CancelCall;
	public delegate void CancelEditRadioDelegate();
	public static CancelEditRadioDelegate CancelEditRadio;
	public delegate void CancelGoingOnDutyDelegate();
	public static CancelGoingOnDutyDelegate CancelGoingOnDuty;
	public delegate void CancelTaxiDelegate();
	public static CancelTaxiDelegate CancelTaxi;
	public delegate void CasinoManagement_AddCurrencyDelegate();
	public static CasinoManagement_AddCurrencyDelegate CasinoManagement_AddCurrency;
	public delegate void CasinoManagement_Add_CancelDelegate();
	public static CasinoManagement_Add_CancelDelegate CasinoManagement_Add_Cancel;
	public delegate void CasinoManagement_Add_SubmitDelegate(string strInput);
	public static CasinoManagement_Add_SubmitDelegate CasinoManagement_Add_Submit;
	public delegate void CasinoManagement_ExitDelegate();
	public static CasinoManagement_ExitDelegate CasinoManagement_Exit;
	public delegate void CasinoManagement_TakeCurrencyDelegate();
	public static CasinoManagement_TakeCurrencyDelegate CasinoManagement_TakeCurrency;
	public delegate void CasinoManagement_Take_CancelDelegate();
	public static CasinoManagement_Take_CancelDelegate CasinoManagement_Take_Cancel;
	public delegate void CasinoManagement_Take_SubmitDelegate(string strInput);
	public static CasinoManagement_Take_SubmitDelegate CasinoManagement_Take_Submit;
	public delegate void ChangeCharacterRequestedDelegate();
	public static ChangeCharacterRequestedDelegate ChangeCharacterRequested;
	public delegate void ChangeFarePerMileDelegate();
	public static ChangeFarePerMileDelegate ChangeFarePerMile;
	public delegate void CharacterLook_CloseDelegate();
	public static CharacterLook_CloseDelegate CharacterLook_Close;
	public delegate void CharCreate_CreateCharacterDelegate();
	public static CharCreate_CreateCharacterDelegate CharCreate_CreateCharacter;
	public delegate void CharCreate_DismissErrorDelegate();
	public static CharCreate_DismissErrorDelegate CharCreate_DismissError;
	public delegate void CharCreate_GotoBodyCam_FarDelegate();
	public static CharCreate_GotoBodyCam_FarDelegate CharCreate_GotoBodyCam_Far;
	public delegate void CharCreate_GotoBodyCam_NearDelegate();
	public static CharCreate_GotoBodyCam_NearDelegate CharCreate_GotoBodyCam_Near;
	public delegate void CharCreate_GotoHeadCamDelegate();
	public static CharCreate_GotoHeadCamDelegate CharCreate_GotoHeadCam;
	public delegate void CharCreate_OfferCreateCharacter_CancelDelegate();
	public static CharCreate_OfferCreateCharacter_CancelDelegate CharCreate_OfferCreateCharacter_Cancel;
	public delegate void CharCreate_OfferCreateCharacter_CreateDelegate();
	public static CharCreate_OfferCreateCharacter_CreateDelegate CharCreate_OfferCreateCharacter_Create;
	public delegate void CharCreate_OnChangeTab_CustomDelegate(uint TabID);
	public static CharCreate_OnChangeTab_CustomDelegate CharCreate_OnChangeTab_Custom;
	public delegate void CharCreate_OnChangeTab_PremadeDelegate(uint TabID);
	public static CharCreate_OnChangeTab_PremadeDelegate CharCreate_OnChangeTab_Premade;
	public delegate void CharCreate_OnRootChanged_AccessoriesDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_AccessoriesDelegate CharCreate_OnRootChanged_Accessories;
	public delegate void CharCreate_OnRootChanged_BraceletsDelegate(string strElementName);
	public static CharCreate_OnRootChanged_BraceletsDelegate CharCreate_OnRootChanged_Bracelets;
	public delegate void CharCreate_OnRootChanged_DecalsDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_DecalsDelegate CharCreate_OnRootChanged_Decals;
	public delegate void CharCreate_OnRootChanged_EarringsDelegate(string strElementName);
	public static CharCreate_OnRootChanged_EarringsDelegate CharCreate_OnRootChanged_Earrings;
	public delegate void CharCreate_OnRootChanged_FullBeardsDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_FullBeardsDelegate CharCreate_OnRootChanged_FullBeards;
	public delegate void CharCreate_OnRootChanged_GlassesDelegate(string strElementName);
	public static CharCreate_OnRootChanged_GlassesDelegate CharCreate_OnRootChanged_Glasses;
	public delegate void CharCreate_OnRootChanged_HatsDelegate(string strElementName);
	public static CharCreate_OnRootChanged_HatsDelegate CharCreate_OnRootChanged_Hats;
	public delegate void CharCreate_OnRootChanged_LegsDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_LegsDelegate CharCreate_OnRootChanged_Legs;
	public delegate void CharCreate_OnRootChanged_ShoesDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_ShoesDelegate CharCreate_OnRootChanged_Shoes;
	public delegate void CharCreate_OnRootChanged_TopsDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_TopsDelegate CharCreate_OnRootChanged_Tops;
	public delegate void CharCreate_OnRootChanged_TorsoDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_TorsoDelegate CharCreate_OnRootChanged_Torso;
	public delegate void CharCreate_OnRootChanged_UndershirtsDelegate(string strElementToReset);
	public static CharCreate_OnRootChanged_UndershirtsDelegate CharCreate_OnRootChanged_Undershirts;
	public delegate void CharCreate_OnRootChanged_WatchesDelegate(string strElementName);
	public static CharCreate_OnRootChanged_WatchesDelegate CharCreate_OnRootChanged_Watches;
	public delegate void CharCreate_ResetRotationDelegate();
	public static CharCreate_ResetRotationDelegate CharCreate_ResetRotation;
	public delegate void CharCreate_SetAgeDelegate(int age);
	public static CharCreate_SetAgeDelegate CharCreate_SetAge;
	public delegate void CharCreate_SetAgeingDelegate(int value);
	public static CharCreate_SetAgeingDelegate CharCreate_SetAgeing;
	public delegate void CharCreate_SetAgeingOpacityDelegate(float value);
	public static CharCreate_SetAgeingOpacityDelegate CharCreate_SetAgeingOpacity;
	public delegate void CharCreate_SetBaseHairDelegate(int value);
	public static CharCreate_SetBaseHairDelegate CharCreate_SetBaseHair;
	public delegate void CharCreate_SetBlemishesDelegate(int value);
	public static CharCreate_SetBlemishesDelegate CharCreate_SetBlemishes;
	public delegate void CharCreate_SetBlemishesOpacityDelegate(float value);
	public static CharCreate_SetBlemishesOpacityDelegate CharCreate_SetBlemishesOpacity;
	public delegate void CharCreate_SetBlushDelegate(int value);
	public static CharCreate_SetBlushDelegate CharCreate_SetBlush;
	public delegate void CharCreate_SetBlushColorDelegate(int value);
	public static CharCreate_SetBlushColorDelegate CharCreate_SetBlushColor;
	public delegate void CharCreate_SetBlushColorHighlightsDelegate(int value);
	public static CharCreate_SetBlushColorHighlightsDelegate CharCreate_SetBlushColorHighlights;
	public delegate void CharCreate_SetBlushOpacityDelegate(float value);
	public static CharCreate_SetBlushOpacityDelegate CharCreate_SetBlushOpacity;
	public delegate void CharCreate_SetBodyBlemishesDelegate(int value);
	public static CharCreate_SetBodyBlemishesDelegate CharCreate_SetBodyBlemishes;
	public delegate void CharCreate_SetBodyBlemishesOpacityDelegate(float value);
	public static CharCreate_SetBodyBlemishesOpacityDelegate CharCreate_SetBodyBlemishesOpacity;
	public delegate void CharCreate_SetCharacterNameDelegate(string strName);
	public static CharCreate_SetCharacterNameDelegate CharCreate_SetCharacterName;
	public delegate void CharCreate_SetCheekboneHeightDelegate(float value);
	public static CharCreate_SetCheekboneHeightDelegate CharCreate_SetCheekboneHeight;
	public delegate void CharCreate_SetCheekWidthDelegate(float value);
	public static CharCreate_SetCheekWidthDelegate CharCreate_SetCheekWidth;
	public delegate void CharCreate_SetCheekWidthLowerDelegate(float value);
	public static CharCreate_SetCheekWidthLowerDelegate CharCreate_SetCheekWidthLower;
	public delegate void CharCreate_SetChestHairDelegate(int value);
	public static CharCreate_SetChestHairDelegate CharCreate_SetChestHair;
	public delegate void CharCreate_SetChestHairColorDelegate(int value);
	public static CharCreate_SetChestHairColorDelegate CharCreate_SetChestHairColor;
	public delegate void CharCreate_SetChestHairColorHighlightsDelegate(int value);
	public static CharCreate_SetChestHairColorHighlightsDelegate CharCreate_SetChestHairColorHighlights;
	public delegate void CharCreate_SetChestHairOpacityDelegate(float value);
	public static CharCreate_SetChestHairOpacityDelegate CharCreate_SetChestHairOpacity;
	public delegate void CharCreate_SetChinEffectDelegate(float value);
	public static CharCreate_SetChinEffectDelegate CharCreate_SetChinEffect;
	public delegate void CharCreate_SetChinSizeDelegate(float value);
	public static CharCreate_SetChinSizeDelegate CharCreate_SetChinSize;
	public delegate void CharCreate_SetChinSizeUnderneathDelegate(float value);
	public static CharCreate_SetChinSizeUnderneathDelegate CharCreate_SetChinSizeUnderneath;
	public delegate void CharCreate_SetChinWidthDelegate(float value);
	public static CharCreate_SetChinWidthDelegate CharCreate_SetChinWidth;
	public delegate void CharCreate_SetComplexionDelegate(int value);
	public static CharCreate_SetComplexionDelegate CharCreate_SetComplexion;
	public delegate void CharCreate_SetComplexionOpacityDelegate(float value);
	public static CharCreate_SetComplexionOpacityDelegate CharCreate_SetComplexionOpacity;
	public delegate void CharCreate_SetComponentDrawable_AccessoriesDelegate(int value);
	public static CharCreate_SetComponentDrawable_AccessoriesDelegate CharCreate_SetComponentDrawable_Accessories;
	public delegate void CharCreate_SetComponentDrawable_DecalsDelegate(int value);
	public static CharCreate_SetComponentDrawable_DecalsDelegate CharCreate_SetComponentDrawable_Decals;
	public delegate void CharCreate_SetComponentDrawable_FullBeardsDelegate(int value);
	public static CharCreate_SetComponentDrawable_FullBeardsDelegate CharCreate_SetComponentDrawable_FullBeards;
	public delegate void CharCreate_SetComponentDrawable_LegsDelegate(int value);
	public static CharCreate_SetComponentDrawable_LegsDelegate CharCreate_SetComponentDrawable_Legs;
	public delegate void CharCreate_SetComponentDrawable_ShoesDelegate(int value);
	public static CharCreate_SetComponentDrawable_ShoesDelegate CharCreate_SetComponentDrawable_Shoes;
	public delegate void CharCreate_SetComponentDrawable_TopsDelegate(int value);
	public static CharCreate_SetComponentDrawable_TopsDelegate CharCreate_SetComponentDrawable_Tops;
	public delegate void CharCreate_SetComponentDrawable_TorsoDelegate(int value);
	public static CharCreate_SetComponentDrawable_TorsoDelegate CharCreate_SetComponentDrawable_Torso;
	public delegate void CharCreate_SetComponentDrawable_UndershirtsDelegate(int value);
	public static CharCreate_SetComponentDrawable_UndershirtsDelegate CharCreate_SetComponentDrawable_Undershirts;
	public delegate void CharCreate_SetComponentPalette_AccessoriesDelegate(int value);
	public static CharCreate_SetComponentPalette_AccessoriesDelegate CharCreate_SetComponentPalette_Accessories;
	public delegate void CharCreate_SetComponentPalette_DecalsDelegate(int value);
	public static CharCreate_SetComponentPalette_DecalsDelegate CharCreate_SetComponentPalette_Decals;
	public delegate void CharCreate_SetComponentPalette_LegsDelegate(int value);
	public static CharCreate_SetComponentPalette_LegsDelegate CharCreate_SetComponentPalette_Legs;
	public delegate void CharCreate_SetComponentPalette_ShoesDelegate(int value);
	public static CharCreate_SetComponentPalette_ShoesDelegate CharCreate_SetComponentPalette_Shoes;
	public delegate void CharCreate_SetComponentPalette_TopsDelegate(int value);
	public static CharCreate_SetComponentPalette_TopsDelegate CharCreate_SetComponentPalette_Tops;
	public delegate void CharCreate_SetComponentPalette_TorsoDelegate(int value);
	public static CharCreate_SetComponentPalette_TorsoDelegate CharCreate_SetComponentPalette_Torso;
	public delegate void CharCreate_SetComponentPalette_UndershirtsDelegate(int value);
	public static CharCreate_SetComponentPalette_UndershirtsDelegate CharCreate_SetComponentPalette_Undershirts;
	public delegate void CharCreate_SetComponentTexture_AccessoriesDelegate(int value);
	public static CharCreate_SetComponentTexture_AccessoriesDelegate CharCreate_SetComponentTexture_Accessories;
	public delegate void CharCreate_SetComponentTexture_DecalsDelegate(int value);
	public static CharCreate_SetComponentTexture_DecalsDelegate CharCreate_SetComponentTexture_Decals;
	public delegate void CharCreate_SetComponentTexture_FullBeardsDelegate(int value);
	public static CharCreate_SetComponentTexture_FullBeardsDelegate CharCreate_SetComponentTexture_FullBeards;
	public delegate void CharCreate_SetComponentTexture_LegsDelegate(int value);
	public static CharCreate_SetComponentTexture_LegsDelegate CharCreate_SetComponentTexture_Legs;
	public delegate void CharCreate_SetComponentTexture_ShoesDelegate(int value);
	public static CharCreate_SetComponentTexture_ShoesDelegate CharCreate_SetComponentTexture_Shoes;
	public delegate void CharCreate_SetComponentTexture_TopsDelegate(int value);
	public static CharCreate_SetComponentTexture_TopsDelegate CharCreate_SetComponentTexture_Tops;
	public delegate void CharCreate_SetComponentTexture_TorsoDelegate(int value);
	public static CharCreate_SetComponentTexture_TorsoDelegate CharCreate_SetComponentTexture_Torso;
	public delegate void CharCreate_SetComponentTexture_UndershirtsDelegate(int value);
	public static CharCreate_SetComponentTexture_UndershirtsDelegate CharCreate_SetComponentTexture_Undershirts;
	public delegate void CharCreate_SetExtraBodyBlemishesDelegate(int value);
	public static CharCreate_SetExtraBodyBlemishesDelegate CharCreate_SetExtraBodyBlemishes;
	public delegate void CharCreate_SetExtraBodyBlemishesOpacityDelegate(float value);
	public static CharCreate_SetExtraBodyBlemishesOpacityDelegate CharCreate_SetExtraBodyBlemishesOpacity;
	public delegate void CharCreate_SetEyebrowDepthDelegate(float value);
	public static CharCreate_SetEyebrowDepthDelegate CharCreate_SetEyebrowDepth;
	public delegate void CharCreate_SetEyebrowHeightDelegate(float value);
	public static CharCreate_SetEyebrowHeightDelegate CharCreate_SetEyebrowHeight;
	public delegate void CharCreate_SetEyeBrowsDelegate(int value);
	public static CharCreate_SetEyeBrowsDelegate CharCreate_SetEyeBrows;
	public delegate void CharCreate_SetEyeBrowsColorDelegate(int value);
	public static CharCreate_SetEyeBrowsColorDelegate CharCreate_SetEyeBrowsColor;
	public delegate void CharCreate_SetEyeBrowsColorHighlightsDelegate(int value);
	public static CharCreate_SetEyeBrowsColorHighlightsDelegate CharCreate_SetEyeBrowsColorHighlights;
	public delegate void CharCreate_SetEyeBrowsOpacityDelegate(float value);
	public static CharCreate_SetEyeBrowsOpacityDelegate CharCreate_SetEyeBrowsOpacity;
	public delegate void CharCreate_SetEyeColorDelegate(int value);
	public static CharCreate_SetEyeColorDelegate CharCreate_SetEyeColor;
	public delegate void CharCreate_SetEyeSizeDelegate(float value);
	public static CharCreate_SetEyeSizeDelegate CharCreate_SetEyeSize;
	public delegate void CharCreate_SetFaceShapeDelegate(int index, int gender, int value);
	public static CharCreate_SetFaceShapeDelegate CharCreate_SetFaceShape;
	public delegate void CharCreate_SetFacialHairDelegate(int value);
	public static CharCreate_SetFacialHairDelegate CharCreate_SetFacialHair;
	public delegate void CharCreate_SetFacialHairColorDelegate(int value);
	public static CharCreate_SetFacialHairColorDelegate CharCreate_SetFacialHairColor;
	public delegate void CharCreate_SetFacialHairColorHighlightsDelegate(int value);
	public static CharCreate_SetFacialHairColorHighlightsDelegate CharCreate_SetFacialHairColorHighlights;
	public delegate void CharCreate_SetFacialHairOpacityDelegate(float value);
	public static CharCreate_SetFacialHairOpacityDelegate CharCreate_SetFacialHairOpacity;
	public delegate void CharCreate_SetGenderDelegate(EGender a_Gender);
	public static CharCreate_SetGenderDelegate CharCreate_SetGender;
	public delegate void CharCreate_SetHairColorDelegate(int value);
	public static CharCreate_SetHairColorDelegate CharCreate_SetHairColor;
	public delegate void CharCreate_SetHairColorHighlightsDelegate(int value);
	public static CharCreate_SetHairColorHighlightsDelegate CharCreate_SetHairColorHighlights;
	public delegate void CharCreate_SetHairStyleDrawableDelegate(int value);
	public static CharCreate_SetHairStyleDrawableDelegate CharCreate_SetHairStyleDrawable;
	public delegate void CharCreate_SetLanguageDelegate(string language);
	public static CharCreate_SetLanguageDelegate CharCreate_SetLanguage;
	public delegate void CharCreate_SetLipSizeDelegate(float value);
	public static CharCreate_SetLipSizeDelegate CharCreate_SetLipSize;
	public delegate void CharCreate_SetLipstickDelegate(int value);
	public static CharCreate_SetLipstickDelegate CharCreate_SetLipstick;
	public delegate void CharCreate_SetLipstickColorDelegate(int value);
	public static CharCreate_SetLipstickColorDelegate CharCreate_SetLipstickColor;
	public delegate void CharCreate_SetLipstickColorHighlightsDelegate(int value);
	public static CharCreate_SetLipstickColorHighlightsDelegate CharCreate_SetLipstickColorHighlights;
	public delegate void CharCreate_SetLipstickOpacityDelegate(float value);
	public static CharCreate_SetLipstickOpacityDelegate CharCreate_SetLipstickOpacity;
	public delegate void CharCreate_SetMakeupDelegate(int value);
	public static CharCreate_SetMakeupDelegate CharCreate_SetMakeup;
	public delegate void CharCreate_SetMakeupColorDelegate(int value);
	public static CharCreate_SetMakeupColorDelegate CharCreate_SetMakeupColor;
	public delegate void CharCreate_SetMakeupColorHighlightsDelegate(int value);
	public static CharCreate_SetMakeupColorHighlightsDelegate CharCreate_SetMakeupColorHighlights;
	public delegate void CharCreate_SetMakeupOpacityDelegate(float value);
	public static CharCreate_SetMakeupOpacityDelegate CharCreate_SetMakeupOpacity;
	public delegate void CharCreate_SetMolesFrecklesDelegate(int value);
	public static CharCreate_SetMolesFrecklesDelegate CharCreate_SetMolesFreckles;
	public delegate void CharCreate_SetMolesFrecklesOpacityDelegate(float value);
	public static CharCreate_SetMolesFrecklesOpacityDelegate CharCreate_SetMolesFrecklesOpacity;
	public delegate void CharCreate_SetMouthSizeDelegate(float value);
	public static CharCreate_SetMouthSizeDelegate CharCreate_SetMouthSize;
	public delegate void CharCreate_SetMouthSizeLowerDelegate(float value);
	public static CharCreate_SetMouthSizeLowerDelegate CharCreate_SetMouthSizeLower;
	public delegate void CharCreate_SetNeckWidthDelegate(float value);
	public static CharCreate_SetNeckWidthDelegate CharCreate_SetNeckWidth;
	public delegate void CharCreate_SetNeckWidthLowerDelegate(float value);
	public static CharCreate_SetNeckWidthLowerDelegate CharCreate_SetNeckWidthLower;
	public delegate void CharCreate_SetNoseAngleDelegate(float value);
	public static CharCreate_SetNoseAngleDelegate CharCreate_SetNoseAngle;
	public delegate void CharCreate_SetNoseSizeHorizontalDelegate(float value);
	public static CharCreate_SetNoseSizeHorizontalDelegate CharCreate_SetNoseSizeHorizontal;
	public delegate void CharCreate_SetNoseSizeOutwardsDelegate(float value);
	public static CharCreate_SetNoseSizeOutwardsDelegate CharCreate_SetNoseSizeOutwards;
	public delegate void CharCreate_SetNoseSizeOutwardsLowerDelegate(float value);
	public static CharCreate_SetNoseSizeOutwardsLowerDelegate CharCreate_SetNoseSizeOutwardsLower;
	public delegate void CharCreate_SetNoseSizeOutwardsUpperDelegate(float value);
	public static CharCreate_SetNoseSizeOutwardsUpperDelegate CharCreate_SetNoseSizeOutwardsUpper;
	public delegate void CharCreate_SetNoseSizeVerticalDelegate(float value);
	public static CharCreate_SetNoseSizeVerticalDelegate CharCreate_SetNoseSizeVertical;
	public delegate void CharCreate_SetPropDrawable_BraceletsDelegate(int value);
	public static CharCreate_SetPropDrawable_BraceletsDelegate CharCreate_SetPropDrawable_Bracelets;
	public delegate void CharCreate_SetPropDrawable_EarringsDelegate(int value);
	public static CharCreate_SetPropDrawable_EarringsDelegate CharCreate_SetPropDrawable_Earrings;
	public delegate void CharCreate_SetPropDrawable_GlassesDelegate(int value);
	public static CharCreate_SetPropDrawable_GlassesDelegate CharCreate_SetPropDrawable_Glasses;
	public delegate void CharCreate_SetPropDrawable_HatsDelegate(int value);
	public static CharCreate_SetPropDrawable_HatsDelegate CharCreate_SetPropDrawable_Hats;
	public delegate void CharCreate_SetPropDrawable_WatchesDelegate(int value);
	public static CharCreate_SetPropDrawable_WatchesDelegate CharCreate_SetPropDrawable_Watches;
	public delegate void CharCreate_SetPropTexture_BraceletsDelegate(int value);
	public static CharCreate_SetPropTexture_BraceletsDelegate CharCreate_SetPropTexture_Bracelets;
	public delegate void CharCreate_SetPropTexture_EarringsDelegate(int value);
	public static CharCreate_SetPropTexture_EarringsDelegate CharCreate_SetPropTexture_Earrings;
	public delegate void CharCreate_SetPropTexture_GlassesDelegate(int value);
	public static CharCreate_SetPropTexture_GlassesDelegate CharCreate_SetPropTexture_Glasses;
	public delegate void CharCreate_SetPropTexture_HatsDelegate(int value);
	public static CharCreate_SetPropTexture_HatsDelegate CharCreate_SetPropTexture_Hats;
	public delegate void CharCreate_SetPropTexture_WatchesDelegate(int value);
	public static CharCreate_SetPropTexture_WatchesDelegate CharCreate_SetPropTexture_Watches;
	public delegate void CharCreate_SetSecondLanguageDelegate(string language);
	public static CharCreate_SetSecondLanguageDelegate CharCreate_SetSecondLanguage;
	public delegate void CharCreate_SetSkinIDDelegate(int skinID);
	public static CharCreate_SetSkinIDDelegate CharCreate_SetSkinID;
	public delegate void CharCreate_SetSkinPercentage_ColorDelegate(float value);
	public static CharCreate_SetSkinPercentage_ColorDelegate CharCreate_SetSkinPercentage_Color;
	public delegate void CharCreate_SetSkinPercentage_ShapeDelegate(float value);
	public static CharCreate_SetSkinPercentage_ShapeDelegate CharCreate_SetSkinPercentage_Shape;
	public delegate void CharCreate_SetSpawnDelegate(EScriptLocation a_SpawnLocation);
	public static CharCreate_SetSpawnDelegate CharCreate_SetSpawn;
	public delegate void CharCreate_SetSunDamageDelegate(int value);
	public static CharCreate_SetSunDamageDelegate CharCreate_SetSunDamage;
	public delegate void CharCreate_SetSunDamageOpacityDelegate(float value);
	public static CharCreate_SetSunDamageOpacityDelegate CharCreate_SetSunDamageOpacity;
	public delegate void CharCreate_SetTypeDelegate(ECharacterType a_Type);
	public static CharCreate_SetTypeDelegate CharCreate_SetType;
	public delegate void CharCreate_StartRotationDelegate(int direction);
	public static CharCreate_StartRotationDelegate CharCreate_StartRotation;
	public delegate void CharCreate_StopRotationDelegate(int direction);
	public static CharCreate_StopRotationDelegate CharCreate_StopRotation;
	public delegate void CharCreate_Tattoo_AddNewDelegate();
	public static CharCreate_Tattoo_AddNewDelegate CharCreate_Tattoo_AddNew;
	public delegate void CharCreate_Tattoo_CancelDelegate();
	public static CharCreate_Tattoo_CancelDelegate CharCreate_Tattoo_Cancel;
	public delegate void CharCreate_Tattoo_ChangeTattooDelegate(int tattooID);
	public static CharCreate_Tattoo_ChangeTattooDelegate CharCreate_Tattoo_ChangeTattoo;
	public delegate void CharCreate_Tattoo_ChangeZoneDelegate(TattooZone tattooZone);
	public static CharCreate_Tattoo_ChangeZoneDelegate CharCreate_Tattoo_ChangeZone;
	public delegate void CharCreate_Tattoo_CreateDelegate();
	public static CharCreate_Tattoo_CreateDelegate CharCreate_Tattoo_Create;
	public delegate void CharCreate_Tattoo_RemoveTattooDelegate(string strElementName, int tattooID);
	public static CharCreate_Tattoo_RemoveTattooDelegate CharCreate_Tattoo_RemoveTattoo;
	public delegate void CharCreate_ToggleClothesDelegate();
	public static CharCreate_ToggleClothesDelegate CharCreate_ToggleClothes;
	public delegate void ChipManagement_Buy_CancelDelegate();
	public static ChipManagement_Buy_CancelDelegate ChipManagement_Buy_Cancel;
	public delegate void ChipManagement_Buy_SubmitDelegate(string strInput);
	public static ChipManagement_Buy_SubmitDelegate ChipManagement_Buy_Submit;
	public delegate void ChipManagement_Sell_CancelDelegate();
	public static ChipManagement_Sell_CancelDelegate ChipManagement_Sell_Cancel;
	public delegate void ChipManagement_Sell_SubmitDelegate(string strInput);
	public static ChipManagement_Sell_SubmitDelegate ChipManagement_Sell_Submit;
	public delegate void CloseAnimationUIDelegate();
	public static CloseAnimationUIDelegate CloseAnimationUI;
	public delegate void CloseAssetTransferDelegate();
	public static CloseAssetTransferDelegate CloseAssetTransfer;
	public delegate void CloseCheckDelegate();
	public static CloseCheckDelegate CloseCheck;
	public delegate void CloseCheckIntDelegate();
	public static CloseCheckIntDelegate CloseCheckInt;
	public delegate void CloseCheckVehDelegate();
	public static CloseCheckVehDelegate CloseCheckVeh;
	public delegate void CloseDonationsDelegate();
	public static CloseDonationsDelegate CloseDonations;
	public delegate void CloseF10MenuDelegate();
	public static CloseF10MenuDelegate CloseF10Menu;
	public delegate void CloseItemsListUIDelegate();
	public static CloseItemsListUIDelegate CloseItemsListUI;
	public delegate void ClosePhoneDelegate();
	public static ClosePhoneDelegate ClosePhone;
	public delegate void CloseRadioManagementDelegate();
	public static CloseRadioManagementDelegate CloseRadioManagement;
	public delegate void CloseVehiclesListUIDelegate();
	public static CloseVehiclesListUIDelegate CloseVehiclesListUI;
	public delegate void ClothingStoreSelector_ExitDelegate();
	public static ClothingStoreSelector_ExitDelegate ClothingStoreSelector_Exit;
	public delegate void ClothingStoreSelector_GotoClothingStoreDelegate();
	public static ClothingStoreSelector_GotoClothingStoreDelegate ClothingStoreSelector_GotoClothingStore;
	public delegate void ClothingStoreSelector_GotoOutfitEditorDelegate();
	public static ClothingStoreSelector_GotoOutfitEditorDelegate ClothingStoreSelector_GotoOutfitEditor;
	public delegate void ClothingStore_CheckoutDelegate();
	public static ClothingStore_CheckoutDelegate ClothingStore_Checkout;
	public delegate void ClothingStore_ExitDelegate();
	public static ClothingStore_ExitDelegate ClothingStore_Exit;
	public delegate void ClothingStore_GetPriceDetailsDelegate();
	public static ClothingStore_GetPriceDetailsDelegate ClothingStore_GetPriceDetails;
	public delegate void ClothingStore_OnRootChanged_AccessoriesDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_AccessoriesDelegate ClothingStore_OnRootChanged_Accessories;
	public delegate void ClothingStore_OnRootChanged_BraceletsDelegate(string strElementName);
	public static ClothingStore_OnRootChanged_BraceletsDelegate ClothingStore_OnRootChanged_Bracelets;
	public delegate void ClothingStore_OnRootChanged_DecalsDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_DecalsDelegate ClothingStore_OnRootChanged_Decals;
	public delegate void ClothingStore_OnRootChanged_EarringsDelegate(string strElementName);
	public static ClothingStore_OnRootChanged_EarringsDelegate ClothingStore_OnRootChanged_Earrings;
	public delegate void ClothingStore_OnRootChanged_GlassesDelegate(string strElementName);
	public static ClothingStore_OnRootChanged_GlassesDelegate ClothingStore_OnRootChanged_Glasses;
	public delegate void ClothingStore_OnRootChanged_HatsDelegate(string strElementName);
	public static ClothingStore_OnRootChanged_HatsDelegate ClothingStore_OnRootChanged_Hats;
	public delegate void ClothingStore_OnRootChanged_LegsDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_LegsDelegate ClothingStore_OnRootChanged_Legs;
	public delegate void ClothingStore_OnRootChanged_MasksDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_MasksDelegate ClothingStore_OnRootChanged_Masks;
	public delegate void ClothingStore_OnRootChanged_ShoesDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_ShoesDelegate ClothingStore_OnRootChanged_Shoes;
	public delegate void ClothingStore_OnRootChanged_TopsDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_TopsDelegate ClothingStore_OnRootChanged_Tops;
	public delegate void ClothingStore_OnRootChanged_TorsoDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_TorsoDelegate ClothingStore_OnRootChanged_Torso;
	public delegate void ClothingStore_OnRootChanged_UndershirtsDelegate(string strElementToReset);
	public static ClothingStore_OnRootChanged_UndershirtsDelegate ClothingStore_OnRootChanged_Undershirts;
	public delegate void ClothingStore_OnRootChanged_WatchesDelegate(string strElementName);
	public static ClothingStore_OnRootChanged_WatchesDelegate ClothingStore_OnRootChanged_Watches;
	public delegate void ClothingStore_SetClothingDelegate(int index, int value);
	public static ClothingStore_SetClothingDelegate ClothingStore_SetClothing;
	public delegate void ClothingStore_SetComponentDrawable_AccessoriesDelegate(int value);
	public static ClothingStore_SetComponentDrawable_AccessoriesDelegate ClothingStore_SetComponentDrawable_Accessories;
	public delegate void ClothingStore_SetComponentDrawable_DecalsDelegate(int value);
	public static ClothingStore_SetComponentDrawable_DecalsDelegate ClothingStore_SetComponentDrawable_Decals;
	public delegate void ClothingStore_SetComponentDrawable_LegsDelegate(int value);
	public static ClothingStore_SetComponentDrawable_LegsDelegate ClothingStore_SetComponentDrawable_Legs;
	public delegate void ClothingStore_SetComponentDrawable_MasksDelegate(int value);
	public static ClothingStore_SetComponentDrawable_MasksDelegate ClothingStore_SetComponentDrawable_Masks;
	public delegate void ClothingStore_SetComponentDrawable_ShoesDelegate(int value);
	public static ClothingStore_SetComponentDrawable_ShoesDelegate ClothingStore_SetComponentDrawable_Shoes;
	public delegate void ClothingStore_SetComponentDrawable_TopsDelegate(int value);
	public static ClothingStore_SetComponentDrawable_TopsDelegate ClothingStore_SetComponentDrawable_Tops;
	public delegate void ClothingStore_SetComponentDrawable_TorsoDelegate(int value);
	public static ClothingStore_SetComponentDrawable_TorsoDelegate ClothingStore_SetComponentDrawable_Torso;
	public delegate void ClothingStore_SetComponentDrawable_UndershirtsDelegate(int value);
	public static ClothingStore_SetComponentDrawable_UndershirtsDelegate ClothingStore_SetComponentDrawable_Undershirts;
	public delegate void ClothingStore_SetComponentTexture_AccessoriesDelegate(int value);
	public static ClothingStore_SetComponentTexture_AccessoriesDelegate ClothingStore_SetComponentTexture_Accessories;
	public delegate void ClothingStore_SetComponentTexture_DecalsDelegate(int value);
	public static ClothingStore_SetComponentTexture_DecalsDelegate ClothingStore_SetComponentTexture_Decals;
	public delegate void ClothingStore_SetComponentTexture_LegsDelegate(int value);
	public static ClothingStore_SetComponentTexture_LegsDelegate ClothingStore_SetComponentTexture_Legs;
	public delegate void ClothingStore_SetComponentTexture_MasksDelegate(int value);
	public static ClothingStore_SetComponentTexture_MasksDelegate ClothingStore_SetComponentTexture_Masks;
	public delegate void ClothingStore_SetComponentTexture_ShoesDelegate(int value);
	public static ClothingStore_SetComponentTexture_ShoesDelegate ClothingStore_SetComponentTexture_Shoes;
	public delegate void ClothingStore_SetComponentTexture_TopsDelegate(int value);
	public static ClothingStore_SetComponentTexture_TopsDelegate ClothingStore_SetComponentTexture_Tops;
	public delegate void ClothingStore_SetComponentTexture_TorsoDelegate(int value);
	public static ClothingStore_SetComponentTexture_TorsoDelegate ClothingStore_SetComponentTexture_Torso;
	public delegate void ClothingStore_SetComponentTexture_UndershirtsDelegate(int value);
	public static ClothingStore_SetComponentTexture_UndershirtsDelegate ClothingStore_SetComponentTexture_Undershirts;
	public delegate void ClothingStore_SetPropDelegate(int index, int value);
	public static ClothingStore_SetPropDelegate ClothingStore_SetProp;
	public delegate void ClothingStore_SetPropDrawable_BraceletsDelegate(int value);
	public static ClothingStore_SetPropDrawable_BraceletsDelegate ClothingStore_SetPropDrawable_Bracelets;
	public delegate void ClothingStore_SetPropDrawable_EarringsDelegate(int value);
	public static ClothingStore_SetPropDrawable_EarringsDelegate ClothingStore_SetPropDrawable_Earrings;
	public delegate void ClothingStore_SetPropDrawable_GlassesDelegate(int value);
	public static ClothingStore_SetPropDrawable_GlassesDelegate ClothingStore_SetPropDrawable_Glasses;
	public delegate void ClothingStore_SetPropDrawable_HatsDelegate(int value);
	public static ClothingStore_SetPropDrawable_HatsDelegate ClothingStore_SetPropDrawable_Hats;
	public delegate void ClothingStore_SetPropDrawable_WatchesDelegate(int value);
	public static ClothingStore_SetPropDrawable_WatchesDelegate ClothingStore_SetPropDrawable_Watches;
	public delegate void ClothingStore_SetPropTexture_BraceletsDelegate(int value);
	public static ClothingStore_SetPropTexture_BraceletsDelegate ClothingStore_SetPropTexture_Bracelets;
	public delegate void ClothingStore_SetPropTexture_EarringsDelegate(int value);
	public static ClothingStore_SetPropTexture_EarringsDelegate ClothingStore_SetPropTexture_Earrings;
	public delegate void ClothingStore_SetPropTexture_GlassesDelegate(int value);
	public static ClothingStore_SetPropTexture_GlassesDelegate ClothingStore_SetPropTexture_Glasses;
	public delegate void ClothingStore_SetPropTexture_HatsDelegate(int value);
	public static ClothingStore_SetPropTexture_HatsDelegate ClothingStore_SetPropTexture_Hats;
	public delegate void ClothingStore_SetPropTexture_WatchesDelegate(int value);
	public static ClothingStore_SetPropTexture_WatchesDelegate ClothingStore_SetPropTexture_Watches;
	public delegate void ClothingStore_SetSkinIDDelegate(int index);
	public static ClothingStore_SetSkinIDDelegate ClothingStore_SetSkinID;
	public delegate void ConsumeDonationPerkDelegate(UInt32 dbid);
	public static ConsumeDonationPerkDelegate ConsumeDonationPerk;
	public delegate void CopyToClipboardItemValueDelegate();
	public static CopyToClipboardItemValueDelegate CopyToClipboardItemValue;
	public delegate void CreateCustomAnimationDelegate(string commandName, string animDictionary, string animName, bool loop, bool stopOnLastFrame, bool onlyAnimateUpperBody, bool allowPlayerMovement, int durationSeconds);
	public static CreateCustomAnimationDelegate CreateCustomAnimation;
	public delegate void CreateFactionDelegate(string strFullName, string strShortName, string strFactionType);
	public static CreateFactionDelegate CreateFaction;
	public delegate void CreateInfoMarker_CancelDelegate();
	public static CreateInfoMarker_CancelDelegate CreateInfoMarker_Cancel;
	public delegate void CreateInfoMarker_SubmitDelegate(string strInput);
	public static CreateInfoMarker_SubmitDelegate CreateInfoMarker_Submit;
	public delegate void CreateKeybindDelegate(ConsoleKey key, EPlayerKeyBindType bindType, string strAction);
	public static CreateKeybindDelegate CreateKeybind;
	public delegate void CreatePhoneMessageDelegate(string to, string content);
	public static CreatePhoneMessageDelegate CreatePhoneMessage;
	public delegate void CustomAnimationDeletion_NoDelegate();
	public static CustomAnimationDeletion_NoDelegate CustomAnimationDeletion_No;
	public delegate void CustomAnimationDeletion_YesDelegate();
	public static CustomAnimationDeletion_YesDelegate CustomAnimationDeletion_Yes;
	public delegate void CustomInterior_CloseWindowDelegate();
	public static CustomInterior_CloseWindowDelegate CustomInterior_CloseWindow;
	public delegate void CustomInterior_Confirmation_NoDelegate();
	public static CustomInterior_Confirmation_NoDelegate CustomInterior_Confirmation_No;
	public delegate void CustomInterior_Confirmation_YesDelegate();
	public static CustomInterior_Confirmation_YesDelegate CustomInterior_Confirmation_Yes;
	public delegate void CustomInterior_ProcessCustomInteriorDelegate(string mapData, string mapType, float markerX, float markerY, float markerZ);
	public static CustomInterior_ProcessCustomInteriorDelegate CustomInterior_ProcessCustomInterior;
	public delegate void DeclinePDTicketDelegate();
	public static DeclinePDTicketDelegate DeclinePDTicket;
	public delegate void DeleteCustomAnimCmdDelegate(string commandName);
	public static DeleteCustomAnimCmdDelegate DeleteCustomAnimCmd;
	public delegate void DeleteKeybindDelegate(int index);
	public static DeleteKeybindDelegate DeleteKeybind;
	public delegate void DenyApplicationDelegate(int accountID);
	public static DenyApplicationDelegate DenyApplication;
	public delegate void DisbandFactionDelegate(int factionIndex);
	public static DisbandFactionDelegate DisbandFaction;
	public delegate void DiscordGetURLHackDelegate(string strURL);
	public static DiscordGetURLHackDelegate DiscordGetURLHack;
	public delegate void DoLoginDelegate(string strUsername, string strPassword, bool bAutoLogin);
	public static DoLoginDelegate DoLogin;
	public delegate void Donation_ChangeInactivityEntityDelegate(Int64 entityID);
	public static Donation_ChangeInactivityEntityDelegate Donation_ChangeInactivityEntity;
	public delegate void Donation_ChangeInactivityLengthDelegate(int inactivityLength);
	public static Donation_ChangeInactivityLengthDelegate Donation_ChangeInactivityLength;
	public delegate void Donation_ChangeInactivityTypeDelegate(EDonationInactivityEntityType entityType);
	public static Donation_ChangeInactivityTypeDelegate Donation_ChangeInactivityType;
	public delegate void DoRegisterDelegate(string strUsername, string strPassword, string strPasswordVerify, string strEmail);
	public static DoRegisterDelegate DoRegister;
	public delegate void DummyDelegate();
	public static DummyDelegate Dummy;
	public delegate void DutyOutfitEditor_DeleteOutfitDelegate();
	public static DutyOutfitEditor_DeleteOutfitDelegate DutyOutfitEditor_DeleteOutfit;
	public delegate void DutyOutfitEditor_EditOutfitDelegate(string strElementName, int outfitIndex);
	public static DutyOutfitEditor_EditOutfitDelegate DutyOutfitEditor_EditOutfit;
	public delegate void DutyOutfitEditor_Loadout_SetAccessory1Delegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetAccessory1Delegate DutyOutfitEditor_Loadout_SetAccessory1;
	public delegate void DutyOutfitEditor_Loadout_SetAccessory2Delegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetAccessory2Delegate DutyOutfitEditor_Loadout_SetAccessory2;
	public delegate void DutyOutfitEditor_Loadout_SetAccessory3Delegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetAccessory3Delegate DutyOutfitEditor_Loadout_SetAccessory3;
	public delegate void DutyOutfitEditor_Loadout_SetHandgun1Delegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetHandgun1Delegate DutyOutfitEditor_Loadout_SetHandgun1;
	public delegate void DutyOutfitEditor_Loadout_SetHandgun2Delegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetHandgun2Delegate DutyOutfitEditor_Loadout_SetHandgun2;
	public delegate void DutyOutfitEditor_Loadout_SetLargeCarriedItemDelegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetLargeCarriedItemDelegate DutyOutfitEditor_Loadout_SetLargeCarriedItem;
	public delegate void DutyOutfitEditor_Loadout_SetLargeWeaponDelegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetLargeWeaponDelegate DutyOutfitEditor_Loadout_SetLargeWeapon;
	public delegate void DutyOutfitEditor_Loadout_SetMeleeDelegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetMeleeDelegate DutyOutfitEditor_Loadout_SetMelee;
	public delegate void DutyOutfitEditor_Loadout_SetProjectileDelegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetProjectileDelegate DutyOutfitEditor_Loadout_SetProjectile;
	public delegate void DutyOutfitEditor_Loadout_SetProjectile2Delegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetProjectile2Delegate DutyOutfitEditor_Loadout_SetProjectile2;
	public delegate void DutyOutfitEditor_Loadout_SetPursuitAccessoryDelegate(string value, string displayName);
	public static DutyOutfitEditor_Loadout_SetPursuitAccessoryDelegate DutyOutfitEditor_Loadout_SetPursuitAccessory;
	public delegate void DutyOutfitEditor_Outfit_OnMouseEnterDelegate(string strElementName, int outfitIndex);
	public static DutyOutfitEditor_Outfit_OnMouseEnterDelegate DutyOutfitEditor_Outfit_OnMouseEnter;
	public delegate void DutyOutfitEditor_Outfit_OnMouseExitDelegate(string strElementName, int outfitIndex);
	public static DutyOutfitEditor_Outfit_OnMouseExitDelegate DutyOutfitEditor_Outfit_OnMouseExit;
	public delegate void DutyOutfitEditor_SelectPreset_CancelDelegate();
	public static DutyOutfitEditor_SelectPreset_CancelDelegate DutyOutfitEditor_SelectPreset_Cancel;
	public delegate void DutyOutfitEditor_SelectPreset_DoneDelegate(string displayName, string value);
	public static DutyOutfitEditor_SelectPreset_DoneDelegate DutyOutfitEditor_SelectPreset_Done;
	public delegate void DutyOutfitEditor_SelectPreset_DropdownSelectionChangedDelegate(string displayName, string value);
	public static DutyOutfitEditor_SelectPreset_DropdownSelectionChangedDelegate DutyOutfitEditor_SelectPreset_DropdownSelectionChanged;
	public delegate void DutyOutfitEditor_SetAccessoryDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetAccessoryDelegate DutyOutfitEditor_SetAccessory;
	public delegate void DutyOutfitEditor_SetBagsAndParachutesDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetBagsAndParachutesDelegate DutyOutfitEditor_SetBagsAndParachutes;
	public delegate void DutyOutfitEditor_SetBodyArmorDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetBodyArmorDelegate DutyOutfitEditor_SetBodyArmor;
	public delegate void DutyOutfitEditor_SetBraceletsDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetBraceletsDelegate DutyOutfitEditor_SetBracelets;
	public delegate void DutyOutfitEditor_SetDecalsDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetDecalsDelegate DutyOutfitEditor_SetDecals;
	public delegate void DutyOutfitEditor_SetEarsDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetEarsDelegate DutyOutfitEditor_SetEars;
	public delegate void DutyOutfitEditor_SetGlassesDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetGlassesDelegate DutyOutfitEditor_SetGlasses;
	public delegate void DutyOutfitEditor_SetHairVisibleDelegate(bool bVisible);
	public static DutyOutfitEditor_SetHairVisibleDelegate DutyOutfitEditor_SetHairVisible;
	public delegate void DutyOutfitEditor_SetHatDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetHatDelegate DutyOutfitEditor_SetHat;
	public delegate void DutyOutfitEditor_SetLegsDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetLegsDelegate DutyOutfitEditor_SetLegs;
	public delegate void DutyOutfitEditor_SetMaskDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetMaskDelegate DutyOutfitEditor_SetMask;
	public delegate void DutyOutfitEditor_SetOutfitNameDelegate(string strName);
	public static DutyOutfitEditor_SetOutfitNameDelegate DutyOutfitEditor_SetOutfitName;
	public delegate void DutyOutfitEditor_SetShoesDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetShoesDelegate DutyOutfitEditor_SetShoes;
	public delegate void DutyOutfitEditor_SetTopDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetTopDelegate DutyOutfitEditor_SetTop;
	public delegate void DutyOutfitEditor_SetTorsoDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetTorsoDelegate DutyOutfitEditor_SetTorso;
	public delegate void DutyOutfitEditor_SetUndershirtDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetUndershirtDelegate DutyOutfitEditor_SetUndershirt;
	public delegate void DutyOutfitEditor_SetWatchesDelegate(string value, string displayName);
	public static DutyOutfitEditor_SetWatchesDelegate DutyOutfitEditor_SetWatches;
	public delegate void Duty_GotoOutfitsDelegate();
	public static Duty_GotoOutfitsDelegate Duty_GotoOutfits;
	public delegate void Duty_SelectOutfit_DropdownSelectionChangedDelegate(string displayName, string value);
	public static Duty_SelectOutfit_DropdownSelectionChangedDelegate Duty_SelectOutfit_DropdownSelectionChanged;
	public delegate void Duty_SelectOutfit_DropdownSelectionHoverChangedDelegate(string displayName, string value);
	public static Duty_SelectOutfit_DropdownSelectionHoverChangedDelegate Duty_SelectOutfit_DropdownSelectionHoverChanged;
	public delegate void EditFactionMessageDelegate(int factionIndex, string strMessage);
	public static EditFactionMessageDelegate EditFactionMessage;
	public delegate void EditInterior_HideDelegate();
	public static EditInterior_HideDelegate EditInterior_Hide;
	public delegate void EditInterior_OnChangeClassDelegate(string strClassName);
	public static EditInterior_OnChangeClassDelegate EditInterior_OnChangeClass;
	public delegate void EditInterior_OnChangeCurrentFurnitureItemDelegate(int index);
	public static EditInterior_OnChangeCurrentFurnitureItemDelegate EditInterior_OnChangeCurrentFurnitureItem;
	public delegate void EditInterior_OnChangeFurnitureItemDelegate(int index);
	public static EditInterior_OnChangeFurnitureItemDelegate EditInterior_OnChangeFurnitureItem;
	public delegate void EditInterior_OnChangeRemovedFurnitureItemDelegate(int index);
	public static EditInterior_OnChangeRemovedFurnitureItemDelegate EditInterior_OnChangeRemovedFurnitureItem;
	public delegate void EditInterior_PickupFurnitureDelegate();
	public static EditInterior_PickupFurnitureDelegate EditInterior_PickupFurniture;
	public delegate void EditInterior_PlaceFurnitureDelegate();
	public static EditInterior_PlaceFurnitureDelegate EditInterior_PlaceFurniture;
	public delegate void EditInterior_RestoreFurnitureDelegate();
	public static EditInterior_RestoreFurnitureDelegate EditInterior_RestoreFurniture;
	public delegate void EditRadioDelegate(long a_RadioID);
	public static EditRadioDelegate EditRadio;
	public delegate void EndCallDelegate();
	public static EndCallDelegate EndCall;
	public delegate void ExitAchievementsDelegate();
	public static ExitAchievementsDelegate ExitAchievements;
	public delegate void ExitCharCreateDelegate();
	public static ExitCharCreateDelegate ExitCharCreate;
	public delegate void ExitPendingAppsScreenDelegate();
	public static ExitPendingAppsScreenDelegate ExitPendingAppsScreen;
	public delegate void ExpandContainerDelegate(int item_index, EItemSocket socket_id, bool bClosewindowIfAlreadyExists);
	public static ExpandContainerDelegate ExpandContainer;
	public delegate void ExtendRadio30DaysDelegate(int a_RadioID);
	public static ExtendRadio30DaysDelegate ExtendRadio30Days;
	public delegate void ExtendRadio7DaysDelegate(int a_RadioID);
	public static ExtendRadio7DaysDelegate ExtendRadio7Days;
	public delegate void FactionInvite_AcceptDelegate();
	public static FactionInvite_AcceptDelegate FactionInvite_Accept;
	public delegate void FactionInvite_DeclineDelegate();
	public static FactionInvite_DeclineDelegate FactionInvite_Decline;
	public delegate void FactionVehicleList_HideDelegate();
	public static FactionVehicleList_HideDelegate FactionVehicleList_Hide;
	public delegate void FadeOutWeaponSelectorDelegate();
	public static FadeOutWeaponSelectorDelegate FadeOutWeaponSelector;
	public delegate void FinalizeLicenseDeviceDelegate(string strTargetName, EWeaponLicenseType weaponLicenseType);
	public static FinalizeLicenseDeviceDelegate FinalizeLicenseDevice;
	public delegate void FindPlayerForReportDelegate(string strPartialName);
	public static FindPlayerForReportDelegate FindPlayerForReport;
	public delegate void FindPlayerForReportResultDelegate(int playerID, string strPlayerName);
	public static FindPlayerForReportResultDelegate FindPlayerForReportResult;
	public delegate void FurnitureStore_HideDelegate();
	public static FurnitureStore_HideDelegate FurnitureStore_Hide;
	public delegate void FurnitureStore_OnChangeClassDelegate(string strClassName);
	public static FurnitureStore_OnChangeClassDelegate FurnitureStore_OnChangeClass;
	public delegate void FurnitureStore_OnChangeFurnitureItemDelegate(int index);
	public static FurnitureStore_OnChangeFurnitureItemDelegate FurnitureStore_OnChangeFurnitureItem;
	public delegate void FurnitureStore_OnCheckoutDelegate();
	public static FurnitureStore_OnCheckoutDelegate FurnitureStore_OnCheckout;
	public delegate void FurnitureStore_ResetCameraDelegate();
	public static FurnitureStore_ResetCameraDelegate FurnitureStore_ResetCamera;
	public delegate void FurnitureStore_StartRotationDelegate(EFurnitureStoreRotationDirection direction);
	public static FurnitureStore_StartRotationDelegate FurnitureStore_StartRotation;
	public delegate void FurnitureStore_StartZoomDelegate(EFurnitureStoreZoomDirection direction);
	public static FurnitureStore_StartZoomDelegate FurnitureStore_StartZoom;
	public delegate void FurnitureStore_StopRotationDelegate();
	public static FurnitureStore_StopRotationDelegate FurnitureStore_StopRotation;
	public delegate void FurnitureStore_StopZoomDelegate();
	public static FurnitureStore_StopZoomDelegate FurnitureStore_StopZoom;
	public delegate void GangTagCreator_AddLayer_RectangleDelegate();
	public static GangTagCreator_AddLayer_RectangleDelegate GangTagCreator_AddLayer_Rectangle;
	public delegate void GangTagCreator_AddLayer_SpriteDelegate();
	public static GangTagCreator_AddLayer_SpriteDelegate GangTagCreator_AddLayer_Sprite;
	public delegate void GangTagCreator_AddLayer_TextDelegate();
	public static GangTagCreator_AddLayer_TextDelegate GangTagCreator_AddLayer_Text;
	public delegate void GangTagCreator_DeleteLayerDelegate(int layerID);
	public static GangTagCreator_DeleteLayerDelegate GangTagCreator_DeleteLayer;
	public delegate void GangTagCreator_EditLayerDelegate(int layerID);
	public static GangTagCreator_EditLayerDelegate GangTagCreator_EditLayer;
	public delegate void GangTagCreator_EditLayer_ChangeSpriteDelegate(string spriteHumanName);
	public static GangTagCreator_EditLayer_ChangeSpriteDelegate GangTagCreator_EditLayer_ChangeSprite;
	public delegate void GangTagCreator_EditLayer_SetAlphaDelegate(int alpha);
	public static GangTagCreator_EditLayer_SetAlphaDelegate GangTagCreator_EditLayer_SetAlpha;
	public delegate void GangTagCreator_EditLayer_SetColorDelegate(int r, int g, int b);
	public static GangTagCreator_EditLayer_SetColorDelegate GangTagCreator_EditLayer_SetColor;
	public delegate void GangTagCreator_EditLayer_SetFontIDDelegate(int fontID);
	public static GangTagCreator_EditLayer_SetFontIDDelegate GangTagCreator_EditLayer_SetFontID;
	public delegate void GangTagCreator_EditLayer_SetHeightDelegate(float fHeight);
	public static GangTagCreator_EditLayer_SetHeightDelegate GangTagCreator_EditLayer_SetHeight;
	public delegate void GangTagCreator_EditLayer_SetOutlineDelegate(bool bOutline);
	public static GangTagCreator_EditLayer_SetOutlineDelegate GangTagCreator_EditLayer_SetOutline;
	public delegate void GangTagCreator_EditLayer_SetScaleDelegate(float fScale);
	public static GangTagCreator_EditLayer_SetScaleDelegate GangTagCreator_EditLayer_SetScale;
	public delegate void GangTagCreator_EditLayer_SetShadowDelegate(bool bShadow);
	public static GangTagCreator_EditLayer_SetShadowDelegate GangTagCreator_EditLayer_SetShadow;
	public delegate void GangTagCreator_EditLayer_SetSpriteRotationDelegate(float fRotation);
	public static GangTagCreator_EditLayer_SetSpriteRotationDelegate GangTagCreator_EditLayer_SetSpriteRotation;
	public delegate void GangTagCreator_EditLayer_SetTextDelegate(string strText);
	public static GangTagCreator_EditLayer_SetTextDelegate GangTagCreator_EditLayer_SetText;
	public delegate void GangTagCreator_EditLayer_SetWidthDelegate(float fWidth);
	public static GangTagCreator_EditLayer_SetWidthDelegate GangTagCreator_EditLayer_SetWidth;
	public delegate void GangTagCreator_EditLayer_SetXCoordinateDelegate(float fX);
	public static GangTagCreator_EditLayer_SetXCoordinateDelegate GangTagCreator_EditLayer_SetXCoordinate;
	public delegate void GangTagCreator_EditLayer_SetYCoordinateDelegate(float fY);
	public static GangTagCreator_EditLayer_SetYCoordinateDelegate GangTagCreator_EditLayer_SetYCoordinate;
	public delegate void GangTagCreator_Exit_DiscardDelegate();
	public static GangTagCreator_Exit_DiscardDelegate GangTagCreator_Exit_Discard;
	public delegate void GangTagCreator_Exit_KeepWIPDelegate();
	public static GangTagCreator_Exit_KeepWIPDelegate GangTagCreator_Exit_KeepWIP;
	public delegate void GangTagCreator_Exit_SaveDelegate();
	public static GangTagCreator_Exit_SaveDelegate GangTagCreator_Exit_Save;
	public delegate void GangTagCreator_MoveLayerDownDelegate(int layerID);
	public static GangTagCreator_MoveLayerDownDelegate GangTagCreator_MoveLayerDown;
	public delegate void GangTagCreator_MoveLayerUpDelegate(int layerID);
	public static GangTagCreator_MoveLayerUpDelegate GangTagCreator_MoveLayerUp;
	public delegate void GangTag_AcceptShareDelegate();
	public static GangTag_AcceptShareDelegate GangTag_AcceptShare;
	public delegate void GangTag_DeclineShareDelegate();
	public static GangTag_DeclineShareDelegate GangTag_DeclineShare;
	public delegate void GangTag_EditSavedTagDelegate();
	public static GangTag_EditSavedTagDelegate GangTag_EditSavedTag;
	public delegate void GangTag_EditWIPTagDelegate();
	public static GangTag_EditWIPTagDelegate GangTag_EditWIPTag;
	public delegate void GenericCharacterCustomization_DismissErrorDelegate();
	public static GenericCharacterCustomization_DismissErrorDelegate GenericCharacterCustomization_DismissError;
	public delegate void GenericCharacterCustomization_ExitDelegate();
	public static GenericCharacterCustomization_ExitDelegate GenericCharacterCustomization_Exit;
	public delegate void GenericCharacterCustomization_FinishDelegate();
	public static GenericCharacterCustomization_FinishDelegate GenericCharacterCustomization_Finish;
	public delegate void GenericCharacterCustomization_GotoFarCamEventDelegate();
	public static GenericCharacterCustomization_GotoFarCamEventDelegate GenericCharacterCustomization_GotoFarCamEvent;
	public delegate void GenericCharacterCustomization_GotoHeadCamEventDelegate();
	public static GenericCharacterCustomization_GotoHeadCamEventDelegate GenericCharacterCustomization_GotoHeadCamEvent;
	public delegate void GenericCharacterCustomization_GotoNearCamEventDelegate();
	public static GenericCharacterCustomization_GotoNearCamEventDelegate GenericCharacterCustomization_GotoNearCamEvent;
	public delegate void GenericCharacterCustomization_ResetRotationEventDelegate();
	public static GenericCharacterCustomization_ResetRotationEventDelegate GenericCharacterCustomization_ResetRotationEvent;
	public delegate void GenericCharacterCustomization_StartRotationEventDelegate(int direction);
	public static GenericCharacterCustomization_StartRotationEventDelegate GenericCharacterCustomization_StartRotationEvent;
	public delegate void GenericCharacterCustomization_StopRotationEventDelegate(int direction);
	public static GenericCharacterCustomization_StopRotationEventDelegate GenericCharacterCustomization_StopRotationEvent;
	public delegate void GenericCharacterCustomization_ToggleClothesDelegate();
	public static GenericCharacterCustomization_ToggleClothesDelegate GenericCharacterCustomization_ToggleClothes;
	public delegate void GenericDataTable_OnCloseDelegate();
	public static GenericDataTable_OnCloseDelegate GenericDataTable_OnClose;
	public delegate void GenericDropdown_OnCloseDelegate();
	public static GenericDropdown_OnCloseDelegate GenericDropdown_OnClose;
	public delegate void GenericListBox_OnCloseDelegate();
	public static GenericListBox_OnCloseDelegate GenericListBox_OnClose;
	public delegate void GenericMessageBox_OnCloseDelegate();
	public static GenericMessageBox_OnCloseDelegate GenericMessageBox_OnClose;
	public delegate void GenericProgressBar_OnCloseDelegate();
	public static GenericProgressBar_OnCloseDelegate GenericProgressBar_OnClose;
	public delegate void GenericPrompt3_OnCloseDelegate();
	public static GenericPrompt3_OnCloseDelegate GenericPrompt3_OnClose;
	public delegate void GenericPrompt_OnCloseDelegate();
	public static GenericPrompt_OnCloseDelegate GenericPrompt_OnClose;
	public delegate void GenericUserLoginBox_OnCloseDelegate();
	public static GenericUserLoginBox_OnCloseDelegate GenericUserLoginBox_OnClose;
	public delegate void Generic_CloseGenericsUIDelegate();
	public static Generic_CloseGenericsUIDelegate Generic_CloseGenericsUI;
	public delegate void Generic_CloseItemMoverUIDelegate();
	public static Generic_CloseItemMoverUIDelegate Generic_CloseItemMoverUI;
	public delegate void Generic_SpawnGenericsDelegate(string name, string model, string amount, string price);
	public static Generic_SpawnGenericsDelegate Generic_SpawnGenerics;
	public delegate void Generic_UpdateGenericPositionDelegate(string posX, string posY, string posZ, string rotX, string rotY, string rotZ);
	public static Generic_UpdateGenericPositionDelegate Generic_UpdateGenericPosition;
	public delegate void Generic_UpdateGenericPreviewPositionDelegate(string posX, string posY, string posZ, string rotX, string rotY, string rotZ);
	public static Generic_UpdateGenericPreviewPositionDelegate Generic_UpdateGenericPreviewPosition;
	public delegate void GetPendingApplicationsDelegate();
	public static GetPendingApplicationsDelegate GetPendingApplications;
	public delegate void GetPhoneContactByNumberDelegate(string callingNumber);
	public static GetPhoneContactByNumberDelegate GetPhoneContactByNumber;
	public delegate void GetPhoneContactsDelegate();
	public static GetPhoneContactsDelegate GetPhoneContacts;
	public delegate void GetPhoneMessagesContactsDelegate();
	public static GetPhoneMessagesContactsDelegate GetPhoneMessagesContacts;
	public delegate void GetPhoneMessagesFromNumberDelegate(string toNumber);
	public static GetPhoneMessagesFromNumberDelegate GetPhoneMessagesFromNumber;
	public delegate void GetTotalUnviewedMessagesDelegate();
	public static GetTotalUnviewedMessagesDelegate GetTotalUnviewedMessages;
	public delegate void GoBackToRegisterDelegate();
	public static GoBackToRegisterDelegate GoBackToRegister;
	public delegate void GoOnDutyDelegate();
	public static GoOnDutyDelegate GoOnDuty;
	public delegate void GotoCreateCharacterDelegate();
	public static GotoCreateCharacterDelegate GotoCreateCharacter;
	public delegate void GotoDiscordLinkingDelegate();
	public static GotoDiscordLinkingDelegate GotoDiscordLinking;
	public delegate void GotoDonationsDelegate();
	public static GotoDonationsDelegate GotoDonations;
	public delegate void GotoEditInteriorDelegate();
	public static GotoEditInteriorDelegate GotoEditInterior;
	public delegate void GotoFullScreenBrowserDelegate(string strURL);
	public static GotoFullScreenBrowserDelegate GotoFullScreenBrowser;
	public delegate void GotoLanguagesDelegate();
	public static GotoLanguagesDelegate GotoLanguages;
	public delegate void GotoRegisterPressedDelegate();
	public static GotoRegisterPressedDelegate GotoRegisterPressed;
	public delegate void GotoSplitItemDelegate();
	public static GotoSplitItemDelegate GotoSplitItem;
	public delegate void GotoViewAchievementsDelegate();
	public static GotoViewAchievementsDelegate GotoViewAchievements;
	public delegate void GotRadioMessageDelegate();
	public static GotRadioMessageDelegate GotRadioMessage;
	public delegate void HelpRequestCommandsDelegate();
	public static HelpRequestCommandsDelegate HelpRequestCommands;
	public delegate void HideChatSettingsDelegate(bool bSaveToServer);
	public static HideChatSettingsDelegate HideChatSettings;
	public delegate void HideControlManagerDelegate();
	public static HideControlManagerDelegate HideControlManager;
	public delegate void HideCreateFactionDelegate();
	public static HideCreateFactionDelegate HideCreateFaction;
	public delegate void HideHelpCenterDelegate();
	public static HideHelpCenterDelegate HideHelpCenter;
	public delegate void HideHudMenuDelegate();
	public static HideHudMenuDelegate HideHudMenu;
	public delegate void HideLicenseDeviceDelegate();
	public static HideLicenseDeviceDelegate HideLicenseDevice;
	public delegate void HideMDCUIsDelegate();
	public static HideMDCUIsDelegate HideMDCUIs;
	public delegate void HidePurchasePropertyUIDelegate();
	public static HidePurchasePropertyUIDelegate HidePurchasePropertyUI;
	public delegate void HideRegisterSuccessMessageBoxDelegate();
	public static HideRegisterSuccessMessageBoxDelegate HideRegisterSuccessMessageBox;
	public delegate void HideStoreDelegate();
	public static HideStoreDelegate HideStore;
	public delegate void IncomingDutyOutfitShare_AcceptDelegate();
	public static IncomingDutyOutfitShare_AcceptDelegate IncomingDutyOutfitShare_Accept;
	public delegate void IncomingDutyOutfitShare_DeclineDelegate();
	public static IncomingDutyOutfitShare_DeclineDelegate IncomingDutyOutfitShare_Decline;
	public delegate void InfoMarkerOwned_DeleteDelegate();
	public static InfoMarkerOwned_DeleteDelegate InfoMarkerOwned_Delete;
	public delegate void InfoMarkerOwned_ExitDelegate();
	public static InfoMarkerOwned_ExitDelegate InfoMarkerOwned_Exit;
	public delegate void InfoMarkerOwned_ReadDelegate();
	public static InfoMarkerOwned_ReadDelegate InfoMarkerOwned_Read;
	public delegate void Inventory_CloseWindowDelegate(int windowIndex);
	public static Inventory_CloseWindowDelegate Inventory_CloseWindow;
	public delegate void Inventory_GoBackDelegate(int windowIndex);
	public static Inventory_GoBackDelegate Inventory_GoBack;
	public delegate void InviteFactionPlayerDelegate(int factionIndex, string strPlayerName);
	public static InviteFactionPlayerDelegate InviteFactionPlayer;
	public delegate void KickFactionMemberDelegate(int factionIndex, int memberIndex);
	public static KickFactionMemberDelegate KickFactionMember;
	public delegate void LanguageMenu_CloseDelegate();
	public static LanguageMenu_CloseDelegate LanguageMenu_Close;
	public delegate void LanguageMenu_SelectLanguageDelegate(string ActiveLanguage);
	public static LanguageMenu_SelectLanguageDelegate LanguageMenu_SelectLanguage;
	public delegate void LeaveFactionDelegate(int factionIndex);
	public static LeaveFactionDelegate LeaveFaction;
	public delegate void LogoutDelegate();
	public static LogoutDelegate Logout;
	public delegate void MapLoader_CancelUploadDelegate();
	public static MapLoader_CancelUploadDelegate MapLoader_CancelUpload;
	public delegate void Marijuana_ExitDelegate();
	public static Marijuana_ExitDelegate Marijuana_Exit;
	public delegate void Marijuana_GetSeedsDelegate();
	public static Marijuana_GetSeedsDelegate Marijuana_GetSeeds;
	public delegate void Marijuana_SellDrugsDelegate(uint count);
	public static Marijuana_SellDrugsDelegate Marijuana_SellDrugs;
	public delegate void MdcGotoPersonDelegate(string strName);
	public static MdcGotoPersonDelegate MdcGotoPerson;
	public delegate void MdcGotoPropertyDelegate(Int64 propertyID);
	public static MdcGotoPropertyDelegate MdcGotoProperty;
	public delegate void MdcGotoVehicleDelegate(Int64 vehicleID);
	public static MdcGotoVehicleDelegate MdcGotoVehicle;
	public delegate void OnChangeFarePerMile_CancelDelegate();
	public static OnChangeFarePerMile_CancelDelegate OnChangeFarePerMile_Cancel;
	public delegate void OnChangeFarePerMile_SubmitDelegate(float fAmount);
	public static OnChangeFarePerMile_SubmitDelegate OnChangeFarePerMile_Submit;
	public delegate void OnChatBoxCommandDelegate(string strCommand);
	public static OnChatBoxCommandDelegate OnChatBoxCommand;
	public delegate void OnChatBoxMessageDelegate(string strMessage);
	public static OnChatBoxMessageDelegate OnChatBoxMessage;
	public delegate void OnChatInputVisibleChangedDelegate(bool bVisible);
	public static OnChatInputVisibleChangedDelegate OnChatInputVisibleChanged;
	public delegate void OnDestroyItemDelegate();
	public static OnDestroyItemDelegate OnDestroyItem;
	public delegate void OnDestroyItem_CancelDelegate();
	public static OnDestroyItem_CancelDelegate OnDestroyItem_Cancel;
	public delegate void OnDestroyItem_ConfirmDelegate();
	public static OnDestroyItem_ConfirmDelegate OnDestroyItem_Confirm;
	public delegate void OnDisconnectedFromServerDelegate();
	public static OnDisconnectedFromServerDelegate OnDisconnectedFromServer;
	public delegate void OnDropItemDelegate();
	public static OnDropItemDelegate OnDropItem;
	public delegate void OnDutyOutfitShareDelegate();
	public static OnDutyOutfitShareDelegate OnDutyOutfitShare;
	public delegate void OnFactionDisband_CancelDelegate();
	public static OnFactionDisband_CancelDelegate OnFactionDisband_Cancel;
	public delegate void OnFactionDisband_ConfirmDelegate();
	public static OnFactionDisband_ConfirmDelegate OnFactionDisband_Confirm;
	public delegate void OnFactionLeave_CancelDelegate();
	public static OnFactionLeave_CancelDelegate OnFactionLeave_Cancel;
	public delegate void OnFactionLeave_ConfirmDelegate();
	public static OnFactionLeave_ConfirmDelegate OnFactionLeave_Confirm;
	public delegate void OnFriskTakeItemDelegate(int index);
	public static OnFriskTakeItemDelegate OnFriskTakeItem;
	public delegate void OnHideFriskDelegate();
	public static OnHideFriskDelegate OnHideFrisk;
	public delegate void OnHideImpoundedVehicleDelegate();
	public static OnHideImpoundedVehicleDelegate OnHideImpoundedVehicle;
	public delegate void OnHTMLLoadedDelegate(string strGuiID, string strFullURI);
	public static OnHTMLLoadedDelegate OnHTMLLoaded;
	public delegate void OnLocksmithRequestChoose_PropertyDelegate();
	public static OnLocksmithRequestChoose_PropertyDelegate OnLocksmithRequestChoose_Property;
	public delegate void OnLocksmithRequestChoose_VehicleDelegate();
	public static OnLocksmithRequestChoose_VehicleDelegate OnLocksmithRequestChoose_Vehicle;
	public delegate void OnLocksmithRequest_CancelDelegate();
	public static OnLocksmithRequest_CancelDelegate OnLocksmithRequest_Cancel;
	public delegate void OnLocksmithRequest_SubmitDelegate(string strInput);
	public static OnLocksmithRequest_SubmitDelegate OnLocksmithRequest_Submit;
	public delegate void OnMergeItemDelegate();
	public static OnMergeItemDelegate OnMergeItem;
	public delegate void OnMoveItemDelegate();
	public static OnMoveItemDelegate OnMoveItem;
	public delegate void OnNamePet_CancelDelegate();
	public static OnNamePet_CancelDelegate OnNamePet_Cancel;
	public delegate void OnNamePet_SubmitDelegate(string strInput);
	public static OnNamePet_SubmitDelegate OnNamePet_Submit;
	public delegate void OnPressPhoneButtonDelegate();
	public static OnPressPhoneButtonDelegate OnPressPhoneButton;
	public delegate void OnRetuneRadio_CancelDelegate();
	public static OnRetuneRadio_CancelDelegate OnRetuneRadio_Cancel;
	public delegate void OnRetuneRadio_SubmitDelegate(string strInput);
	public static OnRetuneRadio_SubmitDelegate OnRetuneRadio_Submit;
	public delegate void OnShareGangTag_CancelDelegate();
	public static OnShareGangTag_CancelDelegate OnShareGangTag_Cancel;
	public delegate void OnShareGangTag_SubmitDelegate(string strInput);
	public static OnShareGangTag_SubmitDelegate OnShareGangTag_Submit;
	public delegate void OnShowItemDelegate();
	public static OnShowItemDelegate OnShowItem;
	public delegate void OnSpawnSelector_LSDelegate();
	public static OnSpawnSelector_LSDelegate OnSpawnSelector_LS;
	public delegate void OnSpawnSelector_PaletoDelegate();
	public static OnSpawnSelector_PaletoDelegate OnSpawnSelector_Paleto;
	public delegate void OnSwitchAccountDelegate(int index);
	public static OnSwitchAccountDelegate OnSwitchAccount;
	public delegate void OnUnlinkDiscord_CancelDelegate();
	public static OnUnlinkDiscord_CancelDelegate OnUnlinkDiscord_Cancel;
	public delegate void OnUnlinkDiscord_ConfirmDelegate();
	public static OnUnlinkDiscord_ConfirmDelegate OnUnlinkDiscord_Confirm;
	public delegate void OnUseItemDelegate();
	public static OnUseItemDelegate OnUseItem;
	public delegate void OnWatchNewTutorial_CancelDelegate();
	public static OnWatchNewTutorial_CancelDelegate OnWatchNewTutorial_Cancel;
	public delegate void OnWatchNewTutorial_Confirm_AllDelegate();
	public static OnWatchNewTutorial_Confirm_AllDelegate OnWatchNewTutorial_Confirm_All;
	public delegate void OnWatchNewTutorial_Confirm_NewDelegate();
	public static OnWatchNewTutorial_Confirm_NewDelegate OnWatchNewTutorial_Confirm_New;
	public delegate void OnWireTransferKeyboard_CancelDelegate();
	public static OnWireTransferKeyboard_CancelDelegate OnWireTransferKeyboard_Cancel;
	public delegate void OnWireTransferKeyboard_SubmitDelegate(string strInput);
	public static OnWireTransferKeyboard_SubmitDelegate OnWireTransferKeyboard_Submit;
	public delegate void OpenMobileBankingUIDelegate();
	public static OpenMobileBankingUIDelegate OpenMobileBankingUI;
	public delegate void OpenRadioManagerDelegate();
	public static OpenRadioManagerDelegate OpenRadioManager;
	public delegate void OpenTransferAssetsDelegate();
	public static OpenTransferAssetsDelegate OpenTransferAssets;
	public delegate void OutfitEditor_DeleteOutfitDelegate();
	public static OutfitEditor_DeleteOutfitDelegate OutfitEditor_DeleteOutfit;
	public delegate void OutfitEditor_EditOutfitDelegate(string strElementName, int outfitIndex);
	public static OutfitEditor_EditOutfitDelegate OutfitEditor_EditOutfit;
	public delegate void OutfitEditor_Outfit_OnMouseEnterDelegate(string strElementName, int outfitIndex);
	public static OutfitEditor_Outfit_OnMouseEnterDelegate OutfitEditor_Outfit_OnMouseEnter;
	public delegate void OutfitEditor_Outfit_OnMouseExitDelegate(string strElementName, int outfitIndex);
	public static OutfitEditor_Outfit_OnMouseExitDelegate OutfitEditor_Outfit_OnMouseExit;
	public delegate void OutfitEditor_SetAccessoriesIndexDelegate(int value);
	public static OutfitEditor_SetAccessoriesIndexDelegate OutfitEditor_SetAccessoriesIndex;
	public delegate void OutfitEditor_SetBagsAndParachutesIndexDelegate(int value);
	public static OutfitEditor_SetBagsAndParachutesIndexDelegate OutfitEditor_SetBagsAndParachutesIndex;
	public delegate void OutfitEditor_SetBraceletsIndexDelegate(int value);
	public static OutfitEditor_SetBraceletsIndexDelegate OutfitEditor_SetBraceletsIndex;
	public delegate void OutfitEditor_SetDecalsIndexDelegate(int value);
	public static OutfitEditor_SetDecalsIndexDelegate OutfitEditor_SetDecalsIndex;
	public delegate void OutfitEditor_SetEarsIndexDelegate(int value);
	public static OutfitEditor_SetEarsIndexDelegate OutfitEditor_SetEarsIndex;
	public delegate void OutfitEditor_SetGlassesIndexDelegate(int value);
	public static OutfitEditor_SetGlassesIndexDelegate OutfitEditor_SetGlassesIndex;
	public delegate void OutfitEditor_SetHairVisibleDelegate(bool bVisible);
	public static OutfitEditor_SetHairVisibleDelegate OutfitEditor_SetHairVisible;
	public delegate void OutfitEditor_SetHatIndexDelegate(int value);
	public static OutfitEditor_SetHatIndexDelegate OutfitEditor_SetHatIndex;
	public delegate void OutfitEditor_SetLegsIndexDelegate(int value);
	public static OutfitEditor_SetLegsIndexDelegate OutfitEditor_SetLegsIndex;
	public delegate void OutfitEditor_SetMaskIndexDelegate(int value);
	public static OutfitEditor_SetMaskIndexDelegate OutfitEditor_SetMaskIndex;
	public delegate void OutfitEditor_SetOutfitNameDelegate(string strName);
	public static OutfitEditor_SetOutfitNameDelegate OutfitEditor_SetOutfitName;
	public delegate void OutfitEditor_SetShoesIndexDelegate(int value);
	public static OutfitEditor_SetShoesIndexDelegate OutfitEditor_SetShoesIndex;
	public delegate void OutfitEditor_SetTopsIndexDelegate(int value);
	public static OutfitEditor_SetTopsIndexDelegate OutfitEditor_SetTopsIndex;
	public delegate void OutfitEditor_SetTorsoIndexDelegate(int value);
	public static OutfitEditor_SetTorsoIndexDelegate OutfitEditor_SetTorsoIndex;
	public delegate void OutfitEditor_SetUndershirtsIndexDelegate(int value);
	public static OutfitEditor_SetUndershirtsIndexDelegate OutfitEditor_SetUndershirtsIndex;
	public delegate void OutfitEditor_SetWatchesIndexDelegate(int value);
	public static OutfitEditor_SetWatchesIndexDelegate OutfitEditor_SetWatchesIndex;
	public delegate void PaydayOverview_OnCloseDelegate();
	public static PaydayOverview_OnCloseDelegate PaydayOverview_OnClose;
	public delegate void PDHelicopterHUD_ToggleMovingVehiclesOnlyDelegate(bool enabled);
	public static PDHelicopterHUD_ToggleMovingVehiclesOnlyDelegate PDHelicopterHUD_ToggleMovingVehiclesOnly;
	public delegate void PDHelicopterHUD_ToggleNVGDelegate(bool enabled);
	public static PDHelicopterHUD_ToggleNVGDelegate PDHelicopterHUD_ToggleNVG;
	public delegate void PDHelicopterHUD_TogglePeopleDelegate(bool enabled);
	public static PDHelicopterHUD_TogglePeopleDelegate PDHelicopterHUD_TogglePeople;
	public delegate void PDHelicopterHUD_ToggleThermalDelegate(bool enabled);
	public static PDHelicopterHUD_ToggleThermalDelegate PDHelicopterHUD_ToggleThermal;
	public delegate void PDHelicopterHUD_ToggleVehiclesOccupiedDelegate(bool enabled);
	public static PDHelicopterHUD_ToggleVehiclesOccupiedDelegate PDHelicopterHUD_ToggleVehiclesOccupied;
	public delegate void PDHelicopterHUD_ToggleVehiclesUnoccupiedDelegate(bool enabled);
	public static PDHelicopterHUD_ToggleVehiclesUnoccupiedDelegate PDHelicopterHUD_ToggleVehiclesUnoccupied;
	public delegate void PersistentNotifications_DismissedDelegate(Int64 notificationID);
	public static PersistentNotifications_DismissedDelegate PersistentNotifications_Dismissed;
	public delegate void PlasticSurgeonOfferCharacterUpgrade_ConfirmDelegate();
	public static PlasticSurgeonOfferCharacterUpgrade_ConfirmDelegate PlasticSurgeonOfferCharacterUpgrade_Confirm;
	public delegate void PlasticSurgeonOfferCharacterUpgrade_DeclineDelegate();
	public static PlasticSurgeonOfferCharacterUpgrade_DeclineDelegate PlasticSurgeonOfferCharacterUpgrade_Decline;
	public delegate void PlasticSurgeon_SetAgeingDelegate(int value);
	public static PlasticSurgeon_SetAgeingDelegate PlasticSurgeon_SetAgeing;
	public delegate void PlasticSurgeon_SetAgeingOpacityDelegate(float value);
	public static PlasticSurgeon_SetAgeingOpacityDelegate PlasticSurgeon_SetAgeingOpacity;
	public delegate void PlasticSurgeon_SetAgeingOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetAgeingOpacity_FinalizeDelegate PlasticSurgeon_SetAgeingOpacity_Finalize;
	public delegate void PlasticSurgeon_SetBlemishesDelegate(int value);
	public static PlasticSurgeon_SetBlemishesDelegate PlasticSurgeon_SetBlemishes;
	public delegate void PlasticSurgeon_SetBlemishesOpacityDelegate(float value);
	public static PlasticSurgeon_SetBlemishesOpacityDelegate PlasticSurgeon_SetBlemishesOpacity;
	public delegate void PlasticSurgeon_SetBlemishesOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetBlemishesOpacity_FinalizeDelegate PlasticSurgeon_SetBlemishesOpacity_Finalize;
	public delegate void PlasticSurgeon_SetBlushDelegate(int value);
	public static PlasticSurgeon_SetBlushDelegate PlasticSurgeon_SetBlush;
	public delegate void PlasticSurgeon_SetBlushColorDelegate(int value);
	public static PlasticSurgeon_SetBlushColorDelegate PlasticSurgeon_SetBlushColor;
	public delegate void PlasticSurgeon_SetBlushColorHighlightsDelegate(int value);
	public static PlasticSurgeon_SetBlushColorHighlightsDelegate PlasticSurgeon_SetBlushColorHighlights;
	public delegate void PlasticSurgeon_SetBlushOpacityDelegate(float value);
	public static PlasticSurgeon_SetBlushOpacityDelegate PlasticSurgeon_SetBlushOpacity;
	public delegate void PlasticSurgeon_SetBlushOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetBlushOpacity_FinalizeDelegate PlasticSurgeon_SetBlushOpacity_Finalize;
	public delegate void PlasticSurgeon_SetBodyBlemishesDelegate(int value);
	public static PlasticSurgeon_SetBodyBlemishesDelegate PlasticSurgeon_SetBodyBlemishes;
	public delegate void PlasticSurgeon_SetBodyBlemishesOpacityDelegate(float value);
	public static PlasticSurgeon_SetBodyBlemishesOpacityDelegate PlasticSurgeon_SetBodyBlemishesOpacity;
	public delegate void PlasticSurgeon_SetBodyBlemishesOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetBodyBlemishesOpacity_FinalizeDelegate PlasticSurgeon_SetBodyBlemishesOpacity_Finalize;
	public delegate void PlasticSurgeon_SetCheekboneHeightDelegate(float value);
	public static PlasticSurgeon_SetCheekboneHeightDelegate PlasticSurgeon_SetCheekboneHeight;
	public delegate void PlasticSurgeon_SetCheekboneHeight_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetCheekboneHeight_FinalizeDelegate PlasticSurgeon_SetCheekboneHeight_Finalize;
	public delegate void PlasticSurgeon_SetCheekWidthDelegate(float value);
	public static PlasticSurgeon_SetCheekWidthDelegate PlasticSurgeon_SetCheekWidth;
	public delegate void PlasticSurgeon_SetCheekWidthLowerDelegate(float value);
	public static PlasticSurgeon_SetCheekWidthLowerDelegate PlasticSurgeon_SetCheekWidthLower;
	public delegate void PlasticSurgeon_SetCheekWidthLower_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetCheekWidthLower_FinalizeDelegate PlasticSurgeon_SetCheekWidthLower_Finalize;
	public delegate void PlasticSurgeon_SetCheekWidth_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetCheekWidth_FinalizeDelegate PlasticSurgeon_SetCheekWidth_Finalize;
	public delegate void PlasticSurgeon_SetChestHairDelegate(int value);
	public static PlasticSurgeon_SetChestHairDelegate PlasticSurgeon_SetChestHair;
	public delegate void PlasticSurgeon_SetChestHairColorDelegate(int value);
	public static PlasticSurgeon_SetChestHairColorDelegate PlasticSurgeon_SetChestHairColor;
	public delegate void PlasticSurgeon_SetChestHairColorHighlightsDelegate(int value);
	public static PlasticSurgeon_SetChestHairColorHighlightsDelegate PlasticSurgeon_SetChestHairColorHighlights;
	public delegate void PlasticSurgeon_SetChestHairOpacityDelegate(float value);
	public static PlasticSurgeon_SetChestHairOpacityDelegate PlasticSurgeon_SetChestHairOpacity;
	public delegate void PlasticSurgeon_SetChestHairOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetChestHairOpacity_FinalizeDelegate PlasticSurgeon_SetChestHairOpacity_Finalize;
	public delegate void PlasticSurgeon_SetChinEffectDelegate(float value);
	public static PlasticSurgeon_SetChinEffectDelegate PlasticSurgeon_SetChinEffect;
	public delegate void PlasticSurgeon_SetChinEffect_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetChinEffect_FinalizeDelegate PlasticSurgeon_SetChinEffect_Finalize;
	public delegate void PlasticSurgeon_SetChinSizeDelegate(float value);
	public static PlasticSurgeon_SetChinSizeDelegate PlasticSurgeon_SetChinSize;
	public delegate void PlasticSurgeon_SetChinSizeUnderneathDelegate(float value);
	public static PlasticSurgeon_SetChinSizeUnderneathDelegate PlasticSurgeon_SetChinSizeUnderneath;
	public delegate void PlasticSurgeon_SetChinSizeUnderneath_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetChinSizeUnderneath_FinalizeDelegate PlasticSurgeon_SetChinSizeUnderneath_Finalize;
	public delegate void PlasticSurgeon_SetChinSize_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetChinSize_FinalizeDelegate PlasticSurgeon_SetChinSize_Finalize;
	public delegate void PlasticSurgeon_SetChinWidthDelegate(float value);
	public static PlasticSurgeon_SetChinWidthDelegate PlasticSurgeon_SetChinWidth;
	public delegate void PlasticSurgeon_SetChinWidth_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetChinWidth_FinalizeDelegate PlasticSurgeon_SetChinWidth_Finalize;
	public delegate void PlasticSurgeon_SetComplexionDelegate(int value);
	public static PlasticSurgeon_SetComplexionDelegate PlasticSurgeon_SetComplexion;
	public delegate void PlasticSurgeon_SetComplexionOpacityDelegate(float value);
	public static PlasticSurgeon_SetComplexionOpacityDelegate PlasticSurgeon_SetComplexionOpacity;
	public delegate void PlasticSurgeon_SetComplexionOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetComplexionOpacity_FinalizeDelegate PlasticSurgeon_SetComplexionOpacity_Finalize;
	public delegate void PlasticSurgeon_SetEyebrowDepthDelegate(float value);
	public static PlasticSurgeon_SetEyebrowDepthDelegate PlasticSurgeon_SetEyebrowDepth;
	public delegate void PlasticSurgeon_SetEyebrowDepth_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetEyebrowDepth_FinalizeDelegate PlasticSurgeon_SetEyebrowDepth_Finalize;
	public delegate void PlasticSurgeon_SetEyebrowHeightDelegate(float value);
	public static PlasticSurgeon_SetEyebrowHeightDelegate PlasticSurgeon_SetEyebrowHeight;
	public delegate void PlasticSurgeon_SetEyebrowHeight_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetEyebrowHeight_FinalizeDelegate PlasticSurgeon_SetEyebrowHeight_Finalize;
	public delegate void PlasticSurgeon_SetEyeBrowsDelegate(int value);
	public static PlasticSurgeon_SetEyeBrowsDelegate PlasticSurgeon_SetEyeBrows;
	public delegate void PlasticSurgeon_SetEyeBrowsColorDelegate(int value);
	public static PlasticSurgeon_SetEyeBrowsColorDelegate PlasticSurgeon_SetEyeBrowsColor;
	public delegate void PlasticSurgeon_SetEyeBrowsColorHighlightsDelegate(int value);
	public static PlasticSurgeon_SetEyeBrowsColorHighlightsDelegate PlasticSurgeon_SetEyeBrowsColorHighlights;
	public delegate void PlasticSurgeon_SetEyeBrowsOpacityDelegate(float value);
	public static PlasticSurgeon_SetEyeBrowsOpacityDelegate PlasticSurgeon_SetEyeBrowsOpacity;
	public delegate void PlasticSurgeon_SetEyeBrowsOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetEyeBrowsOpacity_FinalizeDelegate PlasticSurgeon_SetEyeBrowsOpacity_Finalize;
	public delegate void PlasticSurgeon_SetEyeColorDelegate(int value);
	public static PlasticSurgeon_SetEyeColorDelegate PlasticSurgeon_SetEyeColor;
	public delegate void PlasticSurgeon_SetEyeSizeDelegate(float value);
	public static PlasticSurgeon_SetEyeSizeDelegate PlasticSurgeon_SetEyeSize;
	public delegate void PlasticSurgeon_SetEyeSize_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetEyeSize_FinalizeDelegate PlasticSurgeon_SetEyeSize_Finalize;
	public delegate void PlasticSurgeon_SetFaceShapeDelegate(int index, int gender, int value);
	public static PlasticSurgeon_SetFaceShapeDelegate PlasticSurgeon_SetFaceShape;
	public delegate void PlasticSurgeon_SetLipSizeDelegate(float value);
	public static PlasticSurgeon_SetLipSizeDelegate PlasticSurgeon_SetLipSize;
	public delegate void PlasticSurgeon_SetLipSize_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetLipSize_FinalizeDelegate PlasticSurgeon_SetLipSize_Finalize;
	public delegate void PlasticSurgeon_SetLipstickDelegate(int value);
	public static PlasticSurgeon_SetLipstickDelegate PlasticSurgeon_SetLipstick;
	public delegate void PlasticSurgeon_SetLipstickColorDelegate(int value);
	public static PlasticSurgeon_SetLipstickColorDelegate PlasticSurgeon_SetLipstickColor;
	public delegate void PlasticSurgeon_SetLipstickColorHighlightsDelegate(int value);
	public static PlasticSurgeon_SetLipstickColorHighlightsDelegate PlasticSurgeon_SetLipstickColorHighlights;
	public delegate void PlasticSurgeon_SetLipstickOpacityDelegate(float value);
	public static PlasticSurgeon_SetLipstickOpacityDelegate PlasticSurgeon_SetLipstickOpacity;
	public delegate void PlasticSurgeon_SetLipstickOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetLipstickOpacity_FinalizeDelegate PlasticSurgeon_SetLipstickOpacity_Finalize;
	public delegate void PlasticSurgeon_SetMakeupDelegate(int value);
	public static PlasticSurgeon_SetMakeupDelegate PlasticSurgeon_SetMakeup;
	public delegate void PlasticSurgeon_SetMakeupColorDelegate(int value);
	public static PlasticSurgeon_SetMakeupColorDelegate PlasticSurgeon_SetMakeupColor;
	public delegate void PlasticSurgeon_SetMakeupColorHighlightsDelegate(int value);
	public static PlasticSurgeon_SetMakeupColorHighlightsDelegate PlasticSurgeon_SetMakeupColorHighlights;
	public delegate void PlasticSurgeon_SetMakeupOpacityDelegate(float value);
	public static PlasticSurgeon_SetMakeupOpacityDelegate PlasticSurgeon_SetMakeupOpacity;
	public delegate void PlasticSurgeon_SetMakeupOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetMakeupOpacity_FinalizeDelegate PlasticSurgeon_SetMakeupOpacity_Finalize;
	public delegate void PlasticSurgeon_SetMolesFrecklesDelegate(int value);
	public static PlasticSurgeon_SetMolesFrecklesDelegate PlasticSurgeon_SetMolesFreckles;
	public delegate void PlasticSurgeon_SetMolesFrecklesOpacityDelegate(float value);
	public static PlasticSurgeon_SetMolesFrecklesOpacityDelegate PlasticSurgeon_SetMolesFrecklesOpacity;
	public delegate void PlasticSurgeon_SetMolesFrecklesOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetMolesFrecklesOpacity_FinalizeDelegate PlasticSurgeon_SetMolesFrecklesOpacity_Finalize;
	public delegate void PlasticSurgeon_SetMouthSizeDelegate(float value);
	public static PlasticSurgeon_SetMouthSizeDelegate PlasticSurgeon_SetMouthSize;
	public delegate void PlasticSurgeon_SetMouthSizeLowerDelegate(float value);
	public static PlasticSurgeon_SetMouthSizeLowerDelegate PlasticSurgeon_SetMouthSizeLower;
	public delegate void PlasticSurgeon_SetMouthSizeLower_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetMouthSizeLower_FinalizeDelegate PlasticSurgeon_SetMouthSizeLower_Finalize;
	public delegate void PlasticSurgeon_SetMouthSize_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetMouthSize_FinalizeDelegate PlasticSurgeon_SetMouthSize_Finalize;
	public delegate void PlasticSurgeon_SetNeckWidthDelegate(float value);
	public static PlasticSurgeon_SetNeckWidthDelegate PlasticSurgeon_SetNeckWidth;
	public delegate void PlasticSurgeon_SetNeckWidthLowerDelegate(float value);
	public static PlasticSurgeon_SetNeckWidthLowerDelegate PlasticSurgeon_SetNeckWidthLower;
	public delegate void PlasticSurgeon_SetNeckWidthLower_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNeckWidthLower_FinalizeDelegate PlasticSurgeon_SetNeckWidthLower_Finalize;
	public delegate void PlasticSurgeon_SetNeckWidth_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNeckWidth_FinalizeDelegate PlasticSurgeon_SetNeckWidth_Finalize;
	public delegate void PlasticSurgeon_SetNoseAngleDelegate(float value);
	public static PlasticSurgeon_SetNoseAngleDelegate PlasticSurgeon_SetNoseAngle;
	public delegate void PlasticSurgeon_SetNoseAngle_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNoseAngle_FinalizeDelegate PlasticSurgeon_SetNoseAngle_Finalize;
	public delegate void PlasticSurgeon_SetNoseSizeHorizontalDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeHorizontalDelegate PlasticSurgeon_SetNoseSizeHorizontal;
	public delegate void PlasticSurgeon_SetNoseSizeHorizontal_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeHorizontal_FinalizeDelegate PlasticSurgeon_SetNoseSizeHorizontal_Finalize;
	public delegate void PlasticSurgeon_SetNoseSizeOutwardsDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeOutwardsDelegate PlasticSurgeon_SetNoseSizeOutwards;
	public delegate void PlasticSurgeon_SetNoseSizeOutwardsLowerDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeOutwardsLowerDelegate PlasticSurgeon_SetNoseSizeOutwardsLower;
	public delegate void PlasticSurgeon_SetNoseSizeOutwardsLower_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeOutwardsLower_FinalizeDelegate PlasticSurgeon_SetNoseSizeOutwardsLower_Finalize;
	public delegate void PlasticSurgeon_SetNoseSizeOutwardsUpperDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeOutwardsUpperDelegate PlasticSurgeon_SetNoseSizeOutwardsUpper;
	public delegate void PlasticSurgeon_SetNoseSizeOutwardsUpper_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeOutwardsUpper_FinalizeDelegate PlasticSurgeon_SetNoseSizeOutwardsUpper_Finalize;
	public delegate void PlasticSurgeon_SetNoseSizeOutwards_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeOutwards_FinalizeDelegate PlasticSurgeon_SetNoseSizeOutwards_Finalize;
	public delegate void PlasticSurgeon_SetNoseSizeVerticalDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeVerticalDelegate PlasticSurgeon_SetNoseSizeVertical;
	public delegate void PlasticSurgeon_SetNoseSizeVertical_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetNoseSizeVertical_FinalizeDelegate PlasticSurgeon_SetNoseSizeVertical_Finalize;
	public delegate void PlasticSurgeon_SetSkinPercentage_ColorDelegate(float value);
	public static PlasticSurgeon_SetSkinPercentage_ColorDelegate PlasticSurgeon_SetSkinPercentage_Color;
	public delegate void PlasticSurgeon_SetSkinPercentage_Color_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetSkinPercentage_Color_FinalizeDelegate PlasticSurgeon_SetSkinPercentage_Color_Finalize;
	public delegate void PlasticSurgeon_SetSkinPercentage_ShapeDelegate(float value);
	public static PlasticSurgeon_SetSkinPercentage_ShapeDelegate PlasticSurgeon_SetSkinPercentage_Shape;
	public delegate void PlasticSurgeon_SetSkinPercentage_Shape_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetSkinPercentage_Shape_FinalizeDelegate PlasticSurgeon_SetSkinPercentage_Shape_Finalize;
	public delegate void PlasticSurgeon_SetSunDamageDelegate(int value);
	public static PlasticSurgeon_SetSunDamageDelegate PlasticSurgeon_SetSunDamage;
	public delegate void PlasticSurgeon_SetSunDamageOpacityDelegate(float value);
	public static PlasticSurgeon_SetSunDamageOpacityDelegate PlasticSurgeon_SetSunDamageOpacity;
	public delegate void PlasticSurgeon_SetSunDamageOpacity_FinalizeDelegate(float value);
	public static PlasticSurgeon_SetSunDamageOpacity_FinalizeDelegate PlasticSurgeon_SetSunDamageOpacity_Finalize;
	public delegate void PlayAnimationFromUIDelegate(string commandName);
	public static PlayAnimationFromUIDelegate PlayAnimationFromUI;
	public delegate void PlayerRadial_OnEnterItemDelegate(EPlayerRadialInteractionID index);
	public static PlayerRadial_OnEnterItemDelegate PlayerRadial_OnEnterItem;
	public delegate void PlayerRadial_OnExitItemDelegate(EPlayerRadialInteractionID index);
	public static PlayerRadial_OnExitItemDelegate PlayerRadial_OnExitItem;
	public delegate void PreviewCharacterDelegate(int index);
	public static PreviewCharacterDelegate PreviewCharacter;
	public delegate void PurchaseDonationPerkDelegate(UInt32 dbid);
	public static PurchaseDonationPerkDelegate PurchaseDonationPerk;
	public delegate void PurchaseGCDelegate();
	public static PurchaseGCDelegate PurchaseGC;
	public delegate void PurchaseInactivityProtectionDelegate();
	public static PurchaseInactivityProtectionDelegate PurchaseInactivityProtection;
	public delegate void PurchaseProperty_OnCheckoutDelegate(int purchaserIndex, EPaymentMethod method);
	public static PurchaseProperty_OnCheckoutDelegate PurchaseProperty_OnCheckout;
	public delegate void PurchaseProperty_OnPreviewDelegate();
	public static PurchaseProperty_OnPreviewDelegate PurchaseProperty_OnPreview;
	public delegate void PurchaseProperty_SetMonthlyDownpaymentDelegate(float fDownpayment);
	public static PurchaseProperty_SetMonthlyDownpaymentDelegate PurchaseProperty_SetMonthlyDownpayment;
	public delegate void PurchaseProperty_SetNumMonthlyPaymentsDelegate(int iNumPayments);
	public static PurchaseProperty_SetNumMonthlyPaymentsDelegate PurchaseProperty_SetNumMonthlyPayments;
	public delegate void PurchaseRadioDelegate();
	public static PurchaseRadioDelegate PurchaseRadio;
	public delegate void QuizCompleteDelegate(List<int> lstResponseIndexes);
	public static QuizCompleteDelegate QuizComplete;
	public delegate void ReloadCheckIntDataDelegate();
	public static ReloadCheckIntDataDelegate ReloadCheckIntData;
	public delegate void ReloadCheckVehDataDelegate();
	public static ReloadCheckVehDataDelegate ReloadCheckVehData;
	public delegate void RemovePhoneContactDelegate(string entryNumber, string entryName);
	public static RemovePhoneContactDelegate RemovePhoneContact;
	public delegate void ReportBugDelegate();
	public static ReportBugDelegate ReportBug;
	public delegate void RequestApplicationDetailsDelegate(int accountID);
	public static RequestApplicationDetailsDelegate RequestApplicationDetails;
	public delegate void RequestMergeItemDelegate(Int64 item_dbid);
	public static RequestMergeItemDelegate RequestMergeItem;
	public delegate void RequestMoveItemDelegate(bool is_socket, Int64 socket_or_item_dbid);
	public static RequestMoveItemDelegate RequestMoveItem;
	public delegate void RequestWrittenQuestionsDelegate();
	public static RequestWrittenQuestionsDelegate RequestWrittenQuestions;
	public delegate void ResetChatSettingsDelegate();
	public static ResetChatSettingsDelegate ResetChatSettings;
	public delegate void ResetFareDelegate();
	public static ResetFareDelegate ResetFare;
	public delegate void ResetSplitItemDelegate();
	public static ResetSplitItemDelegate ResetSplitItem;
	public delegate void RespawnFactionVehiclesDelegate(int factionIndex);
	public static RespawnFactionVehiclesDelegate RespawnFactionVehicles;
	public delegate void RestartQuizDelegate();
	public static RestartQuizDelegate RestartQuiz;
	public delegate void RoadblockEditor_HideDelegate();
	public static RoadblockEditor_HideDelegate RoadblockEditor_Hide;
	public delegate void RoadblockEditor_PlaceRoadblockDelegate(int index);
	public static RoadblockEditor_PlaceRoadblockDelegate RoadblockEditor_PlaceRoadblock;
	public delegate void RunPlateDelegate();
	public static RunPlateDelegate RunPlate;
	public delegate void SaveAdminNotesDelegate(string strNotes);
	public static SaveAdminNotesDelegate SaveAdminNotes;
	public delegate void SaveChatSettingsForTabDelegate(int tabIndex, string strJsonData);
	public static SaveChatSettingsForTabDelegate SaveChatSettingsForTab;
	public delegate void SaveChatSettingsGlobalDelegate(int max_messages_displayed, bool showChatboxBackground, float chatboxBackgroundAlpha);
	public static SaveChatSettingsGlobalDelegate SaveChatSettingsGlobal;
	public delegate void SaveControlDelegate(int ControlID, int NewKey);
	public static SaveControlDelegate SaveControl;
	public delegate void SaveInteriorNoteDelegate(string strNote);
	public static SaveInteriorNoteDelegate SaveInteriorNote;
	public delegate void SavePhoneContactDelegate(string entryNumber, string entryName);
	public static SavePhoneContactDelegate SavePhoneContact;
	public delegate void SaveRadioDelegate(string strName, string strEndpoint);
	public static SaveRadioDelegate SaveRadio;
	public delegate void SaveRanksAndSalariesDelegate(int factionIndex, string jsonData);
	public static SaveRanksAndSalariesDelegate SaveRanksAndSalaries;
	public delegate void SaveVehicleNoteDelegate(string strNote);
	public static SaveVehicleNoteDelegate SaveVehicleNote;
	public delegate void ScooterRental_CloseDelegate();
	public static ScooterRental_CloseDelegate ScooterRental_Close;
	public delegate void ScooterRental_RentDelegate();
	public static ScooterRental_RentDelegate ScooterRental_Rent;
	public delegate void SendSMSNotificationDelegate(string number);
	public static SendSMSNotificationDelegate SendSMSNotification;
	public delegate void SetAllControlsToDefaultDelegate();
	public static SetAllControlsToDefaultDelegate SetAllControlsToDefault;
	public delegate void SetAutoSpawnDelegate();
	public static SetAutoSpawnDelegate SetAutoSpawn;
	public delegate void SetMemberRankDelegate(int factionIndex, int memberIndex, int rankIndex);
	public static SetMemberRankDelegate SetMemberRank;
	public delegate void Shard_OnFadedOutDelegate();
	public static Shard_OnFadedOutDelegate Shard_OnFadedOut;
	public delegate void ShareDutyOutfit_SelectPlayer_CancelDelegate();
	public static ShareDutyOutfit_SelectPlayer_CancelDelegate ShareDutyOutfit_SelectPlayer_Cancel;
	public delegate void ShareDutyOutfit_SelectPlayer_DoneDelegate(string displayName, string value);
	public static ShareDutyOutfit_SelectPlayer_DoneDelegate ShareDutyOutfit_SelectPlayer_Done;
	public delegate void ShareDutyOutfit_SelectPlayer_DropdownSelectionChangedDelegate(string displayName, string value);
	public static ShareDutyOutfit_SelectPlayer_DropdownSelectionChangedDelegate ShareDutyOutfit_SelectPlayer_DropdownSelectionChanged;
	public delegate void ShowChatSettingsDelegate();
	public static ShowChatSettingsDelegate ShowChatSettings;
	public delegate void ShowGenericMessageBoxDelegate(string strTitle, string strCaption, string strButtonText, string strButtonEvent);
	public static ShowGenericMessageBoxDelegate ShowGenericMessageBox;
	public delegate void ShowItemInfoDelegate(int item_index, EItemSocket socket_id, float rootX, float rootY);
	public static ShowItemInfoDelegate ShowItemInfo;
	public delegate void SplitItemDelegate(uint new_item_num_stacks);
	public static SplitItemDelegate SplitItem;
	public delegate void SprayCan_EditTagDelegate();
	public static SprayCan_EditTagDelegate SprayCan_EditTag;
	public delegate void SprayCan_ExitDelegate();
	public static SprayCan_ExitDelegate SprayCan_Exit;
	public delegate void SprayCan_ShareTagDelegate();
	public static SprayCan_ShareTagDelegate SprayCan_ShareTag;
	public delegate void SprayCan_TagWallDelegate();
	public static SprayCan_TagWallDelegate SprayCan_TagWall;
	public delegate void StoreCheckoutDelegate(string strJsonData, int numItems);
	public static StoreCheckoutDelegate StoreCheckout;
	public delegate void SubmitAdminReportDelegate(EAdminReportType reportType, string strDetails, int playerID);
	public static SubmitAdminReportDelegate SubmitAdminReport;
	public delegate void SubmitAssetTransferDelegate(long fromCharacter, long toCharacter, float money, float bankMoney, string vehicles, string properties);
	public static SubmitAssetTransferDelegate SubmitAssetTransfer;
	public delegate void SubmitWrittenPortionDelegate(string strQ1Answer, string strQ2Answer, string strQ3Answer, string strQ4Answer);
	public static SubmitWrittenPortionDelegate SubmitWrittenPortion;
	public delegate void TattooArtist_AddNewDelegate();
	public static TattooArtist_AddNewDelegate TattooArtist_AddNew;
	public delegate void TattooArtist_CancelDelegate();
	public static TattooArtist_CancelDelegate TattooArtist_Cancel;
	public delegate void TattooArtist_ChangeTattooDelegate(int tattooID);
	public static TattooArtist_ChangeTattooDelegate TattooArtist_ChangeTattoo;
	public delegate void TattooArtist_ChangeZoneDelegate(TattooZone tattooZone);
	public static TattooArtist_ChangeZoneDelegate TattooArtist_ChangeZone;
	public delegate void TattooArtist_CreateDelegate();
	public static TattooArtist_CreateDelegate TattooArtist_Create;
	public delegate void TattooArtist_OnMouseEnterDelegate(string strElementName, int tattooID);
	public static TattooArtist_OnMouseEnterDelegate TattooArtist_OnMouseEnter;
	public delegate void TattooArtist_OnMouseExitDelegate(string strElementName, int tattooID);
	public static TattooArtist_OnMouseExitDelegate TattooArtist_OnMouseExit;
	public delegate void TattooArtist_RemoveTattooDelegate(string strElementName, int tattooID);
	public static TattooArtist_RemoveTattooDelegate TattooArtist_RemoveTattoo;
	public delegate void ToggleAvailableForHireDelegate();
	public static ToggleAvailableForHireDelegate ToggleAvailableForHire;
	public delegate void ToggleFactionManagerDelegate(int factionIndex, int memberIndex);
	public static ToggleFactionManagerDelegate ToggleFactionManager;
	public delegate void ToggleLocalNametagDelegate();
	public static ToggleLocalNametagDelegate ToggleLocalNametag;
	public delegate void ToggleSilentSirenDelegate();
	public static ToggleSilentSirenDelegate ToggleSilentSiren;
	public delegate void ToggleSpeedTrapDelegate();
	public static ToggleSpeedTrapDelegate ToggleSpeedTrap;
	public delegate void UnimpoundVehicleDelegate(int vehicleID);
	public static UnimpoundVehicleDelegate UnimpoundVehicle;
	public delegate void UpdateCharacterLook_CloseDelegate();
	public static UpdateCharacterLook_CloseDelegate UpdateCharacterLook_Close;
	public delegate void UpdateCharacterLook_SaveDelegate(Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup);
	public static UpdateCharacterLook_SaveDelegate UpdateCharacterLook_Save;
	public delegate void UpdateMessageViewedDelegate(string to);
	public static UpdateMessageViewedDelegate UpdateMessageViewed;
	public delegate void UpdateStolenStateDelegate(bool stolen);
	public static UpdateStolenStateDelegate UpdateStolenState;
	public delegate void UserInput_OnCloseDelegate();
	public static UserInput_OnCloseDelegate UserInput_OnClose;
	public delegate void VehicleCrusher_CrushDelegate();
	public static VehicleCrusher_CrushDelegate VehicleCrusher_Crush;
	public delegate void VehicleCrusher_ExitDelegate();
	public static VehicleCrusher_ExitDelegate VehicleCrusher_Exit;
	public delegate void VehicleModShop_ChangeModCategoryDelegate(int modCategory);
	public static VehicleModShop_ChangeModCategoryDelegate VehicleModShop_ChangeModCategory;
	public delegate void VehicleModShop_OnChangeNeonsColorDelegate(uint r, uint g, uint b);
	public static VehicleModShop_OnChangeNeonsColorDelegate VehicleModShop_OnChangeNeonsColor;
	public delegate void VehicleModShop_OnCheckoutDelegate();
	public static VehicleModShop_OnCheckoutDelegate VehicleModShop_OnCheckout;
	public delegate void VehicleModShop_OnExit_DiscardDelegate();
	public static VehicleModShop_OnExit_DiscardDelegate VehicleModShop_OnExit_Discard;
	public delegate void VehicleModShop_ResetCameraDelegate();
	public static VehicleModShop_ResetCameraDelegate VehicleModShop_ResetCamera;
	public delegate void VehicleModShop_ResetPlateDelegate();
	public static VehicleModShop_ResetPlateDelegate VehicleModShop_ResetPlate;
	public delegate void VehicleModShop_SetPlateTextDelegate(string strPlateText);
	public static VehicleModShop_SetPlateTextDelegate VehicleModShop_SetPlateText;
	public delegate void VehicleModShop_StartRotationDelegate(EVehicleStoreRotationDirection direction);
	public static VehicleModShop_StartRotationDelegate VehicleModShop_StartRotation;
	public delegate void VehicleModShop_StartZoomDelegate(EVehicleStoreZoomDirection direction);
	public static VehicleModShop_StartZoomDelegate VehicleModShop_StartZoom;
	public delegate void VehicleModShop_StopRotationDelegate();
	public static VehicleModShop_StopRotationDelegate VehicleModShop_StopRotation;
	public delegate void VehicleModShop_StopZoomDelegate();
	public static VehicleModShop_StopZoomDelegate VehicleModShop_StopZoom;
	public delegate void VehicleModShop_UpdateModIndexDelegate(int modCategory, int modIndex);
	public static VehicleModShop_UpdateModIndexDelegate VehicleModShop_UpdateModIndex;
	public delegate void VehicleModShop_UpdateNeonStateDelegate(bool neons_enabled);
	public static VehicleModShop_UpdateNeonStateDelegate VehicleModShop_UpdateNeonState;
	public delegate void VehicleModShop_UpdatePriceDelegate();
	public static VehicleModShop_UpdatePriceDelegate VehicleModShop_UpdatePrice;
	public delegate void VehicleRentalStore_HideDelegate();
	public static VehicleRentalStore_HideDelegate VehicleRentalStore_Hide;
	public delegate void VehicleRentalStore_OnChangeClassDelegate(string strClassName);
	public static VehicleRentalStore_OnChangeClassDelegate VehicleRentalStore_OnChangeClass;
	public delegate void VehicleRentalStore_OnChangePrimaryColorDelegate(uint r, uint g, uint b);
	public static VehicleRentalStore_OnChangePrimaryColorDelegate VehicleRentalStore_OnChangePrimaryColor;
	public delegate void VehicleRentalStore_OnChangeRentalLengthDelegate(uint lengthInDays);
	public static VehicleRentalStore_OnChangeRentalLengthDelegate VehicleRentalStore_OnChangeRentalLength;
	public delegate void VehicleRentalStore_OnChangeSecondaryColorDelegate(uint r, uint g, uint b);
	public static VehicleRentalStore_OnChangeSecondaryColorDelegate VehicleRentalStore_OnChangeSecondaryColor;
	public delegate void VehicleRentalStore_OnChangeVehicleDelegate(int vehicleIndex);
	public static VehicleRentalStore_OnChangeVehicleDelegate VehicleRentalStore_OnChangeVehicle;
	public delegate void VehicleRentalStore_OnCheckoutDelegate(int purchaserIndex);
	public static VehicleRentalStore_OnCheckoutDelegate VehicleRentalStore_OnCheckout;
	public delegate void VehicleRentalStore_ResetCameraDelegate();
	public static VehicleRentalStore_ResetCameraDelegate VehicleRentalStore_ResetCamera;
	public delegate void VehicleRentalStore_StartRotationDelegate(EVehicleStoreRotationDirection direction);
	public static VehicleRentalStore_StartRotationDelegate VehicleRentalStore_StartRotation;
	public delegate void VehicleRentalStore_StartZoomDelegate(EVehicleStoreZoomDirection direction);
	public static VehicleRentalStore_StartZoomDelegate VehicleRentalStore_StartZoom;
	public delegate void VehicleRentalStore_StopRotationDelegate();
	public static VehicleRentalStore_StopRotationDelegate VehicleRentalStore_StopRotation;
	public delegate void VehicleRentalStore_StopZoomDelegate();
	public static VehicleRentalStore_StopZoomDelegate VehicleRentalStore_StopZoom;
	public delegate void VehicleRentalStore_ToggleDoorsDelegate();
	public static VehicleRentalStore_ToggleDoorsDelegate VehicleRentalStore_ToggleDoors;
	public delegate void VehicleStore_HideDelegate();
	public static VehicleStore_HideDelegate VehicleStore_Hide;
	public delegate void VehicleStore_OnChangeClassDelegate(string strClassName);
	public static VehicleStore_OnChangeClassDelegate VehicleStore_OnChangeClass;
	public delegate void VehicleStore_OnChangePrimaryColorDelegate(uint r, uint g, uint b);
	public static VehicleStore_OnChangePrimaryColorDelegate VehicleStore_OnChangePrimaryColor;
	public delegate void VehicleStore_OnChangeSecondaryColorDelegate(uint r, uint g, uint b);
	public static VehicleStore_OnChangeSecondaryColorDelegate VehicleStore_OnChangeSecondaryColor;
	public delegate void VehicleStore_OnChangeVehicleDelegate(int vehicleIndex);
	public static VehicleStore_OnChangeVehicleDelegate VehicleStore_OnChangeVehicle;
	public delegate void VehicleStore_OnCheckoutDelegate(int purchaserIndex, EPaymentMethod method);
	public static VehicleStore_OnCheckoutDelegate VehicleStore_OnCheckout;
	public delegate void VehicleStore_ResetCameraDelegate();
	public static VehicleStore_ResetCameraDelegate VehicleStore_ResetCamera;
	public delegate void VehicleStore_SetMonthlyDownpaymentDelegate(float fDownpayment);
	public static VehicleStore_SetMonthlyDownpaymentDelegate VehicleStore_SetMonthlyDownpayment;
	public delegate void VehicleStore_SetNumMonthlyPaymentsDelegate(int iNumPayments);
	public static VehicleStore_SetNumMonthlyPaymentsDelegate VehicleStore_SetNumMonthlyPayments;
	public delegate void VehicleStore_StartRotationDelegate(EVehicleStoreRotationDirection direction);
	public static VehicleStore_StartRotationDelegate VehicleStore_StartRotation;
	public delegate void VehicleStore_StartZoomDelegate(EVehicleStoreZoomDirection direction);
	public static VehicleStore_StartZoomDelegate VehicleStore_StartZoom;
	public delegate void VehicleStore_StopRotationDelegate();
	public static VehicleStore_StopRotationDelegate VehicleStore_StopRotation;
	public delegate void VehicleStore_StopZoomDelegate();
	public static VehicleStore_StopZoomDelegate VehicleStore_StopZoom;
	public delegate void VehicleStore_ToggleDoorsDelegate();
	public static VehicleStore_ToggleDoorsDelegate VehicleStore_ToggleDoors;
	public delegate void ViewFactionVehiclesDelegate(int factionIndex);
	public static ViewFactionVehiclesDelegate ViewFactionVehicles;
}
