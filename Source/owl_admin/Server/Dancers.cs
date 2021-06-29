using GTANetworkAPI;
using System;
using EntityDatabaseID = System.Int64;

public class Dancers
{
	public const uint NEARBY_DISTANCE = 20;

	public Dancers()
	{
		// COMMANDS
		CommandManager.RegisterCommand("nearbydancers", "Shows all nearby dancers", new Action<CPlayer, CVehicle>(NearbyDancersCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("adddancer", "Creates a dancer", new Action<CPlayer, CVehicle, uint, string, string, bool, EntityDatabaseID>(CreateDancer), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("deldancer", "Deletes a dancer", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteDancer), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("gotodancer", "Teleports to a dancer", new Action<CPlayer, CVehicle, EntityDatabaseID>(GoToDancerCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private void NearbyDancersCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Dancers:");
		foreach (var kvPair in DancerPool.GetDancers())
		{
			if (SourcePlayer.Client.Dimension == kvPair.Value.m_dimension)
			{
				float fDist = SourcePlayer.Client.Position.DistanceTo2D(kvPair.Value.m_vecPos);
				if (fDist <= NEARBY_DISTANCE)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1} - tipping allowed: {2} - current tips: ${3} - parent property ID: {4} - {5} distance", kvPair.Value.m_DatabaseID, (PedHash)kvPair.Value.m_dancerSkin, kvPair.Value.m_bAllowTip, kvPair.Value.m_tipMoney, kvPair.Value.m_parentPropertyID, fDist);
				}
			}
		}
	}

	private void GoToDancerCommand(CPlayer player, CVehicle vehicle, EntityDatabaseID dancerID)
	{
		CDancerInstance dancerInst = DancerPool.GetInstanceFromID(dancerID);
		if (dancerInst == null)
		{
			player.SendNotification("Dancers", ENotificationIcon.ExclamationSign, "That dancer does not exist.");
			return;
		}

		player.Client.Position = dancerInst.m_vecPos;
		player.Client.Dimension = dancerInst.m_dimension;
		player.SendNotification("Stores", ENotificationIcon.InfoSign, "Teleported to store {0}.", dancerID);
	}

	private void DeleteDancer(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID ID)
	{
		CDancerInstance dancerInst = DancerPool.GetInstanceFromID(ID);
		if (dancerInst != null)
		{
			DancerPool.DestroyDancer(dancerInst, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a dancer (#{0}).", ID);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Dancer not found.");
		}
	}

	private async void CreateDancer(CPlayer SourcePlayer, CVehicle SourceVehicle, uint dancerSkin, string animDict, string animName, bool AllowTip, EntityDatabaseID ParentPropertyOrMinusOne)
	{
		if (!Enum.IsDefined(typeof(PedHash), dancerSkin))
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "{0} is not a valid skin.");
			return;
		}

		CDancerInstance dancerInt = await DancerPool.CreateDancer(-1, SourcePlayer.Client.Position, SourcePlayer.Client.Rotation.Z, dancerSkin, SourcePlayer.Client.Dimension, 0f, animDict, animName, AllowTip, ParentPropertyOrMinusOne, true).ConfigureAwait(true);

		if (dancerInt != null)
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have created a dancer ({1} (#{0}).", dancerInt.m_DatabaseID, (PedHash)dancerInt.m_dancerSkin);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Unable to create dancer! Please report on the mantis.");
		}
	}
}