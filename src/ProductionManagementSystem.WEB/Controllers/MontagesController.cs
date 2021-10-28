using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Users;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles = RoleEnum.OrderPicker)]
    public class MontagesController : Controller
    {
        private readonly IMontageService _montageService;
        private readonly IDeviceService _deviceService;

        public MontagesController(IMontageService montageService, IDeviceService deviceService)
        {
            _montageService = montageService;
            _deviceService = deviceService;
        }
        
        // GET: Components
        /// <summary>
        /// Display index page
        /// </summary>
        /// <param name="sortOrder">Used for sorting.</param>
        /// <param name="searchString">Used for searching.</param>
        /// <param name="page">Current page.</param>
        /// <param name="pageSize"></param>
        /// <returns>A page with all components sorted by parameter <paramref name="sortOrder"/> and satisfying <paramref name="searchString"/></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int page=1, int pageSize = 50)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["sortOrder"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;

            var components = _montageService.GetAll();
            
            if (!String.IsNullOrEmpty(searchString))
            {
                components = components.Where(c => (c.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Corpus?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Explanation?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Nominal?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Type?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Manufacturer?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
            }

            ViewBag.PageSize = pageSize;
            
            var pageSizes = new SelectList(new[] {10, 25, 50, 100});
            var pageSizeFromList = pageSizes.FirstOrDefault(l => l.Text == pageSize.ToString());
            if (pageSizeFromList != null)
            {
                pageSizeFromList.Selected = true;
            }

            ViewBag.PageSizes = pageSizes;
            var componentsDto = components.ToList();
            ViewBag.MaxPage = componentsDto.Count() / pageSize + (componentsDto.Count() % pageSize == 0 ? 0: 1);
            ViewBag.CountComponents = componentsDto.Count();
            if (page > ViewBag.MaxPage)
            {
                page = 1;
            }
            
            switch (sortOrder)  
            {
                case "name_desc":
                    components = componentsDto.OrderByDescending(d => d.Name);
                    break;
                case "Type":
                    components = componentsDto.OrderBy(d => d.Type);
                    break;
                case "type_desc":
                    components = componentsDto.OrderByDescending(d => d.Type);
                    break;
                case "Quantity":
                    components = componentsDto.OrderBy(d => d.Quantity);
                    break;
                case "quantity_desc":
                    components = componentsDto.OrderByDescending(d => d.Quantity);
                    break;
                default:
                    components = componentsDto.OrderBy(d => d.Name);
                    break;
            }

            ViewBag.AllComponents = components;
            ViewBag.Page = page;
            components = components.Skip((page - 1) * pageSize).Take(pageSize);
            return View(components);
        }

        // GET: Components/Details/5
        public async Task<IActionResult> Details(int id)
        {
            return View(await _montageService.GetByIdAsync(id));    
        }

        // GET: Components/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.AllTypes = await _montageService.GetTypesAsync();
            ViewBag.AllComponents = _montageService.GetAll().Select(c => c.Name).Distinct();
            return View();
        }

        // POST: Components/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Name,Nominal,Corpus,Explanation,Manufacturer,Quantity")] Montage montage)
        {
            if (ModelState.IsValid)
            {
                await _montageService.CreateAsync(montage);
                return RedirectToAction(nameof(Details), new {id = montage.Id});
            }
            return View(montage);
        }

        // GET: Components/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var montage = await _montageService.GetByIdAsync(id);
                ViewBag.AllTypes = await _montageService.GetTypesAsync();
                return View(montage);
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Name,Nominal,Corpus,Explanation,Manufacturer,Quantity")] Montage montage)
        {
            if (id != montage.Id)
            {
                throw new Exception("Страница не найдена.");
            }

            if (ModelState.IsValid)
            {
                await _montageService.UpdateAsync(montage);
                return RedirectToAction(nameof(Details), new {id = montage.Id});
            }
            
            return View(montage);
        }

        // GET: Components/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            return View(await _montageService.GetByIdAsync(id));
        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _montageService.DeleteByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(await _montageService.GetByIdAsync(id));
            }
        }
        
        /// <summary>
        /// Method for getting all components in the form:
        /// {component.Name} {component.Nominal} {component.Corpus}
        /// </summary>
        /// <returns>JSON. Array of all components</returns>
        [HttpGet]
        public async Task<JsonResult> GetAllComponents()
        {
            IEnumerable<Montage> components = _montageService.GetAll();
            foreach (var comp in components)
            {
                comp.Name = comp.ToString();
            }
            
            return Json(components);
        }

        /// <summary>
        /// Method for getting all types of components
        /// </summary>
        /// <returns>JSON. Array of all types of components</returns>
        [NonAction]
        private async Task<IEnumerable<string>> GetAllTypes()
        {
            return await _montageService.GetTypesAsync();
        }
        
        /// <summary>
        /// Page with the addition to the component.
        /// </summary>
        /// <param name="id">Id of the component to add.</param>
        /// <param name="taskId">Task ID for quick return</param>
        /// <returns>Page with form</returns>
        [HttpGet]
        public async Task<IActionResult> Add(int id, int? taskId)
        {
            var montage = await _montageService.GetByIdAsync(id);
            ViewBag.TaskId = taskId;
            
            return View(montage);
        }
        
        /// <summary>
        /// Adding logic
        /// </summary>
        /// <param name="componentId">Id of the component to add.</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>Page /Components or the page with the task from which this page was called</returns>
        [HttpPost]
        public async Task<IActionResult> Add(int componentId, int quantity)
        {
            await _montageService.IncreaseQuantityOfMontageAsync(componentId, quantity);
            return RedirectToAction(nameof(Details), new {id = componentId});
        }
        
        /// <summary>
        /// Page with the access to the component
        /// </summary>
        /// <param name="id">Id of the component to receive.</param>
        /// <returns>Page with form</returns>
        [HttpGet]
        public async Task<IActionResult> Receive(int id)
        {
            var montage = await _montageService.GetByIdAsync(id);
            return View(montage);
        }
        
        /// <summary>
        /// Receiving logic
        /// </summary>
        /// <param name="componentId">Id of the component to add.</param>
        /// <param name="quantity">Quantity to receive</param>
        /// <returns>Page /Components</returns>
        [HttpPost]
        public async Task<IActionResult> Receive(int componentId, int quantity)
        {
            await _montageService.DecreaseQuantityOfDesignAsync(componentId, quantity);
            return RedirectToAction(nameof(Details), new {id = componentId});
        }

        public async Task<IActionResult> AddMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(_deviceService.GetAll(), "Id", "Name");
            var selectListTypes = new SelectList(await _montageService.GetTypesAsync());

            var components = new ComponentsForDevice();
            List<Montage> componentsInDevice = new List<Montage>();
            if (deviceId != null)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                {
                    device.Selected = true;
                }
                
                componentsInDevice.AddRange((await _deviceService.GetByIdAsync((int) deviceId)).Montage.Select(c => c.Component).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(_montageService.GetAll());
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
                await _montageService.IncreaseQuantityOfMontageAsync(components.ComponentId[index], components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(AddMultiple));
        }
        
        public async Task<IActionResult> ReceiveMultiple(int? deviceId, string typeName)
        {
            var selectListDevice = new SelectList(_deviceService.GetAll(), "Id", "Name");
            var selectListTypes = new SelectList(await _montageService.GetTypesAsync());

            var components = new ComponentsForDevice();
            List<Montage> componentsInDevice = new List<Montage>();
            if (deviceId.HasValue)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                {
                    device.Selected = true;
                }
                
                componentsInDevice.AddRange((await _deviceService.GetByIdAsync(deviceId.Value)).Montage.Select(c => c.Component).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(_montageService.GetAll());
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
                await _montageService.IncreaseQuantityOfMontageAsync(components.ComponentId[index], -components.Quantity[index]);
            }
            
            return RedirectToAction(nameof(ReceiveMultiple));
        }
    }
}
