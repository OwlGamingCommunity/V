using GTANetworkAPI;
using System;
using System.Collections.Generic;

public static class TaxiDriverJob
{
	static TaxiDriverJob()
	{

	}

	public static void Init()
	{
		// COMMANDS
		CommandManager.RegisterCommand("endtaxi", "Ends current taxi ride", new Action<CPlayer, CVehicle>(OnEndTaxi), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle);
		CommandManager.RegisterCommand("accepttaxi", "Accepts a taxi ride", new Action<CPlayer, CVehicle, int>(OnAcceptTaxi), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle);

		NetworkEvents.OnPlayerDisconnected += OnPlayerDisconnected;
		RageEvents.RAGE_OnPlayerEnterVehicle += OnPlayerEnterVehicle;
		RageEvents.RAGE_OnPlayerExitVehicle += OnPlayerExitVehicle;

		NetworkEvents.ToggleAvailableForHire += OnToggleAvailableForHire;
		NetworkEvents.ResetFare += OnResetFare;
		NetworkEvents.ChangeFarePerMile += OnChangeFarePerMile;
		NetworkEvents.TaxiDriverJob_AtPickup += OnTaxiDriverJob_AtPickup;
	}

	private static void OnPlayerExitVehicle(Player sender, Vehicle vehicle)
	{
		WeakReference<CPlayer> SourcePlayerRef = PlayerPool.GetPlayerFromClient(sender);
		CPlayer SourcePlayer = SourcePlayerRef.Instance();

		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

		if (SourcePlayer != null && pVehicle != null && pVehicle.VehicleType == EVehicleType.TaxiJob)
		{
			TaxiDriverJobInstance jobInstance = JobSystem.GetTaxiDriverInstance(SourcePlayer);
			if (jobInstance != null)
			{
				jobInstance.OnExitTaxiVehicle(pVehicle);
			}
		}
	}

	public static void OnPlayerEnterVehicle(Player sender, Vehicle vehicle, sbyte seatId)
	{
		WeakReference<CPlayer> SourcePlayerRef = PlayerPool.GetPlayerFromClient(sender);
		CPlayer SourcePlayer = SourcePlayerRef.Instance();
		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

		if (SourcePlayer != null && pVehicle != null && pVehicle.VehicleType == EVehicleType.TaxiJob)
		{
			// TODO_POST_LAUNCH: Check driver
			TaxiDriverJobInstance jobInstance = JobSystem.GetTaxiDriverInstance(SourcePlayer);
			if (jobInstance != null)
			{
				jobInstance.OnEnterTaxiVehicle(pVehicle);
			}
		}
	}

	public static void OnToggleAvailableForHire(CPlayer player)
	{
		TaxiDriverJobInstance jobInstance = JobSystem.GetTaxiDriverInstance(player);
		if (jobInstance != null)
		{
			jobInstance.ToggleAvailableForHire();
		}
	}

	public static void OnResetFare(CPlayer player)
	{
		TaxiDriverJobInstance jobInstance = JobSystem.GetTaxiDriverInstance(player);
		if (jobInstance != null)
		{
			jobInstance.ResetFare();
		}
	}

	public static void OnChangeFarePerMile(CPlayer player, float fCharge)
	{
		TaxiDriverJobInstance jobInstance = JobSystem.GetTaxiDriverInstance(player);
		if (jobInstance != null)
		{
			jobInstance.ChangeFarePerMile(fCharge);
		}
	}

	public static void OnTaxiDriverJob_AtPickup(CPlayer player)
	{
		foreach (TaxiRequest request in m_TaxiRequests.Values)
		{
			if (request.GetAssignedDriver() == player)
			{
				request.DriverArrived();
				break;
			}
		}
	}

	public static void OnPlayerDisconnected(CPlayer player, DisconnectionType type, string reason)
	{
		// Remove any taxi requests created by this person
		if (m_TaxiRequests.ContainsKey(player))
		{
			TaxiRequest request = m_TaxiRequests[player];

			string strCancelMessage = Helpers.FormatString("Taxi request from '{0}' was canceled due to player disconnection.", player.GetCharacterName(ENameType.StaticCharacterName));

			// React differently depending on state
			if (request.State() == TaxiRequest.ETaxiRequestState.New)
			{
				// tell all drivers its canceled
				SendMessageToAllDrivers(strCancelMessage);
			}
			else
			{
				// If in progress or driver assigned, cleanup for driver, and tell individual driver
				CPlayer pAssignedDriver = request.GetAssignedDriver();
				if (pAssignedDriver != null)
				{
					// TODO_CHAT: Notifications for this job
					pAssignedDriver.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, strCancelMessage);
				}
			}

			request.ClientsideCleanup();
			m_TaxiRequests.Remove(request.RequestingPlayer());
		}

