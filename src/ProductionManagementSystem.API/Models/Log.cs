using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.API.Models
{
    public class Log
    {
        public int Id { get; set; }
        
        [Display(Name = "Дата и время")]
        public DateTime DateTime { get; set; }
        
        [Display(Name = "Пользователь")]
        public string UserLogin { get; set; }
        
        [Display(Name = "Сообщение")]
        public string Message { get; set; }

        public int? ComponentId { get; set; }
        public int? DesignId { get; set; }
        public int? DeviceId { get; set; }
        public int? TaskId { get; set; }
        public int? OrderId { get; set; }
    }
}