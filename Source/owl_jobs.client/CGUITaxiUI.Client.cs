internal class CGUITaxiUI : CEFCore
{
	public CGUITaxiUI(OnGUILoadedDelegate callbackOnLoad) : base("owl_jobs.client/taximeter.html", EGUIID.TaxiUI, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void SetFarePerMile(float fCostPerMile)
	{
		Execute("SetFarePerMile", fCostPerMile);
	}

	public void SetCurrentFare(float fCurrentCost, float fDistanceTravelled)
	{
		Execute("SetCurrentFare", fCurrentCost, fDistanceTravelled);
	}

	public void SetIsDriver(bool bIsDriver)
	{
		Execute("SetIsDriver", bIsDriver);
	}

	public void SetAvailableForHire(bool bAvailableForHire)
	{
		Execute("SetAvailableForHire", bAvailableForHire);
	}
}