using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.Tasks
{
    [Table("ObtainedDesigns")]
    public class ObtainedDesign : ObtainedBase
    {
        public Design Design { get; set; }
    }
}