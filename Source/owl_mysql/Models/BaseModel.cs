using EntityDatabaseID = System.Int64;

namespace Database.Models
{
    public class BaseModel
    {
        protected const EntityDatabaseID NO_ID = 0;

        public long Id { get; protected set; }

        public void SetId(EntityDatabaseID id)
        {
            Id = id;
        }
    }
}