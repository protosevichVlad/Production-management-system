using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.Tasks
{
    public class ObtainedMontage : ObtainedBase
    {
        [NotMapped]
        public Montage Montage { get; set; }
    }
}