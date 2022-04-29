using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Services;
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

        public async Task<IActionResult> Index()
        {
            return View(new DataListViewModel<CDBTask>()
            {
                Data = await _taskService.GetAllAsync(),
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
            return View(await _taskService.GetByIdAsync(id));
        }

        [HttpPost]
        [Route("/api/tasks/{id:int}/changeStatus")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody]ChangeStatusViewModel model)
        {
            await _taskService.TransferAsync(id, model.Full, model.To, "");
            return Ok();
        }
    }
}