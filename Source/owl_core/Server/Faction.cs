using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// TODO_LAUNCH: Convert all timestamps to signed int
public class CFaction : CBaseEntity
{
	public CFaction(Int64 a_FactionID, EFactionType a_Type, string a_strName, bool a_bOfficial, string strShortName, string strMessage, float fMoney, List<CFactionRank> a_lstFactionRanks, Int64 a_CreatorID)
	{
		m_DatabaseID = a_FactionID;
		Type = a_Type;
		Name = a_strName;
		IsOfficial = a_bOfficial;
		ShortName = strShortName;
		Message = strMessage;
		Money = fMoney;
		FactionRanks = a_lstFactionRanks;
		CreatorID = a_CreatorID;

		m_HourTimerHandle = MainThreadTimerPool.CreateEntityTimer(ProcessHourTimer, 3600000, this);
	}

	private readonly WeakReference<MainThreadTimer> m_HourTimerHandle = new WeakReference<MainThreadTimer>(null);

	public void ProcessHourTimer(object[] a_Parameters)
	{
		// NOTE: This will not be true hours if server restarts in between, but we're fine with that since factions cant disconnect like players
		ProcessMonthlyPayments();
	}

	public List<CreditDetails> GetActiveCreditDetails()
	{
		List<CreditDetails> lstDetails = new List<CreditDetails>();

		// Vehicles
		List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromFaction(this);
		foreach (CVehicle vehicle in lstVehicles)
		{
			// Do we still have outstanding payments?
			if (vehicle.PaymentsRemaining > 0)
			{
				lstDetails.Add(new CreditDetails(vehicle.GetFullDisplayName(), vehicle.PaymentsMade, vehicle.PaymentsRemaining, vehicle.GetRemainingCredit(true), vehicle.GetRemainingCreditInterest(), ECreditType.Vehicle, vehicle.m_DatabaseID));
			}
		}

		// Properties
		List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByFaction(this);

		foreach (CPropertyInstance propertyInst in lstProperties)
		{
			// Do we still have outstanding payments?
			if (propertyInst.Model.PaymentsRemaining > 0)
			{
				lstDetails.Add(new CreditDetails(propertyInst.Model.Name, propertyInst.Model.PaymentsMade, propertyInst.Model.PaymentsRemaining, propertyInst.GetRemainingCredit(true), propertyInst.GetRemainingCreditInterest(), ECreditType.Property, propertyInst.Model.Id));
			}
		}

		return lstDetails;
	}

