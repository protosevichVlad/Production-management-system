using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.SupplyRequests
{
    [Table("ComponentsSupplyRequests")]
    public class MontageSupplyRequest : SupplyRequest
    {
        [NotMapped]
        public Montage Montage { get; set; }
    }
}