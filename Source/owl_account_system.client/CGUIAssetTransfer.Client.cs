using System.Collections.Generic;
using System.Linq;

internal class CGUIAssetTransfer : CEFCore
{
	public CGUIAssetTransfer(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/asset_transfer.html", EGUIID.AssetTransfer, callbackOnLoad)
	{
		UIEvents.CloseAssetTransfer += CloseAssetTransfer;
		UIEvents.SubmitAssetTransfer += SubmitAssetTransfer;
	}

	public override void OnLoad()
	{
		if (FromCharacterId == 0)
		{
			return;
		}

		Execute("app.SetData", FromCharacterId, FromCharacterName, Money, BankMoney,
			OwlJSON.SerializeObject(Characters, EJsonTrackableIdentifier.CGUIAssetTransferSetDataCharacters), OwlJSON.SerializeObject(Vehicles, EJsonTrackableIdentifier.CGUIAssetTransferSetDataVehicles),
			OwlJSON.SerializeObject(Properties, EJsonTrackableIdentifier.CGUIAssetTransferSetDataProperties));
	}

	public void Show(long characterId, float money, float bankmoney, List<SVehicle> vehicles,
		List<SProperty> properties)
	{
		var characters = CharacterSelection.GetCharacters();
		string characterName = characters.First(character => character.id == characterId).name;

		FromCharacterId = characterId;
		FromCharacterName = characterName;
		Money = money;
		BankMoney = bankmoney;
		Characters = characters;
		Vehicles = vehicles;
		Properties = properties;

		Reload();
		SetVisible(true, true, true);
		Execute("app.SetData", FromCharacterId, FromCharacterName, Money, BankMoney,
			OwlJSON.SerializeObject(Characters, EJsonTrackableIdentifier.CGUIAssetTransferShowCharacters), OwlJSON.SerializeObject(Vehicles, EJsonTrackableIdentifier.CGUIAssetTransferShowVehicles),
			OwlJSON.SerializeObject(Properties, EJsonTrackableIdentifier.CGUIAssetTransferShowProperties));
	}

	public void CloseAssetTransfer()
	{
		FromCharacterId = 0;
		FromCharacterName = null;
		Money = 0.0f;
		BankMoney = 0.0f;
		Characters = null;
		Vehicles = null;
		Properties = null;

		SetVisible(false, false, false);
		CharacterSelection.ShowCharacterUI();
	}

	private static void SubmitAssetTransfer(long fromCharacter, long toCharacter, float money, float bankMoney,
		string vehicles, string properties)
	{
		List<long> vehicleIds = OwlJSON.DeserializeObject<List<long>>(vehicles, EJsonTrackableIdentifier.SubmitAssetTransferVehicles);
		List<long> propertyIds = OwlJSON.DeserializeObject<List<long>>(properties, EJsonTrackableIdentifier.SubmitAssetTransferProperties);

		NetworkEventSender.SendNetworkEvent_RequestTransferAssets(fromCharacter, toCharacter, money, bankMoney,
			vehicleIds, propertyIds);
	}

	private long FromCharacterId;
	private string FromCharacterName;
	private float Money;
	private float BankMoney;
	private List<GetCharactersCharacter> Characters;
	private List<SVehicle> Vehicles;
	private List<SProperty> Properties;
}