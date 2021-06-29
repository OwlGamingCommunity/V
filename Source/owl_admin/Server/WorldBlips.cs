using System;
using EntityDatabaseID = System.Int64;

public class WorldBlips
{
	public const uint NEARBY_DISTANCE = 100;

	public WorldBlips()
	{
		// COMMANDS
		CommandManager.RegisterCommand("nearbyworldblips", "Shows all nearby world blips", new Action<CPlayer, CVehicle>(NearbyWorldBlipsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("addworldblip", "Creates a world blip", new Action<CPlayer, CVehicle, int, int, string>(CreateWorldBlip), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("delworldblip", "Deletes a world blip", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteWorldBlips), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private void NearbyWorldBlipsCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby World Blips:");
		foreach (var kvPair in WorldBlipPool.GetWorldBlips())
		{
			if (SourcePlayer.Client.Dimension == 0)
			{
				float fDist = SourcePlayer.Client.Position.DistanceTo2D(kvPair.Value.m_vecPos);
				if (fDist <= NEARBY_DISTANCE)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1} - Sprite {2} - {3} distance", kvPair.Value.m_DatabaseID, kvPair.Value.m_strName, kvPair.Value.m_Sprite, fDist);
				}
			}
		}
	}

	private void DeleteWorldBlips(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID ID)
	{
		if (!SourcePlayer.IsAdmin(EAdminLevel.LeadAdmin))
		{
			return;
		}

		CWorldBlipInstance blipInst = WorldBlipPool.GetInstanceFromID(ID);
		if (blipInst != null)
		{
			WorldBlipPool.DestroyWorldBlip(blipInst, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a world blip (#{0}).", ID);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "World Blip not found.");
		}
	}

	private async void CreateWorldBlip(CPlayer SourcePlayer, CVehicle SourceVehicle, int Sprite, int Color, string strName)
	{
		if (!SourcePlayer.IsAdmin(EAdminLevel.LeadAdmin))
		{
			return;
		}

		CWorldBlipInstance blipInst = await WorldBlipPool.CreateWorldBlip(-1, strName, Sprite, Color, SourcePlayer.Client.Position, true).ConfigureAwait(true);
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have created a world blip ({1} - Sprite {2} (#{0}).", blipInst.m_DatabaseID, blipInst.m_strName, blipInst.m_Sprite);
	}
}