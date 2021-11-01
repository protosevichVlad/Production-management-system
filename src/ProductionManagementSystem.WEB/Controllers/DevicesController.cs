using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Devices;
using ProductionManagementSystem.Models.Users;
using ProductionManagementSystem.WEB.Models.Device;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles = RoleEnum.Admin)]
    public class DevicesController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IMontageService _montageService;
        private readonly IDesignService _designService;

        public DevicesController(IDeviceService deviceService, IMontageService montageService, IDesignService designService)
        {
            _deviceService = deviceService;
            _montageService = montageService;
            _designService = designService;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            IEnumerable<Device> devices = await _deviceService.GetAllAsync();

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
            
            return View(devices);
        }

        public async Task<IActionResult> GetPartialViewForDeviceItem(string type, int index)
        {
            if (type.ToLower() == "montage")
            {
                DeviceItem<Montage> viewModelMontage = new DeviceItem<Montage>()
                {
                    Id = index,
                    Component = new MontageInDevice(),
                    AllComponents = await _montageService.GetAllAsync(),
                    SelectedComponentId = 0
                };
                return PartialView("Partail/Device/Montageitem", viewModelMontage);
            }
            
            DeviceItem<Design> viewModelDesign = new DeviceItem<Design>()
            {
                Id = index,
                Component = new DesignInDevice(),
                AllComponents = await _designService.GetAllAsync(),
                SelectedComponentId = 0
            };
            return PartialView("Partail/Device/DesignItem", viewModelDesign);
            
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Device device)
        {
            await _deviceService.CreateAsync(device);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var device = await _deviceService.GetByIdAsync(id);
                InitComponents(device);
                ViewBag.Montages = await _montageService.GetAllAsync();
                ViewBag.Designs = await _designService.GetAllAsync();
                return View(device);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Device device)
        {
            try
            {
                await _deviceService.UpdateAsync(device);
                return RedirectToAction(nameof(Details), new {id = device.Id});
            }
            catch (IntersectionOfEntitiesException e)
            {
                TempData["ErrorMessage"] = e.Message;
                TempData["ErrorHeader"] = e.Header;
                return RedirectToAction(nameof(Details), new { id = device.Id });
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var device = await _deviceService.GetByIdAsync(id);
                InitComponents(device);
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
                ViewBag.ErrorHeader = TempData["ErrorHeader"];
                TempData["ErrorMessage"] = null;
                TempData["ErrorHeader"] = null;
                return View(device);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var device = await _deviceService.GetByIdAsync(id);
                return View(device);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // POST: Designs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _deviceService.DeleteByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(await _deviceService.GetByIdAsync(id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDevices()
        {
            return Json(_deviceService.GetAllAsync());
        }

        private void InitComponents(Device device)
        {
            device.Montages = device.Montages.Select(async m =>
            {
                m.Component = await _montageService.GetByIdAsync(m.ComponentId);
                return m;
            }).Select(t => t.Result).Where(i => i != null).ToList();
            device.Designs = device.Designs.Select(async d =>
            {
                d.Component = await _designService.GetByIdAsync(d.ComponentId);
                return d;
            }).Select(t => t.Result).Where(i => i != null).ToList();
        }
    }
    
}
