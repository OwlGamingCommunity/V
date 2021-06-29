//#define PAUSE_ON_HIGH_FRAMETIME
using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

namespace Database
{
	public static class LegacyFunctions
	{
		private static MySQLInstance m_MySQLInst = new MySQLInstance();

		public static void Tick()
		{
			m_MySQLInst.KeepAlive();
		}

		public static async Task<int> GetCharacterPropertiesCount(EntityDatabaseID a_CharacterId)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT COUNT(0) AS c FROM properties WHERE owner={0} AND owner_type={1}", a_CharacterId, EPropertyOwnerType.Player).ConfigureAwait(true);
			return mysqlResult.GetRow(0).GetValue<int>("c");
		}

		public static async Task<int> GetCharacterVehiclesCount(EntityDatabaseID a_CharacterId)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT COUNT(0) AS c FROM vehicles WHERE owner={0} AND type={1}", a_CharacterId, EVehicleType.PlayerOwned).ConfigureAwait(true);
			return mysqlResult.GetRow(0).GetValue<int>("c");
		}


		public static async Task SetCharacterFullBeard(EntityDatabaseID DatabaseID, int style, int color)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET full_beard_style={1}, full_beard_color={2} WHERE char_id={0} LIMIT 1;", DatabaseID, style, color).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterBaseHair(EntityDatabaseID DatabaseID, int BaseHair)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET base_hair={1} WHERE char_id={0} LIMIT 1;", DatabaseID, BaseHair).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterHairStyle(EntityDatabaseID DatabaseID, int HairStyle)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET hair_style={1} WHERE char_id={0} LIMIT 1;", DatabaseID, HairStyle).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterHairColor(EntityDatabaseID DatabaseID, int HairColor)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET hair_color={1} WHERE char_id={0} LIMIT 1;", DatabaseID, HairColor).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterHairColorHighlight(EntityDatabaseID DatabaseID, int HairColorHighlight)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET hair_color_highlight={1} WHERE char_id={0} LIMIT 1;", DatabaseID, HairColorHighlight).ConfigureAwait(true);
		}

		// chest hair
		public static async Task SetCustomCharacterChestHairStyle(EntityDatabaseID DatabaseID, int ChestHairStyle)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET chest_hair={1} WHERE char_id={0} LIMIT 1;", DatabaseID, ChestHairStyle).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterChestHairColor(EntityDatabaseID DatabaseID, int ChestHairColor)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET chest_hair_color={1} WHERE char_id={0} LIMIT 1;", DatabaseID, ChestHairColor).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterChestHairColorHighlight(EntityDatabaseID DatabaseID, int ChestHairColorHighlight)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET chest_hair_color_highlights={1} WHERE char_id={0} LIMIT 1;", DatabaseID, ChestHairColorHighlight).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterChestHairOpacity(EntityDatabaseID DatabaseID, float fOpacity)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET chest_hair_opacity={1} WHERE char_id={0} LIMIT 1;", DatabaseID, fOpacity).ConfigureAwait(true);
		}

		// facial hair
		public static async Task SetCustomCharacterFacialHairStyle(EntityDatabaseID DatabaseID, int FacialHairStyle)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET facial_hair_style={1} WHERE char_id={0} LIMIT 1;", DatabaseID, FacialHairStyle).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterFacialHairColor(EntityDatabaseID DatabaseID, int FacialHairColor)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET facial_hair_color={1} WHERE char_id={0} LIMIT 1;", DatabaseID, FacialHairColor).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterFacialHairColorHighlight(EntityDatabaseID DatabaseID, int FacialHairColorHighlight)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET facial_hair_color_highlight={1} WHERE char_id={0} LIMIT 1;", DatabaseID, FacialHairColorHighlight).ConfigureAwait(true);
		}

		public static async Task SetCustomCharacterFacialHairOpacity(EntityDatabaseID DatabaseID, float fOpacity)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET facial_hair_opacity={1} WHERE char_id={0} LIMIT 1;", DatabaseID, fOpacity).ConfigureAwait(true);
		}

		public static async Task ChangeCharacterType(EntityDatabaseID DatabaseID, ECharacterType characterType)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET type={1} WHERE id={0} LIMIT 1;", DatabaseID, (int)characterType).ConfigureAwait(true);
		}

		public static async Task SetCharacterWIPGangTag(EntityDatabaseID DatabaseID, List<GangTagLayer> lstLayers)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET tag_wip='{1}' WHERE id={0} LIMIT 1;", DatabaseID, Newtonsoft.Json.JsonConvert.SerializeObject(lstLayers)).ConfigureAwait(true);
		}

		public static async Task SetCharacterGangTag(EntityDatabaseID DatabaseID, List<GangTagLayer> lstLayers)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET tag='{1}' WHERE id={0} LIMIT 1;", DatabaseID, Newtonsoft.Json.JsonConvert.SerializeObject(lstLayers)).ConfigureAwait(true);
		}

		public static async Task SetFishingXP(EntityDatabaseID DatabaseID, int a_XP)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET fishing_xp={1} WHERE id={0} LIMIT 1;", DatabaseID, a_XP).ConfigureAwait(true);
		}

		public static async Task SetPlayerJob(EntityDatabaseID DatabaseID, EJobID a_JobID)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET job={1} WHERE id={0} LIMIT 1;", DatabaseID, (int)a_JobID).ConfigureAwait(true);
		}

		public static async Task RemoveSavedSessionsForAccount(EntityDatabaseID AccountID, string strIPAddress, string strSerial)
		{
			await m_MySQLInst.QueryGame("DELETE FROM saved_sessions WHERE account_id={0} AND ip_address='{1}' AND serial='{2}';", AccountID, strIPAddress, strSerial).ConfigureAwait(true);
		}

		public static async Task AddTattoo(EntityDatabaseID characterID, int tattooID)
		{
			await m_MySQLInst.QueryGame("INSERT INTO characters_tattoos (char_id, tattoo_id) VALUES({0}, {1});", characterID, tattooID).ConfigureAwait(true);
		}

		public static async Task RemoveTattoo(EntityDatabaseID characterID, int tattooID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM characters_tattoos WHERE char_id={0} AND tattoo_id={1};", characterID, tattooID).ConfigureAwait(true);
		}

		public static async Task SetVehicleMod(EntityDatabaseID VehicleID, EModSlot modSlot, int modIndex)
		{
			// delete existing
			await m_MySQLInst.QueryGame("DELETE FROM vehicle_mods WHERE vehicle={0} AND category={1};", VehicleID, modSlot).ConfigureAwait(true);

			await m_MySQLInst.QueryGame("INSERT INTO vehicle_mods (vehicle, category, mod_index) VALUES({0}, {1}, {2});", VehicleID, modSlot, modIndex).ConfigureAwait(true);
		}

		public static async Task SetVehicleExtra(EntityDatabaseID VehicleID, int extraID, bool bEnabled)
		{
			// delete entry if already present
			await m_MySQLInst.QueryGame("DELETE FROM vehicle_extras WHERE vehicle_id={0} AND extra={1};", VehicleID, extraID).ConfigureAwait(true);
			await m_MySQLInst.QueryGame("INSERT INTO vehicle_extras (vehicle_id, extra, enabled) VALUES({0}, {1}, {2});", VehicleID, extraID, bEnabled).ConfigureAwait(true);
		}

		public static async Task SetVehiclePlateStyle(EntityDatabaseID VehicleID, EPlateType plateType)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET plate_type={1} WHERE id={0} LIMIT 1;", VehicleID, plateType).ConfigureAwait(true);
		}

		public static async Task ToggleVehiclePlate(EntityDatabaseID VehicleID, bool bTogglePlate)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET show_plate={1} WHERE id={0} LIMIT 1;", VehicleID, bTogglePlate).ConfigureAwait(true);
		}

		public static async Task SetVehicleModel(EntityDatabaseID VehicleID, uint model)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET model={1} WHERE id={0} LIMIT 1;", VehicleID, model).ConfigureAwait(true);
		}

		public static async Task SetVehicleCredit(EntityDatabaseID VehicleID, float fCredit)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET credit_amount={1} WHERE id={0} LIMIT 1;", VehicleID, fCredit).ConfigureAwait(true);
		}

		public static async Task SetVehicleColor(EntityDatabaseID VehicleID, int colorPrimaryR, int colorPrimaryG, int colorPrimaryB, int colorSecondaryR, int colorSecondaryG, int colorSecondaryB)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET color1_r={1}, color1_g={2}, color1_b={3}, color2_r={4}, color2_g={5}, color2_b={6} WHERE id={0} LIMIT 1;", VehicleID, colorPrimaryR, colorPrimaryG, colorPrimaryB, colorSecondaryR, colorSecondaryG, colorSecondaryB).ConfigureAwait(true);
		}

		public static async Task SetVehicleTransmissionType(EntityDatabaseID VehicleID, EVehicleTransmissionType transmissionType)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET transmission={1} WHERE id={0} LIMIT 1;", VehicleID, transmissionType).ConfigureAwait(true);
		}

		public static async Task FactionSetMemberRank(EntityDatabaseID a_CharacterID, EntityDatabaseID a_FactionID, int a_RankIndex)
		{
			await m_MySQLInst.QueryGame("UPDATE faction_memberships SET rank_index={0} WHERE character_id={1} AND faction_id={2} LIMIT 1;", a_RankIndex, a_CharacterID, a_FactionID).ConfigureAwait(true);
		}
		public static async Task FactionSetFactionManager(EntityDatabaseID a_CharacterID, EntityDatabaseID a_FactionID, bool a_bIsManager)
		{
			await m_MySQLInst.QueryGame("UPDATE faction_memberships SET manager={0} WHERE character_id={1} AND faction_id={2} LIMIT 1;", a_bIsManager, a_CharacterID, a_FactionID).ConfigureAwait(true);
		}

		public static async Task SetVehicleRadio(EntityDatabaseID VehicleID, int radio)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET radio={1} WHERE id={0} LIMIT 1;", VehicleID, radio).ConfigureAwait(true);
		}

		public static async Task<int> CreateRadio(EntityDatabaseID AccountID, string strName, string strEndpoint, Int64 expiration)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO radios(account, name, endpoint, expiration_time) VALUES({0}, '{1}', '{2}', {3});", AccountID, strName, strEndpoint, expiration).ConfigureAwait(true);
			return (int)mysqlResult.GetInsertID();
		}

		public static async Task UpdateRadioExpiration(EntityDatabaseID AccountID, EntityDatabaseID RadioID, Int64 newExpiration)
		{
			await m_MySQLInst.QueryGame("UPDATE radios SET expiration_time={0} WHERE id={1} AND account={2} LIMIT 1;", newExpiration, RadioID, AccountID).ConfigureAwait(true);
		}

		public static async Task UpdateRadio(EntityDatabaseID AccountID, EntityDatabaseID RadioID, string strName, string strEndpoint)
		{
			await m_MySQLInst.QueryGame("UPDATE radios SET name='{0}', endpoint='{1}' WHERE id={2} AND account={3} LIMIT 1;", strName, strEndpoint, RadioID, AccountID).ConfigureAwait(true);
		}

		public static async Task RemoveRadio(EntityDatabaseID RadioID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM radios WHERE id={0} LIMIT 1;", RadioID).ConfigureAwait(true);
		}

		// NOTE: This logic should match RadioSystem::OnTickExpireRadios
		public static async Task RemoveExpiredRadios()
		{
			await m_MySQLInst.QueryGame("DELETE FROM radios WHERE account != -1 AND expiration_time > 0 AND expiration_time <= UNIX_TIMESTAMP();").ConfigureAwait(true);
		}

		public static async Task<List<RadioInstance>> LoadAllRadios()
		{
			List<RadioInstance> lstRadios = new List<RadioInstance>();

			await RemoveExpiredRadios().ConfigureAwait(true);
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT id, account, name, IFNULL(endpoint, '') AS endpoint, expiration_time FROM radios;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				int radioID = row.GetValue<int>("id");
				EntityDatabaseID accountID = row.GetValue<EntityDatabaseID>("account");
				string strName = row["name"];
				string strEndpoint = row["endpoint"];
				Int64 expirationTime = row.GetValue<Int64>("expiration_time");

				RadioInstance newRadio = new RadioInstance(radioID, accountID, strName, strEndpoint, expirationTime);
				await newRadio.Resolve().ConfigureAwait(true);
				lstRadios.Add(newRadio);
			}

			return lstRadios;
		}
		public static async Task FactionSetFactionMessage(EntityDatabaseID a_FactionID, string a_strMessage)
		{
			await m_MySQLInst.QueryGame("UPDATE factions SET message=\"{0}\" WHERE id={1} LIMIT 1;", a_strMessage, a_FactionID).ConfigureAwait(true);
		}

		public static async Task FactionLeaveFaction(EntityDatabaseID a_CharacterID, EntityDatabaseID a_FactionID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM faction_memberships WHERE character_id={0} AND faction_id={1} LIMIT 1;", a_CharacterID, a_FactionID).ConfigureAwait(true);
		}

		public static async Task FactionDeleteFaction(EntityDatabaseID a_FactionID)
		{
			await m_MySQLInst.QueryGame("DELETE factions, faction_memberships, faction_ranks FROM factions INNER JOIN faction_memberships INNER JOIN faction_ranks WHERE factions.id={0} AND faction_memberships.faction_id={0} AND faction_ranks.faction_id={0};", a_FactionID).ConfigureAwait(true);
		}

		public static async Task FactionSaveRanksAndSalaries(EntityDatabaseID DatabaseID, SFactionRanksUpdateStruct[] a_RanksAndSalaries)
		{
			await m_MySQLInst.QueryGame(@"UPDATE faction_ranks
		SET NAME = (CASE WHEN rank_index = 0 THEN '{1}'
			WHEN rank_index = 1 THEN '{2}'
			WHEN rank_index = 2 THEN '{3}'
			WHEN rank_index = 3 THEN '{4}'
			WHEN rank_index = 4 THEN '{5}'
			WHEN rank_index = 5 THEN '{6}'
			WHEN rank_index = 6 THEN '{7}'
			WHEN rank_index = 7 THEN '{8}'
			WHEN rank_index = 8 THEN '{9}'
			WHEN rank_index = 9 THEN '{10}'
			WHEN rank_index = 10 THEN '{11}'
			WHEN rank_index = 11 THEN '{12}'
			WHEN rank_index = 12 THEN '{13}'
			WHEN rank_index = 13 THEN '{14}'
			WHEN rank_index = 14 THEN '{15}'
			WHEN rank_index = 15 THEN '{16}'
			WHEN rank_index = 16 THEN '{17}'
			WHEN rank_index = 17 THEN '{18}'
			WHEN rank_index = 18 THEN '{19}'
			WHEN rank_index = 19 THEN '{20}'
			END),
		salary = (CASE WHEN rank_index = 0 THEN {21}
			WHEN rank_index = 1 THEN {22}
			WHEN rank_index = 2 THEN {23}
			WHEN rank_index = 3 THEN {24}
			WHEN rank_index = 4 THEN {25}
			WHEN rank_index = 5 THEN {26}
			WHEN rank_index = 6 THEN {27}
			WHEN rank_index = 7 THEN {28}
			WHEN rank_index = 8 THEN {29}
			WHEN rank_index = 9 THEN {30}
			WHEN rank_index = 10 THEN '{31}'
			WHEN rank_index = 11 THEN '{32}'
			WHEN rank_index = 12 THEN '{33}'
			WHEN rank_index = 13 THEN '{34}'
			WHEN rank_index = 14 THEN '{35}'
			WHEN rank_index = 15 THEN '{36}'
			WHEN rank_index = 16 THEN '{37}'
			WHEN rank_index = 17 THEN '{38}'
			WHEN rank_index = 18 THEN '{39}'
			WHEN rank_index = 19 THEN '{40}'
			END)
		WHERE faction_id={0}", DatabaseID,
			a_RanksAndSalaries[0].Name,
			a_RanksAndSalaries[1].Name,
			a_RanksAndSalaries[2].Name,
			a_RanksAndSalaries[3].Name,
			a_RanksAndSalaries[4].Name,
			a_RanksAndSalaries[5].Name,
			a_RanksAndSalaries[6].Name,
			a_RanksAndSalaries[7].Name,
			a_RanksAndSalaries[8].Name,
			a_RanksAndSalaries[9].Name,
			a_RanksAndSalaries[10].Name,
			a_RanksAndSalaries[11].Name,
			a_RanksAndSalaries[12].Name,
			a_RanksAndSalaries[13].Name,
			a_RanksAndSalaries[14].Name,
			a_RanksAndSalaries[15].Name,
			a_RanksAndSalaries[16].Name,
			a_RanksAndSalaries[17].Name,
			a_RanksAndSalaries[18].Name,
			a_RanksAndSalaries[19].Name,
			a_RanksAndSalaries[0].Salary,
			a_RanksAndSalaries[1].Salary,
			a_RanksAndSalaries[2].Salary,
			a_RanksAndSalaries[3].Salary,
			a_RanksAndSalaries[4].Salary,
			a_RanksAndSalaries[5].Salary,
			a_RanksAndSalaries[6].Salary,
			a_RanksAndSalaries[7].Salary,
			a_RanksAndSalaries[8].Salary,
			a_RanksAndSalaries[9].Salary,
			a_RanksAndSalaries[10].Salary,
			a_RanksAndSalaries[11].Salary,
			a_RanksAndSalaries[12].Salary,
			a_RanksAndSalaries[13].Salary,
			a_RanksAndSalaries[14].Salary,
			a_RanksAndSalaries[15].Salary,
			a_RanksAndSalaries[16].Salary,
			a_RanksAndSalaries[17].Salary,
			a_RanksAndSalaries[18].Salary,
			a_RanksAndSalaries[19].Salary
		).ConfigureAwait(true);
		}

		public static async Task SetFactionMoney(EntityDatabaseID DatabaseID, float a_fMoney)
		{
			await m_MySQLInst.QueryGame("UPDATE factions SET money={1} WHERE id={0} LIMIT 1;", DatabaseID, a_fMoney).ConfigureAwait(true);
		}

		public static async Task SetFactionName(EntityDatabaseID DatabaseID, string a_strFactionName)
		{
			await m_MySQLInst.QueryGame("UPDATE factions SET name='{1}' WHERE id={0} LIMIT 1;", DatabaseID, a_strFactionName).ConfigureAwait(true);
		}

		public static async Task SetFactionShortName(EntityDatabaseID DatabaseID, string a_strFactionShortName)
		{
			await m_MySQLInst.QueryGame("UPDATE factions SET short_name='{1}' WHERE id={0} LIMIT 1;", DatabaseID, a_strFactionShortName).ConfigureAwait(true);
		}

		public static async Task SetAdminLevel(EntityDatabaseID AccountID, EAdminLevel a_AdminLevel)
		{
			await m_MySQLInst.QueryAuth("UPDATE accounts SET admin={1} WHERE id={0} LIMIT 1;", AccountID, (int)a_AdminLevel).ConfigureAwait(true);
		}

		public static async Task AddPlayerAdminHistoryEntry(EntityDatabaseID AccountID, string strAction, EntityDatabaseID admin, int amount, EAdminHistoryType type)
		{
			await m_MySQLInst.QueryGame("INSERT INTO player_admin_history (account, action, admin, amount, type) VALUES({0}, '{1}', {2}, {3}, {4});", AccountID, strAction, admin, amount, (long)type).ConfigureAwait(true);
		}

		public static async Task<int> RemoveBans(string UsernameOrMinusOneToIgnore, String SerialOrMinusOneToIgnore, string IPAddressOrMinusOneToIgnore, EntityDatabaseID admin)
		{
			int bansRemoved = 0;

			if (UsernameOrMinusOneToIgnore != "-1")
			{
				BasicAccountInfo accountInfo = await Database.LegacyFunctions.GetBasicAccountInfoFromExactUsername(UsernameOrMinusOneToIgnore).ConfigureAwait(true);

				if (accountInfo.Result == BasicAccountInfo.EGetAccountInfoResult.OK)
				{
					var res = await m_MySQLInst.QueryAuth("DELETE FROM bans WHERE account={0};", accountInfo.AccountID).ConfigureAwait(true);
					bansRemoved += res.GetNumRowsAffected();

					if (bansRemoved >= 1)
					{
						await Database.LegacyFunctions.AddPlayerAdminHistoryEntry(accountInfo.AccountID, "UNBAN", admin, 0,
							EAdminHistoryType.UNBAN).ConfigureAwait(true);
					}
				}
			}

			if (SerialOrMinusOneToIgnore != "-1")
			{
				var res = await m_MySQLInst.QueryAuth("DELETE FROM bans WHERE LOWER(v_serial)=LOWER('{0}');", SerialOrMinusOneToIgnore).ConfigureAwait(true);
				bansRemoved += res.GetNumRowsAffected();
			}

			if (IPAddressOrMinusOneToIgnore != "-1")
			{
				var res = await m_MySQLInst.QueryAuth("DELETE FROM bans WHERE LOWER(ip)=LOWER('{0}');", SerialOrMinusOneToIgnore).ConfigureAwait(true);
				bansRemoved += res.GetNumRowsAffected();
			}

			return bansRemoved;
		}

		public static async Task<AccountBanDetails> CheckForDeviceBan(string strSerial, string ipAddr)
		{
			// Remove any expired bans first
			await RemoveExpiredBans().ConfigureAwait(true);

			// Just grab one, one is enough to deny entry!
			Task<CMySQLResult> idTask = m_MySQLInst.QueryAuth("SELECT IFNULL(until, ''), reason FROM bans WHERE LOWER(v_serial)=LOWER('{0}') OR LOWER(ip)=LOWER('{1}') LIMIT 1;", strSerial, ipAddr);
			CMySQLResult idResult = await idTask.ConfigureAwait(true);

			if (idResult.NumRows() >= 1)
			{
				var row = idResult.GetRow(0);
				return new AccountBanDetails(row["IFNULL(until, '')"], row["reason"]);
			}

			return new AccountBanDetails();
		}

		private static async Task RemoveExpiredBans()
		{
			await m_MySQLInst.QueryAuth("DELETE FROM bans WHERE UNTIL <= NOW();").ConfigureAwait(true);
		}

		public static async Task<AccountBanDetails> CheckForAccountAndDeviceBan(int accountID, string strSerial, string ipAddr)
		{
			// Remove any expired bans first
			await RemoveExpiredBans().ConfigureAwait(true);

			// Just grab one, one is enough to deny entry!
			Task<CMySQLResult> idTask = m_MySQLInst.QueryAuth("SELECT IFNULL(until, ''), reason FROM bans WHERE account={2} OR LOWER(v_serial)=LOWER('{0}') OR LOWER(ip)=LOWER('{1}') LIMIT 1;", strSerial, ipAddr, accountID);
			CMySQLResult idResult = await idTask.ConfigureAwait(true);

			if (idResult.NumRows() >= 1)
			{
				var row = idResult.GetRow(0);
				return new AccountBanDetails(row["IFNULL(until, '')"], row["reason"]);
			}

			return new AccountBanDetails();
		}

		// DONATOR CURRENCY
		public static async Task<int> GetDonatorCurrency(EntityDatabaseID AccountID)
		{
			int returnValue = 0;
			CMySQLResult result = await m_MySQLInst.QueryAuth("SELECT credits FROM accounts WHERE id={0}", AccountID).ConfigureAwait(true);

			if (result.NumRows() > 0)
			{
				CMySQLRow row = result.GetRow(0);

				returnValue = Convert.ToInt32(row["credits"]);
			}
			return returnValue;
		}

		public static async Task AddDonatorCurrency(EntityDatabaseID AccountID, int amount)
		{
			await m_MySQLInst.QueryAuth("UPDATE accounts SET credits=credits+{1} WHERE id={0} LIMIT 1;", AccountID, amount).ConfigureAwait(true);
		}

		public static async Task SubtractDonatorCurrency(EntityDatabaseID AccountID, int amount)
		{
			await m_MySQLInst.QueryAuth("UPDATE accounts SET credits=credits-{1} WHERE id={0} LIMIT 1;", AccountID, amount).ConfigureAwait(true);
		}

		public static async Task SetDonatorCurrency(EntityDatabaseID AccountID, int amount)
		{
			await m_MySQLInst.QueryAuth("UPDATE accounts SET credits={1} WHERE id={0} LIMIT 1;", AccountID, amount).ConfigureAwait(true);
		}
		// END DONATOR CURRENCY

		// BANS
		public static async Task AddDurationBan(string serial, string ip, EntityDatabaseID accountID, EntityDatabaseID adminAccountID, string strReason, int DurationInHours)
		{
			await m_MySQLInst.QueryAuth("INSERT INTO bans (v_serial, ip, account, admin, reason, until) VALUES(UPPER('{0}'), '{1}', {2}, {3}, '{4}', TIMESTAMPADD(HOUR, {5}, NOW()));", serial, ip, accountID, adminAccountID, strReason, DurationInHours).ConfigureAwait(true);
			await AddPlayerAdminHistoryEntry(accountID, strReason, adminAccountID, DurationInHours, EAdminHistoryType.BAN).ConfigureAwait(true);
		}

		public static async Task AddPermanentBan(string serial, string ip, EntityDatabaseID accountID, EntityDatabaseID adminAccountID, string strReason)
		{
			await m_MySQLInst.QueryAuth("INSERT INTO bans (v_serial, ip, account, admin, reason) VALUES(UPPER('{0}'), '{1}', {2}, {3}, '{4}');", serial, ip, accountID, adminAccountID, strReason).ConfigureAwait(true);
			await AddPlayerAdminHistoryEntry(accountID, strReason, adminAccountID, 0, EAdminHistoryType.BAN).ConfigureAwait(true);
		}

		// PUNISHMENT POINTS
		public static async Task<int> GetActivePunishmentPoints(EntityDatabaseID AccountID)
		{
			int returnValue = 0;
			CMySQLResult result = await m_MySQLInst.QueryAuth("SELECT GREATEST(FLOOR(punishpoints-((DATEDIFF(NOW(), IFNULL(punishdate, NOW()))/45)*2)), 0) FROM accounts WHERE id={0};", AccountID).ConfigureAwait(true);

			if (result.NumRows() > 0)
			{
				CMySQLRow row = result.GetRow(0);

				returnValue = Convert.ToInt32(row.GetFields().ElementAt(0).Value);
			}
			return returnValue;
		}

		public static async Task<int> GetAllPunishmentPoints(EntityDatabaseID AccountID)
		{
			int returnValue = 0;
			CMySQLResult result = await m_MySQLInst.QueryAuth("SELECT punishpoints FROM accounts WHERE id={0};", AccountID).ConfigureAwait(true);

			if (result.NumRows() > 0)
			{
				CMySQLRow row = result.GetRow(0);

				returnValue = Convert.ToInt32(row["punishpoints"]);
			}
			return returnValue;
		}

		public static async Task<bool> IsEntityInactivityProtected(EntityDatabaseID EntityID, EDonationInactivityPurchasables a_Type)
		{
			bool returnValue = false;
			Int64 currentTime = Helpers.GetUnixTimestamp();

			if (a_Type == EDonationInactivityPurchasables.PropertyPurchasable)
			{
				CMySQLResult result = await m_MySQLInst.QueryGame("SELECT id FROM donation_inventory WHERE time_expire > {0} AND donation_id={1} AND property_id={2};", currentTime, a_Type, EntityID).ConfigureAwait(true);
				returnValue = result.NumRows() > 0;
			}
			else if (a_Type == EDonationInactivityPurchasables.VehiclePurchasable)
			{
				CMySQLResult result = await m_MySQLInst.QueryGame("SELECT id FROM donation_inventory WHERE time_expire > {0} AND donation_id={1} AND vehicle_id={2};", currentTime, a_Type, EntityID).ConfigureAwait(true);
				returnValue = result.NumRows() > 0;
			}
			return returnValue;
		}

		public static async Task<bool> IsCharacterInactive(EntityDatabaseID CharacterID)
		{
			bool returnValue = false;
			CMySQLResult result = await m_MySQLInst.QueryGame("SELECT DATEDIFF(NOW(), last_seen) FROM characters WHERE id={0};", CharacterID).ConfigureAwait(true);
			if (result.NumRows() > 0)
			{
				CMySQLRow row = result.GetRow(0);
				int daysSinceLastSeen = Convert.ToInt32(row.GetFields().Values.ElementAt(0));
				returnValue = daysSinceLastSeen > InactivityScannerContains.numDaysToConsiderInactiveForOwnerLogin;
			}
			return returnValue;
		}

		public static async Task AddPunishmentPoints(EntityDatabaseID AccountID, int amount, EntityDatabaseID admin, string reason)
		{
			await m_MySQLInst.QueryAuth("UPDATE accounts SET punishpoints=(SELECT GREATEST(FLOOR(punishpoints-((DATEDIFF(NOW(), IFNULL(punishdate, NOW()))/45)*2)), 0) FROM accounts WHERE id={0})+{1}, punishdate=NOW() WHERE id={0} LIMIT 1;", AccountID, amount).ConfigureAwait(true);
			await AddPlayerAdminHistoryEntry(AccountID, reason, admin, amount, EAdminHistoryType.PUNISH_POINTS).ConfigureAwait(true);
		}

		public static async Task SubtractPunishmentPoints(EntityDatabaseID AccountID, int amount)
		{
			await m_MySQLInst.QueryAuth("UPDATE accounts SET punishpoints=(SELECT GREATEST(FLOOR(punishpoints-((DATEDIFF(NOW(), IFNULL(punishdate, NOW()))/45)*2)), 0) FROM accounts WHERE id={0})-{1}, punishdate=NOW() WHERE id={0} LIMIT 1;", AccountID, amount).ConfigureAwait(true);
		}
		// END PUNISHMENT POINTS

		public static async Task SetPlayerMoney(EntityDatabaseID DatabaseID, float a_fMoney)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET money={1} WHERE id={0} LIMIT 1;", DatabaseID, a_fMoney).ConfigureAwait(true);
		}

		public static async Task SetPlayerAge(EntityDatabaseID databaseID, int playerAge)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET age={1} WHERE id={0} LIMIT 1;", databaseID, playerAge).ConfigureAwait(true);
		}

		public static async Task AddOfflinePlayerMoney(EntityDatabaseID DatabaseID, float a_fMoneyToAdd)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET money=money+{1} WHERE id={0} LIMIT 1;", DatabaseID, a_fMoneyToAdd).ConfigureAwait(true);
		}

		public static async Task AddOfflinePlayerBankMoney(EntityDatabaseID DatabaseID, float a_fMoneyToAdd)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET bank_money=bank_money+{1} WHERE id={0} LIMIT 1;", DatabaseID, a_fMoneyToAdd).ConfigureAwait(true);
		}

		public static async Task<PendingWeaponLicenseStates> GetPendingFirearmsLicenseStates(EntityDatabaseID DatabaseID)
		{
			PendingWeaponLicenseStates returnValue = new PendingWeaponLicenseStates
			{
				Tier1 = EPendingFirearmLicenseState.None,
				Tier2 = EPendingFirearmLicenseState.None,
			};

			CMySQLResult result = await m_MySQLInst.QueryGame("SELECT pending_firearms_lic_state_tier1, pending_firearms_lic_state_tier2 FROM characters WHERE id={0} LIMIT 1;", DatabaseID).ConfigureAwait(true);

			if (result.NumRows() > 0)
			{
				CMySQLRow row = result.GetRow(0);

				returnValue.Tier1 = (EPendingFirearmLicenseState)Convert.ToInt32(row["pending_firearms_lic_state_tier1"]);
				returnValue.Tier2 = (EPendingFirearmLicenseState)Convert.ToInt32(row["pending_firearms_lic_state_tier2"]);
			}

			return returnValue;
		}

		public static async Task SetPlayerDuty(EntityDatabaseID DatabaseID, EDutyType a_DutyType)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET duty={1} WHERE id={0} LIMIT 1;", DatabaseID, (int)a_DutyType).ConfigureAwait(true);
		}

		public static async Task ArrestPlayerTask(EntityDatabaseID DatabaseID, Int64 a_UnjailTimestamp, float a_fBailAmount, string a_strReason, EPrisonCell a_PrisonCell)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET unjail_time={1}, bail_amount={2}, jail_reason='{3}', cell_number={4} WHERE id={0} LIMIT 1;", DatabaseID, a_UnjailTimestamp, a_fBailAmount, a_strReason, (int)a_PrisonCell).ConfigureAwait(true);
		}

		public static async Task UnjailPlayer(EntityDatabaseID DatabaseID)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET unjail_time=0, bail_amount=0.0, jail_reason='', cell_number=0 WHERE id={0} LIMIT 1;", DatabaseID).ConfigureAwait(true);
		}

		public static async Task SetPlayerCuffedState(EntityDatabaseID DatabaseID, bool a_bCuffed, EntityDatabaseID a_Cuffer)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET cuffed={1}, cuffer={2} WHERE id={0} LIMIT 1;", DatabaseID, a_bCuffed, a_Cuffer).ConfigureAwait(true);
		}

		public static async Task SetPlayerBankMoney(EntityDatabaseID DatabaseID, float a_fMoney)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET bank_money={1} WHERE id={0} LIMIT 1;", DatabaseID, a_fMoney).ConfigureAwait(true);
		}

		public static async Task SetPlayerPendingJobMoney(EntityDatabaseID DatabaseID, float a_fMoney)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET pending_job_money={1} WHERE id={0} LIMIT 1;", DatabaseID, a_fMoney).ConfigureAwait(true);
		}

		public static async Task<Int64> UpdateVehicleLastUsedTime(EntityDatabaseID DatabaseID)
		{
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			await m_MySQLInst.QueryGame("UPDATE vehicles SET last_used={1} WHERE id={0};", DatabaseID, unixTimestamp).ConfigureAwait(true);
			return unixTimestamp;
		}

		public static async Task SetVehicleExpiryTime(EntityDatabaseID DatabaseID, Int64 a_ExpiryTimestamp)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET expiry_time={1} WHERE id={0};", DatabaseID, a_ExpiryTimestamp).ConfigureAwait(true);
		}

		public static async Task SetVehicleTowed(EntityDatabaseID DatabaseID, bool bTowed)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET towed={1} WHERE id={0};", DatabaseID, bTowed).ConfigureAwait(true);
		}

		public static async Task SetVehiclePaymentsRemaining(EntityDatabaseID DatabaseID, int a_Value)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET payments_remaining={1} WHERE id={0};", DatabaseID, a_Value).ConfigureAwait(true);
		}

		public static async Task SetVehiclePaymentsMade(EntityDatabaseID DatabaseID, int a_Value)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET payments_made={1} WHERE id={0};", DatabaseID, a_Value).ConfigureAwait(true);
		}

		public static async Task SetVehiclePaymentsMissed(EntityDatabaseID DatabaseID, int a_Value)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET payments_missed={1} WHERE id={0};", DatabaseID, a_Value).ConfigureAwait(true);
		}

		public static async Task OfflineSetPlayerPosition(string strCharacterName, Vector3 vecPos, Dimension dimension)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET x={1}, y={2}, z={3}, dimension={4} WHERE name='{0}' LIMIT 1;", strCharacterName, vecPos.X, vecPos.Y, vecPos.Z, dimension).ConfigureAwait(true);
		}

		public static async Task SetPlayerSpawn(EntityDatabaseID DatabaseID, Vector3 vecPos, float fRot, Dimension dimension)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET x={1}, y={2}, z={3}, rz={4}, dimension={5} WHERE id={0} LIMIT 1;", DatabaseID, vecPos.X, vecPos.Y, vecPos.Z, fRot, dimension).ConfigureAwait(true);
		}

		public static async Task UpdateCharacterLastSeen(EntityDatabaseID DatabaseID)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET last_seen=CURRENT_TIMESTAMP WHERE id={0};", DatabaseID).ConfigureAwait(true);
		}

		public static async Task UpdateAccountDiscordLink(EntityDatabaseID AccountID, UInt64 DiscordID)
		{
			if (DiscordID == 0)
			{
				await m_MySQLInst.QueryAuth("UPDATE accounts SET discord=null WHERE id={0};", AccountID).ConfigureAwait(true);
			}
			else
			{
				await m_MySQLInst.QueryAuth("UPDATE accounts SET discord={1} WHERE id={0};", AccountID, DiscordID).ConfigureAwait(true);
			}
		}

		public static async Task<bool> IsDiscordAccountAlreadyLinkedToAnotherAccount(UInt64 DiscordID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryAuth("SELECT id FROM accounts WHERE discord={0} LIMIT 1;", DiscordID).ConfigureAwait(true);
			return mysqlResult.NumRows() == 0;
		}

		// ADMIN CHECK
		public static async Task<AdminCheckInfo> GetAdminCheckInfo(EntityDatabaseID AccountID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT admin_notes FROM game_accounts WHERE account_id={0} LIMIT 1;", AccountID).ConfigureAwait(true);

			if (mysqlResult.NumRows() >= 1)
			{
				CMySQLRow row = mysqlResult.GetRow(0);

				AdminCheckInfo checkInfo = new AdminCheckInfo
				{
					AdminNotes = row["admin_notes"]
				};

				// get player admin history too
				CMySQLResult mysqlResultHistory = await m_MySQLInst.QueryGame("SELECT action, admin, datetime FROM player_admin_history WHERE account={0} ORDER BY datetime DESC;", AccountID).ConfigureAwait(true);
				foreach (var historyRow in mysqlResultHistory.GetRows())
				{
					checkInfo.AdminHistory.Add(Helpers.FormatString("{0} - {1} - By account #{2}", historyRow["datetime"], historyRow["action"], historyRow["admin"]));
				}

				return checkInfo;
			}

			return new AdminCheckInfo
			{
				AdminNotes = "ERROR"
			};
		}

		public static async Task UpdateAdminNotes(EntityDatabaseID AccountID, string strNotes)
		{
			await m_MySQLInst.QueryGame("UPDATE game_accounts SET admin_notes='{0}' WHERE account_id={1};", strNotes, AccountID).ConfigureAwait(true);
		}

		public static async Task<bool> SerialInUse(string strSerial)
		{
			Task<CMySQLResult> UniqueSerialCheckTask = m_MySQLInst.QueryGame("SELECT account_id from game_accounts WHERE LOWER(serial)=LOWER('{0}') LIMIT 1;", strSerial);
			CMySQLResult serialRows = await UniqueSerialCheckTask.ConfigureAwait(true);

			return serialRows.NumRows() > 0;
		}

