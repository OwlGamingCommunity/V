using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class DrivingTest
{
	public DrivingTest()
	{
		NetworkEvents.AttemptStartDrivingTest += AttemptStartDrivingTest;
		NetworkEvents.AttemptEndDrivingTest += AttemptEndDrivingTest;

		NetworkEvents.DrivingTest_GotoCheckpointState += OnDrivingTest_GotoCheckpointState;
		NetworkEvents.DrivingTest_GetNextCheckpoint += DrivingTest_GotoNextCheckpoint;
		NetworkEvents.DrivingTest_ReturnVehicle += DrivingTest_ReturnVehicle;
		NetworkEvents.OnPlayerDisconnected += OnPlayerDisconnected;

		NetworkEvents.CharacterSpawned += (CPlayer player) =>
		{
			DrivingTest_ResetForPlayer(player);
		};
	}

	public void AttemptStartDrivingTest(CPlayer a_Player, EDrivingTestType a_DrivingTestType, EScriptLocation a_Location)
	{
		m_DrivingTestInstances[a_Player].OnAttemptStart(a_DrivingTestType, a_Location);
	}

	public void AttemptEndDrivingTest(CPlayer a_Player, EDrivingTestType a_DrivingTestType)
	{
		if (m_DrivingTestInstances.ContainsKey(a_Player))
		{
			m_DrivingTestInstances[a_Player].OnAttemptQuit();
		}
	}

	public void DrivingTest_GotoNextCheckpoint(CPlayer player, bool bVisualDamage)
	{
		m_DrivingTestInstances[player].VerifyCheckpoint(bVisualDamage);
	}

	public void DrivingTest_ReturnVehicle(CPlayer player, bool bVisualDamage)
	{
		bool bSuccess = m_DrivingTestInstances[player].VerifyVehicleReturn(bVisualDamage);

		if (bSuccess)
		{
			DrivingTest_ResetForPlayer(player);
		}
	}

	public void OnDrivingTest_GotoCheckpointState(CPlayer player)
	{
		m_DrivingTestInstances[player].GotoCheckpointState();
	}

	public void DrivingTest_ResetForPlayer(CPlayer a_Player)
	{
		AttemptEndDrivingTest(a_Player, EDrivingTestType.None);
		m_DrivingTestInstances.Remove(a_Player);

		m_DrivingTestInstances.Add(a_Player, new DrivingTestInstance(a_Player));
	}

	public void OnPlayerDisconnected(CPlayer player, DisconnectionType type, string reason)
	{
		m_DrivingTestInstances.Remove(player);
	}

	private Dictionary<CPlayer, DrivingTestInstance> m_DrivingTestInstances = new Dictionary<CPlayer, DrivingTestInstance>();
}

internal class DrivingTestInstance
{
	public DrivingTestInstance(CPlayer a_Owner)
	{
		m_Owner.SetTarget(a_Owner);
		m_TestType = EDrivingTestType.None;

		// TODO: Let the player resume? At least dont re-charge them
	}

	protected WeakReference<CPlayer> m_Owner = new WeakReference<CPlayer>(null);
	public EDrivingTestType m_TestType = EDrivingTestType.None;
	private readonly float[] g_fDrivingTestCosts =
	{
		50.0f,
		75.0f,
		100.0f
	};

	public void OnAttemptStart(EDrivingTestType a_TestType, EScriptLocation a_Location)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		float fCost = g_fDrivingTestCosts[(int)a_TestType - 1];
		bool bSuccess = false;

		EItemID itemToGive = EItemID.None;
		if (a_TestType == EDrivingTestType.Bike)
		{
			itemToGive = EItemID.DRIVERS_PERMIT_BIKE;
		}
		else if (a_TestType == EDrivingTestType.Car)
		{
			itemToGive = EItemID.DRIVERS_PERMIT_CAR;
		}
		else if (a_TestType == EDrivingTestType.Truck)
		{
			itemToGive = EItemID.DRIVERS_PERMIT_LARGE;
		}

