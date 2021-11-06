using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Tasks;

namespace ProductionManagementSystem.Models.SupplyRequests
{
    public abstract class SupplyRequest 
    {
        [Display(Name = "№")]
        public int Id { get; set; }
        
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        
        [Display(Name = "Описание")]
        public string Comment { get; set; }
        
        [Display(Name = "Дата добавления")]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }
        
        [Display(Name = "Желаемая дата")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DesiredDate { get; set; }
        public SupplyStatusEnum StatusSupply { get; set; }
        
        
        [NotMapped]
        public Task Task { get; set; }
        
        [Display(Name = "Задача №")]
        public int? TaskId { get; set; }
        
        [NotMapped]
        public Users.User User { get; set; }
        [Display(Name = "Пользователь")]
        public string UserId { get; set; }
        
        public int ComponentId { get; set; }
    }
}