using System;
using System.Collections.Generic;

public class CTattooDefinition
{
	public CTattooDefinition()
	{

	}

#if !SERVER
	public uint GetHash_Collection()
	{
		return HashHelper.GetHashUnsigned(CollectionName + "_overlays");
	}

	public uint GetHash_Tattoo(EGender gender)
	{
		return HashHelper.GetHashUnsigned(GetHashAsString(gender));
	}
#endif

	private string GetHashAsString(EGender gender)
	{
		return gender == EGender.Male ? HashNameMale : HashNameFemale;
	}

	public bool SupportsGender(EGender gender)
	{
		return GetHashAsString(gender).Length > 0;
	}

	public int ID { get; set; }
	public bool Enabled { get; set; }
	public string CollectionName { get; set; }
	public string LocalizedName { get; set; }
	public string HashNameMale { get; set; }
	public string HashNameFemale { get; set; }
	public TattooZone Zone { get; set; }
	public uint ZoneID { get; set; }
}

public class CHairTattooDefinition
{
	public int ID { get; set; }
	public string GXT { get; set; }
	public string Localized { get; set; }
	public int HairTattooCollection { get; set; }
	public int HairTattooOverlay { get; set; }
	public EGender Gender { get; set; }
}

public static class TattooDefinitions
{
	public static CTattooDefinition GetTattooDefinitionFromID(int id)
	{
		foreach (CTattooDefinition tattooDef in g_TattooDefinitions.Values)
		{
			if (Convert.ToUInt32(tattooDef.ID) == id)
			{
				return tattooDef;
			}
		}

		return null;
	}

	public static Dictionary<int, CTattooDefinition> g_TattooDefinitions = new Dictionary<int, CTattooDefinition>();

	public static CHairTattooDefinition GetHairTattooDefinitionFromID(int id)
	{
		foreach (CHairTattooDefinition tattooDef in g_HairTattooDefinitions.Values)
		{
			if (Convert.ToUInt32(tattooDef.ID) == id)
			{
				return tattooDef;
			}
		}

		return null;
	}

	public static Dictionary<int, CHairTattooDefinition> g_HairTattooDefinitions = new Dictionary<int, CHairTattooDefinition>();
}
