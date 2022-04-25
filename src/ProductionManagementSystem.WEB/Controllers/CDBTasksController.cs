using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class CDBTasksController : Controller
    {
        private readonly ICDBTaskService _taskService;

        public CDBTasksController(ICDBTaskService taskService)
        {
            _taskService = taskService;
        }

        public IActionResult Index()
        {
            return View(new DataListViewModel<CDBTask>());
        }

        [HttpGet]
        [Route("/cdbtasks/edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View("Edit");
        }
        
        [HttpGet]
        [Route("/api/tasks/alsoCreated")]
        public async Task<List<CDBTask>> AlsoCreated([FromBody]CDBTask task)
        {
            return await _taskService.AlsoCreatedAsync(task);
        }
    }
}