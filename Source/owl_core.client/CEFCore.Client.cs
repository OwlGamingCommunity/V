//#define NUKE_CEF_ON_DISCONNECT
using System;
using System.Collections;
using System.Collections.Generic;

public delegate void OnGUILoadedDelegate();

[Flags]
public enum ExecutionFlags
{
	DelayedExecution = 0x0,
	ExecuteInstantly = 0x1,
	OverwriteDuplicateCommands = 0x2,
}


public enum EGUIID
{
	None = -1,
	AudioManager,
	HUD,
	Keybindmanager,
	Notifications,
	PlayerRadialMenu,
	GenericDropdown,
	GenericListBox,
	GenericMessageBox,
	GenericPrompt,
	GenericPrompt3,
	PlayerList,
	ShardManager,
	UserInputHelpers,
	TowGetVehicle,
	VehicleModShop,
	EditInterior,
	Frisk,
	GangTagCreator,
	PlayerInventory,
	SprayCanPrompt,
	AchievementOverlay,
	AdminCheck,
	HelpCenter,
	ViewApplication,
	BankingInterface,
	Cellphone,
	Chatbox,
	TaxiUI,
	PurchaseProperty,
	ClothingStore,
	FurnitureStore,
	TattooArtist,
	Barber,
	Store,
	Duty,
	FactionInvite,
	FactionManagement,
	PDLicensingDevice,
	MDCPerson,
	MDCProperty,
	MDCVehicle,
	PDTicket,
	PDVehicleHud,
	Donations,
	RadioManagement,
	VehicleRentalStore,
	VehicleStore,
	AchievementsList,
	Application,
	CharacterCreation,
	CharacterList,
	Login,
	Register,
	PaydayOverview,
	RadioPlayer,
	PlasticSurgeon,
	MarijuanaPed,
	ScooterRentalStore,
	VehicleCrusher,
	Languages,
	AdminCheckVeh,
	AdminCheckInt,
	AnimationsList,
	MapLoader,
	GenericProgressBar,
	AdminReports,
	WeatherInfo,
	RoadblockEditor,
	GenericCreator,
	VehiclesList,
	ItemsList,
	ItemMover,
	OutfitEditor,
	DutyOutfitEditor,
	GenericDataTable,
	PDHelicopterHud,
	ShowCharacterLook,
	UpdateCharacterLook,
	GenericUserLogin,
	AssetTransfer
}

public abstract class CEFCore
{
	private class PendingJSExecution
	{
		public PendingJSExecution(string strCmdName, object[] args)
		{
			CommandName = strCmdName;
			Arguments = args;
		}

		public void Execute(RAGE.Ui.HtmlWindow a_Browser)
		{
			// TODO_CSHARP: Auto serialize complex types, check callers for usage
			string strInvokeCmd = Helpers.FormatString("{0}(", CommandName);
			int index = 0;
			foreach (var arg in Arguments)
			{
				if (index > 0)
				{
					strInvokeCmd += ", ";
				}

				if (arg == null)
				{
					continue;
				}

				if (arg.GetType() == typeof(string))
				{
					// Must put string in quotations, and replace any " inside the string with \"
					strInvokeCmd += Helpers.FormatString("`{0}`", arg.ToString());
				}
				else if (arg.GetType() == typeof(bool)) // FIX for 'True' not being compatible with JS
				{
					strInvokeCmd += arg.ToString().ToLower();
				}
				else if (arg.GetType() == typeof(float) || arg.GetType() == typeof(decimal)) // FIX for yuropeans having 0,05 instead of 0.05...
				{
					strInvokeCmd += Helpers.FormatString("{0:0.##}", arg);
				}
				else if (arg.GetType().IsEnum)
				{
					strInvokeCmd += Convert.ToInt32(arg);
				}
				// TODO: Better way of checking
				else if (arg.GetType().IsArray) // Arrays must be serialized into Javascript arrays
				{
					strInvokeCmd += CreateArrayAsString((Array)arg);
				}
				else if (arg.GetType() is IList) // Containers must be serialized into Javascript arrays
				{
					strInvokeCmd += CreateListAsString((IList)arg);
				}
				else
				{
					strInvokeCmd += arg;
				}

				++index;
			}
			strInvokeCmd += ");";

			a_Browser.ExecuteJs(strInvokeCmd);
		}

		private string CreateListAsString(IList list)
		{
			string stringbuilder = "[";

			int i = 0;
			foreach (var item in list)
			{
				if (i > 0)
				{
					stringbuilder += ", ";
				}

				stringbuilder += item.ToString();

				++i;
			}

			stringbuilder += "]";

			return stringbuilder;
		}

