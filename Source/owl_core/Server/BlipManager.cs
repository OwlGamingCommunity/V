using GTANetworkAPI;
using System.Collections.Generic;

class BlipManager
{
	private List<CBlip> blips = new List<CBlip>();

	public BlipManager()
	{
		// SADPR Points Of Interest
		blips.Add(new CBlip(133, new Vector3(3312.229, 5176.667, 19.61458), "Point of Interest", true)); // Cape Catfish Lighthouse Manor
		blips.Add(new CBlip(133, new Vector3(1538.024, 6619.623, 2.350252), "Point of Interest", true)); // Paleto Cove
		blips.Add(new CBlip(133, new Vector3(98.02966, 6970.242, 10.48898), "Point of Interest", true)); // North Point
		blips.Add(new CBlip(133, new Vector3(-1580.418, 2102.549, 67.61237), "Point of Interest", true)); // Two Hoots Falls
		blips.Add(new CBlip(133, new Vector3(3387.356, 5507.438, 23.08015), "Point of Interest", true)); // Mount Gordo Campsite 1
		blips.Add(new CBlip(133, new Vector3(2934.456, 5321.006, 99.94461), "Point of Interest", true)); // Mount Gordo Campsite 2
		blips.Add(new CBlip(133, new Vector3(2550.846, 6150.613, 161.2355), "Point of Interest", true)); // Mount Gordo Secret Lake
		blips.Add(new CBlip(133, new Vector3(3387.356, 5507.438, 23.08015), "Point of Interest", true)); // Mount Gordo Secret Spring
		blips.Add(new CBlip(133, new Vector3(501.1378, 5604.599, 797.9099), "Point of Interest", true)); // Mount Chiliad Summit
		blips.Add(new CBlip(133, new Vector3(-201.9899, 3931.296, 34.30858), "Point of Interest", true)); // Alamo Sea Campsite
		blips.Add(new CBlip(133, new Vector3(2051.554, 3563.209, 40.37022), "Point of Interest", true)); // Grand Senora Campsite
		blips.Add(new CBlip(500, new Vector3(-118.268, 6455.69, 31.4), "Bank of Paleto Bay", true)); // Bank LS
		blips.Add(new CBlip(500, new Vector3(230.9888, 214.837, 105.9469), "Bank of Los Santos", true)); // Bank LS


		CreateBlips();
	}

	private void CreateBlips()
	{
		foreach (CBlip blip in blips)
		{
			HelperFunctions.Blip.Create(blip.position, blip.ShortRange, 50.0f, 0, blip.name, blip.sprite, -1, true);
		}
	}
}