#if DEBUG
		public static async Task<RegisterAPIResponse> RegisterAccount(string strUsername, string strPassword, string strEmail, string strSerial)
		{
			RegisterAPIResponse returnValue;

			Task<CMySQLResult> UniqueNameCheckTask = m_MySQLInst.QueryAuth("SELECT id, username, email from accounts WHERE LOWER(username)=LOWER('{0}') or LOWER(email)=LOWER('{1}') LIMIT 1;", strUsername, strEmail);
			CMySQLResult rows = await UniqueNameCheckTask.ConfigureAwait(true);

			bool serialInUse = await SerialInUse(strSerial).ConfigureAwait(true);

			if (rows.NumRows() == 0 && !serialInUse)
			{
				// Create account
				string strHashedPassword = Auth.GeneratePassword(strPassword);

				var result = await m_MySQLInst.QueryAuth("INSERT INTO accounts (username, password, email, registerdate, activated) VALUES(LOWER('{0}'), '{1}', LOWER('{2}'), NOW(), 1);", strUsername, strHashedPassword, strEmail).ConfigureAwait(true);
				int accountID = Convert.ToInt32(result.GetInsertID());

				// await CreateGameAccount(accountID, strSerial).ConfigureAwait(true);
				returnValue = new RegisterAPIResponse
				{
					success = true,
					account = accountID
				};
			}
			else
			{
				string strError;
				if (serialInUse)
				{
					strError = "Serial in use";
				}
				else
				{
					string strDBUsername = rows.GetRow(0)["username"];
					string strDBEmail = rows.GetRow(0)["email"];

					if (strDBUsername.ToLower() == strUsername.ToLower())
					{
						strError = "Username taken";
					}
					else
					{
						strError = "Email in use";
					}
				}

				returnValue = new RegisterAPIResponse
				{
					success = false,
					error = strError
				};
			}

			return returnValue;
		}
