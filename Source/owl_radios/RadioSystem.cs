using GTANetworkAPI;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class RadioSystem
{
	const int g_MaxRadiosPerAccount = 5;

	const int g_InitialRadioCost = 700;
	const int g_InitialRadioLengthInDays = 30;

	const int g_RadioCostExtension_7Days = 700;
	const int g_RadioCostExtension_30Days = 200;

	public RadioSystem()
	{
		NetworkEvents.GetBasicRadioInfo += OnGetBasicRadioInfo;
		NetworkEvents.PurchaseRadioRequest += OnPurchaseRadioRequest;

		NetworkEvents.SaveRadio += OnSaveRadio;

		NetworkEvents.CharacterSpawned += OnSpawn;

		LoadAllRadios();

		NetworkEvents.ChangeVehicleRadio += OnChangeVehicleRadio;
		NetworkEvents.RequestBeginChangeBoomboxRadio += OnRequestBeginChangeBoomboxRadio;
		NetworkEvents.ChangeBoomboxRadio += OnChangeBoomboxRadio;

		NetworkEvents.ExtendRadio7Days += (CPlayer a_Player, int a_RadioID) => { OnExtendRadio(a_Player, a_RadioID, 7, g_RadioCostExtension_7Days); };
		NetworkEvents.ExtendRadio30Days += (CPlayer a_Player, int a_RadioID) => { OnExtendRadio(a_Player, a_RadioID, 30, g_RadioCostExtension_30Days); };

		MainThreadTimerPool.CreateGlobalTimer(OnTickExpireRadios, 3600000);

		CommandManager.RegisterCommand("customradios", "Manage custom radio stations", new Action<CPlayer, CVehicle>(CustomRadiosCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "radiostations", "cradios" });
	}

	private void OnSpawn(CPlayer player)
	{
		foreach (RadioInstance radioInst in RadioPool.GetRadios())
		{
			if (radioInst.Account == player.AccountID)
			{
				if (radioInst.ExpirationTime > 0)
				{
					int numDaysExpirationNotice = 7;
					Int64 unixTimestamp = Helpers.GetUnixTimestamp();
					Int64 daysUntilExpiration = (Int64)((radioInst.ExpirationTime - unixTimestamp) / (24 * 60 * 60));

					if (daysUntilExpiration <= numDaysExpirationNotice)
					{
						player.SendNotification("Radios", ENotificationIcon.Headphones, "Your radio station '{0} is due to expire in {1} day(s).", radioInst.Name, daysUntilExpiration);
					}
				}

				if (!radioInst.ResolvedSuccessfully())
				{
					player.SendNotification("Radios", ENotificationIcon.Headphones, "Your radio station '{0} has a bad URL. Please validate and correct it.", radioInst.Name);
				}
			}
		}
	}

	private void CustomRadiosCommand(CPlayer player, CVehicle pVehicle)
	{
		NetworkEventSender.SendNetworkEvent_GotoRadioStationManagement(player);
	}

	private async void OnTickExpireRadios(object[] parameters)
	{
		List<RadioInstance> lstRadiosToRemove = new List<RadioInstance>();

		foreach (RadioInstance radioInst in RadioPool.GetRadios())
		{
			if (radioInst.IsExpired())
			{
				await Database.LegacyFunctions.RemoveRadio(radioInst.ID).ConfigureAwait(true);
				lstRadiosToRemove.Add(radioInst);
			}
		}

		RadioPool.RemoveRange(lstRadiosToRemove);
	}

	public async void LoadAllRadios()
	{
		List<RadioInstance> lstRadios = await Database.LegacyFunctions.LoadAllRadios().ConfigureAwait(true);

		RadioPool.Init(lstRadios);
		NetworkEventSender.SendNetworkEvent_SyncAllRadios_ForAll_IncludeEveryone(lstRadios);

		NAPI.Util.ConsoleOutput("[RADIOS] Loaded {0} Radios!", lstRadios.Count);
	}

	private void OnChangeVehicleRadio(CPlayer a_Player, int a_RadioID)
	{
		if (a_Player.IsInVehicleReal)
		{
			CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(a_Player.Client.Vehicle);

			if (vehicle != null)
			{
				vehicle.CurrentRadioID = a_RadioID;
			}
		}
	}

	private void OnRequestBeginChangeBoomboxRadio(CPlayer player, GTANetworkAPI.Object pickupItem)
	{

		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstanceHandle(pickupItem.Handle);
		if (pWorldItem != null)
		{
			if (pWorldItem.ItemInstance.ItemID == EItemID.BOOMBOX)
			{
				CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(player.Client.Dimension);
				CItemValueBoombox itemValue = (CItemValueBoombox)pWorldItem.ItemInstance.Value;
				EntityDatabaseID droppedBy = itemValue.placedBy;

				if (Property != null)
				{
					if (player.IsAdmin(EAdminLevel.TrialAdmin, true) || Property != null || Property.OwnedBy(player) || Property.RentedBy(player) || (Property.Model.OwnerType == EPropertyOwnerType.Faction && player.IsFactionManager(Property.Model.OwnerId)) || droppedBy == player.ActiveCharacterDatabaseID)
					{
						NetworkEventSender.SendNetworkEvent_RequestBeginChangeBoomboxRadio_Response(player, true);
					}
					else
					{
						player.SendNotification("Boombox", ENotificationIcon.ExclamationSign, "This boombox was placed by another person - you cannot change the station.");
						NetworkEventSender.SendNetworkEvent_RequestBeginChangeBoomboxRadio_Response(player, false);
					}
				}
				else
				{
					if (player.IsAdmin(EAdminLevel.TrialAdmin, true) || droppedBy == player.ActiveCharacterDatabaseID)
					{
						NetworkEventSender.SendNetworkEvent_RequestBeginChangeBoomboxRadio_Response(player, true);
					}
					else
					{
						player.SendNotification("Boombox", ENotificationIcon.ExclamationSign, "This boombox was placed by another person - you cannot change the station.");
						NetworkEventSender.SendNetworkEvent_RequestBeginChangeBoomboxRadio_Response(player, false);
					}
				}
			}
		}
	}

	private void OnChangeBoomboxRadio(CPlayer player, GTANetworkAPI.Object pickupItem, string strStationName, int a_RadioID)
	{
		CWorldItem pWorldItem = WorldItemPool.GetWorldItemFromGTAInstanceHandle(pickupItem.Handle);
		if (pWorldItem != null)
		{
			if (pWorldItem.ItemInstance.ItemID == EItemID.BOOMBOX)
			{
				CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(player.Client.Dimension);
				CItemValueBoombox itemValue = (CItemValueBoombox)pWorldItem.ItemInstance.Value;
				EntityDatabaseID droppedBy = itemValue.placedBy;
				if (Property != null)
				{
					if (player.IsAdmin(EAdminLevel.TrialAdmin, true) || Property.OwnedBy(player) || Property.RentedBy(player) || (Property.Model.OwnerType == EPropertyOwnerType.Faction && player.IsFactionManager(Property.Model.OwnerId)) || droppedBy == player.ActiveCharacterDatabaseID)
					{
						itemValue.radioID = a_RadioID;
						Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);

						pWorldItem.SetData(pickupItem, EDataNames.BOOMBOX_RID, a_RadioID, EDataType.Synced);

						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("changes the boombox to '{0}'.", strStationName));
					}
				}
				else
				{
					if (player.IsAdmin(EAdminLevel.TrialAdmin, true) || droppedBy == player.ActiveCharacterDatabaseID)
					{
						itemValue.radioID = a_RadioID;
						Database.Functions.Items.SaveItemValue(pWorldItem.ItemInstance);

						pWorldItem.SetData(pickupItem, EDataNames.BOOMBOX_RID, a_RadioID, EDataType.Synced);

						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("changes the boombox to '{0}'.", strStationName));
					}
					else
					{
						return;
					}
				}
			}
		}
	}

	private async void OnSaveRadio(CPlayer a_Player, int a_RadioID, string strName, string strEndpoint)
	{
		RadioInstance targetRadio = RadioPool.GetRadioFromIDForPlayer(a_Player, a_RadioID);
		if (targetRadio != null)
		{
			Uri uriResult;
			bool result = Uri.TryCreate(strEndpoint, UriKind.Absolute, out uriResult)
				&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

			if (!result)
			{
				a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "Your radio '{0}' was not saved: '{1}' is not a valid URL.", targetRadio.Name, strEndpoint);
			}
			else if (!strEndpoint.EndsWith(".pls"))
			{
				a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "Your radio '{0}' was not saved: Your URL must end with .pls", targetRadio.Name);
			}
			else
			{
				await Database.LegacyFunctions.UpdateRadio(a_Player.AccountID, a_RadioID, strName, strEndpoint).ConfigureAwait(true);

				targetRadio.Name = strName;
				targetRadio.Endpoint = strEndpoint;
				await targetRadio.Resolve().ConfigureAwait(true);

				a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "Your radio '{0}' has been saved.", targetRadio.Name);

				NetworkEventSender.SendNetworkEvent_SyncSingleRadio_ForAll_IncludeEveryone(targetRadio);
			}
		}
		else
		{
			a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "ERROR: Radio not found");
		}
	}

	private async void OnExtendRadio(CPlayer a_Player, int a_RadioID, int numDays, int costGC)
	{
		int donatorCurrency = await a_Player.GetDonatorCurrency().ConfigureAwait(true);

		if (donatorCurrency >= costGC)
		{
			RadioInstance targetRadio = RadioPool.GetRadioFromIDForPlayer(a_Player, a_RadioID);
			if (targetRadio != null)
			{
				a_Player.SubtractDonatorCurrency(costGC);

				Int64 newExpiration = (targetRadio.ExpirationTime + (numDays * 24 * 60 * 60));
				await Database.LegacyFunctions.UpdateRadioExpiration(a_Player.AccountID, a_RadioID, newExpiration).ConfigureAwait(true);
				targetRadio.ExpirationTime = newExpiration;
				NetworkEventSender.SendNetworkEvent_SyncSingleRadio(a_Player, targetRadio);
				OnGetBasicRadioInfo(a_Player);

				a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "Your radio '{0}' has been extended by {1} days for {2} GC", targetRadio.Name, numDays, costGC);
			}
			else
			{
				a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "ERROR: Radio not found");
			}
		}
		else
		{
			a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "You cannot afford a radio extension of {0} days. Your GC balance is {1} GC, a {0} day radio extension costs {2} GC.", numDays, donatorCurrency, costGC);
		}
	}

	private async void OnGetBasicRadioInfo(CPlayer a_Player)
	{
		int donatorCurrency = await a_Player.GetDonatorCurrency().ConfigureAwait(true);

		NetworkEventSender.SendNetworkEvent_GotBasicRadioInfo(a_Player, donatorCurrency);
	}

	private async void OnPurchaseRadioRequest(CPlayer a_Player)
	{
		List<RadioInstance> lstPlayerRadios = RadioPool.GetRadiosFromPlayer(a_Player);

		if (lstPlayerRadios.Count < g_MaxRadiosPerAccount)
		{
			int donatorCurrency = await a_Player.GetDonatorCurrency().ConfigureAwait(true);

			if (donatorCurrency >= g_InitialRadioCost)
			{
				a_Player.SubtractDonatorCurrency(g_InitialRadioCost);

				string strName = "New Radio";

				Int64 expiration = (Helpers.GetUnixTimestamp() + (g_InitialRadioLengthInDays * 24 * 60 * 60));
				int dbid = await Database.LegacyFunctions.CreateRadio(a_Player.AccountID, strName, String.Empty, expiration).ConfigureAwait(true);

				RadioInstance newRadio = new RadioInstance(dbid, a_Player.AccountID, "New Radio", String.Empty, expiration);
				RadioPool.Add(newRadio);

				OnGetBasicRadioInfo(a_Player);

				a_Player.AwardAchievement(EAchievementID.CreateCustomRadioStation);
				a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "Your new radio has been purchased.");
			}
			else
			{
				a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "You cannot afford a radio. Your GC balance is {0} GC, a radio costs {1} GC.", donatorCurrency, g_InitialRadioCost);
			}
		}
		else
		{
			a_Player.SendNotification("Radios", ENotificationIcon.VolumeUp, "Your account already has the maximum number of radios ({0})", g_MaxRadiosPerAccount);
		}
	}
}