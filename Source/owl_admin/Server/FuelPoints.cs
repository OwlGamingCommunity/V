using System;
using EntityDatabaseID = System.Int64;

public class FuelPoints
{
	public const uint NEARBY_FUELPOINTS_DISTANCE = 20;

	public FuelPoints()
	{
		// COMMANDS
		CommandManager.RegisterCommand("nearbyfuelpoints", "Shows all nearby fuel points", new Action<CPlayer, CVehicle>(NearbyFuelPointsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "nearbyfuel" });
		CommandManager.RegisterCommand("addfuelpoint", "Creates a fuel point", new Action<CPlayer, CVehicle>(CreateFuelPoint), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "addfuel" });
		CommandManager.RegisterCommand("delfuelpoint", "Deletes a fuel point", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteFuelPoint), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "delfuel" });
	}

	private void NearbyFuelPointsCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Fuel Points:");
		foreach (var kvPair in FuelStations.GetFuelPoints())
		{
			if (SourcePlayer.Client.Dimension == kvPair.Value.Dimension)
			{
				float fDist = SourcePlayer.Client.Position.DistanceTo2D(kvPair.Value.Position);
				if (fDist <= NEARBY_FUELPOINTS_DISTANCE)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1} distance", kvPair.Value.DatabaseID, fDist);
				}
			}
		}
	}

	// TODO: Fuel, banks etc might wanna use a pool?
	private void DeleteFuelPoint(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID fuelPointID)
	{
		CFuelPoint fuelPoint = FuelStations.GetFuelPointByID(fuelPointID);
		if (fuelPoint != null)
		{
			FuelStations.DestroyFuelPoint(fuelPoint, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a fuel point (#{0}).", fuelPointID);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Fuel point not found.");
		}
	}

	private async void CreateFuelPoint(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		CFuelPoint fuelPoint = await FuelStations.CreateFuelPoint(-1, SourcePlayer.GetEstimatedGroundPosition(), SourcePlayer.Client.Dimension, true).ConfigureAwait(true);
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have created a fuel point (#{0}).", fuelPoint.DatabaseID);
	}
}