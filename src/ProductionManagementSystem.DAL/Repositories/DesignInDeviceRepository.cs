using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IDesignInDeviceRepository : IRepository<DesignInDevice>
    {
        
    }
    public class DesignInDeviceRepository : Repository<DesignInDevice>, IDesignInDeviceRepository
    {
        public DesignInDeviceRepository(ApplicationContext db) : base(db)
        {
        }
    }
}