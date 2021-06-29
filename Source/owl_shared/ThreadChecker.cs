using System;
using System.Threading;

public class ThreadChecker : IDisposable
{
	private Mutex g_Mutex;
	private int g_MainThreadID = -1;
	private int g_PreferredThreadID = -1;

	public ThreadChecker()
	{
		g_Mutex = new Mutex();
		g_MainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
		g_PreferredThreadID = g_MainThreadID;
	}

	~ThreadChecker()
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
			if (g_Mutex != null)
			{
				g_Mutex.Dispose();
				g_Mutex = null;
			}
		}
		catch
		{

		}
	}

	public void SetCurrentThreadAsPreferredThread()
	{
		g_Mutex.WaitOne();
		g_PreferredThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
		g_Mutex.ReleaseMutex();
	}

	public bool IsOnMainThread()
	{
		g_Mutex.WaitOne();
		bool bIsOnMainThread = (System.Threading.Thread.CurrentThread.ManagedThreadId == g_MainThreadID);
		g_Mutex.ReleaseMutex();

		return bIsOnMainThread;
	}

	public void IsOnMainThread_ThrowIfNot()
	{
		g_Mutex.WaitOne();
		if (System.Threading.Thread.CurrentThread.ManagedThreadId != g_MainThreadID)
		{
			throw new Exception("CRITICAL ERROR: NOT ON MAIN THREAD");
		}
		g_Mutex.ReleaseMutex();
	}

	public void IsOnMainThread_LogIfNot()
	{
		g_Mutex.WaitOne();
		if (System.Threading.Thread.CurrentThread.ManagedThreadId != g_MainThreadID)
		{
			Console.WriteLine("CRITICAL ERROR: NOT ON MAIN THREAD");
		}
		g_Mutex.ReleaseMutex();
	}

	public bool IsOnPrefererdThread()
	{
		g_Mutex.WaitOne();
		bool bIsOnMainThread = (System.Threading.Thread.CurrentThread.ManagedThreadId == g_PreferredThreadID);
		g_Mutex.ReleaseMutex();

		return bIsOnMainThread;
	}

	public void IsOnPreferredThread_ThrowIfNot()
	{
		g_Mutex.WaitOne();
		if (System.Threading.Thread.CurrentThread.ManagedThreadId != g_PreferredThreadID)
		{
			throw new Exception("CRITICAL ERROR: NOT ON PREFERRED THREAD");
		}
		g_Mutex.ReleaseMutex();
	}

	public void IsOnPreferredThread_LogIfNot()
	{
		g_Mutex.WaitOne();
		if (System.Threading.Thread.CurrentThread.ManagedThreadId != g_PreferredThreadID)
		{
			Console.WriteLine("CRITICAL ERROR: NOT ON PREFERRED THREAD");
		}
		g_Mutex.ReleaseMutex();
	}
}