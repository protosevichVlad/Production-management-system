using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            foreach (var design in device.Designs)
            {
                await _db.DesignInDevices.AddAsync(design);
            }
            
            foreach (var montage in device.Montage)
            {
                await _db.MontageInDevices.AddAsync(montage);
            }
            
            await base.CreateAsync(device);
        }

        public override IEnumerable<Device> GetAll()
        {
            var devices = base.GetAll();
            foreach (var device in devices)
            {
                device.Designs = _db.DesignInDevices.Where(d => d.DeviceId == device.Id);
                device.Montage = _db.MontageInDevices.Where(m => m.DeviceId == device.Id);
            }

            return devices;
        }

        public override async Task<Device> GetByIdAsync(int id)
        {
            var device = await base.GetByIdAsync(id);
            device.Designs = _db.DesignInDevices.Where(d => d.DeviceId == device.Id);
            device.Montage = _db.MontageInDevices.Where(m => m.DeviceId == device.Id);
            return device;
        }

        public override void Update(Device device)
        {
            foreach (var design in device.Designs)
            {
                _db.DesignInDevices.Update(design);
            }
            
            foreach (var montage in device.Montage)
            {
                _db.MontageInDevices.Update(montage);
            }
            
            base.Update(device);
        }

        public override void Delete(Device device)
        {
            _db.DesignInDevices.RemoveRange(device.Designs);
            _db.MontageInDevices.RemoveRange(device.Montage);
            
            base.Delete(device);
        }

        public override IEnumerable<Device> Find(Func<Device, bool> predicate)
        {
            var devices = base.Find(predicate);
            foreach (var device in devices)
            {
                device.Designs = _db.DesignInDevices.Where(d => d.DeviceId == device.Id);
                device.Montage = _db.MontageInDevices.Where(m => m.DeviceId == device.Id);
            }

            return devices;
        }
    }
}