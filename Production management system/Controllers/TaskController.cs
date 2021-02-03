using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace ProductionManagementSystem.Controllers
{
    public class TaskController : Controller
    {
        private ApplicationContext _context;

        public TaskController()
        {
            _context = new ApplicationContext();
        }

        [Authorize]
        public IActionResult Show()
        {
            List<Task> tasks = new List<Task>();
            if (User.IsInRole("admin"))
            {
                tasks = _context.Tasks.ToList();
            } 
            else if (User.IsInRole("order_picker"))
            {
                tasks = _context.Tasks.Where(t => t.Status.Contains("Комплектация")).ToList();
            }
            else if (User.IsInRole("assembler"))
            {
                tasks = _context.Tasks.Where(t => t.Status.Contains("Монтаж") || t.Status.Contains("монтаж")).ToList();
            }

            return View(tasks);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            ViewBag.Devices = _context.Devices;
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(IFormCollection collection)
        {
            Task task = new Task();
            task.Customer = collection["Customer"];
            task.StartTime = DateTime.Now;
            task.Deadline = DateTime.Parse(collection["Deadline"]);
            task.EndTime = new DateTime();
            task.Status = "Комплектация";

            List<DeviceInTask> devicesInTask = new List<DeviceInTask>();

            int idDevice = 0;
            foreach(var key in collection.Keys)
            {
                if (key.Contains("Id"))
                {
                    int.TryParse(collection[key], out idDevice);
                } else if (key.Contains("Quantity"))
                {
                    int.TryParse(collection[key], out int quantity);
                    devicesInTask.Add(new DeviceInTask
                    {
                        Device = _context.Devices.FirstOrDefault(d => d.Id == idDevice),
                        Quantity = quantity,
                    });
                }
            }
            task.DevicesInTask = devicesInTask;
            _context.Tasks.Add(task);
            _context.SaveChanges();
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
            
            return Redirect($"/Task/ShowTask/{taskId}");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ShowTask(int id)
        {
            ViewBag.Task = _context.Tasks
                .Include(t => t.DevicesInTask)
                .ThenInclude(d => d.Device).FirstOrDefault(t => t.Id == id);

            ViewBag.Devices = GetAllDeviceFromTask(id, out List<int> quantityDevicesInTask);        
            ViewBag.QuantytiDevices = quantityDevicesInTask;

            ViewBag.DesignTemplate = GetAllDeviceDesignTemplateFromTask(id);
            ViewBag.ComponentTemplate = GetAllDeviceComponentsTemplateFromTask(id);
            ViewBag.ObtainedComponents = GetObtainedСomponents(id);
            ViewBag.ObtainedDesigns = GetObtainedDesigns(id);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker, assembler")]
        public IActionResult NextStage(int taskId)
        {
            Task task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);

            if (task == null)
            {
                return Redirect($"/Task/Show");
            }
            
            if (task.Status.Contains("Комплектация"))
            {
                if (IsAllComponentsEnough(taskId))
                {
                    task.Status = "Монтаж";
                } 
                else
                {
                    if (!task.Status.Contains("монтаж"))
                    {
                        task.Status += ", монтаж";
                    }
                }
            } 
            else if (task.Status.Contains("Монтаж"))
            {
                task.Status = "Настройка";
            }
            else if (task.Status.Contains("Настройка"))
            {
                task.Status = "Сборка";
            }
            else if (task.Status.Contains("Сборка"))
            {
                task.Status = "Проверка";
            }
            else if (task.Status.Contains("Проверка"))
            {
                task.Status = "Отправка";
            }
            else if (task.Status.Contains("Отправка"))
            {
                task.Status = "Задача выполнена";
                task.EndTime = DateTime.Now;
            }
            
            _context.SaveChanges();
            return Redirect($"/Task/ShowTask/{taskId}");
        }

        private List<DeviceDesignTemplate> GetAllDeviceDesignTemplateFromTask(int taskId)
        {
            List<Device> devices = GetAllDeviceFromTask(taskId, out List<int> quantityDevicesInTask);

            List<DeviceDesignTemplate> designs = new List<DeviceDesignTemplate>();
            List<int> designsIds = new List<int>();

            int indexDevice = 0;
            foreach (Device device in devices)
            {
                foreach (DeviceDesignTemplate designTemplate in device.DeviceDesignTemplate)
                {
                    if (!designsIds.Contains(designTemplate.Design.Id))
                    {
                        designsIds.Add(designTemplate.Design.Id);
                        designs.Add(new DeviceDesignTemplate
                        {
                            Design = designTemplate.Design,
                            Description = designTemplate.Description,
                            Quantity = designTemplate.Quantity
                        });
                        designs[^1].Quantity *= quantityDevicesInTask[indexDevice];
                    }
                    else
                    {
                        int index = designsIds.IndexOf(designTemplate.Design.Id);
                        designs[index].Quantity += designTemplate.Quantity * quantityDevicesInTask[indexDevice];
                    }
                }

                indexDevice++;
            }

            return designs;
        }
        
        private List<DeviceComponentsTemplate> GetAllDeviceComponentsTemplateFromTask(int taskId)
        {
            List<Device> devices = GetAllDeviceFromTask(taskId, out List<int> quantityDevicesInTask);

            List<DeviceComponentsTemplate> components = new List<DeviceComponentsTemplate>();
            List<int> componentsIds = new List<int>();

            int indexDevice = 0;
            foreach (Device device in devices)
            {
                foreach (DeviceComponentsTemplate componentTemplate in device.DeviceComponentsTemplate)
                {
                    if (!componentsIds.Contains(componentTemplate.Component.Id))
                    {
                        componentsIds.Add(componentTemplate.Component.Id);
                        components.Add(new DeviceComponentsTemplate
                        {
                            Component = componentTemplate.Component,
                            Description = componentTemplate.Description,
                            Quantity = componentTemplate.Quantity
                        });
                        components[^1].Quantity *= quantityDevicesInTask[indexDevice];
                    }
                    else
                    {
                        int index = componentsIds.IndexOf(componentTemplate.Component.Id);
                        components[index].Quantity += componentTemplate.Quantity * quantityDevicesInTask[indexDevice];
                    }
                }

                indexDevice++;
            }

            return components;
        }

        private List<Device> GetAllDeviceFromTask(int taskId, out List<int> quantytiDevicesInTask)
        {
            quantytiDevicesInTask = new List<int>();

            Task task = _context.Tasks
                .Include(t => t.DevicesInTask)
                .ThenInclude(d => d.Device).FirstOrDefault(t => t.Id == taskId);

            if (task is null)
            {
                return new List<Device>();
            }
            
            List<int> idsDevices = new List<int>();

            foreach (var d in task.DevicesInTask)
            {
                idsDevices.Add(d.Device.Id);
                quantytiDevicesInTask.Add(d.Quantity);
            }

            List<Device> devices = new List<Device>();
            foreach (var idDevice in idsDevices)
            {
                devices.Add(_context.Devices
                    .Include(d => d.DeviceComponentsTemplate)
                    .ThenInclude(d => d.Component)
                    .Include(d => d.DeviceDesignTemplate)
                    .ThenInclude(d => d.Design).FirstOrDefault(d => d.Id == idDevice));
            }

            return devices;
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
        public IActionResult ReceiveComponent(IFormCollection collection)
        {
            int taskId = 0;

            List<ObtainedСomponent> obtainedComp = null;
            
            foreach (var key in collection.Keys)
            {
                if (key.Contains("TaskId"))
                {
                    int.TryParse(collection[key], out taskId);
                    obtainedComp = GetObtainedСomponents(taskId);
                }
                else
                {
                    int.TryParse(collection[key], out int obtained);
                    int.TryParse(key, out int keyInt);
                    
                    var obtComp = obtainedComp.FirstOrDefault(c => c.Id == keyInt);
                    if (obtained != 0 && obtComp != null)
                    {
                        obtComp.Obtained += obtained;
                        obtComp.Component.Quantity -= obtained;
                    }
                }
            }

            _context.SaveChanges();
            return Redirect($"/Task/ReceiveComponent?taskId={taskId}");
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
        public IActionResult ReceiveDesign(IFormCollection collection)
        {
            int taskId = 0;

            List<ObtainedDesign> obtainedDes = null;
            
            foreach (var key in collection.Keys)
            {
                if (key.Contains("TaskId"))
                {
                    int.TryParse(collection[key], out taskId);
                    obtainedDes = GetObtainedDesigns(taskId);
                }
                else
                {
                    int.TryParse(collection[key], out int obtained);
                    int.TryParse(key, out int keyInt);
                    
                    var obtDes = obtainedDes.FirstOrDefault(c => c.Id == keyInt);
                    if (obtained != 0 && obtDes != null)
                    {
                        obtDes.Obtained += obtained;
                        obtDes.Design.Quantity -= obtained;
                    }
                }
            }

            _context.SaveChanges();
            return Redirect($"/Task/ReceiveDesign?taskId={taskId}");
        }
    }
}