	public void GetActiveCredit(out float TotalCreditVehicles, out int NumVehiclesOnCredit, out float TotalCreditProperties, out int NumPropertiesOnCredit)
	{
		TotalCreditVehicles = 0.0f;
		NumVehiclesOnCredit = 0;
		TotalCreditProperties = 0.0f;
		NumPropertiesOnCredit = 0;

		// Vehicles
		List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromFaction(this);
		foreach (CVehicle vehicle in lstVehicles)
		{
			// Do we still have outstanding payments?
			if (vehicle.PaymentsRemaining > 0)
			{
				TotalCreditVehicles += vehicle.GetRemainingCredit();
				++NumVehiclesOnCredit;
			}
		}

		// Properties
		List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByFaction(this);

		foreach (CPropertyInstance propertyInst in lstProperties)
		{
			// Do we still have outstanding payments?
			if (propertyInst.Model.PaymentsRemaining > 0)
			{
				TotalCreditProperties += propertyInst.GetRemainingCredit();
				++NumPropertiesOnCredit;
			}
		}
	}
	private void ProcessMonthlyPayments()
	{
		// VEHICLES
		List<CVehicle> lstVehicles = VehiclePool.GetVehiclesFromFaction(this);

		if (lstVehicles.Count > 0)
		{
			foreach (CVehicle vehicle in lstVehicles)
			{
				// Does this vehicle have payments remaining?
				if (vehicle.PaymentsRemaining > 0)
				{
					float fMonthlyPaymentAmount = vehicle.GetMonthlyPaymentAmount();

					// Can we afford the payment?
					if (SubtractMoney(fMonthlyPaymentAmount))
					{
						float fMonthlyTax = Taxation.GetVehicleMonthlyTaxRate(vehicle.GTAInstance.Class);

						if (!SubtractMoney(fMonthlyTax))
						{
							// TODO: Do something if we cant? Police warrant? Add to MDC?
						}

						SendNotificationToAllManagers(Helpers.FormatString("{0}: ${1:0.00} (Tax: ${3:0.00} - Payments Remaining: {2})", vehicle.GetFullDisplayName(), fMonthlyPaymentAmount, vehicle.PaymentsRemaining - 1, fMonthlyTax));
						vehicle.PaymentsRemaining--;
						vehicle.PaymentsMade++;
						vehicle.PaymentsMissed = 0;
					}
					else
					{
						vehicle.PaymentsMissed++;
						SendNotificationToAllManagers(Helpers.FormatString("{0}: You missed a payment. (Payments Missed: {1}/3)", vehicle.GetFullDisplayName(), vehicle.PaymentsMissed));

						// Do we need to repossess card
						if (vehicle.PaymentsMissed >= 3)
						{
							vehicle.Repossess();
							SendNotificationToAllManagers(Helpers.FormatString("{0}: You failed to make 3 subsequent payments. This vehicle was repossessed.", vehicle.GetFullDisplayName()));
						}
					}
				}
			}
		}

		// PROPERTIES
		List<CPropertyInstance> lstProperties = PropertyPool.GetPropertyInstancesOwnedByFaction(this);

		if (lstProperties.Count > 0)
		{
			foreach (CPropertyInstance propertyInst in lstProperties)
			{
				// Does this property have payments remaining?
				if (propertyInst.Model.PaymentsRemaining > 0)
				{
					float fMonthlyPaymentAmount = propertyInst.GetMonthlyPaymentAmount();

					// Can we afford the payment?
					if (SubtractMoney(fMonthlyPaymentAmount))
					{
						float fMonthlyTax = propertyInst.GetMonthlyTax();

						// Can we afford the payment?
						if (!SubtractMoney(fMonthlyTax))
						{
							// TODO: Do something if we cant? Police warrant? Add to MDC?
						}

						SendNotificationToAllManagers(Helpers.FormatString("{0}: ${1:0.00} (Tax: ${3:0.00} - Payments Remaining: {2})", propertyInst.Model.Name, fMonthlyPaymentAmount, propertyInst.Model.PaymentsRemaining - 1, fMonthlyTax));
						propertyInst.Model.MakePayment();
					}
					else
					{
						propertyInst.Model.MissPayment();
						SendNotificationToAllManagers(Helpers.FormatString("{0}: You missed a payment. (Payments Missed: {1}/3)", propertyInst.Model.Name, propertyInst.Model.PaymentsMissed));

						// Do we need to repossess card
						if (propertyInst.Model.PaymentsMissed >= 3)
						{
							propertyInst.Repossess();
							SendNotificationToAllManagers(Helpers.FormatString("{0}: You failed to make 3 subsequent payments. This property was repossessed.", propertyInst.Model.Name));
						}
					}
				}
			}
		}
	}

	public CFactionRank GetFactionRank(int iRankIndex)
	{
		if (iRankIndex >= FactionRanks.Count)
		{
			return null;
		}

		return FactionRanks[iRankIndex];
	}

	public bool CanFactionAffordCost(float fCost)
	{
		return fCost <= Money;
	}
	public bool SubtractMoney(float fCost)
	{
		if (CanFactionAffordCost(fCost))
		{
			// This saves it also
			Money -= fCost;
			return true;
		}

		return false;
	}

	public void SubtractMoneyAllowNegatve(float fCost)
	{
		// This saves it also
		Money -= fCost;
	}

	public bool CanAffordMonthlyExpense(float fCost)
	{
		return (Money >= (fCost * 10.0f));
	}

