using RAGE;
using System;
using System.Collections.Generic;

public class CClientTrainInstance
{
	public int LastTripWireID { get; set; } = -1;
	public int CurrentSector { get; set; } = -1;

	public CClientTrainInstance(ETrainType a_TrainType, int tripwireID, int Sector, Vector3 a_vecPos, int a_ID, float a_Speed, bool bDoorsOpen)
	{
		TrainType = a_TrainType;
		Position = a_vecPos;
		ID = a_ID;
		Speed = a_Speed;
		AreDoorsOpen = bDoorsOpen;

		LastTripWireID = tripwireID;
		CurrentSector = Sector;
	}

	private DateTime m_LastPacketTime = DateTime.Now;
	RAGE.Vector3 m_vecStartPos = new RAGE.Vector3();
	RAGE.Vector3 m_vecInterpPos = new RAGE.Vector3();
	RAGE.Vector3 m_vecTargetPos = new RAGE.Vector3();
	private bool m_bFakeDestroyed = false;
	public void UpdateSync(RAGE.Vector3 vecTargetPos, float fSpeed, int tripwireID, int currentSector)
	{
		UpdateLastPacketTime();

		LastTripWireID = tripwireID;
		CurrentSector = currentSector;

		// only used for train moving state detection
		Speed = fSpeed;

		// wrap straight to last target pos
		RAGE.Game.Vehicle.SetMissionTrainCoords(m_Handle, m_vecTargetPos.X, m_vecTargetPos.Y, m_vecTargetPos.Z);

		// update last pos and interp pos to be the last target pos
		m_vecStartPos = m_vecTargetPos.CopyVector();
		m_vecInterpPos = m_vecTargetPos.CopyVector();

		// set target to new pos
		m_vecTargetPos = vecTargetPos;
	}

	public bool IsLastPacketTimeConsideredExpired()
	{
		return (DateTime.Now - m_LastPacketTime).TotalSeconds >= 5;
	}

	private bool IsFakeDestroyed()
	{
		return m_bFakeDestroyed;
	}

	public void UpdateLastPacketTime()
	{
		// if we got a packet and were destroyed, re-create
		if (IsFakeDestroyed())
		{
			FakeRecreate();
		}

		m_LastPacketTime = DateTime.Now;
	}

	public void Interp()
	{
		const float PureSyncTimer = 1000 / TrainConstants.PureSyncUpdatesPerSecond;
		float TimeSinceLastPacket = (float)(DateTime.Now - m_LastPacketTime).TotalMilliseconds;
		float fRatio = TimeSinceLastPacket / PureSyncTimer;

		RAGE.Vector3 vecDist = m_vecTargetPos - m_vecStartPos;
		RAGE.Vector3 vecDistToTravelThisFrame = (vecDist * fRatio);
		m_vecInterpPos = m_vecStartPos + vecDistToTravelThisFrame;

		RAGE.Game.Vehicle.SetMissionTrainCoords(m_Handle, m_vecInterpPos.X, m_vecInterpPos.Y, m_vecInterpPos.Z);

		//ChatHelper.DebugMessage("{0} INTERP TO: {1},{2},{3} ({4}", ID, m_vecInterpPos.X, m_vecInterpPos.Y, m_vecInterpPos.Z, fRatio);

		//ENTITY::SET_ENTITY_COORDS(m_GTAHandle, m_vecPosition.x, m_vecPosition.y, m_vecPosition.z, 1, 0, 0, 1);
	}

	public Dictionary<RAGE.Elements.Player, EVehicleSeat> m_dictOccupants = new Dictionary<RAGE.Elements.Player, EVehicleSeat>();
	public Dictionary<RAGE.Elements.Player, RAGE.Elements.Ped> m_dictClones = new Dictionary<RAGE.Elements.Player, RAGE.Elements.Ped>();
	public RAGE.Elements.Ped AddOccupant(RAGE.Elements.Player player, EVehicleSeat seat)
	{
		m_dictOccupants[player] = seat;
		return CreateClone(player, seat);
	}

