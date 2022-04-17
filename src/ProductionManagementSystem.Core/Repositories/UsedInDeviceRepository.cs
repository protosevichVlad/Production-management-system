using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IUsedInDeviceRepository : IRepository<UsedInDevice>
    {
        
    }
    public class UsedInDeviceRepository : Repository<UsedInDevice>, IUsedInDeviceRepository
    {
        public UsedInDeviceRepository(ApplicationContext db) : base(db)
        {
        }
    }
}