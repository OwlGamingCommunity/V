using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class FrameData
{
	[JsonProperty("owl")]
	public double dTimeSpentInScript = 0.0f;

	[JsonProperty("gta")]
	public double dTimeSpentInGTA = 0.0f;

	[JsonProperty("tot")]
	public double dTotalFrameTime = 0.0f;

	private DateTime FrameBeginTime;

	[JsonProperty("func")]
	public Dictionary<int, double> m_dictFunctionTimings = new Dictionary<int, double>();

	[JsonProperty("tmr")]
	public Dictionary<int, double> m_dictTimerTimings = new Dictionary<int, double>();

	[JsonProperty("he")]
	public Dictionary<int, int> m_dictHighExecutions = new Dictionary<int, int>();

	// Not serialized, we generate this at runtime on the viewer end
	[JsonIgnore]
	public Dictionary<string, double> m_dictEventTotalTimings = new Dictionary<string, double>();

	// Not serialized, we generate this at runtime on the viewer end
	[JsonIgnore]
	public Dictionary<string, int> m_dictEventTotalCounts = new Dictionary<string, int>();

	public void RegisterFunctionTime(int legendKey, double dTime)
	{
		m_dictFunctionTimings[legendKey] = Math.Round(dTime, 2);
	}

	public void RegisterTimerTime(int legendKey, double dTime)
	{
		m_dictTimerTimings[legendKey] = Math.Round(dTime, 2);
	}

	public void RegisterHighExecution(int legendKey, int count)
	{
		m_dictHighExecutions[legendKey] = count;
	}

	public void BeginFrame()
	{
		FrameBeginTime = DateTime.Now;
	}

	public void CommitFrame()
	{
		dTotalFrameTime = (DateTime.Now - FrameBeginTime).TotalMilliseconds;
		dTimeSpentInGTA = dTotalFrameTime - dTimeSpentInScript;

		dTimeSpentInScript = Math.Round(dTimeSpentInScript, 2);
		dTimeSpentInGTA = Math.Round(dTimeSpentInGTA, 2);
		dTotalFrameTime = Math.Round(dTotalFrameTime, 2);
	}
}

public static class PerfCaptureViewer
{
	private static Dictionary<string, int> m_dictLegend = null;
	private static Dictionary<int, FrameData> m_dictEntries = null;

	private static bool m_bCaptureLoaded = false;
	private static int m_CurrentFrame = 0;
	private static bool m_bShowGTATimeOnGraph = true;
	private static Dictionary<int, Vector4> m_dictBlocks_Functions = new Dictionary<int, Vector4>();
	private static Dictionary<int, Vector4> m_dictBlocks_Events = new Dictionary<int, Vector4>();

	private static Dictionary<int, RAGE.RGBA> m_dictColors = new Dictionary<int, RAGE.RGBA>();

	private static int Highlight_resetLegend = -1;
	private static RAGE.RGBA Highlight_resetColor = null;

	private static string m_strClickedItemText = null;

	private const string strGTA5KeyName = "GTA 5";

	public static void Init()
	{
		RageEvents.RAGE_OnRender += OnRender;

		LargeDataTransferManager.RegisterIncomingTransferCallbacks(ELargeDataTransferType.PerfCapture, OnPerfCaptureTransfer_Started, OnPerfCaptureTransfer_Progress, OnPerfCaptureTransfer_Complete);
	}

	private static void OnPerfCaptureTransfer_Started(LargeDataTransfer transfer)
	{
		GenericProgressBar.ShowGenericProgressBar("Perf Capture Download", "Downloading...", 0, false, "", UIEventID.Dummy);
	}