		private string CreateArrayAsString(Array arr)
		{
			string stringbuilder = "[";

			for (var i = 0; i < arr.Length; ++i)
			{

				if (i > 0)
				{
					stringbuilder += ", ";
				}

				string strVal = arr.GetValue(i).ToString();
				if (arr.GetValue(i).GetType().IsEnum)
				{
					strVal = Convert.ToString(Convert.ToInt32(arr.GetValue(i)));
				}

				stringbuilder += strVal;
			}

			stringbuilder += "]";

			return stringbuilder;
		}

		public string CommandName { get; set; }
		public object[] Arguments { get; set; }
	}

	private string m_strPath = String.Empty;
	private EGUIID m_guiID = EGUIID.None;

	private bool m_bReloading = false;

	private bool m_bPendingShow = false;
	private bool m_bPendingShow_CursorAndInputEnabled = false;
	private bool m_bPendingShow_AffectChatVisibility = false;

	private bool m_bInputEnabled = false;

	private List<PendingJSExecution> m_lstCachedExecutions = new List<PendingJSExecution>();

	// TODO_CSHARP: Creation of browser should be tied to loading, OR fast mode
	// TODO_CSHARP: Because this isn't tied to loading, loading will never finish until player spawns
	public CEFCore(string path, EGUIID guiID, OnGUILoadedDelegate callbackOnLoad, bool bImmuneToNuking = false)
	{
		m_guiID = guiID;
		m_strPath = Helpers.FormatString("package://{0}#{1}", path, guiID);

		NetworkEvents.CursorStateChange += OnCursorStateChange;
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;
		UIEvents.OnHTMLLoaded += OnBrowserLoaded;

		RageEvents.RAGE_OnTick_OncePerSecond += OnTick_OncePerSecond;

		m_CallbackOnLoad = callbackOnLoad;
		m_bImmuneToNuking = bImmuneToNuking;

		UIEvents.OnDisconnectedFromServer += OnRAGEDisconnectedFromServer;

	}

	public void OnBrowserLoaded(string strGuiID, string strFullURI)
	{
		object outGuidObject;
		if (Enum.TryParse(typeof(EGUIID), strGuiID, out outGuidObject))
		{
			EGUIID guiID = (EGUIID)outGuidObject;

			if (guiID == m_guiID)
			{
				m_bReloading = false;

				NotifyLoadComplete();

				if (m_bPendingShow)
				{
					SetVisible(true, m_bPendingShow_CursorAndInputEnabled, m_bPendingShow_AffectChatVisibility);
				}

				// execute pending commands
				foreach (PendingJSExecution pendingJSExecution in m_lstCachedExecutions)
				{
					pendingJSExecution.Execute(m_CEFBrowser);
				}
			}
		}
	}

	private void OnTick_OncePerSecond()
	{
		const int highThreshold = 10;
		if (m_CEFBrowser != null && m_lstCachedExecutions.Count > highThreshold)
		{
			EventManager.RegisterHighExecutionUI(m_CEFBrowser.Url, m_lstCachedExecutions.Count);
			PerfManager.RegisterHighExecutionUI(m_CEFBrowser.Url, m_lstCachedExecutions.Count);
		}

		if (m_CEFBrowser != null)
		{
			foreach (PendingJSExecution pendingJSExecution in m_lstCachedExecutions)
			{
				pendingJSExecution.Execute(m_CEFBrowser);
			}
		}

		m_lstCachedExecutions.Clear();
	}

	private void OnTick()
	{
		if (CursorManager.IsCursorVisible() != m_bInputEnabled)
		{
			m_bInputEnabled = CursorManager.IsCursorVisible();
			OnCursorStateChange(CursorManager.IsCursorVisible());
		}
	}

	private void OnRAGEDisconnectedFromServer()
	{
#if NUKE_CEF_ON_DISCONNECT
		if (m_CEFBrowser != null && !m_bImmuneToNuking)
		{
			m_CEFBrowser.Destroy();
			m_CEFBrowser = null;
		}
#endif
	}

	// TODO_CSHARP: Check all set visible calls come after executes if needed
	public void NotifyLoadComplete()
	{
		OnLoad();
		m_CallbackOnLoad?.Invoke();
	}

	public abstract void OnLoad();

	public void BeginLoad()
	{
		if (m_CEFBrowser == null)
		{
			m_CEFBrowser = new RAGE.Ui.HtmlWindow(m_strPath);
			m_CEFBrowser.Active = false;
		}
	}

