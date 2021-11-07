﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    
        public override async Task<IEnumerable<MontageInDevice>> GetAllAsync()
        {
            var montageInDevices = (await base.GetAllAsync()).ToList();
            foreach (var montageInDevice in montageInDevices)
                montageInDevice.Component = await _db.Montages.FindAsync(montageInDevice.ComponentId);
            
            return montageInDevices;
        }

        public override async Task<MontageInDevice> GetByIdAsync(int id)
        {
            var designInDevice = await base.GetByIdAsync(id);
            if (designInDevice == null)
                return null;
            
            designInDevice.Component = await _db.Montages.FindAsync(designInDevice.ComponentId);
            return designInDevice;
        }

        public override async Task<IEnumerable<MontageInDevice>> FindAsync(Func<MontageInDevice, bool> predicate)
        {
            var montageInDevices = (await base.FindAsync(predicate)).ToList();
            foreach (var montageInDevice in montageInDevices)
                montageInDevice.Component = await _db.Montages.FindAsync(montageInDevice.ComponentId);
            
            return montageInDevices;
        }
    }
}