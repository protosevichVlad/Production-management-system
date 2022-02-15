using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Tasks;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IObtainedMontageRepository : IRepository<ObtainedMontage>
    {
        
    }

    public class ObtainedMontageRepository : Repository<ObtainedMontage>, IObtainedMontageRepository
    {
        public ObtainedMontageRepository(ApplicationContext db) : base(db)
        {
        }
        
        
        public override async Task<List<ObtainedMontage>> GetAllAsync()
        {
            var obtainedMontages = (await base.GetAllAsync()).ToList();
            foreach (var obtainedMontage in obtainedMontages)
                await InitObtainedMontageAsync(obtainedMontage);

            return obtainedMontages;
        }

        public override async Task<ObtainedMontage> GetByIdAsync(int id)
        {
            var obtainedMontage = await base.GetByIdAsync(id);
            if (obtainedMontage == null) 
                return null;
            
            await InitObtainedMontageAsync(obtainedMontage);
            return obtainedMontage;
        }

        public override async Task<List<ObtainedMontage>> FindAsync(Func<ObtainedMontage, bool> predicate)
        {
            var obtainedMontages = (await base.FindAsync(predicate)).ToList();
            foreach (var obtainedMontage in obtainedMontages)
                await InitObtainedMontageAsync(obtainedMontage);

            return obtainedMontages;
        }

        private async System.Threading.Tasks.Task InitObtainedMontageAsync(ObtainedMontage obtainedMontage)
        {
            obtainedMontage.Montage = await _db.Montages.FindAsync(obtainedMontage.ComponentId);
        }
    }
}