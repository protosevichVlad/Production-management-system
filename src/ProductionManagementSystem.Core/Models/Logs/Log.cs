﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Users;

namespace ProductionManagementSystem.Core.Models.Logs
{
    [Table("Logs")]
    public class Log : BaseEntity
    {
        
        [Display(Name = "Дата и время")]
        public DateTime DateTime { get; set; }
        
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
        
        public User User { get; set; }        
        public string UserId { get; set; }
        
        public int? MontageId { get; set; }
        public int? DesignId { get; set; }
        public int? DeviceId { get; set; }
        public int? TaskId { get; set; }
        public int? OrderId { get; set; }
        public int? MontageSupplyRequestId { get; set; }
        public int? DesignSupplyRequestId { get; set; }
        
        public Log()
        {
            DateTime = DateTime.Now;
        }
    }
}