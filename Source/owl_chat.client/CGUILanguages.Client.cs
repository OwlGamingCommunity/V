using System;
using System.Collections.Generic;

internal class CGUILanguages : CEFCore
{
	private string m_StrNewActiveLanguage = string.Empty;

	public CGUILanguages(OnGUILoadedDelegate callbackOnLoad) : base("owl_chat.client/languages.html", EGUIID.Languages, callbackOnLoad)
	{
		UIEvents.LanguageMenu_Close += OnLanguageMenuClose;
		UIEvents.LanguageMenu_SelectLanguage += OnSelectLanguage;
	}

	private void OnLanguageMenuClose()
	{
		//On closing the UI we send the new language to the server :)
		if (!string.IsNullOrEmpty(m_StrNewActiveLanguage))
		{
			NetworkEventSender.SendNetworkEvent_UpdateActiveLanguage((ECharacterLanguage)Enum.Parse(typeof(ECharacterLanguage), m_StrNewActiveLanguage));
		}

		SetVisible(false, false, false);
		Reload();
	}

	public override void OnLoad()
	{

	}

	public void OnSelectLanguage(string NewActiveLanguage)
	{
		//Set the string of the new active language but don't send event to prevent unnecessary queries.
		m_StrNewActiveLanguage = NewActiveLanguage;
	}


	public void Initialize(List<CCharacterLanguageTransmit> CharacterLanguageList)
	{
		Execute("Reset");

		foreach (var lang in CharacterLanguageList)
		{
			string strProgress = $"{lang.Progress:0}";
			Execute("AddLanguageToUI", lang.CharacterLanguage.ToString(), lang.Active ? true : false, strProgress);

		}
	}
}