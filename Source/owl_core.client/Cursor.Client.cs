using System.Collections.Generic;

public static class CursorManager
{
	static CursorManager()
	{

	}

	static private List<object> g_iCursorVisibleRefCount = new List<object>();
	static private bool g_bPlayerCursorEnabled = false;
	// TODO: This is hacky
	private static object m_DummyThisObject = new object();

	public static void Init()
	{
		ScriptControls.SubscribeToControl(EScriptControlID.ToggleCursor, OnToggleCursor);
		ScriptControls.SubscribeToControl(EScriptControlID.HideCursor, OnHideCursor);

		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.CharacterSelectionApproved += () =>
		{
			g_bPlayerCursorEnabled = false;
			g_iCursorVisibleRefCount.Clear();
		};
	}

	private static void OnTick()
	{
		// Safety checks
		if (g_iCursorVisibleRefCount.Count > 0 && !RAGE.Ui.Cursor.Visible)
		{
			RAGE.Ui.Cursor.Visible = true;
			RAGE.Game.Pad.DisableAllControlActions(0);
			RAGE.Game.Pad.DisableAllControlActions(1);
			RAGE.Game.Pad.DisableAllControlActions(2);

			NetworkEvents.SendLocalEvent_CursorStateChange(true);

		}
		else if (g_iCursorVisibleRefCount.Count == 0 && RAGE.Ui.Cursor.Visible)
		{
			RAGE.Ui.Cursor.Visible = false;
			RAGE.Game.Pad.EnableAllControlActions(0);
			RAGE.Game.Pad.EnableAllControlActions(1);
			RAGE.Game.Pad.EnableAllControlActions(2);

			NetworkEvents.SendLocalEvent_CursorStateChange(false);

		}
	}

	public static bool IsCursorVisible()
	{
		return g_iCursorVisibleRefCount.Count > 0;
	}

	public static Vector2 GetCursorPosition()
	{
		return new Vector2(RAGE.Ui.Cursor.Position.X, RAGE.Ui.Cursor.Position.Y);
	}

	public static void SetCursorVisible(bool a_bVisible, object source)
	{
		if (a_bVisible)
		{
			if (!g_iCursorVisibleRefCount.Contains(source))
			{
				g_iCursorVisibleRefCount.Add(source);
			}
		}
		else
		{
			if (g_iCursorVisibleRefCount.Contains(source))
			{
				g_iCursorVisibleRefCount.Remove(source);

				// If a script removed the cursor, but the player cursor was enabled, we should remove the player cursor too since their action most likely caused it
				OnHideCursor(EControlActionType.Pressed);
			}
		}
	}

	private static void OnToggleCursor(EControlActionType actionType)
	{
		if (!KeyBinds.IsChatInputVisible())
		{
			g_bPlayerCursorEnabled = !g_bPlayerCursorEnabled;

			if (g_bPlayerCursorEnabled)
			{
				SetCursorVisible(true, m_DummyThisObject);
			}
			else
			{
				SetCursorVisible(false, m_DummyThisObject);
			}
		}

	}

	private static void OnHideCursor(EControlActionType actionType)
	{
		if (!KeyBinds.IsChatInputVisible())
		{
			if (g_bPlayerCursorEnabled)
			{
				g_bPlayerCursorEnabled = false;
				SetCursorVisible(false, m_DummyThisObject);
			}
		}
	}

	public static void ForceHidePlayerRequestedCursor()
	{
		// NOTE: Only use this if you're SURE the cursor you are using is player invoked, this is very rare (e.g. radials)
		OnHideCursor(EControlActionType.Pressed);
	}
}