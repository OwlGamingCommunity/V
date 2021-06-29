using GTANetworkAPI;
using System;

public class MetalDetectorSystem 
{
	public MetalDetectorSystem()
	{
		RageEvents.RAGE_OnPlayerEnterColshape += OnPlayerEnterColshape;
	}

	public void OnPlayerEnterColshape(ColShape colshape, Player player)
	{
		bool isMetalDetector = EntityDataManager.GetData<bool>(colshape, EDataNames.IS_DETECTOR);
		WeakReference<CPlayer> playerRef = PlayerPool.GetPlayerFromClient(player);
		CPlayer cPlayer = playerRef.Instance();

		DateTime dt = DateTime.MinValue;
		DateTime detectorLastUsed = dt.AddMilliseconds(Convert.ToDouble(EntityDataManager.GetData<string>(colshape, EDataNames.DETECTOR_LASTUSED)));
		bool canBeUsed = (DateTime.Now.Millisecond - detectorLastUsed.Millisecond >= 2000) ? false : true;

		if (isMetalDetector && canBeUsed)
		{
			EntityDataManager.SetData(colshape, EDataNames.DETECTOR_LASTUSED, DateTime.Now.Millisecond.ToString(), EDataType.Synced);
			Int64 colshapeId = EntityDataManager.GetData<Int64>(colshape, EDataNames.DETECTOR_ID);

			if (colshapeId != 0)
			{
				Vector3 colshapePosition = MetalDetectorPool.GetMetalDetectorInstanceFromID(colshapeId).DetectorPos;

				foreach (Player playerInRange in NAPI.Player.GetPlayersInRadiusOfPosition(15.0, colshapePosition))
				{
					WeakReference<CPlayer> foundPlayerRef = PlayerPool.GetPlayerFromClient(playerInRange);
					CPlayer foundPlayer = foundPlayerRef.Instance();
					if (cPlayer.GetWeaponDataClientside().Count > 0)
					{
						NetworkEventSender.SendNetworkEvent_PlayMetalDetectorAlarm(foundPlayer, colshapePosition);
					}
				}
			}
		}
	}
}