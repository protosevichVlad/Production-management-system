using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Срок")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Deadline { get; set; }
        
        [Display(Name = "Прибор")]
        public int DeviceId { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        [Display(Name = "Статус")]
        public string Status { get; set; }

        public Device Device { get; set; }

        public List<DeviceDesignTemplate> DesignTemplate { get; set; }
        public List<DeviceComponentsTemplate> ComponentTemplate { get; set; }
        public List<ObtainedСomponent> ObtainedComponents { get; set; }
        public List<ObtainedDesign> ObtainedDesigns { get; set; }
    }
}