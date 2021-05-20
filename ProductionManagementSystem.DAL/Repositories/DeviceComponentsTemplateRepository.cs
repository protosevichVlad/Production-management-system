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
    public class DeviceComponentsTemplateRepository : IRepository<DeviceComponentsTemplate>
    {
        private ApplicationContext _db;

        public DeviceComponentsTemplateRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<DeviceComponentsTemplate>> GetAllAsync()
        {
            return await _db.DeviceComponentsTemplates.ToListAsync();
        }

        public async Task<DeviceComponentsTemplate> GetAsync(int id)
        {
            return await _db.DeviceComponentsTemplates.FindAsync(id);
        }

        public IEnumerable<DeviceComponentsTemplate> Find(Func<DeviceComponentsTemplate, bool> predicate)
        {
            return _db.DeviceComponentsTemplates.Where(predicate).ToList();
        }

        public async Task CreateAsync(DeviceComponentsTemplate item)
        {
            await _db.DeviceComponentsTemplates.AddAsync(item);
        }

        public void Update(DeviceComponentsTemplate item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.DeviceComponentsTemplates.FindAsync(id);
            if (item != null)
                _db.DeviceComponentsTemplates.Remove(item);
        }
    }
}