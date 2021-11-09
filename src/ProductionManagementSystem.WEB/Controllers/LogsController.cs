using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Infrastructure;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles = RoleEnum.Admin)]
    public class LogsController : Controller
    {
        private readonly ILogService _logService;

        public LogsController(ILogService service)
        {
            _logService = service;
        }

        public async Task<IActionResult> Index(string userName, int? deviceId, int? componentId, int? designId, int? taskId, int? orderId)
        {
            IEnumerable<Log> logs = await _logService.GetAllAsync();

            if (userName != null)
            {
                logs = logs.Where(l => l.User.UserName == userName);
            }
            
            if (deviceId != null)
            {
                logs = logs.Where(l => l.DesignId == deviceId);
            }
            
            if (componentId != null)
            {
                logs = logs.Where(l => l.MontageId == componentId);
            }
            
            if (designId != null)
            {
                logs = logs.Where(l => l.DesignId == designId);
            }
            
            if (taskId != null)
            {
                logs = logs.Where(l => l.TaskId == taskId);
            }
            
            if (orderId != null)
            {
                logs = logs.Where(l => l.OrderId == orderId);
            }
            
            return View(logs);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                return View(await _logService.GetByIdAsync(id));
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }
    }
}