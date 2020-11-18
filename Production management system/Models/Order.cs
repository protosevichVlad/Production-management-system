using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductionManagementSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Device Device { get; set; }
        public int QuantityDevice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
    }
}
