using System.Collections.Generic;
using System.Linq;

public static class Donations
{
	public static List<DonationPurchasable> g_lstPurchasables = null;

	static Donations()
	{

	}

	public static DonationPurchasable GetPropertyToken()
	{
		return g_lstPurchasables.FirstOrDefault(
			purchasable => purchasable.DonationEffect == EDonationEffect.PropertyToken
		);
	}

	public static DonationPurchasable GetVehicleToken()
	{
		return g_lstPurchasables.FirstOrDefault(
			purchasable => purchasable.DonationEffect == EDonationEffect.VehicleToken
		);
	}
}