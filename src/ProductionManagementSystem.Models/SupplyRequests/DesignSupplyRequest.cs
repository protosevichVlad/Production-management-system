using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.SupplyRequests
{
    [Table("DesignsSupplyRequests")]
    public class DesignSupplyRequest : SupplyRequest
    {
        [Display(Name = "Контструктив")]
        [NotMapped]
        public Design Design { get; set; }
    }
}