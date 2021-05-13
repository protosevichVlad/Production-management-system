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
        public IActionResult Index(string sortOrder, string searchString, int page=1, int pageSize = 50)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["CurrentFilter"] = searchString;
            var designs = _designService.GetDesigns();

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
            ViewBag.AllDesigns = _designService.GetDesigns().Select(d => d.Name).Distinct();
            return View(designsViewModule);
        }

        // GET: Designs/Details/5
        public IActionResult Details(int? id)
        {
            try
            {
                var design = _designService.GetDesign(id);
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
        public IActionResult Create()
        {
            ViewBag.AllTypes = _designService.GetTypes();
            ViewBag.AllDesigns = _designService.GetDesigns().Select(d => d.Name).Distinct();
            return View();
        }
        
        // POST: Designs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Type,Name,Quantity,ShortDescription,Description")] DesignViewModel designViewModel)
        {
            if (ModelState.IsValid)
            {
                var design = _mapper.Map<DesignViewModel, DesignDTO>(designViewModel);
                LogService.UserName = User.Identity?.Name;
                _designService.CreateDesign(design);
                return RedirectToAction(nameof(Index));
            }
            return View(designViewModel);
        }

        // GET: Designs/Edit/5
        public IActionResult Edit(int? id)
        {
            try
            {
                var design = _designService.GetDesign(id);
                var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
                
                ViewBag.AllTypes = _designService.GetTypes();
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
        public IActionResult Edit(int id, [Bind("Id,Type,Name,Quantity,ShortDescription,Description")] DesignViewModel designViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var design = _mapper.Map<DesignViewModel, DesignDTO>(designViewModel);
                    LogService.UserName = User.Identity?.Name;
                    _designService.UpdateDesign(design);
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
        public IActionResult Delete(int? id)
        {
            try
            {
                var design = _designService.GetDesign(id);
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
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                LogService.UserName = User.Identity?.Name;
                _designService.DeleteDesign(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(
                    _mapper.Map<DesignDTO, DesignViewModel>(_designService.GetDesign(id)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public JsonResult GetAllDesigns()
        {
            IEnumerable<DesignDTO> designs = _designService.GetDesigns();
                
            foreach (var des in designs)
            {
                des.Name = des.ToString();
            }
            return Json(designs);
        }
        
        [HttpGet]
        public IActionResult Add(int? id)
        {
            var design = _designService.GetDesign(id);
            var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
            return View(designViewModel);
        }
        
        [HttpPost]
        public IActionResult Add(int designId, int quantity)
        {

            try
            {
                LogService.UserName = User.Identity?.Name;
                _designService.AddDesign(designId, quantity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [HttpGet]
        public IActionResult Receive(int id)
        {

            var design = _designService.GetDesign(id);
            var designViewModel = _mapper.Map<DesignDTO, DesignViewModel>(design);
            return View(designViewModel);
        }
        [HttpPost]
        public IActionResult Receive(int designId, int quantity)
        {

            try
            {
                _designService.AddDesign(designId, -quantity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public IActionResult AddMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(_deviceService.GetDevices(), "Id", "Name");
            var selectListTypes = new SelectList(_designService.GetTypes());

            var components = new ComponentsForDevice();
            List<Design> componentsInDevice = new List<Design>();
            if (deviceId != null)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                {
                    device.Selected = true;
                }
                
                componentsInDevice.AddRange(_deviceService.GetDesignTemplates((int) deviceId).Select(c => c.Design).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(_mapper.Map<IEnumerable<DesignDTO>, IEnumerable<Design>>(_designService.GetDesigns()));
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
        public IActionResult AddMultiple(ComponentsForDevice components)
        {
            if (components == null)
            {
                throw new Exception("Device not found.");
            }

            LogService.UserName = User.Identity?.Name;
            for (var index = 0; index < components.ComponentId.Length; index++)
            {
                _designService.AddDesign(components.ComponentId[index], components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(AddMultiple));
        }
        
        [HttpGet]
        public IActionResult ReceiveMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(_deviceService.GetDevices(), "Id", "Name");
            var selectListTypes = new SelectList(_designService.GetTypes());

            var components = new ComponentsForDevice();
            List<Design> componentsInDevice = new List<Design>();
            if (deviceId != null)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                {
                    device.Selected = true;
                }
                
                componentsInDevice.AddRange(_deviceService.GetDesignTemplates((int) deviceId).Select(c => c.Design).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(_mapper.Map<IEnumerable<DesignDTO>, IEnumerable<Design>>(_designService.GetDesigns()));
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
        public IActionResult ReceiveMultiple(ComponentsForDevice components)
        {
            if (components == null)
            {
                throw new Exception("Device not found.");
            }

            LogService.UserName = User.Identity?.Name;
            for (var index = 0; index < components.ComponentId.Length; index++)
            {
                _designService.AddDesign(components.ComponentId[index], -components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(ReceiveMultiple));
        }
    }
}