#endif

		public static async void UpdateLastLogin(int accountID)
		{
			await m_MySQLInst.QueryAuth("UPDATE accounts SET ucp_lastlogin=NOW() WHERE id={0};", accountID).ConfigureAwait(true);
		}

		public static async Task<Dictionary<EAchievementID, CAchievementInstance>> GetPlayerAchievements(int a_AccountID)
		{
			Dictionary<EAchievementID, CAchievementInstance> result = new Dictionary<EAchievementID, CAchievementInstance>();

			var mysqlResult = await m_MySQLInst.QueryGame("SELECT id, achievement_id, unlocked FROM achievements WHERE account={0};", a_AccountID).ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				EntityDatabaseID databaseID = row.GetValue<EntityDatabaseID>("id");
				EAchievementID achievementID = (EAchievementID)row.GetValue<int>("achievement_id");
				Int64 unlockedTimestamp = row.GetValue<Int64>("unlocked");

				result.Add(achievementID, new CAchievementInstance(databaseID, a_AccountID, achievementID, unlockedTimestamp));
			}

			return result;
		}

		public static async Task RemoveCharacterActiveLanguage(EntityDatabaseID characterID, ECharacterLanguage CurrentActiveLanguageID)
		{
			await m_MySQLInst.QueryGame("UPDATE `character_languages` SET `active` = false WHERE `parent` = {0} AND `language_id` ={1};", characterID, CurrentActiveLanguageID).ConfigureAwait(true);
		}

		public static async Task SetCharacterActiveLanguage(EntityDatabaseID characterID, ECharacterLanguage NewActiveLanguage)
		{
			await m_MySQLInst.QueryGame("UPDATE `character_languages` SET `active` = true WHERE `parent` = {0} AND `language_id` = {1} LIMIT 1;", characterID, NewActiveLanguage).ConfigureAwait(true);
		}

		public static async Task SetCharacterLanguageProgress(EntityDatabaseID characterID, ECharacterLanguage languageID, float fProgress)
		{
			await m_MySQLInst.QueryGame("UPDATE character_languages SET progress={0} WHERE language_id={1} AND parent={2}", fProgress, languageID, characterID).ConfigureAwait(true);
		}

		public static async Task AddXPForLanguage(EntityDatabaseID characterID, ECharacterLanguage languageID, float fProgress)
		{
			await m_MySQLInst.QueryGame("UPDATE character_languages SET progress={0} WHERE parent={1} AND language_id={2}", fProgress, characterID, languageID).ConfigureAwait(true);
		}

		public static async Task RemoveLanguageForPlayer(EntityDatabaseID characterID, ECharacterLanguage languageID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM character_languages WHERE language_id = {0} AND parent = {1} LIMIT 1;", languageID, characterID).ConfigureAwait(true);
		}

		public static async Task<ChatSettings> GetChatSettings(int a_AccountID)
		{
			ChatSettings result = null;

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT num_messages, tab_0, tab_1, tab_2, tab_3, background, background_alpha FROM chat_settings WHERE account_id={0};", a_AccountID).ConfigureAwait(true);

			if (mysqlResult.NumRows() == 1)
			{
				CMySQLRow row = mysqlResult.GetRow(0);
				result = new ChatSettings();

				result.numMessagesToShow = row.GetValue<int>("num_messages");
				result.Tabs.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ChatTab>(row.GetValue<string>("tab_0")));
				result.Tabs.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ChatTab>(row.GetValue<string>("tab_1")));
				result.Tabs.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ChatTab>(row.GetValue<string>("tab_2")));
				result.Tabs.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ChatTab>(row.GetValue<string>("tab_3")));
				result.chatboxBackground = row.GetValue<bool>("background");
				result.chatboxBackgroundAlpha = row.GetValue<float>("background_alpha");
			}

			return result;
		}

		public static async Task ResetChatSettings(int a_AccountID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM chat_settings WHERE account_id={0};", a_AccountID).ConfigureAwait(true);
		}

		public static async Task SaveChatSettings(int a_AccountID, ChatSettings chatSettings)
		{
			// just delete and re-insert for sanity
			await ResetChatSettings(a_AccountID).ConfigureAwait(true);

			await m_MySQLInst.QueryGame("INSERT INTO chat_settings(account_id, num_messages, tab_0, tab_1, tab_2, tab_3, background, background_alpha) VALUES({0}, {1}, '{2}', '{3}', '{4}', '{5}', {6}, {7});", a_AccountID, chatSettings.numMessagesToShow, Newtonsoft.Json.JsonConvert.SerializeObject(chatSettings.Tabs[0]), Newtonsoft.Json.JsonConvert.SerializeObject(chatSettings.Tabs[1]), Newtonsoft.Json.JsonConvert.SerializeObject(chatSettings.Tabs[2]), Newtonsoft.Json.JsonConvert.SerializeObject(chatSettings.Tabs[3]), chatSettings.chatboxBackground, chatSettings.chatboxBackgroundAlpha).ConfigureAwait(true);
		}

		public static async Task ResetControls(int a_AccountID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM game_controls WHERE account={0};", a_AccountID).ConfigureAwait(true);
		}

		public static async Task SaveControls(int a_AccountID, List<GameControlObject> lstGameControls)
		{
			// just delete and re-insert for sanity
			await ResetControls(a_AccountID).ConfigureAwait(true);

			await m_MySQLInst.QueryGame("INSERT INTO game_controls(account, controls) VALUES({0}, '{1}');", a_AccountID, Newtonsoft.Json.JsonConvert.SerializeObject(lstGameControls)).ConfigureAwait(true);
		}

		public static async Task<BasicAccountInfo> GetBasicAccountInfoFromExactUsername(string strUsername)
		{
			Task<CMySQLResult> idTask = m_MySQLInst.QueryAuth("SELECT id, username, admin, IFNULL(discord, '0') AS discord FROM accounts WHERE LOWER(username)=LOWER('{0}') LIMIT 1;", strUsername);
			CMySQLResult idResult = await idTask.ConfigureAwait(true);

			if (idResult.NumRows() == 1)
			{
				return new BasicAccountInfo(int.Parse(idResult.GetRow(0)["id"]), idResult.GetRow(0)["username"], UInt64.Parse(idResult.GetRow(0)["discord"]), (EAdminLevel)Convert.ToInt32(idResult.GetRow(0)["admin"]));
			}

			return new BasicAccountInfo(BasicAccountInfo.EGetAccountInfoResult.NotFound);
		}

		public static async Task<BasicAccountInfo> GetBasicAccountInfoFromDBID(EntityDatabaseID playerDBID)
		{
			Task<CMySQLResult> idTask = m_MySQLInst.QueryAuth("SELECT id, username, admin, IFNULL(discord, '0') AS discord FROM accounts WHERE id = {0} LIMIT 1;", playerDBID);
			CMySQLResult idResult = await idTask.ConfigureAwait(true);

			if (idResult.NumRows() == 1)
			{
				return new BasicAccountInfo(int.Parse(idResult.GetRow(0)["id"]), idResult.GetRow(0)["username"], UInt64.Parse(idResult.GetRow(0)["discord"]), (EAdminLevel)Convert.ToInt32(idResult.GetRow(0)["admin"]));
			}

			return new BasicAccountInfo(BasicAccountInfo.EGetAccountInfoResult.NotFound);
		}

		public static async Task<BasicAccountInfo> GetBasicAccountInfoFromCharacterID(EntityDatabaseID characterID)
		{
			CMySQLResult characterRows = await m_MySQLInst.QueryGame("SELECT account from characters WHERE id={0};", characterID).ConfigureAwait(true);

			if (characterRows.NumRows() == 1)
			{
				CMySQLRow row = characterRows.GetRow(0);
				Task<CMySQLResult> idTask = m_MySQLInst.QueryAuth("SELECT id, username, admin, IFNULL(discord, '0') AS discord FROM accounts WHERE id = {0} LIMIT 1;", row["account"]);
				CMySQLResult idResult = await idTask.ConfigureAwait(true);

				if (idResult.NumRows() == 1)
				{
					return new BasicAccountInfo(int.Parse(idResult.GetRow(0)["id"]), idResult.GetRow(0)["username"], UInt64.Parse(idResult.GetRow(0)["discord"]), (EAdminLevel)Convert.ToInt32(idResult.GetRow(0)["admin"]));
				}
			}

			return new BasicAccountInfo(BasicAccountInfo.EGetAccountInfoResult.NotFound);
		}

		public static async Task<BasicAccountInfo> GetBasicAccountInfoFromDiscordID(UInt64 a_DiscordID)
		{
			Task<CMySQLResult> idTask = m_MySQLInst.QueryAuth("SELECT id, username, admin, IFNULL(discord, '0') AS discord FROM accounts WHERE discord={0};", a_DiscordID);
			CMySQLResult idResult = await idTask.ConfigureAwait(true);

			if (idResult.NumRows() == 1)
			{
				return new BasicAccountInfo(int.Parse(idResult.GetRow(0)["id"]), idResult.GetRow(0)["username"], UInt64.Parse(idResult.GetRow(0)["discord"]), (EAdminLevel)Convert.ToInt32(idResult.GetRow(0)["admin"]));
			}
			else if (idResult.NumRows() > 1)
			{
				return new BasicAccountInfo(BasicAccountInfo.EGetAccountInfoResult.MultipleFound);
			}

			return new BasicAccountInfo(BasicAccountInfo.EGetAccountInfoResult.NotFound);
		}

		public static async Task<BasicAccountInfo> GetBasicAccountInfoFromPartialUsername(string strUsername)
		{
			Task<CMySQLResult> idTask = m_MySQLInst.QueryAuth("SELECT id, username, admin, IFNULL(discord, '0') AS discord FROM accounts WHERE LOWER(username) LIKE LOWER('%{0}%');", strUsername);
			CMySQLResult idResult = await idTask.ConfigureAwait(true);

			if (idResult.NumRows() == 1)
			{
				return new BasicAccountInfo(int.Parse(idResult.GetRow(0)["id"]), idResult.GetRow(0)["username"], UInt64.Parse(idResult.GetRow(0)["discord"]), (EAdminLevel)Convert.ToInt32(idResult.GetRow(0)["admin"]));
			}
			if (idResult.NumRows() > 1)
			{
				return new BasicAccountInfo(BasicAccountInfo.EGetAccountInfoResult.MultipleFound);
			}

			return new BasicAccountInfo(BasicAccountInfo.EGetAccountInfoResult.NotFound);
		}

		public static async Task<List<CDatabaseStructureDancer>> LoadAllDancers()
		{
			List<CDatabaseStructureDancer> lstDancers = new List<CDatabaseStructureDancer>();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM dancers;").ConfigureAwait(true);

			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstDancers.Add(new CDatabaseStructureDancer(row));
			}

			return lstDancers;
		}

		public static async Task<List<CDatabaseStructureStore>> LoadAllStores()
		{
			List<CDatabaseStructureStore> lstStores = new List<CDatabaseStructureStore>();

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM stores;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstStores.Add(new CDatabaseStructureStore(row));
			}

			return lstStores;
		}

		public static async Task<List<CDatabaseStructureWorldBlip>> LoadAllWorldBlips()
		{
			List<CDatabaseStructureWorldBlip> lstWorldBlips = new List<CDatabaseStructureWorldBlip>();

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM world_blips;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstWorldBlips.Add(new CDatabaseStructureWorldBlip(row));
			}

			return lstWorldBlips;
		}

		public static async Task<int> GetGlobalPeakPlayerCount()
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT highest_peak_playercount FROM globals LIMIT 1;").ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				return Convert.ToInt32(mysqlResult.GetRow(0)["highest_peak_playercount"]);
			}

			return 0;
		}

		public static async Task UpdateGlobalPeakPlayerCount(int newPeak)
		{
			await m_MySQLInst.QueryGame("UPDATE globals SET highest_peak_playercount={0};", newPeak).ConfigureAwait(true);
		}

		public static async Task<int> GetGlobalPayDayProgress()
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT payday_progress FROM globals LIMIT 1;").ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				return Convert.ToInt32(mysqlResult.GetRow(0)["payday_progress"]);
			}

			return 0;
		}

		public static async Task AddGlobalPayDayProgress(int progress)
		{
			await m_MySQLInst.QueryGame("UPDATE globals SET payday_progress=payday_progress+{0};", progress).ConfigureAwait(true);
		}

		public static async Task ResetGlobalPayDayProgress()
		{
			await m_MySQLInst.QueryGame("UPDATE globals SET payday_progress=0;").ConfigureAwait(true);
		}

		public static async Task<ulong> GetNextPropertyXPRunAt()
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT next_property_xp_run_at FROM globals LIMIT 1;").ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				return Convert.ToUInt64(mysqlResult.GetRow(0)["next_property_xp_run_at"]);
			}

			return 0;
		}

		public static async Task TouchNextPropertyRunAt()
		{
			await m_MySQLInst.QueryGame("UPDATE globals SET next_property_xp_run_at= UNIX_TIMESTAMP(current_timestamp() + interval 1 week);").ConfigureAwait(true);
		}

		public static async Task<List<DonationPurchasable>> LoadDonationPurchasables()
		{
			List<DonationPurchasable> lstDonationPurchasables = new List<DonationPurchasable>();

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM donation_store;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				UInt32 ID = row.GetValue<UInt32>("id");
				string Title = row["title"];
				string Desc = row["desc"];
				int Cost = row.GetValue<int>("cost");
				EDonationType donationType = (EDonationType)row.GetValue<int>("type");
				bool bUnique = bool.Parse(row["unique"]);
				int Duration = row.GetValue<int>("duration");
				EDonationEffect donationEffect = (EDonationEffect)row.GetValue<int>("effect");
				bool bActive = bool.Parse(row["active"]);

				lstDonationPurchasables.Add(new DonationPurchasable(ID, Title, Desc, Cost, donationType, bUnique, Duration, bActive, donationEffect));
			}

			return lstDonationPurchasables;
		}

		public static async Task RemoveDonationInventoryItem(EntityDatabaseID a_AccountID, EntityDatabaseID a_CharacterID, EntityDatabaseID a_DonationItemID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM donation_inventory WHERE account={0} AND character_id={1} AND id={2} LIMIT 1;", a_AccountID, a_CharacterID, a_DonationItemID).ConfigureAwait(true);
		}

		public static async Task ConsumeDonationInventoryItem(EntityDatabaseID a_AccountID, EntityDatabaseID a_DonationItemID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM donation_inventory WHERE account={0} AND id={1} LIMIT 1;", a_AccountID, a_DonationItemID).ConfigureAwait(true);
		}

		public static async Task RemoveExpiredRentalVehicles()
		{
			await m_MySQLInst.QueryGame("DELETE FROM vehicles WHERE type={0} AND expiry_time <= UNIX_TIMESTAMP();", EVehicleType.RentalCar).ConfigureAwait(true);
		}

		public static async Task<CDatabaseStructureVehicle> LoadVehicle(EntityDatabaseID a_vehicleID)
		{
			CDatabaseStructureVehicle retVal = null;
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM vehicles WHERE id = {0} LIMIT 1;", a_vehicleID).ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				retVal = new CDatabaseStructureVehicle(mysqlResult.GetRow(0));
				retVal.CopyInventory(await LoadInventoryRecursiveAsync(EItemParentTypes.Vehicle, retVal.vehicleID).ConfigureAwait(true));

				retVal.VehicleMods = await LoadVehicleMods(a_vehicleID).ConfigureAwait(true);
				retVal.VehicleExtras = await LoadVehicleExtras(a_vehicleID).ConfigureAwait(true);
			}

			return retVal;
		}

		public static async Task<Dictionary<EModSlot, int>> LoadVehicleMods(EntityDatabaseID a_vehicleID)
		{
			Dictionary<EModSlot, int> dictMods = new Dictionary<EModSlot, int>();

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT category, mod_index FROM vehicle_mods WHERE vehicle={0};", a_vehicleID).ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				EModSlot slot = (EModSlot)row.GetValue<int>("category");
				int mod_index = row.GetValue<int>("mod_index");
				dictMods[slot] = mod_index;
			}

			return dictMods;
		}

		public static async Task<Dictionary<int, bool>> LoadVehicleExtras(EntityDatabaseID a_vehicleID)
		{
			Dictionary<int, bool> dictExtras = new Dictionary<int, bool>();

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT extra, enabled FROM vehicle_extras WHERE vehicle_id={0};", a_vehicleID).ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				int extra = row.GetValue<int>("extra");
				bool bEnabled = row.GetValue<bool>("enabled");
				dictExtras.Add(extra, bEnabled);
			}

			return dictExtras;
		}

		public static async Task<List<CDatabaseStructureBank>> LoadAllBanks()
		{
			List<CDatabaseStructureBank> lstBanks = new List<CDatabaseStructureBank>();

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM banks;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstBanks.Add(new CDatabaseStructureBank(row));
			}

			return lstBanks;
		}

		public static async Task<List<CDatabaseStructureDutyPoint>> LoadAllDutyPoints()
		{
			List<CDatabaseStructureDutyPoint> lstPoints = new List<CDatabaseStructureDutyPoint>();
			var mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM duty_points;").ConfigureAwait(true);

			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstPoints.Add(new CDatabaseStructureDutyPoint(row));
			}

			return lstPoints;
		}

		public static async Task<List<CDatabaseStructureFuelPoint>> LoadAllFuelPoints()
		{
			List<CDatabaseStructureFuelPoint> lstPoints = new List<CDatabaseStructureFuelPoint>();
			var mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM fuel_points;").ConfigureAwait(true);

			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstPoints.Add(new CDatabaseStructureFuelPoint(row));
			}

			return lstPoints;
		}

		public static async Task<List<CDatabaseStructureCarWashPoint>> LoadAllCarWashPoints()
		{
			List<CDatabaseStructureCarWashPoint> lstPoints = new List<CDatabaseStructureCarWashPoint>();
			var mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM carwash_points;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstPoints.Add(new CDatabaseStructureCarWashPoint(row));
			}

			return lstPoints;
		}

		public static async Task<List<CDatabaseStructureScooterRentalShop>> LoadAllScooterRentalShops()
		{
			List<CDatabaseStructureScooterRentalShop> lstPoints = new List<CDatabaseStructureScooterRentalShop>();
			var mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM scooter_rental_shops;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstPoints.Add(new CDatabaseStructureScooterRentalShop(row));
			}

			return lstPoints;
		}

		public static async Task<List<CDatabaseStructureVehicleRepairPoint>> LoadAllVehicleRepairPoints()
		{
			List<CDatabaseStructureVehicleRepairPoint> lstPoints = new List<CDatabaseStructureVehicleRepairPoint>();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM vehiclerepair_points;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstPoints.Add(new CDatabaseStructureVehicleRepairPoint(row));
			}

			return lstPoints;
		}

		public static async Task<List<CItemInstanceDef>> GetItemsInsideContainerRecursive(EntityDatabaseID a_ItemDBID)
		{
			List<CItemInstanceDef> lstItemsInsideFurniture = new List<CItemInstanceDef>();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM inventories WHERE parent_type={0} AND parent={1};", (int)EItemParentTypes.Container, a_ItemDBID).ConfigureAwait(true);
			foreach (CMySQLRow itemRow in mysqlResult.GetRows())
			{
				EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
				EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
				string itemValue = itemRow["item_value"];
				EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
				EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
				UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

				lstItemsInsideFurniture.Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, EItemParentTypes.FurnitureContainer, StackSize));

				// get children too, we need to know whats inside this item so we ensure we copy over contained items too
				List<CItemInstanceDef> lstChildItems = await Database.LegacyFunctions.LoadInventoryRecursiveAsync(EItemParentTypes.Container, databaseID).ConfigureAwait(true);
				lstItemsInsideFurniture.AddRange(lstChildItems);
			}

			return lstItemsInsideFurniture;
		}

		public static async Task<CItemInstanceDef> GetInventoryItemFromFurnitureItem(EntityDatabaseID a_FurnitureID, EntityDatabaseID a_ItemDBID)
		{
			CItemInstanceDef itemInsideFurniture = null;
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM inventories WHERE parent_type={0} AND parent={1} AND id={2};", (int)EItemParentTypes.FurnitureContainer, a_FurnitureID, a_ItemDBID).ConfigureAwait(true);
			if (mysqlResult.NumRows() > 0)
			{
				var itemRow = mysqlResult.GetRow(0);

				EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
				EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
				string itemValue = itemRow["item_value"];
				EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
				EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
				UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

				itemInsideFurniture = CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, EItemParentTypes.FurnitureContainer, StackSize);
			}

			return itemInsideFurniture;
		}

		public static async Task<EntityDatabaseID> CreateFurnitureItemInProperty(CItemInstanceDef a_ItemInstance, uint a_FurnitureID, Vector3 a_vecPos, Vector3 a_vecRot, EntityDatabaseID a_CharacterID, EntityDatabaseID a_PropertyID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO inventories(item_id, item_value, x, y, z, rx, ry, rz, dropped_by, dimension, parent_type, parent, stack_size) VALUES({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12});"
				, (int)a_FurnitureID, a_ItemInstance.GetValueDataSerialized(), a_vecPos.X, a_vecPos.Y, a_vecPos.Z, a_vecRot.X, a_vecRot.Y, a_vecRot.Z, a_CharacterID, a_PropertyID, (int)EItemParentTypes.FurnitureInsideProperty, a_PropertyID, a_ItemInstance.StackSize).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task<List<CDatabaseStructureWorldItem>> LoadAllWorldItems()
		{
			List<CDatabaseStructureWorldItem> lstWorldItems = new List<CDatabaseStructureWorldItem>();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM inventories WHERE parent_type={0};", (int)EItemParentTypes.World).ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstWorldItems.Add(new CDatabaseStructureWorldItem(row));
			}

			return lstWorldItems;
		}

		public enum EFactionNameUniqueResult
		{
			IsUnique,
			FullNameTaken,
			ShortNameTaken
		}

		public static async Task<EFactionNameUniqueResult> IsFactionNameUnique(string a_strFullName, string a_strShortName)
		{
			EFactionNameUniqueResult returnValue = EFactionNameUniqueResult.IsUnique;
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT name, short_name FROM factions WHERE LOWER(name)=LOWER('{0}') || LOWER(short_name)=LOWER('{1}');", a_strFullName, a_strShortName).ConfigureAwait(true);
			if (mysqlResult.NumRows() > 0)
			{
				CMySQLRow row = mysqlResult.GetRow(0);

				string strName = row["name"];
				string strShortName = row["short_name"];

				if (strName.ToLower() == a_strFullName.ToLower())
				{
					returnValue = EFactionNameUniqueResult.FullNameTaken;
				}
				else if (strShortName.ToLower() == a_strShortName.ToLower())
				{
					returnValue = EFactionNameUniqueResult.ShortNameTaken;
				}
			}

			return returnValue;
		}

		public static async Task InvitePlayerToFaction(EntityDatabaseID a_TargetCharacter, EntityDatabaseID a_SourceCharacter, EntityDatabaseID a_FactionID)
		{
			await m_MySQLInst.QueryGame("INSERT INTO faction_invites(target_character, source_character, faction, timestamp) VALUES({0}, {1}, {2}, UNIX_TIMESTAMP());", a_TargetCharacter, a_SourceCharacter, a_FactionID).ConfigureAwait(true);
		}

		public static async Task<bool> DoesCharacterAlreadyHavePendingInviteForFaction(EntityDatabaseID a_Character, EntityDatabaseID a_FactionID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT id FROM faction_invites WHERE target_character={0} AND faction={1} LIMIT 1;", a_Character, a_FactionID).ConfigureAwait(true);
			return mysqlResult.NumRows() != 0;
		}


		public static async Task<CDatabaseStructureFaction> CreateFaction(EntityDatabaseID a_CreatingPlayerDBID, string a_strName, string a_strShortName, EFactionType a_factionType, bool bIsOfficial)
		{
			// NOTE: This also loads the faction
			var mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO factions(type, name, official, short_name, message, money, creator) VALUES({0}, '{1}', {2}, '{3}', '{4}', {5}, {6});", a_factionType, a_strName, bIsOfficial, a_strShortName, Helpers.FormatString("Welcome to {0}", a_strShortName), 0.0f, a_CreatingPlayerDBID).ConfigureAwait(true);
			EntityDatabaseID factionDBID = (EntityDatabaseID)mysqlResult.GetInsertID();

			// Add default ranks
			// TODO_MYSQL: Optimize
			const int numRanks = 20;
			for (int i = 0; i < numRanks; ++i)
			{
				await m_MySQLInst.QueryGame("INSERT INTO faction_ranks(faction_id, name, salary, rank_index) VALUES({0}, '{1}', {2}, {3});", factionDBID, Helpers.FormatString("Rank {0}", i), 0.0f, i).ConfigureAwait(true);
			}

			return await LoadSingleFaction(factionDBID).ConfigureAwait(true);
		}

		public static async Task<int> GetFactionNumberOfMembers(EntityDatabaseID a_FactionID)
		{
			CMySQLResult result = await m_MySQLInst.QueryGame("SELECT id FROM faction_memberships WHERE faction_id={0};", a_FactionID).ConfigureAwait(true);
			return result.NumRows();
		}

		public static async Task AddFactionMembership(EntityDatabaseID a_FactionID, EntityDatabaseID a_CharacterID, int a_Rank, bool a_bIsManager)
		{
			await m_MySQLInst.QueryGame("INSERT INTO faction_memberships(faction_id, character_id, rank_index, manager) VALUES({0}, {1}, {2}, {3});", a_FactionID, a_CharacterID, a_Rank, a_bIsManager).ConfigureAwait(true);
		}

		private static async Task<CDatabaseStructureFaction> LoadSingleFaction(EntityDatabaseID a_FactionID)
		{
			CDatabaseStructureFaction returnValue = null;
			CMySQLResult result = await m_MySQLInst.QueryGame("SELECT * FROM factions WHERE id={0};", a_FactionID).ConfigureAwait(true);

			if (result.NumRows() > 0)
			{
				returnValue = await LoadFactionFromDBRow(result.GetRow(0)).ConfigureAwait(true);

				// TODO_STABILITY: Error if faction already loaded, or remove old?
			}

			return returnValue;
		}

		private static async Task<CDatabaseStructureFaction> LoadFactionFromDBRow(CMySQLRow row)
		{
			CDatabaseStructureFaction faction = new CDatabaseStructureFaction(row);

			CMySQLResult rankRows = await m_MySQLInst.QueryGame("SELECT * FROM faction_ranks WHERE faction_id={0};", faction.factionID).ConfigureAwait(true);

			foreach (CMySQLRow rankRow in rankRows.GetRows())
			{
				EntityDatabaseID databaseID = rankRow.GetValue<EntityDatabaseID>("id");
				string strRankName = rankRow["name"];
				float fSalary = rankRow.GetValue<float>("salary");

				faction.lstFactionRanks.Add(new CFactionRank(databaseID, strRankName, fSalary));
			}

			return faction;
		}


		public static async Task<List<PendingFactionInvite>> GetCharacterPendingFactionInvites(EntityDatabaseID a_CharacterID)
		{
			List<PendingFactionInvite> lstFactionInvites = new List<PendingFactionInvite>();
			CMySQLResult inviteRows = await m_MySQLInst.QueryGame("SELECT * FROM faction_invites WHERE target_character={0};", a_CharacterID).ConfigureAwait(true);

			foreach (CMySQLRow inviteRow in inviteRows.GetRows())
			{
				PendingFactionInvite pendingInvite = new PendingFactionInvite();
				pendingInvite.SourceCharacter = inviteRow.GetValue<EntityDatabaseID>("source_character");
				pendingInvite.FactionID = inviteRow.GetValue<EntityDatabaseID>("faction");

				lstFactionInvites.Add(pendingInvite);
			}
			return lstFactionInvites;
		}

		public static async Task<string> GetUsernameFromAccount(EntityDatabaseID accountID)
		{
			CMySQLResult usernameTask = await m_MySQLInst.QueryAuth("SELECT username from accounts WHERE id={0} LIMIT 1;", accountID).ConfigureAwait(true);
			if (usernameTask.NumRows() > 0)
			{
				return usernameTask.GetRow(0)["username"];
			}

			return String.Empty;
		}

		public static async Task<CGetCharactersResult> GetCharacters(int AccountID)
		{
			// TODO_POST_LAUNCH: If dimension > 0 here, query property name and send that for 'last seen location'
			CGetCharactersResult returnValue = new CGetCharactersResult();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT id, name, x, y, z, rz, IFNULL(TIMESTAMPDIFF(HOUR, last_seen, CURRENT_TIMESTAMP), -1), cked from characters WHERE account={0} ORDER BY IFNULL(last_seen, NOW()) DESC;", AccountID).ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				GetCharactersCharacter newCharacter = new GetCharactersCharacter
				{
					id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
					name = row["name"],
					pos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"])),
					rz = float.Parse(row["rz"]),
					LastSeenHours = Convert.ToInt32(row["IFNULL(TIMESTAMPDIFF(HOUR, last_seen, CURRENT_TIMESTAMP), -1)"]),
					cked = bool.Parse(row["cked"])
				};

				returnValue.m_lstCharacters.Add(newCharacter);
			}

			return returnValue;
		}

		public static async Task<SVerifyCharacterExists> VerifyCharacterExists(string a_strName)
		{
			SVerifyCharacterExists returnValue = new SVerifyCharacterExists();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT id, account, name from characters WHERE LOWER(name)=LOWER('{0}');", a_strName).ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				CMySQLRow row = mysqlResult.GetRow(0);

				returnValue = new SVerifyCharacterExists
				{
					CharacterExists = true,
					AccountID = Convert.ToInt32(row["account"]),
					CharacterID = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
					CharacterNameClean = row["name"]
				};
			}

			return returnValue;
		}

		public static async Task<SGetFactionMembers> GetFactionMembers(EntityDatabaseID a_FactionID)
		{
			SGetFactionMembers result = new SGetFactionMembers();

			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT name, rank_index, manager, DATE_FORMAT(last_seen, \"%b %e at %l:%i %p\") AS seen FROM faction_memberships factionmems, characters chars WHERE factionmems.faction_id={0} AND chars.id = factionmems.character_id;", a_FactionID).ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				SGetFactionMemberInst member = new SGetFactionMemberInst(row["name"], Convert.ToInt32(row["rank_index"]), bool.Parse(row["manager"]), row["seen"]);
				result.lstMembers.Add(member);
			}

			return result;
		}

		public static async Task<EntityDatabaseID> CreateKeybind(EntityDatabaseID AccountID, EntityDatabaseID CharacterID, EPlayerKeyBindType bindType, string strAction, ConsoleKey key)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO keybinds(account, character_id, bind_type, bind_action, bind_key) VALUES({0}, {1}, {2}, '{3}', {4});", AccountID, CharacterID, bindType, strAction, key).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DeleteKeybind(EntityDatabaseID AccountID, EntityDatabaseID KeybindID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM keybinds WHERE account={0} and id={1} LIMIT 1;", AccountID, KeybindID).ConfigureAwait(true);
		}

		public static async Task SetCharacterCurrentVersionToLatest(EntityDatabaseID CharacterID, ECharacterVersions version)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET current_version={0} WHERE id={1};", (int)version, CharacterID).ConfigureAwait(true);
		}

		public static async Task<List<CItemInstanceDef>> LoadInventoryRecursiveAsync(EItemParentTypes itemParentType, EntityDatabaseID parentID)
		{
			List<CItemInstanceDef> lstInventory = new List<CItemInstanceDef>();

			CMySQLResult inventoryRows = await m_MySQLInst.QueryGame("SELECT id, item_id, item_value, current_socket, parent, stack_size FROM inventories WHERE parent_type={0} AND parent={1};", (int)itemParentType, parentID).ConfigureAwait(true);

			// Get inventory
			foreach (CMySQLRow itemRow in inventoryRows.GetRows())
			{
				EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
				EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
				string itemValue = itemRow["item_value"];
				EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
				EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
				UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

				// Is the item a container? Load its contents also
				CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemID];
				if (itemDef.IsContainer)
				{
					// Load the contents of this container
					List<CItemInstanceDef> lstContainerItems = await LoadInventoryRecursiveAsync(EItemParentTypes.Container, databaseID).ConfigureAwait(true);
					lstInventory.AddRange(lstContainerItems);
					// TODO: Should this be more hierarchical? rather than flat?
				}

				lstInventory.Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, itemParentType, StackSize));
			}

			return lstInventory;
		}

		public static async Task<CustomCharacterSkinData> GetCharacterCustomData(EntityDatabaseID CharacterID)
		{
			CustomCharacterSkinData returnValue = new CustomCharacterSkinData();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT ageing, ageing_opacity, makeup, makeup_opacity, makeup_color, makeup_color_highlight, blush, blush_opacity, blush_color, blush_color_highlight, complexion, complexion_opacity, sundamage, sundamage_opacity, lipstick, lipstick_opacity, lipstick_color, lipstick_color_highlights, moles_and_freckles, moles_and_freckles_opacity, nose_size_horizontal, nose_size_vertical, nose_size_outwards, nose_size_outwards_upper, nose_size_outwards_lower, nose_angle, eyebrow_height, eyebrow_depth, cheekbone_height, cheek_width, cheek_width_lower, eye_size, lip_size, mouth_size, mouth_size_lower, chin_size, chin_size_lower, chin_width, chin_effect, neck_width, neck_width_lower, face_blend_1_mother, face_blend_1_father, face_blend_father_percent, skin_blend_father_percent, base_hair, hair_style, hair_color, hair_color_highlight, eye_color, facial_hair_style, facial_hair_color, facial_hair_color_highlight, facial_hair_opacity, blemishes, blemishes_opacity, eyebrows, eyebrows_opacity, eyebrows_color, eyebrows_color_highlight, body_blemishes, body_blemishes_opacity, chest_hair, chest_hair_color, chest_hair_color_highlights, chest_hair_opacity, full_beard_style, full_beard_color FROM characters_custom_data WHERE char_id={0};", CharacterID).ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				CMySQLRow row = mysqlResult.GetRow(0);

				returnValue = new CustomCharacterSkinData(row.GetFields());
			}
			else
			{
				// TODO: Error
			}

			return returnValue;
		}

		public static async Task<List<int>> GetCharacterTattooData(EntityDatabaseID CharacterID)
		{
			List<int> returnValue = new List<int>();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT tattoo_id FROM characters_tattoos WHERE char_id={0};", CharacterID).ConfigureAwait(true);

			foreach (var mysqlRow in mysqlResult.GetRows())
			{
				returnValue.Add(Convert.ToInt32(mysqlRow["tattoo_id"]));
			}

			return returnValue;
		}

		public static async Task<TutorialCheckResult> HasAccountFinishedTutorial(int a_AccountID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT version FROM tutorial_state WHERE account_id={0} LIMIT 1;", a_AccountID).ConfigureAwait(true);
			if (mysqlResult.NumRows() > 0)
			{
				ETutorialVersions tutorialVersion = (ETutorialVersions)Convert.ToInt32(mysqlResult.GetRow(0)["version"]);

				if (tutorialVersion == TutorialConstants.TutorialVersion)
				{
					return new TutorialCheckResult(ETutorialCheckResult.CompletedLatestVersion, tutorialVersion);
				}
				else
				{
					return new TutorialCheckResult(ETutorialCheckResult.CompletedOldVersion, tutorialVersion);
				}
			}

			return new TutorialCheckResult(ETutorialCheckResult.NotComplete, ETutorialVersions.None);
		}

		public static async Task<EntityDatabaseID> CreateGangTag(EntityDatabaseID a_CharacterID, float x, float y, float z, float a_fRotZ, Dimension a_Dimension, List<GangTagLayer> lstLayers)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO gangtags(owner_char, x, y, z, rz, dim, tagdata, progress) VALUES({0}, {1}, {2}, {3}, {4}, {5}, '{6}', 0.0);"
				, a_CharacterID, x, y, z, a_fRotZ, a_Dimension, JsonConvert.SerializeObject(lstLayers)).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DestroyGangTag(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM gangtags WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task<List<CDatabaseStructureGangTag>> LoadAllGangTags()
		{
			List<CDatabaseStructureGangTag> lstGangTags = new List<CDatabaseStructureGangTag>();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM gangtags;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstGangTags.Add(new CDatabaseStructureGangTag(row));
			}

			return lstGangTags;
		}

		public static async Task DeletePendingFactionInvite(EntityDatabaseID a_CharacterID, EntityDatabaseID a_FactionID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM faction_invites WHERE target_character={0} and faction={1} LIMIT 1;", a_CharacterID, a_FactionID).ConfigureAwait(true);
		}

		public static async Task DestroyVehicle(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM vehicles WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateDutyPoint(EDutyType a_DutyType, Vector3 a_vecPos, uint a_Dimension)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO duty_points (type, x, y, z, dimension) VALUES({0}, {1}, {2}, {3}, {4});", a_DutyType, a_vecPos.X, a_vecPos.Y, a_vecPos.Z, a_Dimension).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DestroyDutyPoint(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM duty_points WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateFuelPoint(Vector3 a_vecPos, uint a_Dimension)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO fuel_points (x, y, z, dimension) VALUES({0}, {1}, {2}, {3});", a_vecPos.X, a_vecPos.Y, a_vecPos.Z, a_Dimension).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DestroyFuelPoint(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM fuel_points WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateBank(Vector3 a_vecPos, float fRot, EBankSystemType bankType, uint a_Dimension)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO banks (x, y, z, rz, type, dimension) VALUES({0}, {1}, {2}, {3}, {4}, {5});", a_vecPos.X, a_vecPos.Y, a_vecPos.Z, fRot, bankType, a_Dimension).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DestroyBank(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM banks WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateDancer(Vector3 a_vector3, float fRot, uint dancerSkin, bool bAllowTip, uint dimension, EntityDatabaseID parentPropertyID, string animDict, string animName)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO dancers (x, y, z, rz, dimension, skin, parent_property, allow_tip, tip_money, anim_dict, anim_name) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {9}, 0, '{7}', '{8}');", a_vector3.X, a_vector3.Y, a_vector3.Z, fRot, dimension, dancerSkin, parentPropertyID, animDict, animName, bAllowTip).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DestroyDancer(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM dancers WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task TipDancer(EntityDatabaseID a_DatabaseID, float newTipMoneyTotal)
		{
			await m_MySQLInst.QueryGame("UPDATE dancers SET tip_money={1} WHERE id={0}", a_DatabaseID, newTipMoneyTotal).ConfigureAwait(true);
		}

		public static async Task ResetDancerTipMoney(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("UPDATE dancers SET tip_money=0 WHERE id={0}", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateStore(Vector3 a_vecPos, float fRot, EStoreType storeType, uint dimension, EntityDatabaseID parentPropertyID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO stores (x, y, z, rz, type, dimension, parent_property) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6});", a_vecPos.X, a_vecPos.Y, a_vecPos.Z, fRot, storeType, dimension, parentPropertyID).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async void SetStoreLastRobbedAt(EntityDatabaseID storeID, Int64 timestamp)
		{
			await m_MySQLInst.QueryGame("UPDATE stores SET last_robbed_at = {0} WHERE id = {1}", timestamp, storeID).ConfigureAwait(true);
		}

		public static async Task DestroyStore(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM stores WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateWorldBlip(string strName, int Sprite, int Color, Vector3 vecPos)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO world_blips (name, sprite, color, x, y, z) VALUES('{0}', {1}, {2}, {3}, {4}, {5});", strName, Sprite, Color, vecPos.X, vecPos.Y, vecPos.Z).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DestroyWorldBlip(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM world_blips WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task DestroyCarWashPoint(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM carwash_points WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task DestroyVehicleRepairPoint(EntityDatabaseID a_DatabaseID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM vehiclerepair_points WHERE id={0} LIMIT 1;", a_DatabaseID).ConfigureAwait(true);
		}

		public static async Task DestroyItems(CItemInstanceDef a_ItemInstance)
		{
			await m_MySQLInst.QueryGame("DELETE FROM inventories WHERE item_id={0} and item_value=\"{1}\";", a_ItemInstance.ItemID, a_ItemInstance.GetValueDataSerialized()).ConfigureAwait(true);
		}

		public static async Task UpdateAdminReportCount(EntityDatabaseID a_AccountID)
		{
			await m_MySQLInst.QueryGame("UPDATE game_accounts SET admin_report_count=admin_report_count+1 WHERE account_id={0}", a_AccountID).ConfigureAwait(true);
		}

		public static async Task ToggleLocalPlayerNametag(EntityDatabaseID a_AccountID, bool bToggled)
		{
			await m_MySQLInst.QueryGame("UPDATE game_accounts SET local_nametag_toggled={1} WHERE account_id={0}", a_AccountID, bToggled).ConfigureAwait(true);
		}

		public static async Task SetAutoSpawnCharacter(EntityDatabaseID a_accountID, EntityDatabaseID a_characterID)
		{
			await m_MySQLInst.QueryGame("UPDATE game_accounts SET auto_spawn_character={0} WHERE account_id={1};", a_characterID, a_accountID).ConfigureAwait(true);
		}

		public static async Task ConsumeShowSpawnSelector(EntityDatabaseID a_databaseID)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET show_spawnselector={0} WHERE id={1};", false, a_databaseID).ConfigureAwait(true);
		}

		public static async Task ConsumeFirstUse(EntityDatabaseID a_databaseID)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET first_use={0} WHERE id={1};", false, a_databaseID).ConfigureAwait(true);
		}

		public static async Task SetCharacterMoney(EntityDatabaseID a_databaseID, float a_fMoney)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET money={0} WHERE id={1};", a_fMoney, a_databaseID).ConfigureAwait(true);
		}

		public static async Task SetCharacterBankMoney(EntityDatabaseID a_databaseID, float a_fMoney)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET bank_money={0} WHERE id={1};", a_fMoney, a_databaseID).ConfigureAwait(true);
		}

		public static async Task SetCharacterName(EntityDatabaseID a_databaseID, string a_strName)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET name='{0}' WHERE id={1};", a_strName, a_databaseID).ConfigureAwait(true);
		}

		public static async Task SetCharacterPremadeMasked(EntityDatabaseID a_databaseID, bool bPremadeMasked)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET premade_masked={0} WHERE id={1};", bPremadeMasked, a_databaseID).ConfigureAwait(true);
		}

		public static async Task SetVehiclePlateText(EntityDatabaseID a_VehicleID, string a_strPlateText)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET plate_text='{0}' WHERE id={1};", a_strPlateText, a_VehicleID).ConfigureAwait(true);
		}

		public static async Task SetVehicleNeonsState(EntityDatabaseID a_VehicleID, bool bEnabled, int r, int g, int b)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET neons={0}, neon_r={1}, neon_g={2}, neon_b={3} WHERE id={4};", bEnabled, r, g, b, a_VehicleID).ConfigureAwait(true);
		}

		public static async Task SetVehicleOwner(EntityDatabaseID a_VehicleID, EVehicleType a_vehicleType, EntityDatabaseID a_vehOwner)
		{
			await m_MySQLInst.QueryGame("UPDATE vehicles SET owner={0}, type={1} WHERE id={2};", a_vehOwner, (int)a_vehicleType, a_VehicleID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateVehicle(EVehicleType a_VehicleType, EntityDatabaseID a_OwnerID, uint a_Model, Vector3 a_vecDefaultSpawnPos, Vector3 a_vecDefaultSpawnRot, Vector3 a_Pos, Vector3 a_vecRot, EPlateType a_PlateType, string a_strPlateText, float a_fFuel, int a_colPrimaryR, int a_colPrimaryG, int a_colPrimaryB, int a_colSecondaryR, int a_colSecondaryG, int a_colSecondaryB, int a_ColWheels, int a_Livery, float a_fDirt, float a_fHealth, bool a_bLocked, bool a_bEngineOn, int a_NumPaymentsRemaining, int a_NumPaymentsMade, float a_fCreditAmount, Int64 a_ExpiryTime, float a_fOdometer, Dimension a_Dimension, Int64 last_used, EVehicleTransmissionType transmission, int pearlescent_color, bool is_token_vehicle)
		{
			var mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO vehicles (type, owner, model, spawn_x, spawn_y, spawn_z, spawn_rx, spawn_ry, spawn_rz, plate_type, plate_text, fuel, color1_r, color1_g, color1_b, color2_r, color2_g, color2_b, color_wheel, livery, dirt, health, x, y, z, rx, ry, rz, locked, engine, payments_remaining, payments_made, payments_missed, credit_amount, expiry_time, odometer, dimension, last_used, transmission,  is_token_purchase, pearlescent_color) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, '{10}', {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, 0, {32}, {33}, {34}, {35}, {36}, {37}, {38}, {39});"
				, (int)a_VehicleType, a_OwnerID, a_Model, a_vecDefaultSpawnPos.X, a_vecDefaultSpawnPos.Y, a_vecDefaultSpawnPos.Z, a_vecDefaultSpawnRot.X, a_vecDefaultSpawnRot.Y, a_vecDefaultSpawnRot.Z, (int)a_PlateType, a_strPlateText, a_fFuel, a_colPrimaryR, a_colPrimaryG, a_colPrimaryB, a_colSecondaryR, a_colSecondaryG, a_colSecondaryB, a_ColWheels, a_Livery, a_fDirt, a_fHealth, a_Pos.X, a_Pos.Y, a_Pos.Z, a_vecRot.X, a_vecRot.Y, a_vecRot.Z, a_bLocked, a_bEngineOn, a_NumPaymentsRemaining, a_NumPaymentsMade, a_fCreditAmount, a_ExpiryTime, a_fOdometer, a_Dimension, last_used, transmission, is_token_vehicle, pearlescent_color).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task<CMdtVehicle> GetVehicleByPlate(EVehicleType a_VehicleType, string plate)
		{
			const string strBasicQuery = "SELECT * FROM vehicles WHERE LOWER(plate_text)=LOWER('{0}') LIMIT 1;";
			string strOwnerName = null;
			string strQuery = "";
			if (a_VehicleType == EVehicleType.PlayerOwned || a_VehicleType == EVehicleType.RentalCar)
			{
				strQuery = "SELECT vehicles.*, characters.name as owner_name from vehicles JOIN characters ON characters.id = vehicles.owner WHERE LOWER(plate_text)=LOWER('{0}') LIMIT 1;";
			}
			else if (a_VehicleType == EVehicleType.FactionOwned || a_VehicleType == EVehicleType.FactionOwnedRental)
			{
				strQuery = "SELECT vehicles.*, factions.name as owner_name from vehicles JOIN factions ON factions.id = vehicles.owner WHERE LOWER(plate_text)=LOWER('{0}') LIMIT 1;";
			}
			else if (a_VehicleType == EVehicleType.Civilian)
			{
				strOwnerName = "Civilian/State";
				strQuery = strBasicQuery;
			}
			if (a_VehicleType == EVehicleType.Temporary)
			{
				strOwnerName = "Admin/Temporary";
				strQuery = strBasicQuery;
			}
			else if (a_VehicleType == EVehicleType.TruckerJob)
			{
				strOwnerName = "SA Trucking";
				strQuery = strBasicQuery;
			}
			else if (a_VehicleType == EVehicleType.DeliveryDriverJob)
			{
				strOwnerName = "SA Deliveries";
				strQuery = strBasicQuery;
			}
			else if (a_VehicleType == EVehicleType.BusDriverJob)
			{
				strOwnerName = "SA Transit";
				strQuery = strBasicQuery;
			}
			else if (a_VehicleType == EVehicleType.MailmanJob)
			{
				strOwnerName = "SA PostOp Mail";
				strQuery = strBasicQuery;
			}
			else if (a_VehicleType == EVehicleType.TrashmanJob)
			{
				strOwnerName = "SA Refuse";
				strQuery = strBasicQuery;
			}
			else if (a_VehicleType == EVehicleType.TaxiJob)
			{
				strOwnerName = "SA Taxis";
				strQuery = strBasicQuery;
			}
			else if (a_VehicleType == EVehicleType.DrivingTest_Bike
				|| a_VehicleType == EVehicleType.DrivingTest_Car
				|| a_VehicleType == EVehicleType.DrivingTest_Truck)
			{
				strOwnerName = "DMV";
				strQuery = strBasicQuery;
			}

			if (strQuery == null)
			{
				CMdtVehicle result = new CMdtVehicle
				{
					found = false
				};

				return result;
			}

			CMdtVehicle returnValue = new CMdtVehicle();
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame(strQuery, plate).ConfigureAwait(true);
			if (mysqlResult.NumRows() == 0)
			{
				CMdtVehicle result = new CMdtVehicle
				{
					found = false
				};

				return result;
			}

			CMySQLRow row = mysqlResult.GetRow(0);

			if (strOwnerName == null) // this means we actually queried the owner
			{
				strOwnerName = row["owner_name"];

				if (a_VehicleType == EVehicleType.FactionOwnedRental || a_VehicleType == EVehicleType.RentalCar)
				{
					strOwnerName += " (Rental)";
				}
			}

			returnValue = new CMdtVehicle
			{
				found = true,
				id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
				owner = int.Parse(row["owner"]),
				owner_name = strOwnerName,
				model = uint.Parse(row["model"]),
				plate_type = int.Parse(row["plate_type"]),
				plate_text = row["plate_text"],
				color1_r = int.Parse(row["color1_r"]),
				color1_g = int.Parse(row["color1_g"]),
				color1_b = int.Parse(row["color1_b"]),
				color2_r = int.Parse(row["color2_r"]),
				color2_g = int.Parse(row["color2_g"]),
				color2_b = int.Parse(row["color2_b"]),
				livery = int.Parse(row["livery"])
			};

			return returnValue;
		}

		public static async Task<CMdtProperty> GetPropertyByZip(EPropertyState a_State, EPropertyOwnerType ownerType, string zip)
		{
			// TODO: This needs more work when we have renters
			string strQuery = null;

			if (ownerType == EPropertyOwnerType.Player)
			{
				strQuery = "SELECT properties.*, IFNULL(c_owner.name, '') as owner_name, IFNULL(c_renter.name, '') as renter_name FROM properties left join characters c_owner ON c_owner.id = properties.owner left join characters c_renter ON c_renter.id = properties.renter WHERE properties.id = '{0}' LIMIT 1; ";
			}
			else
			{
				strQuery = "SELECT properties.*, IFNULL(c_owner.name, '') as owner_name, IFNULL(c_renter.name, '') as renter_name FROM properties left join factions c_owner ON c_owner.id = properties.owner left join factions c_renter ON c_renter.id = properties.renter WHERE properties.id = '{0}' LIMIT 1; ";
			}

			if (strQuery == null)
			{
				CMdtProperty result = new CMdtProperty
				{
					found = false
				};

				return result;
			}

			CMdtProperty returnValue = new CMdtProperty();
			var mysqlResult = await m_MySQLInst.QueryGame(strQuery, zip).ConfigureAwait(true);
			if (mysqlResult.NumRows() == 0)
			{
				CMdtProperty result = new CMdtProperty
				{
					found = false
				};

				return result;
			}

			CMySQLRow row = mysqlResult.GetRow(0);
			returnValue = new CMdtProperty
			{
				found = true,
				id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
				name = row["name"],
				owner = int.TryParse(row["owner"], out var tempOwner) ? tempOwner : (int?)null,
				owner_name = row["owner_name"],
				renter = int.TryParse(row["renter"], out var tempRenter) ? tempRenter : (int?)null,
				renter_name = row["renter_name"],
				entrance_x = float.Parse(row["entrance_x"]),
				entrance_y = float.Parse(row["entrance_y"]),
				entrance_z = float.Parse(row["entrance_z"]),
				entrance_dimension = float.Parse(row["entrance_dimension"]),
				buy_price = float.Parse(row["buy_price"]),
				rent_price = float.Parse(row["rent_price"])
			};

			return returnValue;
		}

		public static async Task SavePlasticSurgeonData(EntityDatabaseID characterID,
			int Ageing,
			float AgeingOpacity,
			int Makeup,
			float MakeupOpacity,
			int MakeupColor,
			int MakeupColorHighlight,
			int Blush,
			float BlushOpacity,
			int BlushColor,
			int BlushColorHighlight,
			int Complexion,
			float ComplexionOpacity,
			int SunDamage,
			float SunDamageOpacity,
			int Lipstick,
			float LipstickOpacity,
			int LipstickColor,
			int LipstickColorHighlights,
			int MolesAndFreckles,
			float MolesAndFrecklesOpacity,
			float NoseSizeHorizontal,
			float NoseSizeVertical,
			float NoseSizeOutwards,
			float NoseSizeOutwardsUpper,
			float NoseSizeOutwardsLower,
			float NoseAngle,
			float EyebrowHeight,
			float EyebrowDepth,
			float CheekboneHeight,
			float CheekWidth,
			float CheekWidthLower,
			float EyeSize,
			float LipSize,
			float MouthSize,
			float MouthSizeLower,
			float ChinSize,
			float ChinSizeLower,
			float ChinWidth,
			float ChinEffect,
			float NeckWidth,
			float NeckWidthLower,
			int FaceBlend1Mother,
			int FaceBlend1Father,
			float FaceBlendFatherPercent,
			float SkinBlendFatherPercent,
			int EyeColor,
			int Blemishes,
			float BlemishesOpacity,
			int Eyebrows,
			float EyebrowsOpacity,
			int EyebrowsColor,
			int EyebrowsColorHighlight,
			int BodyBlemishes,
			float BodyBlemishesOpacity)
		{
			await m_MySQLInst.QueryGame("UPDATE characters_custom_data SET ageing={1}, ageing_opacity={2}, makeup={3}, makeup_opacity={4}, makeup_color={5}, makeup_color_highlight={6}, blush={7}, blush_opacity={8}, blush_color={9}, blush_color_highlight={10}, complexion={11}, complexion_opacity={12}, sundamage={13}, sundamage_opacity={14}, lipstick={15}, lipstick_opacity={16}, lipstick_color={17}, lipstick_color_highlights={18}, moles_and_freckles={19}, moles_and_freckles_opacity={20}, nose_size_horizontal={21}, nose_size_vertical={22}, nose_size_outwards={23}, nose_size_outwards_upper={24}, nose_size_outwards_lower={25}, nose_angle={26}, eyebrow_height={27}, eyebrow_depth={28}, cheekbone_height={29}, cheek_width={30}, cheek_width_lower={31}, eye_size={32}, lip_size={33}, mouth_size={34}, mouth_size_lower={35}, chin_size={36}, chin_size_lower={37}, chin_width={38}, chin_effect={39}, neck_width={40}, neck_width_lower={41}, face_blend_1_mother={42}, face_blend_1_father={43}, face_blend_father_percent={44}, skin_blend_father_percent={45}, eye_color={46}, blemishes={47}, blemishes_opacity={48}, eyebrows={49}, eyebrows_opacity={50}, eyebrows_color={51}, eyebrows_color_highlight={52}, body_blemishes={53}, body_blemishes_opacity={54} WHERE char_id={0};",
					characterID,
					Ageing,
					AgeingOpacity,
					Makeup,
					MakeupOpacity,
					MakeupColor,
					MakeupColorHighlight,
					Blush,
					BlushOpacity,
					BlushColor,
					BlushColorHighlight,
					Complexion,
					ComplexionOpacity,
					SunDamage,
					SunDamageOpacity,
					Lipstick,
					LipstickOpacity,
					LipstickColor,
					LipstickColorHighlights,
					MolesAndFreckles,
					MolesAndFrecklesOpacity,
					NoseSizeHorizontal,
					NoseSizeVertical,
					NoseSizeOutwards,
					NoseSizeOutwardsUpper,
					NoseSizeOutwardsLower,
					NoseAngle,
					EyebrowHeight,
					EyebrowDepth,
					CheekboneHeight,
					CheekWidth,
					CheekWidthLower,
					EyeSize,
					LipSize,
					MouthSize,
					MouthSizeLower,
					ChinSize,
					ChinSizeLower,
					ChinWidth,
					ChinEffect,
					NeckWidth,
					NeckWidthLower,
					FaceBlend1Mother,
					FaceBlend1Father,
					FaceBlendFatherPercent,
					SkinBlendFatherPercent,
					EyeColor,
					Blemishes,
					BlemishesOpacity,
					Eyebrows,
					EyebrowsOpacity,
					EyebrowsColor,
					EyebrowsColorHighlight,
					BodyBlemishes,
					BodyBlemishesOpacity
						).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateCharacterCustom(Vector3 vecSpawnPos, float fSpawnRot, EGender gender, string strName, uint skinHash, int Age, int creatorAccountID,
			int Ageing,
			float AgeingOpacity,
			int Makeup,
			float MakeupOpacity,
			int MakeupColor,
			int MakeupColorHighlight,
			int Blush,
			float BlushOpacity,
			int BlushColor,
			int BlushColorHighlight,
			int Complexion,
			float ComplexionOpacity,
			int SunDamage,
			float SunDamageOpacity,
			int Lipstick,
			float LipstickOpacity,
			int LipstickColor,
			int LipstickColorHighlights,
			int MolesAndFreckles,
			float MolesAndFrecklesOpacity,
			float NoseSizeHorizontal,
			float NoseSizeVertical,
			float NoseSizeOutwards,
			float NoseSizeOutwardsUpper,
			float NoseSizeOutwardsLower,
			float NoseAngle,
			float EyebrowHeight,
			float EyebrowDepth,
			float CheekboneHeight,
			float CheekWidth,
			float CheekWidthLower,
			float EyeSize,
			float LipSize,
			float MouthSize,
			float MouthSizeLower,
			float ChinSize,
			float ChinSizeLower,
			float ChinWidth,
			float ChinEffect,
			float NeckWidth,
			float NeckWidthLower,
			int FaceBlend1Mother,
			int FaceBlend1Father,
			float FaceBlendFatherPercent,
			float SkinBlendFatherPercent,
			int BaseHair,
			int HairStyle,
			int HairColor,
			int HairColorHighlights,
			int EyeColor,
			int FacialHairStyle,
			int FacialHairColor,
			int FacialHairColorHighlight,
			float FacialHairOpacity,
			int Blemishes,
			float BlemishesOpacity,
			int Eyebrows,
			float EyebrowsOpacity,
			int EyebrowsColor,
			int EyebrowsColorHighlight,
			List<int> lstTattooIDs,
			int BodyBlemishes,
			float BodyBlemishesOpacity,
			int ChestHair,
			int ChestHairColor,
			int ChestHairColorHighlights,
			float ChestHairOpacity,
			int BeardStyle,
			int BeardTexture,
			ECharacterSource characterSource)
		{
			// TODO_POST_LAUNCH: confirm name valid + not taken here? Already done by callers in account system. Probably won't be used elsewhere?
			EntityDatabaseID InsertID = -1;

			CMySQLResult result = await m_MySQLInst.QueryGame("INSERT INTO characters (name, account, gender, age, jail_reason, type, x, y, z, rz, creation_version, current_version, source) VALUES('{0}', {1}, {2}, {3}, '', {4}, {5}, {6}, {7}, {8}, {9}, {9}, {10});", strName, creatorAccountID, gender, Age, ECharacterType.Custom, vecSpawnPos.X, vecSpawnPos.Y, vecSpawnPos.Z, fSpawnRot, (int)CharacterConstants.LatestCharacterVersion, characterSource).ConfigureAwait(true); ;
			InsertID = (EntityDatabaseID)result.GetInsertID();
			await m_MySQLInst.QueryGame("INSERT INTO characters_custom_data (char_id, ageing, ageing_opacity, makeup, makeup_opacity, makeup_color, makeup_color_highlight, blush, blush_opacity, blush_color, blush_color_highlight, complexion, complexion_opacity, sundamage, sundamage_opacity, lipstick, lipstick_opacity, lipstick_color, lipstick_color_highlights, moles_and_freckles, moles_and_freckles_opacity, nose_size_horizontal, nose_size_vertical, nose_size_outwards, nose_size_outwards_upper, nose_size_outwards_lower, nose_angle, eyebrow_height, eyebrow_depth, cheekbone_height, cheek_width, cheek_width_lower, eye_size, lip_size, mouth_size, mouth_size_lower, chin_size, chin_size_lower, chin_width, chin_effect, neck_width, neck_width_lower, face_blend_1_mother, face_blend_1_father, face_blend_father_percent, skin_blend_father_percent, base_hair, hair_style, hair_color, hair_color_highlight, eye_color, facial_hair_style, facial_hair_color, facial_hair_color_highlight, facial_hair_opacity, blemishes, blemishes_opacity, eyebrows, eyebrows_opacity, eyebrows_color, eyebrows_color_highlight, body_blemishes, body_blemishes_opacity, chest_hair, chest_hair_color, chest_hair_color_highlights, chest_hair_opacity, full_beard_style, full_beard_color)" +
					"VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, {37}, {38}, {39}, {40}, {41}, {42}, {43}, {44}, {45}, {46}, {47}, {48}, {49}, {50}, {51}, {52}, {53}, {54}, {55}, {56}, {57}, {58}, {59}, {60}, {61}, {62}, {63}, {64}, {65}, {66}, {67}, {68});",
					InsertID,
					Ageing,
					AgeingOpacity,
					Makeup,
					MakeupOpacity,
					MakeupColor,
					MakeupColorHighlight,
					Blush,
					BlushOpacity,
					BlushColor,
					BlushColorHighlight,
					Complexion,
					ComplexionOpacity,
					SunDamage,
					SunDamageOpacity,
					Lipstick,
					LipstickOpacity,
					LipstickColor,
					LipstickColorHighlights,
					MolesAndFreckles,
					MolesAndFrecklesOpacity,
					NoseSizeHorizontal,
					NoseSizeVertical,
					NoseSizeOutwards,
					NoseSizeOutwardsUpper,
					NoseSizeOutwardsLower,
					NoseAngle,
					EyebrowHeight,
					EyebrowDepth,
					CheekboneHeight,
					CheekWidth,
					CheekWidthLower,
					EyeSize,
					LipSize,
					MouthSize,
					MouthSizeLower,
					ChinSize,
					ChinSizeLower,
					ChinWidth,
					ChinEffect,
					NeckWidth,
					NeckWidthLower,
					FaceBlend1Mother,
					FaceBlend1Father,
					FaceBlendFatherPercent,
					SkinBlendFatherPercent,
					BaseHair,
					HairStyle,
					HairColor,
					HairColorHighlights,
					EyeColor,
					FacialHairStyle,
					FacialHairColor,
					FacialHairColorHighlight,
					FacialHairOpacity,
					Blemishes,
					BlemishesOpacity,
					Eyebrows,
					EyebrowsOpacity,
					EyebrowsColor,
					EyebrowsColorHighlight,
					BodyBlemishes,
					BodyBlemishesOpacity,
					ChestHair,
					ChestHairColor,
					ChestHairColorHighlights,
					ChestHairOpacity,
					BeardStyle,
					BeardTexture,
					characterSource
						).ConfigureAwait(true);

			// Tattoos
			foreach (int tattooID in lstTattooIDs)
			{
				await m_MySQLInst.QueryGame("INSERT INTO characters_tattoos (char_id, tattoo_id) VALUES({0}, {1});", InsertID, tattooID).ConfigureAwait(true);
			}

			return InsertID;
		}

		public static async Task<EntityDatabaseID> CreateCharacterPremade(Vector3 vecSpawnPos, float fSpawnRot, EGender gender, string strName, uint skinHash, int Age, int creatorAccountID, ECharacterSource characterSource)
		{
			// TODO_POST_LAUNCH: confirm name valid + not taken here? Already done by callers in account system. Probably won't be used elsewhere?
			EntityDatabaseID InsertID = -1;

			// DUMMY VARS PURELY FOR INSERTION SANITY
			int Ageing = 0;
			float AgeingOpacity = 0.0f;
			int Makeup = 0;
			float MakeupOpacity = 0.0f;
			int MakeupColor = 0;
			int MakeupColorHighlight = 0;
			int Blush = 0;
			float BlushOpacity = 0.0f;
			int BlushColor = 0;
			int BlushColorHighlight = 0;
			int Complexion = 0;
			float ComplexionOpacity = 0.0f;
			int SunDamage = 0;
			float SunDamageOpacity = 0.0f;
			int Lipstick = 0;
			float LipstickOpacity = 0.0f;
			int LipstickColor = 0;
			int LipstickColorHighlights = 0;
			int MolesAndFreckles = 0;
			float MolesAndFrecklesOpacity = 0.0f;
			float NoseSizeHorizontal = 0;
			float NoseSizeVertical = 0;
			float NoseSizeOutwards = 0;
			float NoseSizeOutwardsUpper = 0;
			float NoseSizeOutwardsLower = 0;
			float NoseAngle = 0;
			float EyebrowHeight = 0;
			float EyebrowDepth = 0;
			float CheekboneHeight = 0;
			float CheekWidth = 0;
			float CheekWidthLower = 0;
			float EyeSize = 0;
			float LipSize = 0;
			float MouthSize = 0;
			float MouthSizeLower = 0;
			float ChinSize = 0;
			float ChinSizeLower = 0;
			float ChinWidth = 0;
			float ChinEffect = 0;
			float NeckWidth = 0;
			float NeckWidthLower = 0;
			int FaceBlend1Mother = 0;
			int FaceBlend1Father = 0;
			float FaceBlendFatherPercent = 0.0f;
			float SkinBlendFatherPercent = 0.0f;
			int HairStyle = 0;
			int HairColor = 0;
			int HairColorHighlights = 0;
			int EyeColor = 0;
			int FacialHairStyle = 0;
			int FacialHairColor = 0;
			int FacialHairColorHighlight = 0;
			float FacialHairOpacity = 0.0f;
			int Blemishes = 0;
			float BlemishesOpacity = 0.0f;
			int Eyebrows = 0;
			float EyebrowsOpacity = 0.0f;
			int EyebrowsColor = 0;
			int EyebrowsColorHighlight = 0;
			int BodyBlemishes = 0;
			float BodyBlemishesOpacity = 0.0f;
			int ChestHair = 0;
			int ChestHairColor = 0;
			int ChestHairColorHighlights = 0;
			float ChestHairOpacity = 0.0f;
			// END DUMMY VARS


			CMySQLResult result = await m_MySQLInst.QueryGame("INSERT INTO characters (name, account, gender, age, jail_reason, type, x, y, z, rz, creation_version, current_version, source) VALUES('{0}', {1}, {2}, {3}, '', {4}, {5}, {6}, {7}, {8}, {9}, {9}, {10});", strName, creatorAccountID, gender, Age, ECharacterType.Premade, vecSpawnPos.X, vecSpawnPos.Y, vecSpawnPos.Z, fSpawnRot, (int)CharacterConstants.LatestCharacterVersion, characterSource).ConfigureAwait(true);
			InsertID = (EntityDatabaseID)result.GetInsertID();
			await m_MySQLInst.QueryGame("INSERT INTO characters_custom_data (char_id, ageing, ageing_opacity, makeup, makeup_opacity, makeup_color, makeup_color_highlight, blush, blush_opacity, blush_color, blush_color_highlight, complexion, complexion_opacity, sundamage, sundamage_opacity, lipstick, lipstick_opacity, lipstick_color, lipstick_color_highlights, moles_and_freckles, moles_and_freckles_opacity, nose_size_horizontal, nose_size_vertical, nose_size_outwards, nose_size_outwards_upper, nose_size_outwards_lower, nose_angle, eyebrow_height, eyebrow_depth, cheekbone_height, cheek_width, cheek_width_lower, eye_size, lip_size, mouth_size, mouth_size_lower, chin_size, chin_size_lower, chin_width, chin_effect, neck_width, neck_width_lower, face_blend_1_mother, face_blend_1_father, face_blend_father_percent, skin_blend_father_percent, hair_style, hair_color, hair_color_highlight, eye_color, facial_hair_style, facial_hair_color, facial_hair_color_highlight, facial_hair_opacity, blemishes, blemishes_opacity, eyebrows, eyebrows_opacity, eyebrows_color, eyebrows_color_highlight, body_blemishes, body_blemishes_opacity, chest_hair, chest_hair_color, chest_hair_color_highlights, chest_hair_opacity)" +
					 "VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, {37}, {38}, {39}, {40}, {41}, {42}, {43}, {44}, {45}, {46}, {47}, {48}, {49}, {50}, {51}, {52}, {53}, {54}, {55}, {56}, {57}, {58}, {59}, {60}, {61}, {62}, {63}, {64}, {65});",
					 InsertID,
					Ageing,
					AgeingOpacity,
					Makeup,
					MakeupOpacity,
					MakeupColor,
					MakeupColorHighlight,
					Blush,
					BlushOpacity,
					BlushColor,
					BlushColorHighlight,
					Complexion,
					ComplexionOpacity,
					SunDamage,
					SunDamageOpacity,
					Lipstick,
					LipstickOpacity,
					LipstickColor,
					LipstickColorHighlights,
					MolesAndFreckles,
					MolesAndFrecklesOpacity,
					NoseSizeHorizontal,
					NoseSizeVertical,
					NoseSizeOutwards,
					NoseSizeOutwardsUpper,
					NoseSizeOutwardsLower,
					NoseAngle,
					EyebrowHeight,
					EyebrowDepth,
					CheekboneHeight,
					CheekWidth,
					CheekWidthLower,
					EyeSize,
					LipSize,
					MouthSize,
					MouthSizeLower,
					ChinSize,
					ChinSizeLower,
					ChinWidth,
					ChinEffect,
					NeckWidth,
					NeckWidthLower,
					FaceBlend1Mother,
					FaceBlend1Father,
					FaceBlendFatherPercent,
					SkinBlendFatherPercent,
					HairStyle,
					HairColor,
					HairColorHighlights,
					EyeColor,
					FacialHairStyle,
					FacialHairColor,
					FacialHairColorHighlight,
					FacialHairOpacity,
					Blemishes,
					BlemishesOpacity,
					Eyebrows,
					EyebrowsOpacity,
					EyebrowsColor,
					EyebrowsColorHighlight,
					BodyBlemishes,
					BodyBlemishesOpacity,
					ChestHair,
					ChestHairColor,
					ChestHairColorHighlights,
					ChestHairOpacity
				).ConfigureAwait(true);

			return InsertID;
		}

		public static async Task<CStatsResult> GetCharacterStats(EntityDatabaseID CharacterID)
		{
			CStatsResult characterStats = new CStatsResult();

			CMySQLResult characterResult = await m_MySQLInst.QueryGame("SELECT name, age, gender, money, bank_money, job FROM characters WHERE id={0} LIMIT 1;", CharacterID).ConfigureAwait(true);
			if (characterResult.NumRows() == 0)
			{
				return characterStats;
			}
			CMySQLRow characterData = characterResult.GetRow(0);

			characterStats.found = true;
			characterStats.id = CharacterID;
			characterStats.name = characterData["name"];
			characterStats.age = int.Parse(characterData["age"]);
			characterStats.gender = int.Parse(characterData["gender"]);
			characterStats.money = float.Parse(characterData["money"]);
			characterStats.bank_money = float.Parse(characterData["bank_money"]);
			characterStats.job = int.Parse(characterData["job"]);

			CMySQLResult vehicleResult = await m_MySQLInst.QueryGame("SELECT id, model, plate_text FROM vehicles WHERE owner = {0} AND type = {1}", CharacterID, (int)EVehicleType.PlayerOwned).ConfigureAwait(true);

			foreach (CMySQLRow row in vehicleResult.GetRows())
			{
				characterStats.m_lstVehicles.Add(
					new SCharacterVehicleResult
					{
						id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
						model = uint.Parse(row["model"]),
						plate = row["plate_text"]
					}
				);
			}

			CMySQLResult propertyResult = await m_MySQLInst.QueryGame("SELECT id, name FROM properties WHERE owner = {0}  AND owner_type = {1}", CharacterID, (int)EPropertyOwnerType.Player).ConfigureAwait(true);

			foreach (CMySQLRow row in propertyResult.GetRows())
			{
				characterStats.m_lstProperties.Add(
					new SCharacterPropertyResult
					{
						id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
						name = row["name"]
					}
				);
			}

			return characterStats;
		}

		public static async Task<UInt32> GivePlayerDonationToken(EntityDatabaseID a_AccountID, EntityDatabaseID character_id, EDonationEffect a_TokenType)
		{
			// NOTE: This ASSUMES that EDonationEffect matches DB index for vehicle and property token in store...
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			var mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO donation_inventory(account, character_id, time_activated, time_expire, donation_id) VALUES({0}, {1}, {2}, {3}, {4});", a_AccountID, character_id, unixTimestamp, -2, a_TokenType).ConfigureAwait(true);
			return (UInt32)mysqlResult.GetInsertID();
		}

		public static async Task<UInt32> GivePlayerDonationItem(EntityDatabaseID a_AccountID, DonationPurchasable a_Purchasable)
		{
			var mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO donation_inventory(account, character_id, time_activated, time_expire, donation_id) VALUES({0}, {1}, {2}, {3}, {4});", a_AccountID, -1, -1, -1, a_Purchasable.ID).ConfigureAwait(true);
			return (UInt32)mysqlResult.GetInsertID();
		}

		public static async Task<UInt32> GiveInactivityDonationPerk(EntityDatabaseID a_AccountID, EntityDatabaseID a_CharacterID, Int64 time_activated, Int64 time_expire, EDonationInactivityPurchasables purchasableID, EntityDatabaseID a_TargetEntityID, EDonationInactivityEntityType a_TargetEntityType)
		{
			var mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO donation_inventory(account, character_id, time_activated, time_expire, donation_id, vehicle_id, property_id) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6});",
				a_AccountID, a_CharacterID, time_activated, time_expire, purchasableID,
				a_TargetEntityType == EDonationInactivityEntityType.Vehicle ? a_TargetEntityID : -1, a_TargetEntityType == EDonationInactivityEntityType.Property ? a_TargetEntityID : -1).ConfigureAwait(true);
			return (UInt32)mysqlResult.GetInsertID();
		}

		public static async Task UpdateInactivityDonationPerk(EntityDatabaseID a_AccountID, EntityDatabaseID a_CharacterID, Int64 time_expire, EDonationInactivityPurchasables purchasableID, EntityDatabaseID a_TargetEntityID, EDonationInactivityEntityType a_TargetEntityType)
		{
			if (a_TargetEntityType == EDonationInactivityEntityType.Property)
			{
				await m_MySQLInst.QueryGame("UPDATE donation_inventory SET time_expire={0} WHERE account={1} AND character_id={2} AND donation_id={3} AND property_id={4}", time_expire, a_AccountID, a_CharacterID, purchasableID, a_TargetEntityID).ConfigureAwait(true);
			}
			else if (a_TargetEntityType == EDonationInactivityEntityType.Vehicle)
			{
				await m_MySQLInst.QueryGame("UPDATE donation_inventory SET time_expire={0} WHERE account={1} AND character_id={2} AND donation_id={3} AND vehicle_id={4}", time_expire, a_AccountID, a_CharacterID, purchasableID, a_TargetEntityID).ConfigureAwait(true);
			}
		}

		public static async Task ActivateDonationItem(UInt32 dbid, EntityDatabaseID character_id, Int64 time_activated, Int64 time_expire)
		{
			await m_MySQLInst.QueryGame("UPDATE donation_inventory SET character_id={0}, time_activated={1}, time_expire={2} WHERE id={3};", character_id, time_activated, time_expire, dbid).ConfigureAwait(true);
		}

		public static async Task<CTeleportPlaces> GetTeleportPlaces()
		{
			CTeleportPlaces returnValue = new CTeleportPlaces();

			CMySQLResult propertyResult = await m_MySQLInst.QueryGame("SELECT * FROM teleport_places;").ConfigureAwait(true);
			foreach (CMySQLRow row in propertyResult.GetRows())
			{
				returnValue.places.Add(
					new STeleportPlace
					{
						found = true,
						id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
						name = row["name"],
						x = float.Parse(row["x"]),
						y = float.Parse(row["y"]),
						z = float.Parse(row["z"]),
						dimension = uint.Parse(row["dimension"]),
						admin_creator_id = int.Parse(row["admin_creator_id"])
					}
				);
			}

			return returnValue;
		}

		public static async Task<STeleportPlace> GetTeleportPlaceFromName(string name)
		{
			STeleportPlace returnValue = new STeleportPlace();
			CMySQLResult result = await m_MySQLInst.QueryGame("SELECT * FROM teleport_places WHERE LOWER(name) = LOWER('{0}') LIMIT 1;", name).ConfigureAwait(true);
			if (result.NumRows() == 0)
			{
				returnValue = new STeleportPlace
				{
					found = false,
				};
			}
			else
			{
				CMySQLRow row = result.GetRow(0);
				returnValue = new STeleportPlace
				{
					found = true,
					id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
					name = row["name"],
					x = float.Parse(row["x"]),
					y = float.Parse(row["y"]),
					z = float.Parse(row["z"]),
					dimension = uint.Parse(row["dimension"]),
					admin_creator_id = int.Parse(row["admin_creator_id"])
				};
			}

			return returnValue;
		}

		public static async void CreateTeleportLocation(string location, float x, float y, float z, uint dimension, int creatorAccountID)
		{
			await m_MySQLInst.QueryGame("INSERT INTO teleport_places (name, x, y, z, dimension, admin_creator_id) VALUES('{0}', {1}, {2}, {3}, {4}, {5});", location, x, y, z, dimension, creatorAccountID).ConfigureAwait(true);
		}

		public static async void DeleteTeleportLocation(string location)
		{
			await m_MySQLInst.QueryGame("DELETE FROM teleport_places WHERE name = '{0}'", location).ConfigureAwait(true);
		}

		public static async Task<Dictionary<string, string>> GetCharactersByAccount(string strEntry)
		{
			Dictionary<string, string> charactersList = new Dictionary<string, string>();
			string accountId = string.Empty;

			CMySQLResult accountResult = await m_MySQLInst.QueryAuth("SELECT `id` FROM `accounts` WHERE `username` = '{0}';", strEntry).ConfigureAwait(true);
			if (accountResult.NumRows() > 0)
			{
				accountId = accountResult.GetRow(0)["id"];
				charactersList.Add(strEntry, "");
			}

			if (!string.IsNullOrEmpty(accountId))
			{
				CMySQLResult charactersResult = await m_MySQLInst.QueryGame("SELECT `name`, `last_seen` FROM `characters` WHERE `account` = '{0}';", accountId).ConfigureAwait(true);
				foreach (var character in charactersResult.GetRows())
				{
					charactersList.Add(character["name"], character["last_seen"]);
				}
			}

			return charactersList;
		}

		public static async Task<Dictionary<string, string>> GetCharactersByName(string strEntry)
		{
			Dictionary<string, string> charactersList = new Dictionary<string, string>();
			string accountId = string.Empty;

			CMySQLResult accountResult = await m_MySQLInst.QueryGame("SELECT `account` FROM `characters` WHERE `name` = '{0}';", strEntry).ConfigureAwait(true);
			if (accountResult.NumRows() > 0)
			{
				accountId = accountResult.GetRow(0)["account"];
			}

			if (!string.IsNullOrEmpty(accountId))
			{
				CMySQLResult accountName = await m_MySQLInst.QueryAuth("SELECT `username` FROM `accounts` WHERE `id` = {0};", accountId).ConfigureAwait(true);
				if (accountName.NumRows() > 0)
				{
					charactersList.Add(accountName.GetRow(0)["username"], "");
				}

				CMySQLResult charactersResult = await m_MySQLInst.QueryGame("SELECT `name`, `last_seen` FROM `characters` WHERE `account` = '{0}';", accountId).ConfigureAwait(true);
				foreach (var character in charactersResult.GetRows())
				{
					charactersList.Add(character["name"], character["last_seen"]);
				}
			}

			return charactersList;
		}

		public static async Task<EntityDatabaseID> CreateElevator(Vector3 startPosition, float startRotation, uint startDimension, Vector3 endPosition, float endRotation, uint endDimension, bool isCarElevator, string elevatorName)
		{
			var mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO `elevators` (`entrance_x`, `entrance_y`, `entrance_z`, `exit_x`, `exit_y`, `exit_z`, `exit_dimension`, `entrance_dimension`, `car`, `entrance_rot`, `exit_rot`, `name`)" +
				" VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, '{9}', '{10}', '{11}');", startPosition.X, startPosition.Y, startPosition.Z, endPosition.X, endPosition.Y, endPosition.Z,
				endDimension, startDimension, isCarElevator, startRotation, endRotation, elevatorName).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task<CDatabaseStructureElevator> LoadElevator(EntityDatabaseID a_elevatorID)
		{
			CDatabaseStructureElevator retVal = null;
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM elevators WHERE id = {0} LIMIT 1;", a_elevatorID).ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				CMySQLRow row = mysqlResult.GetRow(0);
				retVal = new CDatabaseStructureElevator(row);
			}

			return retVal;
		}

		public static async Task<List<CDatabaseStructureElevator>> LoadAllElevators()
		{
			List<CDatabaseStructureElevator> lstProperties = new List<CDatabaseStructureElevator>();
			var mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM elevators;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstProperties.Add(new CDatabaseStructureElevator(row));
			}

			return lstProperties;
		}

		public static async Task DestroyElevator(EntityDatabaseID elevatorID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM elevators WHERE id={0} LIMIT 1;", elevatorID).ConfigureAwait(true);
		}

		public static async Task<EntityDatabaseID> CreateMetalDetector(Vector3 detectorPosition, float detectorRotation, uint dimension)
		{
			var mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO `metal_detectors` (`position_x`, `position_y`, `position_z`, `rotation`, `dimension`)" +
				" VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');", detectorPosition.X, detectorPosition.Y, detectorPosition.Z, detectorRotation, dimension).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task<CDatabaseStructureMetalDetector> LoadMetalDetector(EntityDatabaseID a_metalDetectorID)
		{
			CDatabaseStructureMetalDetector retVal = null;
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM metal_detectors WHERE id = {0} LIMIT 1;", a_metalDetectorID).ConfigureAwait(true);
			if (mysqlResult.NumRows() == 1)
			{
				CMySQLRow row = mysqlResult.GetRow(0);
				retVal = new CDatabaseStructureMetalDetector(row);
			}

			return retVal;
		}

		public static async Task<List<CDatabaseStructureMetalDetector>> LoadAllMetalDetectors()
		{
			List<CDatabaseStructureMetalDetector> lstMetalDetectors = new List<CDatabaseStructureMetalDetector>();
			var mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM metal_detectors;").ConfigureAwait(true);
			foreach (CMySQLRow row in mysqlResult.GetRows())
			{
				lstMetalDetectors.Add(new CDatabaseStructureMetalDetector(row));
			}

			return lstMetalDetectors;
		}

		public static async Task DestroyMetalDetector(EntityDatabaseID metalDetectorID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM metal_detectors WHERE id={0} LIMIT 1;", metalDetectorID).ConfigureAwait(true);
		}

		public static async Task<bool> HasGottenMarijuanaSeeds(long a_CharacterID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM characters WHERE id={0} AND marijuana_given=1 LIMIT 1;", a_CharacterID).ConfigureAwait(true);
			return mysqlResult.NumRows() > 0;
		}

		public static async Task SetGottenMarijuanaSeeds(long a_CharacterID, bool given = true)
		{
			await m_MySQLInst.QueryGame("UPDATE characters SET marijuana_given={1} WHERE id={0} LIMIT 1;", a_CharacterID, given).ConfigureAwait(true);
		}

		public static async Task<List<CDatabaseStructurePersistentNotification>> LoadAccountNotifications(EntityDatabaseID accountID)
		{
			await RemoveExpiredRadios().ConfigureAwait(true);
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("select id, account_id, title, click_event, body, CAST(created_at as unsigned) as created_at from notifications where account_id = {0}", accountID).ConfigureAwait(true);

			return mysqlResult.GetRows().Select(row => new CDatabaseStructurePersistentNotification(row)).ToList();
		}

		public static async Task<EntityDatabaseID> CreateAccountNotification(EntityDatabaseID accountID, string title, string clickEvent, string body)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame(
				"INSERT INTO notifications (account_id, title, click_event, body) VALUES ({0}, {1}, {2}, {3})",
				accountID, title, clickEvent, body
			).ConfigureAwait(true);
			return (EntityDatabaseID)mysqlResult.GetInsertID();
		}

		public static async Task DeleteAccountNotification(EntityDatabaseID notificationID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM notifications WHERE id={0}", notificationID).ConfigureAwait(true);
		}

		public static async Task<AdminCheckVehInfo> GetAdminCheckVehInfo(EntityDatabaseID VehicleID)
		{

			CMySQLResult mysqlVehicleResult = await m_MySQLInst.QueryGame("SELECT * FROM vehicles WHERE id = {0} LIMIT 1;", VehicleID).ConfigureAwait(true);
			if (mysqlVehicleResult.NumRows() == 1)
			{
				AdminCheckVehInfo checkInfo = new AdminCheckVehInfo
				{
					VehicleDetails = new CDatabaseStructureVehicle(mysqlVehicleResult.GetRow(0))
				};

				// Get notes
				CMySQLResult mysqlResultNotes = await m_MySQLInst.QueryGame("SELECT * FROM vehicle_notes WHERE vehicle_id ={0} LIMIT 100;", VehicleID).ConfigureAwait(true);
				foreach (var noteRow in mysqlResultNotes.GetRows())
				{
					BasicAccountInfo userDetails = await GetBasicAccountInfoFromDBID((EntityDatabaseID)Convert.ChangeType(noteRow["creator_id"], typeof(EntityDatabaseID))).ConfigureAwait(true);
					if (userDetails != null)
					{
						checkInfo.AdminVehicleNotes.Add(new CAdminVehicleNote(
							(EntityDatabaseID)Convert.ChangeType(noteRow["vehicle_id"], typeof(EntityDatabaseID)),
							userDetails.Username, noteRow["note"], noteRow["created_at"]));
					}
				}

				if (checkInfo.VehicleDetails.vehicleType == EVehicleType.PlayerOwned || checkInfo.VehicleDetails.vehicleType == EVehicleType.RentalCar)
				{
					BasicAccountInfo vehicleOwnerDetails = await GetBasicAccountInfoFromCharacterID(checkInfo.VehicleDetails.ownerID).ConfigureAwait(true);
					if (vehicleOwnerDetails != null)
					{
						checkInfo.OwnerName = vehicleOwnerDetails.Username;
					}
					else
					{
						checkInfo.OwnerName = "Unknown (Player vehicle)";
					}
				}
				else if (checkInfo.VehicleDetails.vehicleType == EVehicleType.FactionOwned ||
						 checkInfo.VehicleDetails.vehicleType == EVehicleType.FactionOwnedRental)
				{
					CDatabaseStructureFaction faction =
						await LoadSingleFaction(checkInfo.VehicleDetails.ownerID).ConfigureAwait(true);
					if (faction != null)
					{
						checkInfo.OwnerName = faction.strName;
					}
					else
					{
						checkInfo.OwnerName = "Unknown (Faction vehicle)";
					}
				}
				else
				{
					checkInfo.OwnerName = "Unknown (Typed vehicle)";
				}

				return checkInfo;
			}

			return new AdminCheckVehInfo();
		}

		public static async Task<AdminCheckIntInfo> GetAdminCheckIntInfo(EntityDatabaseID PropertyID)
		{

			CMySQLResult mysqlPropertyResult = await m_MySQLInst.QueryGame("SELECT * FROM properties WHERE id = {0} LIMIT 1;", PropertyID).ConfigureAwait(true);
			if (mysqlPropertyResult.NumRows() == 1)
			{
				AdminCheckIntInfo checkInfo = new AdminCheckIntInfo
				{
					PropertyDetails = Database.Models.Property.FromDB(mysqlPropertyResult.GetRow(0))
				};

				// Get notes
				CMySQLResult mysqlResultNotes = await m_MySQLInst.QueryGame("SELECT * FROM property_notes WHERE property_id ={0} LIMIT 100;", PropertyID).ConfigureAwait(true);
				foreach (var noteRow in mysqlResultNotes.GetRows())
				{
					BasicAccountInfo userDetails = await GetBasicAccountInfoFromDBID((EntityDatabaseID)Convert.ChangeType(noteRow["creator_id"], typeof(EntityDatabaseID))).ConfigureAwait(true);
					checkInfo.AdminPropertyNotes.Add(new CAdminPropertyNote((EntityDatabaseID)Convert.ChangeType(noteRow["property_id"], typeof(EntityDatabaseID)), userDetails.Username, noteRow["note"], noteRow["created_at"]));
				}

				if (checkInfo.PropertyDetails.OwnerType == EPropertyOwnerType.Player && checkInfo.PropertyDetails.OwnerId > 0)
				{
					BasicAccountInfo propertyOwnerDetails = await GetBasicAccountInfoFromCharacterID(checkInfo.PropertyDetails.OwnerId).ConfigureAwait(true);
					if (propertyOwnerDetails != null)
					{
						checkInfo.OwnerName = propertyOwnerDetails.Username;
					}
					else
					{
						checkInfo.OwnerName = "Unknown";
					}
				}
				else if (checkInfo.PropertyDetails.OwnerType == EPropertyOwnerType.Faction)
				{
					CDatabaseStructureFaction faction = await LoadSingleFaction(checkInfo.PropertyDetails.OwnerId).ConfigureAwait(true);
					if (faction != null)
					{
						checkInfo.OwnerName = faction.strName;
					}
					else
					{
						checkInfo.OwnerName = "Unknown";
					}
				}
				else
				{
					checkInfo.OwnerName = "Unknown";
				}

				return checkInfo;
			}

			return new AdminCheckIntInfo();
		}

		public static async void CreateVehicleNote(EntityDatabaseID vehicleID, EntityDatabaseID creator, string note)
		{
			await m_MySQLInst.QueryGame("INSERT INTO `vehicle_notes` (`vehicle_id`, `creator_id`, `note`, `created_at`) VALUES('{0}', '{1}', '{2}', CURRENT_TIMESTAMP);", vehicleID, creator, note).ConfigureAwait(true);
		}

		public static async void CreatePropertyNote(EntityDatabaseID propertyID, EntityDatabaseID creator, string note)
		{
			await m_MySQLInst.QueryGame("INSERT INTO `property_notes` (`property_id`, `creator_id`, `note`, `created_at`) VALUES ('{0}', '{1}', '{2}', CURRENT_TIMESTAMP);", propertyID, creator, note).ConfigureAwait(true);
		}

		public static async Task UpdateVehicleStolenState(EntityDatabaseID vehicleID, bool stolen)
		{
			await m_MySQLInst.QueryGame("UPDATE `vehicles` SET `stolen` = {1} WHERE `vehicles`.`id` = {0};", vehicleID, stolen).ConfigureAwait(true);
		}

		public static async Task<List<OwlMapDatabase>> GetAllCustomInteriors()
		{
			List<OwlMapDatabase> returnVal = new List<OwlMapDatabase>();

			CMySQLResult mysqlMapResult = await m_MySQLInst.QueryGame("SELECT * FROM `custom_interior_maps`;").ConfigureAwait(true);
			foreach (var mapRow in mysqlMapResult.GetRows())
			{
				OwlMapDatabase owlMap = new OwlMapDatabase
				{
					MapID = mapRow.GetValue<EntityDatabaseID>("id"),
					PropertyID = mapRow.GetValue<EntityDatabaseID>("property_id"),
					MarkerX = mapRow.GetValue<float>("marker_x"),
					MarkerY = mapRow.GetValue<float>("marker_y"),
					MarkerZ = mapRow.GetValue<float>("marker_z"),
					UploadedAt = mapRow.GetValue<string>("uploaded_at"),
					UpdatedAt = mapRow.GetValue<string>("updated_at")
				};

				// Get objects linked with map
				CMySQLResult mysqlResultObjects = await m_MySQLInst.QueryGame("SELECT * FROM `custom_interior_objects` WHERE map_id = {0}", owlMap.MapID).ConfigureAwait(true);
				foreach (var objectRow in mysqlResultObjects.GetRows())
				{
					OwlMapObjectDatabase owlObject = new OwlMapObjectDatabase
					{
						model = objectRow.GetValue<string>("model"),
						x = objectRow.GetValue<float>("position_x"),
						y = objectRow.GetValue<float>("position_y"),
						z = objectRow.GetValue<float>("position_z"),
						rx = objectRow.GetValue<float>("rotation_x"),
						ry = objectRow.GetValue<float>("rotation_y"),
						rz = objectRow.GetValue<float>("rotation_z")
					};
					owlMap.MapObjects.Add(owlObject);
				}

				returnVal.Add(owlMap);
			}
			return returnVal;
		}

		public static async Task<int> CreateCustomMap(EntityDatabaseID propertyID, float markerX, float markerY, float markerZ)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("INSERT INTO `custom_interior_maps` (`property_id`, `marker_x`, `marker_y`, `marker_z`, `uploaded_at`) VALUES ('{0}', '{1}', '{2}', '{3}', CURRENT_TIMESTAMP);", propertyID, markerX, markerY, markerZ).ConfigureAwait(true);
			return (int)mysqlResult.GetInsertID();
		}

		public static async Task CreateCustomMapObject(string model, float x, float y, float z, float rx, float ry, float rz, EntityDatabaseID mapID)
		{
			await m_MySQLInst.QueryGame("INSERT INTO `custom_interior_objects` (`model`, `position_x`, `position_y`, `position_z`, `rotation_x`, `rotation_y`, `rotation_z`, `map_id`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", model, x, y, z, rx, ry, rz, mapID).ConfigureAwait(true);
		}

		public static async Task<OwlMapDatabase> GetCustomMap(EntityDatabaseID propertyID)
		{
			CMySQLResult mysqlResult = await m_MySQLInst.QueryGame("SELECT * FROM `custom_interior_maps` WHERE property_id = {0} LIMIT 1;", propertyID).ConfigureAwait(true);
			if (mysqlResult.NumRows() >= 1)
			{
				var row = mysqlResult.GetRow(0);
				OwlMapDatabase foundMap = new OwlMapDatabase
				{
					MapID = row.GetValue<int>("id"),
					PropertyID = row.GetValue<int>("property_id"),
					MarkerX = row.GetValue<float>("marker_x"),
					MarkerY = row.GetValue<float>("marker_y"),
					MarkerZ = row.GetValue<float>("marker_z"),
					UploadedAt = row.GetValue<string>("uploaded_at"),
					UpdatedAt = row.GetValue<string>("updated_at")
				};
				return foundMap;
			}

			return new OwlMapDatabase();
		}

		public static async Task UpdateCustomMapMarker(EntityDatabaseID mapID, float markerX, float markerY, float markerZ)
		{
			await m_MySQLInst.QueryGame("UPDATE `custom_interior_maps` SET `marker_x` = '{0}', `marker_y` = '{1}', `marker_z` = '{2}' WHERE `id` = {3};", markerX, markerY, markerZ, mapID).ConfigureAwait(true);
		}

		public static async Task UpdateCustomMapLastUpdated(EntityDatabaseID mapID)
		{
			await m_MySQLInst.QueryGame("UPDATE `custom_interior_maps` SET `updated_at` = CURRENT_TIMESTAMP WHERE `id` = {0};", mapID).ConfigureAwait(true);
		}

		public static async Task DeleteCustomMap(EntityDatabaseID mapID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM `custom_interior_maps` WHERE `id` = {0};", mapID).ConfigureAwait(true);
		}

		public static async Task DeleteCustomMapObjects(EntityDatabaseID mapID)
		{
			await m_MySQLInst.QueryGame("DELETE FROM `custom_interior_objects` WHERE `map_id` = {0};", mapID).ConfigureAwait(true);
		}

		public static async Task UpdateGenericPosition(EntityDatabaseID databaseID, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
		{
			await m_MySQLInst.QueryGame("UPDATE inventories SET x={0}, y={1}, z={2}, rx={3}, ry={4}, rz={5} WHERE id={6} LIMIT 1;", posX, posY, posZ, rotX, rotY, rotZ, databaseID).ConfigureAwait(true);
		}
	}
}
