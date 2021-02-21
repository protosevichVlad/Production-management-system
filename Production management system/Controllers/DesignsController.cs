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

        public DesignsController(ApplicationContext context)
        {
            _context = context;
        }
        
        private void Log(string message, Design design=null)
        {
            var log = new Log() {DateTime = DateTime.Now, UserLogin = User.Identity.Name, Message = message};
            _context.Logs.Add(log);
            if (design != null)
            {
                var logDesign = new LogDesign() {Log = log, Design = design};
                _context.LogsDesign.Add(logDesign);
            }
            
            _context.SaveChanges();
        }
        
        // GET: Designs
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TypeSortParm"] = sortOrder == "Type" ? "type_desc" : "Type";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["CurrentFilter"] = searchString;
            var designs = from s in _context.Designs
                select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                designs = designs.Where(d => d.Name.Contains(searchString)
                                             || d.Type.Contains(searchString)
                                             || d.ShortDescription.Contains(searchString));
            }
            
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
        public async Task<IActionResult> Create()
        {
            ViewBag.AllTypes = await GetAllTypes();
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
                    ShortDescription = $"ShortDescription{random.Next(100)}",
                    Description = $"Description{random.Next(100)}",
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
        public async Task<IActionResult> Create([Bind("Id,Type,Name,Quantity,ShortDescription,Description")] Design design)
        {
            if (ModelState.IsValid)
            {
                _context.Add(design);
                await _context.SaveChangesAsync();
                Log($"Был создан: {design}.", design);
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
            
            ViewBag.AllTypes = await GetAllTypes();
            return View(design);
        }

        // POST: Designs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Name,Quantity,ShortDescription,Description")] Design design)
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
                    // TODO: delete
                    var designInDb = _context.Designs.FirstOrDefault(d => d.Id == id);
                    Log($"Был изменён {design}.", design);
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
            var logs = _context.LogsDesign.Where(l => l.DesignId == id).ToList();
            foreach (var log in logs)
            {
                _context.LogsDesign.Remove(log);
            }
            
            if (!CheckInDevices(design, out string errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
                return View(design);
            }

            _context.Designs.Remove(design);
            await _context.SaveChangesAsync();
            Log($"Был удалён {design}.");
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool CheckInDevices(Design design , out string errorMessage)
        {
            var designInDevice = _context.DeviceDesignTemplate
                            .FirstOrDefault(d => design.Id == d.DesignId);
            if (designInDevice != null)
            {
                var device = _context.Devices.FirstOrDefault(d => d.Id == designInDevice.DeviceId);
                errorMessage = "Невозможно удаление конструктива.<br />" +
                               $"<i class='bg-light'>{design}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{design}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return false;
            }

            errorMessage = "";
            return true;
        }

        private bool DesignExists(int id)
        {
            return _context.Designs.Any(e => e.Id == id);
        }
        
        [HttpGet]
        public async Task<JsonResult> GetAllDesigns()
        {
            List<Design> designs = await _context.Designs.OrderBy(d => d.Name).ToListAsync();
            foreach (var des in designs)
            {
                des.Name = des.ToString();
            }
            return Json(designs);
        }

        [NonAction]
        private async Task<List<string>> GetAllTypes()
        {
            List<string> types = await  _context.Designs.OrderBy(d => d.Type).Select(d => d.Type).ToListAsync();
            types = types.Distinct().ToList();
            return types;
        }

        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Add(int id, int? taskId)
        {

            var design = _context.Designs.FirstOrDefault(d => d.Id == id);
            if (design is null)
            {
                return NotFound();
            }
            
            ViewBag.TaskId = taskId;
            return View(design);
        }
        
        [HttpPost]
        public IActionResult Add(int designId, int quantity, int taskId)
        {

            Design design = _context.Designs.FirstOrDefault(d => d.Id == designId);
            if (design is null)
            {
                return NotFound();
            }
            design.Quantity += quantity;
            Log($"Было добавлено {quantity}шт. После добавления: {design}.", design);
            _context.SaveChanges();
            
            if (taskId != null)
            {
                return RedirectToAction("Details", "Tasks", new {id = taskId});
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        [Authorize(Roles = "admin, order_picker")]
        public IActionResult Receive(int id)
        {

            var design = _context.Designs.FirstOrDefault(d => d.Id == id);
            if (design is null)
            {
                return NotFound();
            }
            
            return View(design);
        }
        [HttpPost]
        public IActionResult Receive(int designId, int quantity)
        {

            Design design = _context.Designs.FirstOrDefault(d => d.Id == designId);
            if (design is null)
            {
                return NotFound();
            }
            
            design.Quantity -= quantity;
            Log($"Было получено {quantity}шт. После получения: {design}.", design);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
