using GTANetworkAPI;
using EntityDatabaseID = System.Int64;

public class PlasticSurgeon
{
	public PlasticSurgeon()
	{
		NetworkEvents.EnterPlasticSurgeon += OnEnterPlasticSurgeon;
		NetworkEvents.DoCharacterTypeUpgrade += OnDoCharacterTypeUpgrade;

		NetworkEvents.PlasticSurgeon_CalculatePrice += CalculatePrice;
		NetworkEvents.PlasticSurgeon_Checkout += OnCheckout;
	}

	private async void OnDoCharacterTypeUpgrade(CPlayer player)
	{
		// change character type
		await Database.LegacyFunctions.ChangeCharacterType(player.ActiveCharacterDatabaseID, ECharacterType.Custom).ConfigureAwait(true);

		// Remove any premade skin items
		player.Inventory.RemoveAllItemsOfType(EItemID.CLOTHES);

		// Give default clothes
		EItemID[] itemsToGive =
		{
			EItemID.CLOTHES_CUSTOM_LEGS,
			EItemID.CLOTHES_CUSTOM_FEET,
			EItemID.CLOTHES_CUSTOM_ACCESSORY,
			EItemID.CLOTHES_CUSTOM_UNDERSHIRT,
			EItemID.CLOTHES_CUSTOM_DECALS,
			EItemID.CLOTHES_CUSTOM_TOPS,
			EItemID.CLOTHES_CUSTOM_HELMET,
			EItemID.CLOTHES_CUSTOM_GLASSES,
			EItemID.CLOTHES_CUSTOM_EARRINGS,
			EItemID.CLOTHES_CUSTOM_WATCHES,
			EItemID.CLOTHES_CUSTOM_BRACELETS
		};

		foreach (EItemID item in itemsToGive)
		{
			// Add new item
			CItemValueClothingCustom clothingValue = new CItemValueClothingCustom(0, 0, true);
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(item, clothingValue);
			player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
		}

		// Give a free token for each type of change, if they don't have one
		// barber token
		if (!player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Barber))
		{
			DonationPurchasable token = null;

			foreach (var purchasable in Donations.g_lstPurchasables)
			{
				if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Barber)
				{
					token = purchasable;
					break;
				}
			}

