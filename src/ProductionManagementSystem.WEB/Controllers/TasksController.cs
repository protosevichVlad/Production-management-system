using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using ProductionManagementSystem.Core.Infrastructure;
using ProductionManagementSystem.Core.Models.SupplyRequests;
using ProductionManagementSystem.Core.Models.Tasks;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.Core.Services.SupplyRequestServices;
using ProductionManagementSystem.WEB.Models.Tasks;
using Task = ProductionManagementSystem.Core.Models.Tasks.Task;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ITaskService _taskService;
        private readonly IDeviceService _deviceService;
        private readonly ILogService _logService;
        private readonly ISupplyRequestService<SupplyRequest> _supplyRequestService;

        public TasksController(ITaskService taskService, IDeviceService deviceService, UserManager<User> userManager, ILogService logService, ISupplyRequestService<SupplyRequest> supplyRequestService)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _userManager = userManager;
            _logService = logService;
            _supplyRequestService = supplyRequestService;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["DeviceSortParm"] = sortOrder == "Device" ? "device_desc" : "Device";
            ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["OrderIdSortParm"] = sortOrder == "OrderId" ? "orderid_desc" : "OrderId";
            ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
            ViewData["CurrentFilter"] = searchString;


            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var tasks = (await _taskService.GetTasksByUserRoleAsync(roles)).ToList();
            tasks = SortingTasks(tasks, sortOrder).ToList();

            return View(tasks);
        }
        
        public async Task<IActionResult> Create()
        {
            ViewBag.Devices = new SelectList(await _deviceService.GetAllAsync(), "Id", "Name");
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Task taskViewModel)
        {
            await _taskService.CreateAsync(taskViewModel);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var task = await _taskService.GetByIdAsync(id);
                ViewBag.States = new SelectList(GetStates(task), "Id", "Name");
                // ViewBag.Logs = _mapper.Map<IEnumerable<LogDTO>, IEnumerable<LogViewModel>>(_taskService.GetLogs(id));
                return View(new TaskDetailsViewModel()
                {
                    Task = task,
                    Logs = await _logService.GetByTaskIdAsync(id),
                    SupplyRequests = await _supplyRequestService.GetSupplyRequestsByTaskIdAsync(id)
                });
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var task = await _taskService.GetByIdAsync(id);
                return View(task);
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            };
        }
        
        [HttpPost("[controller]/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                await _taskService.DeleteByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.ErrorHeader = ex.Header;
                return View(nameof(Delete), await _taskService.GetByIdAsync(id));
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
            
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var task = await _taskService.GetByIdAsync(id);
                ViewBag.Devices = new SelectList(await _deviceService.GetAllAsync(), "Id", "Name");
                return View(task);
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(Task taskModel)
        {
            await _taskService.UpdateAsync(taskModel);
            return RedirectToAction(nameof(Details), new {id = taskModel.Id});
        }
        
        [HttpPost]
        public async Task<IActionResult> Transfer(int taskId, string full, int to, string message)
        {
            await _taskService.TransferAsync(taskId, full == "true", to, message);
            return RedirectToAction(nameof(Details), new {id = taskId});
        }

        [HttpGet]
        public async Task<IActionResult> ReceiveComponent(int taskId)
        {
            var task = await _taskService.GetByIdAsync(taskId);
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveComponent(int taskId, int[] obtainedCompIds, int[] componentObt)
        {
            await _taskService.ReceiveComponentsAsync(taskId, obtainedCompIds, componentObt);
            return RedirectToAction(nameof(Details), new {id = taskId});
        }
        
        [HttpGet]
        public async Task<IActionResult> ReceiveDesign(int taskId)
        {
            var task = await _taskService.GetByIdAsync(taskId);
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveDesign(int taskId, int[] obtainedCompIds, int[] designObt)
        {
            await _taskService.ReceiveDesignsAsync(taskId, obtainedCompIds, designObt);
            return RedirectToAction(nameof(Details), new {id = taskId});
        }
        
        public async Task<IActionResult> GetTaskItem(int index)
        {
            return PartialView("Partail/Task/TaskItem", new TaskItemViewModel()
            {
                Index = index,
                AllDevices = await _deviceService.GetAllAsync(),
                Quantity = 1
            });
        }

        private static IEnumerable<Task> SortingTasks(IEnumerable<Task> tasks, string sortOrder)
        {
            switch (sortOrder)
            {
                case "num_desc":
                    tasks = tasks.OrderByDescending(t => t.Id);
                    break;
                case "StartDate":
                    tasks = tasks.OrderBy(t => t.StartTime);
                    break;
                case "startdate_desc":
                    tasks = tasks.OrderByDescending(t => t.StartTime);
                    break;
                case "Status":
                    tasks = tasks.OrderBy(t => t.Status);
                    break;
                case "status_desc":
                    tasks = tasks.OrderByDescending(t => t.Status);
                    break;
                case "OrderId":
                    tasks = tasks.OrderBy(t => t.OrderId);
                    break;
                case "orderid_desc":
                    tasks = tasks.OrderByDescending(t => t.OrderId);
                    break;
                case "Deadline":
                    tasks = tasks.OrderBy(t => t.Deadline);
                    break;
                case "deadline_desc":
                    tasks = tasks.OrderByDescending(t => t.Deadline);
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.Id);
                    break;
            }

            return tasks;
        }

        private IEnumerable<dynamic> GetStates(Task task)
        {
            if (task == null)
            {
                throw new PageNotFoundException();
            }
            
            var states = new List<object>();
            foreach(TaskStatusEnum status in Enum.GetValues(typeof(TaskStatusEnum)) )
            {
                states.Add(new {Id = (int)status, Name = _taskService.GetTaskStatusName(status)});
                if (task.Status < status)
                {
                    break;
                } 
            }

            states.Reverse();
            return states;
        }
    }
}
