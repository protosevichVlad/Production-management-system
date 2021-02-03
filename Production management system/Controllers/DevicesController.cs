using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class DevicesController : Controller
    {
        private ApplicationContext _context;

        public DevicesController()
        {
            _context = new ApplicationContext();
        }

        [Authorize(Roles = "admin")]
        public IActionResult Index(string sortOrder)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            var devices = from s in _context.Devices
                select s;
            switch (sortOrder)
            {
                case "name_desc":
                    devices = devices.OrderByDescending(d => d.Name);
                    break;
                case "Name":
                    devices = devices.OrderBy(d => d.Name);
                    break;
                case "num_desc":
                    devices = devices.OrderByDescending(d => d.Id);
                    break;
                case "Quantity":
                    devices = devices.OrderBy(d => d.Quantity);
                    break;
                case "quantity_desc":
                    devices = devices.OrderByDescending(d => d.Quantity);
                    break;
                default:
                    devices = devices.OrderBy(d => d.Id);
                    break;
            }
            return View(devices.ToList());
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            List<Design> designs = _context.Designs.OrderBy(d => d.Name).ToList();
            designs.ForEach((d) => {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(d.Name);
                d.Name = System.Text.Encoding.UTF8.GetString(bytes);
            });

            List<Component> components = _context.Components.OrderBy(c => c.Name).ToList();
            components.ForEach((c) => {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(c.Name);
                c.Name = System.Text.Encoding.UTF8.GetString(bytes);
            });

            ViewBag.Designs = designs;
            ViewBag.Components = components;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Create(IFormCollection collection)
        {
            List<DeviceComponentsTemplate> componentsTemplate = new List<DeviceComponentsTemplate>();
            List<DeviceDesignTemplate> designTemplate = new List<DeviceDesignTemplate>();

            int idComponent = 0;
            int idDesign = 0;

            int quantity = 0;
            Device device = new Device();
            foreach (var key in collection.Keys)
            {

                if (key == "Name")
                {
                    device.Name = collection[key];
                }
                else if (key == "Quantity")
                {
                    if (int.TryParse(collection[key], out int q))
                    {
                        device.Quantity = q;
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
                        if (!int.TryParse(collection[key], out quantity))
                        {
                            quantity = 0;
                        }
                    }
                    else if (key.Contains("Text"))
                    {
                        componentsTemplate.Add(new DeviceComponentsTemplate
                        {
                            Component = _context.Components.Where(c => c.Id == idComponent).FirstOrDefault(),
                            Quantity = quantity,
                            Description = collection[key],
                        });
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
                        if (!int.TryParse(collection[key], out quantity))
                        {
                            quantity = 0;
                        }
                    }
                    else if (key.Contains("Text"))
                    {
                        designTemplate.Add(new DeviceDesignTemplate
                        {
                            Design = _context.Designs.Where(c => c.Id == idDesign).FirstOrDefault(),
                            Quantity = quantity,
                            Description = collection[key],
                        });
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
            _context.Devices.Add(device);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            ViewBag.Device = _context.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design).FirstOrDefault(d => d.Id == id);

            List<Design> designs = _context.Designs.OrderBy(d => d.Name).ToList();
            designs.ForEach((d) => {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(d.Name);
                d.Name = System.Text.Encoding.UTF8.GetString(bytes);
            });

            List<Component> components = _context.Components.OrderBy(c => c.Name).ToList();
            components.ForEach((c) => {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(c.Name);
                c.Name = System.Text.Encoding.UTF8.GetString(bytes);
            });

            ViewBag.Designs = designs;
            ViewBag.Components = components;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(IFormCollection collection)
        {
            List<DeviceComponentsTemplate> componentsTemplate = new List<DeviceComponentsTemplate>();
            List<DeviceDesignTemplate> designTemplate = new List<DeviceDesignTemplate>();

            int idComponent = 0;
            int idDesign = 0;
            int quantity = 0;

            if (!int.TryParse(collection["Id"], out var idDevice))
            {

            }

            Device device = _context.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design).FirstOrDefault(d => d.Id == idDevice);

            if (device == null)
            {
                return NotFound();
            }

            foreach (var key in collection.Keys)
            {
                if (key == "Name")
                {
                    device.Name = collection[key];
                }
                else if (key == "Quantity")
                {
                    if (int.TryParse(collection[key], out quantity))
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
                        if (!int.TryParse(collection[key], out quantity))
                        {
                            quantity = 0;
                        }
                    }
                    else if (key.Contains("Text"))
                    {
                        componentsTemplate.Add(new DeviceComponentsTemplate
                        {
                            Component = _context.Components.Where(c => c.Id == idComponent).FirstOrDefault(),
                            Quantity = quantity,
                            Description = collection[key],
                        });
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
                        if (!int.TryParse(collection[key], out quantity))
                        {
                            quantity = 0;
                        }
                    }
                    else if (key.Contains("Text"))
                    {
                        designTemplate.Add(new DeviceDesignTemplate
                        {
                            Design = _context.Designs.Where(c => c.Id == idDesign).FirstOrDefault(),
                            Quantity = quantity,
                            Description = collection[key],
                        });
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
            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = idDevice });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Details(int id)
        {
            ViewBag.Device = _context.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design).FirstOrDefault(d => d.Id == id);
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design).FirstOrDefaultAsync(d => d.Id == id);
            
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Designs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design).FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound();
            }
            
            _context.Devices.Attach(device);
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult GetAllDevices()
        {
            List<Device> devices = _context.Devices.OrderBy(d => d.Name).ToList();
            return Json(devices);
        }
    }
    
}
