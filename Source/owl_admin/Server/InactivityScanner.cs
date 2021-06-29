using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EntityDatabaseID = System.Int64;
using Vehicle = GTANetworkAPI.Vehicle;

namespace InactivityScanner
{
	public enum ERefundType
	{
		Monetary,
		Token,
		Credit,
		None
	}

	public class InactivityScanner
	{
		WeakReference<MainThreadTimer> g_tmrInactivityScan = new WeakReference<MainThreadTimer>(null);
		private readonly List<int> hoursToNotify = new List<int>() { 1, 2, 3, 6, 12 };

		public InactivityScanner()
		{
			// COMMANDS
			CommandManager.RegisterCommand("inactivityscan", "Runs an inactivity scan", new Action<CPlayer, CVehicle>(AdminRunInactivityScanner), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminIgnoreDuty);

			g_tmrInactivityScan = MainThreadTimerPool.CreateGlobalTimer(DoInactivityScan, 3600000);
		}

		private async void DoInactivityScan(object[] parameters = null)
		{
			int numInactiveVeh = 0;
			int numTotalVeh = 0;
			foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
			{
				CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

				if (pVehicle != null)
				{
					bool bIsInactive = await pVehicle.IsInactive().ConfigureAwait(true);
					if (bIsInactive)
					{
						++numInactiveVeh;
						ERefundType refundType = await RefundVehicleIfApplicable(pVehicle).ConfigureAwait(true);
						SendVehicleWarning(pVehicle, refundType);
						pVehicle.Repossess();
					}

					++numTotalVeh;
				}
			}

			int numInactiveProp = 0;
			int numTotalProp = 0;
			foreach (CPropertyInstance property in PropertyPool.GetAllPropertyInstances())
			{
				bool bIsInactive = await property.IsInactive().ConfigureAwait(true);
				if (bIsInactive)
				{
					++numInactiveProp;
					ERefundType refundType = await RefundPropertyIfApplicable(property).ConfigureAwait(true);
					SendPropertyWarning(property, refundType);
					property.Repossess();
				}

				++numTotalProp;
			}

			if (numInactiveVeh > 0)
			{
				DiscordBotIntegration.PushChannelMessage(
					EDiscordChannelIDs.AdminCommands,
					Helpers.FormatString("[INACTIVITY SCANNER] Deleted {0}/{1} vehicles.", numInactiveVeh, numTotalVeh)
				);
			}

			if (numInactiveProp > 0)
			{
				DiscordBotIntegration.PushChannelMessage(
					EDiscordChannelIDs.AdminCommands,
					Helpers.FormatString("[INACTIVITY SCANNER] Repossessed {0}/{1} properties.", numInactiveProp, numTotalProp)
				);
			}
		}

		private void AdminRunInactivityScanner(CPlayer SenderPlayer, CVehicle SenderVehicle)
		{
			if (SenderPlayer.IsAdmin(EAdminLevel.HeadAdmin))
			{
				HelperFunctions.Chat.SendMessageToAdmins(Helpers.FormatString("[INACTIVITY SCANNER] {0} ({1}) executed an inactivity scan!", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username));
				DoInactivityScan();
			}
		}

		private bool IsBetweenHours(TimeSpan span, int less, int greater)
		{
			return span < TimeSpan.FromHours(less) && span > TimeSpan.FromHours(greater);
		}

		private async void SendPropertyWarning(CPropertyInstance property, ERefundType refundType)
		{
			if (!await property.CanBeInactive().ConfigureAwait(true))
			{
				return;
			}

			SendWarning(property.LastUsed, "property", property.Model.Name, property.Model.OwnerId, refundType);
		}

		private async void SendVehicleWarning(CVehicle vehicle, ERefundType refundType)
		{
			if (!await vehicle.CanBeInactive().ConfigureAwait(true))
			{
				return;
			}

			SendWarning(vehicle.LastUsed, "vehicle", vehicle.GetFullDisplayName(), vehicle.OwnerID, refundType);
		}

