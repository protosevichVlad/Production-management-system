using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.WEB.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Срок")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Deadline { get; set; }
        
        public DateTime StartTime { get; set; }
        
        [Display(Name = "Прибор")]
        public int DeviceId { get; set; }
        public DeviceViewModel Device { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        [Display(Name = "Статус")]
        public string Status { get; set; }

        public int? OrderId { get; set; }
        
        public List<ObtainedDesign> ObtainedDesigns { get; set; }
        public List<ObtainedComponent> ObtainedComponents { get; set; }
    }
}