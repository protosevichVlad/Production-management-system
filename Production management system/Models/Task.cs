using System;
using System.Collections.Generic;

namespace ProductionManagementSystem.Models
{
    public class Task
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Device Device { get; set; }
    }
}
