using System;

internal class CGUIKeyBindManager : CEFCore
{
	public CGUIKeyBindManager(OnGUILoadedDelegate callbackOnLoad) : base("owl_core.client/keybinds.html", EGUIID.Keybindmanager, callbackOnLoad)
	{
		UIEvents.HideControlManager += () => { Core.GetPlayerKeyBindManager()?.Hide(); };
		UIEvents.SaveControl += (int ControlID, int NewKey) => { Core.GetPlayerKeyBindManager()?.OnSaveControl(ControlID, NewKey); };
		UIEvents.SetAllControlsToDefault += () => { Core.GetPlayerKeyBindManager()?.OnSetAllControlsToDefault(); };

		UIEvents.CreateKeybind += (ConsoleKey key, EPlayerKeyBindType bindType, string strAction) => { Core.GetPlayerKeyBindManager()?.OnCreateKeybind(key, bindType, strAction); };
		UIEvents.DeleteKeybind += (int index) => { Core.GetPlayerKeyBindManager()?.OnDeleteKeybind(index); };
	}

	public override void OnLoad()
	{

	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void RegisterKeyCode(int id, string name)
	{
		Execute("RegisterKeyCode", id, name);
	}

	public void AddControlInfo(int control_id, string strName, string strDescription, int keyCurrent, int keyDefault)
	{
		Execute("AddControlInfo", control_id, strName, strDescription, keyCurrent, keyDefault);
	}

	public void CommitControls()
	{
		Execute("CommitControls");
	}

	public void AddKeybind(ConsoleKey key, string strAction, EPlayerKeyBindType keybindType)
	{
		Execute("AddKeybind", key, strAction, keybindType);
	}

	public void CommitKeybinds()
	{
		Execute("CommitKeybinds");
	}

	public void GotoKeybindsTab()
	{
		Execute("GotoKeybindsTab");
	}
}