using System;

namespace ProductionManagementSystem.BLL.DTO
{
    public class LogDTO
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string UserLogin { get; set; }
        public string Message { get; set; }
        public int? ComponentId { get; set; }
        public int? DesignId { get; set; }
        public int? DeviceId { get; set; }
        public int? TaskId { get; set; }
        public int? OrderId { get; set; }
    }
}