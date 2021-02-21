using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.DAL.Entities
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
    }
}