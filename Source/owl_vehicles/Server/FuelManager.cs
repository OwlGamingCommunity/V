using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class FuelSystem : IDisposable
{
	// FUEL TANK LEVELS
	public static float[] g_fFuelTankSizes = new float[]
	{
		20.0f, // VehicleClass_Compacts
		30.0f, // VehicleClass_Sedans
		60.0f, // VehicleClass_SUVs
		40.0f, // VehicleClass_Coupes
		40.0f, // VehicleClass_Muscle
		40.0f, // VehicleClass_SportsClassics
		40.0f, // VehicleClass_Sports
		40.0f, // VehicleClass_Super
		10.0f, // VehicleClass_Motorcycles
		40.0f, // VehicleClass_OffRoad
		60.0f, // VehicleClass_Industrial 
		40.0f, // VehicleClass_Utility 
		40.0f, // VehicleClass_Vans 
		0.0f, // VehicleClass_Cycles 
		250.0f, // VehicleClass_Boats 
		0.0f, // VehicleClass_Helicopters 
		0.0f, // VehicleClass_Planes 
		60.0f, // VehicleClass_Service 
		60.0f, // VehicleClass_Emergency 
		60.0f, // VehicleClass_Military 
		60.0f, // VehicleClass_Commercial 
		0.0f, // VehicleClass_Trains 
	};

	// FUEL CONSUMPTION LEVELS PER MILE
	private static readonly float[] g_fFuelConsumptionRates = new float[]
	{
		0.25f, // VehicleClass_Compacts
		0.375f, // VehicleClass_Sedans
		1.5f, // VehicleClass_SUVs
		1.0f, // VehicleClass_Coupes
		1.0f, // VehicleClass_Muscle
		1.0f, // VehicleClass_SportsClassics
		1.0f, // VehicleClass_Sports
		2.0f, // VehicleClass_Super
		0.0825f, // VehicleClass_Motorcycles
		1.0f, // VehicleClass_OffRoad
		1.5f, // VehicleClass_Industrial 
		1.0f, // VehicleClass_Utility 
		1.0f, // VehicleClass_Vans 
		0.0f, // VehicleClass_Cycles 
		0.0825f, // VehicleClass_Boats 
		0.0f, // VehicleClass_Helicopters 
		0.0f, // VehicleClass_Planes 
		1.5f, // VehicleClass_Service 
		1.5f, // VehicleClass_Emergency 
		1.5f, // VehicleClass_Military 
		1.5f, // VehicleClass_Commercial 
		0.0f, // VehicleClass_Trains 
	};

	private WeakReference<MainThreadTimer> m_UpdateFuelTimer = new WeakReference<MainThreadTimer>(null);
	private const int g_TimeBetweenUpdates = 5000;

	public FuelSystem()
	{
		m_UpdateFuelTimer = MainThreadTimerPool.CreateGlobalTimer(UpdateFuel, g_TimeBetweenUpdates);

		RageEvents.RAGE_OnPlayerEnterVehicle += API_onPlayerEnterVehicle;
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		if (a_CleanupNativeAndManaged)
		{

		}
	}

	private void UpdateFuel(object[] a_Parameters = null)
	{
		foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

			if (pVehicle != null && pVehicle.DoesVehicleConsumeFuel())
			{
				Vector3 vecPos = pVehicle.GTAInstance.Position;

				var weakRef = new WeakReference<CVehicle>(pVehicle);
				VehicleFuelData fuelData = null;
				foreach (var kvPair in m_dictVehicleFuelData)
				{
					if (kvPair.Key.Instance() == pVehicle)
					{
						fuelData = kvPair.Value;
						break;
					}
				}

				if (fuelData != null)
				{
					if (fuelData.m_vecLastPosition != null && !pVehicle.HasTeleportImmunity())
					{
						if (fuelData.m_bVehicleHadDriver || pVehicle.GTAInstance.Occupants.Count > 0)
						{
							float fFuelTankSize = g_fFuelTankSizes[pVehicle.GTAInstance.Class];
							float fFuelTankConsuptionRatePerMeter = g_fFuelConsumptionRates[pVehicle.GTAInstance.Class] / 1609.34f;

							// Only consider Z as distance if we climbed, otherwise velocity will decide whether we were coasting or not
							if (vecPos.Z <= fuelData.m_vecLastPosition.Z)
							{
								vecPos.Z = fuelData.m_vecLastPosition.Z;
							}

							Vector3 vecDist = vecPos - fuelData.m_vecLastPosition;
							float fAvgVelocity = vecDist.Length() / g_TimeBetweenUpdates;

							float fFuelConsumed = fFuelTankConsuptionRatePerMeter * vecDist.Length();

							// clamp fuel consumed to rate per meter at a minimum so idling takes fuel
							if (fFuelConsumed < fFuelTankConsuptionRatePerMeter)
							{
								fFuelConsumed = fFuelTankConsuptionRatePerMeter;
							}

							// TODO: Use acceleration instead of velocity (maintaining speed doesnt use fuel, accelerating does)
							if (fAvgVelocity > 15.0f)
							{
								float fMaxSpeed = pVehicle.GTAInstance.MaxSpeed;

								float fVelocityAsPercentageOfMaxSpeed = (fAvgVelocity / fMaxSpeed) * 100.0f;

								float fMaximumPenalty = fFuelTankConsuptionRatePerMeter * 10.0f;
								float fPenalty = (fVelocityAsPercentageOfMaxSpeed / 100.0f) * fMaximumPenalty;

								fFuelConsumed += fPenalty;
							}

							float fCurrentFuelPercentage = pVehicle.Fuel;
							float fFuelConsumedPercentage = (fFuelConsumed / fFuelTankSize) * 100.0f;
							float fNewFuelPercentage = Math.Min(Math.Max(0, fCurrentFuelPercentage - fFuelConsumedPercentage), 100.0f);
							pVehicle.Fuel = fNewFuelPercentage;

							// Store position
							fuelData.m_bVehicleHadDriver = false;
							fuelData.m_vecLastPosition = vecPos;

							// TODO: Better location for this? it's not really related to fuel, but this event gives us it 'for free'
							pVehicle.Odometer += (vecDist.Length() / 500.0f);

							// If the fuel is zero, cut the engine
							if (pVehicle.Fuel <= 0.0f)
							{
								if (pVehicle.EngineOn)
								{
									pVehicle.EngineOn = false;

									HelperFunctions.Chat.SendObjectDoMessage(pVehicle.GTAInstance.Position, pVehicle.GTAInstance.Dimension, pVehicle.GetFullDisplayName(), "The engine sputters and turns off.");
								}
							}
						}
					}
					else
					{
						fuelData.m_vecLastPosition = vecPos;
					}
				}
				else
				{
					m_dictVehicleFuelData[weakRef] = new VehicleFuelData { m_vecLastPosition = pVehicle.GTAInstance.Position, m_bVehicleHadDriver = false };
				}
			}
		}
	}

	public void API_onPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seat)
	{
		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

		if (pVehicle != null)
		{
			// TODO_POST_LAUNCH: Check driver seat when available (next update)
			m_dictVehicleFuelData[new WeakReference<CVehicle>(pVehicle)] = new VehicleFuelData { m_vecLastPosition = pVehicle.GTAInstance.Position, m_bVehicleHadDriver = false };
		}
	}

	private Dictionary<WeakReference<CVehicle>, VehicleFuelData> m_dictVehicleFuelData = new Dictionary<WeakReference<CVehicle>, VehicleFuelData>();
}

internal class VehicleFuelData
{
	public VehicleFuelData()
	{

	}

	public Vector3 m_vecLastPosition = new Vector3();
	public bool m_bVehicleHadDriver = false;
}