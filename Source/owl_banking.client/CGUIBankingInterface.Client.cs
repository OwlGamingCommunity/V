class CGUIBankingInterface : CEFCore
{
	public CGUIBankingInterface(OnGUILoadedDelegate callbackOnLoad) : base("owl_banking.client/bankinginterface.html", EGUIID.BankingInterface, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void ResetCredits(bool bResetDropdownSelectedText)
	{
		Execute("ResetCredits", bResetDropdownSelectedText);
	}

	public void AddDivider()
	{
		Execute("AddDivider");
	}

	public void AddAccount(string strDisplayName)
	{
		Execute("AddAccount", strDisplayName);
	}

	public void CommitAccounts()
	{
		Execute("CommitAccounts");
	}

	public void SetAccount(int accountIndex)
	{
		Execute("SetAccount", accountIndex);
	}

	public void SetBalance(float fBalance)
	{
		Execute("SetBalance", fBalance);
	}

	public void AddCredit(string strName)
	{
		Execute("AddCredit", strName);
	}

	public void SetButtonsEnabled(bool bEnabled)
	{
		Execute("SetButtonsEnabled", bEnabled);
	}

	public void SetCreditInfo(string strName, int paymentsRemaining, int paymentsMade, float fRemainingCreditAmount, float fInterest)
	{
		Execute("SetCreditInfo", strName, paymentsRemaining, paymentsMade, fRemainingCreditAmount, fInterest);
	}
}