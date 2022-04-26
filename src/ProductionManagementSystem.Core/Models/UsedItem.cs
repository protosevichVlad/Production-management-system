using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models
{
    [Table("UsedItems")]
    public class UsedItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public CDBItemType ItemType { get; set; }
        [NotMapped]
        public UniversalItem Item { get; set; }
        public int InItemId { get; set; }
        public CDBItemType InItemType { get; set; }
        [NotMapped]
        public UniversalItem InItem { get; set; }
        public string Designator { get; set; }
        public int Quantity { get; set; }
    }
}