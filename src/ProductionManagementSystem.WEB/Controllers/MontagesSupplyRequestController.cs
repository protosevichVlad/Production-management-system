using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.SupplyRequests;
using ProductionManagementSystem.WEB.Models;
using ProductionManagementSystem.WEB.Models.Modals;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize]
    public class MontagesSupplyRequestController : Controller
    {
        private readonly IMontageSupplyRequestService _montageSupplyRequestService;
        private readonly ILogService _logService;
        private readonly ITaskService _taskService;
        private readonly IMontageService _montageService;

        public MontagesSupplyRequestController(IMontageSupplyRequestService componentsSupplyRequestService, ITaskService taskService, ILogService logService, IMontageService componentService)
        {
            _montageSupplyRequestService = componentsSupplyRequestService;
            _logService = logService;
            _montageService = componentService;
            _taskService = taskService;
        }

        public async Task<ViewResult> Index()
        {
            return View((await _montageSupplyRequestService.GetAll())
                .OrderBy(c => c.StatusSupply)
                .ThenBy(c => c.DesiredDate).Select(s =>
                {
                    s.Montage = _montageService.GetByIdAsync(s.ComponentId).Result;
                    return s;
                }));
        }

        public async Task<ViewResult> Create(int? taskId, int? componentId)
        {
            MontageSupplyRequest viewModel = new MontageSupplyRequest
            {
                ComponentId = componentId ?? 0,
                DesiredDate = DateTime.Now
            };
            if (taskId.HasValue)
            {
                viewModel.TaskId = taskId;
                ViewBag.Components = (await _taskService.GetByIdAsync(viewModel.TaskId.Value)).ObtainedMontages
                    .Select(c =>  new SelectListItem(
                        _montageService.GetByIdAsync(c.ComponentId).Result.ToString(),
                        c.ComponentId.ToString()
                    )).AsEnumerable();
            }
            else
            {
                ViewBag.Components = (await _montageService.GetAll())
                    .Select(c => new SelectListItem(c.ToString(), c.Id.ToString())).AsEnumerable();
            }
            
            return View(viewModel);
        }
        
        [HttpPost("Create")]
        public async Task<IActionResult> CreatePost(MontageSupplyRequest viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.DateAdded = DateTime.Now;
                // TODO User object
                // viewModel.User.UserName = User.Identity?.Name;
                viewModel.StatusSupply = SupplyStatusEnum.NotAccepted;

                await _montageSupplyRequestService.CreateAsync(viewModel);
                return RedirectToAction(nameof(Index));
            }
            
            return View(nameof(Create), viewModel);
        }

        public async Task<IActionResult> ChangeStatus(int supplyRequestId, int to, string message)
        {
            await _montageSupplyRequestService.ChangeStatusAsync(supplyRequestId, to, message);
            return RedirectToAction(nameof(Details), new {id = supplyRequestId});
        }

        public async Task<ViewResult> Details(int id)
        {
            var viewModel = await _montageSupplyRequestService.GetByIdAsync(id);
            viewModel.Montage = await _montageService.GetByIdAsync(viewModel.ComponentId);

            ViewBag.Modal = new ModalSupplyRequest
            {
                States = new SelectList(GetNameOfStatusSupply(), "Id", "Name"),
                NameModal = "changeStatus",
                SupplyRequestId = id,
            };
            
            return View(viewModel);
        }
        
        public async Task<ViewResult> Edit(int id)
        {
            return View(await _montageSupplyRequestService.GetByIdAsync(id));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(MontageSupplyRequest viewModel)
        {
            if (ModelState.IsValid)
            {
                await _montageSupplyRequestService.UpdateAsync(viewModel);
                return RedirectToAction(nameof(Details), new {id = viewModel.Id});
            }
            
            return View(viewModel);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var viewModel = await _montageSupplyRequestService.GetByIdAsync(id);
            viewModel.Montage = await _montageService.GetByIdAsync(viewModel.ComponentId);
            return View(viewModel);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _montageSupplyRequestService.DeleteByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<object> GetNameOfStatusSupply()
        {
            var result = new List<string>();
            
            foreach (var status in Enum.GetValues(typeof(StatusSupplyEnum)))
            {
                result.Add(status.GetType()
                    .GetMember(status.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>()
                    ?.GetName());
            }

            return result;
        }
    }
}