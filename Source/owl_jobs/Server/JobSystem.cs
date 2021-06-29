using GTANetworkAPI;
using System;
using System.Collections.Generic;

public class JobSystem
{
	private PaydaySystem m_PayDaySystem = new PaydaySystem();

	public JobSystem()
	{
		CommandManager.RegisterCommand("jobstats", "Shows your character job statistics", new Action<CPlayer, CVehicle>(JobStatsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		// TODO_WORKFLOW: Make non static, just construct it here
		TaxiDriverJob.Init();
	}

	public void Init()
	{
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;
		NetworkEvents.OnPlayerDisconnected += OnPlayerDisconnected;
		NetworkEvents.AttemptStartJob += OnAttemptStartJob;
		NetworkEvents.AttemptQuitJob += OnAttemptQuitJob;
		NetworkEvents.CharacterChangeRequested += ResetJobs;
		NetworkEvents.CharacterSpawned += ResetJobs;

		NetworkEvents.StartFishing_Approved += OnStartFishingApproved;
		NetworkEvents.RequestStopFishing += OnRequestStopFishing;
		NetworkEvents.Fishing_OnBiteOutcome += OnFishingBiteOutcome;

		// CHECKPOINT BASED JOBS
		NetworkEvents.CheckpointBasedJob_GotoCheckpointState += OnCheckpointBasedJob_GotoCheckpointState;
		NetworkEvents.CheckpointBasedJob_VerifyCheckpoint += OnCheckpointBasedJob_VerifyCheckpoint;

		// PICKUP DROPOFF BASED JOBS
		NetworkEvents.PickupDropoffBasedJob_GotoPickupState += OnPickupDropoffBasedJob_GotoPickupState;
		NetworkEvents.PickupDropoffBasedJob_GotoDropoffState += OnPickupDropoffBasedJob_GotoDropoffState;
		NetworkEvents.PickupDropoffBasedJob_VerifyPickup += OnPickupDropoffBasedJob_VerifyPickup;
		NetworkEvents.PickupDropoffBasedJob_VerifyDropoff += OnPickupDropoffBasedJob_VerifyDropoff;
	}

	public void JobStatsCommand(CPlayer player, CVehicle vehicle)
	{
		Dictionary<EJobID, BaseJob> jobs = GetPlayerJobs(player);

		player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "~~~~~~~~~~~Job Stats~~~~~~~~~~~");

		if (player.FishingXP > 0)
		{
			player.PushChatMessageWithColor(EChatChannel.Notifications, 102, 255, 153, "Fishing: {0}/{1} (Level: {2})", player.FishingXP, player.GetFishingXPRequiredForNextLevel(), player.GetFishingLevel());
		}

		foreach (var (_, job) in jobs)
		{
			int currentXP = job.GetXP();
			if (currentXP == 0)
			{
				// This conveniently also hides jobs that don't gain XP (eg. taxi, tag remover)
				continue;
			}

			int currentLevel = job.GetLevel() + 1;
			if (job.HasMaxLevel())
			{
				player.PushChatMessageWithColor(
					EChatChannel.Notifications,
					102,
					255,
					153,
					"{0}: Maximum Level Reached",
					job.GetJobName()
				);
				continue;
			}

			player.PushChatMessageWithColor(
				EChatChannel.Notifications,
				102,
				255,
				153,
				"{0}: {1}/{2} (Level: {3})",
				job.GetJobName(),
				job.GetXP(),
				job.GetXPRequiredForLevel(currentLevel),
				currentLevel
			);
		}
	}

	private void OnStartFishingApproved(CPlayer a_Player)
	{
		a_Player.CreateFishingTimer();
		a_Player.SetData(a_Player.Client, EDataNames.FISHING, true, EDataType.Synced);
		a_Player.AddAnimationToQueue((int)AnimationFlags.Loop, "amb@world_human_stand_fishing@idle_a", "idle_a", false, true, true, 0, false);
	}

	public void OnRequestStopFishing(CPlayer a_Player)
	{
		a_Player.StopFishing();
	}

	public void OnFishingBiteOutcome(CPlayer a_Player, int correct, int total)
	{
		if (a_Player.IsFishing())
		{
			bool bKeepFishing = true;

			int percentChanceOfLineSnap = 0;
			const int percentChanceOfLineSnap_Success = 2;
			const int percentChanceOfLineSnap_Failed = 5;

			const float fPassPercentage = 50.0f;
			float fPercentCorrect = ((float)correct / (float)total) * 100.0f;

			if (fPercentCorrect >= fPassPercentage)
			{
				percentChanceOfLineSnap = percentChanceOfLineSnap_Success;

				CItemInstanceDef ItemInstanceDef_FishingCooler = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISH_COOLER_BOX, 0);
				CInventoryItemDefinition itemDefCooler = ItemDefinitions.g_ItemDefinitions[EItemID.FISH_COOLER_BOX];
				if (!a_Player.Inventory.HasItem(ItemInstanceDef_FishingCooler, false, out CItemInstanceDef itemFound_FishingCooler))
				{
					a_Player.SendNotification("Fishing", ENotificationIcon.ExclamationSign, "You caught the fish, but do not have a fishing cooler to store it.");
					HelperFunctions.Chat.SendAmeMessage(a_Player, "puts the fish back in the water.");
				}
				else if (a_Player.Inventory.GetItemsInsideContainer(itemFound_FishingCooler.DatabaseID).Count >= itemDefCooler.ContainerCapacity) // Do we have space in the cooler?
				{
					a_Player.SendNotification("Fishing", ENotificationIcon.ExclamationSign, "You caught the fish, but do not have enough space in your fishing cooler to store it.");
					HelperFunctions.Chat.SendAmeMessage(a_Player, "puts the fish back in the water.");
				}
				else
				{
					// give item
					float fValue = a_Player.Fishing_GetRewardForSuccessAtCurrentLevel();
					CItemInstanceDef fishItemDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISH, fValue);
					if (a_Player.Inventory.CanGiveItem(fishItemDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
					{
						a_Player.Inventory.AddItemToNextFreeSuitableSlot(fishItemDef, EShowInventoryAction.DoNothing, EItemID.FISH_COOLER_BOX, (bool bItemGranted) =>
						{
							// Give XP
							a_Player.UpdateFishingXPAfterCatch();

							a_Player.SendNotification("Fishing", ENotificationIcon.InfoSign, "You caught the fish! +10 XP! You are level {0} ({1}/{2} XP)", a_Player.GetFishingLevel(), a_Player.FishingXP, a_Player.GetFishingXPRequiredForNextLevel());
							HelperFunctions.Chat.SendAmeMessage(a_Player, "puts a fish inside their cooler.");
						});
					}
					else
					{
						a_Player.SendNotification("Fishing", ENotificationIcon.ExclamationSign, "You caught the fish, but can not store it due to: {0}.", strUserFriendlyMessage);
						HelperFunctions.Chat.SendAmeMessage(a_Player, "puts the fish back in the water.");
					}

				}
			}
			else
			{
				percentChanceOfLineSnap = percentChanceOfLineSnap_Failed;
				a_Player.SendNotification("Fishing", ENotificationIcon.ExclamationSign, "You failed to catch the fish!");
			}

			// consume one piece of fishing line
			// Verify ownership to avoid malicious clients
			foreach (CItemInstanceDef item in a_Player.Inventory.GetAllItems())
			{
				if (item.ItemID == EItemID.FISHING_LINE)
				{
					CItemValueBasic itemVal = (CItemValueBasic)item.Value;

					if (itemVal.value <= 1) // remove item, we're done
					{
						a_Player.Inventory.RemoveItem(item);

						// Do we have another fishing line we can use? otherwise stop fishing
						CItemInstanceDef ItemInstanceDef_FishingLine = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISHING_LINE, 0);
						if (!a_Player.Inventory.HasItem(ItemInstanceDef_FishingLine, false, out CItemInstanceDef itemFound_FishingLine))
						{
							a_Player.SendNotification("Fishing", ENotificationIcon.InfoSign, "Your have run out of fishing line.");
							a_Player.StopFishing();
							bKeepFishing = false;
						}
						else
						{
							a_Player.SendNotification("Fishing", ENotificationIcon.InfoSign, "Your have attached a new roll of fishing line to your rod.");
							HelperFunctions.Chat.SendAmeMessage(a_Player, "attaches a new roll of fishing line to their fishing rod.");
						}
					}
					else // deduct one
					{
						itemVal.value -= 1;
						Database.Functions.Items.SaveItemValueAndStackSize(item);
						a_Player.Inventory.TransmitFullInventory(EShowInventoryAction.DoNothing);

						// if we deducted one, that means we still have some line left, let's see if it should snap
						// 5% chance
						int rand = new Random().Next(1, 100);
						if (rand <= percentChanceOfLineSnap)
						{
							a_Player.SendNotification("Fishing", ENotificationIcon.InfoSign, "Your fishing line has snapped.");
							HelperFunctions.Chat.SendAdoMessage(a_Player, "A loud ping is heard as the fishing line snaps.");

							// remove item
							a_Player.Inventory.RemoveItem(item);

							// Do we have another fishing line we can use? otherwise stop fishing
							CItemInstanceDef ItemInstanceDef_FishingLine = CItemInstanceDef.FromBasicValueNoDBID(EItemID.FISHING_LINE, 0);
							if (!a_Player.Inventory.HasItem(ItemInstanceDef_FishingLine, false, out CItemInstanceDef itemFound_FishingLine))
							{
								a_Player.StopFishing();
								bKeepFishing = false;
							}
							else
							{
								a_Player.SendNotification("Fishing", ENotificationIcon.InfoSign, "Your have attached a new roll of fishing line to your rod.");
								HelperFunctions.Chat.SendAmeMessage(a_Player, "attaches a new roll of fishing line to their fishing rod.");
							}
						}
					}

					break;
				}
			}

			if (bKeepFishing)
			{
				a_Player.CreateFishingTimer();
			}
		}
	}

