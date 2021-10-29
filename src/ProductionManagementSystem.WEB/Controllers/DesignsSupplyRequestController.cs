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
    public class DesignsSupplyRequestController : Controller
    {
        private readonly IDesignSupplyRequestService _designSupplyRequestService;
        private readonly ITaskService _taskService;
        private readonly IDesignService _designService;

        public DesignsSupplyRequestController(IDesignSupplyRequestService designSupplyRequestService, ITaskService taskService, ILogService logService, IDesignService designService)
        {
            _designSupplyRequestService = designSupplyRequestService;
            _designService = designService;
            _taskService = taskService;
        }

        public async Task<ViewResult> Index()
        {
            return View(_designSupplyRequestService.GetAll()
                .OrderBy(d => d.StatusSupply)
                .ThenBy(d => d.DesiredDate).Select(s =>
                {
                    s.Design = _designService.GetByIdAsync(s.ComponentId).Result;
                    return s;
                }));
        }

        public async Task<ViewResult> Create(int? taskId, int? designId)
        {
            DesignSupplyRequest viewModel = new DesignSupplyRequest
            {
                ComponentId = designId ?? 0,
                DesiredDate = DateTime.Now
            };
            if (taskId.HasValue)
            {
                viewModel.TaskId = taskId;
                ViewBag.Designs = (await _taskService.GetByIdAsync(viewModel.TaskId.Value)).ObtainedDesigns
                    .Select(c =>  new SelectListItem(
                        _designService.GetByIdAsync(c.ComponentId).Result.ToString(),
                        c.ComponentId.ToString()
                    )).AsEnumerable();
            }
            else
            {
                ViewBag.Designs = _designService.GetAll()
                    .Select(c => new SelectListItem(c.ToString(), c.Id.ToString())).AsEnumerable();
            }
            
            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(DesignSupplyRequest viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.DateAdded = DateTime.Now;
                viewModel.StatusSupply = SupplyStatusEnum.NotAccepted;
                await _designSupplyRequestService.CreateAsync(viewModel);
                return RedirectToAction(nameof(Index));
            }
            
            return View(nameof(Create), viewModel);
        }

        public async Task<IActionResult> ChangeStatus(int supplyRequestId, int to, string message)
        {
            await _designSupplyRequestService.ChangeStatusAsync(supplyRequestId, to, message);
            return RedirectToAction(nameof(Details), new {id = supplyRequestId});
        }

        public async Task<ViewResult> Details(int id)
        {
            var viewModel = await _designSupplyRequestService.GetByIdAsync(id);
            viewModel.Design = await _designService.GetByIdAsync(viewModel.ComponentId);

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
            return View(await _designSupplyRequestService.GetByIdAsync(id));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(DesignSupplyRequest viewModel)
        {
            if (ModelState.IsValid)
            {
                await _designSupplyRequestService.UpdateAsync(viewModel);
                return RedirectToAction(nameof(Details), new {id = viewModel.Id});
            }
            
            return View(viewModel);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var viewModel = await _designSupplyRequestService.GetByIdAsync(id);
            viewModel.Design = await _designService.GetByIdAsync(viewModel.ComponentId);
            return View(viewModel);
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _designSupplyRequestService.DeleteByIdAsync(id);
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