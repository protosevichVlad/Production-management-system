using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.WEB.Models;
using ProductionManagementSystem.WEB.ViewModels.ComponentsSupplyRequestViewModels;

namespace ProductionManagementSystem.WEB.Controllers
{
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
                    cfg.CreateMap<ComponentsSupplyRequestDTO, ComponentsSupplyRequest>();
                    cfg.CreateMap<ComponentsSupplyRequest, ComponentsSupplyRequestDTO>();
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
            var viewModel = new IndexPageComponentsSupplyRequestViewModel()
            {
                ComponentsSupplyRequests = _mapper.Map < IEnumerable<ComponentsSupplyRequestDTO>,
                IEnumerable<ComponentsSupplyRequest>>(
                    await _componentsSupplyRequestService.GetComponentSupplyRequestsAsync())
            };
            
            return View(viewModel);
        }
    }
}