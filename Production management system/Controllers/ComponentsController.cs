using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.Models;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    [Authorize(Roles = RoleEnum.OrderPicker)]
    public class ComponentsController : Controller
    {
        private readonly IComponentService _componentService;
        private IMapper _mapperToViewModel;
        private IMapper _mapperFromViewModel;

        public ComponentsController(IComponentService service)
        {
            _componentService = service;
            _mapperToViewModel = new MapperConfiguration(cfg => cfg.CreateMap<ComponentDTO, ComponentViewModel>())
                .CreateMapper();
            _mapperFromViewModel = new MapperConfiguration(cfg => cfg.CreateMap<ComponentViewModel, ComponentDTO>())
                .CreateMapper();
        }
        
        // GET: Components
        /// <summary>
        /// Display index page
        /// </summary>
        /// <param name="sortOrder">Used for sorting</param>
        /// <param name="searchString">Used for searching</param>
        /// <returns>A page with all components sorted by parameter <paramref name="sortOrder"/> and satisfying <paramref name="searchString"/></returns>
        [HttpGet]
        public IActionResult Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["CurrentFilter"] = searchString;

            var components = _componentService.GetComponents();
            
            if (!String.IsNullOrEmpty(searchString))
            {
                components = components.Where(c => c.Name.Contains(searchString)
                                                    || c.Corpus.Contains(searchString)
                                                    || c.Explanation.Contains(searchString)
                                                    || c.Nominal.Contains(searchString)
                                                    || c.Type.Contains(searchString)
                                                    || c.Manufacturer.Contains(searchString));
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

            var componentsViewModel =
                _mapperToViewModel.Map<IEnumerable<ComponentDTO>, IEnumerable<ComponentViewModel>>(components);
            return View(componentsViewModel);
        }

        // GET: Components/Details/5
        public IActionResult Details(int? id)
        {
            try
            {
                var component = _componentService.GetComponent(id);
                var componentViewModel =
                    _mapperToViewModel.Map<ComponentDTO, ComponentViewModel>(component);
                return View(componentViewModel);
            }
            catch (PageNotFoundException e)
            {
                return NotFound();
            }
        }

        // GET: Components/Create
        public IActionResult Create()
        {
            ViewBag.AllTypes = GetAllTypes();
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
                    _mapperFromViewModel.Map<ComponentViewModel, ComponentDTO>(componentViewModel);
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
                    _mapperToViewModel.Map<ComponentDTO, ComponentViewModel>(component);
                ViewBag.AllTypes = GetAllTypes();
                return View(componentViewModel);
            }
            catch (PageNotFoundException e)
            {
                return NotFound();
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var component =
                        _mapperFromViewModel.Map<ComponentViewModel, ComponentDTO>(componentViewModel);
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
                    _mapperToViewModel.Map<ComponentDTO, ComponentViewModel>(component);
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
                    _mapperToViewModel.Map<ComponentDTO, ComponentViewModel>(_componentService.GetComponent(id)));
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
                _mapperToViewModel.Map<ComponentDTO, ComponentViewModel>(component);
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
                _mapperToViewModel.Map<ComponentDTO, ComponentViewModel>(component);
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
    }
}
