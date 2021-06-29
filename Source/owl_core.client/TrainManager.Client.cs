using RAGE;
using System;
using System.Collections.Generic;

public class TrainManager
{
	private WeakReference<CClientTrainInstance> m_TrainBeingDriven = new WeakReference<CClientTrainInstance>(null);
	private WeakReference<CClientTrainInstance> m_TrainBeingRode = new WeakReference<CClientTrainInstance>(null);
	private List<CClientTrainInstance> m_lstTrains = new List<CClientTrainInstance>();

	public TrainManager()
	{
		// keep the train model in memory
		uint model_MetroTrain = 868868440;
		AsyncModelLoader.RequestSyncInstantLoad(model_MetroTrain);

		NetworkEvents.CreateSyncedTrain += OnCreateSyncedTrain;
		NetworkEvents.DestroySyncedTrain += OnDestroySyncedTrain;

		NetworkEvents.TrainSync_GiveOwnership += OnGiveOwnership;
		NetworkEvents.TrainSync_TakeOwnership += OnRemoveOwnership;

		ClientTimerPool.CreateTimer(SendSync, 200);

		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
		RageEvents.RAGE_OnEntityStreamOut += OnStreamOut;

		RageEvents.RAGE_OnRender += OnRender;

		RageEvents.RAGE_OnTick_LowFrequency += CheckTripwires;

		NetworkEvents.TrainEnter_Approved += OnTrainEnter_Approved;
		NetworkEvents.TrainExit_Approved += OnTrainExit_Approved;

		NetworkEvents.TrainSync_Ack += OnTrainSyncAck;
		NetworkEvents.TrainSync += OnTrainSync;
		NetworkEvents.TrainDoorStateChanged += OnTrainDoorStateChanged;

		RageEvents.RAGE_OnTick_PerFrame += UpdateInterpolation;

		// Controls for driver
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleTrainDoors, ToggleTrainDoors);
		ScriptControls.SubscribeToControl(EScriptControlID.TrainAccelerate, AccelerateTrain);
		ScriptControls.SubscribeToControl(EScriptControlID.TrainDecelerate, DecelerateTrain);

		KeyBinds.Bind(ConsoleKey.Insert, () =>
		{
			foreach (CClientTrainInstance trainInst in m_lstTrains)
			{
				trainInst.FakeDestroy();
			}
		}, EKeyBindType.Released, EKeyBindFlag.Default);

		// TODO_TRAIN: Block normal vehicle leave/enter for train

		// ai start drive test
		KeyBinds.Bind(ConsoleKey.Delete, () =>
		{
			foreach (CClientTrainInstance trainInst in m_lstTrains)
			{
				trainInst.Speed = 30.0f;
				trainInst.SyncGTAInstanceWithSpeed();
			}
		}, EKeyBindType.Released, EKeyBindFlag.Default);

