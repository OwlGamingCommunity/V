public static class ScreenshotHelper
{
	const float g_fQuality = 100.0f;
	const float g_fCompression = 0.0f;
	const EScreenshotFileFormat g_DefaultFileFormat = EScreenshotFileFormat.PNG;

	public enum EScreenshotFileFormat
	{
		JPG,
		PNG,
		BMP
	}

	public static void TakeScreenshot(string strFilenameWithoutExtension)
	{
		RAGE.Input.TakeScreenshot(Helpers.FormatString("{0}.{1}", strFilenameWithoutExtension, g_DefaultFileFormat.ToString().ToLower()), (int)g_DefaultFileFormat, g_fQuality, g_fCompression);
	}
}