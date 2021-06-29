using GTANetworkAPI;
using System;
using System.Collections.Generic;

internal abstract class CollectionCheckpointBasedJob : BaseJob
{
	public CollectionCheckpointBasedJob(CPlayer a_Owner, EJobID a_JobID, EAchievementID a_StartJobAchievement, EAchievementID a_CompleteJobAchievement, string a_strJobName, EDrivingTestType a_DrivingLicenseTypeRequired, EVehicleType a_VehicleTypeRequired) : base(a_Owner, a_JobID, a_StartJobAchievement, a_CompleteJobAchievement, a_strJobName, a_DrivingLicenseTypeRequired, a_VehicleTypeRequired)
	{
		foreach (EScriptLocation location in Enum.GetValues(typeof(EScriptLocation)))
		{
			m_dictLocations.Add(location, new List<Vector3>());
		}
	}

	private enum ECollectionCheckpointBasedJobState
	{
		GetVehicle,
		GotoCheckpoint,
		ReturnToVehicle,
	}

	public void AddCheckpoint(EScriptLocation location, Vector3 vecPos)
	{
		m_dictLocations[location].Add(vecPos);
	}

	public void GotoCheckpointState()
	{
		CPlayer ownerPlayer = m_Owner.Instance();
		if (ownerPlayer == null)
		{
			return;
		}

		m_State = ECollectionCheckpointBasedJobState.GotoCheckpoint;

		m_PositionIndex++;

		if (m_PositionIndex >= m_dictLocations[m_LocationInUse].Count)
		{
			m_PositionIndex = 0;
		}

		Vector3 vecDropoffPos = m_dictLocations[m_LocationInUse][m_PositionIndex];
		NetworkEventSender.SendNetworkEvent_CheckpointBasedJob_GotoCheckpointState_Response(ownerPlayer, m_JobID, vecDropoffPos);
	}

	public abstract int OnGainXP(CPlayer a_Player, int a_XPGained);

	public void VerifyCheckpoint()
	{
		bool bIsValid = false;

		CPlayer ownerPlayer = m_Owner.Instance();
		if (ownerPlayer == null)
		{
			return;
		}

		// Are we expecting this state?
		if (m_State == ECollectionCheckpointBasedJobState.GotoCheckpoint && m_PositionIndex != -1)
		{
			// Are we in a job vehicle?
			if (ownerPlayer.IsInVehicleReal)
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(ownerPlayer.Client.Vehicle);

				if (pVehicle != null && ownerPlayer.Client.VehicleSeat == (int)EVehicleSeat.Driver)
				{
					EVehicleType vehType = pVehicle.VehicleType;

					if (vehType == m_VehicleTypeRequired)
					{
						m_State = ECollectionCheckpointBasedJobState.GotoCheckpoint;
						bIsValid = true;
					}
				}
			}
		}

		const int XPGained = 5;
		int currLevel = GetLevel();

		int newXP = 0;
		if (bIsValid)
		{
			newXP = OnGainXP(ownerPlayer, XPGained);
			OnCompleteRun();
		}

		int xp_required = GetXPRequiredForLevel(currLevel + 1);
		bool bDidLevelUp = newXP >= xp_required;

		bool bHasMaxLevel = HasMaxLevel();
		if (bHasMaxLevel)
		{
			OnCompleteJobFully();
		}

		NetworkEventSender.SendNetworkEvent_CheckpointBasedJob_VerifyCheckpoint_Response(ownerPlayer, m_JobID, bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);
	}

	// TODO_CSHARP: Shared location for client + server
	private ECollectionCheckpointBasedJobState m_State = ECollectionCheckpointBasedJobState.GetVehicle;
	private Dictionary<EScriptLocation, List<Vector3>> m_dictLocations = new Dictionary<EScriptLocation, List<Vector3>>();
}