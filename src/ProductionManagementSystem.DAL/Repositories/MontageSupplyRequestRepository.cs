using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.SupplyRequests;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IMontageSupplyRequestRepository : IRepository<MontageSupplyRequest>
    {
        
    }
    public class MontageSupplyRequestRepository : Repository<MontageSupplyRequest>, IMontageSupplyRequestRepository
    {
        public MontageSupplyRequestRepository(ApplicationContext db) : base(db)
        {
        }
    }
}