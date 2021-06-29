using System;
using System.Collections.Generic;

public static class OwlScriptManager
{
	private static List<OwlScript> m_lstScripts = new List<OwlScript>();

	public static void RegisterScript<T>()
	{
		// Scripts are considered unique by type
		foreach (OwlScript script in m_lstScripts)
		{
			if (script.GetType() == typeof(T))
			{
				return;
			}
		}

		OwlScript newInstance = (OwlScript)Activator.CreateInstance(typeof(T));
		m_lstScripts.Add(newInstance);
	}
}

public class OwlScript
{
	public OwlScript()
	{
		//OwlScriptManager.RegisterScript(this);
	}
}