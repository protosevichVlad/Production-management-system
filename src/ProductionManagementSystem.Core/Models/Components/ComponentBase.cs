using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Core.Models.Components
{
    public class ComponentBase : BaseEntity
    {
        [Display(Name = "Конструктивная единица")]
        public string Type { get; set; }
        
        [Display(Name = "Наименование")]
        [Required]
        public string Name { get; set; }
        
        [Display(Name = "Количество")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        [Required]
        public int Quantity { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
