using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.WEB.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Entities;
using ObtainedComponent = ProductionManagementSystem.WEB.Models.ObtainedComponent;
using ObtainedDesign = ProductionManagementSystem.WEB.Models.ObtainedDesign;

namespace ProductionManagementSystem.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private UserManager<ProductionManagementSystemUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ITaskService _taskService;
        private readonly IDeviceService _deviceService;
        private IMapper _mapper;

        public TasksController(ITaskService taskService, IDeviceService deviceService, UserManager<ProductionManagementSystemUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TaskDTO, TaskViewModel>()
                        .ForMember(
                            task => task.Status, 
                            opt => opt.MapFrom(
                                src => _taskService.GetTaskStatusName(src.Status)
                                )
                            );
                    cfg.CreateMap<TaskViewModel, TaskDTO>();
                    cfg.CreateMap<DeviceDTO, DeviceViewModel>();
                    cfg.CreateMap<LogDTO, LogViewModel>();
                    cfg.CreateMap<ObtainedDesign, ObtainedDesignDTO>();
                    cfg.CreateMap<ObtainedDesignDTO, ObtainedDesign>();
                    cfg.CreateMap<ObtainedComponent, ObtainedComponentDTO>();
                    cfg.CreateMap<ObtainedComponentDTO, ObtainedComponent>();
                })
                .CreateMapper();
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["DeviceSortParm"] = sortOrder == "Device" ? "device_desc" : "Device";
            ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["OrderIdSortParm"] = sortOrder == "OrderId" ? "orderid_desc" : "OrderId";

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var tasksDto = (await _taskService.GetTasksAsync(roles)).ToList();
            var tasksViewModel =
                _mapper.Map<IEnumerable<TaskDTO>, IEnumerable<TaskViewModel>>(tasksDto).ToList();
            
            SortingTasks(tasksViewModel, sortOrder);
            

            ViewData["CurrentFilter"] = searchString;
            
            return View(tasksViewModel);
        }
        
        public async Task<IActionResult> Create()
        {
            ViewBag.Devices = new SelectList(await _deviceService.GetDevicesAsync(), "Id", "Name");
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(TaskViewModel taskViewModel)
        {
            var taskDto = _mapper.Map<TaskViewModel, TaskDTO>(taskViewModel);
            await _taskService.CreateTaskAsync(taskDto);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var taskDto = await _taskService.GetTaskAsync(id);
                var taskViewModel = _mapper.Map<TaskDTO, TaskViewModel>(taskDto);
                ViewBag.States = new SelectList(GetStates(taskDto), "Id", "Name");
                ViewBag.ComponentTemplate = await _deviceService.GetComponentsTemplatesAsync(taskDto.DeviceId);
                ViewBag.DesignTemplate = await _deviceService.GetDesignTemplatesAsync(taskDto.DeviceId);
                ViewBag.Logs = _mapper.Map<IEnumerable<LogDTO>, IEnumerable<LogViewModel>>(_taskService.GetLogs(id));
                return View(taskViewModel);
            }
            catch (PageNotFoundException e)
            {
                throw new Exception("Страница не найдена.");
            }
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var taskDto = await _taskService.GetTaskAsync(id);
                var taskViewModel = _mapper.Map<TaskDTO, TaskViewModel>(taskDto);
                return View(taskViewModel);
            }
            catch (PageNotFoundException e)
            {
                throw new Exception("Страница не найдена.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(Id);
                return RedirectToAction(nameof(Index));
            }
            catch (PageNotFoundException e)
            {
                throw new Exception("Страница не найдена.");
            }
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var taskDto = await _taskService.GetTaskAsync(id);
                var taskViewModel = _mapper.Map<TaskDTO, TaskViewModel>(taskDto);
                ViewBag.Devices = new SelectList(await _deviceService.GetDevicesAsync(), "Id", "Name");
                return View(taskViewModel);
            }
            catch (PageNotFoundException e)
            {
                throw new Exception("Страница не найдена.");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(TaskViewModel taskModel)
        {
            var taskDto = _mapper.Map<TaskViewModel, TaskDTO>(taskModel);
            await _taskService.UpdateTaskAsync(taskDto);
            return RedirectToAction(nameof(Details), new {id = taskModel.Id});
        }
        
        [HttpPost]
        public async Task<IActionResult> Transfer(int taskId, string full, int to, string message)
        {
            LogService.UserName = User.Identity?.Name;
            await _taskService.TransferAsync(taskId, full == "true", to, message);
            return RedirectToAction(nameof(Details), new {id = taskId});
        }

        [HttpGet]
        public async Task<IActionResult> ReceiveComponent(int taskId)
        {
            ViewBag.TaskId = taskId;
            ViewBag.Components = await _taskService.GetDeviceComponentsTemplatesFromTaskAsync(taskId);
            ViewBag.ObtainedComponents = _taskService.GetObtainedComponents(taskId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveComponent(int TaskId, int[] ComponentIds, int[] ComponentObt)
        {
            await _taskService.ReceiveComponentsAsync(TaskId, ComponentIds, ComponentObt);
            return RedirectToAction(nameof(Details), new {id = TaskId});
        }
        
        [HttpGet]
        public async Task<IActionResult> ReceiveDesign(int taskId)
        {
            ViewBag.TaskId = taskId;
            ViewBag.Designs = await _taskService.GetDeviceDesignTemplateFromTaskAsync(taskId);
            ViewBag.ObtainedDesigns = _taskService.GetObtainedDesigns(taskId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveDesign(int TaskId, int[] DesignIds, int[] DesignObt)
        {
            await _taskService.ReceiveDesignsAsync(TaskId, DesignIds, DesignObt);
            return RedirectToAction(nameof(Details), new {id = TaskId});
        }
        
        public static void SortingTasks(IEnumerable<TaskViewModel> tasks, string sortOrder)
        {
            switch (sortOrder)
            {
                case "num_desc":
                    tasks = tasks.OrderByDescending(t => t.Id);
                    break;
                case "Device":
                    tasks = tasks.OrderBy(t => t.Device.Name);
                    break;
                case "device_desc":
                    tasks = tasks.OrderByDescending(t => t.Device.Name);
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
                default:
                    tasks = tasks.OrderBy(t => t.Id);
                    break;
            }
        }

        private IEnumerable<dynamic> GetStates(TaskDTO task)
        {
            if (task == null)
            {
                throw new PageNotFoundException();
            }
            
            var states = new List<object>();
            foreach( StatusEnum status in Enum.GetValues(typeof(StatusEnum)) )
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
