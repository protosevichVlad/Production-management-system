using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IDeviceRepository : IRepository<Device>
    {
        
    }
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(ApplicationContext db) : base(db)
        {
        }
    }
}