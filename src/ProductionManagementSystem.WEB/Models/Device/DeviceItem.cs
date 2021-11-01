using System.Collections.Generic;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.WEB.Models.Device
{
    public class DeviceItem<TComponent> where TComponent : ComponentBase
    {
        public int Id { get; set; }
        public int SelectedComponentId { get; set; }
        public string ComponentType => typeof(TComponent).Name + "s";
        public IEnumerable<TComponent> AllComponents { get; set; }
        public ComponentBaseInDevice<TComponent> Component { get; set; }
    }
}