using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabaseID = System.Int64;

namespace Database.Models
{
#pragma warning disable CA1716 // Identifiers should not match keywords
	public class Property : BaseModel
#pragma warning restore CA1716 // Identifiers should not match keywords
	{
        public const uint TOTAL_MOWING_XP = 100;

        public Vector3 EntrancePosition { get; private set; }
        public Vector3 ExitPosition { get; private set; }
        public EPropertyState State { get; private set; }
        public float BuyPrice { get; private set; }
        public float RentPrice { get; private set; }
        public long OwnerId { get; private set; }
        public long RenterId { get; private set; }
        public bool Locked { get; private set; }
        public EPropertyOwnerType OwnerType { get; private set; }
        public EPropertyOwnerType RenterType { get; private set; }
        public string Name { get; private set; }
        public float EntranceRotation { get; private set; }
        public float ExitRotation { get; private set; }
        public uint EntranceDimension { get; private set; }
        public int InteriorId { get; private set; }
        public int PaymentsRemaining { get; private set; }
        public int PaymentsMade { get; private set; }
        public int PaymentsMissed { get; private set; }
        public float CreditAmount { get; private set; }
        public EPropertyEntranceType EntranceType { get; private set; }
        public long LastUsed { get; private set; }
        public List<CDatabaseStructureFurnitureItem> FurnitureItems { get; private set; }
        public List<FurnitureRemoval> FurnitureRemovals { get; private set; }
        public List<CItemInstanceDef> Inventory { get; private set; }
        public bool HasScriptedBlip { get; private set; }
        public bool IsTokenPurchase { get; private set; }
        public uint XP { get; private set; }
        public ulong LastMowedAt { get; private set; }
        
        public bool IsRental => RenterId != 0;

        public Property(EntityDatabaseID id, Vector3 entrancePosition, Vector3 exitPosition, EPropertyState state, 
            float buyPrice, float rentPrice, Int64 ownerId, Int64 renterId, bool locked, EPropertyOwnerType ownerType, 
            EPropertyOwnerType renterType, string name, float entranceRotation, float exitRotation, 
            uint entranceDimension, int interiorId, int paymentsRemaining, int paymentsMade, int paymentsMissed, 
            float creditAmount, EPropertyEntranceType entranceType, Int64 lastUsed,
            List<CDatabaseStructureFurnitureItem> furnitureItems,
            List<FurnitureRemoval> furnitureRemovals, 
            bool hasScriptedBlip, bool isTokenPurchase, uint uXP, ulong lastMowedAt)
        {
            Id = id;
            EntrancePosition = entrancePosition;
            ExitPosition = exitPosition;
            State = state;
            BuyPrice = buyPrice;
            RentPrice = rentPrice;
            OwnerId = ownerId;
            RenterId = renterId;
            Locked = locked;
            OwnerType = ownerType;
            RenterType = renterType;
            Name = name;
            EntranceRotation = entranceRotation;
            ExitRotation = exitRotation;
            EntranceDimension = entranceDimension;
            InteriorId = interiorId;
            PaymentsRemaining = paymentsRemaining;
            PaymentsMade = paymentsMade;
            PaymentsMissed = paymentsMissed;
            CreditAmount = creditAmount;
            EntranceType = entranceType;
            LastUsed = lastUsed;
            FurnitureItems = furnitureItems;
            FurnitureRemovals = furnitureRemovals;
            HasScriptedBlip = hasScriptedBlip;
            IsTokenPurchase = isTokenPurchase;
            XP = uXP;
            LastMowedAt = lastMowedAt;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                {"entrance_x", EntrancePosition.X},
                {"entrance_y", EntrancePosition.Y},
                {"entrance_z", EntrancePosition.Z},
                {"exit_x", ExitPosition.X},
                {"exit_y", ExitPosition.Y},
                {"exit_z", ExitPosition.Z},
                {"state", State},
                {"buy_price", BuyPrice},
                {"rent_price", RentPrice},
                {"owner", OwnerId},
                {"renter", RenterId},
                {"locked", Locked},
                {"owner_type", OwnerType},
                {"renter_type", RenterType},
                {"name", Name},
                {"entrance_rot", EntranceRotation},
                {"exit_rot", ExitRotation},
                {"entrance_dimension", EntranceDimension},
                {"interior_id", InteriorId},
                {"payments_made", PaymentsMade},
                {"payments_missed", PaymentsMissed},
                {"payments_remaining", PaymentsRemaining},
                {"entrance_type", EntranceType},
                {"credit_amount", CreditAmount},
                {"last_used", LastUsed},
                {"scripted_blip", HasScriptedBlip},
                {"is_token_purchase", IsTokenPurchase},
                {"xp", XP},
                {"last_mowed_at", LastMowedAt},
            };
        }

