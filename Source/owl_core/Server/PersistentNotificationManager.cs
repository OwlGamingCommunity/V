using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using EntityDatabaseID = System.Int64;

public class PersistentNotificationManager
{
    private List<CPersistentNotification> m_lstNotifications;
    private CPlayer m_Player;

    public PersistentNotificationManager(CPlayer player)
    {
        m_Player = player;
    }

    public void LoadAll()
    {
        Database.Functions.Notifications.Get(
            m_Player.AccountID,
            notifications =>
            {
                m_lstNotifications = notifications
                    .Select(row => new CPersistentNotification(row.ID, row.Title, row.ClickEvent, row.Body, row.CreatedAt))
                    .ToList();

                NetworkEventSender.SendNetworkEvent_PersistentNotifications_LoadAll(m_Player, m_lstNotifications);
            }
        );
    }

    public void PushNotification(string title, string body, string clickEvent = null)
    {
        Database.Functions.Notifications.Create(m_Player.AccountID, title, clickEvent, body, 
        notificationID => {
            CPersistentNotification notification = new CPersistentNotification(notificationID, title, clickEvent, body, Helpers.GetUnixTimestamp());
            m_lstNotifications.Add(notification);
            NetworkEventSender.SendNetworkEvent_PersistentNotifications_Created(m_Player, notification);
        });
        
    }

    public void DeleteNotification(EntityDatabaseID notificationID)
    {
        Database.Functions.Notifications.Delete(notificationID);
    }

    public static void SendAccountNotification(EntityDatabaseID accountID, string title, string body, string clickEvent = "")
    {
        CPlayer player = PlayerPool.GetAllPlayers().FirstOrDefault(p => p.AccountID == accountID);
        if (player == null)
        {
            Database.Functions.Notifications.Create(accountID, title, clickEvent, body);
            return;
        }

        player.Notifications.PushNotification(title, body, clickEvent);
    }
}

public class PersistentNotifications 
{
    public PersistentNotifications()
    {
        NetworkEvents.PersistentNotifications_Deleted += OnNotificationDeleted;
    }

    public void OnNotificationDeleted(CPlayer player, EntityDatabaseID notificationID)
    {
        player.Notifications.DeleteNotification(notificationID);
    }
}