		private void SendWarning(DateTime lastUsed, string type, string name, EntityDatabaseID characterID, ERefundType refundType)
		{
			DateTime inactiveAt = DateTime.Now.Subtract(TimeSpan.FromDays(InactivityScannerContains.numDaysToConsiderInactiveForUse));

			if (lastUsed < inactiveAt)
			{
				Database.Functions.Characters.GetAccountIdFromCharacterId(characterID, accountId =>
				{
					PersistentNotificationManager.SendAccountNotification(
						accountId,
						"Inactivity Scanner",
						Helpers.FormatString("Your {0}, {1}, has been deleted due to inactivity.{2}", type, name, ExtraNotificationMessage(refundType))
					);
				});
				return;
			}
			TimeSpan span = lastUsed - inactiveAt;

			foreach (var hour in hoursToNotify)
			{
				if (IsBetweenHours(span, hour + 1, hour))
				{
					Database.Functions.Characters.GetAccountIdFromCharacterId(characterID, accountID =>
					{
						PersistentNotificationManager.SendAccountNotification(
							accountID,
							"Inactivity Scanner",
							Helpers.FormatString("Your {0}, {1}, will be deleted due to inactivity in {2} hours.", type, name, hour)
						);
					});
					return;
				}
			}
		}

		private async Task<ERefundType> RefundPropertyIfApplicable(CPropertyInstance property)
		{
			// FULLY PAID OFF
			if (!property.Model.IsTokenPurchase && property.GetRemainingCredit() == 0.0f && !property.Model.IsRental)
			{
				// if the player is online but just didn't interact with the property. We give him money "the normal way"
				if (PlayerPool.TryGetPlayerFromCharacterId(property.Model.OwnerId, out WeakReference<CPlayer> playerFound))
				{
					playerFound.Instance().AddBankMoney(GetRefundAmount(property, ERefundType.Monetary), PlayerMoneyModificationReason.InactivityScanner_PropertyRefund);
				}
				else
				{
					Database.Functions.Characters.GrantCharacterBankMoney(property.Model.OwnerId, GetRefundAmount(property, ERefundType.Monetary));
				}

				return ERefundType.Monetary;
			}

			// CREDIT
			if (property.GetRemainingCredit() > 0.0f)
			{
				if (PlayerPool.TryGetPlayerFromCharacterId(property.Model.OwnerId, out WeakReference<CPlayer> playerFound))
				{
					playerFound.Instance().AddBankMoney(Math.Max(0, GetRefundAmount(property, ERefundType.Credit)), PlayerMoneyModificationReason.InactivityScanner_PropertyRefund);
				}
				else
				{
					Database.Functions.Characters.GrantCharacterBankMoney(property.Model.OwnerId, Math.Max(0, GetRefundAmount(property, ERefundType.Credit)));
				}

				return ERefundType.Credit;
			}

			// TOKEN 
			if (property.Model.IsTokenPurchase)
			{
				// if player is online and in game rn
				if (PlayerPool.TryGetPlayerFromCharacterId(property.Model.OwnerId, out WeakReference<CPlayer> playerFound))
				{
					if (!playerFound.Instance().DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.PropertyToken))
					{
						await playerFound.Instance().DonationInventory.OnPurchaseScripted(Donations.GetPropertyToken(), true).ConfigureAwait(true);
					}
					else
					{
						// UB, just return *shrug*
						return ERefundType.None;
					}
				}
				else
				{
					// player is offline:
					if (!Offline_HasActiveDonationOfEffectType(property.Model.OwnerId, EDonationEffect.PropertyToken))
					{
						await Database.LegacyFunctions.GivePlayerDonationItem(property.Model.OwnerId, Donations.GetPropertyToken()).ConfigureAwait(true);
					}
				}
				return ERefundType.Token;
			}

			return ERefundType.None;
		}

