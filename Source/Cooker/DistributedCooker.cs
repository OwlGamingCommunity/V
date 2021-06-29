#define DEBUG_SINGLE_THREADED_STATS

using System;
using System.Collections.Generic;
using System.Threading;

public class DistributedWorker : IDisposable
{
	private int m_ID = -1;
	private Thread m_Thread = null;
	private AutoResetEvent m_Signal = new AutoResetEvent(false); // not signalled by default

	// per task data
	private JobBase m_CurrentJob = null;
	private Action<bool, List<string>, List<string>, double> m_CompletionCallback = null;

	private Mutex m_KillMutex = new Mutex();
	private bool m_bKill = false;

	public DistributedWorker(int id)
	{
		m_ID = id;
		m_Thread = new Thread(ThreadedTick);
		m_Thread.Name = String.Format("Distributed Worker Thread {0}", m_ID);
		m_Thread.Start();
	}

	public void Kill()
	{
		m_KillMutex.WaitOne();
		m_bKill = true;
		m_KillMutex.ReleaseMutex();

		// wake it
		m_Signal.Set();
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		m_Signal.Dispose();
	}

	public bool NotifyWork(JobBase job, Action<bool, List<string>, List<string>, double> CompletionCallback)
	{
		m_CompletionCallback = CompletionCallback;
		m_CurrentJob = job;
		m_Signal.Set(); // wake thread
		return true;
	}

	public void ThreadedTick()
	{
		Console.WriteLine("Thread {0} started...", m_ID);

		while (true)
		{
			m_Signal.WaitOne();

			m_KillMutex.WaitOne();
			if (m_bKill)
			{
				break;
			}
			m_KillMutex.ReleaseMutex();

			// if we reach here, the thread has been awoken
			DateTime dtStart = DateTime.Now;
			bool bSuccess = m_CurrentJob.Execute();
			double dtTimeTaken = (DateTime.Now - dtStart).TotalMilliseconds;
			var bufferedIO = m_CurrentJob.GetBufferedIO();
			var errors = m_CurrentJob.GetErrors();
			m_CurrentJob = null;

			m_CompletionCallback(bSuccess, bufferedIO, errors, dtTimeTaken);
		}
	}
}

// NOTE: Distributed cooker is NOT reusable (not that I think we would need to ever?)
public class DistributedCooker : IDisposable
{
	private List<DistributedWorker> m_lstWorkerPool = new List<DistributedWorker>();

	private Mutex g_FreeWorkersMutex = new Mutex();
	private Queue<DistributedWorker> m_queueFreeWorkers = new Queue<DistributedWorker>();

	private Queue<JobBase> m_lstPendingJobs = new Queue<JobBase>();
	private Mutex g_PendingJobsMutex = new Mutex();

	private int m_JobsQueued = 0;
	private int m_JobsFinished = 0;
	private Action<bool, List<string>> m_CompleteAllTasksCallback = null;

	private Mutex g_ErrorsMutex = new Mutex();
	private bool m_bHasErrors = false;
	private List<string> m_lstErrors = new List<string>();

	private Mutex g_MainThreadBufferedIOMutex = new Mutex();
	private List<string> g_MainThreadBufferedIO = new List<string>();

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		g_FreeWorkersMutex.Dispose();
		g_PendingJobsMutex.Dispose();
	}

	public DistributedCooker()
	{
		int numThreads = CookerSettings.UseSinglethread ? 1 : Math.Min(Environment.ProcessorCount, CookerSettings.MaxThreads);
		if (CookerSettings.IsBuildServer())
		{
			numThreads = 8;
		}

		for (int i = 0; i < numThreads; ++i)
		{
			DistributedWorker worker = new DistributedWorker(i);
			m_lstWorkerPool.Add(worker);

			// mark as free
			g_FreeWorkersMutex.WaitOne();
			m_queueFreeWorkers.Enqueue(worker);
			g_FreeWorkersMutex.ReleaseMutex();
		}

		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine("Initialized Distributed Cooker with {0} threads", numThreads);
		Console.WriteLine("Fast Iteration Mode: {0}", CookerSettings.IsFastIterationMode());
		Console.ForegroundColor = ConsoleColor.Gray;
	}

	public void SetCallbackOnCompleteAllTasks(Action<bool, List<string>> CompleteAllTasksCallback)
	{
		m_CompleteAllTasksCallback = CompleteAllTasksCallback;
	}

	public int GetNumQueuedTasks()
	{
		g_PendingJobsMutex.WaitOne();
		int numJobs = m_lstPendingJobs.Count;
		g_PendingJobsMutex.ReleaseMutex();
		return numJobs;
	}

