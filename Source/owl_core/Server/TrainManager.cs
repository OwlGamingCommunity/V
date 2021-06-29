//#define DEBUG_TRAINS
#define ADMIN_SPAWNED_TRAINS_ONLY
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

class TrainManager 
{
	private List<CServerTrainInstance> m_lstTrains = new List<CServerTrainInstance>();

	private int g_nextTrainID = 0;
	public TrainManager()
	{
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;

		// create AI train just before every 3rd station
#if !ADMIN_SPAWNED_TRAINS_ONLY
		int tripwireID = 0;
		int stationID = 0;
		foreach (CTrainTripWire tripWire in TrainConstants.TripWires)
		{


#if DEBUG_TRAINS
			if (tripWire.TripWireType == ETrainTripWireType.StationStop)
#else
			if (tripWire.TripWireType == ETrainTripWireType.ApproachingStation)
#endif
			{
#if DEBUG_TRAINS
				if (stationID == 0)
#else
				if (stationID % 3 == 0)
#endif
				{
					CreateTrain(ETrainType.Metro, tripwireID, tripWire.Sector, tripWire.Position, g_nextTrainID);
					++g_nextTrainID;
				}

				++stationID;
			}

			++tripwireID;
		}
#endif

		RageEvents.RAGE_OnUpdate += CheckSyncers;

		RageEvents.RAGE_OnPlayerDisconnected += OnDisconnect;

		NetworkEvents.TrainEnter += OnRequestTrainEnter;
		NetworkEvents.TrainExit += OnRequestTrainExit;
		NetworkEvents.TrainSync += OnTrainSync;
		NetworkEvents.TrainDoorStateChanged += OnTrainDoorStateChanged;

		CommandManager.RegisterCommand("gototrain", "Teleports to a train", new Action<CPlayer, CVehicle, int>(GotoTrain), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("trains", "Lists all trains", new Action<CPlayer, CVehicle>(TrainList), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("addtrain", "Adds a train", new Action<CPlayer, CVehicle, int>(AddTrain), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("deltrain", "Deletes a train", new Action<CPlayer, CVehicle, int>(DelTrain), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
	}

	private void TrainList(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Trains ({0}):", m_lstTrains.Count);

		foreach (CServerTrainInstance train in m_lstTrains)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "#{0}", train.ID);
		}
	}

	private void AddTrain(CPlayer SenderPlayer, CVehicle SenderVehicle, int stationID)
	{
		if (!SenderPlayer.IsDaniels())
		{
			return;
		}

		int tripwireID = 0;
		int stationIDIter = 0;
		CTrainTripWire tripWireToUse = null;
		foreach (CTrainTripWire tripWire in TrainConstants.TripWires)
		{
			if (tripWire.TripWireType == ETrainTripWireType.StationStop)
			{
				if (stationIDIter == stationID)
				{
					tripWireToUse = tripWire;
					break;
				}

				++stationIDIter;
			}

			++tripwireID;
		}

		if (tripWireToUse != null)
		{
			CreateTrain(ETrainType.Metro, tripwireID, tripWireToUse.Sector, tripWireToUse.Position, g_nextTrainID);
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Train created with ID #{0} at station #{1} ({2})", g_nextTrainID, stationID, tripWireToUse.Name);
			++g_nextTrainID;
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Station {0} was not found. Use /trainstations to see a list of all stations", stationID);
		}
	}

	private void DelTrain(CPlayer SenderPlayer, CVehicle SenderVehicle, int trainID)
	{
		CServerTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Deleted train #{0}", trainID);
			// TODO_TRAINS: What happens to those inside? call leave on all occupants and transport to station? Maybe we dont care since these CMD's are probably debug only
			m_lstTrains.Remove(trainInst);
			NetworkEventSender.SendNetworkEvent_DestroySyncedTrain_ForAll_IncludeEveryone(trainID);
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Train #{0} was not found. Use /trains to see a list of all trains", trainID);
		}
	}

	private void GotoTrain(CPlayer SenderPlayer, CVehicle SenderVehicle, int trainID)
	{
		CServerTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 100, 255, "Teleported to train #{0}", trainID);

			SenderPlayer.SetSafeDimension(0);
			SenderPlayer.Client.Position = trainInst.Position;

			SenderPlayer.OnTeleport();
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Train #{0} was not found. Use /trains to see a list of all trains", trainID);
		}
	}

	private void OnTrainDoorStateChanged(CPlayer a_Syncer, int trainID, bool bDoorsOpen)
	{
		CServerTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			trainInst.OnTrainDoorStateChanged(bDoorsOpen);

			// send ack back to sender
			NetworkEventSender.SendNetworkEvent_TrainSync_Ack(a_Syncer, trainID);

			// send to all but the syncer
			foreach (CPlayer player in PlayerPool.GetAllPlayers_IncludeOutOfGame())
			{
				if (player != a_Syncer)
				{
					NetworkEventSender.SendNetworkEvent_TrainDoorStateChanged(player, trainID, bDoorsOpen);
				}
			}
		}
	}

	private void OnTrainSync(CPlayer a_Syncer, int trainID, float x, float y, float z, float speed, int tripwireID, int currentSector)
	{
		CServerTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			trainInst.OnSyncArrived(new Vector3(x, y, z), speed, tripwireID, currentSector);

			// send ack back to sender
			NetworkEventSender.SendNetworkEvent_TrainSync_Ack(a_Syncer, trainID);

			// send to all but the syncer
			foreach (CPlayer player in PlayerPool.GetAllPlayers_IncludeOutOfGame())
			{
				if (player != a_Syncer)
				{
					NetworkEventSender.SendNetworkEvent_TrainSync(player, trainID, x, y, z, speed, tripwireID, currentSector);
				}
			}
		}
	}

	private CServerTrainInstance GetTrainFromID(int ID)
	{
		foreach (CServerTrainInstance trainInst in m_lstTrains)
		{
			if (trainInst.ID == ID)
			{
				return trainInst;
			}
		}

		return null;
	}

	private void OnRequestTrainExit(CPlayer a_Player, int trainID)
	{
		NetworkEventSender.SendNetworkEvent_TrainExit_Approved_ForAll_IncludeEveryone(a_Player.Client, trainID);

		CServerTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			trainInst.RemoveOccupant(a_Player.Client);
		}
	}

	private void OnRequestTrainEnter(CPlayer a_Player, int trainID, bool bAsDriver)
	{
		// TODO_TRAIN: Check job type for driver, etc
		if (bAsDriver)
		{
			// TODO_TRAIN: Only for head admins at the moment
			if (!a_Player.IsAdmin(EAdminLevel.HeadAdmin))
			{
				return;
			}

			CServerTrainInstance trainInst = GetTrainFromID(trainID);
			if (trainInst != null)
			{
				// does it already have an occupant as driver?
				if (!trainInst.GetOccupant(EVehicleSeat.Driver, out Player currentDriver))
				{
					// set as occupant
					trainInst.AddOccupant(a_Player.Client, EVehicleSeat.Driver);
					NetworkEventSender.SendNetworkEvent_TrainEnter_Approved_ForAll_IncludeEveryone(a_Player.Client, trainID, EVehicleSeat.Driver);

					// update syncer
					trainInst.SetSyncer(a_Player.Client,
					(GTANetworkAPI.Player oldSyncer) =>
					{
						CPlayer oldSyncerPlayer = PlayerPool.GetPlayerFromClient(oldSyncer).Instance();
						if (oldSyncerPlayer != null)
						{
							NetworkEventSender.SendNetworkEvent_TrainSync_TakeOwnership(oldSyncerPlayer, trainInst.ID);
						}
					},
					(GTANetworkAPI.Player newSyncer) =>
					{
						CPlayer oldSyncerPlayer = PlayerPool.GetPlayerFromClient(newSyncer).Instance();
						if (oldSyncerPlayer != null)
						{
							NetworkEventSender.SendNetworkEvent_TrainSync_GiveOwnership(oldSyncerPlayer, trainInst.ID);
						}
					});

					//HelperFunctions.Chat.SendServerMessage(Helpers.FormatString("Train {0} syncer is now: {1} (Reason: Driver)", trainID, a_Player.GetCharacterName(ENameType.StaticCharacterName)));
				}
				else
				{
					a_Player.SendNotification("Train", ENotificationIcon.ExclamationSign, "This train already has a driver.");
					return; // don't send the approve, just return
				}
			}
		}
		else
		{
			// find the next free passenger seat
			CServerTrainInstance trainInst = GetTrainFromID(trainID);
			if (trainInst != null)
			{
				if (trainInst.GetNextFreePassengerSeat(out EVehicleSeat TrainSeat))
				{
					//HelperFunctions.Chat.SendServerMessage(Helpers.FormatString("{0} entered train {1} as passenger in seat {2}", a_Player.GetCharacterName(ENameType.StaticCharacterName), trainID, TrainSeat));
					trainInst.AddOccupant(a_Player.Client, TrainSeat);
					NetworkEventSender.SendNetworkEvent_TrainEnter_Approved_ForAll_IncludeEveryone(a_Player.Client, trainID, TrainSeat);
				}
				else
				{
					a_Player.SendNotification("Train", ENotificationIcon.ExclamationSign, "This train is full.");
					return; // don't send the approve, just return
				}
			}
		}
	}

	private void OnDisconnect(Player client, DisconnectionType type, string reason)
	{
		// remove any trains we are syncing OR were occupying
		foreach (CServerTrainInstance trainInst in m_lstTrains)
		{
			if (trainInst.GetSyncer() == client)
			{
				trainInst.SetSyncer(null,
					(GTANetworkAPI.Player oldSyncer) => { },
					(GTANetworkAPI.Player newSyncer) => { }
				);
			}

			trainInst.RemoveOccupant(client);
		}

		// find a new streamer
		CheckSyncers();
	}

	private void CheckSyncers()
	{
		int numTrains = m_lstTrains.Count;
		List<EntityDatabaseID> lstPlayersUsed = new List<EntityDatabaseID>(); // used so one player isnt syncing everything
		var PlayerList = PlayerPool.GetAllPlayers();

		foreach (CServerTrainInstance trainInst in m_lstTrains)
		{
			bool bApplyNewSyncer = false;
			Player newSyncerPlayer = null;
			GTANetworkAPI.Player syncer = trainInst.GetSyncer();

			// Do we have a driver? If so we're the syncer!
			if (trainInst.GetOccupant(EVehicleSeat.Driver, out Player DriverPlayer))
			{
				if (syncer != DriverPlayer) // TODO_TRAIN: What if player logs out, we should remove them from the train (as driver + passenger)
				{
					bApplyNewSyncer = true;
					newSyncerPlayer = DriverPlayer;
				}
			}

			// If we get here, we dont have a driver, or the driver wasn't valid for some reason
			if (syncer == null && !bApplyNewSyncer)
			{
				// Find a syncer
				Vector3 vecTrainPos = trainInst.Position;

				foreach (var player in PlayerList)
				{
					// Make sure we arent in the ignore list
					if (!lstPlayersUsed.Contains(player.AccountID))
					{
						bApplyNewSyncer = true;
						newSyncerPlayer = player.Client;

						lstPlayersUsed.Add(player.AccountID);

						// Are we out of players? Wipe the ignore list so we wrap around
						if (lstPlayersUsed.Count == PlayerList.Count)
						{
							lstPlayersUsed.Clear();
						}

						break;
					}
				}
			}

			if (bApplyNewSyncer && newSyncerPlayer != null)
			{
				//HelperFunctions.Chat.SendServerMessage(Helpers.FormatString("Train {0} syncer is now: {1}", trainInst.ID, newSyncerPlayer.Name));

				trainInst.SetSyncer(newSyncerPlayer,
				(GTANetworkAPI.Player oldSyncer) =>
				{
					CPlayer oldSyncerPlayer = PlayerPool.GetPlayerFromClient(oldSyncer).Instance();
					if (oldSyncerPlayer != null)
					{
						NetworkEventSender.SendNetworkEvent_TrainSync_TakeOwnership(oldSyncerPlayer, trainInst.ID);
					}
				},
				(GTANetworkAPI.Player newSyncer) =>
				{
					CPlayer oldSyncerPlayer = PlayerPool.GetPlayerFromClient(newSyncer).Instance();
					if (oldSyncerPlayer != null)
					{
						NetworkEventSender.SendNetworkEvent_TrainSync_GiveOwnership(oldSyncerPlayer, trainInst.ID);
					}
				});
			}
		}
	}
	// TODO_TRAIN: client only send snyc if speed or pos changed
	private void CreateTrain(ETrainType trainType, int tripwireID, int Sector, Vector3 vecPos, int ID)
	{
		CServerTrainInstance newTrain = new CServerTrainInstance(trainType, tripwireID, Sector, vecPos, ID);
		m_lstTrains.Add(newTrain);

		// send to all
		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			newTrain.Transmit(player);
		}
	}

	public void OnPlayerConnected(CPlayer a_Player)
	{
		// Send all instances
		foreach (CServerTrainInstance trainInst in m_lstTrains)
		{
			trainInst.Transmit(a_Player);
		}
	}
}