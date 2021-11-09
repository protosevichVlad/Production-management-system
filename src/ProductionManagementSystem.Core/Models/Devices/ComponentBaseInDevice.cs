using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.Devices
{
    public abstract class ComponentBaseInDevice<TComponent> where TComponent : ComponentBase
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int DeviceId { get; set; }
        public string Description { get; set; }
        
        [NotMapped]
        public TComponent Component { get; set; }
        public int ComponentId { get; set; }
        
    }
}