using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Display(Name = "Срок")]
        public DateTime Deadline { get; set; }
        
        [Display(Name = "Дата заказа")]
        public DateTime DateStart { get; set; }
        
        [Display(Name = "Количество")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Поле Количество должно быть больше 0")]
        [Required]
        public int Quantity { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        public Task Task { get; set; }
    }
}