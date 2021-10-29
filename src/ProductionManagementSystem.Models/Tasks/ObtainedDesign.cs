using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.Tasks
{
    [Table("ObtainedDesigns")]
    public class ObtainedDesign : ObtainedBase
    {
        [NotMapped]
        public Design Design { get; set; }
    }
}