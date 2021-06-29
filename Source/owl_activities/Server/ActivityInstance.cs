using System;
using System.Collections.Generic;

public abstract class ActivityInstance
{
	protected ActivityState m_State { get; set; } = null;
	protected List<CPlayer> m_lstSubscribers { get; set; } = new List<CPlayer>();
	protected List<CPlayer> m_lstParticipants { get; set; } = new List<CPlayer>();

	protected Int64 m_uniqueIdentifier { get; set; } = -1;
	protected EActivityType m_activityType { get; set; } = EActivityType.None;

	private int MaxParticipants { get; set; } = 0;
	private DateTime StateBeginTime { get; set; } = DateTime.Now;

	protected void ResetStateTimer()
	{
		StateBeginTime = DateTime.Now;
	}

	protected Int64 GetMillisecondsInState()
	{
		return (Int64)(DateTime.Now - StateBeginTime).TotalMilliseconds;
	}

	public abstract void OnParticipantJoined(CPlayer a_Player);
	public abstract void OnParticipantLeft(CPlayer a_Player);
	public abstract void Update();

	public ActivityInstance(int a_maxParticipants, Int64 uniqueIdentifier, EActivityType activityType)
	{
		MaxParticipants = a_maxParticipants;
		m_uniqueIdentifier = uniqueIdentifier;
		m_activityType = activityType;
	}


	public abstract bool TryParticipate_EventSpecificChecks(CPlayer a_Player, int participantIndex, out string strFailureReason);
	public bool TryParticipate(CPlayer a_Player, out int participantIndex, out string strFailureReason)
	{
		/*
		if (!a_Player.IsAdmin())
		{
			participantIndex = -1;
			strFailureReason = "Sorry, this feature is only for Administrators right now.";
			return false;
		}
		*/

		strFailureReason = String.Empty;

		if (m_lstParticipants.Count < MaxParticipants)
		{
			participantIndex = m_lstParticipants.Count;

			if (!TryParticipate_EventSpecificChecks(a_Player, participantIndex, out string strEventSpecificFailureReason))
			{
				strFailureReason = strEventSpecificFailureReason;
				return false;
			}
			else
			{
				m_lstParticipants.Add(a_Player);
				return true;
			}
		}
		else
		{
			participantIndex = -1;
			strFailureReason = "Table is currently full";
			return false;
		}
	}

	public void ParticipantLeave(CPlayer a_Player)
	{
		if (a_Player != null)
		{
			RemoveSubscriber(a_Player);
			OnParticipantLeft(a_Player);

			NetworkEventSender.SendNetworkEvent_ResetActivityState(a_Player);
		}
	}

	// State replication
	public void TransmitState()
	{
		string strState = Newtonsoft.Json.JsonConvert.SerializeObject(m_State);
		foreach (CPlayer subscriber in m_lstSubscribers)
		{
			NetworkEventSender.SendNetworkEvent_ReplicateActivityState(subscriber, m_uniqueIdentifier, m_activityType, strState);
		}
	}

	public void TransmitStateToPlayer(CPlayer a_Player)
	{
		string strState = Newtonsoft.Json.JsonConvert.SerializeObject(m_State);
		NetworkEventSender.SendNetworkEvent_ReplicateActivityState(a_Player, m_uniqueIdentifier, m_activityType, strState);
	}

	// Subscriptions
	public void AddSubscriber(CPlayer a_Player)
	{
		if (!m_lstSubscribers.Contains(a_Player))
		{
			m_lstSubscribers.Add(a_Player);
			TransmitStateToPlayer(a_Player);
		}
	}

	public void RemoveSubscriber(CPlayer a_Player)
	{
		if (m_lstSubscribers.Contains(a_Player))
		{
			m_lstSubscribers.Remove(a_Player);
		}
	}

	protected void SendActivityMessage(string strFormat, params object[] parameters)
	{
		foreach (CPlayer participant in m_lstParticipants)
		{
			participant.PushChatMessageWithColor(EChatChannel.Nearby, 0, 200, 0, strFormat, parameters);
		}
	}

	public void OnDestroy()
	{
		// We have to copy here to avoid collection being modified
		List<CPlayer> lstParticipantsToRemove = new List<CPlayer>(m_lstParticipants);

		foreach (CPlayer participant in lstParticipantsToRemove)
		{
			ParticipantLeave(participant);
		}
	}
}