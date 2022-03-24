using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.SupplyRequests
{
    [Table("ComponentsSupplyRequests")]
    public class MontageSupplyRequest : SupplyRequest
    {
        [Display(Name = "Монтаж")]
        public Montage Montage { get; set; }
    }
}