using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public override async Task CreateAsync(Device device)
        {
            if (device.Designs != null)
                await _db.DesignInDevices.AddRangeAsync(device.Designs);
            if (device.Montage != null)
                await _db.MontageInDevices.AddRangeAsync(device.Montage);
            
            await base.CreateAsync(device);
        }

        public override async Task<IEnumerable<Device>> GetAllAsync()
        {
            var devices = await base.GetAllAsync();
            if (devices == null)
                return null;
            foreach (var device in devices)
            {
                device.Designs = _db.DesignInDevices.Where(d => d.DeviceId == device.Id).ToList();
                device.Montage = _db.MontageInDevices.Where(m => m.DeviceId == device.Id).ToList();
            }

            return devices;
        }

        public override async Task<Device> GetByIdAsync(int id)
        {
            var device = await base.GetByIdAsync(id);
            if (device == null)
                return null;
            
            device.Designs = await _db.DesignInDevices.Where(d => d.DeviceId == device.Id).ToListAsync();
            device.Montage = await _db.MontageInDevices.Where(m => m.DeviceId == device.Id).ToListAsync();
            return device;
        }

        public override void Update(Device device)
        {
            _db.MontageInDevices.RemoveRange(_db.MontageInDevices.Where(d => d.DeviceId == device.Id));
            _db.DesignInDevices.RemoveRange(_db.DesignInDevices.Where(d => d.DeviceId == device.Id));

            if (device.Designs != null)
            {
                _db.DesignInDevices.AddRange(device.Designs);
            }

            if (device.Montage != null)
            {
                _db.MontageInDevices.AddRange(device.Montage);
            }
            
            base.Update(device);
        }

        public override void Delete(Device device)
        {
            _db.MontageInDevices.RemoveRange(_db.MontageInDevices.Where(d => d.DeviceId == device.Id));
            _db.DesignInDevices.RemoveRange(_db.DesignInDevices.Where(d => d.DeviceId == device.Id));
            
            base.Delete(device);
        }

        public override async Task<IEnumerable<Device>> FindAsync(Func<Device, bool> predicate)
        {
            var devices = await base.FindAsync(predicate);
            if (devices == null)
                return null;
            
            foreach (var device in devices)
            {
                device.Designs = _db.DesignInDevices.Where(d => d.DeviceId == device.Id).ToList();
                device.Montage = _db.MontageInDevices.Where(m => m.DeviceId == device.Id).ToList();
            }

            return devices;
        }
    }
}