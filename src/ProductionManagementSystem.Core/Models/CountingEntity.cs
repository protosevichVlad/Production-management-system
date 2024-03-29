﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Core.Models
{
    public class CountingEntity : BaseEntity
    {
        [Display(Name = "Наименование")]
        [Required]
        public string Name { get; set; }
        
        [Display(Name = "Кол-во")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        [Required]
        public int Quantity { get; set; }
    }
}