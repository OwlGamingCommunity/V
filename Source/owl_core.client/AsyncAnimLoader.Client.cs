using System.Collections.Generic;

public static class AsyncAnimLoader
{
	public delegate void AsyncAnimLoaderDelegate(string DictionaryName);

	private class PendingAsyncAnimLoad
	{
		public PendingAsyncAnimLoad(AsyncAnimLoaderDelegate callback)
		{
			Callback = callback;
		}

		public AsyncAnimLoaderDelegate Callback { get; }
	}

	private static Dictionary<string, List<PendingAsyncAnimLoad>> m_dictPendingLoads = null;

	public static void Init()
	{
		m_dictPendingLoads = new Dictionary<string, List<PendingAsyncAnimLoad>>();
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;
	}

	public static void RequestAsyncLoad(string strDictionary, AsyncAnimLoaderDelegate callback)
	{
		RAGE.Game.Streaming.RequestAnimDict(strDictionary);

		PendingAsyncAnimLoad newPendingLoad = new PendingAsyncAnimLoad(callback);

		// Do we already have a pending load?
		if (!m_dictPendingLoads.ContainsKey(strDictionary))
		{
			m_dictPendingLoads[strDictionary] = new List<PendingAsyncAnimLoad>();
		}

		m_dictPendingLoads[strDictionary].Add(newPendingLoad);
	}

	public static void OnTick()
	{
		List<string> lstToRemove = new List<string>();
		foreach (var kvPair in m_dictPendingLoads)
		{
			// Is this dictionary loaded?
			string strDictionary = kvPair.Key;

			if (RAGE.Game.Streaming.HasAnimDictLoaded(strDictionary))
			{
				try
				{
					// trigger all callbacks
					foreach (PendingAsyncAnimLoad pendingLoad in kvPair.Value)
					{
						pendingLoad.Callback(strDictionary);
					}
				}
				catch { }

				lstToRemove.Add(strDictionary);
			}
		}

		foreach (string dictionary in lstToRemove)
		{
			m_dictPendingLoads.Remove(dictionary);
		}
	}
}