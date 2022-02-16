using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.Tasks;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface ITaskService : IBaseService<Task>
    {
        System.Threading.Tasks.Task<IEnumerable<Task>> GetTasksByUserRoleAsync(IEnumerable<string> roles);
        System.Threading.Tasks.Task TransferAsync(int taskId, bool full, int to, string message);
        string GetTaskStatusName(TaskStatusEnum item);
        System.Threading.Tasks.Task ReceiveComponentsAsync(int taskId, int[] obtainedCompIds, int[] componentObt);
        System.Threading.Tasks.Task ReceiveDesignsAsync(int taskId, int[] obtainedDesIds, int[] designObt);
        System.Threading.Tasks.Task ReceiveComponentAsync(int taskId, int obtainedCompId, int componentObt);
        System.Threading.Tasks.Task ReceiveDesignAsync(int taskId, int obtainedDesId, int designObt);
        System.Threading.Tasks.Task DeleteByIdAsync(int id);
    }
    public class TaskService : BaseServiceWithLogs<Task>, ITaskService
    {
        private readonly IDeviceService _deviceService;
        private readonly IMontageService _montageService;
        private readonly IDesignService _designService;

        public TaskService(IUnitOfWork uow) : base(uow)
        {
            _deviceService = new DeviceService(uow);
            _montageService = new MontageService(uow);
            _designService = new DesignService(uow);
            _currentRepository = _db.TaskRepository;
        }

        public override async System.Threading.Tasks.Task CreateAsync(Task task)
        {
            task.Status = TaskStatusEnum.Equipment;
            task.EndTime = new DateTime();
            task.StartTime = DateTime.Now;

            task.ObtainedDesigns = (await _deviceService.GetByIdAsync(task.DeviceId)).Designs.Select(d =>
                new ObtainedDesign
                {
                    ComponentId = d.ComponentId,
                    TaskId = task.Id,
                });
            task.ObtainedMontages = (await _deviceService.GetByIdAsync(task.DeviceId)).Montages.Select(m =>
                new ObtainedMontage
                {
                    ComponentId = m.ComponentId,
                    TaskId = task.Id,
                });
            
            await base.CreateAsync(task);
        }

        public async System.Threading.Tasks.Task<IEnumerable<Task>> GetTasksByUserRoleAsync(IEnumerable<string> roles)
        {
            TaskStatusEnum accessLevel = ToStatus(roles);
            var tasks = await Find(task => (task.Status & accessLevel) == task.Status);
            return tasks;
        }
        
        public async System.Threading.Tasks.Task TransferAsync(int taskId, bool full, int to, string message)
        {
            var task = await GetByIdAsync(taskId);
            var logString = $"{_db.LogRepository.CurrentUser?.UserName} изменил статус задачи №{taskId} с {GetTaskStatusName(task.Status)} ";
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
            await _db.LogRepository.CreateAsync(new Log {Message = logString, TaskId = task.Id, OrderId = task.OrderId});
            await _db.SaveAsync();
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

        public async System.Threading.Tasks.Task ReceiveComponentsAsync(int taskId, int[] obtainedCompIds, int[] componentObt)
        {
            for (int i = 0; i < componentObt.Length; i++)
            {
                var obtComp = await _db.ObtainedMontageRepository.GetByIdAsync(obtainedCompIds[i]);
                if (obtComp != null)
                {
                    obtComp.Obtained += componentObt[i];
                    await _montageService.DecreaseQuantityAsync(obtComp.ComponentId, componentObt[i]);
                    await _db.ObtainedMontageRepository.UpdateAsync(obtComp);
                }
            }

            await _db.SaveAsync();
        }

        public async System.Threading.Tasks.Task ReceiveDesignsAsync(int taskId, int[] obtainedDesIds, int[] designObt)
        {
            for (int i = 0; i < designObt.Length; i++)
            {
                var obtDes = await _db.ObtainedDesignRepository.GetByIdAsync(obtainedDesIds[i]);
                if (obtDes != null)
                {
                    obtDes.Obtained += designObt[i];
                    await _designService.DecreaseQuantityAsync(obtDes.ComponentId, designObt[i]);
                    await _db.ObtainedDesignRepository.UpdateAsync(obtDes);
                }
            }
            
            await _db.SaveAsync();
        }
        
        public async System.Threading.Tasks.Task ReceiveComponentAsync(int taskId, int obtainedCompId, int componentObt)
        {
            var obtMont = await _db.ObtainedMontageRepository.GetByIdAsync(obtainedCompId);
            if (obtMont != null)
            {
                obtMont.Obtained += componentObt;
                await _montageService.IncreaseQuantityAsync(obtMont.ComponentId, -componentObt);
                await _db.ObtainedMontageRepository.UpdateAsync(obtMont);
            }

            await _db.SaveAsync();
        }
        
        public async System.Threading.Tasks.Task ReceiveDesignAsync(int taskId, int obtainedDesId, int designObt)
        {
            var obtDes = await _db.ObtainedDesignRepository.GetByIdAsync(obtainedDesId);
            if (obtDes != null)
            {
                obtDes.Obtained += designObt;
                await _designService.IncreaseQuantityAsync(obtDes.ComponentId, -designObt);
                await _db.ObtainedDesignRepository.UpdateAsync(obtDes);
            }
            
            await _db.SaveAsync();
        }

       

        public async System.Threading.Tasks.Task DeleteByIdAsync(int id)
        {
            await DeleteAsync(new Task {Id = id});
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
        
        protected override async System.Threading.Tasks.Task CreateLogForCreatingAsync(Task item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была создана задача " + item, TaskId = item.Id, OrderId = item.OrderId});
        }

        protected override async System.Threading.Tasks.Task CreateLogForUpdatingAsync(Task item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была изменёна задача " + item, TaskId = item.Id, OrderId = item.OrderId });
        }

        protected override async System.Threading.Tasks.Task CreateLogForDeletingAsync(Task item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Была удалёна задача " + item, TaskId = item.Id, OrderId = item.OrderId });
        }
        
        protected override bool UpdateLogPredicate(Log log, Task item) => log.TaskId == item.Id; 

        protected override void UpdateLog(Log log) => log.TaskId = null;
    }
}