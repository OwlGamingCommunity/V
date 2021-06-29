using RAGE;

public static class VectorExtensions
{
	public static RAGE.Vector3 CopyVector(this RAGE.Vector3 vecToCopy)
	{
		return new RAGE.Vector3(vecToCopy.X, vecToCopy.Y, vecToCopy.Z);
	}

	public static string AsString(this RAGE.Vector3 vecToCopy)
	{
		return Helpers.FormatString("Vector3: ({0}, {1}, {2})", vecToCopy.X, vecToCopy.Y, vecToCopy.Z);
	}
}

public class Vector3Definition
{
	public Vector3Definition(float a_fX, float a_fY, float a_fZ)
	{
		X = a_fX;
		Y = a_fY;
		Z = a_fZ;
	}

	public RAGE.Vector3 AsRageVector()
	{
		return new RAGE.Vector3(X, Y, Z);
	}

	public float X { get; }
	public float Y { get; }
	public float Z { get; }

	public static implicit operator Vector3(Vector3Definition v)
	{
		return v.AsRageVector();
	}
}