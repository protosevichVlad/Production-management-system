﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Infrastructure;
using ProductionManagementSystem.Core.Models.Orders;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        
    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationContext db) : base(db)
        {
        }

        public override async Task<List<Order>> GetAllAsync()
        {
            var orders = (await base.GetAllAsync()).ToList();
            foreach (var order in orders)
                await InitOrderAsync(order);
            
            return orders;
        }

        public override async Task<Order> GetByIdAsync(int id)
        {
            var order = await base.GetByIdAsync(id);
            if (order == null)
                return null;
            
            await InitOrderAsync(order);
            return order;
        }

        public override async Task<List<Order>> FindAsync(Func<Order, bool> predicate, string includeProperty=null)
        {
            var orders = (await base.FindAsync(predicate)).ToList();
            foreach (var order in orders)
                await InitOrderAsync(order);
            
            return orders;
        }

        private async Task InitOrderAsync(Order order)
        {
            order.Tasks = await _db.Tasks.Where(t => t.OrderId == order.Id).ToListAsync();
            if (order.Tasks.Any())
                order.Status = order.Tasks.Min(t => t.Status).GetTasksStatusName();
        }
    }
}