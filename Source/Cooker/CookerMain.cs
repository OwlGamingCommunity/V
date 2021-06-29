using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace CookerMain
{
	internal class Program
	{
		private static void QueueMaps(DistributedCooker cooker)
		{
			if (!CookerSettings.IsFastIterationMode())
			{
				// TODO_COOKER: This cooks everytime, we should use the hash cache to avoid that
				Job_CookMap.GetMapDirectory();
				Job_CookMap.CreateMapDirectory(true);

				CookerTypes.AssetDescriptor assetDesc = Newtonsoft.Json.JsonConvert.DeserializeObject<CookerTypes.AssetDescriptor>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "..", "Descriptors", "maps.json")));
				foreach (CookerTypes.AssetFile a in assetDesc.Files)
				{
					cooker.QueueJob(new Job_CookMap(a));
				}
			}
		}

		private static void QueueCookSettings(DistributedCooker g_Cooker)
		{
			g_Cooker.QueueJob(new Job_CookSettingsXML());
		}

		private static void QueueClientCodeGen(DistributedCooker g_Cooker)
		{
			string[] strCodeGenItems = new string[]
			{
				"ItemData",
				"VehicleData",
				"GangtagSprites",
				"TattooData",
				"FurnitureData",
				"HairTattooData"
			};

			foreach (string strCodeGenItem in strCodeGenItems)
			{
				bool bNeedsCodeGen = true;

				if (CookerSettings.IsFastIterationMode())
				{
					string strInputFileName = String.Format("{0}.json", strCodeGenItem);
					string inputPath = Path.Combine(Directory.GetCurrentDirectory(), "owl_gamedata", strInputFileName);

					uint newHash = CookerHelpers.HashFile(inputPath);
					string strHashCachePath = Path.Combine(Directory.GetCurrentDirectory(), CookerSettings.HashCacheName, strCodeGenItem + CookerSettings.HashCacheExtensionFIM);

					if (!File.Exists(strHashCachePath))
					{
						Console.WriteLine("FIM: CodeGenning '{0}' because descriptor hash is not present.", strCodeGenItem);
						bNeedsCodeGen = true;
					}
					else
					{
						uint oldHash = Convert.ToUInt32(File.ReadAllText(strHashCachePath));

						if (oldHash != newHash)
						{
							Console.WriteLine("FIM: CodeGenning '{0}' because the descriptor has changed.", strCodeGenItem);
							bNeedsCodeGen = true;
						}
						else
						{
							//Console.WriteLine("FIM: Skipping '{0}' because source JSON file has not changed.", strCodeGenItem);
							bNeedsCodeGen = false;
						}
					}
				}

				if (bNeedsCodeGen)
				{
					g_Cooker.QueueJob(new Job_ClientCodeGen(strCodeGenItem));
				}
			}
		}

		private static void QueueScriptArchives(DistributedCooker g_Cooker)
		{
			bool bIsFIM = CookerSettings.IsFastIterationMode();

			foreach (string descriptorfile in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "..", "Descriptors"), "*.json"))
			{
				bool bNeedsCook = true;

				// in FIM, since we use symlinks we don't care about file changes, we only need to 'cook' if the descriptor changed
				// we do this here instead of in the job because why even waste our thread pool time processing nothing
				if (bIsFIM)
				{
					string strDescriptorPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Descriptors", descriptorfile);
					string strArchiveName = CookerHelpers.GetArchiveName(strDescriptorPath);
					uint newHash = CookerHelpers.HashFile(strDescriptorPath);
					string strHashCachePath = Path.Combine(Directory.GetCurrentDirectory(), CookerSettings.HashCacheName, strArchiveName + CookerSettings.HashCacheExtensionFIM);

					// TODO_COOKER: We have to still always copy/cook clientside files in FIM, we're working on fixing this
					if (strArchiveName.EndsWith(".client") || strArchiveName.EndsWith("_shared"))
					{
						bNeedsCook = true;
					}
					else if (!File.Exists(strHashCachePath))
					{
						Console.WriteLine("FIM: Fake Cooking '{0}' because descriptor hash is not present.", strArchiveName);
						bNeedsCook = true;
					}
					else
					{
						uint oldHash = Convert.ToUInt32(File.ReadAllText(strHashCachePath));

						if (oldHash != newHash)
						{
							Console.WriteLine("FIM: Fake Cooking '{0}' because the descriptor has changed.", strArchiveName);
							bNeedsCook = true;
						}
						else
						{
							//Console.WriteLine("FIM: Skipping '{0}' because descriptor has not changed.", strArchiveName);
							bNeedsCook = false;
						}
					}
				}

				if (bNeedsCook)
				{
					g_Cooker.QueueJob(new Job_BuildArchive(descriptorfile));
				}
			}
		}

		private static void QueueCodeAnalysis(DistributedCooker g_Cooker)
		{
			if (!CookerSettings.IsFastIterationMode())
			{
				foreach (string descriptorfile in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "..", "Descriptors"), "*.json"))
				{
					string strSourceDir = CookerHelpers.GetArchiveName(descriptorfile);
					g_Cooker.QueueJob(new Job_RunCodeAnalysis(strSourceDir));
				}
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

		enum SymbolicLink
		{
			File = 0,
			Directory = 1,
			SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE = 2
		}

		private static void InitialSetup()
		{
			// set initial dir
			if (CookerSettings.IsWindows())
			{
				Directory.SetCurrentDirectory(Path.Combine("..", ".."));
			}
			else if (CookerSettings.IsLinux())
			{
				Directory.SetCurrentDirectory(Path.Combine("Source", "Cooker", "bin"));
			}

			Directory.CreateDirectory(CookerSettings.TempFolder);

			string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "..");
			if (!Directory.Exists(outputFolder))
			{
				Directory.CreateDirectory(outputFolder);
			}

			// create packages folder
			string packagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Output", CookerSettings.GetTargetName(), "Packages");

			if (!Directory.Exists(packagesFolder))
			{
				Directory.CreateDirectory(packagesFolder);
				Console.WriteLine("\tCreating Packages Directory: {0}", packagesFolder);
			}

			Directory.SetCurrentDirectory(outputFolder);

			Directory.CreateDirectory(CookerSettings.HashCacheName);

			// These things are always junctioned, even in full cooks (on windows + debug), not only for FIM

			bool bNeedsPermanentWindowsJunctions = (CookerSettings.IsDebug() && CookerSettings.IsWindows()) || CookerSettings.IsFastIterationMode();
			if (bNeedsPermanentWindowsJunctions)
			{
				// Create junction for debug maps
				string strDebugMapsDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", "Debug", "cooked_maps");
				CreateSymbolicLink(strDebugMapsDir, Job_CookMap.GetMapDirectory(), SymbolicLink.Directory | SymbolicLink.SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE);

				// create junction for debug mods
				string strModsRelease = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", "Release", "client_packages", "game_resources");
				string strModsDebug = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", "Debug", "client_packages", "game_resources");
				CreateSymbolicLink(strModsDebug, strModsRelease, SymbolicLink.Directory | SymbolicLink.SYMBOLIC_LINK_FLAG_ALLOW_UNPRIVILEGED_CREATE);

				// NOTE: Debug will not work on linux due to the above
			}
		}

		private static void Main(string[] args)
		{
#if !DEBUG
			// Release never supports FIM
			CookerSettings.SetFastIterationMode(false);
#else
			bool bIsFIM = false;
			foreach (string arg in args)
			{
				string[] splitArgs = arg.Split("=");
				if (splitArgs.Length == 2)
				{
					if (splitArgs[0] == "-BuildConfig" && splitArgs[1] != "Debug Full Cook") // only if not full cook config
					{
						bIsFIM = true;
					}
				}
			}

			CookerSettings.SetFastIterationMode(bIsFIM);

#endif

			if (!CookerSettings.IsBuildServer())
			{
				CookerHelpers.KillServerProcess();
			}

			if (CookerSettings.IsWindows() && !CookerSettings.IsWindowsDeveloperModeEnabled())
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ERROR: Windows Developer Mode is not enabled. Goto Start -> Developer Mode -> Check the first checkbox\nPress and key to exit...");
				Console.ReadKey(true);
				return;
			}

			// initial setup
			InitialSetup();

			// cooker
			DistributedCooker g_Cooker = new DistributedCooker();

			// register our jobs
			g_Cooker.InitTimeForJob(typeof(Job_BuildArchive));
			g_Cooker.InitTimeForJob(typeof(Job_ClientCodeGen));
			g_Cooker.InitTimeForJob(typeof(Job_CookMap));
			g_Cooker.InitTimeForJob(typeof(Job_CookSettingsXML));
			g_Cooker.InitTimeForJob(typeof(Job_RunCodeAnalysis));
			g_Cooker.InitTimeForJob(typeof(Job_Unarchive));

			// queue work
			QueueCookSettings(g_Cooker);
			QueueMaps(g_Cooker);
			QueueClientCodeGen(g_Cooker);
			QueueScriptArchives(g_Cooker);
			QueueCodeAnalysis(g_Cooker);

			// output data
			int numTasks = g_Cooker.GetNumQueuedTasks();

			DateTime dtStart = DateTime.Now;
			bool bDoneTasks = false;
			bool bSuccess = false;
			g_Cooker.SetCallbackOnCompleteAllTasks((bool bErrors, List<string> strErrorMessages) =>
			{
				Int64 ms = (Int64)(DateTime.Now - dtStart).TotalMilliseconds;

				// CODE ANALYSIS OUTPUT
				CodeAnalysis.ShowResults();
				// END CODE ANALYSIS OUTPUT

				if (bErrors)
				{
					Console.WriteLine("========== Cook FAILED! ({0} tasks in {1} milliseconds) ==========\n", numTasks, ms);

					Console.WriteLine("\tErrors:");

					foreach (string strErrorMsg in strErrorMessages)
					{
						Console.WriteLine("\t\tERROR: {0}", strErrorMsg);
					}

					Console.WriteLine("\n\n");

					bSuccess = false;
				}
				else
				{
					bSuccess = true;
					Console.WriteLine("========== Cook Completed Succesfully! ({0} tasks in {1} milliseconds) ==========\n", numTasks, ms);

					if (!CookerSettings.IsBuildServer() && CookerSettings.IsDebug())
					{
						string debugFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", "Debug");
						string debugOptimizationsPath = Path.Combine(debugFolder, "optimizationdictionary.dat");

						if (!Directory.Exists(debugFolder))
						{
							Directory.CreateDirectory(debugFolder);
						}

						if (!File.Exists(debugOptimizationsPath))
						{
							File.Create(debugOptimizationsPath).Close();
						}

						string releaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", "Release");
						string releaseOptimizationsPath = Path.Combine(releaseFolder, "optimizationdictionary.dat");

						if (!Directory.Exists(releaseFolder))
						{
							Directory.CreateDirectory(releaseFolder);
						}


						File.Copy(debugOptimizationsPath, releaseOptimizationsPath, true);
					}
				}

				bDoneTasks = true;
			});

			while (!bDoneTasks)
			{
				g_Cooker.Tick();
			}

			if (CookerSettings.IsFastIterationMode())
			{
				// Write root index, we just hardcode write js_bridge because its the only place we should have ANY js
				File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", CookerSettings.GetTargetName(), "client_packages", "index.js"), "require('owl_jsbridge.client/index.js');");
			}

			if (!CookerSettings.IsBuildServer())
			{
				if (!CookerSettings.IsFastIterationMode())
				{
					// Are we in debug mode? unarchive the files immediately so we can just hit F5 to run the server directly
					// This will allow us to simply launch server.exe
					Console.WriteLine("\nDevelopment Mode Enabled. Unpackaging archives directly to enable direct-run");
					Directory.SetCurrentDirectory(@"..\Output\Debug\");
					Process process = new Process();
					process.StartInfo.FileName = "cmd.exe";
					process.StartInfo.Arguments = String.Format(@"/c dotnet {0}\owl_boot.dll", CookerSettings.DotNetVersion);
					process.Start();
					process.WaitForExit();
				}
			}

			CookerHelpers.CleanupTemp();
			Environment.Exit(bSuccess ? 0 : 1);
			return;
		}

		private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			Console.WriteLine(outLine.Data);
		}
	}
}
