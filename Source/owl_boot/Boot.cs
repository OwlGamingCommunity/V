
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace Boot
{
	internal class Program
	{
		//private static CHTTPManagementServer g_HTTPServer = new CHTTPManagementServer();


		private static List<string> g_LogLines = new List<string>();
		private static System.Timers.Timer g_DumpLogTimer = null;
		private static string g_strLogFilename = null;

		private static void Main(string[] args)
		{
			// Assembly unloading and process exit have a lot of issues with killing after 2 seconds. So we use SIGINT 
			// And we call e.Cancel so that it doesn't kill the proc after 2 seconds.
			System.Console.CancelKeyPress += async (s, e) =>
			{
				e.Cancel = true;
				Console.WriteLine("Received shutdown signal");
				if (g_Process != null && !g_Process.HasExited)
				{
					try
					{
						var client = new HttpClient();

						// TODO_GITHUB: You should set the environment variable below
						var password = Environment.GetEnvironmentVariable("HTTP_SERVER_PASS") ?? "default";
						var byteArray = Encoding.ASCII.GetBytes(String.Format("ucp:{0}", password));
						client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
						client.Timeout = TimeSpan.FromMinutes(1);
						await client.GetAsync(new Uri("http://localhost:9001/Shutdown")).ConfigureAwait(true);

						g_Process.Kill();
						g_Process.Close();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}

				Environment.Exit(0);
			};

			//g_HTTPServer.Init();
			// Docker container runs properly at it's current directory
#if DEBUG
			Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
			Directory.SetCurrentDirectory(@"..");
#endif

			UnarchiverHelpers.InitialSetup();
			DistributedCooker distributedCooker = new DistributedCooker();

			// queue work
			string[] strArchives = System.IO.Directory.GetFiles(System.IO.Path.Combine("Packages"), "*.ar");
			foreach (string strArchive in strArchives)
			{
				distributedCooker.QueueJob(new Job_Unarchive(strArchive));

			}
			// output data
			int numTasks = distributedCooker.GetNumQueuedTasks();

			DateTime dtStart = DateTime.Now;
			bool bDoneTasks = false;
			distributedCooker.SetCallbackOnCompleteAllTasks((bool bErrors, List<string> strErrorMessages) =>
			{
				Int64 ms = (Int64)(DateTime.Now - dtStart).TotalMilliseconds;

				if (bErrors)
				{
					Console.WriteLine("========== Unarchive FAILED! ({0} tasks in {1} milliseconds) ==========\n", numTasks, ms);
					Console.WriteLine("\tErrors:");

					foreach (string strErrorMsg in strErrorMessages)
					{
						Console.WriteLine("\t\t{0}", strErrorMsg);
					}

					Console.WriteLine("\n\n");
				}
				else
				{
					Console.WriteLine("========== Unarchive Completed Succesfully! ({0} tasks in {1} milliseconds) ==========\n", numTasks, ms);
				}

				bDoneTasks = true;
			});

			while (!bDoneTasks)
			{
				distributedCooker.Tick();
			}

			//string[] strArchives = System.IO.Directory.GetFiles(System.IO.Path.Combine("Packages"), "*.ar");
			//Unarchiver.ArchiveOperation.UnArchive(strArchives);

			g_DumpLogTimer = new System.Timers.Timer();
			g_DumpLogTimer.Elapsed += DumpLog;
			g_DumpLogTimer.Interval = 60000;
			g_DumpLogTimer.Enabled = true;

#if !DEBUG
			RunServer();
#endif
		}


		private static void DumpLog(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				File.WriteAllLinesAsync(g_strLogFilename, g_LogLines.ToArray());
			}
			catch
			{

			}
		}

		private static Process g_Process = null;

		public static void GetServerStats(out string strPlayerCount, out string strFps, out string strUptime)
		{
			if (g_Process != null)
			{
				if (File.Exists("serverstats"))
				{
					string[] strData = File.ReadAllLines("serverstats");
					if (strData.Length == 3)
					{
						strPlayerCount = strData[0];
						strFps = strData[1];
						strUptime = strData[2];
						return;
					}
				}
			}

			strPlayerCount = "Server not running";
			strFps = strPlayerCount;
			strUptime = strPlayerCount;
		}

		private static void RunServer()
		{
			g_LogLines.Clear();
			Random rand = new Random();
			g_strLogFilename = String.Format("LOG_{0}_{1}.txt", rand.Next(), DateTime.Now.ToString());

			g_Process = new Process();
			g_Process.StartInfo.FileName = String.Format("ragemp-server{0}", RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "");

#if !DEBUG
			g_Process.StartInfo.RedirectStandardOutput = true;
			g_Process.StartInfo.RedirectStandardError = true;
			g_Process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
			g_Process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
			g_Process.StartInfo.UseShellExecute = false;
#else
			g_Process.StartInfo.UseShellExecute = true;
#endif
			g_Process.Start();

#if !DEBUG
			g_Process.BeginOutputReadLine();
			g_Process.BeginErrorReadLine();
#endif
#if !DEBUG
			while (!g_Process.HasExited) {
				System.Threading.Thread.Sleep(500);
			}

			// find any stragglers
			foreach (Process proc in Process.GetProcessesByName("DiscordBot"))
			{
				proc.Kill();
			}
			
			Console.WriteLine("---> Writing log to {0}", g_strLogFilename);
			DumpLog(null, null);
			Console.WriteLine("---> Starting new server");
			
			RunServer();
#endif
		}

		private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			Console.WriteLine(outLine.Data);
			g_LogLines.Add(outLine.Data);
		}
	}
}
