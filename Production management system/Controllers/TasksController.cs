using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.ViewModels;

namespace ProductionManagementSystem.Controllers
{
    public class TasksController : Controller
    {
        private ApplicationContext _context;

        public TasksController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Tasks
                .Include(t => t.Device)
                .ToList());
        }
        
        public IActionResult Create()
        {
            ViewBag.Devices = new SelectList(_context.Devices.ToList(), "Id", "Name");
            return View();
        }
        
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Create(TaskViewModel taskModel)
        {
            var task = new Task();
            task.Deadline = taskModel.Deadline;
            task.StartTime = DateTime.Today;
            task.Description = taskModel.Description;
            task.EndTime = new DateTime();
            task.Status = "Комплектация";
            task.Device = await _context.Devices
                .FirstOrDefaultAsync(d => d.Id == taskModel.DeviceId);

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            
            int taskId = task.Id;

            var compInTask = GetAllDeviceComponentsTemplateFromTask(taskId);

            foreach (var componentTemplate in compInTask)
            {
                _context.ObtainedСomponents.Add(new ObtainedСomponent
                {
                    Component = componentTemplate.Component,
                    Obtained = 0,
                    Task = task,
                });
            }
            
            var desInTask = GetAllDeviceDesignTemplateFromTask(taskId);

            foreach (var designTemplate in desInTask)
            {
                _context.ObtainedDesigns.Add(new ObtainedDesign
                {
                    Design = designTemplate.Design,
                    Obtained = 0,
                    Task = task,
                });
            }

            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var task = _context.Tasks
                .Include(t => t.Device)
                .FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            TaskViewModel taskModel = new TaskViewModel()
            {
                Id = task.Id,
                Description = task.Description,
                Deadline = task.Deadline,
                Device = task.Device,
                Status = task.Status,
                DesignTemplate = GetAllDeviceDesignTemplateFromTask((int) id),
                ComponentTemplate = GetAllDeviceComponentsTemplateFromTask((int) id),
                ObtainedComponents = GetObtainedСomponents((int) id),
                ObtainedDesigns = GetObtainedDesigns((int) id)
            };
            
            return View(taskModel);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var task = _context.Tasks
                .Include(t => t.Device)
                .FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            
            return View(task);
        }
        
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var task = _context.Tasks
                .Include(t => t.Device)
                .FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            
            ViewBag.Devices = new SelectList(_context.Devices.ToList(), "Id", "Name");
            return View(new TaskViewModel()
            {
                DeviceId = task.Device.Id, 
                Deadline = task.Deadline,
                Description = task.Description,
                Id = task.Id
            });
        }
        
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Edit(TaskViewModel taskModel)
        {
            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == taskModel.Id);
            if (task == null)
            {
                return NotFound();
            }
            
            task.Deadline = taskModel.Deadline;
            task.Description = taskModel.Description;
            task.Device = await _context.Devices
                .FirstOrDefaultAsync(d => d.Id == taskModel.DeviceId);

            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        
        private List<DeviceDesignTemplate> GetAllDeviceDesignTemplateFromTask(int taskId)
        {
            Device device = _context.Tasks
                .Include(t => t.Device)
                .ThenInclude(d => d.DeviceDesignTemplate)
                .ThenInclude(ddt => ddt.Design)
                .FirstOrDefault(t => t.Id == taskId)?.Device;

            if (device == null)
            {
                return new List<DeviceDesignTemplate>();
            }
            
            List<DeviceDesignTemplate> designs = new List<DeviceDesignTemplate>();

            foreach (DeviceDesignTemplate designTemplate in device.DeviceDesignTemplate)
            {
                designs.Add(new DeviceDesignTemplate
                {
                    Design = designTemplate.Design,
                    Description = designTemplate.Description,
                    Quantity = designTemplate.Quantity
                });
            }

            return designs;
        }
        
        private List<DeviceComponentsTemplate> GetAllDeviceComponentsTemplateFromTask(int taskId)
        {
            Device device = _context.Tasks
                .Include(t => t.Device)
                .ThenInclude(d => d.DeviceComponentsTemplate)
                .ThenInclude(dct => dct.Component)
                .FirstOrDefault(t => t.Id == taskId)?.Device;

            if (device == null)
            {
                return new List<DeviceComponentsTemplate>();
            }
            
            List<DeviceComponentsTemplate> components = new List<DeviceComponentsTemplate>();

            foreach (DeviceComponentsTemplate designTemplate in device.DeviceComponentsTemplate)
            {
                components.Add(new DeviceComponentsTemplate
                {
                    Component = designTemplate.Component,
                    Description = designTemplate.Description,
                    Quantity = designTemplate.Quantity
                });
            }

            return components;
        }

        private bool IsAllComponentsEnough(int taskId)
        {
            return IsAllComponentsEnough(GetAllDeviceDesignTemplateFromTask(taskId), GetAllDeviceComponentsTemplateFromTask(taskId));
        }

        private bool IsAllComponentsEnough(List<DeviceDesignTemplate> designs, List<DeviceComponentsTemplate> components)
        {
            foreach (var design in designs)
            {
                if (design.Design.Quantity < design.Quantity)
                {
                    return false;
                }
            }

            foreach (var component in components)
            {
                if (component.Component.Quantity < component.Quantity)
                {
                    return false;
                }
            }

            return true;
        }

        [HttpGet]
        public IActionResult ReceiveComponent(int taskId)
        {
            ViewBag.TaskId = taskId;
            ViewBag.Components = GetAllDeviceComponentsTemplateFromTask(taskId);
            ViewBag.ObtainedComponents = GetObtainedСomponents(taskId);
            return View();
        }

        private List<ObtainedСomponent> GetObtainedСomponents(int taskId)
        {
            return _context.ObtainedСomponents.
                Include(c => c.Task).
                Include(c => c.Component).
                Where(c => c.Task.Id == taskId).ToList();
        }
        
        [HttpPost]
        public IActionResult ReceiveComponent(int TaskId, int[] ComponentIds, int[] ComponentObt)
        {
            List<ObtainedСomponent> obtainedComp = GetObtainedСomponents(TaskId);
            
            for (int i = 0; i < ComponentObt.Length; i++)
            {
                var obtComp = obtainedComp.FirstOrDefault(c => c.Id == ComponentIds[i]);
                if (obtComp != null)
                {
                    obtComp.Obtained += ComponentObt[i];
                    obtComp.Component.Quantity -= ComponentObt[i];
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new {id = TaskId});
        }
        
        [HttpGet]
        public IActionResult ReceiveDesign(int taskId)
        {
            ViewBag.TaskId = taskId;
            ViewBag.Designs = GetAllDeviceDesignTemplateFromTask(taskId);
            ViewBag.ObtainedDesigns = GetObtainedDesigns(taskId);
            return View();
        }

        private List<ObtainedDesign> GetObtainedDesigns(int taskId)
        {
            return _context.ObtainedDesigns.
                Include(d => d.Task).
                Include(d => d.Design).
                Where(d => d.Task.Id == taskId).ToList();
        }
        
        [HttpPost]
        public IActionResult ReceiveDesign(int TaskId, int[] DesignIds, int[] DesignObt)
        {
            List<ObtainedDesign> obtainedDes = GetObtainedDesigns(TaskId);
            
            for (int i = 0; i < DesignObt.Length; i++)
            {
                
                var obtDes = obtainedDes.FirstOrDefault(c => c.Id == DesignIds[i]);
                if (obtDes != null)
                {
                    obtDes.Obtained += DesignObt[i];
                    obtDes.Design.Quantity -= DesignObt[i];
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new {id = TaskId});
        }
    }
}
