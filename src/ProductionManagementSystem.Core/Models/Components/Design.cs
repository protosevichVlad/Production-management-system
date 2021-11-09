using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.Components
{
    [Table("Designs")]
    public class Design : ComponentBase
    {
        [Display(Name = "Краткое описание")]
        public string ShortDescription { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name} {ShortDescription}";
        }
    }
}