	public void RemoveOccupant(RAGE.Elements.Player player)
	{
		m_dictOccupants.Remove(player);
		DestroyClone(player);
	}

	public void UpdateOccupants()
	{
		foreach (var kvPair in m_dictOccupants)
		{
			//kvPair.Key.SetIntoVehicle(m_Handle, ((int)kvPair.Value) - 1);
		}
	}

	public bool HasHumanDriver()
	{
		return m_dictOccupants.ContainsValue(EVehicleSeat.Driver);
	}


	private RAGE.Elements.Ped CreateClone(RAGE.Elements.Player player, EVehicleSeat seat)
	{
		if (!m_dictClones.ContainsKey(player))
		{
			//ChatHelper.DebugMessage("Create Clone: {0}", player.Name);

			RAGE.Elements.Ped dummyPed = new RAGE.Elements.Ped(player.Model, player.Position, player.GetRotation(0).Z, player.Dimension);
			dummyPed.SetIntoVehicle(m_Handle, (int)seat - 1);
			m_dictClones.Add(player, dummyPed);

			// copy components
			foreach (ECustomClothingComponent component in Enum.GetValues(typeof(ECustomClothingComponent)))
			{
				dummyPed.SetComponentVariation((int)component, player.GetDrawableVariation((int)component), player.GetTextureVariation((int)component), player.GetPaletteVariation((int)component));
			}

			// copy props
			foreach (ECustomPropSlot prop in Enum.GetValues(typeof(ECustomPropSlot)))
			{
				dummyPed.SetPropIndex((int)prop, player.GetPropIndex((int)prop), player.GetPropTextureIndex((int)prop), true);
			}

			// TODO_TRAIN: Do we want to copy face visuals etc? probably
			return dummyPed;
		}
		else
		{
			return m_dictClones[player];
		}
	}

	private void DestroyClone(RAGE.Elements.Player player)
	{
		if (m_dictClones.ContainsKey(player))
		{
			//ChatHelper.DebugMessage("Destroy Clone: {0}", player.Name);

			RAGE.Elements.Ped dummyPed = m_dictClones[player];
			dummyPed.Destroy();
			m_dictClones.Remove(player);
		}
	}

	public void OnPlayerStreamIn(RAGE.Elements.Player player)
	{
		// is the person in the train?
		if (m_dictOccupants.ContainsKey(player))
		{
			// TODO_TRAIN: Create/Destroy clone needs to handle alpha, it currently only gets applied if the person is streamed in and enters a train
			CreateClone(player, m_dictOccupants[player]);
		}
	}

	public void OnPlayerStreamOut(RAGE.Elements.Player player)
	{
		// is the person in the train?
		if (m_dictOccupants.ContainsKey(player))
		{
			DestroyClone(player);
		}
	}

	public float Speed { get; set; } = 0.0f;

	public void SyncGTAInstanceWithSpeed()
	{
		RAGE.Game.Vehicle.SetTrainCruiseSpeed(m_Handle, Speed);
	}

	public void SyncGTAInstanceWithPosition()
	{
		RAGE.Game.Vehicle.SetMissionTrainCoords(m_Handle, Position.X, Position.Y, Position.Z);
	}

