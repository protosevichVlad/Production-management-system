using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models
{
    public class Component: BaseComponentAndDesign
    {
        [Display(Name = "Номинал")]
        public string Nominal { get; set; }
        
        [Display(Name = "Корпус")]
        public string Corpus { get; set; }
        
        [Display(Name = "Пояснение")]
        public string Explanation { get; set; }

        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }
    }
}
