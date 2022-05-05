using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.Tasks;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models;
using ProductionManagementSystem.WEB.Models.AltiumDB;
using ProductionManagementSystem.WEB.Models.Tasks;
using Task = ProductionManagementSystem.Core.Models.Tasks.Task;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class CDBTasksController : Controller
    {
        private readonly ICDBTaskService _taskService;

        public CDBTasksController(ICDBTaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<IActionResult> Index(string orderBy, string q, 
            int? itemPerPage, int? page)
        {
            itemPerPage ??= 20;
            page ??= 1;
            
            var data = await _taskService.GetAllAsync();
            int totalItems = data.Count();
            data = data.Skip(itemPerPage.Value * (page.Value - 1)).Take(itemPerPage.Value).ToList();
            
            return View(new DataListViewModel<CDBTask>()
            {
                Data = data,
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = page.Value,
                    TotalItems = totalItems,
                    ItemPerPage = itemPerPage.Value
                }
            });
        }

        [HttpPut]
        [Route("/cdbtasks/edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View("Edit", 0);
        }
        
        [HttpPost]
        [Route("/api/tasks/alsoCreated")]
        public async Task<List<CDBTask>> AlsoCreated([FromForm]CDBTask task)
        {
            return await _taskService.AlsoCreatedAsync(task);
        }
        
        [HttpPost]
        [Route("/api/tasks")]
        public async Task<ActionResult> AptCreate([FromForm]CDBTask task)
        {
            await _taskService.CreateAsync(task);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(task);
        }

        [HttpPost]
        [Route("/api/tasks/{id:int}/changeStatus")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody]ChangeStatusViewModel model)
        {
            await _taskService.TransferAsync(id, model.Full, model.To, "");
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(task);
        }
        
        [HttpPost]
        
        public async Task<IActionResult> Get(int taskId, List<ObtainedModel> obtainedModels)
        {
            var task = await _taskService.GetByIdAsync(taskId);
            if (task == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            await _taskService.ObtainItems(taskId, obtainedModels);
            return View(task);
        }
    }
}