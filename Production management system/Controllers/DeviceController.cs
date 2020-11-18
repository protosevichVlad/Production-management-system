using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class DeviceController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public DeviceController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Show()
        {
            var db = new ApplicationContext();
            ViewBag.Devices = db.Devices;
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            var db = new ApplicationContext();
            ViewBag.Components = db.Components.OrderBy(c => c.Name);
            ViewBag.Designs = db.Designs.OrderBy(d => d.Name);
            return View();
        }

        [HttpPost]
        public IActionResult Add(IFormCollection collection)
        {
            var db = new ApplicationContext();

            List<DeviceComponentsTemplate> componentsTemplate = new List<DeviceComponentsTemplate>();
            List<DeviceDesignTemplate> designTemplate = new List<DeviceDesignTemplate>();

            int idComponent = 0;
            int idDesign = 0;

            Device device = new Device();
            foreach (var key in collection.Keys)
            {
                if (key == "Name")
                {
                    device.Name = collection[key];
                }
                else if (key == "Quantity")
                {
                    if (int.TryParse(collection[key], out int quantity))
                    {
                        device.Quantity = quantity;
                    }
                    else
                    {
                        device.Quantity = 0;
                    }
                }
                else if (key.Contains("Component"))
                {
                    if (key.Contains("Input"))
                    {
                        if (int.TryParse(collection[key], out int quantity))
                        {
                            componentsTemplate.Add(new DeviceComponentsTemplate
                            {
                                Component = db.Components.Where(c => c.Id == idComponent).FirstOrDefault(),
                                Quantity = quantity,
                            });
                        }
                        else
                        {
                            componentsTemplate.Add(new DeviceComponentsTemplate
                            {
                                Component = db.Components.Where(c => c.Id == idComponent).FirstOrDefault(),
                                Quantity = 0,
                            });
                        }
                        
                    }
                    else
                    {
                        if (int.TryParse(collection[key], out int id))
                        {
                            idComponent = id;
                        }
                        else
                        {
                            throw new ArgumentException("id");
                        }
                    }
                }
                else if (key.Contains("Design"))
                {
                    if (key.Contains("Input"))
                    {
                        if (int.TryParse(collection[key], out int quantity))
                        {
                            designTemplate.Add(new DeviceDesignTemplate
                            {
                                Design = db.Designs.Where(c => c.Id == idDesign).FirstOrDefault(),
                                Quantity = quantity,
                            });
                        }
                        else
                        {
                            designTemplate.Add(new DeviceDesignTemplate
                            {
                                Design = db.Designs.Where(c => c.Id == idDesign).FirstOrDefault(),
                                Quantity = 0,
                            });
                        }
                        
                    }
                    else
                    {
                        if (int.TryParse(collection[key], out int id))
                        {
                            idDesign = id;
                        }
                        else
                        {
                            throw new ArgumentException("id");
                        }
                    }
                }
            }
            device.DeviceComponentsTemplate = componentsTemplate;
            device.DeviceDesignTemplate = designTemplate;
            db.Devices.Add(device);
            db.SaveChanges();
            return Redirect("/Device/Show");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var db = new ApplicationContext();
            ViewBag.Device = db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design)
                .Where(d => d.Id == id).FirstOrDefault();
            ViewBag.Components = db.Components.OrderBy(c => c.Name);
            ViewBag.Designs = db.Designs.OrderBy(d => d.Name);
            return View();
        }

        [HttpPost]
        public IActionResult Edit(IFormCollection collection)
        {
            var db = new ApplicationContext();

            List<DeviceComponentsTemplate> componentsTemplate = new List<DeviceComponentsTemplate>();
            List<DeviceDesignTemplate> designTemplate = new List<DeviceDesignTemplate>();

            int idComponent = 0;
            int idDesign = 0;

            int idDevice = 0;
            if (!int.TryParse(collection["Id"], out idDevice))
            {

            }

            Device device = db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design)
                .Where(d => d.Id == idDevice).FirstOrDefault();

            foreach (var key in collection.Keys)
            {
                if (key == "Name")
                {
                    device.Name = collection[key];
                }
                else if (key == "Quantity")
                {
                    if (int.TryParse(collection[key], out int quantity))
                    {
                        device.Quantity = quantity;
                    }
                    else
                    {
                        device.Quantity = 0;
                    }
                }
                else if (key.Contains("Component"))
                {
                    if (key.Contains("Input"))
                    {
                        if (int.TryParse(collection[key], out int quantity))
                        {
                            componentsTemplate.Add(new DeviceComponentsTemplate
                            {
                                Component = db.Components.Where(c => c.Id == idComponent).FirstOrDefault(),
                                Quantity = quantity,
                            });
                        }
                        else
                        {
                            componentsTemplate.Add(new DeviceComponentsTemplate
                            {
                                Component = db.Components.Where(c => c.Id == idComponent).FirstOrDefault(),
                                Quantity = 0,
                            });
                        }

                    }
                    else
                    {
                        if (int.TryParse(collection[key], out int id))
                        {
                            idComponent = id;
                        }
                        else
                        {
                            throw new ArgumentException("id");
                        }
                    }
                }
                else if (key.Contains("Design"))
                {
                    if (key.Contains("Input"))
                    {
                        if (int.TryParse(collection[key], out int quantity))
                        {
                            designTemplate.Add(new DeviceDesignTemplate
                            {
                                Design = db.Designs.Where(c => c.Id == idDesign).FirstOrDefault(),
                                Quantity = quantity,
                            });
                        }
                        else
                        {
                            designTemplate.Add(new DeviceDesignTemplate
                            {
                                Design = db.Designs.Where(c => c.Id == idDesign).FirstOrDefault(),
                                Quantity = 0,
                            });
                        }

                    }
                    else
                    {
                        if (int.TryParse(collection[key], out int id))
                        {
                            idDesign = id;
                        }
                        else
                        {
                            throw new ArgumentException("id");
                        }
                    }
                }
            }
            device.DeviceComponentsTemplate = componentsTemplate;
            device.DeviceDesignTemplate = designTemplate;
            db.SaveChanges();
            return Redirect($"/Device/ShowDevice/{idDevice}");
        }

        [HttpGet]
        public IActionResult ShowDevice(int id)
        {
            var db = new ApplicationContext();
            ViewBag.Device = db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design)
                .Where(d => d.Id == id).FirstOrDefault();
            return View();
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            var db = new ApplicationContext();
            var device = db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design)
                .Where(d => d.Id == id).FirstOrDefault();

            db.Devices.Attach(device);
            db.Devices.Remove(device);

            db.SaveChanges();
            return Redirect("/Device/Show");
        }
    }
    
}
