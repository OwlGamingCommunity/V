using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

public class CHTTPServer : IDisposable
{
	public enum EAuthMethod
	{
		None,
		Token,
		Account
	}

	private HttpListener server = null;
	private Thread m_httpThread = null;

	private int m_Port = 0;
	private EAuthMethod m_AuthMethod = EAuthMethod.None;
	private string m_strServiceName = String.Empty;
	private string m_strToken = String.Empty;
	private string m_strUsername = String.Empty;
	private string m_strPassword = String.Empty;

	public CHTTPServer(string strDisplayName, int port)
	{
		m_Port = port;
		m_strServiceName = strDisplayName;

		m_httpThread = new Thread(new ThreadStart(ThreadedHTTP));
		m_httpThread.Name = Helpers.FormatString("HTTP Thread ({0})", strDisplayName);
		m_httpThread.Start();

		Console.WriteLine("HTTP: Started service '{0}' on port {1}", strDisplayName, port);
	}

	public void SetAuthMethod_Token(string strToken)
	{
		m_AuthMethod = EAuthMethod.Token;
		m_strToken = strToken;

		Console.WriteLine("HTTP: Set service '{0}' to auth type {1}", m_strServiceName, m_AuthMethod);
	}

	public void SetAuthMethod_BasicAuth(string strUsername, string strPassword)
	{
		m_AuthMethod = EAuthMethod.Account;
		m_strUsername = strUsername;
		m_strPassword = strPassword;

		Console.WriteLine("HTTP: Set service '{0}' to auth type {1}", m_strServiceName, m_AuthMethod);
	}

	private async void ThreadedHTTP()
	{
		server = new HttpListener();

#if DEBUG
		server.Prefixes.Add(Helpers.FormatString("http://127.0.0.1:{0}/", m_Port));
#else
		server.Prefixes.Add(Helpers.FormatString("http://*:{0}/", m_Port));
#endif

		if (m_AuthMethod == EAuthMethod.Account)
		{
			server.AuthenticationSchemes = AuthenticationSchemes.Basic;
		}
		else if (m_AuthMethod == EAuthMethod.Token)
		{
			server.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
		}
		else
		{
			Console.WriteLine("HTTP: No auth type set for service '{0}'", m_strServiceName);
			return;
		}

		server.Start();

		// TODO: Add shutdown support?
		while (Thread.CurrentThread.IsAlive)
		{
			try
			{
				HttpListenerContext context = await server.GetContextAsync().ConfigureAwait(true);
				HttpListenerRequest request = context.Request;
				HttpListenerResponse response = context.Response;

				string strRequestPath = context.Request.Url.LocalPath.ToLower().Substring(1);

				bool bSuccess = false;
				string strBody = "";

				if (!string.IsNullOrEmpty(strRequestPath))
				{
					IPEndPoint source = request.RemoteEndPoint;

					string strRequestName = strRequestPath;

					bool bAuthedSuccessfully = false;
					if (m_AuthMethod == EAuthMethod.Token)
					{
						string strToken = context.Request.Headers.Get("Token");
						bAuthedSuccessfully = (m_strToken == strToken);
					}
					else
					{
						HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;
						bAuthedSuccessfully = (identity.Name == m_strUsername && identity.Password == m_strPassword);
					}

					if (m_dictRequestHandlers.ContainsKey(strRequestName) && bAuthedSuccessfully)
					{
						bool bIsHTML = m_dictRequestHandlers[strRequestName].IsHTML();
						string sResponse = m_dictRequestHandlers[strRequestName].OnRequestGenerateBody(request);
						if (sResponse != null)
						{
							strBody = Helpers.FormatString("{0}{1}{2}", bIsHTML ? "<html><body>" : "", sResponse, bIsHTML ? "</body></html>" : "");
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
				}

				// TODO: On failure, pretend to be offline/not listening?
				byte[] buffer = Encoding.UTF8.GetBytes(bSuccess ? strBody : m_strDefaultBody);

				response.ContentLength64 = buffer.Length;
				Stream st = response.OutputStream;
				st.Write(buffer, 0, buffer.Length);

				context.Response.Close();
			}
			catch
			{

			}
		}
	}

	~CHTTPServer()
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
		try
		{
			if (m_httpThread != null && m_httpThread.IsAlive)
			{
				m_httpThread.Abort();
				m_httpThread = null;
			}

			if (server != null && server.IsListening)
			{
				server.Abort();
				server.Close();
				server = null;
			}
		}
		catch
		{

		}
	}

	public static void AddHandler(string strName, CRequestHandler a_Handler)
	{
		m_dictRequestHandlers.Add(strName.ToLower(), a_Handler);
	}

	private static Dictionary<string, CRequestHandler> m_dictRequestHandlers = new Dictionary<string, CRequestHandler>();
	private const string m_strDefaultBody = "400 Bad Request";

#pragma warning disable CA1806
	public void RegisterHandler<T>()
	{
		Activator.CreateInstance<T>();
	}
}
#pragma warning restore CA1806

public abstract class CRequestHandler
{
	public CRequestHandler(string strName, bool bIsHTML = false)
	{
		CHTTPServer.AddHandler(strName, this);

		m_bIsHTML = bIsHTML;
	}

#pragma warning disable CA1716 // Identifiers should not match keywords
	public virtual string Get(HttpListenerRequest request)
#pragma warning restore CA1716 // Identifiers should not match keywords
	{
		return null;
	}

	public virtual string Post(HttpListenerRequest request)
	{
		return null;
	}

	public string OnRequestGenerateBody(HttpListenerRequest request)
	{
		m_hRequest = request;
		if (request.HttpMethod == "GET")
		{
			return Get(request);
		}
		else if (request.HttpMethod == "POST")
		{
			return Post(request);
		}
		else
		{
			// Unsupported Method
			return null;
		}
	}

	public string StreamedText()
	{
		string text;
		using (StreamReader reader = new StreamReader(GetRequest().InputStream,
											 GetRequest().ContentEncoding))
		{
			text = reader.ReadToEnd();
		}

		return text;
	}

	public bool IsHTML() { return m_bIsHTML; }
	public HttpListenerRequest GetRequest() { return m_hRequest; }
	private readonly bool m_bIsHTML;
	private HttpListenerRequest m_hRequest;
}

/// <summary>
/// All JSON Responses should inherit this class and add anything they need to it
/// </summary>
public abstract class JSONResponse
{
	public bool success { get; set; }

	/// <summary>
	/// Leave this blank if success = true
	/// </summary>
	public string reason { get; set; }

	public static T Parse<T>(string strJSON)
	{
		return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strJSON);
	}
}