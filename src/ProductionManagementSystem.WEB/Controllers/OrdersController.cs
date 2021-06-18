using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Enums;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    [Authorize(Roles = RoleEnum.Admin)]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ITaskService _taskService;
        private IMapper _mapper;
        
        
        public OrdersController(IOrderService orderService, ITaskService taskService)
        {
            _orderService = orderService;
            _taskService = taskService;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<OrderDTO, OrderViewModel>();
                    cfg.CreateMap<TaskDTO, TaskViewModel>()
                        .ForMember(
                        task => task.Status, 
                        opt => opt.MapFrom(
                            src => _taskService.GetTaskStatusName(src.Status)
                        )
                    );
                    cfg.CreateMap<DeviceDTO, DeviceViewModel>();
                    cfg.CreateMap<OrderViewModel, OrderDTO>();
                    cfg.CreateMap<TaskViewModel, TaskDTO>();
                    cfg.CreateMap<DeviceViewModel, DeviceDTO>();
                    cfg.CreateMap<ObtainedDesign, ObtainedDesignDTO>();
                    cfg.CreateMap<ObtainedDesignDTO, ObtainedDesign>();
                    cfg.CreateMap<ObtainedComponent, ObtainedComponentDTO>();
                    cfg.CreateMap<ObtainedComponentDTO, ObtainedComponent>();
                })
                .CreateMapper();
        }

        public async Task<IActionResult> Index()
        {
            var ordersViewModel = _mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>(await _orderService.GetOrdersAsync());
            return View(ordersViewModel);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel orderModel)
        {
            if (ModelState.IsValid)
            {
                var orderDto = _mapper.Map<OrderViewModel, OrderDTO>(orderModel);
                await _orderService.CreateOrderAsync(orderDto); 
                return RedirectToAction(nameof(Index));
            }

            return View(orderModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var orderViewModel = _mapper.Map<OrderDTO, OrderViewModel>(await _orderService.GetOrderAsync(id));
                return View(orderViewModel);
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var orderViewModel = _mapper.Map<OrderDTO, OrderViewModel>(await _orderService.GetOrderAsync(id));
                return View(orderViewModel);
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int? Id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(Id);
                return RedirectToAction(nameof(Index));
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }       
        }
    }
}