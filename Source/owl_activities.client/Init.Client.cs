class Init_ActivitySystem : RAGE.Events.Script { Init_ActivitySystem() { OwlScriptManager.RegisterScript<ActivitySystem_Core>(); } }

class ActivitySystem_Core : OwlScript
{
	public ActivitySystem_Core()
	{
		m_ActivitySystem = new ActivitySystem();
	}

	public static ActivitySystem GetActivitySystem() { return m_ActivitySystem; }
	private static ActivitySystem m_ActivitySystem = null;
}