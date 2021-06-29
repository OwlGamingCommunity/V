using GTANetworkAPI;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;

public class MarijuanaSystem 
{
	private const float MARIJUANA_STACK_PRICE = 35f;
	private const int MARIJUANA_PER_DRIED_ITEM = 28;
	private const int MARIJUANA_BONUS_PER_CARE = 5;
	private const int TIMESTAMP_DAY = 86400;
	private const int DRYING_TIME_REQUIRED = 172800;

	public MarijuanaSystem()
	{
		CommandManager.RegisterCommand("resetweedseeds", "Gets a marijuana plant.", new Action<CPlayer, CVehicle, CPlayer>(ResetWeedSeedsCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("throwaway", "Throw away your current smoke", new Action<CPlayer, CVehicle>(ThrowAwaySmokeCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("giveweed", "Gives a player a marijuana item.", new Action<CPlayer, CVehicle, CPlayer, EGrowthState, uint, bool, bool>(GiveWeedCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.Marijuana_OnGetSeeds += OnGetSeeds;
		NetworkEvents.Marijuana_OnSellDrugs += OnSellDrugs;

		NetworkEvents.Marijuana_OnWater += OnWaterPlant;
		NetworkEvents.Marijuana_OnSheer += OnSheerPlant;
		NetworkEvents.Marijuana_OnFertilize += OnFertilizePlant;
	}

	public void GiveWeedCommand(CPlayer source, CVehicle _, CPlayer other, EGrowthState growthState, uint wateredCount, bool fertilized, bool trimmed)
	{
		if (!source.IsAdmin(EAdminLevel.LeadAdmin))
		{
			return;
		}
		
		long lastWatered = 0;
		if (wateredCount > 0)
		{
			lastWatered = Helpers.GetUnixTimestamp(true);
		}

		CItemValueMarijuanaPlant plant = new CItemValueMarijuanaPlant(growthState, lastWatered, wateredCount, fertilized, trimmed, 0);
		CItemInstanceDef item = CItemInstanceDef.FromTypedObjectNoDBID(EItemID.MARIJUANA_PLANT, plant, 1);

		if (!other.Inventory.CanGiveItem(item, out var _, out var err))
		{
			source.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "Could not give the player the item: {0}", err);
			return;
		}

		other.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);

		source.PushChatMessage(EChatChannel.Notifications, "You gave {0} a marijuana plant.", other.GetCharacterName(ENameType.CharacterDisplayName));
		other.SendNotification("Received Marijuana", ENotificationIcon.Star, "{0} {1} gave you a marijuana plant.", source.AdminTitle, source.Username);
		Log.CreateLog(source, ELogType.AdminCommand, new List<CBaseEntity> { other }, Helpers.FormatString("/giveweed Gave {0} a marijuana plant. Growth State: {1}, Watered Count: {2}, Fertilized: {3}, Trimmed: {4}", other.GetCharacterName(ENameType.StaticCharacterName), growthState, wateredCount, fertilized, trimmed));

	}

	public void ThrowAwaySmokeCommand(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		string strSmokingType;

		if (SenderPlayer.CurrentSmokingType == ESmokingItemType.Cigarette)
		{
			strSmokingType = "Cigarette";
		}
		else if (SenderPlayer.CurrentSmokingType == ESmokingItemType.CigarBasic || SenderPlayer.CurrentSmokingType == ESmokingItemType.CigarHighClass)
		{
			strSmokingType = "Cigar";
		}
		else if (SenderPlayer.CurrentSmokingType == ESmokingItemType.Joint)
		{
			strSmokingType = "Joint";
		}
		else
		{
			strSmokingType = "None";
		}

		if (SenderPlayer.IsSmoking)
		{
			SenderPlayer.StopCurrentAnimation(true, true);
			HelperFunctions.Chat.SendAmeMessage(SenderPlayer, Helpers.FormatString("throws their {0} on the ground", strSmokingType));
			SenderPlayer.SetSmokingOfType(false, ESmokingItemType.None);
		}
		else
		{
			SenderPlayer.SendNotification("Smoking", ENotificationIcon.ExclamationSign, "You need to smoke before you can throw it away.");
		}
	}

	public async void ResetWeedSeedsCommand(CPlayer player, CVehicle vehicle, CPlayer other)
	{
		Log.CreateLog(player, ELogType.AdminCommand, new List<CBaseEntity> { other }, Helpers.FormatString("/resetweedseeds Reset the weed seed dealer for character {0}", other.GetCharacterName(ENameType.StaticCharacterName, player)));
		await Database.LegacyFunctions.SetGottenMarijuanaSeeds(other.ActiveCharacterDatabaseID, false).ConfigureAwait(true);
		player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "You have reset the seed issuer for {0}.", other.GetCharacterName(ENameType.StaticCharacterName, player));
		other.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "{0} {1} has reset the marijuana seed dealer for your character.", player.AdminTitle, player.GetCharacterName(ENameType.StaticCharacterName, other));
	}

	private async void OnGetSeeds(CPlayer player)
	{
		bool gottenSeeds = await Database.LegacyFunctions.HasGottenMarijuanaSeeds(player.ActiveCharacterDatabaseID).ConfigureAwait(true);
		if (gottenSeeds)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "Eddie only gives out seeds once!");
			NetworkEventSender.SendNetworkEvent_Marijuana_CloseMenu(player);
			return;
		}

		await Database.LegacyFunctions.SetGottenMarijuanaSeeds(player.ActiveCharacterDatabaseID).ConfigureAwait(true);
		CItemInstanceDef item = CItemInstanceDef.FromBasicValueNoDBID(EItemID.MARIJUANA_SEED, 0, 5);
		player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);
	}

	private void OnSellDrugs(CPlayer player, uint count)
	{
		CItemInstanceDef item = player.Inventory
			.GetAllItems()
			.FirstOrDefault(i => i.ItemID == EItemID.WEED && i.StackSize >= count);

		if (item == null)
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, "You don't have enough Marijuana!");
			return;
		}

		item.StackSize -= count;
		player.Inventory.RemoveItem(item);

		if (item.StackSize > 0)
		{
			player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);
		}

		float amount = MARIJUANA_STACK_PRICE * count;

		player.AddMoney(amount, PlayerMoneyModificationReason.SellDrugs);
		player.SendNotification("Marijuana", ENotificationIcon.PiggyBank, "You sold {0} grams of marijuana for ${1:0.00}", count, amount);
		NetworkEventSender.SendNetworkEvent_Marijuana_CloseMenu(player);
	}

	public static void WaterNearbyPlant(CPlayer player)
	{
		NetworkEventSender.SendNetworkEvent_Marijuana_WaterNearbyPlant(player);
	}

	public static void SheerNearbyPlant(CPlayer player)
	{
		NetworkEventSender.SendNetworkEvent_Marijuana_SheerNearbyPlant(player);
	}

	public static void FertilizeNearbyPlant(CPlayer player)
	{
		NetworkEventSender.SendNetworkEvent_Marijuana_FertilizeNearbyPlant(player);
	}

	public static void OnWaterPlant(CPlayer player, GTANetworkAPI.Object plantObj)
	{
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();
		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstance(plantObj);
		CItemValueMarijuanaPlant value = pWorldItem.ItemInstance.GetValueData<CItemValueMarijuanaPlant>();

		if (value.lastWatered > unixTimestamp - TIMESTAMP_DAY)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "This plant cannot be watered yet.");
			return;
		}

		if (value.growthState == EGrowthState.FullyGrown)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "This plant is fully grown! Use your shears to cut it to begin the drying process.");
			return;
		}

		value.water(unixTimestamp);

		Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);
		pWorldItem.SetData(plantObj, EDataNames.MARIJUANA_GROWTH_STAGE, value.growthState, EDataType.Synced);
		pWorldItem.SetData(plantObj, EDataNames.MARIJUANA_WATERED, value.lastWatered.ToString(), EDataType.Synced);
		HelperFunctions.Chat.SendAmeMessage(player, "uses a watering can to water the marijuana plant.");

		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "anim@amb@business@weed@weed_inspecting_lo_med_hi@", "weed_stand_checkingleaves_idle_02_inspector", false, true, true, 3000, false);
	}

	public static void OnSheerPlant(CPlayer player, GTANetworkAPI.Object plantObj)
	{
		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstance(plantObj);
		CItemValueMarijuanaPlant value = pWorldItem.ItemInstance.GetValueData<CItemValueMarijuanaPlant>();

		if (value.growthState == EGrowthState.Seed)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "This plant cannot be trimmed yet.");
			return;
		}

		if (value.growthState == EGrowthState.FullyGrown)
		{
			ConvertPlantToDrying(player, pWorldItem, value);
			return;
		}

		if (value.trimmed)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "This plant has already been trimmed.");
			return;
		}

		value.trimmed = true;
		Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);
		pWorldItem.SetData(plantObj, EDataNames.MARIJUANA_TRIMMED, value.trimmed, EDataType.Synced);
		HelperFunctions.Chat.SendAmeMessage(player, "trims the marijuana plant using a pair of shears.");
	}

	protected static void ConvertPlantToDrying(CPlayer player, CWorldItem pWorldItem, CItemValueMarijuanaPlant plant)
	{
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();
		plant.startedDrying = unixTimestamp;
		CItemInstanceDef item = CItemInstanceDef.FromTypedObjectNoDBID(EItemID.MARIJUANA_DRYING, plant, 1);
		CItemInstanceDef potItem = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PLANTING_POT, 0);

		if (!player.Inventory.CanGiveItem(item, out _, out var giveItemError) || !player.Inventory.CanGiveItem(potItem, out _, out giveItemError))
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 100, 100, giveItemError);
			return;
		}

		WorldItemPool.DestroyWorldItem(pWorldItem);
		Database.Functions.Items.DestroyWorldItem(pWorldItem.m_DatabaseID);
		
		player.Inventory.AddItemToNextFreeSuitableSlot(item, EShowInventoryAction.DoNothing, EItemID.None, null);
		player.Inventory.AddItemOrAddToExistingStack(potItem, EShowInventoryAction.DoNothing, EItemID.None, null);
		
		HelperFunctions.Chat.SendAmeMessage(player, "cuts the stem of the marijuana plant.");
		player.SendNotification("Marijuana", ENotificationIcon.Star, "You have received a drying marijuana plant.");
	}

	public static void OnFertilizePlant(CPlayer player, GTANetworkAPI.Object plantObj)
	{
		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstance(plantObj);
		CItemValueMarijuanaPlant value = pWorldItem.ItemInstance.GetValueData<CItemValueMarijuanaPlant>();

		if (value.fertilized)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "This plant has already been fertilized.");
			return;
		}

		if (value.growthState == EGrowthState.FullyGrown)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "This plant can no longer be fertilized.");
			return;
		}

		value.fertilized = true;
		Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);
		pWorldItem.SetData(plantObj, EDataNames.MARIJUANA_FERTILIZED, value.fertilized, EDataType.Synced);
		HelperFunctions.Chat.SendAmeMessage(player, "fertilizes the marijuana plant.");
	}

	public static void SplitMarijuanaDrying(CPlayer player, CItemInstanceDef item)
	{
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();
		CItemValueMarijuanaPlant plant = item.GetValueData<CItemValueMarijuanaPlant>();

		if (plant.startedDrying > unixTimestamp - DRYING_TIME_REQUIRED)
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "You must let the plant dry 48 hours.");
			return;
		}

		if (!player.Inventory.HasItem(CItemInstanceDef.FromItemID(EItemID.SHEERS), false, out CItemInstanceDef outItem))
		{
			player.SendNotification("Marijuana", ENotificationIcon.ExclamationSign, "You need shears to split the dried marijuana plant.");
			return;
		}

		uint stack = getMarijuanaCountFromPlant(plant);
		CItemInstanceDef marijuana = CItemInstanceDef.FromBasicValueNoDBID(EItemID.WEED, 0, stack);
		player.Inventory.AddItemOrAddToExistingStack(marijuana, EShowInventoryAction.DoNothing, EItemID.None, null);

		CItemInstanceDef seeds = CItemInstanceDef.FromBasicValueNoDBID(EItemID.MARIJUANA_SEED, 0, 2);
		player.Inventory.AddItemOrAddToExistingStack(seeds, EShowInventoryAction.Show, EItemID.None, null);

		player.Inventory.RemoveItem(item);

		HelperFunctions.Chat.SendAmeMessage(player, "cuts up a dried marijuana plant.");
	}

	private static uint getMarijuanaCountFromPlant(CItemValueMarijuanaPlant plant)
	{
		uint baseQuantity = MARIJUANA_PER_DRIED_ITEM;

		if (plant.trimmed)
		{
			baseQuantity += MARIJUANA_BONUS_PER_CARE;
		}

		if (plant.fertilized)
		{
			baseQuantity += MARIJUANA_BONUS_PER_CARE;
		}

		return baseQuantity;
	}
}