		KeyBinds.Bind(ConsoleKey.Enter, () =>
		{
			try { throw new Exception(Helpers.FormatString("new Vector3({0}f, {1}f, {2}f)", RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z)); } catch { }
		}, EKeyBindType.Released, EKeyBindFlag.Default);
	}

	private void ToggleTrainDoors(EControlActionType actionType)
	{
		if (IsLocalPlayerDrivingATrain())
		{
			// TODO_TRAIN: Do we need a cooldown to stop the driver spamming it? atleast a couple of seconds
			m_TrainBeingDriven.Instance().SetDoorsOpen(!m_TrainBeingDriven.Instance().AreDoorsOpen);
		}
	}

	private void AccelerateTrain(EControlActionType actionType)
	{
		if (IsLocalPlayerDrivingATrain())
		{
			if (m_TrainBeingDriven.Instance().Speed + TrainConstants.SpeedIncrements <= TrainConstants.MaxSpeed)
			{
				m_TrainBeingDriven.Instance().Speed += TrainConstants.SpeedIncrements;
				m_TrainBeingDriven.Instance().SyncGTAInstanceWithSpeed();
			}
		}
	}

	private void DecelerateTrain(EControlActionType actionType)
	{
		if (IsLocalPlayerDrivingATrain())
		{
			if (m_TrainBeingDriven.Instance().Speed - TrainConstants.SpeedIncrements >= -(TrainConstants.MaxSpeed))
			{
				m_TrainBeingDriven.Instance().Speed -= TrainConstants.SpeedIncrements;
				m_TrainBeingDriven.Instance().SyncGTAInstanceWithSpeed();
			}
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			foreach (CClientTrainInstance trainInst in m_lstTrains)
			{
				trainInst.OnPlayerStreamIn((RAGE.Elements.Player)entity);
			}
		}
	}

	private void OnStreamOut(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			foreach (CClientTrainInstance trainInst in m_lstTrains)
			{
				trainInst.OnPlayerStreamOut((RAGE.Elements.Player)entity);
			}
		}
	}
	// TODO_TRAIN: add driver cams like a train sim
	private void OnTrainEnter_Approved(RAGE.Elements.Player player, int trainID, EVehicleSeat seat)
	{
		CClientTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			// always do the warp process
			RAGE.Elements.Ped dummyPed = trainInst.AddOccupant(player, seat);

			ChatHelper.ErrorMessage("If you see this error, tell Daniels or Vubstersmurf in Discord.");
			RAGE.Game.Entity.AttachEntityToEntity(player.Handle, dummyPed.Handle, -1, 0.0f, 0.0f, 2.0f, 0.0f, 0.0f, 0.0f, true, true, false, false, 0, true);
			player.SetAlpha(0, false);

			if (player == RAGE.Elements.Player.LocalPlayer)
			{
				if (seat == EVehicleSeat.Driver)
				{
					m_TrainBeingDriven.SetTarget(trainInst);
				}
				else
				{
					m_TrainBeingRode.SetTarget(trainInst);
				}
			}

			/*
			if (seat == EVehicleSeat.Driver)
			{
				ChatHelper.DebugMessage("Set train being driven ({0})", player.Name);
			}
			else
			{
				ChatHelper.DebugMessage("Set train being rode as passenger ({0})", player.Name);
			}
			*/
		}
	}

	private void OnTrainExit_Approved(RAGE.Elements.Player player, int trainID)
	{
		CClientTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			// always do the warp process
			trainInst.RemoveOccupant(player);

			RAGE.Game.Entity.DetachEntity(player.Handle, true, true);
			player.SetAlpha(255, false);

			CTrainTripWire lastTripWire = TrainConstants.TripWires[trainInst.LastTripWireID];

			if (lastTripWire != null)
			{
				player.Position = lastTripWire.ExitPosition;
			}

			// is it local?
			if (player == RAGE.Elements.Player.LocalPlayer)
			{
				// this is local state
				m_TrainBeingDriven.SetTarget(null);
				m_TrainBeingRode.SetTarget(null);
			}

			// was it the driver?
			if (!trainInst.HasHumanDriver())
			{
				// apply AI drive
				ApplyAILogicForTripwire(trainInst);
			}
		}
	}

	private void UpdateInterpolation()
	{
		foreach (CClientTrainInstance trainInst in m_lstTrains)
		{
			if (!trainInst.IsLocallySynced)
			{
				trainInst.Interp();
			}

			trainInst.UpdateOccupants();
		}
	}

	private void OnTrainSyncAck(int trainID)
	{
		CClientTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			// Fake update last packet time as we use this for timeout based destruction
			trainInst.UpdateLastPacketTime();
		}
	}

	private void OnTrainDoorStateChanged(int trainID, bool bDoorsOpen)
	{
		CClientTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			trainInst.SetDoorsOpen(bDoorsOpen);
		}
	}

	private void OnTrainSync(int trainID, float x, float y, float z, float speed, int tripwireID, int currentSector)
	{
		CClientTrainInstance trainInst = GetTrainFromID(trainID);
		if (trainInst != null)
		{
			trainInst.UpdateSync(new RAGE.Vector3(x, y, z), speed, tripwireID, currentSector);

			// update the shared 
			ApplyTripwireSharedLogic(trainInst, tripwireID, TrainConstants.TripWires[tripwireID]);
		}
	}

	private CClientTrainInstance GetTrainFromID(int ID)
	{
		foreach (CClientTrainInstance trainInst in m_lstTrains)
		{
			if (trainInst.ID == ID)
			{
				return trainInst;
			}
		}

		return null;
	}

	private void PushTrainAnnouncerMessage(CClientTrainInstance trainInst, bool bOnlyIfInside, bool bNearbyOnly, string strMessage, params string[] parameters)
	{
		if (bOnlyIfInside)
		{
			if (trainInst != m_TrainBeingRode.Instance() && trainInst != m_TrainBeingDriven.Instance())
			{
				return;
			}
		}

		if (bNearbyOnly)
		{
			float fDistance = WorldHelper.GetDistance2D(RAGE.Elements.Player.LocalPlayer.Position, trainInst.GetGTAPosition());
			if (fDistance > 10.0f)
			{
				return;
			}
		}

		strMessage = "[TRAIN ANNOUNCEMENT] " + strMessage;
		ChatHelper.PushMessage(EChatChannel.Nearby, strMessage, parameters);
	}

	private CTrainTripWire GetNextStop(int currentStop)
	{
		for (int i = currentStop + 1; i < TrainConstants.TripWires.Count; ++i)
		{
			CTrainTripWire tripWire = TrainConstants.TripWires[i];
			if (tripWire.TripWireType == ETrainTripWireType.StationStop)
			{
				return tripWire;
			}
		}

		// if we get here, we got to the end, meaning the next stop is wrapped around, so return first station
		foreach (CTrainTripWire tripWire in TrainConstants.TripWires)
		{
			if (tripWire.TripWireType == ETrainTripWireType.StationStop)
			{
				return tripWire;
			}
		}

		return null;
	}

	// NOTE: Triggered once when the tripwire changes / is hit for the first time
	private void ApplyTripwireSharedLogic(CClientTrainInstance trainInst, int tripwireID, CTrainTripWire tripWire)
	{
		if (tripWire.TripWireType == ETrainTripWireType.ApproachingStation)
		{
			PushTrainAnnouncerMessage(trainInst, true, false, "We are now approaching {0}.", tripWire.Name);
		}
		else if (tripWire.TripWireType == ETrainTripWireType.StationStop)
		{
			// find next stop
			CTrainTripWire nextStop = GetNextStop(tripwireID);
			PushTrainAnnouncerMessage(trainInst, false, true, "This stop is {0}. This train is a {1} line train. The next stop is {2}.", tripWire.Name, tripWire.Direction, nextStop.Name);
		}
	}


	// NOTE: Don't do any shared logic here (e.g. announcements) this is ONLY for syncer logic
	private void CheckTripwires()
	{
		foreach (CClientTrainInstance trainInst in m_lstTrains)
		{
			if (trainInst.IsLocallySynced)
			{
				RAGE.Vector3 vecCachedTrainPos = trainInst.GetGTAPosition();

				int tripwireID = 0;
				foreach (CTrainTripWire tripWire in TrainConstants.TripWires)
				{
					float fDist = WorldHelper.GetDistance2D(tripWire.Position, vecCachedTrainPos);

					if (fDist <= TrainConstants.TripWireThreshold)
					{
						// did we already trigger it?
						if (trainInst.LastTripWireID < tripwireID)
						{
							//ChatHelper.ErrorMessage("HIT TRIPWIRE {0} ({1})", tripwireID, tripWire.Name);

							ApplyTripwireSharedLogic(trainInst, tripwireID, tripWire);

							trainInst.LastTripWireID = tripwireID;
							trainInst.CurrentSector = tripWire.Sector;

							ApplyAILogicForTripwire(trainInst);

							// is it the end? reset in 10 sec
							if (tripwireID == TrainConstants.TripWires.Count - 1)
							{
								ClientTimerPool.CreateTimer((object[] parameters) =>
								{
									trainInst.LastTripWireID = 0;
									trainInst.CurrentSector = 0;
								}, 10000, 1);
							}

							break;
						}
					}

					++tripwireID;
				}
			}
		}
	}

	private void ApplyAILogicForTripwire(CClientTrainInstance trainInst)
	{
		if (trainInst.HasHumanDriver() || !trainInst.IsLocallySynced)
		{
			return;
		}

		CTrainTripWire lastTripWire = TrainConstants.TripWires[trainInst.LastTripWireID];
		if (lastTripWire == null)
		{
			return;
		}

		// TODO_TRAIN: We need to manage the transition between human and AI controlled, e.g when driver leaves
		if (lastTripWire.TripWireType == ETrainTripWireType.ApplyBrakes)
		{
			// TODO_TRAIN: kill timers when a human takes over
			if (!trainInst.HasHumanDriver())
			{
				trainInst.Speed = 0.0f;
				trainInst.SyncGTAInstanceWithSpeed();
			}

			// TODO_TRAIN: Allow players to open doors etc if driver
		}
		else if (lastTripWire.TripWireType == ETrainTripWireType.ApproachingStation)
		{
			// TODO_TRAIN: Right now signals are only tied to stations, do we want more elsewhere?

			// are we about to collide?
			// TODO_TRAIN: This logic needs to run for human players too, minus the else
			ETrainSignalState signalState = GetSignalState(out int currentSector, out int nextSector, out int nextSectorPlusOne);
			if (signalState == ETrainSignalState.RedEmergency)
			{
				// TODO_TRAIN: Should these noises play for passengers too?
				// TODO_TRAIN: Should ping for yellow signals, driver should be able to acknowledge yellow signals
				//ChatHelper.DebugMessage("PASSED RED SIGNAL, APPLYING EMERGENCY BRAKE");
				trainInst.Speed = 0.0f;
				trainInst.SyncGTAInstanceWithSpeed();
				RAGE.Game.Audio.PlaySoundFrontend(-1, "Beep_Green", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
			}
			else
			{
				trainInst.Speed = 30.0f;
				trainInst.SyncGTAInstanceWithSpeed();
			}
		}
		else if (lastTripWire.TripWireType == ETrainTripWireType.StationStop)
		{
			ClientTimerPool.CreateTimer((object[] parameters) =>
			{
				trainInst.SetDoorsOpen(false);
			}, 1000, 1);

			// start accelerating again in 10 sec
			ClientTimerPool.CreateTimer((object[] parameters) =>
			{
				trainInst.Speed = 30.0f;
				trainInst.SyncGTAInstanceWithSpeed();

				trainInst.SetDoorsOpen(false);
			}, 20000, 1);
		}
	}

	enum ETrainSignalState
	{
		None,
		Green,
		YellowSingle_RedNext,
		YellowDouble_YellowNext,
		RedEmergency
	}

	private ETrainSignalState GetSignalState(out int currentSector, out int nextSector, out int nextSectorPlusOne)
	{
		ETrainSignalState state = ETrainSignalState.None;
		currentSector = -1;
		nextSector = -1;
		nextSectorPlusOne = -1;

		if (IsLocalPlayerDrivingATrain())
		{
			state = ETrainSignalState.Green;

			CClientTrainInstance myTrain = m_TrainBeingDriven.Instance();
			currentSector = myTrain.CurrentSector;
			int maxSector = TrainConstants.TripWires[TrainConstants.TripWires.Count - 1].Sector;
			nextSector = (currentSector == maxSector) ? 0 : currentSector + 1; // wrap around if we are in max
			nextSectorPlusOne = (currentSector + 1 == maxSector) ? 1 : currentSector + 2; // wrap around if we are in max

			// is there a train in the segment ahead
			foreach (CClientTrainInstance trainInst in m_lstTrains)
			{
				if (trainInst != myTrain)
				{
					if (trainInst.CurrentSector == currentSector)
					{
						state = ETrainSignalState.RedEmergency;
						break; // break here since this is the NEAREST sector in the worst state possible
					}
					else if (trainInst.CurrentSector == nextSector)
					{
						state = ETrainSignalState.YellowSingle_RedNext;
						// no break because red current sector would be worse
					}
					else if (trainInst.CurrentSector == nextSectorPlusOne) // train 2 sectors ahead
					{
						state = ETrainSignalState.YellowDouble_YellowNext;
					}
				}
			}
		}

		return state;
	}

	private bool IsLocalPlayerDrivingATrain()
	{
		return m_TrainBeingDriven.Instance() != null;
	}

	private bool IsLocalPlayerRidingATrain()
	{
		return m_TrainBeingRode.Instance() != null;
	}

	private bool IsLocalPlayerInAnyTrain()
	{
		return IsLocalPlayerDrivingATrain() || IsLocalPlayerRidingATrain();
	}

	private CClientTrainInstance GetTrainInstanceFromLocalPlayer()
	{
		if (IsLocalPlayerDrivingATrain())
		{
			return m_TrainBeingDriven.Instance();
		}
		else if (IsLocalPlayerRidingATrain())
		{
			return m_TrainBeingRode.Instance();
		}

		return null;
	}

	private void OnRender()
	{
		// cleanup any expired trains (this is hacky)
		foreach (CClientTrainInstance trainInst in m_lstTrains)
		{
			if (trainInst.IsLastPacketTimeConsideredExpired())
			{
				trainInst.FakeDestroy();
			}
		}

		// TODO_TRAIN: Nametags will be off because of player fake pos
		// TODO_TRAIN: Proper UI for drivers
		// are we driving? draw signal
		int r = 0;
		int g = 255;
		int b = 0;

		if (IsLocalPlayerDrivingATrain())
		{
			CClientTrainInstance myTrain = m_TrainBeingDriven.Instance();
			int mySector = myTrain.CurrentSector;
			bool bDrawDouble = false;

			ETrainSignalState signalState = GetSignalState(out int currentSector, out int nextSector, out int nextSectorPlusOne);
			if (signalState != ETrainSignalState.None)
			{
				if (signalState == ETrainSignalState.YellowDouble_YellowNext || signalState == ETrainSignalState.YellowSingle_RedNext)
				{
					bDrawDouble = true;
					r = 255;
					g = 255;
					b = 0;
				}
				else if (signalState == ETrainSignalState.RedEmergency)
				{
					r = 255;
					g = 0;
					b = 0;
				}

				if (signalState == ETrainSignalState.RedEmergency)
				{
					TextHelper.Draw2D("APPLYING EMERGENCY BRAKE!", 0.8f, 0.3f, 0.4f, new RAGE.RGBA(255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, false, false);
				}

				TextHelper.Draw2D(Helpers.FormatString("Current Speed: {0}", myTrain.Speed), 0.8f, 0.35f, 0.4f, new RAGE.RGBA(255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, false, false);
				TextHelper.Draw2D(Helpers.FormatString("Current Sector: {0}", mySector), 0.8f, 0.4f, 0.4f, new RAGE.RGBA(255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, false, false);
				TextHelper.Draw2D(Helpers.FormatString("Next Signal: {0}", signalState), 0.8f, 0.45f, 0.4f, new RAGE.RGBA(255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, false, false);
				RAGE.Game.Graphics.DrawRect(0.8f, 0.5f, 0.1f, 0.1f, r, g, b, 200, 0);

				if (bDrawDouble)
				{
					RAGE.Game.Graphics.DrawRect(0.8f, 0.7f, 0.1f, 0.1f, r, g, b, 200, 0);
				}
			}
		}

		// TODO_TRAIN: Because we warp clientside only, server doesnt know the true position of the player...

		if (IsLocalPlayerInAnyTrain())
		{
			CClientTrainInstance trainInst = GetTrainInstanceFromLocalPlayer();

			string strMessage = "";

			// can only leave if the train isnt moving
			if (trainInst.Speed == 0.0f)
			{
				if (trainInst.LastTripWireID != -1)
				{
					// are we in a station?
					CTrainTripWire lastTripWire = TrainConstants.TripWires[trainInst.LastTripWireID];
					if (lastTripWire.TripWireType == ETrainTripWireType.StationStop)
					{
						float fDistanceToStationTop = WorldHelper.GetDistance2D(trainInst.GetGTAPosition(), lastTripWire.Position);
						const float fStationDistanceThreshold = 40.0f;

						if (fDistanceToStationTop > fStationDistanceThreshold)
						{
							strMessage = "You cannot leave the train until it reaches a station.";
						}
						else
						{
							strMessage = Helpers.FormatString("Press {0} to exit train.", ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact).ToString());

							if (KeyBinds.WasKeyJustReleased(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact)))
							{
								if (lastTripWire.ExitPosition != null)
								{
									// event so we can remote dummy for remotes too
									NetworkEventSender.SendNetworkEvent_TrainExit(trainInst.ID);
								}
							}
						}
					}
					else
					{
						strMessage = "You cannot leave the train until it reaches a station.";
					}
				}
			}
			else
			{
				strMessage = "You cannot leave the train while it is moving";
			}

			TextHelper.Draw2D(strMessage, 0.50f, 0.92f, 0.3f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
		}
		else
		{
			foreach (CClientTrainInstance trainInst in m_lstTrains)
			{
				const float fDistThresholdDriver = 2.5f;
				const float fDistThresholdPassenger = 5.0f;
				float fDistance = WorldHelper.GetDistance2D(trainInst.Position, RAGE.Elements.Player.LocalPlayer.Position);

				if (fDistance <= fDistThresholdPassenger || fDistance <= fDistThresholdDriver)
				{
					RAGE.Vector3 vecPosDriverSeat = RAGE.Game.Entity.GetWorldPositionOfEntityBone(trainInst.GetHandle(), RAGE.Game.Entity.GetEntityBoneIndexByName(trainInst.GetHandle(), "seat_dside_f"));

					int handleCarriage0 = RAGE.Game.Vehicle.GetTrainCarriage(trainInst.GetHandle(), 0);
					int handleCarriage1 = RAGE.Game.Vehicle.GetTrainCarriage(trainInst.GetHandle(), 1);

					List<RAGE.Vector3> vecPassengerCoords = new List<RAGE.Vector3>();
					vecPassengerCoords.Add(RAGE.Game.Entity.GetWorldPositionOfEntityBone(handleCarriage0, RAGE.Game.Entity.GetEntityBoneIndexByName(trainInst.GetHandle(), "door_dside_f")));
					vecPassengerCoords.Add(RAGE.Game.Entity.GetWorldPositionOfEntityBone(handleCarriage0, RAGE.Game.Entity.GetEntityBoneIndexByName(trainInst.GetHandle(), "door_dside_r")));
					vecPassengerCoords.Add(RAGE.Game.Entity.GetWorldPositionOfEntityBone(handleCarriage1, RAGE.Game.Entity.GetEntityBoneIndexByName(trainInst.GetHandle(), "door_pside_f")));
					vecPassengerCoords.Add(RAGE.Game.Entity.GetWorldPositionOfEntityBone(handleCarriage1, RAGE.Game.Entity.GetEntityBoneIndexByName(trainInst.GetHandle(), "door_pside_r")));

					// TODO_TRAIN: Only for head admins at the moment
					float fDistToDriverPos = WorldHelper.GetDistance2D(RAGE.Elements.Player.LocalPlayer.Position, vecPosDriverSeat);
					bool bDrawnDriverHint = false;

					EAdminLevel adminLevel = DataHelper.GetLocalPlayerEntityData<EAdminLevel>(EDataNames.ADMIN_LEVEL);
					//if (adminLevel == EAdminLevel.HeadAdmin)
					{
						WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Drive Train (Head Admin Only)", null, () => { WorldInteraction_DriveTrain(trainInst); }, vecPosDriverSeat, 0, false, false, fDistThresholdDriver);
						bDrawnDriverHint = fDistToDriverPos <= fDistThresholdDriver;
					}

					// prioritize drive over ride
					if (!bDrawnDriverHint)
					{
						foreach (RAGE.Vector3 vecPos in vecPassengerCoords)
						{
							WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Ride Train", null, () => { WorldInteraction_RideTrainAsPassenger(trainInst); }, vecPos, 0, false, false, fDistThresholdPassenger);
						}
					}
				}
			}
		}
	}
	// TODO_TRAIN: transmit occupant list, clients must warp peds into train on stream in etc

	private void WorldInteraction_RideTrainAsPassenger(CClientTrainInstance trainInst)
	{
		m_TrainBeingRode.SetTarget(trainInst);

		NetworkEventSender.SendNetworkEvent_TrainEnter(trainInst.ID, false);
	}

	private void WorldInteraction_DriveTrain(CClientTrainInstance trainInst)
	{
		m_TrainBeingDriven.SetTarget(trainInst);

		NetworkEventSender.SendNetworkEvent_TrainEnter(trainInst.ID, true);
	}

	private void SendSync(object[] parameters)
	{
		foreach (CClientTrainInstance trainInst in m_lstTrains)
		{
			if (trainInst.IsLocallySynced)
			{
				RAGE.Vector3 vecGTAPos = trainInst.GetGTAPosition();

				//ChatHelper.DebugMessage("[{3}] Send Pos {0}, {1}, {2} (Speed: {4})", vecGTAPos.X, vecGTAPos.Y, vecGTAPos.Z, trainInst.ID, trainInst.Speed);
				// TODO_TRAIN: Have to store speed on the train object is we are driving

				NetworkEventSender.SendNetworkEvent_TrainSync(trainInst.ID, vecGTAPos.X, vecGTAPos.Y, vecGTAPos.Z, trainInst.Speed, trainInst.LastTripWireID, trainInst.CurrentSector);
			}
		}
	}

	private void OnGiveOwnership(int ID)
	{
		CClientTrainInstance trainInst = GetTrainFromID(ID);
		if (trainInst != null)
		{
			trainInst.IsLocallySynced = true;

			// when we get ownership, immediately re-apply the AI state
			ApplyAILogicForTripwire(trainInst);
		}
	}

	private void OnRemoveOwnership(int ID)
	{
		CClientTrainInstance trainInst = GetTrainFromID(ID);
		if (trainInst != null)
		{
			trainInst.IsLocallySynced = false;
		}
	}

	private void OnCreateSyncedTrain(int ID, ETrainType TrainType, Vector3 Position, int CurrentSector, int LastTripWireID, float Speed, bool bDoorsOpen)
	{
		CClientTrainInstance trainInst = new CClientTrainInstance(TrainType, LastTripWireID, CurrentSector, Position, ID, Speed, bDoorsOpen);
		m_lstTrains.Add(trainInst);
		trainInst.Create(); // stream in immediately

		ApplyAILogicForTripwire(trainInst);

		int handleCarriage0 = RAGE.Game.Vehicle.GetTrainCarriage(trainInst.GetHandle(), 0);
		int handleCarriage1 = RAGE.Game.Vehicle.GetTrainCarriage(trainInst.GetHandle(), 1);
		//ChatHelper.ErrorMessage("CREATED SYNCED TRAIN: {0} and {1} (Sector: {2} and tripwire: {3})", handleCarriage0, handleCarriage1, trainInst.CurrentSector, trainInst.LastTripWireID);
	}

	private void OnDestroySyncedTrain(int ID)
	{
		CClientTrainInstance trainInst = GetTrainFromID(ID);
		if (trainInst != null)
		{
			// TODO_TRAINS: More cleanup of peds etc here
			trainInst.Destroy();
			m_lstTrains.Remove(trainInst);
		}
	}
}