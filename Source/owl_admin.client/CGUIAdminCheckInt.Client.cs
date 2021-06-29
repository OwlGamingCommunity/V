internal class CGUIAdminCheckInt : CEFCore
{
	public CGUIAdminCheckInt(OnGUILoadedDelegate callbackOnLoad) : base("owl_admin.client/checkint.html", EGUIID.AdminCheckInt, callbackOnLoad)
	{
		UIEvents.CloseCheckInt += () => { AdminSystem.GetAdminCheckInt().OnCloseCheckInt(); };
		UIEvents.SaveInteriorNote += (string strNote) => { AdminSystem.GetAdminCheckInt().OnSaveInteriorNote(strNote); };
		UIEvents.ReloadCheckIntData += () => { AdminSystem.GetAdminCheckInt().OnReloadCheckIntData(); };
	}

	public override void OnLoad()
	{

	}
}