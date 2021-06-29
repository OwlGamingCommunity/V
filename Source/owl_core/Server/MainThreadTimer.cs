using System;
using System.Collections.Generic;

public delegate void TimerDelegate(object[] a_Parameters);

public class MainThreadTimerPool
{
	public MainThreadTimerPool()
	{
		RageEvents.RAGE_OnUpdate += Tick;
	}

	public static Int64 GetMillisecondsSinceDateTime(DateTime oldDateTime)
	{
		return (Int64)(DateTime.Now - oldDateTime).TotalMilliseconds;
	}

	public static WeakReference<MainThreadTimer> CreateGlobalTimer(TimerDelegate a_Functor, Int64 a_TimerInMilliseconds, int a_NumIterations = -1, object[] parameters = null)
	{
		MainThreadTimer newTimer = new MainThreadTimer(a_Functor, a_TimerInMilliseconds, null, a_NumIterations, parameters);
		m_lstTimersPendingCreation.Add(newTimer);
		return new WeakReference<MainThreadTimer>(newTimer);
	}

	public static WeakReference<MainThreadTimer> CreateEntityTimer(TimerDelegate a_Functor, Int64 a_TimerInMilliseconds, object a_ParentEntity, int a_NumIterations = -1, object[] parameters = null)
	{
		MainThreadTimer newTimer = new MainThreadTimer(a_Functor, a_TimerInMilliseconds, a_ParentEntity, a_NumIterations, parameters);
		m_lstTimersPendingCreation.Add(newTimer);
		return new WeakReference<MainThreadTimer>(newTimer);
	}

	// NOTE: We never directly let a caller destroy a timer, since they might be doing it from a timer callback which could cause a crash. Instead we let them mark it for pending deletion which the main thread tick picks up on the next iteration
	public static void MarkTimerForDeletion(WeakReference<MainThreadTimer> timer)
	{
		if (timer.Instance() != null)
		{
			timer.Instance().PendingDeletion = true;
		}
	}

	// NOTE: We never directly let a caller destroy a timer, since they might be doing it from a timer callback which could cause a crash. Instead we let them mark it for pending deletion which the main thread tick picks up on the next iteration
	public static void DestroyTimersFromParent(object a_ParentEntity)
	{
		foreach (MainThreadTimer timer in m_lstTimers)
		{
			if (timer.ParentEntity == a_ParentEntity)
			{
				timer.PendingDeletion = true;
			}
		}
	}

	public void Tick()
	{
		List<MainThreadTimer> lstToRemove = new List<MainThreadTimer>();

		foreach (MainThreadTimer timer in m_lstTimers)
		{
			timer.Tick();

			if (timer.PendingDeletion)
			{
				lstToRemove.Add(timer);
			}
		}

		foreach (MainThreadTimer timerToCreate in m_lstTimersPendingCreation)
		{
			m_lstTimers.Add(timerToCreate);
		}
		m_lstTimersPendingCreation.Clear();

		foreach (MainThreadTimer timerToDestroy in lstToRemove)
		{
			m_lstTimers.Remove(timerToDestroy);
		}
	}

	private static List<MainThreadTimer> m_lstTimersPendingCreation = new List<MainThreadTimer>();
	private static List<MainThreadTimer> m_lstTimers = new List<MainThreadTimer>();
}

public class MainThreadTimer
{
	public MainThreadTimer(TimerDelegate a_Functor, Int64 a_TimerInMilliseconds, object a_ParentEntity, int a_TotalIterations, object[] a_Parameters)
	{
		if (a_Parameters == null)
		{
			a_Parameters = Array.Empty<object>();
		}

		Functor = a_Functor;
		TimerInMilliseconds = a_TimerInMilliseconds;
		ParentEntity = a_ParentEntity;
		TotalIterations = a_TotalIterations;
		LastTickTime = DateTime.Now;
		Parameters = a_Parameters;
	}

	public void Tick()
	{
		Int64 timeSinceLastProc = MainThreadTimerPool.GetMillisecondsSinceDateTime(LastTickTime);
		if (!PendingDeletion && timeSinceLastProc >= TimerInMilliseconds)
		{
			LastTickTime = DateTime.Now;

			long millisecondsStart = DateTime.Now.Ticks;
			Functor(Parameters);
			ServerPerfManager.RegisterTimerPerf(Functor, millisecondsStart);

			++NumIterationsDone;
		}

		if (TotalIterations > 0 && NumIterationsDone >= TotalIterations)
		{
			PendingDeletion = true;
		}
	}

	private TimerDelegate Functor;

	public bool PendingDeletion { get; set; } = false;
	int NumIterationsDone = 0;
	private Int64 TimerInMilliseconds;
	private int TotalIterations;
	private DateTime LastTickTime;
	public object ParentEntity { get; } // TODO_LAUNCH: Make this a weak ref!
	private object[] Parameters;
}