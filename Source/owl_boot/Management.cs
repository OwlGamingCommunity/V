using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

internal class CHTTPManagementServer : IDisposable
{
	public CHTTPManagementServer()
	{

	}

	public void Init()
	{
#if !DEBUG
		m_httpThread = new Thread(new ThreadStart(ThreadedHTTP));
		m_httpThread.Start();
#endif
	}

	private bool Authenticate(HttpListenerBasicIdentity identity, IPEndPoint source)
	{
		bool retVal = false;
		if (m_Accounts.ContainsKey(identity.Name))
		{
			AccountDetails accDetails = m_Accounts[identity.Name];
			if (accDetails.Password == identity.Password && source.Address.ToString() == "127.0.0.1" || source.Address.ToString() == accDetails.IP || accDetails.IP == "-1")
			{
				retVal = true;
			}
		}

		return retVal;
	}

	private static string GetHomeResponse()
	{
		string serverStatus = (Process.GetProcessesByName("server").Length == 0) ? "<font color='red'>OFFLINE</font>" : "<font color='green'>ONLINE</font>";
		string strStatus = String.Format("Server Status: {0} <br>&nbsp;&nbsp; <font color='red'><a href='restart'>RESTART</a></font>", serverStatus);
		strStatus += "<br><br><a href='serverlog'>Get Server Log</a>";
		strStatus += "<br><br><a href='serverexceptions'>Get Server Exceptions</a>";
		strStatus += "<br><br><a href='whitelist'>View/Edit Whitelist</a>";
		return strStatus;
	}

	private static string GetRestartResponse()
	{
		string strResp = String.Format("ARE YOU SURE? <br><br><font color='red'><a href='restart_confirm'>YES, RESTART THE SERVER</a></font> <br><br> <font color='green'><a href='home'>NO, GO HOME</a></font>");
		return strResp;
	}

	private static string GetRestartConfirmedResponse()
	{
		string strResp = String.Format("Server Restarted!<br><br><a href='home'>GO HOME</a>");
		return strResp;
	}

	private static string GetServerLog()
	{
		return Path.Combine("dotnet", "server_logs.txt");
	}

	class WhitelistItem
	{
		public string Serial;
		public string Name;
		public string AddedBy;
	}

	private static string GetWhiteList()
	{
		string strResp = "Whitelist does not exist.";

		if (File.Exists("whitelist.json"))
		{
			strResp = "<table><tr><th>Owner</th><th>Serial</th><th>Added By</th><th>Actions</th><tr>";
			List<WhitelistItem> allowedSerials = JsonConvert.DeserializeObject<List<WhitelistItem>>(File.ReadAllText("whitelist.json"));
			foreach (WhitelistItem serialData in allowedSerials)
			{
				strResp += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td><a href='/whitelist_remove/{1}'><font color='red'>REMOVE</font></a></td></tr>", serialData.Name, serialData.Serial, serialData.AddedBy);
			}

			strResp += "</table>";
		}

		strResp += "<br><br><a href='whitelist_add'>ADD NEW USER</a>";
		strResp += "<br><br><a href='home'>GO HOME</a>";

		return strResp;
	}

	private static string GetWhiteListRemove(string[] strParams)
	{
		string strResp = "";
		if (strParams.Length == 1)
		{
			// Try to find and remove
			bool bWasFound = false;
			List<WhitelistItem> allowedSerialsNew = new List<WhitelistItem>();
			List<WhitelistItem> allowedSerials = JsonConvert.DeserializeObject<List<WhitelistItem>>(File.ReadAllText("whitelist.json"));
			foreach (WhitelistItem serialData in allowedSerials)
			{
				if (serialData.Serial != strParams[0])
				{
					allowedSerialsNew.Add(serialData);
				}
				else
				{
					bWasFound = true;
				}
			}

			File.WriteAllText("whitelist.json", JsonConvert.SerializeObject(allowedSerialsNew));

			strResp = bWasFound ? "Access Revoked!" : "Serial was not found";
		}
		else
		{
			strResp = "Invalid Parameters";
		}

		strResp += "<br><br><a href='../whitelist'>GO BACK</a>";

		return strResp;
	}

