using System;

public class BinaryBuffer
{
	private byte[] m_Bytes = null;

	public BinaryBuffer(byte[] bytes)
	{
		m_Bytes = bytes;
	}

	public BinaryBuffer()
	{
		const int defaultBufferSize = 1024;
		m_Bytes = new byte[defaultBufferSize];
	}

	public void FinalizeBuffer()
	{
		// at this point we should consider it finalized / read only
		Array.Resize(ref m_Bytes, byteOffset);
	}

	public byte[] GetBytes()
	{
		return m_Bytes;
	}

	private int byteOffset = 0;
	public string ReadString()
	{
		// get length
		int len = ReadInt();

		// get string
		string buf = String.Empty;
		for (int i = 0; i < len; ++i)
		{
			char ch = BitConverter.ToChar(ReadFromBuffer(sizeof(char)));
			buf += (char)ch;
		}

		return buf;
	}

	public int ReadInt()
	{
		return BitConverter.ToInt32(ReadFromBuffer(sizeof(int)));
	}

	private void WriteToBuffer(byte[] b)
	{
		int len = b.Length * sizeof(byte);

		// Do we need to grow?
		const int growthAmount = 256;
		if (byteOffset + len > m_Bytes.Length)
		{
			int amountToGrow = Math.Max(growthAmount, len);
			Array.Resize(ref m_Bytes, m_Bytes.Length + amountToGrow);
		}

		Array.Copy(b, 0, m_Bytes, byteOffset, len);
		byteOffset += len;

		// TODO: What if the buffer is exhausted
	}

	private byte[] ReadFromBuffer(int numBytes)
	{
		byte[] bytes = new byte[numBytes];
		Array.Copy(m_Bytes, byteOffset, bytes, 0, numBytes);
		byteOffset += numBytes;
		return bytes;
	}

	public void WriteString(string val)
	{
		int len = (val).Length;
		WriteToBuffer(BitConverter.GetBytes(len));

		foreach (char ch in (string)val)
		{
			WriteToBuffer(BitConverter.GetBytes(ch));
		}
	}

	public void WRiteInt(int val)
	{
		WriteToBuffer(BitConverter.GetBytes(val));
	}

	public void WriteFloat(float val)
	{
		WriteToBuffer(BitConverter.GetBytes(val));
	}


	public float ReadFloat()
	{
		return BitConverter.ToSingle(ReadFromBuffer(sizeof(float)));
	}

	public bool ReadBool()
	{
		return BitConverter.ToBoolean(ReadFromBuffer(sizeof(bool)));
	}

	public void WriteBool(bool b)
	{
		WriteToBuffer(BitConverter.GetBytes(b));
	}
}