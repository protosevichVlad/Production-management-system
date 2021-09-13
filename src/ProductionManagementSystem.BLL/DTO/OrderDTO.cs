using System;
using System.Collections.Generic;

namespace ProductionManagementSystem.BLL.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        
        public DateTime Deadline { get; set; }
        
        public DateTime DateStart { get; set; }
        
        public string Customer { get; set; }
        
        public string Description { get; set; }
        public string Status { get; set; }

        public int[] DeviceIds { get; set; }
        public int[] DeviceQuantity { get; set; }
        public string[] DeviceDescriptions { get; set; }
        public string[] DeviceName { get; set; }
        public List<TaskDTO> Tasks { get; set; }
    }
}