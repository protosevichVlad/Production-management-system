using System.Linq;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Models
{
    public class EntityExt : AltiumDbEntity
    {
        public EntityExt(Table table) : base(table)
        {
        }
        
        public EntityExt(AltiumDbEntity altiumDbEntity, Entity entity)
        {
            this.Quantity = entity.Quantity;
            this.Table = entity.Table;
            this.KeyId = entity.KeyId;
            this.ImageUrl = entity.ImageUrl;
            this.TableId = entity.TableId;
            foreach (var pair in altiumDbEntity)
            {
                this[pair.Key] = pair.Value;
            }
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