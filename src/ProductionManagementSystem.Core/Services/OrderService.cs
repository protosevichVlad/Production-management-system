using System;
using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.Orders;
using ProductionManagementSystem.Core.Models.Tasks;
using ProductionManagementSystem.Core.Repositories;

namespace ProductionManagementSystem.Core.Services
{
    public interface IOrderService : IBaseService<Order>
    {
        System.Threading.Tasks.Task<IEnumerable<Task>> GetTasksByOrderIdAsync(int orderId);
        System.Threading.Tasks.Task DeleteByIdAsync(int orderId);
    }

    public class OrderService : BaseServiceWithLogs<Order>, IOrderService
    {
        private readonly ITaskService _taskService; 
        
        public OrderService(IUnitOfWork uow) : base(uow)
        {
            _taskService = new TaskService(uow);
            _currentRepository = _db.OrderRepository;
        }

        protected override LogsItemType ItemType => LogsItemType.Order;

        public override async System.Threading.Tasks.Task CreateAsync(Order order)
        {
            order.DateStart = DateTime.Now.Date;
            foreach (var task in order.Tasks)
                await _taskService.CreateAsync(task);
            
            await base.CreateAsync(order);
        }

        public override async System.Threading.Tasks.Task DeleteAsync(Order order)
        {
            if (order.Tasks != null)
                foreach (var task in order.Tasks)
                    await _taskService.DeleteAsync(task);
            
            await base.DeleteAsync(order);
        }

        protected override int GetEntityId(Order model) => model.Id;

        public async System.Threading.Tasks.Task<IEnumerable<Task>> GetTasksByOrderIdAsync(int orderId)
        {
            return await _db.TaskRepository.FindAsync(t => t.OrderId == orderId);
        }

        public async System.Threading.Tasks.Task DeleteByIdAsync(int orderId)
        {
            await DeleteAsync(new Order {Id = orderId});
        }
    }
}