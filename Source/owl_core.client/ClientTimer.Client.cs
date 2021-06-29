using System;
using System.Collections.Generic;

public delegate void ClientTimerDelegate(object[] a_Parameters);

public static class ClientTimerPool
{
	static ClientTimerPool()
	{

	}

	public static void Init()
	{
		RageEvents.RAGE_TimerTickInternal_DO_NOT_USE += Tick;
	}

	public static WeakReference<ClientTimer> CreateTimer(ClientTimerDelegate a_Functor, int a_TimerInMilliseconds, int a_NumIterations = -1, object[] parameters = null)
	{
		ClientTimer newTimer = new ClientTimer(a_Functor, a_TimerInMilliseconds, a_NumIterations, parameters);
		m_lstTimersPendingAdd.Add(newTimer);
		return new WeakReference<ClientTimer>(newTimer);
	}

	// NOTE: We never directly let a caller destroy a timer, since they might be doing it from a timer callback which could cause a crash. Instead we let them mark it for pending deletion which the main thread tick picks up on the next iteration
	public static void MarkTimerForDeletion(ref WeakReference<ClientTimer> timer)
	{
		ClientTimer outTimer;
		if (timer.TryGetTarget(out outTimer))
		{
			outTimer.PendingDeletion = true;
		}

		timer.SetTarget(null);
	}

	public static void Tick()
	{
		foreach (ClientTimer timerPendingCreation in m_lstTimersPendingAdd)
		{
			m_lstTimers.Add(timerPendingCreation);
		}
		m_lstTimersPendingAdd.Clear();


		List<ClientTimer> lstToRemove = new List<ClientTimer>();

		foreach (ClientTimer timer in m_lstTimers)
		{
			timer.Tick();

			if (timer.PendingDeletion)
			{
				lstToRemove.Add(timer);
			}
		}

		foreach (ClientTimer timerToDestroy in lstToRemove)
		{
			m_lstTimers.Remove(timerToDestroy);
		}
	}

	private static List<ClientTimer> m_lstTimersPendingAdd = new List<ClientTimer>();
	private static List<ClientTimer> m_lstTimers = new List<ClientTimer>();
}

public class ClientTimer
{
	public ClientTimer(ClientTimerDelegate a_Functor, int a_TimerInMilliseconds, int a_TotalIterations, object[] a_Parameters)
	{
		if (a_Parameters == null)
		{
			a_Parameters = Array.Empty<object>();
		}

		Functor = a_Functor;
		TimerInMilliseconds = a_TimerInMilliseconds;
		TotalIterations = a_TotalIterations;
		LastTickTime = DateTime.Now; // TODO_CSHARP: Use StopWatch for more accuracy
		Parameters = a_Parameters;
	}

	public int GetSecondsUntilNextTick()
	{
		double timeSinceLastProc = (DateTime.Now - LastTickTime).TotalMilliseconds;
		return (int)((TimerInMilliseconds - timeSinceLastProc) / 1000);
	}

	public void Tick()
	{
		double timeSinceLastProc = (DateTime.Now - LastTickTime).TotalMilliseconds;

		if (!PendingDeletion && timeSinceLastProc >= TimerInMilliseconds)
		{
			LastTickTime = DateTime.Now;

			long millisecondsStart = DateTime.Now.Ticks;
			Functor(Parameters);
			EventManager.RegisterTimerPerf(Functor, millisecondsStart);

			PerfManager.RegisterTimerPerf(Functor, millisecondsStart);

			++NumIterationsDone;
		}

		if (TotalIterations > 0 && NumIterationsDone >= TotalIterations)
		{
			PendingDeletion = true;
		}
	}

	private ClientTimerDelegate Functor;

	public bool PendingDeletion { get; set; } = false;
	int NumIterationsDone = 0;
	private int TimerInMilliseconds;
	private int TotalIterations;
	private DateTime LastTickTime;
	private object[] Parameters;
}