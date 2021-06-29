using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using Dimension = System.UInt32;

public class NewsSystem 
{
	List<CPlayer> m_BroadcastPendingAcceptance = new List<CPlayer>();
	List<CPlayer> m_BroadcastParticipants = new List<CPlayer>();
	List<CPlayer> m_BroadcastSubscribers = new List<CPlayer>();

	private CPlayer m_InterviewStarter;
	private CPlayer m_CameraOperator;

	private GTANetworkAPI.Object m_NearestNewsCam = null;
	private bool m_BroadcastStarted { get; set; } = false;
	private ENewsBroadcastType m_BroadcastStartedOfType;

	public NewsSystem()
	{

		// INTERVIEW (HOST) COMMANDS
		CommandManager.RegisterCommand("interview", "Invites people to an interview", new Action<CPlayer, CVehicle, CPlayer>(InterviewCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("startbroadcast", "Starts the broadcast, set a nice welcoming message for the broadcast announcement.", new Action<CPlayer, CVehicle, ENewsBroadcastType, string>(OnStartBroadcast), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("broadcaststats", "Current interview statistics (viewers, participants)", new Action<CPlayer, CVehicle>(BroadcastStatsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("endbroadcast", "Ends the current interview", new Action<CPlayer, CVehicle>(OnEndBroadcast), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("updatenewsmessage", "Changes the breaking news message", new Action<CPlayer, CVehicle, string>(UpdateBreakingNewsMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);

		// CAMERA MAN
		CommandManager.RegisterCommand("endcamop", "Stop operating the camera", new Action<CPlayer, CVehicle>(OnStopOperateCamera), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		// INTERVIEW GUESTS
		CommandManager.RegisterCommand("i", "send a message in the interview chat", new Action<CPlayer, CVehicle, string>(OnInterviewMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("acceptinterview", "Accepts the interview", new Action<CPlayer, CVehicle>(OnAcceptInterview), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		// ALL PLAYERS
		CommandManager.RegisterCommand("joinbroadcast", "Joins the current broadcast in progress", new Action<CPlayer, CVehicle>(OnJoinBroadcast), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("leavebroadcast", "Leaves the current broadcast in progress", new Action<CPlayer, CVehicle>(OnLeaveBroadcast), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		// EVENTS
		NetworkEvents.PickupNewsCamera += OnPickupCamera;

		NetworkEvents.NewsCameraOperator += OnOperateNewsCamera;
	}

	public void InterviewCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News) || TargetPlayer == SenderPlayer)
		{
			return;
		}

		if (TargetPlayer != null && (m_BroadcastParticipants.Find(p => p == TargetPlayer) != null) || (m_BroadcastPendingAcceptance.Find(p => p == TargetPlayer) != null))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "{0} has already been invited or is already a participant!", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			return;
		}

		if (TargetPlayer != null)
		{
			TargetPlayer.PushChatMessageWithColor(EChatChannel.Factions, 155, 50, 168, "[NEWS] {0} invited you to join an interview! ((/acceptinterview to join))", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));


			// define as starter so we can get callback from invites.
			m_InterviewStarter = SenderPlayer;
			// add as participant so we also get to read the messages.
			m_BroadcastParticipants.Add(SenderPlayer);
			SenderPlayer.SetBroadcastParticipant(true);
			// add invited player to pending status
			m_BroadcastPendingAcceptance.Add(TargetPlayer);

			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "Succesfully sent an invite to {0}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
		}
	}

	public void OnAcceptInterview(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (m_BroadcastPendingAcceptance.Find(p => p == SenderPlayer) != null)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 155, 50, 168, "[NEWS] You have accepted the invite to join the interview. Wait for the host to start the interview!");

			// on accept add to participants
			m_BroadcastParticipants.Add(SenderPlayer);
			SenderPlayer.SetBroadcastParticipant(true);
			// remove from pending list
			m_BroadcastPendingAcceptance.Remove(SenderPlayer);

			m_InterviewStarter.SendNotification("News", ENotificationIcon.Star, "{0} has joined the interview!", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You haven't been invited to an interview!");
			return;
		}
	}

	public void UpdateBreakingNewsMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string BreakingNewsMessage)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		if (!m_BroadcastStarted && m_BroadcastStartedOfType != ENewsBroadcastType.Television)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 255, 0, 0, "You can only update the news message if you start a TV broadcast.");
			return;
		}

		if (BreakingNewsMessage.Length > 25)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 255, 0, 0, "Text too long. Please use less than 25 characters.");
			return;
		}
		else if (string.IsNullOrEmpty(BreakingNewsMessage))
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 255, 0, 0, "Please fill in a message to display for the viewers.");
		}

