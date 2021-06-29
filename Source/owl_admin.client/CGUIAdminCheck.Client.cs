using System.Collections.Generic;

internal class CGUIAdminCheck : CEFCore
{
	public CGUIAdminCheck(OnGUILoadedDelegate callbackOnLoad) : base("owl_admin.client/check.html", EGUIID.AdminCheck, callbackOnLoad)
	{
		UIEvents.CloseCheck += () => { AdminSystem.GetAdminCheck().OnCloseCheck(); };
		UIEvents.SaveAdminNotes += (string strNotes) => { AdminSystem.GetAdminCheck().OnSaveAdminNotes(strNotes); };
	}

	public override void OnLoad()
	{

	}

	public void SetDetails(string username, string character_name, int playerID, string ipaddress, string staff_rank, int ping, int gamecoins, float money, float bank_money, int health, int armor, string factions,
		string current_vehicle, float x, float y, float z, string interior, uint dimension, uint hours_played_account, uint hours_played_character, string skin, string weapon, int NumPunishmentPointsActive, int NumPunishmentPointsLifetime, string location)
	{
		Execute("SetDetails", username, character_name, playerID, ipaddress, staff_rank, ping, gamecoins, money, bank_money, health, armor, factions, current_vehicle, x, y, z, interior, dimension, hours_played_account, hours_played_character, skin, weapon, NumPunishmentPointsActive, NumPunishmentPointsLifetime, location);
	}

	public void SetAdminNotes(string strNotes)
	{
		Execute("SetAdminNotes", strNotes);
	}

	public void ResetAdminHistory()
	{
		Execute("ResetAdminHistory");
	}

	public void SetAdminHistory(List<string> lstAdminHistory)
	{
		foreach (string strHistoryEntry in lstAdminHistory)
		{
			Execute("AddHistoryEntry", strHistoryEntry);
		}
	}
}