using System;
using System.Collections.Generic;

public class FactionInvites
{
	private CGUIFactionInvite m_FactionInviteUI = new CGUIFactionInvite(() => { });

	private bool m_bShowingInvite = false;
	List<PendingInvite> m_lstPendingInvites = new List<PendingInvite>();

	private class PendingInvite
	{
		public string FactionName { get; }
		public string FromPlayerName { get; }
		public Int64 FactionID { get; }

		public PendingInvite(string strFactionName, string strFromPlayerName, Int64 factionID)
		{
			FactionName = strFactionName;
			FromPlayerName = strFromPlayerName;
			FactionID = factionID;
		}
	}

	public FactionInvites()
	{
		NetworkEvents.ReceivedFactionInvite += OnReceivedFactionInvite;
	}

	~FactionInvites()
	{

	}

	private void ProcessNextInvite()
	{
		if (!m_bShowingInvite)
		{
			if (m_lstPendingInvites.Count > 0)
			{
				PendingInvite pendingInvite = m_lstPendingInvites[0];

				m_FactionInviteUI.SetVisible(true, true, false);
				m_FactionInviteUI.ShowInvite(pendingInvite.FactionName, pendingInvite.FromPlayerName);

				m_bShowingInvite = true;
			}
		}
	}

	private void OnReceivedFactionInvite(string strFactionName, string strFromPlayerName, Int64 factionID)
	{
		m_lstPendingInvites.Add(new PendingInvite(strFactionName, strFromPlayerName, factionID));
		ProcessNextInvite();
	}

	public void SendFactionInviteDecision(bool bAccepted)
	{
		if (m_lstPendingInvites.Count > 0)
		{
			PendingInvite pendingInvite = m_lstPendingInvites[0];
			m_lstPendingInvites.RemoveAt(0);

			m_FactionInviteUI.SetVisible(false, false, false);
			m_bShowingInvite = false;

			NetworkEventSender.SendNetworkEvent_FactionInviteDecision(bAccepted, pendingInvite.FactionID);
			ProcessNextInvite();
		}
	}
}