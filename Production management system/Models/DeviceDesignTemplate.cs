using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductionManagementSystem.Models
{
    public class DeviceDesignTemplate
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Design Design { get; set; }
        public string Description { get; set; }

    }
}
