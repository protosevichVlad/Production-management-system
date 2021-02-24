using System;
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
        public Device Device { get; set; }
        public int DeviceId { get; set; }

        public int? OrderId { get; set; }
        public Order Order { get; set; }
    }
}