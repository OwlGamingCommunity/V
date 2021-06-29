public static class FastCRC32
{
	private static readonly uint[] ChecksumTable = new uint[0x100];
	private const uint Polynomial = 0xEDB88320;

	static FastCRC32()
	{
		for (uint index = 0; index < 0x100; ++index)
		{
			uint item = index;
			for (int bit = 0; bit < 8; ++bit)
				item = ((item & 1) != 0) ? (Polynomial ^ (item >> 1)) : (item >> 1);
			ChecksumTable[index] = item;
		}
	}

	public static uint ComputeHash(byte[] bytes)
	{
		uint result = 0xFFFFFFFF;

		foreach (byte current in bytes)
		{
			result = ChecksumTable[(result & 0xFF) ^ current] ^ (result >> 8);
		}

		return result;
	}
}