using System;

namespace HelperFunctions
{
	public static class Vehicle
	{
		public static string GenerateLicensePlate(long uniqueID)
		{
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			string strInput = Helpers.FormatString("{0}{1}{2}{3}", uniqueID, new Random().Next(), unixTimestamp, uniqueID);

			string strHash = HelperFunctions.Hashing.sha256(strInput);
			return Helpers.FormatString("{0}{1}", strHash.Substring(0, 4), strHash.Substring(28, 4)).ToUpper();
		}
	}
}
