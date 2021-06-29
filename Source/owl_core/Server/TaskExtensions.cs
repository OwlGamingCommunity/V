using GTANetworkAPI;
using System;
using System.Threading.Tasks;

public static class TaskExtensions
{
	public static void ContinueOnMainThread<TResult>(this System.Threading.Tasks.Task<TResult> task, Action<TResult> a)
	{
		task.ContinueWith((innerTask) =>
		{
			NAPI.Task.Run(() =>
			{
				a.Invoke(innerTask.Result);
			});

		}, TaskScheduler.Default);
	}

	public static void RunWithResult<T>(this GTANetworkMethods.Task task, Func<T> a, Action<T> output)
	{
		NAPI.Task.Run(() =>
		{
			T res = a.Invoke();
			output.Invoke(res);
		});
	}
}