	public void SetDoorsOpen(bool bDoorsOpen)
	{
		// Are we the syncer? and has the state changed?
		if (IsLocallySynced)
		{
			if (bDoorsOpen != AreDoorsOpen)
			{
				NetworkEventSender.SendNetworkEvent_TrainDoorStateChanged(ID, bDoorsOpen);
			}
		}

		AreDoorsOpen = bDoorsOpen;

		// Apply to GTA vehicle
		int handleCarriage0 = RAGE.Game.Vehicle.GetTrainCarriage(GetHandle(), 0);
		int handleCarriage1 = RAGE.Game.Vehicle.GetTrainCarriage(GetHandle(), 1);

		for (int i = 0; i < 8; ++i)
		{
			if (bDoorsOpen)
			{
				RAGE.Game.Vehicle.SetVehicleDoorOpen(handleCarriage0, i, false, false);
				RAGE.Game.Vehicle.SetVehicleDoorOpen(handleCarriage1, i, false, false);
			}
			else
			{
				RAGE.Game.Vehicle.SetVehicleDoorShut(handleCarriage0, i, false);
				RAGE.Game.Vehicle.SetVehicleDoorShut(handleCarriage1, i, false);
			}
		}
	}

	public ETrainType TrainType { get; set; }
	public Vector3 Position { get; set; }
	public int ID { get; set; }
	public bool AreDoorsOpen { get; set; } = false;

	private int m_Handle = -1;
	public bool IsLocallySynced { get; set; } = false;

	public int GetHandle()
	{
		return m_Handle;
	}

	public Vector3 GetGTAPosition()
	{
		RAGE.Vector3 vecPos = RAGE.Game.Entity.GetEntityCoords(m_Handle, false);
		Position = vecPos;
		return vecPos;
	}

	public void Create()
	{
		if (m_Handle == -1)
		{
			m_Handle = RAGE.Game.Vehicle.CreateMissionTrain(24, Position.X, Position.Y, Position.Z, true);

			// apply speed + door state on creation
			SyncGTAInstanceWithSpeed();
			SetDoorsOpen(AreDoorsOpen);
		}
	}

	public void FakeRecreate()
	{
		if (IsFakeDestroyed())
		{
			m_bFakeDestroyed = false;

			// TODO_TRAIN: will it get speed etc again: It doesnt, we have to re-apply on recreate

			// hacky, but delete doesn't work...
			RAGE.Game.Vehicle.SetTrainCruiseSpeed(m_Handle, 0.0f);
			RAGE.Game.Vehicle.SetTrainSpeed(m_Handle, 0.0f);

			int handleCarriage0 = RAGE.Game.Vehicle.GetTrainCarriage(m_Handle, 0);
			int handleCarriage1 = RAGE.Game.Vehicle.GetTrainCarriage(m_Handle, 1);

			RAGE.Game.Entity.SetEntityAlpha(handleCarriage0, 255, false);
			RAGE.Game.Entity.SetEntityAlpha(handleCarriage1, 255, false);
			RAGE.Game.Entity.SetEntityCollision(handleCarriage0, true, true);
			RAGE.Game.Entity.SetEntityCollision(handleCarriage1, true, true);
		}
	}

	public void FakeDestroy()
	{
		// fake means we will recreate if a packet arrives
		if (!IsFakeDestroyed())
		{
			m_bFakeDestroyed = true;

			Destroy();
		}
	}

	public void Destroy()
	{
		// hacky, but delete doesn't work...
		RAGE.Game.Vehicle.SetTrainCruiseSpeed(m_Handle, 0.0f);
		RAGE.Game.Vehicle.SetTrainSpeed(m_Handle, 0.0f);

		int handleCarriage0 = RAGE.Game.Vehicle.GetTrainCarriage(m_Handle, 0);
		int handleCarriage1 = RAGE.Game.Vehicle.GetTrainCarriage(m_Handle, 1);

		RAGE.Game.Entity.SetEntityAlpha(m_Handle, 0, false);
		RAGE.Game.Entity.SetEntityAlpha(handleCarriage0, 0, false);
		RAGE.Game.Entity.SetEntityAlpha(handleCarriage1, 0, false);
		RAGE.Game.Entity.SetEntityCollision(m_Handle, false, false);
		RAGE.Game.Entity.SetEntityCollision(handleCarriage0, false, false);
		RAGE.Game.Entity.SetEntityCollision(handleCarriage1, false, false);
	}
}