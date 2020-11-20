using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductionManagementSystem.Models
{
    public class DeviceInTask
    {
        public int Id { get; set; }
        public Device Device { get; set; }
        public int Quantity { get; set; }
    }
}
