using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.Models;
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
                })
                .CreateMapper();
        }

        public IActionResult Index()
        {
            var ordersViewModel = _mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>(_orderService.GetOrders());
            return View(ordersViewModel);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OrderViewModel orderModel)
        {
            if (ModelState.IsValid)
            {
                var orderDto = _mapper.Map<OrderViewModel, OrderDTO>(orderModel);
                _orderService.CreateOrder(orderDto); 
                return RedirectToAction(nameof(Index));
            }

            return View(orderModel);
        }

        public IActionResult Details(int? id)
        {
            try
            {
                var orderViewModel = _mapper.Map<OrderDTO, OrderViewModel>(_orderService.GetOrder(id));
                return View(orderViewModel);
            }
            catch (PageNotFoundException)
            {
                return NotFound();
            }
        }

        public IActionResult Delete(int? id)
        {
            try
            {
                var orderViewModel = _mapper.Map<OrderDTO, OrderViewModel>(_orderService.GetOrder(id));
                return View(orderViewModel);
            }
            catch (PageNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirm(int? Id)
        {
            try
            {
                _orderService.DeleteOrder(Id);
                return RedirectToAction(nameof(Index));
            }
            catch (PageNotFoundException)
            {
                return NotFound();
            }       
        }
    }
}