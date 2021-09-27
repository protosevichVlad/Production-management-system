using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.WEB.Models;

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
        public async Task<ViewResult> Create(ComponentsSupplyRequestViewModel viewModel)
        {
            return View(viewModel);
        }
    }
}