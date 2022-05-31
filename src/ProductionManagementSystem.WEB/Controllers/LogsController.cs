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
using ProductionManagementSystem.WEB.Models;
using ProductionManagementSystem.WEB.Models.AltiumDB;

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

        public async Task<IActionResult> Index(string userName, 
            int? deviceId, int? montageId, int? designId, int? taskId, int? orderId,
            int? designSupplyRequestId, int? montageSupplyRequestId,
            string orderBy, Dictionary<string, List<string>> filter, 
            string q, int? tableId, int? itemPerPage, int? page)
        {
            IEnumerable<Log> logs = await _logService.GetAllAsync();
            var total = logs.Count();

            if (userName != null)
                logs = logs.Where(l => l.User.UserName == userName);
            
            if (deviceId != null)
                logs = logs.Where(l => l.ItemId == deviceId && l.ItemType == LogsItemType.Device);
            
            if (montageId != null)
                logs = logs.Where(l => l.ItemId == montageId && l.ItemType == LogsItemType.Montage);
            
            if (designId != null)
                logs = logs.Where(l => l.ItemId == designId && l.ItemType == LogsItemType.Design);
            
            if (taskId != null)
                logs = logs.Where(l => l.ItemId == taskId && l.ItemType == LogsItemType.Task);
            
            if (orderId != null)
                logs = logs.Where(l => l.ItemId == orderId && l.ItemType == LogsItemType.Order);
            
            if (montageSupplyRequestId != null)
                logs = logs.Where(l => l.ItemId == montageSupplyRequestId && l.ItemType == LogsItemType.MontageSupplyRequest);
            
            if (designSupplyRequestId != null)
                logs = logs.Where(l => l.ItemId == designSupplyRequestId && l.ItemType == LogsItemType.DesignSupplyRequest);

            logs = logs.Skip(((page ?? 1) - 1) * (itemPerPage ?? 20)).Take(itemPerPage ?? 20);
            
            return View(new DataListViewModel<Log>()
            {
                Data = logs.ToList(),
                Filters = null,
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = page ?? 1,
                    TotalItems = total,
                    ItemPerPage = itemPerPage ?? 20,
                }
            });
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