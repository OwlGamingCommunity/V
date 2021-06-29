class Init_Achievements : RAGE.Events.Script { Init_Achievements() { OwlScriptManager.RegisterScript<AchievementSystem>(); } }

class AchievementSystem : OwlScript
{
	public AchievementSystem()
	{
		Achievements.Init();
		Credits.Init();
		m_Uganda = new Uganda();
		m_Vub = new Vub();
	}

	public static Uganda GetUganda() { return m_Uganda; }
	private static Uganda m_Uganda = null;

	private static Vub m_Vub = null;
}