		// Have we completed this test?
		if (pPlayer.HasDrivingLicense(a_TestType))
		{
			pPlayer.SendNotification("Driving Test", ENotificationIcon.ExclamationSign, "You already have this license.", null);
		}
		else if (m_TestType != EDrivingTestType.None)
		{
			pPlayer.SendNotification("Driving Test", ENotificationIcon.ExclamationSign, "You are already taking a drivers test.", null);
		}
		else if (!pPlayer.Inventory.CanGiveItem(CItemInstanceDef.FromBasicValueNoDBID(itemToGive, 0), out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage)) // Do we have space to receive the license? (must check specific license due to stack limits being checked)
		{
			pPlayer.SendNotification("Driving License", ENotificationIcon.ExclamationSign, "You cannot receive a license:<br>{0}", strUserFriendlyMessage);
		}
		else if (!pPlayer.CanPlayerAffordCost(fCost)) // cannot afford
		{
			pPlayer.SendNotification("Driving Test", ENotificationIcon.ExclamationSign, "You do not have enough money to start the test (Required: ${0:0.00}).", fCost);
		}
		else
		{
			bSuccess = true;
		}

		if (bSuccess)
		{
			StartTest(false, a_TestType, a_Location);
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_StartDrivingTest_Rejected(pPlayer);
		}
	}

	public EDrivingTestState GetState()
	{
		return m_State;
	}

	private void Reset()
	{
		m_bHasPendingFailureFlag_Speeding = false;
		m_bHasPendingFailureFlag_Damage = false;

		m_TestType = EDrivingTestType.None;
		m_State = EDrivingTestState.Idle;
		g_PositionIndex = -1;

		m_Owner.Instance().CurrentDrivingTestType = EDrivingTestType.None;
	}

	public void OnAttemptQuit()
	{
		if (m_TestType != EDrivingTestType.None)
		{
			CPlayer pPlayer = m_Owner.Instance();
			if (pPlayer == null)
			{
				return;
			}

			Reset();
			NetworkEventSender.SendNetworkEvent_StopDrivingTest(pPlayer);
		}
	}

	private void StartTest(bool b_IsResume, EDrivingTestType a_TestType, EScriptLocation a_Location)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		Reset();

		if (!b_IsResume)
		{
			pPlayer.SubtractMoney(g_fDrivingTestCosts[(int)a_TestType - 1], PlayerMoneyModificationReason.DrivingTest);
		}

		m_TestType = a_TestType;
		m_State = EDrivingTestState.GetVehicle;
		m_Location = a_Location;

		NetworkEventSender.SendNetworkEvent_StartDrivingTest(pPlayer, m_TestType, b_IsResume);

		pPlayer.CurrentDrivingTestType = a_TestType;
	}

	private class CDrivingTestPoint
	{
		public CDrivingTestPoint(Vector3 a_vecPos, bool bIsHighSpeed)
		{
			Position = a_vecPos;
			IsHighSpeed = bIsHighSpeed;
		}

		public Vector3 Position { get; }
		public bool IsHighSpeed { get; }
	}

	// PALETO
	private CDrivingTestPoint[] g_vecDrivingTestLocations_Paleto =
	{
		new CDrivingTestPoint(new Vector3(-228.8762, 6139.773, 30.60051), true),
		new CDrivingTestPoint(new Vector3(-172.0072, 6195.972, 30.58911), true),
		new CDrivingTestPoint(new Vector3(-133.8456, 6234.295, 30.56069), true),
		new CDrivingTestPoint(new Vector3(-109.917, 6267.537, 30.59333), true),
		new CDrivingTestPoint(new Vector3(-108.3391, 6296.754, 30.75711), true),
		new CDrivingTestPoint(new Vector3(-160.5168, 6349.628, 30.87822), true),
		new CDrivingTestPoint(new Vector3(-158.4914, 6371.469, 30.82913), true),
		new CDrivingTestPoint(new Vector3(-110.141, 6417.527, 30.73945), true),
		new CDrivingTestPoint(new Vector3(-49.63955, 6481.563, 30.75468), true),
		new CDrivingTestPoint(new Vector3(-36.21801, 6528.668, 30.8738), true),
		new CDrivingTestPoint(new Vector3(-65.77367, 6558.224, 30.8726), true),
		new CDrivingTestPoint(new Vector3(-86.34071, 6544.643, 30.87277), true),
		new CDrivingTestPoint(new Vector3(-40.36288, 6517.139, 30.87289), true),
		new CDrivingTestPoint(new Vector3(-86.69323, 6452.045, 30.76427), true),
		new CDrivingTestPoint(new Vector3(-164.4112, 6374.099, 30.82167), true),
		new CDrivingTestPoint(new Vector3(-249.6403, 6289.001, 30.80185), true),
		new CDrivingTestPoint(new Vector3(-292.754, 6245.48, 30.80241), true),
		new CDrivingTestPoint(new Vector3(-282.0453, 6204.872, 30.82177), true),
		new CDrivingTestPoint(new Vector3(-238.4923, 6160.328, 30.81376), true),
		new CDrivingTestPoint(new Vector3(-243.1552, 6142.067, 30.5807), true),
		new CDrivingTestPoint(new Vector3(-268.7105, 6115.587, 30.5977), true),
	};
	private readonly Vector3 g_vecReturnLocation_Paleto = new Vector3(-281.9797, 6121.916, 30.89708);

	// LS
	private CDrivingTestPoint[] g_vecDrivingTestLocations_LS =
	{
		new CDrivingTestPoint(new Vector3(251.3644f, -353.239f, 43.67931f), true),
		new CDrivingTestPoint(new Vector3(171.1193f, -365.4142f, 42.57646f), true),
		new CDrivingTestPoint(new Vector3(85.88465f, -608.2816f, 43.43896f), true),
		new CDrivingTestPoint(new Vector3(34.97386f, -746.627f, 43.42494f), true),
		new CDrivingTestPoint(new Vector3(-24.18571f, -926.848f, 28.61119f), true),
		new CDrivingTestPoint(new Vector3(79.0362f, -989.9647f, 28.59963f), true),
		new CDrivingTestPoint(new Vector3(131.9969f, -943.8701f, 29.0f), true),
		new CDrivingTestPoint(new Vector3(167.387f, -845.6835f, 30.25391f), true),
		new CDrivingTestPoint(new Vector3(217.9723f, -707.1271f, 34.69532f), true),
		new CDrivingTestPoint(new Vector3(256.9854f, -608.8036f, 41.8f), true),
		new CDrivingTestPoint(new Vector3(303.7235f, -491.0555f, 42.53601f), true),
		new CDrivingTestPoint(new Vector3(402.1379f, -396.7172f, 46.03351f), true),
		new CDrivingTestPoint(new Vector3(458.8119f, -340.6398f, 46.51981f), true),
		new CDrivingTestPoint(new Vector3(446.6068f, -299.4415f, 48.19108f), true),
		new CDrivingTestPoint(new Vector3(378.9279f, -276.2753f, 53.0f), true),
		new CDrivingTestPoint(new Vector3(341.4642f, -291.6731f, 53.12204f), true),
		new CDrivingTestPoint(new Vector3(321.2863f, -336.9435f, 47.14926f), true),
		new CDrivingTestPoint(new Vector3(311.9469f, -359.8846f, 44.33186f), true),
		new CDrivingTestPoint(new Vector3(288.9134f, -366.9492f, 44.32549f), true),
		new CDrivingTestPoint(new Vector3(281.6101f, -359.386f, 44.27717f), true),
	};
	private readonly Vector3 g_vecReturnLocation_LS = new Vector3(281.1627f, -346.8235f, 44.40f);

	private CDrivingTestPoint[] GetPositionVectorToUse()
	{
		return m_Location == EScriptLocation.Paleto ? g_vecDrivingTestLocations_Paleto : g_vecDrivingTestLocations_LS;
	}

	private Vector3 GetReturnVectorToUse()
	{
		return m_Location == EScriptLocation.Paleto ? g_vecReturnLocation_Paleto : g_vecReturnLocation_LS;
	}

	public void GotoNextCheckpoint(bool a_bSuccess)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		if (a_bSuccess)
		{
			// don't check speed etc on first checkpoint
			if (g_PositionIndex >= 0)
			{
				// TODO_POST_LAUNCH: Sync speed with actual speed from hud
				bool bIsHighSpeed = GetPositionVectorToUse()[g_PositionIndex].IsHighSpeed;
				float fSpeedThreshold = bIsHighSpeed ? 55.0f : 25.0f;

				Vector3 vecVel = API.Shared.GetEntityVelocity(pPlayer.Client.Vehicle);
				float fSpeed = (float)Math.Sqrt(vecVel.X * vecVel.X + vecVel.Y * vecVel.Y + vecVel.Z * vecVel.Z);

				if (fSpeed > fSpeedThreshold)
				{
					m_bHasPendingFailureFlag_Speeding = true;
				}

				if (pPlayer.Client.Vehicle.Health < 1000.0f)
				{
					m_bHasPendingFailureFlag_Damage = true;
				}
			}

			g_PositionIndex++;
		}

		// Is it the last one?
		if (g_PositionIndex == GetPositionVectorToUse().Length)
		{
			m_State = EDrivingTestState.ReturnVehicle;
			Vector3 vecReturnLocation = GetReturnVectorToUse();
			NetworkEventSender.SendNetworkEvent_DrivingTest_GotoReturnVehicle(pPlayer, a_bSuccess, vecReturnLocation.X, vecReturnLocation.Y, vecReturnLocation.Z);
		}
		else
		{
			m_State = EDrivingTestState.GotoCheckpoint;
			Vector3 vecCheckpointPos = GetPositionVectorToUse()[g_PositionIndex].Position;
			NetworkEventSender.SendNetworkEvent_DrivingTest_GotoNextCheckpoint(pPlayer, a_bSuccess, vecCheckpointPos);
		}
	}

	public void GotoCheckpointState()
	{
		if (m_State == EDrivingTestState.GetVehicle)
		{
			GotoNextCheckpoint(true);
		}
	}

	public void VerifyCheckpoint(bool bVisualDamage)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// Are we expecting this state?
		if ((m_State == EDrivingTestState.GotoCheckpoint && g_PositionIndex != -1))
		{
			m_State = EDrivingTestState.GotoCheckpoint;

			// Are we in a driving test vehicle suitable for this type of test?
			if (pPlayer.IsInVehicleReal)
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);

				if (pVehicle != null)
				{
					EVehicleType vehType = pVehicle.VehicleType;
					bool bInCorrectVehicle = false;

					if (m_TestType == EDrivingTestType.Bike && vehType == EVehicleType.DrivingTest_Bike)
					{
						bInCorrectVehicle = true;
					}
					else if (m_TestType == EDrivingTestType.Car && vehType == EVehicleType.DrivingTest_Car)
					{
						bInCorrectVehicle = true;
					}
					else if (m_TestType == EDrivingTestType.Truck && vehType == EVehicleType.DrivingTest_Truck)
					{
						bInCorrectVehicle = true;
					}

					if (bInCorrectVehicle)
					{
						if (bVisualDamage)
						{
							m_bHasPendingFailureFlag_Damage = true;
						}

						// Are we nearby?
						Vector3 vecCheckpointPos = GetPositionVectorToUse()[g_PositionIndex].Position;
						bool bWithinCheckpoint = pPlayer.IsWithinDistanceOf(vecCheckpointPos, GetRadiusToUse(), Constants.DefaultWorldDimension);
						GotoNextCheckpoint(bWithinCheckpoint);
					}
				}
			}
		}
	}

	public bool VerifyVehicleReturn(bool bVisualDamage)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return false;
		}

		bool bIsValid = false;

		// Are we expecting this state?
		if (m_State == EDrivingTestState.ReturnVehicle && g_PositionIndex != -1)
		{
			// Are we in a trucker job vehicle?
			if (pPlayer.IsInVehicleReal)
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);

				if (pVehicle != null)
				{
					EVehicleType vehType = pVehicle.VehicleType;

					bool bInCorrectVehicle = false;

					if (m_TestType == EDrivingTestType.Bike && vehType == EVehicleType.DrivingTest_Bike)
					{
						bInCorrectVehicle = true;
					}
					else if (m_TestType == EDrivingTestType.Car && vehType == EVehicleType.DrivingTest_Car)
					{
						bInCorrectVehicle = true;
					}
					else if (m_TestType == EDrivingTestType.Truck && vehType == EVehicleType.DrivingTest_Truck)
					{
						bInCorrectVehicle = true;
					}

					if (bInCorrectVehicle)
					{
						// Are we nearby?
						if (pPlayer.IsWithinDistanceOf(GetReturnVectorToUse(), GetRadiusToUse(), Constants.DefaultWorldDimension))
						{
							if (bVisualDamage)
							{
								m_bHasPendingFailureFlag_Damage = true;
							}

							m_State = EDrivingTestState.Idle;
							bIsValid = true;
						}
					}
				}
			}

			bool bPassed = !m_bHasPendingFailureFlag_Speeding && !m_bHasPendingFailureFlag_Damage;

			NetworkEventSender.SendNetworkEvent_DrivingTest_GotoVehicleReturned(pPlayer, bIsValid, bPassed, m_bHasPendingFailureFlag_Speeding, m_bHasPendingFailureFlag_Damage);

			// Only process if it was valid
			if (bIsValid)
			{
				CVehicle DMVVehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);
				// TODO_RAGE_UPDATE:
