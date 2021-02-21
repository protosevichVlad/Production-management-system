using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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
        
        public DateTime StartTime { get; set; }
        
        [Display(Name = "Прибор")]
        public int DeviceId { get; set; }
        
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        [Display(Name = "Статус")]
        public string Status { get; set; }

        public Device Device { get; set; }
        public int? OrderId { get; set; }

        public List<DeviceDesignTemplate> DesignTemplate { get; set; }
        public List<DeviceComponentsTemplate> ComponentTemplate { get; set; }
        public List<ObtainedСomponent> ObtainedComponents { get; set; }
        public List<ObtainedDesign> ObtainedDesigns { get; set; }

        public TaskViewModel() {}
        public TaskViewModel(Task task)
        {
            Deadline = task.Deadline;
            Description = task.Description;
            Id = task.Id;
            Status = GetDisplayName(task.Status);
            Device = task.Device;
            StartTime = task.StartTime;
            OrderId = task.OrderId;
        }
        
        private string GetDisplayName(StatusEnum item)
        {
            List<string> result = new List<string>();
            foreach( StatusEnum foo in Enum.GetValues(typeof(StatusEnum)) )
            {
                if ((item & foo) == foo)
                {
                    result.Add(foo.GetType()
                        .GetMember(foo.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName());
                }
            }

            return string.Join(", ", result);
        }
    }
}