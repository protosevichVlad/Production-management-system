﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Logs;
using ProductionManagementSystem.Models.Orders;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IOrderService : IBaseService<Order>
    {
        Task<IEnumerable<Models.Tasks.Task>> GetTasksByOrderIdAsync(int orderId);
        Task DeleteByIdAsync(int orderId);
    }

    public class OrderService : BaseServiceWithLogs<Order>, IOrderService
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
            await base.CreateAsync(order);

            foreach (var task in order.Tasks)
            {
                task.OrderId = order.Id;
                await _taskService.CreateAsync(task);
            }
        }

        public override async Task DeleteAsync(Order order)
        {
            foreach (var task in order.Tasks)
            {
                await _taskService.DeleteAsync(task);
            }
            
            await base.DeleteAsync(order);
        }

        protected override async Task CreateLogForCreatingAsync(Order item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Был создан монтаж " + item.ToString(), OrderId = item.Id });
        }

        protected override async Task CreateLogForUpdatingAsync(Order item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Был изменён монтаж " + item.ToString(), OrderId = item.Id });
        }

        protected override async Task CreateLogForDeletingAsync(Order item)
        {
            await _db.LogRepository.CreateAsync(new Log()
                { Message = "Был удалён монтаж " + item.ToString(), OrderId = item.Id });
        }

        public async Task<IEnumerable<Models.Tasks.Task>> GetTasksByOrderIdAsync(int orderId)
        {
            return await _db.TaskRepository.FindAsync(t => t.OrderId == orderId);
        }

        public async Task DeleteByIdAsync(int orderId)
        {
            await this.DeleteAsync(new Order() {Id = orderId});
        }

    }
}