	private static string GetWhiteListAdd()
	{
		string strResp = "<form action='/whitelist_add_perform' method='post'>Serial:<input type='text' name='serial'><br> Name (Display Name - Doesn't have to match their Username):<input type='text' name='displayname'><br><br><input type='submit' value='Add'>";
		strResp += "<br><br><a href='whitelist'>GO BACK</a>";
		return strResp;
	}

	private static string GetWhiteListAddPerform(Dictionary<string, string> postData, HttpListenerBasicIdentity requestingIdentity)
	{
		string strResp = "";
		if (postData.ContainsKey("serial") && postData.ContainsKey("displayname"))
		{
			string serial = postData["serial"];
			string displayName = postData["displayname"];

			bool bWasFound = false;
			// does the serial already exist
			List<WhitelistItem> allowedSerials = JsonConvert.DeserializeObject<List<WhitelistItem>>(File.ReadAllText("whitelist.json"));
			foreach (WhitelistItem serialData in allowedSerials)
			{
				if (serialData.Serial == serial)
				{
					bWasFound = true;
				}
			}

			if (bWasFound)
			{
				strResp = String.Format("Serial {0} already exists!", serial);
			}
			else
			{
				strResp = String.Format("Added {0} ({1})", serial, displayName);
				allowedSerials.Add(new WhitelistItem() { Serial = serial.ToLower(), Name = displayName, AddedBy = requestingIdentity.Name });
				File.WriteAllText("whitelist.json", JsonConvert.SerializeObject(allowedSerials));
			}
		}
		else
		{
			strResp = "Form Error!";
		}

		strResp += "<br><br><a href='whitelist'>GO BACK</a>";
		return strResp;
	}

	private static string GetServerExceptions()
	{
		try
		{
			if (File.Exists("exceptions.zip"))
			{
				File.Delete("exceptions.zip");
			}

			// create zip
			using (ZipArchive zip = ZipFile.Open("exceptions.zip", ZipArchiveMode.Create))
			{
				foreach (string file in Directory.GetFiles("exceptions"))
				{
					zip.CreateEntryFromFile(file, Path.GetFileName(file));
				}

				string strRageExceptionsPath = Path.Combine("dotnet", "server_exceptions.txt");
				if (File.Exists(strRageExceptionsPath))
				{
					zip.CreateEntryFromFile(strRageExceptionsPath, Path.GetFileName(strRageExceptionsPath));
				}

			}
		}
		catch
		{

		}

		return "exceptions.zip";
	}

	private static string GetRequestPostData(HttpListenerRequest request)
	{
		if (!request.HasEntityBody)
			return null;
		using (System.IO.Stream body = request.InputStream)
		{
			using (System.IO.StreamReader reader = new StreamReader(body, request.ContentEncoding))
			{
				return reader.ReadToEnd();
			}
		}
	}

