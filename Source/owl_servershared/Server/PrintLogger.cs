using GTANetworkAPI;
using System;

public enum ELogSeverity
{
	DATABASE_DEBUG,
	DEBUG,
	PERFORMANCE,
	LOW,
	MEDIUM,
	HIGH,
	ERROR,
}

public static class PrintLogger
{
#if DEBUG
	const ELogSeverity g_MinLogSeverity = ELogSeverity.MEDIUM;
#else
	const ELogSeverity g_MinLogSeverity = ELogSeverity.MEDIUM;
#endif

	public static void LogMessage(ELogSeverity logSeverity, string strFormat, params object[] strParams)
	{
		if (logSeverity >= g_MinLogSeverity)
		{
			string strPrefix = "";
			if (logSeverity == ELogSeverity.ERROR)
			{
				strPrefix = "[ERROR] ";
				Console.ForegroundColor = ConsoleColor.Red;
			}
			else if (logSeverity == ELogSeverity.DEBUG)
			{
				strPrefix = "[DEBUG] ";
				Console.ForegroundColor = ConsoleColor.Cyan;
			}
			else if (logSeverity == ELogSeverity.PERFORMANCE)
			{
				strPrefix = "[PERFORMANCE] ";
				Console.ForegroundColor = ConsoleColor.Red;
			}

			NAPI.Util.ConsoleOutput(Helpers.FormatString("{0}{1}", strPrefix, strFormat), strParams);

			if (logSeverity == ELogSeverity.ERROR)
			{
				NAPI.Util.ConsoleOutput("CALL STACK: {0}", Environment.StackTrace);

#if DEBUG
				if (!System.Diagnostics.Debugger.IsAttached)
				{
					NAPI.Util.ConsoleOutput("Press any key to continue...");
					Console.Read();
				}
#endif
			}

			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}