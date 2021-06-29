using System;

internal abstract class BaseJob
{
	public BaseJob(EJobID a_JobID, string a_strJobName, string a_strStartJobInstruction, EVehicleType a_VehicleTypeRequired, EWorldPedType a_WorldPedID, uint a_WorldPedHash, RAGE.Vector3 a_WorldPedPos_Paleto, float a_WorldPedRotZ_Paleto, uint a_WorldPedDimension_Paleto, RAGE.Vector3 a_WorldPedPos_LS, float a_WorldPedRotZ_LS, uint a_WorldPedDimension_LS, uint a_SpriteID)
	{
		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ExitVehicleReal += OnExitVehicle;
		RageEvents.RAGE_OnTick_LowFrequency += OnTick;

		m_JobID = a_JobID;
		m_strJobName = a_strJobName;
		m_VehicleTypeRequired = a_VehicleTypeRequired;
		m_strStartJobInstruction = a_strStartJobInstruction;

		// World ped + blip

		// Paleto
		m_refWorldPed_Paleto = WorldPedManager.CreatePed(a_WorldPedID, a_WorldPedHash, a_WorldPedPos_Paleto, a_WorldPedRotZ_Paleto, a_WorldPedDimension_Paleto);
		m_refWorldPed_Paleto.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Interact With Job", null, () => { OnInteract(EScriptLocation.Paleto); }, false, false, 3.0f);
		m_refWorldPed_Paleto.Instance()?.AddBlip(a_SpriteID, true, a_strJobName);

		// LS
		m_refWorldPed_LS = WorldPedManager.CreatePed(a_WorldPedID, a_WorldPedHash, a_WorldPedPos_LS, a_WorldPedRotZ_LS, a_WorldPedDimension_LS);
		m_refWorldPed_LS.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Interact With Job", null, () => { OnInteract(EScriptLocation.LS); }, false, false, 3.0f);
		m_refWorldPed_LS.Instance()?.AddBlip(a_SpriteID, true, a_strJobName);
	}

	private void OnTick()
	{
		// TODO: This could probably be more efficient

		// Update world prompt
		EScriptControlID controlID = EScriptControlID.DummyNone;
		string strMessage = String.Empty;

		EJobID currentJobID = DataHelper.GetLocalPlayerEntityData<EJobID>(EDataNames.JOB_ID);

		if (currentJobID == EJobID.None)
		{
			controlID = EScriptControlID.Interact;
			strMessage = Helpers.FormatString("Start {0} Job", m_strJobName);
		}
		else if (currentJobID == m_JobID)
		{
			controlID = EScriptControlID.Interact;
			strMessage = Helpers.FormatString("Quit {0} Job", m_strJobName);
		}
		else
		{
			strMessage = "You already have another job.";
		}

		m_refWorldPed_Paleto.Instance()?.UpdateWorldHint(controlID, strMessage);
		m_refWorldPed_LS.Instance()?.UpdateWorldHint(controlID, strMessage);
	}

	private void OnInteract(EScriptLocation location)
	{
		EJobID currentJobID = DataHelper.GetLocalPlayerEntityData<EJobID>(EDataNames.JOB_ID);

		if (currentJobID == EJobID.None)
		{
			NetworkEventSender.SendNetworkEvent_AttemptStartJob(m_JobID, location);
		}
		else if (currentJobID == m_JobID)
		{
			NetworkEventSender.SendNetworkEvent_AttemptQuitJob();
		}
	}

	public void StartJob(bool a_bIsResume)
	{
		Reset();
		CleanupAll();

		if (!a_bIsResume)
		{
			ShardManager.ShowShard(Helpers.FormatString("{0} Job Started!", m_strJobName), m_strStartJobInstruction, "Return to your boss at any time to quit your shift");
		}
	}

	public void StopJob()
	{
		Reset();
		CleanupAll();
	}

	public bool IsJobActive()
	{
		EJobID currentJobID = DataHelper.GetLocalPlayerEntityData<EJobID>(EDataNames.JOB_ID);
		return currentJobID == m_JobID;
	}

	private void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		if (IsJobActive())
		{
			if (seatId <= 0)
			{
				EVehicleType vehType = DataHelper.GetEntityData<EVehicleType>(vehicle, EDataNames.VEHICLE_TYPE);

				if (vehType == m_VehicleTypeRequired)
				{
					OnEnteredJobVehicle();
				}
			}
		}
	}

	private void OnExitVehicle(RAGE.Elements.Vehicle vehicle)
	{
		if (IsJobActive())
		{
			OnExitedJobVehicle();
		}
	}

	public void ShowLevelUp(int newLevel)
	{
		ShardManager.ShowShard(Helpers.FormatString("{0} - Level Up!", m_strJobName), Helpers.FormatString("You are now level {0}", newLevel));
	}

	public void ShowJobComplete()
	{
		ShardManager.ShowShard(Helpers.FormatString("{0} - Job Complete!", m_strJobName), Helpers.FormatString("Congratulations - You have completed all levels of the {0} Job.", m_strJobName));
	}

	public abstract void Reset();
	public abstract void CleanupAll();
	public abstract void OnEnteredJobVehicle();
	public abstract void OnExitedJobVehicle();

	protected EJobID m_JobID = EJobID.None;
	protected EVehicleType m_VehicleTypeRequired = EVehicleType.None;
	protected string m_strJobName = String.Empty;
	private string m_strStartJobInstruction = String.Empty;
	protected bool m_bPendingOperation = false;
	private WeakReference<CWorldPed> m_refWorldPed_Paleto = new WeakReference<CWorldPed>(null);
	private WeakReference<CWorldPed> m_refWorldPed_LS = new WeakReference<CWorldPed>(null);
}