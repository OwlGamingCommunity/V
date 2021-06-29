internal class CGUIRegisterScreen : CEFCore
{

	public CGUIRegisterScreen(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/register.html", EGUIID.Register, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void HandleRegisterError(string strError, string[] strErrorDetails)
	{
		string strErrorMsg = strError;

		if (strErrorDetails != null)
		{
			foreach (string strErrorDetail in strErrorDetails)
			{
				strErrorMsg += "<br>";
				strErrorMsg += strErrorDetail;
			}
		}

		Execute("ShowRegisterBox", true, strErrorMsg);
	}

	public void Reset()
	{
		Execute("Reset");
	}
}