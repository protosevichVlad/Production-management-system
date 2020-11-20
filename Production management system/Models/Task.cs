using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductionManagementSystem.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public List<DeviceInTask> DevicesInTask { get; set; }
    }
}
