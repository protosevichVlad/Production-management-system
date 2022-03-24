using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Core.Models
{
    public class BaseEntity
    {
        [Display(Name = "№")]
        public int Id { get; set; }
    }
}