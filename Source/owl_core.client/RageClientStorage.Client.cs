using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class RageClientStorageContainer
{
	[JsonProperty("animations")]
	public List<DO_NOT_USE_CCustomAnimCommand> DO_NOT_USE_CustomAnimations { get; set; } = null;


	[JsonProperty("LFTCache")]
	public Dictionary<string, string> LFTCache { get; set; } = null;

	[JsonProperty("ChatHistory")]
	public List<string> ChatHistory { get; set; } = null;
}

public static class RageClientStorageManager
{
	private static bool m_bDoneFirstTimeLoad = false;
	public static RageClientStorageContainer Container { get; set; } = new RageClientStorageContainer();

	public static void Initialize()
	{
		RageEvents.RAGE_OnRender += RequestFirstTimeLoad;

		NetworkEvents.LoadRageClientStorage += (string strData) =>
		{
			try
			{
				Container = OwlJSON.DeserializeObject<RageClientStorageContainer>(strData, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore,
					MissingMemberHandling = MissingMemberHandling.Ignore,
					CheckAdditionalContent = false
				}, EJsonTrackableIdentifier.ClientStorage);

				NetworkEvents.SendLocalEvent_RageClientStorageLoaded();
			}
			catch (Exception e)
			{
				ExceptionHelper.SendException(e);
				Container = new RageClientStorageContainer();
				NetworkEvents.SendLocalEvent_RageClientStorageLoaded();
			}
		};
	}

	public static void ForceReload()
	{
		m_bDoneFirstTimeLoad = false;
		RageEvents.RAGE_OnRender += RequestFirstTimeLoad;
	}

	// NOTE: This is delayed because of waiting for the JS file to load... so we do it on first script render (after all scripts are loaded + initialized)
	private static void RequestFirstTimeLoad()
	{
		if (!m_bDoneFirstTimeLoad)
		{
			m_bDoneFirstTimeLoad = true;
			RAGE.Events.CallLocal("InitRageClientStorage");

			// de-register render
			RageEvents.RAGE_OnRender -= RequestFirstTimeLoad;
		}
	}

	public static bool Flush()
	{
		// don't flush if we haven't yet loaded because we could overwrite/lose data
		if (m_bDoneFirstTimeLoad)
		{
			string strJSON = OwlJSON.SerializeObject(Container, Formatting.Indented, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore,
				CheckAdditionalContent = false
			}, EJsonTrackableIdentifier.ClientStorageFlush);
			RAGE.Events.CallLocal("FlushRageClientStorage", strJSON);
			return true;
		}
		else
		{
			throw new System.Exception("RAGE client storage has not yet loaded - you are trying to flush too early! No action was taken because you could lose data.");
		}
	}
}