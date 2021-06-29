class Init_PropertySystem : RAGE.Events.Script { Init_PropertySystem() { OwlScriptManager.RegisterScript<PropertySystem>(); } }

class PropertySystem : OwlScript
{
	public PropertySystem()
	{
		m_PropertyManager = new PropertyManager();
	}

	public static PropertyManager GetPropertyManager() { return m_PropertyManager; }
	private static PropertyManager m_PropertyManager = null;
}