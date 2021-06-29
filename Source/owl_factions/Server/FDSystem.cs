using GTANetworkAPI;
using System;
using System.Collections.Generic;

// TODO_LAUNCH: Does this sync if you join after the fire was created? probably not

public class CFireMission
{
	public CFireMission(EFireMissionID a_MissionID, EFireType a_FireType, Vector3 a_vecPos, string a_Title, string a_Desciption)
	{
		MissionID = a_MissionID;
		FireType = a_FireType;
		vecPos = a_vecPos;
		Title = a_Title;
		Description = a_Desciption;
	}

	public EFireMissionID MissionID { get; }
	public EFireType FireType { get; }
	public Vector3 vecPos { get; }
	public string Title { get; }
	public string Description { get; }
}

public class FDSystem : IDisposable
{
	public FDSystem()
	{
		int interval = CalculateNewTimerInterval();
		m_SpawnFireTimer = MainThreadTimerPool.CreateGlobalTimer(SpawnFireTimer_Elapsed, interval, 1);

		// COMMANDS
		CommandManager.RegisterCommand("astartfire", "Starts a fire", new Action<CPlayer, CVehicle>(AdminStartFireCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("astopfire", "Stops a fire", new Action<CPlayer, CVehicle>(AdminStopFireCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		NetworkEvents.FireHeliDropWaterRequest += DropWater;
		NetworkEvents.FirePartialCleanup += FirePartialCleanup;

		NetworkEvents.FireMissionComplete += OnFireMissionComplete;
	}

	private int CalculateNewTimerInterval()
	{
		const int maxTimeSeconds = 1800;
		const int minTimeSeconds = 600;

		int Interval = new Random().Next(minTimeSeconds, maxTimeSeconds) * 1000;
		return Interval;
	}

	private void SpawnFireTimer_Elapsed(object[] a_Parameters = null)
	{
		// Are there enough people online for a fire?
		CFaction Faction = FactionPool.GetFDEMSFaction();
		if (Faction != null)
		{
			const int numEligibleThreshold = 2;
			int numEligible = 0;

			// Do we have enough people online & on duty?
			List<CPlayer> lstFactionMembers = Faction.GetMembers();

			foreach (CPlayer factionMember in lstFactionMembers)
			{
				if (factionMember.IsOnDutyOfType(EDutyType.Fire))
				{
					++numEligible;
				}
			}

			if (numEligible >= numEligibleThreshold)
			{
				StartFireMission();
			}
		}

		// Calculate new timer
		int interval = CalculateNewTimerInterval();
		m_SpawnFireTimer = MainThreadTimerPool.CreateGlobalTimer(SpawnFireTimer_Elapsed, interval, 1);
	}

	WeakReference<MainThreadTimer> m_SpawnFireTimer = new WeakReference<MainThreadTimer>(null);

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{

	}

	private enum EFireState
	{
		OnFire,
		Extinguished,
		Inactive
	}

	private EFireState[] m_bFireStates = new EFireState[FireConstants.MaxFires];

	public void AdminStartFireCommand(CPlayer TargetPlayer, CVehicle TargetVehicle)
	{
		StartFireMission();

		new Logging.Log(TargetPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/astartfire")).execute();
	}

	public void DropWater(CPlayer player)
	{
		if (player.IsInFactionOfType(EFactionType.Medical) && player.IsOnDutyOfType(EDutyType.Fire))
		{
			CVehicle PlayerVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);

			if (PlayerVehicle != null)
			{
				if (PlayerVehicle.HasModel(VehicleHash.Polmav) && PlayerVehicle.IsFactionCar())
				{
					bool bHeliIsMedicalFaction = false;

					List<CFaction> lstGovtFactions = FactionPool.GetGovernmentFactions();
					foreach (CFaction govtFaction in lstGovtFactions)
					{
						if (govtFaction.Type == EFactionType.Medical)
						{
							if (PlayerVehicle.OwnedByFactionID(govtFaction.FactionID))
							{
								bHeliIsMedicalFaction = true;
							}
						}
					}

					if (bHeliIsMedicalFaction)
					{

						ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
						foreach (var iterPlayer in players)
						{
							NetworkEventSender.SendNetworkEvent_FireHeliDropWater(player, player.Client.Position, iterPlayer == player);
						}
					}
				}
			}
		}
	}

	public void AdminStopFireCommand(CPlayer TargetPlayer, CVehicle TargetVehicle)
	{
		OnFireExtinguished(EFireResult.Admin);

		new Logging.Log(TargetPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/astopfire")).execute();
	}

	public static Dictionary<EFireMissionID, CFireMission> g_FireMissionDefinitions = new Dictionary<EFireMissionID, CFireMission>((int)EFireMissionID.MAX)
	{
		{ EFireMissionID.ForestFire, new CFireMission(EFireMissionID.ForestFire, EFireType.Circular, new Vector3(-513.5856, 6009.932, 33.59279), "Forest Fire in the Hills", "A forest fire has broken out up in the Paleto Hills.") },
		//{ EFireMissionID.ForestFire2, new CFireMission(EFireMissionID.ForestFire2, EFireType.Linear, new Vector3(-513.5856, 6009.932, 33.59279), "Other Forest Fire in the Hills", "A different forest fire has broken out up in the Paleto Hills.") },
		{ EFireMissionID.ChurchFire, new CFireMission(EFireMissionID.ChurchFire, EFireType.SemiCircle, new Vector3(-332.816, 6170.854, 32.44806), "Holy Fire", "A fire has broken out at the church.") },
		{ EFireMissionID.MotelFire, new CFireMission(EFireMissionID.MotelFire, EFireType.Circular, new Vector3(-117.8343, 6337.128, 42.36993), "Motel Fire", "A fire has broken out on the motel roof.") },
	};

	private void StartFireMission()
	{
		MainThreadTimerPool.MarkTimerForDeletion(m_FireUpdateTimer);
		MainThreadTimerPool.MarkTimerForDeletion(m_FireExpireTimer);

		for (int i = 0; i < m_bFireStates.Length; ++i)
		{
			m_bFireStates[i] = EFireState.Inactive;
		}

		// TODO: set state as timer ticks
		// TODO: Stop spreading fire if fire gets put out faster than spread
		for (int i = 0; i < m_bFireStates.Length; ++i)
		{
			m_bFireStates[i] = EFireState.OnFire;
		}

		m_FireUpdateTimer = MainThreadTimerPool.CreateGlobalTimer(UpdateFire, 2000);
		m_FireExpireTimer = MainThreadTimerPool.CreateGlobalTimer(ExpireFire, 300000);

		EFireMissionID MissionID = (EFireMissionID)new Random().Next(0, (int)EFireMissionID.MAX - 1);
		MissionID = EFireMissionID.ForestFire; // TODO_STREAM: Stream only
		CFireMission fireMission = g_FireMissionDefinitions[MissionID];

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			bool bIsParticipatingInMission = false;
			if (player.IsInFactionOfType(EFactionType.Medical) && player.IsOnDutyOfType(EDutyType.Fire))
			{
				player.SendNotification(fireMission.Title, ENotificationIcon.ExclamationSign, fireMission.Description, null);
				bIsParticipatingInMission = true;
			}

			NetworkEventSender.SendNetworkEvent_StartFireMission(player, fireMission.MissionID, fireMission.FireType, fireMission.vecPos, bIsParticipatingInMission, fireMission.Title);
		}
	}

	private void ExpireFire(object[] a_Parameters = null)
	{
		OnFireExtinguished(EFireResult.Failed);
	}

	private void UpdateFire(object[] a_Parameters = null)
	{
		List<int> lstSlotsToReignite = new List<int>();

		// chance to reignite 20%
		int reigniteChance = new Random().Next(1, 5);
		if (reigniteChance == 3)
		{
			int numReignited = 0;
			int reigniteAmount = new Random().Next(0, m_bFireStates.Length);

			for (int i = 0; i < m_bFireStates.Length; ++i)
			{
				// TODO: Randomization of slots for reignite?
				if (m_bFireStates[i] == EFireState.Extinguished)
				{
					++numReignited;
					m_bFireStates[i] = EFireState.OnFire;
					lstSlotsToReignite.Add(i);

					if (numReignited >= reigniteAmount)
					{
						break;
					}
				}
			}
		}

		NetworkEventSender.SendNetworkEvent_UpdateFireMission_ForAll_IncludeEveryone(lstSlotsToReignite);
	}

	private enum EFireResult
	{
		Failed,
		Succeeded,
		Admin
	}

	private void OnFireExtinguished(EFireResult result)
	{
		MainThreadTimerPool.MarkTimerForDeletion(m_FireUpdateTimer);
		MainThreadTimerPool.MarkTimerForDeletion(m_FireExpireTimer);

		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player.IsInFactionOfType(EFactionType.Medical) && player.IsOnDutyOfType(EDutyType.Fire))
			{
				if (result == EFireResult.Succeeded)
				{
					player.SendNotification("Fire Mission", ENotificationIcon.ExclamationSign, "The fire has been extinguished. Good Job!", null);
				}
				else if (result == EFireResult.Failed)
				{
					player.SendNotification("Fire Mission", ENotificationIcon.ExclamationSign, "You failed the fire mission.", null);
				}
			}

			NetworkEventSender.SendNetworkEvent_FireFullCleanup(player);
		}
	}

	public void FirePartialCleanup(CPlayer player, List<int> cleanedUpSlots)
	{
		foreach (int slot in cleanedUpSlots)
		{
			m_bFireStates[slot] = EFireState.Extinguished;
		}

		// Is the entire fire out?
		bool bFireCompletelyOut = true;
		for (int i = 0; i < m_bFireStates.Length; ++i)
		{
			// TODO: Randomization of slots for reignite?
			if (m_bFireStates[i] == EFireState.OnFire)
			{
				bFireCompletelyOut = false;
				break;
			}
		}

		NetworkEventSender.SendNetworkEvent_FirePartialCleanup_ForAll_IncludeEveryone(cleanedUpSlots);

		if (bFireCompletelyOut)
		{
			OnFireExtinguished(EFireResult.Succeeded);
		}
	}

	private void OnFireMissionComplete(CPlayer player)
	{
		OnFireExtinguished(EFireResult.Succeeded);
	}

	private WeakReference<MainThreadTimer> m_FireUpdateTimer = new WeakReference<MainThreadTimer>(null);
	private WeakReference<MainThreadTimer> m_FireExpireTimer = new WeakReference<MainThreadTimer>(null);
}