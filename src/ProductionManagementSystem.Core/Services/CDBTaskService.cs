using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.Tasks;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Repositories;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.Core.Services
{
    public interface ICDBTaskService : IBaseService<CDBTask>
    {
        Task<List<CDBTask>> GetTasksByUserRoleAsync(List<string> roles);
        Task TransferAsync(int taskId, bool full, int to, string message);
        string GetTaskStatusName(TaskStatusEnum item);
        Task ObtainItems(int taskId, List<ObtainedModel> obtainedModels);
        Task<List<CDBTask>> AlsoCreatedAsync(CDBTask task);
    }

    public class CDBTaskService : BaseService<CDBTask, IUnitOfWork>, ICDBTaskService
    {
        private readonly ICompDbDeviceService _deviceService;
        private readonly IPcbService _pcbService;
        private readonly IEntityExtService _entityService;

        public CDBTaskService(IUnitOfWork db, ICompDbDeviceService deviceService, IPcbService pcbService, IEntityExtService entityService) : base(db)
        {
            _deviceService = deviceService;
            _pcbService = pcbService;
            _entityService = entityService;
            _currentRepository = _db.CdbTasksRepository;
        }

        public async Task<List<CDBTask>> GetTasksByUserRoleAsync(List<string> roles)
        {
            TaskStatusEnum accessLevel = ToStatus(roles);
            var tasks = await Find(task => (task.Status & accessLevel) == task.Status);
            return tasks;
        }

        public async Task TransferAsync(int taskId, bool full, int to, string message)
        {
            var task = await GetByIdAsync(taskId);
            var logString = $"{_db.LogRepository.CurrentUser?.UserName} изменил статус задачи №{taskId} с {GetTaskStatusName(task.Status)} ";
            if (task.Status == TaskStatusEnum.Warehouse)
            {
                await _deviceService.DecreaseQuantityAsync(task.TaskItemId, 1);
            }
            
            if ((TaskStatusEnum) to == TaskStatusEnum.Warehouse)
            {
                await _deviceService.IncreaseQuantityAsync(task.TaskItemId, 1);
            }
            
            if (full)
                task.Status = (TaskStatusEnum) to;
            else
                task.Status |= (TaskStatusEnum) to;

            await base.UpdateAsync(task);

            logString += $"на {GetTaskStatusName(task.Status)}" + 
                         (!string.IsNullOrWhiteSpace(message) ? $" с сообщением: {message}": string.Empty);
            await _db.LogRepository.CreateAsync(new Log {Message = logString, TaskId = task.Id});
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

        public async Task ObtainItems(int taskId, List<ObtainedModel> obtainedModels)
        {
            var task = await GetByIdAsync(taskId);
            foreach (var obtainedModel in obtainedModels)
            {
                var obtained = await _db.CdbObtainedRepository.GetByIdAsync(obtainedModel.ObtainedId);
                ICalculableService calculableService = obtained.ObtainedItem.ItemType switch
                {
                    CDBItemType.Device => _deviceService,
                    CDBItemType.PCB => _pcbService,
                    CDBItemType.Entity => _entityService,
                    _ => throw new ArgumentOutOfRangeException()
                };

                await calculableService.ChangeQuantityAsync(obtainedModel.ObtainedId, obtainedModel.Quantity);
            }
            
        }

        public async Task<List<CDBTask>> AlsoCreatedAsync(CDBTask task)
        {
                        
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