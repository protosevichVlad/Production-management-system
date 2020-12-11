using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProductionManagementSystem.Controllers
{
    public class ComponentController : Controller
    {
        private ApplicationContext _context;

        public ComponentController()
        {
            _context = new ApplicationContext();
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Index(string sortBy, string splitByType)
        {
            if (sortBy == null)
            {
                sortBy = "Id";
            }

            if (splitByType == null)
            {
                splitByType = "True";
            } 
            
            ViewBag.SortBy = sortBy;
            ViewBag.SplitByType = splitByType;
            

            if (splitByType == "True")
            {
                var componentsSortedByType = _context.Components.OrderBy(c => c.Type);
                switch (sortBy)
                {
                    case "Name":
                        ViewBag.Components = componentsSortedByType.ThenBy(c => c.Name);
                        break;
                    case "Nominal":
                        ViewBag.Components = componentsSortedByType.ThenBy(c => c.Nominal);
                        break;
                    case "Corpus":
                        ViewBag.Components = componentsSortedByType.ThenBy(c => c.Corpus);
                        break;
                    case "Explanation":
                        ViewBag.Components = componentsSortedByType.ThenBy(c => c.Explanation);
                        break;
                    case "Manufacturer":
                        ViewBag.Components = componentsSortedByType.ThenBy(c => c.Manufacturer);
                        break;
                    case "Quantity":
                        ViewBag.Components = componentsSortedByType.ThenBy(c => c.Quantity);
                        break;
                    default:
                        ViewBag.Components = componentsSortedByType;
                        break;
                }
            }
            else
            {
                var components = _context.Components;
                switch (sortBy)
                {
                    case "Name":
                        ViewBag.Components = components.OrderBy(c => c.Name);
                        break;
                    case "Nominal":
                        ViewBag.Components = components.OrderBy(c => c.Nominal);
                        break;
                    case "Corpus":
                        ViewBag.Components = components.OrderBy(c => c.Corpus);
                        break;
                    case "Explanation":
                        ViewBag.Components = components.OrderBy(c => c.Explanation);
                        break;
                    case "Manufacturer":
                        ViewBag.Components = components.OrderBy(c => c.Manufacturer);
                        break;
                    case "Quantity":
                        ViewBag.Components = components.OrderBy(c => c.Quantity);
                        break;
                    default:
                        ViewBag.Components = components;
                        break;
                }
            }
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Create(Component component)
        {
            _context.Components.Add(component);
            _context.SaveChanges();
            return Redirect("/Component/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(int id)
        {
            return View(_context.Components.Find(id));
        }

        [HttpPost]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(Component component)
        {
            Component comp = _context.Components.FirstOrDefault(c => c.Id == component.Id);

            comp.Name = component.Name;
            comp.Type = component.Type;
            comp.Nominal = component.Nominal;
            comp.Corpus = component.Corpus;
            comp.Explanation = component.Explanation;
            comp.Manufacturer = component.Manufacturer;
            comp.Quantity = component.Quantity;

            _context.SaveChanges();
            return Redirect("/Component/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Remove(int id)
        {
            Component comp = new Component
            {
                Id = id
            };

            _context.Components.Attach(comp);
            _context.Components.Remove(comp);

            _context.SaveChanges();
            return Redirect("/Component/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Adding(int taskId, int componentId, int addedQuntity=0)
        {
            if (addedQuntity == 0)
            {
                ViewBag.TaskId = taskId;
                ViewBag.Component = _context.Components.FirstOrDefault(c => c.Id == componentId); ;
                return View();
            }
            else
            {
                Component component = _context.Components.FirstOrDefault(c => c.Id == componentId);
                component.Quantity += addedQuntity;
                _context.SaveChanges();
                return Redirect($"/Task/ShowTask/{taskId}");
            }
        }

        [HttpGet]
        public JsonResult GetAllComponents()
        {
            List<Component> components = _context.Components.OrderBy(c => c.Name).ToList();
            return Json(components);
        }

        public JsonResult GetAllTypes()
        {
            List<string> types = _context.Components.OrderBy(c => c.Type).Select(c => c.Type).ToList();
            types = types.Distinct().ToList();
            return Json(types);
        }
    }
}
