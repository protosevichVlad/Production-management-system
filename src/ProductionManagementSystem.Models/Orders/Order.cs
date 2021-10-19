using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models.Orders
{
    public class Order
    {
        public int Id { get; set; }
        
        [Display(Name = "Срок")]
        public DateTime Deadline { get; set; }
        
        [Display(Name = "Дата заказа")]
        public DateTime DateStart { get; set; }
        
        [Display(Name = "Заказчик")]
        public string Customer { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}