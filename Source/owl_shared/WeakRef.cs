using System;

public static class WeakRefHelper
{
	public static T Instance<T>(this WeakReference<T> inRef) where T : class
	{
		if (!inRef.TryGetTarget(out T var))
		{
			var = null;
		}
		return var;
	}
}