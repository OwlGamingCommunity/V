using GTANetworkAPI;

namespace HelperFunctions
{
	public static class Blip
	{
		public static GTANetworkAPI.Blip Create(Vector3 vecPos, bool bShortRange, float fRange = 50.0f, uint dimension = 0, string strName = "", int spriteID = -1, int spriteColor = -1, bool bAllowSimilarNearby = false)
		{
			// Look for a similar blip nearby before creating this one
			GTANetworkAPI.Blip newBlip = null;
			bool bCanCreate = true;
			if (!bAllowSimilarNearby)
			{
				foreach (var blip in NAPI.Pools.GetAllBlips())
				{
					// is it same sprite?
					if (blip.Dimension == dimension && blip.Sprite == spriteID)
					{
						// how far is it?
						Vector3 vecDistance = new Vector3(vecPos.X - blip.Position.X, vecPos.Y - blip.Position.Y, vecPos.Z - blip.Position.Z);
						float fDistance = vecDistance.Length();

						if (fDistance < 100.0f)
						{
							bCanCreate = false;
						}
					}
				}
			}

			if (bCanCreate)
			{
				newBlip = NAPI.Blip.CreateBlip(vecPos, fRange, dimension);

				if (strName.Length > 0)
				{
					NAPI.Blip.SetBlipName(newBlip, strName);
				}

				if (spriteID != -1)
				{
					NAPI.Blip.SetBlipSprite(newBlip, spriteID);
				}

				if (spriteColor != -1)
				{
					NAPI.Blip.SetBlipColor(newBlip, spriteColor);
				}

				NAPI.Blip.SetBlipShortRange(newBlip, bShortRange);
				newBlip.Scale = 0.8f;
			}

			return newBlip;
		}
	}
}
