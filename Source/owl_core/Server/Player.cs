using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

public enum ENameType
{
	StaticCharacterName, // Character name, never changes, doesnt include masks or anything
	CharacterDisplayName // Character display name, changes when they use masks etc
}

public class CPlayer : CBaseEntity, IDisposable
{
	public EDrivingTestType CurrentDrivingTestType { get; set; } = EDrivingTestType.None;
	private DateTime m_LastUpdateMinutesPlayed;
	private Dimension m_SafeDimension = 0;

	public CPlayer(Player a_Client, int a_PlayerID)
	{
		m_Client = a_Client;
		SetLoggedIn();
		IsSpawned = false;

		PlayerID = a_PlayerID;

		PendingJobMoney = 0.0f;

		SetLoggedOutName();

		m_Inventory = new CPlayerInventory(this);
		m_DonationInventory = new CDonationInventory(this);
		m_Notifications = new PersistentNotificationManager(this);

		m_SaveTimer = MainThreadTimerPool.CreateEntityTimer(Save, 60000, this);

		m_TimePlayedThisSessionTimer = MainThreadTimerPool.CreateEntityTimer(UpdateTimePlayed, 60000, this);
		m_SyncInventoryWithWeaponsTimer = MainThreadTimerPool.CreateEntityTimer(SyncInventoryWithWeaponsTick, 10000, this); // Sync inventory with weapon ammo (also happens on weapon switch)
		m_SaveInventoryTimer = MainThreadTimerPool.CreateEntityTimer(SaveInventoryTick, 30000, this); // Save inventory out every 30 sec

		OnCharacterChangeRequested(); // This resets a bunch of variables

		// create timer to update blip
		CreateUpdateDutyPositionTimer();

		SetData(Client, EDataNames.PING, 0, EDataType.Synced);

		// Set during docker build process

		// TODO_GITHUB: You should set the environment variables below
#if DEBUG
		string version = Environment.GetEnvironmentVariable("GIT_LATEST_TAG") ?? "DEVBUILD";
		string hash = Environment.GetEnvironmentVariable("GIT_COMMIT_SHA") ?? "DEBUG";
#else
		string version = Environment.GetEnvironmentVariable("GIT_LATEST_TAG") ?? "EXTENDED_BETA";
		string hash = Environment.GetEnvironmentVariable("GIT_COMMIT_SHA") ?? "0bba702d";
#endif

		// TODO_LAUNCH: Do we want time played per character?
		m_LastUpdateMinutesPlayed = DateTime.Now;
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		// NOTE: this causes clientside scripts to delete the cellphone attachment object
		SetPhoneNoLongerInUse();
		ResetLastUsedPhone();

		MainThreadTimerPool.DestroyTimersFromParent(this);

		RemoveAllReconners();

		AttemptRestoreModShopVehicle();
	}

	public void OnUpdate()
	{
		// NOTE: This is triggered regardless of being logged in & spawned

		if (IsLoggedIn)
		{
			UpdateAnimationQueue();
		}

		// RAGE_HACK: Dim fix for randomly going back to zer
		if (Client.Dimension != m_SafeDimension)
		{
			Dimension CurrentDim = Client.Dimension;
			Client.Dimension = m_SafeDimension;
			NAPI.Util.ConsoleOutput("Apply RAGE Dimension fix for {5}. Player Dimension was: {0} SafeDimension is: {1} XYZ IS: {2}, {3}, {4}", CurrentDim, m_SafeDimension, Client.Position.X, Client.Position.Y, Client.Position.Z, GetCharacterName(ENameType.StaticCharacterName));
		}
	}
	public Vector3 GetEstimatedGroundPosition()
	{
		Vector3 vecGround = Client.Position;
		vecGround.Z -= 0.75f;
		return vecGround;
	}

	public int AdminJailMinutesLeft { get; set; } = -1;
	public string AdminJailReason { get; set; } = "";
	private Int64 m_AdminUnjailTimestamp = 0;

	public bool IsInAdminJail()
	{
		return AdminJailMinutesLeft > -1;
	}

	public void RecalculateAdminJailMinutesLeft()
	{
		Int64 secondsLeft = m_AdminUnjailTimestamp - Helpers.GetUnixTimestamp();
		AdminJailMinutesLeft = Convert.ToInt32(secondsLeft / 60);
	}

	public void RemoveFromAdminJail()
	{
		m_AdminUnjailTimestamp = 0;
		Database.Functions.Accounts.SetAdminJailInformation(AccountID, -1, "", () =>
		{
			AdminJailReason = "";
			AdminJailMinutesLeft = -1;
			Client.Position = new Vector3(198.82048, -931.2688, 30.686815);
			Client.Dimension = 0;
			Client.Heading = 0;
		});
	}

	public void SetPlayerInAdminJail(int minutes, string reason)
	{
		Database.Functions.Accounts.SetAdminJailInformation(AccountID, minutes, reason, () =>
		{
			AdminJailReason = reason;
			AdminJailMinutesLeft = minutes;
			ApplyAdminJail();
			PutClientInAdminJail();
		});
	}

	public void SetAdminJailData(int minutes, string reason)
	{
		AdminJailReason = reason;
		AdminJailMinutesLeft = minutes;
	}

	public uint ApplyAdminJailOnCharacterSelect()
	{
		if (AdminJailMinutesLeft == -1)
		{
			return 0;
		}

		SendNotification("Admin Jail", ENotificationIcon.ExclamationSign,
			"You are in admin jail for {0} more minutes. Reason: {1}", AdminJailMinutesLeft, AdminJailReason);
		uint dimension = PutClientInAdminJail();
		ApplyAdminJail();

		return dimension;
	}

	private uint PutClientInAdminJail()
	{
		var _random = new Random();
		uint dimension = (uint)(_random.NextDouble() * 1000);
		SetPositionSafe(new Vector3(459.89026, -997.68317, 24.914886));
		Console.WriteLine("Putting into dimension {0}", dimension);
		SetSafeDimension(dimension);
		Client.Heading = 0;

		return dimension;
	}

	private void ApplyAdminJail()
	{
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();
		Int64 UnjailTimestampAddition = AdminJailMinutesLeft * 60;
		m_AdminUnjailTimestamp = unixTimestamp + UnjailTimestampAddition;
	}

	private void UpdateAdminJailTime()
	{
		if (m_AdminUnjailTimestamp != 0)
		{
			// Has our unjail time passed?
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			if (unixTimestamp >= m_AdminUnjailTimestamp)
			{
				RemoveFromAdminJail();
				PushChatMessage(EChatChannel.Notifications, "You have finished your admin jail. Try not to break the rules next time.");
			}
		}
	}

	public void SetSafeDimension(Dimension a_SafeDimension)
	{
		m_SafeDimension = a_SafeDimension;
		m_Client.Dimension = m_SafeDimension;
	}

	public void OnTeleport()
	{
		// When they teleport set the weather. Goes out of sync if they came from inside an interior
		NetworkEventSender.SendNetworkEvent_UpdateWeatherState(this, (int)HelperFunctions.World.GetCurrentWeather());
		OnPropertyPreviewExpire_ScriptedNoExitProperty();
	}

	public void ActivateOutfit(CItemInstanceDef outfitItem)
	{
		CItemValueOutfit outfitValue = (CItemValueOutfit)outfitItem.Value;

		DeactivateAllCustomClothing();

		// find and activate the relevant items
		foreach (var kvPair in outfitValue.Clothes)
		{
			ECustomClothingComponent component = (ECustomClothingComponent)kvPair.Key;
			EntityDatabaseID clothingItemDBID = kvPair.Value;

			// find the item
			CItemInstanceDef itemDef = Inventory.GetItemFromDBID(clothingItemDBID);
			if (itemDef != null)
			{
				ActivateCustomClothing(itemDef);
			}
		}

		foreach (var kvPair in outfitValue.Props)
		{
			ECustomPropSlot prop = (ECustomPropSlot)kvPair.Key;
			EntityDatabaseID propItemDBID = kvPair.Value;

			// find the item
			CItemInstanceDef itemDef = Inventory.GetItemFromDBID(propItemDBID);
			if (itemDef != null)
			{
				ActivateCustomClothing(itemDef);
			}
		}

		// set all other outfits to inactive
		List<CItemInstanceDef> lstOutfits = Inventory.GetAllOutfits();
		foreach (CItemInstanceDef iterOutfit in lstOutfits)
		{
			CItemValueOutfit iterOutfitValue = (CItemValueOutfit)iterOutfit.Value;
			if (iterOutfitValue.IsActive)
			{
				iterOutfitValue.IsActive = false;
				Database.Functions.Items.SaveItemValue(iterOutfit);
			}
		}

		// set new outfit to active
		outfitValue.IsActive = true;
		Database.Functions.Items.SaveItemValue(outfitItem);

		ApplySkinFromInventory(true, true);
	}

	public void ActivateDutyOutfit(EDutyType a_DutyType, CItemInstanceDef outfitItem)
	{
		CItemValueDutyOutfit outfitValue = (CItemValueDutyOutfit)outfitItem.Value;

		// set all other duty outfits of this type to inactive
		List<CItemInstanceDef> lstOutfits = Inventory.GetDutyOutfitsOfType(a_DutyType);
		foreach (CItemInstanceDef iterOutfit in lstOutfits)
		{
			CItemValueDutyOutfit iterOutfitValue = (CItemValueDutyOutfit)iterOutfit.Value;
			if (iterOutfitValue.IsActive && iterOutfitValue.DutyType == a_DutyType) // only deactivate if its the same duty type, so we can in theory be in multiple factions
			{
				iterOutfitValue.IsActive = false;
				Database.Functions.Items.SaveItemValue(iterOutfit);
			}
		}

		// set new duty outfit to active
		outfitValue.IsActive = true;
		Database.Functions.Items.SaveItemValue(outfitItem);

		// this will apply duty skin
		ApplySkinFromInventory();
	}

	public async void SendBasicDonatorInfo()
	{
		int donatorCurrency = await GetDonatorCurrency().ConfigureAwait(true);

		List<DonationInventoryItemTransmit> lstDonationInventory = new List<DonationInventoryItemTransmit>();
		foreach (var donationItem in DonationInventory.Get())
		{
			string strAppliedTo = String.Empty;
			if (donationItem.Character == -1)
			{
				strAppliedTo = "Account";
			}
			else
			{
				if (donationItem.PropertyID != -1)
				{
					CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(donationItem.PropertyID);
					if (propInst != null)
					{
						strAppliedTo = propInst.Model.Name;
					}
				}
				else if (donationItem.VehicleID != -1)
				{
					CVehicle vehicle = VehiclePool.GetVehicleFromID(donationItem.VehicleID);
					if (vehicle != null)
					{
						CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicle.GTAInstance.Model);
						if (vehicleDef != null)
						{
							strAppliedTo = Helpers.FormatString("{0} {1} ({2})", vehicleDef.Manufacturer, vehicleDef.Name, vehicle.GTAInstance.NumberPlate); ;
						}
					}
				}
				else if (donationItem.Character == ActiveCharacterDatabaseID)
				{
					strAppliedTo = GetCharacterName(ENameType.StaticCharacterName);
				}
			}

			if (!String.IsNullOrEmpty(strAppliedTo))
			{
				lstDonationInventory.Add(new DonationInventoryItemTransmit(donationItem, strAppliedTo));
			}
		}

