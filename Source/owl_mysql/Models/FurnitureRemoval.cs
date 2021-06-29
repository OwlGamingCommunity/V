using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;
using GTANetworkAPI;

namespace Database.Models
{
    public class FurnitureRemoval : BaseModel
    {
        public uint Model { get; private set; }
        public Vector3 Position { get; private set; }
        public EntityDatabaseID DroppedById { get; private set; }
        public EntityDatabaseID PropertyId { get; private set; }

        public FurnitureRemoval(EntityDatabaseID id, uint model, Vector3 position, EntityDatabaseID droppedById = 0,
            EntityDatabaseID propertyId = 0)
        {
            Id = id;
            Model = model;
            Position = position;
            DroppedById = droppedById;
            PropertyId = propertyId;
        }

        public void AddPropertyId(EntityDatabaseID propertyId)
        {
            PropertyId = propertyId;
        }
        
        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                {"x", Position.X},
                {"y", Position.Y},
                {"z", Position.Z},
                {"item_value", "{}"},
                {"item_id", Model},
                {"dropped_by", DroppedById},
                {"dimension", PropertyId},
                {"parent_type", EItemParentTypes.DefaultFurnitureRemoval},
                {"parent", PropertyId},
            };
        }
        
        public static FurnitureRemoval FromDB(CMySQLRow row)
        {
            return new FurnitureRemoval(
                row.GetValue<EntityDatabaseID>("id"),
                row.GetValue<uint>("item_id"),
                new Vector3(
                    row.GetValue<float>("x"),
                    row.GetValue<float>("y"),
                    row.GetValue<float>("z")
                ),
                row.GetValue<long>("dropped_by"),
                row.GetValue<EntityDatabaseID>("parent")
            );
        }

        public static void Create(uint model, Vector3 position, EntityDatabaseID droppedById,
            EntityDatabaseID propertyId, Action<FurnitureRemoval> callback)
        {
            FurnitureRemoval dbModel = new FurnitureRemoval(0, model, position, droppedById, propertyId);
            dbModel.Save(callback);
        }

        public void Save(Action<FurnitureRemoval> callback = null)
        {
            if (Id == NO_ID)
            {
                Functions.Furniture.CreateRemoval(this, callback);
            }
            else
            {
                Functions.Furniture.UpdateRemoval(this, callback);
            }
        }
    }
}