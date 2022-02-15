using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Infrastructure;
using ProductionManagementSystem.Core.Models.Orders;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.WEB.Models.Order;
using Task = ProductionManagementSystem.Core.Models.Tasks.Task;

namespace ProductionManagementSystem.WEB.Controllers
{
    [Authorize(Roles = RoleEnum.Admin)]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IDeviceService _deviceService;
        private readonly ITaskService _taskService;
        
        public OrdersController(IOrderService orderService, IDeviceService deviceService, ITaskService taskService)
        {
            _orderService = orderService;
            _deviceService = deviceService;
            _taskService = taskService;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
            ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
            
            var ordersViewModel = await _orderService.GetAllAsync();
            ordersViewModel = SortingOrders(ordersViewModel, sortOrder).ToList();
            return View(ordersViewModel);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order orderModel, int[] DeviceQuantity)
        {
            if (ModelState.IsValid)
            {
                var tasks = new List<Task>();
                for (int i = 0; i < DeviceQuantity.Length; i++)
                {
                    for (int j = 0; j < DeviceQuantity[i]; j++)
                    {
                        tasks.Add(new Task()
                        {
                            Deadline = orderModel.Deadline,
                            DeviceId = orderModel.Tasks[i].DeviceId,
                            Description = orderModel.Tasks[i].Description
                        });
                    }
                }

                orderModel.Tasks = tasks;
                await _orderService.CreateAsync(orderModel);
                return RedirectToAction(nameof(Index));
            }

            return View(orderModel);
        }

        public async Task<IActionResult> Details(int id, string sortOrder)
        {
            try
            {
                ViewData["NumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "num_desc" : "";
                ViewData["DeviceSortParm"] = sortOrder == "Device" ? "device_desc" : "Device";
                ViewData["StartDateSortParm"] = sortOrder == "StartDate" ? "startdate_desc" : "StartDate";
                ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";
                ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";

                var orderViewModel = await _orderService.GetByIdAsync(id);
                var tasks = (await _orderService.GetTasksByOrderIdAsync(orderViewModel.Id)).ToList().Select(async t =>
                {
                    t.Device = await _deviceService.GetByIdAsync(t.DeviceId);
                    return t;
                }).Select(t => t.Result).Where(t => t != null).ToList();
                orderViewModel.Tasks = SortingTasks(tasks, sortOrder).ToList();
                return View(orderViewModel);
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var orderViewModel = await _orderService.GetByIdAsync(id);
                return View(orderViewModel);
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                await _orderService.DeleteByIdAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }       
        }
        
        private static IEnumerable<Task> SortingTasks(IEnumerable<Task> items, string sortOrder)
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
        
        public async Task<IActionResult> GetOrderItem(int index)
        {
            return PartialView("Partail/Order/OrderItem", new OrderItem()
            {
                Index = index,
                AllDevices = await _deviceService.GetAllAsync()
            });
        }

        private static IEnumerable<Order> SortingOrders(IEnumerable<Order> items, string sortOrder)
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