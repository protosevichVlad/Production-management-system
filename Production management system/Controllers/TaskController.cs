using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [Authorize(Roles = "admin, order_picker, assembler, tuner")]
        public IActionResult Show()
        {
            List<Models.Task> tasks = new List<Models.Task>();
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
            Models.Task task = new Models.Task();
            task.Customer = collection["Customer"];
            task.StartTime = DateTime.Now;
            task.Deadline = DateTime.Parse(collection["Deadline"]);
            task.EndTime = new DateTime();
            task.Status = "Комплектация";

            List<DeviceInTask> devicesInTask = new List<DeviceInTask>();

            int idDevice = 0;
            foreach(var key in collection.Keys)
            {
                if (key.Contains("NameDivice"))
                {
                    int.TryParse(collection[key], out idDevice);
                } else if (key.Contains("Quantity"))
                {
                    int.TryParse(collection[key], out int quantity);
                    devicesInTask.Add(new DeviceInTask
                    {
                        Device = _context.Devices.Where(d => d.Id == idDevice).FirstOrDefault(),
                        Quantity = quantity,
                    });
                }
            }
            task.DevicesInTask = devicesInTask;
            _context.Tasks.Add(task);
            _context.SaveChanges();
            int idTasks = task.Id;
            return Redirect($"/Task/ShowTask/{idTasks}");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult ShowTask(int id)
        {
            ViewBag.Task = _context.Tasks
                .Include(t => t.DevicesInTask)
                .ThenInclude(d => d.Device)
                .Where(t => t.Id == id).FirstOrDefault();

            ViewBag.Devices = GetAllDeviceFromTask(id, out List<int> quantytiDevicesInTask);        
            ViewBag.QuantytiDevices = quantytiDevicesInTask;

            ViewBag.DesignTemplate = GetAllDeviceDesignTemplateFromTask(id);
            ViewBag.ComponentTemplate = GetAllDeviceComponentsTemplateFromTask(id);
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult NextStage(int taskId)
        {
            Models.Task task = _context.Tasks
                .Where(t => t.Id == taskId).FirstOrDefault();

            if (task.Status.Contains("Комплектация"))
            {
                if (IsAllComponentsEnough(taskId))
                {
                    task.Status = "Монтаж";
                } else
                {
                    task.Status += ", монтаж";

                }
            } else if (task.Status.Contains("Монтаж"))
            {
                
            }
            _context.SaveChanges();
            return Redirect($"/Task/ShowTask/{taskId}");
        }

        private List<DeviceDesignTemplate> GetAllDeviceDesignTemplateFromTask(int taskId)
        {
            List<Device> devices = GetAllDeviceFromTask(taskId, out List<int> quantytiDevicesInTask);

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
                        designs[^1].Quantity *= quantytiDevicesInTask[indexDevice];
                    }
                    else
                    {
                        int index = designsIds.IndexOf(designTemplate.Design.Id);
                        designs[index].Quantity += designTemplate.Quantity * quantytiDevicesInTask[indexDevice];
                    }
                }

                indexDevice++;
            }

            return designs;
        }
        
        private List<DeviceComponentsTemplate> GetAllDeviceComponentsTemplateFromTask(int taskId)
        {
            List<Device> devices = GetAllDeviceFromTask(taskId, out List<int> quantytiDevicesInTask);

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
                        components[^1].Quantity *= quantytiDevicesInTask[indexDevice];
                    }
                    else
                    {
                        int index = componentsIds.IndexOf(componentTemplate.Component.Id);
                        components[index].Quantity += componentTemplate.Quantity * quantytiDevicesInTask[indexDevice];
                    }
                }

                indexDevice++;
            }

            return components;
        }

        private List<Device> GetAllDeviceFromTask(int taskId, out List<int> quantytiDevicesInTask)
        {
            quantytiDevicesInTask = new List<int>();

            Models.Task task = _context.Tasks
                                .Include(t => t.DevicesInTask)
                                .ThenInclude(d => d.Device)
                                .Where(t => t.Id == taskId).FirstOrDefault();

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
                    .ThenInclude(d => d.Design)
                    .Where(d => d.Id == idDevice).FirstOrDefault());
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
    }
}
