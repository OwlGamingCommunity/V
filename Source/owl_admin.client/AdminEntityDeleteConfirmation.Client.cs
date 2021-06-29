using System;

public class AdminEntityDeleteConfirmation
{
	private Int64 m_EntityID;
	private EEntityType m_EntityType;

	public AdminEntityDeleteConfirmation()
	{
		Reset();

		NetworkEvents.AdminConfirmEntityDelete += OnAdminConfirmEntityDelete;

		UIEvents.AdminEntityDeleteConfirmation_Yes += OnYes;
		UIEvents.AdminEntityDeleteConfirmation_No += OnNo;
	}

	private void OnYes()
	{
		NotificationManager.ShowNotification(Helpers.FormatString("{0} Deletion", m_EntityType), Helpers.FormatString("{0} with ID #{1} was deleted.", m_EntityType, m_EntityID), ENotificationIcon.ExclamationSign);

		if (m_EntityType == EEntityType.Vehicle)
		{
			NetworkEventSender.SendNetworkEvent_AdminDeleteVehicle(m_EntityID);
		}
		else if (m_EntityType == EEntityType.Property)
		{
			NetworkEventSender.SendNetworkEvent_AdminDeleteProperty(m_EntityID);
		}
		else if (m_EntityType == EEntityType.Elevator)
		{
			NetworkEventSender.SendNetworkEvent_AdminDeleteElevator(m_EntityID);
		}
		else if (m_EntityType == EEntityType.Faction)
		{
			NetworkEventSender.SendNetworkEvent_AdminDeleteFaction(m_EntityID);
		}

		Reset();
	}

	private void OnNo()
	{
		NotificationManager.ShowNotification(Helpers.FormatString("{0} Deletion", m_EntityType), Helpers.FormatString("Cancelled deletion of {0} with ID #{1}.", m_EntityType, m_EntityID), ENotificationIcon.ExclamationSign);

		Reset();
	}

	private void Reset()
	{
		m_EntityID = -1;
		m_EntityType = EEntityType.None;
	}

	private void OnAdminConfirmEntityDelete(Int64 entityID, EEntityType entityType)
	{
		m_EntityID = entityID;
		m_EntityType = entityType;

		GenericPromptHelper.ShowPrompt("Confirmation", Helpers.FormatString("Are you SURE you want to delete {0} with ID #{1}?", entityType, entityID), "YES, DELETE IT", "No! Keep It!", UIEventID.AdminEntityDeleteConfirmation_Yes, UIEventID.AdminEntityDeleteConfirmation_No);
	}
}