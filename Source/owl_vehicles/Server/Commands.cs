using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

internal class Commands
{
	public Commands()
	{
		// COMMANDS
		CommandManager.RegisterCommand("sellveh", "Sells the vehicle you are in to someone nearby", new Action<CPlayer, CVehicle, CPlayer>(SellVehicle), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeInVehicle, aliases: new string[] { "sell", "sellcar" });
		CommandManager.RegisterCommand("setvehfaction", "Set the vehicle to faction ownership", new Action<CPlayer, CVehicle, EntityDatabaseID>(SetVehicleFactionOwner), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle, aliases: new string[] { "setcarfaction" });

		CommandManager.RegisterCommand("refuel", "Refuels target players vehicle", new Action<CPlayer, CVehicle, CPlayer>(Refuel), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "fuelveh", "fuelcar" });
		CommandManager.RegisterCommand("setfuel", "Sets target players vehicle fuel", new Action<CPlayer, CVehicle, CPlayer, float>(SetFuel), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

		CommandManager.RegisterCommand("vehlist", "Shows a list of all vehicles", new Action<CPlayer, CVehicle>(OpenVehiclesList), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private void OpenVehiclesList(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		NetworkEventSender.SendNetworkEvent_ShowVehiclesList(SourcePlayer);
	}

	public void SetFuel(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, float FuelLevel)
	{
		if (!TargetPlayer.Client.IsInVehicle)
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "{0} is not in a vehicle.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			return;
		}

		CVehicle targetPlayerVehicle = VehiclePool.GetVehicleFromGTAInstance(TargetPlayer.Client.Vehicle);
		targetPlayerVehicle.Fuel = FuelLevel;

		SourcePlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You have set the fuel to {1} for the vehicle of {0}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), FuelLevel);

		if (SourcePlayer != TargetPlayer)
		{
			TargetPlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "{0} has set your vehicle fuel to {1}.", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), FuelLevel);
		}

		new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/setfuel - Vehicle: {0} - Fuel level: {1}", targetPlayerVehicle.m_DatabaseID, FuelLevel)).execute();
	}

	public void Refuel(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
	{
		if (!TargetPlayer.Client.IsInVehicle)
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "{0} is not in a vehicle.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			return;
		}

		CVehicle targetPlayerVehicle = VehiclePool.GetVehicleFromGTAInstance(TargetPlayer.Client.Vehicle);
		targetPlayerVehicle.Fuel = 100.0f;

		SourcePlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You have refueled the vehicle of {0}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));

		if (SourcePlayer != TargetPlayer)
		{
			TargetPlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "{0} has refueled your vehicle.", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName));
		}

		new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/refuel - Vehicle: {0}", targetPlayerVehicle.m_DatabaseID)).execute();
	}

	private async void SellVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
	{
		if (!SourceVehicle.OwnedBy(SourcePlayer) && !SourcePlayer.IsAdmin())
		{
			return;
		}

		if (!TargetPlayer.Inventory.CanGiveItem(CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, 0), out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "{0} could not receive the vehicle key:<br>{1}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), strUserFriendlyMessage);
			return;
		}

		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, SourceVehicle.m_DatabaseID);

		await HelperFunctions.Items.DeleteAllItems(ItemInstanceDef).ConfigureAwait(true);
		TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
		{
			if (!bItemGranted)
			{
				SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "{0} doesn't have enough space for a key!", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
				return;
			}

			SourceVehicle.SetOwner(EVehicleType.PlayerOwned, TargetPlayer.ActiveCharacterDatabaseID);

			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You have sold the vehicle to {0}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			TargetPlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You now own a {0}!", SourceVehicle.GetFullDisplayName());

			new Logging.Log(SourcePlayer, Logging.ELogType.VehicleRelated, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/sellveh - Vehicle: {0} to {1}", SourceVehicle.m_DatabaseID, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName))).execute();
		});
	}

	private void SetVehicleFactionOwner(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID FactionID)
	{
		if (!SourceVehicle.OwnedBy(SourcePlayer) && !SourcePlayer.IsAdmin())
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "You cannot sell this vehicle.");
			return;
		}

		CFaction Faction = FactionPool.GetFactionFromID(FactionID);
		if (Faction == null)
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "Faction ID not found.");
			return;
		}

		if (!SourcePlayer.IsFactionManager(FactionID) && !SourcePlayer.IsAdmin())
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You must be a manager of the faction in order to assign a vehicle to it.");
			return;
		}

		if (SourceVehicle.m_bTokenPurchase)
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "Token vehicles can not be given to a faction.");
			return;
		}

		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, SourceVehicle.m_DatabaseID);
		HelperFunctions.Items.DeleteAllItems(ItemInstanceDef);
		SourceVehicle.SetOwner(EVehicleType.FactionOwned, FactionID);

		SourcePlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You have transferred the ownership of a {0} to {1}", SourceVehicle.GetFullDisplayName(), Faction.Name);
		new Logging.Log(SourcePlayer, Logging.ELogType.VehicleRelated, null, Helpers.FormatString("/setvehfaction - Vehicle: {0} to faction ID: {1}", SourceVehicle.m_DatabaseID, FactionID)).execute();
	}
}
