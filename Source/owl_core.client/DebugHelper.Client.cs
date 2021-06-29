public static class DebugHelper
{
	private static bool m_bIsDebug = false;

	static DebugHelper()
	{

	}

	public static bool IsDebug()
	{
		return m_bIsDebug;
	}

	public static void SetDebug(bool bIsDebug)
	{
		m_bIsDebug = bIsDebug;
	}
}