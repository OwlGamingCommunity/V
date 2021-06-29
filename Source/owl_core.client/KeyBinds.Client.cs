using System;
using System.Collections.Generic;
using System.Linq;

// customizable controls
public class ScriptControlDescriptor
{
	public delegate void ScriptControlEventDelegate(EControlActionType actionType);

	public ScriptControlDescriptor(EScriptControlID a_ControlID, string a_Name, string a_Description, ConsoleKey a_DefaultKey, EKeyBindType a_KeyBindType, EKeyBindFlag a_KeyBindFlags)
	{
		DelegateToTrigger = null;
		ControlID = a_ControlID;
		Name = a_Name;
		Description = a_Description;
		CurrentKey = a_DefaultKey;
		DefaultKey = a_DefaultKey;
		KeyBindType = a_KeyBindType;
		KeyBindFlags = a_KeyBindFlags;
	}

	public EScriptControlID ControlID { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public ConsoleKey CurrentKey { get; set; }
	public ConsoleKey DefaultKey { get; set; }
	public EKeyBindType KeyBindType { get; set; }
	public EKeyBindFlag KeyBindFlags { get; set; }
	public ScriptControlEventDelegate DelegateToTrigger { get; set; }
}

public static class PlayerKeybinds
{
	public static List<PlayerKeybindObject> Keybinds = new List<PlayerKeybindObject>();
}

public static class ScriptControls
{
	public static bool SubscribeToControl(EScriptControlID a_ControlID, ScriptControlDescriptor.ScriptControlEventDelegate a_Delegate)
	{
		if (a_ControlID == EScriptControlID.DummyNone)
		{
			return false;
		}

		if (g_dictScriptControlDescriptors.ContainsKey(a_ControlID))
		{
			g_dictScriptControlDescriptors[a_ControlID].DelegateToTrigger += a_Delegate;
			return true;
		}
		else
		{
			// TODO: Error
		}

		return false;
	}

	public static void ResetToDefaults()
	{
		foreach (var kvPair in g_dictScriptControlDescriptors)
		{
			kvPair.Value.CurrentKey = kvPair.Value.DefaultKey;
		}
	}


	public static ConsoleKey GetKeyBoundToControl(EScriptControlID a_ControlID)
	{
		if (a_ControlID == EScriptControlID.DummyNone)
		{
			return ConsoleKey.NoName;
		}

		if (g_dictScriptControlDescriptors.ContainsKey(a_ControlID))
		{
			return g_dictScriptControlDescriptors[a_ControlID].CurrentKey;
		}

		return ConsoleKey.NoName;
	}

