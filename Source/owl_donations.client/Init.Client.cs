class Init_DonationSystem : RAGE.Events.Script { Init_DonationSystem() { OwlScriptManager.RegisterScript<DonationSystem>(); } }

class DonationSystem : OwlScript
{
	public DonationSystem()
	{
		m_Donations = new Donations();
	}

	public static Donations GetDonations() { return m_Donations; }
	private static Donations m_Donations = null;
}