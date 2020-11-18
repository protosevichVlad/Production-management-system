using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProductionManagementSystem.Controllers
{
    public class DesignController : Controller
    {
        private readonly ILogger<DesignController> _logger;

        public DesignController(ILogger<DesignController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
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
                var sortedByType = db.Designs.OrderBy(d => d.Type);
                if (sortBy == "Id")
                {
                    ViewBag.Designs = sortedByType.ThenBy(d => d.Id);
                }
                else if (sortBy == "Name")
                {
                    ViewBag.Designs = sortedByType.ThenBy(d => d.Name);
                }
                else if (sortBy == "Quantity")
                {
                    ViewBag.Designs = sortedByType.ThenBy(d => d.Quantity);
                }
                else
                {
                    ViewBag.Designs = sortedByType;
                }
            }
            else
            {
                var designs = db.Designs;
                if (sortBy == "Id")
                {
                    ViewBag.Designs = designs.OrderBy(d => d.Id);
                }
                else if (sortBy == "Name")
                {
                    ViewBag.Designs = designs.OrderBy(d => d.Name);
                }
                else if (sortBy == "Quantity")
                {
                    ViewBag.Designs = designs.OrderBy(d => d.Quantity);
                }
                else if (sortBy == "Type")
                {
                    ViewBag.Designs = designs.OrderBy(d => d.Type);
                }
                else
                {
                    ViewBag.Designs = designs;
                }
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Add(Design design)
        {
            var db = new ApplicationContext();
            db.Designs.Add(design);
            db.SaveChanges();
            return Redirect("/Design/Show");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            var db = new ApplicationContext();
            ViewBag.Design = db.Designs.Find(id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(Design design)
        {
            var db = new ApplicationContext();
            Design des = db.Designs.Where(c => c.Id == design.Id).FirstOrDefault();

            des.Name = design.Name;
            des.Type = design.Type;
            des.Quantity = design.Quantity;

            db.SaveChanges();
            return Redirect("/Design/Show");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Remove(int id)
        {
            ApplicationContext db = new ApplicationContext();

            Design des = new Design
            {
                Id = id
            };

            db.Designs.Attach(des);
            db.Designs.Remove(des);

            db.SaveChanges();
            return Redirect("/Design/Show");
        }
    }
}
