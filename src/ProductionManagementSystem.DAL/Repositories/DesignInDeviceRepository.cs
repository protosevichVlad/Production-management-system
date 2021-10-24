using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.Models.Devices;

namespace ProductionManagementSystem.DAL.Repositories
{
    public interface IDesignInDeviceRepository : IRepository<DesignInDevice>
    {
        IEnumerable<DesignInDevice> GetDesignsByDeviceId(int deviceId);
    }
    public class DesignInDeviceRepository : Repository<DesignInDevice>, IDesignInDeviceRepository
    {
        public DesignInDeviceRepository(ApplicationContext db) : base(db)
        {
        }

        public IEnumerable<DesignInDevice> GetDesignsByDeviceId(int deviceId)
        {
            return _dbSet.Where(d => d.DeviceId == deviceId);
        }
    }
}