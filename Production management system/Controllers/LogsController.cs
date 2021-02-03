using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Controllers
{
    public class LogsController : Controller
    {
        private readonly ApplicationContext _context;

        public LogsController()
        {
            _context = new ApplicationContext();
        }

        public IActionResult Index(int? componentId, int? designId, string userLogin)
        {
            List<Log> logs;
            if (componentId is null && designId is null)
            {
                logs = _context.Logs.OrderByDescending(l => l.DateTime).ToList();
            }
            else if (componentId != null && designId != null)
            {
                return NotFound();
            }
            else if (designId != null)
            {
                logs = _context.LogsDesign
                    .Include(l => l.Log)
                    .Include(l => l.Design)
                    .Where(l => l.Design.Id == designId)
                    .Select(l => l.Log).ToList();
            }
            else
            {
                logs = _context.LogsComponent
                    .Include(l => l.Log)
                    .Include(l => l.Component)
                    .Where(l => l.Component.Id == componentId)
                    .Select(l => l.Log).ToList();
            }

            if (!(userLogin is null))
            {
                logs = logs.Where(l => l.UserLogin == userLogin).ToList();
            }
            
            return View(logs);
        }

        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var log = _context.Logs.FirstOrDefault(l => l.Id == id);
            if (log is null)
            {
                return NotFound();
            }

            var logDesign = _context.LogsDesign
                .Include(l => l.Design)
                .Include(l => l.Log)
                .FirstOrDefault(l => l.Log.Id == id);

            if (!(logDesign is null))
            {
                ViewBag.DesignId = logDesign.Design.Id;
            }
            
            var logComponent = _context.LogsComponent
                .Include(l => l.Component)
                .Include(l => l.Log)
                .FirstOrDefault(l => l.Log.Id == id);

            if (!(logComponent is null))
            {
                ViewBag.ComponentId = logComponent.Component.Id;
            }
            
            return View(log);
        }
    }
}