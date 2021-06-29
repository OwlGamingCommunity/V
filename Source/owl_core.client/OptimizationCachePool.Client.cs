using ExtensionMethods;
using System;
using System.Collections.Generic;

public enum EPoolCacheKey
{
	WorldItem,
	ATM,
	FuelPoint,
	CarWash,
	VehicleRepair,
	SpikeStrip,
	SpikeStrip_NoPickup,
	PropertyMarker,
	ElevatorMarker,
	BoomBox,
	DutyPoint,
	NewsCamera,
	InfoMarker,
	MAX // used for quicker per frame iteration
}

public class PoolEntry
{
	private RAGE.Elements.Entity m_Entity;
	private float m_fDist;

	public PoolEntry(RAGE.Elements.Entity entity, float fDist)
	{
		m_Entity = entity;
		m_fDist = fDist;
	}

	public T GetEntity<T>()
	{
		return (T)Convert.ChangeType(m_Entity, typeof(T));
	}

	public float GetDistance()
	{
		return m_fDist;
	}
}

public static class OptimizationCachePool
{
	private static Dictionary<EPoolCacheKey, PoolEntry> g_dictCache = null;
	private static List<RAGE.Elements.Vehicle> g_StreamedVehicles;

	// we have a swap buffer here that we populate over multiple frames, while maintaining the safe buffer for any gets in the mean time
	private static List<RAGE.Elements.MapObject> g_StreamedObjects; // NOTE: OnStreamedIn in 0.4 doesn't work, so we have to populate it ourselves

	public const float g_fDefaultDistThreshold_Default = 3.0f;
	public const float g_fDefaultDistThreshold_Vehicle = 5.0f;
	public const float g_fDefaultDistThreshold_Render = 50.0f;
	public const float g_fDefaultDistThreshold_Audio = 30.0f;
	public const float g_fDefaultDistThreshold_Large = 100.0f;

	public static void Init()
	{
		g_dictCache = new Dictionary<EPoolCacheKey, PoolEntry>();
		g_StreamedVehicles = new List<RAGE.Elements.Vehicle>();
		g_StreamedObjects = new List<RAGE.Elements.MapObject>();

		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
		RageEvents.RAGE_OnEntityStreamOut += OnStreamOut;

		QueueMultiFrameWork();
	}

