public static class Taxation
{

	public static float GetSalesTax()
	{
		return g_fStateSalesTax;
	}

	public static float GetPropertyMonthlyTax()
	{
		return g_fStateMonthlyTaxRateProperty;
	}

	public static float GetPropertyDownpaymentPercent()
	{
		return g_fPropertyDownpaymentPercent;
	}

	public static int GetPropertyNumMonthlyPayments()
	{
		return g_iPropertyNumMonthlyPayments;
	}

	public static float GetStateIncomeTax()
	{
		return g_fStateIncomeTax;
	}

	public static float GetFederalIncomeTax()
	{
		return g_fFederalIncomeTax;
	}

	public static float GetPaymentPlanInterestPercent()
	{
		return g_fPaymentPlanInterest;
	}

	public static float GetVehicleMonthlyTaxRate(int a_VehicleClass)
	{
		if (a_VehicleClass > -1 && a_VehicleClass < g_fVehicleMonthlyTaxRates.Length)
		{
			return g_fVehicleMonthlyTaxRates[a_VehicleClass];
		}

		return 0.0f;
	}

	private static readonly float[] g_fVehicleMonthlyTaxRates =
	{
		5.0f, // VehicleClass_Compacts
		30.0f, // VehicleClass_Sedans
		50.0f, // VehicleClass_SUVs
		30.0f, // VehicleClass_Coupes
		40.0f, // VehicleClass_Muscle
		40.0f, // VehicleClass_SportsClassics
		40.0f, // VehicleClass_Sports
		140.0f, // VehicleClass_Super
		40.0f, // VehicleClass_Motorcycles
		50.0f, // VehicleClass_OffRoad
		80.0f, // VehicleClass_Industrial 
		80.0f, // VehicleClass_Utility 
		60.0f, // VehicleClass_Vans 
		0.0f, // VehicleClass_Cycles 
		160.0f, // VehicleClass_Boats 
		1200.0f, // VehicleClass_Helicopters 
		2400.0f, // VehicleClass_Planes 
		0.0f, // VehicleClass_Service 
		0.0f, // VehicleClass_Emergency 
		0.0f, // VehicleClass_Military 
		60.0f, // VehicleClass_Commercial 
		0.0f, // VehicleClass_Trains 
	};

	private const float g_fStateSalesTax = 0.05f;

	private const float g_fStateMonthlyTaxRateProperty = 0.0005f;
	private const float g_fPropertyDownpaymentPercent = 0.10f;
	private const int g_iPropertyNumMonthlyPayments = 100;

	// TODO: Add tiered federal income tax, more earnings = more %
	private const float g_fStateIncomeTax = 0.08f;
	private const float g_fFederalIncomeTax = 0.10f;

	private const float g_fPaymentPlanInterest = 0.10f;
}