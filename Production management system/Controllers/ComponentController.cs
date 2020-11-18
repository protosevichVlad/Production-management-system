using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProductionManagementSystem.Controllers
{
    public class ComponentController : Controller
    {
        private readonly ILogger<ComponentController> _logger;

        public ComponentController(ILogger<ComponentController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Show(string sortBy, string splitByType)
        {
            var db = new ApplicationContext();
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
                var componentsSortedByType = db.Components.OrderBy(c => c.Type);
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
                var components = db.Components;
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
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Component component)
        {
            var db = new ApplicationContext();
            db.Components.Add(component);
            db.SaveChanges();
            return Redirect("/Component/Show");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var db = new ApplicationContext();
            ViewBag.Component = db.Components.Find(id);
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Component component)
        {
            var db = new ApplicationContext();
            Component comp = db.Components.Where(c => c.Id == component.Id).FirstOrDefault();

            comp.Name = component.Name;
            comp.Type = component.Type;
            comp.Nominal = component.Nominal;
            comp.Corpus = component.Corpus;
            comp.Explanation = component.Explanation;
            comp.Manufacturer = component.Manufacturer;
            comp.Quantity = component.Quantity;

            db.SaveChanges();
            return Redirect("/Component/Show");
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            ApplicationContext db = new ApplicationContext();

            Component comp = new Component
            {
                Id = id
            };

            db.Components.Attach(comp);
            db.Components.Remove(comp);

            db.SaveChanges();
            return Redirect("/Component/Show");
        }
    }
}
