using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.SupplyRequests
{
    [Table("ComponentsSupplyRequests")]
    public class MontageSupplyRequest : SupplyRequest
    {
        [Display(Name = "Монтаж")]
        [NotMapped]
        public Montage Montage { get; set; }
    }
}