using System;
using System.Collections.Generic;

public static class Tutorial
{
	private static ETutorialVersions m_CurrentTutorialVersion = ETutorialVersions.None;
	private static List<TutorialPhase> m_lstPhases = null; // tutorial phases in progress
	private static int g_CurrentTutorialPhase = -1;
	private static int g_TimeBetweenPhasesMS = 6000;
	private static List<TutorialPhase> g_lstTutorialPhases = new List<TutorialPhase> // tutorial phase definitions
	{
		new TutorialPhase("Keeping your Car Running", "Vehicles require fuel which can be purchased from gas stations.", new RAGE.Vector3(-537.9805f, -1169.723f, 34.18202f), new RAGE.Vector3(-533.3306f, -1189.019f, 25.76605f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Keeping your Car Running", "Vehicles require fuel which can be purchased from gas stations.", new RAGE.Vector3(-92.86067f, 6443.376f, 35.0f), new RAGE.Vector3(-94.25081f, 6420.476f, 32.0f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Taking care of your Greenbacks", "You can deposit your money with the responsible banks of San Andreas.", new RAGE.Vector3(-1328.061f, -841.1362f, 24.96383f), new RAGE.Vector3(-1317.758f, -832.2154f, 20.96959f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Taking care of your Greenbacks", "Salaries will be deposited into your bank account.", new RAGE.Vector3(-124.5045f, 6448.836f, 31.54093f), new RAGE.Vector3(-111.4076f, 6462.317f, 33.0f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Taking care of your Greenbacks", "Any large purchases made or purchases made on credit will be deducted from your bank account.", new RAGE.Vector3(-1221.105f, -314.2131f, 45.63841f), new RAGE.Vector3(-1213.751f, -328.5305f, 41.78419f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Getting the bumps out", "You can repair your vehicle at any Pay n Spray.", new RAGE.Vector3(-368.7344f, -111.0804f, 46.68053f), new RAGE.Vector3(-351.2934f, -116.5016f, 42.84315f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Getting the bumps out", "You can repair your vehicle at any Pay n Spray.", new RAGE.Vector3(-90.40865f, 6443.473f, 41.44516f), new RAGE.Vector3(-74.37033f, 6427.362f, 31.44009f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Cleaning Up", "When your car gets too dirty, you can wash your car at any Car Wash.", new RAGE.Vector3(63.44884f, -1392.349f, 35.38304f), new RAGE.Vector3(49.29171f, -1391.87f, 35.41426f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Cleaning Up", "When your car gets too dirty, you can wash your car at any Car Wash.", new RAGE.Vector3(-90.40865f, 6443.473f, 41.44516f), new RAGE.Vector3(-74.37033f, 6427.362f, 31.44009f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Bug Stars", "The local warehouse district has multiple job opportunities.", new RAGE.Vector3(181.9692f, -3090.927f, 13.747358f), new RAGE.Vector3(156.8457f, -3091.824f, 9.97011f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Clucking Bell", "The local chicken slaughter house has multiple job opportunities.", new RAGE.Vector3(-46.85513f, 6303.532f, 35.64061f), new RAGE.Vector3(-34.97569f, 6312.445f, 35.38077f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("LS Transit", "Do you love dealing with angry passengers who haven't showered in a week? This job is for you.", new RAGE.Vector3(400.9159f, -682.5703f, 36.24302f), new RAGE.Vector3(434.8232f, -656.4547f, 32.75851f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("LS Transit", "Do you love dealing with angry passengers who haven't showered in a week? This job is for you.", new RAGE.Vector3(-294.0483f, 6037.914f, 41.48025f), new RAGE.Vector3(-272.3264f, 6057.076f, 31.50408f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Dirty Job", "The local sanitation department is rife with opportunity.", new RAGE.Vector3(-663.1833f, -1658.521f, 35.09887f), new RAGE.Vector3(-650.6028f, -1641.703f, 30.1273f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Dirty Job", "The local sanitation department is rife with opportunity.", new RAGE.Vector3(-221.1149f, 6324.75f, 37.45197f), new RAGE.Vector3(-202.7189f, 6310.448f, 31.4893f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Controls", "Script controls can be viewed & changed by pressing 'F5' (Default Control) or /controls.", new RAGE.Vector3(-368.6487f, 6168.627f, 41.3641f), new RAGE.Vector3(-331.1462f, 6183.654f, 31.60873f), "", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Getting your wheels", "Vehicles can be purchased or rented from Premium Deluxe Motorsport.", new RAGE.Vector3(-77.91964f, -1114.71f, 35.83131f), new RAGE.Vector3(-63.13662f, -1102.653f, 30.24758f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Getting your wheels", "Vehicles can be purchased or rented from SA Autos.", new RAGE.Vector3(-252.9168f, 6199.387f, 41.48922f), new RAGE.Vector3(-244.5351f, 6206.752f, 41.48922f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Making A Sweet Ride", "You can customize vehicles at the mod shop.", new RAGE.Vector3(-376.2104f, -127.6749f, 38.63237f), new RAGE.Vector3(358.3738f, -134.2314f, 38.75375f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Making A Sweet Ride", "You can customize vehicles at the mod shop.", new RAGE.Vector3(-78.64719f, 6461.007f, 41.44516f), new RAGE.Vector3(-63.9315f, 6446.221f, 31.49915f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Need a drink?", "Visit Robs Liquors for fine liquors.", new RAGE.Vector3(-1236.362f, -887.9047f, 12.33353f), new RAGE.Vector3(-1226.79f, -902.1462f, 12.27844f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Need a drink?", "Visit the Hen House for fine liquors.", new RAGE.Vector3(-291.0054f, 6238.241f, 30.9f), new RAGE.Vector3(-297.4472f, 6256.015f, 33.48907f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Keybinds", "Custom keybinds can be viewed & modified by pressing 'F5' (Default Control) or /keybinds.", new RAGE.Vector3(-368.6487f, 6168.627f, 41.3641f), new RAGE.Vector3(-331.1462f, 6183.654f, 31.60873f), "", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Need a Trim?", "The barber shop can help you look great again!", new RAGE.Vector3(-26.74494f, -133.7293f, 61.05007f),  new RAGE.Vector3(-31.28873f, -146.0465f, 63.03701f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Need a Trim?", "The barber shop can help you look great again!", new RAGE.Vector3(-290.5485f, 6241.754f, 33.43115f), new RAGE.Vector3(-281.8358f, 6232.937f, 35.69069f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("LSPD", "Local law enforcement is provided by the Los Santos Police Department", new RAGE.Vector3(401.9896f, -941.7307f, 35.44016f), new RAGE.Vector3(419.0507f, -964.8798f, 32.41764f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Sheriff's Office", "The Sheriff's Office also provides weapon licensing services.", new RAGE.Vector3(-435.22f, 6024.255f, 31.49012f), new RAGE.Vector3(-441.2458f, 6018.792f, 33.53073f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Inventory", "Press 'I' (Default Control) to access your inventory and perform actions on items.", new RAGE.Vector3(417.2859f, -975.0492f, 39.43258f), new RAGE.Vector3(427.0302f, -966.1271f, 39.3072f), "", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Los Santos County Government & DMV", "Visit the government to create businesses, get licenses & more.", new RAGE.Vector3(249.5734f, -377.5534f, 57.54124f), new RAGE.Vector3(242.2f, -398.63f, 54.92f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Los Santos County Government & DMV", "After starting a driving test, DMV vehicles are located nearby. Certain jobs require a specific license.", new RAGE.Vector3(-268.0878f, 6093.127f, 41.22028f), new RAGE.Vector3(-283.403f, 6127.08f, 31.48368f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Capitalism", "Want to buy some goods? Visit any Store.", new RAGE.Vector3(528.829f, -158.1732f, 67.0584f), new RAGE.Vector3(528.829f, -163.0645f, 67.0584f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Capitalism", "Want to buy some goods? Visit any Store.", new RAGE.Vector3(-313.0091f, 6215.017f, 41.3803f), new RAGE.Vector3(-325.5205f, 6228.155f, 31.50329f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Fighting Fires", "The county Fire Department provides emergency medical services and fire fighting services.", new RAGE.Vector3(228.2697f, -1626.479f, 37.17305f), new RAGE.Vector3(215.3346f, -1640.843f, 34.64876f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Fighting Fires", "The county Fire Department provides emergency medical services and fire fighting services.", new RAGE.Vector3(-380.7405f, 6154.708f, 41.44165f), new RAGE.Vector3(-376.2814f, 6123.142f, 31.44015f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("The 2nd Amendment", "Visit any Ammunation to purchase legal firearms.", new RAGE.Vector3(32.55473f, -1114.966f, 35.30003f), new RAGE.Vector3(23.92203f, -1111.679f, 35.07084f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("The 2nd Amendment", "Visit any Ammunation to purchase legal firearms.", new RAGE.Vector3(-300.1202f, 6052.692f, 41.40788f), new RAGE.Vector3(-322.0884f, 6074.391f, 31.24339f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Interacting", "Most server elements are interacted with by pressing 'E' (Default Control).", new RAGE.Vector3(-1376.919f, -1086.829f, 10.243455f), new RAGE.Vector3(-1369.614f, -1083.06f, 10.243455f), "", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Postal Service", "Looking for a stable job? Try working as a Mailman.", new RAGE.Vector3(71.05593f, 96.081f, 100.1689f), new RAGE.Vector3(78.53011f, 110.5122f, 100.1689f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Postal Service", "Looking for a stable job? Try working as a Mailman.", new RAGE.Vector3(-385.7299f, 6158.088f, 30.697f), new RAGE.Vector3(-388.8588f, 6158.826f, 31.40157f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Taxi Driver", "Visit SA Taxis to submit a job application.", new RAGE.Vector3(910.6868f, -146.7913f, 78.0f), new RAGE.Vector3(908.5013f, -149.7173f, 78.0f), "Los Santos", ETutorialVersions.MoveToLS),
		new TutorialPhase("Taxi Driver", "Visit SA Taxis to submit a job application.", new RAGE.Vector3(-58.11371f, 6543.684f, 41.55122f), new RAGE.Vector3(-75.8418f, 6561.764f, 31.49081f), "Paleto Bay", ETutorialVersions.FirstRelease_Paleto),
		new TutorialPhase("Sea-men", "Boats can be purchased or rented from Puerta Del Sol Boats.", new RAGE.Vector3(-877.6823f, -1363.707f, 20.5f), new RAGE.Vector3(-846.0367f, -1333.93f, 2.605169f), "Los Santos", ETutorialVersions.FishingAndBoats),
		new TutorialPhase("Here Fishy Fishy", "Fishing can be done when on a boat or pier/beach. You will require fishing equipment.", new RAGE.Vector3(-877.6823f, -1363.707f, 20.5f), new RAGE.Vector3(-846.0367f, -1333.93f, 2.605169f), "Los Santos", ETutorialVersions.FishingAndBoats),
		new TutorialPhase("Getting Inked", "Visit Paleto Tattoo's in Downtown Paleto for fine body art.", new RAGE.Vector3(-281.6796f, 6211.704f, 37.48751f), new RAGE.Vector3(-289.7426f, 6200.844f, 31.46757f), "Paleto Bay", ETutorialVersions.Tattoos),
		new TutorialPhase("Getting Inked", "Visit Blazing Tattoo on Vinewood Boulevard for fine body art.", new RAGE.Vector3(315.2422f, 161.7439f, 107.8023f), new RAGE.Vector3(320.8047f, 178.0548f, 103.5427f), "Los Santos", ETutorialVersions.Tattoos),
		new TutorialPhase("Looking Good Again", "Visit a Plastic Surgeon to refresh your body and look young again. No one will ever know.", new RAGE.Vector3(-967.6395f, -331.1745f, 49.73308f), new RAGE.Vector3(-943.8189f, -337.7739f, 40.00513f), "Los Santos only", ETutorialVersions.PlasticSurgeon),
		new TutorialPhase("More Help", "Need a hand? Check out our beginners guide (owl.pm/guide) and join our discord (owl.pm/d).", new RAGE.Vector3(315.2422f, 161.7439f, 107.8023f), new RAGE.Vector3(320.8047f, 178.0548f, 103.5427f), "Los Santos", ETutorialVersions.FirstRelease_Paleto),
	};

	private class TutorialPhase
	{
		public TutorialPhase(string strTitle, string strMessage, RAGE.Vector3 vecCameraPos, RAGE.Vector3 vecLookAt, string strRegion, ETutorialVersions a_Version)
		{
			m_strTitle = strTitle;
			m_strMessage = strMessage;
			m_vecCameraPos = vecCameraPos;
			m_vecLookAt = vecLookAt;
			m_strRegion = strRegion;
			Version = a_Version;
		}

		public void Deactivate()
		{
			CameraManager.DeactivateCamera(ECameraID.TUTORIAL);
		}

		public void Activate()
		{
			CameraManager.RegisterCamera(ECameraID.TUTORIAL, m_vecCameraPos, m_vecLookAt);
			CameraManager.ActivateCamera(ECameraID.TUTORIAL);

			float fGroundZ = WorldHelper.GetGroundPosition(m_vecCameraPos, true, 1, false);
			RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(m_vecCameraPos.X, m_vecCameraPos.Y, fGroundZ - 10.0f);
			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		}

		public void Draw()
		{
			Activate();

			if (m_strRegion != String.Empty)
			{
				RAGE.Game.Graphics.DrawRect(0.0f, 0.125f, 4.0f, 0.06f, 76, 76, 76, 200, 0);
				TextHelper.Draw2D(m_strRegion, 0.5f, 0.1f, 0.9f, 255, 194, 14, 255, RAGE.Game.Font.Pricedown, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			}

			RAGE.Game.Graphics.DrawRect(0.0f, 0.85f, 4.0f, 0.10f, 76, 76, 76, 200, 0);
			TextHelper.Draw2D(m_strTitle, 0.5f, 0.8f, 1.0f, 255, 194, 14, 255, RAGE.Game.Font.HouseScript, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			TextHelper.Draw2D(m_strMessage, 0.5f, 0.86f, 0.5f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
		}

		private string m_strTitle { get; set; }
		private string m_strMessage { get; set; }
		private RAGE.Vector3 m_vecCameraPos { get; set; }
		private RAGE.Vector3 m_vecLookAt { get; set; }
		private string m_strRegion { get; set; }
		public ETutorialVersions Version { get; set; }
	}


	static Tutorial()
	{

	}

	public static void Init()
	{
		// EVENTS
		NetworkEvents.GotoTutorialState += OnGotoTutorialState;

		NetworkEvents.OfferNewTutorial += OnOfferNewTutorial;
		UIEvents.OnWatchNewTutorial_Confirm_All += OnWatchNewTutorial_Confirm_All;
		UIEvents.OnWatchNewTutorial_Confirm_New += OnWatchNewTutorial_Confirm_New;
		UIEvents.OnWatchNewTutorial_Cancel += OnWatchNewTutorial_Cancel;

		RageEvents.RAGE_OnRender += OnRender;
	}

	private static void OnWatchNewTutorial_Confirm_All()
	{
		m_CurrentTutorialVersion = ETutorialVersions.None;
		NetworkEventSender.SendNetworkEvent_RequestTutorialState(ETutorialVersions.None);
	}

	private static void OnWatchNewTutorial_Confirm_New()
	{
		NetworkEventSender.SendNetworkEvent_RequestTutorialState(m_CurrentTutorialVersion);
	}

	private static void OnWatchNewTutorial_Cancel()
	{
		NetworkEventSender.SendNetworkEvent_FinishTutorialState();
	}

	private static void OnRender()
	{
		if (g_CurrentTutorialPhase != -1 && g_CurrentTutorialPhase < m_lstPhases.Count)
		{
			TutorialPhase tutorialPhase = m_lstPhases[g_CurrentTutorialPhase];
			tutorialPhase.Draw();
		}
	}

	private static void OnGotoTutorialState(ETutorialVersions currentTutorialVersion)
	{
		m_lstPhases = DetermineRequiredTutorialPhases(currentTutorialVersion);

		// TODO_CSHARP: Probably a nicer way of doing this rather than calling into login system
		LoginSystem.HideLogin();

		g_CurrentTutorialPhase = -1;
		GotoNextTutorialPhase();
		ClientTimerPool.CreateTimer(GotoNextTutorialPhase, g_TimeBetweenPhasesMS, m_lstPhases.Count);

		m_musicInst = AudioManager.PlayAudio(EAudioIDs.Country, true, true);
	}

	private static List<TutorialPhase> DetermineRequiredTutorialPhases(ETutorialVersions currentVersion)
	{
		List<TutorialPhase> lstPhases = new List<TutorialPhase>();
		foreach (var phase in g_lstTutorialPhases)
		{
			if (phase.Version > currentVersion)
			{
				lstPhases.Add(phase);
			}
		}

		return lstPhases;
	}

	private static string ConvertSecondsToTimeString(int seconds)
	{
		if (seconds < 60)
		{
			return Helpers.FormatString("{0} seconds", seconds);
		}
		else if (seconds == 60)
		{
			return "1 minute";
		}
		else if (seconds > 60)
		{
			int minutes = seconds / 60;
			seconds = seconds % 60;
			return Helpers.FormatString("{0} {1} and {2} {3}", minutes, minutes > 1 ? "minutes" : "minute", seconds, seconds > 1 ? "seconds" : "second");
		}

		return String.Empty;
	}

	public static void OnOfferNewTutorial(ETutorialVersions currentTutorialVersion)
	{
		m_CurrentTutorialVersion = currentTutorialVersion;

		// better messaging for new players, since tutorial is optional now
		if (currentTutorialVersion == ETutorialVersions.None)
		{
			GenericPromptHelper.ShowPrompt("Tutorial", "There is a tutorial on how to play on and interact with the server.<br>As a new player, we recommend you watch it.<br><br>Would you like to watch the tutorial?",
				"Watch Tutorial", "Skip Tutorial", UIEventID.OnWatchNewTutorial_Confirm_All, UIEventID.OnWatchNewTutorial_Cancel);
		}
		else
		{
			List<TutorialPhase> lstPhasesNew = DetermineRequiredTutorialPhases(currentTutorialVersion);
			List<TutorialPhase> lstPhasesAll = DetermineRequiredTutorialPhases(ETutorialVersions.None);
			int lengthInSecondsNew = (lstPhasesNew.Count * g_TimeBetweenPhasesMS) / 1000;
			int lengthInSecondsAll = (lstPhasesAll.Count * g_TimeBetweenPhasesMS) / 1000;

			string strLengthAll = ConvertSecondsToTimeString(lengthInSecondsAll);
			string strLengthNew = ConvertSecondsToTimeString(lengthInSecondsNew);

			string strNewSectionsDesc = String.Empty;
			foreach (ETutorialVersions version in Enum.GetValues(typeof(ETutorialVersions)))
			{
				if (version > currentTutorialVersion)
				{
					string strThisDesc = TutorialConstants.TutorialDescriptions[version];
					if (strNewSectionsDesc == String.Empty)
					{
						strNewSectionsDesc = Helpers.FormatString(" {0}", strThisDesc);
					}
					else
					{
						strNewSectionsDesc += Helpers.FormatString("<br> {0}", strThisDesc);
					}
				}
			}

			GenericPrompt3Helper.ShowPrompt("Tutorial", Helpers.FormatString("There is an updated server tutorial available. <br><br> New Sections include:<br>{0} <br><br> Entire Tutorial: {1}<br> New Sections Only: {2}", strNewSectionsDesc, strLengthAll, strLengthNew),
				"Watch Entire Tutorial", "Watch New Sections", "Skip Tutorial", UIEventID.OnWatchNewTutorial_Confirm_All, UIEventID.OnWatchNewTutorial_Confirm_New, UIEventID.OnWatchNewTutorial_Cancel);
		}
	}

	private static void GotoNextTutorialPhase(object[] arguments = null)
	{
		if (g_CurrentTutorialPhase != -1)
		{
			TutorialPhase prevTutorialPhase = m_lstPhases[g_CurrentTutorialPhase];
			prevTutorialPhase.Deactivate();
		}

		g_CurrentTutorialPhase++;

		if (g_CurrentTutorialPhase == m_lstPhases.Count)
		{
			RAGE.Game.Audio.PlaySoundFrontend(-1, "Success", "DLC_HEIST_HACKING_SNAKE_SOUNDS", true);
			FinishTutorialState();
		}
		else
		{
			RAGE.Game.Audio.PlaySoundFrontend(-1, "SELECT", "HUD_FREEMODE_SOUNDSET", true);

			TutorialPhase tutorialPhase = m_lstPhases[g_CurrentTutorialPhase];
			tutorialPhase.Activate();
		}
	}

	private static void FinishTutorialState()
	{
		// TODO_CSHARP: Probably a nicer way of doing this rather than calling into login system
		CameraManager.DeactivateCamera(ECameraID.TUTORIAL);
		LoginSystem.GotoLoginCamera();

		g_CurrentTutorialPhase = -1;
		NetworkEventSender.SendNetworkEvent_FinishTutorialState();

		AudioManager.FadeOutAudio(m_musicInst);
	}

	private static WeakReference<AudioInstance> m_musicInst = new WeakReference<AudioInstance>(null);
}

