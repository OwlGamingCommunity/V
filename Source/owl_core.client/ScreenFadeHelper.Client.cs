using System;

public enum EScreenFadeState
{
	None,
	FadeOut,
	FadeWait,
	FadeIn
}

public static class ScreenFadeHelper
{
	private static EScreenFadeState m_FadeState = EScreenFadeState.None;

	private static int m_timeMSPerFade = 0;
	private static int m_timeMSBetweenFades = 0;
	private static DateTime m_StartTime = DateTime.Now;
	private static RAGE.RGBA m_Color = null;

	private static OnFadeDelegate m_OnFadeOutStart = null;
	private static OnFadeDelegate m_OnFadeOutComplete = null;
	private static OnFadeDelegate m_OnFadeInStart = null;
	private static OnFadeDelegate m_OnFadeInComplete = null;

	static ScreenFadeHelper()
	{

	}

	public static void Init()
	{
		RageEvents.RAGE_OnRender += OnRender;
	}

	private static void OnRender()
	{
		if (m_FadeState == EScreenFadeState.FadeOut)
		{
			// calculate our progress
			double timeSinceStart = (DateTime.Now - m_StartTime).TotalMilliseconds;

			int alpha = (int)((timeSinceStart / m_timeMSPerFade) * 255.0f);

			RAGE.Game.Graphics.DrawRect(0.0f, 0.0f, 2.0f, 2.0f, (int)m_Color.Red, (int)m_Color.Green, (int)m_Color.Blue, alpha, 0);

			if (timeSinceStart >= m_timeMSPerFade)
			{
				m_OnFadeOutComplete?.Invoke();

				// reset start time
				m_StartTime = DateTime.Now;

				// start fade in
				m_FadeState = EScreenFadeState.FadeWait;
			}
		}
		else if (m_FadeState == EScreenFadeState.FadeWait)
		{
			RAGE.Game.Graphics.DrawRect(0.0f, 0.0f, 2.0f, 2.0f, (int)m_Color.Red, (int)m_Color.Green, (int)m_Color.Blue, 255, 0);

			double timeSinceStart = (DateTime.Now - m_StartTime).TotalMilliseconds;

			if (timeSinceStart >= m_timeMSBetweenFades)
			{
				m_OnFadeInStart?.Invoke();

				// reset start time
				m_StartTime = DateTime.Now;

				// start fade in
				m_FadeState = EScreenFadeState.FadeIn;
			}
		}
		else if (m_FadeState == EScreenFadeState.FadeIn)
		{
			// calculate our progress
			double timeSinceStart = (DateTime.Now - m_StartTime).TotalMilliseconds;

			int alpha = 255 - (int)((timeSinceStart / m_timeMSPerFade) * 255.0f);

			RAGE.Game.Graphics.DrawRect(0.0f, 0.0f, 2.0f, 2.0f, (int)m_Color.Red, (int)m_Color.Green, (int)m_Color.Blue, alpha, 0);

			if (timeSinceStart >= m_timeMSPerFade)
			{
				m_OnFadeInComplete?.Invoke();

				// start fade in
				m_FadeState = EScreenFadeState.None;
			}
		}
	}

	public static bool BeginFade(int timeMSPerFade, int timeMSBetweenFades, OnFadeDelegate OnFadeOutStart, OnFadeDelegate OnFadeOutComplete, OnFadeDelegate OnFadeInStart, OnFadeDelegate OnFadeInComplete, uint r = 0, uint g = 0, uint b = 0)
	{
		if (!IsFading())
		{
			m_timeMSPerFade = timeMSPerFade;
			m_timeMSBetweenFades = timeMSBetweenFades;
			m_StartTime = DateTime.Now;

			m_FadeState = EScreenFadeState.FadeOut;
			m_Color = new RAGE.RGBA(r, g, b);

			m_OnFadeOutStart = OnFadeOutStart;
			m_OnFadeOutComplete = OnFadeOutComplete;
			m_OnFadeInStart = OnFadeInStart;
			m_OnFadeInComplete = OnFadeInComplete;

			m_OnFadeOutStart?.Invoke();

			return true;
		}

		return false;
	}

	public static bool IsFading()
	{
		return m_FadeState != EScreenFadeState.None;
	}

	public delegate void OnFadeDelegate();
}