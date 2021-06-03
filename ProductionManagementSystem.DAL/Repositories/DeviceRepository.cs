using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductionManagementSystem.DAL.EF;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.DAL.Repositories
{
    public class DeviceRepository : IRepository<Device>
    {
        private ApplicationContext _db;

        public DeviceRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            return await _db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                    .ThenInclude(c => c.Component)
                .Include(d => d.DeviceDesignTemplate)
                    .ThenInclude(d => d.Design)
                .ToListAsync();
        }

        public async Task<Device> GetAsync(int id)
        {
            return await _db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                    .ThenInclude(c => c.Component)
                .Include(d => d.DeviceDesignTemplate)
                    .ThenInclude(d => d.Design).FirstOrDefaultAsync(d => d.Id == id);
        }

        public IEnumerable<Device> Find(Func<Device, bool> predicate)
        {
            return _db.Devices
                .Include(d => d.DeviceComponentsTemplate)
                .ThenInclude(c => c.Component)
                .Include(d => d.DeviceDesignTemplate)
                .ThenInclude(d => d.Design).Where(predicate);
        }

        public async Task CreateAsync(Device item)
        {
            await _db.Devices.AddAsync(item);
        }

        public void Update(Device item)
        {
            _db.Devices.Update(item);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.Devices.FindAsync(id);
            if (item != null)
                _db.Devices.Remove(item);
        }

    }
}