public class SpikeStrips
{
	const float g_fDistThreshold = 3.0f;
	const float g_fPopThresholdDist = 0.8f;

	public SpikeStrips()
	{
		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;
	}

	private void OnRender()
	{
		RAGE.Elements.MapObject nearestSpikestrip = GetNearestSpikeStrip(false);
		if (nearestSpikestrip != null)
		{
			WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Pickup Spike Strip", null, OnPickupStrips, nearestSpikestrip.Position, nearestSpikestrip.Dimension, false, false, g_fDistThreshold);
		}
	}

	private void OnTick()
	{
		CheckLocalPlayerForSpikeStripEvents();
	}

	private void OnPickupStrips()
	{
		RAGE.Elements.MapObject nearestSpikestrip = GetNearestSpikeStrip(false);

		if (nearestSpikestrip != null)
		{
			NetworkEventSender.SendNetworkEvent_PickupStrips(nearestSpikestrip);
		}
	}

	private bool IsWithinDistanceForTirePop(RAGE.Vector3 vecEntityPos, RAGE.Vector3 vecBoxPos)
	{
		return WorldHelper.GetDistance(vecEntityPos, vecBoxPos) <= g_fPopThresholdDist;
	}

	private bool IsTireBurstAtAll(RAGE.Elements.Vehicle vehicle, int tireIndex)
	{
		return vehicle.IsTyreBurst(tireIndex, false) || vehicle.IsTyreBurst(tireIndex, true);
	}

	private void CheckLocalPlayerForSpikeStripEvents()
	{
		RAGE.Elements.Vehicle vehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		if (vehicle != null)
		{
			// TODO: Do we need to pop more wheels? e.g. 6 wheel trucks
			RAGE.Vector3 frontLeftWheelPos = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_lf"));
			RAGE.Vector3 frontRightWheelPos = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_rf"));
			RAGE.Vector3 rearLeftWheelPos = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_lr"));
			RAGE.Vector3 rearRightWheelPos = vehicle.GetWorldPositionOfBone(vehicle.GetBoneIndexByName("wheel_rr"));


			foreach (RAGE.Elements.MapObject obj in OptimizationCachePool.StreamedInObjects())
			{
				EObjectTypes objType = DataHelper.GetEntityData<EObjectTypes>(obj, EDataNames.OBJECT_TYPE);

				if (objType == EObjectTypes.SPIKE_STRIP || objType == EObjectTypes.SPIKE_STRIP_NO_PICKUP)
				{
					bool frontLeftTouching = IsWithinDistanceForTirePop(frontLeftWheelPos, obj.Position) && !IsTireBurstAtAll(vehicle, 0);
					bool frontRightTouching = IsWithinDistanceForTirePop(frontRightWheelPos, obj.Position) && !IsTireBurstAtAll(vehicle, 1);
					bool rearLeftTouching = IsWithinDistanceForTirePop(rearLeftWheelPos, obj.Position) && !IsTireBurstAtAll(vehicle, 4);
					bool rearRightTouching = IsWithinDistanceForTirePop(rearRightWheelPos, obj.Position) && !IsTireBurstAtAll(vehicle, 5);

					if (frontLeftTouching || frontRightTouching || rearLeftTouching || rearRightTouching)
					{
						// TODO_POST_LAUNCH: If we hit it at high speed, perhaps insta-pop to rim?
						// TODO_POST_LAUNCH: Check setTyreBurst syncs?
						if (frontLeftTouching)
						{
							vehicle.SetTyreBurst(0, false, 100.0f);
						}

						if (frontRightTouching)
						{
							vehicle.SetTyreBurst(1, false, 100.0f);
						}

						if (rearLeftTouching)
						{
							vehicle.SetTyreBurst(4, false, 100.0f);
						}

						if (rearRightTouching)
						{
							vehicle.SetTyreBurst(5, false, 100.0f);
						}
					}
				}
			}
		}
	}

	private RAGE.Elements.MapObject GetNearestSpikeStrip(bool bAllowNonPickup)
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.SpikeStrip);
		RAGE.Elements.MapObject spikeStrip = poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.MapObject>() : null;

		if (bAllowNonPickup)
		{
			PoolEntry poolEntryNonPickup = OptimizationCachePool.GetPoolItem(EPoolCacheKey.SpikeStrip_NoPickup);

			// Closer than the non pickup version?
			if (poolEntryNonPickup != null)
			{
				if (poolEntry == null || poolEntryNonPickup.GetDistance() < poolEntry.GetDistance())
				{
					spikeStrip = poolEntryNonPickup.GetEntity<RAGE.Elements.MapObject>();
				}
			}
		}

		return spikeStrip;
	}
}