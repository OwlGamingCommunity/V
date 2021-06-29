using System;
using System.Collections.Generic;

public abstract class ActivityInstance
{
	protected ActivityState m_State = null;

	protected Dictionary<int, List<RAGE.Elements.MapObject>> m_dictCardObjects = new Dictionary<int, List<RAGE.Elements.MapObject>>();
	protected List<RAGE.Elements.MapObject> m_lstChipObjects = new List<RAGE.Elements.MapObject>();

	protected Int64 m_uniqueIdentifier = -1;
	protected EActivityType m_activityType = EActivityType.None;

	protected int m_localPlayerParticipantID = -1;
	public bool LocalPlayerIsInTableButMayNotBePlaying()
	{
		return m_localPlayerParticipantID != -1;
	}

	public int GetLocalPlayerParticipantID()
	{
		return m_localPlayerParticipantID;
	}

	public ActivityInstance(Int64 uniqueIdentifier, EActivityType activityType)
	{
		m_uniqueIdentifier = uniqueIdentifier;
		m_activityType = activityType;
	}

	public abstract void OnStateReplication(string strState);
	public abstract void OnRoundOutcome(string strDealerOutcome, List<string> lstPlayerOutcomes);

	public void SetLocalPlayerParticipant(int participantID)
	{
		m_localPlayerParticipantID = participantID;
	}

	public void LocalPlayerLeft()
	{
		m_localPlayerParticipantID = -1;
		CleanupAllUI();
	}

	protected abstract void CleanupAllUI();
}