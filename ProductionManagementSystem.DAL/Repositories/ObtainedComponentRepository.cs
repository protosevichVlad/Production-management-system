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
    public class ObtainedСomponentRepository : IRepository<ObtainedComponent>
    {
        private ApplicationContext _db;

        public ObtainedСomponentRepository(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<ObtainedComponent>> GetAllAsync()
        {
            return await _db.ObtainedСomponents
                .Include(c => c.Task)
                .Include(c => c.Component)
                .ToListAsync();
        }

        public async Task<ObtainedComponent> GetAsync(int id)
        {
            return await _db.ObtainedСomponents
                .Include(c => c.Task)
                .Include(c => c.Component)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IEnumerable<ObtainedComponent> Find(Func<ObtainedComponent, bool> predicate)
        {
            return _db.ObtainedСomponents
                .AsNoTracking()
                .Include(c => c.Task)
                .Include(c => c.Component)
                .Where(predicate).ToList();
        }

        public async Task CreateAsync(ObtainedComponent item)
        {
            await _db.ObtainedСomponents.AddAsync(item);
        }

        public void Update(ObtainedComponent item)
        {
            var obtComp = _db.ObtainedСomponents.FirstOrDefault(obtainedComponent => obtainedComponent.Id == item.Id);
            if (obtComp != null)
            {
                obtComp.Obtained = item.Obtained;
                _db.SaveChanges(); 
            }
            // _db.ObtainedСomponents.Update(item);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _db.ObtainedСomponents.FindAsync(id);
            if (item != null)
                _db.ObtainedСomponents.Remove(item);
        }
    }
}