public static class NotificationManager
{
	static NotificationManager()
	{

	}

	public static void Init()
	{
		NetworkEvents.Notification += ShowNotification;

		m_gui.SetVisible(true, false, false);
	}

	public static void ShowNotification(string strTitle, string strMessage, ENotificationIcon icon)
	{
		string strIcon = NotificationIconHelpers.IconMap[icon];
		m_gui.ShowNotification(strTitle, strMessage, strIcon);
	}

	private static CGUINotifications m_gui = new CGUINotifications(() => { });
}

