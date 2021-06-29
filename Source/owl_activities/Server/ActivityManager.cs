using System;
using System.Collections.Generic;

public abstract class ActivityManager
{
	protected Dictionary<Int64, ActivityInstance> m_dictActivityInstances { get; } = new Dictionary<long, ActivityInstance>();
	protected EActivityType m_activityManagerType { get; } = EActivityType.None;

	public ActivityManager(EActivityType activityManagerType)
	{
		m_activityManagerType = activityManagerType;
	}

	public void OnUpdate()
	{
		foreach (ActivityInstance inst in m_dictActivityInstances.Values)
		{
			inst.Update();
		}
	}

	public void StartActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			// Event must be sent before participate, or they'll miss the first replicate
			if (m_dictActivityInstances[uniqueActivityIdentifier].TryParticipate(a_Player, out int participantIndex, out string strFailureReason))
			{
				NetworkEventSender.SendNetworkEvent_StartActivityApproved(a_Player, participantIndex, uniqueActivityIdentifier, m_activityManagerType);

				// now actually add the participant
				m_dictActivityInstances[uniqueActivityIdentifier].OnParticipantJoined(a_Player);
				m_dictActivityInstances[uniqueActivityIdentifier].AddSubscriber(a_Player);
			}
			else
			{
				OnFailJoinActivity(strFailureReason);
			}
		}
		else
		{
			OnFailJoinActivity("Unknown Error");
		}

		void OnFailJoinActivity(string strReason)
		{
			KillPlayerActivity(a_Player);
			a_Player.SendNotification("Failed to participate in activity", ENotificationIcon.InfoSign, strReason);
		}
	}

	public void StopActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier].ParticipantLeave(a_Player);
		}
	}

	public void KillPlayerActivity(CPlayer player)
	{
		foreach (var activityInstance in m_dictActivityInstances)
		{
			activityInstance.Value.ParticipantLeave(player);
		}
	}

	public void SubscribeActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier].AddSubscriber(a_Player);
		}
	}

	public void UnsubscribeActivity(CPlayer a_Player, Int64 uniqueActivityIdentifier)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier].RemoveSubscriber(a_Player);
		}
	}

	public ActivityInstance CreateNewInstance(Int64 uniqueIdentifier)
	{
		ActivityInstance newInst = CreateNewInstance_Internal(uniqueIdentifier);
		m_dictActivityInstances.Add(uniqueIdentifier, newInst);

		return newInst;
	}

	public void RemoveActivityInstance(Int64 uniqueIdentifier)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueIdentifier))
		{
			ActivityInstance activityInst = m_dictActivityInstances[uniqueIdentifier];
			if (activityInst != null)
			{
				activityInst.OnDestroy();
				m_dictActivityInstances.Remove(uniqueIdentifier);
			}
		}
	}
	protected abstract ActivityInstance CreateNewInstance_Internal(Int64 uniqueIdentifier);
}
