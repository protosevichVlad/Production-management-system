﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Services;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ProductionManagementSystem.Models.Tasks;
using ProductionManagementSystem.Models.Users;
using Task = ProductionManagementSystem.Models.Tasks.Task;

namespace ProductionManagementSystem.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ITaskService _taskService;
        private readonly IDeviceService _deviceService;

        public TasksController(ITaskService taskService, IDeviceService deviceService, UserManager<User> userManager)
        {
            _taskService = taskService;
            _deviceService = deviceService;
            _userManager = userManager;
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


            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var tasks = (await _taskService.GetTasksByUserRoleAsync(roles)).ToList();
            tasks = SortingTasks(tasks, sortOrder).ToList();
            
            return View(tasks);
        }
        
        public async Task<IActionResult> Create()
        {
            ViewBag.Devices = new SelectList(_deviceService.GetAll(), "Id", "Name");
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
                ViewBag.ComponentTemplate = (await _deviceService.GetByIdAsync(task.DeviceId)).Montage;
                ViewBag.DesignTemplate = (await _deviceService.GetByIdAsync(task.DeviceId)).Designs;
                // ViewBag.Logs = _mapper.Map<IEnumerable<LogDTO>, IEnumerable<LogViewModel>>(_taskService.GetLogs(id));
                return View(task);
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
            }
        }
        
        [HttpPost("[controller]/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                await _taskService.DeleteByIdAsync(id);
                return RedirectToAction(nameof(Index));
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
                ViewBag.Devices = new SelectList(_deviceService.GetAll(), "Id", "Name");
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
            ViewBag.TaskId = taskId;
            ViewBag.Components = await _deviceService.GetMontagesFromDeviceByDeviceId(task.DeviceId);
            ViewBag.ObtainedComponents = (await _taskService.GetByIdAsync(task.Id)).ObtainedMontages;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveComponent(int taskId, int[] componentIds, int[] componentObt)
        {
            await _taskService.ReceiveComponentsAsync(taskId, componentIds, componentObt);
            return RedirectToAction(nameof(Details), new {id = taskId});
        }
        
        [HttpGet]
        public async Task<IActionResult> ReceiveDesign(int taskId)
        {
            var task = await _taskService.GetByIdAsync(taskId);
            ViewBag.TaskId = taskId;
            ViewBag.Designs = await _deviceService.GetDesignsFromDeviceByDeviceId(task.DeviceId);
            ViewBag.ObtainedDesigns = (await _taskService.GetByIdAsync(taskId)).ObtainedDesigns;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveDesign(int taskId, int[] designIds, int[] designObt)
        {
            await _taskService.ReceiveDesignsAsync(taskId, designIds, designObt);
            return RedirectToAction(nameof(Details), new {id = taskId});
        }

        private static IEnumerable<Task> SortingTasks(IEnumerable<Task> tasks, string sortOrder)
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
