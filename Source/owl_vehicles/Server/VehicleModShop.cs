using GTANetworkAPI;
using System.Collections.Generic;

public class VehicleModShop
{
	public VehicleModShop()
	{
		NetworkEvents.GotoVehicleModShop += OnGotoModShop;
		NetworkEvents.VehicleModShop_OnExit_Discard += OnExit_Discard;
		NetworkEvents.VehicleModShop_OnCheckout += OnCheckout;
		NetworkEvents.VehicleModShop_GetPrice += GetPrice;
		NetworkEvents.VehicleModShop_GetModPrice += GetModPrice;
	}

	private void GetPrice(CPlayer a_Player, Dictionary<EModSlot, int> lstMods, string strNewPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled)
	{
		CalculateCostOfChanges(a_Player, lstMods, strNewPlateText, neon_r, neon_g, neon_b, neons_enabled, out float fCostMoney, out int CostGC, true, out Dictionary<EModSlot, string> dictOverviewPrices);

		NetworkEventSender.SendNetworkEvent_VehicleModShop_GotPrice(a_Player, fCostMoney, CostGC, dictOverviewPrices);
	}

	private void GetModPrice(CPlayer a_Player, EModSlot modSlot, int modIndex, string strNewPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled)
	{
		Dictionary<EModSlot, int> fakeMods = new Dictionary<EModSlot, int>();
		fakeMods.Add(modSlot, modIndex);

		CalculateCostOfChanges(a_Player, fakeMods, strNewPlateText, neon_r, neon_g, neon_b, neons_enabled, out float fCostMoney, out int CostGC, false, out Dictionary<EModSlot, string> dictOverviewPrices);
		NetworkEventSender.SendNetworkEvent_VehicleModShop_GotModPrice(a_Player, fCostMoney, CostGC);
	}

	private void CalculateCostOfChanges(CPlayer a_Player, Dictionary<EModSlot, int> lstMods, string strNewPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled, out float fCostMoney, out int CostGC, bool bGenerateOverview, out Dictionary<EModSlot, string> dictOverviewPrices)
	{
		float fCostOfChangesMoney = 0.0f;
		int CostOfChangesGC = 0;
		dictOverviewPrices = new Dictionary<EModSlot, string>();

		Vehicle currentVehicle = a_Player.Client.Vehicle;

		if (currentVehicle != null)
		{
			// NOTE: This much match clientside side in calculate mod cost too
			foreach (var kvPair in lstMods)
			{
				bool bHasChanged = false;

				EModSlot slot = kvPair.Key;
				int modID = kvPair.Value;

				// TODO_VEHICLE_MODS: probably doesn't work for certain types
				// basic mods
				if (slot >= EModSlot.Spoilers)
				{
					if (slot == EModSlot.WindowTint)
					{
						if (currentVehicle.WindowTint != modID)
						{
							bHasChanged = true;
						}
					}
					else
					{
						bool bIsDefaultMod = (currentVehicle.GetMod((int)slot) == 255 && modID == 0);
						if (currentVehicle.GetMod((int)slot) != modID && !bIsDefaultMod)
						{
							bHasChanged = true;
						}
					}
				}
				else
				{
					if (slot == EModSlot.CustomizePlateStyle)
					{
						// counter for the exempt plate
						if (modID == 4)
						{
							modID++;
						}

						if (currentVehicle.NumberPlateStyle != modID)
						{
							bHasChanged = true;
						}
					}
					else if (slot == EModSlot.CustomizePlateText)
					{
						if (currentVehicle.NumberPlate.ToLower() != strNewPlateText.ToLower())
						{
							bHasChanged = true;
						}
					}
					else if (slot == EModSlot.Neons)
					{
						Color currentNeonColor = currentVehicle.NeonColor;
						bool currentNeonState = currentVehicle.Neons;
						bool bColorChanged = (currentNeonColor.Red != neon_r || currentNeonColor.Green != neon_g || currentNeonColor.Blue != neon_b);

						if (currentNeonState != neons_enabled)
						{
							bHasChanged = true;
						}
						else if (neons_enabled && bColorChanged) // color changed and it IS enabled, if its disabled, we don't care that they changed the color picker
						{
							bHasChanged = true;
						}
					}
				}

				if (bHasChanged)
				{
					VehicleModHelpers.CalculateModChangeCost(slot, modID, out float fModCost, out int ModCostGC);
					fCostOfChangesMoney += fModCost;
					CostOfChangesGC += ModCostGC;

					// Do we need the overview data?
					if (bGenerateOverview)
					{
						string strPrice = null;
						if (fModCost > 0)
						{
							strPrice = Helpers.FormatString("${0}", fModCost);
						}
						else if (ModCostGC > 0)
						{
							strPrice = Helpers.FormatString("{0} GC", ModCostGC);
						}

						if (strPrice != null)
						{
							dictOverviewPrices[slot] = strPrice;
						}
					}
				}
			}
		}

		fCostMoney = fCostOfChangesMoney;
		CostGC = CostOfChangesGC;
	}

