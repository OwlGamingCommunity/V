using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;

public class AdminReports
{
	public AdminReports()
	{
		NetworkEvents.OnPlayerDisconnected += OnPlayerDisconnected;
		NetworkEvents.SubmitAdminReport += SubmitAdminReport;
		NetworkEvents.CancelAdminReport += CancelAdminReport;
		NetworkEvents.Reports_ReloadReportData += OnReloadReportData;

		RageEvents.RAGE_OnUpdate += API_onUpdate;

		// COMMANDS
		CommandManager.RegisterCommand("report", "Reports an incident to the admin team", new Action<CPlayer, CVehicle>(OpenReportUI), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("er", "Closes the report you made", new Action<CPlayer, CVehicle>(ExitReport), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("ar", "Accepts a report", new Action<CPlayer, CVehicle, int>(AcceptReport), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("cr", "Closes a report", new Action<CPlayer, CVehicle, int>(CloseReport), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("dr", "Drops a report", new Action<CPlayer, CVehicle, int>(DropReport), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("ri", "Report Information", new Action<CPlayer, CVehicle, int>(ReportInformation), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("reports", "Shows all unaccepted reports", new Action<CPlayer, CVehicle>(Reports), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);

	}

	public void OnPlayerDisconnected(CPlayer player, DisconnectionType type, string reason)
	{
		// Remove reports made by this person
		foreach (var report in m_dictAdminReports.Values)
		{
			if (report.m_SourcePlayer.Instance() == player)
			{
				FreeReportID(report.m_reportID);
				report.EndReport(CAdminReport.EAdminReportEndReason.PlayerClosed);
			}
		}

		// Remove reports being handled by this person
		if (player.IsAdmin())
		{
			foreach (var report in m_dictAdminReports.Values)
			{
				if (report.m_Admin.Instance() == player)
				{
					report.DropReport(CAdminReport.EAdminReportDropReason.AdminDisconnected);
				}
			}
		}
	}

	class CAdminReport
	{
		public CAdminReport(CPlayer a_SourcePlayer, CPlayer a_TargetPlayer, EAdminReportType a_ReportType, string a_StrDetails, int a_reportID)
		{
			m_SourcePlayer = new WeakReference<CPlayer>(a_SourcePlayer);
			m_TargetPlayer = new WeakReference<CPlayer>(a_TargetPlayer);
			m_Admin = new WeakReference<CPlayer>(null);
			m_ReportType = a_ReportType;
			m_strDetails = a_StrDetails;
			m_targetCharacterName = a_TargetPlayer.GetCharacterName(ENameType.StaticCharacterName);
			m_targetAccountName = a_TargetPlayer.Username;
			m_reportID = a_reportID;

			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[REPORT] New Admin Report from {0} (Report ID {1}) (/ar {1} to accept!):", a_SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), m_reportID), false, EAdminLevel.TrialAdmin, 200, 200, 0, EChatChannel.AdminReports);
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[REPORT] '{0}'", m_strDetails), false, EAdminLevel.TrialAdmin, 200, 200, 0, EChatChannel.AdminReports);
		}

		private void SendReminder()
		{
			++m_remindersSent;
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[REPORT] Reminder #{2}: Admin Report from {0} (Report ID {1}) (/ar {1} to accept!) is pending.", m_SourcePlayer.Instance().GetCharacterName(ENameType.StaticCharacterName), m_reportID, m_remindersSent), false, EAdminLevel.TrialAdmin, 200, 200, 0, EChatChannel.AdminReports);
		}

		public void Tick()
		{
			// Has this report been handler?
			if (m_Admin.Instance() == null)
			{
				Int64 timeSinceLastReminder = MainThreadTimerPool.GetMillisecondsSinceDateTime(m_lastReminderTime);
				const int msBetweenReminders = 120000;

				if (timeSinceLastReminder >= msBetweenReminders)
				{
					SendReminder();
					m_lastReminderTime = DateTime.Now;
				}
			}
		}

		public void EndReport(EAdminReportEndReason endReason)
		{
			CPlayer adminPlayer = m_Admin.Instance();

			string closingMsg = "";
			if (endReason == EAdminReportEndReason.PlayerClosed)
			{
				closingMsg = "Closed by Player";
			}
			else if (endReason == EAdminReportEndReason.AdminClosed)
			{
				if (adminPlayer != null)
				{
					closingMsg = Helpers.FormatString("Closed by {0} {1} ({2})", adminPlayer.AdminTitle, adminPlayer.GetCharacterName(ENameType.StaticCharacterName), adminPlayer.Username);
				}
				else
				{
					closingMsg = "Closed by Admin";
				}
			}
			else if (endReason == EAdminReportEndReason.Timeout)
			{
				closingMsg = "No admins were available to service this ticket.";
			}
			else if (endReason == EAdminReportEndReason.PlayerDisconnected)
			{
				closingMsg = "Player Disconnected.";
			}

			CPlayer sourcePlayer = m_SourcePlayer.Instance();

			if (sourcePlayer != null)
			{
				sourcePlayer.SendNotification("Admin Report", ENotificationIcon.InfoSign, "Your report against '{0}' with Report ID {1} has been closed. Reason: {2}", m_targetCharacterName, m_reportID, closingMsg);
			}

			// inform admins
			string strSourceName = sourcePlayer != null ? sourcePlayer.GetCharacterName(ENameType.StaticCharacterName) : "Player Left";
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[REPORT] Admin Report from {0} (Report ID {1}) was closed. Reason: {2}", strSourceName, m_reportID, closingMsg), false, EAdminLevel.TrialAdmin, 200, 200, 0, EChatChannel.AdminReports);

			m_HasEnded = true;

			// If it wasn't player requested, inform the player
			if (endReason != EAdminReportEndReason.PlayerClosed)
			{
				if (sourcePlayer != null)
				{
					NetworkEventSender.SendNetworkEvent_AdminReportEnded(sourcePlayer);
				}
			}
		}

		public void DropReport(EAdminReportDropReason dropReason)
		{
			CPlayer adminPlayer = m_Admin.Instance();

			string dropMsg = "";
			if (dropReason == EAdminReportDropReason.AdminDisconnected)
			{
				dropMsg = "Admin Disconnected";
			}
			else if (dropReason == EAdminReportDropReason.AdminDropped)
			{
				if (adminPlayer != null)
				{
					dropMsg = Helpers.FormatString("Dropped by {0} {1} ({2})", adminPlayer.AdminTitle, adminPlayer.GetCharacterName(ENameType.StaticCharacterName), adminPlayer.Username);
				}
				else
				{
					dropMsg = "Dropped by Admin";
				}
			}

			CPlayer sourcePlayer = m_SourcePlayer.Instance();

			if (sourcePlayer != null)
			{
				sourcePlayer.SendNotification("Admin Report", ENotificationIcon.InfoSign, "Your report ({0}) has been dropped by the admin ({1}). Another member of staff will pick it up shortly.", m_reportID, dropMsg);
			}

			// inform admins
			HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[REPORT] Admin Report from {0} (Report ID {1}) was dropped by {2} {3} ({4}) - ({5}).", m_targetCharacterName, m_reportID, adminPlayer.AdminTitle, adminPlayer.GetCharacterName(ENameType.StaticCharacterName), adminPlayer.Username, dropMsg), false, EAdminLevel.TrialAdmin, 200, 200, 0, EChatChannel.AdminReports);

			// Reset handling admin & creation time (for alerts)
			m_Admin = new WeakReference<CPlayer>(null);
			m_creationTime = DateTime.Now;
			m_remindersSent = 0;
		}

		public enum EAdminReportEndReason
		{
			PlayerClosed,
			AdminClosed,
			Timeout,
			PlayerDisconnected
		}

		public enum EAdminReportDropReason
		{
			AdminDropped,
			AdminDisconnected
		}

		public WeakReference<CPlayer> m_TargetPlayer { get; private set; } = new WeakReference<CPlayer>(null);
		public WeakReference<CPlayer> m_SourcePlayer { get; private set; } = new WeakReference<CPlayer>(null);
		public WeakReference<CPlayer> m_Admin { get; private set; } = new WeakReference<CPlayer>(null);
		public EAdminReportType m_ReportType { get; private set; }
		public string m_strDetails { get; private set; }
		public int m_reportID { get; private set; }

		// These values are cached incase the target quits
		public string m_targetCharacterName { get; private set; }
		public string m_targetAccountName { get; private set; }

		private DateTime m_creationTime = DateTime.Now;
		private DateTime m_lastReminderTime = DateTime.Now;
		private int m_remindersSent = 0;
		public bool m_HasEnded { get; private set; } = false;
	}

	Dictionary<int, CAdminReport> m_dictAdminReports = new Dictionary<int, CAdminReport>();

	bool[] g_bReportSlots = new bool[100];

	int GetNextReportID()
	{
		for (int i = 0; i < g_bReportSlots.Length; ++i)
		{
			if (!g_bReportSlots[i])
			{
				g_bReportSlots[i] = true;
				return i + 1;
			}
		}

		return -1;
	}

	void FreeReportID(int id)
	{
		g_bReportSlots[id - 1] = false;
	}

	public void API_onUpdate()
	{
		// remove ended reports
		m_dictAdminReports = m_dictAdminReports.Where(pair => !pair.Value.m_HasEnded).ToDictionary(pair => pair.Key, pair => pair.Value);

		foreach (var report in m_dictAdminReports.Values)
		{
			report.Tick();
		}
	}

	/// <summary>
	/// In addition to the F3 bind for users from MTA
	/// </summary>
	/// <param name="source"></param>
	public void OpenReportUI(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		NetworkEventSender.SendNetworkEvent_ShowHelpCenter(SourcePlayer);
	}

	// TODO: Store how many reports an admin handled

	// ADMIN /dr
	public void DropReport(CPlayer SourcePlayer, CVehicle SourceVehicle, int id)
	{
		// Is this report valid?
		if (m_dictAdminReports.ContainsKey(id))
		{
			CPlayer currentAdminHandler = m_dictAdminReports[id].m_Admin.Instance();

			if (currentAdminHandler == SourcePlayer)
			{
				m_dictAdminReports[id].DropReport(CAdminReport.EAdminReportDropReason.AdminDropped);
			}
			else if (currentAdminHandler != null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "Report {0} is already being handled by another administrator.", id);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "You are not handling this report.", id);
			}
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "Report {0} does not exist!", id);
		}
	}


	// ADMIN /cr
	public void CloseReport(CPlayer SourcePlayer, CVehicle SourceVehicle, int id)
	{
		// Is this report valid?
		if (m_dictAdminReports.ContainsKey(id))
		{
			CPlayer currentAdminHandler = m_dictAdminReports[id].m_Admin.Instance();

			if (currentAdminHandler == SourcePlayer)
			{
				FreeReportID(id);
				m_dictAdminReports[id].EndReport(CAdminReport.EAdminReportEndReason.AdminClosed);
				SourcePlayer.UpdateAdminReportCount();
			}
			else if (currentAdminHandler != null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "Report {0} is already being handled by another administrator.", id);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "You are not handling this report.", id);
			}
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "Report {0} does not exist!", id);
		}
	}

