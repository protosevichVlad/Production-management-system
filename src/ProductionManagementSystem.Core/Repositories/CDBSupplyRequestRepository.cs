using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface ICDBSupplyRequestRepository : IRepository<CDBSupplyRequest>
    {
        
    }

    public class CDBSupplyRequestRepository : Repository<CDBSupplyRequest>, ICDBSupplyRequestRepository
    {
        private readonly EntityExtRepository _entityExtRepository;

        public CDBSupplyRequestRepository(ApplicationContext db) : base(db)
        {
            _entityExtRepository = new EntityExtRepository(db);
        }

        public override async Task<List<CDBSupplyRequest>> GetAllAsync()
        {
            var requests = await base.GetAllAsync();
            for (int i = 0; i < requests.Count; i++)
            {
                requests[i].Entity = await _entityExtRepository.GetByIdAsync(requests[i].ItemId);
            }
            
            return requests;
        }

        public override async Task<CDBSupplyRequest> GetByIdAsync(int id)
        {
            var request = await base.GetByIdAsync(id);
            request.Entity = await _entityExtRepository.GetByIdAsync(request.ItemId);
            return request;
        }
    }
}