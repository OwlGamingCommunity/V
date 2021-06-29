using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class NoteSystem 
{
	private const int MAX_NOTE_LENGTH = 150;
	public const uint NEARBY_NOTES_DISTANCE = 30;
	public NoteSystem()
	{
		CommandManager.RegisterCommand("writenote", "Write a note on a piece of paper", new Action<CPlayer, CVehicle, string>(WriteNoteCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("nearbynotes", "Returns a list of all nearbynotes", new Action<CPlayer, CVehicle>(NearbyNotesCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("delnearbynotes", "Deletes the nearby notes", new Action<CPlayer, CVehicle>(DeleteNearbyNotesCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("delnote", "Delete a specific note", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteNoteCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.AdminToggleNoteLock += OnAdminToggleLockNote;
	}

	public void WriteNoteCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (strMessage.Length > MAX_NOTE_LENGTH)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "Notes can be {0} characters maximum only.", MAX_NOTE_LENGTH);
			return;
		}

		HelperFunctions.Chat.SendAmeMessage(SenderPlayer, "writes something on a piece of paper");

		CItemValueNote noteItemVal = new CItemValueNote(strMessage, SenderPlayer.ActiveCharacterDatabaseID, false, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
		CItemInstanceDef itemDef = CItemInstanceDef.FromTypedObjectNoDBID(EItemID.NOTE, noteItemVal, 1);
		SenderPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, null);

		Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.ItemMovement, null, Helpers.FormatString("/writenote - '{0}' ", strMessage));
	}

	private void OnAdminToggleLockNote(CPlayer SenderPlayer, GTANetworkAPI.Object NoteObject)
	{
		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstanceHandle(NoteObject.Handle);

		if (pWorldItem != null)
		{
			if (pWorldItem.ItemInstance.ItemID == EItemID.NOTE)
			{
				CItemValueNote itemValue = (CItemValueNote)pWorldItem.ItemInstance.Value;
				EntityDatabaseID droppedBy = itemValue.placedBy;

				itemValue.AdminLocked = itemValue.AdminLocked ? false : true;

				Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);

				pWorldItem.SetData(NoteObject, EDataNames.NOTE_LOCKED, itemValue.AdminLocked, EDataType.Synced);

				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, Helpers.FormatString("Successfully {0} the note object.", Helpers.ColorString(255, 255, 255, "{0}", itemValue.AdminLocked ? "locked" : "unlocked")));
				Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("NOTES - {0} a note item ", itemValue.AdminLocked ? "locked" : "unlocked"));
			}
		}
	}

	private void NearbyNotesCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Notes:");
		ICollection<CWorldItem> worldItems = WorldItemPool.GetAllWorldItems();

		foreach (var item in worldItems)
		{
			if (item.ItemInstance.ItemID != EItemID.NOTE)
			{
				// If it's not a note continue.
				continue;
			}

			if (SenderPlayer.Client.Position.DistanceTo2D(item.GTAInstance.Position) <= NEARBY_NOTES_DISTANCE && SenderPlayer.Client.Dimension == item.GTAInstance.Dimension)
			{
				CItemValueNote itemValueNote = (CItemValueNote)item.ItemInstance.Value;
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, Helpers.FormatString("Note ID: {2} - Note: {0} - Owner: {1}", Helpers.ColorString(255, 255, 255, "{0}", itemValueNote.NoteMessage), Helpers.ColorString(255, 255, 255, "{0}", itemValueNote.CharacterDroppedName), Helpers.ColorString(255, 255, 255, "{0}", item.m_DatabaseID)));
			}
		}
	}

	private void DeleteNearbyNotesCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		ICollection<CWorldItem> worldItems = WorldItemPool.GetAllWorldItems();
		List<CWorldItem> lstNearbyNotes = new List<CWorldItem>();
		foreach (var item in worldItems)
		{
			if (item.ItemInstance.ItemID != EItemID.NOTE)
			{
				// If it's not a note continue.
				continue;
			}

			if (SenderPlayer.Client.Position.DistanceTo2D(item.GTAInstance.Position) <= NEARBY_NOTES_DISTANCE && SenderPlayer.Client.Dimension == item.GTAInstance.Dimension)
			{
				lstNearbyNotes.Add(item);
			}
		}

		foreach (var item in lstNearbyNotes)
		{
			EntityDatabaseID DBID = item.m_DatabaseID;
			// Destroy world item
			WorldItemPool.DestroyWorldItem(item);
			Database.Functions.Items.DestroyWorldItem(DBID);
		}

		SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Succesfully deleted the nearby notes.");
		Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.AdminCommand, null, "/delnearbynotes - bulk deleted nearby notes");
	}

	private void DeleteNoteCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, EntityDatabaseID noteID)
	{
		CWorldItem noteItem = WorldItemPool.GetWorldItemFromID(noteID);

		if (noteItem != null)
		{
			WorldItemPool.DestroyWorldItem(noteItem);
			Database.Functions.Items.DestroyWorldItem(noteID);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Succesfully deleted note with ID: {0}.", noteID);
			Logging.Log.CreateLog(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/delnote - deleted note ID:{0}", noteID));
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Note ID: {0} not found.", noteID);
		}
	}
}