	public async void OnDestroy()
	{
		//TODO delete all duty points and custom duty stuff when that becomes a thing.

		// Delete all faction properties
		ICollection<CPropertyInstance> interiors = PropertyPool.GetPropertyInstancesOwnedByFaction(this);
		foreach (CPropertyInstance interior in interiors)
		{
			interior.Repossess();
		}

		// Delete all faction vehicles
		ICollection<CVehicle> vehicles = VehiclePool.GetVehiclesFromFaction(this);
		foreach (CVehicle vehicle in vehicles)
		{
			VehiclePool.DestroyVehicle(vehicle);
		}
		// Remove membership from all members
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player.IsInFaction(FactionID))
			{
				player.RemoveFactionMembership(this);
			}
		}
		// Remove from DB
		await Database.LegacyFunctions.FactionDeleteFaction(FactionID).ConfigureAwait(true);
	}

	public void SendNotificationToAll(string a_strMessage)
	{
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player.IsInFaction(FactionID))
			{
				player.SendNotification(Helpers.FormatString("Faction - {0}", ShortName), ENotificationIcon.InfoSign, a_strMessage, null);
			}
		}
	}

	public void SendNotificationToAllManagers(string a_strMessage)
	{
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player.IsInFaction(FactionID))
			{
				CFactionMembership factionMembership = player.GetFactionMembershipFromFaction(this);

				if (factionMembership != null && factionMembership.Manager)
				{
					player.SendNotification(Helpers.FormatString("Faction - {0} - Managers", ShortName), ENotificationIcon.InfoSign, a_strMessage, null);
				}
			}
		}
	}

	public void SendNotificationToAllExcept(CPlayer a_SendingPlayer, string a_strMessage, CPlayer a_IgnorePlayer)
	{
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player != a_IgnorePlayer && player.IsInFaction(FactionID))
			{
				player.SendNotification(Helpers.FormatString("Faction - {0}", ShortName), ENotificationIcon.InfoSign, a_strMessage, null);
			}
		}
	}

	public int GetNumMembersOnline()
	{
		int numMembers = 0;
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player.IsInFaction(FactionID))
			{
				++numMembers;
			}
		}

		return numMembers;
	}

	public async Task<int> GetTotalNumMembers()
	{
		int totalMembers = await Database.LegacyFunctions.GetFactionNumberOfMembers(FactionID).ConfigureAwait(true);
		return totalMembers;
	}

	public List<CPlayer> GetMembers()
	{
		List<CPlayer> lstMembers = new List<CPlayer>();
		ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
		foreach (var player in players)
		{
			if (player.IsInFaction(FactionID))
			{
				lstMembers.Add(player);
			}
		}

		return lstMembers;
	}

	public bool CanDisband()
	{
		return Type == EFactionType.UserCreated || Type == EFactionType.UserCreatedCriminal;
	}

	public void UpdateFactionRanksAndSalaries(SFactionRanksUpdateStruct[] a_factionRanks)
	{
		for (int i = 0; i < FactionRanks.Count; ++i)
		{
			FactionRanks[i] = new CFactionRank(FactionRanks[i].m_DatabaseID, a_factionRanks[i].Name, a_factionRanks[i].Salary);
		}
	}

	public void SetType(EFactionType newType)
	{
		Type = newType;
		Database.Functions.Factions.SetFactionType(FactionID, Type);
	}

	public void RespawnVehicles()
	{
		List<CVehicle> lstFactionVehicles = VehiclePool.GetVehiclesFromFaction(this);
		foreach (CVehicle factionVehicle in lstFactionVehicles)
		{
			factionVehicle.Respawn(false);
		}
	}
	public Int64 FactionID => m_DatabaseID;
	public EFactionType Type { private set; get; }
	public bool IsOfficial { get; }
	public string Message { get; set; }
	private float m_fMoney = 0.0f;

	private string m_strName = string.Empty;
	private string m_shortName = string.Empty;

	public string ShortName
	{
		get => m_shortName;
		set
		{
			m_shortName = value;
			Database.LegacyFunctions.SetFactionShortName(FactionID, value);
		}
	}

	public string Name
	{
		get => m_strName;
		set
		{
			m_strName = value;
			Database.LegacyFunctions.SetFactionName(FactionID, value);
		}
	}

	public float Money
	{
		get => m_fMoney;
		set
		{
			m_fMoney = value;
			Database.LegacyFunctions.SetFactionMoney(FactionID, m_fMoney);
		}
	}

	public List<CFactionRank> FactionRanks { get; }
}