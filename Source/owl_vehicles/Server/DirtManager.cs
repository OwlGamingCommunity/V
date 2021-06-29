using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class DirtSystem : IDisposable
{
	private WeakReference<MainThreadTimer> m_UpdateDirtTimer = new WeakReference<MainThreadTimer>(null);
	private const int g_TimeBetweenUpdates = 30000;

	public DirtSystem()
	{
		m_UpdateDirtTimer = MainThreadTimerPool.CreateGlobalTimer(UpdateDirt, g_TimeBetweenUpdates);

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

	private void UpdateDirt(object[] a_Parameters = null)
	{
		foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

			if (pVehicle != null && pVehicle.DoesVehicleGetDirty() && !pVehicle.IsBeingWashed)
			{
				Vector3 vecPos = pVehicle.GTAInstance.Position;

				var weakRef = new WeakReference<CVehicle>(pVehicle);
				VehicleDirtData dirtData = null;
				foreach (var kvPair in m_dictVehicleDirtData)
				{
					if (kvPair.Key.Instance() == pVehicle)
					{
						dirtData = kvPair.Value;
						break;
					}
				}

				if (dirtData != null)
				{
					if (dirtData.m_vecLastPosition != null && !pVehicle.HasTeleportImmunity())
					{
						if (VehiclePool.GetVehicleOccupants(pVehicle).Count > 0)
						{
							float fDirtMultiplier = 0.001f; // TODO: By road type?

							// Is it raining? remove dirt instead
							if (Convert.ToInt32(NAPI.World.GetWeather()) >= 6)
							{
								fDirtMultiplier = -fDirtMultiplier;
							}


							Vector3 vecDist = vecPos - dirtData.m_vecLastPosition;

							if (vecDist.Length() > 0.2 || fDirtMultiplier < 0)
							{
								float fDirtAddition = (fDirtMultiplier * vecDist.Length() / 2.0f);
								float fNewDirtLevel = Math.Min(Math.Max(0, pVehicle.Dirt + fDirtAddition), 15.0f);

								pVehicle.Dirt = fNewDirtLevel;

								// Store position
								dirtData.m_vecLastPosition = vecPos;
							}
						}
					}
					else
					{
						dirtData.m_vecLastPosition = vecPos;
					}
				}
				else
				{
					m_dictVehicleDirtData[weakRef] = new VehicleDirtData { m_vecLastPosition = pVehicle.GTAInstance.Position };
				}
			}
		}
	}

	public void API_onPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seatId)
	{
		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

		if (pVehicle != null)
		{
			// TODO_POST_LAUNCH: Check driver
			m_dictVehicleDirtData[new WeakReference<CVehicle>(pVehicle)] = new VehicleDirtData { m_vecLastPosition = pVehicle.GTAInstance.Position };
		}
	}

	private Dictionary<WeakReference<CVehicle>, VehicleDirtData> m_dictVehicleDirtData = new Dictionary<WeakReference<CVehicle>, VehicleDirtData>();
}

internal class VehicleDirtData
{
	public VehicleDirtData()
	{

	}

	public Vector3 m_vecLastPosition = new Vector3();
}