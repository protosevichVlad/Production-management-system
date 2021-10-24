using System;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.Repositories;
using ProductionManagementSystem.Models.Orders;

namespace ProductionManagementSystem.BLL.Services
{
    public interface IOrderService : IBaseService<Order>
    {
        
    }

    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly ITaskService _taskService; 
        
        public OrderService(IUnitOfWork uow) : base(uow)
        {
            _taskService = new TaskService(uow);
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
    }
}