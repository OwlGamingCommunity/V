using System;
using System.Collections.Generic;

public abstract class ActivityManager
{
	private Dictionary<Int64, ActivityInstance> m_dictActivityInstances = new Dictionary<long, ActivityInstance>();

	public void OnStartActivity(Int64 uniqueActivityIdentifier, out ActivityInstance outInstance, int participantID)
	{
		outInstance = null;

		RAGE.Game.Ui.DisplayRadar(false);

		// Do we already have an active activity for this identifier? If so return it and join the game
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier].SetLocalPlayerParticipant(participantID);
			outInstance = m_dictActivityInstances[uniqueActivityIdentifier];
		}
	}

	public void StopActivity(Int64 uniqueActivityIdentifier)
	{
		RAGE.Game.Ui.DisplayRadar(true);

		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier].LocalPlayerLeft();
		}
	}

	public void OnStateReplication(Int64 uniqueActivityIdentifier, string strState)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier].OnStateReplication(strState);
		}
	}

	public void OnRoundOutcome(Int64 uniqueActivityIdentifier, string strDealerOutcome, List<string> lstPlayerOutcomes)
	{
		if (m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier].OnRoundOutcome(strDealerOutcome, lstPlayerOutcomes);
		}
	}

	public ActivityInstance CreateNewInstance(Int64 uniqueActivityIdentifier)
	{
		if (!m_dictActivityInstances.ContainsKey(uniqueActivityIdentifier))
		{
			m_dictActivityInstances[uniqueActivityIdentifier] = CreateNewInstance_Internal(uniqueActivityIdentifier);
		}

		return m_dictActivityInstances[uniqueActivityIdentifier];
	}

	protected abstract ActivityInstance CreateNewInstance_Internal(Int64 uniqueIdentifier);
}