using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models
{
    public class Design
    {
        [Display(Name = "Уникальный номер")]
        public int Id { get; set; }

        [Display(Name = "Конструктивная единица")]
        public string Type { get; set; }
        
        [Display(Name = "Наименовие")]
        public string Name { get; set; }

        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        
    }
}
