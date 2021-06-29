using System;
using System.Collections.Generic;

public class FDSystem
{
	private EFireType m_FireType = EFireType.Circular;

	private bool m_bIsParticipatingInMission = false;
	private EFireMissionID m_FireMission = EFireMissionID.ForestFire;
	string m_strMissionTitle = String.Empty;
	private float m_fRot = 0.0f;
	private RAGE.Vector3 m_vecRootPos = null;
	private RAGE.Vector3 m_vecLastPos = null;

	private uint m_NumDoneThisOffset = 0;
	private uint m_CurrentOffset = 1;

	private RAGE.Elements.Blip m_Blip = null;

	private FireObject[] m_FireObjects = new FireObject[FireConstants.MaxFires];
	private List<int> m_PendingCleanupsToTransmit = new List<int>();

	private WeakReference<ClientTimer> m_timerTransmitCleanups = new WeakReference<ClientTimer>(null);

	// interior doors
	private RAGE.Elements.MapObject g_IntDoor1 = null;
	private RAGE.Elements.MapObject g_IntDoor2 = null;
	private RAGE.Elements.MapObject g_IntDoor3 = null;
	private RAGE.Elements.MapObject g_IntDoor4 = null;

	private class FireObject
	{
		public RAGE.Vector3 Position { get; set; }
		public bool IsActive { get; set; }
		public int FireHandle { get; set; }
		public bool WasExtinguished { get; set; }
		public bool HadFireObject { get; set; }

		public FireObject(RAGE.Vector3 vecPos, bool bIsActive, int fireHandle)
		{
			Position = vecPos;
			IsActive = bIsActive;
			FireHandle = fireHandle;
			WasExtinguished = false;
			HadFireObject = false;
		}

		public void ReIgnite()
		{
			// TODO_LAUNCH: Fires need a streamer really...
			IsActive = true;
			WasExtinguished = false;
			Position.Z = WorldHelper.GetGroundPosition(Position, true);
			FireHandle = RAGE.Game.Fire.StartScriptFire(Position.X, Position.Y, Position.Z, 0, false);
		}

		public void Cleanup()
		{
			RAGE.Game.Fire.StopFireInRange(Position.X, Position.Y, Position.Z, 0.1f);

			if (FireHandle != -1)
			{
				RAGE.Game.Fire.RemoveScriptFire(FireHandle);
			}

			IsActive = false;
			WasExtinguished = true;
		}
	}

	public FDSystem()
	{
		ResetArray();

		CleanupEverything();

		RageEvents.RAGE_OnRender += OnRender;
		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.StartFireMission += OnStartFireMission;
		NetworkEvents.UpdateFireMission += OnUpdateFireMission;
		NetworkEvents.FireHeliDropWater += OnFireHeliDropWater;
		NetworkEvents.FirePartialCleanup += OnFirePartialCleanup;
		NetworkEvents.FireFullCleanup += OnFireFullCleanup;

		ScriptControls.SubscribeToControl(EScriptControlID.DropWaterFromFireHeli, SendFireHeliDropWaterRequest);

		AsyncModelLoader.RequestAsyncLoad(HashHelper.GetHashUnsigned("v_ilev_csr_garagedoor"), (uint modelLoaded) =>
		{
			g_IntDoor1 = new RAGE.Elements.MapObject(modelLoaded, new RAGE.Vector3(212.3313f, -1643.893f, 30.0f), new RAGE.Vector3(0.0f, 0.0f, 320.0f), 255, Constants.FDEMSLockerRoomDimensionPaleto);
			g_IntDoor2 = new RAGE.Elements.MapObject(modelLoaded, new RAGE.Vector3(212.3313f, -1643.893f, 32.8f), new RAGE.Vector3(0.0f, 0.0f, 320.0f), 255, Constants.FDEMSLockerRoomDimensionPaleto);

			g_IntDoor3 = new RAGE.Elements.MapObject(modelLoaded, new RAGE.Vector3(216.48f, -1647.302f, 30.0f), new RAGE.Vector3(0.0f, 0.0f, 320.0f), 255, Constants.FDEMSLockerRoomDimensionPaleto);
			g_IntDoor4 = new RAGE.Elements.MapObject(modelLoaded, new RAGE.Vector3(216.48f, -1647.302f, 32.8f), new RAGE.Vector3(0.0f, 0.0f, 320.0f), 255, Constants.FDEMSLockerRoomDimensionPaleto);
		});
	}

	private void CleanupEverything(bool bForced = false)
	{
		m_bIsParticipatingInMission = false;

		var numFires = RAGE.Game.Fire.GetNumberOfFiresInRange(RAGE.Elements.Player.LocalPlayer.Position.X, RAGE.Elements.Player.LocalPlayer.Position.Y, RAGE.Elements.Player.LocalPlayer.Position.Z, 10000.0f);
		if (numFires > 0 || bForced)
		{
			foreach (FireObject fireObject in m_FireObjects)
			{
				RAGE.Game.Fire.StopFireInRange(fireObject.Position.X, fireObject.Position.Y, fireObject.Position.Z, 1000.0f);
				fireObject.Cleanup();
			}

			ResetArray();
		}

		ClientTimerPool.MarkTimerForDeletion(ref m_timerTransmitCleanups);

		if (m_Blip != null)
		{
			m_Blip.Destroy();
			m_Blip = null;
		}
	}

