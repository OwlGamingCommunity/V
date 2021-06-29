using GTANetworkAPI;
using System;
using System.Net;
using System.Threading;

internal class CRequestHandler_Shutdown : CRequestHandler
{
	public CRequestHandler_Shutdown() : base("Shutdown", true)
	{

	}

	public override string Get(HttpListenerRequest request)
	{
		Semaphore done = new Semaphore(0, 1);
		NAPI.Task.Run(() =>
		{
			HelperFunctions.Chat.SendServerMessage("Server will save and restart for an update in 60 seconds.", r: 255, g: 0, b: 0);
			NetworkEventSender.SendNetworkEvent_ScriptUpdate_ForAll_IncludeEveryone();

			MainThreadTimerPool.CreateGlobalTimer((object[] parameters) =>
			{
				VehiclePool.SaveAll();
				PlayerPool.SaveAll();

				foreach (string resource in NAPI.Resource.GetRunningResources())
				{
					if (resource != "owl_http" && resource != "owl_mysql")
					{
						Console.WriteLine("Shutting down {0}..", resource);
						NAPI.Resource.StopResource(resource);
					}
				}

				while (Database.ThreadedMySQL.HasPendingQueries())
				{
					System.Threading.Thread.Sleep(1);
					Database.ThreadedMySQL.Tick();
				}

				done.Release();
			}, 60000, 1);
		});
		done.WaitOne();
		// Safe to ignore any cancelled operation exceptions, they are handled internally by sentry but
		// rages exception handler catches them anyways.
		Sentry.SentrySdk.Close();
		return "Resource save and shutdown complete";
	}
}
