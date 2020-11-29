using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ProductionManagementSystem.Controllers
{
    public class ComponentController : Controller
    {
        private readonly ILogger<ComponentController> _logger;
        private ApplicationContext _context;

        public ComponentController(ILogger<ComponentController> logger)
        {
            _logger = logger;
            _context = new ApplicationContext();
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Show(string sortBy, string splitByType)
        {
            if (sortBy == null)
            {
                ViewBag.SortBy = "Id";
                sortBy = "Id";
            }
            else
            {
                ViewBag.SortBy = sortBy;
            }

            if (splitByType == null)
            {
                ViewBag.SplitByType = "True";
                splitByType = "True";
            }
            else
            {
                ViewBag.SplitByType = splitByType;
            }
            

            if (splitByType == "True")
            {
                var componentsSortedByType = _context.Components.OrderBy(c => c.Type);
                if (sortBy == "Name")
                {
                    ViewBag.Components = componentsSortedByType.ThenBy(c => c.Name);
                }
                else if (sortBy == "Nominal")
                {
                    ViewBag.Components = componentsSortedByType.ThenBy(c => c.Nominal);
                }
                else if (sortBy == "Corpus")
                {
                    ViewBag.Components = componentsSortedByType.ThenBy(c => c.Corpus);
                }
                else if (sortBy == "Explanation")
                {
                    ViewBag.Components = componentsSortedByType.ThenBy(c => c.Explanation);
                }
                else if (sortBy == "Manufacturer")
                {
                    ViewBag.Components = componentsSortedByType.ThenBy(c => c.Manufacturer);
                }
                else if (sortBy == "Quantity")
                {
                    ViewBag.Components = componentsSortedByType.ThenBy(c => c.Quantity);
                }
                else
                {
                    ViewBag.Components = componentsSortedByType;
                }
            }
            else
            {
                var components = _context.Components;
                if (sortBy == "Name")
                {
                    ViewBag.Components = components.OrderBy(c => c.Name);
                }
                else if (sortBy == "Nominal")
                {
                    ViewBag.Components = components.OrderBy(c => c.Nominal);
                }
                else if (sortBy == "Corpus")
                {
                    ViewBag.Components = components.OrderBy(c => c.Corpus);
                }
                else if (sortBy == "Explanation")
                {
                    ViewBag.Components = components.OrderBy(c => c.Explanation);
                }
                else if (sortBy == "Manufacturer")
                {
                    ViewBag.Components = components.OrderBy(c => c.Manufacturer);
                }
                else if (sortBy == "Quantity")
                {
                    ViewBag.Components = components.OrderBy(c => c.Quantity);
                }
                else if (sortBy == "Type")
                {
                    ViewBag.Components = components.OrderBy(c => c.Type);
                }
                else
                {
                    ViewBag.Components = components;
                }
            }
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add(Component component)
        {
            _context.Components.Add(component);
            _context.SaveChanges();
            return Redirect("/Component/Show");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(int id)
        {
            ViewBag.Component = _context.Components.Find(id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(Component component)
        {
            Component comp = _context.Components.Where(c => c.Id == component.Id).FirstOrDefault();

            comp.Name = component.Name;
            comp.Type = component.Type;
            comp.Nominal = component.Nominal;
            comp.Corpus = component.Corpus;
            comp.Explanation = component.Explanation;
            comp.Manufacturer = component.Manufacturer;
            comp.Quantity = component.Quantity;

            _context.SaveChanges();
            return Redirect("/Component/Show");
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
            return Redirect("/Component/Show");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Adding(int taskId, int componentId, int addedQuntity=0)
        {
            if (addedQuntity == 0)
            {
                ViewBag.TaskId = taskId;
                ViewBag.Component = _context.Components.Where(c => c.Id == componentId).FirstOrDefault(); ;
                return View();
            }
            else
            {
                Component component = _context.Components.Where(c => c.Id == componentId).FirstOrDefault();
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
