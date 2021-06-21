using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IMapper _mapper;
        
        
        public OrdersController(IOrderService orderService, ITaskService taskService)
        {
            _orderService = orderService;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<OrderDTO, OrderViewModel>();
                    cfg.CreateMap<TaskDTO, TaskViewModel>()
                        .ForMember(
                        task => task.Status, 
                        opt => opt.MapFrom(
                            src => taskService.GetTaskStatusName(src.Status)
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

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
            
            var ordersViewModel = _mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>(await _orderService.GetOrdersAsync());
            ordersViewModel = SortingOrders(ordersViewModel, sortOrder);
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

        public async Task<IActionResult> Details(int? id, string sortOrder)
        {
            try
            {
                ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
                ViewData["DeviceSortParm"] = sortOrder == "Device" ? "device_desc" : "Device";
                ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
                ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
                ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
                
                var orderViewModel = _mapper.Map<OrderDTO, OrderViewModel>(await _orderService.GetOrderAsync(id));

                orderViewModel.Tasks = SortingTasks(orderViewModel.Tasks, sortOrder).ToList();
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
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }       
        }
        
        private static IEnumerable<TaskViewModel> SortingTasks(IEnumerable<TaskViewModel> items, string sortOrder)
        {
            switch (sortOrder)
            {
                case "num_desc":
                    items = items.OrderByDescending(t => t.Id);
                    break;
                case "Device":
                    items = items.OrderBy(t => t.Device.Name);
                    break;
                case "device_desc":
                    items = items.OrderByDescending(t => t.Device.Name);
                    break;
                case "StartDate":
                    items = items.OrderBy(t => t.StartTime);
                    break;
                case "startdate_desc":
                    items = items.OrderByDescending(t => t.StartTime);
                    break;
                case "Status":
                    items = items.OrderBy(t => t.Status);
                    break;
                case "status_desc":
                    items = items.OrderByDescending(t => t.Status);
                    break;
                case "OrderId":
                    items = items.OrderBy(t => t.OrderId);
                    break;
                case "orderid_desc":
                    items = items.OrderByDescending(t => t.OrderId);
                    break;
                case "Deadline":
                    items = items.OrderBy(t => t.Deadline);
                    break;
                case "deadline_desc":
                    items = items.OrderByDescending(t => t.Deadline);
                    break;
                default:
                    items = items.OrderBy(t => t.Id);
                    break;
            }

            return items;
        }
        
        private static IEnumerable<OrderViewModel> SortingOrders(IEnumerable<OrderViewModel> items, string sortOrder)
        {
            switch (sortOrder)
            {
                case "num_desc":
                    items = items.OrderByDescending(t => t.Id);
                    break;
                case "StartDate":
                    items = items.OrderBy(t => t.DateStart);
                    break;
                case "startdate_desc":
                    items = items.OrderByDescending(t => t.DateStart);
                    break;
                case "Name":
                    items = items.OrderBy(t => t.Customer);
                    break;
                case "name_desc":
                    items = items.OrderByDescending(t => t.Customer);
                    break;
                case "Status":
                    items = items.OrderBy(t => t.Status);
                    break;
                case "status_desc":
                    items = items.OrderByDescending(t => t.Status);
                    break;
                case "Deadline":
                    items = items.OrderBy(t => t.Deadline);
                    break;
                case "deadline_desc":
                    items = items.OrderByDescending(t => t.Deadline);
                    break;
                default:
                    items = items.OrderBy(t => t.Id);
                    break;
            }

            return items;
        }
    }
}