	private static void QueueMultiFrameWork()
	{
		// shared vars
		List<RAGE.Elements.MapObject> lstStreamedObjectsSwap = null;
		Dictionary<EPoolCacheKey, RAGE.Elements.Entity> dictEntities = null;
		Dictionary<EPoolCacheKey, float> dictDistances = null;
		float fSmallestDistance_Objects = 0.0f;
		float fSmallestDistance_Markers = 0.0f;

		float fDistThreshold_Default = g_fDefaultDistThreshold_Default;
		float fDistThreshold_Vehicle = g_fDefaultDistThreshold_Vehicle;
		float fDistThreshold_Audio = g_fDefaultDistThreshold_Audio;

		MultiFrameWorkLoad workLoad = new MultiFrameWorkLoad(EWorkLoadProcessingType.FrameMillisecondsBudget, 0.7,
		(Queue<object> workQueue) => // init - this function is recalled when this is a looped multiframe queue, so you'll want to clear out any temp vars
		{
			lstStreamedObjectsSwap = new List<RAGE.Elements.MapObject>();
			dictEntities = new Dictionary<EPoolCacheKey, RAGE.Elements.Entity>();
			dictDistances = new Dictionary<EPoolCacheKey, float>();

			fSmallestDistance_Objects = 999999.0f;
			fSmallestDistance_Markers = 999999.0f;

			fDistThreshold_Default = g_fDefaultDistThreshold_Default;
			fDistThreshold_Vehicle = g_fDefaultDistThreshold_Vehicle;
			fDistThreshold_Audio = g_fDefaultDistThreshold_Audio;

			// queue our work items
			workQueue.AddRange(RAGE.Elements.Entities.Objects.Streamed);
			workQueue.AddRange(RAGE.Elements.Entities.Markers.Streamed);

		}, (object objectToProcess) => // tick
		{
			// process object
			RAGE.Elements.Entity entity = (RAGE.Elements.Entity)objectToProcess;
			if (entity.Type == RAGE.Elements.Type.Object)
			{
				// process
				if (RAGE.Elements.Player.LocalPlayer.Dimension == entity.Dimension)
				{
					EObjectTypes objType = DataHelper.GetEntityData<EObjectTypes>(entity, EDataNames.OBJECT_TYPE);

					RAGE.Vector3 vecObjectPos = entity.Position;
					RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
					float fDistance = WorldHelper.GetDistance(vecPlayerPos, vecObjectPos);

					const float fStreamedInDist = 500.0f;
					if (fDistance <= fStreamedInDist)
					{
						lstStreamedObjectsSwap.Add((RAGE.Elements.MapObject)entity);
					}
					else
					{
						return; // don't care if its that far out
					}

					if (objType != EObjectTypes.NONE) // No type set = we don't care for the below, but we DO care for the streamed in objects list
					{
						if (fDistance <= fSmallestDistance_Objects)
						{
							if (fDistance <= fDistThreshold_Audio)
							{
								if (objType == EObjectTypes.OBJECT_TYPE_WORLD_ITEM)
								{
									// Is it a boom box?
									EItemID itemID = DataHelper.GetEntityData<EItemID>(entity, EDataNames.ITEM_ID);
									if (itemID == EItemID.BOOMBOX)
									{
										bool isBoombox = DataHelper.GetEntityData<bool>(entity, EDataNames.BOOMBOX);

										if (isBoombox)
										{
											dictEntities[EPoolCacheKey.BoomBox] = entity;
											dictDistances[EPoolCacheKey.BoomBox] = fDistance;
										}
									}
								}
							}

							if (fDistance <= fDistThreshold_Default)
							{
								fDistThreshold_Default = fDistance;
								// WORLD ITEMS
								if (objType == EObjectTypes.OBJECT_TYPE_WORLD_ITEM)
								{
									dictEntities[EPoolCacheKey.WorldItem] = entity;
									dictDistances[EPoolCacheKey.WorldItem] = fDistance;
								}

								// SPIKE STRIPS
								if (objType == EObjectTypes.SPIKE_STRIP)
								{
									dictEntities[EPoolCacheKey.SpikeStrip] = entity;
									dictDistances[EPoolCacheKey.SpikeStrip] = fDistance;
								}

								// SPIKE STRIPS (NO PICKUP)
								if (objType == EObjectTypes.SPIKE_STRIP_NO_PICKUP)
								{
									dictEntities[EPoolCacheKey.SpikeStrip_NoPickup] = entity;
									dictDistances[EPoolCacheKey.SpikeStrip_NoPickup] = fDistance;
								}

								// NEWS CAMERA
								if (objType == EObjectTypes.NEWS_CAMERA)
								{
									dictEntities[EPoolCacheKey.NewsCamera] = entity;
									dictDistances[EPoolCacheKey.NewsCamera] = fDistance;
								}
							}
						}
					}
				}
			}
			else if (entity.Type == RAGE.Elements.Type.Marker)
			{
				if (RAGE.Elements.Player.LocalPlayer.Dimension == entity.Dimension)
				{
					RAGE.Vector3 vecPos = entity.Position;
					RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
					float fDistance = WorldHelper.GetDistance(vecPlayerPos, vecPos);

					if (fDistance <= fSmallestDistance_Markers)
					{
						if (fDistance <= fDistThreshold_Default)
						{
							fDistThreshold_Default = fDistance;
							// ATMS / BANKS
							bool bIsATM = DataHelper.GetEntityData<bool>(entity, EDataNames.ATM);

							if (bIsATM)
							{
								dictEntities[EPoolCacheKey.ATM] = entity;
								dictDistances[EPoolCacheKey.ATM] = fDistance;
							}

							// PROPERTY MARKERS
							bool isProperty = DataHelper.GetEntityData<bool>(entity, EDataNames.PROP_ENTER) || DataHelper.GetEntityData<bool>(entity, EDataNames.PROP_EXIT);
							if (isProperty)
							{
								dictEntities[EPoolCacheKey.PropertyMarker] = entity;
								dictDistances[EPoolCacheKey.PropertyMarker] = fDistance;
							}

							// ELEVATOR MARKERS
							bool isElevator = DataHelper.GetEntityData<bool>(entity, EDataNames.ELEVATOR_ENTRANCE) || DataHelper.GetEntityData<bool>(entity, EDataNames.ELEVATOR_EXIT);
							if (isElevator)
							{
								dictEntities[EPoolCacheKey.ElevatorMarker] = entity;
								dictDistances[EPoolCacheKey.ElevatorMarker] = fDistance;
							}

							// DUTY POINT
							bool isDutyPoint = DataHelper.GetEntityData<bool>(entity, EDataNames.DUTY_POINT);
							if (isDutyPoint)
							{
								dictEntities[EPoolCacheKey.DutyPoint] = entity;
								dictDistances[EPoolCacheKey.DutyPoint] = fDistance;
							}

							// INFO MARKER
							bool isInfoMarker = DataHelper.GetEntityData<bool>(entity, EDataNames.IS_INFO_MARKER);
							if (isInfoMarker)
							{
								dictEntities[EPoolCacheKey.InfoMarker] = entity;
								dictDistances[EPoolCacheKey.InfoMarker] = fDistance;
							}
						}

						if (fDistance <= fDistThreshold_Vehicle)
						{
							fDistThreshold_Vehicle = fDistance;
							// FUEL POINTS
							bool isFuelPoint = DataHelper.GetEntityData<bool>(entity, EDataNames.FUEL_POINT);
							if (isFuelPoint)
							{
								dictEntities[EPoolCacheKey.FuelPoint] = entity;
								dictDistances[EPoolCacheKey.FuelPoint] = fDistance;
							}

							// CAR WASH
							bool isCarWash = DataHelper.GetEntityData<bool>(entity, EDataNames.CARWASH_POINT);
							if (isCarWash)
							{
								dictEntities[EPoolCacheKey.CarWash] = entity;
								dictDistances[EPoolCacheKey.CarWash] = fDistance;
							}

							// VEHICLE REPAIR
							bool isVehicleRepair = DataHelper.GetEntityData<bool>(entity, EDataNames.VEHREP_POINT);
							if (isVehicleRepair)
							{
								dictEntities[EPoolCacheKey.VehicleRepair] = entity;
								dictDistances[EPoolCacheKey.VehicleRepair] = fDistance;
							}
						}
					}
				}
			}
		}, () => // completion
		{
			// swap our buffers
			g_StreamedObjects = lstStreamedObjectsSwap;
			lstStreamedObjectsSwap = null;

			// STORE IN CACHE
			for (EPoolCacheKey key = 0; key < EPoolCacheKey.MAX; ++key)
			{
				RAGE.Elements.Entity entity;
				dictEntities.TryGetValue(key, out entity);

				if (entity != null)
				{
					g_dictCache[key] = new PoolEntry(dictEntities[key], dictDistances[key]);
				}
				else
				{
					g_dictCache.Remove(key);
				}
			}
			return true; // re queue, we want to keep doing this
		});

		MultiFrameWorkScheduler.QueueWork(workLoad);
	}

