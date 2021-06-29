using System;
using EntityDatabaseID = System.Int64;

public class Stores
{
	public const uint NEARBY_DISTANCE = 20;

	public Stores()
	{
		// COMMANDS
		CommandManager.RegisterCommand("nearbystores", "Shows all nearby stores", new Action<CPlayer, CVehicle>(NearbyStoresCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("gotostore", "Teleports to a store", new Action<CPlayer, CVehicle, EntityDatabaseID>(GotoStoreCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("addstore", "Creates a store", new Action<CPlayer, CVehicle, EStoreType, EntityDatabaseID>(CreateStore), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("delstore", "Deletes a store", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteStore), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private void NearbyStoresCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Stores:");
		foreach (var kvPair in StorePool.GetStores())
		{
			if (SourcePlayer.Client.Dimension == kvPair.Value.m_dimension)
			{
				float fDist = SourcePlayer.Client.Position.DistanceTo2D(kvPair.Value.m_vecPos);
				if (fDist <= NEARBY_DISTANCE)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1} - {2} distance", kvPair.Value.m_DatabaseID, kvPair.Value.m_storeType, fDist);
				}
			}
		}
	}

	private void GotoStoreCommand(CPlayer player, CVehicle vehicle, EntityDatabaseID storeID)
	{
		CStoreInstance storeInst = StorePool.GetInstanceFromID(storeID);
		if (storeInst == null)
		{
			player.SendNotification("Stores", ENotificationIcon.ExclamationSign, "That store does not exist.");
			return;
		}

		player.Client.Position = storeInst.m_vecPos;
		player.Client.Dimension = storeInst.m_dimension;
		player.SendNotification("Stores", ENotificationIcon.InfoSign, "Teleported to store {0}.", storeID);
	}

	private void DeleteStore(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID ID)
	{
		CStoreInstance storeInst = StorePool.GetInstanceFromID(ID);
		if (storeInst != null)
		{
			StorePool.DestroyStore(storeInst, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a store (#{0}).", ID);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Store not found.");
		}
	}

	private async void CreateStore(CPlayer SourcePlayer, CVehicle SourceVehicle, EStoreType storeType, EntityDatabaseID PropertyToTiePurchasesToOrMinusOne)
	{
		CStoreInstance storeInt = await StorePool.CreateStore(-1, SourcePlayer.Client.Position, SourcePlayer.Client.Rotation.Z, storeType, SourcePlayer.Client.Dimension, PropertyToTiePurchasesToOrMinusOne, 0, true).ConfigureAwait(true);

		if (storeInt != null)
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have created a store ({1} (#{0}).", storeInt.m_DatabaseID, storeType);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Unable to create store! Please report on the mantis.");
		}
	}
}