public static class ControlHelper
{
	public static void SetControlDisabledThisFrame(RAGE.Game.Control control)
	{
		for (var i = 0; i < 3; ++i)
		{
			RAGE.Game.Pad.DisableControlAction(i, (int)control, true);
		}
	}
}