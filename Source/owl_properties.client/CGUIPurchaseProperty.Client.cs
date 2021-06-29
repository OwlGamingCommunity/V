class CGUIPurchaseProperty : CEFCore
{
	public CGUIPurchaseProperty(OnGUILoadedDelegate callbackOnLoad) : base("owl_properties.client/purchaseinterior.html", EGUIID.PurchaseProperty, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void ResetData()
	{
		Execute("ResetData");
	}

	public void SetDownpayment(float fDownpayment, float fMin, float fMax)
	{
		Execute("SetDownpayment", fDownpayment, fMin, fMax);
	}

	public void SetPriceInfo(float fPrice, float fInterest, float fCreditAmount, float fMonthlyPaymentTotalAmount, float fMonthlyPayment)
	{
		Execute("SetPriceInfo", fPrice, fInterest, fCreditAmount, fMonthlyPaymentTotalAmount, fMonthlyPayment);
	}

	public void AddDivider()
	{
		Execute("AddDivider");
	}

	public void AddPurchaser(string strPurchaserName)
	{
		Execute("AddPurchaser", strPurchaserName);
	}

	public void AddMethod(string strMethodName)
	{
		Execute("AddMethod", strMethodName);
	}

	public void CommitPurchasesAndMethods()
	{
		Execute("CommitPurchasesAndMethods");
	}
}