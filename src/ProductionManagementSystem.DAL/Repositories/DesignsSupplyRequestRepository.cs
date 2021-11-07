using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public override async Task<IEnumerable<DesignSupplyRequest>> GetAllAsync()
        {
            List<DesignSupplyRequest> designSupplyRequests = (await base.GetAllAsync()).ToList();
            foreach (var designSupplyRequest in designSupplyRequests)
                await InitDesignSupplyRequestAsync(designSupplyRequest);

            return designSupplyRequests;
        }

        public override async Task<DesignSupplyRequest> GetByIdAsync(int id)
        {
            DesignSupplyRequest designSupplyRequest = await base.GetByIdAsync(id);
            await InitDesignSupplyRequestAsync(designSupplyRequest);
            return designSupplyRequest;
        }

        public override async Task<IEnumerable<DesignSupplyRequest>> FindAsync(Func<DesignSupplyRequest, bool> predicate)
        {
            List<DesignSupplyRequest> designSupplyRequests = (await base.FindAsync(predicate)).ToList();
            foreach (var designSupplyRequest in designSupplyRequests)
                await InitDesignSupplyRequestAsync(designSupplyRequest);

            return designSupplyRequests;
        }

        private async Task InitDesignSupplyRequestAsync(DesignSupplyRequest designSupplyRequest)
        {
            designSupplyRequest.Design = await _db.Designs.FindAsync(designSupplyRequest.ComponentId);
            designSupplyRequest.User = await _db.Users.FindAsync(designSupplyRequest.UserId);
        }
    }
}