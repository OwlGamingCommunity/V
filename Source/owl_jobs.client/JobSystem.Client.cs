using System.Collections.Generic;

public class JobSystem
{
	public JobSystem()
	{
		g_dictJobInstances.Add(EJobID.BusDriverJob, new BusDriverJobInstance());
		g_dictJobInstances.Add(EJobID.MailmanJob, new MailmanJobInstance());
		g_dictJobInstances.Add(EJobID.TrashmanJob, new TrashmanJobInstance());
		g_dictJobInstances.Add(EJobID.TruckerJob, new TruckerJobInstance());
		g_dictJobInstances.Add(EJobID.TaxiDriverJob, new TaxiDriverJobInstance());
		g_dictJobInstances.Add(EJobID.DeliveryDriverJob, new DeliveryDriverJobInstance());
		g_dictJobInstances.Add(EJobID.TagRemoverJob, new TagRemoverJobInstance());

		NetworkEvents.StartJob += OnStartJob;
		NetworkEvents.StopJob += OnStopJob;
		NetworkEvents.CheckpointBasedJob_GotoCheckpointState_Response += OnCheckpointBasedJob_GotoCheckpointState_Response;
		NetworkEvents.CheckpointBasedJob_VerifyCheckpoint_Response += OnCheckpointBasedJob_VerifyCheckpoint_Response;

		NetworkEvents.PickupDropoffBasedJob_VerifyDropoff_Response += OnPickupDropoffBasedJob_VerifyDropoff_Response;
		NetworkEvents.PickupDropoffBasedJob_VerifyPickup_Response += OnPickupDropoffBasedJob_VerifyPickup_Response;
		NetworkEvents.PickupDropoffBasedJob_GotoPickupState_Response += OnPickupDropoffBasedJob_GotoPickupState_Response;
		NetworkEvents.PickupDropoffBasedJob_GotoDropoffState_Response += OnPickupDropoffBasedJob_GotoDropoffState_Response;
	}
	private void OnStartJob(EJobID a_JobID, bool a_bIsResume)
	{
		if (a_JobID != EJobID.None)
		{
			g_dictJobInstances[a_JobID].StartJob(a_bIsResume);
		}
	}

	private void OnStopJob(EJobID a_JobID)
	{
		if (a_JobID != EJobID.None)
		{
			g_dictJobInstances[a_JobID].StopJob();
		}
	}

	private void OnCheckpointBasedJob_GotoCheckpointState_Response(EJobID a_JobID, RAGE.Vector3 a_vecCheckpointPos)
	{
		if (a_JobID != EJobID.None)
		{
			var jobInstance = g_dictJobInstances[a_JobID];
			if (jobInstance is CollectionCheckpointBasedJob)
			{
				CollectionCheckpointBasedJob collectionJobInstance = (CollectionCheckpointBasedJob)jobInstance;
				collectionJobInstance.OnGotoCheckpointState(a_vecCheckpointPos);
			}
		}
	}

	private void OnCheckpointBasedJob_VerifyCheckpoint_Response(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel)
	{
		if (a_JobID != EJobID.None)
		{
			var jobInstance = g_dictJobInstances[a_JobID];
			if (jobInstance is CollectionCheckpointBasedJob)
			{
				CollectionCheckpointBasedJob collectionJobInstance = (CollectionCheckpointBasedJob)jobInstance;
				collectionJobInstance.OnVerifyCheckpointResponse(bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);
			}
		}
	}

	private void OnPickupDropoffBasedJob_VerifyPickup_Response(EJobID a_JobID, bool bIsValid)
	{
		if (a_JobID != EJobID.None)
		{
			var jobInstance = g_dictJobInstances[a_JobID];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.OnVerifyPickupResponse(bIsValid);
			}
		}
	}

	private void OnPickupDropoffBasedJob_VerifyDropoff_Response(EJobID a_JobID, bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel)
	{
		if (a_JobID != EJobID.None)
		{
			var jobInstance = g_dictJobInstances[a_JobID];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.OnVerifyDropoffResponse(bIsValid, currLevel, newXP, XPGained, xp_required, bDidLevelUp, bHasMaxLevel);
			}
		}
	}

	private void OnPickupDropoffBasedJob_GotoPickupState_Response(EJobID a_JobID, RAGE.Vector3 a_vecCheckpointPos)
	{
		if (a_JobID != EJobID.None)
		{
			var jobInstance = g_dictJobInstances[a_JobID];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.OnReceivedPickupPosition(a_vecCheckpointPos);
			}
		}
	}

	private void OnPickupDropoffBasedJob_GotoDropoffState_Response(EJobID a_JobID, RAGE.Vector3 a_vecCheckpointPos)
	{
		if (a_JobID != EJobID.None)
		{
			var jobInstance = g_dictJobInstances[a_JobID];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.OnReceivedDropoffPosition(a_vecCheckpointPos);
			}
		}
	}

	private Dictionary<EJobID, BaseJob> g_dictJobInstances = new Dictionary<EJobID, BaseJob>();
}