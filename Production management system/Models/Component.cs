using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models
{
    public class Component
    {
        [Display(Name = "Уникальный номер")]
        public int Id { get; set; }
        
        [Display(Name = "Тип")]

        public string Type { get; set; }

        [Display(Name = "Наименование")]
        [Required]
        public string Name { get; set; }
        
        [Display(Name = "Номинал")]
        public string Nominal { get; set; }
        
        [Display(Name = "Корпус")]
        public string Corpus { get; set; }
        
        [Display(Name = "Пояснение")]
        public string Explanation { get; set; }

        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }

        [Display(Name = "Количество")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Поле Количество должно быть больше 0")]
        [Required]
        public int Quantity { get; set; }
    }
}
