using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.DAL.Enums;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    [Authorize(Roles = RoleEnum.Admin)]
    public class DevicesController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IComponentService _componentService;
        private readonly IDesignService _designService;
        private readonly IMapper _mapper;

        public DevicesController(IDeviceService deviceService, IComponentService componentService, IDesignService designService)
        {
            _deviceService = deviceService;
            _componentService = componentService;
            _designService = designService;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DeviceViewModel, DeviceDTO>();
                    cfg.CreateMap<DeviceDTO, DeviceViewModel>();
                })
                .CreateMapper();
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            var devices = await _deviceService.GetDevicesAsync();

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
            
            return View(_mapper.Map<IEnumerable<DeviceDTO>, IEnumerable<DeviceViewModel>>(devices));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeviceViewModel deviceViewModel)
        {
            DeviceDTO device = _mapper.Map<DeviceViewModel, DeviceDTO>(deviceViewModel);
            LogService.UserName = User.Identity?.Name;
            await _deviceService.CreateDeviceAsync(device);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var device = await _deviceService.GetDeviceAsync(id);
                var deviceViewModel = _mapper.Map<DeviceDTO, DeviceViewModel>(device);
                ViewBag.Components = await _componentService.GetComponentsAsync();
                ViewBag.Designs = await _designService.GetDesignsAsync();
                return View(deviceViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DeviceViewModel deviceViewModel)
        {
            try
            {
                DeviceDTO device = _mapper.Map<DeviceViewModel, DeviceDTO>(deviceViewModel);
                LogService.UserName = User.Identity?.Name;
                await _deviceService.UpdateDeviceAsync(device);
                return RedirectToAction(nameof(Details), new {id = device.Id});
            }
            catch (IntersectionOfEntitiesException e)
            {
                TempData["ErrorMessage"] = e.Message;
                TempData["ErrorHeader"] = e.Header;
                return RedirectToAction(nameof(Details), new { id = deviceViewModel.Id });
                
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
                var device = _mapper.Map<DeviceDTO, DeviceViewModel>(await _deviceService.GetDeviceAsync(id));
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

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var device = _mapper.Map<DeviceDTO, DeviceViewModel>(await _deviceService.GetDeviceAsync(id));
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
                LogService.UserName = User.Identity?.Name;
                await _deviceService.DeleteDeviceAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(_mapper.Map<DeviceDTO, DeviceViewModel>(await _deviceService.GetDeviceAsync(id)));
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
            var devices = await _deviceService.GetDevicesAsync();
            return Json(devices);
        }
    }
    
}
