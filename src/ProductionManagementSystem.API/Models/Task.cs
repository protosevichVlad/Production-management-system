using System;
using System.Collections.Generic;

namespace ProductionManagementSystem.API.Models
{
    public class Task
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        
        public int DeviceId { get; set; }

        public int? OrderId { get; set; }

        public List<ObtainedDesign> ObtainedDesigns { get; set; }
        public List<ObtainedComponent> ObtainedComponents { get; set; }
    }
}
