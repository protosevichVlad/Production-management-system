using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ProductionManagementSystem.Core.Models.Components;
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
        
        public List<DevicesInTask> Devices { get; set; }

        public Order Order { get; set; }
        public int? OrderId { get; set; }

        [NotMapped]
        public IEnumerable<ObtainedDesign> ObtainedDesigns { get; set; }
        [NotMapped]
        public IEnumerable<ObtainedMontage> ObtainedMontages { get; set; }

        [NotMapped]
        public List<DesignInDevice> DesignInDevices
        {
            get {
                var result = new Dictionary<int, DesignInDevice>();
                if (Devices == null) return new List<DesignInDevice>();

                foreach (var devicesInTask in Devices)
                {
                    if (devicesInTask.Device == null) continue;
                    foreach (var designInDevice in devicesInTask.Device.Designs)
                    {
                        if (result.TryGetValue(designInDevice.ComponentId, out var resultDeviceInDevice))
                        {
                            result[designInDevice.ComponentId].Quantity += designInDevice.Quantity * devicesInTask.Quantity;
                        }
                        else
                        {
                            result.Add(designInDevice.ComponentId, new DesignInDevice()
                            {
                                Id = designInDevice.Id,
                                DeviceId = designInDevice.DeviceId,
                                ComponentId = designInDevice.ComponentId,
                                Quantity = designInDevice.Quantity * devicesInTask.Quantity,
                                Description = designInDevice.Description,
                                Design = designInDevice.Design,
                            });
                        }
                    }
                }
                    
                return result.Select(x => x.Value).ToList();
            } 
        }
        [NotMapped]
        public List<MontageInDevice> MontageInDevices { 
            get {
                var result = new Dictionary<int, MontageInDevice>();
                if (Devices == null) return new List<MontageInDevice>();
                foreach (var devicesInTask in Devices)
                {
                    if (devicesInTask.Device == null) continue;
                    foreach (var montageInDevice in devicesInTask.Device.Montages)
                    {
                        if (result.TryGetValue(montageInDevice.ComponentId, out var resultMontageInDevice))
                        {
                            result[montageInDevice.ComponentId].Quantity += montageInDevice.Quantity * devicesInTask.Quantity;
                        }
                        else
                        {
                            result.Add(montageInDevice.ComponentId, new MontageInDevice()
                            {
                                Id = montageInDevice.Id,
                                DeviceId = montageInDevice.DeviceId,
                                ComponentId = montageInDevice.ComponentId,
                                Quantity = montageInDevice.Quantity * devicesInTask.Quantity,
                                Description = montageInDevice.Description,
                                Montage = montageInDevice.Montage,
                            });
                        }
                        
                    }
                }
                    
                return result.Select(x => x.Value).ToList();
            } 
        }

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

    [Table("DevicesInTasks")]
    public class DevicesInTask
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        public int Quantity { get; set; }
    }
}