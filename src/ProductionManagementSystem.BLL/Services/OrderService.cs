﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Orders;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IOrderService : IBaseService<Order>
    {
        IEnumerable<Models.Tasks.Task> GetTasksByOrderId(int orderId);
        Task DeleteByIdAsync(int orderId);
    }

    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly ITaskService _taskService; 
        
        public OrderService(IUnitOfWork uow) : base(uow)
        {
            _taskService = new TaskService(uow);
            _currentRepository = _db.OrderRepository;
        }
        
        public override async Task CreateAsync(Order order)
        {
            order.DateStart = DateTime.Now.Date;
            foreach (var task in order.Tasks)
            {
                await _taskService.CreateAsync(task);
            }

            await base.CreateAsync(order);
        }

        public override async Task DeleteAsync(Order order)
        {
            foreach (var task in order.Tasks)
            {
                await _taskService.DeleteAsync(task);
            }
            
            await base.DeleteAsync(order);
        }

        public IEnumerable<Models.Tasks.Task> GetTasksByOrderId(int orderId)
        {
            return _db.TaskRepository.Find(t => t.OrderId == orderId);
        }

        public async Task DeleteByIdAsync(int orderId)
        {
            await this.DeleteAsync(new Order() {Id = orderId});
        }

    }
}