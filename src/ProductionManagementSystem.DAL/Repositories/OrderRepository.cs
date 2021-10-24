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

        public override async Task<Order> GetByIdAsync(int id)
        {
            var order = await base.GetByIdAsync(id);
            order.Tasks = await _db.Tasks.Where(t => t.OrderId == id).ToListAsync();

            return order;
        }
    }
}