	// TAXI JOB
	public static TaxiDriverJobInstance GetTaxiDriverInstance(CPlayer a_Player)
	{
		if (m_JobInstances.ContainsKey(a_Player))
		{
			if (m_JobInstances[a_Player].ContainsKey(EJobID.TaxiDriverJob))
			{
				return (TaxiDriverJobInstance)m_JobInstances[a_Player][EJobID.TaxiDriverJob];
			}
			else
			{
				return null;
			}
		}

		return null;
	}
	// END TAXI 

	// TAG REMOVER JOB
	public static TagRemoverJobInstance GetTagRemoverInstance(CPlayer a_Player)
	{
		if (m_JobInstances.ContainsKey(a_Player))
		{
			if (m_JobInstances[a_Player].ContainsKey(EJobID.TaxiDriverJob))
			{
				return (TagRemoverJobInstance)m_JobInstances[a_Player][EJobID.TagRemoverJob];
			}
			else
			{
				return null;
			}
		}

		return null;
	}
	// END TAG REMOVER 

	// CHECKPOINT BASED JOBS
	private void OnCheckpointBasedJob_GotoCheckpointState(CPlayer a_Player)
	{
		if (a_Player.Job != EJobID.None)
		{
			var jobInstance = m_JobInstances[a_Player][a_Player.Job];
			if (jobInstance is CollectionCheckpointBasedJob)
			{
				CollectionCheckpointBasedJob collectionJobInstance = (CollectionCheckpointBasedJob)jobInstance;
				collectionJobInstance.GotoCheckpointState();
			}
		}
	}

