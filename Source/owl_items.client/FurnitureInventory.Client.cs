using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class FurnitureInventory
{
	RAGE.Elements.MapObject m_CurrentFurnitureItem = null;
	private bool m_bIsInFurnitureInventory = false;
	private EntityDatabaseID m_CurrentFurnitureDBID = -1;
	private uint m_CurrentFurnitureID = 0;

	public FurnitureInventory()
	{
		RageEvents.RAGE_OnRender += OnRender;
		NetworkEvents.FurnitureInventoryDetails += OnGotFurnitureInventoryDetails;
	}

	public RAGE.Elements.MapObject GetCurrentFurniture()
	{
		return m_CurrentFurnitureItem;
	}

	private void OnGotFurnitureInventoryDetails(List<CItemInstanceDef> inventory)
	{
		ItemSystem.GetPlayerInventory().ShowInventory();

		ItemSystem.GetPlayerInventory().SetCurrentFurnitureInventory(inventory);
		ItemSystem.GetPlayerInventory().OnExpandContainer(-1, EItemSocket.PlacedFurnitureStorage, false);
	}

	public EntityDatabaseID GetCurrentFurnitureItemDBID()
	{
		return m_CurrentFurnitureDBID;
	}

	public CFurnitureDefinition GetCurrentFurnitureItemDefinition()
	{
		if (m_bIsInFurnitureInventory)
		{
			if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(m_CurrentFurnitureID))
			{
				var furnitureDef = FurnitureDefinitions.g_FurnitureDefinitions[m_CurrentFurnitureID];
				return furnitureDef;
			}
		}

		return null;
	}

	public void CloseFurnitureInventory()
	{
		if (m_bIsInFurnitureInventory)
		{
			m_bIsInFurnitureInventory = false;
			NetworkEventSender.SendNetworkEvent_CloseFurnitureInventory();
			m_CurrentFurnitureItem = null;
		}
	}

	private void OnRender()
	{
		if (!ItemSystem.GetPlayerInventory().IsVisible() && RAGE.Elements.Player.LocalPlayer.Vehicle == null)
		{
			CPropertyFurnitureInstance furnitureInstance = FurnitureSystem.GetNearestStreamedFurnitureItemWithAction(); // shared function so its the nearest of ALL actionable items and not just the first category found

			if (furnitureInstance != null)
			{
				RAGE.Vector3 vecPos = furnitureInstance.m_Object.Position;
				uint FurnitureID = furnitureInstance.FurnitureID;

				// get furniture name
				if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(FurnitureID))
				{
					CFurnitureDefinition furnDef = FurnitureDefinitions.g_FurnitureDefinitions[FurnitureID];

					m_CurrentFurnitureDBID = furnitureInstance.DBID;
					m_CurrentFurnitureID = furnitureInstance.FurnitureID;

					if (furnDef.AllowOutfitChange)
					{
						WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Edit Outfits", null, InteractWithFurnitureOutfitChange, vecPos, furnitureInstance.m_Object.Dimension, false, false, ItemConstants.g_fDistFurnitureStorageThreshold, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true, false);
					}
					else if (furnDef.StorageCapacity > 0)
					{
						vecPos.Z += 1.0f;
						WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), Helpers.FormatString("Access '{0}'", furnDef.Name), null, InteractWithFurnitureInventory, vecPos, furnitureInstance.m_Object.Dimension, false, false, ItemConstants.g_fDistFurnitureStorageThreshold, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true, false);
					}

				}
			}
		}
	}

	private void InteractWithFurnitureInventory()
	{
		m_bIsInFurnitureInventory = true;
		NetworkEventSender.SendNetworkEvent_RequestFurnitureInventory(m_CurrentFurnitureDBID);
	}

	private void InteractWithFurnitureOutfitChange()
	{
		NetworkEventSender.SendNetworkEvent_EnterOutfitEditor();
	}
}