using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Devices;

namespace ProductionManagementSystem.Core.Models.Components
{
    public class ComponentBase : CountingEntity
    {
        [Display(Name = "Конструктивная единица")]
        public string Type { get; set; }
        
        [NotMapped]
        [Display(Name = "Используется в")]
        public List<Device> Devices { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
