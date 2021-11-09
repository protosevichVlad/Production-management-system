using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IMontageRepository : IRepository<Montage>
    {
        
    }

    public class MontageRepository : Repository<Montage>, IMontageRepository
    {
        public MontageRepository(ApplicationContext db) : base(db)
        {
        }
    }
}