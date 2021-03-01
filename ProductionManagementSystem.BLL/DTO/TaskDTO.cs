using System;
using System.Collections.Generic;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.BLL.DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime Deadline { get; set; }
        public StatusEnum Status { get; set; }
        public string Description { get; set; }
        public int DeviceId { get; set; }
        public DeviceDTO Device { get; set; }
        public int? OrderId { get; set; }
        public List<ObtainedDesign> ObtainedDesigns { get; set; }
        public List<ObtainedComponent> ObtainedComponents { get; set; }
    }
}