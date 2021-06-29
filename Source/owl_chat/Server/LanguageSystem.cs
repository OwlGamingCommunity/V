using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LanguageSystem
{
	private static readonly string[] whiteListedWords = { "yes", "no", "television", "coffee", "tea", "okay", "ok", "nope", "phone", "telephone" };
	private static Random m_randRandom = new Random();

	public LanguageSystem()
	{
		CommandManager.RegisterCommand("addlanguage", "Adds a language to a character", new Action<CPlayer, CVehicle, CPlayer, ECharacterLanguage, float>(AddPlayerLanguage), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("removelanguage", "Removes a language for a player", new Action<CPlayer, CVehicle, CPlayer, ECharacterLanguage>(RemovePlayerLanguage), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("languages", "Lists all the player their languages", new Action<CPlayer, CVehicle, CPlayer>(ListPlayerLanguages), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

		CommandManager.RegisterCommand("langen", "Switches to English", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.English); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langru", "Switches to Russian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Russian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langde", "Switches to German", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.German); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langfr", "Switches to French", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.French); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langdu", "Switches to Dutch", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Dutch); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langit", "Switches to Italian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Italian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langgd", "Switches to Gaelic", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Gaelic); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langjp", "Switches to Japanese", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Japanese); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langcn", "Switches to Chinese", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Chinese); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langab", "Switches to Arabic", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Arabic); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langno", "Switches to Norwegian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Norwegian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langse", "Switches to Swedish", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Swedish); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langdk", "Switches to Danish", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Danish); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langws", "Switches to Welsh", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Welsh); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langhu", "Switches to Hungarian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Hungarian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langbs", "Switches to Bosnian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Bosnian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langso", "Switches to Somalian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Somalian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langfi", "Switches to Finnish", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Finnish); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langge", "Switches to Georgian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Georgian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langgr", "Switches to Greek", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Greek); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langpo", "Switches to Polish", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Polish); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langpt", "Switches to Portugese", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Portugese); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langtu", "Switches to Turkish", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Turkish); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langet", "Switches to Estonian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Estonian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langko", "Switches to Korean", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Korean); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langvt", "Switches to Viatnemese", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Viatnemese); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langro", "Switches to Romanian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Romanian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langal", "Switches to Albanian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Albanian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langli", "Switches to Lithuanian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Lithuanian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langsb", "Switches to Serbian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Serbian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langcr", "Switches to Croatian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Croatian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langsk", "Switches to Slovak", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Slovak); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langhe", "Switches to Hebrew", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Hebrew); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langpr", "Switches to Persian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Persian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langaf", "Switches to Afrikaans", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Afrikaans); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langlv", "Switches to Latvian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Latvian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langbg", "Switches to Bulgarian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Bulgarian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langar", "Switches to Armenian", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Armenian); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langzu", "Switches to Zulu", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Zulu); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langpj", "Switches to Punjabi", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Punjabi); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("langes", "Switches to Spanish", new Action<CPlayer, CVehicle>((CPlayer player, CVehicle vehicle) => { SwitchActiveLanguageViaCMD(player, ECharacterLanguage.Spanish); }), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		NetworkEvents.OpenLanguagesUI += OnOpenLanguagesUI;
		NetworkEvents.UpdateActiveLanguage += OnUpdateActiveLanguage;
	}

	private void SwitchActiveLanguageViaCMD(CPlayer SenderPlayer, ECharacterLanguage lang)
	{
		// Do we know the language?
		if (SenderPlayer.GetCharacterLanguageOfType(lang) != null)
		{
			SenderPlayer.SendNotification("Languages", ENotificationIcon.ExclamationSign, "Your active language has been set to {0}", lang.ToString());
			OnUpdateActiveLanguage(SenderPlayer, lang);
		}
		else
		{
			SenderPlayer.SendNotification("Languages", ENotificationIcon.ExclamationSign, "You do not know the {0}", lang.ToString());
		}
	}

	private void OnUpdateActiveLanguage(CPlayer SenderPlayer, ECharacterLanguage UpdatedActiveLanguage)
	{
		SenderPlayer.RemoveCharacterActiveLanguage();
		SenderPlayer.UpdateCharacterActiveLanguage(UpdatedActiveLanguage);
	}

	private void OnOpenLanguagesUI(CPlayer SourcePlayer)
	{
		List<CCharacterLanguageTransmit> lstLanguagesToTransmit = new List<CCharacterLanguageTransmit>();

		if (SourcePlayer != null)
		{
			foreach (CCharacterLanguage charLang in SourcePlayer.GetCharacterLanguages())
			{
				CCharacterLanguageTransmit characterLanguageTransmit = GetLanguageInfo(charLang);
				lstLanguagesToTransmit.Add(characterLanguageTransmit);
			}

			NetworkEventSender.SendNetworkEvent_ShowLanguages(SourcePlayer, lstLanguagesToTransmit);
		}
	}

	private CCharacterLanguageTransmit GetLanguageInfo(CCharacterLanguage characterLanguage)
	{
		CCharacterLanguageTransmit characterLanguagesToTransmit = new CCharacterLanguageTransmit(characterLanguage.CharacterLanguage, characterLanguage.Active, characterLanguage.Progress);
		return characterLanguagesToTransmit;
	}

	private void AddPlayerLanguage(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, ECharacterLanguage CharacterLanguage, float LanguageProgress)
	{
		if (LanguageProgress > 100f || LanguageProgress < 0f)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You have to select progress between 0 and 100");
			return;
		}

		if (CharacterLanguage != ECharacterLanguage.None)
		{
			if (!TargetPlayer.KnowsLanguageOfType(CharacterLanguage))
			{
				TargetPlayer.AddLanguageForPlayer(new CCharacterLanguage(CharacterLanguage, false, LanguageProgress), true);
				TargetPlayer.SendNotification("Languages", ENotificationIcon.ThumbsUp, "{0} {1} taught you {2}", SenderPlayer.AdminTitle, SenderPlayer.Username, CharacterLanguage);
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Successfully taught {0} the language {1} ", Helpers.ColorString(255, 255, 255, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)), Helpers.ColorString(255, 255, 255, CharacterLanguage.ToString()));
			}
			else
			{
				TargetPlayer.SetLanguageProgress(CharacterLanguage, LanguageProgress);
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "{0} already has {1} as a language so you have updated the progress instead.", Helpers.ColorString(255, 255, 255, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)), Helpers.ColorString(255, 255, 255, CharacterLanguage.ToString()));
				TargetPlayer.SendNotification("Languages", ENotificationIcon.ThumbsUp, "{0} {1} updated your {2} language progress. New progress: {3}%", SenderPlayer.AdminTitle, SenderPlayer.Username, CharacterLanguage.ToString(), LanguageProgress);
			}
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Invalid Language ID");
		}
	}

	private void RemovePlayerLanguage(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, ECharacterLanguage CharacterLanguage)
	{
		CCharacterLanguage charLanguage = TargetPlayer.GetCharacterLanguageOfType(CharacterLanguage);

		if (TargetPlayer.GetCharacterLanguages().Count <= 1)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "{0} only has one language. Consider adding a language before removing this one.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			return;
		}

		if (charLanguage != null)
		{
			if (CharacterLanguage == TargetPlayer.GetActiveLanguage())
			{
				TargetPlayer.UpdateCharacterActiveLanguage(TargetPlayer.GetCharacterLanguages().First(i => i != charLanguage).CharacterLanguage);
				TargetPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 255, 255, "Because {0} was your active language we have set your active language to the first we could find.", CharacterLanguage.ToString());
			}

			TargetPlayer.RemoveLanguageForPlayer(charLanguage);
			TargetPlayer.SendNotification("Languages", ENotificationIcon.ThumbsUp, "{0} {1} removed {2} from your vocabulary!", SenderPlayer.AdminTitle, SenderPlayer.Username, CharacterLanguage.ToString());
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Successfully removed the language {1} for {0} ", Helpers.ColorString(255, 255, 255, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)), Helpers.ColorString(255, 255, 255, CharacterLanguage.ToString()));
		}
		else
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "{0} doesn't know the specified language.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
		}
	}

	private void ListPlayerLanguages(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer)
	{
		if (TargetPlayer != null)
		{
			SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 255, 0, "~~~~~~~~~~~{0}{1}~~~~~~~~~~~", Helpers.ColorString(0, 255, 0, TargetPlayer.GetCharacterName(ENameType.StaticCharacterName)), Helpers.ColorString(0, 255, 0, "'s Languages"));
			List<CCharacterLanguage> lstCharLanguage = TargetPlayer.GetCharacterLanguages();

			foreach (var lang in lstCharLanguage)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 255, 255, Helpers.FormatString("{0} {1}, {2}% Active: {3}", Helpers.ColorString(0, 255, 0, "-"), lang.CharacterLanguage.ToString(), lang.Progress, lang.Active ? "Yes" : "No"));
			}
		}
	}

	//APPLY LANGUAGE LOGIC
	public static (string, bool, bool) ApplyLanguageLogic(string strInput, CPlayer SenderPlayer, CPlayer TargetPlayer)
	{
		string strOut = string.Empty;
		ECharacterLanguage languageSpoken = SenderPlayer.GetActiveLanguage();
		float SenderLangProgress = SenderPlayer.GetActiveLanguageProgress();
		bool bKnowsLanguage = TargetPlayer.KnowsLanguageOfType(languageSpoken);

		bool bSenderNeedsAward = false;
		bool bTargetNeedsAward = false;


		if (!TargetPlayer.AdminDuty && TargetPlayer == SenderPlayer)
		{
			bSenderNeedsAward = false;
			bTargetNeedsAward = false;
			strOut = CreateGiberrishBasedOnLanguageLevel(strInput, SenderLangProgress);

			return (strOut, bSenderNeedsAward, bTargetNeedsAward);
		}
		else if (TargetPlayer.AdminDuty)
		{
			return (strInput, bSenderNeedsAward, bTargetNeedsAward);
		}

		if (bKnowsLanguage)
		{
			//IF they know the language we check the progress
			float TargetLangProgress = TargetPlayer.GetLanguageProgress(languageSpoken);

			if (TargetLangProgress == 100f)
			{
				//If we are max level we just return the string as is.
				bSenderNeedsAward = false;
				bTargetNeedsAward = false;
				return (strInput, bSenderNeedsAward, bTargetNeedsAward);
			}
			else if (TargetLangProgress < 100f || TargetLangProgress > 0f)
			{
				if (SenderLangProgress > TargetLangProgress)
				{
					bSenderNeedsAward = false;
					bTargetNeedsAward = true;
				}
				else if (SenderLangProgress < TargetLangProgress)
				{
					bSenderNeedsAward = true;
					bTargetNeedsAward = false;
				}
				else
				{
					bSenderNeedsAward = false;
					bTargetNeedsAward = false;
				}

				strOut = CreateGiberrishBasedOnLanguageLevel(strInput, TargetLangProgress);

				return (strOut, bSenderNeedsAward, bTargetNeedsAward);
			}
			else
			{
				return (strInput, bSenderNeedsAward, bTargetNeedsAward);
			}
		}
		else
		{
			//If they don't know the language we can just start complete gibberish
			// Because the level is 0 we will replace every letter in the string.
			bSenderNeedsAward = false;
			bTargetNeedsAward = false;
			strOut = CreateGiberrishBasedOnLanguageLevel(strInput, 10.0f);
			return (strOut, bSenderNeedsAward, bTargetNeedsAward);
		}
	}

	public static void AwardXP(CPlayer Player, ECharacterLanguage langForAward, float fAmountToAward)
	{
		Player.AwardXPForLanguage(langForAward, fAmountToAward);
	}

	//START GIBBERISH GENERATOR LOGIC

	private static string CreateGiberrishBasedOnLanguageLevel(string strInput, float languageProgress)
	{
		int langProgress = 1;

		if (languageProgress != 10.0f)
		{
			langProgress = (int)languageProgress / 10;
		}

		string strOut;
		if (languageProgress == 100)
		{
			strOut = strInput;
		}
		else if (languageProgress > 80f && languageProgress < 100f)
		{
			strOut = GenerateGibberish(strInput, true);
			// only replace vowels and allow whitelisted words
		}
		else
		{
			strOut = GenerateGibberish(strInput, false, langProgress);
			// replace based on level.
		}

		return strOut;
	}

	private static string GenerateRandomLetterFromAlphabet()
	{
		const string alphabet = "abcdefghijklmnopqrstuvwxyz";
		return alphabet[m_randRandom.Next(0, alphabet.Length)].ToString();
	}

	private static string GenerateGibberish(string strInput, bool AllowWhiteListedWords, int numOfIterationsToSkip = 0)
	{

		StringBuilder strOut = new StringBuilder();
		string[] stringAsArray = strInput.Split(' ');
		string vowels = "aeiouyAEIOUY";

		if (numOfIterationsToSkip == 0)
		{
			foreach (var word in stringAsArray)
			{
				if (AllowWhiteListedWords && Array.IndexOf(whiteListedWords, word.ToLower()) >= 0)
				{
					strOut.Append(word);
				}
				else
				{
					foreach (var letter in word)
					{
						if (char.IsWhiteSpace(letter) || char.IsNumber(letter) || char.IsSymbol(letter) || char.IsUpper(letter) || char.IsPunctuation(letter))
						{
							strOut.Append(letter);
						}
						else if (Array.IndexOf(vowels.ToArray(), letter) >= 0)
						{
							string replacedVowel = GenerateRandomLetterFromAlphabet();
							strOut.Append(replacedVowel);
						}
						else
						{
							strOut.Append(letter);
						}
					}
				}

				strOut.Append(" "); // add space on each iteration (after each word)
			}
		}
		else
		{
			int iterations = 0;
			foreach (char letter in strInput)
			{
				// If it's a space/number/symbol we just add that as the letter instead of replacing.
				// our input is level. use modulo to verify we get remainder 0. If true replace. This will replace a letter for each X iterations. If input = 3. We replace every third iteration meaning every third letter.
				if (char.IsWhiteSpace(letter) || char.IsNumber(letter) || char.IsSymbol(letter) || char.IsUpper(letter) || char.IsPunctuation(letter))
				{
					strOut.Append(letter);
				}
				else if ((iterations % numOfIterationsToSkip) == 0)
				{
					string randomLetter = GenerateRandomLetterFromAlphabet();
					strOut.Append(randomLetter);
				}
				else
				{
					strOut.Append(letter);
				}

				iterations++;
			}
		}

		return strOut.ToString();
	}
	// END GIBBERISH LOGIC - fuck this so much *cries*
}
