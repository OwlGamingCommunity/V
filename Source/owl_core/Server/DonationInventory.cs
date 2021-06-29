using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityDatabaseID = System.Int64;

public class CDonationInventory
{
	public CDonationInventory(CPlayer a_OwningPlayer)
	{
		m_OwningPlayer.SetTarget(a_OwningPlayer);

		Reset();
	}

	public void CopyInventory(List<DonationInventoryItem> arrDonationInventory)
	{
		if (arrDonationInventory != null)
		{
			m_arrDonationInventory = arrDonationInventory;
		}
	}

	public void Reset()
	{
		m_arrDonationInventory.Clear();
	}

	public string GetAsJSON()
	{
		return JsonConvert.SerializeObject(m_arrDonationInventory);
	}

	public DonationInventoryItem GetFromDBID(UInt32 dbid)
	{
		foreach (var donationItem in m_arrDonationInventory)
		{
			if (donationItem.ID == dbid)
			{
				return donationItem;
			}
		}

		return null;
	}

	public bool HasDonationOfID(UInt32 id)
	{
		foreach (var donationItem in m_arrDonationInventory)
		{
			if (donationItem.DonationID == id)
			{
				return true;
			}
		}

		return false;
	}

	public bool HasActiveDonationOfID(UInt32 id)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return false;
		}

		foreach (var donationItem in m_arrDonationInventory)
		{
			if (donationItem.DonationID == id && donationItem.IsActive())
			{
				foreach (var purchasable in Donations.g_lstPurchasables)
				{
					if (purchasable.ID == donationItem.DonationID)
					{
						// It must be applied to this character OR account-wide
						if (donationItem.Character == pPlayer.ActiveCharacterDatabaseID || purchasable.m_Type == EDonationType.Account)
						{
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	public DonationInventoryItem GetCurrentInactivityProtection(Int64 entityID, EDonationInactivityPurchasables purchasable)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return null;
		}

		foreach (var donationItem in m_arrDonationInventory)
		{
			if (donationItem.DonationID == (int)purchasable)
			{
				if (donationItem.VehicleID == entityID || donationItem.PropertyID == entityID)
				{
					return donationItem;
				}
			}
		}

		return null;
	}

	public bool HasActiveDonationOfEffectType(EDonationEffect donationEffect)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return false;
		}

		foreach (var donationItem in m_arrDonationInventory)
		{
			if (donationItem.IsActive())
			{
				foreach (var purchasable in Donations.g_lstPurchasables)
				{
					if (purchasable.ID == donationItem.DonationID)
					{
						if (purchasable.DonationEffect == donationEffect)
						{
							// It must be applied to this character OR account-wide
							if (donationItem.Character == pPlayer.ActiveCharacterDatabaseID || purchasable.m_Type == EDonationType.Account)
							{
								return true;
							}
						}
					}
				}
			}
		}

		return false;
	}

	public async void AttemptToFixLosSantosPropertyTokens(EntityDatabaseID characterID)
	{
		foreach (var donationItem in m_arrDonationInventory)
		{
			if (donationItem.IsActive())
			{
				foreach (var purchasable in Donations.g_lstPurchasables)
				{
					if (purchasable.ID == donationItem.DonationID)
					{
						if (purchasable.DonationEffect == EDonationEffect.PropertyToken)
						{
							// It must be applied to this character OR account-wide
							if (donationItem.Character == 0 && purchasable.m_Type == EDonationType.Character && donationItem.TimeExpire == -1)
							{
								Int64 time_activated = Helpers.GetUnixTimestamp();
								await Database.LegacyFunctions.ActivateDonationItem(donationItem.ID, characterID, time_activated, -2).ConfigureAwait(true);

								donationItem.Character = characterID;
								donationItem.TimeActivated = time_activated;
								donationItem.TimeExpire = -2;
							}
						}
					}
				}
			}
		}
	}

	public void RemoveTokenOfTypeForActiveCharacter(EDonationEffect donationEffect)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		foreach (var donationItem in m_arrDonationInventory)
		{
			if (donationItem.IsActive() && donationItem.Character == pPlayer.ActiveCharacterDatabaseID)
			{
				foreach (var purchasable in Donations.g_lstPurchasables)
				{
					if (purchasable.ID == donationItem.DonationID)
					{
						if (purchasable.DonationEffect == donationEffect)
						{
							ConsumeDonationItem(donationItem);
							return;
						}
					}
				}
			}
		}
	}

	public void ConsumeDonationItem(DonationInventoryItem donationItem)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		m_arrDonationInventory.Remove(donationItem);
		Database.LegacyFunctions.RemoveDonationInventoryItem(pPlayer.AccountID, pPlayer.ActiveCharacterDatabaseID, donationItem.ID).ConfigureAwait(true);
	}

	public async Task<DonationInventoryItem> OnPurchaseScripted(DonationPurchasable purchasable, bool bActivate)
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return null;
		}

		// Give in the database
		UInt32 dbid = await Database.LegacyFunctions.GivePlayerDonationItem(pPlayer.AccountID, purchasable).ConfigureAwait(true);

		// Create instance and add + return
		DonationInventoryItem newItem = new DonationInventoryItem(dbid, -1, -1, -1, purchasable.ID, -1, -1);
		m_arrDonationInventory.Add(newItem);

		if (bActivate)
		{
			await OnActivate(dbid, purchasable, newItem, true).ConfigureAwait(true);
		}

		return newItem;
	}

	public async Task<DonationInventoryItem> OnPurchase(DonationPurchasable purchasable)
	{
		// NOTE: Caller already verified that we actually own this
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return null;
		}

		// Subtract money
		pPlayer.SubtractDonatorCurrency(purchasable.Cost);

		// Give in the database
		UInt32 dbid = await Database.LegacyFunctions.GivePlayerDonationItem(pPlayer.AccountID, purchasable).ConfigureAwait(true);

		// Create instance and add + return
		DonationInventoryItem newItem = new DonationInventoryItem(dbid, -1, -1, -1, purchasable.ID, -1, -1);
		m_arrDonationInventory.Add(newItem);

		pPlayer.SendNotification("Donation Perk Purchased", ENotificationIcon.InfoSign, "You have purchased the '{0}' donator perk.", purchasable.Title);
		pPlayer.SendBasicDonatorInfo();
		return newItem;
	}

	public async Task OnActivate(UInt32 dbid, DonationPurchasable purchasable, DonationInventoryItem item, bool bWasSilent = false)
	{
		// NOTE: Caller already verified that we actually own this
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// Custom handling, typically for one time consumable effects
		bool bWasConsumed = false;

		// BIKE LICENSE
		if (purchasable.DonationEffect == EDonationEffect.InstantDriversLicenseBike)
		{
			CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.DRIVERS_PERMIT_BIKE, pPlayer.ActiveCharacterDatabaseID);
			if (pPlayer.HasDrivingLicense(EDrivingTestType.Bike))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You already have a bike license on this character.");
			}
			else if (!pPlayer.Inventory.CanGiveItem(itemInstDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You cannot carry this item:<br>{0}", strUserFriendlyMessage);
			}
			else
			{
				pPlayer.Inventory.AddItemToNextFreeSuitableSlot(CItemInstanceDef.FromBasicValueNoDBID(EItemID.DRIVERS_PERMIT_BIKE, pPlayer.ActiveCharacterDatabaseID), EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						pPlayer.SendNotification("Donation Perk Activated", ENotificationIcon.InfoSign, "You have received a bike license on this character.");

						ConsumeDonationItem(item);
						bWasConsumed = true;
					}
					else
					{
						pPlayer.SendNotification("Donation Perk Failed", ENotificationIcon.ExclamationSign, "Failed to activate donation perk. Please try again later.");
					}
				});
			}
		}
		else if (purchasable.DonationEffect == EDonationEffect.InstantDriversLicenseCar) // CAR LICENSE
		{
			CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.DRIVERS_PERMIT_CAR, pPlayer.ActiveCharacterDatabaseID);
			if (pPlayer.HasDrivingLicense(EDrivingTestType.Car))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You already have a drivers license on this character.");
			}
			else if (!pPlayer.Inventory.CanGiveItem(itemInstDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You cannot carry this item:<br>{0}", strUserFriendlyMessage);
			}
			else
			{
				pPlayer.Inventory.AddItemToNextFreeSuitableSlot(CItemInstanceDef.FromBasicValueNoDBID(EItemID.DRIVERS_PERMIT_CAR, pPlayer.ActiveCharacterDatabaseID), EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						pPlayer.SendNotification("Donation Perk Activated", ENotificationIcon.ExclamationSign, "You have received a drivers license on this character.");
						ConsumeDonationItem(item);
						bWasConsumed = true;
					}
					else
					{
						pPlayer.SendNotification("Donation Perk Failed", ENotificationIcon.ExclamationSign, "Failed to activate donation perk. Please try again later.");
					}
				});
			}
		}
		else if (purchasable.DonationEffect == EDonationEffect.InstantDriversLicenseTruck) // TRUCK LICENSE
		{
			CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.DRIVERS_PERMIT_LARGE, pPlayer.ActiveCharacterDatabaseID);
			if (pPlayer.HasDrivingLicense(EDrivingTestType.Truck))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You already have a large vehicle license on this character.");
			}
			else if (!pPlayer.Inventory.CanGiveItem(itemInstDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You cannot carry this item:<br>{0}", strUserFriendlyMessage);
			}
			else
			{
				pPlayer.Inventory.AddItemToNextFreeSuitableSlot(CItemInstanceDef.FromBasicValueNoDBID(EItemID.DRIVERS_PERMIT_LARGE, pPlayer.ActiveCharacterDatabaseID), EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						pPlayer.SendNotification("Donation Perk Activated", ENotificationIcon.ExclamationSign, "You have received a large vehicle license on this character.");
						ConsumeDonationItem(item);
						bWasConsumed = true;
					}
					else
					{
						pPlayer.SendNotification("Donation Perk Failed", ENotificationIcon.ExclamationSign, "Failed to activate donation perk. Please try again later.");
					}
				});
			}

		}
		else if (purchasable.DonationEffect == EDonationEffect.InstantFirearmsLicenseSmall_DISABLED) // FIREARM
		{
			CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER1, pPlayer.ActiveCharacterDatabaseID);
			if (pPlayer.HasHandgunFirearmLicense())
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You already have a Tier 1 firearm license on this character.");
			}
			else if (!pPlayer.Inventory.CanGiveItem(itemInstDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You cannot carry this item:<br>{0}", strUserFriendlyMessage);
			}
			else
			{
				pPlayer.Inventory.AddItemToNextFreeSuitableSlot(CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER1, pPlayer.ActiveCharacterDatabaseID), EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						pPlayer.SendNotification("Donation Perk Activated", ENotificationIcon.ExclamationSign, "You have received a Tier 1 firearm license on this character.");
						ConsumeDonationItem(item);
						bWasConsumed = true;
					}
					else
					{
						pPlayer.SendNotification("Donation Perk Failed", ENotificationIcon.ExclamationSign, "Failed to activate donation perk. Please try again later.");
					}
				});
			}
		}
		else if (purchasable.DonationEffect == EDonationEffect.InstantFirearmsLicenseLarge_DISABLED) // LARGE FIREARM
		{
			CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER2, pPlayer.ActiveCharacterDatabaseID);
			if (pPlayer.HasLargeFirearmLicense())
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You already have a Tier 2 firearm license on this character.");
			}
			else if (!pPlayer.Inventory.CanGiveItem(itemInstDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
			{
				pPlayer.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You cannot carry this item:<br>{0}", strUserFriendlyMessage);
			}
			else
			{
				pPlayer.Inventory.AddItemToNextFreeSuitableSlot(CItemInstanceDef.FromBasicValueNoDBID(EItemID.FIREARMS_LICENSE_TIER2, pPlayer.ActiveCharacterDatabaseID), EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						pPlayer.SendNotification("Donation Perk Activated", ENotificationIcon.ExclamationSign, "You have received a Tier 2 firearm license on this character.");
						ConsumeDonationItem(item);
						bWasConsumed = true;
					}
					else
					{
						pPlayer.SendNotification("Donation Perk Failed", ENotificationIcon.ExclamationSign, "Failed to activate donation perk. Please try again later.");
					}
				});
			}
		}
		else if (purchasable.DonationEffect >= EDonationEffect.Pet_Boar && purchasable.DonationEffect <= EDonationEffect.Pet_Panther)
		{
			EPetType petToGrant = EPetType.None;

			switch (purchasable.DonationEffect)
			{
				case EDonationEffect.Pet_Boar:
					{
						petToGrant = EPetType.Boar;
						break;
					}
				case EDonationEffect.Pet_Cat:
					{
						petToGrant = EPetType.Cat;
						break;
					}
				case EDonationEffect.Pet_Chickenhawk:
					{
						petToGrant = EPetType.Chickenhawk;
						break;
					}
				case EDonationEffect.Pet_Chop:
					{
						petToGrant = EPetType.Chop;
						break;
					}
				case EDonationEffect.Pet_Cormorant:
					{
						petToGrant = EPetType.Cormorant;
						break;
					}
				case EDonationEffect.Pet_Coyote:
					{
						petToGrant = EPetType.Coyote;
						break;
					}
				case EDonationEffect.Pet_Crow:
					{
						petToGrant = EPetType.Crow;
						break;
					}
				case EDonationEffect.Pet_Hen:
					{
						petToGrant = EPetType.Hen;
						break;
					}
				case EDonationEffect.Pet_Husky:
					{
						petToGrant = EPetType.Husky;
						break;
					}
				case EDonationEffect.Pet_Pig:
					{
						petToGrant = EPetType.Pig;
						break;
					}
				case EDonationEffect.Pet_Pigeon:
					{
						petToGrant = EPetType.Pigeon;
						break;
					}
				case EDonationEffect.Pet_Poodle:
					{
						petToGrant = EPetType.Poodle;
						break;
					}
				case EDonationEffect.Pet_Pug:
					{
						petToGrant = EPetType.Pug;
						break;
					}
				case EDonationEffect.Pet_Rabbit:
					{
						petToGrant = EPetType.Rabbit;
						break;
					}
				case EDonationEffect.Pet_Rat:
					{
						petToGrant = EPetType.Rat;
						break;
					}
				case EDonationEffect.Pet_Retriever:
					{
						petToGrant = EPetType.Retriever;
						break;
					}
				case EDonationEffect.Pet_Rottweiler:
					{
						petToGrant = EPetType.Rottweiler;
						break;
					}
				case EDonationEffect.Pet_Seagull:
					{
						petToGrant = EPetType.Seagull;
						break;
					}
				case EDonationEffect.Pet_Shepherd:
					{
						petToGrant = EPetType.Shepherd;
						break;
					}
				case EDonationEffect.Pet_Westy:
					{
						petToGrant = EPetType.Westy;
						break;
					}
				case EDonationEffect.Pet_Panther:
					{
						petToGrant = EPetType.Panther;
						break;
					}
			}

			if (petToGrant != EPetType.None)
			{
				CItemValuePet petValue = new CItemValuePet(petToGrant, String.Empty, false);
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.PET, petValue);

				pPlayer.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						pPlayer.SendNotification("Donation Perk Activated", ENotificationIcon.ExclamationSign, Helpers.FormatString("You have received a {0} pet on this character.", petToGrant.ToString()));
						ConsumeDonationItem(item);

						bWasConsumed = true;
					}
					else
					{
						pPlayer.SendNotification("Donation Perk Failed", ENotificationIcon.ExclamationSign, "Failed to activate donation perk. Please try again later.");
					}
				});
			}
			else
			{
				pPlayer.SendNotification("Donation Perk Failed", ENotificationIcon.ExclamationSign, "Unknown Pet Type, please report on bugtracker.");
			}

		}

		if (!bWasConsumed)
		{
			// Update state in DB
			Int64 character_id = purchasable.m_Type == EDonationType.Account ? -1 : pPlayer.ActiveCharacterDatabaseID;
			Int64 time_activated = Helpers.GetUnixTimestamp();
			Int64 time_expire = purchasable.Duration > 0 ? (Helpers.GetUnixTimestamp() + (purchasable.Duration * 24 * 60 * 60)) : -1;

			await Database.LegacyFunctions.ActivateDonationItem(dbid, character_id, time_activated, time_expire).ConfigureAwait(true);
			item.Character = character_id;
			item.TimeActivated = time_activated;
			item.TimeExpire = time_expire;

			if (!bWasSilent)
			{
				pPlayer.SendNotification("Donation Perk Activated", ENotificationIcon.ExclamationSign, "You have activated the '{0}' perk for your {1}.", purchasable.Title, purchasable.m_Type == EDonationType.Account ? "Account" : "Current Character");
			}
		}

		if (!bWasSilent)
		{
			pPlayer.SendBasicDonatorInfo();
		}
	}

	public async void Update()
	{
		CPlayer pPlayer = m_OwningPlayer.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// Check for expired perks
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();
		foreach (DonationInventoryItem item in m_arrDonationInventory)
		{
			if (item.TimeExpire >= 0 && item.TimeExpire < unixTimestamp)
			{
				await Database.LegacyFunctions.RemoveDonationInventoryItem(pPlayer.AccountID, pPlayer.ActiveCharacterDatabaseID, item.ID).ConfigureAwait(true);
			}
		}
	}

	public List<DonationInventoryItem> Get()
	{
		return m_arrDonationInventory;
	}

	public void Add(DonationInventoryItem newItem)
	{
		m_arrDonationInventory.Add(newItem);
	}

	private List<DonationInventoryItem> m_arrDonationInventory = new List<DonationInventoryItem>();
	private WeakReference<CPlayer> m_OwningPlayer = new WeakReference<CPlayer>(null);
}