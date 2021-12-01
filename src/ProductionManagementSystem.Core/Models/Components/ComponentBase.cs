using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Core.Models.Components
{
    public class ComponentBase : CountingEntity
    {
        [Display(Name = "Конструктивная единица")]
        public string Type { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
