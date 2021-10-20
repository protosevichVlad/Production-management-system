using Microsoft.AspNetCore.Components;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.DAL.Repositories
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