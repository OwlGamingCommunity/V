using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class VehicleSystem : IDisposable
{
	private WeakReference<MainThreadTimer> m_SaveAllVehiclesTimer = new WeakReference<MainThreadTimer>(null);

	private CarWashStations m_CarWashStations = new CarWashStations();
	private Commands m_Commands = new Commands();
	private DirtSystem m_DirtSystem = new DirtSystem();
	private DrivingTest m_DrivingTest = new DrivingTest();
	private FuelSystem m_FuelSystem = new FuelSystem();
	private FuelStations m_FuelStations = new FuelStations();
	private TurnSignals m_TurnSignals = new TurnSignals();
	private VehicleCrusher m_VehicleCrusher = new VehicleCrusher();
	private VehicleModShop m_VehicleModShop = new VehicleModShop();
	private CVehicleRepairPoints m_CVehicleRepairPoints = new CVehicleRepairPoints();

	public VehicleSystem()
	{
		// Load vehicle data
		try
		{
			PrintLogger.LogMessage(ELogSeverity.HIGH, "VehicleSystem: Deserializing Items");

			CVehicleDefinition[] jsonData = JsonConvert.DeserializeObject<CVehicleDefinition[]>(System.IO.File.ReadAllText(System.IO.Path.Combine("dotnet", "resources", "owl_gamedata", "VehicleData.json")));

			foreach (CVehicleDefinition vehicleDef in jsonData)
			{
				VehicleDefinitions.g_VehicleDefinitions.Add(vehicleDef.Index, vehicleDef);
			}

			PrintLogger.LogMessage(ELogSeverity.HIGH, "Deserialized {0} vehicles.", VehicleDefinitions.g_VehicleDefinitions.Count);
		}
		catch (Exception ex)
		{
			PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading vehicle data: {0}", ex.ToString());
		}

		m_SaveAllVehiclesTimer = MainThreadTimerPool.CreateGlobalTimer(SaveAllVehicles, 300000);

		// COMMANDS
		CommandManager.RegisterCommand("park", "Parks the current vehicle", new Action<CPlayer, CVehicle>(ParkCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle | CommandRequirementsFlags.MustBeInVehicleWithOwnerOrAdminRights, aliases: new string[] { "parkveh" });
		CommandManager.RegisterCommand("apark", "Admin Parks the current vehicle", new Action<CPlayer, CVehicle>(AdminParkCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle | CommandRequirementsFlags.MustBeInVehicle | CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("vehdoor", "Changes the state of a vehicle door", new Action<CPlayer, CVehicle, int>(VehicleDoorCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle, aliases: new string[] { "vd" });
		CommandManager.RegisterCommand("fixveh", "Fixes a vehicle", new Action<CPlayer, CVehicle, CPlayer>(FixCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
		CommandManager.RegisterCommand("fixvehs", "Fixes all the vehicles", new Action<CPlayer, CVehicle>(FixAllCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("getpaletoveh", "Gets a vehicle from Paleto Bay", new Action<CPlayer, CVehicle, EntityDatabaseID>(GetPaletoBayVehicleCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		CommandManager.RegisterCommand("seatbelt", "Toggles the seatbelt when in a vehicle.", new Action<CPlayer, CVehicle>(OnToggleSeatbelt), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("togwindow", "Toggles the windows when in a vehicle", new Action<CPlayer, CVehicle>(OnToggleWindows), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle, aliases: new string[] { "togwindows" });
		CommandManager.RegisterCommand("setvehextra", "Sets the extra of a vehicle", new Action<CPlayer, CVehicle, int, bool>(SetVehicleExtra), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle | CommandRequirementsFlags.MustBeAdminOnDuty);

		// EVENTS
		NetworkEvents.ToggleEngine += OnPlayerToggleEngine;
		NetworkEvents.ToggleEngineStall += OnToggleEngineStall;
		NetworkEvents.ToggleVehicleLocked += OnPlayerToggleVehicleLocked;
		NetworkEvents.ToggleHeadlights += OnPlayerToggleHeadlights;
		NetworkEvents.ToggleWindows += OnPlayerToggleWindows;
		NetworkEvents.ToggleSpotlight += ToggleSpotlight;
		NetworkEvents.SetSpotlightRotation += SetSpotlightRotation;
		NetworkEvents.RadialSetDoorState += OnRadialSetDoorState;
		NetworkEvents.RadialSetLockState += OnRadialSetLockState;
		NetworkEvents.SetVehicleGear += OnSetVehicleGear;
		NetworkEvents.SyncManualVehBrakes += OnSyncManualVehBrakes;
		NetworkEvents.SyncManualVehRpm += OnSyncManualVehRpm;
		NetworkEvents.SyncVehicleHandbrake += OnSyncVehicleHandbrake;

		RageEvents.RAGE_OnPlayerEnterVehicle += API_onPlayerEnterVehicle;
		RageEvents.RAGE_OnPlayerExitVehicle += API_onPlayerExitVehicle;
		RageEvents.RAGE_OnPlayerEnterVehicleAttempt += OnPlayerEnterVehicleAttempt;

		VehiclePool.Init();

		Database.Functions.Vehicles.LoadAllVehicles(async (List<CDatabaseStructureVehicle> lstVehicles) =>
		{
			foreach (CDatabaseStructureVehicle vehicle in lstVehicles)
			{
				CVehicleDefinition vehDef = VehicleDefinitions.GetVehicleDefinitionFromAddon(vehicle.model);

				CVehicle pVehicle = await VehiclePool.CreateVehicle(vehicle.vehicleID, vehicle.vehicleType, vehicle.ownerID, vehDef == null ? (VehicleHash)vehicle.model : 0, vehicle.vecDefaultSpawnPos, vehicle.vecDefaultSpawnRot, vehicle.vecPos, vehicle.vecRot, vehicle.plateType,
					vehicle.strPlateText, vehicle.fFuel, vehicle.colPrimaryR, vehicle.colPrimaryG, vehicle.colPrimaryB, vehicle.colSecondaryR, vehicle.colSecondaryG, vehicle.colSecondaryB, vehicle.colWheel, vehicle.livery, vehicle.fDirt, vehicle.health,
					vehicle.bLocked, vehicle.bEngineOn, vehicle.iPaymentsRemaining, vehicle.iPaymentsMade, vehicle.iPaymentsMissed, vehicle.fCreditAmount, false, false, vehicle.expiryTime, vehicle.fOdometer, vehicle.bTowed, vehicle.dimension,
					vehicle.VehicleMods, vehicle.VehicleExtras, vehicle.bNeons, vehicle.neonR, vehicle.neonG, vehicle.neonB, vehicle.lastUsed, vehicle.radio, vehicle.show_plate, vehicle.token_purchase, vehicle.transmissionType, vehicle.PearlescentColor, vehDef == null ? 0 : vehicle.model).ConfigureAwait(true);

				pVehicle.Inventory.CopyInventory(vehicle.inventory);
			}

			NAPI.Util.ConsoleOutput("[VEHICLES] Loaded {0} Vehicles!", lstVehicles.Count);
		});

		VehiclePool.InitTicks();
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		if (a_CleanupNativeAndManaged)
		{

		}
	}

	private void SetVehicleExtra(CPlayer SourcePlayer, CVehicle SourceVehicle, int extraID, bool enabled)
	{
		if (SourceVehicle != null)
		{
			SourceVehicle.SetExtra(extraID, enabled, true, true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Set extra {0} to {1} from vehicle with ID {2}.", extraID, enabled ? "Enabled" : "Disabled", SourceVehicle.m_DatabaseID);
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You must be in a vehicle to execute this command.");
		}
	}


	private void SaveAllVehicles(object[] a_Parameters = null)
	{
		foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

			if (pVehicle != null)
			{
				pVehicle.Save();
			}
		}
	}

	public void API_onPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seat)
	{
		WeakReference<CPlayer> pPlayerRef = PlayerPool.GetPlayerFromClient(player);
		CPlayer pPlayer = pPlayerRef.Instance();

		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

		if (pPlayer != null && pVehicle != null)
		{
			pVehicle.OnPlayerEnter(pPlayer, seat);

			// entry checks
			bool bCanEnterVehicle = true;
			string strReason = "";

			// Let admins enter anything
			if (!pPlayer.IsAdmin(EAdminLevel.TrialAdmin, true))
			{
				// Can the player DRIVE this vehicle?
				if (seat < 0)
				{
					if (pVehicle.VehicleType == EVehicleType.FactionOwned || pVehicle.VehicleType == EVehicleType.FactionOwnedRental) // Faction vehicles belonging to official factions, must be entered by a faction member only
					{
						CFaction factionInst = FactionPool.GetFactionFromID(pVehicle.OwnerID);
						if (factionInst != null)
						{
							EFactionType factionType = factionInst.Type;

							// When it's a user created criminal so we can't have random people driving faction cars. This also somewhat prevents criminals to just put their cars to their factions without consequences
							if (factionType != EFactionType.UserCreatedCriminal)
							{
								if (!pPlayer.IsInFaction(pVehicle.OwnerID))
								{
									strReason = $"Must be in faction: {factionInst.Name}";
									bCanEnterVehicle = false;
								}
							}
						}
					}
					else if (pVehicle.VehicleType == EVehicleType.TruckerJob)
					{
						strReason = $"Must be a Trucker";
						bCanEnterVehicle = (pPlayer.Job == EJobID.TruckerJob);
					}
					else if (pVehicle.VehicleType == EVehicleType.DeliveryDriverJob)
					{
						strReason = $"Must be a Delivery Driver";
						bCanEnterVehicle = (pPlayer.Job == EJobID.DeliveryDriverJob);
					}
					else if (pVehicle.VehicleType == EVehicleType.BusDriverJob)
					{
						strReason = $"Must be a Bus Driver";
						bCanEnterVehicle = (pPlayer.Job == EJobID.BusDriverJob);
					}
					else if (pVehicle.VehicleType == EVehicleType.MailmanJob)
					{
						strReason = $"Must be a Mailman";
						bCanEnterVehicle = (pPlayer.Job == EJobID.MailmanJob);
					}
					else if (pVehicle.VehicleType == EVehicleType.TrashmanJob)
					{
						strReason = $"Must be a Trash Collector";
						bCanEnterVehicle = (pPlayer.Job == EJobID.TrashmanJob);
					}
					else if (pVehicle.VehicleType == EVehicleType.TaxiJob)
					{
						strReason = $"Must be a Taxi Driver";
						bCanEnterVehicle = (pPlayer.Job == EJobID.TaxiDriverJob);
					}
					else if (pVehicle.VehicleType == EVehicleType.DrivingTest_Bike)
					{
						strReason = $"Must be taking the Bike Driving Test";
						bCanEnterVehicle = pPlayer.CurrentDrivingTestType == EDrivingTestType.Bike;
					}
					else if (pVehicle.VehicleType == EVehicleType.DrivingTest_Car)
					{
						strReason = $"Must be taking the Car Driving Test";
						bCanEnterVehicle = pPlayer.CurrentDrivingTestType == EDrivingTestType.Car;
					}
					else if (pVehicle.VehicleType == EVehicleType.DrivingTest_Truck)
					{
						strReason = $"Must be taking the Truck Driving Test";
						bCanEnterVehicle = pPlayer.CurrentDrivingTestType == EDrivingTestType.Truck;
					}
				}
			}

			if (!bCanEnterVehicle)
			{
				pPlayer.SendNotification("Information", ENotificationIcon.ExclamationSign, "You cannot use this vehicle. ({0})", strReason);

				// TODO_RAGE_UPDATE:
#if RAGE_FIXED_THIS
				pPlayer.RemoveFromVehicle();
#else
				pPlayer.SetPositionSafe(pVehicle.GTAInstance.Position.Around(3.0f));
#endif
			}
			else
			{
				pPlayer.IsInVehicleReal = true;
				pPlayer.IsEnteringVehicle = false;
				NetworkEventSender.SendNetworkEvent_EnterVehicleReal(pPlayer, vehicle, seat);

				if (seat >= 4)
				{
					pPlayer.AwardAchievement(EAchievementID.RiskyMove);
				}
			}
		}
	}

	public void API_onPlayerExitVehicle(Player player, Vehicle vehicle)
	{
		WeakReference<CPlayer> pPlayerRef = PlayerPool.GetPlayerFromClient(player);
		CPlayer pPlayer = pPlayerRef.Instance();

		CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

		if (pVehicle != null && pPlayer != null)
		{
			if (pVehicle.VehicleType == EVehicleType.PlayerOwned || pVehicle.VehicleType == EVehicleType.RentalCar)
			{
				pVehicle.ManualPark(pPlayer, false);
			}

			if (pPlayer != null)
			{
				pVehicle.OnPlayerExit(pPlayer);
			}
			else
			{
				pVehicle.Save();
			}

			if (pPlayer.IsBuckledUp)
			{
				// We don't trigger the seatbelt event here because it triggers too late and players will be outside when the audio plays.
				HelperFunctions.Chat.SendAmeMessage(pPlayer, "unbuckles their seatbelt.");
				pPlayer.SendNotification("Seatbelt", ENotificationIcon.ExclamationSign, "You unbuckled your seatbelt.");
			}
			pPlayer.IsBuckledUp = false;

			pPlayer.IsInVehicleReal = false;
			pPlayer.IsEnteringVehicle = false;
			NetworkEventSender.SendNetworkEvent_ExitVehicleReal(pPlayer, vehicle);
		}
	}

	public void OnPlayerToggleEngine(CPlayer player)
	{
		// TODO_SECURITY: Check player can perform action (owns key, etc for example)
		if (player.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
			if (pVehicle != null)
			{
				// We need to be in the drivers seat
				if (player.Client.VehicleSeat == (int)EVehicleSeat.Driver)
				{
					// We must have the keys OR be in the faction/job

					bool bHasJobForVehicle = pVehicle.IsVehicleForJob(player.Job);
					bool bHasFactionForVehicle = pVehicle.IsVehicleForAnyPlayerFaction(player);
					bool bDoingDrivingTestAndDMVVehicle = pVehicle.IsVehicleForDrivingTest(player);
					bool bIsCivVehicle = pVehicle.VehicleType == EVehicleType.Civilian;
					bool bIsOnDutyadmin = player.IsAdmin(EAdminLevel.TrialAdmin, true);

					if (bHasJobForVehicle || bHasFactionForVehicle || bDoingDrivingTestAndDMVVehicle || bIsOnDutyadmin || bIsCivVehicle || pVehicle.HasKeys(player))
					{
						bool bNewEngineState = !pVehicle.EngineOn;

						if (bNewEngineState)
						{
							// We are trying to start the engine so we should check for fuel and the health
							if (player.Client.Vehicle.Health < 100f)
							{
								// Less than 10% health remains, maybe a mechanic can patch it up a little bit
								// to get it running again at this stage?
								HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString($"turn the key but the engine fails to start."));
								return;
							}
							else if (pVehicle.Fuel == 0f)
							{
								// No gas
								HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString($"turns the key but the engine only sputters."));
								return;
							}
						}

						if (!bNewEngineState) // engine was turned off
						{
							pVehicle.Save();
						}

						pVehicle.EngineOn = bNewEngineState;
						string newEngineState = pVehicle.EngineOn ? "on" : "off";
						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("turns the engine {0}.", newEngineState));
						Logging.Log.CreateLog(player.ActiveCharacterDatabaseID, Logging.EOriginType.Character, Logging.ELogType.VehicleRelated, new List<CBaseEntity>() { pVehicle }, $"ENGINE {newEngineState.ToUpper()}.");
					}
					else
					{
						player.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "You cannot turn the engine {0} without having the keys.", !pVehicle.EngineOn ? "on" : "off");
					}
				}
				else
				{
					player.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "You cannot turn the engine {0} from this seat.", !pVehicle.EngineOn ? "on" : "off");
				}
			}
		}
	}

	public void OnPlayerEnterVehicleAttempt(Player sender, Vehicle vehicle, sbyte seatID)
	{
		WeakReference<CPlayer> PlayerRef = PlayerPool.GetPlayerFromClient(sender);
		CPlayer player = PlayerRef.Instance();

		if (player != null)
		{
			player.StartVehicleEnterTimer();
			player.IsInVehicleReal = false;
			player.IsEnteringVehicle = true;
		}
	}

	public void OnPlayerToggleVehicleLocked(CPlayer player)
	{
		if (!player.Client.Dead)
		{
			if (player.IsInVehicleReal && player.Client.Vehicle != null) // Anyone inside the vehicle, who is in the drivers seat can lock (even without key etc)
			{
				// Anyone inside the vehicle can unlock (unless its a police vehicle)
				// Anyone in the drivers or front passengers seat can lock/unlock
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
				if (pVehicle != null)
				{
					bool bIsLocked = player.Client.Vehicle.Locked;
					bool bNewState = !bIsLocked;
					VehicleSeat seat = (VehicleSeat)player.Client.VehicleSeat;
					bool bCanPerformAction = false;

					if (bNewState) // lock
					{
						if (seat == VehicleSeat.Driver || seat == VehicleSeat.RightFront) // driver and front passenger
						{
							bCanPerformAction = true;
						}
					}
					else // unlock
					{
						// is it a police vehicle?
						if (pVehicle.IsPoliceCar())
						{
							if (seat == VehicleSeat.Driver || seat == VehicleSeat.RightFront) // driver and front passenger
							{
								bCanPerformAction = true;
							}
						}
						else
						{
							bCanPerformAction = true;
						}
					}

					if (bCanPerformAction)
					{
						pVehicle.ToggleLocked();

						// TODO: Fix colors, doesn't seem to be supported in dotnetcore pVehicle.GetColorsDisplayString()
						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("{0} the vehicle ({1}).", player.Client.Vehicle.Locked ? "locked" : "unlocked", pVehicle.GetFullDisplayName()));
					}
					else
					{
						player.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "You cannot {0} the vehicle from this seat.", !player.Client.Vehicle.Locked ? "lock" : "unlock");
					}
				}
			}
			else // Nearby vehicle OR interior unlock, check for key
			{
				// TODO_POST_LAUNCH: Move this, its not specific to vehicles
				// Don't let them unlock whilst entering, this causes weird GTA bugs where they try to forcefully enter the vehicle.
				if (!player.IsEnteringVehicle)
				{
					// TODO: More generic location
					const float fUnlockDist = 10.0f;

					uint playerDimension = player.Client.Dimension;
					float fSmallestDistanceVehicle = 999999.0f;
					float fSmallestDistanceProperty = 999999.0f;
					CVehicle pNearestEligibleVehicle = null;
					CPropertyInstance pNearestEligibleProperty = null;

					// Find the nearest interior which we CAN unlock
					List<CPropertyInstance> lstProperties = PropertyPool.GetAllPropertyInstances();
					foreach (CPropertyInstance propInst in lstProperties)
					{
						float fDistEntrance = (player.Client.Position - propInst.Model.EntrancePosition).Length();
						float fDistExit = (player.Client.Position - propInst.Model.ExitPosition).Length();

						if ((fDistEntrance < fUnlockDist && fDistEntrance <= fSmallestDistanceProperty && propInst.Model.EntranceDimension == playerDimension) || (fDistExit < fUnlockDist && fDistExit <= fSmallestDistanceProperty && propInst.Model.Id == playerDimension))
						{
							bool bHasFactionForProperty = propInst.IsPropertyForAnyPlayerFaction(player);
							if (bHasFactionForProperty || propInst.HasKeys(player) || player.IsAdmin(EAdminLevel.TrialAdmin, true))
							{
								// Can't lock/unlock world interiors
								if (propInst.Model.EntranceType != EPropertyEntranceType.World)
								{
									fSmallestDistanceProperty = Math.Min(fDistEntrance, fDistExit);
									pNearestEligibleProperty = propInst;
								}
							}
						}
					}

					// Find the nearest vehicle which we CAN unlock
					foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
					{
						CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

						if (pVehicle != null)
						{
							float fDist = (player.Client.Position - pVehicle.GTAInstance.Position).Length();

							if (fDist < fUnlockDist && fDist <= fSmallestDistanceVehicle && pVehicle.GTAInstance.Dimension == playerDimension)
							{
								bool bHasJobForVehicle = pVehicle.IsVehicleForJob(player.Job);
								bool bHasFactionForVehicle = pVehicle.IsVehicleForAnyPlayerFaction(player);
								bool bHasAdminAndIsTempVehicle = pVehicle.VehicleType == EVehicleType.Temporary && player.IsAdmin(EAdminLevel.TrialAdmin, true);

								if (bHasJobForVehicle || bHasFactionForVehicle || bHasAdminAndIsTempVehicle || pVehicle.HasKeys(player) || player.IsAdmin(EAdminLevel.TrialAdmin, true))
								{
									fSmallestDistanceVehicle = fDist;
									pNearestEligibleVehicle = pVehicle;
								}
							}
						}
					}

					if (fSmallestDistanceVehicle <= fSmallestDistanceProperty && pNearestEligibleVehicle != null)
					{
						pNearestEligibleVehicle.ToggleLocked();

						CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(pNearestEligibleVehicle.GTAInstance.Model);
						string strVehicleName = Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name);

						// TODO: Fix colors, doesn't seem to be supported in dotnetcore pVehicle.GetColorsDisplayString()
						HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("{0} the vehicle ({1}).", pNearestEligibleVehicle.GTAInstance.Locked ? "locked" : "unlocked", strVehicleName));

						pNearestEligibleVehicle.Save();
					}
					else if (fSmallestDistanceProperty <= fSmallestDistanceVehicle && pNearestEligibleProperty != null)
					{
						if (!pNearestEligibleProperty.Model.IsAlwaysEnterable())
						{
							pNearestEligibleProperty.Model.SetLocked(!pNearestEligibleProperty.Model.Locked);
							HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("{0} the property ({1}).", pNearestEligibleProperty.Model.Locked ? "locked" : "unlocked", pNearestEligibleProperty.Model.Name));
						}
						else
						{
							player.SendNotification("Lock / Unlock", ENotificationIcon.ExclamationSign, "This property is not lockable.", null);
						}
					}
					else
					{
						player.SendNotification("Lock / Unlock", ENotificationIcon.ExclamationSign, "No vehicle / property is nearby which you have access to.", null);
					}
				}
			}
		}
	}

	public void SetSpotlightRotation(CPlayer player, float fRot)
	{
		if (player.IsInVehicleReal)
		{
			if (player.Client.VehicleSeat == (int)EVehicleSeat.Driver) // Drivers seat
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
				if (pVehicle != null)
				{
					pVehicle.SetSpotlightRotation(fRot);
				}
			}
		}
	}

	public void ToggleSpotlight(CPlayer player)
	{
		if (player.IsInVehicleReal)
		{
			if (player.Client.VehicleSeat == (int)EVehicleSeat.Driver) // Drivers seat
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
				if (pVehicle != null)
				{
					pVehicle.ToggleSpotlight();
				}
			}
		}
	}

	public void OnPlayerToggleHeadlights(CPlayer player)
	{
		if (player.IsInVehicleReal)
		{
			if (player.Client.VehicleSeat == (int)EVehicleSeat.Driver) // Drivers seat
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
				if (pVehicle != null)
				{
					string strState = "";
					if (pVehicle.Headlights == EHeadlightState.Off)
					{
						strState = "on";
						pVehicle.Headlights = EHeadlightState.On;
					}
					else if (pVehicle.Headlights == EHeadlightState.On)
					{
						strState = "on to full beam";
						pVehicle.Headlights = EHeadlightState.On_FullBeam;
					}
					else if (pVehicle.Headlights == EHeadlightState.On_FullBeam)
					{
						strState = "off";
						pVehicle.Headlights = EHeadlightState.Off;
					}

					HelperFunctions.Chat.SendAmeMessage(player, Helpers.FormatString("turns the headlights {0}.", strState));
				}
			}
		}
	}

	public void VehicleDoorCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, int doorID)
	{
		if (doorID >= 0 && doorID <= 5)
		{
			if (SourceVehicle.GTAInstance.Locked)
			{
				SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "The vehicle doors are locked.", null);
			}
			else
			{
				SourceVehicle.ToggleDoorState(SourcePlayer, doorID);
			}
		}
		else
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "That door ID is invalid. Door ID's range from 0 to 5.", null);
		}
	}

	private void ParkCommand(CPlayer player, CVehicle pVehicle)
	{
		if (pVehicle != null && player != null)
		{
			if (pVehicle.VehicleType == EVehicleType.PlayerOwned
			|| pVehicle.VehicleType == EVehicleType.FactionOwned
			|| pVehicle.VehicleType == EVehicleType.RentalCar
			|| pVehicle.VehicleType == EVehicleType.FactionOwnedRental)
			{
				pVehicle.ManualPark(player);
				new Logging.Log(player, Logging.ELogType.VehicleRelated, null, Helpers.FormatString("/park - Vehicle: {0}", pVehicle.m_DatabaseID)).execute();
			}
			else
			{
				player.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "/park cannot be used on this vehicle type.", null);
			}
		}
	}

	private void AdminParkCommand(CPlayer player, CVehicle pVehicle)
	{
		pVehicle.AdminPark(player);
		new Logging.Log(player, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/apark - Vehicle: {0}", pVehicle.m_DatabaseID)).execute();
	}

	public void FixCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer)
	{
		if (!TargetPlayer.Client.IsInVehicle)
		{
			SourcePlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "{0} is not in a vehicle.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
			return;
		}

		TargetPlayer.Client.Vehicle.Repair();

		SourcePlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You have repaired the vehicle of {0}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));

		if (SourcePlayer != TargetPlayer)
		{
			TargetPlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "{0} has repaired your vehicle.", SourcePlayer.GetCharacterName(ENameType.StaticCharacterName));
		}

		new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, new List<CBaseEntity> { TargetPlayer }, Helpers.FormatString("/fixveh")).execute();
	}

	public void FixAllCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
	{
		VehiclePool.FixAllVehicles();

		HelperFunctions.Chat.SendAdminAnnouncement(SourcePlayer, " Repaired all the vehicles.");
		new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/fixvehs")).execute();
	}

	public void GetPaletoBayVehicleCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
	{
		CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
		if (Vehicle != null)
		{
			if (Vehicle.IsTowed())
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "That vehicle is in the impound.");
			}
			else
			{
				if (Vehicle.OwnedOrRentedBy(SourcePlayer))
				{
					bool bIsPaleto = Vehicle.GTAInstance.Position.Y >= Constants.BorderOfLStoPaleto;

					if (bIsPaleto)
					{
						Vector3 vecOffsetPos = SourcePlayer.GetOffsetPosInFront(4.0f);
						vecOffsetPos.Z += 1.0f;
						Vehicle.TeleportVehicleOnly(vecOffsetPos);
						Vehicle.Save();
						SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 0, 255, 0, "You have teleported a {0} to you from Paleto Bay.", Vehicle.GetFullDisplayName());

						new Logging.Log(SourcePlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/getpaletoveh - Vehicle ID: {0}", vehicleID)).execute();
					}
					else
					{
						SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "The vehicle is not in Paleto Bay.");
					}
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "You must be the owner or renter of the vehicle.");
				}
			}
		}
		else
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "Vehicle not found.");
		}
	}

	public void OnToggleSeatbelt(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderVehicle == null)
		{
			SenderPlayer.SendNotification("Seatbelt", ENotificationIcon.ExclamationSign, "You must be in a vehicle to buckle/unbuckle your seatbelt.");
			return;
		}

		if (!SenderVehicle.HasSeatbelts())
		{
			SenderPlayer.SendNotification("Seatbelt", ENotificationIcon.ExclamationSign, "This vehicle does not have seatbelts!");
			return;
		}

		if (!SenderPlayer.IsBuckledUp)
		{
			SenderPlayer.IsBuckledUp = true;
			NetworkEventSender.SendNetworkEvent_ToggleSeatbelt(SenderPlayer);
			HelperFunctions.Chat.SendAmeMessage(SenderPlayer, "buckles their seatbelt.");
			SenderPlayer.SendNotification("Seatbelt", ENotificationIcon.ExclamationSign, "You buckled your seatbelt.");
		}
		else
		{
			SenderPlayer.IsBuckledUp = false;
			NetworkEventSender.SendNetworkEvent_ToggleSeatbelt(SenderPlayer);
			HelperFunctions.Chat.SendAmeMessage(SenderPlayer, "unbuckles their seatbelt.");
			SenderPlayer.SendNotification("Seatbelt", ENotificationIcon.ExclamationSign, "You unbuckled your seatbelt.");
		}
	}

	public void OnToggleWindows(CPlayer SenderPlayer, CVehicle SenderVehicle)
	{
		if (SenderVehicle != null && SenderVehicle.IsAircraft())
		{
			return;
		}

		if (SenderVehicle != null && SenderVehicle.HasWindows())
		{
			SenderVehicle.ToggleWindowState(SenderPlayer);
		}
	}

	public void OnPlayerToggleWindows(CPlayer SenderPlayer)
	{
		if (SenderPlayer.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(SenderPlayer.Client.Vehicle);

			if (pVehicle != null && pVehicle.IsAircraft())
			{
				return;
			}

			if (pVehicle != null && pVehicle.HasWindows())
			{
				pVehicle.ToggleWindowState(SenderPlayer);
			}
		}
	}

	public void OnRadialSetDoorState(CPlayer SenderPlayer, int vehicleID, int door)
	{
		CVehicle pVehicle = VehiclePool.GetVehicleFromID(vehicleID);

		if (pVehicle != null)
		{
			bool bHasFactionForVehicle = pVehicle.IsVehicleForAnyPlayerFaction(SenderPlayer);
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, pVehicle.m_DatabaseID);
			bool bHasKey = SenderPlayer.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
			if (bHasFactionForVehicle || bHasKey || !pVehicle.GTAInstance.Locked)
			{
				pVehicle.ToggleDoorState(SenderPlayer, door);
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You require a vehicle key in order to access this vehicle.");
			}
		}
	}

	public void OnRadialSetLockState(CPlayer SenderPlayer, int vehicleID)
	{
		CVehicle pVehicle = VehiclePool.GetVehicleFromID(vehicleID);

		if (pVehicle != null)
		{
			bool bHasFactionForVehicle = pVehicle.IsVehicleForAnyPlayerFaction(SenderPlayer);
			CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, pVehicle.m_DatabaseID);
			bool bHasKey = SenderPlayer.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
			if (bHasFactionForVehicle || bHasKey)
			{
				pVehicle.ToggleLocked();
				HelperFunctions.Chat.SendAmeMessage(SenderPlayer, Helpers.FormatString("{0} the vehicle ({1}).", pVehicle.GTAInstance.Locked ? "locked" : "unlocked", pVehicle.GetFullDisplayName()));
			}
			else
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "You require a vehicle key in order to access this vehicle.");
			}
		}
	}

	private void OnSetVehicleGear(CPlayer SenderPlayer, int gear)
	{
		CVehicle vehicle = SenderPlayer.GetPlayerVehicleIsIn();

		if (vehicle != null)
		{
			vehicle.SetData(vehicle.GTAInstance, EDataNames.MANUAL_VEHICLE_GEAR, gear, EDataType.Synced);
		}
	}

	private void OnSyncManualVehBrakes(CPlayer SenderPlayer, bool bIsOn)
	{
		CVehicle vehicle = SenderPlayer.GetPlayerVehicleIsIn();

		if (vehicle != null)
		{
			vehicle.SetData(vehicle.GTAInstance, EDataNames.MANUAL_VEHICLE_BRAKELIGHTS, bIsOn, EDataType.Synced);
		}
	}

	private void OnSyncManualVehRpm(CPlayer SenderPlayer, float rpm)
	{
		CVehicle vehicle = SenderPlayer.GetPlayerVehicleIsIn();

		if (vehicle != null)
		{
			vehicle.SetData(vehicle.GTAInstance, EDataNames.MANUAL_VEHICLE_RPM, rpm, EDataType.Synced);
		}
	}

	private void OnSyncVehicleHandbrake(CPlayer SenderPlayer)
	{
		CVehicle vehicle = SenderPlayer.GetPlayerVehicleIsIn();

		if (vehicle == null || !vehicle.HasHandbrake())
		{
			return;
		}

		bool bHandbrakeOn = vehicle.GetData<bool>(vehicle.GTAInstance, EDataNames.VEHICLE_HANDBRAKE);
		vehicle.SetData(vehicle.GTAInstance, EDataNames.VEHICLE_HANDBRAKE, !bHandbrakeOn, EDataType.Synced);
		Logging.Log.CreateLog(SenderPlayer.ActiveCharacterDatabaseID, Logging.EOriginType.Character, Logging.ELogType.VehicleRelated, new List<CBaseEntity>() { vehicle }, Helpers.FormatString("HANDBRAKE {0}", bHandbrakeOn ? "ON" : "OFF"));

		if (!bHandbrakeOn)
		{
			HelperFunctions.Chat.SendAmeMessage(SenderPlayer, "pulls up the vehicle handbrake.");
		}
		else
		{
			HelperFunctions.Chat.SendAmeMessage(SenderPlayer, "releases the vehicle handbrake.");
		}

		foreach (CPlayer occupantPlayer in VehiclePool.GetVehicleOccupants(vehicle))
		{
			if (occupantPlayer != null)
			{
				NetworkEventSender.SendNetworkEvent_SyncVehicleHandbrakeSound(occupantPlayer);
			}
		}
	}

	private void OnToggleEngineStall(CPlayer SenderPlayer)
	{
		if (SenderPlayer.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(SenderPlayer.Client.Vehicle);
			if (pVehicle != null)
			{
				pVehicle.EngineOn = false;
			}
		}
	}
}