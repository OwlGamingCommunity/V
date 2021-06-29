using GTANetworkAPI;
using System;
using System.Collections.Generic;

internal abstract class PickupDropoffBasedJob : BaseJob
{
	public PickupDropoffBasedJob(CPlayer a_Owner, EJobID a_JobID, EAchievementID a_StartJobAchievement, EAchievementID a_CompleteJobAchievement, string a_strJobName, EDrivingTestType a_DrivingLicenseTypeRequired, EVehicleType a_VehicleTypeRequired) : base(a_Owner, a_JobID, a_StartJobAchievement, a_CompleteJobAchievement, a_strJobName, a_DrivingLicenseTypeRequired, a_VehicleTypeRequired)
	{
		foreach (EScriptLocation location in Enum.GetValues(typeof(EScriptLocation)))
		{
			m_dictPickupLocations.Add(location, new List<Vector3>());
			m_dictDropoffLocations.Add(location, new List<Vector3>());
		}
	}

	private enum EPickupDropoffBasedJobState
	{
		GetVehicle,
		GotoPickup,
		GotoDropOff,
		ReturnToVehicle, // TODO: Support return to vehicle
	}

	public void AddPickupLocation(EScriptLocation location, Vector3 a_vecPos)
	{
		m_dictPickupLocations[location].Add(a_vecPos);
	}

	public void AddDropoffLocation(EScriptLocation location, Vector3 a_vecPos)
	{
		m_dictDropoffLocations[location].Add(a_vecPos);
	}

	public abstract int OnGainXP(CPlayer a_Player, int a_XPGained);

	public void GotoPickupState()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		m_State = EPickupDropoffBasedJobState.GotoPickup;

		m_PositionIndex = new Random().Next(0, m_dictPickupLocations[m_LocationInUse].Count - 1);
		Vector3 vecPickupPos = m_dictPickupLocations[m_LocationInUse][m_PositionIndex];
		NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_GotoPickupState_Response(pPlayer, m_JobID, vecPickupPos);
	}

	public void GotoDropoffState()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		m_State = EPickupDropoffBasedJobState.GotoDropOff;

		m_PositionIndex = new Random().Next(0, m_dictDropoffLocations[m_LocationInUse].Count - 1);
		Vector3 vecDropoffPos = m_dictDropoffLocations[m_LocationInUse][m_PositionIndex];
		NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_GotoDropoffState_Response(pPlayer, m_JobID, vecDropoffPos);
	}

	public void VerifyPickup()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		bool bIsValid = false;

		// Are we expecting this state?
		if (m_State == EPickupDropoffBasedJobState.GotoPickup && m_PositionIndex != -1)
		{
			if (pPlayer.IsInVehicleReal)
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);

				if (pVehicle != null)
				{
					EVehicleType vehType = pVehicle.VehicleType;

					if (vehType == m_VehicleTypeRequired)
					{
						// Are we nearby?
						Vector3 vecPickupPos = m_dictPickupLocations[m_LocationInUse][m_PositionIndex];
						if (pPlayer.IsWithinDistanceOf(vecPickupPos, g_fRadius * 3.0f, Constants.DefaultWorldDimension)) // NOTE: Multiplied by 3.0f to deal with latency between client + server events
						{
							m_State = EPickupDropoffBasedJobState.GotoDropOff;
							bIsValid = true;
						}
					}
				}
			}
		}

		NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_VerifyPickup_Response(pPlayer, m_JobID, bIsValid);
	}

	public void VerifyDropoff()
	{
		bool bIsValid = false;

		CPlayer ownerPlayer = m_Owner.Instance();
		if (ownerPlayer == null)
		{
			return;
		}

		// Are we expecting this state?
		if (m_State == EPickupDropoffBasedJobState.GotoDropOff && m_PositionIndex != -1)
		{
			if (ownerPlayer.IsInVehicleReal)
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(ownerPlayer.Client.Vehicle);

				if (pVehicle != null)
				{
					EVehicleType vehType = pVehicle.VehicleType;

					if (vehType == m_VehicleTypeRequired)
					{
						// Are we nearby?
						Vector3 vecDropoffPos = m_dictDropoffLocations[m_LocationInUse][m_PositionIndex];
						if (ownerPlayer.IsWithinDistanceOf(vecDropoffPos, g_fRadius * 3.0f, Constants.DefaultWorldDimension)) // NOTE: Multiplied by 3.0f to deal with latency between client + server events
						{
							m_State = EPickupDropoffBasedJobState.GotoPickup;
							bIsValid = true;
						}
					}
				}
			}
		}

		const int XPGained = 10;
		int currLevel = GetLevel();

		int newXP = 0;
		if (bIsValid)
		{
			newXP = OnGainXP(ownerPlayer, XPGained);
			OnCompleteRun();
		}

		int xp_required = GetXPRequiredForLevel(currLevel + 1);
		bool bDidLevelUp = (newXP) >= xp_required;

		bool bHasMaxLevel = HasMaxLevel();
		if (bHasMaxLevel)
		{
			OnCompleteJobFully();
		}

		NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_VerifyDropoff_Response(ownerPlayer, m_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);
	}

	// TODO_CSHARP: Shared location for client + server
	private const float g_fRadius = 3.0f; // NOTE: If you change this you must change the relevant .ts
	private EPickupDropoffBasedJobState m_State = EPickupDropoffBasedJobState.GetVehicle;

	private Dictionary<EScriptLocation, List<Vector3>> m_dictPickupLocations = new Dictionary<EScriptLocation, List<Vector3>>();
	private Dictionary<EScriptLocation, List<Vector3>> m_dictDropoffLocations = new Dictionary<EScriptLocation, List<Vector3>>();
}