using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Models.Components
{
    [Table("Components")]
    public class Montage : ComponentBase
    {
        public string Nominal { get; set; }
        public string Corpus { get; set; }
        public string Explanation { get; set; }
        public string Manufacturer { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name} {Nominal}";
        }
    }
}
