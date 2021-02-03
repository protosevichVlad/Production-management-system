using System;

namespace ProductionManagementSystem.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string UserLogin { get; set; }
        public string Message { get; set; }
    }
}