using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using GTANetworkAPI;
using PlayerType = GTANetworkAPI.Player;
using ObjectType = GTANetworkAPI.Object;
using VehicleType = GTANetworkAPI.Vehicle;
public static class NetworkEvents
{
	public delegate void ActivityRequestInteractDelegate(CPlayer a_Player);
	public static ActivityRequestInteractDelegate ActivityRequestInteract;
	public delegate void AdminClearGangTagsDelegate(CPlayer a_Player, Int64 tagID);
	public static AdminClearGangTagsDelegate AdminClearGangTags;
	public delegate void AdminDeleteElevatorDelegate(CPlayer a_Player, Int64 entityID);
	public static AdminDeleteElevatorDelegate AdminDeleteElevator;
	public delegate void AdminDeleteFactionDelegate(CPlayer a_Player, Int64 entityID);
	public static AdminDeleteFactionDelegate AdminDeleteFaction;
	public delegate void AdminDeletePropertyDelegate(CPlayer a_Player, Int64 entityID);
	public static AdminDeletePropertyDelegate AdminDeleteProperty;
	public delegate void AdminDeleteVehicleDelegate(CPlayer a_Player, Int64 entityID);
	public static AdminDeleteVehicleDelegate AdminDeleteVehicle;
	public delegate void AdminToggleItemLockDelegate(CPlayer a_Player, ObjectType worldItemObject);
	public static AdminToggleItemLockDelegate AdminToggleItemLock;
	public delegate void AdminToggleNoteLockDelegate(CPlayer a_Player, ObjectType worldItemObject);
	public static AdminToggleNoteLockDelegate AdminToggleNoteLock;
	public delegate void AdminTowGotVehiclesDelegate(CPlayer a_Player, List<Int64> lstVehicles);
	public static AdminTowGotVehiclesDelegate AdminTowGotVehicles;
	public delegate void ANPR_GetSpeedDelegate(CPlayer a_Player, Vehicle vehicle);
	public static ANPR_GetSpeedDelegate ANPR_GetSpeed;
	public delegate void AnswerCallDelegate(CPlayer a_Player);
	public static AnswerCallDelegate AnswerCall;
	public delegate void ApproveApplicationDelegate(CPlayer a_Player, int accountID);
	public static ApproveApplicationDelegate ApproveApplication;
	public delegate void AttemptEndDrivingTestDelegate(CPlayer a_Player, EDrivingTestType a_DrivingTestType);
	public static AttemptEndDrivingTestDelegate AttemptEndDrivingTest;
	public delegate void AttemptQuitJobDelegate(CPlayer a_Player);
	public static AttemptQuitJobDelegate AttemptQuitJob;
	public delegate void AttemptStartDrivingTestDelegate(CPlayer a_Player, EDrivingTestType a_DrivingTestType, EScriptLocation a_Location);
	public static AttemptStartDrivingTestDelegate AttemptStartDrivingTest;
	public delegate void AttemptStartJobDelegate(CPlayer a_Player, EJobID a_JobID, EScriptLocation a_Location);
	public static AttemptStartJobDelegate AttemptStartJob;
	public delegate void Banking_GetAccountInfoDelegate(CPlayer a_Player, EPurchaserType PurchaserType, Int64 PurchaserID);
	public static Banking_GetAccountInfoDelegate Banking_GetAccountInfo;
	public delegate void Banking_OnDepositDelegate(CPlayer a_Player, float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID);
	public static Banking_OnDepositDelegate Banking_OnDeposit;
	public delegate void Banking_OnPayDownDebtDelegate(CPlayer a_Player, EPurchaserType CreditSource, Int64 CreditSourceID, ECreditType CreditType, Int64 ID, float fAmount);
	public static Banking_OnPayDownDebtDelegate Banking_OnPayDownDebt;
	public delegate void Banking_OnWireTransferDelegate(CPlayer a_Player, string strTargetName, float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID);
	public static Banking_OnWireTransferDelegate Banking_OnWireTransfer;
	public delegate void Banking_OnWithdrawDelegate(CPlayer a_Player, float fAmount, EPurchaserType PurchaserType, Int64 PurchaserID);
	public static Banking_OnWithdrawDelegate Banking_OnWithdraw;
	public delegate void Banking_PayDownDebtDelegate(CPlayer a_Player, float fAmount);
	public static Banking_PayDownDebtDelegate Banking_PayDownDebt;
	public delegate void Banking_ShowMobileBankingUIDelegate(CPlayer a_Player);
	public static Banking_ShowMobileBankingUIDelegate Banking_ShowMobileBankingUI;
	public delegate void BarberShop_CalculatePriceDelegate(CPlayer a_Player, int BaseHair, int HairStyle, int HairColor, int HairColorHighlights, int ChestHairStyle, int ChestHairColor, int ChestHairColorHighlights, float ChestHairOpacity, int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlights, float FacialHairOpacity, int FullBeardStyle, int FullBeardColor);
	public static BarberShop_CalculatePriceDelegate BarberShop_CalculatePrice;
	public delegate void BarberShop_OnCheckoutDelegate(CPlayer a_Player, Int64 storeID, int BaseHair, int HairStyle, int HairColor, int HairColorHighlights, int ChestHairStyle, int ChestHairColor, int ChestHairColorHighlights, float ChestHairOpacity, int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlights, float FacialHairOpacity, int FullBeardStyle, int FullBeardColor);
	public static BarberShop_OnCheckoutDelegate BarberShop_OnCheckout;
	public delegate void BlackJack_Action_HitMeDelegate(CPlayer a_Player, Int64 uniqueIdentifier);
	public static BlackJack_Action_HitMeDelegate BlackJack_Action_HitMe;
	public delegate void BlackJack_Action_StickDelegate(CPlayer a_Player, Int64 uniqueIdentifier);
	public static BlackJack_Action_StickDelegate BlackJack_Action_Stick;
	public delegate void Blackjack_PlaceBetDelegate(CPlayer a_Player, Int64 uniqueIdentifier, int amount);
	public static Blackjack_PlaceBetDelegate Blackjack_PlaceBet;
	public delegate void Blackjack_PlaceBet_GetDetailsDelegate(CPlayer a_Player);
	public static Blackjack_PlaceBet_GetDetailsDelegate Blackjack_PlaceBet_GetDetails;
	public delegate void BlipSiren_RequestDelegate(CPlayer a_Player);
	public static BlipSiren_RequestDelegate BlipSiren_Request;
	public delegate void CallNumberDelegate(CPlayer a_Player, string a_strNumber);
	public static CallNumberDelegate CallNumber;
	public delegate void CallTaxiDelegate(CPlayer a_Player);
	public static CallTaxiDelegate CallTaxi;
	public delegate void CancelAdminReportDelegate(CPlayer a_Player);
	public static CancelAdminReportDelegate CancelAdminReport;
	public delegate void CancelCallDelegate(CPlayer a_Player);
	public static CancelCallDelegate CancelCall;
	public delegate void CancelGoingOnDutyDelegate(CPlayer a_Player);
	public static CancelGoingOnDutyDelegate CancelGoingOnDuty;
	public delegate void CancelTaggingInProgressDelegate(CPlayer a_Player);
	public static CancelTaggingInProgressDelegate CancelTaggingInProgress;
	public delegate void CancelTaxiDelegate(CPlayer a_Player);
	public static CancelTaxiDelegate CancelTaxi;
	public delegate void CasinoManagement_AddDelegate(CPlayer a_Player, Int64 uniqueActivityIdentifier, int amount);
	public static CasinoManagement_AddDelegate CasinoManagement_Add;
	public delegate void CasinoManagement_GetDetailsDelegate(CPlayer a_Player, Int64 uniqueActivityIdentifier);
	public static CasinoManagement_GetDetailsDelegate CasinoManagement_GetDetails;
	public delegate void CasinoManagement_TakeDelegate(CPlayer a_Player, Int64 uniqueActivityIdentifier, int amount);
	public static CasinoManagement_TakeDelegate CasinoManagement_Take;
	public delegate void ChangeBoomboxRadioDelegate(CPlayer a_Player, ObjectType worldItemObject, string strStationName, int a_RadioID);
	public static ChangeBoomboxRadioDelegate ChangeBoomboxRadio;
	public delegate void ChangeFarePerMileDelegate(CPlayer a_Player, float fCharge);
	public static ChangeFarePerMileDelegate ChangeFarePerMile;
	public delegate void ChangeVehicleRadioDelegate(CPlayer a_Player, int a_RadioID);
	public static ChangeVehicleRadioDelegate ChangeVehicleRadio;
	public delegate void CharacterChangeRequestedDelegate(CPlayer a_Player);
	public static CharacterChangeRequestedDelegate CharacterChangeRequested;
	public static void SendLocalEvent_CharacterChangeRequested(CPlayer a_Player) { NetworkEvents.CharacterChangeRequested?.Invoke(a_Player); }
	public delegate void CharacterSelectedDelegate(CPlayer a_Player, long characterID);
	public static CharacterSelectedDelegate CharacterSelected;
	public delegate void CharacterSelectedLocalDelegate(CPlayer a_Player, long characterID);
	public static CharacterSelectedLocalDelegate CharacterSelectedLocal;
	public static void SendLocalEvent_CharacterSelectedLocal(CPlayer a_Player, long characterID) { NetworkEvents.CharacterSelectedLocal?.Invoke(a_Player, characterID); }
	public delegate void CharacterSpawnedDelegate(CPlayer a_Player);
	public static CharacterSpawnedDelegate CharacterSpawned;
	public static void SendLocalEvent_CharacterSpawned(CPlayer a_Player) { NetworkEvents.CharacterSpawned?.Invoke(a_Player); }
	public delegate void CheckpointBasedJob_GotoCheckpointStateDelegate(CPlayer a_Player);
	public static CheckpointBasedJob_GotoCheckpointStateDelegate CheckpointBasedJob_GotoCheckpointState;
	public delegate void CheckpointBasedJob_VerifyCheckpointDelegate(CPlayer a_Player);
	public static CheckpointBasedJob_VerifyCheckpointDelegate CheckpointBasedJob_VerifyCheckpoint;
	public delegate void ChipManagement_BuyDelegate(CPlayer a_Player, int amount);
	public static ChipManagement_BuyDelegate ChipManagement_Buy;
	public delegate void ChipManagement_Buy_GetDetailsDelegate(CPlayer a_Player);
	public static ChipManagement_Buy_GetDetailsDelegate ChipManagement_Buy_GetDetails;
	public delegate void ChipManagement_SellDelegate(CPlayer a_Player, int amount);
	public static ChipManagement_SellDelegate ChipManagement_Sell;
	public delegate void ChipManagement_Sell_GetDetailsDelegate(CPlayer a_Player);
	public static ChipManagement_Sell_GetDetailsDelegate ChipManagement_Sell_GetDetails;
	public delegate void ClientsideExceptionDelegate(CPlayer a_Player, ClientsideException exceptionObject);
	public static ClientsideExceptionDelegate ClientsideException;
	public delegate void CloseFurnitureInventoryDelegate(CPlayer a_Player);
	public static CloseFurnitureInventoryDelegate CloseFurnitureInventory;
	public delegate void ClosePhoneDelegate(CPlayer a_Player);
	public static ClosePhoneDelegate ClosePhone;
	public delegate void ClosePropertyInventoryDelegate(CPlayer a_Player);
	public static ClosePropertyInventoryDelegate ClosePropertyInventory;
	public delegate void CloseVehicleInventoryDelegate(CPlayer a_Player, VehicleType currentVehicle, EVehicleInventoryType currentVehicleInventoryType, bool bIsInvertedTrunk);
	public static CloseVehicleInventoryDelegate CloseVehicleInventory;
	public delegate void ClothingStore_CalculatePriceDelegate(CPlayer a_Player, int[] DrawablesClothing, int[] TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, uint skinID);
	public static ClothingStore_CalculatePriceDelegate ClothingStore_CalculatePrice;
	public delegate void ClothingStore_OnCheckoutDelegate(CPlayer a_Player, Int64 storeID, int[] DrawablesClothing, int[] TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, uint skinID);
	public static ClothingStore_OnCheckoutDelegate ClothingStore_OnCheckout;
	public delegate void ConsumeDonationPerkDelegate(CPlayer a_Player, UInt32 id);
	public static ConsumeDonationPerkDelegate ConsumeDonationPerk;
	public delegate void CreateCharacterCustomDelegate(CPlayer a_Player, EScriptLocation spawn, EGender gender, string strName, uint SkinHash, int Age, int[] drawables, int[] Textures, Dictionary<ECustomPropSlot, int> PropsDrawables, Dictionary<ECustomPropSlot, int> PropsTextures, int Ageing, float AgeingOpacity, int Makeup, float MakeupOpacity, int MakeupColor, int MakeupColorHighlight, int Blush, float BlushOpacity, int BlushColor, int BlushColorHighlight, int Complexion, float ComplexionOpacity, int SunDamage, float SunDamageOpacity, int Lipstick, float LipstickOpacity, int LipstickColor, int LipstickColorHighlights, int MolesAndFreckles, float MolesAndFrecklesOpacity, float NoseSizeHorizontal, float NoseSizeVertical, float NoseSizeOutwards, float NoseSizeOutwardsUpper, float NoseSizeOutwardsLower, float NoseAngle, float EyebrowHeight, float EyebrowDepth, float CheekboneHeight, float CheekWidth, float CheekWidthLower, float EyeSize, float LipSize, float MouthSize, float MouthSizeLower, float ChinSize, float ChinSizeLower, float ChinWidth, float ChinEffect, float NeckWidth, float NeckWidthLower, int FaceBlend1Mother, int FaceBlend1Father, float FaceBlendFatherPercent, float SkinBlendFatherPercent, int BaseHair, int HairColor, int HairColorHighlights, int EyeColor, int FacialHairStyle, int FacialHairColor, int FacialHairColorHighlight, float FacialHairOpacity, int Blemishes, float BlemishesOpacity, int Eyebrows, float EyebrowsOpacity, int EyebrowsColor, int EyebrowsColorHighlight, List<int> lstTattooIDs, int BodyBlemishes, float BodyBlemishesOpacity, int ChestHair, int ChestHairColor, int ChestHairColorHighlight, float ChestHairOpacity, ECharacterLanguage PrimaryLanguage, ECharacterLanguage SecondaryLanguage);
	public static CreateCharacterCustomDelegate CreateCharacterCustom;
	public delegate void CreateCharacterPremadeDelegate(CPlayer a_Player, EScriptLocation spawn, EGender gender, string strName, uint SkinHash, int Age, int[] drawables, int[] Textures, Dictionary<ECustomPropSlot, int> PropsDrawables, Dictionary<ECustomPropSlot, int> PropsTextures, ECharacterLanguage PrimaryLanguage, ECharacterLanguage SecondaryLanguage);
	public static CreateCharacterPremadeDelegate CreateCharacterPremade;
	public delegate void CreateInfoMarker_ResponseDelegate(CPlayer a_Player, string strText, float x, float y, float z);
	public static CreateInfoMarker_ResponseDelegate CreateInfoMarker_Response;
	public delegate void CreateKeybindDelegate(CPlayer a_Player, ConsoleKey key, EPlayerKeyBindType bindType, string strAction);
	public static CreateKeybindDelegate CreateKeybind;
	public delegate void CreatePhoneMessageDelegate(CPlayer a_Player, string to, string content);
	public static CreatePhoneMessageDelegate CreatePhoneMessage;
	public delegate void CuffPlayerDelegate(CPlayer a_Player, PlayerType targetPlayer);
	public static CuffPlayerDelegate CuffPlayer;
	public delegate void CustomAnim_CreateDelegate(CPlayer a_Player, string commandName, string animDictionary, string animName, bool loop, bool stopOnLastFrame, bool onlyAnimateUpperBody, bool allowPlayerMovement, int durationSeconds, bool isSilent);
	public static CustomAnim_CreateDelegate CustomAnim_Create;
	public delegate void CustomAnim_DeleteDelegate(CPlayer a_Player, string commandName);
	public static CustomAnim_DeleteDelegate CustomAnim_Delete;
	public delegate void CustomInterior_CustomMapTransfer_CancelDelegate(CPlayer a_Player);
	public static CustomInterior_CustomMapTransfer_CancelDelegate CustomInterior_CustomMapTransfer_Cancel;
	public delegate void CustomInterior_CustomMapTransfer_SendBytesDelegate(CPlayer a_Player, byte[] dataBytes);
	public static CustomInterior_CustomMapTransfer_SendBytesDelegate CustomInterior_CustomMapTransfer_SendBytes;
	public delegate void CustomInterior_CustomMapTransfer_StartDelegate(CPlayer a_Player, string mapType, long propertyID, float markerX, float markerY, float markerZ, int expectedBytesLen, int crc);
	public static CustomInterior_CustomMapTransfer_StartDelegate CustomInterior_CustomMapTransfer_Start;
	public delegate void DeleteInfoMarkerDelegate(CPlayer a_Player, Int64 a_InfoMarkerID);
	public static DeleteInfoMarkerDelegate DeleteInfoMarker;
	public delegate void DeleteKeybindDelegate(CPlayer a_Player, int index);
	public static DeleteKeybindDelegate DeleteKeybind;
	public delegate void DenyApplicationDelegate(CPlayer a_Player, int accountID);
	public static DenyApplicationDelegate DenyApplication;
	public delegate void DiscordDeLinkDelegate(CPlayer a_Player);
	public static DiscordDeLinkDelegate DiscordDeLink;
	public delegate void DiscordLinkFinalizeDelegate(CPlayer a_Player, string strURL);
	public static DiscordLinkFinalizeDelegate DiscordLinkFinalize;
	public delegate void DoCharacterTypeUpgradeDelegate(CPlayer a_Player);
	public static DoCharacterTypeUpgradeDelegate DoCharacterTypeUpgrade;
	public delegate void Donation_RequestInactivityEntitiesDelegate(CPlayer a_Player, EDonationInactivityEntityType entityType);
	public static Donation_RequestInactivityEntitiesDelegate Donation_RequestInactivityEntities;
	public delegate void DrivingTest_GetNextCheckpointDelegate(CPlayer a_Player, bool bVisualDamage);
	public static DrivingTest_GetNextCheckpointDelegate DrivingTest_GetNextCheckpoint;
	public delegate void DrivingTest_GotoCheckpointStateDelegate(CPlayer a_Player);
	public static DrivingTest_GotoCheckpointStateDelegate DrivingTest_GotoCheckpointState;
	public delegate void DrivingTest_GotoReturnVehicleDelegate(CPlayer a_Player, bool bSuccess, float x, float y, float z);
	public static DrivingTest_GotoReturnVehicleDelegate DrivingTest_GotoReturnVehicle;
	public delegate void DrivingTest_ReturnVehicleDelegate(CPlayer a_Player, bool bVisualDamage);
	public static DrivingTest_ReturnVehicleDelegate DrivingTest_ReturnVehicle;
	public delegate void DutyOutfitEditor_CreateOrUpdateOutfitDelegate(CPlayer a_Player, string Name, EDutyType a_DutyType, Dictionary<ECustomClothingComponent, int> DrawablesClothing, Dictionary<ECustomClothingComponent, int> TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables, Dictionary<ECustomPropSlot, int> CurrentPropTextures, Dictionary<EDutyWeaponSlot, EItemID> Loadout, Int64 outfitID, EDutyOutfitType charType, uint premadeHash, bool bHideHair);
	public static DutyOutfitEditor_CreateOrUpdateOutfitDelegate DutyOutfitEditor_CreateOrUpdateOutfit;
	public delegate void DutyOutfitEditor_DeleteOutfitDelegate(CPlayer a_Player, Int64 outfitID);
	public static DutyOutfitEditor_DeleteOutfitDelegate DutyOutfitEditor_DeleteOutfit;
	public delegate void DutySystem_RequestUpdatedOutfitListDelegate(CPlayer a_Player, EDutyType a_DutyType);
	public static DutySystem_RequestUpdatedOutfitListDelegate DutySystem_RequestUpdatedOutfitList;
	public delegate void EditInterior_CommitChangeDelegate(CPlayer a_Player, float x, float y, float z, float rx, float ry, float rz, long dbid);
	public static EditInterior_CommitChangeDelegate EditInterior_CommitChange;
	public delegate void EditInterior_PickupFurnitureDelegate(CPlayer a_Player, long dbid);
	public static EditInterior_PickupFurnitureDelegate EditInterior_PickupFurniture;
	public delegate void EditInterior_PlaceFurnitureDelegate(CPlayer a_Player, float x, float y, float z, long dbid);
	public static EditInterior_PlaceFurnitureDelegate EditInterior_PlaceFurniture;
	public delegate void EditInterior_RemoveDefaultFurnitureDelegate(CPlayer a_Player, float x, float y, float z, uint model);
	public static EditInterior_RemoveDefaultFurnitureDelegate EditInterior_RemoveDefaultFurniture;
	public delegate void EditInterior_RestoreFurnitureDelegate(CPlayer a_Player, long dbid);
	public static EditInterior_RestoreFurnitureDelegate EditInterior_RestoreFurniture;
	public delegate void EndCallDelegate(CPlayer a_Player);
	public static EndCallDelegate EndCall;
	public delegate void EndFourthOfJulyEventDelegate(CPlayer a_Player);
	public static EndFourthOfJulyEventDelegate EndFourthOfJulyEvent;
	public delegate void EnterBarberShopDelegate(CPlayer a_Player);
	public static EnterBarberShopDelegate EnterBarberShop;
	public delegate void EnterClothingStoreDelegate(CPlayer a_Player);
	public static EnterClothingStoreDelegate EnterClothingStore;
	public delegate void EnterDutyOutfitEditorDelegate(CPlayer a_Player, EDutyType a_DutyType);
	public static EnterDutyOutfitEditorDelegate EnterDutyOutfitEditor;
	public delegate void EnterOutfitEditorDelegate(CPlayer a_Player);
	public static EnterOutfitEditorDelegate EnterOutfitEditor;
	public delegate void EnterPlasticSurgeonDelegate(CPlayer a_Player);
	public static EnterPlasticSurgeonDelegate EnterPlasticSurgeon;
	public delegate void EnterTattooArtistDelegate(CPlayer a_Player);
	public static EnterTattooArtistDelegate EnterTattooArtist;
	public delegate void ExitFactionMenuDelegate(CPlayer a_Player);
	public static ExitFactionMenuDelegate ExitFactionMenu;
	public delegate void ExitGenericCharacterCustomizationDelegate(CPlayer a_Player);
	public static ExitGenericCharacterCustomizationDelegate ExitGenericCharacterCustomization;
	public delegate void ExtendRadio30DaysDelegate(CPlayer a_Player, int a_RadioID);
	public static ExtendRadio30DaysDelegate ExtendRadio30Days;
	public delegate void ExtendRadio7DaysDelegate(CPlayer a_Player, int a_RadioID);
	public static ExtendRadio7DaysDelegate ExtendRadio7Days;
	public delegate void FactionInviteDecisionDelegate(CPlayer a_Player, bool bAccepted, Int64 FactionID);
	public static FactionInviteDecisionDelegate FactionInviteDecision;
	public delegate void Faction_AdminRequestViewFactionsDelegate(CPlayer a_Player);
	public static Faction_AdminRequestViewFactionsDelegate Faction_AdminRequestViewFactions;
	public delegate void Faction_AdminViewFactions_DeleteFactionDelegate(CPlayer a_Player, Int64 FactionID);
	public static Faction_AdminViewFactions_DeleteFactionDelegate Faction_AdminViewFactions_DeleteFaction;
	public delegate void Faction_AdminViewFactions_JoinFactionDelegate(CPlayer a_Player, Int64 FactionID);
	public static Faction_AdminViewFactions_JoinFactionDelegate Faction_AdminViewFactions_JoinFaction;
	public delegate void Faction_CreateFactionDelegate(CPlayer a_Player, string strFullName, string strShortName, string strFactionType);
	public static Faction_CreateFactionDelegate Faction_CreateFaction;
	public delegate void Faction_DisbandFactionDelegate(CPlayer a_Player, int factionIndex);
	public static Faction_DisbandFactionDelegate Faction_DisbandFaction;
	public delegate void Faction_EditMessageDelegate(CPlayer a_Player, int factionIndex, string strMessage);
	public static Faction_EditMessageDelegate Faction_EditMessage;
	public delegate void Faction_InvitePlayerDelegate(CPlayer a_Player, int factionIndex, string strPlayerName);
	public static Faction_InvitePlayerDelegate Faction_InvitePlayer;
	public delegate void Faction_KickDelegate(CPlayer a_Player, int factionIndex, int member_id);
	public static Faction_KickDelegate Faction_Kick;
	public delegate void Faction_LeaveFactionDelegate(CPlayer a_Player, int factionIndex);
	public static Faction_LeaveFactionDelegate Faction_LeaveFaction;
	public delegate void Faction_RequestFactionInfoDelegate(CPlayer a_Player);
	public static Faction_RequestFactionInfoDelegate Faction_RequestFactionInfo;
	public delegate void Faction_RespawnFactionVehiclesDelegate(CPlayer a_Player, int factionIndex);
	public static Faction_RespawnFactionVehiclesDelegate Faction_RespawnFactionVehicles;
	public delegate void Faction_SaveRanksAndSalariesDelegate(CPlayer a_Player, int factionIndex, string jsonData);
	public static Faction_SaveRanksAndSalariesDelegate Faction_SaveRanksAndSalaries;
	public delegate void Faction_SetMemberRankDelegate(CPlayer a_Player, int faction_id, int member_id, int rank_id);
	public static Faction_SetMemberRankDelegate Faction_SetMemberRank;
	public delegate void Faction_ToggleManagerDelegate(CPlayer a_Player, int factionIndex, int memberIndex);
	public static Faction_ToggleManagerDelegate Faction_ToggleManager;
	public delegate void Faction_ViewFactionVehiclesDelegate(CPlayer a_Player, int factionIndex);
	public static Faction_ViewFactionVehiclesDelegate Faction_ViewFactionVehicles;
	public delegate void FetchTransferAssetsDataDelegate(CPlayer a_Player, long characterId);
	public static FetchTransferAssetsDataDelegate FetchTransferAssetsData;
	public delegate void FinalizeGoOnDutyDelegate(CPlayer a_Player, EDutyType dutyType, Int64 outfitID);
	public static FinalizeGoOnDutyDelegate FinalizeGoOnDuty;
	public delegate void FinalizeLicenseDeviceDelegate(CPlayer a_Player, string strTargetName, EWeaponLicenseType weaponLicenseType, bool isRemoval);
	public static FinalizeLicenseDeviceDelegate FinalizeLicenseDevice;
	public delegate void FinishTutorialStateDelegate(CPlayer a_Player);
	public static FinishTutorialStateDelegate FinishTutorialState;
	public delegate void FireHeliDropWaterRequestDelegate(CPlayer a_Player);
	public static FireHeliDropWaterRequestDelegate FireHeliDropWaterRequest;
	public delegate void FireMissionCompleteDelegate(CPlayer a_Player);
	public static FireMissionCompleteDelegate FireMissionComplete;
	public delegate void FirePartialCleanupDelegate(CPlayer a_Player, List<int> cleanedUpSlots);
	public static FirePartialCleanupDelegate FirePartialCleanup;
	public delegate void Fishing_OnBiteOutcomeDelegate(CPlayer a_Player, int correct, int total);
	public static Fishing_OnBiteOutcomeDelegate Fishing_OnBiteOutcome;
	public delegate void ForceReSelectCharacterDelegate(CPlayer a_Player, long CharacterID);
	public static ForceReSelectCharacterDelegate ForceReSelectCharacter;
	public static void SendLocalEvent_ForceReSelectCharacter(CPlayer a_Player, long CharacterID) { NetworkEvents.ForceReSelectCharacter?.Invoke(a_Player, CharacterID); }
	public delegate void FriskPlayerDelegate(CPlayer a_Player, PlayerType targetPlayer);
	public static FriskPlayerDelegate FriskPlayer;
	public delegate void FurnitureStore_OnCheckoutDelegate(CPlayer a_Player, Int64 storeID, uint FurnitureIndex);
	public static FurnitureStore_OnCheckoutDelegate FurnitureStore_OnCheckout;
	public delegate void GangTags_AcceptShareDelegate(CPlayer a_Player, List<GangTagLayer> lstLayers);
	public static GangTags_AcceptShareDelegate GangTags_AcceptShare;
	public delegate void GangTags_SaveActiveDelegate(CPlayer a_Player, List<GangTagLayer> lstLayers);
	public static GangTags_SaveActiveDelegate GangTags_SaveActive;
	public delegate void GangTags_SaveWIPDelegate(CPlayer a_Player, List<GangTagLayer> lstLayers);
	public static GangTags_SaveWIPDelegate GangTags_SaveWIP;
	public delegate void GangTags_ShareTagDelegate(CPlayer a_Player, string strTargetName);
	public static GangTags_ShareTagDelegate GangTags_ShareTag;
	public delegate void Generics_SpawnGenericsDelegate(CPlayer a_Player, string genericName, string model, int amount, float price);
	public static Generics_SpawnGenericsDelegate Generics_SpawnGenerics;
	public delegate void Generics_UpdateGenericPositionDelegate(CPlayer a_Player, float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, ObjectType item);
	public static Generics_UpdateGenericPositionDelegate Generics_UpdateGenericPosition;
	public delegate void GetBasicDonatorInfoDelegate(CPlayer a_Player);
	public static GetBasicDonatorInfoDelegate GetBasicDonatorInfo;
	public delegate void GetBasicRadioInfoDelegate(CPlayer a_Player);
	public static GetBasicRadioInfoDelegate GetBasicRadioInfo;
	public delegate void GetPhoneContactByNumberDelegate(CPlayer a_Player, string callingNumber);
	public static GetPhoneContactByNumberDelegate GetPhoneContactByNumber;
	public delegate void GetPhoneContactsDelegate(CPlayer a_Player);
	public static GetPhoneContactsDelegate GetPhoneContacts;
	public delegate void GetPhoneMessagesContactsDelegate(CPlayer a_Player);
	public static GetPhoneMessagesContactsDelegate GetPhoneMessagesContacts;
	public delegate void GetPhoneMessagesFromNumberDelegate(CPlayer a_Player, string callingNumber);
	public static GetPhoneMessagesFromNumberDelegate GetPhoneMessagesFromNumber;
	public delegate void GetPhoneStateDelegate(CPlayer a_Player, bool isVisible);
	public static GetPhoneStateDelegate GetPhoneState;
	public delegate void GetPosDelegate(CPlayer a_Player);
	public static GetPosDelegate GetPos;
	public delegate void GetPurchaserAndPaymentMethodsDelegate(CPlayer a_Player, EPurchaseAndPaymentMethodsRequestType requestType);
	public static GetPurchaserAndPaymentMethodsDelegate GetPurchaserAndPaymentMethods;
	public delegate void GetStoreInfoDelegate(CPlayer a_Player, Int64 storeID);
	public static GetStoreInfoDelegate GetStoreInfo;
	public delegate void GetTotalUnviewedMessagesDelegate(CPlayer a_Player);
	public static GetTotalUnviewedMessagesDelegate GetTotalUnviewedMessages;
	public delegate void GotoDiscordLinkingDelegate(CPlayer a_Player);
	public static GotoDiscordLinkingDelegate GotoDiscordLinking;
	public delegate void GotoVehicleModShopDelegate(CPlayer a_Player);
	public static GotoVehicleModShopDelegate GotoVehicleModShop;
	public delegate void HalloweenCoffinDelegate(CPlayer a_Player, bool bInPieces);
	public static HalloweenCoffinDelegate HalloweenCoffin;
	public delegate void HalloweenInteractionDelegate(CPlayer a_Player);
	public static HalloweenInteractionDelegate HalloweenInteraction;
	public delegate void HelpRequestCommandsDelegate(CPlayer a_Player);
	public static HelpRequestCommandsDelegate HelpRequestCommands;
	public delegate void JerryCanRefuelVehicleDelegate(CPlayer a_Player, Int64 itemDBID, Int64 vehicleDBID);
	public static JerryCanRefuelVehicleDelegate JerryCanRefuelVehicle;
	public delegate void LargeDataTransfer_AckFinalTransferDelegate(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_AckFinalTransferDelegate LargeDataTransfer_AckFinalTransfer;
	public delegate void LargeDataTransfer_ClientToServer_BeginDelegate(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, byte[] key);
	public static LargeDataTransfer_ClientToServer_BeginDelegate LargeDataTransfer_ClientToServer_Begin;
	public delegate void LargeDataTransfer_SendBytesDelegate(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier, byte[] dataBytes);
	public static LargeDataTransfer_SendBytesDelegate LargeDataTransfer_SendBytes;
	public delegate void LargeDataTransfer_ServerToClient_AckBlockDelegate(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_ServerToClient_AckBlockDelegate LargeDataTransfer_ServerToClient_AckBlock;
	public delegate void LargeDataTransfer_ServerToClient_AckFinalTransferDelegate(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_ServerToClient_AckFinalTransferDelegate LargeDataTransfer_ServerToClient_AckFinalTransfer;
	public delegate void LargeDataTransfer_ServerToClient_ClientAckDelegate(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier, bool bNeedsTransfer);
	public static LargeDataTransfer_ServerToClient_ClientAckDelegate LargeDataTransfer_ServerToClient_ClientAck;
	public delegate void LargeDataTransfer_ServerToClient_RequestResendDelegate(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier);
	public static LargeDataTransfer_ServerToClient_RequestResendDelegate LargeDataTransfer_ServerToClient_RequestResend;
	public delegate void LocksmithOnPickupKeysDelegate(CPlayer a_Player);
	public static LocksmithOnPickupKeysDelegate LocksmithOnPickupKeys;
	public delegate void LocksmithRequestDuplicationDelegate(CPlayer a_Player, string strKeyType, Int64 keyID);
	public static LocksmithRequestDuplicationDelegate LocksmithRequestDuplication;
	public delegate void LoginPlayerDelegate(CPlayer a_Player, string strUsername, string strPassword, bool bAutoLogin);
	public static LoginPlayerDelegate LoginPlayer;
	public delegate void Marijuana_OnFertilizeDelegate(CPlayer a_Player, ObjectType worldObject);
	public static Marijuana_OnFertilizeDelegate Marijuana_OnFertilize;
	public delegate void Marijuana_OnGetSeedsDelegate(CPlayer a_Player);
	public static Marijuana_OnGetSeedsDelegate Marijuana_OnGetSeeds;
	public delegate void Marijuana_OnSellDrugsDelegate(CPlayer a_Player, uint count);
	public static Marijuana_OnSellDrugsDelegate Marijuana_OnSellDrugs;
	public delegate void Marijuana_OnSheerDelegate(CPlayer a_Player, ObjectType worldObject);
	public static Marijuana_OnSheerDelegate Marijuana_OnSheer;
	public delegate void Marijuana_OnWaterDelegate(CPlayer a_Player, ObjectType worldObject);
	public static Marijuana_OnWaterDelegate Marijuana_OnWater;
	public delegate void MdcGotoPersonDelegate(CPlayer a_Player, string strName);
	public static MdcGotoPersonDelegate MdcGotoPerson;
	public delegate void MdcGotoPropertyDelegate(CPlayer a_Player, Int64 propertyID);
	public static MdcGotoPropertyDelegate MdcGotoProperty;
	public delegate void MdcGotoVehicleDelegate(CPlayer a_Player, Int64 vehicleID);
	public static MdcGotoVehicleDelegate MdcGotoVehicle;
	public delegate void MergeItemDelegate(CPlayer a_Player, Int64 source_item_dbid, Int64 target_item_dbid);
	public static MergeItemDelegate MergeItem;
	public delegate void MoveToRappelPositionDelegate(CPlayer a_Player, int seat);
	public static MoveToRappelPositionDelegate MoveToRappelPosition;
	public delegate void NewsCameraOperatorDelegate(CPlayer a_Player, ObjectType NewsCameraObject);
	public static NewsCameraOperatorDelegate NewsCameraOperator;
	public delegate void OnDestroyItemDelegate(CPlayer a_Player, Int64 item_dbid);
	public static OnDestroyItemDelegate OnDestroyItem;
	public delegate void OnDropItemDelegate(CPlayer a_Player, Int64 item_dbid, float x, float y, float z);
	public static OnDropItemDelegate OnDropItem;
	public delegate void OnEndFriskingDelegate(CPlayer a_Player);
	public static OnEndFriskingDelegate OnEndFrisking;
	public delegate void OnFriskTakeItemDelegate(CPlayer a_Player, Int64 itemID);
	public static OnFriskTakeItemDelegate OnFriskTakeItem;
	public delegate void OnInteractWithDancerDelegate(CPlayer a_Player, Int64 dancerID);
	public static OnInteractWithDancerDelegate OnInteractWithDancer;
	public delegate void OnOperateNewsCameraDelegate(CPlayer a_Player);
	public static OnOperateNewsCameraDelegate OnOperateNewsCamera;
	public delegate void OnOwnerCollectDancerTipsDelegate(CPlayer a_Player, Int64 dancerID);
	public static OnOwnerCollectDancerTipsDelegate OnOwnerCollectDancerTips;
	public delegate void OnPickupItemDelegate(CPlayer a_Player, ObjectType worldItemObject);
	public static OnPickupItemDelegate OnPickupItem;
	public delegate void OnPickupStripsDelegate(CPlayer a_Player, ObjectType spikeStripObject);
	public static OnPickupStripsDelegate OnPickupStrips;
	public delegate void OnPlayerConnectedDelegate(CPlayer a_Player);
	public static OnPlayerConnectedDelegate OnPlayerConnected;
	public static void SendLocalEvent_OnPlayerConnected(CPlayer a_Player) { NetworkEvents.OnPlayerConnected?.Invoke(a_Player); }
	public delegate void OnPlayerDisconnectedDelegate(CPlayer a_Player, DisconnectionType type, string reason);
	public static OnPlayerDisconnectedDelegate OnPlayerDisconnected;
	public static void SendLocalEvent_OnPlayerDisconnected(CPlayer a_Player, DisconnectionType type, string reason) { NetworkEvents.OnPlayerDisconnected?.Invoke(a_Player, type, reason); }
	public delegate void OnPropertyFurnitureInstanceCreatedDelegate(CPropertyFurnitureInstance FurnitureInstance);
	public static OnPropertyFurnitureInstanceCreatedDelegate OnPropertyFurnitureInstanceCreated;
	public static void SendLocalEvent_OnPropertyFurnitureInstanceCreated(CPropertyFurnitureInstance FurnitureInstance) { NetworkEvents.OnPropertyFurnitureInstanceCreated?.Invoke(FurnitureInstance); }
	public delegate void OnPropertyFurnitureInstanceDestroyedDelegate(CPropertyFurnitureInstance FurnitureInstance);
	public static OnPropertyFurnitureInstanceDestroyedDelegate OnPropertyFurnitureInstanceDestroyed;
	public static void SendLocalEvent_OnPropertyFurnitureInstanceDestroyed(CPropertyFurnitureInstance FurnitureInstance) { NetworkEvents.OnPropertyFurnitureInstanceDestroyed?.Invoke(FurnitureInstance); }
	public delegate void OnShowItemDelegate(CPlayer a_Player, Int64 item_dbid);
	public static OnShowItemDelegate OnShowItem;
	public delegate void OnStoreCheckoutDelegate(CPlayer a_Player, Int64 storeID, string strCartContents);
	public static OnStoreCheckoutDelegate OnStoreCheckout;
	public delegate void OnUseItemDelegate(CPlayer a_Player, Int64 item_dbid);
	public static OnUseItemDelegate OnUseItem;
	public delegate void OpenLanguagesUIDelegate(CPlayer a_Player);
	public static OpenLanguagesUIDelegate OpenLanguagesUI;
	public delegate void OutfitEditor_CreateOrUpdateOutfitDelegate(CPlayer a_Player, string Name, Dictionary<int, Int64> ClothingItemIDs, Dictionary<int, Int64> PropItemIDs, Int64 outfitID, bool bHideHair);
	public static OutfitEditor_CreateOrUpdateOutfitDelegate OutfitEditor_CreateOrUpdateOutfit;
	public delegate void OutfitEditor_DeleteOutfitDelegate(CPlayer a_Player, Int64 outfitID);
	public static OutfitEditor_DeleteOutfitDelegate OutfitEditor_DeleteOutfit;
	public delegate void PersistentNotifications_DeletedDelegate(CPlayer a_Player, Int64 notificationID);
	public static PersistentNotifications_DeletedDelegate PersistentNotifications_Deleted;
	public delegate void PickupDropoffBasedJob_GotoDropoffStateDelegate(CPlayer a_Player);
	public static PickupDropoffBasedJob_GotoDropoffStateDelegate PickupDropoffBasedJob_GotoDropoffState;
	public delegate void PickupDropoffBasedJob_GotoPickupStateDelegate(CPlayer a_Player);
	public static PickupDropoffBasedJob_GotoPickupStateDelegate PickupDropoffBasedJob_GotoPickupState;
	public delegate void PickupDropoffBasedJob_VerifyDropoffDelegate(CPlayer a_Player);
	public static PickupDropoffBasedJob_VerifyDropoffDelegate PickupDropoffBasedJob_VerifyDropoff;
	public delegate void PickupDropoffBasedJob_VerifyPickupDelegate(CPlayer a_Player);
	public static PickupDropoffBasedJob_VerifyPickupDelegate PickupDropoffBasedJob_VerifyPickup;
	public delegate void PickupNewsCameraDelegate(CPlayer a_Player, ObjectType newsCameraObject);
	public static PickupNewsCameraDelegate PickupNewsCamera;
	public delegate void PickupStripsDelegate(CPlayer a_Player, ObjectType spikeStripObject);
	public static PickupStripsDelegate PickupStrips;
	public delegate void PlasticSurgeon_CalculatePriceDelegate(CPlayer a_Player);
	public static PlasticSurgeon_CalculatePriceDelegate PlasticSurgeon_CalculatePrice;
	public delegate void PlasticSurgeon_CheckoutDelegate(CPlayer a_Player, Int64 storeID, int Ageing, float AgeingOpacity, int Makeup, float MakeupOpacity, int MakeupColor, int MakeupColorHighlight, int Blush, float BlushOpacity, int BlushColor, int BlushColorHighlight, int Complexion, float ComplexionOpacity, int SunDamage, float SunDamageOpacity, int Lipstick, float LipstickOpacity, int LipstickColor, int LipstickColorHighlights, int MolesAndFreckles, float MolesAndFrecklesOpacity, float NoseSizeHorizontal, float NoseSizeVertical, float NoseSizeOutwards, float NoseSizeOutwardsUpper, float NoseSizeOutwardsLower, float NoseAngle, float EyebrowHeight, float EyebrowDepth, float CheekboneHeight, float CheekWidth, float CheekWidthLower, float EyeSize, float LipSize, float MouthSize, float MouthSizeLower, float ChinSize, float ChinSizeLower, float ChinWidth, float ChinEffect, float NeckWidth, float NeckWidthLower, int FaceBlend1Mother, int FaceBlend1Father, float FaceBlendFatherPercent, float SkinBlendFatherPercent, int EyeColor, int Blemishes, float BlemishesOpacity, int Eyebrows, float EyebrowsOpacity, int EyebrowsColor, int EyebrowsColorHighlight, int BodyBlemishes, float BodyBlemishesOpacity);
	public static PlasticSurgeon_CheckoutDelegate PlasticSurgeon_Checkout;
	public delegate void PlayerLoadedHighPrioDelegate(CPlayer a_Player);
	public static PlayerLoadedHighPrioDelegate PlayerLoadedHighPrio;
	public delegate void PlayerLoadedLowPrioDelegate(CPlayer a_Player);
	public static PlayerLoadedLowPrioDelegate PlayerLoadedLowPrio;
	public delegate void PlayerRawCommandDelegate(CPlayer a_Player, string msg);
	public static PlayerRawCommandDelegate PlayerRawCommand;
	public delegate void PreviewCharacterDelegate(CPlayer a_Player, long characterID);
	public static PreviewCharacterDelegate PreviewCharacter;
	public delegate void Property_MowedLawnDelegate(CPlayer a_Player, Int64 propertyId);
	public static Property_MowedLawnDelegate Property_MowedLawn;
	public delegate void PurchaseDonationPerkDelegate(CPlayer a_Player, UInt32 id);
	public static PurchaseDonationPerkDelegate PurchaseDonationPerk;
	public delegate void PurchaseInactivityProtectionDelegate(CPlayer a_Player, Int64 TargetEntityID, EDonationInactivityEntityType TargetEntityType, int InactivityLength);
	public static PurchaseInactivityProtectionDelegate PurchaseInactivityProtection;
	public delegate void PurchaseProperty_OnCheckoutDelegate(CPlayer a_Player, Int64 PropertyID, EPurchaserType purchaserType, long purchaserID, EPaymentMethod method, float fDownpayment, int numMonthsForPaymentPlan);
	public static PurchaseProperty_OnCheckoutDelegate PurchaseProperty_OnCheckout;
	public delegate void PurchaseProperty_OnPreviewDelegate(CPlayer a_Player, Int64 PropertyID);
	public static PurchaseProperty_OnPreviewDelegate PurchaseProperty_OnPreview;
	public delegate void PurchaseRadioRequestDelegate(CPlayer a_Player);
	public static PurchaseRadioRequestDelegate PurchaseRadioRequest;
	public delegate void PurchaseVehicle_OnCheckoutDelegate(CPlayer a_Player, int VehicleIndex, uint primary_r, uint primary_g, uint primary_b, uint secondary_r, uint secondary_g, uint secondary_b, EPurchaserType purchaserType, long purchaserID, EPaymentMethod method, float fDownpayment, int numMonthsForPaymentPlan, EScriptLocation location, EVehicleStoreType storeType);
	public static PurchaseVehicle_OnCheckoutDelegate PurchaseVehicle_OnCheckout;
	public delegate void QuizCompleteDelegate(CPlayer a_Player, List<int> lstResponseIndexes);
	public static QuizCompleteDelegate QuizComplete;
	public delegate void RadialSetDoorStateDelegate(CPlayer a_Player, int vehicleID, int doorID);
	public static RadialSetDoorStateDelegate RadialSetDoorState;
	public delegate void RadialSetLockStateDelegate(CPlayer a_Player, int vehicleID);
	public static RadialSetLockStateDelegate RadialSetLockState;
	public delegate void ReadInfoMarkerDelegate(CPlayer a_Player, Int64 a_InfoMarkerID);
	public static ReadInfoMarkerDelegate ReadInfoMarker;
	public delegate void RegisterPlayerDelegate(CPlayer a_Player, string strUsername, string strPassword, string strPasswordVerify, string strEmail);
	public static RegisterPlayerDelegate RegisterPlayer;
	public delegate void ReloadCheckIntDataDelegate(CPlayer a_Player, long propertyID);
	public static ReloadCheckIntDataDelegate ReloadCheckIntData;
	public delegate void ReloadCheckVehDataDelegate(CPlayer a_Player, long vehicleID);
	public static ReloadCheckVehDataDelegate ReloadCheckVehData;
	public delegate void RemovePhoneContactDelegate(CPlayer a_Player, string entryNumber, string entryName);
	public static RemovePhoneContactDelegate RemovePhoneContact;
	public delegate void RentalShop_RentScooterDelegate(CPlayer a_Player, Int64 storeID);
	public static RentalShop_RentScooterDelegate RentalShop_RentScooter;
	public delegate void RentVehicle_OnCheckoutDelegate(CPlayer a_Player, int VehicleIndex, uint primary_r, uint primary_g, uint primary_b, uint secondary_r, uint secondary_g, uint secondary_b, EPurchaserType purchaserType, long purchaserID, uint rentalLengthDays, EScriptLocation location, EVehicleStoreType storeType);
	public static RentVehicle_OnCheckoutDelegate RentVehicle_OnCheckout;
	public delegate void Reports_ReloadReportDataDelegate(CPlayer a_Player);
	public static Reports_ReloadReportDataDelegate Reports_ReloadReportData;
	public delegate void RequestAchievementsDelegate(CPlayer a_Player);
	public static RequestAchievementsDelegate RequestAchievements;
	public delegate void RequestApplicationDetailsDelegate(CPlayer a_Player, int accountID);
	public static RequestApplicationDetailsDelegate RequestApplicationDetails;
	public delegate void RequestBeginChangeBoomboxRadioDelegate(CPlayer a_Player, ObjectType worldItemObject);
	public static RequestBeginChangeBoomboxRadioDelegate RequestBeginChangeBoomboxRadio;
	public delegate void RequestCarWashingDelegate(CPlayer a_Player, Int64 a_carWashID);
	public static RequestCarWashingDelegate RequestCarWashing;
	public delegate void RequestChangeCharacterDelegate(CPlayer a_Player);
	public static RequestChangeCharacterDelegate RequestChangeCharacter;
	public delegate void RequestCrouchDelegate(CPlayer a_Player);
	public static RequestCrouchDelegate RequestCrouch;
	public delegate void RequestDimensionChangeDelegate(CPlayer a_Player, uint dimension);
	public static RequestDimensionChangeDelegate RequestDimensionChange;
	public delegate void RequestDutyOutfitListDelegate(CPlayer a_Player, EDutyType a_DutyType);
	public static RequestDutyOutfitListDelegate RequestDutyOutfitList;
	public delegate void RequestEditInteriorDelegate(CPlayer a_Player);
	public static RequestEditInteriorDelegate RequestEditInterior;
	public delegate void RequestEditTagModeDelegate(CPlayer a_Player);
	public static RequestEditTagModeDelegate RequestEditTagMode;
	public delegate void RequestEnterElevatorDelegate(CPlayer a_Player, Int64 ElevatorID);
	public static RequestEnterElevatorDelegate RequestEnterElevator;
	public delegate void RequestEnterInteriorDelegate(CPlayer a_Player, Int64 PropertyID);
	public static RequestEnterInteriorDelegate RequestEnterInterior;
	public delegate void RequestExitElevatorDelegate(CPlayer a_Player, Int64 ElevatorID);
	public static RequestExitElevatorDelegate RequestExitElevator;
	public delegate void RequestExitInteriorDelegate(CPlayer a_Player);
	public static RequestExitInteriorDelegate RequestExitInterior;
	public delegate void RequestExitInteriorForcedDelegate(CPlayer a_Player);
	public static RequestExitInteriorForcedDelegate RequestExitInteriorForced;
	public delegate void RequestFuelingDelegate(CPlayer a_Player, Int64 a_fuelPointID);
	public static RequestFuelingDelegate RequestFueling;
	public delegate void RequestFurnitureInventoryDelegate(CPlayer a_Player, Int64 furnitureDBID);
	public static RequestFurnitureInventoryDelegate RequestFurnitureInventory;
	public delegate void RequestGotoTagModeDelegate(CPlayer a_Player);
	public static RequestGotoTagModeDelegate RequestGotoTagMode;
	public delegate void RequestLogoutDelegate(CPlayer a_Player);
	public static RequestLogoutDelegate RequestLogout;
	public delegate void RequestMailboxDelegate(CPlayer a_Player, Int64 PropertyID);
	public static RequestMailboxDelegate RequestMailbox;
	public delegate void RequestMapDelegate(CPlayer a_Player, int mapID);
	public static RequestMapDelegate RequestMap;
	public delegate void RequestOutfitListDelegate(CPlayer a_Player);
	public static RequestOutfitListDelegate RequestOutfitList;
	public delegate void RequestPendingApplicationsDelegate(CPlayer a_Player);
	public static RequestPendingApplicationsDelegate RequestPendingApplications;
	public delegate void RequestPlateRunDelegate(CPlayer a_Player, Vehicle vehicle);
	public static RequestPlateRunDelegate RequestPlateRun;
	public delegate void RequestPlayerInventoryDelegate(CPlayer a_Player);
	public static RequestPlayerInventoryDelegate RequestPlayerInventory;
	public delegate void RequestPlayerNonSpecificDimensionDelegate(CPlayer a_Player);
	public static RequestPlayerNonSpecificDimensionDelegate RequestPlayerNonSpecificDimension;
	public delegate void RequestPlayerSpecificDimensionDelegate(CPlayer a_Player);
	public static RequestPlayerSpecificDimensionDelegate RequestPlayerSpecificDimension;
	public delegate void RequestQuizQuestionsDelegate(CPlayer a_Player);
	public static RequestQuizQuestionsDelegate RequestQuizQuestions;
	public delegate void RequestStartActivityDelegate(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType);
	public static RequestStartActivityDelegate RequestStartActivity;
	public delegate void RequestStartTaggingDelegate(CPlayer a_Player, float x, float y, float z, float fRotZ);
	public static RequestStartTaggingDelegate RequestStartTagging;
	public delegate void RequestStopActivityDelegate(CPlayer a_Player, Int64 furnitureDBID, EActivityType activityType);
	public static RequestStopActivityDelegate RequestStopActivity;
	public delegate void RequestStopAnimationDelegate(CPlayer a_Player);
	public static RequestStopAnimationDelegate RequestStopAnimation;
	public delegate void RequestStopFishingDelegate(CPlayer a_Player);
	public static RequestStopFishingDelegate RequestStopFishing;
	public delegate void RequestSubscribeActivityDelegate(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType);
	public static RequestSubscribeActivityDelegate RequestSubscribeActivity;
	public delegate void RequestTagCleaningDelegate(CPlayer a_Player, Int64 tagID);
	public static RequestTagCleaningDelegate RequestTagCleaning;
	public delegate void RequestTransferAssetsDelegate(CPlayer a_Player, long fromCharacterId, long toCharacterId, float money, float bankmoney, List<long> vehicles, List<long> properties);
	public static RequestTransferAssetsDelegate RequestTransferAssets;
	public delegate void RequestTutorialStateDelegate(CPlayer a_Player, ETutorialVersions currentTutorialVersion);
	public static RequestTutorialStateDelegate RequestTutorialState;
	public delegate void RequestUnimpoundVehicleDelegate(CPlayer a_Player, VehicleType a_Vehicle, EScriptLocation a_Location);
	public static RequestUnimpoundVehicleDelegate RequestUnimpoundVehicle;
	public delegate void RequestUnsubscribeActivityDelegate(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType);
	public static RequestUnsubscribeActivityDelegate RequestUnsubscribeActivity;
	public delegate void RequestVehicleInventoryDelegate(CPlayer a_Player, VehicleType gtaVehicle, EVehicleInventoryType vehicleInventoryType, bool bIsInvertedTrunk);
	public static RequestVehicleInventoryDelegate RequestVehicleInventory;
	public delegate void RequestVehicleRepairDelegate(CPlayer a_Player, Int64 a_repairPointID);
	public static RequestVehicleRepairDelegate RequestVehicleRepair;
	public delegate void RequestWrittenQuestionsDelegate(CPlayer a_Player);
	public static RequestWrittenQuestionsDelegate RequestWrittenQuestions;
	public delegate void ResetChatSettingsDelegate(CPlayer a_Player);
	public static ResetChatSettingsDelegate ResetChatSettings;
	public delegate void ResetFareDelegate(CPlayer a_Player);
	public static ResetFareDelegate ResetFare;
	public delegate void RetuneRadioDelegate(CPlayer a_Player, Int64 radioID, int channel);
	public static RetuneRadioDelegate RetuneRadio;
	public delegate void Roadblock_PlaceNewDelegate(CPlayer a_Player, int descriptorIndex, float x, float y, float z);
	public static Roadblock_PlaceNewDelegate Roadblock_PlaceNew;
	public delegate void Roadblock_RemoveExistingDelegate(CPlayer a_Player, int entryID);
	public static Roadblock_RemoveExistingDelegate Roadblock_RemoveExisting;
	public delegate void Roadblock_UpdateExistingDelegate(CPlayer a_Player, int entryID, float x, float y, float z, float rx, float ry, float rz);
	public static Roadblock_UpdateExistingDelegate Roadblock_UpdateExisting;
	public delegate void SaveAdminInteriorNoteDelegate(CPlayer a_Player, string strNote, long interiorID);
	public static SaveAdminInteriorNoteDelegate SaveAdminInteriorNote;
	public delegate void SaveAdminNotesDelegate(CPlayer a_Player, string strNotes, int accountID);
	public static SaveAdminNotesDelegate SaveAdminNotes;
	public delegate void SaveAdminVehicleNoteDelegate(CPlayer a_Player, string strNote, long vehicleID);
	public static SaveAdminVehicleNoteDelegate SaveAdminVehicleNote;
	public delegate void SaveChatSettingsDelegate(CPlayer a_Player, ChatSettings chatSettings);
	public static SaveChatSettingsDelegate SaveChatSettings;
	public delegate void SaveControlsDelegate(CPlayer a_Player, List<GameControlObject> lstGameControls);
	public static SaveControlsDelegate SaveControls;
	public delegate void SavePetNameDelegate(CPlayer a_Player, Int64 petID, string strName);
	public static SavePetNameDelegate SavePetName;
	public delegate void SavePhoneContactDelegate(CPlayer a_Player, string entryName, string entryNumber);
	public static SavePhoneContactDelegate SavePhoneContact;
	public delegate void SaveRadioDelegate(CPlayer a_Player, int radioID, string strName, string strEndpoint);
	public static SaveRadioDelegate SaveRadio;
	public delegate void SendSMSNotificationDelegate(CPlayer a_Player, string a_strNumber);
	public static SendSMSNotificationDelegate SendSMSNotification;
	public delegate void SetAllControlsToDefaultDelegate(CPlayer a_Player);
	public static SetAllControlsToDefaultDelegate SetAllControlsToDefault;
	public delegate void SetAutoSpawnCharacterDelegate(CPlayer a_Player, Int64 characterID);
	public static SetAutoSpawnCharacterDelegate SetAutoSpawnCharacter;
	public delegate void SetItemInContainerDelegate(CPlayer a_Player, Int64 item_dbid, Int64 container_dbid, bool is_going_to_socket_container, VehicleType currentVehicle, Int64 currentFurnitureDBID);
	public static SetItemInContainerDelegate SetItemInContainer;
	public delegate void SetItemInSocketDelegate(CPlayer a_Player, Int64 item_dbid, EItemSocket socket_id, VehicleType currentVehicle, Int64 currentFurnitureDBID);
	public static SetItemInSocketDelegate SetItemInSocket;
	public delegate void SetSpotlightRotationDelegate(CPlayer a_Player, float fRotation);
	public static SetSpotlightRotationDelegate SetSpotlightRotation;
	public delegate void SetVehicleGearDelegate(CPlayer a_Player, int gear);
	public static SetVehicleGearDelegate SetVehicleGear;
	public delegate void ShareDutyOutfitDelegate(CPlayer a_Player, Int64 outfitDBID, PlayerType rageTargetPlayer);
	public static ShareDutyOutfitDelegate ShareDutyOutfit;
	public delegate void ShareDutyOutfit_OutcomeDelegate(CPlayer a_Player, bool bAccepted);
	public static ShareDutyOutfit_OutcomeDelegate ShareDutyOutfit_Outcome;
	public delegate void SpawnSelectedDelegate(CPlayer a_Player, EScriptLocation location, long characterID);
	public static SpawnSelectedDelegate SpawnSelected;
	public delegate void SpeedCameraTriggerDelegate(CPlayer a_Player, float speed, int speedLimit, string name, int cameraID);
	public static SpeedCameraTriggerDelegate SpeedCameraTrigger;
	public delegate void SplitItemDelegate(CPlayer a_Player, Int64 current_item_dbid, uint current_item_new_numstacks, uint new_item_num_stacks);
	public static SplitItemDelegate SplitItem;
	public delegate void StartFishing_ApprovedDelegate(CPlayer a_Player);
	public static StartFishing_ApprovedDelegate StartFishing_Approved;
	public delegate void StoreAmmoDelegate(CPlayer a_Player, Dictionary<EWeapons, int> weaponsDiff);
	public static StoreAmmoDelegate StoreAmmo;
	public delegate void StoreWeaponsDelegate(CPlayer a_Player, List<WeaponHash> lstWeapons);
	public static StoreWeaponsDelegate StoreWeapons;
	public delegate void Store_CancelRobberyDelegate(CPlayer a_Player, Int64 storeID);
	public static Store_CancelRobberyDelegate Store_CancelRobbery;
	public delegate void Store_InitiateRobberyDelegate(CPlayer a_Player, Int64 storeID);
	public static Store_InitiateRobberyDelegate Store_InitiateRobbery;
	public delegate void SubmitAdminReportDelegate(CPlayer a_Player, EAdminReportType reportType, string strDetails, PlayerType rageTargetPlayer);
	public static SubmitAdminReportDelegate SubmitAdminReport;
	public delegate void SubmitWrittenPortionDelegate(CPlayer a_Player, string strQ1Answer, string strQ2Answer, string strQ3Answer, string strQ4Answer);
	public static SubmitWrittenPortionDelegate SubmitWrittenPortion;
	public delegate void SyncManualVehBrakesDelegate(CPlayer a_Player, bool bBrakeLights);
	public static SyncManualVehBrakesDelegate SyncManualVehBrakes;
	public delegate void SyncManualVehRpmDelegate(CPlayer a_Player, float fVehicleRPM);
	public static SyncManualVehRpmDelegate SyncManualVehRpm;
	public delegate void SyncVehicleHandbrakeDelegate(CPlayer a_Player);
	public static SyncVehicleHandbrakeDelegate SyncVehicleHandbrake;
	public delegate void TalkToSantaDelegate(CPlayer a_Player);
	public static TalkToSantaDelegate TalkToSanta;
	public delegate void TattooArtist_CalculatePriceDelegate(CPlayer a_Player, List<int> lstTattoos);
	public static TattooArtist_CalculatePriceDelegate TattooArtist_CalculatePrice;
	public delegate void TattooArtist_CheckoutDelegate(CPlayer a_Player, Int64 storeID, List<int> lstTattoos);
	public static TattooArtist_CheckoutDelegate TattooArtist_Checkout;
	public delegate void TaughtVubDelegate(CPlayer a_Player);
	public static TaughtVubDelegate TaughtVub;
	public delegate void TaxiDriverJob_AtPickupDelegate(CPlayer a_Player);
	public static TaxiDriverJob_AtPickupDelegate TaxiDriverJob_AtPickup;
	public delegate void TicketResponseDelegate(CPlayer a_Player, bool bAccepted);
	public static TicketResponseDelegate TicketResponse;
	public delegate void ToggleAvailableForHireDelegate(CPlayer a_Player);
	public static ToggleAvailableForHireDelegate ToggleAvailableForHire;
	public delegate void ToggleEngineDelegate(CPlayer a_Player);
	public static ToggleEngineDelegate ToggleEngine;
	public delegate void ToggleEngineStallDelegate(CPlayer a_Player);
	public static ToggleEngineStallDelegate ToggleEngineStall;
	public delegate void ToggleHeadlightsDelegate(CPlayer a_Player);
	public static ToggleHeadlightsDelegate ToggleHeadlights;
	public delegate void ToggleLeftTurnSignalDelegate(CPlayer a_Player);
	public static ToggleLeftTurnSignalDelegate ToggleLeftTurnSignal;
	public delegate void ToggleLocalPlayerNametagDelegate(CPlayer a_Player);
	public static ToggleLocalPlayerNametagDelegate ToggleLocalPlayerNametag;
	public delegate void ToggleNametagsDelegate(CPlayer a_Player, bool isHidden);
	public static ToggleNametagsDelegate ToggleNametags;
	public delegate void ToggleRightTurnSignalDelegate(CPlayer a_Player);
	public static ToggleRightTurnSignalDelegate ToggleRightTurnSignal;
	public delegate void ToggleSirenModeDelegate(CPlayer a_Player);
	public static ToggleSirenModeDelegate ToggleSirenMode;
	public delegate void ToggleSpotlightDelegate(CPlayer a_Player);
	public static ToggleSpotlightDelegate ToggleSpotlight;
	public delegate void ToggleVehicleLockedDelegate(CPlayer a_Player);
	public static ToggleVehicleLockedDelegate ToggleVehicleLocked;
	public delegate void ToggleWindowsDelegate(CPlayer a_Player);
	public static ToggleWindowsDelegate ToggleWindows;
	public delegate void TowedVehicleList_RequestDelegate(CPlayer a_Player);
	public static TowedVehicleList_RequestDelegate TowedVehicleList_Request;
	public delegate void TrainDoorStateChangedDelegate(CPlayer a_Player, int ID, bool bDoorsOpen);
	public static TrainDoorStateChangedDelegate TrainDoorStateChanged;
	public delegate void TrainEnterDelegate(CPlayer a_Player, int ID, bool bAsDriver);
	public static TrainEnterDelegate TrainEnter;
	public delegate void TrainExitDelegate(CPlayer a_Player, int ID);
	public static TrainExitDelegate TrainExit;
	public delegate void TrainSyncDelegate(CPlayer a_Player, int ID, float x, float y, float z, float speed, int tripwireID, int currentSector);
	public static TrainSyncDelegate TrainSync;
	public delegate void TriggerKeybindDelegate(CPlayer a_Player, int index);
	public static TriggerKeybindDelegate TriggerKeybind;
	public delegate void UgandaStartDelegate(CPlayer a_Player);
	public static UgandaStartDelegate UgandaStart;
	public delegate void UgandaStopDelegate(CPlayer a_Player);
	public static UgandaStopDelegate UgandaStop;
	public delegate void UnlockAchievementDelegate(CPlayer a_Player, EAchievementID achievementID);
	public static UnlockAchievementDelegate UnlockAchievement;
	public delegate void UpdateActiveLanguageDelegate(CPlayer a_Player, ECharacterLanguage ActiveCharacterLanguage);
	public static UpdateActiveLanguageDelegate UpdateActiveLanguage;
	public delegate void UpdateCharacterLookDelegate(CPlayer a_Player, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup);
	public static UpdateCharacterLookDelegate UpdateCharacterLook;
	public delegate void UpdateMessageViewedDelegate(CPlayer a_Player, string toNumber);
	public static UpdateMessageViewedDelegate UpdateMessageViewed;
	public delegate void UpdateStolenStateDelegate(CPlayer a_Player, long vehicleID, bool stolen);
	public static UpdateStolenStateDelegate UpdateStolenState;
	public delegate void UpdateTagCleaningDelegate(CPlayer a_Player, Int64 tagID);
	public static UpdateTagCleaningDelegate UpdateTagCleaning;
	public delegate void UpdateTaggingDelegate(CPlayer a_Player, Int64 tagID);
	public static UpdateTaggingDelegate UpdateTagging;
	public delegate void UseDutyPointDelegate(CPlayer a_Player, EDutyType a_DutyType);
	public static UseDutyPointDelegate UseDutyPoint;
	public delegate void UseFirearmsLicensingDeviceDelegate(CPlayer a_Player, bool isRemoval);
	public static UseFirearmsLicensingDeviceDelegate UseFirearmsLicensingDevice;
	public delegate void VehicleCrusher_CrushVehicleDelegate(CPlayer a_Player);
	public static VehicleCrusher_CrushVehicleDelegate VehicleCrusher_CrushVehicle;
	public delegate void VehicleCrusher_RequestCrushInformationDelegate(CPlayer a_Player);
	public static VehicleCrusher_RequestCrushInformationDelegate VehicleCrusher_RequestCrushInformation;
	public delegate void VehicleModShop_GetModPriceDelegate(CPlayer a_Player, EModSlot modSlot, int modIndex, string strPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled);
	public static VehicleModShop_GetModPriceDelegate VehicleModShop_GetModPrice;
	public delegate void VehicleModShop_GetPriceDelegate(CPlayer a_Player, Dictionary<EModSlot, int> dictPurchasesMods, string strPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled);
	public static VehicleModShop_GetPriceDelegate VehicleModShop_GetPrice;
	public delegate void VehicleModShop_OnCheckoutDelegate(CPlayer a_Player, Dictionary<EModSlot, int> dictPurchasesMods, string strPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled);
	public static VehicleModShop_OnCheckoutDelegate VehicleModShop_OnCheckout;
	public delegate void VehicleModShop_OnExit_DiscardDelegate(CPlayer a_Player);
	public static VehicleModShop_OnExit_DiscardDelegate VehicleModShop_OnExit_Discard;
}
public static class NetworkEventSender
{
	public static void SendNetworkEvent_ActivityRequestInteract_Response(CPlayer a_Player, bool bManager) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ActivityRequestInteract_Response, bManager); }
	public static void SendNetworkEvent_ActivityRequestInteract_Response_ForAll_SpawnedOnly(bool bManager){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ActivityRequestInteract_Response, bManager);} }
	public static void SendNetworkEvent_ActivityRequestInteract_Response_ForAll_IncludeEveryone(bool bManager){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ActivityRequestInteract_Response, bManager);} }
	public static void SendNetworkEvent_Activity_RoundOutcome(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType, string strDealerOutcome, List<string> lstPlayerOutcomes) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Activity_RoundOutcome, uniqueActivityIdentifier, activityType, strDealerOutcome, lstPlayerOutcomes); }
	public static void SendNetworkEvent_Activity_RoundOutcome_ForAll_SpawnedOnly(Int64 uniqueActivityIdentifier, EActivityType activityType, string strDealerOutcome, List<string> lstPlayerOutcomes){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Activity_RoundOutcome, uniqueActivityIdentifier, activityType, strDealerOutcome, lstPlayerOutcomes);} }
	public static void SendNetworkEvent_Activity_RoundOutcome_ForAll_IncludeEveryone(Int64 uniqueActivityIdentifier, EActivityType activityType, string strDealerOutcome, List<string> lstPlayerOutcomes){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Activity_RoundOutcome, uniqueActivityIdentifier, activityType, strDealerOutcome, lstPlayerOutcomes);} }
	public static void SendNetworkEvent_AdminCheck(CPlayer a_Player, PlayerType player, AdminCheckDetails playerDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheck, player, playerDetails); }
	public static void SendNetworkEvent_AdminCheck_ForAll_SpawnedOnly(PlayerType player, AdminCheckDetails playerDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheck, player, playerDetails);} }
	public static void SendNetworkEvent_AdminCheck_ForAll_IncludeEveryone(PlayerType player, AdminCheckDetails playerDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheck, player, playerDetails);} }
	public static void SendNetworkEvent_AdminCheckInt(CPlayer a_Player, long interior, AdminCheckInteriorDetails interiorDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheckInt, interior, interiorDetails); }
	public static void SendNetworkEvent_AdminCheckInt_ForAll_SpawnedOnly(long interior, AdminCheckInteriorDetails interiorDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheckInt, interior, interiorDetails);} }
	public static void SendNetworkEvent_AdminCheckInt_ForAll_IncludeEveryone(long interior, AdminCheckInteriorDetails interiorDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheckInt, interior, interiorDetails);} }
	public static void SendNetworkEvent_AdminCheckVeh(CPlayer a_Player, long vehicleID, AdminCheckVehicleDetails vehicleDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheckVeh, vehicleID, vehicleDetails); }
	public static void SendNetworkEvent_AdminCheckVeh_ForAll_SpawnedOnly(long vehicleID, AdminCheckVehicleDetails vehicleDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheckVeh, vehicleID, vehicleDetails);} }
	public static void SendNetworkEvent_AdminCheckVeh_ForAll_IncludeEveryone(long vehicleID, AdminCheckVehicleDetails vehicleDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminCheckVeh, vehicleID, vehicleDetails);} }
	public static void SendNetworkEvent_AdminConfirmEntityDelete(CPlayer a_Player, Int64 entityID, EEntityType entityType) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminConfirmEntityDelete, entityID, entityType); }
	public static void SendNetworkEvent_AdminConfirmEntityDelete_ForAll_SpawnedOnly(Int64 entityID, EEntityType entityType){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminConfirmEntityDelete, entityID, entityType);} }
	public static void SendNetworkEvent_AdminConfirmEntityDelete_ForAll_IncludeEveryone(Int64 entityID, EEntityType entityType){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminConfirmEntityDelete, entityID, entityType);} }
	public static void SendNetworkEvent_AdminNativeInteriorID(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminNativeInteriorID); }
	public static void SendNetworkEvent_AdminNativeInteriorID_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminNativeInteriorID);} }
	public static void SendNetworkEvent_AdminNativeInteriorID_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminNativeInteriorID);} }
	public static void SendNetworkEvent_AdminPerfHudState(CPlayer a_Player, bool bEnabled) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminPerfHudState, bEnabled); }
	public static void SendNetworkEvent_AdminPerfHudState_ForAll_SpawnedOnly(bool bEnabled){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminPerfHudState, bEnabled);} }
	public static void SendNetworkEvent_AdminPerfHudState_ForAll_IncludeEveryone(bool bEnabled){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminPerfHudState, bEnabled);} }
	public static void SendNetworkEvent_AdminReloadCheckIntDetails(CPlayer a_Player, AdminCheckInteriorDetails interiorDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReloadCheckIntDetails, interiorDetails); }
	public static void SendNetworkEvent_AdminReloadCheckIntDetails_ForAll_SpawnedOnly(AdminCheckInteriorDetails interiorDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReloadCheckIntDetails, interiorDetails);} }
	public static void SendNetworkEvent_AdminReloadCheckIntDetails_ForAll_IncludeEveryone(AdminCheckInteriorDetails interiorDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReloadCheckIntDetails, interiorDetails);} }
	public static void SendNetworkEvent_AdminReloadCheckVehDetails(CPlayer a_Player, AdminCheckVehicleDetails vehicleDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReloadCheckVehDetails, vehicleDetails); }
	public static void SendNetworkEvent_AdminReloadCheckVehDetails_ForAll_SpawnedOnly(AdminCheckVehicleDetails vehicleDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReloadCheckVehDetails, vehicleDetails);} }
	public static void SendNetworkEvent_AdminReloadCheckVehDetails_ForAll_IncludeEveryone(AdminCheckVehicleDetails vehicleDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReloadCheckVehDetails, vehicleDetails);} }
	public static void SendNetworkEvent_AdminReportEnded(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReportEnded); }
	public static void SendNetworkEvent_AdminReportEnded_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReportEnded);} }
	public static void SendNetworkEvent_AdminReportEnded_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminReportEnded);} }
	public static void SendNetworkEvent_AdminTowGetVehicles(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminTowGetVehicles); }
	public static void SendNetworkEvent_AdminTowGetVehicles_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminTowGetVehicles);} }
	public static void SendNetworkEvent_AdminTowGetVehicles_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AdminTowGetVehicles);} }
	public static void SendNetworkEvent_Admin_GotPendingApps(CPlayer a_Player, List<PendingApplication> lstPendingApps) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Admin_GotPendingApps, lstPendingApps); }
	public static void SendNetworkEvent_Admin_GotPendingApps_ForAll_SpawnedOnly(List<PendingApplication> lstPendingApps){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Admin_GotPendingApps, lstPendingApps);} }
	public static void SendNetworkEvent_Admin_GotPendingApps_ForAll_IncludeEveryone(List<PendingApplication> lstPendingApps){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Admin_GotPendingApps, lstPendingApps);} }
	public static void SendNetworkEvent_ANPR_GotSpeed(CPlayer a_Player, float fSpeed) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ANPR_GotSpeed, fSpeed); }
	public static void SendNetworkEvent_ANPR_GotSpeed_ForAll_SpawnedOnly(float fSpeed){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ANPR_GotSpeed, fSpeed);} }
	public static void SendNetworkEvent_ANPR_GotSpeed_ForAll_IncludeEveryone(float fSpeed){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ANPR_GotSpeed, fSpeed);} }
	public static void SendNetworkEvent_ApplicationState(CPlayer a_Player, EApplicationState applicationState) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplicationState, applicationState); }
	public static void SendNetworkEvent_ApplicationState_ForAll_SpawnedOnly(EApplicationState applicationState){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplicationState, applicationState);} }
	public static void SendNetworkEvent_ApplicationState_ForAll_IncludeEveryone(EApplicationState applicationState){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplicationState, applicationState);} }
	public static void SendNetworkEvent_ApplyCustomControls(CPlayer a_Player, List<GameControlObject> lstControls) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyCustomControls, lstControls); }
	public static void SendNetworkEvent_ApplyCustomControls_ForAll_SpawnedOnly(List<GameControlObject> lstControls){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyCustomControls, lstControls);} }
	public static void SendNetworkEvent_ApplyCustomControls_ForAll_IncludeEveryone(List<GameControlObject> lstControls){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyCustomControls, lstControls);} }
	public static void SendNetworkEvent_ApplyCustomSkinData(CPlayer a_Player, PlayerType player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyCustomSkinData, player); }
	public static void SendNetworkEvent_ApplyCustomSkinData_ForAll_SpawnedOnly(PlayerType player){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyCustomSkinData, player);} }
	public static void SendNetworkEvent_ApplyCustomSkinData_ForAll_IncludeEveryone(PlayerType player){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyCustomSkinData, player);} }
	public static void SendNetworkEvent_ApplyPlayerKeybinds(CPlayer a_Player, List<PlayerKeybindObject> lstBinds) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyPlayerKeybinds, lstBinds); }
	public static void SendNetworkEvent_ApplyPlayerKeybinds_ForAll_SpawnedOnly(List<PlayerKeybindObject> lstBinds){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyPlayerKeybinds, lstBinds);} }
	public static void SendNetworkEvent_ApplyPlayerKeybinds_ForAll_IncludeEveryone(List<PlayerKeybindObject> lstBinds){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyPlayerKeybinds, lstBinds);} }
	public static void SendNetworkEvent_ApplyRemoteChatSettings(CPlayer a_Player, ChatSettings chatSettings) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyRemoteChatSettings, chatSettings); }
	public static void SendNetworkEvent_ApplyRemoteChatSettings_ForAll_SpawnedOnly(ChatSettings chatSettings){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyRemoteChatSettings, chatSettings);} }
	public static void SendNetworkEvent_ApplyRemoteChatSettings_ForAll_IncludeEveryone(ChatSettings chatSettings){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ApplyRemoteChatSettings, chatSettings);} }
	public static void SendNetworkEvent_AssetTransferCompleted(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AssetTransferCompleted); }
	public static void SendNetworkEvent_AssetTransferCompleted_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AssetTransferCompleted);} }
	public static void SendNetworkEvent_AssetTransferCompleted_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AssetTransferCompleted);} }
	public static void SendNetworkEvent_AwardAchievement(CPlayer a_Player, int achievementID, string strTitle, string strCaption, int points, EAchievementRarity rarity, int percent) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AwardAchievement, achievementID, strTitle, strCaption, points, rarity, percent); }
	public static void SendNetworkEvent_AwardAchievement_ForAll_SpawnedOnly(int achievementID, string strTitle, string strCaption, int points, EAchievementRarity rarity, int percent){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AwardAchievement, achievementID, strTitle, strCaption, points, rarity, percent);} }
	public static void SendNetworkEvent_AwardAchievement_ForAll_IncludeEveryone(int achievementID, string strTitle, string strCaption, int points, EAchievementRarity rarity, int percent){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.AwardAchievement, achievementID, strTitle, strCaption, points, rarity, percent);} }
	public static void SendNetworkEvent_Banking_GotAccountInfo(CPlayer a_Player, float fMoney, List<CreditDetails> lstCreditDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_GotAccountInfo, fMoney, lstCreditDetails); }
	public static void SendNetworkEvent_Banking_GotAccountInfo_ForAll_SpawnedOnly(float fMoney, List<CreditDetails> lstCreditDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_GotAccountInfo, fMoney, lstCreditDetails);} }
	public static void SendNetworkEvent_Banking_GotAccountInfo_ForAll_IncludeEveryone(float fMoney, List<CreditDetails> lstCreditDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_GotAccountInfo, fMoney, lstCreditDetails);} }
	public static void SendNetworkEvent_Banking_OnServerResponse(CPlayer a_Player, EBankingResponseCode result) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_OnServerResponse, result); }
	public static void SendNetworkEvent_Banking_OnServerResponse_ForAll_SpawnedOnly(EBankingResponseCode result){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_OnServerResponse, result);} }
	public static void SendNetworkEvent_Banking_OnServerResponse_ForAll_IncludeEveryone(EBankingResponseCode result){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_OnServerResponse, result);} }
	public static void SendNetworkEvent_Banking_RefreshCreditInfo(CPlayer a_Player, List<CreditDetails> lstCreditDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_RefreshCreditInfo, lstCreditDetails); }
	public static void SendNetworkEvent_Banking_RefreshCreditInfo_ForAll_SpawnedOnly(List<CreditDetails> lstCreditDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_RefreshCreditInfo, lstCreditDetails);} }
	public static void SendNetworkEvent_Banking_RefreshCreditInfo_ForAll_IncludeEveryone(List<CreditDetails> lstCreditDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_RefreshCreditInfo, lstCreditDetails);} }
	public static void SendNetworkEvent_Banking_RequestInfoResponse(CPlayer a_Player, List<Purchaser> lstPurchasers, List<string> lstMethods) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_RequestInfoResponse, lstPurchasers, lstMethods); }
	public static void SendNetworkEvent_Banking_RequestInfoResponse_ForAll_SpawnedOnly(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_Banking_RequestInfoResponse_ForAll_IncludeEveryone(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Banking_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_BarberShop_GotPrice(CPlayer a_Player, float fPrice, bool hasToken) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BarberShop_GotPrice, fPrice, hasToken); }
	public static void SendNetworkEvent_BarberShop_GotPrice_ForAll_SpawnedOnly(float fPrice, bool hasToken){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BarberShop_GotPrice, fPrice, hasToken);} }
	public static void SendNetworkEvent_BarberShop_GotPrice_ForAll_IncludeEveryone(float fPrice, bool hasToken){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BarberShop_GotPrice, fPrice, hasToken);} }
	public static void SendNetworkEvent_BasicDonatorInfo(CPlayer a_Player, List<DonationPurchasable> lstPurchasables) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BasicDonatorInfo, lstPurchasables); }
	public static void SendNetworkEvent_BasicDonatorInfo_ForAll_SpawnedOnly(List<DonationPurchasable> lstPurchasables){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BasicDonatorInfo, lstPurchasables);} }
	public static void SendNetworkEvent_BasicDonatorInfo_ForAll_IncludeEveryone(List<DonationPurchasable> lstPurchasables){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BasicDonatorInfo, lstPurchasables);} }
	public static void SendNetworkEvent_Blackjack_PlaceBet_GotDetails(CPlayer a_Player, int totalChips) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Blackjack_PlaceBet_GotDetails, totalChips); }
	public static void SendNetworkEvent_Blackjack_PlaceBet_GotDetails_ForAll_SpawnedOnly(int totalChips){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Blackjack_PlaceBet_GotDetails, totalChips);} }
	public static void SendNetworkEvent_Blackjack_PlaceBet_GotDetails_ForAll_IncludeEveryone(int totalChips){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Blackjack_PlaceBet_GotDetails, totalChips);} }
	public static void SendNetworkEvent_BlipSiren_Response(CPlayer a_Player, Vehicle vehicle) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BlipSiren_Response, vehicle); }
	public static void SendNetworkEvent_BlipSiren_Response_ForAll_SpawnedOnly(Vehicle vehicle){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BlipSiren_Response, vehicle);} }
	public static void SendNetworkEvent_BlipSiren_Response_ForAll_IncludeEveryone(Vehicle vehicle){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.BlipSiren_Response, vehicle);} }
	public static void SendNetworkEvent_CallFailed(CPlayer a_Player, ECallFailedReason reason) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallFailed, reason); }
	public static void SendNetworkEvent_CallFailed_ForAll_SpawnedOnly(ECallFailedReason reason){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallFailed, reason);} }
	public static void SendNetworkEvent_CallFailed_ForAll_IncludeEveryone(ECallFailedReason reason){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallFailed, reason);} }
	public static void SendNetworkEvent_CallReceived(CPlayer a_Player, bool bHasExistingTaxiRequest, Int64 number) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallReceived, bHasExistingTaxiRequest, number); }
	public static void SendNetworkEvent_CallReceived_ForAll_SpawnedOnly(bool bHasExistingTaxiRequest, Int64 number){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallReceived, bHasExistingTaxiRequest, number);} }
	public static void SendNetworkEvent_CallReceived_ForAll_IncludeEveryone(bool bHasExistingTaxiRequest, Int64 number){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallReceived, bHasExistingTaxiRequest, number);} }
	public static void SendNetworkEvent_CallState(CPlayer a_Player, Int64 number, bool bIsConnected) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallState, number, bIsConnected); }
	public static void SendNetworkEvent_CallState_ForAll_SpawnedOnly(Int64 number, bool bIsConnected){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallState, number, bIsConnected);} }
	public static void SendNetworkEvent_CallState_ForAll_IncludeEveryone(Int64 number, bool bIsConnected){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CallState, number, bIsConnected);} }
	public static void SendNetworkEvent_CarWashingComplete(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CarWashingComplete); }
	public static void SendNetworkEvent_CarWashingComplete_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CarWashingComplete);} }
	public static void SendNetworkEvent_CarWashingComplete_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CarWashingComplete);} }
	public static void SendNetworkEvent_CarWashingRequestResponse(CPlayer a_Player, bool bSuccess) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CarWashingRequestResponse, bSuccess); }
	public static void SendNetworkEvent_CarWashingRequestResponse_ForAll_SpawnedOnly(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CarWashingRequestResponse, bSuccess);} }
	public static void SendNetworkEvent_CarWashingRequestResponse_ForAll_IncludeEveryone(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CarWashingRequestResponse, bSuccess);} }
	public static void SendNetworkEvent_CasinoManagement_GotDetails(CPlayer a_Player, int chips) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CasinoManagement_GotDetails, chips); }
	public static void SendNetworkEvent_CasinoManagement_GotDetails_ForAll_SpawnedOnly(int chips){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CasinoManagement_GotDetails, chips);} }
	public static void SendNetworkEvent_CasinoManagement_GotDetails_ForAll_IncludeEveryone(int chips){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CasinoManagement_GotDetails, chips);} }
	public static void SendNetworkEvent_ChangeCharacterApproved(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChangeCharacterApproved); }
	public static void SendNetworkEvent_ChangeCharacterApproved_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChangeCharacterApproved);} }
	public static void SendNetworkEvent_ChangeCharacterApproved_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChangeCharacterApproved);} }
	public static void SendNetworkEvent_CharacterSelectionApproved(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CharacterSelectionApproved); }
	public static void SendNetworkEvent_CharacterSelectionApproved_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CharacterSelectionApproved);} }
	public static void SendNetworkEvent_CharacterSelectionApproved_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CharacterSelectionApproved);} }
	public static void SendNetworkEvent_CheckpointBasedJob_GotoCheckpointState_Response(CPlayer a_Player, EJobID a_JobID, Vector3 a_vecCheckpointPos) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CheckpointBasedJob_GotoCheckpointState_Response, a_JobID, a_vecCheckpointPos); }
	public static void SendNetworkEvent_CheckpointBasedJob_GotoCheckpointState_Response_ForAll_SpawnedOnly(EJobID a_JobID, Vector3 a_vecCheckpointPos){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CheckpointBasedJob_GotoCheckpointState_Response, a_JobID, a_vecCheckpointPos);} }
	public static void SendNetworkEvent_CheckpointBasedJob_GotoCheckpointState_Response_ForAll_IncludeEveryone(EJobID a_JobID, Vector3 a_vecCheckpointPos){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CheckpointBasedJob_GotoCheckpointState_Response, a_JobID, a_vecCheckpointPos);} }
	public static void SendNetworkEvent_CheckpointBasedJob_VerifyCheckpoint_Response(CPlayer a_Player, EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CheckpointBasedJob_VerifyCheckpoint_Response, a_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel); }
	public static void SendNetworkEvent_CheckpointBasedJob_VerifyCheckpoint_Response_ForAll_SpawnedOnly(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CheckpointBasedJob_VerifyCheckpoint_Response, a_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);} }
	public static void SendNetworkEvent_CheckpointBasedJob_VerifyCheckpoint_Response_ForAll_IncludeEveryone(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CheckpointBasedJob_VerifyCheckpoint_Response, a_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);} }
	public static void SendNetworkEvent_ChipManagement_Buy_GotDetails(CPlayer a_Player, int chips) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChipManagement_Buy_GotDetails, chips); }
	public static void SendNetworkEvent_ChipManagement_Buy_GotDetails_ForAll_SpawnedOnly(int chips){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChipManagement_Buy_GotDetails, chips);} }
	public static void SendNetworkEvent_ChipManagement_Buy_GotDetails_ForAll_IncludeEveryone(int chips){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChipManagement_Buy_GotDetails, chips);} }
	public static void SendNetworkEvent_ChipManagement_Sell_GotDetails(CPlayer a_Player, int chips) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChipManagement_Sell_GotDetails, chips); }
	public static void SendNetworkEvent_ChipManagement_Sell_GotDetails_ForAll_SpawnedOnly(int chips){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChipManagement_Sell_GotDetails, chips);} }
	public static void SendNetworkEvent_ChipManagement_Sell_GotDetails_ForAll_IncludeEveryone(int chips){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ChipManagement_Sell_GotDetails, chips);} }
	public static void SendNetworkEvent_ClearChatbox(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClearChatbox); }
	public static void SendNetworkEvent_ClearChatbox_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClearChatbox);} }
	public static void SendNetworkEvent_ClearChatbox_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClearChatbox);} }
	public static void SendNetworkEvent_ClearNearbyTags(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClearNearbyTags); }
	public static void SendNetworkEvent_ClearNearbyTags_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClearNearbyTags);} }
	public static void SendNetworkEvent_ClearNearbyTags_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClearNearbyTags);} }
	public static void SendNetworkEvent_ClientFriskPlayer(CPlayer a_Player, PlayerType playerBeingFrisked, List<CItemInstanceDef> inventory) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClientFriskPlayer, playerBeingFrisked, inventory); }
	public static void SendNetworkEvent_ClientFriskPlayer_ForAll_SpawnedOnly(PlayerType playerBeingFrisked, List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClientFriskPlayer, playerBeingFrisked, inventory);} }
	public static void SendNetworkEvent_ClientFriskPlayer_ForAll_IncludeEveryone(PlayerType playerBeingFrisked, List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClientFriskPlayer, playerBeingFrisked, inventory);} }
	public static void SendNetworkEvent_ClosePhoneByToggle(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClosePhoneByToggle); }
	public static void SendNetworkEvent_ClosePhoneByToggle_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClosePhoneByToggle);} }
	public static void SendNetworkEvent_ClosePhoneByToggle_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClosePhoneByToggle);} }
	public static void SendNetworkEvent_ClothingStore_GotPrice(CPlayer a_Player, float fPrice, bool hasToken) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClothingStore_GotPrice, fPrice, hasToken); }
	public static void SendNetworkEvent_ClothingStore_GotPrice_ForAll_SpawnedOnly(float fPrice, bool hasToken){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClothingStore_GotPrice, fPrice, hasToken);} }
	public static void SendNetworkEvent_ClothingStore_GotPrice_ForAll_IncludeEveryone(float fPrice, bool hasToken){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ClothingStore_GotPrice, fPrice, hasToken);} }
	public static void SendNetworkEvent_Create911LocationBlip(CPlayer a_Player, Vector3 position) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Create911LocationBlip, position); }
	public static void SendNetworkEvent_Create911LocationBlip_ForAll_SpawnedOnly(Vector3 position){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Create911LocationBlip, position);} }
	public static void SendNetworkEvent_Create911LocationBlip_ForAll_IncludeEveryone(Vector3 position){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Create911LocationBlip, position);} }
	public static void SendNetworkEvent_CreateBankPed(CPlayer a_Player, Vector3 vecPos, float fRotZ, uint dimension) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateBankPed, vecPos, fRotZ, dimension); }
	public static void SendNetworkEvent_CreateBankPed_ForAll_SpawnedOnly(Vector3 vecPos, float fRotZ, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateBankPed, vecPos, fRotZ, dimension);} }
	public static void SendNetworkEvent_CreateBankPed_ForAll_IncludeEveryone(Vector3 vecPos, float fRotZ, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateBankPed, vecPos, fRotZ, dimension);} }
	public static void SendNetworkEvent_CreateCharacterResponse(CPlayer a_Player, ECreateCharacterResponse response) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateCharacterResponse, response); }
	public static void SendNetworkEvent_CreateCharacterResponse_ForAll_SpawnedOnly(ECreateCharacterResponse response){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateCharacterResponse, response);} }
	public static void SendNetworkEvent_CreateCharacterResponse_ForAll_IncludeEveryone(ECreateCharacterResponse response){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateCharacterResponse, response);} }
	public static void SendNetworkEvent_CreateDancerPed(CPlayer a_Player, Vector3 vecPos, float fRot, uint dimension, Int64 dancerID, uint dancerSkin, bool bAllowTip, TransmitAnimation AnimationTransmit) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateDancerPed, vecPos, fRot, dimension, dancerID, dancerSkin, bAllowTip, AnimationTransmit); }
	public static void SendNetworkEvent_CreateDancerPed_ForAll_SpawnedOnly(Vector3 vecPos, float fRot, uint dimension, Int64 dancerID, uint dancerSkin, bool bAllowTip, TransmitAnimation AnimationTransmit){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateDancerPed, vecPos, fRot, dimension, dancerID, dancerSkin, bAllowTip, AnimationTransmit);} }
	public static void SendNetworkEvent_CreateDancerPed_ForAll_IncludeEveryone(Vector3 vecPos, float fRot, uint dimension, Int64 dancerID, uint dancerSkin, bool bAllowTip, TransmitAnimation AnimationTransmit){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateDancerPed, vecPos, fRot, dimension, dancerID, dancerSkin, bAllowTip, AnimationTransmit);} }
	public static void SendNetworkEvent_CreateGangTag(CPlayer a_Player, Int64 a_ID, Int64 a_OwnerCharacterID, Vector3 vecPos, float fRotZ, uint a_Dimension, List<GangTagLayer> lstLayers, float fProgress) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateGangTag, a_ID, a_OwnerCharacterID, vecPos, fRotZ, a_Dimension, lstLayers, fProgress); }
	public static void SendNetworkEvent_CreateGangTag_ForAll_SpawnedOnly(Int64 a_ID, Int64 a_OwnerCharacterID, Vector3 vecPos, float fRotZ, uint a_Dimension, List<GangTagLayer> lstLayers, float fProgress){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateGangTag, a_ID, a_OwnerCharacterID, vecPos, fRotZ, a_Dimension, lstLayers, fProgress);} }
	public static void SendNetworkEvent_CreateGangTag_ForAll_IncludeEveryone(Int64 a_ID, Int64 a_OwnerCharacterID, Vector3 vecPos, float fRotZ, uint a_Dimension, List<GangTagLayer> lstLayers, float fProgress){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateGangTag, a_ID, a_OwnerCharacterID, vecPos, fRotZ, a_Dimension, lstLayers, fProgress);} }
	public static void SendNetworkEvent_CreateInfoMarker_Request(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateInfoMarker_Request); }
	public static void SendNetworkEvent_CreateInfoMarker_Request_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateInfoMarker_Request);} }
	public static void SendNetworkEvent_CreateInfoMarker_Request_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateInfoMarker_Request);} }
	public static void SendNetworkEvent_CreateStorePed(CPlayer a_Player, Vector3 vecPos, float fRot, uint dimension, Int64 storeID, EStoreType storeType) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateStorePed, vecPos, fRot, dimension, storeID, storeType); }
	public static void SendNetworkEvent_CreateStorePed_ForAll_SpawnedOnly(Vector3 vecPos, float fRot, uint dimension, Int64 storeID, EStoreType storeType){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateStorePed, vecPos, fRot, dimension, storeID, storeType);} }
	public static void SendNetworkEvent_CreateStorePed_ForAll_IncludeEveryone(Vector3 vecPos, float fRot, uint dimension, Int64 storeID, EStoreType storeType){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateStorePed, vecPos, fRot, dimension, storeID, storeType);} }
	public static void SendNetworkEvent_CreateSyncedTrain(CPlayer a_Player, int ID, ETrainType TrainType, Vector3 Position, int CurrentSector, int LastTripWireID, float Speed, bool bDoorsOpen) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateSyncedTrain, ID, TrainType, Position, CurrentSector, LastTripWireID, Speed, bDoorsOpen); }
	public static void SendNetworkEvent_CreateSyncedTrain_ForAll_SpawnedOnly(int ID, ETrainType TrainType, Vector3 Position, int CurrentSector, int LastTripWireID, float Speed, bool bDoorsOpen){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateSyncedTrain, ID, TrainType, Position, CurrentSector, LastTripWireID, Speed, bDoorsOpen);} }
	public static void SendNetworkEvent_CreateSyncedTrain_ForAll_IncludeEveryone(int ID, ETrainType TrainType, Vector3 Position, int CurrentSector, int LastTripWireID, float Speed, bool bDoorsOpen){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CreateSyncedTrain, ID, TrainType, Position, CurrentSector, LastTripWireID, Speed, bDoorsOpen);} }
	public static void SendNetworkEvent_CustomAnim_RequestClientLegacyLoad(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomAnim_RequestClientLegacyLoad); }
	public static void SendNetworkEvent_CustomAnim_RequestClientLegacyLoad_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomAnim_RequestClientLegacyLoad);} }
	public static void SendNetworkEvent_CustomAnim_RequestClientLegacyLoad_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomAnim_RequestClientLegacyLoad);} }
	public static void SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomInterior_CustomMapTransfer_Reset); }
	public static void SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomInterior_CustomMapTransfer_Reset);} }
	public static void SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomInterior_CustomMapTransfer_Reset);} }
	public static void SendNetworkEvent_CustomInterior_OpenCustomIntUI(CPlayer a_Player, long propertyID, string propertyName) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomInterior_OpenCustomIntUI, propertyID, propertyName); }
	public static void SendNetworkEvent_CustomInterior_OpenCustomIntUI_ForAll_SpawnedOnly(long propertyID, string propertyName){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomInterior_OpenCustomIntUI, propertyID, propertyName);} }
	public static void SendNetworkEvent_CustomInterior_OpenCustomIntUI_ForAll_IncludeEveryone(long propertyID, string propertyName){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.CustomInterior_OpenCustomIntUI, propertyID, propertyName);} }
	public static void SendNetworkEvent_Destroy911LocationBlip(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Destroy911LocationBlip); }
	public static void SendNetworkEvent_Destroy911LocationBlip_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Destroy911LocationBlip);} }
	public static void SendNetworkEvent_Destroy911LocationBlip_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Destroy911LocationBlip);} }
	public static void SendNetworkEvent_DestroyBankPed(CPlayer a_Player, Vector3 vecPos, float fRotZ, uint dimension) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyBankPed, vecPos, fRotZ, dimension); }
	public static void SendNetworkEvent_DestroyBankPed_ForAll_SpawnedOnly(Vector3 vecPos, float fRotZ, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyBankPed, vecPos, fRotZ, dimension);} }
	public static void SendNetworkEvent_DestroyBankPed_ForAll_IncludeEveryone(Vector3 vecPos, float fRotZ, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyBankPed, vecPos, fRotZ, dimension);} }
	public static void SendNetworkEvent_DestroyDancerPed(CPlayer a_Player, Vector3 vecPos, float fRot, uint dimension) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyDancerPed, vecPos, fRot, dimension); }
	public static void SendNetworkEvent_DestroyDancerPed_ForAll_SpawnedOnly(Vector3 vecPos, float fRot, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyDancerPed, vecPos, fRot, dimension);} }
	public static void SendNetworkEvent_DestroyDancerPed_ForAll_IncludeEveryone(Vector3 vecPos, float fRot, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyDancerPed, vecPos, fRot, dimension);} }
	public static void SendNetworkEvent_DestroyGangTag(CPlayer a_Player, Int64 a_ID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyGangTag, a_ID); }
	public static void SendNetworkEvent_DestroyGangTag_ForAll_SpawnedOnly(Int64 a_ID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyGangTag, a_ID);} }
	public static void SendNetworkEvent_DestroyGangTag_ForAll_IncludeEveryone(Int64 a_ID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyGangTag, a_ID);} }
	public static void SendNetworkEvent_DestroyStorePed(CPlayer a_Player, Vector3 vecPos, float fRot, uint dimension) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyStorePed, vecPos, fRot, dimension); }
	public static void SendNetworkEvent_DestroyStorePed_ForAll_SpawnedOnly(Vector3 vecPos, float fRot, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyStorePed, vecPos, fRot, dimension);} }
	public static void SendNetworkEvent_DestroyStorePed_ForAll_IncludeEveryone(Vector3 vecPos, float fRot, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroyStorePed, vecPos, fRot, dimension);} }
	public static void SendNetworkEvent_DestroySyncedTrain(CPlayer a_Player, int ID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroySyncedTrain, ID); }
	public static void SendNetworkEvent_DestroySyncedTrain_ForAll_SpawnedOnly(int ID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroySyncedTrain, ID);} }
	public static void SendNetworkEvent_DestroySyncedTrain_ForAll_IncludeEveryone(int ID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DestroySyncedTrain, ID);} }
	public static void SendNetworkEvent_Donation_RespondInactivityEntities(CPlayer a_Player, List<DonationEntityListEntry> lstEntities) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Donation_RespondInactivityEntities, lstEntities); }
	public static void SendNetworkEvent_Donation_RespondInactivityEntities_ForAll_SpawnedOnly(List<DonationEntityListEntry> lstEntities){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Donation_RespondInactivityEntities, lstEntities);} }
	public static void SendNetworkEvent_Donation_RespondInactivityEntities_ForAll_IncludeEveryone(List<DonationEntityListEntry> lstEntities){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Donation_RespondInactivityEntities, lstEntities);} }
	public static void SendNetworkEvent_DrivingLicense_VerifyVehicleReturn(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingLicense_VerifyVehicleReturn); }
	public static void SendNetworkEvent_DrivingLicense_VerifyVehicleReturn_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingLicense_VerifyVehicleReturn);} }
	public static void SendNetworkEvent_DrivingLicense_VerifyVehicleReturn_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingLicense_VerifyVehicleReturn);} }
	public static void SendNetworkEvent_DrivingTest_GotoNextCheckpoint(CPlayer a_Player, bool bSuccess, Vector3 vecTarget) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoNextCheckpoint, bSuccess, vecTarget); }
	public static void SendNetworkEvent_DrivingTest_GotoNextCheckpoint_ForAll_SpawnedOnly(bool bSuccess, Vector3 vecTarget){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoNextCheckpoint, bSuccess, vecTarget);} }
	public static void SendNetworkEvent_DrivingTest_GotoNextCheckpoint_ForAll_IncludeEveryone(bool bSuccess, Vector3 vecTarget){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoNextCheckpoint, bSuccess, vecTarget);} }
	public static void SendNetworkEvent_DrivingTest_GotoReturnVehicle(CPlayer a_Player, bool bSuccess, float x, float y, float z) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoReturnVehicle, bSuccess, x, y, z); }
	public static void SendNetworkEvent_DrivingTest_GotoReturnVehicle_ForAll_SpawnedOnly(bool bSuccess, float x, float y, float z){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoReturnVehicle, bSuccess, x, y, z);} }
	public static void SendNetworkEvent_DrivingTest_GotoReturnVehicle_ForAll_IncludeEveryone(bool bSuccess, float x, float y, float z){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoReturnVehicle, bSuccess, x, y, z);} }
	public static void SendNetworkEvent_DrivingTest_GotoVehicleReturned(CPlayer a_Player, bool bSuccess, bool bPassed, bool bFailedSpeeding, bool bFailedDamage) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoVehicleReturned, bSuccess, bPassed, bFailedSpeeding, bFailedDamage); }
	public static void SendNetworkEvent_DrivingTest_GotoVehicleReturned_ForAll_SpawnedOnly(bool bSuccess, bool bPassed, bool bFailedSpeeding, bool bFailedDamage){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoVehicleReturned, bSuccess, bPassed, bFailedSpeeding, bFailedDamage);} }
	public static void SendNetworkEvent_DrivingTest_GotoVehicleReturned_ForAll_IncludeEveryone(bool bSuccess, bool bPassed, bool bFailedSpeeding, bool bFailedDamage){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DrivingTest_GotoVehicleReturned, bSuccess, bPassed, bFailedSpeeding, bFailedDamage);} }
	public static void SendNetworkEvent_DutyOutfitShareInformClient(CPlayer a_Player, string strPlayerName, string strOutfitName) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DutyOutfitShareInformClient, strPlayerName, strOutfitName); }
	public static void SendNetworkEvent_DutyOutfitShareInformClient_ForAll_SpawnedOnly(string strPlayerName, string strOutfitName){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DutyOutfitShareInformClient, strPlayerName, strOutfitName);} }
	public static void SendNetworkEvent_DutyOutfitShareInformClient_ForAll_IncludeEveryone(string strPlayerName, string strOutfitName){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DutyOutfitShareInformClient, strPlayerName, strOutfitName);} }
	public static void SendNetworkEvent_DutySystem_GotUpdatedOutfitList(CPlayer a_Player, List<CItemInstanceDef> lstOutfits) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DutySystem_GotUpdatedOutfitList, lstOutfits); }
	public static void SendNetworkEvent_DutySystem_GotUpdatedOutfitList_ForAll_SpawnedOnly(List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DutySystem_GotUpdatedOutfitList, lstOutfits);} }
	public static void SendNetworkEvent_DutySystem_GotUpdatedOutfitList_ForAll_IncludeEveryone(List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.DutySystem_GotUpdatedOutfitList, lstOutfits);} }
	public static void SendNetworkEvent_EnterBarberShop_Response(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterBarberShop_Response); }
	public static void SendNetworkEvent_EnterBarberShop_Response_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterBarberShop_Response);} }
	public static void SendNetworkEvent_EnterBarberShop_Response_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterBarberShop_Response);} }
	public static void SendNetworkEvent_EnterClothingStore_Response(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterClothingStore_Response); }
	public static void SendNetworkEvent_EnterClothingStore_Response_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterClothingStore_Response);} }
	public static void SendNetworkEvent_EnterClothingStore_Response_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterClothingStore_Response);} }
	public static void SendNetworkEvent_EnterDutyOutfitEditor_Response(CPlayer a_Player, List<CItemInstanceDef> lstOutfits) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterDutyOutfitEditor_Response, lstOutfits); }
	public static void SendNetworkEvent_EnterDutyOutfitEditor_Response_ForAll_SpawnedOnly(List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterDutyOutfitEditor_Response, lstOutfits);} }
	public static void SendNetworkEvent_EnterDutyOutfitEditor_Response_ForAll_IncludeEveryone(List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterDutyOutfitEditor_Response, lstOutfits);} }
	public static void SendNetworkEvent_EnterElevatorApproved(CPlayer a_Player, float x, float y, float z, int mapID, bool bIsCharacterSelect) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterElevatorApproved, x, y, z, mapID, bIsCharacterSelect); }
	public static void SendNetworkEvent_EnterElevatorApproved_ForAll_SpawnedOnly(float x, float y, float z, int mapID, bool bIsCharacterSelect){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterElevatorApproved, x, y, z, mapID, bIsCharacterSelect);} }
	public static void SendNetworkEvent_EnterElevatorApproved_ForAll_IncludeEveryone(float x, float y, float z, int mapID, bool bIsCharacterSelect){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterElevatorApproved, x, y, z, mapID, bIsCharacterSelect);} }
	public static void SendNetworkEvent_EnterInteriorApproved(CPlayer a_Player, float x, float y, float z, int mapID, bool bIsCharacterSelect) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterInteriorApproved, x, y, z, mapID, bIsCharacterSelect); }
	public static void SendNetworkEvent_EnterInteriorApproved_ForAll_SpawnedOnly(float x, float y, float z, int mapID, bool bIsCharacterSelect){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterInteriorApproved, x, y, z, mapID, bIsCharacterSelect);} }
	public static void SendNetworkEvent_EnterInteriorApproved_ForAll_IncludeEveryone(float x, float y, float z, int mapID, bool bIsCharacterSelect){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterInteriorApproved, x, y, z, mapID, bIsCharacterSelect);} }
	public static void SendNetworkEvent_EnterOutfitEditor_Response(CPlayer a_Player, List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterOutfitEditor_Response, lstOutfits, lstClothing); }
	public static void SendNetworkEvent_EnterOutfitEditor_Response_ForAll_SpawnedOnly(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterOutfitEditor_Response, lstOutfits, lstClothing);} }
	public static void SendNetworkEvent_EnterOutfitEditor_Response_ForAll_IncludeEveryone(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterOutfitEditor_Response, lstOutfits, lstClothing);} }
	public static void SendNetworkEvent_EnterPlasticSurgeon_OfferCharacterUpgrade(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterPlasticSurgeon_OfferCharacterUpgrade); }
	public static void SendNetworkEvent_EnterPlasticSurgeon_OfferCharacterUpgrade_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterPlasticSurgeon_OfferCharacterUpgrade);} }
	public static void SendNetworkEvent_EnterPlasticSurgeon_OfferCharacterUpgrade_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterPlasticSurgeon_OfferCharacterUpgrade);} }
	public static void SendNetworkEvent_EnterPlasticSurgeon_Response(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterPlasticSurgeon_Response); }
	public static void SendNetworkEvent_EnterPlasticSurgeon_Response_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterPlasticSurgeon_Response);} }
	public static void SendNetworkEvent_EnterPlasticSurgeon_Response_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterPlasticSurgeon_Response);} }
	public static void SendNetworkEvent_EnterTattooArtist_Response(CPlayer a_Player, List<int> lstCurrentTattooIDs) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterTattooArtist_Response, lstCurrentTattooIDs); }
	public static void SendNetworkEvent_EnterTattooArtist_Response_ForAll_SpawnedOnly(List<int> lstCurrentTattooIDs){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterTattooArtist_Response, lstCurrentTattooIDs);} }
	public static void SendNetworkEvent_EnterTattooArtist_Response_ForAll_IncludeEveryone(List<int> lstCurrentTattooIDs){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterTattooArtist_Response, lstCurrentTattooIDs);} }
	public static void SendNetworkEvent_EnterVehicleReal(CPlayer a_Player, Vehicle vehicle, int seatId) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterVehicleReal, vehicle, seatId); }
	public static void SendNetworkEvent_EnterVehicleReal_ForAll_SpawnedOnly(Vehicle vehicle, int seatId){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterVehicleReal, vehicle, seatId);} }
	public static void SendNetworkEvent_EnterVehicleReal_ForAll_IncludeEveryone(Vehicle vehicle, int seatId){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.EnterVehicleReal, vehicle, seatId);} }
	public static void SendNetworkEvent_ExitElevatorApproved(CPlayer a_Player, float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitElevatorApproved, x, y, z, mapID, bHasParentInterior, parentMap); }
	public static void SendNetworkEvent_ExitElevatorApproved_ForAll_SpawnedOnly(float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitElevatorApproved, x, y, z, mapID, bHasParentInterior, parentMap);} }
	public static void SendNetworkEvent_ExitElevatorApproved_ForAll_IncludeEveryone(float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitElevatorApproved, x, y, z, mapID, bHasParentInterior, parentMap);} }
	public static void SendNetworkEvent_ExitInteriorApproved(CPlayer a_Player, float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitInteriorApproved, x, y, z, mapID, bHasParentInterior, parentMap); }
	public static void SendNetworkEvent_ExitInteriorApproved_ForAll_SpawnedOnly(float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitInteriorApproved, x, y, z, mapID, bHasParentInterior, parentMap);} }
	public static void SendNetworkEvent_ExitInteriorApproved_ForAll_IncludeEveryone(float x, float y, float z, int mapID, bool bHasParentInterior, int parentMap){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitInteriorApproved, x, y, z, mapID, bHasParentInterior, parentMap);} }
	public static void SendNetworkEvent_ExitVehicleReal(CPlayer a_Player, VehicleType vehicleBeingExited) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitVehicleReal, vehicleBeingExited); }
	public static void SendNetworkEvent_ExitVehicleReal_ForAll_SpawnedOnly(VehicleType vehicleBeingExited){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitVehicleReal, vehicleBeingExited);} }
	public static void SendNetworkEvent_ExitVehicleReal_ForAll_IncludeEveryone(VehicleType vehicleBeingExited){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitVehicleReal, vehicleBeingExited);} }
	public static void SendNetworkEvent_ExitVehicleStart(CPlayer a_Player, VehicleType vehicleBeingExited, int seatID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitVehicleStart, vehicleBeingExited, seatID); }
	public static void SendNetworkEvent_ExitVehicleStart_ForAll_SpawnedOnly(VehicleType vehicleBeingExited, int seatID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitVehicleStart, vehicleBeingExited, seatID);} }
	public static void SendNetworkEvent_ExitVehicleStart_ForAll_IncludeEveryone(VehicleType vehicleBeingExited, int seatID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ExitVehicleStart, vehicleBeingExited, seatID);} }
	public static void SendNetworkEvent_FactionCreateResult(CPlayer a_Player, ECreateFactionResult creationError) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FactionCreateResult, creationError); }
	public static void SendNetworkEvent_FactionCreateResult_ForAll_SpawnedOnly(ECreateFactionResult creationError){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FactionCreateResult, creationError);} }
	public static void SendNetworkEvent_FactionCreateResult_ForAll_IncludeEveryone(ECreateFactionResult creationError){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FactionCreateResult, creationError);} }
	public static void SendNetworkEvent_FactionTransactionComplete(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FactionTransactionComplete); }
	public static void SendNetworkEvent_FactionTransactionComplete_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FactionTransactionComplete);} }
	public static void SendNetworkEvent_FactionTransactionComplete_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FactionTransactionComplete);} }
	public static void SendNetworkEvent_Faction_AdminViewFactions(CPlayer a_Player, List<CFactionListTransmit> lstFactions) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Faction_AdminViewFactions, lstFactions); }
	public static void SendNetworkEvent_Faction_AdminViewFactions_ForAll_SpawnedOnly(List<CFactionListTransmit> lstFactions){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Faction_AdminViewFactions, lstFactions);} }
	public static void SendNetworkEvent_Faction_AdminViewFactions_ForAll_IncludeEveryone(List<CFactionListTransmit> lstFactions){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Faction_AdminViewFactions, lstFactions);} }
	public static void SendNetworkEvent_Faction_ViewFactionVehicles_Response(CPlayer a_Player, List<CFactionVehicleInfoTransmit> lstFactionVehicles) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Faction_ViewFactionVehicles_Response, lstFactionVehicles); }
	public static void SendNetworkEvent_Faction_ViewFactionVehicles_Response_ForAll_SpawnedOnly(List<CFactionVehicleInfoTransmit> lstFactionVehicles){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Faction_ViewFactionVehicles_Response, lstFactionVehicles);} }
	public static void SendNetworkEvent_Faction_ViewFactionVehicles_Response_ForAll_IncludeEveryone(List<CFactionVehicleInfoTransmit> lstFactionVehicles){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Faction_ViewFactionVehicles_Response, lstFactionVehicles);} }
	public static void SendNetworkEvent_FireFullCleanup(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FireFullCleanup); }
	public static void SendNetworkEvent_FireFullCleanup_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FireFullCleanup);} }
	public static void SendNetworkEvent_FireFullCleanup_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FireFullCleanup);} }
	public static void SendNetworkEvent_FireHeliDropWater(CPlayer a_Player, Vector3 vecPos, bool bIsSyncer) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FireHeliDropWater, vecPos, bIsSyncer); }
	public static void SendNetworkEvent_FireHeliDropWater_ForAll_SpawnedOnly(Vector3 vecPos, bool bIsSyncer){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FireHeliDropWater, vecPos, bIsSyncer);} }
	public static void SendNetworkEvent_FireHeliDropWater_ForAll_IncludeEveryone(Vector3 vecPos, bool bIsSyncer){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FireHeliDropWater, vecPos, bIsSyncer);} }
	public static void SendNetworkEvent_FirePartialCleanup(CPlayer a_Player, List<int> cleanedUpSlots) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FirePartialCleanup, cleanedUpSlots); }
	public static void SendNetworkEvent_FirePartialCleanup_ForAll_SpawnedOnly(List<int> cleanedUpSlots){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FirePartialCleanup, cleanedUpSlots);} }
	public static void SendNetworkEvent_FirePartialCleanup_ForAll_IncludeEveryone(List<int> cleanedUpSlots){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FirePartialCleanup, cleanedUpSlots);} }
	public static void SendNetworkEvent_FishingLevelUp(CPlayer a_Player, int newLevel, int xpRequiredForNextLevel) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FishingLevelUp, newLevel, xpRequiredForNextLevel); }
	public static void SendNetworkEvent_FishingLevelUp_ForAll_SpawnedOnly(int newLevel, int xpRequiredForNextLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FishingLevelUp, newLevel, xpRequiredForNextLevel);} }
	public static void SendNetworkEvent_FishingLevelUp_ForAll_IncludeEveryone(int newLevel, int xpRequiredForNextLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FishingLevelUp, newLevel, xpRequiredForNextLevel);} }
	public static void SendNetworkEvent_Fishing_OnBite(CPlayer a_Player, int level) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Fishing_OnBite, level); }
	public static void SendNetworkEvent_Fishing_OnBite_ForAll_SpawnedOnly(int level){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Fishing_OnBite, level);} }
	public static void SendNetworkEvent_Fishing_OnBite_ForAll_IncludeEveryone(int level){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Fishing_OnBite, level);} }
	public static void SendNetworkEvent_Freeze(CPlayer a_Player, bool bFreeze) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Freeze, bFreeze); }
	public static void SendNetworkEvent_Freeze_ForAll_SpawnedOnly(bool bFreeze){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Freeze, bFreeze);} }
	public static void SendNetworkEvent_Freeze_ForAll_IncludeEveryone(bool bFreeze){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Freeze, bFreeze);} }
	public static void SendNetworkEvent_FuelingComplete(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FuelingComplete); }
	public static void SendNetworkEvent_FuelingComplete_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FuelingComplete);} }
	public static void SendNetworkEvent_FuelingComplete_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FuelingComplete);} }
	public static void SendNetworkEvent_FuelingRequestResponse(CPlayer a_Player, bool bSuccess) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FuelingRequestResponse, bSuccess); }
	public static void SendNetworkEvent_FuelingRequestResponse_ForAll_SpawnedOnly(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FuelingRequestResponse, bSuccess);} }
	public static void SendNetworkEvent_FuelingRequestResponse_ForAll_IncludeEveryone(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FuelingRequestResponse, bSuccess);} }
	public static void SendNetworkEvent_FurnitureInventoryDetails(CPlayer a_Player, List<CItemInstanceDef> inventory) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FurnitureInventoryDetails, inventory); }
	public static void SendNetworkEvent_FurnitureInventoryDetails_ForAll_SpawnedOnly(List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FurnitureInventoryDetails, inventory);} }
	public static void SendNetworkEvent_FurnitureInventoryDetails_ForAll_IncludeEveryone(List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FurnitureInventoryDetails, inventory);} }
	public static void SendNetworkEvent_FurnitureStore_OnCheckoutResult(CPlayer a_Player, EStoreCheckoutResult result) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FurnitureStore_OnCheckoutResult, result); }
	public static void SendNetworkEvent_FurnitureStore_OnCheckoutResult_ForAll_SpawnedOnly(EStoreCheckoutResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FurnitureStore_OnCheckoutResult, result);} }
	public static void SendNetworkEvent_FurnitureStore_OnCheckoutResult_ForAll_IncludeEveryone(EStoreCheckoutResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.FurnitureStore_OnCheckoutResult, result);} }
	public static void SendNetworkEvent_GangTags_GotoCreator(CPlayer a_Player, List<GangTagLayer> lstLayers, List<GangTagLayer> lstLayersWIP) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_GotoCreator, lstLayers, lstLayersWIP); }
	public static void SendNetworkEvent_GangTags_GotoCreator_ForAll_SpawnedOnly(List<GangTagLayer> lstLayers, List<GangTagLayer> lstLayersWIP){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_GotoCreator, lstLayers, lstLayersWIP);} }
	public static void SendNetworkEvent_GangTags_GotoCreator_ForAll_IncludeEveryone(List<GangTagLayer> lstLayers, List<GangTagLayer> lstLayersWIP){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_GotoCreator, lstLayers, lstLayersWIP);} }
	public static void SendNetworkEvent_GangTags_GotoTagMode(CPlayer a_Player, List<GangTagLayer> lstLayers) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_GotoTagMode, lstLayers); }
	public static void SendNetworkEvent_GangTags_GotoTagMode_ForAll_SpawnedOnly(List<GangTagLayer> lstLayers){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_GotoTagMode, lstLayers);} }
	public static void SendNetworkEvent_GangTags_GotoTagMode_ForAll_IncludeEveryone(List<GangTagLayer> lstLayers){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_GotoTagMode, lstLayers);} }
	public static void SendNetworkEvent_GangTags_RequestShareTag(CPlayer a_Player, string strSourceName, List<GangTagLayer> lstLayers) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_RequestShareTag, strSourceName, lstLayers); }
	public static void SendNetworkEvent_GangTags_RequestShareTag_ForAll_SpawnedOnly(string strSourceName, List<GangTagLayer> lstLayers){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_RequestShareTag, strSourceName, lstLayers);} }
	public static void SendNetworkEvent_GangTags_RequestShareTag_ForAll_IncludeEveryone(string strSourceName, List<GangTagLayer> lstLayers){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GangTags_RequestShareTag, strSourceName, lstLayers);} }
	public static void SendNetworkEvent_Generics_ShowGenericUI(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Generics_ShowGenericUI); }
	public static void SendNetworkEvent_Generics_ShowGenericUI_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Generics_ShowGenericUI);} }
	public static void SendNetworkEvent_Generics_ShowGenericUI_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Generics_ShowGenericUI);} }
	public static void SendNetworkEvent_GiveAIOwnership(CPlayer a_Player, int number) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GiveAIOwnership, number); }
	public static void SendNetworkEvent_GiveAIOwnership_ForAll_SpawnedOnly(int number){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GiveAIOwnership, number);} }
	public static void SendNetworkEvent_GiveAIOwnership_ForAll_IncludeEveryone(int number){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GiveAIOwnership, number);} }
	public static void SendNetworkEvent_GotBasicDonatorInfo(CPlayer a_Player, int donatorCurrency, List<DonationInventoryItemTransmit> lstDonationInventory) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotBasicDonatorInfo, donatorCurrency, lstDonationInventory); }
	public static void SendNetworkEvent_GotBasicDonatorInfo_ForAll_SpawnedOnly(int donatorCurrency, List<DonationInventoryItemTransmit> lstDonationInventory){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotBasicDonatorInfo, donatorCurrency, lstDonationInventory);} }
	public static void SendNetworkEvent_GotBasicDonatorInfo_ForAll_IncludeEveryone(int donatorCurrency, List<DonationInventoryItemTransmit> lstDonationInventory){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotBasicDonatorInfo, donatorCurrency, lstDonationInventory);} }
	public static void SendNetworkEvent_GotBasicRadioInfo(CPlayer a_Player, int donatorCurrency) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotBasicRadioInfo, donatorCurrency); }
	public static void SendNetworkEvent_GotBasicRadioInfo_ForAll_SpawnedOnly(int donatorCurrency){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotBasicRadioInfo, donatorCurrency);} }
	public static void SendNetworkEvent_GotBasicRadioInfo_ForAll_IncludeEveryone(int donatorCurrency){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotBasicRadioInfo, donatorCurrency);} }
	public static void SendNetworkEvent_GotDonationPurchasables(CPlayer a_Player, List<DonationPurchasable> lstPurchasables) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotDonationPurchasables, lstPurchasables); }
	public static void SendNetworkEvent_GotDonationPurchasables_ForAll_SpawnedOnly(List<DonationPurchasable> lstPurchasables){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotDonationPurchasables, lstPurchasables);} }
	public static void SendNetworkEvent_GotDonationPurchasables_ForAll_IncludeEveryone(List<DonationPurchasable> lstPurchasables){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotDonationPurchasables, lstPurchasables);} }
	public static void SendNetworkEvent_GotoDiscordLinking_Response(CPlayer a_Player, bool bHasLink, string strDiscordName, string strEndpoint) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoDiscordLinking_Response, bHasLink, strDiscordName, strEndpoint); }
	public static void SendNetworkEvent_GotoDiscordLinking_Response_ForAll_SpawnedOnly(bool bHasLink, string strDiscordName, string strEndpoint){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoDiscordLinking_Response, bHasLink, strDiscordName, strEndpoint);} }
	public static void SendNetworkEvent_GotoDiscordLinking_Response_ForAll_IncludeEveryone(bool bHasLink, string strDiscordName, string strEndpoint){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoDiscordLinking_Response, bHasLink, strDiscordName, strEndpoint);} }
	public static void SendNetworkEvent_GotoFurnitureStore(CPlayer a_Player, float fStateSalesTax) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoFurnitureStore, fStateSalesTax); }
	public static void SendNetworkEvent_GotoFurnitureStore_ForAll_SpawnedOnly(float fStateSalesTax){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoFurnitureStore, fStateSalesTax);} }
	public static void SendNetworkEvent_GotoFurnitureStore_ForAll_IncludeEveryone(float fStateSalesTax){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoFurnitureStore, fStateSalesTax);} }
	public static void SendNetworkEvent_GotoLogin(CPlayer a_Player, bool bShowGUI) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoLogin, bShowGUI); }
	public static void SendNetworkEvent_GotoLogin_ForAll_SpawnedOnly(bool bShowGUI){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoLogin, bShowGUI);} }
	public static void SendNetworkEvent_GotoLogin_ForAll_IncludeEveryone(bool bShowGUI){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoLogin, bShowGUI);} }
	public static void SendNetworkEvent_GotoPropertyEditMode(CPlayer a_Player, List<CItemInstanceDef> lstFurnitureItemsAvailable) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoPropertyEditMode, lstFurnitureItemsAvailable); }
	public static void SendNetworkEvent_GotoPropertyEditMode_ForAll_SpawnedOnly(List<CItemInstanceDef> lstFurnitureItemsAvailable){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoPropertyEditMode, lstFurnitureItemsAvailable);} }
	public static void SendNetworkEvent_GotoPropertyEditMode_ForAll_IncludeEveryone(List<CItemInstanceDef> lstFurnitureItemsAvailable){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoPropertyEditMode, lstFurnitureItemsAvailable);} }
	public static void SendNetworkEvent_GotoRadioStationManagement(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoRadioStationManagement); }
	public static void SendNetworkEvent_GotoRadioStationManagement_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoRadioStationManagement);} }
	public static void SendNetworkEvent_GotoRadioStationManagement_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoRadioStationManagement);} }
	public static void SendNetworkEvent_GotoRoadblockEditor(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoRoadblockEditor); }
	public static void SendNetworkEvent_GotoRoadblockEditor_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoRoadblockEditor);} }
	public static void SendNetworkEvent_GotoRoadblockEditor_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoRoadblockEditor);} }
	public static void SendNetworkEvent_GotoTutorialState(CPlayer a_Player, ETutorialVersions currentTutorialVersion) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoTutorialState, currentTutorialVersion); }
	public static void SendNetworkEvent_GotoTutorialState_ForAll_SpawnedOnly(ETutorialVersions currentTutorialVersion){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoTutorialState, currentTutorialVersion);} }
	public static void SendNetworkEvent_GotoTutorialState_ForAll_IncludeEveryone(ETutorialVersions currentTutorialVersion){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoTutorialState, currentTutorialVersion);} }
	public static void SendNetworkEvent_GotoVehicleModShop_Approved(CPlayer a_Player, VehicleType vehicle) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoVehicleModShop_Approved, vehicle); }
	public static void SendNetworkEvent_GotoVehicleModShop_Approved_ForAll_SpawnedOnly(VehicleType vehicle){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoVehicleModShop_Approved, vehicle);} }
	public static void SendNetworkEvent_GotoVehicleModShop_Approved_ForAll_IncludeEveryone(VehicleType vehicle){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotoVehicleModShop_Approved, vehicle);} }
	public static void SendNetworkEvent_GotPhoneContactByNumber(CPlayer a_Player, string contactName) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneContactByNumber, contactName); }
	public static void SendNetworkEvent_GotPhoneContactByNumber_ForAll_SpawnedOnly(string contactName){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneContactByNumber, contactName);} }
	public static void SendNetworkEvent_GotPhoneContactByNumber_ForAll_IncludeEveryone(string contactName){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneContactByNumber, contactName);} }
	public static void SendNetworkEvent_GotPhoneContacts(CPlayer a_Player, List<KeyValuePair<string, string>> contactslist) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneContacts, contactslist); }
	public static void SendNetworkEvent_GotPhoneContacts_ForAll_SpawnedOnly(List<KeyValuePair<string, string>> contactslist){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneContacts, contactslist);} }
	public static void SendNetworkEvent_GotPhoneContacts_ForAll_IncludeEveryone(List<KeyValuePair<string, string>> contactslist){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneContacts, contactslist);} }
	public static void SendNetworkEvent_GotPhoneMessagesContacts(CPlayer a_Player, List<CPhoneMessageContact> contactslist) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneMessagesContacts, contactslist); }
	public static void SendNetworkEvent_GotPhoneMessagesContacts_ForAll_SpawnedOnly(List<CPhoneMessageContact> contactslist){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneMessagesContacts, contactslist);} }
	public static void SendNetworkEvent_GotPhoneMessagesContacts_ForAll_IncludeEveryone(List<CPhoneMessageContact> contactslist){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneMessagesContacts, contactslist);} }
	public static void SendNetworkEvent_GotPhoneMessagesFromNumber(CPlayer a_Player, List<CPhoneMessage> messagesList) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneMessagesFromNumber, messagesList); }
	public static void SendNetworkEvent_GotPhoneMessagesFromNumber_ForAll_SpawnedOnly(List<CPhoneMessage> messagesList){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneMessagesFromNumber, messagesList);} }
	public static void SendNetworkEvent_GotPhoneMessagesFromNumber_ForAll_IncludeEveryone(List<CPhoneMessage> messagesList){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotPhoneMessagesFromNumber, messagesList);} }
	public static void SendNetworkEvent_GotQuizQuestions(CPlayer a_Player, List<CQuizQuestion> lstQuizQuestions) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotQuizQuestions, lstQuizQuestions); }
	public static void SendNetworkEvent_GotQuizQuestions_ForAll_SpawnedOnly(List<CQuizQuestion> lstQuizQuestions){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotQuizQuestions, lstQuizQuestions);} }
	public static void SendNetworkEvent_GotQuizQuestions_ForAll_IncludeEveryone(List<CQuizQuestion> lstQuizQuestions){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotQuizQuestions, lstQuizQuestions);} }
	public static void SendNetworkEvent_GotQuizWrittenQuestions(CPlayer a_Player, List<CQuizWrittenQuestion> lstWrittenQuestions) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotQuizWrittenQuestions, lstWrittenQuestions); }
	public static void SendNetworkEvent_GotQuizWrittenQuestions_ForAll_SpawnedOnly(List<CQuizWrittenQuestion> lstWrittenQuestions){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotQuizWrittenQuestions, lstWrittenQuestions);} }
	public static void SendNetworkEvent_GotQuizWrittenQuestions_ForAll_IncludeEveryone(List<CQuizWrittenQuestion> lstWrittenQuestions){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotQuizWrittenQuestions, lstWrittenQuestions);} }
	public static void SendNetworkEvent_GotStoreInfo(CPlayer a_Player, List<EItemID> items, float fSalesTaxRate) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotStoreInfo, items, fSalesTaxRate); }
	public static void SendNetworkEvent_GotStoreInfo_ForAll_SpawnedOnly(List<EItemID> items, float fSalesTaxRate){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotStoreInfo, items, fSalesTaxRate);} }
	public static void SendNetworkEvent_GotStoreInfo_ForAll_IncludeEveryone(List<EItemID> items, float fSalesTaxRate){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotStoreInfo, items, fSalesTaxRate);} }
	public static void SendNetworkEvent_GotTotalUnviewedMessages(CPlayer a_Player, int unreadMessages) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotTotalUnviewedMessages, unreadMessages); }
	public static void SendNetworkEvent_GotTotalUnviewedMessages_ForAll_SpawnedOnly(int unreadMessages){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotTotalUnviewedMessages, unreadMessages);} }
	public static void SendNetworkEvent_GotTotalUnviewedMessages_ForAll_IncludeEveryone(int unreadMessages){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GotTotalUnviewedMessages, unreadMessages);} }
	public static void SendNetworkEvent_GPS_Clear(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GPS_Clear); }
	public static void SendNetworkEvent_GPS_Clear_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GPS_Clear);} }
	public static void SendNetworkEvent_GPS_Clear_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GPS_Clear);} }
	public static void SendNetworkEvent_GPS_Set(CPlayer a_Player, Vector3 vecPos) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GPS_Set, vecPos); }
	public static void SendNetworkEvent_GPS_Set_ForAll_SpawnedOnly(Vector3 vecPos){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GPS_Set, vecPos);} }
	public static void SendNetworkEvent_GPS_Set_ForAll_IncludeEveryone(Vector3 vecPos){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.GPS_Set, vecPos);} }
	public static void SendNetworkEvent_HelpRequestCommandsResponse(CPlayer a_Player, List<CommandHelpInfo> lstCommands) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.HelpRequestCommandsResponse, lstCommands); }
	public static void SendNetworkEvent_HelpRequestCommandsResponse_ForAll_SpawnedOnly(List<CommandHelpInfo> lstCommands){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.HelpRequestCommandsResponse, lstCommands);} }
	public static void SendNetworkEvent_HelpRequestCommandsResponse_ForAll_IncludeEveryone(List<CommandHelpInfo> lstCommands){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.HelpRequestCommandsResponse, lstCommands);} }
	public static void SendNetworkEvent_InformClientTaggingComplete(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InformClientTaggingComplete); }
	public static void SendNetworkEvent_InformClientTaggingComplete_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InformClientTaggingComplete);} }
	public static void SendNetworkEvent_InformClientTaggingComplete_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InformClientTaggingComplete);} }
	public static void SendNetworkEvent_InformPlayerOfFireModes(CPlayer a_Player, bool isSemiAuto) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InformPlayerOfFireModes, isSemiAuto); }
	public static void SendNetworkEvent_InformPlayerOfFireModes_ForAll_SpawnedOnly(bool isSemiAuto){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InformPlayerOfFireModes, isSemiAuto);} }
	public static void SendNetworkEvent_InformPlayerOfFireModes_ForAll_IncludeEveryone(bool isSemiAuto){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InformPlayerOfFireModes, isSemiAuto);} }
	public static void SendNetworkEvent_InitialJoinEvent(CPlayer a_Player, int day, int month, int year, int hour, int min, int sec, int weather, bool bIsDebug) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InitialJoinEvent, day, month, year, hour, min, sec, weather, bIsDebug); }
	public static void SendNetworkEvent_InitialJoinEvent_ForAll_SpawnedOnly(int day, int month, int year, int hour, int min, int sec, int weather, bool bIsDebug){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InitialJoinEvent, day, month, year, hour, min, sec, weather, bIsDebug);} }
	public static void SendNetworkEvent_InitialJoinEvent_ForAll_IncludeEveryone(int day, int month, int year, int hour, int min, int sec, int weather, bool bIsDebug){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InitialJoinEvent, day, month, year, hour, min, sec, weather, bIsDebug);} }
	public static void SendNetworkEvent_InitiateJerryCanRefuel(CPlayer a_Player, Int64 itemDBID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InitiateJerryCanRefuel, itemDBID); }
	public static void SendNetworkEvent_InitiateJerryCanRefuel_ForAll_SpawnedOnly(Int64 itemDBID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InitiateJerryCanRefuel, itemDBID);} }
	public static void SendNetworkEvent_InitiateJerryCanRefuel_ForAll_IncludeEveryone(Int64 itemDBID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.InitiateJerryCanRefuel, itemDBID);} }
	public static void SendNetworkEvent_JoinTvBroadcast(CPlayer a_Player, PlayerType NewsCameraOperator, ObjectType NewsCameraObject) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.JoinTvBroadcast, NewsCameraOperator, NewsCameraObject); }
	public static void SendNetworkEvent_JoinTvBroadcast_ForAll_SpawnedOnly(PlayerType NewsCameraOperator, ObjectType NewsCameraObject){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.JoinTvBroadcast, NewsCameraOperator, NewsCameraObject);} }
	public static void SendNetworkEvent_JoinTvBroadcast_ForAll_IncludeEveryone(PlayerType NewsCameraOperator, ObjectType NewsCameraObject){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.JoinTvBroadcast, NewsCameraOperator, NewsCameraObject);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_AckBlock(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_AckBlock, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_AckBlock_ForAll_SpawnedOnly(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_AckBlock, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_AckBlock_ForAll_IncludeEveryone(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_AckBlock, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_AckFinalTransfer(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_AckFinalTransfer, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_AckFinalTransfer_ForAll_SpawnedOnly(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_AckFinalTransfer, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_AckFinalTransfer_ForAll_IncludeEveryone(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_AckFinalTransfer, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_RequestResend(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_RequestResend, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_RequestResend_ForAll_SpawnedOnly(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_RequestResend, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_RequestResend_ForAll_IncludeEveryone(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_RequestResend, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_ServerAck(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_ServerAck, a_TransferType, a_Identifier); }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_ServerAck_ForAll_SpawnedOnly(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_ServerAck, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_ClientToServer_ServerAck_ForAll_IncludeEveryone(ELargeDataTransferType a_TransferType, int a_Identifier){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ClientToServer_ServerAck, a_TransferType, a_Identifier);} }
	public static void SendNetworkEvent_LargeDataTransfer_SendBytes(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier, byte[] dataBytes) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_SendBytes, a_TransferType, a_Identifier, dataBytes); }
	public static void SendNetworkEvent_LargeDataTransfer_SendBytes_ForAll_SpawnedOnly(ELargeDataTransferType a_TransferType, int a_Identifier, byte[] dataBytes){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_SendBytes, a_TransferType, a_Identifier, dataBytes);} }
	public static void SendNetworkEvent_LargeDataTransfer_SendBytes_ForAll_IncludeEveryone(ELargeDataTransferType a_TransferType, int a_Identifier, byte[] dataBytes){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_SendBytes, a_TransferType, a_Identifier, dataBytes);} }
	public static void SendNetworkEvent_LargeDataTransfer_ServerToClient_Begin(CPlayer a_Player, ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, bool bAllowClientsideCaching, byte[] key) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ServerToClient_Begin, a_TransferType, a_Identifier, totalBytes, crc32, bAllowClientsideCaching, key); }
	public static void SendNetworkEvent_LargeDataTransfer_ServerToClient_Begin_ForAll_SpawnedOnly(ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, bool bAllowClientsideCaching, byte[] key){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ServerToClient_Begin, a_TransferType, a_Identifier, totalBytes, crc32, bAllowClientsideCaching, key);} }
	public static void SendNetworkEvent_LargeDataTransfer_ServerToClient_Begin_ForAll_IncludeEveryone(ELargeDataTransferType a_TransferType, int a_Identifier, int totalBytes, int crc32, bool bAllowClientsideCaching, byte[] key){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LargeDataTransfer_ServerToClient_Begin, a_TransferType, a_Identifier, totalBytes, crc32, bAllowClientsideCaching, key);} }
	public static void SendNetworkEvent_LeaveTvBroadcast(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LeaveTvBroadcast); }
	public static void SendNetworkEvent_LeaveTvBroadcast_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LeaveTvBroadcast);} }
	public static void SendNetworkEvent_LeaveTvBroadcast_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LeaveTvBroadcast);} }
	public static void SendNetworkEvent_ListNearbyTags(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ListNearbyTags); }
	public static void SendNetworkEvent_ListNearbyTags_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ListNearbyTags);} }
	public static void SendNetworkEvent_ListNearbyTags_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ListNearbyTags);} }
	public static void SendNetworkEvent_LoadCustomMap(CPlayer a_Player, int mapID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadCustomMap, mapID); }
	public static void SendNetworkEvent_LoadCustomMap_ForAll_SpawnedOnly(int mapID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadCustomMap, mapID);} }
	public static void SendNetworkEvent_LoadCustomMap_ForAll_IncludeEveryone(int mapID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadCustomMap, mapID);} }
	public static void SendNetworkEvent_LoadDefaultChatSettings(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadDefaultChatSettings); }
	public static void SendNetworkEvent_LoadDefaultChatSettings_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadDefaultChatSettings);} }
	public static void SendNetworkEvent_LoadDefaultChatSettings_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadDefaultChatSettings);} }
	public static void SendNetworkEvent_LoadTransferAssetsCharacterData(CPlayer a_Player, long characterId, float money, float bankmoney, List<SVehicle> vehicles, List<SProperty> properties) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadTransferAssetsCharacterData, characterId, money, bankmoney, vehicles, properties); }
	public static void SendNetworkEvent_LoadTransferAssetsCharacterData_ForAll_SpawnedOnly(long characterId, float money, float bankmoney, List<SVehicle> vehicles, List<SProperty> properties){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadTransferAssetsCharacterData, characterId, money, bankmoney, vehicles, properties);} }
	public static void SendNetworkEvent_LoadTransferAssetsCharacterData_ForAll_IncludeEveryone(long characterId, float money, float bankmoney, List<SVehicle> vehicles, List<SProperty> properties){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoadTransferAssetsCharacterData, characterId, money, bankmoney, vehicles, properties);} }
	public static void SendNetworkEvent_LocalPlayerInventoryFull(CPlayer a_Player, List<CItemInstanceDef> inventory, EShowInventoryAction showInventoryAction) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LocalPlayerInventoryFull, inventory, showInventoryAction); }
	public static void SendNetworkEvent_LocalPlayerInventoryFull_ForAll_SpawnedOnly(List<CItemInstanceDef> inventory, EShowInventoryAction showInventoryAction){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LocalPlayerInventoryFull, inventory, showInventoryAction);} }
	public static void SendNetworkEvent_LocalPlayerInventoryFull_ForAll_IncludeEveryone(List<CItemInstanceDef> inventory, EShowInventoryAction showInventoryAction){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LocalPlayerInventoryFull, inventory, showInventoryAction);} }
	public static void SendNetworkEvent_LoginResult(CPlayer a_Player, bool bSuccessful, int userID, string titleID, string Username, string strErrorMessage) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoginResult, bSuccessful, userID, titleID, Username, strErrorMessage); }
	public static void SendNetworkEvent_LoginResult_ForAll_SpawnedOnly(bool bSuccessful, int userID, string titleID, string Username, string strErrorMessage){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoginResult, bSuccessful, userID, titleID, Username, strErrorMessage);} }
	public static void SendNetworkEvent_LoginResult_ForAll_IncludeEveryone(bool bSuccessful, int userID, string titleID, string Username, string strErrorMessage){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.LoginResult, bSuccessful, userID, titleID, Username, strErrorMessage);} }
	public static void SendNetworkEvent_Marijuana_CloseMenu(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_CloseMenu); }
	public static void SendNetworkEvent_Marijuana_CloseMenu_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_CloseMenu);} }
	public static void SendNetworkEvent_Marijuana_CloseMenu_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_CloseMenu);} }
	public static void SendNetworkEvent_Marijuana_FertilizeNearbyPlant(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_FertilizeNearbyPlant); }
	public static void SendNetworkEvent_Marijuana_FertilizeNearbyPlant_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_FertilizeNearbyPlant);} }
	public static void SendNetworkEvent_Marijuana_FertilizeNearbyPlant_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_FertilizeNearbyPlant);} }
	public static void SendNetworkEvent_Marijuana_SheerNearbyPlant(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_SheerNearbyPlant); }
	public static void SendNetworkEvent_Marijuana_SheerNearbyPlant_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_SheerNearbyPlant);} }
	public static void SendNetworkEvent_Marijuana_SheerNearbyPlant_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_SheerNearbyPlant);} }
	public static void SendNetworkEvent_Marijuana_WaterNearbyPlant(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_WaterNearbyPlant); }
	public static void SendNetworkEvent_Marijuana_WaterNearbyPlant_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_WaterNearbyPlant);} }
	public static void SendNetworkEvent_Marijuana_WaterNearbyPlant_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Marijuana_WaterNearbyPlant);} }
	public static void SendNetworkEvent_MdcPersonResult(CPlayer a_Player, CStatsResult personInfo) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcPersonResult, personInfo); }
	public static void SendNetworkEvent_MdcPersonResult_ForAll_SpawnedOnly(CStatsResult personInfo){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcPersonResult, personInfo);} }
	public static void SendNetworkEvent_MdcPersonResult_ForAll_IncludeEveryone(CStatsResult personInfo){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcPersonResult, personInfo);} }
	public static void SendNetworkEvent_MdcPropertyResult(CPlayer a_Player, CMdtProperty propertyInfo) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcPropertyResult, propertyInfo); }
	public static void SendNetworkEvent_MdcPropertyResult_ForAll_SpawnedOnly(CMdtProperty propertyInfo){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcPropertyResult, propertyInfo);} }
	public static void SendNetworkEvent_MdcPropertyResult_ForAll_IncludeEveryone(CMdtProperty propertyInfo){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcPropertyResult, propertyInfo);} }
	public static void SendNetworkEvent_MdcVehicleResult(CPlayer a_Player, CMdtVehicle vehicleInfo) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcVehicleResult, vehicleInfo); }
	public static void SendNetworkEvent_MdcVehicleResult_ForAll_SpawnedOnly(CMdtVehicle vehicleInfo){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcVehicleResult, vehicleInfo);} }
	public static void SendNetworkEvent_MdcVehicleResult_ForAll_IncludeEveryone(CMdtVehicle vehicleInfo){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.MdcVehicleResult, vehicleInfo);} }
	public static void SendNetworkEvent_Notification(CPlayer a_Player, string strTitle, string strMessage, ENotificationIcon icon) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Notification, strTitle, strMessage, icon); }
	public static void SendNetworkEvent_Notification_ForAll_SpawnedOnly(string strTitle, string strMessage, ENotificationIcon icon){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Notification, strTitle, strMessage, icon);} }
	public static void SendNetworkEvent_Notification_ForAll_IncludeEveryone(string strTitle, string strMessage, ENotificationIcon icon){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Notification, strTitle, strMessage, icon);} }
	public static void SendNetworkEvent_OfferNewTutorial(CPlayer a_Player, ETutorialVersions currentTutorialVersion) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.OfferNewTutorial, currentTutorialVersion); }
	public static void SendNetworkEvent_OfferNewTutorial_ForAll_SpawnedOnly(ETutorialVersions currentTutorialVersion){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.OfferNewTutorial, currentTutorialVersion);} }
	public static void SendNetworkEvent_OfferNewTutorial_ForAll_IncludeEveryone(ETutorialVersions currentTutorialVersion){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.OfferNewTutorial, currentTutorialVersion);} }
	public static void SendNetworkEvent_OnStoreCheckout_Response(CPlayer a_Player, EStoreCheckoutResult result) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.OnStoreCheckout_Response, result); }
	public static void SendNetworkEvent_OnStoreCheckout_Response_ForAll_SpawnedOnly(EStoreCheckoutResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.OnStoreCheckout_Response, result);} }
	public static void SendNetworkEvent_OnStoreCheckout_Response_ForAll_IncludeEveryone(EStoreCheckoutResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.OnStoreCheckout_Response, result);} }
	public static void SendNetworkEvent_PendingApplicationDetails(CPlayer a_Player, PendingApplicationDetails pendingAppDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PendingApplicationDetails, pendingAppDetails); }
	public static void SendNetworkEvent_PendingApplicationDetails_ForAll_SpawnedOnly(PendingApplicationDetails pendingAppDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PendingApplicationDetails, pendingAppDetails);} }
	public static void SendNetworkEvent_PendingApplicationDetails_ForAll_IncludeEveryone(PendingApplicationDetails pendingAppDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PendingApplicationDetails, pendingAppDetails);} }
	public static void SendNetworkEvent_PerfData(CPlayer a_Player, int fps) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PerfData, fps); }
	public static void SendNetworkEvent_PerfData_ForAll_SpawnedOnly(int fps){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PerfData, fps);} }
	public static void SendNetworkEvent_PerfData_ForAll_IncludeEveryone(int fps){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PerfData, fps);} }
	public static void SendNetworkEvent_PersistentNotifications_Created(CPlayer a_Player, CPersistentNotification notification) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PersistentNotifications_Created, notification); }
	public static void SendNetworkEvent_PersistentNotifications_Created_ForAll_SpawnedOnly(CPersistentNotification notification){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PersistentNotifications_Created, notification);} }
	public static void SendNetworkEvent_PersistentNotifications_Created_ForAll_IncludeEveryone(CPersistentNotification notification){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PersistentNotifications_Created, notification);} }
	public static void SendNetworkEvent_PersistentNotifications_LoadAll(CPlayer a_Player, List<CPersistentNotification> lstNotifications) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PersistentNotifications_LoadAll, lstNotifications); }
	public static void SendNetworkEvent_PersistentNotifications_LoadAll_ForAll_SpawnedOnly(List<CPersistentNotification> lstNotifications){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PersistentNotifications_LoadAll, lstNotifications);} }
	public static void SendNetworkEvent_PersistentNotifications_LoadAll_ForAll_IncludeEveryone(List<CPersistentNotification> lstNotifications){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PersistentNotifications_LoadAll, lstNotifications);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoDropoffState_Response(CPlayer a_Player, EJobID a_JobID, Vector3 a_vecCheckpointPos) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_GotoDropoffState_Response, a_JobID, a_vecCheckpointPos); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoDropoffState_Response_ForAll_SpawnedOnly(EJobID a_JobID, Vector3 a_vecCheckpointPos){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_GotoDropoffState_Response, a_JobID, a_vecCheckpointPos);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoDropoffState_Response_ForAll_IncludeEveryone(EJobID a_JobID, Vector3 a_vecCheckpointPos){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_GotoDropoffState_Response, a_JobID, a_vecCheckpointPos);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoPickupState_Response(CPlayer a_Player, EJobID a_JobID, Vector3 a_vecCheckpointPos) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_GotoPickupState_Response, a_JobID, a_vecCheckpointPos); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoPickupState_Response_ForAll_SpawnedOnly(EJobID a_JobID, Vector3 a_vecCheckpointPos){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_GotoPickupState_Response, a_JobID, a_vecCheckpointPos);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_GotoPickupState_Response_ForAll_IncludeEveryone(EJobID a_JobID, Vector3 a_vecCheckpointPos){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_GotoPickupState_Response, a_JobID, a_vecCheckpointPos);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyDropoff_Response(CPlayer a_Player, EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_VerifyDropoff_Response, a_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyDropoff_Response_ForAll_SpawnedOnly(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_VerifyDropoff_Response, a_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyDropoff_Response_ForAll_IncludeEveryone(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_VerifyDropoff_Response, a_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyPickup_Response(CPlayer a_Player, EJobID a_JobID, bool bIsValid) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_VerifyPickup_Response, a_JobID, bIsValid); }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyPickup_Response_ForAll_SpawnedOnly(EJobID a_JobID, bool bIsValid){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_VerifyPickup_Response, a_JobID, bIsValid);} }
	public static void SendNetworkEvent_PickupDropoffBasedJob_VerifyPickup_Response_ForAll_IncludeEveryone(EJobID a_JobID, bool bIsValid){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PickupDropoffBasedJob_VerifyPickup_Response, a_JobID, bIsValid);} }
	public static void SendNetworkEvent_PlasticSurgeon_GotPrice(CPlayer a_Player, float fPrice, bool hasToken) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlasticSurgeon_GotPrice, fPrice, hasToken); }
	public static void SendNetworkEvent_PlasticSurgeon_GotPrice_ForAll_SpawnedOnly(float fPrice, bool hasToken){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlasticSurgeon_GotPrice, fPrice, hasToken);} }
	public static void SendNetworkEvent_PlasticSurgeon_GotPrice_ForAll_IncludeEveryone(float fPrice, bool hasToken){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlasticSurgeon_GotPrice, fPrice, hasToken);} }
	public static void SendNetworkEvent_PlayCustomAudio(CPlayer a_Player, EAudioIDs audioID, bool bStopAllOtherAudio) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayCustomAudio, audioID, bStopAllOtherAudio); }
	public static void SendNetworkEvent_PlayCustomAudio_ForAll_SpawnedOnly(EAudioIDs audioID, bool bStopAllOtherAudio){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayCustomAudio, audioID, bStopAllOtherAudio);} }
	public static void SendNetworkEvent_PlayCustomAudio_ForAll_IncludeEveryone(EAudioIDs audioID, bool bStopAllOtherAudio){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayCustomAudio, audioID, bStopAllOtherAudio);} }
	public static void SendNetworkEvent_PlayCustomSpeech(CPlayer a_Player, PlayerType player, ESpeechID speechID, ESpeechType speechType) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayCustomSpeech, player, speechID, speechType); }
	public static void SendNetworkEvent_PlayCustomSpeech_ForAll_SpawnedOnly(PlayerType player, ESpeechID speechID, ESpeechType speechType){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayCustomSpeech, player, speechID, speechType);} }
	public static void SendNetworkEvent_PlayCustomSpeech_ForAll_IncludeEveryone(PlayerType player, ESpeechID speechID, ESpeechType speechType){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayCustomSpeech, player, speechID, speechType);} }
	public static void SendNetworkEvent_PlayerWentOffDuty(CPlayer a_Player, PlayerType player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayerWentOffDuty, player); }
	public static void SendNetworkEvent_PlayerWentOffDuty_ForAll_SpawnedOnly(PlayerType player){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayerWentOffDuty, player);} }
	public static void SendNetworkEvent_PlayerWentOffDuty_ForAll_IncludeEveryone(PlayerType player){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayerWentOffDuty, player);} }
	public static void SendNetworkEvent_PlayerWentOnDuty(CPlayer a_Player, PlayerType playerGoingOnDuty, EDutyType dutyType) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayerWentOnDuty, playerGoingOnDuty, dutyType); }
	public static void SendNetworkEvent_PlayerWentOnDuty_ForAll_SpawnedOnly(PlayerType playerGoingOnDuty, EDutyType dutyType){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayerWentOnDuty, playerGoingOnDuty, dutyType);} }
	public static void SendNetworkEvent_PlayerWentOnDuty_ForAll_IncludeEveryone(PlayerType playerGoingOnDuty, EDutyType dutyType){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayerWentOnDuty, playerGoingOnDuty, dutyType);} }
	public static void SendNetworkEvent_PlayMetalDetectorAlarm(CPlayer a_Player, Vector3 colshapePosition) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayMetalDetectorAlarm, colshapePosition); }
	public static void SendNetworkEvent_PlayMetalDetectorAlarm_ForAll_SpawnedOnly(Vector3 colshapePosition){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayMetalDetectorAlarm, colshapePosition);} }
	public static void SendNetworkEvent_PlayMetalDetectorAlarm_ForAll_IncludeEveryone(Vector3 colshapePosition){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PlayMetalDetectorAlarm, colshapePosition);} }
	public static void SendNetworkEvent_PreviewCharacterGotData(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PreviewCharacterGotData); }
	public static void SendNetworkEvent_PreviewCharacterGotData_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PreviewCharacterGotData);} }
	public static void SendNetworkEvent_PreviewCharacterGotData_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PreviewCharacterGotData);} }
	public static void SendNetworkEvent_PropertyMailboxDetails(CPlayer a_Player, Int64 propertyID, EMailboxAccessType accessLevel, List<CItemInstanceDef> inventory) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PropertyMailboxDetails, propertyID, accessLevel, inventory); }
	public static void SendNetworkEvent_PropertyMailboxDetails_ForAll_SpawnedOnly(Int64 propertyID, EMailboxAccessType accessLevel, List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PropertyMailboxDetails, propertyID, accessLevel, inventory);} }
	public static void SendNetworkEvent_PropertyMailboxDetails_ForAll_IncludeEveryone(Int64 propertyID, EMailboxAccessType accessLevel, List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PropertyMailboxDetails, propertyID, accessLevel, inventory);} }
	public static void SendNetworkEvent_Property_CreatePlayerBlip(CPlayer a_Player, Int32 PropertyID, string strName, Vector3 vecPos) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_CreatePlayerBlip, PropertyID, strName, vecPos); }
	public static void SendNetworkEvent_Property_CreatePlayerBlip_ForAll_SpawnedOnly(Int32 PropertyID, string strName, Vector3 vecPos){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_CreatePlayerBlip, PropertyID, strName, vecPos);} }
	public static void SendNetworkEvent_Property_CreatePlayerBlip_ForAll_IncludeEveryone(Int32 PropertyID, string strName, Vector3 vecPos){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_CreatePlayerBlip, PropertyID, strName, vecPos);} }
	public static void SendNetworkEvent_Property_DestroyAllPlayerBlips(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_DestroyAllPlayerBlips); }
	public static void SendNetworkEvent_Property_DestroyAllPlayerBlips_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_DestroyAllPlayerBlips);} }
	public static void SendNetworkEvent_Property_DestroyAllPlayerBlips_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_DestroyAllPlayerBlips);} }
	public static void SendNetworkEvent_Property_DestroyPlayerBlip(CPlayer a_Player, Int32 PropertyID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_DestroyPlayerBlip, PropertyID); }
	public static void SendNetworkEvent_Property_DestroyPlayerBlip_ForAll_SpawnedOnly(Int32 PropertyID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_DestroyPlayerBlip, PropertyID);} }
	public static void SendNetworkEvent_Property_DestroyPlayerBlip_ForAll_IncludeEveryone(Int32 PropertyID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Property_DestroyPlayerBlip, PropertyID);} }
	public static void SendNetworkEvent_PurchaseMethodsResponse(CPlayer a_Player, List<Purchaser> lstPurchasers, List<string> lstMethods) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PurchaseMethodsResponse, lstPurchasers, lstMethods); }
	public static void SendNetworkEvent_PurchaseMethodsResponse_ForAll_SpawnedOnly(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PurchaseMethodsResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_PurchaseMethodsResponse_ForAll_IncludeEveryone(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PurchaseMethodsResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_PurchaseProperty_RequestInfoResponse(CPlayer a_Player, List<Purchaser> lstPurchasers, List<string> lstMethods) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PurchaseProperty_RequestInfoResponse, lstPurchasers, lstMethods); }
	public static void SendNetworkEvent_PurchaseProperty_RequestInfoResponse_ForAll_SpawnedOnly(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PurchaseProperty_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_PurchaseProperty_RequestInfoResponse_ForAll_IncludeEveryone(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.PurchaseProperty_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_QuizResults(CPlayer a_Player, bool bPassed, int numCorrect, int numIncorrect) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.QuizResults, bPassed, numCorrect, numIncorrect); }
	public static void SendNetworkEvent_QuizResults_ForAll_SpawnedOnly(bool bPassed, int numCorrect, int numIncorrect){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.QuizResults, bPassed, numCorrect, numIncorrect);} }
	public static void SendNetworkEvent_QuizResults_ForAll_IncludeEveryone(bool bPassed, int numCorrect, int numIncorrect){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.QuizResults, bPassed, numCorrect, numIncorrect);} }
	public static void SendNetworkEvent_ReadInfoMarker_Response(CPlayer a_Player, Int64 infoMarkerID, bool bIsCreator, string strText) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReadInfoMarker_Response, infoMarkerID, bIsCreator, strText); }
	public static void SendNetworkEvent_ReadInfoMarker_Response_ForAll_SpawnedOnly(Int64 infoMarkerID, bool bIsCreator, string strText){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReadInfoMarker_Response, infoMarkerID, bIsCreator, strText);} }
	public static void SendNetworkEvent_ReadInfoMarker_Response_ForAll_IncludeEveryone(Int64 infoMarkerID, bool bIsCreator, string strText){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReadInfoMarker_Response, infoMarkerID, bIsCreator, strText);} }
	public static void SendNetworkEvent_ReceivedAchievements(CPlayer a_Player, int numAchievements, int maxAchievements, int totalScore, int maxScore, List<AchievementTransmissionObject> lstAchievements) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReceivedAchievements, numAchievements, maxAchievements, totalScore, maxScore, lstAchievements); }
	public static void SendNetworkEvent_ReceivedAchievements_ForAll_SpawnedOnly(int numAchievements, int maxAchievements, int totalScore, int maxScore, List<AchievementTransmissionObject> lstAchievements){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReceivedAchievements, numAchievements, maxAchievements, totalScore, maxScore, lstAchievements);} }
	public static void SendNetworkEvent_ReceivedAchievements_ForAll_IncludeEveryone(int numAchievements, int maxAchievements, int totalScore, int maxScore, List<AchievementTransmissionObject> lstAchievements){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReceivedAchievements, numAchievements, maxAchievements, totalScore, maxScore, lstAchievements);} }
	public static void SendNetworkEvent_ReceivedFactionInvite(CPlayer a_Player, string strFactionName, string strFromPlayerName, Int64 factionID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReceivedFactionInvite, strFactionName, strFromPlayerName, factionID); }
	public static void SendNetworkEvent_ReceivedFactionInvite_ForAll_SpawnedOnly(string strFactionName, string strFromPlayerName, Int64 factionID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReceivedFactionInvite, strFactionName, strFromPlayerName, factionID);} }
	public static void SendNetworkEvent_ReceivedFactionInvite_ForAll_IncludeEveryone(string strFactionName, string strFromPlayerName, Int64 factionID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReceivedFactionInvite, strFactionName, strFromPlayerName, factionID);} }
	public static void SendNetworkEvent_RegisterResult(CPlayer a_Player, bool bSuccess, string error, string[] errors) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RegisterResult, bSuccess, error, errors); }
	public static void SendNetworkEvent_RegisterResult_ForAll_SpawnedOnly(bool bSuccess, string error, string[] errors){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RegisterResult, bSuccess, error, errors);} }
	public static void SendNetworkEvent_RegisterResult_ForAll_IncludeEveryone(bool bSuccess, string error, string[] errors){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RegisterResult, bSuccess, error, errors);} }
	public static void SendNetworkEvent_RemoteCallEnded(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RemoteCallEnded); }
	public static void SendNetworkEvent_RemoteCallEnded_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RemoteCallEnded);} }
	public static void SendNetworkEvent_RemoteCallEnded_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RemoteCallEnded);} }
	public static void SendNetworkEvent_RemoveAsCamOperator(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RemoveAsCamOperator); }
	public static void SendNetworkEvent_RemoveAsCamOperator_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RemoveAsCamOperator);} }
	public static void SendNetworkEvent_RemoveAsCamOperator_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RemoveAsCamOperator);} }
	public static void SendNetworkEvent_RentalShop_CreatePed(CPlayer a_Player, Int64 storeID, Vector3 vecPedPos, float fPedRot, uint pedDimension) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RentalShop_CreatePed, storeID, vecPedPos, fPedRot, pedDimension); }
	public static void SendNetworkEvent_RentalShop_CreatePed_ForAll_SpawnedOnly(Int64 storeID, Vector3 vecPedPos, float fPedRot, uint pedDimension){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RentalShop_CreatePed, storeID, vecPedPos, fPedRot, pedDimension);} }
	public static void SendNetworkEvent_RentalShop_CreatePed_ForAll_IncludeEveryone(Int64 storeID, Vector3 vecPedPos, float fPedRot, uint pedDimension){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RentalShop_CreatePed, storeID, vecPedPos, fPedRot, pedDimension);} }
	public static void SendNetworkEvent_RentalShop_DestroyPed(CPlayer a_Player, Vector3 vecPos, float fRot, uint dimension) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RentalShop_DestroyPed, vecPos, fRot, dimension); }
	public static void SendNetworkEvent_RentalShop_DestroyPed_ForAll_SpawnedOnly(Vector3 vecPos, float fRot, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RentalShop_DestroyPed, vecPos, fRot, dimension);} }
	public static void SendNetworkEvent_RentalShop_DestroyPed_ForAll_IncludeEveryone(Vector3 vecPos, float fRot, uint dimension){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RentalShop_DestroyPed, vecPos, fRot, dimension);} }
	public static void SendNetworkEvent_ReplicateActivityState(CPlayer a_Player, Int64 uniqueActivityIdentifier, EActivityType activityType, string strState) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReplicateActivityState, uniqueActivityIdentifier, activityType, strState); }
	public static void SendNetworkEvent_ReplicateActivityState_ForAll_SpawnedOnly(Int64 uniqueActivityIdentifier, EActivityType activityType, string strState){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReplicateActivityState, uniqueActivityIdentifier, activityType, strState);} }
	public static void SendNetworkEvent_ReplicateActivityState_ForAll_IncludeEveryone(Int64 uniqueActivityIdentifier, EActivityType activityType, string strState){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ReplicateActivityState, uniqueActivityIdentifier, activityType, strState);} }
	public static void SendNetworkEvent_Reports_SendReportData(CPlayer a_Player, List<CPlayerReport> reports) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Reports_SendReportData, reports); }
	public static void SendNetworkEvent_Reports_SendReportData_ForAll_SpawnedOnly(List<CPlayerReport> reports){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Reports_SendReportData, reports);} }
	public static void SendNetworkEvent_Reports_SendReportData_ForAll_IncludeEveryone(List<CPlayerReport> reports){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Reports_SendReportData, reports);} }
	public static void SendNetworkEvent_Reports_UpdateReportData(CPlayer a_Player, List<CPlayerReport> reports) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Reports_UpdateReportData, reports); }
	public static void SendNetworkEvent_Reports_UpdateReportData_ForAll_SpawnedOnly(List<CPlayerReport> reports){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Reports_UpdateReportData, reports);} }
	public static void SendNetworkEvent_Reports_UpdateReportData_ForAll_IncludeEveryone(List<CPlayerReport> reports){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Reports_UpdateReportData, reports);} }
	public static void SendNetworkEvent_RequestBeginChangeBoomboxRadio_Response(CPlayer a_Player, bool bSuccess) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestBeginChangeBoomboxRadio_Response, bSuccess); }
	public static void SendNetworkEvent_RequestBeginChangeBoomboxRadio_Response_ForAll_SpawnedOnly(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestBeginChangeBoomboxRadio_Response, bSuccess);} }
	public static void SendNetworkEvent_RequestBeginChangeBoomboxRadio_Response_ForAll_IncludeEveryone(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestBeginChangeBoomboxRadio_Response, bSuccess);} }
	public static void SendNetworkEvent_RequestDutyOutfitList_Response(CPlayer a_Player, List<CItemInstanceDef> lstOutfits) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestDutyOutfitList_Response, lstOutfits); }
	public static void SendNetworkEvent_RequestDutyOutfitList_Response_ForAll_SpawnedOnly(List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestDutyOutfitList_Response, lstOutfits);} }
	public static void SendNetworkEvent_RequestDutyOutfitList_Response_ForAll_IncludeEveryone(List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestDutyOutfitList_Response, lstOutfits);} }
	public static void SendNetworkEvent_RequestFactionInfo_Response(CPlayer a_Player, List<CFactionTransmit> lstFactions) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestFactionInfo_Response, lstFactions); }
	public static void SendNetworkEvent_RequestFactionInfo_Response_ForAll_SpawnedOnly(List<CFactionTransmit> lstFactions){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestFactionInfo_Response, lstFactions);} }
	public static void SendNetworkEvent_RequestFactionInfo_Response_ForAll_IncludeEveryone(List<CFactionTransmit> lstFactions){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestFactionInfo_Response, lstFactions);} }
	public static void SendNetworkEvent_RequestOutfitList_Response(CPlayer a_Player, List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestOutfitList_Response, lstOutfits, lstClothing); }
	public static void SendNetworkEvent_RequestOutfitList_Response_ForAll_SpawnedOnly(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestOutfitList_Response, lstOutfits, lstClothing);} }
	public static void SendNetworkEvent_RequestOutfitList_Response_ForAll_IncludeEveryone(List<CItemInstanceDef> lstOutfits, List<CItemInstanceDef> lstClothing){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestOutfitList_Response, lstOutfits, lstClothing);} }
	public static void SendNetworkEvent_RequestTicket(CPlayer a_Player, string strOfficerName, float fAmount, string strReason) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestTicket, strOfficerName, fAmount, strReason); }
	public static void SendNetworkEvent_RequestTicket_ForAll_SpawnedOnly(string strOfficerName, float fAmount, string strReason){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestTicket, strOfficerName, fAmount, strReason);} }
	public static void SendNetworkEvent_RequestTicket_ForAll_IncludeEveryone(string strOfficerName, float fAmount, string strReason){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RequestTicket, strOfficerName, fAmount, strReason);} }
	public static void SendNetworkEvent_ResetActivityState(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ResetActivityState); }
	public static void SendNetworkEvent_ResetActivityState_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ResetActivityState);} }
	public static void SendNetworkEvent_ResetActivityState_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ResetActivityState);} }
	public static void SendNetworkEvent_RespawnChar(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RespawnChar); }
	public static void SendNetworkEvent_RespawnChar_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RespawnChar);} }
	public static void SendNetworkEvent_RespawnChar_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RespawnChar);} }
	public static void SendNetworkEvent_RetrievedCharacters(CPlayer a_Player, List<GetCharactersCharacter> lstCharacters, Int64 currentAutoSpawnCharacter) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RetrievedCharacters, lstCharacters, currentAutoSpawnCharacter); }
	public static void SendNetworkEvent_RetrievedCharacters_ForAll_SpawnedOnly(List<GetCharactersCharacter> lstCharacters, Int64 currentAutoSpawnCharacter){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RetrievedCharacters, lstCharacters, currentAutoSpawnCharacter);} }
	public static void SendNetworkEvent_RetrievedCharacters_ForAll_IncludeEveryone(List<GetCharactersCharacter> lstCharacters, Int64 currentAutoSpawnCharacter){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RetrievedCharacters, lstCharacters, currentAutoSpawnCharacter);} }
	public static void SendNetworkEvent_RetuneRadio(CPlayer a_Player, Int64 radioID, int channel) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RetuneRadio, radioID, channel); }
	public static void SendNetworkEvent_RetuneRadio_ForAll_SpawnedOnly(Int64 radioID, int channel){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RetuneRadio, radioID, channel);} }
	public static void SendNetworkEvent_RetuneRadio_ForAll_IncludeEveryone(Int64 radioID, int channel){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.RetuneRadio, radioID, channel);} }
	public static void SendNetworkEvent_SafeTeleport(CPlayer a_Player, float x, float y, float z) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SafeTeleport, x, y, z); }
	public static void SendNetworkEvent_SafeTeleport_ForAll_SpawnedOnly(float x, float y, float z){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SafeTeleport, x, y, z);} }
	public static void SendNetworkEvent_SafeTeleport_ForAll_IncludeEveryone(float x, float y, float z){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SafeTeleport, x, y, z);} }
	public static void SendNetworkEvent_ScriptUpdate(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ScriptUpdate); }
	public static void SendNetworkEvent_ScriptUpdate_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ScriptUpdate);} }
	public static void SendNetworkEvent_ScriptUpdate_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ScriptUpdate);} }
	public static void SendNetworkEvent_SendBreakingNewsMessage(CPlayer a_Player, string BreakingMessage) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SendBreakingNewsMessage, BreakingMessage); }
	public static void SendNetworkEvent_SendBreakingNewsMessage_ForAll_SpawnedOnly(string BreakingMessage){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SendBreakingNewsMessage, BreakingMessage);} }
	public static void SendNetworkEvent_SendBreakingNewsMessage_ForAll_IncludeEveryone(string BreakingMessage){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SendBreakingNewsMessage, BreakingMessage);} }
	public static void SendNetworkEvent_SetDiscordStatus(CPlayer a_Player, string strStatus) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetDiscordStatus, strStatus); }
	public static void SendNetworkEvent_SetDiscordStatus_ForAll_SpawnedOnly(string strStatus){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetDiscordStatus, strStatus);} }
	public static void SendNetworkEvent_SetDiscordStatus_ForAll_IncludeEveryone(string strStatus){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetDiscordStatus, strStatus);} }
	public static void SendNetworkEvent_SetKeybindsDisabled(CPlayer a_Player, bool a_bKeybindsDisabled) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetKeybindsDisabled, a_bKeybindsDisabled); }
	public static void SendNetworkEvent_SetKeybindsDisabled_ForAll_SpawnedOnly(bool a_bKeybindsDisabled){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetKeybindsDisabled, a_bKeybindsDisabled);} }
	public static void SendNetworkEvent_SetKeybindsDisabled_ForAll_IncludeEveryone(bool a_bKeybindsDisabled){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetKeybindsDisabled, a_bKeybindsDisabled);} }
	public static void SendNetworkEvent_SetLoadingWorld(CPlayer a_Player, bool bLoadingWorld) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetLoadingWorld, bLoadingWorld); }
	public static void SendNetworkEvent_SetLoadingWorld_ForAll_SpawnedOnly(bool bLoadingWorld){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetLoadingWorld, bLoadingWorld);} }
	public static void SendNetworkEvent_SetLoadingWorld_ForAll_IncludeEveryone(bool bLoadingWorld){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetLoadingWorld, bLoadingWorld);} }
	public static void SendNetworkEvent_SetPetName(CPlayer a_Player, EPetType a_Type, Int64 petID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetPetName, a_Type, petID); }
	public static void SendNetworkEvent_SetPetName_ForAll_SpawnedOnly(EPetType a_Type, Int64 petID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetPetName, a_Type, petID);} }
	public static void SendNetworkEvent_SetPetName_ForAll_IncludeEveryone(EPetType a_Type, Int64 petID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetPetName, a_Type, petID);} }
	public static void SendNetworkEvent_SetPlayerVisible(CPlayer a_Player, PlayerType targetPlayer, bool bVisible) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetPlayerVisible, targetPlayer, bVisible); }
	public static void SendNetworkEvent_SetPlayerVisible_ForAll_SpawnedOnly(PlayerType targetPlayer, bool bVisible){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetPlayerVisible, targetPlayer, bVisible);} }
	public static void SendNetworkEvent_SetPlayerVisible_ForAll_IncludeEveryone(PlayerType targetPlayer, bool bVisible){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SetPlayerVisible, targetPlayer, bVisible);} }
	public static void SendNetworkEvent_ShowAnimationList(CPlayer a_Player, List<CAnimationCommand> animCmdList) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowAnimationList, animCmdList); }
	public static void SendNetworkEvent_ShowAnimationList_ForAll_SpawnedOnly(List<CAnimationCommand> animCmdList){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowAnimationList, animCmdList);} }
	public static void SendNetworkEvent_ShowAnimationList_ForAll_IncludeEveryone(List<CAnimationCommand> animCmdList){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowAnimationList, animCmdList);} }
	public static void SendNetworkEvent_ShowCharacterLook(CPlayer a_Player, Int64 characterID, string characterName, Int32 age, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowCharacterLook, characterID, characterName, age, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt); }
	public static void SendNetworkEvent_ShowCharacterLook_ForAll_SpawnedOnly(Int64 characterID, string characterName, Int32 age, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowCharacterLook, characterID, characterName, age, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);} }
	public static void SendNetworkEvent_ShowCharacterLook_ForAll_IncludeEveryone(Int64 characterID, string characterName, Int32 age, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowCharacterLook, characterID, characterName, age, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);} }
	public static void SendNetworkEvent_ShowGenericMessageBox(CPlayer a_Player, string strTitle, string strCaption) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowGenericMessageBox, strTitle, strCaption); }
	public static void SendNetworkEvent_ShowGenericMessageBox_ForAll_SpawnedOnly(string strTitle, string strCaption){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowGenericMessageBox, strTitle, strCaption);} }
	public static void SendNetworkEvent_ShowGenericMessageBox_ForAll_IncludeEveryone(string strTitle, string strCaption){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowGenericMessageBox, strTitle, strCaption);} }
	public static void SendNetworkEvent_ShowHelpCenter(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowHelpCenter); }
	public static void SendNetworkEvent_ShowHelpCenter_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowHelpCenter);} }
	public static void SendNetworkEvent_ShowHelpCenter_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowHelpCenter);} }
	public static void SendNetworkEvent_ShowItemsList(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowItemsList); }
	public static void SendNetworkEvent_ShowItemsList_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowItemsList);} }
	public static void SendNetworkEvent_ShowItemsList_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowItemsList);} }
	public static void SendNetworkEvent_ShowKeyBindManager(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowKeyBindManager); }
	public static void SendNetworkEvent_ShowKeyBindManager_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowKeyBindManager);} }
	public static void SendNetworkEvent_ShowKeyBindManager_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowKeyBindManager);} }
	public static void SendNetworkEvent_ShowLanguages(CPlayer a_Player, List<CCharacterLanguageTransmit> lstCharacterLanguages) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowLanguages, lstCharacterLanguages); }
	public static void SendNetworkEvent_ShowLanguages_ForAll_SpawnedOnly(List<CCharacterLanguageTransmit> lstCharacterLanguages){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowLanguages, lstCharacterLanguages);} }
	public static void SendNetworkEvent_ShowLanguages_ForAll_IncludeEveryone(List<CCharacterLanguageTransmit> lstCharacterLanguages){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowLanguages, lstCharacterLanguages);} }
	public static void SendNetworkEvent_ShowMobileBankUI(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowMobileBankUI); }
	public static void SendNetworkEvent_ShowMobileBankUI_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowMobileBankUI);} }
	public static void SendNetworkEvent_ShowMobileBankUI_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowMobileBankUI);} }
	public static void SendNetworkEvent_ShowPayDayOverview(CPlayer a_Player, PayDayDetails paydayDetails) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowPayDayOverview, paydayDetails); }
	public static void SendNetworkEvent_ShowPayDayOverview_ForAll_SpawnedOnly(PayDayDetails paydayDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowPayDayOverview, paydayDetails);} }
	public static void SendNetworkEvent_ShowPayDayOverview_ForAll_IncludeEveryone(PayDayDetails paydayDetails){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowPayDayOverview, paydayDetails);} }
	public static void SendNetworkEvent_ShowSpawnSelector(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowSpawnSelector); }
	public static void SendNetworkEvent_ShowSpawnSelector_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowSpawnSelector);} }
	public static void SendNetworkEvent_ShowSpawnSelector_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowSpawnSelector);} }
	public static void SendNetworkEvent_ShowUpdateCharacterLookUI(CPlayer a_Player, Int64 characterID, string characterName, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowUpdateCharacterLookUI, characterID, characterName, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt); }
	public static void SendNetworkEvent_ShowUpdateCharacterLookUI_ForAll_SpawnedOnly(Int64 characterID, string characterName, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowUpdateCharacterLookUI, characterID, characterName, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);} }
	public static void SendNetworkEvent_ShowUpdateCharacterLookUI_ForAll_IncludeEveryone(Int64 characterID, string characterName, Int32 height, Int32 weight, string physicalAppearance, string scars, string tattoos, string makeup, Int32 createdAt, Int32 updatedAt){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowUpdateCharacterLookUI, characterID, characterName, height, weight, physicalAppearance, scars, tattoos, makeup, createdAt, updatedAt);} }
	public static void SendNetworkEvent_ShowVehiclesList(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowVehiclesList); }
	public static void SendNetworkEvent_ShowVehiclesList_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowVehiclesList);} }
	public static void SendNetworkEvent_ShowVehiclesList_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ShowVehiclesList);} }
	public static void SendNetworkEvent_SpeedCameraTrigger_Response(CPlayer a_Player, float speed, int cameraID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SpeedCameraTrigger_Response, speed, cameraID); }
	public static void SendNetworkEvent_SpeedCameraTrigger_Response_ForAll_SpawnedOnly(float speed, int cameraID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SpeedCameraTrigger_Response, speed, cameraID);} }
	public static void SendNetworkEvent_SpeedCameraTrigger_Response_ForAll_IncludeEveryone(float speed, int cameraID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SpeedCameraTrigger_Response, speed, cameraID);} }
	public static void SendNetworkEvent_StartActivityApproved(CPlayer a_Player, int participantIndex, Int64 uniqueIdentifier, EActivityType activityType) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartActivityApproved, participantIndex, uniqueIdentifier, activityType); }
	public static void SendNetworkEvent_StartActivityApproved_ForAll_SpawnedOnly(int participantIndex, Int64 uniqueIdentifier, EActivityType activityType){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartActivityApproved, participantIndex, uniqueIdentifier, activityType);} }
	public static void SendNetworkEvent_StartActivityApproved_ForAll_IncludeEveryone(int participantIndex, Int64 uniqueIdentifier, EActivityType activityType){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartActivityApproved, participantIndex, uniqueIdentifier, activityType);} }
	public static void SendNetworkEvent_StartDeathEffect(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDeathEffect); }
	public static void SendNetworkEvent_StartDeathEffect_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDeathEffect);} }
	public static void SendNetworkEvent_StartDeathEffect_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDeathEffect);} }
	public static void SendNetworkEvent_StartDrivingTest(CPlayer a_Player, EDrivingTestType testType, bool a_bIsResume) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDrivingTest, testType, a_bIsResume); }
	public static void SendNetworkEvent_StartDrivingTest_ForAll_SpawnedOnly(EDrivingTestType testType, bool a_bIsResume){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDrivingTest, testType, a_bIsResume);} }
	public static void SendNetworkEvent_StartDrivingTest_ForAll_IncludeEveryone(EDrivingTestType testType, bool a_bIsResume){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDrivingTest, testType, a_bIsResume);} }
	public static void SendNetworkEvent_StartDrivingTest_Rejected(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDrivingTest_Rejected); }
	public static void SendNetworkEvent_StartDrivingTest_Rejected_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDrivingTest_Rejected);} }
	public static void SendNetworkEvent_StartDrivingTest_Rejected_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartDrivingTest_Rejected);} }
	public static void SendNetworkEvent_StartFireMission(CPlayer a_Player, EFireMissionID MissionID, EFireType FireType, Vector3 vecPos, bool bIsParticipatingInMission, string strTitle) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFireMission, MissionID, FireType, vecPos, bIsParticipatingInMission, strTitle); }
	public static void SendNetworkEvent_StartFireMission_ForAll_SpawnedOnly(EFireMissionID MissionID, EFireType FireType, Vector3 vecPos, bool bIsParticipatingInMission, string strTitle){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFireMission, MissionID, FireType, vecPos, bIsParticipatingInMission, strTitle);} }
	public static void SendNetworkEvent_StartFireMission_ForAll_IncludeEveryone(EFireMissionID MissionID, EFireType FireType, Vector3 vecPos, bool bIsParticipatingInMission, string strTitle){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFireMission, MissionID, FireType, vecPos, bIsParticipatingInMission, strTitle);} }
	public static void SendNetworkEvent_StartFishing(CPlayer a_Player, int currentLevel, int xpRequiredForNextLevel) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFishing, currentLevel, xpRequiredForNextLevel); }
	public static void SendNetworkEvent_StartFishing_ForAll_SpawnedOnly(int currentLevel, int xpRequiredForNextLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFishing, currentLevel, xpRequiredForNextLevel);} }
	public static void SendNetworkEvent_StartFishing_ForAll_IncludeEveryone(int currentLevel, int xpRequiredForNextLevel){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFishing, currentLevel, xpRequiredForNextLevel);} }
	public static void SendNetworkEvent_StartFourthOfJuly(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFourthOfJuly); }
	public static void SendNetworkEvent_StartFourthOfJuly_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFourthOfJuly);} }
	public static void SendNetworkEvent_StartFourthOfJuly_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFourthOfJuly);} }
	public static void SendNetworkEvent_StartFourthOfJulyFireworksOnly(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFourthOfJulyFireworksOnly); }
	public static void SendNetworkEvent_StartFourthOfJulyFireworksOnly_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFourthOfJulyFireworksOnly);} }
	public static void SendNetworkEvent_StartFourthOfJulyFireworksOnly_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartFourthOfJulyFireworksOnly);} }
	public static void SendNetworkEvent_StartJob(CPlayer a_Player, EJobID a_JobID, bool a_bIsResume) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartJob, a_JobID, a_bIsResume); }
	public static void SendNetworkEvent_StartJob_ForAll_SpawnedOnly(EJobID a_JobID, bool a_bIsResume){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartJob, a_JobID, a_bIsResume);} }
	public static void SendNetworkEvent_StartJob_ForAll_IncludeEveryone(EJobID a_JobID, bool a_bIsResume){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartJob, a_JobID, a_bIsResume);} }
	public static void SendNetworkEvent_StartPerformanceCapture(CPlayer a_Player, int duration) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartPerformanceCapture, duration); }
	public static void SendNetworkEvent_StartPerformanceCapture_ForAll_SpawnedOnly(int duration){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartPerformanceCapture, duration);} }
	public static void SendNetworkEvent_StartPerformanceCapture_ForAll_IncludeEveryone(int duration){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartPerformanceCapture, duration);} }
	public static void SendNetworkEvent_StartRecon(CPlayer a_Player, PlayerType reconTarget) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartRecon, reconTarget); }
	public static void SendNetworkEvent_StartRecon_ForAll_SpawnedOnly(PlayerType reconTarget){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartRecon, reconTarget);} }
	public static void SendNetworkEvent_StartRecon_ForAll_IncludeEveryone(PlayerType reconTarget){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartRecon, reconTarget);} }
	public static void SendNetworkEvent_StartTagCleaningResponse(CPlayer a_Player, bool bApproved) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartTagCleaningResponse, bApproved); }
	public static void SendNetworkEvent_StartTagCleaningResponse_ForAll_SpawnedOnly(bool bApproved){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartTagCleaningResponse, bApproved);} }
	public static void SendNetworkEvent_StartTagCleaningResponse_ForAll_IncludeEveryone(bool bApproved){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartTagCleaningResponse, bApproved);} }
	public static void SendNetworkEvent_StartTaggingResponse(CPlayer a_Player, bool bApproved, Int64 tagID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartTaggingResponse, bApproved, tagID); }
	public static void SendNetworkEvent_StartTaggingResponse_ForAll_SpawnedOnly(bool bApproved, Int64 tagID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartTaggingResponse, bApproved, tagID);} }
	public static void SendNetworkEvent_StartTaggingResponse_ForAll_IncludeEveryone(bool bApproved, Int64 tagID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StartTaggingResponse, bApproved, tagID);} }
	public static void SendNetworkEvent_StopDrivingTest(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopDrivingTest); }
	public static void SendNetworkEvent_StopDrivingTest_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopDrivingTest);} }
	public static void SendNetworkEvent_StopDrivingTest_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopDrivingTest);} }
	public static void SendNetworkEvent_StopJob(CPlayer a_Player, EJobID a_JobID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopJob, a_JobID); }
	public static void SendNetworkEvent_StopJob_ForAll_SpawnedOnly(EJobID a_JobID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopJob, a_JobID);} }
	public static void SendNetworkEvent_StopJob_ForAll_IncludeEveryone(EJobID a_JobID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopJob, a_JobID);} }
	public static void SendNetworkEvent_StopRecon(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopRecon); }
	public static void SendNetworkEvent_StopRecon_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopRecon);} }
	public static void SendNetworkEvent_StopRecon_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.StopRecon);} }
	public static void SendNetworkEvent_Store_OnRobberyFinished(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Store_OnRobberyFinished); }
	public static void SendNetworkEvent_Store_OnRobberyFinished_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Store_OnRobberyFinished);} }
	public static void SendNetworkEvent_Store_OnRobberyFinished_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.Store_OnRobberyFinished);} }
	public static void SendNetworkEvent_SyncAllRadios(CPlayer a_Player, List<RadioInstance> lstRadios) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncAllRadios, lstRadios); }
	public static void SendNetworkEvent_SyncAllRadios_ForAll_SpawnedOnly(List<RadioInstance> lstRadios){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncAllRadios, lstRadios);} }
	public static void SendNetworkEvent_SyncAllRadios_ForAll_IncludeEveryone(List<RadioInstance> lstRadios){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncAllRadios, lstRadios);} }
	public static void SendNetworkEvent_SyncSingleRadio(CPlayer a_Player, RadioInstance radioInst) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncSingleRadio, radioInst); }
	public static void SendNetworkEvent_SyncSingleRadio_ForAll_SpawnedOnly(RadioInstance radioInst){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncSingleRadio, radioInst);} }
	public static void SendNetworkEvent_SyncSingleRadio_ForAll_IncludeEveryone(RadioInstance radioInst){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncSingleRadio, radioInst);} }
	public static void SendNetworkEvent_SyncVehicleHandbrakeSound(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncVehicleHandbrakeSound); }
	public static void SendNetworkEvent_SyncVehicleHandbrakeSound_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncVehicleHandbrakeSound);} }
	public static void SendNetworkEvent_SyncVehicleHandbrakeSound_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.SyncVehicleHandbrakeSound);} }
	public static void SendNetworkEvent_TagCleaningComplete(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TagCleaningComplete); }
	public static void SendNetworkEvent_TagCleaningComplete_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TagCleaningComplete);} }
	public static void SendNetworkEvent_TagCleaningComplete_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TagCleaningComplete);} }
	public static void SendNetworkEvent_TakeAIOwnership(CPlayer a_Player, int number) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TakeAIOwnership, number); }
	public static void SendNetworkEvent_TakeAIOwnership_ForAll_SpawnedOnly(int number){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TakeAIOwnership, number);} }
	public static void SendNetworkEvent_TakeAIOwnership_ForAll_IncludeEveryone(int number){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TakeAIOwnership, number);} }
	public static void SendNetworkEvent_TattooArtist_GotPrice(CPlayer a_Player, float fPrice, bool hasToken, uint numAdded, uint numRemoved) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TattooArtist_GotPrice, fPrice, hasToken, numAdded, numRemoved); }
	public static void SendNetworkEvent_TattooArtist_GotPrice_ForAll_SpawnedOnly(float fPrice, bool hasToken, uint numAdded, uint numRemoved){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TattooArtist_GotPrice, fPrice, hasToken, numAdded, numRemoved);} }
	public static void SendNetworkEvent_TattooArtist_GotPrice_ForAll_IncludeEveryone(float fPrice, bool hasToken, uint numAdded, uint numRemoved){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TattooArtist_GotPrice, fPrice, hasToken, numAdded, numRemoved);} }
	public static void SendNetworkEvent_TaxiAccepted(CPlayer a_Player, Vector3 vecPickupPos) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TaxiAccepted, vecPickupPos); }
	public static void SendNetworkEvent_TaxiAccepted_ForAll_SpawnedOnly(Vector3 vecPickupPos){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TaxiAccepted, vecPickupPos);} }
	public static void SendNetworkEvent_TaxiAccepted_ForAll_IncludeEveryone(Vector3 vecPickupPos){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TaxiAccepted, vecPickupPos);} }
	public static void SendNetworkEvent_TaxiCleanup(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TaxiCleanup); }
	public static void SendNetworkEvent_TaxiCleanup_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TaxiCleanup);} }
	public static void SendNetworkEvent_TaxiCleanup_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TaxiCleanup);} }
	public static void SendNetworkEvent_ToggleClientSideDebugOption(CPlayer a_Player, EClientsideDebugOption clientsideDebugOption) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleClientSideDebugOption, clientsideDebugOption); }
	public static void SendNetworkEvent_ToggleClientSideDebugOption_ForAll_SpawnedOnly(EClientsideDebugOption clientsideDebugOption){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleClientSideDebugOption, clientsideDebugOption);} }
	public static void SendNetworkEvent_ToggleClientSideDebugOption_ForAll_IncludeEveryone(EClientsideDebugOption clientsideDebugOption){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleClientSideDebugOption, clientsideDebugOption);} }
	public static void SendNetworkEvent_ToggleDebugSpam(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleDebugSpam); }
	public static void SendNetworkEvent_ToggleDebugSpam_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleDebugSpam);} }
	public static void SendNetworkEvent_ToggleDebugSpam_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleDebugSpam);} }
	public static void SendNetworkEvent_ToggleSeatbelt(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleSeatbelt); }
	public static void SendNetworkEvent_ToggleSeatbelt_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleSeatbelt);} }
	public static void SendNetworkEvent_ToggleSeatbelt_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.ToggleSeatbelt);} }
	public static void SendNetworkEvent_TowedVehicleList_Response(CPlayer a_Player, List<Int64> lstVehicles) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TowedVehicleList_Response, lstVehicles); }
	public static void SendNetworkEvent_TowedVehicleList_Response_ForAll_SpawnedOnly(List<Int64> lstVehicles){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TowedVehicleList_Response, lstVehicles);} }
	public static void SendNetworkEvent_TowedVehicleList_Response_ForAll_IncludeEveryone(List<Int64> lstVehicles){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TowedVehicleList_Response, lstVehicles);} }
	public static void SendNetworkEvent_TrainDoorStateChanged(CPlayer a_Player, int ID, bool bDoorsOpen) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainDoorStateChanged, ID, bDoorsOpen); }
	public static void SendNetworkEvent_TrainDoorStateChanged_ForAll_SpawnedOnly(int ID, bool bDoorsOpen){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainDoorStateChanged, ID, bDoorsOpen);} }
	public static void SendNetworkEvent_TrainDoorStateChanged_ForAll_IncludeEveryone(int ID, bool bDoorsOpen){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainDoorStateChanged, ID, bDoorsOpen);} }
	public static void SendNetworkEvent_TrainEnter_Approved(CPlayer a_Player, PlayerType player, int ID, EVehicleSeat Seat) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainEnter_Approved, player, ID, Seat); }
	public static void SendNetworkEvent_TrainEnter_Approved_ForAll_SpawnedOnly(PlayerType player, int ID, EVehicleSeat Seat){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainEnter_Approved, player, ID, Seat);} }
	public static void SendNetworkEvent_TrainEnter_Approved_ForAll_IncludeEveryone(PlayerType player, int ID, EVehicleSeat Seat){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainEnter_Approved, player, ID, Seat);} }
	public static void SendNetworkEvent_TrainExit_Approved(CPlayer a_Player, PlayerType player, int ID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainExit_Approved, player, ID); }
	public static void SendNetworkEvent_TrainExit_Approved_ForAll_SpawnedOnly(PlayerType player, int ID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainExit_Approved, player, ID);} }
	public static void SendNetworkEvent_TrainExit_Approved_ForAll_IncludeEveryone(PlayerType player, int ID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainExit_Approved, player, ID);} }
	public static void SendNetworkEvent_TrainSync(CPlayer a_Player, int ID, float x, float y, float z, float speed, int tripwireID, int currentSector) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync, ID, x, y, z, speed, tripwireID, currentSector); }
	public static void SendNetworkEvent_TrainSync_ForAll_SpawnedOnly(int ID, float x, float y, float z, float speed, int tripwireID, int currentSector){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync, ID, x, y, z, speed, tripwireID, currentSector);} }
	public static void SendNetworkEvent_TrainSync_ForAll_IncludeEveryone(int ID, float x, float y, float z, float speed, int tripwireID, int currentSector){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync, ID, x, y, z, speed, tripwireID, currentSector);} }
	public static void SendNetworkEvent_TrainSync_Ack(CPlayer a_Player, int ID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_Ack, ID); }
	public static void SendNetworkEvent_TrainSync_Ack_ForAll_SpawnedOnly(int ID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_Ack, ID);} }
	public static void SendNetworkEvent_TrainSync_Ack_ForAll_IncludeEveryone(int ID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_Ack, ID);} }
	public static void SendNetworkEvent_TrainSync_GiveOwnership(CPlayer a_Player, int id) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_GiveOwnership, id); }
	public static void SendNetworkEvent_TrainSync_GiveOwnership_ForAll_SpawnedOnly(int id){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_GiveOwnership, id);} }
	public static void SendNetworkEvent_TrainSync_GiveOwnership_ForAll_IncludeEveryone(int id){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_GiveOwnership, id);} }
	public static void SendNetworkEvent_TrainSync_TakeOwnership(CPlayer a_Player, int id) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_TakeOwnership, id); }
	public static void SendNetworkEvent_TrainSync_TakeOwnership_ForAll_SpawnedOnly(int id){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_TakeOwnership, id);} }
	public static void SendNetworkEvent_TrainSync_TakeOwnership_ForAll_IncludeEveryone(int id){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.TrainSync_TakeOwnership, id);} }
	public static void SendNetworkEvent_UnloadCustomMap(CPlayer a_Player, int mapID) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UnloadCustomMap, mapID); }
	public static void SendNetworkEvent_UnloadCustomMap_ForAll_SpawnedOnly(int mapID){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UnloadCustomMap, mapID);} }
	public static void SendNetworkEvent_UnloadCustomMap_ForAll_IncludeEveryone(int mapID){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UnloadCustomMap, mapID);} }
	public static void SendNetworkEvent_UpdateFireMission(CPlayer a_Player, List<int> lstSlotsToReIgnite) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateFireMission, lstSlotsToReIgnite); }
	public static void SendNetworkEvent_UpdateFireMission_ForAll_SpawnedOnly(List<int> lstSlotsToReIgnite){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateFireMission, lstSlotsToReIgnite);} }
	public static void SendNetworkEvent_UpdateFireMission_ForAll_IncludeEveryone(List<int> lstSlotsToReIgnite){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateFireMission, lstSlotsToReIgnite);} }
	public static void SendNetworkEvent_UpdateFurnitureCache(CPlayer a_Player, long propertyID, List<CPropertyFurnitureInstance> lstFurniture, List<CPropertyDefaultFurnitureRemovalInstance> lstRemovals) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateFurnitureCache, propertyID, lstFurniture, lstRemovals); }
	public static void SendNetworkEvent_UpdateFurnitureCache_ForAll_SpawnedOnly(long propertyID, List<CPropertyFurnitureInstance> lstFurniture, List<CPropertyDefaultFurnitureRemovalInstance> lstRemovals){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateFurnitureCache, propertyID, lstFurniture, lstRemovals);} }
	public static void SendNetworkEvent_UpdateFurnitureCache_ForAll_IncludeEveryone(long propertyID, List<CPropertyFurnitureInstance> lstFurniture, List<CPropertyDefaultFurnitureRemovalInstance> lstRemovals){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateFurnitureCache, propertyID, lstFurniture, lstRemovals);} }
	public static void SendNetworkEvent_UpdateGangTagLayers(CPlayer a_Player, Int64 a_ID, List<GangTagLayer> lstLayers) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateGangTagLayers, a_ID, lstLayers); }
	public static void SendNetworkEvent_UpdateGangTagLayers_ForAll_SpawnedOnly(Int64 a_ID, List<GangTagLayer> lstLayers){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateGangTagLayers, a_ID, lstLayers);} }
	public static void SendNetworkEvent_UpdateGangTagLayers_ForAll_IncludeEveryone(Int64 a_ID, List<GangTagLayer> lstLayers){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateGangTagLayers, a_ID, lstLayers);} }
	public static void SendNetworkEvent_UpdateGangTagProgress(CPlayer a_Player, Int64 a_ID, float fProgress) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateGangTagProgress, a_ID, fProgress); }
	public static void SendNetworkEvent_UpdateGangTagProgress_ForAll_SpawnedOnly(Int64 a_ID, float fProgress){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateGangTagProgress, a_ID, fProgress);} }
	public static void SendNetworkEvent_UpdateGangTagProgress_ForAll_IncludeEveryone(Int64 a_ID, float fProgress){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateGangTagProgress, a_ID, fProgress);} }
	public static void SendNetworkEvent_UpdateWeatherState(CPlayer a_Player, int weatherValue) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateWeatherState, weatherValue); }
	public static void SendNetworkEvent_UpdateWeatherState_ForAll_SpawnedOnly(int weatherValue){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateWeatherState, weatherValue);} }
	public static void SendNetworkEvent_UpdateWeatherState_ForAll_IncludeEveryone(int weatherValue){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UpdateWeatherState, weatherValue);} }
	public static void SendNetworkEvent_UseCellphone(CPlayer a_Player, bool bHasExistingTaxiRequest, bool isCalled, Int64 number) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseCellphone, bHasExistingTaxiRequest, isCalled, number); }
	public static void SendNetworkEvent_UseCellphone_ForAll_SpawnedOnly(bool bHasExistingTaxiRequest, bool isCalled, Int64 number){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseCellphone, bHasExistingTaxiRequest, isCalled, number);} }
	public static void SendNetworkEvent_UseCellphone_ForAll_IncludeEveryone(bool bHasExistingTaxiRequest, bool isCalled, Int64 number){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseCellphone, bHasExistingTaxiRequest, isCalled, number);} }
	public static void SendNetworkEvent_UseDutyPointResult(CPlayer a_Player, bool bSuccess, EDutyType a_Type, List<CItemInstanceDef> lstOutfits) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseDutyPointResult, bSuccess, a_Type, lstOutfits); }
	public static void SendNetworkEvent_UseDutyPointResult_ForAll_SpawnedOnly(bool bSuccess, EDutyType a_Type, List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseDutyPointResult, bSuccess, a_Type, lstOutfits);} }
	public static void SendNetworkEvent_UseDutyPointResult_ForAll_IncludeEveryone(bool bSuccess, EDutyType a_Type, List<CItemInstanceDef> lstOutfits){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseDutyPointResult, bSuccess, a_Type, lstOutfits);} }
	public static void SendNetworkEvent_UseFirearmsLicensingDevice(CPlayer a_Player, bool isRemoval) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseFirearmsLicensingDevice, isRemoval); }
	public static void SendNetworkEvent_UseFirearmsLicensingDevice_ForAll_SpawnedOnly(bool isRemoval){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseFirearmsLicensingDevice, isRemoval);} }
	public static void SendNetworkEvent_UseFirearmsLicensingDevice_ForAll_IncludeEveryone(bool isRemoval){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseFirearmsLicensingDevice, isRemoval);} }
	public static void SendNetworkEvent_UseSprayCan(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseSprayCan); }
	public static void SendNetworkEvent_UseSprayCan_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseSprayCan);} }
	public static void SendNetworkEvent_UseSprayCan_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.UseSprayCan);} }
	public static void SendNetworkEvent_VehicleCrusher_ShowCrushInterface(CPlayer a_Player, float amount, bool token, string name) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleCrusher_ShowCrushInterface, amount, token, name); }
	public static void SendNetworkEvent_VehicleCrusher_ShowCrushInterface_ForAll_SpawnedOnly(float amount, bool token, string name){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleCrusher_ShowCrushInterface, amount, token, name);} }
	public static void SendNetworkEvent_VehicleCrusher_ShowCrushInterface_ForAll_IncludeEveryone(float amount, bool token, string name){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleCrusher_ShowCrushInterface, amount, token, name);} }
	public static void SendNetworkEvent_VehicleInventoryDetails(CPlayer a_Player, EVehicleInventoryType inventoryType, List<CItemInstanceDef> inventory) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleInventoryDetails, inventoryType, inventory); }
	public static void SendNetworkEvent_VehicleInventoryDetails_ForAll_SpawnedOnly(EVehicleInventoryType inventoryType, List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleInventoryDetails, inventoryType, inventory);} }
	public static void SendNetworkEvent_VehicleInventoryDetails_ForAll_IncludeEveryone(EVehicleInventoryType inventoryType, List<CItemInstanceDef> inventory){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleInventoryDetails, inventoryType, inventory);} }
	public static void SendNetworkEvent_VehicleModShop_GotModPrice(CPlayer a_Player, float fPrice, int GCPrice) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_GotModPrice, fPrice, GCPrice); }
	public static void SendNetworkEvent_VehicleModShop_GotModPrice_ForAll_SpawnedOnly(float fPrice, int GCPrice){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_GotModPrice, fPrice, GCPrice);} }
	public static void SendNetworkEvent_VehicleModShop_GotModPrice_ForAll_IncludeEveryone(float fPrice, int GCPrice){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_GotModPrice, fPrice, GCPrice);} }
	public static void SendNetworkEvent_VehicleModShop_GotPrice(CPlayer a_Player, float fPrice, int GCPrice, Dictionary<EModSlot, string> dictOverviewPrices) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_GotPrice, fPrice, GCPrice, dictOverviewPrices); }
	public static void SendNetworkEvent_VehicleModShop_GotPrice_ForAll_SpawnedOnly(float fPrice, int GCPrice, Dictionary<EModSlot, string> dictOverviewPrices){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_GotPrice, fPrice, GCPrice, dictOverviewPrices);} }
	public static void SendNetworkEvent_VehicleModShop_GotPrice_ForAll_IncludeEveryone(float fPrice, int GCPrice, Dictionary<EModSlot, string> dictOverviewPrices){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_GotPrice, fPrice, GCPrice, dictOverviewPrices);} }
	public static void SendNetworkEvent_VehicleModShop_OnCheckout_Response(CPlayer a_Player, EVehicleModShopCheckoutResult result) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_OnCheckout_Response, result); }
	public static void SendNetworkEvent_VehicleModShop_OnCheckout_Response_ForAll_SpawnedOnly(EVehicleModShopCheckoutResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_OnCheckout_Response, result);} }
	public static void SendNetworkEvent_VehicleModShop_OnCheckout_Response_ForAll_IncludeEveryone(EVehicleModShopCheckoutResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleModShop_OnCheckout_Response, result);} }
	public static void SendNetworkEvent_VehicleRentalStore_OnCheckoutResult(CPlayer a_Player, ERentVehicleResult result) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRentalStore_OnCheckoutResult, result); }
	public static void SendNetworkEvent_VehicleRentalStore_OnCheckoutResult_ForAll_SpawnedOnly(ERentVehicleResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRentalStore_OnCheckoutResult, result);} }
	public static void SendNetworkEvent_VehicleRentalStore_OnCheckoutResult_ForAll_IncludeEveryone(ERentVehicleResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRentalStore_OnCheckoutResult, result);} }
	public static void SendNetworkEvent_VehicleRentalStore_RequestInfoResponse(CPlayer a_Player, List<Purchaser> lstPurchasers, List<string> lstMethods) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRentalStore_RequestInfoResponse, lstPurchasers, lstMethods); }
	public static void SendNetworkEvent_VehicleRentalStore_RequestInfoResponse_ForAll_SpawnedOnly(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRentalStore_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_VehicleRentalStore_RequestInfoResponse_ForAll_IncludeEveryone(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRentalStore_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_VehicleRepairComplete(CPlayer a_Player) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRepairComplete); }
	public static void SendNetworkEvent_VehicleRepairComplete_ForAll_SpawnedOnly(){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRepairComplete);} }
	public static void SendNetworkEvent_VehicleRepairComplete_ForAll_IncludeEveryone(){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRepairComplete);} }
	public static void SendNetworkEvent_VehicleRepairRequestResponse(CPlayer a_Player, bool bSuccess) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRepairRequestResponse, bSuccess); }
	public static void SendNetworkEvent_VehicleRepairRequestResponse_ForAll_SpawnedOnly(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRepairRequestResponse, bSuccess);} }
	public static void SendNetworkEvent_VehicleRepairRequestResponse_ForAll_IncludeEveryone(bool bSuccess){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleRepairRequestResponse, bSuccess);} }
	public static void SendNetworkEvent_VehicleStore_OnCheckoutResult(CPlayer a_Player, EPurchaseVehicleResult result) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleStore_OnCheckoutResult, result); }
	public static void SendNetworkEvent_VehicleStore_OnCheckoutResult_ForAll_SpawnedOnly(EPurchaseVehicleResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleStore_OnCheckoutResult, result);} }
	public static void SendNetworkEvent_VehicleStore_OnCheckoutResult_ForAll_IncludeEveryone(EPurchaseVehicleResult result){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleStore_OnCheckoutResult, result);} }
	public static void SendNetworkEvent_VehicleStore_RequestInfoResponse(CPlayer a_Player, List<Purchaser> lstPurchasers, List<string> lstMethods) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleStore_RequestInfoResponse, lstPurchasers, lstMethods); }
	public static void SendNetworkEvent_VehicleStore_RequestInfoResponse_ForAll_SpawnedOnly(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleStore_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_VehicleStore_RequestInfoResponse_ForAll_IncludeEveryone(List<Purchaser> lstPurchasers, List<string> lstMethods){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.VehicleStore_RequestInfoResponse, lstPurchasers, lstMethods);} }
	public static void SendNetworkEvent_WeatherInfo(CPlayer a_Player, string strWeatherMain, string strWeatherDescription, float weatherTemp, float weatherTempFeelsLike, Int32 weatherHumidity, float weatherWindSpeed) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.WeatherInfo, strWeatherMain, strWeatherDescription, weatherTemp, weatherTempFeelsLike, weatherHumidity, weatherWindSpeed); }
	public static void SendNetworkEvent_WeatherInfo_ForAll_SpawnedOnly(string strWeatherMain, string strWeatherDescription, float weatherTemp, float weatherTempFeelsLike, Int32 weatherHumidity, float weatherWindSpeed){ foreach (var a_Player in PlayerPool.GetAllPlayers()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.WeatherInfo, strWeatherMain, strWeatherDescription, weatherTemp, weatherTempFeelsLike, weatherHumidity, weatherWindSpeed);} }
	public static void SendNetworkEvent_WeatherInfo_ForAll_IncludeEveryone(string strWeatherMain, string strWeatherDescription, float weatherTemp, float weatherTempFeelsLike, Int32 weatherHumidity, float weatherWindSpeed){ foreach (var a_Player in PlayerPool.GetAllPlayers_IncludeOutOfGame()) { EventManager.TriggerRemoteEventForPlayer(a_Player, NetworkEventID.WeatherInfo, strWeatherMain, strWeatherDescription, weatherTemp, weatherTempFeelsLike, weatherHumidity, weatherWindSpeed);} }
}
