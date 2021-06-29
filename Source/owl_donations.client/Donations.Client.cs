using System;
using System.Collections.Generic;

public class Donations
{
	private CGUIDonationsPerks m_DonationsPerkUI = new CGUIDonationsPerks(OnUILoaded);

	// TODO_GITHUB: You should replace the below with your own website
	private const string g_strPurchaseGCURL = "https://website.com/account/purchase/";
	private List<DonationPurchasable> m_lstPurchasables = new List<DonationPurchasable>();

	private Int64 m_entityID = -1;
	private EDonationInactivityEntityType m_CurrentType = EDonationInactivityEntityType.Property;
	private int m_InactivityLength = 7;

	public Donations()
	{
		// TODO_CSHARP: How do we want to handle allowing to show only when keybinds enabled but hide anytime? or do we want to just have every UI have an exit button? probably.
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleDonations, ToggleDonationsUI);

		NetworkEvents.ChangeCharacterApproved += HideDonationsUI;

		NetworkEvents.GotDonationPurchasables += OnGotDonationPurchasables;
		NetworkEvents.GotBasicDonatorInfo += OnGotBasicDonatorInfo;

		NetworkEvents.Donation_RespondInactivityEntities += OnGotDonationInactivityEntities;
	}

	~Donations()
	{

	}

	private static void OnUILoaded()
	{

	}

	private void OnGotDonationInactivityEntities(List<DonationEntityListEntry> lstEntities)
	{
		m_DonationsPerkUI.ClearInactivityEntities();

		if (lstEntities.Count == 0)
		{
			m_DonationsPerkUI.SetInactivityEntitiesEmpty();
		}
		else
		{
			foreach (DonationEntityListEntry entry in lstEntities)
			{
				m_DonationsPerkUI.AddInactivityEntity(entry);
			}
		}
	}

	public void OnChangeInactivityEntity(Int64 entityID)
	{
		m_entityID = entityID;
	}

	public void OnChangeInactivityType(EDonationInactivityEntityType type)
	{
		m_CurrentType = type;
		NetworkEventSender.SendNetworkEvent_Donation_RequestInactivityEntities(type);
	}

	public void OnChangeInactivityLength(int inactivityLength)
	{
		m_InactivityLength = inactivityLength;
		m_DonationsPerkUI.SetInactivityGCCost((m_InactivityLength / DonationConstants.InactivityProtectionIncrement) * DonationConstants.GCCostPer7DaysOfInactivityProtection);
	}

	private void OnGotBasicDonatorInfo(int GCBalance, List<DonationInventoryItemTransmit> lstDonationInventory)
	{
		m_DonationsPerkUI.SetVisible(true, true, false);
		m_DonationsPerkUI.Reset();
		OnChangeInactivityLength(m_InactivityLength);

		foreach (DonationPurchasable purchasable in m_lstPurchasables)
		{
			m_DonationsPerkUI.AddDonationPurchasable(purchasable.ID, purchasable.Title, purchasable.Description, purchasable.m_Type, purchasable.Cost, purchasable.Duration, purchasable.Unique, purchasable.Active);
			// TODO_DONATIONS: Actives we still have to add to the UI for data to work, but we don't show them for purchase
		}

		m_DonationsPerkUI.SetGCBalance(GCBalance);

		foreach (DonationInventoryItemTransmit invItem in lstDonationInventory)
		{
			if (invItem.InventoryItemSource.TimeActivated == -1) // not used
			{
				m_DonationsPerkUI.AddPerkInventoryItem(invItem.InventoryItemSource.ID, invItem.InventoryItemSource.DonationID);
			}
			else
			{
				m_DonationsPerkUI.AddActivePerk(invItem.InventoryItemSource.DonationID, invItem.InventoryItemSource.TimeActivated, invItem.InventoryItemSource.TimeExpire, invItem.AppliedTo);
			}
		}

		m_DonationsPerkUI.CommitData();
	}

	private void ToggleDonationsUI(EControlActionType actionType)
	{
		// ignore if alt is pressed, for users of alt+f4
		if (!KeyBinds.IsKeyDown(0x12))
		{
			if (m_DonationsPerkUI.IsVisible()) // We can hide always, but can only show when eligible
			{
				HideDonationsUI();
			}
			else if (KeyBinds.CanProcessKeybinds())
			{
				GotoDonationsUI();
			}
		}
	}

	public void HideDonationsUI()
	{
		m_DonationsPerkUI.SetVisible(false, false, false);
	}

	public void OnPurchaseGC()
	{
		HideDonationsUI();
		HUD.ShowFullScreenBrowser(g_strPurchaseGCURL);
	}

	// Called from HUD
	public void GotoDonationsUI()
	{
		NetworkEventSender.SendNetworkEvent_GetBasicDonatorInfo();
	}

	public void PurchaseDonationPerk(UInt32 id)
	{
		NetworkEventSender.SendNetworkEvent_PurchaseDonationPerk(id);
	}

	public void ConsumeDonationPerk(UInt32 id)
	{
		NetworkEventSender.SendNetworkEvent_ConsumeDonationPerk(id);
	}

	private void OnGotDonationPurchasables(List<DonationPurchasable> lstPurchasables)
	{
		m_lstPurchasables = lstPurchasables;
	}

	public void PurchaseInactivityProtection()
	{
		if (m_entityID != -1)
		{
			NetworkEventSender.SendNetworkEvent_PurchaseInactivityProtection(m_entityID, m_CurrentType, m_InactivityLength);
		}
		else
		{
			NotificationManager.ShowNotification("Inactivity Protection", "Please select an entity to apply the protection to!", ENotificationIcon.ExclamationSign);
		}
	}
}