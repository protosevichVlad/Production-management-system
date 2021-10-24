using System.Collections.Generic;
using System.Linq;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IMontageInDeviceRepository : IRepository<MontageInDevice>
    {
        public IEnumerable<MontageInDevice> GetMontageByDeviceId(int deviceId);
    }

    public class MontageInDeviceRepository : Repository<MontageInDevice>, IMontageInDeviceRepository
    {
        public MontageInDeviceRepository(ApplicationContext db) : base(db)
        {
        }

        public IEnumerable<MontageInDevice> GetMontageByDeviceId(int deviceId)
        {
            return _dbSet.Where(m => m.DeviceId == deviceId);
        }
    }
}