		NetworkEventSender.SendNetworkEvent_GotBasicDonatorInfo(this, donatorCurrency, lstDonationInventory);
	}


	private void SetLoggedOutName()
	{
		string strNewName = Helpers.FormatString("Player {0}", new Random().Next(1, 99999));
		SetCharacterName(ENameType.StaticCharacterName, strNewName);
		SetCharacterName(ENameType.CharacterDisplayName, strNewName);
	}

	public void SetVisible(bool bVisible)
	{
		NetworkEventSender.SendNetworkEvent_SetPlayerVisible_ForAll_IncludeEveryone(this.Client, bVisible);
	}

	public async void HandleApplicationStateAndTransmitCharacters(bool bCanAutoSpawn)
	{
		NetworkEventSender.SendNetworkEvent_ApplicationState(this, ApplicationState);

		// Have we finished the application process?
		if (ApplicationState == EApplicationState.ApplicationApproved)
		{
			SetVisible(true);
			CGetCharactersResult RetrieveCharactersResult = await Database.LegacyFunctions.GetCharacters(AccountID).ConfigureAwait(false);

			// Should we autospawn?
			bool bGotoCharSelect = true;
			if (bCanAutoSpawn)
			{
				if (AutoSpawnCharacter != -1)
				{
					// Do we actually have this character? Maybe its gone, or its an exploit and its someone elses character
					foreach (var character in RetrieveCharactersResult.m_lstCharacters)
					{
						if (character.id == AutoSpawnCharacter)
						{
							bGotoCharSelect = false;
							NetworkEvents.SendLocalEvent_CharacterSelectedLocal(this, AutoSpawnCharacter);
							break;
						}
					}
				}
			}

			if (bGotoCharSelect)
			{
				NetworkEventSender.SendNetworkEvent_RetrievedCharacters(this, RetrieveCharactersResult.m_lstCharacters, AutoSpawnCharacter);
			}
		}
		else
		{
			if (ApplicationState == EApplicationState.ApplicationRejected)
			{
				// If we were rejected, lets go back to the start. Clientside UI handles the rest.
				SetApplicationState(EApplicationState.NoApplicationSubmitted);
			}
		}
	}

	public void SetApplicationState(EApplicationState state)
	{
		m_ApplicationState = state;
		HandleApplicationStateAndTransmitCharacters(false);
		Database.Functions.Accounts.SetApplicationState(AccountID, state);
	}

	public void SetCurrentVehicleInventory(CVehicle vehicle, EVehicleInventoryType type)
	{
		m_CurrentVehicleInventory = new WeakReference<CVehicle>(vehicle);
		m_VehicleInventoryType = type;
	}

	public EVehicleInventoryType CurrentVehicleInventoryType
	{
		get
		{
			return m_VehicleInventoryType;
		}
	}

	public WeakReference<CVehicle> CurrentVehicleInventory
	{
		get
		{
			return m_CurrentVehicleInventory;
		}
	}

	private WeakReference<CVehicle> m_CurrentVehicleInventory = new WeakReference<CVehicle>(null);
	private EVehicleInventoryType m_VehicleInventoryType = EVehicleInventoryType.NONE;

	public void SetCurrentPropertyInventory(CPropertyInstance property, EMailboxAccessType accessLevel)
	{
		m_CurrentPropertyInventory = new WeakReference<CPropertyInstance>(property);
		m_PropertyInventoryAccessLevel = accessLevel;
	}

	public void ResetCurrentPropertyInventory()
	{
		m_CurrentPropertyInventory = new WeakReference<CPropertyInstance>(null);
		m_PropertyInventoryAccessLevel = EMailboxAccessType.NoAccess;
	}

	public EMailboxAccessType CurrentPropertyInventoryAccessLevel
	{
		get
		{
			return m_PropertyInventoryAccessLevel;
		}
	}

	public WeakReference<CPropertyInstance> CurrentPropertyInventory
	{
		get
		{
			return m_CurrentPropertyInventory;
		}
	}

	private WeakReference<CPropertyInstance> m_CurrentPropertyInventory = new WeakReference<CPropertyInstance>(null);
	private EMailboxAccessType m_PropertyInventoryAccessLevel = EMailboxAccessType.NoAccess;


	public void SetCurrentFurnitureInventory(EntityDatabaseID id)
	{
		CurrentFurnitureInventoryDBID = id;
	}

	public void ResetCurrentFurnitureInventory()
	{
		CurrentFurnitureInventoryDBID = -1;
	}

	public EntityDatabaseID CurrentFurnitureInventory
	{
		get
		{
			return CurrentFurnitureInventoryDBID;
		}
	}
	private EntityDatabaseID CurrentFurnitureInventoryDBID = -1;

	private int m_cachedHealth = 0;
	private int m_cachedArmor = 0;

	public void CacheHealthAndArmor()
	{
		m_cachedHealth = Client.Health;
		m_cachedArmor = Client.Armor;
	}

	public void RestoreHealthAndArmor()
	{
		Client.Health = m_cachedHealth;
		Client.Armor = m_cachedArmor;
	}

	public void SendToCharSelect()
	{
		OnCharacterChangeRequested();
		NetworkEventSender.SendNetworkEvent_ChangeCharacterApproved(this);
		HandleApplicationStateAndTransmitCharacters(false);
		GotoPlayerSpecificDimension();
	}

	public async void ResetAutoSpawnFlag()
	{
		await Database.LegacyFunctions.SetAutoSpawnCharacter(AccountID, -1).ConfigureAwait(true);
	}

	public void SetCharacterSkin(PedHash hash)
	{
		// Cache health and armor to reapply... Changing skin recreates the entire ped in V
		int oldHealth = Client.Health;
		int oldArmor = Client.Armor;

		PedHash oldHash = (PedHash)Client.Model;
		Client.SetSkin(hash);
		SetDefaultClothes();

		Client.Health = oldHealth;
		Client.Armor = oldArmor;
	}

	// CLOTHING HANDLING
	private void SetDefaultClothes()
	{
		BulkClothing clothing = new BulkClothing();

		SetClothingBulk(clothing);
	}

	public bool IsMasked()
	{
		bool bIsPremadeAndMasked = CharacterType == ECharacterType.Premade && m_bIsPremadeMasked;
		int currentMask = m_CurrentClothing.GetComponent((int)ECustomClothingComponent.Masks);
		return (currentMask > 0 && !MaskHelpers.MasksFunctioningAsBeards.Contains(currentMask)) || bIsPremadeAndMasked;
	}

	private void SetClothingBulk(BulkClothing clothing)
	{
		// Should we apply a beard?
		int currentMaskOrBeard = clothing.GetComponent((int)ECustomClothingComponent.Masks);
		if (currentMaskOrBeard <= 0 && m_CustomSkinData != null)
		{
			if (m_CustomSkinData.FullBeardStyle != 0)
			{
				clothing.Set((int)ECustomClothingComponent.Masks, m_CustomSkinData.FullBeardStyle, m_CustomSkinData.FullBeardColor);
			}
		}

		m_CurrentClothing = clothing;
		SetData(Client, EDataNames.CLOTHING, m_CurrentClothing.Serialize(), EDataType.Synced);

		UpdateMaskState();
	}

	private void UpdateMaskState()
	{
		bool bAppliedOverrideName = false;

		if (IsMasked())
		{
			bAppliedOverrideName = true;
			SetCharacterName(ENameType.CharacterDisplayName, CharacterConstants.MaskedDisplayName);
		}

		if (!bAppliedOverrideName)
		{
			SetCharacterName(ENameType.CharacterDisplayName, GetCharacterName(ENameType.StaticCharacterName));
		}
	}

	public BulkClothing GetActiveClothing()
	{
		return m_CurrentClothing;
	}

	private BulkClothing m_CurrentClothing = new BulkClothing();
	// END CLOTHING HANDLING


	public void SendGenericMessageBox(string strTitle, string strCaption)
	{
		NetworkEventSender.SendNetworkEvent_ShowGenericMessageBox(this, strTitle, strCaption);
	}

	public string GetBigSerial()
	{
		return Client.Serial;
	}

	public string GetSmallSerial()
	{
		const int serialStart = 32;
		const int serialLen = 10;
		return Client.Serial.Substring(serialStart, serialLen);
	}

	public enum EKickReason
	{
		GENERIC,
		SERIAL_NOT_AUTHORIZED,
		NO_CEF,
		LOGGED_IN_ELSEWHERE,
		ASSET_TRANSFER,
		PLAYER_REQUESTED,
		ADMIN_BANNED,
		ANTICHEAT,
	}

	public void KickFromServer(EKickReason kickReason, string strOverrideMessage = null)
	{
		// Generic reason
		string strReason = "You were kicked from the server";


		switch (kickReason)
		{
			case EKickReason.ADMIN_BANNED:
				{
					if (strOverrideMessage != null)
					{
						strReason = strOverrideMessage;
					}
					else
					{
						strReason = "You were banned.";
					}
					break;
				}

			case EKickReason.SERIAL_NOT_AUTHORIZED:
				{
					strReason = Helpers.FormatString("Connection Lost: {0}", GetSmallSerial());
					break;
				}

			case EKickReason.NO_CEF:
				{
					strReason = "This server requires CEF to be enabled. Please check your settings.";
					break;
				}

			case EKickReason.LOGGED_IN_ELSEWHERE:
				{
					strReason = "You were logged in somewhere else";
					break;
				}

			case EKickReason.ASSET_TRANSFER:
				{
					strReason = "Asset Transfer Request";
					break;
				}

			case EKickReason.PLAYER_REQUESTED:
				{
					strReason = "Player Requested";
					break;
				}
		}

		PushRawMessage(Helpers.FormatString("Disconnected From Server: {0}", strReason));
		SendGenericMessageBox("Disconnected From Server", strReason);
		Client.Kick();
	}

	private void PushRawMessage(string strRawMessage)
	{
		strRawMessage = strRawMessage.Replace(@"""", @"\""");
		Client.SendChatMessage(strRawMessage);
	}

	// Message no RGB
	public void PushChatMessage(EChatChannel a_ChatChannel, string strFormat, params object[] strParams)
	{
		string strMessage = ((int)a_ChatChannel).ToString() + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	// Message no RGB and player name
	public void PushChatMessageWithPlayerName(EChatChannel a_ChatChannel, ECharacterLanguage eSenderLanguage, string strSenderName, string strFormat, params object[] strParams)
	{
		string strMessage = ((int)a_ChatChannel).ToString() + "[" + eSenderLanguage.ToString() + "] " + strSenderName + ": " + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	public void PushChatMessageWithPlayerNameAndPostfixAndColor(EChatChannel a_ChatChannel, int r, int g, int b, ECharacterLanguage eSenderLanguage, string strSenderName, string strPostFix, string strFormat, params object[] strParams)
	{
		string strColor = "!{" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + "}";
		string strMessage = ((int)a_ChatChannel).ToString() + strColor + "[" + eSenderLanguage.ToString() + "] " + strSenderName + " " + strPostFix + ": " + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	// Message winoth RGB, player name and prefix
	public void PushChatMessageWithPlayerNameAndPrefix(EChatChannel a_ChatChannel, string strPrefix, ECharacterLanguage eSenderLanguage, string strSenderName, string strFormat, params object[] strParams)
	{
		string strMessage = ((int)a_ChatChannel).ToString() + strPrefix + " " + "[" + eSenderLanguage.ToString() + "] " + strSenderName + ": " + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	// Message with RGB
	public void PushChatMessageWithColor(EChatChannel a_ChatChannel, int r, int g, int b, string strFormat, params object[] strParams)
	{
		string strColor = "!{" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + "}";
		string strMessage = ((int)a_ChatChannel).ToString() + strColor + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	// Message with RGB and player name
	public void PushChatMessageWithColorAndPlayerName(EChatChannel a_ChatChannel, int r, int g, int b, string strSenderName, string strFormat, params object[] strParams)
	{
		string strColor = "!{" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + "}";
		string strMessage = ((int)a_ChatChannel).ToString() + strColor + strSenderName + ": " + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	// Message with RGB, player name and prefix with language
	public void PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel a_ChatChannel, int r, int g, int b, ECharacterLanguage eSenderLanguage, string strPrefix, string strSenderName, string strFormat, params object[] strParams)
	{
		string strColor = "!{" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + "}";
		string strMessage = ((int)a_ChatChannel).ToString() + strColor + strPrefix + " " + "[" + eSenderLanguage.ToString() + "] " + strSenderName + ": " + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	// Message with RGB, player name and prefix without language
	public void PushChatMessageWithRGBAndPlayerNameAndPrefixWithoutLanguage(EChatChannel a_ChatChannel, int r, int g, int b, string strPrefix, string strSenderName, string strFormat, params object[] strParams)
	{
		string strColor = "!{" + r.ToString() + ", " + g.ToString() + ", " + b.ToString() + "}";
		string strMessage = ((int)a_ChatChannel).ToString() + strColor + strPrefix + " " + strSenderName + ": " + Helpers.FormatString(strFormat, strParams);
		PushRawMessage(strMessage);
	}

	public void Logout()
	{
		Database.LegacyFunctions.RemoveSavedSessionsForAccount(m_AccountID, Client.Address, Client.Serial);
		SetLoggedIn();
		SetLoggedOutName();
		// TODO_LAUNCH: More here? reset job states etc, any timers have to go AND other things should check if logged in more
	}

	public void TeleportToPlayer(CPlayer a_TargetPlayer)
	{
		// unload our IPL and map, if available
		if (Client.Dimension > 0)
		{
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(Client.Dimension);

			if (propertyInst != null)
			{
				UnloadIPLForProperty(propertyInst);

				NetworkEventSender.SendNetworkEvent_UnloadCustomMap(this, propertyInst.GetMapID());
			}
		}

		// load IPL and map, if available
		if (a_TargetPlayer.Client.Dimension > 0)
		{
			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(a_TargetPlayer.Client.Dimension);

			if (propertyInst != null)
			{
				LoadIPLForProperty(propertyInst);

				NetworkEventSender.SendNetworkEvent_LoadCustomMap(this, propertyInst.GetMapID());
			}
		}

		SetSafeDimension(a_TargetPlayer.Client.Dimension);

		if (IsInVehicleReal && Client.Vehicle != null)
		{
			Client.Vehicle.Dimension = a_TargetPlayer.Client.Dimension;
			Client.Vehicle.Position = a_TargetPlayer.GetOffsetPosInFront().Add(new Vector3(0.0f, 0.0f, 1.0f));
		}
		else
		{
			SetPositionSafe(a_TargetPlayer.GetOffsetPosInFront());
		}
	}

	public Vector3 GetOffsetPosInFront(float fDist = 1.5f)
	{
		Vector3 vecPosInFront = Client.Position;
		float radians = (Client.Rotation.Z + 90.0f) * (3.14f / 180.0f);
		vecPosInFront.X += (float)Math.Cos(radians) * fDist;
		vecPosInFront.Y += (float)Math.Sin(radians) * fDist;
		return vecPosInFront;
	}

	// NOTE: Only triggered when logged in & spawned
	public void OnUpdateInGame()
	{
		UpdateSpawnFix();
		UpdateJailTime();
		UpdateAdminJailTime();

		// We cache the last valid position, this fixes a bug in RAGE where some time after quitting due to a race condition, the position gets reset and pos would save as 0, 0, 0
		if (!(m_Client.Position.X == 0 && m_Client.Position.Y == 0 && m_Client.Position.Z == 0))
		{
			// Only save if we aren't in the player specific dimension (presumably that means we're in a script effect / we don't want to save that pos
			if (m_Client.Dimension != GetPlayerSpecificDimension())
			{
				// Check we arent previewing a property, otherwise we won't cache the position
				if (!IsPreviewingProperty)
				{
					// Only save if we aren't reconning
					if (!IsReconning)
					{
						m_vecCachedSavePosition = m_Client.Position;
						m_fCachedSaveRotation = m_Client.Rotation.Z;
						m_CachedDimension = m_Client.Dimension;
					}
				}
			}
		}

		m_DonationInventory.Update();

		// Update impairment
		if (ImpairmentLevel > 0.0f)
		{
			if (MainThreadTimerPool.GetMillisecondsSinceDateTime(m_LastImpairmentUpdate) > 30000) // lower impairment every 30 seconds, a full 1.0 impairment would take 5 minutes to go away
			{
				ImpairmentLevel -= 0.1f; // set resets timePassedSinceLastImpairmentUpdateSeconds
			}
		}

		// Update drugs
		// We do this manually rather than call SetDrugEffectDisabled to avoid modifying the collection during iteration
		foreach (var kvPair in m_dictDrugDurations)
		{
			// Has this drug effect expired?
			Int64 timeSinceCreation = MainThreadTimerPool.GetMillisecondsSinceDateTime(kvPair.Value.StartTime);
			if (timeSinceCreation >= kvPair.Value.Duration)
			{
				EDataNames dataName = HelperFunctions.Items.GetDataNameFromDrugEffect(kvPair.Key);
				SetData(m_Client, dataName, false, EDataType.Synced);
			}
		}
		m_dictDrugDurations = m_dictDrugDurations.Where(pair => MainThreadTimerPool.GetMillisecondsSinceDateTime(pair.Value.StartTime) < pair.Value.Duration).ToDictionary(pair => pair.Key, pair => pair.Value);
		// End Update Drugs
	}

	public void Jail(int Days, int Hours, int Minutes, string strReason, EPrisonCell a_PrisonCell)
	{
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();
		Int64 UnjailTimestampAddition = ((Days * 24 * 60 * 60) + (Hours * 60 * 60) + (Minutes * 60));
		m_unixTimestampUnjail = unixTimestamp + UnjailTimestampAddition;
		m_PrisonCell = a_PrisonCell;

		Database.LegacyFunctions.ArrestPlayerTask(ActiveCharacterDatabaseID, m_unixTimestampUnjail, 0, strReason, a_PrisonCell);

		ApplyJail(Client.Dimension);
	}

	public bool IsJailed()
	{
		return m_unixTimestampUnjail > 0;
	}

	public float GetBailoutCost()
	{
		return m_fBailAmount;
	}

	private void AttemptRestoreModShopVehicle()
	{
		if (IsInVehicleReal)
		{
			CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(Client.Vehicle);

			if (vehicle != null)
			{
				if (m_Client.Dimension == GetPlayerSpecificDimension()) // vehicle in player specific dimension == mod shop
				{
					List<CPlayer> lstOccupants = VehiclePool.GetVehicleOccupants(vehicle);
					vehicle.TeleportAndWarpOccupants(lstOccupants, vehicle.CachedPositionBeforeModShop, 0, vehicle.CachedRotationBeforeModShop);
					vehicle.GTAInstance.Rotation = vehicle.CachedRotationBeforeModShop;
				}
			}
		}
	}

	public void OnCharacterChangeRequested()
	{
		AttemptRestoreModShopVehicle();

		SetLEOBadgeState(false);

		SetCurrentPet(null);

		IsInFactionMenu = false;

		StopRecon();
		RemoveAllReconners();

		// TODO_ENTITY_DATA: Make a helper method to remove all entity data?
		// Reset player specific datas
		SetData(m_Client, EDataNames.HAS_ANIM, false, EDataType.Synced);
		SetData(m_Client, EDataNames.ANIM_CANCELLABLE, false, EDataType.Synced);
		SetData(m_Client, EDataNames.DUTY, 0, EDataType.Synced);
		SetData(m_Client, EDataNames.HAS_CELL, 0, EDataType.Synced);
		SetData(m_Client, EDataNames.IS_SPAWNED, false, EDataType.Synced);
		ClearData(m_Client, EDataNames.STATUS_MESSAGE);
		ClearData(m_Client, EDataNames.CHARACTER_NAME);
		SetData(m_Client, EDataNames.LOCKSMITH_PENDING_PICKUP, false, EDataType.Synced);
		SetData(m_Client, EDataNames.INTERIOR_MANAGER, false, EDataType.Synced);

		// Badge
		ClearData(m_Client, EDataNames.BADGE_FACTION_NAME);
		ClearData(m_Client, EDataNames.BADGE_NAME);
		ClearData(m_Client, EDataNames.BADGE_COLOR_R);
		ClearData(m_Client, EDataNames.BADGE_COLOR_G);
		ClearData(m_Client, EDataNames.BADGE_COLOR_B);
		SetData(m_Client, EDataNames.BADGE_ENABLED, false, EDataType.Synced);

		ClearBackupBeacon();
		ClearUnitNumber();
		DestroyUpdateDutyPositionTimer();

		SetDefaultProps();

		// this sets the entity data also
		ImpairmentLevel = 0.0f;
		SetDrugEffectDisabled(EDrugEffect.Weed);
		SetDrugEffectDisabled(EDrugEffect.Meth);
		SetDrugEffectDisabled(EDrugEffect.Heroin);
		SetDrugEffectDisabled(EDrugEffect.Cocaine);
		SetDrugEffectDisabled(EDrugEffect.Xanax);

		Save();
		Unjail(true);
		IsSpawned = false;

		NetworkEvents.SendLocalEvent_CharacterChangeRequested(this);

		MainThreadTimerPool.MarkTimerForDeletion(m_PreviewTimer);
		MainThreadTimerPool.MarkTimerForDeletion(m_PayDayTimer);
		MainThreadTimerPool.MarkTimerForDeletion(m_SpawnDimensionFixTimer);

		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(Client.Dimension);

		if (propertyInst != null)
		{
			OnExitProperty(propertyInst);
		}

		IsPreviewingProperty = false;

		// Reset more
		NonPlayerSpecificDimension = 0;
		StopCurrentAnimation(true, true);
		m_CallStartTime = DateTime.Now;
		m_CallDurationStartTime = DateTime.Now;

		m_fMoney = -1.0f;
		m_fBankMoney = -1.0f;
		m_CharacterLanguages.Clear();
		m_lstFactionMemberships.Clear();
		m_Inventory.Reset();
		m_iPaycheckProgress = 0;
		m_DutyType = EDutyType.None;
		m_bCuffed = false;
		m_Cuffer = 0;
		m_Frisking = new WeakReference<CPlayer>(null);
		m_AmmoData.Clear();
		m_lstWeaponsClientside.Clear();
		m_LastUsedPhone = null;

		if (m_FishingTimer.Instance() != null)
		{
			MainThreadTimerPool.MarkTimerForDeletion(m_FishingTimer);
		}

		Freeze(true);
	}

	public void CheckForOutfitAchievements()
	{
		var lstOutfits = Inventory.GetAllOutfits();

		if (lstOutfits.Count == 1)
		{
			AwardAchievement(EAchievementID.MakeOutfit);
		}

		if (lstOutfits.Count >= 10)
		{
			AwardAchievement(EAchievementID.Make10Outfits);
		}
	}

	public bool IsBackupBeaconActive()
	{
		return GetData<bool>(m_Client, EDataNames.BACKUP);
	}

	public void SetBackupBeacon()
	{
		SetData(m_Client, EDataNames.BACKUP, true, EDataType.Synced);
	}

	public void ClearBackupBeacon()
	{
		ClearData(m_Client, EDataNames.BACKUP);
	}

	public bool IsTowtruckBeaconActive()
	{
		return GetData<bool>(m_Client, EDataNames.TOWTRUCK_BEACON);
	}

	public void SetTowtruckBeacon()
	{
		SetData(m_Client, EDataNames.TOWTRUCK_BEACON, true, EDataType.Synced);
	}

	public void ClearTowtruckBeacon()
	{
		ClearData(m_Client, EDataNames.TOWTRUCK_BEACON);
	}

	public void SetUnitNumber(int number)
	{
		SetData(m_Client, EDataNames.UNIT_NUMBER, number, EDataType.Synced);
	}

	public int GetUnitNumber()
	{
		return GetData<int>(m_Client, EDataNames.UNIT_NUMBER);
	}

	public void ClearUnitNumber()
	{
		ClearData(m_Client, EDataNames.UNIT_NUMBER);
	}


	float DutyPos_LastX = 0.0f;
	float DutyPos_LastY = 0.0f;
	float DutyPos_LastRZ = 0.0f;
	private void CreateUpdateDutyPositionTimer()
	{
		if (m_UpdateDutyPositionTimer.Instance() == null)
		{
			m_UpdateDutyPositionTimer = MainThreadTimerPool.CreateEntityTimer(UpdateDutyPosition, 2000, this);
			UpdateDutyPosition(null); // Update immediately
		}
	}

	private void UpdateDutyPosition(object[] parameters)
	{
		const float fDeltaLimit = 1.0f;
		// Only send if the delta > 1.0f;
		if (Math.Abs(m_Client.Position.X - DutyPos_LastX) > fDeltaLimit)
		{
			SetData(m_Client, EDataNames.DP_X, m_Client.Position.X, EDataType.Synced);
			DutyPos_LastX = m_Client.Position.X;
		}

		if (Math.Abs(m_Client.Position.Y - DutyPos_LastY) > fDeltaLimit)
		{
			SetData(m_Client, EDataNames.DP_Y, m_Client.Position.Y, EDataType.Synced);
			DutyPos_LastY = m_Client.Position.Y;
		}

		if (Math.Abs(m_Client.Rotation.Z - DutyPos_LastRZ) > fDeltaLimit)
		{
			SetData(m_Client, EDataNames.DP_RZ, m_Client.Rotation.Z, EDataType.Synced);
			DutyPos_LastRZ = m_Client.Rotation.Z;
		}
	}

	private void DestroyUpdateDutyPositionTimer()
	{
		if (m_UpdateDutyPositionTimer.Instance() != null)
		{
			MainThreadTimerPool.MarkTimerForDeletion(m_UpdateDutyPositionTimer);
			m_UpdateDutyPositionTimer.SetTarget(null);
		}

		ClearData(m_Client, EDataNames.DP_X);
		ClearData(m_Client, EDataNames.DP_Y);
		ClearData(m_Client, EDataNames.DP_RZ);
	}

	public async void Ban(int numHoursOrZeroForPermanent, string Reason, CPlayer banningAdmin)
	{
		if (IsLoggedIn)
		{
			if (numHoursOrZeroForPermanent > 0)
			{
				await Database.LegacyFunctions.AddDurationBan(GetBigSerial(), Client.Address, AccountID, banningAdmin.AccountID, Reason, numHoursOrZeroForPermanent).ConfigureAwait(true);
			}
			else
			{
				await Database.LegacyFunctions.AddPermanentBan(GetBigSerial(), Client.Address, AccountID, banningAdmin.AccountID, Reason).ConfigureAwait(true);
			}

			string strKickMessage = Helpers.FormatString("You were banned by '{0}' for '{1}' {2}.", banningAdmin.GetCharacterName(ENameType.StaticCharacterName), Reason, numHoursOrZeroForPermanent > 0 ? Helpers.FormatString("for {0} hour(s)", numHoursOrZeroForPermanent) : "Permanently");
			KickFromServer(EKickReason.ADMIN_BANNED, strKickMessage);
		}
	}

	public async void AnticheatBan(ECheatType cheatType)
	{
		if (IsLoggedIn)
		{
			await Database.LegacyFunctions.AddPermanentBan(GetBigSerial(), Client.Address, AccountID, -1, Helpers.FormatString("[AC] {0}", cheatType.ToString())).ConfigureAwait(true);

			string strKickMessage = Helpers.FormatString("You were permanently banned by the AntiCheat.");
			KickFromServer(EKickReason.ANTICHEAT, strKickMessage);
		}
	}

	public async void UpdateAdminReportCount()
	{
		AdminReportCount = AdminReportCount + 1;
		await Database.LegacyFunctions.UpdateAdminReportCount(AccountID).ConfigureAwait(true);
		RewardAdminIfEligble();
	}

	private const int ADMIN_REWARD = 75;
	public void RewardAdminIfEligble()
	{
		//Let's just check to be sure.
		if (!IsAdmin())
		{
			return;
		}

		// Elgible give reward
		if ((AdminReportCount % 30) == 0)
		{
			AddDonatorCurrency(ADMIN_REWARD);
			SendNotification("Reward", ENotificationIcon.HeartEmpty, Helpers.FormatString("You have been rewarded {0} GC's for your hard work and effort!", ADMIN_REWARD.ToString()));
			Logging.Log.CreateLog(this, Logging.ELogType.AdminCommand, null, "ADMIN REWARD - Given 75 GC reward for 30 reports.");
		}
		else
		{
			int ReportsRemaining = 30 - (AdminReportCount % 30);
			SendNotification("Reward", ENotificationIcon.InfoSign, Helpers.FormatString("{0} reports remaining for a reward!", ReportsRemaining));
		}
	}

	public void SetDiscordStatus(string strMessage)
	{
		NetworkEventSender.SendNetworkEvent_SetDiscordStatus(this, strMessage);
	}

	private void UpdateJailTime()
	{
		if (m_unixTimestampUnjail != 0)
		{
			// Has our unjail time passed?
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			if (unixTimestamp >= m_unixTimestampUnjail)
			{
				// TODO_CHAT: Should also send notification
				// TODO_CHAT: notifications should all go onto notifactions chat channel
				PushChatMessage(EChatChannel.Notifications, "You have served your sentence. Try to be a law abiding citizen next time.");
				Unjail();
			}
		}
	}

	public void Unjail(bool a_bIsChangeCharacter = false)
	{
		m_unixTimestampUnjail = 0;
		m_PrisonCell = EPrisonCell.One;
		m_fBailAmount = 0.0f;

		if (!a_bIsChangeCharacter)
		{
			// if not mission row, use ExitInterior to exit the interior properly, load any maps etc
			if (Client.Dimension != 0)
			{
				CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(Client.Dimension);
				if (propInst != null)
				{
					OnExitProperty(propInst);
				}
			}
			else
			{
				SetPositionSafe(new Vector3(433.5344, -981.8577, 30.70973));
				SetSafeDimension(0);
				Client.Rotation = new Vector3(0.0f, 0.0f, 84.11957f);
			}

			Database.LegacyFunctions.UnjailPlayer(ActiveCharacterDatabaseID);
			ApplySkinFromInventory();
		}
	}

	public void ReleaseFromJail(CPlayer a_TargetPlayer, bool a_bIsChangeCharacter = false)
	{
		// new method for manually releasing so they don't get teleported to LSPD entrance incase they need to re-arrest or w/e
		a_TargetPlayer.m_unixTimestampUnjail = 0;
		a_TargetPlayer.m_PrisonCell = EPrisonCell.One;
		a_TargetPlayer.m_fBailAmount = 0.0f;

		if (!a_bIsChangeCharacter)
		{
			a_TargetPlayer.SetPositionSafe(GetOffsetPosInFront());

			Database.LegacyFunctions.UnjailPlayer(a_TargetPlayer.ActiveCharacterDatabaseID);

			a_TargetPlayer.ApplySkinFromInventory();
		}
	}

	public void RestoreJailStatusFromDB(Dimension a_DimensionJailedIn, Int64 UnjailTime, EPrisonCell CellNumber, float BailAmount, string JailReason)
	{
		m_unixTimestampUnjail = UnjailTime;
		m_PrisonCell = CellNumber;
		m_fBailAmount = BailAmount;

		// Has jail time been exceeded?
		UpdateJailTime();

		if (m_unixTimestampUnjail != 0)
		{
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			TimeSpan t = TimeSpan.FromSeconds(m_unixTimestampUnjail - unixTimestamp);

			SendNotification("Jail", ENotificationIcon.ExclamationSign, "You still have {0} Days, {1} Hours and {2} Minutes of your sentence to serve for '{3}'.", t.Days, t.Hours, t.Minutes, JailReason);

			ApplyJail(a_DimensionJailedIn);
		}
	}

	private void ApplyJail(Dimension a_DimensionJailedIn)
	{
		// are we in the normal PD int (the mission row one), either in dim 0 or any dim
		Dictionary<EPrisonCell, Vector3> vecPos = new Dictionary<EPrisonCell, Vector3>();
		Dictionary<EPrisonCell, float> fRot = new Dictionary<EPrisonCell, float>();

		// apply defaults (Mission R ow PD)
		vecPos[EPrisonCell.One] = new Vector3(460.2026, -994.1073, 24.91487);
		fRot[EPrisonCell.One] = -87.12308f;
		vecPos[EPrisonCell.Two] = new Vector3(459.5724, -997.8905, 24.91487);
		fRot[EPrisonCell.Two] = -93.60767f;
		vecPos[EPrisonCell.Three] = new Vector3(459.6043, -1001.679, 24.91485);
		fRot[EPrisonCell.Three] = -88.50962f;

		// check for hardcoded custom interior overrides
		if (a_DimensionJailedIn != 0)
		{
			CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(a_DimensionJailedIn);
			if (propInst != null)
			{
				// NOTE: All custom ints need three cells
				// check for custom PD interiors
				if (propInst.Model.InteriorId == PDConstants.VSPDInteriorID)
				{
					vecPos[EPrisonCell.One] = new Vector3(1485.5205, 3204.6094, 47.572563);
					fRot[EPrisonCell.One] = -177.50763f;
					vecPos[EPrisonCell.Two] = new Vector3(1480.5447, 3203.7754, 47.572563);
					fRot[EPrisonCell.Two] = -83.949356f;
					vecPos[EPrisonCell.Three] = new Vector3(1482.1335, 3197.642, 47.572563);
					fRot[EPrisonCell.Three] = -79.95584f;
				}
			}
		}

		SetPositionSafe(vecPos[m_PrisonCell]);
		Client.Rotation = new Vector3(0.0f, 0.0f, fRot[m_PrisonCell]);

		if (Gender == EGender.Male)
		{
			int rng = new Random().Next(0, 4);
			PedHash SkinToUse;
			if (rng == 0)
			{
				SkinToUse = PedHash.Prisoner01;
			}
			else if (rng == 1)
			{
				SkinToUse = PedHash.PrisMuscl01SMY;
			}
			else if (rng == 2)
			{
				SkinToUse = PedHash.Prisoner01SMY;
			}
			else
			{
				SkinToUse = PedHash.Rashkovsky;
			}

			SetCharacterSkin(SkinToUse);
		}
		else
		{
			SetCharacterSkin(PedHash.FatWhite01AFM);
		}

	}

	public void HandleDeathAchievements(Player killerClient, uint weaponID)
	{
		// TODO_RAGE_11: Check why this is null sometimes?
		if (killerClient != null)
		{
			WeakReference<CPlayer> PlayerRef = PlayerPool.GetPlayerFromClient(killerClient);
			CPlayer player = PlayerRef.Instance();

			if (player != null && !IsInFactionOfType(EFactionType.LawEnforcement) && player.IsInFactionOfType(EFactionType.LawEnforcement))
			{
				AwardAchievement(EAchievementID.BeShotByCop);
				player.AwardAchievement(EAchievementID.ShootSomeoneAsCop);
			}
		}
	}

	public bool HasHandgunFirearmLicense()
	{
		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER1, ActiveCharacterDatabaseID);
		return Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
	}

	public bool HasLargeFirearmLicense()
	{
		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER2, ActiveCharacterDatabaseID);
		return Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
	}

	// TODO: Better location
	public string GetDrivingLicenseDisplayName(EDrivingTestType a_Type)
	{
		EItemID itemToCheck = EItemID.None;
		if (a_Type == EDrivingTestType.Bike)
		{
			itemToCheck = EItemID.DRIVERS_PERMIT_BIKE;
		}
		else if (a_Type == EDrivingTestType.Car)
		{
			itemToCheck = EItemID.DRIVERS_PERMIT_CAR;
		}
		else if (a_Type == EDrivingTestType.Truck)
		{
			itemToCheck = EItemID.DRIVERS_PERMIT_LARGE;
		}

		CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemToCheck];
		return itemDef.GetNameIgnoreGenericItems();
	}

	public bool HasDrivingLicense(EDrivingTestType a_Type)
	{
		EItemID itemToCheck = EItemID.None;
		if (a_Type == EDrivingTestType.Bike)
		{
			itemToCheck = EItemID.DRIVERS_PERMIT_BIKE;
		}
		else if (a_Type == EDrivingTestType.Car)
		{
			itemToCheck = EItemID.DRIVERS_PERMIT_CAR;
		}
		else if (a_Type == EDrivingTestType.Truck)
		{
			itemToCheck = EItemID.DRIVERS_PERMIT_LARGE;
		}

		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(itemToCheck, ActiveCharacterDatabaseID);
		return Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
	}

	public bool IsCuffed()
	{
		return m_bCuffed;
	}

	public void RestoreCuffStatusFromDB(bool a_bCuffed, EntityDatabaseID a_Cuffer)
	{
		InternalSetCuffStatus(a_bCuffed, a_Cuffer, true);
	}

	public void ForceUncuff()
	{
		InternalSetCuffStatus(false, 0, false);
	}

	public void InternalSetCuffStatus(bool a_bCuffed, EntityDatabaseID a_Cuffer, bool a_bIsFromDB = false)
	{
		SetData(m_Client, EDataNames.CUFFED, a_bCuffed, EDataType.Synced);

		m_Cuffer = a_Cuffer;

		m_bCuffed = a_bCuffed;

		if (a_bCuffed)
		{
			AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_arresting", "idle", false, true, true, 0, false);
		}
		else
		{
			StopCurrentAnimation(true);
		}

		// Do DB operations if this wasnt being set FROm the DB
		if (!a_bIsFromDB)
		{
			Database.LegacyFunctions.SetPlayerCuffedState(ActiveCharacterDatabaseID, a_bCuffed, a_Cuffer);
		}
	}

	public bool Cuff(CPlayer a_RequestingPlayer)
	{
		bool bResult = false;

		if (a_RequestingPlayer == this)
		{
			a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "You cannot handcuff yourself.", null);
		}
		else
		{
			if (!a_RequestingPlayer.IsCuffed())
			{
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromItemID(EItemID.HANDCUFFS);
				if (a_RequestingPlayer.Inventory.HasItem(ItemInstanceDef, false, out CItemInstanceDef cuffItem))
				{
					// Is the other person cuffed already?
					if (!IsCuffed())
					{
						if (cuffItem != null)
						{
							// Are we near enough?
							float fDist = (a_RequestingPlayer.Client.Position - this.Client.Position).Length();
							if (fDist <= Constants.NearbyPlayerActionDistandThreshold)
							{
								CItemValueBasic cuffData = cuffItem.GetValueData<CItemValueBasic>();
								InternalSetCuffStatus(true, (UInt32)cuffData.value);
								bResult = true;
								HelperFunctions.Chat.SendAmeMessage(a_RequestingPlayer, Helpers.FormatString("tightens the handcuffs around {0}'s wrists.", GetCharacterName(ENameType.StaticCharacterName)));

								// Take the handcuffs from the person and give them a key instead
								bool bItemRemoved = a_RequestingPlayer.Inventory.RemoveItem(cuffItem);
								CItemInstanceDef ItemInstanceToAdd = CItemInstanceDef.FromBasicValueNoDBID(EItemID.HANDCUFFS_KEY, m_Cuffer);
								a_RequestingPlayer.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceToAdd, EShowInventoryAction.DoNothing, EItemID.None, null);
							}
							else
							{
								a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "{0} is too far away.", GetCharacterName(ENameType.StaticCharacterName));
							}
						}
					}
					else
					{
						a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "{0} is already handcuffed.", GetCharacterName(ENameType.StaticCharacterName));
					}
				}
				else
				{
					a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "You cannot handcuff a person without handcuffs.", null);
				}
			}
			else
			{
				a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "You cannot handcuff a person while you are handcuffed.", null);
			}
		}
		return bResult;
	}

	public bool Uncuff(CPlayer a_RequestingPlayer)
	{
		bool bResult = false;

		// Is the other person cuffed already?
		if (!a_RequestingPlayer.IsCuffed())
		{
			if (a_RequestingPlayer != this)
			{
				if (IsCuffed())
				{
					// Do we have the keys to this specific pair of cuffs?
					CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.HANDCUFFS_KEY, m_Cuffer);
					if (a_RequestingPlayer.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem))
					{
						// Are we near enough?
						float fDist = (a_RequestingPlayer.Client.Position - this.Client.Position).Length();
						if (fDist <= Constants.NearbyPlayerActionDistandThreshold)
						{
							InternalSetCuffStatus(false, 0);
							bResult = true;
							HelperFunctions.Chat.SendAmeMessage(a_RequestingPlayer, Helpers.FormatString("removes the handcuffs around {0}'s wrists.", GetCharacterName(ENameType.StaticCharacterName)));

							// Take the keys from the person and give them their handcuffs instead
							bool bItemRemoved = a_RequestingPlayer.Inventory.RemoveItem(outItem);
							CItemInstanceDef ItemInstanceToAdd = CItemInstanceDef.FromBasicValueNoDBID(EItemID.HANDCUFFS, 0);
							a_RequestingPlayer.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceToAdd, EShowInventoryAction.DoNothing, EItemID.None, null);
						}
						else
						{
							a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "{0} is too far away.", GetCharacterName(ENameType.StaticCharacterName));
						}
					}
					else
					{
						a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "You do not have the keys to this pair of handcuffs.");
					}
				}
				else
				{
					a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "{0} is not handcuffed.", GetCharacterName(ENameType.StaticCharacterName));
				}
			}
			else
			{
				a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "You cannot un-handcuff yourself");
			}
		}
		else
		{
			a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "You cannot remove the handcuffs from a person while you are handcuffed.", null);
		}

		return bResult;
	}

	public bool Frisk(CPlayer a_RequestingPlayer, bool bAdmin = false)
	{
		if (a_RequestingPlayer.IsCuffed() && !bAdmin)
		{
			a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "You cannot frisk someone while restrained", null);
		}
		if (!IsCuffed() && !bAdmin)
		{
			a_RequestingPlayer.SendNotification("Restrain", ENotificationIcon.ExclamationSign, "The person must be restrained before you can frisk them.", null);
		}
		else
		{
			// TODO: serverside distance check
			a_RequestingPlayer.SetFrisking(this, bAdmin);
			NetworkEventSender.SendNetworkEvent_ClientFriskPlayer(a_RequestingPlayer, this.Client, Inventory.GetAllItems());

			if (!bAdmin)
			{
				HelperFunctions.Chat.SendAmeMessage(a_RequestingPlayer, Helpers.FormatString("frisks {0}.", GetCharacterName(ENameType.StaticCharacterName)));
			}
			return true;
		}

		return false;
	}

	public void OnPropertyPreviewExpire_ScriptedNoExitProperty()
	{
		MainThreadTimerPool.MarkTimerForDeletion(m_PreviewTimer);
		IsPreviewingProperty = false;
	}

	public void OnPropertyPreviewExpire(object[] a_Parameters = null)
	{
		OnPropertyPreviewExpire_ScriptedNoExitProperty();

		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(Client.Dimension);

		if (propertyInst != null)
		{
			OnExitProperty(propertyInst);
		}
	}

	private string m_strCurrentIPL = String.Empty;
	public void LoadIPLForInteriorDef(CInteriorDefinition interiorDef)
	{
		if (interiorDef != null)
		{
			if (interiorDef.IPLName.Length > 0)
			{
				if (interiorDef.IPLName != m_strCurrentIPL) // Only increment if we arent already in it
				{
					m_strCurrentIPL = interiorDef.IPLName;
					if (!IPLHelper.m_dictIPLRefCounts.ContainsKey(interiorDef.IPLName)) // Needs load
					{
						IPLHelper.m_dictIPLRefCounts.Add(interiorDef.IPLName, 1);
						NAPI.World.RequestIpl(interiorDef.IPLName);
					}
					else // already loaded, just increment the refcount
					{
						IPLHelper.m_dictIPLRefCounts[interiorDef.IPLName]++;
					}
				}
			}
		}
	}

	public void UnloadIPLForInteriorDef(CInteriorDefinition interiorDef)
	{
		if (interiorDef != null)
		{
			if (interiorDef.IPLName.Length > 0)
			{
				int refCount = 0;
				if (m_strCurrentIPL == interiorDef.IPLName && IPLHelper.m_dictIPLRefCounts.ContainsKey(interiorDef.IPLName))
				{
					m_strCurrentIPL = String.Empty;
					IPLHelper.m_dictIPLRefCounts[interiorDef.IPLName]--;
					refCount = IPLHelper.m_dictIPLRefCounts[interiorDef.IPLName];
				}

				if (refCount <= 0)
				{

					IPLHelper.m_dictIPLRefCounts.Remove(interiorDef.IPLName);
					NAPI.World.RemoveIpl(interiorDef.IPLName);
				}
			}
		}
	}

	private void LoadIPLForProperty(CPropertyInstance a_Property)
	{
		CInteriorDefinition interiorDef = InteriorDefinitions.GetInteriorDefinition(a_Property.Model.InteriorId);
		LoadIPLForInteriorDef(interiorDef);
	}

	private void UnloadIPLForProperty(CPropertyInstance a_Property)
	{
		CInteriorDefinition interiorDef = InteriorDefinitions.GetInteriorDefinition(a_Property.Model.InteriorId);
		UnloadIPLForInteriorDef(interiorDef);
	}

	public void OnPreviewProperty(CPropertyInstance a_Property)
	{
		if (!Client.Dead)
		{
			OnEnterProperty(a_Property);
			m_PreviewTimer = MainThreadTimerPool.CreateGlobalTimer(OnPropertyPreviewExpire, 60000);
			Logging.Log.CreateLog(this, Logging.ELogType.PropertyRelated, new List<CBaseEntity>() { a_Property }, "PREVIEW PROPERTY.");
			IsPreviewingProperty = true;
		}
	}

	public void OnEnterProperty(CPropertyInstance a_Property, bool bIsCharacterPreview = false, bool bDontSetPosition = false) // bUsePlayerDimension is for interior preview when on char select preview
	{
		bool isManager = a_Property.OwnedBy(this) || (a_Property.Model.OwnerType == EPropertyOwnerType.Faction && IsFactionManager(a_Property.Model.OwnerId));

		SetData(m_Client, EDataNames.INTERIOR_MANAGER, isManager, EDataType.Synced);


		a_Property.Model.MarkAsUsed();

		Logging.Log.CreateLog(this, Logging.ELogType.PropertyRelated, new List<CBaseEntity>() { a_Property },
			$"Entered {a_Property.Model.Name}.");

		LoadIPLForProperty(a_Property);

		// NOTE: This breaks at > 2 billion entities :(	
		SetSafeDimension(bIsCharacterPreview ? Client.Dimension : (Dimension)a_Property.Model.Id);

		if (!bDontSetPosition)
		{
			Client.Rotation = new Vector3(0.0f, 0.0f, a_Property.Model.ExitRotation);
		}

		Vector3 vecPos = bDontSetPosition ? Client.Position : a_Property.Model.ExitPosition;
		NetworkEventSender.SendNetworkEvent_EnterInteriorApproved(this, vecPos.X, vecPos.Y, vecPos.Z, a_Property.GetMapID(), bDontSetPosition);
		// update weather so inside the interior is always clear.
		NetworkEventSender.SendNetworkEvent_UpdateWeatherState(this, (int)GTANetworkAPI.Weather.EXTRASUNNY);

		a_Property.TransmitFurnitureAndRemovals(this);
	}

	public void RemoveAllReconners()
	{
		foreach (var player in PlayerPool.GetAllPlayers())
		{
			if (player.IsReconningPlayer(this))
			{
				player.StopRecon();
			}
		}
	}

	// TODO_LAUNCH: Properties inside of properties probably don't work? due to prop id + maps?
	public void OnExitProperty(CPropertyInstance a_Property)
	{
		SetData(m_Client, EDataNames.INTERIOR_MANAGER, false, EDataType.Synced);

		a_Property.Model.MarkAsUsed();

		Logging.Log.CreateLog(this, Logging.ELogType.PropertyRelated, new List<CBaseEntity>() { a_Property },
			$"Exited {a_Property.Model.Name}.");

		UnloadIPLForProperty(a_Property);

		// NOTE: This breaks at > 2 billion entities :(
		SetSafeDimension(a_Property.Model.EntranceDimension);
		Client.Rotation = new Vector3(0.0f, 0.0f, a_Property.Model.EntranceRotation);

		// Were we previewing a property? If so kill the timer
		if (IsPreviewingProperty)
		{
			OnPropertyPreviewExpire_ScriptedNoExitProperty();
		}

		bool bHasParentInterior = false;
		int parentMapID = -1;
		CPropertyInstance parentProperty = null;
		if (a_Property.Model.EntranceDimension != 0)
		{
			parentProperty = PropertyPool.GetPropertyInstanceFromID(a_Property.Model.EntranceDimension);
			if (parentProperty != null)
			{
				bHasParentInterior = true;
				parentMapID = parentProperty.GetMapID();
				LoadIPLForProperty(parentProperty);
			}
		}

		NetworkEventSender.SendNetworkEvent_ExitInteriorApproved(this, a_Property.Model.EntrancePosition.X, a_Property.Model.EntrancePosition.Y, a_Property.Model.EntrancePosition.Z, a_Property.GetMapID(), bHasParentInterior, parentMapID);

		if (bHasParentInterior)
		{
			// update weather so inside the interior is always clear.
			NetworkEventSender.SendNetworkEvent_UpdateWeatherState(this, (int)GTANetworkAPI.Weather.EXTRASUNNY);

			parentProperty.TransmitFurnitureAndRemovals(this);
		}
		else
		{
			// Update weather to be synced with the world again.
			NetworkEventSender.SendNetworkEvent_UpdateWeatherState(this, (int)HelperFunctions.World.GetCurrentWeather());
		}
	}

	public void OnEnterElevator(CElevatorInstance a_Elevator, bool bDontSetPosition = false)
	{

		void SendMessageToPlayer(string strMessage, bool bIsRPText)
		{
			WeakReference<CPlayer> targetRef = PlayerPool.GetPlayerFromClient(m_Client);
			CPlayer targetPlayer = targetRef.Instance();
			if (bIsRPText)
			{
				HelperFunctions.Chat.SendAmeMessage(targetPlayer, strMessage);
			}
			else
			{
				targetPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, strMessage);
			}
		}

		void StartTeleport()
		{
			if (a_Elevator.CarElevator)
			{
				if (m_Client != null && m_Client.IsInVehicle && m_Client.VehicleSeat == (int)EVehicleSeat.Driver)
				{
					CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(m_Client.Vehicle);

					if (vehicle != null)
					{
						vehicle.GiveTeleportImmunity();
						List<CPlayer> lstOccupants = VehiclePool.GetVehicleOccupants(vehicle);
						vehicle.TeleportAndWarpOccupants(lstOccupants, a_Elevator.ExitPos, a_Elevator.ExitDim, new Vector3(0.0f, 0.0f, a_Elevator.EndRot));
					}
				}
				else
				{
					SendMessageToPlayer("This elevator can only be accessed by the driver of a vehicle.", false);
				}
			}
			else
			{
				Vector3 vecPos = bDontSetPosition ? Client.Position : a_Elevator.ExitPos;
				SetSafeDimension(a_Elevator.ExitDim);
				NetworkEventSender.SendNetworkEvent_EnterElevatorApproved(this, vecPos.X, vecPos.Y, vecPos.Z, a_Elevator.GetMapID(false), bDontSetPosition);
			}
		}

		if (!bDontSetPosition)
		{
			Client.Rotation = new Vector3(0.0f, 0.0f, a_Elevator.EndRot);
		}

		if (a_Elevator.ExitDim != 0)
		{
			if (a_Elevator.ExitDim == a_Elevator.StartDim)
			{
				StartTeleport();
				return;
			}

			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(a_Elevator.ExitDim);
			bool hasProperty = propertyInst != null && !propertyInst.Model.Locked;
			if (hasProperty)
			{
				LoadIPLForProperty(propertyInst);
				NetworkEventSender.SendNetworkEvent_UpdateWeatherState(this, (int)GTANetworkAPI.Weather.EXTRASUNNY);
				StartTeleport();
			}
			else
			{
				SendMessageToPlayer("pulls on the handle to open the door to find that it is locked.", true);
			}
		}
		else if (a_Elevator.StartDim != 0)
		{
			if (a_Elevator.StartDim == a_Elevator.ExitDim)
			{
				StartTeleport();
				return;
			}

			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(a_Elevator.StartDim);
			bool hasProperty = propertyInst != null && !propertyInst.Model.Locked;
			if (hasProperty)
			{
				StartTeleport();
			}
			else
			{
				SendMessageToPlayer("pulls on the handle to open the door to find that it is locked.", true);
			}
		}
		else
		{
			StartTeleport();
		}
	}

	public void OnExitElevator(CElevatorInstance a_Elevator)
	{

		void SendMessageToPlayer(string strMessage, bool bIsRPText)
		{
			WeakReference<CPlayer> targetRef = PlayerPool.GetPlayerFromClient(m_Client);
			CPlayer targetPlayer = targetRef.Instance();
			if (bIsRPText)
			{
				HelperFunctions.Chat.SendAmeMessage(targetPlayer, strMessage);
			}
			else
			{
				targetPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, strMessage);

			}
		}

		void StartTeleport()
		{
			if (a_Elevator.CarElevator)
			{
				if (m_Client != null && m_Client.IsInVehicle && m_Client.VehicleSeat == (int)EVehicleSeat.Driver)
				{
					CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(m_Client.Vehicle);

					if (vehicle != null)
					{
						vehicle.GiveTeleportImmunity();
						List<CPlayer> lstOccupants = VehiclePool.GetVehicleOccupants(vehicle);
						vehicle.TeleportAndWarpOccupants(lstOccupants, a_Elevator.EntrancePos, a_Elevator.StartDim, new Vector3(0.0f, 0.0f, a_Elevator.StartRot));
					}
				}
				else
				{
					SendMessageToPlayer("This elevator can only be accessed by the driver of a vehicle.", false);
				}
			}
			else
			{
				Client.Rotation = new Vector3(0.0f, 0.0f, a_Elevator.StartRot);
				SetSafeDimension(a_Elevator.StartDim);

				bool bHasParentInterior = false;
				int parentMapID = -1;
				if (a_Elevator.StartDim != 0)
				{
					CPropertyInstance parentProperty = PropertyPool.GetPropertyInstanceFromID(a_Elevator.StartDim);
					if (parentProperty != null)
					{
						bHasParentInterior = true;
						parentMapID = parentProperty.GetMapID();
					}
				}
				else if (a_Elevator.ExitDim != 0)
				{
					CPropertyInstance parentProperty = PropertyPool.GetPropertyInstanceFromID(a_Elevator.ExitDim);
					if (parentProperty != null)
					{
						bHasParentInterior = true;
						parentMapID = parentProperty.GetMapID();
					}
				}

				NetworkEventSender.SendNetworkEvent_ExitElevatorApproved(this, a_Elevator.EntrancePos.X, a_Elevator.EntrancePos.Y, a_Elevator.EntrancePos.Z, a_Elevator.GetMapID(true), bHasParentInterior, parentMapID);
			}
		}

		if (a_Elevator.ExitDim != 0)
		{
			if (a_Elevator.ExitDim == a_Elevator.StartDim)
			{
				StartTeleport();
				return;
			}

			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(a_Elevator.ExitDim);
			bool hasProperty = propertyInst != null && !propertyInst.Model.Locked;
			if (hasProperty)
			{
				StartTeleport();
			}
			else
			{
				SendMessageToPlayer("pulls on the handle to open the door to find that it is locked.", true);
			}
		}
		else if (a_Elevator.StartDim != 0)
		{
			if (a_Elevator.StartDim == a_Elevator.ExitDim)
			{
				StartTeleport();
				return;
			}

			CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(a_Elevator.StartDim);
			bool hasProperty = propertyInst != null && !propertyInst.Model.Locked;
			if (hasProperty)
			{
				NetworkEventSender.SendNetworkEvent_UpdateWeatherState(this, (int)HelperFunctions.World.GetCurrentWeather());
				StartTeleport();
			}
			else
			{
				SendMessageToPlayer("pulls on the handle to open the door to find that it is locked.", true);
			}
		}
		else
		{
			StartTeleport();
		}

	}

	public void SetPositionSafe(Vector3 vecPos)
	{
		NetworkEventSender.SendNetworkEvent_SafeTeleport(this, vecPos.X, vecPos.Y, vecPos.Z);
	}

	private PayDayDetails g_LastPayDayInfo = null;

	public PayDayDetails GetPayDayDetails()
	{
		return g_LastPayDayInfo;
	}

	private void TryAwardPaycheck(object[] parameters)
	{
		if (PaydayProgress >= 60)
		{
			float fVehicleTax = 0.0f;
			float fPropertyTax = 0.0f;

			float fPropertyTaxSaved = 0.0f;
			float fVehicleTaxSaved = 0.0f;

			CalculatePayCheck(out List<CPaycheckEntry> lstPaycheckEntries, true);

			if (lstPaycheckEntries.Count > 0) // Should always have something... since there are state benefits
			{
				float fTotalStateIncomeTax = 0.0f;
				float fTotalFederalIncomeTax = 0.0f;
				float fTotalGrossIncome = 0.0f;
				float fTotalNetIncome = 0.0f;
				float fTotalDonatorPerk = 0.0f;

				foreach (CPaycheckEntry PayCheckEntry in lstPaycheckEntries)
				{
					PayCheckEntry.AppendValues(ref fTotalStateIncomeTax, ref fTotalFederalIncomeTax, ref fTotalGrossIncome, ref fTotalNetIncome);
				}

				// TODO: Actually give tax to government faction (or split it between ems and pd)

				// DONATOR PERKS
				// TODO_PAYDAY: This doesnt actually count towards their credit calculation now :(
				if (DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.SmallDollarPaycheckIncrease))
				{
					fTotalDonatorPerk += 25.0f;
				}

				if (DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.LargeDollarPaycheckIncrease))
				{
					fTotalDonatorPerk += 50.0f;
				}
				fTotalNetIncome += fTotalDonatorPerk;

				// END DONATOR PERKS

				// Add income to bank balance first before doing deductions
				AddBankMoney(fTotalNetIncome, PlayerMoneyModificationReason.PayCheck);

				// store
				g_LastPayDayInfo = new PayDayDetails(lstPaycheckEntries, fTotalDonatorPerk);

				// VEHICLES
				List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromPlayer(this);

				if (lstVehicles.Count > 0)
				{
					foreach (CVehicle vehicle in lstVehicles)
					{
						bool bWasRepossessed = false;
						bool bJustMissedPayment = false;

						float fMonthlyPaymentAmount = vehicle.GetMonthlyPaymentAmount();

						// Does this vehicle have payments remaining?
						if (vehicle.PaymentsRemaining > 0)
						{
							// Can we afford the payment?
							if (SubtractBankBalanceIfCanAfford(fMonthlyPaymentAmount, PlayerMoneyModificationReason.VehicleMonthlyPayment))
							{
								vehicle.PaymentsRemaining--;
								vehicle.PaymentsMade++;
								vehicle.PaymentsMissed = 0;
							}
							else
							{
								vehicle.PaymentsMissed++;

								bJustMissedPayment = true;

								// Do we need to repossess card
								if (vehicle.PaymentsMissed >= 3)
								{
									bWasRepossessed = true;
									vehicle.Repossess();
								}
							}
						}

						// Vehicle tax for this vehicle (always taken, unless a relevant donator perk is active)
						float fMonthlyTax = Taxation.GetVehicleMonthlyTaxRate(vehicle.GTAInstance.Class);

						if (!bWasRepossessed)
						{
							// DONATION PERK: No vehicle tax
							if (DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.NoVehicleTax))
							{
								fVehicleTaxSaved += fMonthlyTax;
								fMonthlyTax = 0.0f;
							}

							// Can we afford the payment?
							if (SubtractBankBalanceIfCanAfford(fMonthlyTax, PlayerMoneyModificationReason.VehicleMonthlyTax))
							{
								fVehicleTax += fMonthlyTax;
							}
							else
							{
								// TODO: Do something if we cant? Police warrant? Add to MDC?
							}
						}

						g_LastPayDayInfo.AddVehicleOrProperty(new PayDayVehicleOrPropertyDetails(true, vehicle.GetFullDisplayName(), fMonthlyPaymentAmount, vehicle.PaymentsRemaining, vehicle.PaymentsMade, vehicle.PaymentsMissed, bJustMissedPayment, bWasRepossessed, fMonthlyTax));
					}
				}

				// PROPERTIES

				List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByPlayer(this);

				if (lstProperties.Count > 0)
				{
					foreach (CPropertyInstance propertyInst in lstProperties)
					{
						bool bWasRepossessed = false;
						bool bJustMissedPayment = false;

						float fMonthlyPaymentAmount = propertyInst.GetMonthlyPaymentAmount();

						// Does this property have payments remaining?
						if (propertyInst.Model.PaymentsRemaining > 0)
						{
							// Can we afford the payment?
							if (SubtractBankBalanceIfCanAfford(fMonthlyPaymentAmount, PlayerMoneyModificationReason.PropertyMonthlyPayment))
							{
								propertyInst.Model.MakePayment();
							}
							else
							{
								propertyInst.Model.MissPayment();

								// Do we need to repossess card
								if (propertyInst.Model.PaymentsMissed >= 3)
								{
									bWasRepossessed = true;
									propertyInst.Repossess();
								}
							}
						}

						// Property tax for this property (always taken, unless a relevant donator perk is active)
						float fMonthlyTax = propertyInst.GetMonthlyTax();

						if (!bWasRepossessed)
						{
							// DONATION PERK: No vehicle tax
							if (DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.NoPropertyTax))
							{
								fPropertyTaxSaved += fMonthlyTax;
								fMonthlyTax = 0.0f;
							}

							// Can we afford the payment?
							if (SubtractBankBalanceIfCanAfford(fMonthlyTax, PlayerMoneyModificationReason.PropertyMonthlyTax))
							{
								fPropertyTax += fMonthlyTax;
							}
							else
							{
								// TODO: Do something if we cant? Police warrant? Add to MDC?
							}
						}

						g_LastPayDayInfo.AddVehicleOrProperty(new PayDayVehicleOrPropertyDetails(false, propertyInst.Model.Name, fMonthlyPaymentAmount, propertyInst.Model.PaymentsRemaining, propertyInst.Model.PaymentsMade, propertyInst.Model.PaymentsMissed, bJustMissedPayment, bWasRepossessed, fMonthlyTax));
					}
				}

				g_LastPayDayInfo.TotalVehicleTaxSaved = fVehicleTaxSaved;
				g_LastPayDayInfo.TotalPropertyTaxSaved = fPropertyTaxSaved;
			}

			PaydayProgress = 0;

			m_PayDayTimer = MainThreadTimerPool.CreateEntityTimer(TryAwardPaycheck, Constants.TimeBetweenPaydays, this, 1);

			PushChatMessageWithColor(EChatChannel.Notifications, 255, 194, 14, "You just received a pay day! Use /ShowPayday to see more details");
			SendNotification("PayDay", ENotificationIcon.USD, "You just received a pay day! Use /ShowPayday to see more details");
		}
	}

	public void CalculatePayCheck(out List<CPaycheckEntry> a_lstPaycheckEntries, bool a_bRewardPaycheck) // NOTE: Reward paycheck gives the money to the player, false generates all the information but does NOT reward the player
	{
		a_lstPaycheckEntries = new List<CPaycheckEntry>();

		float fStateIncomeTax = Taxation.GetStateIncomeTax();
		float fFederalIncomeTax = Taxation.GetFederalIncomeTax();
		float fGrossIncomeCalculation = 0f;

		// Process all paychecks for factions
		foreach (CFactionMembership factionMembership in m_lstFactionMemberships)
		{
			CFactionRank factionRank = factionMembership.Faction.GetFactionRank(factionMembership.Rank);

			if (factionRank != null && factionRank.Salary > 0.0f)
			{
				float fGrossIncome = factionRank.Salary;
				fGrossIncomeCalculation += fGrossIncome;
				string strSalaryName = Helpers.FormatString("{0} {1} Salary", factionMembership.Faction.ShortName, factionRank.Name);
				float fStateIncomeTaxAmount = fStateIncomeTax * fGrossIncome;
				float fFederalIncomeTaxAmount = fFederalIncomeTax * fGrossIncome;
				float fNetIncome = fGrossIncome - (fStateIncomeTaxAmount + fFederalIncomeTaxAmount);
				CPaycheckEntry paycheckEntry = new CPaycheckEntry(fGrossIncome, strSalaryName, fStateIncomeTaxAmount, fFederalIncomeTaxAmount, fNetIncome);

				if (a_bRewardPaycheck)
				{
					if (factionMembership.Faction.SubtractMoney(fGrossIncome))
					{
						a_lstPaycheckEntries.Add(paycheckEntry);
					}
					else
					{
						SendNotification("Paycheck", ENotificationIcon.USD, "{0} cannot afford to pay your salary of {1} for being a {2}.", factionMembership.Faction.ShortName, fGrossIncome, factionRank.Name);
					}
				}
				else
				{
					a_lstPaycheckEntries.Add(paycheckEntry);
				}
			}
		}

		// Do they have any income from jobs?
		if (PendingJobMoney > 0.0f)
		{
			float fGrossIncome = PendingJobMoney;
			fGrossIncomeCalculation += fGrossIncome;
			string strSalaryName = "Jobs";
			float fStateIncomeTaxAmount = fStateIncomeTax * fGrossIncome;
			float fFederalIncomeTaxAmount = fFederalIncomeTax * fGrossIncome;
			float fNetIncome = fGrossIncome - (fStateIncomeTaxAmount + fFederalIncomeTaxAmount); // TODO: Helper functions for tax
			CPaycheckEntry paycheckEntry = new CPaycheckEntry(fGrossIncome, strSalaryName, fStateIncomeTaxAmount, fFederalIncomeTaxAmount, fNetIncome);
			a_lstPaycheckEntries.Add(paycheckEntry);

			// Reset pending job money
			PendingJobMoney = 0.0f;
		}


		// If they don't make much money per paycheque give them state benefits
		if (fGrossIncomeCalculation < 1500.0f)
		{
			float fGrossIncome = 500.0f;
			string strSalaryName = "State Benefits";
			float fStateIncomeTaxAmount = 0.0f;
			float fFederalIncomeTaxAmount = 0.0f;
			float fNetIncome = fGrossIncome - (fStateIncomeTaxAmount + fFederalIncomeTaxAmount);
			CPaycheckEntry paycheckEntry = new CPaycheckEntry(fGrossIncome, strSalaryName, fStateIncomeTaxAmount, fFederalIncomeTaxAmount, fNetIncome);
			a_lstPaycheckEntries.Add(paycheckEntry);
		}
	}



	public string GetJobName()
	{
		Dictionary<EJobID, string> jobs = new Dictionary<EJobID, string>
		{
			{EJobID.None, "None"},
			{EJobID.TruckerJob, "Trucker"},
			{EJobID.DeliveryDriverJob, "Delivery Driver"},
			{EJobID.BusDriverJob, "Bus Driver"},
			{EJobID.MailmanJob, "Mailman"},
			{EJobID.TrashmanJob, "Trash Man"},
			{EJobID.TaxiDriverJob, "Taxi Driver"},
			{EJobID.TagRemoverJob, "Graffiti Remover"},
		};

		return jobs[Job];
	}

	public void OnCharacterPreSpawned(Int64 a_ActiveCharacterDBID)
	{
		ActiveCharacterDatabaseID = a_ActiveCharacterDBID;
		Inventory.Reset();
	}

	private void ApplyRealSpawnDimension(Dimension a_Dimension)
	{
		NetworkEventSender.SendNetworkEvent_SetLoadingWorld(this, false);
		SetSafeDimension(a_Dimension);
	}

	public void OnCharacterSpawned(Int64 ActiveCharacterDBID, Dimension a_PendingDimension)
	{
		// START FOURTH OF JULY
		// This is for the fourth of july event so the fireworks also start for someone who spawned mid event
		if (HelperFunctions.World.IsFourthOfJulyEventInProgress() && HelperFunctions.World.IsFourthOfJuly())
		{
			NetworkEventSender.SendNetworkEvent_StartFourthOfJulyFireworksOnly(this);
		}
		// END FOURTH OF JULY

		// RAGE_HACK: Dimension zero should be slow loaded, if non zero, load immediately

		// stop previous timer if exists
		if (m_SpawnDimensionFixTimer.Instance() != null)
		{
			MainThreadTimerPool.MarkTimerForDeletion(m_SpawnDimensionFixTimer);
		}

		if (a_PendingDimension == 0)
		{
			NetworkEventSender.SendNetworkEvent_SetLoadingWorld(this, true);

			int timerMS = 5000;
#if DEBUG
			timerMS = 1;
#endif

			m_SpawnDimensionFixTimer = MainThreadTimerPool.CreateEntityTimer((object[] paramters) => { ApplyRealSpawnDimension(a_PendingDimension); }, timerMS, this, 1);
		}
		else
		{
			SetSafeDimension(a_PendingDimension);
		}


		// END_RAGE_HACK

		// stop previous timer if exists
		if (m_PayDayTimer.Instance() != null)
		{
			MainThreadTimerPool.MarkTimerForDeletion(m_PayDayTimer);
		}

		m_PayDayTimer = MainThreadTimerPool.CreateEntityTimer(TryAwardPaycheck, Constants.TimeBetweenPaydays - (60000 * PaydayProgress), this, 1);

		NetworkEvents.SendLocalEvent_CharacterSpawned(this);

		InternalSetCuffStatus(m_bCuffed, m_Cuffer, true);

		EAchievementID achievementID = (Client.Position.Y >= Constants.BorderOfLStoPaleto) ? EAchievementID.WelcomeToPaletoBay : EAchievementID.WelcomeToLosSantos;

		bool bAwarded = AwardAchievement(achievementID);

		// Update blips for new characters owned properties
		PropertyPool.UpdatePlayerPropertyBlips(this);

		CheckForTowedVehicles();

		CheckForDynamicAchievements();
	}

	private void CheckForTowedVehicles()
	{
		int numTowed = 0;

		foreach (CVehicle vehicle in VehiclePool.GetVehiclesFromPlayerOwner(ActiveCharacterDatabaseID))
		{
			if (vehicle.IsTowed())
			{
				++numTowed;
			}
		}

		if (numTowed > 0)
		{
			SendNotification("Vehicle Notice", ENotificationIcon.ExclamationSign, "You have {0} personal vehicles in the towing company's impound. Visit the impound to retrieve your vehicles (Tow icon).", numTowed);
		}
	}

	public bool IsWithinDistanceOf(Vector3 a_Vector, float a_fDistanceThreshold, uint a_Dimension, bool a_bCheckDimension = true)
	{
		float fDist = (Client.Position - a_Vector).Length();
		return fDist <= a_fDistanceThreshold && (!a_bCheckDimension || Client.Dimension == a_Dimension);
	}

	public bool IsWithinDistanceOf(CPlayer a_Player, float a_fDistanceThreshold, bool a_bCheckDimension = true)
	{
		return IsWithinDistanceOf(a_Player.Client.Position, a_fDistanceThreshold, a_Player.Client.Dimension, a_bCheckDimension);
	}

	public bool IsWithinDistanceOf(CVehicle a_Vehicle, float a_fDistanceThreshold, bool a_bCheckDimension = true)
	{
		return IsWithinDistanceOf(a_Vehicle.GTAInstance.Position, a_fDistanceThreshold, a_Vehicle.GTAInstance.Dimension, a_bCheckDimension);
	}

	public bool IsWithinDistanceOf(CWorldItem a_WorldItem, float a_fDistanceThreshold, bool a_bCheckDimension = true)
	{
		return IsWithinDistanceOf(a_WorldItem.GTAInstance.Position, a_fDistanceThreshold, a_WorldItem.GTAInstance.Dimension, a_bCheckDimension);
	}

	private void CheckForDynamicAchievements()
	{
		// MONEY
		float fTotalMoney = Money + BankMoney;

		if (fTotalMoney >= 10000)
		{
			AwardAchievement(EAchievementID.Earn_10kMoney);
		}

		if (fTotalMoney >= 50000)
		{
			AwardAchievement(EAchievementID.Earn_50kMoney);
		}

		if (fTotalMoney >= 100000)
		{
			AwardAchievement(EAchievementID.Earn_100kMoney);
		}

		if (fTotalMoney >= 500000)
		{
			AwardAchievement(EAchievementID.Earn_500kMoney);
		}

		if (fTotalMoney >= 1000000)
		{
			AwardAchievement(EAchievementID.Earn_1mMoney);
		}

		// FACTIONS
		if (IsInFactionOfType(EFactionType.LawEnforcement))
		{
			AwardAchievement(EAchievementID.LEOFaction);
		}

		if (IsInFactionOfType(EFactionType.Medical))
		{
			AwardAchievement(EAchievementID.EMSFaction);
		}

		if (IsInFactionOfType(EFactionType.Criminal))
		{
			AwardAchievement(EAchievementID.CrimeFaction);
		}

		if (IsInFactionOfType(EFactionType.NewsFaction))
		{
			AwardAchievement(EAchievementID.NewsFaction);
		}
	}

	private void OnModelChanged(PedHash oldModel, PedHash newModel)
	{
		// TODO: What do we do about skin components etc on change?
		// Restore weapons
		Inventory.SynchronizeAllWeaponsAndAmmoWithInventory();

	}

	private void CheckForTimePlayedAchievements()
	{
		uint hoursPlayed = MinutesPlayed_Account / 60;
		if (hoursPlayed >= 5)
		{
			AwardAchievement(EAchievementID.Play_5Hours);
		}

		if (hoursPlayed >= 10)
		{
			AwardAchievement(EAchievementID.Play_10Hours);
		}

		if (hoursPlayed >= 50)
		{
			AwardAchievement(EAchievementID.Play_50Hours);
		}

		if (hoursPlayed >= 100)
		{
			AwardAchievement(EAchievementID.Play_100Hours);
		}

		if (hoursPlayed >= 500)
		{
			AwardAchievement(EAchievementID.Play_500Hours);
		}

		if (hoursPlayed >= 1000)
		{
			AwardAchievement(EAchievementID.Play_1000Hours);
		}
	}

	private Vector3 m_vecReconRestorePos = new Vector3();
	private Vector3 m_vecReconRestoreRot = new Vector3();
	private Dimension m_vecReconRestoreDimension = 0;
	private bool m_bIsReconning = false;
	private bool m_bIsDisappeared = false;

	private WeakReference<CPlayer> m_reconTarget = null;
	public bool IsReconningPlayer(CPlayer player)
	{
		return (m_reconTarget != null && m_reconTarget.Instance() == player);
	}

	public void StartRecon(CPlayer reconTarget)
	{
		// cache position
		m_vecReconRestorePos = Client.Position;
		m_vecReconRestoreRot = Client.Rotation;
		m_vecReconRestoreDimension = Client.Dimension;

		Client.Position = reconTarget.Client.Position;

		m_reconTarget = new WeakReference<CPlayer>(reconTarget);
		IsReconning = true;
	}

	public void StopRecon()
	{
		// restore position
		if (IsReconning)
		{
			Client.Position = m_vecReconRestorePos;
			Client.Rotation = m_vecReconRestoreRot;
			SetSafeDimension(m_vecReconRestoreDimension);
			HandleRestoreInterior(false); // restore interior, handles map load etc too

			m_reconTarget = null;
			IsReconning = false;
		}
	}

	public bool IsReconning
	{
		get
		{
			return m_bIsReconning;
		}

		private set
		{
			m_bIsReconning = value;
			SetData(Client, EDataNames.RECON, m_bIsReconning, EDataType.Synced);
		}
	}

	public bool IsDisappeared
	{
		get
		{
			return m_bIsDisappeared;
		}

		set
		{
			m_bIsDisappeared = value;
			SetData(Client, EDataNames.DISAPPEAR, m_bIsDisappeared, EDataType.Synced);
		}
	}

	private bool m_bBleeding = false;
	public bool IsBleeding
	{
		get
		{
			return m_bBleeding;
		}

		set
		{
			m_bBleeding = value;
			SetData(Client, EDataNames.BLEEDING, m_bBleeding, EDataType.Synced);
		}
	}

	private bool m_bBleedingAnim = false;
	public bool HasBleedingAnimation
	{
		get
		{
			return m_bBleedingAnim;
		}

		set
		{
			m_bBleedingAnim = value;
		}
	}

	private Int64 m_LastAdvert;
	public Int64 LastAdvert
	{
		get
		{
			return m_LastAdvert;
		}

		set
		{
			m_LastAdvert = value;
		}
	}


	private bool m_bIsHoldingBoomMic = false;
	private bool m_bIsHoldingCam = false;
	private bool m_bIsHoldingMic = false;
	private bool m_bJoinedTvBroadcast = false;
	private bool m_bBroadcastParticipant = false;

	public bool IsBroadcastParticipant
	{
		get
		{
			return m_bBroadcastParticipant;
		}

		set
		{
			m_bBroadcastParticipant = value;
		}
	}

	public void SetBroadcastParticipant(bool Value)
	{
		m_bBroadcastParticipant = Value;
	}

	public bool IsHoldingCam
	{
		get
		{
			return m_bIsHoldingCam;
		}

		set
		{
			m_bIsHoldingCam = value;
			SetData(Client, EDataNames.NEWS_CAM_HAND, m_bIsHoldingCam, EDataType.Synced);
		}
	}

	public void SetHoldingCam(bool Value = false)
	{
		IsHoldingCam = Value;
	}

	public bool IsHoldingMic
	{
		get
		{
			return m_bIsHoldingMic;
		}

		set
		{
			m_bIsHoldingMic = value;
			SetData(Client, EDataNames.NEWS_MIC, m_bIsHoldingMic, EDataType.Synced);
		}
	}

	public void SetHoldingMic(bool Value = false)
	{
		IsHoldingMic = Value;
	}

	public bool IsHoldingBoomMic
	{
		get
		{
			return m_bIsHoldingBoomMic;
		}

		set
		{
			m_bIsHoldingBoomMic = value;
			SetData(Client, EDataNames.NEWS_BOOM_MIC, m_bIsHoldingBoomMic, EDataType.Synced);
		}
	}

	public void SetHoldingBoomMic(bool Value = false)
	{
		IsHoldingBoomMic = Value;
	}

	public bool JoinedTvBroadcast
	{
		get
		{
			return m_bJoinedTvBroadcast;
		}

		set
		{
			m_bJoinedTvBroadcast = value;
			SetData(Client, EDataNames.JOINED_TV, m_bJoinedTvBroadcast, EDataType.Synced);
		}
	}

	public void SetAsJoinedTvBroadcast(bool Value = false)
	{
		JoinedTvBroadcast = Value;
	}

	private bool m_bToggledBinoculars = false;
	private EBinocularsType m_eBinocularsType = EBinocularsType.None;

	public bool ToggledBinoculars
	{
		get
		{
			return m_bToggledBinoculars;
		}

		set
		{
			m_bToggledBinoculars = value;
			SetData(Client, EDataNames.BINOCULARS, m_bToggledBinoculars, EDataType.Synced);
		}
	}

	public void ToggleBinoculars(bool Value)
	{
		ToggledBinoculars = Value;
	}

	public EBinocularsType CurrentBinocularsType
	{
		get
		{
			return m_eBinocularsType;
		}

		set
		{
			m_eBinocularsType = value;
			SetData(Client, EDataNames.BINOCULARS_TYPE, m_eBinocularsType, EDataType.Synced);
		}
	}

	public void SetBinocularsType(EBinocularsType binocularsType)
	{
		if (ToggledBinoculars)
		{
			CurrentBinocularsType = binocularsType;
		}
		else
		{
			CurrentBinocularsType = EBinocularsType.None;
		}
	}

	private bool m_bIsSmoking = false;
	private ESmokingItemType m_eSmokingItemType = ESmokingItemType.None;

	public bool IsSmoking
	{
		get
		{
			return m_bIsSmoking;
		}

		set
		{
			m_bIsSmoking = value;
			SetData(Client, EDataNames.SMOKING, m_bIsSmoking, EDataType.Synced);
		}
	}

	public ESmokingItemType CurrentSmokingType
	{
		get
		{
			return m_eSmokingItemType;
		}

		set
		{
			m_eSmokingItemType = value;
			SetData(Client, EDataNames.SMOKING_TYPE, m_eSmokingItemType, EDataType.Synced);
		}
	}


	public void SetSmokingOfType(bool bIsSmoking, ESmokingItemType eSmokingItemType)
	{
		IsSmoking = bIsSmoking;
		CurrentSmokingType = eSmokingItemType;
	}


	private bool m_bHasSeatbeltOn = false;

	public bool IsBuckledUp
	{
		get
		{
			return m_bHasSeatbeltOn;
		}

		set
		{
			m_bHasSeatbeltOn = value;
			SetData(Client, EDataNames.SEATBELT, m_bHasSeatbeltOn, EDataType.Synced);
		}
	}

	private bool m_bStartedCoinFlip = false;

	public bool StartedCoinFlip
	{
		get
		{
			return m_bStartedCoinFlip;
		}

		set
		{
			m_bStartedCoinFlip = value;
			SetData(Client, EDataNames.COINFLIP, m_bStartedCoinFlip, EDataType.Synced);
		}
	}

	private bool m_bLocksmithPendingPickup = false;

	public bool LocksmithPendingPickup
	{
		get
		{
			return m_bLocksmithPendingPickup;
		}

		set
		{
			m_bLocksmithPendingPickup = value;
			SetData(Client, EDataNames.LOCKSMITH_PENDING_PICKUP, m_bLocksmithPendingPickup, EDataType.Synced);
		}
	}

	public void SetLocksmithPendingPickup(bool bSetPickup = true)
	{
		LocksmithPendingPickup = bSetPickup;
		IsLocksmithMessageSent = bSetPickup;
	}

	private bool bLocksmithMessageSent = false;

	public bool IsLocksmithMessageSent
	{
		get
		{
			return bLocksmithMessageSent;
		}

		set
		{
			bLocksmithMessageSent = value;
		}
	}

	public void StartCoinFlip(bool bStart = true, bool bCoinMovement = false)
	{
		StartedCoinFlip = bStart;

		if (!bStart)
		{
			StopCurrentAnimation(true, true);
		}

		int gAnimFlags = (int)(AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl | AnimationFlags.StopOnLastFrame);
		if (bStart && !bCoinMovement)
		{
			AddAnimationToQueue(gAnimFlags, "anim@mp_player_intuppercoin_roll_and_toss", "enter", false, true, true, 3200, false);
			AddAnimationToQueue(gAnimFlags, "anim@mp_player_intuppercoin_roll_and_toss", "idle_a", false, false, true, 3000, false);
			AddAnimationToQueue(gAnimFlags, "anim@mp_player_intcelebrationmale@coin_roll_and_toss", "coin_roll_and_toss", false, false, true, 3000, false);
		}

		if (bStart && bCoinMovement)
		{
			int animFlags = (int)(AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl | AnimationFlags.Loop);
			AddAnimationToQueue(gAnimFlags, "anim@mp_player_intuppercoin_roll_and_toss", "enter", false, true, true, 3200, false);
			AddAnimationToQueue(animFlags, "anim@mp_player_intuppercoin_roll_and_toss", "idle_a", false, false, true, 10000 * 10000, false);
		}
	}

	private uint m_MinutesPlayed_Account = 0;
	public uint MinutesPlayed_Account
	{
		get => m_MinutesPlayed_Account;
		set
		{
			if (IsSpawned)
			{
				uint diff = value - m_MinutesPlayed_Account;
				Database.Functions.Accounts.UpdateMinutesPlayed(AccountID, diff);
			}

			m_MinutesPlayed_Account = value;

			CheckForTimePlayedAchievements();
		}
	}

	private uint m_MinutesPlayed_Character = 0;
	public uint MinutesPlayed_Character
	{
		get => m_MinutesPlayed_Character;
		set
		{
			if (IsSpawned)
			{
				uint diff = value - m_MinutesPlayed_Character;
				Database.Functions.Characters.UpdateMinutesPlayed(ActiveCharacterDatabaseID, diff);
			}

			m_MinutesPlayed_Character = value;
			SetData(m_Client, EDataNames.MINUTES_PLAYED, value, EDataType.Synced);
		}
	}
	public void Save(object[] a_Parameters = null)
	{
		if (IsSpawned && m_vecCachedSavePosition != null)
		{
			// TODO_OPTIMIZATION: Do we want to flatten our queries? saving a char is > 3 queries
			Database.Functions.Characters.Save(m_DatabaseID, m_vecCachedSavePosition, m_fCachedSaveRotation, m_Client.Health, m_Client.Armor, m_CachedDimension);

			Database.Functions.Characters.SetPaydayProgress(ActiveCharacterDatabaseID, PaydayProgress);

			m_Inventory.SyncInventoryAmmoWithWeaponAmmoAndSave();

			SaveDrugEffects();

			if (AdminJailMinutesLeft > -1)
			{
				RecalculateAdminJailMinutesLeft();
				Database.Functions.Accounts.SetAdminJailInformation(AccountID, AdminJailMinutesLeft, AdminJailReason, () => { });
			}

			// Update minutes played
			TimeSpan TimeSinceLastUpdatedMinutesPlayed = DateTime.Now - m_LastUpdateMinutesPlayed;
			if (TimeSinceLastUpdatedMinutesPlayed.TotalMinutes >= 1) // only reset if we actually did something, otherwise keep the original timestamp
			{
				m_LastUpdateMinutesPlayed = DateTime.Now;
				uint Gain = Convert.ToUInt32(TimeSinceLastUpdatedMinutesPlayed.TotalMinutes);
				Database.Functions.Accounts.UpdateMinutesPlayed(AccountID, Gain);
				Database.Functions.Characters.UpdateMinutesPlayed(ActiveCharacterDatabaseID, Gain);

				m_MinutesPlayed_Account += Gain;
				m_MinutesPlayed_Character += Gain;

				// We can also pulse the achievement here, it could be delayed, but that's fine
				CheckForTimePlayedAchievements();
			}

			// Save their current vehicle too to try and keep them in sync
			if (Client.Vehicle != null)
			{
				CVehicle currentVehicle = VehiclePool.GetVehicleFromGTAInstance(Client.Vehicle);

				if (currentVehicle != null)
				{
					currentVehicle.Save();
				}
			}
		}
	}

	private void SaveDrugEffects()
	{
		float fImpairment = GetData<float>(m_Client, EDataNames.IMPAIRMENT);
		bool bDrugFX1 = GetData<bool>(m_Client, EDataNames.DRUG_FX_1);
		bool bDrugFX2 = GetData<bool>(m_Client, EDataNames.DRUG_FX_2);
		bool bDrugFX3 = GetData<bool>(m_Client, EDataNames.DRUG_FX_3);
		bool bDrugFX4 = GetData<bool>(m_Client, EDataNames.DRUG_FX_4);
		bool bDrugFX5 = GetData<bool>(m_Client, EDataNames.DRUG_FX_5);

		Int64 DrugFX1_Duration = GetRemainingDrugEffectDuration(EDrugEffect.Weed);
		Int64 DrugFX2_Duration = GetRemainingDrugEffectDuration(EDrugEffect.Meth);
		Int64 DrugFX3_Duration = GetRemainingDrugEffectDuration(EDrugEffect.Cocaine);
		Int64 DrugFX4_Duration = GetRemainingDrugEffectDuration(EDrugEffect.Heroin);
		Int64 DrugFX5_Duration = GetRemainingDrugEffectDuration(EDrugEffect.Xanax);

		Database.Functions.Characters.SaveDrugEffects(m_DatabaseID, fImpairment, bDrugFX1, bDrugFX2, bDrugFX3, bDrugFX4, bDrugFX5, DrugFX1_Duration, DrugFX2_Duration, DrugFX3_Duration, DrugFX4_Duration, DrugFX5_Duration);
	}

	public void UpdateTimePlayed(object[] a_Parameters)
	{
		PaydayProgress++;
	}


	public void ReapplyDrugAndImpairmentFromDB(SGetCharacter RetrieveCharacterResult)
	{
		ImpairmentLevel = RetrieveCharacterResult.Impairment;

		// TODO_POST_LAUNCH: Helper function to map FX number to EDrugEffect
		if (RetrieveCharacterResult.DrugFX1)
		{
			SetDrugEffectEnabled(EDrugEffect.Weed, RetrieveCharacterResult.DrugFX1_Duration);
		}

		if (RetrieveCharacterResult.DrugFX2)
		{
			SetDrugEffectEnabled(EDrugEffect.Meth, RetrieveCharacterResult.DrugFX2_Duration);
		}

		if (RetrieveCharacterResult.DrugFX3)
		{
			SetDrugEffectEnabled(EDrugEffect.Cocaine, RetrieveCharacterResult.DrugFX3_Duration);
		}

		if (RetrieveCharacterResult.DrugFX4)
		{
			SetDrugEffectEnabled(EDrugEffect.Heroin, RetrieveCharacterResult.DrugFX4_Duration);
		}

		if (RetrieveCharacterResult.DrugFX5)
		{
			SetDrugEffectEnabled(EDrugEffect.Xanax, RetrieveCharacterResult.DrugFX1_Duration);
		}
	}

	public void SyncInventoryWithWeaponsTick(object[] a_Parameters = null)
	{
		m_Inventory.SyncInventoryAmmoWithWeaponAmmoAndSave();
	}

	public void OnWeaponSwitch(WeaponHash oldWeapon, WeaponHash newWeapon)
	{
		SyncInventoryWithWeaponsTick();

		// set if the current weapon is semi auto
		EItemID itemID = EItemID.None;
		foreach (var kvPair in ItemWeaponDefinitions.g_DictItemIDToWeaponHash)
		{
			if (kvPair.Value == newWeapon)
			{
				itemID = kvPair.Key;
				break;
			}
		}

		if (itemID != EItemID.None)
		{
			CItemInstanceDef itemDef = Inventory.GetFirstItemOfID(itemID);
			if (itemDef != null)
			{
				CItemValueBasic itemValue = (CItemValueBasic)itemDef.Value;
				NetworkEventSender.SendNetworkEvent_InformPlayerOfFireModes(this, itemValue.semi_auto);
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_InformPlayerOfFireModes(this, true);
			}

		}
		else
		{
			NetworkEventSender.SendNetworkEvent_InformPlayerOfFireModes(this, true);
		}

	}

	public void SaveInventoryTick(object[] a_Parameters = null)
	{
		m_Inventory.SyncInventoryAmmoWithWeaponAmmoAndSave();
	}

	public void HandleRestoreInterior(bool bDontSetPosition, bool bOverrideProperty = false, Dimension overrideDimension = 0)
	{
		// Override dimension is so we can force a player to load their DB dimension (for map purposes), but without actually transporting them to a real, live dimension where other players see them
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(bOverrideProperty ? overrideDimension : Client.Dimension);
		if (propertyInst != null)
		{
			OnEnterProperty(propertyInst, bOverrideProperty, bDontSetPosition);
		}
	}

	private void HandleIPL()
	{
		CPropertyInstance propertyInst = PropertyPool.GetPropertyInstanceFromID(Client.Dimension);
		if (propertyInst != null)
		{
			LoadIPLForProperty(propertyInst);
		}
	}

	public bool IsInFaction(Int64 a_FactionID)
	{
		foreach (CFactionMembership factionMembership in m_lstFactionMemberships)
		{
			if (factionMembership.Faction.FactionID == a_FactionID)
			{
				return true;
			}
		}

		return false;
	}

	public bool IsFactionManager(Int64 a_FactionID)
	{
		foreach (CFactionMembership factionMembership in m_lstFactionMemberships)
		{
			if (factionMembership.Faction.FactionID == a_FactionID && factionMembership.Manager)
			{
				return true;
			}
		}

		return false;
	}

	public bool IsInGovernmentFaction()
	{
		return IsInFactionOfType(EFactionType.LawEnforcement) || IsInFactionOfType(EFactionType.Medical) || IsInFactionOfType(EFactionType.Government);
	}

	public bool IsInFactionOfType(EFactionType a_FactionType)
	{
		foreach (CFactionMembership factionMembership in m_lstFactionMemberships)
		{
			if (factionMembership.Faction.Type == a_FactionType)
			{
				return true;
			}
		}

		return false;
	}

	public Player Client => m_Client;

	public bool IsLoggedIn => m_bIsLoggedIn;

	public int AccountID => m_AccountID;

	public string Username => m_Username;

	public CPlayerInventory Inventory => m_Inventory;

	public CDonationInventory DonationInventory => m_DonationInventory;

	public PersistentNotificationManager Notifications => m_Notifications;

	public bool GlobalOocEnabled { get; set; } = true;

	public bool IsCrouched { get; set; } = false;

	public List<CreditDetails> GetActiveCreditDetails()
	{
		List<CreditDetails> lstDetails = new List<CreditDetails>();

		// Vehicles
		List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromPlayer(this);
		foreach (CVehicle vehicle in lstVehicles)
		{
			// Do we still have outstanding payments?
			if (vehicle.PaymentsRemaining > 0)
			{
				lstDetails.Add(new CreditDetails(vehicle.GetFullDisplayName(), vehicle.PaymentsMade, vehicle.PaymentsRemaining, vehicle.GetRemainingCredit(true), vehicle.GetRemainingCreditInterest(), ECreditType.Vehicle, vehicle.m_DatabaseID));
			}
		}

		// Properties
		List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByPlayer(this);

		foreach (CPropertyInstance propertyInst in lstProperties)
		{
			// Do we still have outstanding payments?
			if (propertyInst.Model.PaymentsRemaining > 0)
			{
				lstDetails.Add(new CreditDetails(propertyInst.Model.Name, propertyInst.Model.PaymentsMade, propertyInst.Model.PaymentsRemaining, propertyInst.GetRemainingCredit(true), propertyInst.GetRemainingCreditInterest(), ECreditType.Property, propertyInst.Model.Id));
			}
		}

		return lstDetails;
	}

	public void GetActiveCredit(out float TotalCreditVehicles, out int NumVehiclesOnCredit, out float TotalCreditProperties, out int NumPropertiesOnCredit)
	{
		TotalCreditVehicles = 0.0f;
		NumVehiclesOnCredit = 0;
		TotalCreditProperties = 0.0f;
		NumPropertiesOnCredit = 0;

		// Vehicles
		List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromPlayer(this);
		foreach (CVehicle vehicle in lstVehicles)
		{
			// Do we still have outstanding payments?
			if (vehicle.PaymentsRemaining > 0)
			{
				TotalCreditVehicles += vehicle.GetRemainingCredit();
				++NumVehiclesOnCredit;
			}
		}

		// Properties
		List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByPlayer(this);

		foreach (CPropertyInstance propertyInst in lstProperties)
		{
			// Do we still have outstanding payments?
			if (propertyInst.Model.PaymentsRemaining > 0)
			{
				TotalCreditProperties += propertyInst.GetRemainingCredit();
				++NumPropertiesOnCredit;
			}
		}
	}

	public float GetMonthlyExpenses()
	{
		float fTotalExpenses = 0.0f;

		// Vehicles
		List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromPlayer(this);
		foreach (CVehicle vehicle in lstVehicles)
		{
			// Do we still have outstanding payments?
			if (vehicle.PaymentsRemaining > 0)
			{
				fTotalExpenses += vehicle.GetMonthlyPaymentAmount();
			}

			float fMonthlyTax = Taxation.GetVehicleMonthlyTaxRate(vehicle.GTAInstance.Class);

			// Always take tax
			fTotalExpenses += fMonthlyTax;
		}

		// Properties
		List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByPlayer(this);

		foreach (CPropertyInstance propertyInst in lstProperties)
		{
			// Do we still have outstanding payments?
			if (propertyInst.Model.PaymentsRemaining > 0)
			{
				fTotalExpenses += propertyInst.GetMonthlyPaymentAmount();
			}

			// Property tax for this property (always taken)
			float fMonthlyTax = propertyInst.GetMonthlyTax();

			// Always take tax
			fTotalExpenses += fMonthlyTax;
		}

		return fTotalExpenses;
	}

	public float GetMonthlyIncome()
	{
		CalculatePayCheck(out List<CPaycheckEntry> lstPaycheckEntries, false);

		float fTotalIncome = 0.0f;
		foreach (CPaycheckEntry paycheckEntry in lstPaycheckEntries)
		{
			fTotalIncome += paycheckEntry.GrossIncome;
		}

		return fTotalIncome;
	}

	public bool CanPlayerAffordMonthlyExpense(float fCost)
	{
		// Must have a 10% overhead
		float fCurrentExpenses = GetMonthlyExpenses();
		float fCurrentIncome = GetMonthlyIncome();
		float fCurrentIncomeMinusOverhead = fCurrentIncome - (fCurrentIncome * 0.10f); // Subtract 10% from salary to allow overhead
		float fAvailableFunds = fCurrentIncomeMinusOverhead - fCurrentExpenses;

		return fCost <= fAvailableFunds;
	}

	public bool CanPlayerAffordBankCost(float fCost)
	{
		return fCost <= m_fBankMoney;
	}

	public bool SubtractBankBalanceIfCanAfford(float fCost, PlayerMoneyModificationReason reason)
	{
		if (CanPlayerAffordBankCost(fCost))
		{
			// This also saves it
			RemoveBankMoney(fCost, reason);
			return true;
		}

		return false;
	}

	public bool CanPlayerAffordCost(float fCost)
	{
		return fCost <= m_fMoney;
	}

	public bool SubtractMoney(float fCost, PlayerMoneyModificationReason reason)
	{
		if (CanPlayerAffordCost(fCost))
		{
			// This also saves it
			RemoveMoney(fCost, reason);
			return true;
		}

		return false;
	}

	public void SubtractMoneyAllowNegative(float fCost, PlayerMoneyModificationReason reason)
	{
		// This also saves it
		RemoveMoney(fCost, reason);
	}

	public void SetLoggedIn(int AccountID = 0, bool bLoggedIn = false, EAdminLevel a_AdminLevel = EAdminLevel.None, EScripterLevel a_ScripterLevel = EScripterLevel.None, string Username = "", EApplicationState appState = EApplicationState.NoApplicationSubmitted, UInt32 numApps = 0, UInt64 discordID = 0, Int64 a_autoSpawnCharacter = -1, int a_AdminReportCount = 0, bool a_bLocalPlayerNametagToggled = false)
	{
		// TODO_LAUNCH: Reset a bunch of stuff, perhaps just recreate CPlayer?

		m_ApplicationState = appState;
		NumberOfApplicationsSubmitted = numApps;

		SetData(m_Client, EDataNames.IS_LOGGED_IN, bLoggedIn, EDataType.Synced);
		SetData(m_Client, EDataNames.USERNAME, Username, EDataType.Synced); // FOR THE PLAYER LIST
		m_bIsLoggedIn = bLoggedIn;

		SetData(m_Client, EDataNames.ACCOUNT_ID, AccountID, EDataType.Synced);
		m_AccountID = AccountID;
		AutoSpawnCharacter = a_autoSpawnCharacter;

		AdminLevel = a_AdminLevel;
		ScripterLevel = a_ScripterLevel;
		m_AdminReportCount = a_AdminReportCount;


		m_Username = Username;
		m_DiscordID = discordID;
		LocalNametagToggled = a_bLocalPlayerNametagToggled;

		// If not logged in, we can't be spawned
		if (!bLoggedIn)
		{
			IsSpawned = false;
		}

		// TODO_LAUNCH: Load from db
		if (a_AdminLevel != EAdminLevel.None)
		{
			AdminDuty = true;
		}
		else // must set false for entity data
		{
			AdminDuty = false;
		}

		PlayerPool.SetPlayerAsInGame(this, IsInGame());
	}

	public bool IsInGame()
	{
		return IsLoggedIn && IsSpawned;
	}

	public bool IsSpawned
	{
		get => m_bIsSpawned;
		set
		{
			SetData(m_Client, EDataNames.IS_SPAWNED, value, EDataType.Synced);
			m_bIsSpawned = value;

			PlayerPool.SetPlayerAsInGame(this, IsInGame());
		}
	}


	// ANTICHEAT DATA
	public bool HasTemporaryImmunityAgainstWeaponGrantHacks()
	{
		return TemporaryImmunityAgainstWeaponGrantHacks;
	}

	public void GrantTemporaryImmunityAgainstWeaponGrantHacks()
	{
		if (!TemporaryImmunityAgainstWeaponGrantHacks)
		{
			TemporaryImmunityAgainstWeaponGrantHacks = true;

			MainThreadTimerPool.CreateEntityTimer((object[] parameters) =>
			{
				TemporaryImmunityAgainstWeaponGrantHacks = false;
			}, 5000, this, 1);
		}
	}

	private bool TemporaryImmunityAgainstWeaponGrantHacks = false;

	/// <summary>
	/// Checks if the Player is an admin at a specific level and on duty
	/// </summary>
	/// <param name="level">The minimum level of admin they must be</param>
	/// <param name="onDuty">True checks if the admin is on dutyy</param>
	/// <returns>True if Player is an admin based on the conditions above</returns>
	public bool IsAdmin(EAdminLevel level = EAdminLevel.TrialAdmin, bool checkOnDuty = false)
	{
		return m_AdminLevel >= level && (checkOnDuty ? AdminDuty : true);
	}

	public bool IsScripter(EScripterLevel level = EScripterLevel.TrialScripter)
	{
		return m_ScripterLevel >= level;
	}

	private bool m_bIsPremadeMasked = false;

	public async void SetPremadeMasked(bool bMasked, bool bSaveToDB)
	{
		// Never premade mask a custom char! It could break things, like when they upgrade premade -> custom
		if (CharacterType == ECharacterType.Custom)
		{
			bMasked = false;
		}

		m_bIsPremadeMasked = bMasked;

		if (bSaveToDB)
		{
			await Database.LegacyFunctions.SetCharacterPremadeMasked(ActiveCharacterDatabaseID, bMasked).ConfigureAwait(true);
		}

		UpdateMaskState();
	}

	public void StartVehicleEnterTimer()
	{
		m_VehicleEnterTimer = MainThreadTimerPool.CreateEntityTimer(EnterVehicleTimerTick, 2500, this);
	}

	private void EnterVehicleTimerTick(object[] a_Parameters)
	{
		MainThreadTimerPool.MarkTimerForDeletion(m_VehicleEnterTimer);

		IsEnteringVehicle = false;
	}

	public bool IsInVehicleReal
	{
		get => m_IsInVehicleReal;
		set
		{
			m_IsInVehicleReal = value;

			// Clear animations
			StopCurrentAnimation(true, true);
		}
	}

	public bool IsEnteringVehicle
	{
		get => m_IsEnteringVehicle;
		set => m_IsEnteringVehicle = value;
	}

	private bool m_IsInVehicleReal = false;
	private bool m_IsEnteringVehicle = false;

	public bool AdminDuty
	{
		get => m_AdminDuty;
		set
		{
			SetData(m_Client, EDataNames.ADMIN_DUTY, value, EDataType.Synced);
			m_AdminDuty = value;
		}
	}

	public EScripterLevel ScripterLevel
	{
		get => m_ScripterLevel;
		set
		{
			m_ScripterLevel = value;
			SetData(m_Client, EDataNames.SCRIPTER_LEVEL, value, EDataType.Synced);
		}
	}

	public EAdminLevel AdminLevel
	{
		get => m_AdminLevel;
		set
		{
			SetData(m_Client, EDataNames.ADMIN_LEVEL, value, EDataType.Synced);
			m_AdminLevel = value;
		}
	}

	public int AdminReportCount
	{
		get
		{
			return m_AdminReportCount;
		}

		set
		{
			m_AdminReportCount = value;
		}
	}

	public string AdminTitle => Helpers.GetAdminLevelName(m_AdminLevel);

	public string ScripterTitle => Helpers.GetScripterLevelName(m_ScripterLevel);

	public EDutyType DutyType
	{
		get => m_DutyType;
		set
		{
			SetData(m_Client, EDataNames.DUTY, value, EDataType.Synced);
			m_DutyType = value;
		}
	}

	public void SendNotification(string a_strTitle, ENotificationIcon icon, string a_strMessageFormat, params object[] a_MessageParameters)
	{
		try
		{
			if (a_MessageParameters != null && a_MessageParameters.Length > 0)
			{
				a_strMessageFormat = Helpers.FormatString(a_strMessageFormat, a_MessageParameters);
			}

			NetworkEventSender.SendNetworkEvent_Notification(this, a_strTitle.Replace("\"", "'"), a_strMessageFormat.Replace("\"", "'"), icon);
		}
		catch
		{
			// TODO_LAUNCH_BUGS: Work out why this occurs
		}
	}

	public void HandlePendingWeaponLicenseStates(EPendingFirearmLicenseState pendingFirearmsLicenseStateTier1, EPendingFirearmLicenseState pendingFirearmsLicenseStateTier2)
	{
		if (pendingFirearmsLicenseStateTier2 == EPendingFirearmLicenseState.Issued_PendingPickup)
		{
			SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You have been approved for a Tier 2 Firearms License.<br><br>Please visit the LSPD Front Desk to receive your permit.");
		}

		if (pendingFirearmsLicenseStateTier1 == EPendingFirearmLicenseState.Issued_PendingPickup)
		{
			SendNotification("Los Santos Police Department", ENotificationIcon.Star, "You have been approved for a Tier 1 Firearms License.<br><br>Please visit the LSPD Front Desk to receive your permit.");
		}

		if (pendingFirearmsLicenseStateTier1 == EPendingFirearmLicenseState.Revoked)
		{
			if (HasHandgunFirearmLicense())
			{
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER1, ActiveCharacterDatabaseID);
				bool bRemoved = Inventory.RemoveItemFromBasicDefinition(ItemInstanceDef, true);
				if (bRemoved)
				{
					Database.Functions.Characters.SetTier1PendingLicenseState(ActiveCharacterDatabaseID, EPendingFirearmLicenseState.None);
					SendNotification("Los Santos Police Department", ENotificationIcon.Star, "Your Tier 1 Firearms License was revoked by the LSPD.");
				}
			}
		}

		if (pendingFirearmsLicenseStateTier2 == EPendingFirearmLicenseState.Revoked)
		{
			if (HasLargeFirearmLicense())
			{
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER2, ActiveCharacterDatabaseID);
				bool bRemoved = Inventory.RemoveItemFromBasicDefinition(ItemInstanceDef, true);
				if (bRemoved)
				{
					Database.Functions.Characters.SetTier2PendingLicenseState(ActiveCharacterDatabaseID, EPendingFirearmLicenseState.None);
					SendNotification("Los Santos Police Department", ENotificationIcon.Star, "Your Tier 2 Firearms License was revoked by the LSPD.");
				}
			}
		}
	}

	public void GoOffDuty()
	{
		// armor
		if (m_DutyType == EDutyType.Law_Enforcement || m_DutyType == EDutyType.Fire || m_DutyType == EDutyType.EMS)
		{
			// cache it, this gets applied on skin change
			m_cachedHealth = 100;
			m_cachedArmor = 0;
		}

		Inventory.RemoveAllDutyItems();

		SendNotification("Duty", ENotificationIcon.InfoSign, "You are now off duty.", null);
		DutyType = EDutyType.None;

		Database.LegacyFunctions.SetPlayerDuty(ActiveCharacterDatabaseID, EDutyType.None);

		ClearBackupBeacon();
		ClearUnitNumber();

		// Inform all govt players
		List<CFaction> lstGovtFactions = FactionPool.GetGovernmentFactions();
		foreach (CFaction govtFaction in lstGovtFactions)
		{
			List<CPlayer> lstFactionMembers = govtFaction.GetMembers();

			foreach (CPlayer factionMember in lstFactionMembers)
			{
				if (factionMember.IsOnDuty() || factionMember == this)
				{
					NetworkEventSender.SendNetworkEvent_PlayerWentOffDuty(factionMember, Client);
				}
			}
		}

		SetLEOBadgeState(false);
		ApplySkinFromInventory();
	}

	private bool m_bBadgeEnabled = false;
	public bool BadgeEnabled
	{
		get
		{
			return m_bBadgeEnabled;
		}

		set
		{
			m_bBadgeEnabled = value;
			SetData(m_Client, EDataNames.BADGE_ENABLED, m_bBadgeEnabled, EDataType.Synced);
		}
	}

	public void SetBadge(bool bEnable, System.Drawing.Color badgeColor, string badgeShortFactionName = null, string badgeText = null)
	{
		if (bEnable)
		{
			SetData(m_Client, EDataNames.BADGE_FACTION_NAME, badgeShortFactionName, EDataType.Synced);
			SetData(m_Client, EDataNames.BADGE_NAME, badgeText, EDataType.Synced);
			SetData(m_Client, EDataNames.BADGE_COLOR_R, badgeColor.R, EDataType.Synced);
			SetData(m_Client, EDataNames.BADGE_COLOR_G, badgeColor.G, EDataType.Synced);
			SetData(m_Client, EDataNames.BADGE_COLOR_B, badgeColor.B, EDataType.Synced);

		}
		else
		{
			ClearData(m_Client, EDataNames.BADGE_FACTION_NAME);
			ClearData(m_Client, EDataNames.BADGE_NAME);
			ClearData(m_Client, EDataNames.BADGE_COLOR_R);
			ClearData(m_Client, EDataNames.BADGE_COLOR_G);
			ClearData(m_Client, EDataNames.BADGE_COLOR_B);
		}

		BadgeEnabled = bEnable;
	}

	private bool LocalNametagToggled
	{
		get
		{
			return m_localPlayerNametagToggled;
		}

		set
		{
			m_localPlayerNametagToggled = value;
			SetData(m_Client, EDataNames.LOCALPLAYER_NAMETAG_TOGGLED, m_localPlayerNametagToggled, EDataType.Synced);
		}
	}

	public async Task ToggleLocalNametag()
	{
		LocalNametagToggled = LocalNametagToggled ? false : true;
		await Database.LegacyFunctions.ToggleLocalPlayerNametag(this.AccountID, LocalNametagToggled).ConfigureAwait(true);
	}

	public void SetLEOBadgeState(bool bState)
	{
		SetData(Client, EDataNames.LEO_BADGE, bState, EDataType.Synced);
	}

	public async void GoOnDuty(EDutyType a_Type, CItemInstanceDef outfitDef, bool a_bIsRestoreFromDB)
	{
		// TODO: Make this system generic. Lots of copy paste code below
		if (outfitDef != null && a_Type != EDutyType.None)
		{
			ActivateDutyOutfit(a_Type, outfitDef);

			bool bApply = true;
			if (bApply)
			{
				DutyType = a_Type;
				ApplyDutySkin(a_Type);

				if (!a_bIsRestoreFromDB)
				{
					// duty type specific grants (give thse first since its typically containers)
					// bulk add (required so we can chain the next step that needs these containers to exist and have DBIDs)
					Dictionary<CItemInstanceDef, EItemID> dictBulkItems = new Dictionary<CItemInstanceDef, EItemID>();

					if (DutyCustomSkins_Server.DutyTypeSpecificGrants.ContainsKey(a_Type))
					{
						foreach (DutyItemRelatedGrant dutyTypeGrant in DutyCustomSkins_Server.DutyTypeSpecificGrants[a_Type])
						{
							CItemInstanceDef ItemInstanceToAdd = CItemInstanceDef.FromBasicValueNoDBID(dutyTypeGrant.ItemID, dutyTypeGrant.Value, dutyTypeGrant.Stack);
							((CItemValueBasic)ItemInstanceToAdd.Value).duty = true;
							((CItemValueBasic)ItemInstanceToAdd.Value).is_legal = true;
							((CItemValueBasic)ItemInstanceToAdd.Value).semi_auto = false; // only for weapons, but we can set it anyway

							dictBulkItems.Add(ItemInstanceToAdd, dutyTypeGrant.ContainerToBindTo);
						}
					}
					Inventory.BulkAddIndependentItemsToNextFreeSuitableSlotsSynchronously(dictBulkItems, EShowInventoryAction.DoNothing, (Dictionary<CItemInstanceDef, bool> dictResults) =>
					{
						dictBulkItems.Clear();

						// give loadout (we only do this on GoOnDuty and not ApplyDutySkin since those are on spawn etc too)
						CItemValueDutyOutfit itemVal = (CItemValueDutyOutfit)outfitDef.Value;
						foreach (var loadoutItem in itemVal.Loadout)
						{
							EDutyWeaponSlot slot = (EDutyWeaponSlot)loadoutItem.Key;
							EItemID itemID = (EItemID)loadoutItem.Value;

							// give base item
							if (DutyCustomSkins_Server.DutyServersideGrants.ContainsKey(itemID))
							{
								DutyItemGrant grant = DutyCustomSkins_Server.DutyServersideGrants[itemID];

								// overrides for explicit slots
								EItemID containerToBindTo = grant.ContainerToBindTo;
								if (slot == EDutyWeaponSlot.HandgunLegHolster)
								{
									containerToBindTo = EItemID.HOLSTER_LEG;
								}
								else if (slot == EDutyWeaponSlot.HandgunHipHolster)
								{
									containerToBindTo = EItemID.HOLSTER;
								}

								CItemInstanceDef ItemInstanceToAdd = CItemInstanceDef.FromBasicValueNoDBID(itemID, grant.Value, grant.Stack);
								((CItemValueBasic)ItemInstanceToAdd.Value).duty = true;
								((CItemValueBasic)ItemInstanceToAdd.Value).is_legal = true;
								((CItemValueBasic)ItemInstanceToAdd.Value).semi_auto = false; // only for weapons, but we can set it anyway
								dictBulkItems.Add(ItemInstanceToAdd, containerToBindTo);
							}
						}

						Inventory.BulkAddIndependentItemsToNextFreeSuitableSlotsSynchronously(dictBulkItems, EShowInventoryAction.DoNothing, (Dictionary<CItemInstanceDef, bool> dictResults) =>
						{
							dictBulkItems.Clear();

							// give related items
							foreach (var kvPair in dictResults)
							{
								EItemID itemID = kvPair.Key.ItemID;

								if (DutyCustomSkins_Server.DutyServersideRelatedGrants.ContainsKey(itemID))
								{
									foreach (DutyItemRelatedGrant relatedGrant in DutyCustomSkins_Server.DutyServersideRelatedGrants[itemID])
									{
										CItemInstanceDef RelatedItemInstanceToAdd = CItemInstanceDef.FromBasicValueNoDBID(relatedGrant.ItemID, relatedGrant.Value, relatedGrant.Stack);
										((CItemValueBasic)RelatedItemInstanceToAdd.Value).duty = true;
										((CItemValueBasic)RelatedItemInstanceToAdd.Value).is_legal = true;
										((CItemValueBasic)RelatedItemInstanceToAdd.Value).semi_auto = false; // only for weapons, but we can set it anyway

										dictBulkItems.Add(RelatedItemInstanceToAdd, relatedGrant.ContainerToBindTo);
									}
								}
							}

							Inventory.BulkAddIndependentItemsToNextFreeSuitableSlotsSynchronously(dictBulkItems, EShowInventoryAction.DoNothing, null);
						});
					});
				}

				// Only save in DB if it wasn't being restored from DB or application failed (to reset to zer)
				if (!a_bIsRestoreFromDB || !bApply)
				{
					await Database.LegacyFunctions.SetPlayerDuty(ActiveCharacterDatabaseID, bApply ? DutyType : EDutyType.None).ConfigureAwait(true);
				}
			}

			if (bApply && a_Type != EDutyType.News)
			{
				// Set random unit number
				int randomUnitNumber = new Random().Next(1, 99);
				SetUnitNumber(randomUnitNumber);

				SendNotification("Duty", ENotificationIcon.InfoSign, "You were assigned a randomized unit number of {0}. You can change it using /setunitnumber", randomUnitNumber);
			}

			// Inform all govt players
			if (bApply && a_Type != EDutyType.News)
			{
				List<CFaction> lstGovtFactions = FactionPool.GetGovernmentFactions();
				foreach (CFaction govtFaction in lstGovtFactions)
				{
					List<CPlayer> lstFactionMembers = govtFaction.GetMembers();

					foreach (CPlayer factionMember in lstFactionMembers)
					{
						if (factionMember.IsOnDuty() || factionMember == this)
						{
							NetworkEventSender.SendNetworkEvent_PlayerWentOnDuty(factionMember, this.Client, DutyType);
						}
					}
				}
			}

			// armor
			if (a_Type == EDutyType.Law_Enforcement || a_Type == EDutyType.Fire || a_Type == EDutyType.EMS)
			{
				// cache it, this gets applied on skin change
				m_cachedHealth = 100;
				m_cachedArmor = 100;
			}

			// save
			Save();
		}
	}

	private async void OnApplyDutyFailed()
	{
		DutyType = EDutyType.None;
		ApplySkinFromInventory();
		await Database.LegacyFunctions.SetPlayerDuty(ActiveCharacterDatabaseID, EDutyType.None).ConfigureAwait(true);
	}

	public bool HasHat()
	{
		string propModelsJson = GetData<string>(m_Client, EDataNames.PROP_MODELS);

		if (propModelsJson != null)
		{
			Dictionary<int, int> models = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(propModelsJson);
			return models[(int)ECustomPropSlot.Hats] != 0;
		}

		return false;
	}

	public bool HasGlasses()
	{
		string propModelsJson = GetData<string>(m_Client, EDataNames.PROP_MODELS);

		if (propModelsJson != null)
		{
			Dictionary<int, int> models = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(propModelsJson);
			return models[(int)ECustomPropSlot.Glasses] != 0;
		}

		return false;
	}

	public Dictionary<int, int> GetCurrentPropDrawables()
	{
		string propModelsJson = GetData<string>(m_Client, EDataNames.PROP_MODELS);
		if (propModelsJson != null)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(propModelsJson);
		}

		return new Dictionary<int, int>();
	}

	public Dictionary<int, int> GetCurrentPropTextures()
	{
		string propTexturesJson = GetData<string>(m_Client, EDataNames.PROP_TEXTS);

		if (propTexturesJson != null)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(propTexturesJson);
		}

		return new Dictionary<int, int>();
	}

	public void SyncPlayerAccesory(ECustomPropSlot slot, int model, int texture)
	{
		string propModelsJson = GetData<string>(m_Client, EDataNames.PROP_MODELS);
		string propTexturesJson = GetData<string>(m_Client, EDataNames.PROP_TEXTS);

		if (propModelsJson != null && propTexturesJson != null)
		{
			Dictionary<int, int> models = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(propModelsJson);
			Dictionary<int, int> textures = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(propTexturesJson);

			models[(int)slot] = model;
			textures[(int)slot] = texture;

			SyncPlayerAccessories(models, textures);
		}
	}

	// TODO_RAGE: Work around for SetPlayerAccessory not syncing
	private void SyncPlayerAccessories(Dictionary<int, int> models, Dictionary<int, int> textures)
	{
		SetData(m_Client, EDataNames.PROP_MODELS, Newtonsoft.Json.JsonConvert.SerializeObject(models), EDataType.Synced);
		SetData(m_Client, EDataNames.PROP_TEXTS, Newtonsoft.Json.JsonConvert.SerializeObject(textures), EDataType.Synced);
	}

	private CItemInstanceDef GetActiveDutyOutfit(EDutyType a_DutyType)
	{
		List<CItemInstanceDef> lstOutfits = Inventory.GetDutyOutfitsOfType(a_DutyType);

		foreach (CItemInstanceDef dutyOutfit in lstOutfits)
		{
			CItemValueDutyOutfit itemVal = (CItemValueDutyOutfit)dutyOutfit.Value;
			if (itemVal.IsActive)
			{
				return dutyOutfit;
			}
		}

		return null;
	}

	private CItemInstanceDef GetActiveOutfit()
	{
		List<CItemInstanceDef> lstOutfits = Inventory.GetAllOutfits();

		foreach (CItemInstanceDef outfit in lstOutfits)
		{
			CItemValueOutfit itemVal = (CItemValueOutfit)outfit.Value;
			if (itemVal.IsActive)
			{
				return outfit;
			}
		}

		return null;
	}

	// This doesn't activate it, it just applys the currently/already active one
	public void ApplyDutySkin(EDutyType a_DutyType)
	{
		CItemInstanceDef dutyOutfit = GetActiveDutyOutfit(a_DutyType);
		if (dutyOutfit != null)
		{
			CItemValueDutyOutfit itemVal = (CItemValueDutyOutfit)dutyOutfit.Value;

			SetDefaultProps();

			ApplySkinFromInventory(true, true, true);

			BulkClothing bulkClothing = new BulkClothing();

			//SyncPlayerAccesory(slot.Prop, slot.Drawable, slot.Texture);

			if (itemVal.CharType == EDutyOutfitType.Custom)
			{
				foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
				{
					bulkClothing.Set((int)component, itemVal.Drawables[(int)component], itemVal.Textures[(int)component]);
				}

				foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
				{
					SyncPlayerAccesory(prop, itemVal.PropDrawables[(int)prop], itemVal.PropTextures[(int)prop]);
				}

				SetClothingBulk(bulkClothing);
			}
			else
			{
				SetCharacterSkin((PedHash)itemVal.PremadeHash);
				SetDefaultProps();
			}
		}

		ApplyCharacterBodyCustomization(a_DutyType);
	}

	public void Freeze(bool a_bFreeze)
	{
		NetworkEventSender.SendNetworkEvent_Freeze(this, a_bFreeze);
	}

	public bool IsOnDuty()
	{
		return m_DutyType != EDutyType.None;
	}

	public bool IsOnDutyOfType(EDutyType a_Type)
	{
		return m_DutyType == a_Type;
	}

	public void SetSkinDataFromDB(CustomCharacterSkinData a_CustomSkinData, List<int> lstTattoos)
	{
		m_CustomSkinData = a_CustomSkinData;
		UpdateTattoos(lstTattoos, false);
		ApplySkinFromInventory();
	}

	public async void UpdateTattoos(List<int> lstNewTattoos, bool bSaveDiffToDB)
	{
		if (bSaveDiffToDB)
		{
			// DO a diff so we can remove / add to db
			// see what we added
			foreach (int newTattooID in lstNewTattoos)
			{
				// Not in current? we added it!
				if (!m_lstTattoos.Contains(newTattooID))
				{
					await Database.LegacyFunctions.AddTattoo(ActiveCharacterDatabaseID, newTattooID).ConfigureAwait(true);
				}
			}

			// see what we removed
			foreach (int oldTattooID in m_lstTattoos)
			{
				// Not in new? we removed it!
				if (!lstNewTattoos.Contains(oldTattooID))
				{
					await Database.LegacyFunctions.RemoveTattoo(ActiveCharacterDatabaseID, oldTattooID).ConfigureAwait(true);
				}
			}
		}

		m_lstTattoos = lstNewTattoos;
	}

#pragma warning disable CA1051
	public CustomCharacterSkinData m_CustomSkinData;
#pragma warning restore CA1051
	private List<int> m_lstTattoos;

	public List<int> GetTattoos()
	{
		return m_lstTattoos;
	}

	public List<PlayerKeybindObject> Keybinds { get; set; }

	public ECharacterType CharacterType { get; set; }

	public UInt32 Age { get; set; }

	public CItemInstanceDef GetActivePremadeClothing()
	{
		CItemInstanceDef retVal = null;
		foreach (var item in Inventory.GetAllItems())
		{
			if (item != null)
			{
				if (item.ItemID == EItemID.CLOTHES)
				{
					CItemValueClothingPremade itemVal = (CItemValueClothingPremade)item.Value;

					if (itemVal.IsActive)
					{
						retVal = item;
						break;
					}
				}
			}
		}

		return retVal;
	}

	public CItemInstanceDef GetActiveCustomClothing(EItemID itemType)
	{
		CItemInstanceDef retVal = null;
		if ((itemType >= EItemID.CLOTHES_CUSTOM_FACE && itemType <= EItemID.CLOTHES_CUSTOM_TOPS) || ItemHelpers.IsItemIDAProp(itemType))
		{
			foreach (var item in Inventory.GetAllItems())
			{
				if (item != null)
				{
					if (item.ItemID == itemType)
					{
						CItemValueClothingCustom itemVal = (CItemValueClothingCustom)item.Value;

						if (itemVal.IsActive)
						{
							retVal = item;
							break;
						}
					}
				}
			}
		}

		return retVal;
	}

	public bool ActivatePremadeClothing(CItemInstanceDef itemDef)
	{
		if (DeactivatePremadeClothing())
		{
			CItemValueClothingPremade itemVal = (CItemValueClothingPremade)itemDef.Value;
			itemVal.IsActive = true;

			// Forcefully move item into clothing socket, this item cant be dragged here or added to this socket, but active clothing is in here
			itemDef.CurrentSocket = EItemSocket.Clothing;

			Database.Functions.Items.SaveItemValueAndSocket(itemDef);

			return true;
		}

		return false;
	}

	public bool ActivateCustomClothing(CItemInstanceDef itemDef)
	{
		if (DeactivateCustomClothing(itemDef.ItemID))
		{
			CItemValueClothingCustom itemVal = (CItemValueClothingCustom)itemDef.Value;
			itemVal.IsActive = true;

			// Forcefully move item into clothing socket, this item cant be dragged here or added to this socket, but active clothing is in here
			itemDef.CurrentSocket = EItemSocket.Clothing;

			Database.Functions.Items.SaveItemValueAndSocket(itemDef);

			ApplySkinFromInventory();

			return true;
		}

		return false;
	}

	public bool DeactivatePremadeClothing()
	{
		bool retVal = true;
		foreach (var item in Inventory.GetAllItems())
		{
			if (item != null)
			{
				if (item.ItemID == EItemID.CLOTHES)
				{
					CItemValueClothingPremade itemVal = (CItemValueClothingPremade)item.Value;

					if (itemVal.IsActive)
					{
						itemVal.IsActive = !itemVal.IsActive;
						item.CurrentSocket = EItemSocket.Clothing;
						Database.Functions.Items.SaveItemValueAndSocket(item);
						/*
						bool bSuitableBindingFound = Inventory.DetermineSuitableBindingForItemGranting(item, out EItemParentTypes itemParentType, out long parentDBID, out EItemSocket recommendedSocket);

						// Check we have somewhere else for this item to go
						if (bSuitableBindingFound)
						{
							

							item.CurrentSocket = recommendedSocket;
							Database.LegacyFunctions.SaveItemValueAndSocket(item);
							retVal = true;
						}
						else
						{
							retVal = false;
						}*/

						break;
					}
				}
			}
		}

		return retVal;
	}

	public void DeactivateAllCustomClothing()
	{
		foreach (var item in Inventory.GetAllItems())
		{
			if (item != null)
			{
				if (ItemHelpers.IsItemIDClothing(item.ItemID) || ItemHelpers.IsItemIDAProp(item.ItemID))
				{
					DeactivateCustomClothingEntry(item);
				}
			}
		}
	}

	private bool DeactivateCustomClothingEntry(CItemInstanceDef item)
	{
		bool retVal = true;
		CItemValueClothingCustom itemVal = (CItemValueClothingCustom)item.Value;

		if (itemVal.IsActive)
		{
			bool bSuitableBindingFound = Inventory.DetermineSuitableBindingForItemGranting(item, out EItemParentTypes itemParentType, out long parentDBID, out EItemSocket recommendedSocket);

			// Check we have somewhere else for this item to go
			if (bSuitableBindingFound)
			{
				itemVal.IsActive = false;
				item.CurrentSocket = recommendedSocket;
				Database.Functions.Items.SaveItemValueAndSocket(item);
				retVal = true;
			}
			else
			{
				retVal = false;
			}
		}

		return retVal;
	}

	public bool DeactivateCustomClothing(EItemID itemType)
	{
		bool retVal = true;
		if (ItemHelpers.IsItemIDClothing(itemType) || ItemHelpers.IsItemIDAProp(itemType))
		{
			foreach (var item in Inventory.GetAllItems())
			{
				if (item != null)
				{
					if (item.ItemID == itemType)
					{
						retVal = DeactivateCustomClothingEntry(item);
					}
				}
			}
		}

		return retVal;
	}

	public void SetSpawnFix(Vector3 vecPos, Vector3 vecRot, uint dimension, int health, int armor)
	{
		m_vecRageSpawnFix_Pos = vecPos;
		m_vecRageSpawnFix_Rot = vecRot;
		m_vecRageSpawnFix_Dimension = dimension;
		m_spawnFixHealth = health;
		m_spawnFixArmor = armor;
		m_bDoneSpawnFixPos = false;
		m_bDoneSpawnFixHealth = false;
		m_bDoneSpawnFixArmor = false;

		MainThreadTimerPool.CreateEntityTimer((object[] parameters) =>
		{
			m_bDoneSpawnFixHealth = true;
			m_bDoneSpawnFixArmor = true;
		}, 2000, this, 1);
	}

	private Vector3 m_vecRageSpawnFix_Pos = new Vector3();
	private Vector3 m_vecRageSpawnFix_Rot = new Vector3();
	private uint m_vecRageSpawnFix_Dimension = 0;
	private bool m_bDoneSpawnFixPos = true;
	private bool m_bDoneSpawnFixHealth = true;
	private bool m_bDoneSpawnFixArmor = true;
	private int m_spawnFixHealth = 100;
	private int m_spawnFixArmor = 0;

	public void UpdateSpawnFix()
	{
		// TODO_RAGE: This is a hacky fix for rage teleporting our character randomly to the observatory...
		if (!m_bDoneSpawnFixPos)
		{
			Vector3 vecRootPos = new Vector3(-427.0f, 1116.0f, 326.0f);
			const float fTolerance = 1.0f;

			if ((Client.Position.X >= vecRootPos.X - fTolerance && Client.Position.X <= vecRootPos.X + fTolerance)
				&& (Client.Position.Y >= vecRootPos.Y - fTolerance && Client.Position.Y <= vecRootPos.Y + fTolerance)
				&& (Client.Position.Z >= vecRootPos.Z - fTolerance && Client.Position.Z <= vecRootPos.Z + fTolerance))
			{
				SetPositionSafe(m_vecRageSpawnFix_Pos);
				Client.Rotation = m_vecRageSpawnFix_Rot;

				// only if spawn fix isnt pending
				if (m_SpawnDimensionFixTimer.Instance() == null)
				{
					SetSafeDimension(m_vecRageSpawnFix_Dimension);
				}

				m_bDoneSpawnFixPos = true;
				m_vecRageSpawnFix_Pos = new Vector3();
				m_vecRageSpawnFix_Rot = new Vector3();
			}
		}

		// TODO_RAGE: Hacky fix for rage health changing
		if (!m_bDoneSpawnFixHealth)
		{
			if (Client.Health != m_spawnFixHealth)
			{
				Client.Health = m_spawnFixHealth;
				m_bDoneSpawnFixHealth = true;
			}
		}

		if (!m_bDoneSpawnFixArmor)
		{
			if (Client.Armor != m_spawnFixArmor)
			{
				Client.Armor = m_spawnFixArmor;
				m_bDoneSpawnFixArmor = true;
			}
		}
	}

	public void SendFurnitureList()
	{
		List<CItemInstanceDef> lstFurnitureItems = new List<CItemInstanceDef>();
		foreach (var item in Inventory.GetAllItems())
		{
			if (item.IsFurniture())
			{
				lstFurnitureItems.Add(item);
			}
		}
		NetworkEventSender.SendNetworkEvent_GotoPropertyEditMode(this, lstFurnitureItems);
	}

	public void GotoInteriorEditMode()
	{
		if (Client.Dimension > 0)
		{
			CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(Client.Dimension);
			if (propInst != null)
			{
				bool bHasFactionForProperty = propInst.IsPropertyForAnyPlayerFaction(this, true);
				if (bHasFactionForProperty || propInst.HasKeys(this) || IsAdmin(EAdminLevel.TrialAdmin, true))
				{
					SendFurnitureList();
				}
				else
				{
					SendNotification("Edit Property", ENotificationIcon.ExclamationSign, "You do not have access to edit this property. You must have the keys or be a manager in the faction.", null);
				}
			}
			else
			{
				SendNotification("Edit Property", ENotificationIcon.ExclamationSign, "You are not in a valid property.", null);
			}
		}
		else
		{
			SendNotification("Edit Property", ENotificationIcon.ExclamationSign, "You are not in a valid property.", null);
		}
	}

	private void SetDefaultProps()
	{
		Dictionary<int, int> blankPropDictionary = new Dictionary<int, int>()
		{
			{ (int)ECustomPropSlot.Hats, 0 },
			{ (int)ECustomPropSlot.Glasses, 0 },
			{ (int)ECustomPropSlot.Ears, 0 },
			{ (int)ECustomPropSlot.Watches, 0 },
			{ (int)ECustomPropSlot.Bracelets, 0 }
		};

		/*
		Hats = 0,
	Glasses = 1,
	Ears = 2,
	Watches = 6,
	Bracelets = 7
	*/

		// Props (this initializes it and resets it)
		SetData(m_Client, EDataNames.PROP_MODELS, Newtonsoft.Json.JsonConvert.SerializeObject(blankPropDictionary), EDataType.Synced);
		SetData(m_Client, EDataNames.PROP_TEXTS, Newtonsoft.Json.JsonConvert.SerializeObject(blankPropDictionary), EDataType.Synced);
	}

	public void RestorePet()
	{
		foreach (var item in Inventory.GetAllItems())
		{
			if (item.ItemID == EItemID.PET)
			{
				CItemValuePet itemVal = (CItemValuePet)item.Value;
				if (itemVal.IsActive)
				{
					SetCurrentPet(itemVal);
					break;
				}
			}
		}
	}

	public bool IsCurrentPet(CItemValuePet petData)
	{
		if (petData != null)
		{
			string strCurrentName = GetData<string>(m_Client, EDataNames.PET_NAME);
			EPetType currentPetType = GetData<EPetType>(m_Client, EDataNames.PET_TYPE);

			return strCurrentName == petData.strName && currentPetType == petData.PetType;
		}

		return false;
	}


	public void SetCurrentPet(CItemValuePet petData)
	{
		if (petData == null)
		{
			ClearData(m_Client, EDataNames.PET_NAME);
			ClearData(m_Client, EDataNames.PET_TYPE);
		}
		else
		{
			SetData(m_Client, EDataNames.PET_NAME, petData.strName, EDataType.Synced);
			SetData(m_Client, EDataNames.PET_TYPE, petData.PetType, EDataType.Synced);
		}
	}

	public void ApplySkinFromInventory(bool bIgnoreJobSkin = false, bool bIgnoreDutyskin = false, bool bIgnoreAllClothing = false)
	{
		// TODO_LAUNCH: Support jail skin if jailed
		if (!bIgnoreDutyskin)
		{
			if (IsOnDuty())
			{
				ApplyDutySkin(m_DutyType);
				return;
			}
		}

		// We might re-work this in the future, for now, players didn't like it
		/*
		if (!bIgnoreJobSkin)
		{
			if (Job != EJobID.None)
			{
				ApplyJobSkin();
				return;
			}
		}
		*/

		SetData(m_Client, EDataNames.GENDER, Gender, EDataType.Synced);

		bool bFoundValidClothing = false;

		if (CharacterType == ECharacterType.Premade)
		{
			// Find the active clothing item
			SetData(m_Client, EDataNames.IS_CUSTOM, false, EDataType.Synced);

			ApplyCharacterBodyCustomization();

			if (!bIgnoreAllClothing)
			{
				BulkClothing bulkClothing = new BulkClothing();

				foreach (var item in Inventory.GetAllItems())
				{
					if (item != null)
					{
						if (item.ItemID == EItemID.CLOTHES) // premade clothing
						{
							CItemValueClothingPremade itemVal = (CItemValueClothingPremade)item.Value;

							if (itemVal.IsActive)
							{
								bFoundValidClothing = true;
								SetCharacterSkin(itemVal.SkinHash);
								SetDefaultProps();

								break;
							}
						}
					}
				}

				SetClothingBulk(bulkClothing);
			}
		}
		else // Custom skins
		{
			// Is the skin already set? If so don't reset it as it causes a full char re-stream in
			// We set defaults manually below using the dictionary
			PedHash desiredSkin = Gender == EGender.Male ? PedHash.FreemodeMale01 : PedHash.FreemodeFemale01;
			if (m_Client.Model != (uint)desiredSkin)
			{
				SetCharacterSkin(desiredSkin);
			}

			SetDefaultProps();

			// Used to track which items we had set, so we can apply naked values - we don't care about things like masks etc, just clothing that index 0 isnt naked (e.g. torso index 0 = tshirt)
			Dictionary<EItemID, bool> dictItemsSet = new Dictionary<EItemID, bool>()
			{
				{ EItemID.CLOTHES_CUSTOM_FACE, false },
				{ EItemID.CLOTHES_CUSTOM_MASK, false },
				// TODO_LAUNCH: do we even want to give hair? seems its the same as hair style data?
				{ EItemID.CLOTHES_CUSTOM_HAIR, false },
				{ EItemID.CLOTHES_CUSTOM_TORSO, false },
				{ EItemID.CLOTHES_CUSTOM_LEGS, false },
				{ EItemID.CLOTHES_CUSTOM_BACK, false },
				{ EItemID.CLOTHES_CUSTOM_FEET, false },
				{ EItemID.CLOTHES_CUSTOM_ACCESSORY, false },
				{ EItemID.CLOTHES_CUSTOM_UNDERSHIRT, false },
				{ EItemID.CLOTHES_CUSTOM_BODYARMOR, false },
				{ EItemID.CLOTHES_CUSTOM_DECALS, false },
				{ EItemID.CLOTHES_CUSTOM_TOPS, false },
			};

			BulkClothing bulkClothing = new BulkClothing();
			if (!bIgnoreAllClothing)
			{
				foreach (var item in Inventory.GetAllItems())
				{
					if (item != null)
					{
						if (item.ItemID >= EItemID.CLOTHES_CUSTOM_FACE && item.ItemID <= EItemID.CLOTHES_CUSTOM_TOPS) // custom clothing
						{
							CItemValueClothingCustom itemVal = (CItemValueClothingCustom)item.Value;

							if (itemVal.IsActive)
							{
								bFoundValidClothing = true;
								int index = item.ItemID - EItemID.CLOTHES_CUSTOM_FACE;

								int model = itemVal.Model;
								int texture = itemVal.Texture;

								bulkClothing.Set(index, model, texture);

								dictItemsSet[item.ItemID] = true;
							}
						}
						else if (ItemHelpers.IsItemIDAProp(item.ItemID)) // custom props
						{
							CItemValueClothingCustom itemVal = (CItemValueClothingCustom)item.Value;

							if (itemVal.IsActive)
							{
								bFoundValidClothing = true;

								ECustomPropSlot slot = ECustomPropSlot.Hats;
								if (item.ItemID == EItemID.CLOTHES_CUSTOM_HELMET)
								{
									slot = ECustomPropSlot.Hats;
								}
								else if (item.ItemID == EItemID.CLOTHES_CUSTOM_GLASSES)
								{
									slot = ECustomPropSlot.Glasses;
								}
								else if (item.ItemID == EItemID.CLOTHES_CUSTOM_EARRINGS)
								{
									slot = ECustomPropSlot.Ears;
								}
								else if (item.ItemID == EItemID.CLOTHES_CUSTOM_WATCHES)
								{
									slot = ECustomPropSlot.Watches;
								}
								else if (item.ItemID == EItemID.CLOTHES_CUSTOM_BRACELETS)
								{
									slot = ECustomPropSlot.Bracelets;
								}

								SyncPlayerAccesory(slot, itemVal.Model, itemVal.Texture);
							}
						}
					}
				}
			}

			// Determine which clothing items we didnt see, and apply a naked default if required
			if (!bIgnoreAllClothing)
			{
				foreach (var kvPair in dictItemsSet)
				{
					if (kvPair.Value == false)
					{
						int index = kvPair.Key - EItemID.CLOTHES_CUSTOM_FACE;

						if (kvPair.Key == EItemID.CLOTHES_CUSTOM_LEGS)
						{
							bulkClothing.Set(index, Gender == EGender.Male ? 21 : 15, 0);
						}
						else if (kvPair.Key == EItemID.CLOTHES_CUSTOM_FEET)
						{
							bulkClothing.Set(index, Gender == EGender.Male ? 34 : 35, 0);
						}
						else if (kvPair.Key == EItemID.CLOTHES_CUSTOM_UNDERSHIRT)
						{
							bulkClothing.Set(index, 15, 0);
						}
						else if (kvPair.Key == EItemID.CLOTHES_CUSTOM_TOPS)
						{
							// set torso also to bare chest when removing top
							bulkClothing.Set(EItemID.CLOTHES_CUSTOM_TORSO - EItemID.CLOTHES_CUSTOM_FACE, 15, 0);

							bulkClothing.Set(index, 15, 0);
						}
						else
						{
							bulkClothing.Set(index, 0, 0);
						}
					}
				}
			}
			else
			{
				bFoundValidClothing = true;
			}

			SetClothingBulk(bulkClothing);

			SetData(m_Client, EDataNames.IS_CUSTOM, true, EDataType.Synced);

			ApplyCharacterBodyCustomization();
		}

		// No valid skins? apply nude skin
		if (!bFoundValidClothing)
		{
			if (Gender == EGender.Male)
			{
				SetCharacterSkin(PedHash.Acult01AMM);
			}
			else
			{
				SetCharacterSkin(PedHash.FatCult01AFM);
			}
		}
	}

	public async void UpdateCustomCharacterBodyData(bool bSaveToDB, int Ageing,
			float AgeingOpacity,
			int Makeup,
			float MakeupOpacity,
			int MakeupColor,
			int MakeupColorHighlight,
			int Blush,
			float BlushOpacity,
			int BlushColor,
			int BlushColorHighlight,
			int Complexion,
			float ComplexionOpacity,
			int SunDamage,
			float SunDamageOpacity,
			int Lipstick,
			float LipstickOpacity,
			int LipstickColor,
			int LipstickColorHighlights,
			int MolesAndFreckles,
			float MolesAndFrecklesOpacity,
			float NoseSizeHorizontal,
			float NoseSizeVertical,
			float NoseSizeOutwards,
			float NoseSizeOutwardsUpper,
			float NoseSizeOutwardsLower,
			float NoseAngle,
			float EyebrowHeight,
			float EyebrowDepth,
			float CheekboneHeight,
			float CheekWidth,
			float CheekWidthLower,
			float EyeSize,
			float LipSize,
			float MouthSize,
			float MouthSizeLower,
			float ChinSize,
			float ChinSizeLower,
			float ChinWidth,
			float ChinEffect,
			float NeckWidth,
			float NeckWidthLower,
			int FaceBlend1Mother,
			int FaceBlend1Father,
			float FaceBlendFatherPercent,
			float SkinBlendFatherPercent,
			int EyeColor,
			int Blemishes,
			float BlemishesOpacity,
			int Eyebrows,
			float EyebrowsOpacity,
			int EyebrowsColor,
			int EyebrowsColorHighlight,
			int BodyBlemishes,
			float BodyBlemishesOpacity)
	{
		m_CustomSkinData.Ageing = Ageing;
		m_CustomSkinData.AgeingOpacity = AgeingOpacity;
		m_CustomSkinData.Makeup = Makeup;
		m_CustomSkinData.MakeupOpacity = MakeupOpacity;
		m_CustomSkinData.MakeupColor = MakeupColor;
		m_CustomSkinData.MakeupColorHighlight = MakeupColorHighlight;
		m_CustomSkinData.Blush = Blush;
		m_CustomSkinData.BlushOpacity = BlushOpacity;
		m_CustomSkinData.BlushColor = BlushColor;
		m_CustomSkinData.BlushColorHighlight = BlushColorHighlight;
		m_CustomSkinData.Complexion = Complexion;
		m_CustomSkinData.ComplexionOpacity = ComplexionOpacity;
		m_CustomSkinData.SunDamage = SunDamage;
		m_CustomSkinData.SunDamageOpacity = SunDamageOpacity;
		m_CustomSkinData.Lipstick = Lipstick;
		m_CustomSkinData.LipstickOpacity = LipstickOpacity;
		m_CustomSkinData.LipstickColor = LipstickColor;
		m_CustomSkinData.LipstickColorHighlights = LipstickColorHighlights;
		m_CustomSkinData.MolesAndFreckles = MolesAndFreckles;
		m_CustomSkinData.MolesAndFrecklesOpacity = MolesAndFrecklesOpacity;
		m_CustomSkinData.NoseSizeHorizontal = NoseSizeHorizontal;
		m_CustomSkinData.NoseSizeVertical = NoseSizeVertical;
		m_CustomSkinData.NoseSizeOutwards = NoseSizeOutwards;
		m_CustomSkinData.NoseSizeOutwardsUpper = NoseSizeOutwardsUpper;
		m_CustomSkinData.NoseSizeOutwardsLower = NoseSizeOutwardsLower;
		m_CustomSkinData.NoseAngle = NoseAngle;
		m_CustomSkinData.EyebrowHeight = EyebrowHeight;
		m_CustomSkinData.EyebrowDepth = EyebrowDepth;
		m_CustomSkinData.CheekboneHeight = CheekboneHeight;
		m_CustomSkinData.CheekWidth = CheekWidth;
		m_CustomSkinData.CheekWidthLower = CheekWidthLower;
		m_CustomSkinData.EyeSize = EyeSize;
		m_CustomSkinData.LipSize = LipSize;
		m_CustomSkinData.MouthSize = MouthSize;
		m_CustomSkinData.MouthSizeLower = MouthSizeLower;
		m_CustomSkinData.ChinSize = ChinSize;
		m_CustomSkinData.ChinSizeLower = ChinSizeLower;
		m_CustomSkinData.ChinWidth = ChinWidth;
		m_CustomSkinData.ChinEffect = ChinEffect;
		m_CustomSkinData.NeckWidth = NeckWidth;
		m_CustomSkinData.NeckWidthLower = NeckWidthLower;
		m_CustomSkinData.FaceBlend1Mother = FaceBlend1Mother;
		m_CustomSkinData.FaceBlend1Father = FaceBlend1Father;
		m_CustomSkinData.FaceBlendFatherPercent = FaceBlendFatherPercent;
		m_CustomSkinData.SkinBlendFatherPercent = SkinBlendFatherPercent;
		m_CustomSkinData.EyeColor = EyeColor;
		m_CustomSkinData.Blemishes = Blemishes;
		m_CustomSkinData.BlemishesOpacity = BlemishesOpacity;
		m_CustomSkinData.Eyebrows = Eyebrows;
		m_CustomSkinData.EyebrowsOpacity = EyebrowsOpacity;
		m_CustomSkinData.EyebrowsColor = EyebrowsColor;
		m_CustomSkinData.EyebrowsColorHighlight = EyebrowsColorHighlight;
		m_CustomSkinData.BodyBlemishes = BodyBlemishes;
		m_CustomSkinData.BodyBlemishesOpacity = BodyBlemishesOpacity;

		if (bSaveToDB)
		{
			await Database.LegacyFunctions.SavePlasticSurgeonData(ActiveCharacterDatabaseID, Ageing, AgeingOpacity, Makeup, MakeupOpacity, MakeupColor, MakeupColorHighlight, Blush, BlushOpacity, BlushColor, BlushColorHighlight,
											Complexion, ComplexionOpacity, SunDamage, SunDamageOpacity, Lipstick, LipstickOpacity, LipstickColor,
											LipstickColorHighlights, MolesAndFreckles, MolesAndFrecklesOpacity, NoseSizeHorizontal, NoseSizeVertical, NoseSizeOutwards, NoseSizeOutwardsUpper,
											NoseSizeOutwardsLower, NoseAngle, EyebrowHeight, EyebrowDepth, CheekboneHeight, CheekWidth, CheekWidthLower, EyeSize, LipSize,
											MouthSize, MouthSizeLower, ChinSize, ChinSizeLower, ChinWidth, ChinEffect, NeckWidth, NeckWidthLower, FaceBlend1Mother,
											FaceBlend1Father, FaceBlendFatherPercent, SkinBlendFatherPercent, EyeColor, Blemishes, BlemishesOpacity, Eyebrows, EyebrowsOpacity,
											EyebrowsColor, EyebrowsColorHighlight, BodyBlemishes, BodyBlemishesOpacity).ConfigureAwait(true);
		}

		ApplyCharacterBodyCustomization();
	}

	private CItemValueDutyOutfit m_PendingDutyOutfitShare = null;
	public void SetPendingDutyOutfitShare(CItemValueDutyOutfit dutyOutfit)
	{
		m_PendingDutyOutfitShare = dutyOutfit;
	}

	public void ResetPendingDutyOutfitShare()
	{
		m_PendingDutyOutfitShare = null;
	}

	public CItemValueDutyOutfit GetPendingDutyOutfitShare()
	{
		return m_PendingDutyOutfitShare;
	}

	public bool IsEligbleToUseDutyOfType(EDutyType dutyType)
	{
		if (dutyType == EDutyType.Law_Enforcement && IsInFactionOfType(EFactionType.LawEnforcement))
		{
			return true;
		}
		else if ((dutyType == EDutyType.EMS || dutyType == EDutyType.Fire) && IsInFactionOfType(EFactionType.Medical))
		{
			return true;
		}
		else if ((dutyType == EDutyType.News && IsInFactionOfType(EFactionType.NewsFaction)))
		{
			return true;
		}
		else if (dutyType == EDutyType.Towing && IsInFactionOfType(EFactionType.Towing))
		{
			return true;
		}

		return false;
	}

	private void ApplyCharacterBodyCustomization(EDutyType DutyPreviewType = EDutyType.None)
	{
		if (CharacterType == ECharacterType.Premade)
		{
			ClearData(m_Client, EDataNames.CC_TATTOOS);
			ClearData(m_Client, EDataNames.CC_AGEING);
			ClearData(m_Client, EDataNames.CC_AGEINGOPACITY);
			ClearData(m_Client, EDataNames.CC_MAKEUP);
			ClearData(m_Client, EDataNames.CC_MAKEUPOPACITY);
			ClearData(m_Client, EDataNames.CC_BLUSH);
			ClearData(m_Client, EDataNames.CC_BLUSHOPACITY);
			ClearData(m_Client, EDataNames.CC_BLUSHCOLOR);
			ClearData(m_Client, EDataNames.CC_BLUSHCOLORHIGHLIGHT);
			ClearData(m_Client, EDataNames.CC_COMPLEXION);
			ClearData(m_Client, EDataNames.CC_COMPLEXIONOPACITY);
			ClearData(m_Client, EDataNames.CC_SUNDAMAGE);
			ClearData(m_Client, EDataNames.CC_SUNDAMAGEOPACITY);
			ClearData(m_Client, EDataNames.CC_LIPSTICK);
			ClearData(m_Client, EDataNames.CC_LIPSTICKOPACITY);
			ClearData(m_Client, EDataNames.CC_LIPSTICKCOLOR);
			ClearData(m_Client, EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS);
			ClearData(m_Client, EDataNames.CC_MOLESANDFRECKLES);
			ClearData(m_Client, EDataNames.CC_MOLESANDFRECKLESOPACITY);
			ClearData(m_Client, EDataNames.CC_NOSESIZEHORIZONTAL);
			ClearData(m_Client, EDataNames.CC_NOSESIZEVERTICAL);
			ClearData(m_Client, EDataNames.CC_NOSESIZEOUTWARDS);
			ClearData(m_Client, EDataNames.CC_NOSESIZEOUTWARDSUPPER);
			ClearData(m_Client, EDataNames.CC_NOSESIZEOUTWARDSLOWER);
			ClearData(m_Client, EDataNames.CC_NOSEANGLE);
			ClearData(m_Client, EDataNames.CC_EYEBROWHEIGHT);
			ClearData(m_Client, EDataNames.CC_EYEBROWDEPTH);
			ClearData(m_Client, EDataNames.CC_CHEEKBONEHEIGHT);
			ClearData(m_Client, EDataNames.CC_CHEEKWIDTH);
			ClearData(m_Client, EDataNames.CC_CHEEKWIDTHLOWER);
			ClearData(m_Client, EDataNames.CC_EYESIZE);
			ClearData(m_Client, EDataNames.CC_LIPSIZE);
			ClearData(m_Client, EDataNames.CC_MOUTHSIZE);
			ClearData(m_Client, EDataNames.CC_MOUTHSIZELOWER);
			ClearData(m_Client, EDataNames.CC_CHINSIZE);
			ClearData(m_Client, EDataNames.CC_CHINSIZELOWER);
			ClearData(m_Client, EDataNames.CC_CHINWIDTH);
			ClearData(m_Client, EDataNames.CC_CHINEFFECT);
			ClearData(m_Client, EDataNames.CC_NECKWIDTH);
			ClearData(m_Client, EDataNames.CC_NECKWIDTHLOWER);
			ClearData(m_Client, EDataNames.CC_FACEBLEND1MOTHER);
			ClearData(m_Client, EDataNames.CC_FACEBLEND1FATHER);
			ClearData(m_Client, EDataNames.CC_FACEBLENDFATHERPERCENT);
			ClearData(m_Client, EDataNames.CC_SKINBLENDFATHERPERCENT);
			ClearData(m_Client, EDataNames.CC_HAIRCOLOR);
			ClearData(m_Client, EDataNames.CC_HAIRCOLORHIGHLIGHTS);
			ClearData(m_Client, EDataNames.CC_EYECOLOR);
			ClearData(m_Client, EDataNames.CC_FACIALHAIRSTYLE);
			ClearData(m_Client, EDataNames.CC_FACIALHAIRCOLOR);
			ClearData(m_Client, EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT);
			ClearData(m_Client, EDataNames.CC_FACIALHAIROPACITY);
			ClearData(m_Client, EDataNames.CC_BLEMISHES);
			ClearData(m_Client, EDataNames.CC_BLEMISHESOPACITY);
			ClearData(m_Client, EDataNames.CC_EYEBROWS);
			ClearData(m_Client, EDataNames.CC_EYEBROWSOPACITY);
			ClearData(m_Client, EDataNames.CC_EYEBROWSCOLOR);
			ClearData(m_Client, EDataNames.CC_BODYBLEMISHES);
			ClearData(m_Client, EDataNames.CC_BODYBLEMISHESOPACITY);
			ClearData(m_Client, EDataNames.CC_CHESTHAIR);
			ClearData(m_Client, EDataNames.CC_CHESTHAIROPACITY);
			ClearData(m_Client, EDataNames.CC_CHESTHAIRCOLOR);
			ClearData(m_Client, EDataNames.CC_CHESTHAIRHIGHLIGHT);
		}
		else
		{
			// TODO_DATA: would be awesome if setdata would automatically serialize lists etc
			SetData(m_Client, EDataNames.CC_TATTOOS, Newtonsoft.Json.JsonConvert.SerializeObject(m_lstTattoos), EDataType.Synced, true);

			SetData(m_Client, EDataNames.CC_AGEING, m_CustomSkinData.Ageing, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_AGEINGOPACITY, m_CustomSkinData.AgeingOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MAKEUP, m_CustomSkinData.Makeup, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MAKEUPOPACITY, m_CustomSkinData.MakeupOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MAKEUPCOLOR, m_CustomSkinData.MakeupColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MAKEUPCOLORHIGHLIGHT, m_CustomSkinData.MakeupColorHighlight, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_BLUSH, m_CustomSkinData.Blush, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_BLUSHOPACITY, m_CustomSkinData.BlushOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_BLUSHCOLOR, m_CustomSkinData.BlushColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_BLUSHCOLORHIGHLIGHT, m_CustomSkinData.BlushColorHighlight, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_COMPLEXION, m_CustomSkinData.Complexion, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_COMPLEXIONOPACITY, m_CustomSkinData.ComplexionOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_SUNDAMAGE, m_CustomSkinData.SunDamage, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_SUNDAMAGEOPACITY, m_CustomSkinData.SunDamageOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_LIPSTICK, m_CustomSkinData.Lipstick, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_LIPSTICKOPACITY, m_CustomSkinData.LipstickOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_LIPSTICKCOLOR, m_CustomSkinData.LipstickColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_LIPSTICKCOLORHIGHLIGHTS, m_CustomSkinData.LipstickColorHighlights, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MOLESANDFRECKLES, m_CustomSkinData.MolesAndFreckles, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MOLESANDFRECKLESOPACITY, m_CustomSkinData.MolesAndFrecklesOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NOSESIZEHORIZONTAL, m_CustomSkinData.NoseSizeHorizontal, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NOSESIZEVERTICAL, m_CustomSkinData.NoseSizeVertical, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NOSESIZEOUTWARDS, m_CustomSkinData.NoseSizeOutwards, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NOSESIZEOUTWARDSUPPER, m_CustomSkinData.NoseSizeOutwardsUpper, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NOSESIZEOUTWARDSLOWER, m_CustomSkinData.NoseSizeOutwardsLower, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NOSEANGLE, m_CustomSkinData.NoseAngle, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYEBROWHEIGHT, m_CustomSkinData.EyebrowHeight, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYEBROWDEPTH, m_CustomSkinData.EyebrowDepth, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHEEKBONEHEIGHT, m_CustomSkinData.CheekboneHeight, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHEEKWIDTH, m_CustomSkinData.CheekWidth, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHEEKWIDTHLOWER, m_CustomSkinData.CheekWidthLower, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYESIZE, m_CustomSkinData.EyeSize, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_LIPSIZE, m_CustomSkinData.LipSize, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MOUTHSIZE, m_CustomSkinData.MouthSize, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_MOUTHSIZELOWER, m_CustomSkinData.MouthSizeLower, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHINSIZE, m_CustomSkinData.ChinSize, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHINSIZELOWER, m_CustomSkinData.ChinSizeLower, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHINWIDTH, m_CustomSkinData.ChinWidth, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHINEFFECT, m_CustomSkinData.ChinEffect, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NECKWIDTH, m_CustomSkinData.NeckWidth, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_NECKWIDTHLOWER, m_CustomSkinData.NeckWidthLower, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_FACEBLEND1MOTHER, m_CustomSkinData.FaceBlend1Mother, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_FACEBLEND1FATHER, m_CustomSkinData.FaceBlend1Father, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_FACEBLENDFATHERPERCENT, m_CustomSkinData.FaceBlendFatherPercent, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_SKINBLENDFATHERPERCENT, m_CustomSkinData.SkinBlendFatherPercent, EDataType.Synced, true);

			bool bHairVisible = true;

			// Should we hide our hair for the mask?
			int currentMaskOrBeard = m_CurrentClothing.GetComponent((int)ECustomClothingComponent.Masks);
			bHairVisible &= !MaskHelpers.MasksWhichCoverHair.Contains(currentMaskOrBeard);

			// are we using a duty outfit that requires hair hidden?
			bool bFoundValidDutyOutfit = false;
			if (IsOnDuty() || DutyPreviewType != EDutyType.None)
			{
				EDutyType dutyTypeToUse = IsOnDuty() ? m_DutyType : DutyPreviewType;
				CItemInstanceDef dutyOutfit = GetActiveDutyOutfit(dutyTypeToUse);
				if (dutyOutfit != null)
				{
					CItemValueDutyOutfit outfitDetails = (CItemValueDutyOutfit)dutyOutfit.Value;
					bHairVisible &= !outfitDetails.HideHair;
					bFoundValidDutyOutfit = true;
				}
			}

			// are we using an outfit that requires hair hidden?
			if (!IsOnDuty() && (DutyPreviewType == EDutyType.None || !bFoundValidDutyOutfit))
			{
				CItemInstanceDef outfit = GetActiveOutfit();
				if (outfit != null)
				{
					CItemValueOutfit outfitDetails = (CItemValueOutfit)outfit.Value;
					bHairVisible &= !outfitDetails.HideHair;
				}
			}

			if (!bHairVisible)
			{
				SetData(m_Client, EDataNames.CC_HAIRSTYLE, 0, EDataType.Synced, true);
				SetData(m_Client, EDataNames.CC_BASEHAIR, -1, EDataType.Synced, true);
			}
			else
			{
				SetData(m_Client, EDataNames.CC_HAIRSTYLE, m_CustomSkinData.HairStyle, EDataType.Synced, true);
				SetData(m_Client, EDataNames.CC_BASEHAIR, m_CustomSkinData.BaseHair, EDataType.Synced, true);
			}

			SetData(m_Client, EDataNames.CC_HAIRCOLOR, m_CustomSkinData.HairColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_HAIRCOLORHIGHLIGHTS, m_CustomSkinData.HairColorHighlights, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYECOLOR, m_CustomSkinData.EyeColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_FACIALHAIRSTYLE, m_CustomSkinData.FacialHairStyle, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_FACIALHAIRCOLOR, m_CustomSkinData.FacialHairColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_FACIALHAIRCOLORHIGHLIGHT, m_CustomSkinData.FacialHairColorHighlight, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_FACIALHAIROPACITY, m_CustomSkinData.FacialHairOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_BLEMISHES, m_CustomSkinData.Blemishes, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_BLEMISHESOPACITY, m_CustomSkinData.BlemishesOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYEBROWS, m_CustomSkinData.Eyebrows, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYEBROWSOPACITY, m_CustomSkinData.EyebrowsOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYEBROWSCOLOR, m_CustomSkinData.EyebrowsColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_EYEBROWSCOLORHIGHLIGHT, m_CustomSkinData.EyebrowsColorHighlight, EDataType.Synced, true);

			SetData(m_Client, EDataNames.CC_BODYBLEMISHES, m_CustomSkinData.BodyBlemishes, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_BODYBLEMISHESOPACITY, m_CustomSkinData.BodyBlemishesOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHESTHAIR, m_CustomSkinData.ChestHair, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHESTHAIROPACITY, m_CustomSkinData.ChestHairOpacity, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHESTHAIRCOLOR, m_CustomSkinData.ChestHairColor, EDataType.Synced, true);
			SetData(m_Client, EDataNames.CC_CHESTHAIRHIGHLIGHT, m_CustomSkinData.ChestHairColor, EDataType.Synced, true);
		}
	}

	public void ApplyJobSkin()
	{
		// We might re-work this in the future, for now, players didn't like it
		/*
		PedHash jobSkin = 0;

		// Determine job skin
		if (Job == EJobID.TruckerJob)
		{
			jobSkin = PedHash.Trucker01SMM;
		}
		else if (Job == EJobID.DeliveryDriverJob)
		{
			jobSkin = PedHash.AirworkerSMY;
		}
		else if (Job == EJobID.BusDriverJob)
		{
			jobSkin = PedHash.GentransportSMM;
		}
		else if (Job == EJobID.MailmanJob)
		{
			Random rng = new Random();
			jobSkin = (rng.Next(0, 1) == 0) ? PedHash.Postal01SMM : PedHash.Postal02SMM;
		}
		else if (Job == EJobID.TrashmanJob)
		{
			jobSkin = PedHash.GarbageSMY;
		}
		else if (Job == EJobID.TaxiDriverJob)
		{
			// let player use their own skin
		}

		if (jobSkin == 0)
		{
			ApplySkinFromInventory(true);
		}
		else
		{
			SetCharacterSkin(jobSkin);
			SetDefaultProps();
		}
		*/
	}

	// DONATOR CURRENCY: This is always taken from the DB to ensure that we have the latest value, e.g. player may have spent money elsewhere (MTA).
	//					 No setter for atomicity, use Increase and Decrease instead. Directly setting could cause race conditions where players get free currency.
	public async Task<int> GetDonatorCurrency()
	{
		if (IsLoggedIn)
		{
			return await Database.LegacyFunctions.GetDonatorCurrency(AccountID).ConfigureAwait(true);
		}

		return 0;
	}

	public async void AddDonatorCurrency(int amount)
	{
		if (IsLoggedIn)
		{
			await Database.LegacyFunctions.AddDonatorCurrency(AccountID, amount).ConfigureAwait(true);
		}
	}

	public async void SubtractDonatorCurrency(int amount)
	{
		if (IsLoggedIn)
		{
			await Database.LegacyFunctions.SubtractDonatorCurrency(AccountID, amount).ConfigureAwait(true);
		}
	}

	public async void SetDonatorCurrency(int amount)
	{
		if (IsLoggedIn)
		{
			await Database.LegacyFunctions.SetDonatorCurrency(AccountID, amount).ConfigureAwait(true);
		}
	}

	public async void SetPlayerAge(int age)
	{
		if (IsInGame())
		{
			await Database.LegacyFunctions.SetPlayerAge(ActiveCharacterDatabaseID, age).ConfigureAwait(true);
		}
	}

	// PUNISHMENT POINTS: This is always taken from the DB to ensure that we have the latest value, e.g. player may have been punished elsewhere (MTA).
	//					 No setter for atomicity, use Increase and Decrease instead. Directly setting could cause race conditions where players get extra points.
	public async Task<int> GetActivePunishmentPoints()
	{
		if (IsLoggedIn)
		{
			return await Database.LegacyFunctions.GetActivePunishmentPoints(AccountID).ConfigureAwait(true);
		}

		return 0;
	}

	public async Task<int> GetAllPunishmentPoints()
	{
		if (IsLoggedIn)
		{
			return await Database.LegacyFunctions.GetAllPunishmentPoints(AccountID).ConfigureAwait(true);
		}

		return 0;
	}

	public async void AddPunishmentPoints(CPlayer adminPlayer, int amount, bool bIsRepeatOffence, string reason)
	{
		if (IsLoggedIn)
		{
			await Database.LegacyFunctions.AddPunishmentPoints(AccountID, amount, adminPlayer.m_DatabaseID, reason).ConfigureAwait(true);
			UpdatePunishmentState(adminPlayer, bIsRepeatOffence);
		}
	}

	public async void SubtractPunishmentPoints(CPlayer adminPlayer, int amount)
	{
		if (IsLoggedIn)
		{
			await Database.LegacyFunctions.SubtractPunishmentPoints(AccountID, amount).ConfigureAwait(true);
		}
	}

	private static Dictionary<int, int> dictPointsToBanHours = new Dictionary<int, int>()
		{
			{2, 1},
			{4, 6},
			{6, 12},
			{8, 24},
			{10, 48},
			{12, 72},
			{14, 96},
			{16, 120},
			{18, 144},
			{20, 0} // permanent
		};

	// This checks if we should be banning them or performing some action based on their new points
	public async void UpdatePunishmentState(CPlayer adminCausingStateChange, bool bIsRepeatOffence)
	{
		// need to be careful we arent already banned, so we dont accidentally extend their ban
		int currentPlayerPoints = await GetActivePunishmentPoints().ConfigureAwait(true);

		int numHours = -1;

		foreach (var kvPair in dictPointsToBanHours)
		{
			int numPoints = kvPair.Key;

			if (currentPlayerPoints >= numPoints)
			{
				numHours = kvPair.Value;
			}
		}

		if (numHours != -1)
		{
			if (bIsRepeatOffence)
			{
				numHours *= 2;
			}

			Ban(numHours, Helpers.FormatString("having {0} punishment points.", currentPlayerPoints), adminCausingStateChange);
		}
	}

	public void OnCharacterChange_SetInitialMoney(float fMoney, float fBankMoney, bool bSave)
	{
		m_fMoney = fMoney;
		m_fBankMoney = fBankMoney;

		if (bSave)
		{
			Internal_SaveMoney();
			Internal_SaveBankMoney();
		}
	}

	public void AddMoney(float fAmount, PlayerMoneyModificationReason reason)
	{
		if (fAmount < 0.0f)
		{
			throw new Exception("AddMoney: Attempting to add negative value");
		}

		new Logging.Log(this, Logging.ELogType.PlayerMoneyChange, null, Helpers.FormatString("Player {0} On-Hand Money modified (Add) with reason {1}, ModifyAmount: {2} MoneyBefore: {3} MoneyAfter: {4}",
			GetCharacterName(ENameType.StaticCharacterName), reason.ToString(), fAmount, m_fMoney, m_fMoney + fAmount)).execute();

		m_fMoney += fAmount;
		Internal_SaveMoney();
	}

	public void SetMoney(float fAmount, PlayerMoneyModificationReason reason)
	{
		if (fAmount < 0.0f)
		{
			throw new Exception("SetMoney: Attempting to set negative value");
		}

		new Logging.Log(this, Logging.ELogType.PlayerMoneyChange, null, Helpers.FormatString("Player {0} On-Hand Money modified (Set) with reason {1}, SetAmount: {2} MoneyBefore: {3} MoneyAfter: {4}",
			GetCharacterName(ENameType.StaticCharacterName), reason.ToString(), fAmount, m_fMoney, fAmount)).execute();

		m_fMoney = fAmount;
		Internal_SaveMoney();
	}

	public void RemoveMoney(float fAmount, PlayerMoneyModificationReason reason)
	{
		if (fAmount < 0.0f)
		{
			throw new Exception("AddMoney: Attempting to remove negative value");
		}

		new Logging.Log(this, Logging.ELogType.PlayerMoneyChange, null, Helpers.FormatString("Player {0} On-Hand Money modified (Remove) with reason {1}, ModifyAmount: {2} MoneyBefore: {3} MoneyAfter: {4}",
			GetCharacterName(ENameType.StaticCharacterName), reason.ToString(), fAmount, m_fMoney, m_fMoney - fAmount)).execute();

		m_fMoney -= fAmount;
		Internal_SaveMoney();
	}

	private void Internal_SaveMoney()
	{
		if (IsSpawned)
		{
			Database.LegacyFunctions.SetPlayerMoney(ActiveCharacterDatabaseID, m_fMoney);
		}

		SetData(m_Client, EDataNames.MONEY, m_fMoney, EDataType.Synced);
	}

	public float Money
	{
		get => m_fMoney;
	}

	public void AddBankMoney(float fAmount, PlayerMoneyModificationReason reason)
	{
		if (fAmount < 0.0f)
		{
			throw new Exception("AddBankMoney: Attempting to add negative value");
		}

		new Logging.Log(this, Logging.ELogType.PlayerMoneyChange, null, Helpers.FormatString("Player {0} Bank Money modified (Add) with reason {1}, ModifyAmount: {2} MoneyBefore: {3} MoneyAfter: {4}",
			GetCharacterName(ENameType.StaticCharacterName), reason.ToString(), fAmount, m_fBankMoney, m_fBankMoney + fAmount)).execute();

		m_fBankMoney += fAmount;
		Internal_SaveBankMoney();
	}

	public void SetBankMoney(float fAmount, PlayerMoneyModificationReason reason)
	{
		if (fAmount < 0.0f)
		{
			throw new Exception("SetBankMoney: Attempting to set negative value");
		}

		new Logging.Log(this, Logging.ELogType.PlayerMoneyChange, null, Helpers.FormatString("Player {0} Bank Money modified (Set) with reason {1}, SetAmount: {2} MoneyBefore: {3} MoneyAfter: {4}",
			GetCharacterName(ENameType.StaticCharacterName), reason.ToString(), fAmount, m_fBankMoney, fAmount)).execute();

		m_fBankMoney = fAmount;
		Internal_SaveBankMoney();
	}

	public void RemoveBankMoney(float fAmount, PlayerMoneyModificationReason reason)
	{
		if (fAmount < 0.0f)
		{
			throw new Exception("AddBankMoney: Attempting to remove negative value");
		}

		new Logging.Log(this, Logging.ELogType.PlayerMoneyChange, null, Helpers.FormatString("Player {0} Bank Money modified (Remove) with reason {1}, ModifyAmount: {2} MoneyBefore: {3} MoneyAfter: {4}",
			GetCharacterName(ENameType.StaticCharacterName), reason.ToString(), fAmount, m_fBankMoney, m_fBankMoney - fAmount)).execute();

		m_fBankMoney -= fAmount;
		Internal_SaveBankMoney();
	}

	private void Internal_SaveBankMoney()
	{
		if (IsSpawned)
		{
			Database.LegacyFunctions.SetPlayerBankMoney(ActiveCharacterDatabaseID, m_fBankMoney);
		}

		SetData(m_Client, EDataNames.BANK_MONEY, m_fBankMoney, EDataType.Synced);
	}

	public float BankMoney
	{
		get => m_fBankMoney;
	}

	public float PendingJobMoney
	{
		get => m_fPendingJobMoney;
		set
		{
			if (IsSpawned)
			{
				Database.LegacyFunctions.SetPlayerPendingJobMoney(ActiveCharacterDatabaseID, value);
			}

			m_fPendingJobMoney = value;
		}
	}

	public EJobID Job
	{
		get => m_JobID;
		set
		{
			if (IsSpawned)
			{
				Database.LegacyFunctions.SetPlayerJob(ActiveCharacterDatabaseID, value);
			}

			SetData(m_Client, EDataNames.JOB_ID, (int)value, EDataType.Synced);
			m_JobID = value;

			ApplySkinFromInventory();
		}
	}

	public Int64 ActiveCharacterDatabaseID
	{
		get => m_DatabaseID;
		set
		{
			SetData(m_Client, EDataNames.CHARACTER_ID, value, EDataType.Synced);
			m_DatabaseID = value;
		}
	}

	public int PlayerID
	{
		get => m_PlayerID;
		set
		{
			SetData(m_Client, EDataNames.PLAYER_ID, value, EDataType.Synced);
			m_PlayerID = value;
		}
	}
	public UInt32 GetPlayerSpecificDimension()
	{
		// NOTE: Must also update in clientside WorldHelper
		return UInt32.MaxValue - (UInt32)AccountID;
	}

	public void GotoPlayerSpecificDimension()
	{
		Dimension playerSpecificDimension = GetPlayerSpecificDimension();

		// store old dimension (only if its not the player specific one)
		if (Client.Dimension != playerSpecificDimension)
		{
			NonPlayerSpecificDimension = Client.Dimension;
		}

		SetSafeDimension(playerSpecificDimension);
	}

	public UInt32 GetPlayerNonSpecificDimension()
	{
		return NonPlayerSpecificDimension;
	}

	public void GotoNonPlayerSpecificDimension()
	{
		// restore old dimension
		SetSafeDimension(NonPlayerSpecificDimension);
	}

	public void RemoveFromVehicle()
	{
		if (Client.Vehicle != null)
		{
			NetworkEventSender.SendNetworkEvent_ExitVehicleReal(this, Client.Vehicle);
			Client.WarpOutOfVehicle();
		}
	}

	public int PaydayProgress
	{
		get => m_iPaycheckProgress;
		set
		{
			if (IsSpawned)
			{
				Database.Functions.Characters.SetPaydayProgress(ActiveCharacterDatabaseID, value);
			}

			m_iPaycheckProgress = value;
		}
	}

	public ECharacterLanguage GetActiveLanguage()
	{
		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.Active)
			{
				return charLanguage.CharacterLanguage;
			}
		}

		return ECharacterLanguage.None;
	}

	public float GetActiveLanguageProgress()
	{
		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.Active)
			{
				return charLanguage.Progress;
			}
		}

		return 0.0f;
	}

	public async void SetLanguageProgress(ECharacterLanguage characterLanguage, float LanguageProgress)
	{
		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.CharacterLanguage == characterLanguage)
			{
				await Database.LegacyFunctions.SetCharacterLanguageProgress(ActiveCharacterDatabaseID, charLanguage.CharacterLanguage, LanguageProgress).ConfigureAwait(true);
				charLanguage.Progress = LanguageProgress;
				return;
			}
		}
	}

	public float GetLanguageProgress(ECharacterLanguage characterLanguage)
	{
		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.CharacterLanguage == characterLanguage)
			{
				return charLanguage.Progress;
			}
		}
		return 0.0f;
	}

	public async void RemoveCharacterActiveLanguage()
	{
		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.Active)
			{
				await Database.LegacyFunctions.RemoveCharacterActiveLanguage(ActiveCharacterDatabaseID, charLanguage.CharacterLanguage).ConfigureAwait(true);
				charLanguage.Active = false;
				return;
			}
		}
	}

	public async void UpdateCharacterActiveLanguage(ECharacterLanguage newCharacterLanguage)
	{
		await Database.LegacyFunctions.SetCharacterActiveLanguage(ActiveCharacterDatabaseID, newCharacterLanguage).ConfigureAwait(true);
		CCharacterLanguage newCharLang = GetCharacterLanguageOfType(newCharacterLanguage);
		newCharLang.Active = true;
	}

	public List<CCharacterLanguage> GetCharacterLanguages()
	{
		return m_CharacterLanguages;
	}

	public CCharacterLanguage GetCharacterLanguageOfType(ECharacterLanguage a_CharacterLanguage)
	{
		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.CharacterLanguage == a_CharacterLanguage)
			{
				return charLanguage;
			}
		}

		return null;
	}

	public async void AwardXPForLanguage(ECharacterLanguage langToBeAwarded, float fAmountOfXP)
	{
		float fNewAmount;

		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.CharacterLanguage == langToBeAwarded)
			{
				fNewAmount = charLanguage.Progress + fAmountOfXP;
				if (fNewAmount > 100f)
				{
					fNewAmount = 100f;
				}

				await Database.LegacyFunctions.AddXPForLanguage(ActiveCharacterDatabaseID, langToBeAwarded, fNewAmount).ConfigureAwait(true);

				if (fNewAmount == 100f)
				{
					AwardAchievement(EAchievementID.LearnLanguage);
				}

				charLanguage.Progress = fNewAmount;

				return;
			}
		}
	}

	public bool KnowsLanguageOfType(ECharacterLanguage a_CharacterLanguage)
	{
		foreach (CCharacterLanguage charLanguage in m_CharacterLanguages)
		{
			if (charLanguage.CharacterLanguage == a_CharacterLanguage)
			{
				return true;
			}
		}

		return false;
	}

	public void AddLanguageForPlayer(CCharacterLanguage a_CharacterLanguage, bool bAddToDatabase)
	{
		if (bAddToDatabase)
		{
			Database.Functions.Characters.AddLanguage(ActiveCharacterDatabaseID, a_CharacterLanguage.CharacterLanguage, a_CharacterLanguage.Progress, a_CharacterLanguage.Active);
		}

		m_CharacterLanguages.Add(a_CharacterLanguage);
	}

	public void RemoveLanguageForPlayer(CCharacterLanguage a_CharacterLanguage)
	{
		m_CharacterLanguages.Remove(a_CharacterLanguage);
		Database.LegacyFunctions.RemoveLanguageForPlayer(ActiveCharacterDatabaseID, a_CharacterLanguage.CharacterLanguage);
	}

	public void AddFactionMembership(CFactionMembership a_FactionMembership, bool bAddToDatabase)
	{
		if (!IsInFaction(a_FactionMembership.Faction.FactionID))
		{
			if (bAddToDatabase)
			{
				Database.LegacyFunctions.AddFactionMembership(a_FactionMembership.Faction.FactionID, ActiveCharacterDatabaseID, a_FactionMembership.Rank, a_FactionMembership.Manager);
			}

			m_lstFactionMemberships.Add(a_FactionMembership);
		}
	}



	// TODO_LAUNCH: This doesnt remove from DB?
	public void RemoveFactionMembership(CFactionMembership a_FactionMembership)
	{
		EFactionType factionTypeBeingLeft = a_FactionMembership.Faction.Type;
		m_lstFactionMemberships.Remove(a_FactionMembership);

		// if the player was on duty, and is no longer in an eligible faction, go off duty
		if (m_DutyType == EDutyType.EMS || m_DutyType == EDutyType.Fire)
		{
			if (a_FactionMembership.Faction.Type == EFactionType.Medical && !IsInFactionOfType(EFactionType.Medical))
			{
				GoOffDuty();
			}
		}
		else if (m_DutyType == EDutyType.Law_Enforcement)
		{
			if (a_FactionMembership.Faction.Type == EFactionType.LawEnforcement && !IsInFactionOfType(EFactionType.LawEnforcement))
			{
				GoOffDuty();
			}
		}
	}

	public void RemoveFactionMembership(CFaction a_Faction)
	{
		foreach (CFactionMembership membership in m_lstFactionMemberships)
		{
			if (membership.Faction == a_Faction)
			{
				m_lstFactionMemberships.Remove(membership);
				return;
			}
		}
	}

	public List<CFactionMembership> GetFactionMemberships()
	{
		return m_lstFactionMemberships;
	}

	public CFactionMembership GetFactionMembershipFromFaction(CFaction a_Faction)
	{
		foreach (CFactionMembership membership in m_lstFactionMemberships)
		{
			if (membership.Faction == a_Faction)
			{
				return membership;
			}
		}

		return null;
	}

	public bool IsInAircraft()
	{
		if (Client.Vehicle == null)
		{
			return false;
		}
		else
		{
			CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(Client.Vehicle);
			return vehicle.IsAircraft();
		}
	}


	public void SetFrisking(CPlayer a_Player, bool bAdmin = false)
	{
		m_Frisking.SetTarget(a_Player);
		m_FriskingAsAdmin = bAdmin;
	}

	public bool GetFriskingAsAdmin()
	{
		return m_FriskingAsAdmin;
	}

	public bool IsFriskingPlayer(CPlayer a_Player)
	{
		return m_Frisking.Instance() == a_Player;
	}

	public bool IsFriskingAnyone()
	{
		return m_Frisking != null;
	}

	public CPlayer GetPlayerBeingFrisked()
	{
		return m_Frisking.Instance();
	}

	public CVehicle GetPlayerVehicleIsIn()
	{
		if (IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(m_Client.Vehicle);
			if (pVehicle != null)
			{
				return pVehicle;
			}
		}

		return null;
	}

	// Achievements
	public bool AwardAchievement(EAchievementID a_AchievementID)
	{
		if (a_AchievementID != EAchievementID.None)
		{
			if (!HasAchievement(a_AchievementID))
			{
				Int64 unixTimestamp = Helpers.GetUnixTimestamp();

				// work out rarity
				CAchievementDefinition achievementDef = AchievementDefinitions.g_AchievementDefinitions[a_AchievementID];
				Database.Functions.Achievements.GetPercentOfActiveUsersWithAchievement(true, AccountID, achievementDef.AchievementID, (int percentOfUsers, EAchievementRarity rarity) =>
				{
					Database.Functions.Achievements.GivePlayerAchievement(AccountID, a_AchievementID, unixTimestamp, async (EntityDatabaseID dbid) =>
					{
						m_dictAchievements[a_AchievementID] = new CAchievementInstance(dbid, AccountID, a_AchievementID, unixTimestamp);

						CAchievementDefinition achievementDef = AchievementDefinitions.g_AchievementDefinitions[a_AchievementID];
						NetworkEventSender.SendNetworkEvent_AwardAchievement(this, (int)a_AchievementID, achievementDef.Title, achievementDef.Caption, achievementDef.Points, rarity, percentOfUsers);

						// give GC
						await Database.LegacyFunctions.AddDonatorCurrency(AccountID, achievementDef.Points).ConfigureAwait(true);
					});
				});

				return true;
			}
		}

		return false;
	}

	public bool HasAchievement(EAchievementID a_AchievementID)
	{
		return m_dictAchievements.ContainsKey(a_AchievementID);
	}

	public void CopyAchievements(Dictionary<EAchievementID, CAchievementInstance> a_Source)
	{
		if (a_Source != null)
		{
			m_dictAchievements = a_Source;
		}
	}

	public Dictionary<EAchievementID, CAchievementInstance> GetAchievments()
	{
		return m_dictAchievements;
	}

	private Dictionary<EAchievementID, CAchievementInstance> m_dictAchievements = new Dictionary<EAchievementID, CAchievementInstance>();

	private WeakReference<MainThreadTimer> m_VehicleEnterTimer = new WeakReference<MainThreadTimer>(null);

	private List<Database.Models.CustomAnim> m_lstCustomAnims = new List<Database.Models.CustomAnim>();
	public void LoadCustomAnimations()
	{
		Database.Functions.Accounts.LoadCustomAnims(AccountID,
			(List<Database.Models.CustomAnim> lstAnims) =>
			{
				// NOTE: we do this in the callback so there is no race condition where the client could send back its list before the SQL query finishes meaning we wipe the member variable
				m_lstCustomAnims = lstAnims;

				// Request from client too (if we have space)
				if (HasSpaceForMoreCustomAnimations())
				{
					NetworkEventSender.SendNetworkEvent_CustomAnim_RequestClientLegacyLoad(this);
				}
			});
	}

	public bool HasSpaceForMoreCustomAnimations()
	{
		const int customAnimLimit = 100;
		return (m_lstCustomAnims.Count < customAnimLimit);
	}

	public void AddCustomAnimation(Database.Models.CustomAnim newAnim)
	{
		m_lstCustomAnims.Add(newAnim);
	}

	public bool DeleteCustomAnimation(string strCMD)
	{
		Database.Models.CustomAnim anim = GetCustomAnim(strCMD);
		if (anim != null)
		{
			anim.Delete(() =>
			{
				m_lstCustomAnims.Remove(anim);
			});
			return true;
		}

		return false;
	}

	public Database.Models.CustomAnim GetCustomAnim(string strCMD)
	{
		strCMD = strCMD.ToLower();
		foreach (var customAnim in m_lstCustomAnims)
		{
			if (customAnim.CommandName.ToLower() == strCMD)
			{
				return customAnim;
			}
		}

		return null;
	}

	public List<Database.Models.CustomAnim> GetCustomAnims()
	{
		return m_lstCustomAnims;
	}

	public bool IsInCall(out ECallState a_State)
	{
		a_State = m_CallState;
		return m_CallingPlayer.Instance() != null;
	}

	public CPlayer GetPlayerIsInCallWith()
	{
		return m_CallingPlayer.Instance();
	}

	public void ResetInCall()
	{
		m_CallingPlayer = new WeakReference<CPlayer>(null);
		// clear anim

		// Was phone in use? If so call SetPhoneInUse, this will set us up with the phone anim again rather than the calling anum
		if (GetPhoneInUse() != null)
		{
			SetPhoneInUse(GetPhoneInUse());
		}

		SetCallState(ECallState.None);
	}

	public enum ECallState
	{
		None,
		Connecting,
		Connected,
		Incoming
	}

	private ECallState m_CallState;
	public CScriptedCall CallingHotline { get; set; } = null;

	public bool IsCallingHotline { get; set; } = false;

	public void SetInCall(CPlayer a_RemotePlayer)
	{
		m_CallingPlayer.SetTarget(a_RemotePlayer);

		if (m_CallState != ECallState.Incoming)
		{
			int flagsLoop = (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl);
			int flagsFreeze = (int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl);
			AddAnimationToQueue(flagsLoop, "amb@code_human_wander_mobile@male@enter", "enter", false, true, false, 3500, false);
			AddAnimationToQueue(flagsFreeze, "amb@code_human_wander_mobile@male@idle_a", "idle_b", false, false, true, 0, false);
			AddAnimationToQueue(flagsLoop, "amb@code_human_wander_mobile@male@exit", "exit", false, false, false, 2500, false);
			if (!IsInVehicleReal)
			{
				SetData(Client, EDataNames.HAS_CELL, 2, EDataType.Synced);
			}
		}
	}

	public void SetCallState(ECallState a_CallState)
	{
		// Were we connected and no longer?
		if (m_CallState == ECallState.Connected && a_CallState == ECallState.None)
		{
			// check that we actually made the call
			if (!m_bCallIsIncoming)
			{
				// did it have a valid player (aka it wasn't a hotline)
				if (m_CallingPlayer.Instance() != null)
				{
					// Do we have a donation perk that stops us paying for calls?
					if (!DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.FreeCalls))
					{
						// subtract money
						Int64 callDurationSec = (MainThreadTimerPool.GetMillisecondsSinceDateTime(m_CallDurationStartTime) / 1000);
						float fCost = (float)(0.50f * Math.Ceiling((double)(callDurationSec / 30.0))); // 50 cents per 30 sec
						SubtractMoneyAllowNegative(fCost, PlayerMoneyModificationReason.PhoneCall);
						SendNotification("Phone", ENotificationIcon.Phone, "You spent ${0:0.00} on a phone call.", fCost);
					}
					else
					{
						SendNotification("Phone", ENotificationIcon.Phone, "Your phone call was free due to having an active donator perk.", null);
					}
				}
			}
		}

		m_CallState = a_CallState;

		if (m_CallState == ECallState.Connecting)
		{
			m_CallStartTime = DateTime.Now;
		}
		else if (m_CallState == ECallState.Connected)
		{
			m_CallDurationStartTime = DateTime.Now;
		}

		// Is the call incoming?
		if (a_CallState == ECallState.Incoming)
		{
			m_bCallIsIncoming = true;
		}
		else if (a_CallState == ECallState.Connecting)
		{
			m_bCallIsIncoming = false;
		}
	}

	public bool HasOutgoingCallExpired()
	{
		return (m_CallState == ECallState.Connecting && MainThreadTimerPool.GetMillisecondsSinceDateTime(m_CallStartTime) > 10000);
	}

	private WeakReference<CPlayer> m_CallingPlayer = new WeakReference<CPlayer>(null);

	public CItemValueCellphone GetPhoneInUse()
	{
		return m_PhoneInUse;
	}

	public void SetPhoneNoLongerInUse()
	{
		//NOTE: No need to check here for a vehicle as we want to destroy the phone in any case when no longer in use.
		m_PhoneInUse = null;
		SetData(Client, EDataNames.HAS_CELL, 0, EDataType.Synced);
	}

	public void SetPhoneInUse(CItemValueCellphone a_Phone)
	{
		m_PhoneInUse = a_Phone;
		m_LastUsedPhone = a_Phone;

		if (!GetData<bool>(m_Client, EDataNames.HAS_ANIM))
		{
			int flagsLoop = (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl);
			int flagsFreeze = (int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl);
			AddAnimationToQueue(flagsLoop, "amb@code_human_wander_texting@male@enter", "enter", false, true, false, 3500, false);
			AddAnimationToQueue(flagsFreeze, "amb@code_human_wander_texting@male@idle_a", "idle_a", false, false, true, 0, false);
			AddAnimationToQueue(flagsLoop, "amb@code_human_wander_texting@male@exit", "exit", false, false, false, 2500, false);
		}

		if (!IsInVehicleReal)
		{
			SetData(Client, EDataNames.HAS_CELL, 1, EDataType.Synced);
		}

	}

	public CItemValueCellphone GetLastUsedPhone()
	{
		return m_LastUsedPhone;
	}

	public void ResetLastUsedPhone()
	{
		m_LastUsedPhone = null;
	}
	private CItemValueCellphone m_PhoneInUse;
	private CItemValueCellphone m_LastUsedPhone;

	// Disabled due to a lag bug. Using a variable here to avoid warnings.
	private bool m_disabledAnimations = false;

	public bool IsDaniels()
	{
		return Username.ToLower() == "danielsdev";
	}

	public void AddAnimationToQueue(int a_Flags, string strDictionaryName, string strAnimationName, bool a_bPlayerCanCancel, bool a_bImmediate, bool a_bBlockUntilScriptCancel, int a_DurationMS, bool a_bIsPlayerRequested, bool a_bFreeze = false, float a_Correction = 0.0f)
	{
		if (m_disabledAnimations)
		{
			SendNotification("Animation", ENotificationIcon.ExclamationSign, "Animations are currently disabled.",
				null);
			return;
		}

		if (a_DurationMS > 0)
		{
			a_bBlockUntilScriptCancel = false;
		}

		// Are we currently in a forced animation and the requesting one is not forced?
		if (GetData<bool>(Client, EDataNames.HAS_ANIM) && !GetData<bool>(Client, EDataNames.ANIM_CANCELLABLE) && a_bPlayerCanCancel)
		{
			SendNotification("Animation", ENotificationIcon.ExclamationSign, "You cannot change your animation currently.", null);
		}
		else
		{
			if (a_bImmediate)
			{
				// If player requested, we don't actually dump the anims but we just tell the player no if any of them are not cancellable
				if (a_bIsPlayerRequested)
				{
					foreach (CPlayerAnimationEntry pendingAnim in m_queuePendingAnimations)
					{
						if (!pendingAnim.PlayerCanCancel)
						{
							SendNotification("Animation", ENotificationIcon.ExclamationSign, "You cannot start an animation at this time.", null);
							return;
						}
					}
				}

				// Dump the queue
				m_queuePendingAnimations.Clear();
			}

			CPlayerAnimationEntry newAnim = new CPlayerAnimationEntry(a_Flags, strDictionaryName, strAnimationName, a_bPlayerCanCancel, a_bBlockUntilScriptCancel, a_DurationMS, a_bFreeze, a_Correction);
			m_queuePendingAnimations.Enqueue(newAnim);
		}
	}

	public void StopCurrentAnimation(bool a_bForce = false, bool a_bClearPendingAnimations = false)
	{
		if (a_bForce || (GetData<bool>(Client, EDataNames.HAS_ANIM) && GetData<bool>(Client, EDataNames.ANIM_CANCELLABLE)))
		{
			if (m_queuePendingAnimations.Count > 0)
			{
				CPlayerAnimationEntry currentAnim = m_queuePendingAnimations.Dequeue();
				currentAnim.Stop(this);
			}
		}
		else
		{
			SendNotification("Animation", ENotificationIcon.ExclamationSign, "You cannot stop your animation currently.", null);
		}

		if (a_bClearPendingAnimations)
		{
			m_queuePendingAnimations.Clear();
		}
	}

	private void UpdateAnimationQueue()
	{
		// Do we have anything?
		if (m_queuePendingAnimations.Count > 0)
		{
			// Has the front element expired?
			CPlayerAnimationEntry currentAnimation = m_queuePendingAnimations.Peek();

			if (!currentAnimation.HasStarted())
			{
				currentAnimation.Start(this);
			}

			if (currentAnimation.HasExpired())
			{
				StopCurrentAnimation(true);
				// StopAnimation also pops front
			}
		}
	}

	// BEGIN MEMBER VARIABLES
	private Dimension NonPlayerSpecificDimension = 0;

	private EApplicationState m_ApplicationState = EApplicationState.NoApplicationSubmitted;
	public EApplicationState ApplicationState
	{
		get => m_ApplicationState;
	}

	public UInt32 NumberOfApplicationsSubmitted { get; set; }

	// TODO_CELLPHONE: Reset animation queue on change char
	private Queue<CPlayerAnimationEntry> m_queuePendingAnimations = new Queue<CPlayerAnimationEntry>();

	private DateTime m_CallStartTime = DateTime.Now;
	private DateTime m_CallDurationStartTime = DateTime.Now;
	private bool m_bCallIsIncoming = false;

	public DateTime LastSendPing { get; set; }

	private Vector3 m_vecCachedSavePosition;
	private float m_fCachedSaveRotation;
	private Dimension m_CachedDimension;

	private Player m_Client;

	private bool m_bIsLoggedIn = false;
	private bool m_bIsSpawned = false;
	private float m_fMoney = 0.0f;
	private float m_fPendingJobMoney = 0.0f;
	private float m_fBankMoney = 0.0f;
	private int m_AccountID = 0;
	private string m_Username;
	private EAdminLevel m_AdminLevel;
	private EScripterLevel m_ScripterLevel;
	private bool m_AdminDuty = false;
	private int m_AdminReportCount = 0;
	private bool m_localPlayerNametagToggled = false;
	private EJobID m_JobID = EJobID.None;
	public EGender Gender { get; set; } = EGender.Male;
	public Int64 AutoSpawnCharacter { get; set; } = -1;

	public void SetCharacterNameOnSpawn(string strName)
	{
		SetData(m_Client, EDataNames.CHARACTER_NAME, strName, EDataType.Synced); //THIS IS FOR PLAYERLIST
		SetCharacterName(ENameType.StaticCharacterName, strName);
		SetCharacterName(ENameType.CharacterDisplayName, strName);
	}

	public void SetCharacterNameOnConnect()
	{
		string strTempName = Helpers.FormatString("User {0}", m_PlayerID);
		SetCharacterName(ENameType.StaticCharacterName, strTempName);
		SetCharacterName(ENameType.CharacterDisplayName, strTempName);
	}

	public void SetCharacterNameOnLogin()
	{
		SetCharacterName(ENameType.StaticCharacterName, Username);
		SetCharacterName(ENameType.CharacterDisplayName, Username);
	}

	// NOTE: should never set these directly, make a case specific setter function instead
	private void SetCharacterName(ENameType nameType, string strName)
	{
		m_dictNames[nameType] = strName;

		// This is our actual display name for the client
		if (nameType == ENameType.CharacterDisplayName)
		{
			m_Client.Name = strName;
		}
	}

	public string GetCharacterName(ENameType nameType, CPlayer a_RequestingPlayer = null, bool bAdminOnDutyOverride = false)
	{
		if (a_RequestingPlayer != null && bAdminOnDutyOverride)
		{
			// is it an admin?
			if (a_RequestingPlayer.IsAdmin(EAdminLevel.TrialAdmin, true))
			{
				// If it is display name, masked etc, give back the raw name
				if (nameType == ENameType.CharacterDisplayName)
				{
					nameType = ENameType.StaticCharacterName;
				}
			}
		}

		if (m_dictNames.ContainsKey(nameType))
		{
			return m_dictNames[nameType];
		}

		return String.Empty;
	}

	public bool DoesAnyCharacterNameMatch(string strDesiredExactName)
	{
		foreach (var kvPair in m_dictNames)
		{
			if (kvPair.Value.ToLower() == strDesiredExactName.ToLower())
			{
				return true;
			}
		}

		return false;
	}

	public bool DoesAnyCharacterNameMatchPartial(string strDesiredPartialName)
	{
		foreach (var kvPair in m_dictNames)
		{
			if (kvPair.Value.ToLower().Contains(strDesiredPartialName.ToLower()))
			{
				return true;
			}
		}

		return false;
	}

	private Dictionary<ENameType, string> m_dictNames = new Dictionary<ENameType, string>();

	public bool GetDiscordID(out UInt64 discordID)
	{
		discordID = m_DiscordID;
		return m_DiscordID != 0;
	}

	public async void SetDiscordID(UInt64 discordID, bool bSaveToDB)
	{
		m_DiscordID = discordID;

		if (bSaveToDB)
		{
			await Database.LegacyFunctions.UpdateAccountDiscordLink(AccountID, discordID).ConfigureAwait(true);
		}
	}

	public void GetDiscordUsername(Action<bool, string> MainThreadCompletionCallback)
	{
		DiscordBotIntegration.GetDiscordUsernameFromID(m_DiscordID, MainThreadCompletionCallback);
	}

	private UInt64 m_DiscordID = 0;
	public string DiscordOwlToken { get; set; } = String.Empty;

	private List<CCharacterLanguage> m_CharacterLanguages = new List<CCharacterLanguage>();
	private List<CFactionMembership> m_lstFactionMemberships = new List<CFactionMembership>();
	private CPlayerInventory m_Inventory;
	private CDonationInventory m_DonationInventory;
	private PersistentNotificationManager m_Notifications;
	private WeakReference<MainThreadTimer> m_FishingTimer = new WeakReference<MainThreadTimer>(null);
	private readonly WeakReference<MainThreadTimer> m_SaveTimer = new WeakReference<MainThreadTimer>(null);
	private readonly WeakReference<MainThreadTimer> m_TimePlayedThisSessionTimer = new WeakReference<MainThreadTimer>(null);
	private readonly WeakReference<MainThreadTimer> m_SyncInventoryWithWeaponsTimer = new WeakReference<MainThreadTimer>(null);
	private readonly WeakReference<MainThreadTimer> m_SaveInventoryTimer = new WeakReference<MainThreadTimer>(null);
	private WeakReference<MainThreadTimer> m_UpdateDutyPositionTimer = new WeakReference<MainThreadTimer>(null);
	private WeakReference<MainThreadTimer> m_PayDayTimer = new WeakReference<MainThreadTimer>(null);
	private int m_iPaycheckProgress = 0;
	private int m_PlayerID = -1;

	private WeakReference<MainThreadTimer> m_SpawnDimensionFixTimer = new WeakReference<MainThreadTimer>(null);

	private WeakReference<MainThreadTimer> m_PreviewTimer = new WeakReference<MainThreadTimer>(null);

	private EDutyType m_DutyType = EDutyType.None;

	public bool IsInFactionMenu { get; set; } = false;

	private bool m_bCuffed = false;
	private EntityDatabaseID m_Cuffer = 0;
	private WeakReference<CPlayer> m_Frisking = new WeakReference<CPlayer>(null);
	private bool m_FriskingAsAdmin = false;

	private Int64 m_unixTimestampUnjail = 0;
	private EPrisonCell m_PrisonCell = EPrisonCell.One;
	private float m_fBailAmount = 0.0f;

	public void CopyWeaponDataClientside(List<WeaponHash> lstWeaponsFull)
	{
		m_lstWeaponsClientside = lstWeaponsFull;
	}

	public List<WeaponHash> GetWeaponDataClientside()
	{
		return m_lstWeaponsClientside;
	}

	public void RemoveWeaponDataClientside(WeaponHash weapon)
	{
		m_lstWeaponsClientside.Remove(weapon);
	}

	public void CopyAmmoData(Dictionary<EWeapons, int> diffAmmoData)
	{
		// This is a diff, can't just copy over
		foreach (var kvPair in diffAmmoData)
		{
			// TODO: Better mechanism? Could be exploited, sort of
			// HACK: check the values arent crazy off target, it probably means the skin changed this frame and they lost all weapons (Thanks Rockstar). We will re-give them next frame.
			//		 This fixes the bug with random zero ammo

			bool bCanApply = true;

			if (m_AmmoData.ContainsKey(kvPair.Key))
			{
				int currentAmmo = m_AmmoData[kvPair.Key];
				int newAmmo = kvPair.Value;
				if (currentAmmo > 0 && newAmmo == 0)
				{
					if (currentAmmo > 2) // 2 for safety, really 1 is probably fine
					{
						bCanApply = false;
					}
				}
			}

			if (bCanApply)
			{
				m_AmmoData[kvPair.Key] = kvPair.Value;
			}
		}
	}

	public Dictionary<EWeapons, int> GetAmmoData()
	{
		return m_AmmoData;
	}

	private Dictionary<EWeapons, int> m_AmmoData = new Dictionary<EWeapons, int>();
	private List<WeaponHash> m_lstWeaponsClientside = new List<WeaponHash>();

	// IMPAIRMENT
	/// <summary>
	/// Drunk walking
	/// </summary>
	/// <value>Up to 0.3 low, until 0.6 moderate, 1.0 severe</value>
	public float ImpairmentLevel
	{
		get
		{
			return GetData<float>(m_Client, EDataNames.IMPAIRMENT);
		}
		set
		{
			if (value <= 0.0f)
			{
				value = 0.0f;
			}

			// upper bound
			if (value >= 3.0f)
			{
				value = 3.0f;
			}

			m_LastImpairmentUpdate = DateTime.Now;
			SetData(m_Client, EDataNames.IMPAIRMENT, value, EDataType.Synced);

			if (value > 0.0f && value <= 0.3f)
			{
				MoveClipset = EClipsetID.DrunkLow;
			}
			else if (value > 0.3f && value <= 0.6f)
			{
				MoveClipset = EClipsetID.DrunkMed;
			}
			else if (value > 0.6f)
			{
				MoveClipset = EClipsetID.DrunkHigh;
			}
			else if (value <= 0.0f && !IsCrouched)
			{
				ClearMoveClipset();
			}
		}
	}

	public void ClearMoveClipset()
	{
		MoveClipset = 0;
	}

	public void ClearStrafeClipset()
	{
		StrafeClipset = 0;
	}

	public EClipsetID MoveClipset
	{
		get
		{
			return m_MoveClipset;
		}
		set
		{
			if (value != m_MoveClipset)
			{
				if (value == EClipsetID.None)
				{
					ClearData(Client, EDataNames.MOVE_CLIPSET);
				}
				else
				{
					SetData(Client, EDataNames.MOVE_CLIPSET, value, EDataType.Synced);
				}
			}

			m_MoveClipset = value;
		}
	}

	public EClipsetID StrafeClipset
	{
		get
		{
			return m_StrafeClipset;
		}
		set
		{
			if (value != m_StrafeClipset)
			{

				if (value == EClipsetID.None)
				{
					ClearData(Client, EDataNames.STRAFE_CLIPSET);
				}
				else
				{
					SetData(Client, EDataNames.STRAFE_CLIPSET, value, EDataType.Synced);
				}
			}

			m_StrafeClipset = value;
		}
	}

	private EClipsetID m_MoveClipset = 0;
	private EClipsetID m_StrafeClipset = 0;

	public void IncreaseImpairmentLevel(float fAmount) { ImpairmentLevel += fAmount; }
	public void DecreaseImpairmentLevel(float fAmount) { ImpairmentLevel -= fAmount; }
	private DateTime m_LastImpairmentUpdate = DateTime.Now;

	// DRUG EFFECTS
	class DrugEffectData
	{
		public DrugEffectData(DateTime a_startTime, Int64 a_duration)
		{
			StartTime = a_startTime;
			Duration = a_duration;
		}

		public DateTime StartTime { get; set; }
		public Int64 Duration { get; set; }
	}

	Dictionary<EDrugEffect, DrugEffectData> m_dictDrugDurations = new Dictionary<EDrugEffect, DrugEffectData>();

	/// <summary>
	/// Enables a drug effect on the player.
	/// </summary>
	/// <param name="a_DrugName">The drug effect to apply (DRUG_FX_n) where n is an integer</param>
	/// <param name="a_Duration">Duration in milliseconds</param>
	public void SetDrugEffectEnabled(EDrugEffect a_DrugEffect, Int64 a_Duration)
	{
		EDataNames a_DataName = HelperFunctions.Items.GetDataNameFromDrugEffect(a_DrugEffect);

		if (m_dictDrugDurations.ContainsKey(a_DrugEffect))
		{
			m_dictDrugDurations[a_DrugEffect].Duration += a_Duration;
		}
		else
		{
			m_dictDrugDurations[a_DrugEffect] = new DrugEffectData(DateTime.Now, a_Duration);
		}

		SetData(m_Client, a_DataName, true, EDataType.Synced);
	}

	public void SetDrugEffectDisabled(EDrugEffect a_DrugEffect)
	{
		EDataNames a_DataName = HelperFunctions.Items.GetDataNameFromDrugEffect(a_DrugEffect);
		m_dictDrugDurations.Remove(a_DrugEffect);
		SetData(m_Client, a_DataName, false, EDataType.Synced);
	}

	public bool IsDrugEffectEnabled(EDrugEffect a_DrugEffect)
	{
		EDataNames a_DataName = HelperFunctions.Items.GetDataNameFromDrugEffect(a_DrugEffect);
		return GetData<bool>(m_Client, a_DataName);
	}

	public Int64 GetRemainingDrugEffectDuration(EDrugEffect a_DrugEffect)
	{
		if (m_dictDrugDurations.ContainsKey(a_DrugEffect))
		{
			return MainThreadTimerPool.GetMillisecondsSinceDateTime(m_dictDrugDurations[a_DrugEffect].StartTime);
		}

		return 0;
	}
	// END DRUG EFFECTS

	public int GetExperienceForCurrentJob()
	{
		if (m_JobID != EJobID.None)
		{
			switch (m_JobID)
			{
				default:
					{
						throw new Exception(Helpers.FormatString("Unhandled job ({0}) in CPlayer::GetExperienceForCurrentJob", m_JobID));
					}

				case EJobID.TruckerJob:
					{
						return TruckerJobXP;
					}

				case EJobID.DeliveryDriverJob:
					{
						return DeliveryDriverJobXP;
					}

				case EJobID.BusDriverJob:
					{
						return BusDriverJobXP;
					}

				case EJobID.MailmanJob:
					{
						return MailmanJobXP;
					}

				case EJobID.TrashmanJob:
					{
						return TrashmanJobXP;
					}

				case EJobID.TaxiDriverJob:
					{
						return 0; // Taxi has no XP (yet?)
					}

				case EJobID.TagRemoverJob:
					{
						return 0; // infinite job
					}
			}
		}

		return -1;
	}

	public int TruckerJobXP { get; set; }
	public int DeliveryDriverJobXP { get; set; }
	public int BusDriverJobXP { get; set; }
	public int MailmanJobXP { get; set; }
	public int TrashmanJobXP { get; set; }
	public int FishingXP { get; set; }

	// FISH DATA & LOGIC
	public async void UpdateFishingXPAfterCatch()
	{
		int levelBefore = GetFishingLevel();
		FishingXP += 10;
		await Database.LegacyFunctions.SetFishingXP(ActiveCharacterDatabaseID, FishingXP).ConfigureAwait(true);

		int levelAfter = GetFishingLevel();
		// Did we level up?
		if (levelAfter > levelBefore)
		{
			NetworkEventSender.SendNetworkEvent_FishingLevelUp(this, levelAfter, GetFishingXPRequiredForNextLevel());
		}

		if (IsFishingMaxLevel())
		{
			AwardAchievement(EAchievementID.FishingMaxLevel);
		}
	}

	private bool HasFishingRodForCurrentLevel(out EItemID rodTypeRequired, out string strRodName)
	{
		bool bHasItem = false;
		strRodName = String.Empty;

		int currentLevel = GetFishingLevel();

		if (currentLevel < 10)
		{
			CItemInstanceDef ItemInstanceDef_FishingRod = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISHING_ROD_AMATEUR, 0);
			bHasItem = Inventory.HasItem(ItemInstanceDef_FishingRod, false, out CItemInstanceDef itemFound_FishingRod);
			strRodName = ItemDefinitions.g_ItemDefinitions[EItemID.FISHING_ROD_AMATEUR].GetNameIgnoreGenericItems();
			rodTypeRequired = EItemID.FISHING_ROD_AMATEUR;
		}
		else if (currentLevel < 15)
		{
			CItemInstanceDef ItemInstanceDef_FishingRod = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISHING_ROD_INTERMEDIATE, 0);
			bHasItem = Inventory.HasItem(ItemInstanceDef_FishingRod, false, out CItemInstanceDef itemFound_FishingRod);
			strRodName = bHasItem ? "" : ItemDefinitions.g_ItemDefinitions[EItemID.FISHING_ROD_INTERMEDIATE].GetNameIgnoreGenericItems();
			rodTypeRequired = EItemID.FISHING_ROD_INTERMEDIATE;
		}
		else
		{
			CItemInstanceDef ItemInstanceDef_FishingRod = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISHING_ROD_ADVANCED, 0);
			bHasItem = Inventory.HasItem(ItemInstanceDef_FishingRod, false, out CItemInstanceDef itemFound_FishingRod);
			strRodName = bHasItem ? "" : ItemDefinitions.g_ItemDefinitions[EItemID.FISHING_ROD_ADVANCED].GetNameIgnoreGenericItems();
			rodTypeRequired = EItemID.FISHING_ROD_ADVANCED;
		}

		return bHasItem;
	}

	// TODO_FISHING: Remove fishing logic from player class
	public bool IsFishing()
	{
		return GetData<bool>(m_Client, EDataNames.FISHING);
	}

	public void StopFishing()
	{
		bool bIsFishing = GetData<bool>(m_Client, EDataNames.FISHING);
		if (bIsFishing)
		{
			// TODO_FISHING: Clear instead?
			SetData(m_Client, EDataNames.FISHING, false, EDataType.Synced);
			StopCurrentAnimation(true, false);
		}

		DestroyFishingTimer();
	}

	public void TryToggleFishing(EItemID rodTypeBeingUsed)
	{
		bool bIsFishing = GetData<bool>(m_Client, EDataNames.FISHING);
		if (bIsFishing)
		{
			StopFishing();
		}
		else
		{
			// Do we have what we need to start?
			bool bHasRodItem = HasFishingRodForCurrentLevel(out EItemID rodTypeRequired, out string strRodName);

			CItemInstanceDef ItemInstanceDef_FishingLine = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISHING_LINE, 0);
			CItemInstanceDef ItemInstanceDef_FishingCooler = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISH_COOLER_BOX, 0);
			CInventoryItemDefinition itemDefCooler = ItemDefinitions.g_ItemDefinitions[EItemID.FISH_COOLER_BOX];
			if (rodTypeBeingUsed != rodTypeRequired || !bHasRodItem) // were we using the correct rod for our level?
			{
				SendNotification("Fishing", ENotificationIcon.ExclamationSign, Helpers.FormatString("You require a {0}, line and cooler to fish.", strRodName));
			}
			else if (!Inventory.HasItem(ItemInstanceDef_FishingLine, false, out CItemInstanceDef itemFound_FishingLine))
			{
				SendNotification("Fishing", ENotificationIcon.ExclamationSign, Helpers.FormatString("You require a {0}, line and cooler to fish.", strRodName));
			}
			else if (!Inventory.HasItem(ItemInstanceDef_FishingCooler, false, out CItemInstanceDef itemFound_FishingCooler))
			{
				SendNotification("Fishing", ENotificationIcon.ExclamationSign, Helpers.FormatString("You require a {0}, line and cooler to fish.", strRodName));
			}
			else if (Inventory.GetItemsInsideContainer(itemFound_FishingCooler.DatabaseID).Count >= itemDefCooler.ContainerCapacity) // Do we have space in the cooler?
			{
				SendNotification("Fishing", ENotificationIcon.ExclamationSign, "Your fishing cooler is full! Visit the fishmonger to sell your fish.");
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_StartFishing(this, GetFishingLevel(), GetFishingXPRequiredForNextLevel());

				AwardAchievement(EAchievementID.StartFishing);
				if (IsFishingMaxLevel())
				{
					AwardAchievement(EAchievementID.FishingMaxLevel);
				}
			}
		}
	}

	public void CreateFishingTimer()
	{
		int lowerBound = 30000;
		int upperBound = 60000;
		int timeInMSUntilBite = new Random().Next(lowerBound, upperBound);
		m_FishingTimer = MainThreadTimerPool.CreateEntityTimer(OnFishBiteForPlayer, timeInMSUntilBite, this, 1);
	}

	private void DestroyFishingTimer()
	{
		if (m_FishingTimer.Instance() != null)
		{
			MainThreadTimerPool.MarkTimerForDeletion(m_FishingTimer);
		}
	}

	private void OnFishBiteForPlayer(object[] parameters)
	{
		NetworkEventSender.SendNetworkEvent_Fishing_OnBite(this, GetFishingLevel());
	}

	private static Dictionary<int, float> m_FishingXPAndPayments = new Dictionary<int, float>()
	{
		{ 0, 29.0f },
		{ 20, 38.0f },
		{ 40, 51.0f },
		{ 80, 56.0f },
		{ 160, 65.0f },
		{ 320, 74.0f },
		{ 640, 83.0f },
		{ 1280, 88.0f },
		{ 2560, 95.0f },
		{ 5120, 106.0f },
		{ 7680, 115.0f },
		{ 11520, 124.0f },
		{ 25920, 133.0f },
		{ 38880, 133.0f },
		{ 58320, 142.0f },
		{ 64152, 144.0f },
		{ 70567, 146.0f },
		{ 77623, 148.0f },
		{ 85385, 150.0f },
		{ 93923, 29.0f }
	};

	public bool IsFishingMaxLevel()
	{
		return m_FishingXPAndPayments.Count > 1 && GetFishingLevel() == m_FishingXPAndPayments.Count - 1;
	}

	public float Fishing_GetRewardForSuccessAtCurrentLevel()
	{
		int currentLevel = GetFishingLevel();
		if (currentLevel < m_FishingXPAndPayments.Count)
		{
			return m_FishingXPAndPayments.ElementAt(currentLevel).Value;
		}

		return 0.0f;
	}

	public int GetXPRequiredForFishingLevel(int level)
	{
		if (level < m_FishingXPAndPayments.Count)
		{
			return m_FishingXPAndPayments.ElementAt(level).Key;
		}

		return -1;
	}

	public int GetFishingXPRequiredForNextLevel()
	{
		int currentLevel = GetFishingLevel();
		return GetXPRequiredForFishingLevel(currentLevel + 1);
	}

	public int GetFishingLevel()
	{
		int level = 0;
		int index = 0;
		foreach (var kvPair in m_FishingXPAndPayments)
		{
			int xpAmount = kvPair.Key;
			if (FishingXP >= xpAmount)
			{
				level = index;
			}

			index++;
		}

		return level;
	}
	// END FISH DATA

	public bool IsPreviewingProperty { get; set; }

	private Dictionary<WeaponHash, List<EItemID>> m_dictWeaponAttachments = new Dictionary<WeaponHash, List<EItemID>>();
	public Dictionary<WeaponHash, List<EItemID>> WeaponAttachments
	{
		// must make a new dict + add so the diffing works. Otherwise the person writes directly to the member and the diff check for BW saving below in set won't work
		get
		{
			return new Dictionary<WeaponHash, List<EItemID>>(m_dictWeaponAttachments);
		}

		set
		{
			//if (m_dictWeaponAttachments.Count != value.Count || m_dictWeaponAttachments.Except(value).Any())
			{
				// set data
				SetData(Client, EDataNames.WEAP_MODS, Newtonsoft.Json.JsonConvert.SerializeObject(value), EDataType.Synced);
				m_dictWeaponAttachments = value;
			}
		}
	}

	private List<GangTagLayer> m_lstGangTagLayers;
	private List<GangTagLayer> m_lstGangTagLayersWIP;

	public List<GangTagLayer> GangTag
	{
		get => m_lstGangTagLayers;
		set
		{
			if (IsSpawned)
			{
				Database.LegacyFunctions.SetCharacterGangTag(ActiveCharacterDatabaseID, value);
			}

			m_lstGangTagLayers = value;
		}
	}

	public List<GangTagLayer> GangTagWIP
	{
		get => m_lstGangTagLayersWIP;
		set
		{
			if (IsSpawned)
			{
				Database.LegacyFunctions.SetCharacterWIPGangTag(ActiveCharacterDatabaseID, value);
			}

			m_lstGangTagLayersWIP = value;
		}
	}

	public Vector3 ElevatorStartPosition { get; set; }
	public float ElevatorStartRotation { get; set; }
	public uint ElevatorStartDimension { get; set; }
	public string ElevatorStartName { get; set; }
}

internal class CPlayerAnimationEntry
{
	public CPlayerAnimationEntry(int a_Flags, string strDictionaryName, string strAnimationName, bool a_bPlayerCanCancel, bool a_bBlockUntilScriptCancel, int a_DurationMS, bool a_bFreeze, float a_fZCorrection)
	{
		Flags = a_Flags;
		Dictionary = strDictionaryName;
		Name = strAnimationName;
		PlayerCanCancel = a_bPlayerCanCancel;
		BlockUntilScriptCancel = a_bBlockUntilScriptCancel;
		StartTime = DateTime.Now;
		DurationMS = a_DurationMS;
		Freeze = a_bFreeze;
		ZCorrection = a_fZCorrection;
	}

	public void Start(CPlayer a_StartingPlayer)
	{
		if (Freeze)
		{
			a_StartingPlayer.Freeze(true);
			Vector3 currentPosition = a_StartingPlayer.Client.Position;
			a_StartingPlayer.Client.Position = new Vector3(currentPosition.X, currentPosition.Y, currentPosition.Z - ZCorrection);
			StartAnimation(a_StartingPlayer);
		}
		else
		{
			StartAnimation(a_StartingPlayer);
		}
	}

	private void StartAnimation(CPlayer a_StartingPlayer)
	{
		IsPlaying = true;

		StartTime = DateTime.Now;

		a_StartingPlayer.SetData(a_StartingPlayer.Client, EDataNames.HAS_ANIM, true, EDataType.Synced);
		a_StartingPlayer.SetData(a_StartingPlayer.Client, EDataNames.ANIM_CANCELLABLE, PlayerCanCancel, EDataType.Synced);

		// Why do we serialize this you might ask? It's to condense it all into one data
		// so clientside when it changes, we dont have to worry about order (e.g. dict changes before name and we apply the wrong thing / crash the client)
		TransmitAnimation transmitAnim = new TransmitAnimation(Dictionary, Name, Flags);
		a_StartingPlayer.SetData(a_StartingPlayer.Client, EDataNames.ANIM_DATA, transmitAnim.AsJSON(), EDataType.Synced);
	}

	public void Stop(CPlayer a_StoppingPlayer)
	{
		if (Freeze)
		{
			Vector3 currentPosition = a_StoppingPlayer.Client.Position;
			a_StoppingPlayer.Client.Position = new Vector3(currentPosition.X, currentPosition.Y, currentPosition.Z + ZCorrection);
			a_StoppingPlayer.Freeze(false);
		}

		a_StoppingPlayer.SetData(a_StoppingPlayer.Client, EDataNames.HAS_ANIM, false, EDataType.Synced);
		a_StoppingPlayer.SetData(a_StoppingPlayer.Client, EDataNames.ANIM_CANCELLABLE, false, EDataType.Synced);
		a_StoppingPlayer.ClearData(a_StoppingPlayer.Client, EDataNames.ANIM_DATA);
	}

	public bool HasExpired()
	{
		if (HasStarted())
		{
			if (BlockUntilScriptCancel)
			{
				return false;
			}
			else
			{
				Int64 timeSinceStart = MainThreadTimerPool.GetMillisecondsSinceDateTime(StartTime);
				return timeSinceStart >= DurationMS;
			}
		}

		return false;
	}

	public bool HasStarted()
	{
		return IsPlaying;
	}

	public string Dictionary { get; set; }
	public string Name { get; set; }
	public int Flags { get; set; }
	public bool PlayerCanCancel { get; set; }
	public bool BlockUntilScriptCancel { get; set; }

	public Int64 DurationMS { get; set; }
	public DateTime StartTime { get; set; }
	public bool IsPlaying { get; set; }
	public bool Freeze { get; set; }
	public float ZCorrection { get; set; }

}

static class IPLHelper
{
	public static Dictionary<string, int> m_dictIPLRefCounts = new Dictionary<string, int>();
}

