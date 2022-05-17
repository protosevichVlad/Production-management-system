using System.Collections.Generic;

namespace ProductionManagementSystem.WEB.Models.Tasks
{
    public class TaskItemViewModel
    {
        public int Index { get; set; }
        public int DeviceId { get; set; }
        public int Quantity { get; set; }
        public List<Core.Models.Devices.Device> AllDevices { get; set; }
        public Core.Models.Devices.Device Device { get; set; }
    }
}