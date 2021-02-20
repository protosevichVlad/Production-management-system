using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Models;
using ProductionManagementSystem.ViewModels;

namespace ProductionManagementSystem.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationContext _context;
        
        public OrdersController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.Tasks)
                .ToList();
            List<OrderViewModel> orderModels = new List<OrderViewModel>();
            foreach (var order in orders)
            {
                orderModels.Add(new OrderViewModel()
                {
                    Customer = order.Customer,
                    Deadline = order.Deadline,
                    Description = order.Description,
                    Id = order.Id,
                    DateStart = order.DateStart,
                    Status = GetDisplayName(order.Tasks.Min(a => a.Status))
                });
            }
            return View(orderModels);
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
                Order order = new Order
                {
                    Customer = orderModel.Customer,
                    Deadline = orderModel.Deadline,
                    Description = orderModel.Description,
                    DateStart = DateTime.Now.Date,
                    Tasks = new List<Task>()
                };
                
                for (int i = 0; i < orderModel.DeviceIds.Length; i++)
                {
                    for (int j = 0; j < orderModel.DeviceQuantity[i]; j++)
                    {
                        new TasksController(_context)?.Create(new TaskViewModel()
                        {
                            Deadline = orderModel.Deadline,
                            Description = orderModel.DeviceDescriptions[i],
                            DeviceId = orderModel.DeviceIds[i],
                        });
                        var task = _context.Tasks.ToList().LastOrDefault();
                        if (task != null)
                        {
                            order.Tasks.Add(task);
                        }
                    }
                }

                _context.Orders.Add(order);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index), new {id = order.Id});
            }

            return View(orderModel);
        }

        private string GetDisplayName<T>(T item)
        {
            return  item.GetType()
                        .GetMember(item.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName();   
        }
        
    }
}