			if (token != null) { await player.DonationInventory.OnPurchaseScripted(token, true).ConfigureAwait(true); }
		}

		// clothing store token
		if (!player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Clothing_Store))
		{
			DonationPurchasable token = null;

			foreach (var purchasable in Donations.g_lstPurchasables)
			{
				if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Clothing_Store)
				{
					token = purchasable;
					break;
				}
			}

			if (token != null) { await player.DonationInventory.OnPurchaseScripted(token, true).ConfigureAwait(true); }
		}

		// tattoo artist token
		if (!player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Tattoo))
		{
			DonationPurchasable token = null;

			foreach (var purchasable in Donations.g_lstPurchasables)
			{
				if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Tattoo)
				{
					token = purchasable;
					break;
				}
			}

			if (token != null) { await player.DonationInventory.OnPurchaseScripted(token, true).ConfigureAwait(true); }
		}

		// plastic surgeon token
		if (!player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Plastic_Surgeon))
		{
			DonationPurchasable token = null;

			foreach (var purchasable in Donations.g_lstPurchasables)
			{
				if (purchasable.DonationEffect == EDonationEffect.Free_Visit_Plastic_Surgeon)
				{
					token = purchasable;
					break;
				}
			}

			if (token != null) { await player.DonationInventory.OnPurchaseScripted(token, true).ConfigureAwait(true); }
		}

		player.SendNotification("Premade Character", ENotificationIcon.InfoSign, "Your character was converted to a custom character.<br><br>You must visit the barbers, clothing store and plastic surgeon to customize your character<br><br>You have been granted:<br><br>1 free visit to the Barber<br>1 free visit to the Clothing Store<br>1 free visit to the Tattoo Artist<br>1 free visit to the Plastic Surgeon");

		// Save character (we respawn them so we ensure correct data etc and future proofing)
		player.Save();

		NetworkEvents.SendLocalEvent_ForceReSelectCharacter(player, player.ActiveCharacterDatabaseID);
	}

	private void OnEnterPlasticSurgeon(CPlayer player)
	{
		if (player.CharacterType == ECharacterType.Custom)
		{
			player.GotoPlayerSpecificDimension();
			player.CacheHealthAndArmor();

			// Force the normal skin, we don't let them modify job or duty skins here.
			player.ApplySkinFromInventory(true, true);

			NetworkEventSender.SendNetworkEvent_EnterPlasticSurgeon_Response(player);
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_EnterPlasticSurgeon_OfferCharacterUpgrade(player);
		}
	}

	private void CalculateChangesCost(CPlayer player, out float fPrice, out bool bHasToken)
	{
		fPrice = 100.0f;
		bHasToken = player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.Free_Visit_Plastic_Surgeon);
	}

	private void CalculatePrice(CPlayer player)
	{
		CalculateChangesCost(player, out float fPrice, out bool bHasToken);
		NetworkEventSender.SendNetworkEvent_PlasticSurgeon_GotPrice(player, fPrice, bHasToken);
	}

	private void OnCheckout(CPlayer player, EntityDatabaseID storeID, int Ageing, float AgeingOpacity, int Makeup, float MakeupOpacity, int MakeupColor, int MakeupColorHighlight, int Blush, float BlushOpacity, int BlushColor, int BlushColorHighlight,
											int Complexion, float ComplexionOpacity, int SunDamage, float SunDamageOpacity, int Lipstick, float LipstickOpacity, int LipstickColor,
											int LipstickColorHighlights, int MolesAndFreckles, float MolesAndFrecklesOpacity, float NoseSizeHorizontal, float NoseSizeVertical, float NoseSizeOutwards, float NoseSizeOutwardsUpper,
											float NoseSizeOutwardsLower, float NoseAngle, float EyebrowHeight, float EyebrowDepth, float CheekboneHeight, float CheekWidth, float CheekWidthLower, float EyeSize, float LipSize,
											float MouthSize, float MouthSizeLower, float ChinSize, float ChinSizeLower, float ChinWidth, float ChinEffect, float NeckWidth, float NeckWidthLower, int FaceBlend1Mother,
											int FaceBlend1Father, float FaceBlendFatherPercent, float SkinBlendFatherPercent, int EyeColor, int Blemishes, float BlemishesOpacity, int Eyebrows, float EyebrowsOpacity,
											int EyebrowsColor, int EyebrowsColorHighlight, int BodyBlemishes, float BodyBlemishesOpacity)
	{
		CalculateChangesCost(player, out float fPrice, out bool bHasToken);

		bool bWasPurchased = false;

		if (fPrice > 0.0f)
		{
			// Do we have a token?
			if (bHasToken)
			{
				// consume token
				player.DonationInventory.RemoveTokenOfTypeForActiveCharacter(EDonationEffect.Free_Visit_Plastic_Surgeon);
				bWasPurchased = true;
			}
			else if (player.SubtractMoney(fPrice, PlayerMoneyModificationReason.PlasticSurgeonCheckout))
			{
				bWasPurchased = true;
			}
			else
			{
				bWasPurchased = false;
				player.SendNotification("Plastic Surgeon", ENotificationIcon.ExclamationSign, "You do not have enough money to purchase this plastic surgery work.");
			}

			if (bWasPurchased)
			{
				StoreSystem.HandleStoreTransactionOwnerShare(storeID, fPrice);

				// TODO_PLASTIC_SURGEON: Save changes
				player.UpdateCustomCharacterBodyData(true, Ageing, AgeingOpacity, Makeup, MakeupOpacity, MakeupColor, MakeupColorHighlight, Blush, BlushOpacity, BlushColor, BlushColorHighlight,
											Complexion, ComplexionOpacity, SunDamage, SunDamageOpacity, Lipstick, LipstickOpacity, LipstickColor,
											LipstickColorHighlights, MolesAndFreckles, MolesAndFrecklesOpacity, NoseSizeHorizontal, NoseSizeVertical, NoseSizeOutwards, NoseSizeOutwardsUpper,
											NoseSizeOutwardsLower, NoseAngle, EyebrowHeight, EyebrowDepth, CheekboneHeight, CheekWidth, CheekWidthLower, EyeSize, LipSize,
											MouthSize, MouthSizeLower, ChinSize, ChinSizeLower, ChinWidth, ChinEffect, NeckWidth, NeckWidthLower, FaceBlend1Mother,
											FaceBlend1Father, FaceBlendFatherPercent, SkinBlendFatherPercent, EyeColor, Blemishes, BlemishesOpacity, Eyebrows, EyebrowsOpacity,
											EyebrowsColor, EyebrowsColorHighlight, BodyBlemishes, BodyBlemishesOpacity);

				player.SendNotification("Plastic Surgeon", ENotificationIcon.InfoSign, "Your plastic surgery work was purchased for {0}", bHasToken ? "free (Legacy Character Plastic Surgeon Token)" : Helpers.FormatString("${0:0.00}", fPrice));
			}
		}
		else
		{
			player.SendNotification("Plastic Surgeon", ENotificationIcon.ExclamationSign, "You did not perform any surgery.");
		}
	}
}



