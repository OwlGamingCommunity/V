public class HTTPInstance
{
	public HTTPInstance()
	{
		g_HTTPServer = new CHTTPServer("UCP Bridge", 9001);

		// setup auth
		// TODO_GITHUB: You should set the environment variable below
		g_HTTPServer.SetAuthMethod_BasicAuth("ucp", System.Environment.GetEnvironmentVariable("HTTP_SERVER_PASS") ?? "default");

		// register handlers
		g_HTTPServer.RegisterHandler<CRequestHandler_KickPlayerByID>();
		g_HTTPServer.RegisterHandler<CRequestHandler_VehicleDefinitionsJSON>();
		g_HTTPServer.RegisterHandler<CRequestHandler_DeleteItem>();
		g_HTTPServer.RegisterHandler<CRequestHandler_TransferAssets>();
		g_HTTPServer.RegisterHandler<CRequestHandler_ServerStats>();
		g_HTTPServer.RegisterHandler<CRequestHandler_Shutdown>();
	}

	private static CHTTPServer g_HTTPServer = null;
}