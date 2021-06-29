internal class CGUILoginScreen : CEFCore
{
	public CGUILoginScreen(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/login.html", EGUIID.Login, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void ShowLoginBox(bool bShowError, bool bAutoLoginChecked, string strErrorMessage)
	{
		Execute("ShowLoginBox", bShowError, bAutoLoginChecked, strErrorMessage);
	}

	public void DoBackendLogin(int userID, string titleID, string Username)
	{
		Execute("DoBackendLogin", userID, titleID, Username);
	}

	public void GotoAccessingAccount()
	{
		Execute("ShowInformativeMessage", "Accessing Account...");
	}

	public void Reset()
	{
		Execute("Reset");
	}
}