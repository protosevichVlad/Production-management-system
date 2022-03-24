using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Devices;

namespace ProductionManagementSystem.Core.Repositories
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
    
        public override async Task<List<MontageInDevice>> GetAllAsync()
        {
            var montageInDevices = (await base.GetAllAsync()).ToList();
            foreach (var montageInDevice in montageInDevices)
                montageInDevice.Montage = await _db.Montages.FindAsync(montageInDevice.ComponentId);
            
            return montageInDevices;
        }

        public override async Task<MontageInDevice> GetByIdAsync(int id)
        {
            var designInDevice = await base.GetByIdAsync(id);
            if (designInDevice == null)
                return null;
            
            designInDevice.Montage = await _db.Montages.FindAsync(designInDevice.ComponentId);
            return designInDevice;
        }

        public override async Task<List<MontageInDevice>> FindAsync(Func<MontageInDevice, bool> predicate, string includeProperty=null)
        {
            var montageInDevices = (await base.FindAsync(predicate)).ToList();
            foreach (var montageInDevice in montageInDevices)
                montageInDevice.Montage = await _db.Montages.FindAsync(montageInDevice.ComponentId);
            
            return montageInDevices;
        }
    }
}