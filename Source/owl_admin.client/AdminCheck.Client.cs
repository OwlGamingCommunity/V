using System;

public class AdminCheck
{
	private CGUIAdminCheck m_AdminCheckUI = new CGUIAdminCheck(OnUILoaded);

	private AdminCheckDetails m_CachedDetails = null;
	private RAGE.Elements.Player m_CachedPlayer = null;

	public AdminCheck()
	{
		NetworkEvents.AdminCheck += OnAdminCheck;

		RageEvents.RAGE_OnTick_OncePerSecond += UpdateCheck;
	}

	private static void OnUILoaded()
	{

	}

	private void UpdateCheck()
	{
		if (m_AdminCheckUI.IsVisible())
		{
			ApplyCheckDetails();
		}
	}

	public void OnCloseCheck()
	{
		m_AdminCheckUI.SetVisible(false, false, false);
	}

	public void OnSaveAdminNotes(string strNotes)
	{
		NetworkEventSender.SendNetworkEvent_SaveAdminNotes(strNotes, DataHelper.GetEntityData<int>(m_CachedPlayer, EDataNames.ACCOUNT_ID));
	}

	private void ApplyCheckDetails()
	{
		int latency = DataHelper.GetEntityData<int>(m_CachedPlayer, EDataNames.PING);
		int playerID = DataHelper.GetEntityData<int>(m_CachedPlayer, EDataNames.PLAYER_ID);
		EAdminLevel adminLevel = DataHelper.GetEntityData<EAdminLevel>(m_CachedPlayer, EDataNames.ADMIN_LEVEL);
		string strTitle = Helpers.GetAdminLevelName(adminLevel);
		float fMoney = DataHelper.GetEntityData<float>(m_CachedPlayer, EDataNames.MONEY);
		float fBankMoney = DataHelper.GetEntityData<float>(m_CachedPlayer, EDataNames.BANK_MONEY);

		// What do we want to do with this? it's just dimension...
		string strInterior = m_CachedPlayer.Dimension == 0 ? "World" : m_CachedPlayer.Dimension.ToString();

		// SKIN
		bool bIsCustom = DataHelper.GetEntityData<bool>(m_CachedPlayer, EDataNames.IS_CUSTOM);
		string strSkin = Helpers.FormatString("{0} (Custom: {1})", m_CachedPlayer.Model, bIsCustom ? "Yes" : "No");

		// FACTIONS
		int factionIndex = 0;
		string strFactions = "";
		foreach (CFactionTransmit faction in m_CachedDetails.Factions)
		{
			if (factionIndex > 0)
			{
				strFactions += ", ";
			}

			strFactions += Helpers.FormatString("{0} ({1})", faction.Name, faction.ShortName);
			++factionIndex;
		}

		// WEAPONS
		string strWeapon = "Unarmed";
		int currentWeaponHash = 0;
		if (m_CachedPlayer.GetCurrentWeapon(ref currentWeaponHash, false))
		{
			EItemID itemID = EItemID.None;
			foreach (var kvPair in ItemWeaponDefinitions.g_DictItemIDToWeaponHash)
			{
				if (kvPair.Value == (WeaponHash)currentWeaponHash)
				{
					itemID = kvPair.Key;
				}
			}

			if (itemID != EItemID.None)
			{
				var itemDef = ItemDefinitions.g_ItemDefinitions[itemID];
				if (itemDef != null)
				{
					strWeapon = Helpers.FormatString("{0} (Ammo: {1})", itemDef.GetNameIgnoreGenericItems(), m_CachedPlayer.GetAmmoInWeapon((unchecked((uint)currentWeaponHash))));
				}
			}
		}

		// VEHICLE
		string strCurrentVehicle = "On-Foot";
		RAGE.Elements.Vehicle vehicle = m_CachedPlayer.Vehicle;
		if (vehicle != null)
		{
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicle.Model);
			if (vehicleDef != null)
			{
				strCurrentVehicle = Helpers.FormatString("{0} {1} ({2})", vehicleDef.Manufacturer, vehicleDef.Name, vehicleDef.Class);
			}
		}

		// LOCATION
		int streetNameID = -1;
		int crossroadNameID = -1;
		string strStreetName = String.Empty;
		string strCrossroadName = String.Empty;
		RAGE.Game.Pathfind.GetStreetNameAtCoord(m_CachedPlayer.Position.X, m_CachedPlayer.Position.Y, m_CachedPlayer.Position.Z, ref streetNameID, ref crossroadNameID);

		if (streetNameID != -1)
		{
			strStreetName = RAGE.Game.Ui.GetStreetNameFromHashKey((uint)streetNameID);
		}

		if (crossroadNameID != -1)
		{
			strCrossroadName = RAGE.Game.Ui.GetStreetNameFromHashKey((uint)crossroadNameID);
		}

		string strZoneName = RAGE.Game.Zone.GetNameOfZone(m_CachedPlayer.Position.X, m_CachedPlayer.Position.Y, m_CachedPlayer.Position.Z);
		string strRealZoneName = ZoneNameHelper.ZoneNames.ContainsKey(strZoneName) ? ZoneNameHelper.ZoneNames[strZoneName] : "San Andreas";
		string strLocation = String.Empty;

		if (strCrossroadName == String.Empty)
		{
			strLocation = Helpers.FormatString("{0}, {1}", strStreetName, strRealZoneName);
		}
		else
		{
			strLocation = Helpers.FormatString("{0} & {1}, {2}", strStreetName, strCrossroadName, strRealZoneName);
		}

		m_AdminCheckUI.SetDetails(m_CachedDetails.Username, m_CachedDetails.CharacterName, playerID, m_CachedDetails.IpAddress, strTitle, latency, m_CachedDetails.GameCoins, fMoney, fBankMoney, m_CachedPlayer.GetHealth(), m_CachedPlayer.GetArmour(), strFactions, strCurrentVehicle,
			m_CachedPlayer.Position.X, m_CachedPlayer.Position.Y, m_CachedPlayer.Position.Z, strInterior, m_CachedPlayer.Dimension, m_CachedDetails.HoursPlayed_Account, m_CachedDetails.HoursPlayed_Character, strSkin, strWeapon, m_CachedDetails.NumPunishmentPointsActive, m_CachedDetails.NumPunishmentPointsLifetime, strLocation);
	}

	private void OnAdminCheck(RAGE.Elements.Player targetPlayer, AdminCheckDetails playerDetails)
	{
		m_AdminCheckUI.SetVisible(true, true, false);

		m_CachedPlayer = targetPlayer;
		m_CachedDetails = playerDetails;

		m_AdminCheckUI.SetAdminNotes(playerDetails.AdminNotes);

		m_AdminCheckUI.ResetAdminHistory();
		m_AdminCheckUI.SetAdminHistory(playerDetails.AdminHistory);


		ApplyCheckDetails();
	}
}