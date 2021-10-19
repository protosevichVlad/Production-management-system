using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.WEB.Models;
using ProductionManagementSystem.WEB.Models.Modals;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize]
    public class DesignsSupplyRequestController : Controller
    {
        private readonly IDesignsSupplyRequestService _designsSupplyRequestService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly ITaskService _taskService;
        private readonly IDesignService _designService;

        public DesignsSupplyRequestController(IDesignsSupplyRequestService designsSupplyRequestService, ITaskService taskService, ILogService logService, IDesignService designService)
        {
            _designsSupplyRequestService = designsSupplyRequestService;
            _logService = logService;
            _designService = designService;
            _taskService = taskService;
            
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DesignsSupplyRequestDTO, DesignsSupplyRequestViewModel>();
                    cfg.CreateMap<DesignsSupplyRequestViewModel, DesignsSupplyRequestDTO>();
                    cfg.CreateMap<TaskDTO, TaskViewModel>()
                        .ForMember(
                            task => task.Status, 
                            opt => opt.MapFrom(
                                src => _taskService.GetTaskStatusName(src.Status)
                            )
                        );
                    cfg.CreateMap<TaskViewModel, TaskDTO>();
                    cfg.CreateMap<DeviceDTO, DeviceViewModel>();
                    cfg.CreateMap<LogDTO, LogViewModel>();
                    cfg.CreateMap<ObtainedDesign, ObtainedDesignDTO>();
                    cfg.CreateMap<ObtainedDesignDTO, ObtainedDesign>();
                    cfg.CreateMap<ObtainedComponent, ObtainedComponentDTO>();
                    cfg.CreateMap<ObtainedComponentDTO, ObtainedComponent>();
                    cfg.CreateMap<DesignViewModel, DesignDTO>();
                    cfg.CreateMap<DesignDTO, DesignViewModel>();
                    cfg.CreateMap<StatusSupplyEnum, StatusSupplyEnumDTO>();
                    cfg.CreateMap<StatusSupplyEnumDTO, StatusSupplyEnum>();
                    cfg.CreateMap<UserViewModel, UserDTO>();
                    cfg.CreateMap<UserDTO, UserViewModel>();
                }).CreateMapper();
        }


        public async Task<ViewResult> Index()
        {
            return View(_mapper.Map < IEnumerable<DesignsSupplyRequestDTO>,
                IEnumerable<DesignsSupplyRequestViewModel>>(
                await _designsSupplyRequestService.GetDesignSupplyRequestsAsync()));
        }

        public async Task<ViewResult> Create(int? taskId, int? designId)
        {
            DesignsSupplyRequestViewModel viewModel = new DesignsSupplyRequestViewModel
            {
                DesignId = designId ?? 0,
                DesiredDate = DateTime.Now
            };
            if (taskId.HasValue)
            {
                viewModel.TaskId = taskId;
                ViewBag.Designs = (await _taskService.GetDeviceDesignTemplateFromTaskAsync(viewModel.TaskId.Value))
                    .Select(c =>  new SelectListItem(
                        _designService.GetDesignAsync(c.DesignId).Result.ToString(),
                        c.DesignId.ToString()
                    )).AsEnumerable();
            }
            else
            {
                ViewBag.Designs = (await _designService.GetDesignsAsync())
                    .Select(c => new SelectListItem(c.ToString(), c.Id.ToString())).AsEnumerable();
            }
            
            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(DesignsSupplyRequestViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.DateAdded = DateTime.Now;
                // TODO User object
                // viewModel.User.UserName = User.Identity?.Name;
                viewModel.StatusSupply = StatusSupplyEnum.NotAccepted;

                await _designsSupplyRequestService.CreateDesignSupplyRequestAsync(
                    _mapper.Map<DesignsSupplyRequestViewModel, DesignsSupplyRequestDTO>(viewModel));
                return RedirectToAction(nameof(Index));
            }
            
            return View(nameof(Create), viewModel);
        }

        public async Task<IActionResult> ChangeStatus(int supplyRequestId, int to, string message)
        {
            LogService.UserName = User.Identity?.Name;
            await _designsSupplyRequestService.ChangeStatusAsync(supplyRequestId, to, message);
            return RedirectToAction(nameof(Details), new {id = supplyRequestId});
        }

        public async Task<ViewResult> Details(int? id)
        {
            var viewModel = _mapper.Map<DesignsSupplyRequestDTO, DesignsSupplyRequestViewModel>(
                await _designsSupplyRequestService.GetDesignSupplyRequestAsync(id));

            ViewBag.Modal = new ModalSupplyRequest
            {
                States = new SelectList(GetNameOfStatusSupply(), "Id", "Name"),
                NameModal = "changeStatus",
                SupplyRequestId = id.Value,
            };
            
            return View(viewModel);
        }
        
        public async Task<ViewResult> Edit(int id)
        {
            return View(_mapper.Map<DesignsSupplyRequestDTO, DesignsSupplyRequestViewModel>(await _designsSupplyRequestService.GetDesignSupplyRequestAsync(id)));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(DesignsSupplyRequestViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _designsSupplyRequestService.UpdateDesignSupplyRequestAsync(
                    _mapper.Map<DesignsSupplyRequestViewModel, DesignsSupplyRequestDTO>(viewModel));
                return RedirectToAction(nameof(Details), new {id = viewModel.Id});
            }
            
            return View(viewModel);
        }

        public async Task<ActionResult> Delete(int id)
        {
            return View(
                _mapper.Map<DesignsSupplyRequestDTO, DesignsSupplyRequestViewModel>(
                    await _designsSupplyRequestService.GetDesignSupplyRequestAsync(id)));
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _designsSupplyRequestService.DeleteDesignSupplyRequestAsync(id);
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