	private static void OnPerfCaptureTransfer_Progress(LargeDataTransfer transfer)
	{
		int bytesRemaining = transfer.GetDataLengthEncrypted() - transfer.GetDataOffset();
		int secondsRemaining = (int)Math.Ceiling((float)bytesRemaining / (float)(LargeTransferConstants.MaxBytesPer100ms * 10));
		int percent = (int)(Math.Ceiling(((float)transfer.GetDataOffset() / (float)transfer.GetDataLengthEncrypted() * 100.0f)));

		string strEstimatedTimeRemaining = Helpers.FormatString("{0} remaining (estimated)", Helpers.ConvertSecondsToTimeString(secondsRemaining));
		GenericProgressBar.UpdateCaption(Helpers.FormatString("Downloading... <br>{0}", strEstimatedTimeRemaining));

		GenericProgressBar.UpdateProgress(percent);
	}

	private static void OnPerfCaptureTransfer_Complete(LargeDataTransfer transfer, bool a_bDone)
	{
		GenericProgressBar.CloseAnyProgressBar();

		string strData = System.Text.Encoding.ASCII.GetString(transfer.GetBytes());
		LoadCapture(strData);
	}

	private static void CreateKeybinds()
	{
		KeyBinds.Bind(ConsoleKey.LeftArrow, PrevFrame, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
		KeyBinds.Bind(ConsoleKey.RightArrow, NextFrame, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
		KeyBinds.Bind(ConsoleKey.Home, ToggleGTATime, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
		KeyBinds.Bind(ConsoleKey.End, ExitPerfViewer, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
	}

	private static void DestroyKeybinds()
	{
		// TODO: Add delayed deletion
		KeyBinds.Unbind(ConsoleKey.LeftArrow, PrevFrame, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
		KeyBinds.Unbind(ConsoleKey.RightArrow, NextFrame, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
		KeyBinds.Unbind(ConsoleKey.Home, ToggleGTATime, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
		KeyBinds.Unbind(ConsoleKey.End, ExitPerfViewer, EKeyBindType.Released, EKeyBindFlag.AllowWhenKeybindsDisabled);
	}

	private static void ExitPerfViewer()
	{
		Reset();
	}

	private static void Show()
	{
		CreateKeybinds();
		HUD.SetVisible(false, false, false);
		RAGE.Chat.Show(false);
	}

	private static void Reset()
	{
		HUD.SetVisible(true, false, false);
		RAGE.Chat.Show(true);
		m_bCaptureLoaded = false;
		m_CurrentFrame = 0;
		m_bShowGTATimeOnGraph = true;
		m_dictBlocks_Functions = new Dictionary<int, Vector4>();
		m_dictBlocks_Events = new Dictionary<int, Vector4>();
		m_dictColors = new Dictionary<int, RAGE.RGBA>();
		Highlight_resetLegend = -1;
		Highlight_resetColor = null;
		m_strClickedItemText = null;

		DestroyKeybinds();
	}

	private static void PrevFrame()
	{
		if (m_CurrentFrame > 0)
		{
			--m_CurrentFrame;
		}
	}

	private static void NextFrame()
	{
		if (m_CurrentFrame < m_dictEntries.Count)
		{
			++m_CurrentFrame;
		}
	}

	private static void ToggleGTATime()
	{
		m_bShowGTATimeOnGraph = !m_bShowGTATimeOnGraph;
	}

	public static void LoadCapture(string strCombinedData)
	{
		try
		{
			string[] strSplitData = strCombinedData.Split("%%%");

			if (strSplitData.Length == 2)
			{
				string strLegendData = strSplitData[0];
				string strChunks = strSplitData[1];

				m_dictLegend = OwlJSON.DeserializeObject<Dictionary<string, int>>(RAGE.Util.Deflate.DecompressString(Convert.FromBase64String(strLegendData)), EJsonTrackableIdentifier.PerfCaptureLegend);
				m_dictEntries = OwlJSON.DeserializeObject<Dictionary<int, FrameData>>(RAGE.Util.Deflate.DecompressString(Convert.FromBase64String(strChunks)), EJsonTrackableIdentifier.PerfCaptureEntries);

				Show();
				m_bCaptureLoaded = true;
				m_bShowGTATimeOnGraph = true;

				// fake add GTA so we can select, hover etc
				int GTA5Key = m_dictLegend.Count;

				m_dictLegend.Add(strGTA5KeyName, GTA5Key);

				// now add GTA to each entry
				foreach (var entry in m_dictEntries)
				{
					entry.Value.m_dictFunctionTimings.Add(GTA5Key, entry.Value.dTimeSpentInGTA);

					// also add our event data (if present)
					foreach (var funcCall in entry.Value.m_dictFunctionTimings)
					{
						string strLegendName = GetLegend(funcCall.Key);
						if (strLegendName != strGTA5KeyName)
						{
							string[] strSplit = strLegendName.Split(" ");

							if (strSplit.Length > 1)
							{
								string strEventName = strSplit[strSplit.Length - 1];

								// timing
								if (!entry.Value.m_dictEventTotalTimings.ContainsKey(strEventName))
								{
									entry.Value.m_dictEventTotalTimings[strEventName] = 0.0;
								}

								entry.Value.m_dictEventTotalTimings[strEventName] += funcCall.Value;

								// counts
								if (!entry.Value.m_dictEventTotalCounts.ContainsKey(strEventName))
								{
									entry.Value.m_dictEventTotalCounts[strEventName] = 0;
								}

								entry.Value.m_dictEventTotalCounts[strEventName]++;
							}
						}
					}
				}
			}
		}
		catch
		{

		}
	}


	private static void OnRender()
	{
		if (m_bCaptureLoaded)
		{
			m_dictBlocks_Functions.Clear();
			m_dictBlocks_Events.Clear();

			TextHelper.Draw2D(Helpers.FormatString("Frame {0}/{1}", m_CurrentFrame + 1, m_dictEntries.Count), 0.05f, 0.05f, 0.5f, new RAGE.RGBA(255, 194, 15, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);

			const float fDelta = 0.02f;
			float fY = 0.09f;
			FrameData currentFrame = m_dictEntries[m_CurrentFrame];
			TextHelper.Draw2D(Helpers.FormatString("Frame Time: {0}ms", currentFrame.dTotalFrameTime), 0.07f, fY, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			fY += fDelta;

			TextHelper.Draw2D(Helpers.FormatString("GTA: {0}ms", currentFrame.dTimeSpentInGTA), 0.09f, fY, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			fY += fDelta;

			TextHelper.Draw2D(Helpers.FormatString("Script: {0}ms", currentFrame.dTimeSpentInScript), 0.09f, fY, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			fY += fDelta;

			TextHelper.Draw2D(Helpers.FormatString("{0} function calls", currentFrame.m_dictFunctionTimings.Count), 0.07f, fY, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			fY += fDelta;

			TextHelper.Draw2D(Helpers.FormatString("{0} timer callbacks", currentFrame.m_dictTimerTimings.Count), 0.07f, fY, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			fY += fDelta;

			TextHelper.Draw2D(Helpers.FormatString("{0} high executions", currentFrame.m_dictHighExecutions.Count), 0.07f, fY, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			fY += fDelta;

			// LEGEND
			TextHelper.Draw2D(Helpers.FormatString("<- Prev Frame", currentFrame.dTimeSpentInGTA), 0.8f, 0.09f, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D(Helpers.FormatString("-> Next Frame", currentFrame.dTimeSpentInGTA), 0.8f, 0.11f, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("Home: Include / Exclude GTA time in graph", 0.8f, 0.13f, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			TextHelper.Draw2D("End: Exit Perf Viewer", 0.8f, 0.15f, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);

			// draw boxes
			double dTotalTimeToUse = m_bShowGTATimeOnGraph ? currentFrame.dTotalFrameTime : (currentFrame.dTotalFrameTime - currentFrame.dTimeSpentInGTA);
			const float fOverallBarSize = 0.8f;
			const float fX_Initial = 0.00f;
			float fX = fX_Initial;
			Random rng = new Random();

			// function bars
			TextHelper.Draw2D("Function Timeline", 0.04f, fY + 0.025f, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			const float fBarHeight = 0.1f;
			foreach (var functionTiming in currentFrame.m_dictFunctionTimings)
			{
				string strLegendName = GetLegend(functionTiming.Key);
				if (strLegendName != strGTA5KeyName) // NOTE: Never render GTA here, its in the legend, but we want to do that seperately
				{
					float fPercentOfOverallFrameTiming = (float)(functionTiming.Value / dTotalTimeToUse);
					float fBarWidth = (fPercentOfOverallFrameTiming * fOverallBarSize);

					fBarWidth = Math.Max(fBarWidth, 0.005f);

					if (m_dictColors == null)
					{
						m_dictColors = new Dictionary<int, RAGE.RGBA>();
					}

					// Do we have a color for this key?
					RAGE.RGBA col = null;
					if (m_dictColors.ContainsKey(functionTiming.Key))
					{
						col = m_dictColors[functionTiming.Key];
					}
					else
					{
						uint r = (uint)rng.Next(0, 256);
						uint g = (uint)rng.Next(0, 256);
						uint b = (uint)rng.Next(0, 256);
						col = new RAGE.RGBA(r, g, b, 200);
						m_dictColors[functionTiming.Key] = col;
					}

					float fYRender = fY + 0.1f;
					RAGE.Game.Graphics.DrawRect(fX + (fBarWidth / 2.0f), fYRender, fBarWidth, fBarHeight, (int)col.Red, (int)col.Green, (int)col.Blue, (int)col.Alpha, 0);

					m_dictBlocks_Functions[functionTiming.Key] = new Vector4(fX, fYRender, fBarWidth, fBarHeight);
					fX += fBarWidth; // + 0.04f
				}
			}

			// gta time now
			if (m_bShowGTATimeOnGraph)
			{
				float fPercentOfOverallFrameTimingGTA = (float)(currentFrame.dTimeSpentInGTA / dTotalTimeToUse);
				float fBarWidthGTA = (fPercentOfOverallFrameTimingGTA * fOverallBarSize);

				RAGE.Game.Graphics.DrawRect(fX + (fBarWidthGTA / 2.0f), fY + 0.1f, fBarWidthGTA, 0.1f, 255, 0, 0, 200, 0);
				TextHelper.Draw2D(Helpers.FormatString("GTA 5 ({0}ms)", currentFrame.dTimeSpentInGTA), fX + (fBarWidthGTA / 2.0f), fY + 0.1f, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, false, false);
			}

			// event bars
			int eventCounter = currentFrame.m_dictFunctionTimings.Count;
			fX = fX_Initial;
			fY += fBarHeight * 2.0f;
			TextHelper.Draw2D("Event Timeline", 0.04f, fY + 0.025f, 0.25f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Left, true, true);
			foreach (var eventTiming in currentFrame.m_dictEventTotalTimings)
			{
				float fPercentOfOverallFrameTiming = (float)(eventTiming.Value / currentFrame.dTimeSpentInScript);
				float fBarWidth = (fPercentOfOverallFrameTiming * fOverallBarSize);

				fBarWidth = Math.Max(fBarWidth, 0.005f);

				// Do we have a color for this key?
				RAGE.RGBA col = null;
				if (m_dictColors.ContainsKey(eventCounter))
				{
					col = m_dictColors[eventCounter];
				}
				else
				{
					uint r = (uint)rng.Next(0, 256);
					uint g = (uint)rng.Next(0, 256);
					uint b = (uint)rng.Next(0, 256);
					col = new RAGE.RGBA(r, g, b, 200);
					m_dictColors[eventCounter] = col;
				}

				float fYRender = fY + 0.1f;
				RAGE.Game.Graphics.DrawRect(fX + (fBarWidth / 2.0f), fYRender, fBarWidth, fBarHeight, (int)col.Red, (int)col.Green, (int)col.Blue, (int)col.Alpha, 0);

				m_dictBlocks_Events[eventCounter] = new Vector4(fX, fYRender, fBarWidth, fBarHeight);
				++eventCounter;
				fX += fBarWidth; // + 0.04f
			}

			// check for mouse over or press
			bool bJustLeftClicked = RAGE.Game.Pad.IsDisabledControlJustReleased(0, 24);
			bool bFoundItem = false;

			// reset highlight effect
			m_dictColors[Highlight_resetLegend] = Highlight_resetColor;
			Highlight_resetLegend = -1;
			Highlight_resetColor = null;

			if (CursorManager.IsCursorVisible())
			{
				Vector2 vecCursorPos = CursorManager.GetCursorPosition();

				// convert to screen space
				Vector2 vecResolution = GraphicsHelper.GetScreenResolution();
				float fMouseX = (vecCursorPos.X / vecResolution.X);
				float fMouseY = (vecCursorPos.Y / vecResolution.Y);

				// functions
				foreach (var kvPair in m_dictBlocks_Functions)
				{
					int legendID = kvPair.Key;
					Vector4 vecBlock = kvPair.Value;

					if ((fMouseX >= vecBlock.X && fMouseX <= vecBlock.X + vecBlock.Z) && (fMouseY >= (vecBlock.Y - (0.5 * vecBlock.W)) && fMouseY <= vecBlock.Y + (0.5 * vecBlock.W)))
					{
						// mouseover
						if (m_dictColors.ContainsKey(legendID))
						{
							Highlight_resetColor = m_dictColors[legendID];
						}
						Highlight_resetLegend = legendID;

						m_dictColors[legendID] = new RAGE.RGBA(255, 194, 15, 255);

						string strName = GetLegend(legendID);
						// normal block
						if (strName != null)
						{
							double funcTime = currentFrame.m_dictFunctionTimings[legendID];
							float fPercentOfOverallFrameTiming = (float)(funcTime / currentFrame.dTotalFrameTime) * 100.0f;
							float fPercentOfOverallFrameTimingScript = (float)(funcTime / currentFrame.dTimeSpentInScript) * 100.0f;

							if (bJustLeftClicked)
							{
								m_strClickedItemText = Helpers.FormatString("Selected Function: {0} = {1}ms ({2:0.00}% of total frame, {3:0.00}% of script frame)", strName, funcTime, fPercentOfOverallFrameTiming, fPercentOfOverallFrameTimingScript);
								bFoundItem = true;
							}


							TextHelper.Draw2D(Helpers.FormatString("Highlighted Function: {0} = {1}ms ({2:0.00}% of total frame, {3:0.00}% of script frame)", strName, funcTime, fPercentOfOverallFrameTiming, fPercentOfOverallFrameTimingScript), 0.5f, 0.17f, 0.5f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
						}
						break;
					}
				}

				// events
				int eventBlockCounter = 0;
				foreach (var kvPair in m_dictBlocks_Events)
				{
					int legendID = kvPair.Key;
					Vector4 vecBlock = kvPair.Value;

					if ((fMouseX >= vecBlock.X && fMouseX <= vecBlock.X + vecBlock.Z) && (fMouseY >= (vecBlock.Y - (0.5 * vecBlock.W)) && fMouseY <= vecBlock.Y + (0.5 * vecBlock.W)))
					{
						// mouseover
						if (m_dictColors.ContainsKey(legendID))
						{
							Highlight_resetColor = m_dictColors[legendID];
						}
						Highlight_resetLegend = legendID;

						m_dictColors[legendID] = new RAGE.RGBA(255, 194, 15, 255);

						double eventTiming = currentFrame.m_dictEventTotalTimings.ElementAt(eventBlockCounter).Value;
						int numEvents = currentFrame.m_dictEventTotalCounts.ElementAt(eventBlockCounter).Value;
						string strName = currentFrame.m_dictEventTotalCounts.ElementAt(eventBlockCounter).Key;

						float fPercentOfOverallFrameTiming = (float)(eventTiming / currentFrame.dTotalFrameTime) * 100.0f;
						float fPercentOfOverallFrameTimingScript = (float)(eventTiming / currentFrame.dTimeSpentInScript) * 100.0f;

						if (bJustLeftClicked)
						{
							m_strClickedItemText = Helpers.FormatString("Selected Event: {0} = {1}ms ({4} counts) ({2:0.00}% of total frame, {3:0.00}% of script frame)", strName, eventTiming, fPercentOfOverallFrameTiming, fPercentOfOverallFrameTimingScript, numEvents);
							bFoundItem = true;
						}

						TextHelper.Draw2D(Helpers.FormatString("Highlighted Event: {0} = {1}ms ({4} counts) ({2:0.00}% of total frame, {3:0.00}% of script frame)", strName, eventTiming, fPercentOfOverallFrameTiming, fPercentOfOverallFrameTimingScript, numEvents), 0.5f, 0.17f, 0.5f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
						break;
					}

					++eventBlockCounter;
				}

				// reset click
				if (!bFoundItem && bJustLeftClicked)
				{
					m_strClickedItemText = null;
				}

				if (m_strClickedItemText != null)
				{
					TextHelper.Draw2D(m_strClickedItemText, 0.5f, 0.2f, 0.5f, new RAGE.RGBA(255, 255, 255, 200), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
				}
			}
		}
	}

	private static string GetLegend(int key)
	{
		foreach (var entry in m_dictLegend)
		{
			if (entry.Value == key)
			{
				return entry.Key;
			}
		}

		return null;
	}
}

public static class PerfManager
{
	private static int m_LastFrameIndex = -1;
	private static int m_StartFrame = 0;
	private static bool m_bCapturing = false;
	private static WeakReference<ClientTimer> m_CaptureTimer = new WeakReference<ClientTimer>(null);

	private static Dictionary<string, int> m_dictLegend = null;
	private static Dictionary<int, FrameData> m_dictEntries = null;

	public static void Init()
	{
		m_dictEntries = new Dictionary<int, FrameData>();
		m_dictLegend = new Dictionary<string, int>();

		RageEvents.RAGE_OnRender += OnRender;

		PerfCaptureViewer.Init();

		NetworkEvents.StartPerformanceCapture += StartCapture;
	}

	private static void OnRender()
	{
		if (m_bCapturing)
		{
			RAGE.Game.Graphics.DrawRect(0.0f, 0.85f, 4.0f, 0.10f, 76, 76, 76, 200, 0);
			TextHelper.Draw2D("Performance Capture", 0.5f, 0.8f, 1.0f, 255, 194, 14, 255, RAGE.Game.Font.HouseScript, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			TextHelper.Draw2D(Helpers.FormatString("Capture in progress, {0} seconds remaining!", m_CaptureTimer.Instance().GetSecondsUntilNextTick()), 0.5f, 0.86f, 0.5f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
		}
	}

	private static void StartCapture(int lengthInSeconds)
	{
		if (m_bCapturing)
		{
			NotificationManager.ShowNotification("Performance Capture", "A perf capture is already running, please wait!", ENotificationIcon.ExclamationSign);
			return;
		}

		m_StartFrame = RAGE.Game.Misc.GetFrameCount();
		m_bCapturing = true;

		NotificationManager.ShowNotification("Performance Capture", "Perf Capture started!", ENotificationIcon.ExclamationSign);

		m_CaptureTimer = ClientTimerPool.CreateTimer(StopCapture, lengthInSeconds * 1000, 1);
	}

	private static void StopCapture(object[] parameters)
	{
		m_bCapturing = false;
		m_LastFrameIndex = -1;
		m_StartFrame = 0;

		NotificationManager.ShowNotification("Performance Capture", "Perf Capture finished!", ENotificationIcon.ExclamationSign);

		// TODO: Block new capture if other capture still being uploaded
		string strLegend = Convert.ToBase64String(RAGE.Util.Deflate.Compress(OwlJSON.SerializeObject(m_dictLegend, EJsonTrackableIdentifier.PerfCaptureLegend)));
		string strEntries = Convert.ToBase64String(RAGE.Util.Deflate.Compress(OwlJSON.SerializeObject(m_dictEntries, EJsonTrackableIdentifier.PerfCaptureEntries)));
		string strCombined = Helpers.FormatString("{0}%%%{1}", strLegend, strEntries);

		// begin transfer upload
		int accountID = DataHelper.GetLocalPlayerEntityData<int>(EDataNames.ACCOUNT_ID);
		bool bQueued = LargeDataTransferManager.QueueOutgoingTransfer(ELargeDataTransferType.PerfCapture, accountID, System.Text.Encoding.ASCII.GetBytes(strCombined), null, null, PerformanceCaptureConstants.EncryptionKey);

		// Reset
		m_dictEntries.Clear();
		m_dictLegend.Clear();
	}

	private static FrameData InitFrameDataIfNotPresentAndGet()
	{
		int frameIndex = RAGE.Game.Misc.GetFrameCount() - m_StartFrame;

		// Did we just finish the frame?
		if (m_LastFrameIndex != frameIndex && m_LastFrameIndex != -1)
		{
			m_dictEntries[m_LastFrameIndex].CommitFrame();
		}

		if (!m_dictEntries.ContainsKey(frameIndex))
		{
			m_dictEntries[frameIndex] = new FrameData();
			m_dictEntries[frameIndex].BeginFrame();
			m_LastFrameIndex = frameIndex;
		}

		return m_dictEntries[frameIndex];
	}

	private static int GetLegend(string strEntry)
	{
		if (!m_dictLegend.ContainsKey(strEntry))
		{
			m_dictLegend.Add(strEntry, m_dictLegend.Count);
		}

		return m_dictLegend[strEntry];
	}

	public static void RegisterStatistic(MethodInfo methodInfo, long time_started, string strExtraInfo)
	{
		if (!m_bCapturing)
		{
			return;
		}

		FrameData currFrameData = InitFrameDataIfNotPresentAndGet();

		long end = DateTime.Now.Ticks;
		double time_diff = ((double)end - (double)time_started) / (double)TimeSpan.TicksPerMillisecond;
		string strModuleName = methodInfo.DeclaringType.FullName;

		currFrameData.dTimeSpentInScript += time_diff;

		string strFunctionName = methodInfo.Name;
		string strEntryName = Helpers.FormatString("{0}::{1}", strModuleName, strFunctionName);
		if (strEntryName.Length > 32 && strEntryName.Contains("::"))
		{
			string[] strSplit = strEntryName.Split("::");
			strEntryName = Helpers.FormatString("{0}::{1}", strSplit[0], strSplit[1]);
		}

		if (strExtraInfo != null)
		{
			strEntryName += Helpers.FormatString(" {0}", strExtraInfo);
		}

		currFrameData.RegisterFunctionTime(GetLegend(strEntryName), time_diff);
	}

	public static void RegisterTimerPerf(Delegate timerDelegate, long time_started)
	{
		if (!m_bCapturing)
		{
			return;
		}

		long end = DateTime.Now.Ticks;
		double time_diff = ((double)end - (double)time_started) / (double)TimeSpan.TicksPerMillisecond;

		string strModuleName = timerDelegate.GetMethodInfo().DeclaringType.FullName;
		string strFunctionName = timerDelegate.GetMethodInfo().Name;

		string strEntryName = Helpers.FormatString("{0}::{1}", strModuleName, strFunctionName);

		FrameData currFrameData = InitFrameDataIfNotPresentAndGet();

		currFrameData.RegisterTimerTime(GetLegend(strEntryName), time_diff);
	}

	public static void RegisterHighExecutionUI(string strUI, int count)
	{
		if (!m_bCapturing)
		{
			return;
		}

		FrameData currFrameData = InitFrameDataIfNotPresentAndGet();

		string[] strSplit = strUI.Split("/");
		string strFileName = strSplit[strSplit.Length - 1];

		currFrameData.RegisterHighExecution(GetLegend(strFileName), count);
	}
}