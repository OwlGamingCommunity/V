using System;
using System.Collections.Generic;
using System.Text;

public class CVehicleDefinition
{
	public CVehicleDefinition()
	{

	}

	public uint Hash { get; set; }
	public int Index { get; set; }
	public string Manufacturer { get; set; }
	public string Name { get; set; }
	public string Class { get; set; }
	public bool IsPurchasable { get; set; }
	public float Price { get; set; }
	public bool IsRentable { get; set; }
	public float RentalPricePerDay { get; set; }
	public bool CanBuyWithToken { get; set; } = false;
	public string AddOnName { get; set; } = string.Empty;
	public Dictionary<int, bool> DefaultExtras { get; set; } = new Dictionary<int, bool>();
}

public static class VehicleDefinitions
{
	public static CVehicleDefinition GetVehicleDefinitionFromIndex(int index)
	{
		foreach (CVehicleDefinition vehicleDef in g_VehicleDefinitions.Values)
		{
			if (Convert.ToUInt32(vehicleDef.Index) == index)
			{
				return vehicleDef;
			}
		}

		return null;
	}

	public static CVehicleDefinition GetVehicleDefinitionFromHash(uint hash)
	{
		foreach (CVehicleDefinition vehicleDef in g_VehicleDefinitions.Values)
		{
			// Lets check for add-on vehicles
			if (!string.IsNullOrEmpty(vehicleDef.AddOnName) && HashAddonName(vehicleDef.AddOnName) == hash)
			{
				return vehicleDef;
			}

			if (Convert.ToUInt32(vehicleDef.Hash) == hash)
			{
				return vehicleDef;
			}
		}

		return g_VehicleDefinitions[0];
	}

	public static CVehicleDefinition GetVehicleDefinitionFromAddon(uint addonHashKey)
	{
		foreach (CVehicleDefinition vehicleDef in g_VehicleDefinitions.Values)
		{
			if (HashAddonName(vehicleDef.AddOnName) == addonHashKey)
			{
				return vehicleDef;
			}
		}

		return null;
	}

	private static uint HashAddonName(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return 0U;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(input.ToLower().ToCharArray());
		uint num = 0U;
		int i = 0;
		int num2 = bytes.Length;
		while (i < num2)
		{
			num += (uint)bytes[i];
			num += num << 10;
			num ^= num >> 6;
			i++;
		}
		num += num << 3;
		num ^= num >> 11;
		return num + (num << 15);
	}

	public static Dictionary<int, CVehicleDefinition> g_VehicleDefinitions = new Dictionary<int, CVehicleDefinition>();
}
