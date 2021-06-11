using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.WEB.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Срок")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
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
        public List<TaskViewModel> Tasks { get; set; }
    }
}