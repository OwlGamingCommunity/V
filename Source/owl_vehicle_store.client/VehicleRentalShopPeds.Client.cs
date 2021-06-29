using RAGE;
using System;
using System.Collections.Generic;


public class VehicleRentalShopPeds
{
	private CGUIScooterRentalStore m_ScooterRentalStoreUI;
	private List<WeakReference<CWorldPed>> m_lstWorldPeds = new List<WeakReference<CWorldPed>>();
	private Int64 m_CurrentStoreID = -1;

	public VehicleRentalShopPeds()
	{
		NetworkEvents.RentalShop_CreatePed += OnCreatePed;
		NetworkEvents.RentalShop_DestroyPed += OnDestroyPed;

		UIEvents.ScooterRental_Close += OnCloseUI;
		UIEvents.ScooterRental_Rent += OnRentScooter;
	}

	private void OnCloseUI()
	{
		if (m_ScooterRentalStoreUI != null)
		{
			m_ScooterRentalStoreUI.SetVisible(false, false, false);
			m_ScooterRentalStoreUI = null;
		}
	}

	private void OnRentScooter()
	{
		OnCloseUI();
		NetworkEventSender.SendNetworkEvent_RentalShop_RentScooter(m_CurrentStoreID);
	}

	private void OnCreatePed(Int64 storeID, Vector3 vecPedPos, float fPedRot, uint pedDimension)
	{
		uint skinID = 3882958867;
		string strStoreTypeName = "Vehicle Rental Shop";

		WeakReference<CWorldPed> refWorldPed = WorldPedManager.CreatePed(EWorldPedType.StoreCashier, skinID, vecPedPos, fPedRot, pedDimension);
		refWorldPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, Helpers.FormatString("Talk to {0}", strStoreTypeName), null, () => { OnInteractWithStore(storeID); }, false, false, 3.0f, null, true);
		m_lstWorldPeds.Add(refWorldPed);
	}

	private void OnDestroyPed(Vector3 vecPos, float fRot, uint dimension)
	{
		foreach (var pedRef in m_lstWorldPeds)
		{
			CWorldPed ped = pedRef.Instance();
			if (ped != null)
			{
				if (ped.Position == vecPos && ped.RotZ == fRot && ped.Dimension == dimension)
				{
					WorldPedManager.DestroyPed(ped);
				}
			}
		}
	}

	private void OnInteractWithStore(Int64 storeID)
	{
		m_CurrentStoreID = storeID;
		m_ScooterRentalStoreUI = new CGUIScooterRentalStore(() => { });
		m_ScooterRentalStoreUI.SetVisible(true, true, false);
	}
}
