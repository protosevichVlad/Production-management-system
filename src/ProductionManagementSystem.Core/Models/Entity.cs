using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Models
{
    [Table("Entities")]
    public class Entity
    {
        [Key]
        public int KeyId { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string PartNumber { get; set; }
        public int TableId { get; set; }
        public Table Table { get; set; }
    }
}