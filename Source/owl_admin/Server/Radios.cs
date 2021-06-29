using System;
using EntityDatabaseID = System.Int64;

public class Radios
{
	public Radios()
	{
		// COMMANDS
		CommandManager.RegisterCommand("alistradios", "Shows all radio stations", new Action<CPlayer, CVehicle>(AdminListRadiosCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("adelradio", "Deletes a radio station", new Action<CPlayer, CVehicle, EntityDatabaseID>(AdminDeleteRadio), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private async void AdminListRadiosCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Radio Stations:");
		foreach (var kvPair in RadioPool.GetRadios())
		{
			RadioInstance radioInst = kvPair;
			string strAccountName = await Database.LegacyFunctions.GetUsernameFromAccount(radioInst.Account).ConfigureAwait(true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1} - {2}", radioInst.ID, String.IsNullOrEmpty(strAccountName) ? "DEFAULT RADIO" : strAccountName, radioInst.Endpoint);
		}
	}

	private void AdminDeleteRadio(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID radioID)
	{
		RadioInstance radioInst = RadioPool.GetRadioFromID(radioID);
		if (radioInst != null)
		{
			RadioPool.DestroyRadio(radioInst, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a radio (#{0}).", radioID);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Radio not found.");
		}
	}
}