	private void OnCheckout(CPlayer a_Player, Dictionary<EModSlot, int> lstMods, string strNewPlateText, int neon_r, int neon_g, int neon_b, bool neons_enabled)
	{
		CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(a_Player.Client.Vehicle);

		if (vehicle != null)
		{
			EVehicleModShopCheckoutResult result = EVehicleModShopCheckoutResult.CannotAfford;
			bool bPlateHasChanged = (strNewPlateText.ToLower() != vehicle.GTAInstance.NumberPlate.ToLower());

			if (bPlateHasChanged)
			{
				Database.Functions.Vehicles.IsPlateUnique(strNewPlateText, (bool bPlateUnique) =>
				{
					int plateLen = strNewPlateText.Length;
					bool bPlateValid = plateLen >= 2 && plateLen <= 8 && (System.Text.RegularExpressions.Regex.IsMatch(strNewPlateText, @"[a-zA-Z]") || System.Text.RegularExpressions.Regex.IsMatch(strNewPlateText, @"[0-9]"));
					GotPlateUniqueness(bPlateUnique, bPlateValid);
				});
			}
			else
			{
				GotPlateUniqueness(true, true);
			}

			async void GotPlateUniqueness(bool bPlateUnique, bool bPlateValid)
			{
				CalculateCostOfChanges(a_Player, lstMods, strNewPlateText, neon_r, neon_g, neon_b, neons_enabled, out float fCostMoney, out int CostGC, false, out Dictionary<EModSlot, string> dictOverviewPrices);
				if (!a_Player.CanPlayerAffordBankCost(fCostMoney))
				{
					result = EVehicleModShopCheckoutResult.CannotAfford;
				}
				else if (await a_Player.GetDonatorCurrency().ConfigureAwait(true) < CostGC)
				{
					result = EVehicleModShopCheckoutResult.CannotAffordGC;
				}
				else if (bPlateHasChanged && !bPlateUnique)
				{
					result = EVehicleModShopCheckoutResult.PlateNotUnique;
				}
				else if (bPlateHasChanged && !bPlateValid)
				{
					result = EVehicleModShopCheckoutResult.PlateNotValid;
				}
				else
				{
					if (fCostMoney > 0.0f)
					{
						a_Player.SubtractBankBalanceIfCanAfford(fCostMoney, PlayerMoneyModificationReason.VehicleModShop);
					}

					if (CostGC > 0)
					{
						a_Player.SubtractDonatorCurrency(CostGC);
					}


					foreach (var kvPair in lstMods)
					{
						EModSlot slot = (EModSlot)kvPair.Key;
						int modID = kvPair.Value;
						if (slot >= EModSlot.Spoilers)
						{
							vehicle.SetMod(slot, modID, true, false);
						}
						else
						{
							if (slot == EModSlot.CustomizePlateStyle)
							{
								// counter for the exempt plate
								EPlateType plateType = (EPlateType)modID;
								if (plateType == EPlateType.Blue_White_3_EXEMPT)
								{
									plateType++;
								}

								vehicle.SetPlateStyle(plateType, true);
							}
							else if (slot == EModSlot.CustomizePlateText)
							{
								vehicle.SetPlateText(strNewPlateText, true);
							}
							else if (slot == EModSlot.Neons)
							{
								vehicle.SetNeonsState(neons_enabled, neon_r, neon_g, neon_b, true);
							}
						}
					}

					vehicle.ApplyMods();

					RestoreToWorld(a_Player);

					result = EVehicleModShopCheckoutResult.Success;
				}

				NetworkEventSender.SendNetworkEvent_VehicleModShop_OnCheckout_Response(a_Player, result);
			}
		}
	}

