using System.Collections.Generic;
using System.Linq;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Models
{
    public class EntityExt : Entity
    {
        public EntityExt(Table table) : base(table)
        {
            Table = table;
            TableId = table.Id;
            KeyId = 0;
            Quantity = 0;
            ImageUrl = "";
        }
        
        public EntityExt(Entity entity, EntityDBModel entityDbModel)
        {
            this.Quantity = entityDbModel.Quantity;
            this.Table = entityDbModel.Table;
            this.KeyId = entityDbModel.KeyId;
            this.ImageUrl = entityDbModel.ImageUrl;
            this.TableId = entityDbModel.TableId;
            foreach (var pair in entity)
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
        
        

        public Entity GetAltiumDbEntity()
        {
            return this;
        }
        
        public EntityDBModel GetEntity()
        {
            return new EntityDBModel()
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