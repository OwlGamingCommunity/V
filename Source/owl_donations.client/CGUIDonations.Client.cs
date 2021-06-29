using System;

internal class CGUIDonationsPerks : CEFCore
{
	public CGUIDonationsPerks(OnGUILoadedDelegate callbackOnLoad) : base("owl_donations.client/donationperks.html", EGUIID.Donations, callbackOnLoad)
	{
		UIEvents.CloseDonations += () => { DonationSystem.GetDonations()?.HideDonationsUI(); };
		UIEvents.PurchaseGC += () => { DonationSystem.GetDonations()?.OnPurchaseGC(); };
		UIEvents.GotoDonations += () => { DonationSystem.GetDonations()?.GotoDonationsUI(); }; // Called from HUD
		UIEvents.ConsumeDonationPerk += (UInt32 id) => { DonationSystem.GetDonations()?.ConsumeDonationPerk(id); };
		UIEvents.PurchaseDonationPerk += (UInt32 id) => { DonationSystem.GetDonations()?.PurchaseDonationPerk(id); };
		UIEvents.PurchaseInactivityProtection += () => { DonationSystem.GetDonations()?.PurchaseInactivityProtection(); };

		UIEvents.Donation_ChangeInactivityEntity += (Int64 entityID) => { DonationSystem.GetDonations()?.OnChangeInactivityEntity(entityID); };
		UIEvents.Donation_ChangeInactivityType += (EDonationInactivityEntityType type) => { DonationSystem.GetDonations()?.OnChangeInactivityType(type); };
		UIEvents.Donation_ChangeInactivityLength += (int inactivityLength) => { DonationSystem.GetDonations()?.OnChangeInactivityLength(inactivityLength); };
	}

	public override void OnLoad()
	{

	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void AddDonationPurchasable(UInt32 id, string strTitle, string strDescription, EDonationType donationType, int cost, int duration, bool bUnique, bool bActive)
	{
		Execute("AddDonationPurchasable", id, strTitle, strDescription, donationType, cost, duration, bUnique, bActive);
	}

	public void SetGCBalance(int balance)
	{
		Execute("SetGCBalance", balance);
	}

	public void AddPerkInventoryItem(uint ID, uint donationID)
	{
		Execute("AddPerkInventoryItem", ID, donationID);
	}

	public void AddActivePerk(uint donationID, Int64 timeActivated, Int64 timeExpire, string strAppliedTo)
	{
		Execute("AddActivePerk", donationID, timeActivated, timeExpire, strAppliedTo);
	}

	public void CommitData()
	{
		Execute("CommitData");
	}

	public void ClearInactivityEntities()
	{
		Execute("ClearInactivityEntities");
	}

	public void AddInactivityEntity(DonationEntityListEntry entry)
	{
		Execute("AddInactivityEntity", entry.ID, entry.DisplayName);
	}

	public void SetInactivityEntitiesEmpty()
	{
		Execute("SetInactivityEntitiesEmpty");
	}

	public void SetInactivityGCCost(int cost)
	{
		Execute("SetInactivityGCCost", cost);
	}
}