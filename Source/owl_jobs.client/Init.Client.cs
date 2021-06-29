class Init_JobSystem : RAGE.Events.Script { Init_JobSystem() { OwlScriptManager.RegisterScript<JobSystem_Core>(); } }

class JobSystem_Core : OwlScript
{
	public JobSystem_Core()
	{
		m_JobSystem = new JobSystem();
		m_FishingSystem = new FishingSystem();
	}

	public static JobSystem GetJobSystem() { return m_JobSystem; }
	private static JobSystem m_JobSystem = null;

	public static FishingSystem GetFishingSystem() { return m_FishingSystem; }
	private static FishingSystem m_FishingSystem = null;
}