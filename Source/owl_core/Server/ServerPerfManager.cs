using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Reflection;

using EntityDatabaseID = System.Int64;

public static class ServerPerfManager
{
	const double dThresholdForSlowWarn = 1000.0 / 30.0; // 30hz, so > 33.3ms is slow.
	const Int64 CooldownBetweenSentryReports = 600000; // 10 minutes

	private static Dictionary<string, DateTime> g_ReportCooldowns = new Dictionary<string, DateTime>();

	private static ThreadChecker g_MainThreadChecker = new ThreadChecker();

	static ServerPerfManager()
	{
		LargeDataTransferManager.RegisterIncomingTransferCallbacks(ELargeDataTransferType.PerfCapture, OnPerfCaptureTransfer_Started, OnPerfCaptureTransfer_Progress, OnPerfCaptureTransfer_Complete);

		CommandManager.RegisterCommand("perfcaptures", "Lists performance captures", new Action<CPlayer, CVehicle>(ListPerfCaptures), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeScripter);
		CommandManager.RegisterCommand("startperfcapture", "Starts a performance capture for the target player", new Action<CPlayer, CVehicle, CPlayer, int>(StartPerfCapture), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeScripter);
		CommandManager.RegisterCommand("getperfcapture", "Downloads a performance capture", new Action<CPlayer, CVehicle, EntityDatabaseID>(GetPerfCapture), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeScripter);
		CommandManager.RegisterCommand("delperfcapture", "Deletes a performance capture", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeletePerfCapture), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeScripter);
	}

	private static void StartPerfCapture(CPlayer a_Player, CVehicle a_Vehicle, CPlayer a_TargetPlayer, int durationInSeconds)
	{
		if (durationInSeconds > 0 && durationInSeconds <= 60)
		{
			NetworkEventSender.SendNetworkEvent_StartPerformanceCapture(a_TargetPlayer, durationInSeconds);
		}
		else
		{
			a_Player.PushChatMessageWithColor(EChatChannel.Global, 255, 0, 0, "Performance captures must be between 1 and 60 seconds");
		}
	}

	private static void ListPerfCaptures(CPlayer a_Player, CVehicle a_Vehicle)
	{
		Database.Functions.Util.GetPerformanceCaptures(async (List<CDatabaseStructurePerformanceCapture> lstCaptures) =>
		{
			a_Player.PushChatMessageWithColor(EChatChannel.Global, 100, 100, 255, "Performance Captures:");
			foreach (CDatabaseStructurePerformanceCapture capture in lstCaptures)
			{
				string strAccountName = await Database.LegacyFunctions.GetUsernameFromAccount(capture.AccountID).ConfigureAwait(true);
				a_Player.PushChatMessageWithColor(EChatChannel.Global, 100, 100, 255, "- {0}: ({1})", capture.DBID, strAccountName);
			}
		});
	}

	private static void DeletePerfCapture(CPlayer a_Player, CVehicle a_Vehicle, EntityDatabaseID a_DBID)
	{
		Database.Functions.Util.DeletePerformanceCapture(a_DBID);
		a_Player.PushChatMessageWithColor(EChatChannel.Global, 100, 100, 255, "Performance capture {0} has been deleted (if it existed)", a_DBID);
	}

	private static void GetPerfCapture(CPlayer a_Player, CVehicle a_Vehicle, EntityDatabaseID a_DBID)
	{
		Database.Functions.Util.GetPerformanceCapture(a_DBID, async (CDatabaseStructurePerformanceCapture capture) =>
		{
			if (capture != null)
			{
				bool bQueued = LargeDataTransferManager.QueueOutgoingTransfer(a_Player, ELargeDataTransferType.PerfCapture, -1, System.Text.Encoding.ASCII.GetBytes(capture.strCombinedData), null, null, false, PerformanceCaptureConstants.EncryptionKey);
				if (bQueued)
				{
					string strAccountName = await Database.LegacyFunctions.GetUsernameFromAccount(capture.AccountID).ConfigureAwait(true);
					a_Player.PushChatMessageWithColor(EChatChannel.Global, 100, 100, 255, "Downloading performance capture {0} ({1})", capture.DBID, strAccountName);
				}
				else
				{
					a_Player.PushChatMessageWithColor(EChatChannel.Global, 100, 100, 255, "Could not download performance capture: Another transfer is already in progress");
				}
			}
			else
			{
				a_Player.PushChatMessageWithColor(EChatChannel.Global, 100, 100, 255, "Could not download performance capture: Capture with ID {0} does not exist", a_DBID);
			}
		});
	}

	private static void OnPerfCaptureTransfer_Started(LargeDataTransfer transfer)
	{

	}

	private static void OnPerfCaptureTransfer_Progress(LargeDataTransfer transfer)
	{

	}

	private static void OnPerfCaptureTransfer_Complete(LargeDataTransfer transfer, bool bSuccess)
	{
		string strData = System.Text.Encoding.ASCII.GetString(transfer.GetBytes());

		Database.Functions.Util.SavePerformanceCapture(transfer.m_Identifier, strData, async (EntityDatabaseID dbid) =>
		{
			string strAccountName = await Database.LegacyFunctions.GetUsernameFromAccount(transfer.m_Identifier).ConfigureAwait(true);
			HelperFunctions.Chat.SendMessageToScripters(Helpers.FormatString("[PERF CAPTURE] Capture from '{0}' is complete and now available with ID {1}! (Capture Size: {2:0.00}kb)", strAccountName, dbid, transfer.GetDataLengthDecrypted() / 1024.0f));
		});
	}

	public static void RegisterStatistic(MethodInfo methodInfo, long time_started, string strExtraInfo)
	{
		long end = DateTime.Now.Ticks;
		double time_diff = ((double)end - (double)time_started) / (double)TimeSpan.TicksPerMillisecond;
		string strModuleName = methodInfo.DeclaringType.FullName;

		string strFunctionName = methodInfo.Name;
		string strEntryName = Helpers.FormatString("{0}::{1}", strModuleName, strFunctionName);

		if (strExtraInfo != null)
		{
			strEntryName += Helpers.FormatString(" {0}", strExtraInfo);
		}

		if (time_diff > dThresholdForSlowWarn)
		{
			string strReportMessage = Helpers.FormatString("[{2} THREAD] Slow function: {0} = {1}ms", strEntryName, Math.Round(time_diff, 2), g_MainThreadChecker.IsOnMainThread() ? "MAIN" : "WORKER");
			CheckCooldownExpirations();
			if (!g_ReportCooldowns.ContainsKey(strEntryName))
			{
				DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.ServerPerfAlerts, strReportMessage);

				string strSentryMessage = Helpers.FormatString("[{1} THREAD] Slow function: {0}", strEntryName, g_MainThreadChecker.IsOnMainThread() ? "MAIN" : "WORKER");
				Core.RecordSlowdownToSentry(strSentryMessage, time_diff, Environment.StackTrace);

				g_ReportCooldowns.Add(strEntryName, DateTime.Now);
			}

#if DEBUG
			NAPI.Util.ConsoleOutput(strReportMessage);
#endif
		}
	}

	private static void CheckCooldownExpirations()
	{
		List<string> lstEntriesToRemove = new List<string>();
		foreach (var kvPair in g_ReportCooldowns)
		{
			Int64 millisecondsSinceLastReport = (Int64)(DateTime.Now - kvPair.Value).TotalMilliseconds;
			if (millisecondsSinceLastReport >= CooldownBetweenSentryReports)
			{
				lstEntriesToRemove.Add(kvPair.Key);
			}
		}

		foreach (string strEntryName in lstEntriesToRemove)
		{
			g_ReportCooldowns.Remove(strEntryName);
		}
	}

	public static void RegisterTimerPerf(Delegate timerDelegate, long time_started)
	{
		long end = DateTime.Now.Ticks;
		double time_diff = ((double)end - (double)time_started) / (double)TimeSpan.TicksPerMillisecond;

		string strModuleName = timerDelegate.GetMethodInfo().DeclaringType.FullName;
		string strFunctionName = timerDelegate.GetMethodInfo().Name;

		string strEntryName = Helpers.FormatString("{0}::{1}", strModuleName, strFunctionName);

		if (time_diff > dThresholdForSlowWarn)
		{
			string strReportMessage = Helpers.FormatString("[{2} THREAD] Slow timer: {0} = {1}ms", strEntryName, Math.Round(time_diff, 2), g_MainThreadChecker.IsOnMainThread() ? "MAIN" : "WORKER");
			CheckCooldownExpirations();
			if (!g_ReportCooldowns.ContainsKey(strEntryName))
			{
				DiscordBotIntegration.PushChannelMessage(EDiscordChannelIDs.ServerPerfAlerts, strReportMessage);

				string strSentryMessage = Helpers.FormatString("[{1} THREAD] Slow timer: {0}", strEntryName, g_MainThreadChecker.IsOnMainThread() ? "MAIN" : "WORKER");
				Core.RecordSlowdownToSentry(strSentryMessage, time_diff, Environment.StackTrace);

				g_ReportCooldowns.Add(strEntryName, DateTime.Now);
			}

#if DEBUG
			NAPI.Util.ConsoleOutput(strReportMessage);
#endif
		}
	}

	private static UInt64 g_TotalBytesSent = 0;
	private static UInt64 g_TotalBytesSentCompressed = 0;
	private static UInt64 g_TotalBytesReceived = 0;
	private static UInt64 g_TotalBytesReceivedCompressed = 0;
	public static void UpdateTotalBytesSentData(UInt32 a_sent, UInt32 a_sentcompressed)
	{
		g_TotalBytesSent += a_sent;
		g_TotalBytesSentCompressed += a_sentcompressed;
	}

	public static void GetTotalBytesSentData(out UInt64 sent, out UInt64 sentCompressed, out double sizeKB, out double sizeCompressedKB, out double sizeMB, out double sizeCompressedMB, out string strDisplayString)
	{
		sent = g_TotalBytesSent;
		sentCompressed = g_TotalBytesSentCompressed;

		sizeKB = ((double)g_TotalBytesSent / 1024.0);
		sizeCompressedKB = ((double)g_TotalBytesSentCompressed / 1024.0);
		sizeMB = ((double)g_TotalBytesSent / 1024.0) / 1024.0;
		sizeCompressedMB = ((double)g_TotalBytesSentCompressed / 1024.0) / 1024.0;

		strDisplayString = Helpers.FormatString("Total Bytes Sent: {0} bytes ({1:0.00} KB / {2:0.00} MB) to {3} compressed bytes ({4:0.00} KB / {5:0.00} MB) ({6}%))\n", g_TotalBytesSent, sizeKB, sizeMB, g_TotalBytesSentCompressed, sizeCompressedKB, sizeCompressedMB, ((1.0f - ((float)g_TotalBytesSentCompressed / (float)g_TotalBytesSent)) * 100.0f));
	}

	public static void UpdateTotalBytesReceivedData(UInt32 a_recv, UInt32 a_recvcompressed)
	{
		g_TotalBytesReceived += a_recv;
		g_TotalBytesReceivedCompressed += a_recvcompressed;
	}

	public static void GetTotalBytesReceivedData(out UInt64 recv, out UInt64 recvCompressed, out double sizeKB, out double sizeCompressedKB, out double sizeMB, out double sizeCompressedMB, out string strDisplayString)
	{
		recv = g_TotalBytesReceived;
		recvCompressed = g_TotalBytesReceivedCompressed;

		sizeKB = ((double)g_TotalBytesReceived / 1024.0);
		sizeCompressedKB = ((double)g_TotalBytesReceivedCompressed / 1024.0);
		sizeMB = ((double)g_TotalBytesReceived / 1024.0) / 1024.0;
		sizeCompressedMB = ((double)g_TotalBytesReceivedCompressed / 1024.0) / 1024.0;

		strDisplayString = Helpers.FormatString("Total Bytes Received: {0} bytes ({1:0.00} KB / {2:0.00} MB) to {3} compressed bytes ({4:0.00} KB / {5:0.00} MB) ({6}%))\n", g_TotalBytesReceived, sizeKB, sizeMB, g_TotalBytesReceivedCompressed, sizeCompressedKB, sizeCompressedMB, ((1.0f - ((float)g_TotalBytesReceivedCompressed / (float)g_TotalBytesReceived)) * 100.0f));
	}
}