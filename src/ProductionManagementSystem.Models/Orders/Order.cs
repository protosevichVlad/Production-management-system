using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Tasks;

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
        
        [NotMapped]
        public List<Task> Tasks { get; set; }
    }
}