	private async void ThreadedHTTP()
	{
		server = new HttpListener();

		server.Prefixes.Add("http://*:54005/");

		foreach (var prefix in server.Prefixes)
		{
			Console.WriteLine("Listening on: {0}", prefix);
		}

		server.AuthenticationSchemes = AuthenticationSchemes.Basic;

		server.Start();

		while (Thread.CurrentThread.IsAlive)
		{
			HttpListenerContext context = await server.GetContextAsync().ConfigureAwait(true);
			HttpListenerRequest request = context.Request;
			HttpListenerResponse response = context.Response;

			string strPostDataCombined = GetRequestPostData(request);
			Dictionary<string, string> postData = new Dictionary<string, string>();

			if (!String.IsNullOrEmpty(strPostDataCombined))
			{
				string[] strKeyValuePairs = null;
				// Do we have multiple params or just one?
				if (strPostDataCombined.Contains('&'))
				{
					strKeyValuePairs = strPostDataCombined.Split('&');
				}
				else
				{
					strKeyValuePairs = new string[] { strPostDataCombined };
				}

				// Split the kv pairs
				foreach (string strKvPair in strKeyValuePairs)
				{
					string[] strKvPairSplit = strKvPair.Split('=');
					if (strKvPairSplit.Length == 2)
					{
						postData.Add(strKvPairSplit[0], strKvPairSplit[1]);
					}
				}
			}
			string strRequestPath = context.Request.Url.LocalPath.ToLower().Substring(1);

			bool bSuccess = false;
			string strBody = "";

			HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;
			IPEndPoint source = request.RemoteEndPoint;

			string[] strRequestPathSplit = strRequestPath.ToLower().Split('/');
			string strRequestName = strRequestPathSplit[0];
			string[] strParameters = strRequestPathSplit.Skip(1).Take(strRequestPathSplit.Length - 1).ToArray();

			if (Authenticate(identity, source))
			{
				bool bIsFile = false;
				string sResponse = "";

				if (String.IsNullOrEmpty(strRequestName) || strRequestName == "home")
				{
					sResponse = GetHomeResponse();
				}
				else if (strRequestName == "restart")
				{
					sResponse = GetRestartResponse();
				}
				else if (strRequestName == "restart_confirm")
				{
					// Kill any running process
					foreach (var process in Process.GetProcessesByName("server"))
					{
						process.Kill();
					}

					// Run a new server
					// NOTE: Nothing to do here, restarter will take care of it automatically

					sResponse = GetRestartConfirmedResponse();
				}
				else if (strRequestName == "serverlog")
				{
					bIsFile = true;
					sResponse = GetServerLog();
				}
				else if (strRequestName == "serverexceptions")
				{
					bIsFile = true;
					sResponse = GetServerExceptions();
				}
				else if (strRequestName == "whitelist")
				{
					sResponse = GetWhiteList();
				}
				else if (strRequestName == "whitelist_remove")
				{
					sResponse = GetWhiteListRemove(strParameters);
				}
				else if (strRequestName == "whitelist_add")
				{
					sResponse = GetWhiteListAdd();
				}
				else if (strRequestName == "whitelist_add_perform")
				{
					sResponse = GetWhiteListAddPerform(postData, identity);
				}

				if (sResponse != null)
				{
					if (!bIsFile)
					{
						string strPlayerCount = String.Empty;
						string strFPS = String.Empty;
						string strUptime = String.Empty;
						Boot.Program.GetServerStats(out strPlayerCount, out strFPS, out strUptime);
						strBody = String.Format("{0}<h1>Hello {3}!</h1><h3>Player Count: {4}<br>Server FPS: {5}<br>Uptime:{6}</h3><br>{1}{2}", "<html><body>", sResponse, "</body></html>", identity.Name, strPlayerCount, strFPS, strUptime);
					}
					else
					{
						try
						{
							string path = sResponse;
							using (FileStream fs = File.OpenRead(path))
							{
								string filename = Path.GetFileName(path);
								//response is HttpListenerContext.Response...
								response.ContentLength64 = fs.Length;
								response.SendChunked = false;
								response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
								response.AddHeader("Content-disposition", "attachment; filename=" + filename);

								byte[] buf = new byte[64 * 1024];
								int read;
								using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
								{
									while ((read = fs.Read(buf, 0, buf.Length)) > 0)
									{
										bw.Write(buf, 0, read);
										bw.Flush(); //seems to have no effect
									}

									bw.Close();
								}

								response.StatusCode = (int)HttpStatusCode.OK;
								response.StatusDescription = "OK";
								response.OutputStream.Close();
								continue;
							}
						}
						catch
						{
							strBody = "ERROR";
							bIsFile = false;
						}
					}

					bSuccess = true;
				}
				else
				{
					// unsupported type
					response.StatusCode = 400; // Bad Request
				}
			}
			else
			{
				// Unauthorized
				response.StatusCode = 401;
			}

			// TODO: On failure, pretend to be offline/not listening?
			byte[] buffer = Encoding.UTF8.GetBytes(bSuccess ? strBody : m_strDefaultBody);

			response.ContentLength64 = buffer.Length;
			Stream st = response.OutputStream;
			st.Write(buffer, 0, buffer.Length);

			context.Response.Close();
		}
	}

	~CHTTPManagementServer()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		m_httpThread.Abort();
		server.Abort();
		server.Close();
		server = null;
	}

	private HttpListener server = null;
	private Thread m_httpThread = null;


	private const string m_strDefaultBody = "400 Bad Request";

	struct AccountDetails
	{
#pragma warning disable CS0649
		public string Password;
		public string IP;
#pragma warning restore CS0649
	}

	// TODO_POST_LAUNCH: If we keep this, we should use IP auth when we move off of Azure since we lose that extra FW level security
	Dictionary<string, AccountDetails> m_Accounts = new Dictionary<string, AccountDetails>()
	{
	};
}