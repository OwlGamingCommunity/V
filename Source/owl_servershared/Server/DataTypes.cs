using System;

public struct SFactionRanksUpdateStruct : IEquatable<SFactionRanksUpdateStruct>
{
	public string Name { get; set; }
	public float Salary { get; set; }

	public bool Equals(SFactionRanksUpdateStruct rhs)
	{
		return Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SFactionRanksUpdateStruct))
		{
			return false;
		}

		SFactionRanksUpdateStruct rhsStruct = (SFactionRanksUpdateStruct)obj;
		return rhsStruct.Name == Name && rhsStruct.Salary == Salary;
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int result = 0;
			result = (result * 302) ^ Name.GetHashCode();
			result = (result * 302) ^ Convert.ToInt32(Salary);
			return result;
		}
	}

	public static bool operator ==(SFactionRanksUpdateStruct lhs, SFactionRanksUpdateStruct rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(SFactionRanksUpdateStruct lhs, SFactionRanksUpdateStruct rhs)
	{
		return !lhs.Equals(rhs);
	}
};