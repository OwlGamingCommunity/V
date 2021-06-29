using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

internal class Commands
{
	public Commands()
	{
		// COMMANDS
		CommandManager.RegisterCommand("sellproperty", "Sells the property you are in for a percentage of the value to government (fill in 0) or sell it to a player by entering their name", new Action<CPlayer, CVehicle, string>(SellProperty), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "sellint" });
		CommandManager.RegisterCommand("unrent", "No longer rent this property", new Action<CPlayer, CVehicle>(Unrent), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("setpropfaction", "Set the property to faction ownership", new Action<CPlayer, CVehicle, EntityDatabaseID>(SetPropertyFactionOwner), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "setintfaction" });
	}

	public async void SellProperty(CPlayer SourcePlayer, CVehicle SourceVehicle, string potentialPlayer)
	{
		// TODO_LAUNCH: support for world interiors for all of the functions below...
		if (SourcePlayer.Client.Dimension == 0)
		{
			return;
		}

		CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(SourcePlayer.Client.Dimension);
		if (Property == null)
		{
			return;
		}

		bool bOwned = Property.OwnedBy(SourcePlayer);
		bool bFactionMgr = Property.Model.OwnerType == EPropertyOwnerType.Faction && SourcePlayer.IsFactionManager(Property.Model.OwnerId);
		bool bAdmin = SourcePlayer.IsAdmin() && !potentialPlayer.Equals("0"); // disallow admins to sell to gov.
		if (!bOwned && !bFactionMgr && !bAdmin)
		{
			return;
		}

		if (potentialPlayer.Equals("0"))
		{
			if (Property.Model.PaymentsRemaining != 0)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You cannot sell this property to the government as you still owe {0} payments.", Property.Model.PaymentsRemaining);
				return;
			}

			if (Property.Model.IsTokenPurchase)
			{
				DonationPurchasable propertyToken = Donations.GetPropertyToken();
				if (propertyToken == null)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "Error: Could not return property token.");
					return;
				}

				if (!SourcePlayer.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.PropertyToken))
				{
					await SourcePlayer.DonationInventory.OnPurchaseScripted(propertyToken, true).ConfigureAwait(true);
				}
				SourcePlayer.SendNotification("Realtor", ENotificationIcon.InfoSign, "You have sold the property and received a property token.");
			}
			else
			{
				float value = Property.Model.BuyPrice * 0.7f;

				SourcePlayer.AddMoney(value, PlayerMoneyModificationReason.SellProperty_ToGovernment);
				SourcePlayer.SendNotification("Realtor", ENotificationIcon.InfoSign, "You have sold the property and received ${0:n}", value);
			}

			Property.Repossess(true);
			SourcePlayer.SetPositionSafe(Property.Model.EntrancePosition);
			SourcePlayer.SetSafeDimension(Property.Model.EntranceDimension);

			new Logging.Log(SourcePlayer, Logging.ELogType.PropertyRelated, null, Helpers.FormatString("/sellproperty - Character: {0} - GOVERNMENT - property ID: {1}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.InteriorId)).execute();
			return;
		}

		// Some Player?
		WeakReference<CPlayer> PlayerRef = CommandManager.FindTargetPlayer(SourcePlayer, potentialPlayer);
		CPlayer Player = PlayerRef.Instance();
		if (Player == null || Player.Client.Dimension != SourcePlayer.Client.Dimension)
		{
			SourcePlayer.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "No such player nearby.");
			return;
		}

		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, Property.Model.Id);
		if (!Player.Inventory.CanGiveItem(ItemInstanceDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "{0} could not receive the key:<br>{1}", Player.GetCharacterName(ENameType.StaticCharacterName), strUserFriendlyMessage);
			return;
		}

		await HelperFunctions.Items.DeleteAllItems(ItemInstanceDef).ConfigureAwait(true);

		Player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
		{
			if (!bItemGranted)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "{0} doesn't have enough space for a key!", Player.GetCharacterName(ENameType.StaticCharacterName));
				return;
			}

			Property.SetOwner(EPropertyOwnerType.Player, Player.ActiveCharacterDatabaseID);

			SourcePlayer.SendNotification("Realtor", ENotificationIcon.InfoSign, "Sold the property to {0}", Player.GetCharacterName(ENameType.StaticCharacterName));
			Player.SendNotification("Realtor", ENotificationIcon.InfoSign, "You now own the property {0}", Property.Model.Name);

			new Logging.Log(SourcePlayer, Logging.ELogType.PropertyRelated, new List<CBaseEntity> { Player }, Helpers.FormatString("/sellproperty - Character: {0} - To player: {1} - property ID: {2}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Player.GetCharacterName(ENameType.StaticCharacterName), Property.Model.InteriorId)).execute();
		});
	}

	public void Unrent(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		if (SourcePlayer.Client.Dimension == 0)
		{
			return;
		}

		CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(SourcePlayer.Client.Dimension);
		if (Property == null)
		{
			return;
		}

		if (!Property.RentedBy(SourcePlayer) && (Property.Model.RenterType != EPropertyOwnerType.Faction || !SourcePlayer.IsFactionManager(Property.Model.RenterId)))
		{
			return;
		}
		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValue(Property.Model.Id, EItemID.PROPERTY_KEY, Property.Model.Id);
		SourcePlayer.Inventory.RemoveItems(ItemInstanceDef, true);

		Property.Repossess();
		SourcePlayer.SetPositionSafe(Property.Model.EntrancePosition);
		SourcePlayer.SetSafeDimension(Property.Model.EntranceDimension);

		SourcePlayer.SendNotification("Realtor", ENotificationIcon.InfoSign, "You are not longer renting the property {0}", Property.Model.Name);

		new Logging.Log(SourcePlayer, Logging.ELogType.PropertyRelated, null, Helpers.FormatString("/unrent - Character: {0} - property ID: {1}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.InteriorId)).execute();
	}

	public void SetPropertyFactionOwner(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID factionID)
	{
		if (SourcePlayer.Client.Dimension == 0)
		{
			return;
		}

		CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(SourcePlayer.Client.Dimension);
		if (Property == null)
		{
			return;
		}

		if (!Property.OwnedBy(SourcePlayer) && !SourcePlayer.IsAdmin())
		{
			SourcePlayer.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You do not own this property.");
			return;
		}

		CFaction Faction = FactionPool.GetFactionFromID(factionID);
		if (Faction == null)
		{
			SourcePlayer.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "Faction ID not found.");
			return;
		}

		if (!SourcePlayer.IsFactionManager(factionID) && !SourcePlayer.IsAdmin())
		{
			SourcePlayer.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "You must be a manager of a faction to transfer a property to it.");
			return;
		}

		if (Property.Model.IsTokenPurchase)
		{
			SourcePlayer.SendNotification("Realtor", ENotificationIcon.ExclamationSign, "Token properties can not be given to a faction.");
			return;
		}

		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, Property.Model.Id);
		HelperFunctions.Items.DeleteAllItems(ItemInstanceDef);
		Property.SetOwner(EPropertyOwnerType.Faction, factionID);

		SourcePlayer.SendNotification("Realtor", ENotificationIcon.InfoSign, "You have transferred the ownership of {0} to {1}", Property.Model.Name, Faction.Name);
		new Logging.Log(SourcePlayer, Logging.ELogType.PropertyRelated, null, Helpers.FormatString("/setpropfaction - Character: {0} - property ID: {1} - Faction ID: {2}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.InteriorId, factionID)).execute();
	}
}