	private void OnExit_Discard(CPlayer a_Player)
	{
		// restore the mods, which will overwrite anything the player did clientside
		CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(a_Player.Client.Vehicle);

		if (vehicle != null)
		{
			vehicle.ApplyMods();
		}

		RestoreToWorld(a_Player);
	}

	private void RestoreToWorld(CPlayer a_Player)
	{
		if (a_Player.IsInVehicleReal)
		{
			CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(a_Player.Client.Vehicle);

			if (vehicle != null)
			{
				vehicle.IsInModShop = false;

				// warping their vehicle also warps them
				List<CPlayer> lstOccupants = VehiclePool.GetVehicleOccupants(vehicle);
				vehicle.TeleportAndWarpOccupants(lstOccupants, vehicle.CachedPositionBeforeModShop, 0, vehicle.CachedRotationBeforeModShop);
				vehicle.GTAInstance.Rotation = vehicle.CachedRotationBeforeModShop;
			}
		}
	}

	private void OnGotoModShop(CPlayer a_Player)
	{
		if (a_Player.IsInVehicleReal)
		{
			CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(a_Player.Client.Vehicle);

			if (vehicle != null)
			{
				// Check if eligible to mod this vehicle
				bool bCanModVehicle = false;
				string strMessage = "";
				if (vehicle.VehicleType == EVehicleType.PlayerOwned)
				{
					// are we the owner?
					if (vehicle.OwnedBy(a_Player))
					{
						bCanModVehicle = true;
					}
					else
					{
						strMessage = "You must be the vehicle owner to mod it.";
					}
				}
				else if (vehicle.VehicleType == EVehicleType.FactionOwned)
				{
					CFaction factionOwner = vehicle.GetFactionOwner();

					if (factionOwner != null)
					{
						// check the faction isnt law enforcement
						if (factionOwner.Type != EFactionType.LawEnforcement && factionOwner.Type != EFactionType.Medical)
						{
							// check if we are a faction manager
							if (a_Player.IsFactionManager(factionOwner.FactionID))
							{
								bCanModVehicle = true;
							}
							else
							{
								strMessage = "Vehicles belonging to a faction can only be modded if you are a faction manager.";
							}
						}
						else
						{
							strMessage = "Vehicles belonging to Law Enforcement / FD EMS factions cannot be modded.";
						}
					}
				}
				else
				{
					strMessage = "Vehicles of this type cannot be modded.";
				}

				if (bCanModVehicle)
				{
					vehicle.CachedPositionBeforeModShop = vehicle.GTAInstance.Position;
					vehicle.CachedRotationBeforeModShop = vehicle.GTAInstance.Rotation;
					vehicle.IsInModShop = true;

					vehicle.GiveTeleportImmunity();
					List<CPlayer> lstOccupants = VehiclePool.GetVehicleOccupants(vehicle);
					vehicle.TeleportAndWarpOccupants(lstOccupants, VehicleModHelpers.VecModShopCarPosition, a_Player.GetPlayerSpecificDimension(), new Vector3(0.0f, 0.0f, VehicleModHelpers.fModShopCarRotation));

					NetworkEventSender.SendNetworkEvent_GotoVehicleModShop_Approved(a_Player, vehicle.GTAInstance);
				}
				else
				{
					a_Player.SendNotification("Vehicle Mod Shop", ENotificationIcon.ExclamationSign, strMessage);
				}
			}

		}
	}
}