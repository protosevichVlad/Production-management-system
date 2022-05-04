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
        Task<List<CDBTask>> AlsoCreatedAsync(CDBTask task, Dictionary<UniversalItem, int> quantityInStockDict=null);
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
            ICalculableService calculableService = task.ItemType switch
            {
                CDBItemType.Device => _deviceService,
                CDBItemType.PCB => _pcbService,
                CDBItemType.Entity => _entityService,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (task.Status == TaskStatusEnum.Warehouse)
            {
                await calculableService.DecreaseQuantityAsync(task.ItemId, 1);
            }
            
            if ((TaskStatusEnum) to == TaskStatusEnum.Warehouse)
            {
                await calculableService.IncreaseQuantityAsync(task.ItemId, 1);
            }
            
            if (full)
                task.Status = (TaskStatusEnum) to;
            else
                task.Status |= (TaskStatusEnum) to;

            await _currentRepository.UpdateAsync(task);

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
                ICalculableService calculableService = obtained.UsedItem.ItemType switch
                {
                    CDBItemType.Device => _deviceService,
                    CDBItemType.PCB => _pcbService,
                    CDBItemType.Entity => _entityService,
                    _ => throw new ArgumentOutOfRangeException()
                };

                await calculableService.ChangeQuantityAsync(obtained.UsedItem.ItemId, -obtainedModel.Quantity);
                
                obtained.Quantity += obtainedModel.Quantity;
                await _db.SaveAsync();
            }
            
        }

        public async Task<List<CDBTask>> AlsoCreatedAsync(CDBTask task, Dictionary<UniversalItem, int> quantityInStockDict = null)
        {
            if (quantityInStockDict == null)
            {
                quantityInStockDict = new Dictionary<UniversalItem, int>();
            }
            
            if (task.Id == 0)
            {
                var tasks = await GetAllAsync();
                if (tasks.Count > 0)
                {
                    task.Id = tasks.Max(x => x.Id) + 1;
                }
                else
                {
                    task.Id = 1;
                }
            }

            if (task.Item == null)
            {
                task.Item = task.ItemType switch
                {
                    CDBItemType.Device => new UniversalItem(await _deviceService.GetByIdAsync(task.ItemId)),
                    CDBItemType.PCB => new UniversalItem(await _pcbService.GetByIdAsync(task.ItemId)),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            var items = task.ItemType switch
            {
                CDBItemType.Device => (await _deviceService.GetByIdAsync(task.ItemId)).UsedItems,
                CDBItemType.PCB => (await _pcbService.GetByIdAsync(task.ItemId)).UsedItems,
                _ => new List<UsedItem>()
            };

            var result = new List<CDBTask> {task};

            foreach (var item in items)
            {
                if (item.ItemType != CDBItemType.Entity)
                {
                    int quantityInStock;
                    
                    if (quantityInStockDict.ContainsKey(item.Item))
                    {
                        quantityInStock = quantityInStockDict[item.Item];
                    }
                    else
                    {
                        quantityInStock = item.ItemType switch
                        {
                            CDBItemType.Device => (await _deviceService.GetByIdAsync(item.ItemId))?.Quantity ?? 0,
                            CDBItemType.PCB => (await _pcbService.GetByIdAsync(item.ItemId))?.Quantity ?? 0,
                            _ => throw new ArgumentOutOfRangeException(),
                        };

                        quantityInStockDict[item.Item] = quantityInStock;
                    }

                    var needQuantity = item.Quantity - quantityInStock;
                    if (needQuantity < 1)
                    {
                        needQuantity = 0;
                        quantityInStock -= item.Quantity;
                    }
                    else
                    {
                        quantityInStock = 0;
                    }

                    quantityInStockDict[item.Item] = quantityInStock;
                    for (int i = 0; i < needQuantity; i++)
                    {
                        result.AddRange(await AlsoCreatedAsync(new CDBTask()
                        {
                            Deadline = task.Deadline,
                            Description = $"This task was created because of the creation of task №{task.Id}",
                            ItemId = item.ItemId,
                            ItemType = item.ItemType,
                            ParentTaskId = task.Id,
                            Id = task.Id + result.Count,
                        }, quantityInStockDict));
                    }
                }
            }

            return result;
        }

        public override async Task CreateAsync(CDBTask item)
        {
            var tasks = await AlsoCreatedAsync(item);
            foreach (var task in tasks)
            {
                if (task.Obtained == null || task.Obtained.Count == 0)
                {
                    task.Obtained = await GenerateObtainedAsync(task);
                }
                
                await base.CreateAsync(task);
            }
        }

        private async Task<List<CDBObtained>> GenerateObtainedAsync(CDBTask task)
        {
            var usedItems = await _db.UsedItemRepository.GetByInItemIdAsync(task.ItemId, task.ItemType);
            return usedItems.Select(x => new CDBObtained()
            {
                UsedItemId = x.Id,
                TaskId = task.Id,
            }).ToList();
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