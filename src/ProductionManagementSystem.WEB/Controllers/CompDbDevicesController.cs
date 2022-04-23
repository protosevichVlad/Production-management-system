﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models.Device;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class CompDbDevicesController : Controller
    {
        private readonly ICompDbDeviceService _deviceService;

        public CompDbDevicesController(ICompDbDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _deviceService.GetAllAsync() ?? new List<CompDbDevice>();
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            return View("Create", id);
        }
        
        [HttpGet]
        [Route("/api/devices/{id:int}")]
        public async Task<CompDbDevice> ApiGet([FromRoute] int id)
        {
            var device = await _deviceService.GetByIdAsync(id);
            device.UsedInDevice = device.UsedInDevice.Select(x =>
            {
                x.Component = new CreateDeviceSearchViewModel(x.Component);
                return x;
            }).ToList();
            return device;
        }

        [HttpPost]
        [Route("/api/devices")]
        public async Task<IActionResult> ApiCreate([FromForm] DeviceCreateEditViewModel device)
        {
            await _deviceService.CreateAsync(await MapToDevice(device));
            return Ok(device);
        }
        
        [HttpPut]
        [Route("/api/devices/{id:int}")]
        public async Task<IActionResult> ApiEdit([FromRoute] int id, [FromForm] DeviceCreateEditViewModel device)
        {
            await _deviceService.UpdateAsync(await MapToDevice(device));
            return Ok(device);
        }
        
        [HttpDelete]
        [Route("/api/devices/{id:int}")]
        public async Task<IActionResult> ApiDelete([FromRoute] int id)
        {
            await _deviceService.DeleteByIdAsync(id);
            return Ok();
        }

        public async  Task<IActionResult> Details(int id)
        {
            return View(await _deviceService.GetByIdAsync(id));
        }
        
        [HttpGet]
        [Route("/compDbDevices/latest")]
        public async  Task<IActionResult> LatestDetails()
        {
            return View(nameof(Details), await _deviceService.GetLatest());
        }
        
        [HttpPost]
        [Route("/api/devices/{id:int}/add")]
        public async Task<IActionResult> IncreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _deviceService.IncreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPost]
        [Route("/api/devices/{id:int}/get")]
        public async Task<IActionResult> DecreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _deviceService.DecreaseQuantityAsync(id, quantity);
            return Ok();
        }

        private async Task<CreateEditDevice> MapToDevice(DeviceCreateEditViewModel model)
        {
            var device = new CreateEditDevice()
            {
                Id = model.Id,
                Name = model.Name,
                Variant = model.Variant,
                Description = model.Description,
                Quantity = model.Quantity,
                ReportDate = model.ReportDate,
                UsedInDevice = model.UsedInDevice,
            };
            
            if (model.ImageUploader != null)
            {
                await using var ms = new MemoryStream();
                await model.ImageUploader.CopyToAsync(ms);
                device.Image = ms.ToArray();  
            }
            
            if (model.ThreeDModelUploader != null)
            {
                await using var ms = new MemoryStream();
                await model.ThreeDModelUploader.CopyToAsync(ms);
                device.ThreeDModel = ms.ToArray();  
            }

            return device;
        }
    }
}