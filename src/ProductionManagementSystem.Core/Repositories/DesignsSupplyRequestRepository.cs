using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.SupplyRequests;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IDesignsSupplyRequestRepository : IRepository<DesignSupplyRequest>
    {
        
    }

    public class DesignsSupplyRequestRepository : Repository<DesignSupplyRequest>, IDesignsSupplyRequestRepository
    {
        public DesignsSupplyRequestRepository(ApplicationContext db) : base(db)
        {
        }

        public override async Task<List<DesignSupplyRequest>> GetAllAsync()
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

        public override async Task<List<DesignSupplyRequest>> FindAsync(Func<DesignSupplyRequest, bool> predicate, string includeProperty=null)
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