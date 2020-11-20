using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Models
{
    public class DeviceComponentsTemplate
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Component Component { get; set; }
        public string Description { get; set; }


    }
}