        public Property SetFurnitureItems(List<CDatabaseStructureFurnitureItem> furnitureItems)
        {
            FurnitureItems = furnitureItems;
            return this;
        }

        public Property SetFurnitureRemovals(List<FurnitureRemoval> furnitureRemovals)
        {
            FurnitureRemovals = furnitureRemovals;
            return this;
        }

        public Property SetInventory(List<CItemInstanceDef> items)
        {
            Inventory = items;
            return this;
        }

        public void SetEntrance(Vector3 entrancePosition, float entranceRotation, uint entranceDimension)
        {
            EntrancePosition = entrancePosition;
            EntranceRotation = entranceRotation;
            EntranceDimension = entranceDimension;
            Functions.Properties.Update(this, () => {});
        }

        public void SetExit(Vector3 exitPosition, float exitRotation)
        {
            ExitPosition = exitPosition;
            ExitRotation = exitRotation;
            Functions.Properties.Update(this, () => {});
        }

        public void Purchase(EPropertyState state, bool locked, EPropertyOwnerType ownerType, int paymentsRemaining,
            int paymentsMade, int paymentsMissed, float creditAmount, EntityDatabaseID ownerID, 
            bool isTokenPurchase = false)
        {
            State = state;
            Locked = locked;
            OwnerType = ownerType;
            PaymentsRemaining = paymentsRemaining;
            PaymentsMade = paymentsMade;
            PaymentsMissed = paymentsMissed;
            CreditAmount = creditAmount;
            OwnerId = ownerID;
            IsTokenPurchase = isTokenPurchase;
            LastUsed = Helpers.GetUnixTimestamp();
            Functions.Properties.Update(this, () => {});
        }

        public void SetOwner(EPropertyOwnerType ownerType, EntityDatabaseID ownerId, bool resetCredit = false, 
            bool isTokenPurchase = DEFAULT_IS_TOKEN_PURCHASE)
        {
            OwnerId = ownerId;
            OwnerType = ownerType;
            IsTokenPurchase = isTokenPurchase;
            State = State == EPropertyState.AvailableToBuy_AlwaysEnterable 
                ? EPropertyState.Owned_AlwaysEnterable 
                : EPropertyState.Owned;
            Locked = State == EPropertyState.Owned;
            LastUsed = Helpers.GetUnixTimestamp();

            if (resetCredit)
            {
                CreditAmount = DEFAULT_CREDIT_AMOUNT;
                PaymentsMade = DEFAULT_PAYMENTS_MADE;
                PaymentsMissed = DEFAULT_PAYMENTS_MISSED;
                PaymentsRemaining = DEFAULT_PAYMENTS_REMAINING;
            }

            Functions.Properties.Update(this, () => {});
        }

        public void SetOwnerWithCredit(EPropertyOwnerType ownerType, EntityDatabaseID ownerId, int numberOfPayments, 
            float creditAmount)
        {
            OwnerId = ownerId;
            OwnerType = ownerType;
            IsTokenPurchase = DEFAULT_IS_TOKEN_PURCHASE;
            State = State == EPropertyState.AvailableToBuy_AlwaysEnterable 
                ? EPropertyState.Owned_AlwaysEnterable 
                : EPropertyState.Owned;
            Locked = State == EPropertyState.Owned;
            CreditAmount = creditAmount;
            PaymentsMade = DEFAULT_PAYMENTS_MADE;
            PaymentsMissed = DEFAULT_PAYMENTS_MISSED;
            PaymentsRemaining = numberOfPayments;
        }

        public void SetRenter(EPropertyOwnerType renterType, EntityDatabaseID renterId)
        {
            RenterId = renterId;
            RenterType = renterType;
            IsTokenPurchase = DEFAULT_IS_TOKEN_PURCHASE;
            State = State == EPropertyState.AvailableToRent_AlwaysEnterable 
                ? EPropertyState.Rented_AlwaysEnterable 
                : EPropertyState.Rented;
        }

        public void Repossess()
        {
            State = DEFAULT_PROPERTY_STATE;
            OwnerId = DEFAULT_OWNER_ID;
            RenterId = DEFAULT_RENTER_ID;
            IsTokenPurchase = DEFAULT_IS_TOKEN_PURCHASE;
            PaymentsRemaining = DEFAULT_PAYMENTS_REMAINING;
            PaymentsMade = DEFAULT_PAYMENTS_MADE;
            PaymentsMissed = DEFAULT_PAYMENTS_MISSED;
            Locked = DEFAULT_LOCKED_STATE;
            OwnerType = DEFAULT_OWNER_TYPE;
            RenterType = DEFAULT_RENTER_TYPE;
            
            Functions.Properties.Update(this, () => {});
        }

