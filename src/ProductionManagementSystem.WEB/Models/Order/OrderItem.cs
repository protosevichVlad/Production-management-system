using System.Collections.Generic;

namespace ProductionManagementSystem.WEB.Models.Order
{
    public class OrderItem
    {
        public int Index { get; set; }
        public int Quantity { get; set; } = 0;
        public string Description { get; set; } = "";
        public Core.Models.Devices.Device Device { get; set; }
        public IEnumerable<Core.Models.Devices.Device> AllDevices { get; set; }
    }
}