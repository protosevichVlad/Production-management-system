using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationContext _context;
        
        public OrdersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET
        public IActionResult Index()
        {
            ViewBag.OrderComponents = _context.OrderComponents
                .Include(o => o.Task)
                .Include(o => o.Component)
                .ToList();
            ViewBag.OrderDesign = _context.OrderDesign
                .Include(o => o.Task)
                .Include(o => o.Design)
                .ToList();
            return View();
        }
        
        public IActionResult CreateDesign(int designId, int taskId)
        {
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm");
            OrderDesign order = new OrderDesign();
            ViewBag.Design = _context.Designs.FirstOrDefault(d => d.Id == designId);
            order.DateStart = DateTime.Now;
            ViewBag.TaskId = taskId;
            ViewBag.Tasks = new SelectList(_context.Tasks, "Id", "Id");
            ViewBag.Designs = new SelectList(_context.Designs, "Id", "Name");
            return View(order);
        }

        // POST: Designs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDesign(OrderDesign order, int TaskId, int DesignId)
        {
            if (ModelState.IsValid)
            {
                order.Design = _context.Designs.FirstOrDefault(d => d.Id == DesignId);
                order.Task = _context.Tasks.FirstOrDefault(t => t.Id == TaskId);
                _context.OrderDesign.Add(order);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }
        
        public IActionResult CreateComponent(int componentId, int taskId)
        {
            ViewBag.Date = DateTime.Now.ToString("yyyy-MM-ddThh:mm");
            OrderComponent order = new OrderComponent();
            ViewBag.Component = _context.Components.FirstOrDefault(d => d.Id == componentId);
            order.DateStart = DateTime.Now;
            ViewBag.TaskId = taskId;
            ViewBag.Tasks = new SelectList(_context.Tasks, "Id", "Id");
            ViewBag.Components = new SelectList(_context.Components, "Id", "Name");
            return View(order);
        }

        // POST: Designs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateComponent(OrderComponent order, int TaskId, int ComponentId)
        {
            if (ModelState.IsValid)
            {
                order.Component = _context.Components.FirstOrDefault(d => d.Id == ComponentId);
                order.Task = _context.Tasks.FirstOrDefault(t => t.Id == TaskId);
                _context.OrderComponents.Add(order);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }
    }
}