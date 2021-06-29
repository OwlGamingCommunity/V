using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;

public class CServerTrainInstance
{
	// TODO_TRAIN: Reset this when we reach 'end' of the track
	// TODO_TRAIN: Have to sync this probably
	public int LastTripWireID { get; set; } = -1;
	public int CurrentSector { get; set; } = -1;
	// TODO_TRAIN: When server receives sync, update these values
	public float Speed { get; set; } = 0.0f;

	public ETrainType TrainType { get; set; }
	public Vector3 Position { get; set; }
	public int ID { get; set; }
	public bool AreDoorsOpen { get; set; } = false;

	private WeakReference<GTANetworkAPI.Player> Syncer = new WeakReference<GTANetworkAPI.Player>(null);

	public CServerTrainInstance(ETrainType a_TrainType, int tripwireID, int Sector, Vector3 a_vecPos, int a_ID)
	{
		TrainType = a_TrainType;
		Position = a_vecPos;
		ID = a_ID;

		LastTripWireID = tripwireID;
		CurrentSector = Sector;
	}

	public void OnTrainDoorStateChanged(bool bDoorsOpen)
	{
		AreDoorsOpen = bDoorsOpen;
	}

	public void OnSyncArrived(Vector3 vecPos, float fSpeed, int tripwireID, int currentSector)
	{
		Position = vecPos;
		Speed = fSpeed;
		LastTripWireID = tripwireID;
		CurrentSector = currentSector;
	}

	public void Transmit(CPlayer player)
	{
		NetworkEventSender.SendNetworkEvent_CreateSyncedTrain(player, ID, TrainType, Position, CurrentSector, LastTripWireID, Speed, AreDoorsOpen);
	}

	private Dictionary<EVehicleSeat, GTANetworkAPI.Player> m_dictOccupants = new Dictionary<EVehicleSeat, GTANetworkAPI.Player>();
	private const EVehicleSeat MaxTrainSeat = EVehicleSeat.RearRightPassenger;

	public void AddOccupant(GTANetworkAPI.Player player, EVehicleSeat seat)
	{
		m_dictOccupants[seat] = player;
	}

	public void RemoveOccupant(GTANetworkAPI.Player player)
	{
		if (m_dictOccupants.ContainsValue(player))
		{
			var occupant = m_dictOccupants.First(kvPair => kvPair.Value == player);
			m_dictOccupants.Remove(occupant.Key);
		}
	}

	public bool GetNextFreePassengerSeat(out EVehicleSeat freeSeat)
	{
		freeSeat = EVehicleSeat.Driver;

		for (EVehicleSeat seat = EVehicleSeat.FrontPassenger; seat <= MaxTrainSeat; ++seat)
		{
			if (!m_dictOccupants.ContainsKey(seat))
			{
				freeSeat = seat;
				return true;
			}
		}

		freeSeat = EVehicleSeat.Driver;
		return false;
	}

	public bool GetOccupant(EVehicleSeat seat, out GTANetworkAPI.Player occupant)
	{
		if (m_dictOccupants.ContainsKey(seat))
		{
			occupant = m_dictOccupants[seat];
			return true;
		}

		occupant = null;
		return false;
	}

	public GTANetworkAPI.Player GetSyncer()
	{
		GTANetworkAPI.Player player = null;
		if (Syncer.TryGetTarget(out player))
		{
			return player;
		}

		return null;
	}

	public void SetSyncer(GTANetworkAPI.Player a_Player, Action<GTANetworkAPI.Player> ActionOnCurrentSyncerIfExists, Action<GTANetworkAPI.Player> ActionOnNewSyncerIfExists)
	{
		GTANetworkAPI.Player currentSyncer = GetSyncer();
		if (currentSyncer != null)
		{
			ActionOnCurrentSyncerIfExists(currentSyncer);
		}

		Syncer = new WeakReference<GTANetworkAPI.Player>(a_Player);

		if (a_Player != null)
		{
			ActionOnNewSyncerIfExists(a_Player);
		}
	}
}