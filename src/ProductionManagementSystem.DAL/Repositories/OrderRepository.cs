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
    }
}