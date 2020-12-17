using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models
{
    public class BaseComponentAndDesign
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
    }
}