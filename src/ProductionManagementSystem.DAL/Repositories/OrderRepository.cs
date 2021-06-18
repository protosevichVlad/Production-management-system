using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly ApplicationContext _db;

        public OrderRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _db.Orders
                .Include(o => o.Tasks)
                .ThenInclude(t => t.Device)
                .ToListAsync();
        }

        public async Task<Order> GetAsync(int id)
        {
            return await _db.Orders
                .Include(o => o.Tasks)
                .ThenInclude(t => t.Device)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public IEnumerable<Order> Find(Func<Order, bool> predicate)
        {
            return _db.Orders
                .Include(o => o.Tasks)
                .ThenInclude(t => t.Device)
                .Where(predicate).ToList();
        }

        public async Task CreateAsync(Order item)
        {
            await _db.Orders.AddAsync(item);
        }

        public void Update(Order item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.Orders.FindAsync(id);
            if (item != null)
                _db.Orders.Remove(item);
        }
    }
}