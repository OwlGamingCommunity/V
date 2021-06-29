using System;

public static class DataHelper
{
	public static T GetLocalPlayerEntityData<T>(EDataNames a_DataName)
	{
		return GetEntityData<T>(RAGE.Elements.Player.LocalPlayer, a_DataName);
	}

	public static bool HasEntityData(RAGE.Elements.Entity a_Entity, EDataNames a_DataName)
	{
		return a_Entity.GetSharedData(((int)a_DataName).ToString()) != null;
	}

	public static T GetEntityData<T>(RAGE.Elements.Entity a_Entity, EDataNames a_DataName)
	{
		try
		{
			if (a_Entity != null)
			{
				object data = a_Entity.GetSharedData(((int)a_DataName).ToString());
				if (data != null)
				{
					if (typeof(T) == typeof(bool))
					{
						return (T)Convert.ChangeType(data, typeof(T));
					}
					else
					{
						// downcast trap for 1.1
						if (typeof(T) == typeof(Int32) && data.GetType() == typeof(Int64))
						{
#if DEBUG
							ChatHelper.DebugMessage(Helpers.FormatString("INT64 -> INT32 downcast DATA: {0}", a_DataName.ToString()));
#endif
							throw new Exception(Helpers.FormatString("INT64 -> INT32 downcast DATA: {0}", a_DataName.ToString()));
						}

						// cant changetype a lot of types, just do static cast
						return (T)data;
					}
				}
			}
		}
		catch
		{
			return default(T);
		}


		return default(T);
	}
}

public enum EJsonTrackableIdentifier
{
	ClientStorage,
	CustomAccessoriesProps,
	CustomAccessoriesTextures,
	HairAndTattoos,
	NetworkEvent,
	OnDemandMap,
	PerfCaptureLegend,
	PerfCaptureEntries,
	PrepareEvent,
	UIEvent,
	WorldMaps,
	ClientStorageFlush,
	AdminReloadReports,
	ApplyCheckVehDetails,
	ChatboxAddSettings,
	ChatSettingsForTab,
	CheckIntDetails,
	FurnitureData,
	GangTagData,
	InitializeAnimList,
	InventoryRefreshDutyOutfits,
	InventoryRefreshOutfits,
	ItemData,
	ItemMoverUIClick,
	ItemsListInit,
	LoadVehicleData,
	OnGotPhoneContacts,
	OnGotPhoneMessageContacts,
	OnGotPhoneMessagesFromNumber,
	OutfitEditorApplyPreview,
	OutfitEditorApplyPreview2,
	OutfitEditorApplyPreview3,
	OutfitEditorApplyPreview4,
	OutfitEditorChangeClothing,
	OutfitEditorChangeProp,
	OutfitEditorGotoList,
	ProcessWeaponMods,
	RenderGenericWorldItemName,
	ShowItemDutyOutfit,
	ShowItemOutfit,
	TagData,
	ToggleReportData,
	VehicleDefs,
	VehicleExtras,
	ApplyCheckVehDetails2,
	BlackjackStateReplication,
	ApplyOutfitPreview,
	DutyOutfitEditorGotoList,
	InitDuty,
	InitDuty2,
	CGUIAssetTransferSetDataCharacters,
	CGUIAssetTransferSetDataProperties,
	CGUIAssetTransferSetDataVehicles,
	CGUIAssetTransferShowCharacters,
	CGUIAssetTransferShowProperties,
	CGUIAssetTransferShowVehicles,
	SubmitAssetTransferProperties,
	SubmitAssetTransferVehicles,
	TattooData,
	HairTattooData
}