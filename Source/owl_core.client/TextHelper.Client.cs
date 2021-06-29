using System;

public static class TextHelper
{
	public static void Draw3D(string strMessage, RAGE.Vector3 vecPos, float fScale, RAGE.RGBA color, RAGE.Game.Font font, RAGE.NUI.UIResText.Alignment alignment, bool bOutline, bool bDropShadow)
	{
		Draw3D(strMessage, vecPos, fScale, (int)color.Red, (int)color.Green, (int)color.Blue, (int)color.Alpha, font, alignment, bOutline, bDropShadow);
	}

	public static void Draw3D(string strMessage, RAGE.Vector3 vecPos, float fScale, int r, int g, int b, int a, RAGE.Game.Font font, RAGE.NUI.UIResText.Alignment alignment, bool bOutline, bool bDropShadow)
	{
		Vector2 vecScreenPos = GraphicsHelper.GetScreenPositionFromWorldPosition(vecPos);

		if (vecScreenPos.SetSuccessfully) // This checks the pos is on screen, but we don't actually draw at the 2d coords
		{
			Draw2D(strMessage, vecScreenPos.X, vecScreenPos.Y, fScale, r, g, b, a, font, alignment, bOutline, bDropShadow);
		}
	}

	public static void Draw2D(string strMessage, float x, float y, float fScale, RAGE.RGBA color, RAGE.Game.Font font, RAGE.NUI.UIResText.Alignment alignment, bool bOutline, bool bDropShadow, bool bIsRTT = false)
	{
		Draw2D(strMessage, x, y, fScale, (int)color.Red, (int)color.Green, (int)color.Blue, (int)color.Alpha, font, alignment, bOutline, bDropShadow, bIsRTT);
	}

	public static void Draw2D(string strMessage, float x, float y, float fScale, int r, int g, int b, int a, RAGE.Game.Font font, RAGE.NUI.UIResText.Alignment alignment, bool bOutline, bool bDropShadow, bool bIsRTT = false)
	{
		// TODO_OPTIMIZATION: Optimize this by precaching the calculations below & watching for resolution changes
		Vector2 vecScreenRes = GraphicsHelper.GetScreenResolution();

		float fMultiplierX = 1.0f;
		float fMultiplierY = 1.0f;

		bool bIs16by9 = (vecScreenRes.X / 16) == (vecScreenRes.Y / 9);
		bool bIs16by10 = (vecScreenRes.X / 16) == (vecScreenRes.Y / 10);
		bool bIs4by3 = (vecScreenRes.X / 4) == (vecScreenRes.Y / 3);
		bool bIsWXGA = !bIs16by9 && !bIs16by10 && !bIs4by3 && (Math.Ceiling(vecScreenRes.X / 16) == Math.Ceiling(vecScreenRes.Y / 9));
		bool bIsWQHD = (vecScreenRes.X == 2560 && vecScreenRes.Y == 1440);

		float fDivisionFactorX = vecScreenRes.X / 1920;
		float fDivisionFactorY = vecScreenRes.Y / 1080;

		if (bIsWXGA)
		{
			fMultiplierX = 1.4f;
		}
		else if (bIsWQHD)
		{
			if (fDivisionFactorX <= 1.0f)
			{
				fMultiplierX = 0.75f;
			}
		}



		// RTT text scaling for 4K etc
		if (vecScreenRes.X > 1920 && vecScreenRes.Y > 1080)
		{
			if (bIsRTT)
			{
				fScale /= fDivisionFactorX;
			}
		}

		// underscan (e.g. 1440x900)
		if (fDivisionFactorX < 1.0f && fDivisionFactorY <= 0.95f)
		{
			fMultiplierX = 1.0f + 1.3f * (1.0f - fDivisionFactorX);
		}
		else if (fDivisionFactorX < 1.0f && fDivisionFactorY > 0.95f && fDivisionFactorY < 1.0f)
		{
			fMultiplierX = 1.0f + 1.3f * (1.0f - fDivisionFactorX);
			fMultiplierY = 1.025f;
		}
		else if (fDivisionFactorX == 1.0f && fDivisionFactorY > 1.3f)
		{
			fMultiplierX = 1.0f + 1.3f * (1.0f - fDivisionFactorX);
			fMultiplierY = 1.0f + 0.76f * (1.0f - fDivisionFactorY);
		}
		else if (fDivisionFactorX == 1.0f && fDivisionFactorY > 1.0f)
		{
			fMultiplierX = 1.0f + 1.3f * (1.0f - fDivisionFactorX);
			fMultiplierY = 1.0f + 0.84f * (1.0f - fDivisionFactorY);
		}
		else if (fDivisionFactorX == 1.333333f && fDivisionFactorY == 1.333333f)
		{
			fMultiplierX = 1.0f + 1.3f * (1.0f - fDivisionFactorX);
		}

		// overscan support, UI at multiples of 1080p just scales up from 1080p
		if (vecScreenRes.X > 1920 && vecScreenRes.Y > 1080
		&& ((vecScreenRes.X % 1920 == 0 && vecScreenRes.Y % 1080 == 0)
		|| fDivisionFactorX >= 1.1f
		|| fDivisionFactorY >= 1.1f))
		{
			vecScreenRes.X = 1920;
			vecScreenRes.Y = 1080;
		}


		int xPos = (int)(x * vecScreenRes.X * fMultiplierX);
		int yPos = (int)(y * vecScreenRes.Y * fMultiplierY);

		RAGE.NUI.UIResText uiText = new RAGE.NUI.UIResText(strMessage, new System.Drawing.Point(xPos, yPos), fScale, System.Drawing.Color.FromArgb(a, r, g, b), font, alignment);
		uiText.Outline = bOutline;
		uiText.DropShadow = bDropShadow;
		uiText.Draw(new System.Drawing.Size(0, 0));
	}
}