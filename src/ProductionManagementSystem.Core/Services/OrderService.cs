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
        
        public override async System.Threading.Tasks.Task CreateAsync(Order order)
        {
            order.DateStart = DateTime.Now.Date;
            await base.CreateAsync(order);

            if (order.Tasks != null)
            {
                foreach (var task in order.Tasks)
                {
                    task.OrderId = order.Id;
                    await _taskService.CreateAsync(task);
                }
            }
        }

        public override async System.Threading.Tasks.Task DeleteAsync(Order order)
        {
            if (order.Tasks != null)
                foreach (var task in order.Tasks)
                    await _taskService.DeleteAsync(task);
            
            await base.DeleteAsync(order);
        }

        protected override async System.Threading.Tasks.Task CreateLogForCreatingAsync(Order item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был создан заказ " + item, OrderId = item.Id });
        }

        protected override async System.Threading.Tasks.Task CreateLogForUpdatingAsync(Order item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был изменён заказ " + item, OrderId = item.Id });
        }

        protected override async System.Threading.Tasks.Task CreateLogForDeletingAsync(Order item)
        {
            await _db.LogRepository.CreateAsync(new Log { Message = "Был удалён заказ " + item, OrderId = item.Id });
        }

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