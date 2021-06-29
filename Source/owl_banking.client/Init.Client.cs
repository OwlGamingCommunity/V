class Init_BankingSystem : RAGE.Events.Script { Init_BankingSystem() { OwlScriptManager.RegisterScript<BankingSystem>(); } }

class BankingSystem : OwlScript
{
	public BankingSystem()
	{
		m_Banking = new Banking();
		m_PayDay = new PayDay();
	}

	public static Banking GetBanking() { return m_Banking; }
	private static Banking m_Banking = null;

	public static PayDay GetPayDay() { return m_PayDay; }
	private static PayDay m_PayDay = null;
}