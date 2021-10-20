using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IDesignIndDeviceRepository : IRepository<DesignInDevice>
    {
        
    }
    public class DesignIndDeviceRepository : Repository<DesignInDevice>, IDesignIndDeviceRepository
    {
        public DesignIndDeviceRepository(ApplicationContext db) : base(db)
        {
        }
    }
}