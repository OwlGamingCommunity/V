using System.Runtime.InteropServices;

public static class CookerSettings
{
	public const bool UseSinglethread = false;
	public const int MaxThreads = 16;
	public const string HashCacheName = "HashCache";
	public const string HashCacheExtension = ".hashcache";
	public const string HashCacheExtensionFIM = ".hashcache.fim";
	public const string ArchiveExtension = ".ar";
	public const byte ArchiveVersion = 4;
	public const string TempFolder = "Cooker_Temp";
	public const string DotNetVersion = "netcoreapp3.1";
	private static bool g_bFastIterationMode = false;

	public static bool IsDebug()
	{
#if DEBUG
		return true;
#else
		return false;
#endif
	}

	public static string GetTargetName()
	{
		return IsDebug() ? "Debug" : "Release";
	}

	public static bool SetFastIterationMode(bool bEnabled)
	{
		return g_bFastIterationMode = bEnabled;
	}

	public static bool IsFastIterationMode()
	{
		return g_bFastIterationMode;
	}

	public static bool IsWindowsDeveloperModeEnabled()
	{
		if (IsWindows())
		{
			int? DevMode = (int?)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock", "AllowDevelopmentWithoutDevLicense", null);
			if (DevMode != null)
			{
				if (DevMode == 1)
				{
					return true;
				}

				return false;
			}
		}

		return false;
	}

	public static bool ShouldCookMaps()
	{
		return !IsFastIterationMode();
	}

	public static bool IsBuildServer()
	{
		return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	}

	public static bool IsLiveServer()
	{
		return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	}

	public static bool IsLinux()
	{
		return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	}

	public static bool IsWindows()
	{
		return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	}
}
