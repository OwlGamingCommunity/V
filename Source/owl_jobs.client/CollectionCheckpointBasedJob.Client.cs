using System;

enum ECollectionCheckpointBasedJob_State
{
	GetVehicle,
	GotoCheckpoint,
	ReturnToVehicle,
	WaitingServerResponse
}

abstract class CollectionCheckpointBasedJob : BaseJob
{
	public CollectionCheckpointBasedJob(EJobID a_JobID, string a_strJobName, string a_strCheckpointCompletedTitle, string a_strReturnToVehicleText, string a_strCheckpointName, string a_strStartJobInstruction, EVehicleType a_VehicleTypeRequired,
		EWorldPedType a_WorldPedID, uint a_WorldPedHash, RAGE.Vector3 a_WorldPedPos_Paleto, float a_WorldPedRotZ_Paleto, uint a_WorldPedDimension_Paleto, RAGE.Vector3 a_WorldPedPos_LS, float a_WorldPedRotZ_LS, uint a_WorldPedDimension_LS, uint a_SpriteID)
		: base(a_JobID, a_strJobName, a_strStartJobInstruction, a_VehicleTypeRequired, a_WorldPedID, a_WorldPedHash, a_WorldPedPos_Paleto, a_WorldPedRotZ_Paleto, a_WorldPedDimension_Paleto, a_WorldPedPos_LS, a_WorldPedRotZ_LS, a_WorldPedDimension_LS, a_SpriteID)
	{
		RageEvents.RAGE_OnRender += OnRender;
		m_strCheckpointCompletedTitle = a_strCheckpointCompletedTitle;
		m_strReturnToVehicleText = a_strReturnToVehicleText;
		m_strCheckpointName = a_strCheckpointName;
		m_SpriteID = a_SpriteID;
	}

	public override void CleanupAll()
	{
		if (m_CurrentCheckpointBlip != null)
		{
			m_CurrentCheckpointBlip.Destroy();
			m_CurrentCheckpointBlip = null;
		}

		if (m_CurrentCheckpointMarker != null)
		{
			m_CurrentCheckpointMarker.Destroy();
			m_CurrentCheckpointMarker = null;
		}

		m_vecCurrentCheckpointPos = null;
	}

	public override void Reset()
	{
		m_JobState = ECollectionCheckpointBasedJob_State.GetVehicle;
	}

	public override void OnEnteredJobVehicle()
	{
		if (m_JobState == ECollectionCheckpointBasedJob_State.ReturnToVehicle)
		{
			m_JobState = ECollectionCheckpointBasedJob_State.GotoCheckpoint;
		}
		else
		{
			RequestCheckpoint();
		}
	}

	private void RequestCheckpoint()
	{
		if (!m_bPendingOperation)
		{
			CleanupAll();
			m_bPendingOperation = true;
			m_JobState = ECollectionCheckpointBasedJob_State.WaitingServerResponse;

			NetworkEventSender.SendNetworkEvent_CheckpointBasedJob_GotoCheckpointState();
		}
	}

	public void OnGotoCheckpointState(RAGE.Vector3 a_vecCheckpointPos)
	{
		CleanupAll();
		m_bPendingOperation = false;
		m_JobState = ECollectionCheckpointBasedJob_State.GotoCheckpoint;

		//a_vecCheckpointPos.Z = WorldHelper.GetGroundPosition(a_vecCheckpointPos);
		m_vecCurrentCheckpointPos = a_vecCheckpointPos;

		// Create blip
		m_CurrentCheckpointBlip = new RAGE.Elements.Blip(m_SpriteID, m_vecCurrentCheckpointPos, m_strCheckpointName, shortRange: true);
		m_CurrentCheckpointBlip.SetRoute(true);
		m_CurrentCheckpointBlip.SetRouteColour(9);

		// TODO_CSHARP: Shared radius?
		m_CurrentCheckpointMarker = new RAGE.Elements.Marker(1, m_vecCurrentCheckpointPos, g_fRadius, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.RGBA(255, 194, 15, 200));
	}

	private void OnRender()
	{
		if (m_JobState == ECollectionCheckpointBasedJob_State.GotoCheckpoint)
		{
			RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
			float fDist = WorldHelper.GetDistance(vecPlayerPos, m_vecCurrentCheckpointPos);

			if (!m_bPendingOperation && fDist <= g_fRadius)
			{
				m_bPendingOperation = true;
				m_JobState = ECollectionCheckpointBasedJob_State.WaitingServerResponse;

				NetworkEventSender.SendNetworkEvent_CheckpointBasedJob_VerifyCheckpoint();
			}
		}
	}

	public void OnVerifyCheckpointResponse(bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel)
	{
		m_bPendingOperation = false;

		if (bIsValid)
		{
			if (bHasMaxLevel)
			{
				ShardManager.ShowShard(m_strCheckpointCompletedTitle, Helpers.FormatString("+{0} XP", XPGained));
				ShowJobComplete();
			}
			else if (!bDidLevelUp)
			{
				ShardManager.ShowShard(m_strCheckpointCompletedTitle, Helpers.FormatString("+{0} XP", XPGained), Helpers.FormatString("Level {0}: {1}/{2} XP.", currLevel + 1, newXP, xp_required));
			}
			else
			{
				ShardManager.ShowShard(m_strCheckpointCompletedTitle, Helpers.FormatString("+{0} XP", XPGained));
				ShowLevelUp(currLevel + 2);
			}

			if (!bHasMaxLevel)
			{
				RequestCheckpoint();
			}
			else
			{
				CleanupAll();
			}
		}
		else // Keep trying, server pos probably out of date
		{
			m_JobState = ECollectionCheckpointBasedJob_State.GotoCheckpoint;
		}
	}

	public override void OnExitedJobVehicle()
	{
		if (m_JobState == ECollectionCheckpointBasedJob_State.GotoCheckpoint)
		{
			m_JobState = ECollectionCheckpointBasedJob_State.ReturnToVehicle;
			ShardManager.ShowShard(Helpers.FormatString("{0} Job", m_strJobName), m_strReturnToVehicleText);
		}
	}

	private ECollectionCheckpointBasedJob_State m_JobState = ECollectionCheckpointBasedJob_State.GetVehicle;
	private string m_strCheckpointCompletedTitle = String.Empty;
	private string m_strReturnToVehicleText = String.Empty;
	private string m_strCheckpointName = String.Empty;
	private uint m_SpriteID = 0;
	private RAGE.Vector3 m_vecCurrentCheckpointPos = null;
	private RAGE.Elements.Blip m_CurrentCheckpointBlip = null;
	private RAGE.Elements.Marker m_CurrentCheckpointMarker = null;
	// TODO_CSHARP: A class which is marker + colshape in one?
	const float g_fRadius = 3.0f;
}