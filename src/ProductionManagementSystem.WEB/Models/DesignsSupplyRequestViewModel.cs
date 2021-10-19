using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.WEB.Models
{
    public class DesignsSupplyRequestViewModel
    {
        [Display(Name = "№")]
        public int Id { get; set; }
        
        [Display(Name = "Задача №")]
        public int? TaskId { get; set; }
        public TaskViewModel Task { get; set; }
        [Display(Name = "Монтаж")]
        public int DesignId { get; set; }
        public DesignViewModel Design { get; set; }
        
        [Display(Name = "Пользователь")]
        public UserViewModel User { get; set; }
        
        [Display(Name = "Дата добавления")]
        public DateTime DateAdded { get; set; }
        
        [Display(Name = "Желаемая дата")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DesiredDate { get; set; }
        
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        
        [Display(Name = "Описание")]
        public string Comment { get; set; }
        
        [Display(Name = "Статус")]
        public StatusSupplyEnum StatusSupply { get; set; }
    }
}