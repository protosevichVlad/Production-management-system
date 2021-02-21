using System;
using System.ComponentModel.DataAnnotations;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.ViewModels
{
    public class OrderViewModel
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
        
        [Display(Name = "Статус")]
        public string Status { get; set; }
        public int[] DeviceIds { get; set; }
        public int[] DeviceQuantity { get; set; }
        public string[] DeviceDescriptions { get; set; }
    }
}