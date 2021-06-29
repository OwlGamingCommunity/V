using System;

enum EPickupDropoffBasedJob_State
{
	GetVehicle,
	GotoPickup,
	GotoDropoff,
	ReturnToVehicle,
	WaitingServerResponse
}

abstract class PickupDropoffBasedJob : BaseJob
{
	public PickupDropoffBasedJob(EJobID a_JobID, string a_strJobName, string a_strCheckpointCompletedTitle, string a_strLoadCollectedTitle, string a_strLoadCollectedMessage, string a_strReturnToVehicleText, string a_strPickup, string a_strDropoff, string a_strStartJobInstruction,
		EVehicleType a_VehicleTypeRequired, EWorldPedType a_WorldPedID, uint a_WorldPedHash, RAGE.Vector3 a_WorldPedPos_Paleto, float a_WorldPedRotZ_Paleto, uint a_WorldPedDimension_Paleto, RAGE.Vector3 a_WorldPedPos_LS, float a_WorldPedRotZ_LS, uint a_WorldPedDimension_LS, uint a_SpriteID)
		: base(a_JobID, a_strJobName, a_strStartJobInstruction, a_VehicleTypeRequired, a_WorldPedID, a_WorldPedHash, a_WorldPedPos_Paleto, a_WorldPedRotZ_Paleto, a_WorldPedDimension_Paleto, a_WorldPedPos_LS, a_WorldPedRotZ_LS, a_WorldPedDimension_LS, a_SpriteID)
	{
		RageEvents.RAGE_OnRender += OnRender;
		m_strDropoffCompletedTitle = a_strCheckpointCompletedTitle;
		m_strLoadCollectedTitle = a_strLoadCollectedTitle;
		m_strLoadCollectedMessage = a_strLoadCollectedMessage;
		m_strReturnToVehicleText = a_strReturnToVehicleText;
		m_strPickup = a_strPickup;
		m_strDropoff = a_strDropoff;
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
		m_JobState = EPickupDropoffBasedJob_State.GetVehicle;
	}

	public override void OnEnteredJobVehicle()
	{
		if (HasExistingCheckpointToRestore())
		{
			m_JobState = m_CurrentCheckpointMarkerType;
			return;
		}

		GotoPickupState();
	}

	private bool HasExistingCheckpointToRestore()
	{
		return m_CurrentCheckpointMarkerType != EPickupDropoffBasedJob_State.GetVehicle;
	}

	private void GotoPickupState()
	{
		if (!m_bPendingOperation)
		{
			CleanupAll();
			m_bPendingOperation = true;
			m_JobState = EPickupDropoffBasedJob_State.WaitingServerResponse;

			NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_GotoPickupState();
		}
	}

	// TODO_POST_LAUNCH: Force the player to use the trailer
	// TODO_POST_LAUNCH: Exiting vehicle probably keeps marker etc, we should hide and recreate on re-enter
	// TODO_POST_LAUNCH: RE-enter vehicle generates a new pickup pos, also resets progress if you were on drop off phase
	public void OnReceivedPickupPosition(RAGE.Vector3 a_vecCheckpointPos)
	{
		CleanupAll();
		m_bPendingOperation = false;
		m_JobState = EPickupDropoffBasedJob_State.GotoPickup;

		//a_vecCheckpointPos.Z = WorldHelper.GetGroundPosition(a_vecCheckpointPos);
		m_vecCurrentCheckpointPos = a_vecCheckpointPos;

		// Create blip
		m_CurrentCheckpointBlip = new RAGE.Elements.Blip(m_SpriteID, m_vecCurrentCheckpointPos, m_strPickup, shortRange: true);
		m_CurrentCheckpointBlip.SetRoute(true);
		m_CurrentCheckpointBlip.SetRouteColour(9);

		// TODO_CSHARP: Shared radius?
		m_CurrentCheckpointMarkerType = EPickupDropoffBasedJob_State.GotoPickup;
		m_CurrentCheckpointMarker = new RAGE.Elements.Marker(1, m_vecCurrentCheckpointPos, g_fRadius, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.RGBA(255, 194, 15, 200));
	}

	private void GotoDropoffState()
	{
		if (!m_bPendingOperation)
		{
			CleanupAll();
			m_bPendingOperation = true;
			m_JobState = EPickupDropoffBasedJob_State.WaitingServerResponse;

			NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_GotoDropoffState();
		}
	}

