class CGUIPaydayOverview : CEFCore
{
	public CGUIPaydayOverview(OnGUILoadedDelegate callbackOnLoad) : base("owl_banking.client/paydayoverview.html", EGUIID.PaydayOverview, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void SetNoPaychecks()
	{
		Execute("SetNoPaychecks");
	}

	public void SetNoVehicles()
	{
		Execute("SetNoVehicles");
	}

	public void SetNoProperties()
	{
		Execute("SetNoProperties");
	}

	public void SetNumbers(int numPaychecks, int numVehicles, int numProperties)
	{
		Execute("SetNumbers", numPaychecks, numVehicles, numProperties);
	}

	public void Add_OverviewItem(string strDisplayName, float fAmount)
	{
		Execute("Add_OverviewItem", strDisplayName, fAmount);
	}

	public void Add_PaycheckItem(string strDisplayName, float fAmount)
	{
		Execute("Add_PaycheckItem", strDisplayName, fAmount);
	}

	public void Add_VehicleItem(string strDisplayName, float monthlyPayment, int paymentsRemaining, int paymentsMade, int paymentsMissed, bool missedPayment, bool wasRepossessed, float monthlyTax)
	{
		Execute("Add_VehicleItem", strDisplayName, monthlyPayment, paymentsRemaining, paymentsMade, paymentsMissed, missedPayment, wasRepossessed, monthlyTax);
	}

	public void Add_PropertyItem(string strDisplayName, float monthlyPayment, int paymentsRemaining, int paymentsMade, int paymentsMissed, bool missedPayment, bool wasRepossessed, float monthlyTax)
	{
		Execute("Add_PropertyItem", strDisplayName, monthlyPayment, paymentsRemaining, paymentsMade, paymentsMissed, missedPayment, wasRepossessed, monthlyTax);
	}
}