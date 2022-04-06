using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Models
{
    public class EntityExt : AltiumDbEntity
    {
        public EntityExt(Table table) : base(table)
        {
        }

        public EntityExt()
        {
        }

        public int KeyId { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }

        public AltiumDbEntity GetAltiumDbEntity()
        {
            return this;
        }
        
        public Entity GetEntity()
        {
            return new Entity()
            {
                KeyId = KeyId,
                Quantity = Quantity,
                TableId = TableId,
                PartNumber = PartNumber,
                Table = Table
            };
        }
    }
}