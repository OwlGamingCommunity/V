using System;
using System.Collections.Generic;

public static class AsyncModelLoader
{
	public delegate void AsyncModelLoadDelegate(uint Model);

	private static Dictionary<uint, List<AsyncLoadingHandle>> m_dictPendingLoads = null;

	public static void Init()
	{
		m_dictPendingLoads = new Dictionary<uint, List<AsyncLoadingHandle>>();
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;
	}

	// TODO_ASYNC_LOADER: Refcount and unload
	public class AsyncLoadingHandle
	{
		public AsyncLoadingHandle(object uniqueIdentifier, uint model, AsyncModelLoadDelegate callback)
		{
			UniqueIdentifier = uniqueIdentifier;
			Model = model;
			Callback = callback;
		}

		public object UniqueIdentifier { get; }
		public uint Model { get; }
		public AsyncModelLoadDelegate Callback { get; }
	}

	public static bool CancelPendingAsyncLoad(WeakReference<AsyncLoadingHandle> asyncHandle)
	{
		if (asyncHandle != null)
		{
			var handle = asyncHandle.Instance();
			if (handle != null)
			{
				if (m_dictPendingLoads.ContainsKey(handle.Model))
				{
					foreach (var iter in m_dictPendingLoads[handle.Model])
					{
						if (iter == handle)
						{
							m_dictPendingLoads[handle.Model].Remove(handle);
							return true;
						}
					}

					// was it the only one? cleanup
					if (m_dictPendingLoads[handle.Model].Count == 0)
					{
						m_dictPendingLoads.Remove(handle.Model);
					}
				}
			}
		}

		return false;
	}

	public static WeakReference<AsyncLoadingHandle> RequestAsyncLoad(uint model, AsyncModelLoadDelegate callback, bool bAllowDupe = true, object uniqueIdentifier = null)
	{
		RAGE.Game.Streaming.RequestModel(model);

		AsyncLoadingHandle newPendingLoadHandle = new AsyncLoadingHandle(uniqueIdentifier, model, callback);

		bool bDupeFound = false;
		if (!bAllowDupe)
		{
			if (m_dictPendingLoads.ContainsKey(model))
			{
				foreach (var iterCallback in m_dictPendingLoads[model])
				{
					if (iterCallback.UniqueIdentifier == uniqueIdentifier && iterCallback.Callback == callback)
					{
						bDupeFound = true;
					}
				}
			}
		}

		if (bDupeFound && !bAllowDupe)
		{
			return null;
		}

		// Do we already have a pending load?
		if (!m_dictPendingLoads.ContainsKey(model))
		{
			m_dictPendingLoads[model] = new List<AsyncLoadingHandle>();
		}

		m_dictPendingLoads[model].Add(newPendingLoadHandle);

		return new WeakReference<AsyncLoadingHandle>(newPendingLoadHandle);
	}

	public static void RequestSyncInstantLoad(uint model)
	{
		RAGE.Game.Streaming.RequestModel(model);
		RAGE.Game.Streaming.LoadAllObjectsNow();
	}

	public static void OnTick()
	{
		List<uint> lstToRemove = new List<uint>();
		foreach (var kvPair in m_dictPendingLoads)
		{
			// Is this model loaded?
			uint model = kvPair.Key;

			if (RAGE.Game.Streaming.HasModelLoaded(model))
			{
				try
				{
					// trigger all callbacks
					foreach (AsyncLoadingHandle pendingLoad in kvPair.Value)
					{
						pendingLoad.Callback(model);
					}
				}
				catch { }

				lstToRemove.Add(model);
			}
		}

		foreach (uint model in lstToRemove)
		{
			m_dictPendingLoads.Remove(model);
		}
	}
}