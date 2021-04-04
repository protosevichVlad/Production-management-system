using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.BLL.Services
{
    public class TaskService : ITaskService
    {
        private IUnitOfWork _database { get; }
        private IDeviceService _deviceService;
        private ILogService _logService;
        private IMapper _mapper;

        public TaskService(IUnitOfWork uow)
        {
            _database = uow;
            _deviceService = new DeviceService(uow);
            _logService = new LogService(uow);
            
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Task, TaskDTO>();
                    cfg.CreateMap<Device, DeviceDTO>();
                    cfg.CreateMap<TaskDTO, Task>();
                    cfg.CreateMap<DeviceDTO, Device>();
                    cfg.CreateMap<Log, LogDTO>();
                })
                .CreateMapper();
        }

        public void CreateTask(TaskDTO taskDto)
        {
            var task = _mapper.Map<TaskDTO, Task>(taskDto);
            task.Status = StatusEnum.Equipment;
            task.EndTime = new DateTime();
            task.StartTime = DateTime.Now;

            var device = _deviceService.GetDevice(task.DeviceId);
            task.ObtainedComponents = GetStartObtainedComponent(device, task);
            task.ObtainedDesigns = GetStartObtainedDesign(device, task);

            _database.Tasks.Create(task);
            _database.Save();
        }

        public void UpdateTask(TaskDTO taskDto)
        {
            var task = _mapper.Map<TaskDTO, Task>(taskDto);
            
            _database.Tasks.Update(task);
            _database.Save();
        }
        
        public void EditTask(TaskDTO taskDto)
        {
            var oldTask = _database.Tasks.Get(taskDto.Id);
            oldTask.Description = taskDto.Description;
            oldTask.Deadline = taskDto.Deadline;
            _database.Save();
        }

        public IEnumerable<TaskDTO> GetTasks()
        {
            return _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>(_database.Tasks.GetAll());
        }

        public IEnumerable<TaskDTO> GetTasks(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                return Array.Empty<TaskDTO>();
            }

            StatusEnum accessLevel = ToStatus(roles);
            return _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>(_database.Tasks.GetAll().Where(task => (task.Status & accessLevel) == task.Status));
        }

        public TaskDTO GetTask(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var task = _database.Tasks.Get((int) id) ?? throw new PageNotFoundException();
            var taskDto = _mapper.Map<Task, TaskDTO>(task) ?? throw new PageNotFoundException();

            return taskDto;
        }

        public void Transfer(int taskId, bool full, int to, string message)
        {
            var task = _database.Tasks.Get(taskId);
            var logString = $"{LogService.UserName} изменил статус задачи №{taskId} с {GetTaskStatusName(task.Status)} ";
            if (task.Status == StatusEnum.Warehouse)
            {
                _deviceService.ReceiveDevice(task.DeviceId);
            }
            
            if ((StatusEnum) to == StatusEnum.Warehouse)
            {
                _deviceService.AddDevice(task.DeviceId);
            }
            
            if (full)
            {
                task.Status = (StatusEnum) to;
            }
            else
            {
                task.Status |= (StatusEnum) to;
            }
            
            _database.Save();

            logString += $"на {GetTaskStatusName(task.Status)} с сообщением: {message}";
            _logService.CreateLog(new LogDTO(logString) {TaskId = task.Id, OrderId = task.OrderId});
        }

        public void DeleteTask(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            _database.Tasks.Delete((int) id);
            _database.Save();
        }
        
        public IEnumerable<DeviceDesignTemplate> GetDeviceDesignTemplateFromTask(int taskId)
        {
            var deviceId = GetTasks()
                .FirstOrDefault(t => t.Id == taskId)?.DeviceId;

            if (deviceId == null)
            {
                return new List<DeviceDesignTemplate>();
            }
            
            return _deviceService.GetDesignTemplates((int) deviceId);
        }

        public IEnumerable<DeviceComponentsTemplate> GetDeviceComponentsTemplatesFromTask(int taskId)
        {
            var deviceId = GetTasks()
                .FirstOrDefault(t => t.Id == taskId)?.DeviceId;

            if (deviceId == null)
            {
                return new List<DeviceComponentsTemplate>();
            }
            
            return _deviceService.GetComponentsTemplates((int) deviceId);
        }

        public IEnumerable<ObtainedComponent> GetObtainedСomponents(int taskId)
        {
            return _database.ObtainedСomponents.Find(c => c.Task.Id == taskId);
        }

        public IEnumerable<ObtainedDesign> GetObtainedDesigns(int taskId)
        {
            return _database.ObtainedDesigns.Find(c => c.Task.Id == taskId);
        }

        public IEnumerable<LogDTO> GetLogs(int? taskId)
        {
            if (taskId == null)
            {
                throw new PageNotFoundException();
            }
            
            var logs = _mapper.Map<IEnumerable<Log>, IEnumerable<LogDTO>>(_database.Logs.Find(log => log.TaskId == taskId).Reverse());
            if (logs == null)
            {
                return Array.Empty<LogDTO>();
            }

            return logs;
        }

        public string GetTaskStatusName(StatusEnum item)
        {
            List<string> result = new List<string>();
            foreach (var value in Enum.GetValues<StatusEnum>())
            {
                if ((item & value) == value)
                {
                    result.Add(value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName());
                }
            }
            return String.Join(", ", result);
        }

        public void ReceiveComponent(int taskId, int[] componentIds, int[] componentObt)
        {
            var obtainedComp = GetObtainedСomponents(taskId);
            for (int i = 0; i < componentObt.Length; i++)
            {
                var obtComp = obtainedComp.FirstOrDefault(c => c.Id == componentIds[i]);
                if (obtComp != null)
                {
                    obtComp.Obtained += componentObt[i];
                    obtComp.Component.Quantity -= componentObt[i];
                    _database.ObtainedСomponents.Update(obtComp);
                }
            }
            
            _database.Save();
        }

        public void ReceiveDesign(int taskId, int[] designIds, int[] designObt)
        {
            var obtainedDes = GetObtainedDesigns(taskId);
            for (int i = 0; i < designObt.Length; i++)
            {
                var obtDes = obtainedDes.FirstOrDefault(c => c.Id == designIds[i]);
                if (obtDes != null)
                {
                    obtDes.Obtained += designObt[i];
                    obtDes.Design.Quantity -= designObt[i];
                    _database.ObtainedDesigns.Update(obtDes);
                }
            }
            
            _database.Save();
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        private List<ObtainedDesign> GetStartObtainedDesign(DeviceDTO device, Task task)
        {
            List<ObtainedDesign> result = new List<ObtainedDesign>();
            foreach (var designId in device.DesignIds)
            {
                result.Add(new ObtainedDesign()
                {
                    DesignId = designId,
                    Obtained = 0,
                    Task = task
                });
            }

            return result;
        }
        
        private List<ObtainedComponent> GetStartObtainedComponent(DeviceDTO device, Task task)
        {
            List<ObtainedComponent> result = new List<ObtainedComponent>();
            foreach (var componentId in device.ComponentIds)
            {
                result.Add(new ObtainedComponent()
                {
                    ComponentId = componentId,
                    Obtained = 0,
                    Task = task
                });
            }

            return result;
        }

        private StatusEnum ToStatus(IEnumerable<string> roles)
        {
            StatusEnum status = 0;
            foreach (var role in roles)
            {
                status |= role switch
                {
                    RoleEnum.Assembler => StatusEnum.Montage,
                    RoleEnum.Collector => StatusEnum.Assembly,
                    RoleEnum.OrderPicker => StatusEnum.Equipment,
                    RoleEnum.Shipper => StatusEnum.Warehouse,
                    RoleEnum.Tuner => StatusEnum.Customization,
                    RoleEnum.Validating => StatusEnum.Validate,
                    RoleEnum.Admin => (StatusEnum)int.MaxValue,
                    _ => 0
                };
            }
            return status;
        }
    }
}