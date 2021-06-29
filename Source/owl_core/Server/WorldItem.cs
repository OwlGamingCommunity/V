using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

public class CWorldItem : CBaseEntity
{
	public CWorldItem(CItemInstanceDef a_ItemInstance, Vector3 a_vecPos, float a_fRotZ, Int64 a_DroppedByID, Dimension a_Dimension, float a_fRotX = -90.0f, float a_fRotY = 0.0f)
	{
		ItemInstance = a_ItemInstance;
		DroppedByID = a_DroppedByID;
		m_DatabaseID = ItemInstance.DatabaseID; // For sanity... Not setting this caused bugs in the past

		float fRotX = a_fRotX;
		float fRotY = a_fRotY;
		string model = ItemDefinitions.g_ItemDefinitions[ItemInstance.ItemID].Model;


		// Fix to make boombox stay upright
		if (a_ItemInstance.ItemID == EItemID.BOOMBOX)
		{
			fRotX = 0.0f;
			a_vecPos.Z += 0.215f;
		}

		if (a_ItemInstance.ItemID == EItemID.NOTE)
		{
			fRotX = 0.0f;
			a_vecPos.Z -= 0.05f;
		}

		if (a_ItemInstance.ItemID == EItemID.MARIJUANA_PLANT)
		{
			fRotX = 0.0f;
			CItemValueMarijuanaPlant plant = a_ItemInstance.GetValueData<CItemValueMarijuanaPlant>();
			switch (plant.growthState)
			{
				case EGrowthState.Seed:
					model = "bkr_prop_weed_01_small_01c";
					break;
				case EGrowthState.Sapling:
					model = "bkr_prop_weed_01_small_01b";
					break;
				case EGrowthState.Growing:
					model = "bkr_prop_weed_med_01a";
					break;
				case EGrowthState.FullyGrown:
					model = "bkr_prop_weed_lrg_01b";
					break;
			}
		}

		// TODO: We should have a flag for determining if an item should lay on its side or upright.
		if (a_ItemInstance.ItemID == EItemID.FERTILIZER)
		{
			fRotX = 0.0f;
			a_vecPos.Z += 0.4f;
		}

		if (a_ItemInstance.ItemID == EItemID.GENERIC_ITEM)
		{
			CItemValueGenericItem itemValue = a_ItemInstance.GetValueData<CItemValueGenericItem>();
			model = itemValue.model;
		}

		// TODO: This isn't owned by the resource and wont be destroyed on stop
		uint hash = NAPI.Util.GetHashKey(model);
		m_WorldObject = NAPI.Object.CreateObject(hash, a_vecPos, new Vector3(fRotX, fRotY, a_fRotZ), 255, a_Dimension);

		SetData(m_WorldObject, EDataNames.OBJECT_TYPE, EObjectTypes.OBJECT_TYPE_WORLD_ITEM, EDataType.Synced);
		SetData(m_WorldObject, EDataNames.ITEM_ID, (int)ItemInstance.ItemID, EDataType.Synced);

		if (a_ItemInstance.ItemID == EItemID.GENERIC_ITEM)
		{
			CItemValueGenericItem itemValue = a_ItemInstance.GetValueData<CItemValueGenericItem>();
			SetData(m_WorldObject, EDataNames.ITEM_CUSTOM_VALUE, JsonConvert.SerializeObject(itemValue), EDataType.Synced);
			SetData(m_WorldObject, EDataNames.GENERIC_ROTATION, new Vector3(fRotX, fRotY, a_fRotZ), EDataType.Synced);

			EntityDatabaseID droppedBy = itemValue.PlacedBy;
			bool bAdminLocked = itemValue.AdminLocked;

			SetData(m_WorldObject, EDataNames.ITEM_LOCKED, bAdminLocked, EDataType.Synced);
		}

		// set boombox flag (only set if its emitting, not if it dropped)
		if (a_ItemInstance.ItemID == EItemID.BOOMBOX)
		{
			// Is it actually emitting? or just dropped
			CItemValueBoombox itemValue = (CItemValueBoombox)a_ItemInstance.Value;

			EntityDatabaseID droppedBy = itemValue.placedBy;
			int radioID = itemValue.radioID;

			if (droppedBy != -1)
			{
				SetData(m_WorldObject, EDataNames.BOOMBOX, true, EDataType.Synced);
				SetData(m_WorldObject, EDataNames.BOOMBOX_RID, radioID, EDataType.Synced);
			}
		}

		if (a_ItemInstance.ItemID == EItemID.NOTE)
		{
			CItemValueNote itemValue = (CItemValueNote)a_ItemInstance.Value;

			EntityDatabaseID droppedBy = itemValue.placedBy;
			string strNoteMessage = itemValue.NoteMessage;
			string strCharacterName = itemValue.CharacterDroppedName;
			bool bAdminLocked = itemValue.AdminLocked;

			if (droppedBy != -1)
			{
				SetData(m_WorldObject, EDataNames.NOTE_LOCKED, bAdminLocked, EDataType.Synced);
				SetData(m_WorldObject, EDataNames.NOTE_MESSAGE, strNoteMessage, EDataType.Synced);
				SetData(m_WorldObject, EDataNames.NOTE_OWNER_NAME, strCharacterName, EDataType.Synced);
			}
		}

		if (a_ItemInstance.ItemID == EItemID.MARIJUANA_PLANT)
		{
			CItemValueMarijuanaPlant plant = a_ItemInstance.GetValueData<CItemValueMarijuanaPlant>();
			SetData(m_WorldObject, EDataNames.MARIJUANA_WATERED, plant.lastWatered.ToString(), EDataType.Synced);
			SetData(m_WorldObject, EDataNames.MARIJUANA_GROWTH_STAGE, plant.growthState, EDataType.Synced);
			SetData(m_WorldObject, EDataNames.MARIJUANA_TRIMMED, plant.trimmed, EDataType.Synced);
			SetData(m_WorldObject, EDataNames.MARIJUANA_FERTILIZED, plant.fertilized, EDataType.Synced);
		}
	}

	~CWorldItem()
	{
		Destroy();
	}

	public void Destroy()
	{
		if (m_WorldObject != null && m_WorldObject.Handle != null)
		{
			NAPI.Task.Run(() =>
			{
				NAPI.Entity.DeleteEntity(m_WorldObject.Handle);
				m_WorldObject = null;
			});
		}
	}

	public GTANetworkAPI.Object GTAInstance => m_WorldObject;

	private GTANetworkAPI.Object m_WorldObject = null;


	public CItemInstanceDef ItemInstance { get; }
	public Int64 DroppedByID { get; }
}

