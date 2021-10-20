using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IMontageInDeviceRepository : IRepository<MontageInDevice>
    {
        
    }

    public class MontageInDeviceRepository : Repository<MontageInDevice>, IMontageInDeviceRepository
    {
        public MontageInDeviceRepository(ApplicationContext db) : base(db)
        {
        }
    }
}