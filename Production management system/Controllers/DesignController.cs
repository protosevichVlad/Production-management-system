using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace ProductionManagementSystem.Controllers
{
    public class DesignController : Controller
    {
        private readonly ApplicationContext _context;

        public DesignController()
        {
            _context = new ApplicationContext();
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Index(string sortBy, string splitByType)
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // [Authorize(Roles = "admin, order_picker")]
        public IActionResult Create(Design design)
        {
            _context.Designs.Add(design);
            _context.SaveChanges();
            return Redirect("/Design/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(int id)
        {
            return View(_context.Designs.FirstOrDefault(d => d.Id == id));
        }

        [HttpPost]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Edit(Design design)
        {
            Design des = _context.Designs.FirstOrDefault(c => c.Id == design.Id);

            if (des is null)
            {
                return NotFound();
            }

            des.Name = design.Name;
            des.Type = design.Type;
            des.Quantity = design.Quantity;

            _context.SaveChanges();
            return Redirect("/Design/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Adding(int taskId, int designId, int addedQuntity = 0)
        {
            if (addedQuntity == 0)
            {
                ViewBag.TaskId = taskId;
                ViewBag.Design = _context.Designs.FirstOrDefault(d => d.Id == designId);
                return View();
            }
            else
            {
                Design design = _context.Designs.FirstOrDefault(d => d.Id == designId);
                if (design is null)
                {
                    return NotFound();
                }
                
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

        public JsonResult GetAllTypes()
        {
            List<string> types = _context.Designs.OrderBy(d => d.Type).Select(d => d.Type).ToList();
            types = types.Distinct().ToList();
            return Json(types);
        }
        
        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var design = _context.Designs
                .FirstOrDefault(m => m.Id == id);
            if (design is null)
            {
                return NotFound();
            }

            return View(design);
        }
        
        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var design = _context.Designs
                .FirstOrDefault(m => m.Id == id);
            if (design is null)
            {
                return NotFound();
            }

            return View(design);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var design =  _context.Designs.FirstOrDefault(d => d.Id ==id);
            if (design is null)
            {
                return NotFound();
            }
            
            _context.Designs.Attach(design);
            _context.Designs.Remove(design);
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
