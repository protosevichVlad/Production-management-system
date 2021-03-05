using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

namespace ProductionManagementSystem.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IDeviceService _deviceService;
        private IMapper _mapperToViewModel;
        private IMapper _mapperFromViewModel;

        public TasksController(ITaskService taskService, IDeviceService deviceService)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _mapperToViewModel = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TaskDTO, TaskViewModel>()
                        .ForMember(
                            task => task.Status, 
                            opt => opt.MapFrom(
                                src => _taskService.GetTaskStatusName(src.Status)
                                )
                            );
                    cfg.CreateMap<DeviceDTO, DeviceViewModel>();
                    cfg.CreateMap<LogDTO, LogViewModel>();
                })
                .CreateMapper();
            _mapperFromViewModel = new MapperConfiguration(cfg => cfg.CreateMap<TaskViewModel, TaskDTO>())
                .CreateMapper();
        }

        public IActionResult Index(string sortOrder, string searchString)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["DeviceSortParm"] = sortOrder == "Device" ? "device_desc" : "Device";
            ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["OrderIdSortParm"] = sortOrder == "OrderId" ? "orderid_desc" : "OrderId";

            var tasksDto = _taskService.GetTasks().ToList();
            var tasksViewModel =
                _mapperToViewModel.Map<IEnumerable<TaskDTO>, IEnumerable<TaskViewModel>>(tasksDto).ToList();
            
            SortingTasks(tasksViewModel, sortOrder);
            
            ViewData["CurrentFilter"] = searchString;
            
            return View(tasksViewModel);
        }
        
        public IActionResult Create()
        {
            ViewBag.Devices = new SelectList(_deviceService.GetDevices(), "Id", "Name");
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(TaskViewModel taskViewModel)
        {
            var taskDto = _mapperFromViewModel.Map<TaskViewModel, TaskDTO>(taskViewModel);
            _taskService.CreateTask(taskDto);
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Details(int? id)
        {
            try
            {
                var taskDto = _taskService.GetTask(id);
                var taskViewModel = _mapperToViewModel.Map<TaskDTO, TaskViewModel>(taskDto);
                ViewBag.States = new SelectList(GetStates(taskDto), "Id", "Name");
                ViewBag.ComponentTemplate = _deviceService.GetComponentsTemplates(taskDto.DeviceId);
                ViewBag.DesignTemplate = _deviceService.GetDesignTemplates(taskDto.DeviceId);
                ViewBag.Logs = _taskService.GetLogs(id);
                return View(taskViewModel);
            }
            catch (PageNotFoundException e)
            {
                return NotFound();
            }
        }
        
        public IActionResult Delete(int? id)
        {
            try
            {
                var taskDto = _taskService.GetTask(id);
                var taskViewModel = _mapperToViewModel.Map<TaskDTO, TaskViewModel>(taskDto);
                return View(taskViewModel);
            }
            catch (PageNotFoundException e)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            try
            {
                _taskService.DeleteTask(Id);
                return RedirectToAction(nameof(Index));
            }
            catch (PageNotFoundException e)
            {
                return NotFound();
            }
        }
        
        public IActionResult Edit(int? id)
        {
            try
            {
                var taskDto = _taskService.GetTask(id);
                var taskViewModel = _mapperToViewModel.Map<TaskDTO, TaskViewModel>(taskDto);
                ViewBag.Devices = new SelectList(_deviceService.GetDevices(), "Id", "Name");
                return View(taskViewModel);
            }
            catch (PageNotFoundException e)
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public IActionResult Edit(TaskViewModel taskModel)
        {
            var taskDto = _mapperFromViewModel.Map<TaskViewModel, TaskDTO>(taskModel);
            _taskService.EditTask(taskDto);
            return RedirectToAction(nameof(Details), new {id = taskModel.Id});
        }
        
        [HttpPost]
        public IActionResult Transfer(int taskId, string full, int to, string message)
        {
            LogService.UserName = User.Identity?.Name;
            _taskService.Transfer(taskId, full == "true", to, message);
            return RedirectToAction(nameof(Details), new {id = taskId});
        }

        [HttpGet]
        public IActionResult ReceiveComponent(int taskId)
        {
            ViewBag.TaskId = taskId;
            ViewBag.Components = _taskService.GetDeviceComponentsTemplatesFromTask(taskId);
            ViewBag.ObtainedComponents = _taskService.GetObtainedСomponents(taskId);
            return View();
        }

        [HttpPost]
        public IActionResult ReceiveComponent(int TaskId, int[] ComponentIds, int[] ComponentObt)
        {
            _taskService.ReceiveComponent(TaskId, ComponentIds, ComponentObt);
            return RedirectToAction(nameof(Details), new {id = TaskId});
        }
        
        [HttpGet]
        public IActionResult ReceiveDesign(int taskId)
        {
            ViewBag.TaskId = taskId;
            ViewBag.Designs = _taskService.GetDeviceDesignTemplateFromTask(taskId);
            ViewBag.ObtainedDesigns = _taskService.GetObtainedDesigns(taskId);
            return View();
        }

        [HttpPost]
        public IActionResult ReceiveDesign(int TaskId, int[] DesignIds, int[] DesignObt)
        {
            _taskService.ReceiveDesign(TaskId, DesignIds, DesignObt);
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
