using System;
using EntityDatabaseID = System.Int64;

public class Banks
{
	public const uint NEARBY_DISTANCE = 20;

	public Banks()
	{
		// COMMANDS
		CommandManager.RegisterCommand("nearbybanks", "Shows all nearby banks", new Action<CPlayer, CVehicle>(NearbyBanksCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("addbank", "Creates a bank", new Action<CPlayer, CVehicle, EBankSystemType>(CreateBank), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("delbank", "Deletes a bank", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteBank), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private void NearbyBanksCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Banks:");
		foreach (var kvPair in BankPool.GetBanks())
		{
			if (SourcePlayer.Client.Dimension == kvPair.Value.m_dimension)
			{
				float fDist = SourcePlayer.Client.Position.DistanceTo2D(kvPair.Value.m_vecPos);
				if (fDist <= NEARBY_DISTANCE)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1} - {2} distance", kvPair.Value.m_DatabaseID, kvPair.Value.m_bankType, fDist);
				}
			}
		}
	}

	private void DeleteBank(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID bankID)
	{
		CBankInstance bankInst = BankPool.GetBankByID(bankID);
		if (bankInst != null)
		{
			BankPool.DestroyBank(bankInst, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a bank (#{0}).", bankID);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Bank not found.");
		}
	}

	private async void CreateBank(CPlayer SourcePlayer, CVehicle SourceVehicle, EBankSystemType bankType)
	{
		CBankInstance bankInst = await BankPool.CreateBank(-1, SourcePlayer.Client.Position, SourcePlayer.Client.Rotation.Z, bankType, SourcePlayer.Client.Dimension, true).ConfigureAwait(true);
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have created a bank ({1} (#{0}).", bankInst.m_DatabaseID, bankType);
	}
}