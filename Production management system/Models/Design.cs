using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models
{
    public class Design
    {
        [Display(Name = "Уникальный номер")]
        public int Id { get; set; }

        [Display(Name = "Конструктивная единица")]
        public string Type { get; set; }
        
        [Display(Name = "Наименовие")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Количество")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Поле Количество должно быть больше 0")]
        [Required]
        public int Quantity { get; set; }
        
        [Display(Name = "Краткое описание")]
        [Required]
        public string ShortDescription { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Type} {Name} {ShortDescription}";
        }
    }
}