		private async Task<ERefundType> RefundVehicleIfApplicable(CVehicle vehicle)
		{
			// FULLY PAID OFF
			if (!vehicle.m_bTokenPurchase && vehicle.GetRemainingCredit() == 0.0f && !vehicle.IsRentalCar())
			{
				// if the player is online but just didn't interact with the vehicle. We give him money "the normal way"
				if (PlayerPool.TryGetPlayerFromCharacterId(vehicle.OwnerID, out WeakReference<CPlayer> playerFound))
				{
					playerFound.Instance().AddBankMoney(GetRefundAmount(vehicle, ERefundType.Monetary), PlayerMoneyModificationReason.InactivityScanner_VehicleRefund);
				}
				else
				{
					Database.Functions.Characters.GrantCharacterBankMoney(vehicle.OwnerID, GetRefundAmount(vehicle, ERefundType.Monetary));
				}

				return ERefundType.Monetary;
			}

			// CREDIT
			if (vehicle.GetRemainingCredit() > 0.0f)
			{
				if (PlayerPool.TryGetPlayerFromCharacterId(vehicle.OwnerID, out WeakReference<CPlayer> playerFound))
				{
					playerFound.Instance().AddBankMoney(Math.Max(0, GetRefundAmount(vehicle, ERefundType.Credit)), PlayerMoneyModificationReason.InactivityScanner_VehicleRefund);
				}
				else
				{
					Database.Functions.Characters.GrantCharacterBankMoney(vehicle.OwnerID, Math.Max(0, GetRefundAmount(vehicle, ERefundType.Credit)));
				}

				return ERefundType.Credit;
			}

			// TOKEN 
			if (vehicle.m_bTokenPurchase)
			{
				// if player is online and in game rn
				if (PlayerPool.TryGetPlayerFromCharacterId(vehicle.OwnerID, out WeakReference<CPlayer> playerFound))
				{
					if (!playerFound.Instance().DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.VehicleToken))
					{
						await playerFound.Instance().DonationInventory.OnPurchaseScripted(Donations.GetVehicleToken(), true).ConfigureAwait(true);
					}
					else
					{
						// UB, just return *shrug*
						return ERefundType.None;
					}
				}
				else
				{
					// player is offline:
					if (!Offline_HasActiveDonationOfEffectType(vehicle.OwnerID, EDonationEffect.VehicleToken))
					{
						await Database.LegacyFunctions.GivePlayerDonationItem(vehicle.OwnerID, Donations.GetVehicleToken()).ConfigureAwait(true);
					}
				}
				return ERefundType.Token;
			}

			return ERefundType.None;
		}

		private float GetRefundAmount(CVehicle vehicle, ERefundType type)
		{
			return type switch
			{
				ERefundType.Monetary => VehicleCrusher.GetCrushAmount(vehicle),
				ERefundType.Credit => GetVehicleCreditRefundAmount(vehicle),
				_ => throw new ArgumentOutOfRangeException(nameof(type))
			};
		}

		private float GetRefundAmount(CPropertyInstance property, ERefundType type)
		{
			return type switch
			{
				ERefundType.Monetary => property.Model.BuyPrice * 0.7f,
				ERefundType.Credit => GetPropertyCreditRefundAmount(property),
				_ => throw new ArgumentOutOfRangeException(nameof(type))
			};
		}

		private float GetVehicleCreditRefundAmount(CVehicle vehicle) => VehicleCrusher.GetCrushAmount(vehicle) - vehicle.GetRemainingCredit();
		private float GetPropertyCreditRefundAmount(CPropertyInstance property) => (property.Model.BuyPrice * 0.7f) - property.GetRemainingCredit();
		private string ExtraNotificationMessage(ERefundType refundType)
		{
			return refundType switch
			{
				ERefundType.Monetary => " - You have been refunded 33% of the original price",
				ERefundType.Token => " - You have been refunded your token",
				ERefundType.Credit => " - You have been refunded the credit amount",
				ERefundType.None => string.Empty,
				_ => throw new ArgumentOutOfRangeException(nameof(refundType), refundType, null)
			};
		}

		private bool Offline_HasActiveDonationOfEffectType(long characterId, EDonationEffect effect)
		{
			bool bHasItem = false;
			Database.Functions.Donations.GetDonationInventoryOffline(characterId, donationInventory =>
			{
				foreach (var donationItem in donationInventory)
				{
					if (donationItem.IsActive())
					{
						foreach (var purchasable in Donations.g_lstPurchasables)
						{
							if (purchasable.ID == donationItem.DonationID)
							{
								if (purchasable.DonationEffect == effect)
								{
									// It must be applied to this character OR account-wide
									if (donationItem.Character == characterId || purchasable.m_Type == EDonationType.Account)
									{
										bHasItem = true;
									}
								}
							}
						}
					}
				}
			});
			return bHasItem;
		}
	}
}