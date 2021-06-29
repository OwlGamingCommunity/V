using GTANetworkAPI;
using System.Collections.Generic;

public class Halloween 
{
	public Halloween()
	{
		NetworkEvents.HalloweenInteraction += InteractWithHalloween;
		NetworkEvents.HalloweenCoffin += InteractWithHalloweenCoffin;
	}

	private void InteractWithHalloweenCoffin(CPlayer a_Player, bool bInPieces)
	{
		a_Player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "switch@trevor@annoys_sunbathers", "trev_annoys_sunbathers_loop_girl", !bInPieces, true, true, 0, true);
	}

	private void InteractWithHalloween(CPlayer a_Player)
	{
		if (HelperFunctions.World.IsHalloween())
		{
			// must be custom to get a mask
			if (a_Player.CharacterType == ECharacterType.Premade)
			{
				a_Player.SendNotification("Trick or Treat", ENotificationIcon.ExclamationSign, "Sorry, this feature is only available to custom characters.");
			}
			else
			{
				CItemValueClothingCustom clothingValue = new CItemValueClothingCustom(MaskHelpers.HalloweenMask, 0, true);
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.CLOTHES_CUSTOM_MASK, clothingValue);

				if (a_Player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem))
				{
					a_Player.SendNotification("Trick or Treat", ENotificationIcon.ExclamationSign, "You already had one of my treats! Be Gone!");
				}
				else
				{
					a_Player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							a_Player.ActivateCustomClothing(ItemInstanceDef);

							List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(a_Player, ChatConstants.g_fDistance_Nearby);

							foreach (var player in lstPlayers)
							{
								player.PushChatMessageWithPlayerNameAndPostfixAndColor(EChatChannel.Nearby, 0, 255, 0, ECharacterLanguage.English, "Fiendish Creature", "says", "Boo, {0} has been trick or treated!", a_Player.GetCharacterName(ENameType.StaticCharacterName));
							}

							a_Player.SendNotification("Halloween", ENotificationIcon.ExclamationSign, "You have received a Halloween mask. This item can be equipped like normal clothing, but is only obtainable during the Halloween Event.");

							a_Player.AwardAchievement(EAchievementID.Halloween);
						}
					});
				}
			}

		}
	}
}