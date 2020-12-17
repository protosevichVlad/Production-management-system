using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class DesignsController : Controller
    {
        private readonly ApplicationContext _context;

        public DesignsController()
        {
            _context = new ApplicationContext();
        }

        // GET: Designs
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            var designs = from s in _context.Designs
                select s;
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
            return View(await designs.ToListAsync());
        }

        // GET: Designs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var design = await _context.Designs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (design == null)
            {
                return NotFound();
            }

            return View(design);
        }

        // GET: Designs/Create
        public IActionResult Create()
        {
            return View();
        }
        
        // GET: Designs/Create
        public IActionResult CreateMore()
        {
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int count = 200;
            var random = new Random();
            List<Design> designs = new List<Design>();
            for (int i = 0; i < count; i++)
            {
                designs.Add(new Design
                {
                    Name = $"{letters[random.Next(letters.Length)]}Name{i}",
                    Type = $"Type{random.Next(10)}",
                    Quantity = random.Next(10000),
                });
            }
            _context.Designs.AddRange(designs);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: Designs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Name,Quantity")] Design design)
        {
            if (ModelState.IsValid)
            {
                _context.Add(design);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(design);
        }

        // GET: Designs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var design = await _context.Designs.FindAsync(id);
            if (design == null)
            {
                return NotFound();
            }
            return View(design);
        }

        // POST: Designs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Name,Quantity")] Design design)
        {
            if (id != design.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(design);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesignExists(design.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(design);
        }

        // GET: Designs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var design = await _context.Designs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (design == null)
            {
                return NotFound();
            }

            return View(design);
        }

        // POST: Designs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var design = await _context.Designs.FindAsync(id);
            _context.Designs.Remove(design);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DesignExists(int id)
        {
            return _context.Designs.Any(e => e.Id == id);
        }
        
        [HttpGet]
        public async Task<JsonResult> GetAllDesigns()
        {
            List<Design> designs = await _context.Designs.OrderBy(d => d.Name).ToListAsync();
            return Json(designs);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllTypes()
        {
            List<string> types = await  _context.Designs.OrderBy(d => d.Type).Select(d => d.Type).ToListAsync();
            types = types.Distinct().ToList();
            return Json(types);
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add(int taskId, int designId)
        {

            ViewBag.TaskId = taskId;
            var design = _context.Designs.FirstOrDefault(d => d.Id == designId);
            if (design is null)
            {
                return NotFound();
            }
            
            return View(design);
        }
        [HttpPost]
        public IActionResult Add(int taskId, int designId, int quantity)
        {

            Design design = _context.Designs.FirstOrDefault(d => d.Id == designId);
            if (design is null)
            {
                return NotFound();
            }
            
            design.Quantity += quantity;
            _context.SaveChanges();
            return Redirect($"/Task/ShowTask/{taskId}");
        }
    }
}
