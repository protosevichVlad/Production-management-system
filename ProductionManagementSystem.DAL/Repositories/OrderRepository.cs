using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private ApplicationContext _db;

        public OrderRepository(ApplicationContext context)
        {
            _db = context;
        }

        public IEnumerable<Order> GetAll()
        {
            return _db.Orders
                .Include(o => o.Tasks)
                .ThenInclude(t => t.Device);
        }

        public Order Get(int id)
        {
            return _db.Orders
                .Include(o => o.Tasks)
                .ThenInclude(t => t.Device)
                .FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Order> Find(Func<Order, bool> predicate)
        {
            return _db.Orders
                .Include(o => o.Tasks)
                .ThenInclude(t => t.Device)
                .Where(predicate).ToList();
        }

        public void Create(Order item)
        {
            _db.Orders.Add(item);
        }

        public void Update(Order item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = _db.Orders.Find(id);
            if (item != null)
                _db.Orders.Remove(item);
        }
    }
}