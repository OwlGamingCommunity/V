using GTANetworkAPI;
using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

namespace PlayerAdminCommands
{
	public class Vehicles
	{
		public const uint NEARBY_VEHICLES_DISTANCE = 20;

		public Vehicles()
		{
			// COMMANDS
			CommandManager.RegisterCommand("sendcar", "Teleport a vehicle to a player.", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID>(SendVehicle), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "sendveh", "sendvehicle" });
			CommandManager.RegisterCommand("getcar", "Teleport a vehicle to you.", new Action<CPlayer, CVehicle, EntityDatabaseID>(GetVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "getveh" });
			CommandManager.RegisterCommand("gotocar", "Teleport to a vehicle.", new Action<CPlayer, CVehicle, EntityDatabaseID>(GotoVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "gotoveh" });
			CommandManager.RegisterCommand("unflip", "Unflips a vehicle.", new Action<CPlayer, CVehicle, EntityDatabaseID>(UnflipVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "flipveh" });
			CommandManager.RegisterCommand("respawnall", "Respawn all vehicles.", new Action<CPlayer, CVehicle, bool>(RespawnAllVehicles), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("respawnalljobvehs", "Respawn all job vehicles.", new Action<CPlayer, CVehicle, bool>(RespawnAllJobVehicles), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("respawnalldmvvehs", "Respawn all DMV vehicles.", new Action<CPlayer, CVehicle, bool>(RespawnAllDMVVehicles), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("respawnallfactionvehs", "Respawn all  faction vehicles.", new Action<CPlayer, CVehicle, bool>(RespawnAllFactionVehicles), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("reloadveh", "Reload a vehicle.", new Action<CPlayer, CVehicle, EntityDatabaseID>(ReloadVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "reloadcar" });
			CommandManager.RegisterCommand("respawnveh", "Respawn a vehicle.", new Action<CPlayer, CVehicle, EntityDatabaseID>(RespawnVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "reloadcar" });
			CommandManager.RegisterCommand("admintow", "Tows all vehicles blocking roadways.", new Action<CPlayer, CVehicle, EntityDatabaseID>(AdminTow), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("admintowall", "Tows all vehicles blocking roadways.", new Action<CPlayer, CVehicle>(AdminTowAll), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setvehowner", "Reload a vehicle.", new Action<CPlayer, CVehicle, EntityDatabaseID, EVehicleType, string>(SetOwner), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setcarowner" });
			CommandManager.RegisterCommand("veh", "Create a temporary vehicle.", new Action<CPlayer, CVehicle, string>(TemporaryVehicle), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "car" });
			CommandManager.RegisterCommand("makeveh", "Create a permanent vehicle.", new Action<CPlayer, CVehicle, int, string, string, int, int, int, int, int, int, EVehicleTransmissionType, EPlateType, int>(MakeVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "makecar" });

			CommandManager.RegisterCommand("maketypedveh", "Create a typed vehicle.", new Action<CPlayer, CVehicle, int, EVehicleType, string, int, int, int, int, int, int, EVehicleTransmissionType>(MakeTypedVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("makecivveh", "Create a manual vehicle.", new Action<CPlayer, CVehicle, int, EVehicleTransmissionType>(MakeCivVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("delveh", "Deletes a vehicle.", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "delcar" });
			CommandManager.RegisterCommand("delthisveh", "Deletes your current vehicle.", new Action<CPlayer, CVehicle>(DeleteThisVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "delthiscar" });
			CommandManager.RegisterCommand("nearbyvehicles", "Shows all nearby vehicles.", new Action<CPlayer, CVehicle>(NearbyVehiclesCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "nearbyveh", "nearbyvehs" });
			CommandManager.RegisterCommand("thisveh", "Shows current vehicles information.", new Action<CPlayer, CVehicle>(ThisVehicleCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeInVehicle, aliases: new string[] { "thiscar" });
			CommandManager.RegisterCommand("oldveh", "Shows previous vehicles information.", new Action<CPlayer, CVehicle>(OldVehicleCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "oldcar" });
			CommandManager.RegisterCommand("setplayerinveh", "Sets a player into a vehicle.", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID, int>(SetPlayerInVehicleCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setplayerincar" });
			CommandManager.RegisterCommand("setdirt", "Sets target players vehicles dirt level.", new Action<CPlayer, CVehicle, CPlayer, float>(SetDirtCommand), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("makevehkey", "Gives a vehicle key to the target player.", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID>(MakeVehicleKey), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("changevehlock", "Changes a vehicle lock and gives it to the target player.", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID>(ChangeVehicleLock), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("setvehplate", "Sets the plate of a vehicle.", new Action<CPlayer, CVehicle, string>(SetPlateForVehicle), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setplatetype", "Sets the plate type of a vehicle.", new Action<CPlayer, CVehicle, EPlateType>(SetPlateType), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("togplate", "Toggles the vehicle's plates.", new Action<CPlayer, CVehicle, EntityDatabaseID>(TogVehPlate), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("changevehmodel", "Sets the model of a vehicle.", new Action<CPlayer, CVehicle, int>(ChangeVehicleModel), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setvehcolor", "Sets the color of a vehicle.", new Action<CPlayer, CVehicle, int, int, int, int, int, int>(SetVehColor), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setvehtransmission", "Sets the transmission of a vehicle.", new Action<CPlayer, CVehicle, EVehicleTransmissionType>(SetVehTransmission), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setspecialcolor", "Sets the special(pearlescent) color of a vehicle.", new Action<CPlayer, CVehicle, string, int>(SetSpecialColor), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("removeallvehicledutyitems", "Removes all duty items from all PD & FD vehicles.", new Action<CPlayer, CVehicle>(RemoveAllVehicleDutyItems), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);
			CommandManager.RegisterCommand("superman", "Shows a player inventory", new Action<CPlayer, CVehicle>(SupermanCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);

			// TODO_TOWING: Move to better location when we add job/faction
			NetworkEvents.AdminTowGotVehicles += OnAdminTowGotVehicles;
			NetworkEvents.TowedVehicleList_Request += OnRequestTowedVehicleList;
			NetworkEvents.RequestUnimpoundVehicle += OnRequestUnimpoundVehicle;

			NetworkEvents.AdminDeleteVehicle += OnConfirmVehicleDelete;

			RageEvents.RAGE_OnPlayerExitVehicle += API_onPlayerExitVehicle;
		}

		private void OnRequestUnimpoundVehicle(CPlayer SourcePlayer, Vehicle vehicle, EScriptLocation a_Location)
		{
			CVehicle vehicleInst = VehiclePool.GetVehicleFromGTAInstance(vehicle);

			if (vehicleInst != null)
			{
				if (vehicleInst.OwnedOrRentedBy(SourcePlayer))
				{
					if (SourcePlayer.SubtractBankBalanceIfCanAfford(Constants.CostToUnimpoundCar, PlayerMoneyModificationReason.UnimpoundCar))
					{
						vehicleInst.Unimpound(a_Location);
						SourcePlayer.SendNotification("Vehicle Impound", ENotificationIcon.InfoSign, "Your {0} was unimpounded for ${1:0.00}. Please park responsibly next time.", vehicleInst.GetFullDisplayName(), Constants.CostToUnimpoundCar);
					}
					else
					{
						SourcePlayer.SendNotification("Vehicle Impound", ENotificationIcon.PiggyBank, "You cannot afford the ${0:0.00} fee to unimpound this vehicle.", Constants.CostToUnimpoundCar);
					}
				}
			}

		}

		private async void OnAdminTowGotVehicles(CPlayer SourcePlayer, List<Int64> lstEligibleVehicles)
		{
			int numTowed = 0;
			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

				if (pVehicle != null)
				{
					if (pVehicle.IsWithinVehicleStoreNoParkZone() || lstEligibleVehicles.Contains(Convert.ToInt32(pVehicle.m_DatabaseID)))
					{
						await pVehicle.ScriptTow().ConfigureAwait(true);
						++numTowed;
					}
				}
			}

			SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have towed {0} vehicles!", numTowed);
		}

		private void OnRequestTowedVehicleList(CPlayer SourcePlayer)
		{
			List<Int64> lstTowedVehicles = new List<Int64>();
			foreach (CVehicle vehicle in VehiclePool.GetVehiclesFromPlayerOwner(SourcePlayer.ActiveCharacterDatabaseID))
			{
				if (vehicle.IsTowed())
				{
					// TODO_64BIT: Entity datas are 32bit, but this is 64bit... we need to fix this some day... maybe... if we ever get that many vehicles
					lstTowedVehicles.Add(Convert.ToInt32(vehicle.m_DatabaseID));
				}
			}

			NetworkEventSender.SendNetworkEvent_TowedVehicleList_Response(SourcePlayer, lstTowedVehicles);
		}

		private async void ChangeVehicleLock(CPlayer SourcePlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EntityDatabaseID VehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(VehicleID);
			if (Vehicle != null)
			{
				// Delete old keys
				CItemInstanceDef oldKeyDefinition = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, VehicleID);
				await HelperFunctions.Items.DeleteAllItems(oldKeyDefinition).ConfigureAwait(true);

				// New key
				CItemInstanceDef newKeyDefinition = CItemInstanceDef.FromDefaultValueWithStackSize(EItemID.VEHICLE_KEY, VehicleID, 1);

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;

				if (TargetPlayer.Inventory.CanGiveItem(newKeyDefinition, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(newKeyDefinition, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received a vehicle key for vehicle '{1}' ({2})", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID);
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you a vehicle key for vehicle '{2}' ({3})", SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID);
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive vehicle key '{1}' ({2}): Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID);
						}
					});
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive vehicle key '{1}' ({2}): {3}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID, strHumanReadableError);
				}
			}
		}

		private void MakeVehicleKey(CPlayer SourcePlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EntityDatabaseID VehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(VehicleID);
			if (Vehicle != null)
			{
				CItemInstanceDef itemDef = CItemInstanceDef.FromDefaultValueWithStackSize(EItemID.VEHICLE_KEY, VehicleID, 1);

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;

				if (TargetPlayer.Inventory.CanGiveItem(itemDef, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received a vehicle key for vehicle '{1}' ({2})", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID);
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you a vehicle key for vehicle '{2}' ({3})", SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID);
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive vehicle key '{1}' ({2}): Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID);
						}
					});
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive vehicle key '{1}' ({2}): {3}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Vehicle.GetFullDisplayName(), VehicleID, strHumanReadableError);
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, " No vehicle was found with id {0}", VehicleID);
			}
		}

		private void GetVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				if (Vehicle.IsTowed())
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "That vehicle is in the impound.");
				}
				else
				{
					Vector3 vecOffsetPos = SourcePlayer.GetOffsetPosInFront(4.0f);
					vecOffsetPos.Z += 1.0f;
					Vehicle.TeleportVehicleOnly(vecOffsetPos);
					Vehicle.GTAInstance.Dimension = SourcePlayer.Client.Dimension;
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have teleported a {0} to you.", Vehicle.GetFullDisplayName());
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
			}
		}

		private void GotoVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				if (Vehicle.IsTowed())
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "That vehicle is in the impound.");
				}
				else
				{
					SourcePlayer.SetPositionSafe(Vehicle.GetOffsetPosInFront(4.0f));
					SourcePlayer.SetSafeDimension(Vehicle.GTAInstance.Dimension);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have teleported to a {0}.", Vehicle.GetFullDisplayName());
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
			}
		}

		private void SendVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer,
			EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
				return;
			}

			if (Vehicle.IsTowed())
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "That vehicle is in the impound.");
				return;
			}

			Vector3 vecOffsetPos = TargetPlayer.GetOffsetPosInFront(4.0f);
			vecOffsetPos.Z += 1.0f;
			Vehicle.TeleportVehicleOnly(vecOffsetPos);
			Vehicle.GTAInstance.Dimension = TargetPlayer.Client.Dimension;
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have teleported a {0} to {1}.", Vehicle.GetFullDisplayName(), TargetPlayer.GetCharacterName(ENameType.CharacterDisplayName));
			TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "{0} {1} teleported a {2} to you.", SourcePlayer.AdminTitle, SourcePlayer.Username, Vehicle.GetFullDisplayName());
		}

		private void UnflipVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				Vehicle.GTAInstance.Rotation = new Vector3(0.0f, 0.0f, Vehicle.GTAInstance.Rotation.Z);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have unflipped vehicle {0}.", vehicleID);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
			}
		}

		private void RespawnAllVehicles(CPlayer SourcePlayer, CVehicle SourceVehicle, bool Force)
		{
			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

				pVehicle?.Respawn(Force);
			}

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have respawned all vehicles.");
		}

		private void RespawnAllJobVehicles(CPlayer SourcePlayer, CVehicle SourceVehicle, bool Force)
		{
			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

				if (pVehicle != null && pVehicle.IsJobVehicle())
				{
					pVehicle.Respawn(Force);
				}
			}

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have respawned all job vehicles.");
		}

		private void RespawnAllDMVVehicles(CPlayer SourcePlayer, CVehicle SourceVehicle, bool Force)
		{
			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

				if (pVehicle != null && pVehicle.IsDMVVehicle())
				{
					pVehicle.Respawn(Force);
				}
			}

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have respawned all DMV vehicles.");
		}

		private void RespawnAllFactionVehicles(CPlayer SourcePlayer, CVehicle SourceVehicle, bool Force)
		{
			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

				if (pVehicle != null && (pVehicle.VehicleType == EVehicleType.FactionOwned || pVehicle.VehicleType == EVehicleType.FactionOwnedRental))
				{
					pVehicle.Respawn(Force);
				}
			}

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have respawned all job vehicles.");
		}

		private async void AdminTow(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have towed a {0}.", Vehicle.GetFullDisplayName());
				await Vehicle.ScriptTow().ConfigureAwait(true);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
			}
		}

		private void AdminTowAll(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			NetworkEventSender.SendNetworkEvent_AdminTowGetVehicles(SourcePlayer);
		}

		private void ReloadVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle != null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have reloaded a {0}.", Vehicle.GetFullDisplayName());
				VehiclePool.ReloadVehicle(Vehicle);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
			}
		}

		private void RespawnVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
				return;
			}

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have respawned a {0}.", Vehicle.GetFullDisplayName());
			Vehicle.Respawn(true);
		}

		private async void SetOwner(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID, EVehicleType newVehicleType, string NewOwnerName)
		{
			if (vehicleID != 0 && (newVehicleType == EVehicleType.PlayerOwned || newVehicleType == EVehicleType.FactionOwned))
			{
				CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
				if (Vehicle != null)
				{
					if (newVehicleType == EVehicleType.PlayerOwned)
					{
						// Then we know we want to get the player from the partial information
						WeakReference<CPlayer> newOwnerRef = CommandManager.FindTargetPlayer(SourcePlayer, NewOwnerName);
						CPlayer newOwner = newOwnerRef.Instance();

						if (newOwner != null)
						{
							CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, Vehicle.m_DatabaseID);
							if (newOwner.Inventory.CanGiveItem(ItemInstanceDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
							{
								await HelperFunctions.Items.DeleteAllItems(ItemInstanceDef).ConfigureAwait(true);

								CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, Vehicle.m_DatabaseID);
								SourcePlayer.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
								{
									if (bItemGranted)
									{
										Vehicle.SetOwner(newVehicleType, newOwner.ActiveCharacterDatabaseID);
										SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have set the owner of a {0} to {1}.", Vehicle.GetFullDisplayName(), newOwner.GetCharacterName(ENameType.StaticCharacterName));
									}
								});
							}
							else
							{
								SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "{0} could not receive the key:<br>{1}", newOwner.GetCharacterName(ENameType.StaticCharacterName), strUserFriendlyMessage);
							}
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Player not found.");
						}
					}
					else
					{
						// Otherwise we just want to use it as an integer
						EntityDatabaseID newOwner = 0;
						CFaction Faction = null;

						try
						{
							newOwner = EntityDatabaseID.Parse(NewOwnerName);
							Faction = FactionPool.GetFactionFromID(newOwner);
						}
						catch
						{
						}

						if (Faction != null)
						{
							Vehicle.SetOwner(newVehicleType, Faction.FactionID);
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have set the owner of a {0} to {1}.", Vehicle.GetFullDisplayName(), Faction.Name);
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid Faction ID.");
						}
					}
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
				}
			}
		}

		public static async void TemporaryVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, string greedyArgs)
		{
			EVehicleTransmissionType transmission = EVehicleTransmissionType.Automatic;
			int livery = 0;
			int index = 0;
			var splitGreedyArgs = greedyArgs.Split(' ');

			if (splitGreedyArgs.Length >= 1 && !int.TryParse(splitGreedyArgs[0], out index))
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0,
					"Syntax: [index] (transmission type) (livery).");
				return;
			}

			if (splitGreedyArgs.Length >= 2 && !Enum.TryParse(splitGreedyArgs[1], out transmission))
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0,
					"Invalid Transmissions Type");
				return;
			}

			if (splitGreedyArgs.Length >= 3 && !int.TryParse(splitGreedyArgs[2], out livery))
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid Livery");
				return;
			}

			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(index);
			if (vehicleDef == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid Vehicle Index.");
				return;
			}

			Vector3 vecRot = SourcePlayer.Client.Rotation;
			Vector3 vecPos = SourcePlayer.Client.Position;
			float fDist = 5.0f;
			float radians = (vecRot.Z + 90.0f) * (3.14f / 180.0f);
			vecPos.X += (float)Math.Cos(radians) * fDist;
			vecPos.Y += (float)Math.Sin(radians) * fDist;

			const UInt32 expiryTime = 0;
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			uint addonModel = 0;

			if (vehicleDef.Hash == 0)
			{
				if (!string.IsNullOrEmpty(vehicleDef.AddOnName))
				{
					addonModel = NAPI.Util.GetHashKey(vehicleDef.AddOnName);
				}
			}

			CVehicle veh = await VehiclePool.CreateVehicle(-(++totalTempVehicles), EVehicleType.Temporary, 0, vehicleDef.Hash != 0 ? (VehicleHash)vehicleDef.Hash : 0, vecPos, vecRot, vecPos, vecRot, EPlateType.Blue_White, (index % 2 == 0) ? "DA WEY" : "P3N15", 100.0f, 105, 105, 105, 105, 105,
				105, 1, livery, 0.0f, 100.0f, false, true, 0, 0, 0, 0.0f, false, false, expiryTime, 1337.0f, false, SourcePlayer.Client.Dimension, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, transmission, 0, addonModel).ConfigureAwait(true);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You spawned a {0} {1} (#{2}).", vehicleDef.Manufacturer, vehicleDef.Name, -(totalTempVehicles));
		}

		const string g_PlateText = "SUPERMAN";
		public void API_onPlayerExitVehicle(Player player, Vehicle vehicle)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

			if (pVehicle != null && pVehicle.GetPlateText(true) == g_PlateText)
			{
				VehiclePool.DestroyVehicle(pVehicle);
			}
		}

		public async void SupermanCommand(CPlayer SourcePlayer, CVehicle Vehicle)
		{
			if (Vehicle != null)
			{
				if (Vehicle.GetPlateText(true) == g_PlateText)
				{
					VehiclePool.DestroyVehicle(Vehicle);
					return;
				}

				SourcePlayer.PushChatMessage(EChatChannel.Notifications, "You are already in a vehicle.");
				return;
			}

			const int g_SupermanVehicleIndex = 522;
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(g_SupermanVehicleIndex);
			Vector3 vecRot = SourcePlayer.Client.Rotation;
			Vector3 vecPos = SourcePlayer.Client.Position;
			const UInt32 expiryTime = 0;
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			int livery = 0;
			EVehicleTransmissionType transmission = EVehicleTransmissionType.Automatic;
			uint addonModel = 0;

			await VehiclePool.CreateVehicle(-(++totalTempVehicles), EVehicleType.Temporary, 0, vehicleDef.Hash != 0 ? (VehicleHash)vehicleDef.Hash : 0, vecPos, vecRot, vecPos, vecRot, EPlateType.Blue_White, g_PlateText, 100.0f, 105, 105, 105, 105, 105,
				105, 1, livery, 0.0f, 100.0f, false, true, 0, 0, 0, 0.0f, false, false, expiryTime, 1337.0f, false, SourcePlayer.Client.Dimension, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, transmission, 0, addonModel,
				vehicle =>
				{
					SourcePlayer.Client.SetIntoVehicle(vehicle.GTAInstance, 0);
					vehicle.GTAInstance.Rotation = vecRot;
					vehicle.SetData(vehicle.GTAInstance, EDataNames.VEH_INVISIBLE, true, EDataType.Synced);
					vehicle.SetData(vehicle.GTAInstance, EDataNames.VEHICLE_HANDBRAKE, false, EDataType.Synced);
				}).ConfigureAwait(true);
		}

		private void MakeVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, int index, string plate, string owner, int colorPrimaryR, int colorPrimaryG, int colorPrimaryB, int colorSecondaryR, int colorSecondaryG, int colorSecondaryB, EVehicleTransmissionType transmission, EPlateType plateType = 0, int livery = 0)
		{
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(index);
			if (vehicleDef != null)
			{
				if (plate.Length > 2 && plate.Length <= 8 && colorPrimaryR >= 0 && colorPrimaryR <= 255 && colorPrimaryG >= 0 && colorPrimaryG <= 255 && colorPrimaryB >= 0 && colorPrimaryB <= 255 && colorSecondaryR >= 0 && colorSecondaryR <= 255 && colorSecondaryG >= 0 && colorSecondaryG <= 255 && colorSecondaryB >= 0 && colorSecondaryB <= 255 && livery >= 0 && livery <= 3)
				{
					// check for plate dupes
					bool bPlateUnique = true;
					foreach (var vehicle in NAPI.Pools.GetAllVehicles())
					{
						if (vehicle.NumberPlate.ToLower() == plate.ToLower())
						{
							bPlateUnique = false;
							break;
						}
					}

					if (bPlateUnique)
					{
						WeakReference<CPlayer> OwnerPlayerRef = CommandManager.FindTargetPlayer(SourcePlayer, owner.Replace("_", " "));
						CPlayer OwnerPlayer = OwnerPlayerRef.Instance();
						if (OwnerPlayer != null)
						{
							Vector3 vecRot = SourcePlayer.Client.Rotation;
							Vector3 vecPos = SourcePlayer.Client.Position;
							float fDist = 5.0f;
							float radians = (vecRot.Z + 90.0f) * (3.14f / 180.0f);
							vecPos.X += (float)Math.Cos(radians) * fDist;
							vecPos.Y += (float)Math.Sin(radians) * fDist;

							NAPI.Task.Run(async () =>
							{
								const UInt32 expiryTime = 0;

								Int64 unixTimestamp = Helpers.GetUnixTimestamp();
								uint addonModel = 0;

								if (vehicleDef.Hash == 0)
								{
									if (!string.IsNullOrEmpty(vehicleDef.AddOnName))
									{
										addonModel = NAPI.Util.GetHashKey(vehicleDef.AddOnName);
									}
								}
								CVehicle vehCreated = await VehiclePool.CreateVehicle(0, EVehicleType.PlayerOwned, OwnerPlayer.ActiveCharacterDatabaseID, vehicleDef.Hash != 0 ? (VehicleHash)vehicleDef.Hash : 0, vecPos, vecRot, vecPos, vecRot, plateType, plate, 100.0f, colorPrimaryR, colorPrimaryG, colorPrimaryB, colorSecondaryR, colorSecondaryG, colorSecondaryB, 1, livery, 0.0f, 100.0f, false, false, 0, 0, 0, 0.0f, true, plate == "GENERATE", expiryTime, 0.0f, false, SourcePlayer.Client.Dimension, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, transmission, 0, addonModel).ConfigureAwait(true);

								CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, vehCreated.m_DatabaseID);
								OwnerPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
								{
									if (!bItemGranted)
									{
										SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Makeveh: Player couldn't receive the vehicle keys. The player will have to make inventory space and then an admin can /makevehkey.");
									}
								});
							});
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Player not found.");
						}
					}
					else
					{
						SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "The vehicle plate must be unique, that plate is already in use.");
					}
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "SYNTAX: [Model] [Plate] [Owner ID/Partial] [Primary R] [Primary G] [Primary B] [Secondary R] [Secondary G] [Secondary B] (Plate Type)");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid vehicle Index.");
			}
		}

		private void MakeCivVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, int index, EVehicleTransmissionType transmission = EVehicleTransmissionType.Automatic)
		{
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(index);
			if (vehicleDef != null)
			{
				string strPlate = "GENERATE";
				Random rng = new Random();
				int colorPrimaryR = rng.Next();
				int colorPrimaryG = rng.Next();
				int colorPrimaryB = rng.Next();
				int colorSecondaryR = rng.Next();
				int colorSecondaryG = rng.Next();
				int colorSecondaryB = rng.Next();

				Vector3 vecRot = SourcePlayer.Client.Rotation;
				Vector3 vecPos = SourcePlayer.Client.Position;
				float fDist = 5.0f;
				float radians = (vecRot.Z + 90.0f) * (3.14f / 180.0f);
				vecPos.X += (float)Math.Cos(radians) * fDist;
				vecPos.Y += (float)Math.Sin(radians) * fDist;

				NAPI.Task.Run(async () =>
				{
					const UInt32 expiryTime = 0;

					Int64 unixTimestamp = Helpers.GetUnixTimestamp();
					uint addonModel = 0;

					if (vehicleDef.Hash == 0)
					{
						if (!string.IsNullOrEmpty(vehicleDef.AddOnName))
						{
							addonModel = NAPI.Util.GetHashKey(vehicleDef.AddOnName);
						}
					}
					CVehicle vehCreated = await VehiclePool.CreateVehicle(0, EVehicleType.Civilian, -1, vehicleDef.Hash != 0 ? (VehicleHash)vehicleDef.Hash : 0, vecPos, vecRot, vecPos, vecRot, 0, strPlate, 100.0f, colorPrimaryR, colorPrimaryG, colorPrimaryB, colorSecondaryR, colorSecondaryG, colorSecondaryB, 1, 0, 0.0f, 100.0f, false, false, 0, 0, 0, 0.0f, true, true, expiryTime, 0.0f, false, SourcePlayer.Client.Dimension, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, transmission, 0, addonModel).ConfigureAwait(true);

					CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, vehCreated.m_DatabaseID);
				});
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid vehicle Index.");
			}
		}

		private void MakeTypedVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, int index, EVehicleType type, string strPlate, int colorPrimaryR, int colorPrimaryG, int colorPrimaryB, int colorSecondaryR, int colorSecondaryG, int colorSecondaryB, EVehicleTransmissionType transmission)
		{
			CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(index);
			if (vehicleDef != null)
			{

				Vector3 vecRot = SourcePlayer.Client.Rotation;
				Vector3 vecPos = SourcePlayer.Client.Position;
				float fDist = 5.0f;
				float radians = (vecRot.Z + 90.0f) * (3.14f / 180.0f);
				vecPos.X += (float)Math.Cos(radians) * fDist;
				vecPos.Y += (float)Math.Sin(radians) * fDist;

				NAPI.Task.Run(async () =>
				{
					const UInt32 expiryTime = 0;

					Int64 unixTimestamp = Helpers.GetUnixTimestamp();
					uint addonModel = 0;

					if (vehicleDef.Hash == 0)
					{
						if (!string.IsNullOrEmpty(vehicleDef.AddOnName))
						{
							addonModel = NAPI.Util.GetHashKey(vehicleDef.AddOnName);
						}
					}
					CVehicle vehCreated = await VehiclePool.CreateVehicle(0, type, -1, vehicleDef.Hash != 0 ? (VehicleHash)vehicleDef.Hash : 0, vecPos, vecRot, vecPos, vecRot, 0, strPlate, 100.0f, colorPrimaryR, colorPrimaryG, colorPrimaryB, colorSecondaryR, colorSecondaryG, colorSecondaryB, 1, 0, 0.0f, 100.0f, false, false, 0, 0, 0, 0.0f, true, strPlate.ToUpper() == "GENERATE", expiryTime, 0.0f, false, SourcePlayer.Client.Dimension, new Dictionary<EModSlot, int>(), new Dictionary<int, bool>(), false, 0, 0, 0, unixTimestamp, -1, true, false, transmission, 0, addonModel).ConfigureAwait(true);

					CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.VEHICLE_KEY, vehCreated.m_DatabaseID);
				});
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid vehicle Index.");
			}
		}

		private void DeleteThisVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			if (SourceVehicle == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You are not in a vehicle.");
				return;
			}
			DeleteVehicle(SourcePlayer, SourceVehicle, SourceVehicle.m_DatabaseID);
		}

		private void DeleteVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID vehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (Vehicle == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
				return;
			}

			if (Vehicle.m_DatabaseID > 0)
			{
				NetworkEventSender.SendNetworkEvent_AdminConfirmEntityDelete(SourcePlayer, vehicleID, EEntityType.Vehicle);
				return;
			}

			// For temporary vehicles just delete.
			OnConfirmVehicleDelete(SourcePlayer, vehicleID);
		}

		private void OnConfirmVehicleDelete(CPlayer SourcePlayer, EntityDatabaseID vehicleID)
		{
			if (SourcePlayer.IsAdmin())
			{
				// check vehicle still exists, might have been destroyed by another admin while the UI was up
				CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
				if (Vehicle != null)
				{
					string name = Vehicle.GetFullDisplayName();
					Vehicle.GetOwnerText((string VehicleOwnerText) =>
					{
						VehiclePool.DestroyVehicle(Vehicle);
						SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a {0} (#{1}).", name, vehicleID);

						Log.CreateLog(SourcePlayer, ELogType.AdminCommand, new List<CBaseEntity>() { Vehicle }, $"/delveh {name} (Owner: {VehicleOwnerText})");
					});
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Vehicle not found.");
				}
			}
		}

		private void NearbyVehiclesCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			// TODO: C# doesn't seem to have the getVehiclesInRange function that the NodeJS version does.
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Vehicles:");
			Dictionary<CVehicle, float> NearbyVehDict = new Dictionary<CVehicle, float>();
			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				if (SourcePlayer.Client.Dimension == vehicle.Dimension)
				{
					if (SourcePlayer.Client.Position.DistanceTo2D(vehicle.Position) <= NEARBY_VEHICLES_DISTANCE)
					{
						float fDistanceToveh = SourcePlayer.Client.Position.DistanceTo2D(vehicle.Position);
						CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

						if (pVehicle != null)
						{
							NearbyVehDict.Add(pVehicle, fDistanceToveh);
						}
					}
				}
			}

			foreach (var NearbyVeh in NearbyVehDict.OrderBy(i => i.Value))
			{
				NearbyVeh.Key.GetOwnerText((string ownerText) =>
				{
					Color primaryCol = NearbyVeh.Key.GetRGBPrimaryColor();
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - {1} - {2} ({3})", NearbyVeh.Key.m_DatabaseID, NearbyVeh.Key.GetPlateText(true), Helpers.ColorString(primaryCol.Red, primaryCol.Green, primaryCol.Blue, NearbyVeh.Key.GetFullDisplayName()), ownerText);
				});
			}
		}

		private void ThisVehicleCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			SourceVehicle.GetOwnerText((string ownerText) =>
			{
				Color primaryCol = SourceVehicle.GetRGBPrimaryColor();
				if (SourcePlayer.IsAdmin())
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Current Vehicle #{0} - {1} {2} ({3})", SourceVehicle.m_DatabaseID, SourceVehicle.GetPlateText(true), Helpers.ColorString(primaryCol.Red, primaryCol.Green, primaryCol.Blue, SourceVehicle.GetFullDisplayName()), ownerText);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Current Vehicle #{0}", SourceVehicle.m_DatabaseID);
				}
			});
		}

		private void OldVehicleCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			EntityDatabaseID oldVeh = EntityDataManager.GetData<EntityDatabaseID>(SourcePlayer.Client, EDataNames.PREVIOUS_VEH);
			CVehicle pVehicle = VehiclePool.GetVehicleFromID(oldVeh);
			if (pVehicle == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You were not in a vehicle in this session.");
				return;
			}

			SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "Last Vehicle ID: {0}", pVehicle.m_DatabaseID);
		}

		private void SetPlayerInVehicleCommand(CPlayer SourcePlayer, CVehicle SourceVehicle, CPlayer TargetPlayer, EntityDatabaseID vehicleID, int seat)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(vehicleID);
			if (vehicleID == 0 || Vehicle == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Unknown Vehicle.");
				return;
			}

			if (seat > 7 || seat < 0)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Seat must be between 0 and 3.");
				return;
			}

			// Done checking args
			TargetPlayer.SetPositionSafe(Vehicle.GTAInstance.Position);
			MainThreadTimerPool.CreateEntityTimer((a) =>
			{
				TargetPlayer.Client.SetIntoVehicle(Vehicle.GTAInstance.Handle, seat);
			}, 30, this, 1, new object[] { this });
		}

		private void SetDirtCommand(CPlayer SenderPlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, float fDirt)
		{
			if (fDirt < 0 || fDirt > 15.0f)
			{
				SenderPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid dirt value. Dirt must be between 0.0 and 15.0");
				return;
			}

			if (!TargetPlayer.Client.IsInVehicle)
			{
				SenderPlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "{0} is not in a vehicle.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
				return;
			}

			CVehicle Vehicle = VehiclePool.GetVehicleFromGTAInstance(TargetPlayer.Client.Vehicle);
			if (Vehicle != null)
			{
				SenderPlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "You have set the dirt level of the vehicle of {0} to {1}.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), fDirt);

				if (SenderPlayer != TargetPlayer)
				{
					TargetPlayer.SendNotification("Vehicle", ENotificationIcon.InfoSign, "{0} has set your vehicle dirt level to {1}.", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), fDirt);
				}

				Vehicle.Dirt = fDirt;
			}
			else
			{
				SenderPlayer.SendNotification("Vehicle", ENotificationIcon.ExclamationSign, "{0} is not in a vehicle.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName));
				return;
			}
		}

		private void SetPlateForVehicle(CPlayer SourcePlayer, CVehicle SourceVehicle, string plate)
		{
			if (SourceVehicle != null)
			{
				string plateText = plate;
				bool isParcable = int.TryParse(plateText, out int number);
				if (isParcable && number == -1)
				{
					plateText = HelperFunctions.Vehicle.GenerateLicensePlate(SourceVehicle.m_DatabaseID);
				}

				if (string.IsNullOrEmpty(plateText) || plateText.Length > 8)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You must fill in a valid plate in order to change it.");
				}
				else
				{
					SourceVehicle.SetPlateText(plateText, true);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Set plate text to {0} from vehicle with ID {1}.", plateText, SourceVehicle.m_DatabaseID);
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You must be in a vehicle to execute this command.");
			}
		}

		private void SetPlateType(CPlayer SourcePlayer, CVehicle SourceVehicle, EPlateType type)
		{
			if (SourceVehicle != null)
			{
				if (Enum.IsDefined(typeof(EPlateType), type))
				{
					SourceVehicle.SetPlateStyle(type, true);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Set plate style from vehicle with ID {0}.", SourceVehicle.m_DatabaseID);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You must fill in a valid plate type in order to change it.");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You must be in a vehicle to execute this command.");
			}
		}

		private void TogVehPlate(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID VehicleID)
		{
			CVehicle Vehicle = VehiclePool.GetVehicleFromID(VehicleID);
			if (Vehicle != null)
			{
				bool bIsPlateToggled = Vehicle.IsPlateToggled();
				bool toggleVal = bIsPlateToggled ? false : true;

				Vehicle.TogglePlate(toggleVal, Vehicle.GetPlateText(true));
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Toggled the plates {1} for vehicle {0}.", Vehicle.m_DatabaseID, toggleVal ? "ON" : "OFF");
			}
		}

		private void ChangeVehicleModel(CPlayer SourcePlayer, CVehicle SourceVehicle, int vehicleDefinitionID)
		{
			if (SourceVehicle != null)
			{
				CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromIndex(vehicleDefinitionID);
				if (vehicleDef != null)
				{
					VehiclePool.SetVehicleModel(SourceVehicle, vehicleDef.Hash);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Set vehicle model for vehicle with ID {0} to {1}.", SourceVehicle.m_DatabaseID, vehicleDef.Name);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid Vehicle Index.");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You must be in a vehicle to execute this command.");
			}
		}

		private void RemoveAllVehicleDutyItems(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			int itemsTotal = 0;
			int vehiclesTotal = 0;

			int itemsRemoved = 0;
			int vehiclesRemoved = 0;

			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

				if (pVehicle != null)
				{
					if (pVehicle.IsVehiclePDorFDVehicle())
					{
						++vehiclesTotal;

						List<CItemInstanceDef> lstItems = pVehicle.Inventory.GetAllItems();
						itemsTotal += lstItems.Count;

						List<CItemInstanceDef> lstItemsToRemove = new List<CItemInstanceDef>();
						foreach (CItemInstanceDef item in lstItems)
						{
							object val = item.Value;
							if (val is CItemValueBasic)
							{
								CItemValueBasic valCasted = (CItemValueBasic)val;
								if (valCasted.duty)
								{
									lstItemsToRemove.Add(item);
									++itemsRemoved;
								}
							}
						}

						// did we remove something?
						if (lstItemsToRemove.Count > 0)
						{
							++vehiclesRemoved;
						}

						foreach (CItemInstanceDef itemToRemove in lstItemsToRemove)
						{
							pVehicle.Inventory.RemoveAndDestroyItem_Recursive(itemToRemove);
						}
					}
				}
			}

			SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have removed {0} duty items (total items: {1}) from {2}/{3} vehicles", itemsRemoved, itemsTotal, vehiclesRemoved, vehiclesTotal);
		}


		private void SetVehColor(CPlayer SourcePlayer, CVehicle SourceVehicle, int colorPrimaryR, int colorPrimaryG, int colorPrimaryB, int colorSecondaryR, int colorSecondaryG, int colorSecondaryB)
		{
			if (SourceVehicle != null)
			{
				if (colorPrimaryR >= 0 && colorPrimaryR <= 255 && colorPrimaryG >= 0 && colorPrimaryG <= 255 && colorPrimaryB >= 0 && colorPrimaryB <= 255 && colorSecondaryR >= 0 && colorSecondaryR <= 255 && colorSecondaryG >= 0 && colorSecondaryG <= 255 && colorSecondaryB >= 0 && colorSecondaryB <= 255)
				{
					SourceVehicle.SetVehColor(colorPrimaryR, colorPrimaryG, colorPrimaryB, colorSecondaryR, colorSecondaryG, colorSecondaryB);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Changed colors on vehicle with ID {0}.", SourceVehicle.m_DatabaseID);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid colors");
				}

			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "You are not in a vehicle!");
			}
		}

		private async void SetVehTransmission(CPlayer SourcePlayer, CVehicle SourceVehicle, EVehicleTransmissionType transmission)
		{
			if (SourceVehicle != null)
			{
				SourceVehicle.SetVehTransmission(transmission);
				await VehiclePool.ReloadVehicle(SourceVehicle).ConfigureAwait(true);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Changed transmission to {1} on vehicle with ID {0}.", SourceVehicle.m_DatabaseID, transmission.ToString());
				Log.CreateLog(SourcePlayer, ELogType.AdminCommand, new List<CBaseEntity>() { SourceVehicle }, $"/setvehtransmission veh ID: {SourceVehicle.m_DatabaseID}");

			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "You are not in a vehicle!");
			}
		}

		private void SetSpecialColor(CPlayer player, CVehicle vehicle, string vehicleId, int pearColor)
		{
			EntityDatabaseID vehId = -1;
			bool bSuccess = false;

			if (vehicleId == "*" && vehicle != null)
			{
				vehId = vehicle.m_DatabaseID;
				bSuccess = true;
			}
			else
			{
				// We can assume here it's not a string so we try to cast it to an integer.
				if (int.TryParse(vehicleId, out int parsedVehId))
				{
					vehId = parsedVehId;
					bSuccess = true;
				}
			}

			if (bSuccess && VehiclePool.GetVehicleFromID(vehId) is { } vehToUse)
			{
				vehToUse.SetVehPearlColor(pearColor);
				player.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, Helpers.FormatString("Successfully set the pearlescent color. Vehicle #{0}", vehToUse.m_DatabaseID));
				Log.CreateLog(player, ELogType.AdminCommand, new List<CBaseEntity>() { vehToUse }, $"/setspecialcolor veh ID: {vehToUse.m_DatabaseID}");
			}
			else
			{
				player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, Helpers.FormatString("Invalid vehicle id"));
			}
		}

		private static Int64 totalTempVehicles = 0;
	}
}
