class Init_AnimationSystem : RAGE.Events.Script { Init_AnimationSystem() { OwlScriptManager.RegisterScript<AnimationSystem>(); } }

class AnimationSystem : OwlScript
{
	public AnimationSystem()
	{
		Animations.Init();
		m_AnimationsUI = new AnimationsUI();
	}

	public static AnimationsUI GetAnimationsUI() { return m_AnimationsUI; }
	private static AnimationsUI m_AnimationsUI = null;
}