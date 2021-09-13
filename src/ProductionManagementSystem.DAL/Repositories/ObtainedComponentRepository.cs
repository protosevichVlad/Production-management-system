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
    public class ObtainedComponentRepository : IRepository<ObtainedComponent>
    {
        private readonly ApplicationContext _db;

        public ObtainedComponentRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<ObtainedComponent>> GetAllAsync()
        {
            return await _db.ObtainedComponents
                .Include(c => c.Task)
                .Include(c => c.Component)
                .ToListAsync();
        }

        public async Task<ObtainedComponent> GetAsync(int id)
        {
            return await _db.ObtainedComponents
                .Include(c => c.Task)
                .Include(c => c.Component)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IEnumerable<ObtainedComponent> Find(Func<ObtainedComponent, bool> predicate)
        {
            return _db.ObtainedComponents
                .AsNoTracking()
                .Include(c => c.Task)
                .Include(c => c.Component)
                .Where(predicate).ToList();
        }

        public async Task CreateAsync(ObtainedComponent item)
        {
            await _db.ObtainedComponents.AddAsync(item);
        }

        public void Update(ObtainedComponent item)
        {
            var obtComp = _db.ObtainedComponents.FirstOrDefault(obtainedComponent => obtainedComponent.Id == item.Id);
            if (obtComp != null)
            {
                obtComp.Obtained = item.Obtained;
                _db.SaveChanges(); 
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.ObtainedComponents.FindAsync(id);
            if (item != null)
                _db.ObtainedComponents.Remove(item);
        }
    }
}