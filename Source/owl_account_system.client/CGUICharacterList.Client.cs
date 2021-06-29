
using EntityDatabaseID = System.Int64;

internal class GUICharacterList : CEFCore
{
	public GUICharacterList(OnGUILoadedDelegate callbackOnLoad) : base("owl_account_system.client/characters.html", EGUIID.CharacterList, callbackOnLoad)
	{
		UIEvents.PreviewCharacter += OnPreviewCharacter;
		UIEvents.Logout += OnLogout;
		UIEvents.SetAutoSpawn += OnSetAutoSpawn;
		UIEvents.OpenTransferAssets += OpenTransferAssets;
		UIEvents.GotoViewAchievements += GotoViewAchievements;
		UIEvents.GotoCreateCharacter += OnGotoCreateCharacter;
	}

	public override void OnLoad()
	{

	}

	private void OnGotoCreateCharacter()
	{
		SetVisible(false, false, false);
		CharacterSelection.ResetLastCharacterID();
		CharacterCreation.Show(CharacterCreation.g_strDefaultName);

		CharacterSelection.StopMusic();
	}

	private void GotoViewAchievements()
	{
		SetVisible(false, false, false);
		CharacterSelection.ResetLastCharacterID();
		AchievementsList.Show();
	}

	private void OnPreviewCharacter(int index)
	{
		CharacterSelection.PreviewCharacter(index);
	}

	private void OnSetAutoSpawn()
	{
		CharacterSelection.OnSetAutoSpawn();
	}

	private void OpenTransferAssets()
	{
		CharacterSelection.OpenTransferAssets();
	}

	private void OnLogout()
	{
		NetworkEventSender.SendNetworkEvent_RequestLogout();
	}

	public void ClearCharacters()
	{
		Execute("ClearCharacters");
	}

	public void AddCharacter(EntityDatabaseID ID, string name, int lastSeenHours, RAGE.Vector3 vecPos, bool dead)
	{
		string zoneName = RAGE.Game.Zone.GetNameOfZone(vecPos.X, vecPos.Y, vecPos.Z);
		string realZoneName = ZoneNameHelper.ZoneNames.ContainsKey(zoneName) ? ZoneNameHelper.ZoneNames[zoneName] : "San Andreas";

		// Parse last seen
		string lastSeen = "Unknown";
		if (lastSeenHours < 0)
		{
			lastSeen = "Never";
			realZoneName = "";
		}
		else if (lastSeenHours < 24)
		{
			if (lastSeenHours < 1)
			{
				lastSeen = "Less than an hour ago";
			}
			else if (lastSeenHours == 1)
			{
				lastSeen = "1 hour ago";
			}
			else
			{
				lastSeen = Helpers.FormatString("{0} hours ago", lastSeenHours);
			}
		}
		else if (lastSeenHours >= 24 && lastSeenHours < 48)
		{
			lastSeen = "1 day ago";
		}
		else
		{
			lastSeen = Helpers.FormatString("{0} days ago", lastSeenHours / 24);
		}

		Execute("AddCharacter", ID, name, lastSeen, realZoneName, !dead);
	}

	public void CommitCharacters()
	{
		Execute("GotoCharacterPage", 0);
	}

	public void SetAutoSpawnVisible(bool bVisible)
	{
		Execute("SetAutoSpawnVisible", bVisible);
	}

	public void SetAutoSpawnText(string strText)
	{
		Execute("SetAutoSpawnText", strText);
	}
}