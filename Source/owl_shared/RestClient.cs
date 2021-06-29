using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

public class CRestClient : IDisposable
{
	private static readonly HttpClient m_Client = new HttpClient();
	private string m_strToken = String.Empty;

	private string m_strEndpoint = String.Empty;

	private Mutex m_Mutex = new Mutex();
	private Queue<PendingRequest> m_queuePendingSendRequests = new Queue<PendingRequest>();
	private Queue<PendingMainThreadDispatchedResponse> m_queueCompletedRequestsPendingMainThreadDispatch = new Queue<PendingMainThreadDispatchedResponse>();

	private Thread m_requestThread;
	private AutoResetEvent m_Signal = new AutoResetEvent(false); // not signalled by default

	public enum ERestCallbackThreadingMode
	{
		ContinueOnWorkerThread,
		ContinueOnMainThread
	}

	private class PendingRequest
	{
		public JSONRequest_Base Request { get; }
		public Action<string> CompletionCallback { get; }
		public ERestCallbackThreadingMode ThreadingMode { get; }

		public PendingRequest(JSONRequest_Base a_Request, ERestCallbackThreadingMode a_ThreadingMode, Action<string> a_CompletionCallback)
		{
			Request = a_Request;
			CompletionCallback = a_CompletionCallback;
			ThreadingMode = a_ThreadingMode;
		}
	}

	private class PendingMainThreadDispatchedResponse
	{
		public string ResponseBodyJSON { get; }
		public Action<string> CompletionCallback { get; }

		public PendingMainThreadDispatchedResponse(string a_ResponseBodyJSON, Action<string> a_CompletionCallback)
		{
			ResponseBodyJSON = a_ResponseBodyJSON;
			CompletionCallback = a_CompletionCallback;
		}
	}

	public CRestClient(string strBaseEndpoint, int Port, string strToken)
	{
		m_strToken = strToken;

		m_strEndpoint = Helpers.FormatString("http://{0}:{1}", strBaseEndpoint, Port);
		Console.WriteLine("Created Rest Client with endpoint {0}", m_strEndpoint);

		m_requestThread = new Thread(new ThreadStart(ThreadedRequestManager));
		m_requestThread.Name = "REST Client Thread";
		m_requestThread.Start();
	}

	~CRestClient()
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
			if (m_requestThread != null && m_requestThread.IsAlive)
			{
				m_requestThread.Abort();
				m_requestThread = null;
			}
		}
		catch
		{

		}
	}

	// NOTE: You must call this from main thread
	public void MainThreadTick()
	{
		m_Mutex.WaitOne();
		while (m_queueCompletedRequestsPendingMainThreadDispatch.Count > 0) // while we have requests, try to dispatch
		{
			PendingMainThreadDispatchedResponse pendingResponse = m_queueCompletedRequestsPendingMainThreadDispatch.Dequeue();
			pendingResponse.CompletionCallback(pendingResponse.ResponseBodyJSON);
		}
		m_Mutex.ReleaseMutex();
	}

	private int GetNumPendingRequests()
	{
		m_Mutex.WaitOne();
		int numPendingRequests = m_queuePendingSendRequests.Count;
		m_Mutex.ReleaseMutex();

		return numPendingRequests;
	}

	private async void ThreadedRequestManager()
	{
		while (Thread.CurrentThread.IsAlive)
		{
			m_Signal.WaitOne();
			// process all pending requests
			while (GetNumPendingRequests() > 0) // while we have requests, try to dispatch
			{
				m_Mutex.WaitOne();
				PendingRequest pendingRequest = m_queuePendingSendRequests.Dequeue();
				m_Mutex.ReleaseMutex();

				// TODO: Handle failure
				try
				{
					string strResponse = await pendingRequest.Request.SendRequest(m_Client, m_strToken, m_strEndpoint).ConfigureAwait(true); ;

					if (pendingRequest.CompletionCallback != null)
					{
						if (pendingRequest.ThreadingMode == ERestCallbackThreadingMode.ContinueOnWorkerThread)
						{
							pendingRequest.CompletionCallback(strResponse);
						}
						else
						{
							m_Mutex.WaitOne();
							m_queueCompletedRequestsPendingMainThreadDispatch.Enqueue(new PendingMainThreadDispatchedResponse(strResponse, pendingRequest.CompletionCallback));
							m_Mutex.ReleaseMutex();
						}
					}
				}
				catch
				{

				}
			}
		}
	}

	public void QueueRequest(JSONRequest_Base request, ERestCallbackThreadingMode ThreadingMode, Action<string> completionCallback)
	{
		m_Mutex.WaitOne();
		m_queuePendingSendRequests.Enqueue(new PendingRequest(request, ThreadingMode, completionCallback));
		m_Mutex.ReleaseMutex();

		// wake the thread
		m_Signal.Set();
	}
}