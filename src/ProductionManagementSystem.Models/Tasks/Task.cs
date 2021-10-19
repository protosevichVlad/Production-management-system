using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Devices;
using ProductionManagementSystem.Models.Orders;

namespace ProductionManagementSystem.Models.Tasks
{
    public class Task
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Deadline { get; set; }
        public TaskStatusEnum Status { get; set; }
        public string Description { get; set; }
        
        [NotMapped]
        public Device Device { get; set; }
        public int DeviceId { get; set; }

        [NotMapped]
        public Order Order { get; set; }
        public int? OrderId { get; set; }

        [NotMapped]
        public IEnumerable<ObtainedDesign> ObtainedDesigns { get; set; }
        [NotMapped]
        public IEnumerable<ObtainedMontage> ObtainedMontages { get; set; }
    }
}