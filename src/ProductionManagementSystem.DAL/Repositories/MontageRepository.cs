using Microsoft.AspNetCore.Components;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IMontageRepository : IComponentBaseRepository<Montage>
    {
        
    }

    public class MontageRepository : ComponentBaseRepository<Montage>, IMontageRepository
    {
        public MontageRepository(ApplicationContext db) : base(db)
        {
        }
    }
}