	public void OnReceivedDropoffPosition(RAGE.Vector3 a_vecCheckpointPos)
	{
		CleanupAll();
		m_bPendingOperation = false;
		m_JobState = EPickupDropoffBasedJob_State.GotoDropoff;

		//a_vecCheckpointPos.Z = WorldHelper.GetGroundPosition(a_vecCheckpointPos);
		m_vecCurrentCheckpointPos = a_vecCheckpointPos;

		// Create blip
		m_CurrentCheckpointBlip = new RAGE.Elements.Blip(m_SpriteID, m_vecCurrentCheckpointPos, m_strDropoff, shortRange: true);
		m_CurrentCheckpointBlip.SetRoute(true);
		m_CurrentCheckpointBlip.SetRouteColour(9);

		// TODO_CSHARP: Shared radius?
		m_CurrentCheckpointMarkerType = EPickupDropoffBasedJob_State.GotoDropoff;
		m_CurrentCheckpointMarker = new RAGE.Elements.Marker(1, m_vecCurrentCheckpointPos, g_fRadius, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.RGBA(0, 255, 0, 200));
	}

	public void OnVerifyPickupResponse(bool bIsValid)
	{
		m_bPendingOperation = false;

		if (bIsValid)
		{
			ShardManager.ShowShard(m_strLoadCollectedTitle, m_strLoadCollectedMessage);
			GotoDropoffState();
		}
		else // Keep trying, server pos probably out of date
		{
			//m_JobState = EPickupDropoffBasedJob_State.GotoPickup;
		}
	}

	public void OnVerifyDropoffResponse(bool bIsValid, int currLevel, int newXP, int XPGained, int xp_required, bool bDidLevelUp, bool bHasMaxLevel)
	{
		m_bPendingOperation = false;

		if (bIsValid)
		{
			if (bHasMaxLevel)
			{
				ShardManager.ShowShard(m_strDropoffCompletedTitle, Helpers.FormatString("+{0} XP", XPGained));
				ShowJobComplete();
			}
			else if (!bDidLevelUp)
			{
				ShardManager.ShowShard(m_strDropoffCompletedTitle, Helpers.FormatString("+{0} XP", XPGained), Helpers.FormatString("Level {0}: {1}/{2} XP.", currLevel + 1, newXP, xp_required));
			}
			else
			{
				ShardManager.ShowShard(m_strDropoffCompletedTitle, Helpers.FormatString("+{0} XP", XPGained));
				ShowLevelUp(currLevel + 2);
			}

			if (!bHasMaxLevel)
			{
				GotoPickupState();
			}
			else
			{
				CleanupAll();
			}
		}
		else // Keep trying, server pos probably out of date
		{
			m_JobState = EPickupDropoffBasedJob_State.GotoDropoff;
		}
	}

	private void OnRender()
	{
		if (m_JobState == EPickupDropoffBasedJob_State.GotoPickup || m_JobState == EPickupDropoffBasedJob_State.GotoDropoff)
		{
			if (m_vecCurrentCheckpointPos != null)
			{
				RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
				float fDist = WorldHelper.GetDistance(vecPlayerPos, m_vecCurrentCheckpointPos);

				if (!m_bPendingOperation && fDist <= g_fRadius)
				{
					if (m_JobState == EPickupDropoffBasedJob_State.GotoDropoff)
					{
						NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_VerifyDropoff();
					}
					else if (m_JobState == EPickupDropoffBasedJob_State.GotoPickup)
					{
						NetworkEventSender.SendNetworkEvent_PickupDropoffBasedJob_VerifyPickup();
					}

					m_bPendingOperation = true;
					m_JobState = EPickupDropoffBasedJob_State.WaitingServerResponse;
				}
			}
		}
	}

	public override void OnExitedJobVehicle()
	{
		if (m_JobState == EPickupDropoffBasedJob_State.GotoPickup || m_JobState == EPickupDropoffBasedJob_State.GotoDropoff)
		{
			m_JobState = EPickupDropoffBasedJob_State.ReturnToVehicle;
			ShardManager.ShowShard(Helpers.FormatString("{0} Job", m_strJobName), m_strReturnToVehicleText);
		}
	}

	private EPickupDropoffBasedJob_State m_JobState = EPickupDropoffBasedJob_State.GetVehicle;
	private string m_strDropoffCompletedTitle = String.Empty;
	private string m_strLoadCollectedTitle = String.Empty;
	private string m_strLoadCollectedMessage = String.Empty;
	private string m_strReturnToVehicleText = String.Empty;
	private string m_strPickup = String.Empty;
	private string m_strDropoff = String.Empty;
	private uint m_SpriteID = 0;
	private RAGE.Vector3 m_vecCurrentCheckpointPos = null;
	private RAGE.Elements.Blip m_CurrentCheckpointBlip = null;
	private RAGE.Elements.Marker m_CurrentCheckpointMarker = null;
	private EPickupDropoffBasedJob_State m_CurrentCheckpointMarkerType = EPickupDropoffBasedJob_State.GetVehicle;
	// TODO_CSHARP: A class which is marker + colshape in one?
	const float g_fRadius = 3.0f;
}

