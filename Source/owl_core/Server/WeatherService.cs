using GTANetworkAPI;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public static class WeatherService
{
	private static readonly Random g_WeatherRandom = new Random();
	private static GTANetworkAPI.Weather m_cachedWeatherID = GTANetworkAPI.Weather.CLEAR;

	public static GTANetworkAPI.Weather GetCalculatedWeather()
	{
		// Check for december. If true always return snow of some variant.
		if (HelperFunctions.World.IsChristmas())
		{
			int rand = g_WeatherRandom.Next(0, 2);
			m_cachedWeatherID = rand == 0 ? GTANetworkAPI.Weather.XMAS : GTANetworkAPI.Weather.SNOWLIGHT;
		}
		else
		{
			int rand = g_WeatherRandom.Next(0, 10);
			m_cachedWeatherID = (GTANetworkAPI.Weather)rand;
		}
		return m_cachedWeatherID;
	}

	public static GTANetworkAPI.Weather GetCurrentWeather()
	{
		return Core.IsWeatherOverriden ? Core.WeatherOverride : m_cachedWeatherID;
	}
}