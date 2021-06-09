using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.Models;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    [Authorize(Roles = RoleEnum.OrderPicker)]
    public class DesignsController : Controller
    {
        private readonly IDesignService _designService;
        private readonly IDeviceService _deviceService;
        private IMapper _mapper;
        
        
        public DesignsController(IDesignService service, IDeviceService deviceService)
        {
            _deviceService = deviceService;
            _designService = service;
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DesignDTO, DesignViewModel>();
                cfg.CreateMap<DesignViewModel, DesignDTO>();
                cfg.CreateMap<DesignDTO, Design>();
            })
                .CreateMapper();
        }

        // GET: Designs
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int page=1, int pageSize = 50)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["CurrentFilter"] = searchString;
            var designs = await _designService.GetDesignsAsync();

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
            ViewBag.MaxPage = designs.Count() / pageSize + (designs.Count() % pageSize == 0 ? 0: 1);
            ViewBag.CountComponents = designs.Count();
            ViewBag.Page = page;
            if (page > ViewBag.MaxPage)
            {
                page = 1;
            }
            
            switch (sortOrder)
            {
                case "name_desc":
                    designs = designs.OrderByDescending(d => d.Name);
                    break;
                case "Type":
                    designs = designs.OrderBy(d => d.Type);
                    break;
                case "type_desc":
                    designs = designs.OrderByDescending(d => d.Type);
                    break;
                case "Quantity":
                    designs = designs.OrderBy(d => d.Quantity);
                    break;
                case "quantity_desc":
                    designs = designs.OrderByDescending(d => d.Quantity);
                    break;
                default:
                    designs = designs.OrderBy(d => d.Name);
                    break;
            }

            designs = designs.Skip((page - 1) * pageSize).Take(pageSize);
            var designsViewModule = _mapper.Map<IEnumerable<DesignDTO>, IEnumerable<DesignViewModel>>(designs);
            ViewBag.AllDesigns = (await _designService.GetDesignsAsync()).Select(d => d.Name).Distinct();
            return View(designsViewModule);
        }

        // GET: Designs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var design = await _designService.GetDesignAsync(id);
                var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
                return View(designViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // GET: Designs/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.AllTypes = await _designService.GetTypesAsync();
            ViewBag.AllDesigns = (await _designService.GetDesignsAsync()).Select(d => d.Name).Distinct();
            return View();
        }
        
        // POST: Designs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Name,Quantity,ShortDescription,Description")] DesignViewModel designViewModel)
        {
            if (ModelState.IsValid)
            {
                var design = _mapper.Map<DesignViewModel, DesignDTO>(designViewModel);
                LogService.UserName = User.Identity?.Name;
                await _designService.CreateDesignAsync(design);
                return RedirectToAction(nameof(Index));
            }
            return View(designViewModel);
        }

        // GET: Designs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var design = await _designService.GetDesignAsync(id);
                var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
                
                ViewBag.AllTypes = _designService.GetTypesAsync();
                return View(designViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // POST: Designs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Name,Quantity,ShortDescription,Description")] DesignViewModel designViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var design = _mapper.Map<DesignViewModel, DesignDTO>(designViewModel);
                    LogService.UserName = User.Identity?.Name;
                    await _designService.UpdateDesignAsync(design);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return View(designViewModel);
        }

        // GET: Designs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var design = await _designService.GetDesignAsync(id);
                var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
                return View(designViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // POST: Designs/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                LogService.UserName = User.Identity?.Name;
                await _designService.DeleteDesignAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(
                    _mapper.Map<DesignDTO, DesignViewModel>(await _designService.GetDesignAsync(id)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDesigns()
        {
            IEnumerable<DesignDTO> designs = await _designService.GetDesignsAsync();
                
            foreach (var des in designs)
            {
                des.Name = des.ToString();
            }
            return Json(designs);
        }
        
        [HttpGet]
        public async Task<IActionResult> Add(int? id)
        {
            var design = await _designService.GetDesignAsync(id);
            var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
            return View(designViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(int designId, int quantity)
        {

            try
            {
                LogService.UserName = User.Identity?.Name;
                await _designService.AddDesignAsync(designId, quantity);
                return RedirectToAction(nameof(Details), new { id = designId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Receive(int id)
        {

            var design = await _designService.GetDesignAsync(id);
            var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
            return View(designViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Receive(int designId, int quantity)
        {

            try
            {
                await _designService.AddDesignAsync(designId, -quantity);
                return RedirectToAction(nameof(Details), new { id = designId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public async Task<IActionResult> AddMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(await _deviceService.GetDevicesAsync(), "Id", "Name");
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
                
                componentsInDevice.AddRange((await _deviceService.GetDesignTemplatesAsync((int) deviceId)).Select(c => c.Design).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(_mapper.Map<IEnumerable<DesignDTO>, IEnumerable<Design>>(await _designService.GetDesignsAsync()));
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

            LogService.UserName = User.Identity?.Name;
            for (var index = 0; index < components.ComponentId.Length; index++)
            {
                await _designService.AddDesignAsync(components.ComponentId[index], components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(AddMultiple));
        }
        
        [HttpGet]
        public async Task<IActionResult> ReceiveMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(await _deviceService.GetDevicesAsync(), "Id", "Name");
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
                
                componentsInDevice.AddRange((await _deviceService.GetDesignTemplatesAsync((int) deviceId)).Select(c => c.Design).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(_mapper.Map<IEnumerable<DesignDTO>, IEnumerable<Design>>(await _designService.GetDesignsAsync()));
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

            LogService.UserName = User.Identity?.Name;
            for (var index = 0; index < components.ComponentId.Length; index++)
            {
                await _designService.AddDesignAsync(components.ComponentId[index], -components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(ReceiveMultiple));
        }
    }
}
