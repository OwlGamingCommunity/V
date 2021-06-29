using GTANetworkAPI;
using System;

public class TaxiDriverJobInstance : BaseJob, IDisposable
{
	public TaxiDriverJobInstance(CPlayer a_Owner) : base(a_Owner, EJobID.TaxiDriverJob, EAchievementID.TaxiDriverJob, EAchievementID.None, "Taxi Driver", EDrivingTestType.Car, EVehicleType.TaxiJob)
	{

	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{

	}

	private void UpdateProgress(object[] a_Parameters = null)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// if DriveToLocation
		if (pPlayer.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);
			UpdateDistanceCovered(pVehicle);
		}

	}

	private void UpdateDistanceCovered(CVehicle a_pVehicle)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		if (a_pVehicle != null && a_pVehicle.VehicleType == EVehicleType.TaxiJob && pPlayer.Job == EJobID.TaxiDriverJob)
		{
			Vector3 vecCurrentPos = a_pVehicle.GTAInstance.Position;
			float fDistCoveredSinceLastUpdate = (m_vecLastPosition - vecCurrentPos).Length() * 0.000621f; // in miles
			m_vecLastPosition = vecCurrentPos;
			a_pVehicle.TaxiAddDistance(fDistCoveredSinceLastUpdate);
		}
	}

	// Store previous dist
	private Vector3 m_vecLastPosition = new Vector3();

	private enum ETaxiDriverJobState
	{
		GetVehicle,
		Idle,
		GotoPickup,
		DriveToLocation,
	}

	public void OnExitTaxiVehicle(CVehicle a_pVehicle)
	{
		// Save distance on exit
		UpdateDistanceCovered(a_pVehicle);
	}

	public void OnEnterTaxiVehicle(CVehicle a_pVehicle)
	{
		m_vecLastPosition = a_pVehicle.GTAInstance.Position;
	}

	public void ChangeFarePerMile(float fCharge)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);
		if (pVehicle != null && pVehicle.VehicleType == EVehicleType.TaxiJob && pPlayer.Job == EJobID.TaxiDriverJob)
		{
			pPlayer.SendNotification("Taxi", ENotificationIcon.InfoSign, "Fare set to ${0:0.00}", fCharge);
			pVehicle.TaxiSetCostPerMile(fCharge);
		}
	}

	public void ToggleAvailableForHire()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		if (pPlayer.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);
			if (pVehicle != null && pVehicle.VehicleType == EVehicleType.TaxiJob && pPlayer.Job == EJobID.TaxiDriverJob)
			{
				// We need to be in the drivers seat
				if (pPlayer.Client.VehicleSeat == (int)EVehicleSeat.Driver)
				{
					// Update the state
					bool bNewState = !pVehicle.GetData<bool>(pVehicle.GTAInstance, EDataNames.TAXI_AFH);
					pVehicle.SetAvailableForHire(bNewState);

					pPlayer.SendNotification("Taxi", ENotificationIcon.InfoSign, "Now {0} for hire.", bNewState ? "available" : "unavailable");

					if (bNewState)
					{
						HelperFunctions.Chat.SendScriptedAdvertisementWithSender("Unknown", "YC Taxi is now in service! Reach us through our application and get a ride to your location!", pPlayer);
					}
				}
			}
		}
	}

	public void ResetFare()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		if (pPlayer.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);
			if (pVehicle != null && pVehicle.VehicleType == EVehicleType.TaxiJob && pPlayer.Job == EJobID.TaxiDriverJob)
			{
				// We need to be in the drivers seat
				if (pPlayer.Client.VehicleSeat == (int)EVehicleSeat.Driver)
				{
					pVehicle.TaxiSetCurrentDistance(0.0f);
					pPlayer.SendNotification("Taxi", ENotificationIcon.InfoSign, "Fare has been reset", null);
				}
			}
		}
	}

	public override void OnQuitJob()
	{
		MainThreadTimerPool.MarkTimerForDeletion(m_UpdateProgressTimer);
	}

	public override void OnStartJob(bool b_IsResume)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		m_UpdateProgressTimer = MainThreadTimerPool.CreateEntityTimer(UpdateProgress, 5000, pPlayer);
	}

	public override int GetXP()
	{
		return 0;
	}

	private WeakReference<MainThreadTimer> m_UpdateProgressTimer = new WeakReference<MainThreadTimer>(null);
}