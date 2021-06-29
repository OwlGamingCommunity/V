using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class DonationSystem
{

	public DonationSystem()
	{
		Init();
	}

	private async void Init()
	{
		Donations.g_lstPurchasables = await Database.LegacyFunctions.LoadDonationPurchasables().ConfigureAwait(true);
		NAPI.Util.ConsoleOutput("[DONATIONS] Loaded {0} Donation Purchasables!", Donations.g_lstPurchasables.Count);

		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
		NetworkEvents.GetBasicDonatorInfo += OnGetBasicDonatorInfo;
		NetworkEvents.PurchaseDonationPerk += OnPurchaseDonationPerk;
		NetworkEvents.ConsumeDonationPerk += OnConsumeDonationPerk;

		NetworkEvents.Donation_RequestInactivityEntities += OnChangeInactivityType;

		NetworkEvents.PurchaseInactivityProtection += OnPurchaseInactivityProtection;
	}

	private async void OnPurchaseInactivityProtection(CPlayer player, Int64 TargetEntityID, EDonationInactivityEntityType TargetEntityType, int InactivityLength)
	{
		if (TargetEntityID != -1)
		{
			// security check
			if (InactivityLength >= DonationConstants.InactivityProtectionIncrement && InactivityLength <= DonationConstants.InactivityProtectionMaxDays)
			{
				int GCCost = (InactivityLength / DonationConstants.InactivityProtectionIncrement) * DonationConstants.GCCostPer7DaysOfInactivityProtection;
				int playerGC = await player.GetDonatorCurrency().ConfigureAwait(true);

				if (GCCost <= playerGC)
				{
					// TODO_DONATIONS: Merge this with player.givedonation
					player.SubtractDonatorCurrency(GCCost);
					EDonationInactivityPurchasables purchasableID = TargetEntityType == EDonationInactivityEntityType.Property ? EDonationInactivityPurchasables.PropertyPurchasable : EDonationInactivityPurchasables.VehiclePurchasable;

					long vehicleID = TargetEntityType == EDonationInactivityEntityType.Vehicle ? TargetEntityID : -1;
					long propertyID = TargetEntityType == EDonationInactivityEntityType.Property ? TargetEntityID : -1;

					Int64 time_activated = Helpers.GetUnixTimestamp();
					Int64 time_expire = (Helpers.GetUnixTimestamp() + (InactivityLength * 24 * 60 * 60));

					// Do we already have inactivity protection? If so lets work out how to extend it
					DonationInventoryItem donationInventoryItem = player.DonationInventory.GetCurrentInactivityProtection(TargetEntityID, purchasableID);
					if (donationInventoryItem != null)
					{
						Int64 currentInactivityLengthRemaining = donationInventoryItem.TimeExpire - time_activated;
						time_expire += currentInactivityLengthRemaining;

						await Database.LegacyFunctions.UpdateInactivityDonationPerk(player.AccountID, player.ActiveCharacterDatabaseID, time_expire, purchasableID, TargetEntityID, TargetEntityType).ConfigureAwait(true);

						donationInventoryItem.TimeExpire = time_expire;
					}
					else
					{
						uint dbid = await Database.LegacyFunctions.GiveInactivityDonationPerk(player.AccountID, player.ActiveCharacterDatabaseID, time_activated, time_expire, purchasableID, TargetEntityID, TargetEntityType).ConfigureAwait(true);

						// Create instance and add
						DonationInventoryItem newItem = new DonationInventoryItem(dbid, player.ActiveCharacterDatabaseID, time_activated, time_expire, (uint)purchasableID, vehicleID, propertyID);
						player.DonationInventory.Add(newItem);
					}


					player.SendNotification("Inactivity Protection", ENotificationIcon.InfoSign, "You have purchased the '{0} Inactivity Protection' donator perk for {1} day(s).", TargetEntityType.ToString(), InactivityLength);
					player.SendBasicDonatorInfo();
				}
				else
				{
					player.SendNotification("Inactivity Protection", ENotificationIcon.ExclamationSign, "You cannot afford inactivity protection for {0} days (Needs {1} GC, you only have {2} GC).", InactivityLength, GCCost, playerGC);
				}
			}
		}
		else
		{
			player.SendNotification("Inactivity Protection", ENotificationIcon.ExclamationSign, "Please select an entity to apply the protection to!");
		}
	}

	private void OnChangeInactivityType(CPlayer player, EDonationInactivityEntityType type)
	{
		List<DonationEntityListEntry> lstEntities = new List<DonationEntityListEntry>();

		if (type == EDonationInactivityEntityType.Property)
		{
			List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByPlayer(player);
			foreach (CPropertyInstance propInst in lstProperties)
			{
				if (propInst.CanExpire())
				{
					lstEntities.Add(new DonationEntityListEntry(propInst.Model.Id, propInst.Model.Name));
				}
			}
		}
		else if (type == EDonationInactivityEntityType.Vehicle)
		{
			List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromPlayer(player);
			foreach (CVehicle vehicle in lstVehicles)
			{
				lstEntities.Add(new DonationEntityListEntry(vehicle.m_DatabaseID, Helpers.FormatString("{0} ({1})", vehicle.GetFullDisplayName(), vehicle.GTAInstance.NumberPlate)));
			}
		}

		NetworkEventSender.SendNetworkEvent_Donation_RespondInactivityEntities(player, lstEntities);
	}

	public void OnPlayerConnected(CPlayer player)
	{
		NetworkEventSender.SendNetworkEvent_GotDonationPurchasables(player, Donations.g_lstPurchasables);
	}

	public static void OnGetBasicDonatorInfo(CPlayer player)
	{
		player.SendBasicDonatorInfo();
	}

	public async void OnPurchaseDonationPerk(CPlayer player, UInt32 dbid)
	{
		// TODO: This could be more efficient with access @ index rather than foreach, but its probably not that common enough to care
		foreach (var purchasable in Donations.g_lstPurchasables)
		{
			if (purchasable.ID == dbid)
			{
				// SECURITY: Is the puchasable active?
				if (purchasable.Active)
				{
					// do we have enough money?
					int donatorCurrency = await player.GetDonatorCurrency().ConfigureAwait(true);
					if (donatorCurrency >= purchasable.Cost)
					{
						// Do we have space for this item? this only applies to account donations, character unique donations must be checked at activation
						if (purchasable.Unique && purchasable.m_Type == EDonationType.Account && player.DonationInventory.HasDonationOfID(purchasable.ID))
						{
							player.SendNotification("Donation Perk Purchase Failed", ENotificationIcon.ExclamationSign, "You can only have one of those perks per account.");
						}
						else
						{
							await player.DonationInventory.OnPurchase(purchasable).ConfigureAwait(true);
						}
					}
					else
					{
						player.SendNotification("Donation Perk Purchase Failed", ENotificationIcon.ExclamationSign, "You could not purchase that perk as you only have {0} GC and you require {1} GC.", donatorCurrency, purchasable.Cost);
					}
				}
				else
				{
					player.SendNotification("Donation Perk Purchase Failed", ENotificationIcon.ExclamationSign, "Unknown error occured.");
				}
			}
		}
	}

	public async void OnConsumeDonationPerk(CPlayer player, UInt32 dbid)
	{
		// TODO: This could be more efficient with access @ index rather than foreach, but its probably not that common enough to care
		var donationItem = player.DonationInventory.GetFromDBID(dbid);
		if (donationItem != null)
		{
			foreach (var purchasable in Donations.g_lstPurchasables)
			{
				if (purchasable.ID == donationItem.DonationID)
				{
					// Is this perk unique and already active on this character?
					if (purchasable.Unique && purchasable.m_Type == EDonationType.Character && player.DonationInventory.HasActiveDonationOfID(purchasable.ID))
					{
						player.SendNotification("Donation Perk Activation Failed", ENotificationIcon.ExclamationSign, "You can only have one of those perks active per character.");
					}
					else
					{
						await player.DonationInventory.OnActivate(dbid, purchasable, donationItem).ConfigureAwait(true);
					}
				}
			}
		}
	}
}