using System.Collections.Generic;

public class CGUIHUD : CEFCore
{
	public CGUIHUD(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/hud.html", EGUIID.HUD, callbackOnLoad)
	{

	}

	public override void OnLoad()
	{

	}

	public void SetLocation(string strLocation)
	{
		ExecuteDelayed_OverwriteDupes("SetLocation", strLocation);
	}

	public void SetProgressBar(string text, string width)
	{
		Execute("SetProgressBar", text, width);
	}

	public void HideProgressBar()
	{
		Execute("HideProgressBar");
	}

	public void ShowNotification(string strTitle, string strMessage, ENotificationIcon icon)
	{
		string strIcon = Helpers.FormatString("glyphicon glyphicon-{0}", System.Text.RegularExpressions.Regex.Replace(icon.ToString(), "(?<=[a-z])([A-Z])", "-$1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim().ToLower());

		RAGE.Game.Audio.PlaySoundFrontend(-1, "Retune_High", "MP_RADIO_SFX", true);
		Execute("ShowNotification", strTitle, strMessage, strIcon);
	}

	public void PushPersistentNotification(CPersistentNotification notification)
	{
		Execute("PushPersistentNotification", notification.ID, notification.Title, notification.ClickEvent, notification.Body, notification.CreatedAt);
	}

	public void SetPersistentNotifications(List<CPersistentNotification> lstNotifications)
	{
		foreach (var notification in lstNotifications)
		{
			PushPersistentNotification(notification);
		}
	}

	public void ShowSubtitle(string strMessage, int time)
	{
		Execute("ShowSubtitle", strMessage, time);
	}

	public void ShowRadio(string strMessage)
	{
		Execute("ShowRadio", strMessage);
	}

	public void SetEditInteriorVisible(bool bVisible)
	{
		Execute("SetEditInteriorVisible", bVisible);
	}

	public void ToggleF10Menu(bool bOpen)
	{
		Execute("ToggleHudMenu", bOpen);
	}
}