	private void OnCheckpointBasedJob_VerifyCheckpoint(CPlayer a_Player)
	{
		if (a_Player.Job != EJobID.None)
		{
			var jobInstance = m_JobInstances[a_Player][a_Player.Job];
			if (jobInstance is CollectionCheckpointBasedJob)
			{
				CollectionCheckpointBasedJob collectionJobInstance = (CollectionCheckpointBasedJob)jobInstance;
				collectionJobInstance.VerifyCheckpoint();
			}
		}
	}
	// END CHECKPOINT BASED JOBS

	// PICKUP DROPOFF BASED JOBS
	private void OnPickupDropoffBasedJob_GotoPickupState(CPlayer a_Player)
	{
		if (a_Player.Job != EJobID.None)
		{
			var jobInstance = m_JobInstances[a_Player][a_Player.Job];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.GotoPickupState();
			}
		}
	}

	private void OnPickupDropoffBasedJob_GotoDropoffState(CPlayer a_Player)
	{
		if (a_Player.Job != EJobID.None)
		{
			var jobInstance = m_JobInstances[a_Player][a_Player.Job];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.GotoDropoffState();
			}
		}
	}

	private void OnPickupDropoffBasedJob_VerifyPickup(CPlayer a_Player)
	{
		if (a_Player.Job != EJobID.None)
		{
			var jobInstance = m_JobInstances[a_Player][a_Player.Job];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.VerifyPickup();
			}
		}
	}

	private void OnPickupDropoffBasedJob_VerifyDropoff(CPlayer a_Player)
	{
		if (a_Player.Job != EJobID.None)
		{
			var jobInstance = m_JobInstances[a_Player][a_Player.Job];
			if (jobInstance is PickupDropoffBasedJob)
			{
				PickupDropoffBasedJob pickupDropoffJobInstance = (PickupDropoffBasedJob)jobInstance;
				pickupDropoffJobInstance.VerifyDropoff();
			}
		}
	}
	// END PICKUP DROPOFF BASED JOBS

	private void ResetJobs(CPlayer a_Player)
	{
		DepopulatePlayerJobs(a_Player);
		PopulateJobs(a_Player);

		if (a_Player.Job != EJobID.None)
		{
			NetworkEventSender.SendNetworkEvent_StopJob(a_Player, a_Player.Job);
		}
	}

	private void PopulateJobs(CPlayer a_Player)
	{
		if (m_JobInstances.ContainsKey(a_Player))
		{
			m_JobInstances.Remove(a_Player);
		}

		m_JobInstances.Add(a_Player, new Dictionary<EJobID, BaseJob>());
		m_JobInstances[a_Player].Add(EJobID.TruckerJob, new TruckerJobInstance(a_Player));
		m_JobInstances[a_Player].Add(EJobID.DeliveryDriverJob, new DeliveryDriverJobInstance(a_Player));
		m_JobInstances[a_Player].Add(EJobID.BusDriverJob, new BusDriverJobInstance(a_Player));
		m_JobInstances[a_Player].Add(EJobID.MailmanJob, new MailmanJobInstance(a_Player));
		m_JobInstances[a_Player].Add(EJobID.TrashmanJob, new TrashmanJobInstance(a_Player));
		m_JobInstances[a_Player].Add(EJobID.TaxiDriverJob, new TaxiDriverJobInstance(a_Player));
		m_JobInstances[a_Player].Add(EJobID.TagRemoverJob, new TagRemoverJobInstance(a_Player));
	}

	public Dictionary<EJobID, BaseJob> GetPlayerJobs(CPlayer player)
	{
		return m_JobInstances[player];
	}

	private void DepopulatePlayerJobs(CPlayer a_Player)
	{
		m_JobInstances.Remove(a_Player);
	}

	private void OnPlayerConnected(CPlayer a_Player)
	{
		PopulateJobs(a_Player);
	}

	public void OnPlayerDisconnected(CPlayer a_Player, DisconnectionType type, string reason)
	{
		DepopulatePlayerJobs(a_Player);
	}

	private void OnAttemptStartJob(CPlayer a_Player, EJobID a_JobID, EScriptLocation a_Location)
	{
		if (a_Player.Job == EJobID.None)
		{
			m_JobInstances[a_Player][a_JobID].OnAttemptStart(a_Location);
		}
	}

	private void OnAttemptQuitJob(CPlayer a_Player)
	{
		if (a_Player.Job != EJobID.None)
		{
			m_JobInstances[a_Player][a_Player.Job].OnAttemptQuit();
		}
	}

	private static Dictionary<CPlayer, Dictionary<EJobID, BaseJob>> m_JobInstances = new Dictionary<CPlayer, Dictionary<EJobID, BaseJob>>();
}