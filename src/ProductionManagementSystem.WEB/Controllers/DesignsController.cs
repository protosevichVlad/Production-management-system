using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.Core.Infrastructure;
using ProductionManagementSystem.Core.Models.Components;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles = RoleEnum.OrderPicker)]
    public class DesignsController : Controller
    {
        private readonly IDesignService _designService;
        private readonly IDeviceService _deviceService;
        
        
        public DesignsController(IDesignService service, IDeviceService deviceService)
        {
            _deviceService = deviceService;
            _designService = service;
        }

        // GET: Designs
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int page=1, int pageSize = 50)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["CurrentFilter"] = searchString;
            var designs = await _designService.GetAllAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                designs = designs.Where(d => (d.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                             || (d.Type?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                             || (d.ShortDescription?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false));
            }
            
            ViewBag.PageSize = pageSize;
            var pageSizes = new SelectList(new[] {10, 25, 50, 100});
            var pageSizeFromList = pageSizes.FirstOrDefault(l => l.Text == pageSize.ToString());
            if (pageSizeFromList != null)
            {
                pageSizeFromList.Selected = true;
            }

            ViewBag.PageSizes = pageSizes;
            var designsDto = designs.ToList();
            ViewBag.MaxPage = designsDto.Count() / pageSize + (designsDto.Count() % pageSize == 0 ? 0: 1);
            ViewBag.CountComponents = designsDto.Count();
            ViewBag.Page = page;
            if (page > ViewBag.MaxPage)
            {
                page = 1;
            }
            
            switch (sortOrder)
            {
                case "name_desc":
                    designs = designsDto.OrderByDescending(d => d.Name);
                    break;
                case "Type":
                    designs = designsDto.OrderBy(d => d.Type);
                    break;
                case "type_desc":
                    designs = designsDto.OrderByDescending(d => d.Type);
                    break;
                case "Quantity":
                    designs = designsDto.OrderBy(d => d.Quantity);
                    break;
                case "quantity_desc":
                    designs = designsDto.OrderByDescending(d => d.Quantity);
                    break;
                default:
                    designs = designsDto.OrderBy(d => d.Name);
                    break;
            }

            ViewBag.AllDesigns = designs.Select(d => d.Name).Distinct();
            designs = designs.Skip((page - 1) * pageSize).Take(pageSize);
            return View(designs);
        }

        // GET: Designs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View(await _designService.GetByIdAsync(id));
        }

        // GET: Designs/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.AllTypes = await _designService.GetTypesAsync();
            ViewBag.AllDesigns = (await _designService.GetAllAsync()).Select(d => d.Name).Distinct();
            return View();
        }
        
        // POST: Designs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Name,Quantity,ShortDescription,Description")] Design design)
        {
            if (ModelState.IsValid)
            {
                await _designService.CreateAsync(design);
                return RedirectToAction(nameof(Details), new {id = design.Id});
            }
            return View(design);
        }

        // GET: Designs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var design = await _designService.GetByIdAsync(id);
            ViewBag.AllTypes = await _designService.GetTypesAsync();
            return View(design);
        }

        // POST: Designs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Name,Quantity,ShortDescription,Description")] Design design)
        {
            if (ModelState.IsValid)
            {
                await _designService.UpdateAsync(design);
                return RedirectToAction(nameof(Details), new {id = design.Id});
            }
            return View(design);
        }

        // GET: Designs/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var design = await _designService.GetByIdAsync(id);
            return View(design);
        }

        // POST: Designs/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _designService.DeleteByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(await _designService.GetByIdAsync(id));
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDesigns()
        {
            return Json((await _designService.GetAllAsync()).Select(d => d.ToString()));
        }
        
        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            return View(await _designService.GetByIdAsync(id));
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(int designId, int quantity)
        {
            await _designService.IncreaseQuantityOfDesignAsync(designId, quantity);
            return RedirectToAction(nameof(Details), new { id = designId });
        }
        
        [HttpGet]
        public async Task<IActionResult> Receive(int id)
        {

            return View(await _designService.GetByIdAsync(id));
        }
        [HttpPost]
        public async Task<IActionResult> Receive(int designId, int quantity)
        {

            await _designService.DecreaseQuantityOfDesignAsync(designId, quantity);
            return RedirectToAction(nameof(Details), new { id = designId });
        }
        
        public async Task<IActionResult> AddMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(await _deviceService.GetAllAsync(), "Id", "Name");
            var selectListTypes = new SelectList(await _designService.GetTypesAsync());

            var components = new ComponentsForDevice();
            List<Design> componentsInDevice = new List<Design>();
            if (deviceId.HasValue)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                {
                    device.Selected = true;
                }
                
                componentsInDevice.AddRange((await _deviceService.GetByIdAsync(deviceId.Value)).Designs.Select(c => c.Design).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(await _designService.GetAllAsync());
            }

            if (typeName != null)
            {
                var type = selectListTypes.FirstOrDefault(l => l.Text == typeName);
                if (type != null)
                {
                    type.Selected = true;
                }
                
                componentsInDevice = componentsInDevice.Where(c => c.Type == typeName).ToList();
            }

            ViewBag.TypeNames = selectListTypes;
            ViewBag.Devices = selectListDevice;

            var length = componentsInDevice.Count;
            components.ComponentId = new int[length];
            components.ComponentName = new string[length];
            components.QuantityInStock = new int[length];
            for (var index = 0; index < length; index++)
            {
                var componentInDevice = componentsInDevice[index];
                components.ComponentName[index] = componentInDevice.ToString();
                components.ComponentId[index] = componentInDevice.Id;
                components.QuantityInStock[index] = componentInDevice.Quantity;
            }

            return View(components);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddMultiple(ComponentsForDevice components)
        {
            if (components == null)
            {
                throw new Exception("Device not found.");
            }

            for (var index = 0; index < components.ComponentId.Length; index++)
            {
                await _designService.IncreaseQuantityOfDesignAsync(components.ComponentId[index], components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(AddMultiple));
        }
        
        [HttpGet]
        public async Task<IActionResult> ReceiveMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(await _deviceService.GetAllAsync(), "Id", "Name");
            var selectListTypes = new SelectList(await _designService.GetTypesAsync());

            var components = new ComponentsForDevice();
            List<Design> componentsInDevice = new List<Design>();
            if (deviceId != null)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                {
                    device.Selected = true;
                }
                
                componentsInDevice.AddRange((await _deviceService.GetByIdAsync(deviceId.Value)).Designs.Select(c => c.Design).ToArray());
            }
            else
            {
                componentsInDevice.AddRange( await _designService.GetAllAsync());
            }


            if (typeName != null)
            {
                var type = selectListTypes.FirstOrDefault(l => l.Text == typeName);
                if (type != null)
                {
                    type.Selected = true;
                }
                
                componentsInDevice = componentsInDevice.Where(c => c.Type == typeName).ToList();
            }

            var length = componentsInDevice.Count;
            components.ComponentId = new int[length];
            components.ComponentName = new string[length];
            components.Quantity = new int[length];
            components.QuantityInStock = new int[length];
            for (var index = 0; index < length; index++)
            {
                var componentInDevice = componentsInDevice[index];
                components.ComponentName[index] = componentInDevice.ToString();
                components.ComponentId[index] = componentInDevice.Id;
                components.QuantityInStock[index] = componentInDevice.Quantity;
            }

            ViewBag.TypeNames = selectListTypes;
            ViewBag.Devices = selectListDevice;
            return View(components);
        }
        
        [HttpPost]
        public async Task<IActionResult> ReceiveMultiple(ComponentsForDevice components)
        {
            if (components == null)
            {
                throw new Exception("Device not found.");
            }

            for (var index = 0; index < components.ComponentId.Length; index++)
            {
                await _designService.DecreaseQuantityOfDesignAsync(components.ComponentId[index], components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(ReceiveMultiple));
        }
    }
}
