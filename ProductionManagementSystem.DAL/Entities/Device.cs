using System.Collections.Generic;

namespace ProductionManagementSystem.DAL.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<DeviceComponentsTemplate> DeviceComponentsTemplate { get; set; }
        public List<DeviceDesignTemplate> DeviceDesignTemplate { get; set; }
    }
}
