using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Devices;

namespace ProductionManagementSystem.Core.Repositories
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

        public override async Task<IEnumerable<DesignInDevice>> GetAllAsync()
        {
            var designsInDevice = (await base.GetAllAsync()).ToList();
            foreach (var designInDevice in designsInDevice)
                designInDevice.Component = await _db.Designs.FindAsync(designInDevice.ComponentId);
            
            return designsInDevice;
        }

        public override async Task<DesignInDevice> GetByIdAsync(int id)
        {
            var designInDevice = await base.GetByIdAsync(id);
            designInDevice.Component = await _db.Designs.FindAsync(designInDevice.ComponentId);
            return designInDevice;
        }

        public override async Task<IEnumerable<DesignInDevice>> FindAsync(Func<DesignInDevice, bool> predicate)
        {
            var designsInDevice = (await base.FindAsync(predicate)).ToList();
            foreach (var designInDevice in designsInDevice)
                designInDevice.Component = await _db.Designs.FindAsync(designInDevice.ComponentId);
            
            return designsInDevice;
        }
    }
}