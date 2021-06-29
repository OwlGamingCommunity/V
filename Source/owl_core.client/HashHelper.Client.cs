public static class HashHelper
{
	public static int GetHashSigned(string strName)
	{
		return (int)RAGE.Game.Misc.GetHashKey(strName);
	}

	public static uint GetHashUnsigned(string strName)
	{
		return RAGE.Game.Misc.GetHashKey(strName);
	}
}