	~FDSystem()
	{
		CleanupEverything();
	}

	private void OnTick()
	{
		if (m_bIsParticipatingInMission)
		{
			// Did we just clean one up?
			int index = 0;
			foreach (FireObject fireObject in m_FireObjects)
			{
				if (fireObject.IsActive)
				{
					int numFiresSpecific = RAGE.Game.Fire.GetNumberOfFiresInRange(fireObject.Position.X, fireObject.Position.Y, fireObject.Position.Z, 0.5f);

					if (numFiresSpecific == 0)
					{
						if (fireObject.HadFireObject)
						{
							m_PendingCleanupsToTransmit.Add(index);
							fireObject.Cleanup();
						}

					}
				}

				++index;
			}
		}
	}

	private void OnRender()
	{
		if (m_bIsParticipatingInMission)
		{
			int activeFires = 0;
			int extinguishedFires = 0;

			int i = 0;
			foreach (FireObject fireObject in m_FireObjects)
			{
				if (fireObject.IsActive)
				{
					int numFiresSpecific = RAGE.Game.Fire.GetNumberOfFiresInRange(fireObject.Position.X, fireObject.Position.Y, fireObject.Position.Z, 0.1f);
					if (numFiresSpecific == 1)
					{
						fireObject.HadFireObject = true;
					}
					else
					{
						fireObject.ReIgnite();
					}
					activeFires++;
				}
				else if (fireObject.WasExtinguished)
				{
					extinguishedFires++;
				}

				++i;
			}

			int totalFires = extinguishedFires + activeFires;
			float fPercent = totalFires == 0 ? 100.0f : (extinguishedFires / totalFires) * 100.0f;

			// TODO_LAUNCH: Fix this
			if (fPercent >= 100.0f)
			{
				NetworkEventSender.SendNetworkEvent_FireMissionComplete();
			}

			TextHelper.Draw2D(Helpers.FormatString("{0}: Extinguish the fire!", m_strMissionTitle), 0.5f, 0.86f, 0.5f, new RAGE.RGBA(209, 209, 209, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			//TextHelper.Draw2D(Helpers.FormatString("{0}: {1:0.00}% of fires extinguished", m_strMissionTitle, fPercent), 0.5f, 0.86f, 0.5f, new RAGE.RGBA(209, 209, 209, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
		}
	}

	private void ResetArray()
	{
		m_FireObjects = new FireObject[FireConstants.MaxFires];
		for (int i = 0; i < m_FireObjects.Length; ++i)
		{
			m_FireObjects[i] = new FireObject(new RAGE.Vector3(), false, -1);
		}

		m_bIsParticipatingInMission = false;
		m_vecLastPos = null;
		m_vecRootPos = null;
		m_NumDoneThisOffset = 0;
		m_CurrentOffset = 1;
	}

	private void OnStartFireMission(EFireMissionID MissionID, EFireType FireType, RAGE.Vector3 vecFireRoot, bool bIsParticipatingInMission, string strTitle)
	{
		ResetArray();

		m_bIsParticipatingInMission = bIsParticipatingInMission;
		m_strMissionTitle = strTitle;

		m_fRot = RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f;
		vecFireRoot.Z = WorldHelper.GetGroundPosition(vecFireRoot, true);

		// TODO_POST_LAUNCH: Why does ground pos not work for church?
		if (MissionID == EFireMissionID.ChurchFire)
		{
			vecFireRoot.Z -= 0.7f;
		}

		m_vecRootPos = vecFireRoot.CopyVector();
		m_vecLastPos = vecFireRoot.CopyVector();

		m_FireObjects[0].Position = m_vecRootPos.CopyVector();
		m_FireObjects[0].IsActive = true;
		m_FireObjects[0].ReIgnite();

		m_FireMission = MissionID;
		m_FireType = FireType;

		m_NumDoneThisOffset = 0;
		m_CurrentOffset = 1;

		m_timerTransmitCleanups = ClientTimerPool.CreateTimer(TransmitCleanups, 500);

		m_Blip = new RAGE.Elements.Blip(436, vecFireRoot, strTitle);
	}

	private uint GetOffset()
	{
		if (m_FireType == EFireType.Circular)
		{
			return 5 * m_CurrentOffset;
		}
		else if (m_FireType == EFireType.Linear)
		{
			return 1 * m_CurrentOffset;
		}
		else if (m_FireType == EFireType.SemiCircle)
		{
			return 3 * m_CurrentOffset;
		}

		return 0;
	}

	private void OnUpdateFireMission(List<int> lstSlotsToReignite)
	{
		if (m_vecLastPos != null)
		{
			foreach (int slot in lstSlotsToReignite)
			{
				if (!m_FireObjects[slot].IsActive)
				{
					m_FireObjects[slot].ReIgnite();
				}
			}
		}

		int numActiveFires = 0;
		foreach (var fireObj in m_FireObjects)
		{
			if (fireObj.IsActive)
			{
				++numActiveFires;
			}
		}

		if (m_vecLastPos != null && numActiveFires < FireConstants.MaxFires)
		{
			var NumPerOffset = GetOffset();

			if (m_NumDoneThisOffset == NumPerOffset)
			{
				m_NumDoneThisOffset = 0;
				m_CurrentOffset++;
				NumPerOffset = GetOffset();
			}

			if (m_FireType == EFireType.Circular || m_FireType == EFireType.Linear)
			{
				// make an entire radius circle of fire
				for (int i = 0; i < NumPerOffset; ++i)
				{
					if (numActiveFires < FireConstants.MaxFires)
					{
						float fRot = m_NumDoneThisOffset * (360.0f / NumPerOffset);
						float fDist = 1.0f * m_CurrentOffset;
						RAGE.Vector3 vecFirePos = m_vecLastPos.CopyVector();
						float fRadians = fRot * (3.14f / 180.0f);
						vecFirePos.X += (float)Math.Cos(fRadians) * fDist;
						vecFirePos.Y += (float)Math.Sin(fRadians) * fDist;
						vecFirePos.Z = WorldHelper.GetGroundPosition(vecFirePos, true);

						m_FireObjects[i].Position = vecFirePos;
						m_FireObjects[i].ReIgnite();

						m_NumDoneThisOffset++;

						numActiveFires++;
					}
				}
			}
			else if (m_FireType == EFireType.SemiCircle) // semi circle
			{
				// make an entire radius circle of fire
				for (int i = 0; i < NumPerOffset; ++i)
				{
					if (numActiveFires < FireConstants.MaxFires)
					{
						float fRot = (m_NumDoneThisOffset * (180.0f / NumPerOffset));
						float fDist = 1.0f * m_CurrentOffset;
						RAGE.Vector3 vecFirePos = m_vecLastPos.CopyVector();
						float fRadians = fRot * (3.14f / 180.0f);
						vecFirePos.X += (float)Math.Cos(fRadians) * fDist;
						vecFirePos.Y += (float)Math.Sin(fRadians) * fDist;
						vecFirePos.Z = WorldHelper.GetGroundPosition(vecFirePos, true);

						m_FireObjects[i].Position = vecFirePos;
						m_FireObjects[i].ReIgnite();

						m_NumDoneThisOffset++;

						numActiveFires++;
					}
				}
			}
		}
	}

	private void SendFireHeliDropWaterRequest(EControlActionType actionType)
	{
		// TODO_POST_LAUNCH: Only if in heli clientside (already checked serverside, but would save bandwidth)
		// TODO_POST_LAUNCH: Cooldown serverside
		// TODO_POST_LAUNCH: More missions + start missions from dynamic world events (e.g. car caught fire)
		// TODO_POST_LAUNCH: admin cmd to create custom fires with start pos, spread etc all dynamic/custom
		NetworkEventSender.SendNetworkEvent_FireHeliDropWaterRequest();
	}

	private void OnFireHeliDropWater(RAGE.Vector3 vecPos, bool bIsSyncer)
	{
		vecPos.Z = WorldHelper.GetGroundPosition(vecPos, true);

		// TODO_POST_LAUNCH: take ground if its higher?
		// create a circle at each offset
		for (int j = 0; j < 5; ++j)
		{
			for (int k = 0; k < 5; ++k)
			{
				float fRot = k * (360.0f / 5.0f);
				float fDist = 1.0f * j;
				RAGE.Vector3 vecCurrWaterPos = vecPos.CopyVector();
				float radians = fRot * (3.14f / 180.0f);
				vecCurrWaterPos.X += (float)Math.Cos(radians) * fDist;
				vecCurrWaterPos.Y += (float)Math.Sin(radians) * fDist;

				RAGE.Game.Fire.AddExplosion(vecCurrWaterPos.X, vecCurrWaterPos.Y, vecCurrWaterPos.Z, 24, 1.0F, true, false, 0.0f, true);
			}
		}

		// which fires should we remove?
		// Did we just clean one up?
		if (bIsSyncer)
		{
			int index = 0;
			foreach (FireObject fireObject in m_FireObjects)
			{
				if (fireObject.IsActive)
				{
					float fDistance = WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, fireObject.Position);

					if (fDistance < 20.0f)
					{
						fireObject.IsActive = false;
						m_PendingCleanupsToTransmit.Add(index);
					}
				}

				++index;
			}
		}
	}

	private void OnFirePartialCleanup(List<int> cleanedUpSlots)
	{
		foreach (int slot in cleanedUpSlots)
		{
			FireObject fireObject = m_FireObjects[slot];

			fireObject.Cleanup();
		}
	}

	private void OnFireFullCleanup()
	{
		CleanupEverything();
	}

	private void TransmitCleanups(object[] parameters)
	{
		if (m_PendingCleanupsToTransmit.Count > 0)
		{
			NetworkEventSender.SendNetworkEvent_FirePartialCleanup(m_PendingCleanupsToTransmit);
			m_PendingCleanupsToTransmit.Clear();
		}
	}
}