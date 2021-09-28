﻿using System;
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
    public class ComponentsSupplyRequestController : Controller
    {
        private readonly IComponentsSupplyRequestService _componentsSupplyRequestService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly ITaskService _taskService;

        public ComponentsSupplyRequestController(IComponentsSupplyRequestService componentsSupplyRequestService, ITaskService taskService, ILogService logService)
        {
            _componentsSupplyRequestService = componentsSupplyRequestService;
            _logService = logService;
            _taskService = taskService;
            
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ComponentsSupplyRequestDTO, ComponentsSupplyRequestViewModel>();
                    cfg.CreateMap<ComponentsSupplyRequestViewModel, ComponentsSupplyRequestDTO>();
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
                    cfg.CreateMap<ComponentViewModel, ComponentDTO>();
                    cfg.CreateMap<ComponentDTO, ComponentViewModel>();
                    cfg.CreateMap<StatusSupplyEnum, StatusSupplyEnumDTO>();
                    cfg.CreateMap<StatusSupplyEnumDTO, StatusSupplyEnum>();
                    cfg.CreateMap<UserViewModel, UserDTO>();
                    cfg.CreateMap<UserDTO, UserViewModel>();
                }).CreateMapper();
        }


        public async Task<ViewResult> Index()
        {
            return View(_mapper.Map < IEnumerable<ComponentsSupplyRequestDTO>,
                IEnumerable<ComponentsSupplyRequestViewModel>>(
                await _componentsSupplyRequestService.GetComponentSupplyRequestsAsync()));
        }

        public async Task<ViewResult> Create()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(ComponentsSupplyRequestViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.DateAdded = DateTime.Now;
                // TODO User object
                // viewModel.User.UserName = User.Identity?.Name;
                viewModel.StatusSupply = StatusSupplyEnum.NotAccepted;

                await _componentsSupplyRequestService.CreateComponentSupplyRequestAsync(
                    _mapper.Map<ComponentsSupplyRequestViewModel, ComponentsSupplyRequestDTO>(viewModel));
                return RedirectToAction(nameof(Index));
            }
            
            return View(viewModel);
        }

        public async Task<IActionResult> ChangeStatus(int supplyRequestId, int to, string message)
        {
            LogService.UserName = User.Identity?.Name;
            await _componentsSupplyRequestService.ChangeStatusAsync(supplyRequestId, to, message);
            return RedirectToAction(nameof(Details), new {id = supplyRequestId});
        }

        public async Task<ViewResult> Details(int? id)
        {
            var viewModel = _mapper.Map<ComponentsSupplyRequestDTO, ComponentsSupplyRequestViewModel>(
                await _componentsSupplyRequestService.GetComponentSupplyRequestAsync(id));

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
            return View(_mapper.Map<ComponentsSupplyRequestDTO, ComponentsSupplyRequestViewModel>(await _componentsSupplyRequestService.GetComponentSupplyRequestAsync(id)));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(ComponentsSupplyRequestViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _componentsSupplyRequestService.UpdateComponentSupplyRequestAsync(
                    _mapper.Map<ComponentsSupplyRequestViewModel, ComponentsSupplyRequestDTO>(viewModel));
                return RedirectToAction(nameof(Details), new {id = viewModel.Id});
            }
            
            return View(viewModel);
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