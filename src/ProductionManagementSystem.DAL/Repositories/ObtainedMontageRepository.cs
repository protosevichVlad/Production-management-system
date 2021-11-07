using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Tasks;
using Task = ProductionManagementSystem.Models.Tasks.Task;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IObtainedMontageRepository : IRepository<ObtainedMontage>
    {
        
    }

    public class ObtainedMontageRepository : Repository<ObtainedMontage>, IObtainedMontageRepository
    {
        public ObtainedMontageRepository(ApplicationContext db) : base(db)
        {
        }
        
        
        public override async Task<IEnumerable<ObtainedMontage>> GetAllAsync()
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

        public override async Task<IEnumerable<ObtainedMontage>> FindAsync(Func<ObtainedMontage, bool> predicate)
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