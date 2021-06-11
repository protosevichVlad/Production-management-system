using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using ProductionManagementSystem.Models;
using Task = ProductionManagementSystem.DAL.Entities.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public class TaskService : ITaskService
    {
        private IUnitOfWork _database { get; }
        private IDeviceService _deviceService;
        private IComponentService _componentService;
        private IDesignService _designService;
        private ILogService _logService;
        private IMapper _mapper;

        public TaskService(IUnitOfWork uow)
        {
            _database = uow;
            _deviceService = new DeviceService(uow);
            _componentService = new ComponentService(uow);
            _designService = new DesignService(uow);
            _logService = new LogService(uow);
            
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Task, TaskDTO>();
                    cfg.CreateMap<TaskDTO, Task>();
                    cfg.CreateMap<Device, DeviceDTO>();
                    cfg.CreateMap<DeviceDTO, Device>();
                    cfg.CreateMap<Log, LogDTO>();
                    cfg.CreateMap<ObtainedDesign, ObtainedDesignDTO>();
                    cfg.CreateMap<ObtainedDesignDTO, ObtainedDesign>();
                    cfg.CreateMap<ObtainedComponent, ObtainedComponentDTO>();
                    cfg.CreateMap<ObtainedComponentDTO, ObtainedComponent>();
                    cfg.CreateMap<ComponentDTO, Component>();
                    cfg.CreateMap<Component, ComponentDTO>();
                    cfg.CreateMap<DesignDTO, Design>();
                    cfg.CreateMap<Design, DesignDTO>();
                })
                .CreateMapper();
        }

        public async System.Threading.Tasks.Task CreateTaskAsync(TaskDTO taskDto)
        {
            var task = _mapper.Map<TaskDTO, Task>(taskDto);
            task.Status = StatusEnum.Equipment;
            task.EndTime = new DateTime();
            task.StartTime = DateTime.Now;

            var device = await _deviceService.GetDeviceAsync(task.DeviceId);
            task.ObtainedComponents = _mapper.Map<IEnumerable<ObtainedComponentDTO>, IEnumerable<ObtainedComponent>>(GetStartObtainedComponent(device, task));
            task.ObtainedDesigns = _mapper.Map<IEnumerable<ObtainedDesignDTO>, IEnumerable<ObtainedDesign>>(GetStartObtainedDesign(device, task));

            await _database.Tasks.CreateAsync(task);
            await _database.SaveAsync();
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(TaskDTO taskDto)
        {
            var task = _mapper.Map<TaskDTO, Task>(taskDto);
            
            _database.Tasks.Update(task);
            await _database.SaveAsync();
        }
        
        public async System.Threading.Tasks.Task EditTaskAsync(TaskDTO taskDto)
        {
            var oldTask = await _database.Tasks.GetAsync(taskDto.Id);
            oldTask.Description = taskDto.Description;
            oldTask.Deadline = taskDto.Deadline;
            await _database.SaveAsync();
        }

        public async Task<IEnumerable<TaskDTO>> GetTasksAsync()
        {
            return _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>(await _database.Tasks.GetAllAsync());
        }

        public async Task<IEnumerable<TaskDTO>> GetTasksAsync(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                return Array.Empty<TaskDTO>();
            }

            StatusEnum accessLevel = ToStatus(roles);
            return _mapper.Map<IEnumerable<Task>, IEnumerable<TaskDTO>>((await _database.Tasks.GetAllAsync()).Where(task => (task.Status & accessLevel) == task.Status));
        }

        public async Task<TaskDTO> GetTaskAsync(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var task = await _database.Tasks.GetAsync((int) id) ?? throw new PageNotFoundException();
            var taskDto = _mapper.Map<Task, TaskDTO>(task) ?? throw new PageNotFoundException();

            return taskDto;
        }

        public async System.Threading.Tasks.Task TransferAsync(int taskId, bool full, int to, string message)
        {
            var task = await _database.Tasks.GetAsync(taskId);
            var logString = $"{LogService.UserName} изменил статус задачи №{taskId} с {GetTaskStatusName(task.Status)} ";
            if (task.Status == StatusEnum.Warehouse)
            {
                await _deviceService.ReceiveDeviceAsync(task.DeviceId);
            }
            
            if ((StatusEnum) to == StatusEnum.Warehouse)
            {
                await _deviceService.AddDeviceAsync(task.DeviceId);
            }
            
            if (full)
            {
                task.Status = (StatusEnum) to;
            }
            else
            {
                task.Status |= (StatusEnum) to;
            }
            
            await _database.SaveAsync();

            logString += $"на {GetTaskStatusName(task.Status)} с сообщением: {message}";
            await _logService.CreateLogAsync(new LogDTO(logString) {TaskId = task.Id, OrderId = task.OrderId});
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            await _database.Tasks.DeleteAsync((int) id);
            await _database.SaveAsync();
        }
        
        public async Task<IEnumerable<DeviceDesignTemplate>> GetDeviceDesignTemplateFromTaskAsync(int taskId)
        {
            var deviceId = (await GetTasksAsync())
                .FirstOrDefault(t => t.Id == taskId)?.DeviceId;

            if (deviceId == null)
            {
                return new List<DeviceDesignTemplate>();
            }
            
            return await _deviceService.GetDesignTemplatesAsync((int) deviceId);
        }

        public async Task<IEnumerable<DeviceComponentsTemplate>> GetDeviceComponentsTemplatesFromTaskAsync(int taskId)
        {
            var deviceId = (await GetTasksAsync())
                .FirstOrDefault(t => t.Id == taskId)?.DeviceId;

            if (deviceId == null)
            {
                return new List<DeviceComponentsTemplate>();
            }
            
            return await _deviceService.GetComponentsTemplatesAsync((int) deviceId);
        }

        public IEnumerable<ObtainedComponentDTO> GetObtainedComponents(int taskId)
        {
            return _mapper.Map<IEnumerable<ObtainedComponent>, IEnumerable<ObtainedComponentDTO>>(_database.ObtainedСomponents.Find(c => c.Task.Id == taskId));
        }

        public IEnumerable<ObtainedDesignDTO> GetObtainedDesigns(int taskId)
        {
            return _mapper.Map<IEnumerable<ObtainedDesign>, IEnumerable<ObtainedDesignDTO>>(_database.ObtainedDesigns.Find(c => c.Task.Id == taskId));
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

        public async System.Threading.Tasks.Task ReceiveComponentsAsync(int taskId, int[] componentIds, int[] componentObt)
        {
            var obtainedComp = GetObtainedComponents(taskId);
            for (int i = 0; i < componentObt.Length; i++)
            {
                var obtComp = obtainedComp.FirstOrDefault(c => c.ComponentId == componentIds[i]);
                if (obtComp != null)
                {
                    obtComp.Obtained += componentObt[i];
                    await _componentService.AddComponentAsync(componentIds[i], componentObt[i]);
                    _database.ObtainedСomponents.Update(_mapper.Map<ObtainedComponentDTO, ObtainedComponent>(obtComp));
                }
            }
            
            await _database.SaveAsync();
        }

        public async System.Threading.Tasks.Task ReceiveDesignsAsync(int taskId, int[] designIds, int[] designObt)
        {
            var obtainedDes = GetObtainedDesigns(taskId);
            for (int i = 0; i < designObt.Length; i++)
            {
                var obtDes = obtainedDes.FirstOrDefault(c => c.DesignId == designIds[i]);
                if (obtDes != null)
                {
                    obtDes.Obtained += designObt[i];
                    await _designService.AddDesignAsync(designIds[i], -designObt[i]);
                    _database.ObtainedDesigns.Update(_mapper.Map<ObtainedDesignDTO, ObtainedDesign>(obtDes));
                }
            }
            
            await _database.SaveAsync();
        }
        
        public async System.Threading.Tasks.Task ReceiveComponentAsync(int taskId, int componentId, int componentObt)
        {
            var obtainedComp = GetObtainedComponents(taskId);
            var obtComp = obtainedComp.FirstOrDefault(c => c.ComponentId == componentId);
            if (obtComp != null)
            {
                obtComp.Obtained += componentObt;
                await _componentService.AddComponentAsync(componentId, -componentObt);
                _database.ObtainedСomponents.Update(_mapper.Map<ObtainedComponentDTO, ObtainedComponent>(obtComp));
            }

            await _database.SaveAsync();
        }
        
        public async System.Threading.Tasks.Task ReceiveDesignAsync(int taskId, int designId, int designObt)
        {
            var obtainedDes = GetObtainedDesigns(taskId);
            var obtDes = obtainedDes.FirstOrDefault(c => c.DesignId == designId);
            if (obtDes != null)
            {
                obtDes.Obtained += designObt;
                await _designService.AddDesignAsync(designId, -designObt);
                _database.ObtainedDesigns.Update(_mapper.Map<ObtainedDesignDTO, ObtainedDesign>(obtDes));
            }
            
            await _database.SaveAsync();
        }

        public void Dispose()
        {
            _database.Dispose();
        }

        private IEnumerable<ObtainedDesignDTO> GetStartObtainedDesign(DeviceDTO device, Task task)
        {
            List<ObtainedDesignDTO> result = new List<ObtainedDesignDTO>();
            foreach (var designId in device.DesignIds)
            {
                result.Add(new ObtainedDesignDTO()
                {
                    DesignId = designId,
                    Obtained = 0,
                    Task = _mapper.Map<Task, TaskDTO>(task),
                });
            }

            return result;
        }
        
        private IEnumerable<ObtainedComponentDTO> GetStartObtainedComponent(DeviceDTO device, Task task)
        {
            List<ObtainedComponentDTO> result = new List<ObtainedComponentDTO>();
            foreach (var componentId in device.ComponentIds)
            {
                result.Add(new ObtainedComponentDTO()
                {
                    ComponentId = componentId,
                    Obtained = 0,
                    Task = _mapper.Map<Task, TaskDTO>(task),
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