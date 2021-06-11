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
    public class DeviceDesignTemplateRepository : IRepository<DeviceDesignTemplate>
    {
        private ApplicationContext _db;

        public DeviceDesignTemplateRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<DeviceDesignTemplate>> GetAllAsync()
        {
            return await _db.DeviceDesignTemplates.ToListAsync();
        }

        public async Task<DeviceDesignTemplate> GetAsync(int id)
        {
            return await _db.DeviceDesignTemplates.FindAsync(id);
        }

        public IEnumerable<DeviceDesignTemplate> Find(Func<DeviceDesignTemplate, bool> predicate)
        {
            return _db.DeviceDesignTemplates.Where(predicate).ToList();
        }

        public async Task CreateAsync(DeviceDesignTemplate item)
        {
            await _db.DeviceDesignTemplates.AddAsync(item);
        }

        public void Update(DeviceDesignTemplate item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.DeviceDesignTemplates.FindAsync(id);
            if (item != null)
                _db.DeviceDesignTemplates.Remove(item);
        }
    }
}