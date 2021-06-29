class Init_RadioSystem : RAGE.Events.Script { Init_RadioSystem() { OwlScriptManager.RegisterScript<RadioSystem>(); } }

class RadioSystem : OwlScript
{
	public RadioSystem()
	{
		m_RadioManagement = new RadioManagement();
	}

	public static RadioManagement GetRadioManagement() { return m_RadioManagement; }
	private static RadioManagement m_RadioManagement = null;
}