using GTANetworkAPI;
using System.Collections.Generic;

public class Christmas 
{
	public Christmas()
	{
		NetworkEvents.TalkToSanta += TalkToSanta;
	}

	private void TalkToSanta(CPlayer a_Player)
	{
		if (HelperFunctions.World.IsChristmas())
		{
			uint maxSnowballs = 10;

			if (a_Player.Inventory.HasItem(CItemInstanceDef.FromBasicValueNoDBID(EItemID.WEAPON_SNOWBALL, 0.0f), false, out CItemInstanceDef outItem))
			{
				a_Player.SendNotification("Santa", ENotificationIcon.ExclamationSign, "You're a naughty person who already has too many snowballs! I should give you some coal.");
			}
			else
			{
				a_Player.Inventory.AddItemToNextFreeSuitableSlot(CItemInstanceDef.FromBasicValueNoDBID(EItemID.WEAPON_SNOWBALL, 0.0f, maxSnowballs), EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
				{
					if (bItemGranted)
					{
						List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(a_Player, ChatConstants.g_fDistance_Nearby);

						foreach (var player in lstPlayers)
						{
							player.PushChatMessageWithPlayerNameAndPostfixAndColor(EChatChannel.Nearby, 0, 255, 0, ECharacterLanguage.English, "Santa", "says", "Ho ho ho, {0} has been a good {1}!", a_Player.GetCharacterName(ENameType.StaticCharacterName), a_Player.Gender == EGender.Male ? "boy" : "girl");
						}

						a_Player.SendNotification("Santa", ENotificationIcon.ExclamationSign, "You have received {0} snowballs.", maxSnowballs);

						a_Player.AwardAchievement(EAchievementID.Christmas);
					}
				});

				// if custom char, give them a santa mask
				if (a_Player.CharacterType == ECharacterType.Custom)
				{
					CItemValueClothingCustom clothingValue = new CItemValueClothingCustom(MaskHelpers.ChristmasMask, 0, true);
					CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.CLOTHES_CUSTOM_MASK, clothingValue);

					if (!a_Player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItemMask))
					{
						a_Player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
						{
							if (bItemGranted)
							{
								a_Player.ActivateCustomClothing(ItemInstanceDef);

								a_Player.SendNotification("Halloween", ENotificationIcon.ExclamationSign, "You have received a Santa mask. This item can be equipped like normal clothing, but is only obtainable during the Christmas Event.");

								a_Player.AwardAchievement(EAchievementID.Halloween);
							}
						});
					}
				}
			}

		}
	}
}