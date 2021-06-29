using System;
using System.Collections.Generic;

public abstract class BaseJob
{
	public BaseJob()
	{
	}

	public BaseJob(CPlayer a_Owner, EJobID a_JobID, EAchievementID a_StartJobAchievement, EAchievementID a_CompleteJobAchievement, string a_strJobName, EDrivingTestType a_DrivingLicenseTypeRequired, EVehicleType a_VehicleTypeRequired)
	{
		m_JobID = a_JobID;
		m_StartJobAchievement = a_StartJobAchievement;
		m_CompleteJobAchievement = a_CompleteJobAchievement;
		m_strJobName = a_strJobName;
		m_DrivingLicenseTypeRequired = a_DrivingLicenseTypeRequired;
		m_VehicleTypeRequired = a_VehicleTypeRequired;

		m_Owner.SetTarget(a_Owner);

		// Do we need to auto-start?
		if (a_Owner.Job == m_JobID)
		{
			EScriptLocation location = a_Owner.Client.Position.Y >= Constants.BorderOfLStoPaleto ? EScriptLocation.Paleto : EScriptLocation.LS;
			StartJob(true, location);
		}
	}

	public void StartJob(bool b_IsResume, EScriptLocation location)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		m_LocationInUse = location;
		m_PositionIndex = -1;
		NetworkEventSender.SendNetworkEvent_StartJob(pPlayer, m_JobID, b_IsResume);

		if (m_StartJobAchievement != EAchievementID.None)
		{
			pPlayer.AwardAchievement(m_StartJobAchievement);
		}

		OnStartJob(b_IsResume);
	}

	public abstract void OnStartJob(bool b_IsResume);
	public abstract void OnQuitJob();

	public void OnCompleteRun()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		float fReward = m_SalaryLevels[GetLevel()];

		pPlayer.PendingJobMoney += fReward;

		pPlayer.SendNotification(m_strJobName, ENotificationIcon.PiggyBank, "You earned ${0:0.00} from your job.<br>You have accrued ${1:0.00} which will be deposited to your bank account with your paycheck.", fReward, pPlayer.PendingJobMoney);

		new Logging.Log(pPlayer, Logging.ELogType.CashTransfer, null, Helpers.FormatString("{0} completed a run, earned {1} where total pending money is now {2}", pPlayer.GetCharacterName(ENameType.StaticCharacterName), fReward, pPlayer.PendingJobMoney)).execute();
	}

	public void OnAttemptStart(EScriptLocation location)
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// Have we completed this job?
		if (HasMaxLevel())
		{
			pPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "You have already completed this job.");
		}
		else if (pPlayer.Job != EJobID.None)
		{
			if (pPlayer.Job == m_JobID)
			{
				pPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "You already have this job.");
			}
			else
			{
				pPlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "You already have another job.");
			}
		}
		else if (m_DrivingLicenseTypeRequired != EDrivingTestType.None && !pPlayer.HasDrivingLicense(m_DrivingLicenseTypeRequired))
		{
			pPlayer.SendNotification(m_strJobName, ENotificationIcon.ExclamationSign, Helpers.FormatString("You need a {0} to perform this job", pPlayer.GetDrivingLicenseDisplayName(m_DrivingLicenseTypeRequired)));
		}
		else
		{
			StartJob(false, location);
			pPlayer.Job = m_JobID;
		}
	}

	public void OnCompleteJobFully()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		// Reset job
		pPlayer.Job = EJobID.None;

		// TODO: Respawn vehicle
		// TODO_RAGE_UPDATE:
#if RAGE_FIXED_THIS
		pPlayer.Client.WarpOutOfVehicle();
#else
		if (pPlayer.Client.Vehicle != null)
		{
			pPlayer.SetPositionSafe(pPlayer.Client.Vehicle.Position.Around(3.0f));
		}
#endif
		if (m_CompleteJobAchievement != EAchievementID.None)
		{
			pPlayer.AwardAchievement(m_CompleteJobAchievement);
		}
	}

	public virtual void OnAttemptQuit()
	{
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return;
		}

		pPlayer.Job = EJobID.None;

		OnQuitJob();

		// You can only have one job at a time, so it's fine for every job to self destruct on this event
		NetworkEventSender.SendNetworkEvent_StopJob(pPlayer, m_JobID);
	}

	public void AddLevel(int a_XPRequirement, float a_fSalary)
	{
		m_XPLevels.Add(a_XPRequirement);
		m_SalaryLevels.Add(a_fSalary);
	}

	public int GetLevel()
	{
		int level = 0;
		int index = 0;
		CPlayer pPlayer = m_Owner.Instance();
		if (pPlayer == null)
		{
			return level;
		}

		foreach (int xpAmount in m_XPLevels)
		{
			if (pPlayer != null && GetXP() >= xpAmount)
			{
				level = index;
			}

			index++;
		}

		return level;
	}

	public int GetXPRequiredForLevel(int level)
	{
		return m_XPLevels[level];
	}

	public bool HasMaxLevel()
	{
		// treat 1 level jobs as infinite, and say they aren't completed
		return GetNumLevels() > 1 && GetLevel() == GetNumLevels() - 1;
	}

	public int GetNumLevels()
	{
		return m_XPLevels.Count;
	}

	public string GetJobName()
	{
		return m_strJobName;
	}

	public abstract int GetXP();

	private List<int> m_XPLevels = new List<int>();
	private List<float> m_SalaryLevels = new List<float>();
	protected WeakReference<CPlayer> m_Owner { get; } = new WeakReference<CPlayer>(null);
	protected EJobID m_JobID { get; } = EJobID.None;
	protected EAchievementID m_StartJobAchievement { get; } = EAchievementID.None;
	protected EAchievementID m_CompleteJobAchievement { get; } = EAchievementID.None;
	protected int m_PositionIndex { get; set; } = -1;
	private EDrivingTestType m_DrivingLicenseTypeRequired = EDrivingTestType.None;
	protected EVehicleType m_VehicleTypeRequired { get; } = EVehicleType.None;
	private string m_strJobName = String.Empty;
	protected EScriptLocation m_LocationInUse { get; set; } = EScriptLocation.Paleto;
}