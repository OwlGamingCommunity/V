using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class CVehicleInventory
{
	public CVehicleInventory(CVehicle a_OwningVehicle)
	{
		m_OwningVehicle.SetTarget(a_OwningVehicle);
		Reset();
	}

	public string GetAsJSON()
	{
		return JsonConvert.SerializeObject(m_arrInventorySlots);
	}

	private void RemoveAnyNonVehicleItems()
	{
		List<CItemInstanceDef> itemsToRemove = new List<CItemInstanceDef>();
		foreach (CItemInstanceDef item in m_arrInventorySlots.ToArray())
		{
			if (item.CurrentSocket != EItemSocket.None)
			{
				if (!InventoryHelpers.IsSocketAVehicleSocket(item.CurrentSocket))
				{
					itemsToRemove.Add(item);
				}
			}
		}

		foreach (CItemInstanceDef item in itemsToRemove)
		{
			m_arrInventorySlots.Remove(item);
		}
	}

	public void CopyInventory(List<CItemInstanceDef> arrInventorySlots)
	{
		if (arrInventorySlots != null)
		{
			m_arrInventorySlots = arrInventorySlots;

			// SAFETY: Remove any items that are not in a vehicle socket (if socket is not none)
			RemoveAnyNonVehicleItems();
		}
	}

	public void Reset()
	{
		m_arrInventorySlots.Clear();
	}
	public List<CItemInstanceDef> GetItemsInsideContainer(EntityDatabaseID parentID)
	{
		List<CItemInstanceDef> lstContainerItems = new List<CItemInstanceDef>();
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.ParentType == EItemParentTypes.Container && item.ParentDatabaseID == parentID)
			{
				lstContainerItems.Add(item);
			}
		}

		return lstContainerItems;
	}

	public List<CItemInstanceDef> GetAllItems()
	{
		return m_arrInventorySlots;
	}

	private List<CItemInstanceDef> m_arrInventorySlots = new List<CItemInstanceDef>();
	private WeakReference<CVehicle> m_OwningVehicle = new WeakReference<CVehicle>(null);

	public CItemInstanceDef GetItemFromDBID(EntityDatabaseID a_DBID)
	{
		CItemInstanceDef itemInst = null;
		foreach (var item in m_arrInventorySlots.ToArray())
		{
			if (item.DatabaseID == a_DBID)
			{
				itemInst = item;
				break;
			}
		}

		return itemInst;
	}

	public List<CItemInstanceDef> LocalRemoveItem_NoDBOperation_Recursive(CItemInstanceDef itemToRemove)
	{
		List<CItemInstanceDef> lstItemsRemoved = new List<CItemInstanceDef>();

		// NOTE: This function ONLY removes it locally, it does NOT touch the database. Use SetItemSocket/SetItemInContainer to achieve that

		if (itemToRemove != null)
		{
			// remove root item
			m_arrInventorySlots.Remove(itemToRemove);
			lstItemsRemoved.Add(itemToRemove);

			// Look for child containers and recurse, looking for more children
			foreach (var slot in m_arrInventorySlots.ToArray())
			{
				if (slot.ParentType == EItemParentTypes.Container && slot.ParentDatabaseID == itemToRemove.DatabaseID)
				{
					lstItemsRemoved.AddRange(LocalRemoveItem_NoDBOperation_Recursive(slot));
				}
			}
		}

		return lstItemsRemoved;
	}

	public List<CItemInstanceDef> RemoveAndDestroyItem_Recursive(CItemInstanceDef itemToRemove)
	{
		List<CItemInstanceDef> lstItemsRemoved = new List<CItemInstanceDef>();

		if (itemToRemove != null)
		{
			// remove root item
			m_arrInventorySlots.Remove(itemToRemove);
			lstItemsRemoved.Add(itemToRemove);
			// queue queries on background thread
			Database.Functions.Items.RemoveEntityItem(itemToRemove, null);

			// Look for child containers and recurse, looking for more children
			foreach (var slot in m_arrInventorySlots.ToArray())
			{
				if (slot.ParentType == EItemParentTypes.Container && slot.ParentDatabaseID == itemToRemove.DatabaseID)
				{
					lstItemsRemoved.AddRange(LocalRemoveItem_NoDBOperation_Recursive(slot));
				}
			}
		}

		return lstItemsRemoved;
	}

	public void SetItemSocket(CItemInstanceDef sourceItem, EntityDatabaseID a_ItemDBID, EItemSocket a_SocketID)
	{
		CVehicle pVehicle = m_OwningVehicle.Instance();
		if (pVehicle == null)
		{
			return;
		}

		// TODO_INVENTORY: We have to put removed items in another suitable socket
		if (sourceItem != null)
		{
			EntityDatabaseID vehicleID = pVehicle.m_DatabaseID;
			EntityDatabaseID dbID = sourceItem.DatabaseID;
			EItemParentTypes parentType = EItemParentTypes.Vehicle;
			EntityDatabaseID parentID = vehicleID;

			Database.Functions.Items.SetItemBinding(parentID, sourceItem.ParentDatabaseID, sourceItem.ParentType, dbID, a_SocketID, parentType);

			sourceItem.CurrentSocket = a_SocketID;
			sourceItem.ParentDatabaseID = vehicleID;
			sourceItem.ParentType = parentType;

			m_arrInventorySlots.Add(sourceItem);
		}
		else
		{
			// TODO: Log error
		}
	}

	public void SetItemInContainer(CItemInstanceDef sourceItem, EntityDatabaseID a_ItemDBID, EntityDatabaseID a_ContainerID, bool bGoingToSocketContainer)
	{
		CVehicle pVehicle = m_OwningVehicle.Instance();
		if (pVehicle == null)
		{
			return;
		}

		// TODO_INVENTORY: Vehicles seem to have unlimited space
		// TODO_INVENTORY: We have to put removed items in another suitable socket
		if (sourceItem != null)
		{
			EntityDatabaseID vehicleID = pVehicle.m_DatabaseID;
			EntityDatabaseID dbID = sourceItem.DatabaseID;
			EItemParentTypes parentType = EItemParentTypes.Container;
			EntityDatabaseID parentID = a_ContainerID;

			EItemSocket targetSocket = EItemSocket.None;

			if (bGoingToSocketContainer)
			{
				targetSocket = (EItemSocket)a_ContainerID;
				if (targetSocket == EItemSocket.Vehicle_Console_And_Glovebox || targetSocket == EItemSocket.Vehicle_Seats || targetSocket == EItemSocket.Vehicle_Trunk)
				{
					parentType = EItemParentTypes.Vehicle;
					parentID = vehicleID;
				}
				Database.Functions.Items.SetItemBinding(parentID, sourceItem.ParentDatabaseID, sourceItem.ParentType, dbID, targetSocket, parentType);
			}
			else
			{
				Database.Functions.Items.SetItemBinding(parentID, sourceItem.ParentDatabaseID, sourceItem.ParentType, dbID, EItemSocket.None, parentType);
			}

			sourceItem.CurrentSocket = targetSocket;
			sourceItem.ParentDatabaseID = parentID;
			sourceItem.ParentType = parentType;

			m_arrInventorySlots.Add(sourceItem);
		}
		else
		{
			// TODO: Log error
		}
	}

	public bool AddItemToSocket(CItemInstanceDef a_ItemInstanceDef, EItemSocket a_VehicleSocket)
	{
		CVehicle pVehicle = m_OwningVehicle.Instance();
		if (pVehicle == null)
		{
			return false;
		}

		CInventoryItemDefinition newItemDef = ItemDefinitions.g_ItemDefinitions[a_ItemInstanceDef.ItemID];

		// Update item def
		a_ItemInstanceDef.SetBinding(a_VehicleSocket, pVehicle.m_DatabaseID, EItemParentTypes.Vehicle);
		m_arrInventorySlots.Add(a_ItemInstanceDef);

		// Store in DB
		Database.Functions.Items.GiveEntityItem(a_ItemInstanceDef, (EntityDatabaseID dbID) =>
		{
			a_ItemInstanceDef.DatabaseID = dbID;
		});

		return true;
	}

	public bool AddContainerItem(CItemInstanceDef a_ItemInstanceDef, EntityDatabaseID parentID)
	{
		// Update item def
		a_ItemInstanceDef.SetBinding(EItemSocket.None, parentID, EItemParentTypes.Container);
		m_arrInventorySlots.Add(a_ItemInstanceDef);

		// Store in DB
		Database.Functions.Items.GiveEntityItem(a_ItemInstanceDef, (EntityDatabaseID dbID) =>
		{
			a_ItemInstanceDef.DatabaseID = dbID;
		});

		return true;
	}
}