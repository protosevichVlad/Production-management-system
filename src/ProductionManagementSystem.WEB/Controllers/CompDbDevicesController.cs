using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.PCB;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models.Device;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class CompDbDevicesController : Controller
    {
        private readonly ICompDbDeviceService _deviceService;
        private readonly IEntityExtService _entityService;

        public CompDbDevicesController(ICompDbDeviceService deviceService, IEntityExtService entityService)
        {
            _deviceService = deviceService;
            _entityService = entityService;
        }

        public async Task<IActionResult> Index(string orderBy, string q)
        {
            List<CompDbDevice> data;
            
            if (string.IsNullOrEmpty(q))
            {
                data = await _deviceService.GetAllAsync();
            }
            else
            {
                data = await _deviceService
                    .Find(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(q, StringComparison.InvariantCultureIgnoreCase));
                
                var entity = await _entityService.GetEntityExtByPartNumber(q);
                if (entity != null)
                {
                    data.AddRange(await _deviceService.GetWithEntity(entity.KeyId));
                }
            }
            
            if (!string.IsNullOrEmpty(orderBy))
            {
                if (orderBy.EndsWith("_desc"))
                {
                    data = data.OrderByDescending((x) => typeof(CompDbDevice).GetProperty(orderBy.Substring(0, orderBy.Length - "_desc".Length))?.GetValue(x)).ToList();
                }
                else
                {
                    data = data.OrderBy((x) => typeof(CompDbDevice).GetProperty(orderBy)?.GetValue(x)).ToList();
                }
                
            }
            
            return View(data);
        }

        [Authorize(Roles = RoleEnum.Admin)]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        
        [Authorize(Roles = RoleEnum.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            return View("Create", id);
        }
        
        [HttpGet]
        [Route("/api/devices/{id:int}")]
        public async Task<CompDbDevice> ApiGet([FromRoute] int id)
        {
            return await _deviceService.GetByIdAsync(id);
        }

        [HttpPost]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/devices")]
        public async Task<IActionResult> ApiCreate([FromForm] DeviceCreateEditViewModel device)
        {
            await _deviceService.CreateAsync(await MapToDevice(device));
            return Ok(device);
        }
        
        [HttpPut]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/devices/{id:int}")]
        public async Task<IActionResult> ApiEdit([FromRoute] int id, [FromForm] DeviceCreateEditViewModel device)
        {
            await _deviceService.UpdateAsync(await MapToDevice(device));
            return Ok(device);
        }
        
        [HttpDelete]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/devices/{id:int}")]
        public async Task<IActionResult> ApiDelete([FromRoute] int id)
        {
            await _deviceService.DeleteByIdAsync(id);
            return Ok();
        }

        public async  Task<IActionResult> Details(int id)
        {
            var device = await _deviceService.GetByIdAsync(id);
            if (device == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(device);
        }
        
        [HttpGet]
        [Route("/compDbDevices/latest")]
        public async  Task<IActionResult> LatestDetails()
        {
            return View(nameof(Details), await _deviceService.GetLatest());
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.OrderPicker)]
        [Route("/api/devices/{id:int}/add")]
        public async Task<IActionResult> IncreaseQuantity([FromRoute]int id, [FromBody]int quantity)
        {
            await _deviceService.IncreaseQuantityAsync(id, quantity);
            return Ok();
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.OrderPicker)]
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
                UsedItems = model.UsedItems,
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

        public async Task<IActionResult> PrintVersion(int id)
        {
            var device = await _deviceService.GetByIdAsync(id);
            if (device == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(device);
        }
    }
}