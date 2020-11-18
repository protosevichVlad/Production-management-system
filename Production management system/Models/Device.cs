using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public List<DeviceComponentsTemplate> DeviceComponentsTemplate { get; set; }
        public List<DeviceDesignTemplate> DeviceDesignTemplate { get; set; }

    }
}