		// Remove any taxi requests handled by this person
		foreach (TaxiRequest request in m_TaxiRequests.Values)
		{
			if (request.GetAssignedDriver() == player)
			{
				request.RequestingPlayer().PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TAXI: Your pickup was canceled due to driver disconnection.");
				request.ClientsideCleanup();
				m_TaxiRequests.Remove(request.RequestingPlayer());
				return;
			}
		}
	}

	// CALLING TAXI
	public static void OnRequestTaxi(CPlayer a_Player)
	{
		if (a_Player != null)
		{
			// Do we have a pending taxi request?
			if (m_TaxiRequests.ContainsKey(a_Player))
			{
				a_Player.SendNotification("Taxi", ENotificationIcon.InfoSign, "You have already called a taxi. Click again on the button to cancel your taxi.", null);
			}
			else
			{
				TaxiRequest request = new TaxiRequest(a_Player, a_Player.Client.Position, DateTime.Now, a_Player.PlayerID);
				m_TaxiRequests[a_Player] = request;

				a_Player.SendNotification("Taxi", ENotificationIcon.InfoSign, "You have requested a taxi. Click again on the button to cancel your taxi.", null);

				// tell all drivers its requested
				SendMessageToAllDrivers(Helpers.FormatString("TAXI: Taxi request from '{0}'. Use '/accepttaxi {1}' to accept.", a_Player.GetCharacterName(ENameType.StaticCharacterName), request.GetRequestID()));
			}
		}
	}

	public static bool GetTaxiStateForPlayer(CPlayer a_Player)
	{
		return m_TaxiRequests.ContainsKey(a_Player);
	}

	private static void SendMessageToAllDrivers(string strMessage)
	{
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var pPlayer in players)
		{
			if (pPlayer.Job == EJobID.TaxiDriverJob)
			{
				// Are we in a taxi?
				if (pPlayer.IsInVehicleReal)
				{
					CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);

					if (pVehicle != null && pVehicle.VehicleType == EVehicleType.TaxiJob)
					{
						// TODO_POST_LAUNCH: Seperate channel for jobs
						pPlayer.PushChatMessage(EChatChannel.Notifications, strMessage);
					}
				}
			}
		}
	}

	public static void OnEndTaxi(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		foreach (TaxiRequest request in m_TaxiRequests.Values)
		{
			if (request.GetAssignedDriver() == SourcePlayer)
			{
				// Achievement
				SourcePlayer.AwardAchievement(EAchievementID.CompleteFareTaxiDriverJob);

				// TODO_CHAT: Notifications
				request.RequestingPlayer().PushChatMessage(EChatChannel.Notifications, "TAXI: Your pickup was ended by the driver.");
				SourcePlayer.PushChatMessage(EChatChannel.Notifications, "TAXI: You have ended your current pickup.");

				request.ClientsideCleanup();
				m_TaxiRequests.Remove(request.RequestingPlayer());
				return;
			}
		}

		SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TAXI: You are not assigned to any pickup currently.");
	}

	public static void OnAcceptTaxi(CPlayer SourcePlayer, CVehicle SourceVehicle, int TaxiRequestID)
	{
		// We must be in a taxi
		if (SourceVehicle.VehicleType != EVehicleType.TaxiJob)
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TAXI: You cannot accept this request as you are not in a taxi.");
			return;
		}

		// Do we already have a job assigned?
		foreach (TaxiRequest request in m_TaxiRequests.Values)
		{
			if (request.GetAssignedDriver() == SourcePlayer)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TAXI: You cannot accept this request as you are already on another job.");
				return;
			}
		}

		foreach (TaxiRequest request in m_TaxiRequests.Values)
		{
			if (request.GetRequestID() == TaxiRequestID)
			{
				if (request.State() == TaxiRequest.ETaxiRequestState.New)
				{

					SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 0, 255, 0, "TAXI: You have accepted taxi request {0}. Drive to the pickup location shown on your map.", TaxiRequestID);
					request.RequestingPlayer().PushChatMessageWithColor(EChatChannel.Notifications, 0, 255, 0, "TAXI: A driver accepted your taxi request and is on their way. Your driver is {0}", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName));
					request.AssignDriver(SourcePlayer);
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TAXI: You cannot accept this request as it has already been accepted by another driver.");
			}

			return;
		}
		SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "TAXI: No request was found with this ID.");
	}

	public static void OnRequestCancelTaxi(CPlayer a_Player)
	{
		// TODO: Time out requests if no driver takes it
		if (a_Player != null)
		{
			// Do we have a pending taxi request?
			if (m_TaxiRequests.ContainsKey(a_Player))
			{
				TaxiRequest request = m_TaxiRequests[a_Player];

				string strCancelMessage = Helpers.FormatString("Taxi request from '{0}' was canceled.", a_Player.GetCharacterName(ENameType.StaticCharacterName));

				// React differently depending on state
				if (request.State() == TaxiRequest.ETaxiRequestState.New)
				{
					// tell all drivers its canceled
					SendMessageToAllDrivers(strCancelMessage);
				}
				else
				{
					// If in progress or driver assigned, cleanup for driver, and tell individual driver
					CPlayer pAssignedDriver = request.GetAssignedDriver();
					if (pAssignedDriver != null)
					{
						pAssignedDriver.PushChatMessage(EChatChannel.Notifications, strCancelMessage);

						request.ClientsideCleanup();
					}
				}

				a_Player.SendNotification("Taxi", ENotificationIcon.InfoSign, "You have canceled your taxi request.", null);

				// TODO: Handle clientside for the driver
				m_TaxiRequests.Remove(a_Player);
			}
			else
			{
				a_Player.SendNotification("Taxi", ENotificationIcon.ExclamationSign, "You do not have a pending taxi request. Use a cellphone to request one", null);
			}
		}
	}

	private static Dictionary<CPlayer, TaxiRequest> m_TaxiRequests = new Dictionary<CPlayer, TaxiRequest>();

	private class TaxiRequest
	{
		public TaxiRequest(CPlayer a_RequestingPlayer, Vector3 vecPickupPos, DateTime requestTime, int a_RequestID)
		{
			m_RequestingPlayer.SetTarget(a_RequestingPlayer);
			m_vecPickupPos = vecPickupPos;
			m_requestTime = requestTime;
			m_State = ETaxiRequestState.New;
			m_RequestID = a_RequestID;
		}

		public void ClientsideCleanup()
		{
			CPlayer pPlayer = m_AssignedDriver.Instance();
			if (pPlayer == null)
			{
				return;
			}

			NetworkEventSender.SendNetworkEvent_TaxiCleanup(pPlayer);
		}

		public ETaxiRequestState State()
		{
			return m_State;
		}

		public CPlayer GetAssignedDriver()
		{
			return m_AssignedDriver.Instance();
		}

		public CPlayer RequestingPlayer()
		{
			return m_RequestingPlayer.Instance();
		}

		public void AssignDriver(CPlayer a_Driver)
		{
			m_State = ETaxiRequestState.DriverAssigned;
			m_AssignedDriver.SetTarget(a_Driver);

			NetworkEventSender.SendNetworkEvent_TaxiAccepted(a_Driver, m_vecPickupPos);
		}

		public void DriverArrived()
		{
			if (m_State == ETaxiRequestState.DriverAssigned)
			{
				m_State = ETaxiRequestState.InProgress;

				CPlayer requestingPlayer = m_RequestingPlayer.Instance();
				CPlayer assignedDriver = m_AssignedDriver.Instance();

				if (requestingPlayer != null && assignedDriver != null)
				{
					requestingPlayer.SendNotification("Taxi", ENotificationIcon.InfoSign, "Your taxi has arrived!", null);

					// TODO_CHAT: Notification
					assignedDriver.PushChatMessage(EChatChannel.Notifications, "You have arrived at the pickup location. Reset your meter and wait for the passenger. You can cancel or end a job using /endtaxi");
					assignedDriver.PushChatMessage(EChatChannel.Notifications, "NOTE: Fare is not automatically deducted. You are responsible for getting the passenger to pay via /pay");
				}
			}
		}

		public int GetRequestID()
		{
			return m_RequestID;
		}

		private WeakReference<CPlayer> m_RequestingPlayer = new WeakReference<CPlayer>(null);
		private readonly Vector3 m_vecPickupPos;
		private readonly DateTime m_requestTime;
		private ETaxiRequestState m_State;
		private WeakReference<CPlayer> m_AssignedDriver = new WeakReference<CPlayer>(null);
		private readonly int m_RequestID;

		public enum ETaxiRequestState
		{
			New,
			DriverAssigned,
			InProgress
		}
	}

}
