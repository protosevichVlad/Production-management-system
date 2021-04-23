using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ComponentsController : Controller
    {
        private readonly IComponentService _componentService;
        private readonly IDeviceService _deviceService;
        private IMapper _mapper;

        public ComponentsController(IComponentService service, IDeviceService deviceService)
        {
            _componentService = service;
            _deviceService = deviceService;
            
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ComponentViewModel, ComponentDTO>();
                    cfg.CreateMap<ComponentDTO, ComponentViewModel>();
                    cfg.CreateMap<ComponentDTO, Component>();
                    cfg.CreateMap<Component, ComponentDTO>();
                })
                .CreateMapper();
        }
        
        // GET: Components
        /// <summary>
        /// Display index page
        /// </summary>
        /// <param name="sortOrder">Used for sorting.</param>
        /// <param name="searchString">Used for searching.</param>
        /// <param name="page">Current page.</param>
        /// <returns>A page with all components sorted by parameter <paramref name="sortOrder"/> and satisfying <paramref name="searchString"/></returns>
        [HttpGet]
        public IActionResult Index(string sortOrder, string searchString, int page=1, int pageSize = 50)
        {
            ViewBag.Page = page;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["sortOrder"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;

            var components = _componentService.GetComponents();
            
            if (!String.IsNullOrEmpty(searchString))
            {
                components = components.Where(c => (c.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Corpus?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Explanation?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Nominal?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Type?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false)
                                                    || (c.Manufacturer?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            ViewBag.PageSize = pageSize;
            ViewBag.MaxPage = components.Count() / pageSize + (components.Count() % pageSize == 0 ? 0: 1);
            ViewBag.CountComponents = components.Count();
            if (page > ViewBag.MaxPage)
            {
                throw new ArgumentException($"Страницы №{page} не существует.");
            }
            
            switch (sortOrder)  
            {
                case "name_desc":
                    components = components.OrderByDescending(d => d.Name);
                    break;
                case "Type":
                    components = components.OrderBy(d => d.Type);
                    break;
                case "type_desc":
                    components = components.OrderByDescending(d => d.Type);
                    break;
                case "Quantity":
                    components = components.OrderBy(d => d.Quantity);
                    break;
                case "quantity_desc":
                    components = components.OrderByDescending(d => d.Quantity);
                    break;
                default:
                    components = components.OrderBy(d => d.Name);
                    break;
            }

            components = components.Skip((page - 1) * pageSize).Take(pageSize);
            var componentsViewModel =
                _mapper.Map<IEnumerable<ComponentDTO>, IEnumerable<ComponentViewModel>>(components);
            
            ViewBag.AllComponents = _componentService.GetComponents().Select(c => c.Name).Distinct();
            return View(componentsViewModel);
        }

        // GET: Components/Details/5
        public IActionResult Details(int? id)
        {
            try
            {
                var component = _componentService.GetComponent(id);
                var componentViewModel =
                    _mapper.Map<ComponentDTO, ComponentViewModel>(component);
                return View(componentViewModel);
            }
            catch (PageNotFoundException e)
            {
                throw new Exception("Страница не найдена.");;
            }
        }

        // GET: Components/Create
        public IActionResult Create()
        {
            ViewBag.AllTypes = GetAllTypes();
            ViewBag.AllComponents = _componentService.GetComponents().Select(c => c.Name).Distinct();
            return View();
        }

        // POST: Components/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Type,Name,Nominal,Corpus,Explanation,Manufacturer,Quantity")] ComponentViewModel componentViewModel)
        {
            if (ModelState.IsValid)
            {
                var component =
                    _mapper.Map<ComponentViewModel, ComponentDTO>(componentViewModel);
                LogService.UserName = User.Identity?.Name;
                _componentService.CreateComponent(component);
                return RedirectToAction(nameof(Index));
            }
            return View(componentViewModel);
        }

        // GET: Components/Edit/5
        public IActionResult Edit(int? id)
        {
            try
            {
                var component = _componentService.GetComponent(id);
                var componentViewModel =
                    _mapper.Map<ComponentDTO, ComponentViewModel>(component);
                ViewBag.AllTypes = GetAllTypes();
                return View(componentViewModel);
            }
            catch (PageNotFoundException e)
            {
                throw new Exception("Страница не найдена.");
            }
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Type,Name,Nominal,Corpus,Explanation,Manufacturer,Quantity")] ComponentViewModel componentViewModel)
        {
            if (id != componentViewModel.Id)
            {
                throw new Exception("Страница не найдена.");;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var component =
                        _mapper.Map<ComponentViewModel, ComponentDTO>(componentViewModel);
                    LogService.UserName = User.Identity?.Name;
                    _componentService.UpdateComponent(component);
                }
                catch (Exception exception)
                {
                    throw;
                }
                return RedirectToAction(nameof(Edit), new {id = componentViewModel.Id});
            }
            return View(componentViewModel);
        }

        // GET: Components/Delete/5
        public IActionResult Delete(int? id)
        {
            try
            {
                var component = _componentService.GetComponent(id);
                var componentViewModel =
                    _mapper.Map<ComponentDTO, ComponentViewModel>(component);
                return View(componentViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                LogService.UserName = User.Identity?.Name;
                _componentService.DeleteComponent(id);
                return RedirectToAction(nameof(Index));
            }
            catch (IntersectionOfEntitiesException e)
            {
                ViewBag.ErrorMessage = e.Message;
                ViewBag.ErrorHeader = e.Header;
                return View(
                    _mapper.Map<ComponentDTO, ComponentViewModel>(_componentService.GetComponent(id)));
            }
            catch (Exception e)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Method for getting all components in the form:
        /// {component.Name} {component.Nominal} {component.Corpus}
        /// </summary>
        /// <returns>JSON. Array of all components</returns>
        [HttpGet]
        public JsonResult GetAllComponents()
        {
            IEnumerable<ComponentDTO> components = _componentService.GetComponents();
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
        private IEnumerable<string> GetAllTypes()
        {
            return _componentService.GetTypes();
        }
        
        /// <summary>
        /// Page with the addition to the component.
        /// </summary>
        /// <param name="id">Id of the component to add.</param>
        /// <param name="taskId">Task ID for quick return</param>
        /// <returns>Page with form</returns>
        [HttpGet]
        public IActionResult Add(int id, int? taskId)
        {
            var component = _componentService.GetComponent(id);
            var componentViewModel =
                _mapper.Map<ComponentDTO, ComponentViewModel>(component);
            ViewBag.TaskId = taskId;
            
            return View(componentViewModel);
        }
        
        /// <summary>
        /// Adding logic
        /// </summary>
        /// <param name="componentId">Id of the component to add.</param>
        /// <param name="quantity">Quantity to add</param>
        /// <param name="taskId">Task ID for quick return</param>
        /// <returns>Page /Components or the page with the task from which this page was called</returns>
        [HttpPost]
        public IActionResult Add(int componentId, int quantity)
        {
            try
            {
                LogService.UserName = User.Identity?.Name;
                _componentService.AddComponent(componentId, quantity);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        /// <summary>
        /// Page with the access to the component
        /// </summary>
        /// <param name="id">Id of the component to receive.</param>
        /// <returns>Page with form</returns>
        [HttpGet]
        public IActionResult Receive(int id)
        {
            var component = _componentService.GetComponent(id);
            var componentViewModel =
                _mapper.Map<ComponentDTO, ComponentViewModel>(component);
            return View(componentViewModel);
        }
        
        /// <summary>
        /// Receiving logic
        /// </summary>
        /// <param name="componentId">Id of the component to add.</param>
        /// <param name="quantity">Quantity to receive</param>
        /// <returns>Page /Components</returns>
        [HttpPost]
        public IActionResult Receive(int componentId, int quantity)
        {
            try
            {
                LogService.UserName = User.Identity?.Name;
                _componentService.AddComponent(componentId, -quantity);
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
            var selectListTypes = new SelectList(_componentService.GetTypes());

            var components = new ComponentsForDevice();
            List<Component> componentsInDevice = new List<Component>();
            if (deviceId != null)
            {
                var device = selectListDevice.FirstOrDefault(l => l.Value == deviceId.ToString());
                if (device != null)
                {
                    device.Selected = true;
                }
                
                componentsInDevice.AddRange(_deviceService.GetComponentsTemplates((int) deviceId).Select(c => c.Component).ToArray());
            }
            else
            {
                componentsInDevice.AddRange(_mapper.Map<IEnumerable<ComponentDTO>, IEnumerable<Component>>(_componentService.GetComponents()));
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
            components.Quantity = new int[length];
            for (var index = 0; index < length; index++)
            {
                var componentInDevice = componentsInDevice[index];
                components.ComponentName[index] = componentInDevice.ToString();
                components.ComponentId[index] = componentInDevice.Id;
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
                _componentService.AddComponent(components.ComponentId[index], components.Quantity[index]);
            }
            
            return View(components);
        }
    }
}
