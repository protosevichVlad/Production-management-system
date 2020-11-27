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
        private ApplicationContext _context;

        public DesignController(ILogger<DesignController> logger)
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
                var sortedByType = _context.Designs.OrderBy(d => d.Type);
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
                var designs = _context.Designs;
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
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add(Design design)
        {
            _context.Designs.Add(design);
            _context.SaveChanges();
            return Redirect("/Design/Show");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(int id)
        {
            ViewBag.Design = _context.Designs.Find(id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(Design design)
        {
            Design des = _context.Designs.Where(c => c.Id == design.Id).FirstOrDefault();

            des.Name = design.Name;
            des.Type = design.Type;
            des.Quantity = design.Quantity;

            _context.SaveChanges();
            return Redirect("/Design/Show");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Remove(int id)
        {
            Design des = new Design
            {
                Id = id
            };

            _context.Designs.Attach(des);
            _context.Designs.Remove(des);

            _context.SaveChanges();
            return Redirect("/Design/Show");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Adding(int taskId, int designId, int addedQuntity = 0)
        {
            if (addedQuntity == 0)
            {
                ViewBag.TaskId = taskId;
                ViewBag.Design = _context.Designs.Where(d => d.Id == designId).FirstOrDefault(); ;
                return View();
            }
            else
            {
                Design design = _context.Designs.Where(d => d.Id == designId).FirstOrDefault();
                design.Quantity += addedQuntity;
                _context.SaveChanges();
                return Redirect($"/Task/ShowTask/{taskId}");
            }
        }

        public JsonResult GetAllDesigns()
        {
            List<Design> designs = _context.Designs.OrderBy(d => d.Name).ToList();
            return Json(designs);
        }
    }
}