        public void MakePayment()
        {
            PaymentsRemaining--;
            PaymentsMade++;
            PaymentsMissed = 0;
            Functions.Properties.Update(this, () => {});
        }

        public void MissPayment()
        {
            PaymentsMissed++;
            Functions.Properties.Update(this, () => {});
        }
        
        public void DecreaseCreditAmount(float amount)
        {
            CreditAmount -= amount;
                
            Functions.Properties.Update(this, () => {});
        }
        
        public void ClearCreditAmount()
        {
            CreditAmount = DEFAULT_CREDIT_AMOUNT;
            PaymentsMade = DEFAULT_PAYMENTS_MADE;
            PaymentsRemaining = DEFAULT_PAYMENTS_REMAINING;
            PaymentsMissed = DEFAULT_PAYMENTS_MISSED;
                
            Functions.Properties.Update(this, () => {});
        }

        public void MarkAsUsed()
        {
            LastUsed = Helpers.GetUnixTimestamp();
            Functions.Properties.Update(this, () => {});
        }

        public void SetState(EPropertyState state)
        {
            State = state;
            Functions.Properties.Update(this, () => {});
        }

        public void SetBuyPrice(float buyPrice)
        {
            BuyPrice = buyPrice;
            Functions.Properties.Update(this, () => {});
        }

        public void SetRentPrice(float rentPrice)
        {
            RentPrice = rentPrice;
            Functions.Properties.Update(this, () => {});
        }

        public void SetInterior(int newIntId, Vector3 newExitPos)
        {
            InteriorId = newIntId;
            ExitPosition = newExitPos;
            Functions.Properties.Update(this, () => {});
        }

        public void SetLocked(bool locked)
        {
            Locked = locked;
            Functions.Properties.Update(this, () => {});
        }

        public void SetName(string name)
        {
            Name = name;
            Functions.Properties.Update(this, () => {});
        }

        public void Delete(Action callback)
        {
            Functions.Properties.Delete(this, callback);
        }

        public bool IsAvailable()
        {
            EPropertyState[] availableStates = {
                EPropertyState.AvailableToBuy,
                EPropertyState.AvailableToRent,
                EPropertyState.AvailableToBuy_AlwaysEnterable,
                EPropertyState.AvailableToRent_AlwaysEnterable
            };

            return availableStates.Contains(State);
        }

        public bool IsAlwaysEnterable()
        {
            EPropertyState[] alwaysEnterableStates = {
                EPropertyState.AvailableToBuy_AlwaysEnterable,
                EPropertyState.AvailableToRent_AlwaysEnterable,
                EPropertyState.Owned_AlwaysEnterable,
                EPropertyState.Rented_AlwaysEnterable
            };

            return alwaysEnterableStates.Contains(State);
        }
        
        public bool CanBeRented()
        {
            EPropertyState[] rentableStates = {
                EPropertyState.AvailableToRent,
                EPropertyState.AvailableToRent_AlwaysEnterable,
                EPropertyState.Rented,
                EPropertyState.Rented_AlwaysEnterable
            };

            return rentableStates.Contains(State);
        }
        
        public Property AddXP(uint uXP)
        {
            XP += uXP;
            Functions.Properties.Update(this, () => {});
            return this;
        }

        public Property SetMowed()
        {
            XP += TOTAL_MOWING_XP;
            LastMowedAt = (ulong)Helpers.GetUnixTimestamp(true);
            Functions.Properties.Update(this, () => {});

            return this;
        }

        public Property AddFurnitureRemoval(FurnitureRemoval furnitureRemoval)
        {
            FurnitureRemovals.Add(furnitureRemoval);
            return this;
        }

        public void DecrementWeeklyXP(uint amountToDecrement)
		{
            if (XP == 0)
			{
				return;
			}
			uint currentXP = XP;
			if (amountToDecrement > XP)
			{
				amountToDecrement = XP;
			}

			XP -= amountToDecrement;

			if (XP != currentXP)
			{
                Functions.Properties.Update(this, () => { });
            }
        }

