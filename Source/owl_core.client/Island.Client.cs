using System.Collections.Generic;

namespace owl_core.client
{
	public class Island
	{
		private const string NATIVE_ISLAND_KEY = "HeistIsland";
		private static readonly List<Vector2> g_listBounds = new List<Vector2>
		{
			new Vector2(3263.4353f, -4538.103f),
			new Vector2(5842.904f, -3628.5469f),
			new Vector2(6256.1304f, -6666.665f),
			new Vector2(3302.955f, -6210.961f)
		};

		// It's a bool but an int. Thanks R* I guess. 
		private int bIslandLoaded = 0;

		public Island()
		{
			RageEvents.RAGE_OnTick_LowFrequency += OnTick;
		}

		private void OnTick()
		{
			bool bWithinBounds = IsPointInPolygon(PlayerHelper.GetLocalPlayerPosition().X, PlayerHelper.GetLocalPlayerPosition().Y);
			if (bWithinBounds && bIslandLoaded == 0)
			{
				bIslandLoaded = 1;
				RAGE.Game.Invoker.Invoke(0x5E1460624D194A38, bIslandLoaded);
				RAGE.Game.Invoker.Invoke(0x9A9D1BA639675CF1, NATIVE_ISLAND_KEY, bIslandLoaded);

				// achievement
				NetworkEventSender.SendNetworkEvent_UnlockAchievement(EAchievementID.CayoPericoIsland);
			}
			else if (!bWithinBounds && bIslandLoaded == 1)
			{
				bIslandLoaded = 0;
				RAGE.Game.Invoker.Invoke(0x5E1460624D194A38, bIslandLoaded);
				RAGE.Game.Invoker.Invoke(0x9A9D1BA639675CF1, NATIVE_ISLAND_KEY, bIslandLoaded);
			}
		}

		private bool IsPointInPolygon(float x, float y)
		{
			bool bInside = false;
			int j = g_listBounds.Count - 1;

			for (int i = 0; i < g_listBounds.Count; j = i++)
			{
				var pi = g_listBounds[i];
				var pj = g_listBounds[j];
				if (((pi.Y <= y && y < pj.Y) || (pj.Y <= y && y < pi.Y)) &&
					(x < (pj.X - pi.X) * (y - pi.Y) / (pj.Y - pi.Y) + pi.X))
					bInside = !bInside;
			}

			return bInside;
		}
	}
}