	public static Dictionary<EScriptControlID, ScriptControlDescriptor> g_dictScriptControlDescriptors = new Dictionary<EScriptControlID, ScriptControlDescriptor>()
	{
		{ EScriptControlID.EnterVehicleDriver, new ScriptControlDescriptor(EScriptControlID.EnterVehicleDriver, "Enter Vehicle (Driver)", "Enter a driver vehicle", ConsoleKey.F, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.EnterVehiclePassenger, new ScriptControlDescriptor(EScriptControlID.EnterVehiclePassenger, "Enter Vehicle (Passenger)", "Enter a passenger vehicle", ConsoleKey.G, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleCursor, new ScriptControlDescriptor(EScriptControlID.ToggleCursor, "Toggle Mouse Cursor", "Enables/Disables the mouse cursor used for UI interaction", ConsoleKey.M, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.HideCursor, new ScriptControlDescriptor(EScriptControlID.HideCursor, "Hide Mouse Cursor", "Hides the mouse cursor used for UI interaction", ConsoleKey.Escape, EKeyBindType.Pressed, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ToggleStatistics, new ScriptControlDescriptor(EScriptControlID.ToggleStatistics, "Toggle Statistics", "Changes the current statistics mode", ConsoleKey.F9, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ShowFullScreenBrowser, new ScriptControlDescriptor(EScriptControlID.ShowFullScreenBrowser, "Show Fullscreen Browser", "Shows the fullscreen browser when prompted", ConsoleKey.Insert, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.CloseFullScreenBrowser, new ScriptControlDescriptor(EScriptControlID.CloseFullScreenBrowser, "Close Fullscreen Browser", "Hides the fullscreen browser when visible", ConsoleKey.End, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ChangeMinimapMode, new ScriptControlDescriptor(EScriptControlID.ChangeMinimapMode, "Change Minimap Mode", "Changes the minimap mode", ConsoleKey.Z, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.GetPosition, new ScriptControlDescriptor(EScriptControlID.GetPosition, "Get Position", "Shows you your current position", ConsoleKey.Multiply, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowPlayerList, new ScriptControlDescriptor(EScriptControlID.ShowPlayerList, "Show Player List (Hold)", "Shows the player list", ConsoleKey.Tab, EKeyBindType.Both, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_Melee, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_Melee, "Show Weapon Selector (Melee)", "Shows the weapon selector for melee weapons", ConsoleKey.D1, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_Handguns, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_Handguns, "Show Weapon Selector (Handguns)", "Shows the weapon selector for handgun weapons", ConsoleKey.D2, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_SMG, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_SMG, "Show Weapon Selector (SMG)", "Shows the weapon selector for sub-machine gun weapons", ConsoleKey.D3, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_Rifle, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_Rifle, "Show Weapon Selector (Rifles)", "Shows the weapon selector for long rifle weapons", ConsoleKey.D4, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_MachineGun, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_MachineGun, "Show Weapon Selector (Machine Guns)", "Shows the weapon selector for machine gun weapons", ConsoleKey.D5, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_Shotgun, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_Shotgun, "Show Weapon Selector (Shotguns)", "Shows the weapon selector for shotgun weapons", ConsoleKey.D6, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_Sniper, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_Sniper, "Show Weapon Selector (Snipers)", "Shows the weapon selector for sniper rifles weapons", ConsoleKey.D7, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_RangedProjectile, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_RangedProjectile, "Show Weapon Selector (Projectiles)", "Shows the weapon selector for projectile weapons", ConsoleKey.D8, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_Throwable, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_Throwable, "Show Weapon Selector (Throwables)", "Shows the weapon selector for throwable weapons", ConsoleKey.D9, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowWeaponSelector_Special, new ScriptControlDescriptor(EScriptControlID.ShowWeaponSelector_Special, "Show Weapon Selector (Special)", "Shows the weapon selector for special weapons", ConsoleKey.D0, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.CancelAnimation, new ScriptControlDescriptor(EScriptControlID.CancelAnimation, "Cancel Animation", "Cancels the current animation", ConsoleKey.Spacebar, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowChatInput, new ScriptControlDescriptor(EScriptControlID.ShowChatInput, "Show Chat Input", "Shows the chat input box", ConsoleKey.T, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowChatInput_LocalOOC, new ScriptControlDescriptor(EScriptControlID.ShowChatInput_LocalOOC, "Show Chat Input (Local OOC)", "Shows the chat input box for Local OOC", ConsoleKey.B, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.HideChatInput, new ScriptControlDescriptor(EScriptControlID.HideChatInput, "Hide Chat Input", "Hides the chat input box", ConsoleKey.Escape, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.SubmitChatMessage, new ScriptControlDescriptor(EScriptControlID.SubmitChatMessage, "Submit Chat Input", "Sends the entered chat message/command", ConsoleKey.Enter, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ChatScrollUp, new ScriptControlDescriptor(EScriptControlID.ChatScrollUp, "Chat Scroll Up", "Scrolls the chat box contents up", ConsoleKey.PageUp, EKeyBindType.Pressed, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ChatScrollDown, new ScriptControlDescriptor(EScriptControlID.ChatScrollDown, "Chat Scroll Down", "Scrolls the chat box contents down", ConsoleKey.PageDown, EKeyBindType.Pressed, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ChatScrollHistoryUp, new ScriptControlDescriptor(EScriptControlID.ChatScrollHistoryUp, "Chat History Up", "Moves up to the next older chat message entered (when chat input is visible)", ConsoleKey.UpArrow, EKeyBindType.Pressed, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ChatScrollHistoryDown, new ScriptControlDescriptor(EScriptControlID.ChatScrollHistoryDown, "Chat History Down", "Moves up to the next newer chat message entered (when chat input is visible)", ConsoleKey.DownArrow, EKeyBindType.Pressed, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ToggleChatVisibility, new ScriptControlDescriptor(EScriptControlID.ToggleChatVisibility, "Toggle Chatbox", "Toggles the chat box visibility", ConsoleKey.F7, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ToggleEngine, new ScriptControlDescriptor(EScriptControlID.ToggleEngine, "Toggle Engine", "Turns the engine on/off when in a vehicle", ConsoleKey.J, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleLock, new ScriptControlDescriptor(EScriptControlID.ToggleLock, "Toggle Lock", "Locks/unlocks nearby vehicles & property when out of a vehicle, or the current vehicle.", ConsoleKey.K, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleHeadlights, new ScriptControlDescriptor(EScriptControlID.ToggleHeadlights, "Toggle Headlights", "Turns the headlights on/off when in a vehicle", ConsoleKey.L, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.LeftTurnSignal, new ScriptControlDescriptor(EScriptControlID.LeftTurnSignal, "Toggle Left Turn Signal", "Turns the left turn signal on/off when in a vehicle", ConsoleKey.Oem4, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.RightTurnSignal, new ScriptControlDescriptor(EScriptControlID.RightTurnSignal, "Toggle Right Turn Signal", "Turns the right turn signal on/off when in a vehicle", ConsoleKey.Oem6, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleHelpCenter, new ScriptControlDescriptor(EScriptControlID.ToggleHelpCenter, "Toggle Help Center", "Toggles the Help Center UI", ConsoleKey.F2, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ToggleFactionUI, new ScriptControlDescriptor(EScriptControlID.ToggleFactionUI, "Toggle Faction UI", "Toggles the Faction UI", ConsoleKey.F3, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.DropWaterFromFireHeli, new ScriptControlDescriptor(EScriptControlID.DropWaterFromFireHeli, "Drop Water", "Drops water when in a Fire Department Helicopter", ConsoleKey.NumPad5, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.TogglePoliceSearchlight, new ScriptControlDescriptor(EScriptControlID.TogglePoliceSearchlight, "Toggle Searchlight", "Turns the searchlight on/off when in a police vehicle", ConsoleKey.NumPad5, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleSirensMode, new ScriptControlDescriptor(EScriptControlID.ToggleSirensMode, "Toggle Sirens Mode", "Sets the siren to loud/silent when in an emergency vehicle", ConsoleKey.F6, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleInventory, new ScriptControlDescriptor(EScriptControlID.ToggleInventory, "Toggle Inventory", "Toggles the Inventory UI", ConsoleKey.I, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ToggleDonations, new ScriptControlDescriptor(EScriptControlID.ToggleDonations, "Toggle Donations", "Toggles the Donations UI", ConsoleKey.F4, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.Interact, new ScriptControlDescriptor(EScriptControlID.Interact, "Interact", "Interact with world prompts (e.g. enter property, access store, go on duty", ConsoleKey.E, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ShowChatInput_PrimaryRadio, new ScriptControlDescriptor(EScriptControlID.ShowChatInput_PrimaryRadio, "Show Chat Input (Primary Radio)", "Shows the chat input box for the primary radio", ConsoleKey.Y, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleCruiseControl, new ScriptControlDescriptor(EScriptControlID.ToggleCruiseControl, "Toggle Cruise Control", "Toggles cruise control", ConsoleKey.F8, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleCrouch, new ScriptControlDescriptor(EScriptControlID.ToggleCrouch, "Toggle Crouch", "Toggles crouch mode", ConsoleKey.DownArrow, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.BlipSiren, new ScriptControlDescriptor(EScriptControlID.BlipSiren, "Blip Siren", "Blips the vehicle siren", ConsoleKey.Backspace, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.TakeScreenshot, new ScriptControlDescriptor(EScriptControlID.TakeScreenshot, "Toggle screenshot HUD", "Toggles the HUD for screenshots", ConsoleKey.OemMinus, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.TogglePhone, new ScriptControlDescriptor(EScriptControlID.TogglePhone, "Toggle last used phone", "Toggles the last used phone", ConsoleKey.UpArrow, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ChatScrollToStart, new ScriptControlDescriptor(EScriptControlID.ChatScrollToStart, "Chat Scroll To Start", "Scrolls the chat box contents up to the oldest message", ConsoleKey.Home, EKeyBindType.Pressed, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ChatScrollToEnd, new ScriptControlDescriptor(EScriptControlID.ChatScrollToEnd, "Chat Scroll To End", "Scrolls the chat box contents down to the most recent message", ConsoleKey.End, EKeyBindType.Pressed, EKeyBindFlag.AllowWhenKeybindsDisabled) },
		{ EScriptControlID.ToggleBinocularView, new ScriptControlDescriptor(EScriptControlID.ToggleBinocularView, "Toggle binocular view", "Toggles the binocular view when holding a binoculars ", ConsoleKey.E, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleBinocularFx, new ScriptControlDescriptor(EScriptControlID.ToggleBinocularFx, "Toggle binocular FX", "Toggles FX when holding a binoculars ", ConsoleKey.X, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleWindows, new ScriptControlDescriptor(EScriptControlID.ToggleWindows, "Toggle vehicle windows", "Toggles the windows up and down ", ConsoleKey.X, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleHandbrake, new ScriptControlDescriptor(EScriptControlID.ToggleHandbrake, "Toggle vehicle handbrake", "Toggles the vehicle handbrake ", ConsoleKey.G, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleFireMode, new ScriptControlDescriptor(EScriptControlID.ToggleFireMode, "Toggle fire mode", "Toggles the fire mode of your weapon", ConsoleKey.N, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.ToggleTrainDoors, new ScriptControlDescriptor(EScriptControlID.ToggleTrainDoors, "Control Train Doors", "Controls the train doors when driving a train", ConsoleKey.D, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.TrainAccelerate, new ScriptControlDescriptor(EScriptControlID.TrainAccelerate, "Train Accelerate", "Accelerates when driving a train", ConsoleKey.W, EKeyBindType.Released, EKeyBindFlag.Default) },
		{ EScriptControlID.TrainDecelerate, new ScriptControlDescriptor(EScriptControlID.TrainDecelerate, "Train Decelerate", "Decelerates when driving a train", ConsoleKey.S, EKeyBindType.Released, EKeyBindFlag.Default) }

	};
}

// end customizable controls

public enum EControlActionType
{
	Pressed,
	Released
}

public enum EKeyBindType
{
	Pressed,
	Released,
	Both
}

[Flags]
public enum EKeyBindFlag
{
	Default = 0x0,
	AllowWhenKeybindsDisabled = 0x1,
}

public static class KeyBinds
{
	public delegate void EventKeyDelegate();
	class KeyBindDescriptor
	{
		public KeyBindDescriptor(EventKeyDelegate dele, EKeyBindFlag a_Flags)
		{
			DelegateToTrigger = dele;
			Flags = a_Flags;
		}

		public EventKeyDelegate DelegateToTrigger { get; }
		public EKeyBindFlag Flags { get; }
	}

	static private Dictionary<ConsoleKey, List<KeyBindDescriptor>> KeyEventsPressed { get; set; } = new Dictionary<ConsoleKey, List<KeyBindDescriptor>>();
	static private Dictionary<ConsoleKey, List<KeyBindDescriptor>> KeyEventsReleased { get; set; } = new Dictionary<ConsoleKey, List<KeyBindDescriptor>>();
	static private List<ConsoleKey> KeysJustPressed { get; } = new List<ConsoleKey>();
	static private List<ConsoleKey> KeysJustReleased { get; } = new List<ConsoleKey>();

	private static void SetDefaults()
	{
		// TODO_CSHARP: Make this class non static, add a static getter and remove this init hack
		if (KeyEventsPressed.Count == 0)
		{
			foreach (ConsoleKey key in (ConsoleKey[])Enum.GetValues(typeof(ConsoleKey)))
			{
				KeyEventsPressed.Add(key, new List<KeyBindDescriptor>());
			}
		}

		if (KeyEventsReleased.Count == 0)
		{
			foreach (ConsoleKey key in (ConsoleKey[])Enum.GetValues(typeof(ConsoleKey)))
			{
				KeyEventsReleased.Add(key, new List<KeyBindDescriptor>());
			}
		}

		g_iKeybindsDisabledRefCount = 0;
		g_iGUIInputEnabledRefCount = 0;
	}

	static KeyBinds()
	{

	}

	class CKeyState
	{
		public int key { get; set; }
		public bool state { get; set; }
	}

	//private static Dictionary<int, bool> g_InternalKeyStates = new Dictionary<int, bool>();

	public static void Init()
	{
		SetDefaults();

		NetworkEvents.CharacterSelectionApproved += () =>
		{
			SetDefaults();
		};
		/*
		NetworkEvents.LocalKeysState += (string strData) =>
		{
			List<CKeyState> lstKeyStates = OwlJSON.DeserializeObject<List<CKeyState>>(strData);

			foreach (CKeyState keyState in lstKeyStates)
			{
				g_InternalKeyStates[keyState.key] = keyState.state;
			}
		};
		*/
	}

	// Useful for fishing or failing minigames
	public static bool IsAnyConsoleKeyDown(ConsoleKey ignoreKey, out ConsoleKey keyDown)
	{
		keyDown = ConsoleKey.NoName;

		bool bKeyFound = false;
		foreach (ConsoleKey key in (ConsoleKey[])Enum.GetValues(typeof(ConsoleKey)))
		{
			if (key != ignoreKey)
			{
				if (IsConsoleKeyDown(key))
				{
					keyDown = key;
					bKeyFound = true;
					break;
				}
			}
		}

		return bKeyFound;
	}

	public static bool IsMouseButtonDown(int button)
	{
		return RAGE.Input.IsDown(button) && RAGE.Ui.Windows.Focused;
	}

	public static bool IsKeyDown(int key)
	{
		return RAGE.Input.IsDown(key) && RAGE.Ui.Windows.Focused;
	}

	public static bool IsKeyUp(int key)
	{
		return RAGE.Input.IsUp(key) || !RAGE.Ui.Windows.Focused;
	}

	public static bool IsConsoleKeyDown(ConsoleKey key)
	{
		return RAGE.Input.IsDown((int)key) && RAGE.Ui.Windows.Focused;
	}

	public static bool IsConsoleKeyUp(ConsoleKey key)
	{
		return RAGE.Input.IsUp((int)key) || !RAGE.Ui.Windows.Focused;
	}

	// TODO_CSHARP: When chat is ported over, this variable should belong to chat
	static private bool g_bChatInputShowing = false;
	static private int g_iKeybindsDisabledRefCount = 0;
	static private int g_iGUIInputEnabledRefCount = 0;

	static public bool CanProcessKeybinds()
	{
		// TODO_CSHARP: Clientside player class which lets us get eneity data using enum
		bool bSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		bool bCursorVisible = CursorManager.IsCursorVisible();
		return (bSpawned && !bCursorVisible && !IsChatInputVisible() && !AreKeybindsDisabled() && !IsPauseMenuActive());
	}

	static private bool AreKeybindsDisabled()
	{
		return g_iKeybindsDisabledRefCount > 0;
	}

	static private bool IsGUIInputEnabled()
	{
		return g_iGUIInputEnabledRefCount > 0;
	}

	static public bool IsChatInputVisible()
	{
		return g_bChatInputShowing;
	}

	static public void SetKeybindsDisabled(bool a_bKeybindsDisabled)
	{
		if (a_bKeybindsDisabled)
		{
			g_iKeybindsDisabledRefCount++;

			// was zero, now enabling
			if (g_iKeybindsDisabledRefCount == 0)
			{
				NetworkEvents.SendLocalEvent_InputEnabledChanged(!IsChatInputVisible() && !AreKeybindsDisabled());
			}
		}
		else
		{
			g_iKeybindsDisabledRefCount--;

			if (g_iKeybindsDisabledRefCount < 0)
			{
				g_iKeybindsDisabledRefCount = 0;
			}

			// was > 0, now 0
			if (g_iKeybindsDisabledRefCount == 0)
			{
				NetworkEvents.SendLocalEvent_InputEnabledChanged(!IsChatInputVisible() && !AreKeybindsDisabled());
			}
		}
	}

	static public void SetGUIInputEnabled(bool a_bUIInputEnabled)
	{
		if (a_bUIInputEnabled)
		{
			g_iGUIInputEnabledRefCount++;

			// was zero, now enabling
			if (g_iGUIInputEnabledRefCount == 1)
			{
				NetworkEvents.SendLocalEvent_InputEnabledChanged(!IsChatInputVisible() && !AreKeybindsDisabled());
			}
		}
		else
		{
			g_iGUIInputEnabledRefCount--;

			if (g_iGUIInputEnabledRefCount < 0)
			{
				g_iGUIInputEnabledRefCount = 0;
			}

			// was > 0, now 0
			if (g_iGUIInputEnabledRefCount == 0)
			{
				NetworkEvents.SendLocalEvent_InputEnabledChanged(!IsChatInputVisible() && !AreKeybindsDisabled());
			}
		}
	}

	static public void OnChatShowInput()
	{
		g_bChatInputShowing = true;
	}

	static public void OnChatHideInput()
	{
		g_bChatInputShowing = false;
	}

	static public void Unbind(ConsoleKey key, EventKeyDelegate a_Functor, EKeyBindType a_Type, EKeyBindFlag a_Flags)
	{
		// TODO_CSHARP: We cant specify resource loading order, so this breaks, hence the hack below
		SetDefaults();

		if (a_Functor != null)
		{
			if (a_Type == EKeyBindType.Pressed || a_Type == EKeyBindType.Both)
			{
				KeyEventsPressed[key].RemoveAll(bind => bind.DelegateToTrigger == a_Functor && bind.Flags == a_Flags);
			}

			if (a_Type == EKeyBindType.Released || a_Type == EKeyBindType.Both)
			{
				KeyEventsReleased[key].RemoveAll(bind => bind.DelegateToTrigger == a_Functor && bind.Flags == a_Flags);
			}
		}
	}

	static public void Bind(ConsoleKey key, EventKeyDelegate a_Functor, EKeyBindType a_Type, EKeyBindFlag a_Flags)
	{
		// TODO_CSHARP: We cant specify resource loading order, so this breaks, hence the hack below
		SetDefaults();

		if (a_Functor != null)
		{
			if (a_Type == EKeyBindType.Pressed || a_Type == EKeyBindType.Both)
			{
				KeyEventsPressed[key].Add(new KeyBindDescriptor(a_Functor, a_Flags));
			}

			if (a_Type == EKeyBindType.Released || a_Type == EKeyBindType.Both)
			{
				KeyEventsReleased[key].Add(new KeyBindDescriptor(a_Functor, a_Flags));
			}
		}
	}

	static public bool WasKeyJustPressed(ConsoleKey key)
	{
		return KeysJustPressed.Contains(key);
	}

	static public bool WasKeyJustReleased(ConsoleKey key)
	{
		return KeysJustReleased.Contains(key);
	}

	static public bool IsPauseMenuActive()
	{
		return RAGE.Game.Ui.GetPauseMenuState() > 0;
	}

	static public void OnKeyPressed(ConsoleKey key)
	{
		SetDefaults();

		KeysJustPressed.Add(key);

		// TODO_CSHARP: A way of saying only if keybinds enabled?
		if (KeyEventsPressed.ContainsKey(key))
		{
			foreach (KeyBindDescriptor desc in KeyEventsPressed[key])
			{
				if (CanProcessKeybinds() || desc.Flags.HasFlag(EKeyBindFlag.AllowWhenKeybindsDisabled))
				{
					if (!IsGUIInputEnabled() && !IsPauseMenuActive())
					{
						desc.DelegateToTrigger?.Invoke();
					}
				}
			}
		}

		// Check controls
		foreach (var controlDescKvPair in ScriptControls.g_dictScriptControlDescriptors)
		{
			var controlDesc = controlDescKvPair.Value;
			if (controlDesc.CurrentKey == key)
			{
				if (controlDesc.KeyBindType == EKeyBindType.Pressed || controlDesc.KeyBindType == EKeyBindType.Both)
				{
					if (CanProcessKeybinds() || controlDesc.KeyBindFlags.HasFlag(EKeyBindFlag.AllowWhenKeybindsDisabled))
					{
						if (!IsGUIInputEnabled() && IsFishingAndKeyAllowed(controlDesc.CurrentKey) && IsReconningAndControlAllowed(controlDesc.ControlID) && !IsPauseMenuActive())
						{
							controlDesc.DelegateToTrigger?.Invoke(EControlActionType.Pressed);
						}
					}
				}
			}
		}
	}

	// TODO_CONTROLS: Better way of disabling controls temporarily
	static private bool IsFishingAndKeyAllowed(ConsoleKey keyID)
	{
		bool bIsFishing = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.FISHING);

		// if not reconning, just say its allowed
		if (!bIsFishing)
		{
			return true;
		}

		if (keyID == ConsoleKey.UpArrow
			|| keyID == ConsoleKey.DownArrow
			|| keyID == ConsoleKey.LeftArrow
			|| keyID == ConsoleKey.RightArrow)
		{
			return false;
		}

		return true;
	}

	static private bool IsReconningAndControlAllowed(EScriptControlID controlID)
	{
		bool bIsReconning = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.RECON);

		// if not reconning, just say its allowed
		if (!bIsReconning)
		{
			return true;
		}

		if (controlID == EScriptControlID.ChatScrollToEnd
			|| controlID == EScriptControlID.ChatScrollToStart
			|| controlID == EScriptControlID.ToggleCursor
			|| controlID == EScriptControlID.HideCursor
			|| controlID == EScriptControlID.ToggleStatistics
			|| controlID == EScriptControlID.ShowFullScreenBrowser
			|| controlID == EScriptControlID.CloseFullScreenBrowser
			|| controlID == EScriptControlID.ChangeMinimapMode
			|| controlID == EScriptControlID.GetPosition
			|| controlID == EScriptControlID.ShowPlayerList
			|| controlID == EScriptControlID.ShowChatInput
			|| controlID == EScriptControlID.ShowChatInput_LocalOOC
			|| controlID == EScriptControlID.ShowChatInput_PrimaryRadio
			|| controlID == EScriptControlID.HideChatInput
			|| controlID == EScriptControlID.SubmitChatMessage
			|| controlID == EScriptControlID.ChatScrollUp
			|| controlID == EScriptControlID.ChatScrollDown
			|| controlID == EScriptControlID.ChatScrollHistoryUp
			|| controlID == EScriptControlID.ChatScrollHistoryDown
			|| controlID == EScriptControlID.ToggleChatVisibility
			|| controlID == EScriptControlID.ToggleHelpCenter
			|| controlID == EScriptControlID.ToggleFactionUI
			|| controlID == EScriptControlID.ToggleInventory
			|| controlID == EScriptControlID.ToggleDonations)
		{
			return true;
		}

		// reconning and control isn't allowed
		return false;
	}

	static public void OnKeyReleased(ConsoleKey key)
	{
		SetDefaults();

		KeysJustReleased.Add(key);

		if (KeyEventsReleased.ContainsKey(key))
		{
			foreach (KeyBindDescriptor desc in KeyEventsReleased[key])
			{
				if (CanProcessKeybinds() || desc.Flags.HasFlag(EKeyBindFlag.AllowWhenKeybindsDisabled))
				{
					if (!IsGUIInputEnabled() && !IsPauseMenuActive())
					{
						desc.DelegateToTrigger?.Invoke();
					}
				}
			}
		}

		// Check controls
		foreach (var controlDescKvPair in ScriptControls.g_dictScriptControlDescriptors)
		{
			var controlDesc = controlDescKvPair.Value;
			if (controlDesc.CurrentKey == key)
			{
				if (controlDesc.KeyBindType == EKeyBindType.Released || controlDesc.KeyBindType == EKeyBindType.Both)
				{
					if (CanProcessKeybinds() || controlDesc.KeyBindFlags.HasFlag(EKeyBindFlag.AllowWhenKeybindsDisabled))
					{
						if (!IsGUIInputEnabled() && IsFishingAndKeyAllowed(controlDesc.CurrentKey) && IsReconningAndControlAllowed(controlDesc.ControlID) && !IsPauseMenuActive())
						{
							controlDesc.DelegateToTrigger?.Invoke(EControlActionType.Released);
						}
					}
				}
			}
		}

		// Check keybinds (only on release)
		int index = 0;
		foreach (PlayerKeybindObject keybind in PlayerKeybinds.Keybinds)
		{
			if (keybind.Key == key)
			{
				if (CanProcessKeybinds())
				{
					if (!IsGUIInputEnabled())
					{
						NetworkEventSender.SendNetworkEvent_TriggerKeybind(index);
					}
				}
			}
			++index;
		}
	}

	static public void ResetWasJustState()
	{
		KeysJustPressed.Clear();
		KeysJustReleased.Clear();
	}
}
class KeyBindManager
{
	internal Dictionary<ConsoleKey, bool> KeyList = new Dictionary<ConsoleKey, bool>();

	public KeyBindManager()
	{
		foreach (ConsoleKey key in (ConsoleKey[])Enum.GetValues(typeof(ConsoleKey)))
		{
			KeyList.Add(key, false);
		}

		RageEvents.RAGE_OnTick_PerFrame += OnUpdate;
	}

	private void OnUpdate()
	{
		try
		{
			KeyBinds.ResetWasJustState();

			foreach (KeyValuePair<ConsoleKey, bool> key in KeyList.ToList())
			{
				if (KeyList[key.Key] && !KeyBinds.IsConsoleKeyDown(key.Key))
				{
					KeyList[key.Key] = false;
					KeyBinds.OnKeyReleased(key.Key);
				}
				else if (!KeyList[key.Key] && KeyBinds.IsConsoleKeyDown(key.Key))
				{
					KeyList[key.Key] = true;
					KeyBinds.OnKeyPressed(key.Key);
				}
			}
		}
		catch (Exception ex)
		{
			ExceptionHelper.SendException(ex);
		}
	}
}