	~CEFCore()
	{
		if (m_CEFBrowser != null)
		{
			m_CEFBrowser.Destroy();
		}
	}

	public void MarkAsChat()
	{
		if (m_CEFBrowser != null)
		{
			m_CEFBrowser.MarkAsChat();
		}
	}

	private void OnCursorStateChange(bool bNewState)
	{
		SetInputEnabled(bNewState);
	}

	public void Reload()
	{
		if (m_CEFBrowser != null)
		{
			m_CEFBrowser.Destroy();

			m_bReloading = true;
			m_CEFBrowser = new RAGE.Ui.HtmlWindow(m_strPath);
			m_CEFBrowser.Active = false;
		}
	}

	public void SetVisible(bool bVisible, bool bCursorAndInputEnabled, bool bAffectChatVisibility)
	{
		if (bVisible && m_CEFBrowser == null)
		{
			// on demand loading
			BeginLoad();
			m_bPendingShow = true;
			m_bPendingShow_CursorAndInputEnabled = bCursorAndInputEnabled;
			m_bPendingShow_AffectChatVisibility = bAffectChatVisibility;
		}

		if (!bVisible && m_CEFBrowser != null)
		{
			// TODO_CSHARP: Fix this
			//m_CEFBrowser.Destroy();
			//m_CEFBrowser = null;

			m_bPendingShow = false;
			m_bPendingShow_CursorAndInputEnabled = false;
			m_bPendingShow_AffectChatVisibility = false;
		}

		if (m_CEFBrowser != null && m_CEFBrowser.Active != bVisible)
		{
			m_CEFBrowser.Active = bVisible;

			KeyBinds.SetKeybindsDisabled(bCursorAndInputEnabled);
			CursorManager.SetCursorVisible(bCursorAndInputEnabled, this);

			if (bAffectChatVisibility)
			{
				RAGE.Chat.Show(!bVisible);
			}

			SetInputEnabled(bCursorAndInputEnabled);
		}
	}

	public void SetInputEnabled(bool bInputEnabled)
	{
		KeyBinds.SetGUIInputEnabled(bInputEnabled);
		Execute("SetInputEnabled", bInputEnabled);
	}

	public bool IsVisible()
	{
		if (m_CEFBrowser != null)
		{
			return m_CEFBrowser.Active;
		}

		return false;
	}

	public void Execute(string functionName, params object[] args)
	{
		Execute(functionName, ExecutionFlags.ExecuteInstantly, args);
	}

	public void ExecuteDelayed_KeepDupes(string functionName, params object[] args)
	{
		Execute(functionName, ExecutionFlags.DelayedExecution, args);
	}

	public void ExecuteDelayed_OverwriteDupes(string functionName, params object[] args)
	{
		Execute(functionName, ExecutionFlags.DelayedExecution | ExecutionFlags.OverwriteDuplicateCommands, args);
	}

	private void Execute(string functionName, ExecutionFlags execFlags, params object[] args)
	{
		PendingJSExecution pendingExec = new PendingJSExecution(functionName, args);

		if (execFlags.HasFlag(ExecutionFlags.OverwriteDuplicateCommands))
		{
			m_lstCachedExecutions.RemoveAll(existingExecution => existingExecution.CommandName == functionName);
		}

		if (m_CEFBrowser == null || m_bReloading)
		{
			// Cache it instead for execution on set visible
			// this is a fix for instances where people might want to pre-populate the instance before its visible, and if its set to on demand, the browser won't exist at all.
			m_lstCachedExecutions.Add(pendingExec);
			return;
		}
		else
		{
			if (execFlags.HasFlag(ExecutionFlags.ExecuteInstantly))
			{
				// TODO
				pendingExec.Execute(m_CEFBrowser);
			}
			else if (execFlags.HasFlag(ExecutionFlags.DelayedExecution))
			{
				m_lstCachedExecutions.Add(pendingExec);
			}
		}
	}

	public void SetCursorAndInputEnabled(bool bCursorAndInputEnabled)
	{
		if (m_CEFBrowser != null)
		{
			KeyBinds.SetKeybindsDisabled(bCursorAndInputEnabled);
			CursorManager.SetCursorVisible(bCursorAndInputEnabled, this);
			SetInputEnabled(bCursorAndInputEnabled);
		}
		else
		{
			m_bPendingShow_CursorAndInputEnabled = bCursorAndInputEnabled;
		}
	}

	public RAGE.Ui.HtmlWindow m_CEFBrowser = null;
	private OnGUILoadedDelegate m_CallbackOnLoad = null;
	private bool m_bImmuneToNuking = false;
}