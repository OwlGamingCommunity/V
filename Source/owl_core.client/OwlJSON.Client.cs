using System;
using System.Collections.Generic;

public static class OwlJSON
{
	private static double m_TimeSpentJSONDeserialize = 0.0;
	private static double m_TimeSpentJSONSerialize = 0.0;
	private static Dictionary<string, double> m_dictTimeSpentJSONDeserialize = new Dictionary<string, double>();
	private static Dictionary<string, double> m_dictTimeSpentJSONSerialize = new Dictionary<string, double>();
	private static Dictionary<string, int> m_dictCountJSONDeserialize = new Dictionary<string, int>();
	private static Dictionary<string, int> m_dictCountJSONSerialize = new Dictionary<string, int>();
	private static int m_lastFrameID = -1;
	private static bool m_bDebugSpamEnabled = false;

	public static void Init()
	{
		CheckReset();

		NetworkEvents.ToggleDebugSpam += () =>
		{
			m_bDebugSpamEnabled = !m_bDebugSpamEnabled;
			ChatHelper.DebugMessage("JSON Debug Mode Enabled: {0}", m_bDebugSpamEnabled);
		};
	}

	public static void OnRender()
	{
		CheckReset();
	}

	private static void AddJSONDeserializeTimeThisFrame(double time, EJsonTrackableIdentifier identifier, string optionalIdentifierAddon)
	{
		CheckReset();
		m_TimeSpentJSONDeserialize += time;

		string realIdentifier = optionalIdentifierAddon == null ? identifier.ToString() : Helpers.FormatString("{0}_{1}", identifier.ToString(), optionalIdentifierAddon);
		if (m_dictTimeSpentJSONDeserialize.ContainsKey(realIdentifier))
		{
			m_dictTimeSpentJSONDeserialize[realIdentifier] += time;
		}
		else
		{
			m_dictTimeSpentJSONDeserialize[realIdentifier] = time;
		}

		if (m_dictCountJSONDeserialize.ContainsKey(realIdentifier))
		{
			m_dictCountJSONDeserialize[realIdentifier]++;
		}
		else
		{
			m_dictCountJSONDeserialize[realIdentifier] = 1;
		}
	}

	private static void AddJSONSerializeTimeThisFrame(double time, EJsonTrackableIdentifier identifier, string optionalIdentifierAddon = null)
	{
		CheckReset();
		m_TimeSpentJSONSerialize += time;

		string realIdentifier = optionalIdentifierAddon == null ? identifier.ToString() : Helpers.FormatString("{0}_{1}", identifier.ToString(), optionalIdentifierAddon);
		if (m_dictTimeSpentJSONSerialize.ContainsKey(realIdentifier))
		{
			m_dictTimeSpentJSONSerialize[realIdentifier] += time;
		}
		else
		{
			m_dictTimeSpentJSONSerialize[realIdentifier] = time;
		}

		if (m_dictCountJSONSerialize.ContainsKey(realIdentifier))
		{
			m_dictCountJSONSerialize[realIdentifier]++;
		}
		else
		{
			m_dictCountJSONSerialize[realIdentifier] = 1;
		}
	}

