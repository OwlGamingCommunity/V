class Init_CustomMapSystem : RAGE.Events.Script { Init_CustomMapSystem() { OwlScriptManager.RegisterScript<CustomMapSystem>(); } }

class CustomMapSystem : OwlScript
{
	public CustomMapSystem()
	{
		m_CustomMapLoader = new CustomMapLoader();
	}

	public static CustomMapLoader GetCustomMapLoader() { return m_CustomMapLoader; }
	private static CustomMapLoader m_CustomMapLoader = null;
}