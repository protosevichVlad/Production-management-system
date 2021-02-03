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
        /// <summary>
        /// The recording of information on the change object
        /// </summary>
        /// <param name="message">Description of changes</param>
        /// <param name="component">Element to change</param>
        private void Log(string message, Component component=null)
        {
            var log = new Log() {DateTime = DateTime.Now, UserLogin = User.Identity.Name, Message = message};
            _context.Logs.Add(log);
            if (component != null)
            {
                var logComponent = new LogComponent() {Log = log, Component = component};
                _context.LogsComponent.Add(logComponent);
            }
            
            _context.SaveChanges();
        }

        // GET: Components
        /// <summary>
        /// Display index page
        /// </summary>
        /// <param name="sortOrder">Used for sorting</param>
        /// <param name="searchString">Used for searching</param>
        /// <returns>A page with all components sorted by parameter <paramref name="sortOrder"/> and satisfying <paramref name="searchString"/></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["CurrentFilter"] = searchString;
            
            var components = from s in _context.Components
                select s;
            
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
                Log($"Был создан: {component}.", component);
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
                    Log($"Был изменён {component}.", component);
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
            var logs = _context.LogsComponent.Where(l => l.ComponentId == id).ToList();
            foreach (var log in logs)
            {
                _context.LogsComponent.Remove(log);
            }
            
            Log($"Был удалён {component}.");
            _context.Components.Remove(component);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComponentExists(int id)
        {
            return _context.Components.Any(e => e.Id == id);
        }
        
        /// <summary>
        /// Method for getting all components in the form:
        /// {component.Name} {component.Nominal} {component.Corpus}
        /// </summary>
        /// <returns>JSON. Array of all components</returns>
        [HttpGet]
        public async Task<JsonResult> GetAllComponents()
        {
            List<Component> components = await _context.Components.OrderBy(c => c.Name).ToListAsync();
            foreach (var comp in components)
            {
                comp.Name = $"{comp.Name} {comp.Nominal} {comp.Corpus}";
            }
            
            return Json(components);
        }

        /// <summary>
        /// Method for getting all types of components
        /// </summary>
        /// <returns>JSON. Array of all types of components</returns>
        [HttpGet]
        public async Task<JsonResult> GetAllTypes()
        {
            List<string> types = await _context.Components.OrderBy(c => c.Type).Select(c => c.Type).ToListAsync();
            types = types.Distinct().ToList();
            return Json(types);
        }
        
        /// <summary>
        /// Page with the addition to the component.
        /// </summary>
        /// <param name="id">Id of the component to add.</param>
        /// <param name="taskId">Task ID for quick return</param>
        /// <returns>Page with form</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add(int id, int? taskId)
        {

            var component = _context.Components.FirstOrDefault(d => d.Id == id);
            if (component == null)
            {
                return NotFound();
            }

            ViewBag.TaskId = taskId;
            return View(component);
        }
        
        /// <summary>
        /// Adding logic
        /// </summary>
        /// <param name="componentId">Id of the component to add.</param>
        /// <param name="quantity">Quantity to add</param>
        /// <param name="taskId">Task ID for quick return</param>
        /// <returns>Page /Components or the page with the task from which this page was called</returns>
        [HttpPost]
        public IActionResult Add(int componentId, int quantity, int? taskId)
        {

            var component = _context.Components.FirstOrDefault(d => d.Id == componentId);
            if (component is null)
            {
                return NotFound();
            }
            component.Quantity += quantity;
            Log($"Было добавлено {quantity}шт. После добавления: {component}.", component);
            _context.SaveChanges();
            if (taskId != null)
            {
                return RedirectToAction("ShowTask", "Task", new {id = taskId});
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        /// <summary>
        /// Page with the access to the component
        /// </summary>
        /// <param name="id">Id of the component to receive.</param>
        /// <returns>Page with form</returns>
        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Receive(int id)
        {

            var component = _context.Components.FirstOrDefault(d => d.Id == id);
            if (component is null)
            {
                return NotFound();
            }
            
            return View(component);
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

            Component component = _context.Components.FirstOrDefault(d => d.Id == componentId);
            if (component is null)
            {
                return NotFound();
            }
            
            component.Quantity -= quantity;
            Log($"Было получено {quantity}шт. После получения: {component}.", component);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
