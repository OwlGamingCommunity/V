using System.Collections.Generic;

public class Languages
{
	public Languages()
	{
		NetworkEvents.ShowLanguages += OnShowLanguages;
	}

	private void OnShowLanguages(List<CCharacterLanguageTransmit> CharacterLanguageList)
	{
		m_languagesUI.Initialize(CharacterLanguageList);
		m_languagesUI.SetVisible(true, true, false);
	}

	private CGUILanguages m_languagesUI = new CGUILanguages(() => { });
}
