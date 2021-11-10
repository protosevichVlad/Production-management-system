using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Tasks;

namespace ProductionManagementSystem.Core.Models.Orders
{
    [Table("Orders")]
    public class Order : BaseEntity
    {
        [Display(Name = "Срок")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Deadline { get; set; }
        
        [Display(Name = "Дата заказа")]
        public DateTime DateStart { get; set; }
        
        [Display(Name = "Заказчик")]
        public string Customer { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        [NotMapped]
        public List<Task> Tasks { get; set; }
        
        [Display(Name="Статус")]
        [NotMapped]
        public string Status { get; set; }

        public override string ToString()
        {
            return $"№{Id}";
        }
    }
}