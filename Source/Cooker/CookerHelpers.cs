using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public static class CookerHelpers
{
	public static uint HashFile(string strPath)
	{
		uint crc = FastCRC32.ComputeHash(File.ReadAllBytes(strPath));
		return crc;
	}

	public static uint HashString(string strInput)
	{
		uint crc = FastCRC32.ComputeHash(Encoding.UTF8.GetBytes(strInput));
		return crc;
	}

	public static byte[] GetBytes(string str)
	{
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static string GetString(byte[] bytes)
	{
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}

	public static string GetRuntimePath()
	{
		return Path.Combine("dotnet", "runtime");
	}

	public static bool IsFileServerside(string strFileName, CookerTypes.EAssetType assetType)
	{
		bool bServerSide = false;
		if (strFileName.Contains(".dll") || strFileName.Equals("meta.xml") || assetType == CookerTypes.EAssetType.ServerScript || assetType == CookerTypes.EAssetType.ServerDependency || assetType == CookerTypes.EAssetType.ServerAsset || assetType == CookerTypes.EAssetType.MapFile_Interior || assetType == CookerTypes.EAssetType.MapFile_Persistent || assetType == CookerTypes.EAssetType.DotNetConfigFile || assetType == CookerTypes.EAssetType.RageOverrideDLL)
		{
			bServerSide = true;
		}

		return bServerSide;
	}

	public static string GetArchiveName(string strPathWithExtension)
	{
		bool IsPlatformArchive = strPathWithExtension.ToLower().Contains("deps_") || strPathWithExtension.ToLower().Contains("_linux") || strPathWithExtension.ToLower().Contains("_windows") || strPathWithExtension.ToLower().Contains("_crossplatform") || strPathWithExtension.ToLower().Contains("_rageoverrides");

		string strFileName = Path.GetFileNameWithoutExtension(strPathWithExtension);
		string strPrefix = IsPlatformArchive ? "" : "owl_";
		return String.Format("{0}{1}", strPrefix, strFileName);
	}

	public static string GetMountPathForArchive(string ArchiveName, CookerTypes.EAssetType assetType, string outputFileName)
	{
		bool bIsServerSide = CookerHelpers.IsFileServerside(outputFileName, assetType);

		if (bIsServerSide)
		{
			return Path.Combine("dotnet", "resources", ArchiveName);
		}
		else
		{
			if (outputFileName.EndsWith(".cs"))
			{
				return Path.Combine("client_packages", "cs_packages", ArchiveName);
			}
			else
			{
				return Path.Combine("client_packages", ArchiveName);
			}
		}
	}

	public static void KillServerProcess()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			try
			{
				foreach (Process proc in Process.GetProcessesByName("server"))
				{
					proc.Kill();
				}
			}
			catch
			{

			}
		}
	}

	public static void CleanupTemp()
	{
		try
		{
			if (Directory.Exists(CookerSettings.TempFolder))
			{
				Directory.Delete(CookerSettings.TempFolder, true);
			}
		}
		catch { }

		try
		{
			if (File.Exists("meta.xml"))
			{
				File.Delete("meta.xml");
			}
		}
		catch { }

		try
		{
			foreach (string tempArchiveFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ar"))
			{
				File.Delete(Path.GetFileName(tempArchiveFile));
			}
		}
		catch { }
	}
}