        public static Property FromDB(CMySQLRow row)
        {
            return new Property(
                row.GetValue<EntityDatabaseID>("id"),
                new Vector3(float.Parse(row["entrance_x"]), float.Parse(row["entrance_y"]), float.Parse(row["entrance_z"])),
                new Vector3(float.Parse(row["exit_x"]), float.Parse(row["exit_y"]), float.Parse(row["exit_z"])),
                (EPropertyState)row.GetValue<int>("state"),
                row.GetValue<float>("buy_price"),
                row.GetValue<float>("rent_price"),
                row.GetValue<EntityDatabaseID>("owner"),
                row.GetValue<EntityDatabaseID>("renter"),
                row.GetValue<bool>("locked"),
                (EPropertyOwnerType)row.GetValue<int>("owner_type"),
                (EPropertyOwnerType)row.GetValue<int>("renter_type"),
                row.GetValue<string>("name"),
                row.GetValue<float>("entrance_rot"),
                row.GetValue<float>("exit_rot"),
                row.GetValue<uint>("entrance_dimension"),
                row.GetValue<int>("interior_id"),
                row.GetValue<int>("payments_remaining"),
                row.GetValue<int>("payments_made"),
                row.GetValue<int>("payments_missed"),
                row.GetValue<float>("credit_amount"),
                (EPropertyEntranceType)row.GetValue<int>("entrance_type"),
                row.GetValue<Int64>("last_used"),
                new List<CDatabaseStructureFurnitureItem>(),
                new List<FurnitureRemoval>(),
                row.GetValue<bool>("scripted_blip"),
                row.GetValue<bool>("is_token_purchase"),
                row.GetValue<uint>("xp"),
                row.GetValue<ulong>("last_mowed_at")
            );
        }

        private const EPropertyState DEFAULT_PROPERTY_STATE = EPropertyState.AvailableToBuy;
        private const float DEFAULT_RENT_PRICE = 0;
        private const EntityDatabaseID DEFAULT_OWNER_ID = -1;
        private const EntityDatabaseID DEFAULT_RENTER_ID = -1;
        private const bool DEFAULT_LOCKED_STATE = true;
        private const EPropertyOwnerType DEFAULT_OWNER_TYPE = EPropertyOwnerType.Player;
        private const EPropertyOwnerType DEFAULT_RENTER_TYPE = EPropertyOwnerType.Player;
        private const float DEFAULT_EXIT_ROTATION = 0;
        private const int DEFAULT_PAYMENTS_REMAINING = 0;
        private const int DEFAULT_PAYMENTS_MADE = 0;
        private const int DEFAULT_PAYMENTS_MISSED = 0;
        private const float DEFAULT_CREDIT_AMOUNT = 0.0f;
        private const EPropertyEntranceType DEFAULT_ENTRANCE_TYPE = EPropertyEntranceType.Normal;
        private const bool DEFAULT_IS_TOKEN_PURCHASE = false;
        private const uint DEFAULT_XP = 0;
        private const uint DEFAULT_LAST_MOWED_AT = 0;

        public static void Create(Vector3 entrancePosition, Vector3 exitPosition, float buyPrice, string name, 
            float entranceRotation, uint entranceDimension, 
            List<FurnitureRemoval> furnitureRemovals, int interiorId, bool hasScriptedBlip,
            Action<Property> callback)
        {
            Property model = new Property(
                0,
                entrancePosition,
                exitPosition,
                DEFAULT_PROPERTY_STATE,
                buyPrice,
                DEFAULT_RENT_PRICE,
                DEFAULT_OWNER_ID,
                DEFAULT_RENTER_ID,
                DEFAULT_LOCKED_STATE,
                DEFAULT_OWNER_TYPE,
                DEFAULT_RENTER_TYPE,
                name,
                entranceRotation,
                DEFAULT_EXIT_ROTATION,
                entranceDimension,
                interiorId,
                DEFAULT_PAYMENTS_REMAINING,
                DEFAULT_PAYMENTS_MADE,
                DEFAULT_PAYMENTS_MISSED,
                DEFAULT_CREDIT_AMOUNT,
                DEFAULT_ENTRANCE_TYPE,
                Helpers.GetUnixTimestamp(),
                new List<CDatabaseStructureFurnitureItem>(),
                new List<FurnitureRemoval>(),
                hasScriptedBlip,
                DEFAULT_IS_TOKEN_PURCHASE,
                DEFAULT_XP,
                DEFAULT_LAST_MOWED_AT
            );
            Functions.Properties.Create(model, property =>
            {
                foreach (FurnitureRemoval furnitureRemoval in furnitureRemovals)
                {
                    furnitureRemoval.AddPropertyId(property.Id);
                    furnitureRemoval.Save(result =>
                    {
                        //
                    });
                    property.AddFurnitureRemoval(furnitureRemoval);
                }

                callback(property);
            });
        }
    }
}
