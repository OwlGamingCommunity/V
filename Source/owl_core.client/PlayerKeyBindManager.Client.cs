using System;
using System.Collections.Generic;

public class PlayerKeyBindManager
{
	private CGUIKeyBindManager m_KeyBindManagerUI = new CGUIKeyBindManager(OnUILoaded);

	public PlayerKeyBindManager()
	{
		NetworkEvents.ChangeCharacterApproved += Hide;
		NetworkEvents.ShowKeyBindManager += () => { Show(false); };

		NetworkEvents.ApplyCustomControls += OnApplyCustomControls;
		NetworkEvents.ApplyPlayerKeybinds += OnApplyPlayerKeybinds;

		KeyBinds.Bind(ConsoleKey.F5, Toggle, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
	}

	private static void OnUILoaded()
	{

	}

	private void OnApplyCustomControls(List<GameControlObject> lstControls)
	{
		// Reset to defaults first
		ScriptControls.ResetToDefaults();

		foreach (GameControlObject control in lstControls)
		{
			if (ScriptControls.g_dictScriptControlDescriptors.ContainsKey(control.C))
			{
				ConsoleKey currentKey = ScriptControls.g_dictScriptControlDescriptors[control.C].CurrentKey;
				ConsoleKey newKey = control.K;

				ScriptControls.g_dictScriptControlDescriptors[control.C].CurrentKey = newKey;

				// NOTE: this MUST fire after the key is updated, some callbacks reference g_dictScriptControlDescriptors directly
				NetworkEvents.SendLocalEvent_OnScriptControlChanged(control.C, currentKey, newKey);
			}
		}
	}

	private void OnApplyPlayerKeybinds(List<PlayerKeybindObject> lstKeybinds)
	{
		PlayerKeybinds.Keybinds = lstKeybinds;
	}

	public void Show(bool bGotoKeybinds)
	{
		m_KeyBindManagerUI.Reset();

		foreach (ConsoleKey k in Enum.GetValues(typeof(ConsoleKey)))
		{
			m_KeyBindManagerUI.RegisterKeyCode((int)k, k.ToString());
		}

		// We can populate from the local cache because its always up to date
		foreach (ScriptControlDescriptor controlDesc in ScriptControls.g_dictScriptControlDescriptors.Values)
		{
			m_KeyBindManagerUI.AddControlInfo((int)controlDesc.ControlID, controlDesc.Name, controlDesc.Description, (int)controlDesc.CurrentKey, (int)controlDesc.DefaultKey);
		}

		// Add keybinds
		foreach (PlayerKeybindObject keybind in PlayerKeybinds.Keybinds)
		{
			m_KeyBindManagerUI.AddKeybind(keybind.Key, keybind.Action, keybind.KeybindType);
		}

		m_KeyBindManagerUI.CommitControls();
		m_KeyBindManagerUI.CommitKeybinds();

		if (bGotoKeybinds)
		{
			m_KeyBindManagerUI.GotoKeybindsTab();
		}

		m_KeyBindManagerUI.SetVisible(true, true, false);
	}

	public void Hide()
	{
		m_KeyBindManagerUI.SetVisible(false, false, false);
	}

	public void OnSetAllControlsToDefault()
	{
		foreach (ScriptControlDescriptor controlDesc in ScriptControls.g_dictScriptControlDescriptors.Values)
		{
			ConsoleKey currentKey = controlDesc.CurrentKey;
			ConsoleKey newKey = controlDesc.DefaultKey;

			controlDesc.CurrentKey = newKey;

			// NOTE: this MUST fire after the key is updated, some callbacks reference g_dictScriptControlDescriptors directly
			NetworkEvents.SendLocalEvent_OnScriptControlChanged(controlDesc.ControlID, currentKey, newKey);
		}

		NetworkEventSender.SendNetworkEvent_SetAllControlsToDefault();

		Show(false);
	}

	private void Toggle()
	{
		if (m_KeyBindManagerUI.IsVisible())
		{
			Hide();
		}
		else if (KeyBinds.CanProcessKeybinds()) // We can hide always, but can only show when eligible
		{
			Show(false);
		}
	}

	public void OnSaveControl(int ControlID, int NewKey)
	{
		EScriptControlID controlID = (EScriptControlID)ControlID;
		ConsoleKey currentKey = ScriptControls.g_dictScriptControlDescriptors[controlID].CurrentKey;
		ConsoleKey newKey = (ConsoleKey)NewKey;
		// Did the key change?
		if (currentKey != newKey)
		{
			ScriptControls.g_dictScriptControlDescriptors[controlID].CurrentKey = newKey;

			// send to server
			List<GameControlObject> lstGameControls = new List<GameControlObject>();
			foreach (ScriptControlDescriptor controlDesc in ScriptControls.g_dictScriptControlDescriptors.Values)
			{
				// Only send non-default controls, structure is diffed & serialized
				if (controlDesc.CurrentKey != controlDesc.DefaultKey)
				{
					GameControlObject gameControl = new GameControlObject(controlDesc.ControlID, controlDesc.CurrentKey);
					lstGameControls.Add(gameControl);
				}
			}

			NetworkEventSender.SendNetworkEvent_SaveControls(lstGameControls);

			// NOTE: this MUST fire after the key is updated, some callbacks reference g_dictScriptControlDescriptors directly
			NetworkEvents.SendLocalEvent_OnScriptControlChanged(controlID, currentKey, newKey);
		}

		Show(false);
	}

	public void OnCreateKeybind(ConsoleKey key, EPlayerKeyBindType bindType, string strAction)
	{
		NetworkEventSender.SendNetworkEvent_CreateKeybind(key, bindType, strAction);
		// Clientside doesn't care about id, could remove this really
		PlayerKeybinds.Keybinds.Add(new PlayerKeybindObject(-1, key, bindType, strAction));
		Show(true);
	}

	public void OnDeleteKeybind(int index)
	{
		NetworkEventSender.SendNetworkEvent_DeleteKeybind(index);
		PlayerKeybinds.Keybinds.RemoveAt(index);
		Show(true);
	}

}

