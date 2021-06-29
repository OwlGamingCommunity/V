using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class TattooArtist
{
	public TattooArtist()
	{
		NetworkEvents.EnterTattooArtist += OnEnterTattooArtist;

		NetworkEvents.TattooArtist_CalculatePrice += CalculatePrice;
		NetworkEvents.TattooArtist_Checkout += OnCheckout;
	}

	public void OnEnterTattooArtist(CPlayer player)
	{
		if (player.CharacterType == ECharacterType.Custom)
		{
			player.GotoPlayerSpecificDimension();
			player.CacheHealthAndArmor();

			// Force the normal skin, we don't let them modify job or duty skins here.
			player.ApplySkinFromInventory(true, true);

			NetworkEventSender.SendNetworkEvent_EnterTattooArtist_Response(player, player.GetTattoos());
		}
		else
		{
			// Offer character type change
			NetworkEventSender.SendNetworkEvent_EnterPlasticSurgeon_OfferCharacterUpgrade(player);
		}
	}

	private void CalculateChangesCost(CPlayer player, List<int> lstNewTattoos, out float fPrice, out bool bHasToken, out uint numAdded, out uint numRemoved)
	{
		const float fPricePerChange = 25.0f;
		fPrice = 0.0f;
		bHasToken = player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Tattoo);
		numAdded = 0;
		numRemoved = 0;

		List<int> lstCurrentTattoos = player.GetTattoos();

		// see what we added
		foreach (int newTattooID in lstNewTattoos)
		{
			// Not in current? we added it!
			if (!lstCurrentTattoos.Contains(newTattooID))
			{
				++numAdded;
			}
		}

		// see what we removed
		foreach (int oldTattooID in lstCurrentTattoos)
		{
			// Not in new? we removed it!
			if (!lstNewTattoos.Contains(oldTattooID))
			{
				++numRemoved;
			}
		}

		fPrice = (numRemoved + numAdded) * fPricePerChange;
	}

	private void CalculatePrice(CPlayer player, List<int> lstNewTattoos)
	{
		CalculateChangesCost(player, lstNewTattoos, out float fPrice, out bool bHasToken, out uint numAdded, out uint numRemoved);
		NetworkEventSender.SendNetworkEvent_TattooArtist_GotPrice(player, fPrice, bHasToken, numAdded, numRemoved);
	}

	private void OnCheckout(CPlayer player, EntityDatabaseID storeID, List<int> lstNewTattoos)
	{
		CalculateChangesCost(player, lstNewTattoos, out float fPrice, out bool bHasToken, out uint numAdded, out uint numRemoved);

		bool bWasPurchased = false;

		if (numAdded > 0 || numRemoved > 0)
		{
			// Do we have a token?
			if (bHasToken)
			{
				// consume token
				player.DonationInventory.RemoveTokenOfTypeForActiveCharacter(EDonationEffect.Free_Visit_Tattoo);
				bWasPurchased = true;
			}
			else if (player.SubtractMoney(fPrice, PlayerMoneyModificationReason.TattooArtistCheckout))
			{
				bWasPurchased = true;
			}
			else
			{
				bWasPurchased = false;
				player.SendNotification("Tattoo Artist", ENotificationIcon.ExclamationSign, "You do not have enough money to purchase this tattoo art work.");
			}

			if (bWasPurchased)
			{
				StoreSystem.HandleStoreTransactionOwnerShare(storeID, fPrice);

				// Just over write their tattoos and save, no need to diff since its done inside player.cs
				player.UpdateTattoos(lstNewTattoos, true);

				player.SendNotification("Tattoo Artist", ENotificationIcon.InfoSign, "Your tattoo art work was purchased for {0}", bHasToken ? "free (Legacy Character Tattoo Token)" : Helpers.FormatString("${0:0.00}", fPrice));
			}
		}
		else
		{
			player.SendNotification("Tattoo Artist", ENotificationIcon.ExclamationSign, "You did not add or remove any tattoo art work.");
		}


		// re-apply skin (this also re-applys tattoos)
		player.ApplySkinFromInventory();
	}
}



