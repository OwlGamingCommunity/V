using System;

public class ChatVisualization
{
	private bool g_bVisualizationEnabled = false;

	public ChatVisualization()
	{
		/*KeyBinds.Bind(ConsoleKey.F5, () =>
		{
			g_bVisualizationEnabled = !g_bVisualizationEnabled;
			ChatHelper.DebugMessage("Chat Visualization Enabled: {0}", g_bVisualizationEnabled);
		}, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);*/

		RageEvents.RAGE_OnRender += OnRender;
	}

	private void OnRender()
	{
		if (g_bVisualizationEnabled)
		{
			RAGE.Vector3 vecRootPos = RAGE.Elements.Player.LocalPlayer.Position.Subtract(new RAGE.Vector3(0.0f, 0.0f, 1.0f));
			DrawChatCircle(vecRootPos.CopyVector().Add(new RAGE.Vector3(0.0f, 0.0f, 0.2f)), ChatConstants.g_fDistance_Megaphone, new RAGE.RGBA(0, 255, 0, 150), 0.0f, "MEGAPHONE");
			DrawChatCircle(vecRootPos.CopyVector().Add(new RAGE.Vector3(0.0f, 0.0f, 0.4f)), ChatConstants.g_fDistance_ShoutLoudly, new RAGE.RGBA(255, 0, 0, 150), 0.05f, "SHOUT LOUDLY");
			DrawChatCircle(vecRootPos.CopyVector().Add(new RAGE.Vector3(0.0f, 0.0f, 0.5f)), ChatConstants.g_fDistance_Shout, new RAGE.RGBA(0, 0, 255, 150), 0.1f, "SHOUT");
			DrawChatCircle(vecRootPos.CopyVector().Add(new RAGE.Vector3(0.0f, 0.0f, 1.0f)), ChatConstants.g_fDistance_Nearby, new RAGE.RGBA(0, 162, 232, 150), 0.15f, "NEARBY");
			DrawChatCircle(vecRootPos.CopyVector().Add(new RAGE.Vector3(0.0f, 0.0f, 0.8f)), ChatConstants.g_fDistance_Closeby, new RAGE.RGBA(255, 194, 15, 150), 0.2f, "CLOSEBY");
		}
	}

	private void DrawChatCircle(RAGE.Vector3 vecPos, float fRadius, RAGE.RGBA col, float fTextOffset, string strDescription)
	{
		TextHelper.Draw2D(strDescription, 0.0f, 0.3f + fTextOffset, 0.6f, (int)col.Red, (int)col.Green, (int)col.Blue, (int)col.Alpha, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);

		float fAngularOffset = 10.0f;
		int numPolys = (int)(360.0f / fAngularOffset);

		for (int i = 0; i < numPolys; ++i)
		{
			float fRot = i * fAngularOffset;

			RAGE.Vector3 vecPosInFrontLeft = vecPos.CopyVector();
			RAGE.Vector3 vecPosInFrontRight = vecPos.CopyVector();

			float radians = (fRot) * (3.14f / 180.0f);
			vecPosInFrontLeft.X += (float)Math.Cos(radians) * fRadius;
			vecPosInFrontLeft.Y += (float)Math.Sin(radians) * fRadius;

			radians = (fRot - fAngularOffset) * (3.14f / 180.0f);
			vecPosInFrontRight.X += (float)Math.Cos(radians) * fRadius;
			vecPosInFrontRight.Y += (float)Math.Sin(radians) * fRadius;

			RAGE.Game.Graphics.DrawPoly(vecPos.X, vecPos.Y, vecPos.Z, vecPosInFrontRight.X, vecPosInFrontRight.Y, vecPosInFrontRight.Z, vecPosInFrontLeft.X, vecPosInFrontLeft.Y, vecPosInFrontLeft.Z, (int)col.Red, (int)col.Green, (int)col.Blue, (int)col.Alpha);
		}
	}
}
