using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.Tasks;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models;
using ProductionManagementSystem.WEB.Models.AltiumDB;
using ProductionManagementSystem.WEB.Models.Tasks;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize]
    public class CDBTasksController : Controller
    {
        private readonly ICDBTaskService _taskService;
        private readonly UserManager<User> _userManager;

        public CDBTasksController(ICDBTaskService taskService, UserManager<User> userManager)
        {
            _taskService = taskService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string orderBy, string q, 
            int? itemPerPage, int? page)
        {
            itemPerPage ??= 20;
            page ??= 1;
            
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var data = await _taskService.GetTasksByUserRoleAsync(roles.ToList());
            
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

        [HttpGet]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/cdbtasks/edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute]int id)
        {
            return View(id);
        }

        public IActionResult Create()
        {
            return View("Edit", 0);
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/tasks/alsoCreated")]
        public async Task<List<CDBTask>> AlsoCreated([FromForm]CDBTask task)
        {
            return await _taskService.AlsoCreatedAsync(task);
        }
        
        [HttpPost]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/tasks")]
        public async Task<ActionResult> AptCreate([FromForm]CDBTask task)
        {
            await _taskService.CreateAsync(task);
            return Ok();
        }
        
        [HttpPut]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/tasks")]
        public async Task<ActionResult> AptEdit([FromForm]CDBTask task)
        {
            await _taskService.UpdateAsync(task);
            return Ok();
        }
        
        [HttpDelete]
        [Authorize(Roles = RoleEnum.Admin)]
        [Route("/api/tasks/{id:int}")]
        public async Task<ActionResult> AptDelete([FromRoute]int id)
        {
            await _taskService.DeleteByIdAsync(id);
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
        
        [HttpGet]
        [Route("/api/tasks/{id:int}")]
        public async Task<ActionResult<CDBTask>> ApiDetails(int id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            
            return task;
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