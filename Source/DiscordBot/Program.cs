//#define USE_DISCORD_IN_DEBUG
using System;
using System.Threading;

namespace owl_discord
{
	class Program
	{
		private static DiscordBot g_BotInstance = null;
		private static CHTTPServer g_HTTPServer = null;
		private static CRestClient g_RestClient = null;

		public static DiscordBot GetDiscordBot()
		{
			return g_BotInstance;
		}

		public static CRestClient GetRestClient()
		{
			return g_RestClient;
		}

		static void Main(string[] args)
		{
			g_BotInstance = new DiscordBot();

			g_HTTPServer = new CHTTPServer("DiscordBot", 9999);

			// TODO_GITHUB: You should set the environment variable below
			g_HTTPServer.SetAuthMethod_Token(System.Environment.GetEnvironmentVariable("HTTP_SERVER_PASS") ?? String.Empty);
			g_HTTPServer.RegisterHandler<CRequestHandler_PushRawChannelMessage>();
			g_HTTPServer.RegisterHandler<CRequestHandler_PushDM>();
			g_HTTPServer.RegisterHandler<CRequestHandler_GetDiscordUsernameFromID>();
			g_HTTPServer.RegisterHandler<CRequestHandler_UpdateBotStatus>();

			// TODO_GITHUB: You should set the environment variable below
			g_RestClient = new CRestClient("127.0.0.1", 9998, System.Environment.GetEnvironmentVariable("HTTP_SERVER_PASS") ?? String.Empty);

			Console.WriteLine("DiscordBot: Initialized and waiting for requests");
			while (true)
			{
				Thread.Sleep(100);
				g_RestClient.MainThreadTick();
			}
		}
	}
}