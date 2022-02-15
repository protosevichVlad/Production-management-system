using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Devices;
using ProductionManagementSystem.Core.Models.Orders;

namespace ProductionManagementSystem.Core.Models.Tasks
{
    [Table("Tasks")]
    public class Task : BaseEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        [Display(Name = "Срок")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Deadline { get; set; }
        
        [Display(Name = "Описание")]
        public TaskStatusEnum Status { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        [Display(Name = "Прибор")]
        public Device Device { get; set; }
        public int DeviceId { get; set; }

        public Order Order { get; set; }
        public int? OrderId { get; set; }

        [NotMapped]
        public IEnumerable<ObtainedDesign> ObtainedDesigns { get; set; }
        [NotMapped]
        public IEnumerable<ObtainedMontage> ObtainedMontages { get; set; }

        public Task()
        {
            this.StartTime = DateTime.Now;
            this.Status = TaskStatusEnum.Equipment;
        }

        public override string ToString()
        {
            return $"№{this.Id}";
        }
    }
}