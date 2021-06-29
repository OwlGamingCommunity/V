//#define ALLOW_CHRISTMAS
// NOTE: Flag to disable christmas for testing, snow may be crashy. Similar flag is in WorldHelper.Client.cs on client
using System;

namespace HelperFunctions
{
	public static class World
	{
		public static bool SetWeather(int weatherID)
		{
			if (weatherID >= 0 && weatherID <= 13)
			{
				Core.SetOverrideWeather(true, weatherID);
				return true;
			}
			else
			{
				return false;
			}
		}

		public static GTANetworkAPI.Weather GetCurrentWeather()
		{
			return WeatherService.GetCurrentWeather();
		}

		public static bool IsChristmas()
		{
#if DEBUG
			return false;
#else
#if ALLOW_CHRISTMAS
					return DateTime.Now.Month == 12;
#else
					return false;
#endif
#endif
		}

		public static bool IsHalloween()
		{
			return DateTime.Now.Month == 10 && DateTime.Now.Day >= 15;
		}

		public static bool IsFourthOfJuly()
		{
			return DateTime.Now.Month == 7 && DateTime.Now.Day == 4;
		}

		public static bool IsFourthOfJulyEventInProgress()
		{
			return m_bIsFourthOfJulyEventInProgress;
		}

		public static void SetFourthOfJulyEvent(bool inProgress)
		{
			m_bIsFourthOfJulyEventInProgress = inProgress;
		}

		static bool m_bIsFourthOfJulyEventInProgress = false;
	}
}