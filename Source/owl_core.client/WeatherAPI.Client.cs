public class WeatherAPI
{
	public WeatherAPI()
	{
		NetworkEvents.WeatherInfo += OnWeatherInfo;
	}

	private void OnWeatherInfo(string strWeatherMain, string strWeatherDescription, float weatherTemp, float weatherTempFeelsLike, int weatherHumidity, float weatherWindSpeed)
	{
		if (m_WeatherGUI.IsVisible())
		{
			m_WeatherGUI.SetVisible(false, false, false);
		}

		m_WeatherGUI.SetVisible(true, false, false);
		m_WeatherGUI.Execute("SetAllData", strWeatherDescription, weatherTemp, weatherWindSpeed, weatherHumidity);

		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			m_WeatherGUI.SetVisible(false, false, false);
		}, 10000, 1);
	}

	private static CGUIWeather m_WeatherGUI = new CGUIWeather(() => { });
}
