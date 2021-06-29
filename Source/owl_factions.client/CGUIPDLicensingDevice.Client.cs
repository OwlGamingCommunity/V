internal class CGUIPDLicensingDevice : CEFCore
{
	public CGUIPDLicensingDevice(OnGUILoadedDelegate callbackOnLoad) : base("owl_factions.client/pdlicensedevice.html", EGUIID.PDLicensingDevice, callbackOnLoad)
	{
		UIEvents.HideLicenseDevice += () => { FactionSystem.GetPDSystem()?.OnHideLicenseDevice(); };
		UIEvents.FinalizeLicenseDevice += (string strTargetName, EWeaponLicenseType weaponLicenseType) => { FactionSystem.GetPDSystem()?.OnFinalizeLicenseDevice(strTargetName, weaponLicenseType); };
	}

	public override void OnLoad()
	{

	}
}