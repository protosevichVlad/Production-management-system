using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.Tasks;
using ProductionManagementSystem.Models.Users;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public interface ITaskService : IBaseService<Models.Tasks.Task>
    {
        Task<IEnumerable<Models.Tasks.Task>> GetTasksByUserRoleAsync(IEnumerable<string> roles);
        Task TransferAsync(int taskId, bool full, int to, string message);
        string GetTaskStatusName(TaskStatusEnum item);
        Task ReceiveComponentsAsync(int taskId, int[] componentIds, int[] componentObt);
        Task ReceiveDesignsAsync(int taskId, int[] designIds, int[] designObt);
        Task ReceiveComponentAsync(int taskId, int componentId, int componentObt);
        Task ReceiveDesignAsync(int taskId, int designId, int designObt);
        Task DeleteByIdAsync(int id);
    }
    public class TaskService : BaseService<Models.Tasks.Task>, ITaskService
    {
        private readonly IDeviceService _deviceService;
        private readonly IMontageService _montageService;
        private readonly IDesignService _designService;
        private readonly ILogService _logService;

        public TaskService(IUnitOfWork uow) : base(uow)
        {
            _deviceService = new DeviceService(uow);
            _montageService = new MontageService(uow);
            _designService = new DesignService(uow);
            _logService = new LogService(uow);
            _currentRepository = _db.TaskRepository;
        }

        public override async Task CreateAsync(Models.Tasks.Task task)
        {
            task.Status = TaskStatusEnum.Equipment;
            task.EndTime = new DateTime();
            task.StartTime = DateTime.Now;

            task.ObtainedDesigns = (await _deviceService.GetByIdAsync(task.DeviceId)).Designs.Select(d =>
                new ObtainedDesign()
                {
                    ComponentId = d.Id,
                    TaskId = task.Id,
                    Obtained = d.Quantity
                });
            task.ObtainedMontages = (await _deviceService.GetByIdAsync(task.DeviceId)).Montage.Select(m =>
                new ObtainedMontage()
                {
                    ComponentId = m.Id,
                    TaskId = task.Id,
                    Obtained = m.Quantity
                });
            
            await base.CreateAsync(task);
        }

        public async Task<IEnumerable<Models.Tasks.Task>> GetTasksByUserRoleAsync(IEnumerable<string> roles)
        {
            TaskStatusEnum accessLevel = ToStatus(roles);
            return Find(task => (task.Status & accessLevel) == task.Status);
        }
        
        public async System.Threading.Tasks.Task TransferAsync(int taskId, bool full, int to, string message)
        {
            var task = await GetByIdAsync(taskId);
            // TODO: change username
            var logString = $"UserName изменил статус задачи №{taskId} с {GetTaskStatusName(task.Status)} ";
            if (task.Status == TaskStatusEnum.Warehouse)
            {
                await _deviceService.ReceiveDeviceAsync(task.DeviceId);
            }
            
            if ((TaskStatusEnum) to == TaskStatusEnum.Warehouse)
            {
                await _deviceService.AddDeviceAsync(task.DeviceId);
            }
            
            if (full)
            {
                task.Status = (TaskStatusEnum) to;
            }
            else
            {
                task.Status |= (TaskStatusEnum) to;
            }

            await base.UpdateAsync(task);

            logString += $"на {GetTaskStatusName(task.Status)}" + 
                         (String.IsNullOrWhiteSpace(message) ? $" с сообщением: {message}": String.Empty);
            await _logService.CreateAsync(new Log() {Message = logString, TaskId = task.Id, OrderId = task.OrderId});
        }

        public string GetTaskStatusName(TaskStatusEnum item)
        {
            List<string> result = new List<string>();
            foreach (var value in Enum.GetValues<TaskStatusEnum>())
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
            var obtainedComp = (await GetByIdAsync(taskId)).ObtainedMontages;
            for (int i = 0; i < componentObt.Length; i++)
            {
                var obtComp = obtainedComp.FirstOrDefault(c => c.ComponentId == componentIds[i]);
                if (obtComp != null)
                {
                    obtComp.Obtained += componentObt[i];
                    await _montageService.DecreaseQuantityOfDesignAsync(componentIds[i], componentObt[i]);
                    _db.ObtainedMontageRepository.Update(obtComp);
                }
            }
        }

        public async System.Threading.Tasks.Task ReceiveDesignsAsync(int taskId, int[] designIds, int[] designObt)
        {
            var obtainedDes = (await GetByIdAsync(taskId)).ObtainedDesigns;
            for (int i = 0; i < designObt.Length; i++)
            {
                var obtDes = obtainedDes.FirstOrDefault(c => c.ComponentId == designIds[i]);
                if (obtDes != null)
                {
                    obtDes.Obtained += designObt[i];
                    await _designService.DecreaseQuantityOfDesignAsync(designIds[i], designObt[i]);
                    _db.ObtainedDesignRepository.Update(obtDes);
                }
            }
        }
        
        public async System.Threading.Tasks.Task ReceiveComponentAsync(int taskId, int componentId, int componentObt)
        {
            var obtainedMont = (await GetByIdAsync(taskId)).ObtainedMontages;
            var obtMont = obtainedMont.FirstOrDefault(c => c.ComponentId == componentId);
            if (obtMont != null)
            {
                obtMont.Obtained += componentObt;
                await _montageService.IncreaseQuantityOfMontageAsync(componentId, -componentObt);
                _db.ObtainedMontageRepository.Update(obtMont);
            }
        }
        
        public async System.Threading.Tasks.Task ReceiveDesignAsync(int taskId, int designId, int designObt)
        {
            var obtainedDes = (await GetByIdAsync(taskId)).ObtainedDesigns;
            var obtDes = obtainedDes.FirstOrDefault(c => c.ComponentId == designId);
            if (obtDes != null)
            {
                obtDes.Obtained += designObt;
                await _designService.IncreaseQuantityOfDesignAsync(designId, -designObt);
                _db.ObtainedDesignRepository.Update(obtDes);
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            await this.DeleteAsync(new Models.Tasks.Task() {Id = id});
        }


        private TaskStatusEnum ToStatus(IEnumerable<string> roles)
        {
            TaskStatusEnum status = 0;
            foreach (var role in roles)
            {
                status |= role switch
                {
                    RoleEnum.Assembler => TaskStatusEnum.Montage,
                    RoleEnum.Collector => TaskStatusEnum.Assembly,
                    RoleEnum.OrderPicker => TaskStatusEnum.Equipment,
                    RoleEnum.Shipper => TaskStatusEnum.Warehouse,
                    RoleEnum.Tuner => TaskStatusEnum.Customization,
                    RoleEnum.Validating => TaskStatusEnum.Validate,
                    RoleEnum.Admin => (TaskStatusEnum)int.MaxValue,
                    _ => 0
                };
            }
            return status;
        }
    }
}