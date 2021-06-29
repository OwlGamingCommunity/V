using System.Security.Cryptography;
using System.Text;

namespace HelperFunctions
{
	public static class Hashing
	{
		public static string sha256(string input)
		{
			SHA256Managed crypt = new SHA256Managed();
			string hash = string.Empty;
			byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(input), 0, Encoding.ASCII.GetByteCount(input));
			foreach (byte theByte in crypto)
			{
				hash += theByte.ToString("x2");
			}
			return hash;
		}
	}
}
