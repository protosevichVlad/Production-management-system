using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;
using ProductionManagementSystem.ViewModels;

namespace ProductionManagementSystem.Controllers
{
    public class DevicesController : Controller
    {
        private ApplicationContext _context;

        public DevicesController(ApplicationContext context)
        {
            _context = context;
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
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Create(DeviceViewModel deviceViewModel)
        {
            Device device = new Device();
            device.Name = deviceViewModel.Name;
            device.Quantity = deviceViewModel.Quantity;
            device.DeviceDesignTemplate = new List<DeviceDesignTemplate>();
            device.DeviceComponentsTemplate = new List<DeviceComponentsTemplate>();

            for (int i = 0; i < deviceViewModel.ComponentIds.Length; i++)
            {
                device.DeviceComponentsTemplate.Add(new DeviceComponentsTemplate
                {
                    Component =
                        _context.Components.Where(c => c.Id == deviceViewModel.ComponentIds[i]).FirstOrDefault(),
                    Quantity = deviceViewModel.ComponentQuantity[i],
                    Description = deviceViewModel.ComponentDescriptions[i],
                });
            }
            
            for (int i = 0; i < deviceViewModel.DesignIds.Length; i++)
            {
                device.DeviceDesignTemplate.Add(new DeviceDesignTemplate
                {
                    Design =
                        _context.Designs.Where(c => c.Id == deviceViewModel.DesignIds[i]).FirstOrDefault(),
                    Quantity = deviceViewModel.DesignQuantity[i],
                    Description = deviceViewModel.DesignDescriptions[i],
                });
            }
            
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
        public IActionResult Edit(DeviceViewModel deviceViewModel)
        {
            Device device = _context.Devices
                .Include(d => d.DeviceComponentsTemplate)
                    .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                    .ThenInclude(d => d.Design)
                .FirstOrDefault(d => d.Id == deviceViewModel.Id);

            if (device == null)
            {
                return NotFound();
            }

            device.Name = deviceViewModel.Name;
            device.Quantity = deviceViewModel.Quantity;
            device.DeviceDesignTemplate = new List<DeviceDesignTemplate>();
            device.DeviceComponentsTemplate = new List<DeviceComponentsTemplate>();
            
            for (int i = 0; i < deviceViewModel.ComponentIds.Length; i++)
            {
                device.DeviceComponentsTemplate.Add(new DeviceComponentsTemplate
                {
                    Component =
                        _context.Components.Where(c => c.Id == deviceViewModel.ComponentIds[i]).FirstOrDefault(),
                    Quantity = deviceViewModel.ComponentQuantity[i],
                    Description = deviceViewModel.ComponentDescriptions[i],
                });
            }
            
            for (int i = 0; i < deviceViewModel.DesignIds.Length; i++)
            {
                device.DeviceDesignTemplate.Add(new DeviceDesignTemplate
                {
                    Design =
                        _context.Designs.Where(c => c.Id == deviceViewModel.DesignIds[i]).FirstOrDefault(),
                    Quantity = deviceViewModel.DesignQuantity[i],
                    Description = deviceViewModel.DesignDescriptions[i],
                });
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = device.Id });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Details(int id)
        {
            var model = _context.Devices
                .Include(d => d.DeviceComponentsTemplate)
                    .ThenInclude(d => d.Component)
                .Include(d => d.DeviceDesignTemplate)
                    .ThenInclude(d => d.Design).FirstOrDefault(d => d.Id == id);
            return View(model);
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
