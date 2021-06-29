internal class CGUIAdminCheckVeh : CEFCore
{
	public CGUIAdminCheckVeh(OnGUILoadedDelegate callbackOnLoad) : base("owl_admin.client/checkveh.html", EGUIID.AdminCheckVeh, callbackOnLoad)
	{
		UIEvents.CloseCheckVeh += () => { AdminSystem.GetAdminCheckVeh().OnCloseCheckVeh(); };
		UIEvents.SaveVehicleNote += (string strNote) => { AdminSystem.GetAdminCheckVeh().OnSaveVehicleNote(strNote); };
		UIEvents.ReloadCheckVehData += () => { AdminSystem.GetAdminCheckVeh().OnReloadCheckVehData(); };
		UIEvents.UpdateStolenState += (bool stolen) => { AdminSystem.GetAdminCheckVeh().OnUpdateStolenState(stolen); };
	}

	public override void OnLoad()
	{

	}
}