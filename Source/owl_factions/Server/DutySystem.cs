using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class DutySystem 
{
	public const uint NEARBY_DUTYPOINTS_DISTANCE = 20;

	public DutySystem()
	{
		NetworkEvents.UseDutyPoint += OnUseDutyPoint;
		NetworkEvents.CancelGoingOnDuty += CancelGoingOnDuty;

		NetworkEvents.EnterDutyOutfitEditor += OnEnterDutyOutfitEditor;
		NetworkEvents.RequestDutyOutfitList += OnRequestDutyOutfitList;
		NetworkEvents.DutySystem_RequestUpdatedOutfitList += OnRequestUpdatedOutfitList;

		NetworkEvents.DutyOutfitEditor_CreateOrUpdateOutfit += OnCreateOrUpdateOutfit;
		NetworkEvents.DutyOutfitEditor_DeleteOutfit += OnDeleteOutfit;

		NetworkEvents.FinalizeGoOnDuty += OnFinalizeGoOnDuty;

		// DUTY SPOTS
		CommandManager.RegisterCommand("createduty", "Creates a new duty point for official factions", new Action<CPlayer, CVehicle, EDutyType>(CreateDutyPoint), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("delduty", "Deletes a duty point", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteDutyPoint), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("nearbyduty", "Lists nearby duty points", new Action<CPlayer, CVehicle>(NearbyDutyPoint), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
	}

	private void OnDeleteOutfit(CPlayer player, EntityDatabaseID outfitID)
	{
		CItemInstanceDef outfitDef = player.Inventory.GetItemFromDBID(outfitID);
		if (outfitDef != null)
		{
			player.Inventory.RemoveItem(outfitDef);
		}
	}

	public void OnEnterDutyOutfitEditor(CPlayer player, EDutyType a_DutyType)
	{
		player.GotoPlayerSpecificDimension();
		player.CacheHealthAndArmor();

		// Force the duty skin
		player.ApplyDutySkin(a_DutyType);

		List<CItemInstanceDef> lstDutyOutfits = player.Inventory.GetDutyOutfitsOfType(a_DutyType);

		NetworkEventSender.SendNetworkEvent_EnterDutyOutfitEditor_Response(player, lstDutyOutfits);
	}
	// TODO_DUTY: Preview presets AND items in editor on mouse over in list?
	public void OnRequestDutyOutfitList(CPlayer player, EDutyType a_DutyType)
	{
		// Force the duty skin
		player.ApplyDutySkin(a_DutyType);

		List<CItemInstanceDef> lstDutyOutfits = player.Inventory.GetDutyOutfitsOfType(a_DutyType);

		NetworkEventSender.SendNetworkEvent_RequestDutyOutfitList_Response(player, lstDutyOutfits);
	}

	private void OnRequestUpdatedOutfitList(CPlayer player, EDutyType a_DutyType)
	{
		List<CItemInstanceDef> lstDutyOutfits = player.Inventory.GetDutyOutfitsOfType(a_DutyType);
		NetworkEventSender.SendNetworkEvent_DutySystem_GotUpdatedOutfitList(player, lstDutyOutfits);
	}

	private void OnCreateOrUpdateOutfit(CPlayer player, string strName, EDutyType a_DutyType, Dictionary<ECustomClothingComponent, int> DrawablesClothing, Dictionary<ECustomClothingComponent, int> TexturesClothing, Dictionary<ECustomPropSlot, int> CurrentPropDrawables,
		Dictionary<ECustomPropSlot, int> CurrentPropTextures, Dictionary<EDutyWeaponSlot, EItemID> Loadout, EntityDatabaseID outfitID, EDutyOutfitType a_CharacterType, uint a_PremadeHash, bool a_bHideHair)
	{
		CItemValueDutyOutfit outfitValue = new CItemValueDutyOutfit(strName, a_DutyType, DrawablesClothing, TexturesClothing, CurrentPropDrawables, CurrentPropTextures, Loadout, a_CharacterType, a_PremadeHash, a_bHideHair);

		if (outfitID == -1)
		{
			// give outfit item
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromObjectNoDBIDNoSocketPlayerParent(EItemID.DUTY_OUTFIT, outfitValue);
			player.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
		}
		else
		{
			CItemInstanceDef outfitDef = player.Inventory.GetItemFromDBID(outfitID);
			if (outfitDef != null)
			{
				// update the value and save
				outfitDef.Value = outfitValue;

				// Is it currently active? if so use it so we 'update our clothing'
				if (outfitValue.IsActive && player.IsOnDutyOfType(outfitValue.DutyType))
				{
					player.ActivateDutyOutfit(outfitValue.DutyType, outfitDef);
				}

				Database.Functions.Items.SaveItemValue(outfitDef);
			}
		}
	}

	// START ADMIN DUTY POINTS
	private void NearbyDutyPoint(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby duty Points:");
		foreach (var kvPair in DutyPoints.GetDutyPoints())
		{
			if (SourcePlayer.Client.Dimension == kvPair.Value.Dimension)
			{
				float fDist = SourcePlayer.Client.Position.DistanceTo2D(kvPair.Value.Position);
				if (fDist <= NEARBY_DUTYPOINTS_DISTANCE)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - '{1}' - {2} distance", kvPair.Value.DatabaseID, kvPair.Value.DutyType, fDist);
				}
			}
		}
	}

	private void DeleteDutyPoint(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID dutyPointID)
	{
		CDutyPoint dutyPoint = DutyPoints.GetDutyPointByID(dutyPointID);
		if (dutyPoint != null)
		{
			DutyPoints.DestroyDutyPoint(dutyPoint, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a duty point (#{0}).", dutyPointID);
			new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/delduty duty point ID: {0} ", dutyPointID)).execute();
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Duty point not found.");
		}
	}

	private async void CreateDutyPoint(CPlayer SourcePlayer, CVehicle SourceVehicle, EDutyType DutyType)
	{
		if (!Enum.IsDefined(typeof(EDutyType), DutyType) || DutyType == 0)
		{
			SourcePlayer.SendNotification("Duty Points", ENotificationIcon.ExclamationSign, "Not a valid duty type.");
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 255, 0, "Law Enforcement = 1, EMS = 2, Fire = 3, News = 4, Towing = 5.");
			return;
		}

		CDutyPoint dutyPoint = await DutyPoints.CreateDutyPoint(-1, DutyType, SourcePlayer.GetEstimatedGroundPosition(), SourcePlayer.Client.Dimension, true).ConfigureAwait(true);
		SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have created a duty point of type: '{0}' (#{1}).", DutyType, dutyPoint.DatabaseID);
		new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/createduty - Created duty point ID: {0} Type: {1} ", dutyPoint.DatabaseID, dutyPoint.DutyType)).execute();
	}
	// END ADMIN DUTY POINTS

	public void CancelGoingOnDuty(CPlayer player)
	{
		// Re-apply normal skin
		player.ApplySkinFromInventory();

		// teleport player back to whichever dimension they were in
		player.GotoNonPlayerSpecificDimension();
		player.Freeze(false);

		player.RestoreHealthAndArmor();
	}

	// TODO_DUTY: When they leave a faction, remove their outfits? they cant use them, but still messy
	public void OnUseDutyPoint(CPlayer player, EDutyType dutyType)
	{
		bool bIsOnDuty = player.IsOnDuty();
		if (bIsOnDuty)
		{
			player.GoOffDuty();
		}
		else
		{
			bool bSuccessful = player.IsEligbleToUseDutyOfType(dutyType);

			if (!bSuccessful)
			{
				player.SendNotification("Duty", ENotificationIcon.ExclamationSign, "You are not eligible to go on duty here.");
			}
			else
			{
				player.Freeze(true);

				// Goto our special place
				player.GotoPlayerSpecificDimension();

				// Force the duty skin
				player.ApplyDutySkin(dutyType);
			}

			player.CacheHealthAndArmor();
			NetworkEventSender.SendNetworkEvent_UseDutyPointResult(player, bSuccessful, dutyType, player.Inventory.GetDutyOutfitsOfType(dutyType));
		}
	}

	public void OnFinalizeGoOnDuty(CPlayer player, EDutyType dutyType, EntityDatabaseID outfitID)
	{
		// TODO_POST_LAUNCH: Verify player can actually use this duty type
		if (outfitID != -1) // safety
		{
			CItemInstanceDef outfitDef = player.Inventory.GetItemFromDBID(outfitID);
			if (outfitDef != null)
			{
				player.GotoNonPlayerSpecificDimension();
				player.GoOnDuty(dutyType, outfitDef, false);
				player.Freeze(false);
				player.RestoreHealthAndArmor();
			}
		}
	}
}