#if RAGE_FIXED_THIS
				pPlayer.Client.WarpOutOfVehicle();
#else
				pPlayer.SetPositionSafe(GetReturnVectorToUse());
#endif

				// HACK: We respawn it 1 second later to stop warping the player with them, we need to let RAGE client process the removal
				MainThreadTimerPool.CreateEntityTimer((object[] parameters) =>
				{
					if (DMVVehicle != null)
					{
						DMVVehicle.Respawn(true);
					}
				}, 100, DMVVehicle, 1);

				// TODO: What do we do if the person can't hold the item?
				if (bPassed)
				{
					EItemID itemToGive = EItemID.None;
					if (m_TestType == EDrivingTestType.Bike)
					{
						itemToGive = EItemID.DRIVERS_PERMIT_BIKE;
					}
					else if (m_TestType == EDrivingTestType.Car)
					{
						itemToGive = EItemID.DRIVERS_PERMIT_CAR;
					}
					else if (m_TestType == EDrivingTestType.Truck)
					{
						itemToGive = EItemID.DRIVERS_PERMIT_LARGE;
					}

					CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(itemToGive, pPlayer.ActiveCharacterDatabaseID);
					pPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, null);
				}
			}

			return bIsValid;
		}

		return false;
	}

	private float GetRadiusToUse()
	{
		if (m_TestType == EDrivingTestType.Bike)
		{
			return DrivingTestConstants.g_ColshapeRadius_Small;
		}
		else if (m_TestType == EDrivingTestType.Car)
		{
			return DrivingTestConstants.g_ColshapeRadius_Medium;
		}
		else if (m_TestType == EDrivingTestType.Truck)
		{
			return DrivingTestConstants.g_ColshapeRadius_Large;
		}

		return DrivingTestConstants.g_ColshapeRadius_Small;
	}

	private int g_PositionIndex = -1;
	private EDrivingTestState m_State = EDrivingTestState.GetVehicle;
	private EScriptLocation m_Location = EScriptLocation.Paleto;
	private bool m_bHasPendingFailureFlag_Speeding = false;
	private bool m_bHasPendingFailureFlag_Damage = false;
}