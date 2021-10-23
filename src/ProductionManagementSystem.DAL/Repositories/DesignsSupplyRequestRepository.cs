using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.SupplyRequests;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IDesignsSupplyRequestRepository : IRepository<DesignSupplyRequest>
    {
        
    }

    public class DesignsSupplyRequestRepository : Repository<DesignSupplyRequest>, IDesignsSupplyRequestRepository
    {
        public DesignsSupplyRequestRepository(ApplicationContext db) : base(db)
        {
        }
    }
}