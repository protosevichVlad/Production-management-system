﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models.Devices;

namespace ProductionManagementSystem.Core.Repositories
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
            await base.CreateAsync(device);
            await _db.SaveChangesAsync();

            if (device.Designs != null)
                await _db.DesignInDevices.AddRangeAsync(device.Designs.Select(d =>
                {
                    d.DeviceId = device.Id;
                    return d;
                }));
            if (device.Montages != null)
                await _db.MontageInDevices.AddRangeAsync(device.Montages.Select(m =>
                {
                    m.DeviceId = device.Id;
                    return m;
                }));
        }

        public override async Task<List<Device>> GetAllAsync()
        {
            List<Device> devices = (await base.GetAllAsync()).ToList();
            foreach (var device in devices)
                await InitDeviceAsync(device);
            
            return devices;
        }

        public override async Task<Device> GetByIdAsync(int id)
        {
            var device = await base.GetByIdAsync(id);
            if (device == null)
                return null;

            await InitDeviceAsync(device);
            return device;
        }

        public override async Task UpdateAsync(Device device)
        {
            _db.MontageInDevices.RemoveRange(_db.MontageInDevices.Where(d => d.DeviceId == device.Id));
            _db.DesignInDevices.RemoveRange(_db.DesignInDevices.Where(d => d.DeviceId == device.Id));

            if (device.Designs != null)
                await _db.DesignInDevices.AddRangeAsync(device.Designs.Select(d =>
                {
                    d.DeviceId = device.Id;
                    return d;
                }));
            if (device.Montages != null)
                await _db.MontageInDevices.AddRangeAsync(device.Montages.Select(m =>
                {
                    m.DeviceId = device.Id;
                    return m;
                }));
            
            await base.UpdateAsync(device);
        }

        public override void Delete(Device device)
        {
            _db.MontageInDevices.RemoveRange(_db.MontageInDevices.Where(d => d.DeviceId == device.Id));
            _db.DesignInDevices.RemoveRange(_db.DesignInDevices.Where(d => d.DeviceId == device.Id));
            base.Delete(device);
        }

        public override async Task<List<Device>> FindAsync(Func<Device, bool> predicate, string includeProperty=null)
        {
            List<Device> devices = (await base.FindAsync(predicate)).ToList();
            foreach (var device in devices)
                await InitDeviceAsync(device);

            return devices;
        }

        private async Task InitDeviceAsync(Device device)
        {
            device.Designs = await _db.DesignInDevices.Where(d => d.DeviceId == device.Id).ToListAsync();
            device.Montages = await _db.MontageInDevices.Where(m => m.DeviceId == device.Id).ToListAsync();
            foreach (var montage in device.Montages)
                montage.Montage = await _db.Montages.FindAsync(montage.ComponentId);
            foreach (var design in device.Designs)
                design.Design = await _db.Designs.FindAsync(design.ComponentId);
        }
    }
}