	private static void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			g_StreamedVehicles.Add((RAGE.Elements.Vehicle)entity);
		}
	}

	private static void OnStreamOut(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			g_StreamedVehicles.Remove((RAGE.Elements.Vehicle)entity);
		}
	}

	public static PoolEntry GetPoolItem(EPoolCacheKey key)
	{
		PoolEntry entry;
		g_dictCache.TryGetValue(key, out entry);
		return entry;
	}

	public static List<RAGE.Elements.Vehicle> StreamedInVehicles()
	{
		return g_StreamedVehicles;
	}

	public static List<RAGE.Elements.MapObject> StreamedInObjects()
	{
		return g_StreamedObjects;
	}


	public static RAGE.Elements.Vehicle GetNearestVehicle()
	{
		RAGE.Vector3 vecPlayerPos = PlayerHelper.GetLocalPlayerPosition();
		RAGE.Elements.Vehicle nearestVehicle = null;
		float nearestDistance = 1000f;
		foreach (RAGE.Elements.Vehicle vehicle in StreamedInVehicles())
		{
			float fDistance = WorldHelper.GetDistance(vecPlayerPos, vehicle.Position);
			if (fDistance < nearestDistance)
			{
				nearestVehicle = vehicle;
				nearestDistance = fDistance;
			}
		}

		return nearestVehicle;
	}
}