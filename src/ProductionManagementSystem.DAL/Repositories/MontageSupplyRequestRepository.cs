using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        public override async Task<IEnumerable<MontageSupplyRequest>> GetAllAsync()
        {
            List<MontageSupplyRequest> montageSupplyRequests = (await base.GetAllAsync()).ToList();
            foreach (var montageSupplyRequest in montageSupplyRequests)
                await InitMontageSupplyRequestAsync(montageSupplyRequest);

            return montageSupplyRequests;
        }

        public override async Task<MontageSupplyRequest> GetByIdAsync(int id)
        {
            MontageSupplyRequest montageSupplyRequest = await base.GetByIdAsync(id);
            if (montageSupplyRequest == null)
                return null;
            
            await InitMontageSupplyRequestAsync(montageSupplyRequest);
            return montageSupplyRequest;
        }

        public override async Task<IEnumerable<MontageSupplyRequest>> FindAsync(Func<MontageSupplyRequest, bool> predicate)
        {
            List<MontageSupplyRequest> montageSupplyRequests = (await base.FindAsync(predicate)).ToList();
            foreach (var montageSupplyRequest in montageSupplyRequests)
                await InitMontageSupplyRequestAsync(montageSupplyRequest);

            return montageSupplyRequests;
        }

        private async Task InitMontageSupplyRequestAsync(MontageSupplyRequest montageSupplyRequest)
        {
            montageSupplyRequest.Montage = await _db.Montages.FindAsync(montageSupplyRequest.ComponentId);
            montageSupplyRequest.User = await _db.Users.FindAsync(montageSupplyRequest.UserId);
        }
    }
}