	// ADMIN /ar
	public void AcceptReport(CPlayer SourcePlayer, CVehicle SourceVehicle, int id)
	{
		// Is this report valid?
		if (m_dictAdminReports.ContainsKey(id))
		{
			var report = m_dictAdminReports[id];
			CPlayer currentAdminHandler = report.m_Admin.Instance();

			if (currentAdminHandler != null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] Report {0} is already being handled by another administrator.", id);
			}
			else if (report.m_TargetPlayer.Instance() == SourcePlayer)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] Report {0} is reporting you. You cannot handle it.", id);
			}
			else
			{
				report.m_Admin.SetTarget(SourcePlayer);
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[REPORT] {0} {1} accepted report {2} from {3} against {4}.", SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), id, report.m_SourcePlayer.Instance().GetCharacterName(ENameType.StaticCharacterName), report.m_targetCharacterName), false, EAdminLevel.TrialAdmin, 200, 200, 0, EChatChannel.AdminReports);

				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] You are now handling report ID {0} from '{1}' against '{2}'.", id, report.m_SourcePlayer.Instance().GetCharacterName(ENameType.StaticCharacterName), report.m_targetCharacterName);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] '{0}'", report.m_strDetails);

				report.m_SourcePlayer.Instance().SendNotification("Admin Report", ENotificationIcon.InfoSign, "Your admin report is now being handled by '{0} {1}' ({2})", SourcePlayer.AdminTitle, SourcePlayer.Username, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName));
			}
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] Report {0} does not exist!", id);
		}
	}

	// ADMIN /ri
	public void ReportInformation(CPlayer SourcePlayer, CVehicle SourceVehicle, int id)
	{
		// Is this report valid?
		if (m_dictAdminReports.ContainsKey(id))
		{
			var report = m_dictAdminReports[id];
			CPlayer currentAdminHandler = report.m_Admin.Instance();

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] Report ID {0} from '{1}' against '{2}'.", id, report.m_SourcePlayer.Instance().GetCharacterName(ENameType.StaticCharacterName), report.m_targetCharacterName);
			if (currentAdminHandler != null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] Report {0} is being handled by {1}({2}).", id, currentAdminHandler.GetCharacterName(ENameType.StaticCharacterName), currentAdminHandler.Username);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] Report {0} is not being handled.", id);
			}

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] '{0}'", report.m_strDetails);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminReports, 200, 200, 0, "[ADMIN] [REPORT] Report {0} does not exist!", id);
		}
	}

	public void Reports(CPlayer player, CVehicle vehicle)
	{
		List<CPlayerReport> playerReports = new List<CPlayerReport>();
		foreach (var report in m_dictAdminReports.Values)
		{
			string reporter = report.m_SourcePlayer.Instance() == null ? "None" : report.m_SourcePlayer.Instance().GetCharacterName(ENameType.CharacterDisplayName);
			string handlingAdmin = report.m_Admin.Instance() == null ? "nobody" : report.m_Admin.Instance().Username;
			playerReports.Add(new CPlayerReport(report.m_reportID.ToString(), reporter, report.m_targetCharacterName, report.m_strDetails, report.m_ReportType.ToString(), handlingAdmin));
		}
		NetworkEventSender.SendNetworkEvent_Reports_SendReportData(player, playerReports);
	}

	private void OnReloadReportData(CPlayer player)
	{
		List<CPlayerReport> playerReports = new List<CPlayerReport>();
		foreach (var report in m_dictAdminReports.Values)
		{
			string reporter = report.m_SourcePlayer.Instance() == null ? "None" : report.m_SourcePlayer.Instance().GetCharacterName(ENameType.CharacterDisplayName);
			string handlingAdmin = report.m_Admin.Instance() == null ? "None" : report.m_Admin.Instance().Username;
			playerReports.Add(new CPlayerReport(report.m_reportID.ToString(), reporter, report.m_targetCharacterName, report.m_strDetails, report.m_ReportType.ToString(), handlingAdmin));
		}
		NetworkEventSender.SendNetworkEvent_Reports_UpdateReportData(player, playerReports);
	}

	public void CancelAdminReport(CPlayer sourcePlayer)
	{
		foreach (var report in m_dictAdminReports.Values)
		{
			if (report.m_SourcePlayer.Instance() == sourcePlayer)
			{
				FreeReportID(report.m_reportID);
				report.EndReport(CAdminReport.EAdminReportEndReason.PlayerClosed);
			}
		}
	}

	public void SubmitAdminReport(CPlayer sourcePlayer, EAdminReportType reportType, string strDetails, Player rageTargetPlayer)
	{
		WeakReference<CPlayer> targetPlayerRef = PlayerPool.GetPlayerFromClient(rageTargetPlayer);
		CPlayer targetPlayer = targetPlayerRef.Instance();

		if (targetPlayer != null)
		{
			int reportID = GetNextReportID();
			if (reportID == -1)
			{
				// TODO_LAUNCH: Fatal error + reset clientside UI start
				return;
			}
			else
			{
				CAdminReport newReport = new CAdminReport(sourcePlayer, targetPlayer, reportType, strDetails, reportID);
				m_dictAdminReports[reportID] = newReport;

				sourcePlayer.SendNotification("Admin Report", ENotificationIcon.InfoSign, "Your report against '{0}' has been created with ID {1}. A member of staff will be with you shortly.", targetPlayer.GetCharacterName(ENameType.CharacterDisplayName), reportID);
			}
		}
		else
		{
			sourcePlayer.SendNotification("Admin Report", ENotificationIcon.InfoSign, "Your report against '{0}' could not be filed. The player was not found.", targetPlayer.GetCharacterName(ENameType.CharacterDisplayName));

			// This even will reset the clientside UI, since we didnt actually create a report for them
			NetworkEventSender.SendNetworkEvent_AdminReportEnded(sourcePlayer);
		}
	}

	public void ExitReport(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		foreach (CAdminReport report in m_dictAdminReports.Values)
		{
			if (report.m_SourcePlayer.Instance().Equals(SourcePlayer))
			{
				FreeReportID(report.m_reportID);
				report.EndReport(CAdminReport.EAdminReportEndReason.PlayerClosed);
				NetworkEventSender.SendNetworkEvent_AdminReportEnded(SourcePlayer);
			}
		}
	}
}