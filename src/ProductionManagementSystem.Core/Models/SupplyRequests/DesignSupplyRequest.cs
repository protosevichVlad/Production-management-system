using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.SupplyRequests
{
    [Table("DesignsSupplyRequests")]
    public class DesignSupplyRequest : SupplyRequest
    {
        [Display(Name = "Контструктив")]
        public Design Design { get; set; }
    }
}