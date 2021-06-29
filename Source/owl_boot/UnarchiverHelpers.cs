using System.IO;

public static class UnarchiverHelpers
{
	private static void CleanUp()
	{
		// delete old unarchives
		foreach (string dir in Directory.GetDirectories(Path.Combine("dotnet", "resources")))
		{
			string dirName = Path.GetFileName(dir);
			if (dirName != "owl_boot")
			{
				try
				{
					DeleteDirectory(dir);
				}
				catch
				{

				}
			}
		}
	}

	public static void DeleteDirectory(string target_dir)
	{
		if (Directory.Exists(target_dir))
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(target_dir, false);
		}
	}

	public static bool InitialSetup()
	{
		// Recreate directory
		try { Directory.CreateDirectory("client_packages"); } catch { };
		try { Directory.CreateDirectory(Path.Combine("client_packages", "cs_packages")); } catch { };

		// delete client_packages
		// TODO_COOKER: Make this a task or faster
		foreach (string dirName in Directory.GetDirectories("client_packages"))
		{
			if (dirName.ToString() != "game_resources")
			{
				try { DeleteDirectory(Path.Combine("client_packages", dirName)); } catch { };
			}
		}

		string strTargetDirectory = Path.Combine("dotnet", "resources");

		if (!Directory.Exists(strTargetDirectory))
		{
			Directory.CreateDirectory(strTargetDirectory);
		}

		foreach (string dirName in Directory.GetDirectories(strTargetDirectory))
		{
			if (dirName.ToString() != "owl_boot")
			{
				try { DeleteDirectory(Path.Combine("dotnet", "resources", dirName)); } catch { };
			}
		}

		bool success = true;

		CleanUp();

		CopyBridgeSettingsXML();

		// Write root index, we just hardcode write js_bridge because its the only place we should have ANY js
		File.WriteAllText(Path.Combine("client_packages", "index.js"), "require('owl_jsbridge.client/index.js');");

		if (!success)
		{
			CleanUp();
		}

		return success;
	}

	private static void CopyBridgeSettingsXML()
	{
		File.Copy(Path.Combine("Packages", "settings.xml"), Path.Combine("dotnet", "settings.xml"), true);
	}
}