	private static void CheckReset()
	{
		if (!m_bDebugSpamEnabled)
		{
			return;
		}

		if (RAGE.Game.Misc.GetFrameCount() != m_lastFrameID)
		{
			const float threshold = 5.0f;
			if (m_TimeSpentJSONDeserialize >= threshold || m_TimeSpentJSONSerialize >= threshold)
			{
				ChatHelper.DebugMessage("--> JSON Time Last Frame [{2}]: {0}ms Deserialize, {1}ms serialize", m_TimeSpentJSONDeserialize, m_TimeSpentJSONSerialize, m_lastFrameID);

				ChatHelper.DebugMessage("--> Deserialize Breakdown:");
				foreach (var kvPair in m_dictTimeSpentJSONDeserialize)
				{
					string identifier = kvPair.Key;
					ChatHelper.DebugMessage("{0}x{1} = {2}ms", m_dictCountJSONDeserialize[identifier], identifier, m_dictTimeSpentJSONDeserialize[identifier]);
				}

				ChatHelper.DebugMessage("--> Serialize Breakdown:");
				foreach (var kvPair in m_dictTimeSpentJSONSerialize)
				{
					string identifier = kvPair.Key;
					ChatHelper.DebugMessage("{0}x{1} = {2}ms", m_dictCountJSONSerialize[identifier], identifier, m_dictTimeSpentJSONSerialize[identifier]);
				}
			}

			m_lastFrameID = RAGE.Game.Misc.GetFrameCount();

			m_TimeSpentJSONDeserialize = 0.0;
			m_TimeSpentJSONSerialize = 0.0;
			m_dictTimeSpentJSONDeserialize.Clear();
			m_dictTimeSpentJSONSerialize.Clear();
			m_dictCountJSONDeserialize.Clear();
			m_dictCountJSONSerialize.Clear();
		}
	}

	public static T DeserializeObject<T>(string strJSON, EJsonTrackableIdentifier identifier, string optionalIdentifierAddon = null)
	{
		try
		{
			DateTime dtStart = DateTime.Now;
			T obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strJSON);
			AddJSONDeserializeTimeThisFrame((DateTime.Now - dtStart).TotalMilliseconds, identifier, optionalIdentifierAddon);
			return obj;
		}
		catch
		{
			throw new Exception(Helpers.FormatString("Deserialize Failed, identifier was {0}, input was {1}", identifier, strJSON ?? "NULL"));
		}
	}

	public static T DeserializeObject<T>(string strJSON, Newtonsoft.Json.JsonSerializerSettings settings, EJsonTrackableIdentifier identifier, string optionalIdentifierAddon = null)
	{
		try
		{
			DateTime dtStart = DateTime.Now;
			T obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strJSON, settings);
			AddJSONDeserializeTimeThisFrame((DateTime.Now - dtStart).TotalMilliseconds, identifier, optionalIdentifierAddon);
			return obj;
		}
		catch
		{
			throw new Exception(Helpers.FormatString("Deserialize Failed, identifier was {0}, input was {1}", identifier, strJSON ?? "NULL"));
		}
	}

	public static object DeserializeObject(string strJSON, Type t, EJsonTrackableIdentifier identifier, string optionalIdentifierAddon = null)
	{
		try
		{
			DateTime dtStart = DateTime.Now;
			object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(strJSON, t);
			AddJSONDeserializeTimeThisFrame((DateTime.Now - dtStart).TotalMilliseconds, identifier, optionalIdentifierAddon);
			return obj;
		}
		catch
		{
			throw new Exception(Helpers.FormatString("Deserialize Failed, identifier was {0}, input was {1}", identifier, strJSON ?? "NULL"));
		}
	}

	public static string SerializeObject<T>(T obj, EJsonTrackableIdentifier identifier, string optionalIdentifierAddon = null)
	{
		DateTime dtStart = DateTime.Now;
		string strJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		AddJSONSerializeTimeThisFrame((DateTime.Now - dtStart).TotalMilliseconds, identifier, optionalIdentifierAddon);
		return strJSON;
	}

	public static string SerializeObject<T>(T obj, Newtonsoft.Json.Formatting formatting, Newtonsoft.Json.JsonSerializerSettings settings, EJsonTrackableIdentifier identifier, string optionalIdentifierAddon = null)
	{
		DateTime dtStart = DateTime.Now;
		string strJSON = Newtonsoft.Json.JsonConvert.SerializeObject(obj, formatting, settings);
		AddJSONSerializeTimeThisFrame((DateTime.Now - dtStart).TotalMilliseconds, identifier, optionalIdentifierAddon);
		return strJSON;
	}
}