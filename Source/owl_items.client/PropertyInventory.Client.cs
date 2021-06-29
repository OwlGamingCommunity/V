using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class PropertyInventory
{
	EntityDatabaseID m_CurrentProperty = -1;
	private bool m_bIsInPropertyInventory = false;
	private EMailboxAccessType m_CurrentPropertyAccessLevel = EMailboxAccessType.NoAccess;

	public PropertyInventory()
	{
		NetworkEvents.PropertyMailboxDetails += OnGotPropertyInventoryDetails;
	}

	public EntityDatabaseID GetCurrentPropertyID()
	{
		return m_CurrentProperty;
	}

	private void OnGotPropertyInventoryDetails(Int64 propertyID, EMailboxAccessType accessLevel, List<CItemInstanceDef> inventory)
	{
		m_CurrentProperty = propertyID;
		m_CurrentPropertyAccessLevel = accessLevel;

		ItemSystem.GetPlayerInventory().ShowInventory();

		ItemSystem.GetPlayerInventory().SetCurrentPropertyInventory(inventory);
		ItemSystem.GetPlayerInventory().OnExpandContainer(-1, EItemSocket.Property_Mailbox, false);

		m_bIsInPropertyInventory = true;
	}

	public void ClosePropertyInventory()
	{
		if (m_bIsInPropertyInventory)
		{
			m_bIsInPropertyInventory = false;

			NetworkEventSender.SendNetworkEvent_ClosePropertyInventory();
			m_CurrentProperty = -1;
			m_CurrentPropertyAccessLevel = EMailboxAccessType.NoAccess;
		}
	}
}