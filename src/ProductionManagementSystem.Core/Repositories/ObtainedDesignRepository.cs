using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IObtainedDesignRepository : IRepository<ObtainedDesign>
    {
        
    }
    public class ObtainedDesignRepository : Repository<ObtainedDesign>, IObtainedDesignRepository
    {
        public ObtainedDesignRepository(ApplicationContext db) : base(db)
        {
        }

        public override async Task<IEnumerable<ObtainedDesign>> GetAllAsync()
        {
            var obtainedDesigns = (await base.GetAllAsync()).ToList();
            foreach (var obtainedDesign in obtainedDesigns)
                await InitObtainedDesignAsync(obtainedDesign);

            return obtainedDesigns;
        }

        public override async Task<ObtainedDesign> GetByIdAsync(int id)
        {
            var obtainedDesign = await base.GetByIdAsync(id);
            if (obtainedDesign == null)
                return null;
            
            await InitObtainedDesignAsync(obtainedDesign);
            return obtainedDesign;
        }

        public override async Task<IEnumerable<ObtainedDesign>> FindAsync(Func<ObtainedDesign, bool> predicate)
        {
            var obtainedDesigns = (await base.FindAsync(predicate)).ToList();
            foreach (var obtainedDesign in obtainedDesigns)
                await InitObtainedDesignAsync(obtainedDesign);

            return obtainedDesigns;
        }

        private async Task InitObtainedDesignAsync(ObtainedDesign obtainedDesign)
        {
            obtainedDesign.Design = await _db.Designs.FindAsync(obtainedDesign.ComponentId);
        }
    }
}