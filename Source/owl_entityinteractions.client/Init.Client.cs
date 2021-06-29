class Init_EntityInteractions : RAGE.Events.Script { Init_EntityInteractions() { OwlScriptManager.RegisterScript<EntityInteractions>(); } }

public class EntityInteractions : OwlScript
{
	public EntityInteractions()
	{
		m_PlayerRadialMenu = new PlayerRadialMenu();
	}

	public static PlayerRadialMenu GetPlayerRadialMenu() { return m_PlayerRadialMenu; }
	private static PlayerRadialMenu m_PlayerRadialMenu = null;
}