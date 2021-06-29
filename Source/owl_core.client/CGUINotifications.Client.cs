public class CGUINotifications : CEFCore
{
	public CGUINotifications(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/notifications.html", EGUIID.Notifications, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void ShowNotification(string strTitle, string strMessage, string strIcon)
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "Retune_High", "MP_RADIO_SFX", true);
		Execute("ShowNotification", strTitle, strMessage, strIcon);
	}

	public void ShowGenericError(string strTitle, string strMessage, string strSource)
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "Retune_High", "MP_RADIO_SFX", true);
		Execute("ShowError", strTitle, strMessage, strSource);
	}
}