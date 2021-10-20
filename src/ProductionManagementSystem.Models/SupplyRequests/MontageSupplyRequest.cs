using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.SupplyRequests
{
    public class MontageSupplyRequest : SupplyRequest
    {
        [NotMapped]
        public Montage Montage { get; set; }
    }
}