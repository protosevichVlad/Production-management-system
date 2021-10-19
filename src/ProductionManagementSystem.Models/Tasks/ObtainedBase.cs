using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.Tasks
{
    public abstract class ObtainedBase<TComponent> where TComponent : ComponentBase
    {
        public int Id { get; set; }
        public int Obtained { get; set; }

        [NotMapped]
        public Task Task { get; set; }
        public int TaskId { get; set; }

        
        [NotMapped]
        public ComponentBase Component { get; set; }
        public int ComponentId { get; set; }
    }
}