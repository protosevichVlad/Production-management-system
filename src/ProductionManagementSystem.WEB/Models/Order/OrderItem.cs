using System.Collections.Generic;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.WEB.Models.Order
{
    public class OrderItem
    {
        public int Index { get; set; }
        public int Quantity { get; set; } = 0;
        public string Description { get; set; } = "";
        public ProductionManagementSystem.Models.Devices.Device Device { get; set; }
        public IEnumerable<ProductionManagementSystem.Models.Devices.Device> AllDevices { get; set; }
    }
}