		foreach (CPlayer player in m_BroadcastSubscribers)
		{
			NetworkEventSender.SendNetworkEvent_SendBreakingNewsMessage(player, BreakingNewsMessage);
		}

		SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 255, 20, 147, "[NEWS] Successfully updated the broadcast message to: {0}", BreakingNewsMessage.ToUpper());
	}

	public void OnStartBroadcast(CPlayer SenderPlayer, CVehicle SenderVehicle, ENewsBroadcastType broadCastType, string strMessage)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		//We set the starter here if we don't set the start in the InterviewCommand method.
		if (m_InterviewStarter == null && !SenderPlayer.IsBroadcastParticipant)
		{
			m_InterviewStarter = SenderPlayer;
			m_BroadcastParticipants.Add(SenderPlayer);
			SenderPlayer.SetBroadcastParticipant(true);
		}

		if (broadCastType == ENewsBroadcastType.Television)
		{
			if (m_lstNewsCamera.Count < 1)
			{
				SenderPlayer.SendNotification("News", ENotificationIcon.Star, "Place a news camera to start a TV broadcast.");
				return;
			}

			m_BroadcastStartedOfType = ENewsBroadcastType.Television;
		}
		else if (broadCastType == ENewsBroadcastType.Interview)
		{
			if (m_BroadcastParticipants.Count >= 0 && m_BroadcastPendingAcceptance.Count == 0)
			{
				m_BroadcastStartedOfType = ENewsBroadcastType.Interview;
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Not enough participants for the interview to start. use /interview to invite a player");
				return;
			}
		}
		else
		{
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "No such Broadcast Type.");
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 255, 0, "1 = Television, 2 = Interview/Radio broadcast");
			return;
		}

		foreach (CPlayer player in m_BroadcastParticipants)
		{
			player.PushChatMessageWithColor(EChatChannel.Factions, 255, 20, 147, "[NEWS][{0}] The broadcast is about to start! Get Ready!", GetCurrentBroadcastInProgress());
		}

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (CPlayer player in players)
		{
			if (m_BroadcastParticipants.Where(p => p == player) != null)
			{
				continue;
			}

			player.PushChatMessageWithColor(EChatChannel.Factions, 255, 20, 147, "[NEWS][{0}] {1} ((/joinbroadcast to join))", GetCurrentBroadcastInProgress(), strMessage);
		}
		m_BroadcastStarted = true;
	}

	private void OnInterviewMessage(CPlayer SenderPlayer, CVehicle SenderVehicle, string strMessage)
	{
		if (SenderPlayer.IsBroadcastParticipant && m_BroadcastStarted)
		{
			foreach (CPlayer player in m_BroadcastSubscribers.Concat(m_BroadcastParticipants))
			{
				player.PushChatMessageWithRGBAndPlayerNameAndPrefix(EChatChannel.Factions, 255, 20, 147, SenderPlayer.GetActiveLanguage(), "[NEWS]", Helpers.FormatString("{0}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName)), "{0}", strMessage);
			}
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You are not part of an interview. Or there is no broadcast going on.");
			return;
		}
	}

	public void OnJoinBroadcast(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (m_BroadcastStarted)
		{
			m_BroadcastSubscribers.Add(SenderPlayer);
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "Succesfully joined the Broadcast! They should be starting soon!");
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 255, 20, 147, "[NEWS] ((Use /leavebroadcast to leave.))");

			if (m_BroadcastStartedOfType == ENewsBroadcastType.Television)
			{
				JoinTvBroadcast(SenderPlayer);
			}
			else if (m_BroadcastStartedOfType == ENewsBroadcastType.Interview)
			{
				// We don't do anything because it's only text.
			}
			else
			{
				// Nothing yet... Maybe in the future? :)
			}
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 155, 50, 168, "[NEWS] There is no broadcast right now! Check back later");
			return;
		}
	}

	public void OnLeaveBroadcast(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderPlayer.JoinedTvBroadcast && m_BroadcastStarted)
		{
			if (m_BroadcastStartedOfType == ENewsBroadcastType.Television)
			{
				SenderPlayer.SetAsJoinedTvBroadcast(false);
				NetworkEventSender.SendNetworkEvent_LeaveTvBroadcast(SenderPlayer);
			}
			m_BroadcastSubscribers.Remove(SenderPlayer);
		}
		else
		{
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You are not in a broadcast. Try joining one first.");
		}
	}

	public void BroadcastStatsCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (m_BroadcastStarted && SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			int currentViewCount = m_BroadcastSubscribers.Count;
			int currentParticipantsCount = m_BroadcastParticipants.Count;

			SenderPlayer.PushChatMessageWithColor(EChatChannel.Factions, 155, 50, 168, "[NEWS][{0}] Current viewer count: {1}, Current guests: {2}", GetCurrentBroadcastInProgress(), currentViewCount.ToString(), currentParticipantsCount.ToString());
		}
		else
		{
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You are not eligible to perform this command or there is no broadcast active");
			return;
		}
	}

	public void OnEndBroadcast(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (m_BroadcastStarted && m_InterviewStarter == SenderPlayer)
		{
			ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
			foreach (CPlayer player in players)
			{
				player.PushChatMessageWithColor(EChatChannel.Factions, 155, 50, 168, "[NEWS][{0}] The broadcast has ended! Thanks to everyone who tuned in. See you next time!", GetCurrentBroadcastInProgress());
			}

			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "Succesfully ended the interview!");

			// Clean up clear all lists and reset values
			foreach (CPlayer player in m_BroadcastParticipants)
			{
				if (m_BroadcastStartedOfType == ENewsBroadcastType.Television)
				{
					NetworkEventSender.SendNetworkEvent_LeaveTvBroadcast(player);
					player.SetAsJoinedTvBroadcast(false);
				}

				player.SetBroadcastParticipant(false);
			}

			m_BroadcastParticipants.Clear();
			m_BroadcastSubscribers.Clear();
			// We clear the pending invites to be sure.
			m_BroadcastPendingAcceptance.Clear();

			m_BroadcastStartedOfType = 0; // 0 means a non existing broadcast type thus clearing the variable.
			m_InterviewStarter = null;
			m_BroadcastStarted = false;
		}
	}

	public void JoinTvBroadcast(CPlayer SenderPlayer)
	{
		if (m_lstNewsCamera.Count == 1 && !SenderPlayer.JoinedTvBroadcast && m_CameraOperator != null && m_NearestNewsCam != null)
		{
			SenderPlayer.SetAsJoinedTvBroadcast(true);
			NetworkEventSender.SendNetworkEvent_JoinTvBroadcast(SenderPlayer, m_CameraOperator.Client, m_NearestNewsCam);
		}
		else if (m_lstNewsCamera.Count > 1) // Limit to one camera for now.
		{
			return;
		}
		else if (m_lstNewsCamera.Count < 1 || !m_BroadcastStarted)
		{
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "No broadcast in progress. Try again later!");
		}
	}

	public static void ToggleHandCamera(CPlayer SenderPlayer)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		if (SenderPlayer.IsHoldingBoomMic || SenderPlayer.IsHoldingMic)
		{
			return;
		}

		if (!SenderPlayer.IsHoldingCam)
		{
			SenderPlayer.SetHoldingCam(true);
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You are holding a cam!");
			SenderPlayer.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missfinale_c2mcs_1", "fin_c2_mcs_1_camman", false, true, false, 1000 * 1000, false);
		}
		else
		{
			SenderPlayer.StopCurrentAnimation(true, true);
			SenderPlayer.SetHoldingCam(false);
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You stopped holding a cam!");
		}
	}

	public static void ToggleNewsMicrophone(CPlayer SenderPlayer)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		if (SenderPlayer.IsHoldingBoomMic || SenderPlayer.IsHoldingCam)
		{
			return;
		}

		if (!SenderPlayer.IsHoldingMic)
		{
			SenderPlayer.SetHoldingMic(true);
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You are holding a mic!");
			SenderPlayer.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missheistdocksprep1hold_cellphone", "hold_cellphone", false, true, false, 1000 * 1000, false);
		}
		else
		{
			SenderPlayer.StopCurrentAnimation(true, true);
			SenderPlayer.SetHoldingMic(false);
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You stopped holding a mic!");
		}
	}

	public static void ToggleBoomMic(CPlayer SenderPlayer)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		if (SenderPlayer.IsHoldingCam || SenderPlayer.IsHoldingMic)
		{
			return;
		}


		if (!SenderPlayer.IsHoldingBoomMic)
		{
			SenderPlayer.SetHoldingBoomMic(true);
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You started holding a boom-mic!");
			SenderPlayer.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missfra1", "mcs2_crew_idle_m_boom", false, true, false, 1000 * 1000, false);
		}
		else
		{
			SenderPlayer.StopCurrentAnimation(true, true);
			SenderPlayer.SetHoldingBoomMic(false);
			SenderPlayer.SendNotification("News", ENotificationIcon.Star, "You stopped holding a boom-mic!");
		}
	}

	private string GetCurrentBroadcastInProgress()
	{
		string broadcastTypeString = "None";
		if (m_BroadcastStartedOfType == ENewsBroadcastType.Television)
		{
			broadcastTypeString = "TV";
		}
		else if (m_BroadcastStartedOfType == ENewsBroadcastType.Interview)
		{
			broadcastTypeString = "INTERVIEW";
		}

		return broadcastTypeString;
	}

	//START NEWSCAMERA 
	public static void PlaceNewsCamera(CPlayer SenderPlayer)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		Vector3 vecRot = SenderPlayer.Client.Rotation;
		vecRot.X = 0.0f;
		vecRot.Y = 0.0f;
		vecRot.Z += 180f; // rotate 180 degrees so it's faced where the player is looking upon spawning

		Vector3 vecPos = SenderPlayer.Client.Position;
		vecPos.X += (float)Math.Sin(-vecRot.Z * Math.PI / 180) * -1.0f;
		vecPos.Y += (float)Math.Cos(-vecRot.Z * Math.PI / 180) * -1.0f;

		SenderPlayer.SendNotification("News", ENotificationIcon.Star, "Successfully placed the news camera");
		NAPI.Task.Run(() =>
		{
			CNewsCamera newNewsCamera = new CNewsCamera(vecPos, vecRot, SenderPlayer.Client.Dimension);
			m_lstNewsCamera.Add(newNewsCamera);
		});
	}

	public void OnStopOperateCamera(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (!SenderPlayer.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		if (m_CameraOperator != SenderPlayer)
		{
			return;
		}

		NetworkEventSender.SendNetworkEvent_RemoveAsCamOperator(SenderPlayer);
	}

	public void OnPickupCamera(CPlayer player, GTANetworkAPI.Object newsCameraObject)
	{
		if (!player.IsOnDutyOfType(EDutyType.News))
		{
			return;
		}

		foreach (CNewsCamera newsCamera in m_lstNewsCamera)
		{
			if (newsCamera.Object.Position == newsCameraObject.Position)
			{
				newsCamera.Destroy();
				m_lstNewsCamera.Remove(newsCamera);
				break;
			}
		}
		player.SendNotification("News", ENotificationIcon.Star, "Successfully picked up the news camera");
	}

	public void OnOperateNewsCamera(CPlayer CameraOperator, GTANetworkAPI.Object NewsCameraObject)
	{
		m_CameraOperator = CameraOperator;
		m_NearestNewsCam = NewsCameraObject;
	}

	private class CNewsCamera : CBaseEntity
	{
		public CNewsCamera(Vector3 vecPos, Vector3 vecRot, Dimension a_Dimension)
		{
			Object = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_tv_cam_02_s"), vecPos, vecRot, 255, a_Dimension);

			// TODO_POST_LAUNCH: Move object creation to client side. Read todo SpikeStripsSystem
			SetData(Object, EDataNames.OBJECT_TYPE, EObjectTypes.NEWS_CAMERA, EDataType.Synced);
			SetData(Object, EDataNames.NEWS_CAM, true, EDataType.Synced);
		}

		~CNewsCamera()
		{
			Destroy();
		}

		public void Destroy()
		{
			if (Object != null)
			{
				NAPI.Task.Run(() =>
				{
					NAPI.Entity.DeleteEntity(Object.Handle);
				});
			}
		}

		public GTANetworkAPI.Object Object { get; }
	}

	private static List<CNewsCamera> m_lstNewsCamera = new List<CNewsCamera>();

}