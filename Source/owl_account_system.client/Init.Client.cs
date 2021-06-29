class Init_AccountSystem : RAGE.Events.Script { Init_AccountSystem() { OwlScriptManager.RegisterScript<AccountSystem>(); } }

class AccountSystem : OwlScript
{
	public AccountSystem()
	{
		CTattooDefinition[] jsonData = OwlJSON.DeserializeObject<CTattooDefinition[]>(CTattooData.TattooData, EJsonTrackableIdentifier.TattooData);

		foreach (CTattooDefinition tattooDef in jsonData)
		{
			TattooDefinitions.g_TattooDefinitions.Add(tattooDef.ID, tattooDef);
		}

		CHairTattooDefinition[] jsonHairData = OwlJSON.DeserializeObject<CHairTattooDefinition[]>(CHairTattooData.HairTattooData, EJsonTrackableIdentifier.HairTattooData);

		foreach (CHairTattooDefinition HairTattooDef in jsonHairData)
		{
			TattooDefinitions.g_HairTattooDefinitions.Add(HairTattooDef.ID, HairTattooDef);
		}

		LoginSystem.Init();
		CharacterCreation.Init();
		RegisterSystem.Init();
		CharacterSelection.Init();
		Tutorial.Init();
		AchievementsList.Init();
		Application.Init();
		CustomSkinDataHandler.Init();
		Christmas.Init();
		Halloween.Init();
		CharacterLook.Init();
	}
}