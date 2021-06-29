using System;
using System.Collections.Generic;

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

public static class BinarySerializer
{
	private static Dictionary<int, byte[]> m_cacheSerialization = new Dictionary<int, byte[]>();

	private static Dictionary<uint, object> m_cacheDeserialization = new Dictionary<uint, object>();

	public static T FastDeserialize<T>(byte[] bytes)
	{
		// check cache
		uint incomingHash = FastCRC32.ComputeHash(bytes);
		if (m_cacheDeserialization.Count > 0)
		{
			if (m_cacheDeserialization.ContainsKey(incomingHash))
			{
				return (T)m_cacheDeserialization[incomingHash];
			}
		}

		BinaryBuffer buf = new BinaryBuffer(bytes);
		T retVal = (T)FastDeserialize(ref buf, bytes, typeof(T));
		m_cacheDeserialization[incomingHash] = retVal;
		return retVal;
	}

	private static object FastDeserialize(ref BinaryBuffer buf, byte[] bytes, Type t)
	{
		if (t.FullName.ToString().StartsWith("System.Collections.Generic.List"))
		{
			object o = Activator.CreateInstance(t);

			// read length
			int len = buf.ReadInt();

			Type[] typeParameters = t.GetGenericArguments();
			Type listTemplateType = typeParameters[0];

			for (int i = 0; i < len; ++i)
			{
				object inner = FastDeserialize(ref buf, bytes, listTemplateType);
				((System.Collections.IList)o).Add(inner);
			}

			return o;
		}
		if (t == typeof(string))
		{
			return buf.ReadString();
		}
		else if (t.IsEnum)
		{
			return buf.ReadInt();
		}
		else if (t == typeof(int))
		{
			return buf.ReadInt();
		}
		else if (t == typeof(float))
		{
			return buf.ReadFloat();
		}
		else if (t == typeof(bool))
		{
			return buf.ReadBool();
		}
		else if (t.IsClass) // we must be serializing a class, or list of classes
		{
			object o = Activator.CreateInstance(t);

			System.Reflection.MemberInfo[] arrMemberInfo = t.GetMembers();
			foreach (System.Reflection.MemberInfo memberInfo in arrMemberInfo)
			{
				// TODO: Support memberInfo.MemberType == System.Reflection.MemberTypes.Field
				if (memberInfo.MemberType == System.Reflection.MemberTypes.Property)
				{
					System.Reflection.PropertyInfo field = t.GetProperty(memberInfo.Name);

					object child = FastDeserialize(ref buf, bytes, field.PropertyType);

					field.SetValue(o, child);
				}
			}

			return o;
		}

		// TODO: Error
		return null;
	}

	public static BinaryBuffer FastSerialize(object input)
	{
		// check cache
		int incomingObject = input.GetHashCode();
		if (m_cacheSerialization.Count > 0)
		{
			if (m_cacheSerialization.ContainsKey(incomingObject))
			{
				return new BinaryBuffer(m_cacheSerialization[incomingObject]);
			}
		}

		BinaryBuffer buf = new BinaryBuffer();
		FastSerialize(input, ref buf);
		buf.FinalizeBuffer();
		m_cacheSerialization[incomingObject] = buf.GetBytes();
		return buf;
	}

	private static void FastSerialize(object input, ref BinaryBuffer buf)
	{

		// we must be serializing a class or a list of classes
		if (input is System.Collections.IList)
		{
			// write length
			buf.WRiteInt(((System.Collections.IList)input).Count);

			foreach (var item in (System.Collections.IList)input)
			{
				FastSerialize(item, ref buf);
			}
		}
		else if (input is Enum)
		{
			buf.WRiteInt((int)input);
		}
		else if (input is string)
		{
			buf.WriteString((string)input);
		}
		else if (input is int)
		{
			buf.WRiteInt((int)input);
		}
		else if (input is float)
		{
			buf.WriteFloat((float)input);
		}
		else if (input is bool)
		{
			buf.WriteBool((bool)input);
		}
		else if (input.GetType().IsClass)
		{
			// TODO: Support direct serialization of types?
			System.Reflection.MemberInfo[] arrMemberInfo = input.GetType().GetMembers();
			foreach (System.Reflection.MemberInfo memberInfo in arrMemberInfo)
			{
				// TODO: Support memberInfo.MemberType == System.Reflection.MemberTypes.Field
				if (memberInfo.MemberType == System.Reflection.MemberTypes.Property)
				{
					System.Reflection.PropertyInfo field = input.GetType().GetProperty(memberInfo.Name);
					object val = field.GetValue(input);

					FastSerialize(val, ref buf);
				}
			}
		}
	}
}