#if DEBUG_SINGLE_THREADED_STATS
	private Dictionary<string, double> dictTimePerJobType = new Dictionary<string, double>();
	private Mutex g_TimePerJobMutex = new Mutex();
#endif

	public void InitTimeForJob(Type jobType)
	{
#if DEBUG_SINGLE_THREADED_STATS
		string strJobName = jobType.ToString();
		if (!dictTimePerJobType.ContainsKey(strJobName))
		{
			dictTimePerJobType.Add(strJobName, 0.0);
		}
#endif
	}

	public void Tick()
	{
		// Do we have pending work? dispatch as much as we can
		g_PendingJobsMutex.WaitOne();
		g_FreeWorkersMutex.WaitOne();

		while (m_lstPendingJobs.Count > 0) // while we have jobs, try to dispatch
		{
			// Do we have a thread available to process the job?
			if (m_queueFreeWorkers.Count > 0)
			{
				JobBase job = m_lstPendingJobs.Dequeue();

				DistributedWorker handler = m_queueFreeWorkers.Dequeue();
				bool bHandlerAcceptedWork = handler.NotifyWork(job, (bool bSuccess, List<string> lstBufferedIO, List<string> lstErrors, double dtTimeTaken) => // finished callback (NOTE: NOT ON MAIN THREAD!)
				{
#if DEBUG_SINGLE_THREADED_STATS
					g_TimePerJobMutex.WaitOne();
					string strJobName = job.GetType().ToString();
					if (!dictTimePerJobType.ContainsKey(strJobName))
					{
						dictTimePerJobType[strJobName] = dtTimeTaken;
					}
					else
					{
						dictTimePerJobType[strJobName] += dtTimeTaken;
					}
					g_TimePerJobMutex.ReleaseMutex();
#endif

					if (!bSuccess)
					{
						g_ErrorsMutex.WaitOne();
						m_bHasErrors = true;
						m_lstErrors.AddRange(lstErrors);
						g_ErrorsMutex.ReleaseMutex();
					}

					// store buffered IO for process on main thread
					g_MainThreadBufferedIOMutex.WaitOne();
					g_MainThreadBufferedIO.AddRange(lstBufferedIO);
					g_MainThreadBufferedIOMutex.ReleaseMutex();

					// increment job counter
					g_PendingJobsMutex.WaitOne();
					++m_JobsFinished;
					g_PendingJobsMutex.ReleaseMutex();

					// add the worker back into the free pool
					// always do this, even when we fail!
					g_FreeWorkersMutex.WaitOne();
					m_queueFreeWorkers.Enqueue(handler);
					g_FreeWorkersMutex.ReleaseMutex();
				});

				// if the work wasn't accepted, requeue it and add the handler back to pool (at the end)
				if (!bHandlerAcceptedWork)
				{
					m_queueFreeWorkers.Enqueue(handler);
					m_lstPendingJobs.Enqueue(job);
				}
			}
			else
			{
				// we've used all of our connections
				break;
			}
		}

		// flush main thread buffered IO
		g_MainThreadBufferedIOMutex.WaitOne();
		foreach (string strLine in g_MainThreadBufferedIO)
		{
			Console.WriteLine(strLine);
		}
		g_MainThreadBufferedIO.Clear();
		g_MainThreadBufferedIOMutex.ReleaseMutex();

		// Are we done?
		// We must also have a callback set, meaning all jobs have been queued
		if (m_CompleteAllTasksCallback != null && m_JobsQueued == m_JobsFinished)
		{
			g_ErrorsMutex.WaitOne();
			m_CompleteAllTasksCallback(m_bHasErrors, m_lstErrors);
			g_ErrorsMutex.ReleaseMutex();

#if DEBUG_SINGLE_THREADED_STATS
			// dump stats
			Console.WriteLine("--- Total Time Spent Per Job (Doesn't account for threading):");
			foreach (var kvPair in dictTimePerJobType)
			{
				Console.WriteLine("\t {0} = {1}ms", kvPair.Key, kvPair.Value);
			}
			Console.WriteLine("-------------\n\n");
#endif

			// cleanup
			foreach (var worker in m_lstWorkerPool)
			{
				worker.Kill();
			}

			m_queueFreeWorkers.Clear();
			m_lstWorkerPool.Clear();

			m_JobsQueued = 0;
			m_JobsFinished = 0;
		}

		g_FreeWorkersMutex.ReleaseMutex();
		g_PendingJobsMutex.ReleaseMutex();
	}

	public void QueueJob(JobBase job)
	{
		Console.WriteLine("Queue job: {0}", job.Describe());

		// queue the work
		g_PendingJobsMutex.WaitOne();
		m_lstPendingJobs.Enqueue(job);
		++m_JobsQueued;
		g_PendingJobsMutex.ReleaseMutex();
	}
}
