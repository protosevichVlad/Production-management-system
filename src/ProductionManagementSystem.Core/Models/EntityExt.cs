using System.Collections.Generic;
using System.Linq;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Models
{
    public class EntityExt : AltiumDbEntity
    {
        public EntityExt(Table table) : base(table)
        {
            Table = table;
            TableId = table.Id;
            KeyId = 0;
            Quantity = 0;
            ImageUrl = "";
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

        public int KeyId 
        {
            get => int.Parse(this["KeyId"]);
            set => this["KeyId"] = value.ToString();
        }

        public int Quantity
        {
            get => int.Parse(this["Quantity"]);
            set => this["Quantity"] = value.ToString();
        }

        public string ImageUrl
        {
            get => this["ImageUrl"];
            set => this["ImageUrl"] = value;
        }
        public int TableId 
        { 
            get => int.Parse(this["TableId"]);
            set => this["TableId"] = value.ToString(); }
        
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
                Table = Table,
                ImageUrl = ImageUrl
            };
        }
    }
}