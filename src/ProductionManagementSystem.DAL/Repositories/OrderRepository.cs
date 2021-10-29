using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Orders;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        
    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationContext db) : base(db)
        {
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            var orders = await base.GetAllAsync();
            foreach (var order in orders)
            {
                order.Tasks = _db.Tasks.Where(t => t.OrderId == order.Id).ToList();
            }
            
            return orders;
        }

        public override async Task<Order> GetByIdAsync(int id)
        {
            var order = await base.GetByIdAsync(id);
            if (order == null)
                return null;
            
            order.Tasks = await _db.Tasks.Where(t => t.OrderId == id).ToListAsync();
            return order;
        }
    }
}