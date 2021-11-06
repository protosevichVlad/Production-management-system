using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Models.Components
{
    [Table("Components")]
    public class Montage : ComponentBase
    {
        [Display(Name = "Номинал")]
        public string Nominal { get; set; }
        
        [Display(Name = "Корпус")]
        public string Corpus { get; set; }
        
        [Display(Name = "Пояснение")]
        public string Explanation { get; set; }
        
        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name} {Nominal}";
        }
    }
}
