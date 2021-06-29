using System;

public static class TaskManager
{
	public static void RunThreadedTask<T>(Func<T> taskFunc, Action<T> MainThreadCallback)
	{
		RAGE.Task.RunBackground(() =>
		{
			T retVal = taskFunc();
			RAGE.Task.Run(() => // marshal back to main thread, but use lambda capture for result
			{
				MainThreadCallback(retVal);
			});
		});
	}

	public static void RunThreadedTask<T, T2>(Func<T2, T> taskFunc, T2 inArg, Action<T> MainThreadCallback)
	{
		RAGE.Task.RunBackground(() =>
		{
			T retVal = taskFunc.Invoke(inArg);
			RAGE.Task.Run(() => // marshal back to main thread, but use lambda capture for result
			{
				MainThreadCallback(retVal);
			});
		});
	}

	public static void RunThreadedTask<T, T2, T3>(Func<T2, T3, T> taskFunc, T2 inArg, T3 inArg2, Action<T> MainThreadCallback)
	{
		RAGE.Task.RunBackground(() =>
		{
			T retVal = taskFunc.Invoke(inArg, inArg2);
			RAGE.Task.Run(() => // marshal back to main thread, but use lambda capture for result
			{
				MainThreadCallback(retVal);
			});
		});
	}
}