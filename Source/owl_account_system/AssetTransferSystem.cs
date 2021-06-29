using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AssetTransferSystem
{
	public AssetTransferSystem()
	{
		NetworkEvents.FetchTransferAssetsData += FetchTransferAssetsData;
		NetworkEvents.RequestTransferAssets += RequestTransferAssets;
	}

	private static void FetchTransferAssetsData(CPlayer player, long characterId)
	{
		Database.Functions.Characters.Get(player.AccountID, characterId, false, character =>
		{
			List<SVehicle> vehicles = VehiclePool
				.GetCharacterVehicles(characterId)
				.Where(vehicle => !vehicle.m_bTokenPurchase) // Don't allow token vehicles to be transferred.
				.Select(vehicle => new SVehicle(vehicle.m_DatabaseID, vehicle.GetFullDisplayName()))
				.ToList();

			List<SProperty> properties = PropertyPool
				.GetCharacterProperties(characterId)
				.Where(property => !property.Model.IsTokenPurchase) // Don't allow token properties to be transferred.
				.Select(property => new SProperty(property.Model.Id, property.Model.Name))
				.ToList();

			NetworkEventSender.SendNetworkEvent_LoadTransferAssetsCharacterData(player, characterId, character.money,
				character.bank_money, vehicles, properties);
		});
	}

	private static async void RequestTransferAssets(CPlayer player, long fromCharacterId, long toCharacterId, float money,
		float bankmoney, List<long> vehicleIds, List<long> propertyIds)
	{
		if (!await PurchaseAssetTransfer(player).ConfigureAwait(true))
		{
			player.SendNotification("Asset Transfer Failed", ENotificationIcon.ExclamationSign,
				"You do not have 750 GameCoins.");
			return;
		}

		Database.Functions.Characters.Get(player.AccountID, fromCharacterId, false, fromCharacter =>
		{
			Database.Functions.Characters.Get(player.AccountID, toCharacterId, false, toCharacter =>
			{
				if (!TransferMoney(fromCharacter, toCharacter, money))
				{
					player.SendNotification("Asset Transfer Failed", ENotificationIcon.ExclamationSign,
						"{0} does not have that much money.", fromCharacter.CharacterName);
					return;
				}

				if (!TransferBankMoney(fromCharacter, toCharacter, bankmoney))
				{
					player.SendNotification("Asset Transfer Failed", ENotificationIcon.ExclamationSign,
						"{0} does not have that much bank money.", fromCharacter.CharacterName);
					return;
				}

				TransferVehicles(fromCharacter, toCharacter, vehicleIds);
				TransferProperties(fromCharacter, toCharacter, propertyIds);

				player.SendNotification("Assets Transfered", ENotificationIcon.ExclamationSign,
					"Your assets have been transferred from {0} to {1}", fromCharacter.CharacterName,
					toCharacter.CharacterName);
				NetworkEventSender.SendNetworkEvent_AssetTransferCompleted(player);
			});
		});
	}

	private static async Task<bool> PurchaseAssetTransfer(CPlayer player)
	{
		int playerGCs = await player.GetDonatorCurrency().ConfigureAwait(true);
		if (playerGCs < DonationConstants.GCCostAssetTransfer)
		{
			return false;
		}
		player.SubtractDonatorCurrency(DonationConstants.GCCostAssetTransfer);
		return true;
	}

	private static bool TransferMoney(SGetCharacter fromCharacter, SGetCharacter toCharacter, float amount)
	{
		if (fromCharacter.money < amount)
		{
			return false;
		}

		Database.LegacyFunctions.SetPlayerMoney(fromCharacter.id, fromCharacter.money - amount);
		Database.LegacyFunctions.SetPlayerMoney(toCharacter.id, toCharacter.money + amount);
		return true;
	}

	private static bool TransferBankMoney(SGetCharacter fromCharacter, SGetCharacter toCharacter, float amount)
	{
		if (fromCharacter.bank_money < amount)
		{
			return false;
		}

		Database.LegacyFunctions.SetPlayerBankMoney(fromCharacter.id, fromCharacter.bank_money - amount);
		Database.LegacyFunctions.SetPlayerBankMoney(toCharacter.id, toCharacter.bank_money + amount);
		return true;
	}

	private static void TransferVehicles(SGetCharacter fromCharacter, SGetCharacter toCharacter, List<long> vehicleIds)
	{
		foreach (var vehicle in vehicleIds.Select(vehicleId => VehiclePool.GetVehicleFromID(vehicleId))
			.Where(vehicle => vehicle.OwnedOrRentedByCharacterID(fromCharacter.id)))
		{
			CItemInstanceDef item = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, vehicle.m_DatabaseID);
			HelperFunctions.Items.DeleteAllItems(item);
			item.ParentDatabaseID = toCharacter.id;
			item.ParentType = EItemParentTypes.Player;
			item.CurrentSocket = EItemSocket.Keyring;
			Database.Functions.Items.GiveEntityItem(item);
			vehicle.SetOwner(EVehicleType.PlayerOwned, toCharacter.id);
		}
	}

	private static void TransferProperties(SGetCharacter fromCharacter, SGetCharacter toCharacter, List<long> propertyIds)
	{
		foreach (var property in propertyIds.Select(propertyId => PropertyPool.GetPropertyInstanceFromID(propertyId))
			.Where(property => property.OwnedBy(EPropertyOwnerType.Player, fromCharacter.id)))
		{
			CItemInstanceDef item = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, property.Model.Id);
			HelperFunctions.Items.DeleteAllItems(item);
			item.ParentDatabaseID = toCharacter.id;
			item.ParentType = EItemParentTypes.Player;
			item.CurrentSocket = EItemSocket.Keyring;
			Database.Functions.Items.GiveEntityItem(item);

			property.SetOwner(EPropertyOwnerType.Player, toCharacter.id);
		}
	}
}