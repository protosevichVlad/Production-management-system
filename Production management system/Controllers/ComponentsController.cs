using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class ComponentsController : Controller
    {
        private readonly ApplicationContext _context;

        public ComponentsController()
        {
            _context = new ApplicationContext();
        }

        // GET: Components
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            var components = from s in _context.Components
                select s;
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
            return View(await components.ToListAsync());
        }

        // GET: Components/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components
                .FirstOrDefaultAsync(m => m.Id == id);
            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // GET: Components/Create
        public IActionResult Create()
        {
            return View();
        }
        
        public IActionResult CreateMore()
        {
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int count = 200;
            var random = new Random();
            List<Component> components = new List<Component>();
            for (int i = 0; i < count; i++)
            {
                components.Add(new Component
                {
                    Name = $"{letters[random.Next(letters.Length)]}Name{i}",
                    Corpus = $"{letters[random.Next(letters.Length)]}Corpus{i}",
                    Explanation = $"{letters[random.Next(letters.Length)]}Explanation{i}",
                    Manufacturer = $"{letters[random.Next(letters.Length)]}Manufacturer{i}",
                    Nominal = $"{letters[random.Next(letters.Length)]}Nominal{i}",
                    Type = $"Type{random.Next(10)}",
                    Quantity = random.Next(10000),
                });
            }
            _context.Components.AddRange(components);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // POST: Components/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Name,Nominal,Corpus,Explanation,Manufacturer,Quantity")] Component component)
        {
            if (ModelState.IsValid)
            {
                _context.Add(component);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(component);
        }

        // GET: Components/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components.FindAsync(id);
            if (component == null)
            {
                return NotFound();
            }
            return View(component);
        }

        // POST: Components/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Name,Nominal,Corpus,Explanation,Manufacturer,Quantity")] Component component)
        {
            if (id != component.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(component);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComponentExists(component.Id))
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
            return View(component);
        }

        // GET: Components/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var component = await _context.Components
                .FirstOrDefaultAsync(m => m.Id == id);
            if (component == null)
            {
                return NotFound();
            }

            return View(component);
        }

        // POST: Components/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var component = await _context.Components.FindAsync(id);
            _context.Components.Remove(component);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComponentExists(int id)
        {
            return _context.Components.Any(e => e.Id == id);
        }
        
        [HttpGet]
        public async Task<JsonResult> GetAllComponents()
        {
            List<Component> components = await _context.Components.OrderBy(c => c.Name).ToListAsync();
            return Json(components);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllTypes()
        {
            List<string> types = await _context.Components.OrderBy(c => c.Type).Select(c => c.Type).ToListAsync();
            types = types.Distinct().ToList();
            return Json(types);
        }
        
        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add(int taskId, int componentId)
        {

            ViewBag.TaskId = taskId;
            var component = _context.Components.FirstOrDefault(c => c.Id == componentId);
            if (component is null)
            {
                return NotFound();
            }
            
            return View(component);
        }
        [HttpPost]
        public IActionResult Add(int taskId, int componentId, int quantity)
        {
            var component = _context.Components.FirstOrDefault(c => c.Id == componentId);
            if (component is null)
            {
                return NotFound();
            }
            
            component.Quantity += quantity;
            _context.SaveChanges();
            return Redirect($"/Task/ShowTask/{taskId}");
        }
    }
}
