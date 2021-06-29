using System.Collections.Generic;
using System.Linq;

public static class ScaleformManager
{
	private static bool m_bReadyToDraw = false;

	public static void Init()
	{
		DeactiveAnyScaleform();

		RageEvents.RAGE_OnRender += OnRender;
	}

	// Scaleforms work weird. Order of operations = Request the scaleform => check if the scaleform is loaded(This is broken but w/e) => Push the scaleform function onto the stack. => Pop the stack => Draw the scaleform.
	// Example:
	// ScaleformManager.RequestScaleform("breaking_news", 255, 255, 255, 255, 0.5f, 0.63f, 1.0f, 1.0f); // The request with out parameters.
	// ScaleformManager.PushAndPopScaleforms(); // Push and pop. This will activate all requested scaleforms.

	// Request the scaleform add to our dictionary.
	public static void RequestScaleform(string Scaleform, int red, int green, int blue, int alpha, float x = -1f, float y = -1f, float width = -1f, float height = -1f, bool FullScreen = false, bool isCustom = false, string customScaleformFunctionName = null)
	{
		int scaleformHandle = RAGE.Game.Graphics.RequestScaleformMovie(Scaleform);
		ScaleformInstance newScaleform = new ScaleformInstance(scaleformHandle, Scaleform, red, green, blue, alpha, x, y, width, height, FullScreen, isCustom, customScaleformFunctionName);
		m_dictScaleforms.Add(newScaleform, false);
	}

	//public static void RequestCustomScaleform(string Scaleform, int red, int green, int blue, int alpha, )

	// Check if they are loaded (This doesn't work :( )
	public static bool HaveRequestedScaleformsLoaded()
	{
		foreach (var requestedScaleform in m_dictScaleforms)
		{
			if (RAGE.Game.Graphics.HasScaleformMovieLoaded(requestedScaleform.Key.ScaleformHandle))
			{
				m_dictScaleforms[requestedScaleform.Key] = true;
			}
		}

		return m_dictScaleforms.Values.All(v => v);
	}

	// Push our scaleform functions and pop once done looping through the ones created.
	public static void PushAndPopScaleforms()
	{
		foreach (var scaleform in m_dictScaleforms)
		{
			if (scaleform.Key.IsCustom)
			{
				RAGE.Game.Graphics.PushScaleformMovieFunction(scaleform.Key.ScaleformHandle, scaleform.Key.CustomFunctionName);
			}
			else
			{
				RAGE.Game.Graphics.PushScaleformMovieFunction(scaleform.Key.ScaleformHandle, scaleform.Key.ScaleformName);
			}
		}

		RAGE.Game.Graphics.PopScaleformMovieFunctionVoid();
		// Ready to draw to start the OnRender drawing.
		m_bReadyToDraw = true;
	}

	// This speaks for itself. Remove all scaleforms from the dictionary. Not ready anymore to draw.
	public static void DeactiveAnyScaleform()
	{
		foreach (var scaleform in m_dictScaleforms)
		{
			int scaleformHandle = scaleform.Key.ScaleformHandle;
			RAGE.Game.Graphics.SetScaleformMovieAsNoLongerNeeded(ref scaleformHandle);
		}
		m_dictScaleforms.Clear();
		m_bReadyToDraw = false;
	}

	// Draw all the scaleforms we have in our dictionary.
	private static void OnRender()
	{
		if (m_bReadyToDraw)
		{
			foreach (var scaleform in m_dictScaleforms)
			{
				if (!scaleform.Key.IsFullscreen)
				{
					RAGE.Game.Graphics.DrawScaleformMovie(scaleform.Key.ScaleformHandle, scaleform.Key.screenPosX, scaleform.Key.screenPosY, scaleform.Key.scaleformWidth, scaleform.Key.scaleformHeight, scaleform.Key.colorRed, scaleform.Key.colorGreen, scaleform.Key.colorBlue, scaleform.Key.colorAlpha, 255);
				}
				else
				{
					RAGE.Game.Graphics.DrawScaleformMovieFullscreen(scaleform.Key.ScaleformHandle, scaleform.Key.colorRed, scaleform.Key.colorGreen, scaleform.Key.colorBlue, scaleform.Key.colorAlpha, 255);
				}
			}
		}
	}

	private static Dictionary<ScaleformInstance, bool> m_dictScaleforms = new Dictionary<ScaleformInstance, bool>();
}

public class ScaleformInstance
{
	public ScaleformInstance(int Handle, string Scaleform, int red, int green, int blue, int alpha, float x, float y, float width, float height, bool FullScreen, bool Custom, string customScaleformFunctionName)
	{
		ScaleformHandle = Handle;
		ScaleformName = Scaleform;
		screenPosX = x;
		screenPosY = y;
		scaleformWidth = width;
		scaleformHeight = height;
		colorRed = red;
		colorGreen = green;
		colorBlue = blue;
		colorAlpha = alpha;
		IsFullscreen = FullScreen;
		IsCustom = Custom;
		CustomFunctionName = customScaleformFunctionName;
	}

	public int ScaleformHandle { get; set; }
	public string ScaleformName { get; set; }
	public float screenPosX { get; set; }
	public float screenPosY { get; set; }
	public float scaleformWidth { get; set; }
	public float scaleformHeight { get; set; }
	public int colorRed { get; set; }
	public int colorGreen { get; set; }
	public int colorBlue { get; set; }
	public int colorAlpha { get; set; }
	public bool IsFullscreen { get; set; }
	public bool IsCustom { get; set; }
	public string CustomFunctionName { get; set; }
}

