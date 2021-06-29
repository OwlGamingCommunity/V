using System;
using System.Collections.Generic;

namespace ExtensionMethods
{
	public static class QueueExtensions
	{
		public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enu)
		{
			foreach (T obj in enu)
				queue.Enqueue(obj);
		}
	}
}

#if SERVER
public class MultiFrameWorkScheduler_ServerInit : GTANetworkAPI.Script
{
	public MultiFrameWorkScheduler_ServerInit()
	{
		MultiFrameWorkScheduler.Initialize();
	}
}
#endif

public static class MultiFrameWorkScheduler
{
	private static List<MultiFrameWorkLoad> g_lstWorkloads = new List<MultiFrameWorkLoad>();

	public static void Initialize()
	{
#if SERVER
		RageEvents.RAGE_OnUpdate += OnUpdate;
#else
		RageEvents.RAGE_OnTick_PerFrame += OnUpdate;
#endif
	}

	private static void OnUpdate()
	{
		List<MultiFrameWorkLoad> lstToDestroy = new List<MultiFrameWorkLoad>();

		foreach (MultiFrameWorkLoad workLoad in g_lstWorkloads)
		{
			bool bIsCompleteAndNeedsDestroy = workLoad.Tick();
			if (bIsCompleteAndNeedsDestroy)
			{
				lstToDestroy.Add(workLoad);
			}
		}

		foreach (MultiFrameWorkLoad workLoadToDestroy in lstToDestroy)
		{
			g_lstWorkloads.Remove(workLoadToDestroy);
		}
	}

	public static void QueueWork(MultiFrameWorkLoad workLoad)
	{
		g_lstWorkloads.Add(workLoad);
	}
}

public enum EWorkLoadProcessingType
{
	IterationsPerFrame,
	FrameMillisecondsBudget
}

public class MultiFrameWorkLoad
{
	private Action<Queue<object>> m_InitFunctor = null;
	private Action<object> m_TickFunctor = null;
	private Func<bool> m_CompleteFunctor = null;

	private Queue<object> m_WorkQueue = new Queue<object>();

	private EWorkLoadProcessingType m_ProcessingType;
	private double m_dNumberIterationsOrFrameBudgetInMilliseconds;

	public MultiFrameWorkLoad(EWorkLoadProcessingType ProcessingType, double dNumberIterationsOrFrameBudgetInMilliseconds, Action<Queue<object>> InitFunctor, Action<object> TickFunctor, Func<bool> CompleteFunctor)
	{
		m_ProcessingType = ProcessingType;
		m_dNumberIterationsOrFrameBudgetInMilliseconds = dNumberIterationsOrFrameBudgetInMilliseconds;
		m_InitFunctor = InitFunctor; // This is a constructor, use it to queue your work items etc
		m_TickFunctor = TickFunctor; // This is called for EACH item being processed, up to N per frame. We exit automatically and call CompleteFunctor when the work queue is empty
		m_CompleteFunctor = CompleteFunctor; // This is called upon completion of all work items. Return true if you want to restart your work queue (e.g. call init again, call tick for each, etc). False otherwise.

		m_InitFunctor(m_WorkQueue);
	}

	public bool Tick()
	{
		// Do we have items remaining?
		if (m_ProcessingType == EWorkLoadProcessingType.IterationsPerFrame)
		{
			int NumToProcess = Math.Min((int)m_dNumberIterationsOrFrameBudgetInMilliseconds, m_WorkQueue.Count);
			for (int i = 0; i < NumToProcess; ++i)
			{
				object objectToProcess = m_WorkQueue.Dequeue();
				m_TickFunctor(objectToProcess);
			}
		}
		else if (m_ProcessingType == EWorkLoadProcessingType.FrameMillisecondsBudget)
		{
			// keep going until we run out of things, or hit our frame budget
			long startTicks = DateTime.Now.Ticks;

			int totalWorkItemsRemaining = m_WorkQueue.Count; // faster to cache than keep calling
			while (totalWorkItemsRemaining > 0 && (((double)DateTime.Now.Ticks - (double)startTicks) / (double)TimeSpan.TicksPerMillisecond) < m_dNumberIterationsOrFrameBudgetInMilliseconds)
			{
				object objectToProcess = m_WorkQueue.Dequeue();
				m_TickFunctor(objectToProcess);
				--totalWorkItemsRemaining;
			}
		}

		// Are we done?
		if (m_WorkQueue.Count == 0)
		{
			bool bShouldRequeue = m_CompleteFunctor();
			if (bShouldRequeue)
			{
				m_InitFunctor(m_WorkQueue);
			}
			else
			{
				return true; // true marks us for deletion
			}
		}

		return false;
	}
}